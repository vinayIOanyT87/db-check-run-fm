-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointTemplateProperty
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointTemplateProperty]
@PointTemplatePropertyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointTemplateProperty].[ID],[dbo].[tblPointTemplateProperty].[ValueType],[dbo].[tblPointTemplateProperty].[Value],[dbo].[tblPointTemplateProperty].[CreatedDate],[dbo].[tblPointTemplateProperty].[CreatedBy],[dbo].[tblPointTemplateProperty].[UpdatedDate],[dbo].[tblPointTemplateProperty].[UpdatedBy],[dbo].[tblPointTemplateProperty].[PointTemplatePropertyGuid],[dbo].[tblPointTemplateProperty].[PointTemplateGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointTemplateProperty]
            INNER JOIN [track].[tblPointTemplateProperty] CT
                ON CT.PK_PointTemplatePropertyGuid = [dbo].[tblPointTemplateProperty].[PointTemplatePropertyGuid]
        WHERE CT.PK_PointTemplatePropertyGuid = @PointTemplatePropertyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
