-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointTemplate]
@PointTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointTemplate].[ID],[dbo].[tblPointTemplate].[Description],[dbo].[tblPointTemplate].[Standard],[dbo].[tblPointTemplate].[ExecutionInterval],[dbo].[tblPointTemplate].[LevelUnitIndex],[dbo].[tblPointTemplate].[TemperatureUnitIndex],[dbo].[tblPointTemplate].[DensityUnitIndex],[dbo].[tblPointTemplate].[PressureUnitIndex],[dbo].[tblPointTemplate].[FlowUnitIndex],[dbo].[tblPointTemplate].[VolumeUnitIndex],[dbo].[tblPointTemplate].[MassUnitIndex],[dbo].[tblPointTemplate].[VelocityUnitIndex],[dbo].[tblPointTemplate].[MassFlowUnitIndex],[dbo].[tblPointTemplate].[LevelDecimalPlaces],[dbo].[tblPointTemplate].[TemperatureDecimalPlaces],[dbo].[tblPointTemplate].[DensityDecimalPlaces],[dbo].[tblPointTemplate].[PressureDecimalPlaces],[dbo].[tblPointTemplate].[FlowDecimalPlaces],[dbo].[tblPointTemplate].[VolumeDecimalPlaces],[dbo].[tblPointTemplate].[MassDecimalPlaces],[dbo].[tblPointTemplate].[VelocityDecimalPlaces],[dbo].[tblPointTemplate].[MassFlowDecimalPlaces],[dbo].[tblPointTemplate].[LevelMaximum],[dbo].[tblPointTemplate].[LevelMinimum],[dbo].[tblPointTemplate].[TemperatureMaximum],[dbo].[tblPointTemplate].[TemperatureMinimum],[dbo].[tblPointTemplate].[DensityMaximum],[dbo].[tblPointTemplate].[DensityMinimum],[dbo].[tblPointTemplate].[PressureMaximum],[dbo].[tblPointTemplate].[PressureMinimum],[dbo].[tblPointTemplate].[VolumetricFlowMaximum],[dbo].[tblPointTemplate].[VolumetricFlowMinimum],[dbo].[tblPointTemplate].[VolumeMaximum],[dbo].[tblPointTemplate].[VolumeMinimum],[dbo].[tblPointTemplate].[MassMaximum],[dbo].[tblPointTemplate].[MassMinimum],[dbo].[tblPointTemplate].[VelocityMaximum],[dbo].[tblPointTemplate].[VelocityMinimum],[dbo].[tblPointTemplate].[MassFlowMaximum],[dbo].[tblPointTemplate].[MassFlowMinimum],[dbo].[tblPointTemplate].[CreatedDate],[dbo].[tblPointTemplate].[CreatedBy],[dbo].[tblPointTemplate].[UpdatedDate],[dbo].[tblPointTemplate].[UpdatedBy],[dbo].[tblPointTemplate].[PointTemplateGuid],[dbo].[tblPointTemplate].[SiteGuid],[dbo].[tblPointTemplate].[PointTemplateTypeApplicationStringGuid],[dbo].[tblPointTemplate].[ProfileImageGuid],[dbo].[tblPointTemplate].[DefaultDrawingGuid],[dbo].[tblPointTemplate].[PointCommandStatus],[dbo].[tblPointTemplate].[DeviceAlarmMaps],[dbo].[tblPointTemplate].[PointLogicScript],[dbo].[tblPointTemplate].[Version], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointTemplate]
            INNER JOIN [track].[tblPointTemplate] CT
                ON CT.PK_PointTemplateGuid = [dbo].[tblPointTemplate].[PointTemplateGuid]
        WHERE CT.PK_PointTemplateGuid = @PointTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
