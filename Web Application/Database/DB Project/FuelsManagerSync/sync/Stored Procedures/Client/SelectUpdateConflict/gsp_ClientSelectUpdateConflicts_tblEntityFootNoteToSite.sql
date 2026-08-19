-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityFootNoteToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityFootNoteToSite]
@FootNoteToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityFootNoteToSite].[FootNoteToSiteGuid],[map].[tblEntityFootNoteToSite].[ApplicationStringGuid],[map].[tblEntityFootNoteToSite].[SiteGuid],[map].[tblEntityFootNoteToSite].[CreatedDate],[map].[tblEntityFootNoteToSite].[CreatedBy],[map].[tblEntityFootNoteToSite].[UpdatedDate],[map].[tblEntityFootNoteToSite].[UpdatedBy],[map].[tblEntityFootNoteToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityFootNoteToSite]
            INNER JOIN [track].[tblEntityFootNoteToSite] CT
                ON CT.PK_FootNoteToSiteGuid = [map].[tblEntityFootNoteToSite].[FootNoteToSiteGuid]
        WHERE CT.PK_FootNoteToSiteGuid = @FootNoteToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
