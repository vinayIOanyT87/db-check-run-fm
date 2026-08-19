-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataListValueSite]
@UserDataListValueSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueSite].[UserDataListValueSiteGuid],[dbo].[tblUserDataListValueSite].[UserDataFieldSiteGuid],[dbo].[tblUserDataListValueSite].[Value],[dbo].[tblUserDataListValueSite].[CreatedDate],[dbo].[tblUserDataListValueSite].[CreatedBy],[dbo].[tblUserDataListValueSite].[UpdatedDate],[dbo].[tblUserDataListValueSite].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueSite]
            INNER JOIN [track].[tblUserDataListValueSite] CT
                ON CT.PK_UserDataListValueSiteGuid = [dbo].[tblUserDataListValueSite].[UserDataListValueSiteGuid]
        WHERE CT.PK_UserDataListValueSiteGuid = @UserDataListValueSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
