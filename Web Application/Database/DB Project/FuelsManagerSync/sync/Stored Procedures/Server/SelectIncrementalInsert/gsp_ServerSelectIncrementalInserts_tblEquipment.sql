-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipment
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectIncrementalInserts_tblEquipment]
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
@sync_batch_size_tblEquipment int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblEquipment int
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)
    DECLARE @sync_new_received_anchor_varbinary varbinary(8)

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);
    SET @sync_new_received_anchor_varbinary = CONVERT(varbinary(8), @sync_new_received_anchor);

    IF ((@sync_request_type = 4 AND (@sync_first_time_sync_option_tblEquipment IS NOT NULL AND @sync_first_time_sync_option_tblEquipment = 1))
        OR (@sync_bypass_insert_update_extraction IS NOT NULL AND @sync_bypass_insert_update_extraction = 1))
    BEGIN
        SELECT [dbo].[tblEquipment].[ID],[dbo].[tblEquipment].[Description],[dbo].[tblEquipment].[Make],[dbo].[tblEquipment].[Model],[dbo].[tblEquipment].[Year],[dbo].[tblEquipment].[IssPtNum],[dbo].[tblEquipment].[Fixed],[dbo].[tblEquipment].[StorageType],[dbo].[tblEquipment].[InUse],[dbo].[tblEquipment].[FixedVolume],[dbo].[tblEquipment].[IntoPlane],[dbo].[tblEquipment].[Mobile],[dbo].[tblEquipment].[AttachedTo],[dbo].[tblEquipment].[MediaType],[dbo].[tblEquipment].[Meters],[dbo].[tblEquipment].[DefuelMeterForwards],[dbo].[tblEquipment].[PulseRatio],[dbo].[tblEquipment].[Round],[dbo].[tblEquipment].[Xref],[dbo].[tblEquipment].[LowStockWarning],[dbo].[tblEquipment].[StockTrack],[dbo].[tblEquipment].[Totalisor1],[dbo].[tblEquipment].[Totalisor2],[dbo].[tblEquipment].[FuelingState],[dbo].[tblEquipment].[Volume],[dbo].[tblEquipment].[MeterReading],[dbo].[tblEquipment].[Consecutive_OOS_Variance],[dbo].[tblEquipment].[Notes],[dbo].[tblEquipment].[Capacity],[dbo].[tblEquipment].[SafeFill],[dbo].[tblEquipment].[VolumeUnitIndex],[dbo].[tblEquipment].[TemperatureUnitIndex],[dbo].[tblEquipment].[DensityUnitIndex],[dbo].[tblEquipment].[MassUnitIndex],[dbo].[tblEquipment].[VolumeDecimalPlaces],[dbo].[tblEquipment].[TemperatureDecimalPlaces],[dbo].[tblEquipment].[DensityDecimalPlaces],[dbo].[tblEquipment].[MassDecimalPlaces],[dbo].[tblEquipment].[EquipmentSequence],[dbo].[tblEquipment].[LockedOut],[dbo].[tblEquipment].[LockedOutReason],[dbo].[tblEquipment].[LockedOutDate],[dbo].[tblEquipment].[SerialNumber],[dbo].[tblEquipment].[CompanyEquipmentID],[dbo].[tblEquipment].[TruckCardNumber],[dbo].[tblEquipment].[CreatedDate],[dbo].[tblEquipment].[CreatedBy],[dbo].[tblEquipment].[UpdatedDate],[dbo].[tblEquipment].[UpdatedBy],[dbo].[tblEquipment].[RatedGPM],[dbo].[tblEquipment].[ActualGPM],[dbo].[tblEquipment].[FuelAdditiveFlag],[dbo].[tblEquipment].[ManufactureDate],[dbo].[tblEquipment].[InstallationDate],[dbo].[tblEquipment].[InspectionDate],[dbo].[tblEquipment].[CalibrationDate],[dbo].[tblEquipment].[QCDate],[dbo].[tblEquipment].[SecondaryStorageFlag],[dbo].[tblEquipment].[ManagedEquipmentFlag],[dbo].[tblEquipment].[FuelingType],[dbo].[tblEquipment].[UserData1],[dbo].[tblEquipment].[UserData2],[dbo].[tblEquipment].[UserData3],[dbo].[tblEquipment].[UserData4],[dbo].[tblEquipment].[UserData5],[dbo].[tblEquipment].[UserData6],[dbo].[tblEquipment].[UserData7],[dbo].[tblEquipment].[UserData8],[dbo].[tblEquipment].[UserData9],[dbo].[tblEquipment].[UserData10],[dbo].[tblEquipment].[UserData11],[dbo].[tblEquipment].[UserData12],[dbo].[tblEquipment].[UserData13],[dbo].[tblEquipment].[UserData14],[dbo].[tblEquipment].[UserData15],[dbo].[tblEquipment].[UserData16],[dbo].[tblEquipment].[UserData17],[dbo].[tblEquipment].[UserData18],[dbo].[tblEquipment].[UserData19],[dbo].[tblEquipment].[UserData20],[dbo].[tblEquipment].[UserData21],[dbo].[tblEquipment].[UserData22],[dbo].[tblEquipment].[UserData23],[dbo].[tblEquipment].[UserData24],[dbo].[tblEquipment].[EquipmentGuid],[dbo].[tblEquipment].[SiteGuid],[dbo].[tblEquipment].[CompanyGuid],[dbo].[tblEquipment].[ParentEquipmentGuid],[dbo].[tblEquipment].[EquipmentTypeGuid],[dbo].[tblEquipment].[FuelCardGuid],[dbo].[tblEquipment].[ProductGuid],[dbo].[tblEquipment].[AssignedToMeterGuid],[dbo].[tblEquipment].[AssetTrackingDeviceGuid],[dbo].[tblEquipment].[_MasterRecordGuid],[dbo].[tblEquipment].[HiddenDate],[dbo].[tblEquipment].[ScullyRequired], [dbo].[tblEquipment].[_RowVersion]
            FROM [dbo].[tblEquipment]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblEquipment IS NULL OR 
        (@sync_batch_size_tblEquipment IS NOT NULL AND @sync_batch_size_tblEquipment = 0))
    BEGIN
        SET @sync_batch_size_tblEquipment = 2147483647;
    END


    -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
    -- and/or any new entity site assignments (if assignable).
        SELECT [ID],[Description],[Make],[Model],[Year],[IssPtNum],[Fixed],[StorageType],[InUse],[FixedVolume],[IntoPlane],[Mobile],[AttachedTo],[MediaType],[Meters],[DefuelMeterForwards],[PulseRatio],[Round],[Xref],[LowStockWarning],[StockTrack],[Totalisor1],[Totalisor2],[FuelingState],[Volume],[MeterReading],[Consecutive_OOS_Variance],[Notes],[Capacity],[SafeFill],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[MassUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[MassDecimalPlaces],[EquipmentSequence],[LockedOut],[LockedOutReason],[LockedOutDate],[SerialNumber],[CompanyEquipmentID],[TruckCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[RatedGPM],[ActualGPM],[FuelAdditiveFlag],[ManufactureDate],[InstallationDate],[InspectionDate],[CalibrationDate],[QCDate],[SecondaryStorageFlag],[ManagedEquipmentFlag],[FuelingType],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[EquipmentGuid],[SiteGuid],[CompanyGuid],[ParentEquipmentGuid],[EquipmentTypeGuid],[FuelCardGuid],[ProductGuid],[AssignedToMeterGuid],[AssetTrackingDeviceGuid],[_MasterRecordGuid],[HiddenDate],[ScullyRequired],_RowVersion
        FROM (
            SELECT TOP(@sync_batch_size_tblEquipment) WITH TIES [dbo].[tblEquipment].[ID],[dbo].[tblEquipment].[Description],[dbo].[tblEquipment].[Make],[dbo].[tblEquipment].[Model],[dbo].[tblEquipment].[Year],[dbo].[tblEquipment].[IssPtNum],[dbo].[tblEquipment].[Fixed],[dbo].[tblEquipment].[StorageType],[dbo].[tblEquipment].[InUse],[dbo].[tblEquipment].[FixedVolume],[dbo].[tblEquipment].[IntoPlane],[dbo].[tblEquipment].[Mobile],[dbo].[tblEquipment].[AttachedTo],[dbo].[tblEquipment].[MediaType],[dbo].[tblEquipment].[Meters],[dbo].[tblEquipment].[DefuelMeterForwards],[dbo].[tblEquipment].[PulseRatio],[dbo].[tblEquipment].[Round],[dbo].[tblEquipment].[Xref],[dbo].[tblEquipment].[LowStockWarning],[dbo].[tblEquipment].[StockTrack],[dbo].[tblEquipment].[Totalisor1],[dbo].[tblEquipment].[Totalisor2],[dbo].[tblEquipment].[FuelingState],[dbo].[tblEquipment].[Volume],[dbo].[tblEquipment].[MeterReading],[dbo].[tblEquipment].[Consecutive_OOS_Variance],[dbo].[tblEquipment].[Notes],[dbo].[tblEquipment].[Capacity],[dbo].[tblEquipment].[SafeFill],[dbo].[tblEquipment].[VolumeUnitIndex],[dbo].[tblEquipment].[TemperatureUnitIndex],[dbo].[tblEquipment].[DensityUnitIndex],[dbo].[tblEquipment].[MassUnitIndex],[dbo].[tblEquipment].[VolumeDecimalPlaces],[dbo].[tblEquipment].[TemperatureDecimalPlaces],[dbo].[tblEquipment].[DensityDecimalPlaces],[dbo].[tblEquipment].[MassDecimalPlaces],[dbo].[tblEquipment].[EquipmentSequence],[dbo].[tblEquipment].[LockedOut],[dbo].[tblEquipment].[LockedOutReason],[dbo].[tblEquipment].[LockedOutDate],[dbo].[tblEquipment].[SerialNumber],[dbo].[tblEquipment].[CompanyEquipmentID],[dbo].[tblEquipment].[TruckCardNumber],[dbo].[tblEquipment].[CreatedDate],[dbo].[tblEquipment].[CreatedBy],[dbo].[tblEquipment].[UpdatedDate],[dbo].[tblEquipment].[UpdatedBy],[dbo].[tblEquipment].[RatedGPM],[dbo].[tblEquipment].[ActualGPM],[dbo].[tblEquipment].[FuelAdditiveFlag],[dbo].[tblEquipment].[ManufactureDate],[dbo].[tblEquipment].[InstallationDate],[dbo].[tblEquipment].[InspectionDate],[dbo].[tblEquipment].[CalibrationDate],[dbo].[tblEquipment].[QCDate],[dbo].[tblEquipment].[SecondaryStorageFlag],[dbo].[tblEquipment].[ManagedEquipmentFlag],[dbo].[tblEquipment].[FuelingType],[dbo].[tblEquipment].[UserData1],[dbo].[tblEquipment].[UserData2],[dbo].[tblEquipment].[UserData3],[dbo].[tblEquipment].[UserData4],[dbo].[tblEquipment].[UserData5],[dbo].[tblEquipment].[UserData6],[dbo].[tblEquipment].[UserData7],[dbo].[tblEquipment].[UserData8],[dbo].[tblEquipment].[UserData9],[dbo].[tblEquipment].[UserData10],[dbo].[tblEquipment].[UserData11],[dbo].[tblEquipment].[UserData12],[dbo].[tblEquipment].[UserData13],[dbo].[tblEquipment].[UserData14],[dbo].[tblEquipment].[UserData15],[dbo].[tblEquipment].[UserData16],[dbo].[tblEquipment].[UserData17],[dbo].[tblEquipment].[UserData18],[dbo].[tblEquipment].[UserData19],[dbo].[tblEquipment].[UserData20],[dbo].[tblEquipment].[UserData21],[dbo].[tblEquipment].[UserData22],[dbo].[tblEquipment].[UserData23],[dbo].[tblEquipment].[UserData24],[dbo].[tblEquipment].[EquipmentGuid],[dbo].[tblEquipment].[SiteGuid],[dbo].[tblEquipment].[CompanyGuid],[dbo].[tblEquipment].[ParentEquipmentGuid],[dbo].[tblEquipment].[EquipmentTypeGuid],[dbo].[tblEquipment].[FuelCardGuid],[dbo].[tblEquipment].[ProductGuid],[dbo].[tblEquipment].[AssignedToMeterGuid],[dbo].[tblEquipment].[AssetTrackingDeviceGuid],[dbo].[tblEquipment].[_MasterRecordGuid],[dbo].[tblEquipment].[HiddenDate],[dbo].[tblEquipment].[ScullyRequired],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                FROM [dbo].[tblEquipment]
                    INNER JOIN (SELECT [EquipmentToSiteGuid],[EquipmentGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedEquipmentListForSite](@sync_context_site_guid)) data
                        ON [dbo].[tblEquipment].[EquipmentGuid] = data.[EquipmentGuid]
                    INNER JOIN [track].[tblEquipment] CT
                        ON CT.PK_EquipmentGuid = [dbo].[tblEquipment].[EquipmentGuid] 
                    INNER JOIN [track].[tblEntityEquipmentToSite] MAPCT
                        ON MAPCT.PK_EquipmentToSiteGuid = data.[EquipmentToSiteGuid] 
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
