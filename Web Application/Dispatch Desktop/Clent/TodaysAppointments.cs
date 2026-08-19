namespace Dispatch
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Summary description for TodaysAppointmentsCollectionClass.
	/// </summary>
	[Serializable()]
	public class TodaysAppointmentsCollectionClass : List<TodaysAppointmentClass>
	{
	}

	public class TodaysAppointmentClass
	{
		public Guid IdentityGuid;
		public string Description;
		public string AppointmentCategory;
		public int Duration;
		public DateTimeOffset DueDate = new DateTimeOffset();
		public string AssetText;
		public bool DoNotNotifyAgain;
		public bool InSleepMode;
		public int NumberToSleep;
		public DateTimeOffset SleepTimeInterval = new DateTimeOffset();
		public bool AppointmentIsDue;

		public TodaysAppointmentClass()
		{
			this.Description = string.Empty;
			this.AppointmentCategory = string.Empty;
			this.Duration = 0;
			this.DueDate = DateTime.Now;
			this.AssetText = string.Empty;
			this.DoNotNotifyAgain = false;
			this.InSleepMode = false;
			this.NumberToSleep = 15;
			this.SleepTimeInterval = DateTime.Now;
			this.AppointmentIsDue = false;
			this.IdentityGuid = Guid.Empty;
		}
	}
}
