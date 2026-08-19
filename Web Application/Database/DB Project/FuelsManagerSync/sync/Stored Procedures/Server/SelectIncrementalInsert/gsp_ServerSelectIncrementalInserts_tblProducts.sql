-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProducts
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblProducts]
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
@sync_batch_size_tblProducts int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblProducts int
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblProducts IS NOT NULL AND @sync_first_time_sync_option_tblProducts = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblProducts].[ProductID],[dbo].[tblProducts].[Description],[dbo].[tblProducts].[GenericType],[dbo].[tblProducts].[StockResetDate],[dbo].[tblProducts].[StockTrack],[dbo].[tblProducts].[DensityHighLimit],[dbo].[tblProducts].[DensityLowLimit],[dbo].[tblProducts].[DensityDeadband],[dbo].[tblProducts].[TemperatureHiHiLimit],[dbo].[tblProducts].[TemperatureHighLimit],[dbo].[tblProducts].[TemperatureLowLimit],[dbo].[tblProducts].[TemperatureLoLoLimit],[dbo].[tblProducts].[TemperatureDeadband],[dbo].[tblProducts].[Bonded],[dbo].[tblProducts].[LowStockWarning],[dbo].[tblProducts].[GroundFuel],[dbo].[tblProducts].[ProductCode],[dbo].[tblProducts].[Price],[dbo].[tblProducts].[AviationFuelFlag],[dbo].[tblProducts].[StandardDensity],[dbo].[tblProducts].[ApplyVolumeCorrection],[dbo].[tblProducts].[ApplyStandardDensity],[dbo].[tblProducts].[ApplyDensityLimits],[dbo].[tblProducts].[ApplyTemperatureLimits],[dbo].[tblProducts].[VolumeUnitIndex],[dbo].[tblProducts].[TemperatureUnitIndex],[dbo].[tblProducts].[DensityUnitIndex],[dbo].[tblProducts].[VolumeDecimalPlaces],[dbo].[tblProducts].[TemperatureDecimalPlaces],[dbo].[tblProducts].[DensityDecimalPlaces],[dbo].[tblProducts].[Capitalize],[dbo].[tblProducts].[OctaneNumber],[dbo].[tblProducts].[ReidVaporPressure],[dbo].[tblProducts].[HazardousMaterial],[dbo].[tblProducts].[RegulatoryClass],[dbo].[tblProducts].[LoadRackDisplayText],[dbo].[tblProducts].[ComponentTolerance],[dbo].[tblProducts].[VaporRecovery],[dbo].[tblProducts].[LockedOut],[dbo].[tblProducts].[LockedOutReason],[dbo].[tblProducts].[LockedOutDate],[dbo].[tblProducts].[VarianceTolerance],[dbo].[tblProducts].[DielectricTolerance],[dbo].[tblProducts].[LoadByWeight],[dbo].[tblProducts].[PIDXCode],[dbo].[tblProducts].[ContaminationPromptLoadRackText],[dbo].[tblProducts].[InhibitAccounting],[dbo].[tblProducts].[UserData1],[dbo].[tblProducts].[UserData2],[dbo].[tblProducts].[UserData3],[dbo].[tblProducts].[UserData4],[dbo].[tblProducts].[UserData5],[dbo].[tblProducts].[UserData6],[dbo].[tblProducts].[UserData7],[dbo].[tblProducts].[UserData8],[dbo].[tblProducts].[CreatedDate],[dbo].[tblProducts].[CreatedBy],[dbo].[tblProducts].[UpdatedDate],[dbo].[tblProducts].[UpdatedBy],[dbo].[tblProducts].[MassUnitIndex],[dbo].[tblProducts].[LevelUnitIndex],[dbo].[tblProducts].[FlowUnitIndex],[dbo].[tblProducts].[PressureUnitIndex],[dbo].[tblProducts].[MassDecimalPlaces],[dbo].[tblProducts].[LevelDecimalPlaces],[dbo].[tblProducts].[FlowDecimalPlaces],[dbo].[tblProducts].[PressureDecimalPlaces],[dbo].[tblProducts].[VolumePackageSize],[dbo].[tblProducts].[MassPackageSize],[dbo].[tblProducts].[ProductGuid],[dbo].[tblProducts].[SiteGuid],[dbo].[tblProducts].[LookupProductTypeIndex],[dbo].[tblProducts].[TrackingProductGuid],[dbo].[tblProducts].[TaxCode],[dbo].[tblProducts].[VcfModuleSettings],[dbo].[tblProducts].[ProductColor],[dbo].[tblProducts].[PatternColor],[dbo].[tblProducts].[PatternNumber],[dbo].[tblProducts].[_MasterRecordGuid],[dbo].[tblProducts].[HiddenDate],[dbo].[tblProducts].[AutomaticCloseout],[dbo].[tblProducts].[PIDXFamilyCode],[dbo].[tblProducts].[IsEthanol], [dbo].[tblProducts].[_RowVersion]
            FROM [dbo].[tblProducts]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblProducts IS NULL OR 
        (@sync_batch_size_tblProducts IS NOT NULL AND @sync_batch_size_tblProducts = 0))
    BEGIN
        SET @sync_batch_size_tblProducts = 2147483647;
    END


    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT [ProductID],[Description],[GenericType],[StockResetDate],[StockTrack],[DensityHighLimit],[DensityLowLimit],[DensityDeadband],[TemperatureHiHiLimit],[TemperatureHighLimit],[TemperatureLowLimit],[TemperatureLoLoLimit],[TemperatureDeadband],[Bonded],[LowStockWarning],[GroundFuel],[ProductCode],[Price],[AviationFuelFlag],[StandardDensity],[ApplyVolumeCorrection],[ApplyStandardDensity],[ApplyDensityLimits],[ApplyTemperatureLimits],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[Capitalize],[OctaneNumber],[ReidVaporPressure],[HazardousMaterial],[RegulatoryClass],[LoadRackDisplayText],[ComponentTolerance],[VaporRecovery],[LockedOut],[LockedOutReason],[LockedOutDate],[VarianceTolerance],[DielectricTolerance],[LoadByWeight],[PIDXCode],[ContaminationPromptLoadRackText],[InhibitAccounting],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MassUnitIndex],[LevelUnitIndex],[FlowUnitIndex],[PressureUnitIndex],[MassDecimalPlaces],[LevelDecimalPlaces],[FlowDecimalPlaces],[PressureDecimalPlaces],[VolumePackageSize],[MassPackageSize],[ProductGuid],[SiteGuid],[LookupProductTypeIndex],[TrackingProductGuid],[TaxCode],[VcfModuleSettings],[ProductColor],[PatternColor],[PatternNumber],[_MasterRecordGuid],[HiddenDate],[AutomaticCloseout],[PIDXFamilyCode],[IsEthanol],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblProducts) WITH TIES [dbo].[tblProducts].[ProductID],[dbo].[tblProducts].[Description],[dbo].[tblProducts].[GenericType],[dbo].[tblProducts].[StockResetDate],[dbo].[tblProducts].[StockTrack],[dbo].[tblProducts].[DensityHighLimit],[dbo].[tblProducts].[DensityLowLimit],[dbo].[tblProducts].[DensityDeadband],[dbo].[tblProducts].[TemperatureHiHiLimit],[dbo].[tblProducts].[TemperatureHighLimit],[dbo].[tblProducts].[TemperatureLowLimit],[dbo].[tblProducts].[TemperatureLoLoLimit],[dbo].[tblProducts].[TemperatureDeadband],[dbo].[tblProducts].[Bonded],[dbo].[tblProducts].[LowStockWarning],[dbo].[tblProducts].[GroundFuel],[dbo].[tblProducts].[ProductCode],[dbo].[tblProducts].[Price],[dbo].[tblProducts].[AviationFuelFlag],[dbo].[tblProducts].[StandardDensity],[dbo].[tblProducts].[ApplyVolumeCorrection],[dbo].[tblProducts].[ApplyStandardDensity],[dbo].[tblProducts].[ApplyDensityLimits],[dbo].[tblProducts].[ApplyTemperatureLimits],[dbo].[tblProducts].[VolumeUnitIndex],[dbo].[tblProducts].[TemperatureUnitIndex],[dbo].[tblProducts].[DensityUnitIndex],[dbo].[tblProducts].[VolumeDecimalPlaces],[dbo].[tblProducts].[TemperatureDecimalPlaces],[dbo].[tblProducts].[DensityDecimalPlaces],[dbo].[tblProducts].[Capitalize],[dbo].[tblProducts].[OctaneNumber],[dbo].[tblProducts].[ReidVaporPressure],[dbo].[tblProducts].[HazardousMaterial],[dbo].[tblProducts].[RegulatoryClass],[dbo].[tblProducts].[LoadRackDisplayText],[dbo].[tblProducts].[ComponentTolerance],[dbo].[tblProducts].[VaporRecovery],[dbo].[tblProducts].[LockedOut],[dbo].[tblProducts].[LockedOutReason],[dbo].[tblProducts].[LockedOutDate],[dbo].[tblProducts].[VarianceTolerance],[dbo].[tblProducts].[DielectricTolerance],[dbo].[tblProducts].[LoadByWeight],[dbo].[tblProducts].[PIDXCode],[dbo].[tblProducts].[ContaminationPromptLoadRackText],[dbo].[tblProducts].[InhibitAccounting],[dbo].[tblProducts].[UserData1],[dbo].[tblProducts].[UserData2],[dbo].[tblProducts].[UserData3],[dbo].[tblProducts].[UserData4],[dbo].[tblProducts].[UserData5],[dbo].[tblProducts].[UserData6],[dbo].[tblProducts].[UserData7],[dbo].[tblProducts].[UserData8],[dbo].[tblProducts].[CreatedDate],[dbo].[tblProducts].[CreatedBy],[dbo].[tblProducts].[UpdatedDate],[dbo].[tblProducts].[UpdatedBy],[dbo].[tblProducts].[MassUnitIndex],[dbo].[tblProducts].[LevelUnitIndex],[dbo].[tblProducts].[FlowUnitIndex],[dbo].[tblProducts].[PressureUnitIndex],[dbo].[tblProducts].[MassDecimalPlaces],[dbo].[tblProducts].[LevelDecimalPlaces],[dbo].[tblProducts].[FlowDecimalPlaces],[dbo].[tblProducts].[PressureDecimalPlaces],[dbo].[tblProducts].[VolumePackageSize],[dbo].[tblProducts].[MassPackageSize],[dbo].[tblProducts].[ProductGuid],[dbo].[tblProducts].[SiteGuid],[dbo].[tblProducts].[LookupProductTypeIndex],[dbo].[tblProducts].[TrackingProductGuid],[dbo].[tblProducts].[TaxCode],[dbo].[tblProducts].[VcfModuleSettings],[dbo].[tblProducts].[ProductColor],[dbo].[tblProducts].[PatternColor],[dbo].[tblProducts].[PatternNumber],[dbo].[tblProducts].[_MasterRecordGuid],[dbo].[tblProducts].[HiddenDate],[dbo].[tblProducts].[AutomaticCloseout],[dbo].[tblProducts].[PIDXFamilyCode],[dbo].[tblProducts].[IsEthanol],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblProducts]
                    INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblProducts].[ProductGuid] = data.[ProductGuid]
                    INNER JOIN [track].[tblProducts] CT
                        ON CT.PK_ProductGuid = [dbo].[tblProducts].[ProductGuid] 
                    INNER JOIN [track].[tblEntityProductToSite] MAPCT
                        ON MAPCT.PK_ProductToSiteGuid = data.[ProductToSiteGuid] 
                WHERE (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_client_id_binary))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                        OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                        AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                        AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_client_id_binary)))   -- USE THE CLIENT ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
