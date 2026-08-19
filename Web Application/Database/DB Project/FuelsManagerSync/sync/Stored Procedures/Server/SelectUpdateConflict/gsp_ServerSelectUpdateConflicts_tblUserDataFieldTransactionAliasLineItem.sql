-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataFieldTransactionAliasLineItem
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataFieldTransactionAliasLineItem]
@UserDataFieldTransactionAliasLineItemGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserDataFieldTransactionAliasLineItemGuid],[dbo].[tblUserDataFieldTransactionAliasLineItem].[TransactionAliasGuid],[dbo].[tblUserDataFieldTransactionAliasLineItem].[SiteGuid],[dbo].[tblUserDataFieldTransactionAliasLineItem].[Number],[dbo].[tblUserDataFieldTransactionAliasLineItem].[DisplayOrder],[dbo].[tblUserDataFieldTransactionAliasLineItem].[DisplayName],[dbo].[tblUserDataFieldTransactionAliasLineItem].[LookupUserDataTypeIndex],[dbo].[tblUserDataFieldTransactionAliasLineItem].[Required],[dbo].[tblUserDataFieldTransactionAliasLineItem].[UserGroupGuid],[dbo].[tblUserDataFieldTransactionAliasLineItem].[CreatedDate],[dbo].[tblUserDataFieldTransactionAliasLineItem].[CreatedBy],[dbo].[tblUserDataFieldTransactionAliasLineItem].[UpdatedDate],[dbo].[tblUserDataFieldTransactionAliasLineItem].[UpdatedBy],[dbo].[tblUserDataFieldTransactionAliasLineItem].[DispatchField],[dbo].[tblUserDataFieldTransactionAliasLineItem].[ClearOnNew],[dbo].[tblUserDataFieldTransactionAliasLineItem].[ReadOnly],[dbo].[tblUserDataFieldTransactionAliasLineItem].[Visibility],[dbo].[tblUserDataFieldTransactionAliasLineItem].[DefaultValue], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]
            INNER JOIN [track].[tblUserDataFieldTransactionAliasLineItem] CT
                ON CT.PK_UserDataFieldTransactionAliasLineItemGuid = [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserDataFieldTransactionAliasLineItemGuid]
        WHERE CT.PK_UserDataFieldTransactionAliasLineItemGuid = @UserDataFieldTransactionAliasLineItemGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
