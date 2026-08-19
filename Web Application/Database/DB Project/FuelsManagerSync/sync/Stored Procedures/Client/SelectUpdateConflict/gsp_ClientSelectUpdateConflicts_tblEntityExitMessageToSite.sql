-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityExitMessageToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityExitMessageToSite]
@ExitMessageToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityExitMessageToSite].[ExitMessageToSiteGuid],[map].[tblEntityExitMessageToSite].[ApplicationStringGuid],[map].[tblEntityExitMessageToSite].[SiteGuid],[map].[tblEntityExitMessageToSite].[CreatedDate],[map].[tblEntityExitMessageToSite].[CreatedBy],[map].[tblEntityExitMessageToSite].[UpdatedDate],[map].[tblEntityExitMessageToSite].[UpdatedBy],[map].[tblEntityExitMessageToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityExitMessageToSite]
            INNER JOIN [track].[tblEntityExitMessageToSite] CT
                ON CT.PK_ExitMessageToSiteGuid = [map].[tblEntityExitMessageToSite].[ExitMessageToSiteGuid]
        WHERE CT.PK_ExitMessageToSiteGuid = @ExitMessageToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
