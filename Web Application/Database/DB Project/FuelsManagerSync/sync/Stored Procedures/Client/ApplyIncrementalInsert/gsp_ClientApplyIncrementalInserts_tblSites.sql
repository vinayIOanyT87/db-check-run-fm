-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblSites
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblSites]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(30),
@Number nvarchar(30),
@SPLCCode nvarchar(30),
@Address1 nvarchar(30),
@Address2 nvarchar(30),
@City nvarchar(60),
@State nvarchar(20),
@Zip nvarchar(11),
@Country nvarchar(30),
@Phone nvarchar(20),
@FAX nvarchar(20),
@EmailAddress nvarchar(30),
@EmergencyContact nvarchar(30),
@EmergencyPhone nvarchar(20),
@Enabled bit,
@SiteGroupFlag bit,
@TimeZone nvarchar(50),
@LevelUnitIndex int,
@TemperatureUnitIndex int,
@DensityUnitIndex int,
@PressureUnitIndex int,
@FlowUnitIndex int,
@VolumeUnitIndex int,
@MassUnitIndex int,
@AdditiveVolumeUnitIndex int,
@AdditiveProfileCycleAmountUnitIndex int,
@AdditiveProfileRateUnitIndex int,
@LevelDecimalPlaces tinyint,
@TemperatureDecimalPlaces tinyint,
@DensityDecimalPlaces tinyint,
@PressureDecimalPlaces tinyint,
@FlowDecimalPlaces tinyint,
@VolumeDecimalPlaces tinyint,
@MassDecimalPlaces tinyint,
@AdditiveVolumeDecimalPlaces tinyint,
@AdditiveProfileCycleAmountDecimalPlaces tinyint,
@AdditiveProfileRateDecimalPlaces tinyint,
@InhibitAccessAfterHours bit,
@InhibitMultipleCardIns bit,
@AccessCardInRequired bit,
@CheckSiteNumber bit,
@PromptForCustomerCard bit,
@PromptForTractorOrTanker bit,
@PromptForFirstTrailer bit,
@PromptForSecondTrailer bit,
@PromptForCompartment bit,
@EnforceDriverEquipmentMatch bit,
@EnableAdditiveAccounting bit,
@UseCompanyEquipmentIdentifiers bit,
@UseLastKnownGoodTankData bit,
@MaximumLoadAmount float,
@MaximumLoadTime int,
@MaximumIdleTime int,
@MaximumFlushAmount float,
@MaximumMeterProvingAmount float,
@MaximumReturnsAmount float,
@MaximumNumberOfActiveArms int,
@DriverTimeoutPeriod int,
@DriverWarningPeriod int,
@MaximumPrompts int,
@MaximumVehicleWeight float,
@LoadByNet bit,
@PromptForShipmentNumber bit,
@MaximumProductTemperature float,
@ListEquipment bit,
@DeferStationChanges bit,
@InhibitBOLWithBrokenBlends bit,
@InhibitBOLWithImproperAdditization bit,
@InhibitOverweightBOL bit,
@ExceptionBOLPrinter nvarchar(80),
@EnableAutomaticBOLPrinting bit,
@AutomaticBOLStartNumber int,
@AutomaticBOLEndNumber int,
@SeparateManualBOLNumbering bit,
@ManualBOLStartNumber int,
@ManualBOLEndNumber int,
@TransactionStartNumber int,
@TransactionEndNumber int,
@OrderStartNumber int,
@OrderEndNumber int,
@OpenTransactionWindow int,
@AdministrativeLockDate datetimeoffset(7),
@OperationalLockDate datetimeoffset(7),
@MaximumDaysToRetainLogs int,
@EnableDebugLogging bit,
@EnableAuditLogging bit,
@AutomaticallyPrintAlarmsAndEvents bit,
@AlarmAndEventPrinter nvarchar(80),
@MailServer nvarchar(50),
@MailFrom nvarchar(50),
@MailUserName nvarchar(50),
@MailPassword nvarchar(50),
@DialupName nvarchar(50),
@SCADASystem nvarchar(50),
@InhibitTemplateGraphics bit,
@RefreshInterval int,
@InhibitEndOfDayOperations bit,
@InhibitEndOfMonthOperations bit,
@EndOfDayWarningPeriod int,
@InhibitAutomaticPhysicalInventory bit,
@InhibitAutomaticMeterCloseout bit,
@InhibitAutomaticReportGeneration bit,
@InhibitAutomaticAdjustmentDistribution bit,
@InhibitAutomaticCloseout bit,
@InhibitTankScan bit,
@ReportDirectory nvarchar(80),
@ManageReports bit,
@ManagedReportDirectory nvarchar(80),
@VRURateLimit float,
@VRUHourlyLimit float,
@VRUDailyLimit float,
@VRUYearlyLimit float,
@VRUCurrentYearLimit float,
@VRURateActual float,
@VRUHourlyActual float,
@VRUDailyActual float,
@VRUYearlyActual float,
@VRUCurrentYearActual float,
@VRURateLimitEnabled bit,
@VRUHourlyLimitEnabled bit,
@VRUDailyLimitEnabled bit,
@VRUYearlyLimitEnabled bit,
@VRUCurrentYearLimitEnabled bit,
@WatchdogPeriod int,
@WatchdogCounterStart int,
@WatchdogCounterEnd int,
@NumberDecimalSeparator nvarchar(1),
@NumberGroupSeparator nvarchar(1),
@ListSeparator nvarchar(1),
@TimePattern nvarchar(20),
@TimeSeparator nvarchar(1),
@AMSymbol nvarchar(2),
@PMSymbol nvarchar(2),
@ShortDatePattern nvarchar(20),
@DateSeparator nvarchar(1),
@LongDatePattern nvarchar(30),
@TwoDigitCalendarEndYear int,
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@MinTimeAllowedToChangePwd int,
@MinPwdCharacterLength int,
@PwdExpirationInDays int,
@PwdLockoutThreshold int,
@CheckForPreviousPwd bit,
@StrongPwdUse int,
@PwdHistoryCount int,
@ApplyToAllSiteMembers bit,
@InactivityDisablePeriod int,
@EnforceSingleOwner bit,
@InhibitBOLSummaryAutoPopulate bit,
@InhibitOrderSummaryAutoPopulate bit,
@InhibitSupplyOrderSummaryAutoPopulate bit,
@InvoiceStartNumber int,
@InvoiceEndNumber int,
@PromptForReturns bit,
@PromptForTruckCard bit,
@StartingShortCardNumber int,
@UseShortCardNumber bit,
@ExcessVarianceCount tinyint,
@ExcessVarianceTolerance float,
@DisableArchivePeriod int,
@ExportArchiveDir nvarchar(255),
@ImportArchiveDir nvarchar(255),
@GroupLedgerByID bit,
@InhibitSiteLedgerRollup bit,
@UseTankReconciliation bit,
@SiteGuid uniqueidentifier,
@LookupNumberGroupSizesTypeIndex int,
@LookupQuantityDisplayDefaultIndex tinyint,
@LookupSecondaryStorageFillMethodIndex tinyint,
@LookupMailConnectModeIndex tinyint,
@LookupWatchdogModeIndex tinyint,
@Contact1Name nvarchar(30),
@Contact1Address1 nvarchar(30),
@Contact1Address2 nvarchar(30),
@Contact1City nvarchar(60),
@Contact1State nvarchar(20),
@Contact1Zip nvarchar(11),
@Contact1Country nvarchar(30),
@Contact1PhoneOffice nvarchar(20),
@Contact1Fax nvarchar(20),
@Contact1EmailAddress nvarchar(30),
@Contact2Name nvarchar(30),
@Contact2Address1 nvarchar(30),
@Contact2Address2 nvarchar(30),
@Contact2City nvarchar(60),
@Contact2State nvarchar(20),
@Contact2Zip nvarchar(11),
@Contact2Country nvarchar(30),
@Contact2PhoneOffice nvarchar(20),
@Contact2Fax nvarchar(20),
@Contact2EmailAddress nvarchar(30),
@Contact1PhoneMobile nvarchar(20),
@Contact2PhoneMobile nvarchar(20),
@EnablePasswordHint bit,
@EnablePasswordReset bit,
@MeterReconciliationToleranceIsPercent bit,
@MeterReconciliationReportName nvarchar(60),
@TranslatedHelpURL nvarchar(250),
@AllowUseOfSpecialChars bit,
@EnablePeriodicSyncFlag bit,
@PeriodicSyncIntervalMinutes int,
@DisableSyncTransferFlag bit,
@CardInTimeout int,
@TerminalControlNumber nvarchar(9),
@BlockCloseOnUnpostedBOL bit,
@InhibitLoadRackCardIns bit,
@PromptForThirdTrailer bit,
@PromptForTransactionCompletion bit,
@InhibitCustomerConfirmationPrompt bit,
@EnableBOLPDFArchiving bit,
@BOLPDFArchivingPath nvarchar(50),
@RequireTrailerScully bit,
@Latitude float,
@Longitude float,
@Zoom int,
@GlobalAccessToPersonnel bit,
@GlobalAccessToEquipment bit,
@Enterprise bit,
@OperateTabGroups bit,
@EnterpriseUserId nvarchar(100),
@EnterprisePassword varbinary(256),
@EnterpriseSite nvarchar(30),
@ActiveDirectorySiteGroupGuid uniqueidentifier,
@ServerEndPoint nvarchar(250),
@SecurityMode nvarchar(50),
@SecurityPolicy nvarchar(50),
@MessageEncoding nvarchar(50),
@UserIdentityMethod nvarchar(50),
@UserId nvarchar(250),
@UserPassword nvarchar(250),
@UserCertificatePath nvarchar(250),
@MaximumDaysToRetainArchive int,
@EnforceSalesOrderLimit bit,
@LeakDetectionQuietSamples int,
@LeakDetectionQuietTime int,
@LeakDetectionQuietTimeFactor int,
@LeakDetectionUseMinWait bit,
@LeakDetectionReport nvarchar(60),
@LeakDetectionPrinter nvarchar(80),
@EnableAutomaticMovementTicketPrinting bit,
@MovementTicketReport nvarchar(60),
@MovementTicketPrinter nvarchar(80),
@MaxOperateTabsAllowed int,
@CloseoutTime time,
@PointGroupFileExportDirectory nvarchar(255),
@PointGroupDefaultFileName nvarchar(255),
@EnableMovementTicketPDFArchiving bit,
@MovementTicketFileExportDirectory nvarchar(255),
@MovementTicketExportFileName nvarchar(255),
@MovementNumber int,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    ;   MERGE [dbo].[tblSites] AS existingData
        USING (SELECT @ID 'ID',@Number 'Number',@SPLCCode 'SPLCCode',@Address1 'Address1',@Address2 'Address2',@City 'City',@State 'State',@Zip 'Zip',@Country 'Country',@Phone 'Phone',@FAX 'FAX',@EmailAddress 'EmailAddress',@EmergencyContact 'EmergencyContact',@EmergencyPhone 'EmergencyPhone',@Enabled 'Enabled',@SiteGroupFlag 'SiteGroupFlag',@TimeZone 'TimeZone',@LevelUnitIndex 'LevelUnitIndex',@TemperatureUnitIndex 'TemperatureUnitIndex',@DensityUnitIndex 'DensityUnitIndex',@PressureUnitIndex 'PressureUnitIndex',@FlowUnitIndex 'FlowUnitIndex',@VolumeUnitIndex 'VolumeUnitIndex',@MassUnitIndex 'MassUnitIndex',@AdditiveVolumeUnitIndex 'AdditiveVolumeUnitIndex',@AdditiveProfileCycleAmountUnitIndex 'AdditiveProfileCycleAmountUnitIndex',@AdditiveProfileRateUnitIndex 'AdditiveProfileRateUnitIndex',@LevelDecimalPlaces 'LevelDecimalPlaces',@TemperatureDecimalPlaces 'TemperatureDecimalPlaces',@DensityDecimalPlaces 'DensityDecimalPlaces',@PressureDecimalPlaces 'PressureDecimalPlaces',@FlowDecimalPlaces 'FlowDecimalPlaces',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@MassDecimalPlaces 'MassDecimalPlaces',@AdditiveVolumeDecimalPlaces 'AdditiveVolumeDecimalPlaces',@AdditiveProfileCycleAmountDecimalPlaces 'AdditiveProfileCycleAmountDecimalPlaces',@AdditiveProfileRateDecimalPlaces 'AdditiveProfileRateDecimalPlaces',@InhibitAccessAfterHours 'InhibitAccessAfterHours',@InhibitMultipleCardIns 'InhibitMultipleCardIns',@AccessCardInRequired 'AccessCardInRequired',@CheckSiteNumber 'CheckSiteNumber',@PromptForCustomerCard 'PromptForCustomerCard',@PromptForTractorOrTanker 'PromptForTractorOrTanker',@PromptForFirstTrailer 'PromptForFirstTrailer',@PromptForSecondTrailer 'PromptForSecondTrailer',@PromptForCompartment 'PromptForCompartment',@EnforceDriverEquipmentMatch 'EnforceDriverEquipmentMatch',@EnableAdditiveAccounting 'EnableAdditiveAccounting',@UseCompanyEquipmentIdentifiers 'UseCompanyEquipmentIdentifiers',@UseLastKnownGoodTankData 'UseLastKnownGoodTankData',@MaximumLoadAmount 'MaximumLoadAmount',@MaximumLoadTime 'MaximumLoadTime',@MaximumIdleTime 'MaximumIdleTime',@MaximumFlushAmount 'MaximumFlushAmount',@MaximumMeterProvingAmount 'MaximumMeterProvingAmount',@MaximumReturnsAmount 'MaximumReturnsAmount',@MaximumNumberOfActiveArms 'MaximumNumberOfActiveArms',@DriverTimeoutPeriod 'DriverTimeoutPeriod',@DriverWarningPeriod 'DriverWarningPeriod',@MaximumPrompts 'MaximumPrompts',@MaximumVehicleWeight 'MaximumVehicleWeight',@LoadByNet 'LoadByNet',@PromptForShipmentNumber 'PromptForShipmentNumber',@MaximumProductTemperature 'MaximumProductTemperature',@ListEquipment 'ListEquipment',@DeferStationChanges 'DeferStationChanges',@InhibitBOLWithBrokenBlends 'InhibitBOLWithBrokenBlends',@InhibitBOLWithImproperAdditization 'InhibitBOLWithImproperAdditization',@InhibitOverweightBOL 'InhibitOverweightBOL',@ExceptionBOLPrinter 'ExceptionBOLPrinter',@EnableAutomaticBOLPrinting 'EnableAutomaticBOLPrinting',@AutomaticBOLStartNumber 'AutomaticBOLStartNumber',@AutomaticBOLEndNumber 'AutomaticBOLEndNumber',@SeparateManualBOLNumbering 'SeparateManualBOLNumbering',@ManualBOLStartNumber 'ManualBOLStartNumber',@ManualBOLEndNumber 'ManualBOLEndNumber',@TransactionStartNumber 'TransactionStartNumber',@TransactionEndNumber 'TransactionEndNumber',@OrderStartNumber 'OrderStartNumber',@OrderEndNumber 'OrderEndNumber',@OpenTransactionWindow 'OpenTransactionWindow',@AdministrativeLockDate 'AdministrativeLockDate',@OperationalLockDate 'OperationalLockDate',@MaximumDaysToRetainLogs 'MaximumDaysToRetainLogs',@EnableDebugLogging 'EnableDebugLogging',@EnableAuditLogging 'EnableAuditLogging',@AutomaticallyPrintAlarmsAndEvents 'AutomaticallyPrintAlarmsAndEvents',@AlarmAndEventPrinter 'AlarmAndEventPrinter',@MailServer 'MailServer',@MailFrom 'MailFrom',@MailUserName 'MailUserName',@MailPassword 'MailPassword',@DialupName 'DialupName',@SCADASystem 'SCADASystem',@InhibitTemplateGraphics 'InhibitTemplateGraphics',@RefreshInterval 'RefreshInterval',@InhibitEndOfDayOperations 'InhibitEndOfDayOperations',@InhibitEndOfMonthOperations 'InhibitEndOfMonthOperations',@EndOfDayWarningPeriod 'EndOfDayWarningPeriod',@InhibitAutomaticPhysicalInventory 'InhibitAutomaticPhysicalInventory',@InhibitAutomaticMeterCloseout 'InhibitAutomaticMeterCloseout',@InhibitAutomaticReportGeneration 'InhibitAutomaticReportGeneration',@InhibitAutomaticAdjustmentDistribution 'InhibitAutomaticAdjustmentDistribution',@InhibitAutomaticCloseout 'InhibitAutomaticCloseout',@InhibitTankScan 'InhibitTankScan',@ReportDirectory 'ReportDirectory',@ManageReports 'ManageReports',@ManagedReportDirectory 'ManagedReportDirectory',@VRURateLimit 'VRURateLimit',@VRUHourlyLimit 'VRUHourlyLimit',@VRUDailyLimit 'VRUDailyLimit',@VRUYearlyLimit 'VRUYearlyLimit',@VRUCurrentYearLimit 'VRUCurrentYearLimit',@VRURateActual 'VRURateActual',@VRUHourlyActual 'VRUHourlyActual',@VRUDailyActual 'VRUDailyActual',@VRUYearlyActual 'VRUYearlyActual',@VRUCurrentYearActual 'VRUCurrentYearActual',@VRURateLimitEnabled 'VRURateLimitEnabled',@VRUHourlyLimitEnabled 'VRUHourlyLimitEnabled',@VRUDailyLimitEnabled 'VRUDailyLimitEnabled',@VRUYearlyLimitEnabled 'VRUYearlyLimitEnabled',@VRUCurrentYearLimitEnabled 'VRUCurrentYearLimitEnabled',@WatchdogPeriod 'WatchdogPeriod',@WatchdogCounterStart 'WatchdogCounterStart',@WatchdogCounterEnd 'WatchdogCounterEnd',@NumberDecimalSeparator 'NumberDecimalSeparator',@NumberGroupSeparator 'NumberGroupSeparator',@ListSeparator 'ListSeparator',@TimePattern 'TimePattern',@TimeSeparator 'TimeSeparator',@AMSymbol 'AMSymbol',@PMSymbol 'PMSymbol',@ShortDatePattern 'ShortDatePattern',@DateSeparator 'DateSeparator',@LongDatePattern 'LongDatePattern',@TwoDigitCalendarEndYear 'TwoDigitCalendarEndYear',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@MinTimeAllowedToChangePwd 'MinTimeAllowedToChangePwd',@MinPwdCharacterLength 'MinPwdCharacterLength',@PwdExpirationInDays 'PwdExpirationInDays',@PwdLockoutThreshold 'PwdLockoutThreshold',@CheckForPreviousPwd 'CheckForPreviousPwd',@StrongPwdUse 'StrongPwdUse',@PwdHistoryCount 'PwdHistoryCount',@ApplyToAllSiteMembers 'ApplyToAllSiteMembers',@InactivityDisablePeriod 'InactivityDisablePeriod',@EnforceSingleOwner 'EnforceSingleOwner',@InhibitBOLSummaryAutoPopulate 'InhibitBOLSummaryAutoPopulate',@InhibitOrderSummaryAutoPopulate 'InhibitOrderSummaryAutoPopulate',@InhibitSupplyOrderSummaryAutoPopulate 'InhibitSupplyOrderSummaryAutoPopulate',@InvoiceStartNumber 'InvoiceStartNumber',@InvoiceEndNumber 'InvoiceEndNumber',@PromptForReturns 'PromptForReturns',@PromptForTruckCard 'PromptForTruckCard',@StartingShortCardNumber 'StartingShortCardNumber',@UseShortCardNumber 'UseShortCardNumber',@ExcessVarianceCount 'ExcessVarianceCount',@ExcessVarianceTolerance 'ExcessVarianceTolerance',@DisableArchivePeriod 'DisableArchivePeriod',@ExportArchiveDir 'ExportArchiveDir',@ImportArchiveDir 'ImportArchiveDir',@GroupLedgerByID 'GroupLedgerByID',@InhibitSiteLedgerRollup 'InhibitSiteLedgerRollup',@UseTankReconciliation 'UseTankReconciliation',@SiteGuid 'SiteGuid',@LookupNumberGroupSizesTypeIndex 'LookupNumberGroupSizesTypeIndex',@LookupQuantityDisplayDefaultIndex 'LookupQuantityDisplayDefaultIndex',@LookupSecondaryStorageFillMethodIndex 'LookupSecondaryStorageFillMethodIndex',@LookupMailConnectModeIndex 'LookupMailConnectModeIndex',@LookupWatchdogModeIndex 'LookupWatchdogModeIndex',@Contact1Name 'Contact1Name',@Contact1Address1 'Contact1Address1',@Contact1Address2 'Contact1Address2',@Contact1City 'Contact1City',@Contact1State 'Contact1State',@Contact1Zip 'Contact1Zip',@Contact1Country 'Contact1Country',@Contact1PhoneOffice 'Contact1PhoneOffice',@Contact1Fax 'Contact1Fax',@Contact1EmailAddress 'Contact1EmailAddress',@Contact2Name 'Contact2Name',@Contact2Address1 'Contact2Address1',@Contact2Address2 'Contact2Address2',@Contact2City 'Contact2City',@Contact2State 'Contact2State',@Contact2Zip 'Contact2Zip',@Contact2Country 'Contact2Country',@Contact2PhoneOffice 'Contact2PhoneOffice',@Contact2Fax 'Contact2Fax',@Contact2EmailAddress 'Contact2EmailAddress',@Contact1PhoneMobile 'Contact1PhoneMobile',@Contact2PhoneMobile 'Contact2PhoneMobile',@EnablePasswordHint 'EnablePasswordHint',@EnablePasswordReset 'EnablePasswordReset',@MeterReconciliationToleranceIsPercent 'MeterReconciliationToleranceIsPercent',@MeterReconciliationReportName 'MeterReconciliationReportName',@TranslatedHelpURL 'TranslatedHelpURL',@AllowUseOfSpecialChars 'AllowUseOfSpecialChars',@EnablePeriodicSyncFlag 'EnablePeriodicSyncFlag',@PeriodicSyncIntervalMinutes 'PeriodicSyncIntervalMinutes',@DisableSyncTransferFlag 'DisableSyncTransferFlag',@CardInTimeout 'CardInTimeout',@TerminalControlNumber 'TerminalControlNumber',@BlockCloseOnUnpostedBOL 'BlockCloseOnUnpostedBOL',@InhibitLoadRackCardIns 'InhibitLoadRackCardIns',@PromptForThirdTrailer 'PromptForThirdTrailer',@PromptForTransactionCompletion 'PromptForTransactionCompletion',@InhibitCustomerConfirmationPrompt 'InhibitCustomerConfirmationPrompt',@EnableBOLPDFArchiving 'EnableBOLPDFArchiving',@BOLPDFArchivingPath 'BOLPDFArchivingPath',@RequireTrailerScully 'RequireTrailerScully',@Latitude 'Latitude',@Longitude 'Longitude',@Zoom 'Zoom',@GlobalAccessToPersonnel 'GlobalAccessToPersonnel',@GlobalAccessToEquipment 'GlobalAccessToEquipment',@Enterprise 'Enterprise',@OperateTabGroups 'OperateTabGroups',@EnterpriseUserId 'EnterpriseUserId',@EnterprisePassword 'EnterprisePassword',@EnterpriseSite 'EnterpriseSite',@ActiveDirectorySiteGroupGuid 'ActiveDirectorySiteGroupGuid',@ServerEndPoint 'ServerEndPoint',@SecurityMode 'SecurityMode',@SecurityPolicy 'SecurityPolicy',@MessageEncoding 'MessageEncoding',@UserIdentityMethod 'UserIdentityMethod',@UserId 'UserId',@UserPassword 'UserPassword',@UserCertificatePath 'UserCertificatePath',@MaximumDaysToRetainArchive 'MaximumDaysToRetainArchive',@EnforceSalesOrderLimit 'EnforceSalesOrderLimit',@LeakDetectionQuietSamples 'LeakDetectionQuietSamples',@LeakDetectionQuietTime 'LeakDetectionQuietTime',@LeakDetectionQuietTimeFactor 'LeakDetectionQuietTimeFactor',@LeakDetectionUseMinWait 'LeakDetectionUseMinWait',@LeakDetectionReport 'LeakDetectionReport',@LeakDetectionPrinter 'LeakDetectionPrinter',@EnableAutomaticMovementTicketPrinting 'EnableAutomaticMovementTicketPrinting',@MovementTicketReport 'MovementTicketReport',@MovementTicketPrinter 'MovementTicketPrinter',@MaxOperateTabsAllowed 'MaxOperateTabsAllowed',@CloseoutTime 'CloseoutTime',@PointGroupFileExportDirectory 'PointGroupFileExportDirectory',@PointGroupDefaultFileName 'PointGroupDefaultFileName',@EnableMovementTicketPDFArchiving 'EnableMovementTicketPDFArchiving',@MovementTicketFileExportDirectory 'MovementTicketFileExportDirectory',@MovementTicketExportFileName 'MovementTicketExportFileName',@MovementNumber 'MovementNumber'
                ) AS remoteChanges ([ID],[Number],[SPLCCode],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmailAddress],[EmergencyContact],[EmergencyPhone],[Enabled],[SiteGroupFlag],[TimeZone],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[AdditiveProfileCycleAmountDecimalPlaces],[AdditiveProfileRateDecimalPlaces],[InhibitAccessAfterHours],[InhibitMultipleCardIns],[AccessCardInRequired],[CheckSiteNumber],[PromptForCustomerCard],[PromptForTractorOrTanker],[PromptForFirstTrailer],[PromptForSecondTrailer],[PromptForCompartment],[EnforceDriverEquipmentMatch],[EnableAdditiveAccounting],[UseCompanyEquipmentIdentifiers],[UseLastKnownGoodTankData],[MaximumLoadAmount],[MaximumLoadTime],[MaximumIdleTime],[MaximumFlushAmount],[MaximumMeterProvingAmount],[MaximumReturnsAmount],[MaximumNumberOfActiveArms],[DriverTimeoutPeriod],[DriverWarningPeriod],[MaximumPrompts],[MaximumVehicleWeight],[LoadByNet],[PromptForShipmentNumber],[MaximumProductTemperature],[ListEquipment],[DeferStationChanges],[InhibitBOLWithBrokenBlends],[InhibitBOLWithImproperAdditization],[InhibitOverweightBOL],[ExceptionBOLPrinter],[EnableAutomaticBOLPrinting],[AutomaticBOLStartNumber],[AutomaticBOLEndNumber],[SeparateManualBOLNumbering],[ManualBOLStartNumber],[ManualBOLEndNumber],[TransactionStartNumber],[TransactionEndNumber],[OrderStartNumber],[OrderEndNumber],[OpenTransactionWindow],[AdministrativeLockDate],[OperationalLockDate],[MaximumDaysToRetainLogs],[EnableDebugLogging],[EnableAuditLogging],[AutomaticallyPrintAlarmsAndEvents],[AlarmAndEventPrinter],[MailServer],[MailFrom],[MailUserName],[MailPassword],[DialupName],[SCADASystem],[InhibitTemplateGraphics],[RefreshInterval],[InhibitEndOfDayOperations],[InhibitEndOfMonthOperations],[EndOfDayWarningPeriod],[InhibitAutomaticPhysicalInventory],[InhibitAutomaticMeterCloseout],[InhibitAutomaticReportGeneration],[InhibitAutomaticAdjustmentDistribution],[InhibitAutomaticCloseout],[InhibitTankScan],[ReportDirectory],[ManageReports],[ManagedReportDirectory],[VRURateLimit],[VRUHourlyLimit],[VRUDailyLimit],[VRUYearlyLimit],[VRUCurrentYearLimit],[VRURateActual],[VRUHourlyActual],[VRUDailyActual],[VRUYearlyActual],[VRUCurrentYearActual],[VRURateLimitEnabled],[VRUHourlyLimitEnabled],[VRUDailyLimitEnabled],[VRUYearlyLimitEnabled],[VRUCurrentYearLimitEnabled],[WatchdogPeriod],[WatchdogCounterStart],[WatchdogCounterEnd],[NumberDecimalSeparator],[NumberGroupSeparator],[ListSeparator],[TimePattern],[TimeSeparator],[AMSymbol],[PMSymbol],[ShortDatePattern],[DateSeparator],[LongDatePattern],[TwoDigitCalendarEndYear],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MinTimeAllowedToChangePwd],[MinPwdCharacterLength],[PwdExpirationInDays],[PwdLockoutThreshold],[CheckForPreviousPwd],[StrongPwdUse],[PwdHistoryCount],[ApplyToAllSiteMembers],[InactivityDisablePeriod],[EnforceSingleOwner],[InhibitBOLSummaryAutoPopulate],[InhibitOrderSummaryAutoPopulate],[InhibitSupplyOrderSummaryAutoPopulate],[InvoiceStartNumber],[InvoiceEndNumber],[PromptForReturns],[PromptForTruckCard],[StartingShortCardNumber],[UseShortCardNumber],[ExcessVarianceCount],[ExcessVarianceTolerance],[DisableArchivePeriod],[ExportArchiveDir],[ImportArchiveDir],[GroupLedgerByID],[InhibitSiteLedgerRollup],[UseTankReconciliation],[SiteGuid],[LookupNumberGroupSizesTypeIndex],[LookupQuantityDisplayDefaultIndex],[LookupSecondaryStorageFillMethodIndex],[LookupMailConnectModeIndex],[LookupWatchdogModeIndex],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[EnablePasswordHint],[EnablePasswordReset],[MeterReconciliationToleranceIsPercent],[MeterReconciliationReportName],[TranslatedHelpURL],[AllowUseOfSpecialChars],[EnablePeriodicSyncFlag],[PeriodicSyncIntervalMinutes],[DisableSyncTransferFlag],[CardInTimeout],[TerminalControlNumber],[BlockCloseOnUnpostedBOL],[InhibitLoadRackCardIns],[PromptForThirdTrailer],[PromptForTransactionCompletion],[InhibitCustomerConfirmationPrompt],[EnableBOLPDFArchiving],[BOLPDFArchivingPath],[RequireTrailerScully],[Latitude],[Longitude],[Zoom],[GlobalAccessToPersonnel],[GlobalAccessToEquipment],[Enterprise],[OperateTabGroups],[EnterpriseUserId],[EnterprisePassword],[EnterpriseSite],[ActiveDirectorySiteGroupGuid],[ServerEndPoint],[SecurityMode],[SecurityPolicy],[MessageEncoding],[UserIdentityMethod],[UserId],[UserPassword],[UserCertificatePath],[MaximumDaysToRetainArchive],[EnforceSalesOrderLimit],[LeakDetectionQuietSamples],[LeakDetectionQuietTime],[LeakDetectionQuietTimeFactor],[LeakDetectionUseMinWait],[LeakDetectionReport],[LeakDetectionPrinter],[EnableAutomaticMovementTicketPrinting],[MovementTicketReport],[MovementTicketPrinter],[MaxOperateTabsAllowed],[CloseoutTime],[PointGroupFileExportDirectory],[PointGroupDefaultFileName],[EnableMovementTicketPDFArchiving],[MovementTicketFileExportDirectory],[MovementTicketExportFileName],[MovementNumber])
        ON (existingData.[SiteGuid] = remoteChanges.[SiteGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[Number] = remoteChanges.[Number]
                       ,[SPLCCode] = remoteChanges.[SPLCCode]
                       ,[Address1] = remoteChanges.[Address1]
                       ,[Address2] = remoteChanges.[Address2]
                       ,[City] = remoteChanges.[City]
                       ,[State] = remoteChanges.[State]
                       ,[Zip] = remoteChanges.[Zip]
                       ,[Country] = remoteChanges.[Country]
                       ,[Phone] = remoteChanges.[Phone]
                       ,[FAX] = remoteChanges.[FAX]
                       ,[EmailAddress] = remoteChanges.[EmailAddress]
                       ,[EmergencyContact] = remoteChanges.[EmergencyContact]
                       ,[EmergencyPhone] = remoteChanges.[EmergencyPhone]
                       ,[Enabled] = remoteChanges.[Enabled]
                       ,[SiteGroupFlag] = remoteChanges.[SiteGroupFlag]
                       ,[TimeZone] = remoteChanges.[TimeZone]
                       ,[LevelUnitIndex] = remoteChanges.[LevelUnitIndex]
                       ,[TemperatureUnitIndex] = remoteChanges.[TemperatureUnitIndex]
                       ,[DensityUnitIndex] = remoteChanges.[DensityUnitIndex]
                       ,[PressureUnitIndex] = remoteChanges.[PressureUnitIndex]
                       ,[FlowUnitIndex] = remoteChanges.[FlowUnitIndex]
                       ,[VolumeUnitIndex] = remoteChanges.[VolumeUnitIndex]
                       ,[MassUnitIndex] = remoteChanges.[MassUnitIndex]
                       ,[AdditiveVolumeUnitIndex] = remoteChanges.[AdditiveVolumeUnitIndex]
                       ,[AdditiveProfileCycleAmountUnitIndex] = remoteChanges.[AdditiveProfileCycleAmountUnitIndex]
                       ,[AdditiveProfileRateUnitIndex] = remoteChanges.[AdditiveProfileRateUnitIndex]
                       ,[LevelDecimalPlaces] = remoteChanges.[LevelDecimalPlaces]
                       ,[TemperatureDecimalPlaces] = remoteChanges.[TemperatureDecimalPlaces]
                       ,[DensityDecimalPlaces] = remoteChanges.[DensityDecimalPlaces]
                       ,[PressureDecimalPlaces] = remoteChanges.[PressureDecimalPlaces]
                       ,[FlowDecimalPlaces] = remoteChanges.[FlowDecimalPlaces]
                       ,[VolumeDecimalPlaces] = remoteChanges.[VolumeDecimalPlaces]
                       ,[MassDecimalPlaces] = remoteChanges.[MassDecimalPlaces]
                       ,[AdditiveVolumeDecimalPlaces] = remoteChanges.[AdditiveVolumeDecimalPlaces]
                       ,[AdditiveProfileCycleAmountDecimalPlaces] = remoteChanges.[AdditiveProfileCycleAmountDecimalPlaces]
                       ,[AdditiveProfileRateDecimalPlaces] = remoteChanges.[AdditiveProfileRateDecimalPlaces]
                       ,[InhibitAccessAfterHours] = remoteChanges.[InhibitAccessAfterHours]
                       ,[InhibitMultipleCardIns] = remoteChanges.[InhibitMultipleCardIns]
                       ,[AccessCardInRequired] = remoteChanges.[AccessCardInRequired]
                       ,[CheckSiteNumber] = remoteChanges.[CheckSiteNumber]
                       ,[PromptForCustomerCard] = remoteChanges.[PromptForCustomerCard]
                       ,[PromptForTractorOrTanker] = remoteChanges.[PromptForTractorOrTanker]
                       ,[PromptForFirstTrailer] = remoteChanges.[PromptForFirstTrailer]
                       ,[PromptForSecondTrailer] = remoteChanges.[PromptForSecondTrailer]
                       ,[PromptForCompartment] = remoteChanges.[PromptForCompartment]
                       ,[EnforceDriverEquipmentMatch] = remoteChanges.[EnforceDriverEquipmentMatch]
                       ,[EnableAdditiveAccounting] = remoteChanges.[EnableAdditiveAccounting]
                       ,[UseCompanyEquipmentIdentifiers] = remoteChanges.[UseCompanyEquipmentIdentifiers]
                       ,[UseLastKnownGoodTankData] = remoteChanges.[UseLastKnownGoodTankData]
                       ,[MaximumLoadAmount] = remoteChanges.[MaximumLoadAmount]
                       ,[MaximumLoadTime] = remoteChanges.[MaximumLoadTime]
                       ,[MaximumIdleTime] = remoteChanges.[MaximumIdleTime]
                       ,[MaximumFlushAmount] = remoteChanges.[MaximumFlushAmount]
                       ,[MaximumMeterProvingAmount] = remoteChanges.[MaximumMeterProvingAmount]
                       ,[MaximumReturnsAmount] = remoteChanges.[MaximumReturnsAmount]
                       ,[MaximumNumberOfActiveArms] = remoteChanges.[MaximumNumberOfActiveArms]
                       ,[DriverTimeoutPeriod] = remoteChanges.[DriverTimeoutPeriod]
                       ,[DriverWarningPeriod] = remoteChanges.[DriverWarningPeriod]
                       ,[MaximumPrompts] = remoteChanges.[MaximumPrompts]
                       ,[MaximumVehicleWeight] = remoteChanges.[MaximumVehicleWeight]
                       ,[LoadByNet] = remoteChanges.[LoadByNet]
                       ,[PromptForShipmentNumber] = remoteChanges.[PromptForShipmentNumber]
                       ,[MaximumProductTemperature] = remoteChanges.[MaximumProductTemperature]
                       ,[ListEquipment] = remoteChanges.[ListEquipment]
                       ,[DeferStationChanges] = remoteChanges.[DeferStationChanges]
                       ,[InhibitBOLWithBrokenBlends] = remoteChanges.[InhibitBOLWithBrokenBlends]
                       ,[InhibitBOLWithImproperAdditization] = remoteChanges.[InhibitBOLWithImproperAdditization]
                       ,[InhibitOverweightBOL] = remoteChanges.[InhibitOverweightBOL]
                       ,[ExceptionBOLPrinter] = remoteChanges.[ExceptionBOLPrinter]
                       ,[EnableAutomaticBOLPrinting] = remoteChanges.[EnableAutomaticBOLPrinting]
                       ,[AutomaticBOLStartNumber] = remoteChanges.[AutomaticBOLStartNumber]
                       ,[AutomaticBOLEndNumber] = remoteChanges.[AutomaticBOLEndNumber]
                       ,[SeparateManualBOLNumbering] = remoteChanges.[SeparateManualBOLNumbering]
                       ,[ManualBOLStartNumber] = remoteChanges.[ManualBOLStartNumber]
                       ,[ManualBOLEndNumber] = remoteChanges.[ManualBOLEndNumber]
                       ,[TransactionStartNumber] = remoteChanges.[TransactionStartNumber]
                       ,[TransactionEndNumber] = remoteChanges.[TransactionEndNumber]
                       ,[OrderStartNumber] = remoteChanges.[OrderStartNumber]
                       ,[OrderEndNumber] = remoteChanges.[OrderEndNumber]
                       ,[OpenTransactionWindow] = remoteChanges.[OpenTransactionWindow]
                       ,[AdministrativeLockDate] = remoteChanges.[AdministrativeLockDate]
                       ,[OperationalLockDate] = remoteChanges.[OperationalLockDate]
                       ,[MaximumDaysToRetainLogs] = remoteChanges.[MaximumDaysToRetainLogs]
                       ,[EnableDebugLogging] = remoteChanges.[EnableDebugLogging]
                       ,[EnableAuditLogging] = remoteChanges.[EnableAuditLogging]
                       ,[AutomaticallyPrintAlarmsAndEvents] = remoteChanges.[AutomaticallyPrintAlarmsAndEvents]
                       ,[AlarmAndEventPrinter] = remoteChanges.[AlarmAndEventPrinter]
                       ,[MailServer] = remoteChanges.[MailServer]
                       ,[MailFrom] = remoteChanges.[MailFrom]
                       ,[MailUserName] = remoteChanges.[MailUserName]
                       ,[MailPassword] = remoteChanges.[MailPassword]
                       ,[DialupName] = remoteChanges.[DialupName]
                       ,[SCADASystem] = remoteChanges.[SCADASystem]
                       ,[InhibitTemplateGraphics] = remoteChanges.[InhibitTemplateGraphics]
                       ,[RefreshInterval] = remoteChanges.[RefreshInterval]
                       ,[InhibitEndOfDayOperations] = remoteChanges.[InhibitEndOfDayOperations]
                       ,[InhibitEndOfMonthOperations] = remoteChanges.[InhibitEndOfMonthOperations]
                       ,[EndOfDayWarningPeriod] = remoteChanges.[EndOfDayWarningPeriod]
                       ,[InhibitAutomaticPhysicalInventory] = remoteChanges.[InhibitAutomaticPhysicalInventory]
                       ,[InhibitAutomaticMeterCloseout] = remoteChanges.[InhibitAutomaticMeterCloseout]
                       ,[InhibitAutomaticReportGeneration] = remoteChanges.[InhibitAutomaticReportGeneration]
                       ,[InhibitAutomaticAdjustmentDistribution] = remoteChanges.[InhibitAutomaticAdjustmentDistribution]
                       ,[InhibitAutomaticCloseout] = remoteChanges.[InhibitAutomaticCloseout]
                       ,[InhibitTankScan] = remoteChanges.[InhibitTankScan]
                       ,[ReportDirectory] = remoteChanges.[ReportDirectory]
                       ,[ManageReports] = remoteChanges.[ManageReports]
                       ,[ManagedReportDirectory] = remoteChanges.[ManagedReportDirectory]
                       ,[VRURateLimit] = remoteChanges.[VRURateLimit]
                       ,[VRUHourlyLimit] = remoteChanges.[VRUHourlyLimit]
                       ,[VRUDailyLimit] = remoteChanges.[VRUDailyLimit]
                       ,[VRUYearlyLimit] = remoteChanges.[VRUYearlyLimit]
                       ,[VRUCurrentYearLimit] = remoteChanges.[VRUCurrentYearLimit]
                       ,[VRURateActual] = remoteChanges.[VRURateActual]
                       ,[VRUHourlyActual] = remoteChanges.[VRUHourlyActual]
                       ,[VRUDailyActual] = remoteChanges.[VRUDailyActual]
                       ,[VRUYearlyActual] = remoteChanges.[VRUYearlyActual]
                       ,[VRUCurrentYearActual] = remoteChanges.[VRUCurrentYearActual]
                       ,[VRURateLimitEnabled] = remoteChanges.[VRURateLimitEnabled]
                       ,[VRUHourlyLimitEnabled] = remoteChanges.[VRUHourlyLimitEnabled]
                       ,[VRUDailyLimitEnabled] = remoteChanges.[VRUDailyLimitEnabled]
                       ,[VRUYearlyLimitEnabled] = remoteChanges.[VRUYearlyLimitEnabled]
                       ,[VRUCurrentYearLimitEnabled] = remoteChanges.[VRUCurrentYearLimitEnabled]
                       ,[WatchdogPeriod] = remoteChanges.[WatchdogPeriod]
                       ,[WatchdogCounterStart] = remoteChanges.[WatchdogCounterStart]
                       ,[WatchdogCounterEnd] = remoteChanges.[WatchdogCounterEnd]
                       ,[NumberDecimalSeparator] = remoteChanges.[NumberDecimalSeparator]
                       ,[NumberGroupSeparator] = remoteChanges.[NumberGroupSeparator]
                       ,[ListSeparator] = remoteChanges.[ListSeparator]
                       ,[TimePattern] = remoteChanges.[TimePattern]
                       ,[TimeSeparator] = remoteChanges.[TimeSeparator]
                       ,[AMSymbol] = remoteChanges.[AMSymbol]
                       ,[PMSymbol] = remoteChanges.[PMSymbol]
                       ,[ShortDatePattern] = remoteChanges.[ShortDatePattern]
                       ,[DateSeparator] = remoteChanges.[DateSeparator]
                       ,[LongDatePattern] = remoteChanges.[LongDatePattern]
                       ,[TwoDigitCalendarEndYear] = remoteChanges.[TwoDigitCalendarEndYear]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[MinTimeAllowedToChangePwd] = remoteChanges.[MinTimeAllowedToChangePwd]
                       ,[MinPwdCharacterLength] = remoteChanges.[MinPwdCharacterLength]
                       ,[PwdExpirationInDays] = remoteChanges.[PwdExpirationInDays]
                       ,[PwdLockoutThreshold] = remoteChanges.[PwdLockoutThreshold]
                       ,[CheckForPreviousPwd] = remoteChanges.[CheckForPreviousPwd]
                       ,[StrongPwdUse] = remoteChanges.[StrongPwdUse]
                       ,[PwdHistoryCount] = remoteChanges.[PwdHistoryCount]
                       ,[ApplyToAllSiteMembers] = remoteChanges.[ApplyToAllSiteMembers]
                       ,[InactivityDisablePeriod] = remoteChanges.[InactivityDisablePeriod]
                       ,[EnforceSingleOwner] = remoteChanges.[EnforceSingleOwner]
                       ,[InhibitBOLSummaryAutoPopulate] = remoteChanges.[InhibitBOLSummaryAutoPopulate]
                       ,[InhibitOrderSummaryAutoPopulate] = remoteChanges.[InhibitOrderSummaryAutoPopulate]
                       ,[InhibitSupplyOrderSummaryAutoPopulate] = remoteChanges.[InhibitSupplyOrderSummaryAutoPopulate]
                       ,[InvoiceStartNumber] = remoteChanges.[InvoiceStartNumber]
                       ,[InvoiceEndNumber] = remoteChanges.[InvoiceEndNumber]
                       ,[PromptForReturns] = remoteChanges.[PromptForReturns]
                       ,[PromptForTruckCard] = remoteChanges.[PromptForTruckCard]
                       ,[StartingShortCardNumber] = remoteChanges.[StartingShortCardNumber]
                       ,[UseShortCardNumber] = remoteChanges.[UseShortCardNumber]
                       ,[ExcessVarianceCount] = remoteChanges.[ExcessVarianceCount]
                       ,[ExcessVarianceTolerance] = remoteChanges.[ExcessVarianceTolerance]
                       ,[DisableArchivePeriod] = remoteChanges.[DisableArchivePeriod]
                       ,[ExportArchiveDir] = remoteChanges.[ExportArchiveDir]
                       ,[ImportArchiveDir] = remoteChanges.[ImportArchiveDir]
                       ,[GroupLedgerByID] = remoteChanges.[GroupLedgerByID]
                       ,[InhibitSiteLedgerRollup] = remoteChanges.[InhibitSiteLedgerRollup]
                       ,[UseTankReconciliation] = remoteChanges.[UseTankReconciliation]
                       ,[LookupNumberGroupSizesTypeIndex] = remoteChanges.[LookupNumberGroupSizesTypeIndex]
                       ,[LookupQuantityDisplayDefaultIndex] = remoteChanges.[LookupQuantityDisplayDefaultIndex]
                       ,[LookupSecondaryStorageFillMethodIndex] = remoteChanges.[LookupSecondaryStorageFillMethodIndex]
                       ,[LookupMailConnectModeIndex] = remoteChanges.[LookupMailConnectModeIndex]
                       ,[LookupWatchdogModeIndex] = remoteChanges.[LookupWatchdogModeIndex]
                       ,[Contact1Name] = remoteChanges.[Contact1Name]
                       ,[Contact1Address1] = remoteChanges.[Contact1Address1]
                       ,[Contact1Address2] = remoteChanges.[Contact1Address2]
                       ,[Contact1City] = remoteChanges.[Contact1City]
                       ,[Contact1State] = remoteChanges.[Contact1State]
                       ,[Contact1Zip] = remoteChanges.[Contact1Zip]
                       ,[Contact1Country] = remoteChanges.[Contact1Country]
                       ,[Contact1PhoneOffice] = remoteChanges.[Contact1PhoneOffice]
                       ,[Contact1Fax] = remoteChanges.[Contact1Fax]
                       ,[Contact1EmailAddress] = remoteChanges.[Contact1EmailAddress]
                       ,[Contact2Name] = remoteChanges.[Contact2Name]
                       ,[Contact2Address1] = remoteChanges.[Contact2Address1]
                       ,[Contact2Address2] = remoteChanges.[Contact2Address2]
                       ,[Contact2City] = remoteChanges.[Contact2City]
                       ,[Contact2State] = remoteChanges.[Contact2State]
                       ,[Contact2Zip] = remoteChanges.[Contact2Zip]
                       ,[Contact2Country] = remoteChanges.[Contact2Country]
                       ,[Contact2PhoneOffice] = remoteChanges.[Contact2PhoneOffice]
                       ,[Contact2Fax] = remoteChanges.[Contact2Fax]
                       ,[Contact2EmailAddress] = remoteChanges.[Contact2EmailAddress]
                       ,[Contact1PhoneMobile] = remoteChanges.[Contact1PhoneMobile]
                       ,[Contact2PhoneMobile] = remoteChanges.[Contact2PhoneMobile]
                       ,[EnablePasswordHint] = remoteChanges.[EnablePasswordHint]
                       ,[EnablePasswordReset] = remoteChanges.[EnablePasswordReset]
                       ,[MeterReconciliationToleranceIsPercent] = remoteChanges.[MeterReconciliationToleranceIsPercent]
                       ,[MeterReconciliationReportName] = remoteChanges.[MeterReconciliationReportName]
                       ,[TranslatedHelpURL] = remoteChanges.[TranslatedHelpURL]
                       ,[AllowUseOfSpecialChars] = remoteChanges.[AllowUseOfSpecialChars]
                       ,[EnablePeriodicSyncFlag] = remoteChanges.[EnablePeriodicSyncFlag]
                       ,[PeriodicSyncIntervalMinutes] = remoteChanges.[PeriodicSyncIntervalMinutes]
                       ,[DisableSyncTransferFlag] = remoteChanges.[DisableSyncTransferFlag]
                       ,[CardInTimeout] = remoteChanges.[CardInTimeout]
                       ,[TerminalControlNumber] = remoteChanges.[TerminalControlNumber]
                       ,[BlockCloseOnUnpostedBOL] = remoteChanges.[BlockCloseOnUnpostedBOL]
                       ,[InhibitLoadRackCardIns] = remoteChanges.[InhibitLoadRackCardIns]
                       ,[PromptForThirdTrailer] = remoteChanges.[PromptForThirdTrailer]
                       ,[PromptForTransactionCompletion] = remoteChanges.[PromptForTransactionCompletion]
                       ,[InhibitCustomerConfirmationPrompt] = remoteChanges.[InhibitCustomerConfirmationPrompt]
                       ,[EnableBOLPDFArchiving] = remoteChanges.[EnableBOLPDFArchiving]
                       ,[BOLPDFArchivingPath] = remoteChanges.[BOLPDFArchivingPath]
                       ,[RequireTrailerScully] = remoteChanges.[RequireTrailerScully]
                       ,[Latitude] = remoteChanges.[Latitude]
                       ,[Longitude] = remoteChanges.[Longitude]
                       ,[Zoom] = remoteChanges.[Zoom]
                       ,[GlobalAccessToPersonnel] = remoteChanges.[GlobalAccessToPersonnel]
                       ,[GlobalAccessToEquipment] = remoteChanges.[GlobalAccessToEquipment]
                       ,[Enterprise] = remoteChanges.[Enterprise]
                       ,[OperateTabGroups] = remoteChanges.[OperateTabGroups]
                       ,[EnterpriseUserId] = remoteChanges.[EnterpriseUserId]
                       ,[EnterprisePassword] = remoteChanges.[EnterprisePassword]
                       ,[EnterpriseSite] = remoteChanges.[EnterpriseSite]
                       ,[ActiveDirectorySiteGroupGuid] = remoteChanges.[ActiveDirectorySiteGroupGuid]
                       ,[ServerEndPoint] = remoteChanges.[ServerEndPoint]
                       ,[SecurityMode] = remoteChanges.[SecurityMode]
                       ,[SecurityPolicy] = remoteChanges.[SecurityPolicy]
                       ,[MessageEncoding] = remoteChanges.[MessageEncoding]
                       ,[UserIdentityMethod] = remoteChanges.[UserIdentityMethod]
                       ,[UserId] = remoteChanges.[UserId]
                       ,[UserPassword] = remoteChanges.[UserPassword]
                       ,[UserCertificatePath] = remoteChanges.[UserCertificatePath]
                       ,[MaximumDaysToRetainArchive] = remoteChanges.[MaximumDaysToRetainArchive]
                       ,[EnforceSalesOrderLimit] = remoteChanges.[EnforceSalesOrderLimit]
                       ,[LeakDetectionQuietSamples] = remoteChanges.[LeakDetectionQuietSamples]
                       ,[LeakDetectionQuietTime] = remoteChanges.[LeakDetectionQuietTime]
                       ,[LeakDetectionQuietTimeFactor] = remoteChanges.[LeakDetectionQuietTimeFactor]
                       ,[LeakDetectionUseMinWait] = remoteChanges.[LeakDetectionUseMinWait]
                       ,[LeakDetectionReport] = remoteChanges.[LeakDetectionReport]
                       ,[LeakDetectionPrinter] = remoteChanges.[LeakDetectionPrinter]
                       ,[EnableAutomaticMovementTicketPrinting] = remoteChanges.[EnableAutomaticMovementTicketPrinting]
                       ,[MovementTicketReport] = remoteChanges.[MovementTicketReport]
                       ,[MovementTicketPrinter] = remoteChanges.[MovementTicketPrinter]
                       ,[MaxOperateTabsAllowed] = remoteChanges.[MaxOperateTabsAllowed]
                       ,[CloseoutTime] = remoteChanges.[CloseoutTime]
                       ,[PointGroupFileExportDirectory] = remoteChanges.[PointGroupFileExportDirectory]
                       ,[PointGroupDefaultFileName] = remoteChanges.[PointGroupDefaultFileName]
                       ,[EnableMovementTicketPDFArchiving] = remoteChanges.[EnableMovementTicketPDFArchiving]
                       ,[MovementTicketFileExportDirectory] = remoteChanges.[MovementTicketFileExportDirectory]
                       ,[MovementTicketExportFileName] = remoteChanges.[MovementTicketExportFileName]
                       ,[MovementNumber] = remoteChanges.[MovementNumber]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[Number],[SPLCCode],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmailAddress],[EmergencyContact],[EmergencyPhone],[Enabled],[SiteGroupFlag],[TimeZone],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[AdditiveProfileCycleAmountDecimalPlaces],[AdditiveProfileRateDecimalPlaces],[InhibitAccessAfterHours],[InhibitMultipleCardIns],[AccessCardInRequired],[CheckSiteNumber],[PromptForCustomerCard],[PromptForTractorOrTanker],[PromptForFirstTrailer],[PromptForSecondTrailer],[PromptForCompartment],[EnforceDriverEquipmentMatch],[EnableAdditiveAccounting],[UseCompanyEquipmentIdentifiers],[UseLastKnownGoodTankData],[MaximumLoadAmount],[MaximumLoadTime],[MaximumIdleTime],[MaximumFlushAmount],[MaximumMeterProvingAmount],[MaximumReturnsAmount],[MaximumNumberOfActiveArms],[DriverTimeoutPeriod],[DriverWarningPeriod],[MaximumPrompts],[MaximumVehicleWeight],[LoadByNet],[PromptForShipmentNumber],[MaximumProductTemperature],[ListEquipment],[DeferStationChanges],[InhibitBOLWithBrokenBlends],[InhibitBOLWithImproperAdditization],[InhibitOverweightBOL],[ExceptionBOLPrinter],[EnableAutomaticBOLPrinting],[AutomaticBOLStartNumber],[AutomaticBOLEndNumber],[SeparateManualBOLNumbering],[ManualBOLStartNumber],[ManualBOLEndNumber],[TransactionStartNumber],[TransactionEndNumber],[OrderStartNumber],[OrderEndNumber],[OpenTransactionWindow],[AdministrativeLockDate],[OperationalLockDate],[MaximumDaysToRetainLogs],[EnableDebugLogging],[EnableAuditLogging],[AutomaticallyPrintAlarmsAndEvents],[AlarmAndEventPrinter],[MailServer],[MailFrom],[MailUserName],[MailPassword],[DialupName],[SCADASystem],[InhibitTemplateGraphics],[RefreshInterval],[InhibitEndOfDayOperations],[InhibitEndOfMonthOperations],[EndOfDayWarningPeriod],[InhibitAutomaticPhysicalInventory],[InhibitAutomaticMeterCloseout],[InhibitAutomaticReportGeneration],[InhibitAutomaticAdjustmentDistribution],[InhibitAutomaticCloseout],[InhibitTankScan],[ReportDirectory],[ManageReports],[ManagedReportDirectory],[VRURateLimit],[VRUHourlyLimit],[VRUDailyLimit],[VRUYearlyLimit],[VRUCurrentYearLimit],[VRURateActual],[VRUHourlyActual],[VRUDailyActual],[VRUYearlyActual],[VRUCurrentYearActual],[VRURateLimitEnabled],[VRUHourlyLimitEnabled],[VRUDailyLimitEnabled],[VRUYearlyLimitEnabled],[VRUCurrentYearLimitEnabled],[WatchdogPeriod],[WatchdogCounterStart],[WatchdogCounterEnd],[NumberDecimalSeparator],[NumberGroupSeparator],[ListSeparator],[TimePattern],[TimeSeparator],[AMSymbol],[PMSymbol],[ShortDatePattern],[DateSeparator],[LongDatePattern],[TwoDigitCalendarEndYear],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MinTimeAllowedToChangePwd],[MinPwdCharacterLength],[PwdExpirationInDays],[PwdLockoutThreshold],[CheckForPreviousPwd],[StrongPwdUse],[PwdHistoryCount],[ApplyToAllSiteMembers],[InactivityDisablePeriod],[EnforceSingleOwner],[InhibitBOLSummaryAutoPopulate],[InhibitOrderSummaryAutoPopulate],[InhibitSupplyOrderSummaryAutoPopulate],[InvoiceStartNumber],[InvoiceEndNumber],[PromptForReturns],[PromptForTruckCard],[StartingShortCardNumber],[UseShortCardNumber],[ExcessVarianceCount],[ExcessVarianceTolerance],[DisableArchivePeriod],[ExportArchiveDir],[ImportArchiveDir],[GroupLedgerByID],[InhibitSiteLedgerRollup],[UseTankReconciliation],[SiteGuid],[LookupNumberGroupSizesTypeIndex],[LookupQuantityDisplayDefaultIndex],[LookupSecondaryStorageFillMethodIndex],[LookupMailConnectModeIndex],[LookupWatchdogModeIndex],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[EnablePasswordHint],[EnablePasswordReset],[MeterReconciliationToleranceIsPercent],[MeterReconciliationReportName],[TranslatedHelpURL],[AllowUseOfSpecialChars],[EnablePeriodicSyncFlag],[PeriodicSyncIntervalMinutes],[DisableSyncTransferFlag],[CardInTimeout],[TerminalControlNumber],[BlockCloseOnUnpostedBOL],[InhibitLoadRackCardIns],[PromptForThirdTrailer],[PromptForTransactionCompletion],[InhibitCustomerConfirmationPrompt],[EnableBOLPDFArchiving],[BOLPDFArchivingPath],[RequireTrailerScully],[Latitude],[Longitude],[Zoom],[GlobalAccessToPersonnel],[GlobalAccessToEquipment],[Enterprise],[OperateTabGroups],[EnterpriseUserId],[EnterprisePassword],[EnterpriseSite],[ActiveDirectorySiteGroupGuid],[ServerEndPoint],[SecurityMode],[SecurityPolicy],[MessageEncoding],[UserIdentityMethod],[UserId],[UserPassword],[UserCertificatePath],[MaximumDaysToRetainArchive],[EnforceSalesOrderLimit],[LeakDetectionQuietSamples],[LeakDetectionQuietTime],[LeakDetectionQuietTimeFactor],[LeakDetectionUseMinWait],[LeakDetectionReport],[LeakDetectionPrinter],[EnableAutomaticMovementTicketPrinting],[MovementTicketReport],[MovementTicketPrinter],[MaxOperateTabsAllowed],[CloseoutTime],[PointGroupFileExportDirectory],[PointGroupDefaultFileName],[EnableMovementTicketPDFArchiving],[MovementTicketFileExportDirectory],[MovementTicketExportFileName],[MovementNumber])
                VALUES (@ID,@Number,@SPLCCode,@Address1,@Address2,@City,@State,@Zip,@Country,@Phone,@FAX,@EmailAddress,@EmergencyContact,@EmergencyPhone,@Enabled,@SiteGroupFlag,@TimeZone,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@AdditiveVolumeUnitIndex,@AdditiveProfileCycleAmountUnitIndex,@AdditiveProfileRateUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@AdditiveVolumeDecimalPlaces,@AdditiveProfileCycleAmountDecimalPlaces,@AdditiveProfileRateDecimalPlaces,@InhibitAccessAfterHours,@InhibitMultipleCardIns,@AccessCardInRequired,@CheckSiteNumber,@PromptForCustomerCard,@PromptForTractorOrTanker,@PromptForFirstTrailer,@PromptForSecondTrailer,@PromptForCompartment,@EnforceDriverEquipmentMatch,@EnableAdditiveAccounting,@UseCompanyEquipmentIdentifiers,@UseLastKnownGoodTankData,@MaximumLoadAmount,@MaximumLoadTime,@MaximumIdleTime,@MaximumFlushAmount,@MaximumMeterProvingAmount,@MaximumReturnsAmount,@MaximumNumberOfActiveArms,@DriverTimeoutPeriod,@DriverWarningPeriod,@MaximumPrompts,@MaximumVehicleWeight,@LoadByNet,@PromptForShipmentNumber,@MaximumProductTemperature,@ListEquipment,@DeferStationChanges,@InhibitBOLWithBrokenBlends,@InhibitBOLWithImproperAdditization,@InhibitOverweightBOL,@ExceptionBOLPrinter,@EnableAutomaticBOLPrinting,@AutomaticBOLStartNumber,@AutomaticBOLEndNumber,@SeparateManualBOLNumbering,@ManualBOLStartNumber,@ManualBOLEndNumber,@TransactionStartNumber,@TransactionEndNumber,@OrderStartNumber,@OrderEndNumber,@OpenTransactionWindow,@AdministrativeLockDate,@OperationalLockDate,@MaximumDaysToRetainLogs,@EnableDebugLogging,@EnableAuditLogging,@AutomaticallyPrintAlarmsAndEvents,@AlarmAndEventPrinter,@MailServer,@MailFrom,@MailUserName,@MailPassword,@DialupName,@SCADASystem,@InhibitTemplateGraphics,@RefreshInterval,@InhibitEndOfDayOperations,@InhibitEndOfMonthOperations,@EndOfDayWarningPeriod,@InhibitAutomaticPhysicalInventory,@InhibitAutomaticMeterCloseout,@InhibitAutomaticReportGeneration,@InhibitAutomaticAdjustmentDistribution,@InhibitAutomaticCloseout,@InhibitTankScan,@ReportDirectory,@ManageReports,@ManagedReportDirectory,@VRURateLimit,@VRUHourlyLimit,@VRUDailyLimit,@VRUYearlyLimit,@VRUCurrentYearLimit,@VRURateActual,@VRUHourlyActual,@VRUDailyActual,@VRUYearlyActual,@VRUCurrentYearActual,@VRURateLimitEnabled,@VRUHourlyLimitEnabled,@VRUDailyLimitEnabled,@VRUYearlyLimitEnabled,@VRUCurrentYearLimitEnabled,@WatchdogPeriod,@WatchdogCounterStart,@WatchdogCounterEnd,@NumberDecimalSeparator,@NumberGroupSeparator,@ListSeparator,@TimePattern,@TimeSeparator,@AMSymbol,@PMSymbol,@ShortDatePattern,@DateSeparator,@LongDatePattern,@TwoDigitCalendarEndYear,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@MinTimeAllowedToChangePwd,@MinPwdCharacterLength,@PwdExpirationInDays,@PwdLockoutThreshold,@CheckForPreviousPwd,@StrongPwdUse,@PwdHistoryCount,@ApplyToAllSiteMembers,@InactivityDisablePeriod,@EnforceSingleOwner,@InhibitBOLSummaryAutoPopulate,@InhibitOrderSummaryAutoPopulate,@InhibitSupplyOrderSummaryAutoPopulate,@InvoiceStartNumber,@InvoiceEndNumber,@PromptForReturns,@PromptForTruckCard,@StartingShortCardNumber,@UseShortCardNumber,@ExcessVarianceCount,@ExcessVarianceTolerance,@DisableArchivePeriod,@ExportArchiveDir,@ImportArchiveDir,@GroupLedgerByID,@InhibitSiteLedgerRollup,@UseTankReconciliation,@SiteGuid,@LookupNumberGroupSizesTypeIndex,@LookupQuantityDisplayDefaultIndex,@LookupSecondaryStorageFillMethodIndex,@LookupMailConnectModeIndex,@LookupWatchdogModeIndex,@Contact1Name,@Contact1Address1,@Contact1Address2,@Contact1City,@Contact1State,@Contact1Zip,@Contact1Country,@Contact1PhoneOffice,@Contact1Fax,@Contact1EmailAddress,@Contact2Name,@Contact2Address1,@Contact2Address2,@Contact2City,@Contact2State,@Contact2Zip,@Contact2Country,@Contact2PhoneOffice,@Contact2Fax,@Contact2EmailAddress,@Contact1PhoneMobile,@Contact2PhoneMobile,@EnablePasswordHint,@EnablePasswordReset,@MeterReconciliationToleranceIsPercent,@MeterReconciliationReportName,@TranslatedHelpURL,@AllowUseOfSpecialChars,@EnablePeriodicSyncFlag,@PeriodicSyncIntervalMinutes,@DisableSyncTransferFlag,@CardInTimeout,@TerminalControlNumber,@BlockCloseOnUnpostedBOL,@InhibitLoadRackCardIns,@PromptForThirdTrailer,@PromptForTransactionCompletion,@InhibitCustomerConfirmationPrompt,@EnableBOLPDFArchiving,@BOLPDFArchivingPath,@RequireTrailerScully,@Latitude,@Longitude,@Zoom,@GlobalAccessToPersonnel,@GlobalAccessToEquipment,@Enterprise,@OperateTabGroups,@EnterpriseUserId,@EnterprisePassword,@EnterpriseSite,@ActiveDirectorySiteGroupGuid,@ServerEndPoint,@SecurityMode,@SecurityPolicy,@MessageEncoding,@UserIdentityMethod,@UserId,@UserPassword,@UserCertificatePath,@MaximumDaysToRetainArchive,@EnforceSalesOrderLimit,@LeakDetectionQuietSamples,@LeakDetectionQuietTime,@LeakDetectionQuietTimeFactor,@LeakDetectionUseMinWait,@LeakDetectionReport,@LeakDetectionPrinter,@EnableAutomaticMovementTicketPrinting,@MovementTicketReport,@MovementTicketPrinter,@MaxOperateTabsAllowed,@CloseoutTime,@PointGroupFileExportDirectory,@PointGroupDefaultFileName,@EnableMovementTicketPDFArchiving,@MovementTicketFileExportDirectory,@MovementTicketExportFileName,@MovementNumber)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @SiteGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @SiteGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @SiteGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblSites] WHERE SiteGuid = @SiteGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
