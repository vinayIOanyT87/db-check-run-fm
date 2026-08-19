namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    #region Public enumerations
	/// <summary>
	/// Summary description for DisplayOrderType.
	/// </summary>
	public enum TRANSACTION_SECTION_TYPE
	{
		BODY = 1,
		LINE_ITEMS = 2,
		WEIGHT_READINGS = 3,
		TRANPORT_INFO = 4,
		EXPORT_RESULTS = 5
	}

	/// <summary>
	/// Summary description for TransactionTypes.
	/// </summary>
	public enum TransactionTypes : short
	{
		TransactionType_None = 0,
		T1_PrimaryAdjustment = 1,
		T2_SecondaryAdjustment,
		T3_PrimaryDefuel,
		T4_SecondaryDefuel,
		T5_PrimaryDisbursement,
		T6_SecondaryDisbursement,
		T7_FillStand,
		T8_Receipt,
		T9_Request,
		T10_Unload,
		T11_ConsumerTransfer,
		T12_InventoryNotAffected,
		T13_OwnerTransfer,
		T14_PhysicalInventory,
		T15_PrimaryRegrade,
		T16_SecondaryRegrade,
		T17_Order,
		T18_SupplyOrder,
		T19_EndOfDay,
		T20_EndOfMonth,
		T21_AccountPayableInvoice,
		T22_AccountReceivableInvoice,
		T23_StorageTransfer,
		T24_Aggregate,
		T25_Shipment,
		T_Maximum
	}

	/// <summary>
	/// Indicates whether the company id, name, or both should be displayed
	/// </summary>
	/// <remarks>
	/// 04-14-2008	V. Thompson		CSI 5560
	///								The intent is for the FMCompanyTextBox to be able
	///								to determine whether to display the company name,
	///								ID, or both on TransactionDetails.aspx
	/// </remarks>
	public enum TRANSACTION_SHOW_COMPANY_NAME : short
	{
		SHOW_ID_ONLY = 0,
		SHOW_NAME_ONLY = 1,
		SHOW_NAME_AND_ID = 2
	}
	#endregion

	#region Transaction Alias Collection Class
   [Serializable]
   [CollectionDataContract]
	public class TransactionAliasCollectionClass : List<TransactionAliasClass>
	{
	}
	#endregion

	#region Transaction Alias Class
	/// <summary>
	/// Summary description for TransactionAliasClass.
	/// </summary>
	[Serializable()]
	[DebuggerDisplay("TransactionAliasClass ID={ID},IdentityGuid={IdentityGuid}")]
	[DataContract]
	[KnownType(typeof(TransactionAliasFieldClass))]
	[KnownType(typeof(UserDataFieldClass))]
	public class TransactionAliasClass : BaseDataObject
	{
		#region Public data members
		public override string ID
		{
			get { return this._ID; }
			set
			{
				string temp = value;

				if (string.IsNullOrEmpty(temp) == false)
				{
					temp = temp.Trim();
				}

			    this.SetString("ID", 32, temp, ref this._ID);
			}
		}

		[DataMember]
		public const string ENTITY_TYPE_ID = "Transaction Aliases";

		[DataMember]
		public TransactionTypes _TransTypeID;

		[DataMember]
		public string AssociatedReport = "";

		[DataMember]
		public string AssociatedPreloadReport = "";

		[DataMember]
		public ProductMapCollectionClass ExcludedProductCollection;

		/// <summary>
		/// Gets or sets the user data field collection.
		/// </summary>
		[DataMember]
		public UserDataFieldCollectionClass UserDataFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the line item user data field collection.
		/// </summary>
		[DataMember]
		public UserDataFieldCollectionClass LineItemUserDataFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the transaction field collection.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass TransactionFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the line item field collection.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass LineItemFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the weight reading field collection.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass WeightReadingFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the transport line item field collection.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass TransportLineItemFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the note field collection.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass NoteFieldCollection
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the export result detail field collection.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass ExportResultDetailFieldCollection
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the dispatch user data fields.
		/// </summary>
		[DataMember]
		public UserDataFieldCollectionClass DispatchUserDataFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch line item user data fields.
		/// </summary>
		[DataMember]
		public UserDataFieldCollectionClass DispatchLineItemUserDataFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch transaction fields.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass DispatchTransactionFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch line item fields.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass DispatchLineItemFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch weight reading fields.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass DispatchWeightReadingFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatchtransport line item fields.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass DispatchTransportLineItemFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch note fields.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass DispatchNoteFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the dispatch export result detail fields.
		/// </summary>
		[DataMember]
		public TransactionAliasFieldCollectionClass DispatchExportResultDetailFields
		{
			get;
			set;
		}

		[DataMember]
		public bool EnableAutoCompleteControls { get; set; }

		[DataMember]
		public bool PermitNonReferenceData { get; set; }

		[DataMember]
		public bool UseTransactionDetailWithLayout { get; set; }

		[DataMember]
		public bool DefaultMeterToEquipmentID { get; set; }

		[DataMember]
		public bool LimitSourceEquipmentByProduct { get; set; }

		[DataMember]
		public bool RememberMeterEndForMeterID { get; set; }

		[DataMember]
		public bool PopulateCompaniesFromEquipment { get; set; }

		[DataMember]
		public bool PopulateGrossVolumeFromMeterValues { get; set; }

		[DataMember]
		public bool UseMeterAndCompressionFactorFromMeter { get; set; }


		[DataMember]
		public int LookupDefaultStatusIndex;

		[DataMember]
		public GroupTransactionAliasMapCollectionClass GroupTransactionAliasMapCollection;

		[DataMember]
		public string AssociatedAlias = "";

		[DataMember]
		public byte _LevelDecimalPlaces;

		[DataMember]
		public byte _TemperatureDecimalPlaces;

		[DataMember]
		public byte _DensityDecimalPlaces;

		[DataMember]
		public byte _PressureDecimalPlaces;

		[DataMember]
		public byte _FlowDecimalPlaces;

		[DataMember]
		public byte _VolumeDecimalPlaces;

		[DataMember]
		public byte _MassDecimalPlaces;

		[DataMember]
		public byte _AdditiveVolumeDecimalPlaces;

		[DataMember]
		protected bool _IncludeInDispatch;
		#endregion

		#region Protected data members

		[DataMember]
		protected bool _MeterCloseout;

		[DataMember]
		protected bool _BulkShipment;

		[DataMember]
		protected bool _DistributedImpact;

		[DataMember]
		protected bool _MultipleLineItems;

		[DataMember]
		protected bool _LineItemEditControl;

		[DataMember]
		protected bool _MultipleWeightReadings;

		[DataMember]
		protected bool _LimitSelectionsBasedOnHierarchy;

		[DataMember]
		protected bool _WeightReadingEditControl;

		[DataMember]
		protected bool multipleTransportLineItems;

		[DataMember]
		protected Guid _AssociatedTransactionAliasGuid = Guid.Empty;

		[DataMember]
		protected ulong[] _DestinationEquipmentTypes = { 0, 0, 0 };

		[DataMember]
		protected ulong[] _SourceEquipmentTypes = { 0, 0, 0 };

		[DataMember]
		protected bool _UseComboBoxControls;

		[DataMember]
		protected TRANSACTION_SHOW_COMPANY_NAME _showCompanyName = TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY;

		// The following are flag for enabling drawdown warnings

		[DataMember]
		protected bool enableTotalQtyExceededWarning;

		[DataMember]
		protected bool enableTotalValueExceededWarning;

		[DataMember]
		protected bool enableQtyToleranceExceededWarning;

		[DataMember]
		protected bool enableValueToleranceExceededWarning;

		// Holds a collection of associated transaction aliases

		[DataMember]
		protected TransactionAliasCollectionClass associatedAliases = new TransactionAliasCollectionClass();

		// vt - ADF: This will contain the int value of FM7Accounting.TransactionStatus objects that have
		// been assigned to the transaction alias.  Using the actual enumeration would have been better,
		// however it would create a circular reference between FM7Accounting and ConsolidatedDataObjects

		[DataMember]
		protected ArrayList assignedStatuses = new ArrayList();

		// Determines if a transaction's gross quantity should be the sum of it's associated transactions

		[DataMember]
		protected bool aggregateAssociatedTransactions;

		// Units

		[DataMember]
		protected EngineeringUnit _LevelUnits;

		[DataMember]
		protected EngineeringUnit _TemperatureUnits;

		[DataMember]
		protected EngineeringUnit _DensityUnits;

		[DataMember]
		protected EngineeringUnit _PressureUnits;

		[DataMember]
		protected EngineeringUnit _FlowUnits;

		[DataMember]
		protected EngineeringUnit _VolumeUnits;

		[DataMember]
		protected EngineeringUnit _MassUnits;

		[DataMember]
		protected EngineeringUnit _AdditiveVolumeUnits;

        //Record Versioning Items
        [DataMember]
        protected Guid _MasterRecordGuid;
        [DataMember]
        protected Guid _AssignedToSiteGuid;
        [DataMember]
        protected Guid _AssignedFromSiteGuid;
        [DataMember]
        protected string _AssignedFromSiteId;
		#endregion

		#region Constants
		const TransactionFieldType Transaction = TransactionFieldType.Transaction;
		const TransactionFieldType Note = TransactionFieldType.Note;
		const TransactionFieldType LineItem = TransactionFieldType.LineItem;
		const TransactionFieldType WeightReading = TransactionFieldType.WeightReading;
		const TransactionFieldType TransportInfo = TransactionFieldType.TransportInfo;
		const TransactionFieldType ExportResultDetail = TransactionFieldType.ExportResult;
		#endregion

		#region Select transaction alias clause
		string SelectClause = "SELECT tblTransactionAliases.*," +
			"(SELECT A.AliasName FROM tblTransactionAliases A WHERE A.TransactionAliasGuid = tblTransactionAliases.AssociatedTransactionAliasGuid) AS AssociatedAlias ";
		#endregion

		#region Constructors
		/// <summary>
		/// This is the default constructor for the Transaction Alias Class.
		/// </summary>
		public TransactionAliasClass()
		{
			this.Reset();
		}
		#endregion

		#region Protected methods
		protected void InitializeFields(FieldClass[] Fields)
		{
			foreach (FieldClass Field in Fields)
			{
				if (typeof(TransactionAliasFieldClass).IsInstanceOfType(Field))
				{
					TransactionAliasFieldClass TransactionAliasField = Field as TransactionAliasFieldClass;
					switch (TransactionAliasField.Type)
					{
						case Transaction:
							this.TransactionFieldCollection.Add(TransactionAliasField);
							break;
						case Note:
							this.NoteFieldCollection.Add(TransactionAliasField);
							break;
						case LineItem:
							this.LineItemFieldCollection.Add(TransactionAliasField);
							break;
						case WeightReading:
							this.WeightReadingFieldCollection.Add(TransactionAliasField);
							break;
						case TransportInfo:
							this.TransportLineItemFieldCollection.Add(TransactionAliasField);
							break;
						case ExportResultDetail:
							this.ExportResultDetailFieldCollection.Add(TransactionAliasField);
							break;
						default:
							break;
					}
				}

				else if (typeof(UserDataFieldClass).IsInstanceOfType(Field))
				{
					// Cast the generic field to a UserDataField
					UserDataFieldClass userDataField = (UserDataFieldClass)Field;
					if (userDataField.UserDataEntityType == ENTITY_TYPE.TRANSACTION_ALIAS_LINE_ITEM)
					{
						// Add the field to the line item user data collection
					    this.LineItemUserDataFieldCollection.Add(userDataField);
					}
					else
					{
						// Add the field to the user data field collection
					    this.UserDataFieldCollection.Add(userDataField);
					}
				}
			}
		}
		#endregion

		#region Properties
		public TransactionTypes TransTypeIDForSerializationOnly
		{
			get
			{
				return this._TransTypeID;
			}
			set
			{
			    this._TransTypeID = value;
			}
		}

		[XmlIgnore]
		public TransactionTypes TransTypeID
		{
			get
			{
				return this._TransTypeID;
			}

			set
			{
				this.UserDataFieldCollection = new UserDataFieldCollectionClass();
				this.LineItemUserDataFieldCollection = new UserDataFieldCollectionClass();
				this.TransactionFieldCollection = new TransactionAliasFieldCollectionClass();
				this.LineItemFieldCollection = new TransactionAliasFieldCollectionClass();
				this.WeightReadingFieldCollection = new TransactionAliasFieldCollectionClass();
				this.TransportLineItemFieldCollection = new TransactionAliasFieldCollectionClass();
				this.ExportResultDetailFieldCollection = new TransactionAliasFieldCollectionClass();
				this.NoteFieldCollection = new TransactionAliasFieldCollectionClass();
				this.DispatchUserDataFields = new UserDataFieldCollectionClass();
				this.DispatchLineItemUserDataFields = new UserDataFieldCollectionClass();
				this.DispatchTransactionFields = new TransactionAliasFieldCollectionClass();
				this.DispatchLineItemFields = new TransactionAliasFieldCollectionClass();
				this.DispatchWeightReadingFields = new TransactionAliasFieldCollectionClass();
				this.DispatchTransportLineItemFields = new TransactionAliasFieldCollectionClass();
				this.DispatchNoteFields = new TransactionAliasFieldCollectionClass();
				this.DispatchExportResultDetailFields = new TransactionAliasFieldCollectionClass();

			    this._TransTypeID = value;
				int Order = 0;

				this.MultipleLineItems = true;
				this.LookupDefaultStatusIndex = -1;
				this.LimitSelectionsBasedOnHierarchy = false;
				this.ShowCompanyName = TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY;
				this.MeterCloseout = false;
				this.DistributedImpact = false;
				this.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { EQUIPMENT_TYPE.TANKER_TYPE, EQUIPMENT_TYPE.TRACTOR_TYPE });
				this.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { EQUIPMENT_TYPE.TRAILER_TYPE });

				switch (this._TransTypeID)
				{
					case TransactionTypes.T1_PrimaryAdjustment:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
							};

							UserDataFieldClass userDataFieldType1 = new UserDataFieldClass();
							userDataFieldType1.DisplayOrder = Order++;
							userDataFieldType1.UserDataEntityType = ENTITY_TYPE.TRANSACTION_ALIAS;
							userDataFieldType1.DisplayName = "Type";
							userDataFieldType1.Number = 0;
							userDataFieldType1.UserDataType = USER_DATA_TYPE.LIST;
							userDataFieldType1.UserDataListValueCollection.Add(new UserDataListValueClass{ID = "Company Use" });
							userDataFieldType1.UserDataListValueCollection.Add(new UserDataListValueClass{ID = "Other" });
							userDataFieldType1.UserDataListValueCollection.Add(new UserDataListValueClass{ID = "Over-Short" });
							userDataFieldType1.UserDataListValueCollection.Add(new UserDataListValueClass{ID = "Unsaleable Stock" });

							Fields.Add(userDataFieldType1);

							//UserDataFieldClass userDataFieldType2FFFF = new UserDataFieldClass();
							//userDataFieldType2FFFF.DisplayOrder = Order++;
							//userDataFieldType2FFFF.UserDataEntityType = ENTITY_TYPE.TRANSACTION_ALIAS;
							//userDataFieldType2FFFF.DisplayName = "Type 2FFF";
							//userDataFieldType2FFFF.Number = 1;
							//userDataFieldType2FFFF.UserDataType = USER_DATA_TYPE.TEXT;

							//Fields.Add(userDataFieldType2FFFF);

							Fields.Add(new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"));
                            Fields.Add(new TransactionAliasFieldClass(LineItem, Order++, "Product", "Product"));
                            Fields.Add(new TransactionAliasFieldClass(LineItem, Order++, "NetQuantity", "Net", true));

                            this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T2_SecondaryAdjustment:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),

							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T3_PrimaryDefuel:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ShipToID","Ship To",true),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),

							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T4_SecondaryDefuel:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ShipToID","ShipTo",true),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),

							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T5_PrimaryDisbursement: //BOL
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"DocumentNumber","BOL #"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Stockholder"),
								new TransactionAliasFieldClass(Transaction,Order++,"ShipperID","Shipper"),
								new TransactionAliasFieldClass(Transaction,Order++,"BillToID","Bill To",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ShipToID","Ship To",true),
								new TransactionAliasFieldClass(Transaction,Order++,"CarrierID","Carrier",true),
								new TransactionAliasFieldClass(Transaction,Order++,"OperatorID","Operator"),
								new TransactionAliasFieldClass(Transaction,Order++,"DestinationRegistrationID1","Trailer"),
								new TransactionAliasFieldClass(Transaction,Order++,"PONumber","PO Number"),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Status"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"TimeIn","Time In"),
								new TransactionAliasFieldClass(Transaction,Order++,"TimeOut","Time Out"),
								new TransactionAliasFieldClass(Transaction,Order++,"FST","Load Start"),
								new TransactionAliasFieldClass(Transaction,Order++,"TimeEnd","Load End"),
								new TransactionAliasFieldClass(Transaction,Order++,"LoadID","Load ID"),
								new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"),

								new TransactionAliasFieldClass(LineItem,Order++,"LoadingLocationID","Load Rack"),
								new TransactionAliasFieldClass(LineItem,Order++,"ArmNumber","Arm"),
								new TransactionAliasFieldClass(LineItem,Order++,"BatchNumber","Batch"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterID","Meter"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"AdditiveProfileID","Additive"),
								new TransactionAliasFieldClass(LineItem,Order++,"PresetAmount","Preset"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"DeliveredGrossQuantity","Delivered Gross"),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"DeliveredNetQuantity","Delivered Net"),
								new TransactionAliasFieldClass(LineItem,Order++,"Pressure","Pressure"),
								new TransactionAliasFieldClass(LineItem,Order++,"Density","Density"),
								new TransactionAliasFieldClass(LineItem,Order++,"Temperature","Temp"),
								new TransactionAliasFieldClass(LineItem,Order++,"StorageLocationID","Tank",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"BrokenBlend","Broken Blend"),
								new TransactionAliasFieldClass(LineItem,Order++,"ImproperAdditization","Improper Additization"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterStart","Meter Start"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterStop","Meter Stop"),														
							};

                            this.SetEquipmentTypes(true, 1, new EQUIPMENT_TYPE[] { EQUIPMENT_TYPE.TANKER_TYPE, EQUIPMENT_TYPE.TRACTOR_TYPE, EQUIPMENT_TYPE.TRAILER_TYPE });
                            this.SetEquipmentTypes(true, 2, new EQUIPMENT_TYPE[] { EQUIPMENT_TYPE.TRAILER_TYPE });

                            this.InitializeFields(Fields.ToArray());

							break;
						}

					case TransactionTypes.T6_SecondaryDisbursement:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"DocumentNumber","Document Number"),
								new TransactionAliasFieldClass(Transaction, Order++, "CarrierID", "Carrier ID", true),
							};

							UserDataFieldClass userDataFieldType = new UserDataFieldClass();
							userDataFieldType.DisplayOrder = Order++;
							userDataFieldType.UserDataEntityType = ENTITY_TYPE.TRANSACTION_ALIAS;
							userDataFieldType.DisplayName = "Type";
							userDataFieldType.Number = 0;
							userDataFieldType.UserDataType = USER_DATA_TYPE.LIST;
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Barge" });
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Railcar" });
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Truck" });

							Fields.Add(userDataFieldType);

							Fields.Add(new TransactionAliasFieldClass(Transaction, Order++, "PONumber", "PO Number"));
							Fields.Add(new TransactionAliasFieldClass(Transaction, Order++, "ReversalType", "Reversal", true));
							Fields.Add(new TransactionAliasFieldClass(Transaction, Order++, "ShipToID", "Ship To", true));
							Fields.Add(new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"));
							Fields.Add(new TransactionAliasFieldClass(LineItem, Order++, "Product", "Product"));
							Fields.Add(new TransactionAliasFieldClass(LineItem, Order++, "NetQuantity", "Net", true));

							this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T7_FillStand:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
									 
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T8_Receipt:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"DocumentNumber","Ticket #"),
								new TransactionAliasFieldClass(Transaction,Order++,"CarrierID","Carrier ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"PONumber","PO Number"),
							};

							UserDataFieldClass userDataFieldType = new UserDataFieldClass();
							userDataFieldType.DisplayOrder = Order++;
							userDataFieldType.UserDataEntityType = ENTITY_TYPE.TRANSACTION_ALIAS;
							userDataFieldType.DisplayName = "Type";
							userDataFieldType.Number = 0;
							userDataFieldType.UserDataType = USER_DATA_TYPE.LIST;
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Barge" });
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Pipeline" });
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Rail" });
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Truck" });
							userDataFieldType.UserDataListValueCollection.Add(new UserDataListValueClass { ID = "Vessel" });

							Fields.Add(userDataFieldType);

							Fields.Add(new TransactionAliasFieldClass(Transaction, Order++, "ReversalType", "Reversal", true));
							Fields.Add(new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"));

							Fields.Add(new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"));
							Fields.Add(new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true));
							Fields.Add(new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true) { FieldRequired = true });
							Fields.Add(new TransactionAliasFieldClass(LineItem,Order++, "MeterTotal", "MeterTotal"));

							this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T9_Request:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"SupplierID","Supplier"),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
									 
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T10_Unload:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
									 
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T11_ConsumerTransfer:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ConjoinTransID","Conjoined ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"FromShipToID","From ShipTo"),
								new TransactionAliasFieldClass(Transaction,Order++,"ToShipToID","To ShipTo"),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
									 
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T12_InventoryNotAffected: // Meter Closeout
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"DocumentNumber","Document Number"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Note,Order++,"Notes","Notes"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross Quantity"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterID","Meter",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterStop","Stop",true),
								new TransactionAliasFieldClass(LineItem,Order++,"StorageLocationID","Tank"),

							};

							//this.MultipleLineItems = true;
							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T13_OwnerTransfer:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++, "PONumber", "SAP Batch" ),
								new TransactionAliasFieldClass(Transaction,Order++,"FromManagerID","From Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"ToManagerID","To Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"FromOwnerID","From Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ToOwnerID","To Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),

								new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"),

								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
							};

							this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T14_PhysicalInventory:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"DocumentNumber","Document Number"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal Type"),
								new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"StorageLocationID","Tank",true),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
							};

							this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T15_PrimaryRegrade:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Note, Order++, "Notes", "Notes"),
								new TransactionAliasFieldClass(LineItem,Order++,"FromProduct","From Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"ToProduct","To Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
							};

							this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T16_SecondaryRegrade:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ConjoinTransID","Conjoined ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"LineNumber","Line Number"),
								new TransactionAliasFieldClass(LineItem,Order++,"FromProduct","From Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"ToProduct","To Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Item Status"),
									 
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T17_Order:
						{
							FieldClass[] Fields =
							{
									new TransactionAliasFieldClass( Transaction, Order++, "PONumber", "PO Number" ),
									new TransactionAliasFieldClass( Transaction, Order++, "BillToID", "Bill To",true),
									new TransactionAliasFieldClass( Transaction, Order++, "OwnerID", "Owner" ),
									new TransactionAliasFieldClass( Transaction, Order++, "Site", "Site" ),
									new TransactionAliasFieldClass( Transaction, Order++, "LookupTransactionStatusIndex", "Transaction Status",true ),
									new TransactionAliasFieldClass( Transaction, Order++, "DocumentNumber", "Document Number",true),
									new TransactionAliasFieldClass( Transaction, Order++, "ManagerID", "Manager" ),
									new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
									new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
									new TransactionAliasFieldClass( Transaction, Order++, "AliasName", "Alias" ),
									new TransactionAliasFieldClass( Transaction, Order++, "ShipToID", "Ship To",true ),
									new TransactionAliasFieldClass( Transaction, Order++, "CarrierID", "Carrier",true ),
									new TransactionAliasFieldClass( Transaction, Order++, "EffectiveDate", "Effective Date"),
									new TransactionAliasFieldClass( Transaction, Order++, "ExpirationDate", "Expiration Date"),
									new TransactionAliasFieldClass( Transaction, Order++, "ShipperID", "Shipper"),
									new TransactionAliasFieldClass( Transaction, Order++, "ScheduledDate", "Scheduled Date",true),
									new TransactionAliasFieldClass( Transaction, Order++, "RequestedDeliveryDate", "Requested Date"),
									new TransactionAliasFieldClass( Transaction, Order++, "AutoComplete", "Auto Complete",true),
									new TransactionAliasFieldClass( LineItem, Order++, "LineNumber", "Line" ),
									new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Status"),
									new TransactionAliasFieldClass( LineItem, Order++, "Product", "Product" ),
									new TransactionAliasFieldClass( LineItem, Order++, "NetQuantity", "Net",true),
									new TransactionAliasFieldClass( LineItem,Order++,  "MassQuantity","Mass",true),

									// TODO: Temporarily commented out so that QA does not test financial configuration features.
                                    // new TransactionAliasFieldClass( LineItem, Order++, "ProductPrice", "Price" ),
									new TransactionAliasFieldClass( LineItem, Order++, "NetQuantityReceived", "Net Received", false, true),
									new TransactionAliasFieldClass( LineItem, Order++, "NetQuantityRemaining", "Net Remaining", false, true),
									new TransactionAliasFieldClass( LineItem, Order++, "MassQuantityReceived", "Mass Received", false, true),
									new TransactionAliasFieldClass( LineItem, Order++, "MassQuantityRemaining", "Mass Remaining", false, true),                           
									new TransactionAliasFieldClass( Note, Order++, "Notes", "Notes" ),

							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T18_SupplyOrder:
						{
							List<FieldClass> Fields = new List<FieldClass>
							{
								new TransactionAliasFieldClass( Transaction, Order++, "AliasName", "Alias" ),
								new TransactionAliasFieldClass( Transaction, Order++, "SupplierID","Supplier",true ),
								new TransactionAliasFieldClass( Transaction, Order++, "PONumber", "PO Number" ),
								new TransactionAliasFieldClass( Transaction, Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass( Transaction, Order++, "ManagerID", "Manager" ),
								new TransactionAliasFieldClass( Transaction, Order++, "OwnerID", "Owner" ),
								new TransactionAliasFieldClass( Transaction, Order++, "DocumentNumber", "Supply Order Number",true),
								new TransactionAliasFieldClass( Transaction, Order++, "LookupTransactionStatusIndex", "Transaction Status",true ),
								new TransactionAliasFieldClass( Transaction, Order++, "EffectiveDate", "Estimated Delivery Date From"),
								new TransactionAliasFieldClass( Transaction, Order++, "ExpirationDate", "Estimated Delivery Date To"),
								new TransactionAliasFieldClass( Transaction, Order++, "AutoComplete", "Auto Complete" ),
								new TransactionAliasFieldClass( Note,     Order++, "Notes", "Notes" ),
								new TransactionAliasFieldClass( LineItem, Order++,"LookupTransactionStatusIndex","Status"),
								new TransactionAliasFieldClass( LineItem, Order++, "Product", "Product" ),
								new TransactionAliasFieldClass( LineItem, Order++, "NetQuantity", "Net",true),
								new TransactionAliasFieldClass( LineItem, Order++, "NetQuantityReceived", "Net Received", false, true),
								new TransactionAliasFieldClass( LineItem, Order++, "NetQuantityRemaining", "Net Remaining", false, true),
							};

							this.InitializeFields(Fields.ToArray());
							break;
						}

					case TransactionTypes.T21_AccountPayableInvoice:
						{
							FieldClass[] Fields = 
							{
								new TransactionAliasFieldClass(Transaction, Order++, "DocumentNumber", "Document Number"),

                                // TODO: Temporarily commented out so that QA does not test financial configuration features.
								// new TransactionAliasFieldClass(Transaction, Order++, "TotalExcise", "Total Excise", false, true),
								// new TransactionAliasFieldClass(Transaction, Order++, "TotalGST", "Total GST", false, true),
								// new TransactionAliasFieldClass(Transaction, Order++, "TotalPriceAmount", "Total Price", false, true),
								// new TransactionAliasFieldClass(Transaction, Order++, "TotalPriceWithTax", "Total Price with Tax", false, true),
								new TransactionAliasFieldClass(Transaction, Order++, "LegacyNumber", "Payment Number"),
								new TransactionAliasFieldClass(Transaction, Order++, "ShipmentNumber", "Receipt Number"),
								new TransactionAliasFieldClass(Transaction, Order++, "SupplierID", "Supplier"),
								new TransactionAliasFieldClass(Transaction, Order++, "TransID", "ID"),
								new TransactionAliasFieldClass(Transaction, Order++, "Site", "Site"),
								new TransactionAliasFieldClass(Transaction, Order++, "AliasName", "Alias"),
								new TransactionAliasFieldClass(Transaction, Order++, "TransDateTime", "Transaction Date"),
								new TransactionAliasFieldClass(Transaction, Order++, "InventoryDate", "Inventory Date", true),
								new TransactionAliasFieldClass(Transaction, Order++, "ManagerID", "Manager"),
								new TransactionAliasFieldClass(Transaction, Order++, "OwnerID", "Owner"),
								new TransactionAliasFieldClass(Transaction, Order++, "Flag01", "Invoice/Receipt Comparison"),
								new TransactionAliasFieldClass(LineItem, Order++, "InvoiceNumber", "Invoice Number"),
								new TransactionAliasFieldClass(LineItem, Order++, "InvoiceLineNumber", "Invoice Line Number"),
								new TransactionAliasFieldClass(LineItem, Order++, "Product", "Product"),

								// TODO: Temporarily commented out so that QA does not test financial configuration features.
                                // new TransactionAliasFieldClass(LineItem, Order++, "ProductPrice", "Price"),
								new TransactionAliasFieldClass(LineItem, Order++, "GrossQuantity", "Gross", true),
								new TransactionAliasFieldClass(LineItem, Order++, "NetQuantity", "Net", true),	
								new TransactionAliasFieldClass(LineItem, Order++, "MassQuantity", "Mass", true),

                                // TODO: Temporarily commented out so that QA does not test financial configuration features.
								// new TransactionAliasFieldClass(LineItem, Order++, "Tax1", "Invoiced Excise"),
								// new TransactionAliasFieldClass(LineItem, Order++, "Tax2", "Invoiced GST"),
								new TransactionAliasFieldClass(LineItem, Order++, "AccountCode", "Account Code"),
								new TransactionAliasFieldClass(LineItem, Order++, "CostCentreCode", "Cost Centre Code"),                                
								new TransactionAliasFieldClass(LineItem, Order++, "BatchNumber", "Batch Number")
										  
							};

							this.InitializeFields(Fields);
							break;
						}
					case TransactionTypes.T22_AccountReceivableInvoice:
						{
							FieldClass[] Fields = 
							{
								new TransactionAliasFieldClass(Transaction, Order++, "DocumentNumber", "Document Number"),

                                // TODO: Temporarily commented out so that QA does not test financial configuration features.
                                // new TransactionAliasFieldClass(Transaction, Order++, "TotalExcise", "Total Excise", false, true),
                                // new TransactionAliasFieldClass(Transaction, Order++, "TotalGST", "Total GST", false, true),
                                // new TransactionAliasFieldClass(Transaction, Order++, "TotalPriceAmount", "Total Price", false, true),
								new TransactionAliasFieldClass(Transaction, Order++, "LegacyNumber", "Payment Number"),
								new TransactionAliasFieldClass(Transaction, Order++, "ShipmentNumber", "Receipt Number"),
								new TransactionAliasFieldClass(Transaction, Order++, "SupplierID", "Supplier"),
								new TransactionAliasFieldClass(Transaction, Order++, "TransID", "ID"),
								new TransactionAliasFieldClass(Transaction, Order++, "Site", "Site"),
								new TransactionAliasFieldClass(Transaction, Order++, "AliasName", "Alias"),
								new TransactionAliasFieldClass(Transaction, Order++, "TransDateTime", "Transaction Date"),
								new TransactionAliasFieldClass(Transaction, Order++, "InventoryDate", "Inventory Date", true),
								new TransactionAliasFieldClass(Transaction, Order++, "ManagerID", "Manager"),
								new TransactionAliasFieldClass(Transaction, Order++, "OwnerID", "Owner"),
								new TransactionAliasFieldClass(Transaction, Order++, "ShipToID", "Ship To"),
								new TransactionAliasFieldClass(Transaction, Order++, "Flag01", "Invoice/Receipt Comparison"),
								new TransactionAliasFieldClass(LineItem, Order++, "InvoiceNumber", "Invoice Number"),
								new TransactionAliasFieldClass(LineItem, Order++, "InvoiceLineNumber", "Invoice Line Number"),
								new TransactionAliasFieldClass(LineItem, Order++, "Product", "Product"),
								
                                // TODO: Temporarily commented out so that QA does not test financial configuration features.
                                // new TransactionAliasFieldClass(LineItem, Order++, "ProductPrice", "Price"),
								new TransactionAliasFieldClass(LineItem, Order++, "GrossQuantity", "Gross", true),
								new TransactionAliasFieldClass(LineItem, Order++, "NetQuantity", "Net", true),	
								new TransactionAliasFieldClass(LineItem,Order++,  "MassQuantity", "Mass", true),
								
                                // TODO: Temporarily commented out so that QA does not test financial configuration features.
                                // new TransactionAliasFieldClass(LineItem, Order++, "Tax1", "Invoiced Excise"),
								// new TransactionAliasFieldClass(LineItem, Order++, "Tax2", "Invoiced GST"),                                
								new TransactionAliasFieldClass(LineItem, Order++, "CostCentreCode", "Cost Centre Code")
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T23_StorageTransfer:
						{
							FieldClass[] Fields = 
							{
								new TransactionAliasFieldClass(Transaction, Order++, "ManagerID", "Manager", true),
								new TransactionAliasFieldClass(Transaction, Order++, "OwnerID", "Owner", true),
								new TransactionAliasFieldClass(Transaction, Order++, "TransDateTime", "Transaction Date"),
								new TransactionAliasFieldClass(Transaction, Order++, "InventoryDate", "Inventory Date", true),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Transaction Status",true),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"FromStorageLocationID","From Tank",true),                            
								new TransactionAliasFieldClass(LineItem,Order++,"ToStorageLocationID","To Tank",true)
							};

							this.InitializeFields(Fields);
							break;
						}

					case TransactionTypes.T25_Shipment:
						{
							FieldClass[] Fields =
							{
								new TransactionAliasFieldClass(Transaction,Order++,"TransID","ID"),
								new TransactionAliasFieldClass(Transaction,Order++,"Site","Site"),
								new TransactionAliasFieldClass(Transaction,Order++,"AliasName","Alias"),
								new TransactionAliasFieldClass(Transaction,Order++,"TransDateTime","Transaction Date"),
								new TransactionAliasFieldClass(Transaction,Order++,"InventoryDate","Inventory Date",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ManagerID","Manager"),
								new TransactionAliasFieldClass(Transaction,Order++,"OwnerID","Owner"),
								new TransactionAliasFieldClass(Transaction,Order++,"ShipperID","Shipper"),
								new TransactionAliasFieldClass(Transaction,Order++,"BillToID","Bill To",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ShipToID","Ship To",true),
								new TransactionAliasFieldClass(Transaction,Order++,"CarrierID","Carrier",true),
								new TransactionAliasFieldClass(Transaction,Order++,"ReversalType","Reversal",true),
								new TransactionAliasFieldClass(Transaction,Order++,"LookupTransactionStatusIndex","Status"),
								new TransactionAliasFieldClass(Transaction,Order++,"OperatorID","Operator"),
								new TransactionAliasFieldClass(Transaction,Order++,"DestinationRegistrationID1","Tractor"),
								new TransactionAliasFieldClass(Transaction,Order++,"DestinationRegistrationID2","Trailer"),
								new TransactionAliasFieldClass(WeightReading,Order++,"BeginQuantityValue","Tare Weight"),
								new TransactionAliasFieldClass(WeightReading,Order++,"FinalQuantityValue","Final Weight"),
								new TransactionAliasFieldClass(LineItem,Order++,"LoadingLocationID","Station"),
								new TransactionAliasFieldClass(LineItem,Order++,"BatchNumber","Batch"),
								new TransactionAliasFieldClass(LineItem,Order++,"ArmNumber","Arm"),
								new TransactionAliasFieldClass(LineItem,Order++,"LookupTransactionStatusIndex","Status"),
								new TransactionAliasFieldClass(LineItem,Order++,"Product","Product"),
								new TransactionAliasFieldClass(LineItem,Order++,"AdditiveProfileID","Additive"),
								new TransactionAliasFieldClass(LineItem,Order++,"PresetAmount","Preset"),
								new TransactionAliasFieldClass(LineItem,Order++,"GrossQuantity","Gross",true),
								new TransactionAliasFieldClass(LineItem,Order++,"NetQuantity","Net",true),
								new TransactionAliasFieldClass(LineItem,Order++,"MassQuantity","Mass",true),
								new TransactionAliasFieldClass(LineItem,Order++,"Density","Density"),
								new TransactionAliasFieldClass(LineItem,Order++,"Temperature","Temp"),
								new TransactionAliasFieldClass(LineItem,Order++,"DestinationRegistrationID","Equipment"),
								new TransactionAliasFieldClass(LineItem,Order++,"DestinationCompartmentID","Compartment"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterID","Meter"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterStart","Start"),
								new TransactionAliasFieldClass(LineItem,Order++,"MeterStop","Stop"),
								new TransactionAliasFieldClass(LineItem,Order++,"StorageLocationID","Tank",true),

							};

							this.InitializeFields(Fields);

							break;
						}

					default:
						break;
				}
			}
		}

		public bool MeterCloseout
		{
			get { return this._MeterCloseout; }
			set {
			    this._MeterCloseout = value; }
		}

		public bool BulkShipment
		{
			get { return this._BulkShipment; }
			set {
			    this._BulkShipment = value; }
		}

		public bool DistributedImpact
		{
			get { return this._DistributedImpact; }
			set {
			    this._DistributedImpact = value; }
		}

		public bool MultipleLineItems
		{
			get { return this._MultipleLineItems; }
			set {
			    this._MultipleLineItems = value; }
		}

		public bool LineItemEditControl
		{
			get { return this._LineItemEditControl; }
			set {
			    this._LineItemEditControl = value; }
		}

		public bool MultipleWeightReadings
		{
			get { return this._MultipleWeightReadings; }
			set {
			    this._MultipleWeightReadings = value; }
		}

		public bool LimitSelectionsBasedOnHierarchy
		{
			get { return this._LimitSelectionsBasedOnHierarchy; }
			set {
			    this._LimitSelectionsBasedOnHierarchy = value; }
		}

		public bool WeightReadingEditControl
		{
			get { return this._WeightReadingEditControl; }
			set {
			    this._WeightReadingEditControl = value; }
		}

		public bool UseComboxControls
		{
			get { return this._UseComboBoxControls; }
			set {
			    this._UseComboBoxControls = value; }
		}

		public Guid AssociatedTransactionAliasGuid
		{
			get { return this._AssociatedTransactionAliasGuid; }
			set {
			    this._AssociatedTransactionAliasGuid = value; }
		}

		public bool MultipleTransportLineItems
		{
			get { return this.multipleTransportLineItems; }
			set { this.multipleTransportLineItems = value; }
		}

		// Units
		[XmlIgnore]
		public EngineeringUnit LevelUnits
		{
			get { return this._LevelUnits; }
			set {
			    this._LevelUnits = value; }
		}

		public int LevelUnitsInt
		{
			get { return (int)this.LevelUnits; }
			set
			{
				if (value != 0)
				{
				    this.LevelUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public EngineeringUnit TemperatureUnits
		{
			get { return this._TemperatureUnits; }
			set {
			    this._TemperatureUnits = value; }
		}

		public int TemperatureUnitsInt
		{
			get { return (int)this.TemperatureUnits; }
			set
			{
				if (value != 0)
				{
				    this.TemperatureUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public EngineeringUnit DensityUnits
		{
			get { return this._DensityUnits; }
			set {
			    this._DensityUnits = value; }
		}

		public int DensityUnitsInt
		{
			get { return (int)this.DensityUnits; }
			set
			{
				if (value != 0)
				{
				    this.DensityUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public EngineeringUnit PressureUnits
		{
			get { return this._PressureUnits; }
			set {
			    this._PressureUnits = value; }
		}

		public int PressureUnitsInt
		{
			get { return (int)this.PressureUnits; }
			set { if (value != 0) {
			    this.PressureUnits = (EngineeringUnit)value; } }
		}

		[XmlIgnore]
		public EngineeringUnit FlowUnits
		{
			get { return this._FlowUnits; }
			set {
			    this._FlowUnits = value; }
		}

		public int FlowUnitsInt
		{
			get { return (int)this.FlowUnits; }
			set
			{
				if (value != 0)
				{
				    this.FlowUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public EngineeringUnit VolumeUnits
		{
			get { return this._VolumeUnits; }
			set {
			    this._VolumeUnits = value; }
		}

		public int VolumeUnitsInt
		{
			get { return (int)this.VolumeUnits; }
			set
			{
				if (value != 0)
				{
				    this.VolumeUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public EngineeringUnit MassUnits
		{
			get { return this._MassUnits; }
			set {
			    this._MassUnits = value; }
		}

		public int MassUnitsInt
		{
			get { return (int)this.MassUnits; }
			set
			{
				if (value != 0)
				{
				    this.MassUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public EngineeringUnit AdditiveVolumeUnits
		{
			get { return this._AdditiveVolumeUnits; }
			set {
			    this._AdditiveVolumeUnits = value; }
		}

		public int AdditiveVolumeUnitsInt
		{
			get { return (int)this.AdditiveVolumeUnits; }
			set
			{
				if (value != 0)
				{
				    this.AdditiveVolumeUnits = (EngineeringUnit)value;
				}
			}
		}

		[XmlIgnore]
		public string LevelDecimalPlaces
		{
			get { return this._LevelDecimalPlaces.ToString(); }
			set {
			    this.SetByte("Level Decimal Places", value, ref this._LevelDecimalPlaces); }
		}

		[XmlIgnore]
		public string TemperatureDecimalPlaces
		{
			get { return this._TemperatureDecimalPlaces.ToString(); }
			set
			{
			    this.SetByte("Level Decimal Places", value, ref this._TemperatureDecimalPlaces);
			}
		}

		[XmlIgnore]
		public string DensityDecimalPlaces
		{
			get { return this._DensityDecimalPlaces.ToString(); }
			set
			{
			    this.SetByte("Level Decimal Places", value, ref this._DensityDecimalPlaces);
			}
		}

		[XmlIgnore]
		public string PressureDecimalPlaces
		{
			get { return this._PressureDecimalPlaces.ToString(); }
			set
			{
			    this.SetByte("Level Decimal Places", value, ref this._PressureDecimalPlaces);
			}
		}

		[XmlIgnore]
		public string FlowDecimalPlaces
		{
			get { return this._FlowDecimalPlaces.ToString(); }
			set {
			    this.SetByte("Level Decimal Places", value, ref this._FlowDecimalPlaces); }
		}

		[XmlIgnore]
		public string VolumeDecimalPlaces
		{
			get { return this._VolumeDecimalPlaces.ToString(); }
			set {
			    this.SetByte("Level Decimal Places", value, ref this._VolumeDecimalPlaces); }
		}

		[XmlIgnore]
		public string MassDecimalPlaces
		{
			get { return this._MassDecimalPlaces.ToString(); }
			set {
			    this.SetByte("Level Decimal Places", value, ref this._MassDecimalPlaces); }
		}

		[XmlIgnore]
		public string AdditiveVolumeDecimalPlaces
		{
			get { return this._AdditiveVolumeDecimalPlaces.ToString(); }
			set {
			    this.SetByte("Level Decimal Places", value, ref this._AdditiveVolumeDecimalPlaces); }
		}

		/// <summary>
		/// For replication
		/// </summary>
		public ulong[] InternalDestinationTypes
		{
			get { return this._DestinationEquipmentTypes; }
			set {
			    this._DestinationEquipmentTypes = value; }
		}

		/// <summary>
		/// For replication
		/// </summary>
		public ulong[] InternalSourceTypes
		{
			get { return this._SourceEquipmentTypes; }
			set {
			    this._SourceEquipmentTypes = value; }
		}

		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.TRANSACTION_ALIAS; }
		}

		public override ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}

		/// <summary>
		/// Contains a collection of child Aliases that have been associated
		/// with this Alias
		/// </summary>
		public TransactionAliasCollectionClass AssociatedAliases
		{
			get { return this.associatedAliases; }
			set {
			    this.associatedAliases = value; }
		}

		/// <summary>
		/// Determines whether a transaction should aggregate values
		/// based on its collection of associated transactions
		/// </summary>
		/// <remarks>At this time only Supply Order transaction
		/// types can aggregate associated transactions</remarks>
		public bool AggregateAssociatedTransactions
		{
			get { return this.aggregateAssociatedTransactions; }
			set {
			    this.aggregateAssociatedTransactions = value; }
		}

		public bool EnableTotalQtyExceededWarning
		{
			get { return this.enableTotalQtyExceededWarning; }
			set { this.enableTotalQtyExceededWarning = value; }
		}

		public bool EnableTotalValueExceededWarning
		{
			get { return this.enableTotalValueExceededWarning; }
			set { this.enableTotalValueExceededWarning = value; }
		}

		public bool EnableQtyToleranceExceededWarning
		{
			get { return this.enableQtyToleranceExceededWarning; }
			set { this.enableQtyToleranceExceededWarning = value; }
		}

		public bool EnableValueToleranceExceededWarning
		{
			get { return this.enableValueToleranceExceededWarning; }
			set { this.enableValueToleranceExceededWarning = value; }
		}

		/// <summary>
		/// Contains a collection of integers representing the transaction statuses
		/// that have been assigned to the transaction alias
		/// </summary>
		public ArrayList AssignedStatuses
		{
			get { return this.assignedStatuses; }
			set { this.assignedStatuses = value; }
		}

		/// <summary>
		/// Indicates whether to show company name, ID, or both in 
		/// company UI controls
		/// </summary>
		public TRANSACTION_SHOW_COMPANY_NAME ShowCompanyName
		{
			get { return this._showCompanyName; }
			set { this._showCompanyName = value; }
		}

		/// <summary>
		/// Indicates whether or not to include this TransactionAlias in the new Dispatch screens
		/// </summary>
		public bool IncludeInDispatch
		{
			get { return this._IncludeInDispatch; }
			set { this._IncludeInDispatch = value; }
		}

        public Guid MasterRecordGuid { get { return this._MasterRecordGuid; } set {
            this._MasterRecordGuid = value; } }

        public Guid AssignedToSiteGuid { get { return this._AssignedToSiteGuid; } set {
            this._AssignedToSiteGuid = value; } }

        public Guid AssignedFromSiteGuid { get { return this._AssignedFromSiteGuid; } set {
            this._AssignedFromSiteGuid = value; } }

        public string AssignedFromSiteId { get { return this._AssignedFromSiteId; } set {
            this._AssignedFromSiteId = value; } }


		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblTransactionAliases " +
					"(LookupTransTypeIndex," +
					"AliasName," +
					"SiteGuid," +
					"MeterCloseout," +
					"BulkShipment," +
					"DistributedImpact," +
					"MultipleLineItems," +
					"LineItemEditControl," +
					"MultipleWeightReadings," +
					"LimitSelectionsBasedOnHierarchy," +
					"WeightReadingEditControl," +
					"AssociatedTransactionAliasGuid," +
					"AssociatedReport," +
					"AssociatedPreloadReport," +
					"DestinationEquipmentTypes1," +
					"DestinationEquipmentTypes2," +
					"DestinationEquipmentTypes3," +
					"SourceEquipmentTypes1," +
					"SourceEquipmentTypes2," +
					"SourceEquipmentTypes3," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"ShowCompanyName, " +
					"AggregateAssocTrans, " +
					"EnableTotalQuantityExceededWarning, " +
					"EnableQuantityToleranceExceededWarning, " +
					"EnableTotalValueExceededWarning, " +
					"EnableValueToleranceExceededWarning, " +
					"LookupDefaultStatusIndex," +
					"LevelUnitIndex," +
					"TemperatureUnitIndex," +
					"DensityUnitIndex," +
					"PressureUnitIndex," +
					"FlowUnitIndex," +
					"VolumeUnitIndex," +
					"MassUnitIndex," +
					"AdditiveVolumeUnitIndex," +
					"LevelDecimalPlaces," +
					"TemperatureDecimalPlaces," +
					"DensityDecimalPlaces," +
					"PressureDecimalPlaces," +
					"FlowDecimalPlaces," +
					"VolumeDecimalPlaces," +
					"MassDecimalPlaces," +
					"AdditiveVolumeDecimalPlaces, " +
					"UseComboBoxControls, " +
					"MultipleTransportLineItems, " +
					"IncludeInDispatch, " +
					"EnableAutoCompleteControls," +
					"PermitNonReferenceData," +
					"UseTransactionDetailWithLayout," +
					"DefaultMeterToEquipmentID," +
					"LimitSourceEquipmentByProduct," +
					"RememberMeterEndForMeterID," +
					"PopulateCompaniesFromEquipment," +
					"PopulateGrossVolumeFromMeterValues," +
					"UseMeterAndCompressionFactorFromMeter," +
					"TransactionAliasGuid, " +
                     "_MasterRecordGuid" +
                    ") VALUES (" +
					"@LookupTransTypeIndex," +
					"@AliasName," +
					"@SiteGuid," +
					"@MeterCloseout," +
					"@BulkShipment," +
					"@DistributedImpact," +
					"@MultipleLineItems," +
					"@LineItemEditControl," +
					"@MultipleWeightReadings," +
					"@LimitSelectionsBasedOnHierarchy," +
					"@WeightReadingEditControl," +
					"@AssociatedTransactionAliasGuid," +
					"@AssociatedReport," +
					"@AssociatedPreloadReport," +
					"@DestinationEquipmentTypes1," +
					"@DestinationEquipmentTypes2," +
					"@DestinationEquipmentTypes3," +
					"@SourceEquipmentTypes1," +
					"@SourceEquipmentTypes2," +
					"@SourceEquipmentTypes3," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@ShowCompanyName, " +
					"@AggregateAssocTrans, " +
					"@EnableTotalQuantityExceededWarning, " +
					"@EnableQuantityToleranceExceededWarning, " +
					"@EnableTotalValueExceededWarning, " +
					"@EnableValueToleranceExceededWarning, " +
					"@LookupDefaultStatusIndex," +
					"@LevelUnitIndex," +
					"@TemperatureUnitIndex," +
					"@DensityUnitIndex," +
					"@PressureUnitIndex," +
					"@FlowUnitIndex," +
					"@VolumeUnitIndex," +
					"@MassUnitIndex," +
					"@AdditiveVolumeUnitIndex," +
					"@LevelDecimalPlaces," +
					"@TemperatureDecimalPlaces," +
					"@DensityDecimalPlaces," +
					"@PressureDecimalPlaces," +
					"@FlowDecimalPlaces," +
					"@VolumeDecimalPlaces," +
					"@MassDecimalPlaces," +
					"@AdditiveVolumeDecimalPlaces, " +
					"@UseComboBoxControls, " +
					"@MultipleTransportLineItems, " +
					"@IncludeInDispatch, " +
					"@EnableAutoCompleteControls," +
					"@PermitNonReferenceData," +
					"@UseTransactionDetailWithLayout," +
					"@DefaultMeterToEquipmentID," +
					"@LimitSourceEquipmentByProduct," +
					"@RememberMeterEndForMeterID," +
					"@PopulateCompaniesFromEquipment," +
					"@PopulateGrossVolumeFromMeterValues," +
					"@UseMeterAndCompressionFactorFromMeter," +
					"@TransactionAliasGuid," +
                    "@MasterRecordGuid" +
                    ") ";

			cmd.Parameters.AddWithValue("@LookupTransTypeIndex", ((int)this._TransTypeID));
			cmd.Parameters.AddWithValue("@AliasName", this._ID);
			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@MeterCloseout", (this._MeterCloseout ? "1" : "0"));
			cmd.Parameters.AddWithValue("@BulkShipment", (this._BulkShipment ? "1" : "0"));
			cmd.Parameters.AddWithValue("@DistributedImpact", (this._DistributedImpact ? "1" : "0"));
			cmd.Parameters.AddWithValue("@MultipleLineItems", (this._MultipleLineItems ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LineItemEditControl", (this._LineItemEditControl ? "1" : "0"));
			cmd.Parameters.AddWithValue("@MultipleWeightReadings", (this._MultipleWeightReadings ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LimitSelectionsBasedOnHierarchy", (this._LimitSelectionsBasedOnHierarchy ? "1" : "0"));
			cmd.Parameters.AddWithValue("@WeightReadingEditControl", (this._WeightReadingEditControl ? "1" : "0"));
			cmd.Parameters.Add("@AssociatedTransactionAliasGuid", SqlDbType.UniqueIdentifier).Value = (this._AssociatedTransactionAliasGuid == Guid.Empty) ? (object)DBNull.Value : (object)this._AssociatedTransactionAliasGuid;
			cmd.Parameters.AddWithValue("@AssociatedReport", this.AssociatedReport);
			cmd.Parameters.AddWithValue("@AssociatedPreloadReport", this.AssociatedPreloadReport);
			cmd.Parameters.AddWithValue("@DestinationEquipmentTypes1", (long)this._DestinationEquipmentTypes[0]);
			cmd.Parameters.AddWithValue("@DestinationEquipmentTypes2", (long)this._DestinationEquipmentTypes[1]);
			cmd.Parameters.AddWithValue("@DestinationEquipmentTypes3", (long)this._DestinationEquipmentTypes[2]);
			cmd.Parameters.AddWithValue("@SourceEquipmentTypes1", (long)this._SourceEquipmentTypes[0]);
			cmd.Parameters.AddWithValue("@SourceEquipmentTypes2", (long)this._SourceEquipmentTypes[1]);
			cmd.Parameters.AddWithValue("@SourceEquipmentTypes3", (long)this._SourceEquipmentTypes[2]);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@ShowCompanyName", ((short)this._showCompanyName));
			cmd.Parameters.AddWithValue("@AggregateAssocTrans", (this.aggregateAssociatedTransactions ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableTotalQuantityExceededWarning", (this.enableTotalQtyExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableQuantityToleranceExceededWarning", (this.enableQtyToleranceExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableTotalValueExceededWarning", (this.enableTotalValueExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableValueToleranceExceededWarning", (this.enableValueToleranceExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LookupDefaultStatusIndex", (this.LookupDefaultStatusIndex));
			cmd.Parameters.Add("@LevelUnitIndex", SqlDbType.Int).Value = (this._LevelUnits == 0) ? (object)DBNull.Value : (object)((int)this._LevelUnits);
			cmd.Parameters.Add("@TemperatureUnitIndex", SqlDbType.Int).Value = (this._TemperatureUnits == 0) ? (object)DBNull.Value : (object)((int)this._TemperatureUnits);
			cmd.Parameters.Add("@DensityUnitIndex", SqlDbType.Int).Value = (this._DensityUnits == 0) ? (object)DBNull.Value : (object)((int)this._DensityUnits);
			cmd.Parameters.Add("@PressureUnitIndex", SqlDbType.Int).Value = (this._PressureUnits == 0) ? (object)DBNull.Value : (object)((int)this._PressureUnits);
			cmd.Parameters.Add("@FlowUnitIndex", SqlDbType.Int).Value = (this._FlowUnits == 0) ? (object)DBNull.Value : (object)((int)this._FlowUnits);
			cmd.Parameters.Add("@VolumeUnitIndex", SqlDbType.Int).Value = (this._VolumeUnits == 0) ? (object)DBNull.Value : (object)((int)this._VolumeUnits);
			cmd.Parameters.Add("@MassUnitIndex", SqlDbType.Int).Value = (this._MassUnits == 0) ? (object)DBNull.Value : (object)((int)this._MassUnits);
			cmd.Parameters.Add("@AdditiveVolumeUnitIndex", SqlDbType.Int).Value = (this._AdditiveVolumeUnits == 0) ? (object)DBNull.Value : (object)((int)this._AdditiveVolumeUnits);
			cmd.Parameters.AddWithValue("@LevelDecimalPlaces", this._LevelDecimalPlaces);
			cmd.Parameters.AddWithValue("@TemperatureDecimalPlaces", this._TemperatureDecimalPlaces);
			cmd.Parameters.AddWithValue("@DensityDecimalPlaces", this._DensityDecimalPlaces);
			cmd.Parameters.AddWithValue("@PressureDecimalPlaces", this._PressureDecimalPlaces);
			cmd.Parameters.AddWithValue("@FlowDecimalPlaces", this._FlowDecimalPlaces);
			cmd.Parameters.AddWithValue("@VolumeDecimalPlaces", this._VolumeDecimalPlaces);
			cmd.Parameters.AddWithValue("@MassDecimalPlaces", this._MassDecimalPlaces);
			cmd.Parameters.AddWithValue("@AdditiveVolumeDecimalPlaces", this._AdditiveVolumeDecimalPlaces);
			cmd.Parameters.AddWithValue("@UseComboBoxControls", (this._UseComboBoxControls ? "1" : "0"));
			cmd.Parameters.AddWithValue("@MultipleTransportLineItems", (this.multipleTransportLineItems ? "1" : "0"));
			cmd.Parameters.AddWithValue("@IncludeInDispatch", (this._IncludeInDispatch ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableAutoCompleteControls", ( this.EnableAutoCompleteControls ? "1" : "0" ) );
			cmd.Parameters.AddWithValue("@PermitNonReferenceData", (this.PermitNonReferenceData ? "1" : "0"));
			cmd.Parameters.AddWithValue("@UseTransactionDetailWithLayout", (this.UseTransactionDetailWithLayout ? "1" : "0"));
			cmd.Parameters.AddWithValue("@DefaultMeterToEquipmentID", (this.DefaultMeterToEquipmentID ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LimitSourceEquipmentByProduct", (this.LimitSourceEquipmentByProduct ? "1" : "0"));
			cmd.Parameters.AddWithValue("@RememberMeterEndForMeterID", (this.RememberMeterEndForMeterID ? "1" : "0"));
			cmd.Parameters.AddWithValue("@PopulateCompaniesFromEquipment", (this.PopulateCompaniesFromEquipment ? "1" : "0"));
			cmd.Parameters.AddWithValue("@PopulateGrossVolumeFromMeterValues", (this.PopulateGrossVolumeFromMeterValues ? "1" : "0"));
			cmd.Parameters.AddWithValue("@UseMeterAndCompressionFactorFromMeter", (this.UseMeterAndCompressionFactorFromMeter ? "1" : "0"));
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._IdentityGuid);
            cmd.Parameters.AddWithValue("@MasterRecordGuid", this._IdentityGuid);      //This query can only be used to create master record versions.
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblTransactionAliases SET " +
				"LookupTransTypeIndex = @LookupTransTypeIndex, " +
				"AliasName = @AliasName, " +
				"SiteGuid = @SiteGuid, " +
				"MeterCloseout = @MeterCloseout, " +
				"BulkShipment = @BulkShipment, " +
				"DistributedImpact = @DistributedImpact, " +
				"MultipleLineItems = @MultipleLineItems, " +
				"LineItemEditControl = @LineItemEditControl, " +
				"MultipleWeightReadings = @MultipleWeightReadings, " +
				"MultipleTransportLineItems = @MultipleTransportLineItems, " +
				"LimitSelectionsBasedOnHierarchy = @LimitSelectionsBasedOnHierarchy, " +
				"WeightReadingEditControl = @WeightReadingEditControl, " +
				"AssociatedTransactionAliasGuid = @AssociatedTransactionAliasGuid, " +
				"AssociatedReport = @AssociatedReport, " +
				"AssociatedPreloadReport = @AssociatedPreloadReport, " +
				"DestinationEquipmentTypes1 = @DestinationEquipmentTypes1, " +
				"DestinationEquipmentTypes2 = @DestinationEquipmentTypes2, " +
				"DestinationEquipmentTypes3 = @DestinationEquipmentTypes3, " +
				"SourceEquipmentTypes1 = @SourceEquipmentTypes1, " +
				"SourceEquipmentTypes2 = @SourceEquipmentTypes2, " +
				"SourceEquipmentTypes3 = @SourceEquipmentTypes3, " +
				"UpdatedDate = @UpdatedDate, " +
				"UpdatedBy = @UpdatedBy, " +
				"ShowCompanyName = @ShowCompanyName, " +
				"AggregateAssocTrans = @AggregateAssocTrans, " +
				"EnableTotalQuantityExceededWarning = @EnableTotalQuantityExceededWarning, " +
				"EnableQuantityToleranceExceededWarning = @EnableQuantityToleranceExceededWarning, " +
				"EnableTotalValueExceededWarning = @EnableTotalValueExceededWarning, " +
				"EnableValueToleranceExceededWarning = @EnableValueToleranceExceededWarning, " +
				"LookupDefaultStatusIndex = @LookupDefaultStatusIndex, " +
				"LevelUnitIndex = @LevelUnitIndex, " +
				"VolumeUnitIndex = @VolumeUnitIndex, " +
				"TemperatureUnitIndex = @TemperatureUnitIndex, " +
				"DensityUnitIndex = @DensityUnitIndex, " +
				"MassUnitIndex = @MassUnitIndex, " +
				"FlowUnitIndex = @FlowUnitIndex, " +
				"PressureUnitIndex = @PressureUnitIndex, " +
				"AdditiveVolumeUnitIndex = @AdditiveVolumeUnitIndex, " +
				"LevelDecimalPlaces = @LevelDecimalPlaces, " +
				"VolumeDecimalPlaces = @VolumeDecimalPlaces, " +
				"TemperatureDecimalPlaces = @TemperatureDecimalPlaces, " +
				"DensityDecimalPlaces = @DensityDecimalPlaces, " +
				"MassDecimalPlaces = @MassDecimalPlaces, " +
				"FlowDecimalPlaces = @FlowDecimalPlaces, " +
				"PressureDecimalPlaces = @PressureDecimalPlaces, " +
				"AdditiveVolumeDecimalPlaces = @AdditiveVolumeDecimalPlaces, " +
				"UseComboBoxControls = @UseComboBoxControls, " +
				"IncludeInDispatch = @IncludeInDispatch, " +
				"EnableAutoCompleteControls = @EnableAutoCompleteControls, " +
				"PermitNonReferenceData = @PermitNonReferenceData, " +
				"UseTransactionDetailWithLayout = @UseTransactionDetailWithLayout, " +
				"DefaultMeterToEquipmentID = @DefaultMeterToEquipmentID, " +
				"LimitSourceEquipmentByProduct = @LimitSourceEquipmentByProduct, " +
				"RememberMeterEndForMeterID = @RememberMeterEndForMeterID, " +
				"PopulateCompaniesFromEquipment = @PopulateCompaniesFromEquipment, " +
				"PopulateGrossVolumeFromMeterValues = @PopulateGrossVolumeFromMeterValues, " +
				"UseMeterAndCompressionFactorFromMeter = @UseMeterAndCompressionFactorFromMeter " +
				"WHERE TransactionAliasGuid = @TransactionAliasGuid";

			cmd.Parameters.AddWithValue("@LookupTransTypeIndex", ((int)this._TransTypeID));
			cmd.Parameters.AddWithValue("@AliasName", this._ID);
			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@MeterCloseout", (this._MeterCloseout ? "1" : "0"));
			cmd.Parameters.AddWithValue("@BulkShipment", (this._BulkShipment ? "1" : "0"));
			cmd.Parameters.AddWithValue("@DistributedImpact", (this._DistributedImpact ? "1" : "0"));
			cmd.Parameters.AddWithValue("@MultipleLineItems", (this._MultipleLineItems ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LineItemEditControl", (this._LineItemEditControl ? "1" : "0"));
			cmd.Parameters.AddWithValue("@MultipleWeightReadings", (this._MultipleWeightReadings ? "1" : "0"));
			cmd.Parameters.AddWithValue("@MultipleTransportLineItems", (this.multipleTransportLineItems ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LimitSelectionsBasedOnHierarchy", (this._LimitSelectionsBasedOnHierarchy ? "1" : "0"));
			cmd.Parameters.AddWithValue("@WeightReadingEditControl", (this._WeightReadingEditControl ? "1" : "0"));
			cmd.Parameters.Add("@AssociatedTransactionAliasGuid", SqlDbType.UniqueIdentifier).Value = (this._AssociatedTransactionAliasGuid == Guid.Empty) ? (object)DBNull.Value : (object)this._AssociatedTransactionAliasGuid;
			cmd.Parameters.AddWithValue("@AssociatedReport", this.AssociatedReport);
			cmd.Parameters.AddWithValue("@AssociatedPreloadReport", this.AssociatedPreloadReport);
			cmd.Parameters.AddWithValue("@DestinationEquipmentTypes1", (long)this._DestinationEquipmentTypes[0]);
			cmd.Parameters.AddWithValue("@DestinationEquipmentTypes2", (long)this._DestinationEquipmentTypes[1]);
			cmd.Parameters.AddWithValue("@DestinationEquipmentTypes3", (long)this._DestinationEquipmentTypes[2]);
			cmd.Parameters.AddWithValue("@SourceEquipmentTypes1", (long)this._SourceEquipmentTypes[0]);
			cmd.Parameters.AddWithValue("@SourceEquipmentTypes2", (long)this._SourceEquipmentTypes[1]);
			cmd.Parameters.AddWithValue("@SourceEquipmentTypes3", (long)this._SourceEquipmentTypes[2]);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@ShowCompanyName", ((short)this._showCompanyName));
			cmd.Parameters.AddWithValue("@AggregateAssocTrans", (this.aggregateAssociatedTransactions ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableTotalQuantityExceededWarning", (this.enableTotalQtyExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableQuantityToleranceExceededWarning", (this.enableQtyToleranceExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableTotalValueExceededWarning", (this.enableTotalValueExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableValueToleranceExceededWarning", (this.enableValueToleranceExceededWarning ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LookupDefaultStatusIndex", this.LookupDefaultStatusIndex);
			cmd.Parameters.Add("@LevelUnitIndex", SqlDbType.Int).Value = (this._LevelUnits == 0) ? (object)DBNull.Value : (object)((int)this._LevelUnits);
			cmd.Parameters.Add("@VolumeUnitIndex", SqlDbType.Int).Value = (this._VolumeUnits == 0) ? (object)DBNull.Value : (object)((int)this._VolumeUnits);
			cmd.Parameters.Add("@TemperatureUnitIndex", SqlDbType.Int).Value = (this._TemperatureUnits == 0) ? (object)DBNull.Value : (object)((int)this._TemperatureUnits);
			cmd.Parameters.Add("@DensityUnitIndex", SqlDbType.Int).Value = (this._DensityUnits == 0) ? (object)DBNull.Value : (object)((int)this._DensityUnits);
			cmd.Parameters.Add("@MassUnitIndex", SqlDbType.Int).Value = (this._MassUnits == 0) ? (object)DBNull.Value : (object)((int)this._MassUnits);
			cmd.Parameters.Add("@FlowUnitIndex", SqlDbType.Int).Value = (this._FlowUnits == 0) ? (object)DBNull.Value : (object)((int)this._FlowUnits);
			cmd.Parameters.Add("@PressureUnitIndex", SqlDbType.Int).Value = (this._PressureUnits == 0) ? (object)DBNull.Value : (object)((int)this._PressureUnits);
			cmd.Parameters.Add("@AdditiveVolumeUnitIndex", SqlDbType.Int).Value = (this._AdditiveVolumeUnits == 0) ? (object)DBNull.Value : (object)((int)this._AdditiveVolumeUnits);
			cmd.Parameters.AddWithValue("@LevelDecimalPlaces", this._LevelDecimalPlaces);
			cmd.Parameters.AddWithValue("@VolumeDecimalPlaces", this._VolumeDecimalPlaces);
			cmd.Parameters.AddWithValue("@TemperatureDecimalPlaces", this._TemperatureDecimalPlaces);
			cmd.Parameters.AddWithValue("@DensityDecimalPlaces", this._DensityDecimalPlaces);
			cmd.Parameters.AddWithValue("@MassDecimalPlaces", this._MassDecimalPlaces);
			cmd.Parameters.AddWithValue("@FlowDecimalPlaces", this._FlowDecimalPlaces);
			cmd.Parameters.AddWithValue("@PressureDecimalPlaces", this._PressureDecimalPlaces);
			cmd.Parameters.AddWithValue("@AdditiveVolumeDecimalPlaces", this._AdditiveVolumeDecimalPlaces);
			cmd.Parameters.AddWithValue("@UseComboBoxControls", (this._UseComboBoxControls ? "1" : "0"));
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._IdentityGuid);
			cmd.Parameters.AddWithValue("@IncludeInDispatch", (this._IncludeInDispatch ? "1" : "0"));
			cmd.Parameters.AddWithValue("@EnableAutoCompleteControls", (this.EnableAutoCompleteControls ? "1" : "0" ) );
			cmd.Parameters.AddWithValue("@PermitNonReferenceData", (this.PermitNonReferenceData ? "1" : "0"));
			cmd.Parameters.AddWithValue("@UseTransactionDetailWithLayout", (this.UseTransactionDetailWithLayout ? "1" : "0"));
			cmd.Parameters.AddWithValue("@DefaultMeterToEquipmentID", (this.DefaultMeterToEquipmentID ? "1" : "0"));
			cmd.Parameters.AddWithValue("@LimitSourceEquipmentByProduct", (this.LimitSourceEquipmentByProduct ? "1" : "0"));
			cmd.Parameters.AddWithValue("@RememberMeterEndForMeterID", (this.RememberMeterEndForMeterID ? "1" : "0"));
			cmd.Parameters.AddWithValue("@PopulateCompaniesFromEquipment", (this.PopulateCompaniesFromEquipment ? "1" : "0"));
			cmd.Parameters.AddWithValue("@PopulateGrossVolumeFromMeterValues", (this.PopulateGrossVolumeFromMeterValues ? "1" : "0"));
			cmd.Parameters.AddWithValue("@UseMeterAndCompressionFactorFromMeter", (this.UseMeterAndCompressionFactorFromMeter ? "1" : "0"));
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblTransactionAliases WHERE TransactionAliasGuid = @TransactionAliasGuid";
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._IdentityGuid);
		}

		/// <summary>
		/// Returns SQL used to purge all statuses assigned to the transaction alias
		/// </summary>
		/// <remarks>
		/// With assigned statuses...statuses will be removed then added again.
		/// There is no need to update a status
		/// </remarks>
		public void DeleteAssignedStatusesSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE map.tblTransactionAliasToStatus " +
				"WHERE TransactionAliasGuid = @TransactionAliasGuid";
			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.IdentityGuid);
		}

		/// <summary>
		/// Returns SQL used to retrieve Aliases associated with this Transaction Alias
		/// </summary>
        public void SelectAssociatedAliasesSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT tblTransactionAliases.*, NULL AS AssociatedAlias " +
							"FROM  tblTransactionAliases " +
							"WHERE  TransactionAliasGuid IN " +
									"(SELECT ChildTransactionAliasGuid FROM map.tblAssociatedTransactionAliases " +
                                    "WHERE ParentTransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @ParentTransactionAliasGuid, @TargetSiteGuid))";
			cmd.Parameters.AddWithValue("@ParentTransactionAliasGuid", this.IdentityGuid);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// Returns the SQL used to delete all aliases associated with this alias
		/// </summary>
		public void DeleteAssociatedAliasesSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE map.tblAssociatedTransactionAliases " +
							"WHERE ParentTransactionAliasGuid = @ParentTransactionAliasGuid";
			cmd.Parameters.AddWithValue("@ParentTransactionAliasGuid", this.IdentityGuid);
		}
		#endregion

		#region Public Methods
		public EQUIPMENT_TYPE[] GetCompartmentEquipmentTypes()
		{
			////////////////////////////////////////////////////////////////////////
			//Modify this block of code to use EquipmentTypeClass in correct manner.
			////////////////////////////////////////////////////////////////////////
			ulong EquipmentTypes = this._DestinationEquipmentTypes[0]
										| this._DestinationEquipmentTypes[1]
										| this._DestinationEquipmentTypes[2]
										| this._SourceEquipmentTypes[0]
										| this._SourceEquipmentTypes[1]
										| this._SourceEquipmentTypes[2];

			ArrayList Types = new ArrayList();

			for (EQUIPMENT_TYPE Type = EQUIPMENT_TYPE.TRAILER_TYPE; Type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; Type++)
			{
				if ((EquipmentTypes & (ulong)(0x1 << (int)Type)) != 0 && EquipmentTypeClass.HasCompartments(Type))
				{
					Types.Add(Type);
				}
			}

			return (EQUIPMENT_TYPE[])Types.ToArray(typeof(EQUIPMENT_TYPE));
		}


		public EQUIPMENT_TYPE[] GetEquipmentTypes()
		{
			ulong EquipmentTypes = 0;

			EquipmentTypes = this._DestinationEquipmentTypes[0]
								| this._DestinationEquipmentTypes[1]
								| this._DestinationEquipmentTypes[2]
								| this._SourceEquipmentTypes[0]
								| this._SourceEquipmentTypes[1]
								| this._SourceEquipmentTypes[2];

			ArrayList Types = new ArrayList();
			for (EQUIPMENT_TYPE Type = EQUIPMENT_TYPE.TRAILER_TYPE; Type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; Type++)
			{
				if ((EquipmentTypes & (ulong)(0x1 << (int)Type)) != 0)
				{
					Types.Add(Type);
				}
			}

			return (EQUIPMENT_TYPE[])Types.ToArray(typeof(EQUIPMENT_TYPE));
		}


		public EQUIPMENT_TYPE[] GetEquipmentTypes(bool Destination, byte Number)
		{
			ulong EquipmentTypes = 0;

			ArrayList Types = new ArrayList();

			if (Number < 1 || Number > 3)
			{
				return (EQUIPMENT_TYPE[])Types.ToArray(typeof(EQUIPMENT_TYPE));
			}

			if (Destination)
			{
				EquipmentTypes = this._DestinationEquipmentTypes[Number - 1];
			}
			else
			{
				EquipmentTypes = this._SourceEquipmentTypes[Number - 1];
			}

			for (EQUIPMENT_TYPE Type = EQUIPMENT_TYPE.TRAILER_TYPE; Type < EQUIPMENT_TYPE.MAX_EQUIPMENT_TYPE; Type++)
			{
				if ((EquipmentTypes & (ulong)(0x1 << (int)Type)) != 0)
				{
					Types.Add(Type);
				}
			}
			return (EQUIPMENT_TYPE[])Types.ToArray(typeof(EQUIPMENT_TYPE));
		}

		public bool IncludeType(bool Destination, byte Number, EQUIPMENT_TYPE Type)
		{
			ulong EquipmentTypes = 0;

			if (Number < 1 || Number > 3)
			{
				return false;
			}

			if (Destination)
			{
				EquipmentTypes = this._DestinationEquipmentTypes[Number - 1];
			}
			else
			{
				EquipmentTypes = this._SourceEquipmentTypes[Number - 1];
			}

			return ((EquipmentTypes & (ulong)(0x1 << (int)Type)) != 0) ? true : false;
		}

		public void SetEquipmentTypes(bool Destination, byte Number, EQUIPMENT_TYPE[] Types)
		{
			if (Number < 1 || Number > 3)
			{
				throw new Exception("Invalid Equipment Number");
			}

			ulong EquipmentTypes = 0;

			foreach (EQUIPMENT_TYPE Type in Types)
			{
				EquipmentTypes |= (ulong)0x1 << (int)Type;
			}

			if (Destination)
			{
			    this._DestinationEquipmentTypes[Number - 1] = EquipmentTypes;
			}
			else
			{
			    this._SourceEquipmentTypes[Number - 1] = EquipmentTypes;
			}
		}

		// vthompson CSI 5560
		/// <summary>
		/// Provides a friendly name for a specified TRANSACTION_SHOW_COMPANY_NAME item
		/// </summary>
		/// <param name="item">The TRANSACTION_SHOW_COMPANY_NAME item for which a friendly name
		/// will be returned</param>
		/// <returns>A string containing a friendly name for the specified TRANSACTION_SHOW_COMPANY_NAME</returns>
		/// <remarks>The friendly name returned was originally used for populating a drop down list</remarks>
		public static string GetShowCompanyDisplayName(TRANSACTION_SHOW_COMPANY_NAME item)
		{
			string displayName = "";

			if (item == TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY)
			{
				displayName = "Show ID Only";
			}
			else if (item == TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_ONLY)
			{
				displayName = "Show Name Only";
			}
			else if (item == TRANSACTION_SHOW_COMPANY_NAME.SHOW_NAME_AND_ID)
			{
				displayName = "Show Name and ID";
			}

			return displayName;
		}

		public static string TransactionSectionTypeID(TRANSACTION_SECTION_TYPE Type)
		{
			switch (Type)
			{
				case TRANSACTION_SECTION_TYPE.BODY:
					return "Body";
				case TRANSACTION_SECTION_TYPE.LINE_ITEMS:
					return "Line Items";
				case TRANSACTION_SECTION_TYPE.WEIGHT_READINGS:
					return "Gauge Readings";
				case TRANSACTION_SECTION_TYPE.TRANPORT_INFO:
					return "Transport Line Items";
				case TRANSACTION_SECTION_TYPE.EXPORT_RESULTS:
					return "Export Results";
				default:
					return "Undefined";
			}
		}

		/// <summary>
		/// Generates an array of standard fields in the correct display order.
		/// </summary>
		/// <param name="sectionType">The transaction section type</param>
		/// <returns>Array of standard fields in the correct display order</returns>
		public FieldClass[] DisplayOrder(TRANSACTION_SECTION_TYPE sectionType)
		{
			return this.GetOrderedFields(sectionType, false);
		}

		/// <summary>
		/// Generates an array of dispatch fields in the correct display order.
		/// </summary>
		/// <param name="sectionType">The transaction section type</param>
		/// <returns>Array of dispatch fields in the correct display order</returns>
		public FieldClass[] DispatchDisplayOrder(TRANSACTION_SECTION_TYPE sectionType)
		{
			return this.GetOrderedFields(sectionType, true);
		}

		/// <summary>
		/// Generates an array of fields in the correct display order.
		/// </summary>
		/// <param name="sectionType">The transaction section type</param>
		/// <param name="dispatchFields">Indicates whether to generate dispatch fields or standard fields</param>
		/// <returns>Array of fields in the correct display order</returns>
		public FieldClass[] GetOrderedFields(TRANSACTION_SECTION_TYPE sectionType, bool dispatchFields)
		{
			var transactionFields		= dispatchFields ? this.DispatchTransactionFields : this.TransactionFieldCollection;
			var lineItemFields			= dispatchFields ? this.DispatchLineItemFields : this.LineItemFieldCollection;
			var weightReadingFields		= dispatchFields ? this.DispatchWeightReadingFields : this.WeightReadingFieldCollection;
			var transportLineItemFields = dispatchFields ? this.DispatchTransportLineItemFields : this.TransportLineItemFieldCollection;
			var noteFields				= dispatchFields ? this.DispatchNoteFields : this.NoteFieldCollection;
			var userDataFields			= dispatchFields ? this.DispatchUserDataFields : this.UserDataFieldCollection;
			var lineItemUserDataFields	= dispatchFields ? this.DispatchLineItemUserDataFields : this.LineItemUserDataFieldCollection;
			var exportResultDataFields	= dispatchFields ? this.DispatchExportResultDetailFields : this.ExportResultDetailFieldCollection;

			FieldCollectionClass[] fieldCollections;
			FieldClass[] fields;
			int fieldCount;
			int fieldCollectionCount;

			if (sectionType == TRANSACTION_SECTION_TYPE.BODY)
			{
				fieldCount = transactionFields.Count +
								noteFields.Count +
								userDataFields.Count +
								exportResultDataFields.Count;

				fieldCollectionCount = 4;

				if (!this.MultipleLineItems)
				{
					fieldCount += lineItemFields.Count + lineItemUserDataFields.Count;
					fieldCollectionCount += 2;
				}

				if (!this.MultipleWeightReadings)
				{
					fieldCount += weightReadingFields.Count;
					fieldCollectionCount++;
				}

				if (this.multipleTransportLineItems == false)
				{
					fieldCount += transportLineItemFields.Count;
					fieldCollectionCount++;
				}

				fields = new FieldClass[fieldCount];
				fieldCollections = new FieldCollectionClass[fieldCollectionCount];
				fieldCollections[0] = transactionFields;
				fieldCollections[1] = noteFields;
				fieldCollections[2] = userDataFields;
				fieldCollections[3] = exportResultDataFields;

				int nextFieldCollectionIndex = 4;

				if (this.MultipleLineItems == false)
				{
					fieldCollections[nextFieldCollectionIndex] = lineItemFields;
					nextFieldCollectionIndex++;
					fieldCollections[nextFieldCollectionIndex] = lineItemUserDataFields;
					nextFieldCollectionIndex++;
				}

				if (this.MultipleWeightReadings == false)
				{
					fieldCollections[nextFieldCollectionIndex] = weightReadingFields;
					nextFieldCollectionIndex++;
				}

				if (this.multipleTransportLineItems == false)
				{
					fieldCollections[nextFieldCollectionIndex] = transportLineItemFields;
					nextFieldCollectionIndex++;
				}
			}

			// Line Items
			else if (sectionType == TRANSACTION_SECTION_TYPE.LINE_ITEMS)
			{
				if (this.MultipleLineItems)
				{
					fieldCount = lineItemFields.Count + lineItemUserDataFields.Count;
					fieldCollectionCount = 2;
				}
				else
				{
					fieldCount = 0;
					fieldCollectionCount = 0;
				}

				fields = new FieldClass[fieldCount];
				fieldCollections = new FieldCollectionClass[fieldCollectionCount];
				if (this.MultipleLineItems)
				{
					fieldCollections[0] = lineItemFields;
					fieldCollections[1] = lineItemUserDataFields;
				}
			}

		 // Transport Info
			else if (sectionType == TRANSACTION_SECTION_TYPE.TRANPORT_INFO)
			{
				if (this.MultipleTransportLineItems == true)
				{
					fieldCount = transportLineItemFields.Count;
					fieldCollectionCount = 1;
				}
				else
				{
					fieldCount = 0;
					fieldCollectionCount = 0;
				}

				fields = new FieldClass[fieldCount];
				fieldCollections = new FieldCollectionClass[fieldCollectionCount];

				if (this.MultipleTransportLineItems == true)
				{
					fieldCollections[0] = transportLineItemFields;
				}
			}

				// Gauge Readings
			else
			{
				if (this.MultipleWeightReadings == true)
				{
					fieldCount = weightReadingFields.Count;
					fieldCollectionCount = 1;
				}
				else
				{
					fieldCount = 0;
					fieldCollectionCount = 0;
				}

				fields = new FieldClass[fieldCount];
				fieldCollections = new FieldCollectionClass[fieldCollectionCount];

				if (this.MultipleWeightReadings == true)
				{
					fieldCollections[0] = weightReadingFields;
				}
			}

			// Load Fields Array
			int FieldIndex = 0;
			foreach (FieldCollectionClass FieldCollection in fieldCollections)
			{
				foreach (FieldClass Field in FieldCollection)
				{
					// Put the Note Fields at the end
					if (FieldCollection == this.NoteFieldCollection)
						Field.DisplayOrder = fieldCount - 1;

					// Put the Error Field at the end
					if (FieldCollection == this.ExportResultDetailFieldCollection
					&& Field.DbName == "Error")
						Field.DisplayOrder = fieldCount - 1;

					fields[FieldIndex] = Field;
					FieldIndex++;
				}
			}

			Array.Sort(fields);

			// Note: This should not be necessary but there
			// may be a defect that cause the display order
			// to not be numbered sequentially
			int displayOrder = 0;
			foreach (FieldClass field in fields)
			{
				field.DisplayOrder = displayOrder++;
			}

			return fields;
		}

		public static string TransactionTypeID(TransactionTypes Type)
		{
			switch (Type)
			{
				case TransactionTypes.T1_PrimaryAdjustment:
					return "Adjustment to Primary Storage";
				case TransactionTypes.T2_SecondaryAdjustment:
					return "Adjustment to Secondary Storage";
				case TransactionTypes.T3_PrimaryDefuel:
					return "Product Returned to Primary Storage";
				case TransactionTypes.T4_SecondaryDefuel:
					return "Product Returned to Secondary Storage";
				case TransactionTypes.T5_PrimaryDisbursement:
					return "Product Disbursed from Primary Storage";
				case TransactionTypes.T6_SecondaryDisbursement:
					return "Product Disbursed from Secondary Storage";
				case TransactionTypes.T7_FillStand:
					return "Product Movement from Primary to Secondary Storage";
				case TransactionTypes.T8_Receipt:
					return "Receipt";
				case TransactionTypes.T9_Request:
					return "Request";
				case TransactionTypes.T10_Unload:
					return "Product Movement from Secondary to Primary Storage";
				case TransactionTypes.T11_ConsumerTransfer:
					return "Product Transfer from Consumer to Consumer";
				case TransactionTypes.T12_InventoryNotAffected:
					return "No Effect to Primary or Secondary Storage";
				case TransactionTypes.T13_OwnerTransfer:
					return "Product Transfer from Owner to Owner or Storage to Storage";
				case TransactionTypes.T14_PhysicalInventory:
					return "Physical Inventory";
				case TransactionTypes.T15_PrimaryRegrade:
					return "Product Regrade affecting Primary Storage";
				case TransactionTypes.T16_SecondaryRegrade:
					return "Product Regrade affecting Secondary Storage";
				case TransactionTypes.T17_Order:
					return "Order";
				case TransactionTypes.T18_SupplyOrder:
					return "Supply Order";
				case TransactionTypes.T21_AccountPayableInvoice:
					return "Accounts Payable Invoice";
				case TransactionTypes.T22_AccountReceivableInvoice:
					return "Accounts Receivable Invoice";
				case TransactionTypes.T23_StorageTransfer:
					return "Transfer to Different Storage or Equipment";
				case TransactionTypes.T25_Shipment:
					return "Product Shipped from Primary Storage";
				default:
					return "Undefined";
			}
		}

		public bool IsProductExcluded(Guid productGuid)
		{
			foreach (ProductMapClass ExcludedProduct in this.ExcludedProductCollection)
			{
				if (ExcludedProduct.AssignedGuid == productGuid)
				{
					return true;
				}
			}

			return false;
		}

		public EngineeringUnit GetUnits(SITE_VARIABLE_TYPE Type)
		{
			switch (Type)
			{
				case SITE_VARIABLE_TYPE.LENGTH:
					return this.LevelUnits;

				case SITE_VARIABLE_TYPE.TEMPERATURE:
					return this.TemperatureUnits;

				case SITE_VARIABLE_TYPE.DENSITY:
					return this.DensityUnits;

				case SITE_VARIABLE_TYPE.PRESSURE:
					return this.PressureUnits;

				case SITE_VARIABLE_TYPE.FLOW:
					return this.FlowUnits;

				case SITE_VARIABLE_TYPE.VOLUME:
					return this.VolumeUnits;

				case SITE_VARIABLE_TYPE.MASS:
					return this.MassUnits;

				case SITE_VARIABLE_TYPE.ADDITIVE_VOLUME:
					return this.AdditiveVolumeUnits;

				case SITE_VARIABLE_TYPE.VCF:
					return EngineeringUnit.FmduPCent;


				default:
					return EngineeringUnit.FmduPCent;
			}
		}

		public byte GetDecimalPlaces(SITE_VARIABLE_TYPE type)
		{
			switch (type)
			{
				case SITE_VARIABLE_TYPE.LENGTH:
					return this._LevelDecimalPlaces;

				case SITE_VARIABLE_TYPE.TEMPERATURE:
					return this._TemperatureDecimalPlaces;

				case SITE_VARIABLE_TYPE.DENSITY:
					return this._DensityDecimalPlaces;

				case SITE_VARIABLE_TYPE.PRESSURE:
					return this._PressureDecimalPlaces;

				case SITE_VARIABLE_TYPE.VOLUME:
					return this._VolumeDecimalPlaces;

				case SITE_VARIABLE_TYPE.MASS:
					return this._MassDecimalPlaces;

				case SITE_VARIABLE_TYPE.ADDITIVE_VOLUME:
					return this._AdditiveVolumeDecimalPlaces;

				case SITE_VARIABLE_TYPE.VCF:
					return 4;

				default:
					return 2;
			}
		}

		/// <summary>
		/// This method will just load the transaction alias index.
		/// </summary>
		/// <param name="Set"></param>
		public void LoadIdentityGuid(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set");
			}

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];
		    this._IdentityGuid = DataObject.getValue<Guid>(Row["TransactionAliasGuid"], Guid.Empty);
		}

		/// <summary>
		/// This method will load the Tx alias information.
		/// </summary>
		/// <param name="Set"></param>
		public void Load(DataSet Set)
		{
			if (Set == null)
			{
				throw new ArgumentNullException("Set");
			}

			this.Reset();

			DataTable Table = Set.Tables[0];
			if (Table.Rows.Count == 0)
			{
				return;
			}

			DataRow Row = Table.Rows[0];

		    this._IdentityGuid = DataObject.getValue<Guid>(Row["TransactionAliasGuid"], Guid.Empty);
		    this._MasterRecordGuid = DataObject.getValue<Guid>(Row["_MasterRecordGuid"], Guid.Empty);
		    this._TransTypeID = DataObject.getValue<TransactionTypes>(Row["LookupTransTypeIndex"], TransactionTypes.T_Maximum);
		    this._ID = DataObject.getValue<string>(Row["AliasName"], "");
		    this._SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
		    this._MeterCloseout = DataObject.getValue<bool>(Row["MeterCloseout"], false);
		    this._BulkShipment = DataObject.getValue<bool>(Row["BulkShipment"], false);
		    this._DistributedImpact = DataObject.getValue<bool>(Row["DistributedImpact"], false);
		    this._MultipleLineItems = DataObject.getValue<bool>(Row["MultipleLineItems"], false);
		    this._LineItemEditControl = DataObject.getValue<bool>(Row["LineItemEditControl"], false);
		    this._MultipleWeightReadings = DataObject.getValue<bool>(Row["MultipleWeightReadings"], false);
			this.multipleTransportLineItems = DataObject.getValue<bool>(Row["MultipleTransportLineItems"], false);
		    this._LimitSelectionsBasedOnHierarchy = DataObject.getValue<bool>(Row["LimitSelectionsBasedOnHierarchy"], false);
		    this._WeightReadingEditControl = DataObject.getValue<bool>(Row["WeightReadingEditControl"], false);
		    this._AssociatedTransactionAliasGuid = DataObject.getValue<Guid>(Row["AssociatedTransactionAliasGuid"], Guid.Empty);
		    this.AssociatedReport = DataObject.getValue<string>(Row["AssociatedReport"], "");
		    this.AssociatedPreloadReport = DataObject.getValue<string>(Row["AssociatedPreloadReport"], "");
		    this._DestinationEquipmentTypes[0] = (ulong)DataObject.getValue<long>(Row["DestinationEquipmentTypes1"], 0);
		    this._DestinationEquipmentTypes[1] = (ulong)DataObject.getValue<long>(Row["DestinationEquipmentTypes2"], 0);
		    this._DestinationEquipmentTypes[2] = (ulong)DataObject.getValue<long>(Row["DestinationEquipmentTypes3"], 0);
		    this._SourceEquipmentTypes[0] = (ulong)DataObject.getValue<long>(Row["SourceEquipmentTypes1"], 0);
		    this._SourceEquipmentTypes[1] = (ulong)DataObject.getValue<long>(Row["SourceEquipmentTypes2"], 0);
		    this._SourceEquipmentTypes[2] = (ulong)DataObject.getValue<long>(Row["SourceEquipmentTypes3"], 0);
		    this._CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
		    this._CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
		    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this._CreatedDate);
		    this._UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);

		    this._showCompanyName = DataObject.getValue<TRANSACTION_SHOW_COMPANY_NAME>(Row["ShowCompanyName"], TRANSACTION_SHOW_COMPANY_NAME.SHOW_ID_ONLY);
		    this.AssociatedAlias = DataObject.getValue<string>(Row["AssociatedAlias"], "{None}");

		    this.aggregateAssociatedTransactions = DataObject.getValue<bool>(Row["AggregateAssocTrans"], false);

			this.enableQtyToleranceExceededWarning = DataObject.getValue<bool>(Row["EnableQuantityToleranceExceededWarning"], false);
			this.enableTotalQtyExceededWarning = DataObject.getValue<bool>(Row["EnableTotalQuantityExceededWarning"], false);
			this.enableTotalValueExceededWarning = DataObject.getValue<bool>(Row["EnableTotalValueExceededWarning"], false);
			this.enableValueToleranceExceededWarning = DataObject.getValue<bool>(Row["EnableValueToleranceExceededWarning"], false);

			this.LookupDefaultStatusIndex = DataObject.getValue<int>(Row["LookupDefaultStatusIndex"], -1);

			// Units
		    this._LevelUnits = DataObject.getValue<EngineeringUnit>(Row["LevelUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._TemperatureUnits = DataObject.getValue<EngineeringUnit>(Row["TemperatureUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._DensityUnits = DataObject.getValue<EngineeringUnit>(Row["DensityUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._PressureUnits = DataObject.getValue<EngineeringUnit>(Row["PressureUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._FlowUnits = DataObject.getValue<EngineeringUnit>(Row["FlowUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._VolumeUnits = DataObject.getValue<EngineeringUnit>(Row["VolumeUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._MassUnits = DataObject.getValue<EngineeringUnit>(Row["MassUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._AdditiveVolumeUnits = DataObject.getValue<EngineeringUnit>(Row["AdditiveVolumeUnitIndex"], EngineeringUnit.FmSiteUnits);
		    this._LevelDecimalPlaces = DataObject.getValue<byte>(Row["LevelDecimalPlaces"], 2);
		    this._TemperatureDecimalPlaces = DataObject.getValue<byte>(Row["TemperatureDecimalPlaces"], 0);
		    this._DensityDecimalPlaces = DataObject.getValue<byte>(Row["DensityDecimalPlaces"], 1);
		    this._PressureDecimalPlaces = DataObject.getValue<byte>(Row["PressureDecimalPlaces"], 2);
		    this._FlowDecimalPlaces = DataObject.getValue<byte>(Row["FlowDecimalPlaces"], 1);
		    this._VolumeDecimalPlaces = DataObject.getValue<byte>(Row["VolumeDecimalPlaces"], 0);
		    this._MassDecimalPlaces = DataObject.getValue<byte>(Row["MassDecimalPlaces"], 0);
		    this._AdditiveVolumeDecimalPlaces = DataObject.getValue<byte>(Row["AdditiveVolumeDecimalPlaces"], 0);
		    this._UseComboBoxControls = DataObject.getValue<bool>(Row["UseComboBoxControls"], false);
		    this._IncludeInDispatch = DataObject.getValue<bool>(Row["IncludeInDispatch"], false);

		    this.EnableAutoCompleteControls = (!Row.IsNull( "EnableAutoCompleteControls" )) && (bool)Row["EnableAutoCompleteControls"];
		    this.PermitNonReferenceData = (!Row.IsNull("PermitNonReferenceData")) && (bool)Row["PermitNonReferenceData"];
			this.UseTransactionDetailWithLayout = (!Row.IsNull("UseTransactionDetailWithLayout")) && (bool)Row["UseTransactionDetailWithLayout"];
			this.DefaultMeterToEquipmentID = (!Row.IsNull("DefaultMeterToEquipmentID")) && (bool)Row["DefaultMeterToEquipmentID"];
			this.LimitSourceEquipmentByProduct = (!Row.IsNull("LimitSourceEquipmentByProduct")) && (bool)Row["LimitSourceEquipmentByProduct"];
			this.RememberMeterEndForMeterID = (!Row.IsNull("RememberMeterEndForMeterID")) && (bool)Row["RememberMeterEndForMeterID"];
			this.PopulateCompaniesFromEquipment = (!Row.IsNull("PopulateCompaniesFromEquipment")) && (bool)Row["PopulateCompaniesFromEquipment"];
			this.PopulateGrossVolumeFromMeterValues = (!Row.IsNull("PopulateGrossVolumeFromMeterValues")) && (bool)Row["PopulateGrossVolumeFromMeterValues"];
			this.UseMeterAndCompressionFactorFromMeter = (!Row.IsNull("UseMeterAndCompressionFactorFromMeter")) && (bool)Row["UseMeterAndCompressionFactorFromMeter"];

			if (Table.Columns.IndexOf("ASSIGNEDTOSITEGUID") >= 0) this.AssignedToSiteGuid = DataObject.getValue<Guid>(Row["ASSIGNEDTOSITEGUID"], Guid.Empty);
            if (Table.Columns.IndexOf("ASSIGNEDFROMSITEGUID") >= 0) this.AssignedFromSiteGuid = DataObject.getValue<Guid>(Row["ASSIGNEDFROMSITEGUID"], Guid.Empty);
            if (Table.Columns.IndexOf("ASSIGNEDFROMSITEID") >= 0) this.AssignedFromSiteId = DataObject.getValue<string>(Row["ASSIGNEDFROMSITEID"], "");
		}


		/// <summary>
		/// This method retrieves TransactionAliases that have the IncludeInDispatch boolean value set or disabled.
		/// </summary>
		/// <param name="cmd"></param>
		/// <param name="security"></param>
		/// <param name="bInTransaction"></param>
		/// <param name="isIncludedInDispatch"></param>
		public void SelectByIncludeInDispatchSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction, bool isIncludedInDispatch)
		{
			cmd.CommandText = this.SelectClause +
						" FROM tblTransactionAliases " + SQLUpdateLock(bInTransaction) +
						" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblTransactionAliases", "TransactionAliasGuid") +
						" AND tblTransactionAliases.IncludeInDispatch = @IncludeInDispatch";
			var bitValue = (isIncludedInDispatch) ? "1" : "0";
			cmd.Parameters.AddWithValue("@IncludeInDispatch", bitValue);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// This method will populate the SQL command to retrieve the transaction alias based on
		/// the alias name.
		/// </summary>
		/// <param name="command">SQL command to populate.</param>
		/// <param name="security">The security object.</param>
		/// <param name="inTransaction">Whether the query should be in a DB transaction.</param>
		public void SelectByIdSql(SqlCommand command, SecurityClass security, bool inTransaction)
		{
			command.CommandText = this.SelectClause +
					" FROM tblTransactionAliases " + SQLUpdateLock(inTransaction) +
					" LEFT OUTER JOIN tblTransactionAliases A ON A.TransactionAliasGuid = tblTransactionAliases.AssociatedTransactionAliasGuid " +
					" WHERE " + this.AppendSiteWhereClause(command, security, "tblTransactionAliases", "TransactionAliasGuid") +
					" AND tblTransactionAliases.AliasName = @AliasName ";

			command.Parameters.AddWithValue("@AliasName", this.ID);
		}

		/// <summary>
		/// This method will populate the SQL command to retrieve the transaction alias based on
		/// the transaction alias GUID.
		/// </summary>
		/// <param name="command">SQL command to populate.</param>
		/// <param name="inTransaction">Whether the query should be in a DB transaction.</param>
		public void SelectSQL(SqlCommand command, bool inTransaction)
		{
			command.CommandText = this.SelectClause +
				" FROM tblTransactionAliases " + SQLUpdateLock(inTransaction) +
				" LEFT OUTER JOIN tblTransactionAliases A ON A.TransactionAliasGuid = tblTransactionAliases.AssociatedTransactionAliasGuid " +
				" WHERE tblTransactionAliases.TransactionAliasGuid = @TransactionAliasGuid ";

			command.Parameters.AddWithValue("@TransactionAliasGuid", this.IdentityGuid);
		}

		public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = this.SelectClause +
					" FROM tblTransactionAliases" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblTransactionAliases", "TransactionAliasGuid") +
					" ORDER BY tblTransactionAliases.AliasName";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// Populates the SQL command text used to get a list of transaction status codes that are associated
		/// with transaction aliases that have the "IncludeInDispatch" flag set to true.
		/// </summary>
		/// <param name="cmd">The SQL command object to populate</param>
		/// <param name="security">The security object</param>
		public void EnumerateDispatchStatusesSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = "SELECT DISTINCT lookup.tblTransactionStatus.TransactionStatusIndex," +
												"lookup.tblTransactionStatus.TransactionStatusCode" +
					" FROM tblTransactionAliases" +
					" INNER JOIN map.tblTransactionAliasToStatus" +
					" ON tblTransactionAliases.TransactionAliasGuid = map.tblTransactionAliasToStatus.TransactionAliasGuid" +
					" INNER JOIN lookup.tblTransactionStatus" +
					" ON map.tblTransactionAliasToStatus.LookupTransactionStatusIndex = lookup.tblTransactionStatus.TransactionStatusIndex" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblTransactionAliases", "TransactionAliasGuid") +
					" AND tblTransactionAliases.IncludeInDispatch = 1" +
					" ORDER BY lookup.tblTransactionStatus.TransactionStatusIndex";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		public void EnumerateByTransTypeIDSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = this.SelectClause +
					" FROM tblTransactionAliases" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblTransactionAliases", "TransactionAliasGuid") +
					" AND tblTransactionAliases.LookupTransTypeIndex = @LookupTransTypeIndex" +
					" ORDER BY tblTransactionAliases.AliasName";

			cmd.Parameters.AddWithValue("@LookupTransTypeIndex", ((int)this.TransTypeID));
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}


		public void EnumerateAliasNameByTransTypeID(SqlCommand cmd, SecurityClass security, TransactionTypes Type)
		{
			cmd.CommandText = "SELECT tblTransactionAliases.AliasName FROM tblTransactionAliases" +
					" WHERE" + this.AppendSiteWhereClause(cmd, security, "tblTransactionAliases", "TransactionAliasGuid") +
					" AND tblTransactionAliases.LookupTransTypeIndex = @LookupTransTypeIndex";

			cmd.Parameters.AddWithValue("@LookupTransTypeIndex", ((int)Type));
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// Returns the SQL that will assign the passed status to the transaction alias
		/// </summary>
		/// <param name="status">The int value representing a Transaction Status</param>
		/// <returns>SQL used to assign the passed status to the transaction alias</returns>
		public void GetInsertAvailableStatusSQL(SqlCommand cmd, int status, SecurityClass security)
		{
			cmd.CommandText = "INSERT INTO map.tblTransactionAliasToStatus (TransactionAliasGuid, LookupTransactionStatusIndex, CreatedBy, CreatedDate) " +
						 "VALUES (@TransactionAliasGuid, @LookupTransactionStatusIndex, @CreatedBy, SYSDATETIMEOFFSET())";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.IdentityGuid);
			cmd.Parameters.AddWithValue("@LookupTransactionStatusIndex", status);
			cmd.Parameters.AddWithValue("@CreatedBy", security.UserID);
		}

		public void GetDeleteAvailableStatusSQL(SqlCommand cmd, int status, SecurityClass security)
		{
			cmd.CommandText =
				"DELETE FROM map.tblTransactionAliasToStatus WHERE TransactionAliasGuid = @TransactionAliasGuid AND LookupTransactionStatusIndex = @LookupTransactionStatusIndex";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.IdentityGuid);
			cmd.Parameters.AddWithValue("@LookupTransactionStatusIndex", status);
		}

		/// <summary>
		/// Get the count of meter closeout transaction aliases. Meter closeout transaction aliases must be type 12 transactions
		/// </summary>
		/// <param name="cmd">A SQL command object to populate</param>
		/// <param name="security">Contains security information</param>
		public void GetMeterCloseoutAliasCountSQL(SqlCommand cmd, SecurityClass security)
		{
			cmd.CommandText = " SELECT COUNT(*) FROM tblTransactionAliases" +
				" WHERE" + this.AppendSiteWhereClause( cmd, security, "tblTransactionAliases", "TransactionAliasGuid" ) +
				" AND MeterCloseout = '1' " +
				" AND tblTransactionAliases.LookupTransTypeIndex = @LookupTransTypeIndex";

			cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);
			cmd.Parameters.AddWithValue("@LookupTransTypeIndex", (int)TransactionTypes.T12_InventoryNotAffected);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// Returns SQL used to retrieve assigned statuses for the transaction alias
		/// </summary>
        public void SelectAssignedStatusesSQL(SqlCommand cmd, bool bInTransaction, SecurityClass security)
		{
			cmd.CommandText = "SELECT TransactionAliasGuid, LookupTransactionStatusIndex " +
						 "FROM map.tblTransactionAliasToStatus " + SQLUpdateLock(bInTransaction) +
                         " WHERE TransactionAliasGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Transaction_Alias', @TransactionAliasGuid, @TargetSiteGuid)";

			cmd.Parameters.AddWithValue("@TransactionAliasGuid", this.IdentityGuid);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// Populates the transaction alias' available statuses
		/// </summary>
		/// <param name="ds">The dataset used to populate the statuses</param>
		public void LoadAssignedStatuses(DataSet ds)
		{
			DataTable dt = ds.Tables[0];
		    this.assignedStatuses.Clear();

			foreach (DataRow dr in dt.Rows)
			{
			    this.assignedStatuses.Add(DataObject.getValue<int>(dr["LookupTransactionStatusIndex"], 0));
			}
		}

		/// <summary>
		/// Returns the SQL used to insert a new associated transaction alias
		/// </summary>
		/// <param name="alias">The alias to be associated with this alias</param>
		/// <param name="security">Contains user credentials</param>
		/// <returns>SQL string</returns>
		public void GetInsertAssociatedAliasesSQL(SqlCommand cmd, TransactionAliasClass alias, SecurityClass security)
		{
			cmd.CommandText = "INSERT INTO map.tblAssociatedTransactionAliases " +
						 "(ParentTransactionAliasGuid, ChildTransactionAliasGuid, CreatedBy, CreatedDate)" +
						 "VALUES (@ParentTransactionAliasGuid, @ChildTransactionAliasGuid, @CreatedBy, SYSDATETIMEOFFSET())";

			cmd.Parameters.AddWithValue("@ParentTransactionAliasGuid", this.IdentityGuid);
			cmd.Parameters.AddWithValue("@ChildTransactionAliasGuid", alias.IdentityGuid);
			cmd.Parameters.AddWithValue("@CreatedBy", security.UserID);
		}
		#endregion


		#region Override methods
		/// <summary>
		/// This method will initialize the object to its initial state.
		/// </summary>
		public override void Reset()
		{
			base.Reset();

		    this._TransTypeID = TransactionTypes.T_Maximum;
		    this._MeterCloseout = false;
		    this._BulkShipment = false;
		    this._DistributedImpact = false;
		    this._MultipleLineItems = false;
		    this._LineItemEditControl = false;
		    this._MultipleWeightReadings = false;
			this.multipleTransportLineItems = false;
		    this._LimitSelectionsBasedOnHierarchy = false;
		    this._WeightReadingEditControl = false;
		    this.ExcludedProductCollection = new ProductMapCollectionClass();
			this.UserDataFieldCollection = new UserDataFieldCollectionClass();
			this.LineItemUserDataFieldCollection = new UserDataFieldCollectionClass();
			this.TransactionFieldCollection = new TransactionAliasFieldCollectionClass();
			this.LineItemFieldCollection = new TransactionAliasFieldCollectionClass();
			this.WeightReadingFieldCollection = new TransactionAliasFieldCollectionClass();
			this.TransportLineItemFieldCollection = new TransactionAliasFieldCollectionClass();
			this.NoteFieldCollection = new TransactionAliasFieldCollectionClass();
			this.ExportResultDetailFieldCollection = new TransactionAliasFieldCollectionClass();
			this.DispatchUserDataFields = new UserDataFieldCollectionClass();
			this.DispatchLineItemUserDataFields = new UserDataFieldCollectionClass();
			this.DispatchTransactionFields = new TransactionAliasFieldCollectionClass();
			this.DispatchLineItemFields = new TransactionAliasFieldCollectionClass();
			this.DispatchWeightReadingFields = new TransactionAliasFieldCollectionClass();
			this.DispatchTransportLineItemFields = new TransactionAliasFieldCollectionClass();
			this.DispatchNoteFields = new TransactionAliasFieldCollectionClass();
			this.DispatchExportResultDetailFields = new TransactionAliasFieldCollectionClass();
		    this.GroupTransactionAliasMapCollection = new GroupTransactionAliasMapCollectionClass();
		    this._AssociatedTransactionAliasGuid = Guid.Empty;
		    this.AssociatedReport = "";
		    this.AssociatedPreloadReport = "";
		    this._DestinationEquipmentTypes[0] = 0;
		    this._DestinationEquipmentTypes[1] = 0;
		    this._DestinationEquipmentTypes[2] = 0;
		    this._SourceEquipmentTypes[0] = 0;
		    this._SourceEquipmentTypes[1] = 0;
		    this._SourceEquipmentTypes[2] = 0;
		    this.AssociatedAlias = "{None}";
			this.associatedAliases = new TransactionAliasCollectionClass();
			this.aggregateAssociatedTransactions = false;
			this.enableQtyToleranceExceededWarning = false;
			this.enableTotalQtyExceededWarning = false;
			this.enableTotalValueExceededWarning = false;
			this.enableValueToleranceExceededWarning = false;
			this.LookupDefaultStatusIndex = -1;
			// Units
		    this._LevelUnits = EngineeringUnit.FmSiteUnits;
		    this._TemperatureUnits = EngineeringUnit.FmSiteUnits;
		    this._DensityUnits = EngineeringUnit.FmSiteUnits;
		    this._PressureUnits = EngineeringUnit.FmSiteUnits;
		    this._FlowUnits = EngineeringUnit.FmSiteUnits;
		    this._VolumeUnits = EngineeringUnit.FmSiteUnits;
		    this._MassUnits = EngineeringUnit.FmSiteUnits;
		    this._AdditiveVolumeUnits = EngineeringUnit.FmSiteUnits;
		    this._LevelDecimalPlaces = 2;
		    this._TemperatureDecimalPlaces = 0;
		    this._DensityDecimalPlaces = 1;
		    this._PressureDecimalPlaces = 2;
		    this._FlowDecimalPlaces = 1;
		    this._VolumeDecimalPlaces = 0;
		    this._MassDecimalPlaces = 0;
		    this._AdditiveVolumeDecimalPlaces = 0;
		    this._UseComboBoxControls = false;
		    this._IncludeInDispatch = false;

			this.EnableAutoCompleteControls = false;
			this.PermitNonReferenceData = false;
			this.UseTransactionDetailWithLayout = false;
			this.DefaultMeterToEquipmentID = false;
			this.LimitSourceEquipmentByProduct = false;
			this.RememberMeterEndForMeterID = false;
			this.PopulateCompaniesFromEquipment = false;
			this.PopulateGrossVolumeFromMeterValues = false;
			this.UseMeterAndCompressionFactorFromMeter = false;
		}


        /// <summary>
        /// Override the AppendSiteWhereClause to support TransactionAlias RecordVersioning
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="security"></param>
        /// <param name="entityTable"></param>
        /// <param name="entityGuidColumn"></param>
        public override string AppendSiteWhereClause(SqlCommand cmd, SecurityClass security, string entityTable, string entityGuidColumn)
        {
            string SQL = "";
            SQL = " (" + entityTable + "." + entityGuidColumn + " IN (SELECT " + entityGuidColumn + " FROM [erv].[udf_GetTransactionAliasRecordVersions](@TargetSiteGuid)" + "))";
            return SQL;
        }



		#endregion
	}
	#endregion
}
