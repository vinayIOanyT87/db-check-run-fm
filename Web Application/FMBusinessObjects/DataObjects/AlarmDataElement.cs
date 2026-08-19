using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	public class AlarmDataElement
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

		public AlarmDataElement(ArchiveDataElement element)
		{
			this.PointValueGuid = element.PointValueGuid;
			this.PropertyID = element.PropertyID;
			this.Value = element.Value;
			this.ValueOpcStatus = element.ValueOpcStatus;
			this.ValueTimeStamp = element.ValueTimeStamp;
			this.ArchiveRecordType = element.ArchiveRecordType;
			this.DataType = element.DataType;
			this.EngineeringUnitsIndex = element.EngineeringUnitsIndex;
			this.AlarmPriorityGuid = element.AlarmPriorityGuid;
			this.Acknowledged = element.Acknowledged;
			this.AlarmState = element.AlarmState;
			this.AlarmOrStatusChanged = element.AlarmOrStatusChanged;
		}
	}
}
