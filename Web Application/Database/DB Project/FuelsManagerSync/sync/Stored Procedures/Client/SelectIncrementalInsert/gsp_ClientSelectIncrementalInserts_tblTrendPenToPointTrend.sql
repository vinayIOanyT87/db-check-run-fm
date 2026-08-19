-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTrendPenToPointTrend
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblTrendPenToPointTrend]
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
@sync_batch_size_tblTrendPenToPointTrend int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblTrendPenToPointTrend int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- The FuelsManager Client selection for inserts is not coded to support a default SELECT ALL in order to push into the Enterprise.  This is by design.
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid],[map].[tblTrendPenToPointTrend].[PointTagGuid],[map].[tblTrendPenToPointTrend].[TrendGuid],[map].[tblTrendPenToPointTrend].[PenColor],[map].[tblTrendPenToPointTrend].[CreatedDate],[map].[tblTrendPenToPointTrend].[CreatedBy],[map].[tblTrendPenToPointTrend].[UpdatedDate],[map].[tblTrendPenToPointTrend].[UpdatedBy], [map].[tblTrendPenToPointTrend].[_RowVersion]
            FROM [map].[tblTrendPenToPointTrend]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblTrendPenToPointTrend IS NULL OR 
        (@sync_batch_size_tblTrendPenToPointTrend IS NOT NULL AND @sync_batch_size_tblTrendPenToPointTrend = 0))
    BEGIN
        SET @sync_batch_size_tblTrendPenToPointTrend = 2147483647;
    END

  	-- Insertions associated with Points
	IF 0 <> (SELECT COUNT(*) FROM #SyncTable)
	BEGIN
    SELECT TOP(@sync_batch_size_tblTrendPenToPointTrend) WITH TIES [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid],[map].[tblTrendPenToPointTrend].[PointTagGuid],[map].[tblTrendPenToPointTrend].[TrendGuid],[map].[tblTrendPenToPointTrend].[PenColor],[map].[tblTrendPenToPointTrend].[CreatedDate],[map].[tblTrendPenToPointTrend].[CreatedBy],[map].[tblTrendPenToPointTrend].[UpdatedDate],[map].[tblTrendPenToPointTrend].[UpdatedBy],CT.InsertedRowVersion AS '_RowVersion'
        FROM [map].[tblTrendPenToPointTrend]
            INNER JOIN [track].[tblTrendPenToPointTrend] CT ON CT.PK_TrendPenToPointTrendGuid = [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid] 
				INNER JOIN [track].[tblPointTag] pt ON pt.[PK_PointTagGuid] = CT.FK_ParentPK
            INNER JOIN #SyncTable ON #SyncTable.PK =  pt.FK_ParentPK 
            WHERE ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC
	END
	ELSE
	BEGIN
    SELECT TOP(@sync_batch_size_tblTrendPenToPointTrend) WITH TIES [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid],[map].[tblTrendPenToPointTrend].[PointTagGuid],[map].[tblTrendPenToPointTrend].[TrendGuid],[map].[tblTrendPenToPointTrend].[PenColor],[map].[tblTrendPenToPointTrend].[CreatedDate],[map].[tblTrendPenToPointTrend].[CreatedBy],[map].[tblTrendPenToPointTrend].[UpdatedDate],[map].[tblTrendPenToPointTrend].[UpdatedBy],CT.InsertedRowVersion AS '_RowVersion'
        FROM [map].[tblTrendPenToPointTrend]
            INNER JOIN [track].[tblTrendPenToPointTrend] CT ON CT.PK_TrendPenToPointTrendGuid = [map].[tblTrendPenToPointTrend].[TrendPenToPointTrendGuid] 
				INNER JOIN [track].[tblPointTag] ptt ON ptt.[PK_PointTagGuid] = CT.FK_ParentPK
				INNER JOIN [track].[tblPoint] pt ON pt.PK_PointGuid = ptt.FK_ParentPK
            WHERE ((pt.CurrentSiteGuid = @sync_context_site_guid)
					AND (CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
               AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
               AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC
	END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
