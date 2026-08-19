-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblQualifications
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualifications]
@QualificationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblQualifications].[ID],[dbo].[tblQualifications].[Description],[dbo].[tblQualifications].[Duration],[dbo].[tblQualifications].[Reoccurrence],[dbo].[tblQualifications].[CreatedDate],[dbo].[tblQualifications].[CreatedBy],[dbo].[tblQualifications].[UpdatedDate],[dbo].[tblQualifications].[UpdatedBy],[dbo].[tblQualifications].[QualificationGuid],[dbo].[tblQualifications].[SiteGuid],[dbo].[tblQualifications].[LookupQualificationTypeIndex], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblQualifications]
            INNER JOIN [track].[tblQualifications] CT
                ON CT.PK_QualificationGuid = [dbo].[tblQualifications].[QualificationGuid]
        WHERE CT.PK_QualificationGuid = @QualificationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
