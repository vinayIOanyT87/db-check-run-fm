using System;

namespace LogClient
{
	/// <summary>
	/// Summary description for LoggerImpl.
	/// </summary>
	public class LoggerImpl : System.MarshalByRefObject
	{
		#region Attributes
		internal static LogQueue queue;
		internal static LogWriter logWriter;

		private static int instances = 0;

		protected static System.DateTime mark;
		protected static bool sendDebug;
		protected static bool sendInfo;
		protected static bool sendPerformance;
		protected static bool sendWarn;
		protected static bool sendError;
		protected static bool sendCritical;

		#endregion Attributes

		static LoggerImpl()
		{
			queue = new LogQueue();
		}

		public LoggerImpl()
		{
//			System.Diagnostics.Debugger.Launch();

			instances++;
			if(logWriter == null)
			{
				logWriter = new LogWriter(this/*, queue*/);
				logWriter.CreateLog("LoggerImpl");
				logWriter.writeThread.Start();
			}
		}

		public void Start()
		{
			logWriter.bStop = false;
			Log("LoggerImpl", LogClient.LogLevel.DEBUG, "LoggerImpl.Start()");
		}

		public void Stop()
		{
			logWriter.bStop = true;
			Log("LoggerImpl", LogClient.LogLevel.DEBUG, "LoggerImpl.Stop()");
			logWriter.writeThread.Join();
		}

		//[System.Runtime.Remoting.Messaging.OneWay]
		public void Log(string appName, LogLevel level, string message)
		{
			GetLogLevel();
			bool logMessage = false;
			switch(level)
			{
				case LogLevel.DEBUG:
					logMessage = sendDebug;
					break;
				case LogLevel.INFO:
					logMessage = sendInfo;
					break;
				case LogLevel.PERFORM:
					logMessage = sendPerformance;
					break;
				case LogLevel.WARN:
					logMessage = sendWarn;
					break;
				case LogLevel.ERROR:
					logMessage = sendError;
					break;
				case LogLevel.CRITICAL:
					logMessage = sendCritical;
					break;
			}
			if(logMessage == true)
			{
				lock(logWriter.lockHandle)
				{
					queue.Enqueue(new LogMessage(appName, level, message, System.DateTime.Now));
				}
			}
		}
		
//		[System.Runtime.Remoting.Messaging.OneWay]
		public int CreateLog(string appName)
		{
			if(appName != "LoggerImpl")
			{
				Log("LoggerImpl", LogClient.LogLevel.DEBUG, "LoggerImpl.CreateLog(" + appName + ")");
			}
			logWriter.CreateLog(appName);
			return 0;
		}

		internal void RemoveLog(string appName)
		{
			if(appName != "LoggerImpl")
			{
				Log("LoggerImpl", LogClient.LogLevel.DEBUG, "LoggerImpl.RemoveLog(" + appName + ")");
			}
			logWriter.RemoveLog(appName);
		}

		protected void GetLogLevel()
		{
			const long waitSeconds = 10;
			System.TimeSpan span = System.DateTime.Now - mark;
			if(span.Seconds < waitSeconds) return;
			mark = System.DateTime.Now;

			bool oldDebug = sendDebug;
			bool oldInfo = sendInfo;
			bool oldPerformance = sendPerformance;
			bool oldWarn = sendWarn;
			bool oldError = sendError;
			bool oldCritical = sendCritical;

			Microsoft.Win32.RegistryKey Key =
				Microsoft.Win32.Registry.LocalMachine.OpenSubKey("Software\\Varec\\Logger",true);
			if(Key != null)
			{
				sendDebug = (int) (Key.GetValue("Debug", 0)) > 0;
				sendInfo = (int) (Key.GetValue("Info", 0)) > 0;
				sendPerformance = (int) (Key.GetValue("Performance", 0)) > 0;
				sendWarn = (int) (Key.GetValue("Warn", 1)) > 0;
				sendError = (int) (Key.GetValue("Error", 1)) > 0;
				sendCritical = (int) (Key.GetValue("Critical", 1)) > 0;
			}
			else
			{
				sendDebug = false;
				sendInfo = false;
				sendPerformance = false;
				sendWarn = true;
				sendError = true;
				sendCritical = true;
			}
			
			if(oldDebug != sendDebug)
			{
				Log("LoggerImpl", LogClient.LogLevel.INFO, (sendDebug ? "Start" : "Stopp") + "ing DEBUG logs.");
			}
			if(oldInfo != sendInfo)
			{
				Log("LoggerImpl", LogClient.LogLevel.INFO, (sendInfo ? "Start" : "Stopp") + "ing INFO logs.");
			}
			if(oldPerformance != sendPerformance)
			{
				Log("LoggerImpl", LogClient.LogLevel.INFO, (sendPerformance ? "Start" : "Stopp") + "ing PERFORM logs.");
			}
			if(oldWarn != sendWarn)
			{
				Log("LoggerImpl", LogClient.LogLevel.INFO, (sendWarn ? "Start" : "Stopp") + "ing WARN logs.");
			}
			if(oldError != sendError)
			{
				Log("LoggerImpl", LogClient.LogLevel.INFO, (sendError ? "Start" : "Stopp") + "ing ERROR logs.");
			}
			if(oldCritical != sendCritical)
			{
				Log("LoggerImpl", LogClient.LogLevel.INFO, (sendCritical ? "Start" : "Stopp") + "ing CRITICAL logs.");
			}
		}
	}
}
