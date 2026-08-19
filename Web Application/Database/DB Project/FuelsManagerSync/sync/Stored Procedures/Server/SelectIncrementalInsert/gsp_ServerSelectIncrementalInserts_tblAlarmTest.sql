-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAlarmTest
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblAlarmTest]
@sync_initialized bit,
@sync_last_received_anchor bigint,
@sync_new_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_server_id_binary binary(16),
@sync_context_site_guid uniqueidentifier,
@sync_context_site_id nvarchar(30),
@sync_context_site_guid_list nvarchar(1024),
@sync_context_site_id_list nvarchar(1024),
@sync_table_name nvarchar(512),
@sync_batch_size_tblAlarmTest int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblAlarmTest int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblAlarmTest IS NOT NULL AND @sync_first_time_sync_option_tblAlarmTest = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblAlarmTest].[AlarmTestGuid],[dbo].[tblAlarmTest].[AlarmGuid],[dbo].[tblAlarmTest].[ID],[dbo].[tblAlarmTest].[LimitTagGuid],[dbo].[tblAlarmTest].[TagField],[dbo].[tblAlarmTest].[AlarmPriorityGuid],[dbo].[tblAlarmTest].[NormalUnacknowledgedAlarmPriorityGuid],[dbo].[tblAlarmTest].[TestType],[dbo].[tblAlarmTest].[BitMask],[dbo].[tblAlarmTest].[Enabled],[dbo].[tblAlarmTest].[Order],[dbo].[tblAlarmTest].[AlarmState],[dbo].[tblAlarmTest].[Holdoff],[dbo].[tblAlarmTest].[AlarmText],[dbo].[tblAlarmTest].[HelpFile],[dbo].[tblAlarmTest].[DrawingGuid],[dbo].[tblAlarmTest].[CreatedDate],[dbo].[tblAlarmTest].[CreatedBy],[dbo].[tblAlarmTest].[UpdatedDate],[dbo].[tblAlarmTest].[UpdatedBy],[dbo].[tblAlarmTest].[BitwiseOperator],[dbo].[tblAlarmTest].[TimedHoldOffInSeconds],[dbo].[tblAlarmTest].[AlarmTestTemplateGuid], [dbo].[tblAlarmTest].[_RowVersion]
            FROM [dbo].[tblAlarmTest]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblAlarmTest IS NULL OR 
        (@sync_batch_size_tblAlarmTest IS NOT NULL AND @sync_batch_size_tblAlarmTest = 0))
    BEGIN
        SET @sync_batch_size_tblAlarmTest = 2147483647;
    END

        -- We only do this on tblAlarmTest so we know what which tblAlarmTest records are being processed.
        -- Synchronization will only synchronize other dependent records that are associated with the tblAlarmTest records
        -- that were included.
    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
    -- Tables that are associated with tblAlarmTest are filtered through a temp #SyncTable based on the selected tblAlarmTest records
    -- and therefore are not limited by a TOP(n) clause
    -- 
    SELECT [dbo].[tblAlarmTest].[AlarmTestGuid],[dbo].[tblAlarmTest].[AlarmGuid],[dbo].[tblAlarmTest].[ID],[dbo].[tblAlarmTest].[LimitTagGuid],[dbo].[tblAlarmTest].[TagField],[dbo].[tblAlarmTest].[AlarmPriorityGuid],[dbo].[tblAlarmTest].[NormalUnacknowledgedAlarmPriorityGuid],[dbo].[tblAlarmTest].[TestType],[dbo].[tblAlarmTest].[BitMask],[dbo].[tblAlarmTest].[Enabled],[dbo].[tblAlarmTest].[Order],[dbo].[tblAlarmTest].[AlarmState],[dbo].[tblAlarmTest].[Holdoff],[dbo].[tblAlarmTest].[AlarmText],[dbo].[tblAlarmTest].[HelpFile],[dbo].[tblAlarmTest].[DrawingGuid],[dbo].[tblAlarmTest].[CreatedDate],[dbo].[tblAlarmTest].[CreatedBy],[dbo].[tblAlarmTest].[UpdatedDate],[dbo].[tblAlarmTest].[UpdatedBy],[dbo].[tblAlarmTest].[BitwiseOperator],[dbo].[tblAlarmTest].[TimedHoldOffInSeconds],[dbo].[tblAlarmTest].[AlarmTestTemplateGuid],CT.InsertedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAlarmTest]
				INNER JOIN [track].[tblAlarmTest] CT ON CT.PK_AlarmTestGuid = [dbo].[tblAlarmTest].[AlarmTestGuid] 
            INNER JOIN [track].tblAlarm a ON a.PK_AlarmGuid = CT.FK_ParentPK
            INNER JOIN [track].tblPointTag pt ON pt.PK_PointTagGuid  = A.FK_ParentPK
            INNER JOIN #SyncTable ON #SyncTable.PK = pt.FK_ParentPK

         WHERE (CT.DeletedRowVersion IS NULL)
               AND ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
               AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
               AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
