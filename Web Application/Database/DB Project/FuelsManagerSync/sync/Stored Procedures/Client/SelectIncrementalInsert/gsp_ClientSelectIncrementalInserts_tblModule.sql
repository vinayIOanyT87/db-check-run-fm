-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblModule
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblModule]
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
@sync_batch_size_tblModule int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblModule int
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
        SELECT [dbo].[tblModule].[ID],[dbo].[tblModule].[Description],[dbo].[tblModule].[Standard],[dbo].[tblModule].[ModuleCalculation],[dbo].[tblModule].[ModuleTypeName],[dbo].[tblModule].[ModuleData],[dbo].[tblModule].[ModuleScript],[dbo].[tblModule].[CreatedDate],[dbo].[tblModule].[CreatedBy],[dbo].[tblModule].[UpdatedDate],[dbo].[tblModule].[UpdatedBy],[dbo].[tblModule].[ModuleGuid],[dbo].[tblModule].[SiteGuid], [dbo].[tblModule].[_RowVersion]
            FROM [dbo].[tblModule]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblModule IS NULL OR 
        (@sync_batch_size_tblModule IS NOT NULL AND @sync_batch_size_tblModule = 0))
    BEGIN
        SET @sync_batch_size_tblModule = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblModule) WITH TIES [ID],[Description],[Standard],[ModuleCalculation],[ModuleTypeName],cast([ModuleData] as xml) AS [ModuleData],[ModuleScript],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ModuleGuid],[SiteGuid],_RowVersion
        FROM (
            SELECT [ID],[Description],[Standard],[ModuleCalculation],[ModuleTypeName],[ModuleData],[ModuleScript],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ModuleGuid],[SiteGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblModule) WITH TIES [dbo].[tblModule].[ID],[dbo].[tblModule].[Description],[dbo].[tblModule].[Standard],[dbo].[tblModule].[ModuleCalculation],[dbo].[tblModule].[ModuleTypeName],cast([dbo].[tblModule].[ModuleData] as nvarchar(max)) as [ModuleData],[dbo].[tblModule].[ModuleScript],[dbo].[tblModule].[CreatedDate],[dbo].[tblModule].[CreatedBy],[dbo].[tblModule].[UpdatedDate],[dbo].[tblModule].[UpdatedBy],[dbo].[tblModule].[ModuleGuid],[dbo].[tblModule].[SiteGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblModule]
                        INNER JOIN (SELECT [ModuleToSiteGuid],[PointTemplateToSiteGuid],[ModuleToPointTemplateGuid],[ModuleGuid] FROM [dbo].[udf_GetAssignedModuleForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblModule].[ModuleGuid] = data.[ModuleGuid]
                        INNER JOIN [track].[tblModule] CT
                            ON CT.PK_ModuleGuid = [dbo].[tblModule].[ModuleGuid] 
                        INNER JOIN [track].[tblEntityModuleToSite] MAPCT
                            ON MAPCT.PK_ModuleToSiteGuid = data.[ModuleToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [ID],[Description],[Standard],[ModuleCalculation],[ModuleTypeName],[ModuleData],[ModuleScript],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ModuleGuid],[SiteGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblModule) WITH TIES [dbo].[tblModule].[ID],[dbo].[tblModule].[Description],[dbo].[tblModule].[Standard],[dbo].[tblModule].[ModuleCalculation],[dbo].[tblModule].[ModuleTypeName],cast([dbo].[tblModule].[ModuleData] as nvarchar(max)) as [ModuleData],[dbo].[tblModule].[ModuleScript],[dbo].[tblModule].[CreatedDate],[dbo].[tblModule].[CreatedBy],[dbo].[tblModule].[UpdatedDate],[dbo].[tblModule].[UpdatedBy],[dbo].[tblModule].[ModuleGuid],[dbo].[tblModule].[SiteGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblModule]
                        INNER JOIN (SELECT [ModuleToSiteGuid],[PointTemplateToSiteGuid],[ModuleToPointTemplateGuid],[ModuleGuid] FROM [dbo].[udf_GetAssignedModuleForSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblModule].[ModuleGuid] = data1.[ModuleGuid]
                        INNER JOIN [track].[tblModule] CT
                            ON CT.PK_ModuleGuid = [dbo].[tblModule].[ModuleGuid] 
                        INNER JOIN [track].[tblEntityPointTemplateToSite] MAPCT
                            ON MAPCT.PK_PointTemplateToSiteGuid = data1.[PointTemplateToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs2  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [ID],[Description],[Standard],[ModuleCalculation],[ModuleTypeName],[ModuleData],[ModuleScript],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[ModuleGuid],[SiteGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblModule) WITH TIES [dbo].[tblModule].[ID],[dbo].[tblModule].[Description],[dbo].[tblModule].[Standard],[dbo].[tblModule].[ModuleCalculation],[dbo].[tblModule].[ModuleTypeName],cast([dbo].[tblModule].[ModuleData] as nvarchar(max)) as [ModuleData],[dbo].[tblModule].[ModuleScript],[dbo].[tblModule].[CreatedDate],[dbo].[tblModule].[CreatedBy],[dbo].[tblModule].[UpdatedDate],[dbo].[tblModule].[UpdatedBy],[dbo].[tblModule].[ModuleGuid],[dbo].[tblModule].[SiteGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblModule]
                        INNER JOIN (SELECT [ModuleToSiteGuid],[PointTemplateToSiteGuid],[ModuleToPointTemplateGuid],[ModuleGuid] FROM [dbo].[udf_GetAssignedModuleForSite](@sync_context_site_guid)) data2
                            ON [dbo].[tblModule].[ModuleGuid] = data2.[ModuleGuid]
                        INNER JOIN [track].[tblModule] CT
                            ON CT.PK_ModuleGuid = [dbo].[tblModule].[ModuleGuid] 
                        INNER JOIN [track].[tblModuleToPointTemplate] MAPCT
                            ON MAPCT.PK_ModuleToPointTemplateGuid = data2.[ModuleToPointTemplateGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs3  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ) mainRs
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
