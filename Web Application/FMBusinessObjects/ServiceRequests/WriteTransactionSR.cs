using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using FMBusinessObjects.DataObjects;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    public class WriteTransactionSR : AccountingServiceRequest
	{
		#region Constructor
		public WriteTransactionSR()
		{
		}
		#endregion

		#region Properties
		[DataMember]
		public DataObject TransactionData
		{
			get;
			set;
		}

		[DataMember]
		public DateTimeOffset InventoryDate
		{
			get;
			set;
		}

		[DataMember]
		public string TicketNumber
		{
			get;
			set;
		}

		[DataMember]
		public string FlightNumber
		{
			get;
			set;
		}

		[DataMember]
		public string GrossVolume
		{
			get;
			set;
		}

		[DataMember]
		public string NetVolume
		{
			get;
			set;
		}

		[DataMember]
		public string Product
		{
			get;
			set;
		}

		[DataMember]
		public string Notes
		{
			get;
			set;
		}
		#endregion
	}
}
