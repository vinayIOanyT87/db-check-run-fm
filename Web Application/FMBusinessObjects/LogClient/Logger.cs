// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Logger.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the Logger type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.LogClient
{
	using System;
	using System.Diagnostics;
	using System.Security;

	using FMBusinessObjects.BusinessInterfaces;
	using FMBusinessObjects.ChannelFactories;
	using FMBusinessObjects.UtilityObjects;

	[SecuritySafeCritical]
	public class Logger : ILogger
	{
		#region Constants and Fields

		protected static DateTimeOffset mark;

		protected static bool sendCritical;

		protected static bool sendDebug;

		protected static bool sendError;

		protected static bool sendInfo;

		protected static bool sendPerformance;

		protected static bool sendWarn;

		protected static string url;

		private readonly string appName;

		#endregion

		#region Constructors and Destructors

		/// <summary>
		/// Initializes static members of the <see cref="Logger"/> class.
		/// </summary>
		static Logger()
		{
			Init();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Logger"/> class.
		/// </summary>
		/// <param name="appName">Name of the app.</param>
		public Logger(string appName)
		{
			this.appName = appName;
			Init();
		}

		#endregion

		#region Public Methods and Operators

		/// <summary>
		/// Inits this instance.
		/// </summary>
		public static void Init()
		{
			const long WaitSeconds = 10;

			TimeSpan span = DateTimeOffset.Now - mark;
			if (span.Seconds < WaitSeconds)
			{
				return;
			}

			mark = DateTimeOffset.Now;

			sendDebug = AppSettingsHelper.GetKeyValue("LoggerDebugFlag", false);
			sendInfo = AppSettingsHelper.GetKeyValue("LoggerInfoFlag", false);
			sendPerformance = AppSettingsHelper.GetKeyValue("LoggerPerformanceFlag", false);
			sendWarn = AppSettingsHelper.GetKeyValue("LoggerWarnFlag", true);
			sendError = AppSettingsHelper.GetKeyValue("LoggerErrorFlag", true);
			sendCritical = AppSettingsHelper.GetKeyValue("LoggerCriticalFlag", true);

			// hostname = AppSettingsHelper.GetKeyValue<string>("LoggerHostName", "127.0.0.1");
			// port = AppSettingsHelper.GetKeyValue<int>("LoggerPort", 8086);
		}

		/// <summary>
		/// Logs a critical message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Critical(string message)
		{
			if (sendCritical == false)
			{
				return;
			}

			this.Log(LogLevel.CRITICAL, message);
		}

		/// <summary>
		/// Logs a debug message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Debug(string message)
		{
			if (sendDebug == false)
			{
				return;
			}

			this.Log(LogLevel.DEBUG, message);
		}

		/// <summary>
		/// Logs an error message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Error(string message)
		{
			if (sendError == false)
			{
				return;
			}

			this.Log(LogLevel.ERROR, message);
		}

		/// <summary>
		/// Logs an info message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Info(string message)
		{
			if (sendInfo == false)
			{
				return;
			}

			this.Log(LogLevel.INFO, message);
		}

		/// <summary>
		/// Logs a message of the specified level.
		/// </summary>
		/// <param name="level">The level.</param>
		/// <param name="message">The message.</param>
		[SecuritySafeCritical]
		public void Log(LogLevel level, string message)
		{
			try
			{
				Init();

				FMChannelHelper.MakeCall<ILoggerService>(loggerChannel => { loggerChannel.Log(this.appName, level, message); });
			}
			catch (Exception error)
			{
				System.Diagnostics.Trace.TraceError(error.ToString());
				switch (level)
				{
					case LogLevel.INFO:
					case LogLevel.PERFORM:
						Trace.TraceInformation(message);
						break;
					case LogLevel.WARN:
						Trace.TraceWarning(message);
						break;
					case LogLevel.DEBUG:
						Trace.WriteLine(message);
						break;
					//case LogLevel.CRITICAL:
					//case LogLevel.ERROR:
					default:
						Trace.TraceError(message);
						break;
				}
				
			}
		}

		/// <summary>
		/// Logs a performance message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Perform(string message)
		{
			if (sendPerformance == false)
			{
				return;
			}

			this.Log(LogLevel.PERFORM, message);
		}

		/// <summary>
		/// Logs a warning message.
		/// </summary>
		/// <param name="message">The message.</param>
		public void Warn(string message)
		{
			if (sendWarn == false)
			{
				return;
			}

			this.Log(LogLevel.WARN, message);
		}

		#endregion
	}
}