-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityEmailGroupToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityEmailGroupToSite]
@EmailGroupToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityEmailGroupToSite].[EmailGroupToSiteGuid],[map].[tblEntityEmailGroupToSite].[EmailGroupGuid],[map].[tblEntityEmailGroupToSite].[SiteGuid],[map].[tblEntityEmailGroupToSite].[CreatedDate],[map].[tblEntityEmailGroupToSite].[CreatedBy],[map].[tblEntityEmailGroupToSite].[UpdatedDate],[map].[tblEntityEmailGroupToSite].[UpdatedBy],[map].[tblEntityEmailGroupToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityEmailGroupToSite]
            INNER JOIN [track].[tblEntityEmailGroupToSite] CT
                ON CT.PK_EmailGroupToSiteGuid = [map].[tblEntityEmailGroupToSite].[EmailGroupToSiteGuid]
        WHERE CT.PK_EmailGroupToSiteGuid = @EmailGroupToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
