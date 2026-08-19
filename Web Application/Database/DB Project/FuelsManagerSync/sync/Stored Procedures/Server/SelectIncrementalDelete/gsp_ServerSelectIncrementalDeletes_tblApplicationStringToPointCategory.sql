-- ========================================================================================
-- Author:		<Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToPointCategory
-- Description:	Get Incremental Deleted Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalDeletes_tblApplicationStringToPointCategory]
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
@sync_batch_size_tblApplicationStringToPointCategory int,
@sync_bypass_delete_extraction bit,
@sync_request_type int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)
    
    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF @sync_request_type <> 4 -- This replaced sync_initialized since we can't control it when performing batch synchronization
    BEGIN 
        IF (@sync_batch_size_tblApplicationStringToPointCategory IS NULL OR 
            (@sync_batch_size_tblApplicationStringToPointCategory IS NOT NULL AND @sync_batch_size_tblApplicationStringToPointCategory = 0))
        BEGIN
            SET @sync_batch_size_tblApplicationStringToPointCategory = 2147483647;
        END

			-- Deletions associated with Points
			IF 0 <> (SELECT COUNT(*) FROM #SyncTable)
			BEGIN

			  SELECT TOP(@sync_batch_size_tblApplicationStringToPointCategory) WITH TIES CT.PK_ApplicationStringToPointCategoryGuid 'ApplicationStringToPointCategoryGuid', CT.[FK_ParentPK] 'PointGuid', CT.DeletedRowVersion '_RowVersion'
					FROM [track].[tblApplicationStringToPointCategory] CT
						INNER JOIN #SyncTable ON #SyncTable.PK = CT.FK_ParentPK

					WHERE ((CT.DeletedRowVersion IS NOT NULL)
						   AND (CT.DeletedRowVersion > @sync_last_received_anchor_varbinary)
						   AND (CT.DeletedRowVersion <= @sync_new_received_anchor_varbinary)
						   AND (CT.DeletedContext IS NULL OR CT.DeletedContext <> @sync_client_id_binary))     -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
					ORDER BY CT.DeletedRowVersion ASC
			END

			-- Deletions associated with Application String or Entity Assignment Deletion
			ELSE
			BEGIN
			  SELECT TOP(@sync_batch_size_tblApplicationStringToPointCategory) WITH TIES CT.PK_ApplicationStringToPointCategoryGuid 'ApplicationStringToPointCategoryGuid', CT.[FK_ParentPK] 'PointGuid', CT.DeletedRowVersion '_RowVersion'
					FROM [track].[tblApplicationStringToPointCategory] CT
					INNER JOIN [track].[tblPoint] PT ON PT.PK_PointGuid = CT.FK_ParentPK

					WHERE ((PT.CurrentSiteGuid = @sync_context_site_guid)
							AND (PT.DeletedContext IS NULL)
							AND (CT.DeletedRowVersion IS NOT NULL)
							AND (CT.DeletedRowVersion > @sync_last_received_anchor_varbinary)
							AND (CT.DeletedRowVersion <= @sync_new_received_anchor_varbinary)
							AND (CT.DeletedContext IS NULL OR CT.DeletedContext <> @sync_client_id_binary))     -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
					ORDER BY CT.DeletedRowVersion ASC
			END
    END
    ELSE
    BEGIN
        SELECT CT.PK_ApplicationStringToPointCategoryGuid 'ApplicationStringToPointCategoryGuid', CT.[FK_ParentPK] 'PointGuid', CT.DeletedRowVersion '_RowVersion'
            FROM [track].[tblApplicationStringToPointCategory] CT
            WHERE 1=2
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.

    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SID)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
