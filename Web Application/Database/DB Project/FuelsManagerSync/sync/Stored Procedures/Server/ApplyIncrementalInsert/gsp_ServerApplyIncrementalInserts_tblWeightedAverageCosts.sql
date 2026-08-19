-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblWeightedAverageCosts
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblWeightedAverageCosts]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@WacValue float,
@IsManualOverride bit,
@Source nvarchar(64),
@Notes nvarchar(2048),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@InventoryDate date,
@WeightedAverageCostGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblWeightedAverageCosts varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblWeightedAverageCosts] AS existingData
        USING (SELECT @WacValue 'WacValue',@IsManualOverride 'IsManualOverride',@Source 'Source',@Notes 'Notes',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@InventoryDate 'InventoryDate',@WeightedAverageCostGuid 'WeightedAverageCostGuid',@SiteGuid 'SiteGuid',@ProductGuid 'ProductGuid'
                ) AS remoteChanges ([WacValue],[IsManualOverride],[Source],[Notes],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[InventoryDate],[WeightedAverageCostGuid],[SiteGuid],[ProductGuid])
        ON (existingData.[WeightedAverageCostGuid] = remoteChanges.[WeightedAverageCostGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [WacValue] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WacValue'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[WacValue] ELSE remoteChanges.[WacValue] END
                       ,[IsManualOverride] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IsManualOverride'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[IsManualOverride] ELSE remoteChanges.[IsManualOverride] END
                       ,[Source] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Source'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[Source] ELSE remoteChanges.[Source] END
                       ,[Notes] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Notes'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[Notes] ELSE remoteChanges.[Notes] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[InventoryDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InventoryDate'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[InventoryDate] ELSE remoteChanges.[InventoryDate] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([WacValue],[IsManualOverride],[Source],[Notes],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[InventoryDate],[WeightedAverageCostGuid],[SiteGuid],[ProductGuid])
                VALUES (@WacValue,@IsManualOverride,@Source,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Notes'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN NULL ELSE @Notes END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN NULL ELSE @CreatedBy END),@CreatedDate,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblWeightedAverageCosts)) WHEN 0 THEN NULL ELSE @UpdatedBy END),@UpdatedDate,@InventoryDate,@WeightedAverageCostGuid,@SiteGuid,@ProductGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @WeightedAverageCostGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @WeightedAverageCostGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @WeightedAverageCostGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblWeightedAverageCosts] WHERE WeightedAverageCostGuid = @WeightedAverageCostGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

