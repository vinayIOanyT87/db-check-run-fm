-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityUserGroupToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityUserGroupToSite]
@UserGroupToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityUserGroupToSite].[UserGroupToSiteGuid],[map].[tblEntityUserGroupToSite].[GroupGuid],[map].[tblEntityUserGroupToSite].[SiteGuid],[map].[tblEntityUserGroupToSite].[CreatedDate],[map].[tblEntityUserGroupToSite].[CreatedBy],[map].[tblEntityUserGroupToSite].[UpdatedDate],[map].[tblEntityUserGroupToSite].[UpdatedBy],[map].[tblEntityUserGroupToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityUserGroupToSite]
            INNER JOIN [track].[tblEntityUserGroupToSite] CT
                ON CT.PK_UserGroupToSiteGuid = [map].[tblEntityUserGroupToSite].[UserGroupToSiteGuid]
        WHERE CT.PK_UserGroupToSiteGuid = @UserGroupToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
