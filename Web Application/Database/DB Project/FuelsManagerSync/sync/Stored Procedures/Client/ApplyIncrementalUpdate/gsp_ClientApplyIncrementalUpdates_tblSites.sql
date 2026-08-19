-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblSites
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblSites]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblSites] CT
                        WHERE CT.PK_SiteGuid = @SiteGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblSites].[ID],[dbo].[tblSites].[Number],[dbo].[tblSites].[SPLCCode],[dbo].[tblSites].[Address1],[dbo].[tblSites].[Address2],[dbo].[tblSites].[City],[dbo].[tblSites].[State],[dbo].[tblSites].[Zip],[dbo].[tblSites].[Country],[dbo].[tblSites].[Phone],[dbo].[tblSites].[FAX],[dbo].[tblSites].[EmailAddress],[dbo].[tblSites].[EmergencyContact],[dbo].[tblSites].[EmergencyPhone],[dbo].[tblSites].[Enabled],[dbo].[tblSites].[SiteGroupFlag],[dbo].[tblSites].[TimeZone],[dbo].[tblSites].[LevelUnitIndex],[dbo].[tblSites].[TemperatureUnitIndex],[dbo].[tblSites].[DensityUnitIndex],[dbo].[tblSites].[PressureUnitIndex],[dbo].[tblSites].[FlowUnitIndex],[dbo].[tblSites].[VolumeUnitIndex],[dbo].[tblSites].[MassUnitIndex],[dbo].[tblSites].[AdditiveVolumeUnitIndex],[dbo].[tblSites].[AdditiveProfileCycleAmountUnitIndex],[dbo].[tblSites].[AdditiveProfileRateUnitIndex],[dbo].[tblSites].[LevelDecimalPlaces],[dbo].[tblSites].[TemperatureDecimalPlaces],[dbo].[tblSites].[DensityDecimalPlaces],[dbo].[tblSites].[PressureDecimalPlaces],[dbo].[tblSites].[FlowDecimalPlaces],[dbo].[tblSites].[VolumeDecimalPlaces],[dbo].[tblSites].[MassDecimalPlaces],[dbo].[tblSites].[AdditiveVolumeDecimalPlaces],[dbo].[tblSites].[AdditiveProfileCycleAmountDecimalPlaces],[dbo].[tblSites].[AdditiveProfileRateDecimalPlaces],[dbo].[tblSites].[InhibitAccessAfterHours],[dbo].[tblSites].[InhibitMultipleCardIns],[dbo].[tblSites].[AccessCardInRequired],[dbo].[tblSites].[CheckSiteNumber],[dbo].[tblSites].[PromptForCustomerCard],[dbo].[tblSites].[PromptForTractorOrTanker],[dbo].[tblSites].[PromptForFirstTrailer],[dbo].[tblSites].[PromptForSecondTrailer],[dbo].[tblSites].[PromptForCompartment],[dbo].[tblSites].[EnforceDriverEquipmentMatch],[dbo].[tblSites].[EnableAdditiveAccounting],[dbo].[tblSites].[UseCompanyEquipmentIdentifiers],[dbo].[tblSites].[UseLastKnownGoodTankData],[dbo].[tblSites].[MaximumLoadAmount],[dbo].[tblSites].[MaximumLoadTime],[dbo].[tblSites].[MaximumIdleTime],[dbo].[tblSites].[MaximumFlushAmount],[dbo].[tblSites].[MaximumMeterProvingAmount],[dbo].[tblSites].[MaximumReturnsAmount],[dbo].[tblSites].[MaximumNumberOfActiveArms],[dbo].[tblSites].[DriverTimeoutPeriod],[dbo].[tblSites].[DriverWarningPeriod],[dbo].[tblSites].[MaximumPrompts],[dbo].[tblSites].[MaximumVehicleWeight],[dbo].[tblSites].[LoadByNet],[dbo].[tblSites].[PromptForShipmentNumber],[dbo].[tblSites].[MaximumProductTemperature],[dbo].[tblSites].[ListEquipment],[dbo].[tblSites].[DeferStationChanges],[dbo].[tblSites].[InhibitBOLWithBrokenBlends],[dbo].[tblSites].[InhibitBOLWithImproperAdditization],[dbo].[tblSites].[InhibitOverweightBOL],[dbo].[tblSites].[ExceptionBOLPrinter],[dbo].[tblSites].[EnableAutomaticBOLPrinting],[dbo].[tblSites].[AutomaticBOLStartNumber],[dbo].[tblSites].[AutomaticBOLEndNumber],[dbo].[tblSites].[SeparateManualBOLNumbering],[dbo].[tblSites].[ManualBOLStartNumber],[dbo].[tblSites].[ManualBOLEndNumber],[dbo].[tblSites].[TransactionStartNumber],[dbo].[tblSites].[TransactionEndNumber],[dbo].[tblSites].[OrderStartNumber],[dbo].[tblSites].[OrderEndNumber],[dbo].[tblSites].[OpenTransactionWindow],[dbo].[tblSites].[AdministrativeLockDate],[dbo].[tblSites].[OperationalLockDate],[dbo].[tblSites].[MaximumDaysToRetainLogs],[dbo].[tblSites].[EnableDebugLogging],[dbo].[tblSites].[EnableAuditLogging],[dbo].[tblSites].[AutomaticallyPrintAlarmsAndEvents],[dbo].[tblSites].[AlarmAndEventPrinter],[dbo].[tblSites].[MailServer],[dbo].[tblSites].[MailFrom],[dbo].[tblSites].[MailUserName],[dbo].[tblSites].[MailPassword],[dbo].[tblSites].[DialupName],[dbo].[tblSites].[SCADASystem],[dbo].[tblSites].[InhibitTemplateGraphics],[dbo].[tblSites].[RefreshInterval],[dbo].[tblSites].[InhibitEndOfDayOperations],[dbo].[tblSites].[InhibitEndOfMonthOperations],[dbo].[tblSites].[EndOfDayWarningPeriod],[dbo].[tblSites].[InhibitAutomaticPhysicalInventory],[dbo].[tblSites].[InhibitAutomaticMeterCloseout],[dbo].[tblSites].[InhibitAutomaticReportGeneration],[dbo].[tblSites].[InhibitAutomaticAdjustmentDistribution],[dbo].[tblSites].[InhibitAutomaticCloseout],[dbo].[tblSites].[InhibitTankScan],[dbo].[tblSites].[ReportDirectory],[dbo].[tblSites].[ManageReports],[dbo].[tblSites].[ManagedReportDirectory],[dbo].[tblSites].[VRURateLimit],[dbo].[tblSites].[VRUHourlyLimit],[dbo].[tblSites].[VRUDailyLimit],[dbo].[tblSites].[VRUYearlyLimit],[dbo].[tblSites].[VRUCurrentYearLimit],[dbo].[tblSites].[VRURateActual],[dbo].[tblSites].[VRUHourlyActual],[dbo].[tblSites].[VRUDailyActual],[dbo].[tblSites].[VRUYearlyActual],[dbo].[tblSites].[VRUCurrentYearActual],[dbo].[tblSites].[VRURateLimitEnabled],[dbo].[tblSites].[VRUHourlyLimitEnabled],[dbo].[tblSites].[VRUDailyLimitEnabled],[dbo].[tblSites].[VRUYearlyLimitEnabled],[dbo].[tblSites].[VRUCurrentYearLimitEnabled],[dbo].[tblSites].[WatchdogPeriod],[dbo].[tblSites].[WatchdogCounterStart],[dbo].[tblSites].[WatchdogCounterEnd],[dbo].[tblSites].[NumberDecimalSeparator],[dbo].[tblSites].[NumberGroupSeparator],[dbo].[tblSites].[ListSeparator],[dbo].[tblSites].[TimePattern],[dbo].[tblSites].[TimeSeparator],[dbo].[tblSites].[AMSymbol],[dbo].[tblSites].[PMSymbol],[dbo].[tblSites].[ShortDatePattern],[dbo].[tblSites].[DateSeparator],[dbo].[tblSites].[LongDatePattern],[dbo].[tblSites].[TwoDigitCalendarEndYear],[dbo].[tblSites].[UserData1],[dbo].[tblSites].[UserData2],[dbo].[tblSites].[UserData3],[dbo].[tblSites].[UserData4],[dbo].[tblSites].[UserData5],[dbo].[tblSites].[UserData6],[dbo].[tblSites].[UserData7],[dbo].[tblSites].[UserData8],[dbo].[tblSites].[CreatedDate],[dbo].[tblSites].[CreatedBy],[dbo].[tblSites].[UpdatedDate],[dbo].[tblSites].[UpdatedBy],[dbo].[tblSites].[MinTimeAllowedToChangePwd],[dbo].[tblSites].[MinPwdCharacterLength],[dbo].[tblSites].[PwdExpirationInDays],[dbo].[tblSites].[PwdLockoutThreshold],[dbo].[tblSites].[CheckForPreviousPwd],[dbo].[tblSites].[StrongPwdUse],[dbo].[tblSites].[PwdHistoryCount],[dbo].[tblSites].[ApplyToAllSiteMembers],[dbo].[tblSites].[InactivityDisablePeriod],[dbo].[tblSites].[EnforceSingleOwner],[dbo].[tblSites].[InhibitBOLSummaryAutoPopulate],[dbo].[tblSites].[InhibitOrderSummaryAutoPopulate],[dbo].[tblSites].[InhibitSupplyOrderSummaryAutoPopulate],[dbo].[tblSites].[InvoiceStartNumber],[dbo].[tblSites].[InvoiceEndNumber],[dbo].[tblSites].[PromptForReturns],[dbo].[tblSites].[PromptForTruckCard],[dbo].[tblSites].[StartingShortCardNumber],[dbo].[tblSites].[UseShortCardNumber],[dbo].[tblSites].[ExcessVarianceCount],[dbo].[tblSites].[ExcessVarianceTolerance],[dbo].[tblSites].[DisableArchivePeriod],[dbo].[tblSites].[ExportArchiveDir],[dbo].[tblSites].[ImportArchiveDir],[dbo].[tblSites].[GroupLedgerByID],[dbo].[tblSites].[InhibitSiteLedgerRollup],[dbo].[tblSites].[UseTankReconciliation],[dbo].[tblSites].[SiteGuid],[dbo].[tblSites].[LookupNumberGroupSizesTypeIndex],[dbo].[tblSites].[LookupQuantityDisplayDefaultIndex],[dbo].[tblSites].[LookupSecondaryStorageFillMethodIndex],[dbo].[tblSites].[LookupMailConnectModeIndex],[dbo].[tblSites].[LookupWatchdogModeIndex],[dbo].[tblSites].[Contact1Name],[dbo].[tblSites].[Contact1Address1],[dbo].[tblSites].[Contact1Address2],[dbo].[tblSites].[Contact1City],[dbo].[tblSites].[Contact1State],[dbo].[tblSites].[Contact1Zip],[dbo].[tblSites].[Contact1Country],[dbo].[tblSites].[Contact1PhoneOffice],[dbo].[tblSites].[Contact1Fax],[dbo].[tblSites].[Contact1EmailAddress],[dbo].[tblSites].[Contact2Name],[dbo].[tblSites].[Contact2Address1],[dbo].[tblSites].[Contact2Address2],[dbo].[tblSites].[Contact2City],[dbo].[tblSites].[Contact2State],[dbo].[tblSites].[Contact2Zip],[dbo].[tblSites].[Contact2Country],[dbo].[tblSites].[Contact2PhoneOffice],[dbo].[tblSites].[Contact2Fax],[dbo].[tblSites].[Contact2EmailAddress],[dbo].[tblSites].[Contact1PhoneMobile],[dbo].[tblSites].[Contact2PhoneMobile],[dbo].[tblSites].[EnablePasswordHint],[dbo].[tblSites].[EnablePasswordReset],[dbo].[tblSites].[MeterReconciliationToleranceIsPercent],[dbo].[tblSites].[MeterReconciliationReportName],[dbo].[tblSites].[TranslatedHelpURL],[dbo].[tblSites].[AllowUseOfSpecialChars],[dbo].[tblSites].[EnablePeriodicSyncFlag],[dbo].[tblSites].[PeriodicSyncIntervalMinutes],[dbo].[tblSites].[DisableSyncTransferFlag],[dbo].[tblSites].[CardInTimeout],[dbo].[tblSites].[TerminalControlNumber],[dbo].[tblSites].[BlockCloseOnUnpostedBOL],[dbo].[tblSites].[InhibitLoadRackCardIns],[dbo].[tblSites].[PromptForThirdTrailer],[dbo].[tblSites].[PromptForTransactionCompletion],[dbo].[tblSites].[InhibitCustomerConfirmationPrompt],[dbo].[tblSites].[EnableBOLPDFArchiving],[dbo].[tblSites].[BOLPDFArchivingPath],[dbo].[tblSites].[RequireTrailerScully],[dbo].[tblSites].[Latitude],[dbo].[tblSites].[Longitude],[dbo].[tblSites].[Zoom],[dbo].[tblSites].[GlobalAccessToPersonnel],[dbo].[tblSites].[GlobalAccessToEquipment],[dbo].[tblSites].[Enterprise],[dbo].[tblSites].[OperateTabGroups],[dbo].[tblSites].[EnterpriseUserId],[dbo].[tblSites].[EnterprisePassword],[dbo].[tblSites].[EnterpriseSite],[dbo].[tblSites].[ActiveDirectorySiteGroupGuid],[dbo].[tblSites].[ServerEndPoint],[dbo].[tblSites].[SecurityMode],[dbo].[tblSites].[SecurityPolicy],[dbo].[tblSites].[MessageEncoding],[dbo].[tblSites].[UserIdentityMethod],[dbo].[tblSites].[UserId],[dbo].[tblSites].[UserPassword],[dbo].[tblSites].[UserCertificatePath],[dbo].[tblSites].[MaximumDaysToRetainArchive],[dbo].[tblSites].[EnforceSalesOrderLimit],[dbo].[tblSites].[LeakDetectionQuietSamples],[dbo].[tblSites].[LeakDetectionQuietTime],[dbo].[tblSites].[LeakDetectionQuietTimeFactor],[dbo].[tblSites].[LeakDetectionUseMinWait],[dbo].[tblSites].[LeakDetectionReport],[dbo].[tblSites].[LeakDetectionPrinter],[dbo].[tblSites].[EnableAutomaticMovementTicketPrinting],[dbo].[tblSites].[MovementTicketReport],[dbo].[tblSites].[MovementTicketPrinter],[dbo].[tblSites].[MaxOperateTabsAllowed],[dbo].[tblSites].[CloseoutTime],[dbo].[tblSites].[PointGroupFileExportDirectory],[dbo].[tblSites].[PointGroupDefaultFileName],[dbo].[tblSites].[EnableMovementTicketPDFArchiving],[dbo].[tblSites].[MovementTicketFileExportDirectory],[dbo].[tblSites].[MovementTicketExportFileName],[dbo].[tblSites].[MovementNumber]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblSites]
                        INNER JOIN [track].[tblSites] CT
                            ON CT.PK_SiteGuid = [dbo].[tblSites].[SiteGuid] 
                    WHERE CT.PK_SiteGuid = @SiteGuid
            ) MERGE existingData
            USING (SELECT @ID,@Number,@SPLCCode,@Address1,@Address2,@City,@State,@Zip,@Country,@Phone,@FAX,@EmailAddress,@EmergencyContact,@EmergencyPhone,@Enabled,@SiteGroupFlag,@TimeZone,@LevelUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@PressureUnitIndex,@FlowUnitIndex,@VolumeUnitIndex,@MassUnitIndex,@AdditiveVolumeUnitIndex,@AdditiveProfileCycleAmountUnitIndex,@AdditiveProfileRateUnitIndex,@LevelDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@PressureDecimalPlaces,@FlowDecimalPlaces,@VolumeDecimalPlaces,@MassDecimalPlaces,@AdditiveVolumeDecimalPlaces,@AdditiveProfileCycleAmountDecimalPlaces,@AdditiveProfileRateDecimalPlaces,@InhibitAccessAfterHours,@InhibitMultipleCardIns,@AccessCardInRequired,@CheckSiteNumber,@PromptForCustomerCard,@PromptForTractorOrTanker,@PromptForFirstTrailer,@PromptForSecondTrailer,@PromptForCompartment,@EnforceDriverEquipmentMatch,@EnableAdditiveAccounting,@UseCompanyEquipmentIdentifiers,@UseLastKnownGoodTankData,@MaximumLoadAmount,@MaximumLoadTime,@MaximumIdleTime,@MaximumFlushAmount,@MaximumMeterProvingAmount,@MaximumReturnsAmount,@MaximumNumberOfActiveArms,@DriverTimeoutPeriod,@DriverWarningPeriod,@MaximumPrompts,@MaximumVehicleWeight,@LoadByNet,@PromptForShipmentNumber,@MaximumProductTemperature,@ListEquipment,@DeferStationChanges,@InhibitBOLWithBrokenBlends,@InhibitBOLWithImproperAdditization,@InhibitOverweightBOL,@ExceptionBOLPrinter,@EnableAutomaticBOLPrinting,@AutomaticBOLStartNumber,@AutomaticBOLEndNumber,@SeparateManualBOLNumbering,@ManualBOLStartNumber,@ManualBOLEndNumber,@TransactionStartNumber,@TransactionEndNumber,@OrderStartNumber,@OrderEndNumber,@OpenTransactionWindow,@AdministrativeLockDate,@OperationalLockDate,@MaximumDaysToRetainLogs,@EnableDebugLogging,@EnableAuditLogging,@AutomaticallyPrintAlarmsAndEvents,@AlarmAndEventPrinter,@MailServer,@MailFrom,@MailUserName,@MailPassword,@DialupName,@SCADASystem,@InhibitTemplateGraphics,@RefreshInterval,@InhibitEndOfDayOperations,@InhibitEndOfMonthOperations,@EndOfDayWarningPeriod,@InhibitAutomaticPhysicalInventory,@InhibitAutomaticMeterCloseout,@InhibitAutomaticReportGeneration,@InhibitAutomaticAdjustmentDistribution,@InhibitAutomaticCloseout,@InhibitTankScan,@ReportDirectory,@ManageReports,@ManagedReportDirectory,@VRURateLimit,@VRUHourlyLimit,@VRUDailyLimit,@VRUYearlyLimit,@VRUCurrentYearLimit,@VRURateActual,@VRUHourlyActual,@VRUDailyActual,@VRUYearlyActual,@VRUCurrentYearActual,@VRURateLimitEnabled,@VRUHourlyLimitEnabled,@VRUDailyLimitEnabled,@VRUYearlyLimitEnabled,@VRUCurrentYearLimitEnabled,@WatchdogPeriod,@WatchdogCounterStart,@WatchdogCounterEnd,@NumberDecimalSeparator,@NumberGroupSeparator,@ListSeparator,@TimePattern,@TimeSeparator,@AMSymbol,@PMSymbol,@ShortDatePattern,@DateSeparator,@LongDatePattern,@TwoDigitCalendarEndYear,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@MinTimeAllowedToChangePwd,@MinPwdCharacterLength,@PwdExpirationInDays,@PwdLockoutThreshold,@CheckForPreviousPwd,@StrongPwdUse,@PwdHistoryCount,@ApplyToAllSiteMembers,@InactivityDisablePeriod,@EnforceSingleOwner,@InhibitBOLSummaryAutoPopulate,@InhibitOrderSummaryAutoPopulate,@InhibitSupplyOrderSummaryAutoPopulate,@InvoiceStartNumber,@InvoiceEndNumber,@PromptForReturns,@PromptForTruckCard,@StartingShortCardNumber,@UseShortCardNumber,@ExcessVarianceCount,@ExcessVarianceTolerance,@DisableArchivePeriod,@ExportArchiveDir,@ImportArchiveDir,@GroupLedgerByID,@InhibitSiteLedgerRollup,@UseTankReconciliation,@SiteGuid,@LookupNumberGroupSizesTypeIndex,@LookupQuantityDisplayDefaultIndex,@LookupSecondaryStorageFillMethodIndex,@LookupMailConnectModeIndex,@LookupWatchdogModeIndex,@Contact1Name,@Contact1Address1,@Contact1Address2,@Contact1City,@Contact1State,@Contact1Zip,@Contact1Country,@Contact1PhoneOffice,@Contact1Fax,@Contact1EmailAddress,@Contact2Name,@Contact2Address1,@Contact2Address2,@Contact2City,@Contact2State,@Contact2Zip,@Contact2Country,@Contact2PhoneOffice,@Contact2Fax,@Contact2EmailAddress,@Contact1PhoneMobile,@Contact2PhoneMobile,@EnablePasswordHint,@EnablePasswordReset,@MeterReconciliationToleranceIsPercent,@MeterReconciliationReportName,@TranslatedHelpURL,@AllowUseOfSpecialChars,@EnablePeriodicSyncFlag,@PeriodicSyncIntervalMinutes,@DisableSyncTransferFlag,@CardInTimeout,@TerminalControlNumber,@BlockCloseOnUnpostedBOL,@InhibitLoadRackCardIns,@PromptForThirdTrailer,@PromptForTransactionCompletion,@InhibitCustomerConfirmationPrompt,@EnableBOLPDFArchiving,@BOLPDFArchivingPath,@RequireTrailerScully,@Latitude,@Longitude,@Zoom,@GlobalAccessToPersonnel,@GlobalAccessToEquipment,@Enterprise,@OperateTabGroups,@EnterpriseUserId,@EnterprisePassword,@EnterpriseSite,@ActiveDirectorySiteGroupGuid,@ServerEndPoint,@SecurityMode,@SecurityPolicy,@MessageEncoding,@UserIdentityMethod,@UserId,@UserPassword,@UserCertificatePath,@MaximumDaysToRetainArchive,@EnforceSalesOrderLimit,@LeakDetectionQuietSamples,@LeakDetectionQuietTime,@LeakDetectionQuietTimeFactor,@LeakDetectionUseMinWait,@LeakDetectionReport,@LeakDetectionPrinter,@EnableAutomaticMovementTicketPrinting,@MovementTicketReport,@MovementTicketPrinter,@MaxOperateTabsAllowed,@CloseoutTime,@PointGroupFileExportDirectory,@PointGroupDefaultFileName,@EnableMovementTicketPDFArchiving,@MovementTicketFileExportDirectory,@MovementTicketExportFileName,@MovementNumber
                    ) AS remoteChanges ([ID],[Number],[SPLCCode],[Address1],[Address2],[City],[State],[Zip],[Country],[Phone],[FAX],[EmailAddress],[EmergencyContact],[EmergencyPhone],[Enabled],[SiteGroupFlag],[TimeZone],[LevelUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[PressureUnitIndex],[FlowUnitIndex],[VolumeUnitIndex],[MassUnitIndex],[AdditiveVolumeUnitIndex],[AdditiveProfileCycleAmountUnitIndex],[AdditiveProfileRateUnitIndex],[LevelDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[PressureDecimalPlaces],[FlowDecimalPlaces],[VolumeDecimalPlaces],[MassDecimalPlaces],[AdditiveVolumeDecimalPlaces],[AdditiveProfileCycleAmountDecimalPlaces],[AdditiveProfileRateDecimalPlaces],[InhibitAccessAfterHours],[InhibitMultipleCardIns],[AccessCardInRequired],[CheckSiteNumber],[PromptForCustomerCard],[PromptForTractorOrTanker],[PromptForFirstTrailer],[PromptForSecondTrailer],[PromptForCompartment],[EnforceDriverEquipmentMatch],[EnableAdditiveAccounting],[UseCompanyEquipmentIdentifiers],[UseLastKnownGoodTankData],[MaximumLoadAmount],[MaximumLoadTime],[MaximumIdleTime],[MaximumFlushAmount],[MaximumMeterProvingAmount],[MaximumReturnsAmount],[MaximumNumberOfActiveArms],[DriverTimeoutPeriod],[DriverWarningPeriod],[MaximumPrompts],[MaximumVehicleWeight],[LoadByNet],[PromptForShipmentNumber],[MaximumProductTemperature],[ListEquipment],[DeferStationChanges],[InhibitBOLWithBrokenBlends],[InhibitBOLWithImproperAdditization],[InhibitOverweightBOL],[ExceptionBOLPrinter],[EnableAutomaticBOLPrinting],[AutomaticBOLStartNumber],[AutomaticBOLEndNumber],[SeparateManualBOLNumbering],[ManualBOLStartNumber],[ManualBOLEndNumber],[TransactionStartNumber],[TransactionEndNumber],[OrderStartNumber],[OrderEndNumber],[OpenTransactionWindow],[AdministrativeLockDate],[OperationalLockDate],[MaximumDaysToRetainLogs],[EnableDebugLogging],[EnableAuditLogging],[AutomaticallyPrintAlarmsAndEvents],[AlarmAndEventPrinter],[MailServer],[MailFrom],[MailUserName],[MailPassword],[DialupName],[SCADASystem],[InhibitTemplateGraphics],[RefreshInterval],[InhibitEndOfDayOperations],[InhibitEndOfMonthOperations],[EndOfDayWarningPeriod],[InhibitAutomaticPhysicalInventory],[InhibitAutomaticMeterCloseout],[InhibitAutomaticReportGeneration],[InhibitAutomaticAdjustmentDistribution],[InhibitAutomaticCloseout],[InhibitTankScan],[ReportDirectory],[ManageReports],[ManagedReportDirectory],[VRURateLimit],[VRUHourlyLimit],[VRUDailyLimit],[VRUYearlyLimit],[VRUCurrentYearLimit],[VRURateActual],[VRUHourlyActual],[VRUDailyActual],[VRUYearlyActual],[VRUCurrentYearActual],[VRURateLimitEnabled],[VRUHourlyLimitEnabled],[VRUDailyLimitEnabled],[VRUYearlyLimitEnabled],[VRUCurrentYearLimitEnabled],[WatchdogPeriod],[WatchdogCounterStart],[WatchdogCounterEnd],[NumberDecimalSeparator],[NumberGroupSeparator],[ListSeparator],[TimePattern],[TimeSeparator],[AMSymbol],[PMSymbol],[ShortDatePattern],[DateSeparator],[LongDatePattern],[TwoDigitCalendarEndYear],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MinTimeAllowedToChangePwd],[MinPwdCharacterLength],[PwdExpirationInDays],[PwdLockoutThreshold],[CheckForPreviousPwd],[StrongPwdUse],[PwdHistoryCount],[ApplyToAllSiteMembers],[InactivityDisablePeriod],[EnforceSingleOwner],[InhibitBOLSummaryAutoPopulate],[InhibitOrderSummaryAutoPopulate],[InhibitSupplyOrderSummaryAutoPopulate],[InvoiceStartNumber],[InvoiceEndNumber],[PromptForReturns],[PromptForTruckCard],[StartingShortCardNumber],[UseShortCardNumber],[ExcessVarianceCount],[ExcessVarianceTolerance],[DisableArchivePeriod],[ExportArchiveDir],[ImportArchiveDir],[GroupLedgerByID],[InhibitSiteLedgerRollup],[UseTankReconciliation],[SiteGuid],[LookupNumberGroupSizesTypeIndex],[LookupQuantityDisplayDefaultIndex],[LookupSecondaryStorageFillMethodIndex],[LookupMailConnectModeIndex],[LookupWatchdogModeIndex],[Contact1Name],[Contact1Address1],[Contact1Address2],[Contact1City],[Contact1State],[Contact1Zip],[Contact1Country],[Contact1PhoneOffice],[Contact1Fax],[Contact1EmailAddress],[Contact2Name],[Contact2Address1],[Contact2Address2],[Contact2City],[Contact2State],[Contact2Zip],[Contact2Country],[Contact2PhoneOffice],[Contact2Fax],[Contact2EmailAddress],[Contact1PhoneMobile],[Contact2PhoneMobile],[EnablePasswordHint],[EnablePasswordReset],[MeterReconciliationToleranceIsPercent],[MeterReconciliationReportName],[TranslatedHelpURL],[AllowUseOfSpecialChars],[EnablePeriodicSyncFlag],[PeriodicSyncIntervalMinutes],[DisableSyncTransferFlag],[CardInTimeout],[TerminalControlNumber],[BlockCloseOnUnpostedBOL],[InhibitLoadRackCardIns],[PromptForThirdTrailer],[PromptForTransactionCompletion],[InhibitCustomerConfirmationPrompt],[EnableBOLPDFArchiving],[BOLPDFArchivingPath],[RequireTrailerScully],[Latitude],[Longitude],[Zoom],[GlobalAccessToPersonnel],[GlobalAccessToEquipment],[Enterprise],[OperateTabGroups],[EnterpriseUserId],[EnterprisePassword],[EnterpriseSite],[ActiveDirectorySiteGroupGuid],[ServerEndPoint],[SecurityMode],[SecurityPolicy],[MessageEncoding],[UserIdentityMethod],[UserId],[UserPassword],[UserCertificatePath],[MaximumDaysToRetainArchive],[EnforceSalesOrderLimit],[LeakDetectionQuietSamples],[LeakDetectionQuietTime],[LeakDetectionQuietTimeFactor],[LeakDetectionUseMinWait],[LeakDetectionReport],[LeakDetectionPrinter],[EnableAutomaticMovementTicketPrinting],[MovementTicketReport],[MovementTicketPrinter],[MaxOperateTabsAllowed],[CloseoutTime],[PointGroupFileExportDirectory],[PointGroupDefaultFileName],[EnableMovementTicketPDFArchiving],[MovementTicketFileExportDirectory],[MovementTicketExportFileName],[MovementNumber])
            ON (existingData.[SiteGuid] = remoteChanges.[SiteGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END
    
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
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
