-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalComponentBlendPercentage
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblProcessVariableExternalComponentBlendPercentage]
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
@sync_batch_size_tblProcessVariableExternalComponentBlendPercentage int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProcessVariableExternalComponentBlendPercentage int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblProcessVariableExternalComponentBlendPercentage IS NOT NULL AND @sync_first_time_sync_option_tblProcessVariableExternalComponentBlendPercentage = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCItemID],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DataType],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Quality],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[SIValue],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Maximum],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Minimum],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Input],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[InputEnabled],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[CreatedDate],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[CreatedBy],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[UpdatedBy], [dbo].[tblProcessVariableExternalComponentBlendPercentage].[_RowVersion]
            FROM [dbo].[tblProcessVariableExternalComponentBlendPercentage]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableExternalComponentBlendPercentage IS NULL OR 
        (@sync_batch_size_tblProcessVariableExternalComponentBlendPercentage IS NOT NULL AND @sync_batch_size_tblProcessVariableExternalComponentBlendPercentage = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableExternalComponentBlendPercentage = 2147483647;
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
        SELECT [map].[tblProductToPresetExternalComponent].[ProductToPresetExternalComponentGuid]
                ,[map].[tblProductToPresetExternalComponent].[ProductGuid]
                ,[map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid]
                ,[map].[tblProductToPresetExternalComponent].[TankGuid]
                ,data1.[OwnerSiteGuid]
            FROM [map].[tblProductToPresetExternalComponent]
                INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
                    ON [map].[tblProductToPresetExternalComponent].[ProductGuid] = data1.[ProductGuid]
                INNER JOIN (SELECT [LoadArmGuid],[BayAStationGuid],[BayBStationGuid],[OwnerSiteGuid],[CreatedDate],[UpdatedDate] FROM [dbo].[udf_GetAssociatedLoadArmListForSite](@sync_context_site_guid)) data2
                    ON [map].[tblProductToPresetExternalComponent].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
    )
    SELECT TOP(@sync_batch_size_tblProcessVariableExternalComponentBlendPercentage) WITH TIES [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[InstanceNumber],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProductToPresetExternalComponentGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[OPCItemID],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DataType],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Quality],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[SIValue],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DateTimeStamp],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Maximum],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Minimum],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[DataTypeEnabled],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[Input],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[InputEnabled],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[CreatedDate],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[CreatedBy],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[UpdatedDate],[dbo].[tblProcessVariableExternalComponentBlendPercentage].[UpdatedBy], sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion, NULL) AS '_RowVersion'
        FROM [dbo].[tblProcessVariableExternalComponentBlendPercentage]
            INNER JOIN (SELECT [ProductToPresetExternalComponentGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid] FROM ProductToPresetExternalComponent_CTE) data
                ON [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProductToPresetExternalComponentGuid] = data.[ProductToPresetExternalComponentGuid]
            INNER JOIN [track].[tblProcessVariableExternalComponentBlendPercentage] CT
                ON CT.PK_ProcessVariableProductToPresetExternalComponentGuid = [dbo].[tblProcessVariableExternalComponentBlendPercentage].[ProcessVariableProductToPresetExternalComponentGuid]
            INNER JOIN [track].[tblProductToPresetExternalComponent] MAPCT
                ON MAPCT.PK_ProductToPresetExternalComponentGuid = data.[ProductToPresetExternalComponentGuid]
        WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY _RowVersion ASC


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
