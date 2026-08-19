// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BaseTransactionDO.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the BaseTransactionDO type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Runtime.Serialization;
	using System.Xml.Serialization;

	using FMBusinessObjects.UtilityObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

    /// <summary>
	/// The ticket modes.
	/// </summary>
	public enum TicketModes
	{
		Unknown,
		Manual,
		Auto
	}

	public enum PrimaryTransactionTypes
	{
		NonTransfer,
		Debit,
		Credit
	}

	/// <summary>
	/// The base transaction data object.
	/// </summary>
	[XmlType("BaseTransaction")]
	[DataContract]
	[Serializable]
	[KnownType(typeof(TransactionDO))]
	[KnownType(typeof(Dictionary<string, string>))]
	[KnownType(typeof(GregorianCalendar))]
	public abstract class BaseTransactionDO : DataObject
	{
		#region Constants
		protected const string TimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";
		#endregion

		#region Public data members
		[DataMember]
		public bool AssociatedOrderTx = false;
		[DataMember]
		public string AssociatedOrderProduct = string.Empty;
		#endregion

		#region Protected data members
		[DataMember]
		protected string transID;
		[DataMember]
		protected Guid transactionGuid;
		[DataMember]
		protected Guid conjoinedTransactionGuid;
		[DataMember]
		protected string referenceID;
		[DataMember]
		protected string aliasName;
		[DataMember]
		protected TransactionTypes transTypeID;
		[DataMember]
		protected string subType;
		[DataMember]
		protected string site;
		[DataMember]
		protected string transRefID;
		[DataMember]
		protected DateTime inventoryDate;
		[DataMember]
		protected DateTimeOffset? transactionDateTime;
		[DataMember]
		protected string ownerID;
		[DataMember]
		protected string ownerCode;
		[DataMember]
		protected string managerID;
		[DataMember]
		protected string managerCode;
		[DataMember]
		protected string shipToID;
		[DataMember]
		protected string shipToCode;
		[DataMember]
		protected string billToID;
		[DataMember]
		protected string billToCode;
		[DataMember]
		protected string shipperID;
		[DataMember]
		protected string shipperCode;
		[DataMember]
		protected string supplierID;
		[DataMember]
		protected string supplierCode;
		[DataMember]
		protected string carrierID;
		[DataMember]
		protected string carrierCode;
		[DataMember]
		protected string scacCode;
		[DataMember]
		protected DateTimeOffset requestedByDate;
		[DataMember]
		protected string notes;
		[DataMember]
		protected string additionalInformation;
		[DataMember]
		protected Dictionary<string, string> userDataTable;

		[DataMember]
		protected string ticketSource;
		[DataMember]
		protected TicketModes ticketMode;
		[DataMember]
		protected string documentNumber;
		[DataMember]
		protected string linkedDocumentNumber;
		[DataMember]
		protected string shippingDocumentNumber;
		[DataMember]
		protected string shipmentNumber;
		[DataMember]
		protected string reversedTransID;
		[DataMember]
		protected string reversalType = TransactionDO.None;
		[DataMember]
		protected string conjoinedTransID;

		[DataMember]
		protected string poNumber;
		[DataMember]
		protected string driverIDNumber;

		[DataMember]
		protected RouteInfoDO routeInfo;
		[DataMember]
		protected RouteScheduleDO routeSchedule;
		[DataMember]
		protected DateTimeOffset? timeIn;
		[DataMember]
		protected DateTimeOffset? timeOut;
		[DataMember]
		protected DateTimeOffset? timeEnd;
		[DataMember]
		protected DateTimeOffset? requestedDeliveryDate;
		[DataMember]
		protected string loadID;
		[DataMember]
		protected PaymentInfoDO paymentInfo;

		[DataMember]
		protected DateTime? closeoutDate;
		[DataMember]
		protected bool partialCloseout;
		[DataMember]
		protected DateTimeOffset enterpriseDate;
		[DataMember]
		protected long transversion;
		[DataMember]
		protected TransactionStatus status;
		[DataMember]
		protected bool deleteFlag;

		[DataMember]
		protected EquipmentDO destinationEQ1;
		[DataMember]
		protected EquipmentDO destinationEQ2;
		[DataMember]
		protected EquipmentDO destinationEQ3;
		[DataMember]
		protected EquipmentDO sourceEQ1;
		[DataMember]
		protected EquipmentDO sourceEQ2;
		[DataMember]
		protected EquipmentDO sourceEQ3;

		[DataMember]
		protected DateTimeOffset? effectiveDate;
		[DataMember]
		protected DateTimeOffset? expirationDate;
		[DataMember]
		protected DateTimeOffset? scheduledDate;

		[DataMember]
		protected bool autoComplete;

		[DataMember]
		protected string operatorID;

		[DataMember]
		protected string operatorName;

		[DataMember]
		protected DateTimeOffset createdDate;
		[DataMember]
		protected string createdBy;
		[DataMember]
		protected DateTimeOffset updatedDate;
		[DataMember]
		protected string updatedBy;

		[DataMember]
		protected List<LineItemDO> lineItems;
		[DataMember]
		protected List<WeightReadingDO> weightReadings;
		[DataMember]
		protected List<TransportLineItemDO> transportInfoList;
		[DataMember]
		protected byte[] signature;

		// 05-20-08	vthompson: Adding fields for ADF.  These are to be configurable
		// boolean fields
		[DataMember]
		protected bool flag01;
		[DataMember]
		protected bool flag02;
		[DataMember]
		protected bool flag03;
		[DataMember]
		protected bool flag04;
		[DataMember]
		protected bool flag05;
		[DataMember]
		protected bool flag06;

		[DataMember]
		protected bool fuelAdditiveFlag;

		[DataMember]
		protected string issuePoint;

		[DataMember]
		protected string issuePointNumber;

		[DataMember]
		protected string radioNumber;

		[DataMember]
		protected string gateID;

		[DataMember]
		protected Guid gateGuid;

		// 05-21-08 vthompson: Adding fields for ADF.  These are generic number fields
		[DataMember]
		protected double? number01;
		[DataMember]
		protected double? number02;
		[DataMember]
		protected double? number03;
		[DataMember]
		protected double? number04;
		[DataMember]
		protected double? number05;
		[DataMember]
		protected double? number06;

		// 05-22-2008 vthompson: Adding fields for ADF
		[DataMember]
		protected string contactFirstName;
		[DataMember]
		protected string contactSurname;

		// 07-09-2008 vt: Adding generic date fields for ADF
		[DataMember]
		protected DateTimeOffset? date01;
		[DataMember]
		protected DateTimeOffset? date02;
		[DataMember]
		protected DateTimeOffset? date03;
		[DataMember]
		protected DateTimeOffset? date04;

		[DataMember]
		protected string legacyNumber;
		[DataMember]
		protected string country;
		[DataMember]
		protected string contactInfo;
		[DataMember]
		protected string associatedDocumentNumber;
		[DataMember]
		protected string associatedCLIN;
		[DataMember]
		protected string associatedTransportOrderNumber;
		[DataMember]
		protected bool? submittedToAccounting;
		[DataMember]
		protected TransactionOrigin originApplication;
		[DataMember]
		protected string fuelCardID;
		[DataMember]
		protected DateTimeOffset? requestedDateTime;
		[DataMember]
		protected DateTimeOffset? dispatchedDateTime;

		[DataMember]
		protected EngineeringUnit volumeUnit;
		[DataMember]
		protected EngineeringUnit additiveVolumeUnit;
		[DataMember]
		protected EngineeringUnit levelUnit;
		[DataMember]
		protected EngineeringUnit densityUnit;
		[DataMember]
		protected EngineeringUnit temperatureUnit;
		[DataMember]
		protected EngineeringUnit massUnit;
		[DataMember]
		protected EngineeringUnit flowUnit;
		[DataMember]
		protected EngineeringUnit pressureUnit;

		[DataMember]
		protected byte volumeDecimalPlaces;
		[DataMember]
		protected byte additiveVolumeDecimalPlaces;
		[DataMember]
		protected byte levelDecimalPlaces;
		[DataMember]
		protected byte densityDecimalPlaces;
		[DataMember]
		protected byte temperatureDecimalPlaces;
		[DataMember]
		protected byte massDecimalPlaces;
		[DataMember]
		protected byte flowDecimalPlaces;
		[DataMember]
		protected byte pressureDecimalPlaces;
		[DataMember]
		protected bool errorFlag = false;
		// Fields that are come from tblExportResultDetails.
		[DataMember]
		protected string interfaceData01;
		[DataMember]
		protected string interfaceData02;
		[DataMember]
		protected string interfaceData03;
		[DataMember]
		protected string interfaceData04;
		[DataMember]
		protected string interfaceData05;
		[DataMember]
		protected string interfaceData06;
		[DataMember]
		protected string interfaceData07;
		[DataMember]
		protected string interfaceData08;
		[DataMember]
		protected string transErrorText;
		#endregion

		#region Constructor
		/// <summary>
		/// Initializes a new instance of the <see cref="BaseTransactionDO"/> class. 
		/// This is the default constructor for the Base Transaction Data Object class.
		/// </summary>
		public BaseTransactionDO()
		{
			this.init();
		}
		#endregion

		#region Properties
		[QueryWriterField("Interface Data 01", "tblExportResultDetails.InterfaceData01")]
		public string InterfaceData01
		{
			get { return this.interfaceData01; }
			set { this.interfaceData01 = value; }
		}

		[QueryWriterField("Interface Data 02", "tblExportResultDetails.InterfaceData02")]
		public string InterfaceData02
		{
			get { return this.interfaceData02; }
			set { this.interfaceData02 = value; }
		}

		[QueryWriterField("Interface Data 03", "tblExportResultDetails.InterfaceData03")]
		public string InterfaceData03
		{
			get { return this.interfaceData03; }
			set { this.interfaceData03 = value; }
		}

		[QueryWriterField("Interface Data 04", "tblExportResultDetails.InterfaceData04")]
		public string InterfaceData04
		{
			get { return this.interfaceData04; }
			set { this.interfaceData04 = value; }
		}

		[QueryWriterField("Interface Data 05", "tblExportResultDetails.InterfaceData05")]
		public string InterfaceData05
		{
			get { return this.interfaceData05; }
			set { this.interfaceData05 = value; }
		}

		[QueryWriterField("Interface Data 06", "tblExportResultDetails.InterfaceData06")]
		public string InterfaceData06
		{
			get { return this.interfaceData06; }
			set { this.interfaceData06 = value; }
		}

		[QueryWriterField("Interface Data 07", "tblExportResultDetails.InterfaceData07")]
		public string InterfaceData07
		{
			get { return this.interfaceData07; }
			set { this.interfaceData07 = value; }
		}

		[QueryWriterField("Interface Data 08", "tblExportResultDetails.InterfaceData08")]
		public string InterfaceData08
		{
			get { return this.interfaceData08; }
			set { this.interfaceData08 = value; }
		}

		[QueryWriterField("Error", "tblExportResultDetails.Error")]
		public string TransErrorText
		{
			get { return this.transErrorText; }
			set { this.transErrorText = value; }
		}

		[QueryWriterField("Transaction ID", "tblTransactions.TransID")]
		public string TransID
		{
			get { return transID; }
			set { transID = value; }
		}

		[QueryWriterField("Transaction Guid", "tblTransactions.TransactionGuid")]
		[XmlIgnore]
		public Guid TransactionGuid
		{
			get { return transactionGuid; }
			set { transactionGuid = value; }
		}

		[XmlIgnore]
		public Guid ConjoinedTransactionGuid
		{
			get { return conjoinedTransactionGuid; }
			set { conjoinedTransactionGuid = value; }
		}

        /// <summary>
        /// Represents the conjoined transaction's user data record.
        /// This is used when saving conjoined transactions to ensure that the 
        /// conjoined transaction's user data record is updated.
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public Guid ConjoinedUserDataGuid { get; set; }

        /// <summary>
        /// Represents the conjoined transaction's notes record.
        /// This is used when saving conjoined transactions to ensure that the 
        /// conjoined transaction's notes record is updated.
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public Guid ConjoinedNotesGuid { get; set; }

        /// <summary>
        /// Represents the conjoined transaction's signature record.
        /// This is used when saving conjoined transactions to ensure that the 
        /// conjoined transaction's signature record is updated.
        /// </summary>
        [XmlIgnore]
        [DataMember]
        public Guid ConjoinedSignatureGuid { get; set; }

		[DataMember]
		[XmlIgnore]
		public Byte[] RowVersion { get; set; }

		[QueryWriterField("Ticket Source", "tblTransactions.TicketSource")]
		public string TicketSource
		{
			get { return ticketSource; }
			set { ticketSource = value; }
		}

		[QueryWriterField("Ticket Mode", "tblTransactions.TicketMode")]
		public TicketModes TicketMode
		{
			get { return ticketMode; }
			set { ticketMode = value; }
		}

		[QueryWriterField("Document Number", "tblTransactions.DocumentNumber")]
		public string DocumentNumber
		{
			get { return documentNumber; }
			set { documentNumber = value; }
		}

		[QueryWriterField("Reversal Type", "tblTransactions.ReversalType")]
		public string ReversalType
		{
			get { return reversalType; }
			set { reversalType = value; }
		}

		[QueryWriterField("Reversed Trans ID", "tblTransactions.ReversedTransID")]
		public string ReversedTransID
		{
			get { return reversedTransID; }
			set { reversedTransID = value; }
		}

		[QueryWriterField("Site", "tblTransactions.Site")]
		public string Site
		{
			get { return site; }
			set { site = value; }
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid SiteGuid
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the reference ID.
		/// </summary>
		public string ReferenceID
		{
			get
			{
				return this.referenceID;
			}

			set
			{
				this.referenceID = value;
			}
		}

		[QueryWriterField("Transaction Alias", "tblTransactions.AliasName")]
		public string Alias
		{
			get { return aliasName; }
			set { aliasName = value; }
		}

		[DataMember]
		[XmlIgnore]
		public Guid TransactionAliasGuid { get; set; }

		[QueryWriterField("Sub Type", "tblTransactions.SubType")]
		public string SubType
		{
			get { return subType; }
			set { subType = value; }
		}

		[XmlElement("InventoryDateString")]
		public string InventoryDateString
		{
			get
			{
				return this.inventoryDate.ToString(TimeFormat);
			}
			set
			{
				this.inventoryDate = TimeConverter.ToDate((DateTimeOffset.ParseExact(value, TimeFormat, null))).Date;
			}
		}


		[XmlIgnore]
		public DateTime InventoryDate
		{
			get { return inventoryDate; }
			set { inventoryDate = value.Date; }
		}

		// This property only exists for the Query Writer. It is used to determine
		// how to display the date. In this case, we only want the date and not the
		// time.
		[QueryWriterField("Inventory Date", "tblTransactions.InventoryDate")]
		public Date InventoryDateAsDateOnly
		{
			get { return new Date(); }
		}


		[XmlElement("TransactionDateTimeString")]
		public string TransactionDateTimeString
		{
			get
			{
				return this.transactionDateTime == null ? string.Empty : ((DateTimeOffset)this.transactionDateTime).ToString(TimeFormat);
			}

			set
			{
				this.transactionDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Transaction Date", "tblTransactions.TransDateTime")]
		[XmlIgnore]
		public DateTimeOffset? TransactionDateTime
		{
			get { return transactionDateTime; }
			set { transactionDateTime = value; }
		}

		[QueryWriterField("Owner ID", "tblTransactions.OwnerID")]
		public string OwnerID
		{
			get { return ownerID; }
			set { ownerID = value; }
		}

		[QueryWriterField("Owner Code", "tblTransactions.OwnerCode")]
		public string OwnerCode
		{
			get { return ownerCode; }
			set { ownerCode = value; }
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid OwnerCompanyGuid
		{
			get;
			set;
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid ShipToCompanyGuid
		{
			get;
			set;
		}

		[QueryWriterField("Manager ID", "tblTransactions.ManagerID")]
		public string ManagerID
		{
			get { return managerID; }
			set { managerID = value; }
		}

		[QueryWriterField("Manager Code", "tblTransactions.ManagerCode")]
		public string ManagerCode
		{
			get { return managerCode; }
			set { managerCode = value; }
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid ShipperCompanyGuid { get; set; }

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid BillToCompanyGuid { get; set; }

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid ManagerCompanyGuid { get; set; }

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid SupplierCompanyGuid { get; set; }

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid CarrierCompanyGuid { get; set; }

		[XmlElement("CloseoutDateString")]
		public string CloseoutDateString
		{
			get
			{
				return this.closeoutDate == null ? string.Empty : ((DateTimeOffset)this.closeoutDate).ToString(TimeFormat);
			}

			set
			{
				this.closeoutDate = (value == string.Empty) ? (DateTime?) null : DateTime.ParseExact(value, TimeFormat, null).Date;
			}
		}
	
		[XmlIgnore]
		public DateTime? CloseoutDate
		{
			get { return closeoutDate; }
			set { closeoutDate = value; }
		}

		public bool PartialCloseout
		{
			get { return partialCloseout; }
			set { partialCloseout = value; }
		}

		[DataMember]
		[XmlIgnore]
		public Guid TransactionNoteGuid { get; set; }

		[QueryWriterField("Notes", false)]
		public string Notes
		{
			get { return notes; }
			set { notes = value; }
		}

		[QueryWriterField("AdditionalInformation", false)]
		public string AdditionalInformation
		{
			get { return additionalInformation; }
			set { additionalInformation = value; }
		}

		[DataMember]
		[XmlIgnore]
		public Guid TransactionUserDataGuid { get; set; }

		[XmlIgnoreAttribute]
		public Dictionary<string, string> UserData
		{
			get { return this.userDataTable; }
			private set { this.userDataTable = value; }
		}

		[XmlArray("TransactionLineItems")] //rename the node to "TransctionLineItems"
		[XmlArrayItem(Type = typeof(LineItemDO))]
		public List<LineItemDO> LineItems
		{
			get { return lineItems; }
			set { lineItems = value; }
		}

		[QueryWriterField("Trans Type ID", "tblTransactions.LookupTransTypeIndex")]
		public TransactionTypes TransTypeID
		{
			get { return transTypeID; }
			set { transTypeID = value; }
		}

		[QueryWriterField("Transaction Status", "tblTransactions.LookupTransactionStatusIndex")]
		public TransactionStatus Status
		{
			get { return status; }
			set { status = value; }
		}

		[DataMember]
		[XmlIgnore]
		public Guid TransactionSignatureGuid { get; set; }
		
		public byte[] Signature
		{
			get { return signature; }
			set { signature = value; }
		}

		public long TransVersion
		{
			get { return transversion; }
			set { transversion = value; }
		}

		[QueryWriterField("Created By", "tblTransactions.CreatedBy")]
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		[XmlElement("CreatedDateString")]
		public string CreatedDateString
		{
			get
			{
				return this.createdDate.ToString(TimeFormat);
			}

			set
			{
				this.createdDate = DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


		[QueryWriterField("Created Date", "tblTransactions.CreatedDate")]
		[XmlIgnore]
		public DateTimeOffset CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		[QueryWriterField("Updated By", "tblTransactions.UpdatedBy")]
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		[XmlElement("UpdatedDateString")]
		public string UpdatedDateString
		{
			get
			{
				return this.updatedDate.ToString(TimeFormat);
			}

			set
			{
				this.updatedDate = DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Updated Date", "tblTransactions.UpdatedDate")]
		[XmlIgnore]
		public DateTimeOffset UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		/// <summary>
		/// This property will return true if the transaction had already
		/// been saved. It will return false if the transaction is new. The
		/// check is based on the CreatedBy field being populated.
		/// </summary>
		public bool IsSavedTransaction
		{
			get
			{
				if (string.IsNullOrEmpty(this.createdBy))
				{
					return false;
				}
				return true;
			}
		}

		[QueryWriterField("Submitted To Accounting", "tblTransactions.SubmittedToAccounting")]
		public bool? SubmittedToAccounting
		{
			get { return submittedToAccounting; }
			set { submittedToAccounting = value; }
		}

		[QueryWriterField("Origin Application", "tblTransactions.LookupOriginApplicationIndex")]
		public TransactionOrigin OriginApplication
		{
			get { return originApplication; }
			set { originApplication = value; }
		}

		[QueryWriterField("Fuel Card ID", "tblTransactions.FuelCardID")]
		public string FuelCardID
		{
			get { return fuelCardID; }
			set { fuelCardID = value; }
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public Guid FuelCardGuid { get; set; }

		[XmlElement("RequestedDateTimeString")]
		public string RequestedDateTimeString
		{
			get
			{
				return this.requestedDateTime == null ? string.Empty : ((DateTimeOffset) this.requestedDateTime).ToString(TimeFormat);
			}

			set
			{
				this.requestedDateTime = (value == string.Empty) ? (DateTimeOffset?) null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}

		[QueryWriterField("Requested Date Time", "tblTransactions.RequestedDateTime")]
		[XmlIgnore]
		public DateTimeOffset? RequestedDateTime
		{
			get { return this.requestedDateTime; }
			set { this.requestedDateTime = value; }
		}

		[XmlElement("DispatchedDateTimeString")]
		public string DispatchedDateTimeString
		{
			get
			{
				return this.dispatchedDateTime == null ? string.Empty : ((DateTimeOffset)this.dispatchedDateTime).ToString(TimeFormat);
			}

			set
			{
				this.dispatchedDateTime = (value == string.Empty) ? (DateTimeOffset?)null : DateTimeOffset.ParseExact(value, TimeFormat, null);
			}
		}


	
		[QueryWriterField("Dispatched Date Time", "tblTransactions.DispatchedDateTime")]
		[XmlIgnore]
		public DateTimeOffset? DispatchedDateTime
		{
			get { return dispatchedDateTime; }
			set { dispatchedDateTime = value; }
		}

		public EngineeringUnit VolumeUnits
		{
			get { return volumeUnit; }
			set { volumeUnit = value; }
		}

		public EngineeringUnit AdditiveVolumeUnits
		{
			get { return additiveVolumeUnit; }
			set { additiveVolumeUnit = value; }
		}

		public EngineeringUnit LevelUnits
		{
			get { return levelUnit; }
			set { levelUnit = value; }
		}

		public EngineeringUnit TemperatureUnits
		{
			get { return temperatureUnit; }
			set { temperatureUnit = value; }
		}

		public EngineeringUnit DensityUnits
		{
			get { return densityUnit; }
			set { densityUnit = value; }
		}

		public EngineeringUnit MassUnits
		{
			get { return massUnit; }
			set { massUnit = value; }
		}

		public EngineeringUnit FlowUnits
		{
			get { return flowUnit; }
			set { flowUnit = value; }
		}

		public EngineeringUnit PressureUnits
		{
			get { return pressureUnit; }
			set { pressureUnit = value; }
		}

		public byte VolumeDecimalPlaces
		{
			get { return volumeDecimalPlaces; }
			set { volumeDecimalPlaces = value; }
		}

		public byte AdditiveVolumeDecimalPlaces
		{
			get { return additiveVolumeDecimalPlaces; }
			set { additiveVolumeDecimalPlaces = value; }
		}

		public byte LevelDecimalPlaces
		{
			get { return levelDecimalPlaces; }
			set { levelDecimalPlaces = value; }
		}

		public byte TemperatureDecimalPlaces
		{
			get { return temperatureDecimalPlaces; }
			set { temperatureDecimalPlaces = value; }
		}

		public byte DensityDecimalPlaces
		{
			get { return densityDecimalPlaces; }
			set { densityDecimalPlaces = value; }
		}

		public byte MassDecimalPlaces
		{
			get { return massDecimalPlaces; }
			set { massDecimalPlaces = value; }
		}

		public byte FlowDecimalPlaces
		{
			get { return flowDecimalPlaces; }
			set { flowDecimalPlaces = value; }
		}

		public byte PressureDecimalPlaces
		{
			get { return pressureDecimalPlaces; }
			set { pressureDecimalPlaces = value; }
		}
		#endregion Properties

		#region Override Methods
		override public string getSelectCommand()
		{
			return null;
		}

		override public string getInsertCommand()
		{
			return null;
		}

		override public string getDeleteCommand()
		{
			return null;
		}

		override public string getUpdateCommand()
		{
			return null;
		}
		#endregion

		#region Methods
		/// <summary>
		/// This method initials the base transaction data object to its initial state.
		/// </summary>
		public void init()
		{
			this.TransactionGuid = Guid.Empty;
			this.conjoinedTransactionGuid = Guid.Empty;
			this.conjoinedTransID = string.Empty;

			this.managerID = null;
			this.managerCode = null;
			this.ManagerCompanyGuid = Guid.Empty;

			this.ownerID = null;
			this.ownerCode = null;
			this.OwnerCompanyGuid = Guid.Empty;

			this.shipperID = null;
			this.shipperCode = null;
			this.ShipperCompanyGuid = Guid.Empty;

			this.billToID = null;
			this.billToCode = null;
			this.BillToCompanyGuid = Guid.Empty;

			this.shipToID = null;
			this.shipToCode = null;
			this.ShipToCompanyGuid = Guid.Empty;

			this.supplierID = null;
			this.supplierCode = null;
			this.SupplierCompanyGuid = Guid.Empty;

			this.carrierID = null;
			this.carrierCode = null;
			this.CarrierCompanyGuid = Guid.Empty;

			this.TransactionNoteGuid = Guid.Empty;
			this.TransactionSignatureGuid = Guid.Empty;
			this.TransactionUserDataGuid = Guid.Empty;

			this.lineItems			= new List<LineItemDO>();
			this.userDataTable		= new Dictionary<string, string>();
			this.paymentInfo		= new PaymentInfoDO();
			this.routeInfo			= new RouteInfoDO();
			this.routeSchedule		= new RouteScheduleDO();
			this.weightReadings		= new List<WeightReadingDO>();
			this.transportInfoList	= new List<TransportLineItemDO>();

			this.destinationEQ1 = new EquipmentDO();
			this.destinationEQ2 = new EquipmentDO();
			this.destinationEQ3 = new EquipmentDO();

			this.sourceEQ1 = new EquipmentDO();
			this.sourceEQ2 = new EquipmentDO();
			this.sourceEQ3 = new EquipmentDO();

			this.FuelCardID = null;
			this.FuelCardGuid = Guid.Empty;

		    this.documentNumber = string.Empty;

			this.interfaceData01 = null;
			this.interfaceData02 = null;
			this.interfaceData03 = null;
			this.interfaceData04 = null;
			this.interfaceData05 = null;
			this.interfaceData06 = null;
			this.interfaceData07 = null;
			this.interfaceData08 = null;
			this.transErrorText  = null;

			this.volumeUnit			= EngineeringUnit.FmvMeter3;
			this.additiveVolumeUnit = EngineeringUnit.FmvMeter3;
			this.levelUnit			= EngineeringUnit.FmlMeter;
			this.densityUnit		= EngineeringUnit.FmdKgM3;
			this.temperatureUnit	= EngineeringUnit.FmtDegC;
			this.massUnit			= EngineeringUnit.FmmKg;
			this.flowUnit			= EngineeringUnit.FmvfM3Sec;
			this.pressureUnit		= EngineeringUnit.FmpPa;

			this.volumeDecimalPlaces			= 2;
			this.additiveVolumeDecimalPlaces	= 2;
			this.levelDecimalPlaces				= 2;
			this.densityDecimalPlaces			= 2;
			this.temperatureDecimalPlaces		= 2;
			this.massDecimalPlaces				= 2;
			this.flowDecimalPlaces				= 2;
			this.pressureDecimalPlaces			= 2;
			this.errorFlag						= false;
			this.operatorName					= null;

			// The SubmittedToAccounting flag will initially be false for transactions created
			// by dispatch and true for transactions created by accounting. Since the majority
			// of transactions are created by accounting the flag is defaulted to true.
			this.submittedToAccounting = true;
		}
		#endregion
	}
}
