/*************************************************'
* Script.FirstTimeDataUpload.sql file
* Use this file for add scripts which insert data into an empty table or brand new table (e.g. new lookup table). This file tests and whether the table is empty in order to insert the data
* For incremental insertions, e.g. adding new records to a lookup table, use the Script.IncrementalDataMaintenance.sql file instead.
**************************************************/

/*
IF (SELECT COUNT(*) FROM <TABLE_NAME_HERE>)=0
BEGIN
	
	<ADD INSERT SCRIPT HERE>

END
*/

/* POPULATE tblConfigurationSetting*/
	-- AccountingEnterpriseInterface
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = N'AuditEnabled')
		BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
		VALUES (NEWID(), N'DWORD', N'AuditEnabled', N'0', '1900-01-01','Administrator','1900-01-01','Administrator')
		END

	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'AccountingEnterpriseInterface')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','AccountingEnterpriseInterface','','1900-01-01','Administrator','1900-01-01','Administrator')
		END
	
	-- BKUtility_AdditionalFilesPaths
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_AdditionalFilesPaths')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','BKUtility_AdditionalFilesPaths','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_BUC
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_BUC')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_BUC','0','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_CurrDB
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_CurrDB')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_CurrDB','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_LogFileFullPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_LogFileFullPath')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_LogFileFullPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_LogFilePath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_LogFilePath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_LogFilePath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_Project
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_Project')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_Project','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_SQLDataRoot
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_SQLDataRoot')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_SQLDataRoot','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_SyncTechSystemHome
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_SyncTechSystemHome')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_SyncTechSystemHome','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_Ticks
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_Ticks')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_Ticks','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_xPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_xPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_xPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_yPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_yPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_yPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_ZipFilePath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_ZipFilePath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BKUtility_ZipFilePath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_zxPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_zxPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_zxPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BKUtility_zyPosition
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BKUtility_zyPosition')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BKUtility_zyPosition','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_DataPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_DataPath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_DataPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterpriseCommandTimeout
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseCommandTimeout')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_EnterpriseCommandTimeout','120','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterpriseConnectionTimeout
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseConnectionTimeout')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(NEWID(),'DWORD','BSME_EnterpriseConnectionTimeout','120','1900-01-01','Administrator','1900-01-01','Administrator') 
		END

	-- BSME_EnterpriseDataSource
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseDataSource')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_EnterpriseDataSource','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterprisePassword
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterprisePassword')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_EnterprisePassword','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterprisePort
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterprisePort')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_EnterprisePort','8089','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_EnterpriseUserID
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_EnterpriseUserID')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_EnterpriseUserID','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_LatestSequenceNumber
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_LatestSequenceNumber')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_LatestSequenceNumber','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_MaxEnterpriseConcurrentConnections
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_MaxEnterpriseConcurrentConnections')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_MaxEnterpriseConcurrentConnections','20','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_MaxExpressBatch
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_MaxExpressBatch')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_MaxExpressBatch','200','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_MFCSLogPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_MFCSLogPath')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','BSME_MFCSLogPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_ProcessingSites
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_ProcessingSites')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','BSME_ProcessingSites','SiteAdmin;Base','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_ScanFrequencySeconds
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_ScanFrequencySeconds')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_ScanFrequencySeconds','60','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- BSME_TransactionTimeoutSeconds
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'BSME_TransactionTimeoutSeconds')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','BSME_TransactionTimeoutSeconds','120','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- Common Access Card (CAC) Enable
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'Common Access Card (CAC) Enable')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','Common Access Card (CAC) Enable','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- CustomClientScriptName
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'CustomClientScriptName')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','CustomClientScriptName','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- CustomTransactionFieldAssemblyPath
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'CustomTransactionFieldAssemblyPath')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','CustomTransactionFieldAssemblyPath','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- DataDictionaryAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'DataDictionaryAssemblies')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(NEWID(),'MULTI_SZ','DataDictionaryAssemblies','FuelsManager.dll;','1900-01-01','Administrator','1900-01-01','Administrator') 
		END

	-- DISPATCH_PollTime
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'DISPATCH_PollTime')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','DISPATCH_PollTime','3','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- ExternalExportResultsInterfaceName
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'ExternalExportResultsInterfaceName')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','ExternalExportResultsInterfaceName','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- FMAETranslationsConfigurationSiteGroup
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'FMAETranslationsConfigurationSiteGroup')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','FMAETranslationsConfigurationSiteGroup','Varec','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- Geo-Tracking map icon directory path
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'GeoTrackingMapIconPath')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) 
			VALUES(NEWID(),'SZ','GeoTrackingMapIconPath','~/Areas/images/AssetMapImages/MapIcons','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- Geo-Tracking map refresh time in seconds
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'GeoTrackingMapRefreshTimeInSeconds')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) 
			VALUES(NEWID(),'DWORD','GeoTrackingMapRefreshTimeInSeconds',NULL,'1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IDependencyAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IDependencyAssemblies')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','IDependencyAssemblies','FMBusinessServices.dll','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IDiscoveryAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IDiscoveryAssemblies')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','IDiscoveryAssemblies','FuelsManager.dll;FMBusinessObjects.dll;','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- InstallDetailsSynchronizationProfileID
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'InstallDetailsSynchronizationProfileID')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','InstallDetailsSynchronizationProfileID','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsApplicationReceiversCode_GS03
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsApplicationReceiversCode_GS03')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsApplicationReceiversCode_GS03','040539587050','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsDunsNumber_ISA08
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsDunsNumber_ISA08')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsDunsNumber_ISA08','040539587','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsFuncGrpHdrVerReleaseIndustryIdCode_GS08','004030','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsInterchangeControlVersion_ISA12
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsInterchangeControlVersion_ISA12')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsInterchangeControlVersion_ISA12','00403','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- ExSTARS IRS Transportation Modes FD-Publ 3536-Motor Fuel Excise Tax EDI Guide-09	Rev 11-2005, page 14
	-- All values should be upper case and separated by "="
	-- TFS06 is 2 characters in length. Trailing spaces are added by the applicatio for codes J, B, R and S. 
	-- PRIMARY and SECONDARY storage can be optionally specified

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'HYDRANT TRUCK=J,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','HYDRANT TRUCK=J,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END


	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'GSE TRUCK=RS,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','GSE TRUCK=RS,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END
	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'HYDRANT CART=AH,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','HYDRANT CART=AH,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'STATIONARY CART=AH,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','STATIONARY CART=AH,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'FILL STAND=AH,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','FILL STAND=AH,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TANK=RT,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TANK=RT,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'FILTER=RT')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','FILTER=RT','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TANKER=J,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TANKER=J,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'PIPELINE-I=IP,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','PIPELINE-I=IP,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TRUCK-I=IJ,SECONDARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TRUCK-I=IJ,SECONDARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'RAIL-I=IR')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','RAIL-I=IR','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SHIP-I=IS')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SHIP-I=IS','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BARGE-I=IB')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BARGE-I=IB','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'PIPELINE-E=EP,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','PIPELINE-E=EP,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TRUCK-E=EJ')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TRUCK-E=EJ','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'RAIL-E=ER')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','RAIL-E=ER','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SHIP-E=ES')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SHIP-E=ES','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BARGE-E=EB')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BARGE-E=EB','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'PIPELINE=PL,PRIMARY')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','PIPELINE=PL,PRIMARY','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'TRUCK=J,SECONDARY ')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','TRUCK=J,SECONDARY ','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'RAIL=R')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','RAIL=R','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SHIP=S')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SHIP=S','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BARGE=B')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BARGE=B','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'BOOK ADJUSTMENT=BA')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','BOOK ADJUSTMENT=BA','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'SUMMARY=CE')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','SUMMARY=CE','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsIrsTransportModes
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsIrsTransportModes' AND SettingValue = 'REMOVE FROM TERMINAL=RT')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsIrsTransportModes','REMOVE FROM TERMINAL=RT','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsExStarsISA05Qualifier
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsISA05Qualifier')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsISA05Qualifier','32','1900-01-01','Administrator','1900-01-01','Administrator')
		END

			-- IrsExStarsEnableDebugFeatures
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsExStarsEnableDebugFeatures')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','IrsExStarsEnableDebugFeatures','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- IrsProductCodesRegEx
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'IrsProductCodesRegEx')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
	VALUES(NEWID(),'SZ','IrsProductCodesRegEx','^((E|M|B|D)\d\d)|(090|125|248|122|055|249|093|126|059|223|121|199|100|076|198|224|161|167|150|154|282|283|226|227|231|153|052|196|065|058|145|147|073|074|130|077|225|279|280|265|281|054|075|092|001|049|188|960|285|091)$','1900-01-01','Administrator','1900-01-01','Administrator') 
		END

	-- ISecurityAssemblies
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'ISecurityAssemblies')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'MULTI_SZ','ISecurityAssemblies','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- LoadRackInstalled
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'LoadRackInstalled')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','LoadRackInstalled','1','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- LoadRackPort
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'LoadRackPort')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','LoadRackPort','8087','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- LR_QualityAssuranceInterface
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'LR_QualityAssuranceInterface')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','LR_QualityAssuranceInterface','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- MaxConcurrentSessionsPerUser
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'MaxConcurrentSessionsPerUser')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'DWORD','MaxConcurrentSessionsPerUser','100000','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- NSPA_FuelCardImportConnectionString
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'NSPA_FuelCardImportConnectionString')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','NSPA_FuelCardImportConnectionString','Provider=Microsoft.ACE.OLEDB.12.0;Data Source=<filename>;Extended Properties="Excel 12.0 Xml;HDR=YES;IMEX=1;"','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineAdminDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineAdminDoc')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineAdminDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineAdminTutorialDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineAdminTutorialDoc')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineAdminTutorialDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineHelpDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineHelpDoc')
		BEGIN
	INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineHelpDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- OnlineTutorialDoc
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'OnlineTutorialDoc')
		BEGIN
			INSERT INTO dbo.tblConfigurationSetting(ConfigurationSettingGuid,KeyType,SettingKey,SettingValue,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) 
			VALUES(NEWID(),'SZ','OnlineTutorialDoc','','1900-01-01','Administrator','1900-01-01','Administrator')
		END

	-- DefaultHelpUrl
	if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = 'DefaultHelpURL')
		BEGIN
			INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
			VALUES (N'af1b528a-aed3-4437-a4a2-3268df8c6aa3', N'SZ', N'DefaultHelpURL', N'http://localhost/fmhelp/', N'9/24/2012 11:24:00 AM -04:00', N'Administrator', N'9/24/2012 11:24:00 AM -04:00', N'Administrator')
		END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'InstallDetailsSynchronizationNodeGuid')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f5d0fb9a-95a4-4c9e-8e19-0dd3766aecc1', N'SZ', N'InstallDetailsSynchronizationNodeGuid', N'', N'10/1/2012 3:57:48 PM -04:00', N'Administrator', N'10/1/2012 3:57:48 PM -04:00', N'Administrator')
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'InstallDetailsSynchronizationNodeName')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C0601CA3-5DB5-441B-A75D-CDBD4379FD19', N'SZ', N'InstallDetailsSynchronizationNodeName', N'', N'10/1/2012 3:57:48 PM -04:00', N'Administrator', N'10/1/2012 3:57:48 PM -04:00', N'Administrator')
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityPushEnabled')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1875F86D-0259-4060-BDDE-1EF32F81CE80', N'DWORD', N'EnterpriseVisibilityPushEnabled', N'0', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityOpcUaServerUrl')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'685EDE4E-4587-4756-B494-CE7E80E5772B', N'SZ', N'EnterpriseVisibilityOpcUaServerUrl', N'http://localhost:40002/FuelsManager/OpcUaServer', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityTagsPerCall')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C7CB6F76-C09B-4F5A-A4AA-5AEDA480268F', N'DWORD', N'EnterpriseVisibilityTagsPerCall', N'4096', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityPushPeriod')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'094D1558-DC24-492F-A7B0-A681E40E8F5B', N'SZ', N'EnterpriseVisibilityPushPeriod', N'5.0', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilitySecurityMode')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'AE1BAF9B-F08E-421F-868B-7B5ABBB6DE42', N'SZ', N'EnterpriseVisibilitySecurityMode', N'None', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilitySecurityPolicy')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1DCA9B4B-BBB6-4175-A4A8-9626973BB6AA', N'SZ', N'EnterpriseVisibilitySecurityPolicy', N'None', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityMessageEncoding')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'BE8F46EA-3ECE-4E00-9C9B-19B9E40D46B6', N'SZ', N'EnterpriseVisibilityMessageEncoding', N'Binary', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityUserIdentity')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'A9B0F1AD-67C8-4641-B9F6-8E1104BEE991', N'SZ', N'EnterpriseVisibilityUserIdentity', N'Certificate', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityCertificatePath')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'B23F02F8-AC88-425A-8715-475014B0F775', N'SZ', N'EnterpriseVisibilityCertificatePath', N'', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityUserName')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'E34232CF-C43B-4592-847A-6F3A43E1C637', N'SZ', N'EnterpriseVisibilityUserName', N'', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'EnterpriseVisibilityPassword')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1787C8CA-510B-428D-B03B-029BCE4D4472', N'PWD', N'EnterpriseVisibilityPassword', N'', N'12/10/2015 08:00:00 AM -05:00', 'Administrator', N'12/10/2015 08:00:00 AM -05:00', 'Administrator');
	END
	
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'Enterprise_FilePathsTempFilePath')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'33b280c0-40cc-47a2-b874-d3ac7b411a7b', N'SZ', N'Enterprise_FilePathsTempFilePath', NULL, N'3/12/2013 1:55:01 PM +00:00', N'Administrator', N'3/12/2013 1:55:01 PM +00:00', N'Administrator')
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMActiverDirectoryManageSvr_TestModeFlag' OR SettingKey = 'ActiveDirectoryManageSvr_TestModeFlag')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8945D495-9110-4EDB-B889-8B5E4BB6FFCC', N'SZ', N'ActiveDirectoryManageSvr_TestModeFlag', N'', N'8/5/2019 3:57:48 PM -04:00', N'Administrator', N'8/5/2019 3:57:48 PM -04:00', N'Administrator')
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMActiverDirectoryManageSvr_TestModeFilePath' OR SettingKey = 'ActiveDirectoryManageSvr_TestModeFilePath')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1DD66004-82E2-4432-9BD3-7049B1E3B44F', N'SZ', N'ActiveDirectoryManageSvr_TestModeFilePath', N'', N'8/5/2019 3:57:48 PM -04:00', N'Administrator', N'8/5/2019 3:57:48 PM -04:00', N'Administrator')
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMActiverDirectoryManageSvr_SleepIntervalTime' OR SettingKey = 'ActiveDirectoryManageSvr_SleepIntervalTime')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4A254D92-406D-4DE9-B489-DA9E51D8794D', N'DWORD', N'ActiveDirectoryManageSvr_SleepIntervalTime', N'', N'8/5/2019 3:57:48 PM -04:00', N'Administrator', N'8/5/2019 3:57:48 PM -04:00', N'Administrator')
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMService_AuditLogProcessBatchCount')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'453BAE31-67D9-4465-B2E1-EE4C831AE312', N'DWORD', N'FMService_AuditLogProcessBatchCount', '400', N'04/15/2019 12:00:01 AM -04:00', N'Varec', N'04/15/2019 12:00:01 AM -04:00', N'Varec')	
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'QueryWriterAssemblies')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'B099F194-D1D7-43A5-87E7-7FE54D7FC3F1', N'MULTI_SZ', N'QueryWriterAssemblies', 'FMBusinessObjects', N'05/10/2020 12:00:01 AM -04:00', N'Varec', N'05/10/2020 12:00:01 AM -04:00', N'Varec')	
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'SingleSignOnMode')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'30DAB17F-6CC3-4C04-9196-6EDA5EAB600B', N'DWORD', N'SingleSignOnMode', '0', N'12/12/2022 12:00:01 AM -05:00', N'Varec', N'12/12/2022 12:00:01 AM -05:00', N'Administrator')	
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'SynchronizedSettings')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1878C3B4-942F-4459-89D0-664D4F896AD9', N'MULTI_SZ', N'SynchronizedSettings', 'SynchronizedSettings', N'12/12/2022 12:00:01 AM -05:00', N'Varec', N'12/12/2022 12:00:01 AM -05:00', N'Administrator')	
	END

	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'CustomApplicationType')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES 
		(N'4851BD16-87EC-4AF9-B201-6181B3539AB0', N'SZ', N'CustomApplicationType', '', N'09/16/2025 12:00:01 AM -05:00', N'Varec', N'09/16/2025 12:00:01 AM -05:00', N'Administrator')	
	END
	
	-- PointCalculatorRowVisibilityConfig
	IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'PointCalculatorRowVisibilityConfig')
	BEGIN
		INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'51F90D16-1EE2-4789-BBB8-E2CB16A5ECCA', N'SZ', N'PointCalculatorRowVisibilityConfig', '4294967295', N'06/16/2026 09:00:01 AM -05:00', N'Varec', N'06/16/2026 09:00:01 AM -05:00', N'Administrator')	
	END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MovementNotifyAssembly') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('77abd09c-d223-424e-816c-03a1fc41a734', N'SZ', 'MovementNotifyAssembly', '', N'10/16/2025 5:00:00 PM -04:00', 'Administrator', N'10/16/2025 5:00:00 PM -04:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAggregateField])=0
BEGIN

	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'NetGross', N'Net Gross', N'0a6ec3c7-7973-4c77-a253-623dd410e71a', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Number01', N'Number 1', N'c555d6d1-0c67-4389-9f53-5eb52f74a121', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Number02', N'Number 2', N'05da2e4d-d192-41e9-a045-4772787e2faf', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'Number03', N'Number 3', N'fa93bfe7-f0fa-4ace-a0ad-5aeb54e1d957', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'Number04', N'Number 4', N'3a436c2f-a790-4654-805d-b5cfdbc67935', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'Number05', N'Number 5', N'c216681a-5444-4d57-a1a2-abd07290cd36', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'Number06', N'Number 6', N'b65e74a2-da79-45f3-a30a-00b952a0dc30', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAggregateField] ([AggregateFieldIndex], [AggregateFieldCode], [AggregateFieldName], [AggregateFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'CustomFunction', N'Custom Function', N'b41f2ec0-74ee-408c-935f-f16275e0192d', N'6/18/2012 1:04:15 PM +00:00', N'Administrator', N'6/18/2012 1:04:15 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAirplaneTankLocation])=0
BEGIN
	INSERT INTO [lookup].[tblAirplaneTankLocation] ([TankLocationIndex], [TankLocationCode], [TankLocationName], [TankLocationGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'CENTER', N'Center', N'c0e1b971-e501-4c62-8a4f-d01e270117ee', N'10/23/2012 9:46:57 AM +00:00', N'Administrator', N'10/23/2012 9:46:57 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAirplaneTankLocation] ([TankLocationIndex], [TankLocationCode], [TankLocationName], [TankLocationGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'LEFT', N'Left', N'6508201d-0cec-49da-bb98-6a91ac4e7231', N'10/23/2012 9:46:57 AM +00:00', N'Administrator', N'10/23/2012 9:46:57 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAirplaneTankLocation] ([TankLocationIndex], [TankLocationCode], [TankLocationName], [TankLocationGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'RIGHT', N'Right', N'ce06605e-8320-44f1-8afd-87f375457d96', N'10/23/2012 9:46:57 AM +00:00', N'Administrator', N'10/23/2012 9:46:57 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAirplaneTankToleranceType])=0
BEGIN
	INSERT INTO [lookup].[tblAirplaneTankToleranceType] ([TankToleranceTypeIndex], [TankToleranceTypeCode], [TankToleranceTypeName], [TankToleranceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'MASS', N'Mass', N'2afe88dd-414a-4a63-a6e0-a697d05258ec', N'10/23/2012 9:47:06 AM +00:00', N'Administrator', N'10/23/2012 9:47:06 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAirplaneTankToleranceType] ([TankToleranceTypeIndex], [TankToleranceTypeCode], [TankToleranceTypeName], [TankToleranceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'VOLUME', N'Volume', N'a2264915-261e-46a1-8c9b-c62636777d70', N'10/23/2012 9:47:06 AM +00:00', N'Administrator', N'10/23/2012 9:47:06 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAirplaneTankToleranceType] ([TankToleranceTypeIndex], [TankToleranceTypeCode], [TankToleranceTypeName], [TankToleranceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'PERCENTAGE', N'Percentage', N'940fee11-3b3c-4168-9f7c-90f8e3a1fe2c', N'10/23/2012 9:47:06 AM +00:00', N'Administrator', N'10/23/2012 9:47:06 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAllocationType])=0
BEGIN
	INSERT INTO [lookup].[tblAllocationType] ([AllocationTypeIndex], [AllocationTypeCode], [AllocationTypeName], [AllocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'PRODUCT_ALLOCATION', N'PRODUCT ALLOCATION', N'9fa21fb2-c114-4f79-bc4e-3b1c90993c8e', N'6/18/2012 9:06:06 AM +00:00', N'Administrator', N'6/18/2012 9:06:06 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAllocationType] ([AllocationTypeIndex], [AllocationTypeCode], [AllocationTypeName], [AllocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'PRODUCT_GROUP_ALLOCATION', N'PRODUCT GROUP ALLOCATION', N'b6736d49-0421-4c47-af83-5ec552f699e0', N'6/18/2012 9:06:06 AM +00:00', N'Administrator', N'6/18/2012 9:06:06 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAllocationType] ([AllocationTypeIndex], [AllocationTypeCode], [AllocationTypeName], [AllocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'ALL_PRODUCTS_ALLOCATION', N'ALL PRODUCTS ALLOCATION', N'd541aff4-2e01-40c9-b620-de344f7895d1', N'6/18/2012 9:06:06 AM +00:00', N'Administrator', N'6/18/2012 9:06:06 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAllocationType] ([AllocationTypeIndex], [AllocationTypeCode], [AllocationTypeName], [AllocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'MAX_ALLOCATION_TYPE', N'MAX ALLOCATION TYPE', N'26885652-7e6f-4719-a0c0-bbed548951fa', N'6/18/2012 9:06:06 AM +00:00', N'Administrator', N'6/18/2012 9:06:06 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAssetTrackingDeviceType]) = 0
BEGIN
	INSERT INTO [lookup].[tblAssetTrackingDeviceType] ([AssetTrackingDeviceTypeIndex], [AssetTrackingDeviceTypeCode], [AssetTrackingDeviceTypeName], [AssetTrackingDeviceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'TDU', N'TDU', N'C38A8A6D-B31B-4EA0-9BAC-B2944B390D7D', N'4/7/2016 1:03:42 PM +00:00', N'Administrator', N'4/7/2016 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingDeviceType] ([AssetTrackingDeviceTypeIndex], [AssetTrackingDeviceTypeCode], [AssetTrackingDeviceTypeName], [AssetTrackingDeviceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'WRDCU', N'WRDCU', N'D4095B29-2D62-4F6D-AA4C-5326853DC932', N'4/7/2016 1:03:42 PM +00:00', N'Administrator', N'4/7/2016 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingDeviceType] ([AssetTrackingDeviceTypeIndex], [AssetTrackingDeviceTypeCode], [AssetTrackingDeviceTypeName], [AssetTrackingDeviceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'STANDARD', N'Standard', N'988634A2-6424-4CBB-B3FA-3A7E3BC2DCA0', N'4/7/2016 1:03:42 PM +00:00', N'Administrator', N'4/7/2016 1:03:42 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAssetTrackingPayloadType]) = 0
BEGIN
	INSERT INTO [lookup].[tblAssetTrackingPayloadType] ([AssetTrackingPayloadTypeIndex], [AssetTrackingPayloadTypeCode], [AssetTrackingPayloadTypeName], [AssetTrackingPayloadTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'NONE', N'None', N'E9788777-C7E7-4AFA-AEDB-7B89BDA7EEF3', N'2/28/2017 1:03:42 PM +00:00', N'Administrator', N'2/28/2017 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingPayloadType] ([AssetTrackingPayloadTypeIndex], [AssetTrackingPayloadTypeCode], [AssetTrackingPayloadTypeName], [AssetTrackingPayloadTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TDU', N'TDU', N'BA686E43-4F2F-413B-B487-835A995B7EE6', N'2/28/2017 1:03:42 PM +00:00', N'Administrator', N'2/28/2017 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingPayloadType] ([AssetTrackingPayloadTypeIndex], [AssetTrackingPayloadTypeCode], [AssetTrackingPayloadTypeName], [AssetTrackingPayloadTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'WRDCU', N'WRDCU', N'1B494D7D-6AE3-4BCD-96A8-0DD19865A866', N'2/28/2017 1:03:42 PM +00:00', N'Administrator', N'2/28/2017 1:03:42 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblAssetTrackingMessageState]) = 0
BEGIN
	INSERT INTO [lookup].[tblAssetTrackingMessageState] ([AssetTrackingMessageStateIndex], [AssetTrackingMessageStateCode], [AssetTrackingMessageStateName], [AssetTrackingMessageStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'NONE', N'None', N'F44910D1-6A6F-44BB-9398-49BECF334734', N'4/19/2018 1:03:42 PM +00:00', N'Administrator', N'4/19/2018 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingMessageState] ([AssetTrackingMessageStateIndex], [AssetTrackingMessageStateCode], [AssetTrackingMessageStateName], [AssetTrackingMessageStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'CONTAMINATED', N'Contaminated', N'5C86ADDD-6347-4619-AF4A-A0B5CFC57460', N'4/19/2018 1:03:42 PM +00:00', N'Administrator', N'4/19/2018 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingMessageState] ([AssetTrackingMessageStateIndex], [AssetTrackingMessageStateCode], [AssetTrackingMessageStateName], [AssetTrackingMessageStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'INVESTIGATE', N'Investigate', N'B4D052B7-F8A4-4CD2-950A-80F356420FB8', N'4/19/2018 1:03:42 PM +00:00', N'Administrator', N'4/19/2018 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingMessageState] ([AssetTrackingMessageStateIndex], [AssetTrackingMessageStateCode], [AssetTrackingMessageStateName], [AssetTrackingMessageStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'INVESTIGATION_COMPLETED_FAILED', N'Investigation Completed Failed', N'D952E7E3-1745-4CC4-B186-FF3AC93D552C', N'4/19/2018 1:03:42 PM +00:00', N'Administrator', N'4/19/2018 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblAssetTrackingMessageState] ([AssetTrackingMessageStateIndex], [AssetTrackingMessageStateCode], [AssetTrackingMessageStateName], [AssetTrackingMessageStateGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'INVESTIGATION_COMPLETED_PASSED', N'Investigation Completed Passed', N'06B0DE8A-75B2-4A5E-9B53-458DD92D2844', N'4/19/2018 1:03:42 PM +00:00', N'Administrator', N'4/19/2018 1:03:42 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblChangeQueueRecordType])=0
BEGIN
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'None', N'None', N'39a05ba4-83da-4502-9db8-d29042666821', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Companies', N'Companies', N'5de9ee8c-1e32-4361-8e97-6d2f4838d9f8', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Equipment', N'Equipment', N'e33bd9cb-3679-46f9-b31d-51f5c328d0b3', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'FuelCards', N'FuelCards', N'06c6753b-e289-41a1-bd9e-701106f7172b', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'Personnel', N'Personnel', N'2c37dc18-60d2-4706-9bfe-557bb64e94bc', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'Products', N'Products', N'74ab67d8-15ff-4889-80dd-eaa3c0e418c3', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'Transactions', N'Transactions', N'acf31bf5-7f18-45f3-b55e-c0ea58852197', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'Groups', N'Groups', N'9af766c7-23b0-4066-bd14-91f00b0e171e', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'TransactionAliases', N'Transaction Aliases', N'6d58f5df-4b21-47a5-9c60-4e86bfa5b6c6', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'CloseoutDO', N'Closeout DO', N'e01c5ab1-3a4f-447b-9eb8-fc5bba3a91d6', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'ApplicationStrings', N'Application Strings', N'933f53af-9237-4494-b96e-592da4e4ac86', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'PIDXProfiles', N'PIDX Profiles', N'dcfaaddd-6dd3-46d9-985e-4c84f5e2759f', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblChangeQueueRecordType] ([ChangeQueueRecordTypeIndex], [ChangeQueueRecordTypeCode], [ChangeQueueRecordTypeName], [ChangeQueueRecordTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'PIDXProfileCompanyMaps', N'PIDX Profile Company Maps', N'48c9e0c3-b274-488b-b627-ddda6401133a', N'6/18/2012 1:04:33 PM +00:00', N'Administrator', N'6/18/2012 1:04:33 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblCompanyCrossReferenceType])=0
BEGIN
	INSERT INTO [lookup].[tblCompanyCrossReferenceType] ([CompanyCrossReferenceTypeIndex], [ReferenceTypeName], [CompanyCrossReferenceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Navy', N'c56367b9-7b8f-49da-a5ce-d427b1000076', N'5/28/2013 2:25:58 PM -04:00', N'Administrator', N'5/28/2013 2:25:58 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyCrossReferenceType] ([CompanyCrossReferenceTypeIndex], [ReferenceTypeName], [CompanyCrossReferenceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Other', N'699d5d79-5801-4686-ac10-0f5e8a5b1aa4', N'5/28/2013 2:25:58 PM -04:00', N'Administrator', N'5/28/2013 2:25:58 PM -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblCompanyMapType])=0
BEGIN
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'LOAD_OWNER_MANAGER_MAP', N'LOAD OWNER MANAGER MAP', N'f2a80c16-5acd-48c8-97b4-6999ea2ebafe', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'SHIPPER_OWNER_MAP', N'SHIPPER OWNER MAP', N'c405b7b7-3d7a-474d-b0c1-503a390d9a5a', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'BILLTO_SHIPPER_MAP', N'BILLTO SHIPPER MAP', N'e230c803-6acf-4901-9ac0-7326c6e61bda', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'SHIPTO_BILLTO_MAP', N'SHIPTO BILLTO MAP', N'dd3d9e29-17a3-46e3-b51f-dfdb21bfd71d', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'AUTHORIZED_CARRIER_MAP', N'AUTHORIZED CARRIER MAP', N'6b332f3c-38cb-4c55-83ad-ee362093e557', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'LOADID_SHIPTO_MAP', N'LOADID SHIPTO MAP', N'1e563cce-a470-4c53-bb22-0b3187ab7afe', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'USER_GROUP_COMPANY_MAP', N'USER GROUP COMPANY MAP', N'34637e0c-656b-4242-a1ea-c3eed10aa293', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'COMPANY_GROUP_COMPANY_MAP', N'COMPANY GROUP COMPANY MAP', N'67da085e-2f7e-4425-9f6d-abf2c7156b08', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'FOOT_NOTE_SHIPTO_MAP', N'FOOT NOTE SHIPTO MAP', N'd36336ea-feef-4334-b9a9-f65b95bb7293', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'FOOT_NOTE_SHIPPER_MAP', N'FOOT NOTE SHIPPER MAP', N'2f710273-28d7-4be9-8cd8-ee97c0d301c6', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'LOAD_MAX_COMPANY_MAP_TYPE', N'LOAD MAX COMPANY MAP TYPE', N'87f6febb-c954-434a-a28c-3d87689c0f9f', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'SUPPLIER_OWNER_MAP', N'SUPPLIER OWNER MAP', N'b94679b4-ae71-41ff-85fd-c1ff98309ebb', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'OFFLOADID_SUPPLIER_MAP', N'OFFLOADID SUPPLIER MAP', N'2c229eaf-c09d-47a6-a545-67e81c57525e', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'OFFLOAD_MAX_COMPANY_MAP_TYPE', N'OFFLOAD MAX COMPANY MAP TYPE', N'02c9ff87-e963-49a4-8269-437763b9d840', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex], [CompanyMapTypeCode], [CompanyMapTypeName], [CompanyMapTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'OFFLOAD_OWNER_MANAGER_MAP', N'OFFLOAD OWNER MANAGER MAP', N'443e0fa9-27aa-46d9-abf5-4e1ef832fb88', N'6/18/2012 9:06:21 AM +00:00', N'Administrator', N'6/18/2012 9:06:21 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblCompanyRole])=0
BEGIN
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'MANAGER', N'Manager', N'e712436e-8f32-465b-9b02-7401a5720853', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'OWNER', N'Owner', N'681b447b-c2ef-4a51-ba76-4da3fdda3902', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'SHIPPER', N'Shipper', N'df046548-7975-4c8c-98ce-a02a712c22f6', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'CUSTOMER_BILLTO', N'Customer bill to', N'ed8be11b-ab33-4472-af30-2091e4050e9a', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'CUSTOMER_SHIPTO', N'Customer ship to', N'ca0b9285-d5f8-4495-8135-225f6e5b5410', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'CARRIER', N'Carrier', N'67fb60bd-8ca6-49c3-b053-d5acc3f744e9', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'SUPPLIER', N'Supplier', N'f4ef5a79-5ee2-4541-b505-98025473181c', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'MAX_COMPANY_ROLE', N'Max company role', N'8a0af30f-50da-4522-b473-36b56feb0135', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCompanyRole] ([CompanyRoleIndex], [CompanyRoleCode], [CompanyRoleName], [CompanyRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'NO_COMPANY_ROLE', N'No company role', N'6e9f26a7-8d45-478e-84ef-c9f827f559f9', N'6/15/2012 9:20:12 AM +00:00', N'Administrator', N'6/15/2012 9:20:12 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblCurrencyUnit])=0
BEGIN
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Dollar', N'Dollar', N'c681ef1f-6cb4-4401-b6b6-d4723ebd66db', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Franc', N'Franc', N'336860b3-375a-4412-ba72-e99f10fd3c79', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'Pound', N'Pound', N'cc2b7fd4-b53e-4b6a-b43f-9ebd3223f851', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'Peso', N'Peso', N'65d3c2d0-0438-4ad2-917d-888c301bf470', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'Afghani', N'Afghani', N'87c33f93-3d60-486c-af97-4eee6d7ba056', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'Lek', N'Lek', N'8f71d16c-3e67-4a10-adaf-97dba2cd975a', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'Dinar', N'Dinar', N'fd9db247-e3a8-40c1-bdb4-4a05539ab8d0', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'Kwanza', N'Kwanza', N'2738179e-168f-43a0-b84d-4d3fcc39c858', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'Dram', N'Dram', N'62d6a1a0-22c7-4466-8960-f6e4f73a6db6', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'Florin', N'Florin', N'5ea8c86d-6ba8-4de3-9745-9bf716b5951c', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'Manat', N'Manat', N'75afb0fc-2b1d-4bea-aa99-ce56bf8acaff', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'Taka', N'Taka', N'c6ef496a-257b-4ead-badd-69ac334eaafe', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'Ruble', N'Ruble', N'3e0a6820-7e06-4d71-b458-ddcaf68e303d', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'Ngultrum', N'Ngultrum', N'1c6cd543-84d8-4629-8b69-545e4d42c077', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'Boliviano', N'Boliviano', N'9dadb28a-c3f4-4327-84e8-8dbc6054319d', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'Marka', N'Marka', N'7a357018-0955-4c0b-85fb-370fcded9820', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'Pula', N'Pula', N'f5bec93d-2c6d-4039-84a3-807276fbe22c', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'Real', N'Real', N'd56f2a4d-76b7-4c5e-8861-b7f2a9e37a70', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'Lev', N'Lev', N'0ba028fb-9dd7-4c6d-9010-e70800d2fcfd', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'Riel', N'Riel', N'af6083ef-8e2a-4c6b-9fdc-ec4761df7a07', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'Escudo', N'Escudo', N'1238999a-5085-43cc-b987-6d92e45a2c27', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'Renminbi', N'Renminbi', N'68af1cb3-71f4-4887-8264-8a7d1b3d9552', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'Colon', N'Colon', N'9b968e79-1171-4c0f-b266-d259ee5d2dfb', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (24, N'Kuna', N'Kuna', N'2f276d54-7b0d-4e06-ba20-8c12f58a4142', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (25, N'Koruna', N'Koruna', N'184d7847-9870-443b-b05d-6211db72f368', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'Krone', N'Krone', N'93a45a71-95de-4160-93fb-49f6c7e0d2f2', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (27, N'Nakfa', N'Nakfa', N'33641809-f912-436e-a283-2f53cf0a5b1d', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (28, N'Kroon', N'Kroon', N'45c3b45f-ee49-4d86-b6ef-503b4f06c1f4', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (29, N'Birr', N'Birr', N'd5e2f5df-7702-461b-8876-6a1898b84931', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (30, N'Dalasi', N'Dalasi', N'f7e7e9a1-b720-4593-b492-3892df5ed084', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (31, N'Lari', N'Lari', N'81c83032-e229-44cb-9865-ed93ba85ea57', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (32, N'Cedi', N'Cedi', N'3c80f6e9-5ab3-483b-881f-f27b3d6ad55a', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (33, N'Quetzal', N'Quetzal', N'3ca3db83-e503-41a1-804d-645ef9b02041', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (34, N'Gourde', N'Gourde', N'686feaa4-f102-4182-a235-9bb73ea3d42b', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (35, N'Lempira', N'Lempira', N'74c8a5ed-8a73-4263-ac80-db60339d0942', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (36, N'Forint', N'Forint', N'8717a41f-01cf-4102-b079-4ba3b6f1f35e', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (37, N'Rupee', N'Rupee', N'22ce69ad-7115-416c-92f3-24a6ba46d796', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (38, N'Rupiah', N'Rupiah', N'be3d7a3d-3dd8-4c2d-bc24-5582fd92bd8b', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (39, N'Yen', N'Yen', N'74b1e567-961c-4fac-8c26-5eadcce23745', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (40, N'Tenge', N'Tenge', N'7f75694b-0300-4fb2-b830-31921c931775', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (41, N'Shilling', N'Shilling', N'507fc93f-c991-4db1-98e8-dbd14c8d4b45', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (42, N'Won', N'Won', N'ccb1d20d-9248-4416-accd-bf7f4fd15cd0', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (43, N'Som', N'Som', N'26813d4a-eecf-4c62-98ea-c1fde3e0ae8b', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (44, N'Kip', N'Kip', N'b1eec948-1a5e-4200-bd9d-1b801071c9bd', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (45, N'Lats', N'Lats', N'd291a508-c811-42a3-bcd0-d6099ed93215', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (46, N'Loti', N'Loti', N'e387240b-597e-4d1b-8a2d-3b3c11d5608b', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (47, N'Denar', N'Denar', N'11fde730-47b8-4a81-a0ac-89c4715ab3aa', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (48, N'Ariary', N'Ariary', N'e49c32aa-d0fc-4388-b196-35da8201972a', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (49, N'Kwacha', N'Kwacha', N'96f0ff3a-e184-4d74-a517-c7eb95786f1e', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (50, N'Dirham', N'Dirham', N'6763f46f-229f-41b7-9239-b46ea83102a3', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (51, N'Kina', N'Kina', N'2535fc9e-5faf-49fc-bac6-55d4533649a0', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (52, N'Nuevo_Sol', N'Nuevo Sol', N'858c88ae-e873-4ad4-978f-fe2577e93c7b', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (53, N'Zloty', N'Zloty', N'e4077dae-8fc6-4c53-9507-18168071dff8', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (54, N'Riyal', N'Riyal', N'51eea9f2-3929-4237-8167-f1658da929f6', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblCurrencyUnit] ([CurrencyUnitIndex], [CurrencyUnitCode], [CurrencyUnitName], [CurrencyUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (55, N'Euro', N'Euro', N'9faf716f-68a6-465b-9bc2-87ee41acbe3b', N'6/18/2012 1:04:21 PM +00:00', N'Administrator', N'6/18/2012 1:04:21 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblCustomToolbarType])=0
BEGIN
	INSERT INTO [lookup].[tblCustomToolbarType] ([CustomToolbarTypeIndex], [CustomToolbarTypeCode], [CustomToolbarTypeName], [CustomToolbarTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (-1, N'UNKNOWN_TOOLBAR_TYPE', N'Unknown Toolbar Type', N'69da3ec3-ec0c-456b-a355-5c6fd4f125bc', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarType] ([CustomToolbarTypeIndex], [CustomToolbarTypeCode], [CustomToolbarTypeName], [CustomToolbarTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'DISPATCH_TABULAR_VIEW', N'Dispatch Tabular View', N'333ca367-33e7-4bde-920f-baa7c4c74c28', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblCustomToolbarCommandType])=0
BEGIN
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (-1, N'UNKNOWN_TOOLBAR_COMMAND_TYPE', N'Unknown Toolbar Command Type', -1, N'72d61e43-eeea-4128-8169-b654f784a35f', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TRANSACTION_ALIAS', N'Transaction Alias', -1, N'91c765a4-c3ee-4b08-b2aa-9350accdedda', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (101, N'ARRIVAL', N'Arrival', 1, N'5f4e66f7-8eaa-46cf-9318-a4873db52f24', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (102, N'CANCEL', N'Cancel', 1, N'ce2a7b8d-f6e3-4425-8416-15207888d33c', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator',1,11,'toolStripCancelButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (103, N'CHANGE_OPERATOR_STATUS', N'Change Operator Status', 1, N'6264fd4b-f1da-486f-8baf-608cd8685eab', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (104, N'CONTROL_LOG', N'Control Log', 1, N'9f8670fa-4c22-4ad2-a1d6-5dac1d5f1c62', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator',1,7,'toolStripControlLogButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (105, N'DISPATCHING_VIEW', N'Dispatch', 1, N'adc86c86-1571-4626-a04d-f59f05afa9b3', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator',1,6,'toolStripDispatchButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder]) VALUES (106, N'DISPATCHERS_LIST', N'Dispatchers List', 1, N'3c1446f3-b193-4623-b039-03f70bce0812', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator',1,10)
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (107, N'EVACUATE', N'Evacuate', 1, N'c16474a4-5f61-4278-8344-46ed805f762c', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (108, N'RELEASE_TO_ACCOUNTING', N'Release To Accounting', 1, N'dc30e273-d0bf-4f96-95b4-d4ebffb85b6c', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (109, N'FAST_LOG', N'Fast Log', 1, N'0895ea41-9919-4452-8a5f-fdd9d7b5707e', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 1,3,'toolStripFastLogButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (110, N'FAST_LOG_FILLSTAND', N'Fast Log Fillstand', 1, N'e5c38546-0bbf-4114-87a0-d2556584a6ad', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator',1,4,'toolStripFastLogFillstandButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (111, N'FILLSTAND_COMPLETION', N'Fillstand Completion', 1, N'86b78f20-df27-4cbe-895a-5519cb7d6247', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (113, N'FLIGHT_LINE_STATUS', N'Flight Line', 1, N'748583d4-a7ec-42e6-a1ab-0a46da2cf720', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 1,9,'toolStripFlightLineButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (115, N'HELP', N'Help', 1, N'6f93d95e-f946-4ab5-a784-d3207033c564', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (116, N'OPTIONAL_TIMES', N'Optional Times', 1, N'c04c87a4-f13a-4176-88eb-32fc09957e9d', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (117, N'QUERY_WRITER', N'Query Writer', 1, N'bea344ff-9c0a-4dc2-9e37-9382da4497d5', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (118, N'RECIRCULATION', N'Recirculation', 1, N'b0c84267-202e-4009-a055-94527dca80b2', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [Default], [DefaultOrder], [ImageSource]) VALUES (119, N'RELOG', N'Relog', 1, N'0114f009-d423-431f-b4be-4ad4fc960937', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator',1,5,'toolStripCopyButton.Image.png')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (120, N'REPORTS', N'Reports', 1, N'707885e6-3d3c-495d-8076-7aa6f4130f67', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (121, N'REQUEST', N'Request', 1, N'057f62e1-e250-439a-b268-5c064b00bf70', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (122, N'SERVICE_COMPLETION', N'Service Completion', 1, N'46ed6a96-8bfa-4344-8ce9-37279850f9bf', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (123, N'STANDBY', N'Standby', 1, N'7dadee6e-552a-457f-a17d-4adf4c549f65', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (124, N'START_OF_SERVICE', N'Start Of Service', 1, N'9cf8685d-beea-48d9-b0a8-90b86d68ad12', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (125, N'STOP_OF_SERVICE', N'Stop Of Service', 1, N'3da1a646-4938-47bc-8c50-fdc2faa47489', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (126, N'TOTAL_AND_AVERAGE', N'Total And Average', 1, N'8a1b8e18-215c-44f4-b034-84f15d795ee1', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (128, N'TRANSIENT', N'Transient', 1, N'29ee0332-ad74-4e7f-8770-9a7f7080d602', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (129, N'UNCANCEL', N'Uncancel', 1, N'8e97771f-ce92-4da9-b128-fd0e8f9550b8', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (130, N'REFRESH', N'Refresh', 1, N'5e158b24-13b0-4962-bf45-1494f2e8b989', N'10/23/2012 9:45:47 AM -04:00', N'Administrator', N'10/23/2012 9:45:47 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblCustomToolbarCommandType] ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (131, N'COPY', N'Copy', 1, N'a527ce4e-5870-410a-9534-546c72f8ce25', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblDayOfWeek])=0
BEGIN
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'SUNDAY', N'SUNDAY', N'738cb185-0598-4f44-9c3d-a65fe5de9918', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'MONDAY', N'MONDAY', N'2f79928b-f580-4162-b1b6-0cb48cce923c', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'TUESDAY', N'TUESDAY', N'23fd5810-6266-4b4d-bc69-cf353747f6be', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'WEDNESDAY', N'WEDNESDAY', N'34096f6b-d181-4e37-a6e2-df90556e4e85', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'THURSDAY', N'THURSDAY', N'd1908d13-e4f5-418e-8f66-69d790b638a2', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'FRIDAY', N'FRIDAY', N'a87f2506-cbd4-474e-9af8-57e2ace777b8', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'SATURDAY', N'SATURDAY', N'f2eab33f-fdcc-4903-8a42-eb51655a97ab', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDayOfWeek] ([DayOfWeekIndex], [DayOfWeekCode], [DayOfWeekName], [DayOfWeekGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'HOLIDAY', N'HOLIDAY', N'd6f2de95-56ac-43c1-9664-a18d67aab10f', N'6/18/2012 9:05:55 AM +00:00', N'Administrator', N'6/18/2012 9:05:55 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblDeviceTankType]) = 0
BEGIN
	INSERT INTO [lookup].[tblDeviceTankType] ([DeviceTankTypeIndex], [DeviceTankTypeCode], [DeviceTankTypeName], [DeviceTankTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'OPC', N'OPC', N'6DAD3C64-AFE3-49AE-B4D8-FE28186FC4A6', N'4/7/2016 1:03:42 PM +00:00', N'Administrator', N'4/7/2016 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblDeviceTankType] ([DeviceTankTypeIndex], [DeviceTankTypeCode], [DeviceTankTypeName], [DeviceTankTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'SATELLITE', N'Satellite', N'765DFFA4-706D-4D38-9E91-6E20592D0EF9', N'4/7/2016 1:03:42 PM +00:00', N'Administrator', N'4/7/2016 1:03:42 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblDispatchGridType])=0
BEGIN
	INSERT INTO [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex], [DispatchGridTypeCode], [DispatchGridTypeName], [DispatchGridTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (-1, N'UNKNOWN_GRID_TYPE', N'Unknown Grid Type', N'b56fedd9-bc71-4700-b4f2-8b499b66f6b5', N'9/21/2012 4:08:55 PM -04:00', N'Administrator', N'9/21/2012 4:08:55 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex], [DispatchGridTypeCode], [DispatchGridTypeName], [DispatchGridTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'DISPATCH_TABULAR_VIEW', N'Dispatch Tabular View', N'9f8e4915-0f8b-4c2f-bbf0-3a67c4ecb244', N'9/21/2012 4:08:55 PM -04:00', N'Administrator', N'9/21/2012 4:08:55 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex], [DispatchGridTypeCode], [DispatchGridTypeName], [DispatchGridTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'DISPATCHING_VIEW_ACTIVE_REQUEST_QUEUE', N'Dispatching View - Active Request Queue', N'e184f935-fc54-46a2-ba16-7d4220568f10', N'9/21/2012 4:08:55 PM -04:00', N'Administrator', N'9/21/2012 4:08:55 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex], [DispatchGridTypeCode], [DispatchGridTypeName], [DispatchGridTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'DISPATCHING_VIEW_OPERATOR', N'Dispatching View - Operator', N'9f0ae919-87f3-4dc3-8201-f6f827d928e7', N'9/21/2012 4:08:55 PM -04:00', N'Administrator', N'9/21/2012 4:08:55 PM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex], [DispatchGridTypeCode], [DispatchGridTypeName], [DispatchGridTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'DISPATCHING_VIEW_SERVICING_UNIT', N'Dispatching View - Servicing Unit', N'48190a3d-bea3-4a6a-9459-dbc51519ab48', N'9/21/2012 4:08:55 PM -04:00', N'Administrator', N'9/21/2012 4:08:55 PM -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblDispatchGridColumnType])=0
BEGIN
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (-1, -1, N'c8041132-659c-4367-9de8-cd06b0afd44a', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'UnknownGridColumnType', N'Unknown Grid Column Type', N'', 0, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (1, -1, N'34f79b68-2bbd-4312-bb35-49f710abbff9', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'TransactionAliasUserData', N'Transaction Alias User Data', N'', 0, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (2, -1, N'bc1e5fae-8b9b-4139-93d6-140ae62dbd31', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'TransactionAliasLineItemUserData', N'Transaction Alias Line Item User Data', N'', 0, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (101, 1, N'a1a56fde-4bcb-46cc-9942-9509a864be32', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ControllerLog', N'Controller Log', N'ControlLogUrl', 115, 1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (102, 1, N'1ea57870-6b63-427a-a043-a41c78aacb61', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Status', N'Status', N'Status', 110, 2)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (103, 1, N'daaabd11-594a-47dd-8943-0a4ba4b4805c', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'RequestType', N'Request Type', N'AliasName', 135, 3)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (104, 1, N'ff59431c-0ce0-4178-a5c0-632d4370f6f1', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Dispatched', N'Dispatched', N'DispatchedTime', 95, 4)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (105, 1, N'ed78e16d-f449-405f-b11c-87aa38bca18d', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Operator', N'Operator', N'OperatorID', 95, 5)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (106, 1, N'124433fe-5bf5-474b-8893-95e305d02e50', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'VehicleID', N'Vehicle ID', N'VehicleID', 90, 6)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (107, 1, N'38872e28-db98-4433-8524-4474b46f3afe', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'AircraftID', N'Aircraft ID', N'AircraftID', 90, 7)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (108, 1, N'0c880612-ecd5-4636-bf24-88f779fc6807', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Location', N'Location', N'Location', 85, 8)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (109, 1, N'1412821e-75e0-480b-96fe-252d0f1564f6', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'MDS', N'MDS', N'Model', 75, 9)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (110, 1, N'714707b4-67b6-4ea2-8e5a-3422f736a968', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'NetQuantity', N'Net Quantity', N'NetQuantity', 105, 10)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (111, 1, N'6fa04323-34cd-4822-a673-476815f55934', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Arrival', N'Arrival', N'TimeIn', 90, 11)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (112, 1, N'93845df7-95e5-4425-96b8-75c8ecf71768', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Started', N'Started', N'FST', 90, 12)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (113, 1, N'e4757413-19ce-46b7-a5c2-f9e53b2b35e0', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Stopped', N'Stopped', N'TimeEnd', 90, 13)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (114, 1, N'83b6e9d7-e8e2-49d5-94e0-5447358bd5b5', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Departed', N'Departed', N'TimeOut', 90, 14)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (115, 1, N'90334291-b0dc-4ee6-97c2-0770175849e4', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ProductID', N'Product ID', N'Grade', 90, 15)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (116, 1, N'b6e29999-41df-4138-a934-ddc5a6644015', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ResponseTime', N'Response Time', N'ResponseTime', 120, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (117, 1, N'cd8bb08a-a900-48c2-9d0e-7d5d58b20668', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'GrossQuantity', N'Gross Quantity', N'GrossQuantity', 120, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (118, 1, N'675ff4e3-17f4-48a0-b761-7cb83944111b', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'TransactionDate', N'Transaction Date', N'TransactionDate', 155, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (119, 1, N'055061b4-c084-45bd-acc4-e00b0f91e15a', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Requested', N'Requested', N'RequestedTime', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (120, 1, N'5fe14952-1ce7-48c8-8018-aca82fc208b0', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Radio', N'Radio', N'Radio', 80, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (121, 1, N'ca967c9e-cece-48e4-8392-e262f6c14beb', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FuelTime', N'Fuel Time', N'FuelTime', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (122, 1, N'45327e04-a3d7-47ee-86b7-004bdb8fd0e3', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Variance', N'Variance', N'Variance', 80, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (123, 1, N'9cec3e13-4064-449b-b3a0-82727e6724ea', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ShipToID', N'ShipTo ID', N'ShipToID', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (124, 1, N'db9b8d87-254a-4b62-ad2e-cd1cfdb44c89', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'BillToID', N'BillTo ID', N'BillToID', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (125, 1, N'ee0d41e6-7928-4df4-b090-08c5a74120d2', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'CardNumber', N'Card Number', N'CardNumber', 110, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (126, 1, N'5e85a271-2337-4273-9e0f-d3c6fadd0f45', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FuelCardID', N'FuelCard ID', N'FuelCardID', 100, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (127, 1, N'92fa074c-7362-428a-a664-8fed39c4fff7', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'XREF', N'Reg ID', N'XREF', 80, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (201, 2, N'c9d42ede-f057-447a-b753-54a9e5e5d97d', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Status', N'Status', N'Status', 110, 1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (202, 2, N'55cd6939-1b19-4a98-b49b-3c1ca65aa475', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'RequestType', N'Request Type', N'AliasName', 135, 2)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (203, 2, N'ca957ca1-71fe-4a30-9783-6b750e244ff7', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Dispatched', N'Dispatched', N'DispatchedTime', 95, 3)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (204, 2, N'4543c962-1ad5-412b-95b3-b01ba44fb753', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Operator', N'Operator', N'OperatorID', 95, 4)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (205, 2, N'280900c3-4e2c-4c76-822c-608da8cfd323', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'VehicleID', N'Vehicle ID', N'SourceRegistrationID', 90, 5)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (206, 2, N'2716aa61-b094-4e97-b765-2b52f42a606d', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'AircraftID', N'Aircraft ID', N'DestinationRegistrationID1', 90, 6)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (207, 2, N'22c0ab56-6614-48df-998d-74a45f6640c3', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Location', N'Location', N'Location', 85, 7)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (208, 2, N'15416344-543a-4dcf-b985-abe5116a4f5d', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'MDS', N'MDS', N'Model', 75, 8)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (209, 2, N'da001302-0e99-42e8-9d1e-530ad9d4b5d9', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'NetQuantity', N'Net Quantity', N'NetQuantity', 105, 9)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (210, 2, N'19d6c117-728b-4f16-b542-f64e8bb49203', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Arrival', N'Arrival', N'TimeIn', 90, 10)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (211, 2, N'f05a5b4c-3908-45df-9012-58b3d7983251', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Started', N'Started', N'FST', 90, 11)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (212, 2, N'9200349d-8060-49e1-9b0d-54cedb65678d', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Stopped', N'Stopped', N'TimeEnd', 90, 12)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (213, 2, N'e89697b3-3e8e-4ccb-87d2-e4b40d308f6b', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Departed', N'Departed', N'TimeOut', 90, 13)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (214, 2, N'a74cfdf8-6516-445d-b583-8a92fc90a2f5', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ProductID', N'Product ID', N'Grade', 90, 14)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (215, 2, N'5e253ab1-a925-457d-9227-d2eff8c417e4', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ResponseTime', N'Response Time', N'ResponseTime', 120, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (216, 2, N'babf2eb5-6ca2-4146-a317-d5e3bf81acc8', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'GrossQuantity', N'Gross Quantity', N'GrossQuantity', 120, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (217, 2, N'e544be3a-a1cc-4917-b570-58cdf81f65d2', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'TransactionDate', N'Transaction Date', N'TransactionDate', 155, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (218, 2, N'1fb2ad63-a2e8-4f26-a6d2-ad331c93f5db', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Requested', N'Requested', N'RequestedTime', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (219, 2, N'ec10a701-8baf-4e41-9f9e-cf83348b6c00', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Radio', N'Radio', N'Radio', 80, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (220, 2, N'3e7a46d4-1f33-48d6-9ad5-8407cb86c071', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FuelTime', N'Fuel Time', N'FuelTime', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (221, 2, N'65daffb9-a22c-4c4e-b239-22e42bb21800', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Variance', N'Variance', N'Variance', 80, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (222, 2, N'bc8fab48-0470-41ae-a76e-23bca85a914c', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'ShipToID', N'ShipTo ID', N'ShipToID', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (223, 2, N'9b46b41f-4c6a-4aea-a96d-4fe92275aa28', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'BillToID', N'BillTo ID', N'BillToID', 90, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (224, 2, N'94b5afed-dbf2-43bf-a227-b385b3123774', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'CardNumber', N'Card Number', N'CardNumber', 110, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (225, 2, N'dbaacfc7-2f95-4a4b-bc29-bb42bea595a4', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FuelCardID', N'FuelCard ID', N'FuelCardID', 100, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (226, 2, N'14586a93-2975-4822-b2bf-ab4e7501cd07', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'XREF', N'Reg ID', N'XREF', 80, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (301, 3, N'a14671d8-fa3e-43e3-96ac-962f24764c5c', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Last', N'Last', N'Last', 90, 1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (302, 3, N'aee38dff-3702-4168-ab72-39b108585c72', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'First', N'First', N'First', 90, 2)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (303, 3, N'3ca39d36-9535-47f7-991e-579421db6820', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Status', N'Status', N'Status', 75, 3)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (304, 3, N'96b0ab5c-5ffd-490f-8788-60577b6c9173', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Equipment', N'Equipment', N'Equipment', 200, 4)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (305, 3, N'5caafee7-3261-42a0-8ac4-ef1c74356ed8', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FullName', N'Full Name', N'FullName', 200, -1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (401, 4, N'0b377b0c-3a31-47c8-8d1a-c32e2f8b4442', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'RegID', N'Reg ID', N'RegID', 80, 1)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (402, 4, N'ce86ea34-31b8-4e61-b7b2-5a5199ab80dd', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Vehicle', N'Vehicle', N'Vehicle', 95, 2)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (403, 4, N'6dc040c9-6b31-4f65-bf2c-0f340f5a2fa6', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Type', N'Type', N'Type', 100, 3)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (404, 4, N'a608735e-54f7-4a26-a630-671fd6e712ab', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Grade', N'Grade', N'Grade', 100, 4)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (405, 4, N'22f246b4-4a82-45de-a84a-309b09e0f6af', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'Volume', N'Volume', N'Volume', 80, 5)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (406, 4, N'9261f871-0f52-4d9a-a7da-bea36bfb498d', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FuelAdditiveFlag', N'Fuel Additive Flag', N'FuelAdditiveFlag', 100, 6)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (407, 4, N'295f4f6a-a44f-4f09-a599-7aaeda447a2a', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'InService', N'In Service', N'InService', 85, 7)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (408, 4, N'5db727f1-492b-4272-b0c2-af80afbc738a', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'FuelingState', N'Fueling State', N'FuelingState', 105, 8)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (409, 4, N'2bcfd5b4-938b-4e7a-be30-96817974ffdf', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'IssPt', N'Iss Pt', N'IssPt', 70, 9)
	INSERT INTO [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex], [LookupDispatchGridTypeIndex], [DispatchGridColumnTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ID], [DisplayName], [DataField], [Width], [DefaultColumnOrder]) VALUES (410, 4, N'6f00bcb0-3e2c-4d15-81da-fd78e70db9f7', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/2/2013 10:16:00 AM -04:00', N'LEIDOS-CORP\dossantosa', N'IssPtNum', N'Iss Pt Num', N'IssPtNum', 95, 10)
END

IF (SELECT COUNT(*) FROM [lookup].[tblEngineeringUnit])=0
BEGIN
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'FMT_DegC', N'Degrees Celcius', N'°C', N'9daa8925-906b-498e-b034-5f278f95a9c8', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'FMT_DegF', N'Degrees Farenheit', N'°F', N'4266c947-21e6-4410-97ce-7119fd236dda', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'FMT_DegK', N'Degrees Kelvin', N'Kelvin', N'e75b4e10-8768-49ef-9dc1-694121eaea9d', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'FMT_DegR', N'Degrees Rankine', N'°R', N'f29b9445-cf1a-414b-a648-8f5d799afcdd', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'FMT_Msec', N'MilliSeconds', N'msec', N'50ac35c2-c2eb-454f-9a2b-c0d37184b01e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'FMT_Sec', N'Seconds', N'sec', N'8a4d308d-0344-40c5-8d1b-40afd6d7d2cc', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'FMT_Min', N'Minutes', N'min', N'8fd4a432-039f-4eac-877f-5d1ad62cd5ab', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'FMT_Hour', N'Hours', N'hr', N'6f88288f-a595-413b-ad8a-3399c3b5058b', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'FMT_Day', N'Days', N'days', N'fa76d4e1-1899-4438-9702-3d2e04bd3af4', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'FMT_Week', N'Weeks', N'wks', N'96504f8d-dbf0-44c7-9f73-1c6773a97e6b', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'FMT_Month', N'Months', N'mon', N'04eafd54-208c-408c-baff-be285da985b5', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'FMT_Year', N'Years', N'yrs', N'7770c866-228f-4af8-ad05-7f950a4a5ce2', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'FML_FtIn8th', N'ft/inch/8th', N'ft-in-8th', N'1f44486a-58e6-48d4-838b-57d5e0f9ee80', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'FML_MM', N'Millimeters', N'mm', N'69875112-e9d0-4753-a499-d2bc08ef7947', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'FML_CM', N'Centimeters', N'cm', N'ef685984-5ecb-4dc6-9613-96ed739ea358', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'FML_Meter', N'Meters', N'm', N'77c12242-f93a-485d-9034-3526a7901877', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'FML_KM', N'Kilometers', N'km', N'53b19311-603d-4bba-9b03-d7373595a41c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (24, N'FML_16th', N'16th of Inch', N'16th', N'b33024a9-210f-48e0-acb0-cbf6c9fdfdbf', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (25, N'FML_Inch', N'Inches', N'in', N'6bd179f7-3d15-40cd-8f16-423b2940432f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'FML_Feet', N'Feet', N'ft', N'f97d8fcf-4c76-4a80-af29-3ecd04fab4af', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (27, N'FML_FtIn16th', N'ft/inch/16th', N'ft-in-16th', N'fd387c21-317d-4116-b937-101dcdd18ffd', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (28, N'FML_Yard', N'Yards', N'yd', N'a301867d-db4e-4b76-a770-09c93ee900f1', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (29, N'FML_Mile', N'Miles', N'mi', N'0489d60e-9fd4-4b81-88e5-206ff7a4c37c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (30, N'FMA_MM2', N'square millimeters', N'mm²', N'6eec15aa-95a3-499f-80e2-a8debe1bccae', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (31, N'FMA_CM2', N'square centimeters', N'cm²', N'5090f76b-a98a-4839-8297-ee020fd9f553', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (32, N'FMA_Meter2', N'square meters', N'm²', N'5604ea40-e3d3-484f-8a1c-49f5d0d7edb3', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (33, N'FMA_KM2', N'square kilometers', N'km²', N'2f830f8d-7b66-4ab2-af6f-4451d95e0dc5', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (34, N'FMA_16TH2', N'square 16ths inch', N'16th²', N'f1267f5c-96d7-4906-af06-f5d8752f31a9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (35, N'FMA_Inch2', N'square inches', N'in²', N'62a7d4b0-1112-4767-be13-f6c70ed91f81', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (36, N'FMA_Feet2', N'square feet', N'ft²', N'b5b28c2f-c2dd-40af-8750-62faeeb8d04f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (37, N'FMA_Yard2', N'square yards', N'yd²', N'996cdc27-0b99-42d0-9369-1d87cb7368f6', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (38, N'FMA_Mile2', N'square miles', N'mi²', N'5d38c0dc-0651-41df-b4c9-c7f13539e213', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (40, N'FMV_CM3', N'Cubic centimeters', N'cc', N'97007bcf-af4c-4a9a-abc5-6f307905b735', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (41, N'FMV_Meter3', N'Cubic meters', N'm³', N'f692a04d-680b-412a-8731-ea801b85630c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (42, N'FMV_Litre', N'Liters', N'l', N'5800184a-5ee6-4498-9cf6-a98e4e96ea0f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (43, N'FMV_Inch3', N'Cubic inches', N'in³', N'd32fab5f-5a66-430b-be7d-fac3cd81fecd', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (44, N'FMV_Feet3', N'Cubic feet', N'ft³', N'52d31550-e779-48e8-ad0f-1b28fd571098', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (45, N'FMV_Yard3', N'Cubic yards', N'yd³', N'8929c567-09f2-41e9-b4d7-9d163f4a8278', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (46, N'FMV_USGal', N'U.S. Gallons', N'gal (US)', N'4a720951-869b-4850-997b-b0d5b787f276', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (47, N'FMV_ImpGal', N'Imperial Gallons', N'gal (UK)', N'c5c0abac-8c02-470a-bd00-b804e5cac1b9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (48, N'FMV_BlOil', N'Barrels (Oil)', N'bbl (Oil)', N'1a92c2b0-a47e-4c35-8067-ba04157d84ca', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (49, N'FMV_BlLiq', N'Barrels Liquid', N'bbl (Liq)', N'ee54a656-13b1-4153-b536-dc4acdb5af19', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (50, N'FMV_KL', N'Kiloliters', N'kl', N'62b00a85-e9bf-451c-8f1f-2ef62b887b1a', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (51, N'FMV_MsFt3', N'1000 standard cubic feet', N'MsFt3', N'd07fa7b8-a517-4149-806f-3e189d8a3b5e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (60, N'FMM_Gram', N'Grams', N'g', N'499e41f7-bbc2-44ef-9c3f-792fd56bf176', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (61, N'FMM_KG', N'Kilograms', N'kg', N'065906e1-f471-4b1d-88a4-53160d760334', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (62, N'FMM_MTon', N'Metric Tons', N'ton (m)', N'141ad595-c123-4ce0-b29b-bfc5af922062', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (63, N'FMM_Oz', N'Ounces', N'oz', N'633cc1f4-de63-4808-a0d0-a88e623354bc', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (64, N'FMM_Lb', N'Pounds', N'lb', N'532b5376-9542-487b-a535-2910c8f1e950', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (65, N'FMM_ETon', N'English Tons', N'ton (e)', N'c1e8db86-db36-43d0-81cc-454c6a01d2e5', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (66, N'FMM_STon', N'Short Tons', N'ton (s)', N'6a007d79-1864-4860-9408-0c4d738f9490', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (67, N'FMM_LTon', N'Long Tons', N'ton (l)', N'7ae9c04a-e4de-459a-a017-43d68a04c83f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (68, N'FMM_Mlbs', N'Pounds (Thousands)', N'Mlbs', N'37fc8105-c545-4a0e-a7b1-b0d022d90128', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (70, N'FMP_Pa', N'Pascals', N'Pa', N'de53b0bb-a9b6-44a6-9a46-2c78481b5ece', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (71, N'FMP_KPa', N'KiloPascals', N'kPa', N'107c2152-3fe0-4585-b4cb-ca327b53eb29', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (72, N'FMP_KgCm2', N'kilograms/sq cm', N'kg/cm²', N'9939f0b9-632a-4d33-ad4d-c505c887a696', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (73, N'FMP_Psi', N'pounds/sq in (PSI)', N'PSI', N'36420b21-b711-4f5e-a1f3-32db472d9c3f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (74, N'FMP_PsiG', N'PSI Gauge', N'psig', N'9d8dc67b-aa2a-4a8d-9686-298a9b0fa2ea', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (75, N'FMP_PsiA', N'PSI Absolute', N'psia', N'2c558a79-ea1e-433c-8eea-4d1bcda8cfa0', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (76, N'FMP_InH2O', N'inches H2O @ 68F', N'in H2O', N'7256a986-673e-4063-aade-0a2933f0de20', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (77, N'FMP_FtH2O', N'feet H2O @ 68F', N'ft H2O', N'a0db9926-47c6-414d-9071-af717f72fb60', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (78, N'FMP_InHg', N'Inches Mercury @ 0C', N'in Hg', N'76ca87d7-0624-457b-a827-5a980bb77ad7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (79, N'FMP_LbFt2', N'pounds per sq. ft', N'lb/ft²', N'65a2294d-75d8-460a-8343-8c4b3a290d04', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (80, N'FMP_Torr', N'Torr @ 0C', N'torr', N'042fee0e-432b-4f67-a8ac-5daca539e29a', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (81, N'FMP_Bar', N'Bar', N'bar', N'96d4c6a5-fd5b-427e-b0ed-c671807020b2', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (82, N'FMP_MBar', N'Millibar', N'mbar', N'6a2876a4-20b9-4175-b72e-3f77717f14be', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (83, N'FMP_MMHg', N'mm Mercury @ 0C', N'mm Hg', N'f9a3aec2-d635-4ec1-83cb-46fce28354d7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (84, N'FMP_MMH2O', N'mm H2O @ 68F', N'mm H2O', N'187d6a7f-4c87-4f61-b13c-f515d469acab', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (85, N'FMP_GmCm2', N'grams/sq cm', N'g/cm²', N'9cc46380-d9ab-45e1-bd79-37a06459c722', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (86, N'FMP_Atm', N'Atmospheres', N'atm', N'edbb26f6-f7fe-4911-af3a-01c66c0cf770', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (90, N'FMVF_CCMin', N'cc per min', N'cc/min', N'3441388f-2617-4ec4-973a-f73c7bc50046', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (91, N'FMVF_CCHr', N'cc per hour', N'cc/hr', N'ca3dbfad-d89f-406b-9532-0b0eb0826399', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (92, N'FMVF_M3Sec', N'cu. meters per sec', N'm³/sec', N'7c1cfc23-7fd3-4ae2-850a-ecfa09451ec7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (93, N'FMVF_M3Min', N'cu. meters per Minute', N'm³/min', N'b9400216-b6a7-4036-925f-60b83e3e5836', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (94, N'FMVF_M3Hr', N'cu. meters per Hour', N'm³/hr', N'0c2a9a6e-c8fc-481a-a863-41f6a3223950', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (95, N'FMVF_M3Day', N'cu. meters per Day', N'm³/day', N'3a6512d4-6b25-40c1-a745-a5f45b6a735c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (96, N'FMVF_LtSec', N'Liters per sec', N'l/sec', N'8539525f-075c-454c-a8e3-8b13c1d3cf11', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (97, N'FMVF_LtMin', N'Liters per minute', N'l/min', N'd7c59d69-f85e-4cc4-a5b8-7155b3913527', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (98, N'FMVF_LtHr', N'Liters per Hour', N'l/hr', N'859ea75b-27f5-4ff6-a04a-336088c41786', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (99, N'FMVF_MLPD', N'Million liters per day', N'Ml/day', N'3cac3810-7260-4fd4-982a-f8dd0b7d2de3', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (100, N'FMVF_In3Min', N'cu. inches / min', N'in³/min', N'a963e26d-551c-4839-8896-8bc40125e628', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (101, N'FMVF_In3Hr', N'cu. inches / hour', N'in³/hr', N'f1850d23-a114-4cd6-bcaa-9d8510582ec9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (102, N'FMVF_Ft3Sec', N'cu. feet / sec', N'ft³/sec', N'a3ba1cc5-b25c-4f5e-a5b1-ccce424fe84d', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (103, N'FMVF_Ft3Min', N'cu. feet / min', N'ft³/min', N'a20537f8-386e-4c15-af25-e596a905c8af', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (104, N'FMVF_Ft3Hr', N'cu. feet / hour', N'ft³/hr', N'e834374c-91b5-486c-92b0-b8bd194fed40', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (105, N'FMVF_Ft3Day', N'cu. feet / day', N'ft³/day', N'90bf62b3-c8b4-4cc2-8df3-ff66465d5a97', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (106, N'FMVF_Yd3Min', N'cu. yards / min', N'yd³/min', N'84a7230e-d33f-46eb-b90f-0f0c60e97d63', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (107, N'FMVF_Yd3Hr', N'cu. yards / hour', N'yd³/hr', N'2fa93c0a-debb-44cd-8472-790d8e5c45ce', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (108, N'FMVF_GPS', N'U.S. Gallons / sec', N'gps (US)', N'cc126bc8-0d59-442c-bfbd-3f7bc9288254', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (109, N'FMVF_GPM', N'U.S. Gallons / min', N'gpm (US)', N'632932aa-29b1-44bd-a480-d7617e0c83d6', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (110, N'FMVF_GPH', N'U.S. Gallons / hour', N'gph (US)', N'5889678c-937a-4070-be2a-a58c4b34e1fe', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (111, N'FMVF_MGPD', N'Mill. U.S. Gallons / day', N'MGPD (US)', N'1ca179be-f346-476c-a26e-12715c0fe6b0', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (112, N'FMVF_ImpGPS', N'U.K. Gallons / sec', N'gps (UK)', N'6baf97b7-3aac-4183-9754-c6d9977a95f9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (113, N'FMVF_ImpGPM', N'U.K. Gallons / min', N'gpm (UK)', N'e2d95da6-e0ac-47eb-ae38-b6f07bb50927', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (114, N'FMVF_ImpGPH', N'U.K. Gallons / hour', N'gph (UK)', N'98a93f39-00eb-4b00-ba20-fc8c4e379791', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (115, N'FMVF_ImpMGD', N'Mill. U.K. Gallons / day', N'MGPD (UK)', N'85d4dd49-e36c-4b20-9ec0-1fdf82567e48', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (116, N'FMVF_BPMoil', N'bbl per min (oil)', N'BPM (Oil)', N'85533ec7-281f-44cb-b1bf-ee38420f9d13', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (117, N'FMVF_BPHoil', N'bbl per hour (oil)', N'BPH (Oil)', N'2b4717f3-c5f3-4214-9c7b-73f06d71e6ff', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (118, N'FMVF_BPDoil', N'bbl per day (oil)', N'BPD (Oil)', N'19834122-18d6-4e91-b323-88bbbf1d8548', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (119, N'FMVF_MBDoil', N'Mbbl / day (oil)', N'MBPD (Oil)', N'54e1323c-5898-4414-88b8-b934286e07db', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (120, N'FMVF_BPMliq', N'bbl per min (liq)', N'BPM (Liq)', N'6382674a-3c79-4a4f-8af4-4fe06fa544fc', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (121, N'FMVF_BPHliq', N'bbl per hour (liq)', N'BPH (Liq)', N'e8301145-db43-48f5-8c0c-4b0bdad95b3c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (122, N'FMVF_BPDliq', N'bbl per day (liq)', N'BPD (Liq)', N'390e402b-46be-4c2a-b04b-fc7abd991f02', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (123, N'FMVF_MBDliq', N'Mbbl / day (liq)', N'MBPD (Liq)', N'cf602fe2-0dad-4d53-81a1-13dd1d748510', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (124, N'FMVF_KLSec', N'kiloliters / sec', N'kl/sec', N'777310df-b99a-4378-b1c1-551f5c6b064e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (125, N'FMVF_KLMin', N'kiloliters / min', N'kl/min', N'0c6db6b9-9602-4b02-b4fc-75a991dcebe3', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (126, N'FMVF_KLHr', N'kiloliters / hr', N'kl/hr', N'699b6701-d5e7-421a-b517-fc73255c2d0d', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (127, N'FMVF_KLDay', N'kiloliters / day', N'kl/day', N'd46d10c6-6c5b-453e-9063-b6cbcd76e443', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (130, N'FMMF_LbSec', N'Pounds per sec', N'lb/sec', N'baa61037-8911-43cd-9d85-a3daac8feb08', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (131, N'FMMF_LbMin', N'Pounds per min', N'lb/min', N'5f6ac1fb-e092-4a70-902f-f04c42e1dcce', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (132, N'FMMF_LbHr', N'Pounds per hour', N'lb/hr', N'f7e54ad8-3beb-4716-80da-9491b83e1dbe', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (133, N'FMMF_LbDay', N'Pounds per day', N'lb/day', N'3aaf7d9b-0491-47e4-b584-8e9a4618a3c5', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (134, N'FMMF_MTonMn', N'Metric tons per min', N'ton(m)/min', N'99ecd388-0241-4036-9cfe-9a668e2cb66c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (135, N'FMMF_MTonHr', N'Metric tons per hour', N'ton(m)/hr', N'37024734-5ac5-469e-92eb-ec6ce9b85a4c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (136, N'FMMF_MTonDy', N'Metric tons per day', N'ton(m)/day', N'17d278cf-6e8f-44eb-94be-c374d2d73439', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (137, N'FMMF_STonMn', N'Short tons per min', N'ton(s)/min', N'c7bb7c8f-1421-4bb0-8511-95fb519f4346', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (138, N'FMMF_STonHr', N'Short tons per hour', N'ton(s)/hr', N'f2db6ec4-60b2-40a9-af2c-13fc2b2d95a7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (139, N'FMMF_STonDy', N'Short tons per day', N'ton(s)/day', N'b8982a4e-412b-4746-ac6c-4597534fa1c9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (140, N'FMMF_LTonMn', N'Long tons per min', N'ton(l)/min', N'82881be0-a843-4ffd-8f14-4edd6cef78b7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (141, N'FMMF_LTonHr', N'Long tons per hour', N'ton(l)/hr', N'093d6d57-514c-4dbb-8e6d-d24573343618', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (142, N'FMMF_LTonDy', N'Long tons per day', N'ton(l)/day', N'a2336f2c-556b-419b-abd8-c9c1a14cf04b', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (143, N'FMMF_GmSec', N'Grams per sec', N'g/sec', N'540bb43d-198b-4f0b-9809-5f592d221c09', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (144, N'FMMF_GmMin', N'Grams per min', N'g/min', N'01f1855e-e997-457d-af37-e0ca3d303a17', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (145, N'FMMF_GmHr', N'Grams per hour', N'g/hr', N'7dbf04ad-7dcd-489e-bb39-147270f864a6', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (146, N'FMMF_KgSec', N'Kilograms per sec', N'kg/sec', N'da169842-8fd8-4bc3-9758-6055aad60ca2', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (147, N'FMMF_KgMin', N'Kilograms per min', N'kg/min', N'a99bba68-6d7f-49d3-b073-09780d73c812', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (148, N'FMMF_KgHr', N'Kilograms per hr', N'kg/hr', N'1d6d95dc-1faf-40df-bdae-72cd76dc2310', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (149, N'FMMF_KgDay', N'Kilograms per day', N'kg/day', N'e4b7ddec-f29b-4298-a2de-d77076ec6e39', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (150, N'FMMF_MlbSec', N'Million Pounds per sec', N'Mlbs/sec', N'30c24f68-7076-4156-93c0-efc088087e11', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (151, N'FMMF_MlbMin', N'Million Pounds per min', N'Mlbs/min', N'5b45172d-cb93-454e-8fb5-b4fae3ebbb10', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (152, N'FMMF_MlbHr', N'Million Pounds per hour', N'Mlbs/hr', N'332184b4-5d14-436e-8f55-7b7ea1eae44f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (153, N'FMMF_MlbDay', N'Million Pounds per day', N'Mlbs/day', N'94c00659-1a83-4628-a358-92cbd429b647', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (160, N'FMVR_IPS', N'Inches per sec', N'in/sec', N'684121f3-d460-49d2-ae23-4881abd73194', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (161, N'FMVR_FPS', N'Feet per sec', N'ft/sec', N'ceb48302-3e2f-4e51-a60c-b2b6134355e1', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (162, N'FMVR_FPM', N'Feet per min', N'ft/min', N'a4996843-bc7f-4f64-96a3-0cb3aa151d47', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (163, N'FMVR_MMSec', N'Millimeters per sec', N'mm/sec', N'f0bb65a3-9653-4561-9fd7-88cfee4e2536', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (164, N'FMVR_CMSec', N'Centimeters per sec', N'cm/sec', N'5ac9fd3f-103c-4b47-a574-8876a0da47a7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (165, N'FMVR_MSec', N'Meters per sec', N'm/sec', N'25c8a21c-9f20-437a-9cbb-26092afcfe9c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (166, N'FMVR_MMin', N'Meters per min', N'm/min', N'f97c9851-0610-4f36-b089-14cc14832536', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (167, N'FMVR_MPH', N'Miles per hour', N'MPH', N'21a48fa3-0120-4a6a-8557-8eb003386da9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (168, N'FMVR_MrPH', N'Meters per hour', N'm/hr', N'b98c01b2-a088-4c1a-ae6d-35f78381787c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (169, N'FMVR_KMPH', N'Kilometers per hour', N'KPH', N'b02cf89c-a295-4a60-9b05-4325f9a27256', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (170, N'FMVR_KNOT', N'Knots', N'KNOT', N'34a7e6ba-3752-4054-952c-4ad91196d52c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (171, N'FMVR_MMMin', N'Millimeters / min', N'mm/min', N'bca17124-fd8c-4566-89fd-574635560718', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (180, N'FMD_GCM3', N'Grams / cu. cm.', N'g/cm³', N'f6e14c79-9886-408c-a3ca-62a42ff6b22f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (181, N'FMD_GMl3', N'Grams / milliliter', N'g/ml', N'46fe3831-bcc8-4f2e-87fe-20c11a9f7b10', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (182, N'FMD_GL3', N'Grams / liter', N'g/l', N'0b8f0811-83af-4a83-bfe0-74bc1cab4e62', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (183, N'FMD_KgM3', N'Kilograms / cu. meter', N'kg/m³', N'0d75bf15-8ba1-4808-abdf-299f86cdf093', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (184, N'FMD_KgL3', N'Kilograms / liter', N'kg/l', N'53acd31d-a2a7-43fd-a5a4-e50bf842a002', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (185, N'FMD_LbIn3', N'Pounds / cu. inch', N'lb/in³', N'd6b1ace2-7211-4b76-80c9-0e8fbd1bf438', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (186, N'FMD_LbFt3', N'Pounds / cu. foot', N'lb/ft³', N'e9e9a42c-c0b8-446b-844b-9c537370127e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (187, N'FMD_USLbGal', N'Pounds / gallon (U.S.)', N'lb/gal(US)', N'0590d8de-09b2-4568-8cac-6cfb75af3379', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (188, N'FMD_ImpLbGl', N'Pounds / gallon (U.K.)', N'lb/gal(UK)', N'04e96286-696e-4994-9982-843711d16dfc', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (189, N'FMD_LbBlOil', N'Pounds / barrel (oil)', N'lb/bbl(o)', N'7d6ca752-f5e6-4417-81dd-d6b2ab927bf7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (190, N'FMD_LbBlLiq', N'Pounds / barrel (liq)', N'lb/bbl(l)', N'7d60fbf9-809a-4a46-b7f9-8f998eabceff', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (191, N'FMD_DegAPI', N'Degrees API', N'°API', N'df7f5841-043c-4ea6-81dc-c9c3e2500c30', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (192, N'FMD_SpGrav', N'Specific gravity', N'sp gr', N'80b40559-c9ad-465f-bcc1-3e6d2783775e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (193, N'FMD_PrPlato', N'Percent Plato', N'% Plato', N'1ce55ab9-0e27-4af4-8b87-f4a193dd6a9f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (194, N'FMD_DegBRIX', N'Degrees BRIX', N'°BRIX', N'21c67200-1771-423e-b5db-5fadea02223e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (195, N'FMD_DegBmLt', N'Degrees Baume (light)', N'°Ba (l)', N'999b9e33-1664-4dd5-a34d-5f8dbf53aa9a', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (196, N'FMD_DegBmHy', N'Degrees Baume (heavy)', N'°Ba (h)', N'67735c39-ed35-4114-817a-100f29f04f11', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (197, N'FMD_DegTwad', N'Degrees Twaddell', N'°Tw', N'065e15a3-55aa-437b-b3f4-3302b390c811', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (198, N'FMD_DegBal', N'Degrees Balling', N'°Balling', N'b8e9ba60-b1cc-4467-b48b-e332195a5794', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (199, N'FMD_STnYd3', N'Short tons / cubic yard', N'ton(s)/yd³', N'6cdfc2b0-3eed-4aa8-8abc-7c6f2f03f628', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (200, N'FME_BTU', N'British Thermal Units', N'BTU', N'f1fa2daa-af8b-4ca0-bf8d-a3eab6dad8d6', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (201, N'FME_Cal', N'Calories', N'cal', N'd5c16f2b-5767-4390-b8b2-e94f9a971a1c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (202, N'FME_Joule', N'Joules', N'J', N'6ba34116-b37c-44e9-bb7a-d4bbc116c920', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (203, N'FME_WH', N'Watt-hours', N'WH', N'aa75ba6d-7943-4b6e-a04e-3b15eebce878', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (204, N'FME_KwH', N'Kilowatt-hours', N'kWH', N'bca84a82-8a02-457e-aa46-8b7e6e3fbf84', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (210, N'FMPH_BTUSec', N'BTU / sec', N'BTU/sec', N'f6a5b0bf-b353-43ba-b240-a6076197c49a', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (211, N'FMPH_BTUMin', N'BTU / min', N'BTU/min', N'1a7c9fcd-3331-47a0-90d5-81c595372101', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (212, N'FMPH_BTUHr', N'BTU / hour', N'BTU/hr', N'051c75ed-0ea3-4fe9-bd06-c0f8327d5dd9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (213, N'FMPH_CalMin', N'Cal / min', N'cal/min', N'120ccb85-c726-49bc-9f0c-a3ae1c2c73fa', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (214, N'FMPH_Watt', N'Watts', N'W', N'c271f91d-100c-47a3-83a0-3c2e6730c012', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (215, N'FMPH_KWatts', N'KiloWatts', N'kW', N'85ee5335-23c8-4efa-8d77-90bbc66ac22f', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (216, N'FMPH_KVAmp', N'Kilo Volt-Amperes', N'kVA', N'181b11a9-371b-4b7d-9e95-701368a3ea47', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (217, N'FMPH_HPower', N'Horsepower', N'hp', N'cfa0b823-d309-402e-af0f-96624d7ffb3e', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (220, N'FMEU_MVolts', N'Millivolts', N'mV', N'23a88c9b-4436-40f6-8982-1befd6a5d7fe', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (221, N'FMEU_Volt', N'Volts', N'V', N'21bc3f0e-2b0f-4466-bd80-db00080dd96b', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (222, N'FMEU_MAmps', N'Milliamperes', N'mA', N'c50a91a0-a991-4967-9f66-7e55be913234', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (223, N'FMEU_Amp', N'Amperes', N'A', N'53643e25-bdd8-4b91-a75b-05c9038910eb', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (224, N'FMEU_Ohm', N'Ohms', N'ohm', N'4fd9608f-1c70-4240-a94f-e3bf86db24ef', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (225, N'FMEU_Farad', N'Farads', N'F', N'11db1ce8-c9e2-4011-96b4-457f8d4d5db7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (226, N'FMEU_Coul', N'Coulombs', N'C', N'7fc7be37-75cc-4e35-a94e-7b951524e5e7', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (227, N'FMEU_Henry', N'Henrys', N'H', N'c3c661d2-de15-435d-8c76-e3bfb1ba8d65', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (228, N'FMEU_MicSie', N'MicroSiemens', N'µS', N'bca021e2-b052-43cb-b4d1-50eba5d7f40c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (229, N'FMEU_Siemen', N'Siemens', N'S', N'0c14cd5a-ebc8-4f00-85a5-c6fab5953de9', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (230, N'FMEU_MHO', N'MHOs', N'mho', N'dc3e3ea4-b447-4565-877e-a9985ff5d8f2', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (231, N'FMDU_PwrFct', N'Power factor', N'P.F.', N'3baea276-1258-48db-90de-f3c67452b4cd', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (232, N'FMDU_RPM', N'Revolutions / min', N'RPM', N'6bd46253-bcbf-45c0-aaf3-b059618d2f06', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (233, N'FMDU_Hertz', N'Cycles / sec (Hz)', N'Hz', N'fa100f15-c482-4d5b-b1a4-bb72bd714639', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (234, N'FMDU_PCent', N'Percent', N'%', N'9ee0d2be-8c1f-47f0-8355-5696d19b607a', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (235, N'FMDU_PPM', N'Parts per million', N'PPM', N'62e5e745-02a1-4ae3-8f00-5046ee79094d', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (236, N'FMDU_PHumid', N'% Humidity', N'%H', N'ac756964-f888-4073-9a4b-145a80a10857', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (237, N'FMDU_POxygn', N'% Oxygen', N'%O2', N'a1b0856e-4172-4726-b215-e9847938ce18', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (238, N'FMDU_RHumid', N'Relative Humidity', N'RH', N'627cb0cf-1b5d-48e5-9d6f-d22ac3a69258', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (239, N'FMDU_PH', N'pH', N'pH', N'182ec65f-9f99-4d54-8c6d-858ed13c48f3', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (240, N'FMMU_Centp', N'Centipoise', N'centp', N'f4696545-15e9-43a1-b54e-c4cb323ef98c', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (241, N'FMMU_SolWt', N'% Solids by weight', N'%sol-wt', N'94ea7af1-c145-4a48-b1aa-4d076ad0b4df', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (242, N'FMMU_SolVol', N'% Solids by volume', N'%sol-vol', N'5628e662-4ec4-4ad0-a359-f0fe7dd9b63b', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (243, N'FMMU_StQual', N'% Steam quality', N'%quality', N'c7bf90e3-16c5-45b1-b00a-af7969ed9404', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (244, N'FMMU_Bushel', N'Bushels', N'bushel', N'20551b2e-2925-4f60-b149-fa695aa044a8', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (245, N'FMMU_PrfVol', N'Proof volume', N'pr vol', N'9fe432f9-df97-4abf-8cdb-7e0d8abd8dc4', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (246, N'FMMU_PrfMas', N'Proof mass', N'pr mass', N'da060f58-680a-48db-8ac4-a8ad3d521348', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex], [EngineeringUnitCode], [EngineeringUnitName], [EngineeringUnitAbbreviation], [EngineeringUnitGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (247, N'FMMU_Ft3Lb', N'Cubic feet / pound', N'ft³/lb', N'e9279d16-7a28-4d11-b921-cd06cf232524', N'6/18/2012 1:04:28 PM +00:00', N'Administrator', N'6/18/2012 1:04:28 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblEquipmentType])=0
BEGIN
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'TRAILER_TYPE', N'TRAILER TYPE', N'55e808d0-f46b-46aa-bbbd-c015fd5a32f8', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TRACTOR_TYPE', N'TRACTOR TYPE', N'f00c8e3c-508e-4aa5-934d-35c3e103206e', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'AIRCRAFT_TYPE', N'AIRCRAFT TYPE', N'5955f323-1f4f-4d50-89d6-3dbf26760f46', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'RAILCAR_TYPE', N'RAILCAR TYPE', N'92787ae5-11da-4927-9dff-3cf43751e578', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'BARGE_TYPE', N'BARGE TYPE', N'1a884ea4-2625-48f2-8b49-28b9c4166fd6', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'COMPARTMENT_TYPE', N'COMPARTMENT TYPE', N'f80345f9-92b7-4a30-89cc-cbff2ac99c5a', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'SHIP_TYPE', N'SHIP TYPE', N'280018d6-54ce-4798-9e72-1177a6fe082a', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'PIPELINE_TYPE', N'PIPELINE TYPE', N'ae0fd49b-4dfa-4f25-af43-40a75609b74d', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'HYDRANT_CART_TYPE', N'HYDRANT CART TYPE', N'3a828d21-65d1-412d-bb29-aa3a24ea2847', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'TANKER_TYPE', N'TANKER TYPE', N'8f77ff26-fb32-4b11-96f1-3306f43ab721', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'STATIONARY_CART_TYPE', N'STATIONARY CART TYPE', N'b5ba769f-df29-4c30-b236-b96d5796e2d5', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'OTHER_TYPE', N'OTHER TYPE', N'597ec71b-5c16-4966-8963-749342258a0e', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'SYSTEM_TYPE', N'SYSTEM TYPE', N'eed797b3-98cb-462c-ae8f-e3dd8802effd', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'TANK_TYPE', N'TANK TYPE', N'b5939386-2422-433b-8225-b5874dfea472', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'FILLSTAND_TYPE', N'FILLSTAND TYPE', N'97f051b7-c8b4-4088-a761-62e56808942e', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblEquipmentType] ([EquipmentTypeIndex], [EquipmentTypeCode], [EquipmentTypeName], [EquipmentTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'MAX_EQUIPMENT_TYPE', N'MAX EQUIPMENT TYPE', N'669fb46c-6bcc-4dec-a408-01733269bff5', N'6/18/2012 9:06:28 AM +00:00', N'Administrator', N'6/18/2012 9:06:28 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblExportResultType])=0
BEGIN
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'CLOSEOUT', N'CLOSEOUT', N'075f8d89-3a0b-49af-9d3a-6a562614af6c', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TRANSACTION', N'TRANSACTION', N'32d88437-fd93-4a10-a07f-3a65cb39b71b', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'TANK', N'TANK', N'1a728245-8cb2-4344-b896-aad5b71de12d', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'MAINTENANCE', N'MAINTENANCE', N'c2ee2954-8704-4eb9-bf7d-5277e72bae9f', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'QUALITY', N'QUALITY', N'2722ee5d-6b29-455d-b411-ff5f0a3ddb5c', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'EXPORT_RESULT', N'EXPORT RESULT', N'729acc73-0c53-4a1d-b8b0-7a3a4399d47e', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblExportResultType] ([ExportResultTypeIndex], [ExportResultTypeCode], [ExportResultTypeName], [ExportResultTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'MAX', N'MAX', N'873c63e3-dd96-439d-8e4b-c7c1648d11e5', N'6/18/2012 9:06:38 AM +00:00', N'Administrator', N'6/18/2012 9:06:38 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblFillMethod])=0
BEGIN
	INSERT INTO [lookup].[tblFillMethod] ([FillMethodIndex], [FillMethodCode], [FillMethodName], [FillMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'SAFEFILL', N'SAFEFILL', N'107f8c3e-5a10-4d06-a460-7e44d57b31f9', N'6/18/2012 1:03:11 PM +00:00', N'Administrator', N'6/18/2012 1:03:11 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblFillMethod] ([FillMethodIndex], [FillMethodCode], [FillMethodName], [FillMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'ACTUAL', N'ACTUAL', N'4de1033f-a3e1-494a-9b81-e4654e5daee7', N'6/18/2012 1:03:11 PM +00:00', N'Administrator', N'6/18/2012 1:03:11 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblFilterField])=0
BEGIN
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'MANAGER', N'Manager', N'29979d9b-c6d5-471a-a9e0-9ce9c6caf524', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'OWNER', N'Owner', N'f63a5ca2-bbef-40fd-8512-5b47dbb83659', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'SUPPLIER', N'Supplier', N'7980ee72-f759-47e0-99c3-7a145db8e6c8', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'PONUMBER', N'PO number', N'f5d66c29-18ee-4da0-8e30-95255853d728', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'SHIPTO', N'Ship to', N'7480e2f7-a91a-48a3-8355-430a4e3cb52e', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'BILLTO', N'Bill to', N'fd5fd06b-3e43-4aa6-a8c6-64937cbc2e31', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'DOCUMENTNUMBER', N'Document number', N'1145530c-96f8-4a82-84b5-e3b166a72020', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
	INSERT INTO [lookup].[tblFilterField] ([FilterFieldIndex], [FilterFieldCode], [FilterFieldName], [FilterFieldGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'PRODUCT', N'Product', N'964c1a0f-17a5-4960-a8cb-6d4644a2ac73', N'2012-06-18 13:04:39', N'Administrator', N'2012-06-18 13:04:39', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblListViewFieldType])=0
BEGIN
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TRANSACTION_ALIAS', N'TRANSACTION ALIAS', N'5cecd72d-c75e-4a40-96ce-114cf6b33a93', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'TRANSACTION_ALIAS_FIELD', N'TRANSACTION ALIAS FIELD', N'bbc0000e-057a-4642-8ac1-c41a7a45febc', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'USER_DATA_FIELD', N'USER DATA FIELD', N'f51c8707-8bbd-4b59-9119-eedabe6d81e0', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'STANDARD_FIELD', N'STANDARD FIELD', N'2562c3a8-2249-4032-9dad-53a57c380ee3', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'LINE_ITEM_USER_DATA_FIELD', N'LINE ITEM USER DATA FIELD', N'b2175e87-54d1-4644-a41e-d3065f49a8f1', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'AGGREGATE_FIELD', N'AGGREGATE FIELD', N'2c987018-36da-4e28-a586-56783f53e36c', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex], [ListViewFieldTypeCode], [ListViewFieldTypeName], [ListViewFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'TYPE_MAX', N'TYPE MAX', N'415103ab-30cb-4dbd-8c6d-9b2c61bde7eb', N'6/18/2012 1:00:22 PM +00:00', N'Administrator', N'6/18/2012 1:00:22 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblListViewStandardType])=0
BEGIN
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'LEDGER', N'LEDGER', N'f93586cb-d1e1-46b5-8509-9673f912dcfd', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'METER_RECONCILIATION_SUMMARY', N'METER RECONCILIATION SUMMARY', N'09f7791c-abdd-4f53-80c2-f466c5b149b6', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'RECEIPT_RECONCILIATION', N'RECEIPT RECONCILIATION', N'7b235ca7-1d24-4e7d-9c5a-9c2e2391540f', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'INVENTORY_RECONCILIATION', N'INVENTORY RECONCILIATION', N'b7c950c3-089a-40ca-8850-25ae03236f83', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'CLOSEOUT', N'CLOSEOUT', N'969d6917-b311-42ab-8ef7-f5fb72106fd1', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'EQUIPMENT_TRANSACTION', N'EQUIPMENT TRANSACTION', N'b17c0625-d385-47ca-96e1-ba81fc51d6a0', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'RECEIPT_ASSIGNMENT_ASSIGNED', N'RECEIPT ASSIGNMENT ASSIGNED', N'e73869ad-a83a-417f-beb9-2a1a64f0962d', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'RECEIPT_ASSIGNMENT_AVAILABLE', N'RECEIPT ASSIGNMENT AVAILABLE', N'309235f0-f417-4248-876d-ea5fc5fdea04', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'AUTOMATIC_PHYSICAL_INVENTORY', N'AUTOMATIC PHYSICAL INVENTORY', N'e5b141f7-935b-4cb8-aa88-d5d631e97b00', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'ORDER', N'ORDER', N'8fa1a271-ba67-4c9b-93fc-213e093b1a8f', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'ORDER_ASSOCIATED_TX', N'ORDER ASSOCIATED TX', N'29a1606e-325e-4c2c-831f-364dca7f7bbf', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'BOL_SUMMARY', N'BOL SUMMARY', N'2ad21f28-4cd1-4d79-9e55-62e5a6021b95', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'SUPPLY_ORDER', N'SUPPLY ORDER', N'1424d169-d89c-46ec-9579-2c6045008c79', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'SUPPLY_ORDER_ASSOCIATED_TX', N'SUPPLY ORDER ASSOCIATED TX', N'a0a7f70c-aa4a-4e5d-abb5-563dec4bfd43', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'INVOICE', N'INVOICE', N'cfdf735b-a2bd-4cfd-814d-a15234216ce9', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'ASSOCIATED_TX', N'ASSOCIATED TX', N'96e7086b-8f6e-49b7-ad50-c840731cc3c8', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'RECOVERY', N'RECOVERY', N'1af1c81a-db51-403f-aab8-591a38c0e544', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'RECOVERY_ASSOCIATED_TX', N'RECOVERY ASSOCIATED TX', N'c4b22f23-4655-426e-a256-87f5a545f4a1', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'BULK_ASSOCIATED_TX', N'BULK ASSOCIATED TX', N'00fd1cd2-c940-461a-896c-9801dd0065c6', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'METER_RECONCILIATION_DETAIL', N'METER RECONCILIATION DETAIL', N'b6f78a25-b68e-45f9-ac85-ba479416a4f4', N'6/19/2012 9:06:48 AM -04:00', N'Administrator', N'6/19/2012 9:06:48 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'AUTO_DISTRIBUTION_RULE', N'AUTO DISTRIBUTION RULE', N'646fe8d9-cb87-4097-a93e-54d73ce54195', N'6/19/2012 9:09:01 AM -04:00', N'Administrator', N'6/19/2012 9:09:01 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewStandardType] ([ListViewStandardTypeIndex], [ListViewStandardTypeCode], [ListViewStandardTypeName], [ListViewStandardTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'TYPE_MAX', N'TYPE MAX', N'745df152-69c6-448d-8415-1ea81395a914', N'6/18/2012 1:02:39 PM +00:00', N'Administrator', N'6/18/2012 1:02:39 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblListViewType])=0
BEGIN
	INSERT INTO [lookup].[tblListViewType] ([ListViewTypeIndex], [ListViewTypeCode], [ListViewTypeName], [ListViewTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TRANSACTION_LIST', N'TRANSACTION LIST', N'e8c1501c-1529-4232-a3c2-281e86a8d997', N'6/18/2012 1:00:28 PM +00:00', N'Administrator', N'6/18/2012 1:00:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewType] ([ListViewTypeIndex], [ListViewTypeCode], [ListViewTypeName], [ListViewTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'STANDARD', N'STANDARD', N'3d59cc80-c017-41cc-a9ac-8e91ba5794c1', N'6/18/2012 1:00:28 PM +00:00', N'Administrator', N'6/18/2012 1:00:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewType] ([ListViewTypeIndex], [ListViewTypeCode], [ListViewTypeName], [ListViewTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'AGGREGATE', N'AGGREGATE', N'66beffd9-5ac0-4d42-9080-d9133499319f', N'6/18/2012 1:00:28 PM +00:00', N'Administrator', N'6/18/2012 1:00:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblListViewType] ([ListViewTypeIndex], [ListViewTypeCode], [ListViewTypeName], [ListViewTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'TYPE_MAX', N'TYPE MAX', N'd6b90f70-a396-4247-ae18-6b5cd1b8cafa', N'6/18/2012 1:00:28 PM +00:00', N'Administrator', N'6/18/2012 1:00:28 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblMailServerConnectMode])=0
BEGIN
	INSERT INTO [lookup].[tblMailServerConnectMode] ([MailServerConnectModeIndex], [MailServerConnectModeCode], [MailServerConnectModeName], [MailServerConnectModeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'LAN', N'LAN', N'a6c50c9a-4d80-4873-b81e-b5c53166a90e', N'6/18/2012 1:03:15 PM +00:00', N'Administrator', N'6/18/2012 1:03:15 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMailServerConnectMode] ([MailServerConnectModeIndex], [MailServerConnectModeCode], [MailServerConnectModeName], [MailServerConnectModeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'DIALUP', N'DIALUP', N'b7693ee6-14cb-44b1-be01-6495f808f84e', N'6/18/2012 1:03:15 PM +00:00', N'Administrator', N'6/18/2012 1:03:15 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblMajorCorrectionType])=0
BEGIN
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'CORR_NONE', N'CORR NONE', N'1e2bdba4-2929-4183-9619-df335e124474', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'CORR_NONE_1980', N'CORR NONE 1980', N'86bf0304-65c7-49b2-9c16-606c50d147b5', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'CORR_API_C', N'CORR API C', N'71824d4d-8d39-4821-9b44-70801dbcbf06', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'CORR_API_C_1980', N'CORR API C 1980', N'13a372a5-7910-4e6b-83ee-8d8d9a16fb9c', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'CORR_API_F', N'CORR API F', N'e983364f-0274-4fb6-9ee9-5ea015010216', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'CORR_API_F_1980', N'CORR API F 1980', N'f8c49bef-77e8-454e-ab02-92361d3bac19', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'CORR_POLYNOMIAL_F', N'CORR POLYNOMIAL F', N'cbb1d20c-f0ab-4fc1-941f-45e3f61cc8ba', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'CORR_POLYNOMIAL_F_1980', N'CORR POLYNOMIAL F 1980', N'dca385c6-d179-478b-9bf0-b2a623e577be', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'CORR_LPG_C', N'CORR LPG C', N'020bf5d7-4aa4-4a46-8e49-2a4d36bf6b1e', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'CORR_LPG_C_1980', N'CORR LPG C 1980', N'1e4ce790-8163-43ad-a054-b45b4ba045a8', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'CORR_ASTM_D1555_F', N'CORR ASTM D1555 F', N'0023722b-48e4-4008-be62-ebd11a6ef22b', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'CORR_ASTM_D1555_F_1980', N'CORR ASTM D1555 F 1980', N'45045356-ca97-4912-9c84-5c3b3197685c', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'CORR_ASTM_D1555_C', N'CORR ASTM D1555 C', N'ec1143a2-c960-4970-a639-368fcc74203c', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'CORR_ASTM_D1555_C_1980', N'CORR ASTM D1555 C 1980', N'68335900-69e5-4d94-8133-801d25b5a3e6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'CORR_JAPAN_NONE', N'CORR JAPAN NONE', N'7a2ca8a0-43f2-4cc6-a9f0-2b17aeaf445a', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'CORR_JAPAN_JIS_2249', N'CORR JAPAN JIS 2249', N'11a8b8d3-ff02-4a95-ac67-3efd92621448', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'CORR_JAPAN_JIS_2250', N'CORR JAPAN JIS 2250', N'23adc171-5819-400e-b250-20f6cf9df040', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'CORR_JAPAN_ASTM_D1555', N'CORR JAPAN ASTM D1555', N'8a986f86-ebfd-4f42-97b5-2a3912401a1b', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'CORR_JAPAN_ASTM_D1250', N'CORR JAPAN ASTM D1250', N'0de0d96b-7b84-40e1-9cf6-3661988d5ddd', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'CORR_JAPAN_CHEMICAL', N'CORR JAPAN CHEMICAL', N'0129671a-1f93-4598-9337-8e55818b7180', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'CORR_JAPAN_JIS_2249_TABLE', N'CORR JAPAN JIS 2249 TABLE', N'218e0706-461f-42d7-991b-57775a6011c3', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'CORR_GBT', N'CORR GBT', N'37bea432-be3a-4fc9-9e6e-b1873bbe6f8b', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'CORR_GOST', N'CORR GOST', N'df2a0e10-a91e-405a-8a8b-a4b937566a7c', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'CORR_ASPHALT', N'CORR ASPHALT', N'8170a2e5-4ed5-4244-943e-76bc3d3e715f', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (24, N'CORR_ASTM_D1250_1952', N'CORR ASTM D1250 1952', N'4a4bd66d-215b-4eb7-a347-0a0f9ca712f6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (25, N'CORR_ASTM_COMM_2004', N'CORR ASTM COMM 2004', N'bc86cc46-5a77-49c5-ae6a-cf35ecedc38a', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'CORR_ASTM_D1555_F_2009', N'CORR ASTM D1555 F 2009', N'968189EE-23A9-475C-9D38-9BF7DFB6E530', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (27, N'MAX_MAJOR_CORRECTION_TYPE', N'MAX MAJOR CORRECTION TYPE', N'9f6887cf-4bcc-4ef3-b78c-7f6d40f21120', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblMinorCorrectionType])=0
BEGIN
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'CORR_NONE', N'CORR NONE', N'5142acad-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'CORR_API54A', N'CORR API54A', N'5142acae-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'CORR_API54B', N'CORR API54B', N'5142acaf-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'CORR_API54C', N'CORR API54C', N'5142acb0-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'CORR_API54D', N'CORR API54D', N'5142acb1-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'CORR_API54A_30', N'CORR API54A 30', N'5142acb2-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'CORR_API54B_30', N'CORR API54B 30', N'5142acb3-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'CORR_API54C_30', N'CORR API54C 30', N'5142acb4-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'CORR_API54D_30', N'CORR API54D 30', N'5142acb5-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'CORR_API60A', N'CORR API60A', N'5142acb6-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'CORR_API60B', N'CORR API60B', N'5142acb7-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'CORR_API60D', N'CORR API60D', N'5142acb8-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'CORR_API6A', N'CORR API6A', N'5142acb9-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'CORR_API6B', N'CORR API6B', N'5142acba-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'CORR_API6C', N'CORR API6C', N'5142acbb-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'CORR_API6D', N'CORR API6D', N'5142acbc-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'CORR_API24E', N'CORR API24E', N'5142acbd-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'CORR_POLYNOMIAL', N'CORR POLYNOMIAL', N'5142acbe-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'CORR_LPG', N'CORR LPG', N'5142acbf-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'CORR_BENZENE', N'CORR BENZENE', N'5142acc0-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'CORR_TOLUENE', N'CORR TOLUENE', N'5142acc1-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'CORR_M_XYLENE', N'CORR M XYLENE', N'5142acc2-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'CORR_STYRENE', N'CORR STYRENE', N'5142acc3-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'CORR_O_XYLENE', N'CORR O XYLENE', N'5142acc4-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (24, N'CORR_P_XYLENE', N'CORR P XYLENE', N'5142acc5-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (25, N'CORR_CYCLO_HEXANE', N'CORR CYCLO HEXANE', N'5142acc6-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'CORR_ETHYL_BENZENE', N'CORR ETHYL BENZENE', N'5142acc7-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (27, N'CORR_CUMENE', N'CORR CUMENE', N'5142acc8-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (28, N'CORR_300_AROMATIC', N'CORR 300 AROMATIC', N'5142acc9-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (29, N'CORR_350_AROMATIC', N'CORR 350 AROMATIC', N'5142acca-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (30, N'CORR_JIS_TABLE2', N'CORR JIS TABLE2', N'5142accb-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (31, N'CORR_ASTM_TABLE55', N'CORR ASTM TABLE55', N'5142accc-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (32, N'CORR_ASTM_TABLE6X_54A', N'CORR ASTM TABLE6X_54A', N'5142accd-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (33, N'CORR_ASTM_TABLE6X_54B', N'CORR ASTM TABLE6X_54B', N'5142acce-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (34, N'CORR_ASTM_TABLE2', N'CORR ASTM TABLE2', N'5142accf-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (35, N'CORR_JIS_CHEMICAL1', N'CORR JIS CHEMICAL1', N'5142acd0-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (36, N'CORR_JIS_CHEMICAL2', N'CORR JIS CHEMICAL2', N'5142acd1-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (37, N'CORR_API54A_TABLE', N'CORR API54A TABLE', N'5142acd2-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (38, N'CORR_API54B_TABLE', N'CORR API54B TABLE', N'5142acd3-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (39, N'CORR_API54D_TABLE', N'CORR API54D TABLE', N'5142acd4-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (40, N'CORR_APIGBT60A', N'CORR APIGBT60A', N'5142acd5-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (41, N'CORR_APIGBT60B', N'CORR APIGBT60B', N'5142acd6-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (42, N'CORR_APIGBT60D', N'CORR APIGBT60D', N'5142acd7-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (43, N'CORR_3900_85_20C', N'CORR 3900 85 20C', N'5142acd8-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (44, N'CORR_D4311DEGC_2004', N'CORR D4311DEGC 2004', N'5142acd9-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (45, N'CORR_D4311DEGF_2004', N'CORR D4311DEGF 2004', N'5142acda-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (46, N'CORR_TABLE7', N'CORR TABLE7', N'5142acdb-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (47, N'CORR_D4311DEGC_2009', N'CORR D4311DEGC 2009', N'5142acdc-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (48, N'CORR_D4311DEGF_2009', N'CORR D4311DEGF 2009', N'5142acdd-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (49, N'CORR_D125020DEGC', N'CORR D125020DEGC', N'5142acde-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (50, N'CORR_ALPHA60_SUPPLIED', N'CORR ALPHA60 SUPPLIED', N'5142acdf-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (51, N'CORR_CRUDE_OIL', N'CORR CRUDE OIL', N'5142ace0-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (52, N'CORR_REFINED_PRODUCTS', N'CORR REFINED PRODUCTS', N'5142ace1-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (53, N'CORR_LUBRICATION_OIL', N'CORR LUBRICATION OIL', N'5142ace2-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMinorCorrectionType] ([MinorCorrectionTypeIndex], [MinorCorrectionTypeCode], [MinorCorrectionTypeName], [MinorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (54, N'MAX_MINOR_CORRECTION_TYPE', N'MAX MINOR CORRECTION TYPE', N'5142ace3-cb06-11e7-8f37-80000bf0a2b6', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblConsortiumType])=0
BEGIN
	INSERT INTO [lookup].[tblConsortiumType] ([ConsortiumTypeIndex], [ConsortiumTypeCode], [ConsortiumTypeName], [ConsortiumTypeIndexGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'NonConsortium', N'Non-Consortium', N'6a3d1065-c401-43c6-9ffd-16a926c4ad3c', N'10/19/2012 1:02:58 PM +00:00', N'Administrator', N'10/19/2018 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblConsortiumType] ([ConsortiumTypeIndex], [ConsortiumTypeCode], [ConsortiumTypeName], [ConsortiumTypeIndexGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Consortium', N'Consortium', N'bc5dd10c-800c-4b9f-b207-3902c3a5a857', N'10/19/2018 1:02:58 PM +00:00', N'Administrator', N'10/19/2018 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblConsortiumType] ([ConsortiumTypeIndex], [ConsortiumTypeCode], [ConsortiumTypeName], [ConsortiumTypeIndexGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Itinerant', N'Itinerant', N'195ebee3-c8fc-4433-80c6-c874154f468a', N'10/19/2012 1:02:58 PM +00:00', N'Administrator', N'10/19/2018 1:02:58 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblMapSource]) = 0
BEGIN
	INSERT INTO [lookup].[tblMapSource] ([MapSourceIndex], [MapSourceCode], [MapSourceName], [MapSourceGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'OPENSTREETMAP', N'OpenStreetMap', N'B8E692CC-A4CC-40C2-9D0E-42DFDC07A1A0', N'2/2/2016 1:03:42 PM +00:00', N'Administrator', N'2/2/2016 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMapSource] ([MapSourceIndex], [MapSourceCode], [MapSourceName], [MapSourceGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'MAPSERVERMAP', N'MapServer Map', N'385AE90C-3202-4509-942B-F846CAC18402', N'2/2/2016 1:03:42 PM +00:00', N'Administrator', N'2/2/2016 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMapSource] ([MapSourceIndex], [MapSourceCode], [MapSourceName], [MapSourceGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'GOOGLEMAP', N'Google Map', N'63AFA7E3-1FD2-4AF3-9EF9-A255BA4D4B08', N'2/2/2016 1:03:42 PM +00:00', N'Administrator', N'2/2/2016 1:03:42 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMapSource] ([MapSourceIndex], [MapSourceCode], [MapSourceName], [MapSourceGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'BINGMAP', N'Bing Map', N'68498358-33A4-484B-889B-FB1EF80201E2', N'6/16/2016 1:03:42 PM +00:00', N'Administrator', N'6/16/2016 1:03:42 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblMenuItemType])=0
BEGIN
	EXEC [lookup].[AddMenuItemType] 1, N'DYNAMIC_ACCOUNTING_IMPORT_EXPORT', N'88e537e9-09fe-456e-9066-2da0165ffc65'
	EXEC [lookup].[AddMenuItemType] 2, N'DYNAMIC_ACCOUNTING_INVOICE_ENTRY', N'9f721672-e795-4937-9f70-9797ed2ee8a8'
	EXEC [lookup].[AddMenuItemType] 3, N'DYNAMIC_ADD_TRANSACTION', N'0429a4ca-9e87-4882-b0f6-7f4bd0fb99f3'
	EXEC [lookup].[AddMenuItemType] 4, N'DYNAMIC_ADD_SALES_ORDER', N'0680a712-e542-4057-bcac-f7f592983224'
	EXEC [lookup].[AddMenuItemType] 5, N'DYNAMIC_ADD_SUPPLY_ORDER', N'eaf968d7-d486-4be5-adfd-cbec112e97d6'
	EXEC [lookup].[AddMenuItemType] 6, N'DYNAMIC_REPORT', N'470a1306-094a-4cf1-846c-2d7c606d371b'
	EXEC [lookup].[AddMenuItemType] 7, N'DYNAMIC_OPERATIONS_PROCUREMENT', N'0a483db1-b3f1-4839-806d-8f5f130dece7'
	EXEC [lookup].[AddMenuItemType] 8, N'DYNAMIC_ACCOUNTING_INVOICE_ENTRY_ADF', N'e3a66fed-ad42-4d23-957e-a58f27e0af12'
	EXEC [lookup].[AddMenuItemType] 9, N'DYNAMIC_ACCOUNTING_TRANSACTION_EDITOR', N'C3442B3E-6B06-4931-980F-6F27F22CF465'

	EXEC [lookup].[AddMenuItemType] 1001, N'ACCOUNTING_MAIN_CLOSEOUT_SUMMARY', N'59a035df-418f-4aae-9a7b-43f664916e3d'
	EXEC [lookup].[AddMenuItemType] 1002, N'ACCOUNTING_COMPANIES_CERTIFICATES_AND_PERMITS', N'169b0ba9-7d0c-4f03-a5ed-837d85447698'
	EXEC [lookup].[AddMenuItemType] 1003, N'ACCOUNTING_COMPANIES_COMPANIES', N'2f0a7b27-e57c-4313-bd60-d017442d6194'
	EXEC [lookup].[AddMenuItemType] 1004, N'ACCOUNTING_COMPANIES_GROUPS', N'088c1238-5491-4410-8478-c38a2710ec91'
	EXEC [lookup].[AddMenuItemType] 1005, N'ACCOUNTING_COMPANIES_HIERARCHY', N'0462fd11-6054-4e26-b738-103db183dfc7'
	EXEC [lookup].[AddMenuItemType] 1006, N'ACCOUNTING_COMPANIES_COMPANY_ROLE_ASSIGNMENTS', N'96c2d3d8-ad1c-401e-8f39-90d0f2ab852d'
	EXEC [lookup].[AddMenuItemType] 1007, N'ACCOUNTING_COMPANIES_TYPES', N'950d3cfa-e47f-4ba3-a971-a6abde61958b'
	EXEC [lookup].[AddMenuItemType] 1008, N'ACCOUNTING_MAIN_END_OF_MONTH_APPROVAL', N'8bd09a12-0956-4e0b-aa33-68570adaad4f'
	EXEC [lookup].[AddMenuItemType] 1009, N'ACCOUNTING_IMPORT_EXPORT_6_0_ERROR_RETRIEVAL', N'3ad4801a-e5e3-4a0b-b671-638829cf9d82'
	EXEC [lookup].[AddMenuItemType] 1010, N'ACCOUNTING_IMPORT_EXPORT_6_0_FILE_UPLOAD', N'8a66bb37-9f70-43b8-93b9-b52997fff112'
	EXEC [lookup].[AddMenuItemType] 1011, N'ACCOUNTING_IMPORT_EXPORT_ARTS_IMPORT', N'8559f82e-98a9-406b-bda9-747fdd427f5d'
	EXEC [lookup].[AddMenuItemType] 1012, N'ACCOUNTING_IMPORT_EXPORT_BASE_ENTERPRISE_EXPORT', N'b51869ea-8056-4a0d-a0a0-e29d0fcf7e29'
	EXEC [lookup].[AddMenuItemType] 1013, N'ACCOUNTING_IMPORT_EXPORT_ENTERPRISE_EXPORT', N'5546a665-54ba-4204-8c95-91fb6a226ef9'
	EXEC [lookup].[AddMenuItemType] 1014, N'ACCOUNTING_IMPORT_EXPORT_GAS_STATION_IMPORT', N'4280827f-e36a-4ea6-9d1d-6a6a8c671cd0'
	EXEC [lookup].[AddMenuItemType] 1015, N'ACCOUNTING_IMPORT_EXPORT_MFCS_IMPORT', N'20b947ee-a248-4978-bd28-107e79f34953'
	EXEC [lookup].[AddMenuItemType] 1016, N'ACCOUNTING_IMPORT_EXPORT_SHIPMENT_DOCUMENT_IMPORT', N'ba2fd537-1b30-4af5-991a-c024b18ea9f4'
	EXEC [lookup].[AddMenuItemType] 1017, N'ACCOUNTING_MAIN_INCOMING_TRUCK', N'5ca8067f-8d5e-47a5-bab9-d911c29fffcf'
	EXEC [lookup].[AddMenuItemType] 1018, N'ACCOUNTING_MAIN_INVENTORY_RECONCILIATION', N'd38e48e3-a082-4e46-bde4-834a9ec46e32'
	EXEC [lookup].[AddMenuItemType] 1019, N'ACCOUNTING_MAIN_WAC_SUMMARY', N'53fec264-ba4a-43b0-925e-e8ce84ce1da9'
	EXEC [lookup].[AddMenuItemType] 1020, N'ACCOUNTING_INVOICE_ENTRY_BULK_PAYMENT_SUMMARY', N'c31ce277-9f4b-4b1b-ac9a-87323ec5a5fa'
	EXEC [lookup].[AddMenuItemType] 1021, N'ACCOUNTING_INVOICE_ENTRY_INVOICE_PAYABLE_SUMMARY',  N'9f4510a0-ea04-4dbe-a41e-2056d5705832'
	EXEC [lookup].[AddMenuItemType] 1022, N'ACCOUNTING_INVOICE_ENTRY_INVOICE_SUMMARY',  N'd57d579f-e572-4117-bbad-793754daa8c7'
	EXEC [lookup].[AddMenuItemType] 1023, N'ACCOUNTING_INVOICE_ENTRY_RECEIVABLE_SUMMARY', N'1ab7c1d4-20ec-4c80-b9fe-8b129737dd60'
	EXEC [lookup].[AddMenuItemType] 1024, N'ACCOUNTING_INVOICE_ENTRY_RECEIVABLE_SUMMARY_ADF', N'36098549-c126-40ce-b99f-0dd95f945757'
	EXEC [lookup].[AddMenuItemType] 1025, N'ACCOUNTING_MAIN_LEDGER',  N'82439d98-bc58-4066-83eb-199c54914553'
	EXEC [lookup].[AddMenuItemType] 1026, N'ACCOUNTING_MAIN_OPERATIONS',  N'8573709a-ff59-4674-8f73-6f9b26c36e51'
	EXEC [lookup].[AddMenuItemType] 1027, N'ACCOUNTING_MAIN_METER_RECONCILIATION',  N'6fa2819b-d823-4b63-921c-f60a34efe029'
	EXEC [lookup].[AddMenuItemType] 1028, N'ACCOUNTING_AUTO_DISTRIBUTION',  N'814ac605-9560-4e9e-9b26-44c0d28da162'
	EXEC [lookup].[AddMenuItemType] 1029, N'ACCOUNTING_EXTERNAL_SYSTEM_TRANSACTION_ERROR_SUMMARY',  N'3e76e19a-ffcd-48b5-890b-a06a90e4aba3'
	EXEC [lookup].[AddMenuItemType] 1030, N'ACCOUNTING_IMPORT_EXPORT_BASE_ENTERPRISE_EXPORT_MANUAL', N'ffe6f15a-1680-40a9-92f3-0dc9ae3c30a5'
	EXEC [lookup].[AddMenuItemType] 1031, N'ACCOUNTING_IMPORT_EXPORT_BASE_ENTERPRISE_RESULT_IMPORT_MANUAL',  N'd0e587a1-2ea5-46e4-b18a-26c68d3860c9'
	EXEC [lookup].[AddMenuItemType] 1032, N'ACCOUNTING_IMPORT_EXPORT_RAPS_IMPORT', N'acbe439b-6d5e-42c2-9090-bf5ce8507368'
	EXEC [lookup].[AddMenuItemType] 1033, N'ACCOUNTING_IMPORT_EXPORT_ENTERPRISE_UPLOAD_MANUAL', N'f3300f61-158d-4a9a-8151-18739ca4cf55'
	EXEC [lookup].[AddMenuItemType] 1034, N'ACCOUNTING_IMPORT_EXPORT_SEND_ENTERPRISE_TRANSACTIONS',  N'ec52ed00-90fa-40f6-887a-631e1f63a1aa'
	EXEC [lookup].[AddMenuItemType] 1035, N'ACCOUNTING_MAIN_TRANSACTION_SUMMARY', N'C55DD6FA-CD62-4FE8-BC3D-8B9E43A3AABB'
	EXEC [lookup].[AddMenuItemType] 1036, N'ACCOUNTING_ERROR_SUMMARY', N'c891dc8d-0851-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 1037, N'ACCOUNTING_IMPORT_INTOPLANE_DATA', N'c891dc8e-0851-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 1038, N'ACCOUNTING_MAIN_MOVEMENT',  N'c891dc8f-0851-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 1039, N'ACCOUNTING_EXTERNAL_SYSTEM_UNACKNOWLEDGED_TRANSACTION_SUMMARY', N'c891dc90-0851-11e8-ab4e-80000bf0a2b6'

	EXEC [lookup].[AddMenuItemType] 2001, N'ADMIN_SECURITY_CHANGE_PASSWORD', N'99c408c5-5de3-4233-812a-dfd020b59b9c'
	EXEC [lookup].[AddMenuItemType] 2002, N'ADMIN_SECURITY_USERS', N'264f96bc-cdf2-41c6-80dc-d4aecb25b1f6'
	EXEC [lookup].[AddMenuItemType] 2003, N'ADMIN_SITES_ENTITY_ASSIGNMENTS', N'fe0d3080-d1f5-497a-9a16-e01f01f5fe79'
	EXEC [lookup].[AddMenuItemType] 2004, N'ADMIN_SITES_ENTITY_OWNERSHIP', N'e54839e9-7da3-4ace-bfbb-490d99e29845'
	EXEC [lookup].[AddMenuItemType] 2005, N'ADMIN_SYSTEM_DATA_DICTIONARY', N'c24e3b84-91ff-41d4-804d-e034ac27c99f'
	EXEC [lookup].[AddMenuItemType] 2006, N'ADMIN_SYSTEM_REGIONAL_SETTINGS', N'147b0544-15b1-49df-9d4c-f9e5ae8ead86'
	EXEC [lookup].[AddMenuItemType] 2007, N'ADMIN_SYSTEM_SYSTEM_SETTINGS', N'8a6666a3-a1bf-447c-856f-4e87eb7bb95b'
	EXEC [lookup].[AddMenuItemType] 2008, N'ADMIN_SECURITY_USER_GROUPS', N'006d58b0-5e6c-4ea6-9ea5-b1a5f2f3ca7f'
	EXEC [lookup].[AddMenuItemType] 2009, N'ADMIN_SITES_SITES', N'4a68fe18-ae55-4f44-8644-eb4a57138406'
	EXEC [lookup].[AddMenuItemType] 2010, N'ADMIN_SECURITY_PASSWORD_SETTINGS', N'a5efb02f-9864-41d2-a402-e2f9574875c7'
	EXEC [lookup].[AddMenuItemType] 2011, N'ADMIN_FIELD_LEVEL_CONFIGURATION', N'76451f48-f947-4c21-9dc4-bb3df3e9d234'
	EXEC [lookup].[AddMenuItemType] 2012, N'ADMIN_SECURITY_USER_PERMIMISSIONS', N'85597573-ba8a-4114-a511-58ebefe1fb78'
	EXEC [lookup].[AddMenuItemType] 2013, N'ADMIN_SYSTEM_CONFIGURATION_SETTINGS', N'6454E897-A2F3-4D1F-860A-1D1C0099D134'
	EXEC [lookup].[AddMenuItemType] 2014, N'ADMIN_SYSTEM_WEB_LINKS_CONFIGURATION',  N'075fefad-0853-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 2015, N'ADMIN_SYSTEM_DASHBOARD',  N'075fefae-0853-11e8-ab4e-80000bf0a2b6'

	EXEC [lookup].[AddMenuItemType] 3001, N'ASSETS_EQUIPMENT_EQUIPMENT', N'3c58e5c0-9a30-424b-97ab-a0798821940d'
	EXEC [lookup].[AddMenuItemType] 3002, N'ASSETS_EQUIPMENT_LICENSES', N'405b9158-a93e-4716-9d81-34bcf7d019f2'
	EXEC [lookup].[AddMenuItemType] 3003, N'ASSETS_EQUIPMENT_MAINTENANCE_REASONS', N'07920186-a227-46f6-816d-444cb2ffbf00'
	EXEC [lookup].[AddMenuItemType] 3004, N'ASSETS_EQUIPMENT_STATION', N'3f2b2365-621e-4774-b1eb-9b3920941502'
	EXEC [lookup].[AddMenuItemType] 3005, N'ASSETS_EQUIPMENT_TANKS', N'3945b8e2-49be-4f09-b2e1-0f4bfeb6c1d6'
	EXEC [lookup].[AddMenuItemType] 3006, N'ASSETS_EQUIPMENT_TANK_GROUPS', N'5bdb4b4f-fed5-400a-a93d-e3385520fd9f'
	EXEC [lookup].[AddMenuItemType] 3007, N'ASSETS_EQUIPMENT_TESTS_AND_INSPECTIONS', N'acb37fe9-7c80-4c9d-ae7a-b60671f3b150'
	EXEC [lookup].[AddMenuItemType] 3008, N'ASSETS_EQUIPMENT_TYPE_CLASSES', N'a8535d08-b3d6-4ca6-86eb-0d6e6a5218be'
	EXEC [lookup].[AddMenuItemType] 3009, N'ASSETS_PERSONNEL_HOUSE_CARDS', N'4bd7aecb-e900-4798-856d-4ac2ebfd1068'
	EXEC [lookup].[AddMenuItemType] 3010, N'ASSETS_PERSONNEL_LICENSES', N'741aeaa5-de9c-4889-91d0-c13c43d4e4ef'
	EXEC [lookup].[AddMenuItemType] 3011, N'ASSETS_PERSONNEL_PERSONNEL', N'69d0a766-8245-4091-a66b-e8f6566b3bcc'
	EXEC [lookup].[AddMenuItemType] 3012, N'ASSETS_PERSONNEL_QUALIFICATIONS', N'ec8d1fd1-dd27-4dcd-a22d-7fb91474d36a'
	EXEC [lookup].[AddMenuItemType] 3013, N'ASSETS_PERSONNEL_TRAINING', N'152d957b-b90e-412c-9e61-911206962209'
	EXEC [lookup].[AddMenuItemType] 3014, N'ASSETS_PRODUCTS_ADDITIVE_PROFILES', N'f3395783-ce0f-44b6-8366-fc8dddaec3de'
	EXEC [lookup].[AddMenuItemType] 3015, N'ASSETS_PRODUCTS_DOT_HAZARDOUS_MESSAGES', N'7f04882f-e69d-40b8-a453-7c8115d244f4'
	EXEC [lookup].[AddMenuItemType] 3016, N'ASSETS_PRODUCTS_PRODUCTS', N'7487794a-50ad-4178-bbaa-221ed10c457a'
	EXEC [lookup].[AddMenuItemType] 3017, N'ASSETS_PRODUCTS_PRODUCT_ENTRY_MESSAGES', N'88c77eab-8ec0-472c-b921-781efb961a49'
	EXEC [lookup].[AddMenuItemType] 3018, N'ASSETS_PRODUCTS_PRODUCT_EXIT_MESSAGES', N'6c88c78d-9f7d-4d68-aeda-f80dffc0497a'
	EXEC [lookup].[AddMenuItemType] 3019, N'ASSETS_PRODUCTS_PRODUCT_GROUPS', N'64cb4e22-c0ee-4b9f-9dbe-a1be4939a99f'
	EXEC [lookup].[AddMenuItemType] 3020, N'ASSETS_PRODUCTS_PRODUCT_MESSAGES', N'1b274f20-412e-4d1a-b064-b373cc1df57f'
	EXEC [lookup].[AddMenuItemType] 3021, N'ASSETS_EQUIPMENT_MOBILE_DEVICES', N'f068941a-a98d-4d3d-9d6b-9feb8b7f3e80'
	EXEC [lookup].[AddMenuItemType] 3022, N'ASSETS_IMPORT_IM_TANK_DATA', N'92B5E9C8-159F-4E67-A44A-2FA67A075C49'

	EXEC [lookup].[AddMenuItemType] 4001, N'CONFIG_ACCOUNTING_AUTO_DISTRIBUTION_REASONS', N'40e0d2e6-4dce-4a89-99fb-caee27176ea5'
	EXEC [lookup].[AddMenuItemType] 4002, N'CONFIG_ACCOUNTING_AUTO_DISTRIBUTION_RULES', N'8f5fd9ba-8f2a-4f82-9cd0-ccbc0f38f1a6'
	EXEC [lookup].[AddMenuItemType] 4003, N'CONFIG_ACCOUNTING_CURRENCIES', N'8f25f233-0254-47db-b15e-a65c3896a4ad'
	EXEC [lookup].[AddMenuItemType] 4004, N'CONFIG_ACCOUNTING_EXCISE_TAX', N'daa9fac9-2c5e-440b-956f-445a590ac2aa'
	EXEC [lookup].[AddMenuItemType] 4005, N'CONFIG_ACCOUNTING_GENERAL_CONFIGURATION', N'b71a1ca4-0777-4557-91c2-ae40ce160425'
	EXEC [lookup].[AddMenuItemType] 4006, N'CONFIG_ACCOUNTING_GST_TAX', N'277440a3-b351-40b0-87ea-422f2fa51fbd'
	EXEC [lookup].[AddMenuItemType] 4007, N'CONFIG_ACCOUNTING_IMPORT_EXPORT', N'7cfea440-21d4-4e16-87cd-8898c29cff0d'
	EXEC [lookup].[AddMenuItemType] 4008, N'CONFIG_ACCOUNTING_LEDGER_AGGREGATE_COLUMNS', N'1109e365-7c4e-467c-bad4-4b840614e559'
	EXEC [lookup].[AddMenuItemType] 4009, N'CONFIG_ACCOUNTING_LEDGER_VIEWS', N'e8cfb422-c699-4e10-8192-eed2223d374c'
	EXEC [lookup].[AddMenuItemType] 4010, N'CONFIG_ACCOUNTING_LOCK_DATES', N'be5cf3ae-d7f7-4459-aed9-059ed25d0e36'
	EXEC [lookup].[AddMenuItemType] 4011, N'CONFIG_ACCOUNTING_MARKUP', N'20ba3e40-88f3-4112-9543-4323b7ed9098'
	EXEC [lookup].[AddMenuItemType] 4012, N'CONFIG_ACCOUNTING_STANDING_OFFER_PRICES', N'68b87863-a4e8-43e1-8d28-635bdee0d791'
	EXEC [lookup].[AddMenuItemType] 4013, N'CONFIG_ACCOUNTING_TRANSACTION_ALIASES', N'ad274191-e5c8-414a-929b-69b3db5c34a6'
	EXEC [lookup].[AddMenuItemType] 4014, N'CONFIG_CONTREC_OPC_PORTS', N'69360529-e070-487c-a21d-5c537f228f4a'
	EXEC [lookup].[AddMenuItemType] 4015, N'CONFIG_CONTREC_OPC_PRESETS', N'0052d751-edb9-4b26-961d-7f8ecf119021'
	EXEC [lookup].[AddMenuItemType] 4016, N'CONFIG_DANIEL_OPC_PORTS', N'4bd1cf5a-d593-4166-b3f2-ad29ab3a4d18'
	EXEC [lookup].[AddMenuItemType] 4017, N'CONFIG_DANIEL_OPC_PRESETS', N'7f442b2d-f178-4f14-85ab-b92fa2f467aa'
	EXEC [lookup].[AddMenuItemType] 4018, N'CONFIG_IMPORT_EXPORT_ENTERPRISE_SETTINGS', N'5b450b27-93a3-4af8-9ab5-044d81209705'
	EXEC [lookup].[AddMenuItemType] 4019, N'CONFIG_IMPORT_EXPORT_ENTITY_IMPORT', N'008fb608-20d4-404a-b971-b14a476b6775'
	EXEC [lookup].[AddMenuItemType] 4020, N'CONFIG_IMPORT_EXPORT_ENTITY_EXPORT', N'd168c0ab-73be-4e2a-91dd-19b7e4f7bf40'
	EXEC [lookup].[AddMenuItemType] 4021, N'CONFIG_LOAD_RACK_ALLOCATION_GROUPS', N'e81ad531-5c51-4d73-891d-01a2a39a8ef4'
	EXEC [lookup].[AddMenuItemType] 4022, N'CONFIG_LOAD_RACK_DATA_EXCHANGE_PROFILE', N'7a4a97c7-7b1c-40bc-ba47-790a2ef53b14'
	EXEC [lookup].[AddMenuItemType] 4023, N'CONFIG_LOAD_RACK_STATION_ARMS', N'b5af94fc-5c98-4570-abd2-3efcadd283c8'
	EXEC [lookup].[AddMenuItemType] 4024, N'CONFIG_OPTOMUX_OPC_CONTROLLERS', N'b3fa47ff-dcf7-49df-b065-7720d7a81b92'
	EXEC [lookup].[AddMenuItemType] 4025, N'CONFIG_OPTOMUX_OPC_PORTS', N'dda7cb75-396b-43e2-a4a6-6f9bbc8316eb'
	EXEC [lookup].[AddMenuItemType] 4026, N'CONFIG_OTHER_DATABASE_AUDIT_LOG', N'd63f42e4-f417-48c8-b6d1-d56e9c4431f0'
	EXEC [lookup].[AddMenuItemType] 4027, N'CONFIG_OTHER_FOOTNOTES', N'a2ed6e19-65ca-4037-ba95-c983ac84bc7a'
	EXEC [lookup].[AddMenuItemType] 4028, N'CONFIG_OTHER_FUEL_CARDS', N'53175211-abe0-42c2-ba50-95055acc1013'
	EXEC [lookup].[AddMenuItemType] 4029, N'CONFIG_OTHER_PROFILES', N'f67e2824-a499-415d-a85d-22c423ba80bb'
	EXEC [lookup].[AddMenuItemType] 4030, N'CONFIG_QUALITY_QUALITY_TAGS', N'63b55cb6-438d-4c22-a4db-97e06086bb5b'
	EXEC [lookup].[AddMenuItemType] 4031, N'CONFIG_QUALITY_TESTS_AND_INSPECTIONS', N'4795c3ae-c40d-4707-aa8b-13a8ff02ab16'
	EXEC [lookup].[AddMenuItemType] 4032, N'CONFIG_QUALITY_TESTS_SETS', N'040ca585-45af-4a4d-80b2-b18dfc79c439'
	EXEC [lookup].[AddMenuItemType] 4033, N'CONFIG_REPORTS_QUERIES_MANAGE_QUERIES', N'd4b0f070-34f5-4481-b93e-f09183792aed'
	EXEC [lookup].[AddMenuItemType] 4034, N'CONFIG_REPORTS_QUERIES_QUERY_SETTINGS', N'5f2c53e9-0e04-4841-9c08-0601de13a719'
	EXEC [lookup].[AddMenuItemType] 4035, N'CONFIG_REPORTS_QUERIES_REPORT_ASSIGNMENT', N'79a35d5d-f78e-4e66-9227-609368a8b68f'
	EXEC [lookup].[AddMenuItemType] 4036, N'CONFIG_REPORTS_QUERIES_REPORT_GROUPS', N'630efb4d-3a4b-40b6-83b9-cdbeb335c06a'
	EXEC [lookup].[AddMenuItemType] 4037, N'CONFIG_SITES_RESERVE_LEVELS', N'e3dba0b2-b259-4a15-9046-3256fdebb39d'
	EXEC [lookup].[AddMenuItemType] 4038, N'CONFIG_SITES_GATES', N'5ffd4e05-594d-421a-a8f6-bf5596c97147'
	EXEC [lookup].[AddMenuItemType] 4039, N'CONFIG_SITES_IATA_CODES', N'59ef0aa8-d400-4276-9e7c-75e14a554269'
	EXEC [lookup].[AddMenuItemType] 4040, N'CONFIG_SITES_PROCESS_VARIABLE_MESSAGES', N'0bbbfd34-a830-489b-a2de-193841f291a6'
	EXEC [lookup].[AddMenuItemType] 4041, N'CONFIG_SMITH_METER_OPC_PRESETS', N'4bdc621b-dc25-4b03-8d51-f23606afce1a'
	EXEC [lookup].[AddMenuItemType] 4042, N'CONFIG_SMITH_METER_OPC_CARD_READERS', N'42ab0e96-bf56-4d9d-b0d4-08281f576203'
	EXEC [lookup].[AddMenuItemType] 4043, N'CONFIG_SMITH_METER_OPC_PORTS', N'4a56f961-4d6b-4ada-88d0-31ced57ba97d'
	EXEC [lookup].[AddMenuItemType] 4044, N'CONFIG_SYSTEM_ALARM_AND_EVENTS', N'316f3e65-7e48-4a3c-a337-7160a6c0f014'
	EXEC [lookup].[AddMenuItemType] 4045, N'CONFIG_SYSTEM_ARCHIVE_DATA', N'a8ca0da0-a7ec-470e-9788-70ad504fc82b'
	EXEC [lookup].[AddMenuItemType] 4046, N'CONFIG_SYSTEM_USER_DATA', N'dc9e6ffa-3547-4a99-b020-fc18bf28ad22'
	EXEC [lookup].[AddMenuItemType] 4047, N'CONFIG_SYSTEM_VIEWS', N'0a93492d-fd0c-43c4-b9d8-035e2204b130'
	EXEC [lookup].[AddMenuItemType] 4048, N'CONFIG_WEIGHT_SCALES_OPC_WEIGHT_SCALES', N'9a18d3f8-c8a2-41b5-903d-4e611da59299'
	EXEC [lookup].[AddMenuItemType] 4049, N'CONFIG_SYSTEM_SYNCHRONIZATION_SETTINGS', N'd500d19f-9e4c-46a7-806b-b5fb81833f46'
	EXEC [lookup].[AddMenuItemType] 4050, N'CONFIG OTHER SERVICE REQUEST MESSAGING', N'9f31456d-2b0d-4f79-80ca-9fb3657c5bd3'
	EXEC [lookup].[AddMenuItemType] 4051, N'CONFIG OTHER SERVICE REQUEST MESSAGING ADAPTORS', N'7c0e533b-9afc-4b00-b828-6dac61c715c4'
	EXEC [lookup].[AddMenuItemType] 4052, N'CONFIG_OTHER_FMAE_INTERFACE_TRANSLATIONS', N'6863dbcb-f1c0-4852-a3ab-c1f5042f2d88'
	EXEC [lookup].[AddMenuItemType] 4053, N'CONFIG_IMPORT_EXPORT_FUEL_CARD_IMPORT', N'BADCF6CD-FFF3-4E21-BFED-10CCF9ABBB9C'
	EXEC [lookup].[AddMenuItemType] 4054, N'CONFIG_OTHER_FUEL_CARD_TYPES', N'6690E2BE-D2B9-441D-8ECB-286ADD912CD0'
	EXEC [lookup].[AddMenuItemType] 4055, N'CONFIG_OTHER_FUEL_CARD_LIMITS', N'9E544406-CF81-4044-A3EE-392BFC011C66'
	EXEC [lookup].[AddMenuItemType] 4056, N'CONFIG_ACCOUNTING_IRS_EXSTARS', N'274710E0-590B-497B-8365-67DBBFFFDBE1'
	EXEC [lookup].[AddMenuItemType] 4057, N'CONFIG_AUTOMATED_FUEL_SERVICE_STATIONS', N'9220e4ee-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4058, N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_DATA_IMPORT', N'9220e4ef-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4059, N'CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION', N'9220e4f0-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4060, N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_IMPORT_BLACKLIST', N'9220e4f1-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4061, N'CONFIG_EXPORT_USER_ACCESS_CONTROL_DATA', N'9220e4f2-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4062, N'CONFIG_SECURITY_BASE_USER_PERMISSIONS', N'9220e4f3-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4063, N'CONFIG_SECURITY_ENTERPRISE_USER_PERMISSIONS', N'9220e4f4-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4064, N'CONFIG_PRODUCTS_PRODUCT_AUTHORIZATION_AND_CONTROL', N'9220e4f5-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4065, N'CONFIG_AUTOMATED_FUEL_SERVICE_DEVICES',  N'9220e4f6-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4066, N'CONFIG_WEIGHT_SCALES_OPC_PORTS', N'9220e4f7-0859-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 4069, N'CONFIG_INVMGR_POINT_TEMPLATE_TYPE', N'E8C35889-7D66-4247-B053-930F4508F4CC'
	EXEC [lookup].[AddMenuItemType] 4070, N'CONFIG_INVMGR_POINT_TEMPLATES', N'E7677E07-0A71-4943-AA50-A0C1387E9849'
	EXEC [lookup].[AddMenuItemType] 4071, N'CONFIG_INVMGR_DRAW', N'0EF37D65-B17E-4E81-8B87-7020A29CF556'
	EXEC [lookup].[AddMenuItemType] 4072, N'CONFIG_INVMGR_PICTURESUMMARY', N'214E509D-6D85-47B1-B735-333CAB3A9AE9'
	EXEC [lookup].[AddMenuItemType] 4073, N'CONFIG_INVMGR_POINT_CATEGORY', N'0be1678b-4330-4863-91d9-a0a6540f0555'
	EXEC [lookup].[AddMenuItemType] 4074, N'CONFIG_INVMGR_DRAW_PROTO', N'590787c3-8dca-4010-9b61-0873d813743b'
	EXEC [lookup].[AddMenuItemType] 4075, N'CONFIG_INVMGR_DRAW_PLATFORM', N'1db47749-c5f2-4cd2-823a-fcdd57593332'
	EXEC [lookup].[AddMenuItemType] 4076, N'CONFIG_INVMGR_POINTS', N'27772525-32FD-4F93-BA1A-0687147AC894'
	EXEC [lookup].[AddMenuItemType] 4077, N'CONFIG_INVMGR_MODULELIBRARY', N'100115bf-c300-4ea9-bccd-22a1539fccc5'
	EXEC [lookup].[AddMenuItemType] 4078, N'CONFIG_INVMGR_POINT_ACCESS_CONFIGURATION', N'C6684B62-3C12-4989-B475-C311FEFF12B2'
	EXEC [lookup].[AddMenuItemType] 4083, N'CONFIG_INVMGR_FCEE_MAPPINGS', N'F8C9452C-ACBD-44F3-8374-7F5E4ADD7D10'
	EXEC [lookup].[AddMenuItemType] 4084, N'CONFIG_INVMGR_FCE_DEVICE_SUMMARY', N'D7D37EBD-0E96-4DB4-A6AF-91776456C40F'
	EXEC [lookup].[AddMenuItemType] 4087, N'CONFIG_INVMGR_ROLLING_STOCK_IMPORT', N'13CA996D-6FB5-40AD-893D-105A079DFE3C'
	EXEC [lookup].[AddMenuItemType] 4088, N'CONFIG_IMPORT_EXPORT_STRAP_TABLE_FILE_IMPORT', N'325B0DFA-DE1D-4785-9040-29F94E0D9A47'

	EXEC [lookup].[AddMenuItemType] 5001, N'DISPATCH_CONFIG_GRID_COLUMNS', N'823aeb10-d389-47d1-be46-f18dfde52bf6'
	EXEC [lookup].[AddMenuItemType] 5002, N'DISPATCH_CONFIG_SETTINGS', N'5db6648b-2cc6-4574-9165-8d1366395ed2'
	EXEC [lookup].[AddMenuItemType] 5003, N'DISPATCH_CONFIG_TOOLBARS', N'60412047-5b62-465e-9fd7-57331758daca'
	EXEC [lookup].[AddMenuItemType] 5004, N'DISPATCH_CONFIG_VALIDATIONS', N'666a2140-4077-477c-b023-cfa37ccc1d02'
	EXEC [lookup].[AddMenuItemType] 5101, N'DISPATCH_OPERATION_EVACUATE', N'52827a7e-085a-47cc-88e1-8e7b18f0eb89'
	EXEC [lookup].[AddMenuItemType] 5102, N'DISPATCH_OPERATION_RELEASE_TO_ACCOUNTING', N'132e9dfb-aff4-4e1f-a15b-323fda38e0d3'
	EXEC [lookup].[AddMenuItemType] 5103, N'DISPATCH_OPERATION_STANDBY_STATUS_BOARD', N'09349e13-ac31-4335-b0f9-e9c31cf4a204'
	EXEC [lookup].[AddMenuItemType] 5201, N'DISPATCH_VIEW_CONTROL_LOG', N'5ff18092-6da0-4809-96f3-f3b687d7954d'
	EXEC [lookup].[AddMenuItemType] 5202, N'DISPATCH_VIEW_DISPATCHERS_LIST', N'b3d2cb05-c374-4142-9952-f8ebbc013d18'
	EXEC [lookup].[AddMenuItemType] 5203, N'DISPATCH_VIEW_GRAPHICAL_VIEW', N'00b7025b-4ebd-464c-9872-349201c2f971'
	EXEC [lookup].[AddMenuItemType] 5204, N'DISPATCH_VIEW_TABULAR_VIEW', N'4b722ad5-b472-4968-ae87-17ce911bb1d2'
	EXEC [lookup].[AddMenuItemType] 5205, N'DISPATCH_VIEW_FLIGHT_LINE_STATUS_DISPLAY', N'd1f3f23d-34bc-4c9b-8685-31be5438691f'

	EXEC [lookup].[AddMenuItemType] 6001, N'MY_MENU_ADD_FAVORITE', N'3a4d0ead-1c22-4e87-94e0-255b7f5d815a'
	EXEC [lookup].[AddMenuItemType] 6002, N'MY_MENU_CONFIG_FAVORITES', N'31410e56-54db-48d1-967f-a1aaf1e0a040'

	EXEC [lookup].[AddMenuItemType] 7001, N'OPERATIONS_INTERFACE_FUEL_TRANSACTIONS', N'38f8c7bb-76ce-4aa9-a5fa-0808787d7ab4'
	EXEC [lookup].[AddMenuItemType] 7002, N'OPERATIONS_INTERFACE_ADOFMS_GROUND_FUEL', N'c15f5454-9e11-4481-bfb9-7556110e57cc'
	EXEC [lookup].[AddMenuItemType] 7003, N'OPERATIONS_MAINTENANCE_ADD_MAINTENANCE_RECORD', N'b9f768ee-7c77-44fb-bede-0829e5f38bed'
	EXEC [lookup].[AddMenuItemType] 7004, N'OPERATIONS_QUALITY_ADD_QUALITY_TAG_RECORD', N'f7a9e30f-4b91-468c-929a-68d35ddbb0f8'
	EXEC [lookup].[AddMenuItemType] 7005, N'OPERATIONS_QUALITY_ADD_TEST_SET_RESULTS', N'5c30fcb7-aeef-4aaa-8ff2-13c81e23fb88'
	EXEC [lookup].[AddMenuItemType] 7006, N'OPERATIONS_LOAD_RACK_ADDITIVE_INTERNAL_METERS', N'9771b010-79a4-4094-9f9b-937c20486e68'
	EXEC [lookup].[AddMenuItemType] 7007, N'OPERATIONS_LOAD_RACK_BILLS_OF_LADING', N'b848ea6c-0800-4eb3-b21d-3ce7280bc571'
	EXEC [lookup].[AddMenuItemType] 7008, N'OPERATIONS_LOAD_RACK_ENABLE_STATION_ARMS', N'd335d7c4-095e-457c-a8d3-a54d259cf5c0'
	EXEC [lookup].[AddMenuItemType] 7009, N'OPERATIONS_LOAD_RACK_STATIONS', N'99643445-6365-4e86-992e-2c52f04e2289'
	EXEC [lookup].[AddMenuItemType] 7010, N'OPERATIONS_ENTERPRISE_DATA_DATA_TRANSMISSION_EXPORT', N'345a8129-3ecc-40d3-a430-ddfa1cbb6f9c'
	EXEC [lookup].[AddMenuItemType] 7011, N'OPERATIONS_ENTERPRISE_DATA_DATA_TRANSMISSION_IMPORT', N'e712b47e-c06f-40e5-be3a-4dadcb56f8a0'
	EXEC [lookup].[AddMenuItemType] 7012, N'OPERATIONS_QUALITY_TAG_SUMMARY', N'79d60013-9c4c-4b9f-a5d4-eddad88c388d'
	EXEC [lookup].[AddMenuItemType] 7013, N'OPERATIONS_INVENTORY_MANAGEMENT_PHYSICAL_INVENTORY', N'b43841f8-b894-400d-940c-86cc1cd89306'
	EXEC [lookup].[AddMenuItemType] 7014, N'OPERATIONS_LOAD_RACK_ALLOCATIONS', N'96ff3b14-775a-4744-80de-1b4e928a0127'
	EXEC [lookup].[AddMenuItemType] 7015, N'OPERATIONS_LOAD_RACK_MESSAGES', N'c13241aa-2a98-4aee-bf42-37b96f8bc241'
	EXEC [lookup].[AddMenuItemType] 7016, N'OPERATIONS_LOAD_RACK_OPERATIONS', N'3b0a497e-18c1-4bdb-922a-6576b53b25b0'
	EXEC [lookup].[AddMenuItemType] 7017, N'OPERATIONS_MAINTENANCE_MAINTENANCE_LOG', N'00637383-68e5-4da5-afa6-baab10f848ae'
	EXEC [lookup].[AddMenuItemType] 7018, N'OPERATIONS_LOAD_RACK_RACK_STATUS', N'925abd06-f606-4c43-b058-d606dbf442a2'
	EXEC [lookup].[AddMenuItemType] 7019, N'OPERATIONS_SALES_SALES_ORDER_SUMMARY', N'4434a7d1-431c-47a2-8d20-a4a44ed61fd0'
	EXEC [lookup].[AddMenuItemType] 7020, N'OPERATIONS_SCHEDULER_GET_TEST_SCHEDULE', N'5765a754-2068-49ef-8d0f-a1906fa6f78f'
	EXEC [lookup].[AddMenuItemType] 7021, N'OPERATIONS_SCHEDULER_SCHEDULER_SUMMARY', N'65da50a4-1c8d-48e5-815c-c4027ebcf322'
	EXEC [lookup].[AddMenuItemType] 7022, N'OPERATIONS_SYSTEM_LOGS_ALARM_AND_EVENT_LOG', N'63ee2008-d843-4aca-a890-2ce347bd40e5'
	EXEC [lookup].[AddMenuItemType] 7023, N'OPERATIONS_SYSTEM_LOGS_AUDIT_LOG', N'7dc48c1b-c674-4d8b-a444-78271e5c8ac0'
	EXEC [lookup].[AddMenuItemType] 7024, N'OPERATIONS_SYSTEM_LOGS_DATABASE_AUDIT', N'e72ed768-fa11-4cc1-84a2-dba5206b5e7e'
	EXEC [lookup].[AddMenuItemType] 7025, N'OPERATIONS_LOAD_RACK_TANK_ASSIGNMENT', N'25d70ae0-b6c7-45fc-9594-2166299c97d9'
	EXEC [lookup].[AddMenuItemType] 7026, N'OPERATIONS_QUALITY_TESTING_RESULTS', N'504fb651-d15e-464c-b3c4-268fa3ad44bb'
	EXEC [lookup].[AddMenuItemType] 7027, N'OPERATIONS_TRAINING_TRAINING_ASSIGNMENTS', N'502d74fe-15ae-4162-9442-625e715f6f14'
	EXEC [lookup].[AddMenuItemType] 7028, N'OPERATIONS_TRAINING_TRAINING_SUMMARY', N'99a7c7ca-c462-41fe-b852-d982a2530a09'
	EXEC [lookup].[AddMenuItemType] 7029, N'OPERATIONS_PROCUREMENT_SUPPLY_ORDER_SUMMARY', N'fb844c22-bbe5-4ef6-8130-d6462afdfb1d'
	EXEC [lookup].[AddMenuItemType] 7030, N'OPERATIONS_ENTERPRISE_DATA_ONLINE_SYNCHRONIZATION', N'3715855b-a995-4512-b2db-07f9fc1bb696'
	EXEC [lookup].[AddMenuItemType] 7031, N'OPERATIONS_ENTERPRISE_DATA_OFFLINE_SYNCHRONIZATION', N'c3e4366c-a401-458a-b5a7-ebe698bc75ad'
	EXEC [lookup].[AddMenuItemType] 7032, N'OPERATIONS_ENTERPRISE_DATA_OFFLINE_SYNCHRONIZATION_ENTERPRISE', N'e1257fc9-4da5-4ab2-a943-b226bd5fb42f'
	EXEC [lookup].[AddMenuItemType] 7033, N'OPERATIONS_SYNC_LOGS_CONFLICTS_ERRORS', N'ac1050c7-74be-4769-b660-a6adbbe9b7b6'
	EXEC [lookup].[AddMenuItemType] 7034, N'OPERATIONS_MIGRATION_DATA_EXPORT_IMPORT', N'F88784FA-0EAF-43F4-9941-FF2961928A63'
	EXEC [lookup].[AddMenuItemType] 7035, N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_LOG', N'ea19b018-085c-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 7036, N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_OPERATIONS', N'ea19b019-085c-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 7037, N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_FAILED_TRANSACTIONS', N'ea19b01a-085c-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 7038, N'OPERATIONS_SYNC_DASHBOARD', N'ea19b01b-085c-11e8-ab4e-80000bf0a2b6'
	EXEC [lookup].[AddMenuItemType] 7039, N'OPERATIONS_INVENTORY_MANAGEMENT_TAG_VIEWER', N'786EDC54-3A60-4847-ABC3-A25C99650C1D'
	EXEC [lookup].[AddMenuItemType] 7040, N'OPERATIONS_INVENTORY_MANAGEMENT_POINT_EXPLORER', N'228494C4-FAEE-4B01-B87C-6BD6EA834DCD'
	EXEC [lookup].[AddMenuItemType] 7041, N'OPERATIONS_INVENTORY_MANAGEMENT_OPERATE', N'241AB768-8ACE-4560-9A9B-9F684B9F640D'
	EXEC [lookup].[AddMenuItemType] 7042, N'OPERATIONS_INVENTORY_MANAGEMENT_ALARM_SUMMARY', N'FCC6DD44-D318-45BC-94F3-4067EDC656B2'
	EXEC [lookup].[AddMenuItemType] 7043, N'OPERATIONS_INVENTORY_MANAGEMENT_STATISTICS', N'1AF5E328-E1B4-4E20-A090-484CC5384A95'
	EXEC [lookup].[AddMenuItemType] 7044, N'OPERATIONS_INVENTORY_MANAGEMENT_FCEE_MESSAGES', N'894A6A0B-D304-433B-9257-DE7EA9E7694A'

	EXEC [lookup].[AddMenuItemType] 8001, N'QUICK_LINKS_ADD_QUICK_LINK', N'9d736d07-da06-43fa-a0a9-35bde61a51cf'
	EXEC [lookup].[AddMenuItemType] 8002, N'QUICK_LINKS_CONFIG_QUICK_LINKS', N'7e361691-e738-4fb0-9d5f-cd234cfc3fc3'

	EXEC [lookup].[AddMenuItemType] 9001, N'REPORTS_QUERY_WRITER_CREATE_NEW_QUERY', N'a5919775-7560-4433-81c1-7cf640c27f49'
	EXEC [lookup].[AddMenuItemType] 9002, N'REPORTS_UNCATEGORIZED_ALL_REPORTS', N'6826abc9-7f7f-4352-8939-706e07e2954f'
	EXEC [lookup].[AddMenuItemType] 9003, N'REPORTS_QUERY_WRITER_QUERIES', N'47026d58-6b0a-446a-8144-4b9b645d3b23'
	EXEC [lookup].[AddMenuItemType] 9004, N'REPORTS_WEB_LINKS',  N'65F74693-9E67-4B1D-ACBA-FDC7700AFAA6'

	EXEC [lookup].[AddMenuItemType] 10001, N'DATA_ANALYTICS_VIEWER',  N'78574C40-01D7-4297-B9E9-518BA330734F'

	EXEC [lookup].[AddMenuItemType] 11001, N'MAP_MAPS', N'2EA0DE8A-7CF9-4F95-866B-3DC84D336EC4'
	EXEC [lookup].[AddMenuItemType] 11002, N'MAP_CONFIGURATION',  N'99E89F7B-FBD9-4915-8020-37F88DF50EE4'
	EXEC [lookup].[AddMenuItemType] 11003, N'MAP_ASSET_TRACKING_DEVICE_CONFIG',  N'754355B7-1C5B-473B-800D-6C8EED41DF94'
	EXEC [lookup].[AddMenuItemType] 11004, N'ICON_CONFIGURATION', N'6C814D26-D67C-4326-A1B9-A21CC3419618'
	EXEC [lookup].[AddMenuItemType] 12001, N'VRU_THRESHOLD_CONFIG', N'592EC8C6-CB47-4693-8537-A37113642291'
END

IF (SELECT COUNT(*) FROM [lookup].[tblMessageFrequencyType])=0
BEGIN
	INSERT INTO [lookup].[tblMessageFrequencyType] ([MessageFrequencyTypeIndex], [MessageFrequencyTypeCode], [MessageFrequencyTypeName], [MessageFrequencyTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'ALWAYS', N'ALWAYS', N'5decb463-e677-419a-bca2-d2f0b705c2cf', N'6/18/2012 1:01:41 PM +00:00', N'Administrator', N'6/18/2012 1:01:41 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMessageFrequencyType] ([MessageFrequencyTypeIndex], [MessageFrequencyTypeCode], [MessageFrequencyTypeName], [MessageFrequencyTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'ONCE_PER_DAY', N'ONCE PER DAY', N'b1a2d174-6622-4ae1-b7db-5ee3ad710d3a', N'6/18/2012 1:01:41 PM +00:00', N'Administrator', N'6/18/2012 1:01:41 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMessageFrequencyType] ([MessageFrequencyTypeIndex], [MessageFrequencyTypeCode], [MessageFrequencyTypeName], [MessageFrequencyTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'ONCE', N'ONCE', N'7fc82921-81bb-4c46-81c0-f7a34ac6aaca', N'6/18/2012 1:01:41 PM +00:00', N'Administrator', N'6/18/2012 1:01:41 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMessageFrequencyType] ([MessageFrequencyTypeIndex], [MessageFrequencyTypeCode], [MessageFrequencyTypeName], [MessageFrequencyTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'MAX_TYPE', N'MAX TYPE', N'a82a64b3-f420-49bd-bdf7-8ddde04d9130', N'6/18/2012 1:01:41 PM +00:00', N'Administrator', N'6/18/2012 1:01:41 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblMessageLocationType])=0
BEGIN
	INSERT INTO [lookup].[tblMessageLocationType] ([MessageLocationTypeIndex], [MessageLocationTypeCode], [MessageLocationTypeName], [MessageLocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'GATE', N'GATE', N'3c3c7e1c-25ef-4af8-95f7-df7238402716', N'6/18/2012 1:01:45 PM +00:00', N'Administrator', N'6/18/2012 1:01:45 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMessageLocationType] ([MessageLocationTypeIndex], [MessageLocationTypeCode], [MessageLocationTypeName], [MessageLocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'LOAD_RACK', N'LOAD RACK', N'55ccdcdc-bf51-497b-bedf-376826a5c084', N'6/18/2012 1:01:45 PM +00:00', N'Administrator', N'6/18/2012 1:01:45 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMessageLocationType] ([MessageLocationTypeIndex], [MessageLocationTypeCode], [MessageLocationTypeName], [MessageLocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'GATE_AND_LOAD_RACK', N'GATE AND LOAD RACK', N'e80c955a-0757-4c95-83a6-1fe0097030e4', N'6/18/2012 1:01:45 PM +00:00', N'Administrator', N'6/18/2012 1:01:45 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMessageLocationType] ([MessageLocationTypeIndex], [MessageLocationTypeCode], [MessageLocationTypeName], [MessageLocationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'MAX_TYPE', N'MAX TYPE', N'14a22512-14a4-471a-96cf-c8b5d7e49b56', N'6/18/2012 1:01:45 PM +00:00', N'Administrator', N'6/18/2012 1:01:45 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblNumberGroupSizesType])=0
BEGIN
	INSERT INTO [lookup].[tblNumberGroupSizesType] ([NumberGroupSizesTypeIndex], [NumberGroupSizesTypeCode], [NumberGroupSizesTypeName], [NumberGroupSizesTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'ZERO', N'ZERO', N'cad7c55d-4e2d-4dbb-875b-11bd6b034482', N'6/18/2012 1:02:13 PM +00:00', N'Administrator', N'6/18/2012 1:02:13 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblNumberGroupSizesType] ([NumberGroupSizesTypeIndex], [NumberGroupSizesTypeCode], [NumberGroupSizesTypeName], [NumberGroupSizesTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'THREE', N'THREE', N'7e4daaae-20e0-46a1-ada0-fec4c9696dea', N'6/18/2012 1:02:13 PM +00:00', N'Administrator', N'6/18/2012 1:02:13 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblNumberGroupSizesType] ([NumberGroupSizesTypeIndex], [NumberGroupSizesTypeCode], [NumberGroupSizesTypeName], [NumberGroupSizesTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'TWOTHREE', N'TWOTHREE', N'bf08aa75-6acc-4671-9b0c-45176a04c1c5', N'6/18/2012 1:02:13 PM +00:00', N'Administrator', N'6/18/2012 1:02:13 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblPersonnelRole])=0
BEGIN
	INSERT INTO [lookup].[tblPersonnelRole] ([PersonnelRoleIndex], [PersonnelRoleCode], [PersonnelRoleName], [PersonnelRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'LOADER_ROLE', N'Loader Role', N'd1c13440-b729-40df-a683-4831022831f1', N'6/15/2012 9:20:50 AM +00:00', N'Administrator', N'6/15/2012 9:20:50 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPersonnelRole] ([PersonnelRoleIndex], [PersonnelRoleCode], [PersonnelRoleName], [PersonnelRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'SUPERVISOR_ROLE', N'Supervisor Role', N'02508fe3-1388-4d19-882f-6a1446b52787', N'6/15/2012 9:20:50 AM +00:00', N'Administrator', N'6/15/2012 9:20:50 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPersonnelRole] ([PersonnelRoleIndex], [PersonnelRoleCode], [PersonnelRoleName], [PersonnelRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'OFFLOADER_ROLE', N'Offloader Role', N'd64b95da-58bf-4f2e-9132-fdc4f03237d5', N'2/10/2016 6:20:50 PM +00:00', N'Administrator', N'2/10/2016 6:20:50 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPersonnelRole] ([PersonnelRoleIndex], [PersonnelRoleCode], [PersonnelRoleName], [PersonnelRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'MAX_PERSON_ROLE', N'Max Person Role', N'3ff9cb83-b0e8-4c0f-9acd-2fb3cce2240c', N'6/15/2012 9:20:50 AM +00:00', N'Administrator', N'6/15/2012 9:20:50 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblPresetType])=0
BEGIN
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'ACCULOAD2_STD', N'ACCULOAD2 STD', N'df3986f0-9ebe-4ef2-bb66-529f8e8f9f6a', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'ACCULOAD2_SEQ', N'ACCULOAD2 SEQ', N'40bb6482-596a-46fc-8a3b-3ba97b826d3c', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'ACCULOAD2_RBU', N'ACCULOAD2 RBU', N'11e1b7de-091b-4700-ab2a-029d7b81a43c', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'ACCULOAD2_STM', N'ACCULOAD2 STM', N'05a882b5-eb3b-4a48-ab61-c403739f1d6e', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'ACCULOAD2_SQR', N'ACCULOAD2 SQR', N'dc721326-cb62-43ee-a2e1-c9967ddc7315', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'ACCULOAD2_RBM', N'ACCULOAD2 RBM', N'a9ff931f-4981-4a70-a022-9a9a44672f8f', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'ACCULOADIII_S', N'ACCULOADIII S', N'b393e71d-63a0-4b3d-9a35-12a16b4abb1c', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'ACCULOADIII_Q', N'ACCULOADIII Q', N'd2958aa1-671d-4cab-be2f-223d479b34c0', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'MANUAL', N'MANUAL', N'47e1fb56-9948-40f1-bd3e-36ee1a020ed1', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'MICROLOAD_NET', N'MICROLOAD NET', N'961aa7e9-6d6d-4dca-802d-7e7d61cb9109', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'DANLOAD6000', N'DANLOAD6000', N'cec2ae41-9870-4e85-896d-1857d5bb21ea', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'MULTILOAD_II_SMP', N'MULTILOAD II SMP', N'66098330-c183-4f5c-9c8e-d47eeeb58cd9', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'ACCULOADIII_SA', N'ACCULOADIII SA', N'43e19151-a98d-4daa-9401-6500057fbcd6', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'CONTREC1010', N'CONTREC1010', N'fb2a3ae4-3539-4049-bc0c-2bd784bf288f', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'MULTILOAD_II', N'MULTILOAD II', N'2b38954a-e019-40dd-8053-c1690b20e30e', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'CONTREC1010_RA', N'CONTREC1010 RA', N'c74d710a-ad60-41b0-b23a-87f24818b7b7', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPresetType] ([PresetTypeIndex], [PresetTypeCode], [PresetTypeName], [PresetTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'MAX_PRESET_TYPE', N'MAX PRESET TYPE', N'19adc238-0057-4168-bdfc-3f8860ff45ce', N'6/18/2012 1:00:32 PM +00:00', N'Administrator', N'6/18/2012 1:00:32 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblProcessVariableType])=0
BEGIN
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'UNDEFINED_PV', N'UNDEFINED PV', N'f2d05e20-ee61-4ddc-b69d-39bf9914eba6', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'LEVEL_PV', N'LEVEL PV', N'903229ad-a985-4733-a5c2-0f98b732f93b', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'TEMPERATURE_PV', N'TEMPERATURE PV', N'a9678209-1893-48b8-8728-5dbc0b4484f5', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'GROSS_VOLUME_PV', N'GROSS VOLUME PV', N'2e249b25-0bc5-4aaa-9789-b6bbc320009c', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'NET_VOLUME_PV', N'NET VOLUME PV', N'b781597b-033d-4f6a-a606-0b8b9a81dc33', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'DENSITY_PV', N'DENSITY PV', N'b351ec49-efda-4b71-a1e9-3dae4dc2279e', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'STANDARD_DENSITY_PV', N'STANDARD DENSITY PV', N'84f5bba2-8130-416e-b5ed-01a1896c0802', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'MASS_PV', N'MASS PV', N'900dc007-76ee-423b-bb26-8f6bf8b51584', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'SWING_ARM_STATUS_PV', N'SWING ARM STATUS PV', N'516a5f50-1fb1-4a88-84aa-87935d22f9a8', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'CARDREADER_PV', N'CARDREADER PV', N'2d89953c-9594-4e10-8f17-be4c31a1f1f5', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'RESET_CARDREADER_DATA_PV', N'RESET CARDREADER DATA PV', N'e893effd-73fa-4368-883d-55ad3a8a8efc', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'KEYPAD_DATA_PV', N'KEYPAD DATA PV', N'408edaab-8e5b-4033-9cef-44995edfb316', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'RELEASE_KEYPAD_PV', N'RELEASE KEYPAD PV', N'57236246-a24f-49ae-9fb1-52d96e248f51', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'DISPLAY_PV', N'DISPLAY PV', N'8fbbbd8d-8131-4407-8ee3-7157ce498afb', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'PASSWORD_PV', N'PASSWORD PV', N'e8988060-a063-45dd-aea0-90999fa61545', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'PROMPT_TIMEOUT_PV', N'PROMPT TIMEOUT PV', N'ae5c610c-8322-49f6-b4d3-770445dee97d', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'START_PERMISSIVE_PV', N'START PERMISSIVE PV', N'0fc0af31-f1e8-409d-82e1-764067b6de5a', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'COMPLETION_PERMISSIVE_PV', N'COMPLETION PERMISSIVE PV', N'96220484-7624-47c2-b839-985465d97498', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'GATE_CONTROL_PV', N'GATE CONTROL PV', N'05cee94c-eb69-4117-9d89-a8f9b3bc6663', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'LOADARM_PV', N'LOADARM PV', N'0c024ef8-2a27-4c83-a837-d6da01b4b663', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'SITE_PERMISSIVE_PV', N'SITE PERMISSIVE PV', N'9634c98b-8bec-43b2-8f5c-8625ca128a33', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'SITE_ALARM_OUTPUT_PV', N'SITE ALARM OUTPUT PV', N'aff993a5-ced4-4308-a470-a3a68db96aa0', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'SITE_WATCHDOG_OUTPUT_PV', N'SITE WATCHDOG OUTPUT PV', N'ee7e7435-9c07-4562-a6e5-824291a8f2c5', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'VRU_SETPOINT_PV', N'VRU SETPOINT PV', N'fc8d684c-45ce-4932-b2a4-4c325dc1587b', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (24, N'VRU_DEADBAND_PV', N'VRU DEADBAND PV', N'e75a069b-b2f7-4f7a-88a6-5e440fd5c26f', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (25, N'TRANSACTION_DONE_PV', N'TRANSACTION DONE PV', N'6bcb511a-2b88-4ed0-880d-c9e257ba7f9f', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'TRANSACTION_IN_PROGRESS_PV', N'TRANSACTION IN PROGRESS PV', N'f1cac6f3-c27b-4c29-bbb5-1b0fd59e51a0', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (27, N'BATCH_DONE_PV', N'BATCH DONE PV', N'862a2994-0a09-4e95-83ab-c06adb98271c', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (28, N'VCF_PV', N'VCF PV', N'd681ba78-10ba-425a-bb55-7d8ec7d194a8', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (29, N'LOAD_ARM_RELEASED_PV', N'LOAD ARM RELEASED PV', N'd1f242cd-1a07-4464-b2d4-27a81252a2e9', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (30, N'AVAILABLE_GROSS_VOLUME_PV', N'AVAILABLE GROSS VOLUME PV', N'd9daeda9-512a-47ec-b9bb-8c1e921b9545', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (31, N'REMAINING_GROSS_VOLUME_PV', N'REMAINING GROSS VOLUME PV', N'9083fde9-7aac-44cd-bbb3-7584d90ff2c7', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (32, N'FLOWING_PV', N'FLOWING PV', N'7278b241-c2fd-40c4-90f6-14bd1c1a20a9', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (33, N'STATION_PV', N'STATION PV', N'1ba24c8e-2feb-468b-bc6a-3cbe11e85014', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (34, N'WEIGHT_SCALE_PV', N'WEIGHT SCALE PV', N'7e9b3ee3-084d-4ee8-8026-27864da73ad1', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (35, N'CLEAR_LIST_PV', N'CLEAR LIST PV', N'f52d3175-0b98-4e84-a69e-511eaee2389a', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (36, N'DISPLAY_LIST_PV', N'DISPLAY LIST PV', N'dc5f31c8-fb72-479c-ad1b-5eacae9a8ea0', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (37, N'SELECTED_ITEM_PV', N'SELECTED ITEM PV', N'42a7d94c-2068-4b27-8100-7da300defad1', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (38, N'SELECT_ITEM_PV', N'SELECT ITEM PV', N'7fd5b057-1eee-42b3-9dec-8dd5c33c1047', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (39, N'WRITE_ITEM_PV', N'WRITE ITEM PV', N'7d0e271f-e567-464d-9843-349e0f1de82d', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (40, N'BATCH_ABORTED_PV', N'BATCH ABORTED PV', N'4a17a807-30fc-479e-9baf-2d6971c6f18a', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (41, N'POWER_FAIL_OCCURRED_PV', N'POWER FAIL OCCURRED PV', N'ab35ebbf-ae0f-4f5b-80fb-152f217d7224', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (42, N'GET_KEY_PV', N'GET KEY PV', N'1a040ed8-6db5-40bd-8516-b19b068dbebf', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (43, N'PRESETTING_IN_PROGRESS_PV', N'PRESETTING IN PROGRESS PV', N'65d789a4-76de-4bc6-952f-bf8c64625b4b', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (44, N'TANK_OPERATION_PV', N'TANK OPERATION PV', N'ccbc1855-d217-428c-87ef-0ef125f54aae', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (45, N'VAPOR_PRESSURE_PV', N'VAPOR PRESSURE PV', N'7539b9e5-a0dd-4e0a-b72f-0d6f3aee9b25', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (46, N'AVAILABLE_NET_VOLUME_PV', N'AVAILABLE NET VOLUME PV', N'6c27d6d0-1a01-48b4-a562-be0e4cb63f25', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (47, N'REMAINING_NET_VOLUME_PV', N'REMAINING NET VOLUME PV', N'4b514148-673b-4af8-87da-4a6b4582b377', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (48, N'AUTHORIZED_PV', N'AUTHORIZED PV', N'73fbb4da-7c6f-4dc8-8692-dd9331a884b5', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (49, N'BATCH_ENDED_PV', N'BATCH ENDED PV', N'86c4ff4f-249f-498e-a941-b024251e485c', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (50, N'BATCH_IN_PROGRESS_PV', N'BATCH IN PROGRESS PV', N'bff41f22-c9e3-4359-931f-29f6acc5f585', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (51, N'TRANSACTION_END_REQUESTED_PV', N'TRANSACTION END REQUESTED PV', N'7f3628c9-64c4-4781-9ccb-8062c77d1c02', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (52, N'RECIPE_SELECTED_PV', N'RECIPE SELECTED PV', N'94e74265-fbf7-43ae-a37f-2d0229f10395', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (53, N'PRESET_VOLUME_ENTERED_PV', N'PRESET VOLUME ENTERED PV', N'923f8a03-6ff7-4e38-a61b-3434b863471d', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (54, N'BATCH_AUTHORIZED_PV', N'BATCH AUTHORIZED PV', N'49a3f314-76c9-4b5d-88cf-f7e600578398', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (55, N'TRANSACTION_AUTHORIZED_PV', N'TRANSACTION AUTHORIZED PV', N'841349cd-fec2-4c8b-a584-894c2a8046d8', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (56, N'KEY_PRESSED_PV', N'KEY PRESSED PV', N'77955d15-6c77-4f8a-b323-b3df247d2f58', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (57, N'PRIMARY_ALARM_PV', N'PRIMARY ALARM PV', N'720b97d5-0b2c-412b-9beb-ae45e849fd85', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (58, N'TANK_STATUS_PV', N'TANK STATUS PV', N'66f55baf-3244-4d9d-922c-edd57f8672ef', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (59, N'MANUAL_PV', N'MANUAL PV', N'deac5384-1013-4184-b11d-e15d5fa8e2ab', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (60, N'LOAD_ARM_STATE_PV', N'LOAD ARM STATE PV', N'69bdda11-ec2e-45db-ba8a-b60e5973d951', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (61, N'INPUT_DONE_PV', N'INPUT DONE PV', N'2fd1fb99-1ea3-4ed6-ac44-a47b9731c020', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (62, N'RECIPE_PV', N'RECIPE PV', N'c60d8048-ab00-4809-9069-838e11f0dc9e', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (63, N'INPUT_PERMISSIVE_PV', N'INPUT PERMISSIVE PV', N'820a140d-efd3-44f6-a0f2-b7d8cf2a76d3', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (64, N'OUTPUT_PERMISSIVE_PV', N'OUTPUT PERMISSIVE PV', N'698fba28-095a-4725-9b05-6b485f96b7d9', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (65, N'PERMISSIVE_DELAY_PV', N'PERMISSIVE DELAY PV', N'ec18e400-668a-40eb-aa23-2a1be9fcbccd', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (66, N'COMPONENT_METER_FLOW_TOTAL_PV', N'COMPONENT METER FLOW TOTAL PV', N'c204fc86-c76b-4265-92e9-223be3652ed8', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (67, N'BLEND_PERCENTAGE_PV', N'BLEND PERCENTAGE PV', N'b09f76ca-a48a-4e51-a428-681771206176', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (68, N'ADDITIVE_METER_FLOW_TOTAL_PV', N'ADDITIVE METER FLOW TOTAL PV', N'5b0e6a40-f693-4034-becf-80663f115e95', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (69, N'KEYPAD_DATA_PENDING_PV', N'KEYPAD DATA PENDING PV', N'4216340d-9664-4ef7-bf8b-9a50afdf5d56', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (70, N'ALARM_PV', N'ALARM PV', N'd219f999-c2e2-43de-a48c-e60d96ee001f', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (71, N'TERMINATION_KEY', N'TERMINATION KEY', N'4a92e224-71aa-4b87-b607-c8ce9ef72975', N'6/18/2012 1:01:56 PM +00:00', N'Administrator', N'6/18/2012 1:01:56 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (72, N'WATER_LEVEL_PV', N'WATER LEVEL PV', N'43AA8219-26A9-47FC-BC65-55AEC1D740D9', N'9/20/2017', N'Varec', N'9/20/2017', N'Varec')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (73, N'WATER_VOLUME_PV', N'WATER VOLUME PV', N'A7CAFA7F-1E33-4178-8153-951AD991C521', N'9/20/2017', N'Varec', N'9/20/2017', N'Varec')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (74, N'MAX_PV', N'MAX PV', N'9D10A1AF-A807-4285-AFAA-008A87D46A9D', N'9/20/2017', N'Varec', N'9/20/2017', N'Varec')
END

IF (SELECT COUNT(*) FROM [lookup].[tblProductType])=0
BEGIN
	INSERT INTO [lookup].[tblProductType] ([ProductTypeIndex], [ProductTypeCode], [ProductTypeName], [ProductTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'COMPONENT_PRODUCT', N'COMPONENT PRODUCT', N'9200ecae-b78e-42a5-b0db-8dc2732fd9f6', N'6/18/2012 1:02:02 PM +00:00', N'Administrator', N'6/18/2012 1:02:02 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProductType] ([ProductTypeIndex], [ProductTypeCode], [ProductTypeName], [ProductTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'BLEND_PRODUCT', N'BLEND PRODUCT', N'60210a0b-a10d-4dcc-904b-724801c86161', N'6/18/2012 1:02:02 PM +00:00', N'Administrator', N'6/18/2012 1:02:02 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProductType] ([ProductTypeIndex], [ProductTypeCode], [ProductTypeName], [ProductTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'ADDITIVE_PRODUCT', N'ADDITIVE PRODUCT', N'f1b36227-393a-4e5e-b3c4-6e1bfb36fbf5', N'6/18/2012 1:02:02 PM +00:00', N'Administrator', N'6/18/2012 1:02:02 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProductType] ([ProductTypeIndex], [ProductTypeCode], [ProductTypeName], [ProductTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'ADDITIZED_PRODUCT', N'ADDITIZED PRODUCT', N'e2f04cf6-f466-4cc0-9081-db272a8873ca', N'6/18/2012 1:02:02 PM +00:00', N'Administrator', N'6/18/2012 1:02:02 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblProductType] ([ProductTypeIndex], [ProductTypeCode], [ProductTypeName], [ProductTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'MAX_PRODUCT', N'MAX PRODUCT', N'2b0e458a-af2b-471a-8c2d-064537001935', N'6/18/2012 1:02:02 PM +00:00', N'Administrator', N'6/18/2012 1:02:02 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblQualificationType])=0
BEGIN
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'COMPANY_CERTIFICATE_AND_PERMIT', N'COMPANY CERTIFICATE AND PERMIT', N'8cd53ece-002e-45f4-8005-db86404fe9c8', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'EQUIPMENT_TEST_AND_INSPECTION', N'EQUIPMENT TEST AND INSPECTION', N'021842de-fd27-4448-b001-bc159783f815', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'EQUIPMENT_TAG_AND_LICENSE', N'EQUIPMENT TAG AND LICENSE', N'844e00dd-94c0-4ed1-b58a-72bef9b93c4b', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'PERSON_QUALIFICATION', N'PERSON QUALIFICATION', N'a836f095-22b7-42d7-a09b-a126afb3f732', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'PERSON_LICENSE', N'PERSON LICENSE', N'dba6e42c-3ade-40c8-b3b3-cc57db9f8cdf', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'PERSON_TRAINING', N'PERSON TRAINING', N'e1af95ea-5564-4704-b46e-aafa9c44af57', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQualificationType] ([QualificationTypeIndex], [QualificationTypeCode], [QualificationTypeName], [QualificationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'MAX_QUALIFICATION_TYPE', N'MAX QUALIFICATION TYPE', N'c256b66f-f6a7-4cd0-8790-7b4158c414ac', N'6/18/2012 1:02:08 PM +00:00', N'Administrator', N'6/18/2012 1:02:08 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblQuantityDisplay])=0
BEGIN
	INSERT INTO [lookup].[tblQuantityDisplay] ([QuantityDisplayIndex], [QuantityDisplayCode], [QuantityDisplayName], [QuantityDisplayGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'GROSS_AND_NET', N'GROSS AND NET', N'088f3b86-fc62-432d-9895-19f438d9b26b', N'6/18/2012 1:03:06 PM +00:00', N'Administrator', N'6/18/2012 1:03:06 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQuantityDisplay] ([QuantityDisplayIndex], [QuantityDisplayCode], [QuantityDisplayName], [QuantityDisplayGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'GROSS', N'GROSS', N'221aa92b-ad32-4961-b2b1-8804f253da48', N'6/18/2012 1:03:06 PM +00:00', N'Administrator', N'6/18/2012 1:03:06 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQuantityDisplay] ([QuantityDisplayIndex], [QuantityDisplayCode], [QuantityDisplayName], [QuantityDisplayGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'NET', N'NET', N'cc71641c-f1c1-497d-a175-02f33b9376ac', N'6/18/2012 1:03:06 PM +00:00', N'Administrator', N'6/18/2012 1:03:06 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQuantityDisplay] ([QuantityDisplayIndex], [QuantityDisplayCode], [QuantityDisplayName], [QuantityDisplayGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'MASS', N'MASS', N'776f06cf-64b4-47fe-8b53-40281831b55c', N'6/18/2012 1:03:06 PM +00:00', N'Administrator', N'6/18/2012 1:03:06 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblQuantityDisplay] ([QuantityDisplayIndex], [QuantityDisplayCode], [QuantityDisplayName], [QuantityDisplayGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'PACKAGE', N'PACKAGE', N'9b38d247-2995-490b-907a-0cf051990e73', N'6/18/2012 1:03:06 PM +00:00', N'Administrator', N'6/18/2012 1:03:06 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblResetMethod])=0
BEGIN
	INSERT INTO [lookup].[tblResetMethod] ([ResetMethodIndex], [ResetMethodCode], [ResetMethodName], [ResetMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'REPEAT_METHOD', N'REPEAT METHOD', N'a6d5d6cf-93c1-4299-bd67-83fe37e4d757', N'6/18/2012 1:02:50 PM +00:00', N'Administrator', N'6/18/2012 1:02:50 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetMethod] ([ResetMethodIndex], [ResetMethodCode], [ResetMethodName], [ResetMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'BALANCE_FORWARD_METHOD', N'BALANCE FORWARD METHOD', N'035e4931-4713-40a2-913f-149df805451d', N'6/18/2012 1:02:50 PM +00:00', N'Administrator', N'6/18/2012 1:02:50 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetMethod] ([ResetMethodIndex], [ResetMethodCode], [ResetMethodName], [ResetMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'NEXT_LIMIT_METHOD', N'NEXT LIMIT METHOD', N'789c352b-8083-4635-a233-d576f8e94f11', N'6/18/2012 1:02:50 PM +00:00', N'Administrator', N'6/18/2012 1:02:50 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetMethod] ([ResetMethodIndex], [ResetMethodCode], [ResetMethodName], [ResetMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'NEXT_PLUS_BALANCE_FORWARD_METHOD', N'NEXT PLUS BALANCE FORWARD METHOD', N'8c909f1b-b529-41ba-b250-9942beacf31a', N'6/18/2012 1:02:50 PM +00:00', N'Administrator', N'6/18/2012 1:02:50 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetMethod] ([ResetMethodIndex], [ResetMethodCode], [ResetMethodName], [ResetMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'BOOK_MINUS_UNAVAILABLE_METHOD', N'BOOK MINUS UNAVAILABLE METHOD', N'9e2a501c-4110-4dbb-a02e-c2006d8b4654', N'6/18/2012 1:02:50 PM +00:00', N'Administrator', N'6/18/2012 1:02:50 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetMethod] ([ResetMethodIndex], [ResetMethodCode], [ResetMethodName], [ResetMethodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'MAX_ALLOCATION_METHOD', N'MAX ALLOCATION METHOD', N'1dc223f2-5bce-4fbf-a610-67d198b07f97', N'6/18/2012 1:02:50 PM +00:00', N'Administrator', N'6/18/2012 1:02:50 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblResetPeriod])=0
BEGIN
	INSERT INTO [lookup].[tblResetPeriod] ([ResetPeriodIndex], [ResetPeriodCode], [ResetPeriodName], [ResetPeriodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'DAY_RESET_PERIOD', N'DAY RESET PERIOD', N'7754f18e-0b77-4573-971a-4493fc7d268e', N'6/18/2012 1:02:54 PM +00:00', N'Administrator', N'6/18/2012 1:02:54 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetPeriod] ([ResetPeriodIndex], [ResetPeriodCode], [ResetPeriodName], [ResetPeriodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'WEEK_RESET_PERIOD', N'WEEK RESET PERIOD', N'8db3d806-92e5-4d59-a203-d5655ff9f5be', N'6/18/2012 1:02:54 PM +00:00', N'Administrator', N'6/18/2012 1:02:54 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetPeriod] ([ResetPeriodIndex], [ResetPeriodCode], [ResetPeriodName], [ResetPeriodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'MONTH_RESET_PERIOD', N'MONTH RESET PERIOD', N'9de4e66f-143c-4217-a3f1-acc2f3d9514e', N'6/18/2012 1:02:54 PM +00:00', N'Administrator', N'6/18/2012 1:02:54 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetPeriod] ([ResetPeriodIndex], [ResetPeriodCode], [ResetPeriodName], [ResetPeriodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'YEAR_RESET_PERIOD', N'YEAR RESET PERIOD', N'f5f49dc7-2e43-4024-b986-ca0f87c22d6a', N'6/18/2012 1:02:54 PM +00:00', N'Administrator', N'6/18/2012 1:02:54 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblResetPeriod] ([ResetPeriodIndex], [ResetPeriodCode], [ResetPeriodName], [ResetPeriodGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'MAX_RESET_PERIOD', N'MAX RESET PERIOD', N'1e5dce6b-b2d4-4c89-bf20-34257845dc63', N'6/18/2012 1:02:54 PM +00:00', N'Administrator', N'6/18/2012 1:02:54 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblScheduleType])=0
BEGIN
	INSERT INTO [lookup].[tblScheduleType] ([ScheduleTypeIndex], [ScheduleTypeCode], [ScheduleTypeName], [ScheduleTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'TERMINAL_OPERATIONS_TYPE', N'TERMINAL OPERATIONS TYPE', N'd32109a3-2bf1-4015-bdc5-f6b49e18a811', N'6/18/2012 9:05:46 AM +00:00', N'Administrator', N'6/18/2012 9:05:46 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblScheduleType] ([ScheduleTypeIndex], [ScheduleTypeCode], [ScheduleTypeName], [ScheduleTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'COMPANY_ACCESS_TYPE', N'COMPANY ACCESS TYPE', N'5a080adc-0f96-4807-8753-28cfcd43009d', N'6/18/2012 9:05:46 AM +00:00', N'Administrator', N'6/18/2012 9:05:46 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblScheduleType] ([ScheduleTypeIndex], [ScheduleTypeCode], [ScheduleTypeName], [ScheduleTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'HOLIDAY_TYPE', N'HOLIDAY TYPE', N'9e738e87-f654-4b05-94d0-a448eb59cda4', N'6/18/2012 9:05:46 AM +00:00', N'Administrator', N'6/18/2012 9:05:46 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblScheduleType] ([ScheduleTypeIndex], [ScheduleTypeCode], [ScheduleTypeName], [ScheduleTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'PERSON_ACCESS_TYPE', N'PERSON ACCESS TYPE', N'6c60e80a-927e-4186-890d-6b351016d54e', N'6/18/2012 9:05:46 AM +00:00', N'Administrator', N'6/18/2012 9:05:46 AM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblScheduleType] ([ScheduleTypeIndex], [ScheduleTypeCode], [ScheduleTypeName], [ScheduleTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'MAX_SCHEDULE_TYPE', N'MAX SCHEDULE TYPE', N'8354f2e0-79a0-4cb7-9d74-8c531808ef07', N'6/18/2012 9:05:46 AM +00:00', N'Administrator', N'6/18/2012 9:05:46 AM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblServiceType])=0
BEGIN
	INSERT INTO [lookup].[tblServiceType] ([ServiceTypeIndex], [ServiceTypeCode], [ServiceTypeName], [ServiceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Aviation', N'Aviation', N'25cce146-4732-4d10-98f6-c8e4f487a7e1', N'6/18/2012 1:01:49 PM +00:00', N'Administrator', N'6/18/2012 1:01:49 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblServiceType] ([ServiceTypeIndex], [ServiceTypeCode], [ServiceTypeName], [ServiceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Navy', N'Navy', N'6b7283bd-afb2-41b6-93f4-86cb0c7c4859', N'6/18/2012 1:01:49 PM +00:00', N'Administrator', N'6/18/2012 1:01:49 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblServiceType] ([ServiceTypeIndex], [ServiceTypeCode], [ServiceTypeName], [ServiceTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'Army', N'Army', N'c47547c4-f856-4099-b8cb-51914bce8745', N'6/18/2012 1:01:49 PM +00:00', N'Administrator', N'6/18/2012 1:01:49 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSRMAdaptorFilterType])=0
BEGIN
	INSERT INTO [lookup].[tblSRMAdaptorFilterType] ([SRMAdaptorFilterTypeGuid], [SRMAdaptorFilterType], [SRMAdaptorFilterTypeCode], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1eef7a68-f486-4d23-b209-07f88a747464', N'Gate', 1, N'10/23/2012 9:43:16 AM -04:00', N'Administrator', N'10/23/2012 9:43:16 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblSRMAdaptorFilterType] ([SRMAdaptorFilterTypeGuid], [SRMAdaptorFilterType], [SRMAdaptorFilterTypeCode], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2f2682bf-1c04-45ed-8635-d1e4d43200c9', N'Flight Type', 2, N'10/23/2012 9:43:16 AM -04:00', N'Administrator', N'10/23/2012 9:43:16 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblSRMAdaptorFilterType] ([SRMAdaptorFilterTypeGuid], [SRMAdaptorFilterType], [SRMAdaptorFilterTypeCode], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'58534f40-861c-4e81-94a1-250c707580e8', N'Event Code', 3, N'10/23/2012 9:43:16 AM -04:00', N'Administrator', N'10/23/2012 9:43:16 AM -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblStationType])=0
BEGIN
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'ENTRY_GATE', N'ENTRY GATE', N'8e81b60c-86b9-4e6d-acb3-c827ab40ceb1', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'LOAD_RACK', N'LOAD RACK', N'ea10d035-c58b-48c1-babd-c437f704096a', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'EXIT_GATE', N'EXIT GATE', N'89f965bc-6aea-4af0-a59d-dd5e7fbe19dd', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'WEIGHT_SCALE', N'WEIGHT SCALE', N'894384fc-13ba-40e4-b56e-d561138fb82d', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'BOL', N'BOL', N'67763fcb-976f-428d-aadb-8c6052087311', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'PRELOAD', N'PRELOAD', N'ceab6327-525e-4f9c-acfe-d212cc072c65', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'SIGNATURE', N'SIGNATURE', N'e8a3d9d6-32cc-41c9-a09f-61bdd39e5e5e', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'METER', N'METER', N'2f09e45b-9630-4952-8ee0-561325734941', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'OFF_LOADING', N'OFF LOADING', N'1d20cce6-ff1a-4209-a960-3b550be4df5c', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'MANUAL_BOL', N'MANUAL BOL STATION', N'274ef7ed-c0f6-44e3-8c90-9c6040ae2bf4', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'MAX_STATION_TYPE', N'MAX STATION TYPE', N'85ea15a0-2f4a-413d-8b64-14e260afbc94', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncConflictResolutionStatus])=0
BEGIN
	INSERT INTO [lookup].[tblSyncConflictResolutionStatus] ([SyncConflictResolutionStatusIndex], [SyncConflictResolutionStatusGuid], [StatusCode], [StatusName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SequenceOrder]) VALUES (0, N'b6a7bb20-34fc-406c-9933-d701721b7927', N'PENDING', N'Pending', N'Conflict pending manual user intervention or subsequent synchronization attempt to automatically resolve.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', 2)
	INSERT INTO [lookup].[tblSyncConflictResolutionStatus] ([SyncConflictResolutionStatusIndex], [SyncConflictResolutionStatusGuid], [StatusCode], [StatusName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SequenceOrder]) VALUES (1, N'557d0815-c228-48c5-a45e-0cfca667788f', N'RESOLVED', N'User Resolved', N'Conflict marked as resolved by user.  Waiting for subsequent synchronization attempt to determine if the conflict can be cleared.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger',3 )
	INSERT INTO [lookup].[tblSyncConflictResolutionStatus] ([SyncConflictResolutionStatusIndex], [SyncConflictResolutionStatusGuid], [StatusCode], [StatusName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SequenceOrder]) VALUES (2, N'304e5e6d-350a-4309-9750-fa69765bd1fa', N'CLEARED', N'Cleared', N'Conflict has been resolved.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', 4)
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncConflictType])=0
BEGIN
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'UNKNOWN', N'Unknown', N'3905cdcf-95c4-40c5-8c21-7e305d5ef67f', N'Unexpected conflict.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'ERROR', N'Error', N'b1bf1cb2-a231-4f66-8172-2baf33c32aa0', N'The client or server data store threw an exception while applying a change.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'CLIENTUPD_SERVERUPD', N'Client/Server Update', N'ceec7fc6-7171-418f-96b0-1341233e4104', N'The client and the server updated the same record.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'CLIENTUPD_SERVERDEL', N'Client Update/Server Delete', N'726348fc-48bf-47ee-a2c4-b4cc1b205483', N'The server deleted a record that the client was trying to update.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'CLIENTDEL_SERVERUPD', N'Client Delete/Server Update', N'5c473d17-c4a8-4aa9-aef6-fa7f85af74a1', N'The client deleted a record that the server was trying to update.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'CLIENTINS_SERVERINS', N'Client/Server Insert', N'68b6cc97-543e-4c01-940a-a63648d58aa0', N'The client and the server both inserted a row that has the same primary key value.  This can also be caused by a unique key constraint violation (ie: duplicate ID value).', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncConflictType] ([SyncConflictTypeIndex], [SyncConflictTypeCode], [SyncConflictTypeName], [SyncConflictTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'CLIENTSERVER_DUPLICATEID', N'Client/Server Duplicate ID', N'429f9ffe-d5b6-4086-9202-f3272fdf2bd7', N'Duplicate Record ID within the Entity Assignment Scope for Site/Site Group.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncRequestType])=0
BEGIN
	INSERT INTO [lookup].[tblSyncRequestType] ([SyncRequestTypeIndex], [SyncRequestTypeCode], [SyncRequestTypeName], [SyncRequestTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'MANUAL', N'Manual Request', N'93992a17-e7a9-495a-b5ee-d9497c1d37e2', N'User initiated manual synchronization request.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncRequestType] ([SyncRequestTypeIndex], [SyncRequestTypeCode], [SyncRequestTypeName], [SyncRequestTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'PERIODIC', N'Periodic Request', N'f3b316e9-e922-4087-ac95-43703f6aa7bf', N'Automated periodic synchronization request.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncRequestType] ([SyncRequestTypeIndex], [SyncRequestTypeCode], [SyncRequestTypeName], [SyncRequestTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'SCHEDULED', N'Scheduled Request', N'0a204380-1912-418b-853e-f21873d1d17a', N'Automated schedule-based synchronization request.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncRequestType] ([SyncRequestTypeIndex], [SyncRequestTypeCode], [SyncRequestTypeName], [SyncRequestTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'RESYNC', N'Resync Request', N'80393fbc-7c51-451d-83d6-0279f2b227dd', N'Automatic resynchronization request due to a recent schema upgrade.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncRequestType] ([SyncRequestTypeIndex], [SyncRequestTypeCode], [SyncRequestTypeName], [SyncRequestTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'INIT', N'Initialization Request', N'b4128602-ecd3-414b-b0ae-8daeb6557d3c', N'Initial synchronization of data with the Enterprise.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncSessionState])=0
BEGIN
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'INIT', N'Initializing', N'74fea296-776a-45b2-8c03-25995f37a42f', N'Currently initializing new synchronization session.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'CONN', N'Connecting', N'95bc1c64-2781-4913-a0b3-16732ba4f64c', N'Connecting to remote synchronization node.', N'4/3/2013 3:26:11 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:26:11 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'SYSAUTH', N'Authenticating Service', N'b734502d-c9bd-4ded-ab93-6d2cf9935d30', N'Attempting to authenticate to the remote synchronization service.', N'4/3/2013 3:28:21 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:28:21 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'FMAUTH', N'Authenticating FuelsManager Session', N'b03d3119-39ec-4706-9104-2194fe8732f4', N'Attempting to authenticate with FuelsManager in order to create a FuelsManager session.', N'4/3/2013 3:28:53 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:28:53 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'QUEUED', N'Pending Synchronization', N'004f6a7e-d3db-47b1-92a2-79ace4ef27b1', N'Synchronization is still processing other records.', N'4/3/2013 3:31:44 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:31:44 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'PROCESSINSUPD', N'Synchronizing Inserts and Updates', N'70497aa9-d3af-4a64-946c-de3cc88c3a0c', N'Currently synchronization insert and update changes with a remote node.', N'4/3/2013 3:31:44 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:31:44 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'PROCESSDEL', N'Synchronizing Deletions', N'dfd4153a-259f-4a28-948b-9b94b6324f91', N'Currently synchronizing delete changes with a remote node.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'DOWNLOADBATCHFILE', N'Downloading Change Batch from Server', N'cfd8d223-f632-4a27-8130-446687dbdb75', N'Downloading batch file containing remote changes for the local client.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'UPLOADBATCHFILE', N'Uploading Change Batch to Server', N'8583623f-98ed-4f75-b932-2aedc79dc062', N'Uploading batch file containing local changes to the remote server.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'GETCLIENTCHANGES', N'Selecting Client Changes', N'a7e7c8e2-a02e-42b1-a189-694981e9fca7', N'Selecting changes on the local client for the remote server.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'APPLYCHANGESTOCLIENT', N'Applying Server Changes on Client', N'3080ad48-29af-49c6-8043-b8795f5fa154', N'Applying remote server changes to the local client.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'GETSERVERCHANGES', N'Selecting Server Changes', N'212a6281-d99b-40ed-9100-7f998b057041', N'Selecting changes on the remote server for the local client.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'APPLYCHANGESTOSERVER', N'Applying Client Changes on Server', N'8ed1352e-fcda-4bbd-8dac-a09803d2e685', N'Applying local client changes to the remote server.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'SYNCED', N'Synchronized', N'52623850-3d60-40d4-bb34-e77008e600f8', N'Changes have been synchronized.', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:35:24 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'CONFLICTS', N'Processing Conflict', N'426c3a5a-a653-4f26-ae21-1a089e8d6859', N'Processing synchronization conficts.', N'4/3/2013 3:36:27 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:36:27 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'POSTSYNC', N'Post Synchronization', N'd7bd4cb9-9684-4bac-8056-0933e1b701f5', N'Executing post-synchronization logic.', N'4/3/2013 3:37:25 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:37:25 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'DISCONN', N'Disconnecting', N'c05dbc88-c57a-4ea5-9a6d-ccf83e10421d', N'Disconnecting from remote synchronization node.', N'4/3/2013 3:37:51 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:37:51 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'CLOSE', N'Closing Session', N'd6fac798-8541-4cff-9b6a-f1d04d30d289', N'Closing synchronization session.', N'4/3/2013 3:38:16 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:38:16 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionState] ([SyncSessionStateIndex], [SyncSessionStateCode], [SyncSessionStateName], [SyncSessionStateGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'END', N'Session Ended', N'2d5fcfa6-0cc2-449c-bd61-99dae68e9614', N'Synchronization session is no longer active.', N'4/3/2013 3:45:38 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:45:38 PM -04:00', N'SAIC-US-EAST\petersger')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncSessionStatus])=0
BEGIN
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'NEW', N'New', N'7ebd1ded-daa2-41f4-b0e5-4b71ecc5b26a', N'Newly created synchronization session.', N'4/3/2013 3:40:06 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:40:06 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'STARTED', N'Started', N'd5da9dad-4b56-40f9-8cb4-811fef3e181e', N'Synchronization session has started.', N'4/3/2013 3:40:50 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:40:50 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'COMPOK', N'Completed', N'86644b36-7444-4c56-96b8-b54a3414027e', N'Session completed successfully.', N'4/3/2013 3:46:37 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:46:37 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'COMPCON', N'Completed w/ Conflicts', N'5bef3b4c-3f6e-41d5-a442-8d3a6b2138d5', N'Session completed succesfully but conflicts were encountered.', N'4/3/2013 3:47:02 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:47:02 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'FAILED', N'Failed', N'86e19691-1ae0-4cfb-909d-161363fe129f', N'One or more errors prevented synchronization from successfully completing.', N'4/3/2013 3:48:01 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:48:01 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'USERSTOP', N'Stopped (User)', N'3c52643a-1527-4445-8f8c-7bb0dc47b674', N'Synchronization session was stopped due to a user stop request.', N'4/3/2013 3:50:15 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:50:15 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncSessionStatus] ([SyncSessionStatusIndex], [SyncSessionStatusCode], [SyncSessionStatusName], [SyncSessionStatusGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'SYSSTOP', N'Stopped (System)', N'7d49a384-28f3-48fe-a48b-daaff68b3445', N'Synchronization session was stopped due to a system request.  (System Shutdown, Service Stopped, etc)', N'4/3/2013 3:50:23 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:50:23 PM -04:00', N'SAIC-US-EAST\petersger')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncTransferType])=0
BEGIN
	INSERT INTO [lookup].[tblSyncTransferType] ([SyncTransferTypeIndex], [SyncTransferTypeCode], [SyncTransferTypeName], [SyncTransferTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'ONLINE', N'Online', N'a5204fc0-7ec4-4c7f-af0d-aa939aa63d5b', N'Online synchronization using Enterprise Web Services.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
	INSERT INTO [lookup].[tblSyncTransferType] ([SyncTransferTypeIndex], [SyncTransferTypeCode], [SyncTransferTypeName], [SyncTransferTypeGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'OFFLINE', N'Offline', N'77fe66c6-f157-4d26-8a58-dde48d0ea6eb', N'Offline synchronization using Synchronization Files.', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger', N'4/3/2013 3:23:58 PM -04:00', N'SAIC-US-EAST\petersger')
END

IF (SELECT COUNT(*) FROM [lookup].[tblSyncControllerStep])=0
BEGIN
	INSERT INTO [lookup].[tblSyncControllerStep] ([SyncControllerStepIndex], [SyncControllerStepCode], [SyncControllerStepName], [SyncControllerStepGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'PROCESS_ALL', N'ALL', N'F92612A2-2025-4ED5-88E8-85628CD48E5E',N'Synchronization process is currently processing All.',  N'2/21/2017 00:00:01 AM -04:00', N'Administrator', N'2/21/2017 00:00:01 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblSyncControllerStep] ([SyncControllerStepIndex], [SyncControllerStepCode], [SyncControllerStepName], [SyncControllerStepGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'PROCESS_INSERT_UPDATE', N'Inserts/Updates', N'8D94FFD3-D8B2-418A-8946-BB395771BCB2',N'Synchronization process is currently processing Inserts/Updates.',  N'2/21/2017 00:00:01 AM -04:00', N'Administrator', N'2/21/2017 00:00:01 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblSyncControllerStep] ([SyncControllerStepIndex], [SyncControllerStepCode], [SyncControllerStepName], [SyncControllerStepGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'PROCESS_INSERT_UPDATE_CONFLICT', N'Insert/Update Conflicts', N'AC71C690-C509-4528-B285-0D5DACB7FD41',N'Synchronization process is currently processing Insert/Update Conflicts.',  N'2/21/2017 00:00:01 AM -04:00', N'Administrator', N'2/21/2017 00:00:01 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblSyncControllerStep] ([SyncControllerStepIndex], [SyncControllerStepCode], [SyncControllerStepName], [SyncControllerStepGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'PROCESS_DELETE', N'Deletes', N'474C78CE-457E-4856-BBBF-0C71089A0145', N'Synchronization process is currently processing Deletes.', N'2/21/2017 00:00:01 AM -04:00', N'Administrator', N'2/21/2017 00:00:01 AM -04:00', N'Administrator')
	INSERT INTO [lookup].[tblSyncControllerStep] ([SyncControllerStepIndex], [SyncControllerStepCode], [SyncControllerStepName], [SyncControllerStepGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'PROCESS_DELETE_CONFLICT', N'Delete Conflicts',N'6ED1DC7B-ECC0-4AD5-9F89-3090251EAE23', N'Synchronization process is currently processing Delete Conflicts.',  N'2/21/2017 00:00:01 AM -04:00', N'Administrator', N'2/21/2017 00:00:01 AM -04:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblTestSetStatus])=0
BEGIN
	INSERT INTO [lookup].[tblTestSetStatus] ([TestSetStatusIndex], [TestSetStatusCode], [TestSetStatusName], [TestSetStatusGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'Pending', N'Pending', N'44b47e7a-7438-44e6-9af2-6f14fc5d6cce', N'6/18/2012 1:03:20 PM +00:00', N'Administrator', N'6/18/2012 1:03:20 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTestSetStatus] ([TestSetStatusIndex], [TestSetStatusCode], [TestSetStatusName], [TestSetStatusGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Passed', N'Passed', N'e1c7f822-55f3-4011-8eed-c2d2ddc0cc59', N'6/18/2012 1:03:20 PM +00:00', N'Administrator', N'6/18/2012 1:03:20 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTestSetStatus] ([TestSetStatusIndex], [TestSetStatusCode], [TestSetStatusName], [TestSetStatusGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Failed', N'Failed', N'37546786-707b-43ea-93b5-0e1553d58323', N'6/18/2012 1:03:20 PM +00:00', N'Administrator', N'6/18/2012 1:03:20 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblTransactionFieldType])=0
BEGIN
	INSERT INTO [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex], [TransactionFieldTypeCode], [TransactionFieldTypeName], [TransactionFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'TRANSACTION', N'TRANSACTION', N'6541419e-8f20-4896-9579-d042cce22b89', N'6/18/2012 1:03:36 PM +00:00', N'Administrator', N'6/18/2012 1:03:36 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex], [TransactionFieldTypeCode], [TransactionFieldTypeName], [TransactionFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'LINE_ITEM', N'LINE ITEM', N'1d8aede7-0c7d-48a8-9b17-68d58901db22', N'6/18/2012 1:03:36 PM +00:00', N'Administrator', N'6/18/2012 1:03:36 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex], [TransactionFieldTypeCode], [TransactionFieldTypeName], [TransactionFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'WEIGHT_READING', N'WEIGHT READING', N'2ae54209-99fc-4de8-99a6-d5fe59d6046d', N'6/18/2012 1:03:36 PM +00:00', N'Administrator', N'6/18/2012 1:03:36 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex], [TransactionFieldTypeCode], [TransactionFieldTypeName], [TransactionFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'NOTE', N'NOTE', N'e3766e70-3490-4c4b-81f9-b2d510269e0b', N'6/18/2012 1:03:36 PM +00:00', N'Administrator', N'6/18/2012 1:03:36 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex], [TransactionFieldTypeCode], [TransactionFieldTypeName], [TransactionFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'TRANSPORT_INFO', N'TRANSPORT INFO', N'ce510efd-6f27-4357-99ec-ec78be126679', N'6/18/2012 1:03:36 PM +00:00', N'Administrator', N'6/18/2012 1:03:36 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex], [TransactionFieldTypeCode], [TransactionFieldTypeName], [TransactionFieldTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'TRANSACTION_FIELD_TYPE_MAX', N'TRANSACTION FIELD TYPE MAX', N'4b09c489-b538-4651-8089-c769f29509ac', N'6/18/2012 1:03:36 PM +00:00', N'Administrator', N'6/18/2012 1:03:36 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblTransactionQuality])=0
BEGIN
	INSERT INTO [lookup].[tblTransactionQuality] ([TransactionQualityIndex], [TransactionQualityCode], [TransactionQualityName], [TransactionQualityGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'Quarantined', N'Quarantined', N'600b220f-486b-4e8f-a825-7b7049b021d6', N'6/18/2012 1:03:49 PM +00:00', N'Administrator', N'6/18/2012 1:03:49 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionQuality] ([TransactionQualityIndex], [TransactionQualityCode], [TransactionQualityName], [TransactionQualityGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'Usable', N'Usable', N'dcd1daa7-b87a-4ea9-92fb-715ac3154086', N'6/18/2012 1:03:49 PM +00:00', N'Administrator', N'6/18/2012 1:03:49 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionQuality] ([TransactionQualityIndex], [TransactionQualityCode], [TransactionQualityName], [TransactionQualityGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'Unusable', N'Unusable', N'c6002ab2-753b-4026-8176-1b01d4362ba7', N'6/18/2012 1:03:49 PM +00:00', N'Administrator', N'6/18/2012 1:03:49 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblTransactionTypes])=0
BEGIN
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'TransactionType_None', N'TransactionType None', N'ec069968-8987-4532-8be0-d39088629bbb', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'T1_PrimaryAdjustment', N'T1 PrimaryAdjustment', N'7e9a6881-684f-4b88-86b9-f1a15e641ead', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'T2_SecondaryAdjustment', N'T2 SecondaryAdjustment', N'b8094240-5783-4cec-b4b5-4e383331c983', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'T3_PrimaryDefuel', N'T3 PrimaryDefuel', N'e7ee4c45-f675-4dda-8394-e8643db5911b', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'T4_SecondaryDefuel', N'T4 SecondaryDefuel', N'554c7920-3371-47e7-8ae4-bc422e5c2982', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'T5_PrimaryDisbursement', N'T5 PrimaryDisbursement', N'8075eff9-d612-4ed8-8b54-1ed4560bdf4b', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'T6_SecondaryDisbursement', N'T6 SecondaryDisbursement', N'6e73d978-e05c-485f-816d-2355dcc684a2', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'T7_FillStand', N'T7 FillStand', N'96c638ec-3b4c-4311-afb4-ede602726c04', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'T8_Receipt', N'T8 Receipt', N'70c4a797-d2d2-4f1c-9f43-97892eb575ba', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'T9_Request', N'T9 Request', N'fc147415-2288-4211-bec4-c11c83d57649', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'T10_Unload', N'T10 Unload', N'8270793f-e4ff-4b8a-ba4e-0367af90d04d', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'T11_ConsumerTransfer', N'T11 ConsumerTransfer', N'7717eb84-fb73-43f9-bcc5-edf440055581', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'T12_InventoryNotAffected', N'T12 InventoryNotAffected', N'8bf398cb-cd5a-4aef-9274-cb1f23ac3876', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (13, N'T13_OwnerTransfer', N'T13 OwnerTransfer', N'bc1ef511-45f8-40f3-a1b6-4b6204747bab', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (14, N'T14_PhysicalInventory', N'T14 PhysicalInventory', N'1300a89e-0d0e-441b-92b8-b487244a6bc7', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (15, N'T15_PrimaryRegrade', N'T15 PrimaryRegrade', N'9982db43-feab-43e9-ae67-318ab4b9c63b', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (16, N'T16_SecondaryRegrade', N'T16 SecondaryRegrade', N'80323283-6c74-456d-bcfe-be09f803e8d1', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (17, N'T17_Order', N'T17 Order', N'f14aefd2-03f3-48d8-bb52-374c3fbfa6b7', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (18, N'T18_SupplyOrder', N'T18 SupplyOrder', N'e0c1f9b7-9653-4217-b0d5-ae1c8936c3ae', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (19, N'T19_EndOfDay', N'T19 EndOfDay', N'ba58b95e-8c98-42ea-878d-45c7e782bb49', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (20, N'T20_EndOfMonth', N'T20 EndOfMonth', N'fc174786-2dc4-417c-81c4-f7b679c0765d', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (21, N'T21_AccountPayableInvoice', N'T21 AccountPayableInvoice', N'8c982b41-120c-4609-8e7d-c781c8cecdb7', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (22, N'T22_AccountReceivableInvoice', N'T22 AccountReceivableInvoice', N'686ffddd-9654-44f9-b1cf-e9a8762ef0f6', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (23, N'T23_StorageTransfer', N'T23 StorageTransfer', N'92eb7ec4-ddec-4295-af20-cfae09d7c419', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (24, N'T24_Aggregate', N'T24 Aggregate', N'0eaa6c32-a77c-4009-813d-6c005d7c09a0', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (25, N'T25_Shipment', N'T25 Shipment', N'ee4eb1e7-5e66-4209-82c8-4cb7cbd24ca7', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblTransactionTypes] ([TransactionTypesIndex], [TransactionTypesCode], [TransactionTypesName], [TransactionTypesGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'T_Maximum', N'T Maximum', N'd3eff1f3-bd5e-4ed9-8515-003ea93e1a45', N'6/18/2012 1:03:31 PM +00:00', N'Administrator', N'6/18/2012 1:03:31 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblUserDataType])=0
BEGIN
	INSERT INTO [lookup].[tblUserDataType] ([UserDataTypeIndex], [UserDataTypeCode], [UserDataTypeName], [UserDataTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'TEXT', N'TEXT', N'e53acf6d-ea94-4de2-9e97-653b73953ad8', N'6/18/2012 1:04:02 PM +00:00', N'Administrator', N'6/18/2012 1:04:02 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblUserDataType] ([UserDataTypeIndex], [UserDataTypeCode], [UserDataTypeName], [UserDataTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'LIST', N'LIST', N'36f4cb04-8786-4510-9f59-4b61507ba218', N'6/18/2012 1:04:02 PM +00:00', N'Administrator', N'6/18/2012 1:04:02 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblVariantType])=0
BEGIN
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'System.Byte', N'SmallInt', N'8f434a3d-2062-4ef1-b564-a27b49256b54', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'System.Int16', N'SmallInt', N'4761a22d-9d7c-4df1-94a8-e991df8a951e', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'System.UInt16', N'Int', N'fe961e48-6126-477f-8bd8-ccd1d19f6711', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'System.Int32', N'Int', N'4274dabf-0a70-4d72-9456-e989b0c3d868', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'System.Int64', N'BigInt', N'b3506d9d-0a16-4829-b90a-d5353b4703b7', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'System.UInt32', N'BigInt', N'60e7bb14-595a-4b1c-b6a1-3775eca817f5', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'System.Single', N'Real', N'b643fb6c-6a9d-4da3-abf6-d5a0e086545c', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'System.String', N'NVarChar', N'0fb5493c-03a9-43bd-9246-a5bef5d3791f', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'System.Double', N'Float', N'9f4aacbb-3cb8-416f-acd7-6a41b62dd99b', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (11, N'System.DateTime', N'DateTime', N'b41426b2-787c-4c3a-9f86-73f8d2a75305', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
	INSERT INTO [lookup].[tblVariantType] ([VariantTypeIndex], [CodeType], [DatabaseType], [VariantTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (12, N'System.Int32', N'Int', N'4d8814ab-af9f-49d8-8562-add854712889', N'2012-06-19 08:58:46', N'Administrator', N'2012-06-19 08:58:46', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblVesselType])=0
BEGIN
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'UNDEFINED_VESSEL', N'UNDEFINED VESSEL', N'0b8cc7ff-aa60-44bf-98c2-6f0748f9a692', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'SPHERICAL_VESSEL', N'SPHERICAL VESSEL', N'9b5446e6-219f-4b3b-b44d-22f8bc84d36e', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'CYLINDRICAL_VESSEL', N'CYLINDRICAL VESSEL', N'901f8a5d-2e3d-4f07-8eed-5c645e6ff999', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'BULLET_VESSEL', N'BULLET VESSEL', N'a83f636a-4168-4c6e-8219-050882ef695e', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'PROPANE_VESSEL', N'PROPANE VESSEL', N'ee74d568-93ce-49ff-b50f-e208df5c2115', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'UNDERGROUND_VESSEL', N'UNDERGROUND VESSEL', N'9ef336da-cfbb-44be-8c3c-72a83030e0e9', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (6, N'TANKER_VESSEL', N'TANKER VESSEL', N'f2567d17-319a-4acf-bae4-a1f515dace51', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (7, N'PIPELINE_VESSEL', N'PIPELINE VESSEL', N'9018d229-6bc7-4ee2-8de3-f5d746a7ac47', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (8, N'COLLAPSIBLE_STORAGE_TANK', N'COLLAPSIBLE_STORAGE_TANKE', N'b7b8e688-c703-4685-a443-466034ed04f7', N'5/28/2013 2:18:15 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 2:18:15 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'OTHER_VESSEL', N'OTHER VESSEL', N'c09eac17-90a2-4e7b-a0bc-d96e176a0a7e', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblVesselType] ([VesselTypeIndex], [VesselTypeCode], [VesselTypeName], [VesselTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (10, N'MAX_VESSEL', N'MAX VESSEL', N'088890f9-fbee-44e4-ae8d-639816c478db', N'6/18/2012 1:02:30 PM +00:00', N'Administrator', N'6/18/2012 1:02:30 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblWatchdogMode])=0
BEGIN
	INSERT INTO [lookup].[tblWatchdogMode] ([WatchdogModeIndex], [WatchdogModeCode], [WatchdogModeName], [WatchdogModeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'TOGGLE', N'TOGGLE', N'0c8b653a-ad8e-4f1f-a697-59978a7f4e9d', N'6/18/2012 1:03:26 PM +00:00', N'Administrator', N'6/18/2012 1:03:26 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblWatchdogMode] ([WatchdogModeIndex], [WatchdogModeCode], [WatchdogModeName], [WatchdogModeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'COUNTER', N'COUNTER', N'3e1b069b-05fd-4eae-a9e4-5439811178dc', N'6/18/2012 1:03:26 PM +00:00', N'Administrator', N'6/18/2012 1:03:26 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [dbo].[tblSites])=0
BEGIN
	INSERT INTO [dbo].[tblSites] ([ID], [Number], [SPLCCode], [Address1], [Address2], [City], [State], [Zip], [Country], [Phone], [FAX], [EmailAddress], [EmergencyContact], [EmergencyPhone], [Enabled], [SiteGroupFlag], [TimeZone], [LevelUnitIndex], [TemperatureUnitIndex], [DensityUnitIndex], [PressureUnitIndex], [FlowUnitIndex], [VolumeUnitIndex], [MassUnitIndex], [AdditiveVolumeUnitIndex], [AdditiveProfileCycleAmountUnitIndex], [AdditiveProfileRateUnitIndex], [LevelDecimalPlaces], [TemperatureDecimalPlaces], [DensityDecimalPlaces], [PressureDecimalPlaces], [FlowDecimalPlaces], [VolumeDecimalPlaces], [MassDecimalPlaces], [AdditiveVolumeDecimalPlaces], [AdditiveProfileCycleAmountDecimalPlaces], [AdditiveProfileRateDecimalPlaces], [InhibitAccessAfterHours], [InhibitMultipleCardIns], [AccessCardInRequired], [CheckSiteNumber], [PromptForCustomerCard], [PromptForTractorOrTanker], [PromptForFirstTrailer], [PromptForSecondTrailer], [PromptForCompartment], [EnforceDriverEquipmentMatch], [EnableAdditiveAccounting], [UseCompanyEquipmentIdentifiers], [UseLastKnownGoodTankData], [MaximumLoadAmount], [MaximumLoadTime], [MaximumIdleTime], [MaximumFlushAmount], [MaximumMeterProvingAmount], [MaximumReturnsAmount], [MaximumNumberOfActiveArms], [DriverTimeoutPeriod], [DriverWarningPeriod], [MaximumPrompts], [MaximumVehicleWeight], [LoadByNet], [PromptForShipmentNumber], [MaximumProductTemperature], [ListEquipment], [DeferStationChanges], [InhibitBOLWithBrokenBlends], [InhibitBOLWithImproperAdditization], [InhibitOverweightBOL], [ExceptionBOLPrinter], [EnableAutomaticBOLPrinting], [AutomaticBOLStartNumber], [AutomaticBOLEndNumber], [AutomaticBOLNextNumber], [SeparateManualBOLNumbering], [ManualBOLStartNumber], [ManualBOLEndNumber], [ManualBOLNextNumber], [TransactionStartNumber], [TransactionEndNumber], [TransactionNextNumber], [OrderStartNumber], [OrderEndNumber], [OrderNextNumber], [NumberPrefix], [OpenTransactionWindow], [AdministrativeLockDate], [OperationalLockDate], [MaximumDaysToRetainLogs], [EnableDebugLogging], [EnableAuditLogging], [AutomaticallyPrintAlarmsAndEvents], [AlarmAndEventPrinter], [MailServer], [MailFrom], [MailUserName], [MailPassword], [DialupName], [SCADASystem], [InhibitTemplateGraphics], [RefreshInterval], [InhibitEndOfDayOperations], [InhibitEndOfMonthOperations], [EndOfDayWarningPeriod], [InhibitAutomaticPhysicalInventory], [InhibitAutomaticMeterCloseout], [InhibitAutomaticReportGeneration], [InhibitAutomaticAdjustmentDistribution], [InhibitAutomaticCloseout], [InhibitTankScan], [ReportDirectory], [ManageReports], [ManagedReportDirectory], [VRURateLimit], [VRUHourlyLimit], [VRUDailyLimit], [VRUYearlyLimit], [VRUCurrentYearLimit], [VRURateActual], [VRUHourlyActual], [VRUDailyActual], [VRUYearlyActual], [VRUCurrentYearActual], [VRURateLimitEnabled], [VRUHourlyLimitEnabled], [VRUDailyLimitEnabled], [VRUYearlyLimitEnabled], [VRUCurrentYearLimitEnabled], [WatchdogPeriod], [WatchdogCounterStart], [WatchdogCounterEnd], [NumberDecimalSeparator], [NumberGroupSeparator], [ListSeparator], [TimePattern], [TimeSeparator], [AMSymbol], [PMSymbol], [ShortDatePattern], [DateSeparator], [LongDatePattern], [TwoDigitCalendarEndYear], [UserData1], [UserData2], [UserData3], [UserData4], [UserData5], [UserData6], [UserData7], [UserData8], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [MinTimeAllowedToChangePwd], [MinPwdCharacterLength], [PwdExpirationInDays], [PwdLockoutThreshold], [CheckForPreviousPwd], [StrongPwdUse], [PwdHistoryCount], [ApplyToAllSiteMembers], [InactivityDisablePeriod], [EnforceSingleOwner], [InhibitBOLSummaryAutoPopulate], [InhibitOrderSummaryAutoPopulate], [InhibitSupplyOrderSummaryAutoPopulate], [InvoiceStartNumber], [InvoiceEndNumber], [InvoiceNextNumber], [PromptForReturns], [PromptForTruckCard], [StartingShortCardNumber], [UseShortCardNumber], [ExcessVarianceCount], [ExcessVarianceTolerance], [DisableArchivePeriod], [ExportArchiveDir], [ImportArchiveDir], [GroupLedgerByID], [InhibitSiteLedgerRollup], [UseTankReconciliation], [SiteGuid], [LookupNumberGroupSizesTypeIndex], [LookupQuantityDisplayDefaultIndex], [LookupSecondaryStorageFillMethodIndex], [LookupMailConnectModeIndex], [LookupWatchdogModeIndex], [Contact1Name], [Contact1Address1], [Contact1Address2], [Contact1City], [Contact1State], [Contact1Zip], [Contact1Country], [Contact1PhoneOffice], [Contact1Fax], [Contact1EmailAddress], [Contact2Name], [Contact2Address1], [Contact2Address2], [Contact2City], [Contact2State], [Contact2Zip], [Contact2Country], [Contact2PhoneOffice], [Contact2Fax], [Contact2EmailAddress], [Contact1PhoneMobile], [Contact2PhoneMobile], [EnablePasswordHint], [EnablePasswordReset], [MeterReconciliationToleranceIsPercent], [MeterReconciliationReportName], [TranslatedHelpURL], [AllowUseOfSpecialChars], [EnablePeriodicSyncFlag], [PeriodicSyncIntervalMinutes], [DisableSyncTransferFlag], [Enterprise], [OperateTabGroups],[SecurityMode],[SecurityPolicy],[MessageEncoding],[UserIdentityMethod],[MaximumDaysToRetainArchive], [EnforceSalesOrderLimit], [LeakDetectionQuietSamples], [LeakDetectionQuietTime], [LeakDetectionQuietTimeFactor], [LeakDetectionUseMinWait], [LeakDetectionReport], [LeakDetectionPrinter], [EnableAutomaticMovementTicketPrinting], [MovementTicketReport], [MovementTicketPrinter], [MaxOperateTabsAllowed], [CloseoutTime], [PointGroupFileExportDirectory], [PointGroupDefaultFileName], [EnableMovementTicketPDFArchiving], [MovementTicketFileExportDirectory], [MovementTicketExportFileName]) VALUES (N'SiteAdmin', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 1, 1, 'Eastern Standard Time', 27, 2, 191, 73, 109, 46, 64, 40, NULL, NULL, 2, 0, 0, 2, 1, 0, 0, 0, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'11/23/2011 11:40:00 AM +00:00', N'11/23/2011 11:40:00 AM +00:00', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'/', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, '.', ',', ',', 'hh:mm:ss tt', ':', 'AM', 'PM', 'M/d/yyyy', '/', 'ddddd, MMMMM dd, yyyy', 2029, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 30, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 180, NULL, NULL, NULL, NULL, 0, N'00000000-0000-0000-0000-000000000001', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, 0, NULL, NULL, 1, 0, 0, 0, 0, 1, 'None', 'None', 'Binary', 'Anonymous', 365, 0,null,null,null,null,null,null,0,null,null, 10, NULL, NULL, '%SiteID%_%PointGroupID%', 0, NULL, '%SiteID%_%MovementID%')
END

IF (SELECT COUNT(*) FROM [dbo].[tblSiteAdmin] )=0
BEGIN
	INSERT INTO [dbo].[tblSiteAdmin] ([SiteAdminGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'277ed28e-0a86-42f3-934e-72fa5e890458', N'00000000-0000-0000-0000-000000000001', N'6/18/2012 9:05:00 AM +00:00', N'SAIC-US-EAST\dossantosa', N'6/18/2012 9:05:00 AM +00:00', N'SAIC-US-EAST\dossantosa')
END

IF (SELECT COUNT(*) FROM [dbo].[tblGroups])=0
BEGIN
	INSERT INTO [dbo].[tblGroups] ([GroupID], [GroupDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [GroupGuid], [SiteGuid], [SessionTimeout]) VALUES (N'Administrator', N'System Administrators', N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'00000000-0000-0000-0000-000000000003', N'00000000-0000-0000-0000-000000000001', 10)
END

IF (SELECT COUNT(*) FROM [dbo].[tblHelpMapping] WHERE HelpContextKey = '(default)')=0
BEGIN
	INSERT INTO [dbo].[tblHelpMapping] ([HelpMappingGuid], [HelpContextKey], [HelpPage], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'037762e0-c385-481f-982f-dfea7925bf5a', N'(default)', N'Overview.htm', N'9/24/2012 11:24:21 AM -04:00', N'SAIC-US-EAST\dossantosa', N'9/24/2012 11:24:21 AM -04:00', N'SAIC-US-EAST\dossantosa')
END

IF (SELECT COUNT(*) FROM [dbo].[tblSettings])=0
BEGIN
	SET IDENTITY_INSERT [dbo].[tblSettings] ON
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (4, N'EnterpriseDataIntervalBetweenSendAttemptsInMinutes', N'60')
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (5, N'EnterpriseDataSendAttempts', N'3')
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (2, N'ExportArchiveDir', N'C:\temp\ExportFiles\')
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (7, N'ExportingSiteGuid', N'00000000-0000-0000-0000-000000000001')
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (3, N'ImportArchiveDir', N'C:\temp\ImportFiles\')
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (6, N'LogImportProcessRunInformation', N'false')
	INSERT INTO [dbo].[tblSettings] ([SettingID], [SettingKey], [SettingValue]) VALUES (1, N'URLofEnterpriseDataWebService', N'http://NeedToAddServerName/AccountingImportExport/ImportService.asmx')
	SET IDENTITY_INSERT [dbo].[tblSettings] OFF
END

IF (SELECT COUNT(*) FROM [dbo].[tblSitesAncillaryData])=0
BEGIN
	INSERT INTO [dbo].[tblSitesAncillaryData] ([SiteAncillaryDataGuid], [SiteGuid], [AdjustmentTransactionAliasGuid], [IATAGuid], [InventoryTransactionAliasGuid], [NoteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'811c8bdf-a9aa-4843-a4a0-d89b6584cb52', N'00000000-0000-0000-0000-000000000001', NULL, NULL, NULL, NULL, N'10/23/2012 9:38:08 AM -04:00', N'Varec', N'10/23/2012 9:38:08 AM -04:00', N'Varec')
END

IF (SELECT COUNT(*) FROM [dbo].[tblSyncClientConfiguration])=0
BEGIN
	INSERT INTO [dbo].[tblSyncClientConfiguration] ([SyncClientConfigurationGuid], [RootSiteID], [EnterpriseURL], [SuspendSynchronizationFlag], [ServerAuthUserName], [ServerAuthDomain], [ServerAuthClientCertificate], [FMAuthUserName], [FMAuthClientCertificate], [MessageSecuritySigningCertificate], [MessageSecurityOfflineEncryptionCertificate], [MessageSecurityOfflineDecryptionCertificate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [ServiceMaximumRetryAttempts], [ServiceRetryWaitTime]) VALUES (N'6a75fb31-7de8-414a-a32d-045a84482622', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, N'4/2/2013 6:49:43 PM -04:00', N'administrator', N'4/2/2013 6:49:43 PM -04:00', N'administrator', 3, 3000)
END

IF (SELECT COUNT(*) FROM [dbo].[tblSyncServerConfiguration])=0
BEGIN
	INSERT INTO [dbo].[tblSyncServerConfiguration] ([SyncServerConfigurationGuid], [AllowSynchronizationFlag], [AcceptFMUserAuthenticationFlag], [AcceptClientCertificateAuthenticationFlag], [ClientSignatureRequiredForMessagesFlag], [ClientEncryptionRequiredForMessagesFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'19d46ab0-6701-41ee-91ca-686dc3be7b90', 0, 1, 0, 0, 0, N'4/2/2013 6:48:59 PM -04:00', N'administrator', N'4/2/2013 6:48:59 PM -04:00', N'administrator')
END

IF (SELECT COUNT(*) FROM [dbo].[tblSystemSettings])=0
BEGIN
	INSERT INTO [dbo].[tblSystemSettings] ([ReportServerURL], [StationMessageTimeout], [StationPromptTimeout], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SystemSettingGuid], [ReportServerUserName]) VALUES (N'https://zwsplvbqgv.reporting.windows.net/reportserver', NULL, NULL, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'37b3ec11-442d-4ba4-9b77-81753def8512', N'')
END

IF (SELECT COUNT(*) FROM [dbo].[tblUsers])=0
BEGIN
	INSERT INTO [dbo].[tblUsers] ([UserID], [LastLoginDate], [LastLogoffDate], [ChangePassword], [PasswordTimeStamp], [Name], [EmailAddress], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [PasswordLockoutCount], [InactivityLockout], [InactivityLockoutDate], [UserGuid], [SiteGuid], [PasswordHint],[Password], [UserData1], [UserData2], [UserData3], [UserData4], [UserData5], [UserData6], [UserData7], [UserData8], [PhoneNumber]) VALUES (N'Administrator', N'11/23/2011 7:25:00 PM +00:00', N'11/23/2011 7:25:00 PM +00:00', 0, N'11/23/2011 11:40:00 AM +00:00', N'Administrator', N'', N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 7:25:00 PM +00:00', N'administrator', 0, 0, NULL, N'00000000-0000-0000-0000-000000000002', N'00000000-0000-0000-0000-000000000001', N'No hint available',0xFF460F78C0C7B19ACDA135D82247BF57,'','','','','','','','','')
END

IF (SELECT COUNT(*) FROM [map].[tblSiteToSite])=0
BEGIN
	INSERT INTO [map].[tblSiteToSite] ([SiteToSiteGuid], [ParentSiteGuid], [ChildSiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6f804b18-546c-4924-a8e0-92b7c1953fd2', N'00000000-0000-0000-0000-000000000001', N'00000000-0000-0000-0000-000000000001', N'9/21/2012 4:20:22 PM -04:00', N'SAIC-US-EAST\dossantosa', N'9/21/2012 4:20:22 PM -04:00', N'SAIC-US-EAST\dossantosa')
END

IF (SELECT COUNT(*) FROM [map].[tblUserToGroup])=0
BEGIN
	INSERT INTO [map].[tblUserToGroup] ([UserToGroupGuid], [UserGuid], [GroupGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SiteGuid]) VALUES (N'2c3c5ce7-ee54-4dfb-87b9-12f234ea28b6', N'00000000-0000-0000-0000-000000000002', N'00000000-0000-0000-0000-000000000003', N'3/12/2013 1:48:50 PM +00:00', N'Administrator', N'3/12/2013 1:48:50 PM +00:00', N'Administrator', N'00000000-0000-0000-0000-000000000001')
END

IF (SELECT COUNT(*) FROM [map].[tblGroupToRight])=0
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8817720c-b122-4eb5-8264-9f6e083424eb', N'00000000-0000-0000-0000-000000000003', 0, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5e4f5fc0-a319-4726-aff5-6a24922ec0fe', N'00000000-0000-0000-0000-000000000003', 1, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3d13fe37-a3e9-44fd-bcd0-9f313c83a0e1', N'00000000-0000-0000-0000-000000000003', 2, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4b0467c7-019f-4b56-a227-98db8cff47de', N'00000000-0000-0000-0000-000000000003', 3, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e67af44d-204c-4d9f-b0b7-61d2a9b2ca29', N'00000000-0000-0000-0000-000000000003', 4, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'50e37c23-03e3-4ded-bbd6-b8f5ac85bfad', N'00000000-0000-0000-0000-000000000003', 5, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4f2863c0-a31a-4801-9d91-eac26c4f25a4', N'00000000-0000-0000-0000-000000000003', 7, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bc2807f1-2d3a-414b-9529-06b13092f40b', N'00000000-0000-0000-0000-000000000003', 8, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1762f235-d498-41b5-a3c6-148cd2c5bd9b', N'00000000-0000-0000-0000-000000000003', 9, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e005ea20-bf62-43de-9d0b-be8262ebe167', N'00000000-0000-0000-0000-000000000003', 10, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2b76b816-bf32-4489-a624-155d5d0436cf', N'00000000-0000-0000-0000-000000000003', 11, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd87806cb-f82d-4d1d-985e-14f22b22e276', N'00000000-0000-0000-0000-000000000003', 12, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'11a614ab-ef70-4b42-9dee-07bbfa6ac9b6', N'00000000-0000-0000-0000-000000000003', 13, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f83ae0b7-d289-40f8-9c18-7176dd76e6a5', N'00000000-0000-0000-0000-000000000003', 14, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'02705d35-7c71-4fb1-84fc-69d462b163d0', N'00000000-0000-0000-0000-000000000003', 15, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'03927a3f-de6e-4ff2-b882-945b50b06778', N'00000000-0000-0000-0000-000000000003', 16, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8cbede6c-f3b4-4cb0-8891-eac056e954b0', N'00000000-0000-0000-0000-000000000003', 17, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f26171d6-4439-4f1d-b7f0-80e70d477a51', N'00000000-0000-0000-0000-000000000003', 18, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'42d77f26-31c2-4dd6-ae2a-b69f0d92260b', N'00000000-0000-0000-0000-000000000003', 19, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9baa7db5-5f5f-4667-93b4-9f2e10f68b1b', N'00000000-0000-0000-0000-000000000003', 20, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'82bd914a-efb5-4d6b-87ef-5a7f55241e73', N'00000000-0000-0000-0000-000000000003', 21, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7d2960c3-0f0f-43e1-8fa9-d160d83f1a71', N'00000000-0000-0000-0000-000000000003', 22, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b2a511ea-3329-455a-a6a0-c5d3ead41c32', N'00000000-0000-0000-0000-000000000003', 23, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'503c369e-230b-499a-b4d7-f956be2ac851', N'00000000-0000-0000-0000-000000000003', 24, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'18f98ce7-7acc-4f6a-8377-dd936e30887c', N'00000000-0000-0000-0000-000000000003', 25, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3b60ee09-9c6a-4f7d-9b91-8fb3d1a0369a', N'00000000-0000-0000-0000-000000000003', 26, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2efbaaa2-2387-4e45-9302-a01c5091d161', N'00000000-0000-0000-0000-000000000003', 27, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0060a89c-87cb-4be3-b98a-cb0384757c1d', N'00000000-0000-0000-0000-000000000003', 28, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8cfbbb19-a5ae-44dc-8bf7-44fa920ce03f', N'00000000-0000-0000-0000-000000000003', 29, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c08362fd-ebef-46cd-801e-2525057419a9', N'00000000-0000-0000-0000-000000000003', 30, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'59af4dcb-4f01-4e5e-84c8-59295d5003ef', N'00000000-0000-0000-0000-000000000003', 31, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'678415e1-986e-4458-9170-43b76e57d1b0', N'00000000-0000-0000-0000-000000000003', 32, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e7ba3471-5417-4221-a320-0371524701b3', N'00000000-0000-0000-0000-000000000003', 33, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd5db8ce2-66bc-4b24-b33f-91e60c0af739', N'00000000-0000-0000-0000-000000000003', 34, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'11a26c22-37cf-4ea3-84a2-eed9efa0d252', N'00000000-0000-0000-0000-000000000003', 38, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3f36a9af-582d-4aef-b087-5da914f02c88', N'00000000-0000-0000-0000-000000000003', 39, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0f4800ca-5433-467c-ab35-1444b7efe64b', N'00000000-0000-0000-0000-000000000003', 40, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd5bc2d66-e456-451d-a65a-d6c58905bbec', N'00000000-0000-0000-0000-000000000003', 41, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c068dbc7-c95d-4ad9-b425-998a0fc2f108', N'00000000-0000-0000-0000-000000000003', 42, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd9f91ef9-605b-4efc-bf13-145769f986ad', N'00000000-0000-0000-0000-000000000003', 43, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c0116fda-bb53-41ab-892a-4f64d16be0d5', N'00000000-0000-0000-0000-000000000003', 44, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd5019d8f-4122-4f9e-9986-67dae4bc8262', N'00000000-0000-0000-0000-000000000003', 45, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7f934d75-8b7b-4aac-8a72-ec1c304853ab', N'00000000-0000-0000-0000-000000000003', 46, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'57cf1195-d7b1-4fe0-8fb3-ba0134b344f0', N'00000000-0000-0000-0000-000000000003', 47, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'021454c3-7cb1-42bc-bd0c-bb11c38e9d5c', N'00000000-0000-0000-0000-000000000003', 48, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'794205c4-b32f-4167-8d60-b9a837411a0a', N'00000000-0000-0000-0000-000000000003', 49, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'39d31d45-c4c5-46eb-8497-c76b1c00f0c0', N'00000000-0000-0000-0000-000000000003', 50, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8a95e5f2-d30f-43ec-aaa5-3ac1e6b51e14', N'00000000-0000-0000-0000-000000000003', 51, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd5060629-8567-42ad-b71a-6c053ea04d55', N'00000000-0000-0000-0000-000000000003', 52, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'cc124e85-0bc3-4e2e-81cf-cd0987fa6ba0', N'00000000-0000-0000-0000-000000000003', 53, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'529dd9e6-0a39-452c-995e-a35fc43a222d', N'00000000-0000-0000-0000-000000000003', 54, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5d7a3348-017d-4181-86e3-d6a6cbc2f5bc', N'00000000-0000-0000-0000-000000000003', 55, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'208f623f-435c-4e11-b101-034580a0415e', N'00000000-0000-0000-0000-000000000003', 56, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8ea05a5f-8cbc-48fd-878b-793143e1d8cd', N'00000000-0000-0000-0000-000000000003', 57, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f16d3843-5193-4d2b-8368-287500f90638', N'00000000-0000-0000-0000-000000000003', 64, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'45cdd1f7-0617-4ca9-b816-b910cc62973b', N'00000000-0000-0000-0000-000000000003', 67, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2767535a-4786-426c-8c31-9cba2a044421', N'00000000-0000-0000-0000-000000000003', 68, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'521f6cfb-d348-4c8a-9f4a-84b4b1be4b07', N'00000000-0000-0000-0000-000000000003', 69, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'42b23a3e-d198-4633-8ece-4ef9e34af64b', N'00000000-0000-0000-0000-000000000003', 70, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5a1a4e85-f1c9-4a9e-98df-dc2531d9f45a', N'00000000-0000-0000-0000-000000000003', 71, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'672d385d-5e42-42df-8fd3-e519b8368a82', N'00000000-0000-0000-0000-000000000003', 72, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1cb8ff6c-909f-4cf1-9907-605186cab3f5', N'00000000-0000-0000-0000-000000000003', 73, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f16307d9-3c49-4b9e-9089-52e2f2b27beb', N'00000000-0000-0000-0000-000000000003', 74, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7104fef3-c122-46a0-b5b1-f380d53f8dc7', N'00000000-0000-0000-0000-000000000003', 75, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'371c0e7b-1d73-4658-8c48-fa43a4866568', N'00000000-0000-0000-0000-000000000003', 76, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'39d074d0-f54a-43b8-8960-031532d53b14', N'00000000-0000-0000-0000-000000000003', 77, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'81a58bc0-b41f-469d-be2e-caf15d62565c', N'00000000-0000-0000-0000-000000000003', 78, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8a7fd6e9-9f0c-4ac1-82cd-974dbcf0543c', N'00000000-0000-0000-0000-000000000003', 79, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd8f98910-9ddf-4e71-a5a4-21f80a0346e9', N'00000000-0000-0000-0000-000000000003', 80, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b9d42855-118b-440d-92f6-aff284bfeb69', N'00000000-0000-0000-0000-000000000003', 81, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b8743e3b-6825-4f8c-ac84-f3e09cee1787', N'00000000-0000-0000-0000-000000000003', 82, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd08117c6-703a-474e-99eb-5e5ebd330bca', N'00000000-0000-0000-0000-000000000003', 83, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'291d1407-e183-4afb-b2d2-ab87ce710dab', N'00000000-0000-0000-0000-000000000003', 84, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'eda7d0e6-6f04-4195-95d6-685eda8e83c2', N'00000000-0000-0000-0000-000000000003', 85, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e6fdcce3-29a7-40f3-b189-eceeebca3ab5', N'00000000-0000-0000-0000-000000000003', 86, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'98ca98b4-6b9c-41fc-b232-5f3d6f77568b', N'00000000-0000-0000-0000-000000000003', 87, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'29eb73e5-9c92-4af6-b690-3a08fa25d95c', N'00000000-0000-0000-0000-000000000003', 88, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e14e7aa3-aebe-4c35-9457-2215c61f010c', N'00000000-0000-0000-0000-000000000003', 89, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'465d1042-fe2b-4584-a386-66e8e3334950', N'00000000-0000-0000-0000-000000000003', 90, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'11d8b5f1-73e9-4aa4-85ab-96ae8e392d24', N'00000000-0000-0000-0000-000000000003', 91, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5589408c-5c22-4782-b2a8-f250888ef256', N'00000000-0000-0000-0000-000000000003', 92, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e2547af2-5862-43b1-985b-e1d18c0daa1f', N'00000000-0000-0000-0000-000000000003', 93, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e3bcca05-c3cd-4766-a86f-0d353efd3ebe', N'00000000-0000-0000-0000-000000000003', 94, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'732fb81a-f566-4758-8d66-e30725023e06', N'00000000-0000-0000-0000-000000000003', 95, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'baa44324-78ee-4eae-9d1d-2427f85165b1', N'00000000-0000-0000-0000-000000000003', 96, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ed285d9c-d1d0-4b2e-8119-df6ea889f6ca', N'00000000-0000-0000-0000-000000000003', 97, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4572194c-4669-4f4b-953a-38656c10c830', N'00000000-0000-0000-0000-000000000003', 98, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4e6c4fa1-a2bb-4d87-9224-ad6273091ab4', N'00000000-0000-0000-0000-000000000003', 99, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'00c184cb-7cd9-4da5-bdc2-de4937891d4d', N'00000000-0000-0000-0000-000000000003', 100, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2b4368a9-a844-4967-baab-d5eca751b140', N'00000000-0000-0000-0000-000000000003', 101, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3f8ad7fe-fee4-48f8-b389-46c3bb1db57f', N'00000000-0000-0000-0000-000000000003', 102, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'09b7a15e-8675-4d3d-bc1b-842bc92d7f95', N'00000000-0000-0000-0000-000000000003', 103, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'809089a5-a533-4c0f-9620-0f69bd9f6919', N'00000000-0000-0000-0000-000000000003', 104, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bce6d4c9-556f-4cc2-b410-f25f2f7ddc51', N'00000000-0000-0000-0000-000000000003', 105, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd17fbf04-57ae-43cf-8cd4-cb94516a2523', N'00000000-0000-0000-0000-000000000003', 106, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'98b38746-f7d9-4655-affe-2599af707438', N'00000000-0000-0000-0000-000000000003', 107, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'79aef761-8bf3-422e-8604-b52fb74948c9', N'00000000-0000-0000-0000-000000000003', 108, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'26eeab8f-77d2-4bcd-b341-1935768d74bd', N'00000000-0000-0000-0000-000000000003', 109, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f98b91c6-ea1b-45ff-8f24-0ac7d6327228', N'00000000-0000-0000-0000-000000000003', 110, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'13917f5f-2984-4288-8c24-311201e35730', N'00000000-0000-0000-0000-000000000003', 111, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'63188fef-e089-4084-b874-a8d0e4321315', N'00000000-0000-0000-0000-000000000003', 112, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5bd38f5c-2865-4987-b3b6-1464d85e8c23', N'00000000-0000-0000-0000-000000000003', 113, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'eb5b1ac7-2649-4fe9-a4ee-424a806ef628', N'00000000-0000-0000-0000-000000000003', 116, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c2fc064d-e81c-4fe7-9839-aa425d3acb74', N'00000000-0000-0000-0000-000000000003', 117, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bb323210-c9b6-4f3c-9dd8-81b2a39b55b4', N'00000000-0000-0000-0000-000000000003', 118, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'595c89cf-d2c2-4704-8238-49015cdcfff0', N'00000000-0000-0000-0000-000000000003', 119, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4e7d9d30-689e-43f5-aeef-8e03b3c3c87a', N'00000000-0000-0000-0000-000000000003', 120, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'980524f2-fe4e-4683-9d20-a8d1bc6320d0', N'00000000-0000-0000-0000-000000000003', 121, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3c43da80-1039-4493-ae6a-2721db01e74d', N'00000000-0000-0000-0000-000000000003', 122, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'847e9cd5-b6eb-4f9d-ae4e-e40106a5449c', N'00000000-0000-0000-0000-000000000003', 123, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e3b9d986-4b30-4964-ac5b-d94ada7015c8', N'00000000-0000-0000-0000-000000000003', 125, N'6/19/2012 9:04:10 AM -04:00', N'SAIC-US-EAST\dossantosa', N'6/19/2012 9:04:10 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'cfd09108-cdbf-46b6-afb1-91d4cef96df6', N'00000000-0000-0000-0000-000000000003', 126, N'6/19/2012 9:04:10 AM -04:00', N'SAIC-US-EAST\dossantosa', N'6/19/2012 9:04:10 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd6a1821b-ac70-4311-a96b-08bfb112f9d7', N'00000000-0000-0000-0000-000000000003', 127, N'6/19/2012 9:04:10 AM -04:00', N'SAIC-US-EAST\dossantosa', N'6/19/2012 9:04:10 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dc4b04fc-8d17-419d-8c01-7af61aab3336', N'00000000-0000-0000-0000-000000000003', 130, N'9/21/2012 4:05:06 PM -04:00', N'Varec', N'9/21/2012 4:05:06 PM -04:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e8e6deff-88a1-4f6c-a16f-e8753311d301', N'00000000-0000-0000-0000-000000000003', 128, N'9/21/2012 4:05:06 PM -04:00', N'Varec', N'9/21/2012 4:05:06 PM -04:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1d63b856-9b78-4e85-b99d-6d807243a4a9', N'00000000-0000-0000-0000-000000000003', 129, N'9/21/2012 4:05:06 PM -04:00', N'Varec', N'9/21/2012 4:05:06 PM -04:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1b852111-5908-40d4-8ff7-43df2d2f2790', N'00000000-0000-0000-0000-000000000003', 131, N'9/21/2012 4:15:42 PM -04:00', N'Varec', N'9/21/2012 4:15:42 PM -04:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'980b760d-20c1-45c5-8a3f-8529be204314', N'00000000-0000-0000-0000-000000000003', 132, N'9/21/2012 4:15:42 PM -04:00', N'Varec', N'9/21/2012 4:15:42 PM -04:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'db463d19-84a7-4472-a3a2-458d4d2074cb', N'00000000-0000-0000-0000-000000000003', 124, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e6b697d8-ca2b-4967-b43b-8fd953ca6eba', N'00000000-0000-0000-0000-000000000003', 133, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4e04b09f-e11d-4f6f-b2ab-bd5e52ed01ac', N'00000000-0000-0000-0000-000000000003', 134, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'49b908b2-952a-42ee-8c85-6df77d52a889', N'00000000-0000-0000-0000-000000000003', 135, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c7ce3851-ad7c-402a-b9f5-9220212bf7eb', N'00000000-0000-0000-0000-000000000003', 136, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'de363be8-49d0-491f-b8e8-c639b426f1c9', N'00000000-0000-0000-0000-000000000003', 137, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0dbb1312-7c8b-4e6c-a078-cffae6046d16', N'00000000-0000-0000-0000-000000000003', 138, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8185da01-afa6-40c4-8873-5efb52fe1f26', N'00000000-0000-0000-0000-000000000003', 141, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ede9179d-4da9-4fc1-aa07-2e05ae512f85', N'00000000-0000-0000-0000-000000000003', 142, N'11/28/2012 12:43:28 PM -05:00', N'Varec', N'11/28/2012 12:43:28 PM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f3e7ab51-d080-4715-9ed9-f7da1721ef37', N'00000000-0000-0000-0000-000000000003', 143, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9f5cfe11-acbb-44f1-ba7b-404b3b299b0f', N'00000000-0000-0000-0000-000000000003', 144, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ce922a1a-c991-46e0-a847-a53573bc0f18', N'00000000-0000-0000-0000-000000000003', 145, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1e39a468-1109-497f-8d22-9c2c639ffeb2', N'00000000-0000-0000-0000-000000000003', 146, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'204baef2-71bb-4f36-8762-c4281e7cbcff', N'00000000-0000-0000-0000-000000000003', 147, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'460837b2-7ad8-4a6f-9e9f-b68f6cb0f1a9', N'00000000-0000-0000-0000-000000000003', 148, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'597664fe-f8ed-4c17-95cd-78ba14c9afec', N'00000000-0000-0000-0000-000000000003', 149, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'31ce8e42-0f5b-47c4-8844-8058083575ee', N'00000000-0000-0000-0000-000000000003', 150, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0a89f2de-7c80-4728-80d7-c7b572c9a23a', N'00000000-0000-0000-0000-000000000003', 151, N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa', N'3/12/2013 11:02:32 AM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd2d5eaf4-8887-485e-b5de-bdc9e7ca638e', N'00000000-0000-0000-0000-000000000003', 163, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd1758e4c-6bee-4fa9-899c-33a5c44d0471', N'00000000-0000-0000-0000-000000000003', 164, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'00e3e575-695f-4571-a783-87b905509c5c', N'00000000-0000-0000-0000-000000000003', 165, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6b460d3b-c87a-4848-a0d9-319bc5a5970b', N'00000000-0000-0000-0000-000000000003', 166, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6f751932-bec4-4164-a517-93bf9e0ecb62', N'00000000-0000-0000-0000-000000000003', 167, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'85656831-c2cf-454c-839b-d34c601d3799', N'00000000-0000-0000-0000-000000000003', 168, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7b65b122-81c2-4389-9938-4047f492f5a7', N'00000000-0000-0000-0000-000000000003', 169, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'968dc8aa-dd3d-432e-830b-da5bd6aba1d9', N'00000000-0000-0000-0000-000000000003', 170, N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 12:09:48 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'30ebfdf7-b8b0-457b-ac1b-fcb8ab20c792', N'00000000-0000-0000-0000-000000000003', 171, N'5/28/2013 2:26:28 PM -04:00', N'SAIC-US-EAST\dossantosa', N'5/28/2013 2:26:28 PM -04:00', N'SAIC-US-EAST\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'62838235-7e2d-41ef-a1b0-7bde50c7ff4f', N'00000000-0000-0000-0000-000000000003', 172, N'7/19/2013 11:17:11 AM -04:00', N'LEIDOS-CORP\dossantosa', N'7/19/2013 11:17:11 AM -04:00', N'LEIDOS-CORP\dossantosa')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bbb8b769-2cb2-49f4-aa4f-ea12cf498c67', N'00000000-0000-0000-0000-000000000003', 173, N'11/5/2013 10:23:55 AM -05:00', N'Varec', N'11/5/2013 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'908C2716-27E4-4B61-B53C-EA7695818F84', N'00000000-0000-0000-0000-000000000003', 176, N'7/20/2017 10:23:55 AM -05:00', N'Varec', N'7/20/2017 10:23:55 AM -05:00', N'Varec')

	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'17534D97-3A50-4329-B5AB-7C0F278C9A8E', N'00000000-0000-0000-0000-000000000003', 300, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'325DBD22-A8EE-468C-9D48-14E617B975A3', N'00000000-0000-0000-0000-000000000003', 301, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'62DBA7F1-0C60-4CE6-B825-B13A56CD8D61', N'00000000-0000-0000-0000-000000000003', 302, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'61B2C8FB-1D01-4116-9173-00906B698756', N'00000000-0000-0000-0000-000000000003', 303, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'B95915E9-A019-4DDD-AC55-F612B9E62045', N'00000000-0000-0000-0000-000000000003', 304, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'930B8435-815B-4832-9784-29B11AA8DE11', N'00000000-0000-0000-0000-000000000003', 305, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7DEA958E-6684-4DB5-A11E-B6A4B1918238', N'00000000-0000-0000-0000-000000000003', 306, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'50CBAB53-7FDE-419D-A89D-413CDADADCDD', N'00000000-0000-0000-0000-000000000003', 307, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'FE7DA72A-C686-480C-9AA6-4E3C690BAEF9', N'00000000-0000-0000-0000-000000000003', 308, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'03C24C4B-82C0-4BE7-A70C-021D6B08FAA9', N'00000000-0000-0000-0000-000000000003', 309, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'52459D14-E0C5-46BD-AAB5-649D0AEA1263', N'00000000-0000-0000-0000-000000000003', 310, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6A9A85D9-7770-4066-917D-793975FD889C', N'00000000-0000-0000-0000-000000000003', 311, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'A9E6EEE1-DB6B-417D-B8E7-7775EAD59CD1', N'00000000-0000-0000-0000-000000000003', 312, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C03424BA-1B6B-4EEF-B4CD-1E936945EE04', N'00000000-0000-0000-0000-000000000003', 313, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'22B6BC18-DCE0-417A-8538-30B3C2ADFFA9', N'00000000-0000-0000-0000-000000000003', 314, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1F2EA136-4E59-4DB9-AE44-AA3046EAA372', N'00000000-0000-0000-0000-000000000003', 315, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2137FC6A-4229-4F89-8837-4F0994FA4E42', N'00000000-0000-0000-0000-000000000003', 316, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'CE6ED857-5D88-4C6E-80FB-840958222906', N'00000000-0000-0000-0000-000000000003', 317, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'21140186-E420-41AB-A6EE-9391CF320AEC', N'00000000-0000-0000-0000-000000000003', 318, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'28EA099F-985F-4017-BAD0-69F390E1FC96', N'00000000-0000-0000-0000-000000000003', 319, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'F228BC59-F0E0-4192-AB57-996396C7D6FD', N'00000000-0000-0000-0000-000000000003', 320, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'F27F7FDD-8D69-4BF3-9E46-238E9F3870B6', N'00000000-0000-0000-0000-000000000003', 321, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3D917BAB-168C-4278-8D3D-546BE0025FED', N'00000000-0000-0000-0000-000000000003', 322, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
--	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'E5F56579-2B2A-4F31-8DC1-B1C497BBBFD7', N'00000000-0000-0000-0000-000000000003', 206, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'089D4BC1-941E-41C9-8E6A-47556121420C', N'00000000-0000-0000-0000-000000000003', 324, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8F5C3CF2-412E-49F6-9871-82454EF46878', N'00000000-0000-0000-0000-000000000003', 325, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'CD1AA2D6-E414-4457-83BF-BEE00AD6821F', N'00000000-0000-0000-0000-000000000003', 326, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6B8C27D9-A26A-499F-BD90-75DB746B69E6', N'00000000-0000-0000-0000-000000000003', 327, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'09EE3BEC-528D-44FF-8383-860FA944DA72', N'00000000-0000-0000-0000-000000000003', 328, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'E37C9A7F-5A93-428F-B0C4-1467D2073D63', N'00000000-0000-0000-0000-000000000003', 329, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5C004B22-C87A-4891-92C7-0E2E21FD00E3', N'00000000-0000-0000-0000-000000000003', 330, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'F9702BE0-72C8-4125-BB40-94DD7608DC02', N'00000000-0000-0000-0000-000000000003', 331, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'DC5FEDF6-3D4D-4657-A637-F997D179E61F', N'00000000-0000-0000-0000-000000000003', 332, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6A84BB43-7852-456D-AFB2-5A81C5B6E11C', N'00000000-0000-0000-0000-000000000003', 333, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'E6AD83B3-D2C6-48B5-ABE9-6943C7529795', N'00000000-0000-0000-0000-000000000003', 334, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'E0FD869E-C704-43F8-A8A1-656A5D6AB665', N'00000000-0000-0000-0000-000000000003', 335, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'CD254E15-9A75-4116-87AE-F568062FCF4B', N'00000000-0000-0000-0000-000000000003', 336, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'A915608A-79F5-410F-8D3A-017239EF3834', N'00000000-0000-0000-0000-000000000003', 337, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6604538A-E72C-4E65-930F-BBE160B7CB90', N'00000000-0000-0000-0000-000000000003', 338, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'E5879734-F589-4B74-9DEB-690A02E793D7', N'00000000-0000-0000-0000-000000000003', 339, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'CF9855E8-242B-41E7-9424-1D9C3B40EB86', N'00000000-0000-0000-0000-000000000003', 340, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'B0A5DC58-D8FD-4645-A6EE-211773EAD72F', N'00000000-0000-0000-0000-000000000003', 341, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'B5E53408-C55C-4F96-8890-7E2329C61E20', N'00000000-0000-0000-0000-000000000003', 342, N'12/29/2015 10:23:55 AM -05:00', N'Varec', N'12/29/2015 10:23:55 AM -05:00', N'Varec')

END

IF (SELECT COUNT(*) FROM [map].[tblEntityUserToSite])=0
BEGIN
	INSERT INTO [map].[tblEntityUserToSite] ([UserToSiteGuid], [UserGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES (N'f5176f5b-f6ad-4458-bffb-8a693dcaec81', N'00000000-0000-0000-0000-000000000002', N'00000000-0000-0000-0000-000000000001', N'6/15/2012 9:19:00 AM +00:00', N'SAIC-US-EAST\dossantosa', N'6/15/2012 9:19:00 AM +00:00', N'SAIC-US-EAST\dossantosa', N'00000000-0000-0000-0000-000000000001')
END

IF (SELECT COUNT(*) FROM [map].[tblEntityUserGroupToSite])=0
BEGIN
	INSERT INTO [map].[tblEntityUserGroupToSite] ([UserGroupToSiteGuid], [GroupGuid], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [AssignedFromSiteGuid]) VALUES (N'1c915f30-de7f-494d-8e75-b6c001280064', N'00000000-0000-0000-0000-000000000003', N'00000000-0000-0000-0000-000000000001', N'6/15/2012 9:19:00 AM +00:00', N'SAIC-US-EAST\dossantosa', N'6/15/2012 9:19:00 AM +00:00', N'SAIC-US-EAST\dossantosa', N'00000000-0000-0000-0000-000000000001')
END



IF (SELECT COUNT(*) FROM [dbo].[tblAuditHandler])=0
BEGIN
	
	-- Update tblAuditHandler with TypeID, ParentTypeID, and IDQuery
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblExportRequest'
							, 'Export Requests'
							, ''
							, 'SELECT @ID = a.InterfaceID + '' - '' + a.RequestID'
							+ ' FROM [fmAudit].[tblExportRequest] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblMenuFavorites'
							, 'User - Menu Favorite'
							, ''
							, 'SELECT @ID = CASE WHEN u.UserID IS NULL THEN ua.UserID ELSE u.UserID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(mt.MenuItemTypeName)'
							+ ' FROM [fmAudit].[tblMenuFavorites] a'
							+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblMenuItemType] mt ON mt.MenuItemTypeIndex = a.MenuItemType'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'erv_tblEntityRecordVersioningFieldConfig'
							, 'Field Level Configuration'
							, ''
							, 'SELECT @ID = es.EntityTypeDisplayName + '' - '' + a.TargetField'
							+ ' FROM [fmAudit].[erv_tblEntityRecordVersioningFieldConfig] a'
							+ ' LEFT JOIN [erv].[tblEntitySegmentTemplate] es ON es.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblReserveLevels'
							, 'Reserve Level'
							, ''
							, 'SELECT @ID = CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[tblReserveLevels] a'
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblControllersLog'
							, 'Controller - Memo'
							, ''
							, 'SELECT @ID = a.Controller + '' - '' + a.Memo'
							+ ' FROM [fmAudit].[tblControllersLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblDispatchConfiguration'
							, 'Dispatch Configuration'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblDispatchConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblDispatchGrid'
							, 'Dispatch Grid'
							, 'Dispatch Configuration'
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblDispatchGrid] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblDispatchGridColumn'
							, 'Dispatch Grid - Column'
							, 'Dispatch Configuration'
							, 'SELECT @ID = a.DispatchGridID + '' - '' + a.ID'
							+ ' FROM [fmAudit].[tblDispatchGridColumn] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblCustomToolbar'
							, 'Dispatch Toolbar'
							, 'Dispatch Configuration'
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblCustomToolbar] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblCustomToolbarCommand'
							, 'Dispatch Toolbar - Command'
							, 'Dispatch Configuration'
							, 'SELECT @ID = a.CustomToolbarID + '' - '' + a.ID'
							+ ' FROM [fmAudit].[tblCustomToolbarCommand] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityDispatchConfigurationToSite'
							, 'Site - Dispatch Configuration'
							, 'Dispatch Configuration'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - Dispatch Configuration'''
							+ ' FROM  [fmaudit].[map_tblEntityDispatchConfigurationToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblNotes'
							, 'Notes'
							, ''
							, 'SELECT @ID = a.Note'
							+ ' FROM [fmAudit].[tblNotes] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblMessages'
							, 'Message'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblMessages] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblDataDictionaries'
							, 'Data Dictionary'
							, ''
							, 'SELECT @ID = a.[Key] + '' - '' + a.[Value]'
							+ ' FROM [fmAudit].[tblDataDictionaries] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityDataDictionaryToSite'
							, 'Site - Data Dictionary'
							, 'Data Dictionary'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Key/Value Pairs'''
							+ ' FROM  [fmaudit].[map_tblEntityDataDictionaryToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAppointmentTank'
							, 'Tank Appointment'
							, ''
							, 'SELECT @ID = CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + a.Description'
							+ ' FROM [fmAudit].[tblAppointmentTank] a'
							+ ' LEFT JOIN [dbo].tblTanks t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].tblTanks ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAppointmentTankToSite'
							, 'Site - Tank Appointment'
							, 'Tank Appointment'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + CASE WHEN at.Description IS NULL THEN ata.Description ELSE at.Description END'
							+ ' FROM  [fmaudit].[map_tblEntityAppointmentTankToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAppointmentTank] at ON at.AppointmentTankGuid = a.AppointmentTankGuid'
							+ ' LEFT JOIN [fmaudit].[tblAppointmentTank] ata ON ata.AppointmentTankGuid = a.AppointmentTankGuid AND ata._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = at.TankGuid OR t.TankGuid = ata.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = ata.TankGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAppointmentPersonnel'
							, 'Personnel Appointment'
							, ''
							, 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + a.Description'
							+ ' FROM [fmAudit].[tblAppointmentPersonnel] a'
							+ ' LEFT JOIN [dbo].tblPersonnel p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].tblPersonnel pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAppointmentPersonnelToSite'
							, 'Site - Personnel Appointment'
							, 'Personnel Appointment'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN ap.Description IS NULL THEN apa.Description ELSE ap.Description END'
							+ ' FROM  [fmaudit].[map_tblEntityAppointmentPersonnelToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAppointmentPersonnel] ap ON ap.AppointmentPersonnelGuid = a.AppointmentPersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblAppointmentPersonnel] apa ON apa.AppointmentPersonnelGuid = a.AppointmentPersonnelGuid AND apa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = ap.PersonnelGuid OR p.PersonnelGuid = apa.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = apa.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAppointmentEquipment'
							, 'Equipment Appointment'
							, ''
							, 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + a.Description'
							+ ' FROM [fmAudit].[tblAppointmentEquipment] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAppointmentEquipmentToSite'
							, 'Site - Equipment Appointment'
							, 'Equipment Appointment'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + CASE WHEN ae.Description IS NULL THEN aea.Description ELSE ae.Description END'
							+ ' FROM  [fmaudit].[map_tblEntityAppointmentEquipmentToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAppointmentEquipment] ae ON ae.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblAppointmentEquipment] aea ON aea.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid AND aea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = ae.EquipmentGuid OR e.EquipmentGuid = aea.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = aea.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblQualityTags'
							, 'Quality Tags'
							, ''
							, 'SELECT @ID = a.Name'
							+ ' FROM [fmAudit].[tblQualityTags] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTankQualityTagLog'
							, 'Tank - Quality Tag'
							, 'Quality Tags'
							, 'SELECT @ID = a.TankID + '' - '' + a.QualityTagName'
							+ ' FROM [fmAudit].[tblTankQualityTagLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblEquipmentQualityTagLog'
							, 'Equipment - Quality Tag'
							, 'Quality Tags'
							, 'SELECT @ID = a.EquipmentID + '' - '' + a.QualityTagName'
							+ ' FROM [fmAudit].[tblEquipmentQualityTagLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityQualityTagToSite'
							, 'Site - Quality Tag'
							, 'Quality Tags'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.Name IS NULL THEN qa.Name ELSE q.Name END'
							+ ' FROM  [fmaudit].[map_tblEntityQualityTagToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].tblQualityTags q ON q.QualityTagGuid = a.QualityTagGuid'
							+ ' LEFT JOIN [fmaudit].tblQualityTags qa ON qa.QualityTagGuid = a.QualityTagGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTestSetDefinitions'
							, 'Test Set'
							, ''
							, 'SELECT @ID = a.TestSetName'
							+ ' FROM [fmAudit].[tblTestSetDefinitions] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityTestSetToSite'
							, 'Site - Test Set'
							, 'Test Set'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ts.TestSetName IS NULL THEN tsa.TestSetName ELSE ts.TestSetName END'
							+ ' FROM  [fmaudit].[map_tblEntityTestSetToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTestSetDefinitions] ts ON ts.TestSetDefinitionGuid = a.TestSetDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestSetDefinitions] tsa ON tsa.TestSetDefinitionGuid = a.TestSetDefinitionGuid AND tsa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTestDefinitions'
							, 'Test'
							, ''
							, 'SELECT @ID = a.TestName'
							+ ' FROM [fmAudit].[tblTestDefinitions] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityTestToSite'
							, 'Site - Test'
							, 'Test'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN t.TestName IS NULL THEN ta.TestName ELSE t.TestName END'
							+ ' FROM  [fmaudit].[map_tblEntityTestToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTestDefinitions] t ON t.TestDefinitionGuid = a.TestDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestDefinitions] ta ON ta.TestDefinitionGuid = a.TestDefinitionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblTestDefinitionToTestSetDefinition'
							, 'Test Set - Test'
							, 'Test Set'
							, 'SELECT @ID = CASE WHEN ts.TestSetName IS NULL THEN tsa.TestSetName ELSE ts.TestSetName END + '' - '''
							+ ' + CASE WHEN t.TestName IS NULL THEN ta.TestName ELSE t.TestName END'
							+ ' FROM  [fmaudit].[map_tblTestDefinitionToTestSetDefinition] a'
							+ ' LEFT JOIN [dbo].[tblTestSetDefinitions] ts ON ts.TestSetDefinitionGuid = a.TestSetDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestSetDefinitions] tsa ON tsa.TestSetDefinitionGuid = a.TestSetDefinitionGuid AND tsa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTestDefinitions] t ON t.TestDefinitionGuid = a.TestDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestDefinitions] ta ON ta.TestDefinitionGuid = a.TestDefinitionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTestSetTankResults'
							, 'Test Set - Tank'
							, 'Test Set'
							, 'SELECT @ID = a.TestSetName + '' - '' + a.TankID'
							+ ' FROM [fmAudit].[tblTestSetTankResults] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTestTankResults'
							, 'Test - Tank'
							, 'Test'
							, 'SELECT @ID = a.TestName + '' - '''
							+ ' + CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END '
							+ ' FROM [fmAudit].[tblTestTankResults] a'
							+ ' LEFT JOIN [dbo].[tblTestSetTankResults] t ON t.TestSetTankResultGuid = a.TestSetTankResultGuid'
							+ ' LEFT JOIN [fmAudit].[tblTestSetTankResults] ta on ta.TestSetTankResultGuid = a.TestSetTankResultGuid AND  ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTestSetEquipmentResults'
							, 'Test Set - Equipment'
							, 'Test Set'
							, 'SELECT @ID = a.TestSetName + '' - '' + a.EquipmentID'
							+ ' FROM [fmAudit].[tblTestSetEquipmentResults] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTestEquipmentResults'
							, 'Test - Equipment'
							, 'Test'
							, 'SELECT @ID = a.TestName + '' - '''
							+ ' + CASE WHEN t.EquipmentID IS NULL THEN ta.EquipmentID ELSE t.EquipmentID END'
							+ ' FROM [fmAudit].[tblTestEquipmentResults] a'
							+ ' LEFT JOIN [dbo].[tblTestSetEquipmentResults] t ON t.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid'
							+ ' LEFT JOIN [fmAudit].[tblTestSetEquipmentResults] ta on ta.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid AND  ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblPIDXProfiles'
							, 'Data Exchange Profiles'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblPIDXProfiles] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblPIDXProfileToCompany'
							, 'Data Exchange Profiles - Ship To - Load ID'
							, 'Data Exchange Profiles'
							, 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN h1.ID IS NULL THEN ha1.ID ELSE h1.ID END'
							+ ' FROM  [fmaudit].[map_tblPIDXProfileToCompany] a'
							+ ' LEFT JOIN [dbo].[tblPIDXProfiles] p ON p.PIDXProfileGuid = a.PIDXProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblPIDXProfiles] pa ON pa.PIDXProfileGuid = a.PIDXProfileGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [map].[tblCompanyPersonnelToShipToBillTo] h1 ON h1.CompanyPersonnelToShipToBillToGuid = a.CompanyPersonnelToShipToBillToGuid'
							+ ' LEFT JOIN [fmaudit].[map_tblCompanyPersonnelToShipToBillTo] ha1 ON ha1.CompanyPersonnelToShipToBillToGuid = a.CompanyPersonnelToShipToBillToGuid AND ha1._AuditEventType = ''D'''
							+ ' LEFT JOIN [map].[tblCompanyShipToToBillTo] h2 ON h2.CompanyShipToToBillToGuid = h1.CompanyShipToToBillToGuid'
							+ ' LEFT JOIN [fmaudit].[map_tblCompanyShipToToBillTo] ha2 ON ha2.CompanyShipToToBillToGuid = h1.CompanyShipToToBillToGuid AND ha2._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = h2.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = ha2.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblFuelCards'
							, 'Fuel Card'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblFuelCards] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityFuelCardToSite'
							, 'Site - Fuel Card'
							, 'Fuel Card'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFuelCardToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblFuelCards] ap ON ap.FuelCardGuid = a.FuelCardGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCards] apa ON apa.FuelCardGuid = a.FuelCardGuid AND apa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAdditiveProfiles'
							, 'Additive Profiles'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblAdditiveProfiles] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToAdditiveProfile'
							, 'Additive Profile - Additive'
							, 'Additive Profiles'
							, 'SELECT @ID = CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToAdditiveProfile] a'
							+ ' LEFT JOIN [dbo].[tblAdditiveProfiles] ap ON ap.AdditiveProfileGuid = a.AssignedToAdditiveProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblAdditiveProfiles] apa ON apa.AdditiveProfileGuid = a.AssignedToAdditiveProfileGuid AND apa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAdditiveProfileToSite'
							, 'Site - Additive Profile'
							, 'Additive Profiles'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAdditiveProfileToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAdditiveProfiles] ap ON ap.AdditiveProfileGuid = a.AdditiveProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblAdditiveProfiles] apa ON apa.AdditiveProfileGuid = a.AdditiveProfileGuid AND apa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)



	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAlarmPriorityToSite'
							, 'Site - Alarm Priority'
							, 'Alarm Priorities'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAlarmPriorityToSite] a'
							+ ' LEFT JOIN [dbo].[tblAlarmPriorities] ap ON ap.AlarmPriorityGuid = a.AlarmPriorityGuid'
							+ ' LEFT JOIN [fmaudit].[tblAlarmPriorities] apa ON apa.AlarmPriorityGuid = a.AlarmPriorityGuid AND apa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAlarmAndEvents'
							, 'Alarm And Events'
							, ''
							, 'SELECT @ID = a.Source + '' : '' + a.ID'
							+ ' FROM [fmAudit].[tblAlarmAndEvents] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAlarmAndEventToSite'
							, 'Site - Alarm & Events'
							, 'Alarm And Events'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
							+ ' FROM  [fmaudit].[map_tblEntityAlarmAndEventToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblEmailGroups'
							, 'E-mail Groups'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblEmailGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToEmailAddress'
							, 'E-mail Group - E-mail Address'
							, 'E-mail Groups'
							, 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToEmailAddress] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToAlarmEventCategory'
							, 'E-mail Group - Category'
							, 'E-mail Groups'
							, 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToAlarmEventCategory] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblAlarmPriorityToEmailGroup'
							, 'E-mail Group - Priority'
							, 'E-mail Groups'
							, 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblAlarmPriorityToEmailGroup] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAlarmPriorities] ap ON ap.AlarmPriorityGuid = a.AlarmPriorityGuid'
							+ ' LEFT JOIN [fmaudit].[tblAlarmPriorities] apa ON apa.AlarmPriorityGuid = a.AlarmPriorityGuid AND apa._AuditEventType = ''D'''
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEmailGroupToSite'
							, 'Site - E-mail Group'
							, 'E-mail Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEmailGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUsers'
							, 'Users'
							, ''
							, 'SELECT @ID = a.UserID'
							+ ' FROM [fmAudit].[tblUsers] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityUserToSite'
							, 'Site - User'
							, 'Users'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN u.UserID IS NULL THEN ua.UserID ELSE u.UserID END'
							+ ' FROM  [fmaudit].[map_tblEntityUserToSite] a'
							+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblArchivedUsers'
							, 'Archived Users'
							, ''
							, 'SELECT @ID = a.UserID'
							+ ' FROM [fmAudit].[tblArchivedUsers] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblGroups'
							, 'User Groups'
							, ''
							, 'SELECT @ID = a.GroupID'
							+ ' FROM [fmAudit].[tblGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityUserGroupToSite'
							, 'Site - User Group'
							, 'User Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblEntityUserGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblUserToGroup'
							, 'User Group - User'
							, 'User Groups'
							, 'SELECT @ID = CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END + '' - '''
							+ ' + CASE WHEN u.UserID IS NULL THEN ua.UserID ELSE u.UserID END'
							+ ' FROM  [fmaudit].[map_tblUserToGroup] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblGroupToRight'
							, 'User Group - Right'
							, 'User Groups'
							, 'SELECT @ID = CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.RightName)'
							+ ' FROM  [fmaudit].[map_tblGroupToRight] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblRight] l ON l.RightIndex = a.LookupRightIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyCompanyToUserGroup'
							, 'User Group - Company'
							, 'User Groups'
							, 'SELECT @ID = CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END + '' - '''
							+ ' + CASE WHEN a.CompanyGuid IS NULL THEN ''{All}'' WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblCompanyCompanyToUserGroup] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblCompanies'
							, 'Companies'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblCompanies] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyToRole'
							, 'Company - Role'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.CompanyRoleName)'
							+ ' FROM  [fmaudit].[map_tblCompanyToRole] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblCompanyRole] l ON l.CompanyRoleIndex = a.LookupCompanyRoleIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToCompany'
							, 'Company - Product'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.AssignedToCompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToSupplierProductCompany'
							, 'Company - Product'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToSupplierProductCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.AssignedToCompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToUnavailableInventoryCompany'
							, 'Company - Unavailable Inventory'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToUnavailableInventoryCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.AssignedToCompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblScheduleCompanyAccess'
							, 'Company - Schedule'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.DayOfWeekName)'
							+ ' FROM [fmAudit].[tblScheduleCompanyAccess] a' 
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblDayOfWeek] l ON l.DayOfWeekIndex = a.LookupDayOfWeekIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyAuthorizedCarrierToCompany'
							, 'Company - Authorized Carrier'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c1.ID IS NULL THEN ca1.ID ELSE c1.ID END + '' - '''
							+ ' + CASE WHEN c2.ID IS NULL THEN ca2.ID ELSE c2.ID END'
							+ ' FROM [fmAudit].[map_tblCompanyAuthorizedCarrierToCompany] a' 
							+ ' LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.AssignedToCompanyGuid AND ca1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = a.CompanyGuid AND ca2._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationCompanyCertificateAndPermitToCompany'
							, 'Company - Certificate and Permit'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN c1.ID IS NULL THEN ca1.ID ELSE c1.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationCompanyCertificateAndPermitToCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityCompanyToSite'
							, 'Site - Company'
							, 'Companies'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyToSite] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblMaintenanceReasons'
							, 'Maintenance Reasons'
							,''
							,'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblMaintenanceReasons] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTankMaintenanceLog'
							, 'Tank - Maintenance'
							,''
							,'SELECT @ID = a.TankID + '' - '' + a.MaintenanceReason'
							+ ' FROM [fmAudit].[tblTankMaintenanceLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblEquipmentMaintenanceLog'
							, 'Equipment - Maintenance'
							,''
							,'SELECT @ID = a.EquipmentID + '' - '' + a.MaintenanceReason'
							+ ' FROM [fmAudit].[tblEquipmentMaintenanceLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblEquipment'
							,'Equipment'
							,''
							,'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblEquipment] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableEquipment'
							, 'Equipment - Process Variable'
							, 'Equipment'
							,'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableEquipment] a' 
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationEquipmentTestAndInspectionToEquipment'
							, 'Equipment - Test and Inspection'
							, 'Equipment'
							, 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationEquipmentTestAndInspectionToEquipment] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationEquipmentTagAndLicenseToEquipment'
							, 'Equipment - Tag and License'
							, 'Equipment'
							, 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationEquipmentTagAndLicenseToEquipment] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEquipmentToSite'
							, 'Site - Equipment'
							, 'Equipment'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentToSite] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblEquipmentTypes'
							, 'Equipment Type'
							, ''
							, 'SELECT @ID = a.EqTypeName'
							+ ' FROM [fmAudit].[tblEquipmentTypes] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonTrainingToEquipmentType'
							, 'Equipment Type - Required Training'
							, 'Equipment Types'
							, 'SELECT @ID = CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonQualificationToEquipmentType'
							,'Equipment Type - Required Qualifications'
							,'Equipment Types'
							, 'SELECT @ID = CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonQualificationToEquipmentType] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAirplaneTank'
							, 'Equipment Type - Aircraft Tank'
							, 'Equipment Types'
							, 'SELECT @ID = CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END + '' - '''
							+ ' + a.Alias'
							+ ' FROM  [fmaudit].[tblAirplaneTank] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEquipmentTypeToSite'
							, 'Site - Equipment Type'
							, 'Equipment Types'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentTypeToSite] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblConfigurationSetting'
							, 'Configuration Settings'
							, 'Configuration Settings'
							, 'SELECT @ID = SettingKey'
							+ ' FROM [fmAudit].[tblConfigurationSetting] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSettings'
							, 'Enterprise Export/Import Settings'
							, 'Configuration Settings'
							, 'SELECT @ID = SettingID'
							+ ' FROM [fmAudit].[tblSettings] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSystemSettings'
							, 'System Settings'
							, 'System Settings'
							, 'SELECT @ID = ''N/A'''
							+ ' FROM [fmAudit].[tblSystemSettings] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblPersonnel'
							, 'Personnel'
							, ''
							, 'SELECT @ID = PersonID'
							+ ' FROM [fmAudit].[tblPersonnel] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSchedulePersonnelAccess'
							, 'Person - Schedule'
							, 'Personnel'
							, 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.DayOfWeekName)'
							+ ' FROM [fmAudit].[tblSchedulePersonnelAccess] a' 
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblDayOfWeek] l ON l.DayOfWeekIndex = a.LookupDayOfWeekIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblPersonnelToRole'
							,'Personnel - Role'
							, 'Personnel'
							, 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.PersonnelRoleName)'
							+ ' FROM  [fmaudit].[map_tblPersonnelToRole] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblPersonnelRole] l ON l.PersonnelRoleIndex = a.LookupPersonnelRoleIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonLicenseToPerson'
							, 'Personnel - License'
							, 'Personnel'
							, 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonLicenseToPerson] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonQualificationToPerson'
							, 'Personnel - Qualifications'
							, 'Personnel'
							, 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonQualificationToPerson] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonTrainingToPerson'
							, 'Personnel - Training'
							, 'Personnel'
							, 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonTrainingToPerson] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityPersonnelToSite'
							, 'Site - Person'
							, 'Personnel'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelToSite] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSites'
							, 'Sites'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblSites] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSitesAncillaryData'
							, 'Site Ancillary Data'
							, 'Sites'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM [fmAudit].[tblSitesAncillaryData] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableSite'
							, 'Site - Process Variable'
							, 'Sites'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableSite] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblScheduleTerminalOperation'
							, 'Site - Schedule'
							, 'Sites'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.DayOfWeekName)'
							+ ' FROM [fmAudit].[tblScheduleTerminalOperation] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblDayOfWeek] l ON l.DayOfWeekIndex = a.LookupDayOfWeekIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblScheduleHoliday'
							, 'Site - Holiday'
							, 'Sites'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CONVERT(NVARCHAR(20),a.HolidayDate,101)'
							+ ' FROM [fmAudit].[tblScheduleHoliday] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionAliases'
							, 'Transaction Aliases'
							, ''
							, 'SELECT @ID = AliasName'
							+ ' FROM [fmAudit].[tblTransactionAliases]'
							+ ' WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblTransactionAliasToStatus'
							, 'Transaction Alias - Status'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.TransactionStatusName)'
							+ ' FROM [fmAudit].[map_tblTransactionAliasToStatus] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblTransactionStatus] l ON l.TransactionStatusIndex = a.LookupTransactionStatusIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionAliasFields'
							, 'Transaction Alias - Fields'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + a.DisplayName'
							+ ' FROM [fmAudit].[tblTransactionAliasFields] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldTransactionAlias'
							, 'Transaction Alias - User Data Fields'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldTransactionAlias] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueTransactionAlias'
							, 'Transaction Alias - User Data'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueTransactionAlias] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldTransactionAlias] ud ON ud.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAlias] uda ON uda.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid AND uda._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = ud.TransactionAliasGuid OR t.TransactionAliasGuid = uda.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = uda.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldTransactionAliasLineItem'
							, 'Transaction Alias - Line Item User Data Fields'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldTransactionAliasLineItem] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueTransactionAliasLineItem'
							, 'Transaction Alias - Line Item User Data'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueTransactionAliasLineItem] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] ud ON ud.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAliasLineItem] uda ON uda.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid AND uda._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = ud.TransactionAliasGuid OR t.TransactionAliasGuid = uda.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = uda.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblGroupToTransactionAlias'
							, 'Transaction Alias - User Group'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM [fmAudit].[map_tblGroupToTransactionAlias] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToTransactionAliasExclusion'
							, 'Transaction Alias - Product Exclusion'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToTransactionAliasExclusion] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.AssignedToTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.AssignedToTransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblAssociatedTransactionAliases'
							, 'Transaction Alias - Associated Alias'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN t1.AliasName IS NULL THEN t1a.AliasName ELSE t1.AliasName END + '' - '''
							+ ' + CASE WHEN t2.AliasName IS NULL THEN t2a.AliasName ELSE t2.AliasName END'
							+ ' FROM [fmAudit].[map_tblAssociatedTransactionAliases] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t1 ON t1.TransactionAliasGuid = a.ParentTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] t1a ON t1a.TransactionAliasGuid = a.ParentTransactionAliasGuid AND t1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t2 ON t2.TransactionAliasGuid = a.ChildTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] t2a ON t2a.TransactionAliasGuid = a.ChildTransactionAliasGuid AND t2a._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityTransactionAliasToSite'
							, 'Site - Transaction Alias'
							, 'Transaction Aliases'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' FROM  [fmaudit].[map_tblEntityTransactionAliasToSite] a'
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProducts'
							, 'Products'
							, ''
							, 'SELECT @ID = ProductID'
							+ ' FROM [fmAudit].[tblProducts] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToBlendComponent'
							, 'Product Blend - Component'
							, 'Products'
							, 'SELECT @ID = CASE WHEN b.ProductID IS NULL THEN ba.ProductID ELSE b.ProductID END + '' - '''
							+ ' + CASE WHEN c.ProductID IS NULL THEN ca.ProductID ELSE c.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToBlendComponent] a' 
							+ ' LEFT JOIN [dbo].[tblProducts] b ON b.ProductGuid = a.AssignedToProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] ba ON ba.ProductGuid = a.AssignedToProductGuid  AND ba._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] c ON c.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] ca ON ca.ProductGuid = a.ProductGuid  AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToDotHazardous'
							, 'Product - Dot Hazardous Message'
							, 'Products'
							, 'SELECT @ID = CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + CASE WHEN aps.ID IS NULL THEN apsa.ID ELSE aps.ID END'
							+ ' FROM [fmAudit].[map_tblApplicationStringToDotHazardous] a' 
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON ca.ProductGuid = a.ProductGuid  AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] aps ON aps.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] apsa ON apsa.ApplicationStringGuid = a.ApplicationStringGuid  AND apsa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToProductMessage'
							, 'Product - Product Message'
							, 'Products'
							, 'SELECT @ID = CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + CASE WHEN aps.ID IS NULL THEN apsa.ID ELSE aps.ID END'
							+ ' FROM [fmAudit].[map_tblApplicationStringToProductMessage] a' 
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid  AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] aps ON aps.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] apsa ON apsa.ApplicationStringGuid = a.ApplicationStringGuid  AND apsa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityProductToSite'
							, 'Site - Product'
							, 'Products'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblEntityProductToSite] a'
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblStations'
							, 'Stations'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblStations] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '		
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableStation'
							, 'Station - Process Variable'
							, 'Station'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonQualificationToStation'
							, 'Station - Required Qualifications'
							, 'Station'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM [fmAudit].[map_tblQualificationPersonQualificationToStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationPersonTrainingToStation'
							, 'Station - Required Training'
							, 'Station'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM [fmAudit].[map_tblQualificationPersonTrainingToStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQualificationEquipmentTestAndInspectionToStation'
							, 'Station - Required Training'
							, 'Station'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM [fmAudit].[map_tblQualificationEquipmentTestAndInspectionToStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableStationOutputPermissive'
							, 'Station - Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableStationOutputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableStationInputPermissive'
							, 'Station - Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableStationInputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblLoadArms'
							, 'Station - Load Arm'
							, 'Station'
							, 'SELECT @ID = ISNULL(CASE WHEN a.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN a.BayAArmNumber IS NOT NULL AND a.BayBArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN a.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN a.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,a.BayAArmNumber) ELSE '''' END'
							+ ' + CASE WHEN a.BayAArmNumber IS NOT NULL AND a.BayBArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN a.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,a.BayBArmNumber) ELSE '''' END, '''')'
							+ ' FROM [fmAudit].[tblLoadArms] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = a.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = a.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = a.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = a.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableLoadArm'
							, 'Load Arm - Process Variable'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL) THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableLoadArm] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.LoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.LoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableLoadArmOutPutPermissive'
							, 'Load Arm - Arm Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL) THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableLoadArmOutPutPermissive] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.LoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.LoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableLoadArmInputPermissive'
							, 'Load Arm - Arm Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL) THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableLoadArmInputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.LoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.LoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableNoAdditiveOutputPermissive'
							, 'Load Arm - No Additive Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL) THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableNoAdditiveOutputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.LoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.LoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableNoAdditiveInputPermissive'
							, 'Load Arm - No Additive Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL) THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableNoAdditiveInputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.LoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.LoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToPresetExternalComponent'
							, 'Load Arm - External Component'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToPresetExternalComponent] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableExternalComponentOutputPermissive'
							, 'Load Arm - External Component Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableExternalComponentOutputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetExternalComponent] pc ON pc.ProductToPresetExternalComponentGuid = a.ProductToPresetExternalComponentGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetExternalComponent] pca ON pca.ProductToPresetExternalComponentGuid = a.ProductToPresetExternalComponentGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableExternalComponentInputPermissive'
							, 'Load Arm - External Component Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableExternalComponentInputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetExternalComponent] pc ON pc.ProductToPresetExternalComponentGuid = a.ProductToPresetExternalComponentGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetExternalComponent] pca ON pca.ProductToPresetExternalComponentGuid = a.ProductToPresetExternalComponentGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableExternalComponentBlendPercentage'
							, 'Load Arm - External Component Blend Percentage'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableExternalComponentBlendPercentage] a'
							+ ' LEFT JOIN [map].[tblProductToPresetExternalComponent] pc ON pc.ProductToPresetExternalComponentGuid = a.ProductToPresetExternalComponentGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetExternalComponent] pca ON pca.ProductToPresetExternalComponentGuid = a.ProductToPresetExternalComponentGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToPresetComponentTankOrTankGroup'
							, 'Load Arm - Component'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToPresetComponentTankOrTankGroup] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariableComponentOutputPermissive'
							, 'Load Arm - Component Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableComponentOutputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetComponentTankOrTankGroup] pc ON pc.ProductToPresetComponentTankOrTankGroupGuid = a.ProductToPresetComponentTankOrTankGroupGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetComponentTankOrTankGroup] pca ON pca.ProductToPresetComponentTankOrTankGroupGuid = a.ProductToPresetComponentTankOrTankGroupGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							, 'DECLARE @ProductToPresetComponentTankOrTankGroupGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetComponentTankOrTankGroupGuid = ProductToPresetComponentTankOrTankGroupGuid FROM [fmaudit].[tblProcessVariableComponentOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariableComponentInputPermissive'
							, 'Load Arm - Component Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableComponentInputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetComponentTankOrTankGroup] pc ON pc.ProductToPresetComponentTankOrTankGroupGuid = a.ProductToPresetComponentTankOrTankGroupGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetComponentTankOrTankGroup] pca ON pca.ProductToPresetComponentTankOrTankGroupGuid = a.ProductToPresetComponentTankOrTankGroupGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							, 'DECLARE @ProductToPresetComponentTankOrTankGroupGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetComponentTankOrTankGroupGuid = ProductToPresetComponentTankOrTankGroupGuid FROM [fmaudit].[tblProcessVariableComponentInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToPresetInjector'
							, 'Load Arm - Injector'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToPresetInjector] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariableAdditiveOutputPermissive'
							, 'Load Arm - Injector Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableAdditiveOutputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetInjector] pc ON pc.ProductToPresetInjectorGuid = a.ProductToPresetInjectorGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetInjector] pca ON pca.ProductToPresetInjectorGuid = a.ProductToPresetInjectorGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							, 'DECLARE @ProductToPresetInjectorGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetInjectorGuid = ProductToPresetInjectorGuid FROM [fmaudit].[tblProcessVariableAdditiveOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariableAdditiveInputPermissive'
							, 'Load Arm - Injector Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableAdditiveInputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetInjector] pc ON pc.ProductToPresetInjectorGuid = a.ProductToPresetInjectorGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetInjector] pca ON pca.ProductToPresetInjectorGuid = a.ProductToPresetInjectorGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							, 'DECLARE @ProductToPresetInjectorGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetInjectorGuid = ProductToPresetInjectorGuid FROM [fmaudit].[tblProcessVariableAdditiveInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariablePresetInjector'
							, 'Load Arm - Injector'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariablePresetInjector] a'
							+ ' LEFT JOIN [map].[tblProductToPresetInjector] pc ON pc.ProductToPresetInjectorGuid = a.ProductToPresetInjectorGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetInjector] pca ON pca.ProductToPresetInjectorGuid = a.ProductToPresetInjectorGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							, 	'DECLARE @ProductToPresetInjectorGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetInjectorGuid = ProductToPresetInjectorGuid FROM [fmaudit].[tblProcessVariablePresetInjector] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToPresetRecipe'
							, 'Load Arm - Recipe'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToPresetRecipe] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableRecipeOutputPermissive'
							, 'Load Arm - Recipe Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableRecipeOutputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetRecipe] pc ON pc.ProductToPresetRecipeGuid = a.ProductToPresetRecipeGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetRecipe] pca ON pca.ProductToPresetRecipeGuid = a.ProductToPresetRecipeGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableRecipeInputPermissive'
							, 'Load Arm - Recipe Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableRecipeInputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetRecipe] pc ON pc.ProductToPresetRecipeGuid = a.ProductToPresetRecipeGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetRecipe] pca ON pca.ProductToPresetRecipeGuid = a.ProductToPresetRecipeGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToPresetFlowControlledAdditive'
							, 'Load Arm - Flow Controlled Additive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END'
							+ ' + '' - '' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToPresetFlowControlledAdditive] a'
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = a.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = a.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariableFlowControlledAdditiveOutputPermissive'
							, 'Load Arm - Flow Controlled Additive Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetFlowControlledAdditive] pc ON pc.ProductToPresetFlowControlledAdditiveGuid = a.ProductToPresetFlowControlledAdditiveGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetFlowControlledAdditive] pca ON pca.ProductToPresetFlowControlledAdditiveGuid = a.ProductToPresetFlowControlledAdditiveGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
							, 'DECLARE @ProductToPresetFlowControlledAdditiveGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetFlowControlledAdditiveGuid = ProductToPresetFlowControlledAdditiveGuid FROM [fmaudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblProcessVariableFlowControlledAdditiveInputPermissive'
							, 'Load Arm - Flow Controlled Additive Permissive'
							, 'Station'
							, 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN l.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayAArmNumber)'
							+ ' WHEN la.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayAArmNumber)'
							+ ' ELSE '''' END'
							+ ' + CASE WHEN (l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL)'
							+ ' AND (l.BayBArmNumber IS NOT NULL OR la.BayBArmNumber IS NOT NULL)'
							+ ' THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN l.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,l.BayBArmNumber)'
							+ ' WHEN la.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,la.BayBArmNumber)'
							+ ' ELSE '''' END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableFlowControlledAdditiveInputPermissive] a'
							+ ' LEFT JOIN [map].[tblProductToPresetFlowControlledAdditive] pc ON pc.ProductToPresetFlowControlledAdditiveGuid = a.ProductToPresetFlowControlledAdditiveGuid'
							+ ' LEFT JOIN [fmAudit].[map_tblProductToPresetFlowControlledAdditive] pca ON pca.ProductToPresetFlowControlledAdditiveGuid = a.ProductToPresetFlowControlledAdditiveGuid AND pca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLoadArms] l ON l.LoadArmGuid = pc.AssignedToLoadArmGuid'
							+ ' LEFT JOIN [fmaudit].[tblLoadArms] la ON la.LoadArmGuid = pca.AssignedToLoadArmGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = l.BayAStationGuid OR s1.StationGuid = la.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = la.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = l.BayBStationGuid OR s2.StationGuid = la.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = la.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = pc.ProductGuid OR p.ProductGuid = pca.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON (pa.ProductGuid = pca.ProductGuid OR pa.ProductGuid = pc.ProductGuid) AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
							, 'DECLARE @ProductToPresetFlowControlledAdditiveGuid UNIQUEIDENTIFIER'
							+ '		,@LoadArmGuid UNIQUEIDENTIFIER'
							+ '		,@StationGuid UNIQUEIDENTIFIER;'
							+ 'SELECT @ProductToPresetFlowControlledAdditiveGuid = ProductToPresetFlowControlledAdditiveGuid FROM [fmaudit].[tblProcessVariableFlowControlledAdditiveInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
							+ 'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid AND _AuditEventType = ''D'';'
							+ 'IF @LoadArmGuid IS NULL'
							+ '	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid;'
							+ 'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'IF @StationGuid IS NULL'
							+ '	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
							+ 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
							+ 'IF @SiteGuid IS NULL'
							+ '	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTankGroups'
							, 'Tank Groups'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblTankGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblTankToTankGroup'
							, 'Tank Group - Tanks'
							, 'Tank Groups'
							, 'SELECT @ID = CASE WHEN tg.ID IS NULL THEN tga.ID ELSE tg.ID END + '' - '''
							+ ' + CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END'
							+ ' FROM [fmAudit].[map_tblTankToTankGroup] a'
							+ ' LEFT JOIN [dbo].[tblTankGroups] tg ON tg.TankGroupGuid = a.AssignedToTankGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblTankGroups] tga ON tga.TankGroupGuid = a.AssignedToTankGroupGuid AND tga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblGates'
							, 'Gates'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblGates] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTanks'
							, 'Tanks'
							, ''
							, 'SELECT @ID = TankID'
							+ ' FROM [fmAudit].[tblTanks] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblProcessVariableTank'
							, 'Tank - Process Variable'
							, 'Tanks'
							, 'SELECT @ID = CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableTank] a' 
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblOPCConnections'
							, 'OPC Connections'
							, ''
							, 'SELECT @ID = URL'
							+ ' FROM [fmAudit].[tblOPCConnections] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblMeter'
							,'Meters'
							, ''
							, 'SELECT @ID = MeterID'
							+ ' FROM [fmAudit].[tblMeter] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblMeterToTank'
							,'Tank - Meter'
							, 'Tanks'
							, 'SELECT @ID = CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + CASE WHEN m.MeterID IS NULL THEN ma.MeterID ELSE m.MeterID END'
							+ ' FROM [fmAudit].[map_tblMeterToTank] a'
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblMeter] m ON m.MeterGuid = a.MeterGuid'
							+ ' LEFT JOIN [fmaudit].[tblMeter] ma ON ma.MeterGuid = a.MeterGuid AND ma._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblApplicationString'
							, 'Application Strings'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblApplicationString] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityDotHazardousMessagesToSite'
							, 'Site - Dot Hazardous Message'
							, 'Dot Hazardous Messages'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityDotHazardousMessagesToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityProductMessageToSite'
							, 'Site - Product Message'
							, 'Product Messages'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityProductMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAllocationGroupToSite'
							, 'Site - Allocation Group'
							, 'Allocation Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAllocationGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityProductGroupToSite'
							, 'Site - Product Group'
							, 'Product Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityProductGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityCompanyTypeToSite'
							, 'Site - Company Type'
							, 'Company Types'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyTypeToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityFuelCardTypeToSite'
							, 'Site - Fuel Card Type'
							, 'Fuel Card Types'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFuelCardTypeToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)



	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAlarmAndEventCategoryToSite'
							, 'Site - Alarm Event Category'
							, 'Alarm Event Categories'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAlarmAndEventCategoryToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEmailAddressToSite'
							, 'Site - E-mail Address'
							, ''
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEmailAddressToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityCompanyGroupToSite'
							, 'Site - Company Group'
							, 'Company Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEntryMessageToSite'
							, 'Site - Entry Message'
							, 'Entry Messages'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEntryMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityExitMessageToSite'
							, 'Site - Exit Message'
							, 'Exit Messages'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityExitMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityProcessVariableMessageToSite'
							, 'Site - Process Variable Message'
							, 'Process Variable Messages'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityProcessVariableMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityFootNoteToSite'
							, 'Site - Footnote'
							, 'Footnotes'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFootNoteToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToFootNoteShipToState'
							, 'State - Footnote'
							, 'Footnotes'
							, 'SELECT @ID = CASE WHEN a.AssignedToApplicationStringGuid IS NULL THEN ''{All}'''
							+ ' WHEN p2.ID IS NULL THEN p2a.ID ELSE p2.ID END + '' - '''
							+ ' + CASE WHEN p1.ID IS NULL THEN p1a.ID ELSE p1.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipToState] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p1 ON p1.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] p1a ON p1a.ApplicationStringGuid = a.ApplicationStringGuid AND p1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] p2 ON p2.ApplicationStringGuid = a.AssignedToApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] p2a ON p2a.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND p2a._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToFootNoteShipTo'
							, 'Ship To - Footnote'
							, 'Footnotes'
							, 'SELECT @ID = CASE WHEN a.CompanyGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipTo] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToFootNoteShipper'
							, 'Shipper - Footnote'
							, 'Footnotes'
							, 'SELECT @ID = CASE WHEN a.CompanyGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipper] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToFootNoteProduct'
							, 'Product - Footnote'
							, 'Footnotes'
							, 'SELECT @ID = CASE WHEN a.ProductGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ProductID IS NULL THEN ca.ProductID ELSE c.ProductID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteProduct] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] c ON c.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] ca ON ca.ProductGuid = a.ProductGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblQualifications'
							, 'Qualifications'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblQualifications] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityCompanyCertificateAndPermitToSite'
							, 'Site - Company Certificate And Permit'
							, 'Company Certificates and Permits'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyCertificateAndPermitToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEquipmentTestAndInspectionToSite'
							, 'Site - Equipment Test and Inspection'
							, 'Equipment Tests and Inspections'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentTestAndInspectionToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityEquipmentTagAndLicenseToSite'
							, 'Site - Equipment Tag and License'
							, 'Equipment Tags and Licenses'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentTagAndLicenseToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityPersonnelQualificationToSite'
							, 'Site - Personnel Qualification'
							, 'Personnel Qualifications'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelQualificationToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityPersonnelLicenseToSite'
							, 'Site - Personnel License'
							, 'Personnel Licenses'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelLicenseToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityPersonnelTrainingToSite'
							, 'Site - Personnel Training'
							, 'Personnel Training'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelTrainingToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToProductGroup'
							, 'Product Group - Product'
							, 'Product Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToProductGroup] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToEntryMessage'
							, 'Product Group - Entry Message'
							, 'Product Groups'
							, 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToEntryMessage] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ProductGroupApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ProductGroupApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToExitMessage'
							, 'Product Group - Exit Message'
							, 'Product Groups'
							, 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToExitMessage] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ProductGroupApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ProductGroupApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyCompanyToCompanyGroup'
							, 'Company Group - Company'
							, 'Company Groups'
							, 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblCompanyCompanyToCompanyGroup] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToCompanyGroup'
							, 'Company Group - Product'
							, 'Company Groups'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToCompanyGroup] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAutoDistributionReasonCodes'
							, 'Auto Distribution Reason Code'
							, ''
							, 'SELECT @ID = a.ReasonCode'
							+ ' FROM [fmAudit].[tblAutoDistributionReasonCodes] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAutoDistributionReasonCodeToSite'
							, 'Site - Auto Distribution Reason Code'
							, 'Auto Distribution Reason Code'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN c.ReasonCode IS NULL THEN ca.ReasonCode ELSE c.ReasonCode END'
							+ ' FROM  [fmaudit].[map_tblEntityAutoDistributionReasonCodeToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAutoDistributionReasonCodes] c ON c.AutoDistributionReasonCodeGuid = a.AutoDistributionReasonCodeGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionReasonCodes] ca ON ca.AutoDistributionReasonCodeGuid = a.AutoDistributionReasonCodeGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAutoDistributionRule'
							, 'Auto Distribution Rule'
							, ''
							, 'SELECT @ID = a.RuleID'
							+ ' FROM [fmAudit].[tblAutoDistributionRule] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblManagerToAutoDistributionRule'
							, 'Auto Distribution Rule - Manager'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblManagerToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.ManagerGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.ManagerGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblManagerGroupToAutoDistributionRule'
							, 'Auto Distribution Rule - Manager Group'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblManagerGroupToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ManagerGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ManagerGroupGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToAutoDistributionRule'
							, 'Auto Distribution Rule - Product'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductGroupToAutoDistributionRule'
							, 'Auto Distribution Rule - Product Group'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblProductGroupToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ProductGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ProductGroupGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblTransactionAliasToAutoDistributionRule'
							, 'Auto Distribution Rule - Transaction Alias'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' FROM  [fmaudit].[map_tblTransactionAliasToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblOwnerToAutoDistributionRule'
							, 'Auto Distribution Rule - Owner'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblOwnerToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.OwnerGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.OwnerGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblOwnerGroupToAutoDistributionRule'
							, 'Auto Distribution Rule - Owner Group'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblOwnerGroupToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.OwnerGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.OwnerGroupGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityAutoDistributionRuleToSite'
							, 'Site - Auto Distribution Rule'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END'
							+ ' FROM  [fmaudit].[map_tblEntityAutoDistributionRuleToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblListViews'
							, 'List Views'
							, ''
							, 'SELECT @ID = CASE a.LookupListViewTypeIndex'
							+ ' WHEN 1 THEN CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' WHEN 2 THEN a.ID'
							+ ' WHEN 3 THEN a.ID END'
							+ ' FROM [fmAudit].[tblListViews] a'
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid AND a.LookupListViewTypeIndex = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND a.LookupListViewTypeIndex = 1 AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblListViewStandardType] l ON l.ListViewStandardTypeIndex = a.LookupListViewStandardTypeIndex AND a.LookupListViewTypeIndex = 2'  
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityListViewToSite'
							, 'Site - List View'
							, 'List Views'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN v.LookupListViewTypeIndex IS NULL'
							+ ' THEN CASE va.LookupListViewTypeIndex'
							+ ' WHEN 1 THEN CASE WHEN t2.AliasName IS NULL THEN ta2.AliasName ELSE t2.AliasName END'
							+ ' WHEN 2 THEN va.ID'
							+ ' WHEN 3 THEN va.ID END'
							+ ' ELSE CASE v.LookupListViewTypeIndex'
							+ ' WHEN 1 THEN CASE WHEN t1.AliasName IS NULL THEN ta1.AliasName ELSE t1.AliasName END'
							+ ' WHEN 2 THEN v.ID'
							+ ' WHEN 3 THEN v.ID END'
							+ ' END'
							+ ' FROM [fmAudit].[map_tblEntityListViewToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t1 ON t1.TransactionAliasGuid = v.TransactionAliasGuid AND v.LookupListViewTypeIndex = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta1 ON ta1.TransactionAliasGuid = v.TransactionAliasGuid AND v.LookupListViewTypeIndex = 1 AND ta1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t2 ON t2.TransactionAliasGuid = va.TransactionAliasGuid AND va.LookupListViewTypeIndex = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta2 ON ta2.TransactionAliasGuid = va.TransactionAliasGuid AND va.LookupListViewTypeIndex = 1 AND ta2._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblListViewStandardType] l1 ON l1.ListViewStandardTypeIndex = v.LookupListViewStandardTypeIndex AND v.LookupListViewTypeIndex = 2'  
							+ ' LEFT JOIN [lookup].[tblListViewStandardType] l2 ON l2.ListViewStandardTypeIndex = va.LookupListViewStandardTypeIndex AND va.LookupListViewTypeIndex = 2'  
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblListViewFields'
							, 'List View - List View Field'
							, ''
							, 'DECLARE @Type INT'
							+ ' SET @Type = (SELECT LookupListViewFieldTypeIndex FROM [fmAudit].[tblListViewFields]'
							+ ' WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID)'
							+ ' SELECT @ID = CASE WHEN v.LookupListViewTypeIndex IS NULL'
							+ ' THEN CASE va.LookupListViewTypeIndex'
							+ ' WHEN 1 THEN CASE WHEN t2.AliasName IS NULL THEN ta2.AliasName ELSE t2.AliasName END'
							+ ' WHEN 2 THEN va.ID'
							+ ' WHEN 3 THEN va.ID END'
							+ ' ELSE CASE v.LookupListViewTypeIndex'
							+ ' WHEN 1 THEN CASE WHEN t1.AliasName IS NULL THEN ta1.AliasName ELSE t1.AliasName END'
							+ ' WHEN 2 THEN v.ID'
							+ ' WHEN 3 THEN v.ID END'
							+ ' END  + '' - '''
							+ ' + CASE @Type'
							+ ' WHEN 1 THEN CASE WHEN t3.AliasName IS NULL THEN ta3.AliasName ELSE t3.AliasName END'
							+ ' WHEN 2 THEN CASE WHEN f.DisplayName IS NULL THEN fa.DisplayName ELSE f.DisplayName END'
							+ ' WHEN 3 THEN CASE WHEN u.DisplayName IS NULL THEN ua.DisplayName ELSE u.DisplayName END'
							+ ' WHEN 4 THEN dbo.udf_GetLowerWithInitUpperString(l3.StandardFieldTypeName)'
							+ ' WHEN 5 THEN CASE WHEN lu.DisplayName IS NULL THEN lua.DisplayName ELSE lu.DisplayName END'
							+ ' WHEN 6 THEN CASE WHEN la.ID IS NULL THEN laa.ID ELSE la.ID END'
							+ ' END'
							+ ' FROM [fmAudit].[tblListViewFields] a'
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t1 ON t1.TransactionAliasGuid = v.TransactionAliasGuid AND v.LookupListViewTypeIndex = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta1 ON ta1.TransactionAliasGuid = v.TransactionAliasGuid AND v.LookupListViewTypeIndex = 1 AND ta1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t2 ON t2.TransactionAliasGuid = va.TransactionAliasGuid AND va.LookupListViewTypeIndex = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta2 ON ta2.TransactionAliasGuid = va.TransactionAliasGuid AND va.LookupListViewTypeIndex = 1 AND ta2._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblListViewStandardType] l1 ON l1.ListViewStandardTypeIndex = v.LookupListViewStandardTypeIndex AND v.LookupListViewTypeIndex = 2'  
							+ ' LEFT JOIN [lookup].[tblListViewStandardType] l2 ON l2.ListViewStandardTypeIndex = va.LookupListViewStandardTypeIndex AND va.LookupListViewTypeIndex = 2'  
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t3 ON t3.TransactionAliasGuid = a.TransactionAliasGuid AND @Type = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta3 ON ta3.TransactionAliasGuid = a.TransactionAliasGuid AND @Type = 1 AND ta3._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliasFields] f ON f.TransactionAliasFieldGuid = a.TransactionAliasFieldGuid AND @Type = 2'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliasFields] fa ON fa.TransactionAliasFieldGuid = a.TransactionAliasFieldGuid AND @Type = 2 AND fa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblUserDataFieldTransactionAlias] u ON u.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid AND @Type = 3'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAlias] ua ON ua.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid AND @Type = 3 AND ua._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblStandardFieldType] l3 ON l3.StandardFieldTypeIndex = a.LookupStandardFieldTypeIndex AND @Type = 4'  
							+ ' LEFT JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] lu ON lu.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid AND @Type = 5'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAliasLineItem] lua ON lua.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid AND @Type = 5 AND lua._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLedgerAggregateColumns] la ON la.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND @Type = 6'
							+ ' LEFT JOIN [fmaudit].[tblLedgerAggregateColumns] laa ON laa.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND @Type = 6 AND laa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToLedgerView'
							, 'Ledger View - Product'
							, 'Ledger Views'
							, 'SELECT @ID = CASE WHEN v.ID IS NULL THEN va.ID ELSE v.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToLedgerView] a'
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.AssignedToListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.AssignedToListViewGuid AND va._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblGroupToLedgerView'
							, 'Ledger View - User Group'
							, 'Ledger Views'
							, 'SELECT @ID = CASE WHEN v.ID IS NULL THEN va.ID ELSE v.ID END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblGroupToLedgerView] a'
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityLedgerViewToSite'
							, 'Site - Ledger View'
							, 'Ledger Views'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN v.ID IS NULL THEN va.ID ELSE v.ID END'
							+ ' FROM [fmAudit].[map_tblEntityLedgerViewToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblGeneralConfiguration'
							, 'General Configuration'
							, ''
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM [fmAudit].[tblGeneralConfiguration] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblLedgerAggregateColumns'
							, 'Ledger Aggregate Column'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblLedgerAggregateColumns] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityLedgerAggregateColumnToSite'
							, 'Site - Ledger Aggregate Column'
							, 'Ledger Aggregate Column'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN l.ID IS NULL THEN la.ID ELSE l.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityLedgerAggregateColumnToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLedgerAggregateColumns] l ON l.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid'
							+ ' LEFT JOIN [fmaudit].[tblLedgerAggregateColumns] la ON la.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND la._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblLedgerAggregateColumnToTransactionAlias'
							, 'Auto Distribution Rule - Transaction Alias'
							, 'Auto Distribution Rule'
							, 'SELECT @ID = CASE WHEN l.ID IS NULL THEN la.ID ELSE l.ID END + '' - '''
							+ ' + CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' FROM  [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias] a'
							+ ' LEFT JOIN [dbo].[tblLedgerAggregateColumns] l ON l.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid'
							+ ' LEFT JOIN [fmaudit].[tblLedgerAggregateColumns] la ON la.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblIATA'
							, 'Delivery Locations'
							, ''
							, 'SELECT @ID = a.IATAID'
							+ ' FROM [fmAudit].[tblIATA] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityIATACodeToSite'
							, 'Site - Delivery Location'
							, 'Delivery Locations'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN i.IATAID IS NULL THEN ia.IATAID ELSE i.IATAID END'
							+ ' FROM  [fmaudit].[map_tblEntityIATACodeToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblIATA] i ON i.IATAGuid = a.IATAGuid'
							+ ' LEFT JOIN [fmaudit].[tblIATA] ia ON ia.IATAGuid = a.IATAGuid AND ia._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactions'
							, 'Transactions'
							, ''
							, 'SELECT @ID = a.TransID'
							+ ' FROM [fmAudit].[tblTransactions] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionUserData'
							, 'Transaction User Data'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END'
							+ ' FROM [fmAudit].[tblTransactionUserData] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionNotes'
							, 'Transaction Notes'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END'
							+ ' FROM [fmAudit].[tblTransactionNotes] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionSignature'
							, 'Transaction Signature'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END'
							+ ' FROM [fmAudit].[tblTransactionSignature] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionPIDX'
							, 'Transaction PIDX'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM [fmAudit].[tblTransactionPIDX] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblPIDXProfiles] p ON p.PIDXProfileGuid = a.PIDXProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblPIDXProfiles] pa ON pa.PIDXProfileGuid = a.PIDXProfileGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionWeightReadings'
							, 'Transaction Weight Readings'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + a.CompartmentID'
							+ ' FROM [fmAudit].[tblTransactionWeightReadings] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionLineItems'
							, 'Transaction Line Items'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + CONVERT(NVARCHAR,a.SequenceID+1)'
							+ ' FROM [fmAudit].[tblTransactionLineItems] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionLineItemUserData'
							, 'Transaction Line Item User Data'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN (CASE WHEN tla.TransID IS NULL THEN ta.TransID ELSE tla.TransID END) ELSE t.TransID END  + '' - '''
							+ ' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END'
							+ ' FROM [fmAudit].[tblTransactionLineItemUserData] a'
							+ ' LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = l.TransactionGuid'
							+ ' LEFT JOIN [dbo].[tblTransactions] tla ON tla.TransactionGuid = la.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = la.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionSubLineItems'
							, 'Transaction Sub Line Items'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN (CASE WHEN tla.TransID IS NULL THEN ta.TransID ELSE tla.TransID END) ELSE t.TransID END  + '' - '''
							+ ' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END + '' - '''
							+ ' + CONVERT(NVARCHAR,a.SequenceID+1)'
							+ ' FROM [fmAudit].[tblTransactionSubLineItems] a'
							+ ' LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [dbo].[tblTransactions] tla ON tla.TransactionGuid = la.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblTransactionTransportLineItems'
							, 'Transaction Transport Line Items'
							, 'Transactions'
							, 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + a.TransportOrderNumber'
							+ ' FROM [fmAudit].[tblTransactionTransportLineItems] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblCloseoutInventory'
							, 'Closeout'
							, ''
							, 'SELECT @ID = a.ManagerName + '' - '' + a.ProductName'
							+ ' FROM [fmAudit].[tblCloseoutInventory] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblOwnerCloseout'
							, 'Owner Closeout'
							, 'Closeout'
							, 'SELECT @ID = a.ManagerName + '' - '' + a.OwnerName + '' - '' + a.ProductName'
							+ ' FROM [fmAudit].[tblOwnerCloseout] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblReportGroups'
							, 'Report Groups'
							, 'Reports'
							, 'SELECT @ID = a.GroupName'
							+ ' FROM  [fmaudit].[tblReportGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblReportDetails'
							, 'Report Assignment'
							, 'Reports'
							, 'SELECT @ID = a.ReportName'
							+ ' FROM  [fmaudit].[tblReportDetails] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblGroupToReportDetail'
							, 'Report - User Group'
							, 'Reports'
							, 'SELECT @ID = CASE WHEN r.ReportName IS NULL THEN ra.ReportName ELSE r.ReportName END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblGroupToReportDetail] a'
							+ ' LEFT JOIN [dbo].[tblReportDetails] r ON r.ReportDetailGuid = a.ReportDetailGuid'
							+ ' LEFT JOIN [fmaudit].[tblReportDetails] ra ON ra.ReportDetailGuid = a.ReportDetailGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityReportConfigurationSettingsToSite'
							, 'Site - All Report Configuration'
							, 'Reports'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
							+ ' FROM  [fmaudit].[map_tblEntityReportConfigurationSettingsToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblQueryDefaults'
							, 'Query Settings'
							, ''
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[tblQueryDefaults] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblQueryDefaultFields'
							, 'Query Default Fields'
							, 'Query Settings'
							, 'SELECT @ID = REPLACE(REPLACE(a.Topic,''FMBusinessObjects.DataObjects.'',''''),''Class'','''') + '' - '' + a.FieldName'
							+ ' FROM  [fmaudit].[tblQueryDefaultFields] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityQuerySettingToSite'
							, 'Site - Query Settings'
							, 'Query Settings'
							, 'SELECT @ID = CASE WHEN s2.ID IS NULL THEN sa2.ID ELSE s2.ID END + '' - '''
							+ ' + CASE WHEN s1.ID IS NULL THEN sa1.ID ELSE s1.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityQuerySettingToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s1 ON s1.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa1 ON sa1.SiteGuid = a.SiteGuid AND sa1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s2 ON s2.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa2 ON sa2.SiteGuid = a.MapToSiteGuid AND sa2._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblQueryStorage'
							, 'Query Storage'
							, ''
							, 'SELECT @ID = a.QueryName'
							+ ' FROM  [fmaudit].[tblQueryStorage] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblQueryStorageToGroup'
							, 'Query Storage - User Group'
							, 'Query Storage'
							, 'SELECT @ID = CASE WHEN q.QueryName IS NULL THEN qa.QueryName ELSE q.QueryName END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblQueryStorageToGroup] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQueryStorage] q ON q.QueryStorageGuid = a.QueryStorageGuid'
							+ ' LEFT JOIN [fmaudit].[tblQueryStorage] qa ON qa.QueryStorageGuid = a.QueryStorageGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyLoadOwnerToManager'
							, 'Loading Hierarchy'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](0,a.CompanyLoadOwnerToManagerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyLoadOwnerToManager] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyShipperToOwner'
							, 'Loading Hierarchy'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](1,a.CompanyShipperToOwnerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyShipperToOwner] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyBillToToShipper'
							, 'Loading Hierarchy'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](2,a.CompanyBillToToShipperGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyBillToToShipper] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyShipToToBillTo'
							, 'Loading Hierarchy'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](3,a.CompanyShipToToBillToGuid)'
							+ ' FROM [fmaudit].[map_tblCompanyShipToToBillTo] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyPersonnelToShipToBillTo'
							, 'Loading Hierarchy - Load ID'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](3,a.CompanyShipToToBillToGuid) + '' - '''
							+ ' + a.ID'
							+ ' FROM  [fmaudit].[map_tblCompanyPersonnelToShipToBillTo] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyOffLoadOwnerToManager'
							, 'Off-Loading Hierarchy'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](4,a.CompanyOffLoadOwnerToManagerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyOffLoadOwnerToManager] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanySupplierToOwner'
							, 'Off-Loading Hierarchy'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](5,a.CompanySupplierToOwnerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanySupplierToOwner] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblCompanyPersonnelToSupplierOwner'
							, 'Off-Loading Hierarchy - Load ID'
							, 'Companies'
							, 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](5,a.CompanySupplierToOwnerGuid) + '' - '''
							+ ' + a.ID'
							+ ' FROM  [fmaudit].[map_tblCompanyPersonnelToSupplierOwner] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblHouseCards'
							, 'House Cards'
							, ''
							, 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblHouseCards] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSyncClientConfiguration'
							, 'Synchronization Settings'
							, ''
							, 'SELECT @ID = ''Client Settings'''
							+ ' FROM [fmAudit].[tblSyncClientConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblSyncServerConfiguration'
							, 'Synchronization Settings'
							, ''
							, 'SELECT @ID = ''Server'''
							+ ' FROM [fmAudit].[tblSyncServerConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldCompany'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Companies '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldCompany] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueCompany'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Companies '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueCompany] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldCompany] ud ON ud.UserDataFieldCompanyGuid = a.UserDataFieldCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldCompany] uda ON uda.UserDataFieldCompanyGuid = a.UserDataFieldCompanyGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldEquipment'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Equipment '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldEquipment] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueEquipment'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Equipment '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueEquipment] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldEquipment] ud ON ud.UserDataFieldEquipmentGuid = a.UserDataFieldEquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldEquipment] uda ON uda.UserDataFieldEquipmentGuid = a.UserDataFieldEquipmentGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldFuelCard'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''FuelCard '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldFuelCard] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueFuelCard'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''FuelCard '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueFuelCard] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldFuelCard] ud ON ud.UserDataFieldFuelCardGuid = a.UserDataFieldFuelCardGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldFuelCard] uda ON uda.UserDataFieldFuelCardGuid = a.UserDataFieldFuelCardGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldPersonnel'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Personnel '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldPersonnel] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValuePersonnel'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Personnel '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValuePersonnel] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldPersonnel] ud ON ud.UserDataFieldPersonnelGuid = a.UserDataFieldPersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldPersonnel] uda ON uda.UserDataFieldPersonnelGuid = a.UserDataFieldPersonnelGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldProduct'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Products '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldProduct] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueProduct'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Products '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueProduct] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldProduct] ud ON ud.UserDataFieldProductGuid = a.UserDataFieldProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldProduct] uda ON uda.UserDataFieldProductGuid = a.UserDataFieldProductGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataFieldSite'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Sites '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldSite] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblUserDataListValueSite'
							, 'User Data'
							, 'User Data'
							, 'SELECT @ID = ''Sites '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueSite] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldSite] ud ON ud.UserDataFieldSiteGuid = a.UserDataFieldSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldSite] uda ON uda.UserDataFieldSiteGuid = a.UserDataFieldSiteGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityUserDataToSite'
							, 'Site - User Data Configuration'
							, 'User Data'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
							+ ' FROM  [fmaudit].[map_tblEntityUserDataToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAllocations'
							, 'Allocations'
							, ''
							, 'SELECT @ID = CASE WHEN a.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,a.CompanyShipToToBillToGuid)'
							+ ' WHEN a.CompanyBillToToShipperGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](2,a.CompanyBillToToShipperGuid)'
							+ ' WHEN a.CompanyShipperToOwnerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](1,a.CompanyShipperToOwnerGuid)'
							+ ' WHEN a.CompanyLoadOwnerToManagerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](0,a.CompanyLoadOwnerToManagerGuid)'
							+ ' ELSE ''Invalid Allocation ID'''
							+ ' END'
							+ ' + '' - '' + CONVERT(NVARCHAR,a.EffectiveDate,101)' 
							+ ' FROM [fmAudit].[tblAllocations] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)


	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAllocationLineItems'
							, 'Allocation - LineItem'
							, 'Allocations'
							, 'SELECT @ID = CASE WHEN s.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,s.CompanyShipToToBillToGuid)'
							+ ' WHEN sa.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,sa.CompanyShipToToBillToGuid)'
							+ ' WHEN s.CompanyBillToToShipperGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](2,s.CompanyBillToToShipperGuid)'
							+ ' WHEN sa.CompanyBillToToShipperGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](2,sa.CompanyBillToToShipperGuid)'
							+ ' WHEN s.CompanyShipperToOwnerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](1,s.CompanyShipperToOwnerGuid)'
							+ ' WHEN sa.CompanyShipperToOwnerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](1,sa.CompanyShipperToOwnerGuid)'
							+ ' WHEN s.CompanyLoadOwnerToManagerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](0,s.CompanyLoadOwnerToManagerGuid)'
							+ ' WHEN sa.CompanyLoadOwnerToManagerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](0,sa.CompanyLoadOwnerToManagerGuid)'
							+ ' ELSE ''Invalid Allocation ID'''
							+ ' END  + '' - '''
							+ ' + CASE WHEN s.EffectiveDate IS NOT NULL THEN CONVERT(NVARCHAR,s.EffectiveDate,101)'
							+ ' WHEN sa.EffectiveDate IS NOT NULL THEN CONVERT(NVARCHAR,sa.EffectiveDate,101)'
							+ ' END + '' - '''
							+ ' + ISNULL(CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END,''Null Product'') + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ResetPeriodName)' 
							+ ' FROM [fmAudit].[tblAllocationLineItems] a'
							+ ' LEFT JOIN [dbo].[tblAllocations] s ON s.AllocationGuid = a.AllocationGuid'
							+ ' LEFT JOIN [fmaudit].[tblAllocations] sa ON sa.AllocationGuid = a.AllocationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.AssignedProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.AssignedProductGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblResetPeriod] l ON l.ResetPeriodIndex = a.LookupResetPeriodIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)
	
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingDevice'
							, 'Asset Tracking Device'
							, ''
							, 'SELECT @ID = a.DeviceID'
							+ ' FROM [fmAudit].[tblAssetTrackingDevice] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)
	
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingDetail'
							, 'Asset Tracking Detail'
							, ''
							, 'SELECT @ID = a.AssetTrackingDeviceID'
							+ ' FROM [fmAudit].[tblAssetTrackingDetail] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)
	
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingIconConfiguration'
							, 'Asset Tracking Icon Configuration'
							, ''
							, 'SELECT @ID = a.IconConfigurationID'
							+ ' FROM [fmAudit].[tblAssetTrackingIconConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)
	
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingMapConfiguration'
							, 'Asset Tracking Map Configuration'
							, ''
							, 'SELECT @ID = a.MapName'
							+ ' FROM [fmAudit].[tblAssetTrackingMapConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblVRUThresholds'
							, 'VRUThresholds'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblVRUThresholds] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
							, 'SELECT @SiteGuid = SiteGuid'
							+ ' FROM [fmaudit].[tblVRUThresholds] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblProductToVruTrackingConfig'
							, 'VRUThresholds - Product Tracking'
							, 'VRUThresholds'
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '' + '
							+ ' CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToVruTrackingConfig] a'
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.AssignedToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.AssignedToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToFootNoteAdditiveProfile'
							, 'Additive Profiles - Footnote'
							, 'Additive Profiles'
							, 'SELECT @ID = CASE WHEN a.ProductGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ProductID IS NULL THEN ca.ProductID ELSE c.ProductID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteAdditiveProfile] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] a2 ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND a2._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAdditiveProfiles] ap ON ap.AdditiveProfileGuid = a.AdditiveProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblAdditiveProfiles] a3 ON a3.AdditiveProfileGuid = a.AdditiveProfileGuid AND a3._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblFCEEMapping'
							, 'FCEE Mapping'
							, ''
							, 'SELECT @ID = 
CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '' 
+ CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - ''
+ TRIM(CASE WHEN d.ImeiNumber IS NULL THEN da.ImeiNumber ELSE d.ImeiNumber END) + '' - ''
+ m.EdgeMessageName + '' - '' 
+ ''Index:''+TRIM(CONVERT(NVARCHAR(3), a.[Index])) 
+ TRIM(CASE WHEN a.[Device] IS NULL THEN '''' ELSE '' - Device:'' + CONVERT(NVARCHAR(3), a.[Device]) END)
FROM  [fmaudit].[tblFCEEMapping] a 
LEFT JOIN [lookup].[tblEdgeMessage] m ON a.MsgType=m.EdgeMessageIndex
LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = a.PointGuid 
LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'' 
LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = a.FCEDeviceGuid 
LEFT JOIN [fmaudit].[tblFCEDevice] da ON da.FCEDeviceGuid = a.FCEDeviceGuid AND da._AuditEventType = ''D'' 
LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = p.SiteGuid 
LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = p.SiteGuid AND sa._AuditEventType = ''D'' 
WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
							, 'SELECT @SiteGuid = ISNULL(p.SiteGuid, pa.SiteGuid)
FROM  [fmaudit].[tblFCEEMapping] a 
LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = a.PointGuid 
LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'' 
WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblFCEDevice'
							, 'FCE Device'
							, ''
							, 'SELECT @ID = TRIM(a.ImeiNumber) FROM  [fmaudit].[tblFCEDevice] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
							, 'SELECT @SiteGuid = a.SiteGuid FROM  [fmaudit].[tblFCEDevice] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)

END


IF (SELECT COUNT(*) FROM [lookup].[tblActivationStatus]) = 0
BEGIN
	INSERT INTO [lookup].[tblActivationStatus](
				[ActivationStatusIndex],
				[ActivationStatusCode],
				[ActivationStatusName],
				[ActivationStatusGuid],
				[CreatedDate],
				[CreatedBy],
				[UpdatedDate],
				[UpdatedBy])
	VALUES	(0,'ACTIVE','ACTIVE','17D95A7C-DEB8-4C86-9066-F881A0F2B201',SYSDATETIME(),'Administrator',SYSDATETIME(),'Administrator'),
				(1,'INACTIVE','INACTIVE','428AD813-9F64-446D-8783-191E6D6B4CA8',SYSDATETIME(),'Administrator',SYSDATETIME(),'Administrator'),
				(2,'CANCELLED','CANCELLED','8CFD0977-904F-4D0F-AF5A-36A881AF6FC6',SYSDATETIME(),'Administrator',SYSDATETIME(),'Administrator')
END


IF (SELECT COUNT(*) FROM [lookup].[tblTimeZone])=0
BEGIN
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Dateline Standard Time', -720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Dateline Daylight Time', -720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'UTC-11', -660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Hawaiian Standard Time', -600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Hawaiian Daylight Time', -600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Alaskan Standard Time', -540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Alaskan Daylight Time', -480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pacific Standard Time (Mexico)', -480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pacific Daylight Time (Mexico)', -420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pacific Standard Time', -480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pacific Daylight Time', -420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'US Mountain Standard Time', -420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'US Mountain Daylight Time', -420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mountain Standard Time (Mexico)', -420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mountain Daylight Time (Mexico)', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mountain Standard Time', -420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mountain Daylight Time', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central America Standard Time', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central America Daylight Time', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Standard Time', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Daylight Time', -300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Standard Time (Mexico)', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Daylight Time (Mexico)', -300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Canada Central Standard Time', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Canada Central Daylight Time', -360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SA Pacific Standard Time', -300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SA Pacific Daylight Time', -300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Eastern Standard Time', -300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Eastern Daylight Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'US Eastern Standard Time', -300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'US Eastern Daylight Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Venezuela Standard Time', -270)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Venezuela Daylight Time', -270)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Paraguay Standard Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Paraguay Daylight Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Atlantic Standard Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Atlantic Daylight Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Brazilian Standard Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Brazilian Daylight Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SA Western Standard Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SA Western Daylight Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pacific SA Standard Time', -240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pacific SA Daylight Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Newfoundland Standard Time', -210)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Newfoundland Daylight Time', -150)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. South America Standard Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. South America Daylight Time', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Argentina Standard Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Argentina Daylight Time', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SA Eastern Standard Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SA Eastern Daylight Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Greenland Standard Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Greenland Daylight Time', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Montevideo Standard Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Montevideo Daylight Time', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Bahia Standard Time', -180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Bahia Daylight Time', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'UTC-02', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mid-Atlantic Standard Time', -120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mid-Atlantic Daylight Time', -60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Azores Standard Time', -60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Azores Daylight Time', 0)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Cape Verde Standard Time', -60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Cape Verde Daylight Time', -60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Morocco Standard Time', 0)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Morocco Daylight Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Coordinated Universal Time', 0)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'GMT Standard Time', 0)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'GMT Daylight Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Greenwich Standard Time', 0)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Greenwich Daylight Time', 0)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'W. Europe Standard Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'W. Europe Daylight Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Europe Standard Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Europe Daylight Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Romance Standard Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Romance Daylight Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central European Standard Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central European Daylight Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'W. Central Africa Standard Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'W. Central Africa Daylight Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Namibia Standard Time', 60)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Namibia Daylight Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'GTB Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'GTB Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Middle East Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Middle East Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Egypt Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Egypt Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Syria Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Syria Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. Europe Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. Europe Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'South Africa Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'South Africa Daylight Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'FLE Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'FLE Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Turkey Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Turkey Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Jerusalem Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Jerusalem Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Libya Standard Time', 120)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Libya Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Jordan Standard Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Jordan Daylight Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Arabic Standard Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Arabic Daylight Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Kaliningrad Standard Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Kaliningrad Daylight Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Arab Standard Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Arab Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. Africa Standard Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. Africa Daylight Time', 180)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Iran Standard Time', 210)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Iran Daylight Time', 270)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Arabian Standard Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Arabian Daylight Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Azerbaijan Standard Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Azerbaijan Daylight Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Russian Standard Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Russian Daylight Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mauritius Standard Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Mauritius Daylight Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Georgian Standard Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Georgian Daylight Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Caucasus Standard Time', 240)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Caucasus Daylight Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Afghanistan Standard Time', 270)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Afghanistan Daylight Time', 270)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'West Asia Standard Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'West Asia Daylight Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pakistan Standard Time', 300)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Pakistan Daylight Time', 360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'India Standard Time', 330)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'India Daylight Time', 330)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Sri Lanka Standard Time', 330)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Sri Lanka Daylight Time', 330)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Nepal Standard Time', 345)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Nepal Daylight Time', 345)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Asia Standard Time', 360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Asia Daylight Time', 360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Bangladesh Standard Time', 360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Bangladesh Daylight Time', 420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Ekaterinburg Standard Time', 360)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Ekaterinburg Daylight Time', 420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Myanmar Standard Time', 390)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Myanmar Daylight Time', 390)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SE Asia Standard Time', 420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'SE Asia Daylight Time', 420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'N. Central Asia Standard Time', 420)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'N. Central Asia Daylight Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'China Standard Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'China Daylight Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'North Asia Standard Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'North Asia Daylight Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Malay Peninsula Standard Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Malay Peninsula Daylight Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'W. Australia Standard Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'W. Australia Daylight Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Taipei Standard Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Taipei Daylight Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Ulaanbaatar Standard Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Ulaanbaatar Daylight Time', 480)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'North Asia East Standard Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'North Asia East Daylight Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Tokyo Standard Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Tokyo Daylight Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Korea Standard Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Korea Daylight Time', 540)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Cen. Australia Standard Time', 570)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Cen. Australia Daylight Time', 630)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'AUS Central Standard Time', 570)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'AUS Central Daylight Time', 570)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. Australia Standard Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'E. Australia Daylight Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'AUS Eastern Standard Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'AUS Eastern Daylight Time', 660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'West Pacific Standard Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'West Pacific Daylight Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Tasmania Standard Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Tasmania Daylight Time', 660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Yakutsk Standard Time', 600)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Yakutsk Daylight Time', 660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Pacific Standard Time', 660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Central Pacific Daylight Time', 660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Vladivostok Standard Time', 660)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Vladivostok Daylight Time', 720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'New Zealand Standard Time', 720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'New Zealand Daylight Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'UTC+12', 720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Fiji Standard Time', 720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Fiji Daylight Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Magadan Standard Time', 720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Magadan Daylight Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Kamchatka Standard Time', 720)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Kamchatka Daylight Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Tonga Standard Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Tonga Daylight Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Samoa Standard Time', 780)
	INSERT INTO [lookup].[tblTimeZone] ( TimeZoneName, OffsetMinutes) values(  'Samoa Daylight Time', 840)
END

IF (SELECT COUNT(*) FROM [lookup].[tblTransactionOrigin]) = 0
BEGIN
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(0, 'None', 'None', '0DE79099-50A5-4F98-9C06-F40147C2CB1B', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(1, 'Accounting', 'Accounting', '31AC42BC-0CFD-415D-A4DD-BCEC90D9EC2D', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(2, 'TerminalAutomationService', 'TerminalAutomationService', 'A5E63B4E-3193-4F44-95FE-3501EE9EB208', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(3, 'Dispatch', 'Dispatch', 'D589D799-D3CC-4846-AA01-CCF182F06A28', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(4, 'ADCUploadInterface', 'ADCUploadInterface', 'AB62E7F1-B16A-45B3-9643-EC6346F8CD9D', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(5, 'TransactionImportProcessor', 'TransactionImportProcessor', 'EC68A232-B6E7-42B8-91ED-A5D1AC2B63DC', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(6, 'TransactionImportProcessorV6', 'TransactionImportProcessorV6', '2CD6285E-290A-4D74-A930-EFDC6B91AB99', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(7, 'TransactionImportProcessorV6Update', 'TransactionImportProcessorV6Update', '24DF16CE-141B-4E75-994B-E8969A0E6DCF', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(8, 'ServiceRequestMessaging', 'ServiceRequestMessaging', '6BF3BA1B-88E9-42CF-883E-86C142CAD2F0', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(12, 'BaseLevelTransaction', 'BaseLevelTransaction', '334AC2CB-F1DB-4E86-B507-32AD0E013B9A', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(13, 'EnterpriseLevelTransaction', 'EnterpriseLevelTransaction', '48D43B32-C978-4674-AE70-06718E3FC6F0', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(14, 'AdcUploadedAtBaseLevel', 'AdcUploadedAtBaseLevel', '0892EEA2-4875-4DFE-9B86-AC57E6F2A77E', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(15, 'AdcUploadedAtEnterpriseLevel', 'AdcUploadedAtEnterpriseLevel', '47C8A3C0-2A09-48EC-A7B3-1EA13DCAD359', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator', '2014-03-19 14:31:03.1511955 -04:00', 'Adminstrator')
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(16, 'DispatchEnterprise', 'DispatchEnterprise', 'C1E77F1E-176D-4CB3-82AB-E8A4F0B3C2C4', '2015-09-03 14:31:03.1511955 -04:00', 'Adminstrator', '2015-09-03 14:31:03.1511955 -04:00', 'Adminstrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblFuelCardLimitPeriod]) = 0
BEGIN
	INSERT INTO lookup.tblFuelCardLimitPeriod (FuelCardLimitPeriodIndex, FuelCardLimitPeriodCode, FuelCardLimitPeriodName, FuelCardLimitPeriodGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (0, 'Day', 'Day', N'38bff932-cfcc-4a53-b126-c144d54c8a6d', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblFuelCardLimitPeriod (FuelCardLimitPeriodIndex, FuelCardLimitPeriodCode, FuelCardLimitPeriodName, FuelCardLimitPeriodGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (1, 'Week', 'Week', N'6b659d12-c50a-4e34-b896-70155a2150fa', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblFuelCardLimitPeriod (FuelCardLimitPeriodIndex, FuelCardLimitPeriodCode, FuelCardLimitPeriodName, FuelCardLimitPeriodGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (2, 'Month', 'Month', N'0788cee2-94a0-4bb8-946f-4bf3cc02bf1a', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblFuelCardLimitPeriod (FuelCardLimitPeriodIndex, FuelCardLimitPeriodCode, FuelCardLimitPeriodName, FuelCardLimitPeriodGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (3, 'Year', 'Year', N'e4b72b5f-6958-4d37-a2e1-9d318ab44938', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00')

	INSERT INTO lookup.tblFuelCardLimitPeriod (FuelCardLimitPeriodIndex, FuelCardLimitPeriodCode, FuelCardLimitPeriodName, FuelCardLimitPeriodGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (4, 'Transactional', 'Transactional', N'2898679f-346f-407f-a6ea-5fdd8d57fd96', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00')
END

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_CATALOG='tblSites') AND EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_CATALOG='tblSitesShadow')
BEGIN

	UPDATE dbo.tblSitesShadow SET
			dbo.tblSitesShadow.ID										= inserted.ID										
		,	dbo.tblSitesShadow.Number 									= inserted.Number 									
		,	dbo.tblSitesShadow.SPLCCode 								= inserted.SPLCCode 								
		,	dbo.tblSitesShadow.Address1 								= inserted.Address1 								
		,	dbo.tblSitesShadow.Address2 								= inserted.Address2 								
		,	dbo.tblSitesShadow.City 									= inserted.City 									
		,	dbo.tblSitesShadow.State 									= inserted.State 									
		,	dbo.tblSitesShadow.Zip 										= inserted.Zip 										
		,	dbo.tblSitesShadow.Country 									= inserted.Country 									
		,	dbo.tblSitesShadow.Phone 									= inserted.Phone 									
		,	dbo.tblSitesShadow.FAX 										= inserted.FAX 										
		,	dbo.tblSitesShadow.EmailAddress 							= inserted.EmailAddress 							
		,	dbo.tblSitesShadow.EmergencyContact 						= inserted.EmergencyContact 						
		,	dbo.tblSitesShadow.EmergencyPhone							= inserted.EmergencyPhone							
		,	dbo.tblSitesShadow.Enabled 									= inserted.Enabled 									
		,	dbo.tblSitesShadow.SiteGroupFlag 							= inserted.SiteGroupFlag 							
		,	dbo.tblSitesShadow.TimeZone 								= inserted.TimeZone 								
		,	dbo.tblSitesShadow.LevelUnitIndex							= inserted.LevelUnitIndex							
		,	dbo.tblSitesShadow.TemperatureUnitIndex						= inserted.TemperatureUnitIndex						
		,	dbo.tblSitesShadow.DensityUnitIndex							= inserted.DensityUnitIndex							
		,	dbo.tblSitesShadow.PressureUnitIndex						= inserted.PressureUnitIndex						
		,	dbo.tblSitesShadow.FlowUnitIndex							= inserted.FlowUnitIndex							
		,	dbo.tblSitesShadow.VolumeUnitIndex							= inserted.VolumeUnitIndex							
		,	dbo.tblSitesShadow.MassUnitIndex							= inserted.MassUnitIndex							
		,	dbo.tblSitesShadow.AdditiveVolumeUnitIndex					= inserted.AdditiveVolumeUnitIndex					
		,	dbo.tblSitesShadow.AdditiveProfileCycleAmountUnitIndex		= inserted.AdditiveProfileCycleAmountUnitIndex		
		,	dbo.tblSitesShadow.AdditiveProfileRateUnitIndex				= inserted.AdditiveProfileRateUnitIndex				
		,	dbo.tblSitesShadow.LevelDecimalPlaces						= inserted.LevelDecimalPlaces						
		,	dbo.tblSitesShadow.TemperatureDecimalPlaces					= inserted.TemperatureDecimalPlaces					
		,	dbo.tblSitesShadow.DensityDecimalPlaces						= inserted.DensityDecimalPlaces						
		,	dbo.tblSitesShadow.PressureDecimalPlaces					= inserted.PressureDecimalPlaces					
		,	dbo.tblSitesShadow.FlowDecimalPlaces						= inserted.FlowDecimalPlaces						
		,	dbo.tblSitesShadow.VolumeDecimalPlaces						= inserted.VolumeDecimalPlaces						
		,	dbo.tblSitesShadow.MassDecimalPlaces						= inserted.MassDecimalPlaces						
		,	dbo.tblSitesShadow.AdditiveVolumeDecimalPlaces				= inserted.AdditiveVolumeDecimalPlaces				
		,	dbo.tblSitesShadow.AdditiveProfileCycleAmountDecimalPlaces	= inserted.AdditiveProfileCycleAmountDecimalPlaces	
		,	dbo.tblSitesShadow.AdditiveProfileRateDecimalPlaces			= inserted.AdditiveProfileRateDecimalPlaces			
		,	dbo.tblSitesShadow.InhibitAccessAfterHours					= inserted.InhibitAccessAfterHours					
		,	dbo.tblSitesShadow.InhibitMultipleCardIns					= inserted.InhibitMultipleCardIns					
		,	dbo.tblSitesShadow.AccessCardInRequired						= inserted.AccessCardInRequired						
		,	dbo.tblSitesShadow.CheckSiteNumber							= inserted.CheckSiteNumber							
		,	dbo.tblSitesShadow.PromptForCustomerCard					= inserted.PromptForCustomerCard					
		,	dbo.tblSitesShadow.PromptForTractorOrTanker					= inserted.PromptForTractorOrTanker					
		,	dbo.tblSitesShadow.PromptForFirstTrailer					= inserted.PromptForFirstTrailer					
		,	dbo.tblSitesShadow.PromptForSecondTrailer					= inserted.PromptForSecondTrailer					
		,	dbo.tblSitesShadow.PromptForCompartment						= inserted.PromptForCompartment						
		,	dbo.tblSitesShadow.EnforceDriverEquipmentMatch				= inserted.EnforceDriverEquipmentMatch				
		,	dbo.tblSitesShadow.EnableAdditiveAccounting					= inserted.EnableAdditiveAccounting					
		,	dbo.tblSitesShadow.UseCompanyEquipmentIdentifiers			= inserted.UseCompanyEquipmentIdentifiers			
		,	dbo.tblSitesShadow.UseLastKnownGoodTankData					= inserted.UseLastKnownGoodTankData					
		,	dbo.tblSitesShadow.MaximumLoadAmount						= inserted.MaximumLoadAmount						
		,	dbo.tblSitesShadow.MaximumLoadTime							= inserted.MaximumLoadTime							
		,	dbo.tblSitesShadow.MaximumIdleTime							= inserted.MaximumIdleTime							
		,	dbo.tblSitesShadow.MaximumFlushAmount						= inserted.MaximumFlushAmount						
		,	dbo.tblSitesShadow.MaximumMeterProvingAmount				= inserted.MaximumMeterProvingAmount				
		,	dbo.tblSitesShadow.MaximumReturnsAmount						= inserted.MaximumReturnsAmount						
		,	dbo.tblSitesShadow.MaximumNumberOfActiveArms				= inserted.MaximumNumberOfActiveArms				
		,	dbo.tblSitesShadow.DriverTimeoutPeriod						= inserted.DriverTimeoutPeriod						
		,	dbo.tblSitesShadow.DriverWarningPeriod						= inserted.DriverWarningPeriod						
		,	dbo.tblSitesShadow.MaximumPrompts							= inserted.MaximumPrompts							
		,	dbo.tblSitesShadow.MaximumVehicleWeight						= inserted.MaximumVehicleWeight						
		,	dbo.tblSitesShadow.LoadByNet								= inserted.LoadByNet								
		,	dbo.tblSitesShadow.PromptForShipmentNumber					= inserted.PromptForShipmentNumber					
		,	dbo.tblSitesShadow.MaximumProductTemperature				= inserted.MaximumProductTemperature				
		,	dbo.tblSitesShadow.ListEquipment							= inserted.ListEquipment							
		,	dbo.tblSitesShadow.DeferStationChanges						= inserted.DeferStationChanges						
		,	dbo.tblSitesShadow.InhibitBOLWithBrokenBlends				= inserted.InhibitBOLWithBrokenBlends				
		,	dbo.tblSitesShadow.InhibitBOLWithImproperAdditization		= inserted.InhibitBOLWithImproperAdditization		
		,	dbo.tblSitesShadow.InhibitOverweightBOL						= inserted.InhibitOverweightBOL						
		,	dbo.tblSitesShadow.ExceptionBOLPrinter						= inserted.ExceptionBOLPrinter						
		,	dbo.tblSitesShadow.EnableAutomaticBOLPrinting				= inserted.EnableAutomaticBOLPrinting				
		,	dbo.tblSitesShadow.AutomaticBOLStartNumber					= inserted.AutomaticBOLStartNumber					
		,	dbo.tblSitesShadow.AutomaticBOLEndNumber					= inserted.AutomaticBOLEndNumber					
		,	dbo.tblSitesShadow.AutomaticBOLNextNumber					= inserted.AutomaticBOLNextNumber					
		,	dbo.tblSitesShadow.SeparateManualBOLNumbering				= inserted.SeparateManualBOLNumbering				
		,	dbo.tblSitesShadow.ManualBOLStartNumber						= inserted.ManualBOLStartNumber						
		,	dbo.tblSitesShadow.ManualBOLEndNumber						= inserted.ManualBOLEndNumber						
		,	dbo.tblSitesShadow.ManualBOLNextNumber						= inserted.ManualBOLNextNumber						
		,	dbo.tblSitesShadow.TransactionStartNumber					= inserted.TransactionStartNumber					
		,	dbo.tblSitesShadow.TransactionEndNumber						= inserted.TransactionEndNumber						
		,	dbo.tblSitesShadow.TransactionNextNumber					= inserted.TransactionNextNumber					
		,	dbo.tblSitesShadow.OrderStartNumber							= inserted.OrderStartNumber							
		,	dbo.tblSitesShadow.OrderEndNumber							= inserted.OrderEndNumber							
		,	dbo.tblSitesShadow.OrderNextNumber							= inserted.OrderNextNumber							
		,	dbo.tblSitesShadow.NumberPrefix								= inserted.NumberPrefix								
		,	dbo.tblSitesShadow.OpenTransactionWindow					= inserted.OpenTransactionWindow					
		,	dbo.tblSitesShadow.AdministrativeLockDate					= inserted.AdministrativeLockDate					
		,	dbo.tblSitesShadow.OperationalLockDate						= inserted.OperationalLockDate						
		,	dbo.tblSitesShadow.MaximumDaysToRetainLogs					= inserted.MaximumDaysToRetainLogs					
		,	dbo.tblSitesShadow.EnableDebugLogging						= inserted.EnableDebugLogging						
		,	dbo.tblSitesShadow.EnableAuditLogging						= inserted.EnableAuditLogging						
		,	dbo.tblSitesShadow.AutomaticallyPrintAlarmsAndEvents		= inserted.AutomaticallyPrintAlarmsAndEvents		
		,	dbo.tblSitesShadow.AlarmAndEventPrinter						= inserted.AlarmAndEventPrinter						
		,	dbo.tblSitesShadow.MailServer								= inserted.MailServer								
		,	dbo.tblSitesShadow.MailFrom									= inserted.MailFrom									
		,	dbo.tblSitesShadow.MailUserName								= inserted.MailUserName								
		,	dbo.tblSitesShadow.MailPassword								= inserted.MailPassword								
		,	dbo.tblSitesShadow.DialupName								= inserted.DialupName								
		,	dbo.tblSitesShadow.SCADASystem								= inserted.SCADASystem								
		,	dbo.tblSitesShadow.InhibitTemplateGraphics					= inserted.InhibitTemplateGraphics					
		,	dbo.tblSitesShadow.RefreshInterval							= inserted.RefreshInterval							
		,	dbo.tblSitesShadow.InhibitEndOfDayOperations				= inserted.InhibitEndOfDayOperations				
		,	dbo.tblSitesShadow.InhibitEndOfMonthOperations				= inserted.InhibitEndOfMonthOperations				
		,	dbo.tblSitesShadow.EndOfDayWarningPeriod					= inserted.EndOfDayWarningPeriod					
		,	dbo.tblSitesShadow.InhibitAutomaticPhysicalInventory		= inserted.InhibitAutomaticPhysicalInventory		
		,	dbo.tblSitesShadow.InhibitAutomaticMeterCloseout			= inserted.InhibitAutomaticMeterCloseout			
		,	dbo.tblSitesShadow.InhibitAutomaticReportGeneration			= inserted.InhibitAutomaticReportGeneration			
		,	dbo.tblSitesShadow.InhibitAutomaticAdjustmentDistribution	= inserted.InhibitAutomaticAdjustmentDistribution	
		,	dbo.tblSitesShadow.InhibitAutomaticCloseout					= inserted.InhibitAutomaticCloseout					
		,	dbo.tblSitesShadow.InhibitTankScan							= inserted.InhibitTankScan							
		,	dbo.tblSitesShadow.ReportDirectory							= inserted.ReportDirectory							
		,	dbo.tblSitesShadow.ManageReports							= inserted.ManageReports							
		,	dbo.tblSitesShadow.ManagedReportDirectory					= inserted.ManagedReportDirectory					
		,	dbo.tblSitesShadow.VRURateLimit								= inserted.VRURateLimit								
		,	dbo.tblSitesShadow.VRUHourlyLimit							= inserted.VRUHourlyLimit							
		,	dbo.tblSitesShadow.VRUDailyLimit							= inserted.VRUDailyLimit							
		,	dbo.tblSitesShadow.VRUYearlyLimit							= inserted.VRUYearlyLimit							
		,	dbo.tblSitesShadow.VRUCurrentYearLimit						= inserted.VRUCurrentYearLimit						
		,	dbo.tblSitesShadow.VRURateActual							= inserted.VRURateActual							
		,	dbo.tblSitesShadow.VRUHourlyActual							= inserted.VRUHourlyActual							
		,	dbo.tblSitesShadow.VRUDailyActual							= inserted.VRUDailyActual							
		,	dbo.tblSitesShadow.VRUYearlyActual							= inserted.VRUYearlyActual							
		,	dbo.tblSitesShadow.VRUCurrentYearActual						= inserted.VRUCurrentYearActual						
		,	dbo.tblSitesShadow.VRURateLimitEnabled						= inserted.VRURateLimitEnabled						
		,	dbo.tblSitesShadow.VRUHourlyLimitEnabled					= inserted.VRUHourlyLimitEnabled					
		,	dbo.tblSitesShadow.VRUDailyLimitEnabled						= inserted.VRUDailyLimitEnabled						
		,	dbo.tblSitesShadow.VRUYearlyLimitEnabled					= inserted.VRUYearlyLimitEnabled					
		,	dbo.tblSitesShadow.VRUCurrentYearLimitEnabled				= inserted.VRUCurrentYearLimitEnabled				
		,	dbo.tblSitesShadow.WatchdogPeriod							= inserted.WatchdogPeriod							
		,	dbo.tblSitesShadow.WatchdogCounterStart						= inserted.WatchdogCounterStart						
		,	dbo.tblSitesShadow.WatchdogCounterEnd						= inserted.WatchdogCounterEnd						
		,	dbo.tblSitesShadow.NumberDecimalSeparator					= inserted.NumberDecimalSeparator					
		,	dbo.tblSitesShadow.NumberGroupSeparator						= inserted.NumberGroupSeparator						
		,	dbo.tblSitesShadow.ListSeparator							= inserted.ListSeparator							
		,	dbo.tblSitesShadow.TimePattern								= inserted.TimePattern								
		,	dbo.tblSitesShadow.TimeSeparator							= inserted.TimeSeparator							
		,	dbo.tblSitesShadow.AMSymbol									= inserted.AMSymbol									
		,	dbo.tblSitesShadow.PMSymbol									= inserted.PMSymbol									
		,	dbo.tblSitesShadow.ShortDatePattern							= inserted.ShortDatePattern							
		,	dbo.tblSitesShadow.DateSeparator							= inserted.DateSeparator							
		,	dbo.tblSitesShadow.LongDatePattern							= inserted.LongDatePattern							
		,	dbo.tblSitesShadow.TwoDigitCalendarEndYear					= inserted.TwoDigitCalendarEndYear					
		,	dbo.tblSitesShadow.UserData1								= inserted.UserData1								
		,	dbo.tblSitesShadow.UserData2								= inserted.UserData2								
		,	dbo.tblSitesShadow.UserData3								= inserted.UserData3								
		,	dbo.tblSitesShadow.UserData4								= inserted.UserData4								
		,	dbo.tblSitesShadow.UserData5								= inserted.UserData5								
		,	dbo.tblSitesShadow.UserData6								= inserted.UserData6								
		,	dbo.tblSitesShadow.UserData7								= inserted.UserData7								
		,	dbo.tblSitesShadow.UserData8								= inserted.UserData8								
		,	dbo.tblSitesShadow.CreatedDate								= inserted.CreatedDate								
		,	dbo.tblSitesShadow.CreatedBy								= inserted.CreatedBy								
		,	dbo.tblSitesShadow.UpdatedDate								= inserted.UpdatedDate								
		,	dbo.tblSitesShadow.UpdatedBy								= inserted.UpdatedBy								
		,	dbo.tblSitesShadow.MinTimeAllowedToChangePwd				= inserted.MinTimeAllowedToChangePwd				
		,	dbo.tblSitesShadow.MinPwdCharacterLength					= inserted.MinPwdCharacterLength					
		,	dbo.tblSitesShadow.PwdExpirationInDays						= inserted.PwdExpirationInDays						
		,	dbo.tblSitesShadow.PwdLockoutThreshold						= inserted.PwdLockoutThreshold						
		,	dbo.tblSitesShadow.CheckForPreviousPwd						= inserted.CheckForPreviousPwd						
		,	dbo.tblSitesShadow.StrongPwdUse								= inserted.StrongPwdUse								
		,	dbo.tblSitesShadow.PwdHistoryCount							= inserted.PwdHistoryCount							
		,	dbo.tblSitesShadow.ApplyToAllSiteMembers					= inserted.ApplyToAllSiteMembers					
		,	dbo.tblSitesShadow.InactivityDisablePeriod					= inserted.InactivityDisablePeriod					
		,	dbo.tblSitesShadow.EnforceSingleOwner						= inserted.EnforceSingleOwner						
		,	dbo.tblSitesShadow.InhibitBOLSummaryAutoPopulate			= inserted.InhibitBOLSummaryAutoPopulate			
		,	dbo.tblSitesShadow.InhibitOrderSummaryAutoPopulate			= inserted.InhibitOrderSummaryAutoPopulate			
		,	dbo.tblSitesShadow.InhibitSupplyOrderSummaryAutoPopulate	= inserted.InhibitSupplyOrderSummaryAutoPopulate	
		,	dbo.tblSitesShadow.InvoiceStartNumber						= inserted.InvoiceStartNumber						
		,	dbo.tblSitesShadow.InvoiceEndNumber							= inserted.InvoiceEndNumber							
		,	dbo.tblSitesShadow.InvoiceNextNumber						= inserted.InvoiceNextNumber						
		,	dbo.tblSitesShadow.PromptForReturns							= inserted.PromptForReturns							
		,	dbo.tblSitesShadow.PromptForTruckCard						= inserted.PromptForTruckCard						
		,	dbo.tblSitesShadow.StartingShortCardNumber					= inserted.StartingShortCardNumber					
		,	dbo.tblSitesShadow.UseShortCardNumber						= inserted.UseShortCardNumber						
		,	dbo.tblSitesShadow.ExcessVarianceCount						= inserted.ExcessVarianceCount						
		,	dbo.tblSitesShadow.ExcessVarianceTolerance					= inserted.ExcessVarianceTolerance					
		,	dbo.tblSitesShadow.DisableArchivePeriod						= inserted.DisableArchivePeriod						
		,	dbo.tblSitesShadow.ExportArchiveDir							= inserted.ExportArchiveDir							
		,	dbo.tblSitesShadow.ImportArchiveDir							= inserted.ImportArchiveDir							
		,	dbo.tblSitesShadow.GroupLedgerByID							= inserted.GroupLedgerByID							
		,	dbo.tblSitesShadow.InhibitSiteLedgerRollup					= inserted.InhibitSiteLedgerRollup					
		,	dbo.tblSitesShadow.UseTankReconciliation					= inserted.UseTankReconciliation					
		,	dbo.tblSitesShadow.SiteGuid									= inserted.SiteGuid									
		,	dbo.tblSitesShadow.LookupNumberGroupSizesTypeIndex			= inserted.LookupNumberGroupSizesTypeIndex			
		,	dbo.tblSitesShadow.LookupQuantityDisplayDefaultIndex		= inserted.LookupQuantityDisplayDefaultIndex		
		,	dbo.tblSitesShadow.LookupSecondaryStorageFillMethodIndex	= inserted.LookupSecondaryStorageFillMethodIndex	
		,	dbo.tblSitesShadow.LookupMailConnectModeIndex				= inserted.LookupMailConnectModeIndex				
		,	dbo.tblSitesShadow.LookupWatchdogModeIndex					= inserted.LookupWatchdogModeIndex					
		,	dbo.tblSitesShadow.Contact1Name								= inserted.Contact1Name								
		,	dbo.tblSitesShadow.Contact1Address1							= inserted.Contact1Address1							
		,	dbo.tblSitesShadow.Contact1Address2							= inserted.Contact1Address2							
		,	dbo.tblSitesShadow.Contact1City								= inserted.Contact1City								
		,	dbo.tblSitesShadow.Contact1State							= inserted.Contact1State							
		,	dbo.tblSitesShadow.Contact1Zip								= inserted.Contact1Zip								
		,	dbo.tblSitesShadow.Contact1Country							= inserted.Contact1Country							
		,	dbo.tblSitesShadow.Contact1PhoneOffice						= inserted.Contact1PhoneOffice						
		,	dbo.tblSitesShadow.Contact1Fax								= inserted.Contact1Fax								
		,	dbo.tblSitesShadow.Contact1EmailAddress						= inserted.Contact1EmailAddress						
		,	dbo.tblSitesShadow.Contact2Name								= inserted.Contact2Name								
		,	dbo.tblSitesShadow.Contact2Address1							= inserted.Contact2Address1							
		,	dbo.tblSitesShadow.Contact2Address2							= inserted.Contact2Address2							
		,	dbo.tblSitesShadow.Contact2City								= inserted.Contact2City								
		,	dbo.tblSitesShadow.Contact2State							= inserted.Contact2State							
		,	dbo.tblSitesShadow.Contact2Zip								= inserted.Contact2Zip								
		,	dbo.tblSitesShadow.Contact2Country							= inserted.Contact2Country							
		,	dbo.tblSitesShadow.Contact2PhoneOffice						= inserted.Contact2PhoneOffice						
		,	dbo.tblSitesShadow.Contact2Fax								= inserted.Contact2Fax								
		,	dbo.tblSitesShadow.Contact2EmailAddress						= inserted.Contact2EmailAddress						
		,	dbo.tblSitesShadow.Contact1PhoneMobile						= inserted.Contact1PhoneMobile						
		,	dbo.tblSitesShadow.Contact2PhoneMobile						= inserted.Contact2PhoneMobile						
		,	dbo.tblSitesShadow.EnablePasswordHint						= inserted.EnablePasswordHint						
		,	dbo.tblSitesShadow.EnablePasswordReset						= inserted.EnablePasswordReset						
		,	dbo.tblSitesShadow.MeterReconciliationToleranceIsPercent	= inserted.MeterReconciliationToleranceIsPercent	
		,	dbo.tblSitesShadow.MeterReconciliationReportName			= inserted.MeterReconciliationReportName			
		,	dbo.tblSitesShadow.TranslatedHelpURL						= inserted.TranslatedHelpURL						
		,	dbo.tblSitesShadow.AllowUseOfSpecialChars					= inserted.AllowUseOfSpecialChars					
		,	dbo.tblSitesShadow.EnablePeriodicSyncFlag					= inserted.EnablePeriodicSyncFlag
		,	dbo.tblSitesShadow.PeriodicSyncIntervalMinutes			= inserted.PeriodicSyncIntervalMinutes	
		,	dbo.tblSitesShadow.DisableSyncTransferFlag				= inserted.DisableSyncTransferFlag					
		,	dbo.tblSitesShadow.DeletedDate								= NULL				
	FROM 
	(
		SELECT s.* FROM [dbo].[tblSites] s
		INNER JOIN [dbo].[tblSitesShadow] sw
		ON s.SiteGuid = sw.SiteGuid
	)inserted
	WHERE dbo.tblSitesShadow.SiteGuid = inserted.SiteGuid

	INSERT INTO [dbo].tblSitesShadow WITH(ROWLOCK)(
			[ID]
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
		,	[SiteGuid]
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
		,	[DisableSyncTransferFlag]
		)
		SELECT 
			i.[ID]
		,	i.[Number]
		,	i.[SPLCCode]
		,	i.[Address1]
		,	i.[Address2]
		,	i.[City]
		,	i.[State]
		,	i.[Zip]
		,	i.[Country]
		,	i.[Phone]
		,	i.[FAX]
		,	i.[EmailAddress]
		,	i.[EmergencyContact]
		,	i.[EmergencyPhone]
		,	i.[Enabled]
		,	i.[SiteGroupFlag]
		,	i.[TimeZone]
		,	i.[LevelUnitIndex]
		,	i.[TemperatureUnitIndex]
		,	i.[DensityUnitIndex]
		,	i.[PressureUnitIndex]
		,	i.[FlowUnitIndex]
		,	i.[VolumeUnitIndex]
		,	i.[MassUnitIndex]
		,	i.[AdditiveVolumeUnitIndex]
		,	i.[AdditiveProfileCycleAmountUnitIndex]
		,	i.[AdditiveProfileRateUnitIndex]
		,	i.[LevelDecimalPlaces]
		,	i.[TemperatureDecimalPlaces]
		,	i.[DensityDecimalPlaces]
		,	i.[PressureDecimalPlaces]
		,	i.[FlowDecimalPlaces]
		,	i.[VolumeDecimalPlaces]
		,	i.[MassDecimalPlaces]
		,	i.[AdditiveVolumeDecimalPlaces]
		,	i.[AdditiveProfileCycleAmountDecimalPlaces]
		,	i.[AdditiveProfileRateDecimalPlaces]
		,	i.[InhibitAccessAfterHours]
		,	i.[InhibitMultipleCardIns]
		,	i.[AccessCardInRequired]
		,	i.[CheckSiteNumber]
		,	i.[PromptForCustomerCard]
		,	i.[PromptForTractorOrTanker]
		,	i.[PromptForFirstTrailer]
		,	i.[PromptForSecondTrailer]
		,	i.[PromptForCompartment]
		,	i.[EnforceDriverEquipmentMatch]
		,	i.[EnableAdditiveAccounting]
		,	i.[UseCompanyEquipmentIdentifiers]
		,	i.[UseLastKnownGoodTankData]
		,	i.[MaximumLoadAmount]
		,	i.[MaximumLoadTime]
		,	i.[MaximumIdleTime]
		,	i.[MaximumFlushAmount]
		,	i.[MaximumMeterProvingAmount]
		,	i.[MaximumReturnsAmount]
		,	i.[MaximumNumberOfActiveArms]
		,	i.[DriverTimeoutPeriod]
		,	i.[DriverWarningPeriod]
		,	i.[MaximumPrompts]
		,	i.[MaximumVehicleWeight]
		,	i.[LoadByNet]
		,	i.[PromptForShipmentNumber]
		,	i.[MaximumProductTemperature]
		,	i.[ListEquipment]
		,	i.[DeferStationChanges]
		,	i.[InhibitBOLWithBrokenBlends]
		,	i.[InhibitBOLWithImproperAdditization]
		,	i.[InhibitOverweightBOL]
		,	i.[ExceptionBOLPrinter]
		,	i.[EnableAutomaticBOLPrinting]
		,	i.[AutomaticBOLStartNumber]
		,	i.[AutomaticBOLEndNumber]
		,	i.[AutomaticBOLNextNumber]
		,	i.[SeparateManualBOLNumbering]
		,	i.[ManualBOLStartNumber]
		,	i.[ManualBOLEndNumber]
		,	i.[ManualBOLNextNumber]
		,	i.[TransactionStartNumber]
		,	i.[TransactionEndNumber]
		,	i.[TransactionNextNumber]
		,	i.[OrderStartNumber]
		,	i.[OrderEndNumber]
		,	i.[OrderNextNumber]
		,	i.[NumberPrefix]
		,	i.[OpenTransactionWindow]
		,	i.[AdministrativeLockDate]
		,	i.[OperationalLockDate]
		,	i.[MaximumDaysToRetainLogs]
		,	i.[EnableDebugLogging]
		,	i.[EnableAuditLogging]
		,	i.[AutomaticallyPrintAlarmsAndEvents]
		,	i.[AlarmAndEventPrinter]
		,	i.[MailServer]
		,	i.[MailFrom]
		,	i.[MailUserName]
		,	i.[MailPassword]
		,	i.[DialupName]
		,	i.[SCADASystem]
		,	i.[InhibitTemplateGraphics]
		,	i.[RefreshInterval]
		,	i.[InhibitEndOfDayOperations]
		,	i.[InhibitEndOfMonthOperations]
		,	i.[EndOfDayWarningPeriod]
		,	i.[InhibitAutomaticPhysicalInventory]
		,	i.[InhibitAutomaticMeterCloseout]
		,	i.[InhibitAutomaticReportGeneration]
		,	i.[InhibitAutomaticAdjustmentDistribution]
		,	i.[InhibitAutomaticCloseout]
		,	i.[InhibitTankScan]
		,	i.[ReportDirectory]
		,	i.[ManageReports]
		,	i.[ManagedReportDirectory]
		,	i.[VRURateLimit]
		,	i.[VRUHourlyLimit]
		,	i.[VRUDailyLimit]
		,	i.[VRUYearlyLimit]
		,	i.[VRUCurrentYearLimit]
		,	i.[VRURateActual]
		,	i.[VRUHourlyActual]
		,	i.[VRUDailyActual]
		,	i.[VRUYearlyActual]
		,	i.[VRUCurrentYearActual]
		,	i.[VRURateLimitEnabled]
		,	i.[VRUHourlyLimitEnabled]
		,	i.[VRUDailyLimitEnabled]
		,	i.[VRUYearlyLimitEnabled]
		,	i.[VRUCurrentYearLimitEnabled]
		,	i.[WatchdogPeriod]
		,	i.[WatchdogCounterStart]
		,	i.[WatchdogCounterEnd]
		,	i.[NumberDecimalSeparator]
		,	i.[NumberGroupSeparator]
		,	i.[ListSeparator]
		,	i.[TimePattern]
		,	i.[TimeSeparator]
		,	i.[AMSymbol]
		,	i.[PMSymbol]
		,	i.[ShortDatePattern]
		,	i.[DateSeparator]
		,	i.[LongDatePattern]
		,	i.[TwoDigitCalendarEndYear]
		,	i.[UserData1]
		,	i.[UserData2]
		,	i.[UserData3]
		,	i.[UserData4]
		,	i.[UserData5]
		,	i.[UserData6]
		,	i.[UserData7]
		,	i.[UserData8]
		,	i.[CreatedDate]
		,	i.[CreatedBy]
		,	i.[UpdatedDate]
		,	i.[UpdatedBy]
		,	i.[MinTimeAllowedToChangePwd]
		,	i.[MinPwdCharacterLength]
		,	i.[PwdExpirationInDays]
		,	i.[PwdLockoutThreshold]
		,	i.[CheckForPreviousPwd]
		,	i.[StrongPwdUse]
		,	i.[PwdHistoryCount]
		,	i.[ApplyToAllSiteMembers]
		,	i.[InactivityDisablePeriod]
		,	i.[EnforceSingleOwner]
		,	i.[InhibitBOLSummaryAutoPopulate]
		,	i.[InhibitOrderSummaryAutoPopulate]
		,	i.[InhibitSupplyOrderSummaryAutoPopulate]
		,	i.[InvoiceStartNumber]
		,	i.[InvoiceEndNumber]
		,	i.[InvoiceNextNumber]
		,	i.[PromptForReturns]
		,	i.[PromptForTruckCard]
		,	i.[StartingShortCardNumber]
		,	i.[UseShortCardNumber]
		,	i.[ExcessVarianceCount]
		,	i.[ExcessVarianceTolerance]
		,	i.[DisableArchivePeriod]
		,	i.[ExportArchiveDir]
		,	i.[ImportArchiveDir]
		,	i.[GroupLedgerByID]
		,	i.[InhibitSiteLedgerRollup]
		,	i.[UseTankReconciliation]
		,	i.[SiteGuid]
		,	i.[LookupNumberGroupSizesTypeIndex]
		,	i.[LookupQuantityDisplayDefaultIndex]
		,	i.[LookupSecondaryStorageFillMethodIndex]
		,	i.[LookupMailConnectModeIndex]
		,	i.[LookupWatchdogModeIndex]
		,	i.[Contact1Name]
		,	i.[Contact1Address1]
		,	i.[Contact1Address2]
		,	i.[Contact1City]
		,	i.[Contact1State]
		,	i.[Contact1Zip]
		,	i.[Contact1Country]
		,	i.[Contact1PhoneOffice]
		,	i.[Contact1Fax]
		,	i.[Contact1EmailAddress]
		,	i.[Contact2Name]
		,	i.[Contact2Address1]
		,	i.[Contact2Address2]
		,	i.[Contact2City]
		,	i.[Contact2State]
		,	i.[Contact2Zip]
		,	i.[Contact2Country]
		,	i.[Contact2PhoneOffice]
		,	i.[Contact2Fax]
		,	i.[Contact2EmailAddress]
		,	i.[Contact1PhoneMobile]
		,	i.[Contact2PhoneMobile]
		,	i.[EnablePasswordHint]
		,	i.[EnablePasswordReset]
		,	i.[MeterReconciliationToleranceIsPercent]
		,	i.[MeterReconciliationReportName]
		,	i.[TranslatedHelpURL]
		,	i.[AllowUseOfSpecialChars]
		,	i.[EnablePeriodicSyncFlag]
		,	i.[PeriodicSyncIntervalMinutes]
		,	i.[DisableSyncTransferFlag]
		FROM 
		(
			SELECT s.* FROM [dbo].[tblSites] s WHERE s.SiteGuid NOT IN (SELECT SiteGuid FROM [dbo].[tblSitesShadow])
		) i

END

--IF OBJECT_ID('FK_tblAuditLog_SiteGuid', 'F') IS NULL 
--BEGIN
--	ALTER TABLE [dbo].[tblAuditLog] ADD CONSTRAINT FK_tblAuditLog_ShadowSiteGuid FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSitesShadow] ([SiteGuid])
--END

-- INSERT RECORD FOR <Air> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Air')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Air', 'AIR',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <AirExpress> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'AirExpress')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'AirExpress', 'AIREXPRESS',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Barge> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Barge')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Barge', 'BARGE',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <BestWayShippersOption> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'BestWayShippersOption')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'BestWayShippersOption', 'BESTWAYSHIPPERSOPTION',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Boat> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Boat')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Boat', 'BOAT',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <CustomerPickup> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'CustomerPickup')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'CustomerPickup', 'CUSTOMERPICKUP',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <ExpeditedTruck> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'ExpeditedTruck')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'ExpeditedTruck', 'EXPEDITEDTRUCK',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <MotorCommonCarrier> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'MotorCommonCarrier')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'MotorCommonCarrier', 'MOTORCOMMONCARRIER',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Ocean> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Ocean')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Ocean', 'OCEAN',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Other> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Other')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Other', 'OTHER',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <ParcelPost> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'ParcelPost')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'ParcelPost', 'PARCELPOST',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Pipeline> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Pipeline')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Pipeline', 'PIPELINE',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Rail Car> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Rail Car')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Rail Car', 'RAIL',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Railcar> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Railcar')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Railcar', 'RAIL',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <SupplierTruck> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'SupplierTruck')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'SupplierTruck', 'SUPPLIERTRUCK',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Tank> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Tank')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Tank', 'OTHER',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Tank Transfer> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Tank Transfer')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Tank Transfer', 'OTHER',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Truck> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Truck')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Truck', 'TRUCK',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

-- INSERT RECORD FOR <Vessel> int dbo.tblExportTransportModeMapping TABLE
IF NOT EXISTS (Select FMATransportMode from dbo.tblExportTransportModeMapping where FMATransportMode = 'Vessel')
BEGIN
INSERT INTO dbo.tblExportTransportModeMapping (ExportTransportModeMappingGuid,FMATransportMode,FuelPlusTransportMode,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
VALUES (newid(),'Vessel', 'VESSEL',SYSDATETIMEOFFSET(),'Varec',SYSDATETIMEOFFSET(),'Varec')
END

