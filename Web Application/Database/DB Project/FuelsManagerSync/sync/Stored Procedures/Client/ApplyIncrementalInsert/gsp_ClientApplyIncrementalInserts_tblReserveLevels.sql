-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblReserveLevels
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblReserveLevels]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@MinimumLevel float,
@WarningLevel float,
@ReserveLevelGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblReserveLevels] AS existingData
        USING (SELECT @MinimumLevel 'MinimumLevel',@WarningLevel 'WarningLevel',@ReserveLevelGuid 'ReserveLevelGuid',@SiteGuid 'SiteGuid',@ProductGuid 'ProductGuid'
                ) AS remoteChanges ([MinimumLevel],[WarningLevel],[ReserveLevelGuid],[SiteGuid],[ProductGuid])
        ON (existingData.[ReserveLevelGuid] = remoteChanges.[ReserveLevelGuid])
        WHEN MATCHED  THEN
            UPDATE SET [MinimumLevel] = remoteChanges.[MinimumLevel]
                       ,[WarningLevel] = remoteChanges.[WarningLevel]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]

        WHEN NOT MATCHED THEN
            INSERT ([MinimumLevel],[WarningLevel],[ReserveLevelGuid],[SiteGuid],[ProductGuid])
                VALUES (@MinimumLevel,@WarningLevel,@ReserveLevelGuid,@SiteGuid,@ProductGuid)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ReserveLevelGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ReserveLevelGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ReserveLevelGuid)
        END
        SET NOCOUNT OFF
    END
    

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
