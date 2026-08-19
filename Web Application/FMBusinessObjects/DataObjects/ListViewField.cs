// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ListViewField.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the LISTVIEW_FIELD_TYPE type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	#region Public enumerations
	/// <summary>
	/// The listview field type.
	/// </summary>
	public enum LISTVIEW_FIELD_TYPE
	{
		TRANSACTION_ALIAS = 1,
		TRANSACTION_ALIAS_FIELD = 2,
		USER_DATA_FIELD = 3,
		STANDARD_FIELD = 4,
		LINE_ITEM_USER_DATA_FIELD = 5,
		AGGREGATE_FIELD = 6,
		TYPE_MAX = 7
	}

	/// <summary>
	/// The standard field type.
	/// </summary>
	public enum STANDARD_FIELD_TYPE
	{
		BEGIN_INVENTORY = 1,
		BOOK_INVENTORY = 2,
		INVENTORY_DATE = 3,
		ASSET_ID = 4,
		METER_START = 5,
		METER_STOP = 6,
		DIFFERENTIAL = 7,
		TOTAL_VOLUME = 8,
		VARIANCE = 9,
		TRANSACTION_DATE = 10,
		TRANSACTION_ID = 11,
		PHYSICAL_INVENTORY = 12,
		TOTAL_VARIANCE = 13,
		RECEIPTS = 14,
		ADJUSTMENTS = 15,
		DEFUELS = 16,
		ISSUES = 17,
		LOAD_RACK = 18,
		REQUEST = 19,
		ROTATION = 20,
		TRANSFERS = 21,
		TRANSACTION_TYPE = 22,
		VOLUME = 23,
		CONSUMER = 24,
		DESTINATION_REGISTRATION_ID = 25,
		SERIAL_NUMBER = 26,
		PRODUCT = 27,
		LOCATION = 28,
		INVENTORY = 29,
		TEMPERATURE = 30,
		DENSITY = 31,
		VCF = 32,
		SITE = 33,
		BILLED_VOLUME = 34,
		MEASURED_VOLUME = 35,
		BILL_OF_LADING_NUMBER = 36,
		BOOK_RECEIPTS = 37,
		ASSIGNED = 38,
		REMAINING = 39,
		OWNER = 40,
		MANAGER = 41,
		TYPE_MAX = 42,
		TOTAL_PHYSICAL_INVENTORY = 43,
		BILLTOID = 44,
		SHIPTOID = 45,
		TRANSACTION_ALIAS = 46,
		DOCUMENT_NUMBER = 47,
		PO_NUMBER = 48,
		SCHEDULED_DATE = 49,
		ORDER_STATUS = 50,
		EFFECTIVE_DATE = 51,
		EXPIRATION_DATE = 52,
		ETA = 53,
		BOL_NUMBER = 54,
		SHIPPER = 55,
		BOL_STATUS = 56,
		BOL_DATE_TIME = 57,
		BOL_CARRIER = 58,
		BOL_MANAGER = 59,
		BOL_OWNER = 60,
		TOTAL_ACTIVITY = 61,
		CLOSEOUT_DATE = 62,
		ESTIMATED_DATE_FROM = 63,
		ESTIMATED_DATE_TO = 64,
		ORDER_CONFIRM_NUMBER = 65,
		STANDING_OFFER_NUMBER = 66,
		REQUIRED_DATE = 67,
		SUPPLIER = 68,
		REQUESTED_DELIVERY_DATE = 69,
		SHIPMENT_NUMBER = 70,
		USER_DATA_1 = 71,
		USER_DATA_2 = 72,
		USER_DATA_3 = 73,
		USER_DATA_4 = 74,
		USER_DATA_5 = 75,
		USER_DATA_6 = 76,
		USER_DATA_7 = 77,
		USER_DATA_8 = 78,
		USER_DATA_9 = 79,
		USER_DATA_10 = 80,
		USER_DATA_11 = 81,
		USER_DATA_12 = 82,
		USER_DATA_13 = 83,
		USER_DATA_14 = 84,
		USER_DATA_15 = 85,
		USER_DATA_16 = 86,
		USER_DATA_17 = 87,
		USER_DATA_18 = 88,
		USER_DATA_19 = 89,
		USER_DATA_20 = 90,
		USER_DATA_21 = 91,
		USER_DATA_22 = 92,
		USER_DATA_23 = 93,
		USER_DATA_24 = 94,
		OPERATORID = 95,
		DESTEQUIPMENT1ID = 96,
		DESTEQUIPMENT2ID = 97,
		DESTEQUIPMENT3ID = 98,
		EXCISE = 99,
		COST_CENTRE_CODE = 100,
		GST = 101,
		INVOICE_NUMBER = 102,
		VOUCHER_NUMBER = 103,
		ACCOUNT_CODE = 104,
		LEGACY_NUMBER = 105,
		CONTACT_INFO = 106,
		CONTACT_SURNAME = 107,
		CONTACT_FIRST_NAME = 108,
		PRODUCT_PRICE = 109,
		GROSS_QUANTITY = 110,
		NET_QUANTITY = 111,
		BATCH_NUMBER = 112,
		ORDER_NUMBER = 113,
		PAYMENT_NUMBER = 114,
		TOTAL_AMOUNT = 115,
		REBATE_FLAG = 116,
		LINE_ITEM_USER_DATA_01 = 117,
		LINE_ITEM_USER_DATA_02 = 118,
		LINE_ITEM_USER_DATA_03 = 119,
		LINE_ITEM_USER_DATA_04 = 120,
		LINE_ITEM_USER_DATA_05 = 121,
		LINE_ITEM_USER_DATA_06 = 122,
		LINE_ITEM_USER_DATA_07 = 123,
		LINE_ITEM_USER_DATA_08 = 124,
		LINE_ITEM_USER_DATA_09 = 125,
		LINE_ITEM_USER_DATA_10 = 126,
		LINE_ITEM_USER_DATA_11 = 127,
		LINE_ITEM_USER_DATA_12 = 128,
		LINE_ITEM_USER_DATA_13 = 129,
		LINE_ITEM_USER_DATA_14 = 130,
		LINE_ITEM_USER_DATA_15 = 131,
		LINE_ITEM_USER_DATA_16 = 132,
		LINE_ITEM_USER_DATA_17 = 133,
		LINE_ITEM_USER_DATA_18 = 134,
		LINE_ITEM_USER_DATA_19 = 135,
		LINE_ITEM_USER_DATA_20 = 136,
		LINE_ITEM_USER_DATA_21 = 137,
		LINE_ITEM_USER_DATA_22 = 138,
		LINE_ITEM_USER_DATA_23 = 139,
		LINE_ITEM_USER_DATA_24 = 140,
		MASSQUANTITY = 141,
		GROSS_MANUALVALUE = 142,
		NET_MANUALVALUE = 143,
		MASS_MANUALVALUE = 144,
		VCF_MANUALVALUE = 145,
		ALTERNATIVE_NET_VOLUME = 146,
		TOLERANCE = 147,
		ALLOWED_GAIN_LOSS = 148,
		VARIANCE_PERCENTAGE = 149,
		ROTATES_BACKWARDS = 150,
		METER_TOTAL = 151,
		TRANSACTION_METER_TOTAL = 152,
		METER_VARIANCE = 153,
		METER_RECONCILIATION_ERROR = 154,
		METER_SKIP = 155,
		FLIGHT_NUMBER = 156,
		TICKET_NUMBER = 157,
		STATION = 158,
		METER_ID = 159,
		VIEW_DETAILS = 160,
		AUTO_DISTRIBUTION_RULE_ID = 161,
		AUTO_DISTRIBUTION_RULE_DESCRIPTION = 162,
		ENABLED = 163,
		AUTO_DISTRIBUTION_DEFAULT_EOM = 164,
		AUTO_DISTRIBUTION_TRANSACTION_ALIAS = 165,
		REASON_CODE = 166,
		MANAGERS = 167,
		PRODUCTS = 168,
		AUTO_DISTRIBUTION_TRANSACTION_ALIASES = 169,
		OWNERS = 170,
		SITE_GUID = 171,        // hidden field
		IDENTITY_GUID = 172,    // hidden field
		PACKAGE_MANUALVALUE = 173,
		DELETE_FLAG = 174,               // hidden field
		REVERSAL_TYPE = 175,
		DESTINATIONSERIALNUMBER1 = 176,
		DESTINATIONSERIALNUMBER2 = 177,
		DESTINATIONSERIALNUMBER3 = 178,
		TRANSACTION_VOLUME_TOTAL = 179,
		VOLUME_VARIANCE = 180
	}

	#endregion

	#region List View Field Collection Class
	/// <summary>
	/// The list view field collection class.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class ListViewFieldCollectionClass : List<ListViewFieldClass>
	{
	}
	#endregion

	/// <summary>
	/// The list view field class.
	/// </summary>
	[Serializable]
	[DataContract]
	[KnownType(typeof(LedgerAggregateColumnClass))]
	public class ListViewFieldClass : BaseDataObject
	{
		#region Protected data members
		[DataMember] protected Guid _ListViewGuid;
		[DataMember] protected LISTVIEW_FIELD_TYPE _Type;
		[DataMember] protected Guid _TypeGuid;
		[DataMember] protected STANDARD_FIELD_TYPE _StandardFieldType;
		[DataMember] protected int _ColumnOrder;
		[DataMember] protected string _DataPath;
		[DataMember] protected bool virtualField;
		#endregion

		/// <summary>
		/// The standard field type GUID prefix.
		/// </summary>
		private const string StandardFieldTypeGuidPrefix = "30000000-0000-0000-0000-000000000";

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewFieldClass"/> class. 
		/// This is the default constructor for the List View Field Class.
		/// </summary>
		public ListViewFieldClass()
		{
			this.Initialize();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewFieldClass"/> class.
		/// </summary>
		/// <param name="type">
		/// The type.
		/// </param>
		/// <param name="typeGuid">
		/// The type GUID.
		/// </param>
		/// <param name="columnOrder">
		/// The column order.
		/// </param>
		/// <param name="columnName">
		/// The column name.
		/// </param>
		public ListViewFieldClass(
								 LISTVIEW_FIELD_TYPE type,
								 Guid typeGuid,
								 int columnOrder,
								 string columnName)
		{
			this._Type = type;
			this._TypeGuid = typeGuid;
			this._StandardFieldType = GetStandardFieldTypeFromGuid(typeGuid);
			this._ColumnOrder = columnOrder;
			this._ID = columnName;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the transaction alias GUID.
		/// </summary>
		[DataMember]
		public Guid TransactionAliasGuid
		{
			get;
			set;
		}

		[DataMember]
		public string ListViewID
		{
			get;
			set;
		}

		public Guid ListViewGuid
		{
			get { return _ListViewGuid; }
			set { _ListViewGuid = value; }
		}

		public LISTVIEW_FIELD_TYPE Type
		{
			get { return _Type; }
			set { _Type = value; }
		}

		public Guid TypeGuid
		{
			get { return _TypeGuid; }
			set { _TypeGuid = value; this._StandardFieldType = GetStandardFieldTypeFromGuid(value); }
		}

		public STANDARD_FIELD_TYPE StandardFieldType
		{
			get { return _StandardFieldType; }
			set { _StandardFieldType = value; }
		}

		public int ColumnOrder
		{
			get { return _ColumnOrder; }
			set { _ColumnOrder = value; }
		}

		public string DataPath
		{
			get { return _DataPath; }
			set { _DataPath = value; }
		}

		public bool VirtualField
		{
			get { return this.virtualField; }
			set { this.virtualField = value; }
		}

		public const string DESTINATION_SERIAL_NUMBER_1_DISPLAY = "Destination Serial Number 1";
		public const string DESTINATION_SERIAL_NUMBER_2_DISPLAY = "Destination Serial Number 2";
		public const string DESTINATION_SERIAL_NUMBER_3_DISPLAY = "Destination Serial Number 3";

		[DataMember]
		public LedgerAggregateColumnClass.AggregateType AggregateType
		{
			get;
			set;
		}

		/// <summary>
		/// This property will return True if the transaction field
		/// type is a Link.
		/// </summary>
		public bool IsLink
		{
			get
			{
				switch (_Type)
				{
					case LISTVIEW_FIELD_TYPE.STANDARD_FIELD:
						switch (StandardFieldType)
						{
							case STANDARD_FIELD_TYPE.TOTAL_PHYSICAL_INVENTORY:
								return false;
							default:
								return false;
						}
					case LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS:
						return true;
					case LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD:
						return false;
					case LISTVIEW_FIELD_TYPE.USER_DATA_FIELD:
						return false;
					case LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD:
						return false;
					case LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD:
						return true;
					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Returns true if current field is a hidden field
		/// </summary>
		public bool IsHidden
		{
			get
			{
				return IsFieldHidden(StandardFieldType);
			}
		}

		/// <summary>
		/// Gets a value indicating whether is column wrapped.
		/// </summary>
		public bool IsColumnWrapped
		{
			get
			{
				return IsColumnWrappedForField(this.StandardFieldType);
			}
		}

		/// <summary>
		/// Gets a value indicating whether row parameter.
		/// </summary>
		public bool RowParameter
		{
			get
			{
				switch (this._Type)
				{
					case LISTVIEW_FIELD_TYPE.STANDARD_FIELD:
						{
							return this.StandardFieldType == STANDARD_FIELD_TYPE.INVENTORY_DATE;
						}

					case LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS:
						return false;

					case LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD:
						return false;

					case LISTVIEW_FIELD_TYPE.USER_DATA_FIELD:
						return false;

					case LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD:
						return false;

					case LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD:
						return false;

					default:
						return false;
				}
			}
		}

		/// <summary>
		/// This property will return True if the transaction field type
		/// is data dictionary type.
		/// </summary>
		public bool DataDictionaryType
		{
			get
			{
				switch (_Type)
				{
					case LISTVIEW_FIELD_TYPE.STANDARD_FIELD:
						return true;

					case LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS:
					case LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD:
					case LISTVIEW_FIELD_TYPE.USER_DATA_FIELD:
					case LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD:
					case LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD:
					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Gets the data type.
		/// </summary>
		public Type DataType
		{
			get
			{
				Type returnType = typeof(string);

				if (this._Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD)
				{
					switch (this._StandardFieldType)
					{
						case STANDARD_FIELD_TYPE.DELETE_FLAG:
						case STANDARD_FIELD_TYPE.ENABLED:
						case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_DEFAULT_EOM:
							returnType = typeof(bool);
							break;
						case STANDARD_FIELD_TYPE.SITE_GUID:
						case STANDARD_FIELD_TYPE.IDENTITY_GUID:
							returnType = typeof(Guid);
							break;
					}
				}

				return returnType;
			}
		}

		/// <summary>
		/// Gets the select clause.
		/// </summary>
		private string SelectClause
		{
			get
			{
				const string SQL =
					"SELECT lvf.*, ta.AliasName AS AliasName, "
					+ "taf.AliasID AS AliasID, "
					+ "taf.DisplayName AS AliasFieldName,  "
					+ "taf.LookupTransactionFieldTypeIndex AS AliasFieldType,  "
					+ "taf.DbName AS AliasFieldDbName,  "
					+ "udf1.DisplayName AS UserDataName,  "
					+ "udf2.DisplayName AS LineItemUserDataName,  "
					+ "udf1.Number AS UserDataNumber,  "
					+ "udf2.Number AS LineItemUserDataNumber,   "
					+ "taf.Virtual AS VirtualField,  "
					+ "lac.ID AS AggregateID ";

				return SQL;
			}
		}
		#endregion

		/// <summary>
		/// The get standard field type from GUID.
		/// </summary>
		/// <param name="standardFieldTypeGuid">
		/// The standard field type GUID.
		/// </param>
		/// <returns>
		/// The <see cref="STANDARD_FIELD_TYPE"/>.
		/// </returns>
		public static STANDARD_FIELD_TYPE GetStandardFieldTypeFromGuid(Guid standardFieldTypeGuid)
		{
			var standardFieldType = STANDARD_FIELD_TYPE.TYPE_MAX;
			string guidString = standardFieldTypeGuid.ToString();
			string enumSuffix = guidString.Substring(guidString.Length - 3);
			string prefix = guidString.Substring(0, guidString.Length - 3);

			if (prefix == StandardFieldTypeGuidPrefix)
			{
				standardFieldType = (STANDARD_FIELD_TYPE)Convert.ToInt32(enumSuffix);
			}

			return standardFieldType;
		}

		/// <summary>
		/// The get GUID from standard field type.
		/// </summary>
		/// <param name="standardFieldType">
		/// The standard field type.
		/// </param>
		/// <returns>
		/// The <see cref="Guid"/>.
		/// </returns>
		public static Guid GetGuidFromStandardFieldType(STANDARD_FIELD_TYPE standardFieldType)
		{
			string prefix = StandardFieldTypeGuidPrefix;
			string enumSuffix = ((int)standardFieldType).ToString("D3");
			Guid standardFieldTypeGuid = Guid.Parse(prefix + enumSuffix);

			return standardFieldTypeGuid;
		}


		/// <summary>
		/// Returns true if the given field type is a hidden field
		/// </summary>
		/// <param name="fieldType">
		/// </param>
		/// <returns>
		/// </returns>
		public static bool IsFieldHidden(STANDARD_FIELD_TYPE fieldType)
		{
			bool retValue = false;

			switch (fieldType)
			{
				case STANDARD_FIELD_TYPE.SITE_GUID:
				case STANDARD_FIELD_TYPE.IDENTITY_GUID:
				case STANDARD_FIELD_TYPE.DELETE_FLAG:
					retValue = true;
					break;
			}

			return retValue;
		}

		/// <summary>
		/// Returns true if the column for the given field type should be wrapped or not
		/// </summary>
		/// <param name="fieldType">
		/// </param>
		/// <returns>
		/// </returns>
		public static bool IsColumnWrappedForField(STANDARD_FIELD_TYPE fieldType)
		{
			bool retValue = false;
			switch (fieldType)
			{
				case STANDARD_FIELD_TYPE.MANAGERS:
				case STANDARD_FIELD_TYPE.OWNERS:
				case STANDARD_FIELD_TYPE.PRODUCTS:
				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_TRANSACTION_ALIASES:
				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_RULE_DESCRIPTION:
					retValue = true;
					break;
			}

			return retValue;
		}

		/// <summary>
		/// This method will return the display name or the DB name of the standard fields.
		/// </summary>
		/// <param name="Type"></param>
		/// <param name="DisplayForm"></param>
		/// <returns></returns>
		public static string StandardFieldTypeID(STANDARD_FIELD_TYPE Type, bool DisplayForm)
		{
			string fieldValue;

			switch (Type)
			{
				case STANDARD_FIELD_TYPE.BEGIN_INVENTORY:
					{
						fieldValue = DisplayForm ? "Begin Inventory" : "BeginInventory";
						break;
					}

				case STANDARD_FIELD_TYPE.BOOK_INVENTORY:
					{
						fieldValue = DisplayForm ? "Book Inventory" : "BookInventory";
						break;
					}

				case STANDARD_FIELD_TYPE.INVENTORY_DATE:
					{
						fieldValue = DisplayForm ? "Inventory Date" : "InventoryDate";
						break;
					}

				case STANDARD_FIELD_TYPE.ASSET_ID:
					{
						fieldValue = DisplayForm ? "Asset ID" : "AssetID";
						break;
					}

				case STANDARD_FIELD_TYPE.METER_START:
					{
						fieldValue = DisplayForm ? "Meter Start" : "MeterStart";
						break;
					}

				case STANDARD_FIELD_TYPE.METER_STOP:
					{
						fieldValue = DisplayForm ? "Meter Stop" : "MeterStop";
						break;
					}

				case STANDARD_FIELD_TYPE.DIFFERENTIAL:
					{
						fieldValue = "Differential";
						break;
					}

				case STANDARD_FIELD_TYPE.TOTAL_VOLUME:
					{
						fieldValue = DisplayForm ? "Total Volume" : "TotalVolume";

						break;
					}

				case STANDARD_FIELD_TYPE.VARIANCE:
					{
						fieldValue = "Variance";
						break;
					}

				case STANDARD_FIELD_TYPE.TRANSACTION_DATE:
					{
						fieldValue = DisplayForm ? "Transaction Date" : "TransactionDate";

						break;
					}

				case STANDARD_FIELD_TYPE.TRANSACTION_ID:
					{
						fieldValue = DisplayForm ? "Transaction ID" : "TransactionID";

						break;
					}

				case STANDARD_FIELD_TYPE.PHYSICAL_INVENTORY:
					{
						fieldValue = DisplayForm ? "Physical Inventory" : "PhysicalInventory";

						break;
					}

				case STANDARD_FIELD_TYPE.TOTAL_PHYSICAL_INVENTORY:
					{
						fieldValue = DisplayForm ? "Total Physical Inventory" : "TotalPhysicalInventory";

						break;
					}

				case STANDARD_FIELD_TYPE.TOTAL_VARIANCE:
					{
						fieldValue = DisplayForm ? "Total Variance" : "TotalVariance";

						break;
					}

				case STANDARD_FIELD_TYPE.RECEIPTS:
					{
						fieldValue = "Receipts";
						break;
					}

				case STANDARD_FIELD_TYPE.ADJUSTMENTS:
					{
						fieldValue = "Adjustments";
						break;
					}

				case STANDARD_FIELD_TYPE.DEFUELS:
					{
						fieldValue = "Defuels";
						break;
					}

				case STANDARD_FIELD_TYPE.ISSUES:
					{
						fieldValue = "Issues";
						break;
					}

				case STANDARD_FIELD_TYPE.LOAD_RACK:
					{
						fieldValue = "Load Rack";

						break;
					}

				case STANDARD_FIELD_TYPE.REQUEST:
					{
						fieldValue = "Request";
						break;
					}

				case STANDARD_FIELD_TYPE.ROTATION:
					{
						fieldValue = "Rotation";
						break;
					}

				case STANDARD_FIELD_TYPE.TRANSFERS:
					{
						fieldValue = "Transfers";
						break;
					}

				case STANDARD_FIELD_TYPE.TRANSACTION_TYPE:
					{
						fieldValue = DisplayForm ? "Transaction Type" : "TransactionAlias";

						break;
					}

				case STANDARD_FIELD_TYPE.VOLUME:
					{
						fieldValue = "Volume";
						break;
					}

				case STANDARD_FIELD_TYPE.CONSUMER:
					{
						fieldValue = "Consumer";
						break;
					}

				case STANDARD_FIELD_TYPE.DESTINATION_REGISTRATION_ID:
					{
						fieldValue = DisplayForm ? "Destination Registration ID" : "DestinationRegistrationID";

						break;
					}

				case STANDARD_FIELD_TYPE.SERIAL_NUMBER:
					{
						fieldValue = DisplayForm ? "Serial Number" : "SerialNumber";

						break;
					}

				case STANDARD_FIELD_TYPE.PRODUCT:
					{
						fieldValue = "Product";
						break;
					}

				case STANDARD_FIELD_TYPE.LOCATION:
					{
						fieldValue = "Location";
						break;
					}

				case STANDARD_FIELD_TYPE.INVENTORY:
					{
						fieldValue = "Inventory";
						break;
					}

				case STANDARD_FIELD_TYPE.TEMPERATURE:
					{
						fieldValue = "Temperature";
						break;
					}

				case STANDARD_FIELD_TYPE.DENSITY:
					{
						fieldValue = "Density";
						break;
					}

				case STANDARD_FIELD_TYPE.VCF:
					{
						fieldValue = "VCF";
						break;
					}

				case STANDARD_FIELD_TYPE.SITE:
					{
						fieldValue = "site";
						break;
					}

				case STANDARD_FIELD_TYPE.BILLED_VOLUME:
					{
						fieldValue = DisplayForm ? "Billed Volume" : "BilledVolume";

						break;
					}

				case STANDARD_FIELD_TYPE.MEASURED_VOLUME:
					{
						fieldValue = DisplayForm ? "Measured Volume" : "MeasuredVolume";

						break;
					}

				case STANDARD_FIELD_TYPE.BILL_OF_LADING_NUMBER:
					{
						fieldValue = DisplayForm ? "Bill Of Lading Number" : "BillOfLadingNumber";

						break;
					}

				case STANDARD_FIELD_TYPE.BOOK_RECEIPTS:
					{
						fieldValue = DisplayForm ? "Book Receipts" : "BookReceipts";

						break;
					}

				case STANDARD_FIELD_TYPE.ASSIGNED:
					{
						fieldValue = "Assigned";
						break;
					}

				case STANDARD_FIELD_TYPE.REMAINING:
					{
						fieldValue = "Remaining";
						break;
					}

				case STANDARD_FIELD_TYPE.OWNER:
					{
						fieldValue = DisplayForm ? "Owner" : "OwnerID";
						break;
					}

				case STANDARD_FIELD_TYPE.MANAGER:
					{
						fieldValue = DisplayForm ? "Manager" : "ManagerID";
						break;
					}

				case STANDARD_FIELD_TYPE.BILLTOID:
					{
						fieldValue = DisplayForm ? "Bill-To" : "BillToID";

						break;
					}

				case STANDARD_FIELD_TYPE.SHIPTOID:
					{
						fieldValue = DisplayForm ? "Ship-To" : "ShipToID";
						break;
					}

				case STANDARD_FIELD_TYPE.TRANSACTION_ALIAS:
					{
						fieldValue = DisplayForm ? "Transaction Alias" : "TransactionAlias";

						break;
					}

				case STANDARD_FIELD_TYPE.DOCUMENT_NUMBER:
					{
						fieldValue = DisplayForm ? "Document Number" : "DocumentNumber";

						break;
					}

				case STANDARD_FIELD_TYPE.PO_NUMBER:
					{
						fieldValue = DisplayForm ? "PO Number" : "PONumber";

						break;
					}

				case STANDARD_FIELD_TYPE.SCHEDULED_DATE:
					{
						fieldValue = DisplayForm ? "Scheduled Date" : "ScheduledDate";
						break;
					}

				case STANDARD_FIELD_TYPE.ORDER_STATUS:
					{
						fieldValue = DisplayForm ? "Order Status" : "LookupTransactionStatusIndex";
						break;
					}

				case STANDARD_FIELD_TYPE.EFFECTIVE_DATE:
					{
						fieldValue = DisplayForm ? "Effective Date" : "EffectiveDate";
						break;
					}

				case STANDARD_FIELD_TYPE.EXPIRATION_DATE:
					{
						fieldValue = DisplayForm ? "Expiration Date" : "ExpirationDate";
						break;
					}

				case STANDARD_FIELD_TYPE.ETA:
					{
						fieldValue = "ETA";
						break;
					}

				case STANDARD_FIELD_TYPE.BOL_NUMBER:
					{
						fieldValue = DisplayForm ? "BOL Number" : "DocumentNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.BOL_STATUS:
					{
						fieldValue = DisplayForm ? "Status" : "LookupTransactionStatusIndex";
						break;
					}

				case STANDARD_FIELD_TYPE.BOL_DATE_TIME:
					{
						fieldValue = DisplayForm ? "Date Time" : "TransDateTime";

						break;
					}

				case STANDARD_FIELD_TYPE.BOL_CARRIER:
					{
						fieldValue = DisplayForm ? "Carrier" : "CarrierID";

						break;
					}

				case STANDARD_FIELD_TYPE.SHIPPER:
					{
						fieldValue = DisplayForm ? "Shipper" : "ShipperID";

						break;
					}

				case STANDARD_FIELD_TYPE.BOL_MANAGER:
					{
						fieldValue = DisplayForm ? "Manager" : "ManagerID";

						break;
					}

				case STANDARD_FIELD_TYPE.BOL_OWNER:
					{
						fieldValue = DisplayForm ? "Owner" : "OwnerID";

						break;
					}

				case STANDARD_FIELD_TYPE.TOTAL_ACTIVITY:
					{
						fieldValue = DisplayForm ? "Total Activity" : "TotalActivity";

						break;
					}

				case STANDARD_FIELD_TYPE.CLOSEOUT_DATE:
					{
						fieldValue = DisplayForm ? "Closeout Date" : "CloseoutDate";

						break;
					}

				case STANDARD_FIELD_TYPE.ESTIMATED_DATE_FROM:
					{
						fieldValue = DisplayForm ? "Estimated Delivery Date From" : "EstimatedDeliveryDateFrom";
						break;
					}

				case STANDARD_FIELD_TYPE.ESTIMATED_DATE_TO:
					{
						fieldValue = DisplayForm ? "Estimated Delivery Date To" : "EstimatedDeliveryDateTo";
						break;
					}

				case STANDARD_FIELD_TYPE.ORDER_CONFIRM_NUMBER:
					{
						fieldValue = DisplayForm ? "Confirmation Number" : "ConfirmationNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.STANDING_OFFER_NUMBER:
					{
						fieldValue = DisplayForm ? "Price List Number" : "StandingOfferNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.REQUIRED_DATE:
					{
						fieldValue = DisplayForm ? "Required Delivery Date" : "RequiredDeliveryDate";
						break;
					}

				case STANDARD_FIELD_TYPE.SUPPLIER:
					{
						fieldValue = DisplayForm ? "Supplier" : "SupplierID";
						break;
					}

				case STANDARD_FIELD_TYPE.REQUESTED_DELIVERY_DATE:
					{
						fieldValue = DisplayForm ? "Requested Delivery Date" : "RequestedDeliveryDate";

						break;
					}

				case STANDARD_FIELD_TYPE.SHIPMENT_NUMBER:
					{
						fieldValue = DisplayForm ? "Shipment Number" : "ShipmentNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_1:
					{
						fieldValue = DisplayForm ? "User Data 1" : "UserData1";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_2:
					{
						fieldValue = DisplayForm ? "User Data 2" : "UserData2";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_3:
					{
						fieldValue = DisplayForm ? "User Data 3" : "UserData3";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_4:
					{
						fieldValue = DisplayForm ? "User Data 4" : "UserData4";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_5:
					{
						fieldValue = DisplayForm ? "User Data 5" : "UserData5";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_6:
					{
						fieldValue = DisplayForm ? "User Data 6" : "UserData6";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_7:
					{
						fieldValue = DisplayForm ? "User Data 7" : "UserData7";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_8:
					{
						fieldValue = DisplayForm ? "User Data 8" : "UserData8";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_9:
					{
						fieldValue = DisplayForm ? "User Data 9" : "UserData9";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_10:
					{
						fieldValue = DisplayForm ? "User Data 10" : "UserData10";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_11:
					{
						fieldValue = DisplayForm ? "User Data 11" : "UserData11";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_12:
					{
						fieldValue = DisplayForm ? "User Data 12" : "UserData12";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_13:
					{
						fieldValue = DisplayForm ? "User Data 13" : "UserData13";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_14:
					{
						fieldValue = DisplayForm ? "User Data 14" : "UserData14";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_15:
					{
						fieldValue = DisplayForm ? "User Data 15" : "UserData15";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_16:
					{
						fieldValue = DisplayForm ? "User Data 16" : "UserData16";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_17:
					{
						fieldValue = DisplayForm ? "User Data 17" : "UserData17";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_18:
					{
						fieldValue = DisplayForm ? "User Data 18" : "UserData18";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_19:
					{
						fieldValue = DisplayForm ? "User Data 19" : "UserData19";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_20:
					{
						fieldValue = DisplayForm ? "User Data 20" : "UserData20";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_21:
					{
						fieldValue = DisplayForm ? "User Data 21" : "UserData21";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_22:
					{
						fieldValue = DisplayForm ? "User Data 22" : "UserData22";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_23:
					{
						fieldValue = DisplayForm ? "User Data 23" : "UserData23";
						break;
					}

				case STANDARD_FIELD_TYPE.USER_DATA_24:
					{
						fieldValue = DisplayForm ? "User Data 24" : "UserData24";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_01:
					{
						fieldValue = DisplayForm ? "Line Item User Data 1" : "LineItemUserData1";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_02:
					{
						fieldValue = DisplayForm ? "User Data 2" : "UserData2";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_03:
					{
						fieldValue = DisplayForm ? "Line Item User Data 3" : "LineItemUserData3";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_04:
					{
						fieldValue = DisplayForm ? "Line Item User Data 4" : "LineItemUserData4";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_05:
					{
						fieldValue = DisplayForm ? "Line Item User Data 5" : "LineItemUserData5";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_06:
					{
						fieldValue = DisplayForm ? "Line Item User Data 6" : "LineItemUserData6";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_07:
					{
						fieldValue = DisplayForm ? "Line Item User Data 7" : "LineItemUserData7";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_08:
					{
						fieldValue = DisplayForm ? "Line Item User Data 8" : "LineItemUserData8";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_09:
					{
						fieldValue = DisplayForm ? "Line Item User Data 9" : "LineItemUserData9";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_10:
					{
						fieldValue = DisplayForm ? "Line Item User Data 10" : "LineItemUserData10";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_11:
					{
						fieldValue = DisplayForm ? "Line Item User Data 11" : "LineItemUserData11";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_12:
					{
						fieldValue = DisplayForm ? "Line Item User Data 12" : "LineItemUserData12";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_13:
					{
						fieldValue = DisplayForm ? "Line Item User Data 13" : "LineItemUserData13";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_14:
					{
						fieldValue = DisplayForm ? "Line Item User Data 14" : "LineItemUserData14";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_15:
					{
						fieldValue = DisplayForm ? "Line Item User Data 15" : "LineItemUserData15";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_16:
					{
						fieldValue = DisplayForm ? "Line Item User Data 16" : "LineItemUserData16";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_17:
					{
						fieldValue = DisplayForm ? "Line Item User Data 17" : "LineItemUserData17";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_18:
					{
						fieldValue = DisplayForm ? "Line Item User Data 18" : "LineItemUserData18";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_19:
					{
						fieldValue = DisplayForm ? "Line Item User Data 19" : "LineItemUserData19";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_20:
					{
						fieldValue = DisplayForm ? "Line Item User Data 20" : "LineItemUserData20";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_21:
					{
						fieldValue = DisplayForm ? "Line Item User Data 21" : "LineItemUserData21";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_22:
					{
						fieldValue = DisplayForm ? "Line Item User Data 22" : "LineItemUserData22";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_23:
					{
						fieldValue = DisplayForm ? "Line Item User Data 23" : "LineItemUserData23";
						break;
					}

				case STANDARD_FIELD_TYPE.LINE_ITEM_USER_DATA_24:
					{
						fieldValue = DisplayForm ? "Line Item User Data 24" : "LineItemUserData24";
						break;
					}

				case STANDARD_FIELD_TYPE.OPERATORID:
					{
						fieldValue = DisplayForm ? "Operator" : "OperatorID";
						break;
					}

				case STANDARD_FIELD_TYPE.DESTEQUIPMENT1ID:
					{
						fieldValue = DisplayForm ? "Equipment 1" : "DestRegistrationID1";
						break;
					}

				case STANDARD_FIELD_TYPE.DESTEQUIPMENT2ID:
					{
						fieldValue = DisplayForm ? "Equipment 2" : "DestRegistrationID2";
						break;
					}

				case STANDARD_FIELD_TYPE.DESTEQUIPMENT3ID:
					{
						fieldValue = DisplayForm ? "Equipment 3" : "DestRegistrationID3";
						break;
					}

				// vthompson 8/5/2008
				case STANDARD_FIELD_TYPE.EXCISE:
					{
						fieldValue = "Excise";
						break;
					}

				case STANDARD_FIELD_TYPE.COST_CENTRE_CODE:
					{
						fieldValue = DisplayForm ? "Cost Centre Code" : "CostCentreCode";
						break;
					}

				case STANDARD_FIELD_TYPE.PRODUCT_PRICE:
					{
						fieldValue = DisplayForm ? "Product Price" : "ProductPrice";
						break;
					}

				case STANDARD_FIELD_TYPE.GROSS_QUANTITY:
					{
						fieldValue = DisplayForm ? "Gross Quantity" : "GrossQuantity";
						break;
					}

				case STANDARD_FIELD_TYPE.NET_QUANTITY:
					{
						fieldValue = DisplayForm ? "Net Quantity" : "NetQuantity";
						break;
					}

				case STANDARD_FIELD_TYPE.MASSQUANTITY:
					{
						fieldValue = DisplayForm ? "Mass Quantity" : "MassQuantity";
						break;
					}

				case STANDARD_FIELD_TYPE.BATCH_NUMBER:
					{
						fieldValue = DisplayForm ? "Batch Number" : "BatchNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.ORDER_NUMBER:
					{
						fieldValue = DisplayForm ? "Order Number" : "OrderNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.PAYMENT_NUMBER:
					{
						fieldValue = DisplayForm ? "Payment Number" : "PaymentNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.REBATE_FLAG:
					{
						fieldValue = "Rebate";
						break;
					}

				case STANDARD_FIELD_TYPE.TOTAL_AMOUNT:
					{
						fieldValue = DisplayForm ? "Total Amount" : "TotalAmount";
						break;
					}

				case STANDARD_FIELD_TYPE.ACCOUNT_CODE:
					{
						fieldValue = DisplayForm ? "Account Code" : "AccountCode";
						break;
					}

				case STANDARD_FIELD_TYPE.LEGACY_NUMBER:
					{
						fieldValue = DisplayForm ? "Legacy Number" : "LegacyNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.CONTACT_FIRST_NAME:
					{
						fieldValue = DisplayForm ? "Contact First Name" : "ContactFirstName";
						break;
					}

				case STANDARD_FIELD_TYPE.CONTACT_INFO:
					{
						fieldValue = DisplayForm ? "Contact Info" : "ContactInfo";
						break;
					}

				case STANDARD_FIELD_TYPE.CONTACT_SURNAME:
					{
						fieldValue = DisplayForm ? "Contact Surname" : "ContactSurname";
						break;
					}

				case STANDARD_FIELD_TYPE.GST:
					{
						fieldValue = "GST";
						break;
					}

				case STANDARD_FIELD_TYPE.INVOICE_NUMBER:
					{
						fieldValue = DisplayForm ? "Invoice Number" : "InvoiceNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.VOUCHER_NUMBER:
					{
						fieldValue = DisplayForm ? "Voucher Number" : "VoucherNumber";
						break;
					}

				case STANDARD_FIELD_TYPE.NET_MANUALVALUE:
					{
						fieldValue = DisplayForm ? "Net Manual Value" : "NetManualValueFlag";
						break;
					}
				case STANDARD_FIELD_TYPE.MASS_MANUALVALUE:
					{
						fieldValue = DisplayForm ? "Mass Manual Value" : "MassManualValueFlag";
						break;
					}
				case STANDARD_FIELD_TYPE.GROSS_MANUALVALUE:
					{
						fieldValue = DisplayForm ? "Gross Manual Value" : "GrossManualValueFlag";
						break;
					}
				case STANDARD_FIELD_TYPE.VCF_MANUALVALUE:
					{
						fieldValue = DisplayForm ? "Vcf Manual Value" : "VcfManualValueFlag";
						break;
					}

				case STANDARD_FIELD_TYPE.ALTERNATIVE_NET_VOLUME:
					{
						fieldValue = DisplayForm ? "Alternative Net Volume" : "AlternativeNetVolume";
						break;
					}

				case STANDARD_FIELD_TYPE.TOLERANCE:
					{
						fieldValue = DisplayForm ? "Tolerance Percentage" : "Tolerance";
						break;
					}

				case STANDARD_FIELD_TYPE.ALLOWED_GAIN_LOSS:
					{
						fieldValue = DisplayForm ? "Allowed Gain/Loss" : "AllowableGainLoss";
						break;
					}

				case STANDARD_FIELD_TYPE.VARIANCE_PERCENTAGE:
					{
						fieldValue = DisplayForm ? "Variance Percentage" : "VariancePercentage";
						break;
					}

				case STANDARD_FIELD_TYPE.ROTATES_BACKWARDS:
					{
						fieldValue = DisplayForm ? "Rotates Backwards" : "RotatesBackwardsFlag";
						break;
					}
				case STANDARD_FIELD_TYPE.METER_TOTAL:
					{
						fieldValue = DisplayForm ? "Meter Difference" : "MeterTotal";
						break;
					}
				case STANDARD_FIELD_TYPE.TRANSACTION_METER_TOTAL:
					{
						fieldValue = DisplayForm ? "Transaction Meter Total" : "TransactionMeterTotal";
						break;
					}
				case STANDARD_FIELD_TYPE.METER_VARIANCE:
					{
						fieldValue = DisplayForm ? "Meter Variance" : "MeterVariance";
						break;
					}
				case STANDARD_FIELD_TYPE.METER_RECONCILIATION_ERROR:
					{
						fieldValue = DisplayForm ? "Error" : "IsError";
						break;
					}
				case STANDARD_FIELD_TYPE.METER_SKIP:
					{
						fieldValue = DisplayForm ? "Meter Skip" : "MeterSkip";
						break;
					}
				case STANDARD_FIELD_TYPE.FLIGHT_NUMBER:
					{
						fieldValue = DisplayForm ? "Flight Number" : "FlightNumber";
						break;
					}
				case STANDARD_FIELD_TYPE.TICKET_NUMBER:
					{
						fieldValue = DisplayForm ? "Ticket Number" : "TicketNumber";
						break;
					}
				case STANDARD_FIELD_TYPE.METER_ID:
					{
						fieldValue = DisplayForm ? "Meter ID" : "MeterID";
						break;
					}
				case STANDARD_FIELD_TYPE.STATION:
					{
						fieldValue = DisplayForm ? "Station ID" : "StationID";
						break;
					}
				case STANDARD_FIELD_TYPE.VIEW_DETAILS:
					{
						fieldValue = DisplayForm ? "View Details" : "ViewDetails";
						break;
					}

				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_RULE_ID:
					fieldValue = DisplayForm ? "Rule ID" : "RuleID";
					break;

				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_RULE_DESCRIPTION:
					fieldValue = "Description";
					break;

				case STANDARD_FIELD_TYPE.ENABLED:
					fieldValue = "Enabled";
					break;

				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_DEFAULT_EOM:
					fieldValue = DisplayForm ? "Default EOM" : "DefaultEOM";
					break;

				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_TRANSACTION_ALIAS:
					fieldValue = DisplayForm ? "Transaction Alias" : "TransactionAliasName";
					break;

				case STANDARD_FIELD_TYPE.REASON_CODE:
					fieldValue = DisplayForm ? "Reason Code" : "DefaultReasonCodeString";
					break;

				case STANDARD_FIELD_TYPE.MANAGERS:
					fieldValue = DisplayForm ? "Managers" : "ManagerList";
					break;

				case STANDARD_FIELD_TYPE.PRODUCTS:
					fieldValue = DisplayForm ? "Products" : "ProductList";
					break;

				case STANDARD_FIELD_TYPE.AUTO_DISTRIBUTION_TRANSACTION_ALIASES:
					fieldValue = DisplayForm ? "Throughput Transaction Aliases" : "TransactionAliasList";
					break;

				case STANDARD_FIELD_TYPE.OWNERS:
					fieldValue = DisplayForm ? "Owners" : "OwnerList";
					break;

				case STANDARD_FIELD_TYPE.SITE_GUID:
					fieldValue = "SiteGuid";
					break;

				case STANDARD_FIELD_TYPE.IDENTITY_GUID:
					fieldValue = "IdentityGuid";
					break;
				case STANDARD_FIELD_TYPE.DELETE_FLAG:
					fieldValue = "DeleteFlag";
					break;
				case STANDARD_FIELD_TYPE.REVERSAL_TYPE:
					{
						fieldValue = DisplayForm ? "Reversal Type" : "ReversalType";
						break;
					}
				case STANDARD_FIELD_TYPE.DESTINATIONSERIALNUMBER1:
					fieldValue = DisplayForm ? DESTINATION_SERIAL_NUMBER_1_DISPLAY : "DestinationSerialNumber1";
					break;
				case STANDARD_FIELD_TYPE.DESTINATIONSERIALNUMBER2:
					fieldValue = DisplayForm ? DESTINATION_SERIAL_NUMBER_2_DISPLAY : "DestinationSerialNumber2";
					break;
				case STANDARD_FIELD_TYPE.DESTINATIONSERIALNUMBER3:
					fieldValue = DisplayForm ? DESTINATION_SERIAL_NUMBER_3_DISPLAY : "DestinationSerialNumber3";
					break;
				case STANDARD_FIELD_TYPE.TRANSACTION_VOLUME_TOTAL:
					{
						fieldValue = DisplayForm ? "Transaction Volume Total" : "TransactionVolumeTotal";
						break;
					}
				case STANDARD_FIELD_TYPE.VOLUME_VARIANCE:
					{
						fieldValue = DisplayForm ? "Volume Variance" : "VolumeVariance";
						break;
					}
				default:
					{
						fieldValue = "Undefined";
						break;
					}
			}

			return fieldValue;
		}


		/// <summary>
		/// This method will reset the List View Field object to its initial state.
		/// </summary>
		public override void Reset()
		{
			this.Initialize();
		}

		/// <summary>
		/// This method will load the List View Field data retrieved from the database.
		/// </summary>
		/// <param name="o">
		/// The field object.
		/// </param>
		public override void Load(object o)
		{
			this.Reset();

			if (o is DataSet set)
			{
				DataTable table = set.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				DataRow row = table.Rows[0];

				this._IdentityGuid = DataObject.getValue<Guid>(row["ListViewFieldGuid"], Guid.Empty);
				this._ListViewGuid = DataObject.getValue<Guid>(row["ListViewGuid"], Guid.Empty);
				this._Type = DataObject.getValue<LISTVIEW_FIELD_TYPE>(row["LookupListViewFieldTypeIndex"], LISTVIEW_FIELD_TYPE.TYPE_MAX);
				this._StandardFieldType = STANDARD_FIELD_TYPE.TYPE_MAX;

				if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS)
				{
					this._TypeGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
					this._ID = DataObject.getValue<string>(row["AliasName"], string.Empty);
					this._DataPath = "QuantityList[" + this._ID + "]";
				}
				else if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD)
				{
					this._TypeGuid = DataObject.getValue<Guid>(row["TransactionAliasFieldGuid"], Guid.Empty);
					this.TransactionAliasGuid = DataObject.getValue<Guid>(row["TransactionAliasGuid"], Guid.Empty);
					this._ID = DataObject.getValue<string>(row["AliasFieldName"], string.Empty);
					this._DataPath = DataObject.getValue<string>(row["AliasFieldDbName"], string.Empty);

					this.virtualField = DataObject.getValue<bool>(row["VirtualField"], false);

					bool isLineItem = DataObject.getValue<TransactionFieldType>(row["AliasFieldType"], TransactionFieldType.LineItem)
												== TransactionFieldType.LineItem;

					// Add "Item" prefix to select columns duplicated in the line/sub line item tables
					if ((this._DataPath == "DeletedFlag" || this._DataPath == "TransactionStatus") && isLineItem)
					{
						this._DataPath = "Item" + this._DataPath;
					}

					// Add "Item" prefix to select columns duplicated in the line/sub line item tables 
					if (((this._DataPath == "Number01")
						|| (this._DataPath == "Number02")
						|| (this._DataPath == "Number03")
						|| (this._DataPath == "Number04")
						|| (this._DataPath == "Number05")
						|| (this._DataPath == "Flag01")
						|| (this._DataPath == "Flag02")
						|| (this._DataPath == "Flag03")
						|| (this._DataPath == "Flag04")
						|| (this._DataPath == "Flag05")
						|| (this._DataPath == "Flag06")
						|| (this._DataPath == "Date01")
						|| (this._DataPath == "Date02")
						|| (this._DataPath == "Date03")
						|| (this._DataPath == "Date04")) && isLineItem)
					{
						this._DataPath = "Item" + this._DataPath;
					}
				}
				else if (this._Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD)
				{
					this._TypeGuid = DataObject.getValue<Guid>(row["UserDataFieldTransactionAliasGuid"], Guid.Empty);
					this._ID = DataObject.getValue<string>(row["UserDataName"], string.Empty);
					this._DataPath = "UserData" + (DataObject.getValue<byte>(row["UserDataNumber"], 0) + 1);
				}
				else if (this._Type == LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD)
				{
					this._TypeGuid = DataObject.getValue<Guid>(row["UserDataFieldTransactionAliasLineItemGuid"], Guid.Empty);
					this._ID = DataObject.getValue<string>(row["LineItemUserDataName"], string.Empty);
					this._DataPath = "LineItemUserData" + (DataObject.getValue<byte>(row["LineItemUserDataNumber"], 0) + 1);
				}
				else if (this._Type == LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD)
				{
					this._TypeGuid = DataObject.getValue<Guid>(row["LedgerAggregateColumnGuid"], Guid.Empty);
					this._ID = DataObject.getValue<string>(row["AggregateID"], "0");
					this._DataPath = "QuantityList[" + this._ID + "]";
				}
				else
				{
					// Otherwise _Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD
					this._StandardFieldType = DataObject.getValue<STANDARD_FIELD_TYPE>(row["LookupStandardFieldTypeIndex"], STANDARD_FIELD_TYPE.TYPE_MAX);
					this._TypeGuid = GetGuidFromStandardFieldType(this._StandardFieldType);
					this._ID = StandardFieldTypeID(this.StandardFieldType, true);
					this._DataPath = StandardFieldTypeID(this.StandardFieldType, false);
				}

				this.ListViewID = DataObject.getValue<string>(row["ListViewID"], string.Empty);
				this._ColumnOrder = DataObject.getValue<int>(row["ColumnOrder"], 0);
				this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
				this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
				this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
				this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
			}
			else if (o is ListViewFieldClass listViewField)
			{
				this._IdentityGuid = listViewField.IdentityGuid;
				this._ID = listViewField.ID;
				this._ListViewGuid = listViewField.ListViewGuid;
				this._Type = listViewField.Type;
				this._TypeGuid = listViewField.TypeGuid;
				this._StandardFieldType = listViewField.StandardFieldType;
				this._ColumnOrder = listViewField.ColumnOrder;
				this._DataPath = listViewField.DataPath;
				this._CreatedDate = listViewField.CreatedDate;
				this._CreatedBy = listViewField.CreatedBy;
				this._UpdatedDate = listViewField.UpdatedDate;
				this._UpdatedBy = listViewField.UpdatedBy;
			}
			else
			{
				base.Load(o);
			}
		}

		#region paramaterized SQL
		/// <summary>
		/// The insert SQL.
		/// </summary>
		/// <param name="cmd">
		/// The command.
		/// </param>
		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblListViewFields (ListViewGuid, LookupListViewFieldTypeIndex,";

			if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS)
			{
				cmd.CommandText += "TransactionAliasGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD)
			{
				cmd.CommandText += "TransactionAliasFieldGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD)
			{
				cmd.CommandText += "UserDataFieldTransactionAliasGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD)
			{
				cmd.CommandText += "UserDataFieldTransactionAliasLineItemGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD)
			{
				cmd.CommandText += "LedgerAggregateColumnGuid,";
			}
			else
			{
				// Otherwise _Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD
				cmd.CommandText += "LookupStandardFieldTypeIndex,";
			}

			cmd.CommandText +=
					"ListViewID," +
					"ColumnOrder," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy," +
					"ListViewFieldGuid" +
					") VALUES (" +
					"@ListViewGuid," +
					"@LookupListViewFieldTypeIndex,";

			if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS)
			{
				cmd.CommandText += "@TransactionAliasGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD)
			{
				cmd.CommandText += "@TransactionAliasFieldGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD)
			{
				cmd.CommandText += "@UserDataFieldTransactionAliasGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD)
			{
				cmd.CommandText += "@UserDataFieldTransactionAliasLineItemGuid,";
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD)
			{
				cmd.CommandText += "@LedgerAggregateColumnGuid,";
			}
			else
			{
				// Otherwise _Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD
				cmd.CommandText += "@LookupStandardFieldTypeIndex,";
			}

			cmd.CommandText +=
					"@ListViewID," +
					"@ColumnOrder," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy," +
					"@ListViewFieldGuid)";

			cmd.Parameters.AddWithValue("@ListViewGuid", this._ListViewGuid);
			cmd.Parameters.AddWithValue("@LookupListViewFieldTypeIndex", (int)this._Type);

			if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.TRANSACTION_ALIAS_FIELD)
			{
				cmd.Parameters.AddWithValue("@TransactionAliasFieldGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.USER_DATA_FIELD)
			{
				cmd.Parameters.AddWithValue("@UserDataFieldTransactionAliasGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.LINE_ITEM_USER_DATA_FIELD)
			{
				cmd.Parameters.AddWithValue("@UserDataFieldTransactionAliasLineItemGuid", this._TypeGuid);
			}
			else if (this._Type == LISTVIEW_FIELD_TYPE.AGGREGATE_FIELD)
			{
				cmd.Parameters.AddWithValue("@LedgerAggregateColumnGuid", this._TypeGuid);
			}
			else
			{
				// Otherwise _Type == LISTVIEW_FIELD_TYPE.STANDARD_FIELD
				cmd.Parameters.AddWithValue("@LookupStandardFieldTypeIndex", (int)this._StandardFieldType);
			}

			cmd.Parameters.AddWithValue("@ListViewID", this.ListViewID);
			cmd.Parameters.AddWithValue("@ColumnOrder", this._ColumnOrder);
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@ListViewFieldGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The update SQL.
		/// </summary>
		/// <param name="cmd">
		/// The command.
		/// </param>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblListViewFields SET " +
				"ColumnOrder = @ColumnOrder," +
				"UpdatedDate = @UpdatedDate," +
				"UpdatedBy = @UpdatedBy," +
				"ListViewID = @ListViewID " +
				"WHERE ListViewFieldGuid = @ListViewFieldGuid";

			cmd.Parameters.AddWithValue("@ColumnOrder", this._ColumnOrder);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			cmd.Parameters.AddWithValue("@ListViewID", this.ListViewID);
			cmd.Parameters.AddWithValue("@ListViewFieldGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The purge SQL.
		/// </summary>
		/// <param name="cmd">
		/// The command.
		/// </param>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblListViewFields WHERE ListViewFieldGuid = @ListViewFieldGuid";
			cmd.Parameters.AddWithValue("@ListViewFieldGuid", this._IdentityGuid);
		}

		/// <summary>
		/// The select sql.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void SelectSQL(SqlCommand command, bool inTransaction)
		{
			command.CommandText = this.SelectClause
						 + " FROM tblListViewFields lvf " + SQLUpdateLock(inTransaction)
						 + " LEFT OUTER JOIN tblTransactionAliases ta ON lvf.TransactionAliasGuid = ta.TransactionAliasGuid AND lvf.LookupListViewFieldTypeIndex = 1 "
						 + " LEFT OUTER JOIN tblTransactionAliasFields taf ON lvf.LookupListViewFieldTypeIndex = taf.LookupTransactionFieldTypeIndex AND lvf.LookupListViewFieldTypeIndex = 2 "
						 + " LEFT OUTER JOIN tblUserDataFieldTransactionAlias udf1 ON lvf.UserDataFieldTransactionAliasGuid = udf1.UserDataFieldTransactionAliasGuid AND lvf.LookupListViewFieldTypeIndex = 3 "
						 + " LEFT OUTER JOIN tblUserDataFieldTransactionAliasLineItem udf2 ON lvf.UserDataFieldTransactionAliasLineItemGuid = udf2.UserDataFieldTransactionAliasLineItemGuid AND lvf.LookupListViewFieldTypeIndex = 5 "
						 + " LEFT OUTER JOIN tblLedgerAggregateColumns lac ON lvf.LedgerAggregateColumnGuid = lac.LedgerAggregateColumnGuid "
						 + " WHERE lvf.ListViewFieldGuid = @ListViewFieldGuid ";

			var parm = new SqlParameter("@ListViewFieldGuid", SqlDbType.UniqueIdentifier) { Value = this.IdentityGuid };
			command.Parameters.Add(parm);
		}

		/// <summary>
		/// The enumerate sql.
		/// </summary>
		/// <param name="command"></param>
		/// <param name="inTransaction">The in transaction.</param>
		public void EnumerateSQL(SqlCommand command, bool inTransaction)
		{
			command.CommandText = this.SelectClause
						 + " FROM tblListViewFields lvf " + SQLUpdateLock(inTransaction)
						 + " LEFT OUTER JOIN tblTransactionAliases ta ON lvf.TransactionAliasGuid = ta.TransactionAliasGuid AND lvf.LookupListViewFieldTypeIndex = 1 "
						 + " LEFT OUTER JOIN tblTransactionAliasFields taf ON lvf.LookupListViewFieldTypeIndex = taf.LookupTransactionFieldTypeIndex AND lvf.LookupListViewFieldTypeIndex = 2 "
						 + " LEFT OUTER JOIN tblUserDataFieldTransactionAlias udf1 ON lvf.UserDataFieldTransactionAliasGuid = udf1.UserDataFieldTransactionAliasGuid AND lvf.LookupListViewFieldTypeIndex = 3 "
						 + " LEFT OUTER JOIN tblUserDataFieldTransactionAliasLineItem udf2 ON lvf.UserDataFieldTransactionAliasLineItemGuid = udf2.UserDataFieldTransactionAliasLineItemGuid AND lvf.LookupListViewFieldTypeIndex = 5 "
						 + " LEFT OUTER JOIN tblLedgerAggregateColumns lac ON lvf.LedgerAggregateColumnGuid = lac.LedgerAggregateColumnGuid "
						 + " WHERE ListViewGuid = @ListViewGuid "
						 + " ORDER BY ColumnOrder ";

			var parm = new SqlParameter("@ListViewGuid", SqlDbType.UniqueIdentifier) { Value = this.ListViewGuid };
			command.Parameters.Add(parm);
		}
		#endregion

		/// <summary>
		/// The initialize.
		/// </summary>
		private void Initialize()
		{
			base.Reset();

			this._ListViewGuid = Guid.Empty;
			this._Type = LISTVIEW_FIELD_TYPE.TYPE_MAX;
			this._TypeGuid = Guid.Empty;
			this._StandardFieldType = STANDARD_FIELD_TYPE.TYPE_MAX;
			this._ColumnOrder = 0;
			this._DataPath = string.Empty;
		}
	}
}
