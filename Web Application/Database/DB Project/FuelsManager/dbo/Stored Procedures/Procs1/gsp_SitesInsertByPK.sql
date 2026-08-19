CREATE PROCEDURE [dbo].[gsp_SitesInsertByPK]
(
		@SiteGuid uniqueidentifier=NULL OUTPUT
	,	@ID nvarchar(30)=NULL
	,	@Number nvarchar(30)=NULL
	,	@SPLCCode nvarchar(30)=NULL
	,	@Address1 nvarchar(30)=NULL
	,	@Address2 nvarchar(30)=NULL
	,	@City nvarchar(60)=NULL
	,	@State nvarchar(20)=NULL
	,	@Zip nvarchar(11)=NULL
	,	@Country nvarchar(30)=NULL
	,	@Phone nvarchar(20)=NULL
	,	@FAX nvarchar(20)=NULL
	,	@EmailAddress nvarchar(30)=NULL
	,	@EmergencyContact nvarchar(30)=NULL
	,	@EmergencyPhone nvarchar(20)=NULL
	,	@Enabled bit=NULL
	,	@SiteGroupFlag bit=NULL
	,	@TimeZone nvarchar(50)=NULL
	,	@LevelUnitIndex int=NULL
	,	@TemperatureUnitIndex int=NULL
	,	@DensityUnitIndex int=NULL
	,	@PressureUnitIndex int=NULL
	,	@FlowUnitIndex int=NULL
	,	@VolumeUnitIndex int=NULL
	,	@MassUnitIndex int=NULL
	,	@AdditiveVolumeUnitIndex int=NULL
	,	@AdditiveProfileCycleAmountUnitIndex int=NULL
	,	@AdditiveProfileRateUnitIndex int=NULL
	,	@LevelDecimalPlaces tinyint=NULL
	,	@TemperatureDecimalPlaces tinyint=NULL
	,	@DensityDecimalPlaces tinyint=NULL
	,	@PressureDecimalPlaces tinyint=NULL
	,	@FlowDecimalPlaces tinyint=NULL
	,	@VolumeDecimalPlaces tinyint=NULL
	,	@MassDecimalPlaces tinyint=NULL
	,	@AdditiveVolumeDecimalPlaces tinyint=NULL
	,	@AdditiveProfileCycleAmountDecimalPlaces tinyint=NULL
	,	@AdditiveProfileRateDecimalPlaces tinyint=NULL
	,	@InhibitAccessAfterHours bit=NULL
	,	@InhibitMultipleCardIns bit=NULL
	,	@AccessCardInRequired bit=NULL
	,	@CheckSiteNumber bit=NULL
	,	@PromptForCustomerCard bit=NULL
	,	@PromptForTractorOrTanker bit=NULL
	,	@PromptForFirstTrailer bit=NULL
	,	@PromptForSecondTrailer bit=NULL
	,	@PromptForCompartment bit=NULL
	,	@EnforceDriverEquipmentMatch bit=NULL
	,	@EnableAdditiveAccounting bit=NULL
	,	@UseCompanyEquipmentIdentifiers bit=NULL
	,	@UseLastKnownGoodTankData bit=NULL
	,	@MaximumLoadAmount float=NULL
	,	@MaximumLoadTime int=NULL
	,	@MaximumIdleTime int=NULL
	,	@MaximumFlushAmount float=NULL
	,	@MaximumMeterProvingAmount float=NULL
	,	@MaximumReturnsAmount float=NULL
	,	@MaximumNumberOfActiveArms int=NULL
	,	@DriverTimeoutPeriod int=NULL
	,	@DriverWarningPeriod int=NULL
	,	@MaximumPrompts int=NULL
	,	@MaximumVehicleWeight float=NULL
	,	@LoadByNet bit=NULL
	,	@PromptForShipmentNumber bit=NULL
	,	@MaximumProductTemperature float=NULL
	,	@ListEquipment bit=NULL
	,	@DeferStationChanges bit=NULL
	,	@InhibitBOLWithBrokenBlends bit=NULL
	,	@InhibitBOLWithImproperAdditization bit=NULL
	,	@InhibitOverweightBOL bit=NULL
	,	@ExceptionBOLPrinter nvarchar(80)=NULL
	,	@EnableAutomaticBOLPrinting bit=NULL
	,	@AutomaticBOLStartNumber int=NULL
	,	@AutomaticBOLEndNumber int=NULL
	,	@AutomaticBOLNextNumber int=NULL
	,	@SeparateManualBOLNumbering bit=NULL
	,	@ManualBOLStartNumber int=NULL
	,	@ManualBOLEndNumber int=NULL
	,	@ManualBOLNextNumber int=NULL
	,	@TransactionStartNumber int=NULL
	,	@TransactionEndNumber int=NULL
	,	@TransactionNextNumber int=NULL
	,	@OrderStartNumber int=NULL
	,	@OrderEndNumber int=NULL
	,	@OrderNextNumber int=NULL
	,	@NumberPrefix nvarchar(10)=NULL
	,	@OpenTransactionWindow int=NULL
	,	@AdministrativeLockDate datetimeoffset(7)=NULL
	,	@OperationalLockDate datetimeoffset(7)=NULL
	,	@MaximumDaysToRetainLogs int=NULL
	,	@EnableDebugLogging bit=NULL
	,	@EnableAuditLogging bit=NULL
	,	@AutomaticallyPrintAlarmsAndEvents bit=NULL
	,	@AlarmAndEventPrinter nvarchar(80)=NULL
	,	@MailServer nvarchar(50)=NULL
	,	@MailFrom nvarchar(50)=NULL
	,	@MailUserName nvarchar(50)=NULL
	,	@MailPassword nvarchar(50)=NULL
	,	@DialupName nvarchar(50)=NULL
	,	@SCADASystem nvarchar(50)=NULL
	,	@InhibitTemplateGraphics bit=NULL
	,	@RefreshInterval int=NULL
	,	@InhibitEndOfDayOperations bit=NULL
	,	@InhibitEndOfMonthOperations bit=NULL
	,	@EndOfDayWarningPeriod int=NULL
	,	@InhibitAutomaticPhysicalInventory bit=NULL
	,	@InhibitAutomaticMeterCloseout bit=NULL
	,	@InhibitAutomaticReportGeneration bit=NULL
	,	@InhibitAutomaticAdjustmentDistribution bit=NULL
	,	@InhibitAutomaticCloseout bit=NULL
	,	@InhibitTankScan bit=NULL
	,	@ReportDirectory nvarchar(80)=NULL
	,	@ManageReports bit=NULL
	,	@ManagedReportDirectory nvarchar(80)=NULL
	,	@VRURateLimit float=NULL
	,	@VRUHourlyLimit float=NULL
	,	@VRUDailyLimit float=NULL
	,	@VRUYearlyLimit float=NULL
	,	@VRUCurrentYearLimit float=NULL
	,	@VRURateActual float=NULL
	,	@VRUHourlyActual float=NULL
	,	@VRUDailyActual float=NULL
	,	@VRUYearlyActual float=NULL
	,	@VRUCurrentYearActual float=NULL
	,	@VRURateLimitEnabled bit=NULL
	,	@VRUHourlyLimitEnabled bit=NULL
	,	@VRUDailyLimitEnabled bit=NULL
	,	@VRUYearlyLimitEnabled bit=NULL
	,	@VRUCurrentYearLimitEnabled bit=NULL
	,	@WatchdogPeriod int=NULL
	,	@WatchdogCounterStart int=NULL
	,	@WatchdogCounterEnd int=NULL
	,	@NumberDecimalSeparator nvarchar(1)=NULL
	,	@NumberGroupSeparator nvarchar(1)=NULL
	,	@ListSeparator nvarchar(1)=NULL
	,	@TimePattern nvarchar(20)=NULL
	,	@TimeSeparator nvarchar(1)=NULL
	,	@AMSymbol nvarchar(2)=NULL
	,	@PMSymbol nvarchar(2)=NULL
	,	@ShortDatePattern nvarchar(20)=NULL
	,	@DateSeparator nvarchar(1)=NULL
	,	@LongDatePattern nvarchar(30)=NULL
	,	@TwoDigitCalendarEndYear int=NULL
	,	@UserData1 nvarchar(60)=NULL
	,	@UserData2 nvarchar(60)=NULL
	,	@UserData3 nvarchar(60)=NULL
	,	@UserData4 nvarchar(60)=NULL
	,	@UserData5 nvarchar(60)=NULL
	,	@UserData6 nvarchar(60)=NULL
	,	@UserData7 nvarchar(60)=NULL
	,	@UserData8 nvarchar(60)=NULL
	,	@CreatedDate datetimeoffset(7)=NULL
	,	@CreatedBy udtUserID=NULL
	,	@UpdatedDate datetimeoffset(7)=NULL
	,	@UpdatedBy udtUserID=NULL
	,	@MinTimeAllowedToChangePwd int=NULL
	,	@MinPwdCharacterLength int=NULL
	,	@PwdExpirationInDays int=NULL
	,	@PwdLockoutThreshold int=NULL
	,	@CheckForPreviousPwd bit=NULL
	,	@StrongPwdUse int=NULL
	,	@PwdHistoryCount int=NULL
	,	@ApplyToAllSiteMembers bit=NULL
	,	@InactivityDisablePeriod int=NULL
	,	@EnforceSingleOwner bit=NULL
	,	@InhibitBOLSummaryAutoPopulate bit=NULL
	,	@InhibitOrderSummaryAutoPopulate bit=NULL
	,	@InhibitSupplyOrderSummaryAutoPopulate bit=NULL
	,	@InvoiceStartNumber int=NULL
	,	@InvoiceEndNumber int=NULL
	,	@InvoiceNextNumber int=NULL
	,	@PromptForReturns bit=NULL
	,	@PromptForTruckCard bit=NULL
	,	@StartingShortCardNumber int=NULL
	,	@UseShortCardNumber bit=NULL
	,	@ExcessVarianceCount tinyint=NULL
	,	@ExcessVarianceTolerance float=NULL
	,	@DisableArchivePeriod int=NULL
	,	@ExportArchiveDir nvarchar(255)=NULL
	,	@ImportArchiveDir nvarchar(255)=NULL
	,	@GroupLedgerByID bit=NULL
	,	@InhibitSiteLedgerRollup bit=NULL
	,	@UseTankReconciliation bit=NULL
	,	@LookupNumberGroupSizesTypeIndex int=NULL
	,	@LookupQuantityDisplayDefaultIndex tinyint=NULL
	,	@LookupSecondaryStorageFillMethodIndex tinyint=NULL
	,	@LookupMailConnectModeIndex tinyint=NULL
	,	@LookupWatchdogModeIndex tinyint=NULL
	,	@Contact1Name nvarchar(30)=NULL
	,	@Contact1Address1 nvarchar(30)=NULL
	,	@Contact1Address2 nvarchar(30)=NULL
	,	@Contact1City nvarchar(60)=NULL
	,	@Contact1State nvarchar(20)=NULL
	,	@Contact1Zip nvarchar(11)=NULL
	,	@Contact1Country nvarchar(30)=NULL
	,	@Contact1PhoneOffice nvarchar(20)=NULL
	,	@Contact1Fax nvarchar(20)=NULL
	,	@Contact1EmailAddress nvarchar(30)=NULL
	,	@Contact2Name nvarchar(30)=NULL
	,	@Contact2Address1 nvarchar(30)=NULL
	,	@Contact2Address2 nvarchar(30)=NULL
	,	@Contact2City nvarchar(60)=NULL
	,	@Contact2State nvarchar(20)=NULL
	,	@Contact2Zip nvarchar(11)=NULL
	,	@Contact2Country nvarchar(30)=NULL
	,	@Contact2PhoneOffice nvarchar(20)=NULL
	,	@Contact2Fax nvarchar(20)=NULL
	,	@Contact2EmailAddress nvarchar(30)=NULL
	,	@Contact1PhoneMobile nvarchar(20)=NULL
	,	@Contact2PhoneMobile nvarchar(20)=NULL
	,	@EnablePasswordHint bit=NULL
	,	@EnablePasswordReset bit=NULL
	,	@MeterReconciliationToleranceIsPercent bit=NULL
	,	@MeterReconciliationReportName nvarchar(60)=NULL
	,	@TranslatedHelpURL nvarchar(250)=NULL
	,	@AllowUseOfSpecialChars bit=NULL
	,	@EnablePeriodicSyncFlag bit=NULL
	,	@PeriodicSyncIntervalMinutes int=NULL
	,	@_RowVersion timestamp=NULL OUTPUT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored procedure: [dbo].[gsp_SitesInsertByPK] 
	-- Author: DBA - Auto generated
	-- Version/Date: 1.0.001 / 2014-02-05 15:58:32.4382767 -05:00
	-- Purpose: Insert into table [dbo].[tblSites]
	-- Notes:
	------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON;
	BEGIN TRY
 
 
		SET @SiteGuid=NEWID();
		SET @CreatedDate=ISNULL(@CreatedDate,sysdatetimeoffset())
 
		INSERT INTO [dbo].[tblSites] 
		(
			[SiteGuid]
		,	[ID]
		,	[Number]
		,	[SPLCCode]
		,	[Address1]
		,	[Address2]
		,	[City]
		,	[State]
		,	[Zip]
		,	[Country]
		,	[Phone]
		,	[FAX]
		,	[EmailAddress]
		,	[EmergencyContact]
		,	[EmergencyPhone]
		,	[Enabled]
		,	[SiteGroupFlag]
		,	[TimeZone]
		,	[LevelUnitIndex]
		,	[TemperatureUnitIndex]
		,	[DensityUnitIndex]
		,	[PressureUnitIndex]
		,	[FlowUnitIndex]
		,	[VolumeUnitIndex]
		,	[MassUnitIndex]
		,	[AdditiveVolumeUnitIndex]
		,	[AdditiveProfileCycleAmountUnitIndex]
		,	[AdditiveProfileRateUnitIndex]
		,	[LevelDecimalPlaces]
		,	[TemperatureDecimalPlaces]
		,	[DensityDecimalPlaces]
		,	[PressureDecimalPlaces]
		,	[FlowDecimalPlaces]
		,	[VolumeDecimalPlaces]
		,	[MassDecimalPlaces]
		,	[AdditiveVolumeDecimalPlaces]
		,	[AdditiveProfileCycleAmountDecimalPlaces]
		,	[AdditiveProfileRateDecimalPlaces]
		,	[InhibitAccessAfterHours]
		,	[InhibitMultipleCardIns]
		,	[AccessCardInRequired]
		,	[CheckSiteNumber]
		,	[PromptForCustomerCard]
		,	[PromptForTractorOrTanker]
		,	[PromptForFirstTrailer]
		,	[PromptForSecondTrailer]
		,	[PromptForCompartment]
		,	[EnforceDriverEquipmentMatch]
		,	[EnableAdditiveAccounting]
		,	[UseCompanyEquipmentIdentifiers]
		,	[UseLastKnownGoodTankData]
		,	[MaximumLoadAmount]
		,	[MaximumLoadTime]
		,	[MaximumIdleTime]
		,	[MaximumFlushAmount]
		,	[MaximumMeterProvingAmount]
		,	[MaximumReturnsAmount]
		,	[MaximumNumberOfActiveArms]
		,	[DriverTimeoutPeriod]
		,	[DriverWarningPeriod]
		,	[MaximumPrompts]
		,	[MaximumVehicleWeight]
		,	[LoadByNet]
		,	[PromptForShipmentNumber]
		,	[MaximumProductTemperature]
		,	[ListEquipment]
		,	[DeferStationChanges]
		,	[InhibitBOLWithBrokenBlends]
		,	[InhibitBOLWithImproperAdditization]
		,	[InhibitOverweightBOL]
		,	[ExceptionBOLPrinter]
		,	[EnableAutomaticBOLPrinting]
		,	[AutomaticBOLStartNumber]
		,	[AutomaticBOLEndNumber]
		,	[AutomaticBOLNextNumber]
		,	[SeparateManualBOLNumbering]
		,	[ManualBOLStartNumber]
		,	[ManualBOLEndNumber]
		,	[ManualBOLNextNumber]
		,	[TransactionStartNumber]
		,	[TransactionEndNumber]
		,	[TransactionNextNumber]
		,	[OrderStartNumber]
		,	[OrderEndNumber]
		,	[OrderNextNumber]
		,	[NumberPrefix]
		,	[OpenTransactionWindow]
		,	[AdministrativeLockDate]
		,	[OperationalLockDate]
		,	[MaximumDaysToRetainLogs]
		,	[EnableDebugLogging]
		,	[EnableAuditLogging]
		,	[AutomaticallyPrintAlarmsAndEvents]
		,	[AlarmAndEventPrinter]
		,	[MailServer]
		,	[MailFrom]
		,	[MailUserName]
		,	[MailPassword]
		,	[DialupName]
		,	[SCADASystem]
		,	[InhibitTemplateGraphics]
		,	[RefreshInterval]
		,	[InhibitEndOfDayOperations]
		,	[InhibitEndOfMonthOperations]
		,	[EndOfDayWarningPeriod]
		,	[InhibitAutomaticPhysicalInventory]
		,	[InhibitAutomaticMeterCloseout]
		,	[InhibitAutomaticReportGeneration]
		,	[InhibitAutomaticAdjustmentDistribution]
		,	[InhibitAutomaticCloseout]
		,	[InhibitTankScan]
		,	[ReportDirectory]
		,	[ManageReports]
		,	[ManagedReportDirectory]
		,	[VRURateLimit]
		,	[VRUHourlyLimit]
		,	[VRUDailyLimit]
		,	[VRUYearlyLimit]
		,	[VRUCurrentYearLimit]
		,	[VRURateActual]
		,	[VRUHourlyActual]
		,	[VRUDailyActual]
		,	[VRUYearlyActual]
		,	[VRUCurrentYearActual]
		,	[VRURateLimitEnabled]
		,	[VRUHourlyLimitEnabled]
		,	[VRUDailyLimitEnabled]
		,	[VRUYearlyLimitEnabled]
		,	[VRUCurrentYearLimitEnabled]
		,	[WatchdogPeriod]
		,	[WatchdogCounterStart]
		,	[WatchdogCounterEnd]
		,	[NumberDecimalSeparator]
		,	[NumberGroupSeparator]
		,	[ListSeparator]
		,	[TimePattern]
		,	[TimeSeparator]
		,	[AMSymbol]
		,	[PMSymbol]
		,	[ShortDatePattern]
		,	[DateSeparator]
		,	[LongDatePattern]
		,	[TwoDigitCalendarEndYear]
		,	[UserData1]
		,	[UserData2]
		,	[UserData3]
		,	[UserData4]
		,	[UserData5]
		,	[UserData6]
		,	[UserData7]
		,	[UserData8]
		,	[CreatedDate]
		,	[CreatedBy]
		,	[UpdatedDate]
		,	[UpdatedBy]
		,	[MinTimeAllowedToChangePwd]
		,	[MinPwdCharacterLength]
		,	[PwdExpirationInDays]
		,	[PwdLockoutThreshold]
		,	[CheckForPreviousPwd]
		,	[StrongPwdUse]
		,	[PwdHistoryCount]
		,	[ApplyToAllSiteMembers]
		,	[InactivityDisablePeriod]
		,	[EnforceSingleOwner]
		,	[InhibitBOLSummaryAutoPopulate]
		,	[InhibitOrderSummaryAutoPopulate]
		,	[InhibitSupplyOrderSummaryAutoPopulate]
		,	[InvoiceStartNumber]
		,	[InvoiceEndNumber]
		,	[InvoiceNextNumber]
		,	[PromptForReturns]
		,	[PromptForTruckCard]
		,	[StartingShortCardNumber]
		,	[UseShortCardNumber]
		,	[ExcessVarianceCount]
		,	[ExcessVarianceTolerance]
		,	[DisableArchivePeriod]
		,	[ExportArchiveDir]
		,	[ImportArchiveDir]
		,	[GroupLedgerByID]
		,	[InhibitSiteLedgerRollup]
		,	[UseTankReconciliation]
		,	[LookupNumberGroupSizesTypeIndex]
		,	[LookupQuantityDisplayDefaultIndex]
		,	[LookupSecondaryStorageFillMethodIndex]
		,	[LookupMailConnectModeIndex]
		,	[LookupWatchdogModeIndex]
		,	[Contact1Name]
		,	[Contact1Address1]
		,	[Contact1Address2]
		,	[Contact1City]
		,	[Contact1State]
		,	[Contact1Zip]
		,	[Contact1Country]
		,	[Contact1PhoneOffice]
		,	[Contact1Fax]
		,	[Contact1EmailAddress]
		,	[Contact2Name]
		,	[Contact2Address1]
		,	[Contact2Address2]
		,	[Contact2City]
		,	[Contact2State]
		,	[Contact2Zip]
		,	[Contact2Country]
		,	[Contact2PhoneOffice]
		,	[Contact2Fax]
		,	[Contact2EmailAddress]
		,	[Contact1PhoneMobile]
		,	[Contact2PhoneMobile]
		,	[EnablePasswordHint]
		,	[EnablePasswordReset]
		,	[MeterReconciliationToleranceIsPercent]
		,	[MeterReconciliationReportName]
		,	[TranslatedHelpURL]
		,	[AllowUseOfSpecialChars]
		,	[EnablePeriodicSyncFlag]
		,	[PeriodicSyncIntervalMinutes]
		)
		VALUES
		(
			@SiteGuid
		,	@ID
		,	@Number
		,	@SPLCCode
		,	@Address1
		,	@Address2
		,	@City
		,	@State
		,	@Zip
		,	@Country
		,	@Phone
		,	@FAX
		,	@EmailAddress
		,	@EmergencyContact
		,	@EmergencyPhone
		,	@Enabled
		,	@SiteGroupFlag
		,	@TimeZone
		,	@LevelUnitIndex
		,	@TemperatureUnitIndex
		,	@DensityUnitIndex
		,	@PressureUnitIndex
		,	@FlowUnitIndex
		,	@VolumeUnitIndex
		,	@MassUnitIndex
		,	@AdditiveVolumeUnitIndex
		,	@AdditiveProfileCycleAmountUnitIndex
		,	@AdditiveProfileRateUnitIndex
		,	@LevelDecimalPlaces
		,	@TemperatureDecimalPlaces
		,	@DensityDecimalPlaces
		,	@PressureDecimalPlaces
		,	@FlowDecimalPlaces
		,	@VolumeDecimalPlaces
		,	@MassDecimalPlaces
		,	@AdditiveVolumeDecimalPlaces
		,	@AdditiveProfileCycleAmountDecimalPlaces
		,	@AdditiveProfileRateDecimalPlaces
		,	@InhibitAccessAfterHours
		,	@InhibitMultipleCardIns
		,	@AccessCardInRequired
		,	@CheckSiteNumber
		,	@PromptForCustomerCard
		,	@PromptForTractorOrTanker
		,	@PromptForFirstTrailer
		,	@PromptForSecondTrailer
		,	@PromptForCompartment
		,	@EnforceDriverEquipmentMatch
		,	@EnableAdditiveAccounting
		,	@UseCompanyEquipmentIdentifiers
		,	@UseLastKnownGoodTankData
		,	@MaximumLoadAmount
		,	@MaximumLoadTime
		,	@MaximumIdleTime
		,	@MaximumFlushAmount
		,	@MaximumMeterProvingAmount
		,	@MaximumReturnsAmount
		,	@MaximumNumberOfActiveArms
		,	@DriverTimeoutPeriod
		,	@DriverWarningPeriod
		,	@MaximumPrompts
		,	@MaximumVehicleWeight
		,	@LoadByNet
		,	@PromptForShipmentNumber
		,	@MaximumProductTemperature
		,	@ListEquipment
		,	@DeferStationChanges
		,	@InhibitBOLWithBrokenBlends
		,	@InhibitBOLWithImproperAdditization
		,	@InhibitOverweightBOL
		,	@ExceptionBOLPrinter
		,	@EnableAutomaticBOLPrinting
		,	@AutomaticBOLStartNumber
		,	@AutomaticBOLEndNumber
		,	@AutomaticBOLNextNumber
		,	@SeparateManualBOLNumbering
		,	@ManualBOLStartNumber
		,	@ManualBOLEndNumber
		,	@ManualBOLNextNumber
		,	@TransactionStartNumber
		,	@TransactionEndNumber
		,	@TransactionNextNumber
		,	@OrderStartNumber
		,	@OrderEndNumber
		,	@OrderNextNumber
		,	@NumberPrefix
		,	@OpenTransactionWindow
		,	@AdministrativeLockDate
		,	@OperationalLockDate
		,	@MaximumDaysToRetainLogs
		,	@EnableDebugLogging
		,	@EnableAuditLogging
		,	@AutomaticallyPrintAlarmsAndEvents
		,	@AlarmAndEventPrinter
		,	@MailServer
		,	@MailFrom
		,	@MailUserName
		,	@MailPassword
		,	@DialupName
		,	@SCADASystem
		,	@InhibitTemplateGraphics
		,	@RefreshInterval
		,	@InhibitEndOfDayOperations
		,	@InhibitEndOfMonthOperations
		,	@EndOfDayWarningPeriod
		,	@InhibitAutomaticPhysicalInventory
		,	@InhibitAutomaticMeterCloseout
		,	@InhibitAutomaticReportGeneration
		,	@InhibitAutomaticAdjustmentDistribution
		,	@InhibitAutomaticCloseout
		,	@InhibitTankScan
		,	@ReportDirectory
		,	@ManageReports
		,	@ManagedReportDirectory
		,	@VRURateLimit
		,	@VRUHourlyLimit
		,	@VRUDailyLimit
		,	@VRUYearlyLimit
		,	@VRUCurrentYearLimit
		,	@VRURateActual
		,	@VRUHourlyActual
		,	@VRUDailyActual
		,	@VRUYearlyActual
		,	@VRUCurrentYearActual
		,	@VRURateLimitEnabled
		,	@VRUHourlyLimitEnabled
		,	@VRUDailyLimitEnabled
		,	@VRUYearlyLimitEnabled
		,	@VRUCurrentYearLimitEnabled
		,	@WatchdogPeriod
		,	@WatchdogCounterStart
		,	@WatchdogCounterEnd
		,	@NumberDecimalSeparator
		,	@NumberGroupSeparator
		,	@ListSeparator
		,	@TimePattern
		,	@TimeSeparator
		,	@AMSymbol
		,	@PMSymbol
		,	@ShortDatePattern
		,	@DateSeparator
		,	@LongDatePattern
		,	@TwoDigitCalendarEndYear
		,	@UserData1
		,	@UserData2
		,	@UserData3
		,	@UserData4
		,	@UserData5
		,	@UserData6
		,	@UserData7
		,	@UserData8
		,	@CreatedDate
		,	@CreatedBy
		,	@UpdatedDate
		,	@UpdatedBy
		,	@MinTimeAllowedToChangePwd
		,	@MinPwdCharacterLength
		,	@PwdExpirationInDays
		,	@PwdLockoutThreshold
		,	@CheckForPreviousPwd
		,	@StrongPwdUse
		,	@PwdHistoryCount
		,	@ApplyToAllSiteMembers
		,	@InactivityDisablePeriod
		,	@EnforceSingleOwner
		,	@InhibitBOLSummaryAutoPopulate
		,	@InhibitOrderSummaryAutoPopulate
		,	@InhibitSupplyOrderSummaryAutoPopulate
		,	@InvoiceStartNumber
		,	@InvoiceEndNumber
		,	@InvoiceNextNumber
		,	@PromptForReturns
		,	@PromptForTruckCard
		,	@StartingShortCardNumber
		,	@UseShortCardNumber
		,	@ExcessVarianceCount
		,	@ExcessVarianceTolerance
		,	@DisableArchivePeriod
		,	@ExportArchiveDir
		,	@ImportArchiveDir
		,	@GroupLedgerByID
		,	@InhibitSiteLedgerRollup
		,	@UseTankReconciliation
		,	@LookupNumberGroupSizesTypeIndex
		,	@LookupQuantityDisplayDefaultIndex
		,	@LookupSecondaryStorageFillMethodIndex
		,	@LookupMailConnectModeIndex
		,	@LookupWatchdogModeIndex
		,	@Contact1Name
		,	@Contact1Address1
		,	@Contact1Address2
		,	@Contact1City
		,	@Contact1State
		,	@Contact1Zip
		,	@Contact1Country
		,	@Contact1PhoneOffice
		,	@Contact1Fax
		,	@Contact1EmailAddress
		,	@Contact2Name
		,	@Contact2Address1
		,	@Contact2Address2
		,	@Contact2City
		,	@Contact2State
		,	@Contact2Zip
		,	@Contact2Country
		,	@Contact2PhoneOffice
		,	@Contact2Fax
		,	@Contact2EmailAddress
		,	@Contact1PhoneMobile
		,	@Contact2PhoneMobile
		,	@EnablePasswordHint
		,	@EnablePasswordReset
		,	@MeterReconciliationToleranceIsPercent
		,	@MeterReconciliationReportName
		,	@TranslatedHelpURL
		,	@AllowUseOfSpecialChars
		,	@EnablePeriodicSyncFlag
		,	@PeriodicSyncIntervalMinutes
		)
 
		SELECT @_RowVersion=_RowVersion        
		FROM [dbo].[tblSites]           
		WHERE SiteGuid=@SiteGuid;
	
 
	END TRY
	BEGIN CATCH        
		DECLARE	@_ErrMessage NVARCHAR(2048)      
				, @_ErrNumber INT           
				, @_ErrProcName NVARCHAR(126)           
				, @_ErrLineNumber INT;            
		SET @_ErrMessage = ERROR_MESSAGE();        
		SET @_ErrNumber = ERROR_NUMBER();        
		SET @_ErrProcName= ERROR_PROCEDURE();        
		SET @_ErrLineNumber = ERROR_LINE();            
		SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13)+CHAR(10)                 
						+ 'Number: ' + CAST(@_ErrNumber AS VARCHAR(20)) + CHAR(13)+CHAR(10)                 
						+ 'Procedure Name: gsp_SitesInsertByPK' + CHAR(13)+CHAR(10)                  
						+ 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS VARCHAR(20)),'') + CHAR(13)+CHAR(10);         
		RAISERROR(@_ErrMessage,18,1);      
	END CATCH    
END     
	
