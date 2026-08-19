namespace FMPointService.Logging
{
	using System.Diagnostics;


	internal class EventLogger
	{
		private static readonly EventLog EventLog = new EventLog( "Application", ".", "FuelsManager Point Service" );

		public string ServiceName
		{
			get
			{
				return EventLog.Source;
			}
			set
			{
				EventLog.Source = value;
			} 
		}

		private void WriteEntry(string entry, EventLogEntryType entryType)
		{
			EventLog.WriteEntry(entry, entryType);
		}

		public void Error(string entry)
		{
			this.WriteEntry(entry, EventLogEntryType.Error);
		}

		public void Info(string entry)
		{
			this.WriteEntry( entry, EventLogEntryType.Information );
		}

		public void Warning(string entry)
		{
			this.WriteEntry( entry, EventLogEntryType.Warning );
		}
	}
}
