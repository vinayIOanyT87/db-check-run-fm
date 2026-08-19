-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmAndEvents
-- Description: Apply Deletes
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalDeletes_tblAlarmAndEvents]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AlarmAndEventGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DELETE [dbo].[tblAlarmAndEvents]  
        FROM [dbo].[tblAlarmAndEvents]  
            INNER JOIN [track].[tblAlarmAndEvents] CT
                ON CT.PK_AlarmAndEventGuid = [dbo].[tblAlarmAndEvents].[AlarmAndEventGuid]
        WHERE AlarmAndEventGuid = @AlarmAndEventGuid
            AND (@sync_force_write = 1 
            OR (DeletedRowVersion IS NULL OR DeletedRowVersion <= @sync_last_received_anchor))

    SET @sync_row_count = @@rowcount;
    
    -- Keep in mind that a pending insert conflict record means the delete statement could produce a rowcount of 0, so unlike inserts/updates, we should 
    -- remove any pending conflict records associated with this primary key since any pending inserts/updates would "re-introduce" the record.
    -- 
    SET NOCOUNT ON
    
    IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmAndEventGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
    BEGIN
        DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmAndEventGuid))
        DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AlarmAndEventGuid)
    END
    
    SET NOCOUNT OFF
    
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.

    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CD)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
