-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEntryMessageToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityEntryMessageToSite]
@EntryMessageToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEntryMessageToSite].[EntryMessageToSiteGuid],[map].[tblEntityEntryMessageToSite].[ApplicationStringGuid],[map].[tblEntityEntryMessageToSite].[SiteGuid],[map].[tblEntityEntryMessageToSite].[CreatedDate],[map].[tblEntityEntryMessageToSite].[CreatedBy],[map].[tblEntityEntryMessageToSite].[UpdatedDate],[map].[tblEntityEntryMessageToSite].[UpdatedBy],[map].[tblEntityEntryMessageToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEntryMessageToSite]
            INNER JOIN [track].[tblEntityEntryMessageToSite] CT
                ON CT.PK_EntryMessageToSiteGuid = [map].[tblEntityEntryMessageToSite].[EntryMessageToSiteGuid]
        WHERE CT.PK_EntryMessageToSiteGuid = @EntryMessageToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
