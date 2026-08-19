// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GetTransactionSR.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the GetTransactionRequest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.ServiceRequests
{
	using System;
	using System.Collections.Generic;
	using System.Runtime.Serialization;

	using FMBusinessObjects.DataObjects;

	/// <summary>
	/// The get transaction request.
	/// </summary>
	public enum GetTransactionRequest
	{
		SITE_TYPEID_ALIAS_STATUS_LOCATION_LINEITEMSTATUS,
		SITE_TYPEID_ALIAS_TRANSDATE_COMPANIES,
		SITE_TYPEID_ALIAS_DOCUMENTNUMBER,
		SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID,
		SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS,
		SITE_TYPEID_TRANSDATE_STATUS_OPERATORPERSONNELGUID_LINEITEMSTATUS_INVENTORYDATE,
		SITE_TYPEID_SHIPMENTNUMBER,
		SITE_TYPEID_ALIAS_TRANSDATE_RECEIVINGEQUIPMENT_ISSPT_ISSPTNUM_SERIAL,
		CUSTOM_INTERFACE_QUERY,
		SITE_GET_EOD_TRANSACTIONS,
		SITE_TYPEID_REVERSEDTRANSID,
		SITE_TYPEID_STATUS_REF_NUM,
		SITE_ORIGINSTATION_FINALSTATION_SHIPTOID_ROUTINGID_ROUTEORIGINATIONDATE, // The combination of fields used to uniquely identify a flight for Service Request Messaging
		SITE_ORIGINSTATION_SHIPTOID_DESTINATIONSERIALNUMBER1_ETD,				// The combination of fields used to search for a matching flight when processing an arrival message for ServiceRequestMessaging
		SITE_MANAGER_PRODUCT_UNPOSTED_ISSUE,
        GET_TRANSACTION_TYPE_AND_ALIAS,
        GET_LATEST_UPDATED_TRANSACTION,
        SITE_DOCUMENTNUMBER,
		ALIAS_ROW_VERSION,
	};

	/// <summary>
	/// The get transaction service request.
	/// </summary>
	[Serializable]
	[DataContract]
	public class GetTransactionSR : AccountingServiceRequest
	{
		#region Properties
		/// <summary>
		/// Gets or sets the transaction status.
		/// This property gets and sets the transaction status
		/// enumeration.
		/// </summary>
		[DataMember]
		public TransactionStatus TransStatus { get; set; }

		/// <summary>
		/// A collection of statuses used to filter which transactions we get
		/// </summary>
		[DataMember]
		public List<TransactionStatus> TransStatuses { get; set; }

		/// <summary>
		/// Gets or sets the reference ID.
		/// This property gets and set the transaction reference
		/// ID that points to an associated transaction.
		/// </summary>
		[DataMember]
		public string ReferenceID { get; set; }

		/// <summary>
		/// Gets or sets the trans type ID.
		/// </summary>
		[DataMember]
		public TransactionTypes TransTypeID { get; set; }

		/// <summary>
		/// Gets or sets the alias name.
		/// </summary>
		[DataMember]
		public string AliasName { get; set; }

		/// <summary>
		/// Gets or sets the request.
		/// </summary>
		[DataMember]
		public GetTransactionRequest Request { get; set; }

		/// <summary>
		/// Gets or sets the location.
		/// </summary>
		[DataMember]
		public string Location { get; set; }

		/// <summary>
		/// Gets or sets the status.
		/// </summary>
		[DataMember]
		public string Status { get; set; }

		/// <summary>
		/// Gets or sets the beginning date.
		/// </summary>
		[DataMember]
		public DateTimeOffset BeginningDate { get; set; }

		/// <summary>
		/// Gets or sets the ending date.
		/// </summary>
		[DataMember]
		public DateTimeOffset EndingDate { get; set; }

		/// <summary>
		/// Gets or sets the manager ID.
		/// </summary>
		[DataMember]
		public string ManagerID { get; set; }

		/// <summary>
		/// Gets or sets the owner ID.
		/// </summary>
		[DataMember]
		public string OwnerID { get; set; }

		/// <summary>
		/// Gets or sets the shipper ID.
		/// </summary>
		[DataMember]
		public string ShipperID { get; set; }

		/// <summary>
		/// Gets or sets the bill to ID.
		/// </summary>
		[DataMember]
		public string BillToID { get; set; }

		/// <summary>
		/// Gets or sets the ship to ID.
		/// </summary>
		[DataMember]
		public string ShipToID { get; set; }

		/// <summary>
		/// Gets or sets the carrier ID.
		/// </summary>
		[DataMember]
		public string CarrierID { get; set; }

		/// <summary>
		/// Gets or sets the document number.
		/// </summary>
		[DataMember]
		public string DocumentNumber { get; set; }

		/// <summary>
		/// Gets or sets the operator personnel GUID.
		/// </summary>
		[DataMember]
		public Guid OperatorPersonnelGuid { get; set; }

		/// <summary>
		/// Gets or sets the line item status.
		/// </summary>
		[DataMember]
		public string LineItemStatus { get; set; }

		/// <summary>
		/// Gets or sets the inventory date.
		/// </summary>
		[DataMember]
		public DateTimeOffset InventoryDate { get; set; }

		/// <summary>
		/// Gets or sets the shipment number.
		/// </summary>
		[DataMember]
		public string ShipmentNumber { get; set; }

		/// <summary>
		/// Gets or sets the product.
		/// </summary>
		[DataMember]
		public string Product { get; set; }

		/// <summary>
		/// Gets or sets the location ID.
		/// </summary>
		[DataMember]
		public string LocationID { get; set; }

		/// <summary>
		/// Gets or sets the transaction date time.
		/// </summary>
		[DataMember]
		public DateTimeOffset TransactionDateTime { get; set; }

		/// <summary>
		/// Gets or sets the receiving equipment.
		/// </summary>
		[DataMember]
		public string ReceivingEquipment { get; set; }

		/// <summary>
		/// Gets or sets the ISS point.
		/// </summary>
		[DataMember]
		public string IssPt { get; set; }

		/// <summary>
		/// Gets or sets the ISS point number.
		/// </summary>
		[DataMember]
		public string IssPtNum { get; set; }

		/// <summary>
		/// Gets or sets the serial.
		/// </summary>
		[DataMember]
		public string Serial { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether sent to enterprise.
		/// </summary>
		[DataMember]
		public bool SentToEnterprise { get; set; }

		/// <summary>
		/// Gets or sets the custom query.
		/// </summary>
		[DataMember]
		public string CustomQuery { get; set; }

		/// <summary>
		/// Gets or sets the reversed transaction ID.
		/// </summary>
		[DataMember]
		public string ReversedTransID { get; set; }

		/// <summary>
		/// Gets or sets the origin station IATA ID.
		/// </summary>
		[DataMember]
		public string OriginStationIATAID { get; set; }

		/// <summary>
		/// Gets or sets the final station IATA ID.
		/// </summary>
		[DataMember]
		public string FinalStationIATAID { get; set; }

		/// <summary>
		/// Gets or sets the routing ID.
		/// </summary>
		[DataMember]
		public string RoutingID { get; set; }

		/// <summary>
		/// Gets or sets the route origination date.
		/// </summary>
		[DataMember]
		public DateTimeOffset RouteOriginationDate { get; set; }

		/// <summary>
		/// Gets or sets the destination serial number 1.
		/// </summary>
		[DataMember]
		public string DestinationSerialNumber1 { get; set; }

		/// <summary>
		/// Gets or sets the destination serial number 2.
		/// </summary>
		[DataMember]
		public string DestinationSerialNumber2
		{ get; set; }

		/// <summary>
		/// Gets or sets the destination serial number 3.
		/// </summary>
		[DataMember]
		public string DestinationSerialNumber3
		{ get; set; }

		/// <summary>
		/// Gets or sets the ETD.
		/// </summary>
		[DataMember]
		public DateTimeOffset ETD { get; set; }

		/// <summary>
		/// Gets or sets the transaction ID.
		/// </summary>
		[DataMember]
		public string TransId { get; set; }

		/// <summary>
		/// Gets or sets the Card Number.
		/// </summary>
		[DataMember]
		public string CardNumber
		{ get; set; }


        /// Gets or sets the RowVersion.
        /// </summary>
        [DataMember]
		public byte[] RowVersion
        { get; set; }


        /// Gets or sets the ConvertToSiteUnits.
        /// </summary>
        [DataMember]
        public bool ConvertToSiteUnits
        { get; set; }

		/// <summary>
		/// The name of the interface issuing this request
		/// </summary>
		[DataMember]
		public string InterfaceName { get; set; }

		#endregion
	}
}
