-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDispatchConfiguration
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblDispatchConfiguration]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@DispatchConfigurationGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@ID nvarchar(50),
@DisplayCurrentTime bit,
@DispatchDataRefreshPeriod int,
@TabularViewDisplayMilitaryDate bit,
@QuantityNotZeroCheck bit,
@ExactlyOneManagerCheck bit,
@ExactlyOneOwnerCheck bit,
@DispatchFuelAdditiveFlagCheck bit,
@FastLogFuelAdditiveFlagCheck bit,
@FillstandVolumeWithinToleranceCheck bit,
@ReturnToBulkVolumeWithinToleranceCheck bit,
@RecirculationVolumesGreaterThanZeroCheck bit,
@OperatorIsInCheck bit,
@OperatorNotAssignedCheck bit,
@OperatorHasRequiredTrainingCheck bit,
@OperatorTrainingNotExpiredCheck bit,
@OperatorNotLockedOutCheck bit,
@OperatorHasRequiredQualificationsCheck bit,
@OperatorQualificationsNotExpiredCheck bit,
@DefuelStatusCheck bit,
@RefuelStatusCheck bit,
@EquipmentFuelGradeCheck bit,
@EquipmentNotLockedOutCheck bit,
@EquipmentNotAssignedCheck bit,
@EquipmentInServiceCheck bit,
@TagLicenseNotExpiredCheck bit,
@TestInspectionNotExpiredCheck bit,
@QualityControlCheckupDateCheck bit,
@CautionQualityTagCheck bit,
@WarningQualityTagCheck bit,
@DangerQualityTagCheck bit,
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@EnableServiceRequests bit,
@AutomaticRestartDelay int,
@EquipmentRequired bit,
@PersonnelRequired bit,
@FillToActualOrStandard int,
@OperationalWindowPastHours int,
@OperationalWindowFutureHours int,
@ShowGridLines bit,
@StaticTimeDisplay bit,
@UseArrivalTime bit,
@UseStartTime bit,
@UseStopTime bit,
@FuelsManagerReportURL nvarchar(max),
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblDispatchConfiguration] CT
                        WHERE CT.PK_DispatchConfigurationGuid = @DispatchConfigurationGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid],[dbo].[tblDispatchConfiguration].[SiteGuid],[dbo].[tblDispatchConfiguration].[ID],[dbo].[tblDispatchConfiguration].[DisplayCurrentTime],[dbo].[tblDispatchConfiguration].[DispatchDataRefreshPeriod],[dbo].[tblDispatchConfiguration].[TabularViewDisplayMilitaryDate],[dbo].[tblDispatchConfiguration].[QuantityNotZeroCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneManagerCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneOwnerCheck],[dbo].[tblDispatchConfiguration].[DispatchFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FastLogFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FillstandVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[ReturnToBulkVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[RecirculationVolumesGreaterThanZeroCheck],[dbo].[tblDispatchConfiguration].[OperatorIsInCheck],[dbo].[tblDispatchConfiguration].[OperatorNotAssignedCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredTrainingCheck],[dbo].[tblDispatchConfiguration].[OperatorTrainingNotExpiredCheck],[dbo].[tblDispatchConfiguration].[OperatorNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredQualificationsCheck],[dbo].[tblDispatchConfiguration].[OperatorQualificationsNotExpiredCheck],[dbo].[tblDispatchConfiguration].[DefuelStatusCheck],[dbo].[tblDispatchConfiguration].[RefuelStatusCheck],[dbo].[tblDispatchConfiguration].[EquipmentFuelGradeCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotAssignedCheck],[dbo].[tblDispatchConfiguration].[EquipmentInServiceCheck],[dbo].[tblDispatchConfiguration].[TagLicenseNotExpiredCheck],[dbo].[tblDispatchConfiguration].[TestInspectionNotExpiredCheck],[dbo].[tblDispatchConfiguration].[QualityControlCheckupDateCheck],[dbo].[tblDispatchConfiguration].[CautionQualityTagCheck],[dbo].[tblDispatchConfiguration].[WarningQualityTagCheck],[dbo].[tblDispatchConfiguration].[DangerQualityTagCheck],[dbo].[tblDispatchConfiguration].[CreatedDate],[dbo].[tblDispatchConfiguration].[CreatedBy],[dbo].[tblDispatchConfiguration].[UpdatedDate],[dbo].[tblDispatchConfiguration].[UpdatedBy],[dbo].[tblDispatchConfiguration].[EnableServiceRequests],[dbo].[tblDispatchConfiguration].[AutomaticRestartDelay],[dbo].[tblDispatchConfiguration].[EquipmentRequired],[dbo].[tblDispatchConfiguration].[PersonnelRequired],[dbo].[tblDispatchConfiguration].[FillToActualOrStandard],[dbo].[tblDispatchConfiguration].[OperationalWindowPastHours],[dbo].[tblDispatchConfiguration].[OperationalWindowFutureHours],[dbo].[tblDispatchConfiguration].[ShowGridLines],[dbo].[tblDispatchConfiguration].[StaticTimeDisplay],[dbo].[tblDispatchConfiguration].[UseArrivalTime],[dbo].[tblDispatchConfiguration].[UseStartTime],[dbo].[tblDispatchConfiguration].[UseStopTime],[dbo].[tblDispatchConfiguration].[FuelsManagerReportURL]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblDispatchConfiguration]
                        INNER JOIN [track].[tblDispatchConfiguration] CT
                            ON CT.PK_DispatchConfigurationGuid = [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid] 
                    WHERE CT.PK_DispatchConfigurationGuid = @DispatchConfigurationGuid
            ) MERGE existingData
            USING (SELECT @DispatchConfigurationGuid,@SiteGuid,@ID,@DisplayCurrentTime,@DispatchDataRefreshPeriod,@TabularViewDisplayMilitaryDate,@QuantityNotZeroCheck,@ExactlyOneManagerCheck,@ExactlyOneOwnerCheck,@DispatchFuelAdditiveFlagCheck,@FastLogFuelAdditiveFlagCheck,@FillstandVolumeWithinToleranceCheck,@ReturnToBulkVolumeWithinToleranceCheck,@RecirculationVolumesGreaterThanZeroCheck,@OperatorIsInCheck,@OperatorNotAssignedCheck,@OperatorHasRequiredTrainingCheck,@OperatorTrainingNotExpiredCheck,@OperatorNotLockedOutCheck,@OperatorHasRequiredQualificationsCheck,@OperatorQualificationsNotExpiredCheck,@DefuelStatusCheck,@RefuelStatusCheck,@EquipmentFuelGradeCheck,@EquipmentNotLockedOutCheck,@EquipmentNotAssignedCheck,@EquipmentInServiceCheck,@TagLicenseNotExpiredCheck,@TestInspectionNotExpiredCheck,@QualityControlCheckupDateCheck,@CautionQualityTagCheck,@WarningQualityTagCheck,@DangerQualityTagCheck,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@EnableServiceRequests,@AutomaticRestartDelay,@EquipmentRequired,@PersonnelRequired,@FillToActualOrStandard,@OperationalWindowPastHours,@OperationalWindowFutureHours,@ShowGridLines,@StaticTimeDisplay,@UseArrivalTime,@UseStartTime,@UseStopTime,@FuelsManagerReportURL
                    ) AS remoteChanges ([DispatchConfigurationGuid],[SiteGuid],[ID],[DisplayCurrentTime],[DispatchDataRefreshPeriod],[TabularViewDisplayMilitaryDate],[QuantityNotZeroCheck],[ExactlyOneManagerCheck],[ExactlyOneOwnerCheck],[DispatchFuelAdditiveFlagCheck],[FastLogFuelAdditiveFlagCheck],[FillstandVolumeWithinToleranceCheck],[ReturnToBulkVolumeWithinToleranceCheck],[RecirculationVolumesGreaterThanZeroCheck],[OperatorIsInCheck],[OperatorNotAssignedCheck],[OperatorHasRequiredTrainingCheck],[OperatorTrainingNotExpiredCheck],[OperatorNotLockedOutCheck],[OperatorHasRequiredQualificationsCheck],[OperatorQualificationsNotExpiredCheck],[DefuelStatusCheck],[RefuelStatusCheck],[EquipmentFuelGradeCheck],[EquipmentNotLockedOutCheck],[EquipmentNotAssignedCheck],[EquipmentInServiceCheck],[TagLicenseNotExpiredCheck],[TestInspectionNotExpiredCheck],[QualityControlCheckupDateCheck],[CautionQualityTagCheck],[WarningQualityTagCheck],[DangerQualityTagCheck],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableServiceRequests],[AutomaticRestartDelay],[EquipmentRequired],[PersonnelRequired],[FillToActualOrStandard],[OperationalWindowPastHours],[OperationalWindowFutureHours],[ShowGridLines],[StaticTimeDisplay],[UseArrivalTime],[UseStartTime],[UseStopTime],[FuelsManagerReportURL])
            ON (existingData.[DispatchConfigurationGuid] = remoteChanges.[DispatchConfigurationGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [SiteGuid] = remoteChanges.[SiteGuid]
                       ,[ID] = remoteChanges.[ID]
                       ,[DisplayCurrentTime] = remoteChanges.[DisplayCurrentTime]
                       ,[DispatchDataRefreshPeriod] = remoteChanges.[DispatchDataRefreshPeriod]
                       ,[TabularViewDisplayMilitaryDate] = remoteChanges.[TabularViewDisplayMilitaryDate]
                       ,[QuantityNotZeroCheck] = remoteChanges.[QuantityNotZeroCheck]
                       ,[ExactlyOneManagerCheck] = remoteChanges.[ExactlyOneManagerCheck]
                       ,[ExactlyOneOwnerCheck] = remoteChanges.[ExactlyOneOwnerCheck]
                       ,[DispatchFuelAdditiveFlagCheck] = remoteChanges.[DispatchFuelAdditiveFlagCheck]
                       ,[FastLogFuelAdditiveFlagCheck] = remoteChanges.[FastLogFuelAdditiveFlagCheck]
                       ,[FillstandVolumeWithinToleranceCheck] = remoteChanges.[FillstandVolumeWithinToleranceCheck]
                       ,[ReturnToBulkVolumeWithinToleranceCheck] = remoteChanges.[ReturnToBulkVolumeWithinToleranceCheck]
                       ,[RecirculationVolumesGreaterThanZeroCheck] = remoteChanges.[RecirculationVolumesGreaterThanZeroCheck]
                       ,[OperatorIsInCheck] = remoteChanges.[OperatorIsInCheck]
                       ,[OperatorNotAssignedCheck] = remoteChanges.[OperatorNotAssignedCheck]
                       ,[OperatorHasRequiredTrainingCheck] = remoteChanges.[OperatorHasRequiredTrainingCheck]
                       ,[OperatorTrainingNotExpiredCheck] = remoteChanges.[OperatorTrainingNotExpiredCheck]
                       ,[OperatorNotLockedOutCheck] = remoteChanges.[OperatorNotLockedOutCheck]
                       ,[OperatorHasRequiredQualificationsCheck] = remoteChanges.[OperatorHasRequiredQualificationsCheck]
                       ,[OperatorQualificationsNotExpiredCheck] = remoteChanges.[OperatorQualificationsNotExpiredCheck]
                       ,[DefuelStatusCheck] = remoteChanges.[DefuelStatusCheck]
                       ,[RefuelStatusCheck] = remoteChanges.[RefuelStatusCheck]
                       ,[EquipmentFuelGradeCheck] = remoteChanges.[EquipmentFuelGradeCheck]
                       ,[EquipmentNotLockedOutCheck] = remoteChanges.[EquipmentNotLockedOutCheck]
                       ,[EquipmentNotAssignedCheck] = remoteChanges.[EquipmentNotAssignedCheck]
                       ,[EquipmentInServiceCheck] = remoteChanges.[EquipmentInServiceCheck]
                       ,[TagLicenseNotExpiredCheck] = remoteChanges.[TagLicenseNotExpiredCheck]
                       ,[TestInspectionNotExpiredCheck] = remoteChanges.[TestInspectionNotExpiredCheck]
                       ,[QualityControlCheckupDateCheck] = remoteChanges.[QualityControlCheckupDateCheck]
                       ,[CautionQualityTagCheck] = remoteChanges.[CautionQualityTagCheck]
                       ,[WarningQualityTagCheck] = remoteChanges.[WarningQualityTagCheck]
                       ,[DangerQualityTagCheck] = remoteChanges.[DangerQualityTagCheck]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[EnableServiceRequests] = remoteChanges.[EnableServiceRequests]
                       ,[AutomaticRestartDelay] = remoteChanges.[AutomaticRestartDelay]
                       ,[EquipmentRequired] = remoteChanges.[EquipmentRequired]
                       ,[PersonnelRequired] = remoteChanges.[PersonnelRequired]
                       ,[FillToActualOrStandard] = remoteChanges.[FillToActualOrStandard]
                       ,[OperationalWindowPastHours] = remoteChanges.[OperationalWindowPastHours]
                       ,[OperationalWindowFutureHours] = remoteChanges.[OperationalWindowFutureHours]
                       ,[ShowGridLines] = remoteChanges.[ShowGridLines]
                       ,[StaticTimeDisplay] = remoteChanges.[StaticTimeDisplay]
                       ,[UseArrivalTime] = remoteChanges.[UseArrivalTime]
                       ,[UseStartTime] = remoteChanges.[UseStartTime]
                       ,[UseStopTime] = remoteChanges.[UseStopTime]
                       ,[FuelsManagerReportURL] = remoteChanges.[FuelsManagerReportURL]

            WHEN NOT MATCHED THEN
                INSERT ([DispatchConfigurationGuid],[SiteGuid],[ID],[DisplayCurrentTime],[DispatchDataRefreshPeriod],[TabularViewDisplayMilitaryDate],[QuantityNotZeroCheck],[ExactlyOneManagerCheck],[ExactlyOneOwnerCheck],[DispatchFuelAdditiveFlagCheck],[FastLogFuelAdditiveFlagCheck],[FillstandVolumeWithinToleranceCheck],[ReturnToBulkVolumeWithinToleranceCheck],[RecirculationVolumesGreaterThanZeroCheck],[OperatorIsInCheck],[OperatorNotAssignedCheck],[OperatorHasRequiredTrainingCheck],[OperatorTrainingNotExpiredCheck],[OperatorNotLockedOutCheck],[OperatorHasRequiredQualificationsCheck],[OperatorQualificationsNotExpiredCheck],[DefuelStatusCheck],[RefuelStatusCheck],[EquipmentFuelGradeCheck],[EquipmentNotLockedOutCheck],[EquipmentNotAssignedCheck],[EquipmentInServiceCheck],[TagLicenseNotExpiredCheck],[TestInspectionNotExpiredCheck],[QualityControlCheckupDateCheck],[CautionQualityTagCheck],[WarningQualityTagCheck],[DangerQualityTagCheck],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableServiceRequests],[AutomaticRestartDelay],[EquipmentRequired],[PersonnelRequired],[FillToActualOrStandard],[OperationalWindowPastHours],[OperationalWindowFutureHours],[ShowGridLines],[StaticTimeDisplay],[UseArrivalTime],[UseStartTime],[UseStopTime],[FuelsManagerReportURL])
                    VALUES (@DispatchConfigurationGuid,@SiteGuid,@ID,@DisplayCurrentTime,@DispatchDataRefreshPeriod,@TabularViewDisplayMilitaryDate,@QuantityNotZeroCheck,@ExactlyOneManagerCheck,@ExactlyOneOwnerCheck,@DispatchFuelAdditiveFlagCheck,@FastLogFuelAdditiveFlagCheck,@FillstandVolumeWithinToleranceCheck,@ReturnToBulkVolumeWithinToleranceCheck,@RecirculationVolumesGreaterThanZeroCheck,@OperatorIsInCheck,@OperatorNotAssignedCheck,@OperatorHasRequiredTrainingCheck,@OperatorTrainingNotExpiredCheck,@OperatorNotLockedOutCheck,@OperatorHasRequiredQualificationsCheck,@OperatorQualificationsNotExpiredCheck,@DefuelStatusCheck,@RefuelStatusCheck,@EquipmentFuelGradeCheck,@EquipmentNotLockedOutCheck,@EquipmentNotAssignedCheck,@EquipmentInServiceCheck,@TagLicenseNotExpiredCheck,@TestInspectionNotExpiredCheck,@QualityControlCheckupDateCheck,@CautionQualityTagCheck,@WarningQualityTagCheck,@DangerQualityTagCheck,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@EnableServiceRequests,@AutomaticRestartDelay,@EquipmentRequired,@PersonnelRequired,@FillToActualOrStandard,@OperationalWindowPastHours,@OperationalWindowFutureHours,@ShowGridLines,@StaticTimeDisplay,@UseArrivalTime,@UseStartTime,@UseStopTime,@FuelsManagerReportURL)
            ;
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @DispatchConfigurationGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @DispatchConfigurationGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @DispatchConfigurationGuid)
        END
        SET NOCOUNT OFF
    END
    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblDispatchConfiguration] WHERE DispatchConfigurationGuid = @DispatchConfigurationGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
