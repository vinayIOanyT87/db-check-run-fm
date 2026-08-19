/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/


-- Remove the SYNC metadata and let the upgrade populate it again
DELETE FROM [sync].[tblSyncTableToScopeMapCommand]
DELETE FROM [sync].[tblSyncTableToScopeMapColumn]
DELETE FROM [sync].[tblSyncTableToScopeMap]
DELETE FROM [sync].[tblSyncTable]
DELETE FROM [sync].[tblSyncDependencyGroup]
DELETE FROM [sync].[tblSyncScope]
DELETE FROM [sync].[tblSyncScopeType]
/*
************************************
	DATA INSERT SECTION:
************************************
*/
PRINT '***************************************************************************'
PRINT '** PREPARE INITIALIZE / UPDATE STATIC REFERENCE DATA (i.e. lookup data)  **'
PRINT '***************************************************************************'
PRINT '** NOTE: Changes to existing data will appear below.  If a lookup index changed; '
PRINT '         You are responsible for adding the appropriate statements to the '
PRINT '         Script.IncrementalDataMaintenance.sql deployment script so existing '
PRINT '**       records are updated to use the new lookup index value. '

:r .\syncData\sync.tblSyncProfile.refdata.sql
:r .\syncData\sync.tblSyncScopeType.refdata.sql
:r .\syncData\sync.tblSyncDependencyGroup.refdata.sql
:r .\syncData\sync.tblSyncTable.refdata.sql
:r .\syncData\sync.tblSyncScope.refdata.sql
:r .\syncData\sync.tblSyncTableToScopeMap.refdata.sql
:r .\syncData\sync.tblSyncTableToScopeMapColumn.refdata.sql
:r .\syncData\sync.tblSyncTableToScopeMapCommand.refdata.sql

PRINT '***************************************************************************'
PRINT '** FINISHED INITIALIZE / UPDATE STATIC REFERENCE DATA (i.e. lookup data) **'
PRINT '***************************************************************************'

-- Make sure the FirstTimeSyncOption column gets initialized to 0.  Existing application code that uses a default value when SQL returns a NULL needs to be refactored and improved.
-- For now, make sure existing records are initialized to a NON-NULL value.
IF EXISTS(SELECT 1 FROM [sync].[tblSyncTableToScopeMap] WHERE [FirstTimeSyncOption] IS NULL)
BEGIN
	UPDATE [sync].[tblSyncTableToScopeMap] SET [FirstTimeSyncOption] = 0 WHERE [FirstTimeSyncOption] IS NULL
END


IF (SELECT COUNT(*) FROM [sync].[tblSiteType])=0
BEGIN
	-- Create a List of Site Types
	INSERT INTO [sync].[tblSiteType] ([SiteTypeIndex], [SiteTypeGuid], [SiteTypeID], [SiteTypeName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (0, N'b16e5dc9-8f29-446e-823c-e594a9ea361a', N'Root', N'Site is the root site for synchronization.', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N'administrator', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N'administrator')
	INSERT INTO [sync].[tblSiteType] ([SiteTypeIndex], [SiteTypeGuid], [SiteTypeID], [SiteTypeName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (1, N'c5f042ae-a683-4196-b3a7-2679c98902a4', N'Reference', N'Site is being synchronized as a Reference Site', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N'administrator', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N'administrator')
	INSERT INTO [sync].[tblSiteType] ([SiteTypeIndex], [SiteTypeGuid], [SiteTypeID], [SiteTypeName], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (2, N'2cdf15d2-e9c2-4836-be1e-6f0a539104f4', N'Hosted', N'Site is being synchronized as a Hosted Site', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N'administrator', CAST(0x07AFCA0C59BFEF360B10FF AS DateTimeOffset), N'administrator')
END


IF NOT EXISTS (SELECT 1 FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'InstallDetailsSynchronizationProfileID')
BEGIN
	INSERT INTO [dbo].[tblConfigurationSetting] ([ConfigurationSettingGuid], [KeyType], [SettingKey], [SettingValue], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy]) VALUES (N'4D66AE28-3EAD-428D-8DC3-62090EF79748', N'SZ', N'InstallDetailsSynchronizationProfileID', N'', N'2/5/2014 3:57:48 PM -04:00', N'Administrator', N'2/5/2014 3:57:48 PM -04:00', N'Administrator')
END

IF EXISTS (SELECT 1 FROM [lookup].[tblSyncConflictResolutionStatus] WHERE SyncConflictResolutionStatusGuid = N'b6a7bb20-34fc-406c-9933-d701721b7927' AND SequenceOrder IS NULL)
BEGIN
	UPDATE [lookup].[tblSyncConflictResolutionStatus] SET SequenceOrder = 2 WHERE SyncConflictResolutionStatusGuid = N'b6a7bb20-34fc-406c-9933-d701721b7927'
END

IF EXISTS (SELECT 1 FROM [lookup].[tblSyncConflictResolutionStatus] WHERE SyncConflictResolutionStatusGuid = N'557d0815-c228-48c5-a45e-0cfca667788f' AND SequenceOrder IS NULL)
BEGIN
	UPDATE [lookup].[tblSyncConflictResolutionStatus] SET SequenceOrder = 3 WHERE SyncConflictResolutionStatusGuid = N'557d0815-c228-48c5-a45e-0cfca667788f'
END

IF EXISTS (SELECT 1 FROM [lookup].[tblSyncConflictResolutionStatus] WHERE SyncConflictResolutionStatusGuid = N'304e5e6d-350a-4309-9750-fa69765bd1fa' AND SequenceOrder IS NULL)
BEGIN
	UPDATE [lookup].[tblSyncConflictResolutionStatus] SET SequenceOrder = 4 WHERE SyncConflictResolutionStatusGuid = N'304e5e6d-350a-4309-9750-fa69765bd1fa'
END

DECLARE @nextSyncConflictResStatusIndex bigint
SELECT @nextSyncConflictResStatusIndex = max(SyncConflictResolutionStatusIndex) + 1 FROM [lookup].[tblSyncConflictResolutionStatus] 

IF NOT EXISTS (SELECT 1 FROM [lookup].[tblSyncConflictResolutionStatus] WHERE StatusCode = N'AUTORETRY')
BEGIN
	INSERT INTO [lookup].[tblSyncConflictResolutionStatus] ([SyncConflictResolutionStatusIndex], [SyncConflictResolutionStatusGuid], [StatusCode], [StatusName], [LongDescription], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [SequenceOrder]) VALUES (@nextSyncConflictResStatusIndex, N'021116E3-39E0-4A14-8B7F-87402CF3BBC4', N'AUTORETRY', N'Automatic Retry', N'Synchronization will automatically retry synchronization attempt of this record during the next synchronization session.  No user input required at this time.', '2/7/2014 1:43:25 PM -04:00',N'Administrator', '2/7/2014 1:43:25 PM -04:00', N'Administrator', 1)
END


-- We need to reset the sync anchors for these tables back to a baseline during this one time update
IF EXISTS(SELECT 1 FROM tblVersion WHERE [Version]='9.0.0.0')
BEGIN
	IF NOT EXISTS(SELECT 1 FROM tblVersion WHERE [Version]='9.0.0.0.1')
	BEGIN
		IF (SELECT COUNT(*) FROM [sync].[tblSyncAnchor] WHERE TableName = 'dbo.tblFuelCardLimit')<>0
		BEGIN
			UPDATE [sync].[tblSyncAnchor] SET LastReceivedAnchor = 0x0, LastReceivedAnchor2 = 0x0 WHERE TableName = 'dbo.tblFuelCardLimit'
		END

		IF (SELECT COUNT(*) FROM [sync].[tblSyncAnchor] WHERE TableName = 'dbo.tblFuelCardLimitLineItem')<>0
		BEGIN
			UPDATE [sync].[tblSyncAnchor] SET LastReceivedAnchor = 0x0, LastReceivedAnchor2 = 0x0 WHERE TableName = 'dbo.tblFuelCardLimitLineItem'
		END

		IF (SELECT COUNT(*) FROM [sync].[tblSyncAnchor] WHERE TableName = 'map.tblEntityFuelCardLimitToSite')<>0
		BEGIN
			UPDATE [sync].[tblSyncAnchor] SET LastReceivedAnchor = 0x0, LastReceivedAnchor2 = 0x0 WHERE TableName = 'map.tblEntityFuelCardLimitToSite'
		END

		IF (SELECT COUNT(*) FROM [sync].[tblSyncAnchor] WHERE TableName = 'map.tblFuelCardLimitToFuelCard')<>0
		BEGIN
			UPDATE [sync].[tblSyncAnchor] SET LastReceivedAnchor = 0x0, LastReceivedAnchor2 = 0x0 WHERE TableName = 'map.tblFuelCardLimitToFuelCard'
		END
	END
END
GO


-- Exclude the following [dbo].[tblSites] columns from Synchronization
-- [AutomaticBOLNextNumber]
-- [ManualBOLNextNumber]
-- [TransactionNextNumber]
-- [OrderNextNumber]
-- [InvoiceNextNumber]
-- [NumberPrefix]
DELETE FROM [sync].[tblSyncTableToScopeMapColumn] WHERE [SyncTableToScopeMapColumnGuid] IN (
				SELECT sttsmc.[SyncTableToScopeMapColumnGuid]
					FROM [sync].[tblSyncTable] st
						INNER JOIN [sync].[tblSyncTableToScopeMap] sttsm
							ON st.[SyncTableGuid] = sttsm.[SyncTableGuid]
						INNER JOIN [sync].[tblSyncTableToScopeMapColumn] sttsmc
							ON sttsm.[SyncTableToScopeMapGuid] = sttsmc.[SyncTableToScopeMapGuid]
					WHERE st.[TableName] = 'dbo.tblSites'
						AND sttsmc.ColumnName IN ('AutomaticBOLNextNumber',
													'ManualBOLNextNumber',
													'TransactionNextNumber',
													'OrderNextNumber',
													'NumberPrefix',
													'InvoiceNextNumber'))


/* These are being replaced by the correct one and since we don't deploy using the Drop Objects not in Source, we need to clean these up */
/****** Object:  Index [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID]    Script Date: 10/27/2016 3:11:50 PM ******/
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[sync].[tblSyncSessionScopeLog]') AND name = N'IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID')
BEGIN
	DROP INDEX [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID] ON [sync].[tblSyncSessionScopeLog]
END

/****** Object:  Index [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID_SiteGuid]    Script Date: 10/27/2016 3:11:50 PM ******/
IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'[sync].[tblSyncSessionScopeLog]') AND name = N'IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID_SiteGuid')
BEGIN
	DROP INDEX [IX_tblSyncSessionScopeLog_SyncSessionLogGuid_ScopeID_SiteGuid] ON [sync].[tblSyncSessionScopeLog]
END
GO

-- add missing column to auditlog syncing
IF NOT EXISTS(SELECT 1 FROM  [sync].[tblSyncTableToScopeMapColumn] WHERE [SyncTableToScopeMapGuid] = '162E87F5-52CB-4617-A0EA-2FACF7709C22' and ColumnName = 'AuditContext')
BEGIN
	UPDATE [sync].[tblSyncRecordConflict] SET SyncConflictResolutionStatusIndex = 2, ResolvedDate = SYSDATETIMEOFFSET(), updateddate =  SYSDATETIMEOFFSET(), updatedby = 'Administrator', ResolvedBy = 'administrator' WHERE  CommandText = 'sync.gsp_ServerApplyIncrementalInserts_tblAuditLog' and ResolvedDate is null
END
GO
-- Remove Description column from Sync Columns for map.tblEntityMaintenanceReasonToSite definition
IF EXISTS (SELECT 1 FROM [sync].[tblSyncTableToScopeMapColumn] WHERE [SyncTableToScopeMapColumnGuid] = N'D03B9885-75EE-48FF-A80B-E1DF713AA04D')
BEGIN
	DELETE FROM [sync].[tblSyncTableToScopeMapColumn] WHERE [SyncTableToScopeMapColumnGuid] = N'D03B9885-75EE-48FF-A80B-E1DF713AA04D'
END


IF NOT EXISTS(SELECT 1 FROM [sync].[tblSyncTableToScopeMap] WHERE ID = 'tblAuditLog' AND [MaxBatchSegmentRowCount] = 250)
BEGIN
	UPDATE [sync].[tblSyncTableToScopeMap] SET [MaxBatchSegmentRowCount] = 250 WHERE ID = 'tblAuditLog'
END

-- Change tblAuditLog to a bi-directional sync scope map so both systems can receive a record of any audit changes made by the remote system now that the audit triggers no longer
-- fire when synchronizing (propagating changes from one system to another).
IF NOT EXISTS(SELECT 1 FROM [sync].[tblSyncTableToScopeMap] WHERE ID = 'tblAuditLog' AND [SyncDirection] = 1)
BEGIN
	UPDATE [sync].[tblSyncTableToScopeMap] SET [SyncDirection] = 2 WHERE ID = 'tblAuditLog'
END

-- Setting the First Time Sync Option to 1 means that it will not be synchronized during the initial sync process.  This is needed because we recently changed the tblAuditLog to 
-- synchronize bi-directionally rather than being an upload only sync direction.
IF NOT EXISTS(SELECT 1 FROM [sync].[tblSyncTableToScopeMap] WHERE ID = 'tblAuditLog' AND [FirstTimeSyncOption] = 1)
BEGIN
	UPDATE [sync].[tblSyncTableToScopeMap] SET [FirstTimeSyncOption] = 0 WHERE ID = 'tblAuditLog'
END



