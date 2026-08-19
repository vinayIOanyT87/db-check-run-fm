using System;

using FMBusinessObjects.LogClient;
using FMBusinessObjects.UtilityObjects;

namespace FMBusinessServices.InternalClasses.FMLogger
{
	/// <summary>
	/// Summary description for LoggerImpl.
	/// </summary>
	public class LoggerImpl
	{
		#region Attributes
		internal static LogQueue queue;
		internal static LogWriter logWriter;

		private static int instances = 0;

		protected static DateTimeOffset mark;
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
			if (logWriter == null)
			{
				logWriter = new LogWriter(this/*, queue*/);
				logWriter.CreateLog("LoggerImpl");
				logWriter.writeThread.Start();
			}
		}

		public void Start()
		{
			logWriter.bStop = false;
			Log("LoggerImpl", LogLevel.DEBUG, "LoggerImpl.Start()");
		}

		public void Stop()
		{
			logWriter.bStop = true;
			Log("LoggerImpl", LogLevel.DEBUG, "LoggerImpl.Stop()");
			logWriter.writeThread.Join();
		}

		//[System.Runtime.Remoting.Messaging.OneWay]
		public void Log(string appName, LogLevel level, string message)
		{
			GetLogLevel();
			bool logMessage = false;
			switch (level)
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
			if (logMessage == true)
			{
				lock (logWriter.lockHandle)
				{
					queue.Enqueue(new LogMessage(appName, level, message, DateTimeOffset.Now));
				}
			}
		}

		//		[System.Runtime.Remoting.Messaging.OneWay]
		public int CreateLog(string appName)
		{
			if (appName != "LoggerImpl")
			{
				Log("LoggerImpl", LogLevel.DEBUG, "LoggerImpl.CreateLog(" + appName + ")");
			}
			logWriter.CreateLog(appName);
			return 0;
		}

		internal void RemoveLog(string appName)
		{
			if (appName != "LoggerImpl")
			{
				Log("LoggerImpl", LogLevel.DEBUG, "LoggerImpl.RemoveLog(" + appName + ")");
			}
			logWriter.RemoveLog(appName);
		}

		protected void GetLogLevel()
		{
			const long waitSeconds = 10;
			TimeSpan span = DateTimeOffset.Now - mark;
			if (span.Seconds < waitSeconds) return;
			mark = DateTimeOffset.Now;

			bool oldDebug = sendDebug;
			bool oldInfo = sendInfo;
			bool oldPerformance = sendPerformance;
			bool oldWarn = sendWarn;
			bool oldError = sendError;
			bool oldCritical = sendCritical;

			sendDebug = AppSettingsHelper.GetKeyValue<bool>("LoggerDebugFlag", false);
			sendInfo = AppSettingsHelper.GetKeyValue<bool>("LoggerInfoFlag", false);
			sendPerformance = AppSettingsHelper.GetKeyValue<bool>("LoggerPerformanceFlag", false);
			sendWarn = AppSettingsHelper.GetKeyValue<bool>("LoggerWarnFlag", true);
			sendError = AppSettingsHelper.GetKeyValue<bool>("LoggerErrorFlag", true);
			sendCritical = AppSettingsHelper.GetKeyValue<bool>("LoggerCriticalFlag", true);

			if (oldDebug != sendDebug)
			{
				Log("LoggerImpl", LogLevel.INFO, (sendDebug ? "Start" : "Stopp") + "ing DEBUG logs.");
			}
			if (oldInfo != sendInfo)
			{
				Log("LoggerImpl", LogLevel.INFO, (sendInfo ? "Start" : "Stopp") + "ing INFO logs.");
			}
			if (oldPerformance != sendPerformance)
			{
				Log("LoggerImpl", LogLevel.INFO, (sendPerformance ? "Start" : "Stopp") + "ing PERFORM logs.");
			}
			if (oldWarn != sendWarn)
			{
				Log("LoggerImpl", LogLevel.INFO, (sendWarn ? "Start" : "Stopp") + "ing WARN logs.");
			}
			if (oldError != sendError)
			{
				Log("LoggerImpl", LogLevel.INFO, (sendError ? "Start" : "Stopp") + "ing ERROR logs.");
			}
			if (oldCritical != sendCritical)
			{
				Log("LoggerImpl", LogLevel.INFO, (sendCritical ? "Start" : "Stopp") + "ing CRITICAL logs.");
			}
		}
	}
}
