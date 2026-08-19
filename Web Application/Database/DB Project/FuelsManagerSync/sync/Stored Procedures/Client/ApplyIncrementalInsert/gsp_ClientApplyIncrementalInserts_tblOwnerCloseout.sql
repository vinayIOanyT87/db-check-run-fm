-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblOwnerCloseout
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblOwnerCloseout]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Site nvarchar(30),
@ManagerName nvarchar(100),
@ProductName nvarchar(30),
@CloseoutDate date,
@OwnerName nvarchar(100),
@GrossBookInventory float,
@NetBookInventory float,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@GrossBookPrice float,
@NetBookPrice float,
@TransVersion bigint,
@MassBookInventory float,
@MassBookPrice float,
@OwnerCloseoutGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ManagerCompanyGuid uniqueidentifier,
@OwnerCompanyGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblOwnerCloseout] AS existingData
        USING (SELECT @Site 'Site',@ManagerName 'ManagerName',@ProductName 'ProductName',@CloseoutDate 'CloseoutDate',@OwnerName 'OwnerName',@GrossBookInventory 'GrossBookInventory',@NetBookInventory 'NetBookInventory',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@GrossBookPrice 'GrossBookPrice',@NetBookPrice 'NetBookPrice',@TransVersion 'TransVersion',@MassBookInventory 'MassBookInventory',@MassBookPrice 'MassBookPrice',@OwnerCloseoutGuid 'OwnerCloseoutGuid',@SiteGuid 'SiteGuid',@ManagerCompanyGuid 'ManagerCompanyGuid',@OwnerCompanyGuid 'OwnerCompanyGuid',@ProductGuid 'ProductGuid'
                ) AS remoteChanges ([Site],[ManagerName],[ProductName],[CloseoutDate],[OwnerName],[GrossBookInventory],[NetBookInventory],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[TransVersion],[MassBookInventory],[MassBookPrice],[OwnerCloseoutGuid],[SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ProductGuid])
        ON (existingData.[OwnerCloseoutGuid] = remoteChanges.[OwnerCloseoutGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [Site] = remoteChanges.[Site]
                       ,[ManagerName] = remoteChanges.[ManagerName]
                       ,[ProductName] = remoteChanges.[ProductName]
                       ,[CloseoutDate] = remoteChanges.[CloseoutDate]
                       ,[OwnerName] = remoteChanges.[OwnerName]
                       ,[GrossBookInventory] = remoteChanges.[GrossBookInventory]
                       ,[NetBookInventory] = remoteChanges.[NetBookInventory]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[GrossBookPrice] = remoteChanges.[GrossBookPrice]
                       ,[NetBookPrice] = remoteChanges.[NetBookPrice]
                       ,[TransVersion] = remoteChanges.[TransVersion]
                       ,[MassBookInventory] = remoteChanges.[MassBookInventory]
                       ,[MassBookPrice] = remoteChanges.[MassBookPrice]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[ManagerCompanyGuid] = remoteChanges.[ManagerCompanyGuid]
                       ,[OwnerCompanyGuid] = remoteChanges.[OwnerCompanyGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]

        WHEN NOT MATCHED THEN
            INSERT ([Site],[ManagerName],[ProductName],[CloseoutDate],[OwnerName],[GrossBookInventory],[NetBookInventory],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[GrossBookPrice],[NetBookPrice],[TransVersion],[MassBookInventory],[MassBookPrice],[OwnerCloseoutGuid],[SiteGuid],[ManagerCompanyGuid],[OwnerCompanyGuid],[ProductGuid])
                VALUES (@Site,@ManagerName,@ProductName,@CloseoutDate,@OwnerName,@GrossBookInventory,@NetBookInventory,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@GrossBookPrice,@NetBookPrice,@TransVersion,@MassBookInventory,@MassBookPrice,@OwnerCloseoutGuid,@SiteGuid,@ManagerCompanyGuid,@OwnerCompanyGuid,@ProductGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @OwnerCloseoutGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @OwnerCloseoutGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @OwnerCloseoutGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblOwnerCloseout] WHERE OwnerCloseoutGuid = @OwnerCloseoutGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
