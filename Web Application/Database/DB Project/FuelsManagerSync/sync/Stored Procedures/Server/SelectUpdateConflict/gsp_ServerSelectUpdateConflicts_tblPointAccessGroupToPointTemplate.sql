-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblPointAccessGroupToPointTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointAccessGroupToPointTemplate]
@PointAccessGroupToPointTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblPointAccessGroupToPointTemplate].[PointAccessGroupToPointTemplateGuid],[map].[tblPointAccessGroupToPointTemplate].[PointAccessGroupGuid],[map].[tblPointAccessGroupToPointTemplate].[PointTemplateGuid],[map].[tblPointAccessGroupToPointTemplate].[CreatedDate],[map].[tblPointAccessGroupToPointTemplate].[CreatedBy],[map].[tblPointAccessGroupToPointTemplate].[UpdatedDate],[map].[tblPointAccessGroupToPointTemplate].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblPointAccessGroupToPointTemplate]
            INNER JOIN [track].[tblPointAccessGroupToPointTemplate] CT
                ON CT.PK_PointAccessGroupToPointTemplateGuid = [map].[tblPointAccessGroupToPointTemplate].[PointAccessGroupToPointTemplateGuid]
        WHERE CT.PK_PointAccessGroupToPointTemplateGuid = @PointAccessGroupToPointTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
