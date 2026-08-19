// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventLogging.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   This EventLogging class wraps the functionality to write to the event log.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.UtilityObjects
{
	using System;
	using System.Diagnostics;

	using FMBusinessObjects.Constants;

	/// <summary>
	/// This EventLogging class wraps the functionality to write to the event log.
	/// </summary>
	public class EventLogging
	{
		#region Private attributes
		/// <summary>
		/// The event source.
		/// </summary>
		private const string EventSource = "FuelsManager";

		/// <summary>
		/// The event log name.
		/// </summary>
		private string eventLogName = "Application";

		/// <summary>
		/// The error constants.
		/// </summary>
		private ErrorConstants errorConstants;

		/// <summary>
		/// The event log.
		/// </summary>
		private EventLog eventLog;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogging"/> class.
		/// </summary>
		public EventLogging( )
		{
			this.Initialize( );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="EventLogging"/> class.
		/// </summary>
		/// <param name="logName">
		/// The log name.
		/// </param>
		public EventLogging(string logName)
		{
			this.Initialize( );

			if ( string.IsNullOrEmpty(logName) == false )
			{
				this.eventLogName = logName;
			}
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value: This property returns true if there is an error writing to the log.
		/// Otherwise, it returns false.
		/// </summary>
		public bool ErrorFlag
		{
			get { return this.errorConstants.ErrorFlag; }
		}

		/// <summary>
		/// Gets a value: This property returns the error message if there was an error.
		/// Otherwise, it returns null.
		/// </summary>
		public string ErrorMessage
		{
			get { return this.errorConstants.ErrorMessage; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method accepts a string message and writes it to the
		/// specified log.
		/// </summary>
		/// <param name="msg">
		/// The message.
		/// </param>
		/// <param name="evntLogType">
		/// The event log type.
		/// </param>
		public void LogEvent(string msg, EventLogEntryType evntLogType)
		{
			if ( string.IsNullOrEmpty(msg) == false )
			{
				try
				{
					this.eventLog.WriteEntry(msg, evntLogType);
				}
				catch ( Exception ex )
				{
					const string Message = "Could not write to the Event log. ";
					this.errorConstants.ClearErrors( );
					this.errorConstants.AppendErrors(Message + ex.Message);
				}
			}
		}
		#endregion

		#region Private Methods
		/// <summary>
		/// This method initializes the EventLogging object to its initial state. It creates
		/// an event log for IDE.
		/// </summary>
		private void Initialize( )
		{
			this.errorConstants = new ErrorConstants( );

			if ( EventLog.SourceExists(EventSource) == false )
			{
				var sourceData = new EventSourceCreationData(EventSource, this.eventLogName);
				EventLog.CreateEventSource(sourceData);
			}

			this.eventLog = new EventLog { Source = EventSource, Log = this.eventLogName };
		}
		#endregion
	}
}

