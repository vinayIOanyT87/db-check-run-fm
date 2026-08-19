-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableExternalMeterInputPermissive
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblProcessVariableExternalMeterInputPermissive]
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
@sync_batch_size_tblProcessVariableExternalMeterInputPermissive int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProcessVariableExternalMeterInputPermissive int
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
        SELECT [dbo].[tblProcessVariableExternalMeterInputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalMeterInputPermissive].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DataType],[dbo].[tblProcessVariableExternalMeterInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Quality],[dbo].[tblProcessVariableExternalMeterInputPermissive].[SIValue],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Maximum],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Minimum],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Input],[dbo].[tblProcessVariableExternalMeterInputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalMeterInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalMeterInputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalMeterInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalMeterInputPermissive].[UpdatedBy], [dbo].[tblProcessVariableExternalMeterInputPermissive].[_RowVersion]
            FROM [dbo].[tblProcessVariableExternalMeterInputPermissive]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableExternalMeterInputPermissive IS NULL OR 
        (@sync_batch_size_tblProcessVariableExternalMeterInputPermissive IS NOT NULL AND @sync_batch_size_tblProcessVariableExternalMeterInputPermissive = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableExternalMeterInputPermissive = 2147483647;
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
        SELECT TOP(@sync_batch_size_tblProcessVariableExternalMeterInputPermissive) WITH TIES [dbo].[tblProcessVariableExternalMeterInputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[InstanceNumber],[dbo].[tblProcessVariableExternalMeterInputPermissive].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[OPCConnectionGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[OPCItemID],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DataType],[dbo].[tblProcessVariableExternalMeterInputPermissive].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Quality],[dbo].[tblProcessVariableExternalMeterInputPermissive].[SIValue],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DateTimeStamp],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Maximum],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Minimum],[dbo].[tblProcessVariableExternalMeterInputPermissive].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableExternalMeterInputPermissive].[DataTypeEnabled],[dbo].[tblProcessVariableExternalMeterInputPermissive].[Input],[dbo].[tblProcessVariableExternalMeterInputPermissive].[InputEnabled],[dbo].[tblProcessVariableExternalMeterInputPermissive].[MessageApplicationStringGuid],[dbo].[tblProcessVariableExternalMeterInputPermissive].[CreatedDate],[dbo].[tblProcessVariableExternalMeterInputPermissive].[CreatedBy],[dbo].[tblProcessVariableExternalMeterInputPermissive].[UpdatedDate],[dbo].[tblProcessVariableExternalMeterInputPermissive].[UpdatedBy], sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion, NULL) AS '_RowVersion' 
            FROM [dbo].[tblProcessVariableExternalMeterInputPermissive]
                INNER JOIN (SELECT [ProductToOffloadExternalMeterGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid] FROM ProductToOffloadExternalMeter_CTE) data
                    ON [dbo].[tblProcessVariableExternalMeterInputPermissive].[ProductToOffloadExternalMeterGuid] = data.[ProductToOffloadExternalMeterGuid]
                INNER JOIN [track].[tblProcessVariableExternalMeterInputPermissive] CT
                    ON CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = [dbo].[tblProcessVariableExternalMeterInputPermissive].[ProcessVariableProductToOffloadExternalMeterGuid]
                INNER JOIN [track].[tblProductToOffloadExternalMeter] MAPCT
                    ON MAPCT.PK_ProductToOffloadExternalMeterGuid = data.[ProductToOffloadExternalMeterGuid]
            WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY [_RowVersion] ASC;

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
