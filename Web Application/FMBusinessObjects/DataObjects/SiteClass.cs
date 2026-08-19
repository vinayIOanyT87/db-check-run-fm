// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SiteClass.cs" company="Varec, Inc.">
//	Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//	Defines the SiteCollectionClass type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Globalization;
	using System.Runtime.InteropServices;
	using System.Runtime.Serialization;
	using System.Text.RegularExpressions;
	using Constants;
	using UtilityObjects;

	using Varec.CommonComponents.EngineeringUnitsLibrary;

	#region Public Enumerations
	public enum NUMBER_GROUP_SIZES_TYPE
	{
		ZERO = 0,
		THREE = 1,
		TWOTHREE = 2
	}

	public enum MAIL_SERVER_CONNECT_MODE
	{
		LAN = 0,
		DIALUP = 1
	}

	public enum WATCHDOG_MODE : byte
	{
		TOGGLE = 0,
		COUNTER = 1
	}

	public enum SITE_VARIABLE_TYPE : byte
	{
		LENGTH = 0,
		TEMPERATURE = 1,
		DENSITY = 2,
		PRESSURE = 3,
		FLOW = 4,
		VOLUME = 5,
		MASS = 6,
		ADDITIVE_VOLUME = 7,
		VCF = 8,
		DEFAULT = 9,
		ADDITIVE_CYCLE_AMOUNT = 10,
		ADDITIVE_RATE_AMOUNT = 11,
		ADDITIVE_MASS = 12,
		PACKAGE = 13
	}

	public enum QuantityDisplay { GROSS_AND_NET, GROSS, NET, MASS, PACKAGE }

	public enum StrongPasswordUsage { None, Strong, Enhanced }

	public enum DOCUMENT_TYPE
	{
		AUTOMATIC_BOL = 0,
		MANUAL_BOL = 1,
		TRANSACTION = 2,
		ORDER = 3,
		MAX_TYPE = 4
	};
	#endregion

	/// <summary>
	/// The site collection class.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	[KnownType(typeof(SiteClass))]
	public class SiteCollectionClass : List<SiteClass>
	{
	}

	/// <summary>
	/// The site class.
	/// </summary>
	[Serializable]
	[DataContract]
	[KnownType(typeof(GregorianCalendar))]
	[KnownType(typeof(ProcessVariableClass))]
	[KnownType(typeof(ScheduleClass))]
	[KnownType(typeof(ApplicationStringClass))]
	public class SiteClass : FMBaseDataObjectWithUserData, IAlarmAndEventDiscovery
	{
		#region Public data members
		/// <summary>
		/// The fill method.
		/// </summary>
		public enum FILL_METHOD
		{
			SAFEFILL = 0,
			ACTUAL = 1
		};

		/// <summary>
		/// The entity type ID.
		/// </summary>
		public const string ENTITY_TYPE_ID = "Sites";

		/// <summary>
		/// The max user data.
		/// </summary>
		public const int MAX_USER_DATA = 8;

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
		public byte _AdditiveProfileCycleAmountDecimalPlaces;

		[DataMember]
		public byte _AdditiveProfileRateDecimalPlaces;

		[DataMember]
		public int _MaximumLoadTime;

		[DataMember]
		public int _MaximumIdleTime;

		[DataMember]
		public SIDouble _MaximumLoadAmount;
		[DataMember]
		public SIDouble _MaximumFlushAmount;
		[DataMember]
		public SIDouble _MaximumMeterProvingAmount;
		[DataMember]
		public SIDouble _MaximumReturnsAmount;


		[DataMember]
		public int _MaximumNumberOfActiveArms;

		[DataMember]
		public int _DriverTimeoutPeriod;

		[DataMember]
		public int _DriverWarningPeriod;

		[DataMember]
		public int _MaximumPrompts;


		[DataMember]
		public int _AutomaticBOLStartNumber;

		[DataMember]
		public int _AutomaticBOLEndNumber;

		[DataMember]
		public int _AutomaticBOLNextNumber;

		[DataMember]
		protected bool _SeparateManualBOLNumbering;

		[DataMember]
		public int _ManualBOLStartNumber;

		[DataMember]
		public int _ManualBOLEndNumber;

		[DataMember]
		public int _ManualBOLNextNumber;

		[DataMember]
		public int _TransactionStartNumber;

		[DataMember]
		public int _TransactionEndNumber;

		[DataMember]
		public int _TransactionNextNumber;

		[DataMember]
		public int _OrderStartNumber;

		[DataMember]
		public int _OrderEndNumber;

		[DataMember]
		public int _OrderNextNumber;

		// vt 07-15-2008
		[DataMember]
		public int _InvoiceStartNumber;

		[DataMember]
		public int _InvoiceEndNumber;

		[DataMember]
		public int _InvoiceNextNumber;

		[DataMember]
		public int _EndOfDayWarningPeriod;

		[DataMember]
		public SIDouble _MaximumVehicleWeight = new SIDouble( );

		[DataMember]
		public SIDouble _MaximumProductTemperature = new SIDouble( );

		[DataMember]
		public int _OpenTransactionWindow;

		[DataMember]
		public Date _AdministrativeLockDate = new Date( );

		[DataMember]
		public DateAndTime _OperationalLockDate = new DateAndTime( );

		// System
		[DataMember]
		public int _MaximumDaysToRetainLogs;

		// Vapor Recovery Unit (VRU)
		[DataMember]
		public SIDouble _VRURateLimit = new SIDouble( );

		[DataMember]
		public SIDouble _VRUHourlyLimit = new SIDouble( );

		[DataMember]
		public SIDouble _VRUDailyLimit = new SIDouble( );

		[DataMember]
		public SIDouble _VRUYearlyLimit = new SIDouble( );

		[DataMember]
		public SIDouble _VRUCurrentYearLimit = new SIDouble( );

		[DataMember]
		public SIDouble _VRURateActual = new SIDouble( );

		[DataMember]
		public SIDouble _VRUHourlyActual = new SIDouble( );

		[DataMember]
		public SIDouble _VRUDailyActual = new SIDouble( );

		[DataMember]
		public SIDouble _VRUYearlyActual = new SIDouble( );

		[DataMember]
		public SIDouble _VRUCurrentYearActual = new SIDouble( );

		// Process I/O
		[DataMember]
		public int _WatchdogPeriod;

		[DataMember]
		public int _WatchdogCounterStart;

		[DataMember]
		public int _WatchdogCounterEnd;

		//Notes
		[DataMember]
		public Guid NoteGuid;

		[DataMember]
		public NoteClass Note;

		// Data obtained from subquery
		[DataMember]
		public string InventoryTransactionAliasID;
		[DataMember]
		public string AdjustmentTransactionAliasID;
		[DataMember]
		public string IATAID;

		// Collections
		[DataMember]
		public ScheduleCollectionClass OperatingScheduleCollection;

		[DataMember]
		public ScheduleCollectionClass HolidayScheduleCollection;

		[DataMember]
		public ProcessVariableCollectionClass ProcessVariableCollection;

		[DataMember]
		public SiteToSiteMapCollectionClass SiteToSiteMapCollection;

		[DataMember]
		public ApplicationStringCollectionClass SiteCertificateCollection;

		// TFS #30561
		[DataMember]
		public bool _EnablePasswordHint;

		[DataMember]
		public bool _EnablePasswordReset;

		[DataMember]
		public bool _AllowUseOfSpecialChars;

		[DataMember]
		public bool _EnablePeriodicSyncFlag;

		[DataMember]
		public int _PeriodicSyncIntervalMinutes;

		[DataMember]
		public bool _DisableSyncTransferFlag;

		// Alarm and Events
		public static AlarmAndEventDescriptorClass ManualEndOfDayEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ManualEndOfDayKey);
		public static AlarmAndEventDescriptorClass AutomaticEndOfDayEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, AutomaticEndOfDayKey);
		public static AlarmAndEventDescriptorClass AutomaticEndOfMonthEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, AutomaticEndOfMonthKey);
		public static AlarmAndEventDescriptorClass UseLastKnownGoodTankDataEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, UseLastKnownGoodTankDataKey);
		public static AlarmAndEventDescriptorClass UseCurrentTankDataEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, UseCurrentTankDataKey);
		public static AlarmAndEventDescriptorClass EndOfDayProcessingBeginEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EndOfDayProcessingBeginKey);
		public static AlarmAndEventDescriptorClass EndOfMonthProcessingBeginDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EndOfMonthProcessingBeginKey);
		public static AlarmAndEventDescriptorClass EndOfDayProcessingEndEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EndOfDayProcessingEndKey);
		public static AlarmAndEventDescriptorClass EndOfMonthProcessingEndEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EndOfMonthProcessingEndKey);
		public static AlarmAndEventDescriptorClass ReloadTanksEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ReloadTanksKey);
		public static AlarmAndEventDescriptorClass EndOfDayFailedPhysInvCreateEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EodFailedPhysicalInvCreateKey);
		public static AlarmAndEventDescriptorClass EndOfMonthFailedPhysInvCreateEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EomFailedPhysicalInvCreateKey);
		public static AlarmAndEventDescriptorClass TankIgnoredPhysInvCreateEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, TankIgnoredPhysicalInvCreateKey);
		#endregion

		#region Protected data members
		// General
		[DataMember]
		protected string _Number;

		[DataMember]
		protected string _SPLCCode;

		[DataMember]
		protected string _Address1;

		[DataMember]
		protected string _Address2;

		[DataMember]
		protected string _City;

		[DataMember]
		protected string _State;

		[DataMember]
		protected string _Zip;

		[DataMember]
		protected string _Country;

		[DataMember]
		protected string _Phone;

		[DataMember]
		protected string _Fax;

		[DataMember]
		protected string _EmailAddress;

		[DataMember]
		protected string _EmergencyContact;

		[DataMember]
		protected string _EmergencyPhone;

		[DataMember]
		protected bool _Enabled;

		[DataMember]
		protected bool _SiteGroup;

		[DataMember]
		protected string _TimeZone;

		[DataMember]
		protected string _TerminalControlNumber;

		[DataMember]
		protected bool _InhibitLoadRackCardIns;

		[DataMember]
		protected Guid _IATAGuid;

		[DataMember]
		protected bool _inhibitSiteLedgerRollup;

		[DataMember]
		protected string _Contact1Name;
		[DataMember]
		protected string _Contact1Address1;
		[DataMember]
		protected string _Contact1Address2;
		[DataMember]
		protected string _Contact1City;
		[DataMember]
		protected string _Contact1State;
		[DataMember]
		protected string _Contact1Zip;
		[DataMember]
		protected string _Contact1Country;
		[DataMember]
		protected string _Contact1PhoneOffice;
		[DataMember]
		protected string _Contact1PhoneMobile;
		[DataMember]
		protected string _Contact1Fax;
		[DataMember]
		protected string _Contact1EmailAddress;
		[DataMember]
		protected string _Contact2Name;
		[DataMember]
		protected string _Contact2Address1;
		[DataMember]
		protected string _Contact2Address2;
		[DataMember]
		protected string _Contact2City;
		[DataMember]
		protected string _Contact2State;
		[DataMember]
		protected string _Contact2Zip;
		[DataMember]
		protected string _Contact2Country;
		[DataMember]
		protected string _Contact2PhoneOffice;
		[DataMember]
		protected string _Contact2PhoneMobile;
		[DataMember]
		protected string _Contact2Fax;
		[DataMember]
		protected string _Contact2EmailAddress;
		[DataMember]
		protected double? latitude;
		[DataMember]
		protected double? longitude;
		[DataMember]
		protected int? zoom;
		  [DataMember]
		 protected Guid activeDirectorySiteGroupGuid;

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

		[DataMember]
		protected EngineeringUnit _AdditiveProfileCycleAmountUnits;

		[DataMember]
		protected EngineeringUnit _AdditiveProfileRateUnits;

		[DataMember]
		protected QuantityDisplay _QuantityDisplayDefault;

		// Load Rack
		[DataMember]
		protected bool _InhibitAccessAfterHours;

		[DataMember]
		protected bool _InhibitMultipleCardIns;

		[DataMember]
		protected bool _AccessCardInRequired;

		[DataMember]
		protected bool _CheckSiteNumber;

		[DataMember]
		protected bool _PromptForCustomerCard;

		[DataMember]
		protected bool _PromptForTractorOrTanker;

		[DataMember]
		protected bool _PromptForFirstTrailer;

		[DataMember]
		protected bool _PromptForSecondTrailer;

		[DataMember]
		protected bool _PromptForThirdTrailer;

		[DataMember]
		protected bool _PromptForCompartment;

		[DataMember]
		protected bool _PromptForTransactionCompletion;

		[DataMember]
		protected bool _InhibitCustomerConfirmationPrompt;

		[DataMember]
		protected bool _RequireTrailerScully;

		[DataMember]
		protected int _CardInTimeout;

		[DataMember]
		protected bool _EnforceDriverEquipmentMatch;

		[DataMember]
		protected bool _EnableAdditiveAccounting;

		[DataMember]
		protected bool _UseCompanyEquipmentIdentifiers;

		[DataMember]
		protected bool _UseLastKnownGoodTankData;

		[DataMember]
		protected Guid _InventoryTransactionAliasGuid;

		[DataMember]
		protected Guid _AdjustmentTransactionAliasGuid;

		[DataMember]
		protected bool _LoadByNet;

		[DataMember]
		protected bool _PromptForShipmentNumber;

		[DataMember]
		protected bool _ListEquipment;

		[DataMember]
		protected bool _DeferStationChanges;

		[DataMember]
		protected bool _PromptForReturns;

		[DataMember]
		protected bool _PromptForTruckCard;

		[DataMember]
		protected int _StartingShortCardNumber;

		[DataMember]
		protected bool _UseShortCardNumber;

		[DataMember]
		protected byte _ExcessVarianceCount;

		[DataMember]
		protected double _ExcessVarianceTolerance;

		[DataMember]
		protected FILL_METHOD _SecondaryStorageFillMethod;

		// Transactions
		[DataMember]
		protected bool _InhibitBOLWithBrokenBlends;

		[DataMember]
		protected bool _InhibitBOLWithImproperAdditization;

		[DataMember]
		protected bool _InhibitOverweightBOL;

		[DataMember]
		protected string _ExceptionBOLPrinter;

		[DataMember]
		protected bool _EnableAutomaticBOLPrinting;

		[DataMember]
		protected bool _EnableBOLPDFArchiving;

		[DataMember]
		protected string _BOLPDFArchivingPath;

		[DataMember]
		protected string _NumberPrefix;

		[DataMember]
		protected bool _EnableDebugLogging;

		[DataMember]
		protected bool _EnableAuditLogging;

		[DataMember]
		protected bool _AutomaticallyPrintAlarmsAndEvents;

		[DataMember]
		protected string _AlarmAndEventPrinter;

		[DataMember]
		protected string _MailServer;

		[DataMember]
		protected string _MailFrom;

		[DataMember]
		protected string _MailUserName;

		[DataMember]
		protected string _MailPassword;

		[DataMember]
		protected MAIL_SERVER_CONNECT_MODE _MailConnectMode;

		[DataMember]
		protected string _DialupName;

		[DataMember]
		protected string _SCADASystem;

		[DataMember]
		protected bool _InhibitTemplateGraphics;

		[DataMember]
		public int _RefreshInterval;

		[DataMember]
		protected bool _InhibitEndOfDayOperations;

		[DataMember]
		protected bool _InhibitEndOfMonthOperations;

		[DataMember]
		protected bool _InhibitAutomaticPhysicalInventory;

		[DataMember]
		protected bool _InhibitAutomaticMeterCloseout;

		[DataMember]
		protected bool _InhibitAutomaticReportGeneration;

		[DataMember]
		protected bool _InhibitAutomaticAdjustmentDistribution;

		[DataMember]
		protected bool _InhibitAutomaticCloseout;

		[DataMember]
		protected bool _InhibitTankScan;

		[DataMember]
		protected string _ReportDirectory;

		[DataMember]
		protected bool _ManageReports;

		[DataMember]
		protected string _ManagedReportDirectory;

		[DataMember]
		protected bool _EnforceSingleOwner;

		[DataMember]
		protected bool _InhibitBOLSummaryAutoPopulate;

		[DataMember]
		protected bool _InhibitOrderSummaryAutoPopulate;

		[DataMember]
		protected bool _InhibitSupplyOrderSummaryAutoPopulate;

		[DataMember]
		protected string _ExportArchiveDir;

		[DataMember]
		protected string _ImportArchiveDir;

		[DataMember]
		protected bool _GroupLedgerByID;

		[DataMember]
		protected bool _VRURateLimitEnabled;

		[DataMember]
		protected bool _VRUHourlyLimitEnabled;

		[DataMember]
		protected bool _VRUDailyLimitEnabled;

		[DataMember]
		protected bool _VRUYearlyLimitEnabled;

		[DataMember]
		protected bool _VRUCurrentYearLimitEnabled;

		[DataMember]
		protected WATCHDOG_MODE _WatchdogMode;

		// Regional Settings
		[DataMember]
		protected NUMBER_GROUP_SIZES_TYPE _NumberGroupSizesType = NUMBER_GROUP_SIZES_TYPE.THREE;

		[DataMember]
		protected string _NumberDecimalSeparator = ".";

		[DataMember]
		protected string _NumberGroupSeparator = ",";

		[DataMember]
		protected string _ListSeparator = ",";

		[DataMember]
		protected string _TimePattern = "hh:mm:ss tt";

		[DataMember]
		protected string _TimeSeparator = ":";

		[DataMember]
		protected string _AMSymbol = "AM";

		[DataMember]
		protected string _PMSymbol = "PM";

		[DataMember]
		protected string _ShortDatePattern = "M/D/yyyy";

		[DataMember]
		protected string _DateSeparator = "/";

		[DataMember]
		protected string _LongDatePattern = "ddddd, MMMMM dd, yyyy";

		[DataMember]
		protected int _TwoDigitCalendarEndYear = 2029;

		// Password configuration data members
		[DataMember]
		protected int minTimeAllowedToChangePwd;

		[DataMember]
		protected int minPwdCharacterLength;

		[DataMember]
		protected int pwdExpirationInDays;

		[DataMember]
		protected int pwdLockoutThreshold;

		[DataMember]
		protected int pwdHistoryCount;

		[DataMember]
		protected bool checkForPreviousPwd;

		[DataMember]
		protected int StrongPwdUse;

		[DataMember]
		protected bool applyToAllSiteMembers;

		[DataMember]
		protected int inactivityDisablePeriod;

		[DataMember]
		protected int disableArchivePeriod;

		[DataMember]
		protected bool _BlockCloseOnUnpostedBol;

		[DataMember]
		protected bool useTankReconciliation;

		  //Enterprise Query Credentials - System Tab
		 [DataMember]
		 protected string _EnterpriseUserId;

		 [DataMember]
		 protected string _EnterprisePassword;

		 [DataMember]
		 protected string _EnterpriseSite;

		[DataMember]
		protected bool _EnforceSalesOrderLimit;

		[DataMember]
		protected int _LeakDetectionQuietSamples;

		[DataMember]
		protected int _LeakDetectionQuietTime;

		[DataMember]
		protected int _LeakDetectionQuietTimeFactor;

		[DataMember]
		protected bool _LeakDetectionUseMinWait;

		[DataMember]
		protected string _LeakDetectionReport;

		[DataMember]
		protected string _LeakDetectionPrinter;

		[DataMember]
		protected string _PointGroupFileExportDirectory;

		[DataMember]
		protected string _PointGroupDefaultFileName;

		[DataMember]
		protected bool _EnableMovementTicketPDFArchiving;

		[DataMember]
		protected string _MovementTicketFileExportDirectory;

		[DataMember]
		protected string _MovementTicketExportFileName;
		#endregion

		#region Private data members
		[DataMember]
		private bool _MeterReconciliationToleranceIsPercent;

		[DataMember]
		private string _MeterReconciliationReportName = string.Empty;

		[DataMember]
		private string _TranslatedHelpURL = string.Empty;

		[DataMember]
		private bool _Enterprise;

		[DataMember]
		private bool _OperateTabGroups;

		[DataMember]
		private string serverEndPoint;

		[DataMember]
		private string securityMode;

		[DataMember]
		public string securityPolicy;

		[DataMember]
		private string messageEncoding;

		[DataMember]
		private string userIdentityMethod;

		[DataMember]
		private string userId;

		[DataMember]
		private string userPassword;

		[DataMember]
		public string userCertificatePath;

		[DataMember]
		public int maximumDaysToRetainArchive;

		[DataMember]
		protected bool _EnableAutomaticMovementTicketPrinting;

		[DataMember]
		private string _MovementTicketReport = string.Empty;

		[DataMember]
		private string _MovementTicketPrinter = string.Empty;

		[DataMember]
		private int _MaxOperateTabsAllowed;

      [DataMember]
      private TimeSpan? _CloseoutTime = null;

      private const string ManualEndOfDayKey = "Manually Initiated End Of Day";
		private const string AutomaticEndOfDayKey = "Automatically Initiated End Of Day";
		private const string AutomaticEndOfMonthKey = "Automatically Initiated End Of Month";
		private const string UseLastKnownGoodTankDataKey = "Use Last Known Good Tank Data";
		private const string UseCurrentTankDataKey = "Use Current Tank Data";
		private const string EndOfDayProcessingBeginKey = "End Of Day Processing Commenced";
		private const string EndOfMonthProcessingBeginKey = "End Of Month Processing Commenced";
		private const string EndOfDayProcessingEndKey = "End Of Day Processing Complete";
		private const string EndOfMonthProcessingEndKey = "End Of Month Processing Complete";
		 private const string ReloadTanksKey = "Reloading tank definitions";
		private const string EodFailedPhysicalInvCreateKey = "End Of Day Physical Inventory Create Failed";
		private const string EomFailedPhysicalInvCreateKey = "End Of Month Physical Inventory Create Failed";
		private const string TankIgnoredPhysicalInvCreateKey = "Physical Inventory Create Tank Skipped";
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="SiteClass"/> class.
		/// </summary>
		public SiteClass( )
		{
			this.Initialize( );
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="SiteClass"/> class.
		/// </summary>
		/// <param name="noReset">
		/// The no reset.
		/// </param>
		public SiteClass(bool noReset)
		{
			if ( noReset == false )
			{
				this.Initialize( );
			}
		}
		#endregion

		#region Properties
		// General
		public override string ID { get { return this._ID; } set {
			this.SetString("Name", 30, value, ref this._ID); } }
		public override ENTITY_TYPE EntityType { get { return ENTITY_TYPE.SITE; } }

		public string Number { get { return this._Number; } set {
			this.SetString("Number", 30, value, ref this._Number); } }
		public string SPLCCode { get { return this._SPLCCode; } set {
			this.SetString("SPLCCode", 30, value, ref this._SPLCCode); } }
		public string Address1 { get { return this._Address1; } set {
			this.SetString("Address1", 30, value, ref this._Address1); } }
		public string Address2 { get { return this._Address2; } set {
			this.SetString("Address2", 30, value, ref this._Address2); } }
		public string City { get { return this._City; } set {
			this.SetString("City", 60, value, ref this._City); } }
		public string State { get { return this._State; } set {
			this.SetString("State", 20, value, ref this._State); } }
		public string Zip { get { return this._Zip; } set {
			this.SetString("Zip", 11, value, ref this._Zip); } }
		public string Country { get { return this._Country; } set {
			this.SetString("Country", 30, value, ref this._Country); } }
		public string Phone { get { return this._Phone; } set {
			this.SetString("Phone", 20, value, ref this._Phone); } }
		public string Fax { get { return this._Fax; } set {
			this.SetString("Fax", 20, value, ref this._Fax); } }
		public string EmailAddress { get { return this._EmailAddress; } set {
			this.SetString("Email Address", 30, value, ref this._EmailAddress); } }
		public string EmergencyContact { get { return this._EmergencyContact; } set {
			this.SetString("Emergency Contact", 30, value, ref this._EmergencyContact); } }
		public string EmergencyPhone { get { return this._EmergencyPhone; } set {
			this.SetString("Emergency Phone", 20, value, ref this._EmergencyPhone); } }
		public bool Enabled { get { return this._Enabled; } set {
			this._Enabled = value; } }
		public bool SiteGroup { get { return this._SiteGroup; } set {
			this._SiteGroup = value; } }
		public string TimeZone { get { return this._TimeZone; } set {
			this.SetString("Time Zone", 50, value, ref this._TimeZone); } }
		public string TerminalControlNumber { get { return this._TerminalControlNumber;} set {
				this.SetString("Terminal Control Number", 9, value, ref this._TerminalControlNumber); }}
		public bool InhibitLoadRackCardIns { get { return this._InhibitLoadRackCardIns;} set {
					this._InhibitLoadRackCardIns = value; }}
		public bool EnforceSingleOwner { get { return this._EnforceSingleOwner; } set { this._EnforceSingleOwner = value; } }
		public bool InhibitBOLSummaryAutoPopulate { get { return this._InhibitBOLSummaryAutoPopulate; } set {
			this._InhibitBOLSummaryAutoPopulate = value; } }
		public bool InhibitOrderSummaryAutoPopulate { get { return this._InhibitOrderSummaryAutoPopulate; } set {
			this._InhibitOrderSummaryAutoPopulate = value; } }
		public bool InhibitSupplyOrderSummaryAutoPopulate { get { return this._InhibitSupplyOrderSummaryAutoPopulate; } set {
			this._InhibitSupplyOrderSummaryAutoPopulate = value; } }
		public Guid IATAGuid { get { return this._IATAGuid; } set {
			this._IATAGuid = value; } }
		public string Contact1Name { get { return this._Contact1Name; } set {
			this.SetString("Contact 1 Name", 30, value, ref this._Contact1Name); } }
		public string Contact1Address1 { get { return this._Contact1Address1; } set {
			this.SetString("Contact 1 Address 1", 30, value, ref this._Contact1Address1); } }
		public string Contact1Address2 { get { return this._Contact1Address2; } set {
			this.SetString("Contact 1 Address 2", 30, value, ref this._Contact1Address2); } }
		public string Contact1City { get { return this._Contact1City; } set {
			this.SetString("Contact 1 City", 60, value, ref this._Contact1City); } }
		public string Contact1State { get { return this._Contact1State; } set {
			this.SetString("Contact 1 State", 20, value, ref this._Contact1State); } }
		public string Contact1Zip { get { return this._Contact1Zip; } set {
			this.SetString("Contact 1 Zip", 11, value, ref this._Contact1Zip); } }
		public string Contact1Country { get { return this._Contact1Country; } set {
			this.SetString("Contact 1 Country", 30, value, ref this._Contact1Country); } }
		public string Contact1PhoneOffice { get { return this._Contact1PhoneOffice; } set {
			this.SetString("Contact 1 Phone Office", 20, value, ref this._Contact1PhoneOffice); } }
		public string Contact1PhoneMobile { get { return this._Contact1PhoneMobile; } set {
			this.SetString("Contact 1 Phone Mobile", 20, value, ref this._Contact1PhoneMobile); } }
		public string Contact1Fax { get { return this._Contact1Fax; } set {
			this.SetString("Contact 1 Fax", 20, value, ref this._Contact1Fax); } }
		public string Contact1EmailAddress { get { return this._Contact1EmailAddress; } set {
			this.SetString("Contact 1 Email", 30, value, ref this._Contact1EmailAddress); } }
		public string Contact2Name { get { return this._Contact2Name; } set {
			this.SetString("Contact 2 Name", 30, value, ref this._Contact2Name); } }
		public string Contact2Address1 { get { return this._Contact2Address1; } set {
			this.SetString("Contact 2 Address 1", 30, value, ref this._Contact2Address1); } }
		public string Contact2Address2 { get { return this._Contact2Address2; } set {
			this.SetString("Contact 2 Address 2", 30, value, ref this._Contact2Address2); } }
		public string Contact2City { get { return this._Contact2City; } set {
			this.SetString("Contact 2 City", 60, value, ref this._Contact2City); } }
		public string Contact2State { get { return this._Contact2State; } set {
			this.SetString("Contact 2 State", 20, value, ref this._Contact2State); } }
		public string Contact2Zip { get { return this._Contact2Zip; } set {
			this.SetString("Contact 2 Zip", 11, value, ref this._Contact2Zip); } }
		public string Contact2Country { get { return this._Contact2Country; } set {
			this.SetString("Contact 2 Country", 30, value, ref this._Contact2Country); } }
		public string Contact2PhoneOffice { get { return this._Contact2PhoneOffice; } set {
			this.SetString("Contact 2 Phone Office", 20, value, ref this._Contact2PhoneOffice); } }
		public string Contact2PhoneMobile { get { return this._Contact2PhoneMobile; } set {
			this.SetString("Contact 2 Phone Mobile", 20, value, ref this._Contact2PhoneMobile); } }
		public string Contact2Fax { get { return this._Contact2Fax; } set {
			this.SetString("Contact 2 Fax", 20, value, ref this._Contact2Fax); } }
		public string Contact2EmailAddress { get { return this._Contact2EmailAddress; } set {
			this.SetString("Contact 2 Email", 30, value, ref this._Contact2EmailAddress); } }

		public double? Latitude
		{
			get { return this.latitude; }
			set { this.latitude = value; }
		}

		public double? Longitude
		{
			get { return this.longitude; }
			set { this.longitude = value; }
		}

		public int? Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		 public Guid ActiveDirectorySiteGroupGuid
		 {
			  get { return this.activeDirectorySiteGroupGuid; }
			  set { this.activeDirectorySiteGroupGuid = value; }
		 }

		public string LatitudeStr
		{
			get
			{
				if (this.latitude == null)
				{
					return string.Empty;
				}

				return this.latitude.ToString();
			}
			set
			{
				this.latitude = null;

				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				double latitudeOut;

				if (double.TryParse(value, out latitudeOut) == false)
				{
					throw new Exception("[Latitude] Must be numeric.");
				}

				if (latitudeOut < -90.0 || latitudeOut > 90.0)
				{
					throw new Exception("[Latitude] Must be between -90.0 and 90.0");
				}

				this.latitude = latitudeOut;
			}
		}

		public string LongitudeStr
		{
			get
			{
				if (this.longitude == null)
				{
					return string.Empty;
				}

				return this.longitude.ToString();
			}
			set
			{
				this.longitude = null;

				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				double longitudeOut;

				if (double.TryParse(value, out longitudeOut) == false)
				{
					throw new Exception("[Longitude] Must be numeric.");
				}

				if (longitudeOut < -180.0 || longitudeOut > 180.0)
				{
					throw new Exception("[Longitude] Must be between -180.0 and 180.0");
				}

				this.longitude = longitudeOut;
			}
		}

		public string ZoomStr
		{
			get
			{
				if (this.zoom == null)
				{
					return string.Empty;
				}

				return this.zoom.ToString();
			}
			set
			{
				this.zoom = null;

				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				int zoomOut;

				if (int.TryParse(value, out zoomOut) == false)
				{
					throw new Exception("[Zoom] Must be numeric.");
				}

				if (zoomOut < 0 || zoomOut > 25)
				{
					throw new Exception("[Zoom] Must be between -0 and 25");
				}

				this.zoom = zoomOut;
			}
		}

		// Units
		public EngineeringUnit LevelUnits { get { return this._LevelUnits; } set {
			this._LevelUnits = value; } }
		public EngineeringUnit TemperatureUnits { get { return this._TemperatureUnits; } set {
			this._TemperatureUnits = value; } }
		public EngineeringUnit DensityUnits { get { return this._DensityUnits; } set {
			this._DensityUnits = value; } }
		public EngineeringUnit PressureUnits { get { return this._PressureUnits; } set {
			this._PressureUnits = value; } }
		public EngineeringUnit FlowUnits { get { return this._FlowUnits; } set {
			this._FlowUnits = value; } }
		public EngineeringUnit VolumeUnits { get { return this._VolumeUnits; } set {
			this._VolumeUnits = value; } }
		public EngineeringUnit MassUnits { get { return this._MassUnits; } set {
			this._MassUnits = value; } }
		public EngineeringUnit AdditiveVolumeUnits { get { return this._AdditiveVolumeUnits; } set {
			this._AdditiveVolumeUnits = value; } }
		public EngineeringUnit AdditiveProfileCycleAmountUnits { get { return this._AdditiveProfileCycleAmountUnits; } set {
			this._AdditiveProfileCycleAmountUnits = value; } }
		public EngineeringUnit AdditiveProfileRateUnits { get { return this._AdditiveProfileRateUnits; } set {
			this._AdditiveProfileRateUnits = value; } }

		/// <summary>
		/// This property will return true if the site is excluded from
		/// rolling up in the ledger.  Otherwise, it returns false indicating
		/// that the site will be in the ledger rollup.
		/// </summary>
		public bool InhibitSiteLedgerRollup
		{
			get { return this._inhibitSiteLedgerRollup; }
			set { this._inhibitSiteLedgerRollup = value; }
		}

		public string LevelDecimalPlaces
		{
			get { return this._LevelDecimalPlaces.ToString(); }
			set {
				this.SetByte("Level Decimal Places", value, ref this._LevelDecimalPlaces); }
		}

		public string TemperatureDecimalPlaces
		{
			get { return this._TemperatureDecimalPlaces.ToString(); }
			set {
				this.SetByte("Temperature Decimal Places", value, ref this._TemperatureDecimalPlaces); }
		}

		public string DensityDecimalPlaces
		{
			get { return this._DensityDecimalPlaces.ToString(); }
			set {
				this.SetByte("Density Decimal Places", value, ref this._DensityDecimalPlaces); }
		}

		public string PressureDecimalPlaces
		{
			get { return this._PressureDecimalPlaces.ToString(); }
			set {
				this.SetByte("Pressure Decimal Places", value, ref this._PressureDecimalPlaces); }
		}

		public string FlowDecimalPlaces
		{
			get { return this._FlowDecimalPlaces.ToString(); }
			set {
				this.SetByte("Flow Decimal Places", value, ref this._FlowDecimalPlaces); }
		}

		public string VolumeDecimalPlaces
		{
			get { return this._VolumeDecimalPlaces.ToString(); }
			set {
				this.SetByte("Volume Decimal Places", value, ref this._VolumeDecimalPlaces); }
		}

		public string MassDecimalPlaces
		{
			get { return this._MassDecimalPlaces.ToString(); }
			set {
				this.SetByte("Mass Decimal Places", value, ref this._MassDecimalPlaces); }
		}

		public string AdditiveVolumeDecimalPlaces
		{
			get { return this._AdditiveVolumeDecimalPlaces.ToString(); }
			set {
				this.SetByte("Additive Volume Decimal Places", value, ref this._AdditiveVolumeDecimalPlaces); }
		}

		public string AdditiveProfileCycleAmountDecimalPlaces
		{
			get { return this._AdditiveProfileCycleAmountDecimalPlaces.ToString(); }
			set {
				this.SetByte("Additive Profile Cycle Amount Decimal Places", value, ref this._AdditiveProfileCycleAmountDecimalPlaces); }
		}

		public string AdditiveProfileRateDecimalPlaces
		{
			get { return this._AdditiveProfileRateDecimalPlaces.ToString(); }
			set {
				this.SetByte("Additive Profile Rate Decimal Places", value, ref this._AdditiveProfileRateDecimalPlaces); }
		}

		public QuantityDisplay QuantityDisplayDefault
		{
			get { return this._QuantityDisplayDefault; }
			set { this._QuantityDisplayDefault = value; }
		}

		public bool EnablePasswordHint
		{
			get { return this._EnablePasswordHint; }
			set { this._EnablePasswordHint = value; }
		}

		public bool EnablePasswordReset
		{
			get { return this._EnablePasswordReset; }
			set { this._EnablePasswordReset = value; }
		}

		public bool AllowUseOfSpecialChars
		{
			get { return this._AllowUseOfSpecialChars; }
			set { this._AllowUseOfSpecialChars = value; }
		}

		public bool EnablePeriodicSyncFlag
		{
				get { return this._EnablePeriodicSyncFlag; }
				set { this._EnablePeriodicSyncFlag = value; }
		}

		public int PeriodicSyncIntervalMinutes
		{
				get { return this._PeriodicSyncIntervalMinutes; }
				set { this._PeriodicSyncIntervalMinutes = value; }
		}

		public bool DisableSyncTransferFlag
		{
			get { return this._DisableSyncTransferFlag; }
			set { this._DisableSyncTransferFlag = value; }
		}

		public bool Enterprise
		{
			get { return this._Enterprise; }
			set { this._Enterprise = value; }
		}

		public bool OperateTabGroups
		{
			get { return this._OperateTabGroups; }
			set { this._OperateTabGroups = value; }
		}

		// Load Rack
		public bool InhibitAccessAfterHours { get { return this._InhibitAccessAfterHours; } set {
			this._InhibitAccessAfterHours = value; } }
		public bool InhibitMultipleCardIns { get { return this._InhibitMultipleCardIns; } set {
			this._InhibitMultipleCardIns = value; } }
		public bool AccessCardInRequired { get { return this._AccessCardInRequired; } set {
			this._AccessCardInRequired = value; } }
		public bool CheckSiteNumber { get { return this._CheckSiteNumber; } set {
			this._CheckSiteNumber = value; } }
		public bool PromptForCustomerCard { get { return this._PromptForCustomerCard; } set {
			this._PromptForCustomerCard = value; } }
		public bool PromptForTractorOrTanker { get { return this._PromptForTractorOrTanker; } set {
			this._PromptForTractorOrTanker = value; } }
		public bool PromptForFirstTrailer { get { return this._PromptForFirstTrailer; } set {
			this._PromptForFirstTrailer = value; } }
		public bool PromptForSecondTrailer { get { return this._PromptForSecondTrailer; } set {
			this._PromptForSecondTrailer = value; } }
		public bool PromptForThirdTrailer { get { return this._PromptForThirdTrailer; } set {
				this._PromptForThirdTrailer = value; } }
		public bool PromptForCompartment { get { return this._PromptForCompartment; } set {
			this._PromptForCompartment = value; } }
		public bool PromptForTransactionCompletion { get { return this._PromptForTransactionCompletion; } set {
				this._PromptForTransactionCompletion = value; } }
		public bool InhibitCustomerConfirmationPrompt { get {return this._InhibitCustomerConfirmationPrompt; } set {
				this._InhibitCustomerConfirmationPrompt = value; } }
		public bool RequireTrailerScully { get { return this._RequireTrailerScully; } set {
				this._RequireTrailerScully = value; } }
		public string CardInTimeout { get { return this._CardInTimeout.ToString(CultureInfo.InvariantCulture); } set {
				this.SetInt("Card In Timeout", value, ref this._CardInTimeout); } }
		public bool EnforceDriverEquipmentMatch { get { return this._EnforceDriverEquipmentMatch; } set {
			this._EnforceDriverEquipmentMatch = value; } }
		public bool EnableAdditiveAccounting { get { return this._EnableAdditiveAccounting; } set {
			this._EnableAdditiveAccounting = value; } }
		public bool UseCompanyEquipmentIdentifiers { get { return this._UseCompanyEquipmentIdentifiers; } set {
			this._UseCompanyEquipmentIdentifiers = value; } }
		public bool UseLastKnownGoodTankData { get { return this._UseLastKnownGoodTankData; } set {
			this._UseLastKnownGoodTankData = value; } }

		public string MaximumLoadTime { get { return this._MaximumLoadTime.ToString(); } set {
			this.SetInt("Maximum Load Time", value, ref this._MaximumLoadTime); } }
		public string MaximumIdleTime { get { return this._MaximumIdleTime.ToString(); } set {
			this.SetInt("Maximum Idle Time", value, ref this._MaximumIdleTime); } }

		public string MaximumLoadAmount { get { return this._MaximumLoadAmount.ToString(); } set {
			this.SetSIDouble("Maximum Load Amount", value, ref this._MaximumLoadAmount); } }
		public string MaximumFlushAmount { get { return this._MaximumFlushAmount.ToString(); } set {
			this.SetSIDouble("Maximum Flush Amount", value, ref this._MaximumFlushAmount); } }
		public string MaximumMeterProvingAmount { get { return this._MaximumMeterProvingAmount.ToString(); } set {
			this.SetSIDouble("Maximum Meter Proving Amount", value, ref this._MaximumMeterProvingAmount); } }
		public string MaximumReturnsAmount { get { return this._MaximumReturnsAmount.ToString(); } set {
			this.SetSIDouble("Maximum Returns Amount", value, ref this._MaximumReturnsAmount); } }
		public string MaximumVehicleWeight { get { return this._MaximumVehicleWeight.ToString(); } set {
			this.SetSIDouble("Maximum Vehicle Weight", value, ref this._MaximumVehicleWeight); } }
		public string MaximumProductTemperature { get { return this._MaximumProductTemperature.ToString(); } set {
			this.SetSIDouble("Maximum Product Temperature", value, ref this._MaximumProductTemperature); } }

		public string MaximumNumberOfActiveArms { get { return this._MaximumNumberOfActiveArms.ToString(); } set {
			this.SetInt("Maximum Number Of Active Arms", value, ref this._MaximumNumberOfActiveArms); } }
		public string DriverTimeoutPeriod { get { return this._DriverTimeoutPeriod.ToString(); } set {
			this.SetInt("Driver Timeout Period", value, ref this._DriverTimeoutPeriod); } }
		public string DriverWarningPeriod { get { return this._DriverWarningPeriod.ToString(); } set {
			this.SetInt("Driver Warning Period", value, ref this._DriverWarningPeriod); } }
		public string MaximumPrompts { get { return this._MaximumPrompts.ToString(); } set {
			this.SetInt("Maximum Prompts", value, ref this._MaximumPrompts); } }
		public Guid InventoryTransactionAliasGuid { get { return this._InventoryTransactionAliasGuid; } set {
			this._InventoryTransactionAliasGuid = value; } }
		public Guid AdjustmentTransactionAliasGuid { get { return this._AdjustmentTransactionAliasGuid; } set {
			this._AdjustmentTransactionAliasGuid = value; } }
		public bool LoadByNet { get { return this._LoadByNet; } set {
			this._LoadByNet = value; } }
		public bool PromptForShipmentNumber { get { return this._PromptForShipmentNumber; } set {
			this._PromptForShipmentNumber = value; } }
		public bool ListEquipment { get { return this._ListEquipment; } set {
			this._ListEquipment = value; } }
		public bool DeferStationChanges { get { return this._DeferStationChanges; } set {
			this._DeferStationChanges = value; } }
		public bool PromptForReturns { get { return this._PromptForReturns; } set {
			this._PromptForReturns = value; } }
		public bool PromptForTruckCard { get { return this._PromptForTruckCard; } set {
			this._PromptForTruckCard = value; } }
		public string StartingShortCardNumber { get { return this._StartingShortCardNumber.ToString(); } set {
			this.SetInt("Starting Short CardNumber", value, ref this._StartingShortCardNumber); } }
		public bool UseShortCardNumber { get { return this._UseShortCardNumber; } set {
			this._UseShortCardNumber = value; } }
		public string ExcessVarianceCount { get { return this._ExcessVarianceCount.ToString(); } set {
			this.SetByte("Variance Count", value, ref this._ExcessVarianceCount); } }
		public string ExcessVarianceTolerance { get { return this._ExcessVarianceTolerance.ToString(); } set {
			this.SetDouble("Variance Tolerance", value, ref this._ExcessVarianceTolerance); } }
		public FILL_METHOD SecondaryStorageFillMethod { get { return this._SecondaryStorageFillMethod; } set {
			this._SecondaryStorageFillMethod = value; } }

		// Transactions
		public bool InhibitBOLWithBrokenBlends { get { return this._InhibitBOLWithBrokenBlends; } set {
			this._InhibitBOLWithBrokenBlends = value; } }
		public bool InhibitBOLWithImproperAdditization { get { return this._InhibitBOLWithImproperAdditization; } set {
			this._InhibitBOLWithImproperAdditization = value; } }
		public bool InhibitOverweightBOL { get { return this._InhibitOverweightBOL; } set {
			this._InhibitOverweightBOL = value; } }
		public string ExceptionBOLPrinter { get { return this._ExceptionBOLPrinter; } set {
			this.SetString("Exception BOL Printer", 80, value, ref this._ExceptionBOLPrinter); } }
		public bool EnableAutomaticBOLPrinting { get { return this._EnableAutomaticBOLPrinting; } set {
			this._EnableAutomaticBOLPrinting = value; } }
		public string AutomaticBOLStartNumber { get { return this._AutomaticBOLStartNumber.ToString(); } set {
			this.SetInt("Automatic BOL Start Number", value, ref this._AutomaticBOLStartNumber); } }
		public string AutomaticBOLEndNumber { get { return this._AutomaticBOLEndNumber.ToString(); } set {
			this.SetInt("Automatic BOL End Number", value, ref this._AutomaticBOLEndNumber); } }
		public string AutomaticBOLNextNumber { get { return this._AutomaticBOLNextNumber.ToString(); } set {
			this.SetInt("Automatic BOL Next Number", value, ref this._AutomaticBOLNextNumber); } }
		public bool SeparateManualBOLNumbering { get { return this._SeparateManualBOLNumbering; } set {
			this._SeparateManualBOLNumbering = value; } }
		public string ManualBOLStartNumber { get { return this._ManualBOLStartNumber.ToString(); } set {
			this.SetInt("Manual BOL Start Number", value, ref this._ManualBOLStartNumber); } }
		public string ManualBOLEndNumber { get { return this._ManualBOLEndNumber.ToString(); } set {
			this.SetInt("Manual BOL End Number", value, ref this._ManualBOLEndNumber); } }
		public string ManualBOLNextNumber { get { return this._ManualBOLNextNumber.ToString(); } set {
			this.SetInt("Manual BOL Next Number", value, ref this._ManualBOLNextNumber); } }
		public bool EnableBOLPDFArchiving { get { return this._EnableBOLPDFArchiving; } set {
				this._EnableBOLPDFArchiving = value; } }
		public string BOLPDFArchivingPath { get { return this._BOLPDFArchivingPath; } set {
				this._BOLPDFArchivingPath = value; } }
		public string TransactionStartNumber { get { return this._TransactionStartNumber.ToString(); } set {
			this.SetInt("Transaction Start Number", value, ref this._TransactionStartNumber); } }
		public string TransactionEndNumber { get { return this._TransactionEndNumber.ToString(); } set {
			this.SetInt("Transaction End Number", value, ref this._TransactionEndNumber); } }
		public string TransactionNextNumber { get { return this._TransactionNextNumber.ToString(); } set {
			this.SetInt("Transaction Next Number", value, ref this._TransactionNextNumber); } }
		public string OrderStartNumber { get { return this._OrderStartNumber.ToString(); } set {
			this.SetInt("Order Start Number", value, ref this._OrderStartNumber); } }
		public string OrderEndNumber { get { return this._OrderEndNumber.ToString(); } set {
			this.SetInt("Order End Number", value, ref this._OrderEndNumber); } }
		public string OrderNextNumber { get { return this._OrderNextNumber.ToString(); } set {
			this.SetInt("Order Next Number", value, ref this._OrderNextNumber); } }
		// vt 07-15-2008
		public string InvoiceStartNumber
		{
			get { return this._InvoiceStartNumber.ToString(); }
			set {
				this.SetInt("Invoice Start Number", value, ref this._InvoiceStartNumber); }
		}

		public string InvoiceEndNumber
		{
			get { return this._InvoiceEndNumber.ToString(); }
			set {
				this.SetInt("Invoice End Number", value, ref this._InvoiceEndNumber); }
		}

		public string InvoiceNextNumber
		{
			get { return this._InvoiceNextNumber.ToString(); }
			set {
				this.SetInt("Invoice Next Number", value, ref this._InvoiceNextNumber); }
		}

		public string NumberPrefix { get { return this._NumberPrefix; } set {
			this.SetString("Number Prefix", 10, value, ref this._NumberPrefix); } }

		public string OpenTransactionWindow { get { return this._OpenTransactionWindow.ToString(); } set {
			this.SetInt("OpenTransactionWindow", value, ref this._OpenTransactionWindow); } }

		public string AdministrativeLockDate
		{
			get
			{
				return this._AdministrativeLockDate.ToString();
			}
			set
			{
				this.SetDate("Administrative Lock Date", value, ref this._AdministrativeLockDate);
				if (this._AdministrativeLockDate.IsTodayOrAfter)
					throw new Exception("Lock Date must be prior to current date");
			}
		}

		public string OperationalLockDate
		{
			get
			{
				string opLockDate = this._OperationalLockDate.Value.ToString();
				int delimiterIndex = opLockDate.IndexOf(" ");

				if (delimiterIndex >= 0)
				{
					string shortDatePattern		= this.GetDateTimeFormatInfo().ShortDatePattern;
					string formattedShortDate	= this._OperationalLockDate.Value.ToString(shortDatePattern);
					string timeComponent		= opLockDate.Substring(delimiterIndex + 1);
					opLockDate					= formattedShortDate + " " + timeComponent;
				}

				return opLockDate;
			}
			set
			{
				this.SetDateAndTime("Operational Lock Date", value, ref this._OperationalLockDate);
			}
		}

		// System
		public string MaximumDaysToRetainLogs { get { return this._MaximumDaysToRetainLogs.ToString(); } set {
			this.SetInt("Maximum Days To Retain Logs", value, ref this._MaximumDaysToRetainLogs); } }
		public bool EnableDebugLogging { get { return this._EnableDebugLogging; } set {
			this._EnableDebugLogging = value; } }
		public bool EnableAuditLogging { get { return this._EnableAuditLogging; } set {
			this._EnableAuditLogging = value; } }
		public bool AutomaticallyPrintAlarmsAndEvents { get { return this._AutomaticallyPrintAlarmsAndEvents; } set {
			this._AutomaticallyPrintAlarmsAndEvents = value; } }
		public string AlarmAndEventPrinter { get { return this._AlarmAndEventPrinter; } set {
			this.SetString("Alarm And Event Printer", 80, value, ref this._AlarmAndEventPrinter); } }
		public string MailServer { get { return this._MailServer; } set {
			this.SetString("Mail Server", 50, value, ref this._MailServer); } }
		public string MailFrom { get { return this._MailFrom; } set {
			this.SetString("Mail From", 50, value, ref this._MailFrom); } }
		public string MailUserName { get { return this._MailUserName; } set {
			this.SetString("Mail User Name", 50, value, ref this._MailUserName); } }
		public string MailPassword { get { return this._MailPassword; } set {
			this.SetString("Mail Password", 50, value, ref this._MailPassword); } }
		public MAIL_SERVER_CONNECT_MODE MailConnectMode { get { return this._MailConnectMode; } set {
			this._MailConnectMode = value; } }
		public string DialupName { get { return this._DialupName; } set {
			this.SetString("Dialup Name", 50, value, ref this._DialupName); } }
		public string SCADASystem { get { return this._SCADASystem; } set {
			this.SetString("SCADA System", 50, value, ref this._SCADASystem); } }
		public bool InhibitTemplateGraphics { get { return this._InhibitTemplateGraphics; } set {
			this._InhibitTemplateGraphics = value; } }
		public string RefreshInterval { get { return this._RefreshInterval.ToString(); } set {
			this.SetInt("Refresh Interval", value, ref this._RefreshInterval); } }
		public bool InhibitEndOfDayOperations { get { return this._InhibitEndOfDayOperations; } set {
			this._InhibitEndOfDayOperations = value; } }
		public bool InhibitEndOfMonthOperations { get { return this._InhibitEndOfMonthOperations; } set {
			this._InhibitEndOfMonthOperations = value; } }
		public string EndOfDayWarningPeriod { get { return this._EndOfDayWarningPeriod.ToString(); } set {
			this.SetInt("End Of Day Warning Period", value, ref this._EndOfDayWarningPeriod); } }
		public bool InhibitAutomaticPhysicalInventory { get { return this._InhibitAutomaticPhysicalInventory; } set {
			this._InhibitAutomaticPhysicalInventory = value; } }
		public bool InhibitAutomaticMeterCloseout { get { return this._InhibitAutomaticMeterCloseout; } set {
			this._InhibitAutomaticMeterCloseout = value; } }
		public bool InhibitAutomaticReportGeneration { get { return this._InhibitAutomaticReportGeneration; } set {
			this._InhibitAutomaticReportGeneration = value; } }
		public bool InhibitAutomaticAdjustmentDistribution { get { return this._InhibitAutomaticAdjustmentDistribution; } set {
			this._InhibitAutomaticAdjustmentDistribution = value; } }
		public bool InhibitAutomaticCloseout { get { return this._InhibitAutomaticCloseout; } set {
			this._InhibitAutomaticCloseout = value; } }
		public bool BlockCloseOnUnpostedBol { get { return this._BlockCloseOnUnpostedBol; } set {
				this._BlockCloseOnUnpostedBol = value; } }
		public bool InhibitTankScan { get { return this._InhibitTankScan; } set {
			this._InhibitTankScan = value; } }
		public string ReportDirectory { get { return this._ReportDirectory; } set {
			this.SetString("Report Directory", 80, value, ref this._ReportDirectory); } }
		public bool ManageReports { get { return this._ManageReports; } set {
			this._ManageReports = value; } }
		public string ManagedReportDirectory { get { return this._ManagedReportDirectory; } set {
			this.SetString("Managed Report Directory", 80, value, ref this._ManagedReportDirectory); } }
		public string ExportArchiveDir { get { return this._ExportArchiveDir; } set {
			this.SetString("Export Archive Directory", 255, value, ref this._ExportArchiveDir); } }
		public string ImportArchiveDir { get { return this._ImportArchiveDir; } set {
			this.SetString("Import Archive Directory", 255, value, ref this._ImportArchiveDir); } }
		public bool GroupLedgerByID { get { return this._GroupLedgerByID; } set {
			this._GroupLedgerByID = value; } }
		public bool MeterReconciliationToleranceIsPercent { get { return this._MeterReconciliationToleranceIsPercent; } set {
			this._MeterReconciliationToleranceIsPercent = value; } }
		public string MeterReconciliationReportName { get { return this._MeterReconciliationReportName; } set {
			this._MeterReconciliationReportName = value; } }
		public string TranslatedHelpURL { get { return this._TranslatedHelpURL; } set {
			this._TranslatedHelpURL = value; } }
		public bool EnableAutomaticMovementTicketPrinting { get { return this._EnableAutomaticMovementTicketPrinting; } set {
			this._EnableAutomaticMovementTicketPrinting = value;}}
		public string MovementTicketReportName { get { return this._MovementTicketReport; } set {
			this._MovementTicketReport = value; } }
		public string MovementTicketPrinter { get { return this._MovementTicketPrinter; } set {
			this._MovementTicketPrinter = value; } }
		public int MaxOperateTabsAllowed	{ get	{ return _MaxOperateTabsAllowed;} set {
				this._MaxOperateTabsAllowed = value; }}

		public TimeSpan? CloseoutTime
		{
			get
			{
				return _CloseoutTime;
			}
			set
			{
				this._CloseoutTime = value;
			}
		}

		public string PointGroupFileExportDirectory
		{
			get { return this._PointGroupFileExportDirectory; }
			set
			{
				this.SetString("Point Group File Export Directory", 255, value, ref this._PointGroupFileExportDirectory);
			}
		}
		
		public string PointGroupDefaultFileName
		{
			get { return this._PointGroupDefaultFileName; }
			set
			{
				// strip out any file extensions
				this.SetString("Point Group Export File Name", 255, System.IO.Path.GetFileNameWithoutExtension(value), ref this._PointGroupDefaultFileName);
			}
		}
		public bool EnableMovementTicketPDFArchiving
		{
			get { return this._EnableMovementTicketPDFArchiving; }
			set
			{
				this._EnableMovementTicketPDFArchiving = value;
			}
		}

		public string MovementTicketFileExportDirectory
		{
			get { return this._MovementTicketFileExportDirectory; }
			set
			{
				this.SetString("Movement Ticket File Export Directory", 255, value, ref this._MovementTicketFileExportDirectory);
			}
		}

		public string MovementTicketExportFileName
		{
			get { return this._MovementTicketExportFileName; }
			set
			{
				// strip out any file extensions
				this.SetString("Movement Ticket Export File Name", 255, System.IO.Path.GetFileNameWithoutExtension(value), ref this._MovementTicketExportFileName);
			}
		}

		// Vapor Recovery Unit (VRU)
		public string VRURateLimit
		{
			get { return this._VRURateLimit.ToString(); }
			set {
				this.SetSIDouble("VRU Rate Limit", value, ref this._VRURateLimit); }
		}

		public string VRUHourlyLimit
		{
			get { return this._VRUHourlyLimit.ToString(); }
			set {
				this.SetSIDouble("VRU Hourly Limit", value, ref this._VRUHourlyLimit); }
		}

		public string VRUDailyLimit
		{
			get { return this._VRUDailyLimit.ToString(); }
			set {
				this.SetSIDouble("VRU Daily Limit", value, ref this._VRUDailyLimit); }
		}

		public string VRUYearlyLimit
		{
			get { return this._VRUYearlyLimit.ToString(); }
			set {
				this.SetSIDouble("VRU Yearly Limit", value, ref this._VRUYearlyLimit); }
		}

		public string VRUCurrentYearLimit
		{
			get { return this._VRUCurrentYearLimit.ToString(); }
			set {
				this.SetSIDouble("VRU Current Year Limit", value, ref this._VRUCurrentYearLimit); }
		}

		public string VRURateActual
		{
			get { return this._VRURateActual.ToString(); }
			set {
				this.SetSIDouble("VRU Rate Actual", value, ref this._VRURateActual); }
		}

		public string VRUHourlyActual
		{
			get { return this._VRUHourlyActual.ToString(); }
			set {
				this.SetSIDouble("VRU Hourly Actual", value, ref this._VRUHourlyActual); }
		}

		public string VRUDailyActual
		{
			get { return this._VRUDailyActual.ToString(); }
			set {
				this.SetSIDouble("VRU Daily Actual", value, ref this._VRUDailyActual); }
		}

		public string VRUYearlyActual
		{
			get { return this._VRUYearlyActual.ToString(); }
			set {
				this.SetSIDouble("VRU Yearly Actual", value, ref this._VRUYearlyActual); }
		}

		public string VRUCurrentYearActual
		{
			get { return this._VRUCurrentYearActual.ToString(); }
			set {
				this.SetSIDouble("VRU Current Year Actual", value, ref this._VRUCurrentYearActual); }
		}

		public bool VRURateLimitEnabled
		{
			get { return this._VRURateLimitEnabled; }
			set {
				this._VRURateLimitEnabled = value; }
		}

		public bool VRUHourlyLimitEnabled
		{
			get { return this._VRUHourlyLimitEnabled; }
			set {
				this._VRUHourlyLimitEnabled = value; }
		}

		public bool VRUDailyLimitEnabled
		{
			get { return this._VRUDailyLimitEnabled; }
			set {
				this._VRUDailyLimitEnabled = value; }
		}

		public bool VRUYearlyLimitEnabled
		{
			get { return this._VRUYearlyLimitEnabled; }
			set {
				this._VRUYearlyLimitEnabled = value; }
		}

		public bool VRUCurrentYearLimitEnabled
		{
			get { return this._VRUCurrentYearLimitEnabled; }
			set {
				this._VRUCurrentYearLimitEnabled = value; }
		}

		// Process I/O
		public int WatchdogPeriod { get { return this._WatchdogPeriod; } set {
			this._WatchdogPeriod = value; } }

		public WATCHDOG_MODE WatchdogMode
		{
			get { return this._WatchdogMode; }
			set
			{
				this._WatchdogMode = value;
				foreach (ProcessVariableClass ProcessVariable in this.ProcessVariableCollection)
				{
					if (ProcessVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.SITE_WATCHDOG_OUTPUT_PV)
					{
						if (this._WatchdogMode == WATCHDOG_MODE.TOGGLE)
							ProcessVariable.DataType = VarEnum.VT_BOOL;
						else
							ProcessVariable.DataType = VarEnum.VT_UI2;
						break;
					}
				}
			}
		}

		public string WatchdogCounterStart
		{
			get { return this._WatchdogCounterStart.ToString(); }
			set
			{
				int Value = 0;
				this.SetInt("Watchdog Counter Start", value, ref Value);
				if (Value > 65536
				|| Value < 0)
					throw new Exception("Watchdog Counter Start Out Of Range 0 - 65536");
				this._WatchdogCounterStart = Value;
			}
		}

		public string WatchdogCounterEnd
		{
			get { return this._WatchdogCounterEnd.ToString(); }
			set
			{
				int Value = 0;
				this.SetInt("Watchdog Counter End", value, ref Value);
				if (Value > 65536
				|| Value < 0)
					throw new Exception("Watchdog Counter End Out Of Range 0 - 65536");
				this._WatchdogCounterEnd = Value;
			}
		}

		// Regional Settings
		public NUMBER_GROUP_SIZES_TYPE NumberGroupSizesType { get { return this._NumberGroupSizesType; } set {
			this._NumberGroupSizesType = value; } }
		public string NumberDecimalSeparator { get { return this._NumberDecimalSeparator; } set {
			this.SetString("Number Decimal Separator", 1, value, ref this._NumberDecimalSeparator); } }
		public string NumberGroupSeparator { get { return this._NumberGroupSeparator; } set {
			this.SetString("Number Group Separator", 1, value, ref this._NumberGroupSeparator); } }
		public string ListSeparator { get { return this._ListSeparator; } set {
			this.SetString("List Separator", 1, value, ref this._ListSeparator); } }
		public string TimePattern { get { return this._TimePattern; } set {
			this.SetString("Time Pattern", 20, value, ref this._TimePattern); } }
		public string TimeSeparator
		{
			get
			{
				return this._TimeSeparator;
			}
			set
			{
				this.SetString("Time Separator", 10, value, ref this._TimeSeparator);
				if (this._TimeSeparator == "-")
					throw new Exception("'-' is not supported as Time Separator");
			}
		}
		public string AMSymbol { get { return this._AMSymbol; } set {
			this.SetString("AM Symbol", 2, value, ref this._AMSymbol); } }
		public string PMSymbol { get { return this._PMSymbol; } set {
			this.SetString("PM Symbol", 2, value, ref this._PMSymbol); } }
		public string ShortDatePattern { get { return this._ShortDatePattern; } set {
			this.SetString("Short Date Pattern", 20, value, ref this._ShortDatePattern); } }
		public string DateSeparator { get { return this._DateSeparator; } set {
			this.SetString("Date Separator", 1, value, ref this._DateSeparator); } }
		public string LongDatePattern { get { return this._LongDatePattern; } set {
			this.SetString("Long Date Pattern", 30, value, ref this._LongDatePattern); } }
		public int TwoDigitCalendarEndYear { get { return this._TwoDigitCalendarEndYear; } set {
			this._TwoDigitCalendarEndYear = value; } }

		public string FormatValue(object val, int decimalPlaces)
		{
			if (val == null)
			{
				return "null";
			}
			if (val is double || val is float
									|| val is sbyte || val is byte
									|| val is short || val is ushort
									|| val is int || val is uint
									|| val is long || val is ulong)
			{
				var numFormatProvider = this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.DEFAULT);


				// https://stackoverflow.com/questions/22988533/convert-tostringdecimal-iformatprovider-or-string-format
				// https://msdn.microsoft.com/en-us/library/shxtf045(v=vs.110).aspx
				// The use of Convert.ToString forces uses the default formatting string of "G" which does not append the number
				// of zeros based on the numberFormatProvider
				if (val is double)
				{
					//NumberDecimalDigits should only be set for floating point values
					numFormatProvider.NumberDecimalDigits = decimalPlaces;
					return ((double)(val)).ToString("F", numFormatProvider);
				}
				else if (val is float)
				{
					//NumberDecimalDigits should only be set for floating point values
					numFormatProvider.NumberDecimalDigits = decimalPlaces;
					return ((float)(val)).ToString("F", numFormatProvider);
				}
				else
				{
					return Convert.ToString(val, numFormatProvider);
				}

			}
			if (val is DateTime || val is DateTimeOffset)
			{
				var dateFormatProvider = this.GetDateTimeFormatInfo();
				return Convert.ToString(val, dateFormatProvider);
			}
			return val.ToString();
		}

		public int MinTimeAllowedToChangePassword
		{
			get { return this.minTimeAllowedToChangePwd; }
			set { this.minTimeAllowedToChangePwd = value; }
		}

		public int MinPasswordCharacterLength
		{
			get { return this.minPwdCharacterLength; }
			set { this.minPwdCharacterLength = value; }
		}

		public int PasswordExpirationInDays
		{
			get { return this.pwdExpirationInDays; }
			set { this.pwdExpirationInDays = value; }
		}

		public int PasswordLockoutThreshold
		{
			get { return this.pwdLockoutThreshold; }
			set { this.pwdLockoutThreshold = value; }
		}

		public int PasswordHistoryCount
		{
			get { return this.pwdHistoryCount; }
			set { this.pwdHistoryCount = value; }
		}

		public bool CheckForPreviousPassword
		{
			get { return this.checkForPreviousPwd; }
			set { this.checkForPreviousPwd = value; }
		}

		public int StrongPasswordUse
		{
			get { return this.StrongPwdUse; }
			set { this.StrongPwdUse = value; }
		}

		public bool ApplyToAllSiteMembers
		{
			get { return this.applyToAllSiteMembers; }
			set { this.applyToAllSiteMembers = value; }
		}

		public int InactivityDisablePeriod
		{
			get { return this.inactivityDisablePeriod; }
			set { this.inactivityDisablePeriod = value; }
		}

		public int DisableArchivePeriod
		{
			get { return this.disableArchivePeriod; }
			set { this.disableArchivePeriod = value; }
		}

		public bool UseTankReconciliation
		{
			get { return this.useTankReconciliation; }
			set { this.useTankReconciliation = value; }
		}

		  //Enterprise Query Credentials - System Tab
		 public string EnterpriseUserId
		 {
				get { return this._EnterpriseUserId; }
				set { this._EnterpriseUserId = value; }
		  }

		 public string EnterprisePassword
		 {
				get { return this._EnterprisePassword; }
				set { this._EnterprisePassword = value; }
		  }

		 public string EnterpriseSite
		 {
				get { return this._EnterpriseSite; }
				set { this._EnterpriseSite = value; }
		  }

		public string ServerEndPoint { get { return this.serverEndPoint; } set { this.serverEndPoint = value; } }

		public string SecurityMode { get { return this.securityMode; } set { this.securityMode = value; } }

		public string SecurityPolicy { get { return this.securityPolicy; } set { this.securityPolicy = value; } }

		public string MessageEncoding { get { return this.messageEncoding; } set { this.messageEncoding = value; } }

		public string UserIdentityMethod { get { return this.userIdentityMethod; } set { this.userIdentityMethod = value; } }

		public string UserId { get { return this.userId; } set { this.userId = value; } }

		public string UserPassword { get { return this.userPassword; } set { this.userPassword = value; } }

		public string UserCertificatePath { get { return this.userCertificatePath; } set { this.userCertificatePath = value; } }


		public string MaximumDaysToRetainArchive
		{
			get { return this.maximumDaysToRetainArchive.ToString(); }
			set
			{
				this.SetInt("Maximum Days To Retain Archive", value, ref this.maximumDaysToRetainArchive);
			}
		}

		public Boolean EnforceSalesOrderLimit { get { return this._EnforceSalesOrderLimit; } set { this._EnforceSalesOrderLimit = value; } }


		// Leak Detection Module site settings
		public int LeakDetectionMinQuietSamples { get { return this._LeakDetectionQuietSamples; } set { this._LeakDetectionQuietSamples = value; } }
		public int LeakDetectionMinQuietTime { get { return this._LeakDetectionQuietTime; } set { this._LeakDetectionQuietTime = value; } }
		public int LeakDetectionQuietTimeFactor { get { return this._LeakDetectionQuietTimeFactor; } set { this._LeakDetectionQuietTimeFactor = value; } }
		public bool LeakDetectionUseMinWait { get { return this._LeakDetectionUseMinWait; } set { this._LeakDetectionUseMinWait = value; } }
		public string LeakDetectionReport { get { return this._LeakDetectionReport; } set { this._LeakDetectionReport = value; } }
		public string LeakDetectionPrinter { get { return this._LeakDetectionPrinter; } set { this._LeakDetectionPrinter = value; } }


		/// <summary>
		/// Is this the SiteAdmin Site?
		/// </summary>
		public bool IsAdminSite => IsAdminSiteGuid(this.IdentityGuid);

		/// <summary>
		/// Is this Guid the SiteAdmin Guid?
		/// </summary>
		/// <param name="targetGuid"></param>
		/// <returns></returns>
		public static bool IsAdminSiteGuid(Guid targetGuid)
		{
			return targetGuid == Guids.SiteAdminGuid;
		}
		/// <summary>
		/// Is this the Default Site? (for single site)
		/// </summary>
		public bool IsDefaultSite => IsDefaultSiteGuid(this.IdentityGuid);

		/// <summary>
		/// Is this Guid the Default Site Guid? (for single site)
		/// </summary>
		/// <param name="targetGuid"></param>
		/// <returns></returns>
		public static bool IsDefaultSiteGuid(Guid targetGuid)
		{
			return targetGuid == Guids.SiteDefaultGuid;
		}


		public NumberFormatInfo GetNumberFormatInfo(SITE_VARIABLE_TYPE type)
		{
			NumberFormatInfo format = new NumberFormatInfo
												{
													NumberDecimalDigits = this.GetSiteDecimalPlaces(type),
													NumberGroupSizes = this.GetNumberGroupSizes(),
													NumberDecimalSeparator = this.NumberDecimalSeparator,
													NumberGroupSeparator = this.NumberGroupSeparator
												};


			return format;
		}

		public DateTimeFormatInfo GetDateTimeFormatInfo()
		{
			DateTimeFormatInfo format = new DateTimeFormatInfo
												{
														AMDesignator = this.AMSymbol,
														PMDesignator = this.PMSymbol,
														LongTimePattern = this.TimePattern,
														ShortTimePattern = this.TimePattern,
														TimeSeparator = this.TimeSeparator,
														ShortDatePattern = this.ShortDatePattern,
														DateSeparator = this.DateSeparator,
														LongDatePattern = this.LongDatePattern
												};
			return format;
		}

		/// <summary>
		/// Gets the time zone info for the site.
		/// </summary>
		/// <returns>A TimeZoneInfo object with the time zone information configured for the site or 
		/// TimeZoneInfo.Local if an error occurs looking the time zone information up.</returns>
		public TimeZoneInfo GetTimeZoneInfo()
		{
			TimeZoneInfo timeZoneInfo;
			try
			{
				timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById( this.TimeZone );
			}
			catch ( Exception )
			{
				timeZoneInfo = TimeZoneInfo.Local;
			}

			return timeZoneInfo;
		}

		/// <summary>
		/// The select clause.
		/// </summary>
		private string SelectClause = "SELECT Sites.*, " 
									+ "AncillaryData.InventoryTransactionAliasGuid, "
												+ "AncillaryData.AdjustmentTransactionAliasGuid, "
												+ "AncillaryData.IATAGuid,AncillaryData.NoteGuid, "
												+ "TransAliases1.AliasName AS InventoryTransactionAliasID, "
												+ "TransAliases2.AliasName AS AdjustmentTransactionAliasID, " + "Iata.IATAID AS IATAID ";

		AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
				AlarmAndEventDescriptorClass[] descriptors = { ManualEndOfDayEventDescriptor,
																			AutomaticEndOfDayEventDescriptor,
																			AutomaticEndOfMonthEventDescriptor,
																			UseLastKnownGoodTankDataEventDescriptor,
																			UseCurrentTankDataEventDescriptor,
																			EndOfDayProcessingBeginEventDescriptor,
																			EndOfMonthProcessingBeginDescriptor,
																			EndOfDayProcessingEndEventDescriptor,
																			EndOfMonthProcessingEndEventDescriptor,
																			ReloadTanksEventDescriptor,
																			EndOfDayFailedPhysInvCreateEventDescriptor,
																			EndOfMonthFailedPhysInvCreateEventDescriptor,
																			TankIgnoredPhysInvCreateEventDescriptor};

				return descriptors;
			}
		}

		public static SqlCommand FetchNextSequenceNumberSql(SecurityClass security, DOCUMENT_TYPE type, Guid siteGuid)
		{
				var cmd = new SqlCommand();

				string sequenceToUpdate;

				switch (type)
				{
					case DOCUMENT_TYPE.AUTOMATIC_BOL:
						sequenceToUpdate = "usp_GetNextAutomaticBOLNumber";
						break;
					case DOCUMENT_TYPE.MANUAL_BOL:
						sequenceToUpdate = "usp_GetNextManualBOLNumber";
						break;
					case DOCUMENT_TYPE.ORDER:
						sequenceToUpdate = "usp_GetNextOrderNumber";
						break;
					case DOCUMENT_TYPE.TRANSACTION:
						sequenceToUpdate = "usp_GetNextTransactionNumber";
						break;
					default:
						throw new ApplicationException("Invalid Document Type " + type + " requested");
				}

				cmd.CommandText = sequenceToUpdate;
				cmd.CommandType = CommandType.StoredProcedure;

				var updatedByParam = cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);
				updatedByParam.Value = security.UserID;
				var updatedDateParam = cmd.Parameters.Add("@UpdatedDate", SqlDbType.DateTimeOffset);
				updatedDateParam.Value = DateTimeOffset.Now;

				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);

				return cmd;
		}

		public static SqlCommand ResetSequenceNumberSql(SecurityClass security, DOCUMENT_TYPE type, Guid siteGuid)
		{
				var cmd = new SqlCommand();

				string sequenceToUpdate;
				string sequenceStart;

				switch (type)
				{
					case DOCUMENT_TYPE.AUTOMATIC_BOL:
						sequenceToUpdate = "AutomaticBOLNextNumber";
						sequenceStart = "AutomaticBOLStartNumber";
						break;
					case DOCUMENT_TYPE.MANUAL_BOL:
						sequenceToUpdate = "ManualBOLNextNumber";
						sequenceStart = "ManualBOLStartNumber";
						break;
					case DOCUMENT_TYPE.ORDER:
						sequenceToUpdate = "OrderNextNumber";
						sequenceStart = "OrderStartNumber";
						break;
					case DOCUMENT_TYPE.TRANSACTION:
						sequenceToUpdate = "TransactionNextNumber";
						sequenceStart = "TransactionStartNumber";
						break;
					default:
						throw new ApplicationException("Invalid Document Type " + type + " requested");
				}

				cmd.CommandText = "UPDATE tblSites SET " + sequenceToUpdate + " = " + sequenceStart + ", "
										+ "UpdatedDate = GETDATE(), UpdatedBy = @UpdatedBy "
										+ "WHERE SiteGuid = @SiteGuid ";

				var updatedByParam = cmd.Parameters.Add("@UpdatedBy", SqlDbType.NVarChar, 50);
				updatedByParam.Value = security.UserID;

				cmd.Parameters.AddWithValue("@SiteGuid", security.SiteGuid);

				return cmd;
		}

		public AlarmAndEventLogClass ManualEndOfDayEvent(string userID)
		{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(ManualEndOfDayEventDescriptor)
																	{
																			AssociatedData = userID
																	};
				return alarmAndEventLog;
		}

		public AlarmAndEventLogClass AutomaticEndOfDayEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(AutomaticEndOfDayEventDescriptor);
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass AutomaticEndOfMonthEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(AutomaticEndOfMonthEventDescriptor);
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass UseLastKnownGoodTankDataEvent(string userID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(UseLastKnownGoodTankDataEventDescriptor)
																	{
																		AssociatedData = userID
																	};
			return alarmAndEventLog;
		}

		public AlarmAndEventLogClass UseCurrentTankDataEvent(string userID)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(UseCurrentTankDataEventDescriptor)
																	{
																		AssociatedData = userID
																	};
			return alarmAndEventLog;
		}

		public AlarmAndEventLogClass EndOfDayProcessingBeginEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EndOfDayProcessingBeginEventDescriptor);
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass EndOfMonthProcessingBeginEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EndOfMonthProcessingBeginDescriptor);
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass EndOfDayProcessingEndEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EndOfDayProcessingEndEventDescriptor);
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass EndOfMonthProcessingEndEvent
		{
			get
			{
				AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EndOfMonthProcessingEndEventDescriptor);
				return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass ReloadTanksEvent
		{
				get
				{
					AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(ReloadTanksEventDescriptor);
					return alarmAndEventLog;
				}
		}

		/// <summary>
		/// This property returns the alarm and event log class for the failed end of day
		/// or end of month physical inventory create.
		/// </summary>
		/// <param name="errorStr">The failed error string.</param>
		/// <returns>Returns the alarm & event log class.</returns>
		public AlarmAndEventLogClass EndOfDayMonthFailedPhysInvCreateEvent(string errorStr, AlarmAndEventDescriptorClass descriptor)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(descriptor)
			{
				AssociatedData = errorStr
			};

			return alarmAndEventLog;
		}

		/// <summary>
		/// This property returns the alarm and event log class for the a tank that is
		/// ignored during the physical inventory create.
		/// </summary>
		/// <param name="errorStr">The info string.</param>
		/// <returns>Returns the alarm & event log class.</returns>
		public AlarmAndEventLogClass TankIgnoredPhysInvCreateEvent(string infoStr)
		{
			AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(TankIgnoredPhysInvCreateEventDescriptor)
			{
				AssociatedData = infoStr
			};

			return alarmAndEventLog;
		}
		#endregion

		public static string FillMethodID(FILL_METHOD fillMethod)
		{
			switch (fillMethod)
			{
				case FILL_METHOD.ACTUAL:
					return "Actual";
				case FILL_METHOD.SAFEFILL:
					return "Safe Fill";
				default:
					return "Invalid";
			}
		}

		public bool IsRowVersionSame(Byte[] aRowVersion)
		{
			if (this.RowVersion.Length != aRowVersion.Length)
			{
				return false;
			}
			for (int i = 0; i < this.RowVersion.Length; i++)
			{
				if (this.RowVersion[i] != aRowVersion[i])
				{
					return false;
				}
			}
			return true;
		}


		/// <summary>
		/// The reset.
		/// </summary>
		public override void Reset()
		{
			this.Initialize();
		}

		/// <summary>
		/// The load limit site member by data set.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Dataset cannot be null.
		/// </exception>
		public void LoadLimitSiteMemberByDataSet(DataSet dataSet)
		{
			if ( dataSet == null )
			{
				throw new ArgumentNullException(nameof(dataSet));
			}

			if (dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0)
			{
				DataRow row = dataSet.Tables[0].Rows[0];
				this.LoadLimitSiteMemberByRow(row);
			}
		}

		/// <summary>
		/// The load limit site member by row.
		/// </summary>
		/// <param name="row">
		/// The row.
		/// </param>
		public void LoadLimitSiteMemberByRow(DataRow row)
		{
			if (row == null)
			{
				return;
			}

			// SELECT Sites.SiteGuid, Sites.ID, Sites.Number, Sites.SiteGroupFlag"
			this._SiteGuid		= row.IsNull("SiteGuid") ? Guid.Empty : (Guid)row["SiteGuid"];
			this._IdentityGuid	= this._SiteGuid;
			this._ID			= row.IsNull("ID") ? string.Empty : (string)row["ID"];
			this._Number		= row.IsNull("Number") ? string.Empty : (string)row["Number"];
			this._SiteGroup		= !row.IsNull("SiteGroupFlag") && (bool)row["SiteGroupFlag"];
		}

		/// <summary>
		/// This method loads the Site object from the database.
		/// </summary>
		/// <param name="dataSet"></param>
		public void Load(DataSet dataSet)
		{
			if (dataSet == null)
			{
				throw new ArgumentNullException(nameof(dataSet));
			}

			this.Reset();

			DataTable table = dataSet.Tables[0];
			if (table.Rows.Count == 0)
				return;

			DataRow row = table.Rows[0];

			this.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);

			this.IdentityGuid = this.SiteGuid;

			// General
			base.ID = DataObject.getValue(row["ID"], "");
			this.SiteID = base.ID;
			this.Number = DataObject.getValue(row["Number"], "");
			this.SPLCCode = DataObject.getValue(row["SPLCCode"], "");
			this.Address1 = DataObject.getValue(row["Address1"], "");
			this.Address2 = DataObject.getValue(row["Address2"], "");
			this.City = DataObject.getValue(row["City"], "");
			this.State = DataObject.getValue(row["State"], "");
			this.Zip = DataObject.getValue(row["Zip"], "");
			this.Country = DataObject.getValue(row["Country"], "");
			this.Phone = DataObject.getValue(row["Phone"], "");
			this.Fax = DataObject.getValue(row["Fax"], "");
			this.EmailAddress = DataObject.getValue(row["EmailAddress"], "");
			this.EmergencyContact = DataObject.getValue(row["EmergencyContact"], "");
			this.EmergencyPhone = DataObject.getValue(row["EmergencyPhone"], "");
			this.Enabled = DataObject.getValue(row["Enabled"], true);
			this.SiteGroup = DataObject.getValue(row["SiteGroupFlag"], false);
			this.TimeZone = DataObject.getValue(row["TimeZone"], "Eastern Standard Time");
				this._TerminalControlNumber = DataObject.getValue(row["TerminalControlNumber"], string.Empty);
				this._InhibitLoadRackCardIns = DataObject.getValue(row["InhibitLoadRackCardIns"], false);
			this.IATAGuid = DataObject.getValue(row["IATAGuid"], Guid.Empty);
			this.InhibitSiteLedgerRollup = DataObject.getValue(row["InhibitSiteLedgerRollup"], false);
			this._Contact1Name = DataObject.getValue(row["Contact1Name"], string.Empty);
			this._Contact1Address1 = DataObject.getValue(row["Contact1Address1"], string.Empty);
			this._Contact1Address2 = DataObject.getValue(row["Contact1Address2"], string.Empty);
			this._Contact1City = DataObject.getValue(row["Contact1City"], string.Empty);
			this._Contact1State = DataObject.getValue(row["Contact1State"], string.Empty);
			this._Contact1Zip = DataObject.getValue(row["Contact1Zip"], string.Empty);
			this._Contact1Country = DataObject.getValue(row["Contact1Country"], string.Empty);
			this._Contact1PhoneOffice = DataObject.getValue(row["Contact1PhoneOffice"], string.Empty);
			this._Contact1PhoneMobile = DataObject.getValue(row["Contact1PhoneMobile"], string.Empty);
			this._Contact1Fax = DataObject.getValue(row["Contact1Fax"], string.Empty);
			this._Contact1EmailAddress = DataObject.getValue(row["Contact1EmailAddress"], string.Empty);
			this._Contact2Name = DataObject.getValue(row["Contact2Name"], string.Empty);
			this._Contact2Address1 = DataObject.getValue(row["Contact2Address1"], string.Empty);
			this._Contact2Address2 = DataObject.getValue(row["Contact2Address2"], string.Empty);
			this._Contact2City = DataObject.getValue(row["Contact2City"], string.Empty);
			this._Contact2State = DataObject.getValue(row["Contact2State"], string.Empty);
			this._Contact2Zip = DataObject.getValue(row["Contact2Zip"], string.Empty);
			this._Contact2Country = DataObject.getValue(row["Contact2Country"], string.Empty);
			this._Contact2PhoneOffice = DataObject.getValue(row["Contact2PhoneOffice"], string.Empty);
			this._Contact2PhoneMobile = DataObject.getValue(row["Contact2PhoneMobile"], string.Empty);
			this._Contact2Fax = DataObject.getValue(row["Contact2Fax"], string.Empty);
			this._Contact2EmailAddress = DataObject.getValue(row["Contact2EmailAddress"], string.Empty);
			this.latitude = DataObject.getValue<double?>(row["Latitude"], null);
			this.longitude = DataObject.getValue<double?>(row["Longitude"], null);
			this.zoom = DataObject.getValue<int?>(row["Zoom"], null);
				this.activeDirectorySiteGroupGuid = DataObject.getValue(row["ActiveDirectorySiteGroupGuid"], Guid.Empty);
				this.RowVersion = DataObject.getValue<Byte[]>(row["_RowVersion"], null);

			// Units
			this.LevelUnits = DataObject.getValue(row["LevelUnitIndex"], EngineeringUnit.FmlFtIn16Th);
			this.TemperatureUnits = DataObject.getValue(row["TemperatureUnitIndex"], EngineeringUnit.FmtDegF);

			this._DensityUnits = DataObject.getValue(row["DensityUnitIndex"], EngineeringUnit.FmdDegApi);
			this._PressureUnits = DataObject.getValue(row["PressureUnitindex"], EngineeringUnit.FmpPsi);
			this._FlowUnits = DataObject.getValue(row["FlowUnitIndex"], EngineeringUnit.FmvfGpm);
			this._VolumeUnits = DataObject.getValue(row["VolumeUnitIndex"], EngineeringUnit.FmvUsGal);
			this._MassUnits = DataObject.getValue(row["MassUnitIndex"], EngineeringUnit.FmmLb);
			this._AdditiveVolumeUnits = DataObject.getValue(row["AdditiveVolumeUnitIndex"], EngineeringUnit.FmvCm3);
			this._AdditiveProfileCycleAmountUnits = DataObject.getValue(row["AdditiveProfileCycleAmountUnitIndex"], EngineeringUnit.FmvCm3);
			this._AdditiveProfileRateUnits = DataObject.getValue(row["AdditiveProfileRateUnitIndex"], EngineeringUnit.FmvUsGal);
			this._LevelDecimalPlaces = DataObject.getValue<byte>(row["LevelDecimalPlaces"], 2);
			this._TemperatureDecimalPlaces = DataObject.getValue<byte>(row["TemperatureDecimalPlaces"], 0);
			this._DensityDecimalPlaces = DataObject.getValue<byte>(row["DensityDecimalPlaces"], 1);
			this._PressureDecimalPlaces = DataObject.getValue<byte>(row["PressureDecimalPlaces"], 2);
			this._FlowDecimalPlaces = DataObject.getValue<byte>(row["FlowDecimalPlaces"], 1);
			this._VolumeDecimalPlaces = DataObject.getValue<byte>(row["VolumeDecimalPlaces"], 0);
			this._MassDecimalPlaces = DataObject.getValue<byte>(row["MassDecimalPlaces"], 0);
			this._AdditiveVolumeDecimalPlaces = DataObject.getValue<byte>(row["AdditiveVolumeDecimalPlaces"], 0);
			this._AdditiveProfileCycleAmountDecimalPlaces = DataObject.getValue<byte>(row["AdditiveProfileCycleAmountDecimalPlaces"], 0);
			this._AdditiveProfileRateDecimalPlaces = DataObject.getValue<byte>(row["AdditiveProfileRateDecimalPlaces"], 0);
			this._QuantityDisplayDefault = (QuantityDisplay)DataObject.getValue(row["LookupQuantityDisplayDefaultIndex"], (byte)QuantityDisplay.GROSS);

			//// Load Rack
			this._InhibitAccessAfterHours = DataObject.getValue(row["InhibitAccessAfterHours"], false);
			this._InhibitMultipleCardIns = DataObject.getValue(row["InhibitMultipleCardIns"], true);
			this._AccessCardInRequired = DataObject.getValue(row["AccessCardInRequired"], true);
			this._CheckSiteNumber = DataObject.getValue(row["CheckSiteNumber"], false);
			this._PromptForCustomerCard = DataObject.getValue(row["PromptForCustomerCard"], false);
			this._PromptForTractorOrTanker = DataObject.getValue(row["PromptForTractorOrTanker"], false);
			this._PromptForFirstTrailer = DataObject.getValue(row["PromptForFirstTrailer"], false);
			this._PromptForSecondTrailer = DataObject.getValue(row["PromptForSecondTrailer"], false);
				this._PromptForThirdTrailer = DataObject.getValue(row["PromptForThirdTrailer"], false);
			this._PromptForCompartment = DataObject.getValue(row["PromptForCompartment"], false);
				this._PromptForTransactionCompletion = DataObject.getValue(row["PromptForTransactionCompletion"], false);
				this._InhibitCustomerConfirmationPrompt = DataObject.getValue(row["InhibitCustomerConfirmationPrompt"], false);
				this._RequireTrailerScully = DataObject.getValue(row["RequireTrailerScully"], false);
				this._CardInTimeout = DataObject.getValue(row["CardInTimeout"], 30);
			this._EnforceDriverEquipmentMatch = DataObject.getValue(row["EnforceDriverEquipmentMatch"], true);
			this._EnableAdditiveAccounting = DataObject.getValue(row["EnableAdditiveAccounting"], true);
			this._UseCompanyEquipmentIdentifiers = DataObject.getValue(row["UseCompanyEquipmentIdentifiers"], false);
			this._UseLastKnownGoodTankData = DataObject.getValue(row["UseLastKnownGoodTankData"], false);

			this._MaximumLoadAmount.SIValue = DataObject.getValue(row["MaximumLoadAmount"], 5000.0);
			this._MaximumLoadTime = DataObject.getValue(row["MaximumLoadTime"], 720);  // default to 12 hours
			this._MaximumIdleTime = DataObject.getValue(row["MaximumIdleTime"], 10);
			this._MaximumFlushAmount.SIValue = DataObject.getValue(row["MaximumFlushAmount"], 500.0);
			this._MaximumMeterProvingAmount.SIValue = DataObject.getValue(row["MaximumMeterProvingAmount"], 500.0);
			this._MaximumReturnsAmount.SIValue = DataObject.getValue(row["MaximumReturnsAmount"], 500.0);
			this._MaximumNumberOfActiveArms = DataObject.getValue(row["MaximumNumberOfActiveArms"], 10);
			this._DriverTimeoutPeriod = DataObject.getValue(row["DriverTimeoutPeriod"], 90);
			this._DriverWarningPeriod = DataObject.getValue(row["DriverWarningPeriod"], 5);
			this._MaximumPrompts = DataObject.getValue(row["MaximumPrompts"], 3);
			this._InventoryTransactionAliasGuid = DataObject.getValue(row["InventoryTransactionAliasGuid"], Guid.Empty);
			this._AdjustmentTransactionAliasGuid = DataObject.getValue(row["AdjustmentTransactionAliasGuid"], Guid.Empty);
			this._MaximumVehicleWeight.SIValue = DataObject.getValue(row["MaximumVehicleWeight"], 36287.392);
			this._LoadByNet = DataObject.getValue(row["LoadByNet"], false);
			this._PromptForShipmentNumber = DataObject.getValue(row["PromptForShipmentNumber"], false);
			this._MaximumProductTemperature.SIValue = DataObject.getValue(row["MaximumProductTemperature"], 15.56);
			this._ListEquipment = DataObject.getValue(row["ListEquipment"], false);
			this._DeferStationChanges = DataObject.getValue(row["DeferStationChanges"], false);
			this._EnforceSalesOrderLimit = DataObject.getValue(row["EnforceSalesOrderLimit"], false);

			this._LeakDetectionQuietSamples = DataObject.getValue(row["LeakDetectionQuietSamples"], 6);
			this._LeakDetectionQuietTime = DataObject.getValue(row["LeakDetectionQuietTime"], 1440);
			this._LeakDetectionQuietTimeFactor = DataObject.getValue(row["LeakDetectionQuietTimeFactor"], 8);
			this._LeakDetectionUseMinWait = DataObject.getValue(row["LeakDetectionUseMinWait"], false);
			this._LeakDetectionReport = DataObject.getValue(row["LeakDetectionReport"], string.Empty);
			this._LeakDetectionPrinter = DataObject.getValue(row["LeakDetectionPrinter"], string.Empty);


			// Transactions
			this._InhibitBOLWithBrokenBlends = DataObject.getValue(row["InhibitBOLWithBrokenBlends"], true);
			this._InhibitBOLWithImproperAdditization = DataObject.getValue(row["InhibitBOLWithImproperAdditization"], true);
			this._InhibitOverweightBOL = DataObject.getValue(row["InhibitOverweightBOL"], true);
			this._ExceptionBOLPrinter = DataObject.getValue(row["ExceptionBOLPrinter"], "");
			this._EnableAutomaticBOLPrinting = DataObject.getValue(row["EnableAutomaticBOLPrinting"], true);
			this._AutomaticBOLStartNumber = DataObject.getValue(row["AutomaticBOLStartNumber"], 0);
			this._AutomaticBOLEndNumber = DataObject.getValue(row["AutomaticBOLEndNumber"], 0);
			this._AutomaticBOLNextNumber = DataObject.getValue(row["AutomaticBOLNextNumber"], 0);
			this._SeparateManualBOLNumbering = DataObject.getValue(row["SeparateManualBOLNumbering"], false);
			this._EnableBOLPDFArchiving = DataObject.getValue(row["EnableBOLPDFArchiving"], true);
			this._BOLPDFArchivingPath = DataObject.getValue(row["BOLPDFArchivingPath"], "");
			this._ManualBOLStartNumber = DataObject.getValue(row["ManualBOLStartNumber"], 0);
			this._ManualBOLEndNumber = DataObject.getValue(row["ManualBOLEndNumber"], 0);
			this._ManualBOLNextNumber = DataObject.getValue(row["ManualBOLNextNumber"], 0);
			this._TransactionStartNumber = DataObject.getValue(row["TransactionStartNumber"], 0);
			this._TransactionEndNumber = DataObject.getValue(row["TransactionEndNumber"], 0);
			this._TransactionNextNumber = DataObject.getValue(row["TransactionNextNumber"], 0);
			this._OrderStartNumber = DataObject.getValue(row["OrderStartNumber"], 0);
			this._OrderEndNumber = DataObject.getValue(row["OrderEndNumber"], 0);
			this._OrderNextNumber = DataObject.getValue(row["OrderNextNumber"], 0);
			this._NumberPrefix = DataObject.getValue(row["NumberPrefix"], "%Date%");
			this._OpenTransactionWindow = DataObject.getValue(row["OpenTransactionWindow"], 2);
			this._AdministrativeLockDate.Value = DataObject.getValue(row["AdministrativeLockDate"], TimeConverter.Today(this._AdministrativeLockDate.StandardName).AddDays(-1));
			this._OperationalLockDate.Value = DataObject.getValue(row["OperationalLockDate"], TimeConverter.Now(this._OperationalLockDate.StandardName).AddDays(-1));

			// System
			this._MaximumDaysToRetainLogs = DataObject.getValue(row["MaximumDaysToRetainLogs"], 60);
			this.maximumDaysToRetainArchive = DataObject.getValue(row["MaximumDaysToRetainArchive"], 365);
			this._EnableDebugLogging = DataObject.getValue(row["EnableDebugLogging"], false);
			this._EnableAuditLogging = DataObject.getValue(row["EnableAuditLogging"], true);
			this._AutomaticallyPrintAlarmsAndEvents = DataObject.getValue(row["AutomaticallyPrintAlarmsAndEvents"], false);
			this._AlarmAndEventPrinter = DataObject.getValue(row["AlarmAndEventPrinter"], string.Empty);
			this._MailServer = DataObject.getValue(row["MailServer"], "localhost");
			this._MailFrom = DataObject.getValue(row["MailFrom"], "");
			this._MailUserName = DataObject.getValue(row["MailUserName"], string.Empty);
			this._MailPassword = DataObject.getValue(row["MailPassword"], string.Empty);
			this._MailConnectMode = (MAIL_SERVER_CONNECT_MODE)DataObject.getValue(row["LookupMailConnectModeIndex"], (byte)MAIL_SERVER_CONNECT_MODE.LAN);
			this._DialupName = DataObject.getValue(row["DialupName"], "");
			this._SCADASystem = DataObject.getValue(row["SCADASystem"], "localhost");
			this._InhibitTemplateGraphics = DataObject.getValue(row["InhibitTemplateGraphics"], false);
			this._RefreshInterval = DataObject.getValue(row["RefreshInterval"], 5);
			this._InhibitEndOfDayOperations = DataObject.getValue(row["InhibitEndOfDayOperations"], false);
			this._InhibitEndOfMonthOperations = DataObject.getValue(row["InhibitEndOfMonthOperations"], false);
			this._EndOfDayWarningPeriod = DataObject.getValue(row["EndOfDayWarningPeriod"], 30);
			this._InhibitAutomaticPhysicalInventory = DataObject.getValue(row["InhibitAutomaticPhysicalInventory"], false);
			this._InhibitAutomaticMeterCloseout = DataObject.getValue(row["InhibitAutomaticMeterCloseout"], true);
			this._InhibitAutomaticReportGeneration = DataObject.getValue(row["InhibitAutomaticReportGeneration"], true);
			this._InhibitAutomaticAdjustmentDistribution = DataObject.getValue(row["InhibitAutomaticAdjustmentDistribution"], true);
			this._InhibitAutomaticCloseout = DataObject.getValue(row["InhibitAutomaticCloseout"], true);
			this._BlockCloseOnUnpostedBol = DataObject.getValue(row["BlockCloseOnUnpostedBol"], false);
			this._InhibitTankScan = DataObject.getValue(row["InhibitTankScan"], false);
			this._ReportDirectory = DataObject.getValue(row["ReportDirectory"], "/Standard Reports");
			this._ManageReports = DataObject.getValue(row["ManageReports"], false);
			this._ManagedReportDirectory = DataObject.getValue(row["ManagedReportDirectory"], "");
			this._MeterReconciliationToleranceIsPercent = DataObject.getValue(row["MeterReconciliationToleranceIsPercent"], false);
			this._MeterReconciliationReportName = DataObject.getValue(row["MeterReconciliationReportName"], string.Empty);
			this._TranslatedHelpURL = DataObject.getValue(row["TranslatedHelpURL"], string.Empty);
			this._EnableAutomaticMovementTicketPrinting = DataObject.getValue(row["EnableAutomaticMovementTicketPrinting"], true);
			this._MovementTicketReport = DataObject.getValue(row["MovementTicketReport"], string.Empty);
			this._MovementTicketPrinter = DataObject.getValue(row["MovementTicketPrinter"], string.Empty);
			this._MaxOperateTabsAllowed = DataObject.getValue(row["MaxOperateTabsAllowed"], 10);
			this._CloseoutTime = row["CloseoutTime"] as TimeSpan?;
			this._PointGroupFileExportDirectory = DataObject.getValue(row["PointGroupFileExportDirectory"], "");
			this._PointGroupDefaultFileName = DataObject.getValue(row["PointGroupDefaultFileName"], "");
			this._EnableMovementTicketPDFArchiving = DataObject.getValue(row["EnableMovementTicketPDFArchiving"], false);
			this._MovementTicketFileExportDirectory = DataObject.getValue(row["MovementTicketFileExportDirectory"], "");
			this._MovementTicketExportFileName = DataObject.getValue(row["MovementTicketExportFileName"], "");

			// Vapor Recovery Unit (VRU)
			this._VRURateLimit.SIValue = DataObject.getValue(row["VRURateLimit"], 0.0);
			this._VRUHourlyLimit.SIValue = DataObject.getValue(row["VRUHourlyLimit"], 0.0);
			this._VRUDailyLimit.SIValue = DataObject.getValue(row["VRUDailyLimit"], 0.0);
			this._VRUYearlyLimit.SIValue = DataObject.getValue(row["VRUYearlyLimit"], 0.0);
			this._VRUCurrentYearLimit.SIValue = DataObject.getValue(row["VRUCurrentYearLimit"], 0.0);
			this._VRURateActual.SIValue = DataObject.getValue(row["VRURateActual"], 0.0);
			this._VRUHourlyActual.SIValue = DataObject.getValue(row["VRUHourlyActual"], 0.0);
			this._VRUDailyActual.SIValue = DataObject.getValue(row["VRUDailyActual"], 0.0);
			this._VRUYearlyActual.SIValue = DataObject.getValue(row["VRUYearlyActual"], 0.0);
			this._VRUCurrentYearActual.SIValue = DataObject.getValue(row["VRUCurrentYearActual"], 0.0);
			this._VRURateLimitEnabled = DataObject.getValue(row["VRURateLimitEnabled"], false);
			this._VRUHourlyLimitEnabled = DataObject.getValue(row["VRUHourlyLimitEnabled"], false);
			this._VRUDailyLimitEnabled = DataObject.getValue(row["VRUDailyLimitEnabled"], false);
			this._VRUYearlyLimitEnabled = DataObject.getValue(row["VRUYearlyLimitEnabled"], false);
			this._VRUCurrentYearLimitEnabled = DataObject.getValue(row["VRUCurrentYearLimitEnabled"], false);

			// Process Variables
			this._WatchdogPeriod = DataObject.getValue(row["WatchdogPeriod"], 10);
			this._WatchdogMode = (WATCHDOG_MODE)DataObject.getValue(row["LookupWatchdogModeIndex"], (byte)WATCHDOG_MODE.TOGGLE);
			this._WatchdogCounterStart = DataObject.getValue(row["WatchdogCounterStart"], 0);
			this._WatchdogCounterEnd = DataObject.getValue(row["WatchdogCounterStart"], 1000);


			// Regional Settings
			this._NumberGroupSizesType = (NUMBER_GROUP_SIZES_TYPE)DataObject.getValue(row["LookupNumberGroupSizesTypeIndex"], (int)NUMBER_GROUP_SIZES_TYPE.THREE);
			this._NumberDecimalSeparator = DataObject.getValue(row["NumberDecimalSeparator"], ".");
			this._NumberGroupSeparator = DataObject.getValue(row["NumberGroupSeparator"], ",");
			this._ListSeparator = DataObject.getValue(row["ListSeparator"], ",");
			this._TimePattern = DataObject.getValue(row["TimePattern"], "hh:mm:ss tt");
			this._TimeSeparator = DataObject.getValue(row["TimeSeparator"], ":");
			this._AMSymbol = DataObject.getValue(row["AMSymbol"], "AM");
			this._PMSymbol = DataObject.getValue(row["PMSymbol"], "PM");
			this._ShortDatePattern = DataObject.getValue(row["ShortDatePattern"], "M/d/yyyy");
			this._DateSeparator = DataObject.getValue(row["DateSeparator"], "/");
			this._LongDatePattern = DataObject.getValue(row["LongDatePattern"], "ddddd, MMMMM dd, yyyy");
			this._TwoDigitCalendarEndYear = DataObject.getValue(row["TwoDigitCalendarEndYear"], 2029);

			//// User Data
			for (int userDataIndex = 0; userDataIndex < MAX_USER_DATA; userDataIndex++)
			{
				string fieldName = "UserData" + (userDataIndex + 1);
				this.UserData[userDataIndex] = DataObject.getValue(row[fieldName], string.Empty);
			}

			//Notes
			this.NoteGuid = DataObject.getValue(row["NoteGuid"], Guid.Empty);

			// Audit
			this.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			this.CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
			this.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this.CreatedDate);
			this.UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);

			// Password configuration
			this.minTimeAllowedToChangePwd = DataObject.getValue(row["MinTimeAllowedToChangePwd"], 0);
			this.minPwdCharacterLength = DataObject.getValue(row["MinPwdCharacterLength"], UserClass.UserDataCount);
			this.pwdExpirationInDays = DataObject.getValue(row["PwdExpirationInDays"], 999);
			this.pwdLockoutThreshold = DataObject.getValue(row["PwdLockoutThreshold"], 0);
			this.checkForPreviousPwd = DataObject.getValue(row["CheckForPreviousPwd"], false);
			this.StrongPwdUse = DataObject.getValue(row["StrongPwdUse"], (int)StrongPasswordUsage.None);
			this.pwdHistoryCount = DataObject.getValue(row["pwdHistoryCount"], 0);
			this.applyToAllSiteMembers = DataObject.getValue(row["applyToAllSiteMembers"], false);
			this.inactivityDisablePeriod = DataObject.getValue(row["inactivityDisablePeriod"], 0);
			this._EnforceSingleOwner = DataObject.getValue(row["EnforceSingleOwner"], true);
			this._InhibitBOLSummaryAutoPopulate = DataObject.getValue(row["InhibitBOLSummaryAutoPopulate"], false);
			this._InhibitOrderSummaryAutoPopulate = DataObject.getValue(row["InhibitOrderSummaryAutoPopulate"], false);
			this._InhibitSupplyOrderSummaryAutoPopulate = DataObject.getValue(row["InhibitSupplyOrderSummaryAutoPopulate"], false);
			//// vt 07-15-2008
			this._InvoiceStartNumber = DataObject.getValue(row["InvoiceStartNumber"], 0);
			this._InvoiceEndNumber = DataObject.getValue(row["InvoiceEndNumber"], 0);
			this._InvoiceNextNumber = DataObject.getValue(row["InvoiceNextNumber"], 0);

			this._PromptForReturns = DataObject.getValue(row["PromptForReturns"], false);
			this._PromptForTruckCard = DataObject.getValue(row["PromptForTruckCard"], false);
			this._StartingShortCardNumber = DataObject.getValue(row["StartingShortCardNumber"], 1);
			this._UseShortCardNumber = DataObject.getValue(row["UseShortCardNumber"], false);
			this._ExcessVarianceCount = DataObject.getValue<byte>(row["ExcessVarianceCount"], 2);
			this._ExcessVarianceTolerance = DataObject.getValue(row["ExcessVarianceTolerance"], 2.0);
			this._SecondaryStorageFillMethod = (FILL_METHOD)(DataObject.getValue(row["LookupSecondaryStorageFillMethodIndex"], (byte)FILL_METHOD.ACTUAL));

			this.disableArchivePeriod = DataObject.getValue(row["disableArchivePeriod"], 180);
			this._ExportArchiveDir = DataObject.getValue(row["ExportArchiveDir"], "ExportArchiveDir");
			this._ImportArchiveDir = DataObject.getValue(row["ImportArchiveDir"], "ImportArchiveDir");
			this._GroupLedgerByID = DataObject.getValue(row["GroupLedgerByID"], false);

			this.useTankReconciliation = DataObject.getValue(row["UseTankReconciliation"], false);

			this.InventoryTransactionAliasID = DataObject.getValue(row["InventoryTransactionAliasID"], "{None}");
			this.AdjustmentTransactionAliasID = DataObject.getValue(row["AdjustmentTransactionAliasID"], "{None}");
			this.IATAID = DataObject.getValue(row["IATAID"], "{None}");

			// Password Hint/Forgotten Password
			this._EnablePasswordHint = DataObject.getValue(row["EnablePasswordHint"], false);
			this._EnablePasswordReset = DataObject.getValue(row["EnablePasswordReset"], false);
			this._AllowUseOfSpecialChars = DataObject.getValue(row["AllowUseOfSpecialChars"], false);

			this._EnablePeriodicSyncFlag = DataObject.getValue(row["EnablePeriodicSyncFlag"], false);
			this._PeriodicSyncIntervalMinutes = DataObject.getValue(row["PeriodicSyncIntervalMinutes"], 0);
			this._DisableSyncTransferFlag = DataObject.getValue(row["DisableSyncTransferFlag"], false);
			this._Enterprise = DataObject.getValue<bool>(row["Enterprise"], false);
			this._OperateTabGroups = DataObject.getValue<bool>(row["OperateTabGroups"], false);

				//Enterprise Query Credentials
			 this._EnterpriseUserId = DataObject.getValue(row["EnterpriseUserId"], string.Empty);
				this._EnterprisePassword = row.IsNull("EnterprisePassword") ? string.Empty : UserClass.decode((byte[])row["EnterprisePassword"], this.SiteGuid);
			 this._EnterpriseSite = DataObject.getValue(row["EnterpriseSite"], string.Empty);

			this.serverEndPoint = DataObject.getValue(row["ServerEndPoint"], string.Empty);
			this.securityMode = DataObject.getValue(row["SecurityMode"], string.Empty);
			this.securityPolicy = DataObject.getValue(row["SecurityPolicy"], string.Empty);
			this.messageEncoding = DataObject.getValue(row["MessageEncoding"], string.Empty);
			this.userIdentityMethod = DataObject.getValue(row["UserIdentityMethod"], string.Empty);
			this.userId = DataObject.getValue(row["UserId"], string.Empty);
			this.userPassword = DataObject.getValue(row["UserPassword"], string.Empty);
			this.userCertificatePath = DataObject.getValue(row["UserCertificatePath"], string.Empty);
		}

		/// <summary>
		/// This method is specific for synchronization on client servers where ancillary data does not get synchronized for non-hosted site records.
		/// </summary>
		/// <param name="set"></param>
		public void LoadPartial(DataSet set)
		{
				if (set == null)
				{
					throw new ArgumentNullException(nameof(set));
				}

			this.Reset();

				DataTable table = set.Tables[0];
				if (table.Rows.Count == 0)
					return;

				DataRow row = table.Rows[0];

			this.SiteGuid = DataObject.getValue(row["SiteGuid"], Guid.Empty);

			this.IdentityGuid = this.SiteGuid;

				// General
				base.ID = DataObject.getValue(row["ID"], "");
				this.SiteID = base.ID;
				this.Number = DataObject.getValue(row["Number"], "");
				this.SPLCCode = DataObject.getValue(row["SPLCCode"], "");
				this.Enabled = DataObject.getValue(row["Enabled"], true);
				this.SiteGroup = DataObject.getValue(row["SiteGroupFlag"], false);

				// Audit
			this.CreatedDate = DataObject.getValue(row["CreatedDate"], DateTimeOffset.Now);
			this.CreatedBy = DataObject.getValue(row["CreatedBy"], ADMIN);
			this.UpdatedDate = DataObject.getValue(row["UpdatedDate"], this.CreatedDate);
			this.UpdatedBy = DataObject.getValue(row["UpdatedBy"], ADMIN);

			this._EnablePeriodicSyncFlag = DataObject.getValue(row["EnablePeriodicSyncFlag"], false);
			this._PeriodicSyncIntervalMinutes = DataObject.getValue(row["PeriodicSyncIntervalMinutes"], 0);
		}

		public void InsertSQL(SqlCommand cmd)
		{
			cmd.CommandText = "INSERT INTO tblSites (" +
					"SiteGuid," + 
					"ID," +

					// General
					"Number," +
					"SPLCCode," +
					"Address1," +
					"Address2," +
					"City," +
					"State," +
					"Zip," +
					"Country," +
					"Phone," +
					"Fax," +
					"EmergencyContact," +
					"EmergencyPhone," +
					"EmailAddress," +
					"Enabled," +
					"SiteGroupFlag," +
					"TimeZone," +
					"TerminalControlNumber," +
					"InhibitLoadRackCardIns," +
					"EnforceSingleOwner, " +
					"InhibitBOLSummaryAutoPopulate, " +
					"InhibitOrderSummaryAutoPopulate, " +
					"InhibitSupplyOrderSummaryAutoPopulate, " +
					"InhibitSiteLedgerRollup, " +
					"Contact1Name," +
					"Contact1Address1," +
					"Contact1Address2," +
					"Contact1City," +
					"Contact1State," +
					"Contact1Zip," +
					"Contact1Country," +
					"Contact1PhoneOffice," +
					"Contact1PhoneMobile," +
					"Contact1Fax," +
					"Contact1EmailAddress," +
					"Contact2Name," +
					"Contact2Address1," +
					"Contact2Address2," +
					"Contact2City," +
					"Contact2State," +
					"Contact2Zip," +
					"Contact2Country," +
					"Contact2PhoneOffice," +
					"Contact2PhoneMobile," +
					"Contact2Fax," +
					"Contact2EmailAddress," +
					"Latitude, " +
					"Longitude, " +
					"Zoom, " +
						  "ActiveDirectorySiteGroupGuid, " +

						  // Units
						  "LevelUnitIndex," +
					"TemperatureUnitIndex," +
					"DensityUnitIndex," +
					"PressureUnitIndex," +
					"FlowUnitIndex," +
					"VolumeUnitIndex," +
					"MassUnitIndex," +
					"AdditiveVolumeUnitIndex," +
					"AdditiveProfileCycleAmountUnitIndex," +
					"AdditiveProfileRateUnitIndex," +
					"LevelDecimalPlaces," +
					"TemperatureDecimalPlaces," +
					"DensityDecimalPlaces," +
					"PressureDecimalPlaces," +
					"FlowDecimalPlaces," +
					"VolumeDecimalPlaces," +
					"MassDecimalPlaces," +
					"AdditiveVolumeDecimalPlaces," +
					"AdditiveProfileCycleAmountDecimalPlaces," +
					"AdditiveProfileRateDecimalPlaces," +
					"LookupQuantityDisplayDefaultIndex," +

					// Load Rack
					"InhibitAccessAfterHours," +
					"InhibitMultipleCardIns," +
					"AccessCardInRequired," +
					"CheckSiteNumber," +
					"PromptForCustomerCard," +
					"PromptForTractorOrTanker," +
					"PromptForFirstTrailer," +
					"PromptForSecondTrailer," +
					"PromptForThirdTrailer," +
					"PromptForCompartment," +
					"PromptForTransactionCompletion," +
					"InhibitCustomerConfirmationPrompt," +
					"RequireTrailerScully," +
					"CardInTimeout," +
					"EnforceDriverEquipmentMatch," +
					"EnableAdditiveAccounting," +
					"UseCompanyEquipmentIdentifiers," +
					"UseLastKnownGoodTankData," +
					"MaximumLoadAmount," +
					"MaximumLoadTime," +
					"MaximumIdleTime," +
					"MaximumFlushAmount," +
					"MaximumMeterProvingAmount," +
					"MaximumReturnsAmount," +
					"MaximumNumberOfActiveArms," +
					"DriverTimeoutPeriod," +
					"DriverWarningPeriod," +
					"MaximumPrompts," +
					"MaximumVehicleWeight," +
					"LoadByNet," +
					"PromptForShipmentNumber," +
					"MaximumProductTemperature," +
					"ListEquipment," +
					"DeferStationChanges," +
					"PromptForReturns," +
					"PromptForTruckCard," +
					"StartingShortCardNumber," +
					"UseShortCardNumber," +
					"ExcessVarianceCount," +
					"ExcessVarianceTolerance," +
					"LookupSecondaryStorageFillMethodIndex," +
					"EnforceSalesOrderLimit," +

					"LeakDetectionQuietSamples," +
					"LeakDetectionQuietTime," +
					"LeakDetectionQuietTimeFactor," +
					"LeakDetectionUseMinWait," +
					"LeakDetectionReport," +
					"LeakDetectionPrinter," +
					
					// Transactions
					"InhibitBOLWithBrokenBlends," +
					"InhibitBOLWithImproperAdditization," +
					"InhibitOverweightBOL," +
					"ExceptionBOLPrinter," +
					"EnableAutomaticBOLPrinting," +
					"AutomaticBOLStartNumber," +
					"AutomaticBOLEndNumber," +
					"AutomaticBOLNextNumber," +
					"SeparateManualBOLNumbering," +
					"ManualBOLStartNumber," +
					"ManualBOLEndNumber," +
					"ManualBOLNextNumber," +
					"TransactionStartNumber," +
					"TransactionEndNumber," +
					"TransactionNextNumber," +
					"OrderStartNumber," +
					"OrderEndNumber," +
					"OrderNextNumber," +
					"InvoiceStartNumber," +
					"InvoiceEndNumber," +
					"InvoiceNextNumber," +
					"NumberPrefix," +
					"OpenTransactionWindow," +
					"AdministrativeLockDate," +
					"OperationalLockDate," +
					"EnableBOLPDFArchiving," +
					"BOLPDFArchivingPath," +

					// System
					"MaximumDaysToRetainLogs," +
					"EnableDebugLogging," +
					"EnableAuditLogging," +
					"AutomaticallyPrintAlarmsAndEvents," +
					"AlarmAndEventPrinter," +
					"MailServer," +
					"MailFrom," +
					"MailUserName," +
					"MailPassword," +
					"LookupMailConnectModeIndex," +
					"DialupName," +
					"SCADASystem," +
					"InhibitTemplateGraphics," +
					"RefreshInterval," +
					"InhibitEndOfDayOperations," +
					"InhibitEndOfMonthOperations," +
					"EndOfDayWarningPeriod," +
					"InhibitAutomaticPhysicalInventory," +
					"InhibitAutomaticMeterCloseout," +
					"InhibitAutomaticReportGeneration," +
					"InhibitAutomaticAdjustmentDistribution," +
					"InhibitAutomaticCloseout," +
					"InhibitTankScan," +
					"ReportDirectory," +
					"ManageReports," +
					"ManagedReportDirectory," +
					"ExportArchiveDir," +
					"ImportArchiveDir," +
					"GroupLedgerByID," +
					"MeterReconciliationToleranceIsPercent," + 
					"MeterReconciliationReportName," +
					"TranslatedHelpURL," +
					"BlockCloseOnUnpostedBol, " +
					"EnableAutomaticMovementTicketPrinting, " +
					"MovementTicketReport, " +
					"MovementTicketPrinter, " +
					"MaxOperateTabsAllowed, " +
					"CloseoutTime, " +
					"PointGroupFileExportDirectory," +
					"PointGroupDefaultFileName," +
					"EnableMovementTicketPDFArchiving, " +
					"MovementTicketFileExportDirectory," +
					"MovementTicketExportFileName," +
					// Vapor Recovery Unit (VRU)
					"VRURateLimit," +
					"VRUHourlyLimit," +
					"VRUDailyLimit," +
					"VRUYearlyLimit," +
					"VRUCurrentYearLimit," +
					"VRURateActual," +
					"VRUHourlyActual," +
					"VRUDailyActual," +
					"VRUYearlyActual," +
					"VRUCurrentYearActual," +
					"VRURateLimitEnabled," +
					"VRUHourlyLimitEnabled," +
					"VRUDailyLimitEnabled," +
					"VRUYearlyLimitEnabled," +
					"VRUCurrentYearLimitEnabled," +

					// Process I/O
					"WatchdogPeriod," +
					"LookupWatchdogModeIndex," +
					"WatchdogCounterStart," +
					"WatchdogCounterEnd," +

					// Regional Settings
					"LookupNumberGroupSizesTypeIndex," +
					"NumberDecimalSeparator," +
					"NumberGroupSeparator," +
					"ListSeparator," +
					"TimePattern," +
					"TimeSeparator," +
					"AMSymbol," +
					"PMSymbol," +
					"ShortDatePattern," +
					"DateSeparator," +
					"LongDatePattern," +
					"TwoDigitCalendarEndYear," +

					// Additional Data
					"UserData1," +
					"UserData2," +
					"UserData3," +
					"UserData4," +
					"UserData5," +
					"UserData6," +
					"UserData7," +
					"UserData8," +

					// Audit Data
					"CreatedDate, " +
					"CreatedBy, " +
					"UpdatedDate, " +
					"UpdatedBy ," +

					// Password configuration
					"MinTimeAllowedToChangePwd, " +
					"MinPwdCharacterLength, " +
					"PwdExpirationInDays, " +
					"PwdLockoutThreshold, " +
					"PwdHistoryCount, " +
					"CheckForPreviousPwd, " +
					"StrongPwdUse, " +
					"ApplyToAllSiteMembers, " +
					"InactivityDisablePeriod, " +
					"DisableArchivePeriod, " +
					"UseTankReconciliation, " +

					// Load Rack
					"EnablePasswordHint," +
					"EnablePasswordReset, " +
					"AllowUseOfSpecialChars, " +
					"EnablePeriodicSyncFlag, " +
					"PeriodicSyncIntervalMinutes, " +
					"DisableSyncTransferFlag, " +

					// Sites
					"Enterprise," +
					"OperateTabGroups, " +

					//Enterprise Query Credentials
					"EnterpriseUserId, " +
					"EnterprisePassword, " +
					"EnterpriseSite," +

					// OPC UA 
					"ServerEndPoint," +
					"SecurityMode," +
					"SecurityPolicy," +
					"MessageEncoding," +
					"UserIdentityMethod," +
					"UserId," +
					"UserPassword," +
					"UserCertificatePath," +

					// Additional System
					"MaximumDaysToRetainArchive" +
				") " +
				"VALUES (" +
					"@SiteGuid," +
					"@ID," +

					// General
					"@Number," +
					"@SPLCCode," +
					"@Address1," +
					"@Address2," +
					"@City," +
					"@State," +
					"@Zip," +
					"@Country," +
					"@Phone," +
					"@Fax," +
					"@EmergencyContact," +
					"@EmergencyPhone," +
					"@EmailAddress," +
					"@Enabled," +
					"@SiteGroupFlag," +
					"@TimeZone," +
					"@TerminalControlNumber," +
					"@InhibitLoadRackCardIns," +
					"@EnforceSingleOwner, " +
					"@InhibitBOLSummaryAutoPopulate, " +
					"@InhibitOrderSummaryAutoPopulate, " +
					"@InhibitSupplyOrderSummaryAutoPopulate, " +
					"@InhibitSiteLedgerRollup, " +
					"@Contact1Name," +
					"@Contact1Address1," +
					"@Contact1Address2," +
					"@Contact1City," +
					"@Contact1State," +
					"@Contact1Zip," +
					"@Contact1Country," +
					"@Contact1PhoneOffice," +
					"@Contact1PhoneMobile," +
					"@Contact1Fax," +
					"@Contact1EmailAddress," +
					"@Contact2Name," +
					"@Contact2Address1," +
					"@Contact2Address2," +
					"@Contact2City," +
					"@Contact2State," +
					"@Contact2Zip," +
					"@Contact2Country," +
					"@Contact2PhoneOffice," +
					"@Contact2PhoneMobile," +
					"@Contact2Fax," +
					"@Contact2EmailAddress," +
					"@Latitude, " +
					"@Longitude, " +
					"@Zoom, " +
						  "@ActiveDirectorySiteGroupGuid, " +

						  // Units
						  "@LevelUnitIndex," +
					"@TemperatureUnitIndex," +
					"@DensityUnitIndex," +
					"@PressureUnitIndex," +
					"@FlowUnitIndex," +
					"@VolumeUnitIndex," +
					"@MassUnitIndex," +
					"@AdditiveVolumeUnitIndex," +
					"@AdditiveProfileCycleAmountUnitIndex," +
					"@AdditiveProfileRateUnitIndex," +
					"@LevelDecimalPlaces," +
					"@TemperatureDecimalPlaces," +
					"@DensityDecimalPlaces," +
					"@PressureDecimalPlaces," +
					"@FlowDecimalPlaces," +
					"@VolumeDecimalPlaces," +
					"@MassDecimalPlaces," +
					"@AdditiveVolumeDecimalPlaces," +
					"@AdditiveProfileCycleAmountDecimalPlaces," +
					"@AdditiveProfileRateDecimalPlaces," +
					"@QuantityDisplayDefault," +

					// Load Rack
					"@InhibitAccessAfterHours," +
					"@InhibitMultipleCardIns," +
					"@AccessCardInRequired," +
					"@CheckSiteNumber," +
					"@PromptForCustomerCard," +
					"@PromptForTractorOrTanker," +
					"@PromptForFirstTrailer," +
					"@PromptForSecondTrailer," +
					"@PromptForThirdTrailer," +
					"@PromptForCompartment," +
					"@PromptForTransactionCompletion," +
					"@InhibitCustomerConfirmationPrompt," +
					"@RequireTrailerScully," +
					"@CardInTimeout," +
					"@EnforceDriverEquipmentMatch," +
					"@EnableAdditiveAccounting," +
					"@UseCompanyEquipmentIdentifiers," +
					"@UseLastKnownGoodTankData," +
					"@MaximumLoadAmount," +
					"@MaximumLoadTime," +
					"@MaximumIdleTime," +
					"@MaximumFlushAmount," +
					"@MaximumMeterProvingAmount," +
					"@MaximumReturnsAmount," +
					"@MaximumNumberOfActiveArms," +
					"@DriverTimeoutPeriod," +
					"@DriverWarningPeriod," +
					"@MaximumPrompts," +
					"@MaximumVehicleWeight," +
					"@LoadByNet," +
					"@PromptForShipmentNumber," +
					"@MaximumProductTemperature," +
					"@ListEquipment," +
					"@DeferStationChanges," +
					"@PromptForReturns," +
					"@PromptForTruckCard," +
					"@StartingShortCardNumber," +
					"@UseShortCardNumber," +
					"@ExcessVarianceCount," +
					"@ExcessVarianceTolerance," +
					"@SecondaryStorageFillMethod," +
					"@EnforceSalesOrderLimit," +

					"@LeakDetectionQuietSamples," +
					"@LeakDetectionQuietTime," +
					"@LeakDetectionQuietTimeFactor," +
					"@LeakDetectionUseMinWait," +
					"@LeakDetectionReport," +
					"@LeakDetectionPrinter," +

					// Transactions
					"@InhibitBOLWithBrokenBlends," +
					"@InhibitBOLWithImproperAdditization," +
					"@InhibitOverweightBOL," +
					"@ExceptionBOLPrinter," +
					"@EnableAutomaticBOLPrinting," +
					"@AutomaticBOLStartNumber," +
					"@AutomaticBOLEndNumber," +
					"@AutomaticBOLNextNumber," +
					"@SeparateManualBOLNumbering," +
					"@ManualBOLStartNumber," +
					"@ManualBOLEndNumber," +
					"@ManualBOLNextNumber," +
					"@TransactionStartNumber," +
					"@TransactionEndNumber," +
					"@TransactionNextNumber," +
					"@OrderStartNumber," +
					"@OrderEndNumber," +
					"@OrderNextNumber," +
					"@InvoiceStartNumber," +
					"@InvoiceEndNumber," +
					"@InvoiceNextNumber," +
					"@NumberPrefix," +
					"@OpenTransactionWindow," +
					"@AdministrativeLockDate," +
					"@OperationalLockDate," +
					"@EnableBOLPDFArchiving," +
					"@BOLPDFArchivingPath," +

					// System
					"@MaximumDaysToRetainLogs," +
					"@EnableDebugLogging," +
					"@EnableAuditLogging," +
					"@AutomaticallyPrintAlarmsAndEvents," +
					"@AlarmAndEventPrinter," +
					"@MailServer," +
					"@MailFrom," +
					"@MailUserName," +
					"@MailPassword," +
					"@MailConnectMode," +
					"@DialupName," +
					"@SCADASystem," +
					"@InhibitTemplateGraphics," +
					"@RefreshInterval," +
					"@InhibitEndOfDayOperations," +
					"@InhibitEndOfMonthOperations," +
					"@EndOfDayWarningPeriod," +
					"@InhibitAutomaticPhysicalInventory," +
					"@InhibitAutomaticMeterCloseout," +
					"@InhibitAutomaticReportGeneration," +
					"@InhibitAutomaticAdjustmentDistribution," +
					"@InhibitAutomaticCloseout," +
					"@InhibitTankScan," +
					"@ReportDirectory," +
					"@ManageReports," +
					"@ManagedReportDirectory," +
					"@ExportArchiveDir," +
					"@ImportArchiveDir," +
					"@GroupLedgerByID," +
					"@MeterReconciliationToleranceIsPercent," +
					"@MeterReconciliationReportName," +
					"@TranslatedHelpURL," +
					"@BlockCloseOnUnpostedBol," +
					"@EnableAutomaticMovementTicketPrinting," +
					"@MovementTicketReport," +
					"@MovementTicketPrinter," +
					"@MaxOperateTabsAllowed," +
					"@CloseoutTime," +
					"@PointGroupFileExportDirectory," +
					"@PointGroupDefaultFileName," +
					"@EnableMovementTicketPDFArchiving," +
					"@MovementTicketFileExportDirectory," +
					"@MovementTicketExportFileName," +

					// Vapor Recovery Unit (VRU)
					"@VRURateLimit," +
					"@VRUHourlyLimit," +
					"@VRUDailyLimit," +
					"@VRUYearlyLimit," +
					"@VRUCurrentYearLimit," +
					"@VRURateActual," +
					"@VRUHourlyActual," +
					"@VRUDailyActual," +
					"@VRUYearlyActual," +
					"@VRUCurrentYearActual," +
					"@VRURateLimitEnabled," +
					"@VRUHourlyLimitEnabled," +
					"@VRUDailyLimitEnabled," +
					"@VRUYearlyLimitEnabled," +
					"@VRUCurrentYearLimitEnabled," +

					// Process I/O
					"@WatchdogPeriod," +
					"@WatchdogMode," +
					"@WatchdogCounterStart," +
					"@WatchdogCounterEnd," +

					// Regional Settings
					"@NumberGroupSizesType," +
					"@NumberDecimalSeparator," +
					"@NumberGroupSeparator," +
					"@ListSeparator," +
					"@TimePattern," +
					"@TimeSeparator," +
					"@AMSymbol," +
					"@PMSymbol," +
					"@ShortDatePattern," +
					"@DateSeparator," +
					"@LongDatePattern," +
					"@TwoDigitCalendarEndYear," +

					// Additional Data
					"@UserData1," +
					"@UserData2," +
					"@UserData3," +
					"@UserData4," +
					"@UserData5," +
					"@UserData6," +
					"@UserData7," +
					"@UserData8," +

					// Audit Data
					"@CreatedDate, " +
					"@CreatedBy, " +
					"@UpdatedDate, " +
					"@UpdatedBy ," +

					// Password configuration
					"@MinTimeAllowedToChangePwd, " +
					"@MinPwdCharacterLength, " +
					"@PwdExpirationInDays, " +
					"@PwdLockoutThreshold, " +
					"@PwdHistoryCount, " +
					"@CheckForPreviousPwd, " +
					"@StrongPwdUse, " +
					"@ApplyToAllSiteMembers, " +
					"@InactivityDisablePeriod, " +
					"@DisableArchivePeriod, " +
					"@UseTankReconciliation, " +

					// Load Rack
					"@EnablePasswordHint, " +
					"@EnablePasswordReset, " +
					"@AllowUseOfSpecialChars, " +
					"@EnablePeriodicSyncFlag, " +
					"@PeriodicSyncIntervalMinutes, " +
					"@DisableSyncTransferFlag," +

					// Sites
					"@Enterprise," +
					"@OperateTabGroups," +

						  //Enterprise Query Credentials
						  "@EnterpriseUserId, " +
						  "@EnterprisePassword, " +
						  "@EnterpriseSite," +

					// OPC UA
					"@ServerEndPoint," +
					"@SecurityMode," +
					"@SecurityPolicy," +
					"@MessageEncoding," +
					"@UserIdentityMethod," +
					"@UserId," +
					"@UserPassword," +
					"@UserCertificatePath," +

					// Additional System
					"@MaximumDaysToRetainArchive" +
					")" +
					" INSERT INTO tblSitesAncillaryData ("+
					"SiteGuid," + 
					"InventoryTransactionAliasGuid," +
					"AdjustmentTransactionAliasGuid," +
					"IATAGuid," +
					"NoteGuid," +
					"CreatedDate," +
					"CreatedBy," +
					"UpdatedDate," +
					"UpdatedBy)" +
					" VALUES (" +
					"@SiteGuid," +
					"@IATAGuid, " +
					"@InventoryTransactionAliasGuid," +
					"@AdjustmentTransactionAliasGuid," +
					"@NoteGuid," +
					"@CreatedDate," +
					"@CreatedBy," +
					"@UpdatedDate," +
					"@UpdatedBy" +
					")";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@ID", this.ID);

			// General
			cmd.Parameters.AddWithValue("@Number", this.Number);
			cmd.Parameters.AddWithValue("@SPLCCode", this.SPLCCode);
			cmd.Parameters.AddWithValue("@Address1", this.Address1);
			cmd.Parameters.AddWithValue("@Address2", this.Address2);
			cmd.Parameters.AddWithValue("@City", this.City);
			cmd.Parameters.AddWithValue("@State", this.State);
			cmd.Parameters.AddWithValue("@Zip", this.Zip);
			cmd.Parameters.AddWithValue("@Country", this.Country);
			cmd.Parameters.AddWithValue("@Phone", this.Phone);
			cmd.Parameters.AddWithValue("@Fax", this.Fax);
			cmd.Parameters.AddWithValue("@EmergencyContact", this.EmergencyContact);
			cmd.Parameters.AddWithValue("@EmergencyPhone", this.EmergencyPhone);
			cmd.Parameters.AddWithValue("@EmailAddress", this.EmailAddress);
			cmd.Parameters.AddWithValue("@Enabled", ((this.Enabled) ? 1 : 0));
			cmd.Parameters.AddWithValue("@SiteGroupFlag", ((this.SiteGroup) ? 1 : 0));
			cmd.Parameters.AddWithValue("@TimeZone", this.TimeZone);
			cmd.Parameters.AddWithValue("@TerminalControlNumber", this.TerminalControlNumber);
			cmd.Parameters.AddWithValue("@InhibitLoadRackCardIns", (this.InhibitLoadRackCardIns ? 1 : 0));
			cmd.Parameters.AddWithValue("@EnforceSingleOwner", (this.EnforceSingleOwner ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitBOLSummaryAutoPopulate", (this.InhibitBOLSummaryAutoPopulate ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitOrderSummaryAutoPopulate", (this.InhibitOrderSummaryAutoPopulate ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitSupplyOrderSummaryAutoPopulate", (this.InhibitSupplyOrderSummaryAutoPopulate ? 1 : 0));
			cmd.Parameters.Add(DataObject.NewGuidParameter("@IATAGuid", this.IATAGuid, true));
			cmd.Parameters.AddWithValue("@InhibitSiteLedgerRollup", (this.InhibitSiteLedgerRollup ? 1 : 0));
			cmd.Parameters.AddWithValue("@Contact1Name", string.IsNullOrEmpty(this._Contact1Name) ? (object)DBNull.Value : this._Contact1Name);
			cmd.Parameters.AddWithValue("@Contact1Address1", string.IsNullOrEmpty(this._Contact1Address1) ? (object)DBNull.Value : this._Contact1Address1);
			cmd.Parameters.AddWithValue("@Contact1Address2", string.IsNullOrEmpty(this._Contact1Address2) ? (object)DBNull.Value : this._Contact1Address2);
			cmd.Parameters.AddWithValue("@Contact1City", string.IsNullOrEmpty(this._Contact1City) ? (object)DBNull.Value : this._Contact1City);
			cmd.Parameters.AddWithValue("@Contact1State", string.IsNullOrEmpty(this._Contact1State) ? (object)DBNull.Value : this._Contact1State);
			cmd.Parameters.AddWithValue("@Contact1Zip", string.IsNullOrEmpty(this._Contact1Zip) ? (object)DBNull.Value : this._Contact1Zip);
			cmd.Parameters.AddWithValue("@Contact1Country", string.IsNullOrEmpty(this._Contact1Country) ? (object)DBNull.Value : this._Contact1Country);
			cmd.Parameters.AddWithValue("@Contact1PhoneOffice", string.IsNullOrEmpty(this._Contact1PhoneOffice) ? (object)DBNull.Value : this._Contact1PhoneOffice);
			cmd.Parameters.AddWithValue("@Contact1PhoneMobile", string.IsNullOrEmpty(this._Contact1PhoneMobile) ? (object)DBNull.Value : this._Contact1PhoneMobile);
			cmd.Parameters.AddWithValue("@Contact1Fax", string.IsNullOrEmpty(this._Contact1Fax) ? (object)DBNull.Value : this._Contact1Fax);
			cmd.Parameters.AddWithValue("@Contact1EmailAddress", string.IsNullOrEmpty(this._Contact1EmailAddress) ? (object)DBNull.Value : this._Contact1EmailAddress);
			cmd.Parameters.AddWithValue("@Contact2Name", string.IsNullOrEmpty(this._Contact2Name) ? (object)DBNull.Value : this._Contact2Name);
			cmd.Parameters.AddWithValue("@Contact2Address1", string.IsNullOrEmpty(this._Contact2Address1) ? (object)DBNull.Value : this._Contact2Address1);
			cmd.Parameters.AddWithValue("@Contact2Address2", string.IsNullOrEmpty(this._Contact2Address2) ? (object)DBNull.Value : this._Contact2Address2);
			cmd.Parameters.AddWithValue("@Contact2City", string.IsNullOrEmpty(this._Contact2City) ? (object)DBNull.Value : this._Contact2City);
			cmd.Parameters.AddWithValue("@Contact2State", string.IsNullOrEmpty(this._Contact2State) ? (object)DBNull.Value : this._Contact2State);
			cmd.Parameters.AddWithValue("@Contact2Zip", string.IsNullOrEmpty(this._Contact2Zip) ? (object)DBNull.Value : this._Contact2Zip);
			cmd.Parameters.AddWithValue("@Contact2Country", string.IsNullOrEmpty(this._Contact2Country) ? (object)DBNull.Value : this._Contact2Country);
			cmd.Parameters.AddWithValue("@Contact2PhoneOffice", string.IsNullOrEmpty(this._Contact2PhoneOffice) ? (object)DBNull.Value : this._Contact2PhoneOffice);
			cmd.Parameters.AddWithValue("@Contact2PhoneMobile", string.IsNullOrEmpty(this._Contact2PhoneMobile) ? (object)DBNull.Value : this._Contact2PhoneMobile);
			cmd.Parameters.AddWithValue("@Contact2Fax", string.IsNullOrEmpty(this._Contact2Fax) ? (object)DBNull.Value : this._Contact2Fax);
			cmd.Parameters.AddWithValue("@Contact2EmailAddress", string.IsNullOrEmpty(this._Contact2EmailAddress) ? (object)DBNull.Value : this._Contact2EmailAddress);

			if (this.latitude == null)
			{
				cmd.Parameters.AddWithValue("@Latitude", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@Latitude", this.latitude.Value); 
			}

			if (this.longitude == null)
			{
				cmd.Parameters.AddWithValue("@Longitude", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@Longitude", this.longitude.Value);
			}

			if (this.zoom == null)
			{
				cmd.Parameters.AddWithValue("@Zoom", DBNull.Value);
			}
			else
			{
				cmd.Parameters.AddWithValue("@Zoom", this.zoom.Value);
			}

			 if (this.activeDirectorySiteGroupGuid == null || this.activeDirectorySiteGroupGuid == Guid.Empty)
			 {
					 cmd.Parameters.AddWithValue("@ActiveDirectorySiteGroupGuid", DBNull.Value);
				}
			 else
				{
					 cmd.Parameters.AddWithValue("@ActiveDirectorySiteGroupGuid", this.activeDirectorySiteGroupGuid);
				}

				// Units
				cmd.Parameters.AddWithValue("@LevelUnitIndex", ((int)this.LevelUnits));
			cmd.Parameters.AddWithValue("@TemperatureUnitIndex", ((int)this.TemperatureUnits));
			cmd.Parameters.AddWithValue("@DensityUnitIndex", ((int)this.DensityUnits));
			cmd.Parameters.AddWithValue("@PressureUnitIndex", ((int)this.PressureUnits));
			cmd.Parameters.AddWithValue("@FlowUnitIndex", ((int)this.FlowUnits));
			cmd.Parameters.AddWithValue("@VolumeUnitIndex", ((int)this.VolumeUnits));
			cmd.Parameters.AddWithValue("@MassUnitIndex", ((int)this.MassUnits));
			cmd.Parameters.AddWithValue("@AdditiveVolumeUnitIndex", ((int)this.AdditiveVolumeUnits));
			cmd.Parameters.AddWithValue("@AdditiveProfileCycleAmountUnitIndex", ((int)this.AdditiveProfileCycleAmountUnits));
			cmd.Parameters.AddWithValue("@AdditiveProfileRateUnitIndex", ((int)this.AdditiveProfileRateUnits));
			cmd.Parameters.AddWithValue("@LevelDecimalPlaces", (int)this._LevelDecimalPlaces);
			cmd.Parameters.AddWithValue("@TemperatureDecimalPlaces", (int)this._TemperatureDecimalPlaces);
			cmd.Parameters.AddWithValue("@DensityDecimalPlaces", (int)this._DensityDecimalPlaces);
			cmd.Parameters.AddWithValue("@PressureDecimalPlaces", (int)this._PressureDecimalPlaces);
			cmd.Parameters.AddWithValue("@FlowDecimalPlaces", (int)this._FlowDecimalPlaces);
			cmd.Parameters.AddWithValue("@VolumeDecimalPlaces", (int)this._VolumeDecimalPlaces);
			cmd.Parameters.AddWithValue("@MassDecimalPlaces", (int)this._MassDecimalPlaces);
			cmd.Parameters.AddWithValue("@AdditiveVolumeDecimalPlaces", (int)this._AdditiveVolumeDecimalPlaces);
			cmd.Parameters.AddWithValue("@AdditiveProfileCycleAmountDecimalPlaces", (int)this._AdditiveProfileCycleAmountDecimalPlaces);
			cmd.Parameters.AddWithValue("@AdditiveProfileRateDecimalPlaces", (int)this._AdditiveProfileRateDecimalPlaces);
			cmd.Parameters.AddWithValue("@QuantityDisplayDefault", ((int)this.QuantityDisplayDefault));

			// Load Rack
			cmd.Parameters.AddWithValue("@InhibitAccessAfterHours", (this.InhibitAccessAfterHours ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitMultipleCardIns", (this.InhibitMultipleCardIns ? 1 : 0));
			cmd.Parameters.AddWithValue("@AccessCardInRequired", (this.AccessCardInRequired ? 1 : 0));
			cmd.Parameters.AddWithValue("@CheckSiteNumber", (this.CheckSiteNumber ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForCustomerCard", (this.PromptForCustomerCard ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForTractorOrTanker", (this.PromptForTractorOrTanker ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForFirstTrailer", (this.PromptForFirstTrailer ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForSecondTrailer", (this.PromptForSecondTrailer ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForThirdTrailer", (this.PromptForThirdTrailer ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForCompartment", (this.PromptForCompartment ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForTransactionCompletion", (this.PromptForTransactionCompletion ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitCustomerConfirmationPrompt", (this.InhibitCustomerConfirmationPrompt ? 1 : 0));
			cmd.Parameters.AddWithValue("@RequireTrailerScully", (this.RequireTrailerScully ? 1 : 0));
			cmd.Parameters.AddWithValue("@CardInTimeout", (this._CardInTimeout));
			cmd.Parameters.AddWithValue("@EnforceDriverEquipmentMatch", (this.EnforceDriverEquipmentMatch ? 1 : 0));
			cmd.Parameters.AddWithValue("@EnableAdditiveAccounting", (this.EnableAdditiveAccounting ? 1 : 0));
			cmd.Parameters.AddWithValue("@UseCompanyEquipmentIdentifiers", (this.UseCompanyEquipmentIdentifiers ? 1 : 0));
			cmd.Parameters.AddWithValue("@UseLastKnownGoodTankData", (this.UseLastKnownGoodTankData ? 1 : 0));
			cmd.Parameters.AddWithValue("@MaximumLoadAmount", this._MaximumLoadAmount.SIValue);
			cmd.Parameters.AddWithValue("@MaximumLoadTime", this._MaximumLoadTime);
			cmd.Parameters.AddWithValue("@MaximumIdleTime", this._MaximumIdleTime);
			cmd.Parameters.AddWithValue("@MaximumFlushAmount", this._MaximumFlushAmount.SIValue);
			cmd.Parameters.AddWithValue("@MaximumMeterProvingAmount", this._MaximumMeterProvingAmount.SIValue);
			cmd.Parameters.AddWithValue("@MaximumReturnsAmount", this._MaximumReturnsAmount.SIValue);
			cmd.Parameters.AddWithValue("@MaximumNumberOfActiveArms", this._MaximumNumberOfActiveArms);
			cmd.Parameters.AddWithValue("@DriverTimeoutPeriod", this._DriverTimeoutPeriod);
			cmd.Parameters.AddWithValue("@DriverWarningPeriod", this._DriverWarningPeriod);
			cmd.Parameters.AddWithValue("@MaximumPrompts", this._MaximumPrompts);
			cmd.Parameters.Add(DataObject.NewGuidParameter("@InventoryTransactionAliasGuid", this.InventoryTransactionAliasGuid, true));
			cmd.Parameters.Add(DataObject.NewGuidParameter("@AdjustmentTransactionAliasGuid", this.AdjustmentTransactionAliasGuid, true));
			cmd.Parameters.AddWithValue("@MaximumVehicleWeight", this._MaximumVehicleWeight.SIValue);
			cmd.Parameters.AddWithValue("@LoadByNet", (this.LoadByNet ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForShipmentNumber", (this.PromptForShipmentNumber ? 1 : 0));
			cmd.Parameters.AddWithValue("@MaximumProductTemperature", this._MaximumProductTemperature.SIValue);
			cmd.Parameters.AddWithValue("@ListEquipment", (this.ListEquipment ? 1 : 0));
			cmd.Parameters.AddWithValue("@DeferStationChanges", (this.DeferStationChanges ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForReturns", (this.PromptForReturns ? 1 : 0));
			cmd.Parameters.AddWithValue("@PromptForTruckCard", (this.PromptForTruckCard ? 1 : 0));
			cmd.Parameters.AddWithValue("@StartingShortCardNumber", this._StartingShortCardNumber);
			cmd.Parameters.AddWithValue("@UseShortCardNumber", (this.UseShortCardNumber ? 1 : 0));
			cmd.Parameters.AddWithValue("@ExcessVarianceCount", (int)this._ExcessVarianceCount);
			cmd.Parameters.AddWithValue("@ExcessVarianceTolerance", this._ExcessVarianceTolerance);
			cmd.Parameters.AddWithValue("@SecondaryStorageFillMethod", ((int)this.SecondaryStorageFillMethod));
			cmd.Parameters.AddWithValue("@EnforceSalesOrderLimit", (this.EnforceSalesOrderLimit ? 1 : 0));

			cmd.Parameters.AddWithValue("@LeakDetectionQuietSamples", (int)this._LeakDetectionQuietSamples);
			cmd.Parameters.AddWithValue("@LeakDetectionQuietTime", (int)this._LeakDetectionQuietTime);
			cmd.Parameters.AddWithValue("@LeakDetectionQuietTimeFactor", (int)this._LeakDetectionQuietTimeFactor);
			cmd.Parameters.AddWithValue("@LeakDetectionUseMinWait", (this._LeakDetectionUseMinWait ? 1 : 0));
			cmd.Parameters.AddWithValue("@LeakDetectionReport", this._LeakDetectionReport);
			cmd.Parameters.AddWithValue("@LeakDetectionPrinter", this._LeakDetectionPrinter);

			// Transactions
			cmd.Parameters.AddWithValue("@InhibitBOLWithBrokenBlends", (this.InhibitBOLWithBrokenBlends ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitBOLWithImproperAdditization", (this.InhibitBOLWithImproperAdditization ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitOverweightBOL", (this.InhibitOverweightBOL ? 1 : 0));
			cmd.Parameters.AddWithValue("@ExceptionBOLPrinter", this.ExceptionBOLPrinter);
			cmd.Parameters.AddWithValue("@EnableAutomaticBOLPrinting", (this.EnableAutomaticBOLPrinting ? 1 : 0));
			cmd.Parameters.AddWithValue("@AutomaticBOLStartNumber", this._AutomaticBOLStartNumber);
			cmd.Parameters.AddWithValue("@AutomaticBOLEndNumber", this._AutomaticBOLEndNumber);
			cmd.Parameters.AddWithValue("@AutomaticBOLNextNumber", this._AutomaticBOLNextNumber);
			cmd.Parameters.AddWithValue("@SeparateManualBOLNumbering", (this.SeparateManualBOLNumbering ? 1 : 0));
			cmd.Parameters.AddWithValue("@ManualBOLStartNumber", this._ManualBOLStartNumber);
			cmd.Parameters.AddWithValue("@ManualBOLEndNumber", this._ManualBOLEndNumber);
			cmd.Parameters.AddWithValue("@ManualBOLNextNumber", this._ManualBOLNextNumber);
			cmd.Parameters.AddWithValue("@TransactionStartNumber", this._TransactionStartNumber);
			cmd.Parameters.AddWithValue("@TransactionEndNumber", this._TransactionEndNumber);
			cmd.Parameters.AddWithValue("@TransactionNextNumber", this._TransactionNextNumber);
			cmd.Parameters.AddWithValue("@OrderStartNumber", this._OrderStartNumber);
			cmd.Parameters.AddWithValue("@OrderEndNumber", this._OrderEndNumber);
			cmd.Parameters.AddWithValue("@OrderNextNumber", this._OrderNextNumber);
			cmd.Parameters.AddWithValue("@InvoiceStartNumber", this._InvoiceStartNumber);
			cmd.Parameters.AddWithValue("@InvoiceEndNumber", this._InvoiceEndNumber);
			cmd.Parameters.AddWithValue("@InvoiceNextNumber", this._InvoiceNextNumber);
			cmd.Parameters.AddWithValue("@NumberPrefix", this.NumberPrefix);
			cmd.Parameters.AddWithValue("@OpenTransactionWindow", this._OpenTransactionWindow);

			// Truncate the Time part
			cmd.Parameters.AddWithValue("@AdministrativeLockDate", TimeConverter.ToDate(this._AdministrativeLockDate.Value));
			cmd.Parameters.AddWithValue("@OperationalLockDate", this._OperationalLockDate.Value);
			cmd.Parameters.AddWithValue("@EnableBOLPDFArchiving", (this._EnableBOLPDFArchiving ? 1 : 0));
			cmd.Parameters.AddWithValue("@BOLPDFArchivingPath", this._BOLPDFArchivingPath);

			// System
			cmd.Parameters.AddWithValue("@MaximumDaysToRetainLogs", this._MaximumDaysToRetainLogs);
			cmd.Parameters.AddWithValue("@EnableDebugLogging", (this.EnableDebugLogging ? 1 : 0));
			cmd.Parameters.AddWithValue("@EnableAuditLogging", (this.EnableAuditLogging ? 1 : 0));
			cmd.Parameters.AddWithValue("@AutomaticallyPrintAlarmsAndEvents", (this.AutomaticallyPrintAlarmsAndEvents ? 1 : 0));
			cmd.Parameters.AddWithValue("@AlarmAndEventPrinter", this.AlarmAndEventPrinter);
			cmd.Parameters.AddWithValue("@MailServer", this.MailServer);
			cmd.Parameters.AddWithValue("@MailFrom", this.MailFrom);
			cmd.Parameters.AddWithValue("@MailUserName", this.MailUserName);
			cmd.Parameters.AddWithValue("@MailPassword", this.MailPassword);
			cmd.Parameters.AddWithValue("@MailConnectMode", ((int)this.MailConnectMode));
			cmd.Parameters.AddWithValue("@DialupName", this.DialupName);
			cmd.Parameters.AddWithValue("@SCADASystem", this.SCADASystem);
			cmd.Parameters.AddWithValue("@InhibitTemplateGraphics", (this.InhibitTemplateGraphics ? 1 : 0));
			cmd.Parameters.AddWithValue("@RefreshInterval", this._RefreshInterval);
			cmd.Parameters.AddWithValue("@InhibitEndOfDayOperations", (this.InhibitEndOfDayOperations ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitEndOfMonthOperations", (this.InhibitEndOfMonthOperations ? 1 : 0));
			cmd.Parameters.AddWithValue("@EndOfDayWarningPeriod", this._EndOfDayWarningPeriod);
			cmd.Parameters.AddWithValue("@InhibitAutomaticPhysicalInventory", (this.InhibitAutomaticPhysicalInventory ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitAutomaticMeterCloseout", (this.InhibitAutomaticMeterCloseout ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitAutomaticReportGeneration", (this.InhibitAutomaticReportGeneration ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitAutomaticAdjustmentDistribution", (this.InhibitAutomaticAdjustmentDistribution ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitAutomaticCloseout", (this.InhibitAutomaticCloseout ? 1 : 0));
			cmd.Parameters.AddWithValue("@BlockCloseOnUnpostedBol", (this.BlockCloseOnUnpostedBol ? 1 : 0));
			cmd.Parameters.AddWithValue("@InhibitTankScan", (this.InhibitTankScan ? 1 : 0));
			cmd.Parameters.AddWithValue("@ReportDirectory", this.ReportDirectory);
			cmd.Parameters.AddWithValue("@ManageReports", (this.ManageReports ? 1 : 0));
			cmd.Parameters.AddWithValue("@ManagedReportDirectory", this.ManagedReportDirectory);
			cmd.Parameters.AddWithValue("@ExportArchiveDir", this.ExportArchiveDir);
			cmd.Parameters.AddWithValue("@ImportArchiveDir", this.ImportArchiveDir);
			cmd.Parameters.AddWithValue("@GroupLedgerByID", (this.GroupLedgerByID ? 1 : 0));
			cmd.Parameters.AddWithValue("@MeterReconciliationToleranceIsPercent", (this.MeterReconciliationToleranceIsPercent ? 1 : 0));
			cmd.Parameters.AddWithValue("@MeterReconciliationReportName", this.MeterReconciliationReportName);
			cmd.Parameters.AddWithValue("@TranslatedHelpURL", this.TranslatedHelpURL);
			cmd.Parameters.AddWithValue("@EnableAutomaticMovementTicketPrinting", this.EnableAutomaticMovementTicketPrinting);
			cmd.Parameters.AddWithValue("@MovementTicketReport", this.MovementTicketReportName);
			cmd.Parameters.AddWithValue("@MovementTicketPrinter", this.MovementTicketPrinter);
			cmd.Parameters.AddWithValue("@MaxOperateTabsAllowed", this.MaxOperateTabsAllowed);
			cmd.Parameters.AddWithValue("@CloseoutTime", this.CloseoutTime == null ? (object)DBNull.Value : this.CloseoutTime.Value);
			cmd.Parameters.AddWithValue("@PointGroupFileExportDirectory", this.PointGroupFileExportDirectory);
			cmd.Parameters.AddWithValue("@PointGroupDefaultFileName", this.PointGroupDefaultFileName);
			cmd.Parameters.AddWithValue("@EnableMovementTicketPDFArchiving", this.EnableMovementTicketPDFArchiving);
			cmd.Parameters.AddWithValue("@MovementTicketFileExportDirectory", this.MovementTicketFileExportDirectory);
			cmd.Parameters.AddWithValue("@MovementTicketExportFileName", this.MovementTicketExportFileName);


			// Vapor Recovery Unit (VRU)
			cmd.Parameters.AddWithValue("@VRURateLimit", this._VRURateLimit.SIValue);
			cmd.Parameters.AddWithValue("@VRUHourlyLimit", this._VRUHourlyLimit.SIValue);
			cmd.Parameters.AddWithValue("@VRUDailyLimit", this._VRUDailyLimit.SIValue);
			cmd.Parameters.AddWithValue("@VRUYearlyLimit", this._VRUYearlyLimit.SIValue);
			cmd.Parameters.AddWithValue("@VRUCurrentYearLimit", this._VRUCurrentYearLimit.SIValue);
			cmd.Parameters.AddWithValue("@VRURateActual", this._VRURateActual.SIValue);
			cmd.Parameters.AddWithValue("@VRUHourlyActual", this._VRUHourlyActual.SIValue);
			cmd.Parameters.AddWithValue("@VRUDailyActual", this._VRUDailyActual.SIValue);
			cmd.Parameters.AddWithValue("@VRUYearlyActual", this._VRUYearlyActual.SIValue);
			cmd.Parameters.AddWithValue("@VRUCurrentYearActual", this._VRUCurrentYearActual.SIValue);
			cmd.Parameters.AddWithValue("@VRURateLimitEnabled", (this.VRURateLimitEnabled ? 1 : 0));
			cmd.Parameters.AddWithValue("@VRUHourlyLimitEnabled", (this.VRUHourlyLimitEnabled ? 1 : 0));
			cmd.Parameters.AddWithValue("@VRUDailyLimitEnabled", (this.VRUDailyLimitEnabled ? 1 : 0));
			cmd.Parameters.AddWithValue("@VRUYearlyLimitEnabled", (this.VRUYearlyLimitEnabled ? 1 : 0));
			cmd.Parameters.AddWithValue("@VRUCurrentYearLimitEnabled", (this.VRUCurrentYearLimitEnabled ? 1 : 0));

			// Process I/O
			cmd.Parameters.AddWithValue("@WatchdogPeriod", this.WatchdogPeriod);
			cmd.Parameters.AddWithValue("@WatchdogMode", ((int)this.WatchdogMode));
			cmd.Parameters.AddWithValue("@WatchdogCounterStart", this._WatchdogCounterStart);
			cmd.Parameters.AddWithValue("@WatchdogCounterEnd", this._WatchdogCounterEnd);

			// Regional Settings
			cmd.Parameters.AddWithValue("@NumberGroupSizesType", ((int)this._NumberGroupSizesType));
			cmd.Parameters.AddWithValue("@NumberDecimalSeparator", this._NumberDecimalSeparator);
			cmd.Parameters.AddWithValue("@NumberGroupSeparator", this._NumberGroupSeparator);
			cmd.Parameters.AddWithValue("@ListSeparator", this._ListSeparator);
			cmd.Parameters.AddWithValue("@TimePattern", this._TimePattern);
			cmd.Parameters.AddWithValue("@TimeSeparator", this._TimeSeparator);
			cmd.Parameters.AddWithValue("@AMSymbol", this._AMSymbol);
			cmd.Parameters.AddWithValue("@PMSymbol", this._PMSymbol);
			cmd.Parameters.AddWithValue("@ShortDatePattern", this._ShortDatePattern);
			cmd.Parameters.AddWithValue("@DateSeparator", this._DateSeparator);
			cmd.Parameters.AddWithValue("@LongDatePattern", this._LongDatePattern);
			cmd.Parameters.AddWithValue("@TwoDigitCalendarEndYear", this._TwoDigitCalendarEndYear);

			// Additional Data
			cmd.Parameters.AddWithValue("@UserData1", this.UserData[0]);
			cmd.Parameters.AddWithValue("@UserData2", this.UserData[1]);
			cmd.Parameters.AddWithValue("@UserData3", this.UserData[2]);
			cmd.Parameters.AddWithValue("@UserData4", this.UserData[3]);
			cmd.Parameters.AddWithValue("@UserData5", this.UserData[4]);
			cmd.Parameters.AddWithValue("@UserData6", this.UserData[5]);
			cmd.Parameters.AddWithValue("@UserData7", this.UserData[6]);
			cmd.Parameters.AddWithValue("@UserData8", this.UserData[7]);

			// Notes 
			cmd.Parameters.AddWithValue("@NoteGuid", (this.NoteGuid == Guid.Empty ? (object)DBNull.Value : this.NoteGuid));

			// Audit Data
			cmd.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			cmd.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			cmd.Parameters.AddWithValue("@UpdatedBy ", this._UpdatedBy);

			// Password configuration
			cmd.Parameters.AddWithValue("@MinTimeAllowedToChangePwd", this.minTimeAllowedToChangePwd);
			cmd.Parameters.AddWithValue("@MinPwdCharacterLength", this.minPwdCharacterLength);
			cmd.Parameters.AddWithValue("@PwdExpirationInDays", this.pwdExpirationInDays);
			cmd.Parameters.AddWithValue("@PwdLockoutThreshold", this.pwdLockoutThreshold);
			cmd.Parameters.AddWithValue("@PwdHistoryCount", this.pwdHistoryCount);
			cmd.Parameters.AddWithValue("@CheckForPreviousPwd", (this.checkForPreviousPwd ? 1 : 0));
			cmd.Parameters.AddWithValue("@StrongPwdUse", this.StrongPwdUse);
			cmd.Parameters.AddWithValue("@ApplyToAllSiteMembers", (this.applyToAllSiteMembers ? 1 : 0));
			cmd.Parameters.AddWithValue("@InactivityDisablePeriod", this.inactivityDisablePeriod);
			cmd.Parameters.AddWithValue("@DisableArchivePeriod", this.disableArchivePeriod);
			cmd.Parameters.AddWithValue("@UseTankReconciliation", (this.useTankReconciliation ? 1 : 0));

			//Password Hint/Forgotten Password
			cmd.Parameters.AddWithValue("@EnablePasswordHint", (this._EnablePasswordHint ? 1 : 0));
			cmd.Parameters.AddWithValue("@EnablePasswordReset", (this._EnablePasswordReset ? 1 : 0));
			cmd.Parameters.AddWithValue("@AllowUseOfSpecialChars", (this._AllowUseOfSpecialChars ? 1 : 0));

			// These are not exposed via the Site interface and are defaulted to 0
			cmd.Parameters.AddWithValue("@EnablePeriodicSyncFlag", 0);
			cmd.Parameters.AddWithValue("@PeriodicSyncIntervalMinutes", 0);
			cmd.Parameters.AddWithValue("@DisableSyncTransferFlag", 0);

			// Only on the Sites tab
			cmd.Parameters.AddWithValue("@Enterprise", this._Enterprise);
			cmd.Parameters.AddWithValue("@OperateTabGroups", this._OperateTabGroups);

				//Enterprise Query Credentials
			 cmd.Parameters.AddWithValue("@EnterpriseUserId", this._EnterpriseUserId);
				if (!string.IsNullOrEmpty(this._EnterprisePassword))
				{
					 cmd.Parameters.AddWithValue("@EnterprisePassword", UserClass.encode(this._EnterprisePassword, this.SiteGuid));
				}
				else
				{
					 cmd.Parameters.Add("@EnterprisePassword", SqlDbType.VarBinary).Value = DBNull.Value;
				}
				cmd.Parameters.AddWithValue("@EnterpriseSite", this._EnterpriseSite);

			// OPC UA

			cmd.Parameters.AddWithValue("@ServerEndPoint", this.serverEndPoint);
			cmd.Parameters.AddWithValue("@SecurityMode", this.securityMode);
			cmd.Parameters.AddWithValue("@SecurityPolicy", this.securityPolicy);
			cmd.Parameters.AddWithValue("@MessageEncoding", this.messageEncoding);
			cmd.Parameters.AddWithValue("@UserIdentityMethod", this.userIdentityMethod);
			cmd.Parameters.AddWithValue("@UserId", this.userId);
			cmd.Parameters.AddWithValue("@UserPassword", this.userPassword);
			cmd.Parameters.AddWithValue("@UserCertificatePath", this.userCertificatePath);

			// Additional System
			cmd.Parameters.AddWithValue("@MaximumDaysToRetainArchive", this.MaximumDaysToRetainArchive);
		}

		public void UpdateSQL(SqlCommand cmd, DATA_TYPE type)
		{
			if (type == DATA_TYPE.CONFIG)
			{
				cmd.CommandText = "UPDATE tblSites " +

					// General
					"SET ID = @ID, " +
					"Number = @Number, " +
					"SPLCCode = @SPLCCode, " +
					"Address1 = @Address1, " +
					"Address2 = @Address2, " +
					"City = @City, " +
					"State = @State, " +
					"Zip = @Zip, " +
					"Country = @Country, " +
					"Phone = @Phone, " +
					"Fax = @Fax, " +
					"EmergencyContact = @EmergencyContact, " +
					"EmergencyPhone = @EmergencyPhone, " +
					"EmailAddress = @EmailAddress, " +
					"Enabled = @Enabled, " +
					"SiteGroupFlag = @SiteGroupFlag, " +
					"TimeZone = @TimeZone, " +
					"TerminalControlNumber = @TerminalControlNumber, " +
					"InhibitLoadRackCardIns = @InhibitLoadRackCardIns, " +
					"EnforceSingleOwner = @EnforceSingleOwner, " +
					"InhibitBOLSummaryAutoPopulate = @InhibitBOLSummaryAutoPopulate, " +
					"InhibitOrderSummaryAutoPopulate = @InhibitOrderSummaryAutoPopulate, " +
					"InhibitSupplyOrderSummaryAutoPopulate = @InhibitSupplyOrderSummaryAutoPopulate, " +
					"InhibitSiteLedgerRollup = @InhibitSiteLedgerRollup, " +
					"Contact1Name = @Contact1Name," +
					"Contact1Address1 = @Contact1Address1," +
					"Contact1Address2 = @Contact1Address2," +
					"Contact1City = @Contact1City," +
					"Contact1State = @Contact1State," +
					"Contact1Zip = @Contact1Zip," +
					"Contact1Country = @Contact1Country," +
					"Contact1PhoneOffice = @Contact1PhoneOffice," +
					"Contact1PhoneMobile = @Contact1PhoneMobile," +
					"Contact1Fax = @Contact1Fax," +
					"Contact1EmailAddress = @Contact1EmailAddress," +
					"Contact2Name = @Contact2Name," +
					"Contact2Address1 = @Contact2Address1," +
					"Contact2Address2 = @Contact2Address2," +
					"Contact2City = @Contact2City," +
					"Contact2State = @Contact2State," +
					"Contact2Zip = @Contact2Zip," +
					"Contact2Country = @Contact2Country," +
					"Contact2PhoneOffice = @Contact2PhoneOffice," +
					"Contact2PhoneMobile = @Contact2PhoneMobile," +
					"Contact2Fax = @Contact2Fax," +
					"Contact2EmailAddress = @Contact2EmailAddress," +
					"Latitude = @Latitude, " +
					"Longitude = @Longitude, " +
					"Zoom = @Zoom, " +
					"ActiveDirectorySiteGroupGuid = @ActiveDirectorySiteGroupGuid, " +

						  // Units
					"LevelUnitIndex = @LevelUnitIndex, " +
					"VolumeUnitIndex = @VolumeUnitIndex, " +
					"TemperatureUnitIndex = @TemperatureUnitIndex, " +
					"DensityUnitIndex = @DensityUnitIndex, " +
					"MassUnitIndex = @MassUnitIndex, " +
					"FlowUnitIndex = @FlowUnitIndex, " +
					"PressureUnitIndex = @PressureUnitIndex, " +
					"AdditiveVolumeUnitIndex = @AdditiveVolumeUnitIndex, " +
					"AdditiveProfileCycleAmountUnitIndex = @AdditiveProfileCycleAmountUnitIndex, " +
					"AdditiveProfileRateUnitIndex = @AdditiveProfileRateUnitIndex, " +
					"LevelDecimalPlaces = @LevelDecimalPlaces, " +
					"VolumeDecimalPlaces = @VolumeDecimalPlaces, " +
					"TemperatureDecimalPlaces = @TemperatureDecimalPlaces, " +
					"DensityDecimalPlaces = @DensityDecimalPlaces, " +
					"MassDecimalPlaces = @MassDecimalPlaces, " +
					"FlowDecimalPlaces = @FlowDecimalPlaces, " +
					"PressureDecimalPlaces = @PressureDecimalPlaces, " +
					"AdditiveVolumeDecimalPlaces = @AdditiveVolumeDecimalPlaces, " +
					"AdditiveProfileCycleAmountDecimalPlaces = @AdditiveProfileCycleAmountDecimalPlaces, " +
					"AdditiveProfileRateDecimalPlaces = @AdditiveProfileRateDecimalPlaces, " +
					"LookupQuantityDisplayDefaultIndex = @QuantityDisplayDefault, " +

					// Load Rack
					"InhibitAccessAfterHours = @InhibitAccessAfterHours, " +
					"InhibitMultipleCardIns = @InhibitMultipleCardIns, " +
					"AccessCardInRequired = @AccessCardInRequired, " +
					"CheckSiteNumber = @CheckSiteNumber, " +
					"PromptForCustomerCard = @PromptForCustomerCard, " +
					"PromptForTractorOrTanker = @PromptForTractorOrTanker, " +
					"PromptForFirstTrailer = @PromptForFirstTrailer, " +
					"PromptForSecondTrailer = @PromptForSecondTrailer, " +
					"PromptForThirdTrailer = @PromptForThirdTrailer, " +
					"PromptForCompartment = @PromptForCompartment, " +
					"PromptForTransactionCompletion = @PromptForTransactionCompletion, " +
					"InhibitCustomerConfirmationPrompt = @InhibitCustomerConfirmationPrompt, " +
					"RequireTrailerScully = @RequireTrailerScully, " +
					"CardInTimeout = @CardInTimeout, " +
					"EnforceDriverEquipmentMatch = @EnforceDriverEquipmentMatch, " +
					"EnableAdditiveAccounting = @EnableAdditiveAccounting, " +
					"UseCompanyEquipmentIdentifiers = @UseCompanyEquipmentIdentifiers, " +
					"MaximumLoadAmount = @MaximumLoadAmount, " +
					"MaximumLoadTime = @MaximumLoadTime, " +
					"MaximumIdleTime = @MaximumIdleTime, " +
					"MaximumFlushAmount = @MaximumFlushAmount, " +
					"MaximumMeterProvingAmount = @MaximumMeterProvingAmount, " +
					"MaximumReturnsAmount = @MaximumReturnsAmount, " +
					"MaximumNumberOfActiveArms = @MaximumNumberOfActiveArms, " +
					"DriverTimeoutPeriod = @DriverTimeoutPeriod, " +
					"DriverWarningPeriod = @DriverWarningPeriod, " +
					"MaximumPrompts = @MaximumPrompts, " +
					"MaximumVehicleWeight = @MaximumVehicleWeight, " +
					"LoadByNet = @LoadByNet, " +
					"PromptForShipmentNumber = @PromptForShipmentNumber, " +
					"MaximumProductTemperature = @MaximumProductTemperature, " +
					"ListEquipment = @ListEquipment, " +
					"DeferStationChanges = @DeferStationChanges, " +
					"PromptForReturns = @PromptForReturns, " +
					"PromptForTruckCard = @PromptForTruckCard, " +
					"StartingShortCardNumber = @StartingShortCardNumber, " +
					"UseShortCardNumber = @UseShortCardNumber, " +
					"ExcessVarianceCount = @ExcessVarianceCount, " +
					"ExcessVarianceTolerance = @ExcessVarianceTolerance, " +
					"LookupSecondaryStorageFillMethodIndex = @SecondaryStorageFillMethod, " +
					"EnforceSalesOrderLimit = @EnforceSalesOrderLimit, " +

					"LeakDetectionQuietSamples = @LeakDetectionQuietSamples," +
					"LeakDetectionQuietTime = @LeakDetectionQuietTime," +
					"LeakDetectionQuietTimeFactor= @LeakDetectionQuietTimeFactor," +
					"LeakDetectionUseMinWait =@LeakDetectionUseMinWait," +
					"LeakDetectionReport = @LeakDetectionReport," +
					"LeakDetectionPrinter = @LeakDetectionPrinter," +

					// Transactions
					"InhibitBOLWithBrokenBlends = @InhibitBOLWithBrokenBlends, " +
					"InhibitBOLWithImproperAdditization = @InhibitBOLWithImproperAdditization, " +
					"InhibitOverweightBOL = @InhibitOverweightBOL, " +
					"ExceptionBOLPrinter = @ExceptionBOLPrinter, " +
					"EnableAutomaticBOLPrinting = @EnableAutomaticBOLPrinting, " +
					"AutomaticBOLStartNumber = @AutomaticBOLStartNumber, " +
					"AutomaticBOLEndNumber = @AutomaticBOLEndNumber, " +
					"AutomaticBOLNextNumber = @AutomaticBOLNextNumber, " +
					"SeparateManualBOLNumbering = @SeparateManualBOLNumbering, " +
					"ManualBOLStartNumber = @ManualBOLStartNumber, " +
					"ManualBOLEndNumber = @ManualBOLEndNumber, " +
					"ManualBOLNextNumber = @ManualBOLNextNumber, " +
					"TransactionStartNumber = @TransactionStartNumber, " +
					"TransactionEndNumber = @TransactionEndNumber, " +
					"TransactionNextNumber = @TransactionNextNumber, " +
					"OrderStartNumber = @OrderStartNumber, " +
					"OrderEndNumber = @OrderEndNumber, " +
					"OrderNextNumber = @OrderNextNumber, " +
					// vt 07-15-2008
					"InvoiceStartNumber = @InvoiceStartNumber, " +
					"InvoiceEndNumber = @InvoiceEndNumber, " +
					"InvoiceNextNumber = @InvoiceNextNumber, " +
					"NumberPrefix = @NumberPrefix, " +
					"OpenTransactionWindow = @OpenTransactionWindow, " +
					"AdministrativeLockDate = @AdministrativeLockDate, " +
					"OperationalLockDate = @OperationalLockDate, " +
					"EnableBOLPDFArchiving = @EnableBOLPDFArchiving, " +
					"BOLPDFArchivingPath = @BOLPDFArchivingPath, " +

					// System
					"MaximumDaysToRetainLogs = @MaximumDaysToRetainLogs, " +
					"EnableDebugLogging = @EnableDebugLogging, " +
					"EnableAuditLogging = @EnableAuditLogging, " +
					"AutomaticallyPrintAlarmsAndEvents = @AutomaticallyPrintAlarmsAndEvents, " +
					"AlarmAndEventPrinter = @AlarmAndEventPrinter, " +
					"MailServer = @MailServer, " +
					"MailFrom = @MailFrom, " +
					"MailUserName = @MailUserName, " +
					"MailPassword = @MailPassword, " +
					"LookupMailConnectModeIndex = @MailConnectMode, " +
					"DialupName = @DialupName, " +
					"SCADASystem = @SCADASystem, " +
					"InhibitTemplateGraphics = @InhibitTemplateGraphics, " +
					"RefreshInterval = @RefreshInterval, " +
					"InhibitEndOfDayOperations = @InhibitEndOfDayOperations, " +
					"InhibitEndOfMonthOperations = @InhibitEndOfMonthOperations, " +
					"EndOfDayWarningPeriod = @EndOfDayWarningPeriod, " +
					"InhibitAutomaticPhysicalInventory = @InhibitAutomaticPhysicalInventory, " +
					"InhibitAutomaticMeterCloseout = @InhibitAutomaticMeterCloseout, " +
					"InhibitAutomaticReportGeneration = @InhibitAutomaticReportGeneration, " +
					"InhibitAutomaticAdjustmentDistribution = @InhibitAutomaticAdjustmentDistribution, " +
					"InhibitAutomaticCloseout = @InhibitAutomaticCloseout, " +
					"BlockCloseOnUnpostedBol = @BlockCloseOnUnpostedBol, " +
					"InhibitTankScan = @InhibitTankScan, " +
					"ReportDirectory = @ReportDirectory, " +
					"ManageReports = @ManageReports, " +
					"ManagedReportDirectory = @ManagedReportDirectory, " +
					"ExportArchiveDir = @ExportArchiveDir, " +
					"ImportArchiveDir = @ImportArchiveDir, " +
					"GroupLedgerByID = @GroupLedgerByID, " +
					"MeterReconciliationToleranceIsPercent = @MeterReconciliationToleranceIsPercent," +
					"MeterReconciliationReportName = @MeterReconciliationReportName," +
					"TranslatedHelpURL = @TranslatedHelpURL," +
					"EnableAutomaticMovementTicketPrinting = @EnableAutomaticMovementTicketPrinting," +
					"MovementTicketReport = @MovementTicketReport," +
					"MovementTicketPrinter = @MovementTicketPrinter," +
					"MaxOperateTabsAllowed = @MaxOperateTabsAllowed," +
					"CloseoutTime = @CloseoutTime," +
					"PointGroupFileExportDirectory = @PointGroupFileExportDirectory," +
					"PointGroupDefaultFileName = @PointGroupDefaultFileName," +
					"EnableMovementTicketPDFArchiving = @EnableMovementTicketPDFArchiving," +
					"MovementTicketFileExportDirectory = @MovementTicketFileExportDirectory," +
					"MovementTicketExportFileName = @MovementTicketExportFileName," +

					// Vapor Recovery Unit (VRU)
					"VRURateLimit = @VRURateLimit, " +
					"VRUHourlyLimit = @VRUHourlyLimit, " +
					"VRUDailyLimit = @VRUDailyLimit, " +
					"VRUYearlyLimit = @VRUYearlyLimit, " +
					"VRUCurrentYearLimit = @VRUCurrentYearLimit, " +
					"VRURateLimitEnabled = @VRURateLimitEnabled, " +
					"VRUHourlyLimitEnabled = @VRUHourlyLimitEnabled, " +
					"VRUDailyLimitEnabled = @VRUDailyLimitEnabled, " +
					"VRUYearlyLimitEnabled = @VRUYearlyLimitEnabled, " +
					"VRUCurrentYearLimitEnabled = @VRUCurrentYearLimitEnabled, " +

					// Process I/O
					"WatchdogPeriod = @WatchdogPeriod, " +
					"LookupWatchdogModeIndex = @WatchdogMode, " +
					"WatchdogCounterStart = @WatchdogCounterStart, " +
					"WatchdogCounterEnd = @WatchdogCounterEnd, " +

					// Regional Settings
					"LookupNumberGroupSizesTypeIndex = @NumberGroupSizesType, " +
					"NumberDecimalSeparator = @NumberDecimalSeparator, " +
					"NumberGroupSeparator = @NumberGroupSeparator, " +
					"ListSeparator = @ListSeparator, " +
					"TimePattern = @TimePattern, " +
					"TimeSeparator = @TimeSeparator, " +
					"AMSymbol = @AMSymbol, " +
					"PMSymbol = @PMSymbol, " +
					"ShortDatePattern = @ShortDatePattern, " +
					"DateSeparator = @DateSeparator, " +
					"LongDatePattern = @LongDatePattern, " +
					"TwoDigitCalendarEndYear = @TwoDigitCalendarEndYear, " +

					// Additional Data
					"UserData1 = @UserData1, " +
					"UserData2 = @UserData2, " +
					"UserData3 = @UserData3, " +
					"UserData4 = @UserData4, " +
					"UserData5 = @UserData5, " +
					"UserData6 = @UserData6, " +
					"UserData7 = @UserData7, " +
					"UserData8 = @UserData8, " +

					// Audit Data
					"UpdatedDate = @UpdatedDate, " +
					"UpdatedBy	= @UpdatedBy, " +

					// Password configuration
					"MinTimeAllowedToChangePwd = @MinTimeAllowedToChangePwd, " +
					"MinPwdCharacterLength = @MinPwdCharacterLength, " +
					"PwdExpirationInDays = @PwdExpirationInDays, " +
					"PwdLockoutThreshold = @PwdLockoutThreshold, " +
					"PwdHistoryCount = @PwdHistoryCount, " +
					"CheckForPreviousPwd = @CheckForPreviousPwd, " +
					"StrongPwdUse = @StrongPwdUse, " +
					"ApplyToAllSiteMembers = @ApplyToAllSiteMembers, " +
					"InactivityDisablePeriod = @InactivityDisablePeriod, " +
					"DisableArchivePeriod = @DisableArchivePeriod, " +
					"UseTankReconciliation = @UseTankReconciliation, " +

					//Password Hint / Forgotten Password
					"EnablePasswordHint = @EnablePasswordHint, " +
					"EnablePasswordReset = @EnablePasswordReset, " +
					"AllowUseOfSpecialChars = @AllowUseOfSpecialChars, " +

					 // Sites tab
					"Enterprise = @Enterprise, " +
					"OperateTabGroups = @OperateTabGroups, " +

					//Enterprise Query Credentials
					"EnterpriseUserId = @EnterpriseUserId, " +
					"EnterprisePassword = @EnterprisePassword, " +
					"EnterpriseSite = @EnterpriseSite,"+

					// OPC UA
					"ServerEndPoint = @ServerEndPoint," +
					"SecurityMode = @SecurityMode," +
					"SecurityPolicy = @SecurityPolicy," +
					"MessageEncoding = @MessageEncoding," +
					"UserIdentityMethod = @UserIdentityMethod," +
					"UserId = @UserId," +
					"UserPassword = @UserPassword," +
					"UserCertificatePath = @UserCertificatePath," +

					// Additional System
					"MaximumDaysToRetainArchive = @MaximumDaysToRetainArchive" +
					" WHERE SiteGuid = @SiteGuid" +
					" " +
					"UPDATE tblSitesAncillaryData " +
					"SET InventoryTransactionAliasGuid = @InventoryTransactionAliasGuid, " +
					"AdjustmentTransactionAliasGuid = @AdjustmentTransactionAliasGuid, " +
					"IATAGuid = @IATAGuid, " +
					"NoteGuid = @NoteGuid," +
					"UpdatedDate = @UpdatedDate, " +
					"UpdatedBy	= @updatedBy " +
					" WHERE SiteGuid = @SiteGuid";

				cmd.Parameters.AddWithValue("@ID", this.ID);

				// General
				cmd.Parameters.AddWithValue("@Number", this.Number);
				cmd.Parameters.AddWithValue("@SPLCCode", this.SPLCCode);
				cmd.Parameters.AddWithValue("@Address1", this.Address1);
				cmd.Parameters.AddWithValue("@Address2", this.Address2);
				cmd.Parameters.AddWithValue("@City", this.City);
				cmd.Parameters.AddWithValue("@State", this.State);
				cmd.Parameters.AddWithValue("@Zip", this.Zip);
				cmd.Parameters.AddWithValue("@Country", this.Country);
				cmd.Parameters.AddWithValue("@Phone", this.Phone);
				cmd.Parameters.AddWithValue("@Fax", this.Fax);
				cmd.Parameters.AddWithValue("@EmergencyContact", this.EmergencyContact);
				cmd.Parameters.AddWithValue("@EmergencyPhone", this.EmergencyPhone);
				cmd.Parameters.AddWithValue("@EmailAddress", this.EmailAddress);
				cmd.Parameters.AddWithValue("@Enabled", ((this.Enabled) ? 1 : 0));
				cmd.Parameters.AddWithValue("@SiteGroupFlag", ((this.SiteGroup) ? 1 : 0));
				cmd.Parameters.AddWithValue("@TimeZone", this.TimeZone);
				cmd.Parameters.AddWithValue("@TerminalControlNumber", this.TerminalControlNumber);
				cmd.Parameters.AddWithValue("@InhibitLoadRackCardIns", (this.InhibitLoadRackCardIns ? 1 : 0));
				cmd.Parameters.AddWithValue("@EnforceSingleOwner", (this.EnforceSingleOwner ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitBOLSummaryAutoPopulate", (this.InhibitBOLSummaryAutoPopulate ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitOrderSummaryAutoPopulate", (this.InhibitOrderSummaryAutoPopulate ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitSupplyOrderSummaryAutoPopulate", (this.InhibitSupplyOrderSummaryAutoPopulate ? 1 : 0));
				cmd.Parameters.Add(DataObject.NewGuidParameter("@IATAGuid", this.IATAGuid, true));
				cmd.Parameters.AddWithValue("@InhibitSiteLedgerRollup", (this.InhibitSiteLedgerRollup ? 1 : 0));
				cmd.Parameters.AddWithValue("@Contact1Name", string.IsNullOrEmpty(this._Contact1Name) ? (object)DBNull.Value : this._Contact1Name);
				cmd.Parameters.AddWithValue("@Contact1Address1", string.IsNullOrEmpty(this._Contact1Address1) ? (object)DBNull.Value : this._Contact1Address1);
				cmd.Parameters.AddWithValue("@Contact1Address2", string.IsNullOrEmpty(this._Contact1Address2) ? (object)DBNull.Value : this._Contact1Address2);
				cmd.Parameters.AddWithValue("@Contact1City", string.IsNullOrEmpty(this._Contact1City) ? (object)DBNull.Value : this._Contact1City);
				cmd.Parameters.AddWithValue("@Contact1State", string.IsNullOrEmpty(this._Contact1State) ? (object)DBNull.Value : this._Contact1State);
				cmd.Parameters.AddWithValue("@Contact1Zip", string.IsNullOrEmpty(this._Contact1Zip) ? (object)DBNull.Value : this._Contact1Zip);
				cmd.Parameters.AddWithValue("@Contact1Country", string.IsNullOrEmpty(this._Contact1Country) ? (object)DBNull.Value : this._Contact1Country);
				cmd.Parameters.AddWithValue("@Contact1PhoneOffice", string.IsNullOrEmpty(this._Contact1PhoneOffice) ? (object)DBNull.Value : this._Contact1PhoneOffice);
				cmd.Parameters.AddWithValue("@Contact1PhoneMobile", string.IsNullOrEmpty(this._Contact1PhoneMobile) ? (object)DBNull.Value : this._Contact1PhoneMobile);
				cmd.Parameters.AddWithValue("@Contact1Fax", string.IsNullOrEmpty(this._Contact1Fax) ? (object)DBNull.Value : this._Contact1Fax);
				cmd.Parameters.AddWithValue("@Contact1EmailAddress", string.IsNullOrEmpty(this._Contact1EmailAddress) ? (object)DBNull.Value : this._Contact1EmailAddress);
				cmd.Parameters.AddWithValue("@Contact2Name", string.IsNullOrEmpty(this._Contact2Name) ? (object)DBNull.Value : this._Contact2Name);
				cmd.Parameters.AddWithValue("@Contact2Address1", string.IsNullOrEmpty(this._Contact2Address1) ? (object)DBNull.Value : this._Contact2Address1);
				cmd.Parameters.AddWithValue("@Contact2Address2", string.IsNullOrEmpty(this._Contact2Address2) ? (object)DBNull.Value : this._Contact2Address2);
				cmd.Parameters.AddWithValue("@Contact2City", string.IsNullOrEmpty(this._Contact2City) ? (object)DBNull.Value : this._Contact2City);
				cmd.Parameters.AddWithValue("@Contact2State", string.IsNullOrEmpty(this._Contact2State) ? (object)DBNull.Value : this._Contact2State);
				cmd.Parameters.AddWithValue("@Contact2Zip", string.IsNullOrEmpty(this._Contact2Zip) ? (object)DBNull.Value : this._Contact2Zip);
				cmd.Parameters.AddWithValue("@Contact2Country", string.IsNullOrEmpty(this._Contact2Country) ? (object)DBNull.Value : this._Contact2Country);
				cmd.Parameters.AddWithValue("@Contact2PhoneOffice", string.IsNullOrEmpty(this._Contact2PhoneOffice) ? (object)DBNull.Value : this._Contact2PhoneOffice);
				cmd.Parameters.AddWithValue("@Contact2PhoneMobile", string.IsNullOrEmpty(this._Contact2PhoneMobile) ? (object)DBNull.Value : this._Contact2PhoneMobile);
				cmd.Parameters.AddWithValue("@Contact2Fax", string.IsNullOrEmpty(this._Contact2Fax) ? (object)DBNull.Value : this._Contact2Fax);
				cmd.Parameters.AddWithValue("@Contact2EmailAddress", string.IsNullOrEmpty(this._Contact2EmailAddress) ? (object)DBNull.Value : this._Contact2EmailAddress);

				if (this.latitude == null)
				{
					cmd.Parameters.AddWithValue("@Latitude", DBNull.Value);		
				}
				else
				{
					cmd.Parameters.AddWithValue("@Latitude", this.latitude.Value);					
				}

				if (this.longitude == null)
				{
					cmd.Parameters.AddWithValue("@Longitude", DBNull.Value);
				}
				else
				{
					cmd.Parameters.AddWithValue("@Longitude", this.longitude.Value);
				}

				if (this.zoom == null)
				{
					cmd.Parameters.AddWithValue("@Zoom", DBNull.Value);
				}
				else
				{
					cmd.Parameters.AddWithValue("@Zoom", this.zoom.Value);
				}

					 if (this.activeDirectorySiteGroupGuid == null || this.activeDirectorySiteGroupGuid == Guid.Empty)
					 {
						  cmd.Parameters.AddWithValue("@ActiveDirectorySiteGroupGuid", DBNull.Value);
					 }
					 else
					 {
						  cmd.Parameters.AddWithValue("@ActiveDirectorySiteGroupGuid", this.activeDirectorySiteGroupGuid);
					 }

					 // Units
					 cmd.Parameters.AddWithValue("@LevelUnitIndex", ((int)this.LevelUnits));
				cmd.Parameters.AddWithValue("@TemperatureUnitIndex", ((int)this.TemperatureUnits));
				cmd.Parameters.AddWithValue("@DensityUnitIndex", ((int)this.DensityUnits));
				cmd.Parameters.AddWithValue("@PressureUnitIndex", ((int)this.PressureUnits));
				cmd.Parameters.AddWithValue("@FlowUnitIndex", ((int)this.FlowUnits));
				cmd.Parameters.AddWithValue("@VolumeUnitIndex", ((int)this.VolumeUnits));
				cmd.Parameters.AddWithValue("@MassUnitIndex", ((int)this.MassUnits));
				cmd.Parameters.AddWithValue("@AdditiveVolumeUnitIndex", ((int)this.AdditiveVolumeUnits));
				cmd.Parameters.AddWithValue("@AdditiveProfileCycleAmountUnitIndex", ((int)this.AdditiveProfileCycleAmountUnits));
				cmd.Parameters.AddWithValue("@AdditiveProfileRateUnitIndex", ((int)this.AdditiveProfileRateUnits));
				cmd.Parameters.AddWithValue("@LevelDecimalPlaces", (int)this._LevelDecimalPlaces);
				cmd.Parameters.AddWithValue("@TemperatureDecimalPlaces", (int)this._TemperatureDecimalPlaces);
				cmd.Parameters.AddWithValue("@DensityDecimalPlaces", (int)this._DensityDecimalPlaces);
				cmd.Parameters.AddWithValue("@PressureDecimalPlaces", (int)this._PressureDecimalPlaces);
				cmd.Parameters.AddWithValue("@FlowDecimalPlaces", (int)this._FlowDecimalPlaces);
				cmd.Parameters.AddWithValue("@VolumeDecimalPlaces", (int)this._VolumeDecimalPlaces);
				cmd.Parameters.AddWithValue("@MassDecimalPlaces", (int)this._MassDecimalPlaces);
				cmd.Parameters.AddWithValue("@AdditiveVolumeDecimalPlaces", (int)this._AdditiveVolumeDecimalPlaces);
				cmd.Parameters.AddWithValue("@AdditiveProfileCycleAmountDecimalPlaces", (int)this._AdditiveProfileCycleAmountDecimalPlaces);
				cmd.Parameters.AddWithValue("@AdditiveProfileRateDecimalPlaces", (int)this._AdditiveProfileRateDecimalPlaces);
				cmd.Parameters.AddWithValue("@QuantityDisplayDefault", ((int)this.QuantityDisplayDefault));

				// Load Rack
				cmd.Parameters.AddWithValue("@InhibitAccessAfterHours", (this.InhibitAccessAfterHours ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitMultipleCardIns", (this.InhibitMultipleCardIns ? 1 : 0));
				cmd.Parameters.AddWithValue("@AccessCardInRequired", (this.AccessCardInRequired ? 1 : 0));
				cmd.Parameters.AddWithValue("@CheckSiteNumber", (this.CheckSiteNumber ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForCustomerCard", (this.PromptForCustomerCard ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForTractorOrTanker", (this.PromptForTractorOrTanker ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForFirstTrailer", (this.PromptForFirstTrailer ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForSecondTrailer", (this.PromptForSecondTrailer ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForThirdTrailer", (this.PromptForThirdTrailer ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForCompartment", (this.PromptForCompartment ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForTransactionCompletion", (this.PromptForTransactionCompletion ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitCustomerConfirmationPrompt", (this.InhibitCustomerConfirmationPrompt ? 1 : 0));
				cmd.Parameters.AddWithValue("@RequireTrailerScully", (this.RequireTrailerScully ? 1 : 0));
				cmd.Parameters.AddWithValue("@CardInTimeout", (this._CardInTimeout));
				cmd.Parameters.AddWithValue("@EnforceDriverEquipmentMatch", (this.EnforceDriverEquipmentMatch ? 1 : 0));
				cmd.Parameters.AddWithValue("@EnableAdditiveAccounting", (this.EnableAdditiveAccounting ? 1 : 0));
				cmd.Parameters.AddWithValue("@UseCompanyEquipmentIdentifiers", (this.UseCompanyEquipmentIdentifiers ? 1 : 0));
				cmd.Parameters.AddWithValue("@MaximumLoadAmount", this._MaximumLoadAmount.SIValue);
				cmd.Parameters.AddWithValue("@MaximumLoadTime", this._MaximumLoadTime);
				cmd.Parameters.AddWithValue("@MaximumIdleTime", this._MaximumIdleTime);
				cmd.Parameters.AddWithValue("@MaximumFlushAmount", this._MaximumFlushAmount.SIValue);
				cmd.Parameters.AddWithValue("@MaximumMeterProvingAmount", this._MaximumMeterProvingAmount.SIValue);
				cmd.Parameters.AddWithValue("@MaximumReturnsAmount", this._MaximumReturnsAmount.SIValue);
				cmd.Parameters.AddWithValue("@MaximumNumberOfActiveArms", this._MaximumNumberOfActiveArms);
				cmd.Parameters.AddWithValue("@DriverTimeoutPeriod", this._DriverTimeoutPeriod);
				cmd.Parameters.AddWithValue("@DriverWarningPeriod", this._DriverWarningPeriod);
				cmd.Parameters.AddWithValue("@MaximumPrompts", this._MaximumPrompts);
				cmd.Parameters.Add(DataObject.NewGuidParameter("@InventoryTransactionAliasGuid", this.InventoryTransactionAliasGuid, true));
				cmd.Parameters.Add(DataObject.NewGuidParameter("@AdjustmentTransactionAliasGuid", this.AdjustmentTransactionAliasGuid, true));
				cmd.Parameters.AddWithValue("@MaximumVehicleWeight", this._MaximumVehicleWeight.SIValue);
				cmd.Parameters.AddWithValue("@LoadByNet", (this.LoadByNet ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForShipmentNumber", (this.PromptForShipmentNumber ? 1 : 0));
				cmd.Parameters.AddWithValue("@MaximumProductTemperature", this._MaximumProductTemperature.SIValue);
				cmd.Parameters.AddWithValue("@ListEquipment", (this.ListEquipment ? 1 : 0));
				cmd.Parameters.AddWithValue("@DeferStationChanges", (this.DeferStationChanges ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForReturns", (this.PromptForReturns ? 1 : 0));
				cmd.Parameters.AddWithValue("@PromptForTruckCard", (this.PromptForTruckCard ? 1 : 0));
				cmd.Parameters.AddWithValue("@StartingShortCardNumber", this._StartingShortCardNumber);
				cmd.Parameters.AddWithValue("@UseShortCardNumber", (this.UseShortCardNumber ? 1 : 0));
				cmd.Parameters.AddWithValue("@ExcessVarianceCount", (int)this._ExcessVarianceCount);
				cmd.Parameters.AddWithValue("@ExcessVarianceTolerance", this._ExcessVarianceTolerance);
				cmd.Parameters.AddWithValue("@SecondaryStorageFillMethod", ((int)this.SecondaryStorageFillMethod));
				cmd.Parameters.AddWithValue("@EnforceSalesOrderLimit", (this.EnforceSalesOrderLimit ? 1 : 0));

				cmd.Parameters.AddWithValue("@LeakDetectionQuietSamples", (int)this._LeakDetectionQuietSamples);
				cmd.Parameters.AddWithValue("@LeakDetectionQuietTime", (int)this._LeakDetectionQuietTime);
				cmd.Parameters.AddWithValue("@LeakDetectionQuietTimeFactor", (int)this._LeakDetectionQuietTimeFactor);
				cmd.Parameters.AddWithValue("@LeakDetectionUseMinWait", (this._LeakDetectionUseMinWait ? 1 : 0));
				cmd.Parameters.AddWithValue("@LeakDetectionReport", this._LeakDetectionReport);
				cmd.Parameters.AddWithValue("@LeakDetectionPrinter", this._LeakDetectionPrinter);

				// Transactions
				cmd.Parameters.AddWithValue("@InhibitBOLWithBrokenBlends", (this.InhibitBOLWithBrokenBlends ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitBOLWithImproperAdditization", (this.InhibitBOLWithImproperAdditization ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitOverweightBOL", (this.InhibitOverweightBOL ? 1 : 0));
				cmd.Parameters.AddWithValue("@ExceptionBOLPrinter", this.ExceptionBOLPrinter);
				cmd.Parameters.AddWithValue("@EnableAutomaticBOLPrinting", (this.EnableAutomaticBOLPrinting ? 1 : 0));
				cmd.Parameters.AddWithValue("@AutomaticBOLStartNumber", this._AutomaticBOLStartNumber);
				cmd.Parameters.AddWithValue("@AutomaticBOLEndNumber", this._AutomaticBOLEndNumber);
				cmd.Parameters.AddWithValue("@AutomaticBOLNextNumber", this._AutomaticBOLNextNumber);
				cmd.Parameters.AddWithValue("@SeparateManualBOLNumbering", (this.SeparateManualBOLNumbering ? 1 : 0));
				cmd.Parameters.AddWithValue("@ManualBOLStartNumber", this._ManualBOLStartNumber);
				cmd.Parameters.AddWithValue("@ManualBOLEndNumber", this._ManualBOLEndNumber);
				cmd.Parameters.AddWithValue("@ManualBOLNextNumber", this._ManualBOLNextNumber);
				cmd.Parameters.AddWithValue("@TransactionStartNumber", this._TransactionStartNumber);
				cmd.Parameters.AddWithValue("@TransactionEndNumber", this._TransactionEndNumber);
				cmd.Parameters.AddWithValue("@TransactionNextNumber", this._TransactionNextNumber);
				cmd.Parameters.AddWithValue("@OrderStartNumber", this._OrderStartNumber);
				cmd.Parameters.AddWithValue("@OrderEndNumber", this._OrderEndNumber);
				cmd.Parameters.AddWithValue("@OrderNextNumber", this._OrderNextNumber);
				// vt 07-15-2008
				cmd.Parameters.AddWithValue("@InvoiceStartNumber", this._InvoiceStartNumber);
				cmd.Parameters.AddWithValue("@InvoiceEndNumber", this._InvoiceEndNumber);
				cmd.Parameters.AddWithValue("@InvoiceNextNumber", this._InvoiceNextNumber);
				cmd.Parameters.AddWithValue("@NumberPrefix", this.NumberPrefix);
				cmd.Parameters.AddWithValue("@OpenTransactionWindow", this._OpenTransactionWindow);
				// Truncate the Time part
				cmd.Parameters.AddWithValue("@AdministrativeLockDate", TimeConverter.ToDate(this._AdministrativeLockDate.Value));
				cmd.Parameters.AddWithValue("@OperationalLockDate", this._OperationalLockDate.Value);
				cmd.Parameters.AddWithValue("@EnableBOLPDFArchiving", (this._EnableBOLPDFArchiving ? 1 : 0));
				cmd.Parameters.AddWithValue("@BOLPDFArchivingPath", this._BOLPDFArchivingPath);

				// System
				cmd.Parameters.AddWithValue("@MaximumDaysToRetainLogs", this._MaximumDaysToRetainLogs);
				cmd.Parameters.AddWithValue("@EnableDebugLogging", (this.EnableDebugLogging ? 1 : 0));
				cmd.Parameters.AddWithValue("@EnableAuditLogging", (this.EnableAuditLogging ? 1 : 0));
				cmd.Parameters.AddWithValue("@AutomaticallyPrintAlarmsAndEvents", (this.AutomaticallyPrintAlarmsAndEvents ? 1 : 0));
				cmd.Parameters.AddWithValue("@AlarmAndEventPrinter", this.AlarmAndEventPrinter);
				cmd.Parameters.AddWithValue("@MailServer", this.MailServer);
				cmd.Parameters.AddWithValue("@MailFrom", this.MailFrom);
				cmd.Parameters.AddWithValue("@MailUserName", this.MailUserName);
				cmd.Parameters.AddWithValue("@MailPassword", this.MailPassword);
				cmd.Parameters.AddWithValue("@MailConnectMode", ((int)this.MailConnectMode));
				cmd.Parameters.AddWithValue("@DialupName", this.DialupName);
				cmd.Parameters.AddWithValue("@SCADASystem", this.SCADASystem);
				cmd.Parameters.AddWithValue("@InhibitTemplateGraphics", (this.InhibitTemplateGraphics ? 1 : 0));
				cmd.Parameters.AddWithValue("@RefreshInterval", this._RefreshInterval);
				cmd.Parameters.AddWithValue("@InhibitEndOfDayOperations", (this.InhibitEndOfDayOperations ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitEndOfMonthOperations", (this.InhibitEndOfMonthOperations ? 1 : 0));
				cmd.Parameters.AddWithValue("@EndOfDayWarningPeriod", this._EndOfDayWarningPeriod);
				cmd.Parameters.AddWithValue("@InhibitAutomaticPhysicalInventory", (this.InhibitAutomaticPhysicalInventory ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitAutomaticMeterCloseout", (this.InhibitAutomaticMeterCloseout ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitAutomaticReportGeneration", (this.InhibitAutomaticReportGeneration ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitAutomaticAdjustmentDistribution", (this.InhibitAutomaticAdjustmentDistribution ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitAutomaticCloseout", (this.InhibitAutomaticCloseout ? 1 : 0));
				cmd.Parameters.AddWithValue("@BlockCloseOnUnpostedBol", (this.BlockCloseOnUnpostedBol ? 1 : 0));
				cmd.Parameters.AddWithValue("@InhibitTankScan", (this.InhibitTankScan ? 1 : 0));
				cmd.Parameters.AddWithValue("@ReportDirectory", this.ReportDirectory);
				cmd.Parameters.AddWithValue("@ManageReports", (this.ManageReports ? 1 : 0));
				cmd.Parameters.AddWithValue("@ManagedReportDirectory", this.ManagedReportDirectory);
				cmd.Parameters.AddWithValue("@ExportArchiveDir", this.ExportArchiveDir);
				cmd.Parameters.AddWithValue("@ImportArchiveDir", this.ImportArchiveDir);
				cmd.Parameters.AddWithValue("@GroupLedgerByID", (this.GroupLedgerByID ? 1 : 0));
				cmd.Parameters.AddWithValue("@MeterReconciliationToleranceIsPercent", (this.MeterReconciliationToleranceIsPercent ? 1 : 0));
				cmd.Parameters.AddWithValue("@MeterReconciliationReportName", this.MeterReconciliationReportName);
				cmd.Parameters.AddWithValue("@TranslatedHelpURL", this.TranslatedHelpURL);
				cmd.Parameters.AddWithValue("@EnableAutomaticMovementTicketPrinting", this.EnableAutomaticMovementTicketPrinting);
				cmd.Parameters.AddWithValue("@MovementTicketReport", this.MovementTicketReportName);
				cmd.Parameters.AddWithValue("@MovementTicketPrinter", this.MovementTicketPrinter);
				cmd.Parameters.AddWithValue("@MaxOperateTabsAllowed", this.MaxOperateTabsAllowed);
				cmd.Parameters.AddWithValue("@CloseoutTime", this.CloseoutTime == null ? (object)DBNull.Value : this.CloseoutTime.Value);
				cmd.Parameters.AddWithValue("@PointGroupFileExportDirectory", this.PointGroupFileExportDirectory);
				cmd.Parameters.AddWithValue("@PointGroupDefaultFileName", this.PointGroupDefaultFileName);
				cmd.Parameters.AddWithValue("@EnableMovementTicketPDFArchiving", this.EnableMovementTicketPDFArchiving);
				cmd.Parameters.AddWithValue("@MovementTicketFileExportDirectory", this.MovementTicketFileExportDirectory);
				cmd.Parameters.AddWithValue("@MovementTicketExportFileName", this.MovementTicketExportFileName);


				// Vapor Recovery Unit (VRU)
				cmd.Parameters.AddWithValue("@VRURateLimit", this._VRURateLimit.SIValue);
				cmd.Parameters.AddWithValue("@VRUHourlyLimit", this._VRUHourlyLimit.SIValue);
				cmd.Parameters.AddWithValue("@VRUDailyLimit", this._VRUDailyLimit.SIValue);
				cmd.Parameters.AddWithValue("@VRUYearlyLimit", this._VRUYearlyLimit.SIValue);
				cmd.Parameters.AddWithValue("@VRUCurrentYearLimit", this._VRUCurrentYearLimit.SIValue);
				cmd.Parameters.AddWithValue("@VRURateLimitEnabled", (this.VRURateLimitEnabled ? 1 : 0));
				cmd.Parameters.AddWithValue("@VRUHourlyLimitEnabled", (this.VRUHourlyLimitEnabled ? 1 : 0));
				cmd.Parameters.AddWithValue("@VRUDailyLimitEnabled", (this.VRUDailyLimitEnabled ? 1 : 0));
				cmd.Parameters.AddWithValue("@VRUYearlyLimitEnabled", (this.VRUYearlyLimitEnabled ? 1 : 0));
				cmd.Parameters.AddWithValue("@VRUCurrentYearLimitEnabled", (this.VRUCurrentYearLimitEnabled ? 1 : 0));

				// Process I/O
				cmd.Parameters.AddWithValue("@WatchdogPeriod", this.WatchdogPeriod);
				cmd.Parameters.AddWithValue("@WatchdogMode", ((int)this.WatchdogMode));
				cmd.Parameters.AddWithValue("@WatchdogCounterStart", this._WatchdogCounterStart);
				cmd.Parameters.AddWithValue("@WatchdogCounterEnd", this._WatchdogCounterEnd);

				// Regional Settings
				cmd.Parameters.AddWithValue("@NumberGroupSizesType", ((int)this._NumberGroupSizesType));
				cmd.Parameters.AddWithValue("@NumberDecimalSeparator", this._NumberDecimalSeparator);
				cmd.Parameters.AddWithValue("@NumberGroupSeparator", this._NumberGroupSeparator);
				cmd.Parameters.AddWithValue("@ListSeparator", this._ListSeparator);
				cmd.Parameters.AddWithValue("@TimePattern", this._TimePattern);
				cmd.Parameters.AddWithValue("@TimeSeparator", this._TimeSeparator);
				cmd.Parameters.AddWithValue("@AMSymbol", this._AMSymbol);
				cmd.Parameters.AddWithValue("@PMSymbol", this._PMSymbol);
				cmd.Parameters.AddWithValue("@ShortDatePattern", this._ShortDatePattern);
				cmd.Parameters.AddWithValue("@DateSeparator", this._DateSeparator);
				cmd.Parameters.AddWithValue("@LongDatePattern", this._LongDatePattern);
				cmd.Parameters.AddWithValue("@TwoDigitCalendarEndYear", this._TwoDigitCalendarEndYear);

				// Additional Data
				cmd.Parameters.AddWithValue("@UserData1", this.UserData[0]);
				cmd.Parameters.AddWithValue("@UserData2", this.UserData[1]);
				cmd.Parameters.AddWithValue("@UserData3", this.UserData[2]);
				cmd.Parameters.AddWithValue("@UserData4", this.UserData[3]);
				cmd.Parameters.AddWithValue("@UserData5", this.UserData[4]);
				cmd.Parameters.AddWithValue("@UserData6", this.UserData[5]);
				cmd.Parameters.AddWithValue("@UserData7", this.UserData[6]);
				cmd.Parameters.AddWithValue("@UserData8", this.UserData[7]);

				// Notes 
				cmd.Parameters.AddWithValue("@NoteGuid", (this.NoteGuid == Guid.Empty ? (object)DBNull.Value : this.NoteGuid));

				// Audit Data
				cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
				cmd.Parameters.AddWithValue("@UpdatedBy ", this._UpdatedBy);

				// Password configuration
				cmd.Parameters.AddWithValue("@MinTimeAllowedToChangePwd", this.minTimeAllowedToChangePwd);
				cmd.Parameters.AddWithValue("@MinPwdCharacterLength", this.minPwdCharacterLength);
				cmd.Parameters.AddWithValue("@PwdExpirationInDays", this.pwdExpirationInDays);
				cmd.Parameters.AddWithValue("@PwdLockoutThreshold", this.pwdLockoutThreshold);
				cmd.Parameters.AddWithValue("@PwdHistoryCount", this.pwdHistoryCount);
				cmd.Parameters.AddWithValue("@CheckForPreviousPwd", (this.checkForPreviousPwd ? 1 : 0));
				cmd.Parameters.AddWithValue("@StrongPwdUse", this.StrongPwdUse);
				cmd.Parameters.AddWithValue("@ApplyToAllSiteMembers", (this.applyToAllSiteMembers ? 1 : 0));
				cmd.Parameters.AddWithValue("@InactivityDisablePeriod", this.inactivityDisablePeriod);
				cmd.Parameters.AddWithValue("@DisableArchivePeriod", this.disableArchivePeriod);
				cmd.Parameters.AddWithValue("@UseTankReconciliation", (this.useTankReconciliation ? 1 : 0));

				// Password Hint / Forgotten Password
				cmd.Parameters.AddWithValue("@EnablePasswordHint", (this._EnablePasswordHint ? 1 : 0));
				cmd.Parameters.AddWithValue("@EnablePasswordReset", (this._EnablePasswordReset ? 1 : 0));
				cmd.Parameters.AddWithValue("@AllowUseOfSpecialChars", (this._AllowUseOfSpecialChars ? 1 : 0));
				cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);

				// Sites Tab
				cmd.Parameters.AddWithValue("@Enterprise", this._Enterprise);
				cmd.Parameters.AddWithValue("@OperateTabGroups", this._OperateTabGroups);

					 //Enterprise Query Credentials
					 cmd.Parameters.AddWithValue("@EnterpriseUserId", this._EnterpriseUserId);
					 if (!string.IsNullOrEmpty(this._EnterprisePassword))
				 {
					  cmd.Parameters.AddWithValue("@EnterprisePassword", UserClass.encode(this._EnterprisePassword, this.SiteGuid));
				 }else
				 {
						  cmd.Parameters.Add("@EnterprisePassword", SqlDbType.VarBinary).Value = DBNull.Value;
				 }
				cmd.Parameters.AddWithValue("@EnterpriseSite", this._EnterpriseSite);
				cmd.Parameters.AddWithValue("@ServerEndPoint", this.serverEndPoint);
				cmd.Parameters.AddWithValue("@SecurityMode", this.securityMode);
				cmd.Parameters.AddWithValue("@SecurityPolicy", this.securityPolicy);
				cmd.Parameters.AddWithValue("@MessageEncoding", this.messageEncoding);
				cmd.Parameters.AddWithValue("@UserIdentityMethod", this.userIdentityMethod);
				cmd.Parameters.AddWithValue("@UserId", this.userId);
				cmd.Parameters.AddWithValue("@UserPassword", this.userPassword);
				cmd.Parameters.AddWithValue("@UserCertificatePath", this.userCertificatePath);
				cmd.Parameters.AddWithValue("@MaximumDaysToRetainArchive", this.MaximumDaysToRetainArchive);
			}
			else if (type == DATA_TYPE.SYNCCONFIG)
			{
				cmd.CommandText = "UPDATE tblSites " +
					"SET EnablePeriodicSyncFlag = @EnablePeriodicSyncFlag, " +
					"PeriodicSyncIntervalMinutes = @PeriodicSyncIntervalMinutes, " +
					"DisableSyncTransferFlag = @DisableSyncTransferFlag," +
					// Audit Data
					"UpdatedDate = @UpdatedDate, " +
					"UpdatedBy = @UpdatedBy " +
					" WHERE SiteGuid = @SiteGuid";

				cmd.Parameters.AddWithValue("@EnablePeriodicSyncFlag", (this._EnablePeriodicSyncFlag ? 1 : 0));
				cmd.Parameters.AddWithValue("@PeriodicSyncIntervalMinutes", this._PeriodicSyncIntervalMinutes);
				cmd.Parameters.AddWithValue("@DisableSyncTransferFlag", this._DisableSyncTransferFlag ? 1 : 0);
				cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
				cmd.Parameters.AddWithValue("@UpdatedBy ", this._UpdatedBy);
				cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			}
			else if (type == DATA_TYPE.DYNAMIC)
			{
					cmd.CommandText = "UPDATE tblSites " +
						"SET UseLastKnownGoodTankData = @UseLastKnownGoodTankData, " +
						"VRURateActual = @VRURateActual, " +
						"VRUHourlyActual = @VRUHourlyActual, " +
						"VRUDailyActual = @VRUDailyActual, " +
						"VRUYearlyActual = @VRUYearlyActual, " +
						"VRUCurrentYearActual = @VRUCurrentYearActual, " +

						// Audit Data
						"UpdatedDate = @UpdatedDate, " +
						"UpdatedBy = @UpdatedBy " +
						" WHERE SiteGuid = @SiteGuid";

					cmd.Parameters.AddWithValue("@UseLastKnownGoodTankData", (this.UseLastKnownGoodTankData ? 1 : 0));
					cmd.Parameters.AddWithValue("@VRURateActual", this._VRURateActual.SIValue);
					cmd.Parameters.AddWithValue("@VRUHourlyActual", this._VRUHourlyActual.SIValue);
					cmd.Parameters.AddWithValue("@VRUDailyActual", this._VRUDailyActual.SIValue);
					cmd.Parameters.AddWithValue("@VRUYearlyActual", this._VRUYearlyActual.SIValue);
					cmd.Parameters.AddWithValue("@VRUCurrentYearActual", this._VRUCurrentYearActual.SIValue);
					cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
					cmd.Parameters.AddWithValue("@UpdatedBy ", this._UpdatedBy);
					cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
				}
				else
				{
					cmd.CommandText = "UPDATE tblSites " +
						"SET AutomaticBOLNextNumber = @AutomaticBOLNextNumber, " +
						"ManualBOLNextNumber = @ManualBOLNextNumber, " +
						"TransactionNextNumber = @TransactionNextNumber, " +
						"OrderNextNumber = @OrderNextNumber, " +
						// vt 07-15-2008
						"InvoiceNextNumber = @InvoiceNextNumber, " +

						// Audit Data
						"UpdatedDate = @UpdatedDate, " +
						"UpdatedBy = @UpdatedBy " +
						" WHERE SiteGuid = @SiteGuid";

					cmd.Parameters.AddWithValue("@AutomaticBOLNextNumber", this._AutomaticBOLNextNumber);
					cmd.Parameters.AddWithValue("@ManualBOLNextNumber", this._ManualBOLNextNumber);
					cmd.Parameters.AddWithValue("@TransactionNextNumber", this._TransactionNextNumber);
					cmd.Parameters.AddWithValue("@OrderNextNumber", this._OrderNextNumber);
					cmd.Parameters.AddWithValue("@InvoiceNextNumber", this._InvoiceNextNumber);
					cmd.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
					cmd.Parameters.AddWithValue("@UpdatedBy ", this._UpdatedBy);
					cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
				}
		}

		/// <summary>
		/// The select SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL Command.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <param name="getAssociatedAliases">
		/// The get associated aliases.
		/// </param>
		public void SelectSQL(SqlCommand cmd, bool inTransaction, bool getAssociatedAliases)
		{
				// changed join on tblSiteAncillaryData to outer join, as sites created/populated by sync may not have ancillary data
			cmd.CommandText = this.SelectClause 
							+ " FROM tblSites Sites " + SQLUpdateLock(inTransaction) 
							+ " LEFT OUTER JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid"
							+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
							+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
							+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
							+ " WHERE Sites.SiteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}

		/// <summary>
		/// Special partial select command used by synchronization when to load only the main Site record without needing the Ancillary data.
		/// </summary>
		/// <param name="cmd">
		/// The SQL Command.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		/// <param name="getAssociatedAliases">
		/// The get associated aliases.
		/// </param>
		public void SelectPartialSQL(SqlCommand cmd, bool inTransaction, bool getAssociatedAliases)
		{
				cmd.CommandText = "SELECT SiteGuid, ID, Number, SPLCCode, Enabled, SiteGroupFlag, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, EnablePeriodicSyncFlag, PeriodicSyncIntervalMinutes, DisableSyncTransferFlag " 
										+ " FROM [dbo].[tblSites] " + SQLUpdateLock(inTransaction)
										+ " WHERE SiteGuid = @SiteGuid";

				cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}

		/// <summary>
		/// The select by ID SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="inTransaction">
		/// The in transaction.
		/// </param>
		public void SelectByIdsql(SqlCommand cmd, bool inTransaction = false)
		{
			cmd.CommandText = this.SelectClause 
				+ " FROM tblSites Sites " + SQLUpdateLock(inTransaction) 
				+ " LEFT OUTER JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid" 
				+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
				+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
				+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
				+ " WHERE Sites.ID = @SID";

			var prm = new SqlParameter("@SID", SqlDbType.VarChar, 30) { Direction = ParameterDirection.Input, Value = this.ID };
			cmd.Parameters.Add(prm);
		}

		/// <summary>
		/// Returns SQL query to determine if site is a site group.
		/// </summary>
		/// <param name="cmd">The CMD.</param>
		/// <param name="siteGuid">Identity Guid of the site.</param>
		public static void IsGroupSiteSQL( SqlCommand cmd, Guid siteGuid )
		{
			cmd.CommandText = "SELECT SiteGroupFlag FROM tblSites WHERE SiteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue( "@SiteGuid", siteGuid );
		}

		/// <summary>
		/// The enumerate SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause 
					+ " FROM tblSites Sites" 
					+ " JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid" 
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
					+ " ORDER BY Sites.ID";
			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}

		/// <summary>
		/// The SQL to get a Site's PointGroupExportDirectory/FileName.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="siteGuid">
		/// The site Guid.
		/// </param>
		public static void GetPointGroupExportDirectoryFileNameSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "Select PointGroupFileExportDirectory, PointGroupDefaultFileName FROM tblSites Where SiteGuid = @SiteGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		/// <summary>
		/// The SQL to get a Site's Movement Ticket Export Directory/FileName.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="siteGuid">
		/// The site Guid.
		/// </param>
		public static void GetMovementTicketExportDirectoryFileNameSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "Select MovementTicketFileExportDirectory, MovementTicketExportFileName FROM tblSites Where SiteGuid = @SiteGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		/// <summary>
		/// The SQL to get a Site's row version.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="siteGuid">
		/// The site Guid.
		/// </param>
		public static void GetRowVersionSQL(SqlCommand cmd, Guid siteGuid)
		{
			cmd.CommandText = "Select _RowVersion FROM tblSites Where SiteGuid = @SiteGuid";
			cmd.Parameters.AddWithValue("@SiteGuid", siteGuid);
		}

		  /// <summary>
		  /// This method will add an enumerate email information select to the SQL command.
		  /// </summary>
		  /// <param name="sqlCommand">The SQL command</param>
		  public void EnumerateMailInfoSql(SqlCommand sqlCommand)
		  {
				const string Sql = "SELECT SiteGuid, ID, MailServer, EmailAddress, DialupName, MailFrom, LookupMailConnectModeIndex, MailUserName, MailPassword "
										  + "FROM tblSites WHERE MailServer IS NOT NULL AND MailServer <> '' ";
				sqlCommand.CommandText = Sql;
		  }

		  /// <summary>
		  /// This method will add an enumerate report directory select to the SQL command.
		  /// </summary>
		  /// <param name="sqlCommand">
		  /// The SQL command.
		  /// </param>
		  public void EnumerateReportDirectorySql(SqlCommand sqlCommand)
		{
			const string Sql = "SELECT Sites.ManagedReportDirectory, Sites.ManageReports FROM tblSites Sites ";
			sqlCommand.CommandText = Sql;
		}

		/// <summary>
		/// This method will populate the SQL command with an enumerate all index ID site group SQL.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void EnumerateAllIndexIdSiteGroupSql(SqlCommand sqlCommand)
		{
			const string Sql = "SELECT Sites.ID, Sites.SiteGuid, Sites.SiteGroupFlag " +
								"FROM tblSites Sites " +
								"ORDER BY Sites.ID";

			sqlCommand.CommandText = Sql;
		}

		/// <summary>
		/// The enumerate by site group SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		public void EnumerateBySiteGroupSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause 
					+ " FROM tblSites Sites JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid" 
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
					+ " WHERE Sites.SiteGroupFlag = @SiteGroupFlag"
					+ " ORDER BY Sites.ID";

			cmd.Parameters.AddWithValue("@SiteGroupFlag", this.SiteGroup ? 1 : 0);
		}

		/// <summary>
		/// The enumerate by parent site SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL Command.
		/// </param>
		public void EnumerateByParentSiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause
					+ " FROM tblSites Sites" 
					+ " JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid"
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
					+ " INNER JOIN map.tblSiteToSite SiteMap ON SiteMap.ChildSiteGuid = Sites.SiteGuid" 
					+ " WHERE SiteMap.ParentSiteGuid = @SiteGuid" 
					+ " ORDER BY Sites.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}

		/// <summary>
		/// The enumerate by parent site and assigned user SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="userGuid">
		/// The user GUID.
		/// </param>
		public void EnumerateByParentSiteAndAssignedUserSQL(SqlCommand cmd, Guid userGuid)
		{
			cmd.CommandText =	this.SelectClause 
								+ " FROM dbo.tblSites Sites " 
								+ " JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid" 
								+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
								+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
								+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
								+ " JOIN map.tblEntityUserToSite m ON Sites.SiteGuid = m.SiteGuid AND [UserGuid] = @UserGuid" 
								+ " INNER JOIN map.tblSiteToSite SiteMap ON SiteMap.ChildSiteGuid = Sites.SiteGuid" 
								+ " WHERE SiteMap.ParentSiteGuid = @SiteGuid" 
								+ " ORDER BY Sites.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
		}

		  //enumerate all the site related to a user
		 public void EnumerateByUser(SqlCommand cmd, Guid userGuid)
		 {
			  cmd.CommandText = "SELECT * FROM tblSites s JOIN map.tblEntityUserToSite e ON s.SiteGuid = e.SiteGuid "
									  + "WHERE e.UserGuid = @UserGuid";

			  cmd.Parameters.AddWithValue("@UserGuid", userGuid);
		 }
		/// <summary>
		/// The enumerate by child site SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL Command.
		/// </param>
		public void EnumerateByChildSiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause 
					+ " FROM tblSites Sites" 
					+ " JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid" 
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
					+ " INNER JOIN map.tblSiteToSite SiteMap ON SiteMap.ParentSiteGuid = Sites.SiteGuid" 
					+ " WHERE SiteMap.ChildSiteGuid = @SiteGuid" 
					+ " ORDER BY Sites.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}


		public void EnumerateByCandidateChildrenSitesSQL(SqlCommand cmd)
		{
			cmd.CommandText = "map.usp_GetCandidateChildrenSites";
			cmd.CommandType = CommandType.StoredProcedure;
			cmd.Parameters.AddWithValue("@SiteGroupGuid", this.SiteGuid);
		}

		/// <summary>
		/// The enumerate by child site for user SQL.
		/// </summary>
		/// <param name="cmd">
		/// The SQL command.
		/// </param>
		/// <param name="userGuid">
		/// The user GUID.
		/// </param>
		public void EnumerateByChildSiteForUserSQL(SqlCommand cmd, Guid userGuid)
		{
			cmd.CommandText = this.SelectClause 
					+ " FROM dbo.tblSites Sites" 
					+ " JOIN tblSitesAncillaryData AncillaryData ON Sites.SiteGuid = AncillaryData.SiteGuid" 
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases1 ON TransAliases1.TransactionAliasGuid = AncillaryData.InventoryTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblTransactionAliases TransAliases2 ON TransAliases2.TransactionAliasGuid = AncillaryData.AdjustmentTransactionAliasGuid"
					+ " LEFT OUTER JOIN tblIATA Iata ON Iata.IATAGuid = AncillaryData.IATAGuid"
					+ " INNER JOIN map.tblSiteToSite SiteMap ON SiteMap.ParentSiteGuid = Sites.SiteGuid" 
					+ " INNER JOIN (SELECT DISTINCT UserGroup.SiteGid FROM map.tblUserToGroup UserGroup WHERE UserGroup.UserGuid = "
					+ " @UserGuid) AccessSites ON Sites.SiteGuid = AccessSites.SiteGuid " 
					+ " WHERE SiteMap.ChildSiteGuid = @SiteGuid" 
					+ " ORDER BY Sites.ID";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
			cmd.Parameters.AddWithValue("@UserGuid", userGuid);
		}

		/// <summary>
		/// The enumerate limit site member by parent site SQL.
		/// </summary>
		/// <param name="sqlCommand">
		/// The SQL command.
		/// </param>
		public void EnumerateLimitSiteMemberByParentSiteSql(SqlCommand sqlCommand)
		{
			const string Sql = "SELECT Sites.SiteGuid, Sites.ID, Sites.Number, Sites.SiteGroupFlag"
								+ " FROM dbo.tblSites Sites LEFT JOIN map.tblSiteToSite SiteMap ON SiteMap.ChildSiteGuid = Sites.SiteGuid"
								+ " WHERE SiteMap.ParentSiteGuid = @SiteGuid AND SiteMap.ChildSiteGuid = Sites.SiteGuid"
								+ " ORDER BY Sites.ID";

			sqlCommand.CommandText = Sql;

			var parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = this.SiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		public void EnumerateSiteSynchronizationListBySiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = "dbo.usp_GetSiteToSiteSynchronizationListForSiteID";
			cmd.CommandType = CommandType.StoredProcedure;

			cmd.Parameters.AddWithValue("@SiteID", this.ID);
		}

		public void EnumerateSiteInfoSQL(SqlCommand cmd)
		{
			cmd.CommandText = "SELECT [SiteGuid], [ID], [AdministrativeLockDate], [OperationalLockDate], [SiteGroupFlag], [Number] FROM tblSites";
		}

		public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText =	"DELETE FROM tblSitesAncillaryData WHERE SiteGuid = @SiteGuid"+
								" EXEC dbo.usp_tblSites_ApplicationDelete @SiteGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}

		public void UpdatedDateSQL(SqlCommand cmd, bool bInTransaction)
		{
			cmd.CommandText = "SELECT [UpdatedDate] FROM tblSites WITH (NOLOCK) WHERE SiteGuid = @SiteGuid";

			cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
		}

		public void CreateDefaultSingleSiteSQL(SqlCommand cmd)
		{
			cmd.CommandText = "usp_CreateDefaultSingleSite";
			cmd.CommandType = CommandType.StoredProcedure;
		}

		public void HasDatabaseChangedSQL(SqlCommand cmd)
		{
				cmd.CommandText = "dbo.usp_CheckForNewerSiteRecordByUpdatedDate";
				cmd.CommandType = CommandType.StoredProcedure;

				cmd.Parameters.AddWithValue("@SiteGuid", this.SiteGuid);
				cmd.Parameters.AddWithValue("@UpdatedDate", this.UpdatedDate);
		}

		public EngineeringUnit GetSiteUnits(SITE_VARIABLE_TYPE type)
		{
			switch (type)
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

				case SITE_VARIABLE_TYPE.ADDITIVE_CYCLE_AMOUNT:
					return this.AdditiveProfileCycleAmountUnits;

				case SITE_VARIABLE_TYPE.ADDITIVE_RATE_AMOUNT:
					return this.AdditiveProfileRateUnits;

				default:
					return EngineeringUnit.FmduPCent;
			}
		}

		public byte GetSiteDecimalPlaces(SITE_VARIABLE_TYPE type)
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

				case SITE_VARIABLE_TYPE.ADDITIVE_CYCLE_AMOUNT:
					return this._AdditiveProfileCycleAmountDecimalPlaces;

				case SITE_VARIABLE_TYPE.ADDITIVE_RATE_AMOUNT:
					return this._AdditiveProfileRateDecimalPlaces;

				default:
					return 2;
			}
		}

		public int[] GetNumberGroupSizes()
		{
			int[] numberGroupSizes;
			switch (this.NumberGroupSizesType)
			{
				case NUMBER_GROUP_SIZES_TYPE.ZERO:
					numberGroupSizes = new int[1];
					numberGroupSizes[0] = 0;
					break;
				case NUMBER_GROUP_SIZES_TYPE.THREE:
					numberGroupSizes = new int[1];
					numberGroupSizes[0] = 3;
					break;
				case NUMBER_GROUP_SIZES_TYPE.TWOTHREE:
					numberGroupSizes = new int[2];
					numberGroupSizes[0] = 3;
					numberGroupSizes[1] = 2;
					break;
				default:
					numberGroupSizes = new int[1];
					numberGroupSizes[0] = 0;
					break;
			}

			return numberGroupSizes;
		}

		/// <summary>
		/// Validate the input of Meter Factor. The Meter Factor must be present, numeric, and greater than zero.
		/// This method will throw if an error is detected
		/// </summary>
		/// <param name="meterFactor">The text the user typed in for the meter factor</param>
		/// <returns>If successful, the meter factor as a double</returns>
		public static int ValidateMaxOperateTabsAllowed(string maxTabsAllowedText)
		{
			// Validate the input of MaxOperateTabsAllowed. It must be present, whole number greater than zero.
			int maxTabsAllowed;

			Regex isRightDecimalFmt = new Regex(@"^([1-9]|[1-9][0-9]|[1-9][0-9][0-9])$");
			maxTabsAllowedText = maxTabsAllowedText.Trim();

			if (string.IsNullOrEmpty(maxTabsAllowedText))
			{
				throw new ApplicationException("Max Tabs Allowed In Operate is required");
			}
			else if (isRightDecimalFmt.IsMatch(maxTabsAllowedText))
			{
				bool _ = int.TryParse(maxTabsAllowedText, out maxTabsAllowed);
			}
			else
			{
				throw new ApplicationException("Max Tabs Allowed In Operate must be positive number in the range from 1 to 999");
			}

			return maxTabsAllowed;
		}

		/// <summary>
		/// This method initializes the object.
		/// </summary>
		private void Initialize()
		{
			base.Reset();

			// General
			this._Number = "";
			this._SPLCCode = "";
			this._Address1 = "";
			this._Address2 = "";
			this._City = "";
			this._State = "";
			this._Zip = "";
			this._Country = "";
			this._Phone = "";
			this._Fax = "";
			this._EmailAddress = "";
			this._EmergencyContact = "";
			this._EmergencyPhone = "";
			this._Enabled = true;
			this._SiteGroup = false;
			this._TimeZone = "Eastern Standard Time";
			this._TerminalControlNumber = string.Empty;
			this._InhibitLoadRackCardIns = false;
			this._inhibitSiteLedgerRollup = false;
			this._EnforceSingleOwner = false;
			this._InhibitBOLSummaryAutoPopulate = false;
			this._InhibitOrderSummaryAutoPopulate = false;
			this._InhibitSupplyOrderSummaryAutoPopulate = false;
			this._IATAGuid = Guid.Empty;
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
			this.latitude = null;
			this.longitude = null;
			this.zoom = null;
			this.activeDirectorySiteGroupGuid = Guid.Empty;

			var strMaxOperateTabsAllowed = FMBusinessObjects.UtilityObjects.AppSettingsHelper.GetKeyValue<string>("MaxOperateTabsAllowedPerSite", "10");
			this.MaxOperateTabsAllowed = Convert.ToInt32(strMaxOperateTabsAllowed);

			this.CloseoutTime = null;

			// Units
			this._LevelUnits = EngineeringUnit.FmlFtIn16Th;
			this._TemperatureUnits = EngineeringUnit.FmtDegF;
			this._DensityUnits = EngineeringUnit.FmdDegApi;
			this._PressureUnits = EngineeringUnit.FmpPsi;
			this._FlowUnits = EngineeringUnit.FmvfGpm;
			this._VolumeUnits = EngineeringUnit.FmvUsGal;
			this._MassUnits = EngineeringUnit.FmmLb;
			this._AdditiveVolumeUnits = EngineeringUnit.FmvCm3;
			this._AdditiveProfileCycleAmountUnits = EngineeringUnit.FmvCm3;
			this._AdditiveProfileRateUnits = EngineeringUnit.FmvUsGal;
			this._LevelDecimalPlaces = 2;
			this._TemperatureDecimalPlaces = 0;
			this._DensityDecimalPlaces = 1;
			this._PressureDecimalPlaces = 2;
			this._FlowDecimalPlaces = 1;
			this._VolumeDecimalPlaces = 0;
			this._MassDecimalPlaces = 0;
			this._AdditiveVolumeDecimalPlaces = 0;
			this._AdditiveProfileCycleAmountDecimalPlaces = 0;
			this._AdditiveProfileRateDecimalPlaces = 0;
			this._QuantityDisplayDefault = QuantityDisplay.GROSS; /*Gross*/

			// Password Hints/Forgotten Password
			this._EnablePasswordReset = false;
			this._EnablePasswordHint = false;

			this._AllowUseOfSpecialChars = true;

			this._EnablePeriodicSyncFlag = false;
			this._PeriodicSyncIntervalMinutes = 0;
			this._DisableSyncTransferFlag = false;

			// Load Rack
			this._InhibitAccessAfterHours = false;
			this._InhibitMultipleCardIns = true;
			this._AccessCardInRequired = true;
			this._CheckSiteNumber = false;
			this._PromptForCustomerCard = true;
			this._PromptForTractorOrTanker = false;
			this._PromptForFirstTrailer = false;
			this._PromptForSecondTrailer = false;
			this._PromptForThirdTrailer = false;
			this._PromptForCompartment = false;
			this._PromptForTransactionCompletion = false;
			this._InhibitCustomerConfirmationPrompt = false;
			this._RequireTrailerScully = false;
			this._CardInTimeout = 30;
			this._EnforceDriverEquipmentMatch = true;
			this._EnableAdditiveAccounting = true;
			this._UseCompanyEquipmentIdentifiers = false;
			this._UseLastKnownGoodTankData = false;
			this._MaximumLoadAmount = new SIDouble(this._VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), 1.3209);
			this._MaximumLoadTime = 720; // 12 hours
			this._MaximumIdleTime = 10;
			this._MaximumFlushAmount = new SIDouble(this._VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), .1321);
			this._MaximumMeterProvingAmount = new SIDouble(this._VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), .1321);
			this._MaximumReturnsAmount = new SIDouble(this._VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.VOLUME), .1321);
			this._MaximumNumberOfActiveArms = 10;
			this._DriverTimeoutPeriod = 90;
			this._DriverWarningPeriod = 5;
			this._MaximumPrompts = 3;
			this._InventoryTransactionAliasGuid = Guid.Empty;
			this._AdjustmentTransactionAliasGuid = Guid.Empty;
			this._MaximumVehicleWeight = new SIDouble(this._MassUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.MASS), 36287.392);
			this._LoadByNet = false;
			this._PromptForShipmentNumber = false;
			this._MaximumProductTemperature = new SIDouble(this._TemperatureUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.TEMPERATURE), 15.56);
			this._ListEquipment = false;
			this._DeferStationChanges = false;
			this._PromptForReturns = false;
			this._PromptForTruckCard = false;
			this._StartingShortCardNumber = 1;
			this._UseShortCardNumber = false;
			this._ExcessVarianceCount = 2;
			this._ExcessVarianceTolerance = 2.0;
			this._SecondaryStorageFillMethod = FILL_METHOD.ACTUAL;
			this._EnforceSalesOrderLimit = false;
			
			this._LeakDetectionQuietSamples = 6;
			this._LeakDetectionQuietTime= 1440;
			this._LeakDetectionQuietTimeFactor= 8;
			this._LeakDetectionUseMinWait=	false;
			this._LeakDetectionReport = string.Empty;
			this._LeakDetectionPrinter = string.Empty; 

			// Transactions
			this._InhibitBOLWithBrokenBlends = true;
			this._InhibitBOLWithImproperAdditization = true;
			this._InhibitOverweightBOL = true;
			this._ExceptionBOLPrinter = "";
			this._EnableAutomaticBOLPrinting = true;
			this._AutomaticBOLStartNumber = 0;
			this._AutomaticBOLEndNumber = 10000000;
			this._AutomaticBOLNextNumber = 0;
			this._SeparateManualBOLNumbering = false;
			this._ManualBOLStartNumber = 0;
			this._ManualBOLEndNumber = 10000000;
			this._ManualBOLNextNumber = 0;
			this._TransactionStartNumber = 0;
			this._TransactionEndNumber = 10000000;
			this._TransactionNextNumber = 0;
			this._OrderStartNumber = 0;
			this._OrderEndNumber = 10000000;
			this._OrderNextNumber = 0;
			this._InvoiceStartNumber = 0;
			this._InvoiceEndNumber = 10000000;
			this._InvoiceNextNumber = 0;
			this._NumberPrefix = "%Date%";
			this._OpenTransactionWindow = 2;
			this._AdministrativeLockDate.Value = TimeConverter.Today().AddDays(-1);
			this._OperationalLockDate.Value = DateTimeOffset.Now.AddDays(-1);
			this._EnableBOLPDFArchiving = false;
			this._BOLPDFArchivingPath = "";

			// System
			this._MaximumDaysToRetainLogs = 60;
			this._EnableDebugLogging = false;
			this._EnableAuditLogging = true;
			this._AutomaticallyPrintAlarmsAndEvents = false;
			this._AlarmAndEventPrinter = "";
			this._MailServer = "localhost";
			this._MailFrom = "";
			this._MailUserName = "";
			this._MailPassword = "";
			this._MailConnectMode = MAIL_SERVER_CONNECT_MODE.LAN;
			this._DialupName = "";
			this._SCADASystem = "localhost";
			this._InhibitTemplateGraphics = false;
			this._RefreshInterval = 5;
			this._InhibitEndOfDayOperations = false;
			this._InhibitEndOfMonthOperations = false;
			this._EndOfDayWarningPeriod = 30;
			this._InhibitAutomaticPhysicalInventory = false;
			this._InhibitAutomaticMeterCloseout = true;
			this._InhibitAutomaticReportGeneration = true;
			this._InhibitAutomaticAdjustmentDistribution = true;
			this._InhibitAutomaticCloseout = true;
			this._BlockCloseOnUnpostedBol = false;
			this._InhibitTankScan = false;
			this._ReportDirectory = "/Standard Reports";
			this._ManageReports = false;
			this._ManagedReportDirectory = "";
			this._ExportArchiveDir = "";
			this._ImportArchiveDir = "";
			this._GroupLedgerByID = false;
			this._MeterReconciliationReportName = string.Empty;
			this._MeterReconciliationToleranceIsPercent = false;
			this._TranslatedHelpURL = string.Empty;

			// Vapor Recovery Unit (VRU)
			this._VRURateLimit = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUHourlyLimit = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUDailyLimit = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUYearlyLimit = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUCurrentYearLimit = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRURateActual = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUHourlyActual = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUDailyActual = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUYearlyActual = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRUCurrentYearActual = new SIDouble(this.VolumeUnits, this.GetNumberFormatInfo(SITE_VARIABLE_TYPE.FLOW), 0);
			this._VRURateLimitEnabled = false;
			this._VRUHourlyLimitEnabled = false;
			this._VRUDailyLimitEnabled = false;
			this._VRUYearlyLimitEnabled = false;
			this._VRUCurrentYearLimitEnabled = false;


			// Regional Settings
			this._NumberGroupSizesType = NUMBER_GROUP_SIZES_TYPE.THREE;
			this._NumberDecimalSeparator = ".";
			this._NumberGroupSeparator = ",";
			this._ListSeparator = ",";
			this._TimePattern = "hh:mm:ss tt";
			this._TimeSeparator = ":";
			this._AMSymbol = "AM";
			this._PMSymbol = "PM";
			this._ShortDatePattern = "M/d/yyyy";
			this._DateSeparator = "/";
			this._LongDatePattern = "ddddd, MMMMM dd, yyyy";
			this._TwoDigitCalendarEndYear = 2029;

			// Process Variables
			this._WatchdogPeriod = 10;
			this._WatchdogMode = WATCHDOG_MODE.TOGGLE;
			this._WatchdogCounterStart = 0;
			this._WatchdogCounterEnd = 1000;

			// Password configuration data members
			this.minTimeAllowedToChangePwd = 0;		// Tells the validator the minimum time to allow changes
			this.minPwdCharacterLength = UserClass.UserDataCount;	// Tells the validator the minimum characters allow for the pwd
			this.pwdExpirationInDays = 999;			// Tells the validator when the pwd has expired
			this.pwdLockoutThreshold = 0;			// Tells the validator to lockout the user after X failures
			this.pwdHistoryCount = 0;				// Tells the validator how many previous pwds to compare to
			this.checkForPreviousPwd = false;		// Tells the validator whether to check the pwd history for a match
			this.StrongPwdUse = (int)StrongPasswordUsage.None;		// Tells the validator whether to check for a strong type Password
			this.applyToAllSiteMembers = false;	// If true, then apply the Password settings to all children sites.
			this.inactivityDisablePeriod = 0;		// Tells the validator the period of time to lockout if during inactivity
			this.disableArchivePeriod = 0;			// Tells the validator the period of time to archive if during lockout

			this.useTankReconciliation = false;

			// Additional Data
			this.UserData = new UserDataClass();

			//Notes
			this.NoteGuid = Guid.Empty;
			this.Note = new NoteClass();

			this.InventoryTransactionAliasID = "";
			this.AdjustmentTransactionAliasID = "";
			this.IATAID = "";

			//Reports
			this._EnableMovementTicketPDFArchiving = false;
			this._PointGroupFileExportDirectory = "";
			this._PointGroupDefaultFileName = "%SiteID%_%PointGroupID%";
			this._MovementTicketFileExportDirectory = "";
			this._MovementTicketExportFileName = "%SiteID%_%MovementID%";

			DAY_OF_WEEK[] dayOfWeek = {DAY_OF_WEEK.SUNDAY,
										DAY_OF_WEEK.MONDAY,
										DAY_OF_WEEK.TUESDAY,
										DAY_OF_WEEK.WEDNESDAY,
										DAY_OF_WEEK.THURSDAY,
										DAY_OF_WEEK.FRIDAY,
										DAY_OF_WEEK.SATURDAY};

			this.OperatingScheduleCollection = new ScheduleCollectionClass();

			for (int item = 0; item < 7; item++)
			{
				ScheduleClass schedule = new ScheduleClass
												{
														Type = SCHEDULE_TYPE.TERMINAL_OPERATIONS_TYPE,
														Day = (int)dayOfWeek[item]
												};

				this.OperatingScheduleCollection.Add(schedule);
			}

			this.HolidayScheduleCollection = new ScheduleCollectionClass();

			PROCESS_VARIABLE_TYPE[] pvType ={PROCESS_VARIABLE_TYPE.SITE_ALARM_OUTPUT_PV,
														PROCESS_VARIABLE_TYPE.SITE_WATCHDOG_OUTPUT_PV,
														PROCESS_VARIABLE_TYPE.VRU_SETPOINT_PV,
														PROCESS_VARIABLE_TYPE.VRU_DEADBAND_PV,
														PROCESS_VARIABLE_TYPE.UNDEFINED_PV};

			this.ProcessVariableCollection = new ProcessVariableCollectionClass();

			int pv = 0;
			while (pvType[pv] != PROCESS_VARIABLE_TYPE.UNDEFINED_PV)
			{
				ProcessVariableClass processVariable = new ProcessVariableClass
																	{
																		UnitType = UNIT_TYPE.SITE_UNIT,
																		ProcessVariableType = pvType[pv]
																	};

				if (pvType[pv] == PROCESS_VARIABLE_TYPE.SITE_ALARM_OUTPUT_PV
				|| pvType[pv] == PROCESS_VARIABLE_TYPE.SITE_WATCHDOG_OUTPUT_PV)
					processVariable.DataType = VarEnum.VT_BOOL;
				else
				{
					processVariable.DataType = VarEnum.VT_R8;
					processVariable.SetValue(0.0, this.VolumeUnits);
				}
				processVariable.DataTypeEnabled = false;
				processVariable.Input = false;
				processVariable.InputEnabled = false;
				this.ProcessVariableCollection.Add(processVariable);
				pv++;
			}

			this.SiteCertificateCollection = new ApplicationStringCollectionClass();
			this.SiteToSiteMapCollection = new SiteToSiteMapCollectionClass();

			this._AdministrativeLockDate.Format = this.GetDateTimeFormatInfo();
			this._OperationalLockDate.Format = this.GetDateTimeFormatInfo();

			this._Enterprise = false;
			this._OperateTabGroups = true;

			 this._EnterpriseUserId = "";
			 this._EnterprisePassword = "";
			 this._EnterpriseSite = "";

			this.serverEndPoint = "";
			this.securityMode = "None";
			this.securityPolicy = "None";
			this.messageEncoding = "Binary";
			this.userIdentityMethod = "Anonymous";
			this.userId = "";
			this.userPassword = "";
			this.userCertificatePath = "";

			this.maximumDaysToRetainArchive = 365;
		}
	}
}
