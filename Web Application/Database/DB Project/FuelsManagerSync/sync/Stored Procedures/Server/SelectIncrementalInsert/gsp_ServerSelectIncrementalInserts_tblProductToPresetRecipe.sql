-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblProductToPresetRecipe
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblProductToPresetRecipe]
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
@sync_batch_size_tblProductToPresetRecipe int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProductToPresetRecipe int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblProductToPresetRecipe IS NOT NULL AND @sync_first_time_sync_option_tblProductToPresetRecipe = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid],[map].[tblProductToPresetRecipe].[ProductGuid],[map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid],[map].[tblProductToPresetRecipe].[Sequence],[map].[tblProductToPresetRecipe].[BlendPercentage],[map].[tblProductToPresetRecipe].[AdditiveRate],[map].[tblProductToPresetRecipe].[Ratio],[map].[tblProductToPresetRecipe].[AdditiveCycleVolume],[map].[tblProductToPresetRecipe].[Tolerance],[map].[tblProductToPresetRecipe].[PresetNumber],[map].[tblProductToPresetRecipe].[AdditiveProfileGuid],[map].[tblProductToPresetRecipe].[TankGuid],[map].[tblProductToPresetRecipe].[MeterID],[map].[tblProductToPresetRecipe].[ShipToProductID],[map].[tblProductToPresetRecipe].[ShipToProductCode],[map].[tblProductToPresetRecipe].[ShipToLoadRackDisplayText],[map].[tblProductToPresetRecipe].[UnavailableInventoryGross],[map].[tblProductToPresetRecipe].[UnavailableInventoryNet],[map].[tblProductToPresetRecipe].[CreatedDate],[map].[tblProductToPresetRecipe].[CreatedBy],[map].[tblProductToPresetRecipe].[UpdatedDate],[map].[tblProductToPresetRecipe].[UpdatedBy],[map].[tblProductToPresetRecipe].[EnableRecipe], [map].[tblProductToPresetRecipe].[_RowVersion]
            FROM [map].[tblProductToPresetRecipe]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProductToPresetRecipe IS NULL OR 
        (@sync_batch_size_tblProductToPresetRecipe IS NOT NULL AND @sync_batch_size_tblProductToPresetRecipe = 0))
    BEGIN
        SET @sync_batch_size_tblProductToPresetRecipe = 2147483647;
    END

    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT TOP(@sync_batch_size_tblProductToPresetRecipe) WITH TIES [ProductToPresetRecipeGuid],[ProductGuid],[AssignedToLoadArmGuid],[Sequence],[BlendPercentage],[AdditiveRate],[Ratio],[AdditiveCycleVolume],[Tolerance],[PresetNumber],[AdditiveProfileGuid],[TankGuid],[MeterID],[ShipToProductID],[ShipToProductCode],[ShipToLoadRackDisplayText],[UnavailableInventoryGross],[UnavailableInventoryNet],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableRecipe],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblProductToPresetRecipe) WITH TIES [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid],[map].[tblProductToPresetRecipe].[ProductGuid],[map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid],[map].[tblProductToPresetRecipe].[Sequence],[map].[tblProductToPresetRecipe].[BlendPercentage],[map].[tblProductToPresetRecipe].[AdditiveRate],[map].[tblProductToPresetRecipe].[Ratio],[map].[tblProductToPresetRecipe].[AdditiveCycleVolume],[map].[tblProductToPresetRecipe].[Tolerance],[map].[tblProductToPresetRecipe].[PresetNumber],[map].[tblProductToPresetRecipe].[AdditiveProfileGuid],[map].[tblProductToPresetRecipe].[TankGuid],[map].[tblProductToPresetRecipe].[MeterID],[map].[tblProductToPresetRecipe].[ShipToProductID],[map].[tblProductToPresetRecipe].[ShipToProductCode],[map].[tblProductToPresetRecipe].[ShipToLoadRackDisplayText],[map].[tblProductToPresetRecipe].[UnavailableInventoryGross],[map].[tblProductToPresetRecipe].[UnavailableInventoryNet],[map].[tblProductToPresetRecipe].[CreatedDate],[map].[tblProductToPresetRecipe].[CreatedBy],[map].[tblProductToPresetRecipe].[UpdatedDate],[map].[tblProductToPresetRecipe].[UpdatedBy],[map].[tblProductToPresetRecipe].[EnableRecipe],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,MAPCT2.InsertedRowVersion) AS '_RowVersion'
                FROM [map].[tblProductToPresetRecipe]
                    INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data
                        ON [map].[tblProductToPresetRecipe].[ProductGuid] = data.[ProductGuid]
                    INNER JOIN (SELECT [LoadArmGuid],[BayAStationGuid],[BayBStationGuid],[OwnerSiteGuid],[CreatedDate],[UpdatedDate] FROM [dbo].[udf_GetAssociatedLoadArmListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid] = data1.[LoadArmGuid]
                    INNER JOIN [track].[tblProductToPresetRecipe] CT
                        ON CT.PK_ProductToPresetRecipeGuid = [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid] 
                    INNER JOIN [track].[tblEntityProductToSite] MAPCT
                        ON MAPCT.PK_ProductToSiteGuid = data.[ProductToSiteGuid]
                    INNER JOIN [track].[tblLoadArms] MAPCT2
                        ON MAPCT2.PK_LoadArmGuid = data1.[LoadArmGuid]
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT2.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT2.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT2.InsertedContext IS NULL OR MAPCT2.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1   -- DetectedSubFunctions: True / IncludeEntityAssignments: True
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
