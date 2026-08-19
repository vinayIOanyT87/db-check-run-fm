-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblPictures
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblPictures]
@PictureGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblPictures].[PictureGuid],[dbo].[tblPictures].[ID],[dbo].[tblPictures].[Description],[dbo].[tblPictures].[ImageStream],[dbo].[tblPictures].[IsSystemImage],[dbo].[tblPictures].[ImageHash],[dbo].[tblPictures].[SiteGuid],[dbo].[tblPictures].[CreatedDate],[dbo].[tblPictures].[CreatedBy],[dbo].[tblPictures].[UpdatedDate],[dbo].[tblPictures].[UpdatedBy],[dbo].[tblPictures].[ContentType], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblPictures]
            INNER JOIN [track].[tblPictures] CT
                ON CT.PK_PictureGuid = [dbo].[tblPictures].[PictureGuid]
        WHERE CT.PK_PictureGuid = @PictureGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
