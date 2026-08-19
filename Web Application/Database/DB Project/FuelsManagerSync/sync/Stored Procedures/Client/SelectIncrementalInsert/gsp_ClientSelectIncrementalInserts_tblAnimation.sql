-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAnimation
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblAnimation]
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
@sync_batch_size_tblAnimation int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblAnimation int
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
        SELECT [dbo].[tblAnimation].[AnimationGuid],[dbo].[tblAnimation].[ID],[dbo].[tblAnimation].[SiteGuid],[dbo].[tblAnimation].[AnimationTestGroupList],[dbo].[tblAnimation].[CreatedDate],[dbo].[tblAnimation].[CreatedBy],[dbo].[tblAnimation].[UpdatedDate],[dbo].[tblAnimation].[UpdatedBy], [dbo].[tblAnimation].[_RowVersion]
            FROM [dbo].[tblAnimation]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblAnimation IS NULL OR 
        (@sync_batch_size_tblAnimation IS NOT NULL AND @sync_batch_size_tblAnimation = 0))
    BEGIN
        SET @sync_batch_size_tblAnimation = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblAnimation) WITH TIES [AnimationGuid],[ID],[SiteGuid],[AnimationTestGroupList],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
        FROM (
            SELECT [AnimationGuid],[ID],[SiteGuid],[AnimationTestGroupList],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblAnimation) WITH TIES [dbo].[tblAnimation].[AnimationGuid],[dbo].[tblAnimation].[ID],[dbo].[tblAnimation].[SiteGuid],cast([dbo].[tblAnimation].[AnimationTestGroupList] as nvarchar(max)) as [AnimationTestGroupList],[dbo].[tblAnimation].[CreatedDate],[dbo].[tblAnimation].[CreatedBy],[dbo].[tblAnimation].[UpdatedDate],[dbo].[tblAnimation].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblAnimation]
                        INNER JOIN (SELECT [AnimationGuid],[PointTemplateToSiteGuid] FROM [dbo].[udf_GetAssignedPointDetailAnimationListPerSite](@sync_context_site_guid)) data
                            ON [dbo].[tblAnimation].[AnimationGuid] = data.[AnimationGuid]
                        INNER JOIN [track].[tblAnimation] CT
                            ON CT.PK_AnimationGuid = [dbo].[tblAnimation].[AnimationGuid] 
                        INNER JOIN [track].[tblEntityPointTemplateToSite] MAPCT
                            ON MAPCT.PK_PointTemplateToSiteGuid = data.[PointTemplateToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [AnimationGuid],[ID],[SiteGuid],[AnimationTestGroupList],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblAnimation) WITH TIES [dbo].[tblAnimation].[AnimationGuid],[dbo].[tblAnimation].[ID],[dbo].[tblAnimation].[SiteGuid],cast([dbo].[tblAnimation].[AnimationTestGroupList] as nvarchar(max)) as [AnimationTestGroupList],[dbo].[tblAnimation].[CreatedDate],[dbo].[tblAnimation].[CreatedBy],[dbo].[tblAnimation].[UpdatedDate],[dbo].[tblAnimation].[UpdatedBy],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,NULL,NULL) AS '_RowVersion'
                    FROM [dbo].[tblAnimation]
                        INNER JOIN (SELECT [AnimationGuid] FROM [dbo].[udf_GetAssignedAnimationListPerSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblAnimation].[AnimationGuid] = data1.[AnimationGuid]
                        INNER JOIN [track].[tblAnimation] CT
                            ON CT.PK_AnimationGuid = [dbo].[tblAnimation].[AnimationGuid] 
                WHERE ((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs2  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ) mainRs
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
