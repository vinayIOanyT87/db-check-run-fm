using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FMBusinessObjects.DataObjects
{
   [Serializable]
   [DataContract]
	public class SupplyOrderSummaryContext
	{
		[DataMember] public string Manager = "";
		[DataMember] public string ManagerTip = "";

		[DataMember] public string Owner = "";
		[DataMember] public string OwnerTip = "";

		[DataMember] public string Shipper = "";
		[DataMember] public string ShipperTip = "";

		[DataMember] public string Supplier = "";
		[DataMember] public string SupplierTip = "";

		[DataMember] public string Product = "";
		[DataMember] public string OrderStatus = "";
		[DataMember] public string OrderType = "";
		[DataMember] public DateTimeOffset StartDate;
		[DataMember] public DateTimeOffset EndDate;
		[DataMember] public string SortExpression = "";

		[DataMember] public int DateFilterType = 0;
		[DataMember] public string OrderNumber = "";
	}
}
