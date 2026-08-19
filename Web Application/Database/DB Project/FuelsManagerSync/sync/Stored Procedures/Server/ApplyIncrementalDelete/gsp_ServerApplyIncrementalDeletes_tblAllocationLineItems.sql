-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAllocationLineItems
-- Description: Apply Deletes
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalDeletes_tblAllocationLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@AllocationLineItemGuid uniqueidentifier,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

	SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    DELETE [dbo].[tblAllocationLineItems]  
        FROM [dbo].[tblAllocationLineItems]  
            INNER JOIN [track].[tblAllocationLineItems] CT
                ON CT.PK_AllocationLineItemGuid = [dbo].[tblAllocationLineItems].[AllocationLineItemGuid]
        WHERE AllocationLineItemGuid = @AllocationLineItemGuid
            AND (@sync_force_write = 1 
            OR (DeletedRowVersion IS NULL OR DeletedRowVersion <= @sync_last_received_anchor_varbinary))

    SET @sync_row_count = @@rowcount;
    
    -- Keep in mind that a pending insert conflict record means the delete statement could produce a rowcount of 0, so unlike inserts/updates, we should 
    -- remove any pending conflict records associated with this primary key since any pending inserts/updates would "re-introduce" the record.
    -- 
    SET NOCOUNT ON
    
    IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationLineItemGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
    BEGIN
        DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationLineItemGuid))
        DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @AllocationLineItemGuid)
    END
    
    SET NOCOUNT OFF
    
    SET @minValidVersion = 0;	-- This is used to detect Change Tracking cleanup
					            -- If we support this, we should add a column to SynchronizationTable
								-- that records the MinValidVersion after change tracking information for
								-- a table gets cleaned up.  I don't think this will be necessary.

    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CD)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
