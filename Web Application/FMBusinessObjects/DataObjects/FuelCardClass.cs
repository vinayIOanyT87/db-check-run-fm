
namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    using FMBusinessObjects.BusinessInterfaces;
    using FMBusinessObjects.ChannelFactories;
    using FMBusinessObjects.Constants;
    using FMBusinessObjects.UtilityObjects;

    using FMCore;

    /// <summary>
    /// Summary description for FuelCardCollectionClass.
    /// </summary>
    [Serializable]
   [CollectionDataContract]
	public class FuelCardCollectionClass : List<FuelCardClass> { }

	/// <summary>
	/// Summary description for FuelCard.
	/// </summary>
	[Serializable()]
	[EntityImportExportWorksheetAttribute("FUEL CARD")]
	[DataContract]
	[QueryWriterTopic(typeof(FuelCardClass), "Fuel Cards")]
	[QueryWriterTopicSecurity(RIGHT.VIEW_FUEL_CARD_DATA)]
	[QueryWriterTopicSecurity(RIGHT.MODIFY_FUEL_CARD_DATA)]
    public class FuelCardClass : FMBaseDataObjectWithUserData
	{
		public enum Filters { COMPANY, TEXT, NONE };
		public enum Statuses { ACTIVE, INACTIVE, CANCELLED, LOCKED, LOSTSTOLEN };

		static public string[] STATUS_NAMES = { "Active", "Inactive", "Cancelled", "Locked", "LostStolen" };
		public const string ENTITY_TYPE_ID = "Fuel Card";

		static public string SELECT_SQL(bool bInTransaction, int a_limit)
		{
            string select = "declare @CompanyGuidTable TABLE (CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
                            "INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid)" + Environment.NewLine +
                            "SELECT DISTINCT ";
			if (a_limit > 0)
				select +=   "TOP " + a_limit + " ";

			return select + " tblFuelCards.*, " +
							"shipto.ID as ShipToID," +
							"shipto.Code as ShipToCode," +
							"shipto.Name as ShipToName," +
							"shipto.Address1 as ShipToAddress," +
							"shipto.City as ShipToCity," +
							"shipto.State as ShipToState," +
							"billto.ID as BillToID," +
							"billto.Code as BillToCode," +
							"billto.Name as BillToName," +
							"billto.Address1 as BillToAddress," +
							"billto.City as BillToCity," +
							"billto.State as BillToState," +
							"shipper.ID as ShipperID," +
							"shipper.Code as ShipperCode," +
							"shipper.Name as ShipperName," +
							"shipper.Address1 as ShipperAddress," +
							"shipper.City as ShipperCity," +
							"shipper.State as ShipperState," +
							"owner.ID as OwnerID," +
							"owner.Code as OwnerCode," +
							"owner.Name as OwnerName," +
							"owner.Address1 as OwnerAddress," +
							"owner.City as OwnerCity," +
							"owner.State as OwnerState," +
							"manager.ID as ManagerID," +
							"manager.Code as ManagerCode," +
							"manager.Name as ManagerName," +
							"manager.Address1 as ManagerAddress," +
							"manager.City as ManagerCity," +
							"manager.State as ManagerState, " +
                            "appstr.ID as FuelCardTypeApplicationStringID  " + 
                            "FROM [dbo].[tblFuelCards] " +
                            "LEFT JOIN [dbo].[tblApplicationString] appstr " +
                            " ON [dbo].[tblFuelCards].[FuelCardTypeApplicationStringGuid] = appstr.[ApplicationStringGuid] " +
							"LEFT JOIN (select * from tblCompanies " + SQLUpdateLock(bInTransaction) + " where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) shipto ON tblFuelCards.ShipToCompanyGuid = shipto._MasterRecordGuid " +
							"LEFT JOIN (select * from tblCompanies " + SQLUpdateLock(bInTransaction) + " where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) billto ON tblFuelCards.BillToCompanyGuid = billto._MasterRecordGuid " +
							"LEFT JOIN (select * from tblCompanies " + SQLUpdateLock(bInTransaction) + " where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) shipper ON tblFuelCards.ShipperCompanyGuid = shipper._MasterRecordGuid " +
							"LEFT JOIN (select * from tblCompanies " + SQLUpdateLock(bInTransaction) + " where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) owner ON tblFuelCards.OwnerCompanyGuid = owner._MasterRecordGuid " +
							"LEFT JOIN (select * from tblCompanies " + SQLUpdateLock(bInTransaction) + " where tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) manager ON tblFuelCards.ManagerCompanyGuid = manager._MasterRecordGuid ";
		}


		protected string provider;
		protected Statuses status;
		protected Guid shipToGuid;
		protected string shipToID;
		protected string shipToCode;
		protected string shipToName;
		protected string shipToAddress;
		protected string shipToCity;
		protected string shipToState;
		protected Guid billToGuid;
		protected string billToID;
		protected string billToCode;
		protected string billToName;
		protected string billToAddress;
		protected string billToCity;
		protected string billToState;
		protected Guid shipperGuid;
		protected string shipperID;
		protected string shipperCode;
		protected string shipperName;
		protected string shipperAddress;
		protected string shipperCity;
		protected string shipperState;
		protected Guid ownerGuid;
		protected string ownerID;
		protected string ownerCode;
		protected string ownerName;
		protected string ownerAddress;
		protected string ownerCity;
		protected string ownerState;
		protected Guid managerGuid;
		protected string managerID;
		protected string managerCode;
		protected string managerName;
		protected string managerAddress;
		protected string managerCity;
		protected string managerState;
	    protected string equipmentType;
		protected EquipmentCollectionClass equipmentCollection;
		[DataMember] protected FuelCardLimit fuelCardLimit;
		protected string notes;
		protected int inactivityPeriod;
		protected DateTimeOffset statusModifiedDate;
		protected string statusModifiedBy;

        protected Date expirationDateFormat = null;
        protected DateTimeOffset? expirationDate;

	    protected bool transientCardFlag;

        protected string pin;

	    protected string providerID;

        protected Guid fuelCardTypeApplicationStringGuid;

	    protected string fuelCardTypeApplicationStringID;

		
		[EntityImportExportAttribute("SITE*", 105, "SITEGUID")]
		new public Guid SiteGuid { get { return _SiteGuid; } set { _SiteGuid = value; } }

		/// <summary>
		/// This property sets and gets the ID.
		/// </summary>
		[QueryWriterField("ID", "tblFuelCards.ID")]
		[EntityImportExportAttribute("FUELCARDID*", 195, "ID")]
		[DataMember]
		public override string ID { get { return _ID; } set { SetString("ID", 50, value, ref _ID); } }

		/// <summary>
		/// This property sets and gets the provider.
		/// </summary>
		[QueryWriterField("Provider", "tblFuelCards.Provider")]
		[EntityImportExportAttribute("PROVIDER", 170, "Provider")]
		[DataMember]
		public string Provider
		{
			get { return provider; }
			set { provider = value; }
		}

		/// <summary>
		/// This property sets and gets the managerGuid.
		/// </summary>
		[XmlIgnoreAttribute]
		[DataMember]
		public Guid ManagerGuid
		{
			get { return managerGuid; }
			set { managerGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the managerID.
		/// </summary>
		[QueryWriterField("Manager", "tblCompanies_Manager.ID", false)]
		[EntityImportExportAttribute("MANAGERID", 70, "ManagerID")]
		[DataMember]
		public string ManagerID
		{
			get { return managerID; }
			set { managerID = value; }
		}

		/// <summary>
		/// This property sets and gets the managerCode.
		/// </summary>
		[QueryWriterField("ManagerCode", "tblCompanies_Manager.Code", false)]
		[EntityImportExportAttribute("MANAGERCODE", 70, "ManagerCode")]
		[DataMember]
		public string ManagerCode
		{
			get { return managerCode; }
			set { managerCode = value; }
		}


		/// <summary>
		/// This property sets and gets the ownerGuid.
		/// </summary>
		[XmlIgnoreAttribute]
		[DataMember]
		public Guid OwnerGuid
		{
			get { return ownerGuid; }
			set { ownerGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the ownerID.
		/// </summary>
		[QueryWriterField("Owner", "tblCompanies_Owner.ID", false)]
		[EntityImportExportAttribute("OWNERID", 70, "OwnerID")]
		[DataMember]
		public string OwnerID
		{
			get { return ownerID; }
			set { ownerID = value; }
		}

		/// <summary>
		/// This property sets and gets the ownerCode.
		/// </summary>
		[QueryWriterField("OwnerCode", "tblCompanies_Owner.Code", false)]
		[EntityImportExportAttribute("OWNERCODE", 70, "OwnerCode")]
		[DataMember]
		public string OwnerCode
		{
			get { return ownerCode; }
			set { ownerCode = value; }
		}


		/// <summary>
		/// This property sets and gets the shipperGuid.
		/// </summary>
		[XmlIgnoreAttribute]
		[DataMember]
		public Guid ShipperGuid
		{
			get { return shipperGuid; }
			set { shipperGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the shipperID.
		/// </summary>
		[QueryWriterField("Shipper", "tblCompanies_Shipper.ID", false)]
		[EntityImportExportAttribute("SHIPPERID", 70, "ShipperID")]
		[DataMember]
		public string ShipperID
		{
			get { return shipperID; }
			set { shipperID = value; }
		}

		/// <summary>
		/// This property sets and gets the shipperCode.
		/// </summary>
		[QueryWriterField("ShipperCode", "tblCompanies_Shipper.Code", false)]
		[EntityImportExportAttribute("SHIPPERCODE", 70, "ShipperCode")]
		[DataMember]
		public string ShipperCode
		{
			get { return shipperCode; }
			set { shipperCode = value; }
		}

		/// <summary>
		/// This property sets and gets the billToGuid.
		/// </summary>
		[XmlIgnoreAttribute]
		[DataMember]
		public Guid BillToGuid
		{
			get { return billToGuid; }
			set { billToGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the billToID.
		/// </summary>
		[QueryWriterField("BillTo", "tblCompanies_BillTo.ID", false)]
		[EntityImportExportAttribute("BILLTOID", 70, "BillToID")]
		[DataMember]
		public string BillToID
		{
			get { return billToID; }
			set { billToID = value; }
		}

		/// <summary>
		/// This property sets and gets the billToCode.
		/// </summary>
		[QueryWriterField("BillToCode", "tblCompanies_BillTo.Code", false)]
		[EntityImportExportAttribute("BILLTOCODE", 70, "BillToCode")]
		[DataMember]
		public string BillToCode
		{
			get { return billToCode; }
			set { billToCode = value; }
		}



		/// <summary>
		/// This property sets and gets the shipToGuid.
		/// </summary>
		[XmlIgnoreAttribute]
		[DataMember]
		public Guid ShipToGuid
		{
			get { return shipToGuid; }
			set { shipToGuid = value; }
		}

		/// <summary>
		/// This property sets and gets the shipToID.
		/// </summary>
		[QueryWriterField("ShipTo", "tblCompanies_ShipTo.ID", false)]
		[EntityImportExportAttribute("SHIPTOID", 70, "ShipToID")]
		[DataMember]
		public string ShipToID
		{
			get { return shipToID; }
			set { shipToID = value; }
		}

		/// <summary>
		/// This property sets and gets the shipToCode.
		/// </summary>
		[QueryWriterField("ShipToCode", "tblCompanies_ShipTo.Code", false)]
		[EntityImportExportAttribute("SHIPTOCODE", 70, "ShipToCode")]
		[DataMember]
		public string ShipToCode
		{
			get { return shipToCode; }
			set { shipToCode = value; }
		}

		/// <summary>
		/// This property sets and gets the equipmentCollection.
		/// </summary>
		[EntityImportExportWorksheetAttribute("EQUIPMENT ASSIGNMENTS")]
		[EntityImportExportAttribute("EQUIPMENTID*", 100, "ID")]
        [QueryWriterField("Equipment ID", "tblEquipment.ID")]
        [XmlIgnore]
		[DataMember]
		public EquipmentCollectionClass EquipmentCollection
		{
			get { return equipmentCollection; }
			set { equipmentCollection = value; }
		}

        [QueryWriterField("Equipment Type", "tblEquipmentTypes.EqTypeName")]
        [XmlIgnore]
        [DataMember]
        public string EquipmentType
        {
            get { return equipmentType; }
            set { equipmentType = value; }
        }

		/// <summary>
		/// This property sets and gets the fuel card limit.
		/// </summary>
		[XmlIgnore]
		public FuelCardLimit FuelCardLimit
		{
			get { return this.fuelCardLimit; }
			set
			{
				this.fuelCardLimit = value;
			}
		}

		/// <summary>
		/// This property sets and gets the fuel card limit list. Only
		/// used for the Entity export since it wants a collection.
		/// </summary>
		[EntityImportExportWorksheetAttribute("FUEL CARD ASSIGNMENT")]
		[EntityImportExportAttribute("FUELCARDLIMITID*", 50, "ID")]
		[XmlIgnore]
		public IEnumerable<FuelCardLimit> FuelCardLimits
		{
			get
			{
				if (this.fuelCardLimit == null)
				{
					return new List<FuelCardLimit>().AsReadOnly();
				}

				return new List<FuelCardLimit> { this.fuelCardLimit }.AsReadOnly();
			}
		}

		/// <summary>
		/// This property sets and gets the Status as Text.
		/// </summary>
		[EntityImportExportAttribute("STATUS", 50, "Status")]
		[DataMember]
		public Statuses Status
		{
			get { return status; }
			set { status = value; }
		}

		/// <summary>
		/// This property sets and gets the Status as Text.
		/// </summary>
		[DataMember]
		public string StatusID
		{
			get { return STATUS_NAMES[(int)status]; }
			set
			{
				for (Statuses i = Statuses.ACTIVE; i < Statuses.CANCELLED; i++)
				{
					if (value == STATUS_NAMES[(int)i])
					{
						status = i;
						break;
					}
				}
			}
		}


		[QueryWriterField("Status", "tblFuelCards.ActivationStatus")]
		protected Statuses ActivationStatus { get { return status; } }

		/// <summary>
		/// This property sets and gets the Inactivity Period of type int.
		/// </summary>
		[QueryWriterField("Inactivity Period", "tblFuelCards.InactivityPeriod")]
		[EntityImportExportAttribute("INACTIVITYPERIOD", 30, "InactivityPeriod")]
		[DataMember]
		public int InactivityPeriod
		{
			get { return inactivityPeriod; }
			set { inactivityPeriod = value; }
		}

		/// <summary>
		/// This property sets and gets the Note of type string.
		/// </summary>
		[QueryWriterField("Notes", "tblFuelCards.Notes")]
		[EntityImportExportAttribute("NOTES", 200, "Notes")]
		[DataMember]
		public string Notes
		{
			get { return notes; }
			set { notes = value; }
		}

		[QueryWriterField("Status Modified By")]
		[DataMember]
		public string StatusModifiedBy
		{
			get { return statusModifiedBy; }
			set { statusModifiedBy = value; }
		}

		[QueryWriterField("Status Modified Date")]
		[DataMember]
		public DateTimeOffset StatusModifiedDate
		{
			get { return statusModifiedDate; }
			set { statusModifiedDate = value; }

		}

        [QueryWriterField("Expiration Date", "tblFuelCards.ExpirationDate")]
        [DataMember]
        public DateTimeOffset? ExpirationDate
        {
            get { return this.expirationDate; }
            set { this.expirationDate = value; }
        }

        [XmlIgnore]
        public string ExpirationFormattedDate
        {
            get
            {
                return this.expirationDate.HasValue ? this.expirationDate.Value.ToString(expirationDateFormat.Format) : string.Empty;
            }
        }

        [DataMember]
        public Date ExpirationDateFormat
        {
            get { return this.expirationDateFormat; }
            set { this.expirationDateFormat = value; }
        }

        [QueryWriterField("Transient Card")]
        [DataMember]
        public bool TransientCardFlag
        {
            get { return this.transientCardFlag; }
            set { this.transientCardFlag = value; }
        }

        /// <summary>
        /// This property sets and gets the ID.
        /// </summary>
        [DataMember]
        public string PIN
        {
            get
            {
                return this.pin;
            }

            set
            {
                this.SetString("pin", 256, value, ref this.pin);
            }
        }

	    /// <summary>
        /// This property sets and gets the ProviderID
        /// </summary>
		[QueryWriterField("Provider ID", "tblFuelCards.ProviderID")]
		[DataMember]
        public string ProviderID
        {
            get
            {
                return this.providerID;
            }

            set
            {
                this.SetString("providerID", 60, value, ref this.providerID);
            }
        }

	    /// <summary>
        /// This property sets and gets the FuelCardType Application String Guid
        /// </summary>
        [DataMember]
        public Guid FuelCardTypeApplicationStringGuid 
        { 
            get { return this.fuelCardTypeApplicationStringGuid; }
            set { this.fuelCardTypeApplicationStringGuid = value; } 
        }

        /// <summary>
        /// This property sets and gets the Fuel Card Type ID.
        /// </summary>
        [DataMember]
        public string FuelCardTypeApplicationStringID
        {
            get { return this.fuelCardTypeApplicationStringID; }
            set { this.fuelCardTypeApplicationStringID = value; }
        }

        /// <summary>
        /// Represents the date + time that this fuel card was hidden
        /// A null value indicates the fuel card is not hidden.
        /// Although this field is stored as a datetime it is represented to users
        /// as a checkbox. 
        /// </summary>
        [DataMember]
        public DateTimeOffset? HiddenDate { get; set; }

        /// <summary>
        /// This property is here to support entity import + export of the hidden date.
        /// The Entity import + export functionality doesn't play nice with nullable DateTimeOffsets
        /// </summary>
        [EntityImportExportAttribute("HIDDENDATE", 70, "HIDDENDATE")]
        public string HiddenDateAsString
        {
            get
            {
                if (this.HiddenDate.HasValue)
                {
                    return this.HiddenDate.Value.ToString();
                }
                else
                {
                    return string.Empty;
                }
            }

            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.HiddenDate = null;
                }
                else
                {
                    this.HiddenDate = DateTimeOffset.Parse(value);
                }
            }
        }

        [QueryWriterField("User Data 1", "tblFuelCards.UserData1")]
        [EntityImportExportAttribute("USERDATA1", 70, "UserData1")]
		[DataMember]
		public string UserData1 { get { return UserData[0]; } set { UserData[0] = value; } }

        [QueryWriterField("User Data 2", "tblFuelCards.UserData2")]
		[EntityImportExportAttribute("USERDATA2", 70, "UserData2")]
		[DataMember]
		public string UserData2 { get { return UserData[1]; } set { UserData[1] = value; } }

        [QueryWriterField("User Data 3", "tblFuelCards.UserData3")]
		[EntityImportExportAttribute("USERDATA3", 70, "UserData3")]
		[DataMember]
		public string UserData3 { get { return UserData[2]; } set { UserData[2] = value; } }

        [QueryWriterField("User Data 4", "tblFuelCards.UserData4")]
        [EntityImportExportAttribute("USERDATA4", 70, "UserData4")]
		[DataMember]
		public string UserData4 { get { return UserData[3]; } set { UserData[3] = value; } }

        [QueryWriterField("User Data 5", "tblFuelCards.UserData5")]
		[EntityImportExportAttribute("USERDATA5", 70, "UserData5")]
		[DataMember]
		public string UserData5 { get { return UserData[4]; } set { UserData[4] = value; } }

        [QueryWriterField("User Data 6", "tblFuelCards.UserData6")]
        [EntityImportExportAttribute("USERDATA6", 70, "UserData6")]
		[DataMember]
		public string UserData6 { get { return UserData[5]; } set { UserData[5] = value; } }

        [QueryWriterField("User Data 7", "tblFuelCards.UserData7")]
        [EntityImportExportAttribute("USERDATA7", 70, "UserData7")]
		[DataMember]
		public string UserData7 { get { return UserData[6]; } set { UserData[6] = value; } }

        [QueryWriterField("User Data 8", "tblFuelCards.UserData8")]
        [EntityImportExportAttribute("USERDATA8", 70, "UserData8")]
		[DataMember]
		public string UserData8 { get { return UserData[7]; } set { UserData[7] = value; } }

		public string ManagerToolTip
		{
			get
			{
				string ToolTip = "";

				if (managerName != "")
					ToolTip = managerName;
				else
					ToolTip = _ID;

				if (managerAddress != "")
					ToolTip += ", " + managerAddress;
				if (managerCity != "")
					ToolTip += ", " + managerCity;
				if (managerState != "")
					ToolTip += ", " + managerState;
				return ToolTip;
			}
		}

		public string OwnerToolTip
		{
			get
			{
				string ToolTip = "";

				if (ownerName != "")
					ToolTip = ownerName;
				else
					ToolTip = _ID;

				if (ownerAddress != "")
					ToolTip += ", " + ownerAddress;
				if (ownerCity != "")
					ToolTip += ", " + ownerCity;
				if (ownerState != "")
					ToolTip += ", " + ownerState;
				return ToolTip;
			}
		}

		public string ShipperToolTip
		{
			get
			{
				string ToolTip = "";

				if (shipperName != "")
					ToolTip = shipperName;
				else
					ToolTip = _ID;

				if (shipperAddress != "")
					ToolTip += ", " + shipperAddress;
				if (shipperCity != "")
					ToolTip += ", " + shipperCity;
				if (shipperState != "")
					ToolTip += ", " + shipperState;
				return ToolTip;
			}
		}

		public string BillToToolTip
		{
			get
			{
				string ToolTip = "";

				if (billToName != "")
					ToolTip = billToName;
				else
					ToolTip = _ID;

				if (billToAddress != "")
					ToolTip += ", " + billToAddress;
				if (billToCity != "")
					ToolTip += ", " + billToCity;
				if (billToState != "")
					ToolTip += ", " + billToState;
				return ToolTip;
			}
		}

		public string ShipToToolTip
		{
			get
			{
				string ToolTip = "";

				if (shipToName != "")
					ToolTip = shipToName;
				else
					ToolTip = _ID;

				if (shipToAddress != "")
					ToolTip += ", " + shipToAddress;
				if (shipToCity != "")
					ToolTip += ", " + shipToCity;
				if (shipToState != "")
					ToolTip += ", " + shipToState;
				return ToolTip;
			}
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public override ENTITY_TYPE EntityType
		{
			get
			{
				return ENTITY_TYPE.FUEL_CARD;
			}
		}

		[XmlIgnoreAttribute]
		[DataMember]
		public override ENTITY_TYPE ParentEntityType
		{
			get
			{
				return ENTITY_TYPE.NONE;
			}
		}

		/// <summary>
		/// This is the default constructor for the fuel card class.
		/// </summary>
		public FuelCardClass()
		{
            this.Reset();
            this.expirationDateFormat = new Date();
		    this.ExpirationDateFormat.Format = System.Threading.Thread.CurrentThread.CurrentCulture.DateTimeFormat;
            
		}

		/// <summary>
		/// This constructor will initialize the fuelcard class based on the the site.
		/// </summary>
		/// <param name="Site"></param>
        public FuelCardClass(SiteClass site)
		{
            this.Reset();
            // Really important that we set this if we know the site because otherwise the "Load" query will not return the Company IDs since it needs to know the "TargetSiteGuid" for record versioning.
            this.SiteGuid = site.SiteGuid;

            this.expirationDateFormat = new Date(site);
            
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="FuelCardClass"/> class. 
        /// </summary>
        /// <param name="fuelCard">
        /// The fuel Card to make a copy of
        /// </param>
        public FuelCardClass(FuelCardClass fuelCard)
            : base((FMBaseDataObjectWithUserData)fuelCard)
        {
            this.shipToID = string.IsNullOrEmpty(fuelCard.shipToID) ? string.Empty : string.Copy(fuelCard.shipToID);
            this.shipToCode = string.IsNullOrEmpty(fuelCard.shipToCode) ? string.Empty : string.Copy(fuelCard.shipToCode);
            this.shipToGuid = fuelCard.shipToGuid;
            this.shipToName = string.IsNullOrEmpty(fuelCard.shipToName) ? string.Empty : string.Copy(fuelCard.shipToName);
            this.shipToAddress = string.IsNullOrEmpty(fuelCard.shipToAddress) ? string.Empty : string.Copy(fuelCard.shipToAddress);
            this.shipToCity = string.IsNullOrEmpty(fuelCard.shipToCity) ? string.Empty : string.Copy(fuelCard.shipToCity);
            this.shipToState = string.IsNullOrEmpty(fuelCard.shipToState) ? string.Empty : string.Copy(fuelCard.shipToState);
            this.billToID = string.IsNullOrEmpty(fuelCard.billToID) ? string.Empty : string.Copy(fuelCard.billToID);
            this.billToCode = string.IsNullOrEmpty(fuelCard.billToCode) ? string.Empty : string.Copy(fuelCard.billToCode);
            this.billToGuid = fuelCard.billToGuid;
            this.billToName = string.IsNullOrEmpty(fuelCard.billToName) ? string.Empty : string.Copy(fuelCard.billToName);
            this.billToAddress = string.IsNullOrEmpty(fuelCard.billToAddress) ? string.Empty : string.Copy(fuelCard.billToAddress);
            this.billToCity = string.IsNullOrEmpty(fuelCard.billToCity) ? string.Empty : string.Copy(fuelCard.billToCity);
            this.billToState = string.IsNullOrEmpty(fuelCard.billToState) ? string.Empty : string.Copy(fuelCard.billToState);
            this.shipperID = string.IsNullOrEmpty(fuelCard.shipperID) ? string.Empty : string.Copy(fuelCard.shipperID);
            this.shipperCode = string.IsNullOrEmpty(fuelCard.shipperCode) ? string.Empty : string.Copy(fuelCard.shipperCode);
            this.shipperGuid = fuelCard.shipperGuid;
            this.shipperName = string.IsNullOrEmpty(fuelCard.shipperName) ? string.Empty : string.Copy(fuelCard.shipperName);
            this.shipperAddress = string.IsNullOrEmpty(fuelCard.shipperAddress) ? string.Empty : string.Copy(fuelCard.shipperAddress);
            this.shipperCity = string.IsNullOrEmpty(fuelCard.shipperCity) ? string.Empty : string.Copy(fuelCard.shipperCity);
            this.shipperState = string.IsNullOrEmpty(fuelCard.shipperState) ? string.Empty : string.Copy(fuelCard.shipperState);
            this.ownerID = string.IsNullOrEmpty(fuelCard.ownerID) ? string.Empty : string.Copy(fuelCard.ownerID);
            this.ownerCode = string.IsNullOrEmpty(fuelCard.ownerCode) ? string.Empty : string.Copy(fuelCard.ownerCode);
            this.ownerGuid = fuelCard.ownerGuid;
            this.ownerName = string.IsNullOrEmpty(fuelCard.ownerName) ? string.Empty : string.Copy(fuelCard.ownerName);
            this.ownerAddress = string.IsNullOrEmpty(fuelCard.ownerAddress) ? string.Empty : string.Copy(fuelCard.ownerAddress);
            this.ownerCity = string.IsNullOrEmpty(fuelCard.ownerCity) ? string.Empty : string.Copy(fuelCard.ownerCity);
            this.ownerState = string.IsNullOrEmpty(fuelCard.ownerState) ? string.Empty : string.Copy(fuelCard.ownerState);
            this.managerID = string.IsNullOrEmpty(fuelCard.managerID) ? string.Empty : string.Copy(fuelCard.managerID);
            this.managerCode = string.IsNullOrEmpty(fuelCard.managerCode) ? string.Empty : string.Copy(fuelCard.managerCode);
            this.managerGuid = fuelCard.managerGuid;
            this.managerName = string.IsNullOrEmpty(fuelCard.managerName) ? string.Empty : string.Copy(fuelCard.managerName);
            this.managerAddress = string.IsNullOrEmpty(fuelCard.managerAddress) ? string.Empty : string.Copy(fuelCard.managerAddress);
            this.managerCity = string.IsNullOrEmpty(fuelCard.managerCity) ? string.Empty : string.Copy(fuelCard.managerCity);
            this.managerState = string.IsNullOrEmpty(fuelCard.managerState) ? string.Empty : string.Copy(fuelCard.managerState);
            this.provider = string.IsNullOrEmpty(fuelCard.provider) ? string.Empty : string.Copy(fuelCard.provider);
            this.inactivityPeriod = fuelCard.inactivityPeriod;
            this.status = fuelCard.status;
            this.notes = string.IsNullOrEmpty(fuelCard.notes) ? string.Empty : string.Copy(fuelCard.notes);
            this.statusModifiedBy = string.IsNullOrEmpty(fuelCard.statusModifiedBy) ? string.Empty : string.Copy(fuelCard.statusModifiedBy);
            this.statusModifiedDate = fuelCard.statusModifiedDate;
            this.expirationDate = fuelCard.expirationDate;
            this.expirationDateFormat = fuelCard.expirationDateFormat;
            this.transientCardFlag = fuelCard.transientCardFlag;
            this.pin = string.IsNullOrEmpty(fuelCard.pin) ? string.Empty : string.Copy(fuelCard.pin);
            this.providerID = string.IsNullOrEmpty(fuelCard.providerID) ? string.Empty : string.Copy(fuelCard.providerID);
            this.fuelCardTypeApplicationStringGuid = fuelCard.fuelCardTypeApplicationStringGuid;
            this.fuelCardTypeApplicationStringID = string.IsNullOrEmpty(fuelCard.fuelCardTypeApplicationStringID) ? string.Empty : string.Copy(fuelCard.fuelCardTypeApplicationStringID);

            this.equipmentCollection = new EquipmentCollectionClass();
            this.equipmentCollection.AddRange(fuelCard.EquipmentCollection);

	        this.fuelCardLimit = null;
            this.HiddenDate = fuelCard.HiddenDate;
            this.UserData = new UserDataClass();

            for (var index = 0; index < fuelCard.UserData.UserData.Length; index++)
            {
                this.UserData[index] = string.Copy(fuelCard.UserData[index]);
            }
        }

        /// <summary>
		/// This method resets the object to its initial state.
		/// </summary>
		public override void Reset()
		{
            base.Reset();
            Initialize();
        }

        private void Initialize()
        {
            this.shipToID = string.Empty;
            this.shipToCode = string.Empty;
            this.shipToGuid = Guid.Empty;
            this.shipToName = string.Empty;
            this.shipToAddress = string.Empty;
            this.shipToCity = string.Empty;
            this.shipToState = string.Empty;
            this.billToID = string.Empty;
            this.billToCode = string.Empty;
            this.billToGuid = Guid.Empty;
            this.billToName = string.Empty;
            this.billToAddress = string.Empty;
            this.billToCity = string.Empty;
            this.billToState = string.Empty;
            this.shipperID = string.Empty;
            this.shipperCode = string.Empty;
            this.shipperGuid = Guid.Empty;
            this.shipperName = string.Empty;
            this.shipperAddress = string.Empty;
            this.shipperCity = string.Empty;
            this.shipperState = string.Empty;
            this.ownerID = string.Empty;
            this.ownerCode = string.Empty;
            this.ownerGuid = Guid.Empty;
            this.ownerName = string.Empty;
            this.ownerAddress = string.Empty;
            this.ownerCity = string.Empty;
            this.ownerState = string.Empty;
            this.managerID = string.Empty;
            this.managerCode = string.Empty;
            this.managerGuid = Guid.Empty;
            this.managerName = string.Empty;
            this.managerAddress = string.Empty;
            this.managerCity = string.Empty;
            this.managerState = string.Empty;
            this.equipmentCollection = new EquipmentCollectionClass();
			this.fuelCardLimit = null;
            this.provider = string.Empty;
            this.inactivityPeriod = 4;
            this.status = Statuses.INACTIVE;
            this.notes = string.Empty;
            this.statusModifiedBy = this.CreatedBy;
            this.statusModifiedDate = this.CreatedDate;
            this.expirationDate = null;
            this.transientCardFlag = false;
            this.pin = string.Empty;
            this.providerID = string.Empty;
            this.fuelCardTypeApplicationStringGuid = Guid.Empty;
            this.fuelCardTypeApplicationStringID = string.Empty;
            this.HiddenDate = null;
            this.UserData = new UserDataClass();
        }

        /// <summary>
		/// This method loads the object with the information from the 
		/// database.
		/// </summary>
		/// <param name="Set"></param>
		/// 
		public void Load(DataRow row)
		{
			if (row == null)
			{
				throw new ArgumentNullException("Set");
			}

			Reset();

			base.IdentityGuid = DataObject.getValue<Guid>(row["FuelCardGuid"], Guid.Empty);
			base.SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
			base.ID = DataObject.getValue<string>(row["ID"], string.Empty);
			base.CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
			base.CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
            base.UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this.CreatedDate);
			base.UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
            this.provider = DataObject.getValue<string>(row["Provider"], string.Empty);
            this.shipToGuid = DataObject.getValue<Guid>(row["ShipToCompanyGuid"], Guid.Empty);
            this.shipToID = DataObject.getValue<string>(row["ShipToID"], string.Empty);
            this.shipToCode = DataObject.getValue<string>(row["ShipToCode"], string.Empty);
            this.shipToName = DataObject.getValue<string>(row["ShipToName"], string.Empty);
            this.shipToAddress = DataObject.getValue<string>(row["ShipToAddress"], string.Empty);
            this.shipToCity = DataObject.getValue<string>(row["ShipToCity"], string.Empty);
            this.shipToState = DataObject.getValue<string>(row["ShipToState"], string.Empty);
            this.billToGuid = DataObject.getValue<Guid>(row["BillToCompanyGuid"], Guid.Empty);
            this.billToID = DataObject.getValue<string>(row["BillToID"], string.Empty);
            this.billToCode = DataObject.getValue<string>(row["BillToCode"], string.Empty);
            this.billToName = DataObject.getValue<string>(row["BillToName"], string.Empty);
            this.billToAddress = DataObject.getValue<string>(row["BillToAddress"], string.Empty);
            this.billToCity = DataObject.getValue<string>(row["BillToCity"], string.Empty);
            this.billToState = DataObject.getValue<string>(row["BillToState"], string.Empty);
            this.shipperGuid = DataObject.getValue<Guid>(row["ShipperCompanyGuid"], Guid.Empty);
            this.shipperID = DataObject.getValue<string>(row["ShipperID"], string.Empty);
            this.shipperCode = DataObject.getValue<string>(row["ShipperCode"], string.Empty);
            this.shipperName = DataObject.getValue<string>(row["ShipperName"], string.Empty);
            this.shipperAddress = DataObject.getValue<string>(row["ShipperAddress"], string.Empty);
            this.shipperCity = DataObject.getValue<string>(row["ShipperCity"], string.Empty);
            this.shipperState = DataObject.getValue<string>(row["ShipperState"], string.Empty);
            this.ownerGuid = DataObject.getValue<Guid>(row["OwnerCompanyGuid"], Guid.Empty);
            this.ownerID = DataObject.getValue<string>(row["OwnerID"], string.Empty);
            this.ownerCode = DataObject.getValue<string>(row["OwnerCode"], string.Empty);
            this.ownerName = DataObject.getValue<string>(row["OwnerName"], string.Empty);
            this.ownerAddress = DataObject.getValue<string>(row["OwnerAddress"], string.Empty);
            this.ownerCity = DataObject.getValue<string>(row["OwnerCity"], string.Empty);
            this.ownerState = DataObject.getValue<string>(row["OwnerState"], string.Empty);
            this.managerGuid = DataObject.getValue<Guid>(row["ManagerCompanyGuid"], Guid.Empty);
            this.managerID = DataObject.getValue<string>(row["ManagerID"], string.Empty);
            this.managerCode = DataObject.getValue<string>(row["ManagerCode"], string.Empty);
            this.managerName = DataObject.getValue<string>(row["ManagerName"], string.Empty);
            this.managerAddress = DataObject.getValue<string>(row["ManagerAddress"], string.Empty);
            this.managerCity = DataObject.getValue<string>(row["ManagerCity"], string.Empty);
            this.managerState = DataObject.getValue<string>(row["ManagerState"], string.Empty);
            this.notes = DataObject.getValue<string>(row["Notes"], string.Empty);
            this.inactivityPeriod = DataObject.getValue<int>(row["InactivityPeriod"], 4);
            this.status = DataObject.getValue<Statuses>(row["ActivationStatus"], Statuses.INACTIVE);
            this.statusModifiedDate = DataObject.getValue<DateTimeOffset>(row["StatusModifiedDate"], this.CreatedDate);
            this.statusModifiedBy = DataObject.getValue<string>(row["StatusModifiedBy"], this.CreatedBy);
            this.expirationDate = DataObject.getOptionalDateTimeOffset(row["ExpirationDate"]);
            this.transientCardFlag = DataObject.getValue<bool>(row["TransientCardFlag"], false);
            this.pin = (DBNull.Value != row["PIN"]) ? CryptoHelper.DecryptAesSymmetric((byte[])row["PIN"], Guids.SiteAdminGuid) : string.Empty;
            this.providerID = DataObject.getValue<string>(row["ProviderID"], string.Empty);
            this.fuelCardTypeApplicationStringGuid = DataObject.getValue<Guid>(row["FuelCardTypeApplicationStringGuid"], Guid.Empty);
            this.fuelCardTypeApplicationStringID = DataObject.getValue<string>(row["FuelCardTypeApplicationStringID"], string.Empty);
            this.HiddenDate = DataObject.getValue<DateTimeOffset?>(row["HiddenDate"], null);
            this.UserData[0] = DataObject.getValue<string>(row["UserData1"], string.Empty);
            this.UserData[1] = DataObject.getValue<string>(row["UserData2"], string.Empty);
            this.UserData[2] = DataObject.getValue<string>(row["UserData3"], string.Empty);
            this.UserData[3] = DataObject.getValue<string>(row["UserData4"], string.Empty);
            this.UserData[4] = DataObject.getValue<string>(row["UserData5"], string.Empty);
            this.UserData[5] = DataObject.getValue<string>(row["UserData6"], string.Empty);
            this.UserData[6] = DataObject.getValue<string>(row["UserData7"], string.Empty);
			this.UserData[7] = DataObject.getValue<string>(row["UserData8"], string.Empty);
		}

		/// <summary>
		/// This method will load data into the fuel card object.
		/// </summary>
		/// <param name="o">Can be a dataset or XML object.</param>
		public override void Load(object o)
		{
			if (typeof(DataSet).IsInstanceOfType(o))
			{
				var dataSet = o as DataSet;

				if (dataSet == null)
				{
					throw new ArgumentNullException("Set");
				}

				Reset();

				DataTable table = dataSet.Tables[0];

				if (table.Rows.Count == 0)
				{
					return;
				}

				Load(table.Rows[0]);
			}
			else if (typeof(XmlNode).IsInstanceOfType(o))
			{
				var xmlNode = o as XmlNode;
				base.Load(xmlNode);

				if (xmlNode != null)
				{
					foreach (XmlNode node in xmlNode.ChildNodes)
					{
						if (node.Name == "Equipments")
						{
							foreach (XmlNode equipmentNode in node.ChildNodes)
							{
								var equipment = new EquipmentClass();

								if (equipmentNode.Attributes != null)
								{
									equipment.ID = equipmentNode.Attributes["ID"].Value;
									equipment.Type =
										(EQUIPMENT_TYPE)Enum.Parse(typeof(EQUIPMENT_TYPE), equipmentNode.Attributes["Type"].Value, true);
								}

								this.equipmentCollection.Add(equipment);
							}
						}
						else if (node.Name == "FuelCardLimits")
						{
							foreach (XmlNode fuelCardLimitNode in node.ChildNodes)
							{
								this.fuelCardLimit = new FuelCardLimit();

								if (fuelCardLimitNode.Attributes != null)
								{
									fuelCardLimit.ID = fuelCardLimitNode.Attributes["ID"].Value;
								}

								break;
							}
						}
						else
						{
							throw new Exception("FuelCard : Unknown node type on load");
						}
					}
				}
			}
			else
			{
				throw new Exception("FuelCard : Unknown object type on load");
			}
		}

		/// <summary>
		/// This method will store the equipment and fuel card limit into an
		/// XML node.
		/// </summary>
		/// <param name="o">The XML document.</param>
		public override void Store(Object o)
		{
			if (typeof(XmlNode).IsInstanceOfType(o))
			{
				base.Store(o);
				var fuelCardNode = (XmlNode) o;

				if (fuelCardNode.OwnerDocument != null)
				{
					XmlNode equipmentsNode = fuelCardNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Equipments", null);
					fuelCardNode.AppendChild(equipmentsNode);

					foreach (EquipmentClass equipment in this.equipmentCollection)
					{
						if (equipmentsNode.OwnerDocument != null)
						{
							XmlNode equipmentNode = equipmentsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "Equipment", null);

							if (equipmentNode.OwnerDocument != null)
							{
								XmlAttribute attribute = equipmentNode.OwnerDocument.CreateAttribute("ID");
								attribute.Value = equipment.ID;

								if (equipmentNode.Attributes != null)
								{
									equipmentNode.Attributes.Append(attribute);

									attribute = equipmentNode.OwnerDocument.CreateAttribute("Type");
									attribute.Value = equipment.Type.ToString();
									equipmentNode.Attributes.Append(attribute);
								}
							}

							equipmentsNode.AppendChild(equipmentNode);
						}
					}

					XmlNode fuelCardLimitsNode = fuelCardNode.OwnerDocument.CreateNode(XmlNodeType.Element, "FuelCardLimits", null);
					fuelCardNode.AppendChild(fuelCardLimitsNode);

					if (this.fuelCardLimit != null && this.fuelCardLimit.ID.Equals(string.Empty) == false)
					{
						if (fuelCardLimitsNode.OwnerDocument != null)
						{
							XmlNode fuelCardLimitNode = fuelCardLimitsNode.OwnerDocument.CreateNode(XmlNodeType.Element, "FuelCardLimit", null);

							if (fuelCardLimitNode.OwnerDocument != null)
							{
								XmlAttribute attribute = fuelCardLimitNode.OwnerDocument.CreateAttribute("ID");
								attribute.Value = fuelCardLimit.ID;

								if (fuelCardLimitNode.Attributes != null)
								{
									fuelCardLimitNode.Attributes.Append(attribute);
								}
							}

							fuelCardLimitsNode.AppendChild(fuelCardLimitNode);
						}
					}
				}
			}
		}

		//***************************************************************************************************************
		// This method will return a SQL statement that retrieves a list of fuel cards using the company guid and
		// search filter as a criterion.
		//***************************************************************************************************************
		static public void EnumerateSQL(
			SqlCommand cmd, 
			SecurityClass security, 
			Guid managerGuid, 
			Guid ownerGuid, 
			Guid shipperGuid, 
			Guid billToGuid, 
			Guid shipToGuid,
            Guid fuelCardTypeApplicationStringGuid,
			string filter, 
			int a_limit,
			bool? transientFlag, 
            bool hideHiddenFuelCards = false)
		{

			FuelCardClass fuelCard = new FuelCardClass();
			string sql = SELECT_SQL(false, a_limit) +
						"WHERE  " + fuelCard.AppendSiteWhereClause(cmd, security, "tblFuelCards", "FuelCardGuid");

			if (shipToGuid != Guid.Empty)
			{
				sql += "  AND tblFuelCards.ShipToCompanyGuid = '" + shipToGuid + "'";
			}

			if (billToGuid != Guid.Empty)
			{
				sql += "  AND tblFuelCards.BillToCompanyGuid = '" + billToGuid + "'";
			}

			if (shipperGuid != Guid.Empty)
			{
				sql += "  AND tblFuelCards.ShipperCompanyGuid = '" + shipperGuid + "'";
			}

			if (ownerGuid != Guid.Empty)
			{
				sql += "  AND tblFuelCards.OwnerCompanyGuid = '" + ownerGuid + "'";
			}

			if (managerGuid != Guid.Empty)
			{
				sql += "  AND tblFuelCards.ManagerCompanyGuid = '" + managerGuid + "'";
			}

		    if (fuelCardTypeApplicationStringGuid != Guid.Empty)
		    {
                sql += " AND tblFuelCards.FuelCardTypeApplicationStringGuid = @FuelCardTypeApplicationStringGuid";
                cmd.Parameters.Add("@FuelCardTypeApplicationStringGuid", SqlDbType.UniqueIdentifier).Value = fuelCardTypeApplicationStringGuid;
		    }

			if (transientFlag != null)
			{
				sql += " AND tblFuelCards.TransientCardFlag = @TransientCardFlag ";
				cmd.Parameters.Add("@TransientCardFlag", SqlDbType.Bit);
				cmd.Parameters["@TransientCardFlag"].Value = transientFlag;
			}

			bool hasFilter = false;
			if (filter != null)
			{
				filter = filter.Trim();
				filter = FuelsManagerExtensions.EscapeLikeClauseCharacters(filter);
				if (filter.Length > 0)
				{
					string status_filter = "";
					for (int i = 0; i < STATUS_NAMES.Length; i++)
					{
						if (STATUS_NAMES[i].ToLower().IndexOf(filter.ToLower()) > -1)
						{
							status_filter += " OR tblFuelCards.ActivationStatus = " + i;
						}
					}

					hasFilter = true;

					sql += " AND ";
					sql += " (UPPER(tblFuelCards.ID) LIKE UPPER(@SearchFilter) OR " +
						 "  UPPER(shipto.ID)			LIKE UPPER(@SearchFilter) OR " +
						 "  UPPER(billto.ID)			LIKE UPPER(@SearchFilter) OR " +
						 "  UPPER(shipper.ID)			LIKE UPPER(@SearchFilter) OR " +
						 "  UPPER(owner.ID)			LIKE UPPER(@SearchFilter) OR " +
						 "  UPPER(manager.ID)			LIKE UPPER(@SearchFilter) OR  " +
						 "  UPPER(tblFuelCards.provider)	LIKE UPPER(@SearchFilter)  " +
						 status_filter + ")";

				}
			}

		    if (hideHiddenFuelCards)
		    {
		        sql += " AND tblFuelCards.HiddenDate IS NULL ";
		    }

			sql += " ORDER BY tblFuelCards.ID";

			cmd.CommandText = sql;

			if (hasFilter)
			{
				cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 255);
				cmd.Parameters["@SearchFilter"].Value = "%" + filter + "%";
			}
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;

		}


        static public void EnumerateSQL(SqlCommand cmd, SecurityClass security, Guid managerGuid, Guid ownerGuid, Guid shipperGuid, Guid billToGuid, Guid shipToGuid, Guid fuelCardTypeApplicationStringGuid, string filter, bool hideHiddenFuelCards = false)
		{
			EnumerateSQL(cmd, security, managerGuid, ownerGuid, shipperGuid, billToGuid, shipToGuid, fuelCardTypeApplicationStringGuid, filter, -1, transientFlag: null, hideHiddenFuelCards: hideHiddenFuelCards);
		}

	    /// <summary>
	    /// This method will set the SQL command to retrieve fuel cards
	    /// for the auto complete functionality.
	    /// </summary>
	    /// <param name="cmd"></param>
	    /// <param name="security"></param>
	    /// <param name="hideHiddenFuelCards">If true, only fuel cards not marked as hidden will be returned</param>
	    static public void EnumerateForAutoCompleteSQL(SqlCommand cmd, SecurityClass security, bool hideHiddenFuelCards = false)
		{
			var fuelCard = new FuelCardClass();
			string sql = "SELECT ID, ExpirationDate FROM tblFuelCards " +
						"WHERE " + fuelCard.AppendSiteWhereClause(cmd, security, "tblFuelCards", "FuelCardGuid");

			// Only get fuel cards with active status "0" (which means they are Active).
			sql += " AND tblFuelCards.ActivationStatus = 0 ";

	        if (hideHiddenFuelCards)
	        {
	            sql += " AND tblFuelCards.HiddenDate IS NULL ";
	        }

			sql += " ORDER BY tblFuelCards.ID ";

			cmd.CommandText = sql;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		static public void EnumerateForSummarySql(
												SqlCommand cmd, 
												SecurityClass security, 
												Guid managerGuid, 
												Guid ownerGuid, 
												Guid shipperGuid, 
												Guid billToGuid, 
												Guid shipToGuid,
                                                Guid fuelCardTypeApplicationStringGuid,
												string filter,
												bool transientFlag,
                                                bool hideHiddenFuelCards = false)
		{
			EnumerateSQL(cmd, security, managerGuid, ownerGuid, shipperGuid, billToGuid, shipToGuid, fuelCardTypeApplicationStringGuid, filter, -1, transientFlag, hideHiddenFuelCards: hideHiddenFuelCards);
		}

	    /// <summary>
	    /// Generate SQL to call a stored procedure which lists all fuel cards not assigned to a fuel card limit except the provided fuel card limit.
	    /// Optionally limit the fuel cards returned to those with an ID containing the provided searchFilter
	    /// </summary>
	    /// <param name="cmd">The SqlCommand to populate with information</param>
	    /// <param name="security">Contains Security information</param>
	    /// <param name="fuelCardLimitGuid">Fuel cards assigned to this limit will be returned.</param>
	    /// <param name="searchFilter">If provided, limits the fuel cards returned to those containing the value provided in the ID field</param>
	    public static void EnumerateNotAssignedToFuelCardLimitSQL(SqlCommand cmd, SecurityClass security, Guid fuelCardLimitGuid, string searchFilter)
        {
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = "usp_FuelCardsEnumerateNotAssignedToAFuelCardLimit";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
            cmd.Parameters.Add("@FuelCardLimitGuid", SqlDbType.UniqueIdentifier).Value = fuelCardLimitGuid == Guid.Empty ? (object)DBNull.Value : fuelCardLimitGuid;

            if (!string.IsNullOrWhiteSpace(searchFilter))
            {
                cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50).Value = searchFilter;
            }
        }

		public void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClause)
		{
            cmd.CommandText =     "declare @CompanyGuidTable TABLE (CompanyGuid uniqueidentifier NULL)" + Environment.NewLine
                                + "INSERT INTO @CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid)" + Environment.NewLine 
                                + "declare @EquipmentGuidTable TABLE (EquipmentGuid uniqueidentifier NULL)" + Environment.NewLine 
                                + "INSERT INTO @EquipmentGuidTable SELECT EquipmentGuid FROM [erv].[udf_GetEquipmentRecordVersions](@TargetSiteGuid)" + Environment.NewLine 
                                + selectClause 
				                + ", tblFuelCards.FuelCardGuid AS EntityGuid"
				                + ", tblCompanies_Manager.ID AS 'tblCompanies_Manager.ID'"
				                + ", tblCompanies_Manager.Code AS 'tblCompanies_Manager.Code'"
				                + ", tblCompanies_Owner.ID AS 'tblCompanies_Owner.ID'"
				                + ", tblCompanies_Owner.Code AS 'tblCompanies_Owner.Code'"
				                + ", tblCompanies_Shipper.ID AS 'tblCompanies_Shipper.ID'"
				                + ", tblCompanies_Shipper.Code AS 'tblCompanies_Shipper.Code'"
				                + ", tblCompanies_ShipTo.ID AS 'tblCompanies_ShipTo.ID'"
				                + ", tblCompanies_ShipTo.Code AS 'tblCompanies_ShipTo.Code'"
				                + ", tblCompanies_BillTo.ID AS 'tblCompanies_BillTo.ID'"
				                + ", tblCompanies_BillTo.Code AS 'tblCompanies_BillTo.Code'"
				                + " FROM tblFuelCards "
				                + " LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies_Manager ON tblCompanies_Manager.[_MasterRecordGuid] = tblFuelCards.ManagerCompanyGuid"
				                + " LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies_Owner ON tblCompanies_Owner.[_MasterRecordGuid] = tblFuelCards.OwnerCompanyGuid"
				                + " LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies_Shipper ON tblCompanies_Shipper.[_MasterRecordGuid] = tblFuelCards.ShipperCompanyGuid"
				                + " LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies_ShipTo ON tblCompanies_ShipTo.[_MasterRecordGuid] = tblFuelCards.ShipToCompanyGuid"
                                + " LEFT JOIN (SELECT * FROM tblCompanies WHERE tblCompanies.CompanyGuid IN (SELECT CompanyGuid FROM @CompanyGuidTable)) tblCompanies_BillTo ON tblCompanies_BillTo.[_MasterRecordGuid] = tblFuelCards.BillToCompanyGuid"
                                + " LEFT JOIN tblEquipment ON tblEquipment.FuelCardGuid = tblFuelCards.FuelCardGuid"
                                + " LEFT JOIN tblEquipmentTypes ON tblEquipmentTypes.EquipmentTypeGuid = tblEquipment.EquipmentTypeGuid"
				                + " WHERE " + this.AppendSiteWhereClause(cmd, security, "tblFuelCards", "FuelCardGuid");

			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
		}

		/// <summary>
		/// This method is used when the edit button is clicked on the query writer results form
		/// </summary>
		/// <returns>The page corresponding to this entity</returns>
		public string DetailPageReference()
		{
			return "/FuelCardWebApp/FCRC_DetailForm.aspx";
		}

		static public void EnumerateSQLByEquipment(SqlCommand cmd, SecurityClass security, Guid equipmentGuid)
		{
			FuelCardClass fuelCard = new FuelCardClass();
			string sql = SELECT_SQL(false, -1) +
									  "WHERE " + fuelCard.AppendSiteWhereClause(cmd, security, "tblFuelCards", "FuelCardGuid") +
                                      " AND tblFuelCards.FuelCardGuid = (SELECT FuelCardGuid FROM tblEquipment WHERE tblEquipment.EquipmentGuid = [erv].[udf_GetFirstParentRecordVersionGuid] ('Equipment', @EquipmentGuid, @TargetSiteGuid)) ";
			sql += " ORDER BY tblFuelCards.ID";

			cmd.CommandText = sql;

			cmd.Parameters.Add("@EquipmentGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@EquipmentGuid"].Value = equipmentGuid;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
		}

		/// <summary>
		/// This method will return the enumeration SQL to retrieve all the fuel card
		/// records based only on the security context.
		/// </summary>
		/// <param name="Security"></param>
		/// <returns></returns>
		static public void EnumerateSQL(SqlCommand cmd, SecurityClass security)
		{
			EnumerateSQL(cmd, security, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, null, -1, transientFlag: null);
		}

        /// <summary>
		/// This method constructs a SQL command to insert a new fuel card record in the database.
		/// </summary>
		public void InsertSQL(SqlCommand cmd)
		{
			//
			// Query string to insert all but dates (CreatedDate, StatusModifiedDate, UpdatedDate), 
			// which are set to current date and time as default values.
			//
			cmd.CommandText = "INSERT INTO tblFuelCards " +
							"(" +
							"ID, " +
							"SiteGuid, " +
							"ShipToCompanyGuid, " +
							"BillToCompanyGuid, " +
							"ShipperCompanyGuid, " +
							"OwnerCompanyGuid, " +
							"ManagerCompanyGuid, " +
							"Provider, " +
							"InactivityPeriod, " +
							"Notes, " +
							"ActivationStatus, " +
							"StatusModifiedDate, " +
							"StatusModifiedBy, " +
                            "HiddenDate," + 
							"UserData1," +
							"UserData2," +
							"UserData3," +
							"UserData4," +
							"UserData5," +
							"UserData6," +
							"UserData7," +
							"UserData8," +
							"CreatedBy, " +
							"CreatedDate, " +
							"UpdatedBy," +
							"UpdatedDate," +
							"FuelCardGuid," +
                            "ExpirationDate," +
                            "TransientCardFlag," + 
                            "PIN," +
                            "ProviderID," +
                            "FuelCardTypeApplicationStringGuid" +
                            ") VALUES (  " +
							"@ID, " +
							"@SiteGuid, " +
							"@ShipToCompanyGuid, " +
							"@BillToCompanyGuid, " +
							"@ShipperCompanyGuid, " +
							"@OwnerCompanyGuid, " +
							"@ManagerCompanyGuid, " +
							"@Provider," +
							"@InactivityPeriod, " +
							"@Notes, " +
							"@Status, " +
							"@StatusModifiedDate, " +
							"@StatusModifiedBy, " +
                            "@HiddenDate," + 
							"@UserData0," +
							"@UserData1," +
							"@UserData2," +
							"@UserData3," +
							"@UserData4," +
							"@UserData5," +
							"@UserData6," +
							"@UserData7," +
							"@CreatedBy, " +
							"SYSDATETIMEOFFSET()," +
							"@UpdatedBy," +
							"SYSDATETIMEOFFSET()," +
							"@FuelCardGuid," +
                            "@ExpirationDate," +
                            "@TransientCardFlag," +
                            "@PIN," +
                            "@ProviderID," +
                            "@FuelCardTypeApplicationStringGuid" +
                            ")";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ShipToCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@BillToCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ShipperCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Provider", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@InactivityPeriod", SqlDbType.Int);
			cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 4000);
			cmd.Parameters.Add("@Status", SqlDbType.Int);
			cmd.Parameters.Add("@StatusModifiedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@StatusModifiedBy", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UserData0", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData1", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData2", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData3", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData4", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData5", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData6", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData7", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@FuelCardGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@TransientCardFlag", SqlDbType.Bit);
            cmd.Parameters.Add("@PIN", SqlDbType.VarBinary, 256);
            cmd.Parameters.Add("@ProviderID", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@FuelCardTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;

			if (ShipToGuid == Guid.Empty)
			{
				cmd.Parameters["@ShipToCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ShipToCompanyGuid"].Value = ShipToGuid;
			}

			if (BillToGuid == Guid.Empty)
			{
				cmd.Parameters["@BillToCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@BillToCompanyGuid"].Value = BillToGuid;
			}

			if (ShipperGuid == Guid.Empty)
			{
				cmd.Parameters["@ShipperCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ShipperCompanyGuid"].Value = ShipperGuid;
			}

			if (OwnerGuid == Guid.Empty)
			{
				cmd.Parameters["@OwnerCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@OwnerCompanyGuid"].Value = OwnerGuid;
			}

			if (ManagerGuid == Guid.Empty)
			{
				cmd.Parameters["@ManagerCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ManagerCompanyGuid"].Value = ManagerGuid;
			}

			cmd.Parameters["@Provider"].Value = string.IsNullOrEmpty(provider) ? string.Empty : provider;
			cmd.Parameters["@InactivityPeriod"].Value = InactivityPeriod;
			cmd.Parameters["@Notes"].Value = string.IsNullOrEmpty(notes) ? string.Empty : notes;
			cmd.Parameters["@Status"].Value = (int)Status;
			cmd.Parameters["@StatusModifiedDate"].Value = StatusModifiedDate;
			cmd.Parameters["@StatusModifiedBy"].Value = statusModifiedBy;
            cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;
			cmd.Parameters["@UserData0"].Value = UserData[0];
			cmd.Parameters["@UserData1"].Value = UserData[1];
			cmd.Parameters["@UserData2"].Value = UserData[2];
			cmd.Parameters["@UserData3"].Value = UserData[3];
			cmd.Parameters["@UserData4"].Value = UserData[4];
			cmd.Parameters["@UserData5"].Value = UserData[5];
			cmd.Parameters["@UserData6"].Value = UserData[6];
			cmd.Parameters["@UserData7"].Value = UserData[7];
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@CreatedBy"].Value = CreatedBy;
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@FuelCardGuid"].Value = _IdentityGuid;

            cmd.Parameters["@ExpirationDate"].Value = this.expirationDate.HasValue ? this.expirationDate.Value : (object)DBNull.Value;
            cmd.Parameters["@TransientCardFlag"].Value = this.TransientCardFlag;
            cmd.Parameters["@PIN"].Value = CryptoHelper.EncryptAesSymmetric(this.pin, Guids.SiteAdminGuid);
            cmd.Parameters["@ProviderID"].Value = this.ProviderID;
            cmd.Parameters["@FuelCardTypeApplicationStringGuid"].Value = (this.FuelCardTypeApplicationStringGuid
                                                                          == Guid.Empty)
                                                                             ? (object)DBNull.Value
                                                                             : this.FuelCardTypeApplicationStringGuid;
		}

		/// <summary>
		/// This method constructs an update SQL command used to update an existing fuel card record.
		/// </summary>
		public void UpdateSQL(SqlCommand cmd)
		{
			cmd.CommandText = "UPDATE tblFuelCards SET " +
				"SiteGuid = @SiteGuid, " +
				"ShipToCompanyGuid = @ShipToCompanyGuid, " +
				"BillToCompanyGuid = @BillToCompanyGuid, " +
				"ShipperCompanyGuid = @ShipperCompanyGuid, " +
				"OwnerCompanyGuid = @OwnerCompanyGuid, " +
				"ManagerCompanyGuid = @ManagerCompanyGuid, " +
				"provider = @Provider, " +
				"notes = @Notes, " +
				"inactivityPeriod = @InactivityPeriod, " +
				"ActivationStatus = @Status, " +
				"statusModifiedDate = @StatusModifiedDate, " +
				"StatusModifiedBy = @StatusModifiedBy, " +
                "HiddenDate = @HiddenDate," + 
				"ID = @ID, " +
				"UserData1 = @UserData0," + // like userData[0]
				"UserData2 = @UserData1," +
				"UserData3 = @UserData2," +
				"UserData4 = @UserData3," +
				"UserData5 = @UserData4," +
				"UserData6 = @UserData5," +
				"UserData7 = @UserData6," +
				"UserData8 = @UserData7," +
				"UpdatedDate = SYSDATETIMEOFFSET(), " +
				"UpdatedBy =  @UpdatedBy, " +
                "ExpirationDate =  @ExpirationDate, " +
                "TransientCardFlag =  @TransientCardFlag, " +
                "PIN = @PIN, " +
                "ProviderID = @ProviderID, " +
                "FuelCardTypeApplicationStringGuid = @FuelCardTypeApplicationStringGuid " +
                "WHERE FuelCardGuid = @IdentityGuid";

			cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ShipToCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@BillToCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ShipperCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@OwnerCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@ManagerCompanyGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters.Add("@Provider", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@InactivityPeriod", SqlDbType.Int);
			cmd.Parameters.Add("@Notes", SqlDbType.NVarChar, 4000);
			cmd.Parameters.Add("@Status", SqlDbType.Int);
			cmd.Parameters.Add("@StatusModifiedDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@StatusModifiedBy", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
			cmd.Parameters.Add("@UserData0", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData1", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData2", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData3", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData4", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData5", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData6", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UserData7", SqlDbType.NVarChar, 60);
			cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@TransientCardFlag", SqlDbType.Bit);
		    cmd.Parameters.Add("@PIN", SqlDbType.VarBinary, 256);
            cmd.Parameters.Add("@ProviderID", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@FuelCardTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);

			cmd.Parameters["@SiteGuid"].Value = SiteGuid;

			if (ShipToGuid == Guid.Empty)
			{
				cmd.Parameters["@ShipToCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ShipToCompanyGuid"].Value = ShipToGuid;
			}

			if (BillToGuid == Guid.Empty)
			{
				cmd.Parameters["@BillToCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@BillToCompanyGuid"].Value = BillToGuid;
			}

			if (ShipperGuid == Guid.Empty)
			{
				cmd.Parameters["@ShipperCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ShipperCompanyGuid"].Value = ShipperGuid;
			}

			if (OwnerGuid == Guid.Empty)
			{
				cmd.Parameters["@OwnerCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@OwnerCompanyGuid"].Value = OwnerGuid;
			}

			if (ManagerGuid == Guid.Empty)
			{
				cmd.Parameters["@ManagerCompanyGuid"].Value = DBNull.Value;
			}
			else
			{
				cmd.Parameters["@ManagerCompanyGuid"].Value = ManagerGuid;
			}

			cmd.Parameters["@Provider"].Value = string.IsNullOrEmpty(provider) ? string.Empty : provider;
			cmd.Parameters["@InactivityPeriod"].Value = InactivityPeriod;
			cmd.Parameters["@Notes"].Value = string.IsNullOrEmpty(notes) ? string.Empty : notes;
			cmd.Parameters["@Status"].Value = (int)Status;
			cmd.Parameters["@StatusModifiedDate"].Value = StatusModifiedDate;
			cmd.Parameters["@StatusModifiedBy"].Value = statusModifiedBy;
            cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;
			cmd.Parameters["@UserData0"].Value = UserData[0];
			cmd.Parameters["@UserData1"].Value = UserData[1];
			cmd.Parameters["@UserData2"].Value = UserData[2];
			cmd.Parameters["@UserData3"].Value = UserData[3];
			cmd.Parameters["@UserData4"].Value = UserData[4];
			cmd.Parameters["@UserData5"].Value = UserData[5];
			cmd.Parameters["@UserData6"].Value = UserData[6];
			cmd.Parameters["@UserData7"].Value = UserData[7];
			cmd.Parameters["@UpdatedBy"].Value = UpdatedBy;
			cmd.Parameters["@ID"].Value = ID;
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;

            cmd.Parameters["@ExpirationDate"].Value = this.expirationDate.HasValue ? this.expirationDate.Value : (object)DBNull.Value;
            cmd.Parameters["@TransientCardFlag"].Value = this.TransientCardFlag;

            cmd.Parameters["@PIN"].Value = CryptoHelper.EncryptAesSymmetric(this.pin, Guids.SiteAdminGuid);
            cmd.Parameters["@ProviderID"].Value = this.ProviderID;
            cmd.Parameters["@FuelCardTypeApplicationStringGuid"].Value = (this.FuelCardTypeApplicationStringGuid
                                                                          == Guid.Empty)
                                                                             ? (object)DBNull.Value
                                                                             : this.FuelCardTypeApplicationStringGuid;
        }

		/// <summary>
		/// This method constructs the SQL command to delete a fuel card record from the database.
		/// </summary>
		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblFuelCards WHERE FuelCardGuid = @IdentityGuid ";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
		}

		/// <summary>
		/// This method constructs the SQL select by IdentityGuid command.
		/// </summary>
		public void SelectSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = SELECT_SQL(bInTransaction, -1) + " WHERE tblFuelCards.FuelCardGuid = @IdentityGuid";

			cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@IdentityGuid"].Value = IdentityGuid;
			cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
			cmd.Parameters["@TargetSiteGuid"].Value = this._SiteGuid;
		}

        /// <summary>
        /// This method constructs the SQL select by IdentityGuid command.
        /// </summary>
        public void SelectByIdSQL(SqlCommand cmd, bool bInTransaction)
        {
            cmd.CommandText = SELECT_SQL(bInTransaction, -1) + " WHERE tblFuelCards.ID = @ID";

            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
            cmd.Parameters["@ID"].Value = this.ID;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = this._SiteGuid;
        }

        /// <summary>
		/// This method constructs the SQL select by ID command.
		/// </summary>
		public void SelectIdentityGuidSQL(SqlCommand cmd, SecurityClass security, bool bInTransaction)
		{
			cmd.CommandText = "SELECT FuelCardGuid FROM tblFuelCards" + SQLUpdateLock(bInTransaction) +
									 " WHERE " + this.AppendSiteWhereClause(cmd, security, "tblFuelCards", "FuelCardGuid") +
									 " AND tblFuelCards.ID = @ID";

			cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 50);
			cmd.Parameters["@ID"].Value = ID;
		}

		public QueryWriterFieldCollection QueryAliasFields(SecurityClass Security, QueryWriterFieldCollection Fields)
		{
			var userDataFieldCollection =
				FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
					x => x.EnumerateByEntityType(Security, ENTITY_TYPE.FUEL_CARD, Guid.Empty, false, false));

			QueryWriterFieldCollection newCollection = new QueryWriterFieldCollection(Fields);

			var UserFields = from F in newCollection
								  where F.DisplayName.StartsWith("User Data")
								  select F;

			foreach (var userField in UserFields)
			{
				if (UpdateFieldName(userField, userDataFieldCollection) == false)
				{
					userField.DisplayName = string.Empty;
				}
			}

			// Remove any blanked out fields.  Wish we could do it above but
			// it disrupts the enumeration.
			for (int Index = newCollection.Count - 1; Index >= 0; --Index)
			{
				if (string.IsNullOrEmpty(newCollection[Index].DisplayName))
				{
					newCollection.RemoveAt(Index);
				}
			}

			QueryClass.ApplyDataDictionary(Security, newCollection);

			return newCollection;

		}
	}

}
