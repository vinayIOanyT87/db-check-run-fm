// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TransactionDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the TransactionDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Security;
    using System.Xml.Serialization;

    using BusinessInterfaces;
    using ChannelFactories;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// The transaction Data Object.
	/// </summary>
	[XmlRoot("Transaction")]
	[XmlType("Transaction")]
	[XmlInclude(typeof(OwnerTransferDO)), XmlInclude(typeof(ConsumerTransferDO)), XmlInclude(typeof(RegradeDO)), XmlInclude(typeof(StorageTransferDO))]
	[QueryWriterTopic(typeof(TransactionDO), "Transactions", typeof(LineItemDO), "tblCombinedTable", UseDataDictionary = false, SupportsArchiveQuery = true)]
	[QueryWriterTopicSecurity(RIGHT.VIEW_TRANSACTION_DATA)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_TRANSACTION_DATA)]
	[Serializable]
	[DataContract]
	[KnownType(typeof(PaymentInfoDO))]
	[KnownType(typeof(RouteInfoDO))]
	[KnownType(typeof(RouteScheduleDO))]
	[KnownType(typeof(EquipmentDO))]
	[KnownType(typeof(TransactionPIDXDO))]
	[KnownType(typeof(TransportLineItemDO))]
	[KnownType(typeof(WeightReadingDO))]
	[KnownType(typeof(LineItemDO))]
	[KnownType(typeof(RegradeDO))]
	[KnownType(typeof(ConsumerTransferDO))]
	[KnownType(typeof(OwnerTransferDO))]
	[KnownType(typeof(StorageTransferDO))]
	public class TransactionDO : BaseTransactionDO
	{
		#region Public constants
		public const string DEBIT = "D";
		public const string CREDIT = "C";

		public const string None = "", Original = "O", Reversal = "R", Update = "U", ReversalWithUpdate = "RU", UpdateOriginal = "UO";

		public const string UserDataKeyPrefix = "TAUD";
		public const string USER_DATA_KEY_01 = UserDataKeyPrefix + "1";
		public const string USER_DATA_KEY_02 = UserDataKeyPrefix + "2";
		public const string USER_DATA_KEY_03 = UserDataKeyPrefix + "3";
		public const string USER_DATA_KEY_04 = UserDataKeyPrefix + "4";
		public const string USER_DATA_KEY_05 = UserDataKeyPrefix + "5";
		public const string USER_DATA_KEY_06 = UserDataKeyPrefix + "6";
		public const string USER_DATA_KEY_07 = UserDataKeyPrefix + "7";
		public const string USER_DATA_KEY_08 = UserDataKeyPrefix + "8";
		public const string USER_DATA_KEY_09 = UserDataKeyPrefix + "9";
		public const string USER_DATA_KEY_10 = UserDataKeyPrefix + "10";
		public const string USER_DATA_KEY_11 = UserDataKeyPrefix + "11";
		public const string USER_DATA_KEY_12 = UserDataKeyPrefix + "12";
		public const string USER_DATA_KEY_13 = UserDataKeyPrefix + "13";
		public const string USER_DATA_KEY_14 = UserDataKeyPrefix + "14";
		public const string USER_DATA_KEY_15 = UserDataKeyPrefix + "15";
		public const string USER_DATA_KEY_16 = UserDataKeyPrefix + "16";
		public const string USER_DATA_KEY_17 = UserDataKeyPrefix + "17";
		public const string USER_DATA_KEY_18 = UserDataKeyPrefix + "18";
		public const string USER_DATA_KEY_19 = UserDataKeyPrefix + "19";
		public const string USER_DATA_KEY_20 = UserDataKeyPrefix + "20";
		public const string USER_DATA_KEY_21 = UserDataKeyPrefix + "21";
		public const string USER_DATA_KEY_22 = UserDataKeyPrefix + "22";
		public const string USER_DATA_KEY_23 = UserDataKeyPrefix + "23";
		public const string USER_DATA_KEY_24 = UserDataKeyPrefix + "24";

		public const string UserDataLineItemKeyPrefix = "TALUD";
		public const string USER_DATA_LINE_ITEM_KEY_01 = UserDataLineItemKeyPrefix + "1";
		public const string USER_DATA_LINE_ITEM_KEY_02 = UserDataLineItemKeyPrefix + "2";
		public const string USER_DATA_LINE_ITEM_KEY_03 = UserDataLineItemKeyPrefix + "3";
		public const string USER_DATA_LINE_ITEM_KEY_04 = UserDataLineItemKeyPrefix + "4";
		public const string USER_DATA_LINE_ITEM_KEY_05 = UserDataLineItemKeyPrefix + "5";
		public const string USER_DATA_LINE_ITEM_KEY_06 = UserDataLineItemKeyPrefix + "6";
		public const string USER_DATA_LINE_ITEM_KEY_07 = UserDataLineItemKeyPrefix + "7";
		public const string USER_DATA_LINE_ITEM_KEY_08 = UserDataLineItemKeyPrefix + "8";
		public const string USER_DATA_LINE_ITEM_KEY_09 = UserDataLineItemKeyPrefix + "9";
		public const string USER_DATA_LINE_ITEM_KEY_10 = UserDataLineItemKeyPrefix + "10";
		public const string USER_DATA_LINE_ITEM_KEY_11 = UserDataLineItemKeyPrefix + "11";
		public const string USER_DATA_LINE_ITEM_KEY_12 = UserDataLineItemKeyPrefix + "12";
		public const string USER_DATA_LINE_ITEM_KEY_13 = UserDataLineItemKeyPrefix + "13";
		public const string USER_DATA_LINE_ITEM_KEY_14 = UserDataLineItemKeyPrefix + "14";
		public const string USER_DATA_LINE_ITEM_KEY_15 = UserDataLineItemKeyPrefix + "15";
		public const string USER_DATA_LINE_ITEM_KEY_16 = UserDataLineItemKeyPrefix + "16";
		public const string USER_DATA_LINE_ITEM_KEY_17 = UserDataLineItemKeyPrefix + "17";
		public const string USER_DATA_LINE_ITEM_KEY_18 = UserDataLineItemKeyPrefix + "18";
		public const string USER_DATA_LINE_ITEM_KEY_19 = UserDataLineItemKeyPrefix + "19";
		public const string USER_DATA_LINE_ITEM_KEY_20 = UserDataLineItemKeyPrefix + "20";
		public const string USER_DATA_LINE_ITEM_KEY_21 = UserDataLineItemKeyPrefix + "21";
		public const string USER_DATA_LINE_ITEM_KEY_22 = UserDataLineItemKeyPrefix + "22";
		public const string USER_DATA_LINE_ITEM_KEY_23 = UserDataLineItemKeyPrefix + "23";
		public const string USER_DATA_LINE_ITEM_KEY_24 = UserDataLineItemKeyPrefix + "24";

		#endregion

		#region Private attributes
		/// <summary>
		/// The conjoin reversed transaction ID.
		/// </summary>
		[DataMember]
		private string conjoinReversedTransId;

		/// <summary>
		/// Used for transaction validation during the save process.
		/// </summary>
		[DataMember]
		private bool? permitNonReferenceData;

		/// <summary>
		/// The transaction PIDX collection.
		/// </summary>
		[DataMember]
		private List<TransactionPIDXDO> transPidxCollection;

		/// <summary>
		/// Hash map of database column name to TransactionDO property name.  Primarily used to determine which
		/// fields to clear when a transaction is copied or created from an existing transaction.  Only entries
		/// in which the database column name is different from the TransactionDO property name should be added
		/// to the map.  To specify the property of a nested object, append the property name to the nested
		/// object property name with a dot.  For example the BillTo property of the PaymentInfo nested object
		/// is specified as "PaymentInfo.BillTo"
		/// </summary>
		private static readonly Dictionary<string, string> DbNameToPropertyMap = new Dictionary<string, string>
				{
					{ "AliasName", "Alias" },
					{ "AssociatedDocNumber", "AssociatedDocumentNumber" },
					{ "DriverIdentificationNumber", "DriverIDNumber" },
					{ "LookupOriginApplicationIndex", "OriginApplication" },
					{ "LookupTransactionStatusIndex", "Status" },
					{ "TransDateTime", "TransactionDateTime" },
					{ "TransReferenceID", "TransRefID" },
					{ "BillToID", "PaymentInfo.BillTo" },
					{ "CashAmount", "PaymentInfo.CashAmount" },
					{ "CreditAmount", "PaymentInfo.CreditCardAmount" },
					{ "CardExpiration", "PaymentInfo.CreditCardExpiration" },
					{ "CardName", "PaymentInfo.CreditCardName" },
					{ "CardNumber", "PaymentInfo.CreditCardNumber" },
					{ "CardType", "PaymentInfo.CreditCardType" },
					{ "FinalStationIATAID", "RouteInfo.FinalStationIATAID" },
					{ "InternationalRouteIndicator", "RouteInfo.InternationalRouteIndicator" },
					{ "NextStationIATAID", "RouteInfo.NextStationIATAID" },
					{ "OriginStationIATAID", "RouteInfo.OriginStationIATAID" },
					{ "PreviousRoutingID", "RouteInfo.PreviousRoutingID" },
					{ "PreviousStationIATAID", "RouteInfo.PreviousStationIATAID" },
					{ "RouteOriginationDate", "RouteInfo.RouteOriginationDate" },
					{ "RoutingID", "RouteInfo.RoutingID" },
					{ "ETA", "RouteSchedule.ETA" },
					{ "ETD", "RouteSchedule.ETD" },
					{ "FST", "RouteSchedule.FST" },
					{ "SFT", "RouteSchedule.SFT" },
					{ "STA", "RouteSchedule.STA" },
					{ "STD", "RouteSchedule.STD" },
					{ "DestinationRegistrationID1", "DestinationEQ1.RegistrationID" },
					{ "DestinationSerialNumber1", "DestinationEQ1.SerialNumber" },
					{ "DestinationEquipmentType1", "DestinationEQ1.EquipmentType" },
					{ "DestinationEquipmentSecondaryType1", "DestinationEQ1.SecondaryEquipmentType" },
					{ "DestinationEquipmentModel1", "DestinationEQ1.EquipmentModel" },
					{ "DestinationCompanyEquipmentID1", "DestinationEQ1.CompanyEquipmentID" },
					{ "DestinationRegistrationID2", "DestinationEQ2.RegistrationID" },
					{ "DestinationSerialNumber2", "DestinationEQ2.SerialNumber" },
					{ "DestinationEquipmentType2", "DestinationEQ2.EquipmentType" },
					{ "DestinationEquipmentSecondaryType2", "DestinationEQ2.SecondaryEquipmentType" },
					{ "DestinationEquipmentModel2", "DestinationEQ2.EquipmentModel" },
					{ "DestinationCompanyEquipmentID2", "DestinationEQ2.CompanyEquipmentID" },
					{ "DestinationRegistrationID3", "DestinationEQ3.RegistrationID" },
					{ "DestinationSerialNumber3", "DestinationEQ3.SerialNumber" },
					{ "DestinationEquipmentType3", "DestinationEQ3.EquipmentType" },
					{ "DestinationEquipmentSecondaryType3", "DestinationEQ3.SecondaryEquipmentType" },
					{ "DestinationEquipmentModel3", "DestinationEQ3.EquipmentModel" },
					{ "DestinationCompanyEquipmentID3", "DestinationEQ3.CompanyEquipmentID" },
					{ "SourceRegistrationID1", "SourceEQ1.RegistrationID" },
					{ "SourceSerialNumber1", "SourceEQ1.SerialNumber" },
					{ "SourceEquipmentType1", "SourceEQ1.EquipmentType" },
					{ "SourceEquipmentSecondaryType1", "SourceEQ1.SecondaryEquipmentType" },
					{ "SourceEquipmentModel1", "SourceEQ1.EquipmentModel" },
					{ "SourceCompanyEquipmentID1", "SourceEQ1.CompanyEquipmentID" },
					{ "SourceRegistrationID2", "SourceEQ2.RegistrationID" },
					{ "SourceSerialNumber2", "SourceEQ2.SerialNumber" },
					{ "SourceEquipmentType2", "SourceEQ2.EquipmentType" },
					{ "SourceEquipmentSecondaryType2", "SourceEQ2.SecondaryEquipmentType" },
					{ "SourceEquipmentModel2", "SourceEQ2.EquipmentModel" },
					{ "SourceCompanyEquipmentID2", "SourceEQ2.CompanyEquipmentID" },
					{ "SourceRegistrationID3", "SourceEQ3.RegistrationID" },
					{ "SourceSerialNumber3", "SourceEQ3.SerialNumber" },
					{ "SourceEquipmentType3", "SourceEQ3.EquipmentType" },
					{ "SourceEquipmentSecondaryType3", "SourceEQ3.SecondaryEquipmentType" },
					{ "SourceEquipmentModel3", "SourceEQ3.EquipmentModel" },
					{ "SourceCompanyEquipmentID3", "SourceEQ3.CompanyEquipmentID" },
					{ string.Empty, string.Empty }
				};
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionDO"/> class. 
		/// This is the default constructor for the Transaction Data Object class.
		/// </summary>
		public TransactionDO()
		{
			this.ShipToCompanyGuid = Guid.Empty;
			this.OperatorPersonnelGuid = Guid.Empty;
			this.permitNonReferenceData = null;
		}
		#endregion

		#region Properties
		[XmlIgnore]
		public List<TransactionPIDXDO> TransPIDXCollection
		{
			get { return this.transPidxCollection; }
			set { this.transPidxCollection = value; }
		}

		/// <summary>
		/// Gets and sets the permit non reference data used to transaction
		/// validation in the save process.
		/// </summary>
        [XmlIgnore]
        public bool? PermitNonReferenceData
		{
			get { return this.permitNonReferenceData; }
			set { this.permitNonReferenceData = value; }
		}

		public string ConjoinReversedTransID
		{
			get { return this.conjoinReversedTransId; }
			set { this.conjoinReversedTransId = value; }
		}

		/// <summary>
		/// The Response Time for a dispatch transaction.  Placed here in the base class to support Query.
		/// </summary>
		[QueryWriterField("Response Time", "tblTransactions.ResponseTime", false)]
		private string ResponseTime { get; set; }

		/// <summary>
		/// The Fuel Time for a dispatch transaction.  Placed here in the base class to support Query.
		/// </summary>
		[QueryWriterField("Fuel Time", "tblTransactions.FuelTime", false)]
		private string FuelTime { get; set; }


		/// <summary>
		/// The ID of the To Product for an Regrade (type 15) transaction).  Placed here in the 
		/// base class to support Query.
		/// </summary>
		[QueryWriterField("To Product", "tblTransactionLineItems.ToProduct", false)]
		private string ToProduct { get; set; }

		/// <summary>
		/// The ID of the From Product for an Regrade (type 15) transaction).  Placed here in the 
		/// base class to support Query.  Not used by subclass so it is only for Query use.
		/// </summary>
		[QueryWriterField("From Product", "tblTransactionLineItems.FromProduct", false)]
		private string FromProduct { get; set; }

		/// <summary>
		/// The ID of the Manager for an OwnerTransfer (type 13) transaction).  Placed here in the 
		/// base class to support Query.
		/// </summary>
		[QueryWriterField("To Manager", "tblTransactions.ToManagerID", false)]
		[DataMember]
		public string ToManagerID
		{
			get;
			set;
		}

		/// <summary>
		/// The ID of the Manager for an OwnerTransfer (type 13) transaction).  Placed here in the 
		/// base class to support Query.  Not used by subclass so it is only for Query use.
		/// </summary>
		[QueryWriterField("From Manager", "tblTransactions.FromManagerID", false)]
		[DataMember]
		public string FromManagerID { get; set; }

		/// <summary>
		/// The ID of the Owner for an OwnerTransfer (type 13) transaction).  Placed here in the 
		/// base class to support Query.
		/// </summary>
		[QueryWriterField("To Owner", "tblTransactions.ToOwnerID", false)]
		[DataMember]
		public string ToOwnerID { get; set; }

		/// <summary>
		/// The ID of the Owner for an OwnerTransfer (type 13) transaction).  Placed here in the 
		/// base class to support Query.  Not used by subclass so it is only for Query use.
		/// </summary>
		[QueryWriterField("From Owner", "tblTransactions.FromOwnerID", false)]
		[DataMember]
		public string FromOwnerID { get; set; }

		/// <summary>
		/// The ID of the Carrier for an OwnerTransfer (type 13) transaction).  Placed here in the 
		/// base class to support Query.  Not used by subclass so it is only for Query use.
		/// </summary>
		[QueryWriterField("From Carrier", "tblTransactions.FromCarrierID", false)]
		[DataMember]
		public string FromCarrierID { get; set; }

		/// <summary>
		/// The ID of the Carrier for an OwnerTransfer (type 13) transaction).  Placed here in the 
		/// base class to support Query.
		/// </summary>
		[QueryWriterField("To Carrier", "tblTransactions.ToCarrierID", false)]
		[DataMember]
		public string ToCarrierID { get; set; }

		[QueryWriterField("Trans Ref ID", "tblTransactions.TransReferenceID")]
		public string TransRefID
		{
			get { return base.transRefID; }
			set { base.transRefID = value; }
		}

		[QueryWriterField("Linked Document Number", "tblTransactions.LinkedDocumentNumber")]
		public string LinkedDocumentNumber
		{
			get { return base.linkedDocumentNumber; }
			set { base.linkedDocumentNumber = value; }
		}

		[QueryWriterField("Shipping Document Number", "tblTransactions.ShippingDocumentNumber")]
		public string ShippingDocumentNumber
		{
			get { return base.shippingDocumentNumber; }
			set { base.shippingDocumentNumber = value; }
		}

		[QueryWriterField("Shipment Number", "tblTransactions.ShipmentNumber")]
		public string ShipmentNumber
		{
			get { return base.shipmentNumber; }
			set { base.shipmentNumber = value; }
		}

		public string ConjoinedTransID
		{
			get { return base.conjoinedTransID; }
			set { base.conjoinedTransID = value; }
		}

		[QueryWriterField("Ship To ID", "tblTransactions.ShipToID")]
		public string ShipToID
		{
			get { return base.shipToID; }
			set { base.shipToID = value; }
		}

		[QueryWriterField("Ship To Code", "tblTransactions.ShipToCode")]
		public string ShipToCode
		{
			get { return base.shipToCode; }
			set { base.shipToCode = value; }
		}

		[QueryWriterField("Bill To ID", "tblTransactions.BillToID")]
		public string BillToID
		{
			get { return base.billToID; }
			set { base.billToID = value; }
		}

		[QueryWriterField("Bill To Code", "tblTransactions.BillToCode")]
		public string BillToCode
		{
			get { return base.billToCode; }
			set { base.billToCode = value; }
		}

		[QueryWriterField("Shipper ID", "tblTransactions.ShipperID")]
		public string ShipperID
		{
			get { return base.shipperID; }
			set { base.shipperID = value; }
		}

		[QueryWriterField("Shipper Code", "tblTransactions.ShipperCode")]
		[DataMember]
		public string ShipperCode
		{
			get;
			set;
		}

		[QueryWriterField("Carrier ID", "tblTransactions.CarrierID")]
		public string CarrierID
		{
			get { return base.carrierID; }
			set { base.carrierID = value; }
		}

		[QueryWriterField("Carrier Code", "tblTransactions.CarrierCode")]
		public string CarrierCode
		{
			get { return base.carrierCode; }
			set { base.carrierCode = value; }
		}

		[QueryWriterField("SCAC Code", "tblTransactions.SCACCode")]
		public string SCACCode
		{
			get { return base.scacCode; }
			set { base.scacCode = value; }
		}

		[QueryWriterField("Supplier ID", "tblTransactions.SupplierID")]
		public string SupplierID
		{
			get { return base.supplierID; }
			set { base.supplierID = value; }
		}

		[QueryWriterField("Supplier Code", "tblTransactions.SupplierCode")]
		public string SupplierCode
		{
			get { return base.supplierCode; }
			set { base.supplierCode = value; }
		}

		[QueryWriterField("PO Number", "tblTransactions.PONumber")]
		public string PONumber
		{
			get { return base.poNumber; }
			set { base.poNumber = value; }
		}

		[QueryWriterField("Driver ID Number", "tblTransactions.DriverIdentificationNumber")]
		public string DriverIDNumber
		{
			get { return base.driverIDNumber; }
			set { base.driverIDNumber = value; }
		}

		[QueryWriterField("Card Number", "tblTransactions.CardNumber")]
		public string CardNumber
		{
			get { return this.paymentInfo.CreditCardNumber; }
		}

		[QueryWriterField("Card Name", "tblTransactions.CardName")]
		public string CardName
		{
			get { return this.paymentInfo.CreditCardName; }
		}

		public PaymentInfoDO PaymentInfo
		{
			get { return this.paymentInfo; }
			set { this.paymentInfo = value; }
		}

		public RouteInfoDO RouteInfo
		{
			get { return this.routeInfo; }
			set { this.routeInfo = value; }
		}

		public RouteScheduleDO RouteSchedule
		{
			get { return this.routeSchedule; }
			set { this.routeSchedule = value; }
		}

		[QueryWriterField("FST", "tblTransactions.FST")]
        [XmlIgnore]
        public DateTimeOffset? FST
		{
			get { return this.timeIn; }
			set { this.timeIn = value; }
		}

		[QueryWriterField("Final Station IATA ID", "tblTransactions.FinalStationIATAID")]
		private string FinalStationIATAID
		{
			get { return this.routeInfo.FinalStationIATAID; }
		}

		[XmlElement("TimeInString")]
		public string TimeInString
		{
			get
			{
				return this.timeIn == null ? string.Empty : ((DateTimeOffset)this.timeIn).ToString(TimeFormat);
			}

			set
			{
				this.timeIn = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Time In", "tblTransactions.TimeIn")]
		[XmlIgnore]
		public DateTimeOffset? TimeIn
		{
			get { return base.timeIn; }
			set { base.timeIn = value; }
		}

		[XmlElement("TimeOutString")]
		public string TimeOutString
		{
			get
			{
				return this.timeOut == null ? string.Empty : ((DateTimeOffset)this.timeOut).ToString(TimeFormat);
			}

			set
			{
				this.timeOut = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Time Out", "tblTransactions.TimeOut")]
		[XmlIgnore]
		public DateTimeOffset? TimeOut
		{
			get { return base.timeOut; }
			set { base.timeOut = value; }
		}

		[XmlElement("TimeEndString")]
		public string TimeEndString
		{
			get
			{
				return this.timeEnd == null ? string.Empty : ((DateTimeOffset)this.timeEnd).ToString(TimeFormat);
			}

			set
			{
				this.timeEnd = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Time End", "tblTransactions.TimeEnd")]
		[XmlIgnore]
		public DateTimeOffset? TimeEnd
		{
			get { return base.timeEnd; }
			set { base.timeEnd = value; }
		}

		[QueryWriterField("Est Fueling Duration", "tblTransactions.EstimatedFuelingDuration")]
		[DataMember]
		public int? EstimatedFuelingDuration { get; set; }

		[XmlElement("RequestedDeliveryDateString")]
		public string RequestedDeliveryDateString
		{
			get
			{
				return this.requestedDeliveryDate == null ? string.Empty : ((DateTimeOffset)this.requestedDeliveryDate).ToString(TimeFormat);
			}

			set
			{
				this.requestedDeliveryDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Requested Delivery Date", "tblTransactions.RequestedDeliveryDate")]
		[XmlIgnore]
		public DateTimeOffset? RequestedDeliveryDate
		{
			get { return base.requestedDeliveryDate; }
			set { base.requestedDeliveryDate = value; }
		}

		[QueryWriterField("Load ID", "tblTransactions.LoadID")]
		public string LoadID
		{
			get { return base.loadID; }
			set { base.loadID = value; }
		}

		[QueryWriterField("Deleted", "tblTransactions.DeleteFlag")]
		public bool DeleteFlag
		{
			get { return base.deleteFlag; }
			set { base.deleteFlag = value; }
		}

		[XmlArrayItem(Type = typeof(WeightReadingDO))]
		public List<WeightReadingDO> WeightReadings
		{
			get { return base.weightReadings; }
			set { base.weightReadings = value; }
		}

		[XmlArrayItem(Type = typeof(TransportLineItemDO))]
		public List<TransportLineItemDO> TransportInfoList
		{
			get { return base.transportInfoList; }
			set { base.transportInfoList = value; }
		}

		[QueryWriterField("Destination Registration ID 1", "tblTransactions.DestinationRegistrationID1")]
		public string DestinationRegistrationID1
		{
			get { return this.DestinationEQ1.RegistrationID; }
			set { base.destinationEQ1.RegistrationID = value; }
		}

		[QueryWriterField("Destination Model 1", "tblTransactions.DestinationEquipmentModel1")]
		public string DestinationEquipmentModel1
		{
			get { return DestinationEQ1.EquipmentModel; }
		}

        [XmlIgnoreAttribute]
        [QueryWriterField("Destination Equipment Type 1", "tblTransactions.DestinationEquipmentType1", "tblTransactions.DestinationEquipmentSecondaryType1")]
        public string DestinationEquipmentType1
        {
            get { return this.DestinationEQ1.EquipmentType; }
        }

        [XmlIgnoreAttribute]
        [QueryWriterField("Destination Equipment Secondary Type 1", "tblTransactions.DestinationEquipmentSecondaryType1")]
        public string DestinationEquipmentSecondaryType1
        {
            get { return this.DestinationEQ1.EquipmentSecondaryTypeName; }
        }


		[XmlIgnoreAttribute]
		[QueryWriterField("Destination Serial number 1", "tblTransactions.DestinationSerialNumber1")]
		public string DestinationSerialNumber1
		{
			get { return this.DestinationEQ1.SerialNumber; }
		}

		[QueryWriterField("Destination Registration ID 2", "tblTransactions.DestinationRegistrationID2")]
		public string DestinationRegistrationID2
		{
			get { return this.DestinationEQ2.RegistrationID; }
			set { base.destinationEQ2.RegistrationID = value; }
		}

		[QueryWriterField("Destination Model 2", "tblTransactions.DestinationEquipmentModel2")]
		public string DestinationEquipmentModel2
		{
			get { return DestinationEQ2.EquipmentModel; }
		}

        [XmlIgnoreAttribute]
        [QueryWriterField("Destination Equipment Type 2", "tblTransactions.DestinationEquipmentType2", "tblTransactions.DestinationEquipmentSecondaryType2")]
        public string DestinationEquipmentType2
        {
            get { return this.DestinationEQ2.EquipmentType; }
        }


		[XmlIgnoreAttribute]
		[QueryWriterField("Destination Serial number 2", "tblTransactions.DestinationSerialNumber2")]
		public string DestinationSerialNumber2
		{
			get { return this.DestinationEQ2.SerialNumber; }
		}


		[XmlIgnoreAttribute]
        [QueryWriterField("Destination Equipment Secondary Type 2", "tblTransactions.DestinationEquipmentSecondaryType2")]
        public string DestinationEquipmentSecondaryType2
        {
            get { return this.DestinationEQ2.EquipmentSecondaryTypeName; }
        }

        [QueryWriterField("Destination Registration ID 3", "tblTransactions.DestinationRegistrationID3")]
		public string DestinationRegistrationID3
		{
			get { return this.DestinationEQ3.RegistrationID; }
			set { base.destinationEQ3.RegistrationID = value; }
		}

		[QueryWriterField("Destination Model 3", "tblTransactions.DestinationEquipmentModel3")]
		public string DestinationEquipmentModel3
		{
			get { return DestinationEQ3.EquipmentModel; }
		}

        [XmlIgnoreAttribute]
        [QueryWriterField("Destination Equipment Type 3", "tblTransactions.DestinationEquipmentType3", "tblTransactions.DestinationEquipmentSecondaryType3")]
        public string DestinationEquipmentType3
        {
            get { return this.DestinationEQ3.EquipmentType; }
        }

        [XmlIgnoreAttribute]
        [QueryWriterField("Destination Equipment Secondary Type 3", "tblTransactions.DestinationEquipmentSecondaryType3")]
        public string DestinationEquipmentSecondaryType3
        {
            get { return this.DestinationEQ3.EquipmentSecondaryTypeName; }
        }

		[XmlIgnoreAttribute]
		[QueryWriterField("Destination Serial number 3", "tblTransactions.DestinationSerialNumber3")]
		public string DestinationSerialNumber3
		{
			get { return this.DestinationEQ3.SerialNumber; }
		}

		public EquipmentDO DestinationEQ1
		{
			get { return base.destinationEQ1; }
			set { base.destinationEQ1 = value; }
		}

		[QueryWriterField("Dest Equip 1 Company ID", "tblTransactions.DestinationCompanyEquipmentID1")]
		public string DestinationEQ1ID
		{
			get { return this.DestinationEQ1.CompanyEquipmentID; }
		}

		public EquipmentDO DestinationEQ2
		{
			get { return base.destinationEQ2; }
			set { base.destinationEQ2 = value; }
		}

		[QueryWriterField("Dest Equip 2 Company ID", "tblTransactions.DestinationCompanyEquipmentID2")]
		public string DestinationEQ2ID
		{
			get { return this.DestinationEQ2.CompanyEquipmentID; }
		}

		public EquipmentDO DestinationEQ3
		{
			get { return base.destinationEQ3; }
			set { base.destinationEQ3 = value; }
		}

		[QueryWriterField("Dest Equip 3 Company ID", "tblTransactions.DestinationCompanyEquipmentID3")]
		public string DestinationEQ3ID
		{
			get { return this.DestinationEQ3.CompanyEquipmentID; }
		}

        public EquipmentDO DestinationEQ4
	    {
	        get
	        {
	            return null;
	        }
	        set
	        {
	            throw new NotImplementedException();
	        }
	    }

	    [QueryWriterField("Dest Equip 4 Company ID", "tblTransactions.DestinationCompanyEquipmentID4")]
	    public string DestinationEQ4ID => string.Empty;

		public EquipmentDO SourceEQ1
		{
			get { return base.sourceEQ1; }
			set { base.sourceEQ1 = value; }
		}

		[QueryWriterField("Source Equip 1 Company ID", "tblTransactions.SourceCompanyEquipmentID1")]
		public string SourceEQ1ID
		{
			get { return this.SourceEQ1.CompanyEquipmentID; }
		}

		public EquipmentDO SourceEQ2
		{
			get { return base.sourceEQ2; }
			set { base.sourceEQ2 = value; }
		}

		[QueryWriterField("Source Equip 2 Company ID", "tblTransactions.SourceCompanyEquipmentID2")]
		public string SourceEQ2ID
		{
			get { return this.SourceEQ2.CompanyEquipmentID; }
		}

		public EquipmentDO SourceEQ3
		{
			get { return base.sourceEQ3; }
			set { base.sourceEQ3 = value; }
		}

		[QueryWriterField("Source Equip 3 Company ID", "tblTransactions.SourceCompanyEquipmentID3")]
		public string SourceEQ3ID
		{
			get { return this.SourceEQ3.CompanyEquipmentID; }
		}

        public EquipmentDO SourceEQ4
        {
            get { return null; }
            set { throw new NotImplementedException(); }
        }

	    [QueryWriterField("Source Equip 3 Company ID", "tblTransactions.SourceCompanyEquipmentID3")]
	    public string SourceEQ4ID => string.Empty;

		[QueryWriterField("Source Registration ID 1", "tblTransactions.SourceRegistrationID1")]
		public string SourceRegistrationID1
		{
			get { return this.SourceEQ1.RegistrationID; }
		}

		[QueryWriterField("Source Model 1", "tblTransactions.SourceEquipmentModel1")]
		public string SourceEquipmentModel1
		{
			get { return SourceEQ1.EquipmentModel; }
		}

        [QueryWriterField("Source Equipment Type 1", "tblTransactions.SourceEquipmentType1", "tblTransactions.SourceEquipmentSecondaryType1")]
        public string SourceEquipmentType1
        {
            get { return this.SourceEQ1.EquipmentType; }
        }

        [QueryWriterField("Source Equipment Secondary Type 1", "tblTransactions.SourceEquipmentSecondaryType1")]
        public string SourceEquipmentSecondaryType1
        {
            get { return this.SourceEQ1.EquipmentSecondaryTypeName; }
        }

        [QueryWriterField("Source Registration ID 2", "tblTransactions.SourceRegistrationID2")]
		public string SourceRegistrationID2
		{
			get { return this.SourceEQ2.RegistrationID; }
		}

		[QueryWriterField("Source Model 2", "tblTransactions.SourceEquipmentModel2")]
		public string SourceEquipmentModel2
		{
			get { return SourceEQ2.EquipmentModel; }
		}

        [QueryWriterField("Source Equipment Type 2", "tblTransactions.SourceEquipmentType2", "tblTransactions.SourceEquipmentSecondaryType2")]
        public string SourceEquipmentType2
        {
            get { return this.SourceEQ2.EquipmentType; }
        }

        [QueryWriterField("Source Equipment Secondary Type 2", "tblTransactions.SourceEquipmentSecondaryType2")]
        public string SourceEquipmentSecondaryType2
        {
            get { return this.SourceEQ2.EquipmentSecondaryTypeName; }
        }

        [QueryWriterField("Source Registration ID 3", "tblTransactions.SourceRegistrationID3")]
		public string SourceRegistrationID3
		{
			get { return this.SourceEQ3.RegistrationID; }
		}

		[QueryWriterField("Source Model 3", "tblTransactions.SourceEquipmentModel3")]
		public string SourceEquipmentModel3
		{
			get { return SourceEQ3.EquipmentModel; }
		}

        [QueryWriterField("Source Equipment Type 3", "tblTransactions.SourceEquipmentType3", "tblTransactions.SourceEquipmentSecondaryType3")]
        public string SourceEquipmentType3
        {
            get { return this.SourceEQ3.EquipmentType; }
        }

        [QueryWriterField("Source Equipment Secondary Type 3", "tblTransactions.SourceEquipmentSecondaryType3")]
        public string SourceEquipmentSecondaryType3
        {
            get { return this.SourceEQ3.EquipmentSecondaryTypeName; }
        }

        [QueryWriterField("Operator ID", "tblTransactions.OperatorID")]
		public string OperatorID
		{
			get { return base.operatorID; }
			set { base.operatorID = value; }
		}

		[QueryWriterField("Operator Name", "tblTransactions.OperatorName")]
		public string OperatorName
		{
			get { return base.operatorName; }
			set { base.operatorName = value; }
		}

		[XmlIgnore]
		[DataMember]
		public Guid OperatorPersonnelGuid { get; set; }

		[XmlElement("EffectiveDateString")]
		public string EffectiveDateString
		{
			get
			{
				return this.effectiveDate == null ? string.Empty : ((DateTimeOffset)this.effectiveDate).ToString(TimeFormat);
			}

			set
			{
				this.effectiveDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? EffectiveDate
		{
			get { return base.effectiveDate; }
			set { base.effectiveDate = value; }
		}

		// This property only exists for the Query Writer. It is used to determine
		// how to display the date. In this case, we only want the date and not the
		// time.
		[QueryWriterField("Effective Date", "tblTransactions.EffectiveDate")]
		[XmlIgnore]
		public Date EffectiveDateAsDateOnly
		{
			get { return new Date(); }
			private set { ; }
		}

		[XmlElement("ExpirationDateString")]
		public string ExpirationDateString
		{
			get
			{
				return this.expirationDate == null ? string.Empty : ((DateTimeOffset)this.expirationDate).ToString(TimeFormat);
			}

			set
			{
				this.expirationDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[XmlIgnore]
		public DateTimeOffset? ExpirationDate
		{
			get { return base.expirationDate; }
			set { base.expirationDate = value; }
		}

		// This property only exists for the Query Writer. It is used to determine
		// how to display the date. In this case, we only want the date and not the
		// time.
		[QueryWriterField("Expiration Date", "tblTransactions.ExpirationDate")]
		[XmlIgnore]
		public Date ExpirationDateAsDateOnly
		{
			get { return new Date(); }
			private set { ; }
		}

		[XmlElement("ScheduledDateString")]
		public string ScheduledDateString
		{
			get
			{
				return this.scheduledDate == null ? string.Empty : ((DateTimeOffset)this.scheduledDate).ToString(TimeFormat);
			}

			set
			{
				this.scheduledDate = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Scheduled Date", "tblTransactions.ScheduledDate")]
		[XmlIgnore]
		public DateTimeOffset? ScheduledDate
		{
			get { return base.scheduledDate; }
			set { base.scheduledDate = value; }
		}

		[QueryWriterField("Auto Complete", "tblTransactions.AutoComplete")]
		public bool AutoComplete
		{
			get { return base.autoComplete; }
			set { base.autoComplete = value; }
		}

		// 5-20-08	vthompson: Properties added for ADF (Flag01 - Flag06)
		[QueryWriterField("Flag 1", "tblTransactions.Flag01")]
		public bool Flag01
		{
			get { return base.flag01; }
			set { base.flag01 = value; }
		}

		[QueryWriterField("Flag 2", "tblTransactions.Flag02")]
		public bool Flag02
		{
			get { return base.flag02; }
			set { base.flag02 = value; }
		}

		[QueryWriterField("Flag 3", "tblTransactions.Flag03")]
		public bool Flag03
		{
			get { return base.flag03; }
			set { base.flag03 = value; }
		}

		[QueryWriterField("Flag 4", "tblTransactions.Flag04")]
		public bool Flag04
		{
			get { return base.flag04; }
			set { base.flag04 = value; }
		}

		[QueryWriterField("Flag 5", "tblTransactions.Flag05")]
		public bool Flag05
		{
			get { return base.flag05; }
			set { base.flag05 = value; }
		}

		[QueryWriterField("Flag 6", "tblTransactions.Flag06")]
		public bool Flag06
		{
			get { return base.flag06; }
			set { base.flag06 = value; }
		}

		[QueryWriterField("Fuel Additive Flag", "tblTransactions.FuelAdditiveFlag")]
		public bool FuelAdditiveFlag
		{
			get { return base.fuelAdditiveFlag; }
			set { base.fuelAdditiveFlag = value; }
		}

		[QueryWriterField("Issue Point", "tblTransactions.IssuePoint")]
		public string IssuePoint
		{
			get { return base.issuePoint; }
			set { base.issuePoint = value; }
		}

		[QueryWriterField("Issue Point Number", "tblTransactions.IssuePointNumber")]
		public string IssuePointNumber
		{
			get { return base.issuePointNumber; }
			set { base.issuePointNumber = value; }
		}

		[QueryWriterField("RadioNumber", "tblTransactions.RadioNumber")]
		public string RadioNumber
		{
			get { return base.radioNumber; }
			set { base.radioNumber = value; }
		}

		[QueryWriterField("GateID", "tblTransactions.GateID")]
		public string GateID
		{
			get { return base.gateID; }
			set { base.gateID = value; }
		}

        [XmlIgnore]
        public Guid GateGuid
		{
			get { return base.gateGuid; }
			set { base.gateGuid = value; }
		}

		[QueryWriterField("Number 1", "tblTransactions.Number01")]
		public double? Number01
		{
			get { return base.number01; }
			set { base.number01 = value; }
		}

		[QueryWriterField("Number 2", "tblTransactions.Number02")]
		public double? Number02
		{
			get { return base.number02; }
			set { base.number02 = value; }
		}

		[QueryWriterField("Number 3", "tblTransactions.Number03")]
		public double? Number03
		{
			get { return base.number03; }
			set { base.number03 = value; }
		}

		[QueryWriterField("Number 4", "tblTransactions.Number04")]
		public double? Number04
		{
			get { return base.number04; }
			set { base.number04 = value; }
		}

		[QueryWriterField("Number 5", "tblTransactions.Number05")]
		public double? Number05
		{
			get { return base.number05; }
			set { base.number05 = value; }
		}

		[QueryWriterField("Number 6", "tblTransactions.Number06")]
		public double? Number06
		{
			get { return base.number06; }
			set { base.number06 = value; }
		}

		[QueryWriterField("ErrorFlag", "tblTransactions.ErrorFlag")]
		public bool ErrorFlag
		{
			get { return base.errorFlag; }
			set { base.errorFlag = value; }
		}

		// 05-22-2008 vthompson ADF requested fields
		[QueryWriterField("Contact First Name", "tblTransactions.ContactFirstName")]
		public string ContactFirstName
		{
			get { return base.contactFirstName; }
			set { base.contactFirstName = value; }
		}

		[QueryWriterField("Contact Surname", "tblTransactions.ContactSurname")]
		public string ContactSurname
		{
			get { return base.contactSurname; }
			set { base.contactSurname = value; }
		}

		[XmlElement("Date01String")]
		public string Date01String
		{
			get
			{
				return this.date01 == null ? string.Empty : ((DateTimeOffset)this.date01).ToString(TimeFormat);
			}

			set
			{
				this.date01 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		// vt: Generic date fields
		[QueryWriterField("Date 1", "tblTransactions.Date01")]
		[XmlIgnore]
		public DateTimeOffset? Date01
		{
			get { return base.date01; }
			set { base.date01 = value; }
		}

		[XmlElement("Date02String")]
		public string Date02String
		{
			get
			{
				return this.date02 == null ? string.Empty : ((DateTimeOffset)this.date02).ToString(TimeFormat);
			}

			set
			{
				this.date02 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Date 2", "tblTransactions.Date02")]
		[XmlIgnore]
		public DateTimeOffset? Date02
		{
			get { return base.date02; }
			set { base.date02 = value; }
		}

		[XmlElement("Date03String")]
		public string Date03String
		{
			get
			{
				return this.date03 == null ? string.Empty : ((DateTimeOffset)this.date03).ToString(TimeFormat);
			}

			set
			{
				this.date03 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Date 3", "tblTransactions.Date03")]
		[XmlIgnore]
		public DateTimeOffset? Date03
		{
			get { return base.date03; }
			set { base.date03 = value; }
		}

		[XmlElement("Date04String")]
		public string Date04String
		{
			get
			{
				return this.date04 == null ? string.Empty : ((DateTimeOffset)this.date04).ToString(TimeFormat);
			}

			set
			{
				this.date04 = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Date 4", "tblTransactions.Date04")]
		[XmlIgnore]
		public DateTimeOffset? Date04
		{
			get { return base.date04; }
			set { base.date04 = value; }
		}

		// vt: Adding more fields
		[QueryWriterField("Legacy Number", "tblTransactions.LegacyNumber")]
		public string LegacyNumber
		{
			get { return base.legacyNumber; }
			set { base.legacyNumber = value; }
		}

		[QueryWriterField("Country", "tblTransactions.Country")]
		public string Country
		{
			get { return base.country; }
			set { base.country = value; }
		}

		[QueryWriterField("Contact Info", "tblTransactions.ContactInfo")]
		public string ContactInfo
		{
			get { return base.contactInfo; }
			set { base.contactInfo = value; }
		}

		[QueryWriterField( "Associated Document Number", "tblTransactions.AssociatedDocNumber" )]
		public string AssociatedDocumentNumber
		{
			get { return base.associatedDocumentNumber; }
			set { base.associatedDocumentNumber = value; }
		}

		[QueryWriterField("Associated CLIN")]
		public string AssociatedCLIN
		{
			get { return base.associatedCLIN; }
			set { base.associatedCLIN = value; }
		}

		[QueryWriterField("Associated Transport Order Number")]
		public string AssociatedTransportOrderNumber
		{
			get { return this.associatedTransportOrderNumber; }
			set { this.associatedTransportOrderNumber = value; }
		}

		[QueryWriterField("User Data 1", "tblTransactionUserData.UserData1")]
		public string UserData1
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_01))
				{
					return this.userDataTable[USER_DATA_KEY_01];
				}

				return null;
			}
			set { userDataTable[USER_DATA_KEY_01] = value; }
		}

		[QueryWriterField("User Data 2", "tblTransactionUserData.UserData2")]
		public string UserData2
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_02))
				{
					return this.userDataTable[USER_DATA_KEY_02];
				}

				return null;
			}

			set { userDataTable[USER_DATA_KEY_02] = value; }
		}
		[QueryWriterField("User Data 3", "tblTransactionUserData.UserData3")]
		public string UserData3
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_03))
				{
					return this.userDataTable[USER_DATA_KEY_03];
				}

				return null;
			}

			set { userDataTable[USER_DATA_KEY_03] = value; }
		}
		[QueryWriterField("User Data 4", "tblTransactionUserData.UserData4")]
		public string UserData4
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_04))
				{
					return this.userDataTable[USER_DATA_KEY_04];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_04] = value;
			}
		}

		[QueryWriterField("User Data 5", "tblTransactionUserData.UserData5")]
		public string UserData5
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_05))
				{
					return this.userDataTable[USER_DATA_KEY_05];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_05] = value;
			}
		}

		[QueryWriterField("User Data 6", "tblTransactionUserData.UserData6")]
		public string UserData6
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_06))
				{
					return this.userDataTable[USER_DATA_KEY_06];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_06] = value;
			}
		}
		[QueryWriterField("User Data 7", "tblTransactionUserData.UserData7")]
		public string UserData7
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_07))
				{
					return this.userDataTable[USER_DATA_KEY_07];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_07] = value;
			}
		}

		[QueryWriterField("User Data 8", "tblTransactionUserData.UserData8")]
		public string UserData8
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_08))
				{
					return this.userDataTable[USER_DATA_KEY_08];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_08] = value;
			}
		}
		[QueryWriterField("User Data 9", "tblTransactionUserData.UserData9")]
		public string UserData9
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_09))
				{
					return this.userDataTable[USER_DATA_KEY_09];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_09] = value;
			}
		}

		[QueryWriterField("User Data 10", "tblTransactionUserData.UserData10")]
		public string UserData10
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_10))
				{
					return this.userDataTable[USER_DATA_KEY_10];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_10] = value;
			}
		}
		[QueryWriterField("User Data 11", "tblTransactionUserData.UserData11")]
		public string UserData11
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_11))
				{
					return this.userDataTable[USER_DATA_KEY_11];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_11] = value;
			}
		}

		[QueryWriterField("User Data 12", "tblTransactionUserData.UserData12")]
		public string UserData12
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_12))
				{
					return this.userDataTable[USER_DATA_KEY_12];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_12] = value;
			}
		}

		[QueryWriterField("User Data 13", "tblTransactionUserData.UserData13")]
		public string UserData13
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_13))
				{
					return this.userDataTable[USER_DATA_KEY_13];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_13] = value;
			}
		}

		[QueryWriterField("User Data 14", "tblTransactionUserData.UserData14")]
		public string UserData14
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_14))
				{
					return this.userDataTable[USER_DATA_KEY_14];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_14] = value;
			}
		}

		[QueryWriterField("User Data 15", "tblTransactionUserData.UserData15")]
		public string UserData15
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_15))
				{
					return this.userDataTable[USER_DATA_KEY_15];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_15] = value;
			}
		}
		[QueryWriterField("User Data 16", "tblTransactionUserData.UserData16")]
		public string UserData16
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_16))
				{
					return this.userDataTable[USER_DATA_KEY_16];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_16] = value;
			}
		}
		[QueryWriterField("User Data 17", "tblTransactionUserData.UserData17")]
		public string UserData17
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_17))
				{
					return this.userDataTable[USER_DATA_KEY_17];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_17] = value;
			}
		}

		[QueryWriterField("User Data 18", "tblTransactionUserData.UserData18")]
		public string UserData18
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_18))
				{
					return this.userDataTable[USER_DATA_KEY_18];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_18] = value;
			}
		}

		[QueryWriterField("User Data 19", "tblTransactionUserData.UserData19")]
		public string UserData19
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_19))
				{
					return this.userDataTable[USER_DATA_KEY_19];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_19] = value;
			}
		}

		[QueryWriterField("User Data 20", "tblTransactionUserData.UserData20")]
		public string UserData20
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_20))
				{
					return this.userDataTable[USER_DATA_KEY_20];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_20] = value;
			}
		}

		[QueryWriterField("User Data 21", "tblTransactionUserData.UserData21")]
		public string UserData21
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_21))
				{
					return this.userDataTable[USER_DATA_KEY_21];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_21] = value;
			}
		}

		[QueryWriterField("User Data 22", "tblTransactionUserData.UserData22")]
		public string UserData22
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_22))
				{
					return this.userDataTable[USER_DATA_KEY_22];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_22] = value;
			}
		}

		[QueryWriterField("User Data 23", "tblTransactionUserData.UserData23")]
		public string UserData23
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_23))
				{
					return this.userDataTable[USER_DATA_KEY_23];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_23] = value;
			}
		}

		[QueryWriterField("User Data 24", "tblTransactionUserData.UserData24")]
		public string UserData24
		{
			get
			{
				if (this.userDataTable.ContainsKey(USER_DATA_KEY_24))
				{
					return this.userDataTable[USER_DATA_KEY_24];
				}

				return null;
			}
			set
			{
				userDataTable[USER_DATA_KEY_24] = value;
			}
		}

	    [QueryWriterField("ShippingMethod", "tblTransactions.ShippingMethod")]
        [DataMember]
        public string ShippingMethod { get; set; }

		[XmlIgnore]
		[DataMember]
		public Guid ReasonCodeGuid
		{
			get;
			set;
		}

		[XmlIgnore]
		[DataMember]
		public Guid ReferencedTransactionGuid { get; set; }

		// vt 07-14-2008
		/// <summary>
		/// Calculates and returns the total product price stored in
		/// the line items.
		/// </summary>
		public double TotalPrice
		{
			get
			{
				double totalPrice = 0;
				foreach (LineItemDO lineItem in this.LineItems)
				{
					totalPrice += lineItem.TotalValue;
				}

				return totalPrice;
			}
		}

		/// <summary>
		/// Total price with tax is the sum of all line item prices
		/// plus the sum of all line item taxes
		/// </summary>
		/// 
		public double TotalPriceWithTax
		{
			get
			{
				double total = 0;
				foreach (LineItemDO lineItem in this.LineItems)
				{
					total += lineItem.TotalPriceWithTax;
				}

				return total;
			}
		}

		/// <summary>
		/// Returns the total excise tax from the line items.
		/// The Excise tax is stored in the line item's Tax1 property
		/// </summary>
		/// 
		public double TotalExcise
		{
			get
			{
				double total = 0;
				foreach (LineItemDO lineItem in this.LineItems)
				{
					if (lineItem.Tax1 != null)
						total += lineItem.Tax1.Value;
				}

				return total;
			}
		}

		/// <summary>
		/// Returns the total GST tax from the line items.
		/// The GST tax is stored in the line item's Tax2 property.
		/// </summary>
		/// 
		public double TotalGST
		{
			get
			{
				double total = 0;
				foreach (LineItemDO lineItem in this.LineItems)
				{
					if (lineItem.Tax2 != null)
						total += lineItem.Tax2.Value;
				}

				return total;
			}
		}

		/// <summary>
		/// Returns the total markup from the line items.
		/// Markup is stored in the line item's Tax3 property.
		/// </summary>
		/// 
		public double TotalMarkup
		{
			get
			{
				double total = 0;
				foreach (LineItemDO lineItem in this.LineItems)
				{
					if (lineItem.Tax3 != null)
						total += lineItem.Tax3.Value;
				}

				return total;
			}
		}


		/// <summary>
		/// This property returns the total gross quantity of the aggregate
		/// of line item gross quantity field.
		/// </summary>
		public double TotalGrossQuantity//(EngineeringUnit currentSiteVolumeUnit)//int currentSiteIndex)
		{
			[SecuritySafeCritical]
			get
			{
				double total = 0;
				EngineeringUnits EngineeringUnits = new EngineeringUnits();

				foreach (LineItemDO lineItem in this.LineItems)
				{
					if (lineItem.Quantity != null && lineItem.ProductGuid != Guid.Empty)
					{
						EngineeringUnit lineItemVolumeUnit = lineItem.VolumeUnits;
						double volume = lineItem.Quantity.GrossInventoryChange;

						EngineeringUnits.Convert(lineItem.Quantity.GrossInventoryChange,
												  lineItemVolumeUnit,
												  ref volume,
												  volumeUnit,
												  0);
						total += volume;
					}
				}

				total = Math.Round(total, volumeDecimalPlaces, MidpointRounding.AwayFromZero);
				return total;
			}
		}

		/// <summary>
		/// This property returns the total net quantity of the aggregate
		/// of line item net quantity field.
		/// </summary>
		public double TotalNetQuantity//(EngineeringUnit currentSiteVolumeUnit)
		{
			[SecuritySafeCritical]
			get
			{
				double total = 0;

				foreach (LineItemDO lineItem in this.LineItems)
				{
					if (lineItem.Quantity != null && lineItem.ProductGuid != Guid.Empty)
					{
						EngineeringUnit lineItemVolumeUnit = lineItem.VolumeUnits;
						double volume = lineItem.Quantity.NetInventoryChange;
						EngineeringUnits.Convert(lineItem.Quantity.NetInventoryChange,
												 lineItemVolumeUnit,
												 ref volume,
												 volumeUnit,
												 0);
						total += volume;
					}
				}

				total = Math.Round(total, volumeDecimalPlaces, MidpointRounding.AwayFromZero);
				return total;
			}
		}
		public double TotalMassQuantity
		{
			[SecuritySafeCritical]
			get
			{
				double total = 0;

				foreach (LineItemDO lineItem in this.LineItems)
				{
					if (lineItem.Quantity != null && lineItem.ProductGuid != Guid.Empty)
					{
						EngineeringUnit lineItemMassUnit = lineItem.MassUnits;
						double mass = lineItem.Quantity.Mass;
						EngineeringUnits.Convert(lineItem.Quantity.Mass,
												 lineItemMassUnit,
												 ref mass,
												 massUnit,
												 0);
						total += mass;
					}
				}

				total = Math.Round(total, massDecimalPlaces, MidpointRounding.AwayFromZero);
				return total;
			}
		}

		/// <summary>
		/// This method corrects the volume signs for the appropriate transaction types.
		/// </summary>
		public void SetVolumeSigns(bool forDisplay)
		{

			switch (this.TransTypeID)
			{
				case TransactionTypes.T5_PrimaryDisbursement:
				case TransactionTypes.T6_SecondaryDisbursement:
				case TransactionTypes.T25_Shipment:
					{
						foreach (LineItemDO lineItem in this.LineItems)
						{
							// Check to see if this is for going to the screen.
							// If so, then ensure that the values are positive.
							if (forDisplay)
							{
								if ((lineItem.Quantity.GrossInventoryChange < 0
									|| lineItem.Quantity.DeliveredGrossInventoryChange < 0
									|| lineItem.Quantity.NetInventoryChange < 0
									|| lineItem.Quantity.DeliveredNetInventoryChange < 0
									|| lineItem.Quantity.MassInventoryChange < 0
									|| lineItem.Quantity.PackageInventoryChange < 0)
									&& this.ReversalType != Reversal
									&& this.ReversalType != ReversalWithUpdate)
								{
									lineItem.Quantity.GrossInventoryChange *= -1;
									lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
									lineItem.Quantity.NetInventoryChange *= -1;
									lineItem.Quantity.DeliveredNetInventoryChange *= -1;
									lineItem.Quantity.MassInventoryChange *= -1;
									lineItem.Quantity.PackageInventoryChange *= -1;
								}

								if ((lineItem.Quantity.GrossInventoryChange > 0
									|| lineItem.Quantity.DeliveredGrossInventoryChange > 0
									|| lineItem.Quantity.NetInventoryChange > 0
									|| lineItem.Quantity.DeliveredNetInventoryChange > 0
									|| lineItem.Quantity.MassInventoryChange > 0
									|| lineItem.Quantity.PackageInventoryChange > 0)
									&& (this.ReversalType == Reversal
									|| this.ReversalType == ReversalWithUpdate))
								{
									lineItem.Quantity.GrossInventoryChange *= -1;
									lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
									lineItem.Quantity.NetInventoryChange *= -1;
									lineItem.Quantity.DeliveredNetInventoryChange *= -1;
									lineItem.Quantity.MassInventoryChange *= -1;
									lineItem.Quantity.PackageInventoryChange *= -1;
								}
							}

								// Check to see if this is for going to the database.
							// If so, then ensure that the values are negative.
							else
							{
								if ((lineItem.Quantity.GrossInventoryChange > 0
									|| lineItem.Quantity.DeliveredGrossInventoryChange > 0
									|| lineItem.Quantity.NetInventoryChange > 0
									|| lineItem.Quantity.DeliveredNetInventoryChange > 0
									|| lineItem.Quantity.MassInventoryChange > 0
									|| lineItem.Quantity.PackageInventoryChange > 0)
									&& this.ReversalType != Reversal
									&& this.ReversalType != ReversalWithUpdate)
								{
									lineItem.Quantity.GrossInventoryChange *= -1;
									lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
									lineItem.Quantity.NetInventoryChange *= -1;
									lineItem.Quantity.DeliveredNetInventoryChange *= -1;
									lineItem.Quantity.MassInventoryChange *= -1;
									lineItem.Quantity.PackageInventoryChange *= -1;
								}

								if ((lineItem.Quantity.GrossInventoryChange < 0
									|| lineItem.Quantity.DeliveredGrossInventoryChange < 0
									|| lineItem.Quantity.NetInventoryChange < 0
									|| lineItem.Quantity.DeliveredNetInventoryChange < 0
									|| lineItem.Quantity.MassInventoryChange < 0
									|| lineItem.Quantity.PackageInventoryChange < 0)
									&& (this.ReversalType == Reversal
									|| this.ReversalType == ReversalWithUpdate))
								{
									lineItem.Quantity.GrossInventoryChange *= -1;
									lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
									lineItem.Quantity.NetInventoryChange *= -1;
									lineItem.Quantity.DeliveredNetInventoryChange *= -1;
									lineItem.Quantity.MassInventoryChange *= -1;
									lineItem.Quantity.PackageInventoryChange *= -1;
								}
							}

							foreach (SubLineItemDO sublineItem in lineItem.SubLineItems)
							{
								// Check to see if this is for going to the screen.
								// If so, then ensure that the values are positive.
								if (forDisplay)
								{
									if ((sublineItem.Quantity.GrossInventoryChange < 0
										|| sublineItem.Quantity.DeliveredGrossInventoryChange < 0
										|| sublineItem.Quantity.NetInventoryChange < 0
										|| sublineItem.Quantity.DeliveredNetInventoryChange < 0
										|| sublineItem.Quantity.MassInventoryChange < 0
										|| sublineItem.Quantity.PackageInventoryChange < 0)
										&& this.ReversalType != Reversal
										&& this.ReversalType != ReversalWithUpdate)
									{
										sublineItem.Quantity.GrossInventoryChange *= -1;
										sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										sublineItem.Quantity.NetInventoryChange *= -1;
										sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
										sublineItem.Quantity.MassInventoryChange *= -1;
										sublineItem.Quantity.PackageInventoryChange *= -1;
									}

									if ((sublineItem.Quantity.GrossInventoryChange > 0
										|| sublineItem.Quantity.DeliveredGrossInventoryChange > 0
										|| sublineItem.Quantity.NetInventoryChange > 0
										|| sublineItem.Quantity.DeliveredNetInventoryChange > 0
										|| sublineItem.Quantity.MassInventoryChange > 0
										|| sublineItem.Quantity.PackageInventoryChange > 0)
										&& (this.ReversalType == Reversal
										|| this.ReversalType == ReversalWithUpdate))
									{
										sublineItem.Quantity.GrossInventoryChange *= -1;
										sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										sublineItem.Quantity.NetInventoryChange *= -1;
										sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
										sublineItem.Quantity.MassInventoryChange *= -1;
										sublineItem.Quantity.PackageInventoryChange *= -1;
									}
								}

									// Check to see if this is for going to the database.
								// If so, then ensure that the values are negative.
								else
								{
									if ((sublineItem.Quantity.GrossInventoryChange > 0
										|| sublineItem.Quantity.DeliveredGrossInventoryChange > 0
										|| sublineItem.Quantity.NetInventoryChange > 0
										|| sublineItem.Quantity.DeliveredNetInventoryChange > 0
										|| sublineItem.Quantity.MassInventoryChange > 0
										|| sublineItem.Quantity.PackageInventoryChange > 0)
										&& this.ReversalType != Reversal
										&& this.ReversalType != ReversalWithUpdate)
									{
										sublineItem.Quantity.GrossInventoryChange *= -1;
										sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										sublineItem.Quantity.NetInventoryChange *= -1;
										sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
										sublineItem.Quantity.MassInventoryChange *= -1;
										sublineItem.Quantity.PackageInventoryChange *= -1;
									}

									if ((sublineItem.Quantity.GrossInventoryChange < 0
										|| sublineItem.Quantity.DeliveredGrossInventoryChange < 0
										|| sublineItem.Quantity.NetInventoryChange < 0
										|| sublineItem.Quantity.DeliveredNetInventoryChange < 0
										|| sublineItem.Quantity.MassInventoryChange < 0
										|| sublineItem.Quantity.PackageInventoryChange < 0)
										&& (this.ReversalType == Reversal
										|| this.ReversalType == ReversalWithUpdate))
									{
										sublineItem.Quantity.GrossInventoryChange *= -1;
										sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										sublineItem.Quantity.NetInventoryChange *= -1;
										sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
										sublineItem.Quantity.MassInventoryChange *= -1;
										sublineItem.Quantity.PackageInventoryChange *= -1;
									}
								}
							}
						}
						break;
					}

				case TransactionTypes.T11_ConsumerTransfer:
				case TransactionTypes.T13_OwnerTransfer:
				case TransactionTypes.T15_PrimaryRegrade:
				case TransactionTypes.T23_StorageTransfer:
				case TransactionTypes.T16_SecondaryRegrade:
					{
						// For a transfer or regrade transaction the subtype must be initialized.
						// This is for a new transaction since an exiting transaction will of the 
						// above type will already have this field set.
						if (string.IsNullOrEmpty(this.SubType))
						{
							this.SubType = DEBIT;
						}

						foreach (LineItemDO lineItem in this.LineItems)
						{
							if (forDisplay)
							{
								if ((lineItem.Quantity.GrossInventoryChange < 0
									|| lineItem.Quantity.DeliveredGrossInventoryChange < 0
									|| lineItem.Quantity.NetInventoryChange < 0
									|| lineItem.Quantity.DeliveredNetInventoryChange < 0
									|| lineItem.Quantity.MassInventoryChange < 0
									|| lineItem.Quantity.PackageInventoryChange < 0)
									&& this.ReversalType != Reversal
									&& this.ReversalType != ReversalWithUpdate)
								{
									lineItem.Quantity.GrossInventoryChange *= -1;
									lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
									lineItem.Quantity.NetInventoryChange *= -1;
									lineItem.Quantity.DeliveredNetInventoryChange *= -1;
									lineItem.Quantity.MassInventoryChange *= -1;
									lineItem.Quantity.PackageInventoryChange *= -1;
								}

								if ((lineItem.Quantity.GrossInventoryChange > 0
									|| lineItem.Quantity.DeliveredGrossInventoryChange > 0
									|| lineItem.Quantity.NetInventoryChange > 0
									|| lineItem.Quantity.DeliveredNetInventoryChange > 0
									|| lineItem.Quantity.MassInventoryChange > 0
									|| lineItem.Quantity.PackageInventoryChange > 0)
									&& (this.ReversalType == Reversal
									|| this.ReversalType == ReversalWithUpdate))
								{
									lineItem.Quantity.GrossInventoryChange *= -1;
									lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
									lineItem.Quantity.NetInventoryChange *= -1;
									lineItem.Quantity.DeliveredNetInventoryChange *= -1;
									lineItem.Quantity.MassInventoryChange *= -1;
									lineItem.Quantity.PackageInventoryChange *= -1;
								}
							}

							else
							{
								// If the transaction is the FROM (debit), ensure that
								// the value is negative. If the transaction is the TO
								// (CREDIT), ensure that the value is positive.
								if (this.SubType.ToUpper().Equals(DEBIT))
								{
									if ((lineItem.Quantity.GrossInventoryChange > 0
										|| lineItem.Quantity.DeliveredGrossInventoryChange > 0
										|| lineItem.Quantity.NetInventoryChange > 0
										|| lineItem.Quantity.DeliveredNetInventoryChange > 0
										|| lineItem.Quantity.MassInventoryChange > 0
										|| lineItem.Quantity.PackageInventoryChange > 0)
										&& this.ReversalType != Reversal
										&& this.ReversalType != ReversalWithUpdate)
									{
										lineItem.Quantity.GrossInventoryChange *= -1;
										lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										lineItem.Quantity.NetInventoryChange *= -1;
										lineItem.Quantity.DeliveredNetInventoryChange *= -1;
										lineItem.Quantity.MassInventoryChange *= -1;
										lineItem.Quantity.PackageInventoryChange *= -1;
									}

									if ((lineItem.Quantity.GrossInventoryChange < 0
										|| lineItem.Quantity.DeliveredGrossInventoryChange < 0
										|| lineItem.Quantity.NetInventoryChange < 0
										|| lineItem.Quantity.DeliveredNetInventoryChange < 0
										|| lineItem.Quantity.MassInventoryChange < 0
										|| lineItem.Quantity.PackageInventoryChange < 0)
										&& (this.ReversalType == Reversal
										|| this.ReversalType == ReversalWithUpdate))
									{
										lineItem.Quantity.GrossInventoryChange *= -1;
										lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										lineItem.Quantity.NetInventoryChange *= -1;
										lineItem.Quantity.DeliveredNetInventoryChange *= -1;
										lineItem.Quantity.MassInventoryChange *= -1;
										lineItem.Quantity.PackageInventoryChange *= -1;
									}
								}

								else if (this.SubType.ToUpper().Equals(CREDIT))
								{
									if ((lineItem.Quantity.GrossInventoryChange < 0
										|| lineItem.Quantity.DeliveredGrossInventoryChange < 0
										|| lineItem.Quantity.NetInventoryChange < 0
										|| lineItem.Quantity.DeliveredNetInventoryChange < 0
										|| lineItem.Quantity.MassInventoryChange < 0
										|| lineItem.Quantity.PackageInventoryChange < 0)
										&& this.ReversalType != Reversal
										&& this.ReversalType != ReversalWithUpdate)
									{
										lineItem.Quantity.GrossInventoryChange *= -1;
										lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										lineItem.Quantity.NetInventoryChange *= -1;
										lineItem.Quantity.DeliveredNetInventoryChange *= -1;
										lineItem.Quantity.MassInventoryChange *= -1;
										lineItem.Quantity.PackageInventoryChange *= -1;
									}

									if ((lineItem.Quantity.GrossInventoryChange > 0
										|| lineItem.Quantity.DeliveredGrossInventoryChange > 0
										|| lineItem.Quantity.NetInventoryChange > 0
										|| lineItem.Quantity.DeliveredNetInventoryChange > 0
										|| lineItem.Quantity.MassInventoryChange > 0
										|| lineItem.Quantity.PackageInventoryChange > 0)
										&& (this.ReversalType == Reversal
										|| this.ReversalType == ReversalWithUpdate))
									{
										lineItem.Quantity.GrossInventoryChange *= -1;
										lineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										lineItem.Quantity.NetInventoryChange *= -1;
										lineItem.Quantity.DeliveredNetInventoryChange *= -1;
										lineItem.Quantity.MassInventoryChange *= -1;
										lineItem.Quantity.PackageInventoryChange *= -1;
									}
								}
							}

							foreach (SubLineItemDO sublineItem in lineItem.SubLineItems)
							{
								if (forDisplay)
								{
									if ((sublineItem.Quantity.GrossInventoryChange < 0
										|| sublineItem.Quantity.DeliveredGrossInventoryChange < 0
										|| sublineItem.Quantity.NetInventoryChange < 0
										|| sublineItem.Quantity.DeliveredNetInventoryChange < 0
										|| sublineItem.Quantity.MassInventoryChange < 0
										|| sublineItem.Quantity.PackageInventoryChange < 0)
										&& this.ReversalType != Reversal
										&& this.ReversalType != ReversalWithUpdate)
									{
										sublineItem.Quantity.GrossInventoryChange *= -1;
										sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
										sublineItem.Quantity.NetInventoryChange *= -1;
										sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
										sublineItem.Quantity.MassInventoryChange *= -1;
										sublineItem.Quantity.PackageInventoryChange *= -1;
									}
								}

								else
								{
									// If the transaction is the FROM (debit), ensure that
									// the value is negative. If the transaction is the TO
									// (CREDIT), ensure thathe value is positive.
									if (this.SubType.ToUpper().Equals(DEBIT))
									{
										if ((sublineItem.Quantity.GrossInventoryChange > 0
											|| sublineItem.Quantity.DeliveredGrossInventoryChange > 0
											|| sublineItem.Quantity.NetInventoryChange > 0
											|| sublineItem.Quantity.DeliveredNetInventoryChange > 0
											|| sublineItem.Quantity.MassInventoryChange > 0
											|| sublineItem.Quantity.PackageInventoryChange > 0)
											&& this.ReversalType != Reversal
											&& this.ReversalType != ReversalWithUpdate)
										{
											sublineItem.Quantity.GrossInventoryChange *= -1;
											sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
											sublineItem.Quantity.NetInventoryChange *= -1;
											sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
											sublineItem.Quantity.MassInventoryChange *= -1;
											sublineItem.Quantity.PackageInventoryChange *= -1;
										}
									}

									else if (this.SubType.ToUpper().Equals(CREDIT))
									{
										if ((sublineItem.Quantity.GrossInventoryChange < 0
											|| sublineItem.Quantity.DeliveredGrossInventoryChange < 0
											|| sublineItem.Quantity.NetInventoryChange < 0
											|| sublineItem.Quantity.DeliveredNetInventoryChange < 0
											|| sublineItem.Quantity.MassInventoryChange < 0
											|| sublineItem.Quantity.PackageInventoryChange < 0)
											&& this.ReversalType != Reversal
											&& this.ReversalType != ReversalWithUpdate)
										{
											sublineItem.Quantity.GrossInventoryChange *= -1;
											sublineItem.Quantity.DeliveredGrossInventoryChange *= -1;
											sublineItem.Quantity.NetInventoryChange *= -1;
											sublineItem.Quantity.DeliveredNetInventoryChange *= -1;
											sublineItem.Quantity.MassInventoryChange *= -1;
											sublineItem.Quantity.PackageInventoryChange *= -1;
										}
									}
								}
							}
						}
						break;
					}
			}
		}

		public virtual void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClause, string dbName)
		{
			if (!string.IsNullOrEmpty(dbName) && dbName[0] != '[')
			{
				dbName = "[" + dbName + "]";
			}

			string SQL =
				"DECLARE @SiteList TABLE ( Site [nvarchar] (50) NOT NULL ) " +

				"INSERT INTO @SiteList Select ID from dbo.tblSites, map.tblSiteToSite where ParentSiteGuid = @SiteGuid " +
				  "AND map.tblSiteToSite.ChildSiteGuid = dbo.tblSites.SiteGuid " +


				"DECLARE @AuthorizedCompanies TABLE ( [ID] [nvarchar] (100) NOT NULL ) " +
				"INSERT INTO @AuthorizedCompanies SELECT * FROM udf_AuthorizedCompanies(@LoginSiteGuid,@SiteGuid,@UserGuid) " +

				 "SELECT * FROM ( " +
				  "{0} " +
				  ",{1}..tblTransactions.[TransID] as EntityGuid " +
				  ",{1}..tblTransactions.[DeleteFlag] as 'MainTransactionDeleteFlag' " +
				  ",{1}..tblTransactions.[AliasName] as 'InternalAliasName' " +
				  ",{1}..tblTransactions.[LookupTransTypeIndex] as 'InternalTransTypeID' " +
				  ",{1}..tblTransactions.[ReversalType] as 'InternalReversalType' " +
				  ",{1}..tblTransactions.[SubType] as 'InternalSubType' " +
						",{1}..tblTransactions.[TransactionAliasGuid] as 'tblTransactions.TransactionAliasGuid'" +
						",{1}..tblTransactions.[SiteGuid] as 'InternalSiteGuidTimeZone'" +
						",{4} as 'tblTransactionLineItems.VolumeUnit' " +
				  ",(SELECT DISTINCT ShowDeletedTrxFlag from tblGeneralConfiguration WHERE SiteGuid = @SiteGuid) as ShowDeletedTrxFlag " +
				  ",(SELECT Notes FROM {1}..tblTransactionNotes WHERE {1}..tblTransactionNotes.TransactionGuid = {1}..tblTransactions.TransactionGuid) AS Notes " +

				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.Temperature,{2},{3}) AS 'tblTransactionLineItems.Temperature' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.GrossQuantity,{4},{5}) AS 'InternalGross' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.NetQuantity,{4},{5}) AS 'InternalNet' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.GrossQuantity,{4},{5}) AS 'tblTransactionLineItems.GrossQuantity'" +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.NetQuantity,{4},{5}) AS 'tblTransactionLineItems.NetQuantity'" +

					",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 13 AND {1}..tblTransactions.SubType = 'D' THEN ISNULL({1}..tblTransactions.ManagerID,'') " +
						"ELSE '' " +
					"END AS 'tblTransactions.FromManagerID' " +

					",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 13 AND {1}..tblTransactions.SubType = 'C' THEN ISNULL({1}..tblTransactions.ManagerID,'') " +
						"ELSE '' " +
					"END AS 'tblTransactions.ToManagerID' " +

					",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 13 AND {1}..tblTransactions.SubType = 'D' THEN ISNULL({1}..tblTransactions.OwnerID,'') " +
						"ELSE '' " +
					"END as 'tblTransactions.FromOwnerID' " +

					",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 13 AND {1}..tblTransactions.SubType = 'C' THEN ISNULL({1}..tblTransactions.OwnerID,'') " +
						"ELSE '' " +
					"END as 'tblTransactions.ToOwnerID' " +

					",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 13 AND {1}..tblTransactions.SubType = 'D' THEN ISNULL({1}..tblTransactions.CarrierID,'') " +
						"ELSE '' " +
					"END as 'tblTransactions.FromCarrierID' " +

					",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 13 AND {1}..tblTransactions.SubType = 'C' THEN ISNULL({1}..tblTransactions.CarrierID,'') " +
						"ELSE '' " +
					"END as 'tblTransactions.ToCarrierID' " +

						 ",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 15 AND {1}..tblTransactions.SubType = 'D' THEN ISNULL({1}..tblTransactionLineItems.Product,'') " +
							 "ELSE '' " +
						 "END as 'tblTransactionLineItems.FromProduct' " +

						 ",CASE " +
						"WHEN {1}..tblTransactions.LookupTransTypeIndex = 15 AND {1}..tblTransactions.SubType = 'C' THEN ISNULL({1}..tblTransactionLineItems.Product,'') " +
							 "ELSE '' " +
						 "END as 'tblTransactionLineItems.ToProduct' " +

					",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.MassQuantity,{10},{11}) as 'tblTransactionLineItems.MassQuantity' " +

				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.LineFill,{4},{5}) AS 'tblTransactionLineItems.LineFill' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.BottomVolume,{4},{5}) AS 'tblTransactionLineItems.BottomVolume' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.NetCapacity,{4},{5}) AS 'tblTransactionLineItems.NetCapacity' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.ReceiptVariance,{4},{5}) AS 'tblTransactionLineItems.ReceiptVariance' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.LoadRackVariance,{4},{5}) AS 'tblTransactionLineItems.LoadRackVariance' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.PresetAmount,{4},{5}) AS 'tblTransactionLineItems.PresetAmount' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.Density,{6},{7}) AS 'tblTransactionLineItems.Density' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.DifferentialPressure,{8},{9}) AS 'tblTransactionLineItems.DifferentialPressure' " +
				  ",dbo.udf_ConvertFromSIUnits({1}..tblTransactionLineItems.FreezePoint,{2},{3}) AS 'tblTransactionLineItems.FreezePoint' " +
				  ",DATEDIFF(minute,tblTransactions.RequestedDateTime,tblTransactions.TimeIn) as 'tblTransactions.ResponseTime' " +
				  ",DATEDIFF(minute,tblTransactions.FST,tblTransactions.TimeEnd) as 'tblTransactions.FuelTime' " +
						",etd1.EqTypeName as 'tblTransactions.DestinationEquipmentSecondaryType1' " +
						",etd2.EqTypeName as 'tblTransactions.DestinationEquipmentSecondaryType2' " +
						",etd3.EqTypeName as 'tblTransactions.DestinationEquipmentSecondaryType3' " +
						",ets1.EqTypeName as 'tblTransactions.SourceEquipmentSecondaryType1' " +
						",ets2.EqTypeName as 'tblTransactions.SourceEquipmentSecondaryType2' " +
						",ets3.EqTypeName as 'tblTransactions.SourceEquipmentSecondaryType3' " +

				  "FROM {1}..tblTransactions WITH(NOLOCK) " +
				  "LEFT OUTER JOIN {1}..tblTransactionLineItems WITH(NOLOCK) ON {1}..tblTransactions.TransactionGuid = {1}..tblTransactionLineItems.TransactionGuid " +
				  "LEFT JOIN {1}..tblSites WITH(NOLOCK) ON {1}..tblTransactions.SiteGuid = {1}..tblSites.SiteGuid " +
						"LEFT JOIN {1}..tblTransactionAliases WITH(NOLOCK) ON {1}..tblTransactionAliases.TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', {1}..tblTransactions.TransactionAliasGuid, @SiteGuid) " +
						"LEFT JOIN {1}..tblProducts WITH(NOLOCK) ON {1}..tblProducts.ProductGuid = {1}..tblTransactionLineItems.ProductGuid " +
						"LEFT JOIN {1}..tblTransactionUserData WITH(NOLOCK) ON {1}..tblTransactionUserData.TransactionGuid = {1}..tblTransactions.TransactionGuid " +
				  "LEFT JOIN {1}..tblTransactionLineItemUserData WITH(NOLOCK) ON {1}..tblTransactionLineItems.TransactionLineItemGuid = {1}..tblTransactionLineItemUserData.TransactionLineItemGuid " +
				  "LEFT JOIN {1}..tblExportResultDetails ON {1}..tblExportResultDetails.RecordID = {1}..tblTransactions.TransID " +
					"AND {1}..tblTransactions.TransVersion = tblExportResultDetails.TransVersion " +
					"AND (tblExportResultDetails.ExportResultGuid IN (SELECT ex.ExportResultGuid FROM {1}..tblExportResultDetails ex LEFT OUTER JOIN {1}..tblExportResults e On ex.ExportResultGuid = e.ExportResultGuid " +
					"WHERE InterfaceName = 'SAPTransactionResult' AND Fail = CAST(0 as BIT))) " +
				"LEFT OUTER JOIN {1}..tblEquipment ed1 WITH (NOLOCK) ON ed1.EquipmentGuid = {1}..tblTransactions.Destination1EquipmentGuid " +
						"LEFT OUTER JOIN {1}..tblEquipmentTypes etd1 WITH (NOLOCK) ON etd1.EquipmentTypeGuid = ed1.EquipmentTypeGuid " +
						"LEFT OUTER JOIN {1}..tblEquipment ed2 WITH (NOLOCK) ON ed2.EquipmentGuid = {1}..tblTransactions.Destination2EquipmentGuid " +
						"LEFT OUTER JOIN {1}..tblEquipmentTypes etd2 WITH (NOLOCK) ON etd2.EquipmentTypeGuid = ed2.EquipmentTypeGuid " +
						"LEFT OUTER JOIN {1}..tblEquipment ed3 WITH (NOLOCK) ON ed3.EquipmentGuid = {1}..tblTransactions.Destination3EquipmentGuid " +
						"LEFT OUTER JOIN {1}..tblEquipmentTypes etd3 WITH (NOLOCK) ON etd3.EquipmentTypeGuid = ed3.EquipmentTypeGuid " +
						"LEFT OUTER JOIN {1}..tblEquipment es1 WITH (NOLOCK) ON es1.EquipmentGuid = {1}..tblTransactions.Source1EquipmentGuid " +
						"LEFT OUTER JOIN {1}..tblEquipmentTypes ets1 WITH (NOLOCK) ON ets1.EquipmentTypeGuid = es1.EquipmentTypeGuid " +
						"LEFT OUTER JOIN {1}..tblEquipment es2 WITH (NOLOCK) ON es2.EquipmentGuid = {1}..tblTransactions.Source2EquipmentGuid " +
						"LEFT OUTER JOIN {1}..tblEquipmentTypes ets2 WITH (NOLOCK) ON ets2.EquipmentTypeGuid = es2.EquipmentTypeGuid " +
						"LEFT OUTER JOIN {1}..tblEquipment es3 WITH (NOLOCK) ON es3.EquipmentGuid = {1}..tblTransactions.Source3EquipmentGuid " +
						"LEFT OUTER JOIN {1}..tblEquipmentTypes ets3 WITH (NOLOCK) ON ets3.EquipmentTypeGuid = es3.EquipmentTypeGuid " +

						"WHERE (Site IN (SELECT * FROM @SiteList)) " +
				  "AND (  " +
					 "{1}..tblTransactions.CarrierID IN (SELECT * from @AuthorizedCompanies) " +
					 "OR {1}..tblTransactions.ShipperID IN (SELECT * from @AuthorizedCompanies) " +
					 "OR {1}..tblTransactions.ShipToID IN (SELECT * from @AuthorizedCompanies) " +
					 "OR {1}..tblTransactions.SupplierID IN (SELECT * from @AuthorizedCompanies) " +
					 "OR {1}..tblTransactions.ManagerID IN (SELECT * from @AuthorizedCompanies) " +
					 "OR {1}..tblTransactions.OwnerID IN (SELECT * from @AuthorizedCompanies) " +
					 "OR {1}..tblTransactions.BillToID IN (SELECT * from @AuthorizedCompanies) " +
				  ") " +
				") tblCombinedTable WHERE (ShowDeletedTrxFlag=1 OR MainTransactionDeleteFlag=0)";

			string getUnitsTemplate = "dbo.udf_GetUnitsIndex({1}..tblProducts.{0}, {1}..tblTransactionAliases.{0}, {1}..tblSites.{0})";
			string getDecimalsTemplate = "dbo.udf_GetDecimalPlaces({1}..tblProducts.{0}, {1}..tblTransactionAliases.{0}, {1}..tblSites.{0})";
			string getVolumeUnitsTemplate = "dbo.udf_GetVolumeUnitsIndex({2}..tblProducts.LookupProductTypeIndex, {2}..tblProducts.{0}, {2}..tblTransactionAliases.{0}, {2}..tblSites.{0}, {2}..tblTransactionAliases.{1}, {2}..tblSites.{1})";
			string getVolumeDecimalsTemplate = "dbo.udf_GetVolumeDecimalPlaces({2}..tblProducts.LookupProductTypeIndex, {2}..tblProducts.{0}, {2}..tblTransactionAliases.{0}, {2}..tblSites.{0}, {2}..tblTransactionAliases.{1}, {2}..tblSites.{1})";

			string tempUnits = string.Format(getUnitsTemplate, "TemperatureUnitIndex", dbName);
			string tempDec = string.Format(getDecimalsTemplate, "TemperatureDecimalPlaces", dbName);

			string volUnits = string.Format(getVolumeUnitsTemplate, "VolumeUnitIndex", "AdditiveVolumeUnitIndex", dbName);
			string volDec = string.Format(getVolumeDecimalsTemplate, "VolumeDecimalPlaces", "AdditiveVolumeDecimalPlaces", dbName);

			string densityUnits = string.Format(getUnitsTemplate, "DensityUnitIndex", dbName);
			string densityDec = string.Format(getDecimalsTemplate, "DensityDecimalPlaces", dbName);

			string pressureUnits = string.Format(getUnitsTemplate, "PressureUnitIndex", dbName);
			string pressureDec = string.Format(getDecimalsTemplate, "PressureDecimalPlaces", dbName);

			string massUnits = string.Format(getUnitsTemplate, "MassUnitIndex", dbName);
			string massDec = string.Format(getDecimalsTemplate, "MassDecimalPlaces", dbName);

			cmd.CommandText = string.Format(SQL,
										selectClause,  // {0}
										dbName,        // {1}
										tempUnits,     // {2}
										tempDec,       // {3}
										volUnits,      // {4}
										volDec,        // {5}
										densityUnits,  // {6}
										densityDec,    // {7}
										pressureUnits, // {8}
										pressureDec,   // {9}
										massUnits, // {10}
										massDec); // {11}

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@LoginSiteGuid", security.LoginSiteGuid);

			cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);

			if (security.UserGuid == Guid.Empty)
			{
				cmd.Parameters["@UserGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@UserGuid"].Value = security.UserGuid;
			}
		}

		/// <summary>
		/// Queries the alias fields.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="fields">The fields.</param>
		/// <returns>A collection of aliased fields.</returns>
		[SecurityCritical]
		public QueryWriterFieldCollection QueryAliasFields(SecurityClass security, QueryWriterFieldCollection fields, QueryWriterAliasGuidCollection aliasGuids)
		{
			// Start by blanking all the field display values
			foreach (QueryWriterField field in fields)
			{
				field.DisplayName = string.Empty;
			}

			var groupCollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x =>
				 x.EnumerateByUser(security, security.UserGuid));

			var aliasFields = new List<FieldCollectionClass>();

			if (aliasGuids != null && aliasGuids.Count > 0)
			{
				foreach (QueryWriterAliasGuid aliasGuid in aliasGuids)
				{
					FMChannelHelper.MakeCall<ITransactionAliases>(
						x => this.AddMainAliasFields(security, x, groupCollection, aliasFields, aliasGuid.AliasGuid));
				}
			}
			else
			{
				FMChannelHelper.MakeCall<ITransactionAliases>(x => this.AddMainAliasFields(security, x, groupCollection, aliasFields, Guid.Empty));
			}



			// JS20100812 WI-16687 remove censor fields from being selectable
			TransactionAliasFieldCollectionClass censorByRight = this.GetCensorFieldsByRights(security);
			foreach (FieldCollectionClass fieldCollection in aliasFields)
			{
				if (fieldCollection.GetType() == typeof(TransactionAliasFieldCollectionClass))
				{
					var col = fieldCollection as TransactionAliasFieldCollectionClass;
					foreach (TransactionAliasFieldClass censorField in censorByRight)
					{
						this.RemoveCensorField(col, censorField);
					}
				}
			}

			var newCollection = new QueryWriterFieldCollection();

			foreach (var field in fields)
			{
				this.SetDisplayNames(aliasFields, field);
				if (string.IsNullOrEmpty(field.DisplayName) == false)
				{
					newCollection.Add(field);
				}
				else if (field.FieldName.Equals("TransID"))
				{
					// WI-16495 Add Transaction ID to the list because it is so useful but will
					// never show up because it is not contained within a real control
					field.DisplayName = "Transaction ID";
					newCollection.Add(field);
				}
				else if (field.FieldName.Equals("Notes"))
				{
					// WI-17158 Add Notes to the list because it's on every transaction but not technically
					// a configurable transaction alias field either
					field.DisplayName = "Notes";
					newCollection.Add(field);
				}
			}

			return newCollection;
		}
		/// <summary>
		/// Adds the main alias fields.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="aliases">The aliases.</param>
		/// <param name="groupCollection">The group collection.</param>
		/// <param name="aliasFields">The alias fields.</param>
		private void AddMainAliasFields( SecurityClass security, ITransactionAliases aliases, GroupCollectionClass groupCollection, List<FieldCollectionClass> aliasFields, Guid aliasGuid )
		{
			TransactionAliasCollectionClass aliasCollection;

			if (aliasGuid != Guid.Empty)
			{
				aliasCollection = new TransactionAliasCollectionClass();
				aliasCollection.Add(aliases.Get(security, aliasGuid, false));
			}
			else
			{
				aliasCollection = aliases.Enumerate(security);
			}

			FMChannelHelper.MakeCall<ITransactionAliasFields>(
				x => this.AddAliasCollection(security, x, aliases, groupCollection, aliasFields, aliasCollection));
		}

		/// <summary>
		/// Adds the alias collection.
		/// </summary>
		/// <param name="security">The security.</param>
		/// <param name="aliasFields">The tx alias fields.</param>
		/// <param name="aliases">The aliases.</param>
		/// <param name="groupCollection">The group collection.</param>
		/// <param name="aliasFieldList">The alias fields.</param>
		/// <param name="aliasCollection">The alias collection.</param>
		private void AddAliasCollection( SecurityClass security, ITransactionAliasFields aliasFields, ITransactionAliases aliases, GroupCollectionClass groupCollection, List<FieldCollectionClass> aliasFieldList, TransactionAliasCollectionClass aliasCollection )
		{
			foreach ( var transactionAlias in aliasCollection )
			{
				TransactionAliasClass fullAlias = aliases.Get( security, transactionAlias.IdentityGuid, false );

				// Only include aliases assigned for the current user
				if ( fullAlias.GroupTransactionAliasMapCollection.Count == 0 || this.UserHasAccessToGroup( fullAlias, groupCollection ) )
				{
					TransactionAliasFieldCollectionClass transactionFieldCollection			= aliasFields.Enumerate( security, transactionAlias.IdentityGuid, TransactionFieldType.Transaction, false, false );
					TransactionAliasFieldCollectionClass lineItemFieldCollection			= aliasFields.Enumerate( security, transactionAlias.IdentityGuid, TransactionFieldType.LineItem, false, false );
					TransactionAliasFieldCollectionClass exportResultFieldCollection		= aliasFields.Enumerate(security, transactionAlias.IdentityGuid, TransactionFieldType.ExportResult, false, false);
					TransactionAliasFieldCollectionClass transportLineItemFieldCollection	= aliasFields.Enumerate(security, transactionAlias.IdentityGuid, TransactionFieldType.TransportInfo, false, false);

					aliasFieldList.Add(transactionFieldCollection);
					aliasFieldList.Add(lineItemFieldCollection);
					aliasFieldList.Add(exportResultFieldCollection);
					aliasFieldList.Add(transportLineItemFieldCollection);

					aliasFieldList.Add( fullAlias.UserDataFieldCollection );

					// JS20101001 WI-18005 add the line item user data fields
					aliasFieldList.Add( fullAlias.LineItemUserDataFieldCollection );
				}
			}
		}

		private void RemoveCensorField(TransactionAliasFieldCollectionClass col, TransactionAliasFieldClass censorField)
		{
			for (int index = 0; index < col.Count; ++index)
			{
				if (col.Item(index).DbName.Equals(censorField.DbName))
				{
					col.RemoveAt(index);
					break;
				}
			}
		}

		private bool UserHasAccessToGroup(TransactionAliasClass alias, GroupCollectionClass groupcollection)
		{
			foreach (GroupTransactionAliasMapClass group in alias.GroupTransactionAliasMapCollection)
			{
				foreach (GroupClass userGroup in groupcollection)
				{
					if (userGroup.IdentityGuid == group.GroupGuid)
					{
						return true;
					}
				}
			}

			return false;

		}

		protected void SetDisplayNames(List<FieldCollectionClass> AliasFields,
												QueryWriterField Field)
		{
			foreach (FieldCollectionClass AliasFieldCollection in AliasFields)
			{
				foreach (FieldClass AliasField in AliasFieldCollection)
				{
					string dbName = AliasField.DbName;

					if (AliasField.GetType().Equals(typeof(TransactionAliasFieldClass)))
					{
						if ((AliasField as TransactionAliasFieldClass).Type == TransactionFieldType.Transaction)
						{
							dbName = "tblTransactions." + dbName;
						}
						else if ((AliasField as TransactionAliasFieldClass).Type == TransactionFieldType.LineItem)
						{
							dbName = "tblTransactionLineItems." + dbName;
						}
						else
						{
                            dbName = "tblExportResultDetails." + dbName;
                        }
					}
					else if (AliasField.GetType().Equals(typeof(UserDataFieldClass)))
					{
						dbName = string.Format("UserData{0}", (AliasField as UserDataFieldClass).Number + 1);
						if ((AliasField as UserDataFieldClass).UserDataEntityType == ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM)
						{
							dbName = "tblTransactionLineItemUserData." + dbName;
						}
						else
						{
							dbName = "tblTransactionUserData." + dbName;
						}
					}

					dbName = dbName.ToUpper();

					// Search the Query field list and see if the field already exists with this display name
					if (dbName.Equals(Field.DBFieldName.ToUpper()))
					{
						// Only add name if it isn't already in the list
						string testValue = AliasField.DisplayName;
						if (testValue[testValue.Length - 1].Equals(':'))
						{
							testValue = testValue.Substring(0, testValue.Length - 1);
						}

						if (Field.DisplayName.Contains(testValue) == false)
						{
							Field.DisplayName += string.IsNullOrEmpty(Field.DisplayName) ? string.Empty : "/";
							Field.DisplayName += testValue;
						}

					}

				}

			}

		}

		public void QueryWriterPreProcess(SecurityClass security, DataSet set)
		{
			SetDisplaySigns(security, set);
		}

		public void QueryWriterPostProcess(SecurityClass security, DataSet set)
		{
			ProduceStatisticsRow(security, set);
		}

        /// <summary>
        /// Don't let the query writer try to total the InternalTransTypeID field.
        /// Since it's an internal field, we don't need to total it.
        /// Totalling the field may result in overflowing the max size for the field (int16)
        /// </summary>
	    public static List<string> QueryWriterExcludedTotalFields
        {
            get
            {
                return new List<string> { "InternalTransTypeID" };
            }
        }

        public string QueryWriterTotalsFilter
		{
			get
			{
				return "(InternalSubType IS NULL OR InternalSubType IN ('','C'))";
			}
		}

		/// <summary>
		/// The set display signs.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="set">
		/// The set.
		/// </param>
		private void SetDisplaySigns(SecurityClass security, DataSet set)
		{
			var lineItem = new LineItemDO();
			this.lineItems.Add(lineItem);

			foreach (DataRow row in set.Tables[0].Rows)
			{
				if (row["Internal__RowType"].ToString().Equals("DataRow"))
				{
					lineItem.Quantity.NetInventoryChange = getDouble(row["tblTransactionLineItems.NetQuantity"]);
					lineItem.Quantity.GrossInventoryChange = getDouble(row["tblTransactionLineItems.GrossQuantity"]);
					lineItem.Quantity.MassInventoryChange = getDouble(row["tblTransactionLineItems.MassQuantity"]);
					this.transTypeID = getValue<TransactionTypes>(row["InternalTransTypeID"], TransactionTypes.TransactionType_None);
					this.ReversalType = getString(row["InternalReversalType"]);
					this.SubType = getString(row["InternalSubType"]);

					// used by query writer only to keep signs correct for query results for these particular transactions
					if ( this.transTypeID == TransactionTypes.T11_ConsumerTransfer
						|| this.transTypeID == TransactionTypes.T13_OwnerTransfer
						|| this.transTypeID == TransactionTypes.T15_PrimaryRegrade
						|| this.transTypeID == TransactionTypes.T23_StorageTransfer
						|| this.transTypeID == TransactionTypes.T16_SecondaryRegrade )
					{
						this.SetVolumeSigns(false);
					}
					else
					{
						this.SetVolumeSigns(true);
					}

					row["tblTransactionLineItems.NetQuantity"] = lineItem.Quantity.NetInventoryChange;
					row["tblTransactionLineItems.GrossQuantity"] = lineItem.Quantity.GrossInventoryChange;
					row["tblTransactionLineItems.MassQuantity"] = lineItem.Quantity.MassInventoryChange;
				}
			}
		}

		/// <summary>
		/// The produce statistics row.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="set">
		/// The set.
		/// </param>
		private void ProduceStatisticsRow(SecurityClass security, DataSet set)
		{
			if (set.Tables[0].Columns["tblTransactionLineItems.GrossQuantity"] != null)
			{
				DataTable resultsTable = set.Tables[0];

				// Create a table for returning the results
				var statisticsTable = new DataTable();
				statisticsTable.Columns.Add(new DataColumn("TotalQuantity", Type.GetType("System.String")));
				statisticsTable.Columns.Add(new DataColumn("AverageQuantity", Type.GetType("System.String")));
				statisticsTable.Columns.Add(new DataColumn("MaxIssue", Type.GetType("System.String")));
				statisticsTable.Columns.Add(new DataColumn("MinIssue", Type.GetType("System.String")));

				DataRow row = statisticsTable.NewRow();

				string whereClause = string.Format("{0} = '{1}' AND MainTransactionDeleteFlag = 0 AND {2}", QueryClass.ROW_TYPE, QueryRowType.DataRow.ToString(), QueryWriterTotalsFilter);

				row["TotalQuantity"] = resultsTable.Compute("sum([tblTransactionLineItems.GrossQuantity])", whereClause);
				row["AverageQuantity"] = string.Format("{0:0.00}", resultsTable.Compute("avg([tblTransactionLineItems.GrossQuantity])", whereClause));
				row["MaxIssue"] = resultsTable.Compute("max([tblTransactionLineItems.GrossQuantity])", whereClause);
				row["MinIssue"] = resultsTable.Compute("min([tblTransactionLineItems.GrossQuantity])", whereClause);

				statisticsTable.Rows.Add(row);

				set.Tables.Add(statisticsTable);
			}
		}

		/// <summary>
		/// The get censor fields by rights.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasFieldCollectionClass"/>.
		/// </returns>
		protected TransactionAliasFieldCollectionClass GetCensorFieldsByRights(SecurityClass security)
		{
			var censorFields = new TransactionAliasFieldCollectionClass();

			var isAdf = FMChannelHelper.MakeCall<IHardwareKey, bool>(x => x.IsADFKey());

			// JS20100812 WI-16687 filter out financial fields if user do not have the right
			if (!security.HasRight(RIGHT.VIEW_FINANCIAL_DATA))
			{
				censorFields = FMChannelHelper.MakeCall<ITransactionAliases, TransactionAliasFieldCollectionClass>(
					x => this.ProcessCensorFields(x, security, isAdf));
			}

			return censorFields;
		}

		/// <summary>
		/// The process censor fields.
		/// </summary>
		/// <param name="aliases">
		/// The aliases.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="isAdf">
		/// The is ADF.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionAliasFieldCollectionClass"/>.
		/// </returns>
		private TransactionAliasFieldCollectionClass ProcessCensorFields(ITransactionAliases aliases, SecurityClass security, bool isAdf)
		{
			var censorFields = new TransactionAliasFieldCollectionClass();

			TransactionAliasCollectionClass aliasCollection = aliases.Enumerate( security );

			foreach ( TransactionAliasClass alias in aliasCollection )
			{
				TransactionAliasClass fullAlias = aliases.Get( security, alias.IdentityGuid, false );
				foreach ( TransactionAliasFieldClass field in fullAlias.TransactionFieldCollection )
				{
					if (field.IsFinancialField)
					{
						censorFields.Add( field );
					}
				}

				foreach ( TransactionAliasFieldClass field in fullAlias.LineItemFieldCollection )
				{
					// ADF only, also remove number fields for sales and issues
					if ( isAdf &&
					( field.ID.ToUpper().Equals( "NUMBER01" ) ||
					 field.ID.ToUpper().Equals( "NUMBER02" ) ||
					 field.ID.ToUpper().Equals( "NUMBER03" ) ||
					 field.ID.ToUpper().Equals( "NUMBER04" ) ||
					 field.ID.ToUpper().Equals( "NUMBER05" ) ||
					 field.ID.ToUpper().Equals( "NUMBER06" ) ) &&
					( fullAlias.ID.ToUpper().Contains( "SALE" ) ||
					 fullAlias.ID.ToUpper().Contains( "ISSUE" ) )
						)
					{
						censorFields.Add( field );
					}
					else if (field.IsFinancialField)
					{
						censorFields.Add( field );
					}
				}
			}

			return censorFields;
		}

		/// <summary>
		/// The censor fields if necessary.
		/// </summary>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="set">
		/// The set.
		/// </param>
		private void CensorFieldsIfNecessary(SecurityClass security, DataSet set)
		{
			// Get a list of alias definitions 
			var censorFields = new TransactionAliasFieldCollectionClass();

			// Determine if there are fields we need to censor based on security access
			FMChannelHelper.MakeCall<ITransactionAliases>(
				aliases =>
					{
						var aliasCollection = aliases.Enumerate(security);
						foreach (TransactionAliasClass alias in aliasCollection)
						{
							TransactionAliasClass fullAlias = aliases.Get(security, alias.IdentityGuid, false);

							GatherCensoredFields(censorFields, fullAlias);
						}
					});

			var groupcollection = FMChannelHelper.MakeCall<IGroups, GroupCollectionClass>(x => x.EnumerateByUser(security, security.UserGuid));

			// Go through the fields and remove any from the list where the user already
			// has access through the group setting
			for (int index = censorFields.Count - 1; index >= 0; --index)
			{
				TransactionAliasFieldClass checkField = censorFields[index];

				if (this.UserHasAccessToField(checkField, groupcollection))
				{
					censorFields.RemoveAt(index);
				}
			}

			// JS20100812 WI-16687 censor fields for existing queries
			TransactionAliasFieldCollectionClass censorByRight = this.GetCensorFieldsByRights(security);
			foreach (TransactionAliasFieldClass field in censorByRight)
			{
				censorFields.Add(field);
			}

			// Now go through the data set and blank out the applicable rows
			if (censorFields.Count > 0)
			{
				// Find the field in the dataset
				foreach (DataRow row in set.Tables[0].Rows)
				{
					foreach (TransactionAliasFieldClass field in censorFields)
					{
						CensorField(set, row, field);
					}
				}
			}
		}

		/// <summary>
		/// The censor field.
		/// </summary>
		/// <param name="set">
		/// The set.
		/// </param>
		/// <param name="row">
		/// The row.
		/// </param>
		/// <param name="field">
		/// The field.
		/// </param>
		private static void CensorField(DataSet set, DataRow row, TransactionAliasFieldClass field)
		{
			string testString = getValue<string>(row["InternalAliasName"], string.Empty);

			if (testString.Equals(field.AliasName))
			{
				string dbName = field.DbName;

				if (field.DisplayOrder == 0)
				{
					dbName = "tblTransactions." + dbName;
				}
				else if (field.DisplayOrder == 1)
				{
					dbName = "tblTransactionLineItems." + dbName;
				}
				else if (field.DisplayOrder == 2)
				{
					dbName = "tblTransactionUserData." + dbName;
				}

				if (set.Tables[0].Columns[dbName] != null)
				{
					row[dbName] = DBNull.Value;
				}
			}
		}

		/// <summary>
		/// The gather censored fields.
		/// </summary>
		/// <param name="censorFields">
		/// The censor fields.
		/// </param>
		/// <param name="fullAlias">
		/// The full alias.
		/// </param>
		private static void GatherCensoredFields(TransactionAliasFieldCollectionClass censorFields, TransactionAliasClass fullAlias)
		{
			// Get the detail level fields
			var groupFields = from F in fullAlias.TransactionFieldCollection
									where F.UserGroupGuid != Guid.Empty
									select F;

			foreach ( var groupField in groupFields )
			{
				// Use the DisplayOrder field to save the type for concatenating a table name to the
				// field name later in this routine.
				groupField.DisplayOrder = 0;
				censorFields.Add(groupField);
			}

			// Get the line item fields
			groupFields = from F in fullAlias.LineItemFieldCollection
							  where F.UserGroupGuid != Guid.Empty
							  select F;

			foreach (var groupField in groupFields)
			{
				// Use the DisplayOrder field to save the type for concatenating a table name to the
				// field name later in this routine.
				groupField.DisplayOrder = 1;
				censorFields.Add(groupField);
			}

			// Get the user data fields
			var userFields = from F in fullAlias.UserDataFieldCollection
								  where F.UserGroupGuid != Guid.Empty
								  select F;

			foreach (var userField in userFields)
			{
				// Use the DisplayOrder field to save the type for concatenating a table name to the
				// field name later in this routine.  Also, since for some reason a user data field
				// is not consonidered a transaction alias field type, we have to transfer information
				// to another, more convenient object.
				var newField = new TransactionAliasFieldClass
					               {
						               DisplayOrder = 2,
						               AliasName = fullAlias.ID,
						               UserGroupGuid = userField.UserGroupGuid,
						               DbName = userField.DbName
					               };

				censorFields.Add(newField);
			}
		}

		/// <summary>
		/// The user has access to field.
		/// </summary>
		/// <param name="checkField">
		/// The check field.
		/// </param>
		/// <param name="groupcollection">
		/// The group collection.
		/// </param>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		private bool UserHasAccessToField(TransactionAliasFieldClass checkField, GroupCollectionClass groupcollection)
		{
			foreach (GroupClass group in groupcollection)
			{
				if (group.IdentityGuid == checkField.UserGroupGuid)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// The detail page reference.
		/// </summary>
		/// <returns>
		/// The <see cref="string"/>.
		/// </returns>
		public string DetailPageReference()
		{
			return ConfigurationManager.AppSettings["AccountingTransactionDetailURL"];
		}

		#endregion

		#region Methods to handle whether a property should be serialized.
		/// <summary>
		/// This method causes the Number 01 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber01( )
		{
			return this.number01.HasValue;
		}

		/// <summary>
		/// This method causes the Number 02 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber02( )
		{
			return this.number02.HasValue;
		}

		/// <summary>
		/// This method causes the Number 03 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber03( )
		{
			return this.number03.HasValue;
		}

		/// <summary>
		/// This method causes the Number 04 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber04( )
		{
			return this.number04.HasValue;
		}

		/// <summary>
		/// This method causes the Number 05 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber05( )
		{
			return this.number05.HasValue;
		}

		/// <summary>
		/// This method causes the Number 06 property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeNumber06( )
		{
			return this.number06.HasValue;
		}

		/// <summary>
		/// This method causes the Estimated Fueling Duration property to not be serialized if it
		/// is null. The reason we have to do this is because the xsd validator
		/// fails if the element is set to xsi:nillable = true. This is a
		/// Microsoft's solution.
		/// </summary>
		/// <returns>
		/// The <see cref="bool"/>.
		/// </returns>
		public bool ShouldSerializeEstimatedFuelingDuration( )
		{
			return this.EstimatedFuelingDuration.HasValue;
		}
		#endregion

		/// <summary>
		/// Gets the TransactionDo property name associated with the specified database column name.
		/// </summary>
		/// <param name="databaseColumnName">The database column name of the property</param>
		/// <returns>The property name associated with the specified database column name</returns>
		public static string GetPropertyName(string databaseColumnName)
		{
			string propertyName;
			if ( DbNameToPropertyMap.TryGetValue(databaseColumnName, out propertyName) )
			{
				return propertyName;
			}

			return databaseColumnName;
		}

		/// <summary>
		/// Finds the line item.
		/// </summary>
		/// <param name="lineItemID">The line item unique identifier.</param>
		/// <returns>The specified line item data object or null.</returns>
		public LineItemDO FindLineItem( string lineItemID )
		{
			var itemID = new Guid(lineItemID);

			foreach ( LineItemDO item in this.LineItems )
			{
				if ( item.TransactionLineItemGuid == itemID )
				{
					return item;
				}
			}

			return null;
		}

		public override string ToString()
		{
			return this.transID;
		}
	}

	#region Transaction DO Collection
	/// <summary>
	/// The transaction data object collection.
	/// </summary>
	[KnownType(typeof(TransactionDO))]
	[CollectionDataContract]
	public class TransactionDOCollection : CollectionBase
	{
		/// <summary>
		/// The this.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <returns>
		/// The <see cref="TransactionDO"/>.
		/// </returns>
		public TransactionDO this[int index]
		{
			get
			{
				return this.List[index] as TransactionDO;
			}

			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// The add.
		/// </summary>
		/// <param name="transactionDO">
		/// The transaction data object.
		/// </param>
		public void Add(TransactionDO transactionDO)
		{
			List.Add(transactionDO);
		}

		/// <summary>
		/// The remove.
		/// </summary>
		/// <param name="index">
		/// The index.
		/// </param>
		/// <exception cref="Exception">
		/// Invalid Index count.
		/// </exception>
		public void Remove(int index)
		{
			if (index > this.Count - 1 || index < 0)
			{
				throw new Exception("Invalid Index");
			}
			
			List.RemoveAt(index);
		}
	}
	#endregion
}
