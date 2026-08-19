-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblQualityTags
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualityTags]
@QualityTagGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblQualityTags].[Name],[dbo].[tblQualityTags].[Severity],[dbo].[tblQualityTags].[Active],[dbo].[tblQualityTags].[CreatedDate],[dbo].[tblQualityTags].[CreatedBy],[dbo].[tblQualityTags].[UpdatedDate],[dbo].[tblQualityTags].[UpdatedBy],[dbo].[tblQualityTags].[QualityTagGuid],[dbo].[tblQualityTags].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblQualityTags]
            INNER JOIN [track].[tblQualityTags] CT
                ON CT.PK_QualityTagGuid = [dbo].[tblQualityTags].[QualityTagGuid]
        WHERE CT.PK_QualityTagGuid = @QualityTagGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
