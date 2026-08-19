-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblSiteToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblSiteToSite]
@SiteToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblSiteToSite].[SiteToSiteGuid],[map].[tblSiteToSite].[ParentSiteGuid],[map].[tblSiteToSite].[ChildSiteGuid],[map].[tblSiteToSite].[CreatedDate],[map].[tblSiteToSite].[CreatedBy],[map].[tblSiteToSite].[UpdatedDate],[map].[tblSiteToSite].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblSiteToSite]
            INNER JOIN [track].[tblSiteToSite] CT
                ON CT.PK_SiteToSiteGuid = [map].[tblSiteToSite].[SiteToSiteGuid]
        WHERE CT.PK_SiteToSiteGuid = @SiteToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
