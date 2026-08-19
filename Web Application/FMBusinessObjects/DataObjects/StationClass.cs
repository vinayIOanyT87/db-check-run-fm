using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace FMBusinessObjects.DataObjects
{
	public enum STATION_TYPE
	{
		ENTRY_GATE = 0,
		LOAD_RACK = 1,
		EXIT_GATE = 2,
		WEIGHT_SCALE = 3,
		BOL = 4,
		PRELOAD = 5,
		SIGNATURE = 6,
		METER = 7,
		OFF_LOADING = 8,
      MANUAL_BOL = 9,
      MAX_STATION_TYPE = 10,
    };

	public enum STATION_INTERFACE_TYPE
	{
		ACCULOADIII_Q = 0,
		PROXIMITY_CARD_READER = 1,
		PASS_CONTROLLER = 2,
		VAREC_DET = 3,
		MANUAL = 4,
		MICROLOAD_NET = 5,
		DANLOAD6000 = 6,
		MULTILOAD_II_SMP = 7,
		SIGNATURE = 8,
		METER = 9,
		ACCULOADIII_SA = 10,
		CONTREC1010 = 11,
		MULTILOAD_II = 12,
		CONTREC1010_RA = 13,
		SCULLY  = 14,
		RCU_II_OPEN = 15,
		RCU_II_RCU = 16,
		HID_CARD_READER = 17,
		REVUELTARADMTX = 18,
		OSDP_CARD_READER = 19,
		MAX_TYPE = 20
	};

	/// <summary>
	/// Summary description for StationCollectionClass.
	/// </summary>
	[Serializable()]
	[CollectionDataContract]
	public class StationCollectionClass : List<StationClass> { }

	/// <summary>
	/// Summary description for Station.
	/// </summary>
   [Serializable]
   [DataContract]
	public class StationClass : BaseDataObject, IAlarmAndEventDiscovery, IDataDictionary
	{
		[DataMember]
		STATION_TYPE _Type;
		[DataMember]
		bool _SwingArmPosition; // 1/0 = A/B
		[DataMember]
		bool _VaporRecovery;
		[DataMember]
		STATION_INTERFACE_TYPE _InterfaceType;
		[DataMember]
		bool _Enabled;
		[DataMember]
		bool _CardReader;
		[DataMember]
		bool _ThirtyFiveBitCardSupport;
		[DataMember]
		string _BOLPrinter;
		[DataMember]
		string _PreloadPrinter;
		[DataMember]
		int _BOLAgeInMinutes;
		[DataMember]
		Guid _IssueByVolumeTransactionAliasGuid;
		[DataMember]
		Guid _IssueByWeightTransactionAliasGuid;
		[DataMember]
		Guid _ReceiptByVolumeTransactionAliasGuid;
		[DataMember]
		Guid _ReceiptByWeightTransactionAliasGuid;
		[DataMember]
		int _NumberOfCopies;
		[DataMember]
		int _NumberOfPreloadCopies;
		[DataMember]
		bool _InhibitLoadingByLoadID;
		[DataMember]
		bool _InhibitOperatingModePrompt;
		[DataMember]
		bool _SynchronizeReferenceDensity;
		[DataMember]
		string _SignatureDevice;
		[DataMember]
		bool _SetDefaultPresetToZero;
		[DataMember]
		public ProcessVariableCollectionClass ProcessVariableCollection;
		[DataMember]
		public LoadArmCollectionClass LoadArmCollection;
		[DataMember]
		public PermissivesClass StationPermissives;
        [DataMember]
        bool _EnableScully;
        [DataMember]
        bool _EnableEquipmentValidate;
        [DataMember]
		private Guid associatedTankGuid;
		[DataMember]
		private string associatedTankId;
		[DataMember]
		private string armsServiced;
		[DataMember]
		private bool _InhibitSettingRecipeNames;
		[DataMember]
		int _SignatureDevicePort;
		[DataMember]
		int _SignatureDeviceBaudRate;
		[DataMember]
		private string _MeterRecircCardNumber;
		[DataMember]
		Guid _RecircTransactionAliasGuid;
		[DataMember]
		bool _EnableDynamicRecipes;

      [DataMember]
      public bool EthanolExcess { get; set; }

      [DataMember]
		public int StationPromptTimeout { get; set; }

		[DataMember]
		public int StationMessageTimeout { get; set; }

		// Data Obtained By Sub Query
		[DataMember]
		public string IssueByVolumeTransactionAliasID;

		[DataMember]
		public string IssueByWeightTransactionAliasID;

		[DataMember]
		public string ReceiptByVolumeTransactionAliasID;

		[DataMember]
		public string ReceiptByWeightTransactionAliasID;

		[DataMember]
		public string RecircTransactionAliasID;

		[DataMember]
		public bool _TouchKeyReader;

		[DataMember]
		public bool _OffLoadByOffLoadID;

		[DataMember]
		public bool _UseManualMeterData;

		[DataMember]
		public bool _PromptForBOLNumber;

        [DataMember]
        public bool _QueryForTrailers;

        [DataMember]
        public bool _PromptForGravity;

        [DataMember]
        public bool _PromptForTemperature;

        [DataMember]
		public int _LastTransactionNumber;

		[DataMember]
		public DateTimeOffset _LastTransactionNumberDateTime;

		[DataMember]
		public bool LogCommunications { get; set; }

		[DataMember]
		public string LogCommPath { get; set; }

		[DataMember]
		public QualificationMapCollectionClass ReqQualificationsCollection;

		[DataMember]
		public QualificationMapCollectionClass ReqTrainingCollection;

        [DataMember]
        public QualificationMapCollectionClass ReqLicenseCollection;

        [DataMember]
		public QualificationMapCollectionClass ReqTestsandInspectionsCollection;

        [DataMember]
        public QualificationMapCollectionClass ReqEquipmentTagAndLicenseCollection;

	    [DataMember]
	    public MeterClass Meter { get; set; }

		private const string ShutdownKey = "Transaction terminated due to Shutdown";
		public static AlarmAndEventDescriptorClass ShutdownAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ShutdownKey);

        private const string MaximumLoadTimeKey = "Transaction terminated due to Maximum Load Time";
		public static AlarmAndEventDescriptorClass MaximumLoadTimeAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, MaximumLoadTimeKey);

        private const string MaximumIdleTimeKey = "Transaction terminated due to Maximum Idle Time";
		public static AlarmAndEventDescriptorClass MaximumIdleTimeAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, MaximumIdleTimeKey);

        private const string ProductUnavailableKey = "Product Unavailable";
		public static AlarmAndEventDescriptorClass ProductUnavailableAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ProductUnavailableKey);

        private const string AdditiveProfileUnavailableKey = "Additive Profile Unavailable";
		public static AlarmAndEventDescriptorClass AdditiveProfileUnavailableAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, AdditiveProfileUnavailableKey);

        private const string BrokenBlendKey = "Broken Blend";
		public static AlarmAndEventDescriptorClass BrokenBlendAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, BrokenBlendKey);

        private const string ImproperAdditizationKey = "Improper Additization";
		public static AlarmAndEventDescriptorClass ImproperAdditizationAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ImproperAdditizationKey);

        private const string NoTankCertificationKey = "No Tank Certification";
		public static AlarmAndEventDescriptorClass NoTankCertificationAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, NoTankCertificationKey);

        private const string FailedCertificateOfAnalysisKey = "Failed Certificate Of Analysis";
		public static AlarmAndEventDescriptorClass FailedCertificateOfAnalysisAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, FailedCertificateOfAnalysisKey);

        private const string DevicePowerFailureKey = "Power Failure";
		public static AlarmAndEventDescriptorClass DevicePowerFailureDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, DevicePowerFailureKey);

        private const string LineItemsNotServedKey = "Station cannot load all items";
		public static AlarmAndEventDescriptorClass LineItemsNotServedDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, LineItemsNotServedKey);

        private const string ProductLockedOutKey = "Product Locked Out";
		public static AlarmAndEventDescriptorClass ProductLockedOutDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ProductLockedOutKey);

        private const string ExcessVehicleWeightKey = "Excess Vehicle Weight";
		public static AlarmAndEventDescriptorClass ExcessVehicleWeightAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ExcessVehicleWeightKey);

        private const string CreateCertificateOfAnalysisFailedKey = "Create Certificate Of Analysis Failed";
		public static AlarmAndEventDescriptorClass CreateCertificateOfAnalysisFailedAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, CreateCertificateOfAnalysisFailedKey);

        private const string InvalidStorageLocationKey = "Invalid Storage Location";
		public static AlarmAndEventDescriptorClass InvalidStorageLocationEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidStorageLocationKey);

        private const string NoProductAllocationKey = "No Product Allocation";
		public static AlarmAndEventDescriptorClass NoProductAllocationAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, NoProductAllocationKey);

        private const string NoProductPIDXAuthorizationKey = "No Product PIDX Authorization";
		public static AlarmAndEventDescriptorClass NoProductPIDXAuthorizationAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, NoProductPIDXAuthorizationKey);

        static readonly string OverrideNoProductPIDXAuthorizationKey = "Override No Product PIDX Authorization";
        public static AlarmAndEventDescriptorClass OverrideNoProductPIDXAuthorizationEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OverrideNoProductPIDXAuthorizationKey);

        private const string PIDXUnavailableKey = "PIDX Unavailable";
		public static AlarmAndEventDescriptorClass PIDXUnavailableAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PIDXUnavailableKey);

        static readonly string ErrorReadingMeterKey = "Error Reading Meter";
        public static AlarmAndEventDescriptorClass ErrorReadingMeterDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ErrorReadingMeterKey);

        private const string PidxDenialKey = "PIDX Denial";
		public static AlarmAndEventDescriptorClass PidxDenialAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, PidxDenialKey);

        private static readonly string OverridePidxDenialKey = "Override PIDX Denial";
        public static AlarmAndEventDescriptorClass OverridePIDXDenialEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OverridePidxDenialKey);

        private const string InvalidCardNumberKey = "Invalid Card Number";
		public static AlarmAndEventDescriptorClass InvalidCardNumberEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidCardNumberKey);

        private const string InvalidDriverIDKey = "Invalid Driver ID";
		public static AlarmAndEventDescriptorClass InvalidDriverIDEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidDriverIDKey);

        private const string InvalidTrailerIDKey = "Invalid Trailer ID";
		public static AlarmAndEventDescriptorClass InvalidTrailerIDEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidTrailerIDKey);

        private const string InvalidTractorOrTrankerIDKey = "Invalid Trackor/Tanker ID";
		public static AlarmAndEventDescriptorClass InvalidTractorOrTankerIDEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidTractorOrTrankerIDKey);

        private const string InvalidPinKey = "Invalid PIN";
		public static AlarmAndEventDescriptorClass InvalidPinEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidPinKey);

        private const string InvalidCustomerNumberKey = "Invalid Customer Number";
		public static AlarmAndEventDescriptorClass InvalidCustomerNumberEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidCustomerNumberKey);

        private const string InvalidCarrierKey = "Invalid Carrier";
		public static AlarmAndEventDescriptorClass InvalidCarrierEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidCarrierKey);

        private const string LoadingInquiryKey = "Loading Inquiry";
		public static AlarmAndEventDescriptorClass LoadingInquiryEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, LoadingInquiryKey);

        private const string PreviouslyLoadedInquiryKey = "Previously Loaded Inquiry";
		public static AlarmAndEventDescriptorClass PreviouslyLoadedInquiryEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, PreviouslyLoadedInquiryKey);

        private const string CompartmentEmptyInquiryKey = "Compartment Empty Inquiry";
		public static AlarmAndEventDescriptorClass CompartmentEmptyInquiryEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CompartmentEmptyInquiryKey);

        private const string BatchTotalDiscrepencyKey = "Batch Total Discrepency";
		public static AlarmAndEventDescriptorClass BatchTotalDiscrepencyEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, BatchTotalDiscrepencyKey);

        private const string SetAdditiveMeterTotalizerKey = "Set Additive Meter Totalizer";
		public static AlarmAndEventDescriptorClass SetAdditiveMeterTotalizerEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, SetAdditiveMeterTotalizerKey);

        private const string DriverNotQualifiedKey = "Driver Not Qualified";
		public static AlarmAndEventDescriptorClass DriverNotQualifiedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverNotQualifiedKey);

        private const string DriverNotTrainedKey = "Driver Not Trained";
		public static AlarmAndEventDescriptorClass DriverNotTrainedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverNotTrainedKey);

        private const string DriverNotLicensedKey = "Driver Not Licensed";
        public static AlarmAndEventDescriptorClass DriverNotLicensedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverNotLicensedKey);

        private const string EquipmentNotAuthorizedKey = "Equipment Not Authorized";
		public static AlarmAndEventDescriptorClass EquipmentNotAuthorizedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentNotAuthorizedKey);       

        private const string TerminalAccessNotScheduledKey = "Terminal Access Not Scheduled";
		public static AlarmAndEventDescriptorClass TerminalAccessNotScheduledEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, TerminalAccessNotScheduledKey);

        public const string LoadbyLoadidInhibitedKey = "Load by LoadID Inhibited";
        public static AlarmAndEventDescriptorClass LoadbyLoadidInhibitedDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, LoadbyLoadidInhibitedKey);

        public const string NoOrdersAvailableKey = "No Orders Available";
        public static AlarmAndEventDescriptorClass NoOrdersAvailableDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, NoOrdersAvailableKey);

        public const string MaxRetriesExceededKey = "Max Retries Exceeded";
        public static AlarmAndEventDescriptorClass MaxRetriesExceededKeyDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, MaxRetriesExceededKey);

        public const string NoCompartmentsToLoadKey = "No Compartments to Load";
        public static AlarmAndEventDescriptorClass NoCompartmentsToLoadDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, NoCompartmentsToLoadKey);

        public const string MismatchTractorOrTankerKey = "Mismatch Tractor/Tanker";
        public static AlarmAndEventDescriptorClass MismatchTractorOrTankerDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, MismatchTractorOrTankerKey);

        public const string MismatchTrailerKey = "Mismatch Trailer";
        public static AlarmAndEventDescriptorClass MismatchTrailerDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, MismatchTrailerKey);

        public static string BeginDriverDownloadKey = "Begin Driver Download";
        public static AlarmAndEventDescriptorClass BeginDriverDownloadDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, BeginDriverDownloadKey);

        public static string BeginEquipmentDownloadKey = "Begin Equipment Download";
        public static AlarmAndEventDescriptorClass BeginEquipmentDownloadDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, BeginEquipmentDownloadKey);

        public static string CardExpiredKey = "Card Expired";
        public static AlarmAndEventDescriptorClass CardExpiredKeyEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CardExpiredKey);

        public static string CompanyHierarchyInvalidKey = "Company Hierarchy is Invalid";
        public static AlarmAndEventDescriptorClass CompanyHierarchyInvalidDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, CompanyHierarchyInvalidKey);

        public static string DriverDownloadCompleteKey = "Driver Download Complete";
        public static AlarmAndEventDescriptorClass DriverDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverDownloadCompleteKey);

        public static string DriverDownloadInterruptedKey = "Driver Download Interrupted";
        public static AlarmAndEventDescriptorClass DriverDownloadInterruptedDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverDownloadInterruptedKey);

        public static string DriverDownloadLimitExceededKey = "Driver Download Limit Exceeded";
        public static AlarmAndEventDescriptorClass DriverDownloadLimitExceededDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, DriverDownloadLimitExceededKey);

        public static string EquipmentDownloadCompleteKey = "Equipment Download Complete";
        public static AlarmAndEventDescriptorClass EquipmentDownloadCompleteDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentDownloadCompleteKey);

        public static string EquipmentDownloadInterruptedKey = "Equipment Download Interrupted";
        public static AlarmAndEventDescriptorClass EquipmentDownloadInterruptedDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentDownloadInterruptedKey);

        public static string EquipmentDownloadLimitExceededKey = "Equipment Download Limit Exceeded";
        public static AlarmAndEventDescriptorClass EquipmentDownloadLimitExceededDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentDownloadLimitExceededKey);

        private const string EquipmentCardLogInKey = "Equipment Entry Successful";
        public static readonly AlarmAndEventDescriptorClass EquipmentCardLogInEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentCardLogInKey);

        //private const string EquipmentGroupCardLogInKey = "Equipment Group Entry Successful";
        //public static readonly AlarmAndEventDescriptorClass EquipmentGroupCardLogInEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, EquipmentGroupCardLogInKey);

        public static string ErrorSettingPermissiveKey = "Error Setting Permissive";
        public static AlarmAndEventDescriptorClass ErrorSettingPermissiveDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ErrorSettingPermissiveKey);

        public static string InvalidDriverIdentifierKey = "Invalid Driver Download Value";
        public static AlarmAndEventDescriptorClass InvalidDriverIdentifierDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidDriverIdentifierKey);

        public static string InvalidEquipmentIdentifierKey = "Invalid Equipment Download Value";
        public static AlarmAndEventDescriptorClass InvalidEquipmentIdentifierDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidEquipmentIdentifierKey);

        public static string InvalidHouseCardNumberKey = "Invalid House Card Number";
        public static AlarmAndEventDescriptorClass InvalidHouseCardNumberEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, InvalidHouseCardNumberKey);

        public static string NoProductsAvailableKey = "No Products Available";
        public static AlarmAndEventDescriptorClass NoProductsAvailableDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, NoProductsAvailableKey);

        public static string OfflineTransactionUploadedKey = "Offline Transaction Uploaded";
        public static AlarmAndEventDescriptorClass OfflineTransactionUploadedKeyDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OfflineTransactionUploadedKey);

        public static string OrderAliasInvalidKey = "Order associated alias invalid";
        public static AlarmAndEventDescriptorClass OrderAliasInvalidDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OrderAliasInvalidKey);

        public static string ScullyBypassUsedKey = "Scully Bypass Key Used";
        public static AlarmAndEventDescriptorClass ScullyBypassUsedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ScullyBypassUsedKey);

        public static string ScullyTIMNotDetectedKey = "Scully TIM Not Detected";
        public static AlarmAndEventDescriptorClass ScullyTIMNotDetectedEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ScullyTIMNotDetectedKey);

        public static string ScullyTIMNotMatchTruckCardNumberKey = "Scully TIM Not Match Truck Card Number";
        public static AlarmAndEventDescriptorClass ScullyTIMNotMatchTruckCardNumberEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, ScullyTIMNotMatchTruckCardNumberKey);

        private const string ExternalComponentPercentageBadKey = "External Component Blend Percentage Bad";
        public static readonly AlarmAndEventDescriptorClass ExternalComponentPercentageBadAlarmDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, ExternalComponentPercentageBadKey);

        public static string StationErrorKey = "Station Error";
        public static AlarmAndEventDescriptorClass StationErrorKeyDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, StationErrorKey);

        public static string InvalidStateForOperationKey = "The station is in an invalid state for the operation";
        public static AlarmAndEventDescriptorClass InvalidStateForOperationKeyDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, InvalidStateForOperationKey);

        public static string UnsupportedAdditiveKey = "Unsupported additive on arm";
        public static AlarmAndEventDescriptorClass UnsupportedAdditiveKeyDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, UnsupportedAdditiveKey);

        public static string UnsupportedComponentKey = "Unsupported component on arm";
        public static AlarmAndEventDescriptorClass UnsupportedComponentKeyDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, UnsupportedComponentKey);

        public static string OfflineLoadIdFallbackKey = "Offline Load ID fell back to default";
        public static AlarmAndEventDescriptorClass OfflineLoadIdFallbackKeyDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OfflineLoadIdFallbackKey);

        public static string OfflineOffloadIdFallbackKey = "Offline Offload ID fell back to default";
        public static AlarmAndEventDescriptorClass OfflineOffloadIdFallbackKeyDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, OfflineOffloadIdFallbackKey);

        public static string OfflineLoadIdFallbackFailedKey = "Unable to default invalid offline Load ID";
        public static AlarmAndEventDescriptorClass OfflineLoadIdFallbackFailedKeyDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, OfflineLoadIdFallbackFailedKey);

        public static string OfflineOffloadIdFallbackFailedKey = "Unable to default invalid offline Offload ID";
        public static AlarmAndEventDescriptorClass OfflineOffloadIdFallbackFailedKeyDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, OfflineOffloadIdFallbackFailedKey);

        public static string TrailerMissingScullyIDKey = "Trailer Missing Scully ID";
        public static AlarmAndEventDescriptorClass TrailerMissingScullyIDEventDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, TrailerMissingScullyIDKey);

        public static string FailedToSetDensityKey = "Failed To Set Density";
        public static AlarmAndEventDescriptorClass FailedToSetDensityDescriptor = new AlarmAndEventDescriptorClass(false, LoadRackKey, FailedToSetDensityKey);

			public static string DynamicRecipeDownloadErrorKey = "Failed To Dynamically Download the Recipe";
			public static AlarmAndEventDescriptorClass DynamicRecipeDownloadErrorDescriptor = new AlarmAndEventDescriptorClass(true, LoadRackKey, DynamicRecipeDownloadErrorKey);


		readonly string SelectClause = "SELECT tblStations.*," +
									"(SELECT AliasName FROM tblTransactionAliases WHERE tblTransactionAliases.TransactionAliasGuid = tblStations.IssueByVolumeTransactionAliasGuid) AS IssueByVolumeTransactionAliasID," +
									"(SELECT AliasName FROM tblTransactionAliases WHERE tblTransactionAliases.TransactionAliasGuid = tblStations.IssueByWeightTransactionAliasGuid) AS IssueByWeightTransactionAliasID," +
									"(SELECT AliasName FROM tblTransactionAliases WHERE tblTransactionAliases.TransactionAliasGuid = tblStations.ReceiptByVolumeTransactionAliasGuid) AS ReceiptByVolumeTransactionAliasID," +
									"(SELECT AliasName FROM tblTransactionAliases WHERE tblTransactionAliases.TransactionAliasGuid = tblStations.ReceiptByWeightTransactionAliasGuid) AS ReceiptByWeightTransactionAliasID, " +
									"(SELECT AliasName FROM tblTransactionAliases WHERE tblTransactionAliases.TransactionAliasGuid = tblStations.RecircTransactionAliasGuid) AS RecircTransactionAliasID, " +
									"(SELECT TankID FROM tblTanks WHERE tblTanks.TankGuid = tblStations.TankGuid) as AssociatedTankID ";

		public const int MinNumberOfCopies = 1;
		public const int MaxNumberOfCopies = 99;


		public override string ID { get { return this._ID; } set {
		    this.SetString("ID", 50, value, ref this._ID); } }

		/// <summary>
		/// Gets or sets the current type of the Station
		/// </summary>
		/// <remarks>
		/// When setting the station type, this property will copy over process variables
		/// appropriate to both types.
		/// Process variables used by the old type and not by the new type will be discarded.
		/// Process variables used by the new type but not by the old type will be added
		/// with default values.
		/// 
		/// Note that Signature stations have no PVs
		/// Meter stations have only one PV, METER_FLOW_TOTAL
		/// </remarks>
		public STATION_TYPE Type
		{
			get
			{
				return this._Type;
			}
			set
			{
				this._Type = value;
			    this.LoadArmCollection = new LoadArmCollectionClass();
				ProcessVariableCollectionClass newProcessVariableCollection = new ProcessVariableCollectionClass();

				bool gateControlFound = false;
				bool weightScaleFound = false;
				bool stationFound = false;
				bool meterTotalFound = false;

                // Preserve the process variables from the old type which still apply to the new type
                foreach (ProcessVariableClass processVariable in this.ProcessVariableCollection)
				{
					if (this._Type != STATION_TYPE.LOAD_RACK
					&& processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.START_PERMISSIVE_PV)
						continue;

					if (this._Type != STATION_TYPE.WEIGHT_SCALE
						&& processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV)
					{
						weightScaleFound = true;
						if (this._Type != STATION_TYPE.WEIGHT_SCALE)
							continue;
					}

					if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
					{
						gateControlFound = true;
						if (this._Type != STATION_TYPE.ENTRY_GATE
						&& this._Type != STATION_TYPE.EXIT_GATE)
							continue;
					}

					if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.STATION_PV)
					{
						stationFound = true;
						if (this._Type == STATION_TYPE.METER)
						{
							continue;
						}
					}

					if (processVariable.ProcessVariableType == PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV)
					{
						meterTotalFound = true;
						if (this._Type != STATION_TYPE.METER && this._Type != STATION_TYPE.OFF_LOADING)
						{
							continue;
						}
					}

					if (this._Type == STATION_TYPE.SIGNATURE)
					{
						// Signature stations do not connect with OPC devices or SCADA
						continue;
					}

					newProcessVariableCollection.Add(processVariable);
				}

			    this.ProcessVariableCollection = newProcessVariableCollection;

				// Add process variables which are in the new type but were not in the old type.

				// All types except for SIGNATURE and METER should have a STATION_PV
				if (!stationFound &&
					!(this._Type == STATION_TYPE.SIGNATURE || this._Type == STATION_TYPE.METER || this._Type == STATION_TYPE.OFF_LOADING))
				{
					ProcessVariableClass ProcessVariable = new ProcessVariableClass();
					ProcessVariable.UnitType = UNIT_TYPE.STATION_UNIT;
					ProcessVariable.ProcessVariableType = PROCESS_VARIABLE_TYPE.STATION_PV;
					ProcessVariable.DataType = VarEnum.VT_EMPTY;
					ProcessVariable.Input = true;
				    this.ProcessVariableCollection.Add(ProcessVariable);
				}

				// ENTRY_GATE and EXIT_GATE should have a GATE_CONTROL_PV
				if (!gateControlFound
				&& (this._Type == STATION_TYPE.ENTRY_GATE
				|| this._Type == STATION_TYPE.EXIT_GATE))
				{
					ProcessVariableClass ProcessVariable = new ProcessVariableClass();
					ProcessVariable.UnitType = UNIT_TYPE.STATION_UNIT;
					ProcessVariable.ProcessVariableType = PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV;
					ProcessVariable.DataType = VarEnum.VT_BOOL;
					ProcessVariable.Input = false;
				    this.ProcessVariableCollection.Add(ProcessVariable);
				}

				// Only METER should have METER_FLOW_TOTAL
				if (!meterTotalFound
				&& (this._Type == STATION_TYPE.METER || this._Type == STATION_TYPE.OFF_LOADING))
				{
					ProcessVariableClass ProcessVariable = new ProcessVariableClass();
					ProcessVariable.UnitType = UNIT_TYPE.STATION_UNIT;
					ProcessVariable.ProcessVariableType = PROCESS_VARIABLE_TYPE.COMPONENT_METER_FLOW_TOTAL_PV;
					ProcessVariable.DataType = VarEnum.VT_R8;
					ProcessVariable.Input = true;
				    this.ProcessVariableCollection.Add(ProcessVariable);
				}

				// Only WEIGHT_SCALE should have a WEIGHT_SCALE_PV
				if (!weightScaleFound
					&& this._Type == STATION_TYPE.WEIGHT_SCALE)
				{
					ProcessVariableClass ProcessVariable = new ProcessVariableClass();
					ProcessVariable.UnitType = UNIT_TYPE.STATION_UNIT;
					ProcessVariable.ProcessVariableType = PROCESS_VARIABLE_TYPE.WEIGHT_SCALE_PV;
					ProcessVariable.DataType = VarEnum.VT_BOOL;
					ProcessVariable.Input = false;
				    this.ProcessVariableCollection.Add(ProcessVariable);
				}
			}
		}

		public string SwingArmPosition
		{
			get
			{
				return (this._SwingArmPosition) ? "A" : "B";
			}
			set
			{
				if (value == "A") this._SwingArmPosition = true;
				else if (value == "B") this._SwingArmPosition = false;
				else
					throw new Exception("Invalid Swing Arm Position " + value);
			}
		}

		public bool VaporRecovery { get { return this._VaporRecovery; } set {
		    this._VaporRecovery = value; } }

		public STATION_INTERFACE_TYPE InterfaceType { get { return this._InterfaceType; } set {
		    this._InterfaceType = value; } }

		public bool Enabled { get { return this._Enabled; } set {
		    this._Enabled = value; } }

		public string BOLPrinter { get { return this._BOLPrinter; } set {
		    this.SetString("BOL Printer", 80, value, ref this._BOLPrinter); } }

		public string PreloadPrinter { get { return this._PreloadPrinter; } set {
		    this.SetString("Preload Printer", 80, value, ref this._PreloadPrinter); } }

		public int BOLAgeInMinutes { get { return this._BOLAgeInMinutes; } set {
		    this._BOLAgeInMinutes = value; } }

		public Guid IssueByVolumeTransactionAliasGuid { get { return this._IssueByVolumeTransactionAliasGuid; } set {
		    this._IssueByVolumeTransactionAliasGuid = value; } }

		public Guid IssueByWeightTransactionAliasGuid { get { return this._IssueByWeightTransactionAliasGuid; } set {
		    this._IssueByWeightTransactionAliasGuid = value; } }

		public Guid ReceiptByVolumeTransactionAliasGuid { get { return this._ReceiptByVolumeTransactionAliasGuid; } set {
		    this._ReceiptByVolumeTransactionAliasGuid = value; } }

		public Guid ReceiptByWeightTransactionAliasGuid { get { return this._ReceiptByWeightTransactionAliasGuid; } set {
		    this._ReceiptByWeightTransactionAliasGuid = value; } }

		public int SignatureDevicePort { get { return this._SignatureDevicePort; } set {
		    this._SignatureDevicePort = value; } }

		public int SignatureDeviceBaudRate { get { return this._SignatureDeviceBaudRate; } set {
		    this._SignatureDeviceBaudRate = value; } }

		public string SignatureDevice { get { return this._SignatureDevice; } set {
		    this.SetString("Signature Device", 20, value, ref this._SignatureDevice); } }

		public string MeterRecircCardNumber { get { return this._MeterRecircCardNumber; } set {
		    this.SetString("Meter Recirc Card Number", 30, value, ref this._MeterRecircCardNumber); } }

		public Guid RecircTransactionAliasGuid { get { return this._RecircTransactionAliasGuid; } set {
		    this._RecircTransactionAliasGuid = value; } }

		/// <summary>
		/// This property will return the number of copies to be printed.
		/// The property will ensure that the values is in a of 1 to 10.
		/// </summary>
		public int NumberOfCopies
		{
			get
			{
				return this._NumberOfCopies;
			}

			set
			{
				int copies = value;
				if ((copies < MinNumberOfCopies) || (copies > MaxNumberOfCopies))
				{
					this._NumberOfCopies = MinNumberOfCopies;
				}
				else
				{
					this._NumberOfCopies = value;
				}
			}
		}


		public int NumberOfPreloadCopies
		{
			get
			{
				return this._NumberOfPreloadCopies;
			}

			set
			{
				int copies = value;
				if ((copies < MinNumberOfCopies) || (copies > MaxNumberOfCopies))
				{
					this._NumberOfPreloadCopies = MinNumberOfCopies;
				}
				else
				{
					this._NumberOfPreloadCopies = value;
				}
			}
		}


		public bool InhibitLoadingByLoadID { get { return this._InhibitLoadingByLoadID; } set {
		    this._InhibitLoadingByLoadID = value; } }

        public bool EnableScully { get { return this._EnableScully;} set {
            this._EnableScully = value; } }

        public bool EnableEquipmentValidate { get { return this._EnableEquipmentValidate; } set {
            this._EnableEquipmentValidate = value; } }
        public bool InhibitOperatingModePrompt { get { return this._InhibitOperatingModePrompt; } set {
		    this._InhibitOperatingModePrompt = value; } }

		public bool SynchronizeReferenceDensity { get { return this._SynchronizeReferenceDensity; } set {
		    this._SynchronizeReferenceDensity = value; } }

		public bool SetDefaultPresetToZero { get { return this._SetDefaultPresetToZero; } set {
		    this._SetDefaultPresetToZero = value; } }

		public bool CardReader { get { return this._CardReader; } set { this._CardReader = value; } }

		public bool ThirtyFiveBitCardSupport { get { return this._ThirtyFiveBitCardSupport; } set { this._ThirtyFiveBitCardSupport = value; } }

		public Guid AssociatedTankGuid { get { return this.associatedTankGuid; } set { this.associatedTankGuid = value; } }

		public string AssociatedTankId { get { return this.associatedTankId; } set { this.associatedTankId = value; } }

		public string ArmsServiced { get { return this.armsServiced; } set {
		    this.SetString("Arms Serviced", 30, value, ref this.armsServiced); } }

		public bool InhibitSettingRecipeNames { get { return this._InhibitSettingRecipeNames; } set {
		    this._InhibitSettingRecipeNames = value; } }

		public bool TouchKeyReader { get { return this._TouchKeyReader; } set {
		    this._TouchKeyReader = value; } }

		public bool OffLoadByOffLoadID { get { return this._OffLoadByOffLoadID; } set {
		    this._OffLoadByOffLoadID = value; } }

		public bool UseManualMeterData { get { return this._UseManualMeterData; } set {
		    this._UseManualMeterData = value; } }

		public bool PromptForBOLNumber { get { return this._PromptForBOLNumber; } set {
		    this._PromptForBOLNumber = value; } }

		public int LastTransactionNumber { get { return this._LastTransactionNumber; } set {
		    this._LastTransactionNumber = value; } }

		public DateTimeOffset LastTransactionNumberDateTime { get { return this._LastTransactionNumberDateTime; } set {
		    this._LastTransactionNumberDateTime = value; } }

	    public bool QueryForTrailers { get { return this._QueryForTrailers; } set {
	        this._QueryForTrailers = value; } }

        public bool PromptForGravity { get { return this._PromptForGravity; } set {
            this._PromptForGravity = value; } }

        public bool PromptForTemperature { get { return this._PromptForTemperature; } set {
             this._PromptForTemperature = value; } }

		public bool EnableDynamicRecipes
		{
			get { return this._EnableDynamicRecipes; }
			set {  this._EnableDynamicRecipes = value; }
		}

		  AlarmAndEventDescriptorClass[] IAlarmAndEventDiscovery.AlarmAndEvents
		{
			get
			{
                AlarmAndEventDescriptorClass[] descriptors = {
                                                                AdditiveProfileUnavailableAlarmDescriptor,
                                                                BatchTotalDiscrepencyEventDescriptor,
                                                                BrokenBlendAlarmDescriptor,
                                                                CompartmentEmptyInquiryEventDescriptor,
                                                                CreateCertificateOfAnalysisFailedAlarmDescriptor,
                                                                DevicePowerFailureDescriptor,
                                                                DriverNotQualifiedEventDescriptor,
                                                                DriverNotTrainedEventDescriptor,
                                                                DriverNotLicensedEventDescriptor,
                                                                EquipmentNotAuthorizedEventDescriptor,
                                                                ExcessVehicleWeightAlarmDescriptor,
                                                                FailedCertificateOfAnalysisAlarmDescriptor,
                                                                ImproperAdditizationAlarmDescriptor,
                                                                InvalidCardNumberEventDescriptor,
                                                                InvalidCarrierEventDescriptor,
                                                                InvalidCustomerNumberEventDescriptor,
                                                                InvalidDriverIDEventDescriptor,
                                                                InvalidPinEventDescriptor,
                                                                InvalidStorageLocationEventDescriptor,
                                                                InvalidTractorOrTankerIDEventDescriptor,
                                                                InvalidTrailerIDEventDescriptor,
                                                                LineItemsNotServedDescriptor,
                                                                LoadingInquiryEventDescriptor,
                                                                MaximumIdleTimeAlarmDescriptor,
                                                                MaximumLoadTimeAlarmDescriptor,
                                                                NoProductAllocationAlarmDescriptor,
                                                                NoProductPIDXAuthorizationAlarmDescriptor,
                                                                OverrideNoProductPIDXAuthorizationEventDescriptor,
                                                                NoTankCertificationAlarmDescriptor,
                                                                ErrorReadingMeterDescriptor,
                                                                PidxDenialAlarmDescriptor,
                                                                OverridePIDXDenialEventDescriptor,
                                                                PIDXUnavailableAlarmDescriptor,
                                                                PreviouslyLoadedInquiryEventDescriptor,
                                                                ProductLockedOutDescriptor,
                                                                ProductUnavailableAlarmDescriptor,
                                                                SetAdditiveMeterTotalizerEventDescriptor,
                                                                ShutdownAlarmDescriptor,
                                                                TerminalAccessNotScheduledEventDescriptor,
                                                                LoadbyLoadidInhibitedDescriptor,
                                                                NoOrdersAvailableDescriptor,
                                                                MaxRetriesExceededKeyDescriptor,
                                                                NoCompartmentsToLoadDescriptor,
                                                                MismatchTractorOrTankerDescriptor,
                                                                MismatchTrailerDescriptor,
                                                                BeginDriverDownloadDescriptor,
                                                                BeginEquipmentDownloadDescriptor,
                                                                CardExpiredKeyEventDescriptor,
                                                                CompanyHierarchyInvalidDescriptor,
                                                                DriverDownloadCompleteDescriptor,
                                                                DriverDownloadInterruptedDescriptor,
                                                                DriverDownloadLimitExceededDescriptor,
                                                                EquipmentDownloadCompleteDescriptor,
                                                                EquipmentDownloadInterruptedDescriptor,
                                                                EquipmentDownloadLimitExceededDescriptor,
                                                                EquipmentCardLogInEventDescriptor,
                                                                //EquipmentGroupCardLogInEventDescriptor
                                                                ErrorSettingPermissiveDescriptor,
                                                                InvalidDriverIdentifierDescriptor,
                                                                InvalidEquipmentIdentifierDescriptor,
                                                                InvalidHouseCardNumberEventDescriptor,
                                                                NoProductsAvailableDescriptor,
                                                                OfflineTransactionUploadedKeyDescriptor,
                                                                OrderAliasInvalidDescriptor,
                                                                ScullyBypassUsedEventDescriptor,
                                                                ScullyTIMNotDetectedEventDescriptor,
                                                                ExternalComponentPercentageBadAlarmDescriptor,
                                                                StationErrorKeyDescriptor,
                                                                InvalidStateForOperationKeyDescriptor,
                                                                UnsupportedAdditiveKeyDescriptor,
                                                                UnsupportedComponentKeyDescriptor,
                                                                OfflineLoadIdFallbackKeyDescriptor,
                                                                OfflineOffloadIdFallbackKeyDescriptor,
                                                                OfflineLoadIdFallbackFailedKeyDescriptor,
                                                                OfflineOffloadIdFallbackFailedKeyDescriptor,
                                                                TrailerMissingScullyIDEventDescriptor,
                                                                FailedToSetDensityDescriptor
                                                            };
                return descriptors;
			}
		}


		public AlarmAndEventLogClass LineItemsNotServed
		{
			get
			{
			    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(LineItemsNotServedDescriptor)
			                                                 {
			                                                     AssociatedData = this.ID
			                                                 };
			    return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass DevicePowerFailureAlarm
		{
			get
			{
			    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DevicePowerFailureDescriptor)
			                                                 {
			                                                     AssociatedData = this.ID
			                                                 };
			    return alarmAndEventLog;
			}
		}

		public AlarmAndEventLogClass ShutdownAlarm(string transactionID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ShutdownAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + transactionID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass MaximumLoadTimeAlarm(string TransactionID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(MaximumLoadTimeAlarmDescriptor)
		                                             {
		                                                 AssociatedData = this.ID + " - "+ TransactionID
		                                             };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass MaximumIdleTimeAlarm(string transactionID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(MaximumIdleTimeAlarmDescriptor)
		                                             {
		                                                 AssociatedData = this.ID + " - " + transactionID
		                                             };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass ProductUnavailableAlarm(string product)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(ProductUnavailableAlarmDescriptor)
		                                             {
		                                                 AssociatedData = this.ID + " - " + product
		                                             };
		    return alarmAndEventLog;
		}

        public AlarmAndEventLogClass ProductUnavailableAlarm(string product, string driver)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(ProductUnavailableAlarmDescriptor)
            {
                AssociatedData = this.ID + " - " + product + " - " + driver
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass AdditiveProfileUnavailableAlarm(string additiveProfile)
		{
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(AdditiveProfileUnavailableAlarmDescriptor)
            { AssociatedData = this.ID + " - " + additiveProfile };
            return alarmAndEventLog;
		}

		public AlarmAndEventLogClass BrokenBlendAlarm(int ItemNumber, string Product)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(BrokenBlendAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - Item " + ItemNumber.ToString() + " - " + Product;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass ImproperAdditizationAlarm(int ItemNumber, string AdditiveProfile)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ImproperAdditizationAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - Item " + ItemNumber.ToString() + " - " + AdditiveProfile;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass ProductLockedOutAlarm(string Product)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ProductLockedOutDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + Product;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass NoTankCertificationAlarm(string Product)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(NoTankCertificationAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = Product;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass FailedCertificateOfAnalysisAlarm(string ShipTo, string Product)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(FailedCertificateOfAnalysisAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = ShipTo + " - " + Product;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass ExcessVehicleWeightAlarm(string Equipment, string Weight)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(ExcessVehicleWeightAlarmDescriptor);
			AlarmAndEventLog.AssociatedData = Equipment + " - " + Weight;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass CreateCertificateOfAnalysisFailedAlarm(string transID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(CreateCertificateOfAnalysisFailedAlarmDescriptor)
		        {
		            AssociatedData = "TransID - " + transID
		        };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass InvalidStorageLocationEvent(string transID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(InvalidStorageLocationEventDescriptor)
		                                             {
		                                                 AssociatedData = "TransID - " + transID
		                                             };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass NoProductAllocationAlarm(string product)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(NoProductAllocationAlarmDescriptor)
		                                             {
		                                                 AssociatedData = this.ID + " - " + product
		                                             };
		    return alarmAndEventLog;
		}

	    public AlarmAndEventLogClass NoProductPIDXAuthorizationAlarm(string product)
	    {
	        AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(NoProductPIDXAuthorizationAlarmDescriptor)
	                                                 {
	                                                     AssociatedData = this.ID + " - " + product
	                                                 };
	        return alarmAndEventLog;
	    }

	    /// <summary>
        /// Create an alarm recording an product not being authorized by PIDX.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="customerId">
        /// The customer Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass NoProductPIDXAuthorizationAlarm(string product, string driverId, string customerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(NoProductPIDXAuthorizationAlarmDescriptor)
            {
                AssociatedData =
                    this.ID + " - "
                    + product
                    + " - Driver " + driverId
                    + " - Customer " + customerId
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording an override of a PIDX non-authorization.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass OverrideNoProductPIDXAuthorizationEvent(string product)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OverrideNoProductPIDXAuthorizationEventDescriptor)
            {
                AssociatedData
                                               =
                                               this.ID
                                               + " - " + product
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording an override of a PIDX non-authorization.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="customerId">
        /// The customer Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass OverrideNoProductPIDXAuthorizationEvent(string product, string driverId, string customerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OverrideNoProductPIDXAuthorizationEventDescriptor)
            {
                AssociatedData = this.ID + " - " + product + " - Driver " + driverId + " - Customer " + customerId
            };
            return alarmAndEventLog;
		}

        public AlarmAndEventLogClass ErrorReadingMeterEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ErrorReadingMeterDescriptor)
            {
                AssociatedData = this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass PIDXUnavailableAlarm(string profileID)
		{
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PIDXUnavailableAlarmDescriptor)
                                                     {
                                                         AssociatedData = profileID
                                                     };
            return alarmAndEventLog;
		}

		public AlarmAndEventLogClass PIDXDenialAlarm(string profileID, string reason)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(PidxDenialAlarmDescriptor)
		                                             {
		                                                 AssociatedData = profileID + " - " + reason
		                                             };
		    return alarmAndEventLog;
		}

        /// <summary>
        /// Create an alarm recording a PIDX denial.
        /// </summary>
        /// <param name="profileId">
        /// The profile id.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="driverId">
        /// the driver id
        /// </param>
        /// <param name="customerId">
        /// The customer Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/>.
        /// </returns>
        public AlarmAndEventLogClass PIDXDenialAlarm(string profileId, string reason, string driverId, string customerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(PidxDenialAlarmDescriptor)
            {
                AssociatedData =
                   profileId
                   + " - " + reason
                   + " - Driver " + driverId
                   + " - Customer " + customerId
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording an override of a PIDX denial.
        /// </summary>
        /// <param name="profileId">
        /// The profile id.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass OverridePIDXDenialEvent(string profileId, string reason)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OverridePIDXDenialEventDescriptor)
            {
                AssociatedData =
                                               profileId
                                               + " - " + reason
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording an override of a PIDX denial.
        /// </summary>
        /// <param name="profileId">
        /// The profile id.
        /// </param>
        /// <param name="reason">
        /// The reason.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="customerId">
        /// The customer Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass OverridePIDXDenialEvent(string profileId, string reason, string driverId, string customerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OverridePIDXDenialEventDescriptor)
            {
                AssociatedData =
                    profileId
                    + " - " + reason
                    + " - Driver " + driverId
                    + " - Customer " + customerId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidCardNumberEvent(string CardNumber)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(InvalidCardNumberEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + CardNumber;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass InvalidDriverIDEvent(string DriverID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(InvalidDriverIDEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + DriverID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass InvalidTrailerIDEvent(string trailerID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(InvalidTrailerIDEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + trailerID
		                                                 };
		    return alarmAndEventLog;
		}

        /// <summary>
        /// Create an event recording an invalid entry for a trailer.
        /// </summary>
        /// <param name="trailerId">
        /// The trailer id.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="carrierId">
        /// The carrier Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass InvalidTrailerIDEvent(string trailerId, string driverId, string carrierId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(InvalidTrailerIDEventDescriptor)
            {
                AssociatedData = this.ID + " - " + trailerId + " - Driver " + driverId + " - Carrier " + carrierId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidTractorOfTankerIDEvent(string tractorOrTankerID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(InvalidTractorOrTankerIDEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + tractorOrTankerID
		                                                 };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass InvalidPinEvent(string driverID, string pin)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(InvalidPinEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + driverID + " - " + pin
		                                                 };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass InvalidCustomerNumberEvent(string loadID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(InvalidCustomerNumberEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + loadID
		                                                 };
		    return alarmAndEventLog;
		}

        public AlarmAndEventLogClass InvalidCustomerNumberEvent(string loadId, string driverId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(InvalidCustomerNumberEventDescriptor)
            {
                AssociatedData = this.ID + " - " + loadId + " - Driver " + driverId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidCarrierEvent(string DriverID)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(InvalidCarrierEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + DriverID;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass LoadingInquiryEvent(string DriverID, string PromptLoadRackText, string Response)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(LoadingInquiryEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + DriverID + " - " + PromptLoadRackText + " - " + Response;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass PreviouslyLoadedInquiryEvent(string DriverID, string PromptLoadRackText, string Response)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(PreviouslyLoadedInquiryEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + DriverID + " - " + PromptLoadRackText + " - " + Response;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass ComparmentEmptyInquiryEvent(string DriverID, string PromptLoadRackText, string Response)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(CompartmentEmptyInquiryEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - " + DriverID + " - " + PromptLoadRackText + " - " + Response;
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass BatchTotalDiscrepencyEvent(string DocumentNumber, string Batch, double Discrepency)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(BatchTotalDiscrepencyEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - Document " + DocumentNumber + " - Batch " + Batch + " - Discrepency " + Discrepency.ToString();
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass SetAdditiveMeterTotalizerEvent(int ArmNumber, string ProductID, double Value)
		{
			AlarmAndEventLogClass AlarmAndEventLog = new AlarmAndEventLogClass(SetAdditiveMeterTotalizerEventDescriptor);
			AlarmAndEventLog.AssociatedData = this.ID + " - Arm " + ArmNumber.ToString() + " - " + ProductID + " - " + Value.ToString();
			return AlarmAndEventLog;
		}

		public AlarmAndEventLogClass DriverNotQualifiedEvent(string DriverID, string QualificationID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DriverNotQualifiedEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + DriverID + " - " + QualificationID
		                                                 };
		    return alarmAndEventLog;
		}

		public AlarmAndEventLogClass DriverNotTrainedEvent(string driverID, string trainingID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DriverNotTrainedEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + driverID + " - " + trainingID
		                                                 };
		    return alarmAndEventLog;
		}

        public AlarmAndEventLogClass DriverNotLicensedEvent(string driverID, string licenseID)
        {
            AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(DriverNotLicensedEventDescriptor)
                                                        {
                                                            AssociatedData = this.ID + " - " + driverID + " - " + licenseID
                                                        };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentNotAuthorizedEvent(string equipmentID, string testOrInspectionID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(EquipmentNotAuthorizedEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + equipmentID + " - " + testOrInspectionID
		                                                 };
		    return alarmAndEventLog;
		}

        public AlarmAndEventLogClass TerminalAccessNotScheduledEvent(string driverID)
		{
		    AlarmAndEventLogClass alarmAndEventLog = new AlarmAndEventLogClass(TerminalAccessNotScheduledEventDescriptor)
		                                                 {
		                                                     AssociatedData = this.ID + " - " + driverID
		                                                 };
		    return alarmAndEventLog;
		}

        public AlarmAndEventLogClass LoadbyLoadidInhibitedEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(LoadbyLoadidInhibitedDescriptor)
            {
                AssociatedData = this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass NoOrdersAvailableEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(NoOrdersAvailableDescriptor)
            {
                AssociatedData = this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass MaxRetriesExceededKeyEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MaxRetriesExceededKeyDescriptor)
            {
                AssociatedData = this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass NoCompartmentsToLoadEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(NoCompartmentsToLoadDescriptor)
            {
                AssociatedData = this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording a carrier-to-equipment mismatch on a tractor or tanker.
        /// </summary>
        /// <param name="tractorOrTankerId">
        /// The tractor or tanker id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass MismatchTractorOrTankerEvent(string tractorOrTankerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MismatchTractorOrTankerDescriptor)
            {
                AssociatedData = this.ID + " - " + tractorOrTankerId
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording a carrier-to-equipment mismatch on a tractor or tanker.
        /// </summary>
        /// <param name="tractorOrTankerId">
        /// The tractor or tanker id.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="carrierId">
        /// The carrier Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/> containing the event data.
        /// </returns>
        public AlarmAndEventLogClass MismatchTractorOrTankerEvent(string tractorOrTankerId, string driverId, string carrierId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MismatchTractorOrTankerDescriptor)
            {
                AssociatedData = this.ID + " - " + tractorOrTankerId + " - Driver " + driverId + " - Carrier " + carrierId
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording a carrier-to-equipment mismatch on a trailor.
        /// </summary>
        /// <param name="trailerId">
        /// The trailer id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/>.
        /// </returns>
        public AlarmAndEventLogClass MismatchTrailerEvent(string trailerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MismatchTrailerDescriptor)
            {
                AssociatedData = this.ID + " - " + trailerId
            };
            return alarmAndEventLog;
        }

        /// <summary>
        /// Create an event recording a carrier-to-equipment mismatch on a trailor.
        /// </summary>
        /// <param name="trailerId">
        /// The trailer id.
        /// </param>
        /// <param name="driverId">
        /// The driver Id.
        /// </param>
        /// <param name="carrierId">
        /// The carrier Id.
        /// </param>
        /// <returns>
        /// The <see cref="AlarmAndEventLogClass"/>.
        /// </returns>
        public AlarmAndEventLogClass MismatchTrailerEvent(string trailerId, string driverId, string carrierId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(MismatchTrailerDescriptor)
            {
                AssociatedData = this.ID + " - " + trailerId + " Driver - " + driverId + " Carrier - " + carrierId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass BeginDriverDownloadEvent(string type)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(BeginDriverDownloadDescriptor)
            {
                AssociatedData = this.ID + " - " + type
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass BeginEquipmentDownloadEvent(string type)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(BeginEquipmentDownloadDescriptor)
            {
                AssociatedData = this.ID + " - " + type
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CardExpiredKeyEvent(string expirationDate)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CardExpiredKeyEventDescriptor)
            {
                AssociatedData =
                                               this.ID + " - "
                                               + expirationDate
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CompanyHierarchyInvalidEvent(string stationId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CompanyHierarchyInvalidDescriptor)
            {
                AssociatedData =
                    this.ID + " - "
                    + stationId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass CompanyHierarchyInvalidEvent(string driverId, string customerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(CompanyHierarchyInvalidDescriptor)
            {
                AssociatedData =
                    this.ID + " - Driver "
                    + driverId + " - Customer "
                    + customerId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass DriverDownloadCompleteEvent()
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(DriverDownloadCompleteDescriptor)
            {
                AssociatedData = this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass DriverDownloadInterruptedEvent()
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(DriverDownloadInterruptedDescriptor)
            {
                AssociatedData = this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass DriverDownloadLimitExceededEvent()
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(DriverDownloadLimitExceededDescriptor)
            {
                AssociatedData = this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentDownloadCompleteEvent()
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentDownloadCompleteDescriptor)
            {
                AssociatedData = this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentDownloadInterruptedEvent()
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentDownloadInterruptedDescriptor)
            {
                AssociatedData = this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentDownloadLimitExceededEvent()
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentDownloadLimitExceededDescriptor)
            {
                AssociatedData = this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass EquipmentCardLogInEvent(EquipmentClass trailer)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentCardLogInEventDescriptor)
            {
                AssociatedData = this.ID + " - " + trailer.ID + " - " + trailer.TruckCardNumber
            };
            return alarmAndEventLog;
        }
        /* EquipmentGroupClass is only used for Newcastle Stolthaven, and not exist in 9.x yet
        public AlarmAndEventLogClass EquipmentGroupCardLogInEvent(EquipmentGroupClass equipmentGroup)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(EquipmentGroupCardLogInEventDescriptor)
            {
                AssociatedData = this.ID + " - " + equipmentGroup.ID + " - " + equipmentGroup.CardNumber
            };
            return alarmAndEventLog;
        }
        */

        public AlarmAndEventLogClass ErrorSettingPermissiveEvent(string stationID, string opcPoint)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ErrorSettingPermissiveDescriptor)
            {
                AssociatedData =
                                               this.ID + " - " + opcPoint
                                               + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidDriverIdentifier(string id)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(InvalidDriverIdentifierDescriptor)
            {
                AssociatedData = this.ID + " - " + id
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidEquipmentIdentifier(string id)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(InvalidEquipmentIdentifierDescriptor)
            {
                AssociatedData = this.ID + " - " + id
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidHouseCardNumberEvent(string cardNumber)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(InvalidHouseCardNumberEventDescriptor)
            {
                AssociatedData =
                                               this.ID + " - "
                                               + cardNumber
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass NoProductsAvailableEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(NoProductsAvailableDescriptor)
            {
                AssociatedData =
                                               this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OfflineTransactionUploadedEvent(string bolNumber)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OfflineTransactionUploadedKeyDescriptor)
            {
                AssociatedData = bolNumber + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OrderAliasInvalidEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OrderAliasInvalidDescriptor)
            {
                AssociatedData =
                                               this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass ScullyBypassUsedEvent(string driverId, string carrierId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ScullyBypassUsedEventDescriptor)
            {
                AssociatedData =
                    this.ID + " - Driver - " + driverId
                    + " - Carrier - " + carrierId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass ScullyTIMNotDetectedEvent(string driverId, string carrierId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ScullyTIMNotDetectedEventDescriptor)
            {
                AssociatedData =
                     this.ID + " - Driver - " + driverId
                    + " - Carrier - " + carrierId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass ScullyTIMNotMatchTruckCardNumberEvent(string driverId, string carrierId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ScullyTIMNotMatchTruckCardNumberEventDescriptor)
            {
                AssociatedData =
                     this.ID + " - Driver - " + driverId
                    + " - Carrier - " + carrierId
            };
            return alarmAndEventLog;
        }
        public AlarmAndEventLogClass ExternalComponentPercentageBadAlarm(string armId, string productId, string tag)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(ExternalComponentPercentageBadAlarmDescriptor)
            {
                AssociatedData = this.ID
                                 + " - Unable to read blend percentage for external component "
                                 + productId
                                 + " from tag ["
                                 + tag
                                 + "] on arm "
                                 + armId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass StationErrorAlarm(string errorMessage)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(StationErrorKeyDescriptor)
            {
                AssociatedData = this.ID + "Reported Error - " + errorMessage
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass InvalidStateForOperationAlarm(string state, string operation)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(InvalidStateForOperationKeyDescriptor)
            {
                AssociatedData = this.ID + " - " + "Attempted to perform operation *" + operation + "* while in state *" + state + "*"
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass UnsupportedAdditiveAlarm(int arm, int productNumber, string productId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(UnsupportedAdditiveKeyDescriptor)
            {
                AssociatedData = this.ID
                                 + " - Unsupported additive "
                                 + productNumber.ToString(CultureInfo.InvariantCulture)
                                 + " ("
                                 + productId
                                 + ") on arm "
                                 + arm.ToString(CultureInfo.InvariantCulture)
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass UnsupportedComponentAlarm(int arm, int productNumber, string productId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(UnsupportedComponentKeyDescriptor)
            {
                AssociatedData = this.ID
                                 + " - Unsupported component "
                                 + productNumber.ToString(CultureInfo.InvariantCulture)
                                 + " ("
                                 + productId
                                 + ") on arm "
                                 + arm.ToString(CultureInfo.InvariantCulture)
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OfflineLoadIdFallbackEvent(string unrecognizedLoadId, string externalTransactionNumber)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OfflineLoadIdFallbackKeyDescriptor)
            {
                AssociatedData = unrecognizedLoadId + " fell back to default load id 999999 on remote transaction " + externalTransactionNumber + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OfflineOffloadIdFallbackEvent(string unrecognizedLoadId, string externalTransactionNumber)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OfflineOffloadIdFallbackKeyDescriptor)
            {
                AssociatedData = unrecognizedLoadId + " fell back to default offload id 999999 on remote transaction " + externalTransactionNumber + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OfflineLoadIdFallbackFailureAlarm(string unrecognizedLoadId, string externalTransactionNumber)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OfflineLoadIdFallbackFailedKeyDescriptor)
            {
                AssociatedData = "Failed to import offline load with load id " + unrecognizedLoadId + " on remote transaction " + externalTransactionNumber + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass OfflineOffloadIdFallbackFailureAlarm(string unrecognizedLoadId, string externalTransactionNumber)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(OfflineOffloadIdFallbackFailedKeyDescriptor)
            {
                AssociatedData = "Failed to import offline offload with offload id " + unrecognizedLoadId + " on remote transaction " + externalTransactionNumber + " - " + this.ID
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass TrailerMissingScullyIDEvent(string driverId, string trailerId)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(TrailerMissingScullyIDEventDescriptor)
            {
                AssociatedData =
                    this.ID + " - Driver - " + driverId
                    + " - Trailer - " + trailerId
            };
            return alarmAndEventLog;
        }

        public AlarmAndEventLogClass FailedToSetDensityEvent(string stationID)
        {
            var alarmAndEventLog = new AlarmAndEventLogClass(FailedToSetDensityDescriptor)
            {
                AssociatedData =
                                               this.ID + " - " + stationID
            };
            return alarmAndEventLog;
        }
			public AlarmAndEventLogClass DynamicRecipeDownloadErrorAlarm(string stationID)
			{
				var alarmAndEventLog = new AlarmAndEventLogClass(DynamicRecipeDownloadErrorDescriptor)
				{
					AssociatedData = this.ID + " - " + stationID
				};
				return alarmAndEventLog;
			}

		public StationClass()
		{
		    this.Reset();
		}

		public static string TypeID(STATION_TYPE type)
		{
			switch (type)
			{
				case STATION_TYPE.ENTRY_GATE:
					return "Entry Gate";
				case STATION_TYPE.EXIT_GATE:
					return "Exit Gate";
				case STATION_TYPE.LOAD_RACK:
					return "Load Rack";
				case STATION_TYPE.WEIGHT_SCALE:
					return "Weight Scale";
				case STATION_TYPE.BOL:
					return "Bill Of Lading";
				case STATION_TYPE.PRELOAD:
					return "Preload";
				case STATION_TYPE.SIGNATURE:
					return "Signature Station";
				case STATION_TYPE.METER:
					return "Meter Station";
				case STATION_TYPE.OFF_LOADING:
					return "Off-loading Station";
				default:
					return "Undefined";
			}
		}

		public static string InterfaceTypeID(STATION_INTERFACE_TYPE Type)
		{
         switch (Type)
         {
            case STATION_INTERFACE_TYPE.ACCULOADIII_Q:
               return "Accuload III-Q/IV";
            case STATION_INTERFACE_TYPE.PROXIMITY_CARD_READER:
               return "Proximity Card Reader";
            case STATION_INTERFACE_TYPE.PASS_CONTROLLER:
               return "PASS Controller";
            case STATION_INTERFACE_TYPE.VAREC_DET:
               return "Varec DET";
            case STATION_INTERFACE_TYPE.MANUAL:
               return "Manual";
            case STATION_INTERFACE_TYPE.MICROLOAD_NET:
               return "Microload.net";
            case STATION_INTERFACE_TYPE.DANLOAD6000:
               return "Danload 6000";
            case STATION_INTERFACE_TYPE.MULTILOAD_II_SMP:
               return "Multiload II SMP";
            case STATION_INTERFACE_TYPE.SIGNATURE:
               return "Signature";
            case STATION_INTERFACE_TYPE.METER:
               return "Meter";
            case STATION_INTERFACE_TYPE.ACCULOADIII_SA:
               return "Accuload III-SA";
            case STATION_INTERFACE_TYPE.CONTREC1010:
               return "Contrec 1010";
            case STATION_INTERFACE_TYPE.MULTILOAD_II:
               return "Multiload II";
            case STATION_INTERFACE_TYPE.CONTREC1010_RA:
               return "Contrec 1010RA";
            case STATION_INTERFACE_TYPE.RCU_II_OPEN:
               return "RCU II Open Protocol";
            case STATION_INTERFACE_TYPE.RCU_II_RCU:
               return "RCU II Rcu Protocol";
            case STATION_INTERFACE_TYPE.HID_CARD_READER:
               return "HID Card Reader";
				case STATION_INTERFACE_TYPE.REVUELTARADMTX:
					return "MTX HID Card Reader";
				case STATION_INTERFACE_TYPE.OSDP_CARD_READER:
					return "OSDP Card Reader";
				default:
               return "Undefined";
         }
      }

      string[] IDataDictionary.Keys(SecurityClass security)
        {
            string[] keys =
            {
                MaximumLoadTimeKey,
                MaximumIdleTimeKey,
                ProductUnavailableKey,
                AdditiveProfileUnavailableKey,
                BrokenBlendKey,
                ImproperAdditizationKey,
                DevicePowerFailureKey,
                LineItemsNotServedKey,
                ProductLockedOutKey,
                NoTankCertificationKey,
                FailedCertificateOfAnalysisKey,
                ExcessVehicleWeightKey,
                CreateCertificateOfAnalysisFailedKey,
                InvalidStorageLocationKey,
                NoProductPIDXAuthorizationKey,
                OverrideNoProductPIDXAuthorizationKey,
                //PIDXDenialKey,
                OverridePidxDenialKey,
                PIDXUnavailableKey,
                OfflineTransactionUploadedKey,
                OfflineLoadIdFallbackKey,
                OfflineLoadIdFallbackFailedKey,
                OfflineOffloadIdFallbackKey,
                OfflineOffloadIdFallbackFailedKey,
                InvalidStateForOperationKey,
                BeginEquipmentDownloadKey,
                EquipmentDownloadInterruptedKey,
                EquipmentDownloadCompleteKey,
                EquipmentDownloadLimitExceededKey,
                BeginDriverDownloadKey,
                DriverDownloadInterruptedKey,
                DriverDownloadCompleteKey,
                DriverDownloadLimitExceededKey,
                UnsupportedComponentKey,
                UnsupportedAdditiveKey,
                StationErrorKey,
                EquipmentCardLogInKey,
                //EquipmentGroupCardLogInKey,
                ExternalComponentPercentageBadKey
            };
            return keys;
        }

        public override void Reset()
		{
			base.Reset();
		    this._Type = STATION_TYPE.ENTRY_GATE;
		    this._SwingArmPosition = true;
		    this.ProcessVariableCollection = null;
		    this._InterfaceType = STATION_INTERFACE_TYPE.MAX_TYPE;
		    this._Enabled = true;
		    this._CardReader = true;
		    this._ThirtyFiveBitCardSupport = false;
		    this._BOLPrinter = "";
		    this._PreloadPrinter = "";
		    this._NumberOfCopies = MinNumberOfCopies;
		    this._NumberOfPreloadCopies = MinNumberOfCopies;
		    this._BOLAgeInMinutes = 60;
		    this._IssueByVolumeTransactionAliasGuid = Guid.Empty;
		    this._IssueByWeightTransactionAliasGuid = Guid.Empty;
		    this._ReceiptByVolumeTransactionAliasGuid = Guid.Empty;
		    this._ReceiptByWeightTransactionAliasGuid = Guid.Empty;
		    this._InhibitLoadingByLoadID = false;
		    this._InhibitOperatingModePrompt = false;
		    this._SynchronizeReferenceDensity = true;
		    this._SignatureDevice = "";
		    this._SetDefaultPresetToZero = false;
		    this._SignatureDevicePort = 1;
		    this._SignatureDeviceBaudRate = 115200;
		    this._MeterRecircCardNumber = "";
		    this._RecircTransactionAliasGuid = Guid.Empty;
            this._PromptForGravity = false;
            this._PromptForTemperature = false;
            this.LogCommunications = false;
		    this.LogCommPath = string.Empty;
            this._EnableScully = false;
            this._EnableEquipmentValidate = false;
            this.StationMessageTimeout = 2;
		    this.StationPromptTimeout = 60;
			this._EnableDynamicRecipes = false;
			this.EthanolExcess = false;

		    this.LoadArmCollection = null;
			PROCESS_VARIABLE_TYPE[] PVType = {PROCESS_VARIABLE_TYPE.STATION_PV,
														 PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV,
														  PROCESS_VARIABLE_TYPE.UNDEFINED_PV};

		    this.ProcessVariableCollection = new ProcessVariableCollectionClass();

			int iPV = 0;
			while (PVType[iPV] != PROCESS_VARIABLE_TYPE.UNDEFINED_PV)
			{
				ProcessVariableClass ProcessVariable = new ProcessVariableClass();
				ProcessVariable.UnitType = UNIT_TYPE.STATION_UNIT;
				ProcessVariable.ProcessVariableType = PVType[iPV];

				// Determine the DataType
				if (PVType[iPV] == PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
					ProcessVariable.DataType = VarEnum.VT_BOOL;
				else
					ProcessVariable.DataType = VarEnum.VT_EMPTY;

				// Determine I/O
				ProcessVariable.DataTypeEnabled = false;
				if (PVType[iPV] == PROCESS_VARIABLE_TYPE.GATE_CONTROL_PV)
					ProcessVariable.Input = false;
				else
					ProcessVariable.Input = true;

				ProcessVariable.InputEnabled = false;
			    this.ProcessVariableCollection.Add(ProcessVariable);
				iPV++;
			}

		    this.LoadArmCollection = new LoadArmCollectionClass();

		    this.StationPermissives = new PermissivesClass();
		    this.StationPermissives.InputUnitType = UNIT_TYPE.STATION_INPUT_PERMISSIVE;
		    this.StationPermissives.OutputUnitType = UNIT_TYPE.STATION_OUTPUT_PERMISSIVE;

		    this.IssueByVolumeTransactionAliasID = "";
		    this.IssueByWeightTransactionAliasID = "";
		    this.ReceiptByVolumeTransactionAliasID = "";
		    this.ReceiptByWeightTransactionAliasID = "";
		    this.RecircTransactionAliasID = "";

		    this.associatedTankGuid = Guid.Empty;
		    this.associatedTankId = "{None}";
		    this.armsServiced = "";
		    this._InhibitSettingRecipeNames = true;
		    this._TouchKeyReader = false;
		    this._OffLoadByOffLoadID = true;
		    this._UseManualMeterData = false;
		    this._PromptForBOLNumber = false;
            this._QueryForTrailers = false;
            this._LastTransactionNumber = 0;
		    this._LastTransactionNumberDateTime = DateTimeOffset.Now;
		    this.ReqQualificationsCollection = new QualificationMapCollectionClass();
		    this.ReqTrainingCollection = new QualificationMapCollectionClass();
            this.ReqLicenseCollection = new QualificationMapCollectionClass();
            this.ReqTestsandInspectionsCollection = new QualificationMapCollectionClass();
            this.ReqEquipmentTagAndLicenseCollection = new QualificationMapCollectionClass();
            this.Meter = new MeterClass();
		}

		/// <summary>
		/// This method will load the station object with data from the 
		/// database.
		/// </summary>
		/// <param name="o"></param>
		public override void Load(Object o)
		{
		    this.Reset();

			if (typeof(DataSet).IsInstanceOfType(o))
			{
				DataSet Set = (DataSet)o;
				DataTable Table = Set.Tables[0];

				if (Table.Rows.Count == 0)
					return;

				DataRow Row = Table.Rows[0];

			    this._IdentityGuid = DataObject.getValue<Guid>(Row["StationGuid"], Guid.Empty);
			    this._SiteGuid = DataObject.getValue<Guid>(Row["SiteGuid"], Guid.Empty);
			    this._ID = DataObject.getValue<string>(Row["ID"], "");
			    this._Type = DataObject.getValue<STATION_TYPE>(Row["LookupStationTypeIndex"], STATION_TYPE.ENTRY_GATE);
			    this._SwingArmPosition = DataObject.getValue<bool>(Row["SwingArmPosition"], true);
			    this._VaporRecovery = DataObject.getValue<bool>(Row["VaporRecovery"], false);
			    this._InterfaceType = DataObject.getValue<STATION_INTERFACE_TYPE>(Row["LookupStationInterfaceTypeIndex"], STATION_INTERFACE_TYPE.MAX_TYPE);
			    this._Enabled = DataObject.getValue<bool>(Row["Enabled"], true);
			    this._BOLPrinter = DataObject.getValue<string>(Row["BOLPrinter"], "");
			    this._PreloadPrinter = DataObject.getValue<string>(Row["PreloadPrinter"], "");
			    this._BOLAgeInMinutes = DataObject.getValue<int>(Row["BOLAgeInMinutes"], 60);
			    this._IssueByVolumeTransactionAliasGuid = DataObject.getValue<Guid>(Row["IssueByVolumeTransactionAliasGuid"], Guid.Empty);
			    this._IssueByWeightTransactionAliasGuid = DataObject.getValue<Guid>(Row["IssueByWeightTransactionAliasGuid"], Guid.Empty);
			    this._ReceiptByVolumeTransactionAliasGuid = DataObject.getValue<Guid>(Row["ReceiptByVolumeTransactionAliasGuid"], Guid.Empty);
			    this._ReceiptByWeightTransactionAliasGuid = DataObject.getValue<Guid>(Row["ReceiptByWeightTransactionAliasGuid"], Guid.Empty);
			    this._CardReader = DataObject.getValue<bool>(Row["CardReader"], true);
			    this._ThirtyFiveBitCardSupport = DataObject.getValue<bool>(Row["ThirtyFiveBitCardSupport"], false);
			    this.CreatedDate = DataObject.getValue<DateTimeOffset>(Row["CreatedDate"], DateTimeOffset.Now);
			    this.CreatedBy = DataObject.getValue<string>(Row["CreatedBy"], ADMIN);
			    this.UpdatedDate = DataObject.getValue<DateTimeOffset>(Row["UpdatedDate"], this.CreatedDate);
			    this.UpdatedBy = DataObject.getValue<string>(Row["UpdatedBy"], ADMIN);
			    this._NumberOfCopies = DataObject.getValue<int>(Row["NumberOfCopies"], MinNumberOfCopies);
			    this._NumberOfPreloadCopies = DataObject.getValue<int>(Row["NumberOfPreloadCopies"], MinNumberOfCopies);
			    this._InhibitLoadingByLoadID = DataObject.getValue<bool>(Row["InhibitLoadingByLoadID"], false);
			    this._InhibitOperatingModePrompt = DataObject.getValue<bool>(Row["InhibitOperatingModePrompt"], false);
			    this._SynchronizeReferenceDensity = DataObject.getValue<bool>(Row["SynchronizeReferenceDensity"], true);
			    this._SignatureDevice = DataObject.getValue<string>(Row["SignatureDevice"], "");
			    this._SetDefaultPresetToZero = DataObject.getValue<bool>(Row["SetDefaultPresetToZero"], false);
			    this.IssueByVolumeTransactionAliasID = DataObject.getValue<string>(Row["IssueByVolumeTransactionAliasID"], "{None}");
			    this.IssueByWeightTransactionAliasID = DataObject.getValue<string>(Row["IssueByWeightTransactionAliasID"], "{None}");
			    this.ReceiptByVolumeTransactionAliasID = DataObject.getValue<string>(Row["ReceiptByVolumeTransactionAliasID"], "{None}");
			    this.ReceiptByWeightTransactionAliasID = DataObject.getValue<string>(Row["ReceiptByWeightTransactionAliasID"], "{None}");
			    this.RecircTransactionAliasID = DataObject.getValue<string>(Row["RecircTransactionAliasID"], "{None}");
			    this.associatedTankGuid = DataObject.getValue<Guid>(Row["TankGuid"], Guid.Empty);
			    this.associatedTankId = DataObject.getValue<string>(Row["AssociatedTankID"], "{None}");
			    this.armsServiced = DataObject.getValue<string>(Row["ArmsServiced"], "");
			    this._InhibitSettingRecipeNames = DataObject.getValue<bool>(Row["InhibitSettingRecipeNames"], false);
			    this._SignatureDevicePort = DataObject.getValue<int>(Row["SignatureDevicePort"], 1);
			    this._SignatureDeviceBaudRate = DataObject.getValue<int>(Row["SignatureDeviceBaudRate"], 115200);
			    this._MeterRecircCardNumber = DataObject.getValue<string>(Row["MeterRecircCardNumber"], "");
			    this._RecircTransactionAliasGuid = DataObject.getValue<Guid>(Row["RecircTransactionAliasGuid"], Guid.Empty);
			    this._TouchKeyReader = DataObject.getValue<bool>(Row["TouchKeyReader"], false);
			    this._OffLoadByOffLoadID = DataObject.getValue<bool>(Row["OffLoadByOffLoadID"], true);
			    this._UseManualMeterData = DataObject.getValue<bool>(Row["UseManualMeterData"], false);
			    this._PromptForBOLNumber = DataObject.getValue<bool>(Row["PromptForBOLNumber"], false);
                this._QueryForTrailers = (!Row.IsNull("QueryForTrailers")) && (bool)Row["QueryForTrailers"];
                this._PromptForGravity = (!Row.IsNull("PromptForGravityCaptured")) && (bool)Row["PromptForGravityCaptured"];
                this._PromptForTemperature = (!Row.IsNull("PromptForTemperatureCaptured")) && (bool)Row["PromptForTemperatureCaptured"];
                this._LastTransactionNumber = DataObject.getValue<int>(Row["LastTransactionNumber"], 0);
			    this._LastTransactionNumberDateTime = DataObject.getValue<DateTimeOffset>(Row["LastTransactionNumberDateTime"], DateTimeOffset.Now);
				this.LogCommunications = DataObject.getValue<bool>(Row["LogCommunications"], false);
				this.LogCommPath = DataObject.getValue<string>(Row["LogCommPath"], string.Empty);
                this._EnableScully = DataObject.getValue<bool>(Row["EnableScully"], false);
                this._EnableEquipmentValidate = DataObject.getValue<bool>(Row["EnableEquipmentValidate"], false); 
				this.StationMessageTimeout = DataObject.getValue<int>(Row["StationMessageTimeout"], 2);
				this.StationPromptTimeout = DataObject.getValue<int>(Row["StationPromptTimeout"], 60);
			    this.Meter.IdentityGuid = DataObject.getValue<Guid>(Row["AssignedMeterGuid"], Guid.Empty);
            this._EnableDynamicRecipes = DataObject.getValue<bool>(Row["EnableDynamicRecipes"], false);
            this.EthanolExcess = DataObject.getValue<bool>(Row["EthanolExcess"], false);

            this.RowVersion = DataObject.getValue<Byte[]>(Row["_RowVersion"], null);

			}
			else if (typeof(StationClass).IsInstanceOfType(o))
			{
				StationClass Station = (StationClass)o;

			    this._IdentityGuid = Station._IdentityGuid;
			    this._SiteGuid = Station._SiteGuid;
			    this._ID = Station.ID;
			    this._Type = Station.Type;
			    this.SwingArmPosition = Station.SwingArmPosition;
			    this._VaporRecovery = Station.VaporRecovery;
			    this._InterfaceType = Station.InterfaceType;
			    this._Enabled = Station.Enabled;
			    this._BOLPrinter = Station.BOLPrinter;
			    this._PreloadPrinter = Station.PreloadPrinter;
			    this._BOLAgeInMinutes = Station.BOLAgeInMinutes;
			    this._IssueByVolumeTransactionAliasGuid = Station.IssueByVolumeTransactionAliasGuid;
			    this._IssueByWeightTransactionAliasGuid = Station.IssueByWeightTransactionAliasGuid;
			    this._ReceiptByVolumeTransactionAliasGuid = Station.ReceiptByVolumeTransactionAliasGuid;
			    this._ReceiptByWeightTransactionAliasGuid = Station.ReceiptByWeightTransactionAliasGuid;
			    this._CreatedDate = Station.CreatedDate;
			    this._CreatedBy = Station.CreatedBy;
			    this._UpdatedDate = Station.UpdatedDate;
			    this._UpdatedBy = Station.UpdatedBy;
			    this.IssueByVolumeTransactionAliasID = Station.IssueByVolumeTransactionAliasID;
			    this.IssueByWeightTransactionAliasID = Station.IssueByWeightTransactionAliasID;
			    this.ReceiptByVolumeTransactionAliasID = Station.ReceiptByVolumeTransactionAliasID;
			    this.ReceiptByWeightTransactionAliasID = Station.ReceiptByWeightTransactionAliasID;
			    this.RecircTransactionAliasID = Station.RecircTransactionAliasID;
			    this._CardReader = Station.CardReader;
			    this._ThirtyFiveBitCardSupport = Station.ThirtyFiveBitCardSupport;
			    this._NumberOfCopies = Station.NumberOfCopies;
			    this._NumberOfPreloadCopies = Station.NumberOfPreloadCopies;
			    this._InhibitLoadingByLoadID = Station.InhibitLoadingByLoadID;
			    this._InhibitOperatingModePrompt = Station.InhibitOperatingModePrompt;
			    this._SynchronizeReferenceDensity = Station.SynchronizeReferenceDensity;
			    this._SignatureDevice = Station.SignatureDevice;
			    this._SetDefaultPresetToZero = Station.SetDefaultPresetToZero;
			    this.associatedTankGuid = Station.AssociatedTankGuid;
			    this.associatedTankId = Station.AssociatedTankId;
			    this.armsServiced = Station.ArmsServiced;
			    this._InhibitSettingRecipeNames = Station.InhibitSettingRecipeNames;
			    this._SignatureDevicePort = Station.SignatureDevicePort;
			    this._SignatureDeviceBaudRate = Station.SignatureDeviceBaudRate;
			    this._MeterRecircCardNumber = Station.MeterRecircCardNumber;
			    this._RecircTransactionAliasGuid = Station.RecircTransactionAliasGuid;
			    this._TouchKeyReader = Station.TouchKeyReader;
			    this._OffLoadByOffLoadID = Station.OffLoadByOffLoadID;
			    this._UseManualMeterData = Station.UseManualMeterData;
			    this._PromptForBOLNumber = Station.PromptForBOLNumber;
                this._QueryForTrailers = Station.QueryForTrailers;
                this._PromptForGravity = Station.PromptForGravity;
                this._PromptForTemperature = Station.PromptForTemperature;
                this._LastTransactionNumber = Station.LastTransactionNumber;
			    this._LastTransactionNumberDateTime = Station.LastTransactionNumberDateTime;
			    this.LogCommunications = Station.LogCommunications;
			    this.LogCommPath = Station.LogCommPath;
                this._EnableScully = Station.EnableScully;
                this._EnableEquipmentValidate = Station.EnableEquipmentValidate;
                this.StationMessageTimeout = Station.StationMessageTimeout;
			    this.StationPromptTimeout = Station.StationPromptTimeout;
                this.Meter.IdentityGuid = Station.Meter.IdentityGuid;
            this._EnableDynamicRecipes = Station.EnableDynamicRecipes;
            this.EthanolExcess = Station.EthanolExcess;

            this.ProcessVariableCollection.Clear();
				foreach (ProcessVariableClass PV in Station.ProcessVariableCollection)
				{
					ProcessVariableClass NewPV = new ProcessVariableClass();
					NewPV.Load(PV);
				    this.ProcessVariableCollection.Add(PV);
				}

				foreach (LoadArmClass LoadArm in Station.LoadArmCollection)
				{
					LoadArmClass NewLoadArm = new LoadArmClass();
					NewLoadArm.Load(LoadArm);
				    this.LoadArmCollection.Add(LoadArm);
				}
			}
		}

		/// <summary>
		/// New variant of InsertSql which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// </summary>
		public void InsertSql(SqlCommand command)
		{
			command.CommandText = "INSERT INTO tblStations " +
									"(SiteGuid," +
									"ID," +
									"LookupStationTypeIndex," +
									"SwingArmPosition," +
									"VaporRecovery," +
									"LookupStationInterfaceTypeIndex," +
									"Enabled," +
									"BOLPrinter," +
									"PreloadPrinter," +
									"BOLAgeInMinutes," +
									"IssueByVolumeTransactionAliasGuid," +
									"IssueByWeightTransactionAliasGuid," +
									"ReceiptByVolumeTransactionAliasGuid," +
									"ReceiptByWeightTransactionAliasGuid," +
									"CardReader," +
									"ThirtyFiveBitCardSupport," +
									"CreatedDate," +
									"CreatedBy," +
									"UpdatedDate," +
									"UpdatedBy, " +
									"NumberOfCopies," +
									"NumberOfPreloadCopies," +
									"InhibitLoadingByLoadID," +
									"InhibitOperatingModePrompt," +
									"SynchronizeReferenceDensity," +
									"SetDefaultPresetToZero," +
									"SignatureDevice," +
									"TankGuid," +
									"ArmsServiced," +
									"InhibitSettingRecipeNames," +
							"SignatureDevicePort," +
							"SignatureDeviceBaudRate," +
									"MeterRecircCardNumber," +
									"RecircTransactionAliasGuid," +
									"TouchKeyReader," +
									"OffLoadByOffLoadID," +
									"UseManualMeterData," +
									"PromptForBOLNumber," +
                                    "QueryForTrailers," +
                                    "PromptForGravityCaptured, " +
                                    "PromptForTemperatureCaptured, " +
                                    "LastTransactionNumber," +
									"LastTransactionNumberDateTime, " +
                                    "LogCommunications, " +
                                    "LogCommPath, " +
                                    "EnableScully, " +
                                    "EnableEquipmentValidate, " +                                    
                                     "StationPromptTimeout, "+
                                     "StationMessageTimeout, "+
                                     "AssignedMeterGuid, " +
                                     "EnableDynamicRecipes, " +
                                     "EthanolExcess " +
							") VALUES (" +
									"@SiteGuid," +
									"@ID," +
									"@LookupStationTypeIndex," +
									"@SwingArmPosition," +
									"@VaporRecovery," +
									"@LookupStationInterfaceTypeIndex," +
									"@Enabled," +
									"@BOLPrinter," +
									"@PreloadPrinter," +
									"@BOLAgeInMinutes," +
									"@IssueByVolumeTransactionAliasGuid," +
									"@IssueByWeightTransactionAliasGuid," +
									"@ReceiptByVolumeTransactionAliasGuid," +
									"@ReceiptByWeightTransactionAliasGuid," +
									"@CardReader," +
									"@ThirtyFiveBitCardSupport," +
									"@CreatedDate," +
									"@CreatedBy," +
									"@UpdatedDate," +
									"@UpdatedBy, " +
									"@NumberOfCopies," +
									"@NumberOfPreloadCopies," +
									"@InhibitLoadingByLoadID," +
									"@InhibitOperatingModePrompt," +
									"@SynchronizeReferenceDensity," +
									"@SetDefaultPresetToZero," +
									"@SignatureDevice," +
									"@TankGuid," +
									"@ArmsServiced," +
									"@InhibitSettingRecipeNames," +
									"@SignatureDevicePort," +
									"@SignatureDeviceBaudRate," +
									"@MeterRecircCardNumber," +
									"@RecircTransactionAliasGuid," +
									"@TouchKeyReader," +
									"@OffLoadByOffLoadID," +
									"@UseManualMeterData," +
									"@PromptForBOLNumber," +
                                    "@QueryForTrailers," +
                                    "@PromptForGravity, " + 
                                    "@PromptForTemperature, " + 
                                    "@LastTransactionNumber," +
									"@LastTransactionNumberDateTime," +
                                    "@LogCommunications," +
                                    "@LogCommPath," +
                                    "@EnableScully, " +
                                    "@EnableEquipmentValidate, " +
                                    "@StationPromptTimeout," +
                                    "@StationMessageTimeout," + 
                                    "@AssignedMeterGuid, " +
                                    "@EnableDynamicRecipes, " +
                                    "@EthanolExcess " +
                                   ")";
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			command.Parameters.AddWithValue("@ID", this._ID);
			command.Parameters.AddWithValue("@LookupStationTypeIndex", (int)this._Type);
			command.Parameters.AddWithValue("@SwingArmPosition", this._SwingArmPosition);
			command.Parameters.AddWithValue("@VaporRecovery", this._VaporRecovery);
			command.Parameters.AddWithValue("@LookupStationInterfaceTypeIndex", (int)this._InterfaceType);
			command.Parameters.AddWithValue("@Enabled", this._Enabled);
			command.Parameters.AddWithValue("@BOLPrinter", this._BOLPrinter);
			command.Parameters.AddWithValue("@PreloadPrinter", this._PreloadPrinter);
			command.Parameters.AddWithValue("@BOLAgeInMinutes", this._BOLAgeInMinutes);
			command.Parameters.Add(DataObject.NewGuidParameter("@IssueByVolumeTransactionAliasGuid", this._IssueByVolumeTransactionAliasGuid, true));
			command.Parameters.Add(DataObject.NewGuidParameter("@IssueByWeightTransactionAliasGuid", this._IssueByWeightTransactionAliasGuid, true));
			command.Parameters.Add(DataObject.NewGuidParameter("@ReceiptByVolumeTransactionAliasGuid", this._ReceiptByVolumeTransactionAliasGuid, true));
			command.Parameters.Add(DataObject.NewGuidParameter("@ReceiptByWeightTransactionAliasGuid", this._ReceiptByWeightTransactionAliasGuid, true));
			command.Parameters.AddWithValue("@CardReader", this._CardReader);
			command.Parameters.AddWithValue("@ThirtyFiveBitCardSupport", this._ThirtyFiveBitCardSupport);
			command.Parameters.AddWithValue("@CreatedDate", this._CreatedDate);
			command.Parameters.AddWithValue("@CreatedBy", this._CreatedBy);
			command.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			command.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			command.Parameters.AddWithValue("@NumberOfCopies", this._NumberOfCopies);
			command.Parameters.AddWithValue("@NumberOfPreloadCopies", this._NumberOfPreloadCopies);
			command.Parameters.AddWithValue("@InhibitLoadingByLoadID", this._InhibitLoadingByLoadID);
			command.Parameters.AddWithValue("@InhibitOperatingModePrompt", this._InhibitOperatingModePrompt);
			command.Parameters.AddWithValue("@SynchronizeReferenceDensity", this._SynchronizeReferenceDensity);
			command.Parameters.AddWithValue("@SetDefaultPresetToZero", this._SetDefaultPresetToZero);
			command.Parameters.AddWithValue("@SignatureDevice", this._SignatureDevice);
			command.Parameters.Add(DataObject.NewGuidParameter("@TankGuid", this.associatedTankGuid, true));
			command.Parameters.AddWithValue("@ArmsServiced", this.armsServiced);
			command.Parameters.AddWithValue("@InhibitSettingRecipeNames", this._InhibitSettingRecipeNames);
			command.Parameters.AddWithValue("@SignatureDevicePort", this._SignatureDevicePort);
			command.Parameters.AddWithValue("@SignatureDeviceBaudRate", this._SignatureDeviceBaudRate);
			command.Parameters.AddWithValue("@MeterRecircCardNumber", this._MeterRecircCardNumber);
			command.Parameters.Add(DataObject.NewGuidParameter("@RecircTransactionAliasGUid", this._RecircTransactionAliasGuid, true));
			command.Parameters.AddWithValue("@TouchKeyReader", this._TouchKeyReader);
			command.Parameters.AddWithValue("@OffLoadByOffLoadID", this._OffLoadByOffLoadID);
			command.Parameters.AddWithValue("@UseManualMeterData", this._UseManualMeterData);
			command.Parameters.AddWithValue("@PromptForBOLNumber", this._PromptForBOLNumber);
            command.Parameters.AddWithValue("@QueryForTrailers", this._QueryForTrailers);
            command.Parameters.AddWithValue("@PromptForGravity", this._PromptForGravity);
            command.Parameters.AddWithValue("@PromptForTemperature", this._PromptForTemperature);
            command.Parameters.AddWithValue("@LastTransactionNumber", this._LastTransactionNumber);
			command.Parameters.AddWithValue("@LastTransactionNumberDateTime", this._LastTransactionNumberDateTime);
			command.Parameters.AddWithValue("@LogCommunications",this.LogCommunications);
			command.Parameters.AddWithValue("@LogCommPath",this.LogCommPath);
	        command.Parameters.AddWithValue("@EnableScully", this._EnableScully);
            command.Parameters.AddWithValue("@EnableEquipmentValidate", this._EnableEquipmentValidate);
            command.Parameters.AddWithValue("@StationPromptTimeout",this.StationPromptTimeout);
            command.Parameters.AddWithValue("@StationMessageTimeout",this.StationMessageTimeout);
		    command.Parameters.AddWithValue("@AssignedMeterGuid", this.Meter.IdentityGuid);
         command.Parameters.AddWithValue("@EnableDynamicRecipes", this._EnableDynamicRecipes);
         command.Parameters.AddWithValue("@EthanolExcess", this.EthanolExcess);
      }

      /// <summary>
      /// New variant of UpdateSQL which populates a SqlCommand object
      /// 
      /// Variables are added as command parameters instead of inline in the commandtext.
      /// </summary>
      public void UpdateSql(SqlCommand command)
		{
			command.CommandText = "UPDATE tblStations " +
									"SET SiteGuid = @SiteGuid," +
									"ID = @ID," +
									"LookupStationTypeIndex = @LookupStationTypeIndex," +
									"SwingArmPosition = @SwingArmPosition," +
									"VaporRecovery = @VaporRecovery," +
									"LookupStationInterfaceTypeIndex = @LookupStationInterfaceTypeIndex," +
									"Enabled = @Enabled," +
									"BOLPrinter = @BOLPrinter," +
									"PreloadPrinter = @PreloadPrinter," +
									"NumberOfCopies = @NumberOfCopies," +
									"NumberOfPreloadCopies = @NumberOfPreloadCopies," +
									"InhibitLoadingByLoadID = @InhibitLoadingByLoadID," +
									"InhibitOperatingModePrompt = @InhibitOperatingModePrompt," +
									"SynchronizeReferenceDensity = @SynchronizeReferenceDensity," +
									"SetDefaultPresetToZero = @SetDefaultPresetToZero," +
									"SignatureDevice = @SignatureDevice," +
									"BOLAgeInMinutes = @BOLAgeInMinutes," +
									"IssueByVolumeTransactionAliasGuid = @IssueByVolumeTransactionAliasGuid," +
									"IssueByWeightTransactionAliasGuid = @IssueByWeightTransactionAliasGuid," +
									"ReceiptByVolumeTransactionAliasGuid = @ReceiptByVolumeTransactionAliasGuid," +
									"ReceiptByWeightTransactionAliasGuid = @ReceiptByWeightTransactionAliasGuid," +
									"CardReader = @CardReader," +
									"ThirtyFiveBitCardSupport = @ThirtyFiveBitCardSupport," +
									"UpdatedDate = @UpdatedDate," +
									"UpdatedBy = @UpdatedBy," +
									"TankGuid = @TankGuid, " +
									"ArmsServiced = @ArmsServiced," +
									"InhibitSettingRecipeNames = @InhibitSettingRecipeNames," +
									"SignatureDevicePort = @SignatureDevicePort," +
									"SignatureDeviceBaudRate = @SignatureDeviceBaudRate," +
									"MeterRecircCardNumber = @MeterRecircCardNumber," +
									"RecircTransactionAliasGuid = @RecircTransactionAliasGuid," +
									"TouchKeyReader = @TouchKeyReader," +
									"OffLoadByOffLoadID = @OffLoadByOffLoadID," +
									"UseManualMeterData = @UseManualMeterData," +
									"PromptForBOLNumber = @PromptForBOLNumber," +
                                    "QueryForTrailers = @QueryForTrailers, " +
                                    "PromptForGravityCaptured = @PromptForGravity, " +
                                    "PromptForTemperatureCaptured = @PromptForTemperature, " +
                                    "LastTransactionNumber = @LastTransactionNumber," +
									"LastTransactionNumberDateTime = @LastTransactionNumberDateTime, " +
                                    "LogCommunications = @LogCommunications," +
                                     "LogCommPath = @LogCommPath, " +
                                    "EnableScully = @EnableScully, " +
                                    "EnableEquipmentValidate = @EnableEquipmentValidate, " +
                                    "StationPromptTimeout = @StationPromptTimeout, "+
                                    "StationMessageTimeout = @StationMessageTimeout, " +
                                    "AssignedMeterGuid = @AssignedMeterGuid, " +
                                    "EnableDynamicRecipes = @EnableDynamicRecipes, " +
                                    "EthanolExcess = @EthanolExcess " +
"WHERE [StationGuid] = @StationGuid";
		command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			command.Parameters.AddWithValue("@ID", this._ID);
			command.Parameters.AddWithValue("@LookupStationTypeIndex", (int)this._Type);
			command.Parameters.AddWithValue("@SwingArmPosition", this._SwingArmPosition);
			command.Parameters.AddWithValue("@VaporRecovery", this._VaporRecovery);
			command.Parameters.AddWithValue("@LookupStationInterfaceTypeIndex", (int)this._InterfaceType);
			command.Parameters.AddWithValue("@Enabled", this._Enabled);
			command.Parameters.AddWithValue("@BOLPrinter", this._BOLPrinter);
			command.Parameters.AddWithValue("@PreloadPrinter", this._PreloadPrinter);
			command.Parameters.AddWithValue("@NumberOfCopies", this.NumberOfCopies);
			command.Parameters.AddWithValue("@NumberOfPreloadCopies", this.NumberOfPreloadCopies);
			command.Parameters.AddWithValue("@InhibitLoadingByLoadID", this._InhibitLoadingByLoadID);
			command.Parameters.AddWithValue("@InhibitOperatingModePrompt", this._InhibitOperatingModePrompt);
			command.Parameters.AddWithValue("@SynchronizeReferenceDensity", this._SynchronizeReferenceDensity);
			command.Parameters.AddWithValue("@SetDefaultPresetToZero", this._SetDefaultPresetToZero);
			command.Parameters.AddWithValue("@SignatureDevice", this._SignatureDevice);
			command.Parameters.AddWithValue("@BOLAgeInMinutes", this._BOLAgeInMinutes);
			command.Parameters.AddWithValue("@StationGuid", this._IdentityGuid);
			command.Parameters.Add(DataObject.NewGuidParameter("@IssueByVolumeTransactionAliasGuid", this._IssueByVolumeTransactionAliasGuid, true));
			command.Parameters.Add(DataObject.NewGuidParameter("@IssueByWeightTransactionAliasGuid", this._IssueByWeightTransactionAliasGuid, true));
			command.Parameters.Add(DataObject.NewGuidParameter("@ReceiptByVolumeTransactionAliasGuid", this._ReceiptByVolumeTransactionAliasGuid, true));
			command.Parameters.Add(DataObject.NewGuidParameter("@ReceiptByWeightTransactionAliasGuid", this._ReceiptByWeightTransactionAliasGuid, true));
			command.Parameters.AddWithValue("@CardReader", this._CardReader);
			command.Parameters.AddWithValue("@ThirtyFiveBitCardSupport", this._ThirtyFiveBitCardSupport);
			command.Parameters.AddWithValue("@UpdatedDate", this._UpdatedDate);
			command.Parameters.AddWithValue("@UpdatedBy", this._UpdatedBy);
			command.Parameters.Add(DataObject.NewGuidParameter("@TankGuid", this.associatedTankGuid, true));
			command.Parameters.AddWithValue("@ArmsServiced", this.armsServiced);
			command.Parameters.AddWithValue("@InhibitSettingRecipeNames", this._InhibitSettingRecipeNames);
			command.Parameters.AddWithValue("@SignatureDevicePort", this._SignatureDevicePort);
			command.Parameters.AddWithValue("@SignatureDeviceBaudRate", this._SignatureDeviceBaudRate);
			command.Parameters.AddWithValue("@MeterRecircCardNumber", this._MeterRecircCardNumber);
			command.Parameters.Add(DataObject.NewGuidParameter("@RecircTransactionAliasGuid", this._RecircTransactionAliasGuid, true));
			command.Parameters.AddWithValue("@TouchKeyReader", this._TouchKeyReader);
			command.Parameters.AddWithValue("@OffLoadByOffLoadID", this._OffLoadByOffLoadID);
			command.Parameters.AddWithValue("@UseManualMeterData", this._UseManualMeterData);
			command.Parameters.AddWithValue("@PromptForBOLNumber", this._PromptForBOLNumber);
            command.Parameters.AddWithValue("@QueryForTrailers", this._QueryForTrailers);
            command.Parameters.AddWithValue("@PromptForGravity", this._PromptForGravity);
            command.Parameters.AddWithValue("@PromptForTemperature", this._PromptForTemperature);
            command.Parameters.AddWithValue("@LastTransactionNumber", this._LastTransactionNumber);
			command.Parameters.AddWithValue("@LastTransactionNumberDateTime", this._LastTransactionNumberDateTime);
			command.Parameters.AddWithValue("@LogCommunications",this.LogCommunications);
			command.Parameters.AddWithValue("@LogCommPath",this.LogCommPath);
	        command.Parameters.AddWithValue("@EnableScully", this._EnableScully);
            command.Parameters.AddWithValue("@EnableEquipmentValidate", this._EnableEquipmentValidate);
            command.Parameters.AddWithValue("@StationPromptTimeout",this.StationPromptTimeout);
            command.Parameters.AddWithValue("@StationMessageTimeout",this.StationMessageTimeout);
		    command.Parameters.AddWithValue("@AssignedMeterGuid", this.Meter.IdentityGuid);
         command.Parameters.AddWithValue("@EnableDynamicRecipes", this._EnableDynamicRecipes);
         command.Parameters.AddWithValue("@EthanolExcess", this.EthanolExcess);
      }

      /// <summary>
      /// New variant of PurgeSql which populates a SqlCommand object
      /// 
      /// Variables are added as command parameters instead of inline in the commandtext.
      /// </summary>
      public void PurgeSQL(SqlCommand cmd)
		{
			cmd.CommandText = "DELETE FROM tblStations " +
					"WHERE [StationGuid] = @StationGuid";

			cmd.Parameters.AddWithValue("@StationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// New variant of SelectSQL which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// </summary>
		public void SelectSQL(SqlCommand command, bool bInTransaction)
		{
			command.CommandText = this.SelectClause +
									" FROM tblStations " + SQLUpdateLock(bInTransaction) +
									" WHERE [StationGuid] = @StationGuid";
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@StationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// New variant of SelectSQL which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// 
		/// Get the next recipe preset number across the arms for this station
		/// </summary>
		public void NextPresetNumberSQL(SqlCommand command, bool bInTransaction)
		{
			command.CommandText = "SELECT TOP 1 PresetNumber" +
									" FROM map.tblProductToPresetRecipe recipe INNER JOIN tblLoadArms loadArm ON recipe.AssignedToLoadArmGuid = loadArm.LoadArmGuid " + SQLUpdateLock(bInTransaction) +
									" WHERE LoadArmGuid in (SELECT LoadArmGuid FROM tblLoadArms " + SQLUpdateLock(bInTransaction) + " WHERE BayAStationGuid = @StationGuid OR BayBStationGuid = @StationGuid)" +
									" ORDER BY PresetNumber desc";
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@StationGuid", this._IdentityGuid);
		}

		/// <summary>
		/// New variant of SelectSQL which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// 
		/// Get the status of whether Enable Dynamic Recipes on a station is set or not?
		/// </summary>
		public void IsDynamicRecipesEnabled(SqlCommand command)
		{
			command.CommandText = "SELECT EnableDynamicRecipes" +
									" FROM dbo.tblStations  " +
									" WHERE StationGuid = @StationGuid " +
									" AND LookupStationTypeIndex = @LookupStationTypeIndex ";

			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@StationGuid", this._IdentityGuid);
			command.Parameters.AddWithValue("@LookupStationTypeIndex", (int)this._Type);
		}

		/// <summary>
		/// New variant of SelectSQL which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// 
		/// Get the status of whether Enable Dynamic Recipes on a station is set or not?
		/// </summary>
		public void IsDynamicRecipesEnabledOnPartnerStations(SqlCommand command, List<Guid> partnerStationGuids)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("SELECT EnableDynamicRecipes FROM dbo.tblStations WHERE LookupStationTypeIndex = ");
			stringBuilder.Append((int)this._Type);
			stringBuilder.Append(" AND StationGuid IN ('");
			
			foreach (var guid in partnerStationGuids)
			{
				stringBuilder.Append(guid);
				stringBuilder.Append("','");
			}
			stringBuilder.Remove(stringBuilder.Length - 2, 2);
			stringBuilder.Append(")");

			command.CommandText = stringBuilder.ToString();
			command.CommandType = CommandType.Text;
		}

		/// <summary>
		/// New variant of SelectByIDSQL which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// </summary>
		public void SelectByIDSQL(SqlCommand command, bool bInTransaction)
		{
			command.CommandText = this.SelectClause +
									"FROM tblStations " + SQLUpdateLock(bInTransaction) +
									" WHERE SiteGuid = @SiteGuid" +
									" AND ID = @ID";
			command.CommandType = CommandType.Text;
			command.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			command.Parameters.AddWithValue("@ID", this._ID);
		}

		/// <summary>
		/// New variant of EnumerateSql which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// </summary>
		public void EnumerateSQL(SqlCommand cmd)
		{
			cmd.CommandText = this.SelectClause +
					" FROM tblStations" +
					" WHERE SiteGuid = @SiteGuid" +
					" ORDER BY ID";

			cmd.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
		}

		/// <summary>
		/// New variant of EnumerateByTypeSQL which populates a SqlCommand object
		/// 
		/// Variables are added as command parameters instead of inline in the commandtext.
		/// </summary>
		public void EnumerateByTypeSQL(SqlCommand command)
		{
			command.CommandText = this.SelectClause +
									"FROM tblStations" +
									" WHERE SiteGuid = @SiteGuid " +
									" AND LookupStationTypeIndex = @LookupStationTypeIndex " +
									" ORDER BY ID";
			command.CommandType = CommandType.Text;

			command.Parameters.AddWithValue("@SiteGuid", this._SiteGuid);
			command.Parameters.AddWithValue("@LookupStationTypeIndex", (int)this._Type);
		}
	}
}
