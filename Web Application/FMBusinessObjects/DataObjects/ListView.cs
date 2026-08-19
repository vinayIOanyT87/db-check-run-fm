namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    #region Public enumerations
	public enum LISTVIEW_TYPE
	{
		TRANSACTION_LIST = 1,
		STANDARD = 2,
		AGGREGATE = 3,
		TYPE_MAX = 4
	}

	public enum LISTVIEW_STANDARD_TYPE
	{
		LEDGER = 1,
		METER_RECONCILIATION_SUMMARY = 2,
		RECEIPT_RECONCILIATION = 3,
		INVENTORY_RECONCILIATION = 4,
		CLOSEOUT = 5,
		EQUIPMENT_TRANSACTION = 6,
		RECEIPT_ASSIGNMENT_ASSIGNED = 7,
		RECEIPT_ASSIGNMENT_AVAILABLE = 8,
		AUTOMATIC_PHYSICAL_INVENTORY = 10,
		ORDER = 11,
		ORDER_ASSOCIATED_TX = 12,
		BOL_SUMMARY = 13,
		SUPPLY_ORDER = 14,
		SUPPLY_ORDER_ASSOCIATED_TX = 15,
		INVOICE = 16,
		ASSOCIATED_TX = 17,
		RECOVERY = 18,
		RECOVERY_ASSOCIATED_TX = 19,
		BULK_ASSOCIATED_TX = 20,
		METER_RECONCILIATION_DETAIL = 21,
		AUTO_DISTRIBUTION_RULE = 22,
		TYPE_MAX = 23
	}
	#endregion

	#region List View Collection Class
	/// <summary>
	/// Summary description for ListViewCollectionClass.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class ListViewCollectionClass : List<ListViewClass>
	{
		public void RemoveByIdentityGuid(ListViewClass listView)
		{
			int idx = 0;
			foreach (ListViewClass item in this)
			{
				if (item.IdentityGuid == listView.IdentityGuid)
				{
					this.RemoveAt(idx);
					return;
				}
				idx++;
			}
		}
	}
	#endregion

	/// <summary>
	/// Summary description for ListViewClass.
	/// </summary>
	[Serializable]
	[DataContract]
	[KnownType(typeof(ProductMapCollectionClass))]
	[KnownType(typeof(GroupLedgerViewMapCollectionClass))]
	[KnownType(typeof(ListViewFieldCollectionClass))]
	public class ListViewClass : BaseDataObject
	{
		#region Private constants and fields
		private const string AutoDistributionRulesName = "Automatic Distribution Rules";
		#endregion Private constants and fields

		[DataMember]
		protected LISTVIEW_TYPE _Type;
		[DataMember]
		protected Guid _TypeGuid;
		[DataMember]
		protected LISTVIEW_STANDARD_TYPE _ListViewStandardType;
		[DataMember]
		public ListViewFieldCollectionClass ListViewFieldCollection;

		// Ledger view specific
		[DataMember]
		public ProductMapCollectionClass ProductMapCollection;
		[DataMember]
		public GroupLedgerViewMapCollectionClass GroupMapCollection;

		public string ProductList
		{
			get
			{
				string returnText = string.Empty;

				foreach (ProductMapClass productMap in this.ProductMapCollection)
				{
					returnText += ", " + productMap.AssignedID;
				}

				if (returnText.Length > 0)
				{
					returnText = returnText.Substring(1);
				}

				return returnText;
			}
		}

		public string UserGroupList
		{
			get
			{
				string returnText = string.Empty;

				foreach (GroupLedgerViewMapClass groupMap in this.GroupMapCollection)
				{
					returnText += ", " + groupMap.ID;
				}

				if (returnText.Length > 0)
				{
					returnText = returnText.Substring(1);
				}

				return returnText;
			}
		}

		public override string ID
		{
			get
			{
				if (this._Type == LISTVIEW_TYPE.STANDARD && this.ListViewStandardType != LISTVIEW_STANDARD_TYPE.LEDGER)
				{
					return ListViewStandardTypeID(this.ListViewStandardType);
				}
				else
				{
					return this._ID;
				}
			}
		    set
		    {
                this.SetString("ID", 50, value, ref this._ID);
            }
		}

		public LISTVIEW_TYPE Type
		{
			get { return this._Type; }
			set {
			    this._Type = value; }
		}

		public Guid TypeGuid
		{
			get { return this._TypeGuid; }
			set {
			    this._TypeGuid = value;
			    this._ListViewStandardType = GetStandardTypeFromGuid(this._TypeGuid); }
		}

		public LISTVIEW_STANDARD_TYPE ListViewStandardType
		{
			get { return this._ListViewStandardType; }
			set {
			    this._ListViewStandardType = value;
			    this._ID = this.ID; }
		}


		private const string ListviewStandardTypeGuidPrefix = "20000000-0000-0000-0000-000000000";
		public static LISTVIEW_STANDARD_TYPE GetStandardTypeFromGuid(Guid standardTypeGuid)
		{
			LISTVIEW_STANDARD_TYPE standardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;
			string guidString = standardTypeGuid.ToString();
			string enumSuffix = guidString.Substring(guidString.Length - 3);
			string prefix = guidString.Substring(0, guidString.Length - 3);
			if (prefix == ListviewStandardTypeGuidPrefix)
			{
				standardType = (LISTVIEW_STANDARD_TYPE)Convert.ToInt32(enumSuffix);
			}
			return standardType;
		}

		public static Guid GetGuidFromStandardType(LISTVIEW_STANDARD_TYPE standardType)
		{
		    string prefix = ListviewStandardTypeGuidPrefix;
			string enumSuffix = ((int)standardType).ToString("D3");
			var standardTypeGuid = Guid.Parse(prefix + enumSuffix);
			return standardTypeGuid;
		}

		public static STANDARD_FIELD_TYPE[] GetStandardViewFields(LISTVIEW_STANDARD_TYPE listViewStandardType)
		{
			switch (listViewStandardType)
			{
				case LISTVIEW_STANDARD_TYPE.LEDGER:
					{
						STANDARD_FIELD_TYPE[] fields = {STANDARD_FIELD_TYPE.BEGIN_INVENTORY,
														STANDARD_FIELD_TYPE.BOOK_INVENTORY,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.TOTAL_PHYSICAL_INVENTORY,
														STANDARD_FIELD_TYPE.VARIANCE,
														STANDARD_FIELD_TYPE.TOTAL_VARIANCE
					};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_SUMMARY:
					{
						STANDARD_FIELD_TYPE[] fields = {	
															STANDARD_FIELD_TYPE.ASSET_ID,
															STANDARD_FIELD_TYPE.METER_ID,
															STANDARD_FIELD_TYPE.ROTATES_BACKWARDS,
															STANDARD_FIELD_TYPE.METER_START,
															STANDARD_FIELD_TYPE.METER_STOP,
															STANDARD_FIELD_TYPE.METER_TOTAL,
															STANDARD_FIELD_TYPE.TRANSACTION_METER_TOTAL,
															STANDARD_FIELD_TYPE.METER_VARIANCE,
															STANDARD_FIELD_TYPE.TRANSACTION_VOLUME_TOTAL,
															STANDARD_FIELD_TYPE.VOLUME_VARIANCE,
															STANDARD_FIELD_TYPE.PRODUCT,
															STANDARD_FIELD_TYPE.BOL_CARRIER,
															STANDARD_FIELD_TYPE.METER_RECONCILIATION_ERROR
														};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_DETAIL:
					{
						STANDARD_FIELD_TYPE[] fields = {	
															STANDARD_FIELD_TYPE.TRANSACTION_ID,
															STANDARD_FIELD_TYPE.PRODUCT,
															STANDARD_FIELD_TYPE.METER_START,
															STANDARD_FIELD_TYPE.METER_STOP,
															STANDARD_FIELD_TYPE.METER_TOTAL,
															STANDARD_FIELD_TYPE.VOLUME,
															STANDARD_FIELD_TYPE.METER_SKIP,
															STANDARD_FIELD_TYPE.BOL_CARRIER,
															STANDARD_FIELD_TYPE.STATION,
															STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
															STANDARD_FIELD_TYPE.FLIGHT_NUMBER,
															STANDARD_FIELD_TYPE.TICKET_NUMBER
														};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.RECEIPT_RECONCILIATION:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.TRANSACTION_ID,
																STANDARD_FIELD_TYPE.BILLED_VOLUME,
																STANDARD_FIELD_TYPE.BILLED_VOLUME,
																STANDARD_FIELD_TYPE.MEASURED_VOLUME,
																STANDARD_FIELD_TYPE.BILL_OF_LADING_NUMBER,
																STANDARD_FIELD_TYPE.BOOK_RECEIPTS,
																STANDARD_FIELD_TYPE.ASSIGNED,
																STANDARD_FIELD_TYPE.REMAINING
															};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.INVENTORY_RECONCILIATION:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.INVENTORY_DATE,
																STANDARD_FIELD_TYPE.BEGIN_INVENTORY,
																STANDARD_FIELD_TYPE.BOOK_INVENTORY,
																STANDARD_FIELD_TYPE.TOTAL_PHYSICAL_INVENTORY,
																STANDARD_FIELD_TYPE.VARIANCE,
																STANDARD_FIELD_TYPE.TOTAL_VARIANCE,
																STANDARD_FIELD_TYPE.TOTAL_ACTIVITY,
																STANDARD_FIELD_TYPE.TOLERANCE,
																STANDARD_FIELD_TYPE.ALLOWED_GAIN_LOSS,
																STANDARD_FIELD_TYPE.VARIANCE_PERCENTAGE
															};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.CLOSEOUT:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.CLOSEOUT_DATE,
																STANDARD_FIELD_TYPE.BOOK_INVENTORY,
																STANDARD_FIELD_TYPE.TOTAL_PHYSICAL_INVENTORY,
																STANDARD_FIELD_TYPE.VARIANCE,
																STANDARD_FIELD_TYPE.TOTAL_VARIANCE
															};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.EQUIPMENT_TRANSACTION:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.METER_START,
																STANDARD_FIELD_TYPE.METER_STOP,
																STANDARD_FIELD_TYPE.VOLUME,
																STANDARD_FIELD_TYPE.TRANSACTION_TYPE,
																STANDARD_FIELD_TYPE.TRANSACTION_ID,
																STANDARD_FIELD_TYPE.DIFFERENTIAL,
																STANDARD_FIELD_TYPE.VARIANCE,		
					};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.RECEIPT_ASSIGNMENT_ASSIGNED:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.TRANSACTION_ID,
																STANDARD_FIELD_TYPE.OWNER,
																STANDARD_FIELD_TYPE.VOLUME
															};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.RECEIPT_ASSIGNMENT_AVAILABLE:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.TRANSACTION_ID,
																STANDARD_FIELD_TYPE.OWNER,
																STANDARD_FIELD_TYPE.VOLUME
															};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.AUTOMATIC_PHYSICAL_INVENTORY:
					{
						STANDARD_FIELD_TYPE[] fields ={	STANDARD_FIELD_TYPE.LOCATION,
																STANDARD_FIELD_TYPE.VOLUME,
																STANDARD_FIELD_TYPE.TEMPERATURE,
																STANDARD_FIELD_TYPE.DENSITY,
																STANDARD_FIELD_TYPE.INVENTORY_DATE
															};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.ORDER:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.TRANSACTION_DATE,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
														STANDARD_FIELD_TYPE.TRANSACTION_ID,
														STANDARD_FIELD_TYPE.DOCUMENT_NUMBER,
														STANDARD_FIELD_TYPE.PO_NUMBER,
														STANDARD_FIELD_TYPE.ORDER_STATUS,
														STANDARD_FIELD_TYPE.SCHEDULED_DATE,
														STANDARD_FIELD_TYPE.EFFECTIVE_DATE,
														STANDARD_FIELD_TYPE.EXPIRATION_DATE,
														STANDARD_FIELD_TYPE.ETA,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.MANAGER,
														STANDARD_FIELD_TYPE.OWNER,
														STANDARD_FIELD_TYPE.BILLTOID,
														STANDARD_FIELD_TYPE.SHIPPER,
														STANDARD_FIELD_TYPE.SHIPTOID,
														STANDARD_FIELD_TYPE.BOL_CARRIER,
														STANDARD_FIELD_TYPE.REQUESTED_DELIVERY_DATE,
														STANDARD_FIELD_TYPE.SHIPMENT_NUMBER,
														STANDARD_FIELD_TYPE.OPERATORID,
														STANDARD_FIELD_TYPE.DESTEQUIPMENT1ID,
														STANDARD_FIELD_TYPE.DESTEQUIPMENT2ID,
														STANDARD_FIELD_TYPE.DESTEQUIPMENT3ID,
														STANDARD_FIELD_TYPE.USER_DATA_1,
														STANDARD_FIELD_TYPE.USER_DATA_2,
														STANDARD_FIELD_TYPE.USER_DATA_3,
														STANDARD_FIELD_TYPE.USER_DATA_4,
														STANDARD_FIELD_TYPE.USER_DATA_5,
														STANDARD_FIELD_TYPE.USER_DATA_6,
														STANDARD_FIELD_TYPE.USER_DATA_7,
														STANDARD_FIELD_TYPE.USER_DATA_8,
														STANDARD_FIELD_TYPE.USER_DATA_9,
														STANDARD_FIELD_TYPE.USER_DATA_10,
														STANDARD_FIELD_TYPE.USER_DATA_11,
														STANDARD_FIELD_TYPE.USER_DATA_12,
														STANDARD_FIELD_TYPE.USER_DATA_13,
														STANDARD_FIELD_TYPE.USER_DATA_14,
														STANDARD_FIELD_TYPE.USER_DATA_15,
														STANDARD_FIELD_TYPE.USER_DATA_16,
														STANDARD_FIELD_TYPE.USER_DATA_17,
														STANDARD_FIELD_TYPE.USER_DATA_18,
														STANDARD_FIELD_TYPE.USER_DATA_19,
														STANDARD_FIELD_TYPE.USER_DATA_20,
														STANDARD_FIELD_TYPE.USER_DATA_21,
														STANDARD_FIELD_TYPE.USER_DATA_22,
														STANDARD_FIELD_TYPE.USER_DATA_23,
														STANDARD_FIELD_TYPE.USER_DATA_24,
														STANDARD_FIELD_TYPE.DELETE_FLAG
					};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.TRANSACTION_DATE,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
														STANDARD_FIELD_TYPE.TRANSACTION_ID,
														STANDARD_FIELD_TYPE.DOCUMENT_NUMBER,
														STANDARD_FIELD_TYPE.ORDER_STATUS,
														STANDARD_FIELD_TYPE.PO_NUMBER,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.MANAGER,
														STANDARD_FIELD_TYPE.OWNER,
														STANDARD_FIELD_TYPE.BILLTOID,
														STANDARD_FIELD_TYPE.SHIPPER,
														STANDARD_FIELD_TYPE.SHIPTOID,
														STANDARD_FIELD_TYPE.BOL_CARRIER
													};

						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.BOL_SUMMARY:
					{
						STANDARD_FIELD_TYPE[] fields = {STANDARD_FIELD_TYPE.BOL_NUMBER,
																STANDARD_FIELD_TYPE.BOL_MANAGER,
																STANDARD_FIELD_TYPE.BOL_OWNER,
																STANDARD_FIELD_TYPE.BOL_STATUS,
																STANDARD_FIELD_TYPE.SHIPPER,
																STANDARD_FIELD_TYPE.SHIPTOID,
																STANDARD_FIELD_TYPE.BILLTOID,
																STANDARD_FIELD_TYPE.BOL_DATE_TIME,
																STANDARD_FIELD_TYPE.BOL_CARRIER,
																STANDARD_FIELD_TYPE.PO_NUMBER,
																STANDARD_FIELD_TYPE.REVERSAL_TYPE,
																STANDARD_FIELD_TYPE.DESTINATIONSERIALNUMBER1,
																STANDARD_FIELD_TYPE.DESTINATIONSERIALNUMBER2,
																STANDARD_FIELD_TYPE.DESTINATIONSERIALNUMBER3
																};

						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.TRANSACTION_ID,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.TRANSACTION_DATE,
														STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
														STANDARD_FIELD_TYPE.ORDER_CONFIRM_NUMBER,
														STANDARD_FIELD_TYPE.ESTIMATED_DATE_FROM,
														STANDARD_FIELD_TYPE.ESTIMATED_DATE_TO,
														STANDARD_FIELD_TYPE.REQUIRED_DATE,
														STANDARD_FIELD_TYPE.ORDER_STATUS,
														STANDARD_FIELD_TYPE.PO_NUMBER,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.MANAGER,
														STANDARD_FIELD_TYPE.OWNER,
														STANDARD_FIELD_TYPE.BILLTOID,
														STANDARD_FIELD_TYPE.SHIPPER,
														STANDARD_FIELD_TYPE.SHIPTOID,
														STANDARD_FIELD_TYPE.BOL_CARRIER,
														STANDARD_FIELD_TYPE.DOCUMENT_NUMBER
													};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER_ASSOCIATED_TX:
					{
						STANDARD_FIELD_TYPE[] fields =	{	STANDARD_FIELD_TYPE.TRANSACTION_ID,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.TRANSACTION_DATE,
														STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
														STANDARD_FIELD_TYPE.DOCUMENT_NUMBER,
														STANDARD_FIELD_TYPE.ORDER_STATUS,
														STANDARD_FIELD_TYPE.PO_NUMBER,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.MANAGER,
														STANDARD_FIELD_TYPE.OWNER,
														STANDARD_FIELD_TYPE.BILLTOID,
														STANDARD_FIELD_TYPE.SHIPPER,
														STANDARD_FIELD_TYPE.SHIPTOID,
														STANDARD_FIELD_TYPE.BOL_CARRIER,
										  STANDARD_FIELD_TYPE.GROSS_QUANTITY,
										  STANDARD_FIELD_TYPE.NET_QUANTITY
													};

						return fields;
					}
				// vthompson 8/5/2008
				case LISTVIEW_STANDARD_TYPE.INVOICE:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.PRODUCT,
														STANDARD_FIELD_TYPE.SHIPTOID,
														STANDARD_FIELD_TYPE.INVOICE_NUMBER,
														STANDARD_FIELD_TYPE.GST,
														STANDARD_FIELD_TYPE.EXCISE,
														STANDARD_FIELD_TYPE.COST_CENTRE_CODE,
														STANDARD_FIELD_TYPE.ACCOUNT_CODE,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.PAYMENT_NUMBER,
														STANDARD_FIELD_TYPE.PRODUCT_PRICE,
														STANDARD_FIELD_TYPE.GROSS_QUANTITY,
														STANDARD_FIELD_TYPE.NET_QUANTITY,
										  STANDARD_FIELD_TYPE.ORDER_NUMBER,
										  STANDARD_FIELD_TYPE.REBATE_FLAG,
										  STANDARD_FIELD_TYPE.TOTAL_AMOUNT,
										  STANDARD_FIELD_TYPE.USER_DATA_1
													};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.TRANSACTION_DATE,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.PO_NUMBER,
														STANDARD_FIELD_TYPE.DOCUMENT_NUMBER,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.MANAGER,
														STANDARD_FIELD_TYPE.OWNER,
														STANDARD_FIELD_TYPE.BILLTOID,
														STANDARD_FIELD_TYPE.SHIPTOID,
										  STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
										  STANDARD_FIELD_TYPE.PRODUCT,
										  STANDARD_FIELD_TYPE.GROSS_QUANTITY,
										  STANDARD_FIELD_TYPE.NET_QUANTITY
													};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.RECOVERY:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.PRODUCT,
														STANDARD_FIELD_TYPE.SHIPTOID,
														STANDARD_FIELD_TYPE.INVOICE_NUMBER,
														STANDARD_FIELD_TYPE.GST,
														STANDARD_FIELD_TYPE.EXCISE,
														STANDARD_FIELD_TYPE.COST_CENTRE_CODE,
														STANDARD_FIELD_TYPE.ACCOUNT_CODE,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.PAYMENT_NUMBER,
														STANDARD_FIELD_TYPE.PRODUCT_PRICE,
														STANDARD_FIELD_TYPE.GROSS_QUANTITY,
														STANDARD_FIELD_TYPE.NET_QUANTITY,
										  STANDARD_FIELD_TYPE.ORDER_NUMBER,
										  STANDARD_FIELD_TYPE.TOTAL_AMOUNT,
										  STANDARD_FIELD_TYPE.DOCUMENT_NUMBER
													};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.RECOVERY_ASSOCIATED_TX:
					{
						STANDARD_FIELD_TYPE[] fields = {	STANDARD_FIELD_TYPE.TRANSACTION_DATE,
														STANDARD_FIELD_TYPE.INVENTORY_DATE,
														STANDARD_FIELD_TYPE.PO_NUMBER,
														STANDARD_FIELD_TYPE.DOCUMENT_NUMBER,
														STANDARD_FIELD_TYPE.SUPPLIER,
														STANDARD_FIELD_TYPE.MANAGER,
														STANDARD_FIELD_TYPE.OWNER,
														STANDARD_FIELD_TYPE.BILLTOID,
														STANDARD_FIELD_TYPE.SHIPTOID,
										  STANDARD_FIELD_TYPE.TRANSACTION_ALIAS,
										  STANDARD_FIELD_TYPE.PRODUCT
													};
						return fields;
					}

				case LISTVIEW_STANDARD_TYPE.AUTO_DISTRIBUTION_RULE:
					{
						STANDARD_FIELD_TYPE[] fields = {
							STANDARD_FIELD_TYPE.SITE_GUID,
							STANDARD_FIELD_TYPE.IDENTITY_GUID,
							STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_RULE_ID,
							STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_RULE_DESCRIPTION,
							STANDARD_FIELD_TYPE.MANAGERS,
							STANDARD_FIELD_TYPE.PRODUCTS,
							STANDARD_FIELD_TYPE.OWNERS,
							STANDARD_FIELD_TYPE.ENABLED,
							STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_DEFAULT_EOM,
							STANDARD_FIELD_TYPE.REASON_CODE,
							STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_TRANSACTION_ALIASES,
							STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_TRANSACTION_ALIAS,
							
						};
						return fields;
					}
				default:
					{
						STANDARD_FIELD_TYPE[] fields = { };
						return fields;
					}
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				if (this.Type == LISTVIEW_TYPE.STANDARD && this.ListViewStandardType == LISTVIEW_STANDARD_TYPE.LEDGER)
				{
					return ENTITY_TYPE.LEDGER_VIEW;
				}

				return ENTITY_TYPE.LIST_VIEW;
			}
		}

		[XmlIgnore]
		public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

	    [XmlIgnore]
		public ENTITY_TYPE LedgerEntityType => ENTITY_TYPE.LEDGER_VIEW;

	    public ListViewClass()
		{
			this.Reset();
		}

		public ListViewClass(LISTVIEW_TYPE listViewType, Guid typeGuid)
		{
		    this._Type = listViewType;
		    this._TypeGuid = typeGuid;
		    this._ListViewStandardType = GetStandardTypeFromGuid(this._TypeGuid);
		}

		public static string ListViewTypeID(LISTVIEW_TYPE listViewType)
		{
			switch (listViewType)
			{
				case LISTVIEW_TYPE.STANDARD:
					return "Standard";
				case LISTVIEW_TYPE.TRANSACTION_LIST:
					return "Transaction List";
				case LISTVIEW_TYPE.AGGREGATE:
					return "Aggregate Column List";
				default:
					return "Undefined";
			}
		}

		public static string ListViewStandardTypeID(LISTVIEW_STANDARD_TYPE listViewStandardType)
		{
			switch (listViewStandardType)
			{
				case LISTVIEW_STANDARD_TYPE.LEDGER:
					return "Ledger";

				case LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_SUMMARY:
					return "Meter Reconciliation Summary";

				case LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_DETAIL:
					return "Meter Reconciliation Detail";

				case LISTVIEW_STANDARD_TYPE.RECEIPT_RECONCILIATION:
					return "Receipt Reconciliation";

				case LISTVIEW_STANDARD_TYPE.INVENTORY_RECONCILIATION:
					return "Inventory Reconciliation";

				case LISTVIEW_STANDARD_TYPE.CLOSEOUT:
					return "Closeout";

				case LISTVIEW_STANDARD_TYPE.EQUIPMENT_TRANSACTION:
					return "Equipment Transaction";

				case LISTVIEW_STANDARD_TYPE.RECEIPT_ASSIGNMENT_ASSIGNED:
					return "Receipt Assignment Assigned";

				case LISTVIEW_STANDARD_TYPE.RECEIPT_ASSIGNMENT_AVAILABLE:
					return "Receipt Assignment Available";

				case LISTVIEW_STANDARD_TYPE.AUTOMATIC_PHYSICAL_INVENTORY:
					return "Automatic Physical Inventory";

				case LISTVIEW_STANDARD_TYPE.ORDER:
					return "Order Summary";

				case LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX:
					return "Order Associated Transactions";

				case LISTVIEW_STANDARD_TYPE.BOL_SUMMARY:
					return "BOL Summary";

				case LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER:
					return "Supply Order Summary";

				case LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER_ASSOCIATED_TX:
					return "Supply Order Associated Transactions";

				case LISTVIEW_STANDARD_TYPE.INVOICE:
					return "Payment Invoice Summary";

				case LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX:
					return "Payment Associated Transactions";

				case LISTVIEW_STANDARD_TYPE.RECOVERY:
					return "Recovery Invoice Summary";

				case LISTVIEW_STANDARD_TYPE.RECOVERY_ASSOCIATED_TX:
					return "Recovery Associated Transactions";

				case LISTVIEW_STANDARD_TYPE.AUTO_DISTRIBUTION_RULE:
					return AutoDistributionRulesName;

				default:
					return "Undefined";
			}
		}

		public static LISTVIEW_STANDARD_TYPE GetListViewStandardType(string typeID)
		{
			if (typeID == "Ledger")
			{
				return LISTVIEW_STANDARD_TYPE.LEDGER;
			}
			else if (typeID == "Meter Reconciliation Summary")
			{
				return LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_SUMMARY;
			}
			else if (typeID == "Meter Reconciliation Detail")
			{
				return LISTVIEW_STANDARD_TYPE.METER_RECONCILIATION_DETAIL;
			}
			else if (typeID == "Receipt Reconciliation")
			{
				return LISTVIEW_STANDARD_TYPE.RECEIPT_RECONCILIATION;
			}
			else if (typeID == "Inventory Reconciliation")
			{
				return LISTVIEW_STANDARD_TYPE.INVENTORY_RECONCILIATION;
			}
			else if (typeID == "Closeout")
			{
				return LISTVIEW_STANDARD_TYPE.CLOSEOUT;
			}
			else if (typeID == "Equipment Transaction")
			{
				return LISTVIEW_STANDARD_TYPE.EQUIPMENT_TRANSACTION;
			}
			else if (typeID == "Receipt Assignment Assigned")
			{
				return LISTVIEW_STANDARD_TYPE.RECEIPT_ASSIGNMENT_ASSIGNED;
			}
			else if (typeID == "Receipt Assignment Available")
			{
				return LISTVIEW_STANDARD_TYPE.RECEIPT_ASSIGNMENT_AVAILABLE;
			}
			else if (typeID == "Automatic Physical Inventory")
			{
				return LISTVIEW_STANDARD_TYPE.AUTOMATIC_PHYSICAL_INVENTORY;
			}
			else if (typeID == "Order Summary")
			{
				return LISTVIEW_STANDARD_TYPE.ORDER;
			}
			else if (typeID == "Order Associated Transactions")
			{
				return LISTVIEW_STANDARD_TYPE.ORDER_ASSOCIATED_TX;
			}
			else if (typeID == "BOL Summary")
			{
				return LISTVIEW_STANDARD_TYPE.BOL_SUMMARY;
			}
			else if (typeID == "Supply Order Summary")
			{
				return LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER;
			}
			else if (typeID == "Supply Order Associated Transactions")
			{
				return LISTVIEW_STANDARD_TYPE.SUPPLY_ORDER_ASSOCIATED_TX;
			}
			else if (typeID == "Payment Associated Transactions")
			{
				return LISTVIEW_STANDARD_TYPE.ASSOCIATED_TX;
			}
			else if (typeID == "Invoice Summary")
			{
				return LISTVIEW_STANDARD_TYPE.INVOICE;
			}
			else if (typeID == "Recovery Invoice Summary")
			{
				return LISTVIEW_STANDARD_TYPE.RECOVERY;
			}
			else if (typeID == "Recovery Associated Transactions")
			{
				return LISTVIEW_STANDARD_TYPE.RECOVERY_ASSOCIATED_TX;
			}
			else if (typeID == AutoDistributionRulesName)
			{
				return LISTVIEW_STANDARD_TYPE.AUTO_DISTRIBUTION_RULE;
			}
			else
			{
				return LISTVIEW_STANDARD_TYPE.TYPE_MAX;
			}
		}

		public override void Reset()
		{
			base.Reset();
		    this._Type = LISTVIEW_TYPE.TYPE_MAX;
		    this._TypeGuid = Guid.Empty;
		    this._ListViewStandardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;
		    this.ListViewFieldCollection = new ListViewFieldCollectionClass();
		    this.ProductMapCollection = new ProductMapCollectionClass();
		    this.GroupMapCollection = new GroupLedgerViewMapCollectionClass();
		}

		public void Load(DataSet set)
		{
			if (set == null)
			{
				throw new ArgumentNullException(nameof(set));
			}

			this.Reset();

			DataTable table = set.Tables[0];

			if (table.Rows.Count == 0)
			{
				return;
			}

			DataRow row = table.Rows[0];

		    this._IdentityGuid = DataObject.getValue<Guid>(row["ListViewGuid"], Guid.Empty);
		    this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
		    this._Type = DataObject.getValue<LISTVIEW_TYPE>(row["LookupListViewTypeIndex"], LISTVIEW_TYPE.TYPE_MAX);

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
			    this._TypeGuid = DataObject.getValue<Guid>(row["LedgerAggregateColumnGuid"], Guid.Empty);
			    this._ListViewStandardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;
				// Aggregate views get their ID from the associated Aggregate name
			    this._ID = DataObject.getValue<string>(row["AggregateName"], "");
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
			    this._TypeGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
			    this._ListViewStandardType = LISTVIEW_STANDARD_TYPE.TYPE_MAX;
				// Transaction list views get their ID from the associated Alias name
			    this._ID = DataObject.getValue<string>(row["AliasName"], "");
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
			    this._ListViewStandardType = DataObject.getValue<LISTVIEW_STANDARD_TYPE>(row["LookupListViewStandardTypeIndex"], LISTVIEW_STANDARD_TYPE.TYPE_MAX);
			    this._TypeGuid = GetGuidFromStandardType(this._ListViewStandardType);
				// Standard views store their ID in the ListViews table
			    this._ID = DataObject.getValue<string>(row["ID"], "");
			}

		    this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
		    this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
		    this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
		    this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
		}

		#region paramaterized SQL

		private string SelectClause => "SELECT tblListViews.*," +
		                               "(SELECT AliasName FROM tblTransactionAliases WHERE tblListViews.LookupListViewTypeIndex = 1 " +
		                               "AND tblListViews.TransactionAliasGuid = tblTransactionAliases.TransactionAliasGuid) AS AliasName," +
		                               "(SELECT ID FROM tblLedgerAggregateColumns WHERE tblListViews.LookupListViewTypeIndex = 3 " +
		                               "AND tblListViews.LedgerAggregateColumnGuid = tblLedgerAggregateColumns.LedgerAggregateColumnGuid) AS AggregateName ";

	    public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblListViews " +
				"(SiteGuid," +
				"LookupListViewTypeIndex,";

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.CommandText += "LedgerAggregateColumnGuid,";
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.CommandText += "TransactionAliasGuid,";
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.CommandText += "LookupListViewStandardTypeIndex,";
			}

			cmd.CommandText +=
				"ID," +
				"CreatedDate," +
				"CreatedBy," +
				"UpdatedDate," +
				"UpdatedBy," +
				"ListViewGuid" +
				") VALUES (" +
				"@SiteGuid," +
				"@LookupListViewTypeIndex,";

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.CommandText += "@LedgerAggregateColumnGuid,";
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.CommandText += "@TransactionAliasGuid,";
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.CommandText += "@LookupListViewStandardTypeIndex,";
			}

			cmd.CommandText +=
				"@ID," +
				"@CreatedDate," +
				"@CreatedBy," +
				"@UpdatedDate," +
				"@UpdatedBy," +
				"@ListViewGuid)";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@LookupListViewTypeIndex", (int)this._Type);

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._TypeGuid);
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.Parameters.AddWithValue("@LookupListViewStandardTypeIndex", (int)this._ListViewStandardType);
			}

			cmd.Parameters.AddWithValue("@ID", this._ID);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@ListViewGuid", this._IdentityGuid);
		}

		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblListViews SET " +
					"SiteGuid = @SiteGuid," +
					"ID = @ID," +
					"UpdatedDate = @UpdatedDate," +
					"UpdatedBy = @UpdatedBy " +
					"WHERE ListViewGuid = @ListViewGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@ListViewGuid", this._IdentityGuid);
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblListViews " +
					"WHERE ListViewGuid = @ListViewGuid";

			cmd.Parameters.AddWithValue("@ListViewGuid", this._IdentityGuid);
		}

		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblListViews " + SQLUpdateLock(bInTransaction) +
				" WHERE ListViewGuid = @ListViewGuid";

			cmd.Parameters.AddWithValue("@ListViewGuid", this._IdentityGuid);
		}

		public void SelectByTypeAndForeignKeySQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblListViews, " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + " " + SQLUpdateLock(bInTransaction) +
				" WHERE " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + ".SiteGuid = @SiteGuid" +
				" AND " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + ".ListViewGuid = tblListViews.ListViewGuid";

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.CommandText += " AND tblListViews.LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid";
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.CommandText += " AND tblListViews.TransactionAliasGuid = @TransactionAliasGuid";
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.CommandText += " AND tblListViews.LookupListViewStandardTypeIndex = @LookupListViewStandardTypeIndex";
			}

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._TypeGuid);
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.Parameters.AddWithValue("@LookupListViewStandardTypeIndex", (int)this._ListViewStandardType);
			}
		}

		public void SelectByLedgerIDSQL(bool bInTransaction, SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblListViews, " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + " " + SQLUpdateLock(bInTransaction) +
				" WHERE " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + ".SiteGuid = @SiteGuid" +
				" AND " + EntityToSiteMapClass.GetMappingTableName(this.EntityType) + ".ListViewGuid = tblListViews.ListViewGuid" +
				" AND tblListViews.ID = @ID";

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.CommandText += " AND tblListViews.LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid";
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
                cmd.CommandText += " AND tblListViews.TransactionAliasGuid = (SELECT _MasterRecordGuid FROM tblTransactionAliases WHERE TransactionAliasGuid =  @TransactionAliasGuid)";
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.CommandText += " AND tblListViews.LookupListViewStandardTypeIndex = @LookupListViewStandardTypeIndex";
			}

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			cmd.Parameters.AddWithValue("@ID", this._ID);

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._TypeGuid);
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.Parameters.AddWithValue("@LookupListViewStandardTypeIndex", (int)this._ListViewStandardType);
			}
		}

		public void EnumerateSQL(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblListViews" +
				" WHERE" + this.AppendSiteWhereClauseParameters(cmd, security, "tblListViews", "ListViewGuid");
			//cmd Parameters and parameter values will be created in AppendSiteWhereClauseParameters
		}

		public void EnumerateByTypeAndForeignKeySQL(SecurityClass security, SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblListViews" +
				" WHERE" + this.AppendSiteWhereClauseParameters(cmd, security, "tblListViews", "ListViewGuid");

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.CommandText += " AND tblListViews.LedgerAggregateColumnGuid = @LedgerAggregateColumnGuid";
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.CommandText += " AND tblListViews.TransactionAliasGuid = @TransactionAliasGuid";
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.CommandText += " AND tblListViews.LookupListViewStandardTypeIndex = @LookupListViewStandardTypeIndex";
			}

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);

			if (this._Type == LISTVIEW_TYPE.AGGREGATE)
			{
				cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_TYPE.TRANSACTION_LIST)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._TypeGuid);
			}
			else // Otherwise _Type == LISTVIEW_TYPE.STANDARD
			{
				cmd.Parameters.AddWithValue("@LookupListViewStandardTypeIndex", (int)this._ListViewStandardType);
			}
		}

		public void EnumerateAggregatesByAliasGuidSQL(SecurityClass security, SqlCommand cmd, Guid aliasGuid)
		{
			cmd.CommandText = this.SelectClause +
				" FROM tblListViews" +
				" WHERE " + this.AppendSiteWhereClause(cmd, security, "tblListViews", "ListViewGuid") +
				" AND tblListViews.LookupListViewTypeIndex = " + ((int)LISTVIEW_TYPE.AGGREGATE) +
				" AND tblListViews.LedgerAggregateColumnGuid in (Select LedgerAggregateColumnGuid from map.tblLedgerAggregateColumnToTransactionAlias where TransactionAliasGuid = @AliasGuid)";

			cmd.Parameters.AddWithValue("@AliasGuid", aliasGuid);
		}

		/// <summary>
		/// This method will populate an SQL command to enumerate the List Views based on
		/// site, user, and product map assignment.
		/// </summary>
		/// <param name="security">The security object.</param>
		/// <param name="command">The SQL Command object.</param>
		/// <param name="productGuid">The product GUID.</param>
		public void EnumerateByProductAndUserSQL(SecurityClass security, SqlCommand command, Guid productGuid)
		{
			command.CommandText = this.SelectClause +
				" FROM tblListViews" +
				" WHERE " + this.AppendSiteWhereClause(command, security, "tblListViews", "ListViewGuid") +
				" AND tblListViews.LookupListViewTypeIndex = " + ((int) LISTVIEW_TYPE.STANDARD) +
				" AND tblListViews.LookupListViewStandardTypeIndex = " + ((int) LISTVIEW_STANDARD_TYPE.LEDGER) +
				" AND ListViewGuid IN (SELECT AssignedToListViewGuid FROM map.tblProductToLedgerView WHERE ProductGuid = @ProductGuid " +
				" OR ProductGuid = (SELECT prv.MasterRecordGuid FROM [erv].[udf_GetProductRecordVersions](@SiteGuid) prv " +
				" WHERE prv.ProductGuid = @ProductGuid)) " +
				" AND ListViewGuid IN (SELECT ListViewGuid FROM map.tblGroupToLedgerView WHERE GroupGuid IN " +
				" (SELECT GroupGuid FROM map.tblUserToGroup WHERE UserGuid = @UserGuid AND SiteGuid = @SiteGuid))";

			var parm = new SqlParameter("@ProductGuid", SqlDbType.UniqueIdentifier) { Value = productGuid };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@UserGuid", SqlDbType.UniqueIdentifier) { Value = security.UserGuid };
			command.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			command.Parameters.Add(parm);
		}
		#endregion
	}
}
