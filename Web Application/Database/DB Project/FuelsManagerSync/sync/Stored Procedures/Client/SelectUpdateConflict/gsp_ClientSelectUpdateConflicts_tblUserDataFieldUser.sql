-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldUser
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataFieldUser]
@UserDataFieldUserGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldUser].[UserDataFieldUserGuid],[dbo].[tblUserDataFieldUser].[TransactionAliasGuid],[dbo].[tblUserDataFieldUser].[SiteGuid],[dbo].[tblUserDataFieldUser].[Number],[dbo].[tblUserDataFieldUser].[DisplayOrder],[dbo].[tblUserDataFieldUser].[DisplayName],[dbo].[tblUserDataFieldUser].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldUser].[Required],[dbo].[tblUserDataFieldUser].[UserGroupGuid],[dbo].[tblUserDataFieldUser].[CreatedDate],[dbo].[tblUserDataFieldUser].[CreatedBy],[dbo].[tblUserDataFieldUser].[UpdatedDate],[dbo].[tblUserDataFieldUser].[UpdatedBy],[dbo].[tblUserDataFieldUser].[DispatchField],[dbo].[tblUserDataFieldUser].[ClearOnNew],[dbo].[tblUserDataFieldUser].[ReadOnly],[dbo].[tblUserDataFieldUser].[Visibility],[dbo].[tblUserDataFieldUser].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldUser]
            INNER JOIN [track].[tblUserDataFieldUser] CT
                ON CT.PK_UserDataFieldUserGuid = [dbo].[tblUserDataFieldUser].[UserDataFieldUserGuid]
        WHERE CT.PK_UserDataFieldUserGuid = @UserDataFieldUserGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
