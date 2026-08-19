-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalMeterOutputPermissive
-- Description: Get Updated Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalUpdates_tblProcessVariableExternalMeterOutputPermissive]
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
@sync_batch_size_tblProcessVariableExternalMeterOutputPermissive int,
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
        SELECT [dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DataType],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Quality],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[SIValue],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Maximum],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Minimum],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Input],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[UpdatedBy], [dbo].[tblProcessVariableExternalMeterOutputPermissive].[_RowVersion]
            FROM [dbo].[tblProcessVariableExternalMeterOutputPermissive]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableExternalMeterOutputPermissive IS NULL OR 
        (@sync_batch_size_tblProcessVariableExternalMeterOutputPermissive IS NOT NULL AND @sync_batch_size_tblProcessVariableExternalMeterOutputPermissive = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableExternalMeterOutputPermissive = 2147483647;
    END

        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblProducts and we must go 
        -- through tblProductToOffloadExternalMeter.  If you need to change this, it's better to make the changes to the templates 
        -- (client and server) and regenerate this script.  This will keep the templates up-to-date for other developers.
        -- CREATED BY THE CodeSmith Template Engine using a static template because the SiteGuid is in tblStation and we must go 
        -- through tblLoadArms.  If you need to change this, it's better to make the changes to the templates (client and server) 
        -- and regenerate this script.  This will keep the templates up-to-date for other developers.

        -- First identify the ProductToOffloadExternalMeter records that will be synchronized as an insert.  This is driven by our change tracking as well as by the ProductToOffloadExternalMeter change tracking
        ; WITH ProductToOffloadExternalMeter_CTE ([ProductToOffloadExternalMeterGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid])
        AS (
            SELECT [map].[tblProductToOffloadExternalMeter].[ProductToOffloadExternalMeterGuid],[map].[tblProductToOffloadExternalMeter].[ProductGuid],[map].[tblProductToOffloadExternalMeter].[AssignedToLoadArmGuid],[map].[tblProductToOffloadExternalMeter].[TankGuid],data.[OwnerSiteGuid]
                FROM [map].[tblProductToOffloadExternalMeter]
                    INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data
                        ON [map].[tblProductToOffloadExternalMeter].[ProductGuid] = data.[ProductGuid]
                    INNER JOIN (SELECT [LoadArmGuid],[BayAStationGuid],[BayBStationGuid],[OwnerSiteGuid],[CreatedDate],[UpdatedDate] FROM [dbo].[udf_GetAssociatedLoadArmListForSite](@sync_context_site_guid)) data2
                        ON [map].[tblProductToOffloadExternalMeter].[AssignedToLoadArmGuid] = data2.[LoadArmGuid]
        )
        SELECT TOP(@sync_batch_size_tblProcessVariableExternalMeterOutputPermissive) WITH TIES [dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DataType],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Quality],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[SIValue],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Maximum],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Minimum],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[Input],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalMeterOutputPermissive].[UpdatedBy], sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion, NULL) AS '_RowVersion' 
            FROM [dbo].[tblProcessVariableExternalMeterOutputPermissive]
                INNER JOIN (SELECT [ProductToOffloadExternalMeterGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid] FROM ProductToOffloadExternalMeter_CTE) data
                    ON [dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProductToOffloadExternalMeterGuid] = data.[ProductToOffloadExternalMeterGuid]
                INNER JOIN [track].[tblProcessVariableExternalMeterOutputPermissive] CT
                    ON CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = [dbo].[tblProcessVariableExternalMeterOutputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid]
                INNER JOIN [track].[tblProductToOffloadExternalMeter] MAPCT
                    ON MAPCT.PK_ProductToOffloadExternalMeterGuid = data.[ProductToOffloadExternalMeterGuid]
            WHERE (((CT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.UpdatedRowVersion > CT.InsertedRowVersion)
                    AND (CT.UpdatedContext IS NULL OR CT.UpdatedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    OR ((MAPCT.UpdatedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (MAPCT.UpdatedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (MAPCT.UpdatedRowVersion > MAPCT.InsertedRowVersion)
                    AND (MAPCT.UpdatedContext IS NULL OR MAPCT.UpdatedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SIU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
