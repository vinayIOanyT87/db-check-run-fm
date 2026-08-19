using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
	/// <summary>
	/// 
	/// </summary>
	[DataContract]
	public class ArchiveDataSR
	{
		[DataMember]
		public SecurityClass Security { get; set; }

		[DataMember]
		public DateTimeOffset StartDate { get; set; }

		[DataMember]
		public DateTimeOffset EndDate { get; set; }

		[DataMember]
		public bool CheckAccounting { get; set; }

		[DataMember]
		public bool CheckQC { get; set; }

		[DataMember]
		public bool CheckMaintenance { get; set; }

		[DataMember]
		public bool CheckAlarm { get; set; }

		[DataMember]
		public bool CheckAudit { get; set; }


	}
}
