-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProcessVariableOffloadExternalMeter
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblProcessVariableOffloadExternalMeter]
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
@sync_batch_size_tblProcessVariableOffloadExternalMeter int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProcessVariableOffloadExternalMeter int
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblProcessVariableOffloadExternalMeter IS NOT NULL AND @sync_first_time_sync_option_tblProcessVariableOffloadExternalMeter = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblProcessVariableOffloadExternalMeter].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[InstanceNumber],[dbo].[tblProcessVariableOffloadExternalMeter].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[OPCConnectionGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[OPCItemID],[dbo].[tblProcessVariableOffloadExternalMeter].[DataType],[dbo].[tblProcessVariableOffloadExternalMeter].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[Quality],[dbo].[tblProcessVariableOffloadExternalMeter].[SIValue],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[DateTimeStamp],[dbo].[tblProcessVariableOffloadExternalMeter].[Maximum],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[Minimum],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[DataTypeEnabled],[dbo].[tblProcessVariableOffloadExternalMeter].[Input],[dbo].[tblProcessVariableOffloadExternalMeter].[InputEnabled],[dbo].[tblProcessVariableOffloadExternalMeter].[MessageApplicationStringGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[CreatedDate],[dbo].[tblProcessVariableOffloadExternalMeter].[CreatedBy],[dbo].[tblProcessVariableOffloadExternalMeter].[UpdatedDate],[dbo].[tblProcessVariableOffloadExternalMeter].[UpdatedBy], [dbo].[tblProcessVariableOffloadExternalMeter].[_RowVersion]
            FROM [dbo].[tblProcessVariableOffloadExternalMeter]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProcessVariableOffloadExternalMeter IS NULL OR 
        (@sync_batch_size_tblProcessVariableOffloadExternalMeter IS NOT NULL AND @sync_batch_size_tblProcessVariableOffloadExternalMeter = 0))
    BEGIN
        SET @sync_batch_size_tblProcessVariableOffloadExternalMeter = 2147483647;
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
        SELECT TOP(@sync_batch_size_tblProcessVariableOffloadExternalMeter) WITH TIES [dbo].[tblProcessVariableOffloadExternalMeter].[ProcessVariableProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupProcessVariableTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[InstanceNumber],[dbo].[tblProcessVariableOffloadExternalMeter].[ProductToOffloadExternalMeterGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[OPCConnectionGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[OPCItemID],[dbo].[tblProcessVariableOffloadExternalMeter].[DataType],[dbo].[tblProcessVariableOffloadExternalMeter].[ServerEngineeringUnitsIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[Quality],[dbo].[tblProcessVariableOffloadExternalMeter].[SIValue],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupSIValueVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[DateTimeStamp],[dbo].[tblProcessVariableOffloadExternalMeter].[Maximum],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupMaximumVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[Minimum],[dbo].[tblProcessVariableOffloadExternalMeter].[LookupMinimumVariantTypeIndex],[dbo].[tblProcessVariableOffloadExternalMeter].[DataTypeEnabled],[dbo].[tblProcessVariableOffloadExternalMeter].[Input],[dbo].[tblProcessVariableOffloadExternalMeter].[InputEnabled],[dbo].[tblProcessVariableOffloadExternalMeter].[MessageApplicationStringGuid],[dbo].[tblProcessVariableOffloadExternalMeter].[CreatedDate],[dbo].[tblProcessVariableOffloadExternalMeter].[CreatedBy],[dbo].[tblProcessVariableOffloadExternalMeter].[UpdatedDate],[dbo].[tblProcessVariableOffloadExternalMeter].[UpdatedBy], sync.udf_GetMaxRowVersion(CT.UpdatedRowVersion,MAPCT.UpdatedRowVersion, NULL) AS '_RowVersion' 
            FROM [dbo].[tblProcessVariableOffloadExternalMeter]
                INNER JOIN (SELECT [ProductToOffloadExternalMeterGuid],[ProductGuid],[AssignedToLoadArmGuid],[TankGuid],[OwnerSiteGuid] FROM ProductToOffloadExternalMeter_CTE) data
                    ON [dbo].[tblProcessVariableOffloadExternalMeter].[ProductToOffloadExternalMeterGuid] = data.[ProductToOffloadExternalMeterGuid]
                INNER JOIN [track].[tblProcessVariableOffloadExternalMeter] CT
                    ON CT.PK_ProcessVariableProductToOffloadExternalMeterGuid = [dbo].[tblProcessVariableOffloadExternalMeter].[ProcessVariableProductToOffloadExternalMeterGuid]
                INNER JOIN [track].[tblProductToOffloadExternalMeter] MAPCT
                    ON MAPCT.PK_ProductToOffloadExternalMeterGuid = data.[ProductToOffloadExternalMeterGuid]
            WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                    OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                    AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                    AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
        ORDER BY [_RowVersion] ASC;


    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
