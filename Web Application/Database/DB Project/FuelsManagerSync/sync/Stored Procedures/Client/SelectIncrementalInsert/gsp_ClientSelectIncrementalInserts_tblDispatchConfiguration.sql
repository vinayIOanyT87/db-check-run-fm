-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDispatchConfiguration
-- Description: Get New Records
-- Enumerations:
--      @sync_request_type  = MANUAL (0), PERIODIC (1), SCHEDULED (2), RESYNC (3), INIT (4)
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectIncrementalInserts_tblDispatchConfiguration]
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
@sync_batch_size_tblDispatchConfiguration int,
@sync_bypass_insert_update_extraction bit,
@sync_request_type int,
@sync_first_time_sync_option_tblDispatchConfiguration int
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
        SELECT [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid],[dbo].[tblDispatchConfiguration].[SiteGuid],[dbo].[tblDispatchConfiguration].[ID],[dbo].[tblDispatchConfiguration].[DisplayCurrentTime],[dbo].[tblDispatchConfiguration].[DispatchDataRefreshPeriod],[dbo].[tblDispatchConfiguration].[TabularViewDisplayMilitaryDate],[dbo].[tblDispatchConfiguration].[QuantityNotZeroCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneManagerCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneOwnerCheck],[dbo].[tblDispatchConfiguration].[DispatchFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FastLogFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FillstandVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[ReturnToBulkVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[RecirculationVolumesGreaterThanZeroCheck],[dbo].[tblDispatchConfiguration].[OperatorIsInCheck],[dbo].[tblDispatchConfiguration].[OperatorNotAssignedCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredTrainingCheck],[dbo].[tblDispatchConfiguration].[OperatorTrainingNotExpiredCheck],[dbo].[tblDispatchConfiguration].[OperatorNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredQualificationsCheck],[dbo].[tblDispatchConfiguration].[OperatorQualificationsNotExpiredCheck],[dbo].[tblDispatchConfiguration].[DefuelStatusCheck],[dbo].[tblDispatchConfiguration].[RefuelStatusCheck],[dbo].[tblDispatchConfiguration].[EquipmentFuelGradeCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotAssignedCheck],[dbo].[tblDispatchConfiguration].[EquipmentInServiceCheck],[dbo].[tblDispatchConfiguration].[TagLicenseNotExpiredCheck],[dbo].[tblDispatchConfiguration].[TestInspectionNotExpiredCheck],[dbo].[tblDispatchConfiguration].[QualityControlCheckupDateCheck],[dbo].[tblDispatchConfiguration].[CautionQualityTagCheck],[dbo].[tblDispatchConfiguration].[WarningQualityTagCheck],[dbo].[tblDispatchConfiguration].[DangerQualityTagCheck],[dbo].[tblDispatchConfiguration].[CreatedDate],[dbo].[tblDispatchConfiguration].[CreatedBy],[dbo].[tblDispatchConfiguration].[UpdatedDate],[dbo].[tblDispatchConfiguration].[UpdatedBy],[dbo].[tblDispatchConfiguration].[EnableServiceRequests],[dbo].[tblDispatchConfiguration].[AutomaticRestartDelay],[dbo].[tblDispatchConfiguration].[EquipmentRequired],[dbo].[tblDispatchConfiguration].[PersonnelRequired],[dbo].[tblDispatchConfiguration].[FillToActualOrStandard],[dbo].[tblDispatchConfiguration].[OperationalWindowPastHours],[dbo].[tblDispatchConfiguration].[OperationalWindowFutureHours],[dbo].[tblDispatchConfiguration].[ShowGridLines],[dbo].[tblDispatchConfiguration].[StaticTimeDisplay],[dbo].[tblDispatchConfiguration].[UseArrivalTime],[dbo].[tblDispatchConfiguration].[UseStartTime],[dbo].[tblDispatchConfiguration].[UseStopTime],[dbo].[tblDispatchConfiguration].[FuelsManagerReportURL], [dbo].[tblDispatchConfiguration].[_RowVersion]
            FROM [dbo].[tblDispatchConfiguration]
            WHERE 1=2;
            
        RETURN;
    END

    IF (@sync_batch_size_tblDispatchConfiguration IS NULL OR 
        (@sync_batch_size_tblDispatchConfiguration IS NOT NULL AND @sync_batch_size_tblDispatchConfiguration = 0))
    BEGIN
        SET @sync_batch_size_tblDispatchConfiguration = 2147483647;
    END


        -- Get a list of the Owned/Assigned Entities and locate any newly inserted entities
        -- and/or any new entity site assignments (if assignable).
            SELECT TOP(@sync_batch_size_tblDispatchConfiguration) WITH TIES [DispatchConfigurationGuid],[SiteGuid],[ID],[DisplayCurrentTime],[DispatchDataRefreshPeriod],[TabularViewDisplayMilitaryDate],[QuantityNotZeroCheck],[ExactlyOneManagerCheck],[ExactlyOneOwnerCheck],[DispatchFuelAdditiveFlagCheck],[FastLogFuelAdditiveFlagCheck],[FillstandVolumeWithinToleranceCheck],[ReturnToBulkVolumeWithinToleranceCheck],[RecirculationVolumesGreaterThanZeroCheck],[OperatorIsInCheck],[OperatorNotAssignedCheck],[OperatorHasRequiredTrainingCheck],[OperatorTrainingNotExpiredCheck],[OperatorNotLockedOutCheck],[OperatorHasRequiredQualificationsCheck],[OperatorQualificationsNotExpiredCheck],[DefuelStatusCheck],[RefuelStatusCheck],[EquipmentFuelGradeCheck],[EquipmentNotLockedOutCheck],[EquipmentNotAssignedCheck],[EquipmentInServiceCheck],[TagLicenseNotExpiredCheck],[TestInspectionNotExpiredCheck],[QualityControlCheckupDateCheck],[CautionQualityTagCheck],[WarningQualityTagCheck],[DangerQualityTagCheck],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableServiceRequests],[AutomaticRestartDelay],[EquipmentRequired],[PersonnelRequired],[FillToActualOrStandard],[OperationalWindowPastHours],[OperationalWindowFutureHours],[ShowGridLines],[StaticTimeDisplay],[UseArrivalTime],[UseStartTime],[UseStopTime],[FuelsManagerReportURL],_RowVersion
            FROM (
                SELECT TOP(@sync_batch_size_tblDispatchConfiguration) WITH TIES [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid],[dbo].[tblDispatchConfiguration].[SiteGuid],[dbo].[tblDispatchConfiguration].[ID],[dbo].[tblDispatchConfiguration].[DisplayCurrentTime],[dbo].[tblDispatchConfiguration].[DispatchDataRefreshPeriod],[dbo].[tblDispatchConfiguration].[TabularViewDisplayMilitaryDate],[dbo].[tblDispatchConfiguration].[QuantityNotZeroCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneManagerCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneOwnerCheck],[dbo].[tblDispatchConfiguration].[DispatchFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FastLogFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FillstandVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[ReturnToBulkVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[RecirculationVolumesGreaterThanZeroCheck],[dbo].[tblDispatchConfiguration].[OperatorIsInCheck],[dbo].[tblDispatchConfiguration].[OperatorNotAssignedCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredTrainingCheck],[dbo].[tblDispatchConfiguration].[OperatorTrainingNotExpiredCheck],[dbo].[tblDispatchConfiguration].[OperatorNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredQualificationsCheck],[dbo].[tblDispatchConfiguration].[OperatorQualificationsNotExpiredCheck],[dbo].[tblDispatchConfiguration].[DefuelStatusCheck],[dbo].[tblDispatchConfiguration].[RefuelStatusCheck],[dbo].[tblDispatchConfiguration].[EquipmentFuelGradeCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotAssignedCheck],[dbo].[tblDispatchConfiguration].[EquipmentInServiceCheck],[dbo].[tblDispatchConfiguration].[TagLicenseNotExpiredCheck],[dbo].[tblDispatchConfiguration].[TestInspectionNotExpiredCheck],[dbo].[tblDispatchConfiguration].[QualityControlCheckupDateCheck],[dbo].[tblDispatchConfiguration].[CautionQualityTagCheck],[dbo].[tblDispatchConfiguration].[WarningQualityTagCheck],[dbo].[tblDispatchConfiguration].[DangerQualityTagCheck],[dbo].[tblDispatchConfiguration].[CreatedDate],[dbo].[tblDispatchConfiguration].[CreatedBy],[dbo].[tblDispatchConfiguration].[UpdatedDate],[dbo].[tblDispatchConfiguration].[UpdatedBy],[dbo].[tblDispatchConfiguration].[EnableServiceRequests],[dbo].[tblDispatchConfiguration].[AutomaticRestartDelay],[dbo].[tblDispatchConfiguration].[EquipmentRequired],[dbo].[tblDispatchConfiguration].[PersonnelRequired],[dbo].[tblDispatchConfiguration].[FillToActualOrStandard],[dbo].[tblDispatchConfiguration].[OperationalWindowPastHours],[dbo].[tblDispatchConfiguration].[OperationalWindowFutureHours],[dbo].[tblDispatchConfiguration].[ShowGridLines],[dbo].[tblDispatchConfiguration].[StaticTimeDisplay],[dbo].[tblDispatchConfiguration].[UseArrivalTime],[dbo].[tblDispatchConfiguration].[UseStartTime],[dbo].[tblDispatchConfiguration].[UseStopTime],[dbo].[tblDispatchConfiguration].[FuelsManagerReportURL],sync.udf_GetMaxRowVersion(CT.InsertedRowVersion,MAPCT.InsertedRowVersion,NULL) AS '_RowVersion'
                    FROM [dbo].[tblDispatchConfiguration]
                        INNER JOIN (SELECT [DispatchConfigurationToSiteGuid],[DispatchConfigurationGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedDispatchConfigurationListForSite](@sync_context_site_guid)) data
                            ON [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid] = data.[DispatchConfigurationGuid]
                        INNER JOIN [track].[tblDispatchConfiguration] CT
                            ON CT.PK_DispatchConfigurationGuid = [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid] 
                        INNER JOIN [track].[tblEntityDispatchConfigurationToSite] MAPCT
                            ON MAPCT.PK_DispatchConfigurationToSiteGuid = data.[DispatchConfigurationToSiteGuid] 
                WHERE ( [dbo].[tblDispatchConfiguration].[SiteGuid] = @sync_context_site_guid )
                        AND (((CT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (CT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (CT.InsertedContext IS NULL OR CT.InsertedContext <> @sync_server_id_binary))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                            OR ((MAPCT.InsertedRowVersion > @sync_last_received_anchor_varbinary)
                            AND (MAPCT.InsertedRowVersion <= @sync_new_received_anchor_varbinary)
                            AND (MAPCT.InsertedContext IS NULL OR MAPCT.InsertedContext <> @sync_server_id_binary)))   -- USE THE SERVER ID HERE SO WE DO NOT RETURN WHAT WAS JUST RECEIVED
                ORDER BY _RowVersion ASC
            ) rs1  -- DetectedSubFunctions OR IncludeEntityAssignments: False (and not tblPersonnel, tblEquipment or tblProducts)
        ORDER BY _RowVersion ASC

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SII)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor) 
END
