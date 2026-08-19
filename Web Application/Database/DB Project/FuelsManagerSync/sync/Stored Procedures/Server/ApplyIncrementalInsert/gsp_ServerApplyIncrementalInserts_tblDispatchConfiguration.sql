-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDispatchConfiguration
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblDispatchConfiguration]
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblDispatchConfiguration varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblDispatchConfiguration] AS existingData
        USING (SELECT @DispatchConfigurationGuid 'DispatchConfigurationGuid',@SiteGuid 'SiteGuid',@ID 'ID',@DisplayCurrentTime 'DisplayCurrentTime',@DispatchDataRefreshPeriod 'DispatchDataRefreshPeriod',@TabularViewDisplayMilitaryDate 'TabularViewDisplayMilitaryDate',@QuantityNotZeroCheck 'QuantityNotZeroCheck',@ExactlyOneManagerCheck 'ExactlyOneManagerCheck',@ExactlyOneOwnerCheck 'ExactlyOneOwnerCheck',@DispatchFuelAdditiveFlagCheck 'DispatchFuelAdditiveFlagCheck',@FastLogFuelAdditiveFlagCheck 'FastLogFuelAdditiveFlagCheck',@FillstandVolumeWithinToleranceCheck 'FillstandVolumeWithinToleranceCheck',@ReturnToBulkVolumeWithinToleranceCheck 'ReturnToBulkVolumeWithinToleranceCheck',@RecirculationVolumesGreaterThanZeroCheck 'RecirculationVolumesGreaterThanZeroCheck',@OperatorIsInCheck 'OperatorIsInCheck',@OperatorNotAssignedCheck 'OperatorNotAssignedCheck',@OperatorHasRequiredTrainingCheck 'OperatorHasRequiredTrainingCheck',@OperatorTrainingNotExpiredCheck 'OperatorTrainingNotExpiredCheck',@OperatorNotLockedOutCheck 'OperatorNotLockedOutCheck',@OperatorHasRequiredQualificationsCheck 'OperatorHasRequiredQualificationsCheck',@OperatorQualificationsNotExpiredCheck 'OperatorQualificationsNotExpiredCheck',@DefuelStatusCheck 'DefuelStatusCheck',@RefuelStatusCheck 'RefuelStatusCheck',@EquipmentFuelGradeCheck 'EquipmentFuelGradeCheck',@EquipmentNotLockedOutCheck 'EquipmentNotLockedOutCheck',@EquipmentNotAssignedCheck 'EquipmentNotAssignedCheck',@EquipmentInServiceCheck 'EquipmentInServiceCheck',@TagLicenseNotExpiredCheck 'TagLicenseNotExpiredCheck',@TestInspectionNotExpiredCheck 'TestInspectionNotExpiredCheck',@QualityControlCheckupDateCheck 'QualityControlCheckupDateCheck',@CautionQualityTagCheck 'CautionQualityTagCheck',@WarningQualityTagCheck 'WarningQualityTagCheck',@DangerQualityTagCheck 'DangerQualityTagCheck',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@EnableServiceRequests 'EnableServiceRequests',@AutomaticRestartDelay 'AutomaticRestartDelay',@EquipmentRequired 'EquipmentRequired',@PersonnelRequired 'PersonnelRequired',@FillToActualOrStandard 'FillToActualOrStandard',@OperationalWindowPastHours 'OperationalWindowPastHours',@OperationalWindowFutureHours 'OperationalWindowFutureHours',@ShowGridLines 'ShowGridLines',@StaticTimeDisplay 'StaticTimeDisplay',@UseArrivalTime 'UseArrivalTime',@UseStartTime 'UseStartTime',@UseStopTime 'UseStopTime',@FuelsManagerReportURL 'FuelsManagerReportURL'
                ) AS remoteChanges ([DispatchConfigurationGuid],[SiteGuid],[ID],[DisplayCurrentTime],[DispatchDataRefreshPeriod],[TabularViewDisplayMilitaryDate],[QuantityNotZeroCheck],[ExactlyOneManagerCheck],[ExactlyOneOwnerCheck],[DispatchFuelAdditiveFlagCheck],[FastLogFuelAdditiveFlagCheck],[FillstandVolumeWithinToleranceCheck],[ReturnToBulkVolumeWithinToleranceCheck],[RecirculationVolumesGreaterThanZeroCheck],[OperatorIsInCheck],[OperatorNotAssignedCheck],[OperatorHasRequiredTrainingCheck],[OperatorTrainingNotExpiredCheck],[OperatorNotLockedOutCheck],[OperatorHasRequiredQualificationsCheck],[OperatorQualificationsNotExpiredCheck],[DefuelStatusCheck],[RefuelStatusCheck],[EquipmentFuelGradeCheck],[EquipmentNotLockedOutCheck],[EquipmentNotAssignedCheck],[EquipmentInServiceCheck],[TagLicenseNotExpiredCheck],[TestInspectionNotExpiredCheck],[QualityControlCheckupDateCheck],[CautionQualityTagCheck],[WarningQualityTagCheck],[DangerQualityTagCheck],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableServiceRequests],[AutomaticRestartDelay],[EquipmentRequired],[PersonnelRequired],[FillToActualOrStandard],[OperationalWindowPastHours],[OperationalWindowFutureHours],[ShowGridLines],[StaticTimeDisplay],[UseArrivalTime],[UseStartTime],[UseStopTime],[FuelsManagerReportURL])
        ON (existingData.[DispatchConfigurationGuid] = remoteChanges.[DispatchConfigurationGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[DisplayCurrentTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DisplayCurrentTime'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[DisplayCurrentTime] ELSE remoteChanges.[DisplayCurrentTime] END
                       ,[DispatchDataRefreshPeriod] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DispatchDataRefreshPeriod'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[DispatchDataRefreshPeriod] ELSE remoteChanges.[DispatchDataRefreshPeriod] END
                       ,[TabularViewDisplayMilitaryDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TabularViewDisplayMilitaryDate'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[TabularViewDisplayMilitaryDate] ELSE remoteChanges.[TabularViewDisplayMilitaryDate] END
                       ,[QuantityNotZeroCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QuantityNotZeroCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[QuantityNotZeroCheck] ELSE remoteChanges.[QuantityNotZeroCheck] END
                       ,[ExactlyOneManagerCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExactlyOneManagerCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[ExactlyOneManagerCheck] ELSE remoteChanges.[ExactlyOneManagerCheck] END
                       ,[ExactlyOneOwnerCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ExactlyOneOwnerCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[ExactlyOneOwnerCheck] ELSE remoteChanges.[ExactlyOneOwnerCheck] END
                       ,[DispatchFuelAdditiveFlagCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DispatchFuelAdditiveFlagCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[DispatchFuelAdditiveFlagCheck] ELSE remoteChanges.[DispatchFuelAdditiveFlagCheck] END
                       ,[FastLogFuelAdditiveFlagCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FastLogFuelAdditiveFlagCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[FastLogFuelAdditiveFlagCheck] ELSE remoteChanges.[FastLogFuelAdditiveFlagCheck] END
                       ,[FillstandVolumeWithinToleranceCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FillstandVolumeWithinToleranceCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[FillstandVolumeWithinToleranceCheck] ELSE remoteChanges.[FillstandVolumeWithinToleranceCheck] END
                       ,[ReturnToBulkVolumeWithinToleranceCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReturnToBulkVolumeWithinToleranceCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[ReturnToBulkVolumeWithinToleranceCheck] ELSE remoteChanges.[ReturnToBulkVolumeWithinToleranceCheck] END
                       ,[RecirculationVolumesGreaterThanZeroCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RecirculationVolumesGreaterThanZeroCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[RecirculationVolumesGreaterThanZeroCheck] ELSE remoteChanges.[RecirculationVolumesGreaterThanZeroCheck] END
                       ,[OperatorIsInCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorIsInCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorIsInCheck] ELSE remoteChanges.[OperatorIsInCheck] END
                       ,[OperatorNotAssignedCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorNotAssignedCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorNotAssignedCheck] ELSE remoteChanges.[OperatorNotAssignedCheck] END
                       ,[OperatorHasRequiredTrainingCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorHasRequiredTrainingCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorHasRequiredTrainingCheck] ELSE remoteChanges.[OperatorHasRequiredTrainingCheck] END
                       ,[OperatorTrainingNotExpiredCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorTrainingNotExpiredCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorTrainingNotExpiredCheck] ELSE remoteChanges.[OperatorTrainingNotExpiredCheck] END
                       ,[OperatorNotLockedOutCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorNotLockedOutCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorNotLockedOutCheck] ELSE remoteChanges.[OperatorNotLockedOutCheck] END
                       ,[OperatorHasRequiredQualificationsCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorHasRequiredQualificationsCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorHasRequiredQualificationsCheck] ELSE remoteChanges.[OperatorHasRequiredQualificationsCheck] END
                       ,[OperatorQualificationsNotExpiredCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperatorQualificationsNotExpiredCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperatorQualificationsNotExpiredCheck] ELSE remoteChanges.[OperatorQualificationsNotExpiredCheck] END
                       ,[DefuelStatusCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DefuelStatusCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[DefuelStatusCheck] ELSE remoteChanges.[DefuelStatusCheck] END
                       ,[RefuelStatusCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RefuelStatusCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[RefuelStatusCheck] ELSE remoteChanges.[RefuelStatusCheck] END
                       ,[EquipmentFuelGradeCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentFuelGradeCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[EquipmentFuelGradeCheck] ELSE remoteChanges.[EquipmentFuelGradeCheck] END
                       ,[EquipmentNotLockedOutCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentNotLockedOutCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[EquipmentNotLockedOutCheck] ELSE remoteChanges.[EquipmentNotLockedOutCheck] END
                       ,[EquipmentNotAssignedCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentNotAssignedCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[EquipmentNotAssignedCheck] ELSE remoteChanges.[EquipmentNotAssignedCheck] END
                       ,[EquipmentInServiceCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentInServiceCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[EquipmentInServiceCheck] ELSE remoteChanges.[EquipmentInServiceCheck] END
                       ,[TagLicenseNotExpiredCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TagLicenseNotExpiredCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[TagLicenseNotExpiredCheck] ELSE remoteChanges.[TagLicenseNotExpiredCheck] END
                       ,[TestInspectionNotExpiredCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TestInspectionNotExpiredCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[TestInspectionNotExpiredCheck] ELSE remoteChanges.[TestInspectionNotExpiredCheck] END
                       ,[QualityControlCheckupDateCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QualityControlCheckupDateCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[QualityControlCheckupDateCheck] ELSE remoteChanges.[QualityControlCheckupDateCheck] END
                       ,[CautionQualityTagCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CautionQualityTagCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[CautionQualityTagCheck] ELSE remoteChanges.[CautionQualityTagCheck] END
                       ,[WarningQualityTagCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('WarningQualityTagCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[WarningQualityTagCheck] ELSE remoteChanges.[WarningQualityTagCheck] END
                       ,[DangerQualityTagCheck] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DangerQualityTagCheck'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[DangerQualityTagCheck] ELSE remoteChanges.[DangerQualityTagCheck] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[EnableServiceRequests] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EnableServiceRequests'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[EnableServiceRequests] ELSE remoteChanges.[EnableServiceRequests] END
                       ,[AutomaticRestartDelay] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AutomaticRestartDelay'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[AutomaticRestartDelay] ELSE remoteChanges.[AutomaticRestartDelay] END
                       ,[EquipmentRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentRequired'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[EquipmentRequired] ELSE remoteChanges.[EquipmentRequired] END
                       ,[PersonnelRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PersonnelRequired'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[PersonnelRequired] ELSE remoteChanges.[PersonnelRequired] END
                       ,[FillToActualOrStandard] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FillToActualOrStandard'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[FillToActualOrStandard] ELSE remoteChanges.[FillToActualOrStandard] END
                       ,[OperationalWindowPastHours] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperationalWindowPastHours'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperationalWindowPastHours] ELSE remoteChanges.[OperationalWindowPastHours] END
                       ,[OperationalWindowFutureHours] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OperationalWindowFutureHours'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[OperationalWindowFutureHours] ELSE remoteChanges.[OperationalWindowFutureHours] END
                       ,[ShowGridLines] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ShowGridLines'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[ShowGridLines] ELSE remoteChanges.[ShowGridLines] END
                       ,[StaticTimeDisplay] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StaticTimeDisplay'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[StaticTimeDisplay] ELSE remoteChanges.[StaticTimeDisplay] END
                       ,[UseArrivalTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseArrivalTime'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[UseArrivalTime] ELSE remoteChanges.[UseArrivalTime] END
                       ,[UseStartTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseStartTime'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[UseStartTime] ELSE remoteChanges.[UseStartTime] END
                       ,[UseStopTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UseStopTime'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[UseStopTime] ELSE remoteChanges.[UseStopTime] END
                       ,[FuelsManagerReportURL] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelsManagerReportURL'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN existingData.[FuelsManagerReportURL] ELSE remoteChanges.[FuelsManagerReportURL] END

        WHEN NOT MATCHED THEN
            INSERT ([DispatchConfigurationGuid],[SiteGuid],[ID],[DisplayCurrentTime],[DispatchDataRefreshPeriod],[TabularViewDisplayMilitaryDate],[QuantityNotZeroCheck],[ExactlyOneManagerCheck],[ExactlyOneOwnerCheck],[DispatchFuelAdditiveFlagCheck],[FastLogFuelAdditiveFlagCheck],[FillstandVolumeWithinToleranceCheck],[ReturnToBulkVolumeWithinToleranceCheck],[RecirculationVolumesGreaterThanZeroCheck],[OperatorIsInCheck],[OperatorNotAssignedCheck],[OperatorHasRequiredTrainingCheck],[OperatorTrainingNotExpiredCheck],[OperatorNotLockedOutCheck],[OperatorHasRequiredQualificationsCheck],[OperatorQualificationsNotExpiredCheck],[DefuelStatusCheck],[RefuelStatusCheck],[EquipmentFuelGradeCheck],[EquipmentNotLockedOutCheck],[EquipmentNotAssignedCheck],[EquipmentInServiceCheck],[TagLicenseNotExpiredCheck],[TestInspectionNotExpiredCheck],[QualityControlCheckupDateCheck],[CautionQualityTagCheck],[WarningQualityTagCheck],[DangerQualityTagCheck],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[EnableServiceRequests],[AutomaticRestartDelay],[EquipmentRequired],[PersonnelRequired],[FillToActualOrStandard],[OperationalWindowPastHours],[OperationalWindowFutureHours],[ShowGridLines],[StaticTimeDisplay],[UseArrivalTime],[UseStartTime],[UseStopTime],[FuelsManagerReportURL])
                VALUES (@DispatchConfigurationGuid,@SiteGuid,@ID,@DisplayCurrentTime,@DispatchDataRefreshPeriod,@TabularViewDisplayMilitaryDate,@QuantityNotZeroCheck,@ExactlyOneManagerCheck,@ExactlyOneOwnerCheck,@DispatchFuelAdditiveFlagCheck,@FastLogFuelAdditiveFlagCheck,@FillstandVolumeWithinToleranceCheck,@ReturnToBulkVolumeWithinToleranceCheck,@RecirculationVolumesGreaterThanZeroCheck,@OperatorIsInCheck,@OperatorNotAssignedCheck,@OperatorHasRequiredTrainingCheck,@OperatorTrainingNotExpiredCheck,@OperatorNotLockedOutCheck,@OperatorHasRequiredQualificationsCheck,@OperatorQualificationsNotExpiredCheck,@DefuelStatusCheck,@RefuelStatusCheck,@EquipmentFuelGradeCheck,@EquipmentNotLockedOutCheck,@EquipmentNotAssignedCheck,@EquipmentInServiceCheck,@TagLicenseNotExpiredCheck,@TestInspectionNotExpiredCheck,@QualityControlCheckupDateCheck,@CautionQualityTagCheck,@WarningQualityTagCheck,@DangerQualityTagCheck,@CreatedDate,@CreatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN NULL ELSE @UpdatedBy END),@EnableServiceRequests,@AutomaticRestartDelay,@EquipmentRequired,@PersonnelRequired,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FillToActualOrStandard'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN NULL ELSE @FillToActualOrStandard END),@OperationalWindowPastHours,@OperationalWindowFutureHours,@ShowGridLines,@StaticTimeDisplay,@UseArrivalTime,@UseStartTime,@UseStopTime,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelsManagerReportURL'), @sync_supported_columns_tblDispatchConfiguration)) WHEN 0 THEN NULL ELSE @FuelsManagerReportURL END))
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
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

