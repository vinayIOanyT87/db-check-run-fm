-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldSite]
@UserDataFieldSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldSite].[UserDataFieldSiteGuid],[dbo].[tblUserDataFieldSite].[TransactionAliasGuid],[dbo].[tblUserDataFieldSite].[SiteGuid],[dbo].[tblUserDataFieldSite].[Number],[dbo].[tblUserDataFieldSite].[DisplayOrder],[dbo].[tblUserDataFieldSite].[DisplayName],[dbo].[tblUserDataFieldSite].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldSite].[Required],[dbo].[tblUserDataFieldSite].[UserGroupGuid],[dbo].[tblUserDataFieldSite].[CreatedDate],[dbo].[tblUserDataFieldSite].[CreatedBy],[dbo].[tblUserDataFieldSite].[UpdatedDate],[dbo].[tblUserDataFieldSite].[UpdatedBy],[dbo].[tblUserDataFieldSite].[DispatchField],[dbo].[tblUserDataFieldSite].[ClearOnNew],[dbo].[tblUserDataFieldSite].[ReadOnly],[dbo].[tblUserDataFieldSite].[Visibility],[dbo].[tblUserDataFieldSite].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldSite]
            INNER JOIN [track].[tblUserDataFieldSite] CT
                ON CT.PK_UserDataFieldSiteGuid = [dbo].[tblUserDataFieldSite].[UserDataFieldSiteGuid]
        WHERE CT.PK_UserDataFieldSiteGuid = @UserDataFieldSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
