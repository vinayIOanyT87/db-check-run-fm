namespace InProcLogging
{
	using System.Diagnostics;
	using System.Threading;

	public class Logger
	{
		public enum LoggerTypes
		{
			NullLogger,
			FileLogger,
			DebugLogger
		}
		
		private static ILogger logger;

		#region Methods
	
		static Logger()
		{			
			Initialize(LoggerTypes.NullLogger);
		}

		private static string lastDebug { get; set; }

		private static string lastError { get; set; }

		public static void InitializeFileLogger(string logFileName, int maxLogFileSizeKb, 
										LogSeverity loggingLevel, ThreadPriority priority)
		{
			Initialize(LoggerTypes.FileLogger, new object[]
				{logFileName, maxLogFileSizeKb, loggingLevel, priority});
		}

		public static void Initialize(LoggerTypes loggerType, params object[] args)
		{
			switch(loggerType)
			{
				case LoggerTypes.FileLogger:
					Debug.Assert(args != null && args.Length >= 4);
					logger = new FileLogger(args); 
					break;
				case LoggerTypes.DebugLogger:
					logger = new DebugLogger(); 
					break;
				case LoggerTypes.NullLogger:
				default:
					logger = new NullLogger(); 
					break;
			}
		}

		public static void Log(LogSeverity severity, string message)
		{
			logger.Log(severity, message);
		}

		public static void Log(LogSeverity severity, string message, params object[] args)
		{
			logger.Log(severity, message, args);
		}

		public static void LogDebug(string message)
		{
			if (message == lastDebug)
			{
				return; // successive redundant debug level messages are filtered
			}

			lastError = string.Empty;
			lastDebug = message;
			logger.Log(LogSeverity.Debug, message);
		}

		public static void LogInfo(string message)
		{
			lastDebug = string.Empty;
			lastError = string.Empty;
			logger.Log(LogSeverity.Info, message);
		}

		public static void LogStatus(string message)
		{
			lastDebug = string.Empty;
			lastError = string.Empty;
			logger.Log(LogSeverity.Status, message);
		}

		public static void LogWarning(string message)
		{
			lastDebug = string.Empty;
			lastError = string.Empty;
			logger.Log(LogSeverity.Warning, message);
		}

		public static void LogError(string message)
		{
			if (message == lastError)
			{
				return; // successive redundant error level messages are filtered
			}
			lastDebug = string.Empty;
			lastError = message;
			logger.Log(LogSeverity.Error, message);
		}

		public static void LogCritical(string message)
		{
			lastDebug = string.Empty;
			lastError = string.Empty;
			logger.Log(LogSeverity.Critical, message);
		}

		public static void LogFatal(string message)
		{
			lastDebug = string.Empty;
			lastError = string.Empty;
			logger.Log(LogSeverity.Fatal, message);
		}

		public static void LogDebug(string message, params object[] args)
		{
			LogDebug(string.Format(message, args));
		}

		public static void LogInfo(string message, params object[] args)
		{
			LogInfo(string.Format(message, args));
		}

		public static void LogStatus(string message, params object[] args)
		{
			LogStatus(string.Format(message, args));
		}

		public static void LogWarning(string message, params object[] args)
		{
			LogWarning(string.Format(message, args));
		}

		public static void LogError(string message, params object[] args)
		{
			LogError(string.Format(message, args));
		}

		public static void LogCritical(string message, params object[] args)
		{
			LogCritical(string.Format(message, args));
		}

		public static void LogFatal(string message, params object[] args)
		{
			LogFatal(string.Format(message, args));
		}

		public static void Shutdown()
		{
			ILogger temp = logger;
			logger = new NullLogger();
			temp.Dispose();
		}

		#endregion

		public static void Flush()
		{
			logger.Flush();
		}
	}
}

