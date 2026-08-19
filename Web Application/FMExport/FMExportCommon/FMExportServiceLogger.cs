// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FMExportServiceLogger.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Writes exceptions and messages from the FMExport service to the event log
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMExportService
{
	using System;
	using System.Diagnostics;

	/// <summary>
	/// Writes exceptions and messages from the FMExport service to the event log
	/// </summary>
	public sealed class FMExportServiceLogger
	{
		/// <summary>
		/// The event log name.
		/// </summary>
		private const string EventLogName = "Application";

		/// <summary>
		/// The event source name for the FMExport service.
		/// </summary>
		private const string EventSourceName = "FMExportService";

		/// <summary>
		/// The lazy singleton instance
		/// </summary>
		private static readonly Lazy<FMExportServiceLogger> LazyInstance =
			new Lazy<FMExportServiceLogger>(() => new FMExportServiceLogger());

		/// <summary>
		/// The event log.
		/// </summary>
		private EventLog eventLog;

		/// <summary>
		/// Prevents a default instance of the FMExportServiceLogger class from being created.
		/// </summary>
		private FMExportServiceLogger()
		{
			this.Initialized = false;
		}

		/// <summary>
		/// Gets the instance.
		/// </summary>
		public static FMExportServiceLogger Instance
		{
			get
			{
				if (!LazyInstance.Value.Initialized)
				{
					bool eventSourceCreated = CreateEventSource();
					if (eventSourceCreated)
					{
						LazyInstance.Value.Initialized = true;
						LazyInstance.Value.eventLog = new EventLog(EventLogName, ".", EventSourceName);
					}
				}

				return LazyInstance.Value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether initialized.
		/// </summary>
		public bool Initialized { get; private set; }

		/// <summary>
		/// Create the event source if it does not already exist.
		/// </summary>
		/// <returns>True if the event source exists or is created successfully</returns>
		private static bool CreateEventSource()
		{
			if (!EventLog.SourceExists(EventSourceName))
			{
				try
				{
					EventLog.CreateEventSource(EventSourceName, EventLogName);
					System.Threading.Thread.Sleep(500);
				}
				catch (Exception ex)
				{
					var log = new EventLog(EventLogName);
					log.WriteEntry(
						"Unable to create event source for " + EventSourceName + ".\n" + ex.ToString(),
						EventLogEntryType.Error,
						999);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Log a message to the event log or to the trace if running in Azure
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="logType">The type of log record to create (Information, Warning, Error)</param>
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

			this.eventLog.WriteEntry(message, logType);
		}

		/// <summary>
		/// Log a message to the event log or to the trace if running in Azure
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="logType">The type of log record to create (Information, Warning, Error)</param>
		/// <param name="eventId">The application specific identifier for the event</param>
		private void LogEvent(string message, EventLogEntryType logType, int eventId)
		{
			if (logType == EventLogEntryType.Error)
			{
				Trace.TraceError(message);
			}
			else if (logType == EventLogEntryType.Warning)
			{
				Trace.TraceWarning(message);
			}

			this.eventLog.WriteEntry(message, logType, eventId);
		}

		/// <summary>
		/// Log a message to the event log as an error
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogError(string message)
		{
			this.LogEvent(message, EventLogEntryType.Error);
		}

		/// <summary>
		/// Log a message to the event log as an error
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventId">The application specific identifier for the event</param>
		public void LogError(string message, int eventId)
		{
			this.LogEvent(message, EventLogEntryType.Error, eventId);
		}

		/// <summary>
		/// Log a message to the event log as a warning
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogWarning(string message)
		{
			this.LogEvent(message, EventLogEntryType.Warning);
		}

		/// <summary>
		/// Log a message to the event log as a warning
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventId">The application specific identifier for the event</param>
		public void LogWarning(string message, int eventId)
		{
			this.LogEvent(message, EventLogEntryType.Warning, eventId);
		}

		/// <summary>
		/// Log a message to the event log as an information event
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogInfo(string message)
		{
			this.LogEvent(message, EventLogEntryType.Information);
		}

		/// <summary>
		/// Log a message to the event log as an information event
		/// </summary>
		/// <param name="message">The message to log</param>
		/// <param name="eventId">The application specific identifier for the event</param>
		public void LogInfo(string message, int eventId)
		{
			this.LogEvent(message, EventLogEntryType.Information, eventId);
		}

		/// <summary>
		/// Log a message to the trace log
		/// </summary>
		/// <param name="message">The message to log</param>
		public void LogTrace(string message)
		{
			Trace.TraceInformation(message);
		}
	}
}
