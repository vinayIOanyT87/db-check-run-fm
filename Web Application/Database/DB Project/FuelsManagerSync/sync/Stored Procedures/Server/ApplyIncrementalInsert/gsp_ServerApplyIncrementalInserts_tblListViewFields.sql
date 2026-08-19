-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblListViewFields
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblListViewFields]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ColumnOrder int,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@ListViewID nvarchar(50),
@ListViewFieldGuid uniqueidentifier,
@LookupListViewFieldTypeIndex int,
@LookupStandardFieldTypeIndex int,
@ListViewGuid uniqueidentifier,
@TransactionAliasGuid uniqueidentifier,
@TransactionAliasFieldGuid uniqueidentifier,
@UserDataFieldTransactionAliasGuid uniqueidentifier,
@UserDataFieldTransactionAliasLineItemGuid uniqueidentifier,
@LedgerAggregateColumnGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblListViewFields varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblListViewFields] AS existingData
        USING (SELECT @ColumnOrder 'ColumnOrder',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@ListViewID 'ListViewID',@ListViewFieldGuid 'ListViewFieldGuid',@LookupListViewFieldTypeIndex 'LookupListViewFieldTypeIndex',@LookupStandardFieldTypeIndex 'LookupStandardFieldTypeIndex',@ListViewGuid 'ListViewGuid',@TransactionAliasGuid 'TransactionAliasGuid',@TransactionAliasFieldGuid 'TransactionAliasFieldGuid',@UserDataFieldTransactionAliasGuid 'UserDataFieldTransactionAliasGuid',@UserDataFieldTransactionAliasLineItemGuid 'UserDataFieldTransactionAliasLineItemGuid',@LedgerAggregateColumnGuid 'LedgerAggregateColumnGuid'
                ) AS remoteChanges ([ColumnOrder],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ListViewID],[ListViewFieldGuid],[LookupListViewFieldTypeIndex],[LookupStandardFieldTypeIndex],[ListViewGuid],[TransactionAliasGuid],[TransactionAliasFieldGuid],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[LedgerAggregateColumnGuid])
        ON (existingData.[ListViewFieldGuid] = remoteChanges.[ListViewFieldGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ColumnOrder] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ColumnOrder'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[ColumnOrder] ELSE remoteChanges.[ColumnOrder] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[ListViewID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ListViewID'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[ListViewID] ELSE remoteChanges.[ListViewID] END
                       ,[LookupListViewFieldTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupListViewFieldTypeIndex'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[LookupListViewFieldTypeIndex] ELSE remoteChanges.[LookupListViewFieldTypeIndex] END
                       ,[LookupStandardFieldTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupStandardFieldTypeIndex'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[LookupStandardFieldTypeIndex] ELSE remoteChanges.[LookupStandardFieldTypeIndex] END
                       ,[ListViewGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ListViewGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[ListViewGuid] ELSE remoteChanges.[ListViewGuid] END
                       ,[TransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionAliasGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[TransactionAliasGuid] ELSE remoteChanges.[TransactionAliasGuid] END
                       ,[TransactionAliasFieldGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionAliasFieldGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[TransactionAliasFieldGuid] ELSE remoteChanges.[TransactionAliasFieldGuid] END
                       ,[UserDataFieldTransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserDataFieldTransactionAliasGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[UserDataFieldTransactionAliasGuid] ELSE remoteChanges.[UserDataFieldTransactionAliasGuid] END
                       ,[UserDataFieldTransactionAliasLineItemGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserDataFieldTransactionAliasLineItemGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[UserDataFieldTransactionAliasLineItemGuid] ELSE remoteChanges.[UserDataFieldTransactionAliasLineItemGuid] END
                       ,[LedgerAggregateColumnGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LedgerAggregateColumnGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN existingData.[LedgerAggregateColumnGuid] ELSE remoteChanges.[LedgerAggregateColumnGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([ColumnOrder],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ListViewID],[ListViewFieldGuid],[LookupListViewFieldTypeIndex],[LookupStandardFieldTypeIndex],[ListViewGuid],[TransactionAliasGuid],[TransactionAliasFieldGuid],[UserDataFieldTransactionAliasGuid],[UserDataFieldTransactionAliasLineItemGuid],[LedgerAggregateColumnGuid])
                VALUES (@ColumnOrder,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@ListViewID,@ListViewFieldGuid,@LookupListViewFieldTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupStandardFieldTypeIndex'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN NULL ELSE @LookupStandardFieldTypeIndex END),@ListViewGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionAliasGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN NULL ELSE @TransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionAliasFieldGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN NULL ELSE @TransactionAliasFieldGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserDataFieldTransactionAliasGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN NULL ELSE @UserDataFieldTransactionAliasGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserDataFieldTransactionAliasLineItemGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN NULL ELSE @UserDataFieldTransactionAliasLineItemGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LedgerAggregateColumnGuid'), @sync_supported_columns_tblListViewFields)) WHEN 0 THEN NULL ELSE @LedgerAggregateColumnGuid END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ListViewFieldGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ListViewFieldGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ListViewFieldGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblListViewFields] WHERE ListViewFieldGuid = @ListViewFieldGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

