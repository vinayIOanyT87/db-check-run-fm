///<summary>
/// FILE NAME:	Logger.cs
/// PURPOSE:	Logger Class
///	COMMENTS:
///		Copyright (C) E+H Systems & Gauging, Inc. Norcross, GA, USA, 2000
///		This file shall not be copied or reproduced in any form without
///		the express written consent of Endress+Hauser.
///
///	AUTHOR(S):	W. Gray
///	VERSION:	1.0.0  Current version
///
///	MODIFICATION HISTORY:
/// Date:		By:			Reason:
/// ---------	----------  -------------------------------------------
/// 12/04/2008	W.Gray		7.4.6.0 - Revised to not catch exceptions in Init
///							and to catch only socketException in Log.  This
///							is to ensure that logging problems can be reported
///							by the system. (CSI 6323)
///</summary>

using System;
using System.ServiceProcess;
using System.Security;

namespace LogClient
{
    [SecuritySafeCriticalAttribute]
    public class Logger : ILogger
    {
        #region Attributes
        string appName;

        protected static Object loggerImpl;  //>>>>>>>>>>>>>>>>>>>>>>>>>>>>>HBNote:  loggerImpl must be of the WCF service interface type. Used System.Object for now to allow code to compile.

        protected static string url;
        protected static System.DateTime mark;
        protected static bool sendDebug;
        protected static bool sendInfo;
        protected static bool sendPerformance;
        protected static bool sendWarn;
        protected static bool sendError;
        protected static bool sendCritical;
        #endregion Attributes

        static Logger()
        {
            Init();
        }

        public Logger(string appName)
        {
            this.appName = appName;
            Init();
        }

        public static void Init()
        {
            string hostname = null;
            int port = 0;
            const long waitSeconds = 10;
            System.TimeSpan span = System.DateTime.Now - mark;
            if (span.Seconds < waitSeconds) return;
            mark = System.DateTime.Now;

            sendDebug = AppSettingsHelper.GetKeyValue<bool>("LoggerDebugFlag", false);
            sendInfo = AppSettingsHelper.GetKeyValue<bool>("LoggerInfoFlag", false);
            sendPerformance = AppSettingsHelper.GetKeyValue<bool>("LoggerPerformanceFlag", false);
            sendWarn = AppSettingsHelper.GetKeyValue<bool>("LoggerWarnFlag", true);
            sendError = AppSettingsHelper.GetKeyValue<bool>("LoggerErrorFlag", true);
            sendCritical = AppSettingsHelper.GetKeyValue<bool>("LoggerCriticalFlag", true);
            hostname = AppSettingsHelper.GetKeyValue<string>("LoggerHostName", "127.0.0.1");
            port = AppSettingsHelper.GetKeyValue<int>("LoggerPort", 8086);

            if (loggerImpl != null)
                return;

            //>>>>>>>>>>>>>>>>>>>>>>>>HBNote: loggerImpl = create proxy to WCF Service here

        }

        #region ILogger Members
        [SecuritySafeCritical]
        public void Log(LogClient.LogLevel level, string message)
        {
            Init();
            if (loggerImpl != null)
            {
                ServiceController serviceController = null;
                try
                {
                    serviceController = new ServiceController("FuelsManager Service");
                    var testStatus = serviceController.Status;
                }
                catch (InvalidOperationException)
                {
                    // This means the service was not found installed on the system
                    serviceController = null;
                }

                if (serviceController != null
                   && serviceController.Status == ServiceControllerStatus.Running)
                {
                    try
                    {
                        //loggerImpl.Log(appName, level, message);   //HBNote: Uncomment line once we have a proxy for the Logger service
                    }
                    catch (System.Net.Sockets.SocketException)
                    {
                        loggerImpl = null;
                    }
                    catch (Exception e)
                    {
                        loggerImpl = null;
                        throw e;
                    }
                }
            }
        }

        public void Debug(string message)
        {
            if (sendDebug == false) return;
            Log(LogClient.LogLevel.DEBUG, message);
        }

        public void Perform(string message)
        {
            if (sendPerformance == false) return;
            Log(LogClient.LogLevel.PERFORM, message);
        }

        public void Info(string message)
        {
            if (sendInfo == false) return;
            Log(LogClient.LogLevel.INFO, message);
        }

        public void Warn(string message)
        {
            if (sendWarn == false) return;
            Log(LogClient.LogLevel.WARN, message);
        }

        public void Error(string message)
        {
            if (sendError == false) return;
            Log(LogClient.LogLevel.ERROR, message);
        }

        public void Critical(string message)
        {
            if (sendCritical == false) return;
            Log(LogClient.LogLevel.CRITICAL, message);
        }
        #endregion


    }


}
