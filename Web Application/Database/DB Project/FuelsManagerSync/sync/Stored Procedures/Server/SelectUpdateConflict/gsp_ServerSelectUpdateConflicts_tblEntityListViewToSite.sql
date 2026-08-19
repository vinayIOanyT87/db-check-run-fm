-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityListViewToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityListViewToSite]
@ListViewToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityListViewToSite].[ListViewToSiteGuid],[map].[tblEntityListViewToSite].[ListViewGuid],[map].[tblEntityListViewToSite].[SiteGuid],[map].[tblEntityListViewToSite].[CreatedDate],[map].[tblEntityListViewToSite].[CreatedBy],[map].[tblEntityListViewToSite].[UpdatedDate],[map].[tblEntityListViewToSite].[UpdatedBy],[map].[tblEntityListViewToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityListViewToSite]
            INNER JOIN [track].[tblEntityListViewToSite] CT
                ON CT.PK_ListViewToSiteGuid = [map].[tblEntityListViewToSite].[ListViewToSiteGuid]
        WHERE CT.PK_ListViewToSiteGuid = @ListViewToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
