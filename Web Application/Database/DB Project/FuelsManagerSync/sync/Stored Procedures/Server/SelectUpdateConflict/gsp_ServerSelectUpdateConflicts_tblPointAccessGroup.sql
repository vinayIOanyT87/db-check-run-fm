-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointAccessGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblPointAccessGroup]
@PointAccessGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointAccessGroup].[PointAccessGroupGuid],[dbo].[tblPointAccessGroup].[ID],[dbo].[tblPointAccessGroup].[SiteGuid],[dbo].[tblPointAccessGroup].[CreatedDate],[dbo].[tblPointAccessGroup].[CreatedBy],[dbo].[tblPointAccessGroup].[UpdatedDate],[dbo].[tblPointAccessGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointAccessGroup]
            INNER JOIN [track].[tblPointAccessGroup] CT
                ON CT.PK_PointAccessGroupGuid = [dbo].[tblPointAccessGroup].[PointAccessGroupGuid]
        WHERE CT.PK_PointAccessGroupGuid = @PointAccessGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
