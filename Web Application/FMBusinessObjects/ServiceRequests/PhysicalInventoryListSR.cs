using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.ServiceRequests
{
    [Serializable]
    [DataContract]
	public class PhysicalInventoryListSR : AccountingServiceRequest
	{
		#region Attributes
		[DataMember]
		private DateTime inventoryDate;
		[DataMember]
		private string manager;
		[DataMember]
		private string product;
		[DataMember]
		private DateTimeOffset? firstDate;
		[DataMember]
		private DateTimeOffset? lastDate;
		#endregion

		#region Constructor
		public PhysicalInventoryListSR ( )
		{
		}
		#endregion

		#region Properties

		public string Manager
		{
			get { return manager; }
			set { manager = value; }
		}

		public string Product
		{
			get { return product; }
			set { product = value; }
		}

		public DateTime InventoryDate
		{
			get { return inventoryDate; }
			set { inventoryDate = value; }
		}

		public DateTimeOffset? FirstDate
		{
			get { return firstDate; }
			set { firstDate = value; }
		}

		public DateTimeOffset? LastDate
		{
			get { return lastDate; }
			set { lastDate = value; }
		}
		#endregion
	}
}
