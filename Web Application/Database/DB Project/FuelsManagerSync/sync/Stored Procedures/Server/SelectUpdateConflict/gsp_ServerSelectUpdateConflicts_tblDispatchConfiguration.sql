-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblDispatchConfiguration
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblDispatchConfiguration]
@DispatchConfigurationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid],[dbo].[tblDispatchConfiguration].[SiteGuid],[dbo].[tblDispatchConfiguration].[ID],[dbo].[tblDispatchConfiguration].[DisplayCurrentTime],[dbo].[tblDispatchConfiguration].[DispatchDataRefreshPeriod],[dbo].[tblDispatchConfiguration].[TabularViewDisplayMilitaryDate],[dbo].[tblDispatchConfiguration].[QuantityNotZeroCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneManagerCheck],[dbo].[tblDispatchConfiguration].[ExactlyOneOwnerCheck],[dbo].[tblDispatchConfiguration].[DispatchFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FastLogFuelAdditiveFlagCheck],[dbo].[tblDispatchConfiguration].[FillstandVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[ReturnToBulkVolumeWithinToleranceCheck],[dbo].[tblDispatchConfiguration].[RecirculationVolumesGreaterThanZeroCheck],[dbo].[tblDispatchConfiguration].[OperatorIsInCheck],[dbo].[tblDispatchConfiguration].[OperatorNotAssignedCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredTrainingCheck],[dbo].[tblDispatchConfiguration].[OperatorTrainingNotExpiredCheck],[dbo].[tblDispatchConfiguration].[OperatorNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[OperatorHasRequiredQualificationsCheck],[dbo].[tblDispatchConfiguration].[OperatorQualificationsNotExpiredCheck],[dbo].[tblDispatchConfiguration].[DefuelStatusCheck],[dbo].[tblDispatchConfiguration].[RefuelStatusCheck],[dbo].[tblDispatchConfiguration].[EquipmentFuelGradeCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotLockedOutCheck],[dbo].[tblDispatchConfiguration].[EquipmentNotAssignedCheck],[dbo].[tblDispatchConfiguration].[EquipmentInServiceCheck],[dbo].[tblDispatchConfiguration].[TagLicenseNotExpiredCheck],[dbo].[tblDispatchConfiguration].[TestInspectionNotExpiredCheck],[dbo].[tblDispatchConfiguration].[QualityControlCheckupDateCheck],[dbo].[tblDispatchConfiguration].[CautionQualityTagCheck],[dbo].[tblDispatchConfiguration].[WarningQualityTagCheck],[dbo].[tblDispatchConfiguration].[DangerQualityTagCheck],[dbo].[tblDispatchConfiguration].[CreatedDate],[dbo].[tblDispatchConfiguration].[CreatedBy],[dbo].[tblDispatchConfiguration].[UpdatedDate],[dbo].[tblDispatchConfiguration].[UpdatedBy],[dbo].[tblDispatchConfiguration].[EnableServiceRequests],[dbo].[tblDispatchConfiguration].[AutomaticRestartDelay],[dbo].[tblDispatchConfiguration].[EquipmentRequired],[dbo].[tblDispatchConfiguration].[PersonnelRequired],[dbo].[tblDispatchConfiguration].[FillToActualOrStandard],[dbo].[tblDispatchConfiguration].[OperationalWindowPastHours],[dbo].[tblDispatchConfiguration].[OperationalWindowFutureHours],[dbo].[tblDispatchConfiguration].[ShowGridLines],[dbo].[tblDispatchConfiguration].[StaticTimeDisplay],[dbo].[tblDispatchConfiguration].[UseArrivalTime],[dbo].[tblDispatchConfiguration].[UseStartTime],[dbo].[tblDispatchConfiguration].[UseStopTime],[dbo].[tblDispatchConfiguration].[FuelsManagerReportURL], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblDispatchConfiguration]
            INNER JOIN [track].[tblDispatchConfiguration] CT
                ON CT.PK_DispatchConfigurationGuid = [dbo].[tblDispatchConfiguration].[DispatchConfigurationGuid]
        WHERE CT.PK_DispatchConfigurationGuid = @DispatchConfigurationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
