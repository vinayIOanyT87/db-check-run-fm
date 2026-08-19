-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityUserToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblEntityUserToSite]
@UserToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityUserToSite].[UserToSiteGuid],[map].[tblEntityUserToSite].[UserGuid],[map].[tblEntityUserToSite].[SiteGuid],[map].[tblEntityUserToSite].[CreatedDate],[map].[tblEntityUserToSite].[CreatedBy],[map].[tblEntityUserToSite].[UpdatedDate],[map].[tblEntityUserToSite].[UpdatedBy],[map].[tblEntityUserToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityUserToSite]
            INNER JOIN [track].[tblEntityUserToSite] CT
                ON CT.PK_UserToSiteGuid = [map].[tblEntityUserToSite].[UserToSiteGuid]
        WHERE CT.PK_UserToSiteGuid = @UserToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
