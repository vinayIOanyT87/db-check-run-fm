// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FuelsManagerServiceLogger.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Writes exceptions and messages from the FuelsManager Service to the event log
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FuelsManagerService
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Writes exceptions and messages from the FuelsManager Service to the event log
	/// </summary>
	public sealed class FuelsManagerServiceLogger
	{
		/// <summary>
		/// The singleton instance
		/// </summary>
		private static readonly FuelsManagerServiceLogger instance = new FuelsManagerServiceLogger();

		/// <summary>
		/// Default constructor. 
		/// </summary>
		private FuelsManagerServiceLogger()
		{
		}

		/// <summary>
		/// Get the singleton instance
		/// </summary>
		public static FuelsManagerServiceLogger Instance
		{
			get
			{
				return FuelsManagerServiceLogger.instance;
			}
		}

		/// <summary>
		/// Log a message to the event log or to the trace if running in Azure
		/// </summary>
		/// <param name="message">The details to log</param>
		/// <param name="logType">The type of log record to create (warning, error)</param>
		private void LogEvent(string message, EventLogEntryType logType)
		{
			if (logType == EventLogEntryType.Error)
			{
				Trace.TraceError(message);
			}
			else if (logType == EventLogEntryType.Warning)
			{
				Trace.TraceWarning(message);
			}

			using (EventLog eventLog = new EventLog("Application", ".", "FuelsManagerService"))
			{
				eventLog.WriteEntry(message, logType);
			}
		}

		/// <summary>
		/// Log an exception as an error
		/// </summary>
		/// <param name="ex">The exception to log</param>
		public void LogError(Exception ex)
		{
			this.LogEvent(ex.ToString(), EventLogEntryType.Error);
		}

		/// <summary>
		/// Log a message as an error
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogError(string message)
		{
			this.LogEvent(message, EventLogEntryType.Error);
		}

		/// <summary>
		/// Log an Exception as a warning
		/// </summary>
		/// <param name="ex">The exception to log</param>
		public void LogWarning(Exception ex)
		{
			this.LogEvent(ex.ToString(), EventLogEntryType.Warning);
		}

		/// <summary>
		/// Log a message as a warning
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogWarning(string message)
		{
			this.LogEvent(message, EventLogEntryType.Warning);
		}

		/// <summary>
		/// Log a message to the trace
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogTrace(string message)
		{
			Trace.TraceInformation(message);
		}
	}
}
