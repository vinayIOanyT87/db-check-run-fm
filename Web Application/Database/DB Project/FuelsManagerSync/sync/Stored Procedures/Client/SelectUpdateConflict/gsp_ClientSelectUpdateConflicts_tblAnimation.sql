-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAnimation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAnimation]
@AnimationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAnimation].[AnimationGuid],[dbo].[tblAnimation].[ID],[dbo].[tblAnimation].[SiteGuid],[dbo].[tblAnimation].[AnimationTestGroupList],[dbo].[tblAnimation].[CreatedDate],[dbo].[tblAnimation].[CreatedBy],[dbo].[tblAnimation].[UpdatedDate],[dbo].[tblAnimation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAnimation]
            INNER JOIN [track].[tblAnimation] CT
                ON CT.PK_AnimationGuid = [dbo].[tblAnimation].[AnimationGuid]
        WHERE CT.PK_AnimationGuid = @AnimationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
