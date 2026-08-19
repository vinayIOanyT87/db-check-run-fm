-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalComponentInputPermissive
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalUpdates_tblProcessVariableExternalComponentInputPermissive]
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
@sync_batch_size_tblProcessVariableExternalComponentInputPermissive int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    -- During an initial synchronization, we don't want to bring back any updates since we 
    -- should be picking them up with the select incremental inserts 
    --
    IF ((@sync_request_type = 4)
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DataType],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Quality],[dbo].[tblProcessVariableExternalComponentInputPermissive].[SIValue],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Maximum],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Minimum],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Input],[dbo].[tblProcessVariableExternalComponentInputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalComponentInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalComponentInputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalComponentInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentInputPermissive].[UpdatedBy], [dbo].[tblProcessVariableExternalComponentInputPermissive].[_RowVersion]
            FROM [dbo].[tblProcessVariableExternalComponentInputPermissive]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableExternalComponentInputPermissive IS NULL OR 
        (@sync_batch_size_tblProcessVariableExternalComponentInputPermissive IS NOT NULL AND @sync_batch_size_tblProcessVariableExternalComponentInputPermissive = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableExternalComponentInputPermissive = 2147483647;
    END

        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblProducts and we must go 
        -- through tblProductToPresetInjector.  If you need to change this, it's better to make the changes to the templates 
        -- (client and server) and regenerate this script.  This will keep the templates up-to-date for other developers.
        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblStation and we must go 
        -- through tblLoadArms.  If you need to change this, it's better to make the changes to the templates (client and server) 
        -- and regenerate this script.  This will keep the templates up-to-date for other developers.

        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblProducts and we must go 
        -- through tblProductToPresetExternalComponent.  If you need to change this, it's better to make the changes to the templates 
        -- (client and server) and regenerate this script.  This will keep the templates up-to-date for other developers.
        -- First identify the ProductToPresetExternalComponent records that will be synchronized as an insert.  This is driven by our change tracking as well as by the ProductToPresetExternalComponent change tracking
        ; WITH ProductToPresetExternalComponent_CTE ([ProductToPresetExternalComponentGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid])
        AS (
            SELECT [map].[tblProductToPresetExternalComponent].[ProductToPresetExternalComponentGuid],[map].[tblProductToPresetExternalComponent].[ProductGuid],[map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid],[map].[tblProductToPresetExternalComponent].[TankGuid],data.[OwnerSiteGuid]
                FROM [map].[tblProductToPresetExternalComponent]
                    INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data
                        ON [map].[tblProductToPresetExternalComponent].[ProductGuid] = data.[ProductGuid]
                    INNER JOIN (SELECT [LoadArmGuid],[BayAStationGuid],[BayBStationGuid],[OwnerSiteGuid],[CreatedDate],[UpdatedDate] FROM [dbo].[udf_GetAssociatedLoadArmListForSite](@sync_context_site_guid)) data2
                        ON [map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
        )
        SELECT TOP(@sync_batch_size_tblProcessVariableExternalComponentInputPermissive) WITH TIES [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DataType],[dbo].[tblProcessVariableExternalComponentInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Quality],[dbo].[tblProcessVariableExternalComponentInputPermissive].[SIValue],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Maximum],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Minimum],[dbo].[tblProcessVariableExternalComponentInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentInputPermissive].[Input],[dbo].[tblProcessVariableExternalComponentInputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalComponentInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentInputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalComponentInputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalComponentInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentInputPermissive].[UpdatedBy], sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion, NULL) AS '_RowVersion' 
            FROM [dbo].[tblProcessVariableExternalComponentInputPermissive]
                INNER JOIN (SELECT [ProductToPresetExternalComponentGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid] FROM ProductToPresetExternalComponent_CTE) data
                    ON [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProductToPresetExternalComponentGuid] = data.[ProductToPresetExternalComponentGuid]
                INNER JOIN [track].[tblProcessVariableExternalComponentInputPermissive] CT
                    ON CT.PK_ProcessVariableProductToPresetExternalComponentGuid = [dbo].[tblProcessVariableExternalComponentInputPermissive].[ProcessVariableProductToPresetExternalComponentGuid]
                INNER JOIN [track].[tblProductToPresetExternalComponent] MAPCT
                    ON MAPCT.PK_ProductToPresetExternalComponentGuid = data.[ProductToPresetExternalComponentGuid]
            WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                    AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                    AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
