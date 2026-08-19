-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPointGroup
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPointGroup]
@PointGroupGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPointGroup].[PointGroupGuid],[dbo].[tblPointGroup].[ID],[dbo].[tblPointGroup].[Description],[dbo].[tblPointGroup].[PointGroupType],[dbo].[tblPointGroup].[OwnerUserGuid],[dbo].[tblPointGroup].[SiteGuid],[dbo].[tblPointGroup].[CreatedDate],[dbo].[tblPointGroup].[CreatedBy],[dbo].[tblPointGroup].[UpdatedDate],[dbo].[tblPointGroup].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPointGroup]
            INNER JOIN [track].[tblPointGroup] CT
                ON CT.PK_PointGroupGuid = [dbo].[tblPointGroup].[PointGroupGuid]
        WHERE CT.PK_PointGroupGuid = @PointGroupGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
