namespace FMBusinessObjects.DataObjects
{
	using System;

	
	public class ArchiveDataElement
	{
		public Guid PointValueGuid { get; set; }
		private string propertyID;
		// Convert null to string.Empty as evidently the Partition Key cannot have null elements.
		public string PropertyID
		{
			get { return this.propertyID; }
			set { this.propertyID = (value == null) ? "" : value; }
		}
		public string Value { get; set; }
		public long ValueOpcStatus { get; set; }
		public DateTimeOffset ValueTimeStamp { get; set; }
		public int ArchiveRecordType { get; set; }
		public string DataType { get; set; }
		public int EngineeringUnitsIndex { get; set; }
		public Guid AlarmPriorityGuid { get; set; }
		public bool Acknowledged { get; set; }
		public string AlarmState { get; set; }
		public bool AlarmOrStatusChanged { get; set; }
		public DateTimeOffset RecordTimeStamp { get; set; }
		public string QualityString { get; set; }
		public Guid SiteGuid { get; set; }

		public long Partition
		{
			get
			{
				return ArchiveDataElement.GetPartition(this.ValueTimeStamp);
			}
			set
			{
				var dummyYearMonth = value;
			}
		}

		public static int GetPartition(DateTimeOffset timeStamp)
		{
			return timeStamp.Year * 100 + timeStamp.Month;
		}
	}
}
