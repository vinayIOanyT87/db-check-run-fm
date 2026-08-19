namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Runtime.Serialization;

   [Serializable]
   [DataContract]
	public class OrderSummaryContext
	{
		[DataMember] public string Manager = string.Empty;
		[DataMember] public string ManagerTip = string.Empty;

		[DataMember] public string Owner = string.Empty;
		[DataMember] public string OwnerTip = string.Empty;

		[DataMember] public string ShipTo = string.Empty;
		[DataMember] public string ShipToTip = string.Empty;

		[DataMember] public string BillTo = string.Empty;
		[DataMember] public string BillToTip = string.Empty;

		[DataMember] public string Carrier = string.Empty;
		[DataMember] public string CarrierTip = string.Empty;

		[DataMember] public string Shipper = string.Empty;
		[DataMember] public string ShipperTip = string.Empty;

		[DataMember] public string Product = string.Empty;
		[DataMember] public string OrderStatus = string.Empty;
		[DataMember] public string OrderType = string.Empty;
		[DataMember] public DateTimeOffset StartDate;
		[DataMember] public DateTimeOffset EndDate;
		[DataMember] public string SortExpression = string.Empty;

		[DataMember] public int DateFilterType = 0;

		[DataMember] public string OrderNumber = string.Empty;
	}
}
