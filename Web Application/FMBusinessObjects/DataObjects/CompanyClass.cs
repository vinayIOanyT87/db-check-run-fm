// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompanyClass.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   ENTER FILE SUMMARY HERE
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.SqlClient;
    using System.Globalization;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Xml;
    using System.Xml.Serialization;

    using BusinessInterfaces;
    using ChannelFactories;
    using FMBusinessObjects.Constants;
    using UtilityObjects;

    using Varec.CommonComponents.EngineeringUnitsLibrary;

    [Serializable]
    [CollectionDataContract]
    public class CompanyCollectionClass : List<CompanyClass>
    {
        #region Public Methods and Operators

        /// <summary>
        ///     Find the object with the given Master Record globally unique identifier in the given list
        /// </summary>
        /// <param name="targetMasterRecordGuid">
        ///     The target Master Record globally unique identifier.
        /// </param>
        /// <returns>
        ///     Company class
        /// </returns>
        public CompanyClass FindByMasterRecordGuid(Guid targetMasterRecordGuid)
        {
            return this.FirstOrDefault(item => item.MasterRecordGuid == targetMasterRecordGuid);
        }

        public CompanyClass Find(Guid guid)
        {
            return this.FindByGuid(guid);
        }


        /// <summary>
        ///     Removes matching IdentityGuids
        /// </summary>
        /// <param name="company">Company to check agaisnt List</param>
        /// <returns>True if Company is removed</returns>
        public new bool Remove(CompanyClass company)
        {
            foreach (CompanyClass item in this)
            {
                if (item.IdentityGuid == company.IdentityGuid)
                {
                    return base.Remove(item);
                }
            }
            return false;
        }

        #endregion
    }

    /// <summary>
    ///     Data object for FuelsManager Company entities.
    /// </summary>
    [EntityImportExportWorksheet("COMPANIES")]
    [DataContract]
    [Serializable]
    [KnownType(typeof(CompanyMapClass))]
    [KnownType(typeof(CompanyMapAuthorizedCarrierClass))]
    [KnownType(typeof(CompanyMapBillToShipperClass))]
    [KnownType(typeof(CompanyMapCompanyGroupCompanyClass))]
    [KnownType(typeof(CompanyMapFootNoteShipperClass))]
    [KnownType(typeof(CompanyMapFootNoteShipToClass))]
    [KnownType(typeof(CompanyMapLoadIdShipToClass))]
    [KnownType(typeof(CompanyMapLoadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapOffloadIdSupplierClass))]
    [KnownType(typeof(CompanyMapOffloadOwnerManagerClass))]
    [KnownType(typeof(CompanyMapPersonAssignedCompanyClass))]
    [KnownType(typeof(CompanyMapShipperOwnerClass))]
    [KnownType(typeof(CompanyMapShipToBillToClass))]
    [KnownType(typeof(CompanyMapSupplierOwnerClass))]
    [KnownType(typeof(CompanyMapUserGroupCompanyClass))]
    [QueryWriterTopic(typeof(CompanyClass), "Companies")]
    [QueryWriterTopicSecurity(RIGHT.VIEW_COMPANY_DATA)]
    [QueryWriterTopicSecurity(RIGHT.MODIFY_COMPANY_DATA)]
    public class CompanyClass : FMBaseDataObjectWithUserData, IAlarmAndEventDiscovery, IDataDictionary
    {
        #region Constants and Fields

        public const string EntityTypeID = "Companies";

        public const string CachedCompanyrecordversionTableName = "@CompanyGuidTable";

        public string CachedCompanyrecordversionGuidTableOperation =
                                        "declare " + CachedCompanyrecordversionTableName + " TABLE ( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
                                        "INSERT INTO " + CachedCompanyrecordversionTableName + " SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid)" + Environment.NewLine;

        public const string CachedCompanyrecordversionGuidTableSelect = "SELECT CompanyGuid from " + CachedCompanyrecordversionTableName;

        [EntityImportExportWorksheet("COMPANY ACCESS SCHEDULE")]
        [EntityImportExport("TYPE", 100, "Type", 1)]
        [EntityImportExport("DAY", 110, "DayText", 2)]
        [EntityImportExport("ENABLED", 110, "Enabled", 3)]
        [EntityImportExport("OPENINGTIME", 110, "OpeningTimeString", 4)]
        [EntityImportExport("CLOSINGTIME", 110, "ClosingTimeString", 5)]
        [DataMember]
        public ScheduleCollectionClass AccessScheduleCollection;

        [EntityImportExportWorksheet("CARRIERS", typeof(CompanyMapAuthorizedCarrierClass))]
        [EntityImportExport("ID*", 130, "AssignedID", 1)]
        [DataMember]
        public CompanyMapCollectionClass AuthorizedCarrierCollection;

        [EntityImportExportWorksheet("CUSTOMER PRODUCTS")]
        [EntityImportExport("ID*", 125, "AssignedID", 1)]
        [EntityImportExport("ADDITIVEPROFILEID*", 130, "AdditiveProfileID", 2)]
        [EntityImportExport("SHIPTOPRODUCTID", 130, "ShipToProductID", 3)]
        [EntityImportExport("SHIPTOPRODUCTCODE", 130, "ShipToProductCode", 4)]
        [EntityImportExport("SHIPTOLOADRACKTEXT", 130, "ShipToLoadRackDisplayText", 5)]
        [EntityImportExport("INSTRUCTIONS", 1000, "Note", 6)]
        [DataMember]
        public ProductMapCollectionClass AuthorizedProductCollection;

        [EntityImportExportWorksheet("CUSTOMERS")]
        [EntityImportExport("ID*", 130, "AssignedToID")]
        [DataMember]
        public CompanyMapCollectionClass CarrierCustomerShipToCollection;

        [EntityImportExportWorksheet("CERTIFICATES AND PERMITS")]
        [EntityImportExport("ID*", 110, "ID", 1)]
        [EntityImportExport("NUMBER", 110, "Number", 2)]
        [EntityImportExport("EXPIRATIONDATE", 110, "ExpirationDateString", 3)]
        [DataMember]
        public QualificationMapCollectionClass CertificateAndPermitCollection;

        [DataMember]
        [XmlIgnore]
        public Guid CustomerBillToTypeApplicationStringGuid;

        [DataMember]
        [XmlIgnore]
        public Guid CustomerShipToTypeApplicationStringGuid;

        [EntityImportExportWorksheet("COMPANIES EQUIPMENT")]
        [EntityImportExport("ID*", 100, "ID", 1)]
        [EntityImportExport("TYPECLASS", 100, "TypeClass", 2)]
        [DataMember]
        public EquipmentCollectionClass EquipmentCollection;

        [EntityImportExportWorksheet("USER GROUPS", typeof(CompanyMapUserGroupCompanyClass))]
        [EntityImportExport("ID*", 130, "AssignedToID", 1)]
        [DataMember]
        public CompanyMapCollectionClass GroupMapCollection;

        [DataMember]
        [XmlIgnore]
        public Guid IATAGuid;

        [DataMember]
        [EntityImportExport("NOTE", 110, "NOTE")]
        public string Note;

        [EntityImportExportWorksheet("DRIVERS")]
        [EntityImportExport("ID*", 110, "ID", 1)]
        [EntityImportExport("FIRSTNAME", 90, "FirstName", 2)]
        [EntityImportExport("MIDDLENAME", 90, "MiddleName", 3)]
        [EntityImportExport("LASTNAME", 95, "LastName", 4)]
        [DataMember]
        public CompanyMapCollectionClass AssignedPersonnelCollection;

        [EntityImportExportWorksheet("COMPANY ROLES", "ROLEID*")]
        [EntityImportExport("ROLEID*", 110, "ID")]
        [DataMember]
        public CompanyRoleMapCollectionClass RoleCollection;

        [DataMember]
        [XmlIgnore]
        public Guid ShipperTypeApplicationStringGuid;

        [EntityImportExportWorksheet("SUPPLIER PRODUCTS")]
        [EntityImportExport("ID*", 125, "AssignedID", 1)]
        [DataMember]
        public ProductMapCollectionClass SupplierAuthorizedProductCollection;

        [EntityImportExportWorksheet("UNAVAILABLE INVENTORY")]
        [EntityImportExport("ID*", 125, "AssignedID", 1)]
        [EntityImportExport("UNAVAILABLEGROSS", 130, "UnavailableInventoryGross", 2)]
        [EntityImportExport("UNAVAILABLENET", 130, "UnavailableInventoryNet", 3)]
        [DataMember]
        public ProductMapCollectionClass UnavailableInventoryCollection;

        [DataMember]
        public Date _EffectiveDate;

        [DataMember]
        public Date _ExpirationDate;

        [DataMember]
        public Date _InsuranceExpiration;

        [DataMember]
        public DateAndTime _LastActivityDate; // Excluded from Property Map

        [DataMember]
        public FMDecimal _LiabilityAmount;

        [DataMember]
        public Date _LicenseExpiration;

        [DataMember]
        public Date _LockedOutDate; // Excluded from Property Map

        [DataMember]
        protected string _AccountNumber;

        [DataMember]
        protected bool _AdditiveAccounting;

        [DataMember]
        protected string _Address1;

        [DataMember]
        protected string _Address2;

        [DataMember]
        protected bool _AllowDriverEntry;

        [DataMember]
        protected Guid _AssignedFromSiteGuid;

        [DataMember]
        protected string _AssignedFromSiteId;

        [DataMember]
        protected Guid _AssignedToSiteGuid;

        [DataMember]
        protected string _City;

        [DataMember]
        protected string _Code;

        [DataMember]
        protected string _Contact1Address1;

        [DataMember]
        protected string _Contact1Address2;

        [DataMember]
        protected string _Contact1City;

        [DataMember]
        protected string _Contact1Country;

        [DataMember]
        protected string _Contact1EmailAddress;

        [DataMember]
        protected string _Contact1Fax;

        [DataMember]
        protected string _Contact1Name;

        [DataMember]
        protected string _Contact1PhoneMobile;

        [DataMember]
        protected string _Contact1PhoneOffice;

        [DataMember]
        protected string _Contact1State;

        [DataMember]
        protected string _Contact1Zip;

        [DataMember]
        protected string _Contact2Address1;

        [DataMember]
        protected string _Contact2Address2;

        [DataMember]
        protected string _Contact2City;

        [DataMember]
        protected string _Contact2Country;

        [DataMember]
        protected string _Contact2EmailAddress;

        [DataMember]
        protected string _Contact2Fax;

        [DataMember]
        protected string _Contact2Name;

        [DataMember]
        protected string _Contact2PhoneMobile;

        [DataMember]
        protected string _Contact2PhoneOffice;

        [DataMember]
        protected string _Contact2State;

        [DataMember]
        protected string _Contact2Zip;

        [DataMember]
        protected string _Country;

        [DataMember]
        protected bool _CreditOK;

        // Linked Items

        [DataMember]
        protected string _CustomerBillToTypeID;

        [DataMember]
        protected string _CustomerShipToTypeID;

        [DataMember]
        protected bool _DeliveryToTerminalPermitted;

        [DataMember]
        protected bool _DisableBillToAllocationsCheck;

        [DataMember]
        protected bool _DisableOwnerAllocationsCheck;

        [DataMember]
        protected bool _DisableShipToAllocationsCheck;

        [DataMember]
        protected bool _DisableShipperAllocationsCheck;

        [DataMember]
        protected string _EPANumber;

        [DataMember]
        protected string _EmergencyContact;

        [DataMember]
        protected string _EmergencyPhone;

        [DataMember]
        protected string _Fax;

        [DataMember]
        protected string _FederalID;

        [DataMember]
        protected string _FederalID2;

        [DataMember]
        protected string _FederalID3;

        [DataMember]
        protected string _FederalID4;

        [DataMember]
        protected string _FederalID5;

        [DataMember]
        protected string _StateID;

        [DataMember]
        protected string _FlightPrefix;

        [DataMember]
        protected bool _FlushPermitted;

        [DataMember]
        protected bool _HazardousMaterialExclusion;

        [DataMember]
        protected string _IATAID;

        [DataMember]
        protected string _InsuranceCompany;

        [DataMember]
        protected string _InsurancePolicy;

        [DataMember]
        protected string _LicenseNumber;

        [DataMember]
        protected string _LoadRackDisplayText;

        [DataMember]
        protected bool _LockedOut;

        [DataMember]
        protected string _LockedOutReason;

        [DataMember]
        protected double _LowStockWarning;

        //Record Versioning Items
        [DataMember]
        protected Guid _MasterRecordGuid;

        [DataMember]
        protected double _MaximumVehicleWeight;

        [DataMember]
        protected string _Name;

        [DataMember]
        protected string _ShortName;

        [DataMember]
        protected bool _OnHold;

        [DataMember]
        protected bool _PINRequired;

        [DataMember]
        protected string _Phone;

        [DataMember]
        protected bool _PickupFlights;

        [DataMember]
        protected bool _PumpOffPermitted;

        [DataMember]
        protected bool _PurchaseOrderRequired;

        [DataMember]
        protected string _ReceivableAccount;

        [DataMember]
        protected string _RefinerCode;

        [DataMember]
        protected string _SCACCode;

        [DataMember]
        protected string _ShipperTypeID;

        [DataMember]
        protected string _State;

        [DataMember]
        protected bool _StockTrack;

        [DataMember]
        protected bool _SufferLossGain;

        [DataMember]
        protected string _TaxNumber;

        [DataMember]
        protected EngineeringUnit _WeightUnits;

        [DataMember]
        protected string _Zip;

        protected DateTimeFormatInfo DateTimeFormatInfo;

        [DataMember]
        protected bool _ScullyRequired;

        [DataMember]
        protected ConsortiumTypes? _ConsortiumType;

        [DataMember] protected string companyIataCode;
        [DataMember] protected string companyIcaoCode;

        private const string GroupCompaniesSubQueryClause =
            ",(SELECT DISTINCT a.CompanyGuid AS AuthorizedCompanyGuid FROM " + CachedCompanyrecordversionTableName + " a,"
            + "(SELECT DISTINCT map.tblCompanyCompanyToUserGroup.CompanyGuid"
            + " FROM map.tblCompanyCompanyToUserGroup, map.tblUserToGroup"
            + " WHERE map.tblCompanyCompanyToUserGroup.GroupGuid = map.tblUserToGroup.GroupGuid"
            + " AND map.tblUserToGroup.UserGuid = @UserGuid) AS tblUserCompanyMap"
            + " WHERE tblUserCompanyMap.CompanyGuid IN (" + CachedCompanyrecordversionGuidTableSelect + ")"
            + " OR tblUserCompanyMap.CompanyGuid IS NULL) tblAuthorizedCompanies";

        private const string SelectIDCodeIdentityGuidOnlyClause =
            "SELECT DISTINCT tblCompanies.[ID]," + " tblCompanies.[Code]," + " tblCompanies.CompanyGuid,"
            + "tblCompanies.[ShipperTypeApplicationStringGuid],"
            + "tblCompanies.[CustomerBillToTypeApplicationStringGuid],"
            + "tblCompanies.[CustomerShipToTypeApplicationStringGuid],"
            + " tblCompanies._MasterRecordGuid, "
            + " tblCompanies.[HiddenDate]";

        private static readonly AlarmAndEventDescriptorClass CompanyAccessScheduleAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyAccessScheduleKey);

        private static readonly AlarmAndEventDescriptorClass CompanyCreditAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyCreditKey);

        private static readonly AlarmAndEventDescriptorClass CompanyHazardousMaterialExclusionEventDescriptor =
            new AlarmAndEventDescriptorClass(false, LoadRackKey, CompanyHazardousMaterialExclusionKey);

        private static readonly AlarmAndEventDescriptorClass CompanyInactiveAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyInactiveKey);

        private static readonly AlarmAndEventDescriptorClass CompanyInsuranceExpiredAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyInsuranceExpiredKey);

        private static readonly AlarmAndEventDescriptorClass CompanyInsuranceWarningEventDescriptor =
            new AlarmAndEventDescriptorClass(false, LoadRackKey, CompanyInsuranceWarningKey);

        private static readonly AlarmAndEventDescriptorClass CompanyLicenseExpiredAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyLicenseExpiredKey);

        private static readonly AlarmAndEventDescriptorClass CompanyLicenseWarningEventDescriptor =
            new AlarmAndEventDescriptorClass(false, LoadRackKey, CompanyLicenseWarningKey);

        private static readonly AlarmAndEventDescriptorClass CompanyLockOutEventDescriptor =
            new AlarmAndEventDescriptorClass(false, SystemKey, CompanyLockOutKey);

        private static readonly AlarmAndEventDescriptorClass CompanyLockedOutAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyLockedOutKey);

        private static readonly AlarmAndEventDescriptorClass CompanyUnauthorizedCarrierAlarmDescriptor =
            new AlarmAndEventDescriptorClass(true, LoadRackKey, CompanyUnauthorizedCarrierKey);

        private static readonly AlarmAndEventDescriptorClass AuditLogEntityAddedEventDescriptor =
            new AlarmAndEventDescriptorClass(false, SystemKey, EntityAddedKey);

        private static readonly AlarmAndEventDescriptorClass AuditLogEntityModifiedEventDescriptor =
            new AlarmAndEventDescriptorClass(false, SystemKey, EntityModifiedKey);

        private static readonly AlarmAndEventDescriptorClass AuditLogEntityPurgedEventDescriptor =
            new AlarmAndEventDescriptorClass(false, SystemKey, EntityPurgedKey);

        private static readonly AlarmAndEventDescriptorClass AuditLogDatabaseTraceViewedEventDescriptor =
            new AlarmAndEventDescriptorClass(false, SystemKey, DatabaseTraceViewedKey);

        private const string CompanyAccessScheduleKey = "Company Access Schedule";

        private const string CompanyCreditKey = "Company Credit";

        private const string CompanyHazardousMaterialExclusionKey = "Hazardous Material Exclusion";

        private const string CompanyInactiveKey = "Company Inactive";

        private const string CompanyInsuranceExpiredKey = "Company Insurance Expired";

        private const string CompanyInsuranceWarningKey = "Company Insurance Impending Expiration";

        private const string CompanyLicenseExpiredKey = "Company License Expired";

        private const string CompanyLicenseWarningKey = "Company License Impending Expiration";

        private const string CompanyLockOutKey = "Company Lock Out";

        private const string CompanyLockedOutKey = "Company Locked Out";

        private const string CompanyUnauthorizedCarrierKey = "Unauthorized Carrier";

        private const string EntityAddedKey = "Entity Added";

        private const string EntityModifiedKey = "Entity Modified";

        private const string EntityPurgedKey = "Entity Purged";

        private const string DatabaseTraceViewedKey = "Database Audit Log viewed";


        private string selectClause = "SELECT DISTINCT tblCompanies.*,"
                                      + "(SELECT IATAID FROM tblIATA WHERE tblCompanies.IATAGuid= tblIATA.IATAGuid) AS IATAID,"
                                      + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.ShipperTypeApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS ShipperTypeID,"
                                      + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.CustomerBillToTypeApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS CustomerBillToTypeID,"
                                      + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.CustomerShipToTypeApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS CustomerShipToTypeID";

        private string selectCompanySelectRoleClause = "SELECT DISTINCT " + "tblCompanies.[ID], "
                                                       + "tblCompanies.[Code], " + "tblCompanies.[CompanyGuid], "
                                                       + "tblCompanies.[Name], " + "tblCompanies.[Address1], "
                                                       + "tblCompanies.[Address2], " + "tblCompanies.[City], "
                                                       + "tblCompanies.[State], " + "tblCompanies.[_MasterRecordGuid]";

        #endregion

        #region Constructors and Destructors

        /// <summary>
        ///     This is the default constructor for the companies class.
        /// </summary>
        public CompanyClass()
        {
            this._LastActivityDate = new DateAndTime();
            this._EffectiveDate = new Date();
            this._ExpirationDate = new Date();
            this._LicenseExpiration = new Date();
            this._InsuranceExpiration = new Date();
            this._LockedOutDate = new Date();
            this._LiabilityAmount = new FMDecimal();

            this.Initialize();
        }

        /// <summary>
        ///     This constructor will initialize the company class based on the the site.
        /// </summary>
        /// <param name="site"></param>
        public CompanyClass(SiteClass site)
        {
            this.DateTimeFormatInfo = site.GetDateTimeFormatInfo();
            this._LastActivityDate = new DateAndTime(site);
            this._EffectiveDate = new Date(site);
            this._ExpirationDate = new Date(site);
            this._LicenseExpiration = new Date(site);
            this._InsuranceExpiration = new Date(site);
            this._LockedOutDate = new Date(site);
            this._LiabilityAmount = new FMDecimal(site.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT));

            this.Initialize();
        }

        #endregion

        #region Public Properties

        public AlarmAndEventLogClass AccessScheduleAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyAccessScheduleAlarmDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("Account Number")]
        [EntityImportExport("ACCOUNTNUMBER", 95, "ACCOUNTNUMBER")]
        public string AccountNumber
        {
            get
            {
                return this._AccountNumber;
            }
            set
            {
                this.SetString("Account #", 30, value, ref this._AccountNumber);
            }
        }

        public bool Active => this._EffectiveDate.IsTodayOrBefore && this._ExpirationDate.IsTodayOrAfter;

        [QueryWriterField("Additive Accounting")]
        [EntityImportExport("ADDITIVEACCOUNTING", 117, "ADDITIVEACCOUNTING")]
        public bool AdditiveAccounting
        {
            get
            {
                return this._AdditiveAccounting;
            }
            set
            {
                this._AdditiveAccounting = value;
            }
        }

        [QueryWriterField("Address 1")]
        [EntityImportExport("ADDRESS 1", 190, "ADDRESS1")]
        public string Address1
        {
            get
            {
                return this._Address1;
            }
            set
            {
                this.SetString("Address1", 60, value, ref this._Address1);
            }
        }

        [QueryWriterField("Address 2")]
        [EntityImportExport("ADDRESS 2", 90, "ADDRESS2")]
        public string Address2
        {
            get
            {
                return this._Address2;
            }
            set
            {
                this.SetString("Address2", 60, value, ref this._Address2);
            }
        }

        [QueryWriterField("Allow Driver Entry")]
        [EntityImportExport("ALLOWDRIVERENTRY", 111, "ALLOWDRIVERENTRY")]
        public bool AllowDriverEntry
        {
            get
            {
                return this._AllowDriverEntry;
            }
            set
            {
                this._AllowDriverEntry = value;
            }
        }

        public Guid AssignedFromSiteGuid
        {
            get
            {
                return this._AssignedFromSiteGuid;
            }
            set
            {
                this._AssignedFromSiteGuid = value;
            }
        }

        public string AssignedFromSiteId
        {
            get
            {
                return this._AssignedFromSiteId;
            }
            set
            {
                this._AssignedFromSiteId = value;
            }
        }

        public Guid AssignedToSiteGuid
        {
            get
            {
                return this._AssignedToSiteGuid;
            }
            set
            {
                this._AssignedToSiteGuid = value;
            }
        }

        [QueryWriterField("City")]
        [EntityImportExport("CITY", 90, "CITY")]
        public string City
        {
            get
            {
                return this._City;
            }
            set
            {
                this.SetString("City", 60, value, ref this._City);
            }
        }

        [QueryWriterField("Code")]
        [EntityImportExport("CODE", 70, "CODE")]
        public string Code
        {
            get
            {
                return this._Code;
            }
            set
            {
                this.SetString("Code", 10, value, ref this._Code);
            }
        }

        public string CompanyToolTip
        {
            get
            {
                string toolTip;

                if (string.IsNullOrEmpty(this._Name) == false)
                {
                    toolTip = this._Name;
                }
                else
                {
                    toolTip = this._ID;
                }

                if (string.IsNullOrEmpty(this._Address1) == false)
                {
                    toolTip += ", " + this._Address1;
                }

                if (string.IsNullOrEmpty(this._Address2) == false)
                {
                    toolTip += " " + this._Address2;
                }

                if (string.IsNullOrEmpty(this._City) == false)
                {
                    toolTip += ", " + this._City;
                }

                if (string.IsNullOrEmpty(this._State) == false)
                {
                    toolTip += ", " + this._State;
                }

                return toolTip;
            }
        }

        [QueryWriterField("Contact 1 Address 1")]
        [EntityImportExport("CONTACT1ADDRESS1", 190, "Contact1Address1")]
        public string Contact1Address1
        {
            get
            {
                return this._Contact1Address1;
            }
            set
            {
                this.SetString("Contact 1 Address 1", 30, value, ref this._Contact1Address1);
            }
        }

        [QueryWriterField("Contact 1 Address 2")]
        [EntityImportExport("CONTACT1ADDRESS2", 105, "Contact1Address2")]
        public string Contact1Address2
        {
            get
            {
                return this._Contact1Address2;
            }
            set
            {
                this.SetString("Contact 1 Address 2", 30, value, ref this._Contact1Address2);
            }
        }

        [QueryWriterField("Contact 1 City")]
        [EntityImportExport("CONTACT1CITY", 90, "Contact1City")]
        public string Contact1City
        {
            get
            {
                return this._Contact1City;
            }
            set
            {
                this.SetString("Contact 1 City", 60, value, ref this._Contact1City);
            }
        }

        [QueryWriterField("Contact 1 Country")]
        [EntityImportExport("CONTACT1COUNTRY", 105, "Contact1Country")]
        public string Contact1Country
        {
            get
            {
                return this._Contact1Country;
            }
            set
            {
                this.SetString("Contact 1 Country", 30, value, ref this._Contact1Country);
            }
        }

        [QueryWriterField("Contact 1 Email Address")]
        [EntityImportExport("CONTACT1EMAILADDRESS", 130, "Contact1EmailAddress")]
        public string Contact1EmailAddress
        {
            get
            {
                return this._Contact1EmailAddress;
            }
            set
            {
                this.SetString("Contact 1 Email", 30, value, ref this._Contact1EmailAddress);
            }
        }

        [QueryWriterField("Contact 1 Fax")]
        [EntityImportExport("CONTACT1FAX", 100, "Contact1Fax")]
        public string Contact1Fax
        {
            get
            {
                return this._Contact1Fax;
            }
            set
            {
                this.SetString("Contact 1 Fax", 20, value, ref this._Contact1Fax);
            }
        }

        [QueryWriterField("Contact 1 Name")]
        [EntityImportExport("CONTACT1NAME", 190, "Contact1Name")]
        public string Contact1Name
        {
            get
            {
                return this._Contact1Name;
            }
            set
            {
                this.SetString("Contact 1 Name", 30, value, ref this._Contact1Name);
            }
        }

        [QueryWriterField("Contact 1 Phone Mobile")]
        [EntityImportExport("CONTACT1PHONEMOBILE", 130, "Contact1PhoneMobile")]
        public string Contact1PhoneMobile
        {
            get
            {
                return this._Contact1PhoneMobile;
            }
            set
            {
                this.SetString("Contact 1 Phone Mobile", 20, value, ref this._Contact1PhoneMobile);
            }
        }

        [QueryWriterField("Contact 1 Phone Office")]
        [EntityImportExport("CONTACT1PHONEOFFICE", 130, "Contact1PhoneOffice")]
        public string Contact1PhoneOffice
        {
            get
            {
                return this._Contact1PhoneOffice;
            }
            set
            {
                this.SetString("Contact 1 Phone Office", 20, value, ref this._Contact1PhoneOffice);
            }
        }

        [QueryWriterField("Contact 1 State")]
        [EntityImportExport("CONTACT1STATE", 100, "Contact1State")]
        public string Contact1State
        {
            get
            {
                return this._Contact1State;
            }
            set
            {
                this.SetString("Contact 1 State", 20, value, ref this._Contact1State);
            }
        }

        [QueryWriterField("Contact 1 Zip")]
        [EntityImportExport("CONTACT1ZIP", 70, "Contact1Zip")]
        public string Contact1Zip
        {
            get
            {
                return this._Contact1Zip;
            }
            set
            {
                this.SetString("Contact 1 Zip", 11, value, ref this._Contact1Zip);
            }
        }

        [QueryWriterField("Contact 2 Address 1")]
        [EntityImportExport("CONTACT2ADDRESS1", 190, "Contact2Address1")]
        public string Contact2Address1
        {
            get
            {
                return this._Contact2Address1;
            }
            set
            {
                this.SetString("Contact 2 Address 1", 30, value, ref this._Contact2Address1);
            }
        }

        [QueryWriterField("Contact 2 Address 2")]
        [EntityImportExport("CONTACT2ADDRESS2", 105, "Contact2Address2")]
        public string Contact2Address2
        {
            get
            {
                return this._Contact2Address2;
            }
            set
            {
                this.SetString("Contact 2 Address 2", 30, value, ref this._Contact2Address2);
            }
        }

        [QueryWriterField("Contact 2 City")]
        [EntityImportExport("CONTACT2CITY", 90, "Contact2City")]
        public string Contact2City
        {
            get
            {
                return this._Contact2City;
            }
            set
            {
                this.SetString("Contact 2 City", 60, value, ref this._Contact2City);
            }
        }

        [QueryWriterField("Contact 2 Country")]
        [EntityImportExport("CONTACT2COUNTRY", 105, "Contact2Country")]
        public string Contact2Country
        {
            get
            {
                return this._Contact2Country;
            }
            set
            {
                this.SetString("Contact 2 Country", 30, value, ref this._Contact2Country);
            }
        }

        [QueryWriterField("Contact 2 Email Address")]
        [EntityImportExport("CONTACT2EMAILADDRESS", 130, "Contact2EmailAddress")]
        public string Contact2EmailAddress
        {
            get
            {
                return this._Contact2EmailAddress;
            }
            set
            {
                this.SetString("Contact 2 Email", 30, value, ref this._Contact2EmailAddress);
            }
        }

        [QueryWriterField("Contact 2 Fax")]
        [EntityImportExport("CONTACT2FAX", 100, "Contact2Fax")]
        public string Contact2Fax
        {
            get
            {
                return this._Contact2Fax;
            }
            set
            {
                this.SetString("Contact 2 Fax", 20, value, ref this._Contact2Fax);
            }
        }

        [QueryWriterField("Contact 2 Name")]
        [EntityImportExport("CONTACT2NAME", 190, "Contact2Name")]
        public string Contact2Name
        {
            get
            {
                return this._Contact2Name;
            }
            set
            {
                this.SetString("Contact 2 Name", 30, value, ref this._Contact2Name);
            }
        }

        [QueryWriterField("Contact 2 Phone Mobile")]
        [EntityImportExport("CONTACT2PHONEMOBILE", 130, "Contact2PhoneMobile")]
        public string Contact2PhoneMobile
        {
            get
            {
                return this._Contact2PhoneMobile;
            }
            set
            {
                this.SetString("Contact 2 Phone Mobile", 20, value, ref this._Contact2PhoneMobile);
            }
        }

        [QueryWriterField("Contact 2 Phone Office")]
        [EntityImportExport("CONTACT2PHONEOFFICE", 130, "Contact2PhoneOffice")]
        public string Contact2PhoneOffice
        {
            get
            {
                return this._Contact2PhoneOffice;
            }
            set
            {
                this.SetString("Contact 2 Phone Office", 20, value, ref this._Contact2PhoneOffice);
            }
        }

        [QueryWriterField("Contact 2 State")]
        [EntityImportExport("CONTACT2STATE", 100, "Contact2State")]
        public string Contact2State
        {
            get
            {
                return this._Contact2State;
            }
            set
            {
                this.SetString("Contact 2 State", 20, value, ref this._Contact2State);
            }
        }

        [QueryWriterField("Contact 2 Zip")]
        [EntityImportExport("CONTACT2ZIP", 70, "Contact2Zip")]
        public string Contact2Zip
        {
            get
            {
                return this._Contact2Zip;
            }
            set
            {
                this.SetString("Contact 2 Zip", 11, value, ref this._Contact2Zip);
            }
        }

        [QueryWriterField("Country")]
        [EntityImportExport("COUNTRY", 70, "COUNTRY")]
        public string Country
        {
            get
            {
                return this._Country;
            }
            set
            {
                this.SetString("Country", 30, value, ref this._Country);
            }
        }

        public AlarmAndEventLogClass CreditAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyCreditAlarmDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("Credit OK")]
        [EntityImportExport("CREDITOK", 70, "CREDITOK")]
        public bool CreditOK
        {
            get
            {
                return this._CreditOK;
            }
            set
            {
                this._CreditOK = value;
            }
        }

        [QueryWriterField("Bill To Type ID", "CustomerBillToTypeApplicationString.ID", false)]
        [EntityImportExport("CUSTOMERBILLTOTYPEID", 135, "CUSTOMERBILLTOTYPEID")]
        public string CustomerBillToTypeID
        {
            get
            {
                return this._CustomerBillToTypeID;
            }
            set
            {
                this._CustomerBillToTypeID = value;
            }
        }

        [QueryWriterField("Ship To Type ID", "CustomerShipToTypeApplicationString.ID", false)]
        [EntityImportExport("CUSTOMERSHIPTOTYPEID", 135, "CUSTOMERSHIPTOTYPEID")]
        public string CustomerShipToTypeID
        {
            get
            {
                return this._CustomerShipToTypeID;
            }
            set
            {
                this._CustomerShipToTypeID = value;
            }
        }

        [QueryWriterField("Delivery To Terminal Permitted")]
        [EntityImportExport("DELIVERYTOTERMINALPERMITTED", 175, "DELIVERYTOTERMINALPERMITTED")]
        public bool DeliveryToTerminalPermitted
        {
            get
            {
                return this._DeliveryToTerminalPermitted;
            }
            set
            {
                this._DeliveryToTerminalPermitted = value;
            }
        }

        [QueryWriterField("Disable Bill To Allocations Check")]
        [EntityImportExport("DISABLEBILLTOALLOCATIONSCHECK", 75, "DISABLEBILLTOALLOCATIONSCHECK")]
        public bool DisableBillToAllocationsCheck
        {
            get
            {
                return this._DisableBillToAllocationsCheck;
            }
            set
            {
                this._DisableBillToAllocationsCheck = value;
            }
        }

        [QueryWriterField("Disable Owner Allocations Check")]
        [EntityImportExport("DISABLEOWNERALLOCATIONSCHECK", 75, "DISABLEOWNERALLOCATIONSCHECK")]
        public bool DisableOwnerAllocationsCheck
        {
            get
            {
                return this._DisableOwnerAllocationsCheck;
            }
            set
            {
                this._DisableOwnerAllocationsCheck = value;
            }
        }

        [QueryWriterField("Disable Ship To Allocations Check")]
        [EntityImportExport("DISABLESHIPTOALLOCATIONSCHECK", 75, "DISABLESHIPTOALLOCATIONSCHECK")]
        public bool DisableShipToAllocationsCheck
        {
            get
            {
                return this._DisableShipToAllocationsCheck;
            }
            set
            {
                this._DisableShipToAllocationsCheck = value;
            }
        }

        [QueryWriterField("Disable Shipper Allocations Check")]
        [EntityImportExport("DISABLESHIPPERALLOCATIONSCHECK", 75, "DISABLESHIPPERALLOCATIONSCHECK")]
        public bool DisableShipperAllocationsCheck
        {
            get
            {
                return this._DisableShipperAllocationsCheck;
            }
            set
            {
                this._DisableShipperAllocationsCheck = value;
            }
        }

        [QueryWriterField("EPA Number")]
        [EntityImportExport("EPANUMBER", 70, "EPANUMBER")]
        public string EPANumber
        {
            get
            {
                return this._EPANumber;
            }
            set
            {
                this.SetString("EPA Number", 20, value, ref this._EPANumber);
            }
        }

        [XmlIgnore]
        public string EffectiveDate
        {
            get
            {
                return this._EffectiveDate.ToString();
            }
            set
            {
                this.SetDate("Effective Date", value, ref this._EffectiveDate);
            }
        }

        [QueryWriterField("Effective Date", "EffectiveDate")]
        [EntityImportExport("EFFECTIVE DATE", 92, "EFFECTIVEDATETIME")]
        public Date EffectiveDateTime
        {
            get
            {
                return this._EffectiveDate;
            }
            set
            {
                this._EffectiveDate = value;
            }
        }

        [QueryWriterField("Emergency Contact")]
        [EntityImportExport("EMERGENCY CONTACT", 115, "EMERGENCYCONTACT")]
        public string EmergencyContact
        {
            get
            {
                return this._EmergencyContact;
            }
            set
            {
                this.SetString("Emergency Contact", 30, value, ref this._EmergencyContact);
            }
        }

        [QueryWriterField("Emergency Phone")]
        [EntityImportExport("EMERGENCY PHONE", 105, "EMERGENCYPHONE")]
        public string EmergencyPhone
        {
            get
            {
                return this._EmergencyPhone;
            }
            set
            {
                this.SetString("Emergency Phone", 20, value, ref this._EmergencyPhone);
            }
        }

        [XmlIgnore]
        public override ENTITY_TYPE EntityType => ENTITY_TYPE.COMPANY;

        [XmlIgnore]
        public string ExpirationDate
        {
            get
            {
                return this._ExpirationDate.ToString();
            }
            set
            {
                this.SetDate("Expiration Date", value, ref this._ExpirationDate);
            }
        }

        [QueryWriterField("Expiration Date", "ExpirationDate")]
        [EntityImportExport("EXPIRATION DATE", 92, "EXPIRATIONDATE")]
        public Date ExpirationDateTime
        {
            get
            {
                return this._ExpirationDate;
            }
            set
            {
                this._ExpirationDate = value;
            }
        }

        [QueryWriterField("Fax")]
        [EntityImportExport("FAX", 70, "FAX")]
        public string Fax
        {
            get
            {
                return this._Fax;
            }
            set
            {
                this.SetString("Fax", 20, value, ref this._Fax);
            }
        }

        [QueryWriterField("Federal ID")]
        [EntityImportExport("FEDERALID", 70, "FEDERALID")]
        public string FederalID
        {
            get
            {
                return this._FederalID;
            }
            set
            {
                this.SetString("Federal ID", 20, value, ref this._FederalID);
            }
        }

        [QueryWriterField("Federal ID2")]
        [EntityImportExport("FEDERALID2", 70, "FEDERALID2")]
        public string FederalID2
        {
            get
            {
                return this._FederalID2;
            }
            set
            {
                this.SetString("Federal ID2", 20, value, ref this._FederalID2);
            }
        }

        [QueryWriterField("Federal ID3")]
        [EntityImportExport("FEDERALID3", 70, "FEDERALID3")]
        public string FederalID3
        {
            get
            {
                return this._FederalID3;
            }
            set
            {
                this.SetString("Federal ID3", 20, value, ref this._FederalID3);
            }
        }

        [QueryWriterField("Federal ID4")]
        [EntityImportExport("FEDERALID4", 70, "FEDERALID4")]
        public string FederalID4
        {
            get
            {
                return this._FederalID4;
            }
            set
            {
                this.SetString("Federal ID4", 20, value, ref this._FederalID4);
            }
        }

        [QueryWriterField("Federal ID5")]
        [EntityImportExport("FEDERALID5", 70, "FEDERALID5")]
        public string FederalID5
        {
            get
            {
                return this._FederalID5;
            }
            set
            {
                this.SetString("Federal ID5", 20, value, ref this._FederalID5);
            }
        }

        [QueryWriterField("State ID")]
        [EntityImportExport("STATELID", 70, "STATEID")]
        public string StateID
        {
            get
            {
                return this._StateID;
            }
            set
            {
                this.SetString("State ID", 20, value, ref this._StateID);
            }
        }


        [QueryWriterField("Flight Prefix")]
        [EntityImportExport("FLIGHT PREFIX", 84, "FLIGHTPREFIX")]
        public string FlightPrefix
        {
            get
            {
                return this._FlightPrefix;
            }
            set
            {
                this.SetString("FlightPrefix", 5, value, ref this._FlightPrefix);
            }
        }

        [QueryWriterField("Flush Permitted")]
        [EntityImportExport("FLUSHPERMITTED", 95, "FLUSHPERMITTED")]
        public bool FlushPermitted
        {
            get
            {
                return this._FlushPermitted;
            }
            set
            {
                this._FlushPermitted = value;
            }
        }

        [QueryWriterField("Hazardous Material Exclusion")]
        [EntityImportExport("HAZARDOUSMATERIALEXCLUSION", 175, "HAZARDOUSMATERIALEXCLUSION")]
        public bool HazardousMaterialExclusion
        {
            get
            {
                return this._HazardousMaterialExclusion;
            }
            set
            {
                this._HazardousMaterialExclusion = value;
            }
        }

        [QueryWriterField("Delivery Location", "tblIATA.IATAID", false)]
        [EntityImportExport("IATAID", 65, "IATAID")]
        public string IATAID
        {
            get
            {
                return this._IATAID;
            }
            set
            {
                this._IATAID = value;
            }
        }

        [QueryWriterField("ID", "tblCompanies.ID", false)]
        [EntityImportExport("COMPANYID*", 105, "ID")]
        public override string ID
        {
            get
            {
                return this._ID;
            }
            set
            {
                string temp = value;

                if (string.IsNullOrEmpty(temp) == false)
                {
                    temp = temp.Trim();
                }

                this.SetString("ID", 100, temp, ref this._ID);
            }
        }

        public AlarmAndEventLogClass InactiveAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyInactiveAlarmDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        public AlarmAndEventLogClass InactiveStationAlarm(string driverId, string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CompanyInactiveAlarmDescriptor)
            {
                AssociatedData = this.ID + " - " + driverId + " - " + stationId
            };
            return alarmAndEventLog;
        }

        [QueryWriterField("Insurance Company")]
        [EntityImportExport("INSURANCECOMPANY", 113, "INSURANCECOMPANY")]
        public string InsuranceCompany
        {
            get
            {
                return this._InsuranceCompany;
            }
            set
            {
                this.SetString("Insurance Company", 20, value, ref this._InsuranceCompany);
            }
        }

        [XmlIgnore]
        public string InsuranceExpiration
        {
            get
            {
                return this._InsuranceExpiration.ToString();
            }
            set
            {
                this.SetDate("Insurance Expiration Date", value, ref this._InsuranceExpiration);
            }
        }

        [QueryWriterField("Insurance Expiration Date", "InsuranceExpiration")]
        [EntityImportExport("INSURANCEEXPIRATION", 123, "INSURANCEEXPIRATION")]
        public Date InsuranceExpirationDateTime
        {
            get
            {
                return this._InsuranceExpiration;
            }
            set
            {
                this._InsuranceExpiration = value;
            }
        }

        public bool InsuranceExpired => this._InsuranceExpiration.IsTodayOrBefore;

        public AlarmAndEventLogClass InsuranceExpiredAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyInsuranceExpiredAlarmDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("Insurance Policy")]
        [EntityImportExport("INSURANCEPOLICY", 101, "INSURANCEPOLICY")]
        public string InsurancePolicy
        {
            get
            {
                return this._InsurancePolicy;
            }
            set
            {
                this.SetString("Insurance Policy", 20, value, ref this._InsurancePolicy);
            }
        }

        public AlarmAndEventLogClass InsuranceWarningEvent
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyInsuranceWarningEventDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [XmlIgnore]
        public string LastActivityDate
        {
            get
            {
                return this._LastActivityDate.ToString();
            }
            set
            {
                this.SetDateAndTime("Last Activity Date", value, ref this._LastActivityDate);
            }
        }

        [QueryWriterField("Last Activity Date", "LastActivityDate")]
        [EntityImportExport("LASTACTIVITYDATE", 120, "LASTACTIVITYDATE")]
        public DateAndTime LastActivityDateObject
        {
            get
            {
                return this._LastActivityDate;
            }
            set
            {
                this._LastActivityDate = value;
            }
        }

        public string LiabilityAmount
        {
            get
            {
                return this._LiabilityAmount.ToString();
            }
            set
            {
                this.SetDecimal("Liability Amount", value, ref this._LiabilityAmount);
            }
        }

        [QueryWriterField("Liability Amount", "LiabilityAmount")]
        [EntityImportExport("LIABILITYAMOUNT", 99, "LIABILITYAMOUNT")]
        [XmlIgnore]
        public decimal LiabilityAmountDouble
        {
            get
            {
                return this._LiabilityAmount.Value;
            }
            private set
            {
                ;
            }
        }

        [XmlIgnore]
        public string LicenseExpiration
        {
            get
            {
                return this._LicenseExpiration.ToString();
            }
            set
            {
                this.SetDate("License Expiration Date", value, ref this._LicenseExpiration);
            }
        }

        [QueryWriterField("License Expiration Date", "LicenseExpiration")]
        [EntityImportExport("LICENSEEXPIRATIONDATE", 109, "LICENSEEXPIRATIONDATE")]
        public Date LicenseExpirationDate
        {
            get
            {
                return this._LicenseExpiration;
            }
            set
            {
                this._LicenseExpiration = value;
            }
        }

        public bool LicenseExpired => this._LicenseExpiration.IsTodayOrBefore;

        public AlarmAndEventLogClass LicenseExpiredAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyLicenseExpiredAlarmDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("License Number")]
        [EntityImportExport("LICENSENUMBER", 92, "LICENSENUMBER")]
        public string LicenseNumber
        {
            get
            {
                return this._LicenseNumber;
            }
            set
            {
                this.SetString("License Number", 20, value, ref this._LicenseNumber);
            }
        }

        public AlarmAndEventLogClass LicenseWarningEvent
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyLicenseWarningEventDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("Load Rack Display Text")]
        [EntityImportExport("LOADRACKDISPLAYTEXT", 105, "LOADRACKDISPLAYTEXT")]
        public string LoadRackDisplayText
        {
            get
            {
                return this._LoadRackDisplayText;
            }
            set
            {
                this.SetString("Load Rack Display Text", 30, value, ref this._LoadRackDisplayText);
            }
        }

        public AlarmAndEventLogClass LockOutEvent
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyLockOutEventDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("Locked Out")]
        [EntityImportExport("LOCKEDOUT", 70, "LOCKEDOUT")]
        public bool LockedOut
        {
            get
            {
                return this._LockedOut;
            }
            set
            {
                this._LockedOut = value;
            }
        }

        /// <summary>
        /// Represents the date + time that this company was hidden
        /// A null value indicates the company is not hidden.
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

        public AlarmAndEventLogClass LockedOutAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyLockedOutAlarmDescriptor) { AssociatedData = this.ID };
                return alarmAndEventLog;
            }
        }

        public AlarmAndEventLogClass LockedOutStationAlarm(string driverId, string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CompanyLockedOutAlarmDescriptor)
            {
                AssociatedData = this.ID + " - " + driverId + " - " + stationId
            };
            return alarmAndEventLog;
        }

        [XmlIgnore]
        public string LockedOutDate
        {
            get
            {
                return this._LockedOutDate.ToString();
            }
            set
            {
                this.SetDate("Locked Out Date", value, ref this._LockedOutDate);
            }
        }

        [QueryWriterField("Locked Out Date", "LockedOutDate")]
        [EntityImportExport("LOCKEDOUTDATETIME", 93, "LOCKEDOUTDATETIME")]
        public Date LockedOutDateTime
        {
            get
            {
                return this._LockedOutDate;
            }
            set
            {
                this._LockedOutDate = value;
            }
        }

        [QueryWriterField("Locked Out Reason")]
        [EntityImportExport("LOCKEDOUTREASON", 107, "LOCKEDOUTREASON")]
        public string LockedOutReason
        {
            get
            {
                return this._LockedOutReason;
            }
            set
            {
                this.SetString("Locked Out Reason", 80, value, ref this._LockedOutReason);
            }
        }

        [QueryWriterField("Low Stock Warning")]
        [EntityImportExport("LOWSTOCKWARNING", 111, "LOWSTOCKWARNING")]
        public double LowStockWarning
        {
            get
            {
                return this._LowStockWarning;
            }
            set
            {
                this._LowStockWarning = value;
            }
        }

        public Guid MasterRecordGuid
        {
            get
            {
                return this._MasterRecordGuid;
            }
            set
            {
                this._MasterRecordGuid = value;
            }
        }

        [QueryWriterField("Maximum Vehicle Weight")]
        [EntityImportExport("MAXIMUMVEHICLEWEIGHT", 137, "MAXIMUMVEHICLEWEIGHT")]
        public double MaximumVehicleWeight
        {
            get
            {
                return this._MaximumVehicleWeight;
            }
            set
            {
                this._MaximumVehicleWeight = value;
            }
        }

        [QueryWriterField("Name", "tblCompanies.Name", false)]
        [EntityImportExport("NAME", 195, "NAME")]
        public string Name
        {
            get
            {
                return this._Name;
            }
            set
            {
                this.SetString("Name", 64, value, ref this._Name);
            }
        }

        [QueryWriterField("Short Name")]
        [EntityImportExport("SHORTNAME", 25, "SHORTNAME")]
        public string ShortName
        {
            get
            {
                return this._ShortName;
            }
            set
            {
                this.SetString("ShortName", 4, value, ref this._ShortName);
            }
        }


        [QueryWriterField("On Hold")]
        [EntityImportExport("ONHOLD", 50, "ONHOLD")]
        public bool OnHold
        {
            get
            {
                return this._OnHold;
            }
            set
            {
                this._OnHold = value;
            }
        }

        [QueryWriterField("PIN Required")]
        [EntityImportExport("PINREQUIRED", 75, "PINREQUIRED")]
        public bool PINRequired
        {
            get
            {
                return this._PINRequired;
            }
            set
            {
                this._PINRequired = value;
            }
        }

        [QueryWriterField("Scully Required")]
        [EntityImportExport("SCULLYREQUIRED", 75, "SCULLYREQUIRED")]
        public bool ScullyRequired
        {
            get
            {
                return this._ScullyRequired;
            }
            set
            {
                this._ScullyRequired = value;
            }
        }

        [XmlIgnore]
        public override ENTITY_TYPE ParentEntityType => ENTITY_TYPE.NONE;

        [QueryWriterField("Phone")]
        [EntityImportExport("PHONE", 70, "PHONE")]
        public string Phone
        {
            get
            {
                return this._Phone;
            }
            set
            {
                this.SetString("Phone", 20, value, ref this._Phone);
            }
        }

        [QueryWriterField("Pickup Flights")]
        [EntityImportExport("PICKUPFLIGHTS", 88, "PICKUPFLIGHTS")]
        public bool PickupFlights
        {
            get
            {
                return this._PickupFlights;
            }
            set
            {
                this._PickupFlights = value;
            }
        }

        [QueryWriterField("Pump Off Permitted")]
        [EntityImportExport("PUMPOFFPERMITTED", 111, "PUMPOFFPERMITTED")]
        public bool PumpOffPermitted
        {
            get
            {
                return this._PumpOffPermitted;
            }
            set
            {
                this._PumpOffPermitted = value;
            }
        }

        [QueryWriterField("Purchase Order Required")]
        [EntityImportExport("PURCHASEORDERREQUIRED", 145, "PURCHASEORDERREQUIRED")]
        public bool PurchaseOrderRequired
        {
            get
            {
                return this._PurchaseOrderRequired;
            }
            set
            {
                this._PurchaseOrderRequired = value;
            }
        }

        [QueryWriterField("Receivable Account")]
        [EntityImportExport("RECEIVABLEACCOUNT", 116, "RECEIVABLEACCOUNT")]
        public string ReceivableAccount
        {
            get
            {
                return this._ReceivableAccount;
            }
            set
            {
                this.SetString("Receivable Account", 20, value, ref this._ReceivableAccount);
            }
        }

        [QueryWriterField("Refiner Code")]
        [EntityImportExport("REFINERCODE", 75, "REFINERCODE")]
        public string RefinerCode
        {
            get
            {
                return this._RefinerCode;
            }
            set
            {
                this.SetString("Refiner Code", 20, value, ref this._RefinerCode);
            }
        }

        [QueryWriterField("SCAC Code")]
        [EntityImportExport("SCACCODE", 70, "SCACCODE")]
        public string SCACCode
        {
            get
            {
                return this._SCACCode;
            }
            set
            {
                this.SetString("SCAC Code", 4, value, ref this._SCACCode);
            }
        }

        [QueryWriterField("Shipper Type ID", "ShipperTypeApplicationString.ID", false)]
        [EntityImportExport("SHIPPERTYPEID", 96, "SHIPPERTYPEID")]
        public string ShipperTypeID
        {
            get
            {
                return this._ShipperTypeID;
            }
            set
            {
                this._ShipperTypeID = value;
            }
        }

        [EntityImportExport("SITE*", 105, "SITEGUID")]
        public new Guid SiteGuid
        {
            get
            {
                return this._SiteGuid;
            }
            set
            {
                this._SiteGuid = value;
            }
        }

        [QueryWriterField("State")]
        [EntityImportExport("STATE", 70, "STATE")]
        public string State
        {
            get
            {
                return this._State;
            }
            set
            {
                this.SetString("State", 20, value, ref this._State);
            }
        }

        [QueryWriterField("Stock Track")]
        [EntityImportExport("STOCKTRACK", 75, "STOCKTRACK")]
        public bool StockTrack
        {
            get
            {
                return this._StockTrack;
            }
            set
            {
                this._StockTrack = value;
            }
        }

        [QueryWriterField("Suffer Loss Gain")]
        [EntityImportExport("SUFFERLOSSGAIN", 96, "SUFFERLOSSGAIN")]
        public bool SufferLossGain
        {
            get
            {
                return this._SufferLossGain;
            }
            set
            {
                this._SufferLossGain = value;
            }
        }

        [QueryWriterField("Tax Number")]
        [EntityImportExport("TAXNUMBER", 70, "TAXNUMBER")]
        public string TaxNumber
        {
            get
            {
                return this._TaxNumber;
            }
            set
            {
                this.SetString("Tax Number", 20, value, ref this._TaxNumber);
            }
        }

        public AlarmAndEventLogClass UnauthorizedCarrierAlarm
        {
            get
            {
                var alarmAndEventLog = new AlarmAndEventLogClass(CompanyUnauthorizedCarrierAlarmDescriptor)
                {
                    AssociatedData = this.ID
                };
                return alarmAndEventLog;
            }
        }

        [QueryWriterField("User Data 1", "tblCompanies.UserData1")]
        [EntityImportExport("USERDATA1", 75, "USERDATA1")]
        public string UserData1
        {
            get
            {
                return this.UserData[0];
            }
            set
            {
                this.UserData[0] = value;
            }
        }

        [QueryWriterField("User Data 2", "tblCompanies.UserData2")]
        [EntityImportExport("USERDATA2", 75, "USERDATA2")]
        public string UserData2
        {
            get
            {
                return this.UserData[1];
            }
            set
            {
                this.UserData[1] = value;
            }
        }

        [QueryWriterField("User Data 3", "tblCompanies.UserData3")]
        [EntityImportExport("USERDATA3", 75, "USERDATA3")]
        public string UserData3
        {
            get
            {
                return this.UserData[2];
            }
            set
            {
                this.UserData[2] = value;
            }
        }

        [QueryWriterField("User Data 4", "tblCompanies.UserData4")]
        [EntityImportExport("USERDATA4", 75, "USERDATA4")]
        public string UserData4
        {
            get
            {
                return this.UserData[3];
            }
            set
            {
                this.UserData[3] = value;
            }
        }

        [QueryWriterField("User Data 5", "tblCompanies.UserData5")]
        [EntityImportExport("USERDATA5", 75, "USERDATA5")]
        public string UserData5
        {
            get
            {
                return this.UserData[4];
            }
            set
            {
                this.UserData[4] = value;
            }
        }

        [QueryWriterField("User Data 6", "tblCompanies.UserData6")]
        [EntityImportExport("USERDATA6", 75, "USERDATA6")]
        public string UserData6
        {
            get
            {
                return this.UserData[5];
            }
            set
            {
                this.UserData[5] = value;
            }
        }

        [QueryWriterField("User Data 7", "tblCompanies.UserData7")]
        [EntityImportExport("USERDATA7", 75, "USERDATA7")]
        public string UserData7
        {
            get
            {
                return this.UserData[6];
            }
            set
            {
                this.UserData[6] = value;
            }
        }

        [QueryWriterField("User Data 8", "tblCompanies.UserData8")]
        [EntityImportExport("USERDATA8", 75, "USERDATA8")]
        public string UserData8
        {
            get
            {
                return this.UserData[7];
            }
            set
            {
                this.UserData[7] = value;
            }
        }

        [QueryWriterField("Weight Units")]
        [EntityImportExport("WEIGHTUNITS", 75, "WEIGHTUNITS")]
        public EngineeringUnit WeightUnits
        {
            get
            {
                return this._WeightUnits;
            }
            set
            {
                this._WeightUnits = value;
            }
        }

        [QueryWriterField("Zip")]
        [EntityImportExport("ZIP", 70, "ZIP")]
        public string Zip
        {
            get
            {
                return this._Zip;
            }
            set
            {
                this.SetString("Zip", 11, value, ref this._Zip);
            }
        }

        [QueryWriterField("Consortium Type", "ConsortiumType", false)]
        [EntityImportExport("ConsortiumTypeIndex", 120, "ConsortiumTypeIndex")]
        public ConsortiumTypes? ConsortiumType
        {
            get
            {
                return this._ConsortiumType;
            }
            set
            {
                this._ConsortiumType = value;
            }
        }

        [QueryWriterField("Company IATA Code", "CompanyIATACode")]
        [EntityImportExport("CompanyIATACode", 50, "CompanyIATACode")]
        public string CompanyIataCode
        {
            get { return this.companyIataCode; }
            set { this.companyIataCode = value; }
        }

        [QueryWriterField("Company ICAO Code", "CompanyICAOCode")]
        [EntityImportExport("CompanyICAOCode", 50, "CompanyICAOCode")]
        public string CompanyIcaoCode
        {
            get { return this.companyIcaoCode; }
            set { this.companyIcaoCode = value; }
        }
        #endregion

        #region Explicit Interface Properties

        AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
        {
            get
            {
                AlarmAndEventDescriptorClass[] descriptors =
                {
                    CompanyLockOutEventDescriptor,
                    CompanyLockedOutAlarmDescriptor,
                    CompanyInactiveAlarmDescriptor,
                    CompanyAccessScheduleAlarmDescriptor,
                    CompanyCreditAlarmDescriptor,
                    CompanyLicenseWarningEventDescriptor,
                    CompanyLicenseExpiredAlarmDescriptor,
                    CompanyInsuranceWarningEventDescriptor,
                    CompanyInsuranceExpiredAlarmDescriptor,
                    CompanyUnauthorizedCarrierAlarmDescriptor,
                    CompanyHazardousMaterialExclusionEventDescriptor,
                    AuditLogEntityAddedEventDescriptor,
                    AuditLogEntityModifiedEventDescriptor,
                    AuditLogEntityPurgedEventDescriptor,
                    AuditLogDatabaseTraceViewedEventDescriptor
                };
                return descriptors;
            }
        }

        #endregion

        #region Data dictionary
        string[] IDataDictionary.Keys(SecurityClass security)
        {
            string[] keys =
            {
                EntityAddedKey,
                EntityModifiedKey,
                EntityPurgedKey
            };

            return keys;
        }
        #endregion

        #region Public Methods and Operators

        public string DetailPageReference()
        {
            return "FMWebApp\\CompanyForm.aspx";
        }

        public void EnumerateAllRoles(SqlCommand cmd, SecurityClass security, bool byGroupCompanies)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                + "SELECT tblCompanies.[CompanyGuid], tblCompanies.[_MasterRecordGuid], tblCompanies.[ID], tblCompanies.[Code], LookupCompanyRoleIndex"
                + " FROM tblCompanies, map.tblCompanyToRole";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                   : "") + " AND map.tblCompanyToRole.CompanyGuid = tblCompanies._MasterRecordGuid"
                               + " ORDER BY tblCompanies.ID";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateAuthorizedCustomerShipToForColumnValueSQL(
            SqlCommand cmd,
            SecurityClass security,
            string column,
            string value,
            Guid carrierGuid)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause + " FROM tblCompanies,map.tblCompanyToRole" + " WHERE"
                              + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                              + " AND tblCompanies.CompanyGuid IN"
                              + " (SELECT AssignedToCompanyGuid FROM map.tblCompanyAuthorizedCarrierToCompany WHERE CompanyGuid = @CarrierGuid)"
                              + " AND tblCompanies.CompanyGuid IN" + " (SELECT CompanyGuid FROM map.tblCompanyShipToToBillTo)"
                              + " AND map.tblCompanyToRole.CompanyGuid = tblCompanies._MasterRecordGuid"
                              + " AND map.tblCompanyToRole.LookupCompanyRoleIndex = @CustomerShipToType ";

            if ((string.IsNullOrEmpty(column) == false && string.IsNullOrEmpty(value) == false))
            {
                cmd.CommandText += " AND tblCompanies." + column + " = @Value";

                cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 100);

                cmd.Parameters["@Value"].Value = value;
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";

            cmd.Parameters.Add("@CarrierGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@CustomerShipToType", SqlDbType.Int);

            cmd.Parameters["@CarrierGuid"].Value = carrierGuid;
            cmd.Parameters["@CustomerShipToType"].Value = ((int)COMPANY_ROLE.CUSTOMER_SHIPTO);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateAuthorizedSupplierForColumnValueSQL(SqlCommand cmd, SecurityClass security, string column)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + "SELECT DISTINCT tblCompanies." + column + " FROM tblCompanies,map.tblCompanyToRole" + " WHERE"
                              + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                              + " AND tblCompanies.CompanyGuid IN" + " (SELECT CompanyGuid FROM map.tblCompanySupplierToOwner)"
                              + " AND map.tblCompanyToRole.CompanyGuid = tblCompanies._MasterRecordGuid"
                              + " AND map.tblCompanyToRole.LookupCompanyRoleIndex = @Role AND map.tblCompanyToRole.SiteGuid = @TargetSiteGuid"
                              + " ORDER BY tblCompanies." + column;

            cmd.Parameters.Add("@Role", SqlDbType.Int);
            cmd.Parameters["@Role"].Value = ((int)COMPANY_ROLE.SUPPLIER);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateAuthorizedSupplierForColumnValueSQL(
            SqlCommand cmd,
            SecurityClass security,
            string column,
            string value)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause + " FROM tblCompanies,map.tblCompanyToRole" + " WHERE"
                              + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                              + " AND tblCompanies.CompanyGuid IN" + " (SELECT CompanyGuid FROM map.tblCompanySupplierToOwner)"
                              + " AND map.tblCompanyToRole.CompanyGuid = tblCompanies._MasterRecordGuid"
                              + " AND map.tblCompanyToRole.LookupCompanyRoleIndex = @SupplierType";

            if ((string.IsNullOrEmpty(column) == false && string.IsNullOrEmpty(value) == false))
            {
                cmd.CommandText += " AND tblCompanies." + column + " = @Value";

                cmd.Parameters.Add("@Value", SqlDbType.NVarChar, 100);

                cmd.Parameters["@Value"].Value = value;
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";
            cmd.Parameters.Add("@SupplierType", SqlDbType.Int);
            cmd.Parameters["@SupplierType"].Value = ((int)COMPANY_ROLE.SUPPLIER);
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateByCodeSQL(SqlCommand cmd, SecurityClass security)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause + " FROM tblCompanies" + " WHERE"
                              + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid") + " AND tblCompanies.Code = @Code"
                              + " ORDER BY tblCompanies.ID";

            cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 10);
            cmd.Parameters["@Code"].Value = this.Code;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        /// <summary>
        ///     This method will enumerate a list of companies and return only the company IDs.
        ///     If true is passed in for the user group to companies association, then a subset
        ///     of the company IDs are returned.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="security"></param>
        /// <param name="byGroupCompanies"></param>
        /// <returns></returns>
        public void EnumerateByCompanyIDsSQL(SqlCommand cmd, SecurityClass security, bool byGroupCompanies)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                               + "SELECT tblCompanies.ID " + "FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid "
                                   : "") + " ORDER BY tblCompanies.ID";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        /// <summary>
        ///     This method will return an SQL string that retrieves specific company data for
        ///     the company summary page based on the role and filter.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="security"></param>
        /// <param name="role"></param>
        /// <param name="filter"></param>
        /// <param name="byGroupCompanies"></param>
        /// <param name="limit"></param>
        /// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
        /// <returns></returns>
        public void EnumerateByRoleAndFilterCompanyGridSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            string filter,
            bool byGroupCompanies,
            int limit,
            bool hideHiddenCompanies = false)
        {
            bool hasFilter = false;

            if (limit > 0)
            {
                cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                                  + "SELECT TOP " + limit.ToString() + " ";
            }
            else
            {
                cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                                  + "SELECT ";
            }

            cmd.CommandText += "SiteGuid, CompanyGuid AS IdentityGuid, _MasterRecordGuid, ID, Code, Name, Address1, City, State, tblCompanies.HiddenDate ";

            cmd.CommandText += " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                   : "");

            if (role != COMPANY_ROLE.MAX_COMPANY_ROLE)
            {
                cmd.CommandText += " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WHERE"
                                   + " map.tblCompanyToRole.LookupCompanyRoleIndex = @Role AND map.tblCompanyToRole.SiteGuid = @TargetSiteGuid ) ";

                cmd.Parameters.Add("@Role", SqlDbType.Int);

                cmd.Parameters["@Role"].Value = (int)role;
            }

            if (!string.IsNullOrEmpty(filter))
            {
                cmd.CommandText += " AND (tblCompanies.ID LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Name LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Code LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Address1 LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.City LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.State LIKE(UPPER(@SearchFilter))"
                                   + ")";

                hasFilter = true;
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText += " AND tblCompanies.HiddenDate IS NULL";
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";

            if (hasFilter)
            {
                string searchFilter = "%" + filter + "%";
                searchFilter = searchFilter.ToUpper();

                cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 256);
                cmd.Parameters["@SearchFilter"].Value = searchFilter;
            }

            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        /// <summary>
        ///     This method will a SQL that only retrieves companies by role. The select clause only
        ///     returns CompanyID, CompanyGuid, CompanyCode, CompanyName, CompanyAddress1, CompanyAddress2,
        ///     CompanyState, and CompanyCity.  This SQL query is used by the Company Select form when a Role
        ///     is supplied and is based on the user's filter.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="security"></param>
        /// <param name="role"></param>
        /// <param name="filter"></param>
        /// <param name="loadTypes"></param>
        /// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
        /// <returns></returns>
        public void EnumerateByRoleAndFilterCompanySelectSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            string filter,
            bool loadTypes,
            bool hideHiddenCompanies = false)
        {
            bool hasFilter = false;

            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectCompanySelectRoleClause;

            if (loadTypes)
            {
                cmd.CommandText +=
                    ",(SELECT ID FROM tblApplicationString WHERE tblCompanies.ShipperTypeGuid = tblApplicationString.ApplicationStringGuid) AS ShipperTypeID,"
                    + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.CustomerBillToTypeGuid = tblApplicationString.ApplicationStringGuid) AS CustomerBillToTypeID,"
                    + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.CustomerShipToTypeGuid = tblApplicationString.ApplicationStringGuid) AS CustomerShipToTypeID";
            }

            cmd.CommandText += " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, false);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + ("");

            if (role != COMPANY_ROLE.MAX_COMPANY_ROLE)
            {
                cmd.CommandText += " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WHERE"
                                   + " map.tblCompanyToRole.LookupCompanyRoleIndex = @Role ) ";
            }

            if (!string.IsNullOrEmpty(filter))
            {
                cmd.CommandText += " AND (tblCompanies.ID LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Name LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Code LIKE(UPPER(@SearchFilter)))";

                hasFilter = true;
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText += " AND tblCompanies.HiddenDate IS NULL";
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";

            if (hasFilter)
            {
                cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 256);
                cmd.Parameters["@SearchFilter"].Value = "%" + filter + "%";
            }

            cmd.Parameters.Add("@Role", SqlDbType.Int);
            cmd.Parameters["@Role"].Value = (int)role;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateByRoleAndFilterCompanySelectSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            string filter)
        {
            this.EnumerateByRoleAndFilterCompanySelectSQL(cmd, security, role, filter, false);
        }

        public void EnumerateByRoleAndFilterSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            string filter,
            bool byGroupCompanies)
        {
            bool hasFilter = false;
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause;

            cmd.CommandText += " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                   : "");

            if (role != COMPANY_ROLE.MAX_COMPANY_ROLE)
            {
                cmd.CommandText += " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WHERE"
                                   + " map.tblCompanyToRole.LookupCompanyRoleIndex = @Role AND map.tblCompanyToRole.SiteGuid = @TargetSiteGuid) ";

                cmd.Parameters.Add("@Role", SqlDbType.Int);

                cmd.Parameters["@Role"].Value = (int)role;
            }

            if (!string.IsNullOrEmpty(filter))
            {
                cmd.CommandText += " AND (tblCompanies.ID LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Name LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Code LIKE(UPPER(@SearchFilter)))";

                hasFilter = true;
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";

            if (hasFilter)
            {
                string searchFilter = "%" + filter + "%";
                searchFilter = searchFilter.ToUpper();

                cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 256);
                cmd.Parameters["@SearchFilter"].Value = searchFilter;
            }
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        /// <summary>
        ///     This method will return an enumeration of company data by role. It purpose is to only
        ///     return specific information for the company grid (performance).
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="security"></param>
        /// <param name="role"></param>
        /// <param name="byGroupCompanies"></param>
        /// <param name="limit"></param>
        /// <param name="hideHiddenCompanies">If true, only companies that are not marked as hidden will be returned</param>
        /// <returns></returns>
        public void EnumerateByRoleCompanyGridSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            bool byGroupCompanies,
            int limit,
            bool hideHiddenCompanies = false)
        {
            if (limit > 0)
            {
                cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                                  + "SELECT TOP " + limit + " ";
            }
            else
            {
                cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                                  + "SELECT ";
            }

            cmd.CommandText += "SiteGuid, CompanyGuid AS IdentityGuid, _MasterRecordGuid, ID, Code, Name, Address1, City, State, tblCompanies.HiddenDate "
                               + " FROM tblCompanies WITH(NOLOCK)";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE " + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                   : "");

            if (role != COMPANY_ROLE.MAX_COMPANY_ROLE)
            {
                cmd.CommandText +=
                    " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WITH(NOLOCK) WHERE"
                    + " map.tblCompanyToRole.LookupCompanyRoleIndex = @Role AND map.tblCompanyToRole.SiteGuid = @TargetSiteGuid )";

                cmd.Parameters.Add("@Role", SqlDbType.Int);
                cmd.Parameters["@Role"].Value = (int)role;
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText += " AND tblCompanies.HiddenDate IS NULL";
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";

            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateByRoleGetIDCodeTypesIdentityGuidOnlySQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE[] roles,
            bool hideHiddenCompanies = false)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + SelectIDCodeIdentityGuidOnlyClause + " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, false);

            cmd.CommandText += " WHERE " + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid");

            if (roles != null && roles.Length > 0)
            {
                cmd.CommandText += " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WHERE"
                                   + " map.tblCompanyToRole.SiteGuid = @TargetSiteGuid AND map.tblCompanyToRole.LookupCompanyRoleIndex IN (";

                int item = 0;
                foreach (COMPANY_ROLE role in roles)
                {
                    cmd.CommandText += ((int)role) + ((++item == roles.Length) ? "" : ",");
                }
                cmd.CommandText += "))";
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText += " AND (tblCompanies.HiddenDate IS NULL)";
            }

            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateByRoleSQL(SqlCommand cmd, SecurityClass security, COMPANY_ROLE role, bool byGroupCompanies, bool hideHiddenCompanies)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause + " FROM tblCompanies WITH(NOLOCK)";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE " + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                   : string.Empty);

            if (role != COMPANY_ROLE.MAX_COMPANY_ROLE)
            {
                cmd.CommandText +=
                    " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WITH(NOLOCK) WHERE"
                    + " map.tblCompanyToRole.LookupCompanyRoleIndex = @Role AND map.tblCompanyToRole.SiteGuid = @TargetSiteGuid)";

                cmd.Parameters.Add("@Role", SqlDbType.Int);
                cmd.Parameters["@Role"].Value = (int)role;
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText += " AND tblCompanies.HiddenDate IS NULL";
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        //***************************************************************************************************************************
        // This method will return an enumerated list of companies using the security, role, filter and by group companies
        // criterion.  This method is the same as the EnumerateByRoleSQL with the exception that is has a filter parameter
        // that the user populates in order to only find companies that contain their search criterion.
        //***************************************************************************************************************************

        public void EnumerateColumnForAuthorizedCustomerShipToSQL(
            SqlCommand cmd,
            SecurityClass security,
            Guid carrierGuid,
            string column)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + "SELECT DISTINCT a." + column + " FROM tblCompanies a" + " INNER JOIN map.tblCompanyToRole b"
                              + " ON b.CompanyGuid = a._MasterRecordGuid"
                              + //CompanyRoles are managed by MasterRecordGuid, seperately from Record Versioning.
                              " AND b.SiteGuid = @TargetSiteGuid" + " WHERE" + this.AppendSiteWhereClause(security, "a", "CompanyGuid")
                              + " AND b.LookupCompanyRoleIndex = @Role"
                              + " AND ((@CarrierGuid IS NULL) OR (a.CompanyGuid IN (SELECT AssignedToCompanyGuid FROM map.tblCompanyAuthorizedCarrierToCompany WHERE CompanyGuid = @CarrierGuid)))"
                              + " ORDER BY a." + column;

            cmd.Parameters.Add("@Role", SqlDbType.Int);
            cmd.Parameters["@Role"].Value = ((int)COMPANY_ROLE.CUSTOMER_SHIPTO);

            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;

            cmd.Parameters.Add("@CarrierGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@CarrierGuid"].Value = DBNull.Value;
            if (carrierGuid != Guid.Empty)
            {
                cmd.Parameters["@CarrierGuid"].Value = carrierGuid;
            }
        }

        public void EnumerateForCompanySelectRoleSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            bool loadTypes,
            bool hideHiddenCompanies = false)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectCompanySelectRoleClause;

            if (loadTypes)
            {
                cmd.CommandText +=
                    ",(SELECT ID FROM tblApplicationString WHERE tblCompanies.ShipperTypeApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS ShipperTypeID,"
                    + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.CustomerBillToTypeApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS CustomerBillToTypeID,"
                    + "(SELECT ID FROM tblApplicationString WHERE tblCompanies.CustomerShipToTypeApplicationStringGuid = tblApplicationString.ApplicationStringGuid) AS CustomerShipToTypeID";
            }

            cmd.CommandText += " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, false);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + ("");

            if (role != COMPANY_ROLE.MAX_COMPANY_ROLE)
            {
                cmd.CommandText += " AND tblCompanies._MasterRecordGuid IN (SELECT CompanyGuid FROM map.tblCompanyToRole WHERE"
                                   + " map.tblCompanyToRole.LookupCompanyRoleIndex = @Role)";

                cmd.Parameters.Add("@Role", SqlDbType.Int);
                cmd.Parameters["@Role"].Value = (int)role;
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText += " AND tblCompanies.HiddenDate IS NULL";
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        /// <summary>
        ///     This method returns a SQL that only retrieves companies by role. The select clause only
        ///     returns CompanyID, CompanyGuid, CompanyCode, CompanyName, CompanyAddress1, CompanyAddress2,
        ///     CompanyState, and CompanyCity.  This SQL query is used by the Company Select form when a Role
        ///     is supplied.
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="security"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public void EnumerateForCompanySelectRoleSQL(SqlCommand cmd, SecurityClass security, COMPANY_ROLE role)
        {
            this.EnumerateForCompanySelectRoleSQL(cmd, security, role, false);
        }

        public void EnumerateGroupCompaniesSubQuery(SqlCommand cmd, SecurityClass security, bool byGroupCompanies)
        {
            if (byGroupCompanies)
            {
                if (!cmd.CommandText.Contains(this.CachedCompanyrecordversionGuidTableOperation))
                {
                    cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                                        + cmd.CommandText;
                }

                cmd.CommandText += GroupCompaniesSubQueryClause;

                cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@UserGuid"].Value = security.UserGuid;
            }
        }

        public void EnumerateHierarchialCustomerFromRoleSQL(
            SqlCommand cmd,
            SecurityClass security,
            COMPANY_ROLE role,
            Guid identityGuid,
            string filter,
            bool hideHiddenCompanies = false)
        {
            var selectType = COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
            bool hasFilter = false;

            switch (role)
            {
                case COMPANY_ROLE.OWNER:
                    selectType = COMPANY_MAP_TYPE.LOAD_OWNER_MANAGER_MAP;
                    break;
                case COMPANY_ROLE.SHIPPER:
                    selectType = COMPANY_MAP_TYPE.SHIPPER_OWNER_MAP;
                    break;
                case COMPANY_ROLE.CUSTOMER_BILLTO:
                    selectType = COMPANY_MAP_TYPE.BILLTO_SHIPPER_MAP;
                    break;
                case COMPANY_ROLE.CUSTOMER_SHIPTO:
                    selectType = COMPANY_MAP_TYPE.SHIPTO_BILLTO_MAP;
                    break;
                case COMPANY_ROLE.SUPPLIER:
                    selectType = COMPANY_MAP_TYPE.SUPPLIER_OWNER_MAP;
                    break;
                default:
                    break;
            }

            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause + " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, false);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + ("");

            cmd.CommandText += " AND tblCompanies._MasterRecordGuid IN (SELECT "
                               + CompanyMapClass.GetMappingTableAssignedGuidColumnName(selectType) + " FROM "
                               + CompanyMapClass.GetMappingTableName(selectType) + " WHERE" + " "
                               + CompanyMapClass.GetMappingTableName(selectType) + "."
                               + CompanyMapClass.GetMappingTableAssignedToGuidColumnName(selectType) + " = @IdentityGuid "
                               + " AND " + CompanyMapClass.GetMappingTableName(selectType) + ".SiteGuid = @SiteGuid)";

            if (!string.IsNullOrEmpty(filter))
            {
                cmd.CommandText += " AND (tblCompanies.ID LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Name LIKE(UPPER(@SearchFilter))"
                                   + " OR tblCompanies.Code LIKE(UPPER(@SearchFilter)))";

                hasFilter = true;
            }

            if (hideHiddenCompanies)
            {
                cmd.CommandText = " AND tblCompanies.HiddenDate IS NULL";
            }

            cmd.CommandText += " ORDER BY tblCompanies.ID";

            if (hasFilter)
            {
                string searchFilter = "%" + filter + "%";
                searchFilter = searchFilter.ToUpper();

                cmd.Parameters.Add("@SearchFilter", SqlDbType.NVarChar, 256);
                cmd.Parameters["@SearchFilter"].Value = searchFilter;
            }

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@IdentityGuid"].Value = identityGuid;
            cmd.Parameters["@SiteGuid"].Value = security.SiteGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
        }

        public void EnumerateSQL(SqlCommand cmd, SecurityClass security, bool byGroupCompanies)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + this.selectClause + " FROM tblCompanies";

            this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

            cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                               + (byGroupCompanies
                                   ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                   : "") + " ORDER BY tblCompanies.ID";
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = this.SiteGuid;
        }

        public void EnumerateSQLIDCodeIdentityGuidOnly(SqlCommand cmd, SecurityClass security, bool byGroupCompanies)
        {
            if (byGroupCompanies)
            {
                // use a more optimized query
                EnumerateSQLIDCodeIdentityGuidOnlyByGroupCompanies(cmd, security);
            }
            else
            {
                cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                                      + SelectIDCodeIdentityGuidOnlyClause + " FROM tblCompanies";

                this.EnumerateGroupCompaniesSubQuery(cmd, security, byGroupCompanies);

                cmd.CommandText += " WHERE" + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                                       + (byGroupCompanies
                                           ? " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid"
                                           : "") + " ORDER BY tblCompanies.ID";
                cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
                cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
            }
        }

        private void EnumerateSQLIDCodeIdentityGuidOnlyByGroupCompanies(SqlCommand cmd, SecurityClass security)
        {
            cmd.CommandText = "CREATE TABLE #CompanyGuidTable( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine +
                "CREATE TABLE #AuthorizedCompanies( CompanyGuid uniqueidentifier NULL)" + Environment.NewLine + Environment.NewLine +
                "INSERT INTO #AuthorizedCompanies" + Environment.NewLine +
                "SELECT DISTINCT map.tblCompanyCompanyToUserGroup.CompanyGuid" + Environment.NewLine +
                "FROM map.tblCompanyCompanyToUserGroup" + Environment.NewLine +
                "JOIN map.tblUserToGroup ug" + Environment.NewLine +
                "ON map.tblCompanyCompanyToUserGroup.GroupGuid = ug.GroupGuid" + Environment.NewLine +
                "AND ug.UserGuid = @UserGuid" + Environment.NewLine +
                "JOIN map.tblEntityUserGroupToSite" + Environment.NewLine +
                "ON map.tblEntityUserGroupToSite.GroupGuid = ug.GroupGuid" + Environment.NewLine +
                "AND(map.tblEntityUserGroupToSite.SiteGuid = @TargetSiteGuid)" + Environment.NewLine + Environment.NewLine +
                "IF EXISTS(SELECT 1 FROM #AuthorizedCompanies WHERE CompanyGuid IS NULL )" + Environment.NewLine +
                "BEGIN" + Environment.NewLine +
                "    INSERT INTO #CompanyGuidTable SELECT CompanyGuid FROM [erv].[udf_GetCompanyRecordVersions](@TargetSiteGuid)" + Environment.NewLine +
                "END" + Environment.NewLine +
                "ELSE" + Environment.NewLine +
                "BEGIN" + Environment.NewLine +
                "    INSERT INTO #CompanyGuidTable SELECT CompanyGuid FROM #AuthorizedCompanies" + Environment.NewLine +
                "END" + Environment.NewLine + Environment.NewLine +
                "SELECT DISTINCT a.[ID], " + Environment.NewLine +
                "a.[Code], a.CompanyGuid," + Environment.NewLine +
                "a.[ShipperTypeApplicationStringGuid]," + Environment.NewLine +
                "a.[CustomerBillToTypeApplicationStringGuid]," + Environment.NewLine +
                "a.[CustomerShipToTypeApplicationStringGuid], " + Environment.NewLine +
                "a._MasterRecordGuid,  " + Environment.NewLine +
                "a.[HiddenDate]" + Environment.NewLine +
                "FROM tblCompanies a" + Environment.NewLine +
                "JOIN #CompanyGuidTable b" + Environment.NewLine +
                "ON a.CompanyGuid = b.CompanyGuid" + Environment.NewLine +
                "ORDER BY a.ID" + Environment.NewLine + Environment.NewLine +
                "DROP TABLE #CompanyGuidTable" + Environment.NewLine +
                "DROP TABLE #AuthorizedCompanies";

            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@TargetSiteGuid"].Value = security.SiteGuid;
            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters["@UserGuid"].Value = security.UserGuid;
        }

        public bool HasRole(COMPANY_ROLE role)
        {
            foreach (CompanyRoleMapClass availableRole in this.RoleCollection)
            {
                if (availableRole.Role == role)
                {
                    return true;
                }
            }

            return false;
        }

        public AlarmAndEventLogClass HazardousMaterialExclusionEvent(string productID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CompanyHazardousMaterialExclusionEventDescriptor)
            {
                AssociatedData = this.ID + " - " + productID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass HazardousMaterialExclusionEvent(string productID, string driverID, string station)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CompanyHazardousMaterialExclusionEventDescriptor)
            {
                AssociatedData = this.ID + " - " + productID + " - " + driverID + " - " + station
            };
            return alarmAndEventLog;
        }

        public void InsertSQL(SqlCommand cmd)
        {
            cmd.CommandText = "INSERT INTO tblCompanies " + "(SiteGuid," + "ID," + "Code," + "Name," + "ShortName," + "Address1," + "Address2,"
                              + "City," + "State," + "Zip," + "Country," + "Phone," + "Fax," + "EmergencyContact,"
                              + "EmergencyPhone," + "FlightPrefix," + "EffectiveDate," + "ExpirationDate," + "IATAGuid,"
                              + "OnHold," + "PickupFlights," + "StockTrack," + "SufferLossGain," + "LowStockWarning,"
                              + "LockedOut," + "LockedOutReason," + "LockedOutDate," + "ShipperTypeApplicationStringGuid,"
                              + "CustomerBillToTypeApplicationStringGuid," + "CustomerShipToTypeApplicationStringGuid,"
                              + "ReceivableAccount," + "RefinerCode," + "LastActivityDate," + "CreditOK," + "AdditiveAccounting,"
                              + "PurchaseOrderRequired," + "EPANumber," + "FederalID," + "FederalID2," + "FederalID3," + "FederalID4," + "FederalID5," + "StateID,"
                              + "TaxNumber," + "FlushPermitted,"
                              + "PumpOffPermitted," + "DeliveryToTerminalPermitted," + "LicenseNumber," + "LicenseExpiration,"
                              + "InsuranceCompany," + "InsurancePolicy," + "LiabilityAmount," + "HazardousMaterialExclusion,"
                              + "InsuranceExpiration," + "AllowDriverEntry," + "PINRequired," + "MaximumVehicleWeight,"
                              + "WeightUnits," + "AccountNumber," + "SCACCode," + "Note," + "DisableOwnerAllocationsCheck,"
                              + "DisableShipperAllocationsCheck," + "DisableBillToAllocationsCheck,"
                              + "DisableShipToAllocationsCheck," + "LoadRackDisplayText," + "Contact1Name," + "Contact1Address1,"
                              + "Contact1Address2," + "Contact1City," + "Contact1State," + "Contact1Zip," + "Contact1Country,"
                              + "Contact1PhoneOffice," + "Contact1PhoneMobile," + "Contact1Fax," + "Contact1EmailAddress,"
                              + "Contact2Name," + "Contact2Address1," + "Contact2Address2," + "Contact2City," + "Contact2State,"
                              + "Contact2Zip," + "Contact2Country," + "Contact2PhoneOffice," + "Contact2PhoneMobile,"
                              + "Contact2Fax," + "Contact2EmailAddress," + "HiddenDate," + "UserData1," + "UserData2," + "UserData3,"
                              + "UserData4," + "UserData5," + "UserData6," + "UserData7," + "UserData8," + "CreatedDate,"
                              + "CreatedBy," + "UpdatedDate," + "UpdatedBy," + "CompanyGuid," + "_MasterRecordGuid," + "ScullyRequired," + "ConsortiumTypeIndex, "
                              + "CompanyIATACode, " + "CompanyICAOCode"
                              + ") VALUES (" + "@SiteGuid," + "@ID," + "@Code," + "@Name," + "@ShortName," + "@Address1," + "@Address2,"
                              + "@City," + "@State," + "@Zip," + "@Country," + "@Phone," + "@Fax," + "@EmergencyContact,"
                              + "@EmergencyPhone," + "@FlightPrefix," + "@EffectiveDate," + "@ExpirationDate," + "@IATAGuid,"
                              + "@OnHold," + "@PickupFlights," + "@StockTrack," + "@SufferLossGain," + "@LowStockWarning,"
                              + "@LockedOut," + "@LockedOutReason," + "@LockedOutDate," + "@ShipperTypeApplicationStringGuid,"
                              + "@CustomerBillToTypeApplicationStringGuid," + "@CustomerShipToTypeApplicationStringGuid,"
                              + "@ReceivableAccount," + "@RefinerCode," + "@LastActivityDate," + "@CreditOK,"
                              + "@AdditiveAccounting," + "@PurchaseOrderRequired," + "@EPANumber,"
                              + "@FederalID," + "@FederalID2," + "@FederalID3," + "@FederalID4," + "@FederalID5," + "@StateID,"
                              + "@TaxNumber," + "@FlushPermitted," + "@PumpOffPermitted," + "@DeliveryToTerminalPermitted,"
                              + "@LicenseNumber," + "@LicenseExpiration," + "@InsuranceCompany," + "@InsurancePolicy,"
                              + "@LiabilityAmount," + "@HazardousMaterialExclusion," + "@InsuranceExpiration,"
                              + "@AllowDriverEntry," + "@PINRequired," + "@MaximumVehicleWeight," + "@WeightUnits,"
                              + "@AccountNumber," + "@SCACCode," + "@Note," + "@DisableOwnerAllocationsCheck,"
                              + "@DisableShipperAllocationsCheck," + "@DisableBillToAllocationsCheck,"
                              + "@DisableShipToAllocationsCheck," + "@LoadRackDisplayText," + "@Contact1Name,"
                              + "@Contact1Address1," + "@Contact1Address2," + "@Contact1City," + "@Contact1State,"
                              + "@Contact1Zip," + "@Contact1Country," + "@Contact1PhoneOffice," + "@Contact1PhoneMobile,"
                              + "@Contact1Fax," + "@Contact1EmailAddress," + "@Contact2Name," + "@Contact2Address1,"
                              + "@Contact2Address2," + "@Contact2City," + "@Contact2State," + "@Contact2Zip,"
                              + "@Contact2Country," + "@Contact2PhoneOffice," + "@Contact2PhoneMobile," + "@Contact2Fax,"
                              + "@Contact2EmailAddress," + "@HiddenDate," + "@UserData0," + // 0
                              "@UserData1," + // 1
                              "@UserData2," + // 2
                              "@UserData3," + // 3
                              "@UserData4," + // 4
                              "@UserData5," + // 5
                              "@UserData6," + // 6
                              "@UserData7," + // 7
                              "@CreatedDate," + "@CreatedBy," + "@UpdatedDate," + "@UpdatedBy," + "@CompanyGuid,"
                              + "@MasterRecordGuid," + "@ScullyRequired," + "@ConsortiumTypeIndex, " + "@CompanyIataCode, " + "@CompanyIcaoCode" + ") ";

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@ShortName", SqlDbType.NVarChar, 4);
            cmd.Parameters.Add("@Address1", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@City", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@State", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Zip", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Country", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Fax", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@EmergencyContact", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@EmergencyPhone", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FlightPrefix", SqlDbType.NVarChar, 5);
            cmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@IATAGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@OnHold", SqlDbType.Bit);
            cmd.Parameters.Add("@PickupFlights", SqlDbType.Bit);
            cmd.Parameters.Add("@StockTrack", SqlDbType.Bit);
            cmd.Parameters.Add("@SufferLossGain", SqlDbType.Bit);
            cmd.Parameters.Add("@LowStockWarning", SqlDbType.Float);
            cmd.Parameters.Add("@LockedOut", SqlDbType.Bit);
            cmd.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
            cmd.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@ShipperTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@CustomerBillToTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@CustomerShipToTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CreditOK", SqlDbType.Bit);
            cmd.Parameters.Add("@AdditiveAccounting", SqlDbType.Bit);
            cmd.Parameters.Add("@PurchaseOrderRequired", SqlDbType.Bit);
            cmd.Parameters.Add("@ReceivableAccount", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@RefinerCode", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@EPANumber", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID2", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID3", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID4", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID5", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@StateID", SqlDbType.NVarChar, 20);

            cmd.Parameters.Add("@TaxNumber", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FlushPermitted", SqlDbType.Bit);
            cmd.Parameters.Add("@PumpOffPermitted", SqlDbType.Bit);
            cmd.Parameters.Add("@DeliveryToTerminalPermitted", SqlDbType.Bit);
            cmd.Parameters.Add("@LicenseNumber", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@LicenseExpiration", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@InsuranceCompany", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@InsurancePolicy", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@LiabilityAmount", SqlDbType.Money);
            cmd.Parameters.Add("@HazardousMaterialExclusion", SqlDbType.Bit);
            cmd.Parameters.Add("@InsuranceExpiration", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@AllowDriverEntry", SqlDbType.Bit);
            cmd.Parameters.Add("@PINRequired", SqlDbType.Bit);
            cmd.Parameters.Add("@MaximumVehicleWeight", SqlDbType.Float);
            cmd.Parameters.Add("@WeightUnits", SqlDbType.SmallInt);
            cmd.Parameters.Add("@AccountNumber", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SCACCode", SqlDbType.NVarChar, 4);
            cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 2000);
            cmd.Parameters.Add("@DisableOwnerAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@DisableShipperAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@DisableBillToAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@DisableShipToAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@LoadRackDisplayText", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1Name", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1Address1", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1Address2", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1City", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@Contact1State", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1Zip", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Contact1Country", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1PhoneOffice", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1PhoneMobile", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1Fax", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1EmailAddress", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2Name", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2Address1", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2Address2", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2City", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@Contact2State", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2Zip", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Contact2Country", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2PhoneOffice", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2PhoneMobile", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2Fax", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2EmailAddress", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UserData0", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData1", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData2", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData3", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData4", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData5", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData6", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData7", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CreatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@CompanyGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@MasterRecordGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ScullyRequired", SqlDbType.Bit);
            cmd.Parameters.Add("@ConsortiumTypeIndex", SqlDbType.Int);
            cmd.Parameters.Add("@CompanyIataCode", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@CompanyIcaoCode", SqlDbType.NVarChar, 50);

            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
            cmd.Parameters["@ID"].Value = this._ID;
            cmd.Parameters["@Code"].Value = this._Code;
            cmd.Parameters["@Name"].Value = this._Name;
            cmd.Parameters["@ShortName"].Value = this._ShortName;
            cmd.Parameters["@Address1"].Value = this._Address1;
            cmd.Parameters["@Address2"].Value = this._Address2;
            cmd.Parameters["@City"].Value = this._City;
            cmd.Parameters["@State"].Value = this._State;
            cmd.Parameters["@Zip"].Value = this._Zip;
            cmd.Parameters["@Country"].Value = this._Country;
            cmd.Parameters["@Phone"].Value = this._Phone;
            cmd.Parameters["@Fax"].Value = this._Fax;
            cmd.Parameters["@EmergencyContact"].Value = this._EmergencyContact;
            cmd.Parameters["@EmergencyPhone"].Value = this._EmergencyPhone;
            cmd.Parameters["@FlightPrefix"].Value = this._FlightPrefix;
            cmd.Parameters["@EffectiveDate"].Value = this._EffectiveDate.Value;
            cmd.Parameters["@ExpirationDate"].Value = this._ExpirationDate.Value;
            cmd.Parameters["@CompanyIataCode"].Value = this.companyIataCode;
            cmd.Parameters["@CompanyIcaoCode"].Value = this.companyIcaoCode;

            if (this.IATAGuid != Guid.Empty)
            {
                cmd.Parameters["@IATAGuid"].Value = this.IATAGuid;
            }
            else
            {
                cmd.Parameters["@IATAGuid"].Value = DBNull.Value;
            }

            if (this.OnHold)
            {
                cmd.Parameters["@OnHold"].Value = 1;
            }
            else
            {
                cmd.Parameters["@OnHold"].Value = 0;
            }

            if (this.PickupFlights)
            {
                cmd.Parameters["@PickupFlights"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PickupFlights"].Value = 0;
            }

            if (this.StockTrack)
            {
                cmd.Parameters["@StockTrack"].Value = 1;
            }
            else
            {
                cmd.Parameters["@StockTrack"].Value = 0;
            }

            if (this.SufferLossGain)
            {
                cmd.Parameters["@SufferLossGain"].Value = 1;
            }
            else
            {
                cmd.Parameters["@SufferLossGain"].Value = 0;
            }

            cmd.Parameters["@LowStockWarning"].Value = this.LowStockWarning;

            if (this.LockedOut)
            {
                cmd.Parameters["@LockedOut"].Value = 1;
            }
            else
            {
                cmd.Parameters["@LockedOut"].Value = 0;
            }

            cmd.Parameters["@LockedOutReason"].Value = this._LockedOutReason;
            cmd.Parameters["@LockedOutDate"].Value = this._LockedOutDate.Value;

            if (this.ShipperTypeApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters["@ShipperTypeApplicationStringGuid"].Value = this.ShipperTypeApplicationStringGuid;
            }
            else
            {
                cmd.Parameters["@ShipperTypeApplicationStringGuid"].Value = DBNull.Value;
            }

            if (this.CustomerBillToTypeApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters["@CustomerBillToTypeApplicationStringGuid"].Value = this.CustomerBillToTypeApplicationStringGuid;
            }
            else
            {
                cmd.Parameters["@CustomerBillToTypeApplicationStringGuid"].Value = DBNull.Value;
            }

            if (this.CustomerShipToTypeApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters["@CustomerShipToTypeApplicationStringGuid"].Value = this.CustomerShipToTypeApplicationStringGuid;
            }
            else
            {
                cmd.Parameters["@CustomerShipToTypeApplicationStringGuid"].Value = DBNull.Value;
            }

            cmd.Parameters["@LastActivityDate"].Value = this._LastActivityDate.Value;

            if (this.CreditOK)
            {
                cmd.Parameters["@CreditOK"].Value = 1;
            }
            else
            {
                cmd.Parameters["@CreditOK"].Value = 0;
            }

            if (this.AdditiveAccounting)
            {
                cmd.Parameters["@AdditiveAccounting"].Value = 1;
            }
            else
            {
                cmd.Parameters["@AdditiveAccounting"].Value = 0;
            }

            if (this.PurchaseOrderRequired)
            {
                cmd.Parameters["@PurchaseOrderRequired"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PurchaseOrderRequired"].Value = 0;
            }

            cmd.Parameters["@ReceivableAccount"].Value = this._ReceivableAccount;
            cmd.Parameters["@RefinerCode"].Value = this._RefinerCode;
            cmd.Parameters["@EPANumber"].Value = this._EPANumber;
            cmd.Parameters["@FederalID"].Value = this._FederalID;
            cmd.Parameters["@FederalID2"].Value = this._FederalID2;
            cmd.Parameters["@FederalID3"].Value = this._FederalID3;
            cmd.Parameters["@FederalID4"].Value = this._FederalID4;
            cmd.Parameters["@FederalID5"].Value = this._FederalID5;
            cmd.Parameters["@StateID"].Value = this._StateID;
            cmd.Parameters["@TaxNumber"].Value = this._TaxNumber;

            if (this.FlushPermitted)
            {
                cmd.Parameters["@FlushPermitted"].Value = 1;
            }
            else
            {
                cmd.Parameters["@FlushPermitted"].Value = 0;
            }

            if (this.PumpOffPermitted)
            {
                cmd.Parameters["@PumpOffPermitted"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PumpOffPermitted"].Value = 0;
            }

            if (this.DeliveryToTerminalPermitted)
            {
                cmd.Parameters["@DeliveryToTerminalPermitted"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DeliveryToTerminalPermitted"].Value = 0;
            }

            cmd.Parameters["@LicenseNumber"].Value = this._LicenseNumber;
            cmd.Parameters["@LicenseExpiration"].Value = this._LicenseExpiration.Value;
            cmd.Parameters["@InsuranceCompany"].Value = this._InsuranceCompany;
            cmd.Parameters["@InsurancePolicy"].Value = this._InsurancePolicy;
            cmd.Parameters["@LiabilityAmount"].Value = this._LiabilityAmount.Value;

            if (this.HazardousMaterialExclusion)
            {
                cmd.Parameters["@HazardousMaterialExclusion"].Value = 1;
            }
            else
            {
                cmd.Parameters["@HazardousMaterialExclusion"].Value = 0;
            }

            cmd.Parameters["@InsuranceExpiration"].Value = this._InsuranceExpiration.Value;

            if (this.AllowDriverEntry)
            {
                cmd.Parameters["@AllowDriverEntry"].Value = 1;
            }
            else
            {
                cmd.Parameters["@AllowDriverEntry"].Value = 0;
            }

            if (this.PINRequired)
            {
                cmd.Parameters["@PINRequired"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PINRequired"].Value = 0;
            }

            if (this.ScullyRequired)
            {
                cmd.Parameters["@ScullyRequired"].Value = 1;
            }
            else
            {
                cmd.Parameters["@ScullyRequired"].Value = 0;
            }

            if (this.ConsortiumType.HasValue)
            {
                cmd.Parameters["@ConsortiumTypeIndex"].Value = (int?)this.ConsortiumType;
            }
            else
            {
                cmd.Parameters["@ConsortiumTypeIndex"].Value = DBNull.Value;
            }

            cmd.Parameters["@MaximumVehicleWeight"].Value = this._MaximumVehicleWeight;
            cmd.Parameters["@WeightUnits"].Value = (int)this._WeightUnits;
            cmd.Parameters["@AccountNumber"].Value = this._AccountNumber;
            cmd.Parameters["@SCACCode"].Value = this._SCACCode;

            if (this.Note != string.Empty)
            {
                cmd.Parameters["@Note"].Value = this.Note;
            }
            else
            {
                cmd.Parameters["@Note"].Value = DBNull.Value;
            }

            if (this.DisableOwnerAllocationsCheck)
            {
                cmd.Parameters["@DisableOwnerAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableOwnerAllocationsCheck"].Value = 0;
            }

            if (this.DisableShipperAllocationsCheck)
            {
                cmd.Parameters["@DisableShipperAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableShipperAllocationsCheck"].Value = 0;
            }

            if (this.DisableBillToAllocationsCheck)
            {
                cmd.Parameters["@DisableBillToAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableBillToAllocationsCheck"].Value = 0;
            }

            if (this.DisableShipToAllocationsCheck)
            {
                cmd.Parameters["@DisableShipToAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableShipToAllocationsCheck"].Value = 0;
            }

            cmd.Parameters["@LoadRackDisplayText"].Value = this._LoadRackDisplayText;
            cmd.Parameters["@Contact1Name"].Value = string.IsNullOrEmpty(this._Contact1Name)
                ? (object)DBNull.Value
                : this._Contact1Name;
            cmd.Parameters["@Contact1Address1"].Value = string.IsNullOrEmpty(this._Contact1Address1)
                ? (object)DBNull.Value
                : this._Contact1Address1;
            cmd.Parameters["@Contact1Address2"].Value = string.IsNullOrEmpty(this._Contact1Address2)
                ? (object)DBNull.Value
                : this._Contact1Address2;
            cmd.Parameters["@Contact1City"].Value = string.IsNullOrEmpty(this._Contact1City)
                ? (object)DBNull.Value
                : this._Contact1City;
            cmd.Parameters["@Contact1State"].Value = string.IsNullOrEmpty(this._Contact1State)
                ? (object)DBNull.Value
                : this._Contact1State;
            cmd.Parameters["@Contact1Zip"].Value = string.IsNullOrEmpty(this._Contact1Zip)
                ? (object)DBNull.Value
                : this._Contact1Zip;
            cmd.Parameters["@Contact1Country"].Value = string.IsNullOrEmpty(this._Contact1Country)
                ? (object)DBNull.Value
                : this._Contact1Country;
            cmd.Parameters["@Contact1PhoneOffice"].Value = string.IsNullOrEmpty(this._Contact1PhoneOffice)
                ? (object)DBNull.Value
                : this._Contact1PhoneOffice;
            cmd.Parameters["@Contact1PhoneMobile"].Value = string.IsNullOrEmpty(this._Contact1PhoneMobile)
                ? (object)DBNull.Value
                : this._Contact1PhoneMobile;
            cmd.Parameters["@Contact1Fax"].Value = string.IsNullOrEmpty(this._Contact1Fax)
                ? (object)DBNull.Value
                : this._Contact1Fax;
            cmd.Parameters["@Contact1EmailAddress"].Value = string.IsNullOrEmpty(this._Contact1EmailAddress)
                ? (object)DBNull.Value
                : this._Contact1EmailAddress;
            cmd.Parameters["@Contact2Name"].Value = string.IsNullOrEmpty(this._Contact2Name)
                ? (object)DBNull.Value
                : this._Contact2Name;
            cmd.Parameters["@Contact2Address1"].Value = string.IsNullOrEmpty(this._Contact2Address1)
                ? (object)DBNull.Value
                : this._Contact2Address1;
            cmd.Parameters["@Contact2Address2"].Value = string.IsNullOrEmpty(this._Contact2Address2)
                ? (object)DBNull.Value
                : this._Contact2Address2;
            cmd.Parameters["@Contact2City"].Value = string.IsNullOrEmpty(this._Contact2City)
                ? (object)DBNull.Value
                : this._Contact2City;
            cmd.Parameters["@Contact2State"].Value = string.IsNullOrEmpty(this._Contact2State)
                ? (object)DBNull.Value
                : this._Contact2State;
            cmd.Parameters["@Contact2Zip"].Value = string.IsNullOrEmpty(this._Contact2Zip)
                ? (object)DBNull.Value
                : this._Contact2Zip;
            cmd.Parameters["@Contact2Country"].Value = string.IsNullOrEmpty(this._Contact2Country)
                ? (object)DBNull.Value
                : this._Contact2Country;
            cmd.Parameters["@Contact2PhoneOffice"].Value = string.IsNullOrEmpty(this._Contact2PhoneOffice)
                ? (object)DBNull.Value
                : this._Contact2PhoneOffice;
            cmd.Parameters["@Contact2PhoneMobile"].Value = string.IsNullOrEmpty(this._Contact2PhoneMobile)
                ? (object)DBNull.Value
                : this._Contact2PhoneMobile;
            cmd.Parameters["@Contact2Fax"].Value = string.IsNullOrEmpty(this._Contact2Fax)
                ? (object)DBNull.Value
                : this._Contact2Fax;
            cmd.Parameters["@Contact2EmailAddress"].Value = string.IsNullOrEmpty(this._Contact2EmailAddress)
                ? (object)DBNull.Value
                : this._Contact2EmailAddress;

            cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;

            cmd.Parameters["@UserData0"].Value = this.UserData[0];
            cmd.Parameters["@UserData1"].Value = this.UserData[1];
            cmd.Parameters["@UserData2"].Value = this.UserData[2];
            cmd.Parameters["@UserData3"].Value = this.UserData[3];
            cmd.Parameters["@UserData4"].Value = this.UserData[4];
            cmd.Parameters["@UserData5"].Value = this.UserData[5];
            cmd.Parameters["@UserData6"].Value = this.UserData[6];
            cmd.Parameters["@UserData7"].Value = this.UserData[7];
            cmd.Parameters["@CreatedDate"].Value = this.CreatedDate;
            cmd.Parameters["@CreatedBy"].Value = this._CreatedBy;
            cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
            cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
            cmd.Parameters["@CompanyGuid"].Value = this._IdentityGuid;
            //This query can only be used to create master record versions.
            this.MasterRecordGuid = this.IdentityGuid;
            cmd.Parameters["@MasterRecordGuid"].Value = this.MasterRecordGuid;
        }

        public bool InsuranceWarning(TimeSpan warningPeriod)
        {
            if (this._InsuranceExpiration.Value - DateTimeOffset.Now < warningPeriod)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LicenseWarning(TimeSpan warningPeriod)
        {
            if (this._LicenseExpiration.Value - DateTimeOffset.Now < warningPeriod)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override void Load(object o)
        {
            this.Reset();

            DataTable table = null;
            DataRow row = null;

            var dataRow = o as DataRow;
            if (dataRow != null)
            {
                row = dataRow;
            }

            var set = o as DataSet;
            if (set != null)
            {

                table = set.Tables[0];
                if (table.Rows.Count == 0)
                {
                    return;
                }

                row = table.Rows[0];
            }

            if (row != null)
            {
                this._IdentityGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
                this._MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty);
                this._SiteGuid = DataObject.getValue<Guid>(row["SiteGuid"], Guid.Empty);
                this._ID = DataObject.getValue<string>(row["ID"], "");
                this._Code = DataObject.getValue<string>(row["Code"], "");
                this._Name = DataObject.getValue<string>(row["Name"], "");
                this._ShortName = DataObject.getValue<string>(row["ShortName"], "");
                this._Address1 = DataObject.getValue<string>(row["Address1"], "");
                this._Address2 = DataObject.getValue<string>(row["Address2"], "");
                this._City = DataObject.getValue<string>(row["City"], "");
                this._State = DataObject.getValue<string>(row["State"], "");
                this._Zip = DataObject.getValue<string>(row["Zip"], "");
                this._Country = DataObject.getValue<string>(row["Country"], "");
                this._Phone = DataObject.getValue<string>(row["Phone"], "");
                this._Fax = DataObject.getValue<string>(row["FAX"], "");
                this._EmergencyContact = DataObject.getValue<string>(row["EmergencyContact"], "");
                this._EmergencyPhone = DataObject.getValue<string>(row["EmergencyPhone"], "");
                this._FlightPrefix = DataObject.getValue<string>(row["FlightPrefix"], "");
                this._EffectiveDate.Value = DataObject.getValue<DateTimeOffset>(
                    row["EffectiveDate"],
                    TimeConverter.Today(this._EffectiveDate.StandardName));
                this._ExpirationDate.Value = DataObject.getValue<DateTimeOffset>(
                    row["ExpirationDate"],
                    TimeConverter.Today(this._ExpirationDate.StandardName));
                this.IATAGuid = DataObject.getValue<Guid>(row["IATAGuid"], Guid.Empty);
                this._OnHold = DataObject.getValue<bool>(row["OnHold"], false);
                this._PickupFlights = DataObject.getValue<bool>(row["PickupFLights"], false);
                this._StockTrack = DataObject.getValue<bool>(row["StockTrack"], false);
                this._SufferLossGain = DataObject.getValue<bool>(row["SufferLossGain"], false);
                this._LowStockWarning = DataObject.getValue<double>(row["LowStockWarning"], 0.0);
                this._LockedOut = DataObject.getValue<bool>(row["LockedOut"], false);
                this._LockedOutReason = DataObject.getValue<string>(row["LockedOutReason"], "");
                this._LockedOutDate.Value = DataObject.getValue<DateTimeOffset>(
                    row["LockedOutDate"],
                    TimeConverter.Today(this._LockedOutDate.StandardName));
                this.ShipperTypeApplicationStringGuid = DataObject.getValue<Guid>(
                    row["ShipperTypeApplicationStringGuid"],
                    Guid.Empty);
                this.CustomerBillToTypeApplicationStringGuid =
                    DataObject.getValue<Guid>(row["CustomerBillToTypeApplicationStringGuid"], Guid.Empty);
                this.CustomerShipToTypeApplicationStringGuid =
                    DataObject.getValue<Guid>(row["CustomerShipToTypeApplicationStringGuid"], Guid.Empty);
                this._ReceivableAccount = DataObject.getValue<string>(row["ReceivableAccount"], "");
                this._RefinerCode = DataObject.getValue<string>(row["RefinerCode"], "");
                this._LastActivityDate.Value = DataObject.getValue<DateTimeOffset>(row["LastActivityDate"], DateTimeOffset.Now);
                this._CreditOK = DataObject.getValue<bool>(row["CreditOK"], true);
                this._AdditiveAccounting = DataObject.getValue<bool>(row["AdditiveAccounting"], false);
                this._PurchaseOrderRequired = DataObject.getValue<bool>(row["PurchaseOrderRequired"], false);
                this._EPANumber = DataObject.getValue<string>(row["EPANumber"], "");
                this._FederalID = DataObject.getValue<string>(row["FederalID"], "");
                this._FederalID2 = DataObject.getValue<string>(row["FederalID2"], "");
                this._FederalID3 = DataObject.getValue<string>(row["FederalID3"], "");
                this._FederalID4 = DataObject.getValue<string>(row["FederalID4"], "");
                this._FederalID5 = DataObject.getValue<string>(row["FederalID5"], "");
                this._StateID = DataObject.getValue<string>(row["StateID"], "");
                this._TaxNumber = DataObject.getValue<string>(row["TaxNumber"], "");
                this._FlushPermitted = DataObject.getValue<bool>(row["FlushPermitted"], false);
                this._PumpOffPermitted = DataObject.getValue<bool>(row["PumpOffPermitted"], false);
                this._DeliveryToTerminalPermitted = DataObject.getValue<bool>(row["DeliveryToTerminalPermitted"], false);
                this._LicenseNumber = DataObject.getValue<string>(row["LicenseNumber"], "");
                this._LicenseExpiration.Value = DataObject.getValue<DateTimeOffset>(
                    row["LicenseExpiration"],
                    TimeConverter.Today(this._LicenseExpiration.StandardName));
                this._InsuranceCompany = DataObject.getValue<string>(row["InsuranceCompany"], "");
                this._InsurancePolicy = DataObject.getValue<string>(row["InsurancePolicy"], "");
                this._LiabilityAmount.Value = DataObject.getValue<Decimal>(row["LiabilityAmount"], 0);
                this._HazardousMaterialExclusion = DataObject.getValue<bool>(row["HazardousMaterialExclusion"], false);
                this._InsuranceExpiration.Value = DataObject.getValue<DateTimeOffset>(
                    row["InsuranceExpiration"],
                    TimeConverter.Today(this._InsuranceExpiration.StandardName));
                this._AllowDriverEntry = DataObject.getValue<bool>(row["AllowDriverEntry"], false);
                this._PINRequired = DataObject.getValue<bool>(row["PINRequired"], true);
                this._ScullyRequired = DataObject.getValue<bool>(row["ScullyRequired"], false);
                this._ConsortiumType = (ConsortiumTypes?)DataObject.getValue<int?>(row["ConsortiumTypeIndex"], null);
                this._MaximumVehicleWeight = DataObject.getValue<double>(row["MaximumVehicleWeight"], 0.0);
                this._WeightUnits =
                    (EngineeringUnit)DataObject.getValue<short>(row["WeightUnits"], (short)EngineeringUnit.FmmMTon);
                this._AccountNumber = DataObject.getValue<string>(row["AccountNumber"], "");
                this._SCACCode = DataObject.getValue<string>(row["SCACCode"], "");
                this.Note = DataObject.getValue<string>(row["Note"], string.Empty);
                this._DisableOwnerAllocationsCheck = DataObject.getValue<bool>(row["DisableOwnerAllocationsCheck"], false);
                this._DisableShipperAllocationsCheck = DataObject.getValue<bool>(row["DisableShipperAllocationsCheck"], false);
                this._DisableBillToAllocationsCheck = DataObject.getValue<bool>(row["DisableBillToAllocationsCheck"], false);
                this._DisableShipToAllocationsCheck = DataObject.getValue<bool>(row["DisableShipToAllocationsCheck"], false);
                this._LoadRackDisplayText = DataObject.getValue<string>(row["LoadRackDisplayText"], "");
                this._Contact1Name = DataObject.getValue<string>(row["Contact1Name"], string.Empty);
                this._Contact1Address1 = DataObject.getValue<string>(row["Contact1Address1"], string.Empty);
                this._Contact1Address2 = DataObject.getValue<string>(row["Contact1Address2"], string.Empty);
                this._Contact1City = DataObject.getValue<string>(row["Contact1City"], string.Empty);
                this._Contact1State = DataObject.getValue<string>(row["Contact1State"], string.Empty);
                this._Contact1Zip = DataObject.getValue<string>(row["Contact1Zip"], string.Empty);
                this._Contact1Country = DataObject.getValue<string>(row["Contact1Country"], string.Empty);
                this._Contact1PhoneOffice = DataObject.getValue<string>(row["Contact1PhoneOffice"], string.Empty);
                this._Contact1PhoneMobile = DataObject.getValue<string>(row["Contact1PhoneMobile"], string.Empty);
                this._Contact1Fax = DataObject.getValue<string>(row["Contact1Fax"], string.Empty);
                this._Contact1EmailAddress = DataObject.getValue<string>(row["Contact1EmailAddress"], string.Empty);
                this._Contact2Name = DataObject.getValue<string>(row["Contact2Name"], string.Empty);
                this._Contact2Address1 = DataObject.getValue<string>(row["Contact2Address1"], string.Empty);
                this._Contact2Address2 = DataObject.getValue<string>(row["Contact2Address2"], string.Empty);
                this._Contact2City = DataObject.getValue<string>(row["Contact2City"], string.Empty);
                this._Contact2State = DataObject.getValue<string>(row["Contact2State"], string.Empty);
                this._Contact2Zip = DataObject.getValue<string>(row["Contact2Zip"], string.Empty);
                this._Contact2Country = DataObject.getValue<string>(row["Contact2Country"], string.Empty);
                this._Contact2PhoneOffice = DataObject.getValue<string>(row["Contact2PhoneOffice"], string.Empty);
                this._Contact2PhoneMobile = DataObject.getValue<string>(row["Contact2PhoneMobile"], string.Empty);
                this._Contact2Fax = DataObject.getValue<string>(row["Contact2Fax"], string.Empty);
                this._Contact2EmailAddress = DataObject.getValue<string>(row["Contact2EmailAddress"], string.Empty);
                this.HiddenDate = DataObject.getValue<DateTimeOffset?>(row["HiddenDate"], null);
                this.UserData[0] = DataObject.getValue<string>(row["UserData1"], "");
                this.UserData[1] = DataObject.getValue<string>(row["UserData2"], "");
                this.UserData[2] = DataObject.getValue<string>(row["UserData3"], "");
                this.UserData[3] = DataObject.getValue<string>(row["UserData4"], "");
                this.UserData[4] = DataObject.getValue<string>(row["UserData5"], "");
                this.UserData[5] = DataObject.getValue<string>(row["UserData6"], "");
                this.UserData[6] = DataObject.getValue<string>(row["UserData7"], "");
                this.UserData[7] = DataObject.getValue<string>(row["UserData8"], "");
                this._CreatedDate = DataObject.getValue<DateTimeOffset>(row["CreatedDate"], DateTimeOffset.Now);
                this._CreatedBy = DataObject.getValue<string>(row["CreatedBy"], ADMIN);
                this._UpdatedDate = DataObject.getValue<DateTimeOffset>(row["UpdatedDate"], this._CreatedDate);
                this._UpdatedBy = DataObject.getValue<string>(row["UpdatedBy"], ADMIN);
                this._IATAID = DataObject.getValue<string>(row["IATAID"], "{None}");
                this._ShipperTypeID = DataObject.getValue<string>(row["ShipperTypeID"], "{None}");
                this._CustomerBillToTypeID = DataObject.getValue<string>(row["CustomerBillToTypeID"], "{None}");
                this._CustomerShipToTypeID = DataObject.getValue<string>(row["CustomerShipToTypeID"], "{None}");
                this._SCACCode = DataObject.getValue<string>(row["SCACCode"], "");
                this.companyIataCode = DataObject.getValue<string>(row["CompanyIATACode"], string.Empty);
                this.companyIcaoCode = DataObject.getValue<string>(row["CompanyICAOCode"], string.Empty);

                if (table != null)
                {
                    if (table.Columns.IndexOf("ASSIGNEDTOSITEGUID") >= 0)
                    {
                        this.AssignedToSiteGuid = DataObject.getValue<Guid>(row["ASSIGNEDTOSITEGUID"], Guid.Empty);
                    }
                    if (table.Columns.IndexOf("ASSIGNEDFROMSITEGUID") >= 0)
                    {
                        this.AssignedFromSiteGuid = DataObject.getValue<Guid>(row["ASSIGNEDFROMSITEGUID"], Guid.Empty);
                    }
                    if (table.Columns.IndexOf("ASSIGNEDFROMSITEID") >= 0)
                    {
                        this.AssignedFromSiteId = DataObject.getValue<string>(row["ASSIGNEDFROMSITEID"], "");
                    }
                }
            }
            else
            {
                base.Load(o);

                var companyNode = o as XmlNode;
                if (companyNode != null)
                {
                    if (companyNode.Attributes?["Note"] != null)
                    {
                        this.Note = companyNode.Attributes["Note"].Value;
                    }

                    foreach (XmlNode node in companyNode)
                    {
                        if (node.Name == "Roles")
                        {
                            foreach (XmlNode roleNode in node)
                            {
                                var role = new CompanyRoleMapClass();
                                role.Load(roleNode);
                                this.RoleCollection.Add(role);
                            }
                        }

                        else if (node.Name == "AuthorizedProducts")
                        {
                            foreach (XmlNode productNode in node)
                            {
                                var authorizedProduct = new ProductMapClass();
                                authorizedProduct.Load(productNode);
                                this.AuthorizedProductCollection.Add(authorizedProduct);
                            }
                        }

                        else if (node.Name == "SupplierAuthorizedProducts")
                        {
                            foreach (XmlNode productNode in node)
                            {
                                var authorizedProduct = new ProductMapClass();
                                authorizedProduct.Load(productNode);
                                this.SupplierAuthorizedProductCollection.Add(authorizedProduct);
                            }
                        }

                        // added (IGO 02-Sep-2008)
                        else if (node.Name == "UnavailableInventories")
                        {
                            foreach (XmlNode productNode in node)
                            {
                                var unavailableInventory = new ProductMapClass();
                                unavailableInventory.Load(productNode);
                                this.UnavailableInventoryCollection.Add(unavailableInventory);
                            }
                        }

                        else if (node.Name == "AuthorizedCarriers")
                        {
                            foreach (XmlNode carrierNode in node)
                            {
                                var authorizedCarrier = CompanyMapClass.CreateCompanyMap(carrierNode);
                                this.AuthorizedCarrierCollection.Add(authorizedCarrier);
                            }
                        }

                        else if (node.Name == "CertificatesAndPermits")
                        {
                            int sequence = 0;
                            foreach (XmlNode certificateAndPermitNode in node)
                            {
                                var certificateAndPermit = new QualificationMapClass();
                                certificateAndPermit.Load(certificateAndPermitNode);
                                certificateAndPermit.Sequence = sequence++;
                                this.CertificateAndPermitCollection.Add(certificateAndPermit);
                            }
                        }

                        else if (node.Name == "AccessSchedule")
                        {
                            this.AccessScheduleCollection.Clear();
                            foreach (XmlNode scheduleEntry in node)
                            {
                                var schedule = new ScheduleClass(this.DateTimeFormatInfo);
                                schedule.Load(scheduleEntry);
                                this.AccessScheduleCollection.Add(schedule);
                            }
                        }

                        else if (node.Name == "AuthorizedCustomers")
                        {
                            foreach (XmlNode customerNode in node)
                            {
                                var authorizedCustomer = CompanyMapClass.CreateCompanyMap(customerNode);
                                this.CarrierCustomerShipToCollection.Add(authorizedCustomer);
                            }
                        }

                        else if (node.Name == "Drivers")
                        {
                            foreach (XmlNode driverNode in node)
                            {
                                var assignedPerson = CompanyMapClass.CreateCompanyMap(driverNode);
                                this.AssignedPersonnelCollection.Add(assignedPerson);
                            }
                        }

                        else if (node.Name == "Equipment")
                        {
                            foreach (XmlNode equipmentNode in node)
                            {
                                var equipment = new EquipmentClass();
                                equipment.Load(equipmentNode);
                                this.EquipmentCollection.Add(equipment);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     This method will load the company object with the information needed by
        ///     the company select form.
        /// </summary>
        /// <param name="obj"></param>
        public void LoadCompanySelectRole(object obj)
        {
            DataRow row = null;

            var dataRow = obj as DataRow;
            if (dataRow != null)
            {
                row = dataRow;
            }

            var set = obj as DataSet;
            if (set != null)
            {
                DataTable table = set.Tables[0];

                if (table.Rows.Count == 0)
                {
                    return;
                }

                row = table.Rows[0];
            }

            if (row != null)
            {
                this._ID = DataObject.getValue<string>(row["ID"], "");
                this._Code = DataObject.getValue<string>(row["Code"], "");
                this._IdentityGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
                this.MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty);
                this._Name = DataObject.getValue<string>(row["Name"], "");
                this._Address1 = DataObject.getValue<string>(row["Address1"], "");
                this._Address2 = DataObject.getValue<string>(row["Address2"], "");
                this._City = DataObject.getValue<string>(row["City"], "");
                this._State = DataObject.getValue<string>(row["State"], "");

                if (row.Table.Columns.Contains("CustomerBillToTypeID"))
                {
                    this.CustomerBillToTypeID = DataObject.getValue<string>(row["CustomerBillToTypeID"], "{None}");
                    if (row.Table.Columns.Contains("CustomerShipToTypeID"))
                    {
                        this.CustomerShipToTypeID = DataObject.getValue<string>(row["CustomerShipToTypeID"], "{None}");
                    }
                }
            }
        }

        public void LoadIDCodeTypesIdentityGuid(object o)
        {
            DataRow row = null;

            var dataRow = o as DataRow;
            if (dataRow != null)
            {
                row = dataRow;
            }

            var set = o as DataSet;
            if (set != null)
            {
                DataTable table = set.Tables[0];
                if (table.Rows.Count == 0)
                {
                    return;
                }

                row = table.Rows[0];
            }

            if (row != null)
            {
                this._ID = DataObject.getValue<string>(row["ID"], "");
                this._Code = DataObject.getValue<string>(row["Code"], "");
                this._IdentityGuid = DataObject.getValue<Guid>(row["CompanyGuid"], Guid.Empty);
                this.ShipperTypeApplicationStringGuid = DataObject.getValue<Guid>(
                    row["ShipperTypeApplicationStringGuid"],
                    Guid.Empty);
                this.CustomerBillToTypeApplicationStringGuid =
                    DataObject.getValue<Guid>(row["CustomerBillToTypeApplicationStringGuid"], Guid.Empty);
                this.CustomerShipToTypeApplicationStringGuid =
                    DataObject.getValue<Guid>(row["CustomerShipToTypeApplicationStringGuid"], Guid.Empty);
                this.MasterRecordGuid = DataObject.getValue<Guid>(row["_MasterRecordGuid"], Guid.Empty);
                this.HiddenDate = DataObject.getValue<DateTimeOffset?>(row["HiddenDate"], null);
            }
        }

        public void PurgeSQL(SqlCommand cmd)
        {
            cmd.CommandText = "DELETE FROM tblCompanies WHERE CompanyGuid = @IdentityGuid";

            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);

            cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
        }

        public QueryWriterFieldCollection QueryAliasFields(SecurityClass security, QueryWriterFieldCollection fields)
        {
            UserDataFieldCollectionClass userDataFieldCollection =
                FMChannelHelper.MakeCall<IUserDataFields, UserDataFieldCollectionClass>(
                    x => x.EnumerateByEntityType(security, ENTITY_TYPE.COMPANY, Guid.Empty, false, false));

            var newCollection = new QueryWriterFieldCollection(fields);

            IEnumerable<QueryWriterField> userFields = from f in newCollection
                                                       where f.DisplayName.StartsWith("User Data")
                                                       select f;

            foreach (QueryWriterField userField in userFields)
            {
                if (this.UpdateFieldName(userField, userDataFieldCollection) == false)
                {
                    userField.DisplayName = string.Empty;
                }
            }

            // Remove any blanked out fields.  Wish we could do it above but
            // it disrupts the enumeration.
            for (int index = newCollection.Count - 1; index >= 0; --index)
            {
                if (string.IsNullOrEmpty(newCollection[index].DisplayName))
                {
                    newCollection.RemoveAt(index);
                }
            }

            QueryClass.ApplyDataDictionary(security, newCollection);

            return newCollection;
        }

        public void QueryWriterSQL(SqlCommand cmd, SecurityClass security, string selectClauseParam)
        {
            cmd.CommandText = this.CachedCompanyrecordversionGuidTableOperation
                              + selectClauseParam + "," + "tblCompanies.ID AS 'tblCompanies.ID',"
                              + "tblCompanies.Name AS 'tblCompanies.Name'," + "tblCompanies.CompanyGuid AS EntityGuid,"
                              + "lookup.tblConsortiumType.[ConsortiumTypeName] AS ConsortiumType,"
                              + "tblIATA.IATAID AS 'tblIATA.IATAID',"
                              + "ShipperTypeApplicationString.ID AS 'ShipperTypeApplicationString.ID',"
                              + "CustomerBillToTypeApplicationString.ID AS 'CustomerBillToTypeApplicationString.ID',"
                              + "CustomerShipToTypeApplicationString.ID AS 'CustomerShipToTypeApplicationString.ID'"                              
                              + " FROM tblCompanies" + " LEFT JOIN tblIATA ON tblCompanies.IATAGuid = tblIATA.IATAGuid"
                              + " LEFT JOIN tblApplicationString ShipperTypeApplicationString ON tblCompanies.ShipperTypeApplicationStringGuid = ShipperTypeApplicationString.ApplicationStringGuid"
                              + " LEFT JOIN tblApplicationString CustomerBillToTypeApplicationString ON tblCompanies.CustomerBillToTypeApplicationStringGuid = CustomerBillToTypeApplicationString.ApplicationStringGuid"
                              + " LEFT JOIN tblApplicationString CustomerShipToTypeApplicationString ON tblCompanies.CustomerShipToTypeApplicationStringGuid = CustomerShipToTypeApplicationString.ApplicationStringGuid"
                              + " LEFT JOIN lookup.tblConsortiumType ON lookup.tblConsortiumType.ConsortiumTypeIndex = tblCompanies.ConsortiumTypeIndex"
                              + GroupCompaniesSubQueryClause + " WHERE "
                              + this.AppendSiteWhereClause(security, "tblCompanies", "CompanyGuid")
                              + " AND tblCompanies.CompanyGuid = tblAuthorizedCompanies.AuthorizedCompanyGuid";

            cmd.Parameters.Add("@UserGuid", SqlDbType.UniqueIdentifier).Value = security.UserGuid;
            cmd.Parameters.Add("@TargetSiteGuid", SqlDbType.UniqueIdentifier).Value = security.SiteGuid;
        }

        public AlarmAndEventLogClass EntityAddedEvent(string entity)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(AuditLogEntityAddedEventDescriptor)
            {
                AssociatedData = entity + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EntityModifiedEvent(string entity)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(AuditLogEntityModifiedEventDescriptor)
            {
                AssociatedData = entity + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EntityPurgedEvent(string entity)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(AuditLogEntityPurgedEventDescriptor)
            {
                AssociatedData = entity + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass DatabaseTraceViewedEvent(string viewerID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(AuditLogDatabaseTraceViewedEventDescriptor)
            {
                AssociatedData = "Database trace/audit logs viewed by : " + viewerID
            };
            return alarmAndEventLog;
        }

        public override void Reset()
        {
            base.Reset();
            this.Initialize();
        }

        public void SetSelectLimit(int aLimit)
        {
            if (aLimit > 0)
            {
                // remove the SELECT
                this.selectClause = this.selectClause.Remove(0, 15);
                this.selectCompanySelectRoleClause = this.selectCompanySelectRoleClause.Remove(0, 15);

                // add custom select
                this.selectClause = "SELECT DISTINCT TOP " + aLimit + " " + this.selectClause;
                this.selectCompanySelectRoleClause = "SELECT DISTINCT TOP " + aLimit + " " + this.selectCompanySelectRoleClause;
            }
        }

        /// <summary>
        ///     Override the SiteWhereClause to support Company RecordVersioning.
        /// </summary>
        /// <param name="security"></param>
        /// <param name="entityTable"></param>
        /// <param name="entityGuidColumn"></param>
        /// <returns></returns>
        public string AppendSiteWhereClause(SecurityClass security, string entityTable, string entityGuidColumn)
        {
            var sql = " (" + entityTable + "." + entityGuidColumn + " IN (SELECT " + entityGuidColumn
                         + " FROM " + CachedCompanyrecordversionTableName + "))";
            return sql;
        }

        public override void Store(object o)
        {
            var companyNode = o as XmlNode;
            if (companyNode != null)
            {
                base.Store(companyNode);

                if (this.Note != string.Empty)
                {
                    XmlAttribute attribute = companyNode.OwnerDocument?.CreateAttribute("Note");
                    if (attribute != null)
                    {
                        attribute.Value = this.Note;
                        companyNode.Attributes?.Append(attribute);
                    }
                }

                var companyRolesNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Roles", null);
                if (companyRolesNode != null)
                {
                    companyNode.AppendChild(companyRolesNode);
                    foreach (CompanyRoleMapClass role in this.RoleCollection)
                    {
                        var companyRoleNode =
                            companyRolesNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Role", null);
                        role.Store(companyRoleNode);
                        if (companyRoleNode != null)
                        {
                            companyRolesNode.AppendChild(companyRoleNode);
                        }
                    }
                }

                if (this.HasRole(COMPANY_ROLE.CUSTOMER_SHIPTO))
                {
                    var authorizedCarriersNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AuthorizedCarriers", null);
                    if (authorizedCarriersNode != null)
                    {
                        companyNode.AppendChild(authorizedCarriersNode);
                        foreach (CompanyMapClass carrier in this.AuthorizedCarrierCollection)
                        {
                            var authorizedCarrierNode = authorizedCarriersNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AuthorizedCarrier", null);
                            carrier.Store(authorizedCarrierNode);
                            if (authorizedCarrierNode != null)
                            {
                                authorizedCarriersNode.AppendChild(authorizedCarrierNode);
                            }
                        }
                    }

                    var authorizedProductsNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AuthorizedProducts", null);
                    if (authorizedProductsNode != null)
                    {
                        companyNode.AppendChild(authorizedProductsNode);
                        foreach (ProductMapClass product in this.AuthorizedProductCollection)
                        {
                            if (product.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
                            {
                                continue;
                            }

                            var authorizedProductNode = authorizedProductsNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AuthorizedProduct", null);
                            product.Store(authorizedProductNode);
                            if (authorizedProductNode != null)
                            {
                                authorizedProductsNode.AppendChild(authorizedProductNode);
                            }
                        }
                    }
                }

                if (this.HasRole(COMPANY_ROLE.SUPPLIER))
                {
                    var authorizedProductsNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "SupplierAuthorizedProducts", null);
                    if (authorizedProductsNode != null)
                    {
                        companyNode.AppendChild(authorizedProductsNode);
                        foreach (ProductMapClass product in this.SupplierAuthorizedProductCollection)
                        {
                            if (product.Type == PRODUCT_MAP_TYPE.PRODUCT_COMPANY_GROUP_MAP)
                            {
                                continue;
                            }

                            var authorizedProductNode = authorizedProductsNode.OwnerDocument?.CreateNode(XmlNodeType.Element,
                                                                                                        "SupplierAuthorizedProduct",
                                                                                                        null);
                            product.Store(authorizedProductNode);
                            if (authorizedProductNode != null)
                            {
                                authorizedProductsNode.AppendChild(authorizedProductNode);
                            }
                        }
                    }
                }

                // added (IGO 02-Sep-2008)
                if (this.HasRole(COMPANY_ROLE.OWNER))
                {
                    var unavailableInventoriesNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "UnavailableInventories", null);
                    if (unavailableInventoriesNode != null)
                    {
                        companyNode.AppendChild(unavailableInventoriesNode);
                        foreach (ProductMapClass product in this.UnavailableInventoryCollection)
                        {
                            var unavailableInventoryNode = unavailableInventoriesNode.OwnerDocument?.CreateNode(XmlNodeType.Element,
                                                                                                                "UnavailableInventory",
                                                                                                                null);
                            product.Store(unavailableInventoryNode);
                            if (unavailableInventoryNode != null)
                            {
                                unavailableInventoriesNode.AppendChild(unavailableInventoryNode);
                            }
                        }
                    }
                }

                var certificatesAndPermitsNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "CertificatesAndPermits", null);
                if (certificatesAndPermitsNode != null)
                {
                    companyNode.AppendChild(certificatesAndPermitsNode);
                    foreach (QualificationMapClass certificateAndPermit in this.CertificateAndPermitCollection)
                    {
                        var certificateAndPermitNode = certificatesAndPermitsNode.OwnerDocument?.CreateNode(XmlNodeType.Element,
                                                                                                            "CertificateAndPermit",
                                                                                                            null);
                        certificateAndPermit.Store(certificateAndPermitNode);
                        if (certificateAndPermitNode != null)
                        {
                            certificatesAndPermitsNode.AppendChild(certificateAndPermitNode);
                        }
                    }
                }

                if (this.HasRole(COMPANY_ROLE.CARRIER))
                {
                    var accessScheduleNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AccessSchedule", null);
                    if (accessScheduleNode != null)
                    {
                        companyNode.AppendChild(accessScheduleNode);
                        foreach (ScheduleClass schedule in this.AccessScheduleCollection)
                        {
                            var scheduleNode = accessScheduleNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AccessScheduleEntry", null);
                            schedule.Store(scheduleNode);
                            if (scheduleNode != null)
                            {
                                accessScheduleNode.AppendChild(scheduleNode);
                            }
                        }
                    }

                    var authorizedCustomersNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "AuthorizedCustomers", null);
                    if (authorizedCustomersNode != null)
                    {
                        companyNode.AppendChild(authorizedCustomersNode);
                        foreach (CompanyMapClass customer in this.CarrierCustomerShipToCollection)
                        {
                            var authorizedCustomerNode = authorizedCustomersNode.OwnerDocument?.CreateNode(XmlNodeType.Element,
                                                                                                            "AuthorizedCustomer",
                                                                                                            null);
                            customer.Store(authorizedCustomerNode);
                            if (authorizedCustomerNode != null)
                            {
                                authorizedCustomersNode.AppendChild(authorizedCustomerNode);
                            }
                        }
                    }

                    var driversNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Drivers", null);
                    if (driversNode != null)
                    {
                        companyNode.AppendChild(driversNode);
                        foreach (CompanyMapClass driver in this.AssignedPersonnelCollection)
                        {
                            var driverNode = driversNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Driver", null);
                            XmlAttribute attribute = driversNode.OwnerDocument?.CreateAttribute("ID");
                            if (attribute != null)
                            {
                                attribute.Value = driver.AssignedToID;
                                driverNode?.Attributes?.Append(attribute);
                            }

                            attribute = driversNode.OwnerDocument?.CreateAttribute("FirstName");
                            if (attribute != null)
                            {
                                attribute.Value = driver.AssignedToName;
                                driverNode?.Attributes?.Append(attribute);
                            }

                            attribute = driversNode.OwnerDocument?.CreateAttribute("MiddleName");
                            if (attribute != null)
                            {
                                attribute.Value = driver.AssignedToMiddleName;
                                driverNode?.Attributes?.Append(attribute);
                            }

                            attribute = driversNode.OwnerDocument?.CreateAttribute("LastName");
                            if (attribute != null)
                            {
                                attribute.Value = driver.AssignedToMiddleName;
                                driverNode?.Attributes?.Append(attribute);
                            }

                            if (driverNode != null)
                            {
                                driversNode.AppendChild(driverNode);
                            }
                        }
                    }

                    var equipmentsNode = companyNode.OwnerDocument?.CreateNode(XmlNodeType.Element, "Equipment", null);
                    if (equipmentsNode != null)
                    {
                        companyNode.AppendChild(equipmentsNode);
                        foreach (EquipmentClass equipment in this.EquipmentCollection)
                        {
                            var equipmentNode = equipmentsNode.OwnerDocument?.CreateNode(XmlNodeType.Element,
                                                                                        EquipmentTypeClass.TypeID(equipment.Type),
                                                                                        null);
                            XmlAttribute attribute = equipmentsNode.OwnerDocument?.CreateAttribute("ID");
                            if (attribute != null)
                            {
                                attribute.Value = equipment.ID;
                                equipmentNode?.Attributes?.Append(attribute);
                            }

                            if (equipmentNode != null)
                            {
                                equipmentsNode.AppendChild(equipmentNode);
                            }
                        }
                    }
                }
            }
        }

        public void UpdateSQL(SqlCommand cmd, DATA_TYPE type)
        {
            if (type == DATA_TYPE.CONFIG)
            {
                cmd.CommandText = "UPDATE tblCompanies " + "SET SiteGuid = @SiteGuid," + "ID = @ID," + "Code = @Code,"
                                  + "Name = @Name," + "ShortName = @ShortName," + "Address1 = @Address1," + "Address2 = @Address2," + "City = @City,"
                                  + "State = @State," + "Zip = @Zip," + "Country = @Country," + "Phone = @Phone," + "Fax = @Fax,"
                                  + "EmergencyContact = @EmergencyContact," + "EmergencyPhone = @EmergencyPhone,"
                                  + "FlightPrefix = @FlightPrefix," + "EffectiveDate = @EffectiveDate,"
                                  + "ExpirationDate = @ExpirationDate," + "IATAGuid = @IATAGuid," + "OnHold = @OnHold,"
                                  + "PickupFlights = @PickupFlights," + "StockTrack = @StockTrack,"
                                  + "SufferLossGain = @SufferLossGain," + "LowStockWarning = @LowStockWarning,"
                                  + "LockedOut = @LockedOut," + "LockedOutReason = @LockedOutReason,"
                                  + "LockedOutDate = @LockedOutDate,"
                                  + "ShipperTypeApplicationStringGuid = @ShipperTypeApplicationStringGuid,"
                                  + "CustomerBillToTypeApplicationStringGuid = @CustomerBillToTypeApplicationStringGuid,"
                                  + "CustomerShipToTypeApplicationStringGuid = @CustomerShipToTypeApplicationStringGuid,"
                                  + "ReceivableAccount = @ReceivableAccount," + "RefinerCode = @RefinerCode,"
                                  + "CreditOK = @CreditOK," + "AdditiveAccounting = @AdditiveAccounting,"
                                  + "PurchaseOrderRequired = @PurchaseOrderRequired," + "EPANumber = @EPANumber,"
                                  + "FederalID = @FederalID," + "FederalID2 = @FederalID2," + "FederalID3 = @FederalID3," + "FederalID4 = @FederalID4," + "FederalID5 = @FederalID5," + "StateID = @StateID,"
                                  + "TaxNumber = @TaxNumber," + "FlushPermitted = @FlushPermitted,"
                                  + "PumpOffPermitted = @PumpOffPermitted,"
                                  + "DeliveryToTerminalPermitted = @DeliveryToTerminalPermitted,"
                                  + "LicenseNumber = @LicenseNumber," + "LicenseExpiration = @LicenseExpiration,"
                                  + "InsuranceCompany = @InsuranceCompany," + "InsurancePolicy = @InsurancePolicy,"
                                  + "LiabilityAmount = @LiabilityAmount,"
                                  + "HazardousMaterialExclusion = @HazardousMaterialExclusion,"
                                  + "InsuranceExpiration = @InsuranceExpiration," + "AllowDriverEntry = @AllowDriverEntry,"
                                  + "PINRequired = @PINRequired," + "MaximumVehicleWeight = @MaximumVehicleWeight,"
                                  + "WeightUnits = @WeightUnits," + "AccountNumber = @AccountNumber," + "SCACCode = @SCACCode,"
                                  + "Note = @Note," + "DisableOwnerAllocationsCheck = @DisableOwnerAllocationsCheck,"
                                  + "DisableShipperAllocationsCheck = @DisableShipperAllocationsCheck,"
                                  + "DisableBillToAllocationsCheck = @DisableBillToAllocationsCheck,"
                                  + "DisableShipToAllocationsCheck = @DisableShipToAllocationsCheck,"
                                  + "LoadRackDisplayText = @LoadRackDisplayText," + "Contact1Name = @Contact1Name,"
                                  + "Contact1Address1 = @Contact1Address1," + "Contact1Address2 = @Contact1Address2,"
                                  + "Contact1City = @Contact1City," + "Contact1State = @Contact1State,"
                                  + "Contact1Zip = @Contact1Zip," + "Contact1Country = @Contact1Country,"
                                  + "Contact1PhoneOffice = @Contact1PhoneOffice," + "Contact1PhoneMobile = @Contact1PhoneMobile,"
                                  + "Contact1Fax = @Contact1Fax," + "Contact1EmailAddress = @Contact1EmailAddress,"
                                  + "Contact2Name = @Contact2Name," + "Contact2Address1 = @Contact2Address1,"
                                  + "Contact2Address2 = @Contact2Address2," + "Contact2City = @Contact2City,"
                                  + "Contact2State = @Contact2State," + "Contact2Zip = @Contact2Zip,"
                                  + "Contact2Country = @Contact2Country," + "Contact2PhoneOffice = @Contact2PhoneOffice,"
                                  + "Contact2PhoneMobile = @Contact2PhoneMobile," + "Contact2Fax = @Contact2Fax,"
                                  + "Contact2EmailAddress = @Contact2EmailAddress,"
                                  + "HiddenDate = @HiddenDate,"
                                  + "UserData1 = @UserData0,"
                                  + "UserData2 = @UserData1," + "UserData3 = @UserData2," + "UserData4 = @UserData3,"
                                  + "UserData5 = @UserData4," + "UserData6 = @UserData5," + "UserData7 = @UserData6,"
                                  + "UserData8 = @UserData7," + "UpdatedDate = @UpdatedDate," + "UpdatedBy = @UpdatedBy, "
                                  + "ScullyRequired = @ScullyRequired, " + "ConsortiumTypeIndex = @ConsortiumTypeIndex, "
                                  + "CompanyIATACode = @CompanyIataCode, " + "CompanyICAOCode = @CompanyIcaoCode "
                                  + "WHERE CompanyGuid = @IdentityGuid";
            }
            else
            {
                cmd.CommandText = "UPDATE tblCompanies " + "SET LastActivityDate = @LastActivityDate,"
                                  + "UpdatedDate = @UpdatedDate," + "UpdatedBy = @UpdatedBy " + "WHERE CompanyGuid = @IdentityGuid";
            }

            cmd.Parameters.Add("@SiteGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ID", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@Code", SqlDbType.NVarChar, 10);
            cmd.Parameters.Add("@Name", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@ShortName", SqlDbType.NVarChar, 4);
            cmd.Parameters.Add("@Address1", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@Address2", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@City", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@State", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Zip", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Country", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Phone", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Fax", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@EmergencyContact", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@EmergencyPhone", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FlightPrefix", SqlDbType.NVarChar, 5);
            cmd.Parameters.Add("@EffectiveDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@ExpirationDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@IATAGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@OnHold", SqlDbType.Bit);
            cmd.Parameters.Add("@PickupFlights", SqlDbType.Bit);
            cmd.Parameters.Add("@StockTrack", SqlDbType.Bit);
            cmd.Parameters.Add("@SufferLossGain", SqlDbType.Bit);
            cmd.Parameters.Add("@LowStockWarning", SqlDbType.Float);
            cmd.Parameters.Add("@LockedOut", SqlDbType.Bit);
            cmd.Parameters.Add("@LockedOutReason", SqlDbType.NVarChar, 80);
            cmd.Parameters.Add("@LockedOutDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@ShipperTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@CustomerBillToTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@CustomerShipToTypeApplicationStringGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@ReceivableAccount", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@RefinerCode", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@CreditOK", SqlDbType.Bit);
            cmd.Parameters.Add("@AdditiveAccounting", SqlDbType.Bit);
            cmd.Parameters.Add("@PurchaseOrderRequired", SqlDbType.Bit);
            cmd.Parameters.Add("@EPANumber", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID2", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID3", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID4", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FederalID5", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@StateID", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@TaxNumber", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@FlushPermitted", SqlDbType.Bit);
            cmd.Parameters.Add("@PumpOffPermitted", SqlDbType.Bit);
            cmd.Parameters.Add("@DeliveryToTerminalPermitted", SqlDbType.Bit);
            cmd.Parameters.Add("@LicenseNumber", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@LicenseExpiration", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@InsuranceCompany", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@InsurancePolicy", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@LiabilityAmount", SqlDbType.Money);
            cmd.Parameters.Add("@HazardousMaterialExclusion", SqlDbType.Bit);
            cmd.Parameters.Add("@InsuranceExpiration", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@AllowDriverEntry", SqlDbType.Bit);
            cmd.Parameters.Add("@PINRequired", SqlDbType.Bit);
            cmd.Parameters.Add("@ScullyRequired", SqlDbType.Bit);
            cmd.Parameters.Add("@ConsortiumTypeIndex", SqlDbType.Int);
            cmd.Parameters.Add("@MaximumVehicleWeight", SqlDbType.Float);
            cmd.Parameters.Add("@WeightUnits", SqlDbType.SmallInt);
            cmd.Parameters.Add("@AccountNumber", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@SCACCode", SqlDbType.NVarChar, 4);
            cmd.Parameters.Add("@Note", SqlDbType.NVarChar, 2000);
            cmd.Parameters.Add("@DisableOwnerAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@DisableShipperAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@DisableBillToAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@DisableShipToAllocationsCheck", SqlDbType.Bit);
            cmd.Parameters.Add("@LoadRackDisplayText", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1Name", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1Address1", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1Address2", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1City", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@Contact1State", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1Zip", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Contact1Country", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact1PhoneOffice", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1PhoneMobile", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1Fax", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact1EmailAddress", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2Name", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2Address1", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2Address2", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2City", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@Contact2State", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2Zip", SqlDbType.NVarChar, 11);
            cmd.Parameters.Add("@Contact2Country", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@Contact2PhoneOffice", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2PhoneMobile", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2Fax", SqlDbType.NVarChar, 20);
            cmd.Parameters.Add("@Contact2EmailAddress", SqlDbType.NVarChar, 30);
            cmd.Parameters.Add("@HiddenDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UserData0", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData1", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData2", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData3", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData4", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData5", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData6", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UserData7", SqlDbType.NVarChar, 60);
            cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 100);
            cmd.Parameters.Add("@IdentityGuid", SqlDbType.UniqueIdentifier);
            cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTimeOffset);
            cmd.Parameters.Add("@CompanyIataCode", SqlDbType.NVarChar, 50);
            cmd.Parameters.Add("@CompanyIcaoCode", SqlDbType.NVarChar, 50);

            cmd.Parameters["@SiteGuid"].Value = this.SiteGuid;
            cmd.Parameters["@ID"].Value = this._ID;
            cmd.Parameters["@Code"].Value = this._Code;
            cmd.Parameters["@Name"].Value = this._Name;
            cmd.Parameters["@ShortName"].Value = this._ShortName;
            cmd.Parameters["@Address1"].Value = this._Address1;
            cmd.Parameters["@Address2"].Value = this._Address2;
            cmd.Parameters["@City"].Value = this._City;
            cmd.Parameters["@State"].Value = this._State;
            cmd.Parameters["@Zip"].Value = this._Zip;
            cmd.Parameters["@Country"].Value = this._Country;
            cmd.Parameters["@Phone"].Value = this._Phone;
            cmd.Parameters["@Fax"].Value = this._Fax;
            cmd.Parameters["@EmergencyContact"].Value = this._EmergencyContact;
            cmd.Parameters["@EmergencyPhone"].Value = this._EmergencyPhone;
            cmd.Parameters["@FlightPrefix"].Value = this._FlightPrefix;
            cmd.Parameters["@EffectiveDate"].Value = this._EffectiveDate.Value;
            cmd.Parameters["@ExpirationDate"].Value = this._ExpirationDate.Value;
            cmd.Parameters["@CompanyIataCode"].Value = this.companyIataCode;
            cmd.Parameters["@CompanyIcaoCode"].Value = this.companyIcaoCode;

            if (this.IATAGuid != Guid.Empty)
            {
                cmd.Parameters["@IATAGuid"].Value = this.IATAGuid;
            }
            else
            {
                cmd.Parameters["@IATAGuid"].Value = DBNull.Value;
            }

            if (this.OnHold)
            {
                cmd.Parameters["@OnHold"].Value = 1;
            }
            else
            {
                cmd.Parameters["@OnHold"].Value = 0;
            }

            if (this.PickupFlights)
            {
                cmd.Parameters["@PickupFlights"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PickupFlights"].Value = 0;
            }

            if (this.StockTrack)
            {
                cmd.Parameters["@StockTrack"].Value = 1;
            }
            else
            {
                cmd.Parameters["@StockTrack"].Value = 0;
            }

            if (this.SufferLossGain)
            {
                cmd.Parameters["@SufferLossGain"].Value = 1;
            }
            else
            {
                cmd.Parameters["@SufferLossGain"].Value = 0;
            }

            cmd.Parameters["@LowStockWarning"].Value = this.LowStockWarning;

            if (this.LockedOut)
            {
                cmd.Parameters["@LockedOut"].Value = 1;
            }
            else
            {
                cmd.Parameters["@LockedOut"].Value = 0;
            }

            cmd.Parameters["@LockedOutReason"].Value = this._LockedOutReason;

            cmd.Parameters["@LockedOutDate"].Value = this._LockedOutDate.Value;

            if (this.ShipperTypeApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters["@ShipperTypeApplicationStringGuid"].Value = this.ShipperTypeApplicationStringGuid;
            }
            else
            {
                cmd.Parameters["@ShipperTypeApplicationStringGuid"].Value = DBNull.Value;
            }

            if (this.CustomerBillToTypeApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters["@CustomerBillToTypeApplicationStringGuid"].Value = this.CustomerBillToTypeApplicationStringGuid;
            }
            else
            {
                cmd.Parameters["@CustomerBillToTypeApplicationStringGuid"].Value = DBNull.Value;
            }

            if (this.CustomerShipToTypeApplicationStringGuid != Guid.Empty)
            {
                cmd.Parameters["@CustomerShipToTypeApplicationStringGuid"].Value = this.CustomerShipToTypeApplicationStringGuid;
            }
            else
            {
                cmd.Parameters["@CustomerShipToTypeApplicationStringGuid"].Value = DBNull.Value;
            }

            cmd.Parameters["@ReceivableAccount"].Value = this._ReceivableAccount;
            cmd.Parameters["@RefinerCode"].Value = this._RefinerCode;

            if (this.CreditOK)
            {
                cmd.Parameters["@CreditOK"].Value = 1;
            }
            else
            {
                cmd.Parameters["@CreditOK"].Value = 0;
            }

            if (this.AdditiveAccounting)
            {
                cmd.Parameters["@AdditiveAccounting"].Value = 1;
            }
            else
            {
                cmd.Parameters["@AdditiveAccounting"].Value = 0;
            }

            if (this.PurchaseOrderRequired)
            {
                cmd.Parameters["@PurchaseOrderRequired"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PurchaseOrderRequired"].Value = 0;
            }

            cmd.Parameters["@EPANumber"].Value = this._EPANumber;
            cmd.Parameters["@FederalID"].Value = this._FederalID;
            cmd.Parameters["@FederalID2"].Value = this._FederalID2;
            cmd.Parameters["@FederalID3"].Value = this._FederalID3;
            cmd.Parameters["@FederalID4"].Value = this._FederalID4;
            cmd.Parameters["@FederalID5"].Value = this._FederalID5;
            cmd.Parameters["@StateID"].Value = this._StateID;
            cmd.Parameters["@TaxNumber"].Value = this._TaxNumber;

            if (this.FlushPermitted)
            {
                cmd.Parameters["@FlushPermitted"].Value = 1;
            }
            else
            {
                cmd.Parameters["@FlushPermitted"].Value = 0;
            }

            if (this.PumpOffPermitted)
            {
                cmd.Parameters["@PumpOffPermitted"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PumpOffPermitted"].Value = 0;
            }

            if (this.DeliveryToTerminalPermitted)
            {
                cmd.Parameters["@DeliveryToTerminalPermitted"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DeliveryToTerminalPermitted"].Value = 0;
            }

            cmd.Parameters["@LicenseNumber"].Value = this._LicenseNumber;
            cmd.Parameters["@LicenseExpiration"].Value = this._LicenseExpiration.Value;
            cmd.Parameters["@InsuranceCompany"].Value = this._InsuranceCompany;
            cmd.Parameters["@InsurancePolicy"].Value = this._InsurancePolicy;
            cmd.Parameters["@LiabilityAmount"].Value = this._LiabilityAmount.Value;

            if (this.HazardousMaterialExclusion)
            {
                cmd.Parameters["@HazardousMaterialExclusion"].Value = 1;
            }
            else
            {
                cmd.Parameters["@HazardousMaterialExclusion"].Value = 0;
            }

            cmd.Parameters["@InsuranceExpiration"].Value = this._InsuranceExpiration.Value;

            if (this.AllowDriverEntry)
            {
                cmd.Parameters["@AllowDriverEntry"].Value = 1;
            }
            else
            {
                cmd.Parameters["@AllowDriverEntry"].Value = 0;
            }

            if (this.PINRequired)
            {
                cmd.Parameters["@PINRequired"].Value = 1;
            }
            else
            {
                cmd.Parameters["@PINRequired"].Value = 0;
            }

            if (this.ScullyRequired)
            {
                cmd.Parameters["@ScullyRequired"].Value = 1;
            }
            else
            {
                cmd.Parameters["@ScullyRequired"].Value = 0;
            }

            if (this.ConsortiumType != null)
            {
                cmd.Parameters["@ConsortiumTypeIndex"].Value = (int?)this.ConsortiumType;
            }
            else
            {
                cmd.Parameters["@ConsortiumTypeIndex"].Value = DBNull.Value;

            }
            cmd.Parameters["@MaximumVehicleWeight"].Value = this._MaximumVehicleWeight;
            cmd.Parameters["@WeightUnits"].Value = (int)this._WeightUnits;

            cmd.Parameters["@AccountNumber"].Value = this._AccountNumber;
            cmd.Parameters["@SCACCode"].Value = this._SCACCode;

            if (this.Note != string.Empty)
            {
                cmd.Parameters["@Note"].Value = this.Note;
            }
            else
            {
                cmd.Parameters["@Note"].Value = DBNull.Value;
            }

            if (this.DisableOwnerAllocationsCheck)
            {
                cmd.Parameters["@DisableOwnerAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableOwnerAllocationsCheck"].Value = 0;
            }

            if (this.DisableShipperAllocationsCheck)
            {
                cmd.Parameters["@DisableShipperAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableShipperAllocationsCheck"].Value = 0;
            }

            if (this.DisableBillToAllocationsCheck)
            {
                cmd.Parameters["@DisableBillToAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableBillToAllocationsCheck"].Value = 0;
            }

            if (this.DisableShipToAllocationsCheck)
            {
                cmd.Parameters["@DisableShipToAllocationsCheck"].Value = 1;
            }
            else
            {
                cmd.Parameters["@DisableShipToAllocationsCheck"].Value = 0;
            }

            cmd.Parameters["@LoadRackDisplayText"].Value = this._LoadRackDisplayText;
            cmd.Parameters["@Contact1Name"].Value = string.IsNullOrEmpty(this._Contact1Name)
                ? (object)DBNull.Value
                : this._Contact1Name;
            cmd.Parameters["@Contact1Address1"].Value = string.IsNullOrEmpty(this._Contact1Address1)
                ? (object)DBNull.Value
                : this._Contact1Address1;
            cmd.Parameters["@Contact1Address2"].Value = string.IsNullOrEmpty(this._Contact1Address2)
                ? (object)DBNull.Value
                : this._Contact1Address2;
            cmd.Parameters["@Contact1City"].Value = string.IsNullOrEmpty(this._Contact1City)
                ? (object)DBNull.Value
                : this._Contact1City;
            cmd.Parameters["@Contact1State"].Value = string.IsNullOrEmpty(this._Contact1State)
                ? (object)DBNull.Value
                : this._Contact1State;
            cmd.Parameters["@Contact1Zip"].Value = string.IsNullOrEmpty(this._Contact1Zip)
                ? (object)DBNull.Value
                : this._Contact1Zip;
            cmd.Parameters["@Contact1Country"].Value = string.IsNullOrEmpty(this._Contact1Country)
                ? (object)DBNull.Value
                : this._Contact1Country;
            cmd.Parameters["@Contact1PhoneOffice"].Value = string.IsNullOrEmpty(this._Contact1PhoneOffice)
                ? (object)DBNull.Value
                : this._Contact1PhoneOffice;
            cmd.Parameters["@Contact1PhoneMobile"].Value = string.IsNullOrEmpty(this._Contact1PhoneMobile)
                ? (object)DBNull.Value
                : this._Contact1PhoneMobile;
            cmd.Parameters["@Contact1Fax"].Value = string.IsNullOrEmpty(this._Contact1Fax)
                ? (object)DBNull.Value
                : this._Contact1Fax;
            cmd.Parameters["@Contact1EmailAddress"].Value = string.IsNullOrEmpty(this._Contact1EmailAddress)
                ? (object)DBNull.Value
                : this._Contact1EmailAddress;
            cmd.Parameters["@Contact2Name"].Value = string.IsNullOrEmpty(this._Contact2Name)
                ? (object)DBNull.Value
                : this._Contact2Name;
            cmd.Parameters["@Contact2Address1"].Value = string.IsNullOrEmpty(this._Contact2Address1)
                ? (object)DBNull.Value
                : this._Contact2Address1;
            cmd.Parameters["@Contact2Address2"].Value = string.IsNullOrEmpty(this._Contact2Address2)
                ? (object)DBNull.Value
                : this._Contact2Address2;
            cmd.Parameters["@Contact2City"].Value = string.IsNullOrEmpty(this._Contact2City)
                ? (object)DBNull.Value
                : this._Contact2City;
            cmd.Parameters["@Contact2State"].Value = string.IsNullOrEmpty(this._Contact2State)
                ? (object)DBNull.Value
                : this._Contact2State;
            cmd.Parameters["@Contact2Zip"].Value = string.IsNullOrEmpty(this._Contact2Zip)
                ? (object)DBNull.Value
                : this._Contact2Zip;
            cmd.Parameters["@Contact2Country"].Value = string.IsNullOrEmpty(this._Contact2Country)
                ? (object)DBNull.Value
                : this._Contact2Country;
            cmd.Parameters["@Contact2PhoneOffice"].Value = string.IsNullOrEmpty(this._Contact2PhoneOffice)
                ? (object)DBNull.Value
                : this._Contact2PhoneOffice;
            cmd.Parameters["@Contact2PhoneMobile"].Value = string.IsNullOrEmpty(this._Contact2PhoneMobile)
                ? (object)DBNull.Value
                : this._Contact2PhoneMobile;
            cmd.Parameters["@Contact2Fax"].Value = string.IsNullOrEmpty(this._Contact2Fax)
                ? (object)DBNull.Value
                : this._Contact2Fax;
            cmd.Parameters["@Contact2EmailAddress"].Value = string.IsNullOrEmpty(this._Contact2EmailAddress)
                ? (object)DBNull.Value
                : this._Contact2EmailAddress;

            cmd.Parameters["@HiddenDate"].Value = this.HiddenDate ?? (object)DBNull.Value;
            cmd.Parameters["@UserData0"].Value = this.UserData[0];
            cmd.Parameters["@UserData1"].Value = this.UserData[1];
            cmd.Parameters["@UserData2"].Value = this.UserData[2];
            cmd.Parameters["@UserData3"].Value = this.UserData[3];
            cmd.Parameters["@UserData4"].Value = this.UserData[4];
            cmd.Parameters["@UserData5"].Value = this.UserData[5];
            cmd.Parameters["@UserData6"].Value = this.UserData[6];
            cmd.Parameters["@UserData7"].Value = this.UserData[7];
            cmd.Parameters["@UpdatedBy"].Value = this._UpdatedBy;
            cmd.Parameters["@UpdatedDate"].Value = this.UpdatedDate;
            cmd.Parameters["@LastActivityDate"].Value = this._LastActivityDate.Value;
            cmd.Parameters["@IdentityGuid"].Value = this.IdentityGuid;
        }

        #endregion

        #region Methods

        private void Initialize()
        {
            this._Code = "";
            this._Name = "";
            this._ShortName = "";
            this._Address1 = "";
            this._Address2 = "";
            this._City = "";
            this._State = "";
            this._Zip = "";
            this._Country = "";
            this._Phone = "";
            this._Fax = "";
            this._EmergencyContact = "";
            this._EmergencyPhone = "";
            this._FlightPrefix = "";
            this._EffectiveDate.Value = TimeConverter.Today(this._EffectiveDate.StandardName);
            this._ExpirationDate.Value = TimeConverter.Today(this._ExpirationDate.StandardName);
            this.IATAGuid = Guid.Empty;
            this._OnHold = false;
            this._PickupFlights = false;
            this._StockTrack = false;
            this._SufferLossGain = false;
            this._LowStockWarning = 0.0;
            this._LockedOut = false;
            this._LockedOutReason = "";
            this._LockedOutDate.Value = TimeConverter.Today(this._LockedOutDate.StandardName);
            this._LastActivityDate.Value = TimeConverter.Now(this._LastActivityDate.StandardName);
            this.ShipperTypeApplicationStringGuid = Guid.Empty;
            this.CustomerBillToTypeApplicationStringGuid = Guid.Empty;
            this.CustomerShipToTypeApplicationStringGuid = Guid.Empty;
            this._ReceivableAccount = "";
            this._RefinerCode = "";
            this._CreditOK = true;
            this._AdditiveAccounting = false;
            this._PurchaseOrderRequired = false;
            this._EPANumber = "";
            this._FederalID = "";
            this._FederalID2 = "";
            this._FederalID3 = "";
            this._FederalID4 = "";
            this._FederalID5 = "";
            this._StateID = "";
            this._TaxNumber = "";
            this._FlushPermitted = false;
            this._PumpOffPermitted = false;
            this._DeliveryToTerminalPermitted = false;
            this._LicenseNumber = "";
            this._LicenseExpiration.Value = TimeConverter.Today(this._LicenseExpiration.StandardName);
            this._InsuranceCompany = "";
            this._InsurancePolicy = "";
            this._LiabilityAmount.Value = new Decimal(0);
            this._HazardousMaterialExclusion = false;
            this._InsuranceExpiration.Value = TimeConverter.Today(this._LicenseExpiration.StandardName);
            this._AllowDriverEntry = false;
            this._PINRequired = true;
            this._ScullyRequired = false;
            this._ConsortiumType = null;
            this._MaximumVehicleWeight = 0.0;
            this._WeightUnits = EngineeringUnit.FmmMTon;
            this._AccountNumber = "";
            this._SCACCode = "";
            this.Note = string.Empty;
            this._DisableOwnerAllocationsCheck = false;
            this._DisableShipperAllocationsCheck = false;
            this._DisableBillToAllocationsCheck = false;
            this._DisableShipToAllocationsCheck = false;
            this._LoadRackDisplayText = "";
            this._Contact1Name = string.Empty;
            this._Contact1Address1 = string.Empty;
            this._Contact1Address2 = string.Empty;
            this._Contact1City = string.Empty;
            this._Contact1State = string.Empty;
            this._Contact1Zip = string.Empty;
            this._Contact1Country = string.Empty;
            this._Contact1PhoneOffice = string.Empty;
            this._Contact1PhoneMobile = string.Empty;
            this._Contact1Fax = string.Empty;
            this._Contact1EmailAddress = string.Empty;
            this._Contact2Name = string.Empty;
            this._Contact2Address1 = string.Empty;
            this._Contact2Address2 = string.Empty;
            this._Contact2City = string.Empty;
            this._Contact2State = string.Empty;
            this._Contact2Zip = string.Empty;
            this._Contact2Country = string.Empty;
            this._Contact2PhoneOffice = string.Empty;
            this._Contact2PhoneMobile = string.Empty;
            this._Contact2Fax = string.Empty;
            this._Contact2EmailAddress = string.Empty;
            this.HiddenDate = null;
            this.UserData = new UserDataClass();
            this._IATAID = "";
            this._ShipperTypeID = "";
            this._CustomerBillToTypeID = "";
            this._CustomerShipToTypeID = "";
            this.RoleCollection = new CompanyRoleMapCollectionClass();
            this.AuthorizedCarrierCollection = new CompanyMapCollectionClass();
            this.CarrierCustomerShipToCollection = new CompanyMapCollectionClass();
            this.AssignedPersonnelCollection = new CompanyMapCollectionClass();
            this.EquipmentCollection = new EquipmentCollectionClass();
            this.CertificateAndPermitCollection = new QualificationMapCollectionClass();
            this.AuthorizedProductCollection = new ProductMapCollectionClass();
            this.UnavailableInventoryCollection = new ProductMapCollectionClass(); // added (IGO 02-Sep-2008)				
            this.GroupMapCollection = new CompanyMapCollectionClass();
            this.AccessScheduleCollection = new ScheduleCollectionClass();
            this.SupplierAuthorizedProductCollection = new ProductMapCollectionClass();
            this.companyIataCode = string.Empty;
            this.companyIcaoCode = string.Empty;
        }
		#endregion
    }

    #region application exception class

    /// <summary>
    ///     Custom exception for errors occurring in ReceiveImport
    /// </summary>
    [Serializable()]
    public class CompanyClassException : ApplicationException
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes a new instance of the Exception class.
        /// </summary>
        public CompanyClassException()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the Exception class with a specified error message.
        /// </summary>
        /// <param name="msg">Error message</param>
        public CompanyClassException(string msg)
            : base(msg)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the Exception class with a specified error message and
        ///     a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="msg">Error message</param>
        /// <param name="innerException">inner exception that is the cause of this exception</param>
        public CompanyClassException(string msg, Exception innerException)
            : base(msg, innerException)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the Exception class with serialized data
        /// </summary>
        /// <param name="info">The SerializationInfo that holds the serialized object data about the exception being thrown</param>
        /// <param name="context">The StreamingContext that contains contextual information about the source or destination</param>
        protected CompanyClassException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }

    #endregion
}