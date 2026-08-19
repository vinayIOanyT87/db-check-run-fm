-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExportResultDetails
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblExportResultDetails]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@RecordID nvarchar(64),
@Fail bit,
@TransVersion bigint,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@Error nvarchar(250),
@ExportResultDetailGuid uniqueidentifier,
@ExportResultGuid uniqueidentifier,
@InterfaceData01 nvarchar(100),
@InterfaceData02 nvarchar(100),
@InterfaceData03 nvarchar(100),
@InterfaceData04 nvarchar(100),
@InterfaceData05 nvarchar(100),
@InterfaceData06 nvarchar(100),
@InterfaceData07 nvarchar(100),
@InterfaceData08 nvarchar(100),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblExportResultDetails] AS existingData
        USING (SELECT @RecordID 'RecordID',@Fail 'Fail',@TransVersion 'TransVersion',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@Error 'Error',@ExportResultDetailGuid 'ExportResultDetailGuid',@ExportResultGuid 'ExportResultGuid',@InterfaceData01 'InterfaceData01',@InterfaceData02 'InterfaceData02',@InterfaceData03 'InterfaceData03',@InterfaceData04 'InterfaceData04',@InterfaceData05 'InterfaceData05',@InterfaceData06 'InterfaceData06',@InterfaceData07 'InterfaceData07',@InterfaceData08 'InterfaceData08'
                ) AS remoteChanges ([RecordID],[Fail],[TransVersion],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Error],[ExportResultDetailGuid],[ExportResultGuid],[InterfaceData01],[InterfaceData02],[InterfaceData03],[InterfaceData04],[InterfaceData05],[InterfaceData06],[InterfaceData07],[InterfaceData08])
        ON (existingData.[ExportResultDetailGuid] = remoteChanges.[ExportResultDetailGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [RecordID] = remoteChanges.[RecordID]
                       ,[Fail] = remoteChanges.[Fail]
                       ,[TransVersion] = remoteChanges.[TransVersion]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[Error] = remoteChanges.[Error]
                       ,[ExportResultGuid] = remoteChanges.[ExportResultGuid]
                       ,[InterfaceData01] = remoteChanges.[InterfaceData01]
                       ,[InterfaceData02] = remoteChanges.[InterfaceData02]
                       ,[InterfaceData03] = remoteChanges.[InterfaceData03]
                       ,[InterfaceData04] = remoteChanges.[InterfaceData04]
                       ,[InterfaceData05] = remoteChanges.[InterfaceData05]
                       ,[InterfaceData06] = remoteChanges.[InterfaceData06]
                       ,[InterfaceData07] = remoteChanges.[InterfaceData07]
                       ,[InterfaceData08] = remoteChanges.[InterfaceData08]

        WHEN NOT MATCHED THEN
            INSERT ([RecordID],[Fail],[TransVersion],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[Error],[ExportResultDetailGuid],[ExportResultGuid],[InterfaceData01],[InterfaceData02],[InterfaceData03],[InterfaceData04],[InterfaceData05],[InterfaceData06],[InterfaceData07],[InterfaceData08])
                VALUES (@RecordID,@Fail,@TransVersion,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@Error,@ExportResultDetailGuid,@ExportResultGuid,@InterfaceData01,@InterfaceData02,@InterfaceData03,@InterfaceData04,@InterfaceData05,@InterfaceData06,@InterfaceData07,@InterfaceData08)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExportResultDetailGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExportResultDetailGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ExportResultDetailGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblExportResultDetails] WHERE ExportResultDetailGuid = @ExportResultDetailGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
