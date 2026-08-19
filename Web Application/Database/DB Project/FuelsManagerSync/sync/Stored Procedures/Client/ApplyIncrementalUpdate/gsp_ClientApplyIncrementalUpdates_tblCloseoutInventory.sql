-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblCloseoutInventory
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblCloseoutInventory]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Site nvarchar(30),
@CloseoutDate date,
@ProductName nvarchar(30),
@ManagerName nvarchar(100),
@GrossBookInventory float,
@NetBookInventory float,
@GrossPhysicalInventory float,
@NetPhysicalInventory float,
@GrossVariance float,
@NetVariance float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@GrossBookPrice float,
@NetBookPrice float,
@GrossPhysicalPrice float,
@NetPhysicalPrice float,
@TransVersion bigint,
@MassBookInventory float,
@MassPhysicalInventory float,
@MassVariance float,
@MassBookPrice float,
@MassPhysicalPrice float,
@CloseoutInventoryGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ManagerCompanyGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblCloseoutInventory] CT
                        WHERE CT.PK_CloseoutInventoryGuid = @CloseoutInventoryGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblCloseoutInventory].[Site],[dbo].[tblCloseoutInventory].[CloseoutDate],[dbo].[tblCloseoutInventory].[ProductName],[dbo].[tblCloseoutInventory].[ManagerName],[dbo].[tblCloseoutInventory].[GrossBookInventory],[dbo].[tblCloseoutInventory].[NetBookInventory],[dbo].[tblCloseoutInventory].[GrossPhysicalInventory],[dbo].[tblCloseoutInventory].[NetPhysicalInventory],[dbo].[tblCloseoutInventory].[GrossVariance],[dbo].[tblCloseoutInventory].[NetVariance],[dbo].[tblCloseoutInventory].[CreatedDate],[dbo].[tblCloseoutInventory].[CreatedBy],[dbo].[tblCloseoutInventory].[UpdatedDate],[dbo].[tblCloseoutInventory].[UpdatedBy],[dbo].[tblCloseoutInventory].[GrossBookPrice],[dbo].[tblCloseoutInventory].[NetBookPrice],[dbo].[tblCloseoutInventory].[GrossPhysicalPrice],[dbo].[tblCloseoutInventory].[NetPhysicalPrice],[dbo].[tblCloseoutInventory].[TransVersion],[dbo].[tblCloseoutInventory].[MassBookInventory],[dbo].[tblCloseoutInventory].[MassPhysicalInventory],[dbo].[tblCloseoutInventory].[MassVariance],[dbo].[tblCloseoutInventory].[MassBookPrice],[dbo].[tblCloseoutInventory].[MassPhysicalPrice],[dbo].[tblCloseoutInventory].[CloseoutInventoryGuid],[dbo].[tblCloseoutInventory].[SiteGuid],[dbo].[tblCloseoutInventory].[ManagerCompanyGuid],[dbo].[tblCloseoutInventory].[ProductGuid]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblCloseoutInventory]
                        INNER JOIN [track].[tblCloseoutInventory] CT
                            ON CT.PK_CloseoutInventoryGuid = [dbo].[tblCloseoutInventory].[CloseoutInventoryGuid] 
                    WHERE CT.PK_CloseoutInventoryGuid = @CloseoutInventoryGuid
            ) MERGE existingData
            USING (SELECT @Site,@CloseoutDate,@ProductName,@ManagerName,@GrossBookInventory,@NetBookInventory,@GrossPhysicalInventory,@NetPhysicalInventory,@GrossVariance,@NetVariance,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@GrossBookPrice,@NetBookPrice,@GrossPhysicalPrice,@NetPhysicalPrice,@TransVersion,@MassBookInventory,@MassPhysicalInventory,@MassVariance,@MassBookPrice,@MassPhysicalPrice,@CloseoutInventoryGuid,@SiteGuid,@ManagerCompanyGuid,@ProductGuid
                    ) AS remoteChanges ([Site],[CloseoutDate],[ProductName],[ManagerName],[GrossBookInventory],[NetBookInventory],[GrossPhysicalInventory],[NetPhysicalInventory],[GrossVariance],[NetVariance],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[GrossPhysicalPrice],[NetPhysicalPrice],[TransVersion],[MassBookInventory],[MassPhysicalInventory],[MassVariance],[MassBookPrice],[MassPhysicalPrice],[CloseoutInventoryGuid],[SiteGuid],[ManagerCompanyGuid],[ProductGuid])
            ON (existingData.[CloseoutInventoryGuid] = remoteChanges.[CloseoutInventoryGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [Site] = remoteChanges.[Site]
                       ,[CloseoutDate] = remoteChanges.[CloseoutDate]
                       ,[ProductName] = remoteChanges.[ProductName]
                       ,[ManagerName] = remoteChanges.[ManagerName]
                       ,[GrossBookInventory] = remoteChanges.[GrossBookInventory]
                       ,[NetBookInventory] = remoteChanges.[NetBookInventory]
                       ,[GrossPhysicalInventory] = remoteChanges.[GrossPhysicalInventory]
                       ,[NetPhysicalInventory] = remoteChanges.[NetPhysicalInventory]
                       ,[GrossVariance] = remoteChanges.[GrossVariance]
                       ,[NetVariance] = remoteChanges.[NetVariance]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[GrossBookPrice] = remoteChanges.[GrossBookPrice]
                       ,[NetBookPrice] = remoteChanges.[NetBookPrice]
                       ,[GrossPhysicalPrice] = remoteChanges.[GrossPhysicalPrice]
                       ,[NetPhysicalPrice] = remoteChanges.[NetPhysicalPrice]
                       ,[TransVersion] = remoteChanges.[TransVersion]
                       ,[MassBookInventory] = remoteChanges.[MassBookInventory]
                       ,[MassPhysicalInventory] = remoteChanges.[MassPhysicalInventory]
                       ,[MassVariance] = remoteChanges.[MassVariance]
                       ,[MassBookPrice] = remoteChanges.[MassBookPrice]
                       ,[MassPhysicalPrice] = remoteChanges.[MassPhysicalPrice]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[ManagerCompanyGuid] = remoteChanges.[ManagerCompanyGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]

            WHEN NOT MATCHED THEN
                INSERT ([Site],[CloseoutDate],[ProductName],[ManagerName],[GrossBookInventory],[NetBookInventory],[GrossPhysicalInventory],[NetPhysicalInventory],[GrossVariance],[NetVariance],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[GrossPhysicalPrice],[NetPhysicalPrice],[TransVersion],[MassBookInventory],[MassPhysicalInventory],[MassVariance],[MassBookPrice],[MassPhysicalPrice],[CloseoutInventoryGuid],[SiteGuid],[ManagerCompanyGuid],[ProductGuid])
                    VALUES (@Site,@CloseoutDate,@ProductName,@ManagerName,@GrossBookInventory,@NetBookInventory,@GrossPhysicalInventory,@NetPhysicalInventory,@GrossVariance,@NetVariance,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@GrossBookPrice,@NetBookPrice,@GrossPhysicalPrice,@NetPhysicalPrice,@TransVersion,@MassBookInventory,@MassPhysicalInventory,@MassVariance,@MassBookPrice,@MassPhysicalPrice,@CloseoutInventoryGuid,@SiteGuid,@ManagerCompanyGuid,@ProductGuid)
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
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CloseoutInventoryGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CloseoutInventoryGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @CloseoutInventoryGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblCloseoutInventory] WHERE CloseoutInventoryGuid = @CloseoutInventoryGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
