-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblOPCConnections
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblOPCConnections]
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
@sync_batch_size_tblOPCConnections int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblOPCConnections int
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
        SELECT [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid], [dbo].[tblOPCConnections].[_RowVersion]
            FROM [dbo].[tblOPCConnections]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblOPCConnections IS NULL OR 
        (@sync_batch_size_tblOPCConnections IS NOT NULL AND @sync_batch_size_tblOPCConnections = 0))
    BEGIN
        SET @sync_batch_size_tblOPCConnections = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
        FROM (
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableSiteGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableSiteListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableSite] MAPCT
                            ON MAPCT.PK_ProcessVariableSiteGuid = data.[ProcessVariableSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableTankGuid],[TankGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableTankListForSite](@sync_context_site_guid)) data1
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data1.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableTank] MAPCT
                            ON MAPCT.PK_ProcessVariableTankGuid = data1.[ProcessVariableTankGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs2  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableEquipmentGuid],[EquipmentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableEquipmentListForSite](@sync_context_site_guid)) data2
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data2.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableEquipment] MAPCT
                            ON MAPCT.PK_ProcessVariableEquipmentGuid = data2.[ProcessVariableEquipmentGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs3  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableStationListForSite](@sync_context_site_guid)) data3
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data3.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableStation] MAPCT
                            ON MAPCT.PK_ProcessVariableStationGuid = data3.[ProcessVariableStationGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs4  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableStationInputPermissiveListForSite](@sync_context_site_guid)) data4
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data4.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableStationInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableStationGuid = data4.[ProcessVariableStationGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs5  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableStationGuid],[StationGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableStationOutputPermissiveListForSite](@sync_context_site_guid)) data5
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data5.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableStationOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableStationGuid = data5.[ProcessVariableStationGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs6  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableLoadArmListForSite](@sync_context_site_guid)) data6
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data6.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableLoadArm] MAPCT
                            ON MAPCT.PK_ProcessVariableLoadArmGuid = data6.[ProcessVariableLoadArmGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs7  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableLoadArmInputPermissiveListForSite](@sync_context_site_guid)) data7
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data7.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableLoadArmInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableLoadArmGuid = data7.[ProcessVariableLoadArmGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs8  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableLoadArmOutputPermissiveListForSite](@sync_context_site_guid)) data8
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data8.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableLoadArmOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableLoadArmGuid = data8.[ProcessVariableLoadArmGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs9  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableNoAdditiveInputPermissiveListForSite](@sync_context_site_guid)) data9
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data9.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableNoAdditiveInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableLoadArmGuid = data9.[ProcessVariableLoadArmGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs10  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableLoadArmGuid],[LoadArmGuid],[OPCConnectionGuid],[StationGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableNoAdditiveOutputPermissiveListForSite](@sync_context_site_guid)) data10
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data10.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableNoAdditiveOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableLoadArmGuid = data10.[ProcessVariableLoadArmGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs11  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableAdditiveInputPermissiveListForSite](@sync_context_site_guid)) data11
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data11.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableAdditiveInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetInjectorGuid = data11.[ProcessVariableProductToPresetInjectorGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs12  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableAdditiveOutputPermissiveListForSite](@sync_context_site_guid)) data12
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data12.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableAdditiveOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetInjectorGuid = data12.[ProcessVariableProductToPresetInjectorGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs13  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[ProductToPresetComponentTankOrTankGroupGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableComponentInputPermissiveListForSite](@sync_context_site_guid)) data13
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data13.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableComponentInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid = data13.[ProcessVariableProductToPresetComponentTankOrTankGroupGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs14  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetComponentTankOrTankGroupGuid],[ProductToPresetComponentTankOrTankGroupGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableComponentOutputPermissiveListForSite](@sync_context_site_guid)) data14
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data14.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableComponentOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetComponentTankOrTankGroupGuid = data14.[ProcessVariableProductToPresetComponentTankOrTankGroupGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs15  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableExternalComponentBlendPercentageListForSite](@sync_context_site_guid)) data15
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data15.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableExternalComponentBlendPercentage] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetExternalComponentGuid = data15.[ProcessVariableProductToPresetExternalComponentGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs16  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableExternalComponentInputPermissiveListForSite](@sync_context_site_guid)) data16
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data16.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableExternalComponentInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetExternalComponentGuid = data16.[ProcessVariableProductToPresetExternalComponentGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs17  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetExternalComponentGuid],[ProductToPresetExternalComponentGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableExternalComponentOutputPermissiveListForSite](@sync_context_site_guid)) data17
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data17.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableExternalComponentOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetExternalComponentGuid = data17.[ProcessVariableProductToPresetExternalComponentGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs18  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariablePresetInjectorGuid],[ProductToPresetInjectorGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariablePresetInjectorListForSite](@sync_context_site_guid)) data18
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data18.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariablePresetInjector] MAPCT
                            ON MAPCT.PK_ProcessVariablePresetInjectorGuid = data18.[ProcessVariablePresetInjectorGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs19  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetRecipeGuid],[ProductToPresetRecipeGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableRecipeInputPermissiveListForSite](@sync_context_site_guid)) data19
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data19.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableRecipeInputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetRecipeGuid = data19.[ProcessVariableProductToPresetRecipeGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs20  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
            UNION
            SELECT [URL],[ProgID],[CreatedDate],[CreatedBy],[OPCConnectionGuid],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblOPCConnections) WITH TIES [dbo].[tblOPCConnections].[URL],[dbo].[tblOPCConnections].[ProgID],[dbo].[tblOPCConnections].[CreatedDate],[dbo].[tblOPCConnections].[CreatedBy],[dbo].[tblOPCConnections].[OPCConnectionGuid],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblOPCConnections]
                        INNER JOIN (SELECT [ProcessVariableProductToPresetRecipeGuid],[ProductToPresetRecipeGuid],[OPCConnectionGuid],[MessageApplicationStringGuid],[OwnerSiteGuid] FROM [dbo].[udf_GetAssociatedProcessVariableRecipeOutputPermissiveListForSite](@sync_context_site_guid)) data20
                            ON [dbo].[tblOPCConnections].[OPCConnectionGuid] = data20.[OPCConnectionGuid]
                        INNER JOIN [track].[tblOPCConnections] CT
                            ON CT.PK_OPCConnectionGuid = [dbo].[tblOPCConnections].[OPCConnectionGuid] 
                        INNER JOIN [track].[tblProcessVariableRecipeOutputPermissive] MAPCT
                            ON MAPCT.PK_ProcessVariableProductToPresetRecipeGuid = data20.[ProcessVariableProductToPresetRecipeGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs21  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ) mainRs
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
