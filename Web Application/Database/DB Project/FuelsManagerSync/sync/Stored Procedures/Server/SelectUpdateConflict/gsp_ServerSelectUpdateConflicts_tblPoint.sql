-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPoint
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPoint]
@PointGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPoint].[ID],[dbo].[tblPoint].[Description],[dbo].[tblPoint].[Enabled],[dbo].[tblPoint].[Standard],[dbo].[tblPoint].[ExecutionInterval],[dbo].[tblPoint].[LevelUnitIndex],[dbo].[tblPoint].[TemperatureUnitIndex],[dbo].[tblPoint].[DensityUnitIndex],[dbo].[tblPoint].[PressureUnitIndex],[dbo].[tblPoint].[FlowUnitIndex],[dbo].[tblPoint].[VolumeUnitIndex],[dbo].[tblPoint].[MassUnitIndex],[dbo].[tblPoint].[VelocityUnitIndex],[dbo].[tblPoint].[MassFlowUnitIndex],[dbo].[tblPoint].[LevelDecimalPlaces],[dbo].[tblPoint].[TemperatureDecimalPlaces],[dbo].[tblPoint].[DensityDecimalPlaces],[dbo].[tblPoint].[PressureDecimalPlaces],[dbo].[tblPoint].[FlowDecimalPlaces],[dbo].[tblPoint].[VolumeDecimalPlaces],[dbo].[tblPoint].[MassDecimalPlaces],[dbo].[tblPoint].[VelocityDecimalPlaces],[dbo].[tblPoint].[MassFlowDecimalPlaces],[dbo].[tblPoint].[LevelMaximum],[dbo].[tblPoint].[LevelMinimum],[dbo].[tblPoint].[TemperatureMaximum],[dbo].[tblPoint].[TemperatureMinimum],[dbo].[tblPoint].[DensityMaximum],[dbo].[tblPoint].[DensityMinimum],[dbo].[tblPoint].[PressureMaximum],[dbo].[tblPoint].[PressureMinimum],[dbo].[tblPoint].[VolumetricFlowMaximum],[dbo].[tblPoint].[VolumetricFlowMinimum],[dbo].[tblPoint].[VolumeMaximum],[dbo].[tblPoint].[VolumeMinimum],[dbo].[tblPoint].[MassMaximum],[dbo].[tblPoint].[MassMinimum],[dbo].[tblPoint].[VelocityMaximum],[dbo].[tblPoint].[VelocityMinimum],[dbo].[tblPoint].[MassFlowMaximum],[dbo].[tblPoint].[MassFlowMinimum],[dbo].[tblPoint].[CreatedDate],[dbo].[tblPoint].[CreatedBy],[dbo].[tblPoint].[UpdatedDate],[dbo].[tblPoint].[UpdatedBy],[dbo].[tblPoint].[PointGuid],[dbo].[tblPoint].[SiteGuid],[dbo].[tblPoint].[PointTemplateGuid],[dbo].[tblPoint].[ProfileImageGuid],[dbo].[tblPoint].[ProductGuid],[dbo].[tblPoint].[Notes],[dbo].[tblPoint].[OverrideDefaultDrawingGuid],[dbo].[tblPoint].[PointTemplateVersion], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPoint]
            INNER JOIN [track].[tblPoint] CT
                ON CT.PK_PointGuid = [dbo].[tblPoint].[PointGuid]
        WHERE CT.PK_PointGuid = @PointGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
