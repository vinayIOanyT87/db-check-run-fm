using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FMBusinessObjects.DataObjects
{
	public class SimpleArchiveDataElement
	{
		public Guid PointValueGuid { get; set; }
		public string Value { get; set; }
		public long ValueOpcStatus { get; set; }
		public DateTimeOffset ValueTimeStamp { get; set; }
		public string DataType { get; set; }
		public int EngineeringUnitsIndex { get; set; }
		public string QualityString { get; set; }

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
