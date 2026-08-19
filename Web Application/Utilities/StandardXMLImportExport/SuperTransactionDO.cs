using System;

using FM7Accounting;

namespace StandardXMLImportExport
{
	/// <summary>
	/// Summary description for SuperTransactionDO.
	/// </summary>
	public class SuperTransactionDO : TransactionDO
	{
		#region Properties
		new string TransTypeID
		{
			set { transTypeID = value; }
		}
		public string TransRefID
		{
			get { return transRefID; }
			set { transRefID = value; }
		}
		public string LinkedDocumentNumber
		{
			get { return linkedDocumentNumber; }
			set { linkedDocumentNumber = value; }
		}
		public string ShippingDocumentNumber
		{
			get { return shippingDocumentNumber; }
			set { shippingDocumentNumber = value; }
		}
		public string ShipmentNumber
		{
			get { return shipmentNumber; }
			set { shipmentNumber = value; }
		}
		public string ConjoinedTransID
		{
			get { return conjoinedTransID; }
			set { conjoinedTransID = value; }
		}
		public string ShipTo
		{
			get { return shipTo; }
			set { shipTo = value; }
		}
		public string BillTo
		{
			get { return billTo; }
			set { billTo = value; }
		}
		public string Shipper
		{
			get { return shipper; }
			set { shipper = value; }
		}
		public string Carrier
		{
			get { return carrier; }
			set { carrier = value; }
		}
		public string SCACCode
		{
			get { return scacCode; }
			set { scacCode = value; }
		}
		public string Supplier
		{
			get { return supplier; }
			set { supplier = value; }
		}
		public string PONumber
		{
			get { return poNumber; }
			set { poNumber = value; }
		}
		public string DriverIDNumber
		{
			get { return driverIDNumber; }
			set { driverIDNumber = value; }
		}
		public PaymentInfoDO PaymentInfo
		{
			get { return paymentInfo; }
			set { paymentInfo = value; }
		}
		public RouteInfoDO RouteInfo
		{
			get { return routeInfo; }
			set { routeInfo = value; }
		}
		public RouteScheduleDO RouteSchedule
		{
			get { return routeSchedule; }
			set { routeSchedule = value; }
		}
		public string Location
		{
			get { return location; }
			set { location = value; }
		}
		public string TimeIn
		{
			get { return timeIn; }
			set { timeIn = value; }
		}
		public string TimeOut
		{
			get { return timeOut; }
			set {timeOut = value; }
		}
		public string TimeEnd
		{
			get { return timeEnd; }
			set { timeEnd = value; }
		}
		public bool SimultaneousFueling
		{
			get { return simultaneousFueling; }
			set { simultaneousFueling = value; }
		}
		public VDouble EstimatedFuelingDuration
		{
			get { return estimatedFuelingDuration; }
			set { estimatedFuelingDuration = value; }
		}
		public VDateTime RequestedDeliveryDate
		{
			get { return requestedDeliveryDate; }
			set { requestedDeliveryDate = value; }
		}
		public string LoadID
		{
			get { return loadID; }
			set { loadID = value; }
		}
		public bool DeleteFlag
		{
			get { return deleteFlag; }
			set { deleteFlag = value; }
		}
		public System.Collections.ArrayList AviationGaugeReadings
		{
			get { return aviationGaugeReadings; }
			set { aviationGaugeReadings = value; }
		}
		#endregion
		public SuperTransactionDO()
		{
		
		}
	}
}
