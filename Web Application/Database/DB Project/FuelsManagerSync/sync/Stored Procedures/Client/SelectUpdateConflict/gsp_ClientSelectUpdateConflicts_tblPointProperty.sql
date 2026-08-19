-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointProperty
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointProperty]
@PointPropertyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointProperty].[ID],[dbo].[tblPointProperty].[ValueType],[dbo].[tblPointProperty].[Value],[dbo].[tblPointProperty].[CreatedDate],[dbo].[tblPointProperty].[CreatedBy],[dbo].[tblPointProperty].[UpdatedDate],[dbo].[tblPointProperty].[UpdatedBy],[dbo].[tblPointProperty].[PointPropertyGuid],[dbo].[tblPointProperty].[PointTemplatePropertyGuid],[dbo].[tblPointProperty].[PointGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointProperty]
            INNER JOIN [track].[tblPointProperty] CT
                ON CT.PK_PointPropertyGuid = [dbo].[tblPointProperty].[PointPropertyGuid]
        WHERE CT.PK_PointPropertyGuid = @PointPropertyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
