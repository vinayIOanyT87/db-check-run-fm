-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplateTag
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointTemplateTag]
@PointTemplateTagGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointTemplateTag].[ID],[dbo].[tblPointTemplateTag].[EngineeringUnitsType],[dbo].[tblPointTemplateTag].[EngineeringUnitsIndex],[dbo].[tblPointTemplateTag].[DecimalPlaces],[dbo].[tblPointTemplateTag].[ServerEngineeringUnitsIndex],[dbo].[tblPointTemplateTag].[ValueType],[dbo].[tblPointTemplateTag].[Value],[dbo].[tblPointTemplateTag].[Maximum],[dbo].[tblPointTemplateTag].[Minimum],[dbo].[tblPointTemplateTag].[PointTagInputOutputTypeIndex],[dbo].[tblPointTemplateTag].[Input],[dbo].[tblPointTemplateTag].[AlarmStatus],[dbo].[tblPointTemplateTag].[ApplyPointTemplateEngineeringUnits],[dbo].[tblPointTemplateTag].[ApplyPointTemplateDecimalPlaces],[dbo].[tblPointTemplateTag].[ApplyPointTemplateMaximum],[dbo].[tblPointTemplateTag].[ApplyPointTemplateMinimum],[dbo].[tblPointTemplateTag].[CreatedDate],[dbo].[tblPointTemplateTag].[CreatedBy],[dbo].[tblPointTemplateTag].[UpdatedDate],[dbo].[tblPointTemplateTag].[UpdatedBy],[dbo].[tblPointTemplateTag].[PointTemplateTagGuid],[dbo].[tblPointTemplateTag].[PointTemplateGuid],[dbo].[tblPointTemplateTag].[WellKnownIdentityGuid],[dbo].[tblPointTemplateTag].[AlarmsEnabled],[dbo].[tblPointTemplateTag].[InhibitInputOutputTypeConfiguration],[dbo].[tblPointTemplateTag].[InhibitOverride],[dbo].[tblPointTemplateTag].[Module],[dbo].[tblPointTemplateTag].[Archived], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointTemplateTag]
            INNER JOIN [track].[tblPointTemplateTag] CT
                ON CT.PK_PointTemplateTagGuid = [dbo].[tblPointTemplateTag].[PointTemplateTagGuid]
        WHERE CT.PK_PointTemplateTagGuid = @PointTemplateTagGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
