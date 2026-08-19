-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldTransactionAlias
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldTransactionAlias]
@UserDataFieldTransactionAliasGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid],[dbo].[tblUserDataFieldTransactionAlias].[Number],[dbo].[tblUserDataFieldTransactionAlias].[DisplayOrder],[dbo].[tblUserDataFieldTransactionAlias].[DisplayName],[dbo].[tblUserDataFieldTransactionAlias].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldTransactionAlias].[Required],[dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid],[dbo].[tblUserDataFieldTransactionAlias].[CreatedDate],[dbo].[tblUserDataFieldTransactionAlias].[CreatedBy],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedDate],[dbo].[tblUserDataFieldTransactionAlias].[UpdatedBy],[dbo].[tblUserDataFieldTransactionAlias].[DispatchField],[dbo].[tblUserDataFieldTransactionAlias].[ClearOnNew],[dbo].[tblUserDataFieldTransactionAlias].[ReadOnly],[dbo].[tblUserDataFieldTransactionAlias].[Visibility],[dbo].[tblUserDataFieldTransactionAlias].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldTransactionAlias]
            INNER JOIN [track].[tblUserDataFieldTransactionAlias] CT
                ON CT.PK_UserDataFieldTransactionAliasGuid = [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid]
        WHERE CT.PK_UserDataFieldTransactionAliasGuid = @UserDataFieldTransactionAliasGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
