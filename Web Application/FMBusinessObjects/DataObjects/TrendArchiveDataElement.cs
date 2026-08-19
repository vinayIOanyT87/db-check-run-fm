namespace FMBusinessObjects.DataObjects
{
	using System;

	public sealed class TrendArchiveDataElement : IComparable
	{
		public string Value;
		public long ValueOpcStatus;
		public DateTimeOffset ValueTimeStamp;
		public int ArchiveRecordType;
		public int EngineeringUnitsIndex;
		public Guid AlarmPriorityGuid;
		public bool Acknowledged;
		public string AlarmState;
		public bool AlarmOrStatusChanged;

		public int CompareTo(object o)
		{
			var trendArchiveDataElement = o as TrendArchiveDataElement;
			if (trendArchiveDataElement == null)
				throw new Exception("Invalid TrendArchiveDataElement");
			return this.ValueTimeStamp.CompareTo(trendArchiveDataElement.ValueTimeStamp);
		}
	}
}
