namespace FCEEService
{
    using System;
    using System.Configuration;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Management;
    using System.ServiceProcess;
    using System.IO;
    using System.Net;
    using System.Text.Json;
    using System.Threading.Tasks;
    using InProcLogging;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.DataObjects;
    using FMBusinessObjects.Constants;
    using Opc.Ua;

    public partial class FCEEServer : ServiceBase
    {
        private readonly HttpListener httpListener;
        private const int HandlerThread = 2;
        private static readonly EventLogger EventLogger = new EventLogger();
        private readonly string[] hex = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        private bool messageLogging;
        private static bool pointStatusProcessing;

        public FCEEServer(HttpListener listener, string url)
        {
            AutoLog = false;

            this.httpListener = listener;
            try
            {
                string[] prefixes = url.Split(',');
                if (prefixes.Length == 0)
                {
                    throw (new Exception("Invalid Url"));
                }

                foreach (var prefix in prefixes)
                {
                    if (prefix.ToUpper().StartsWith("HTTP"))
                    {
                        httpListener.Prefixes.Add(prefix);
                    }
                }
            }
            catch (Exception e)
            {
                EventLogger.Error(e.Message);
            }

            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());

            messageLogging = Convert.ToBoolean(ConfigurationManager.AppSettings["MessageLogging"] == null ? "false" : ConfigurationManager.AppSettings["MessageLogging"]);
            pointStatusProcessing = Convert.ToBoolean(ConfigurationManager.AppSettings["PointStatusProcessing"] == null ? "false" : ConfigurationManager.AppSettings["PointStatusProcessing"]);


            if (httpListener.IsListening)
                return;

            httpListener.Start();

            Logger.InitializeFileLogger(AppDomain.CurrentDomain.BaseDirectory + "\\" + this.GetServiceName() + ".log", 10000, LogSeverity.Debug, System.Threading.ThreadPriority.Normal);
            Logger.LogInfo(this.GetServiceName() + " Started");
            Logger.Flush();

            for (int i = 0; i < HandlerThread; i++)
            {
                httpListener.GetContextAsync().ContinueWith(ProcessRequestHandler);
            }

            EventLogger.Info(this.GetServiceName() + " Started");

        }

        protected override void OnStop()
        {
            if (httpListener.IsListening)
                httpListener.Stop();

            Logger.LogInfo(this.GetServiceName() + " Stopped");
            Logger.Flush();

            EventLogger.Info(this.GetServiceName() + " Stopped");
        }

        private void ProcessRequestHandler(Task<HttpListenerContext> result)
        {
            if (!httpListener.IsListening)
                return;

            var context = result.Result;
            var request = context.Request;

            // Start new listener which replaces this
            httpListener.GetContextAsync().ContinueWith(ProcessRequestHandler);

            try
            {
                var security = new SecurityClass();
                security.UserID = "FCEEService";
                security.SiteID = "SiteAdmin";
                security.SiteGuid = Guids.SiteAdminGuid;
                using (var memoryStream = new MemoryStream())
                {
                    request.InputStream.CopyTo(memoryStream);

                    var msgBody = memoryStream.ToArray();
                    var logMessage = string.Empty;

                    for (var index = 0; index < msgBody.Length; index++)
                    {
                        var msgByte = msgBody[index];
                        logMessage += hex[(msgByte & 0xF0) >> 4];
                        logMessage += hex[msgByte & 0x0F];
                        logMessage += " ";
                    }

                    if (messageLogging)
                    {
                        Logger.LogInfo("Received Length " + Convert.ToString(msgBody.Length) + " Message: " + logMessage);
                    }

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var processRequestHandler = FMChannelHelper.MakeCall<IFCEEServiceManager, Tuple<bool, int, byte[]>>(x => x.ProcessRequestHandler(security, pointStatusProcessing, memoryStream, request.ContentType, request.HttpMethod));
                    var isSuccess = processRequestHandler.Item1;
                    var httpStatusCode = processRequestHandler.Item2;
                    var responseBody = processRequestHandler.Item3;

                    if (isSuccess)
                    {
                        var response = context.Response;
                        response.StatusCode = httpStatusCode;
                        response.ContentType = "text/plain";
                        response.ContentLength64 = responseBody.Length;
                        response.OutputStream.Write(responseBody, 0, responseBody.Length);
                        response.OutputStream.Close();
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError("Error processing request : " + e.Message);
            }
        }

        protected String GetServiceName()
        {
            // Calling System.ServiceProcess.ServiceBase::ServiceNamea allways returns
            // an empty string,
            // see https://connect.microsoft.com/VisualStudio/feedback/ViewFeedback.aspx?FeedbackID=387024

            // So we have to do some more work to find out our service name, this only works if
            // the process contains a single service, if there are more than one services hosted
            // in the process you will have to do something else

            int processId = Process.GetCurrentProcess().Id;
            String query = "SELECT * FROM Win32_Service where ProcessId = " + processId;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher(query);

            foreach (var obj in searcher.Get())
            {
                var queryObj = obj as ManagementObject;
                if (queryObj != null)
                {
                    return queryObj["Name"].ToString();
                }
            }

            return "FuelsManager Point Service";
        }
    }
}

