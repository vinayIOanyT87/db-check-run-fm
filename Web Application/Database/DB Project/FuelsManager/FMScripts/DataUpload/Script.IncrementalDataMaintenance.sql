/*************************************************'
* Script.IncrementalDataMaintenance.sql file
* Use this file for include scripts for:
* 1. Insert data into a table that already has data (e.g. new entry into an already populated lookup table). It is required that the insert script verifies whether the inserting record does not exist).
* 2. Update the content of a record(s) present in a table
* 3. Delete records from a table 
**************************************************/
IF EXISTS (SELECT * FROM tblConfigurationSetting WHERE SettingKey = 'Server_Time_Zone')
BEGIN
	DELETE FROM tblConfigurationSetting WHERE SettingKey = 'Server_Time_Zone'
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ReportTimeZone') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'SZ', 'ReportTimeZone', null, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'Cassandra_ConsistencyLevel') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'SZ', 'Cassandra_ConsistencyLevel', 'One', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'Cassandra_ReplicationFactor') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'DWORD', 'Cassandra_ReplicationFactor', 1, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'Cassandra_Configuration') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('f68b3f63-f0f4-4e56-8039-72b54b20903a', 'MULTI_SZ', 'Cassandra_Configuration', N'127.0.0.1', N'2015-08-20 08:00:00 AM +00:00', 'Administrator', N'2015-08-20 08:00:00 AM +00:00', 'Administrator')
END
ELSE BEGIN
	UPDATE tblConfigurationSetting SET KeyType = 'MULTI_SZ' WHERE ConfigurationSettingGuid = 'f68b3f63-f0f4-4e56-8039-72b54b20903a'
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'Cassandra_Username') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('B3357A4B-47E9-4E3E-8F73-43278A47446A', 'MULTI_SZ', 'Cassandra_Username', N'cassandra', N'2015-08-20 08:00:00 AM +00:00', 'Administrator', N'2015-08-20 08:00:00 AM +00:00', 'Administrator')
END
ELSE BEGIN
	UPDATE tblConfigurationSetting SET KeyType = 'MULTI_SZ' WHERE ConfigurationSettingGuid = 'B3357A4B-47E9-4E3E-8F73-43278A47446A'
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'Cassandra_Password') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('AFCCE7B9-D73A-435C-903C-A30AEED75BA3', N'PWD', 'Cassandra_Password', N'AN0Wex01HLpSIfuLxb6grA==', N'2015-08-20 08:00:00 AM +00:00', 'Administrator', N'2015-08-20 08:00:00 AM +00:00', 'Administrator')
END
ELSE BEGIN
	UPDATE tblConfigurationSetting SET KeyType = 'PWD' WHERE ConfigurationSettingGuid = 'AFCCE7B9-D73A-435C-903C-A30AEED75BA3'
END

if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = N'AuditEnabled')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
	VALUES (NEWID(), N'DWORD', N'AuditEnabled', N'0', SYSDATETIMEOFFSET(),'Administrator',SYSDATETIMEOFFSET(),'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'BSME_TransactionTimeoutSeconds') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'DWORD', 'BSME_TransactionTimeoutSeconds', 1, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'BSME_ScanFrequencySeconds')=0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'DWORD', 'BSME_ScanFrequencySeconds', 1, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'BSME_MaxExpressBatch') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'DWORD', 'BSME_MaxExpressBatch', 200, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'QueryWriterAssemblies')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] 
	([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
	VALUES 
	(N'B099F194-D1D7-43A5-87E7-7FE54D7FC3F1', N'MULTI_SZ', N'QueryWriterAssemblies', 'FMBusinessObjects', N'05/10/2020 12:00:01 AM -04:00', N'Varec', N'05/10/2020 12:00:01 AM -04:00', N'Varec')	
END

IF NOT EXISTS (SELECT * FROM tblConfigurationSetting WHERE SettingKey = 'FMAETranslationsConfigurationSiteGroup')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] 
	(
		[ConfigurationSettingGuid], 
		[KeyType], 
		[SettingKey], 
		[SettingValue], 
		[CreatedDate], 
		[CreatedBy], 
		[UpdatedDate], 
		[UpdatedBy]
	) 
	VALUES 
	(
		N'8F377F06-6AD4-4BC7-A059-3CD9E37D1D1D', 
		N'SZ',
		N'FMAETranslationsConfigurationSiteGroup',
		N'SiteAdmin',
		N'01/14/2014 9:41:27 AM -05:00',
		N'Administrator',
		N'01/14/2014 9:41:27 AM -05:00',
		N'Administrator'
	)
END

IF NOT EXISTS (SELECT * FROM tblConfigurationSetting WHERE SettingKey = 'SingleSignOnMode')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] 
	(
		[ConfigurationSettingGuid], 
		[KeyType], 
		[SettingKey], 
		[SettingValue], 
		[CreatedDate], 
		[CreatedBy], 
		[UpdatedDate], 
		[UpdatedBy]
	) 
	VALUES 
	(
		N'30DAB17F-6CC3-4C04-9196-6EDA5EAB600B', 
		N'DWORD',
		N'SingleSignOnMode',
		N'0',
		N'12/12/2022 9:41:27 AM -05:00',
		N'Administrator',
		N'12/12/2022 9:41:27 AM -05:00',
		N'Administrator'
	)
END

IF NOT EXISTS (SELECT * FROM tblConfigurationSetting WHERE SettingKey = 'SynchronizedSettings')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] 
	(
		[ConfigurationSettingGuid], 
		[KeyType], 
		[SettingKey], 
		[SettingValue], 
		[CreatedDate], 
		[CreatedBy], 
		[UpdatedDate], 
		[UpdatedBy]
	) 
	VALUES 
	(
		N'1878C3B4-942F-4459-89D0-664D4F896AD9', 
		N'MULTI_SZ',
		N'SynchronizedSettings',
		N'SynchronizedSettings;PointCalculatorRowVisibilityConfig',
		N'12/12/2022 9:41:27 AM -05:00',
		N'Administrator',
		N'12/12/2022 9:41:27 AM -05:00',
		N'Administrator'
	)
END
ELSE
BEGIN
	Update [dbo].[tblConfigurationSetting] 
	Set SettingValue = CASE 
		WHEN SettingValue IS NOT NULL 
			AND RTRIM(SettingValue) != '' 
			AND SettingValue NOT LIKE '%PointCalculatorRowVisibilityConfig%' 
			AND SettingValue NOT LIKE '%;'
		THEN SettingValue + ';PointCalculatorRowVisibilityConfig'
		WHEN SettingValue IS NOT NULL 
			AND RTRIM(SettingValue) != '' 
			AND SettingValue NOT LIKE '%PointCalculatorRowVisibilityConfig%' 
			AND SettingValue LIKE '%;'
		THEN SettingValue + 'PointCalculatorRowVisibilityConfig'
		WHEN SettingValue IS NULL OR RTRIM(SettingValue) = ''
		THEN 'SynchronizedSettings;PointCalculatorRowVisibilityConfig'
		ELSE SettingValue
	END
	WHERE SettingKey = 'SynchronizedSettings'
END

IF EXISTS (SELECT * FROM tblConfigurationSetting WHERE ConfigurationSettingGuid = 'fcc5fe6c-fa80-4636-924e-d5ec702cd7e5')
BEGIN
	DELETE FROM tblConfigurationSetting WHERE ConfigurationSettingGuid = 'fcc5fe6c-fa80-4636-924e-d5ec702cd7e5'
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'UseNewTransactionAliasConfigScreen') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(NEWID(), 'DWORD', 'UseNewTransactionAliasConfigScreen', '0', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MaximumOperateSessions_Enterprise') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('5487ae1a-0681-11ef-b8d2-103d1cbd9c45', 'DWORD', 'MaximumOperateSessions_Enterprise', '10', N'4/29/2024 5:00:00 PM -04:00', 'Administrator', N'4/29/2024 5:00:00 PM -04:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MaximumOperateSessions_Terminal') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('5487ae1b-0681-11ef-b8d2-103d1cbd9c45', 'DWORD', 'MaximumOperateSessions_Terminal', '10', N'4/29/2024 5:00:00 PM -04:00', 'Administrator', N'4/29/2024 5:00:00 PM -04:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'OperateTagRefreshInterval') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('e3961ec0-274f-11ef-b8f1-103d1cbd9c42', 'DWORD', 'OperateTagRefreshInterval', '1', N'6/10/2024 5:00:00 PM -04:00', 'Administrator', N'4/29/2024 5:00:00 PM -04:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'OperateAlarmRefreshInterval') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('e3961ec1-274f-11ef-b8f1-103d1cbd9c42', 'DWORD', 'OperateAlarmRefreshInterval', '1', N'6/10/2024 5:00:00 PM -04:00', 'Administrator', N'4/29/2024 5:00:00 PM -04:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'MovementNotifyAssembly') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('77abd09c-d223-424e-816c-03a1fc41a734', N'SZ', 'MovementNotifyAssembly', '', N'10/16/2025 5:00:00 PM -04:00', 'Administrator', N'10/16/2025 5:00:00 PM -04:00', 'Administrator')
END



IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 1035 OR MenuItemTypeCode = 'ACCOUNTING_MAIN_TRANSACTION_SUMMARY') = 0)
BEGIN
	INSERT INTO lookup.tblMenuItemType
		(MenuItemTypeIndex, MenuItemTypeCode, MenuItemTypeName, MenuItemTypeGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(1035, N'ACCOUNTING_MAIN_TRANSACTION_SUMMARY', N'ACCOUNTING_MAIN_TRANSACTION_SUMMARY', N'C55DD6FA-CD62-4FE8-BC3D-8B9E43A3AABB', N'3/03/2014 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END

IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 2013 OR MenuItemTypeCode = 'ADMIN_SYSTEM_CONFIGURATION_SETTINGS') = 0)
BEGIN
	INSERT INTO lookup.tblMenuItemType
		(MenuItemTypeIndex, MenuItemTypeCode, MenuItemTypeName, MenuItemTypeGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(2013, N'ADMIN_SYSTEM_CONFIGURATION_SETTINGS', N'ADMIN_SYSTEM_CONFIGURATION_SETTINGS', N'6454E897-A2F3-4D1F-860A-1D1C0099D134', N'3/03/2014 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END

IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4053 OR MenuItemTypeCode = 'CONFIG_IMPORT_EXPORT_FUEL_CARD_IMPORT') = 0)
BEGIN
	INSERT INTO lookup.tblMenuItemType
		(MenuItemTypeIndex, MenuItemTypeCode, MenuItemTypeName, MenuItemTypeGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(4053, 'CONFIG_IMPORT_EXPORT_FUEL_CARD_IMPORT', 'CONFIG_IMPORT_EXPORT_FUEL_CARD_IMPORT', N'BADCF6CD-FFF3-4E21-BFED-10CCF9ABBB9C', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator', '2014-05-01 00:00:00.0000000 -04:00', 'Administrator')
END
ELSE
BEGIN
    IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE (MenuItemTypeIndex = 4053 OR MenuItemTypeCode = 'CONFIG_IMPORT_EXPORT_FUEL_CARD_IMPORT') AND MenuItemTypeGuid = N'BADCF6CD-FFF3-4E21-BFED-10CCF9ABBB9C') = 0)
    BEGIN
        UPDATE lookup.tblMenuItemType SET MenuItemTypeGuid = N'BADCF6CD-FFF3-4E21-BFED-10CCF9ABBB9C'
                                            ,CreatedDate = '2014-05-01 00:00:00.0000000 -04:00'
                                            ,UpdatedDate = '2014-05-01 00:00:00.0000000 -04:00'
                                        WHERE MenuItemTypeIndex = 4053 OR MenuItemTypeCode = 'CONFIG_IMPORT_EXPORT_FUEL_CARD_IMPORT'
    END
END

-- menu items for Inventory Management
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4069 AND MenuItemTypeGuid = 'E8C35889-7D66-4247-B053-930F4508F4CC')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = 'E8C35889-7D66-4247-B053-930F4508F4CC'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4069 AND MenuItemTypeGuid = 'E8C35889-7D66-4247-B053-930F4508F4CC')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4069, N'CONFIG_INVMGR_POINT_TEMPLATE_TYPE', N'E8C35889-7D66-4247-B053-930F4508F4CC'
	END
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4070 AND MenuItemTypeGuid = 'E7677E07-0A71-4943-AA50-A0C1387E9849')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = 'E7677E07-0A71-4943-AA50-A0C1387E9849'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4070 AND MenuItemTypeGuid = 'E7677E07-0A71-4943-AA50-A0C1387E9849')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4070, N'CONFIG_INVMGR_POINT_TEMPLATES', N'E7677E07-0A71-4943-AA50-A0C1387E9849'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4071 AND MenuItemTypeGuid = '0EF37D65-B17E-4E81-8B87-7020A29CF556')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '0EF37D65-B17E-4E81-8B87-7020A29CF556'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4071 AND MenuItemTypeGuid = '0EF37D65-B17E-4E81-8B87-7020A29CF556')
	BEGIN
			EXEC [lookup].[AddMenuItemType] 4071, N'CONFIG_INVMGR_DRAW', N'0EF37D65-B17E-4E81-8B87-7020A29CF556'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4072 AND MenuItemTypeGuid = '214E509D-6D85-47B1-B735-333CAB3A9AE9')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '214E509D-6D85-47B1-B735-333CAB3A9AE9'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4072 AND MenuItemTypeGuid = '214E509D-6D85-47B1-B735-333CAB3A9AE9')
	BEGIN
			EXEC [lookup].[AddMenuItemType] 4072, N'CONFIG_INVMGR_PICTURESUMMARY', N'214E509D-6D85-47B1-B735-333CAB3A9AE9'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4073 AND MenuItemTypeGuid = '0be1678b-4330-4863-91d9-a0a6540f0555')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '0be1678b-4330-4863-91d9-a0a6540f0555'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4073 AND MenuItemTypeGuid = '0be1678b-4330-4863-91d9-a0a6540f0555')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4073, N'CONFIG_INVMGR_POINT_CATEGORY', N'0be1678b-4330-4863-91d9-a0a6540f0555'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4074 AND MenuItemTypeGuid = '590787c3-8dca-4010-9b61-0873d813743b')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '590787c3-8dca-4010-9b61-0873d813743b'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4074 AND MenuItemTypeGuid = '590787c3-8dca-4010-9b61-0873d813743b')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4074, N'CONFIG_INVMGR_DRAW_PROTO', N'590787c3-8dca-4010-9b61-0873d813743b'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4075 AND MenuItemTypeGuid = '1db47749-c5f2-4cd2-823a-fcdd57593332')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '1db47749-c5f2-4cd2-823a-fcdd57593332'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4075 AND MenuItemTypeGuid = '1db47749-c5f2-4cd2-823a-fcdd57593332')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4075, N'CONFIG_INVMGR_DRAW_PLATFORM', N'1db47749-c5f2-4cd2-823a-fcdd57593332'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4076 AND MenuItemTypeGuid = '27772525-32FD-4F93-BA1A-0687147AC894')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '27772525-32FD-4F93-BA1A-0687147AC894'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4076 AND MenuItemTypeGuid = '27772525-32FD-4F93-BA1A-0687147AC894')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4076, N'CONFIG_INVMGR_POINTS', N'27772525-32FD-4F93-BA1A-0687147AC894'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4077 AND MenuItemTypeGuid = '100115bf-c300-4ea9-bccd-22a1539fccc5')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '100115bf-c300-4ea9-bccd-22a1539fccc5'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4077 AND MenuItemTypeGuid = '100115bf-c300-4ea9-bccd-22a1539fccc5')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4077, N'CONFIG_INVMGR_MODULELIBRARY', N'100115bf-c300-4ea9-bccd-22a1539fccc5'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4078 AND MenuItemTypeGuid = 'C6684B62-3C12-4989-B475-C311FEFF12B2')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = 'C6684B62-3C12-4989-B475-C311FEFF12B2'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4078 AND MenuItemTypeGuid = 'C6684B62-3C12-4989-B475-C311FEFF12B2')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4078, N'CONFIG_INVMGR_POINT_ACCESS_CONFIGURATION', N'C6684B62-3C12-4989-B475-C311FEFF12B2'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4083 AND MenuItemTypeGuid = 'F8C9452C-ACBD-44F3-8374-7F5E4ADD7D10')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = 'F8C9452C-ACBD-44F3-8374-7F5E4ADD7D10'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4083 AND MenuItemTypeGuid = 'F8C9452C-ACBD-44F3-8374-7F5E4ADD7D10')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4083, N'CONFIG_INVMGR_FCEE_MAPPINGS', N'F8C9452C-ACBD-44F3-8374-7F5E4ADD7D10'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4084 AND MenuItemTypeGuid = 'D7D37EBD-0E96-4DB4-A6AF-91776456C40F')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = 'D7D37EBD-0E96-4DB4-A6AF-91776456C40F'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4084 AND MenuItemTypeGuid = 'D7D37EBD-0E96-4DB4-A6AF-91776456C40F')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4084, N'CONFIG_INVMGR_FCE_DEVICE_SUMMARY', N'D7D37EBD-0E96-4DB4-A6AF-91776456C40F'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4087 AND MenuItemTypeGuid = '13CA996D-6FB5-40AD-893D-105A079DFE3C')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '13CA996D-6FB5-40AD-893D-105A079DFE3C'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4087 AND MenuItemTypeGuid = '13CA996D-6FB5-40AD-893D-105A079DFE3C')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4087, N'CONFIG_INVMGR_ROLLING_STOCK_IMPORT', N'13CA996D-6FB5-40AD-893D-105A079DFE3C'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4088 AND MenuItemTypeGuid = '325B0DFA-DE1D-4785-9040-29F94E0D9A47')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '325B0DFA-DE1D-4785-9040-29F94E0D9A47'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4088 AND MenuItemTypeGuid = '325B0DFA-DE1D-4785-9040-29F94E0D9A47')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4088, N'CONFIG_IMPORT_EXPORT_STRAP_TABLE_FILE_IMPORT', N'325B0DFA-DE1D-4785-9040-29F94E0D9A47'
	END
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 7039 AND MenuItemTypeGuid = '786EDC54-3A60-4847-ABC3-A25C99650C1D')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '786EDC54-3A60-4847-ABC3-A25C99650C1D'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7039 AND MenuItemTypeGuid = '786EDC54-3A60-4847-ABC3-A25C99650C1D')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7039, N'OPERATIONS_INVENTORY_MANAGEMENT_TAG_VIEWER', N'786EDC54-3A60-4847-ABC3-A25C99650C1D'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 7040 AND MenuItemTypeGuid = '228494C4-FAEE-4B01-B87C-6BD6EA834DCD')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '228494C4-FAEE-4B01-B87C-6BD6EA834DCD'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7040 AND MenuItemTypeGuid = '228494C4-FAEE-4B01-B87C-6BD6EA834DCD')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7040, N'OPERATIONS_INVENTORY_MANAGEMENT_POINT_EXPLORER', N'228494C4-FAEE-4B01-B87C-6BD6EA834DCD'
	END	
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 7041 AND MenuItemTypeGuid = '241AB768-8ACE-4560-9A9B-9F684B9F640D')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '241AB768-8ACE-4560-9A9B-9F684B9F640D'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7041 AND MenuItemTypeGuid = '241AB768-8ACE-4560-9A9B-9F684B9F640D')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7041, N'OPERATIONS_INVENTORY_MANAGEMENT_OPERATE', N'241AB768-8ACE-4560-9A9B-9F684B9F640D'
	END		
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 7042 AND MenuItemTypeGuid = 'FCC6DD44-D318-45BC-94F3-4067EDC656B2')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = 'FCC6DD44-D318-45BC-94F3-4067EDC656B2'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7042 AND MenuItemTypeGuid = 'FCC6DD44-D318-45BC-94F3-4067EDC656B2')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7042, N'OPERATIONS_INVENTORY_MANAGEMENT_ALARM_SUMMARY', N'FCC6DD44-D318-45BC-94F3-4067EDC656B2'
	END		
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 7043 AND MenuItemTypeGuid = '1AF5E328-E1B4-4E20-A090-484CC5384A95')
	BEGIN
		DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '1AF5E328-E1B4-4E20-A090-484CC5384A95'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7043 AND MenuItemTypeGuid = '1AF5E328-E1B4-4E20-A090-484CC5384A95')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7043, N'OPERATIONS_INVENTORY_MANAGEMENT_STATISTICS', N'1AF5E328-E1B4-4E20-A090-484CC5384A95'
	END

	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 7044 AND MenuItemTypeGuid = '894A6A0B-D304-433B-9257-DE7EA9E7694A')
	BEGIN
	DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '894A6A0B-D304-433B-9257-DE7EA9E7694A'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7044 AND MenuItemTypeGuid = '894A6A0B-D304-433B-9257-DE7EA9E7694A')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7044, N'OPERATIONS_INVENTORY_MANAGEMENT_FCEE_MESSAGES', N'894A6A0B-D304-433B-9257-DE7EA9E7694A'
	END
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4085 AND MenuItemTypeGuid = '42c1158f-2a6f-11ee-b81a-103d1cbd9c45')
	BEGIN
	DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '42c1158f-2a6f-11ee-b81a-103d1cbd9c45'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4085 AND MenuItemTypeGuid = '42c1158f-2a6f-11ee-b81a-103d1cbd9c45')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4085, N'CONFIG_OSDP_OPC_CONTROLLERS', N'42c1158f-2a6f-11ee-b81a-103d1cbd9c45'
	END
	
	IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex <> 4086 AND MenuItemTypeGuid = '42c11590-2a6f-11ee-b81a-103d1cbd9c45')
	BEGIN
	DELETE FROM lookup.tblMenuItemType WHERE MenuItemTypeGuid = '42c11590-2a6f-11ee-b81a-103d1cbd9c45'
	END
	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4086 AND MenuItemTypeGuid = '42c11590-2a6f-11ee-b81a-103d1cbd9c45')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 4086, N'CONFIG_OSDP_OPC_PORTS', N'42c11590-2a6f-11ee-b81a-103d1cbd9c45'
	END

	IF NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7045 AND MenuItemTypeGuid = 'FB194BC5-9BA1-48A1-9640-A85736748D58')
	BEGIN
		EXEC [lookup].[AddMenuItemType] 7045, N'OPERATIONS_INVENTORY_MANAGEMENT_OPERATE_STATISTICS', N'FB194BC5-9BA1-48A1-9640-A85736748D58'
	END
	

IF 'MAX_EQUIPMENT_TYPE' = (SELECT EquipmentTypeCode FROM lookup.tblEquipmentType WHERE EquipmentTypeIndex = 15)
BEGIN
	; DISABLE TRIGGER [lookup].[trg_insupd_tblEquipmentType_ForSync] ON lookup.tblEquipmentType;
	UPDATE lookup.tblEquipmentType SET EquipmentTypeIndex = 18 where EquipmentTypeIndex = 15;
	INSERT lookup.tblEquipmentType (EquipmentTypeIndex,EquipmentTypeCode,EquipmentTypeName,EquipmentTypeGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (15,'CONTAINER','CONTAINER','D7B83305-9291-46B1-AAC0-FC4F0AC34DC9', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator');
	INSERT lookup.tblEquipmentType (EquipmentTypeIndex,EquipmentTypeCode,EquipmentTypeName,EquipmentTypeGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (16,'VEHICLE','VEHICLE','C07F0ABC-BFB2-41AE-8E63-222A65667F6B', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator');
	INSERT lookup.tblEquipmentType (EquipmentTypeIndex,EquipmentTypeCode,EquipmentTypeName,EquipmentTypeGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (17,'INFRASTRUCTURE','INFRASTRUCTURE','5D5797D3-1F26-4215-BBC0-9B2A639FBA48', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator');
	ENABLE TRIGGER [lookup].[trg_insupd_tblEquipmentType_ForSync] ON lookup.tblEquipmentType;
END

IF 'MAX_PERSON_ROLE' = (SELECT [PersonnelRoleCode] FROM [lookup].[tblPersonnelRole] WHERE [PersonnelRoleIndex] = 2)
BEGIN
	; DISABLE TRIGGER [lookup].[trg_insupd_tblPersonnelRole_ForSync] ON [lookup].[tblPersonnelRole];
	UPDATE [lookup].[tblPersonnelRole] SET PersonnelRoleIndex = 3 where PersonnelRoleIndex = 2;
	UPDATE [lookup].[tblPersonnelRole] SET PersonnelRoleCode = N'LOADER_ROLE', PersonnelRoleName = N'Loader Role' where PersonnelRoleIndex = 0;
	INSERT INTO [lookup].[tblPersonnelRole] ([PersonnelRoleIndex], [PersonnelRoleCode], [PersonnelRoleName], [PersonnelRoleGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'OFFLOADER_ROLE', N'Offloader Role', N'd64b95da-58bf-4f2e-9132-fdc4f03237d5', N'2/10/2016 6:20:50 PM +00:00', N'Administrator', N'2/10/2016 6:20:50 PM +00:00', N'Administrator');
	ENABLE TRIGGER [lookup].[trg_insupd_tblPersonnelRole_ForSync] ON [lookup].[tblPersonnelRole];
END

IF N'MAX_STATION_TYPE' = (SELECT [StationTypeCode] FROM [lookup].[tblStationType] WHERE [StationTypeIndex] = 9)
BEGIN
	; DISABLE TRIGGER [lookup].[trg_insupd_tblStationType_ForSync] ON [lookup].[tblStationType];
	UPDATE [lookup].[tblStationType] SET StationTypeIndex = 10 where StationTypeIndex = 9;
	INSERT INTO [lookup].[tblStationType] ([StationTypeIndex], [StationTypeCode], [StationTypeName], [StationTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (9, N'MANUAL_BOL', N'MANUAL BOL STATION', N'274ef7ed-c0f6-44e3-8c90-9c6040ae2bf4', N'6/18/2012 1:02:18 PM +00:00', N'Administrator', N'6/18/2012 1:02:18 PM +00:00', N'Administrator');
	ENABLE TRIGGER [lookup].[trg_insupd_tblStationType_ForSync] ON [lookup].[tblStationType];
END

-- Correct GUID and Dates for CONTAINER
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblEquipmentType] WHERE [EquipmentTypeCode] = 'CONTAINER' AND EquipmentTypeGuid = 'D7B83305-9291-46B1-AAC0-FC4F0AC34DC9' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
BEGIN
   UPDATE [lookup].[tblEquipmentType] SET EquipmentTypeGuid = 'D7B83305-9291-46B1-AAC0-FC4F0AC34DC9', CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [EquipmentTypeCode] = 'CONTAINER' 
END

-- Correct GUID and Dates for VEHICLE
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblEquipmentType] WHERE [EquipmentTypeCode] = 'VEHICLE' AND EquipmentTypeGuid = 'C07F0ABC-BFB2-41AE-8E63-222A65667F6B' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
BEGIN
   UPDATE [lookup].[tblEquipmentType] SET EquipmentTypeGuid = 'C07F0ABC-BFB2-41AE-8E63-222A65667F6B', CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [EquipmentTypeCode] = 'VEHICLE' 
END

-- Correct GUID and Dates for INFRASTRUCTURE
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblEquipmentType] WHERE [EquipmentTypeCode] = 'INFRASTRUCTURE' AND EquipmentTypeGuid = '5D5797D3-1F26-4215-BBC0-9B2A639FBA48' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
BEGIN
   UPDATE [lookup].[tblEquipmentType] SET EquipmentTypeGuid = '5D5797D3-1F26-4215-BBC0-9B2A639FBA48', CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [EquipmentTypeCode] = 'INFRASTRUCTURE' 
END




IF ((SELECT COUNT(*) FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'SyncEnabled') = 0)
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting]
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(N'603D5775-B8CE-4DB5-B842-CC07FF39B327', 'DWORD', 'SyncEnabled', '1', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF ((SELECT COUNT(*) FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'UseLastKnownGoodStatus') = 0)
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting]
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(N'31003F9E-3CA9-44cf-87C4-611601022024', 'DWORD', 'UseLastKnownGoodStatus', '0', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF ((SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'NSPA_FuelCardImportConnectionString') = 0)
BEGIN
	INSERT INTO tblConfigurationSetting
		(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(NEWID(), 'SZ', 'NSPA_FuelCardImportConnectionString', 
		 'Provider=Microsoft.ACE.OLEDB.12.0;Data Source=<filename>;Extended Properties="Excel 12.0 Xml;HDR=YES;IMEX=1;"',
		 SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF ((SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ExternalExportResultsInterfaceName') = 0)
BEGIN
	INSERT INTO tblConfigurationSetting
		(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
		(NEWID(), 'MULTI_SZ', 'ExternalExportResultsInterfaceName', NULL, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'BaseLevelTransaction') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(12, 'BaseLevelTransaction', 'BaseLevelTransaction', N'334AC2CB-F1DB-4E86-B507-32AD0E013B9A', N'3/03/2014 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'BaseLevelTransaction' AND TransactionOriginGuid = N'334AC2CB-F1DB-4E86-B507-32AD0E013B9A') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'334AC2CB-F1DB-4E86-B507-32AD0E013B9A' 
                                                ,CreatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                                ,UpdatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                            WHERE TransactionOriginCode = 'BaseLevelTransaction' AND TransactionOriginIndex = 12
    END
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'EnterpriseLevelTransaction') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(13, 'EnterpriseLevelTransaction', 'EnterpriseLevelTransaction', N'48D43B32-C978-4674-AE70-06718E3FC6F0', N'3/03/2014 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'EnterpriseLevelTransaction' AND TransactionOriginGuid = N'48D43B32-C978-4674-AE70-06718E3FC6F0') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'48D43B32-C978-4674-AE70-06718E3FC6F0' 
                                                ,CreatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                                ,UpdatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                            WHERE TransactionOriginCode = 'EnterpriseLevelTransaction' AND TransactionOriginIndex = 13
    END
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'AdcUploadedAtBaseLevel') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(14, 'AdcUploadedAtBaseLevel', 'AdcUploadedAtBaseLevel', N'0892EEA2-4875-4DFE-9B86-AC57E6F2A77E', N'3/03/2014 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'AdcUploadedAtBaseLevel' AND TransactionOriginGuid = N'0892EEA2-4875-4DFE-9B86-AC57E6F2A77E') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'0892EEA2-4875-4DFE-9B86-AC57E6F2A77E' 
                                                ,CreatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                                ,UpdatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                            WHERE TransactionOriginCode = 'AdcUploadedAtBaseLevel' AND TransactionOriginIndex = 14
    END
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'AdcUploadedAtEnterpriseLevel') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(15, 'AdcUploadedAtEnterpriseLevel', 'AdcUploadedAtEnterpriseLevel', N'47C8A3C0-2A09-48EC-A7B3-1EA13DCAD359', N'3/03/2014 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'AdcUploadedAtEnterpriseLevel' AND TransactionOriginGuid = N'47C8A3C0-2A09-48EC-A7B3-1EA13DCAD359') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'47C8A3C0-2A09-48EC-A7B3-1EA13DCAD359'
                                                ,CreatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                                ,UpdatedDate = N'3/03/2014 1:49:09 PM -04:00'
                                            WHERE TransactionOriginCode = 'AdcUploadedAtEnterpriseLevel' AND TransactionOriginIndex = 15
    END
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'FlightlineADCFromDispatch') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(9, 'FlightlineADCFromDispatch', 'FlightlineADCFromDispatch', N'A1603803-6A9C-41F9-8BAA-25E4B0B179DE', N'11/03/2015 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'FlightlineADCFromDispatch' AND TransactionOriginGuid = N'A1603803-6A9C-41F9-8BAA-25E4B0B179DE') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'A1603803-6A9C-41F9-8BAA-25E4B0B179DE' 
									   ,CreatedDate = N'11/03/2015 1:49:09 PM -04:00'
									   ,UpdatedDate = N'11/03/2015 1:49:09 PM -04:00'
								    WHERE TransactionOriginCode = 'FlightlineADCFromDispatch' AND TransactionOriginIndex = 9
    END
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'FlightlineADCForDispatch') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(10, 'FlightlineADCForDispatch', 'FlightlineADCForDispatch', N'85129119-7315-4B62-AF9B-A0E3B985C138', N'11/03/2015 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'FlightlineADCForDispatch' AND TransactionOriginGuid = N'85129119-7315-4B62-AF9B-A0E3B985C138') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'85129119-7315-4B62-AF9B-A0E3B985C138' 
									   ,CreatedDate = N'11/03/2015 1:49:09 PM -04:00'
									   ,UpdatedDate = N'11/03/2015 1:49:09 PM -04:00'
								    WHERE TransactionOriginCode = 'FlightlineADCForDispatch' AND TransactionOriginIndex = 10
    END
END

IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'FlightlineADCStandard') = 0)
BEGIN
	INSERT INTO lookup.tblTransactionOrigin
	(TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(11, 'FlightlineADCStandard', 'FlightlineADCStandard', N'76002485-1B86-41C2-812D-E161260D026E', N'11/03/2015 1:49:09 PM -04:00', N'Administrator', N'3/03/2014 1:49:09 PM -04:00', N'Administrator')
END
ELSE
BEGIN
    -- If this lookup is using a different Guid than the fixed one, we need to correct this.
    IF ((SELECT COUNT(*) FROM lookup.tblTransactionOrigin WHERE TransactionOriginCode = 'FlightlineADCStandard' AND TransactionOriginGuid = N'76002485-1B86-41C2-812D-E161260D026E') = 0)
    BEGIN
	    UPDATE lookup.tblTransactionOrigin SET TransactionOriginGuid = N'76002485-1B86-41C2-812D-E161260D026E' 
									   ,CreatedDate = N'11/03/2015 1:49:09 PM -04:00'
									   ,UpdatedDate = N'11/03/2015 1:49:09 PM -04:00'
								    WHERE TransactionOriginCode = 'FlightlineADCStandard' AND TransactionOriginIndex = 11
    END
END

-- Insert new Activation Status for ACTIVE
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'ACTIVE')
BEGIN
    INSERT INTO [lookup].[tblActivationStatus]
                ([ActivationStatusIndex]
                ,[ActivationStatusCode]
                ,[ActivationStatusName]
                ,[ActivationStatusGuid]
                ,[CreatedDate]
                ,[CreatedBy]
                ,[UpdatedDate]
                ,[UpdatedBy])
            VALUES
                (1,'ACTIVE','ACTIVE','17D95A7C-DEB8-4C86-9066-F881A0F2B201', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator')
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'ACTIVE' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
    BEGIN
        UPDATE [lookup].[tblActivationStatus] SET CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [ActivationStatusCode] = 'ACTIVE' 

    END
END

-- Insert new Activation Status for INACTIVE
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'INACTIVE')
BEGIN
    INSERT INTO [lookup].[tblActivationStatus]
                ([ActivationStatusIndex]
                ,[ActivationStatusCode]
                ,[ActivationStatusName]
                ,[ActivationStatusGuid]
                ,[CreatedDate]
                ,[CreatedBy]
                ,[UpdatedDate]
                ,[UpdatedBy])
            VALUES
                (2,'INACTIVE','INACTIVE','428AD813-9F64-446D-8783-191E6D6B4CA8', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator')
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'INACTIVE' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
    BEGIN
        UPDATE [lookup].[tblActivationStatus] SET CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [ActivationStatusCode] = 'INACTIVE' 

    END
END

-- Insert new Activation Status for CANCELLED
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'CANCELLED')
BEGIN
    INSERT INTO [lookup].[tblActivationStatus]
                ([ActivationStatusIndex]
                ,[ActivationStatusCode]
                ,[ActivationStatusName]
                ,[ActivationStatusGuid]
                ,[CreatedDate]
                ,[CreatedBy]
                ,[UpdatedDate]
                ,[UpdatedBy])
            VALUES
                (3,'CANCELLED','CANCELLED','8CFD0977-904F-4D0F-AF5A-36A881AF6FC6', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator')
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'CANCELLED' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
    BEGIN
        UPDATE [lookup].[tblActivationStatus] SET CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [ActivationStatusCode] = 'CANCELLED' 

    END
END






-- Insert new Activation Status for LOCKED
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'LOCKED')
BEGIN
    INSERT INTO [lookup].[tblActivationStatus]
                ([ActivationStatusIndex]
                ,[ActivationStatusCode]
                ,[ActivationStatusName]
                ,[ActivationStatusGuid]
                ,[CreatedDate]
                ,[CreatedBy]
                ,[UpdatedDate]
                ,[UpdatedBy])
            VALUES
                (3,'LOCKED','LOCKED','7C91755C-6EA1-4712-BFF9-364E7B0C4C68', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator')
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'LOCKED' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
    BEGIN
        UPDATE [lookup].[tblActivationStatus] SET CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [ActivationStatusCode] = 'LOCKED' 

    END
END

-- Insert new Activation Status for LOST/STOLEN
IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'LOSTSTOLEN')
BEGIN
    INSERT INTO [lookup].[tblActivationStatus]
                ([ActivationStatusIndex]
                ,[ActivationStatusCode]
                ,[ActivationStatusName]
                ,[ActivationStatusGuid]
                ,[CreatedDate]
                ,[CreatedBy]
                ,[UpdatedDate]
                ,[UpdatedBy])
            VALUES
                (4,'LOSTSTOLEN','LOST/STOLEN','1798C88E-9E4C-48CD-9BD5-473B63399A0C', N'3/03/2014 1:49:09 PM -04:00','Administrator',N'3/03/2014 1:49:09 PM -04:00','Administrator')
END
ELSE
BEGIN
    IF NOT EXISTS (SELECT 1 FROM [lookup].[tblActivationStatus] WHERE [ActivationStatusCode] = 'LOSTSTOLEN' AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00')
    BEGIN
        UPDATE [lookup].[tblActivationStatus] SET CreatedDate = N'3/03/2014 1:49:09 PM -04:00', UpdatedDate = N'3/03/2014 1:49:09 PM -04:00' WHERE [ActivationStatusCode] = 'LOSTSTOLEN' 

    END
END


IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4054 OR MenuItemTypeCode = 'CONFIG_OTHER_FUEL_CARD_TYPES') = 0)
BEGIN
	INSERT INTO [lookup].[tblMenuItemType] ([MenuItemTypeIndex], [MenuItemTypeCode], [MenuItemTypeName], [MenuItemTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
    VALUES 
    (4054, N'CONFIG_OTHER_FUEL_CARD_TYPES', N'CONFIG_OTHER_FUEL_CARD_TYPES', N'6690E2BE-D2B9-441D-8ECB-286ADD912CD0', N'3/20/2014 4:08:03 PM -04:00', N'Administrator', N'3/20/2014 4:08:03 PM -04:00', N'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4055))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
           ([MenuItemTypeIndex]
           ,[MenuItemTypeCode]
           ,[MenuItemTypeName]
           ,[MenuItemTypeGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (4055
           ,N'CONFIG_OTHER_FUEL_CARD_LIMITS'
           ,N'CONFIG_OTHER_FUEL_CARD_LIMITS'
           ,N'9E544406-CF81-4044-A3EE-392BFC011C66'
           ,N'3/03/2014 1:49:09 PM -04:00'
           ,N'Administrator'
           ,N'3/03/2014 1:49:09 PM -04:00'
           ,N'Administrator')
END
ELSE
BEGIN
    IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 4055 AND CreatedDate = N'3/03/2014 1:49:09 PM -04:00'))
    BEGIN
        UPDATE [lookup].[tblMenuItemType] 
            SET CreatedDate = N'3/03/2014 1:49:09 PM -04:00'
                ,UpdatedDate = N'3/03/2014 1:49:09 PM -04:00'
        WHERE MenuItemTypeIndex = 4055
    END
END


IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'tblFuelCardLimit'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblFuelCardLimit'
							, 'Fuel Card Limit'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblFuelCardLimit] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)
END


IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'map_tblEntityFuelCardLimitToSite'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblEntityFuelCardLimitToSite'
							, 'Site - Fuel Card Limit'
							, ''
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN fl.ID IS NULL THEN fla.ID ELSE fl.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFuelCardLimitToSite] a'
							+ ' LEFT JOIN [dbo].[tblFuelCardLimit] fl ON fl.FuelCardLimitGuid = a.FuelCardLimitGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCardLimit] fla ON fla.FuelCardLimitGuid = a.FuelCardLimitGuid AND fla._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)
END
ELSE
BEGIN
	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN fl.ID IS NULL THEN fla.ID ELSE fl.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFuelCardLimitToSite] a'
							+ ' LEFT JOIN [dbo].[tblFuelCardLimit] fl ON fl.FuelCardLimitGuid = a.FuelCardLimitGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCardLimit] fla ON fla.FuelCardLimitGuid = a.FuelCardLimitGuid AND fla._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFuelCardLimitToSite'
END

IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'map_tblFuelCardLimitToFuelCard'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblFuelCardLimitToFuelCard'
							, 'Fuel Card Limit - Fuel Card'
							, 'Fuel Card Limit'
							, 'SELECT @ID = CASE WHEN fl.ID IS NULL THEN fla.ID ELSE fl.ID END + '' - '''
							+ ' + CASE WHEN f.ID IS NULL THEN fa.ID ELSE f.ID END'
							+ ' FROM  [fmaudit].[map_tblFuelCardLimitToFuelCard] a'
							+ ' LEFT JOIN [dbo].[tblFuelCardLimit] fl ON fl.FuelCardLimitGuid = a.FuelCardLimitGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCardLimit] fla ON fla.FuelCardLimitGuid = a.FuelCardLimitGuid AND fla._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblFuelCards] f ON f.FuelCardGuid = a.FuelCardGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCards] fa ON fa.FuelCardGuid = a.FuelCardGuid AND fa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)
END

IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'tblFuelCardLimitLineItem'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblFuelCardLimitLineItem'
							, 'Fuel Card Limit - Line Item'
							, 'Fuel Card Limit'
							, 'SELECT @ID = CASE WHEN fl.ID IS NULL THEN fla.ID ELSE fl.ID END  + '' - '''
							+ ' +  CASE WHEN p.ProductID IS NOT NULL THEN p.ProductID ' 
							+ ' WHEN pg.ID IS NOT NULL THEN pg.ID '
							+ ' ELSE ''All Products'' END '
							+ ' + '' - '' + l.FuelCardLimitPeriodName' 
							+ ' FROM [fmAudit].[tblFuelCardLimitLineItem] a'
							+ ' LEFT JOIN [dbo].[tblFuelCardLimit] fl ON fl.FuelCardLimitGuid = a.FuelCardLimitGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCardLimit] fla ON fla.FuelCardLimitGuid = a.FuelCardLimitGuid AND fla._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [dbo].[tblApplicationString] pg ON pg.ApplicationStringGuid = a.ProductGroupApplicationStringGuid'
							+ ' LEFT JOIN [lookup].[tblFuelCardLimitPeriod] l ON l.FuelCardLimitPeriodIndex = a.Period'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)
END







UPDATE tblAuditHandler SET IDQuery = N'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END FROM [fmAudit].[tblTransactionLineItemUserData] a  LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = l.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = la.TransactionGuid AND ta._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = N'tblTransactionLineItemUserData'




-- Correcting the misspelling of Hierarchy.  Update all records with the misspelling.
IF ((SELECT COUNT(*) FROM tblAuditHandler WHERE TypeID = 'Loading Heirarchy') > 0)
BEGIN
	UPDATE tblAuditHandler SET TypeID = 'Loading Hierarchy' WHERE TypeID = 'Loading Heirarchy'
END

IF ((SELECT COUNT(*) FROM tblAuditHandler WHERE TypeID = 'Off-Loading Heirarchy') > 0)
BEGIN
	UPDATE tblAuditHandler SET TypeID = 'Off-Loading Hierarchy' WHERE TypeID = 'Off-Loading Heirarchy'
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'Common Access Card (CAC) Enable')=0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('CA80342C-CEF4-4CC9-93F5-3BC69F483874', 'DWORD', 'Common Access Card (CAC) Enable', 1, N'7/16/2014 8:30:00 AM -04:00', 'Administrator', N'7/16/2014 8:30:00 AM -04:00', 'Administrator')
END

IF ((SELECT COUNT(*) FROM tblAuditHandler WHERE TableName = 'tblAllocationLineItems') > 0)
BEGIN
	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,s.CompanyShipToToBillToGuid) WHEN sa.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,sa.CompanyShipToToBillToGuid) WHEN s.CompanyBillToToShipperGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](2,s.CompanyBillToToShipperGuid) WHEN sa.CompanyBillToToShipperGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](2,sa.CompanyBillToToShipperGuid) WHEN s.CompanyShipperToOwnerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](1,s.CompanyShipperToOwnerGuid) WHEN sa.CompanyShipperToOwnerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](1,sa.CompanyShipperToOwnerGuid) WHEN s.CompanyLoadOwnerToManagerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](0,s.CompanyLoadOwnerToManagerGuid) WHEN sa.CompanyLoadOwnerToManagerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](0,sa.CompanyLoadOwnerToManagerGuid) ELSE ''Invalid Allocation ID'' END  + '' - '' + CASE WHEN s.EffectiveDate IS NOT NULL THEN CONVERT(NVARCHAR,s.EffectiveDate,101) WHEN sa.EffectiveDate IS NOT NULL THEN CONVERT(NVARCHAR,sa.EffectiveDate,101) END + '' - '' + ISNULL(CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END,''Null Product'') + '' - '' + dbo.udf_GetLowerWithInitUpperString(l.ResetPeriodName) FROM [fmAudit].[tblAllocationLineItems] a LEFT JOIN [dbo].[tblAllocations] s ON s.AllocationGuid = a.AllocationGuid LEFT JOIN [fmaudit].[tblAllocations] sa ON sa.AllocationGuid = a.AllocationGuid AND sa._AuditEventType = ''D'' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.AssignedProductGuid LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.AssignedProductGuid AND pa._AuditEventType = ''D'' LEFT JOIN [lookup].[tblResetPeriod] l ON l.ResetPeriodIndex = a.LookupResetPeriodIndex WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	Where TableName = 'tblAllocationLineItems'
END

IF ((SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ForceCloseoutButtonDisable') = 0) 
BEGIN
	INSERT INTO tblConfigurationSetting
	(
		ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy
	)
	VALUES
	(
		'482DA60D-1EDA-4EBB-B1D4-5EB67F1E282E', 'SZ', 'ForceCloseoutButtonDisable', 'FALSE', N'7/31/2014 3:30:00 AM -04:00', 'Administrator', N'7/31/2014 3:30:00 AM -04:00', 'Administrator'
	)
END

IF ((SELECT COUNT(*) FROM tblAuditHandler WHERE TableName = 'map_tblEntityFuelCardTypeToSite') = 0)
BEGIN
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	)
END




UPDATE tblAuditHandler SET TableName = 'map_tblApplicationStringToAlarmEventCategory' WHERE TableName = 'map_tblApplicationStringToAlarmEventCatery'
UPDATE tblAuditHandler SET TypeID = 'E-mail Group - Category' WHERE TableName = 'map_tblApplicationStringToAlarmEventCategory'
UPDATE tblAuditHandler SET ParentTypeID = 'E-mail Groups' WHERE TableName = 'map_tblApplicationStringToAlarmEventCategory'
UPDATE tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END FROM  [fmaudit].[map_tblApplicationStringToAlarmEventCategory] a LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToAlarmEventCategory'
UPDATE tblAuditHandler SET TableName = 'map_tblEntityAlarmAndEventCategoryToSite' WHERE TableName = 'map_tblEntityAlarmAndEventCateryToSite'
UPDATE tblAuditHandler SET TypeID = 'Site - Alarm Event Category' WHERE TableName = 'map_tblEntityAlarmAndEventCategoryToSite'
UPDATE tblAuditHandler SET ParentTypeID = 'Alarm Event Categories' WHERE TableName = 'map_tblEntityAlarmAndEventCategoryToSite'
UPDATE tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END FROM  [fmaudit].[map_tblEntityAlarmAndEventCategoryToSite] a LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmAndEventCategoryToSite'
UPDATE tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
									+ ' FROM  [fmaudit].[map_tblEntityReportConfigurationSettingsToSite] a'
									+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
									+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
									+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
									WHERE TableName = 'map_tblEntityReportConfigurationSettingsToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.InterfaceID + '' - '' + a.RequestID'
							+ ' FROM [fmAudit].[tblExportRequest] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '  WHERE TableName = 'tblExportRequest'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN u.UserID IS NULL THEN ua.UserID ELSE u.UserID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(mt.MenuItemTypeName)'
							+ ' FROM [fmAudit].[tblMenuFavorites] a'
							+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblMenuItemType] mt ON mt.MenuItemTypeIndex = a.MenuItemType'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMenuFavorites'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = es.EntityTypeDisplayName + '' - '' + a.TargetField'
							+ ' FROM [fmAudit].[erv_tblEntityRecordVersioningFieldConfig] a'
							+ ' LEFT JOIN [erv].[tblEntitySegmentTemplate] es ON es.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'erv_tblEntityRecordVersioningFieldConfig'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[tblReserveLevels] a'
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblReserveLevels'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.Controller + '' - '' + a.Memo'
							+ ' FROM [fmAudit].[tblControllersLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblControllersLog'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblDispatchConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDispatchConfiguration'


	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblDispatchGrid] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDispatchGrid'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.DispatchGridID + '' - '' + a.ID'
							+ ' FROM [fmAudit].[tblDispatchGridColumn] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDispatchGridColumn'
	
	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblCustomToolbar] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCustomToolbar'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.CustomToolbarID + '' - '' + a.ID'
							+ ' FROM [fmAudit].[tblCustomToolbarCommand] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCustomToolbarCommand'
	
	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - Dispatch Configuration'''
							+ ' FROM  [fmaudit].[map_tblEntityDispatchConfigurationToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityDispatchConfigurationToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.Note'
							+ ' FROM [fmAudit].[tblNotes] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblNotes'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblMessages] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMessages'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.[Key] + '' - '' + a.[Value]'
							+ ' FROM [fmAudit].[tblDataDictionaries] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDataDictionaries'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Key/Value Pairs'''
							+ ' FROM  [fmaudit].[map_tblEntityDataDictionaryToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityDataDictionaryToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + a.Description'
							+ ' FROM [fmAudit].[tblAppointmentTank] a'
							+ ' LEFT JOIN [dbo].tblTanks t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].tblTanks ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAppointmentTank'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + CASE WHEN at.Description IS NULL THEN ata.Description ELSE at.Description END'
							+ ' FROM  [fmaudit].[map_tblEntityAppointmentTankToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAppointmentTank] at ON at.AppointmentTankGuid = a.AppointmentTankGuid'
							+ ' LEFT JOIN [fmaudit].[tblAppointmentTank] ata ON ata.AppointmentTankGuid = a.AppointmentTankGuid AND ata._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = at.TankGuid OR t.TankGuid = ata.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = ata.TankGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAppointmentTankToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + a.Description'
							+ ' FROM [fmAudit].[tblAppointmentPersonnel] a'
							+ ' LEFT JOIN [dbo].tblPersonnel p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].tblPersonnel pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAppointmentPersonnel'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN ap.Description IS NULL THEN apa.Description ELSE ap.Description END'
							+ ' FROM  [fmaudit].[map_tblEntityAppointmentPersonnelToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAppointmentPersonnel] ap ON ap.AppointmentPersonnelGuid = a.AppointmentPersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblAppointmentPersonnel] apa ON apa.AppointmentPersonnelGuid = a.AppointmentPersonnelGuid AND apa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = ap.PersonnelGuid OR p.PersonnelGuid = apa.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = apa.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAppointmentPersonnelToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + a.Description'
							+ ' FROM [fmAudit].[tblAppointmentEquipment] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAppointmentEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + CASE WHEN ae.Description IS NULL THEN aea.Description ELSE ae.Description END'
							+ ' FROM  [fmaudit].[map_tblEntityAppointmentEquipmentToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAppointmentEquipment] ae ON ae.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblAppointmentEquipment] aea ON aea.AppointmentEquipmentGuid = a.AppointmentEquipmentGuid AND aea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = ae.EquipmentGuid OR e.EquipmentGuid = aea.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = aea.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAppointmentEquipmentToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.Name'
							+ ' FROM [fmAudit].[tblQualityTags] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQualityTags'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TankID + '' - '' + a.QualityTagName'
							+ ' FROM [fmAudit].[tblTankQualityTagLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTankQualityTagLog'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.EquipmentID + '' - '' + a.QualityTagName'
							+ ' FROM [fmAudit].[tblEquipmentQualityTagLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipmentQualityTagLog'
	
	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.Name IS NULL THEN qa.Name ELSE q.Name END'
							+ ' FROM  [fmaudit].[map_tblEntityQualityTagToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].tblQualityTags q ON q.QualityTagGuid = a.QualityTagGuid'
							+ ' LEFT JOIN [fmaudit].tblQualityTags qa ON qa.QualityTagGuid = a.QualityTagGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityQualityTagToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TestSetName'
							+ ' FROM [fmAudit].[tblTestSetDefinitions] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetDefinitions'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ts.TestSetName IS NULL THEN tsa.TestSetName ELSE ts.TestSetName END'
							+ ' FROM  [fmaudit].[map_tblEntityTestSetToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTestSetDefinitions] ts ON ts.TestSetDefinitionGuid = a.TestSetDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestSetDefinitions] tsa ON tsa.TestSetDefinitionGuid = a.TestSetDefinitionGuid AND tsa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityTestSetToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TestName'
							+ ' FROM [fmAudit].[tblTestDefinitions] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestDefinitions'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN t.TestName IS NULL THEN ta.TestName ELSE t.TestName END'
							+ ' FROM  [fmaudit].[map_tblEntityTestToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTestDefinitions] t ON t.TestDefinitionGuid = a.TestDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestDefinitions] ta ON ta.TestDefinitionGuid = a.TestDefinitionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityTestToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN ts.TestSetName IS NULL THEN tsa.TestSetName ELSE ts.TestSetName END + '' - '''
							+ ' + CASE WHEN t.TestName IS NULL THEN ta.TestName ELSE t.TestName END'
							+ ' FROM  [fmaudit].[map_tblTestDefinitionToTestSetDefinition] a'
							+ ' LEFT JOIN [dbo].[tblTestSetDefinitions] ts ON ts.TestSetDefinitionGuid = a.TestSetDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestSetDefinitions] tsa ON tsa.TestSetDefinitionGuid = a.TestSetDefinitionGuid AND tsa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTestDefinitions] t ON t.TestDefinitionGuid = a.TestDefinitionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTestDefinitions] ta ON ta.TestDefinitionGuid = a.TestDefinitionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblTestDefinitionToTestSetDefinition'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TestSetName + '' - '' + a.TankID'
							+ ' FROM [fmAudit].[tblTestSetTankResults] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetTankResults'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TestName + '' - '''
							+ ' + CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END'
							+ ' FROM [fmAudit].[tblTestTankResults] a'
							+ ' LEFT JOIN [dbo].[tblTestSetTankResults] t ON t.TestSetTankResultGuid = a.TestSetTankResultGuid'
							+ ' LEFT JOIN [fmAudit].[tblTestSetTankResults] ta on ta.TestSetTankResultGuid = a.TestSetTankResultGuid AND  ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestTankResults'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TestSetName + '' - '' + a.EquipmentID'
							+ ' FROM [fmAudit].[tblTestSetEquipmentResults] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetEquipmentResults'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TestName + '' - '''
							+ ' + CASE WHEN t.EquipmentID IS NULL THEN ta.EquipmentID ELSE t.EquipmentID END'
							+ ' FROM [fmAudit].[tblTestEquipmentResults] a'
							+ ' LEFT JOIN [dbo].[tblTestSetEquipmentResults] t ON t.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid'
							+ ' LEFT JOIN [fmAudit].[tblTestSetEquipmentResults] ta on ta.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid AND  ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestEquipmentResults'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblPIDXProfiles] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblPIDXProfiles'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblPIDXProfileToCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblFuelCards] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblFuelCards'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFuelCardToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblFuelCards] ap ON ap.FuelCardGuid = a.FuelCardGuid'
							+ ' LEFT JOIN [fmaudit].[tblFuelCards] apa ON apa.FuelCardGuid = a.FuelCardGuid AND apa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFuelCardToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblAdditiveProfiles] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAdditiveProfiles'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToAdditiveProfile] a'
							+ ' LEFT JOIN [dbo].[tblAdditiveProfiles] ap ON ap.AdditiveProfileGuid = a.AssignedToAdditiveProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblAdditiveProfiles] apa ON apa.AdditiveProfileGuid = a.AssignedToAdditiveProfileGuid AND apa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToAdditiveProfile'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAdditiveProfileToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAdditiveProfiles] ap ON ap.AdditiveProfileGuid = a.AdditiveProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblAdditiveProfiles] apa ON apa.AdditiveProfileGuid = a.AdditiveProfileGuid AND apa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAdditiveProfileToSite'


	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAlarmPriorityToSite] a'
							+ ' LEFT JOIN [dbo].[tblAlarmPriorities] ap ON ap.AlarmPriorityGuid = a.AlarmPriorityGuid'
							+ ' LEFT JOIN [fmaudit].[tblAlarmPriorities] apa ON apa.AlarmPriorityGuid = a.AlarmPriorityGuid AND apa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmPriorityToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.Source + '' : '' + a.ID'
							+ ' FROM [fmAudit].[tblAlarmAndEvents] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAlarmAndEvents'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
							+ ' FROM  [fmaudit].[map_tblEntityAlarmAndEventToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmAndEventToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblEmailGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEmailGroups'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToEmailAddress] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToEmailAddress'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToAlarmEventCategory] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToAlarmEventCategory'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END + '' - '''
							+ ' + CASE WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END'
							+ ' FROM  [fmaudit].[map_tblAlarmPriorityToEmailGroup] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAlarmPriorities] ap ON ap.AlarmPriorityGuid = a.AlarmPriorityGuid'
							+ ' LEFT JOIN [fmaudit].[tblAlarmPriorities] apa ON apa.AlarmPriorityGuid = a.AlarmPriorityGuid AND apa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblAlarmPriorityToEmailGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN eg.ID IS NULL THEN ega.ID ELSE eg.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEmailGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblEmailGroups] eg ON eg.EmailGroupGuid = a.EmailGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblEmailGroups] ega ON ega.EmailGroupGuid = a.EmailGroupGuid AND ega._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEmailGroupToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.UserID'
							+ ' FROM [fmAudit].[tblUsers] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUsers'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN u.UserID IS NULL THEN ua.UserID ELSE u.UserID END'
							+ ' FROM  [fmaudit].[map_tblEntityUserToSite] a'
							+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityUserToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.UserID'
							+ ' FROM [fmAudit].[tblArchivedUsers] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblArchivedUsers'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.GroupID'
							+ ' FROM [fmAudit].[tblGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblGroups'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblEntityUserGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityUserGroupToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END + '' - '''
							+ ' + CASE WHEN u.UserID IS NULL THEN ua.UserID ELSE u.UserID END'
							+ ' FROM  [fmaudit].[map_tblUserToGroup] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblUsers] u ON u.UserGuid = a.UserGuid'
							+ ' LEFT JOIN [fmaudit].[tblUsers] ua ON ua.UserGuid = a.UserGuid AND ua._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblUserToGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.RightName)'
							+ ' FROM  [fmaudit].[map_tblGroupToRight] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblRight] l ON l.RightIndex = a.LookupRightIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToRight'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END + '' - '''
							+ ' + CASE WHEN a.CompanyGuid IS NULL THEN ''{All}'' WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblCompanyCompanyToUserGroup] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyCompanyToUserGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblCompanies] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCompanies'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.CompanyRoleName)'
							+ ' FROM  [fmaudit].[map_tblCompanyToRole] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblCompanyRole] l ON l.CompanyRoleIndex = a.LookupCompanyRoleIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyToRole'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.AssignedToCompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToSupplierProductCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.AssignedToCompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToSupplierProductCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToUnavailableInventoryCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.AssignedToCompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToUnavailableInventoryCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.DayOfWeekName)'
							+ ' FROM [fmAudit].[tblScheduleCompanyAccess] a' 
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblDayOfWeek] l ON l.DayOfWeekIndex = a.LookupDayOfWeekIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblScheduleCompanyAccess'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c1.ID IS NULL THEN ca1.ID ELSE c1.ID END + '' - '''
							+ ' + CASE WHEN c2.ID IS NULL THEN ca2.ID ELSE c2.ID END'
							+ ' FROM [fmAudit].[map_tblCompanyAuthorizedCarrierToCompany] a' 
							+ ' LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.AssignedToCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.AssignedToCompanyGuid AND ca1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c2 ON c2.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca2 ON ca2.CompanyGuid = a.CompanyGuid AND ca2._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyAuthorizedCarrierToCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN c1.ID IS NULL THEN ca1.ID ELSE c1.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationCompanyCertificateAndPermitToCompany] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c1 ON c1.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca1 ON ca1.CompanyGuid = a.CompanyGuid AND ca1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationCompanyCertificateAndPermitToCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyToSite] a'
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblMaintenanceReasons] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMaintenanceReasons'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TankID + '' - '' + a.MaintenanceReason'
							+ ' FROM [fmAudit].[tblTankMaintenanceLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTankMaintenanceLog'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.EquipmentID + '' - '' + a.MaintenanceReason'
							+ ' FROM [fmAudit].[tblEquipmentMaintenanceLog] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipmentMaintenanceLog'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblEquipment] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableEquipment] a' 
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationEquipmentTestAndInspectionToEquipment] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationEquipmentTestAndInspectionToEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationEquipmentTagAndLicenseToEquipment] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationEquipmentTagAndLicenseToEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN e.ID IS NULL THEN ea.ID ELSE e.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentToSite] a'
							+ ' LEFT JOIN [dbo].[tblEquipment] e ON e.EquipmentGuid = a.EquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipment] ea ON ea.EquipmentGuid = a.EquipmentGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.EqTypeName'
							+ ' FROM [fmAudit].[tblEquipmentTypes] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipmentTypes'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonTrainingToEquipmentType'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonQualificationToEquipmentType] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonQualificationToEquipmentType'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END + '' - '''
							+ ' + a.Alias'
							+ ' FROM  [fmaudit].[tblAirplaneTank] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAirplaneTank'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN e.EqTypeName IS NULL THEN ea.EqTypeName ELSE e.EqTypeName END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentTypeToSite] a'
							+ ' LEFT JOIN [dbo].[tblEquipmentTypes] e ON e.EquipmentTypeGuid = a.EquipmentTypeGuid'
							+ ' LEFT JOIN [fmaudit].[tblEquipmentTypes] ea ON ea.EquipmentTypeGuid = a.EquipmentTypeGuid AND ea._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentTypeToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = SettingKey'
							+ ' FROM [fmAudit].[tblConfigurationSetting] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblConfigurationSetting'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = SettingID'
							+ ' FROM [fmAudit].[tblSettings] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSettings'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''N/A'''
							+ ' FROM [fmAudit].[tblSystemSettings] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSystemSettings'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = PersonID'
							+ ' FROM [fmAudit].[tblPersonnel] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblPersonnel'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.DayOfWeekName)'
							+ ' FROM [fmAudit].[tblSchedulePersonnelAccess] a' 
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblDayOfWeek] l ON l.DayOfWeekIndex = a.LookupDayOfWeekIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSchedulePersonnelAccess'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.PersonnelRoleName)'
							+ ' FROM  [fmaudit].[map_tblPersonnelToRole] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblPersonnelRole] l ON l.PersonnelRoleIndex = a.LookupPersonnelRoleIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblPersonnelToRole'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonLicenseToPerson] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonLicenseToPerson'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonQualificationToPerson] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonQualificationToPerson'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblQualificationPersonTrainingToPerson] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonTrainingToPerson'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.PersonID IS NULL THEN pa.PersonID ELSE p.PersonID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelToSite] a'
							+ ' LEFT JOIN [dbo].[tblPersonnel] p ON p.PersonnelGuid = a.PersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblPersonnel] pa ON pa.PersonnelGuid = a.PersonnelGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelToSite'

	Update tblAuditHandler SET IDQuery ='SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblSites] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSites'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM [fmAudit].[tblSitesAncillaryData] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSitesAncillaryData'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableSite] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.DayOfWeekName)'
							+ ' FROM [fmAudit].[tblScheduleTerminalOperation] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblDayOfWeek] l ON l.DayOfWeekIndex = a.LookupDayOfWeekIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblScheduleTerminalOperation'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CONVERT(NVARCHAR(20),a.HolidayDate,101)'
							+ ' FROM [fmAudit].[tblScheduleHoliday] a' 
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblScheduleHoliday'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = AliasName'
							+ ' FROM [fmAudit].[tblTransactionAliases]'
							+ ' WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionAliases'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.TransactionStatusName)'
							+ ' FROM [fmAudit].[map_tblTransactionAliasToStatus] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblTransactionStatus] l ON l.TransactionStatusIndex = a.LookupTransactionStatusIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblTransactionAliasToStatus'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + a.DisplayName'
							+ ' FROM [fmAudit].[tblTransactionAliasFields] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionAliasFields'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldTransactionAlias] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldTransactionAlias'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueTransactionAlias] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldTransactionAlias] ud ON ud.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAlias] uda ON uda.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid AND uda._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = ud.TransactionAliasGuid OR t.TransactionAliasGuid = uda.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = uda.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueTransactionAlias'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldTransactionAliasLineItem] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldTransactionAliasLineItem'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueTransactionAliasLineItem] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] ud ON ud.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAliasLineItem] uda ON uda.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid AND uda._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = ud.TransactionAliasGuid OR t.TransactionAliasGuid = uda.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = uda.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueTransactionAliasLineItem'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM [fmAudit].[map_tblGroupToTransactionAlias] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToTransactionAlias'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToTransactionAliasExclusion] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.AssignedToTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.AssignedToTransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToTransactionAliasExclusion'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t1.AliasName IS NULL THEN t1a.AliasName ELSE t1.AliasName END + '' - '''
							+ ' + CASE WHEN t2.AliasName IS NULL THEN t2a.AliasName ELSE t2.AliasName END'
							+ ' FROM [fmAudit].[map_tblAssociatedTransactionAliases] a' 
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t1 ON t1.TransactionAliasGuid = a.ParentTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] t1a ON t1a.TransactionAliasGuid = a.ParentTransactionAliasGuid AND t1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t2 ON t2.TransactionAliasGuid = a.ChildTransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] t2a ON t2a.TransactionAliasGuid = a.ChildTransactionAliasGuid AND t2a._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblAssociatedTransactionAliases'

	Update tblAuditHandler SET IDQuery =  'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' FROM  [fmaudit].[map_tblEntityTransactionAliasToSite] a'
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityTransactionAliasToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ProductID'
							+ ' FROM [fmAudit].[tblProducts] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProducts'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN b.ProductID IS NULL THEN ba.ProductID ELSE b.ProductID END + '' - '''
							+ ' + CASE WHEN c.ProductID IS NULL THEN ca.ProductID ELSE c.ProductID END'
							+ ' FROM [fmAudit].[map_tblProductToBlendComponent] a' 
							+ ' LEFT JOIN [dbo].[tblProducts] b ON b.ProductGuid = a.AssignedToProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] ba ON ba.ProductGuid = a.AssignedToProductGuid  AND ba._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] c ON c.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] ca ON ca.ProductGuid = a.ProductGuid  AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToBlendComponent'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + CASE WHEN aps.ID IS NULL THEN apsa.ID ELSE aps.ID END'
							+ ' FROM [fmAudit].[map_tblApplicationStringToDotHazardous] a' 
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON ca.ProductGuid = a.ProductGuid  AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] aps ON aps.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] apsa ON apsa.ApplicationStringGuid = a.ApplicationStringGuid  AND apsa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToDotHazardous'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END + '' - '''
							+ ' + CASE WHEN aps.ID IS NULL THEN apsa.ID ELSE aps.ID END'
							+ ' FROM [fmAudit].[map_tblApplicationStringToProductMessage] a' 
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid  AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] aps ON aps.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] apsa ON apsa.ApplicationStringGuid = a.ApplicationStringGuid  AND apsa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToProductMessage'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblEntityProductToSite] a'
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProductToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblStations] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '	 WHERE TableName = 'tblStations'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableStation'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM [fmAudit].[map_tblQualificationPersonQualificationToStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonQualificationToStation'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM [fmAudit].[map_tblQualificationPersonTrainingToStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonTrainingToStation'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM [fmAudit].[map_tblQualificationEquipmentTestAndInspectionToStation] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationEquipmentTestAndInspectionToStation'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableStationOutputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableStationOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(pvt.ProcessVariableTypeName) + '' '''
							+ ' + CONVERT(NVARCHAR,a.InstanceNumber+1)'
							+ ' FROM [fmAudit].[tblProcessVariableStationInputPermissive] a'
							+ ' LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] pvt ON pvt.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableStationInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN a.BayAArmNumber IS NOT NULL THEN'
							+ ' CASE WHEN s1.ID IS NULL THEN s1a.ID ELSE s1.ID END ELSE '''' END'
							+ ' + CASE WHEN a.BayAArmNumber IS NOT NULL AND a.BayBArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN a.BayBArmNumber IS NOT NULL THEN'
							+ ' + CASE WHEN s2.ID IS NULL THEN s2a.ID ELSE s2.ID END ELSE '''' END'
							+ ' + '' - '''
							+ ' + CASE WHEN a.BayAArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,a.BayAArmNumber) ELSE '''' END'
							+ ' + CASE WHEN a.BayAArmNumber IS NOT NULL AND a.BayBArmNumber IS NOT NULL THEN '', '' ELSE '''' END'
							+ ' + CASE WHEN a.BayBArmNumber IS NOT NULL THEN CONVERT(NVARCHAR,a.BayBArmNumber) ELSE '''' END'
							+ ' FROM [fmAudit].[tblLoadArms] a' 
							+ ' LEFT JOIN [dbo].[tblStations] s1 ON s1.StationGuid = a.BayAStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s1a ON s1a.StationGuid = a.BayAStationGuid AND s1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblStations] s2 ON s2.StationGuid = a.BayBStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblStations] s2a ON s2a.StationGuid = a.BayBStationGuid AND s2a._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblLoadArms'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableLoadArm'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableLoadArmOutPutPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableLoadArmInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableNoAdditiveOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableNoAdditiveInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToPresetExternalComponent'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableExternalComponentOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableExternalComponentInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableExternalComponentBlendPercentage'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToPresetComponentTankOrTankGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableComponentOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableComponentInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToPresetInjector'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableAdditiveOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableAdditiveInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariablePresetInjector'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToPresetRecipe'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableRecipeOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableRecipeInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToPresetFlowControlledAdditive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableFlowControlledAdditiveOutputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.BayAArmNumber IS NOT NULL OR la.BayAArmNumber IS NOT NULL THEN'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableFlowControlledAdditiveInputPermissive'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblTankGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '	 WHERE TableName = 'tblTankGroups'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN tg.ID IS NULL THEN tga.ID ELSE tg.ID END + '' - '''
							+ ' + CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END'
							+ ' FROM [fmAudit].[map_tblTankToTankGroup] a'
							+ ' LEFT JOIN [dbo].[tblTankGroups] tg ON tg.TankGroupGuid = a.AssignedToTankGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblTankGroups] tga ON tga.TankGroupGuid = a.AssignedToTankGroupGuid AND tga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '	 WHERE TableName = 'map_tblTankToTankGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblGates] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '	 WHERE TableName = 'tblGates'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = TankID'
							+ ' FROM [fmAudit].[tblTanks] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTanks'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + dbo.udf_GetLowerWithInitUpperString(l.ProcessVariableTypeName)'
							+ ' FROM [fmAudit].[tblProcessVariableTank] a' 
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblProcessVariableType] l ON l.ProcessVariableTypeIndex = a.LookupProcessVariableTypeIndex'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableTank'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = URL'
							+ ' FROM [fmAudit].[tblOPCConnections] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblOPCConnections'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = MeterID'
							+ ' FROM [fmAudit].[tblMeter] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMeter'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TankID IS NULL THEN ta.TankID ELSE t.TankID END + '' - '''
							+ ' + CASE WHEN m.MeterID IS NULL THEN ma.MeterID ELSE m.MeterID END'
							+ ' FROM [fmAudit].[map_tblMeterToTank] a'
							+ ' LEFT JOIN [dbo].[tblTanks] t ON t.TankGuid = a.TankGuid'
							+ ' LEFT JOIN [fmaudit].[tblTanks] ta ON ta.TankGuid = a.TankGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblMeter] m ON m.MeterGuid = a.MeterGuid'
							+ ' LEFT JOIN [fmaudit].[tblMeter] ma ON ma.MeterGuid = a.MeterGuid AND ma._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblMeterToTank'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblApplicationString] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblApplicationString'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityDotHazardousMessagesToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityDotHazardousMessagesToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityProductMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProductMessageToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAllocationGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAllocationGroupToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityProductGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProductGroupToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyTypeToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyTypeToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFuelCardTypeToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFuelCardTypeToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityAlarmAndEventCategoryToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmAndEventCategoryToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEmailAddressToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEmailAddressToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyGroupToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyGroupToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEntryMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEntryMessageToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityExitMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityExitMessageToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityProcessVariableMessageToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProcessVariableMessageToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityFootNoteToSite] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFootNoteToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN a.AssignedToApplicationStringGuid IS NULL THEN ''{All}'''
							+ ' WHEN p2.ID IS NULL THEN p2a.ID ELSE p2.ID END + '' - '''
							+ ' + CASE WHEN p1.ID IS NULL THEN p1a.ID ELSE p1.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipToState] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p1 ON p1.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] p1a ON p1a.ApplicationStringGuid = a.ApplicationStringGuid AND p1a._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] p2 ON p2.ApplicationStringGuid = a.AssignedToApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] p2a ON p2a.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND p2a._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteShipToState'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN a.CompanyGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipTo] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteShipTo'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN a.CompanyGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipper] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteShipper'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN a.ProductGuid IS NULL THEN ''{All}'''
							+ ' WHEN c.ProductID IS NULL THEN ca.ProductID ELSE c.ProductID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteProduct] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] c ON c.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] ca ON ca.ProductGuid = a.ProductGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteProduct'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ID'
							+ ' FROM [fmAudit].[tblQualifications] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQualifications'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityCompanyCertificateAndPermitToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyCertificateAndPermitToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentTestAndInspectionToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentTestAndInspectionToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityEquipmentTagAndLicenseToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentTagAndLicenseToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelQualificationToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelQualificationToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelLicenseToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelLicenseToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN q.ID IS NULL THEN qa.ID ELSE q.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityPersonnelTrainingToSite] a'
							+ ' LEFT JOIN [dbo].[tblQualifications] q ON q.QualificationGuid = a.QualificationGuid'
							+ ' LEFT JOIN [fmaudit].[tblQualifications] qa ON qa.QualificationGuid = a.QualificationGuid AND qa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelTrainingToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToProductGroup] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToProductGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToEntryMessage] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ProductGroupApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ProductGroupApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToEntryMessage'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToExitMessage] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ProductGroupApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ProductGroupApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToExitMessage'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblCompanyCompanyToCompanyGroup] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ApplicationStringGuid AND pa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.CompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.CompanyGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyCompanyToCompanyGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToCompanyGroup] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToCompanyGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ReasonCode'
							+ ' FROM [fmAudit].[tblAutoDistributionReasonCodes] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAutoDistributionReasonCodes'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN c.ReasonCode IS NULL THEN ca.ReasonCode ELSE c.ReasonCode END'
							+ ' FROM  [fmaudit].[map_tblEntityAutoDistributionReasonCodeToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAutoDistributionReasonCodes] c ON c.AutoDistributionReasonCodeGuid = a.AutoDistributionReasonCodeGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionReasonCodes] ca ON ca.AutoDistributionReasonCodeGuid = a.AutoDistributionReasonCodeGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAutoDistributionReasonCodeToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.RuleID'
							+ ' FROM [fmAudit].[tblAutoDistributionRule] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblManagerToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.ManagerGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.ManagerGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblManagerToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblManagerGroupToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ManagerGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ManagerGroupGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblManagerGroupToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM  [fmaudit].[map_tblProductGroupToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] p ON p.ApplicationStringGuid = a.ProductGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] pa ON pa.ApplicationStringGuid = a.ProductGroupGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductGroupToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' FROM  [fmaudit].[map_tblTransactionAliasToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblTransactionAliasToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN c.ID IS NULL THEN ca.ID ELSE c.ID END'
							+ ' FROM  [fmaudit].[map_tblOwnerToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblCompanies] c ON c.CompanyGuid = a.OwnerGuid'
							+ ' LEFT JOIN [fmaudit].[tblCompanies] ca ON ca.CompanyGuid = a.OwnerGuid AND ca._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblOwnerToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblOwnerGroupToAutoDistributionRule] a'
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.OwnerGroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.OwnerGroupGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblOwnerGroupToAutoDistributionRule'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN r.RuleID IS NULL THEN ra.RuleID ELSE r.RuleID END'
							+ ' FROM  [fmaudit].[map_tblEntityAutoDistributionRuleToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAutoDistributionRule] r ON r.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid'
							+ ' LEFT JOIN [fmaudit].[tblAutoDistributionRule] ra ON ra.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND ra._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAutoDistributionRuleToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE a.LookupListViewTypeIndex'
							+ ' WHEN 1 THEN CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' WHEN 2 THEN a.ID'
							+ ' WHEN 3 THEN a.ID END'
							+ ' FROM [fmAudit].[tblListViews] a'
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid AND a.LookupListViewTypeIndex = 1'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND a.LookupListViewTypeIndex = 1 AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [lookup].[tblListViewStandardType] l ON l.ListViewStandardTypeIndex = a.LookupListViewStandardTypeIndex AND a.LookupListViewTypeIndex = 2'  
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblListViews'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityListViewToSite'

	Update tblAuditHandler SET IDQuery = 'DECLARE @Type INT'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblListViewFields'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN v.ID IS NULL THEN va.ID ELSE v.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblProductToLedgerView] a'
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.AssignedToListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.AssignedToListViewGuid AND va._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToLedgerView'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN v.ID IS NULL THEN va.ID ELSE v.ID END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblGroupToLedgerView] a'
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToLedgerView'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN v.ID IS NULL THEN va.ID ELSE v.ID END'
							+ ' FROM [fmAudit].[map_tblEntityLedgerViewToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid'
							+ ' LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityLedgerViewToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM [fmAudit].[tblGeneralConfiguration] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblGeneralConfiguration'
	
	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblLedgerAggregateColumns] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblLedgerAggregateColumns'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN l.ID IS NULL THEN la.ID ELSE l.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityLedgerAggregateColumnToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblLedgerAggregateColumns] l ON l.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid'
							+ ' LEFT JOIN [fmaudit].[tblLedgerAggregateColumns] la ON la.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND la._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityLedgerAggregateColumnToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN l.ID IS NULL THEN la.ID ELSE l.ID END + '' - '''
							+ ' + CASE WHEN t.AliasName IS NULL THEN ta.AliasName ELSE t.AliasName END'
							+ ' FROM  [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias] a'
							+ ' LEFT JOIN [dbo].[tblLedgerAggregateColumns] l ON l.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid'
							+ ' LEFT JOIN [fmaudit].[tblLedgerAggregateColumns] la ON la.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactionAliases] t ON t.TransactionAliasGuid = a.TransactionAliasGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionAliases] ta ON ta.TransactionAliasGuid = a.TransactionAliasGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblLedgerAggregateColumnToTransactionAlias'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.IATAID'
							+ ' FROM [fmAudit].[tblIATA] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblIATA'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN i.IATAID IS NULL THEN ia.IATAID ELSE i.IATAID END'
							+ ' FROM  [fmaudit].[map_tblEntityIATACodeToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblIATA] i ON i.IATAGuid = a.IATAGuid'
							+ ' LEFT JOIN [fmaudit].[tblIATA] ia ON ia.IATAGuid = a.IATAGuid AND ia._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityIATACodeToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.TransID'
							+ ' FROM [fmAudit].[tblTransactions] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactions'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END'
							+ ' FROM [fmAudit].[tblTransactionUserData] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionUserData'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END'
							+ ' FROM [fmAudit].[tblTransactionNotes] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionNotes'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END'
							+ ' FROM [fmAudit].[tblTransactionSignature] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionSignature'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END'
							+ ' FROM [fmAudit].[tblTransactionPIDX] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblPIDXProfiles] p ON p.PIDXProfileGuid = a.PIDXProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblPIDXProfiles] pa ON pa.PIDXProfileGuid = a.PIDXProfileGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionPIDX'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + a.CompartmentID'
							+ ' FROM [fmAudit].[tblTransactionWeightReadings] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionWeightReadings'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + CONVERT(NVARCHAR,a.SequenceID+1)'
							+ ' FROM [fmAudit].[tblTransactionLineItems] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionLineItems'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN (CASE WHEN tla.TransID IS NULL THEN ta.TransID ELSE tla.TransID END) ELSE t.TransID END  + '' - '''
							+ ' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END'
							+ ' FROM [fmAudit].[tblTransactionLineItemUserData] a'
							+ ' LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = l.TransactionGuid'
							+ ' LEFT JOIN [dbo].[tblTransactions] tla ON tla.TransactionGuid = la.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = la.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionLineItemUserData'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN (CASE WHEN tla.TransID IS NULL THEN ta.TransID ELSE tla.TransID END) ELSE t.TransID END  + '' - '''
							+ ' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END + '' - '''
							+ ' + CONVERT(NVARCHAR,a.SequenceID+1)'
							+ ' FROM [fmAudit].[tblTransactionSubLineItems] a'
							+ ' LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [dbo].[tblTransactions] tla ON tla.TransactionGuid = la.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionSubLineItems'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN ta.TransID ELSE t.TransID END  + '' - '''
							+ ' + a.TransportOrderNumber'
							+ ' FROM [fmAudit].[tblTransactionTransportLineItems] a'
							+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
							+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionTransportLineItems'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ManagerName + '' - '' + a.ProductName'
							+ ' FROM [fmAudit].[tblCloseoutInventory] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCloseoutInventory'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ManagerName + '' - '' + a.OwnerName + '' - '' + a.ProductName'
							+ ' FROM [fmAudit].[tblOwnerCloseout] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblOwnerCloseout'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.GroupName'
							+ ' FROM  [fmaudit].[tblReportGroups] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblReportGroups'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ReportName'
							+ ' FROM  [fmaudit].[tblReportDetails] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblReportDetails'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN r.ReportName IS NULL THEN ra.ReportName ELSE r.ReportName END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblGroupToReportDetail] a'
							+ ' LEFT JOIN [dbo].[tblReportDetails] r ON r.ReportDetailGuid = a.ReportDetailGuid'
							+ ' LEFT JOIN [fmaudit].[tblReportDetails] ra ON ra.ReportDetailGuid = a.ReportDetailGuid AND ra._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToReportDetail'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
							+ ' FROM  [fmaudit].[map_tblEntityReportConfigurationSettingsToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityReportConfigurationSettingsToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[tblQueryDefaults] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQueryDefaults'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = REPLACE(REPLACE(a.Topic,''FMBusinessObjects.DataObjects.'',''''),''Class'','''') + '' - '' + a.FieldName'
							+ ' FROM  [fmaudit].[tblQueryDefaultFields] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQueryDefaultFields'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s2.ID IS NULL THEN sa2.ID ELSE s2.ID END + '' - '''
							+ ' + CASE WHEN s1.ID IS NULL THEN sa1.ID ELSE s1.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityQuerySettingToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s1 ON s1.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa1 ON sa1.SiteGuid = a.SiteGuid AND sa1._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s2 ON s2.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa2 ON sa2.SiteGuid = a.MapToSiteGuid AND sa2._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityQuerySettingToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.QueryName'
							+ ' FROM  [fmaudit].[tblQueryStorage] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQueryStorage'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN q.QueryName IS NULL THEN qa.QueryName ELSE q.QueryName END + '' - '''
							+ ' + CASE WHEN g.GroupID IS NULL THEN ga.GroupID ELSE g.GroupID END'
							+ ' FROM  [fmaudit].[map_tblQueryStorageToGroup] a'
							+ ' LEFT JOIN [dbo].[tblGroups] g ON g.GroupGuid = a.GroupGuid'
							+ ' LEFT JOIN [fmaudit].[tblGroups] ga ON ga.GroupGuid = a.GroupGuid AND ga._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblQueryStorage] q ON q.QueryStorageGuid = a.QueryStorageGuid'
							+ ' LEFT JOIN [fmaudit].[tblQueryStorage] qa ON qa.QueryStorageGuid = a.QueryStorageGuid AND qa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQueryStorageToGroup'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](0,a.CompanyLoadOwnerToManagerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyLoadOwnerToManager] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyLoadOwnerToManager'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](1,a.CompanyShipperToOwnerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyShipperToOwner] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyShipperToOwner'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](2,a.CompanyBillToToShipperGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyBillToToShipper] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyBillToToShipper'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](3,a.CompanyShipToToBillToGuid)'
							+ ' FROM [fmaudit].[map_tblCompanyShipToToBillTo] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyShipToToBillTo'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](3,a.CompanyShipToToBillToGuid) + '' - '''
							+ ' + a.ID'
							+ ' FROM  [fmaudit].[map_tblCompanyPersonnelToShipToBillTo] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyPersonnelToShipToBillTo'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](4,a.CompanyOffLoadOwnerToManagerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanyOffLoadOwnerToManager] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyOffLoadOwnerToManager'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](5,a.CompanySupplierToOwnerGuid)'
							+ ' FROM  [fmaudit].[map_tblCompanySupplierToOwner] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanySupplierToOwner'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = [dbo].[udf_FormatCompanyHierarchy](5,a.CompanySupplierToOwnerGuid) + '' - '''
							+ ' + a.ID'
							+ ' FROM  [fmaudit].[map_tblCompanyPersonnelToSupplierOwner] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyPersonnelToSupplierOwner'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = a.ID'
							+ ' FROM [fmAudit].[tblHouseCards] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblHouseCards'

	Update tblAuditHandler SET IDQuery =  'SELECT @ID = ''Client Settings'''
							+ ' FROM [fmAudit].[tblSyncClientConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSyncClientConfiguration'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Server'''
							+ ' FROM [fmAudit].[tblSyncServerConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSyncServerConfiguration'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Companies '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldCompany] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Companies '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueCompany] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldCompany] ud ON ud.UserDataFieldCompanyGuid = a.UserDataFieldCompanyGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldCompany] uda ON uda.UserDataFieldCompanyGuid = a.UserDataFieldCompanyGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueCompany'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Equipment '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldEquipment] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Equipment '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueEquipment] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldEquipment] ud ON ud.UserDataFieldEquipmentGuid = a.UserDataFieldEquipmentGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldEquipment] uda ON uda.UserDataFieldEquipmentGuid = a.UserDataFieldEquipmentGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueEquipment'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''FuelCard '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldFuelCard] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldFuelCard'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''FuelCard '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueFuelCard] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldFuelCard] ud ON ud.UserDataFieldFuelCardGuid = a.UserDataFieldFuelCardGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldFuelCard] uda ON uda.UserDataFieldFuelCardGuid = a.UserDataFieldFuelCardGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueFuelCard'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Personnel '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldPersonnel] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldPersonnel'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Personnel '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValuePersonnel] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldPersonnel] ud ON ud.UserDataFieldPersonnelGuid = a.UserDataFieldPersonnelGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldPersonnel] uda ON uda.UserDataFieldPersonnelGuid = a.UserDataFieldPersonnelGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValuePersonnel'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Products '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldProduct] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldProduct'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Products '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueProduct] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldProduct] ud ON ud.UserDataFieldProductGuid = a.UserDataFieldProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldProduct] uda ON uda.UserDataFieldProductGuid = a.UserDataFieldProductGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueProduct'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Sites '''
							+ ' + CONVERT(NVARCHAR,a.Number+1) + '' - '' + a.DisplayName'
							+ ' FROM [fmAudit].[tblUserDataFieldSite] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = ''Sites '''
							+ ' + CASE WHEN ud.Number IS NULL THEN CONVERT(NVARCHAR,uda.Number+1) + '' - '' + uda.DisplayName'
							+ ' ELSE CONVERT(NVARCHAR,ud.Number+1) + '' - '' + ud.DisplayName END + '' : '''
							+ ' + a.Value'
							+ ' FROM [fmAudit].[tblUserDataListValueSite] a'
							+ ' LEFT JOIN [dbo].[tblUserDataFieldSite] ud ON ud.UserDataFieldSiteGuid = a.UserDataFieldSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblUserDataFieldSite] uda ON uda.UserDataFieldSiteGuid = a.UserDataFieldSiteGuid AND uda._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - All Alarm & Events'''
							+ ' FROM  [fmaudit].[map_tblEntityUserDataToSite] a'
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.MapToSiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.MapToSiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityUserDataToSite'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN a.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,a.CompanyShipToToBillToGuid)'
							+ ' WHEN a.CompanyBillToToShipperGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](2,a.CompanyBillToToShipperGuid)'
							+ ' WHEN a.CompanyShipperToOwnerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](1,a.CompanyShipperToOwnerGuid)'
							+ ' WHEN a.CompanyLoadOwnerToManagerGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](0,a.CompanyLoadOwnerToManagerGuid)'
							+ ' ELSE ''Invalid Allocation ID'''
							+ ' END'
							+ ' + '' - '' + CONVERT(NVARCHAR,a.EffectiveDate,101)' 
							+ ' FROM [fmAudit].[tblAllocations] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAllocations'

	Update tblAuditHandler SET IDQuery = 'SELECT @ID = CASE WHEN s.CompanyShipToToBillToGuid IS NOT NULL THEN [dbo].[udf_FormatCompanyHierarchy](3,s.CompanyShipToToBillToGuid)'
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
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAllocationLineItems'

	
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGroupGuid FROM [fmaudit].[erv_tblEntityRecordVersioningFieldConfig] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'erv_tblEntityRecordVersioningFieldConfig'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblAlarmPriorityToEmailGroup] a LEFT JOIN [dbo].[tblEmailGroups] s ON s.EmailGroupGuid = a.EmailGroupGuid LEFT JOIN [fmaudit].[tblEmailGroups] sa ON sa.EmailGroupGuid = a.EmailGroupGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblAlarmPriorityToEmailGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToAlarmEventCategory] a LEFT JOIN [dbo].[tblEmailGroups] s ON s.EmailGroupGuid = a.EmailGroupGuid LEFT JOIN [fmaudit].[tblEmailGroups] sa ON sa.EmailGroupGuid = a.EmailGroupGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToAlarmEventCategory'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToDotHazardousMessage] a LEFT JOIN [dbo].[tblProducts] s ON s.ProductGuid = a.ProductGuid LEFT JOIN [fmaudit].[tblProducts] sa ON sa.ProductGuid = a.ProductGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToDotHazardous'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToEmailAddress] a LEFT JOIN [dbo].[tblEmailGroups] s ON s.EmailGroupGuid = a.EmailGroupGuid LEFT JOIN [fmaudit].[tblEmailGroups] sa ON sa.EmailGroupGuid = a.EmailGroupGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToEmailAddress'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToEntryMessage] a LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToEntryMessage'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToExitMessage] a LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToExitMessage'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToFootNoteProduct] a LEFT JOIN [dbo].[tblProducts] s ON s.ProductGuid = a.ProductGuid LEFT JOIN [fmaudit].[tblProducts] sa ON sa.ProductGuid = a.ProductGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteProduct'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipper] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.CompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.CompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteShipper'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipTo] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.CompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.CompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteShipTo'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToFootNoteShipToState] a LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToFootNoteShipToState'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblApplicationStringToProductMessage] a LEFT JOIN [dbo].[tblProducts] s ON s.ProductGuid = a.ProductGuid LEFT JOIN [fmaudit].[tblProducts] sa ON sa.ProductGuid = a.ProductGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblApplicationStringToProductMessage'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblAssociatedTransactionAliases] a LEFT JOIN [dbo].[tblTransactionAliases] s ON s.TransactionAliasGuid = a.ParentTransactionAliasGuid LEFT JOIN [fmaudit].[tblTransactionAliases] sa ON sa.TransactionAliasGuid = a.ParentTransactionAliasGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblAssociatedTransactionAliases'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyAuthorizedCarrierToCompany] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyAuthorizedCarrierToCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyBillToToShipper] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyBillToToShipper'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyCompanyToCompanyGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyCompanyToCompanyGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyCompanyToUserGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyCompanyToUserGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyLoadOwnerToManager] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyLoadOwnerToManager'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyOffLoadOwnerToManager] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyOffLoadOwnerToManager'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyPersonnelToShipToBillTo] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyPersonnelToShipToBillTo'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyPersonnelToSupplierOwner] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyPersonnelToSupplierOwner'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyShipperToOwner] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyShipperToOwner'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyShipToToBillTo] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyShipToToBillTo'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanySupplierToOwner] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanySupplierToOwner'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblCompanyToRole] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblCompanyToRole'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAdditiveProfileToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAdditiveProfileToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAlarmAndEventCategoryToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmAndEventCategoryToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = MapToSiteGuid FROM [fmaudit].[map_tblEntityAlarmAndEventToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmAndEventToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAlarmPriorityToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAlarmPriorityToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAllocationGroupToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAllocationGroupToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAppointmentEquipmentToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAppointmentEquipmentToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAppointmentPersonnelToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAppointmentPersonnelToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAppointmentTankToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAppointmentTankToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAutoDistributionReasonCodeToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAutoDistributionReasonCodeToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityAutoDistributionRuleToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityAutoDistributionRuleToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityCompanyCertificateAndPermitToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyCertificateAndPermitToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityCompanyGroupToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyGroupToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityCompanyToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityCompanyTypeToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityCompanyTypeToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityFuelCardTypeToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFuelCardTypeToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = MapToSiteGuid FROM [fmaudit].[map_tblEntityDataDictionaryToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityDataDictionaryToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityDispatchConfigurationToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityDispatchConfigurationToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityDotHazardousMessagesToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityDotHazardousMessagesToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEmailAddressToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEmailAddressToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEmailGroupToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEmailGroupToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEntryMessageToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEntryMessageToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEquipmentTagAndLicenseToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentTagAndLicenseToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEquipmentTestAndInspectionToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentTestAndInspectionToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEquipmentToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityEquipmentTypeToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityEquipmentTypeToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityExitMessageToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityExitMessageToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityFootNoteToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFootNoteToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityFuelCardLimitToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFuelCardLimitToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityFuelCardToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityFuelCardToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityIATACodeToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityIATACodeToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityLedgerAggregateColumnToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityLedgerAggregateColumnToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityLedgerViewToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityLedgerViewToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityListViewToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityListViewToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPersonnelLicenseToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelLicenseToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPersonnelQualificationToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelQualificationToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPersonnelToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityPersonnelTrainingToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityPersonnelTrainingToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityProcessVariableMessageToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProcessVariableMessageToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityProductGroupToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProductGroupToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityProductMessageToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProductMessageToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityProductToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityProductToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityQualityTagToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityQualityTagToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = MapToSiteGuid FROM [fmaudit].[map_tblEntityQuerySettingToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityQuerySettingToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = MapToSiteGuid FROM [fmaudit].[map_tblEntityReportConfigurationSettingsToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityReportConfigurationSettingsToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityTestSetToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityTestSetToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityTestToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityTestToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityTransactionAliasToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityTransactionAliasToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = MapToSiteGuid FROM [fmaudit].[map_tblEntityUserDataToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityUserDataToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityUserGroupToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityUserGroupToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityUserToSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblEntityUserToSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblFuelCardLimitToFuelCard] a LEFT JOIN [dbo].[tblFuelCards] s ON s.FuelCardGuid = a.FuelCardGuid LEFT JOIN [fmaudit].[tblFuelCards] sa ON sa.FuelCardGuid = a.FuelCardGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblFuelCardLimitToFuelCard'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblGroupToLedgerView] a LEFT JOIN [dbo].[tblListViews] s ON s.ListViewGuid = a.ListViewGuid LEFT JOIN [fmaudit].[tblListViews] sa ON sa.ListViewGuid = a.ListViewGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToLedgerView'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblGroupToReportDetail] a LEFT JOIN [dbo].[tblReportDetails] s ON s.ReportDetailGuid = a.ReportDetailGuid LEFT JOIN [fmaudit].[tblReportDetails] sa ON sa.ReportDetailGuid = a.ReportDetailGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToReportDetail'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblGroupToRight] a LEFT JOIN [dbo].[tblGroups] s ON s.GroupGuid = a.GroupGuid LEFT JOIN [fmaudit].[tblGroups] sa ON sa.GroupGuid = a.GroupGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToRight'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblGroupToTransactionAlias] a LEFT JOIN [dbo].[tblTransactionAliases] s ON s.TransactionAliasGuid = a.TransactionAliasGuid LEFT JOIN [fmaudit].[tblTransactionAliases] sa ON sa.TransactionAliasGuid = a.TransactionAliasGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblGroupToTransactionAlias'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblLedgerAggregateColumnToTransactionAlias] a LEFT JOIN [dbo].[tblLedgerAggregateColumns] s ON s.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid LEFT JOIN [fmaudit].[tblLedgerAggregateColumns] sa ON sa.LedgerAggregateColumnGuid = a.LedgerAggregateColumnGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblLedgerAggregateColumnToTransactionAlias'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblManagerGroupToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblManagerGroupToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblManagerToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblManagerToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblMeterToTank] a LEFT JOIN [dbo].[tblTanks] s ON s.TankGuid = a.TankGuid LEFT JOIN [fmaudit].[tblTanks] sa ON sa.TankGuid = a.TankGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblMeterToTank'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblOwnerGroupToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblOwnerGroupToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblOwnerToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblOwnerToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblPersonnelToRole] a LEFT JOIN [dbo].[tblPersonnel] s ON s.PersonnelGuid = a.PersonnelGuid LEFT JOIN [fmaudit].[tblPersonnel] sa ON sa.PersonnelGuid = a.PersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblPersonnelToRole'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblPIDXProfileToCompany] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblPIDXProfileToCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductGroupToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductGroupToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToAdditiveProfile] a LEFT JOIN [dbo].[tblAdditiveProfiles] s ON s.AdditiveProfileGuid = a.AssignedToAdditiveProfileGuid LEFT JOIN [fmaudit].[tblAdditiveProfiles] sa ON sa.AdditiveProfileGuid = a.AssignedToAdditiveProfileGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToAdditiveProfile'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToBlendComponent] a LEFT JOIN [dbo].[tblProducts] s ON s.ProductGuid = a.AssignedToProductGuid LEFT JOIN [fmaudit].[tblProducts] sa ON sa.ProductGuid = a.AssignedToProductGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToBlendComponent'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToCompany] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.AssignedToCompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.AssignedToCompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToCompanyGroup] a LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToCompanyGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToLedgerView] a LEFT JOIN [dbo].[tblListViews] s ON s.ListViewGuid = a.AssignedToListViewGuid LEFT JOIN [fmaudit].[tblListViews] sa ON sa.ListViewGuid = a.AssignedToListViewGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToLedgerView'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'map_tblProductToPresetComponentTankOrTankGroup'

UPDATE tblAuditHandler SET SiteGuidQuery = 
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'map_tblProductToPresetExternalComponent'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'map_tblProductToPresetInjector'

UPDATE tblAuditHandler SET SiteGuidQuery = 
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'map_tblProductToPresetRecipe'

UPDATE tblAuditHandler SET SiteGuidQuery = 
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetFlowControlledAdditive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'map_tblProductToPresetFlowControlledAdditive'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToProductGroup] a LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.AssignedToApplicationStringGuid LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.AssignedToApplicationStringGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToProductGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToSupplierProductCompany] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.AssignedToCompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.AssignedToCompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToSupplierProductCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToTransactionAliasExclusion] a LEFT JOIN [dbo].[tblTransactionAliases] s ON s.TransactionAliasGuid = a.AssignedToTransactionAliasGuid LEFT JOIN [fmaudit].[tblTransactionAliases] sa ON sa.TransactionAliasGuid = a.AssignedToTransactionAliasGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToTransactionAliasExclusion'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblProductToUnavailableInventoryCompany] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.AssignedToCompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.AssignedToCompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblProductToUnavailableInventoryCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationCompanyCertificateAndPermitToCompany] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.CompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.CompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationCompanyCertificateAndPermitToCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationEquipmentTagAndLicenseToEquipment] a LEFT JOIN [dbo].[tblEquipment] s ON s.EquipmentGuid = a.EquipmentGuid LEFT JOIN [fmaudit].[tblEquipment] sa ON sa.EquipmentGuid = a.EquipmentGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationEquipmentTagAndLicenseToEquipment'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationEquipmentTestAndInspectionToEquipment] a LEFT JOIN [dbo].[tblEquipment] s ON s.EquipmentGuid = a.EquipmentGuid LEFT JOIN [fmaudit].[tblEquipment] sa ON sa.EquipmentGuid = a.EquipmentGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationEquipmentTestAndInspectionToEquipment'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationEquipmentTestAndInspectionToStation] a LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationEquipmentTestAndInspectionToStation'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonLicenseToPerson] a LEFT JOIN [dbo].[tblPersonnel] s ON s.PersonnelGuid = a.PersonnelGuid LEFT JOIN [fmaudit].[tblPersonnel] sa ON sa.PersonnelGuid = a.PersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonLicenseToPerson'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonQualificationToEquipmentType] a LEFT JOIN [dbo].[tblEquipmentTypes] s ON s.EquipmentTypeGuid = a.EquipmentTypeGuid LEFT JOIN [fmaudit].[tblEquipmentTypes] sa ON sa.EquipmentTypeGuid = a.EquipmentTypeGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonQualificationToEquipmentType'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonLicenseToPerson] a LEFT JOIN [dbo].[tblPersonnel] s ON s.PersonnelGuid = a.PersonnelGuid LEFT JOIN [fmaudit].[tblPersonnel] sa ON sa.PersonnelGuid = a.PersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonQualificationToPerson'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonQualificationToStation] a LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonQualificationToStation'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonTrainingToEquipmentType] a LEFT JOIN [dbo].[tblEquipmentTypes] s ON s.EquipmentTypeGuid = a.EquipmentTypeGuid LEFT JOIN [fmaudit].[tblEquipmentTypes] sa ON sa.EquipmentTypeGuid = a.EquipmentTypeGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonTrainingToEquipmentType'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonTrainingToPerson] a LEFT JOIN [dbo].[tblPersonnel] s ON s.PersonnelGuid = a.PersonnelGuid LEFT JOIN [fmaudit].[tblPersonnel] sa ON sa.PersonnelGuid = a.PersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonTrainingToPerson'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQualificationPersonTrainingToStation] a LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQualificationPersonTrainingToStation'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblQueryStorageToGroup] a LEFT JOIN [dbo].[tblGroups] s ON s.GroupGuid = a.GroupGuid LEFT JOIN [fmaudit].[tblGroups] sa ON sa.GroupGuid = a.GroupGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblQueryStorageToGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblTankToTankGroup] a LEFT JOIN [dbo].[tblTankGroups] s ON s.TankGroupGuid = a.AssignedToTankGroupGuid LEFT JOIN [fmaudit].[tblTankGroups] sa ON sa.TankGroupGuid = a.AssignedToTankGroupGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblTankToTankGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.OwnerSiteGuid IS NULL THEN sa.OwnerSiteGuid ELSE s.OwnerSiteGuid END FROM  [fmaudit].[map_tblTestDefinitionToTestSetDefinition] a LEFT JOIN [dbo].[tblTestSetDefinitions] s ON s.TestSetDefinitionGuid = a.TestSetDefinitionGuid LEFT JOIN [fmaudit].[tblTestSetDefinitions] sa ON sa.TestSetDefinitionGuid = a.TestSetDefinitionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' WHERE TableName = 'map_tblTestDefinitionToTestSetDefinition'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblTransactionAliasToAutoDistributionRule] a LEFT JOIN [dbo].[tblAutoDistributionRule] s ON s.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid LEFT JOIN [fmaudit].[tblAutoDistributionRule] sa ON sa.AutoDistributionRuleGuid = a.AutoDistributionRuleGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblTransactionAliasToAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[map_tblTransactionAliasToStatus] a LEFT JOIN [dbo].[tblTransactionAliases] s ON s.TransactionAliasGuid = a.TransactionAliasGuid LEFT JOIN [fmaudit].[tblTransactionAliases] sa ON sa.TransactionAliasGuid = a.TransactionAliasGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblTransactionAliasToStatus'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblUserToGroup] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'map_tblUserToGroup'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAdditiveProfiles] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAdditiveProfiles'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblAirplaneTank] a LEFT JOIN [dbo].[tblEquipmentTypes] s ON s.EquipmentTypeGuid = a.EquipmentTypeGuid LEFT JOIN [fmaudit].[tblEquipmentTypes] sa ON sa.EquipmentTypeGuid = a.EquipmentTypeGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAirplaneTank'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAlarmAndEvents] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAlarmAndEvents'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblAllocationLineItems] a LEFT JOIN [dbo].[tblAllocations] s ON s.AllocationGuid = a.AllocationGuid LEFT JOIN [fmaudit].[tblAllocations] sa ON sa.AllocationGuid = a.AllocationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAllocationLineItems'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAllocations] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAllocations'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblApplicationString] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblApplicationString'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAppointmentEquipment] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAppointmentEquipment'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAppointmentPersonnel] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAppointmentPersonnel'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAppointmentTank] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAppointmentTank'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblArchivedUsers] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblArchivedUsers'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAutoDistributionReasonCodes] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAutoDistributionReasonCodes'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAutoDistributionRule] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblAutoDistributionRule'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblCloseoutInventory] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCloseoutInventory'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblCompanies] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCompanies'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = _AuditSiteGuid FROM [fmaudit].[tblConfigurationSetting] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblConfigurationSetting'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblControllersLog] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblControllersLog'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblCustomToolbar] a LEFT JOIN [dbo].[tblDispatchConfiguration] s ON s.DispatchConfigurationGuid = a.DispatchConfigurationGuid LEFT JOIN [fmaudit].[tblDispatchConfiguration] sa ON sa.DispatchConfigurationGuid = a.DispatchConfigurationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblCustomToolbar'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @CustomToolBarGuid UNIQUEIDENTIFIER'
	+'		,@DispatchConfigurationGuid UNIQUEIDENTIFIER;'
	+'SELECT @CustomToolBarGuid = CustomToolBarGuid FROM [fmaudit].[tblCustomToolbarCommand] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @DispatchConfigurationGuid = DispatchConfigurationGuid FROM [fmaudit].[tblCustomToolBar] WHERE CustomToolBarGuid = @CustomToolBarGuid AND _AuditEventType = ''D'';'
	+'IF @DispatchConfigurationGuid IS NULL'
	+'	SELECT @DispatchConfigurationGuid = DispatchConfigurationGuid FROM [dbo].[tblCustomToolBar] WHERE CustomToolBarGuid = @CustomToolBarGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblDispatchConfiguration] WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblDispatchConfiguration] WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid;'
	WHERE TableName = 'tblCustomToolbarCommand'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblDataDictionaries] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDataDictionaries'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblDispatchConfiguration] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDispatchConfiguration'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblDispatchGrid] a LEFT JOIN [dbo].[tblDispatchConfiguration] s ON s.DispatchConfigurationGuid = a.DispatchConfigurationGuid LEFT JOIN [fmaudit].[tblDispatchConfiguration] sa ON sa.DispatchConfigurationGuid = a.DispatchConfigurationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblDispatchGrid'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @DispatchGridGuid UNIQUEIDENTIFIER'
	+'		,@DispatchConfigurationGuid UNIQUEIDENTIFIER;'
	+'SELECT @DispatchGridGuid = CustomToolBarGuid FROM [fmaudit].[tblCustomToolbarCommand] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @DispatchConfigurationGuid = DispatchConfigurationGuid FROM [fmaudit].[tblDispatchGrid] WHERE DispatchGridGuid = @DispatchGridGuid AND _AuditEventType = ''D'';'
	+'IF @DispatchConfigurationGuid IS NULL'
	+'	SELECT @DispatchConfigurationGuid = DispatchConfigurationGuid FROM [dbo].[tblDispatchGrid] WHERE DispatchGridGuid = @DispatchGridGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblDispatchConfiguration] WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblDispatchConfiguration] WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid;'
	WHERE TableName = 'tblDispatchGridColumn'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblEmailGroups] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEmailGroups'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblEquipment] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipment'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblEquipmentMaintenanceLog] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipmentMaintenanceLog'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblEquipmentQualityTagLog] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipmentQualityTagLog'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblEquipmentTypes] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblEquipmentTypes'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001''' WHERE TableName = 'tblExportRequest'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblFuelCardLimit] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblFuelCardLimit'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblFuelCardLimitLineItem] a LEFT JOIN [dbo].[tblFuelCardLimit] s ON s.FuelCardLimitGuid = a.FuelCardLimitGuid LEFT JOIN [fmaudit].[tblFuelCardLimit] sa ON sa.FuelCardLimitGuid = a.FuelCardLimitGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblFuelCardLimitLineItem'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblFuelCards] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblFuelCards'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblGates] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblGates'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblGeneralConfiguration] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblGeneralConfiguration'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblGroups] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblGroups'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblHouseCards] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblHouseCards'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblIATA] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblIATA'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblLedgerAggregateColumns] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblLedgerAggregateColumns'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblListViewFields] a LEFT JOIN [dbo].[tblListViews] s ON s.ListViewGuid = a.ListViewGuid LEFT JOIN [fmaudit].[tblListViews] sa ON sa.ListViewGuid = a.ListViewGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblListViewFields'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblListViews] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblListViews'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = LoadArmGuid FROM [fmaudit].[tblLoadArms] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblLoadArms'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001''' WHERE TableName = 'tblMaintenanceReasons'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblMenuFavorites] a LEFT JOIN [dbo].[tblUsers] s ON s.UserGuid = a.UserGuid LEFT JOIN [fmaudit].[tblUsers] sa ON sa.UserGuid = a.UserGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMenuFavorites'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblMessages] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMessages'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblMeter] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblMeter'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001''' WHERE TableName = 'tblNotes'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001''' WHERE TableName = 'tblOPCConnections'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblOwnerCloseout] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblOwnerCloseout'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPersonnel] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblPersonnel'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblPIDXProfiles] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblPIDXProfiles'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetInjectorGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetInjectorGuid = ProductToPresetInjectorGuid FROM [fmaudit].[tblProcessVariableAdditiveInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableAdditiveInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetInjectorGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetInjectorGuid = ProductToPresetInjectorGuid FROM [fmaudit].[tblProcessVariableAdditiveOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableAdditiveOutputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetComponentTankOrTankGroupGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetComponentTankOrTankGroupGuid = ProductToPresetComponentTankOrTankGroupGuid FROM [fmaudit].[tblProcessVariableComponentInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableComponentInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetComponentTankOrTankGroupGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetComponentTankOrTankGroupGuid = ProductToPresetComponentTankOrTankGroupGuid FROM [fmaudit].[tblProcessVariableComponentOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetComponentTankOrTankGroup] WHERE ProductToPresetComponentTankOrTankGroupGuid = @ProductToPresetComponentTankOrTankGroupGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableComponentOutputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblProcessVariableEquipment] a LEFT JOIN [dbo].[tblEquipment] s ON s.EquipmentGuid = a.EquipmentGuid LEFT JOIN [fmaudit].[tblEquipment] sa ON sa.EquipmentGuid = a.EquipmentGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableEquipment'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetExternalComponentGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetExternalComponentGuid = ProductToPresetExternalComponentGuid FROM [fmaudit].[tblProcessVariableExternalComponentBlendPercentage] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetExternalComponent] WHERE ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetExternalComponent] WHERE ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableExternalComponentBlendPercentage'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetExternalComponentGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetExternalComponentGuid = ProductToPresetExternalComponentGuid FROM [fmaudit].[tblProcessVariableExternalComponentInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetExternalComponent] WHERE ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetExternalComponent] WHERE ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableExternalComponentInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetExternalComponentGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetExternalComponentGuid = ProductToPresetExternalComponentGuid FROM [fmaudit].[tblProcessVariableExternalComponentOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetExternalComponent] WHERE ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetExternalComponent] WHERE ProductToPresetExternalComponentGuid = @ProductToPresetExternalComponentGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableExternalComponentOutputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = LoadArmGuid FROM [fmaudit].[tblProcessVariableLoadArm] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableLoadArm'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = LoadArmGuid FROM [fmaudit].[tblProcessVariableLoadArmInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableLoadArmInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = LoadArmGuid FROM [fmaudit].[tblProcessVariableLoadArmOutPutPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableLoadArmOutPutPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = LoadArmGuid FROM [fmaudit].[tblProcessVariableNoAdditiveInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableNoAdditiveInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @LoadArmGuid = LoadArmGuid FROM [fmaudit].[tblProcessVariableNoAdditiveOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableNoAdditiveOutputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetInjectorGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetInjectorGuid = ProductToPresetInjectorGuid FROM [fmaudit].[tblProcessVariablePresetInjector] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetInjector] WHERE ProductToPresetInjectorGuid = @ProductToPresetInjectorGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariablePresetInjector'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetRecipeGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetRecipeGuid = ProductToPresetRecipeGuid FROM [fmaudit].[tblProcessVariableRecipeInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetRecipe] WHERE ProductToPresetRecipeGuid = @ProductToPresetRecipeGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetRecipe] WHERE ProductToPresetRecipeGuid = @ProductToPresetRecipeGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableRecipeInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetRecipeGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetRecipeGuid = ProductToPresetRecipeGuid FROM [fmaudit].[tblProcessVariableRecipeOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetRecipe] WHERE ProductToPresetRecipeGuid = @ProductToPresetRecipeGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetRecipe] WHERE ProductToPresetRecipeGuid = @ProductToPresetRecipeGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableRecipeOutputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetFlowControlledAdditiveGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetFlowControlledAdditiveGuid = ProductToPresetFlowControlledAdditiveGuid FROM [fmaudit].[tblProcessVariableFlowControlledAdditiveInputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableFlowControlledAdditiveInputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery =
	'DECLARE @ProductToPresetFlowControlledAdditiveGuid UNIQUEIDENTIFIER'
	+'		,@LoadArmGuid UNIQUEIDENTIFIER'
	+'		,@StationGuid UNIQUEIDENTIFIER;'
	+'SELECT @ProductToPresetFlowControlledAdditiveGuid = ProductToPresetFlowControlledAdditiveGuid FROM [fmaudit].[tblProcessVariableFlowControlledAdditiveOutputPermissive] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
	+'SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [fmaudit].[map_tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid AND _AuditEventType = ''D'';'
	+'IF @LoadArmGuid IS NULL'
	+'	SELECT @LoadArmGuid = AssignedToLoadArmGuid FROM [map].[tblProductToPresetFlowControlledAdditive] WHERE ProductToPresetFlowControlledAdditiveGuid = @ProductToPresetFlowControlledAdditiveGuid;'
	+'SELECT @StationGuid = BayAStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [fmaudit].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid AND _AuditEventType = ''D'';'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayAStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'IF @StationGuid IS NULL'
	+'	SELECT @StationGuid = BayBStationGuid FROM [dbo].[tblLoadArms] WHERE LoadArmGuid = @LoadArmGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] WHERE StationGuid = @StationGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid = SiteGuid FROM [dbo].[tblStations] WHERE StationGuid = @StationGuid;'
	WHERE TableName = 'tblProcessVariableFlowControlledAdditiveOutputPermissive'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblProcessVariableSite] a LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblProcessVariableStation] a LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableStation'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblProcessVariableStationInputPermissive] a LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableStationInputPermissive'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblProcessVariableStationOutputPermissive] a LEFT JOIN [dbo].[tblStations] s ON s.StationGuid = a.StationGuid LEFT JOIN [fmaudit].[tblStations] sa ON sa.StationGuid = a.StationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableStationOutputPermissive'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblProcessVariableTank] a LEFT JOIN [dbo].[tblTanks] s ON s.TankGuid = a.TankGuid LEFT JOIN [fmaudit].[tblTanks] sa ON sa.TankGuid = a.TankGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProcessVariableTank'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblProducts] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblProducts'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblQualifications] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQualifications'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblQualityTags] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQualityTags'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblQueryDefaultFields] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQueryDefaultFields'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblQueryDefaults] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQueryDefaults'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblQueryStorage] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblQueryStorage'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblReportDetails] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblReportDetails'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblReportGroups] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblReportGroups'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblReserveLevels] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblReserveLevels'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblScheduleCompanyAccess] a LEFT JOIN [dbo].[tblCompanies] s ON s.CompanyGuid = a.CompanyGuid LEFT JOIN [fmaudit].[tblCompanies] sa ON sa.CompanyGuid = a.CompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblScheduleCompanyAccess'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblScheduleHoliday] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblScheduleHoliday'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblSchedulePersonnelAccess] a LEFT JOIN [dbo].[tblPersonnel] s ON s.PersonnelGuid = a.PersonnelGuid LEFT JOIN [fmaudit].[tblPersonnel] sa ON sa.PersonnelGuid = a.PersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSchedulePersonnelAccess'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblScheduleTerminalOperation] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblScheduleTerminalOperation'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM tblSites WHERE SiteGuid = ''00000000-0000-0000-0000-000000000001''' WHERE TableName = 'tblSettings'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblSites] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSites'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblSitesAncillaryData] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSitesAncillaryData'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblStations] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblStations'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = _AuditSiteGuid FROM [fmaudit].[tblSyncClientConfiguration] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSyncClientConfiguration'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = _AuditSiteGuid FROM [fmaudit].[tblSyncServerConfiguration] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSyncServerConfiguration'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = _AuditSiteGuid FROM [fmaudit].[tblSystemSettings] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblSystemSettings'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTankGroups] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTankGroups'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTankMaintenanceLog] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTankMaintenanceLog'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTankQualityTagLog] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTankQualityTagLog'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTanks] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTanks'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = OwnerSiteGuid FROM [fmaudit].[tblTestDefinitions] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestDefinitions'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTestEquipmentResults] a LEFT JOIN [dbo].[tblTestSetEquipmentResults] s ON s.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid LEFT JOIN [fmaudit].[tblTestSetEquipmentResults] sa ON sa.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestEquipmentResults'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = OwnerSiteGuid FROM [fmaudit].[tblTestSetDefinitions] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetDefinitions'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTestSetTankResults] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetTankResults'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTestSetTankResults] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetTankResults'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTestEquipmentResults] a LEFT JOIN [dbo].[tblTestSetEquipmentResults] s ON s.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid LEFT JOIN [fmaudit].[tblTestSetEquipmentResults] sa ON sa.TestSetEquipmentResultGuid = a.TestSetEquipmentResultGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTestSetEquipmentResults'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTransactionAliases] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionAliases'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionAliasFields] a LEFT JOIN [dbo].[tblTransactionAliases] s ON s.TransactionAliasGuid = a.TransactionAliasGuid LEFT JOIN [fmaudit].[tblTransactionAliases] sa ON sa.TransactionAliasGuid = a.TransactionAliasGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionAliasFields'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionLineItems] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionLineItems'

UPDATE tblAuditHandler SET SiteGuidQuery = 
	+'DECLARE @TransactionLineItemGuid UNIQUEIDENTIFIER'
	+'		, @TransactionGuid UNIQUEIDENTIFIER;'
	+'SELECT @TransactionLineItemGuid = TransactionLineItemGuid FROM [fmaudit].[tblTransactionLineItemUserData] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID;'
	+'SELECT @TransactionGuid = TransactionGuid FROM [fmaudit].[tblTransactionLineItems] WHERE TransactionLineItemGuid = @TransactionLineItemGuid AND _AuditEventType = ''D'';'
	+'IF @TransactionGuid IS NULL'
	+'	SELECT @TransactionGuid = TransactionGuid FROM [dbo].[tblTransactionLineItems] WHERE TransactionLineItemGuid = @TransactionLineItemGuid;'
	+'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTransactions] WHERE TransactionGuid = @TransactionGuid AND _AuditEventType = ''D'';'
	+'IF @SiteGuid IS NULL'
	+'	SELECT @SiteGuid  = SiteGuid FROM [dbo].[tblTransactions] WHERE TransactionGuid = @TransactionGuid;'
	WHERE TableName = 'tblTransactionLineItemUserData'

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionNotes] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionNotes'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionPIDX] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionPIDX'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblTransactions] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactions'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionSignature] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionSignature'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionSubLineItems] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionSubLineItems'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionTransportLineItems] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionTransportLineItems'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionUserData] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionUserData'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblTransactionWeightReadings] a LEFT JOIN [dbo].[tblTransactions] s ON s.TransactionGuid = a.TransactionGuid LEFT JOIN [fmaudit].[tblTransactions] sa ON sa.TransactionGuid = a.TransactionGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblTransactionWeightReadings'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldCompany] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldEquipment] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldEquipment'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldFuelCard] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldFuelCard'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldPersonnel] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldPersonnel'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldProduct] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldProduct'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldSite] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldTransactionAlias] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldTransactionAlias'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUserDataFieldTransactionAliasLineItem] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataFieldTransactionAliasLineItem'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueCompany] a LEFT JOIN [dbo].[tblUserDataFieldCompany] s ON s.UserDataFieldCompanyGuid = a.UserDataFieldCompanyGuid LEFT JOIN [fmaudit].[tblUserDataFieldCompany] sa ON sa.UserDataFieldCompanyGuid = a.UserDataFieldCompanyGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueCompany'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueEquipment] a LEFT JOIN [dbo].[tblUserDataFieldEquipment] s ON s.UserDataFieldEquipmentGuid = a.UserDataFieldEquipmentGuid LEFT JOIN [fmaudit].[tblUserDataFieldEquipment] sa ON sa.UserDataFieldEquipmentGuid = a.UserDataFieldEquipmentGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueEquipment'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueFuelCard] a LEFT JOIN [dbo].[tblUserDataFieldFuelCard] s ON s.UserDataFieldFuelCardGuid = a.UserDataFieldFuelCardGuid LEFT JOIN [fmaudit].[tblUserDataFieldFuelCard] sa ON sa.UserDataFieldFuelCardGuid = a.UserDataFieldFuelCardGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueFuelCard'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValuePersonnel] a LEFT JOIN [dbo].[tblUserDataFieldPersonnel] s ON s.UserDataFieldPersonnelGuid = a.UserDataFieldPersonnelGuid LEFT JOIN [fmaudit].[tblUserDataFieldPersonnel] sa ON sa.UserDataFieldPersonnelGuid = a.UserDataFieldPersonnelGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValuePersonnel'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueProduct] a LEFT JOIN [dbo].[tblUserDataFieldProduct] s ON s.UserDataFieldProductGuid = a.UserDataFieldProductGuid LEFT JOIN [fmaudit].[tblUserDataFieldProduct] sa ON sa.UserDataFieldProductGuid = a.UserDataFieldProductGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueProduct'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueSite] a LEFT JOIN [dbo].[tblUserDataFieldSite] s ON s.UserDataFieldSiteGuid = a.UserDataFieldSiteGuid LEFT JOIN [fmaudit].[tblUserDataFieldSite] sa ON sa.UserDataFieldSiteGuid = a.UserDataFieldSiteGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueSite'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueTransactionAlias] a LEFT JOIN [dbo].[tblUserDataFieldTransactionAlias] s ON s.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAlias] sa ON sa.UserDataFieldTransactionAliasGuid = a.UserDataFieldTransactionAliasGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueTransactionAlias'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM  [fmaudit].[tblUserDataListValueTransactionAliasLineItem] a LEFT JOIN [dbo].[tblUserDataFieldTransactionAliasLineItem] s ON s.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid LEFT JOIN [fmaudit].[tblUserDataFieldTransactionAliasLineItem] sa ON sa.UserDataFieldTransactionAliasLineItemGuid = a.UserDataFieldTransactionAliasLineItemGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUserDataListValueTransactionAliasLineItem'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblUsers] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID ' WHERE TableName = 'tblUsers'
GO


IF EXISTS (SELECT 1 FROM [dbo].[tblSyncClientConfiguration] WHERE [ServiceMaximumRetryAttempts] IS NULL OR [ServiceRetryWaitTime] IS NULL)
BEGIN
    UPDATE [dbo].[tblSyncClientConfiguration] 
        SET [ServiceMaximumRetryAttempts] = 3 WHERE [ServiceMaximumRetryAttempts] IS NULL

    UPDATE [dbo].[tblSyncClientConfiguration] 
        SET [ServiceRetryWaitTime] = 3000 WHERE [ServiceRetryWaitTime] IS NULL
END

-- Update Administrators Password
UPDATE tblUsers set PasswordTimeStamp = sysdatetimeoffset() where UserGuid = '00000000-0000-0000-0000-000000000002'



IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'IsEnterprise' )
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C9B14E04-8B70-4D38-9FB7-C4351D52963A', N'DWORD', N'IsEnterprise', N'0', N'8/20/2014 3:24:27 PM -04:00', 'Administrator', N'8/20/2014 3:24:27 PM -04:00', 'Administrator');
END

-- 
IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = N'InstallDetailsSynchronizationNodeName' )
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C0601CA3-5DB5-441B-A75D-CDBD4379FD19', N'SZ', N'InstallDetailsSynchronizationNodeName', N'', N'10/1/2012 3:57:48 PM -04:00', N'Administrator', N'10/1/2012 3:57:48 PM -04:00', N'Administrator')
END

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_CATALOG = DB_NAME() AND TABLE_NAME='tblSites')
AND EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_CATALOG = DB_NAME() AND TABLE_NAME='tblSitesShadow')
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
GO

-- WCG 9/8/2014 - Not clear why this is commmented out.

--IF OBJECT_ID('FK_tblAuditLog_SiteGuid', 'F') IS NULL 
--BEGIN
--	ALTER TABLE [dbo].[tblAuditLog] ADD CONSTRAINT FK_tblAuditLog_ShadowSiteGuid FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSitesShadow] ([SiteGuid])
--END

UPDATE [dbo].[tblAuditHandler]
SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN (CASE WHEN tla.TransID IS NULL THEN ta.TransID ELSE tla.TransID END) ELSE t.TransID END  + '' - '''
				+ ' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END'
				+ ' FROM [fmAudit].[tblTransactionLineItemUserData] a'
				+ ' LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid'
				+ ' LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'''
				+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = l.TransactionGuid'
				+ ' LEFT JOIN [dbo].[tblTransactions] tla ON tla.TransactionGuid = la.TransactionGuid'
				+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = la.TransactionGuid AND ta._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
WHERE TableName = 'tblTransactionLineItemUserData'

UPDATE [dbo].[tblAuditHandler]
SET IDQuery = 'SELECT @ID = CASE WHEN t.TransID IS NULL THEN (CASE WHEN tla.TransID IS NULL THEN ta.TransID ELSE tla.TransID END) ELSE t.TransID END  + '' - '''
				+ ' + CASE WHEN l.SequenceID IS NULL THEN CONVERT(NVARCHAR,la.SequenceID+1) ELSE CONVERT(NVARCHAR,l.SequenceID+1) END + '' - '''
				+ ' + CONVERT(NVARCHAR,a.SequenceID+1)'
				+ ' FROM [fmAudit].[tblTransactionSubLineItems] a'
				+ ' LEFT JOIN [dbo].[tblTransactionLineItems] l ON l.TransactionLineItemGuid = a.TransactionLineItemGuid'
				+ ' LEFT JOIN [fmaudit].[tblTransactionLineItems] la ON la.TransactionLineItemGuid = a.TransactionLineItemGuid AND la._AuditEventType = ''D'''
				+ ' LEFT JOIN [dbo].[tblTransactions] t ON t.TransactionGuid = a.TransactionGuid'
				+ ' LEFT JOIN [dbo].[tblTransactions] tla ON tla.TransactionGuid = la.TransactionGuid'
				+ ' LEFT JOIN [fmaudit].[tblTransactions] ta ON ta.TransactionGuid = a.TransactionGuid AND ta._AuditEventType = ''D'''
				+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID '
WHERE TableName = 'tblTransactionSubLineItems'


IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'IsEnterprise' )
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C9B14E04-8B70-4D38-9FB7-C4351D52963A', N'DWORD', N'IsEnterprise', N'1', N'8/20/2014 3:24:27 PM -04:00', 'Administrator', N'8/20/2014 3:24:27 PM -04:00', 'Administrator');
END

--IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 181))
--BEGIN
--	INSERT INTO [lookup].[tblRight]
--           (RightIndex
--           ,RightCode
--           ,RightName
--           ,RightGuid
--           ,CreatedDate
--           ,CreatedBy
--           ,UpdatedDate
--           ,UpdatedBy)
--     VALUES
--           (181
--           ,'VIEW_EXTERNAL_STATION'
--           ,'VIEW_EXTERNAL_STATION'
--           ,'93E2CBFD-320B-466D-AD76-FBEB6B73FBDC'
--           ,N'12/02/2014 1:49:09 PM -04:00'
--           ,'Administrator'
--           ,N'12/02/2014 1:49:09 PM -04:00'
--           ,'Administrator')
--END


--IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 182))
--BEGIN
--	INSERT INTO [lookup].[tblRight]
--           (RightIndex
--           ,RightCode
--           ,RightName
--           ,RightGuid
--           ,CreatedDate
--           ,CreatedBy
--           ,UpdatedDate
--           ,UpdatedBy)
--     VALUES
--           (182
--           ,'MODIFY_EXTERNAL_STATION'
--           ,'MODIFY_EXTERNAL_STATION'
--           ,'BA9D30CA-9642-4407-BCB6-0B65F4C31752'
--           ,N'12/02/2014 1:49:09 PM -04:00'
--           ,'Administrator'
--           ,N'12/02/2014 1:49:09 PM -04:00'
--           ,'Administrator')
--END



-- Changing the SyncRequestTypeGuid for Index 4 because it was the same as Index 3.  This would cause problems if a unique constraint were to be applied.
IF EXISTS (SELECT 1 FROM [lookup].[tblSyncRequestType] WHERE [SyncRequestTypeIndex] = 4 AND [SyncRequestTypeGuid] = N'80393fbc-7c51-451d-83d6-0279f2b227dd')
BEGIN
	UPDATE [lookup].[tblSyncRequestType] SET [SyncRequestTypeGuid] = N'b4128602-ecd3-414b-b0ae-8daeb6557d3c' WHERE [SyncRequestTypeIndex] = 4 AND [SyncRequestTypeGuid] = N'80393fbc-7c51-451d-83d6-0279f2b227dd'
END

GO

IF NOT EXISTS(SELECT 1 FROM [lookup].[tblTransactionOrigin] WHERE [TransactionOriginIndex] = 16)
BEGIN
	INSERT INTO [lookup].[tblTransactionOrigin] ( TransactionOriginIndex, TransactionOriginCode, TransactionOriginName, TransactionOriginGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) values(16, 'DispatchEnterprise', 'DispatchEnterprise', 'C1E77F1E-176D-4CB3-82AB-E8A4F0B3C2C4', '2015-09-03 14:31:03.1511955 -04:00', 'Adminstrator', '2015-09-03 14:31:03.1511955 -04:00', 'Adminstrator')
END

IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 2014) = 0)
BEGIN
	INSERT INTO lookup.tblMenuItemType
	(
		MenuItemTypeIndex, 
		MenuItemTypeCode, 
		MenuItemTypeName, 
		MenuItemTypeGuid, 
		CreatedDate, 
		CreatedBy, 
		UpdatedDate, 
		UpdatedBy
	)
	VALUES
	(
		2014,
		N'ADMIN_SYSTEM_WEB_LINKS_CONFIGURATION',
		N'ADMIN_SYSTEM_WEB_LINKS_CONFIGURATION',
		N'EEEEB7EB-4117-45F0-9E6A-BBB7F110C266',
		N'2/26/2015 3:08:03 PM -04:00',
		N'Administrator',
		N'2/26/2015 3:08:03 PM -04:00',
		N'Administrator'
	)
END

IF ((SELECT COUNT(*) FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 9004) = 0)
BEGIN
	INSERT INTO lookup.tblMenuItemType
	(
		MenuItemTypeIndex, 
		MenuItemTypeCode, 
		MenuItemTypeName, 
		MenuItemTypeGuid, 
		CreatedDate, 
		CreatedBy, 
		UpdatedDate, 
		UpdatedBy
	)
	VALUES
	(
		9004,
		N'REPORTS_WEB_LINKS',
		N'REPORTS_WEB_LINKS',
		N'65F74693-9E67-4B1D-ACBA-FDC7700AFAA6',
		N'2/26/2015 3:08:03 PM -04:00',
		N'Administrator',
		N'2/26/2015 3:08:03 PM -04:00',
		N'Administrator'
	)
END


IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'FMService_ReindexEnabled') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(N'1F806D54-FC85-4C03-A019-40BE9EFCB977', 'DWORD', 'FMService_ReindexEnabled', 1, SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'FMService_ReindexScheduledTime') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	(N'B1432F00-9350-4089-B3B5-0E1377E7129A', 'TIME', 'FMService_ReindexScheduledTime', '3:00 AM', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 7038))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType] (
			[MenuItemTypeIndex]
		   ,[MenuItemTypeCode]
		   ,[MenuItemTypeName]
		   ,[MenuItemTypeGuid]
		   ,[CreatedDate]
		   ,[CreatedBy]
		   ,[UpdatedDate]
		   ,[UpdatedBy]
	)
	VALUES (
			7038
		   ,N'OPERATIONS_SYNC_DASHBOARD'
		   ,N'OPERATIONS_SYNC_DASHBOARD'
		   ,N'930B310F-99A4-476F-905F-78F6C20215B3'
		   ,SYSDATETIMEOFFSET()
		   ,N'Administrator'
		   ,SYSDATETIMEOFFSET()
		   ,N'Administrator'
	)
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 2015))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType] (
			[MenuItemTypeIndex]
		   ,[MenuItemTypeCode]
		   ,[MenuItemTypeName]
		   ,[MenuItemTypeGuid]
		   ,[CreatedDate]
		   ,[CreatedBy]
		   ,[UpdatedDate]
		   ,[UpdatedBy]
	)
	VALUES (
			2015
		   ,N'ADMIN_SYSTEM_DASHBOARD'
		   ,N'ADMIN_SYSTEM_DASHBOARD'
		   ,N'61AA75D0-B66B-4903-A7B8-E4EE9FFBF18E'
		   ,SYSDATETIMEOFFSET()
		   ,N'Administrator'
		   ,SYSDATETIMEOFFSET()
		   ,N'Administrator'
	)
END



IF NOT EXISTS(SELECT 1 FROM [lookup].[tblSyncControllerStep] WHERE SyncControllerStepGuid = N'F92612A2-2025-4ED5-88E8-85628CD48E5E')
BEGIN
	INSERT INTO [lookup].[tblSyncControllerStep] ([SyncControllerStepIndex], [SyncControllerStepCode], [SyncControllerStepName], [SyncControllerStepGuid], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'PROCESS_ALL', N'ALL', N'F92612A2-2025-4ED5-88E8-85628CD48E5E',N'Synchronization process is currently processing Inserts/Updates.',  N'2/21/2017 00:00:01 AM -04:00', N'Administrator', N'2/21/2017 00:00:01 AM -04:00', N'Administrator')
END
GO

-- UPDATING CUSTOM TOOLBAR COMMANDS
DECLARE @tblCustomToolbarCommandType TABLE
(
	[CustomToolbarCommandTypeIndex] [int],
	[CustomToolbarCommandTypeCode] [nvarchar](100),
	[CustomToolbarCommandTypeName] [nvarchar](100),
	[LookupCustomToolbarTypeIndex] [int],
	[CustomToolbarCommandTypeGuid] [uniqueidentifier],
	[CreatedDate] [datetimeoffset],
	[CreatedBy] [dbo].[udtUserID],
	[UpdatedDate] [datetimeoffset],
	[UpdatedBy] [dbo].[udtUserID],
	[Default] [bit] NULL,
	[DefaultOrder] [int] NULL,
	[ImageSource] [nvarchar](100) NULL
);

; MERGE INTO [lookup].[tblCustomToolbarCommandType] AS Target
USING (VALUES 
(-1, N'UNKNOWN_TOOLBAR_COMMAND_TYPE', N'Unknown Toolbar Command Type', -1, N'72d61e43-eeea-4128-8169-b654f784a35f', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(1, N'TRANSACTION_ALIAS', N'Transaction Alias', -1, N'91c765a4-c3ee-4b08-b2aa-9350accdedda', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(101, N'ARRIVAL', N'Arrival', 1, N'5f4e66f7-8eaa-46cf-9318-a4873db52f24', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(102, N'CANCEL', N'Cancel', 1, N'ce2a7b8d-f6e3-4425-8416-15207888d33c', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1,11,'toolStripCancelButton.Image.png'),
(103, N'CHANGE_OPERATOR_STATUS', N'Change Operator Status', 1, N'6264fd4b-f1da-486f-8baf-608cd8685eab', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(104, N'CONTROL_LOG', N'Control Log', 1, N'9f8670fa-4c22-4ad2-a1d6-5dac1d5f1c62', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1,7,'toolStripControlLogButton.Image.png'),
(105, N'DISPATCHING_VIEW', N'Dispatch', 1, N'adc86c86-1571-4626-a04d-f59f05afa9b3', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1,6,'toolStripDispatchButton.Image.png'),
(106, N'DISPATCHERS_LIST', N'Dispatchers List', 1, N'3c1446f3-b193-4623-b039-03f70bce0812', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1,10, null),
(107, N'EVACUATE', N'Evacuate', 1, N'c16474a4-5f61-4278-8344-46ed805f762c', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(108, N'RELEASE_TO_ACCOUNTING', N'Release To Accounting', 1, N'dc30e273-d0bf-4f96-95b4-d4ebffb85b6c', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(109, N'FAST_LOG', N'Fast Log', 1, N'0895ea41-9919-4452-8a5f-fdd9d7b5707e', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator', 1,3,'toolStripFastLogButton.Image.png'),
(110, N'FAST_LOG_FILLSTAND', N'Fast Log Fillstand', 1, N'e5c38546-0bbf-4114-87a0-d2556584a6ad', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1,4,'toolStripFastLogFillstandButton.Image.png'),
(111, N'FILLSTAND_COMPLETION', N'Fillstand Completion', 1, N'86b78f20-df27-4cbe-895a-5519cb7d6247', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(113, N'FLIGHT_LINE_STATUS', N'Flight Line', 1, N'748583d4-a7ec-42e6-a1ab-0a46da2cf720', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator', 1,9,'toolStripFlightLineButton.Image.png'),
(115, N'HELP', N'Help', 1, N'6f93d95e-f946-4ab5-a784-d3207033c564', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(116, N'OPTIONAL_TIMES', N'Optional Times', 1, N'c04c87a4-f13a-4176-88eb-32fc09957e9d', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(117, N'QUERY_WRITER', N'Query Writer', 1, N'bea344ff-9c0a-4dc2-9e37-9382da4497d5', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(118, N'RECIRCULATION', N'Recirculation', 1, N'b0c84267-202e-4009-a055-94527dca80b2', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(119, N'RELOG', N'Relog', 1, N'0114f009-d423-431f-b4be-4ad4fc960937', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1,5,'toolStripCopyButton.Image.png'),
(120, N'REPORTS', N'Reports', 1, N'707885e6-3d3c-495d-8076-7aa6f4130f67', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(121, N'REQUEST', N'Request', 1, N'057f62e1-e250-439a-b268-5c064b00bf70', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator', 1, 1, 'toolStripRequestRefuelButton.Image.png'),
(122, N'SERVICE_COMPLETION', N'Service Completion', 1, N'46ed6a96-8bfa-4344-8ce9-37279850f9bf', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(123, N'STANDBY', N'Standby', 1, N'7dadee6e-552a-457f-a17d-4adf4c549f65', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator', 1, 8, 'toolStripStandbyButton.Image.png'),
(124, N'START_OF_SERVICE', N'Start Of Service', 1, N'9cf8685d-beea-48d9-b0a8-90b86d68ad12', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(125, N'STOP_OF_SERVICE', N'Stop Of Service', 1, N'3da1a646-4938-47bc-8c50-fdc2faa47489', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(126, N'TOTAL_AND_AVERAGE', N'Total And Average', 1, N'8a1b8e18-215c-44f4-b034-84f15d795ee1', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(128, N'TRANSIENT', N'Transient', 1, N'29ee0332-ad74-4e7f-8770-9a7f7080d602', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator', 1, 2, 'toolStripTransientButton.Image.png'),
(129, N'UNCANCEL', N'Uncancel', 1, N'8e97771f-ce92-4da9-b128-fd0e8f9550b8', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null),
(130, N'REFRESH', N'Refresh', 1, N'5e158b24-13b0-4962-bf45-1494f2e8b989', N'10/23/2012 9:45:47 AM -04:00', N'Administrator', N'5/7/2017 4:59:33 PM -04:00', N'Administrator',1, 12,null),
(131, N'COPY', N'Copy', 1, N'a527ce4e-5870-410a-9534-546c72f8ce25', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', N'9/21/2012 3:21:57 PM -04:00', N'Administrator', 0, null, null)
) AS SOURCE ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Default],[DefaultOrder] ,[ImageSource])
ON (Target.[CustomToolbarCommandTypeIndex] = Source.[CustomToolbarCommandTypeIndex])
WHEN MATCHED AND EXISTS (SELECT target.[CustomToolbarCommandTypeIndex],
						target.[CustomToolbarCommandTypeCode],
						target.[CustomToolbarCommandTypeName],
						target.[CustomToolbarCommandTypeGuid],
						target.[LookupCustomToolbarTypeIndex],
						target.[CreatedDate],
						target.[CreatedBy],
						target.[UpdatedDate],
						target.[UpdatedBy],
						target.[Default],
						target.[DefaultOrder] ,
						target.[ImageSource]
						EXCEPT 
						SELECT source.[CustomToolbarCommandTypeIndex],
						source.[CustomToolbarCommandTypeCode],
						source.[CustomToolbarCommandTypeName],
						source.[CustomToolbarCommandTypeGuid],
						source.[LookupCustomToolbarTypeIndex],
						source.[CreatedDate],
						source.[CreatedBy],
						source.[UpdatedDate],
						source.[UpdatedBy],
						source.[Default],
						source.[DefaultOrder] ,
						source.[ImageSource] ) THEN
	UPDATE SET CustomToolbarCommandTypeIndex = source.[CustomToolbarCommandTypeIndex],
						CustomToolbarCommandTypeCode = source.[CustomToolbarCommandTypeCode],
						CustomToolbarCommandTypeName = source.[CustomToolbarCommandTypeName],
						LookupCustomToolbarTypeIndex = source.[LookupCustomToolbarTypeIndex],
						CustomToolbarCommandTypeGuid = source.[CustomToolbarCommandTypeGuid],
						CreatedDate = source.[CreatedDate],
						CreatedBy = source.[CreatedBy],
						UpdatedDate = source.[UpdatedDate],
						UpdatedBy = source.[UpdatedBy],
						[Default] = source.[Default],
						DefaultOrder = source.[DefaultOrder] ,
						ImageSource = source.[ImageSource] 
WHEN NOT MATCHED THEN
	INSERT ([CustomToolbarCommandTypeIndex], [CustomToolbarCommandTypeCode], [CustomToolbarCommandTypeName], [LookupCustomToolbarTypeIndex], [CustomToolbarCommandTypeGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Default],[DefaultOrder] ,[ImageSource])
		VALUES (source.[CustomToolbarCommandTypeIndex],
						source.[CustomToolbarCommandTypeCode],
						source.[CustomToolbarCommandTypeName],
						source.[LookupCustomToolbarTypeIndex],
						source.[CustomToolbarCommandTypeGuid],
						source.[CreatedDate],
						source.[CreatedBy],
						source.[UpdatedDate],
						source.[UpdatedBy],
						source.[Default],
						source.[DefaultOrder] ,
						source.[ImageSource]);
GO


--perform closeout permission
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 176)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'908C2716-27E4-4B61-B53C-EA7695818F84', N'00000000-0000-0000-0000-000000000003', 176, N'7/20/2017 10:23:55 AM -05:00', N'Varec', N'7/20/2017 10:23:55 AM -05:00', N'Varec') 
END


--view reconilciation permission
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 24)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'503c369e-230b-499a-b4d7-f956be2ac851', N'00000000-0000-0000-0000-000000000003', 24, N'11/23/2011 11:40:00 AM +00:00', N'Varec', N'11/23/2011 11:40:00 AM +00:00', N'Varec')
END

GO


IF NOT EXISTS (SELECT 1 FROM [lookup].[tblProcessVariableType] WHERE [ProcessVariableTypeCode] = N'WATER_LEVEL_PV')
BEGIN
	DELETE FROM [lookup].[tblProcessVariableType] WHERE [ProcessVariableTypeCode] = N'MAX_PV'
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (72, N'WATER_LEVEL_PV', N'WATER LEVEL PV', N'43AA8219-26A9-47FC-BC65-55AEC1D740D9', N'9/20/2017', N'Varec', N'9/20/2017', N'Varec')
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (74, N'MAX_PV', N'MAX PV', N'9D10A1AF-A807-4285-AFAA-008A87D46A9D', N'9/20/2017', N'Varec', N'9/20/2017', N'Varec')
END

IF NOT EXISTS (SELECT 1 FROM [lookup].[tblProcessVariableType] WHERE [ProcessVariableTypeCode] = N'WATER_VOLUME_PV')
BEGIN
	INSERT INTO [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex], [ProcessVariableTypeCode], [ProcessVariableTypeName], [ProcessVariableTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (73, N'WATER_VOLUME_PV', N'WATER VOLUME PV', N'A7CAFA7F-1E33-4178-8153-951AD991C521', N'9/20/2017', N'Varec', N'9/20/2017', N'Varec')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 10001))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
           ([MenuItemTypeIndex]
           ,[MenuItemTypeCode]
           ,[MenuItemTypeName]
           ,[MenuItemTypeGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (10001
           ,N'DATA_ANALYTICS_VIEWER'
           ,N'DATA_ANALYTICS_VIEWER'
           ,N'7F85776D-83FA-4FA0-9718-017971E7CFE1'
           ,N'2015-10-23 15:16:23.7437223 -04:00'
           ,N'Administrator'
           ,N'2015-10-23 15:16:23.7437223 -04:00'
           ,N'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 11001))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
           ([MenuItemTypeIndex]
           ,[MenuItemTypeCode]
           ,[MenuItemTypeName]
           ,[MenuItemTypeGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (11001
           ,N'MAP_MAPS'
           ,N'MAP_MAPS'
           ,N'2EA0DE8A-7CF9-4F95-866B-3DC84D336EC4'
           ,N'2016-01-21 15:16:23.7437223 -04:00'
           ,N'Administrator'
           ,N'2016-01-21 15:16:23.7437223 -04:00'
           ,N'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 11002))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
           ([MenuItemTypeIndex]
           ,[MenuItemTypeCode]
           ,[MenuItemTypeName]
           ,[MenuItemTypeGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (11002
           ,N'MAP_CONFIGURATION'
           ,N'MAP_CONFIGURATION'
           ,N'99E89F7B-FBD9-4915-8020-37F88DF50EE4'
           ,N'2016-01-21 15:16:23.7437223 -04:00'
           ,N'Administrator'
           ,N'2016-01-21 15:16:23.7437223 -04:00'
           ,N'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 11003))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
           ([MenuItemTypeIndex]
           ,[MenuItemTypeCode]
           ,[MenuItemTypeName]
           ,[MenuItemTypeGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (11003
           ,N'MAP_ASSET_TRACKING_DEVICE_CONFIG'
           ,N'MAP_ASSET_TRACKING_DEVICE_CONFIG'
           ,N'754355B7-1C5B-473B-800D-6C8EED41DF94'
           ,N'2016-02-17 15:16:23.7437223 -04:00'
           ,N'Administrator'
           ,N'2016-02-17 15:16:23.7437223 -04:00'
           ,N'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 1040))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
			([MenuItemTypeIndex]
			,[MenuItemTypeCode]
			,[MenuItemTypeName]
			,[MenuItemTypeGuid]
			,[CreatedDate]
			,[CreatedBy]
			,[UpdatedDate]
			,[UpdatedBy])
		VALUES
			(1040
			,N'ACCOUNTING_STANDARD_IMPORT_TRANSACTION_DATA'
			,N'ACCOUNTING_STANDARD_IMPORT_TRANSACTION_DATA'
			,N'815E08B5-F5C6-4110-B9C5-2CE0E4B1089E'
			,N'2019-09-09 17:35:00 -04:00'
			,'Administrator'
			,N'2019-09-09 17:35:00 -04:00'
			,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMenuItemType WHERE MenuItemTypeIndex = 11004))
BEGIN
	INSERT INTO [lookup].[tblMenuItemType]
           ([MenuItemTypeIndex]
           ,[MenuItemTypeCode]
           ,[MenuItemTypeName]
           ,[MenuItemTypeGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (11004
           ,N'ICON_CONFIGURATION'
           ,N'ICON_CONFIGURATION'
           ,N'6C814D26-D67C-4326-A1B9-A21CC3419618'
           ,N'2016-06-02 15:16:23.7437223 -04:00'
           ,N'Administrator'
           ,N'2016-06-02 15:16:23.7437223 -04:00'
           ,N'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'DataAnalyticsServerURL') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('E605ACC6-5C50-4F64-A650-810A0E6CA4BC', 'SZ', 'DataAnalyticsServerURL', 'http://10.33.19.177', '2015-11-03 09:25:53.6702147 -07:00', 'Administrator', '2015-11-03 09:25:53.6702147 -07:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'GeoTrackingMapIconPath') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('4A365AE9-DB01-4C72-ACF0-89B798A05C2A', 'SZ', 'GeoTrackingMapIconPath', '~/Areas/images/AssetMapImages/MapIcons', '2015-05-26 07:11:53.6702147 -04:00', 'Administrator', '2015-05-26 07:11:53.6702147 -04:00', 'Administrator')
END

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'GeoTrackingMapRefreshTimeInSeconds') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('5B0C5891-25B9-48AF-B32A-05F88F1CBE3A', 'DWORD', 'GeoTrackingMapRefreshTimeInSeconds', NULL, '2015-06-08 07:11:53.6702147 -04:00', 'Administrator', '2015-06-08 07:11:53.6702147 -04:00', 'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblMapSource WHERE MapSourceIndex = 3))
BEGIN
	INSERT INTO [lookup].[tblMapSource]
           ([MapSourceIndex]
           ,[MapSourceCode]
           ,[MapSourceName]
           ,[MapSourceGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (3
           ,N'BINGMAP'
           ,N'Bing Map'
           ,N'7E36CA2C-24CC-46C6-82DE-69B57DFF039C'
           ,N'2016-06-16 15:16:23.7437223 -04:00'
           ,N'Administrator'
           ,N'2016-06-16 15:16:23.7437223 -04:00'
           ,N'Administrator')
END

IF (NOT EXISTS(SELECT 1 FROM lookup.tblMajorCorrectionType WHERE MajorCorrectionTypeIndex = 26 AND MajorCorrectionTypeCode = N'CORR_ASTM_D1555_F_2009'))
BEGIN
	DELETE FROM lookup.tblMajorCorrectionType where MajorCorrectionTypeIndex = 26
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (26, N'CORR_ASTM_D1555_F_2009', N'CORR ASTM D1555 F 2009', N'968189EE-23A9-475C-9D38-9BF7DFB6E530', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblMajorCorrectionType] ([MajorCorrectionTypeIndex], [MajorCorrectionTypeCode], [MajorCorrectionTypeName], [MajorCorrectionTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (27, N'MAX_MAJOR_CORRECTION_TYPE', N'MAX MAJOR CORRECTION TYPE', N'9f6887cf-4bcc-4ef3-b78c-7f6d40f21120', N'6/18/2012 1:02:58 PM +00:00', N'Administrator', N'6/18/2012 1:02:58 PM +00:00', N'Administrator')
END

IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'tblAssetTrackingDevice'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingDevice'
							, 'Asset Tracking Device'
							, ''
							, 'SELECT @ID = a.DeviceID'
							+ ' FROM [fmAudit].[tblAssetTrackingDevice] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)
END

IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'tblAssetTrackingDetail'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingDetail'
							, 'Asset Tracking Detail'
							, ''
							, 'SELECT @ID = a.AssetTrackingDeviceID'
							+ ' FROM [fmAudit].[tblAssetTrackingDetail] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)
END

IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'tblAssetTrackingIconConfiguration'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingIconConfiguration'
							, 'Asset Tracking Icon Configuration'
							, ''
							, 'SELECT @ID = a.IconConfigurationID'
							+ ' FROM [fmAudit].[tblAssetTrackingIconConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)
END

IF (NOT EXISTS (SELECT * FROM tblAuditHandler WHERE TableName = 'tblAssetTrackingMapConfiguration'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'tblAssetTrackingMapConfiguration'
							, 'Asset Tracking Map Configuration'
							, ''
							, 'SELECT @ID = a.MapName'
							+ ' FROM [fmAudit].[tblAssetTrackingMapConfiguration] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
	)
END


IF NOT EXISTS (SELECT 1 FROM [dbo].[tblAuditHandler] WHERE TableName = 'tblFCEEMapping')
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblFCEEMapping'
							, 'FCEE Mapping'
							, ''
							, 'SELECT @ID = 
CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '' 
+ CASE WHEN p.ID IS NULL THEN pa.ID ELSE p.ID END + '' - ''
+ TRIM(CASE WHEN d.ImeiNumber IS NULL THEN da.ImeiNumber ELSE d.ImeiNumber END) + '' - ''
+ m.EdgeMessageName + '' - '' 
+ ''Index:'' + TRIM(CONVERT(NVARCHAR(3), a.[Index])) 
+ TRIM(CASE WHEN a.[Device] IS NULL THEN '''' ELSE '' - Device:'' + CONVERT(NVARCHAR(3), a.[Device]) END)
FROM  [fmaudit].[tblFCEEMapping] a 
LEFT JOIN [lookup].[tblEdgeMessage] m ON a.MsgType=m.EdgeMessageIndex
LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = a.PointGuid 
LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'' 
LEFT JOIN [dbo].[tblFCEDevice] d ON d.FCEDeviceGuid = a.FCEDeviceGuid 
LEFT JOIN [fmaudit].[tblFCEDevice] da ON da.FCEDeviceGuid = a.FCEDeviceGuid AND da._AuditEventType = ''D'' 
LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = p.SiteGuid 
LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = p.SiteGuid AND sa._AuditEventType = ''D'' 
WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'
							, 'SELECT @SiteGuid = ISNULL(p.SiteGuid, pa.SiteGuid)
FROM  [fmaudit].[tblFCEEMapping] a 
LEFT JOIN [dbo].[tblPoint] p ON p.PointGuid = a.PointGuid 
LEFT JOIN [fmaudit].[tblPoint] pa ON pa.PointGuid = a.PointGuid AND pa._AuditEventType = ''D'' 
WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'
	)
END

IF NOT EXISTS (SELECT 1 FROM [dbo].[tblAuditHandler] WHERE TableName = 'tblFCEDevice')
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblFCEDevice'
							, 'FCE Device'
							, ''
							, 'SELECT @ID = TRIM(a.ImeiNumber) FROM  [fmaudit].[tblFCEDevice] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
							, 'SELECT @SiteGuid = a.SiteGuid FROM  [fmaudit].[tblFCEDevice] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)
END

UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAssetTrackingDevice] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' WHERE TableName = 'tblAssetTrackingDevice'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAssetTrackingDetail] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' WHERE TableName = 'tblAssetTrackingDetail'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAssetTrackingIconConfiguration] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' WHERE TableName = 'tblAssetTrackingIconConfiguration'
UPDATE tblAuditHandler SET SiteGuidQuery = 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblAssetTrackingMapConfiguration] a WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID' WHERE TableName = 'tblAssetTrackingMapConfiguration'




-- Add Audit Handler for new tblVRUThresholds table
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblAuditHandler] WHERE TableName = 'tblVRUThresholds')
BEGIN
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
END

-- Add Audit Handler for new map_tblProductToVruTrackingConfig table
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblAuditHandler] WHERE TableName = 'map_tblProductToVruTrackingConfig')
BEGIN
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
END

-- Add Audit Handler for new map_tblApplicationStringToFootNoteAdditiveProfile table.  Delete the old one first if it's there.
delete from dbo.tblAuditHandler where TableName = 'map_tblApplicationStringToFootNoteAdditiveProfile'
IF NOT EXISTS (SELECT 1 FROM [dbo].[tblAuditHandler] WHERE TableName = 'map_tblApplicationStringToFootNoteAdditiveProfile')
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery) VALUES (
							'map_tblApplicationStringToFootNoteAdditiveProfile'
							, 'Additive Profiles - Footnote'
							, 'Additive Profiles'
							, 'SELECT @ID = CASE WHEN a.AdditiveProfileGuid IS NULL THEN ''{All}'''
							+ ' WHEN ap.ID IS NULL THEN apa.ID ELSE ap.ID END + '' - '''
							+ ' + CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END'
							+ ' FROM  [fmaudit].[map_tblApplicationStringToFootNoteAdditiveProfile] a'
							+ ' LEFT JOIN [dbo].[tblApplicationString] s ON s.ApplicationStringGuid = a.ApplicationStringGuid'
							+ ' LEFT JOIN [fmaudit].[tblApplicationString] sa ON sa.ApplicationStringGuid = a.ApplicationStringGuid AND sa._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblAdditiveProfiles] ap ON ap.AdditiveProfileGuid = a.AdditiveProfileGuid'
							+ ' LEFT JOIN [fmaudit].[tblAdditiveProfiles] apa ON apa.AdditiveProfileGuid = a.AdditiveProfileGuid AND apa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'
	)
END

--------------------------------- Start of Point Type creation -----------------------------------------------
DECLARE @PointTemplateTypeIndex INT
DECLARE @ApplicationStringGuid UNIQUEIDENTIFIER
DECLARE @PointSiteGuid UNIQUEIDENTIFIER
SET @PointSiteGuid = '00000000-0000-0000-0000-000000000001'
SELECT @PointTemplateTypeIndex = ApplicationStringTypeIndex FROM lookup.tblApplicationStringType WHERE ApplicationStringTypeCode = 'POINT_TEMPLATE_TYPE'

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = 'Tank') = 0)
BEGIN
	SET @ApplicationStringGuid = 'E78CD406-4C19-4978-8940-FA4E404E3E53'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Tank', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('B0E1B642-9C3C-4587-961B-F5505BD1AA65', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = 'Valve') = 0)
BEGIN
	SET @ApplicationStringGuid = 'E33A769F-3EFC-46C6-A50F-A103454BFE97'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Valve', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('5233FF4B-354E-4B59-9658-C627546B231D', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = 'Pump') = 0)
BEGIN
	SET @ApplicationStringGuid = '1135AA41-525B-4024-BF3D-6BF2D55A034B'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Pump', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('B6C238F9-F4BF-4B49-86EE-24BBE22E7722', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = 'Meter') = 0)
BEGIN
	SET @ApplicationStringGuid = '9403A36F-33F6-4DCC-857D-F53C8DC66196'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Meter', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('B5BC41D2-E182-47AB-BE82-09E45B98DC4E', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = 'Preset') = 0)
BEGIN
	SET @ApplicationStringGuid = '7EA082F3-6FBF-4136-A2D7-8A3670E9A9EF'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Preset', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('0DCC185F-29C0-43E7-A1A3-4B596D908370', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ID = 'Pipe') = 0)
BEGIN
	SET @ApplicationStringGuid = '55F0E8B8-3A74-40D0-8B8C-675A4B6A478C'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Pipe', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('E9A87EF0-AF03-46B9-A33C-2815A22426DE', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

-- Check for System point template type of System.
IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ApplicationStringGuid = '2DDEB3E0-545C-444B-B1BF-9CAB048F21B7') = 0)
BEGIN
	SET @ApplicationStringGuid = '2DDEB3E0-545C-444B-B1BF-9CAB048F21B7'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('System', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('478052D9-1C6B-41E0-A5D2-13A4CD6F56FC', @ApplicationStringGuid, @PointSiteGuid, '2012-08-18 09:06:12.0000000 -04:00', 'administrator', '2012-08-18 09:06:12.0000000 -04:00', 'administrator', @PointSiteGuid)
END

-- Check for System point template type of Movement.
IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ApplicationStringGuid = 'A89562CE-FB16-47D3-9BD6-C33AD3BD2141') = 0)
BEGIN
	SET @ApplicationStringGuid = 'A89562CE-FB16-47D3-9BD6-C33AD3BD2141'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Movement', '2022-06-07 07:00:0.0000000 -04:00', 'administrator', '2022-06-07 07:00:0.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('C1853946-BCDB-4697-BBE9-64C67DDEDAEC', @ApplicationStringGuid, @PointSiteGuid, '2022-06-07 07:00:0.0000000 -04:00', 'administrator', '2022-06-07 07:00:0.0000000 -04:00', 'administrator', @PointSiteGuid)
END

-- Check for System point template type of Movement Control.
IF ((SELECT COUNT(ID) FROM tblApplicationString WHERE ApplicationStringGuid = 'E8CA745C-2C38-4B52-B15C-EF738AD41305') = 0)
BEGIN
	SET @ApplicationStringGuid = 'E8CA745C-2C38-4B52-B15C-EF738AD41305'

	INSERT INTO tblApplicationString (ID, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, ApplicationStringGuid, SiteGuid, LookupApplicationStringTypeIndex)
	VALUES ('Movement Control', '2022-06-29 07:00:0.0000000 -04:00', 'administrator', '2022-06-29 07:00:0.0000000 -04:00', 'administrator', @ApplicationStringGuid, @PointSiteGuid, @PointTemplateTypeIndex)

	INSERT INTO map.tblEntityPointTemplateTypeToSite (PointTemplateTypeToSiteGuid, ApplicationStringGuid, SiteGuid, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy, AssignedFromSiteGuid)
	VALUES('2D08C15D-D1AF-4B97-A1D6-30EAA923FFDF', @ApplicationStringGuid, @PointSiteGuid, '2022-06-29 07:00:0.0000000 -04:00', 'administrator', '2022-06-29 07:00:0.0000000 -04:00', 'administrator', @PointSiteGuid)
END
ELSE
BEGIN
	UPDATE tblApplicationString SET ID = 'Movement Control' WHERE ApplicationStringGuid = 'E8CA745C-2C38-4B52-B15C-EF738AD41305'
END


--------------------------------- End of Point Type creation -----------------------------------------------

----------------------------------------- NORMAL UNACKNOWLEDGED ALARM PRIORITY ---------------------------------------
--

----------------------------------------------------------------------------------------------------------------------

Declare @NormalAlarmPriorityID nvarchar(max)
Declare @NormalAlarmPriorityGuid uniqueidentifier
Declare @NormalAlarmPointSiteGuid uniqueidentifier
Declare @NormalAlarmPriorityToSiteGuid uniqueidentifier

Set @NormalAlarmPriorityID =    'Normal Unacknowledged';
set @NormalAlarmPriorityGuid =  '5b7d7344-7d3c-4cde-a834-b5e2c8bfe11f';
Set @NormalAlarmPointSiteGuid = '00000000-0000-0000-0000-000000000001';
Set @NormalAlarmPriorityToSiteGuid = '203C6ECC-9B17-448A-8086-1A4CB6E3904D';

IF ((SELECT COUNT(ID) FROM tblAlarmPriorities WHERE ID = @NormalAlarmPriorityID) = 0)
BEGIN
	INSERT INTO [dbo].[tblAlarmPriorities] ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
   VALUES (@NormalAlarmPriorityID,'00FF00','000000','000000','00FF00','Silence.mp3', SYSDATETIMEOFFSET(), 'administrator', SYSDATETIMEOFFSET(), 'administrator', @NormalAlarmPriorityGuid, @NormalAlarmPointSiteGuid, null)

	INSERT INTO [map].[tblEntityAlarmPriorityToSite] ([AlarmPriorityToSiteGuid],[AlarmPriorityGuid],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
   VALUES(@NormalAlarmPriorityToSiteGuid, @NormalAlarmPriorityGuid, @NormalAlarmPointSiteGuid, SYSDATETIMEOFFSET(), 'administrator', SYSDATETIMEOFFSET(), 'administrator', @NormalAlarmPointSiteGuid)
END
ELSE
BEGIN 
	UPDATE [dbo].[tblAlarmPriorities] 
	SET [SoundFile] = ISNULL([SoundFile],'Silence.mp3'),
	[AlarmPriorityGuid] = ISNULL([AlarmPriorityGuid],@NormalAlarmPriorityGuid),
	[Priority] = NULL
	WHERE ID = @NormalAlarmPriorityID
END



----------------------------------Audit Handlers------------------------------------------------------------

:r .\Script.AuditHandler.sql

------------------------------------------------- DEFAULT SYNCHRONIZATION EVENT LOG CONFIGURATION ----------------------------------------------
--
-- Note : Any additions here should also be made to AlarmEventAssignmentPage.ascs.cs AssignmentDataGridUpdateCommand
--


IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Manual Synchronization Complete' AND AlarmAndEventGuid = '9A7D7144-07FE-4FED-BDD0-7EC43ABFBBA0')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Manual Synchronization Complete',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'9A7D7144-07FE-4FED-BDD0-7EC43ABFBBA0','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Manual Synchronization Initiated' AND AlarmAndEventGuid = 'CEE75B37-0E18-4A3E-8F14-133FD658D607')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Manual Synchronization Initiated',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'CEE75B37-0E18-4A3E-8F14-133FD658D607','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Periodic Synchronization Complete' AND AlarmAndEventGuid = '356DB540-EE86-4395-A395-590925166FA4')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Periodic Synchronization Complete',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'356DB540-EE86-4395-A395-590925166FA4','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Periodic Synchronization Initiated' AND AlarmAndEventGuid = 'B744E7E3-67E8-4415-AF1F-754C1B79A0AE')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Periodic Synchronization Initiated',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'B744E7E3-67E8-4415-AF1F-754C1B79A0AE','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Stop Synchronization Complete' AND AlarmAndEventGuid = 'C6BF383D-1C87-4D93-A67B-46831425AFF4')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Stop Synchronization Complete',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'C6BF383D-1C87-4D93-A67B-46831425AFF4','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Stop Synchronization Initiated' AND AlarmAndEventGuid = '795842A6-A6B1-47B3-A679-7C4A977CB1DC')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Stop Synchronization Initiated',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'795842A6-A6B1-47B3-A679-7C4A977CB1DC','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Synchronization Configuration Error' AND AlarmAndEventGuid = 'A2B69367-C57F-4A3F-9556-6CFB56219954')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Synchronization Configuration Error',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'A2B69367-C57F-4A3F-9556-6CFB56219954','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Synchronization Conflict(s) Detected' AND AlarmAndEventGuid = '5A1FD84B-21E0-4C62-8DF1-F101BE4AC863')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Synchronization Conflict(s) Detected',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'5A1FD84B-21E0-4C62-8DF1-F101BE4AC863','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Synchronization Currently Disabled' AND AlarmAndEventGuid = '6F6C6B8F-A5C8-4B67-8CBE-BAD60EAFC89B')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Synchronization Currently Disabled',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'6F6C6B8F-A5C8-4B67-8CBE-BAD60EAFC89B','00000000-0000-0000-0000-000000000001',null,null)
END

IF NOT EXISTS (SELECT * FROM tblAlarmAndEvents WHERE ID = 'Synchronization Error Encountered' AND AlarmAndEventGuid = '07B31A82-B095-4D1F-B4F2-9CB2F65924AC')
BEGIN
	INSERT INTO tblAlarmAndEvents (Source,Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,Enabled,AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid) values ('Data Synchronization', 0, 'Synchronization Error Encountered',null,null,'2016-05-12','administrator','2016-05-12','administrator',1,'07B31A82-B095-4D1F-B4F2-9CB2F65924AC','00000000-0000-0000-0000-000000000001',null,null)
END
-------------------------------------------------------------------------------------------------------------
------------------------------------ License Alarm and Event configuration ----------------------------------
-------------------------------------------------------------------------------------------------------------
PRINT 'Starting configuring email notifications for license and alarm status not normal'

DECLARE @SiteAdminGuid UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'

DECLARE @LicenseApplicationStringGuid UNIQUEIDENTIFIER = '31C95640-697D-4B71-94E2-5923A8B5506E'
DECLARE @LicenseApplicationStringUpdatedDate DateTimeOffset = '2024-03-27 12:00:00 +4:00'
DECLARE @tmpLicense TABLE([ApplicationStringToEmailAddressGuid] UNIQUEIDENTIFIER,[ApplicationStringGuid] UNIQUEIDENTIFIER,[Sequence] int)
DECLARE @LicensePrioritySiteAdminMappingGuid UNIQUEIDENTIFIER = '1ba66fbf-9709-4bf2-89fb-5cf46b7d1922'
DECLARE @LicensePriorityMappingNamespaceGuid nvarchar(max) = '{e9141f1c-373f-4518-8bf8-b68eaaf62fe6}'
DECLARE @LicenseAlarmPriorityToEmailGroupMapping UNIQUEIDENTIFIER = 'cd2a2ce5-6338-48c5-9141-f5adae35ce73'

DECLARE @IsEnterprise BIT;
SET @IsEnterprise = (CAST((SELECT SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'IsEnterprise') AS BIT)) 

IF NOT EXISTS (SELECT * FROM [dbo].[tblApplicationString] WHERE [ID]='License' AND [ApplicationStringGuid] = @LicenseApplicationStringGuid AND [SiteGuid]=@SiteAdminGuid AND UpdatedDate >= @LicenseApplicationStringUpdatedDate)
BEGIN
	PRINT 'Begin INSERTING/UPDATING [tblApplicationString] to add/modify alarm & event log source License'

	INSERT INTO @tmpLicense([ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[Sequence])
	SELECT [ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[Sequence] 
	FROM map.tblApplicationStringToEmailAddress m 
	WHERE m.[ApplicationStringGuid] = @LicenseApplicationStringGuid

	DELETE FROM map.tblApplicationStringToEmailAddress WHERE [ApplicationStringGuid] IN (SELECT  [ApplicationStringGuid] FROM  [dbo].[tblApplicationString]
							WHERE [ID]='License' AND [SiteGuid]=@SiteAdminGuid AND [ApplicationStringGuid] <> @LicenseApplicationStringGuid)
	DELETE FROM  [dbo].[tblApplicationString]
							WHERE [ID]='License' AND [SiteGuid]=@SiteAdminGuid AND [ApplicationStringGuid] <> @LicenseApplicationStringGuid

	MERGE INTO [dbo].[tblApplicationString] AS tgt
		USING (
			VALUES (
				'License', @LicenseApplicationStringUpdatedDate, 'administrator', @LicenseApplicationStringUpdatedDate, 'administrator', NULL, NULL, @LicenseApplicationStringGuid, @SiteAdminGuid, 6
				)
			) AS src (ID, [CreatedDate],[CreatedBy],[UpdatedDate] ,[UpdatedBy],[StartDate],[EndDate] ,[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex])
			ON tgt.[ApplicationStringGuid] = src.[ApplicationStringGuid]
		WHEN MATCHED
			THEN
				UPDATE
				SET 
				ID = src.ID
				,[CreatedDate] = src.[CreatedDate]
				,[CreatedBy] = src.[CreatedBy]
				,[UpdatedDate]  = src.[UpdatedDate]
				,[UpdatedBy] = src.[UpdatedBy]
				,[StartDate] = src.[StartDate]
				,[EndDate]  = src.[EndDate]
				,[SiteGuid] = src.[SiteGuid]
				,[LookupApplicationStringTypeIndex] = src.[LookupApplicationStringTypeIndex]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (ID, [CreatedDate],[CreatedBy],[UpdatedDate] ,[UpdatedBy],[StartDate],[EndDate] ,[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex] )
				VALUES (ID, [CreatedDate],[CreatedBy],[UpdatedDate] ,[UpdatedBy],[StartDate],[EndDate] ,[ApplicationStringGuid],[SiteGuid],[LookupApplicationStringTypeIndex] );



	PRINT 'Completed INSERTING/UPDATING [tblApplicationString] to add/modify alarm & event log source License'
END

DECLARE @AlarmPriorityNotifyGuid UNIQUEIDENTIFIER = 'A3094EE3-314D-4834-B498-71D6B8075283'
DECLARE @AlarmPriorityNotifyUpdatedDate DateTimeOffset = '2024-03-27 12:00:00 +4:00'

-- remove records referencing wrong @AlarmPriorityNotifyGuid
DELETE FROM map.tblEntityAlarmPriorityToSite WHERE [AlarmPriorityGuid] = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmPriorityToSiteGuid<>@LicensePrioritySiteAdminMappingGuid
DELETE FROM map.tblEntityAlarmPriorityToSite WHERE [AlarmPriorityGuid] IN (SELECT [AlarmPriorityGuid] FROM [dbo].[tblAlarmPriorities] WHERE ID='Notify' AND SiteGuid=@SiteAdminGuid AND [AlarmPriorityGuid]<>@AlarmPriorityNotifyGuid)
DELETE FROM map.tblAlarmPriorityToEmailGroup WHERE [AlarmPriorityGuid] IN (SELECT [AlarmPriorityGuid] FROM [dbo].[tblAlarmPriorities] WHERE ID='Notify' AND SiteGuid=@SiteAdminGuid AND [AlarmPriorityGuid]<>@AlarmPriorityNotifyGuid)
DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents	WHERE [PriorityGuid] IN (SELECT [AlarmPriorityGuid] FROM [dbo].[tblAlarmPriorities] WHERE ID='Notify' AND SiteGuid=@SiteAdminGuid AND [AlarmPriorityGuid]<>@AlarmPriorityNotifyGuid))
DELETE FROM dbo.tblAlarmAndEvents			 WHERE [PriorityGuid] IN (SELECT [AlarmPriorityGuid] FROM [dbo].[tblAlarmPriorities] WHERE ID='Notify' AND SiteGuid=@SiteAdminGuid AND [AlarmPriorityGuid]<>@AlarmPriorityNotifyGuid)
DELETE FROM [dbo].[tblAlarmPriorities] WHERE ID='Notify' AND SiteGuid=@SiteAdminGuid AND [AlarmPriorityGuid]<>@AlarmPriorityNotifyGuid


IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblAlarmPriorities] WHERE [ID]='Notify' AND [AlarmPriorityGuid]=@AlarmPriorityNotifyGuid AND [SiteGuid]=@SiteAdminGuid AND UpdatedDate >= @AlarmPriorityNotifyUpdatedDate)
BEGIN
	PRINT 'Begin INSERTING/UPDATING [tblAlarmPriorities] to add/modify License Notify priority'

	DELETE FROM map.tblEntityAlarmPriorityToSite WHERE [AlarmPriorityGuid]=@AlarmPriorityNotifyGuid
	DELETE FROM map.tblAlarmPriorityToEmailGroup WHERE [AlarmPriorityGuid] = @AlarmPriorityNotifyGuid
	UPDATE dbo.tblAlarmAndEvents SET [PriorityGuid] = NULL WHERE [PriorityGuid] = @AlarmPriorityNotifyGuid

	MERGE INTO [dbo].[tblAlarmPriorities] AS tgt
		USING (
			VALUES (
				'Notify','000000','FF00FF','FF00FF','696969','',	@AlarmPriorityNotifyUpdatedDate,'administrator',@AlarmPriorityNotifyUpdatedDate,'administrator',@AlarmPriorityNotifyGuid	,@SiteAdminGuid,1
				)
			) AS src ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority])
			ON tgt.AlarmPriorityGuid = src.AlarmPriorityGuid
		WHEN MATCHED
			THEN
				UPDATE
				SET 
				[ID] = src.[ID]
				,[BackgroundSteady] = src.[BackgroundSteady]
				,[BackgroundAlternate] = src.[BackgroundAlternate]
				,[TextSteady] = src.[TextSteady]
				,[TextAlternate] = src.[TextAlternate]
				,[SoundFile] = src.[SoundFile]
				,[CreatedDate] = src.[CreatedDate]
				,[CreatedBy] = src.[CreatedBy]
				,[UpdatedDate] = src.[UpdatedDate]
				,[UpdatedBy] = src.[UpdatedBy]
				,[SiteGuid] = src.[SiteGuid]
				,[Priority] = src.[Priority]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority] )
				VALUES ([ID],[BackgroundSteady],[BackgroundAlternate],[TextSteady],[TextAlternate],[SoundFile],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AlarmPriorityGuid],[SiteGuid],[Priority] );



	PRINT 'Completed INSERTING/UPDATING [tblAlarmPriorities] to add/modify License Notify priority'

END

IF NOT EXISTS(SELECT TOP 1 1 FROM map.tblEntityAlarmPriorityToSite WHERE AlarmPriorityToSiteGuid=@LicensePrioritySiteAdminMappingGuid)
BEGIN
	INSERT INTO map.tblEntityAlarmPriorityToSite (AlarmPriorityToSiteGuid,	AlarmPriorityGuid,	SiteGuid,	CreatedDate,	CreatedBy,	UpdatedDate,	UpdatedBy,	AssignedFromSiteGuid)
	SELECT @LicensePrioritySiteAdminMappingGuid, a.AlarmPriorityGuid,	@SiteAdminGuid,	a.createddate, a.createdby, a.updateddate, a.updatedby,@SiteAdminGuid FROM [tblAlarmPriorities] a
		WHERE a.[AlarmPriorityGuid]=@AlarmPriorityNotifyGuid 
END

INSERT INTO [map].[tblEntityAlarmPriorityToSite]([AlarmPriorityToSiteGuid],[AlarmPriorityGuid],[SiteGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
SELECT NEWID(),a.[AlarmPriorityGuid],s.SiteGuid,a.CreatedDate,'Administrator',a.UpdatedDate,'Administrator',@SiteAdminGuid
FROM tblSites s, [dbo].[tblAlarmPriorities] a
WHERE a.[AlarmPriorityGuid]=@AlarmPriorityNotifyGuid 
		AND s.Enterprise = @IsEnterprise
		AND s.SiteGuid <> @SiteAdminGuid -- we've already added the SiteAdmin self-mapping above
		AND NOT EXISTS (SELECT TOP 1 1 FROM map.tblEntityAlarmPriorityToSite m WHERE m.AlarmPriorityGuid = a.[AlarmPriorityGuid] AND m.SiteGuid = s.SiteGuid)

DECLARE @LicenseEmailGroupGuid UNIQUEIDENTIFIER = 'A1D606A5-BF39-436D-9FC5-A9E7F62C5D0B'
DECLARE @LicenseEmailGroupToSiteAdminGuid uniqueidentifier = '5aa81242-7037-48e9-b354-935637b9d33d'

DECLARE @LicenseEmailGroupId NVARCHAR(100) = 'License Expiration Notification'
DECLARE @LicenseCreatedDate DATETIMEOFFSET = '2024-03-27 12:20:00 +4:00'

IF NOT EXISTS (SELECT * FROM [dbo].[tblEmailGroups] WHERE ID=@LicenseEmailGroupId AND [EmailGroupGuid] = @LicenseEmailGroupGuid AND SiteGuid=@SiteAdminGuid AND UpdatedDate >= @LicenseCreatedDate)
BEGIN
	PRINT 'Begin INSERTING/UPDATING e-mail group to add/modify License Expiration Notification'

	INSERT INTO @tmpLicense([ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[Sequence])
	SELECT [ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[Sequence] 
	FROM map.tblApplicationStringToEmailAddress m 
	WHERE m.[EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@LicenseEmailGroupId)

	DELETE FROM map.tblEntityEmailGroupToSite			WHERE [EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@LicenseEmailGroupId)
	DELETE FROM map.tblApplicationStringToEmailAddress	WHERE [EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@LicenseEmailGroupId)
	DELETE FROM map.tblAlarmPriorityToEmailGroup		WHERE [EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@LicenseEmailGroupId)
	DELETE FROM [dbo].[tblEmailGroups] WHERE ID=@LicenseEmailGroupId AND SiteGuid=@SiteAdminGuid  AND [EmailGroupGuid] <> @LicenseEmailGroupGuid 

	MERGE INTO [dbo].[tblEmailGroups] AS tgt
		USING (
			VALUES (
				@LicenseEmailGroupGuid
				, @LicenseEmailGroupId
				, 1
				, '1900-01-01 00:00:00.0000000 +00:00'
				,'1900-01-01 23:59:00.0000000 +00:00'
				, 1
				, @LicenseCreatedDate
				, 'administrator'
				, @LicenseCreatedDate
				, 'administrator'
				,  @SiteAdminGuid
				)
			) AS src ([EmailGroupGuid], [ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid])
			ON tgt.[EmailGroupGuid] = src.[EmailGroupGuid]
		WHEN MATCHED
			THEN
				UPDATE
				SET 
				[ID] = src.[ID]
				,[AlwaysEnabled] = src.[AlwaysEnabled]
				,[StartTime] = src.[StartTime]
				,[EndTime] = src.[EndTime]
				,[CategoriesAndPriorities] = src.[CategoriesAndPriorities]
				,[CreatedDate] = src.[CreatedDate]
				,[CreatedBy] = src.[CreatedBy]
				,[UpdatedDate] = src.[UpdatedDate]
				,[UpdatedBy] = src.[UpdatedBy]
				,[SiteGuid] = src.[SiteGuid]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT ([EmailGroupGuid], [ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid] )
				VALUES ([EmailGroupGuid], [ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid] );

END

INSERT INTO map.tblApplicationStringToEmailAddress ([ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[EmailGroupGuid],[Sequence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
SELECT [ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],@LicenseEmailGroupGuid,[Sequence],@LicenseCreatedDate, 'administrator',@LicenseCreatedDate, 'administrator'
FROM @tmpLicense

INSERT INTO map.tblAlarmPriorityToEmailGroup ([AlarmPriorityEmailGroupGuid],[AlarmPriorityGuid],[EmailGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
SELECT @LicenseAlarmPriorityToEmailGroupMapping, a.alarmpriorityguid, e.emailgroupguid, a.createddate, a.createdby, a.updateddate, a.updatedby FROM [tblAlarmPriorities] a, [tblEmailGroups] e  
WHERE a.alarmpriorityguid=@AlarmPriorityNotifyGuid and e.emailgroupguid=@LicenseEmailGroupGuid AND
	NOT EXISTS(SELECT TOP 1 1 FROM map.tblAlarmPriorityToEmailGroup x WHERE x.alarmpriorityguid=a.alarmpriorityguid AND x.emailgroupguid=e.emailgroupguid ) 

IF NOT EXISTS (SELECT * FROM map.tblEntityEmailGroupToSite WHERE [EmailGroupToSiteGuid] = @LicenseEmailGroupToSiteAdminGuid)
BEGIN
	DELETE FROM m FROM map.tblEntityEmailGroupToSite m JOIN dbo.tblEmailGroups e ON m.emailgroupguid =e.emailgroupguid
			WHERE  e.[EmailGroupGuid]=@LicenseEmailGroupGuid AND e.SiteGuid=m.SiteGuid	
	-- Fixed guid for SiteAdmin mapping
	INSERT INTO map.tblEntityEmailGroupToSite ([EmailGroupToSiteGuid],[EmailGroupGuid],[SiteGuid] ,[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
	SELECT @LicenseEmailGroupToSiteAdminGuid,e.[EmailGroupGuid],e.siteGuid ,e.[CreatedDate],e.[CreatedBy],e.[UpdatedDate],e.[UpdatedBy],e.[SiteGuid] 
	FROM dbo.tblEmailGroups e WHERE  e.[EmailGroupGuid]=@LicenseEmailGroupGuid	
END

INSERT INTO map.tblEntityEmailGroupToSite ([EmailGroupToSiteGuid],[EmailGroupGuid],[SiteGuid] ,[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
SELECT NEWID(),e.[EmailGroupGuid],s.SiteGuid,@LicenseCreatedDate,'Administrator',@LicenseCreatedDate,'Administrator',@SiteAdminGuid
FROM tblSites s, dbo.tblEmailGroups e
WHERE e.[EmailGroupGuid]=@LicenseEmailGroupGuid	 AND
		s.Enterprise = @IsEnterprise AND 
		s.SiteGuid <> @SiteAdminGuid AND -- we've already added the SiteAdmin self-mapping above
		NOT EXISTS (SELECT TOP 1 1 FROM map.tblEntityEmailGroupToSite m WHERE m.SiteGuid = s.SiteGuid AND m.EmailGroupGuid = e.[EmailGroupGuid])

DECLARE @AlarmAndEventGuid UNIQUEIDENTIFIER = '920D9EC7-561E-4E1C-852B-D4E7A4A1F7C6' 
DECLARE @AlarmAndEventID NVARCHAR(1024) = 'Your FuelsManager license will expire in 1 day'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );


END


SET @AlarmAndEventGuid  = '52FE507C-E1B1-45C6-911A-39CF8CEEAC1E' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 2 days'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );


END

SET @AlarmAndEventGuid  = 'A1DAFA91-F96A-4FF5-B35F-D2BB9371D8E4' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 3 days'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END

SET @AlarmAndEventGuid  = '58C8A8DF-F73A-4171-9D8B-53097B20CD62' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 4 days'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );


END


SET @AlarmAndEventGuid  = '5611F211-82B0-4F07-8A41-A4357468C24A' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 5 days'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END

SET @AlarmAndEventGuid  = '83B37B12-0E1F-472C-B026-816F65549F7F' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 6 days'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );


END


SET @AlarmAndEventGuid  = 'DE4106AF-725F-41D5-8AA8-73C955AFDB8A' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 7 days'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END


SET @AlarmAndEventGuid  = '4890E6CC-ED1B-435D-BEDB-A56775B49518' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 30 days or less'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );


END


SET @AlarmAndEventGuid  = '6D53BCB5-2BF4-4A6E-9F88-78A5CFB5659E' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 60 days or less'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END



SET @AlarmAndEventGuid  = '50081A2D-0D56-48FD-BFC7-CCD97EDFAD20' 
SET @AlarmAndEventID  = 'Your FuelsManager license will expire in 90 days or less'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND AlarmAndEventGuid = @AlarmAndEventGuid AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License',	1, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END

DELETE FROM dbo.tblAlarmAndEvents WHERE ID LIKE '30 day license warning ack%' AND  (SiteGuid <> @SiteAdminGuid OR AlarmAndEventGuid <> '94854676-6C93-4CD4-99C8-6CEE24FFCA61')
DELETE FROM dbo.tblAlarmAndEvents WHERE ID LIKE '60 day license warning ack%' AND  (SiteGuid <> @SiteAdminGuid OR AlarmAndEventGuid <> 'E420C994-EEC3-4E03-963A-F56074D21FC5')
DELETE FROM dbo.tblAlarmAndEvents WHERE ID LIKE '90 day license warning ack%' AND  (SiteGuid <> @SiteAdminGuid OR AlarmAndEventGuid <> 'C34C1BC0-7B85-4283-9A3D-A72A85D88311')

SET @AlarmAndEventGuid  = '94854676-6C93-4CD4-99C8-6CEE24FFCA61'
SET @AlarmAndEventID  = '30 day license warning acknowledged'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND SiteGuid = @SiteAdminGuid AND AlarmAndEventGuid = @AlarmAndEventGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License', 0, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, NULL)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END



SET @AlarmAndEventGuid  = 'E420C994-EEC3-4E03-963A-F56074D21FC5' 
SET @AlarmAndEventID  = '60 day license warning acknowledged'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND SiteGuid = @SiteAdminGuid AND AlarmAndEventGuid = @AlarmAndEventGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License', 0, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, NULL)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );
END


SET @AlarmAndEventGuid  = 'C34C1BC0-7B85-4283-9A3D-A72A85D88311' 
SET @AlarmAndEventID  = '90 day license warning acknowledged'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND SiteGuid = @SiteAdminGuid AND AlarmAndEventGuid = @AlarmAndEventGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES ('License', 0, @AlarmAndEventID, NULL, NULL, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator', 1, @AlarmAndEventGuid, @SiteAdminGuid, NULL, NULL)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

END

IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightName = 'ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING')
BEGIN
	INSERT INTO lookup.tblRight (RightIndex, RightCode, RightName)
	VALUES (369, 'ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING', 'ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING')
END

IF NOT EXISTS(SELECT TOP 1 1 FROM map.tblGroupToRight WHERE GroupToRightGuid ='5E18F48F-BD99-4E4C-9C83-04483D960446') 
BEGIN
INSERT INTO map.tblGroupToRight (GroupToRightGuid,  GroupGuid, LookupRightIndex, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy) 
	SELECT '5E18F48F-BD99-4E4C-9C83-04483D960446', GroupGuid, RightIndex, SysDateTimeOffset(), 'administrator', SysDateTimeOffset(), 'administrator' FROM dbo.tblGroups g, lookup.tblRight r
	WHERE GroupID='administrator' AND r.RightName='ALLOW_ACK_FM_LICENSE_EXPIRATION_WARNING'
END

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = '25BE49B0-041C-497F-BBF6-69E4AA7A7552' AND [SettingKey]='FMLicensePreExpiryEmail' AND [UpdatedDate] >= '2024-04-23') 
BEGIN
  DELETE FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = '25BE49B0-041C-497F-BBF6-69E4AA7A7552' AND [SettingKey] <> 'FMLicensePreExpiryEmail'
  MERGE INTO [dbo].[tblConfigurationSetting] AS tgt
		USING (
			VALUES            
          ('25BE49B0-041C-497F-BBF6-69E4AA7A7552'
           ,'MULTI_SZ'
           ,'FMLicensePreExpiryEmail'
           ,N'Hello!

We hope you have been enjoying your experience with Varec''s FuelsManager solution. We wanted to send you a friendly reminder that your software license subscription is due to expire in {0} day{1} on {2}.

To ensure uninterrupted access to all the features and benefits you have come to rely on, please renew your subscription before the expiration date to avoid any loss of access.

To renew your subscription, simply contact Varec Sales:
   • E-mail:  Sales@varec.com
   • Web:  https://www.varec.com/contact/sales-support/
   • Phone:  +1 770-447-9202 (US) or +1 866-698-2732 (Internationally)
   
If you have any questions or need assistance, please don''t hesitate to contact our support team:
   • E-mail:  Support@varec.com
   • Web:  https://www.varec.com/contact/technical-support/
   • Phone:  +1 770-446-0818 (US) or +1 800-999-6708 (Internationally)

Thank you for being a valued customer of FuelsManager,
Varec, Inc., a wholly owned subsidiary of Leidos'
           ,'2024-04-23',
		   'administrator', 
		   '2024-04-23', 
		   'administrator')
			) AS src(
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
			ON tgt.[SettingKey] = src.[SettingKey]
		WHEN MATCHED
			THEN
				UPDATE
				SET [KeyType]=src.[KeyType]
			   ,[ConfigurationSettingGuid]=src.[ConfigurationSettingGuid]
			   ,[SettingValue]=src.[SettingValue]
			   ,[CreatedDate]=src.[CreatedDate]
			   ,[CreatedBy]=src.[CreatedBy]
			   ,[UpdatedDate]=src.[UpdatedDate]
			   ,[UpdatedBy]=src.[UpdatedBy]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
				VALUES (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   );

END

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = 'C2D3865E-B30F-4A45-B376-3A42BE5B57CE' AND [SettingKey]='FMLicensePostExpiryEmail' AND [UpdatedDate] >= '2024-04-23') 
BEGIN
  DELETE FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = 'C2D3865E-B30F-4A45-B376-3A42BE5B57CE' AND [SettingKey] <> 'FMLicensePostExpiryEmail'
  MERGE INTO [dbo].[tblConfigurationSetting] AS tgt
		USING (
			VALUES            
			('C2D3865E-B30F-4A45-B376-3A42BE5B57CE'
           ,'MULTI_SZ'
           ,'FMLicensePostExpiryEmail'
           ,N'Hello!

We hope you have been enjoying your experience with Varec''s FuelsManager solution. We wanted to send you a friendly reminder that your software license subscription has expired on {0}.

To regain access to all the features and benefits you have come to rely on, please renew your subscription now.

To renew your subscription, simply contact Varec Sales:
   • E-mail:  Sales@varec.com
   • Web:  https://www.varec.com/contact/sales-support/
   • Phone:  +1 770-447-9202 (US) or +1 866-698-2732 (Internationally)
   
If you have any questions or need assistance, please don''t hesitate to contact our support team:
   • E-mail:  Support@varec.com
   • Web:  https://www.varec.com/contact/technical-support/
   • Phone:  +1 770-446-0818 (US) or +1 800-999-6708 (Internationally)

Thank you for being a valued customer of FuelsManager,
Varec, Inc., a wholly owned subsidiary of Leidos'
           ,'2024-04-23',
		   'administrator', 
		   '2024-04-23', 
		   'administrator')
			) AS src(
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
			ON tgt.[SettingKey] = src.[SettingKey]
		WHEN MATCHED
			THEN
				UPDATE
				SET [KeyType]=src.[KeyType]
			   ,[ConfigurationSettingGuid]=src.[ConfigurationSettingGuid]
			   ,[SettingValue]=src.[SettingValue]
			   ,[CreatedDate]=src.[CreatedDate]
			   ,[CreatedBy]=src.[CreatedBy]
			   ,[UpdatedDate]=src.[UpdatedDate]
			   ,[UpdatedBy]=src.[UpdatedBy]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
				VALUES (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   );
END

--
-- Configure alarm & event email group for alarm status not normal
--
DECLARE @AlarmStatusNotNormalUpdateDate DateTimeOffset = '2024-03-27 18:00:00 +4:00'

SET @AlarmAndEventID = 'Point Alarm Status Not Normal Notification'
SET @AlarmAndEventGuid = '49EF4146-CD70-47E9-AD26-44247F8519BF'
IF NOT EXISTS (SELECT * FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND SiteGuid = @SiteAdminGuid AND AlarmAndEventGuid = @AlarmAndEventGuid AND [UpdatedDate] >= @AlarmStatusNotNormalUpdateDate AND PriorityGuid = @AlarmPriorityNotifyGuid)
BEGIN
	DELETE FROM map.tblEmailTemplateToAlarmAndEvent where AlarmAndEventGuid in (SELECT AlarmAndEventGuid FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID
								AND PriorityGuid = @AlarmPriorityNotifyGuid AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid)
	DELETE FROM dbo.tblAlarmAndEvents WHERE ID = @AlarmAndEventID AND SiteGuid=@SiteAdminGuid AND AlarmAndEventGuid <> @AlarmAndEventGuid
	MERGE INTO [dbo].[tblAlarmAndEvents] AS tgt
	USING (
		VALUES (
		'Point Manager'
		, 1
		, @AlarmAndEventID
		, NULL
		, NULL
		, @AlarmStatusNotNormalUpdateDate
		, 'administrator'
		, @AlarmStatusNotNormalUpdateDate
		, 'administrator'
		, 1
		, @AlarmAndEventGuid
		, @SiteAdminGuid
		, NULL
		, @AlarmPriorityNotifyGuid)
		) AS src ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid)
		ON tgt.AlarmAndEventGuid = src.AlarmAndEventGuid
	WHEN MATCHED
		THEN
			UPDATE
			SET 
			[Source]=src.[Source]
			,Alarm=src.Alarm
			,ID=src.ID
			,CategoryIndex=src.CategoryIndex
			,PriorityIndex=src.PriorityIndex
			,[CreatedDate]=src.[CreatedDate]
			,[CreatedBy]=src.[CreatedBy]
			,[UpdatedDate]=src.[UpdatedDate]
			,[UpdatedBy]=src.[UpdatedBy]
			,[Enabled]=src.[Enabled]
			,SiteGuid=src.SiteGuid
			,CategoryGuid=src.CategoryGuid
			,PriorityGuid=src.PriorityGuid
	WHEN NOT MATCHED BY TARGET
		THEN
			INSERT ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid )
			VALUES ([Source],Alarm,ID,CategoryIndex,PriorityIndex,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy,[Enabled],AlarmAndEventGuid,SiteGuid,CategoryGuid,PriorityGuid );

	PRINT 'Completed INSERTING/UPDATING tblAlarmAndEvents to add/modify "Point Alarm Status Not Normal Notification"'
END

DECLARE @AlarmStatusNotNormalEmailConfigGuid UNIQUEIDENTIFIER = 'CD14B593-BDB4-4B4C-A7AF-AF41305EA975'
DECLARE @AlarmStatusNotNormalEmailUpdatedDate DateTimeOffset = '2024-03-27 18:00:00 +4:00'

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = @AlarmStatusNotNormalEmailConfigGuid AND [SettingKey]='AlarmStatusNotNormalEmail' AND [UpdatedDate] >= @AlarmStatusNotNormalEmailUpdatedDate) 
BEGIN
  DELETE FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = @AlarmStatusNotNormalEmailConfigGuid AND [SettingKey] <> 'AlarmStatusNotNormalEmail'

  MERGE INTO [dbo].[tblConfigurationSetting] AS tgt
		USING (
			VALUES (
				@AlarmStatusNotNormalEmailConfigGuid
			   ,'MULTI_SZ'
			   ,'AlarmStatusNotNormalEmail'
			   ,N'Hello!${NewLine}${AlarmID} from ${PointID} at ${SiteID}.${NewLine}Description=${Description}${NewLine}Tag ID=${TagID}${NewLine}Alarm Priority ID=${AlarmPriorityID}'
			   ,@AlarmStatusNotNormalEmailUpdatedDate,
			   'administrator', 
			   @AlarmStatusNotNormalEmailUpdatedDate, 
			   'administrator'
			   )
			) AS src(
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
			ON tgt.[SettingKey] = src.[SettingKey]
		WHEN MATCHED
			THEN
				UPDATE
				SET [ConfigurationSettingGuid]=src.[ConfigurationSettingGuid]
			   ,[SettingKey]=src.[SettingKey]
			   ,[SettingValue]=src.[SettingValue]
			   ,[CreatedDate]=src.[CreatedDate]
			   ,[CreatedBy]=src.[CreatedBy]
			   ,[UpdatedDate]=src.[UpdatedDate]
			   ,[UpdatedBy]=src.[UpdatedBy]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
				VALUES (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   );
END


DECLARE @EmailGroupGuidAlarmNotNormal UNIQUEIDENTIFIER = '91D267A7-5D80-483D-8958-B053163EB3EE'
DECLARE @EmailGroupIdAlarmNotNormal NVARCHAR(100) = 'Alarm Not Normal Notification'
DECLARE @CreatedDateAlarmNotNormal DATETIMEOFFSET = '2024-03-27 18:00:00 +4:00'
DECLARE @NotNormalAlarmPriorityToEmailGroupMappingGuid UNIQUEIDENTIFIER = 'e9edc693-2785-4166-aa35-151552111d9d'

IF NOT EXISTS (SELECT [EmailGroupGuid] FROM [dbo].[tblEmailGroups] WHERE ID = @EmailGroupIdAlarmNotNormal AND EmailGroupGuid=@EmailGroupGuidAlarmNotNormal AND SiteGuid=@SiteAdminGuid AND @CreatedDateAlarmNotNormal <= UpdatedDate)
BEGIN
	PRINT 'Begin INSERTING/UPDATING [tblEmailGroups] to add/modify "Alarm Not Normal Notification"'
	DECLARE @tmpAlarmNotNormal TABLE([ApplicationStringToEmailAddressGuid] UNIQUEIDENTIFIER,[ApplicationStringGuid] UNIQUEIDENTIFIER,[Sequence] int)

	INSERT INTO @tmpAlarmNotNormal([ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[Sequence])
	SELECT [ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[Sequence] FROM map.[tblApplicationStringToEmailAddress] m WHERE m.[EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@EmailGroupIdAlarmNotNormal)

	DELETE FROM map.[tblEntityEmailGroupToSite]				WHERE [EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@EmailGroupIdAlarmNotNormal)
	DELETE FROM map.[tblApplicationStringToEmailAddress]	WHERE [EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@EmailGroupIdAlarmNotNormal)
	DELETE FROM map.[tblAlarmPriorityToEmailGroup]			WHERE [EmailGroupGuid] IN (SELECT d.[EmailGroupGuid] FROM [dbo].[tblEmailGroups] d WHERE d.ID=@EmailGroupIdAlarmNotNormal)

	MERGE INTO [dbo].[tblEmailGroups] AS tgt
		USING (
			VALUES (
				@EmailGroupGuidAlarmNotNormal
				, @EmailGroupIdAlarmNotNormal
				, 1
				, '1900-01-01 00:00:00.0000000 +00:00'
				,'1900-01-01 23:59:00.0000000 +00:00'
				, 1
				, @CreatedDateAlarmNotNormal
				, 'administrator'
				, @CreatedDateAlarmNotNormal
				, 'administrator'
				,  @SiteAdminGuid
				)
			) AS src ([EmailGroupGuid], [ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid])
			ON tgt.[EmailGroupGuid] = src.[EmailGroupGuid]
		WHEN MATCHED
			THEN
				UPDATE
				SET 
				[ID] = src.[ID]
				,[AlwaysEnabled] = src.[AlwaysEnabled]
				,[StartTime] = src.[StartTime]
				,[EndTime] = src.[EndTime]
				,[CategoriesAndPriorities] = src.[CategoriesAndPriorities]
				,[CreatedDate] = src.[CreatedDate]
				,[CreatedBy] = src.[CreatedBy]
				,[UpdatedDate] = src.[UpdatedDate]
				,[UpdatedBy] = src.[UpdatedBy]
				,[SiteGuid] = src.[SiteGuid]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT ([EmailGroupGuid], [ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid] )
				VALUES ([EmailGroupGuid], [ID],[AlwaysEnabled],[StartTime],[EndTime],[CategoriesAndPriorities],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[SiteGuid] );

	INSERT INTO map.[tblApplicationStringToEmailAddress] ([ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],[EmailGroupGuid],[Sequence],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
	SELECT [ApplicationStringToEmailAddressGuid],[ApplicationStringGuid],@EmailGroupGuidAlarmNotNormal,[Sequence],@CreatedDateAlarmNotNormal, 'administrator',@CreatedDateAlarmNotNormal, 'administrator'
	FROM @tmpAlarmNotNormal x WHERE NOT EXISTS(SELECT TOP 1 1 FROM map.[tblApplicationStringToEmailAddress] y WHERE x.[ApplicationStringToEmailAddressGuid]=y.[ApplicationStringToEmailAddressGuid])

	INSERT INTO map.[tblEntityEmailGroupToSite] ([EmailGroupToSiteGuid],[EmailGroupGuid],[SiteGuid] ,[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
	SELECT NEWID(),e.[EmailGroupGuid],e.siteGuid ,e.[CreatedDate],e.[CreatedBy],e.[UpdatedDate],e.[UpdatedBy],e.[SiteGuid] 
	FROM dbo.tblEmailGroups e inner join dbo.tblSites s on e.SiteGuid = s.SiteGuid
	WHERE  e.[EmailGroupGuid]=@EmailGroupGuidAlarmNotNormal
		AND s.Enterprise = @IsEnterprise
		AND NOT EXISTS(SELECT TOP 1 1 FROM map.[tblEntityEmailGroupToSite] x WHERE x.SiteGuid=e.SiteGuid AND x.EmailGroupGuid=e.EmailGroupGuid)

	PRINT 'Completed INSERTING/UPDATING [tblEmailGroups] to add/modify "Alarm Not Normal Notification"'
END

INSERT INTO map.[tblAlarmPriorityToEmailGroup] ([AlarmPriorityEmailGroupGuid],[AlarmPriorityGuid],[EmailGroupGuid],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy])
SELECT @NotNormalAlarmPriorityToEmailGroupMappingGuid, a.alarmpriorityguid, e.emailgroupguid, a.createddate, a.createdby, a.updateddate, a.updatedby FROM [tblAlarmPriorities] a, [tblEmailGroups] e  
WHERE a.alarmpriorityguid=@AlarmPriorityNotifyGuid and e.emailgroupguid=@EmailGroupGuidAlarmNotNormal AND
	NOT EXISTS(SELECT TOP 1 1 FROM map.tblAlarmPriorityToEmailGroup x WHERE x.alarmpriorityguid=a.alarmpriorityguid AND x.emailgroupguid=e.emailgroupguid ) 

INSERT INTO map.[tblEntityEmailGroupToSite] ([EmailGroupToSiteGuid],[EmailGroupGuid],[SiteGuid] ,[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AssignedFromSiteGuid])
SELECT NEWID(),@EmailGroupGuidAlarmNotNormal,s.SiteGuid,@CreatedDateAlarmNotNormal,'Administrator',@CreatedDateAlarmNotNormal,'Administrator',@SiteAdminGuid
FROM tblSites s
WHERE NOT EXISTS (SELECT 1
				  FROM map.tblEntityEmailGroupToSite m
				  WHERE m.SiteGuid = s.SiteGuid
				  AND EmailGroupGuid = @EmailGroupGuidAlarmNotNormal)
	AND s.Enterprise = @IsEnterprise

DECLARE @AlarmStatusNotNormalEmailNotificationIntervalConfigGuid UNIQUEIDENTIFIER = '31C38C94-0AA1-492D-90BD-46A2889CC4F7'
DECLARE @SettingKey NVARCHAR(50) = 'AlarmStatusNotNormalEmailNotificationInterval'

IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = @AlarmStatusNotNormalEmailNotificationIntervalConfigGuid AND SettingKey=@SettingKey AND [UpdatedDate] >= @CreatedDateAlarmNotNormal) 
BEGIN
	PRINT 'Begin INSERTING/UPDATING tblConfigurationSetting to add/modify AlarmStatusNotNormalEmailNotificationInterval'
	DELETE FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = @AlarmStatusNotNormalEmailNotificationIntervalConfigGuid AND [SettingKey] <> 'AlarmStatusCheckInterval'

   MERGE INTO [dbo].[tblConfigurationSetting] AS tgt
		USING (
			VALUES (
				@AlarmStatusNotNormalEmailNotificationIntervalConfigGuid
			   ,'DWORD'
			   ,'AlarmStatusNotNormalEmailNotificationInterval'
			   ,N'15'
			   ,@CreatedDateAlarmNotNormal,
			   'administrator', 
			   @CreatedDateAlarmNotNormal, 
			   'administrator'
			   )
			) AS src(
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
			ON tgt.SettingKey = src.SettingKey
		WHEN MATCHED
			THEN
				UPDATE
				SET [ConfigurationSettingGuid]=src.[ConfigurationSettingGuid]
			   ,[SettingKey]=src.[SettingKey]
			   ,[SettingValue]=src.[SettingValue]
			   ,[CreatedDate]=src.[CreatedDate]
			   ,[CreatedBy]=src.[CreatedBy]
			   ,[UpdatedDate]=src.[UpdatedDate]
			   ,[UpdatedBy]=src.[UpdatedBy]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
				VALUES (
				src.[ConfigurationSettingGuid]
			   ,src.[KeyType]
			   ,src.[SettingKey]
			   ,src.[SettingValue]
			   ,src.[CreatedDate]
			   ,src.[CreatedBy]
			   ,src.[UpdatedDate]
			   ,src.[UpdatedBy]
			   );

	PRINT 'Completed INSERTING/UPDATING tblConfigurationSetting to add/modify AlarmStatusNotNormalEmailNotificationInterval'
END

DECLARE @AlarmStatusCheckIntervalConfigGuid UNIQUEIDENTIFIER = '16C31E18-4DE5-44D7-9EB4-A69D689DC9FC'
IF NOT EXISTS(SELECT TOP 1 1 FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = @AlarmStatusCheckIntervalConfigGuid AND [SettingKey]='AlarmStatusCheckInterval' AND [UpdatedDate] >= @CreatedDateAlarmNotNormal) 
BEGIN
	PRINT 'Begin INSERTING/UPDATING tblConfigurationSetting to add/modify AlarmStatusCheckInterval'
	DELETE FROM [dbo].[tblConfigurationSetting] WHERE [ConfigurationSettingGuid] = @AlarmStatusCheckIntervalConfigGuid AND [SettingKey] <> 'AlarmStatusCheckInterval'

    MERGE INTO [dbo].[tblConfigurationSetting] AS tgt
		USING (
			VALUES (
				@AlarmStatusCheckIntervalConfigGuid
			   ,'DWORD'
			   ,'AlarmStatusCheckInterval'
			   ,N'5'
			   ,@CreatedDateAlarmNotNormal,
			   'administrator', 
			   @CreatedDateAlarmNotNormal, 
			   'administrator'
			   )
			) AS src(
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
			ON tgt.[SettingKey] = src.[SettingKey] 
		WHEN MATCHED
			THEN
				UPDATE
				SET [ConfigurationSettingGuid]=src.[ConfigurationSettingGuid]
			   ,[SettingKey]=src.[SettingKey]
			   ,[SettingValue]=src.[SettingValue]
			   ,[CreatedDate]=src.[CreatedDate]
			   ,[CreatedBy]=src.[CreatedBy]
			   ,[UpdatedDate]=src.[UpdatedDate]
			   ,[UpdatedBy]=src.[UpdatedBy]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
				[ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy]
			   )
				VALUES (
				src.[ConfigurationSettingGuid]
			   ,src.[KeyType]
			   ,src.[SettingKey]
			   ,src.[SettingValue]
			   ,src.[CreatedDate]
			   ,src.[CreatedBy]
			   ,src.[UpdatedDate]
			   ,src.[UpdatedBy]
			   );

	PRINT 'Completed INSERTING/UPDATING tblConfigurationSetting to add/modify AlarmStatusCheckInterval'
END
PRINT 'Completed configuring email notifications for license and alarm status not normal'


------------------------------------------------- END DEFAULT SYNCHRONIZATION EVENT LOG CONFIGURATION ----------------------------------------------


------------------------------------------------- SYSTEM PICTURES ----------------------------------------------

DECLARE @TankIconImageStream VARBINARY(MAX), @TankIconId NVARCHAR(30),@Site_Guid UNIQUEIDENTIFIER, @TankIconImageHash NVARCHAR(100), @TankIconPictureGuid UNIQUEIDENTIFIER
SET @TankIconImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001F080600000095818F780000000467414D410000B18F0BFC610500000A30694343504943432070726F66696C65000048899D96775454D71687CFBD777AA1CD30142943EFBD0D20BD37A9D2446198196028030E3334B121A2021145440415418222068C8622B1228A858060C11E9020A0C4601451517933B25674E5E5BD9797DF1F677D6B9FBDF73D67EF7DD6BA0090BCFDB9BC74580A80349E801FE2E54A8F8C8AA663FB010CF00003CC0060B2323302423DC380483E1E6EF44C9113F82208803777C42B00378DBC83E874F0FF499A95C11788D20489D882CDC96489B850C4A9D9820CB17D46C4D4F81431C32831F3450714B1BC981317D9F0B3CF223B8B999DC6638B587CE60C761A5BCC3D22DE9A25E48818F11771511697932DE25B22D64C15A67145FC561C9BC6616602802289ED020E2B49C4A62226F1C342DC44BC14001C29F12B8EFF8A059C1C81F8526EE919B97C6E629280AECBD2A39BD9DA32E8DE9CEC548E406014C464A530F96CBA5B7A5A0693970BC0E29D3F4B465C5BBAA8C8D666B6D6D646E6C6665F15EABF6EFE4D897BBB48AF823FF70CA2F57DB1FD955F7A3D008C59516D767CB1C5EF05A0633300F2F7BFD8340F022029EA5BFBC057F7A189E7254920C8B03331C9CECE36E67258C6E282FEA1FFE9F037F4D5F78CC5E9FE280FDD9D93C014A60AE8E2BAB1D253D3857C7A660693C5A11BFD7988FF71E05F9FC3308493C0E17378A28870D194717989A276F3D85C01379D47E7F2FE5313FF61D89FB438D722511A3E016AAC31901AA002E4D73E80A21001127340B403FDD1377F7C3810BFBC08D589C5B9FF2CE8DFB3C265E225939BF839CE2D248CCE12F2B316F7C4CF12A0010148022A50002A4003E80223600E6C803D70061EC0170482301005560116480269800FB2413ED8088A4009D80176836A500B1A40136801274007380D2E80CBE03AB8016E830760048C83E76006BC01F310046121324481142055480B3280CC2106E4087940FE50081405C54189100F1242F9D026A8042A87AAA13AA809FA1E3A055D80AE4283D03D68149A827E87DEC3084C82A9B032AC0D9BC00CD805F683C3E0957022BC1ACE830BE1ED70155C0F1F83DBE10BF075F8363C023F8767118010111AA28618210CC40D0944A29104848FAC438A914AA41E6941BA905EE42632824C23EF50181405454719A1EC51DEA8E528166A356A1DAA14558D3A826A47F5A06EA2465133A84F68325A096D80B643FBA023D189E86C7411BA12DD886E435F42DF468FA3DF6030181A46076383F1C6446192316B30A598FD9856CC79CC20660C338BC56215B00658076C20968915608BB07BB1C7B0E7B043D871EC5B1C11A78A33C779E2A2713C5C01AE127714771637849BC0CDE3A5F05A783B7C209E8DCFC597E11BF05DF801FC387E9E204DD0213810C208C9848D842A420BE112E121E11591485427DA1283895CE2066215F138F10A7194F88E2443D227B991624842D276D261D279D23DD22B3299AC4D7626479305E4EDE426F245F263F25B098A84B1848F045B62BD448D44BBC490C40B49BCA496A48BE42AC93CC94AC993920392D35278296D293729A6D43AA91AA95352C352B3D2146933E940E934E952E9A3D257A52765B032DA321E326C99429943321765C628084583E246615136511A289728E3540C5587EA434DA69650BFA3F653676465642D65C36573646B64CFC88ED0109A36CD87964A2BA39DA0DDA1BD9753967391E3C86D936B911B929B935F22EF2CCF912F966F95BF2DFF5E81AEE0A190A2B053A143E191224A515F3158315BF180E225C5E925D425F64B584B8A979C58725F0956D2570A515AA37448A94F69565945D94B394379AFF245E569159A8AB34AB24A85CA599529558AAAA32A57B542F59CEA33BA2CDD859E4AAFA2F7D067D494D4BCD5846A756AFD6AF3EA3AEACBD50BD45BD51F691034181A091A151ADD1A339AAA9A019AF99ACD9AF7B5F05A0CAD24AD3D5ABD5A73DA3ADA11DA5BB43BB42775E4757C74F2749A751EEA92759D7457EBD6EBDED2C3E831F452F4F6EBDDD087F5ADF493F46BF4070C60036B03AEC17E834143B4A1AD21CFB0DE70D88864E4629465D46C346A4C33F6372E30EE307E61A269126DB2D3A4D7E493A99569AA6983E9033319335FB302B32EB3DFCDF5CD59E635E6B72CC8169E16EB2D3A2D5E5A1A58722C0F58DEB5A25805586DB1EAB6FA686D63CDB76EB19EB2D1B489B3D96733CCA0328218A58C2BB6685B57DBF5B6A76DDFD959DB09EC4ED8FD666F649F627FD47E72A9CE52CED286A5630EEA0E4C873A871147BA639CE341C711273527A653BDD313670D67B673A3F3848B9E4BB2CB319717AEA6AE7CD736D739373BB7B56EE7DD11772FF762F77E0F198FE51ED51E8F3DD53D133D9B3D67BCACBCD6789DF7467BFB79EFF41EF651F661F934F9CCF8DAF8AEF5EDF123F985FA55FB3DF1D7F7E7FB7705C001BE01BB021E2ED35AC65BD61108027D0277053E0AD2095A1DF46330263828B826F8698859487E486F28253436F468E89B30D7B0B2B007CB75970B9777874B86C7843785CF45B84794478C449A44AE8DBC1EA518C58DEA8CC64687473746CFAEF058B17BC5788C554C51CC9D953A2B73565E5DA5B82A75D59958C95866ECC938745C44DCD1B80FCC40663D7336DE277E5FFC0CCB8DB587F59CEDCCAE604F711C38E59C89048784F284C94487C45D8953494E499549D35C376E35F765B277726DF25C4A60CAE19485D488D4D6345C5A5CDA299E0C2F85D793AE929E933E986190519431B2DA6EF5EED5337C3F7E632694B932B3534015FD4CF50975859B85A3598E5935596FB3C3B34FE648E7F072FA72F573B7E54EE479E67DBB06B586B5A63B5F2D7F63FEE85A97B575EBA075F1EBBAD76BAC2F5C3FBEC16BC3918D848D291B7F2A302D282F78BD29625357A172E186C2B1CD5E9B9B8B248AF845C35BECB7D46E456DE56EEDDF66B16DEFB64FC5ECE26B25A62595251F4A59A5D7BE31FBA6EA9B85ED09DBFBCBACCB0EECC0ECE0EDB8B3D369E79172E9F2BCF2B15D01BBDA2BE815C515AF77C7EEBE5A695959BB87B047B867A4CABFAA73AFE6DE1D7B3F542755DFAE71AD69DDA7B46FDBBEB9FDECFD43079C0FB4D42AD796D4BE3FC83D78B7CEABAEBD5EBBBEF210E650D6A1A70DE10DBDDF32BE6D6A546C2C69FC78987778E448C8919E269BA6A6A34A47CB9AE16661F3D4B1986337BE73FFAEB3C5A8A5AE95D65A721C1C171E7FF67DDCF7774EF89DE83EC938D9F283D60FFBDA286DC5ED507B6EFB4C4752C7486754E7E029DF53DD5DF65D6D3F1AFF78F8B4DAE99A33B267CACE12CE169E5D3897776EF67CC6F9E90B8917C6BA63BB1F5C8CBC78AB27B8A7FF92DFA52B973D2F5FEC75E93D77C5E1CAE9AB76574F5D635CEBB86E7DBDBDCFAAAFED27AB9FDAFAADFBDB076C063A6FD8DEE81A5C3A7876C869E8C24DF79B976FF9DCBA7E7BD9EDC13BCBEFDC1D8E191EB9CBBE3B792FF5DECBFB59F7E71F6C78887E58FC48EA51E563A5C7F53FEBFDDC3A623D7266D47DB4EF49E8930763ACB1E7BF64FEF261BCF029F969E584EA44D3A4F9E4E929CFA91BCF563C1B7F9EF17C7EBAE857E95FF7BDD07DF1C36FCEBFF5CD44CE8CBFE4BF5CF8BDF495C2ABC3AF2D5F77CF06CD3E7E93F6667EAEF8ADC2DB23EF18EF7ADF47BC9F98CFFE80FD50F551EF63D727BF4F0F17D21616FE050398F3FC1437453B000000206348524D00007A26000080840000FA00000080E8000075300000EA6000003A98000017709CBA513C00000006624B474400FF00FF00FFA0BDA793000000097048597300000B1300000B1301009A9C180000000774494D4507E0041D0D262694680D2A0000037B4944415458C3EDD85B88D6451806F0DFDA5F49534BD94E2A6687C5CC200D920843E84045054546415414E81451DD44D245175184571D0884170D0A02EBA24088488AA2D00E8A905D5456A42E4B9AEB79D5D2CCAF9BF7937FCBE6B7DFB60B7BD10B73F1CD3733FF679E799E7967A6A3D168684629A5C244ED4523220E18A1E868341A4A29F3F11216A21AC23807F1169E898883C30A70E9D2A597E36B4C1886F1BEC0A288383E5C002BBC98E03EC1C311B1BD9D014A29A725F3EFE21ADC8DD5C3C9602F3A7145447C3B08401566E07C9C8B337116EEC4227C8EF7D0974BBF1B3BB02D228E0E85C1AAA6A38100CDC42DB81657E212FC811EECC23EECC7F8EC320DF33029C19F9375534B29DDF8065FE2C3C110529D82A939782597EFE3646625BE8F885D03B47F120BB02E221E1AE0FFC9989D935C8865A5941E3C1D116BDB06989A5C8DC511D1F75FB594EEDE98254A2963F100DE2FA55C1C11DDED023C0FF7E35829650D36B670E7072985B75B68B80373703B962486A9E82EA5CCC205B5E6FBAB533813EEC562BC931ADA8CEFF0733F0D1EC9F248F69F8D33D23C9D69AA59B80CF373ECB578022BD0FCDEEBB8AE06E570F52FCBF1572905EEC3B311F1689A657ECEFEA2746C27A62498F1B5218EE170823FE962ACC2666CC93E8F27F8133542A6D7C6D9D72A6B4CC04FA594AF72C61BB03222F6B4ABC152CA44CCC5F5780137623D0EA091C4F4A277B01A944BF6186E4BEA1F445729A50FDDD889BD38948C35E3F464682ACEC6CCDC33B763133EC35311B1B594B2B59FB426355721228E548370DF1EBC994529655CEA697A7E744A1E30EAA9F26802DE9B4BDC931BF5E1169F5B835B9B004B295DD510B68B63F831CB70C7727CD434087EAD8CA288887558D78E06076B80091857AB3A315CC7AEAA0D000B6ADBCC85B93D74A611C60CD0A799DF7F4B336DC30F998B3744C4EE01FA2CC74DB5AA1DAD00DE554A599C0E1E3704022667E9CAC3C649864B299BF006C6D6EABB7322CDE8690570D508C96D0CAECA52D7E08ACC2CFF6838AA6354B9B894720FAECE9FBFE3F96A9411764396E666FFDA68DB07970C24D6FF35D88606E7E69DA719BD75807F8D028CAFE671AC197D159ECBACD0D3AF714FD68F741C47F37C794766A7661CEAA8BFCDF4A3BB0B2FE73D63A4B4BA11CB22E2D353BECDB4D0C534DC9CA96A1E2ECD03E95098FA258FFCEBF35EBC65508F476D0AB9235F1566B47127D9899E88F8B3DD59FD0DE48A2E6D581B34C80000000049454E44AE426082
SET @TankIconId = 'Tank Template'
SET @TankIconPictureGuid = '01C81D1D-2BE2-430E-8352-B4AFFE70EC42'
SET @Site_Guid = '00000000-0000-0000-0000-000000000001'
--SHA1 Hash computed using base64 representation of byte stream saved in @ImageStream variable above
--Used the following website: http://www.movable-type.co.uk/scripts/sha1.html
set @TankIconImageHash = '1222e2f9d484d07bdc80cf447d93e0567f854fab' 

DECLARE @BlankImageStream VARBINARY(MAX), @BlankId NVARCHAR(30), @BlankImageHash NVARCHAR(100), @BlankPictureGuid UNIQUEIDENTIFIER
SET @BlankImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001E08060000005EDD5CDD0000001974455874536F6674776172650041646F626520496D616765526561647971C9653C000000874944415478DA626018059401467C92696969FFE9E59059B36661750BD3600F411622D50902B1010D42ED00A15822D68120C7AD07E20B54749F03A124468A0341E002D0C78ED4721DB1E97BD0A7C151078E3A70D481A30E1C75E0A803471D38EAC051078E3A7058749AC07D087A7643497220A8F7454C0767C01C3810213764D2E0A00700010600FCCE17734C42B4110000000049454E44AE426082
SET @BlankId = 'Blank Template'
SET @BlankPictureGuid = '4D702ECA-EA22-42B4-9744-BFD39192D4F9'

--SHA1 Hash computed using base64 representation of byte stream saved in @ImageStream variable above
--Used the following website: http://www.movable-type.co.uk/scripts/sha1.html
set @BlankImageHash = 'de05b7efa8991d33b15733aa5103e8a266b53a7b' 

DECLARE @BulletTankImageStream VARBINARY(MAX), @BulletTankId NVARCHAR(30), @BulletTankImageHash NVARCHAR(100), @BulletTankPictureGuid UNIQUEIDENTIFIER
SET @BulletTankImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001E08060000005EDD5CDD0000001974455874536F6674776172650041646F626520496D616765526561647971C9653C000002B04944415478DACC58CB71E24010151477138242183240112C8EC0E8C21588001481B4572EB2234044803602B41158CE0047B0DBE37AE3EA6ACF8C8490BCDB555D9286F93CBDD7DD332208FE731BDDD279B55ACDE93217CDD5E17028FE19400215D26547BE209F3ABA95E48F04F4DA37C04903B83DC019AB01E60DCF0FE44BB07A248FBE85410236C58273C650420C951676D7E41B3465E4BF217B350840803B932B722DD996167B167D9600A63C736BB65F34689FF434D7D912D701D69E8D2D3FA40C5CC4C1D1648AFC956EF30670DA4CECBED298452F0C224BCF788CB8A4602D176FA8C19FC08006939017785E03A431CDE4F6DE24C9D9643E7009970E2FF661883DED19C6A5C8FE0D3DBFD3EF7B4FDC6F789BEE3B16EC85602611899032D634B3FB362505E111619CB69D476E05153E5D87148FC11FB81662F19CD5BF5866324B0817C84A804C1DFDF4BC31C849B056C5011A994E3C29587BE6DA31C0D4CC251F409AF80B21BDCD2A94B412F7C158506C6A9EB127266DD22067D5426ED367EDD8462F4852ED1729B19988CBBB70C8DED57E1A32901481659732AED5AA272D6A9994DD487EEDB05B9442B1921153DBB6CA89A7C22BF17C648C9A36B3E8092CD70D32D71823CB4B6E398868C5621F837CC0D1D3CF1CC1525AAC8634BFB43C9C6130AF1C2AB94A4F326213FCC15B7E691BC83E772AA8F58541596602C7863D945D7905B0D4D769E37910F1B518029C4C30710E306DB32680A78100168E22FDCC2A47DDA6CC146CB3EFD31247FD8D6F3AF2EB4144F3569C64EEB5CC568E9028674686F3C06ADBA2FAFA6AAB1AB6CCE94D0C328B111BEA0E70B5EFCB0F4933EA0410524788C76547E6221B389C0F9523D3B3B60C1A900926537D3187970E1DDB6DD51A204ED6970E191DE2C3E9D1719EDC7A182C27372CB4BBB3DCA4B66403E8A2CD4753E93BBA23EDCBEFFEF3E8AF000300AD4A4C650081CDAE0000000049454E44AE426082
SET @BulletTankId = 'Bullet Tank Template'
SET @BulletTankPictureGuid = '5DDE6A21-1DA4-4907-8529-D743B236F167'

set @BulletTankImageHash = 'b253e14db5ae1c252c299a250f6f3be35f0fc1fe' 

DECLARE @DialImageStream VARBINARY(MAX), @DialId NVARCHAR(30), @DialImageHash NVARCHAR(100), @DialPictureGuid UNIQUEIDENTIFIER
SET @DialImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001E08060000005EDD5CDD0000001974455874536F6674776172650041646F626520496D616765526561647971C9653C000002354944415478DAEC57CB6DC2401005C43DEE20A603BB823815402A005FB8625700AE0072E5025480A90052419C0E5C824BC8ACF42C8D36E3F5FA8312458C34329F61F6EDCC9B0F83C143FEB90CFB72B45C2E1D7A78785BECF7FBEC570112A0193D5E002AA830CB4915D00BE98D40E77705882845A47352B7C5BD8EA427027AEB1D20227620757AC8DCA48C26A3462E45786C19B52DE9A2C2A4401AB9703E4AA27C6DC8B7CAC219B605BDF77590430B7057E130E5E4449A9A8A01519F0A97F3F1BC6A19F1757FC386E054B41272B26BC85D15A93578FB0E50079D9FE437B4E62039BD6AD5A988FD464E8A8EEDE820443423BFBE643FAE701269E0C4DBB5E80067A125A90BBF5A5731D2F1C9B8D107380FE05C099C89C75204D70C5CD603B805BA80D49E6255706473C6F7EABC981B8C84E8717E7405B731F4CE1D813902FC0CA98FF01B19200C39EFB2B67C4354D61526298B949EF615F82AA678CE5E270D0B4045FE1907BA86469D6999B96885E320504709A0C76E985B826B32025551845AAB92E6F2B404386207F15B7C58820B509DB6F339D4695341A399C441A7E656926C1B503326306903DA043A40AFE65652BFF484752A412AF5A2308D47E93CC76A9B31882B8CAB10E09FB03796F255E3CBABF82CE5005354F1A92D604C8CDCB0619B8AC7314E12A47562EB516DC504883B763022C5BE67B16947C2673F1A759B15BE4E6E759C46D34E509C29E673DE958365330F0C4DB9C0BCB5C9C8E66E7F3B313F572CDD05221177DD1F7B15CC6077F0903F24DF020C001A7FDF6551D0AB9F0000000049454E44AE426082
SET @DialId = 'Dial Template'
SET @DialPictureGuid = '60040373-938F-4539-B163-E155FE7832BD'

set @DialImageHash = '0f7c50e8b239a61b26bd86c75f81626e6afe9ead' 

DECLARE @PumpImageStream VARBINARY(MAX), @PumpId NVARCHAR(30), @PumpImageHash NVARCHAR(100), @PumpPictureGuid UNIQUEIDENTIFIER
SET @PumpImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001E08060000005EDD5CDD0000001974455874536F6674776172650041646F626520496D616765526561647971C9653C0000024D4944415478DAEC57C175824010159FF7500254105281A48260077AF12A5600541073F52256103B102B081D480721152433BED9BCC9661610D078C8BC372E0F66766777E6CF5F07837FE92616FECCE7731F863D68029AF53477B15EAF8BAE938C8477FBBE760F1B2F61D8816E21D8561B1F5E384336E814370DC1EE2953371520179F028D6F3540251104B9F9AB0033A655326D7A92A31E82CA415F0004A90012ACBF08D4319C6456079EAE01AE6081250B084161ABF64241A794D2A9E08FEFDD4BA5580FCE83E1081AE8866037C34085391CF00BCE6DD463CDC6A376F1A3D660D147EDE48E6487EF4B16E80E6C73B27BA3F9B8E0F7495D8A0BC522601C6B75F429F825EAD46871BE89885A0AAFB584E645BF576DAEFA13AC60025F6016A43097BEE3B703A1566220649125A73CF0791732F2A04EB98F1AD4F935124EE5BBC60404E706C6690512899A0EEC795BB38047ECE1B745E2B0418FD3E59EA13355F558238B4B05581AD2C65B485C11E48C36699BFCBB328974821EA4CCC1C28731A41AB4B51A75D833B69D9081CE11FA64A681537585BC3240702CC138177AD733E88435DF2756930521BA640BC70C50BF4027F0F298EAFFB109D56D850003E459AAC115A9DA7D488F4B81977D43C944C2A5A3719B490DB5B8A145A536E4F2CB03D96DDA80C46A78750F29ADA60D24D2FF0FAC55F20B5A5CDB4E29B69A7A106BF83580C2893F40EFC8D6EB70AF6C5C834A2654FC5E45536E1B50F7EB16229A5A4636B8A2586D9CA82D2CAA38B487BF0EA714B7BAB0127BB8C420C5CD9DA001AD4E0D88CE91F1D928BEA670AAFB126000007DDB93C92C33BE0000000049454E44AE426082
SET @PumpId = 'Pump Template'
SET @PumpPictureGuid = '4617DF3C-A6CF-4947-88EB-ED6780048467'

set @PumpImageHash = 'b4bd7398711765e3f63b4cbd1f7ae14a2a6daa68' 

DECLARE @SquareTankImageStream VARBINARY(MAX), @SquareTankId NVARCHAR(30), @SquareTankImageHash NVARCHAR(100), @SquareTankPictureGuid UNIQUEIDENTIFIER
SET @SquareTankImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001E08060000005EDD5CDD0000001974455874536F6674776172650041646F626520496D616765526561647971C9653C000002B24944415478DACC58ED71E23010C5190AE02A38D301A92050414C05903FFC0DAE80BB0A027FF983A920A682F355105F05E70EE20E726F334F371B8D8565E17CECCC8EC6D85A3DEDBEDD95180CBEB844F60FABD56ADAD5C87EBF2FDE1D20802D313C404701766AE80C40CBAE13B16E8C21766D7CA89E0DB88AEA2B13CEDB40E7019B7B723905E0530D7044D4E38E1E48303C067A5EA43833B7D2217E21C0C8139818155A2CE84509730EDD8584DA25C300CE24049558AF0CE025BE118A1CA119C0562DF62696072B3D67E8094A8CDC13944F2863727283B905C1E658B87670D00EF9AC1520B32B21B0F882284DA907D8CC309E003457EF7F426FD4F3CE37C4134E8C7B2C6B02B416AF1A6F62FC11C441EE326F4886901A29B68E4D051DF6D7166D729D64E7427CA071C9CA2DC62DB96812A4CDB302EAA4B9C766704B9B85AABF5A6EBC3848001292350C97E4862C96624C99CDB7F4EEFF0C54DF55A67542175682699ECDADC8E4AE56F7A60EE2F917013679E668884E0AC8E2A5090D13CCD0A2C9D333DFFEED53664AEEDC2C2460128090B065049B095086F0DEC1D5BCA176CA661E1CDFBFDABDF2007862FB9B1350AD0AB310FC098BFCC5F82CA5C45A4C36279418C386AB4FAF5529D2BAE8D4495456A7F4C4425120B6B2D678D6A7E55D3B8A7FE50BF0BB05D400C8AC625E3414619F8D97977270496EE5049029E3B2CB2DB5AD5D1E1CBFDB27A1DFBA7877392C98E490D0DEB90E01CCEA5865F4947D797AA6BB8C2C8A849F66685012E34E87935EDE183EE2D9D7DE5827965D7E42009A0C7E64F3FFC3EA9F841822A78BDECE83363F2F3D3D6093CF5688B7EC56AF7235F87CA97CB3B86661FD5081B7AEBD00E2C36FD6BB9D4A8A3EBD956B47B45D3B230F8EF471B2CE5C45BC81835AD2A823A1BBDC4DDAEE22AE42FD268AD105D9D774BBF3BECD05FF371300D45C098A3EEFC346FE093000E81C33E25E3589750000000049454E44AE426082
SET @SquareTankId = 'Square Tank Template'
SET @SquareTankPictureGuid = '73612C2F-2729-47A3-9299-3C930F689673'

set @SquareTankImageHash = '7e7bbe487e3fdc44be9f8923051952d1e4fe1b24' 

DECLARE @ValveImageStream VARBINARY(MAX), @ValveId NVARCHAR(30), @ValveImageHash NVARCHAR(100), @ValvePictureGuid UNIQUEIDENTIFIER
SET @ValveImageStream = 0x89504E470D0A1A0A0000000D49484452000000280000001E08060000005EDD5CDD0000001974455874536F6674776172650041646F626520496D616765526561647971C9653C000001B54944415478DAEC973F4EC33018C553C8C098918D1C2152976E2927A03D4161C90A1D99DA9E8076435D684F006C8C6463899423980DB6B0B195F749AF92653591E3B66A85F2494F96FFD5BFBC2FB15DCF3BF268B94E4C92244211405D8BE1EF50319FCFF3BD01024860AEA12B4BA82AD8576801E0622780807B407157B298823EB5B61134D1EA175058F25053400EABD63EB58013B0B106B424944487E519F40DC584295817B888E324BD6F74CFE3B84EBBDDFEC9B2ECA36C7DDFC2C098E50B9EB66FC0874CFB2DD4D3BA7A9480CEA03EE62A63EE33C7C82B33754E317E48009E58557430A633015DCD59EF1AE9D7DB0BD653684007256E00BF704E31ECCF250D4CD339170B995659F017FA625FA84D555A7BC1BEF5BB18B0ED1E708FBBFC8A7B74AF6BC0D886A2B3295F99629FFB60C0F479048E37A438652991DB00ED0C7003F0985BCC3A26001A1FEC2431E056657D806C1D0C90AE79867366C8A6ADAABED4AAF0B7346F643946DE437740EE75831AF3962E8EF0C88C6A4C19FADAB153E702903A3A1ED55C273839F6FB6003D80036800D6003D8006E17BE76FBADFBE7DB259635CF71B5ED7D7065FB40B8FD5C7AFF31FE04180064318432974199D50000000049454E44AE426082
SET @ValveId = 'Valve Template'
SET @ValvePictureGuid = '817A6F13-964D-44FC-99B0-E5AD47F4E119'

set @ValveImageHash = 'be29421cbdd3fea6296b6fd35177553a1441a8b8' 


SELECT @TankIconPictureGuid as [PictureGuid],
		@TankIconId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@TankIconImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@TankIconImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]
INTO #ImagesTemp
UNION
SELECT @BlankPictureGuid as [PictureGuid],
		@BlankId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@BlankImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@BlankImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]
UNION
SELECT @BulletTankPictureGuid as [PictureGuid],
		@BulletTankId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@BulletTankImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@BulletTankImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]
UNION
SELECT @DialPictureGuid as [PictureGuid],
		@DialId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@DialImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@DialImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]
UNION
SELECT @PumpPictureGuid as [PictureGuid],
		@PumpId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@PumpImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@PumpImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]
UNION
SELECT @SquareTankPictureGuid as [PictureGuid],
		@SquareTankId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@SquareTankImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@SquareTankImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]
UNION
SELECT @ValvePictureGuid as [PictureGuid],
		@ValveId as [ID],
		'Point Template Icon' as [Description],
		'Point Template Icon' as [Standard],
		'image/png' as [ContentType],
		@ValveImageStream as [ImageStream],
		1 as [IsSystemImage] ,
		@ValveImageHash as [ImageHash],
		@Site_Guid as [SiteGuid],
		N'03/08/2017 10:23:55 AM -05:00' as [CreatedDate],
		'Administrator' as [CreatedBy],
		N'03/08/2017 10:23:55 AM -05:00' as [UpdatedDate],
		'Administrator' as [UpdatedBy]


MERGE dbo.tblPictures AS Target
USING 
( SELECT * FROM #ImagesTemp) 
AS Source
ON (Target.[PictureGuid] = Source.[PictureGuid])
WHEN MATCHED THEN
    UPDATE SET target.[PictureGuid] = source.[PictureGuid],
				target.[ID] = source.[ID],
				target.[Description] = source.[Description],
				target.[IsSystemImage] = source.[IsSystemImage],
				target.[ImageHash] = source.[ImageHash],
				target.[ContentType] = source.[ContentType],
				target.[ImageStream] = source.[Imagestream],
				target.[SiteGuid] = source.[SiteGuid],
				target.[CreatedDate] = source.[CreatedDate],
				target.[CreatedBy] = source.[CreatedBy],
				target.[UpdatedDate] = source.[UpdatedDate],
				target.[UpdatedBy] = source.[UpdatedBy]
WHEN NOT MATCHED BY TARGET THEN
    INSERT ([PictureGuid],[ID], [Description], [ContentType], [ImageStream], [IsSystemImage], [ImageHash], [SiteGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy])
    VALUES (Source.[PictureGuid], Source.[ID], Source.[Description], Source.[ContentType], Source.[ImageStream], Source.[IsSystemImage], Source.[ImageHash],
	Source.[SiteGuid], Source.[CreatedDate], Source.[CreatedBy], Source.[UpdatedDate], Source.[UpdatedBy]);

DROP TABLE #ImagesTemp

------------------------------------------------- STANDARD TEMPLATEs -------------------------------------------------

:r .\Script.WellKnownTagGuid.sql

:r .\Script.StandardModules.sql

:r .\Script.StandardTank.sql

:r .\Script.StandardMovement.sql

:r .\Script.StandardVolume.sql

:r .\Script.StandardMovementControl.sql

:r .\Script.Tdu.sql

:r .\Script.StandardNode.sql

DECLARE @SiteGuid UNIQUEIDENTIFIER  = '00000000-0000-0000-0000-000000000001'

-- This handles the update of point alarms at the time when AlarmPriority was renamed to Order.  This is not editable at the point level
UPDATE tblAlarm SET [ORDER] = CASE WHEN (SELECT TOP(1) [ORDER] FROM tblAlarmTemplate at WHERE tblAlarm.ID = at.ID) IS NULL THEN 0 ELSE (SELECT TOP(1) [ORDER] FROM tblAlarmTemplate at WHERE tblAlarm.ID = at.ID) END

-- This handles the update of point for any point template changes
UPDATE tblPoint SET PointTemplateVersion = (SELECT [Version] from tblPointTemplate WHERE tblPointTemplate.PointTemplateGuid = tblPoint.PointTemplateGuid)




------------------------------------------------- END STANDARD TANK TEMPLATE ----------------------------------------------

IF (SELECT COUNT(*) FROM [lookup].[tblPointTagInputOutputType])=0
BEGIN
	INSERT INTO [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex], [PointTagInputOutputTypeCode], [PointTagInputOutputTypeName], [PointTagInputOutputTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'UNDEFINED', N'UNDEFINED', N'80F5A628-3F6A-4D8A-88B0-7D533268BB8C', N'10/17/2014 1:00:28 PM +00:00', N'Administrator', N'10/17/2014 1:00:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex], [PointTagInputOutputTypeCode], [PointTagInputOutputTypeName], [PointTagInputOutputTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'MANUAL', N'MANUAL', N'AB3B5FC8-DE62-487E-B939-80F617E4D4E2', N'10/17/2014 1:00:28 PM +00:00', N'Administrator', N'10/17/2014 1:00:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex], [PointTagInputOutputTypeCode], [PointTagInputOutputTypeName], [PointTagInputOutputTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'CALCULATED', N'CALCULATED', N'FC81D913-EA25-4087-AE3B-4674E9B4E267', N'10/17/2014 1:00:28 PM +00:00', N'Administrator', N'10/17/2014 1:00:28 PM +00:00', N'Administrator')
	INSERT INTO [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex], [PointTagInputOutputTypeCode], [PointTagInputOutputTypeName], [PointTagInputOutputTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (3, N'OPC UA', N'OPC UA', N'DC397705-E521-4DD4-B8E1-0C071A5C60DC', N'10/17/2014 1:00:28 PM +00:00', N'Administrator', N'10/17/2014 1:00:28 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblPointTagInputOutputType])= 4
BEGIN
	INSERT INTO [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex], [PointTagInputOutputTypeCode], [PointTagInputOutputTypeName], [PointTagInputOutputTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (4, N'FCEE', N'FCEE', N'27E2A85F-D1F8-4FCE-87FC-82E63E1A041D', N'09/30/2022 1:00:28 PM +00:00', N'Administrator', N'09/30/2022 1:00:28 PM +00:00', N'Administrator')
END

IF (SELECT COUNT(*) FROM [lookup].[tblPointTagInputOutputType])= 5
BEGIN
	INSERT INTO [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex], [PointTagInputOutputTypeCode], [PointTagInputOutputTypeName], [PointTagInputOutputTypeGuid], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (5, N'SYSTEM', N'SYSTEM', N'4D67F26B-0570-43EA-9A5C-E2ACA6646849', N'05/03/2023 1:00:28 PM +00:00', N'Administrator', N'05/03/2023 1:00:28 PM +00:00', N'Administrator')
END


IF (SELECT COUNT(*) FROM [lookup].[tblPointServiceHealthStatus])=0
BEGIN
	INSERT INTO lookup.tblPointServiceHealthStatus (PointServiceHealthStatusIndex,PointServiceHealthStatusCode,PointServiceHealthStatusName,PointServiceHealthStatusGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (0,'GOOD','FMPoint Service Available For Point Processing','AAF70078-E683-433C-9F40-A069C48AF651',N'08/29/2015 1:24:00 PM +00:00', N'Administrator', N'08/29/2015 1:24:00 PM +00:00', N'Administrator')
	INSERT INTO lookup.tblPointServiceHealthStatus (PointServiceHealthStatusIndex,PointServiceHealthStatusCode,PointServiceHealthStatusName,PointServiceHealthStatusGuid,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy) VALUES (1,'BAD','FMPoint Service NOT Available For Point Processing','1ACEC155-3D14-414C-AFD1-2F562C6537B4',N'08/29/2015 1:24:00 PM +00:00', N'Administrator', N'08/29/2015 1:24:00 PM +00:00', N'Administrator')
END

------------------------------------------------- CORRECT UNITS TYPE FOR TAGS THAT ARE NOT Double or Single ----------------------------------------------

UPDATE [dbo].[tblPointTemplateTag] SET EngineeringUnitsType = 16 where  ValueType <> 'System.Double' and ValueType <> 'System.Single'
UPDATE [dbo].[tblPointTag] SET EngineeringUnitsType = 16 where  ValueType <> 'System.Double' and ValueType <> 'System.Single'

------------------------------------------------- ADD THE WELL KNOWN GUID FOR THE TANK STATUS ------------------------------------------------------------
UPDATE [dbo].[tblPointTemplateTag] SET WellKnownIdentityGuid = '834B9D8A-C17A-48f6-97FC-1B18EB562866' where ID = 'Tank Status'


----------------------------------------- DELETE OBSOLETE ERV EXTERNAL ATTRIBUTES -------------------------------------------------------------------------
IF EXISTS (SELECT * FROM [erv].[tblEntityExternalAttribute] WHERE EntityExternalAttributeGuid = 'D12B776D-B14A-4ACF-9DBC-C2AF4FA7D779')
BEGIN
	--Delete Personnel CompanyGuid internal attribute field. This field is being replaced by new Personnel and Company external attribute entries against the [map].[tblCompanyPersonnelAssignedToCompany] mapping table.
	DELETE [erv].[tblEntityExternalAttribute] 
	WHERE EntityExternalAttributeGuid = 'D12B776D-B14A-4ACF-9DBC-C2AF4FA7D779'
	AND EntitySegmentTemplateGuid = '825F4C39-F7ED-43F5-B35D-AE2E5DAD6281'
	AND InternalFieldName = 'CompanyGuid'
	AND RelationshipName = 'Company'

	--Delete Company [map].[tblCompanyToRole] external attribute field. Company Roles are created/cloned and deleted independently of Record Versioning during company-to-site assignments. They are maintained separately in map.tblCompanyToRole through the Company Role Assignment Web page, using a combination of Company MasterRecordGuid and Siteguid.
	DELETE [erv].[tblEntityExternalAttribute] 
	WHERE EntityExternalAttributeGuid = '83413851-708C-482C-9CC9-5A630B696D37'
	AND EntitySegmentTemplateGuid = '44642D4C-6CDD-4BDE-B246-68EDC01A064F'
	AND RelationshipTableName = '[map].[tblCompanyToRole]'
	AND RelationshipName = 'CompanyRoles'
END

----------------------------------------- UPDATE tblAlarm references to tblAlarmTemplate -------------------------------------------------------------------------

UPDATE tblAlarm SET AlarmTemplateGuid = (SELECT AlarmTemplateGuid FROM tblAlarmTemplate at WHERE at.ID = tblAlarm.ID AND at.InputTemplateTagGuid = (SELECT PointTemplateTagGuid FROM tblPointTag pt WHERE pt.PointTagGuid = tblAlarm.InputTagGuid))  

----------------------------------------- INSERT Module to Site Entity Mappings-----------------------------------------------------------------------------------

declare @ModuleGuid uniqueidentifier
declare @ModuleToSiteGuid uniqueidentifier

set @ModuleGuid = '9F0BEAE3-FFC2-47ED-9C01-0E724D6813F8'
set @ModuleToSiteGuid = 'A26C093E-A87A-4517-97DB-C67E39C85AD6'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '66120ECB-547A-4A48-B9C2-2056068746E6'
set @ModuleToSiteGuid = 'BB173B87-41D4-46F2-8504-F44C9B81B1EA'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '89D5B38A-214B-45D5-B2ED-36101D527508'
set @ModuleToSiteGuid = '83731DCF-668E-4086-8FF2-542665182623'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '1DA35F8B-1DB1-4D72-95FB-42B15D03FF5B'
set @ModuleToSiteGuid = '93846A43-44CE-4CB5-BE2B-4BDDE00A135E'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = 'F769E8AF-1F5F-4EC7-A2E5-58759EF79186'
set @ModuleToSiteGuid = '3B7A1795-4AF4-498A-B4E7-39D86C3E2D80'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '923120A7-1A76-4A72-94BF-75B84265E503'
set @ModuleToSiteGuid = 'DE2D5790-4040-4835-ACBD-9157496E429F'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '9FA81AF0-451C-47C6-A0A1-86B4377EC79B'
set @ModuleToSiteGuid = '1CAD9B6C-6085-4B89-84D3-74B45C743509'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = 'DFA650C2-FD52-45BF-A3C6-8799F928027C'
set @ModuleToSiteGuid = '7CCC3BA9-B832-4D67-AD6C-9EEEAF079B65'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = 'A109B4D4-CB54-4361-A331-A585344C01D9'
set @ModuleToSiteGuid = '2A08D5D7-FADB-483C-BD70-76343068EE06'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '07B4584F-7B0B-436B-80A2-A9D5E89FE4F2'
set @ModuleToSiteGuid = '5596E31B-7EB9-418F-BCB2-02E8989210F0'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = 'DB8313DD-E9BD-4BCF-8584-B3B6B33E827E'
set @ModuleToSiteGuid = '1CC43269-EFE6-43FB-AB0F-F54402D2CB55'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '26DE3166-5417-415C-9801-BB2E363D2447'
set @ModuleToSiteGuid = '5504320F-DFD0-4AD0-B8EF-686A8196D080'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '7AAC4E9D-46A4-4AEC-AC8E-D1543BE71532'
set @ModuleToSiteGuid = '6D78A7A3-4156-4318-AC4E-054DD64CE9B3'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '0802B471-540B-4128-8535-D597E9328BEC'
set @ModuleToSiteGuid = '1A8C5964-CD42-48BE-A958-815E77D6F126'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = '06B43B26-8383-4AA5-A9FF-DB46E4F3578A'
set @ModuleToSiteGuid = 'E9F3B78A-4B8F-437E-B22A-50EC4D7C1BEA'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')

set @ModuleGuid = 'E0024C94-0725-4423-9261-EDE9D84A6ACC'
set @ModuleToSiteGuid = 'E96185A0-863A-445D-B5DA-A30FDF3B47A6'
if exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid) delete map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid <> @ModuleToSiteGuid
if not exists(select * from map.tblEntityModuleToSite where ModuleGuid = @ModuleGuid and SiteGuid = '00000000-0000-0000-0000-000000000001' and ModuleToSiteGuid = @ModuleToSiteGuid) insert map.tblEntityModuleToSite (ModuleToSiteGuid, ModuleGuid, SiteGuid, AssignedFromSiteGuid) values (@ModuleToSiteGuid, @ModuleGuid, '00000000-0000-0000-0000-000000000001', '00000000-0000-0000-0000-000000000001')




----------------------------------------- UPDATA DeviceAlarmMapReference Limit Tags from short to int---------------------------------------------------------------

--UPDATE ptt SET Maximum = 4294967295.0
--FROM tblPointTemplateTag ptt
--where ptt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference'


--UPDATE ptt2 SET ValueType = 'System.UInt32'
--, Value = '<unsignedInt>' + CAST(ptt2.Value.value('unsignedShort[1]', 'int') AS NVARCHAR(8)) + '</unsignedInt>'
--, Maximum = 4294967295.0
--FROM tblPointTemplateTag ptt1
--INNER JOIN tblAlarmTemplate at on at.InputTemplateTagGuid = ptt1.PointTemplateTagGuid
--INNER JOIN tblAlarmTestTemplate att on att.AlarmTemplateGuid = at.AlarmTemplateGuid
--INNER JOIN tblPointTemplateTag ptt2 on ptt2.PointTemplateTagGuid = att.LimitTemplateTagGuid
--where ptt1.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference' AND ptt2.ValueType = 'System.UInt16'

--UPDATE pt SET Maximum = 4294967295.0
--FROM tblPointTag pt
--where pt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference'


--UPDATE pt2 SET ValueType = 'System.UInt32'
--, Value = '<unsignedInt>' + CAST(pt2.Value.value('unsignedShort[1]', 'int') AS NVARCHAR(8)) + '</unsignedInt>'
--, Maximum = 4294967295.0
--FROM tblPointTag pt1
--INNER JOIN tblAlarm a on a.InputTagGuid = pt1.PointTagGuid
--INNER JOIN tblAlarmTest at on at.AlarmGuid = a.AlarmGuid
--INNER JOIN tblPointTag pt2 on pt2.PointTagGuid = at.LimitTagGuid
--where pt1.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference'

----------------------------------------- UPDATE Points to PointTemplateVersion---------------------------------------------------------------

UPDATE dbo.tblPoint SET PointTemplateVersion = (SELECT [Version] FROM dbo.tblPointTemplate pt WHERE pt.PointTemplateGuid = dbo.tblPoint.PointTemplateGuid)

----------------------------------------- UPDATE OpcUaServer SiteGuid DELETING any rows that are not referenced by tblPointTag

DELETE FROM dbo.tblOpcUaServer WHERE OpcUaServerGuid NOT IN (SELECT OpcUaServerGuid FROM tblPointTag WHERE OpcUaServerGuid IS NOT NULL)

UPDATE oas SET SiteGuid = p.SiteGuid FROM dbo.tblOpcUaServer oas
INNER JOIN tblPointTag pt ON pt.OpcUaServerGuid = oas.OpcUaServerGuid
INNER JOIN tblPoint p ON p.PointGuid = pt.PointGuid


----------------------------------------- SPLIT map.tblTrendPenToTrend -----------------------------------------------------------------------

IF EXISTS(SELECT * FROM	INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblTrendPenToTrend')
BEGIN
	INSERT INTO map.tblTrendPenToPointTrend (TrendPenToPointTrendGuid,PointTagGuid,TrendGuid,PenColor,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT tptt.TrendPenToTrendGuid,tptt.PointTagGuid,tptt.TrendGuid,tptt.PenColor,tptt.CreatedDate,tptt.CreatedBy,tptt.UpdatedDate,tptt.UpdatedBy
		FROM map.tblTrendPenToTrend tptt
		INNER JOIN dbo.tblTrend t ON t.TrendGuid = tptt.TrendGuid
		WHERE t.PointTemplateGuid IS NULL

	INSERT INTO map.tblTrendPenToDetailTrend (TrendPenToDetailTrendGuid,PointTemplateTagGuid,TrendGuid,PenColor,CreatedDate,CreatedBy,UpdatedDate,UpdatedBy)
		SELECT tptt.TrendPenToTrendGuid,tptt.PointTemplateTagGuid,tptt.TrendGuid,tptt.PenColor,tptt.CreatedDate,tptt.CreatedBy,tptt.UpdatedDate,tptt.UpdatedBy
		FROM map.tblTrendPenToTrend tptt
		INNER JOIN dbo.tblTrend t ON t.TrendGuid = tptt.TrendGuid
		WHERE t.PointTemplateGuid IS NOT NULL

	DROP TABLE [map].[tblTrendPenToTrend]
	DROP TABLE [FMAudit].[map_tblTrendPenToTrend]
	DROP TABLE [Track].[tblTrendPenToTrend]
END


----------------------------------------- Map Investigation Right -----------------------------------------------------------------------
IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 203))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (203
           ,'MAP_INITIATE_INVESTIGATION'
           ,'MAP_INITIATE_INVESTIGATION'
           ,'369D964E-6011-4530-AC1B-6BC146EAF28B'
           ,N'2018-02-08 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2018-02-08 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 204))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (204
           ,'MAP_COMPLETE_INVESTIGATION'
           ,'MAP_COMPLETE_INVESTIGATION'
           ,'A12FABA3-1BCA-423B-A44A-0A948D59843C'
           ,N'2018-02-08 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2018-02-08 10:30:00 AM -05:00'
           ,'Administrator')
END

----------------------------------------- Active Directory -----------------------------------------------------------------------
--IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'map' AND TABLE_NAME = 'tblSiteToActiveDirectorySiteGroup') > 0)
--BEGIN
--    DROP TABLE map.tblSiteToActiveDirectorySiteGroup
--END

--IF ((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'map' AND TABLE_NAME = 'tblSiteToActiveDirectoryUserGroup') > 0)
--BEGIN
--   DROP TABLE map.tblSiteToActiveDirectoryUserGroup
--END


IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'ActiveDirectoryDomainName' )
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'91a55401-b6ab-4830-8358-4365e1d78a7d', N'SZ', N'ActiveDirectoryDomainName', N'NeedValue', N'8/8/2019 8:8:00 PM -04:00', 'Administrator', N'8/8/2019 8:8:00 PM -04:00', 'Administrator');
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'ActiveDirectorySitesOrganizationalUnitPath' )
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ba71aa2b-ce15-4b79-b31e-4cbbd2671d7b', N'SZ', N'ActiveDirectorySitesOrganizationalUnitPath', N'NeedValue', N'8/8/2019 8:8:00 PM -04:00', 'Administrator', N'8/8/2019 8:8:00 PM -04:00', 'Administrator');
END

IF NOT EXISTS (SELECT 1 FROM tblConfigurationSetting WHERE SettingKey = 'ActiveDirectoryUserGroupsOrganizationalUnitPath' )
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a2561ad0-75f5-4425-833f-e2f78f4bd7b5', N'SZ', N'ActiveDirectoryUserGroupsOrganizationalUnitPath', N'NeedValue', N'8/8/2019 8:8:00 PM -04:00', 'Administrator', N'8/8/2019 8:8:00 PM -04:00', 'Administrator');
END

-- Configuration settigns additions.
IF EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMActiverDirectoryManageSvr_TestModeFlag')
BEGIN
    IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'ActiveDirectoryManageSvr_TestModeFlag')
    BEGIN
        UPDATE dbo.tblConfigurationSetting SET SettingKey = 'ActiveDirectoryManageSvr_TestModeFlag' WHERE SettingKey = 'FMActiverDirectoryManageSvr_TestModeFlag'
    END
END

IF EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMActiverDirectoryManageSvr_TestModeFilePath')
BEGIN
    IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'ActiveDirectoryManageSvr_TestModeFilePath')
    BEGIN
        UPDATE dbo.tblConfigurationSetting SET SettingKey = 'ActiveDirectoryManageSvr_TestModeFilePath' WHERE SettingKey = 'FMActiverDirectoryManageSvr_TestModeFilePath'
    END
END

IF EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'FMActiverDirectoryManageSvr_SleepIntervalTime')
BEGIN
    IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'ActiveDirectoryManageSvr_SleepIntervalTime')
    BEGIN
        UPDATE dbo.tblConfigurationSetting SET SettingKey = 'ActiveDirectoryManageSvr_SleepIntervalTime' WHERE SettingKey = 'FMActiverDirectoryManageSvr_SleepIntervalTime'
    END
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
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4A254D92-406D-4DE9-B489-DA9E51D8794D', N'DWORD', N'ActivrDirectoryManageSvr_SleepIntervalTime', N'', N'8/5/2019 3:57:48 PM -04:00', N'Administrator', N'8/5/2019 3:57:48 PM -04:00', N'Administrator')
END


-- Remove old indexes that were generated from the older CodeSmith templates as well as indexes that don't make any sense.
-- Activation Status isn't even valid for these tables
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND name = N'IX_track_tblActiveDirectoryUserGroup_InsertContext')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectoryUserGroup_InsertContext] ON [track].[tblActiveDirectoryUserGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND name = N'IX_track_tblActiveDirectoryUserGroup_UpdateContext')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectoryUserGroup_UpdateContext] ON [track].[tblActiveDirectoryUserGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND name = N'IX_track_tblActiveDirectoryUserGroup_DeleteContext')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectoryUserGroup_DeleteContext] ON [track].[tblActiveDirectoryUserGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND name = N'IX_tblActiveDirectoryUserGroup_PK_CurrentSiteGuid')
BEGIN
	DROP INDEX [IX_tblActiveDirectoryUserGroup_PK_CurrentSiteGuid] ON [track].[tblActiveDirectoryUserGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND name = N'IX_track_tblActiveDirectoryUserGroup_PK_ParentFK')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectoryUserGroup_PK_ParentFK] ON [track].[tblActiveDirectoryUserGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectoryUserGroup]') AND name = N'IX_track_tblActiveDirectoryUserGroup_PK_ActivationStatusIndex')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectoryUserGroup_PK_ActivationStatusIndex] ON [track].[tblActiveDirectoryUserGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND name = N'IX_track_tblActiveDirectorySiteGroup_InsertContext')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectorySiteGroup_InsertContext] ON [track].[tblActiveDirectorySiteGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND name = N'IX_track_tblActiveDirectorySiteGroup_UpdateContext')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectorySiteGroup_UpdateContext] ON [track].[tblActiveDirectorySiteGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND name = N'IX_track_tblActiveDirectorySiteGroup_DeleteContext')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectorySiteGroup_DeleteContext] ON [track].[tblActiveDirectorySiteGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND name = N'IX_tblActiveDirectorySiteGroup_PK_CurrentSiteGuid')
BEGIN
	DROP INDEX [IX_tblActiveDirectorySiteGroup_PK_CurrentSiteGuid] ON [track].[tblActiveDirectorySiteGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND name = N'IX_track_tblActiveDirectorySiteGroup_PK_ParentFK')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectorySiteGroup_PK_ParentFK] ON [track].[tblActiveDirectorySiteGroup]
END

IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[track].[tblActiveDirectorySiteGroup]') AND name = N'IX_track_tblActiveDirectorySiteGroup_PK_ActivationStatusIndex')
BEGIN
	DROP INDEX [IX_track_tblActiveDirectorySiteGroup_PK_ActivationStatusIndex] ON [track].[tblActiveDirectorySiteGroup]
END

-- Remove foreign key constraints for referenced entities from transaction related tables since the entities may no longer be associated with the site and this would prevent an
-- initial sync from completing.
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_BillToCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_BillToCompanyGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_CarrierCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_CarrierCompanyGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_Destination1EquipmentGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_Destination1EquipmentGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_Destination2EquipmentGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_Destination2EquipmentGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_Destination3EquipmentGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_Destination3EquipmentGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_FinalStationIATAGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_FinalStationIATAGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_FuelCardGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_FuelCardGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_GateGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_GateGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_ManagerCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_ManagerCompanyGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_NextStationIATAGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_NextStationIATAGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_OperatorPersonnelGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_OperatorPersonnelGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_OriginStationIATAGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_OriginStationIATAGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_OwnerCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_OwnerCompanyGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_PreviousStationIATAGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_PreviousStationIATAGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_ReasonCodeGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_ReasonCodeGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_ShipperCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_ShipperCompanyGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_ShipToCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_ShipToCompanyGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_Source1EquipmentGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_Source1EquipmentGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_Source2EquipmentGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_Source2EquipmentGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_Source3EquipmentGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_Source3EquipmentGuid]
GO

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[FK_tblTransactions_SupplierCompanyGuid]') AND parent_object_id = OBJECT_ID(N'[dbo].[tblTransactions]'))
	ALTER TABLE [dbo].[tblTransactions] DROP CONSTRAINT [FK_tblTransactions_SupplierCompanyGuid]
GO

-- remove all non clustered indexes from the tracking tables if still using the old indexes since the dacpac deployment does not drop them
DECLARE @sql NVARCHAR(MAX) = N'';

SELECT @sql += N'DROP INDEX ' 
	+ QUOTENAME(SCHEMA_NAME(o.[schema_id]))
	+ '.' + QUOTENAME(o.name) 
	+ '.' + QUOTENAME(i.name) + ';' + char(13)
	FROM sys.indexes AS i
	INNER JOIN sys.tables AS o
	ON i.[object_id] = o.[object_id]
WHERE i.type = 2 --non clustered indexes
AND is_primary_key = 0
AND i.index_id <> 0
AND o.is_ms_shipped = 0
AND SCHEMA_NAME(o.[schema_id]) = 'track'
AND QUOTENAME(i.name) LIKE '%_%Context]'

EXEC sp_executesql @sql;
GO

--------------------------------------------------------- Set Default MaximumDaysToRetainArchive --------------------------------------------
UPDATE dbo.tblSites SET MaximumDaysToRetainArchive = 365 WHERE MaximumDaysToRetainArchive IS NULL

--------------------------------------------------------- Apply Standard Tank Archived setting to derived points ---------------------------
UPDATE pt SET pt.Archived = ptt.Archived FROM dbo.tblPointTag pt
INNER JOIN dbo.tblPointTemplateTag ptt ON ptt.PointTemplateTagGuid = pt.PointTemplateTagGuid
INNER JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid
WHERE p.PointTemplateGuid = '0ADB4947-1CC4-4A44-91F8-E76F281EA718'

--------------------------------------------------------- INSERT Standard Tank Density Product Gaged to derived points ----------------------
INSERT INTO tblPointTag ([ID],[EngineeringUnitsType],[EngineeringUnitsIndex],[DecimalPlaces],[ServerEngineeringUnitsIndex],[ValueType],
	[Status],[Value],[Maximum],[Minimum],[PointTagInputOutputTypeIndex],[Input],[AlarmStatus],[ApplyPointEngineeringUnits],
	[ApplyPointDecimalPlaces],[ApplyPointMaximum],[ApplyPointMinimum],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],
	[PointTagGuid],p.[PointGuid],[PointTemplateTagGuid],[AlarmsEnabled],[InhibitInputOutputTypeConfiguration],[InhibitOverride],[Archived])
SELECT ptt.ID, ptt.EngineeringUnitsType, ptt.EngineeringUnitsIndex, ptt.DecimalPlaces, ptt.ServerEngineeringUnitsIndex, ptt.ValueType,
	0, ptt.Value, ptt.Maximum, ptt.Minimum, ptt.PointTagInputOutputTypeIndex, ptt.Input, ptt.AlarmStatus,ptt.ApplyPointTemplateEngineeringUnits,
	ptt.ApplyPointTemplateDecimalPlaces,ptt.ApplyPointTemplateMaximum,ptt.ApplyPointTemplateMinimum,
	ptt.UpdatedDate,ptt.UpdatedBy,ptt.UpdatedDate,ptt.UpdatedBy,
	NEWID(),p.PointGuid, ptt.PointTemplateTagGuid,ptt.AlarmsEnabled,ptt.InhibitInputOutputTypeConfiguration,ptt.InhibitOverride,ptt.Archived
	FROM dbo.tblPointTemplateTag ptt
	INNER JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid
	INNER JOIN dbo.tblPoint p ON p.PointTemplateGuid = pt.PointTemplateGuid
	WHERE pt.PointTemplateGuid = '0ADB4947-1CC4-4A44-91F8-E76F281EA718'
	AND ptt.ID = 'Density Product Gauge'
	AND p.PointGuid NOT IN (SELECT PointGuid FROM tblPointTag WHERE ID = 'Density Product Gauge')

--=================================================================
-- New rights for movement summary
--=================================================================
IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 344))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (344
           ,'OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY'
           ,'OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY'
           ,'8E654E80-5071-43D5-99AE-4499DADA9E51'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 345))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (345
           ,'OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY'
           ,'OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY'
           ,'73E93ED8-FAD3-4ABE-BFCB-FFEA6E43476B'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 346))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (346
           ,'OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY'
           ,'OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY'
           ,'7C64CF41-EAB2-4197-B1BB-D4DC9D81A3DD'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 347))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (347
           ,'OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY'
           ,'OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY'
           ,'254A2390-213F-4283-9ED2-22F99481F141'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 348))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (348
           ,'OPERATE_MODIFY_MOVEMENT_SUMMARY'
           ,'OPERATE_MODIFY_MOVEMENT_SUMMARY'
           ,'DE4EBA97-13CC-4750-B682-3FC3BE254970'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 349))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (349
           ,'OPERATE_VIEW_MOVEMENT_SUMMARY'
           ,'OPERATE_VIEW_MOVEMENT_SUMMARY'
           ,'164465E4-077B-4D21-8C53-D70DC47AFAC3'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator'
           ,N'2022-08-21 10:30:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 361))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (361
           ,'MOBILE_LAUNCH'
           ,'MOBILE_LAUNCH'
           ,'5c6b4fb6-ad7d-11ed-ab97-e8f4082b401b'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 362))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (362
           ,'MOBILE_VIEW_CONFIGURATION'
           ,'MOBILE_VIEW_CONFIGURATION'
           ,'5c6b4fb7-ad7d-11ed-ab97-e8f4082b401b'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 363))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (363
           ,'MOBILE_MODIFY_CONFIGURATION '
           ,'MOBILE_MODIFY_CONFIGURATION '
           ,'5c6b4fb8-ad7d-11ed-ab97-e8f4082b401b'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 364))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (364
           ,'MOBILE_ROOT_MENU_DISPLAY '
           ,'MOBILE_ROOT_MENU_DISPLAY '
           ,'5c6b4fb9-ad7d-11ed-ab97-e8f4082b401b'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator'
           ,N'2023-02-15 5:30:00 PM -05:00'
           ,'Administrator')
END


-- Rename right OPERATE_VIEW_LEAK_DETECTION  to OPERATE_PERFORM_LEAK_DETECTION
UPDATE [lookup].[tblRight]
SET [RightCode] = 'OPERATE_PERFORM_LEAK_DETECTION',
	[RightName] = 'OPERATE_PERFORM_LEAK_DETECTION'
WHERE RightIndex = 365 AND [RightCode] = 'OPERATE_VIEW_LEAK_DETECTION'

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 365))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
		(365
		, 'OPERATE_PERFORM_LEAK_DETECTION'
		, 'OPERATE_PERFORM_LEAK_DETECTION'
		, '0972CA16-BC78-47CD-A4EF-C31B37B3693F'
		, N'2023-04-14 5:30:00 PM -05:00'
		, 'Administrator'
		, N'2023-04-14 5:30:00 PM -05:00'
		, 'Administrator')
END

-- Add right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 365)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2B296E19-358E-4618-A15D-8218A903E0CA', N'00000000-0000-0000-0000-000000000003', 365, N'2023-04-14 5:30:00 PM -05:00', 'Varec', N'2023-04-14 5:30:00 PM -05:00', N'Varec') 
END


------------------------------- REMOVE ALARM AND EVENT CONFIGURATION FOR ENALBED AND CATETEGORY AND PRIORITPY NULL ----------------------------------------------

DELETE FROM [dbo].[tblAlarmAndEvents] WHERE Enabled = CAST(1 AS BIT) AND CategoryGuid IS NULL AND PriorityGuid IS NULL


IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'ValidateDestinationEquipment') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('1CAD22C8-D70D-4F3F-A387-FAF98DA9C9E3', 'SZ', 'ValidateDestinationEquipment', N'true', '2023-04-14','Administrator','2023-04-14','Administrator')
END

--===================================================================
-- Add Movement History rights.
--===================================================================
IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 366))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (366
           ,'OPERATE_MODIFY_MOVEMENT_HISTORY'
           ,'OPERATE_MODIFY_MOVEMENT_HISTORY'
           ,'342A67C9-2661-4C9F-ADA8-2BB6E0E15CAD'
           ,N'2023-04-17 10:20:00 AM -05:00'
           ,'Administrator'
           ,N'2023-04-17 10:20:00 AM -05:00'
           ,'Administrator')
END

IF (NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 367))
BEGIN
	INSERT INTO [lookup].[tblRight]
           ([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           (367
           ,'OPERATE_VIEW_MOVEMENT_HISTORY'
           ,'OPERATE_VIEW_MOVEMENT_HISTORY'
           ,'7A9D4FDF-AF46-4CFD-A23B-3643B4FC6085'
           ,N'2023-04-17 10:20:00 AM -05:00'
           ,'Administrator'
           ,N'2023-04-17 10:20:00 AM -05:00'
           ,'Administrator')
END


IF (NOT EXISTS (SELECT * FROM [dbo].[tblGaugeType]))
BEGIN
	INSERT INTO [dbo].[tblGaugeType]
	(
		[GaugeTypeGuid],
		[GaugeTypeIndex],
		[ID],
		[Name], 
		[Type],
		[DeltaTemp], 
		[Threshold], 
		[CertificationLeakRate], 
		[MinHours],
		[CreatedDate],
		[CreatedBy],
		[UpdatedDate],
		[UpdatedBy]  
	)
	VALUES
		('C2FB7F72-52EE-4190-84B8-B215ED6B564D',0,'Generic','Generic',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('96F54DE4-B672-426A-98D1-A5E8F5C6305B',1,'Undefined','Undefined',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('945D9EFE-6EDF-41C6-9BCE-AAF530839574',2,'_4_20mA_Device','4-20mA Device',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('672BB0EC-01AA-40C3-B721-71DA49C4F8BB',3,'Barton_Model_3500_Type1','Barton Model 3500 (Type1)',2,6.9,0.1,0.2,24,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('554212EF-17E9-4EB9-91F9-6B4400E0FE27',4,'Barton_Model_3500_Type2','Barton Model 3500 (Type2)',2,1.5,1,2,48,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('516FC494-501D-47E5-93AB-92AFB254610D',5,'Barton_Model_3500_Type3','Barton Model 3500 (Type3)',2,1,1.5,3,48,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('1E944071-3AFD-433B-96C4-CFB008159D3E',6,'EH_NMS8x','E+H NMS8x',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('1CA1BD88-9FA6-4E99-A455-7FDAE238E8E1',7,'Enraf_954','Enraf 954',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('6B348E2B-4B28-4842-BBE4-6126866EE1D9',8,'Enraf_Model_811','Enraf Model 811',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('AFE2FF0C-E67E-446F-A710-DB7E1C159C80',9,'Enraf_Model_818','Enraf Model 818',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('B4B1AF75-F23C-4457-82A3-4E02DDA24076',10,'Enraf_Model_854','Enraf Model 854',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('C97684F3-B975-43B0-897C-E838CA67AFBF',11,'Enraf_Model_873','Enraf Model 873',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('5E99C92A-F3A7-43B1-8958-38BF1778F70A',12,'GPE_Gauge','GPE Gauge',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('3019E03C-AB8B-4680-8568-465B58241978',13,'GSI_1901','GSI 1901',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('B9D2E9A7-61E5-4F45-BCD4-2E4F0BDFB4D1',14,'GSI_Model_2000','GSI Model 2000',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('67A8DE93-A50D-45B6-9328-6B8F41A19BB9',15,'HART_Device','HART Device',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('28E19C72-266D-43BE-9587-3CBB83F24E1E',16,'LJ_Model_8100','L&J Model 8100',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('29EB73B3-6F17-45D2-96D4-BFD51737DFE4',17,'LJ_Model_MCG1000','L&J Model MCG1000',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('8F4E0AB4-A63B-48EF-981A-6C8495D37D40',18,'LJ_Model_MCG1200','L&J Model MCG1200',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('2911C421-496D-4CC7-9732-96A948EEFFEA',19,'LJ_Model_MCG1500','L&J Model MCG1500',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('548AD92E-D63A-49E7-83FC-357EAC1A8E97',20,'LJ_Model_MCG2000','L&J Model MCG2000',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('9A931AC3-C9B8-45CD-A975-3CB3CB12F3BB',21,'MTS_DDA_Compatible_Type1','MTS DDA Compatible (Type1)',2,3.6,0.1,0.2,24,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('A7DC30E5-41B8-46A6-BB7F-237ED3F5FA22',22,'MTS_DDA_Compatible_Type2','MTS DDA Compatible (Type2)',2,0.1,0.1,0.2,24,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('7019897F-E0C0-40A9-9840-C1074ECA4343',23,'SAAB_L_2_Radar','Saab L/2 Radar',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('B16C60BF-A696-4246-BFFE-AB06B71CC133',24,'Saab_Rex_3920_Radar','Saab Rex 3920 Radar',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('F8678364-DEB9-4DEC-AD4A-64348331A4EB',25,'Saab_Rex_3945_Radar','Saab Rex 3945 Radar',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('D1B25E24-0555-4C22-B25F-2803DC446B71',26,'Sakura_Proservo_LT51_5600','Sakura Proservo LT51/5600',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('4332624F-8CD3-45F7-B2C1-FF5D72FDF64F',27,'Sakura_Proservo_NMS530','Sakura Proservo NMS530',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('F149D778-9B10-4609-BDD7-3431D67D189F',28,'Sakura_TGM4000','Sakura TGM4000',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('0CF5748F-CBEB-4328-9045-6C10E9AD69E1',29,'TI_Model_111A','TI Model 111A',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('5B3764CC-923D-4A2C-A40C-8E68161E5DE2',30,'TI_Model_121','TI Model 121',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('91D173AA-004D-4AC9-98C6-6B7C6B57A05B',31,'TI_Model_150','TI Model 150',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('AC6BC037-B655-4ECF-8F42-544102A4A623',32,'Varec_7501_HTG','Varec 7501 HTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('EA9AE381-6C46-4E8B-88A1-B968DE0388AF',33,'Varec_ATT','Varec ATT',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('6F7C234A-E6A9-4E5C-A6BB-0137D84E84E4',34,'Varec_HIU','Varec HIU',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('FE0CAAF4-96CD-4ED9-8C3B-1B172D57DC2D',35,'Varec_MFT','Varec MFT',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('332C27AB-1D2F-4A43-B23A-CE8CB500F29B',36,'Varec_Model_1651','Varec Model 1651',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('ADCE8A64-B022-4887-B964-CDC03CECFD69',37,'Varec_Model_1671','Varec Model 1671',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('20A2BF50-4EE8-45C0-8128-095CA8170AB5',38,'Varec_Model_1800','Varec Model 1800',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('DD3FCC11-1BAB-41FA-818E-C811F77F624C',39,'Varec_Model_1900','Varec Model 1900',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('6763ECAB-69F9-428B-BAB0-01E390F5FD30',40,'Varec_Model_4560_SM','Varec Model 4560 SM',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('D4EF28F3-C01A-4A95-890A-25EFC128782E',41,'Varec_Model_4590_TSM','Varec Model 4590 TSM',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('F1EDF59E-9463-4642-BAE3-942A3526DFA0',42,'Varec_Model_5000_MTG','Varec Model 5000 MTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('0948FF6A-5DCC-456A-8FD5-55B1E5143D5C',43,'Varec_Model_6000_STG','Varec Model 6000 STG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('5DE761EC-9E0B-4057-9C57-72D86CD7D3ED',44,'Varec_Model_6005_STG','Varec Model 6005 STG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('876CA53F-105D-4799-B691-5B6B20040B60',45,'Varec_Model_6500','Varec Model 6500',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('D70A2BED-E4F3-41E8-AC8F-ECA47A588F92',46,'Varec_Model_6603','Varec Model 6603',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('F254B75A-9516-4401-B24E-1C98AC1FC693',47,'Varec_Model_7230_RTG','Varec Model 7230 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('B824C94B-D8CD-4875-99F0-B0A5FB4D384F',48,'Varec_Model_7231_RTG','Varec Model 7231 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('10E93295-637A-44DF-B4EE-075BE809E6A3',49,'Varec_Model_7240_RTG','Varec Model 7240 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('1B82CBF3-80FA-44E7-9CAD-6AFDACAFD496',50,'Varec_Model_7244_RTG','Varec Model 7244 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('E64D27BE-A771-4F3B-8A79-DB5A4AED2AB8',51,'Varec_Model_7245_RTG','Varec Model 7245 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('55AA67D5-4348-48FA-A90A-E78BF5CD7559',52,'Varec_Model_7500_HTG','Varec Model 7500 HTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('45E7B0DA-1D57-4373-8DDF-CC008AE87EE6',53,'Varec_Model_7530_RTG','Varec Model 7530 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('6AE63E0C-AEAC-41D4-8BED-537F79EFC05C',54,'Varec_Model_7531_RTG','Varec Model 7531 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('0B3CFACF-1FE3-4506-8BA6-711990B33D0D',55,'Varec_Model_7532_RTG','Varec Model 7532 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('9C0E6BE2-5935-4E27-8172-61921ACC3CD2',56,'Varec_Model_7533_RTG','Varec Model 7533 RTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('325C2438-FDBE-4190-AE34-3E8354EFFE38',57,'Varec_Model_7600_HTG','Varec Model 7600 HTG',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('DADEB89E-3DE7-4761-9480-5A02B3CE45AE',58,'Varec_MSP_1002','Varec MSP 1002',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('78DF2060-9CBB-488B-BDA3-EB3E1AB47D16',59,'Whessoe_1315','Whessoe 1315',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('67B191D8-CB91-44A9-BAB0-25541C68366A',60,'Whessoe_ITG_50','Whessoe ITG 50',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('622050C3-DCD0-4A18-9CC5-D9D87C30DEB3',61,'Whessoe_ITG_60','Whessoe ITG 60',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('C30C746F-106F-4A5E-B631-00E12147A4D1',62,'Whessoe_ITG_70','Whessoe ITG 70',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('B532B7B1-4941-4C92-87E4-1073D3D1C894',63,'Whessoe_Model_1323','Whessoe Model 1323',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('92A9FA23-749D-4380-A182-5132B4DF2D9C',64,'Whessoe_Model_2046','Whessoe Model 2046',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('E07F6188-D640-42A4-8F7F-EBFBE2C8F4F2',65,'Whessoe_Model_2047','Whessoe Model 2047',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator'),
		('079BBF9F-CE12-45E4-92FC-940F036EA8CA',66,'Whessoe_Model_770','Whessoe Model 770',1,NULL ,NULL,NULL,NULL,'2023-05-08','Administrator','2023-05-08','Administrator')

END

-- Fix error in inital data upload, can be removed after first deployment fixes test servers as it is fixed in script above as well.
IF (EXISTS (SELECT * FROM [dbo].[tblGaugeType] WHERE [ID] = 'MTS_DDA_Compatible_Type2' AND [UpdatedDate] = '2023-05-08' 
			AND [Threshold] = 1.5 AND [CertificationLeakRate] = 3 AND [MinHours] =48))
BEGIN
	UPDATE [dbo].[tblGaugeType]
	SET  [Threshold] = 0.1, [CertificationLeakRate] = 0.2 , [MinHours] = 24
	WHERE [ID] = 'MTS_DDA_Compatible_Type2' AND [UpdatedDate] = '2023-05-08'
END

------------------------------------------------- ADD THE WELL KNOWN GUID FOR THE LEAK RATE can be removed after first deployment fixes test servers ---------------
UPDATE [dbo].[tblPointTemplateTag] SET WellKnownIdentityGuid = '91F98328-AF76-4229-B701-4600B2B645EE' WHERE ID = 'Leak Rate'

------------------------------------------------- REVISE the K Polynomial double array for compatibility with DataContractSerializer -------------------------------
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(REPLACE(REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<K>','<K xmlns:a="http://schemas.microsoft.com/2003/10/Serialization/Arrays">'),'<double','<a:double'),'</double','</a:double')) WHERE ID = 'Volume Correction'

------------------------------------------------- REVISE Tank Transfer Modes ---------------------------------------------------------------------------------------

UPDATE dbo.tblPointTemplateTag SET ValueType = 'FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode', Value = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),Value),'TankTransferModes','TankTransferMode'))
 WHERE ID = 'Transfer Mode' AND PointTemplateGuid IN (SELECT PointTemplateGuid FROM map.tblModuleToPointTemplate WHERE ModuleGuid IN (SELECT ModuleGuid FROM dbo.tblModule WHERE ModuleTypeName = 'TankTransfer.FMTankTransfer'))

UPDATE dbo.tblPointTag SET ValueType = 'FMBusinessObjects.DataObjects.CodedVariables.TankTransferMode', Value = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),Value),'TankTransferModes','TankTransferMode')) 
 WHERE ID = 'Transfer Mode' AND PointTemplateTagGuid IN (SELECT PointTemplateTagGuid FROM dbo.tblPointTemplateTag WHERE PointTemplateGuid IN (SELECT PointTemplateGuid FROM map.tblModuleToPointTemplate WHERE ModuleGuid IN (SELECT ModuleGuid FROM dbo.tblModule WHERE ModuleTypeName = 'TankTransfer.FMTankTransfer')))

------------------------------------------------- REVISE Tank Transfer Statuses ---------------------------------------------------------------------------------------

UPDATE dbo.tblPointTemplateTag SET ValueType = 'FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses', Value = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),Value),'TankTransferStatuses','TransferStatuses'))
 WHERE ID = 'Transfer Status' AND PointTemplateGuid IN (SELECT PointTemplateGuid FROM map.tblModuleToPointTemplate WHERE ModuleGuid IN (SELECT ModuleGuid FROM dbo.tblModule WHERE ModuleTypeName = 'TankTransfer.FMTankTransfer'))

UPDATE dbo.tblPointTag SET ValueType = 'FMBusinessObjects.DataObjects.CodedVariables.TransferStatuses', Value = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),Value),'TankTransferStatuses','TransferStatuses')) 
 WHERE ID = 'Transfer Status' AND PointTemplateTagGuid IN (SELECT PointTemplateTagGuid FROM dbo.tblPointTemplateTag WHERE PointTemplateGuid IN (SELECT PointTemplateGuid FROM map.tblModuleToPointTemplate WHERE ModuleGuid IN (SELECT ModuleGuid FROM dbo.tblModule WHERE ModuleTypeName = 'TankTransfer.FMTankTransfer')))




------------------------------------------------- REVISE the System Types Used as system types available for modules settings for compatibility with DataContractSerializer-----------

UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<double>','<double xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.Double'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<float>','<float xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.Single'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<short>','<short xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.Int16'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<unsignedShort>','<unsignedShort xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.UInt16'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<int>','<int xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.Int32'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<unsignedInt>','<unsignedInt xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.UInt32'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<boolean>','<boolean xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.Boolean'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<dateTime>','<dateTime xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.DateTime'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<DateTimeOffset>','<DateTimeOffset xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.DateTimeOffset'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<TimeSpan>','<TimeSpan xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.TimeSpan'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<string>','<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/">')) WHERE ValueType = 'System.String'
UPDATE tblPointProperty SET VALUE = CONVERT(XML,REPLACE(CONVERT(NVARCHAR(MAX),VALUE),'<string />','<string xmlns="http://schemas.microsoft.com/2003/10/Serialization/"> /')) WHERE ValueType = 'System.String'

if not exists (Select SettingKey from dbo.tblConfigurationSetting where SettingKey = N'DatabaseUpdated')
	BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) 
	VALUES ('49348563-DE10-4DD5-A2DE-330197F544D7', N'DWORD', N'DatabaseUpdated', N'1', '2023-05-23','Administrator','2023-05-23','Administrator')
	END

-- Copy WellKnownIdentityGuid to PointTemplateTag's that have been copied and not had it set correctly
IF EXISTS (SELECT * from tblPointTemplateTag where  ID IN ( 'Volume Net Standard Unrounded','Level Product', 'Transfer Start Volume Water') AND (WellKnownIdentityGuid IS NULL OR WellKnownIdentityGuid = '00000000-0000-0000-0000-000000000000'))
BEGIN
	UPDATE tag
	SET tag.WellKnownIdentityGuid = stdWellKnownGuid
	FROM tblPointTemplateTag tag
	JOIN (
		SELECT stdWellKnownGuid, customWellKnownGuid, stdTagID, customTagID, PointTemplateTagGuid
		FROM 
		(
			SELECT DISTINCT WellKnownIdentityGuid AS stdWellKnownGuid, id AS stdTagID 
			FROM tblPointTemplateTag 
			WHERE WellKnownIdentityGuid IS NOT NULL AND WellKnownIdentityGuid <> '00000000-0000-0000-0000-000000000000'
			AND ID <> 'Transfer Target' -- Transfer Target' currently is in 2 modules with different WellKnownIdentityGuid
		) AS standard
		JOIN
		(
			SELECT WellKnownIdentityGuid AS customWellKnownGuid, id AS customTagID, PointTemplateTagGuid
			FROM tblPointTemplateTag 
			WHERE WellKnownIdentityGuid IS NULL OR WellKnownIdentityGuid = '00000000-0000-0000-0000-000000000000'
		) AS custom
		ON standard.stdTagID = custom.customTagID
		WHERE stdWellKnownGuid <> customWellKnownGuid OR customWellKnownGuid IS NULL
	) AS needsUpdate
	ON tag.PointTemplateTagGuid = needsUpdate.PointTemplateTagGuid
END

-- Add OPERATE_CREATE_PUBLIC_MOVEMENT_SUMMARY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 344)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4E2176A6-57E4-4E03-A819-4A0D503C0950', N'00000000-0000-0000-0000-000000000003', 344, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_CREATE_SHARED_MOVEMENT_SUMMARY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 345)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'606B3B9E-38A1-4B37-88B0-88D180AB6183', N'00000000-0000-0000-0000-000000000003', 345, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_MODIFY_PUBLIC_MOVEMENT_SUMMARY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 346)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'EEA5A679-9EFA-487E-9D8B-D9AA5AD1AF31', N'00000000-0000-0000-0000-000000000003', 346, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_MODIFY_SHARED_MOVEMENT_SUMMARY  right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 347)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'797A3D1D-67C9-4B6D-912A-E8C970A9ABF1', N'00000000-0000-0000-0000-000000000003', 347, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_MODIFY_MOVEMENT_SUMMARY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 348)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3C7E57C8-CBB9-4E77-829F-9F9163769991', N'00000000-0000-0000-0000-000000000003', 348, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_VIEW_MOVEMENT_SUMMARY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 349)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'C706E9A3-1C51-4C89-9F8B-A88CDA6850BD', N'00000000-0000-0000-0000-000000000003', 349, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_MODIFY_MOVEMENT_HISTORY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 366)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'119FAC93-A600-4DA9-9CB7-EBBDD86AAA31', N'00000000-0000-0000-0000-000000000003', 366, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add OPERATE_VIEW_MOVEMENT_HISTORY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 367)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1109990B-7512-4E76-B731-6ACB567BDE1A', N'00000000-0000-0000-0000-000000000003', 367, N'2023-06-29 5:30:00 PM -05:00', 'Varec', N'2023-06-29 5:30:00 PM -05:00', N'Varec') 
END

-- Add VIEW_OPERATE_STATISTICS right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 371)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'46EB9CF8-AFD6-494D-9988-250974A4C0CC', N'00000000-0000-0000-0000-000000000003', 371, N'2024-07-03 9:30:00 AM -04:00', 'Varec', N'2024-07-03 9:30:00 AM -04:00', N'Varec') 
END



-------------------------------------------------------------------------------------------------------------------
--   V-222411 : The application must automatically disable accounts after a 35 day period of account inactivity. --
-------------------------------------------------------------------------------------------------------------------
BEGIN TRANSACTION

	DECLARE @disablePeriod int = 35
	DECLARE @CurrentDate Datetimeoffset = SYSDATETIMEOFFSET()
	DECLARE @UpdatedBy NVARCHAR(100) = 'Administrator'

	UPDATE [dbo].[tblSites] SET InactivityDisablePeriod=@disablePeriod, UpdatedDate=@CurrentDate, UpdatedBy=@UpdatedBy WHERE InactivityDisablePeriod<>@disablePeriod

	UPDATE u SET InactivityLockout=1, InactivityLockoutDate=SYSDATETIMEOFFSET(), PasswordLockoutCount=0,  UpdatedBy=@UpdatedBy, UpdatedDate=@CurrentDate
		FROM [dbo].[tblUsers] u JOIN dbo.tblSites s ON s.SiteGuid=u.SiteGuid
		WHERE DATEDIFF(day, LastLoginDate, @CurrentDate) >= s.InactivityDisablePeriod
		AND u.UserGuid <> CONVERT(UniqueIdentifier, '00000000-0000-0000-0000-000000000002')
		AND InactivityLockoutDate IS NULL
	
COMMIT TRANSACTION


-- Rename Administrators group for local site to avoid confusion from group asignd from SiteAdmin
UPDATE  A 
SET GroupID = 'Local Administrators', 
	GroupDescription = 'Local System Administrators'
FROM tblGroups A
WHERE (GroupID =  'Administrators' AND NOT EXISTS (SELECT * FROM tblGroups B WHERE A.SiteGuid = B.SiteGuid AND B.GroupID =  'Local Administrators'))
AND SiteGuid <> '00000000-0000-0000-0000-000000000001' 


IF ((SELECT COUNT(SettingKey) FROM tblConfigurationSetting WHERE ConfigurationSettingGuid = '3C1F0859-EDF6-4DF1-9FC1-A4A8D94180CD') = 0)
BEGIN
	-- insert the ConfigurationSetting to track last exported transaction by the TransactionExportService
	INSERT INTO [dbo].[tblConfigurationSetting]
			   ([ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		 VALUES ('3C1F0859-EDF6-4DF1-9FC1-A4A8D94180CD'
			   ,'DWORD'
			   ,'TransactionExportServiceLastRowVersion'
			   ,null -- Adjust to last string value like '0x0000000003D4F4BD' if left as null all Transactions will be exported on first run
			   ,GETDATE()
			   ,'Administrator'
			   ,GETDATE()
			   ,'Administrator')
END


-- Delete SP1 PointTemplateProperties that are no longer applicable
DELETE FROM tblPointTemplateProperty WHERE PointTemplatePropertyGuid IN ('5D373DC8-5277-4423-BD28-DF3DA8CF0384','C0FC2A35-A191-4272-9641-5C3FC960A451','0E5E64C9-BC89-49B5-9C68-C3F4EADEF05F')


-- Trigger a fix for product _VcfModuleSettings to change in serilizer from 
-- CachingXmlSerializerFactory to DataContractSerializer
-- Fix is run from FMBusinessServices Global.asax
-- Can be removed after FM12 SP3 as it will be fixed on first start after upgrade to SP3
IF ((SELECT COUNT(*) FROM tblProducts WHERE CAST(VcfModuleSettings as nvarchar(max)) like '%<double>%') > 0)
BEGIN
	UPDATE [dbo].[tblConfigurationSetting]
	SET [SettingValue] = 1
	WHERE ConfigurationSettingGuid = '49348563-DE10-4DD5-A2DE-330197F544D7'
END


IF ((SELECT COUNT(SettingKey) FROM tblConfigurationSetting WHERE ConfigurationSettingGuid = 'C24CAB58-4248-4BAC-B8D1-238AF41D13D2') = 0)
BEGIN
	-- insert the ConfigurationSetting to set Max Concurrent Users Per Server
	INSERT INTO [dbo].[tblConfigurationSetting]
			   ([ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		 VALUES ('C24CAB58-4248-4BAC-B8D1-238AF41D13D2'
			   ,'DWORD'
			   ,'MaxConcurrentUsersPerServer'
			   ,20
			   ,GETDATE()
			   ,'Administrator'
			   ,GETDATE()
			   ,'Administrator')
END

IF ((SELECT COUNT(SettingKey) FROM tblConfigurationSetting WHERE ConfigurationSettingGuid = 'D3D3A231-7BCE-441D-9A39-E0BA8529206B') = 0)
BEGIN
	-- insert the ConfigurationSetting to for timeout check of Concurrent Users Per Server to see if sessions are active. 
	INSERT INTO [dbo].[tblConfigurationSetting]
			   ([ConfigurationSettingGuid]
			   ,[KeyType]
			   ,[SettingKey]
			   ,[SettingValue]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
		 VALUES ('D3D3A231-7BCE-441D-9A39-E0BA8529206B'
			   ,'DWORD'
			   ,'ConcurrentUsersTimeOut'
			   ,10
			   ,GETDATE()
			   ,'Administrator'
			   ,GETDATE()
			   ,'Administrator')
END


UPDATE [dbo].[tblGaugeType]
SET	CertificationLeakRate = 0.2,
	DeltaTemp = 7.49,
	MinHours = 24,
	Threshold = 0.1
WHERE ID = 'Enraf_Model_854'
AND CertificationLeakRate IS NULL
AND  DeltaTemp IS NULL
AND  MinHours IS NULL
AND  Threshold IS NULL

IF ((SELECT COUNT(ID) FROM tblGaugeType WHERE ID = 'Enraf_Model_854_85D') = 0)
BEGIN
	INSERT INTO [dbo].[tblGaugeType]
           ([GaugeTypeGuid]
           ,[GaugeTypeIndex]
           ,[ID]
           ,[Name]
           ,[Type]
           ,[DeltaTemp]
           ,[Threshold]
           ,[CertificationLeakRate]
           ,[MinHours]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
     VALUES
           ('B223C7CC-27D9-4EE3-820F-2C4B681003D4'
           ,67
           ,'Enraf_Model_854_85D'
           ,'Enraf Model 854 (85D)'
           ,1
           ,2.9
           ,0.71
           ,1.42
           ,72
           ,GETDATE()
           ,'Administrator'
           ,GETDATE()
           ,'Administrator')
END


IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'LicenseExpiryFullScreenEnabled') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('ECBA01B2-8BCA-41D1-A92E-C4DF98461AD8', 'SZ', 'LicenseExpiryFullScreenEnabled', N'true', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 370)
BEGIN
	INSERT INTO [lookup].[tblRight] 
			([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	VALUES 
			(370
			,'CONFIGURE_NOTIFY_ALARMS_ON_POINTS'
			,'CONFIGURE_NOTIFY_ALARMS_ON_POINTS'
			,'70CEC986-E720-4DB9-8BDE-0725837F5B6D'
			,N'2024-06-17 10:20:00 AM -05:00'
			,'Administrator'
			,N'2024-06-17 10:20:00 AM -05:00'
			,'Administrator')
END

IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = 372)
BEGIN
	INSERT INTO [lookup].[tblRight] 
			([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	VALUES 
			(372
			,'OPERATE_VIEW_UNPUBLISHED'
			,'OPERATE_VIEW_UNPUBLISHED'
			,'AC3921AF-FD5B-402F-BDCF-08A104D2E882'
			,N'2024-08-19 10:20:00 AM -05:00'
			,'Administrator'
			,N'2024-08-19 10:20:00 AM -05:00'
			,'Administrator')
END

-- Add OPERATE_VIEW_UNPUBLISHED right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 372)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'AC3921AF-FD5B-402F-BDCF-08A104D2E882', N'00000000-0000-0000-0000-000000000003', 372, N'2024-08-19 9:30:00 AM -04:00', 'Varec', N'2024-08-19 9:30:00 AM -04:00', N'Varec') 
END

-- Add OPERATE_VIEW_POINT_HISTORY right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = 373)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3A9C9413-C411-4029-A399-83F8DBBCCA48', N'00000000-0000-0000-0000-000000000003', 373, N'2024-08-19 9:30:00 AM -04:00', 'Varec', N'2024-08-19 9:30:00 AM -04:00', N'Varec') 
END


--Modify Site Closeout Time
DECLARE @RightIndex INT = 374
IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = @RightIndex)
BEGIN
	INSERT INTO [lookup].[tblRight] 
			([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	VALUES 
			(@RightIndex
			,'MODIFY_SITE_CLOSEOUT_TIME'
			,'MODIFY_SITE_CLOSEOUT_TIME'
			,'5CF5E97D-769C-4295-B202-03460403A737'
			,N'2024-12-16 10:20:00 AM -05:00'
			,'Administrator'
			,N'2024-12-16 10:20:00 AM -05:00'
			,'Administrator')
END

-- Add MODIFY_SITE_CLOSEOUT_TIME right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = @RightIndex)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'171978A9-D2CC-4E02-8925-2FE49810160A', N'00000000-0000-0000-0000-000000000003', @RightIndex, N'2024-12-15 9:30:00 AM -04:00', 'Varec', N'2024-12-15 9:30:00 AM -04:00', N'Varec') 
END

--View Only Site Closeout Time
SET @RightIndex  = 375
IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = @RightIndex)
BEGIN
	INSERT INTO [lookup].[tblRight] 
			([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	VALUES 
			(@RightIndex
			,'VIEW_ONLY_SITE_CLOSEOUT_TIME'
			,'VIEW_ONLY_SITE_CLOSEOUT_TIME'
			,'8F7070F6-9A51-4C08-83C3-5E1FA7B35A16'
			,N'2024-12-16 10:20:00 AM -05:00'
			,'Administrator'
			,N'2024-12-16 10:20:00 AM -05:00'
			,'Administrator')
END

-- Add VIEW_SITE_CLOSEOUT_TIME right to the Administrator User Group 
IF NOT EXISTS (SELECT * FROM map.tblGroupToRight WHERE GroupGuid = '00000000-0000-0000-0000-000000000003' AND LookupRightIndex = @RightIndex)
BEGIN
	INSERT INTO [map].[tblGroupToRight] ([GroupToRightGuid], [GroupGuid], [LookupRightIndex], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7FE51242-BDB3-4B05-A8A8-70D07B6AEBAE', N'00000000-0000-0000-0000-000000000003', @RightIndex, N'2024-12-15 9:30:00 AM -04:00', 'Varec', N'2024-12-15 9:30:00 AM -04:00', N'Varec') 
END

--Operate administer point groups
SET @RightIndex  = 376
IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = @RightIndex)
BEGIN
	INSERT INTO [lookup].[tblRight] 
			([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	VALUES 
			(@RightIndex
			,'OPERATE_ADMINISTER_POINT_GROUP'
			,'OPERATE_ADMINISTER_POINT_GROUP'
			,'3DB167B4-2AE3-40E3-8D18-441074450FF2'
			,N'2026-08-03 10:20:00 AM -04:00'
			,'Administrator'
			,N'2026-08-03 10:20:00 AM -04:00'
			,'Administrator')
END

--Operate administer movement summary
SET @RightIndex  = 377
IF NOT EXISTS (SELECT * FROM lookup.tblRight WHERE RightIndex = @RightIndex)
BEGIN
	INSERT INTO [lookup].[tblRight] 
			([RightIndex]
           ,[RightCode]
           ,[RightName]
           ,[RightGuid]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	VALUES 
			(@RightIndex
			,'OPERATE_ADMINISTER_MOVEMENT_SUMMARY'
			,'OPERATE_ADMINISTER_MOVEMENT_SUMMARY'
			,'0763EAD1-A54A-471A-864C-1057F729C725'
			,N'2026-08-03 10:20:00 AM -04:00'
			,'Administrator'
			,N'2026-08-03 10:20:00 AM -04:00'
			,'Administrator')
END

-- Restore PointTemplateTagGuid for 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference'
UPDATE pt  
SET PointTemplateTagGuid = ptt.PointTemplateTagGuid 
FROM tblPointTag pt
LEFT JOIN tblPoint p ON p.PointGuid = pt.PointGuid
LEFT JOIN tblPointTemplate ptmp ON ptmp.PointTemplateGuid = p.PointTemplateGuid
LEFT JOIN tblPointTemplateTag ptt ON ptt.PointTemplateGuid = ptmp.PointTemplateGuid
WHERE pt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference'
AND pt.ID =  ptt.ID

-- Set AlarmTemplateGuid to NULL for Alarms Associated with Tags of value type FMBusinessObjects.DataObjects.DeviceAlarmMapReference
UPDATE a Set AlarmTemplateGuid = null FROM tblAlarm a
inner join tblPointTag pt on pt.PointTagGuid = a.InputTagGuid
where pt.ValueType = 'FMBusinessObjects.DataObjects.DeviceAlarmMapReference'

IF (SELECT COUNT(*) FROM tblConfigurationSetting WHERE SettingKey = 'AlarmSilenceAuditLoggingEnabled') = 0
BEGIN
	INSERT INTO tblConfigurationSetting
	(ConfigurationSettingGuid, KeyType, SettingKey, SettingValue, CreatedDate, CreatedBy, UpdatedDate, UpdatedBy)
	VALUES
	('C08A8DE8-C377-4B97-9F80-7017C1D02733', 'SZ', 'AlarmSilenceAuditLoggingEnabled', N'false', SYSDATETIMEOFFSET(), 'Administrator', SYSDATETIMEOFFSET(), 'Administrator')
END

-- Update Vessel Tank Material when prior value is 'MildSteel' or 'StainlessSteel'
UPDATE tblPointTemplateProperty SET Value.modify('replace value of (/Vessel/TankMaterial/text())[1] with "MildCarbon"') WHERE ID = 'Vessel' AND Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(9)') = 'MildSteel'
UPDATE tblPointTemplateProperty SET Value.modify('replace value of (/Vessel/TankMaterial/text())[1] with "Stainless304"') WHERE ID = 'Vessel' AND Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(14)') = 'StainlessSteel'
UPDATE tblPointProperty SET Value.modify('replace value of (/Vessel/TankMaterial/text())[1] with "MildCarbon"') WHERE ID = 'Vessel' AND Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(9)') = 'MildSteel'
UPDATE tblPointProperty SET Value.modify('replace value of (/Vessel/TankMaterial/text())[1] with "Stainless304"') WHERE ID = 'Vessel' AND Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(14)') = 'StainlessSteel'
 
-- Update Vessel Tank Expantion Coeficiemt when prior value is 'MildCarbon' or 'StainlessSteel' AND TemperatureUnitIndex is 1 or 3
UPDATE ptp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "1.12E-05"') FROM tblPointTemplateProperty ptp
LEFT JOIN tblPointTemplate pt ON pt.PointTemplateGuid = ptp.PointTemplateGuid
WHERE ptp.ID = 'Vessel' AND ptp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(10)') = 'MildCarbon' AND pt.TemperatureUnitIndex IN (1,3)

UPDATE ptp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "6.20E-06"') FROM tblPointTemplateProperty ptp
LEFT JOIN tblPointTemplate pt ON pt.PointTemplateGuid = ptp.PointTemplateGuid
WHERE ptp.ID = 'Vessel' AND ptp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(10)') = 'MildCarbon' AND pt.TemperatureUnitIndex IN (2,4)

UPDATE ptp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "1.73E-05"') FROM tblPointTemplateProperty ptp
LEFT JOIN tblPointTemplate pt ON pt.PointTemplateGuid = ptp.PointTemplateGuid
WHERE ptp.ID = 'Vessel' AND ptp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(12)') = 'Stainless304' AND pt.TemperatureUnitIndex IN (1,3)

UPDATE ptp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "9.60E-06"') FROM tblPointTemplateProperty ptp
LEFT JOIN tblPointTemplate pt ON pt.PointTemplateGuid = ptp.PointTemplateGuid
WHERE ptp.ID = 'Vessel' AND ptp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(12)') = 'Stainless304' AND pt.TemperatureUnitIndex IN (2,4)

UPDATE pp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "1.12E-05"') FROM tblPointProperty pp
LEFT JOIN tblPoint p ON p.PointGuid = pp.PointGuid
WHERE pp.ID = 'Vessel' AND pp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(10)') = 'MildCarbon' AND p.TemperatureUnitIndex IN (1,3)

UPDATE pp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "6.20E-06"') FROM tblPointProperty pp
LEFT JOIN tblPoint p ON p.PointGuid = pp.PointGuid
WHERE pp.ID = 'Vessel' AND pp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(10)') = 'MildCarbon' AND p.TemperatureUnitIndex IN (2,4)

UPDATE pp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "1.73E-05"') FROM tblPointProperty pp
LEFT JOIN tblPoint p ON p.PointGuid = pp.PointGuid
WHERE pp.ID = 'Vessel' AND pp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(12)') = 'Stainless304' AND p.TemperatureUnitIndex IN (1,3)

UPDATE pp SET Value.modify('replace value of (/Vessel/TankExpansionCoefficient/Value/text())[1] with "9.60E-06"') FROM tblPointProperty pp
LEFT JOIN tblPoint p ON p.PointGuid = pp.PointGuid
WHERE pp.ID = 'Vessel' AND pp.Value.value('(/Vessel/TankMaterial/text())[1]','nvarchar(12)') = 'Stainless304' AND p.TemperatureUnitIndex IN (2,4)

IF NOT EXISTS (SELECT SettingKey FROM dbo.tblConfigurationSetting WHERE SettingKey = 'CustomApplicationType')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES 
	(N'4851BD16-87EC-4AF9-B201-6181B3539AB0', N'SZ', N'CustomApplicationType', '', N'09/16/2025 12:00:01 AM -05:00', N'Varec', N'09/16/2025 12:00:01 AM -05:00', N'Administrator')	
END

-- Move data from tblMovementSummaryColumns and tblMovementSummaryRows to tblMovementSummary
UPDATE ms SET ColumnsDefinition = msc.ColumnsDefinition, FontSize = msc.FontSize FROM dbo.tblMovementSummary ms INNER JOIN dbo.tblMovementSummaryColumns msc ON ms.MovementSummaryGuid = msc.MovementSummaryGuid
UPDATE ms SET RowsDefinition = msr.RowsDefinition FROM dbo.tblMovementSummary ms INNER JOIN dbo.tblMovementSummaryRows msr ON ms.MovementSummaryGuid = msr.MovementSummaryGuid
DELETE FROM dbo.tblMovementSummaryColumns
DELETE FROM dbo.tblMovementSummaryRows

BEGIN TRANSACTION
	DECLARE @FCEEPointTagInputOutputTypeIndex INT = 4
	--
	-- set default server engineering units for point template tags that don't have a default value.
	--
	--UPDATE ptt SET ServerEngineeringUnitsIndex=EngineeringUnitsIndex FROM tblPointTemplateTag ptt JOIN tblPointTemplate pt ON ptt.PointTemplateGuid=pt.PointTemplateGuid
	--WHERE ServerEngineeringUnitsIndex IN (0,255) AND EngineeringUnitsType not in (15, 16) 
	--AND valueType IN ('System.Single','System.Double') 


	-- Set following server engineering units for the following fce message types and tags
	--	WAGOPLCMsg 
	--		Density ('Density Product Observed') from FMdKgM3
	--	VeederRootInventoryReportMsg
	--		Height ('Level Product') from FmlInch
	--		Water  ('Level Water') from FmlInch
	--	VeederRootTLS50Msg
	--		Level ('Level Product')	from FmlInch
	--		WaterLevel ('Level Water')  from FmlInch	
	UPDATE ptt SET ServerEngineeringUnitsIndex=25	--FML_Inch 
	FROM tblPointTag ptt 
	JOIN tblPoint p ON p.PointGuid=ptt.PointGuid
	JOIN tblFCEEMapping m ON m.PointGuid=p.PointGuid
	JOIN lookup.tblEdgeMessage e ON m.MsgType=e.EdgeMessageIndex
	WHERE ServerEngineeringUnitsIndex IN (0,255) AND EngineeringUnitsType not in (15, 16) 
	AND valueType IN ('System.Single','System.Double') and PointTagInputOutputTypeIndex=@FCEEPointTagInputOutputTypeIndex
	AND (
		(e.EdgeMessageCode = 'VeederRootTLS350' AND ptt.ID IN ('Level Product', 'Level Water'))
	OR 	(e.EdgeMessageCode = 'VeederRootInventoryReport' AND ptt.ID IN ('Level Product', 'Level Water'))
	)

	UPDATE ptt SET ServerEngineeringUnitsIndex=183 --FMD_KgM3 
	FROM tblPointTag ptt 
	JOIN tblPoint p ON p.PointGuid=ptt.PointGuid
	JOIN tblFCEEMapping m ON m.PointGuid=p.PointGuid
	JOIN lookup.tblEdgeMessage e ON m.MsgType=e.EdgeMessageIndex
	WHERE ServerEngineeringUnitsIndex IN (0,255) AND EngineeringUnitsType not in (15, 16) 
	AND valueType IN ('System.Single','System.Double') and PointTagInputOutputTypeIndex=@FCEEPointTagInputOutputTypeIndex
	AND e.EdgeMessageCode = 'WAGOPLC' AND ptt.ID = 'Density Product Observed' 

	-- Remaining point tags that don't have their server engineering units set to their default values.
	UPDATE ptt SET ServerEngineeringUnitsIndex=EngineeringUnitsIndex FROM tblPointTag ptt JOIN tblPoint pt ON ptt.PointGuid=pt.PointGuid
	WHERE ServerEngineeringUnitsIndex IN (0,255) AND EngineeringUnitsType not in (15, 16) 
	AND valueType IN ('System.Single','System.Double')

 COMMIT TRANSACTION