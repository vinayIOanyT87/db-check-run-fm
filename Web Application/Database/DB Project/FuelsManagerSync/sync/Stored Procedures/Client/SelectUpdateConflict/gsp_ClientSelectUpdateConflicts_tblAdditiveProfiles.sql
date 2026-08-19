-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAdditiveProfiles
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAdditiveProfiles]
@AdditiveProfileGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAdditiveProfiles].[ID],[dbo].[tblAdditiveProfiles].[Description],[dbo].[tblAdditiveProfiles].[CreatedDate],[dbo].[tblAdditiveProfiles].[CreatedBy],[dbo].[tblAdditiveProfiles].[UpdatedDate],[dbo].[tblAdditiveProfiles].[UpdatedBy],[dbo].[tblAdditiveProfiles].[AdditiveProfileGuid],[dbo].[tblAdditiveProfiles].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAdditiveProfiles]
            INNER JOIN [track].[tblAdditiveProfiles] CT
                ON CT.PK_AdditiveProfileGuid = [dbo].[tblAdditiveProfiles].[AdditiveProfileGuid]
        WHERE CT.PK_AdditiveProfileGuid = @AdditiveProfileGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
