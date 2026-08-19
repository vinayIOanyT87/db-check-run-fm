using System;

using LogClient;

namespace LogClient
{
	/// <summary>
	/// Summary description for LogMessage.
	/// </summary>
	public class LogMessage
	{
		#region Attributes
		protected string appName;
		protected LogLevel logLevel;
		protected string message;
		protected System.DateTime time;
		#endregion Attributes

		#region Properties
		public string AppName
		{
			get { return appName; }
			set { appName = value; }
		}
		public LogLevel LogLevel
		{
			get { return logLevel; }
			set { logLevel = value; }
		}
		public string Message
		{
			get { return message; }
			set { message = value; }
		}
		public System.DateTime Time
		{
			get { return time; }
			set { time = value; }
		}
		#endregion Properties

		public LogMessage(string appName, LogLevel logLevel, string message, System.DateTime time)
		{
			this.AppName = appName;
			this.logLevel = logLevel;
			this.message = message;
			this.time = time;
		}
		public LogMessage()
		{
		}
	}
}
