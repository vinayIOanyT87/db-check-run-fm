-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableRecipeOutputPermissive
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblProcessVariableRecipeOutputPermissive]
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
@sync_batch_size_tblProcessVariableRecipeOutputPermissive int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProcessVariableRecipeOutputPermissive int
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
        SELECT [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableRecipeOutputPermissive].[ProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableRecipeOutputPermissive].[DataType],[dbo].[tblProcessVariableRecipeOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[Quality],[dbo].[tblProcessVariableRecipeOutputPermissive].[SIValue],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableRecipeOutputPermissive].[Maximum],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[Minimum],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableRecipeOutputPermissive].[Input],[dbo].[tblProcessVariableRecipeOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableRecipeOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableRecipeOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableRecipeOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableRecipeOutputPermissive].[UpdatedBy], [dbo].[tblProcessVariableRecipeOutputPermissive].[_RowVersion]
            FROM [dbo].[tblProcessVariableRecipeOutputPermissive]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableRecipeOutputPermissive IS NULL OR 
        (@sync_batch_size_tblProcessVariableRecipeOutputPermissive IS NOT NULL AND @sync_batch_size_tblProcessVariableRecipeOutputPermissive = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableRecipeOutputPermissive = 2147483647;
    END

        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblProducts and we must go 
        -- through tblProductToPresetInjector.  If you need to change this, it's better to make the changes to the templates 
        -- (client and server) and regenerate this script.  This will keep the templates up-to-date for other developers.
        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblStation and we must go 
        -- through tblLoadArms.  If you need to change this, it's better to make the changes to the templates (client and server) 
        -- and regenerate this script.  This will keep the templates up-to-date for other developers.

        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblProducts and we must go 
        -- through tblProductToPresetRecipe.  If you need to change this, it's better to make the changes to the templates 
        -- (client and server) and regenerate this script.  This will keep the templates up-to-date for other developers.
        -- First identify the ProductToPresetRecipe records that will be synchronized as an insert.  This is driven by our change tracking as well as by the ProductToPresetRecipe change tracking
        ; WITH ProductToPresetRecipe_CTE ([ProductToPresetRecipeGuid],[ProductGuid],[AssignedToLoadArmGuid],[OwnerSiteGuid])
        AS (
        SELECT [map].[tblProductToPresetRecipe].[ProductToPresetRecipeGuid]
                ,[map].[tblProductToPresetRecipe].[ProductGuid]
                ,[map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid]
                ,data.[OwnerSiteGuid]
            FROM [map].[tblProductToPresetRecipe]
                INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data
                    ON [map].[tblProductToPresetRecipe].[ProductGuid] = data.[ProductGuid]
                INNER JOIN (SELECT [LoadArmGuid],[BayAStationGuid],[BayBStationGuid],[OwnerSiteGuid],[CreatedDate],[UpdatedDate] FROM [dbo].[udf_GetAssociatedLoadArmListForSite](@sync_context_site_guid)) data2
                    ON [map].[tblProductToPresetRecipe].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
        )
        -- Now that we know which ProductToPresetRecipe records would have been inserted, we can focus on whether these records have been inserted or the loadarm, similar to an entity assignment
        SELECT TOP(@sync_batch_size_tblProcessVariableRecipeOutputPermissive) WITH TIES [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableRecipeOutputPermissive].[ProductToPresetRecipeGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableRecipeOutputPermissive].[DataType],[dbo].[tblProcessVariableRecipeOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[Quality],[dbo].[tblProcessVariableRecipeOutputPermissive].[SIValue],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableRecipeOutputPermissive].[Maximum],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[Minimum],[dbo].[tblProcessVariableRecipeOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableRecipeOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableRecipeOutputPermissive].[Input],[dbo].[tblProcessVariableRecipeOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableRecipeOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableRecipeOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableRecipeOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableRecipeOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableRecipeOutputPermissive].[UpdatedBy], sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion, NULL) AS '_RowVersion' 
            FROM [dbo].[tblProcessVariableRecipeOutputPermissive]
                INNER JOIN (SELECT [ProductToPresetRecipeGuid],[ProductGuid],[AssignedToLoadArmGuid],[OwnerSiteGuid] FROM ProductToPresetRecipe_CTE) data
                    ON [dbo].[tblProcessVariableRecipeOutputPermissive].[ProductToPresetRecipeGuid] = data.[ProductToPresetRecipeGuid]
                INNER JOIN [track].[tblProcessVariableRecipeOutputPermissive] CT
                    ON CT.PK_ProcessVariableProductToPresetRecipeGuid = [dbo].[tblProcessVariableRecipeOutputPermissive].[ProcessVariableProductToPresetRecipeGuid]
                INNER JOIN [track].[tblProductToPresetRecipe] MAPCT
                    ON MAPCT.PK_ProductToPresetRecipeGuid = data.[ProductToPresetRecipeGuid]
            WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
            ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
