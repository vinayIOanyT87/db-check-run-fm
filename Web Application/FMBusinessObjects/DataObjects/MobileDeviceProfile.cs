// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MobileDeviceProfile.cs" company="Varec, Inc.">
//   Copyright (c) Varec, Inc.  All rights reserved.
// </copyright>
// <summary>
//   Defines the MobileDeviceProfile type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace FMBusinessObjects.DataObjects
{
	using System;
	using System.Collections.Generic;
	using System.Data;
	using System.Data.SqlClient;
	using System.Runtime.Serialization;

	/// <summary>
	/// The purpose of the Mobile Device Profile data object is to contain data and SQL to add,
	/// modify, and delete profile information from the database.
	/// </summary>
	[Serializable]
	[DataContract]
	public class MobileDeviceProfile : DataObject
	{
		#region Private data members
		[DataMember] private Guid mobileDeviceProfileGuid;
		[DataMember] private Guid siteGuid;
		[DataMember] private string profileId;
		[DataMember] private string description;
		[DataMember] private bool showProductScreen;
		[DataMember] private bool generateTicketNumber;
		[DataMember] private bool showOperatorFieldInFlightList;
		[DataMember] private bool useDefaultPrinter;
		[DataMember] private string defaultPrinter;
		[DataMember] private byte[] adminPassword;
		[DataMember] private string shutdownHotKey;
		[DataMember] private string printerComPort;
		[DataMember] private int? searchType;
		[DataMember] private bool loggingOption;
		[DataMember] private int? allowableFailedLoginAttempts;
		[DataMember] private int? fuelDistributionPrecision;
		[DataMember] private bool makeDefaultProfile;
		[DataMember] private string vehicleId;
		[DataMember] private bool monitorScreenTransitionTiming;
		[DataMember] private bool bypassFsrCheckOnScreenTrans;
		[DataMember] private bool showFuelUpdateCheckStatusWin;
		[DataMember] private double? rtdTemperatureRangeMax;
		[DataMember] private double? rtdTemperatureRangeMin;
		[DataMember] private double? defaultTemperature;
		[DataMember] private bool strictUserValidation;
		[DataMember] private bool verifyFuelingEquipment;
		[DataMember] private bool allowEditRequiredFuelLoad;
		[DataMember] private bool allowBackAfterArrivalScreen;
		[DataMember] private bool allowBackAfterTicketPrinted;
		[DataMember] private bool requirePrint;
		[DataMember] private bool totalFuelLoadCheck;
		[DataMember] private bool volumetricThresholdValidation;
		[DataMember] private bool validateShipNumber;
		[DataMember] private bool allowVtoModification;
		[DataMember] private bool allowFlightGateModification;
		[DataMember] private int? tankPositionBalanceVerification;
		[DataMember] private double? tankPositionBalancePercentage;
		[DataMember] private bool overrideWingBalancePercentVar;
		[DataMember] private bool bypassDistributionTolerance;
		[DataMember] private bool vehicleIdCheck;
		[DataMember] private bool gseFuelMustMatch;
		[DataMember] private bool allowManualMeter;
		[DataMember] private bool useValidLogicGaTrans;
		[DataMember] private bool allowShipNumberModification;
		[DataMember] private bool allowAircraftTypeModification;
		[DataMember] private bool allowDestinationModification;
		[DataMember] private int? ticketPrinting;
		[DataMember] private int? aircraftTypeVerification;
		[DataMember] private int? destination;
		[DataMember] private int? gate;
		[DataMember] private int? shipNumber;
		[DataMember] private int? meterTotal;
		[DataMember] private int? volumePumped;
		[DataMember] private int? tankCapacity;
		[DataMember] private bool eaStrictUserValidation;
		[DataMember] private bool eaVerifyFuelingEquipment;
		[DataMember] private bool eaAllowEditOfRequiredFuelLoad;
		[DataMember] private bool eaAllowBackAfterArrivalScreen;
		[DataMember] private bool eaAllowBackAfterTicketPrinted;
		[DataMember] private bool eaRequirePrint;
		[DataMember] private bool eaTotalFuelLoad;
		[DataMember] private bool eaVolumetricThresholdValidation;
		[DataMember] private bool eaValidateShipNumber;
		[DataMember] private bool eaAllowVtoModification;
		[DataMember] private bool eaAllowFlightGateModification;
		[DataMember] private bool eaTankDiffPercentage;
		[DataMember] private bool eaWingBalancePercentage;
		[DataMember] private bool eaBypassDistributionTolerance;
		[DataMember] private bool eaVehicleIdCheck;
		[DataMember] private bool eaGseFuelMustMatch;
		[DataMember] private bool eaAllowManualMeter;
		[DataMember] private bool eaUseValidationLogicGaTrans;
		[DataMember] private bool eaAllowShipNumberModification;
		[DataMember] private bool eaAllowAircraftTypeModification;
		[DataMember] private bool eaAllowDestinationModification;
		[DataMember] private bool eaDestination;
		[DataMember] private bool eaTicketPrinting;
		[DataMember] private bool eaAircraftType;
		[DataMember] private bool eaShipNumber;
		[DataMember] private bool eaGateNumber;
		[DataMember] private bool eaMeterTotal;
		[DataMember] private bool eaVolumePumped;
		[DataMember] private bool eaTankCapacity;
		[DataMember] private int? equipmentType;
		[DataMember] private Guid foreignKeyToMapEquipment;
		[DataMember] private Guid issueTransaction;
		[DataMember] private Guid defuelTransaction;
		[DataMember] private Guid rotationTransaction;
		[DataMember] private Guid meterCloseout;
		[DataMember] private Guid deIceTransaction;
		[DataMember] private Guid gseTransaction;
		[DataMember] private Guid manualConsumer;
		[DataMember] private Guid manualVendor;
		[DataMember] private Guid manualShipper;
		[DataMember] private Guid manualManager;
		[DataMember] private Guid manualSupplier;
		[DataMember] private Guid manualBillTo;
		[DataMember] private Guid manualProduct;
		[DataMember] private int manualStationId;
		[DataMember] private Guid closeoutConsumer;
		[DataMember] private Guid closeoutOwner;
		[DataMember] private Guid closeoutVendor;
		[DataMember] private bool inhibitOverridingTemperature;
		[DataMember] private double? manualTemperature;
		[DataMember] private double? manualDensity;
		[DataMember] private bool hasDCU;
		[DataMember] private bool bluetoothDCU;
		[DataMember] private bool logDCUActions;
		[DataMember] private bool hasAveryHardoll;
		[DataMember] private string dcuComPort;
		[DataMember] private int? dcuReadRetry;
		[DataMember] private int? dcuDisconnectDelay;
		[DataMember] private int? dcuCommunicationFailRestart;
		[DataMember] private string averyHardollComPort;
		[DataMember] private string averyHardollMeterId;
		[DataMember] private bool confirmFuelCaps;
		[DataMember] private bool vtoEnabled;
		[DataMember] private bool enabledInOpGauges;
		[DataMember] private bool useDispensingVehicleGseTrans;
		[DataMember] private int? gseWaitMsecForGetMeter;
		[DataMember] private int? gseInactiveLogoutMinutes;
		[DataMember] private int? gseInactiveTimeout;
		[DataMember] private int? barcodeInvalidWarningSeconds;
		[DataMember] private double? deIceBlendDefault;
		[DataMember] private int? communicationTimeoutSeconds;
		[DataMember] private int? connectionRetries;
		[DataMember] private int? connectionRetryTimeout;
		[DataMember] private int? connectionType;
		[DataMember] private int? updateInterval;
		[DataMember] private bool pingVerificationIpAddress;
		[DataMember] private int? vehicleUpdateInterval;
		[DataMember] private int? presubmitDelay;
		[DataMember] private string verificationIpAddress;
		[DataMember] private DateTimeOffset? createdDate;
		[DataMember] private DateTimeOffset? updatedDate;
		[DataMember] private string updatedBy;
		[DataMember] private string createdBy;
		[DataMember] private MobileDeviceProfileAnalogInputCollection analogInputCollection;
		[DataMember] private MobileDeviceProfilePrinterCollection printerCollection;
		[DataMember] private MobileDeviceProfileToMobileDeviceMapCollection assignedMobileDeviceCollection;
		[DataMember]private MobileDeviceProfileToMobileDeviceMapCollection unassignedMobileDeviceCollection;
		[DataMember] private List<MobileDeviceProfilePrinter> deletedPrinterCollection;
		[DataMember] private List<MobileDeviceProfileAnalogInput> deletedAnalogInputCollection;
		[DataMember] private List<MobileDeviceProfileToMobileDeviceMapClass> removeMobileDeviceMapCollection; 
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="MobileDeviceProfile"/> class. 
		/// This is the default constructor for the Mobile Device Profile class.
		/// </summary>
		public MobileDeviceProfile ( )
		{
			this.Reset ( );
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the mobile device profile guid.
		/// </summary>
		public Guid MobileDeviceProfileGuid
		{
			get { return this.mobileDeviceProfileGuid; }
			set { this.mobileDeviceProfileGuid = value; }
		}

		/// <summary>
		/// Gets or sets the site guid.
		/// </summary>
		public Guid SiteGuid
		{
			get { return this.siteGuid; }
			set { this.siteGuid = value; }
		}

		/// <summary>
		/// Gets or sets the profile id.
		/// </summary>
		public string ProfileId
		{
			get { return this.profileId; }
			set { this.profileId = value; }
		}

		/// <summary>
		/// Gets or sets the description.
		/// </summary>
		public string Description
		{
			get { return this.description; }
			set { this.description = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether confirm fuel caps.
		/// </summary>
		public bool ConfirmFuelCaps
		{
			get { return this.confirmFuelCaps; }
			set { this.confirmFuelCaps = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show product screen.
		/// </summary>
		public bool ShowProductScreen
		{
			get { return this.showProductScreen; }
			set { this.showProductScreen = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether generate ticket number.
		/// </summary>
		public bool GenerateTicketNumber
		{
			get { return this.generateTicketNumber; }
			set { this.generateTicketNumber = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show operator field in flight list.
		/// </summary>
		public bool ShowOperatorFieldInFlightList
		{
			get { return this.showOperatorFieldInFlightList; }
			set { this.showOperatorFieldInFlightList = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether use default printer.
		/// </summary>
		public bool UseDefaultPrinter
		{
			get { return this.useDefaultPrinter; }
			set { this.useDefaultPrinter = value; }
		}

		/// <summary>
		/// Gets or sets the default printer.
		/// </summary>
		public string DefaultPrinter
		{
			get { return this.defaultPrinter; }
			set { this.defaultPrinter = value; }
		}

		/// <summary>
		/// Gets or sets the admin password.
		/// </summary>
		public byte[] AdminPassword
		{
			get { return this.adminPassword; }
		    set { this.adminPassword = value; }
		}

		/// <summary>
		/// Gets or sets the shutdown hot key.
		/// </summary>
		public string ShutdownHotKey
		{
			get { return this.shutdownHotKey; }
			set { this.shutdownHotKey = value; }
		}

		/// <summary>
		/// Gets or sets the printer com port.
		/// </summary>
		public string PrinterComPort
		{
			get { return this.printerComPort; }
			set { this.printerComPort = value; }
		}

		/// <summary>
		/// Gets or sets the search type.
		/// </summary>
		public int? SearchType
		{
			get { return this.searchType; }
			set { this.searchType = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether logging option.
		/// </summary>
		public bool LoggingOption
		{
			get { return this.loggingOption; }
			set { this.loggingOption = value; }
		}

		/// <summary>
		/// Gets or sets the allowable failed login attempts.
		/// </summary>
		public int? AllowableFailedLoginAttempts
		{
			get { return this.allowableFailedLoginAttempts; }
			set { this.allowableFailedLoginAttempts = value; }
		}

		/// <summary>
		/// Gets or sets the fuel distribution precision.
		/// </summary>
		public int? FuelDistributionPrecision
		{
			get { return this.fuelDistributionPrecision; }
			set { this.fuelDistributionPrecision = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether make default profile.
		/// </summary>
		public bool MakeDefaultProfile
		{
			get { return this.makeDefaultProfile; }
			set { this.makeDefaultProfile = value; }
		}

		/// <summary>
		/// Gets or sets the vehicle id.
		/// </summary>
		public string VehicleId
		{
			get { return this.vehicleId; }
			set { this.vehicleId = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether monitor screen transition timing.
		/// </summary>
		public bool MonitorScreenTransitionTiming
		{
			get { return this.monitorScreenTransitionTiming; }
			set { this.monitorScreenTransitionTiming = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether bypass fsr check on screen trans.
		/// </summary>
		public bool BypassFsrCheckOnScreenTrans
		{
			get { return this.bypassFsrCheckOnScreenTrans; }
			set { this.bypassFsrCheckOnScreenTrans = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether show fuel update check status win.
		/// </summary>
		public bool ShowFuelUpdateCheckStatusWin
		{
			get { return this.showFuelUpdateCheckStatusWin; }
			set { this.showFuelUpdateCheckStatusWin = value; }
		}

		/// <summary>
		/// Gets or sets the rtd temperature range max.
		/// </summary>
		public double? RtdTemperatureRangeMax
		{
			get { return this.rtdTemperatureRangeMax; }
			set { this.rtdTemperatureRangeMax = value; }
		}

		/// <summary>
		/// Gets or sets the rtd temperature range min.
		/// </summary>
		public double? RtdTemperatureRangeMin
		{
			get { return this.rtdTemperatureRangeMin; }
			set { this.rtdTemperatureRangeMin = value; }
		}

		/// <summary>
		/// Gets or sets the default temperature.
		/// </summary>
		public double? DefaultTemperature
		{
			get { return this.defaultTemperature; }
			set { this.defaultTemperature = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether strict user validation.
		/// </summary>
		public bool StrictUserValidation
		{
			get { return this.strictUserValidation; }
			set { this.strictUserValidation = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether verify fueling equipment.
		/// </summary>
		public bool VerifyFuelingEquipment
		{
			get { return this.verifyFuelingEquipment; }
			set { this.verifyFuelingEquipment = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow edit required fuel load.
		/// </summary>
		public bool AllowEditRequiredFuelLoad
		{
			get { return this.allowEditRequiredFuelLoad; }
			set { this.allowEditRequiredFuelLoad = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow back after arrival screen.
		/// </summary>
		public bool AllowBackAfterArrivalScreen
		{
			get { return this.allowBackAfterArrivalScreen; }
			set { this.allowBackAfterArrivalScreen = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow back after ticket printed.
		/// </summary>
		public bool AllowBackAfterTicketPrinted
		{
			get { return this.allowBackAfterTicketPrinted; }
			set { this.allowBackAfterTicketPrinted = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether require print.
		/// </summary>
		public bool RequirePrint
		{
			get { return this.requirePrint; }
			set { this.requirePrint = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether total fuel load check.
		/// </summary>
		public bool TotalFuelLoadCheck
		{
			get { return this.totalFuelLoadCheck; }
			set { this.totalFuelLoadCheck = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether volumetric threshold validation.
		/// </summary>
		public bool VolumetricThresholdValidation
		{
			get { return this.volumetricThresholdValidation; }
			set { this.volumetricThresholdValidation = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether validate ship number.
		/// </summary>
		public bool ValidateShipNumber
		{
			get { return this.validateShipNumber; }
			set { this.validateShipNumber = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow vto modification.
		/// </summary>
		public bool AllowVtoModification
		{
			get { return this.allowVtoModification; }
			set { this.allowVtoModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow flight gate modification.
		/// </summary>
		public bool AllowFlightGateModification
		{
			get { return this.allowFlightGateModification; }
			set { this.allowFlightGateModification = value; }
		}

		/// <summary>
		/// Gets or sets the tank position balance verification.
		/// </summary>
		public int? TankPositionBalanceVerification
		{
			get { return this.tankPositionBalanceVerification; }
			set { this.tankPositionBalanceVerification = value; }
		}

		/// <summary>
		/// Gets or sets the tank position balance percentage.
		/// </summary>
		public double? TankPositionBalancePercentage
		{
			get { return this.tankPositionBalancePercentage; }
			set { this.tankPositionBalancePercentage = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether override wing balance percent var.
		/// </summary>
		public bool OverrideWingBalancePercentVar
		{
			get { return this.overrideWingBalancePercentVar; }
			set { this.overrideWingBalancePercentVar = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether bypass distribution tolerance.
		/// </summary>
		public bool BypassDistributionTolerance
		{
			get { return this.bypassDistributionTolerance; }
			set { this.bypassDistributionTolerance = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether vehicle id check.
		/// </summary>
		public bool VehicleIdCheck
		{
			get { return this.vehicleIdCheck; }
			set { this.vehicleIdCheck = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether gse fuel must match.
		/// </summary>
		public bool GseFuelMustMatch
		{
			get { return this.gseFuelMustMatch; }
			set { this.gseFuelMustMatch = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow manual meter.
		/// </summary>
		public bool AllowManualMeter
		{
			get { return this.allowManualMeter; }
			set { this.allowManualMeter = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether use valid logic ga trans.
		/// </summary>
		public bool UseValidLogicGaTrans
		{
			get { return this.useValidLogicGaTrans; }
			set { this.useValidLogicGaTrans = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow ship number modification.
		/// </summary>
		public bool AllowShipNumberModification
		{
			get { return this.allowShipNumberModification; }
			set { this.allowShipNumberModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow aircraft type modification.
		/// </summary>
		public bool AllowAircraftTypeModification
		{
			get { return this.allowAircraftTypeModification; }
			set { this.allowAircraftTypeModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether allow destination modification.
		/// </summary>
		public bool AllowDestinationModification
		{
			get { return this.allowDestinationModification; }
			set { this.allowDestinationModification = value; }
		}

		/// <summary>
		/// Gets or sets the ticket printing.
		/// </summary>
		public int? TicketPrinting
		{
			get { return this.ticketPrinting; }
			set { this.ticketPrinting = value; }
		}

		/// <summary>
		/// Gets or sets the aircraft type verification.
		/// </summary>
		public int? AircraftTypeVerification
		{
			get { return this.aircraftTypeVerification; }
			set { this.aircraftTypeVerification = value; }
		}

		/// <summary>
		/// Gets or sets the destination.
		/// </summary>
		public int? Destination
		{
			get { return this.destination; }
			set { this.destination = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether gate.
		/// </summary>
		public int? Gate
		{
			get { return this.gate; }
			set { this.gate = value; }
		}

		/// <summary>
		/// Gets or sets the ship number.
		/// </summary>
		public int? ShipNumber
		{
			get { return this.shipNumber; }
			set { this.shipNumber = value; }
		}

		/// <summary>
		/// Gets or sets the meter total.
		/// </summary>
		public int? MeterTotal
		{
			get { return this.meterTotal; }
			set { this.meterTotal = value; }
		}

		/// <summary>
		/// Gets or sets the volume pumped.
		/// </summary>
		public int? VolumePumped
		{
			get { return this.volumePumped; }
			set { this.volumePumped = value; }
		}

		/// <summary>
		/// Gets or sets the tank capacity.
		/// </summary>
		public int? TankCapacity
		{
			get { return this.tankCapacity; }
			set { this.tankCapacity = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea strict user validation.
		/// </summary>
		public bool EaStrictUserValidation
		{
			get { return this.eaStrictUserValidation; }
			set { this.eaStrictUserValidation = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea verify fueling equipment.
		/// </summary>
		public bool EaVerifyFuelingEquipment
		{
			get { return this.eaVerifyFuelingEquipment; }
			set { this.eaVerifyFuelingEquipment = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow edit of required fuel load.
		/// </summary>
		public bool EaAllowEditOfRequiredFuelLoad
		{
			get { return this.eaAllowEditOfRequiredFuelLoad; }
			set { this.eaAllowEditOfRequiredFuelLoad = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow back after arrival screen.
		/// </summary>
		public bool EaAllowBackAfterArrivalScreen
		{
			get { return this.eaAllowBackAfterArrivalScreen; }
			set { this.eaAllowBackAfterArrivalScreen = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow back after ticket printed.
		/// </summary>
		public bool EaAllowBackAfterTicketPrinted
		{
			get { return this.eaAllowBackAfterTicketPrinted; }
			set { this.eaAllowBackAfterTicketPrinted = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea require print.
		/// </summary>
		public bool EaRequirePrint
		{
			get { return this.eaRequirePrint; }
			set { this.eaRequirePrint = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea total fuel load.
		/// </summary>
		public bool EaTotalFuelLoad
		{
			get { return this.eaTotalFuelLoad; }
			set { this.eaTotalFuelLoad = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea volumetric threshold validation.
		/// </summary>
		public bool EaVolumetricThresholdValidation
		{
			get { return this.eaVolumetricThresholdValidation; }
			set { this.eaVolumetricThresholdValidation = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea validate ship number.
		/// </summary>
		public bool EaValidateShipNumber
		{
			get { return this.eaValidateShipNumber; }
			set { this.eaValidateShipNumber = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow vto modification.
		/// </summary>
		public bool EaAllowVtoModification
		{
			get { return this.eaAllowVtoModification; }
			set { this.eaAllowVtoModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow flight gate modification.
		/// </summary>
		public bool EaAllowFlightGateModification
		{
			get { return this.eaAllowFlightGateModification; }
			set { this.eaAllowFlightGateModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea tank diff percentage.
		/// </summary>
		public bool EaTankDiffPercentage
		{
			get { return this.eaTankDiffPercentage; }
			set { this.eaTankDiffPercentage = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea wing balance percentage.
		/// </summary>
		public bool EaWingBalancePercentage
		{
			get { return this.eaWingBalancePercentage; }
			set { this.eaWingBalancePercentage = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea bypass distribution tolerance.
		/// </summary>
		public bool EaBypassDistributionTolerance
		{
			get { return this.eaBypassDistributionTolerance; }
			set { this.eaBypassDistributionTolerance = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea vehicle id check.
		/// </summary>
		public bool EaVehicleIdCheck
		{
			get { return this.eaVehicleIdCheck; }
			set { this.eaVehicleIdCheck = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea gse fuel must match.
		/// </summary>
		public bool EaGseFuelMustMatch
		{
			get { return this.eaGseFuelMustMatch; }
			set { this.eaGseFuelMustMatch = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow manual meter.
		/// </summary>
		public bool EaAllowManualMeter
		{
			get { return this.eaAllowManualMeter; }
			set { this.eaAllowManualMeter = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea use validation logic ga trans.
		/// </summary>
		public bool EaUseValidationLogicGaTrans
		{
			get { return this.eaUseValidationLogicGaTrans; }
			set { this.eaUseValidationLogicGaTrans = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow ship number modification.
		/// </summary>
		public bool EaAllowShipNumberModification
		{
			get { return this.eaAllowShipNumberModification; }
			set { this.eaAllowShipNumberModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow aircraft type modification.
		/// </summary>
		public bool EaAllowAircraftTypeModification
		{
			get { return this.eaAllowAircraftTypeModification; }
			set { this.eaAllowAircraftTypeModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea allow destination modification.
		/// </summary>
		public bool EaAllowDestinationModification
		{
			get { return this.eaAllowDestinationModification; }
			set { this.eaAllowDestinationModification = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea destination.
		/// </summary>
		public bool EaDestination
		{
			get { return this.eaDestination; }
			set { this.eaDestination = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea ticket printing.
		/// </summary>
		public bool EaTicketPrinting
		{
			get { return this.eaTicketPrinting; }
			set { this.eaTicketPrinting = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea aircraft type.
		/// </summary>
		public bool EaAircraftType
		{
			get { return this.eaAircraftType; }
			set { this.eaAircraftType = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea ship number.
		/// </summary>
		public bool EaShipNumber
		{
			get { return this.eaShipNumber; }
			set { this.eaShipNumber = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea gate number.
		/// </summary>
		public bool EaGateNumber
		{
			get { return this.eaGateNumber; }
			set { this.eaGateNumber = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea meter total.
		/// </summary>
		public bool EaMeterTotal
		{
			get { return this.eaMeterTotal; }
			set { this.eaMeterTotal = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea volume pumped.
		/// </summary>
		public bool EaVolumePumped
		{
			get { return this.eaVolumePumped; }
			set { this.eaVolumePumped = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ea tank capacity.
		/// </summary>
		public bool EaTankCapacity
		{
			get { return this.eaTankCapacity; }
			set { this.eaTankCapacity = value; }
		}

		/// <summary>
		/// Gets or sets the equipment type.
		/// </summary>
		public int? EquipmentType
		{
			get { return this.equipmentType; }
			set { this.equipmentType = value; }
		}

		/// <summary>
		/// Gets or sets the foreign key to map equipment.
		/// </summary>
		public Guid ForeignKeyToMapEquipment
		{
			get { return this.foreignKeyToMapEquipment; }
			set { this.foreignKeyToMapEquipment = value; }
		}

		/// <summary>
		/// Gets or sets the issue transaction.
		/// </summary>
		public Guid IssueTransaction
		{
			get { return this.issueTransaction; }
			set { this.issueTransaction = value; }
		}

		/// <summary>
		/// Gets or sets the defuel transaction.
		/// </summary>
		public Guid DefuelTransaction
		{
			get { return this.defuelTransaction; }
			set { this.defuelTransaction = value; }
		}

		/// <summary>
		/// Gets or sets the rotation transaction.
		/// </summary>
		public Guid RotationTransaction
		{
			get { return this.rotationTransaction; }
			set { this.rotationTransaction = value; }
		}

		/// <summary>
		/// Gets or sets the meter closeout.
		/// </summary>
		public Guid MeterCloseout
		{
			get { return this.meterCloseout; }
			set { this.meterCloseout = value; }
		}

		/// <summary>
		/// Gets or sets the de ice transaction.
		/// </summary>
		public Guid DeIceTransaction
		{
			get { return this.deIceTransaction; }
			set { this.deIceTransaction = value; }
		}

		/// <summary>
		/// Gets or sets the gse transaction.
		/// </summary>
		public Guid GseTransaction
		{
			get { return this.gseTransaction; }
			set { this.gseTransaction = value; }
		}

		/// <summary>
		/// Gets or sets the manual consumer.
		/// </summary>
		public Guid ManualConsumer
		{
			get { return this.manualConsumer; }
			set { this.manualConsumer = value; }
		}

		/// <summary>
		/// Gets or sets the manual vendor.
		/// </summary>
		public Guid ManualVendor
		{
			get { return this.manualVendor; }
			set { this.manualVendor = value; }
		}

		/// <summary>
		/// Gets or sets the closeout owner.
		/// </summary>
		public Guid CloseoutOwner
		{
			get { return this.closeoutOwner; }
			set { this.closeoutOwner = value; }
		}

		/// <summary>
		/// Gets or sets the closeout vendor.
		/// </summary>
		public Guid CloseoutVendor
		{
			get { return this.closeoutVendor; }
			set { this.closeoutVendor = value; }
		}

		/// <summary>
		/// Gets or sets the closeout shipper.
		/// </summary>
		public Guid ManualShipper
		{
			get { return this.manualShipper; }
			set { this.manualShipper = value; }
		}

		/// <summary>
		/// Gets or sets the closeout manager.
		/// </summary>
		public Guid ManualManager
		{
			get { return this.manualManager; }
			set { this.manualManager = value; }
		}

		/// <summary>
		/// Gets or sets the closeout supplier.
		/// </summary>
		public Guid ManualSupplier
		{
			get { return this.manualSupplier; }
			set { this.manualSupplier = value; }
		}

		/// <summary>
		/// Gets or sets the closeout bill to.
		/// </summary>
		public Guid ManualBillTo
		{
			get { return this.manualBillTo; }
			set { this.manualBillTo = value; }
		}

		/// <summary>
		/// Gets or sets the manual consumer.
		/// </summary>
		public Guid CloseoutConsumer
		{
			get { return this.closeoutConsumer; }
			set { this.closeoutConsumer = value; }
		}

		/// <summary>
		/// Gets or sets the manual product.
		/// </summary>
		public Guid ManualProduct
		{
			get { return this.manualProduct; }
			set { this.manualProduct = value; }
		}

		/// <summary>
		/// Gets or sets the manual station id.
		/// </summary>
		public int ManualStationId
		{
			get { return this.manualStationId; }
			set { this.manualStationId = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether inhibit overriding temperature.
		/// </summary>
		public bool InhibitOverridingTemperature
		{
			get { return this.inhibitOverridingTemperature; }
			set { this.inhibitOverridingTemperature = value; }
		}

		/// <summary>
		/// Gets or sets the manual temperature.
		/// </summary>
		public double? ManualTemperature
		{
			get { return this.manualTemperature; }
			set { this.manualTemperature = value; }
		}

		/// <summary>
		/// Gets or sets the manual density.
		/// </summary>
		public double? ManualDensity
		{
			get { return this.manualDensity; }
			set { this.manualDensity = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether has dcu.
		/// </summary>
		public bool HasDCU
		{
			get { return this.hasDCU; }
			set { this.hasDCU = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether bluetooth dcu.
		/// </summary>
		public bool BluetoothDcu
		{
			get { return this.bluetoothDCU; }
			set { this.bluetoothDCU = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether log dcu actions.
		/// </summary>
		public bool LogDCUActions
		{
			get { return this.logDCUActions; }
			set { this.logDCUActions = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether has avery hardoll.
		/// </summary>
		public bool HasAveryHardoll
		{
			get { return this.hasAveryHardoll; }
			set { this.hasAveryHardoll = value; }
		}

		/// <summary>
		/// Gets or sets the dcu com port.
		/// </summary>
		public string DcuComPort
		{
			get { return this.dcuComPort; }
			set { this.dcuComPort = value; }
		}

		/// <summary>
		/// Gets or sets the dcu read retry.
		/// </summary>
		public int? DcuReadRetry
		{
			get { return this.dcuReadRetry; }
			set { this.dcuReadRetry = value; }
		}

		/// <summary>
		/// Gets or sets the dcu disconnect delay.
		/// </summary>
		public int? DcuDisconnectDelay
		{
			get { return this.dcuDisconnectDelay; }
			set { this.dcuDisconnectDelay = value; }
		}

		/// <summary>
		/// Gets or sets the dcu communication fail restart.
		/// </summary>
		public int? DcuCommunicationFailRestart
		{
			get { return this.dcuCommunicationFailRestart; }
			set { this.dcuCommunicationFailRestart = value; }
		}

		/// <summary>
		/// Gets or sets the avery hardoll com port.
		/// </summary>
		public string AveryHardollComPort
		{
			get { return this.averyHardollComPort; }
			set { this.averyHardollComPort = value; }
		}

		/// <summary>
		/// Gets or sets the avery hardoll meter id.
		/// </summary>
		public string AveryHardollMeterId
		{
			get { return this.averyHardollMeterId; }
			set { this.averyHardollMeterId = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether vto enabled.
		/// </summary>
		public bool VtoEnabled
		{
			get { return this.vtoEnabled; }
			set { this.vtoEnabled = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether enable in op gauges.
		/// </summary>
		public bool EnabledInOpGauges
		{
			get { return this.enabledInOpGauges; }
			set { this.enabledInOpGauges = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether use dispensing vehicle gse trans.
		/// </summary>
		public bool UseDispensingVehicleGseTrans
		{
			get { return this.useDispensingVehicleGseTrans; }
			set { this.useDispensingVehicleGseTrans = value; }
		}

		/// <summary>
		/// Gets or sets the gse wait msec for get meter.
		/// </summary>
		public int? GseWaitMsecForGetMeter
		{
			get { return this.gseWaitMsecForGetMeter; }
			set { this.gseWaitMsecForGetMeter = value; }
		}

		/// <summary>
		/// Gets or sets the gse inactive logout minutes.
		/// </summary>
		public int? GseInactiveLogoutMinutes
		{
			get { return this.gseInactiveLogoutMinutes; }
			set { this.gseInactiveLogoutMinutes = value; }
		}

		/// <summary>
		/// Gets or sets the gse inactive timeout.
		/// </summary>
		public int? GseInactiveTimeout
		{
			get { return this.gseInactiveTimeout; }
			set { this.gseInactiveTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the barcode invalid warning seconds.
		/// </summary>
		public int? BarcodeInvalidWarningSeconds 
		{
			get { return this.barcodeInvalidWarningSeconds; }
			set { this.barcodeInvalidWarningSeconds = value; }
		}

		/// <summary>
		/// Gets or sets the de ice blend default.
		/// </summary>
		public double? DeIceBlendDefault
		{
			get { return this.deIceBlendDefault; }
			set { this.deIceBlendDefault = value; }
		}

		/// <summary>
		/// Gets or sets the communication timeout seconds.
		/// </summary>
		public int? CommunicationTimeoutSeconds 
		{
			get { return this.communicationTimeoutSeconds; }
			set { this.communicationTimeoutSeconds = value; }
		}

		/// <summary>
		/// Gets or sets the connection retries.
		/// </summary>
		public int? ConnectionRetries
		{
			get { return this.connectionRetries; }
			set { this.connectionRetries = value; }
		}

		/// <summary>
		/// Gets or sets the connection retry timeout.
		/// </summary>
		public int? ConnectionRetryTimeout
		{
			get { return this.connectionRetryTimeout; }
			set { this.connectionRetryTimeout = value; }
		}

		/// <summary>
		/// Gets or sets the connection type.
		/// </summary>
		public int? ConnectionType
		{
			get { return this.connectionType; }
			set { this.connectionType = value; }
		}

		/// <summary>
		/// Gets or sets the update interval.
		/// </summary>
		public int? UpdateInterval 
		{
			get { return this.updateInterval; }
			set { this.updateInterval = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether ping verification ip address.
		/// </summary>
		public bool PingVerificationIpAddress
		{
			get { return this.pingVerificationIpAddress; }
			set { this.pingVerificationIpAddress = value; }
		}

		/// <summary>
		/// Gets or sets the vehicle update interval.
		/// </summary>
		public int? VehicleUpdateInterval 
		{
			get { return this.vehicleUpdateInterval; }
			set { this.vehicleUpdateInterval = value; }
		}

		/// <summary>
		/// Gets or sets the presubmit delay.
		/// </summary>
		public int? PresubmitDelay 
		{
			get { return this.presubmitDelay; }
			set { this.presubmitDelay = value; }
		}

		/// <summary>
		/// Gets or sets the verification ip address.
		/// </summary>
		public string VerificationIpAddress
		{
			get { return this.verificationIpAddress; }
			set { this.verificationIpAddress = value; }
		}

		/// <summary>
		/// Gets or sets the created by.
		/// </summary>
		public string CreatedBy
		{
			get { return this.createdBy; }
			set { this.createdBy = value; }
		}

		/// <summary>
		/// Gets or sets the updated by.
		/// </summary>
		public string UpdatedBy
		{
			get { return this.updatedBy; }
			set { this.updatedBy = value; }
		}

		/// <summary>
		/// Gets or sets the created date.
		/// </summary>
		public DateTimeOffset? CreatedDate
		{
			get { return this.createdDate; }
			set { this.createdDate = value; }
		}

		/// <summary>
		/// Gets or sets the updated date.
		/// </summary>
		public DateTimeOffset? UpdatedDate
		{
			get { return this.updatedDate; }
			set { this.updatedDate = value; }
		}

		/// <summary>
		/// Gets or sets the analog input collection.
		/// </summary>
		public MobileDeviceProfileAnalogInputCollection AnalogInputCollection
		{
			get { return this.analogInputCollection; }
			set { this.analogInputCollection = value; }
		}

		/// <summary>
		/// Gets or sets the printer collection.
		/// </summary>
		public MobileDeviceProfilePrinterCollection PrinterCollection
		{
			get { return this.printerCollection; }
			set { this.printerCollection = value; }
		}

		/// <summary>
		/// Gets or sets the deleted printer collection.
		/// </summary>
		public List<MobileDeviceProfilePrinter> DeletedPrinterCollection
		{
			get { return this.deletedPrinterCollection; }
			set { this.deletedPrinterCollection = value; }
		}

		/// <summary>
		/// Gets or sets the profile to Mobile device map collection.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMapCollection AssignedMobileDeviceCollection
		{
			get { return this.assignedMobileDeviceCollection; }
			set { this.assignedMobileDeviceCollection = value; }
		}

		/// <summary>
		/// Gets or sets the unassigned Mobile device map collection.
		/// </summary>
		public List<MobileDeviceProfileToMobileDeviceMapClass> RemoveMobileDeviceMapCollection
		{
			get { return this.removeMobileDeviceMapCollection; }
			set { this.removeMobileDeviceMapCollection = value; }
		}

		/// <summary>
		/// Gets or sets the unassign mobile device collection.
		/// </summary>
		public MobileDeviceProfileToMobileDeviceMapCollection UnassignMobileDeviceCollection
		{
			get { return this.unassignedMobileDeviceCollection; }
			set { this.unassignedMobileDeviceCollection = value; }
		}

		/// <summary>
		/// Gets or sets the deleted analog input collection.
		/// </summary>
		public List<MobileDeviceProfileAnalogInput> DeletedAnalogInputCollection
		{
			get { return this.deletedAnalogInputCollection; }
			set { this.deletedAnalogInputCollection = value; }
		}

		/// <summary>
		/// Gets the entity type.
		/// </summary>
		public override ENTITY_TYPE EntityType
		{
			get { return ENTITY_TYPE.MOBILE_DEVICE_PROFILE; }
		}

		/// <summary>
		/// Gets the parent entity type.  Mobile Device Profile does not have an parent
		/// entity.
		/// </summary>
		public ENTITY_TYPE ParentEntityType
		{
			get { return ENTITY_TYPE.NONE; }
		}
		#endregion

		#region Public methods
		/// <summary>
		/// This method loads the retrieved data from the database and loads the data into the
		/// object.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void Load ( DataSet dataSet )
		{
			if ( ( dataSet != null ) && ( dataSet.Tables.Count > 0 ) )
			{
				DataTable table = dataSet.Tables[0];

				if ( ( table != null ) && ( table.Rows != null ) && ( table.Rows.Count > 0 ) )
				{
					DataRow row = table.Rows[0];
					this.LoadRow ( row );

					// Serialize the dataset to be used later for comparison.
					this.SerializeData(dataSet);
				}
			}
		}

		/// <summary>
		/// This method will load a single record only.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void LoadSingle(DataSet dataSet)
		{
			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];
					this.LoadRow(row);

					// Serialize the dataset to be used later for comparison.
					this.SerializeData(dataSet);
				}
			}
		}

		/// <summary>
		/// This method will load the Default Profile Constraint dataset and return either
		/// a true or false. If there is already an existing default profile define for the
		/// site or assigned site, then true is returned.  Otherwise, false is returned.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool LoadCheckDefaultProfileConstraint(DataSet dataSet)
		{
			bool defaultProfileExist = false;

			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];

					if ( row.IsNull("ProfileDefaultCount") == false )
					{
						int defaultProfileCount = (int) row["ProfileDefaultCount"];
						defaultProfileExist = defaultProfileCount > 0;
					}
				}
			}

			return defaultProfileExist;
		}

		/// <summary>
		/// This method will load the Profile ID Uniqueness dataset and return either
		/// a true (exists) or false. If there is already a profile ID that matches the ID
		/// in the current site or if it has been assigned down, then true is returned.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		/// <returns>
		/// The System.Boolean.
		/// </returns>
		public bool LoadProfileIdCheckUniqueness(DataSet dataSet)
		{
			bool profileIdExists = false;

			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				DataTable table = dataSet.Tables[0];

				if ( (table != null) && (table.Rows != null) && (table.Rows.Count > 0) )
				{
					DataRow row = table.Rows[0];

					if ( row.IsNull("ProfileIDCount") == false )
					{
						int profileIdCount = (int) row["ProfileIDCount"];
						profileIdExists = profileIdCount > 0;
					}
				}
			}

			return profileIdExists;
		}

		/// <summary>
		/// This method initialize the object to its initial state.
		/// </summary>
		public void Reset ( )
		{
			this.mobileDeviceProfileGuid			= Guid.Empty;
			this.siteGuid							= Guid.Empty;
			this.profileId							= string.Empty;
			this.description						= string.Empty;
			this.confirmFuelCaps					= false;
			this.showProductScreen					= false;
			this.generateTicketNumber				= false;
			this.showOperatorFieldInFlightList		= false;
			this.useDefaultPrinter					= false;
			this.defaultPrinter						= string.Empty;
			this.adminPassword						= null;
			this.shutdownHotKey						= string.Empty;
			this.printerComPort						= string.Empty;
			this.searchType							= null;
			this.loggingOption						= false;
			this.allowableFailedLoginAttempts		= null;
			this.fuelDistributionPrecision			= null;
			this.makeDefaultProfile					= false;
			this.vehicleId							= string.Empty;
			this.monitorScreenTransitionTiming		= false;
			this.bypassFsrCheckOnScreenTrans		= false;
			this.showFuelUpdateCheckStatusWin		= false;
			this.rtdTemperatureRangeMax				= null;
			this.rtdTemperatureRangeMin				= null;
			this.defaultTemperature					= null;
			this.strictUserValidation				= false;
			this.verifyFuelingEquipment				= false;
			this.allowEditRequiredFuelLoad			= false;
			this.allowBackAfterArrivalScreen		= false;
			this.allowBackAfterTicketPrinted		= false;
			this.requirePrint						= false;
			this.totalFuelLoadCheck					= false;
			this.volumetricThresholdValidation		= false;
			this.validateShipNumber					= false;
			this.allowVtoModification				= false;
			this.allowFlightGateModification		= false;
			this.tankPositionBalanceVerification	= null;
			this.tankPositionBalancePercentage		= null;
			this.overrideWingBalancePercentVar		= false;
			this.bypassDistributionTolerance		= false;
			this.vehicleIdCheck						= false;
			this.gseFuelMustMatch					= false;
			this.allowManualMeter					= false;
			this.useValidLogicGaTrans				= false;
			this.allowShipNumberModification		= false;
			this.allowAircraftTypeModification		= false;
			this.allowDestinationModification		= false;
			this.ticketPrinting						= null;
			this.aircraftTypeVerification			= null;
			this.destination						= null;
			this.gate								= null;
			this.shipNumber							= null;
			this.meterTotal							= null;
			this.volumePumped						= null;
			this.tankCapacity						= null;
			this.eaStrictUserValidation				= false;
			this.eaVerifyFuelingEquipment			= false;
			this.eaAllowEditOfRequiredFuelLoad		= false;
			this.eaAllowBackAfterArrivalScreen		= false;
			this.eaAllowBackAfterTicketPrinted		= false;
			this.eaRequirePrint						= false;
			this.eaTotalFuelLoad					= false;
			this.eaVolumetricThresholdValidation	= false;
			this.eaValidateShipNumber				= false;
			this.eaAllowVtoModification				= false;
			this.eaAllowFlightGateModification		= false;
			this.eaTankDiffPercentage				= false;
			this.eaWingBalancePercentage			= false;
			this.eaBypassDistributionTolerance		= false;
			this.eaVehicleIdCheck					= false;
			this.eaGseFuelMustMatch					= false;
			this.eaAllowManualMeter					= false;
			this.eaUseValidationLogicGaTrans		= false;
			this.eaAllowShipNumberModification		= false;
			this.eaAllowAircraftTypeModification	= false;
			this.eaAllowDestinationModification		= false;
			this.eaDestination						= false;
			this.eaTicketPrinting					= false;
			this.eaAircraftType						= false;
			this.eaShipNumber						= false;
			this.eaGateNumber						= false;
			this.eaMeterTotal						= false;
			this.eaVolumePumped						= false;
			this.eaTankCapacity						= false;
			this.equipmentType						= null;
			this.foreignKeyToMapEquipment			= Guid.Empty;
			this.issueTransaction					= Guid.Empty;
			this.defuelTransaction					= Guid.Empty;
			this.rotationTransaction				= Guid.Empty;
			this.meterCloseout						= Guid.Empty;
			this.deIceTransaction					= Guid.Empty;
			this.gseTransaction						= Guid.Empty;
			this.manualConsumer						= Guid.Empty;
			this.manualVendor						= Guid.Empty;
			this.closeoutOwner						= Guid.Empty;
			this.closeoutVendor						= Guid.Empty;
			this.manualShipper						= Guid.Empty;
			this.manualManager						= Guid.Empty;
			this.manualSupplier						= Guid.Empty;
			this.manualBillTo						= Guid.Empty;
			this.closeoutConsumer					= Guid.Empty;
			this.manualProduct						= Guid.Empty;
			this.manualStationId					= 0;
			this.inhibitOverridingTemperature		= false;
			this.manualTemperature					= null;
			this.manualDensity						= null;
			this.hasDCU								= false;
			this.bluetoothDCU						= false;
			this.logDCUActions						= false;
			this.hasAveryHardoll					= false;
			this.dcuComPort							= string.Empty;
			this.dcuReadRetry						= null;
			this.dcuDisconnectDelay					= null;
			this.dcuCommunicationFailRestart		= null;
			this.averyHardollComPort				= string.Empty;
			this.averyHardollMeterId				= string.Empty;
			this.vtoEnabled							= false;
			this.enabledInOpGauges					= false;
			this.useDispensingVehicleGseTrans		= false;
			this.gseWaitMsecForGetMeter				= null;
			this.gseInactiveLogoutMinutes			= null;
			this.gseInactiveTimeout					= null;
			this.barcodeInvalidWarningSeconds		= null;
			this.deIceBlendDefault					= 50.0;
			this.communicationTimeoutSeconds		= null;
			this.connectionRetries					= null;
			this.connectionRetryTimeout				= null;
			this.connectionType						= null;
			this.updateInterval						= null;
			this.pingVerificationIpAddress			= false;
			this.presubmitDelay						= null;
			this.verificationIpAddress				= string.Empty;
			this.createdBy							= string.Empty;
			this.updatedBy							= string.Empty;
			this.createdDate						= null;
			this.updatedDate						= null;
			this.analogInputCollection				= new MobileDeviceProfileAnalogInputCollection();
			this.printerCollection					= new MobileDeviceProfilePrinterCollection();
			this.deletedPrinterCollection			= new List<MobileDeviceProfilePrinter>();
			this.assignedMobileDeviceCollection		= new MobileDeviceProfileToMobileDeviceMapCollection();
			this.deletedAnalogInputCollection		= new List<MobileDeviceProfileAnalogInput>();
			this.removeMobileDeviceMapCollection	= new List<MobileDeviceProfileToMobileDeviceMapClass>();
			this.unassignedMobileDeviceCollection	= new MobileDeviceProfileToMobileDeviceMapCollection();
		}
		#endregion

		#region SQL Statements
		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to check that there is only one profile that is the default
		/// profile for a site or an assigned site.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void DefaultProfileConstraintSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = string.Empty;

			if ( this.makeDefaultProfile )
			{
				string select	= "SELECT COUNT(*) AS ProfileDefaultCount ";
				string from		= "FROM tblMobileDeviceProfile WITH (NOLOCK) ";
				string where	= "WHERE MakeDefaultProfile = 1 AND "
									+ AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");

				sqlCommand.CommandText = select + from + where;
			}
		}

		/// <summary>
		/// This method will populate the sql command with the SQL text to
		/// check for the profile ID being unique for the current site and
		/// if it had been entity assigned down.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void CheckForProfileIdUniquenessSql(SqlCommand sqlCommand, SecurityClass security)
		{
			sqlCommand.CommandText = string.Empty;

			string select = "SELECT SUM(ProfileIDCount) AS ProfileIDCount ";
			string from = "FROM (SELECT COUNT(*) AS ProfileIDCount " +
				          "FROM tblMobileDeviceProfile WITH (NOLOCK) " +
						  "WHERE ProfileID = @ProfileID AND SiteGuid = @SiteGuid " +
						  "UNION " +
						  "SELECT COUNT(*) AS ProfileIDCount " +
						  "FROM map.tblEntityMobileDeviceProfileToSite em INNER JOIN tblMobileDeviceProfile p " +
						  "ON em.MobileDeviceProfileGuid = p.MobileDeviceProfileGuid " +
						  "WHERE em.SiteGuid = @SiteGuid AND p.ProfileID = @ProfileID " +
						  ") AS ProfileIDCountTable ";

			sqlCommand.CommandText = select + from;

			var parm = new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = this.ProfileId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@SiteGuid", SqlDbType.UniqueIdentifier) { Value = security.SiteGuid };
			sqlCommand.Parameters.Add(parm);
		}

		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to retrieve all profiles.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void EnumerateAllSql ( SqlCommand sqlCommand, SecurityClass security )
		{
			string select = "SELECT tblTemp.DeviceCount, tblMobileDeviceProfile.* ";
			string from = "FROM tblMobileDeviceProfile WITH ( NOLOCK ) LEFT OUTER JOIN "
			              + "(SELECT MobileDeviceProfileGuid, COUNT(*) AS DeviceCount "
			              + "FROM map.tblMobileDeviceProfileToMobileDevice "
						  + "GROUP BY MobileDeviceProfileGuid) AS tblTemp ON tblMobileDeviceProfile.MobileDeviceProfileGuid = tblTemp.MobileDeviceProfileGuid ";
			string where  = "WHERE " + AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");

			sqlCommand.CommandText = select + from + where;
		}

		/// <summary>
		/// This method will populate the sql command with the SQL text
		/// to retrieve all profiles based on the find filter.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="findFilter">
		/// The find filter.
		/// </param>
		public void EnumerateByFindFilterSql(SqlCommand sqlCommand, SecurityClass security, string findFilter)
		{
			string select = "SELECT tblTemp.DeviceCount, tblMobileDeviceProfile.* ";
			string from = "FROM tblMobileDeviceProfile WITH ( NOLOCK ) LEFT OUTER JOIN "
						  + "(SELECT MobileDeviceProfileGuid, COUNT(*) AS DeviceCount "
						  + "FROM map.tblMobileDeviceProfileToMobileDevice "
						  + "GROUP BY MobileDeviceProfileGuid) AS tblTemp ON tblMobileDeviceProfile.MobileDeviceProfileGuid = tblTemp.MobileDeviceProfileGuid ";

			if ( string.IsNullOrEmpty(findFilter) == false )
			{
				string where2 = "WHERE (ProfileID LIKE (@FindFilter1) OR Description LIKE (@FindFilter2)) AND " +
								AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");

				sqlCommand.CommandText = select + from + where2;

				string idFindFilter = findFilter;
				string descFindFilter = findFilter;

				if (findFilter.Length > 50)
				{
					idFindFilter = findFilter.Substring(0, 50);
				}

				if (findFilter.Length > 200)
				{
					descFindFilter = findFilter.Substring(0, 200);
				}

				idFindFilter = "%" + idFindFilter + "%";
				descFindFilter = "%" + descFindFilter + "%";

				var parm = new SqlParameter("@FindFilter1", SqlDbType.NVarChar, 50) { Value = idFindFilter };
				sqlCommand.Parameters.Add(parm);

				parm = new SqlParameter("@FindFilter2", SqlDbType.NVarChar, 200) { Value = descFindFilter };
				sqlCommand.Parameters.Add(parm);
			}
			else
			{
				string where1 = "WHERE " + AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");
				sqlCommand.CommandText = select + from + where1;
			}
		}


		/// <summary>
		/// This method will populate the sql command to retrieve the GUID based
		/// on a profile ID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void GetGuidSql ( SqlCommand sqlCommand, SecurityClass security )
		{
			string select = "SELECT MobileDeviceProfileGUID ";
			string from   = "FROM tblMobileDeviceProfile WITH ( NOLOCK ) ";
			string where  = "WHERE ProfileID = @ProfileID AND " +
							AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");

			sqlCommand.CommandText = select + from + where;

			SqlParameter parm = new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = this.profileId };
			sqlCommand.Parameters.Add ( parm );
		}

		/// <summary>
		/// This method will populate the sql command to retrieve mobile device profile data
		/// based on the profile ID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		public void GetByProfileIdSql ( SqlCommand sqlCommand, SecurityClass security )
		{
			string select = "SELECT * ";
			string from   = "FROM tblMobileDeviceProfile WITH ( NOLOCK ) ";
			string where = "WHERE ProfileID = @ProfileID AND " +
			               AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");

			sqlCommand.CommandText = select + from + where;

			SqlParameter parm = new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = this.profileId };
			sqlCommand.Parameters.Add ( parm );
		}

		/// <summary>
		/// This method will populate the sql command to retrieve mobile device profile data
		/// based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="security">
		/// The security.
		/// </param>
		/// <param name="withEntityClause">
		/// The with Entity Clause.
		/// </param>
		public void GetByProfileGuidSql(SqlCommand sqlCommand, SecurityClass security, bool withEntityClause)
		{
			string select = "SELECT * ";
			string from   = "FROM tblMobileDeviceProfile WITH ( NOLOCK ) ";
			string where  = "WHERE MobileDeviceProfileGUID = @MobileDeviceProfileGUID ";

			if ( withEntityClause )
			{
				where = where + " AND " + this.AppendSiteWhereClause(sqlCommand, security, "tblMobileDeviceProfile", "MobileDeviceProfileGuid");
			}

			sqlCommand.CommandText = select + from + where;

			var parm = new SqlParameter ( "@MobileDeviceProfileGUID", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add ( parm );
		}

		/// <summary>
		/// This method will populate the sql command to remove a mobile device profile record
		/// from the database based on the GUID.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void PurgeSql ( SqlCommand sqlCommand)
		{
			string select = "DELETE FROM  tblMobileDeviceProfile ";
			string where  = "WHERE MobileDeviceProfileGUID = @MobileDeviceProfileGUID ";

			sqlCommand.CommandText = select + where;

			var parm = new SqlParameter ( "@MobileDeviceProfileGUID", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add ( parm );
		}

		/// <summary>
		/// This method will populate the sql command with the insert data.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		public void InsertSql ( SqlCommand sqlCommand )
		{
			string insert = "INSERT INTO tblMobileDeviceProfile ( " +
							"MobileDeviceProfileGuid, " + 
							"SiteGuid,  " + 
							"ProfileID,  " + 
							"Description,  " + 
							"ConfirmFuelCaps,  " + 
							"ShowProductScreen,  " + 
							"GenerateTicketNumber,  " + 
							"ShowOperatorFieldInFlightList,  " + 
							"UseDefaultPrinter,  " + 
							"DefaultPrinter,  " + 
							"AdminPassword,  " + 
							"ShutdownHotKey,  " + 
							"PrinterCOMPort,  " + 
							"SearchType,  " + 
							"LoggingOption, " +
							"AllowableFailedLoginAttempts, " +
							"FuelDistributionPrecision, " +
							"MakeDefaultProfile, " +
							"VehicleID, " +
							"MonitorScreenTransitionTiming, " +
							"BypassFsrCheckOnScreenTrans, " +
							"ShowFuelUpdateCheckStatusWin, " +
							"RTDTemperatureRangeMax,  " + 
							"RTDTemperatureRangeMin,  " + 
							"DefaultTemperature, " +
							"StrictUserValidation,  " + 
							"VerifyFuelingEquipment,  " + 
							"AllowEditRequiredFuelLoad,  "  + 
							"AllowBackAfterArrivalScreen,  " + 
							"AllowBackAfterTicketPrinted,  " + 
							"RequirePrint, " +
							"TotalFuelLoadCheck, " +
							"VolumetricThresholdValidation, " +
							"ValidateShipNumber, " +
							"AllowVTOModification, " +
							"AllowFlightGateModification, " +
							"TankPositionBalanceVerification,  " + 
							"TankPositionBalancePercentage, " +
							"OverrideWingBalancePercentVar,  " + 
							"BypassDistributionTolerance,  " + 
							"VehicleIDCheck, " +
							"GSEFuelMustMatch, " +
							"AllowManualMeter, " +
							"UseValidLogicGATrans, " +
							"AllowShipNumberModification, " +
							"AllowAircraftTypeModification, " +
							"AllowDestinationModification, " +
							"TicketPrinting,  " + 
							"AircraftTypeVerification,  " + 
							"Destination,  " + 
							"Gate, " +
							"ShipNumber,  " + 
							"MeterTotal,  " + 
							"VolumePumped,  " + 
							"TankCapacity,  " + 
							"EAStrictUserValidation, " +
							"EAVerifyFuelingEquipment, " +
							"EAAllowEditOfRequiredFuelLoad, " +
							"EAAllowBackAfterArrivalScreen, " +
							"EAAllowBackAfterTicketPrinted, " +
							"EARequirePrint, " +
							"EATotalFuelLoad, " +
							"EAVolumetricThresholdValidation, " +
							"EAValidateShipNumber, " +
							"EAAllowVtoModification, " +
							"EAAllowFlightGateModification, " +
							"EATankDiffPercentage, " +
							"EAWingBalancePercentage, " +
							"EABypassDistributionTolerance, " +
							"EAVehicleIDCheck, " +
							"EAGseFuelMustMatch, " +
							"EAAllowManualMeter, " +
							"EAUseValidationLogicGATrans, " +
							"EAAllowShipNumberModification, " +
							"EAAllowAircraftTypeModification, " +
							"EAAllowDestinationModification, " +
							"EADestination, " +
							"EATicketPrinting, " +
							"EAAircraftType, " +
							"EAShipNumber, " +
							"EAGateNumber, " +
							"EAMeterTotal, " +
							"EAVolumePumped, " +
							"EATankCapacity, " +
							"EquipmentType,  " + 
							"ForeignKeyToMapEquipment,  " + 
							"IssueTransaction,  " + 
							"DefuelTransaction,  "  + 
							"RotationTransaction,  " + 
							"MeterCloseout,  " + 
							"DeIceTransaction, " +
							"GSETransaction, " +
							"ManualConsumer, " + 
							"ManualVendor, " +
							"CloseoutOwner,  " + 
							"CloseoutVendor,  " + 
							"ManualShipper,  " + 
							"ManualManager,  " +
							"ManualSupplier,  " +
							"ManualBillTo,  " + 
							"CloseoutConsumer,  " + 
							"ManualProduct,  " + 
							"ManualStationID,  "  + 
							"InhibitOverridingTemperature,  " + 
							"ManualTemperature,  " + 
							"ManualDensity,  " + 
							"HasDCU,  " + 
							"BluetoothDCU,  " + 
							"LogDCUActions,  " + 
							"HasAveryHardoll,  " +  
							"DCUComPort,  " + 
							"DCUReadRetry,  " + 
							"DCUDisconnectDelay,  "  + 
							"DCUCommunicationFailRestart, " +
							"AveryHardollComPort,  " + 
							"AveryHardollMeterID, " +
							"VTOEnabled, " +
							"EnabledInOpGauges, " +
							"UseDispensingVehicleGSETrans, " +
							"GSEWaitMSecForGetMeter, " +
							"GSEInactiveLogoutMinutes, " +
							"GSEInactiveTimeout, " +
							"BarcodeInvalidWarningSeconds, " +
							"DeIceBlendDefault, " +
							"CommunicationTimeoutSeconds, " +
							"ConnectionRetries, " +
							"ConnectionRetryTimeout, " +
							"ConnectionType, " +
							"UpdateInterval, " +
							"PingVerificationIPAddress, " +
							"VehicleUpdateInterval, " +
							"PresubmitDelay, " +
							"VerificationIPAddress, " +
							"CreatedBy, " +
							"CreatedDate, " +
							"UpdatedBy, " +
							"UpdatedDate ) ";

			string insertValues = "VALUES ( " +
								"@MobileDeviceProfileGuid, " + 
								"@SiteGuid,  " +
								"@ProfileID,  " +
								"@Description,  " +
								"@ConfirmFuelCaps,  " +
								"@ShowProductScreen,  " +
								"@GenerateTicketNumber,  " +
								"@ShowOperatorFieldInFlightList,  " +
								"@UseDefaultPrinter,  " +
								"@DefaultPrinter,  " +
								"@AdminPassword,  " +
								"@ShutdownHotKey,  " +
								"@PrinterCOMPort,  " +
								"@SearchType,  " +
								"@LoggingOption, " +
								"@AllowableFailedLoginAttempts, " +
								"@FuelDistributionPrecision, " +
								"@MakeDefaultProfile, " +
								"@VehicleID, " +
								"@MonitorScreenTransitionTiming, " +
								"@BypassFsrCheckOnScreenTrans, " +
								"@ShowFuelUpdateCheckStatusWin, " +
								"@RTDTemperatureRangeMax,  " +
								"@RTDTemperatureRangeMin,  " +
								"@DefaultTemperature, " +
								"@StrictUserValidation,  " +
								"@VerifyFuelingEquipment,  " +
								"@AllowEditRequiredFuelLoad,  " +
								"@AllowBackAfterArrivalScreen,  " +
								"@AllowBackAfterTicketPrinted,  " +
								"@RequirePrint, " +
								"@TotalFuelLoadCheck, " +
								"@VolumetricThresholdValidation, " +
								"@ValidateShipNumber, " +
								"@AllowVTOModification, " +
								"@AllowFlightGateModification, " +
								"@TankPositionBalanceVerification,  " +
								"@TankPositionBalancePercentage, " +
								"@OverrideWingBalancePercentVar,  " +
								"@BypassDistributionTolerance,  " +
								"@VehicleIDCheck, " +
								"@GSEFuelMustMatch, " +
								"@AllowManualMeter, " +
								"@UseValidLogicGATrans, " +
								"@AllowShipNumberModification, " +
								"@AllowAircraftTypeModification, " +
								"@AllowDestinationModification, " +
								"@TicketPrinting,  " +
								"@AircraftTypeVerification,  " +
								"@Destination,  " +
								"@Gate, " +
								"@ShipNumber,  " +
								"@MeterTotal,  " +
								"@VolumePumped,  " +
								"@TankCapacity,  " +
								"@EAStrictUserValidation, " +
								"@EAVerifyFuelingEquipment, " +
								"@EAAllowEditOfRequiredFuelLoad, " +
								"@EAAllowBackAfterArrivalScreen, " +
								"@EAAllowBackAfterTicketPrinted, " +
								"@EARequirePrint, " +
								"@EATotalFuelLoad, " +
								"@EAVolumetricThresholdValidation, " +
								"@EAValidateShipNumber, " +
								"@EAAllowVtoModification, " +
								"@EAAllowFlightGateModification, " +
								"@EATankDiffPercentage, " +
								"@EAWingBalancePercentage, " +
								"@EABypassDistributionTolerance, " +
								"@EAVehicleIDCheck, " +
								"@EAGseFuelMustMatch, " +
								"@EAAllowManualMeter, " +
								"@EAUseValidationLogicGATrans, " +
								"@EAAllowShipNumberModification, " +
								"@EAAllowAircraftTypeModification, " +
								"@EAAllowDestinationModification, " +
								"@EADestination, " +
								"@EATicketPrinting, " +
								"@EAAircraftType, " +
								"@EAShipNumber, " +
								"@EAGateNumber, " +
								"@EAMeterTotal, " +
								"@EAVolumePumped, " +
								"@EATankCapacity, " +
								"@EquipmentType,  " +
								"@ForeignKeyToMapEquipment,  " +
								"@IssueTransaction,  " +
								"@DefuelTransaction,  " +
								"@RotationTransaction,  " +
								"@MeterCloseout,  " +
								"@DeIceTransaction, " +
								"@GSETransaction, " +
								"@ManualConsumer,  " +
								"@ManualVendor, " +
								"@CloseoutOwner,  " +
								"@CloseoutVendor,  " +
								"@ManualShipper,  " +
								"@ManualManager,  " +
								"@ManualSupplier,  " +
								"@ManualBillTo,  " +
								"@CloseoutConsumer,  " +
								"@ManualProduct,  " +
								"@ManualStationID,  " +
								"@InhibitOverridingTemperature,  " +
								"@ManualTemperature,  " +
								"@ManualDensity,  " +
								"@HasDCU,  " +
								"@BluetoothDCU,  " +
								"@LogDCUActions,  " +
								"@HasAveryHardoll,  " +
								"@DCUComPort,  " +
								"@DCUReadRetry,  " +
								"@DCUDisconnectDelay,  " +
								"@DCUCommunicationFailRestart, " +
								"@AveryHardollComPort,  " +
								"@AveryHardollMeterID, " +
								"@VTOEnabled, " +
								"@EnabledInOpGauges, " +
								"@UseDispensingVehicleGSETrans, " +
								"@GSEWaitMSecForGetMeter, " +
								"@GSEInactiveLogoutMinutes, " +
								"@GSEInactiveTimeout, " +
								"@BarcodeInvalidWarningSeconds, " +
								"@DeIceBlendDefault, " +
								"@CommunicationTimeoutSeconds, " +
								"@ConnectionRetries, " +
								"@ConnectionRetryTimeout, " +
								"@ConnectionType, " +
								"@UpdateInterval, " +
								"@PingVerificationIPAddress, " +
								"@VehicleUpdateInterval, " +
								"@PresubmitDelay, " +
								"@VerificationIPAddress, " +
								"@CreatedBy, " +
								"@CreatedDate, " +
								"@UpdatedBy, " +
								"@UpdatedDate ) ";

			sqlCommand.CommandText = insert + insertValues;

			SqlParameter parm = new SqlParameter ( "@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier ) { Value = this.mobileDeviceProfileGuid };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@SiteGuid", SqlDbType.UniqueIdentifier ) { Value = this.siteGuid };
			sqlCommand.Parameters.Add ( parm );

			parm = string.IsNullOrEmpty(this.profileId) ? new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = this.profileId };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.description) ? new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = DBNull.Value } : new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = this.description };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@ConfirmFuelCaps", SqlDbType.Bit ) { Value = this.ConfirmFuelCaps ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ShowProductScreen", SqlDbType.Bit ) { Value = this.showProductScreen ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@GenerateTicketNumber", SqlDbType.Bit ) { Value = this.generateTicketNumber ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ShowOperatorFieldInFlightList", SqlDbType.Bit ) { Value = this.showOperatorFieldInFlightList ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@UseDefaultPrinter", SqlDbType.Bit ) { Value = this.useDefaultPrinter ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = string.IsNullOrEmpty(this.defaultPrinter) ? new SqlParameter("@DefaultPrinter", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@DefaultPrinter", SqlDbType.NVarChar, 50) { Value = this.defaultPrinter };
			sqlCommand.Parameters.Add(parm);

			parm = this.adminPassword == null ? new SqlParameter("@AdminPassword", SqlDbType.VarBinary) { Value = DBNull.Value } : new SqlParameter("@AdminPassword", SqlDbType.VarBinary) { Value = this.adminPassword };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.shutdownHotKey) ? new SqlParameter("@ShutdownHotKey", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@ShutdownHotKey", SqlDbType.NVarChar, 50) { Value = this.shutdownHotKey };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.printerComPort) ? new SqlParameter("@PrinterCOMPort", SqlDbType.NVarChar, 10) { Value = DBNull.Value } : new SqlParameter("@PrinterCOMPort", SqlDbType.NVarChar, 10) { Value = this.printerComPort };
			sqlCommand.Parameters.Add(parm);

			parm = this.searchType == null ? new SqlParameter("@SearchType", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@SearchType", SqlDbType.Int) { Value = this.searchType.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@LoggingOption", SqlDbType.Bit ) { Value = this.loggingOption ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = this.allowableFailedLoginAttempts == null ? new SqlParameter("@AllowableFailedLoginAttempts", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@AllowableFailedLoginAttempts", SqlDbType.Int) { Value = this.allowableFailedLoginAttempts.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.fuelDistributionPrecision == null ? new SqlParameter("@FuelDistributionPrecision", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@FuelDistributionPrecision", SqlDbType.Int) { Value = this.fuelDistributionPrecision.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@MakeDefaultProfile", SqlDbType.Bit) { Value = this.makeDefaultProfile };
			sqlCommand.Parameters.Add ( parm );

			parm = string.IsNullOrEmpty(this.vehicleId) ? new SqlParameter("@VehicleID", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@VehicleID", SqlDbType.NVarChar, 50) { Value = this.vehicleId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@MonitorScreenTransitionTiming", SqlDbType.Bit ) { Value = this.monitorScreenTransitionTiming ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@BypassFsrCheckOnScreenTrans", SqlDbType.Bit ) { Value = this.bypassFsrCheckOnScreenTrans ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ShowFuelUpdateCheckStatusWin", SqlDbType.Bit ) { Value = this.showFuelUpdateCheckStatusWin ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = this.rtdTemperatureRangeMax == null ? new SqlParameter("@RTDTemperatureRangeMax", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@RTDTemperatureRangeMax", SqlDbType.Float) { Value = this.rtdTemperatureRangeMax.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.rtdTemperatureRangeMin == null ? new SqlParameter("@RTDTemperatureRangeMin", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@RTDTemperatureRangeMin", SqlDbType.Float) { Value = this.rtdTemperatureRangeMin.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.defaultTemperature == null ? new SqlParameter("@DefaultTemperature", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@DefaultTemperature", SqlDbType.Float) { Value = this.defaultTemperature.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@StrictUserValidation", SqlDbType.Bit ) { Value = this.strictUserValidation ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@VerifyFuelingEquipment", SqlDbType.Bit ) { Value = this.verifyFuelingEquipment ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowEditRequiredFuelLoad", SqlDbType.Bit ) { Value = this.allowEditRequiredFuelLoad ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowBackAfterArrivalScreen", SqlDbType.Bit ) { Value = this.allowBackAfterArrivalScreen ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowBackAfterTicketPrinted", SqlDbType.Bit ) { Value = this.allowBackAfterTicketPrinted ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@RequirePrint", SqlDbType.Bit ) { Value = this.requirePrint ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@TotalFuelLoadCheck", SqlDbType.Bit ) { Value = this.totalFuelLoadCheck ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@VolumetricThresholdValidation", SqlDbType.Bit ) { Value = this.volumetricThresholdValidation ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ValidateShipNumber", SqlDbType.Bit ) { Value = this.validateShipNumber ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowVTOModification", SqlDbType.Bit ) { Value = this.allowVtoModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowFlightGateModification", SqlDbType.Bit ) { Value = this.allowFlightGateModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = this.tankPositionBalanceVerification == null ? new SqlParameter("@TankPositionBalanceVerification", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@TankPositionBalanceVerification", SqlDbType.Int) { Value = this.tankPositionBalanceVerification.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.TankPositionBalancePercentage == null ? new SqlParameter("@TankPositionBalancePercentage", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@TankPositionBalancePercentage", SqlDbType.Float) { Value = this.tankPositionBalancePercentage.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@OverrideWingBalancePercentVar", SqlDbType.Bit ) { Value = this.overrideWingBalancePercentVar ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@BypassDistributionTolerance", SqlDbType.Bit ) { Value = this.bypassDistributionTolerance ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@VehicleIDCheck", SqlDbType.Bit ) { Value = this.vehicleIdCheck ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@GSEFuelMustMatch", SqlDbType.Bit ) { Value = this.gseFuelMustMatch ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowManualMeter", SqlDbType.Bit ) { Value = this.allowManualMeter ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@UseValidLogicGATrans", SqlDbType.Bit ) { Value = this.useValidLogicGaTrans ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowShipNumberModification", SqlDbType.Bit ) { Value = this.allowShipNumberModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowAircraftTypeModification", SqlDbType.Bit ) { Value = this.allowAircraftTypeModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@AllowDestinationModification", SqlDbType.Bit ) { Value = this.allowDestinationModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = this.ticketPrinting == null ? new SqlParameter("@TicketPrinting", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@TicketPrinting", SqlDbType.Int) { Value = this.ticketPrinting.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.aircraftTypeVerification == null ? new SqlParameter("@AircraftTypeVerification", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@AircraftTypeVerification", SqlDbType.Int) { Value = this.aircraftTypeVerification.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.destination == null ? new SqlParameter("@Destination", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@Destination", SqlDbType.Int) { Value = this.destination.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.gate == null ? new SqlParameter("@Gate", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@Gate", SqlDbType.Int) { Value = this.gate.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.shipNumber == null ? new SqlParameter("@ShipNumber", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ShipNumber", SqlDbType.Int) { Value = this.shipNumber.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.meterTotal == null ? new SqlParameter("@MeterTotal", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@MeterTotal", SqlDbType.Int) { Value = this.meterTotal.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.volumePumped == null ? new SqlParameter("@VolumePumped", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@VolumePumped", SqlDbType.Int) { Value = this.volumePumped.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.tankCapacity == null ? new SqlParameter("@TankCapacity", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@TankCapacity", SqlDbType.Int) { Value = this.tankCapacity.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@EAStrictUserValidation", SqlDbType.Bit ) { Value = this.eaStrictUserValidation ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAVerifyFuelingEquipment", SqlDbType.Bit ) { Value = this.eaVerifyFuelingEquipment ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowEditOfRequiredFuelLoad", SqlDbType.Bit ) { Value = this.eaAllowEditOfRequiredFuelLoad ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowBackAfterArrivalScreen", SqlDbType.Bit ) { Value = this.eaAllowBackAfterArrivalScreen ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowBackAfterTicketPrinted", SqlDbType.Bit ) { Value = this.eaAllowBackAfterTicketPrinted ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EARequirePrint", SqlDbType.Bit ) { Value = this.eaRequirePrint ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EATotalFuelLoad", SqlDbType.Bit ) { Value = this.eaTotalFuelLoad ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAVolumetricThresholdValidation", SqlDbType.Bit ) { Value = this.eaVolumetricThresholdValidation ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAValidateShipNumber", SqlDbType.Bit ) { Value = this.eaValidateShipNumber ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowVtoModification", SqlDbType.Bit ) { Value = this.eaAllowVtoModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowFlightGateModification", SqlDbType.Bit ) { Value = this.eaAllowFlightGateModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EATankDiffPercentage", SqlDbType.Bit ) { Value = this.eaTankDiffPercentage ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAWingBalancePercentage", SqlDbType.Bit ) { Value = this.eaWingBalancePercentage ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EABypassDistributionTolerance", SqlDbType.Bit ) { Value = this.eaBypassDistributionTolerance ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAVehicleIDCheck", SqlDbType.Bit ) { Value = this.eaVehicleIdCheck ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAGseFuelMustMatch", SqlDbType.Bit ) { Value = this.eaGseFuelMustMatch ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowManualMeter", SqlDbType.Bit ) { Value = this.eaAllowManualMeter ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAUseValidationLogicGATrans", SqlDbType.Bit ) { Value = this.eaUseValidationLogicGaTrans ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowShipNumberModification", SqlDbType.Bit ) { Value = this.eaAllowShipNumberModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowAircraftTypeModification", SqlDbType.Bit ) { Value = this.eaAllowAircraftTypeModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAllowDestinationModification", SqlDbType.Bit ) { Value = this.eaAllowDestinationModification ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter("@EADestination", SqlDbType.Bit) { Value = this.eaDestination ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@EATicketPrinting", SqlDbType.Bit) { Value = this.eaTicketPrinting ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAAircraftType", SqlDbType.Bit ) { Value = this.eaAircraftType ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAShipNumber", SqlDbType.Bit ) { Value = this.eaShipNumber ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAGateNumber", SqlDbType.Bit ) { Value = this.eaGateNumber ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAMeterTotal", SqlDbType.Bit ) { Value = this.eaMeterTotal ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EAVolumePumped", SqlDbType.Bit ) { Value = this.eaVolumePumped ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@EATankCapacity", SqlDbType.Bit ) { Value = this.eaTankCapacity ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = this.equipmentType == null ? new SqlParameter("@EquipmentType", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@EquipmentType", SqlDbType.Int) { Value = this.equipmentType.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@ForeignKeyToMapEquipment", SqlDbType.UniqueIdentifier ) { Value = this.foreignKeyToMapEquipment };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@IssueTransaction", SqlDbType.UniqueIdentifier ) { Value = this.issueTransaction };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@DefuelTransaction", SqlDbType.UniqueIdentifier ) { Value = this.defuelTransaction };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@RotationTransaction", SqlDbType.UniqueIdentifier ) { Value = this.rotationTransaction };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@MeterCloseout", SqlDbType.UniqueIdentifier ) { Value = this.meterCloseout };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter("@DeIceTransaction", SqlDbType.UniqueIdentifier) { Value = this.deIceTransaction };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@GSETransaction", SqlDbType.UniqueIdentifier) { Value = this.gseTransaction };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@ManualConsumer", SqlDbType.UniqueIdentifier) { Value = this.manualConsumer };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter("@ManualVendor", SqlDbType.UniqueIdentifier) { Value = this.manualVendor };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@CloseoutOwner", SqlDbType.UniqueIdentifier ) { Value = this.closeoutOwner };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@CloseoutVendor", SqlDbType.UniqueIdentifier ) { Value = this.closeoutVendor };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ManualShipper", SqlDbType.UniqueIdentifier ) { Value = this.manualShipper };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter("@ManualManager", SqlDbType.UniqueIdentifier) { Value = this.manualManager };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter("@ManualSupplier", SqlDbType.UniqueIdentifier) { Value = this.manualSupplier };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter("@ManualBillTo", SqlDbType.UniqueIdentifier) { Value = this.manualBillTo };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@CloseoutConsumer", SqlDbType.UniqueIdentifier ) { Value = this.closeoutConsumer };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ManualProduct", SqlDbType.UniqueIdentifier ) { Value = this.manualProduct };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@ManualStationID", SqlDbType.Int ) { Value = this.manualStationId };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@InhibitOverridingTemperature", SqlDbType.Bit ) { Value = this.inhibitOverridingTemperature ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = this.manualTemperature == null ? new SqlParameter("@ManualTemperature", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@ManualTemperature", SqlDbType.Float) { Value = this.manualTemperature.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.manualDensity == null ? new SqlParameter("@ManualDensity", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@ManualDensity", SqlDbType.Float) { Value = this.manualDensity.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter ( "@HasDCU", SqlDbType.Bit ) { Value = this.hasDCU ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@BluetoothDCU", SqlDbType.Bit ) { Value = this.bluetoothDCU ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@LogDCUActions", SqlDbType.Bit ) { Value = this.logDCUActions ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@HasAveryHardoll", SqlDbType.Bit ) { Value = this.hasAveryHardoll ? 1 : 0 };
			sqlCommand.Parameters.Add ( parm );

			parm = string.IsNullOrEmpty(this.dcuComPort) ? new SqlParameter("@DCUComPort", SqlDbType.NVarChar, 4) { Value = DBNull.Value } : new SqlParameter("@DCUComPort", SqlDbType.NVarChar, 4) { Value = this.dcuComPort };
			sqlCommand.Parameters.Add(parm);

			parm = this.dcuReadRetry == null ? new SqlParameter("@DCUReadRetry", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@DCUReadRetry", SqlDbType.Int) { Value = this.dcuReadRetry.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.dcuDisconnectDelay == null ? new SqlParameter("@DCUDisconnectDelay", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@DCUDisconnectDelay", SqlDbType.Int) { Value = this.dcuDisconnectDelay.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.dcuCommunicationFailRestart == null ? new SqlParameter("@DCUCommunicationFailRestart", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@DCUCommunicationFailRestart", SqlDbType.Int) { Value = this.dcuCommunicationFailRestart.Value };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.averyHardollComPort) ? new SqlParameter("@AveryHardollComPort", SqlDbType.NVarChar, 4) { Value = DBNull.Value } : new SqlParameter("@AveryHardollComPort", SqlDbType.NVarChar, 4) { Value = this.averyHardollComPort };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.averyHardollMeterId) ? new SqlParameter("@AveryHardollMeterID", SqlDbType.NVarChar, 4) { Value = DBNull.Value } : new SqlParameter("@AveryHardollMeterID", SqlDbType.NVarChar, 4) { Value = this.averyHardollMeterId };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@VTOEnabled", SqlDbType.Bit) { Value = this.vtoEnabled ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@EnabledInOpGauges", SqlDbType.Bit) { Value = this.enabledInOpGauges ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@UseDispensingVehicleGSETrans", SqlDbType.Bit) { Value = this.useDispensingVehicleGseTrans ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = this.gseWaitMsecForGetMeter == null ? new SqlParameter("@GSEWaitMSecForGetMeter", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@GSEWaitMSecForGetMeter", SqlDbType.Int) { Value = this.gseWaitMsecForGetMeter.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.gseInactiveLogoutMinutes == null ? new SqlParameter("@GSEInactiveLogoutMinutes", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@GSEInactiveLogoutMinutes", SqlDbType.Int) { Value = this.gseInactiveLogoutMinutes.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.gseInactiveTimeout == null ? new SqlParameter("@GSEInactiveTimeout", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@GSEInactiveTimeout", SqlDbType.Int) { Value = this.gseInactiveTimeout.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.barcodeInvalidWarningSeconds == null ? new SqlParameter("@BarcodeInvalidWarningSeconds", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@BarcodeInvalidWarningSeconds", SqlDbType.Int) { Value = this.barcodeInvalidWarningSeconds.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.deIceBlendDefault == null ? new SqlParameter("@DeIceBlendDefault", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@DeIceBlendDefault", SqlDbType.Float) { Value = this.deIceBlendDefault.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.communicationTimeoutSeconds == null ? new SqlParameter("@CommunicationTimeoutSeconds", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@CommunicationTimeoutSeconds", SqlDbType.Int) { Value = this.communicationTimeoutSeconds.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.connectionRetries == null ? new SqlParameter("@ConnectionRetries", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ConnectionRetries", SqlDbType.Int) { Value = this.connectionRetries.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.connectionRetryTimeout == null ? new SqlParameter("@ConnectionRetryTimeout", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ConnectionRetryTimeout", SqlDbType.Int) { Value = this.connectionRetryTimeout.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.connectionType == null ? new SqlParameter("@ConnectionType", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ConnectionType", SqlDbType.Int) { Value = this.connectionType.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.updateInterval == null ? new SqlParameter("@UpdateInterval", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@UpdateInterval", SqlDbType.Int) { Value = this.updateInterval.Value };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@PingVerificationIPAddress", SqlDbType.Bit) { Value = this.pingVerificationIpAddress ? 1 : 0 };
			sqlCommand.Parameters.Add(parm);

			parm = this.vehicleUpdateInterval == null ? new SqlParameter("@VehicleUpdateInterval", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@VehicleUpdateInterval", SqlDbType.Int) { Value = this.vehicleUpdateInterval.Value };
			sqlCommand.Parameters.Add(parm);

			parm = this.presubmitDelay == null ? new SqlParameter("@PresubmitDelay", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@PresubmitDelay", SqlDbType.Int) { Value = this.presubmitDelay.Value };
			sqlCommand.Parameters.Add(parm);

			parm = string.IsNullOrEmpty(this.verificationIpAddress) ? new SqlParameter("@VerificationIPAddress", SqlDbType.NVarChar, 15) { Value = DBNull.Value } : new SqlParameter("@VerificationIPAddress", SqlDbType.NVarChar, 15) { Value = this.verificationIpAddress };
			sqlCommand.Parameters.Add(parm);

			parm = new SqlParameter("@CreatedBy", SqlDbType.NVarChar, 100) { Value = this.createdBy };
			sqlCommand.Parameters.Add ( parm );

			parm = new SqlParameter ( "@UpdatedBy", SqlDbType.NVarChar, 100 ) { Value = this.updatedBy };
			sqlCommand.Parameters.Add ( parm );
			
			if ( this.createdDate != null )
			{
				parm = new SqlParameter ( "@CreatedDate", SqlDbType.DateTimeOffset ) { Value = this.createdDate };
				sqlCommand.Parameters.Add ( parm );
			}

			if ( this.updatedDate != null )
			{
				parm = new SqlParameter ( "@UpdatedDate", SqlDbType.DateTimeOffset ) { Value = this.updatedDate };
				sqlCommand.Parameters.Add ( parm );
			}
		}

		/// <summary>
		/// This method will populate the sql command with on the columns that 
		/// have changed. It will set the sqlCommand to null if there are no columns
		/// that have changed.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command. Will be null if no column changes.
		/// </param>
		public void UpdateSql ( SqlCommand sqlCommand )
		{
			string update = "UPDATE tblMobileDeviceProfile SET ";
			string where  = "WHERE MobileDeviceProfileGuid = @MobileDeviceProfileGuid ";

			// Will return a list of property names that their values changed.
			List<string> changedProperties = this.CompareForChanges();

			if ( ( changedProperties == null ) || ( changedProperties.Count == 0 ) )
			{
				sqlCommand.CommandText = string.Empty;
			}
			else
			{
				bool firstTime = true;
				List<string> updateVariables = this.BuildUpdateSql(sqlCommand, changedProperties);

				foreach ( string setCommand in updateVariables )
				{
					if ( firstTime )
					{
						update = update + setCommand;
						firstTime = false;
					}
					else
					{
						update = update + ", " + setCommand;
					}
				}

				if ( updateVariables.Count > 0 )
				{
					var parm = new SqlParameter("@MobileDeviceProfileGuid", SqlDbType.UniqueIdentifier) { Value = this.MobileDeviceProfileGuid };
					sqlCommand.Parameters.Add(parm);

					sqlCommand.CommandText = update + " " + where;
				}
			}
		}
		#endregion

		#region Private methods
		/// <summary>
		/// This method will build an update statement on the columns that changed.
		/// </summary>
		/// <param name="sqlCommand">
		/// The sql command.
		/// </param>
		/// <param name="changedProperties">
		/// The changed properties.
		/// </param>
		/// <returns>Returns a collection of update statements to be updated.
		/// </returns>
		private List<string> BuildUpdateSql(SqlCommand sqlCommand, List<string> changedProperties )
		{
			var updateVariables = new List<string>( );
			bool hasOtherChanges = false;
			SqlParameter parm;

			foreach ( string propertyName in changedProperties )
			{
				if ( propertyName.Equals ( "SiteGuid" ) )
				{
					updateVariables.Add ( " SiteGuid = @SiteGuid" );
					parm = new SqlParameter ( "@SiteGuid", SqlDbType.UniqueIdentifier ) { Value = this.siteGuid };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ProfileId" ) )
				{
					updateVariables.Add ( " ProfileID = @ProfileID" );
					parm = string.IsNullOrEmpty(this.profileId) ? new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@ProfileID", SqlDbType.NVarChar, 50) { Value = this.profileId };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "Description" ) )
				{
					updateVariables.Add ( " Description = @Description" );
					parm = string.IsNullOrEmpty(this.description) ? new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = DBNull.Value } : new SqlParameter("@Description", SqlDbType.NVarChar, 200) { Value = this.description };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ConfirmFuelCaps" ) )
				{
					updateVariables.Add ( " ConfirmFuelCaps = @ConfirmFuelCaps" );
					parm = new SqlParameter ( "@ConfirmFuelCaps", SqlDbType.Bit ) { Value = this.ConfirmFuelCaps ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ShowProductScreen" ) )
				{
					updateVariables.Add ( " ShowProductScreen = @ShowProductScreen" );
					parm = new SqlParameter ( "@ShowProductScreen", SqlDbType.Bit ) { Value = this.showProductScreen ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "GenerateTicketNumber" ) )
				{
					updateVariables.Add ( " GenerateTicketNumber = @GenerateTicketNumber" );
					parm = new SqlParameter ( "@GenerateTicketNumber", SqlDbType.Bit ) { Value = this.generateTicketNumber ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ShowOperatorFieldInFlightList" ) )
				{
					updateVariables.Add ( " ShowOperatorFieldInFlightList = @ShowOperatorFieldInFlightList" );
					parm = new SqlParameter ( "@ShowOperatorFieldInFlightList", SqlDbType.Bit ) { Value = this.showOperatorFieldInFlightList ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "UseDefaultPrinter" ) )
				{
					updateVariables.Add ( " UseDefaultPrinter = @UseDefaultPrinter" );
					parm = new SqlParameter ( "@UseDefaultPrinter", SqlDbType.Bit ) { Value = this.useDefaultPrinter ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "DefaultPrinter" ) )
				{
					updateVariables.Add ( " DefaultPrinter = @DefaultPrinter" );
					parm = string.IsNullOrEmpty(this.defaultPrinter) ? new SqlParameter("@DefaultPrinter", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@DefaultPrinter", SqlDbType.NVarChar, 50) { Value = this.defaultPrinter };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("AdminPassword") )
				{
					updateVariables.Add(" AdminPassword = @AdminPassword");
					parm = this.adminPassword == null ? new SqlParameter("@AdminPassword", SqlDbType.VarBinary) { Value = DBNull.Value } : new SqlParameter("@AdminPassword", SqlDbType.VarBinary) { Value = this.adminPassword };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ShutdownHotKey" ) )
				{
					updateVariables.Add ( " ShutdownHotKey = @ShutdownHotKey" );
					parm = string.IsNullOrEmpty(this.shutdownHotKey) ? new SqlParameter("@ShutdownHotKey", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@ShutdownHotKey", SqlDbType.NVarChar, 50) { Value = this.shutdownHotKey };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "PrinterComPort" ) )
				{
					updateVariables.Add ( " PrinterCOMPort = @PrinterCOMPort" );
					parm = string.IsNullOrEmpty(this.printerComPort) ? new SqlParameter("@PrinterCOMPort", SqlDbType.NVarChar, 10) { Value = DBNull.Value } : new SqlParameter("@PrinterCOMPort", SqlDbType.NVarChar, 10) { Value = this.printerComPort };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "SearchType" ) )
				{
					updateVariables.Add ( " SearchType = @SearchType" );
					parm = this.searchType == null ? new SqlParameter("@SearchType", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@SearchType", SqlDbType.Int) { Value = this.searchType.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "LoggingOption" ) )
				{
					updateVariables.Add ( " LoggingOption = @LoggingOption" );
					parm = new SqlParameter ( "@LoggingOption", SqlDbType.Bit ) { Value = this.loggingOption ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowableFailedLoginAttempts" ) )
				{
					updateVariables.Add ( " AllowableFailedLoginAttempts = @AllowableFailedLoginAttempts" );
					parm = this.allowableFailedLoginAttempts == null ? new SqlParameter("@AllowableFailedLoginAttempts", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@AllowableFailedLoginAttempts", SqlDbType.Int) { Value = this.allowableFailedLoginAttempts.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "FuelDistributionPrecision" ) )
				{
					updateVariables.Add ( " FuelDistributionPrecision = @FuelDistributionPrecision" );
					parm = this.fuelDistributionPrecision == null ? new SqlParameter("@FuelDistributionPrecision", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@FuelDistributionPrecision", SqlDbType.Int) { Value = this.fuelDistributionPrecision.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("MakeDefaultProfile") )
				{
					updateVariables.Add(" MakeDefaultProfile = @MakeDefaultProfile");
					parm = new SqlParameter("@MakeDefaultProfile", SqlDbType.Bit) { Value = this.makeDefaultProfile };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "VehicleId" ) )
				{
					updateVariables.Add ( " VehicleID = @VehicleID" );
					parm = string.IsNullOrEmpty(this.vehicleId) ? new SqlParameter("@VehicleID", SqlDbType.NVarChar, 50) { Value = DBNull.Value } : new SqlParameter("@VehicleID", SqlDbType.NVarChar, 50) { Value = this.vehicleId };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "MonitorScreenTransitionTiming" ) )
				{
					updateVariables.Add ( " MonitorScreenTransitionTiming = @MonitorScreenTransitionTiming" );
					parm = new SqlParameter ( "@MonitorScreenTransitionTiming", SqlDbType.Bit ) { Value = this.monitorScreenTransitionTiming ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "BypassFsrCheckOnScreenTrans" ) )
				{
					updateVariables.Add ( " BypassFsrCheckOnScreenTrans = @BypassFsrCheckOnScreenTrans" );
					parm = new SqlParameter ( "@BypassFsrCheckOnScreenTrans", SqlDbType.Bit ) { Value = this.bypassFsrCheckOnScreenTrans ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ShowFuelUpdateCheckStatusWin" ) )
				{
					updateVariables.Add ( " ShowFuelUpdateCheckStatusWin = @ShowFuelUpdateCheckStatusWin" );
					parm = new SqlParameter ( "@ShowFuelUpdateCheckStatusWin", SqlDbType.Bit ) { Value = this.showFuelUpdateCheckStatusWin ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "RtdTemperatureRangeMax" ) )
				{
					updateVariables.Add ( " RTDTemperatureRangeMax = @RTDTemperatureRangeMax" );
					parm = this.rtdTemperatureRangeMax == null ? new SqlParameter("@RTDTemperatureRangeMax", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@RTDTemperatureRangeMax", SqlDbType.Float) { Value = this.rtdTemperatureRangeMax.Value };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "RtdTemperatureRangeMin" ) )
				{
					updateVariables.Add ( " RTDTemperatureRangeMin = @RTDTemperatureRangeMin" );
					parm = this.rtdTemperatureRangeMin == null ? new SqlParameter("@RTDTemperatureRangeMin", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@RTDTemperatureRangeMin", SqlDbType.Float) { Value = this.rtdTemperatureRangeMin.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "DefaultTemperature" ) )
				{
					updateVariables.Add ( " DefaultTemperature = @DefaultTemperature" );
					parm = this.defaultTemperature == null ? new SqlParameter("@DefaultTemperature", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@DefaultTemperature", SqlDbType.Float) { Value = this.defaultTemperature.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "StrictUserValidation" ) )
				{
					updateVariables.Add ( " StrictUserValidation = @StrictUserValidation" );
					parm = new SqlParameter ( "@StrictUserValidation", SqlDbType.Bit ) { Value = this.strictUserValidation ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
				}

				if ( propertyName.Equals ( "VerifyFuelingEquipment" ) )
				{
					updateVariables.Add ( " VerifyFuelingEquipment = @VerifyFuelingEquipment" );
					parm = new SqlParameter ( "@VerifyFuelingEquipment", SqlDbType.Bit ) { Value = this.verifyFuelingEquipment ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowEditRequiredFuelLoad" ) )
				{
					updateVariables.Add ( " AllowEditRequiredFuelLoad = @AllowEditRequiredFuelLoad" );
					parm = new SqlParameter ( "@AllowEditRequiredFuelLoad", SqlDbType.Bit ) { Value = this.allowEditRequiredFuelLoad ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowBackAfterArrivalScreen" ) )
				{
					updateVariables.Add ( " AllowBackAfterArrivalScreen = @AllowBackAfterArrivalScreen" );
					parm = new SqlParameter ( "@AllowBackAfterArrivalScreen", SqlDbType.Bit ) { Value = this.allowBackAfterArrivalScreen ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowBackAfterTicketPrinted" ) )
				{
					updateVariables.Add ( " AllowBackAfterTicketPrinted = @AllowBackAfterTicketPrinted" );
					parm = new SqlParameter ( "@AllowBackAfterTicketPrinted", SqlDbType.Bit ) { Value = this.allowBackAfterTicketPrinted ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "RequirePrint" ) )
				{
					updateVariables.Add ( " RequirePrint = @RequirePrint" );
					parm = new SqlParameter ( "@RequirePrint", SqlDbType.Bit ) { Value = this.requirePrint ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "TotalFuelLoadCheck" ) )
				{
					updateVariables.Add ( " TotalFuelLoadCheck = @TotalFuelLoadCheck" );
					parm = new SqlParameter ( "@TotalFuelLoadCheck", SqlDbType.Bit ) { Value = this.totalFuelLoadCheck ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "VolumetricThresholdValidation" ) )
				{
					updateVariables.Add ( " VolumetricThresholdValidation = @VolumetricThresholdValidation" );
					parm = new SqlParameter ( "@VolumetricThresholdValidation", SqlDbType.Bit ) { Value = this.volumetricThresholdValidation ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ValidateShipNumber" ) )
				{
					updateVariables.Add ( " ValidateShipNumber = @ValidateShipNumber" );
					parm = new SqlParameter ( "@ValidateShipNumber", SqlDbType.Bit ) { Value = this.validateShipNumber ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowVtoModification" ) )
				{
					updateVariables.Add ( " AllowVTOModification = @AllowVTOModification" );
					parm = new SqlParameter ( "@AllowVTOModification", SqlDbType.Bit ) { Value = this.allowVtoModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowFlightGateModification" ) )
				{
					updateVariables.Add ( " AllowFlightGateModification = @AllowFlightGateModification" );
					parm = new SqlParameter ( "@AllowFlightGateModification", SqlDbType.Bit ) { Value = this.allowFlightGateModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "TankPositionBalanceVerification" ) )
				{
					updateVariables.Add ( " TankPositionBalanceVerification = @TankPositionBalanceVerification" );
					parm = this.tankPositionBalanceVerification == null ? new SqlParameter("@TankPositionBalanceVerification", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@TankPositionBalanceVerification", SqlDbType.Int) { Value = this.tankPositionBalanceVerification.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "TankPositionBalancePercentage" ) )
				{
					updateVariables.Add ( " TankPositionBalancePercentage = @TankPositionBalancePercentage" );
					parm = this.TankPositionBalancePercentage == null ? new SqlParameter("@TankPositionBalancePercentage", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@TankPositionBalancePercentage", SqlDbType.Float) { Value = this.TankPositionBalancePercentage.Value };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "OverrideWingBalancePercentVar" ) )
				{
					updateVariables.Add ( " OverrideWingBalancePercentVar = @OverrideWingBalancePercentVar" );
					parm = new SqlParameter ( "@OverrideWingBalancePercentVar", SqlDbType.Bit ) { Value = this.overrideWingBalancePercentVar ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "BypassDistributionTolerance" ) )
				{
					updateVariables.Add ( " BypassDistributionTolerance = @BypassDistributionTolerance" );
					parm = new SqlParameter ( "@BypassDistributionTolerance", SqlDbType.Bit ) { Value = this.bypassDistributionTolerance ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "VehicleIdCheck" ) )
				{
					updateVariables.Add ( " VehicleIDCheck = @VehicleIDCheck" );
					parm = new SqlParameter ( "@VehicleIDCheck", SqlDbType.Bit ) { Value = this.vehicleIdCheck ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "GseFuelMustMatch" ) )
				{
					updateVariables.Add ( " GSEFuelMustMatch = @GSEFuelMustMatch" );
					parm = new SqlParameter ( "@GSEFuelMustMatch", SqlDbType.Bit ) { Value = this.gseFuelMustMatch ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowManualMeter" ) )
				{
					updateVariables.Add ( " AllowManualMeter = @AllowManualMeter" );
					parm = new SqlParameter ( "@AllowManualMeter", SqlDbType.Bit ) { Value = this.allowManualMeter ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "UseValidLogicGaTrans" ) )
				{
					updateVariables.Add ( " UseValidLogicGATrans = @UseValidLogicGATrans" );
					parm = new SqlParameter ( "@UseValidLogicGATrans", SqlDbType.Bit ) { Value = this.useValidLogicGaTrans ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowShipNumberModification" ) )
				{
					updateVariables.Add ( " AllowShipNumberModification = @AllowShipNumberModification" );
					parm = new SqlParameter ( "@AllowShipNumberModification", SqlDbType.Bit ) { Value = this.AllowShipNumberModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowAircraftTypeModification" ) )
				{
					updateVariables.Add ( " AllowAircraftTypeModification = @AllowAircraftTypeModification" );
					parm = new SqlParameter ( "@AllowAircraftTypeModification", SqlDbType.Bit ) { Value = this.allowAircraftTypeModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AllowDestinationModification" ) )
				{
					updateVariables.Add ( " AllowDestinationModification = @AllowDestinationModification" );
					parm = new SqlParameter ( "@AllowDestinationModification", SqlDbType.Bit ) { Value = this.allowDestinationModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "TicketPrinting" ) )
				{
					updateVariables.Add ( " TicketPrinting = @TicketPrinting" );
					parm = this.ticketPrinting == null ? new SqlParameter("@TicketPrinting", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@TicketPrinting", SqlDbType.Int) { Value = this.ticketPrinting.Value };
					sqlCommand.Parameters.Add(parm);
				}

				if ( propertyName.Equals ( "AircraftTypeVerification" ) )
				{
					updateVariables.Add ( " AircraftTypeVerification = @AircraftTypeVerification" );
					parm = this.aircraftTypeVerification == null ? new SqlParameter("@AircraftTypeVerification", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@AircraftTypeVerification", SqlDbType.Int) { Value = this.aircraftTypeVerification.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "Destination" ) )
				{
					updateVariables.Add ( " Destination = @Destination" );
					parm = this.destination == null ? new SqlParameter("@Destination", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@Destination", SqlDbType.Int) { Value = this.destination.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "Gate" ) )
				{
					updateVariables.Add ( " Gate = @Gate" );
					parm = this.gate == null ? new SqlParameter("@Gate", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@Gate", SqlDbType.Int) { Value = this.gate.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ShipNumber" ) )
				{
					updateVariables.Add ( " ShipNumber = @ShipNumber" );
					parm = this.ShipNumber == null ? new SqlParameter("@ShipNumber", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ShipNumber", SqlDbType.Int) { Value = this.shipNumber.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "MeterTotal" ) )
				{
					updateVariables.Add ( " MeterTotal = @MeterTotal" );
					parm = this.meterTotal == null ? new SqlParameter("@MeterTotal", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@MeterTotal", SqlDbType.Int) { Value = this.meterTotal.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "VolumePumped" ) )
				{
					updateVariables.Add ( " VolumePumped = @VolumePumped" );
					parm = this.volumePumped == null ? new SqlParameter("@VolumePumped", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@VolumePumped", SqlDbType.Int) { Value = this.volumePumped.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "TankCapacity" ) )
				{
					updateVariables.Add ( " TankCapacity = @TankCapacity" );
					parm = this.tankCapacity == null ? new SqlParameter("@TankCapacity", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@TankCapacity", SqlDbType.Int) { Value = this.tankCapacity.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaStrictUserValidation" ) )
				{
					updateVariables.Add ( " EAStrictUserValidation = @EAStrictUserValidation" );
					parm = new SqlParameter ( "@EAStrictUserValidation", SqlDbType.Bit ) { Value = this.eaStrictUserValidation ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaVerifyFuelingEquipment" ) )
				{
					updateVariables.Add ( " EAVerifyFuelingEquipment = @EAVerifyFuelingEquipment" );
					parm = new SqlParameter ( "@EAVerifyFuelingEquipment", SqlDbType.Bit ) { Value = this.eaVerifyFuelingEquipment ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowEditOfRequiredFuelLoad" ) )
				{
					updateVariables.Add ( " EAAllowEditOfRequiredFuelLoad = @EAAllowEditOfRequiredFuelLoad" );
					parm = new SqlParameter ( "@EAAllowEditOfRequiredFuelLoad", SqlDbType.Bit ) { Value = this.eaAllowEditOfRequiredFuelLoad ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowBackAfterArrivalScreen" ) )
				{
					updateVariables.Add ( " EAAllowBackAfterArrivalScreen = @EAAllowBackAfterArrivalScreen" );
					parm = new SqlParameter ( "@EAAllowBackAfterArrivalScreen", SqlDbType.Bit ) { Value = this.eaAllowBackAfterArrivalScreen ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowBackAfterTicketPrinted" ) )
				{
					updateVariables.Add ( " EAAllowBackAfterTicketPrinted = @EAAllowBackAfterTicketPrinted" );
					parm = new SqlParameter ( "@EAAllowBackAfterTicketPrinted", SqlDbType.Bit ) { Value = this.eaAllowBackAfterTicketPrinted ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaRequirePrint" ) )
				{
					updateVariables.Add ( " EARequirePrint = @EARequirePrint" );
					parm = new SqlParameter ( "@EARequirePrint", SqlDbType.Bit ) { Value = this.eaRequirePrint ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaTotalFuelLoad" ) )
				{
					updateVariables.Add ( " EATotalFuelLoad = @EATotalFuelLoad" );
					parm = new SqlParameter ( "@EATotalFuelLoad", SqlDbType.Bit ) { Value = this.eaTotalFuelLoad ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaVolumetricThresholdValidation" ) )
				{
					updateVariables.Add ( " EAVolumetricThresholdValidation = @EAVolumetricThresholdValidation" );
					parm = new SqlParameter ( "@EAVolumetricThresholdValidation", SqlDbType.Bit ) { Value = this.eaVolumetricThresholdValidation ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaValidateShipNumber" ) )
				{
					updateVariables.Add ( " EAValidateShipNumber = @EAValidateShipNumber" );
					parm = new SqlParameter ( "@EAValidateShipNumber", SqlDbType.Bit ) { Value = this.eaValidateShipNumber ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowVtoModification" ) )
				{
					updateVariables.Add ( " EAAllowVtoModification = @EAAllowVtoModification" );
					parm = new SqlParameter ( "@EAAllowVtoModification", SqlDbType.Bit ) { Value = this.eaAllowVtoModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowFlightGateModification" ) )
				{
					updateVariables.Add ( " EAAllowFlightGateModification = @EAAllowFlightGateModification" );
					parm = new SqlParameter ( "@EAAllowFlightGateModification", SqlDbType.Bit ) { Value = this.eaAllowFlightGateModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaTankDiffPercentage" ) )
				{
					updateVariables.Add ( " EATankDiffPercentage = @EATankDiffPercentage" );
					parm = new SqlParameter ( "@EATankDiffPercentage", SqlDbType.Bit ) { Value = this.eaTankDiffPercentage ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaWingBalancePercentage" ) )
				{
					updateVariables.Add ( " EAWingBalancePercentage = @EAWingBalancePercentage" );
					parm = new SqlParameter ( "@EAWingBalancePercentage", SqlDbType.Bit ) { Value = this.eaWingBalancePercentage ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaBypassDistributionTolerance" ) )
				{
					updateVariables.Add ( " EABypassDistributionTolerance = @EABypassDistributionTolerance" );
					parm = new SqlParameter ( "@EABypassDistributionTolerance", SqlDbType.Bit ) { Value = this.eaBypassDistributionTolerance ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaVehicleIdCheck" ) )
				{
					updateVariables.Add ( " EAVehicleIDCheck = @EAVehicleIDCheck" );
					parm = new SqlParameter ( "@EAVehicleIDCheck", SqlDbType.Bit ) { Value = this.eaVehicleIdCheck ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaGseFuelMustMatch" ) )
				{
					updateVariables.Add ( " EAGseFuelMustMatch = @EAGseFuelMustMatch" );
					parm = new SqlParameter ( "@EAGseFuelMustMatch", SqlDbType.Bit ) { Value = this.eaGseFuelMustMatch ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowManualMeter" ) )
				{
					updateVariables.Add ( " EAAllowManualMeter = @EAAllowManualMeter" );
					parm = new SqlParameter ( "@EAAllowManualMeter", SqlDbType.Bit ) { Value = this.eaAllowManualMeter ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaUseValidationLogicGaTrans" ) )
				{
					updateVariables.Add ( " EAUseValidationLogicGATrans = @EAUseValidationLogicGATrans" );
					parm = new SqlParameter ( "@EAUseValidationLogicGATrans", SqlDbType.Bit ) { Value = this.eaUseValidationLogicGaTrans ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowShipNumberModification" ) )
				{
					updateVariables.Add ( " EAAllowShipNumberModification = @EAAllowShipNumberModification" );
					parm = new SqlParameter ( "@EAAllowShipNumberModification", SqlDbType.Bit ) { Value = this.eaAllowShipNumberModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowAircraftTypeModification" ) )
				{
					updateVariables.Add ( " EAAllowAircraftTypeModification = @EAAllowAircraftTypeModification" );
					parm = new SqlParameter ( "@EAAllowAircraftTypeModification", SqlDbType.Bit ) { Value = this.eaAllowAircraftTypeModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAllowDestinationModification" ) )
				{
					updateVariables.Add(" EAAllowDestinationModification = @EAAllowDestinationModification");
					parm = new SqlParameter("@EAAllowDestinationModification", SqlDbType.Bit) { Value = this.eaAllowDestinationModification ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("EaDestination") )
				{
					updateVariables.Add(" EADestination = @EADestination");
					parm = new SqlParameter("@EADestination", SqlDbType.Bit) { Value = this.eaDestination ? 1 : 0 };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaTicketPrinting" ) )
				{
					updateVariables.Add ( " EATicketPrinting = @EATicketPrinting" );
					parm = new SqlParameter ( "@EATicketPrinting", SqlDbType.Bit ) { Value = this.eaTicketPrinting ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaAircraftType" ) )
				{
					updateVariables.Add ( " EAAircraftType = @EAAircraftType" );
					parm = new SqlParameter ( "@EAAircraftType", SqlDbType.Bit ) { Value = this.eaAircraftType ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaShipNumber" ) )
				{
					updateVariables.Add ( " EAShipNumber = @EAShipNumber" );
					parm = new SqlParameter ( "@EAShipNumber", SqlDbType.Bit ) { Value = this.eaShipNumber ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaGateNumber" ) )
				{
					updateVariables.Add ( " EAGateNumber = @EAGateNumber" );
					parm = new SqlParameter ( "@EAGateNumber", SqlDbType.Bit ) { Value = this.eaGateNumber ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaMeterTotal" ) )
				{
					updateVariables.Add ( " EAMeterTotal = @EAMeterTotal" );
					parm = new SqlParameter ( "@EAMeterTotal", SqlDbType.Bit ) { Value = this.eaMeterTotal ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaVolumePumped" ) )
				{
					updateVariables.Add ( " EAVolumePumped = @EAVolumePumped" );
					parm = new SqlParameter ( "@EAVolumePumped", SqlDbType.Bit ) { Value = this.eaVolumePumped ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EaTankCapacity" ) )
				{
					updateVariables.Add ( " EATankCapacity = @EATankCapacity" );
					parm = new SqlParameter ( "@EATankCapacity", SqlDbType.Bit ) { Value = this.eaTankCapacity ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "EquipmentType" ) )
				{
					updateVariables.Add ( " EquipmentType = @EquipmentType" );
					parm = this.equipmentType == null ? new SqlParameter("@EquipmentType", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@EquipmentType", SqlDbType.Int) { Value = this.equipmentType.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ForeignKeyToMapEquipment" ) )
				{
					updateVariables.Add ( " ForeignKeyToMapEquipment = @ForeignKeyToMapEquipment" );
					parm = new SqlParameter ( "@ForeignKeyToMapEquipment", SqlDbType.UniqueIdentifier ) { Value = this.foreignKeyToMapEquipment };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "IssueTransaction" ) )
				{
					updateVariables.Add ( " IssueTransaction = @IssueTransaction" );
					parm = new SqlParameter ( "@IssueTransaction", SqlDbType.UniqueIdentifier ) { Value = this.issueTransaction };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "DefuelTransaction" ) )
				{
					updateVariables.Add ( " DefuelTransaction = @DefuelTransaction" );
					parm = new SqlParameter ( "@DefuelTransaction", SqlDbType.UniqueIdentifier ) { Value = this.defuelTransaction };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "RotationTransaction" ) )
				{
					updateVariables.Add ( " RotationTransaction = @RotationTransaction" );
					parm = new SqlParameter ( "@RotationTransaction", SqlDbType.UniqueIdentifier ) { Value = this.rotationTransaction };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "MeterCloseout" ) )
				{
					updateVariables.Add ( " MeterCloseout = @MeterCloseout" );
					parm = new SqlParameter ( "@MeterCloseout", SqlDbType.UniqueIdentifier ) { Value = this.meterCloseout };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("DeIceTransaction") )
				{
					updateVariables.Add(" DeIceTransaction = @DeIceTransaction");
					parm = new SqlParameter("@DeIceTransaction", SqlDbType.UniqueIdentifier) { Value = this.deIceTransaction };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("GseTransaction") )
				{
					updateVariables.Add(" GSETransaction = @GSETransaction");
					parm = new SqlParameter("@GSETransaction", SqlDbType.UniqueIdentifier) { Value = this.gseTransaction };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ManualConsumer" ) )
				{
					updateVariables.Add ( " ManualConsumer = @ManualConsumer" );
					parm = new SqlParameter ( "@ManualConsumer", SqlDbType.UniqueIdentifier ) { Value = this.manualConsumer };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ManualVendor") )
				{
					updateVariables.Add(" ManualVendor = @ManualVendor");
					parm = new SqlParameter("@ManualVendor", SqlDbType.UniqueIdentifier) { Value = this.manualVendor };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "CloseoutOwner" ) )
				{
					updateVariables.Add ( " CloseoutOwner = @CloseoutOwner" );
					parm = new SqlParameter ( "@CloseoutOwner", SqlDbType.UniqueIdentifier ) { Value = this.closeoutOwner };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "CloseoutVendor" ) )
				{
					updateVariables.Add ( " CloseoutVendor = @CloseoutVendor" );
					parm = new SqlParameter ( "@CloseoutVendor", SqlDbType.UniqueIdentifier) { Value = this.closeoutVendor };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ManualShipper" ) )
				{
					updateVariables.Add(" ManualShipper = @ManualShipper");
					parm = new SqlParameter("@ManualShipper", SqlDbType.UniqueIdentifier) { Value = this.manualShipper };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ManualManager") )
				{
					updateVariables.Add(" ManualManager = @ManualManager");
					parm = new SqlParameter("@ManualManager", SqlDbType.UniqueIdentifier) { Value = this.manualManager };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ManualSupplier") )
				{
					updateVariables.Add(" ManualSupplier = @ManualSupplier");
					parm = new SqlParameter("@ManualSupplier", SqlDbType.UniqueIdentifier) { Value = this.manualSupplier };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ManualBillTo") )
				{
					updateVariables.Add(" ManualBillTo = @ManualBillTo");
					parm = new SqlParameter("@ManualBillTo", SqlDbType.UniqueIdentifier) { Value = this.manualBillTo };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "CloseoutConsumer" ) )
				{
					updateVariables.Add ( " CloseoutConsumer = @CloseoutConsumer" );
					parm = new SqlParameter ( "@CloseoutConsumer", SqlDbType.UniqueIdentifier ) { Value = this.closeoutConsumer };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ManualProduct" ) )
				{
					updateVariables.Add ( " ManualProduct = @ManualProduct" );
					parm = new SqlParameter ( "@ManualProduct", SqlDbType.UniqueIdentifier ) { Value = this.manualProduct };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ManualStationId" ) )
				{
					updateVariables.Add ( " ManualStationID = @ManualStationID" );
					parm = new SqlParameter ( "@ManualStationID", SqlDbType.Int ) { Value = this.manualStationId };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "InhibitOverridingTemperature" ) )
				{
					updateVariables.Add ( " InhibitOverridingTemperature = @InhibitOverridingTemperature" );
					parm = new SqlParameter ( "@InhibitOverridingTemperature", SqlDbType.Bit ) { Value = this.inhibitOverridingTemperature ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ManualTemperature" ) )
				{
					updateVariables.Add ( " ManualTemperature = @ManualTemperature" );
					parm = this.manualTemperature == null ? new SqlParameter("@ManualTemperature", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@ManualTemperature", SqlDbType.Float) { Value = this.manualTemperature.Value };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "ManualDensity" ) )
				{
					updateVariables.Add ( " ManualDensity = @ManualDensity" );
					parm = this.manualDensity == null ? new SqlParameter("@ManualDensity", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@ManualDensity", SqlDbType.Float) { Value = this.manualDensity.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "HasDcu" ) )
				{
					updateVariables.Add ( " HasDCU = @HasDCU" );
					parm = new SqlParameter ( "@HasDCU", SqlDbType.Bit ) { Value = this.hasDCU ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "BluetoothDcu" ) )
				{
					updateVariables.Add ( " BluetoothDCU = @BluetoothDCU" );
					parm = new SqlParameter ( "@BluetoothDCU", SqlDbType.Bit ) { Value = this.bluetoothDCU ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "LogDcuActions" ) )
				{
					updateVariables.Add ( " LogDCUActions = @LogDCUActions" );
					parm = new SqlParameter ( "@LogDCUActions", SqlDbType.Bit ) { Value = this.logDCUActions ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "HasAveryHardoll" ) )
				{
					updateVariables.Add ( " HasAveryHardoll = @HasAveryHardoll" );
					parm = new SqlParameter ( "@HasAveryHardoll", SqlDbType.Bit ) { Value = this.hasAveryHardoll ? 1 : 0 };
					sqlCommand.Parameters.Add ( parm );
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "DcuComPort" ) )
				{
					updateVariables.Add ( " DCUComPort = @DCUComPort" );
					parm = string.IsNullOrEmpty(this.dcuComPort) ? new SqlParameter("@DCUComPort", SqlDbType.NVarChar, 4) { Value = DBNull.Value } : new SqlParameter("@DCUComPort", SqlDbType.NVarChar, 4) { Value = this.dcuComPort };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "DcuReadRetry" ) )
				{
					updateVariables.Add ( " DCUReadRetry = @DCUReadRetry" );
					parm = this.dcuReadRetry == null ? new SqlParameter("@DCUReadRetry", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@DCUReadRetry", SqlDbType.Int) { Value = this.dcuReadRetry.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "DcuDisconnectDelay" ) )
				{
					updateVariables.Add ( " DCUDisconnectDelay = @DCUDisconnectDelay" );
					parm = this.dcuDisconnectDelay == null ? new SqlParameter("@DCUDisconnectDelay", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@DCUDisconnectDelay", SqlDbType.Int) { Value = this.dcuDisconnectDelay.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("DcuCommunicationFailRestart") )
				{
					updateVariables.Add(" DCUCommunicationFailRestart = @DCUCommunicationFailRestart");
					parm = this.dcuCommunicationFailRestart == null ? new SqlParameter("@DCUCommunicationFailRestart", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@DCUCommunicationFailRestart", SqlDbType.Int) { Value = this.dcuCommunicationFailRestart.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AveryHardollComPort" ) )
				{
					updateVariables.Add ( " AveryHardollComPort = @AveryHardollComPort" );
					parm = string.IsNullOrEmpty(this.averyHardollComPort) ? new SqlParameter("@AveryHardollComPort", SqlDbType.NVarChar, 4) { Value = DBNull.Value } : new SqlParameter("@AveryHardollComPort", SqlDbType.NVarChar, 4) { Value = this.averyHardollComPort };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals ( "AveryHardollMeterID" ) )
				{
					updateVariables.Add ( " AveryHardollMeterID = @AveryHardollMeterID" );
					parm = string.IsNullOrEmpty(this.averyHardollMeterId) ? new SqlParameter("@AveryHardollMeterID", SqlDbType.NVarChar, 4) { Value = DBNull.Value } : new SqlParameter("@AveryHardollMeterID", SqlDbType.NVarChar, 4) { Value = this.averyHardollMeterId };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("VtoEnabled") )
				{
					updateVariables.Add(" VTOEnabled = @VTOEnabled");
					parm = new SqlParameter("@VTOEnabled", SqlDbType.Bit) { Value = this.vtoEnabled ? 1 : 0 };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("EnabledInOpGauges") )
				{
					updateVariables.Add(" EnabledInOpGauges = @EnabledInOpGauges");
					parm = new SqlParameter("@EnabledInOpGauges", SqlDbType.Bit) { Value = this.enabledInOpGauges ? 1 : 0 };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("UseDispensingVehicleGseTrans") )
				{
					updateVariables.Add(" UseDispensingVehicleGSETrans = @UseDispensingVehicleGSETrans");
					parm = new SqlParameter("@UseDispensingVehicleGSETrans", SqlDbType.Bit) { Value = this.useDispensingVehicleGseTrans ? 1 : 0 };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("GseWaitMsecForGetMeter") )
				{
					updateVariables.Add(" GSEWaitMSecForGetMeter = @GSEWaitMSecForGetMeter");
					parm = this.gseWaitMsecForGetMeter == null ? new SqlParameter("@GSEWaitMSecForGetMeter", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@GSEWaitMSecForGetMeter", SqlDbType.Int) { Value = this.gseWaitMsecForGetMeter.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("GseInactiveLogoutMinutes") )
				{
					updateVariables.Add(" GSEInactiveLogoutMinutes = @GSEInactiveLogoutMinutes");
					parm = this.gseInactiveLogoutMinutes == null ? new SqlParameter("@GSEInactiveLogoutMinutes", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@GSEInactiveLogoutMinutes", SqlDbType.Int) { Value = this.gseInactiveLogoutMinutes.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("GseInactiveTimeout") )
				{
					updateVariables.Add(" GSEInactiveTimeout = @GSEInactiveTimeout");
					parm = this.gseInactiveTimeout == null ? new SqlParameter("@GSEInactiveTimeout", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@GSEInactiveTimeout", SqlDbType.Int) { Value = this.gseInactiveTimeout.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("BarcodeInvalidWarningSeconds") )
				{
					updateVariables.Add(" BarcodeInvalidWarningSeconds = @BarcodeInvalidWarningSeconds");
					parm = this.barcodeInvalidWarningSeconds == null ? new SqlParameter("@BarcodeInvalidWarningSeconds", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@BarcodeInvalidWarningSeconds", SqlDbType.Int) { Value = this.barcodeInvalidWarningSeconds.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("DeIceBlendDefault") )
				{
					updateVariables.Add(" DeIceBlendDefault = @DeIceBlendDefault");
					parm = this.deIceBlendDefault == null ? new SqlParameter("@DeIceBlendDefault", SqlDbType.Float) { Value = DBNull.Value } : new SqlParameter("@DeIceBlendDefault", SqlDbType.Float) { Value = this.deIceBlendDefault.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("CommunicationTimeoutSeconds") )
				{
					updateVariables.Add(" CommunicationTimeoutSeconds = @CommunicationTimeoutSeconds");
					parm = this.communicationTimeoutSeconds == null ? new SqlParameter("@CommunicationTimeoutSeconds", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@CommunicationTimeoutSeconds", SqlDbType.Int) { Value = this.communicationTimeoutSeconds.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ConnectionRetries") )
				{
					updateVariables.Add(" ConnectionRetries = @ConnectionRetries");
					parm = this.connectionRetries == null ? new SqlParameter("@ConnectionRetries", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ConnectionRetries", SqlDbType.Int) { Value = this.connectionRetries.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ConnectionRetryTimeout") )
				{
					updateVariables.Add(" ConnectionRetryTimeout = @ConnectionRetryTimeout");
					parm = this.connectionRetryTimeout == null ? new SqlParameter("@ConnectionRetryTimeout", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ConnectionRetryTimeout", SqlDbType.Int) { Value = this.connectionRetryTimeout.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("ConnectionType") )
				{
					updateVariables.Add(" ConnectionType = @ConnectionType");
					parm = this.connectionType == null ? new SqlParameter("@ConnectionType", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@ConnectionType", SqlDbType.Int) { Value = this.connectionType.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("UpdateInterval") )
				{
					updateVariables.Add(" UpdateInterval = @UpdateInterval");
					parm = this.updateInterval == null ? new SqlParameter("@UpdateInterval", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@UpdateInterval", SqlDbType.Int) { Value = this.updateInterval.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PingVerificationIpAddress") )
				{
					updateVariables.Add(" PingVerificationIPAddress = @PingVerificationIPAddress");
					parm = new SqlParameter("@PingVerificationIPAddress", SqlDbType.Bit) { Value = this.pingVerificationIpAddress ? 1 : 0 };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("VehicleUpdateInterval") )
				{
					updateVariables.Add(" VehicleUpdateInterval = @VehicleUpdateInterval");
					parm = this.vehicleUpdateInterval == null ? new SqlParameter("@VehicleUpdateInterval", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@VehicleUpdateInterval", SqlDbType.Int) { Value = this.vehicleUpdateInterval.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("PresubmitDelay") )
				{
					updateVariables.Add(" PresubmitDelay = @PresubmitDelay");
					parm = this.presubmitDelay == null ? new SqlParameter("@PresubmitDelay", SqlDbType.Int) { Value = DBNull.Value } : new SqlParameter("@PresubmitDelay", SqlDbType.Int) { Value = this.presubmitDelay.Value };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}

				if ( propertyName.Equals("VerificationIpAddress") )
				{
					updateVariables.Add(" VerificationIPAddress = @VerificationIPAddress");
					parm = string.IsNullOrEmpty(this.verificationIpAddress) ? new SqlParameter("@VerificationIPAddress", SqlDbType.NVarChar, 15) { Value = DBNull.Value } : new SqlParameter("@VerificationIPAddress", SqlDbType.NVarChar, 15) { Value = this.verificationIpAddress };
					sqlCommand.Parameters.Add(parm);
					hasOtherChanges = true;
				}
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add ( " CreatedBy = @CreatedBy" );
				parm = new SqlParameter ( "@CreatedBy", SqlDbType.NVarChar, 100 ) { Value = this.createdBy };
				sqlCommand.Parameters.Add ( parm );
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add ( " UpdatedBy = @UpdatedBy" );
				parm = new SqlParameter ( "@UpdatedBy", SqlDbType.NVarChar, 100 ) { Value = this.updatedBy };
				sqlCommand.Parameters.Add ( parm );
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add ( " CreatedDate = @CreatedDate" );
				parm = new SqlParameter ( "@CreatedDate", SqlDbType.DateTimeOffset ) { Value = this.createdDate };
				sqlCommand.Parameters.Add ( parm );
			}

			if ( hasOtherChanges )
			{
				updateVariables.Add ( " UpdatedDate = @UpdatedDate" );
				parm = new SqlParameter ( "@UpdatedDate", SqlDbType.DateTimeOffset ) { Value = this.updatedDate };
				sqlCommand.Parameters.Add ( parm );
			}

			return updateVariables;
		}

		/// <summary>
		/// This method will compare the current property values with the old values. If there
		/// are changes the name of the property is added to a change list.
		/// </summary>
		/// <returns>A change list that contains the properties that have changed.
		/// </returns>
		private List<string> CompareForChanges ( )
		{
			var oldDataSet = ( DataSet ) this.DeserializeData ( );

			var oldMobileDeviceProfile = new MobileDeviceProfile();
			oldMobileDeviceProfile.Load(oldDataSet);

			List<string> changedProperties = this.GetChangedColumns ( this, oldMobileDeviceProfile );
			return changedProperties;
		}

		/// <summary>
		/// This method will load the information from the data row into the object's
		/// properties.
		/// </summary>
		/// <param name="row">The data row to be loaded.</param>
		private void LoadRow ( DataRow row )
		{
			if ( row != null )
			{
				this.mobileDeviceProfileGuid			= row.IsNull ( "MobileDeviceProfileGuid" ) ? Guid.Empty : ( Guid ) row["mobileDeviceProfileGuid"];
				this.siteGuid							= row.IsNull ( "SiteGuid" ) ? Guid.Empty : ( Guid ) row["SiteGuid"];
				this.profileId							= row.IsNull ( "ProfileID" ) ? string.Empty : ( string ) row["ProfileID"];
				this.description						= row.IsNull ( "Description" ) ? string.Empty : ( string ) row["Description"];
				this.confirmFuelCaps					= row.IsNull ( "ConfirmFuelCaps" ) ? false : ( bool ) row["ConfirmFuelCaps"];
				this.showProductScreen					= row.IsNull ( "ShowProductScreen" ) ? false : ( bool ) row["ShowProductScreen"];
				this.generateTicketNumber				= row.IsNull ( "GenerateTicketNumber" ) ? false : ( bool ) row["GenerateTicketNumber"];
				this.showOperatorFieldInFlightList		= row.IsNull ( "ShowOperatorFieldInFlightList" ) ? false : ( bool ) row["ShowOperatorFieldInFlightList"];
				this.useDefaultPrinter					= row.IsNull ( "UseDefaultPrinter" ) ? false : ( bool ) row["UseDefaultPrinter"];
				this.defaultPrinter						= row.IsNull ( "DefaultPrinter" ) ? string.Empty : ( string ) row["DefaultPrinter"];
				this.shutdownHotKey						= row.IsNull("ShutdownHotKey") ? string.Empty : (string)row["ShutdownHotKey"];
				this.printerComPort						= row.IsNull("PrinterCOMPort") ? string.Empty : (string)row["PrinterCOMPort"];
				this.loggingOption						= row.IsNull ("LoggingOption") ? false : (bool)row["LoggingOption"];

				this.adminPassword = null;
				if ( row.IsNull("AdminPassword") == false )
				{
					this.adminPassword = (byte[])row["AdminPassword"];
				}
				this.searchType = null;
				if ( row.IsNull("SearchType") == false )
				{
					this.searchType = (int) row["SearchType"];
				}

				this.allowableFailedLoginAttempts = null;
				if ( row.IsNull("AllowableFailedLoginAttempts") == false )
				{
					this.allowableFailedLoginAttempts = (int) row["AllowableFailedLoginAttempts"];
				}

				this.fuelDistributionPrecision = null;
				if ( row.IsNull("FuelDistributionPrecision") == false )
				{
					this.fuelDistributionPrecision = (int) row["FuelDistributionPrecision"];
				}

				this.rtdTemperatureRangeMax = null;
				if ( row.IsNull("RTDTemperatureRangeMax") == false )
				{
					this.rtdTemperatureRangeMax = (double) row["RTDTemperatureRangeMax"];
				}

				this.rtdTemperatureRangeMin = null;
				if ( row.IsNull("RTDTemperatureRangeMin") == false )
				{
					this.rtdTemperatureRangeMin = (double) row["RTDTemperatureRangeMin"];
				}

				this.defaultTemperature = null;
				if ( row.IsNull("DefaultTemperature") == false )
				{
					this.defaultTemperature = (double) row["DefaultTemperature"];
				}

				this.dcuReadRetry = null;
				if ( row.IsNull("DcuReadRetry") == false )
				{
					this.dcuReadRetry = (int) row["DcuReadRetry"];
				}

				this.dcuDisconnectDelay = null;
				if ( row.IsNull("DcuDisconnectDelay") == false )
				{
					this.dcuDisconnectDelay = (int) row["DcuDisconnectDelay"];
				}

				this.dcuCommunicationFailRestart = null;
				if ( row.IsNull("DCUCommunicationFailRestart") == false )
				{
					this.dcuCommunicationFailRestart = (int) row["DCUCommunicationFailRestart"];
				}

				this.tankPositionBalanceVerification = null;
				if ( row.IsNull("TankPositionBalanceVerification") == false )
				{
					this.tankPositionBalanceVerification = (int) row["TankPositionBalanceVerification"];
				}

				this.ticketPrinting = null;
				if ( row.IsNull("TicketPrinting") == false )
				{
					this.ticketPrinting = (int) row["TicketPrinting"];
				}

				this.aircraftTypeVerification = null;
				if ( row.IsNull("AircraftTypeVerification") == false )
				{
					this.aircraftTypeVerification = (int) row["AircraftTypeVerification"];
				}

				this.destination = null;
				if ( row.IsNull("Destination") == false )
				{
					this.destination = (int) row["Destination"];
				}

				this.gate = null;
				if ( row.IsNull("Gate") == false )
				{
					this.gate = (int) row["Gate"];
				}

				this.shipNumber = null;
				if ( row.IsNull("ShipNumber") == false )
				{
					this.shipNumber = (int) row["ShipNumber"];
				}

				this.meterTotal = null;
				if ( row.IsNull("MeterTotal") == false )
				{
					this.meterTotal = (int) row["MeterTotal"];
				}

				this.volumePumped = null;
				if ( row.IsNull("VolumePumped") == false )
				{
					this.volumePumped = (int) row["VolumePumped"];
				}

				this.tankCapacity = null;
				if ( row.IsNull("TankCapacity") == false )
				{
					this.tankCapacity = (int) row["TankCapacity"];
				}

				this.equipmentType = null;
				if ( row.IsNull("EquipmentType") == false )
				{
					this.equipmentType = (int) row["EquipmentType"];
				}

				this.gseWaitMsecForGetMeter = null;
				if ( row.IsNull("GSEWaitMSecForGetMeter") == false )
				{
					this.gseWaitMsecForGetMeter = (int) row["GSEWaitMSecForGetMeter"];
				}

				this.gseInactiveLogoutMinutes = null;
				if ( row.IsNull("GSEInactiveLogoutMinutes") == false )
				{
					this.gseInactiveLogoutMinutes = (int) row["GSEInactiveLogoutMinutes"];
				}

				this.gseInactiveTimeout = null;
				if ( row.IsNull("GSEInactiveTimeout") == false )
				{
					this.gseInactiveTimeout = (int) row["GSEInactiveTimeout"];
				}

				this.barcodeInvalidWarningSeconds = null;
				if ( row.IsNull("BarcodeInvalidWarningSeconds") == false )
				{
					this.barcodeInvalidWarningSeconds = (int) row["BarcodeInvalidWarningSeconds"];
				}

				this.communicationTimeoutSeconds = null;
				if ( row.IsNull("CommunicationTimeoutSeconds") == false )
				{
					this.communicationTimeoutSeconds = (int) row["CommunicationTimeoutSeconds"];
				}

				this.connectionRetries = null;
				if ( row.IsNull("ConnectionRetries") == false )
				{
					this.connectionRetries = (int) row["ConnectionRetries"];
				}

				this.connectionRetryTimeout = null;
				if ( row.IsNull("ConnectionRetryTimeout") == false )
				{
					this.connectionRetryTimeout = (int) row["ConnectionRetryTimeout"];
				}

				this.connectionType = null;
				if ( row.IsNull("ConnectionType") == false )
				{
					this.connectionType = (int) row["ConnectionType"];
				}

				this.updateInterval = null;
				if ( row.IsNull("UpdateInterval") == false )
				{
					this.updateInterval = (int) row["UpdateInterval"];
				}

				this.vehicleUpdateInterval = null;
				if ( row.IsNull("VehicleUpdateInterval") == false )
				{
					this.vehicleUpdateInterval = (int) row["VehicleUpdateInterval"];
				}

				this.presubmitDelay = null;
				if ( row.IsNull("PresubmitDelay") == false )
				{
					this.presubmitDelay = (int) row["PresubmitDelay"];
				}

				this.makeDefaultProfile					= row.IsNull("MakeDefaultProfile") ? false : (bool) row["MakeDefaultProfile"];
				this.vehicleId							= row.IsNull ("VehicleID") ? string.Empty : (string) row["VehicleID"];
				this.monitorScreenTransitionTiming		= row.IsNull ("MonitorScreenTransitionTiming") ? false : (bool) row["MonitorScreenTransitionTiming"];
				this.bypassFsrCheckOnScreenTrans		= row.IsNull ("BypassFsrCheckOnScreenTrans") ? false : (bool) row["BypassFsrCheckOnScreenTrans"];
				this.showFuelUpdateCheckStatusWin		= row.IsNull ("ShowFuelUpdateCheckStatusWin") ? false : (bool) row["ShowFuelUpdateCheckStatusWin"];
				this.strictUserValidation				= row.IsNull("StrictUserValidation") ? false : (bool) row["StrictUserValidation"];
				this.verifyFuelingEquipment				= row.IsNull("VerifyFuelingEquipment") ? false : (bool) row["VerifyFuelingEquipment"];
				this.allowEditRequiredFuelLoad			= row.IsNull("AllowEditRequiredFuelLoad") ? false : (bool) row["AllowEditRequiredFuelLoad"];
				this.allowBackAfterArrivalScreen		= row.IsNull("AllowBackAfterArrivalScreen") ? false : (bool)row["AllowBackAfterArrivalScreen"];
				this.allowBackAfterTicketPrinted		= row.IsNull("AllowBackAfterTicketPrinted") ? false : (bool)row["AllowBackAfterTicketPrinted"];
				this.requirePrint						= row.IsNull("RequirePrint") ? false : (bool) row["RequirePrint"];
				this.totalFuelLoadCheck					= row.IsNull("TotalFuelLoadCheck") ? false : (bool) row["TotalFuelLoadCheck"];
				this.volumetricThresholdValidation		= row.IsNull("VolumetricThresholdValidation") ? false : (bool) row["VolumetricThresholdValidation"];
				this.validateShipNumber					= row.IsNull("ValidateShipNumber") ? false : (bool) row["ValidateShipNumber"];
				this.allowVtoModification				= row.IsNull("AllowVTOModification") ? false : (bool) row["AllowVTOModification"];
				this.allowFlightGateModification		= row.IsNull("AllowFlightGateModification") ? false : (bool) row["AllowFlightGateModification"];
				this.tankPositionBalancePercentage		= row.IsNull("TankPositionBalancePercentage") ? 0 : (double) row["TankPositionBalancePercentage"];
				this.overrideWingBalancePercentVar		= row.IsNull("OverrideWingBalancePercentVar") ? false : (bool)row["OverrideWingBalancePercentVar"];
				this.bypassDistributionTolerance		= row.IsNull("BypassDistributionTolerance") ? false : (bool)row["BypassDistributionTolerance"];
				this.vehicleIdCheck						= row.IsNull("VehicleIDCheck") ? false : (bool) row["VehicleIDCheck"];
				this.gseFuelMustMatch					= row.IsNull("GSEFuelMustMatch") ? false : (bool) row["GSEFuelMustMatch"];
				this.allowManualMeter					= row.IsNull("AllowManualMeter") ? false : (bool) row["AllowManualMeter"];
				this.useValidLogicGaTrans				= row.IsNull("UseValidLogicGATrans") ? false : (bool) row["UseValidLogicGATrans"];
				this.allowShipNumberModification		= row.IsNull("AllowShipNumberModification") ? false : (bool) row["AllowShipNumberModification"];
				this.allowAircraftTypeModification		= row.IsNull("AllowAircraftTypeModification") ? false : (bool) row["AllowAircraftTypeModification"];
				this.allowDestinationModification		= row.IsNull("AllowDestinationModification") ? false : (bool) row["AllowDestinationModification"];
				this.eaStrictUserValidation				= row.IsNull("EAStrictUserValidation") ? false : (bool) row["EAStrictUserValidation"];
				this.eaVerifyFuelingEquipment			= row.IsNull("EAVerifyFuelingEquipment") ? false : (bool) row["EAVerifyFuelingEquipment"];
				this.eaAllowEditOfRequiredFuelLoad		= row.IsNull("EAAllowEditOfRequiredFuelLoad") ? false : (bool) row["EAAllowEditOfRequiredFuelLoad"];
				this.eaAllowBackAfterArrivalScreen		= row.IsNull("EAAllowBackAfterArrivalScreen") ? false : (bool) row["EAAllowBackAfterArrivalScreen"];
				this.eaAllowBackAfterTicketPrinted		= row.IsNull("EAAllowBackAfterTicketPrinted") ? false : (bool) row["EAAllowBackAfterTicketPrinted"];
				this.eaRequirePrint						= row.IsNull("EARequirePrint") ? false : (bool) row["EARequirePrint"];
				this.eaTotalFuelLoad					= row.IsNull("EATotalFuelLoad") ? false : (bool) row["EATotalFuelLoad"];
				this.eaVolumetricThresholdValidation	= row.IsNull("EAVolumetricThresholdValidation") ? false : (bool) row["EAVolumetricThresholdValidation"];
				this.eaValidateShipNumber				= row.IsNull("EAValidateShipNumber") ? false : (bool) row["EAValidateShipNumber"];
				this.eaAllowVtoModification				= row.IsNull("EAAllowVtoModification") ? false : (bool) row["EAAllowVtoModification"];
				this.eaAllowFlightGateModification		= row.IsNull("EAAllowFlightGateModification") ? false : (bool) row["EAAllowFlightGateModification"];
				this.eaTankDiffPercentage				= row.IsNull("EATankDiffPercentage") ? false : (bool) row["EATankDiffPercentage"];
				this.eaWingBalancePercentage			= row.IsNull("EAWingBalancePercentage") ? false : (bool) row["EAWingBalancePercentage"];
				this.eaBypassDistributionTolerance		= row.IsNull("EABypassDistributionTolerance") ? false : (bool) row["EABypassDistributionTolerance"];
				this.eaVehicleIdCheck					= row.IsNull("EAVehicleIDCheck") ? false : (bool) row["EAVehicleIDCheck"];
				this.eaGseFuelMustMatch					= row.IsNull("EAGseFuelMustMatch") ? false : (bool) row["EAGseFuelMustMatch"];
				this.eaAllowManualMeter					= row.IsNull("EAAllowManualMeter") ? false : (bool) row["EAAllowManualMeter"];
				this.eaUseValidationLogicGaTrans		= row.IsNull("EAUseValidationLogicGATrans") ? false : (bool) row["EAUseValidationLogicGATrans"];
				this.eaAllowShipNumberModification		= row.IsNull("EAAllowShipNumberModification") ? false : (bool) row["EAAllowShipNumberModification"];
				this.eaAllowAircraftTypeModification	= row.IsNull("EAAllowAircraftTypeModification") ? false : (bool) row["EAAllowAircraftTypeModification"];
				this.eaAllowDestinationModification		= row.IsNull("EAAllowDestinationModification") ? false : (bool) row["EAAllowDestinationModification"];
				this.eaDestination						= row.IsNull("EADestination") ? false : (bool) row["EADestination"];
				this.eaTicketPrinting					= row.IsNull("EATicketPrinting") ? false : (bool) row["EATicketPrinting"];
				this.eaAircraftType						= row.IsNull("EAAircraftType") ? false : (bool) row["EAAircraftType"];
				this.eaShipNumber						= row.IsNull("EAShipNumber") ? false : (bool) row["EAShipNumber"];
				this.eaGateNumber						= row.IsNull("EAGateNumber") ? false : (bool) row["EAGateNumber"];
				this.eaMeterTotal						= row.IsNull("EAMeterTotal") ? false : (bool) row["EAMeterTotal"];
				this.eaVolumePumped						= row.IsNull("EAVolumePumped") ? false : (bool) row["EAVolumePumped"];
				this.eaTankCapacity						= row.IsNull("EATankCapacity") ? false : (bool) row["EATankCapacity"];
				this.foreignKeyToMapEquipment			= row.IsNull("ForeignKeyToMapEquipment") ? Guid.Empty : (Guid)row["ForeignKeyToMapEquipment"];
				this.issueTransaction					= row.IsNull("IssueTransaction") ? Guid.Empty : (Guid)row["IssueTransaction"];
				this.defuelTransaction					= row.IsNull("DefuelTransaction") ? Guid.Empty : (Guid)row["DefuelTransaction"];
				this.rotationTransaction				= row.IsNull("RotationTransaction") ? Guid.Empty : (Guid)row["RotationTransaction"];
				this.meterCloseout						= row.IsNull("MeterCloseout") ? Guid.Empty : (Guid)row["MeterCloseout"];
				this.deIceTransaction					= row.IsNull("DeIceTransaction") ? Guid.Empty : (Guid) row["DeIceTransaction"];
				this.gseTransaction						= row.IsNull("GSETransaction") ? Guid.Empty : (Guid) row["GSETransaction"];
				this.manualConsumer						= row.IsNull("ManualConsumer") ? Guid.Empty : (Guid)row["ManualConsumer"];
				this.manualVendor						= row.IsNull("ManualVendor") ? Guid.Empty : (Guid) row["ManualVendor"];
				this.closeoutOwner						= row.IsNull("CloseoutOwner") ? Guid.Empty : (Guid) row["CloseoutOwner"];
				this.closeoutVendor						= row.IsNull("CloseoutVendor") ? Guid.Empty : (Guid)row["CloseoutVendor"];
				this.manualShipper						= row.IsNull("ManualShipper") ? Guid.Empty : (Guid) row["ManualShipper"];
				this.manualManager						= row.IsNull("ManualManager") ? Guid.Empty : (Guid) row["ManualManager"];
				this.manualSupplier						= row.IsNull("ManualSupplier") ? Guid.Empty : (Guid) row["ManualSupplier"];
				this.manualBillTo						= row.IsNull("ManualBillTo") ? Guid.Empty : (Guid) row["ManualBillTo"];
				this.closeoutConsumer					= row.IsNull("CloseoutConsumer") ? Guid.Empty : (Guid)row["CloseoutConsumer"];
				this.manualProduct						= row.IsNull("ManualProduct") ? Guid.Empty : (Guid)row["ManualProduct"];
				this.manualStationId					= row.IsNull("ManualStationID") ? 0 : (int)row["ManualStationID"];
				this.inhibitOverridingTemperature		= row.IsNull("InhibitOverridingTemperature") ? false : (bool)row["InhibitOverridingTemperature"];
				this.manualTemperature					= row.IsNull("ManualTemperature") ? 0.0 : (double)row["ManualTemperature"];
				this.manualDensity						= row.IsNull("ManualDensity") ? 0.0 : (double)row["ManualDensity"];
				this.hasDCU								= row.IsNull("HasDCU") ? false : (bool)row["HasDCU"];
				this.bluetoothDCU						= row.IsNull("BluetoothDCU") ? false : (bool)row["BluetoothDCU"];
				this.logDCUActions						= row.IsNull("LogDCUActions") ? false : (bool)row["LogDCUActions"];
				this.hasAveryHardoll					= row.IsNull("HasAveryHardoll") ? false : (bool)row["HasAveryHardoll"];
				this.dcuComPort							= row.IsNull("DcuComPort") ? string.Empty : (string)row["DcuComPort"];
				this.averyHardollComPort				= row.IsNull("AveryHardollComPort") ? string.Empty : (string)row["AveryHardollComPort"];
				this.averyHardollMeterId				= row.IsNull("AveryHardollMeterID") ? string.Empty : (string)row["AveryHardollMeterID"];
				this.vtoEnabled							= row.IsNull("VTOEnabled") ? false : (bool) row["VTOEnabled"];
				this.enabledInOpGauges					= row.IsNull("EnabledInOpGauges") ? false : (bool) row["EnabledInOpGauges"];
				this.useDispensingVehicleGseTrans		= row.IsNull("UseDispensingVehicleGSETrans") ? false : (bool) row["UseDispensingVehicleGSETrans"];
				this.deIceBlendDefault					= row.IsNull("DeIceBlendDefault") ? 0.0 : (double) row["DeIceBlendDefault"];
				this.pingVerificationIpAddress			= row.IsNull("PingVerificationIPAddress") ? false : (bool) row["PingVerificationIPAddress"];
				this.verificationIpAddress				= row.IsNull("VerificationIPAddress") ? string.Empty : (string) row["VerificationIPAddress"];
				this.createdBy							= row.IsNull("CreatedBy") ? string.Empty : (string)row["CreatedBy"];
				this.updatedBy							= row.IsNull("UpdatedBy") ? string.Empty : (string)row["UpdatedBy"];

				if ( row.IsNull ( "CreatedDate" ) == false )
				{
					this.createdDate = ( DateTimeOffset ) row [ "CreatedDate" ];
				}

				if ( row.IsNull ( "UpdatedDate" ) == false )
				{
					this.updatedDate = ( DateTimeOffset ) row [ "UpdatedDate" ];
				}
			}
		}
		#endregion

		#region Overrides

		/// <summary>
		/// The get update command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getUpdateCommand ( )
		{
			return null;
		}

		/// <summary>
		/// The get delete command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getDeleteCommand ( )
		{
			return null;
		}

		/// <summary>
		/// The get insert command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getInsertCommand ( )
		{
			return null;
		}

		/// <summary>
		/// The get select command.
		/// </summary>
		/// <returns>
		/// The System.String.
		/// </returns>
		override public string getSelectCommand ( )
		{
			return null;
		}
		#endregion
	}

	#region Mobile Device Profile Collection Class
	/// <summary>
	/// This class contains a collection of mobile device profile objects.
	/// </summary>
	[Serializable]
	[CollectionDataContract]
	public class MobileDeviceProfileCollection : List<MobileDeviceProfile>
	{
		#region Public methods
		/// <summary>
		/// This method will load the collection using a dataset. It will separate
		/// each row and create a new datarow with the row. This is for each
		/// analog input object.
		/// </summary>
		/// <param name="dataSet">
		/// The data set.
		/// </param>
		public void Load(DataSet dataSet)
		{
			if ( (dataSet != null) && (dataSet.Tables.Count > 0) )
			{
				var table = dataSet.Tables[0];

				foreach ( DataRow row in table.Rows )
				{
					var singleRowDataSet = dataSet.Clone( );
					var newTable		 = singleRowDataSet.Tables[0];
					var newRow			 = newTable.NewRow( );

					newRow.ItemArray = row.ItemArray;
					newTable.Rows.Add(newRow);

					var profile = new MobileDeviceProfile( );
					profile.LoadSingle(singleRowDataSet);
					this.Add(profile);
				}
			}
		}
		#endregion
	}
	#endregion
}
