
namespace FMUAAlarmServer
{

    using System;

    using System.ServiceProcess;

    using Softing.Opc.Ua.Sdk;

    using Softing.Opc.Ua.Sdk.Configuration;
    using System.Net;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Management;
    using System.IO;
    using InProcLogging;
    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;

    public partial class FMUAAlarmServerService : ServiceBase
    {

        private EventLog eventLog;
        public FMUAAlarmServerService()
        {
            this.AutoLog = false;
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
            InitializeComponent();
            this.eventLog = new EventLog("Application", ".", this.ServiceName);
        }


        protected override void OnStart(string[] args)
        {
            //System.Threading.Thread.CurrentThread.ApartmentState = System.Threading.ApartmentState.STA;
            this.MyStart(Environment.GetCommandLineArgs());
            this.eventLog.WriteEntry(FMUAAlarmServerService.GetServiceName() + " Started", EventLogEntryType.Information);
        }

        protected override void OnStop()
        {
            Logger.LogCritical(this.ServiceName + " Stopping!!!");
            Logger.Flush();
            Logger.Shutdown();
            this.eventLog.WriteEntry(FMUAAlarmServerService.GetServiceName() + " Stopped", EventLogEntryType.Information);
        }

        protected AlarmsServer Serv;


        public String GetLocalHostName()
        {
            // Get the local computer host name.
            String hostName = Dns.GetHostName();
            hostName = hostName.ToLower();
            Console.WriteLine("Computer name :" + hostName);
            Logger.LogDebug("Computer name :" + hostName);
            return hostName;
        }

        protected int GetPortNumFromConnString(string connString)
        {
            int endIndex = connString.IndexOf("/alarmsserver/none");
            if(endIndex == -1)
            {
                return -1;
            }
            string shortConnString = connString.Substring(0, endIndex);
            int startIndex = shortConnString.LastIndexOf(':');
            if(startIndex == -1 || startIndex >= shortConnString.Length)
            {
                return -1;
            }
            string portString = shortConnString.Substring(startIndex + 1);
            int port = -1;
            try
            {
                port = Convert.ToInt32(portString);
            }
            catch(Exception)
            {
                return -1;
            }
            return port;
        }

        protected int FindLeastPortFromConnectionString(List<string> opcUaConnectionStrings)
        {
            const int startingPortNum = 1000000;
            int port = startingPortNum;
            foreach (var connString in opcUaConnectionStrings)
            {
                int tempPort = GetPortNumFromConnString(connString);
                if(tempPort != -1 && tempPort < port)
                {
                    port = tempPort;
                }
            }
            if(port == startingPortNum)
            {
                return -1;
            }
            return port;
        }


        public void MyStart(object argsObject)
        {
            string[] args = (string[])argsObject;
            try
            {
                FMChannelHelper.MakeCall<IHardwareKey, ushort>(x => x.CheckActivatedLicenceVersion());
                ApplicationInstance application = new ApplicationInstance();
                application.ApplicationType = ApplicationType.Server;
                application.ConfigSectionName = "FMUAAlarmServer";
                Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
                string logFileName = this.ServiceName;

                bool result;

                //	TODO - design time license activation
                //	Fill in your design time license activation keys here
                //
                //	NOTE: you can activate one or more features at the same time
                //	activate the Server feature				
                //  result = application.ActivateLicense(LicenseFeature.Server, "XXXX-XXXX-XXXX-XXXX-XXXX");
                result = application.ActivateLicense(LicenseFeature.Server, "0fb0-00d8-b497-952f-6897");

                if (!result)
                {
                    return;
                }

                // process and command line arguments.
                if (application.ProcessCommandLine(true, args))
                {
                    return;
                }



                string configFilePath = "";
                bool useExistingPoints = false;

                for (int i = 0; i < args.Length; i++)
                {
                    if (args[i] == "/config_file")
                    {
                        i++;
                        configFilePath = args[i];
                    }

                    if (args[i] == "/use_existing_points")
                    {
                        System.Console.WriteLine("Using Existing Points In Database!");
                        useExistingPoints = true;
                    }
                }

                if (configFilePath == null)
                {
                    configFilePath = "";
                }

                if (configFilePath != "")
                {
                    //	string removeSlashConfigFilePath = configFilePath.Replace('\\', '_');
                    //	removeSlashConfigFilePath = removeSlashConfigFilePath.Replace('/', '_');
                    //	removeSlashConfigFilePath = removeSlashConfigFilePath.Replace(':', '_');
                    //	char[] MyChar = { '\r', '\n' };
                    //	removeSlashConfigFilePath = removeSlashConfigFilePath.Trim(MyChar);
                    //	logFileName = logFileName + "_" + removeSlashConfigFilePath;
                    char[] MyChar = { '\r', '\n' };
                    configFilePath = configFilePath.Trim(MyChar);
                }

                // load the application configuration.
                if (configFilePath != null && configFilePath != "")
                {
                    while (true)
                    {
                        try
                        {
                            application.LoadApplicationConfiguration(configFilePath, true);
                            break;
                        }
                        catch (Exception lacEx)
                        {
                            System.Console.WriteLine("FMUAAlarmServerService.MyStart LoadApplicationConfiguration Exception: " + lacEx.Message);
                            Logger.LogError("FMUAAlarmServerService.MyStart LoadApplicationConfiguration Exception: " + lacEx.Message);
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }
                else
                {
                    while (true)
                    {
                        try
                        {
                            application.LoadApplicationConfiguration(true);
                            break;
                        }
                        catch (Exception lacEx)
                        {
                            System.Console.WriteLine("FMUAAlarmServerService.MyStart LoadApplicationConfiguration Exception: " + lacEx.Message);
                            Logger.LogError("FMUAAlarmServerService.MyStart LoadApplicationConfiguration Exception: " + lacEx.Message);
                            System.Threading.Thread.Sleep(1000);
                        }
                    }
                }

                string hostName = GetLocalHostName();
                List<string> opcUaConnectionStrings = new List<string>();
                const string localHostStr = "localhost";
                foreach (string t in application.ApplicationConfiguration.ServerConfiguration.BaseAddresses)
                {
                    var lowerCaseT = t.ToLower() + "/none";
                    if (lowerCaseT.Contains(localHostStr))
                    {
                        var hostnameT = lowerCaseT.Replace(localHostStr, hostName);
                        opcUaConnectionStrings.Add(hostnameT);
                    }
                    else
                    {
                        opcUaConnectionStrings.Add(lowerCaseT);
                    }
                }

                int connLeastPort = FindLeastPortFromConnectionString(opcUaConnectionStrings);
                if(connLeastPort == -1)
                {
                    connLeastPort = Process.GetCurrentProcess().Id;
                }

                


                Logger.InitializeFileLogger(AppDomain.CurrentDomain.BaseDirectory + "\\" + FMUAAlarmServerService.GetServiceName() + "_" + connLeastPort + ".log", 10000, LogSeverity.Debug, System.Threading.ThreadPriority.Normal);
                Logger.LogCritical(this.ServiceName + " Started!!!");
                Logger.Flush();

                Logger.LogDebug("Config File " + configFilePath);
                Logger.LogDebug("Using Existing Points In Database is " + (useExistingPoints == true ? "true" : "false"));

                //Application.Configuration.ApplicationName = "Shawn Alarm Server";
                //Application.Configuration.Security.ApplicationCertificateSubject = Application.Configuration.ApplicationName;

                // check the application certificate.	
                try
                {
                    application.CheckApplicationInstanceCertificate(false, 0);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Logger.LogError("FMUAAlarmServerService.MyStart Exception " + ex.Message);
                }


                this.Serv = new AlarmsServer();

                // start the server.
                application.Start(this.Serv);

                foreach (var connString in opcUaConnectionStrings)
                {
                    Console.WriteLine(connString);
                    Logger.LogDebug(connString);
                }

                if (useExistingPoints)
                {
                    ConfigureExistingPoints cep = new ConfigureExistingPoints(opcUaConnectionStrings);
                    cep.Configure();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.LogError("FMUAAlarmServerService.MyStart Outter Exception " + e.Message);
            }
        }

        public static String GetServiceName()
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

            return "FMUAAlarmServerService";
        }
    }


    /// <summary>
    /// The <b>AlarmsServer</b> namespace contains classes which implement a Sample Server.
    /// </summary>
    /// <exclude/>
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class NamespaceDoc
    {
    }
}
