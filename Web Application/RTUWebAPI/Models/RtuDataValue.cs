using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RTUWebAPI.Models
{
	public class RtuDataValue
	{
		public string value { get; set; }

		public DateTime? timeStamp { get; set; }

		public UInt32 status { get; set; }

		public string dataType { get; set; }

		public string displayFormat { get; set; }

		public UInt32 identifier { get; set; }

		public RtuDataValue( string value,
									DateTime? timeStamp,
									UInt32 status,
									string dataType,
									string displayFormat,
									UInt32 identifier)
		{
			this.value = value;
			this.timeStamp = timeStamp;
			this.status = status;
			this.dataType = dataType;
			this.displayFormat = displayFormat;
			this.identifier = identifier;
		}
	}
}
