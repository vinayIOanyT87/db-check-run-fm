-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAllocationLineItems
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblAllocationLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@Limit float,
@Next float,
@ResetMultiple int,
@ResetDate datetimeoffset(7),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@AllocationLineItemGuid uniqueidentifier,
@LookupAllocationTypeIndex int,
@LookupResetMethodIndex int,
@LookupResetPeriodIndex int,
@AllocationGuid uniqueidentifier,
@AssignedProductGuid uniqueidentifier,
@AssignedApplicationStringGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblAllocationLineItems varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblAllocationLineItems] AS existingData
        USING (SELECT @Limit 'Limit',@Next 'Next',@ResetMultiple 'ResetMultiple',@ResetDate 'ResetDate',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@AllocationLineItemGuid 'AllocationLineItemGuid',@LookupAllocationTypeIndex 'LookupAllocationTypeIndex',@LookupResetMethodIndex 'LookupResetMethodIndex',@LookupResetPeriodIndex 'LookupResetPeriodIndex',@AllocationGuid 'AllocationGuid',@AssignedProductGuid 'AssignedProductGuid',@AssignedApplicationStringGuid 'AssignedApplicationStringGuid'
                ) AS remoteChanges ([Limit],[Next],[ResetMultiple],[ResetDate],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AllocationLineItemGuid],[LookupAllocationTypeIndex],[LookupResetMethodIndex],[LookupResetPeriodIndex],[AllocationGuid],[AssignedProductGuid],[AssignedApplicationStringGuid])
        ON (existingData.[AllocationLineItemGuid] = remoteChanges.[AllocationLineItemGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [Limit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Limit'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[Limit] ELSE remoteChanges.[Limit] END
                       ,[Next] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Next'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[Next] ELSE remoteChanges.[Next] END
                       ,[ResetMultiple] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ResetMultiple'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[ResetMultiple] ELSE remoteChanges.[ResetMultiple] END
                       ,[ResetDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ResetDate'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[ResetDate] ELSE remoteChanges.[ResetDate] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[LookupAllocationTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupAllocationTypeIndex'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[LookupAllocationTypeIndex] ELSE remoteChanges.[LookupAllocationTypeIndex] END
                       ,[LookupResetMethodIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupResetMethodIndex'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[LookupResetMethodIndex] ELSE remoteChanges.[LookupResetMethodIndex] END
                       ,[LookupResetPeriodIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupResetPeriodIndex'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[LookupResetPeriodIndex] ELSE remoteChanges.[LookupResetPeriodIndex] END
                       ,[AllocationGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AllocationGuid'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[AllocationGuid] ELSE remoteChanges.[AllocationGuid] END
                       ,[AssignedProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedProductGuid'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[AssignedProductGuid] ELSE remoteChanges.[AssignedProductGuid] END
                       ,[AssignedApplicationStringGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedApplicationStringGuid'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN existingData.[AssignedApplicationStringGuid] ELSE remoteChanges.[AssignedApplicationStringGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([Limit],[Next],[ResetMultiple],[ResetDate],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[AllocationLineItemGuid],[LookupAllocationTypeIndex],[LookupResetMethodIndex],[LookupResetPeriodIndex],[AllocationGuid],[AssignedProductGuid],[AssignedApplicationStringGuid])
                VALUES (@Limit,@Next,@ResetMultiple,@ResetDate,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@AllocationLineItemGuid,@LookupAllocationTypeIndex,@LookupResetMethodIndex,@LookupResetPeriodIndex,@AllocationGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedProductGuid'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN NULL ELSE @AssignedProductGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedApplicationStringGuid'), @sync_supported_columns_tblAllocationLineItems)) WHEN 0 THEN NULL ELSE @AssignedApplicationStringGuid END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationLineItemGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationLineItemGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationLineItemGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblAllocationLineItems] WHERE AllocationLineItemGuid = @AllocationLineItemGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

