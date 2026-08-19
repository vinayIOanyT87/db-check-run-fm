/*************************************************'
* Script.IncrementalDataMaintenance.sql file
* Use this file for include scripts for:
* 1. Insert data into a table that already has data (e.g. new entry into an already populated lookup table). It is required that the insert script verifies whether the inserting record does not exist).
* 2. Update the content of a record(s) present in a table
* 3. Delete records from a table 
**************************************************/
IF (NOT EXISTS (SELECT 1 FROM tblAuditHandler WHERE TableName = 'tblExternalStation'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblExternalStation'
							, 'External Station'
							, ''
							, 'SELECT @ID = ID'
							+ ' FROM [fmaudit].[tblExternalStation] a'
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
							, 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblExternalStation] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'
	)
END


IF (NOT EXISTS (SELECT 1 FROM tblAuditHandler WHERE TableName = 'map_tblEntityExternalStationToSite'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'map_tblEntityExternalStationToSite'
							, 'Site - External Station'
							, ''
							, 'SELECT @ID = CASE WHEN s.ID IS NULL THEN sa.ID ELSE s.ID END + '' - '''
							+ ' + CASE WHEN externalStation.ID IS NULL THEN externalStationAudit.ID ELSE externalStation.ID END'
							+ ' FROM  [fmaudit].[map_tblEntityExternalStationToSite] a'
							+ ' LEFT JOIN [dbo].[tblExternalStation] externalStation ON externalStation.ExternalStationGuid = a.ExternalStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblExternalStation] externalStationAudit ON externalStationAudit.ExternalStationGuid = a.ExternalStationGuid AND externalStationAudit._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblSitesShadow] s ON s.SiteGuid = a.SiteGuid'
							+ ' LEFT JOIN [fmaudit].[tblSites] sa ON sa.SiteGuid = a.SiteGuid AND sa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID',
							'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[map_tblEntityExternalStationToSite] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'
	)
END

IF (NOT EXISTS (SELECT 1 FROM tblAuditHandler WHERE TableName = 'map_tblExternalStationToProduct'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'map_tblExternalStationToProduct'
							, 'External Station - Product'
							, 'External Station'
							, 'SELECT @ID = CASE WHEN externalStation.ID IS NULL THEN externalStationAudit.ID ELSE externalStation.ID END + '' - '''
							+ ' + CASE WHEN p.ProductID IS NULL THEN pa.ProductID ELSE p.ProductID END'
							+ ' FROM  [fmaudit].[map_tblExternalStationToProduct] a'
							+ ' LEFT JOIN [dbo].[tblExternalStation] externalStation ON externalStation.ExternalStationGuid = a.ExternalStationGuid'
							+ ' LEFT JOIN [fmaudit].[tblExternalStation] externalStationAudit ON externalStationAudit.ExternalStationGuid = a.ExternalStationGuid AND externalStationAudit._AuditEventType = ''D'''
							+ ' LEFT JOIN [dbo].[tblProducts] p ON p.ProductGuid = a.ProductGuid'
							+ ' LEFT JOIN [fmaudit].[tblProducts] pa ON pa.ProductGuid = a.ProductGuid AND pa._AuditEventType = ''D'''
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID',
							'SELECT @SiteGuid = CASE WHEN s.SiteGuid IS NULL THEN sa.SiteGuid ELSE s.SiteGuid END FROM [fmaudit].[map_tblExternalStationToProduct] a LEFT JOIN [dbo].[tblExternalStation] s ON s.ExternalStationGuid = a.ExternalStationGuid LEFT JOIN [fmaudit].[tblExternalStation] sa ON sa.ExternalStationGuid = a.ExternalStationGuid AND sa._AuditEventType = ''D'' WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'
	)
END

IF (NOT EXISTS (SELECT 1 FROM tblAuditHandler WHERE TableName = 'tblExternalStationGeneralConfiguration'))
BEGIN
	INSERT INTO [dbo].[tblAuditHandler] (TableName, TypeID, ParentTypeID, IDQuery, SiteGuidQuery) VALUES (
							'tblExternalStationGeneralConfiguration'
							, 'External Station General Configuration'
							, ''
							, 'SELECT @ID = tblSites.ID'
							+ ' FROM [fmaudit].[tblExternalStationGeneralConfiguration] a'
							+ ' INNER JOIN tblSites ON a.SiteGuid = tblSites.SiteGuid ' 
							+ ' WHERE a._AuditEventSequence = 1 AND a._AuditGUID = @_AuditGUID'		
							, 'SELECT @SiteGuid = SiteGuid FROM [fmaudit].[tblExternalStationGeneralConfiguration] a WHERE a._AuditEventSequence = 1 AND a. _AuditGUID = @_AuditGUID'
	)
END

IF (NOT EXISTS (SELECT 1 FROM lookup.tblExternalStationLogType WHERE ExternalStationLogTypeIndex = 2))
BEGIN
	INSERT INTO lookup.tblExternalStationLogType (ExternalStationLogTypeIndex, ExternalStationLogTypeCode, ExternalStationLogTypeName, ExternalStationLogTypeGuid, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate) 
	VALUES (2, 'StationEvent', 'Station Event', N'3551B8C3-5DF2-4AA6-BBD7-D5F9FCB2EEAD', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00', 'Administrator', '2015-01-26 00:00:00.0000000 -04:00')
END

-- Originally the right was called VIEW_EXTERNAL_STATION but that was being confused with the STATIONS used by Terminal Automation.
-- 'IncrementaldataMaintenance: Adding VIEW_AUTOMATED_FUEL_SERVICE_STATION Right'
IF EXISTS (SELECT 1 FROM lookup.tblRight WHERE RightIndex = 181 AND (RightCode <> N'VIEW_EXTERNAL_STATION' AND RightCode <> N'VIEW_AUTOMATED_FUEL_SERVICE_STATION'))
BEGIN
	RAISERROR('FAILED to add 181 - View Automated Fuel Service Station Rights to the system.  181 is already being used by another right.', 16, 1)
END
ELSE IF EXISTS (SELECT 1 FROM lookup.tblRight WHERE RightIndex = 181 AND RightCode = N'VIEW_EXTERNAL_STATION')
BEGIN
	PRINT 'IncrementaldataMaintenance: Updating VIEW_AUTOMATED_FUEL_SERVICE_STATION Right'
	UPDATE [lookup].[tblRight] 
		SET RightCode = 'VIEW_AUTOMATED_FUEL_SERVICE_STATION',
			RightName = 'VIEW_AUTOMATED_FUEL_SERVICE_STATION' 
	WHERE RightIndex = 181;
END
ELSE
BEGIN
	IF (NOT EXISTS (SELECT 1 FROM lookup.tblRight WHERE RightIndex = 181 AND RightCode = N'VIEW_AUTOMATED_FUEL_SERVICE_STATION'))
	BEGIN
	   PRINT 'IncrementaldataMaintenance: Inserting VIEW_AUTOMATED_FUEL_SERVICE_STATION Right'
	   INSERT INTO [lookup].[tblRight]
			 (RightIndex
			 ,RightCode
			 ,RightName
			 ,RightGuid
			 ,CreatedDate
			 ,CreatedBy
			 ,UpdatedDate
			 ,UpdatedBy)
	   VALUES
			 (181
			 ,'VIEW_AUTOMATED_FUEL_SERVICE_STATION'
			 ,'VIEW_AUTOMATED_FUEL_SERVICE_STATION'
			 ,'93E2CBFD-320B-466D-AD76-FBEB6B73FBDC'
			 ,N'12/02/2014 1:49:09 PM -04:00'
			 ,'Administrator'
			 ,N'12/02/2014 1:49:09 PM -04:00'
			 ,'Administrator')
	END
END

-- Originally the right was called MODIFY_EXTERNAL_STATION but that was being confused with the STATIONS used by Terminal Automation.
-- 'IncrementaldataMaintenance: Adding MODIFY_AUTOMATED_FUEL_SERVICE_STATION Right'
IF EXISTS (SELECT 1 FROM lookup.tblRight WHERE RightIndex = 182 AND RightCode <> N'MODIFY_EXTERNAL_STATION' AND RightCode <> N'MODIFY_AUTOMATED_FUEL_SERVICE_STATION')
BEGIN
	RAISERROR('FAILED to add 182 - Modify Automated Fuel Service Station Rights to the system.  182 is already being used by another right.', 16, 1)
END
ELSE IF EXISTS (SELECT 1 FROM lookup.tblRight WHERE RightIndex = 182 AND RightCode = N'MODIFY_EXTERNAL_STATION')
BEGIN
	PRINT 'IncrementaldataMaintenance: Updating MODIFY_AUTOMATED_FUEL_SERVICE_STATION Right'
	UPDATE [lookup].[tblRight] 
		SET RightCode = 'MODIFY_AUTOMATED_FUEL_SERVICE_STATION',
			RightName = 'MODIFY_AUTOMATED_FUEL_SERVICE_STATION' 
	WHERE RightIndex = 182;
END
ELSE
BEGIN
	IF (NOT EXISTS (SELECT 1 FROM lookup.tblRight WHERE RightIndex = 182 AND RightCode = N'MODIFY_AUTOMATED_FUEL_SERVICE_STATION'))
	BEGIN
		PRINT 'IncrementaldataMaintenance: Inserting MODIFY_AUTOMATED_FUEL_SERVICE_STATION Right'
		INSERT INTO [lookup].[tblRight]
			   (RightIndex
			   ,RightCode
			   ,RightName
			   ,RightGuid
			   ,CreatedDate
			   ,CreatedBy
			   ,UpdatedDate
			   ,UpdatedBy)
		 VALUES
			   (182
			   ,'MODIFY_AUTOMATED_FUEL_SERVICE_STATION'
			   ,'MODIFY_AUTOMATED_FUEL_SERVICE_STATION'
			   ,'BA9D30CA-9642-4407-BCB6-0B65F4C31752'
			   ,N'12/02/2014 1:49:09 PM -04:00'
			   ,'Administrator'
			   ,N'12/02/2014 1:49:09 PM -04:00'
			   ,'Administrator')
	END
END

PRINT 'IncrementaldataMaintenance: Adding CONFIG_AUTOMATED_FUEL_SERVICE_STATIONS menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_OTHER_EXTERNAL_STATIONS' OR MenuItemTypeCode = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATIONS' OR MenuItemTypeIndex = 4057))
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
		   (4057
		   ,N'CONFIG_OTHER_EXTERNAL_STATIONS'
		   ,N'CONFIG_OTHER_EXTERNAL_STATIONS'
		   ,N'A4C68EA8-E24A-4545-87B7-690DCCDE9DE7'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator')
END

PRINT 'IncrementaldataMaintenance: Adding CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_DATA_IMPORT menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_IMPORT_EXPORT_EXTERNAL_STATION_DATA_IMPORT' OR MenuItemTypeCode = N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_DATA_IMPORT' OR MenuItemTypeIndex = 4058))
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
		   (4058
		   ,N'CONFIG_IMPORT_EXPORT_EXTERNAL_STATION_DATA_IMPORT'
		   ,N'CONFIG_IMPORT_EXPORT_EXTERNAL_STATION_DATA_IMPORT'
		   ,N'F216F0A5-7066-4DF5-8687-C41BCA2A36F1'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator')
END

PRINT 'IncrementaldataMaintenance: Adding CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_OTHER_EXTERNAL_STATION_GENERAL_CONFIGURATION' OR MenuItemTypeCode = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION' OR MenuItemTypeIndex = 4059))
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
		   (4059
		   ,N'CONFIG_OTHER_EXTERNAL_STATION_GENERAL_CONFIGURATION'
		   ,N'CONFIG_OTHER_EXTERNAL_STATION_GENERAL_CONFIGURATION'
		   ,N'8605FD5C-C757-4D61-B09B-C6952DDB0F08'
		   ,N'1/13/2015 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'1/13/2015  1:49:09 PM -04:00'
		   ,N'Administrator')
END


PRINT 'IncrementaldataMaintenance: Adding CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_IMPORT_BLACKLIST menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_IMPORT_BLACKLIST' OR MenuItemTypeCode = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION' OR MenuItemTypeIndex = 4060))
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
		   (4060
		   ,N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_IMPORT_BLACKLIST'
		   ,N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_IMPORT_BLACKLIST'
		   ,N'287B3706-BD9D-41EF-AC87-2EDF92667AE0'
		   ,N'1/13/2015 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'1/13/2015  1:49:09 PM -04:00'
		   ,N'Administrator')
END


PRINT 'IncrementaldataMaintenance: Adding OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_LOG menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_LOG' OR MenuItemTypeCode = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_LOG' OR MenuItemTypeIndex = 7035))
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
		   (7035
		   ,N'OPERATIONS_EXTERNAL_STATION_LOG'
		   ,N'OPERATIONS_EXTERNAL_STATION_LOG'
		   ,N'2CD5EB29-914C-4274-B202-53C34345DCA4'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator')
END

PRINT 'IncrementaldataMaintenance: Adding OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_OPERATIONS menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_OPERATIONS' OR MenuItemTypeCode = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_OPERATIONS' OR MenuItemTypeIndex = 7036))
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
		   (7036
		   ,N'OPERATIONS_EXTERNAL_STATION_OPERATIONS'
		   ,N'OPERATIONS_EXTERNAL_STATION_OPERATIONS'
		   ,N'2BEAC9E8-7E73-457D-98A8-363CB912C615'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'12/02/2014 1:49:09 PM -04:00'
		   ,N'Administrator')
END

PRINT 'IncrementaldataMaintenance: Adding OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_FAILED_TRANSACTIONS menu item type'
IF (NOT EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_FAILED_TRANSACTIONS' OR MenuItemTypeCode = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_FAILED_TRANSACTIONS' OR MenuItemTypeIndex = 7037))
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
		   (7037
		   ,N'OPERATIONS_EXTERNAL_STATION_FAILED_TRANSACTIONS'
		   ,N'OPERATIONS_EXTERNAL_STATION_FAILED_TRANSACTIONS'
		   ,N'D1E8DD97-3764-4441-A0BF-867AAA9AF670'
		   ,N'12/17/2014 1:49:09 PM -04:00'
		   ,N'Administrator'
		   ,N'12/17/2014 1:49:09 PM -04:00'
		   ,N'Administrator')
END

-- Originally the menu options for the initial Gasboy Deployment used the term External Station.  This could create confusion with
-- Terminal Automation Stations so External Stations are being renamed to AUTOMATED FUEL SERVICE STATIONS (AFSS).

-- BEGIN RENAME OF EXTERNAL STATIONS
IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_OTHER_EXTERNAL_STATIONS' and MenuItemTypeIndex = 4057)
BEGIN
	UPDATE [lookup].[tblMenuItemType] 
		SET MenuItemTypeCode = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATIONS',
			MenuItemTypeName = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATIONS'
	WHERE MenuItemTypeCode = N'CONFIG_OTHER_EXTERNAL_STATIONS' 
			AND MenuItemTypeIndex = 4057
END

IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_IMPORT_EXPORT_EXTERNAL_STATION_DATA_IMPORT' and MenuItemTypeIndex = 4058)
BEGIN
	UPDATE [lookup].[tblMenuItemType] 
		SET MenuItemTypeCode = N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_DATA_IMPORT',
			MenuItemTypeName = N'CONFIG_IMPORT_EXPORT_AUTOMATED_FUEL_SERVICE_STATION_DATA_IMPORT'
	WHERE MenuItemTypeCode = N'CONFIG_IMPORT_EXPORT_EXTERNAL_STATION_DATA_IMPORT' 
			AND MenuItemTypeIndex = 4058
END

IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'CONFIG_OTHER_EXTERNAL_STATION_GENERAL_CONFIGURATION' AND MenuItemTypeIndex = 4059)
BEGIN
	UPDATE [lookup].[tblMenuItemType] 
		SET MenuItemTypeCode = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION',
			MenuItemTypeName = N'CONFIG_AUTOMATED_FUEL_SERVICE_STATION_GENERAL_CONFIGURATION'
	WHERE MenuItemTypeCode = N'CONFIG_OTHER_EXTERNAL_STATION_GENERAL_CONFIGURATION' 
			AND MenuItemTypeIndex = 4059
END

IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_LOG' and MenuItemTypeIndex = 7035)
BEGIN
	UPDATE [lookup].[tblMenuItemType] 
		SET MenuItemTypeCode = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_LOG',
			MenuItemTypeName = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_LOG'
	WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_LOG' 
			AND MenuItemTypeIndex = 7035
END

IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_OPERATIONS' and MenuItemTypeIndex = 7036)
BEGIN
	UPDATE [lookup].[tblMenuItemType] 
		SET MenuItemTypeCode = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_OPERATIONS',
			MenuItemTypeName = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_OPERATIONS'
	WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_OPERATIONS' 
			AND MenuItemTypeIndex = 7036
END

IF EXISTS (SELECT 1 FROM lookup.tblMenuItemType WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_FAILED_TRANSACTIONS' and MenuItemTypeIndex = 7037)
BEGIN
	UPDATE [lookup].[tblMenuItemType] 
		SET MenuItemTypeCode = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_FAILED_TRANSACTIONS',
			MenuItemTypeName = N'OPERATIONS_AUTOMATED_FUEL_SERVICE_STATION_FAILED_TRANSACTIONS'
	WHERE MenuItemTypeCode = N'OPERATIONS_EXTERNAL_STATION_FAILED_TRANSACTIONS' 
			AND MenuItemTypeIndex = 7037
END
-- END RENAME OF EXTERNAL STATIONS


IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'305AEC28-3B3A-4BD2-AF03-9A15EC8ED553'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationLogType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'305AEC28-3B3A-4BD2-AF03-9A15EC8ED553' AND TableName = N'lookup.tblExternalStationLogType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'305AEC28-3B3A-4BD2-AF03-9A15EC8ED553', N'lookup.tblExternalStationLogType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'437675B8-B4FA-4003-BCB7-2EFB126585BD'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationSessionState'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'437675B8-B4FA-4003-BCB7-2EFB126585BD' AND TableName = N'lookup.tblExternalStationSessionState')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'437675B8-B4FA-4003-BCB7-2EFB126585BD', N'lookup.tblExternalStationSessionState', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'94F3A0B3-CD7F-4A17-83B1-5131CE423350'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationSessionStatus'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'94F3A0B3-CD7F-4A17-83B1-5131CE423350' AND TableName = N'lookup.tblExternalStationSessionStatus')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'94F3A0B3-CD7F-4A17-83B1-5131CE423350', N'lookup.tblExternalStationSessionStatus', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'3F5F8F1A-C135-4C25-A4D7-A0D8A60730AF'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationSessionType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'3F5F8F1A-C135-4C25-A4D7-A0D8A60730AF' AND TableName = N'lookup.tblExternalStationSessionType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'3F5F8F1A-C135-4C25-A4D7-A0D8A60730AF', N'lookup.tblExternalStationSessionType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'AB84F4CB-45FB-40A4-9182-A1AC4528A982'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationStatus'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'AB84F4CB-45FB-40A4-9182-A1AC4528A982' AND TableName = N'lookup.tblExternalStationStatus')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'AB84F4CB-45FB-40A4-9182-A1AC4528A982', N'lookup.tblExternalStationStatus', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'09208933-44A9-4302-A6A7-88D9781B4C0D'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationTransactionFailedStatus'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'09208933-44A9-4302-A6A7-88D9781B4C0D' AND TableName = N'lookup.tblExternalStationTransactionFailedStatus')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'09208933-44A9-4302-A6A7-88D9781B4C0D', N'lookup.tblExternalStationTransactionFailedStatus', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'6149BF39-4BB0-46B5-8A6B-4E65608BFE5C'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationTransactionStatus'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'6149BF39-4BB0-46B5-8A6B-4E65608BFE5C' AND TableName = N'lookup.tblExternalStationTransactionStatus')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'6149BF39-4BB0-46B5-8A6B-4E65608BFE5C', N'lookup.tblExternalStationTransactionStatus', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'78A0134B-B510-4E1A-ABB4-F0ED63684582'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblExternalStationType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'78A0134B-B510-4E1A-ABB4-F0ED63684582' AND TableName = N'lookup.tblExternalStationType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'78A0134B-B510-4E1A-ABB4-F0ED63684582', N'lookup.tblExternalStationType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'C67B74F4-D784-4087-9C05-523F4D19B5B2'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyAuthType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'C67B74F4-D784-4087-9C05-523F4D19B5B2' AND TableName = N'lookup.tblGasboyAuthType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'C67B74F4-D784-4087-9C05-523F4D19B5B2', N'lookup.tblGasboyAuthType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9B0DE2A6-8A92-4255-9F5F-04CA7225A5EA'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyDeviceType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9B0DE2A6-8A92-4255-9F5F-04CA7225A5EA' AND TableName = N'lookup.tblGasboyDeviceType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'9B0DE2A6-8A92-4255-9F5F-04CA7225A5EA', N'lookup.tblGasboyDeviceType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'D6BD373C-6160-48B8-94E2-21C69C0D7FDD'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyEmployeeType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'D6BD373C-6160-48B8-94E2-21C69C0D7FDD' AND TableName = N'lookup.tblGasboyEmployeeType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'D6BD373C-6160-48B8-94E2-21C69C0D7FDD', N'lookup.tblGasboyEmployeeType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'F97D871A-641A-4592-8B00-C3638B778E96'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyEventErrorClassCode'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'F97D871A-641A-4592-8B00-C3638B778E96' AND TableName = N'lookup.tblGasboyEventErrorClassCode')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'F97D871A-641A-4592-8B00-C3638B778E96', N'lookup.tblGasboyEventErrorClassCode', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'AB390D46-13B4-420A-9041-224BCE84446F'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyEventObjectType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'AB390D46-13B4-420A-9041-224BCE84446F' AND TableName = N'lookup.tblGasboyEventObjectType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'AB390D46-13B4-420A-9041-224BCE84446F', N'lookup.tblGasboyEventObjectType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'39AFDF23-5DF8-4B01-8B52-4F037BD1AFC5'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyHardwareType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'39AFDF23-5DF8-4B01-8B52-4F037BD1AFC5' AND TableName = N'lookup.tblGasboyHardwareType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'39AFDF23-5DF8-4B01-8B52-4F037BD1AFC5', N'lookup.tblGasboyHardwareType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'6A9C6E64-D47F-448C-A78E-512AE0E65D03'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyRecordStatus'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'6A9C6E64-D47F-448C-A78E-512AE0E65D03' AND TableName = N'lookup.tblGasboyRecordStatus')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'6A9C6E64-D47F-448C-A78E-512AE0E65D03', N'lookup.tblGasboyRecordStatus', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9C9EECFB-CBC7-43A8-8ADD-707B4B6F78CA'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyTwoStageDriverValidationType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9C9EECFB-CBC7-43A8-8ADD-707B4B6F78CA' AND TableName = N'lookup.tblGasboyTwoStageDriverValidationType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'9C9EECFB-CBC7-43A8-8ADD-707B4B6F78CA', N'lookup.tblGasboyTwoStageDriverValidationType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'E0EEED65-C033-493B-B53F-7E7473F85CCD'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyVehiclePlateCheckType'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'E0EEED65-C033-493B-B53F-7E7473F85CCD' AND TableName = N'lookup.tblGasboyVehiclePlateCheckType')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'E0EEED65-C033-493B-B53F-7E7473F85CCD', N'lookup.tblGasboyVehiclePlateCheckType', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'537B2472-6C50-4D67-8D78-6E3EC617E918'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblExternalStation'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'537B2472-6C50-4D67-8D78-6E3EC617E918' AND TableName = N'dbo.tblExternalStation')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'537B2472-6C50-4D67-8D78-6E3EC617E918', N'dbo.tblExternalStation', N'2AD7A4B9-68B1-45D8-AEAF-00938E96F22D', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'A2538EAE-243B-4BDE-9F53-912F067DE01A'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblExternalStationGeneralConfiguration'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'A2538EAE-243B-4BDE-9F53-912F067DE01A' AND TableName = N'dbo.tblExternalStationGeneralConfiguration')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'A2538EAE-243B-4BDE-9F53-912F067DE01A', N'dbo.tblExternalStationGeneralConfiguration', N'64B99EFE-9B71-4E01-AC04-1DED944FFA2B', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'3519987C-A304-4803-9C6D-771A06733C1D'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblExternalStationLog'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'3519987C-A304-4803-9C6D-771A06733C1D' AND TableName = N'dbo.tblExternalStationLog')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'3519987C-A304-4803-9C6D-771A06733C1D', N'dbo.tblExternalStationLog', N'64B99EFE-9B71-4E01-AC04-1DED944FFA2B', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'17FA1676-388A-4149-BEAC-8794565F158C'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblExternalStationTransaction'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'17FA1676-388A-4149-BEAC-8794565F158C' AND TableName = N'dbo.tblExternalStationTransaction')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'17FA1676-388A-4149-BEAC-8794565F158C', N'dbo.tblExternalStationTransaction', N'64B99EFE-9B71-4E01-AC04-1DED944FFA2B', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'FE34DE40-3BF5-424B-B91F-D0CA8B785470'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblExternalGasboyStation'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'FE34DE40-3BF5-424B-B91F-D0CA8B785470' AND TableName = N'dbo.tblExternalGasboyStation')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag], [ParentSyncTableGuid], [ParentForeignKeyColumnName]) VALUES (N'FE34DE40-3BF5-424B-B91F-D0CA8B785470', N'dbo.tblExternalGasboyStation', N'64B99EFE-9B71-4E01-AC04-1DED944FFA2B', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1, N'537B2472-6C50-4D67-8D78-6E3EC617E918', N'ExternalStationGuid')
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'4CB5B4B8-CF70-4217-8521-23190DF7CB3F'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'map.tblEntityExternalStationToSite'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'4CB5B4B8-CF70-4217-8521-23190DF7CB3F' AND TableName = N'map.tblEntityExternalStationToSite')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'4CB5B4B8-CF70-4217-8521-23190DF7CB3F', N'map.tblEntityExternalStationToSite', N'64B99EFE-9B71-4E01-AC04-1DED944FFA2B', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9611564D-B201-4B8B-A3CD-2825B4668FA1'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'map.tblExternalStationToProduct'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9611564D-B201-4B8B-A3CD-2825B4668FA1' AND TableName = N'map.tblExternalStationToProduct')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'9611564D-B201-4B8B-A3CD-2825B4668FA1', N'map.tblExternalStationToProduct', N'64B99EFE-9B71-4E01-AC04-1DED944FFA2B', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'F5DCE07A-DAB7-4EF9-B59D-B95CD93658B2'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblExternalStationTransactionError'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'F5DCE07A-DAB7-4EF9-B59D-B95CD93658B2' AND TableName = N'dbo.tblExternalStationTransactionError')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag], [ParentSyncTableGuid], [ParentForeignKeyColumnName]) VALUES (N'F5DCE07A-DAB7-4EF9-B59D-B95CD93658B2', N'dbo.tblExternalStationTransactionError', N'69EB8DA5-C8AF-42FA-BE84-0E8BFA74104F', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1, '17FA1676-388A-4149-BEAC-8794565F158C', 'ExternalStationTransactionGuid')
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'911162B1-D595-4694-819A-085286EC4BA5'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblGasboyDepartment'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'911162B1-D595-4694-819A-085286EC4BA5' AND TableName = N'dbo.tblGasboyDepartment')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'911162B1-D595-4694-819A-085286EC4BA5', N'dbo.tblGasboyDepartment', N'69EB8DA5-C8AF-42FA-BE84-0E8BFA74104F', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'E58359EE-81A1-4C3E-B040-0BBF158FD605'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblGasboyFleet'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'E58359EE-81A1-4C3E-B040-0BBF158FD605' AND TableName = N'dbo.tblGasboyFleet')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'E58359EE-81A1-4C3E-B040-0BBF158FD605', N'dbo.tblGasboyFleet', N'69EB8DA5-C8AF-42FA-BE84-0E8BFA74104F', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'6468F691-86BB-4E01-9AE7-E6452824DD03'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblGasboyStationEvent'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'6468F691-86BB-4E01-9AE7-E6452824DD03' AND TableName = N'dbo.tblGasboyStationEvent')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'6468F691-86BB-4E01-9AE7-E6452824DD03', N'dbo.tblGasboyStationEvent', N'69EB8DA5-C8AF-42FA-BE84-0E8BFA74104F', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'B66A7D3D-5755-40C8-B53A-05427682247E'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblGasboyDevice'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'B66A7D3D-5755-40C8-B53A-05427682247E' AND TableName = N'dbo.tblGasboyDevice')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'B66A7D3D-5755-40C8-B53A-05427682247E', N'dbo.tblGasboyDevice', N'20B7E0F1-3038-4F47-AC42-F448169CF303', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'3D971FD3-E796-4621-BD74-01113AF680D2'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'dbo.tblGasboyStationGeneralConfiguration'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'3D971FD3-E796-4621-BD74-01113AF680D2' AND TableName = N'dbo.tblGasboyStationGeneralConfiguration')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'3D971FD3-E796-4621-BD74-01113AF680D2', N'dbo.tblGasboyStationGeneralConfiguration', N'20B7E0F1-3038-4F47-AC42-F448169CF303', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9C20ACFB-FEDE-45B3-A740-A7B1D2D1E76F'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'map.tblEntityGasboyDepartmentToSite'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9C20ACFB-FEDE-45B3-A740-A7B1D2D1E76F' AND TableName = N'map.tblEntityGasboyDepartmentToSite')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'9C20ACFB-FEDE-45B3-A740-A7B1D2D1E76F', N'map.tblEntityGasboyDepartmentToSite', N'20B7E0F1-3038-4F47-AC42-F448169CF303', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'978C6F3F-2BC7-4BEF-A465-E0D2C6DCE3FF'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'map.tblEntityGasboyFleetToSite'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'978C6F3F-2BC7-4BEF-A465-E0D2C6DCE3FF' AND TableName = N'map.tblEntityGasboyFleetToSite')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'978C6F3F-2BC7-4BEF-A465-E0D2C6DCE3FF', N'map.tblEntityGasboyFleetToSite', N'20B7E0F1-3038-4F47-AC42-F448169CF303', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'68122FDF-6D31-4B6A-944D-560C4D77062E'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'map.tblGasboyDepartmentToGasboyFleet'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'68122FDF-6D31-4B6A-944D-560C4D77062E' AND TableName = N'map.tblGasboyDepartmentToGasboyFleet')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'68122FDF-6D31-4B6A-944D-560C4D77062E', N'map.tblGasboyDepartmentToGasboyFleet', N'20B7E0F1-3038-4F47-AC42-F448169CF303', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9489C2A7-BEBE-4FD0-BB1F-021D40CE7977'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'map.tblEntityGasboyDeviceToSite'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'9489C2A7-BEBE-4FD0-BB1F-021D40CE7977' AND TableName = N'map.tblEntityGasboyDeviceToSite')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'9489C2A7-BEBE-4FD0-BB1F-021D40CE7977', N'map.tblEntityGasboyDeviceToSite', N'9E6ADA63-3FC7-48A3-84A1-C2FEF42B5D27', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 1, 1)
END

IF NOT EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'031EACEC-B99E-4152-8D23-79C562C0F96D'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE TableName = N'lookup.tblGasboyErrorCode'
				UNION
				SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'031EACEC-B99E-4152-8D23-79C562C0F96D' AND TableName = N'lookup.tblGasboyErrorCode')
BEGIN
	INSERT INTO [sync].[tblSyncTable] ([SyncTableGuid], [TableName], [SyncDependencyGroupGuid], [LastSchemaDate], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [IsSiteFilteredFlag], [IsSiteFilteredOnDeleteFlag]) VALUES (N'031EACEC-B99E-4152-8D23-79C562C0F96D', N'lookup.tblGasboyErrorCode', N'FCD07CF0-1692-4831-965F-1FD5D1DD421C', NULL, '2016-02-16 00:00:00.0000 -05:00', 'Administrator', '2016-02-16 00:00:00.0000 -05:00', 'Administrator', 0, 0)
END


/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationLogType'
									AND sttsm.[ID] = N'tblExternalStationLogType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a557ba7d-69ed-4418-8426-1173d9805466', N'tblExternalStationLogType', N'7167bd49-019e-4784-8228-d2d89549be29', N'305aec28-3b3a-4bd2-af03-9a15ec8ed553', 3, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e4b6a65f-8386-425e-be5b-ba860f090fb0', N'a557ba7d-69ed-4418-8426-1173d9805466', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationLogType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationLogType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationLogType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationLogType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationLogType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationLogType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationLogType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationLogType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'06912c78-c099-4f88-a06f-8102da0c43b0', N'a557ba7d-69ed-4418-8426-1173d9805466', N'ExternalStationLogTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1a600c89-58cf-4f67-8940-3c1fe35079f1', N'a557ba7d-69ed-4418-8426-1173d9805466', N'ExternalStationLogTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f2e3d4c4-0e3a-4e0a-9599-25b29a55d580', N'a557ba7d-69ed-4418-8426-1173d9805466', N'ExternalStationLogTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4df3ba6b-00af-4c5d-8b86-246fbced00ca', N'a557ba7d-69ed-4418-8426-1173d9805466', N'ExternalStationLogTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dd9f2103-ebae-4234-a1a3-3ef1ba50589c', N'a557ba7d-69ed-4418-8426-1173d9805466', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6d4393db-03a1-4995-b003-a11838a59196', N'a557ba7d-69ed-4418-8426-1173d9805466', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'20780993-8861-4a09-af33-f347d4ce3295', N'a557ba7d-69ed-4418-8426-1173d9805466', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'90293707-4f19-451a-9ee1-9fe6e1dab7a9', N'a557ba7d-69ed-4418-8426-1173d9805466', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationSessionState'
									AND sttsm.[ID] = N'tblExternalStationSessionState')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'tblExternalStationSessionState', N'7167bd49-019e-4784-8228-d2d89549be29', N'437675b8-b4fa-4003-bcb7-2efb126585bd', 4, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9c28b807-a88d-430b-af73-7aabda31ef67', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationSessionState', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationSessionState', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationSessionState', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationSessionState', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationSessionState', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationSessionState', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationSessionState', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationSessionState', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'76500a7d-cb0b-472a-92ce-5440134a71f4', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'ExternalStationSessionStateIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'21cbc0b5-7e49-4492-804f-2f60446d807c', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'ExternalStationSessionStateCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'27d34f8a-c254-44bb-91d8-16c6e9cc5633', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'ExternalStationSessionStateName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9074b3d4-ff2e-45d1-9b0e-8eb49b32358b', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'ExternalStationSessionStateGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3e3c4e98-b3e3-4a2f-9fee-0bfcf0b8de2f', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'LongDescription', 4, N'NVarChar', 1024, 0, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ffb8a2c2-ffc0-4f23-b5f7-dc25f3dfeaf4', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'CreatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e865095f-8a8e-4a84-852a-daeff2a49649', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'CreatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'510e079d-6000-4df6-94bc-8c42b4679854', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'UpdatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'767947dc-4df3-40ec-9eeb-397e5df006b3', N'4c5113d1-9f59-49d5-ac34-1acfade3ef7e', N'UpdatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationSessionStatus'
									AND sttsm.[ID] = N'tblExternalStationSessionStatus')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'tblExternalStationSessionStatus', N'7167bd49-019e-4784-8228-d2d89549be29', N'94f3a0b3-cd7f-4a17-83b1-5131ce423350', 5, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4e797c86-4765-41e2-a3df-dff44068774f', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationSessionStatus', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationSessionStatus', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7ffbf04b-97e5-432c-9414-5cf12d9deed1', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'ExternalStationSessionStatusIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'52e882d4-58bf-4e9e-8874-e5e65d113ca7', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'ExternalStationSessionStatusCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a59c34b6-a79c-401e-ba06-2c89d3c235b1', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'ExternalStationSessionStatusName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f1a8c6d2-3240-4e68-ba14-1990a75e87f9', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'ExternalStationSessionStatusGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'660e040a-6cdf-41b0-b5a7-c8fbe629f49a', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'LongDescription', 4, N'NVarChar', 1024, 0, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2073433f-148e-4fdd-b0a9-f84899e83cfe', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'CreatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bff22760-8d58-4b6c-a5c3-523e98083a69', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'CreatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4712a18e-fc0a-47c2-9f55-aa07365a39ea', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'UpdatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6781ba83-edf2-4d83-96db-cfb9e994970f', N'90c120d5-d947-47c0-9fe3-5fdb759c2535', N'UpdatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationSessionType'
									AND sttsm.[ID] = N'tblExternalStationSessionType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'tblExternalStationSessionType', N'7167bd49-019e-4784-8228-d2d89549be29', N'3f5f8f1a-c135-4c25-a4d7-a0d8a60730af', 6, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'54322bee-3b4d-4239-b0b7-e114d8cd9538', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationSessionType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationSessionType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationSessionType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationSessionType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationSessionType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationSessionType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationSessionType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationSessionType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd6d10c42-0078-4336-94b2-c3554f3d1787', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'ExternalStationSessionTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'76a13fca-5a37-40e6-aea1-a0edbbaa7cdc', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'ExternalStationSessionTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7060b2d3-e63c-4fd2-b7e2-4787b3d27ede', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'ExternalStationSessionTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3a743f81-d42f-46f3-a082-55ad796d7eff', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'ExternalStationSessionTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a62a6337-beb8-4b69-8aff-038efd6bd39d', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'LongDescription', 4, N'NVarChar', 1024, 0, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f561cafa-4927-4556-9788-59ac7e8efe1f', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'CreatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0592c165-b631-46ca-946c-da9b574b02b4', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'CreatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5a1f8a6a-201d-4fb4-8e0c-38af0fb2761b', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'UpdatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5970f9dc-de77-463e-907d-c28164d5328b', N'63b1e80a-242d-4266-b1a6-de38ac0f9ba9', N'UpdatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationStatus'
									AND sttsm.[ID] = N'tblExternalStationStatus')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'tblExternalStationStatus', N'7167bd49-019e-4784-8228-d2d89549be29', N'ab84f4cb-45fb-40a4-9182-a1ac4528a982', 7, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a64ee3ed-440f-4428-b79f-3d3c84fa5091', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationStatus', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationStatus', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationStatus', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationStatus', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationStatus', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationStatus', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationStatus', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationStatus', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3267a631-e70b-4669-9e07-5dfad77b43e5', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'ExternalStationStatusIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7bd6a83a-e39f-412f-b4f1-7794ab40a6a2', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'ExternalStationStatusCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8faecb62-53c0-4851-809c-59fd6ecab3a2', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'ExternalStationStatusName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ba29b523-b86a-410e-bbbe-4566bc064074', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'ExternalStationStatusGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'26944e46-84c5-4e6f-850f-7699cd9b1ce5', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fadb13bb-a0ba-4c14-8c88-133bba733cc3', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'94cf9f4f-7347-418b-a150-a328ae8b955c', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'20df541e-42fa-4d1c-b6d4-89524bd5f3cc', N'd15cb52a-b6f7-4c68-9707-c8c0edf1365f', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationTransactionFailedStatus'
									AND sttsm.[ID] = N'tblExternalStationTransactionFailedStatus')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'tblExternalStationTransactionFailedStatus', N'7167bd49-019e-4784-8228-d2d89549be29', N'09208933-44a9-4302-a6a7-88d9781b4c0d', 8, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c9077ac8-8a05-4ffc-adf0-6302a3dbb465', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationTransactionFailedStatus', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationTransactionFailedStatus', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dc0a581b-95d5-44ad-ad60-3c4f8760c736', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'ExternalStationTransactionFailedStatusIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1de2d255-6e28-4118-a715-c4676cab8d3d', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'ExternalStationTransactionFailedStatusCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'43254003-851b-4e85-86ad-7aa47e075841', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'ExternalStationTransactionFailedStatusName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd97cce8e-2ce4-4e29-9892-674e5cbc2ce5', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'ExternalStationTransactionFailedStatusGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'daacd98a-f5d4-4753-bf4a-12cdc65b96a4', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'LongDescription', 4, N'NVarChar', 1024, 0, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'88e37602-5fc9-477f-9219-e4b64a908db5', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'DisplayOrder', 5, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9a587883-38f2-4a5f-acd4-57f7ec8a33fe', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'FinalState', 6, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fa193b62-13b8-42ed-993b-66ea4a1246a5', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'CreatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'cb4c480b-6d0c-47fc-9c57-99e56235d167', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'CreatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8609a3f4-ba98-4af2-a053-65c0bb705b6b', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'UpdatedBy', 9, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1a3041a1-e85d-41f1-bca3-8a50fe2068d3', N'19a9c18b-282f-471c-8c8f-d0ec5d8ccd0b', N'UpdatedDate', 10, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationTransactionStatus'
									AND sttsm.[ID] = N'tblExternalStationTransactionStatus')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'tblExternalStationTransactionStatus', N'7167bd49-019e-4784-8228-d2d89549be29', N'6149bf39-4bb0-46b5-8a6b-4e65608bfe5c', 9, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fe412766-bd63-4fb0-bca4-682dd0630449', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationTransactionStatus', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationTransactionStatus', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'18a1244b-20a7-464d-8984-e4f5b3df2016', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'ExternalStationTransactionStatusIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'79c25c79-d428-4981-bf4d-dbfcf201e39f', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'ExternalStationTransactionStatusCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e2a4d342-573f-467a-9fbb-a323b7592081', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'ExternalStationTransactionStatusName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5a7856f1-bc1d-4b3f-bab4-74d3e25b1206', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'ExternalStationTransactionStatusGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6921e428-c335-45a9-acf5-1b6ba87e9c46', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'LongDescription', 4, N'NVarChar', 1024, 0, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'352e4745-212f-4235-a4c4-e0ac4bbc048a', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'DisplayOrder', 5, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6cd6579f-e030-4d59-98a2-cbc51269aeab', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'CreatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'289d10fe-305b-46d4-9d9f-6dd188bc5b3a', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'CreatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9f11b3fc-f4da-4eab-b6cc-295d10989bcf', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'UpdatedBy', 8, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0479bf43-b3fe-4f02-bef2-3e1c068c0fd6', N'c1a91208-ac30-4ac3-b6a2-f8cf3e832baa', N'UpdatedDate', 9, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblExternalStationType'
									AND sttsm.[ID] = N'tblExternalStationType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'tblExternalStationType', N'7167bd49-019e-4784-8228-d2d89549be29', N'78a0134b-b510-4e1a-abb4-f0ed63684582', 10, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'26615459-298c-4378-ad3c-f99ec6d68818', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5691a690-f9f1-45dd-b0ce-da57006cd44b', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'ExternalStationTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'069b2bd2-791f-4bb7-8007-0bcf22ce9c2a', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'ExternalStationTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8f2c71a6-c50a-4d18-a65f-31e55893be80', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'ExternalStationTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3f008403-c428-4fec-8815-c56f759e84a3', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'ExternalStationTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fdd27c0d-65f5-4e40-8f1c-8d8b37316298', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ca0b6f6b-96da-4522-a1f7-c90f4e06d9f4', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bba15642-d2c1-4a5f-9cde-b13e5c1b708e', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2c654c75-6768-4d1b-937e-b95baf522943', N'6232e8a0-2f5e-485b-bf7b-dc3828d91b02', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyAuthType'
									AND sttsm.[ID] = N'tblGasboyAuthType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5d111d29-4a40-454b-b6a7-be646398fb56', N'tblGasboyAuthType', N'7167bd49-019e-4784-8228-d2d89549be29', N'c67b74f4-d784-4087-9c05-523f4d19b5b2', 11, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a2840201-dcd5-48e9-ac0e-416c68289823', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyAuthType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyAuthType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyAuthType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyAuthType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyAuthType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyAuthType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyAuthType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyAuthType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'63b15fb3-8e92-4744-a5e8-5bc9beffb9e7', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'GasboyAuthTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'851ebb8c-a65d-4deb-a7ba-b9bbe163b9b0', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'GasboyAuthTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9cbbd679-3f6a-482c-b354-7296f0394711', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'GasboyAuthTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2c85859e-ff89-4405-a5ef-854f25050677', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'GasboyAuthTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'611b59aa-923b-4bf7-9fef-d57584358111', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'695211ae-ee74-4259-a514-acfd41be7871', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6f7ba0f0-2d3b-413c-b5ec-30e25816e987', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd660504b-db1e-45cc-bf52-c2f52e8a806b', N'5d111d29-4a40-454b-b6a7-be646398fb56', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyDeviceType'
									AND sttsm.[ID] = N'tblGasboyDeviceType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e44584ad-0061-4919-8810-ddb68abdee73', N'tblGasboyDeviceType', N'7167bd49-019e-4784-8228-d2d89549be29', N'9b0de2a6-8a92-4255-9f5f-04ca7225a5ea', 12, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a338b1ed-41b5-4fc0-9c3e-4086b59df998', N'e44584ad-0061-4919-8810-ddb68abdee73', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyDeviceType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyDeviceType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyDeviceType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyDeviceType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyDeviceType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyDeviceType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyDeviceType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyDeviceType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'982e83da-ed80-4491-8905-d86e1f3e291e', N'e44584ad-0061-4919-8810-ddb68abdee73', N'GasboyDeviceTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b24fd6fb-75a7-472d-a48b-59aa9a275be7', N'e44584ad-0061-4919-8810-ddb68abdee73', N'GasboyDeviceTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'eb05b63a-159a-41bf-91a0-e3dba1fd1ea9', N'e44584ad-0061-4919-8810-ddb68abdee73', N'GasboyDeviceTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8ac7eb47-3fbe-455a-8be1-cd5234f33559', N'e44584ad-0061-4919-8810-ddb68abdee73', N'GasboyDeviceTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f2810364-f82c-4d98-ab05-a02f3da780ff', N'e44584ad-0061-4919-8810-ddb68abdee73', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'eeb07072-7656-4dc0-8904-c8f0df93f190', N'e44584ad-0061-4919-8810-ddb68abdee73', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'25167226-f184-4503-9746-f9cddbddc11f', N'e44584ad-0061-4919-8810-ddb68abdee73', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'62615560-01cd-4f0d-a9f6-8abb27abfa8a', N'e44584ad-0061-4919-8810-ddb68abdee73', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyEmployeeType'
									AND sttsm.[ID] = N'tblGasboyEmployeeType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'tblGasboyEmployeeType', N'7167bd49-019e-4784-8228-d2d89549be29', N'd6bd373c-6160-48b8-94e2-21c69c0d7fdd', 13, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6c7fcf2e-e667-4e10-aa54-2bca0d22cf17', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyEmployeeType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyEmployeeType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyEmployeeType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyEmployeeType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyEmployeeType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyEmployeeType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyEmployeeType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyEmployeeType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'15994063-8188-456a-92db-2e5b4239659f', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'GasboyEmployeeTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8fa8ddb4-0086-4132-a553-f38d16098005', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'GasboyEmployeeTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e62310de-f652-4e9b-8a66-59ab7c6e9238', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'GasboyEmployeeTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5f096cca-f78f-4117-ab21-29c1c4022f7d', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'GasboyEmployeeTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ca104390-d728-4f1a-8e5d-2e8babf8d2c8', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9ab6d60f-5261-4005-b9b2-bdae733d011e', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9e3243a9-bf30-430e-9366-4fcd3a2a7265', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'df99dcfe-9b3d-4b50-af42-d6d20af8efd1', N'b8dfeb20-8a70-43e0-b444-d6d8a5de8f9c', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyEventErrorClassCode'
									AND sttsm.[ID] = N'tblGasboyEventErrorClassCode')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'tblGasboyEventErrorClassCode', N'7167bd49-019e-4784-8228-d2d89549be29', N'f97d871a-641a-4592-8b00-c3638b778e96', 14, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'45b4b452-fef6-4e2b-8d5f-2b84f3b36b7e', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyEventErrorClassCode', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyEventErrorClassCode', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f378ce8c-b805-465a-b846-630ef001c6b3', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'GasboyEventErrorClassCodeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6c5184ce-8cea-4411-8e3e-a8c2486ec4c2', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'GasboyEventErrorClassCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ff91f4c2-91f7-4f65-b60d-964e185068be', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'GasboyEventErrorClassCodeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'93ebf2b0-bb19-4595-b3e9-d97841488240', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'GasboyEventErrorClassCodeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'606f0551-cab2-4be1-b9ab-a22216d5e7ff', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'80acd776-4bc6-4079-ba4f-7992e5e9b18c', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9c76a1a6-62a3-4393-92cc-8e97ea1a6e01', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'972e5476-7379-4a8e-9970-11d8f7176f87', N'65461d1b-99a2-48fa-8fe4-1e21c1434f09', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyEventObjectType'
									AND sttsm.[ID] = N'tblGasboyEventObjectType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'tblGasboyEventObjectType', N'7167bd49-019e-4784-8228-d2d89549be29', N'ab390d46-13b4-420a-9041-224bce84446f', 15, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'96dddd81-605f-47ea-9a89-c0ece349b6eb', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyEventObjectType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyEventObjectType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyEventObjectType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyEventObjectType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyEventObjectType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyEventObjectType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyEventObjectType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyEventObjectType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c22e702c-4ef1-46b0-988c-083543028c42', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'GasboyEventObjectTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'801ff64c-1e9f-4b99-bf66-6afae174e679', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'GasboyEventObjectTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9515aafe-16dd-410e-a58a-78106f3d65cb', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'GasboyEventObjectTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'46fb66d0-ba24-43b7-aa21-92c84424144b', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'GasboyEventObjectTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b2e43fed-0977-4a08-a896-ac9d03bfbc20', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4b70820e-968a-4ae5-9a33-3f6b30d102b4', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8495e130-cacd-486d-a6ee-745c14788bcd', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9f3acabf-3df9-4637-bc18-884c722a9730', N'04538ca4-3cba-4fc8-888d-58c2ef9c52bf', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyHardwareType'
									AND sttsm.[ID] = N'tblGasboyHardwareType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'tblGasboyHardwareType', N'7167bd49-019e-4784-8228-d2d89549be29', N'39afdf23-5df8-4b01-8b52-4f037bd1afc5', 16, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f97172b9-29f6-4ea0-861b-c3601d7c07ef', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyHardwareType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyHardwareType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyHardwareType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyHardwareType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyHardwareType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyHardwareType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyHardwareType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyHardwareType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'33665a45-3bf6-45c3-a256-7d81c8dea717', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'GasboyHardwareTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f7052c84-e813-45f0-8687-bc680a363cea', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'GasboyHardwareTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fdc71389-3a05-4087-a5d2-a2e67b5c9b43', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'GasboyHardwareTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'952fe99d-8bfc-4d15-8875-b88d31e2ac50', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'GasboyHardwareTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6f2ed2ba-bc7b-4a7b-bad4-e23bedebec0f', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2b291b47-42fc-4767-8642-b296a26de17e', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ef971298-59b6-4d52-8f20-e733b1b54dab', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e26d28fb-f4da-447f-a920-b975fe42825c', N'833b0e6d-f66c-41a7-8618-a5070e8e1a3b', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyRecordStatus'
									AND sttsm.[ID] = N'tblGasboyRecordStatus')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'20b98df3-cefa-4697-a489-d9746579707e', N'tblGasboyRecordStatus', N'7167bd49-019e-4784-8228-d2d89549be29', N'6a9c6e64-d47f-448c-a78e-512ae0e65d03', 17, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1dcd3258-3176-4cf0-b134-9d9f97398a18', N'20b98df3-cefa-4697-a489-d9746579707e', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyRecordStatus', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyRecordStatus', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyRecordStatus', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyRecordStatus', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyRecordStatus', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyRecordStatus', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyRecordStatus', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyRecordStatus', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'323eb467-1beb-4c3a-8258-1bd384bbbee6', N'20b98df3-cefa-4697-a489-d9746579707e', N'GasboyRecordStatusIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f653d6a4-10c7-4903-b0ad-0f9307904ecc', N'20b98df3-cefa-4697-a489-d9746579707e', N'GasboyRecordStatusCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c2e188ab-98d2-491b-b9db-72f820dc3561', N'20b98df3-cefa-4697-a489-d9746579707e', N'GasboyRecordStatusName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c96d777d-0720-4d21-bfc9-a4c11950a495', N'20b98df3-cefa-4697-a489-d9746579707e', N'GasboyRecordStatusGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5b39dea5-27f7-444b-bac2-9fa61f627d9d', N'20b98df3-cefa-4697-a489-d9746579707e', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'98365768-0c25-409b-bf6f-4c6e3290da2d', N'20b98df3-cefa-4697-a489-d9746579707e', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a736be90-fec5-4fa2-9d55-429990a1b3e3', N'20b98df3-cefa-4697-a489-d9746579707e', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'571958be-afc4-46fb-8fbb-70587cc3fced', N'20b98df3-cefa-4697-a489-d9746579707e', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyTwoStageDriverValidationType'
									AND sttsm.[ID] = N'tblGasboyTwoStageDriverValidationType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'tblGasboyTwoStageDriverValidationType', N'7167bd49-019e-4784-8228-d2d89549be29', N'9c9eecfb-cbc7-43a8-8add-707b4b6f78ca', 18, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bf124ef6-1c8c-4791-ac85-ca798f5b74a9', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyTwoStageDriverValidationType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyTwoStageDriverValidationType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2ddc2abd-9fe7-4aa4-904e-f2943409296d', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'GasboyTwoStageDriverValidationTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'cc24f5ae-09e0-4b2f-b874-0189bb59654d', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'GasboyTwoStageDriverValidationTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'64ca14a2-1cdc-4539-9467-8375d3850bd4', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'GasboyTwoStageDriverValidationTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bbf7fa7b-8e9c-4625-976c-6339c3c21a77', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'GasboyTwoStageDriverValidationTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'095e1c2c-04a9-470f-bb14-e6ba7ce4bc44', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ae5f9d19-1566-4485-930a-698c42dee4f1', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bf0340d5-4476-4afd-bedf-88c0092aea83', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b59b52e5-fcaf-4bc5-b1be-f9bfabb56768', N'2cf7920d-b019-4f5a-86c4-ddd849a04040', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyVehiclePlateCheckType'
									AND sttsm.[ID] = N'tblGasboyVehiclePlateCheckType')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'tblGasboyVehiclePlateCheckType', N'7167bd49-019e-4784-8228-d2d89549be29', N'e0eeed65-c033-493b-b53f-7e7473f85ccd', 19, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a2e9aee5-c3e7-4ac2-b633-14e37123545e', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyVehiclePlateCheckType', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyVehiclePlateCheckType', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e1a01ab9-601c-4720-aab4-d74e12b1a9ab', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'GasboyVehiclePlateCheckTypeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f49aa843-d9ce-438e-9a89-eef6fbc72515', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'GasboyVehiclePlateCheckTypeCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'da167b93-db6e-4204-b075-0332df83fc4f', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'GasboyVehiclePlateCheckTypeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'84c4c3f3-be6c-4092-853a-11e9190e09da', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'GasboyVehiclePlateCheckTypeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'102b2aa1-315b-4717-9b47-cab9f02528c6', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7290885d-ebaa-4d36-a36f-6fa65cb992f4', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'be7f1926-440c-4257-8682-9d95b9838581', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5a9a545f-7af0-453e-bcf2-edb1aa2beb68', N'7519d652-c2c4-40a5-adc1-77e8e6d92c68', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END


/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblExternalStation'
									AND sttsm.[ID] = N'tblExternalStation')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1935b85f-3935-434f-bb65-8395b140a60f', N'tblExternalStation', N'4a89a33f-8619-4bc3-b1a9-d94f4d492e1e', N'537b2472-6c50-4d67-8d78-6e3ec617e918', 2, 2, 0, 0, NULL, NULL, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'61556ef3-6f47-46e4-bf51-3f5c65306c4f', N'1935b85f-3935-434f-bb65-8395b140a60f', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStation', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStation', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStation', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStation', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStation', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStation', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStation', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStation', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fe629ca9-6e60-4d44-bc9b-264840b9266d', N'1935b85f-3935-434f-bb65-8395b140a60f', N'ExternalStationGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3904cefb-d2c8-496e-8b2c-f386f42622f1', N'1935b85f-3935-434f-bb65-8395b140a60f', N'SiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f3226d80-f5b6-4786-afa2-76bad0031a19', N'1935b85f-3935-434f-bb65-8395b140a60f', N'ID', 2, N'NVarChar', 50, 0, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'02996967-8151-4822-ae02-afa1ce9eee26', N'1935b85f-3935-434f-bb65-8395b140a60f', N'LookupExternalStationTypeIndex', 3, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c29c76ae-ffe1-49c5-914c-fffb4752dbaf', N'1935b85f-3935-434f-bb65-8395b140a60f', N'BillingID', 4, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9607d3a0-4f38-45f9-b1e7-783556f1abb0', N'1935b85f-3935-434f-bb65-8395b140a60f', N'DownloadTransactionsAutomatically', 5, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7fc9ccf4-d061-48a2-8bd4-749e640542a4', N'1935b85f-3935-434f-bb65-8395b140a60f', N'LookupExternalStationStatusIndex', 6, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f1750845-4330-4be7-9920-bb6e710991e6', N'1935b85f-3935-434f-bb65-8395b140a60f', N'LastSuccessfulConnection', 7, N'DateTimeOffset', 10, 34, 7, 1, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0ad15b14-d55e-442e-aa4a-e9ada11725c4', N'1935b85f-3935-434f-bb65-8395b140a60f', N'LastConnectionAttempt', 8, N'DateTimeOffset', 10, 34, 7, 1, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'173e9f2d-f9b0-4f79-8df1-4b90d60f6ab8', N'1935b85f-3935-434f-bb65-8395b140a60f', N'LastTransactionID', 9, N'BigInt', 8, 19, 0, 1, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a87420c1-0e5a-4b54-9d5e-e1f8e2265155', N'1935b85f-3935-434f-bb65-8395b140a60f', N'CreatedBy', 10, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c087658f-dbd4-4f8e-9250-7a1227f56fd1', N'1935b85f-3935-434f-bb65-8395b140a60f', N'CreatedDate', 11, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7916c3a8-d05c-46af-9c0d-487070172edf', N'1935b85f-3935-434f-bb65-8395b140a60f', N'UpdatedBy', 12, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'051e1705-edd2-45a5-9a6d-25debfb95aaa', N'1935b85f-3935-434f-bb65-8395b140a60f', N'UpdatedDate', 13, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 1:54:08 PM -05:00', NULL, N'3/8/2016 1:54:08 PM -05:00', NULL)
END

/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblExternalGasboyStation'
									AND sttsm.[ID] = N'tblExternalGasboyStation')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'tblExternalGasboyStation', N'99bab581-5bed-488a-b4f4-32e1ca6cebde', N'fe34de40-3bf5-424b-b91f-d0ca8b785470', 11, 2, 0, 0, NULL, NULL, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a5784773-aa63-4c09-80a0-4982ea301ed6', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalGasboyStation', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalGasboyStation', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalGasboyStation', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalGasboyStation', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalGasboyStation', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalGasboyStation', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalGasboyStation', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalGasboyStation', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f291bc79-fc78-4dba-bcdd-a8833a149936', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'ExternalStationGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'74273b46-3217-48d8-b0c0-ff46da04cd13', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'SiteCode', 1, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ec3797f9-576e-452e-992e-3b36ebc52a8b', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'IPAddress', 2, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'37a6355a-ebed-4c70-874a-897f1124b500', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'UserName', 3, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dd0a7ba5-3436-4142-9941-c4b0a7309f3b', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'Password', 4, N'VarBinary', 256, 0, 0, 1, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'35ebbd45-9335-4634-b2e4-6035694a0122', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'CreatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'64743983-2c72-40bd-aef8-f4606e27ee4a', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'CreatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5583b285-73c2-466c-a5a6-d10f1fc3d429', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'UpdatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'208519b4-f794-4bb1-b31f-40efcbcb9868', N'1ec3aebf-9d5c-4ca9-921d-9600b643cef3', N'UpdatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblExternalStationGeneralConfiguration'
									AND sttsm.[ID] = N'tblExternalStationGeneralConfiguration')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'tblExternalStationGeneralConfiguration', N'99bab581-5bed-488a-b4f4-32e1ca6cebde', N'a2538eae-243b-4bde-9f53-912f067de01a', 12, 2, 0, 0, NULL, NULL, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'416e2ff6-e548-44cc-b59c-38b7580b35dd', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationGeneralConfiguration', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2d8be9ae-2e7b-4ceb-b04b-6a52d7055ec3', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'ExternalStationGeneralConfigurationGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'43cc2060-38a9-427a-9b12-6f37acf890ad', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'SiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'de250aa3-5827-4c87-8398-ee8bd0aa1151', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'RetailSaleTransactionAliasGuid', 2, N'UniqueIdentifier', 16, 0, 0, 1, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5303c19f-80be-4de1-a73a-06bf36b480b7', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'DownloadTransactionsIntervalMinutes', 3, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'af5f651c-43d4-47bc-930d-02c6cc58df87', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'DownloadEventsIntervalMinutes', 4, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd5f34769-0d0f-40bb-b4a6-35f7220e553a', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'CreatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5c0927cd-f211-45ff-9885-48195227faa6', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'CreatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'52b218bb-09fa-4dcd-90cd-fcb044eb317b', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'UpdatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'32066f37-5cde-4c14-b123-1c466743593a', N'c2851ced-5a7f-4a9c-89e3-993cb185d644', N'UpdatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblExternalStationLog'
									AND sttsm.[ID] = N'tblExternalStationLog')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'tblExternalStationLog', N'99bab581-5bed-488a-b4f4-32e1ca6cebde', N'3519987c-a304-4803-9c6d-771a06733c1d', 13, 2, 0, 0, NULL, NULL, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd8515b75-1a77-4c94-bae8-a95bbb88e94f', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationLog', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationLog', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationLog', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationLog', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationLog', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationLog', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationLog', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationLog', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a06ca534-92aa-4806-9f13-c7ae97d5a057', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'ExternalStationLogGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ff96f432-de4e-48dc-a69d-58b371b88e80', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'SiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e413b314-87b1-4d40-b928-ba04f8b30c88', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'ExternalStationGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2373f98c-cbd1-4635-8e61-afef4c4510ed', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'LogText', 3, N'NVarChar', -1, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'27e66268-e68f-48fd-8694-99148bc3f489', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'LookupExternalStationLogTypeIndex', 4, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'500c2338-67ee-45ec-b5bc-12a6bd63d03b', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'LogDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c570ce7a-2e3f-4f73-a468-17b87eafd52e', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'CreatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a48798b8-6ef1-491e-ab27-c692b08a27ae', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'CreatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5ffbdc33-d841-4972-b37b-945482a6ed33', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'UpdatedBy', 8, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2387bc64-ddfa-47f7-a27e-897d64f379a7', N'f89186cb-61e2-4a90-9415-3eee9a3f3437', N'UpdatedDate', 9, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblExternalStationTransaction'
									AND sttsm.[ID] = N'tblExternalStationTransaction')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b29e7b89-725c-4351-921e-0e65a829e10f', N'tblExternalStationTransaction', N'99bab581-5bed-488a-b4f4-32e1ca6cebde', N'17fa1676-388a-4149-beac-8794565f158c', 14, 2, 0, 0, NULL, NULL, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bf31ae27-6ee9-4bf9-9397-94be98e591c4', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationTransaction', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationTransaction', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationTransaction', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationTransaction', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationTransaction', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationTransaction', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationTransaction', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationTransaction', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1fcbc9ca-52a0-4a0f-8c8f-9eb485847169', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'ExternalStationTransactionGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e72abf87-6d8f-40ab-9163-edb2a2514448', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'ExternalStationGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'eacd3642-be2a-485e-8182-21021c4b4fad', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'SiteGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6c0ba3e7-b0a9-43f5-aaa6-66c9c4c16402', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'StationTransactionID', 3, N'NVarChar', 20, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9b09f829-8b7a-422e-9ce9-5ca5465f21d5', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'RawTransactionData', 4, N'NVarChar', -1, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fb9f4d5a-f489-4d56-812e-c388853be00b', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'CreatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'697467fb-2a41-4ebc-9f36-37733f25412f', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'CreatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'808879a0-e498-4797-a308-a5063fa7d4f9', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'UpdatedBy', 7, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'40cd78bf-5bd3-4ebb-b68a-7d42b65cc3c7', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'UpdatedDate', 8, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'90a9c86d-ab37-41ce-800b-5d5fc3085a41', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'LookupExternalStationTransactionStatusIndex', 10, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e672c571-4029-4f2e-9551-3d902594b538', N'b29e7b89-725c-4351-921e-0e65a829e10f', N'LookupExternalStationTransactionFailedStatusIndex', 11, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblEntityExternalStationToSite'
									AND sttsm.[ID] = N'tblEntityExternalStationToSite')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'tblEntityExternalStationToSite', N'99bab581-5bed-488a-b4f4-32e1ca6cebde', N'4cb5b4b8-cf70-4217-8521-23190df7cb3f', 15, 2, 0, 0, NULL, NULL, N'3/8/2016 2:03:40 PM -05:00', NULL, N'3/8/2016 2:03:40 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'24d605e7-455a-4655-b252-152a7b4c671f', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblEntityExternalStationToSite', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblEntityExternalStationToSite', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0e4d2764-8f37-4fe2-b0a1-99195fec36cb', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'ExternalStationToSiteGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f8a829f4-d0bc-4567-b31a-1187acbba17f', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'ExternalStationGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a1350b83-4275-4c73-bb60-32b2882b1935', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'SiteGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1d25a6d2-83ef-4884-a6c6-cd180075ae4d', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'AssignedFromSiteGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ae2169b5-7832-4a48-8a0c-105691451d15', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dbd482e2-1b06-4b2c-8a15-3c6f73caeead', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'82d30218-22a0-464e-a414-55cdb8df2b5b', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'43174f54-0ccd-43e7-8da1-bec7fa112bde', N'b09f0c6c-334d-4aea-99b3-8d5cd1060d46', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblExternalStationToProduct'
									AND sttsm.[ID] = N'tblExternalStationToProduct')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'tblExternalStationToProduct', N'99bab581-5bed-488a-b4f4-32e1ca6cebde', N'9611564d-b201-4b8b-a3cd-2825b4668fa1', 16, 2, 0, 0, NULL, NULL, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'50761131-397d-4ec4-8f4a-0dd721357cf0', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationToProduct', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationToProduct', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationToProduct', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationToProduct', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationToProduct', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationToProduct', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationToProduct', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationToProduct', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a7828f97-0bf9-40c1-b419-daa92d5b06b7', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'ExternalStationToProductGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a3d4d57f-8fdc-44a9-bf05-14308416d4a4', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'ExternalStationGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'61a1b201-418c-4ce3-9c38-40b5d6713e2d', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'ExternalStationProduct', 2, N'NVarChar', 50, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a367ff78-226a-499a-8cb3-d5b962d681db', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'ProductGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1cccef9b-a7bd-41da-a42c-da6030bd4f3e', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'cf050cd7-9dd1-4764-97d2-d97707037d8a', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'f9200b26-5bd0-4833-90fd-4d89c201341d', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4e51d2c1-384e-4edb-b574-839b87d1c1c6', N'abe1b566-af26-4577-b436-b5c88dc97f8e', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:03:41 PM -05:00', NULL, N'3/8/2016 2:03:41 PM -05:00', NULL)
END


/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblExternalStationTransactionError'
									AND sttsm.[ID] = N'tblExternalStationTransactionError')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'241e9cab-2924-4ca6-b588-b062a9084920', N'tblExternalStationTransactionError', N'c320ffce-18f6-47a5-93ca-676b78e18fea', N'f5dce07a-dab7-4ef9-b59d-b95cd93658b2', 13, 2, 0, 0, NULL, NULL, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c4c86fee-3e29-4312-a447-069f80f7ed94', N'241e9cab-2924-4ca6-b588-b062a9084920', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblExternalStationTransactionError', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblExternalStationTransactionError', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblExternalStationTransactionError', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblExternalStationTransactionError', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblExternalStationTransactionError', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblExternalStationTransactionError', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblExternalStationTransactionError', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblExternalStationTransactionError', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8966b58a-a13c-44e3-a901-5ad2c218faea', N'241e9cab-2924-4ca6-b588-b062a9084920', N'ExternalStationTransactionErrorGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b7ac5204-f452-44b8-84f9-f66dbef774d7', N'241e9cab-2924-4ca6-b588-b062a9084920', N'ExternalStationTransactionGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1d8b144a-5f84-4580-af6f-f53e6825c4fe', N'241e9cab-2924-4ca6-b588-b062a9084920', N'Error', 2, N'NVarChar', 1000, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'48c16c01-2c81-4407-bc0f-b59d25ba6f6b', N'241e9cab-2924-4ca6-b588-b062a9084920', N'CreatedBy', 3, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'687bb0e2-cb27-417d-8db2-e9f9fec26a65', N'241e9cab-2924-4ca6-b588-b062a9084920', N'CreatedDate', 4, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8a122e22-86fd-426d-9af9-1e6545e43961', N'241e9cab-2924-4ca6-b588-b062a9084920', N'UpdatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5ff37826-d536-4590-a6cb-c7a5cf674d64', N'241e9cab-2924-4ca6-b588-b062a9084920', N'UpdatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblGasboyDepartment'
									AND sttsm.[ID] = N'tblGasboyDepartment')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'tblGasboyDepartment', N'c320ffce-18f6-47a5-93ca-676b78e18fea', N'911162b1-d595-4694-819a-085286ec4ba5', 14, 2, 0, 0, NULL, NULL, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2173c721-8534-46d7-8abc-076d6aab88b3', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyDepartment', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyDepartment', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyDepartment', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyDepartment', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyDepartment', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyDepartment', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyDepartment', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyDepartment', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4e713e9e-771c-4922-a869-be50a25bc0b3', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'GasboyDepartmentGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2b4d7c5d-c16f-4207-a316-6ab8711d8329', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'SiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'369defbc-88d5-4c0d-ada1-8ff8d18e0f8c', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'DepartmentCode', 2, N'BigInt', 8, 19, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7379d945-c1c2-42cd-b15e-27a10ed59e86', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'DepartmentName', 3, N'NVarChar', 50, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0cc01943-4dfc-4a76-96ce-d29b050c6cd5', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'GroupRuleName', 4, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4ca54286-f4f6-4d30-b2ee-149c6cd5a2c9', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'PriceListName', 5, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'60ee1a7b-e6c7-4ea6-be97-a55611d994dc', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'LookupGasboyRecordStatusIndex', 6, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9d6db531-8098-49c7-8387-c88b5796289d', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'UsePINCodeFlag', 7, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'984db522-effd-4dc8-925e-2c73636cd67f', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'PINCode', 8, N'VarBinary', 256, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3442a4be-db3d-499f-8ede-a5c9d23bd709', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'AuthPINFrom', 9, N'TinyInt', 1, 3, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd53750e8-b945-40a0-9b08-d348f0d4f57f', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'PromptForVehiclePlateFlag', 10, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'578ebc16-6973-4b59-8845-a1e33403bc45', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'LookupGasboyVehiclePlateCheckTypeIndex', 11, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'366b0fcc-7186-4f40-a71b-2761859aebed', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'AlwaysPromptForAdditionalValidationFlag', 12, N'TinyInt', 1, 3, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a38befa7-f851-4c1e-92b6-732a395c2f89', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'CreatedBy', 13, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'af689f16-4651-4aeb-b848-c2141b745362', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'CreatedDate', 14, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9117715a-8fe2-4dfb-8456-ff3d7f3413c0', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'UpdatedBy', 15, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a319169b-15d8-4715-88d9-5ccf04068eeb', N'1de6f479-b7a4-493c-ba7b-4e3ebe45953d', N'UpdatedDate', 16, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblGasboyFleet'
									AND sttsm.[ID] = N'tblGasboyFleet')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a6272264-3881-469e-a231-efeaf95f698d', N'tblGasboyFleet', N'c320ffce-18f6-47a5-93ca-676b78e18fea', N'e58359ee-81a1-4c3e-b040-0bbf158fd605', 15, 2, 0, 0, NULL, NULL, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1b0909e5-d38a-4d7f-9949-8a1f6dde981d', N'a6272264-3881-469e-a231-efeaf95f698d', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyFleet', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyFleet', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyFleet', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyFleet', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyFleet', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyFleet', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyFleet', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyFleet', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'777d160d-0621-4c0a-83b3-66c140902b6b', N'a6272264-3881-469e-a231-efeaf95f698d', N'GasboyFleetGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'63214ede-7e03-4d57-97dd-768866a2ab00', N'a6272264-3881-469e-a231-efeaf95f698d', N'SiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2e802e2b-cf42-4a2a-9a77-530b42fcd077', N'a6272264-3881-469e-a231-efeaf95f698d', N'FleetCode', 2, N'BigInt', 8, 19, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'21262c61-69b2-448f-a915-dcb01e11525a', N'a6272264-3881-469e-a231-efeaf95f698d', N'FleetName', 3, N'NVarChar', 50, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'110287c0-be1a-4e23-bf2d-0a20c16b65cf', N'a6272264-3881-469e-a231-efeaf95f698d', N'GroupRuleName', 4, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b60aec2f-9dcc-49af-841a-bbb199e10f1d', N'a6272264-3881-469e-a231-efeaf95f698d', N'PriceListName', 5, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c9968459-8b0c-45b2-b12d-fc9e8096ca3b', N'a6272264-3881-469e-a231-efeaf95f698d', N'LookupGasboyRecordStatusIndex', 6, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0c926099-39d7-4869-928f-52a925710f35', N'a6272264-3881-469e-a231-efeaf95f698d', N'UsePINCodeFlag', 7, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9ee7105a-2775-4498-83ed-9e12e185638c', N'a6272264-3881-469e-a231-efeaf95f698d', N'PINCode', 8, N'VarBinary', 256, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9d015299-665b-4c0b-a154-5f7f5486ab78', N'a6272264-3881-469e-a231-efeaf95f698d', N'AuthPINFrom', 9, N'TinyInt', 1, 3, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5c341a22-4a09-4230-9a0e-8b5fc816a4c9', N'a6272264-3881-469e-a231-efeaf95f698d', N'PromptForVehiclePlateFlag', 10, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd132b238-9538-468b-bc4e-c8431008417c', N'a6272264-3881-469e-a231-efeaf95f698d', N'LookupGasboyVehiclePlateCheckTypeIndex', 11, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3e9dc21d-5403-4baf-939f-34ca10c6088d', N'a6272264-3881-469e-a231-efeaf95f698d', N'AlwaysPromptForAdditionalValidationFlag', 12, N'TinyInt', 1, 3, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'479cab7f-b943-4324-b353-aa75e2ce02c9', N'a6272264-3881-469e-a231-efeaf95f698d', N'CreatedBy', 13, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'685b382f-750d-4427-81f8-de744b94470f', N'a6272264-3881-469e-a231-efeaf95f698d', N'CreatedDate', 14, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'61eabddf-b677-4847-b240-93bfbe657c83', N'a6272264-3881-469e-a231-efeaf95f698d', N'UpdatedBy', 15, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'367bba2e-7084-4917-ba7c-ed9bd7ef5d0f', N'a6272264-3881-469e-a231-efeaf95f698d', N'UpdatedDate', 16, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblGasboyStationEvent'
									AND sttsm.[ID] = N'tblGasboyStationEvent')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7097b95c-62a7-4f43-abb7-74834e451023', N'tblGasboyStationEvent', N'c320ffce-18f6-47a5-93ca-676b78e18fea', N'6468f691-86bb-4e01-9ae7-e6452824dd03', 16, 2, 0, 0, NULL, NULL, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'88219c33-c619-4357-949e-bac9c33a0981', N'7097b95c-62a7-4f43-abb7-74834e451023', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyStationEvent', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyStationEvent', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyStationEvent', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyStationEvent', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyStationEvent', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyStationEvent', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyStationEvent', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyStationEvent', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd9c423a2-1078-4934-9bb4-6e6dc86f9046', N'7097b95c-62a7-4f43-abb7-74834e451023', N'GasboyStationEventGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'76f08cdc-dc7a-4253-865c-dfca73073f8f', N'7097b95c-62a7-4f43-abb7-74834e451023', N'ExternalStationLogGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ff8721d0-8e4e-4bba-9513-3b70d810c457', N'7097b95c-62a7-4f43-abb7-74834e451023', N'EventID', 2, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0ee1d4a0-bbc7-4b74-b5d5-84312cb9ffd5', N'7097b95c-62a7-4f43-abb7-74834e451023', N'LookupGasboyEventErrorClassCodeIndex', 3, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5db4f792-f0ba-422d-ac73-4b1a7ab74165', N'7097b95c-62a7-4f43-abb7-74834e451023', N'ErrorCode', 4, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a20f8e42-d625-4cd9-b664-9186202841b9', N'7097b95c-62a7-4f43-abb7-74834e451023', N'FleetID', 5, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a19b49e9-59fb-4184-b81d-d845d2b20227', N'7097b95c-62a7-4f43-abb7-74834e451023', N'ObjectID', 6, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e20862e9-b1cd-4fc6-bf02-28694e354b8f', N'7097b95c-62a7-4f43-abb7-74834e451023', N'LookupGasboyEventObjectTypeIndex', 7, N'Int', 4, 10, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'8c47ac5a-6d33-4785-9acd-4e79d7ac2d83', N'7097b95c-62a7-4f43-abb7-74834e451023', N'DeviceName', 8, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a920d62e-ce0f-4ad1-af79-59bf43b12e58', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field1', 9, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'df036aa1-83d2-443e-beab-90bdc7bcb1bc', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field2', 10, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'371f3efc-f709-47ab-8c14-0d3121795a7b', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field3', 11, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bc1909d8-9fb0-49c0-afdb-d9df2ba49e75', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field4', 12, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd958cdf3-275b-4f67-9ef8-1ce6d9419864', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field5', 13, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'af054b8a-41d4-4540-9a75-ee6430cc29e8', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field6', 14, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c55359ae-74c8-4e9c-a87b-0879bebf5a51', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field7', 15, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b7cd7ce7-d8ca-4afc-ba72-ffbe4271878d', N'7097b95c-62a7-4f43-abb7-74834e451023', N'Field8', 16, N'NVarChar', 100, 0, 0, 1, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5040c3a5-ba6e-4fd4-8cbf-50a5af5718be', N'7097b95c-62a7-4f43-abb7-74834e451023', N'CreatedBy', 17, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'61288083-d76c-4baf-b208-b4dd967dceb6', N'7097b95c-62a7-4f43-abb7-74834e451023', N'CreatedDate', 18, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c8e96cec-c59b-4719-87e6-75074d27fc96', N'7097b95c-62a7-4f43-abb7-74834e451023', N'UpdatedBy', 19, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3fa8b5a3-eb7a-4115-ac94-6f292a9beeb2', N'7097b95c-62a7-4f43-abb7-74834e451023', N'UpdatedDate', 20, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:04:53 PM -05:00', NULL, N'3/8/2016 2:04:53 PM -05:00', NULL)
END



/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblGasboyDevice'
									AND sttsm.[ID] = N'tblGasboyDevice')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'80e966fa-af49-41ca-8834-3747ba34821f', N'tblGasboyDevice', N'184bdc35-fab9-49c0-bc53-f5475696879a', N'b66a7d3d-5755-40c8-b53a-05427682247e', 2, 2, 0, 0, NULL, NULL, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5ae4f690-1750-4170-aeb0-3bc8dc5cfa9b', N'80e966fa-af49-41ca-8834-3747ba34821f', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyDevice', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyDevice', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyDevice', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyDevice', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyDevice', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyDevice', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyDevice', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyDevice', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bb90f630-0198-46fe-9494-9c8d72d31ae4', N'80e966fa-af49-41ca-8834-3747ba34821f', N'GasboyDeviceGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5dab7922-8ede-4020-9eaf-94f5459721e9', N'80e966fa-af49-41ca-8834-3747ba34821f', N'SiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'582dfde1-427f-44f6-99db-6c206995ba4f', N'80e966fa-af49-41ca-8834-3747ba34821f', N'GasboyDepartmentGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'66d8d6bf-459d-41ef-923a-225a6d94a60d', N'80e966fa-af49-41ca-8834-3747ba34821f', N'DeviceCode', 3, N'BigInt', 8, 19, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7d577e39-7d1d-4088-b1b1-e2921cb22dc3', N'80e966fa-af49-41ca-8834-3747ba34821f', N'DeviceName', 4, N'NVarChar', 50, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b78c0200-2d8b-49c3-a1c1-2b46a4979405', N'80e966fa-af49-41ca-8834-3747ba34821f', N'CardNumber', 5, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3f6da4fd-a04a-4faa-9e9c-e296be358f2d', N'80e966fa-af49-41ca-8834-3747ba34821f', N'GroupRuleName', 6, N'NVarChar', 50, 0, 0, 1, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4b875e42-7968-4e2d-8a85-799f94e2fd32', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyDeviceTypeIndex', 7, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ecdae699-1606-4300-8cf9-8570056698ff', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyRecordStatusIndex', 8, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4afde7ca-dfad-4e94-a227-19f70daae1fa', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyHardwareTypeIndex', 9, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7be79dcd-56de-4895-877e-8099c0d4121f', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyAuthTypeIndex', 10, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b53d4859-5e34-4b65-b73e-6991f9c8d6d1', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyEmployeeTypeIndex', 11, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0cb9dab2-8689-4643-9f89-37b1fc427c3d', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyTwoStageDriverValidationTypeIndex', 12, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'84af21d5-772b-4cf2-93ac-7dcf863dbd18', N'80e966fa-af49-41ca-8834-3747ba34821f', N'UsePINCodeFlag', 13, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7ce6237f-9611-45e9-aec3-340f4ceeed2b', N'80e966fa-af49-41ca-8834-3747ba34821f', N'PINCode', 14, N'VarBinary', 256, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5920ef1d-574a-4aab-b6fe-6485205b3236', N'80e966fa-af49-41ca-8834-3747ba34821f', N'AuthPINFrom', 15, N'TinyInt', 1, 3, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bab08e8b-f3fa-4e12-9131-4ee0396e63ff', N'80e966fa-af49-41ca-8834-3747ba34821f', N'VehiclePlate', 16, N'NVarChar', 50, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fa4af34a-bf22-4070-8ff4-80f3a4d0994c', N'80e966fa-af49-41ca-8834-3747ba34821f', N'PromptForVehiclePlateFlag', 17, N'Bit', 1, 1, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'90fe4dbb-28b4-4f44-a3b8-8dfb150a2374', N'80e966fa-af49-41ca-8834-3747ba34821f', N'LookupGasboyVehiclePlateCheckTypeIndex', 18, N'Int', 4, 10, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1e5982db-b73f-4ddd-951c-b1fb2e426ef3', N'80e966fa-af49-41ca-8834-3747ba34821f', N'AlwaysPromptForAdditionalValidationFlag', 19, N'TinyInt', 1, 3, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2a4c0476-d6d0-4123-8e1e-0ee130101d67', N'80e966fa-af49-41ca-8834-3747ba34821f', N'CreatedBy', 20, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'346b7217-70b5-4b97-9b7c-ba6d2abd4f7c', N'80e966fa-af49-41ca-8834-3747ba34821f', N'CreatedDate', 21, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fd3897a0-c0bf-40b5-bec7-51670842e24d', N'80e966fa-af49-41ca-8834-3747ba34821f', N'UpdatedBy', 22, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'c12525ce-c798-47f6-9fde-e2cabcb99be9', N'80e966fa-af49-41ca-8834-3747ba34821f', N'UpdatedDate', 23, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'dbo.tblGasboyStationGeneralConfiguration'
									AND sttsm.[ID] = N'tblGasboyStationGeneralConfiguration')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'tblGasboyStationGeneralConfiguration', N'184bdc35-fab9-49c0-bc53-f5475696879a', N'3d971fd3-e796-4621-bd74-01113af680d2', 3, 2, 0, 0, NULL, NULL, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'bbfa8888-75f1-452f-ba08-a674f514718a', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyStationGeneralConfiguration', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyStationGeneralConfiguration', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5a29fb5b-da90-4b47-a52d-9885cf4e08eb', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'ExternalStationGeneralConfigurationGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'92952be5-a16c-40f0-ab88-0326c7e984b1', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'DefaultGasboyFleetGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e62822ec-139c-4a7a-8331-0f2f0da2d1ae', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'DefaultGasboyDepartmentGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'92c87017-5269-4a37-85fe-18df6bda85c1', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'CreatedBy', 3, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'638ed70f-58a9-467b-b1dc-dafb09fc99f3', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'CreatedDate', 4, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ad41b220-ab2a-4b9e-a922-8199408cbb75', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'UpdatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0c193ea0-7dc8-4ecc-9e9b-2c61b4c50d27', N'e372ecf2-7ca4-40bc-9cce-33bc33676b69', N'UpdatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblEntityGasboyDepartmentToSite'
									AND sttsm.[ID] = N'tblEntityGasboyDepartmentToSite')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'tblEntityGasboyDepartmentToSite', N'184bdc35-fab9-49c0-bc53-f5475696879a', N'9c20acfb-fede-45b3-a740-a7b1d2d1e76f', 4, 2, 0, 0, NULL, NULL, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a995abf3-d0c4-4c57-beba-9da9d31e2343', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblEntityGasboyDepartmentToSite', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblEntityGasboyDepartmentToSite', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'6422bdf8-b201-4cb6-8215-612c3781489c', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'GasboyDepartmentToSiteGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'e0bf6a97-ff74-4cd3-8bcc-47d9b26aa20b', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'GasboyDepartmentGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'9a8bab3c-aa7e-42b2-a6a4-47b26343b9a2', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'SiteGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b47bbe83-ed3e-4937-bd02-8ee65efdb077', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'AssignedFromSiteGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'2e75a900-b0a4-4faa-a584-85a5e0989aa9', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0d5ff67f-b8a2-4d75-8cf9-a13845dac07e', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4d771e8c-0802-4f9f-94de-04a1140889a2', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'08ffafbd-afad-4046-9b80-7584881ada34', N'07c98bdc-b93d-491e-9cc7-b912d835d827', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblEntityGasboyFleetToSite'
									AND sttsm.[ID] = N'tblEntityGasboyFleetToSite')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'tblEntityGasboyFleetToSite', N'184bdc35-fab9-49c0-bc53-f5475696879a', N'978c6f3f-2bc7-4bef-a465-e0d2c6dce3ff', 5, 2, 0, 0, NULL, NULL, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'139a7c30-f31c-4491-bb87-7a3ccd1bfb79', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblEntityGasboyFleetToSite', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblEntityGasboyFleetToSite', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ca3b92ca-7198-4c99-8a08-8200227210fd', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'GasboyFleetToSiteGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'516994a6-051d-409b-8cf4-b84d6f4869f4', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'GasboyFleetGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'5d44067b-8c6c-4e9d-9f52-bde1aa4d0801', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'SiteGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b56909be-8d9d-441d-9210-e28ab5daaca2', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'AssignedFromSiteGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ee47abcd-29e2-4fdf-8632-f84ed3c75648', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'aca963ab-f27a-4279-88e3-fcc1e0761e18', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a364265f-7169-49fb-b84b-626fb7b0d74e', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4dc7edf4-da7d-495d-af95-cfed6f27d577', N'3cefb267-ab82-4a46-9ec0-4ef9cdbb381c', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblGasboyDepartmentToGasboyFleet'
									AND sttsm.[ID] = N'tblGasboyDepartmentToGasboyFleet')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7b28fc01-049a-4999-aa63-16508c69c14b', N'tblGasboyDepartmentToGasboyFleet', N'184bdc35-fab9-49c0-bc53-f5475696879a', N'68122fdf-6d31-4b6a-944d-560c4d77062e', 6, 2, 0, 0, NULL, NULL, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'523b6376-67bc-42c6-9ea5-6ee4362b43ad', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyDepartmentToGasboyFleet', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyDepartmentToGasboyFleet', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dcebfbcc-01fc-49cf-bfdd-004128fd1bb5', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'GasboyDepartmentToGasboyFleetGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b2fb4153-5da5-48c4-94e8-1678c89f4dec', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'GasboyFleetGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'75537aba-b6ba-4893-b65d-22816734d979', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'GasboyDepartmentGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'844c2bf3-3ca3-42f2-ac10-bca4ac8b4176', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'CreatedBy', 3, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'0925e05b-8622-46dc-abb3-75090ba548f3', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'CreatedDate', 4, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3cb6d499-e2ae-43d9-9e8e-f84c38fad568', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'UpdatedBy', 5, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'd5decd0f-1174-49f5-8e6b-4f34d25cbd47', N'7b28fc01-049a-4999-aa63-16508c69c14b', N'UpdatedDate', 6, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:05:55 PM -05:00', NULL, N'3/8/2016 2:05:55 PM -05:00', NULL)
END



/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblEntityGasboyDeviceToSite'
									AND sttsm.[ID] = N'tblEntityGasboyDeviceToSite')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'tblEntityGasboyDeviceToSite', N'50c4b179-cf13-4671-bacf-94c7d5fff9c8', N'9489c2a7-bebe-4fd0-bb1f-021d40ce7977', 8, 2, 0, 0, NULL, NULL, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'ee5b513d-7f82-462d-a67c-b86082962b7e', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblEntityGasboyDeviceToSite', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblEntityGasboyDeviceToSite', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'032779b2-7e92-4e72-8088-017bb3f38129', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'GasboyDeviceToSiteGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'FC31D1CB-AC28-40E2-9A1A-45F2BC12FDE6', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'OwnerSiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
	INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3FF98F6E-6741-4BC1-A300-2A1710702EC0', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'MapToSiteGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dd05e18e-971c-4393-8e46-d0cdaae65ef8', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'AssignedFromSiteGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fea32312-6944-4e9c-a814-fce8312c12ae', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'660d9a71-523e-4faa-8817-97962a976543', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b880b23c-eeb8-4367-8160-6a4fa9756a42', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7dcb9113-2b9c-4482-9884-060d46bca3cc', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
END

/* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP - Tables} */
-- If the SyncTableToScope Mapping is old we need to modify it.
IF EXISTS (SELECT SyncTableToScopeMapColumnGuid FROM [sync].[tblSyncTableToScopeMapColumn] sc 
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON sc.[SyncTableToScopeMapGuid] = sttsm.[SyncTableToScopeMapGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'map.tblEntityGasboyDeviceToSite'
									AND sttsm.[ID] = N'tblEntityGasboyDeviceToSite'
									AND [ColumnName] = 'SiteGuid')
BEGIN

	DELETE FROM [sync].[tblSyncTableToScopeMapColumn] WHERE [SyncTableToScopeMapGuid] = '7046b29e-94dc-4013-9152-52d04ccf82f0'

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'032779b2-7e92-4e72-8088-017bb3f38129', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'GasboyDeviceToSiteGuid', 0, N'UniqueIdentifier', 16, 0, 0, 0, 1, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'FC31D1CB-AC28-40E2-9A1A-45F2BC12FDE6', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'OwnerSiteGuid', 1, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
	INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3FF98F6E-6741-4BC1-A300-2A1710702EC0', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'MapToSiteGuid', 2, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'dd05e18e-971c-4393-8e46-d0cdaae65ef8', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'AssignedFromSiteGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'fea32312-6944-4e9c-a814-fce8312c12ae', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'660d9a71-523e-4faa-8817-97962a976543', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'b880b23c-eeb8-4367-8160-6a4fa9756a42', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'7dcb9113-2b9c-4482-9884-060d46bca3cc', N'7046b29e-94dc-4013-9152-52d04ccf82f0', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:06:34 PM -05:00', NULL, N'3/8/2016 2:06:34 PM -05:00', NULL)
END

-- If the SyncTableToScope Mapping Does Not Exist, we need to create it.
IF NOT EXISTS (SELECT 1 FROM (SELECT SyncProfileGuid FROM [sync].[tblSyncProfile] WHERE ID = '{Complete}') sp
								INNER JOIN [sync].[tblSyncScope] ss
									ON sp.[SyncProfileGuid] = ss.[SyncProfileGuid]
								INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
									ON ss.[SyncScopeGuid] = sttsm.[SyncScopeGuid]
								INNER JOIN [sync].[tblSyncTable] st
									ON sttsm.[SyncTableGuid] = st.[SyncTableGuid]
							WHERE st.[TableName] = N'lookup.tblGasboyErrorCode'
									AND sttsm.[ID] = N'tblGasboyErrorCode')
BEGIN
    INSERT INTO [sync].[tblSyncTableToScopeMap] ([SyncTableToScopeMapGuid], [ID], [SyncScopeGuid], [SyncTableGuid], [SyncOrder], [SyncDirection], [MaxBatchSegmentRowCount], [MaxTransferSegmentKB], [AdditionalFilterJoinClause], [AdditionalFilterWhereClause], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'tblGasboyErrorCode', N'7167bd49-019e-4784-8228-d2d89549be29', N'031EACEC-B99E-4152-8D23-79C562C0F96D', 20, 0, 0, 0, NULL, NULL, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP COMMANDS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapCommand] ([SyncTableToScopeMapCommandGuid], [SyncTableToScopeMapGuid], [SelectIncrementalInserts], [ApplyIncrementalInserts], [SelectIncrementalUpdates], [ApplyIncrementalUpdates], [SelectIncrementalDeletes], [ApplyIncrementalDeletes], [SelectUpdateConflicts], [SelectDeleteConflicts], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'a2840201-dcd5-48e9-ac0e-416c68289823', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'sync.gsp_[NodeType]SelectIncrementalInserts_tblGasboyErrorCode', N'sync.gsp_[NodeType]ApplyIncrementalInserts_tblGasboyErrorCode', N'sync.gsp_[NodeType]SelectIncrementalUpdates_tblGasboyErrorCode', N'sync.gsp_[NodeType]ApplyIncrementalUpdates_tblGasboyErrorCode', N'sync.gsp_[NodeType]SelectIncrementalDeletes_tblGasboyErrorCode', N'sync.gsp_[NodeType]ApplyIncrementalDeletes_tblGasboyErrorCode', N'sync.gsp_[NodeType]SelectUpdateConflicts_tblGasboyErrorCode', N'sync.gsp_[NodeType]SelectDeleteConflicts_tblGasboyErrorCode', N'1/1/0001 12:00:00 AM +00:00', NULL, N'1/1/0001 12:00:00 AM +00:00', NULL)

    /* {CheckPoint: INSERTING DEFAULT SYNCHRONIZATION TABLE TO SCOPE MAP SELECTED COLUMNS - Tables} */
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'3CA30AF9-62DB-4837-87DA-7074C2888B0C', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'GasboyErrorCodeIndex', 0, N'Int', 4, 10, 0, 0, 1, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1FFD5E28-BDBE-4D07-AEE5-35D72D84DEEB', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'GasboyErrorCode', 1, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'461FE3D6-D53C-4938-A057-CED3C2FE1EE8', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'GasboyErrorCodeName', 2, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'CFDBFE09-F136-4164-8241-B6CAEEA0F716', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'GasboyErrorCodeGuid', 3, N'UniqueIdentifier', 16, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'273DE01C-1E14-4805-A63D-BF5778B559B3', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'CreatedBy', 4, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'1146964B-8C7B-4717-BFC3-80C8C850E771', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'CreatedDate', 5, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'AAD08304-895E-4979-B42C-AF603184B7EE', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'UpdatedBy', 6, N'NVarChar', 100, 0, 0, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
    INSERT INTO [sync].[tblSyncTableToScopeMapColumn] ([SyncTableToScopeMapColumnGuid], [SyncTableToScopeMapGuid], [ColumnName], [ColumnIndex], [ColumnType], [ColumnSize], [ColumnPrecision], [ColumnScale], [IsNullableFlag], [IsPrimaryKeyMemberFlag], [IsIdentityColumnFlag], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'696EE6E8-7059-4EE3-83CF-913B482E9945', N'FFF795B8-7D2A-4E48-81BE-43AD00F2E2A8', N'UpdatedDate', 7, N'DateTimeOffset', 10, 34, 7, 0, 0, 0, N'3/8/2016 2:01:31 PM -05:00', NULL, N'3/8/2016 2:01:31 PM -05:00', NULL)
END



IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyFleet] WHERE GasboyFleetGuid = N'00000009-0000-0000-0000-000000000000')
BEGIN
	UPDATE [dbo].[tblGasboyFleet] SET 
		SiteGuid = N'00000000-0000-0000-0000-000000000001'
		, FleetCode = 1
		, FleetName = 'Fleet'
		, LookupGasboyRecordStatusIndex = 2
		, UsePINCodeFlag = 0
		, PINCode =  CONVERT(VARBINARY(25), '0x', 1)
		, AuthPINFrom = 1
		, PromptForVehiclePlateFlag = 0
		, LookupGasboyVehiclePlateCheckTypeIndex = 1
		, AlwaysPromptForAdditionalValidationFlag = 0
		, CreatedBy = 'Administrator'
		, CreatedDate = '2017-02-02 00:00:00.0000000 -04:00'
		, UpdatedBy = 'Administrator'
		, UpdatedDate = '2017-02-02 00:00:00.0000000 -04:00'
		, FleetID = 900000001
	WHERE GasboyFleetGuid = N'00000009-0000-0000-0000-000000000000'
END
ELSE
BEGIN
	INSERT INTO dbo.tblGasboyFleet ( GasboyFleetGuid, SiteGuid, FleetCode, FleetName, LookupGasboyRecordStatusIndex, UsePINCodeFlag, PINCode, AuthPINFrom, PromptForVehiclePlateFlag, LookupGasboyVehiclePlateCheckTypeIndex, AlwaysPromptForAdditionalValidationFlag, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, FleetID)
	VALUES (N'00000009-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', 1, 'Default Fleet', 2, 0,  CONVERT(VARBINARY(25), '0x', 1), 1, 0, 1, 0, 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 900000001)
END

IF EXISTS (SELECT 1 FROM [dbo].[tblGasboyDepartment] WHERE GasboyDepartmentGuid = N'00000001-0000-0000-0000-000000000000')
BEGIN
	UPDATE [dbo].[tblGasboyDepartment] SET
		SiteGuid = N'00000000-0000-0000-0000-000000000001'
		, DepartmentCode = 9999
		, DepartmentName = 'Blacklist Department'
		, LookupGasboyRecordStatusIndex = 2
		, UsePINCodeFlag = 0
		, PINCode = CONVERT(VARBINARY(25), '0x', 1)
		, AuthPINFrom = 1
		, PromptForVehiclePlateFlag = 0
		, LookupGasboyVehiclePlateCheckTypeIndex = 3
		, AlwaysPromptForAdditionalValidationFlag = 0
		, CreatedBy = 'Administrator'
		, CreatedDate = '2017-02-02 00:00:00.0000000 -04:00'
		, UpdatedBy = 'Administrator'
		, UpdatedDate = '2017-02-02 00:00:00.0000000 -04:00'
		, DepartmentID = 900000003
	WHERE GasboyDepartmentGuid = N'00000001-0000-0000-0000-000000000000'
END
ELSE
BEGIN
	INSERT INTO dbo.tblGasboyDepartment (GasboyDepartmentGuid, SiteGuid, DepartmentCode, DepartmentName, LookupGasboyRecordStatusIndex, UsePINCodeFlag, PINCode, AuthPINFrom, PromptForVehiclePlateFlag, LookupGasboyVehiclePlateCheckTypeIndex, AlwaysPromptForAdditionalValidationFlag, CreatedBy, CreatedDate, UpdatedBy, UpdatedDate, DepartmentID)
	VALUES (N'00000001-0000-0000-0000-000000000000', N'00000000-0000-0000-0000-000000000001', 9999, 'Blacklist Department', 2, 0,  CONVERT(VARBINARY(25), '0x', 1), 1, 0, 3, 0, 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 'Administrator', '2017-02-02 00:00:00.0000000 -04:00', 900000003)
END