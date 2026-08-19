-- ***********************************************************************************************************************************************************************
-- During an initial DACPAC deployment, the DB Schema won't exist so we need to always check the database schema/catalog when doing anything in the Pre-Deployment Scripts
-- ***********************************************************************************************************************************************************************

-- Remove existing data from tracking tables before dropping them so we don't raise a data loss warning.
--
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='track' AND TABLE_CATALOG='tblReportDetails')
BEGIN
	DELETE FROM [track].[tblReportDetails]
END

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='track' AND TABLE_CATALOG='tblReportGroups')
BEGIN
	DELETE FROM [track].[tblReportGroups]
END

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='track' AND TABLE_CATALOG='tblEntityReportConfigurationSettingsToSite')
BEGIN
	DELETE FROM [track].[tblEntityReportConfigurationSettingsToSite]
END

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='track' AND TABLE_CATALOG='tblGroupToReportDetail')
BEGIN
	DELETE FROM [track].[tblGroupToReportDetail]
END

-- See if tblTransactions.InventoryDate has been converted to a Date field AND if the unique document trigger exists
-- When InventoryDate was converted to a Date field, the trigger was move into the NSPA dacpac so we no longer own it.
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'tblTransactions' AND COLUMN_NAME = 'InventoryDate' AND DATA_TYPE = 'datetimeoffset')
    AND OBJECT_ID('trg_insupd_tblTransactions_DocNumberUniqueness', 'TR') IS NOT NULL
BEGIN
    DROP TRIGGER [dbo].[trg_insupd_tblTransactions_DocNumberUniqueness]
END

IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='track' AND TABLE_CATALOG='tblReportApprovalState')
BEGIN
	DELETE FROM [track].[tblReportApprovalState]
END

-- During NSPA v9.1 merge back to the main branch, the SyncTableGuid for dbo.tblAuditLog was changed.  This will cause two tblSyncTable entries to appear in the table which
-- causes a problem during synchronization.  When the client is synchronizing a SyncGroup (Scope), the server dynamically creates the scope based on the client's table list
-- but since dbo.tblAuditLog is defined twice, an exception is thrown because the key (table name) already exists in the collection.
-- Due to foreign key constraints, the easiest way to resolve this is to simply delete the [sync].[tblSyncTableToScopeMapCommand], [sync].[tblSyncTableToScopeMapColumn], 
-- [sync].[tblSyncTableToScopeMap] and [sync].[tblSyncTable] records for anything associated with dbo.tblAuditLog and let the sync refdata.sql scripts add the correct
-- entries back to the tables.
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='sync' AND TABLE_CATALOG='tblSyncTable')
BEGIN
	-- If the bad SyncTableGuid exists remove it and anything that was referencing it.
	IF EXISTS (SELECT 1 FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'E23E2E0D-34E7-41CA-B8E4-65064ECCC1C2')
	BEGIN
		IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='sync' AND TABLE_CATALOG='tblSyncTableToScopeMap')
		BEGIN
			IF EXISTS (SELECT 1 FROM [sync].[tblSyncTableToScopeMap] WHERE SyncTableGuid = N'E23E2E0D-34E7-41CA-B8E4-65064ECCC1C2')
			BEGIN
				IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='sync' AND TABLE_CATALOG='tblSyncTableToScopeMapCommand')
				BEGIN
					DELETE FROM [sync].[tblSyncTableToScopeMapCommand] WHERE SyncTableToScopeMapGuid IN (SELECT SyncTableToScopeMapGuid FROM [sync].[tblSyncTableToScopeMap] WHERE SyncTableGuid = N'E23E2E0D-34E7-41CA-B8E4-65064ECCC1C2')
				END	

				IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='sync' AND TABLE_CATALOG='tblSyncTableToScopeMapColumn')
				BEGIN
					DELETE FROM [sync].[tblSyncTableToScopeMapColumn] WHERE SyncTableToScopeMapGuid IN (SELECT SyncTableToScopeMapGuid FROM [sync].[tblSyncTableToScopeMap] WHERE SyncTableGuid = N'E23E2E0D-34E7-41CA-B8E4-65064ECCC1C2')
				END

				DELETE FROM [sync].[tblSyncTableToScopeMap] WHERE SyncTableGuid = N'E23E2E0D-34E7-41CA-B8E4-65064ECCC1C2'
			END
		END

		DELETE FROM [sync].[tblSyncTable] WHERE SyncTableGuid = N'E23E2E0D-34E7-41CA-B8E4-65064ECCC1C2'
	END
END
GO

-- ***********************************************************************************************************
-- Migration script from FM11 to FM12 for lookup.tblStandardFieldType

-- Remove existing lookup.tblStandardFieldType data for new FM11 values that conflict with new FM12 SP1 values
-- ***********************************************************************************************************
IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='lookup' AND TABLE_NAME='tblStandardFieldType')
BEGIN
	IF EXISTS (SELECT 1 FROM [lookup].[tblStandardFieldType] WHERE StandardFieldTypeCode = 'DESTINATIONSERIALNUMBER1' AND StandardFieldTypeIndex = 175)
	BEGIN
		IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='tblListViewFields')
		BEGIN
			DELETE FROM [dbo].[tblListViewFields] WHERE LookupStandardFieldTypeIndex = 175
		END

		DELETE FROM [lookup].[tblStandardFieldType] WHERE StandardFieldTypeIndex = 175
	END

	IF EXISTS (SELECT 1 FROM [lookup].[tblStandardFieldType] WHERE StandardFieldTypeCode = 'DESTINATIONSERIALNUMBER2' AND StandardFieldTypeIndex = 176)
	BEGIN
		IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='tblListViewFields')
		BEGIN
			DELETE FROM [dbo].[tblListViewFields] WHERE LookupStandardFieldTypeIndex = 176
		END

		DELETE FROM [lookup].[tblStandardFieldType] WHERE StandardFieldTypeIndex = 176
	END

	IF EXISTS (SELECT 1 FROM [lookup].[tblStandardFieldType] WHERE StandardFieldTypeCode = 'DESTINATIONSERIALNUMBER3' AND StandardFieldTypeIndex = 177)
	BEGIN
		IF EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA='dbo' AND TABLE_NAME='tblListViewFields')
		BEGIN
			DELETE FROM [dbo].[tblListViewFields] WHERE LookupStandardFieldTypeIndex = 177
		END

		DELETE FROM [lookup].[tblStandardFieldType] WHERE StandardFieldTypeIndex = 177
	END
END