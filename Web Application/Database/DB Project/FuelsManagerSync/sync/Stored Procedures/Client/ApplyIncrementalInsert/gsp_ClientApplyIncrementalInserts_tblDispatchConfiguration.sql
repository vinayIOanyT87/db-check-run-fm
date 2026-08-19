-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDispatchConfiguration
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblDispatchConfiguration]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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

    ;   MERGE [dbo].[tblDispatchConfiguration] AS existingData
        USING (SELECT @DispatchConfigurationGuid 'DispatchConfigurationGuid',@SiteGuid 'SiteGuid',@ID 'ID',@DisplayCurrentTime 'DisplayCurrentTime',@DispatchDataRefreshPeriod 'DispatchDataRefreshPeriod',@TabularViewDisplayMilitaryDate 'TabularViewDisplayMilitaryDate',@QuantityNotZeroCheck 'QuantityNotZeroCheck',@ExactlyOneManagerCheck 'ExactlyOneManagerCheck',@ExactlyOneOwnerCheck 'ExactlyOneOwnerCheck',@DispatchFuelAdditiveFlagCheck 'DispatchFuelAdditiveFlagCheck',@FastLogFuelAdditiveFlagCheck 'FastLogFuelAdditiveFlagCheck',@FillstandVolumeWithinToleranceCheck 'FillstandVolumeWithinToleranceCheck',@ReturnToBulkVolumeWithinToleranceCheck 'ReturnToBulkVolumeWithinToleranceCheck',@RecirculationVolumesGreaterThanZeroCheck 'RecirculationVolumesGreaterThanZeroCheck',@OperatorIsInCheck 'OperatorIsInCheck',@OperatorNotAssignedCheck 'OperatorNotAssignedCheck',@OperatorHasRequiredTrainingCheck 'OperatorHasRequiredTrainingCheck',@OperatorTrainingNotExpiredCheck 'OperatorTrainingNotExpiredCheck',@OperatorNotLockedOutCheck 'OperatorNotLockedOutCheck',@OperatorHasRequiredQualificationsCheck 'OperatorHasRequiredQualificationsCheck',@OperatorQualificationsNotExpiredCheck 'OperatorQualificationsNotExpiredCheck',@DefuelStatusCheck 'DefuelStatusCheck',@RefuelStatusCheck 'RefuelStatusCheck',@EquipmentFuelGradeCheck 'EquipmentFuelGradeCheck',@EquipmentNotLockedOutCheck 'EquipmentNotLockedOutCheck',@EquipmentNotAssignedCheck 'EquipmentNotAssignedCheck',@EquipmentInServiceCheck 'EquipmentInServiceCheck',@TagLicenseNotExpiredCheck 'TagLicenseNotExpiredCheck',@TestInspectionNotExpiredCheck 'TestInspectionNotExpiredCheck',@QualityControlCheckupDateCheck 'QualityControlCheckupDateCheck',@CautionQualityTagCheck 'CautionQualityTagCheck',@WarningQualityTagCheck 'WarningQualityTagCheck',@DangerQualityTagCheck 'DangerQualityTagCheck',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@EnableServiceRequests 'EnableServiceRequests',@AutomaticRestartDelay 'AutomaticRestartDelay',@EquipmentRequired 'EquipmentRequired',@PersonnelRequired 'PersonnelRequired',@FillToActualOrStandard 'FillToActualOrStandard',@OperationalWindowPastHours 'OperationalWindowPastHours',@OperationalWindowFutureHours 'OperationalWindowFutureHours',@ShowGridLines 'ShowGridLines',@StaticTimeDisplay 'StaticTimeDisplay',@UseArrivalTime 'UseArrivalTime',@UseStartTime 'UseStartTime',@UseStopTime 'UseStopTime',@FuelsManagerReportURL 'FuelsManagerReportURL'
                ) AS remoteChanges ([DispatchConfigurationGuid],[SiteGuid],[ID],[DisplayCurrentTime],[DispatchDataRefreshPeriod],[TabularViewDisplayMilitaryDate],[QuantityNotZeroCheck],[ExactlyOneManagerCheck],[ExactlyOneOwnerCheck],[DispatchFuelAdditiveFlagCheck],[FastLogFuelAdditiveFlagCheck],[FillstandVolumeWithinToleranceCheck],[ReturnToBulkVolumeWithinToleranceCheck],[RecirculationVolumesGreaterThanZeroCheck],[OperatorIsInCheck],[OperatorNotAssignedCheck],[OperatorHasRequiredTrainingCheck],[OperatorTrainingNotExpiredCheck],[OperatorNotLockedOutCheck],[OperatorHasRequiredQualificationsCheck],[OperatorQualificationsNotExpiredCheck],[DefuelStatusCheck],[RefuelStatusCheck],[EquipmentFuelGradeCheck],[EquipmentNotLockedOutCheck],[EquipmentNotAssignedCheck],[EquipmentInServiceCheck],[TagLicenseNotExpiredCheck],[TestInspectionNotExpiredCheck],[QualityControlCheckupDateCheck],[CautionQualityTagCheck],[WarningQualityTagCheck],[DangerQualityTagCheck],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableServiceRequests],[AutomaticRestartDelay],[EquipmentRequired],[PersonnelRequired],[FillToActualOrStandard],[OperationalWindowPastHours],[OperationalWindowFutureHours],[ShowGridLines],[StaticTimeDisplay],[UseArrivalTime],[UseStartTime],[UseStopTime],[FuelsManagerReportURL])
        ON (existingData.[DispatchConfigurationGuid] = remoteChanges.[DispatchConfigurationGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
