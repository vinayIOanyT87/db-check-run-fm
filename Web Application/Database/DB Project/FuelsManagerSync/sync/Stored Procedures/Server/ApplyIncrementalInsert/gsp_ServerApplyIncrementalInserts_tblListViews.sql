-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblListViews
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblListViews]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@ID nvarchar(50),
@ListViewGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupListViewTypeIndex int,
@LookupListViewStandardTypeIndex int,
@LedgerAggregateColumnGuid uniqueidentifier,
@TransactionAliasGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblListViews varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblListViews] AS existingData
        USING (SELECT @CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@ID 'ID',@ListViewGuid 'ListViewGuid',@SiteGuid 'SiteGuid',@LookupListViewTypeIndex 'LookupListViewTypeIndex',@LookupListViewStandardTypeIndex 'LookupListViewStandardTypeIndex',@LedgerAggregateColumnGuid 'LedgerAggregateColumnGuid',@TransactionAliasGuid 'TransactionAliasGuid'
                ) AS remoteChanges ([CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ID],[ListViewGuid],[SiteGuid],[LookupListViewTypeIndex],[LookupListViewStandardTypeIndex],[LedgerAggregateColumnGuid],[TransactionAliasGuid])
        ON (existingData.[ListViewGuid] = remoteChanges.[ListViewGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupListViewTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupListViewTypeIndex'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[LookupListViewTypeIndex] ELSE remoteChanges.[LookupListViewTypeIndex] END
                       ,[LookupListViewStandardTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupListViewStandardTypeIndex'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[LookupListViewStandardTypeIndex] ELSE remoteChanges.[LookupListViewStandardTypeIndex] END
                       ,[LedgerAggregateColumnGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LedgerAggregateColumnGuid'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[LedgerAggregateColumnGuid] ELSE remoteChanges.[LedgerAggregateColumnGuid] END
                       ,[TransactionAliasGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionAliasGuid'), @sync_supported_columns_tblListViews)) WHEN 0 THEN existingData.[TransactionAliasGuid] ELSE remoteChanges.[TransactionAliasGuid] END

        WHEN NOT MATCHED THEN
            INSERT ([CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ID],[ListViewGuid],[SiteGuid],[LookupListViewTypeIndex],[LookupListViewStandardTypeIndex],[LedgerAggregateColumnGuid],[TransactionAliasGuid])
                VALUES (@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@ID,@ListViewGuid,@SiteGuid,@LookupListViewTypeIndex,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupListViewStandardTypeIndex'), @sync_supported_columns_tblListViews)) WHEN 0 THEN NULL ELSE @LookupListViewStandardTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LedgerAggregateColumnGuid'), @sync_supported_columns_tblListViews)) WHEN 0 THEN NULL ELSE @LedgerAggregateColumnGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionAliasGuid'), @sync_supported_columns_tblListViews)) WHEN 0 THEN NULL ELSE @TransactionAliasGuid END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ListViewGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ListViewGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ListViewGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblListViews] WHERE ListViewGuid = @ListViewGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

