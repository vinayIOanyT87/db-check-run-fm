USE [msdb]
GO

/****** Object:  Job [Seed Memory with TransactionData]    Script Date: 4/15/2015 12:18:18 PM ******/
BEGIN TRANSACTION
DECLARE @ReturnCode INT
SELECT @ReturnCode = 0
/****** Object:  JobCategory [[Uncategorized (Local)]]    Script Date: 4/15/2015 12:18:18 PM ******/
IF NOT EXISTS (SELECT name FROM msdb.dbo.syscategories WHERE name=N'[Uncategorized (Local)]' AND category_class=1)
BEGIN
EXEC @ReturnCode = msdb.dbo.sp_add_category @class=N'JOB', @type=N'LOCAL', @name=N'[Uncategorized (Local)]'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback

END

DECLARE @jobId BINARY(16)
EXEC @ReturnCode =  msdb.dbo.sp_add_job @job_name=N'Seed Memory with TransactionData', 
		@enabled=1, 
		@notify_level_eventlog=0, 
		@notify_level_email=0, 
		@notify_level_netsend=0, 
		@notify_level_page=0, 
		@delete_level=0, 
		@description=N'No description available.', 
		@category_name=N'[Uncategorized (Local)]', 
		@owner_login_name=N'sa', @job_id = @jobId OUTPUT
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Load All Reference Data]    Script Date: 4/15/2015 12:18:18 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Load All Reference Data', 
		@step_id=1, 
		@cmdexec_success_code=0, 
		@on_success_action=3, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/* tblAdditiveProfiles */
Select * from [dbo].[tblAdditiveProfiles]
GO

/* tblAirplaneTank */
Select * from [dbo].[tblAirplaneTank]
GO

/* tblAlarmAndEventLog */
Select * from [dbo].[tblAlarmAndEventLog]
GO

/* tblAlarmAndEvents */
Select * from [dbo].[tblAlarmAndEvents]
GO

/* tblAlarmPriorities */
Select * from [dbo].[tblAlarmPriorities]
GO

/* tblAllocationLineItems */
Select * from [dbo].[tblAllocationLineItems]
GO

/* tblAllocations */
Select * from [dbo].[tblAllocations]
GO

/* tblApplicationString */
Select * from [dbo].[tblApplicationString]
GO

/* tblAppointmentEquipment */
Select * from [dbo].[tblAppointmentEquipment]
GO

/* tblAppointmentPersonnel */
Select * from [dbo].[tblAppointmentPersonnel]
GO

/* tblAppointmentTank */
Select * from [dbo].[tblAppointmentTank]
GO

/* tblArchivedUsers */
Select * from [dbo].[tblArchivedUsers]
GO

/* tblAuditHandler */
Select * from [dbo].[tblAuditHandler]
GO

/* tblAuditLog */
Select * from [dbo].[tblAuditLog]
GO

/* tblAutoDistributionReasonCodes */
Select * from [dbo].[tblAutoDistributionReasonCodes]
GO

/* tblAutoDistributionRule */
Select * from [dbo].[tblAutoDistributionRule]
GO

/* tblB2BResults */
Select * from [dbo].[tblB2BResults]
GO

/* tblBulkPaymentLinks */
Select * from [dbo].[tblBulkPaymentLinks]
GO

/* tblBulkPayments */
Select * from [dbo].[tblBulkPayments]
GO

/* tblChangeLog */
Select * from [dbo].[tblChangeLog]
GO

/* tblChangesQueue */
Select * from [dbo].[tblChangesQueue]
GO

/* tblCloseoutInventory */
Select * from [dbo].[tblCloseoutInventory]
GO

/* tblCompanies */
Select * from [dbo].[tblCompanies]
GO

/* tblCompanyCrossReference */
Select * from [dbo].[tblCompanyCrossReference]
GO

/* tblCompanyCrossReferenceMap */
Select * from [dbo].[tblCompanyCrossReferenceMap]
GO

/* tblConfigurationSetting */
Select * from [dbo].[tblConfigurationSetting]
GO

/* tblControllersLog */
Select * from [dbo].[tblControllersLog]
GO

/* tblCurrencies */
Select * from [dbo].[tblCurrencies]
GO

/* tblCurrencyLineItems */
Select * from [dbo].[tblCurrencyLineItems]
GO

/* tblCustomToolbar */
Select * from [dbo].[tblCustomToolbar]
GO

/* tblCustomToolbarCommand */
Select * from [dbo].[tblCustomToolbarCommand]
GO

/* tblDataDictionaries */
Select * from [dbo].[tblDataDictionaries]
GO

/* tblDispatchConfiguration */
Select * from [dbo].[tblDispatchConfiguration]
GO

/* tblDispatchGrid */
Select * from [dbo].[tblDispatchGrid]
GO

/* tblDispatchGridColumn */
Select * from [dbo].[tblDispatchGridColumn]
GO

/* tblEmailGroups */
Select * from [dbo].[tblEmailGroups]
GO

/* tblEnterpriseQueue */
Select * from [dbo].[tblEnterpriseQueue]
GO

/* tblEquipment */
Select * from [dbo].[tblEquipment]
GO

/* tblEquipmentMaintenanceLog */
Select * from [dbo].[tblEquipmentMaintenanceLog]
GO

/* tblEquipmentQualityTagLog */
Select * from [dbo].[tblEquipmentQualityTagLog]
GO

/* tblEquipmentTypes */
Select * from [dbo].[tblEquipmentTypes]
GO

/* tblExcise */
Select * from [dbo].[tblExcise]
GO

/* tblExportPaiceTransTracking */
Select * from [dbo].[tblExportPaiceTransTracking]
GO

/* tblExportRequest */
Select * from [dbo].[tblExportRequest]
GO

/* tblExportResultDetails */
Select * from [dbo].[tblExportResultDetails]
GO

/* tblExportResults */
Select * from [dbo].[tblExportResults]
GO

/* tblExStarsEndingInventory */
Select * from [dbo].[tblExStarsEndingInventory]
GO

/* tblExStarsFilings */
Select * from [dbo].[tblExStarsFilings]
GO

/* tblExStarsIrsErrorCodes */
Select * from [dbo].[tblExStarsIrsErrorCodes]
GO

/* tblExStarsProductPriorInventory */
Select * from [dbo].[tblExStarsProductPriorInventory]
GO

/* tblExStarsReportedErrors */
Select * from [dbo].[tblExStarsReportedErrors]
GO

/* tblExStarsSiteConfig */
Select * from [dbo].[tblExStarsSiteConfig]
GO

/* tblFilterViews */
Select * from [dbo].[tblFilterViews]
GO

/* tblFuelCardLimit */
Select * from [dbo].[tblFuelCardLimit]
GO

/* tblFuelCardLimitLineItem */
Select * from [dbo].[tblFuelCardLimitLineItem]
GO

/* tblFuelCards */
Select * from [dbo].[tblFuelCards]
GO

/* tblGates */
Select * from [dbo].[tblGates]
GO

/* tblGeneralConfiguration */
Select * from [dbo].[tblGeneralConfiguration]
GO

/* tblGeneralConfigurationAliases */
Select * from [dbo].[tblGeneralConfigurationAliases]
GO

/* tblGroups */
Select * from [dbo].[tblGroups]
GO

/* tblGST */
Select * from [dbo].[tblGST]
GO

/* tblHelpMapping */
Select * from [dbo].[tblHelpMapping]
GO

/* tblHouseCards */
Select * from [dbo].[tblHouseCards]
GO

/* tblIATA */
Select * from [dbo].[tblIATA]
GO

/* tblImportExportConfig */
Select * from [dbo].[tblImportExportConfig]
GO

/* tblImportExportFilters */
Select * from [dbo].[tblImportExportFilters]
GO

/* tblImportExportPlugins */
Select * from [dbo].[tblImportExportPlugins]
GO

/* tblInvoiceQueries */
Select * from [dbo].[tblInvoiceQueries]
GO

/* tblLedgerAggregateColumns */
Select * from [dbo].[tblLedgerAggregateColumns]
GO

/* tblListViewFields */
Select * from [dbo].[tblListViewFields]
GO

/* tblListViews */
Select * from [dbo].[tblListViews]
GO

/* tblLoadArms */
Select * from [dbo].[tblLoadArms]
GO

/* tblMaintenanceReasons */
Select * from [dbo].[tblMaintenanceReasons]
GO

/* tblMarkup */
Select * from [dbo].[tblMarkup]
GO

/* tblMenuFavorites */
Select * from [dbo].[tblMenuFavorites]
GO

/* tblMessageLog */
Select * from [dbo].[tblMessageLog]
GO

/* tblMessages */
Select * from [dbo].[tblMessages]
GO

/* tblMeter */
Select * from [dbo].[tblMeter]
GO

/* tblMigrationExportImportLog */
Select * from [dbo].[tblMigrationExportImportLog]
GO

/* tblMobileDevice */
Select * from [dbo].[tblMobileDevice]
GO

/* tblMobileDeviceProfile */
Select * from [dbo].[tblMobileDeviceProfile]
GO

/* tblMobileDeviceProfileAnalogInput */
Select * from [dbo].[tblMobileDeviceProfileAnalogInput]
GO

/* tblMobileDeviceProfilePrinter */
Select * from [dbo].[tblMobileDeviceProfilePrinter]
GO

/* tblNotes */
Select * from [dbo].[tblNotes]
GO

/* tblOPCConnections */
Select * from [dbo].[tblOPCConnections]
GO

/* tblOwnerCloseout */
Select * from [dbo].[tblOwnerCloseout]
GO

/* tblPersonnel */
Select * from [dbo].[tblPersonnel]
GO

/* tblPIDXProfiles */
Select * from [dbo].[tblPIDXProfiles]
GO

/* tblProcessVariableAdditiveInputPermissive */
Select * from [dbo].[tblProcessVariableAdditiveInputPermissive]
GO

/* tblProcessVariableAdditiveOutputPermissive */
Select * from [dbo].[tblProcessVariableAdditiveOutputPermissive]
GO

/* tblProcessVariableComponentInputPermissive */
Select * from [dbo].[tblProcessVariableComponentInputPermissive]
GO

/* tblProcessVariableComponentOutputPermissive */
Select * from [dbo].[tblProcessVariableComponentOutputPermissive]
GO

/* tblProcessVariableEquipment */
Select * from [dbo].[tblProcessVariableEquipment]
GO

/* tblProcessVariableExternalComponentBlendPercentage */
Select * from [dbo].[tblProcessVariableExternalComponentBlendPercentage]
GO

/* tblProcessVariableExternalComponentInputPermissive */
Select * from [dbo].[tblProcessVariableExternalComponentInputPermissive]
GO

/* tblProcessVariableExternalComponentOutputPermissive */
Select * from [dbo].[tblProcessVariableExternalComponentOutputPermissive]
GO

/* tblProcessVariableLoadArm */
Select * from [dbo].[tblProcessVariableLoadArm]
GO

/* tblProcessVariableLoadArmInputPermissive */
Select * from [dbo].[tblProcessVariableLoadArmInputPermissive]
GO

/* tblProcessVariableLoadArmOutPutPermissive */
Select * from [dbo].[tblProcessVariableLoadArmOutPutPermissive]
GO

/* tblProcessVariableNoAdditiveInputPermissive */
Select * from [dbo].[tblProcessVariableNoAdditiveInputPermissive]
GO

/* tblProcessVariableNoAdditiveOutputPermissive */
Select * from [dbo].[tblProcessVariableNoAdditiveOutputPermissive]
GO

/* tblProcessVariablePresetInjector */
Select * from [dbo].[tblProcessVariablePresetInjector]
GO

/* tblProcessVariableRecipeInputPermissive */
Select * from [dbo].[tblProcessVariableRecipeInputPermissive]
GO

/* tblProcessVariableRecipeOutputPermissive */
Select * from [dbo].[tblProcessVariableRecipeOutputPermissive]
GO

/* tblProcessVariableSite */
Select * from [dbo].[tblProcessVariableSite]
GO

/* tblProcessVariableStation */
Select * from [dbo].[tblProcessVariableStation]
GO

/* tblProcessVariableStationInputPermissive */
Select * from [dbo].[tblProcessVariableStationInputPermissive]
GO

/* tblProcessVariableStationOutputPermissive */
Select * from [dbo].[tblProcessVariableStationOutputPermissive]
GO

/* tblProcessVariableTank */
Select * from [dbo].[tblProcessVariableTank]
GO

/* tblProducts */
Select * from [dbo].[tblProducts]
GO

/* tblQualifications */
Select * from [dbo].[tblQualifications]
GO

/* tblQualityTags */
Select * from [dbo].[tblQualityTags]
GO

/* tblQueryDefaultFields */
Select * from [dbo].[tblQueryDefaultFields]
GO

/* tblQueryDefaults */
Select * from [dbo].[tblQueryDefaults]
GO

/* tblQueryStorage */
Select * from [dbo].[tblQueryStorage]
GO

/* tblReportApprovals */
Select * from [dbo].[tblReportApprovals]
GO

/* tblReportDetails */
Select * from [dbo].[tblReportDetails]
GO

/* tblReportGroups */
Select * from [dbo].[tblReportGroups]
GO

/* tblReserveLevels */
Select * from [dbo].[tblReserveLevels]
GO

/* tblSavedQueries */
Select * from [dbo].[tblSavedQueries]
GO

/* tblSavedQueryItems */
Select * from [dbo].[tblSavedQueryItems]
GO

/* tblScheduleCompanyAccess */
Select * from [dbo].[tblScheduleCompanyAccess]
GO

/* tblScheduleHoliday */
Select * from [dbo].[tblScheduleHoliday]
GO

/* tblSchedulePersonnelAccess */
Select * from [dbo].[tblSchedulePersonnelAccess]
GO

/* tblScheduleTerminalOperation */
Select * from [dbo].[tblScheduleTerminalOperation]
GO

/* tblSequences */
Select * from [dbo].[tblSequences]
GO

/* tblSessions */
Select * from [dbo].[tblSessions]
GO

/* tblSettings */
Select * from [dbo].[tblSettings]
GO

/* tblSiteAdmin */
Select * from [dbo].[tblSiteAdmin]
GO

/* tblSites */
Select * from [dbo].[tblSites]
GO

/* tblSitesAncillaryData */
Select * from [dbo].[tblSitesAncillaryData]
GO

/* tblSitesShadow */
Select * from [dbo].[tblSitesShadow]
GO

/* tblSRMAdaptor */
Select * from [dbo].[tblSRMAdaptor]
GO

/* tblSRMAdaptorFilter */
Select * from [dbo].[tblSRMAdaptorFilter]
GO

/* tblSRMAdaptorFPES */
Select * from [dbo].[tblSRMAdaptorFPES]
GO

/* tblSRMConfiguration */
Select * from [dbo].[tblSRMConfiguration]
GO

/* tblSRMDuplicateMessageInformation */
Select * from [dbo].[tblSRMDuplicateMessageInformation]
GO

/* tblSRMMessage */
Select * from [dbo].[tblSRMMessage]
GO

/* tblSRMMessageRetryQueue */
Select * from [dbo].[tblSRMMessageRetryQueue]
GO

/* tblStandardImportConfig */
Select * from [dbo].[tblStandardImportConfig]
GO

/* tblStandingOffers */
Select * from [dbo].[tblStandingOffers]
GO

/* tblStations */
Select * from [dbo].[tblStations]
GO

/* tblSyncClientConfiguration */
Select * from [dbo].[tblSyncClientConfiguration]
GO

/* tblSyncServerConfiguration */
Select * from [dbo].[tblSyncServerConfiguration]
GO

/* tblSystemSettings */
Select * from [dbo].[tblSystemSettings]
GO

/* tblTankGroups */
Select * from [dbo].[tblTankGroups]
GO

/* tblTankMaintenanceLog */
Select * from [dbo].[tblTankMaintenanceLog]
GO

/* tblTankQualityTagLog */
Select * from [dbo].[tblTankQualityTagLog]
GO

/* tblTanks */
Select * from [dbo].[tblTanks]
GO

/* tblTestDefinitions */
Select * from [dbo].[tblTestDefinitions]
GO

/* tblTestEquipmentResults */
Select * from [dbo].[tblTestEquipmentResults]
GO

/* tblTestSetDefinitions */
Select * from [dbo].[tblTestSetDefinitions]
GO

/* tblTestSetEquipmentResults */
Select * from [dbo].[tblTestSetEquipmentResults]
GO

/* tblTestSetTankResults */
Select * from [dbo].[tblTestSetTankResults]
GO

/* tblTestTankResults */
Select * from [dbo].[tblTestTankResults]
GO

/* tblUserDataFieldCompany */
Select * from [dbo].[tblUserDataFieldCompany]
GO

/* tblUserDataFieldEquipment */
Select * from [dbo].[tblUserDataFieldEquipment]
GO

/* tblUserDataFieldFuelCard */
Select * from [dbo].[tblUserDataFieldFuelCard]
GO

/* tblUserDataFieldPersonnel */
Select * from [dbo].[tblUserDataFieldPersonnel]
GO

/* tblUserDataFieldProduct */
Select * from [dbo].[tblUserDataFieldProduct]
GO

/* tblUserDataFieldSite */
Select * from [dbo].[tblUserDataFieldSite]
GO

/* tblUserDataFieldTransactionAlias */
Select * from [dbo].[tblUserDataFieldTransactionAlias]
GO

/* tblUserDataFieldTransactionAliasLineItem */
Select * from [dbo].[tblUserDataFieldTransactionAliasLineItem]
GO

/* tblUserDataListValueCompany */
Select * from [dbo].[tblUserDataListValueCompany]
GO

/* tblUserDataListValueEquipment */
Select * from [dbo].[tblUserDataListValueEquipment]
GO

/* tblUserDataListValueFuelCard */
Select * from [dbo].[tblUserDataListValueFuelCard]
GO

/* tblUserDataListValuePersonnel */
Select * from [dbo].[tblUserDataListValuePersonnel]
GO

/* tblUserDataListValueProduct */
Select * from [dbo].[tblUserDataListValueProduct]
GO

/* tblUserDataListValueSite */
Select * from [dbo].[tblUserDataListValueSite]
GO

/* tblUserDataListValueTransactionAlias */
Select * from [dbo].[tblUserDataListValueTransactionAlias]
GO

/* tblUserDataListValueTransactionAliasLineItem */
Select * from [dbo].[tblUserDataListValueTransactionAliasLineItem]
GO

/* tblUsers */
Select * from [dbo].[tblUsers]
GO

/* tblVersion */
Select * from [dbo].[tblVersion]
GO

/* tblWeightedAverageCosts */
Select * from [dbo].[tblWeightedAverageCosts]
GO

/* tblAdditiveProfiles - IX_tblAdditiveProfiles_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAdditiveProfiles]
GO

/* tblAdditiveProfiles - IX_tblAdditiveProfiles_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblAdditiveProfiles]
GO

/* tblAdditiveProfiles - IXU_tblAdditiveProfiles_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblAdditiveProfiles]
GO

/* tblAirplaneTank - IX_tblAirplaneTank_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAirplaneTank]
GO

/* tblAlarmAndEventLog - IX_tblAlarmAndEventLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAlarmAndEventLog]
GO

/* tblAlarmAndEventLog - IX_tblAlarmAndEventLog_SiteGuid_CreatedDate */
SELECT [SiteGuid] FROM [dbo].[tblAlarmAndEventLog]
GO

/* tblAlarmAndEvents - IX_tblAlarmAndEvents_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAlarmAndEvents]
GO

/* tblAlarmAndEvents - IX_tblAlarmAndEvents_Source_ID */
SELECT [Source],[ID] FROM [dbo].[tblAlarmAndEvents]
GO

/* tblAlarmAndEvents - IXU_tblAlarmAndEvents_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblAlarmAndEvents]
GO

/* tblAlarmPriorities - IX_tblAlarmPriorities_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAlarmPriorities]
GO

/* tblAlarmPriorities - IXU_tblAlarmPriorities_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblAlarmPriorities]
GO

/* tblAllocationLineItems - IX_tblAllocationLineItems_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAllocationLineItems]
GO

/* tblAllocations - IX_tblAllocations_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAllocations]
GO

/* tblApplicationString - IX_tblApplicationString_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblApplicationString]
GO

/* tblApplicationString - IXU_tblApplicationString_ApplicationStringGuid_CreatedDate */
SELECT [ApplicationStringGuid] FROM [dbo].[tblApplicationString]
GO

/* tblApplicationString - IXU_tblApplicationString_ID_SiteGuid */
SELECT [ID],[SiteGuid],[LookupApplicationStringTypeIndex] FROM [dbo].[tblApplicationString]
GO

/* tblAppointmentEquipment - IX_tblAppointmentEquipment_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAppointmentEquipment]
GO

/* tblAppointmentPersonnel - IX_tblAppointmentPersonnel_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAppointmentPersonnel]
GO

/* tblAppointmentTank - IX_tblAppointmentTank_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAppointmentTank]
GO

/* tblArchivedUsers - IX_tblArchivedUsers_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblArchivedUsers]
GO

/* tblAuditHandler - IX_tblAuditHandler_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAuditHandler]
GO

/* tblAuditLog - IX_tblAuditLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAuditLog]
GO

/* tblAutoDistributionReasonCodes - IX_tblAutoDistributionReasonCodes_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAutoDistributionReasonCodes]
GO

/* tblAutoDistributionRule - IX_tblAutoDistributionRule_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblAutoDistributionRule]
GO

/* tblAutoDistributionRule - IXU_tblAutoDistributionRule_RuleID_SiteGuid */
SELECT [RuleID],[SiteGuid] FROM [dbo].[tblAutoDistributionRule]
GO

/* tblB2BResults - IX_tblB2BResults */
SELECT [ReceivedSentDate] FROM [dbo].[tblB2BResults]
GO

/* tblBulkPaymentLinks - IX_tblBulkPaymentLinks_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblBulkPaymentLinks]
GO

/* tblBulkPaymentLinks - IXU_tblBulkPaymentLinks_BulkPaymentID */
SELECT [BulkPaymentID] FROM [dbo].[tblBulkPaymentLinks]
GO

/* tblBulkPayments - IX_tblBulkPayments_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblBulkPayments]
GO

/* tblChangeLog - IX_tblChangeLog */
SELECT [DateEvent] FROM [dbo].[tblChangeLog]
GO

/* tblChangesQueue - IX_tblChangesQueue_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblChangesQueue]
GO

/* tblCloseoutInventory - IX_tblCloseoutInventory_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblCloseoutInventory]
GO

/* tblCloseoutInventory - IX_tblCloseoutInventory_ManagerCompanyGuid */
SELECT [ManagerCompanyGuid] FROM [dbo].[tblCloseoutInventory]
GO

/* tblCloseoutInventory - IX_tblCloseoutInventory_Site_CloseoutDate_ProductName_ManagerName */
SELECT [Site],[CloseoutDate],[ProductName],[ManagerName] FROM [dbo].[tblCloseoutInventory]
GO

/* tblCompanies - IX_tblCompanies_Code */
SELECT [Code] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IX_tblCompanies_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IX_tblCompanies_SiteGuid_ID */
SELECT [SiteGuid],[ID] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IXU_tblCompanies_CompanyGuid_ID */
SELECT [CompanyGuid],[ID] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IXU_tblCompanies_GetCompanyRecordVersionsCovering */
SELECT [CompanyGuid],[_MasterRecordGuid],[SiteGuid] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IXU_tblCompanies_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IXU_tblCompanies_MasterGuid */
SELECT [_MasterRecordGuid] FROM [dbo].[tblCompanies]
GO

/* tblCompanies - IXU_tblCompanies_SiteGuid_MasterRecordGuid */
SELECT [SiteGuid],[_MasterRecordGuid] FROM [dbo].[tblCompanies]
GO

/* tblCompanyCrossReference - IX_tblCompanyCrossReference_GUID */
SELECT [CompanyCrossReferenceGuid] FROM [dbo].[tblCompanyCrossReference]
GO

/* tblCompanyCrossReferenceMap - UIX_tblCompanyCrossReferenceMap_MapKeyName_MapValueName */
SELECT [MapKeyName],[MapValueName] FROM [dbo].[tblCompanyCrossReferenceMap]
GO

/* tblConfigurationSetting - IX_tblConfigurationSetting */
SELECT [CreatedDate] FROM [dbo].[tblConfigurationSetting]
GO

/* tblControllersLog - IX_tblControllersLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblControllersLog]
GO

/* tblCurrencies - IX_tblCurrencies_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblCurrencies]
GO

/* tblCurrencies - IX_tblCurrencies_UnitDisplayName */
SELECT [UnitDisplayName] FROM [dbo].[tblCurrencies]
GO

/* tblCurrencyLineItems - IX_tblCurrencyLineItems_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblCurrencyLineItems]
GO

/* tblCustomToolbar - IX_tblCustomToolbar_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblCustomToolbar]
GO

/* tblCustomToolbar - IXU_tblCustomToolbar_ID_DispatchConfigurationGuid */
SELECT [ID],[DispatchConfigurationGuid] FROM [dbo].[tblCustomToolbar]
GO

/* tblCustomToolbarCommand - IX_tblCustomToolbarCommand_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblCustomToolbarCommand]
GO

/* tblCustomToolbarCommand - IXU_tblCustomToolbarCommand_ID_TransactionAliasGuid_CustomToolbarGuid */
SELECT [ID],[TransactionAliasGuid],[CustomToolbarGuid] FROM [dbo].[tblCustomToolbarCommand]
GO

/* tblDataDictionaries - IX_tblDataDictionaries_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblDataDictionaries]
GO

/* tblDataDictionaries - IX_tblDataDictionaries_Key_SiteGuid */
SELECT [Key],[SiteGuid] FROM [dbo].[tblDataDictionaries]
GO

/* tblDataDictionaries - IX_tblDataDictionaries_UpdatedDate_SiteGuid_Key */
SELECT [UpdatedDate],[SiteGuid],[Key] FROM [dbo].[tblDataDictionaries]
GO

/* tblDispatchConfiguration - IX_tblDispatchConfiguration_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblDispatchConfiguration]
GO

/* tblDispatchConfiguration - IXU_tblDispatchConfiguration_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblDispatchConfiguration]
GO

/* tblDispatchGrid - IX_tblDispatchGrid_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblDispatchGrid]
GO

/* tblDispatchGrid - IXU_tblDispatchGrid_ID_DispatchConfigurationGuid */
SELECT [ID],[DispatchConfigurationGuid] FROM [dbo].[tblDispatchGrid]
GO

/* tblDispatchGridColumn - IX_tblDispatchGridColumn_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblDispatchGridColumn]
GO

/* tblDispatchGridColumn - IXU_tblDispatchGridColumn_ID_UserDataFieldTransAliasGuid_UserDataFieldTransAliasLineItemGuid_DispatchGridGuid_UserGuid */
SELECT [ID],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[DispatchGridGuid],[UserGuid] FROM [dbo].[tblDispatchGridColumn]
GO

/* tblEmailGroups - IX_tblEmailGroups_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblEmailGroups]
GO

/* tblEmailGroups - IXU_tblEmailGroups_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblEmailGroups]
GO

/* tblEnterpriseQueue - IX_EnterpriseQueue_DateAdded */
SELECT [DateAdded] FROM [dbo].[tblEnterpriseQueue]
GO

/* tblEquipment - IX_tblEquipment_AssignedToMeterGuid */
SELECT [AssignedToMeterGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IX_tblEquipment_CompanyGuid */
SELECT [CompanyGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IX_tblEquipment_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IX_tblEquipment_ParentEquipmentGuid */
SELECT [ParentEquipmentGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IXU_tblEquipment_001 */
SELECT [_MasterRecordGuid],[SiteGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IXU_tblEquipment_EquipmentGuid_CoveringBasicFields */
SELECT [EquipmentGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IXU_tblEquipment_EquipmentGuid_CoveringForLineItemTrigger */
SELECT [EquipmentGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IXU_tblEquipment_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipment - IXU_tblEquipment_SiteGuid_MasterRecordGuid */
SELECT [SiteGuid],[_MasterRecordGuid] FROM [dbo].[tblEquipment]
GO

/* tblEquipmentMaintenanceLog - IX_tblEquipmentMaintenanceLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblEquipmentMaintenanceLog]
GO

/* tblEquipmentMaintenanceLog - IX_tblEquipmentMaintenanceLog_EquipmentGuid */
SELECT [EquipmentGuid] FROM [dbo].[tblEquipmentMaintenanceLog]
GO

/* tblEquipmentMaintenanceLog - IX_tblEquipmentMaintenanceLog_MaintenanceReasonGuid */
SELECT [MaintenanceReasonGuid] FROM [dbo].[tblEquipmentMaintenanceLog]
GO

/* tblEquipmentMaintenanceLog - IX_tblEquipmentMaintenanceLog_OperatorGuid */
SELECT [OperatorPersonnelGuid] FROM [dbo].[tblEquipmentMaintenanceLog]
GO

/* tblEquipmentMaintenanceLog - IX_tblEquipmentMaintenanceLog_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblEquipmentMaintenanceLog]
GO

/* tblEquipmentQualityTagLog - IX_tblEquipmentQualityTagLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblEquipmentQualityTagLog]
GO

/* tblEquipmentTypes - IX_tblEquipmentTypes_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblEquipmentTypes]
GO

/* tblExcise - IX_tblExcise_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblExcise]
GO

/* tblExportPaiceTransTracking - IX_tblExportPaiceTransTracking_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblExportPaiceTransTracking]
GO

/* tblExportRequest - IX_tblExportRequest_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblExportRequest]
GO

/* tblExportRequest - UIX_tblExportRequest_RequestID */
SELECT [RequestID] FROM [dbo].[tblExportRequest]
GO

/* tblExportResultDetails - IX_tblExportResultDetails_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblExportResultDetails]
GO

/* tblExportResultDetails - IX_tblExportResultDetails_ExportResultGuid */
SELECT [ExportResultGuid] FROM [dbo].[tblExportResultDetails]
GO

/* tblExportResultDetails - IX_tblExportResultDetails_GuidRowVersion */
SELECT [ExportResultDetailGuid],[_RowVersion] FROM [dbo].[tblExportResultDetails]
GO

/* tblExportResultDetails - IX_tblExportResultDetails_RecordID_TransVersion */
SELECT [RecordID],[TransVersion] FROM [dbo].[tblExportResultDetails]
GO

/* tblExportResults - IX_tblExportResults_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblExportResults]
GO

/* tblExportResults - IX_tblExportResults_LookupExportResultTypeIndex */
SELECT [LookupExportResultTypeIndex] FROM [dbo].[tblExportResults]
GO

/* tblExportResults - IX_tblExportResults_SiteGuid */
SELECT [SiteGuid],[ExportResultGuid] FROM [dbo].[tblExportResults]
GO

/* tblExStarsEndingInventory - tblExStarsEndingInventory_PK */
SELECT [CreatedDate] FROM [dbo].[tblExStarsEndingInventory]
GO

/* tblExStarsFilings - IX_tblExStarsFilingsCreatedDate */
SELECT [FilingCreated] FROM [dbo].[tblExStarsFilings]
GO

/* tblExStarsFilings - IX_tblExStarsFilingsGuid */
SELECT [ExStarsFilingsGuid] FROM [dbo].[tblExStarsFilings]
GO

/* tblExStarsFilings - IX_tblExStarsFilingsMgrSiteType */
SELECT [FilingStartDate],[ManagerCompanyGuid],[SiteGuid],[ReportType],[Modifier] FROM [dbo].[tblExStarsFilings]
GO

/* tblExStarsFilings - IX_tblExStarsFilingsTransactionSetControlNUmber */
SELECT [TransSetControlNumber] FROM [dbo].[tblExStarsFilings]
GO

/* tblExStarsIrsErrorCodes - IX_tblExStarsIrsErrorCodesGuid */
SELECT [ExStarsIrsErrorCodesGuid] FROM [dbo].[tblExStarsIrsErrorCodes]
GO

/* tblExStarsProductPriorInventory - IX_tblExStarsEndingInventory_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblExStarsProductPriorInventory]
GO

/* tblExStarsProductPriorInventory - IX_tblExStarsEndingInventory_SiteMangerProduct */
SELECT [SiteGuid],[ManagerCompanyGuid],[TaxCode] FROM [dbo].[tblExStarsProductPriorInventory]
GO

/* tblExStarsReportedErrors - IX_tblExStarsReportedErrorsCreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblExStarsReportedErrors]
GO

/* tblExStarsReportedErrors - IX_tblExStarsReportedErrorsGuid */
SELECT [ExStarsReportedErrorsGuid] FROM [dbo].[tblExStarsReportedErrors]
GO

/* tblExStarsReportedErrors - IX_tblExStarsReportedErrorsUnique */
SELECT [ExStarsFilingsGuid],[SequenceNumber],[PBI01_Primary],[PBI01_Secondary],[PBI03_Primary],[PBI03_Secondary],[PBI04] FROM [dbo].[tblExStarsReportedErrors]
GO

/* tblExStarsSiteConfig - IX_tblExStarsSiteConfig_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblExStarsSiteConfig]
GO

/* tblExStarsSiteConfig - IX_tblExStarsSiteConfig_SiteManager */
SELECT [SiteGuid],[ManagerCompanyGuid] FROM [dbo].[tblExStarsSiteConfig]
GO

/* tblFilterViews - IX_tblFilterViews_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblFilterViews]
GO

/* tblFuelCardLimit - IX_tblFuelCardLimit_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblFuelCardLimit]
GO

/* tblFuelCardLimit - IX_tblFuelCardLimit_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblFuelCardLimit]
GO

/* tblFuelCardLimit - UIX_tblFuelCardLimit_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblFuelCardLimit]
GO

/* tblFuelCardLimitLineItem - IX_tblFuelCardLimitLineItem_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblFuelCardLimitLineItem]
GO

/* tblFuelCardLimitLineItem - IX_tblFuelCardLimitLineItem_FuelCardLimitGuid */
SELECT [FuelCardLimitGuid] FROM [dbo].[tblFuelCardLimitLineItem]
GO

/* tblFuelCardLimitLineItem - IX_tblFuelCardLimitLineItem_ProductGroupApplicationStringGuid */
SELECT [ProductGroupApplicationStringGuid] FROM [dbo].[tblFuelCardLimitLineItem]
GO

/* tblFuelCardLimitLineItem - IX_tblFuelCardLimitLineItem_ProductGuid */
SELECT [ProductGuid] FROM [dbo].[tblFuelCardLimitLineItem]
GO

/* tblFuelCards - IX_tblFuelCards_BillToCompanyGuid */
SELECT [BillToCompanyGuid] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_FuelCardTypeApplicationStringGuid */
SELECT [FuelCardTypeApplicationStringGuid] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_ID */
SELECT [ID] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_ManagerCompanyGuid */
SELECT [ManagerCompanyGuid] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_OwnerCompanyGuid */
SELECT [OwnerCompanyGuid] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_ProviderID */
SELECT [ProviderID] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_ShipperCompanyGuid */
SELECT [ShipperCompanyGuid] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_ShipToCompanyGuid */
SELECT [ShipToCompanyGuid] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IX_tblFuelCards_UserData1 */
SELECT [UserData1] FROM [dbo].[tblFuelCards]
GO

/* tblFuelCards - IXU_tblFuelCards_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblFuelCards]
GO

/* tblGates - IX_tblGates_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblGates]
GO

/* tblGates - IXU_tblGates_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblGates]
GO

/* tblGeneralConfiguration - IX_tblGeneralConfiguration_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblGeneralConfiguration]
GO

/* tblGeneralConfigurationAliases - IX_tblGeneralConfigurationAliases_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblGeneralConfigurationAliases]
GO

/* tblGroups - IX_tblGroups_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblGroups]
GO

/* tblGroups - IXU_tblGroups_GroupGuid_GroupID */
SELECT [GroupGuid],[GroupID] FROM [dbo].[tblGroups]
GO

/* tblGroups - IXU_tblGroups_GroupID_SiteGuid */
SELECT [GroupID],[SiteGuid],[GroupGuid] FROM [dbo].[tblGroups]
GO

/* tblGST - IX_tblGST_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblGST]
GO

/* tblHelpMapping - IXU_tblHelpMapping_HelpContextKey */
SELECT [HelpContextKey] FROM [dbo].[tblHelpMapping]
GO

/* tblHouseCards - IX_tblHouseCards_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblHouseCards]
GO

/* tblHouseCards - IX_tblHouseCards_DriverPersonnelGuid */
SELECT [DriverPersonnelGuid] FROM [dbo].[tblHouseCards]
GO

/* tblHouseCards - IXU_tblHouseCards_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblHouseCards]
GO

/* tblIATA - IX_tblIATA_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblIATA]
GO

/* tblIATA - IX_tblIATA_SiteGuid_IATAID */
SELECT [SiteGuid],[IATAID] FROM [dbo].[tblIATA]
GO

/* tblIATA - IXU_tblIATA_IATAID_SiteGuid */
SELECT [IATAID],[SiteGuid] FROM [dbo].[tblIATA]
GO

/* tblImportExportConfig - IX_tblImportExportConfig */
SELECT [ImportExportConfigGuid] FROM [dbo].[tblImportExportConfig]
GO

/* tblImportExportFilters - IX_tblImportExportFilters */
SELECT [ImportExportFilterGuid] FROM [dbo].[tblImportExportFilters]
GO

/* tblImportExportPlugins - IX_tblImportExportPlugins */
SELECT [ImportExportPluginGuid] FROM [dbo].[tblImportExportPlugins]
GO

/* tblInvoiceQueries - IX_tblInvoiceQueries_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblInvoiceQueries]
GO

/* tblLedgerAggregateColumns - IX_tblLedgerAggregateColumns_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblLedgerAggregateColumns]
GO

/* tblLedgerAggregateColumns - IX_tblLedgerAggregateColumns_LedgerAggregateColumnGuid */
SELECT [LedgerAggregateColumnGuid] FROM [dbo].[tblLedgerAggregateColumns]
GO

/* tblListViewFields - IX_tblListViewFields_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblListViewFields]
GO

/* tblListViewFields - IX_tblListViewFields_ListViewGuid_OtherFKFields */
SELECT [ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasFieldGuid],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[ColumnOrder] FROM [dbo].[tblListViewFields]
GO

/* tblListViews - IX_tblListViews_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblListViews]
GO

/* tblListViews - IXU_tblListViews_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblListViews]
GO

/* tblLoadArms - IX_tblLoadArms_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblLoadArms]
GO

/* tblMaintenanceReasons - IX_tblMaintenanceReasons_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMaintenanceReasons]
GO

/* tblMarkup - IX_tblMarkup_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMarkup]
GO

/* tblMenuFavorites - IX_tblMenuFavorites */
SELECT [CreatedDate] FROM [dbo].[tblMenuFavorites]
GO

/* tblMessageLog - IX_tblMessageLog_CompanyGuid */
SELECT [CompanyGuid] FROM [dbo].[tblMessageLog]
GO

/* tblMessageLog - IX_tblMessageLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMessageLog]
GO

/* tblMessageLog - IX_tblMessageLog_PersonnelGuid_CompanyGuid_CreatedDate_MessageGuid */
SELECT [PersonnelGuid],[CompanyGuid],[MessageGuid] FROM [dbo].[tblMessageLog]
GO

/* tblMessages - IX_tblMessages_CompanyGuid */
SELECT [CompanyGuid] FROM [dbo].[tblMessages]
GO

/* tblMessages - IX_tblMessages_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMessages]
GO

/* tblMessages - IXU_tblMessages_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblMessages]
GO

/* tblMeter - IX_tblMeter_ClusterIdx */
SELECT [ClusterIdx] FROM [dbo].[tblMeter]
GO

/* tblMeter - IXU_tblMeter_MeterID_SiteGuid */
SELECT [MeterID],[SiteGuid] FROM [dbo].[tblMeter]
GO

/* tblMigrationExportImportLog - IX_tblMigrationExportImportLog_ActivityID */
SELECT [ActivityID] FROM [dbo].[tblMigrationExportImportLog]
GO

/* tblMigrationExportImportLog - IX_tblMigrationExportImportLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMigrationExportImportLog]
GO

/* tblMigrationExportImportLog - IX_tblMigrationExportImportLog_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblMigrationExportImportLog]
GO

/* tblMobileDevice - IX_tblMobileDeviceCreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMobileDevice]
GO

/* tblMobileDeviceProfile - IX_tblMobileDeviceProfile_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMobileDeviceProfile]
GO

/* tblMobileDeviceProfileAnalogInput - IX_tblMobileDeviceProfileAnalogInputCreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMobileDeviceProfileAnalogInput]
GO

/* tblMobileDeviceProfilePrinter - IX_tblMobileDeviceProfilePrinterCreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblMobileDeviceProfilePrinter]
GO

/* tblNotes - IX_tblNotes_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblNotes]
GO

/* tblOPCConnections - IX_tblOPCConnections_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblOPCConnections]
GO

/* tblOwnerCloseout - IX_tblOwnerCloseout_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblOwnerCloseout]
GO

/* tblOwnerCloseout - IX_tblOwnerCloseOut_JournalReportCoveringIndex */
SELECT [ManagerCompanyGuid],[OwnerCompanyGuid],[ProductGuid],[CloseoutDate] FROM [dbo].[tblOwnerCloseout]
GO

/* tblOwnerCloseout - IX_tblOwnerCloseout_ManagerCompanyGuid */
SELECT [ManagerCompanyGuid] FROM [dbo].[tblOwnerCloseout]
GO

/* tblOwnerCloseout - IX_tblOwnerCloseOut_MgrNameOwnerNameProdNameDate */
SELECT [Site],[ManagerName],[ProductName],[OwnerName],[CloseoutDate] FROM [dbo].[tblOwnerCloseout]
GO

/* tblOwnerCloseout - IX_tblOwnerCloseOut_MgrOwnerProdDate */
SELECT [CloseoutDate],[SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ProductGuid] FROM [dbo].[tblOwnerCloseout]
GO

/* tblOwnerCloseout - IX_tblOwnerCloseout_OwnerCompanyGuid */
SELECT [OwnerCompanyGuid] FROM [dbo].[tblOwnerCloseout]
GO

/* tblPersonnel - IX_tblPersonnel_CompanyGuid */
SELECT [CompanyGuid] FROM [dbo].[tblPersonnel]
GO

/* tblPersonnel - IX_tblPersonnel_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblPersonnel]
GO

/* tblPersonnel - IXU_tblPersonnel_PersonID_SiteGuid */
SELECT [PersonID],[SiteGuid] FROM [dbo].[tblPersonnel]
GO

/* tblPersonnel - IXU_tblPersonnel_PersonnelGuid_IncludeBasicInformation */
SELECT [PersonnelGuid] FROM [dbo].[tblPersonnel]
GO

/* tblPersonnel - IXU_tblPersonnel_SiteGuid_MasterRecordGuid */
SELECT [SiteGuid],[_MasterRecordGuid] FROM [dbo].[tblPersonnel]
GO

/* tblPIDXProfiles - IX_tblPIDXProfiles_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblPIDXProfiles]
GO

/* tblPIDXProfiles - IXU_tblPIDXProfiles_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblPIDXProfiles]
GO

/* tblProcessVariableAdditiveInputPermissive - IX_tblProcessVariableAdditiveInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableAdditiveInputPermissive]
GO

/* tblProcessVariableAdditiveOutputPermissive - IX_tblProcessVariableAdditiveOutputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableAdditiveOutputPermissive]
GO

/* tblProcessVariableComponentInputPermissive - IX_tblProcessVariableComponentInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableComponentInputPermissive]
GO

/* tblProcessVariableComponentOutputPermissive - IX_tblProcessVariableComponentOutputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableComponentOutputPermissive]
GO

/* tblProcessVariableEquipment - IX_tblProcessVariableEquipment_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableEquipment]
GO

/* tblProcessVariableExternalComponentBlendPercentage - IX_tblProcessVariableExternalComponentBlendPercentage_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableExternalComponentBlendPercentage]
GO

/* tblProcessVariableExternalComponentInputPermissive - IX_tblProcessVariableExternalComponentInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableExternalComponentInputPermissive]
GO

/* tblProcessVariableExternalComponentOutputPermissive - IX_tblProcessVariableExternalComponentOutputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableExternalComponentOutputPermissive]
GO

/* tblProcessVariableLoadArm - IX_tblProcessVariableLoadArm_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableLoadArm]
GO

/* tblProcessVariableLoadArmInputPermissive - IX_tblProcessVariableLoadArmInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableLoadArmInputPermissive]
GO

/* tblProcessVariableLoadArmOutPutPermissive - IX_tblProcessVariableLoadArmOutPutPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableLoadArmOutPutPermissive]
GO

/* tblProcessVariableNoAdditiveInputPermissive - IX_tblProcessVariableNoAdditiveInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableNoAdditiveInputPermissive]
GO

/* tblProcessVariableNoAdditiveOutputPermissive - IX_tblProcessVariableNoAdditiveOutputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableNoAdditiveOutputPermissive]
GO

/* tblProcessVariablePresetInjector - IX_tblProcessVariablePresetInjector_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariablePresetInjector]
GO

/* tblProcessVariableRecipeInputPermissive - IX_tblProcessVariableRecipeInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableRecipeInputPermissive]
GO

/* tblProcessVariableRecipeOutputPermissive - IX_tblProcessVariableRecipeOutputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableRecipeOutputPermissive]
GO

/* tblProcessVariableSite - IX_tblProcessVariableSite_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableSite]
GO

/* tblProcessVariableStation - IX_tblProcessVariableStation_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableStation]
GO

/* tblProcessVariableStationInputPermissive - IX_tblProcessVariableStationInputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableStationInputPermissive]
GO

/* tblProcessVariableStationOutputPermissive - IX_tblProcessVariableStationOutputPermissive_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableStationOutputPermissive]
GO

/* tblProcessVariableTank - IX_tblProcessVariableTank_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProcessVariableTank]
GO

/* tblProducts - IX_tblProducts_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblProducts]
GO

/* tblProducts - IXU_tblProducts_ProductID_SiteGuid */
SELECT [ProductID],[SiteGuid] FROM [dbo].[tblProducts]
GO

/* tblProducts - IXU_tblProducts_SiteGuid_MasterRecordGuid */
SELECT [SiteGuid],[_MasterRecordGuid] FROM [dbo].[tblProducts]
GO

/* tblQualifications - IX_tblQualifications_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblQualifications]
GO

/* tblQualifications - IXU_tblQualifications_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblQualifications]
GO

/* tblQualityTags - IX_tblQualityTags_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblQualityTags]
GO

/* tblQueryDefaultFields - IX_tblQueryDefaultFields_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblQueryDefaultFields]
GO

/* tblQueryDefaults - IX_tblQueryDefaults_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblQueryDefaults]
GO

/* tblQueryStorage - IX_tblQueryStorage_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblQueryStorage]
GO

/* tblQueryStorage - IX_tblQueryStorage_SiteGuid_QueryName */
SELECT [SiteGuid],[QueryName] FROM [dbo].[tblQueryStorage]
GO

/* tblReportApprovals - IX_tblReportApprovals_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblReportApprovals]
GO

/* tblReportDetails - IX_tblReportDetails_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblReportDetails]
GO

/* tblReportDetails - IX_tblReportDetails_SiteGuid_ReportName */
SELECT [SiteGuid],[ReportName] FROM [dbo].[tblReportDetails]
GO

/* tblReportGroups - IX_tblReportGroups_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblReportGroups]
GO

/* tblReserveLevels - IX_tblReserveLevels */
SELECT [ReserveLevelGuid] FROM [dbo].[tblReserveLevels]
GO

/* tblSavedQueries - IX_tblSavedQueries_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSavedQueries]
GO

/* tblSavedQueryItems - IX_tblSavedQueryItems_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSavedQueryItems]
GO

/* tblScheduleCompanyAccess - IX_tblScheduleCompanyAccess_CompanyGuid */
SELECT [CompanyGuid] FROM [dbo].[tblScheduleCompanyAccess]
GO

/* tblScheduleCompanyAccess - IX_tblScheduleCompanyAccess_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblScheduleCompanyAccess]
GO

/* tblScheduleHoliday - IX_tblScheduleHoliday_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblScheduleHoliday]
GO

/* tblSchedulePersonnelAccess - IX_tblSchedulePersonnelAccess_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSchedulePersonnelAccess]
GO

/* tblScheduleTerminalOperation - IX_tblScheduleTerminalOperation_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblScheduleTerminalOperation]
GO

/* tblSequences - IX_tblSequences */
SELECT [SequenceGuid] FROM [dbo].[tblSequences]
GO

/* tblSessions - IX_tblSessions_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSessions]
GO

/* tblSessions - IX_tblSessions_SqlServerSessionID */
SELECT [SqlServerSessionID] FROM [dbo].[tblSessions]
GO

/* tblSessions - IX_tblSessions_UpdatedDate_Timeout */
SELECT [UpdatedDate],[Timeout] FROM [dbo].[tblSessions]
GO

/* tblSessions - IX_tblSessions_UserGuid */
SELECT [UserGuid] FROM [dbo].[tblSessions]
GO

/* tblSessions - IXU_tblSessions_SessionGuid */
SELECT [SessionGuid] FROM [dbo].[tblSessions]
GO

/* tblSettings - IX_tblSettings */
SELECT [SettingKey] FROM [dbo].[tblSettings]
GO

/* tblSettings - IXU_tblSettings_SettingID */
SELECT [SettingID] FROM [dbo].[tblSettings]
GO

/* tblSiteAdmin - IX_tblSiteAdmin_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSiteAdmin]
GO

/* tblSites - IX_tblSites_ID */
SELECT [CreatedDate] FROM [dbo].[tblSites]
GO

/* tblSites - IXU_tblSites_ID */
SELECT [ID] FROM [dbo].[tblSites]
GO

/* tblSites - IXU_tblSites_SiteGuid_CreatedDate */
SELECT [SiteGuid] FROM [dbo].[tblSites]
GO

/* tblSitesAncillaryData - IXC_tblSitesAncillaryData */
SELECT [CreatedDate] FROM [dbo].[tblSitesAncillaryData]
GO

/* tblSitesAncillaryData - IXU_tblSitesAncillaryData_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblSitesAncillaryData]
GO

/* tblSitesShadow - IX_tblSitesShadow_ID */
SELECT [CreatedDate] FROM [dbo].[tblSitesShadow]
GO

/* tblSitesShadow - IXU_tblSitesShadow_ID */
SELECT [ID] FROM [dbo].[tblSitesShadow]
GO

/* tblSitesShadow - IXU_tblSitesShadow_SiteGuid_CreatedDate */
SELECT [SiteGuid] FROM [dbo].[tblSitesShadow]
GO

/* tblSRMAdaptor - UIX_tblSRMAdaptor_SRMAdaptorName */
SELECT [SRMAdaptorName] FROM [dbo].[tblSRMAdaptor]
GO

/* tblSRMAdaptorFilter - IX_tblSRMAdaptorFilter_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSRMAdaptorFilter]
GO

/* tblSRMAdaptorFilter - IX_tblSRMAdaptorFilter_KeyFields */
SELECT [SRMAdaptorGuid],[SiteGuid],[SRMAdaptorFilterTypeCode],[FilterValue] FROM [dbo].[tblSRMAdaptorFilter]
GO

/* tblSRMAdaptorFilter - IX_tblSRMAdaptorFilter_SRMAdaptorGuid_SiteGuid */
SELECT [SRMAdaptorGuid],[SiteGuid] FROM [dbo].[tblSRMAdaptorFilter]
GO

/* tblSRMAdaptorFPES - IX_tblSRMAdaptorFPES_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSRMAdaptorFPES]
GO

/* tblSRMConfiguration - IX_tblSRMConfiguration_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSRMConfiguration]
GO

/* tblSRMDuplicateMessageInformation - IX_tblSRMDuplicateMessageInformation_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSRMDuplicateMessageInformation]
GO

/* tblSRMDuplicateMessageInformation - UIX_tblSRMDuplicateMessageInformation_FlightKeyFields */
SELECT [FlightNumber],[FlightOriginationDate],[OriginIATACode],[DestinationIATACode],[AirlineIATACode],[TimesLegFlown] FROM [dbo].[tblSRMDuplicateMessageInformation]
GO

/* tblSRMMessage - IX_tblSRMMessage_FlightOriginationDate */
SELECT [FlightOriginationDate] FROM [dbo].[tblSRMMessage]
GO

/* tblSRMMessageRetryQueue - IX_tblSRMMessageRetryQueue_RetryID */
SELECT [RetryID] FROM [dbo].[tblSRMMessageRetryQueue]
GO

/* tblStandardImportConfig - IX_tblStandardImportConfig */
SELECT [StandardImportConfigGuid] FROM [dbo].[tblStandardImportConfig]
GO

/* tblStandingOffers - IX_tblStandingOffers_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblStandingOffers]
GO

/* tblStandingOffers - IX_tblStandingOffers_SiteGuid_SupplierCompanyGuid_ProductGuid_EffectiveDate_ExpirationDate_LocationIATAGuid */
SELECT [SiteGuid],[SupplierCompanyGuid],[ProductGuid],[EffectiveDate],[ExpirationDate],[LocationIATAGuid] FROM [dbo].[tblStandingOffers]
GO

/* tblStandingOffers - IX_tblStandingOffers_SupplierCompanyGuid */
SELECT [SupplierCompanyGuid] FROM [dbo].[tblStandingOffers]
GO

/* tblStations - IX_tblStations_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblStations]
GO

/* tblStations - IXU_tblStations_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblStations]
GO

/* tblSyncClientConfiguration - IX_tblSyncClientConfiguration */
SELECT [CreatedDate] FROM [dbo].[tblSyncClientConfiguration]
GO

/* tblSyncServerConfiguration - IX_tblSyncServerConfiguration */
SELECT [CreatedDate] FROM [dbo].[tblSyncServerConfiguration]
GO

/* tblSystemSettings - IX_tblSystemSettings_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblSystemSettings]
GO

/* tblTankGroups - IX_tblTankGroups_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTankGroups]
GO

/* tblTankGroups - IXU_tblTankGroups_ID_SiteGuid */
SELECT [ID],[SiteGuid] FROM [dbo].[tblTankGroups]
GO

/* tblTankMaintenanceLog - IX_tblTankMaintenanceLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTankMaintenanceLog]
GO

/* tblTankMaintenanceLog - IX_tblTankMaintenanceLog_MaintenanceReasonGuid */
SELECT [MaintenanceReasonGuid] FROM [dbo].[tblTankMaintenanceLog]
GO

/* tblTankMaintenanceLog - IX_tblTankMaintenanceLog_OperatorPersonnelGuid */
SELECT [OperatorPersonnelGuid] FROM [dbo].[tblTankMaintenanceLog]
GO

/* tblTankMaintenanceLog - IX_tblTankMaintenanceLog_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblTankMaintenanceLog]
GO

/* tblTankMaintenanceLog - IX_tblTankMaintenanceLog_TankGuid */
SELECT [TankGuid] FROM [dbo].[tblTankMaintenanceLog]
GO

/* tblTankQualityTagLog - IX_tblTankQualityTagLog_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTankQualityTagLog]
GO

/* tblTanks - IX_tblTanks_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTanks]
GO

/* tblTanks - IX_tblTanks_ManagerCompanyGuid */
SELECT [ManagerCompanyGuid] FROM [dbo].[tblTanks]
GO

/* tblTanks - IXU_tblTanks_TankID_SiteGuid */
SELECT [TankID],[SiteGuid] FROM [dbo].[tblTanks]
GO

/* tblTestDefinitions - IX_tblTestDefinitions_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTestDefinitions]
GO

/* tblTestEquipmentResults - IX_tblTestEquipmentResults_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTestEquipmentResults]
GO

/* tblTestSetDefinitions - IX_tblTestSetDefinitions_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTestSetDefinitions]
GO

/* tblTestSetEquipmentResults - IX_tblTestSetEquipmentResults_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTestSetEquipmentResults]
GO

/* tblTestSetTankResults - IX_tblTestSetTankResults_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTestSetTankResults]
GO

/* tblTestTankResults - IX_tblTestTankResults_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTestTankResults]
GO

/* tblUserDataFieldCompany - IX_tblUserDataFieldCompany_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldCompany]
GO

/* tblUserDataFieldEquipment - IX_tblUserDataFieldEquipment_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldEquipment]
GO

/* tblUserDataFieldFuelCard - IX_tblUserDataFieldFuelCard_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldFuelCard]
GO

/* tblUserDataFieldPersonnel - IX_tblUserDataFieldPersonnel_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldPersonnel]
GO

/* tblUserDataFieldProduct - IX_tblUserDataFieldProduct_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldProduct]
GO

/* tblUserDataFieldSite - IX_tblUserDataFieldSite_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldSite]
GO

/* tblUserDataFieldTransactionAlias - IX_tblUserDataFieldTransactionAlias_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldTransactionAlias]
GO

/* tblUserDataFieldTransactionAliasLineItem - IX_tblUserDataFieldTransactionAliasLineItem_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]
GO

/* tblUserDataListValueCompany - IX_tblUserDataFieldCompany_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueCompany]
GO

/* tblUserDataListValueEquipment - IX_tblUserDataFieldEquipment_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueEquipment]
GO

/* tblUserDataListValueFuelCard - IX_tblUserDataFieldFuelCard_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueFuelCard]
GO

/* tblUserDataListValuePersonnel - IX_tblUserDataFieldPersonnel_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValuePersonnel]
GO

/* tblUserDataListValueProduct - IX_tblUserDataFieldProduct_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueProduct]
GO

/* tblUserDataListValueSite - IX_tblUserDataFieldSite_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueSite]
GO

/* tblUserDataListValueTransactionAlias - IX_tblUserDataListValueTransactionAlias_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueTransactionAlias]
GO

/* tblUserDataListValueTransactionAliasLineItem - IX_tblUserDataListValueTransactionAliasLineItem_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUserDataListValueTransactionAliasLineItem]
GO

/* tblUsers - IX_tblUsers_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblUsers]
GO

/* tblUsers - IXU_tblUsers_UserGuid */
SELECT [UserGuid] FROM [dbo].[tblUsers]
GO

/* tblUsers - IXU_tblUsers_UserID_SiteGuid */
SELECT [UserID],[SiteGuid] FROM [dbo].[tblUsers]
GO

/* tblVersion - IX_tblVersion */
SELECT [CreatedDate] FROM [dbo].[tblVersion]
GO

/* tblWeightedAverageCosts - IX_tblWeightedAverageCosts_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblWeightedAverageCosts]
GO

', 
		@database_name=N'FuelsManagerDB', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
/****** Object:  Step [Load All Transasction Data]    Script Date: 4/15/2015 12:18:18 PM ******/
EXEC @ReturnCode = msdb.dbo.sp_add_jobstep @job_id=@jobId, @step_name=N'Load All Transasction Data', 
		@step_id=2, 
		@cmdexec_success_code=0, 
		@on_success_action=1, 
		@on_success_step_id=0, 
		@on_fail_action=2, 
		@on_fail_step_id=0, 
		@retry_attempts=0, 
		@retry_interval=0, 
		@os_run_priority=0, @subsystem=N'TSQL', 
		@command=N'/* tblTransactionAliases */
Select * from [dbo].[tblTransactionAliases]
GO

/* tblTransactionAliasFields */
Select * from [dbo].[tblTransactionAliasFields]
GO

/* tblTransactionLineItems */
Select * from [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItemUserData */
Select * from [dbo].[tblTransactionLineItemUserData]
GO

/* tblTransactionLinks */
Select * from [dbo].[tblTransactionLinks]
GO

/* tblTransactionNotes */
Select * from [dbo].[tblTransactionNotes]
GO

/* tblTransactionPIDX */
Select * from [dbo].[tblTransactionPIDX]
GO

/* tblTransactions */
Select * from [dbo].[tblTransactions]
GO

/* tblTransactionSignature */
Select * from [dbo].[tblTransactionSignature]
GO

/* tblTransactionSubLineItems */
Select * from [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionTransportLineItems */
Select * from [dbo].[tblTransactionTransportLineItems]
GO

/* tblTransactionUserData */
Select * from [dbo].[tblTransactionUserData]
GO

/* tblTransactionWeightReadings */
Select * from [dbo].[tblTransactionWeightReadings]
GO

/* tblTransactionAliases - IX_tblTransactionAliases_AliasName_SiteGuid */
SELECT [AliasName],[SiteGuid] FROM [dbo].[tblTransactionAliases]
GO

/* tblTransactionAliases - IX_tblTransactionAliases_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblTransactionAliases]
GO

/* tblTransactionAliases - IXU_tblTransactionAliases_SiteGuid_MasterRecordGuid */
SELECT [SiteGuid],[_MasterRecordGuid] FROM [dbo].[tblTransactionAliases]
GO

/* tblTransactionAliases - IXU_tblTransactionAliases_TransactionAliasGuid_AliasName */
SELECT [TransactionAliasGuid],[AliasName] FROM [dbo].[tblTransactionAliases]
GO

/* tblTransactionAliasFields - IX_tblTransactionAliasFields_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTransactionAliasFields]
GO

/* tblTransactionAliasFields - IX_tblTransactionAliasFields_TransactionAliasGuid_LookupTransactionFieldTypeIndex_DispatchField */
SELECT [TransactionAliasGuid],[LookupTransactionFieldTypeIndex],[DispatchField] FROM [dbo].[tblTransactionAliasFields]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_AdditiveProfileGuid */
SELECT [AdditiveProfileGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_CoveringJournalReports */
SELECT [ProductGuid],[TransactionGuid],[DeleteFlag] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_CurrencyGuid */
SELECT [CurrencyGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_DestinationCompartmentEquipmentGuid */
SELECT [DestinationCompartmentEquipmentGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_DestinationEquipmentGuid */
SELECT [DestinationEquipmentGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_LedgerCoveringIndex */
SELECT [TransactionGuid],[SequenceID] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_LoadingLocationStationGuid */
SELECT [LoadingLocationStationGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_MeterGuid */
SELECT [MeterGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_MeterGuid_TransactionGuid */
SELECT [MeterGuid],[TransactionGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_OperatorPersonnelGuid */
SELECT [OperatorPersonnelGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_OrderReferenceTransactionLineItemGuid_GrossQuantity_NetQuantity */
SELECT [OrderReferenceTransactionLineItemGuid],[GrossQuantity],[NetQuantity] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_Product */
SELECT [Product] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_ProductGuid */
SELECT [ProductGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_SourceCompartmentEquipmentGuid */
SELECT [SourceCompartmentEquipmentGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_SourceEquipmentGuid */
SELECT [SourceEquipmentGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_StorageLocationTankGuid */
SELECT [StorageLocationTankGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_TransactionGuid_SequenceID */
SELECT [TransactionGuid],[SequenceID] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_TransactionGuid_TransVersion */
SELECT [TransactionGuid],[TransVersion] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IX_tblTransactionLineItems_TransactionInventoryDate */
SELECT [TransactionInventoryDate] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItems - IXU_tblTransactionLineItems_CoveringAssociatedTransactionQueries */
SELECT [TransactionLineItemGuid] FROM [dbo].[tblTransactionLineItems]
GO

/* tblTransactionLineItemUserData - IX_tblTransactionLineItemUserData_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionLineItemUserData]
GO

/* tblTransactionLineItemUserData - IX_tblTransactionLineItemUserData_TransactionLineItemGuid */
SELECT [TransactionLineItemGuid] FROM [dbo].[tblTransactionLineItemUserData]
GO

/* tblTransactionLinks - IX_tblTransactionLinks_CreatedDate */
SELECT [CreatedDate] FROM [dbo].[tblTransactionLinks]
GO

/* tblTransactionLinks - IX_tblTransactionLinks_LinkedTransactionLineItemGuid */
SELECT [LinkedTransactionLineItemGuid] FROM [dbo].[tblTransactionLinks]
GO

/* tblTransactionLinks - IX_tblTransactionLinks_LinkedTransID */
SELECT [LinkedTransID] FROM [dbo].[tblTransactionLinks]
GO

/* tblTransactionLinks - IX_tblTransactionLinks_OriginalTransID_TransactionLineItemGuid */
SELECT [OriginalTransID],[TransactionLineItemGuid] FROM [dbo].[tblTransactionLinks]
GO

/* tblTransactionNotes - IX_tblTransactionNotes_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionNotes]
GO

/* tblTransactionNotes - IX_tblTransactionNotes_TransactionGuid */
SELECT [TransactionGuid] FROM [dbo].[tblTransactionNotes]
GO

/* tblTransactionPIDX - IX_tblTransactionPIDX_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionPIDX]
GO

/* tblTransactionPIDX - IX_tblTransactionPIDX_TransactionGuid_PIDXProfileGuid */
SELECT [TransactionGuid],[PIDXProfileGuid] FROM [dbo].[tblTransactionPIDX]
GO

/* tblTransactions - IX_tblTransactions_AuditRpt */
SELECT [DeleteFlag],[InventoryDate],[SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[LookupTransactionStatusIndex],[LookupTransTypeIndex],[CarrierCompanyGuid],[ReversalType] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_BillToCompanyGuid */
SELECT [BillToCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_CarrierCompanyGuid */
SELECT [CarrierCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_ConjoinedTransID */
SELECT [ConjoinTransID] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_CoveringJournalReports */
SELECT [OwnerCompanyGuid],[ManagerCompanyGuid],[InventoryDate],[LookupTransTypeIndex],[TransactionGuid],[DeleteFlag],[OwnerID],[AliasName],[Site] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_DailyConsumptionRpt */
SELECT [SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[CarrierCompanyGuid],[ShipToCompanyGuid],[DeleteFlag],[InventoryDate],[LookupTransactionStatusIndex],[ReversalType] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Destination1EquipmentGuid */
SELECT [Destination1EquipmentGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Destination2EquipmentGuid */
SELECT [Destination2EquipmentGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Destination3EquipmentGuid */
SELECT [Destination3EquipmentGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_FinalStationIATAGuid */
SELECT [FinalStationIATAGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_FlightArrivalFields */
SELECT [SiteGuid],[OriginStationIATAID],[ShipToID],[DestinationSerialNumber1],[ETD] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_FlightKeyFields */
SELECT [SiteGuid],[OriginStationIATAID],[FinalStationIATAID],[ShipToID],[RoutingID],[RouteOriginationDate] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_FuelCardGuid */
SELECT [FuelCardGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_GateGuid */
SELECT [GateGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_InventoryDate */
SELECT [InventoryDate] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_LedgerCovering */
SELECT [SiteGuid],[InventoryDate],[ManagerCompanyGuid],[OwnerCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_LookupTransTypeIndex */
SELECT [LookupTransTypeIndex] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_ManagerCompanyGuid */
SELECT [ManagerCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_NextStationIATAGuid */
SELECT [NextStationIATAGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_OperatorPersonnelGuid */
SELECT [OperatorPersonnelGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_OriginStationIATAGuid */
SELECT [OriginStationIATAGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_OwnerCompanyGuid */
SELECT [OwnerCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Paice_Covering */
SELECT [InventoryDate],[SiteGuid],[TransactionAliasGuid],[OwnerCompanyGuid],[ManagerCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_PreviousStationIATAGuid */
SELECT [PreviousStationIATAGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_ReasonCodeGuid */
SELECT [ReasonCodeGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_ShipperCompanyGuid */
SELECT [ShipperCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_ShipToCompanyGuid */
SELECT [ShipToCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_SiteGuid */
SELECT [SiteGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_SiteGuid_RequestedDateTime_AliasName_DeleteFlag */
SELECT [SiteGuid],[RequestedDateTime],[AliasName],[DeleteFlag] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Source1EquipmentGuid */
SELECT [Source1EquipmentGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Source2EquipmentGuid */
SELECT [Source2EquipmentGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_Source3EquipmentGuid */
SELECT [Source3EquipmentGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_SupplierCompanyGuid */
SELECT [SupplierCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_SupplierID */
SELECT [SupplierID] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_TransactionAliasGuid */
SELECT [TransactionAliasGuid],[InventoryDate],[DeleteFlag] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_TransactionStatus_DocumentNumber */
SELECT [LookupTransactionStatusIndex],[DocumentNumber] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IX_tblTransactions_TransDateTime */
SELECT [TransDateTime] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IXU_tblTransactions_CoveringAssociatedTransactionQueries */
SELECT [TransactionGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IXU_tblTransactions_CoveringPreviousVersionInformation */
SELECT [TransactionGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IXU_tblTransactions_ReversedTransID */
SELECT [ReversedTransID] FROM [dbo].[tblTransactions]
GO

/* tblTransactions - IXU_tblTransactions_TransID */
SELECT [TransID],[ManagerCompanyGuid] FROM [dbo].[tblTransactions]
GO

/* tblTransactionSignature - IX_tblTransactionSignature_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionSignature]
GO

/* tblTransactionSignature - IX_tblTransactionSignature_TransactionGuid */
SELECT [TransactionGuid] FROM [dbo].[tblTransactionSignature]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_LedgerCoveringIndex */
SELECT [TransactionGuid] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_MeterGuid */
SELECT [MeterGuid] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_ProductGuid */
SELECT [ProductGuid] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_TransactionGuid_TransVersion */
SELECT [TransactionGuid],[TransVersion] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_TransactionInventoryDate */
SELECT [TransactionInventoryDate] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionSubLineItems - IX_tblTransactionSubLineItems_TransactionLineItemGuid_SequenceID */
SELECT [TransactionLineItemGuid],[SequenceID] FROM [dbo].[tblTransactionSubLineItems]
GO

/* tblTransactionTransportLineItems - IX_tblTransactionTransportLineItems_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionTransportLineItems]
GO

/* tblTransactionTransportLineItems - IX_tblTransactionTransportLineItems_TransactionGuid_TransportOrderNumber */
SELECT [TransactionGuid],[TransportOrderNumber] FROM [dbo].[tblTransactionTransportLineItems]
GO

/* tblTransactionUserData - IX_tblTransactionUserData_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionUserData]
GO

/* tblTransactionUserData - IX_tblTransactionUserData_TransactionGuid */
SELECT [TransactionGuid] FROM [dbo].[tblTransactionUserData]
GO

/* tblTransactionWeightReadings - IX_tblTransactionWeightReadings_ClusterIdx */
SELECT [_ClusterIdx] FROM [dbo].[tblTransactionWeightReadings]
GO

/* tblTransactionWeightReadings - IX_tblTransactionWeightReadings_TransactionGuid_HistoricalFlag */
SELECT [TransactionGuid],[HistoricalFlag] FROM [dbo].[tblTransactionWeightReadings]
GO

', 
		@database_name=N'FuelsManagerDB', 
		@flags=0
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_update_job @job_id = @jobId, @start_step_id = 1
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Daily Load', 
		@enabled=1, 
		@freq_type=4, 
		@freq_interval=1, 
		@freq_subday_type=1, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20150324, 
		@active_end_date=99991231, 
		@active_start_time=20000, 
		@active_end_time=235959, 
		@schedule_uid=N'20d11e8e-fe0f-40a3-ab30-c46a996f76d8'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobschedule @job_id=@jobId, @name=N'Initial Load Schedule', 
		@enabled=1, 
		@freq_type=64, 
		@freq_interval=0, 
		@freq_subday_type=0, 
		@freq_subday_interval=0, 
		@freq_relative_interval=0, 
		@freq_recurrence_factor=0, 
		@active_start_date=20141015, 
		@active_end_date=99991231, 
		@active_start_time=0, 
		@active_end_time=235959, 
		@schedule_uid=N'd13233f2-b63d-4539-ab19-f3b7bfac62fb'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
EXEC @ReturnCode = msdb.dbo.sp_add_jobserver @job_id = @jobId, @server_name = N'(local)'
IF (@@ERROR <> 0 OR @ReturnCode <> 0) GOTO QuitWithRollback
COMMIT TRANSACTION
GOTO EndSave
QuitWithRollback:
    IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
EndSave:

GO

