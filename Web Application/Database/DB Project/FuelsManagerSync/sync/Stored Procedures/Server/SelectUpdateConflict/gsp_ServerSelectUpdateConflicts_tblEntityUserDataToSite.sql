-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityUserDataToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityUserDataToSite]
@UserDataToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid],[map].[tblEntityUserDataToSite].[OwnerSiteGuid],[map].[tblEntityUserDataToSite].[MapToSiteGuid],[map].[tblEntityUserDataToSite].[CreatedDate],[map].[tblEntityUserDataToSite].[CreatedBy],[map].[tblEntityUserDataToSite].[UpdatedDate],[map].[tblEntityUserDataToSite].[UpdatedBy],[map].[tblEntityUserDataToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityUserDataToSite]
            INNER JOIN [track].[tblEntityUserDataToSite] CT
                ON CT.PK_UserDataToSiteGuid = [map].[tblEntityUserDataToSite].[UserDataToSiteGuid]
        WHERE CT.PK_UserDataToSiteGuid = @UserDataToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
