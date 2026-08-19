-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueTransactionAliasLineItem
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblUserDataListValueTransactionAliasLineItem]
@UserDataListValueTransactionAliasLineItemGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueTransactionAliasLineItem].[UserDataListValueTransactionAliasLineItemGuid],[dbo].[tblUserDataListValueTransactionAliasLineItem].[UserDataFieldTransactionAliasLineItemGuid],[dbo].[tblUserDataListValueTransactionAliasLineItem].[Value],[dbo].[tblUserDataListValueTransactionAliasLineItem].[CreatedDate],[dbo].[tblUserDataListValueTransactionAliasLineItem].[CreatedBy],[dbo].[tblUserDataListValueTransactionAliasLineItem].[UpdatedDate],[dbo].[tblUserDataListValueTransactionAliasLineItem].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueTransactionAliasLineItem]
            INNER JOIN [track].[tblUserDataListValueTransactionAliasLineItem] CT
                ON CT.PK_UserDataListValueTransactionAliasLineItemGuid = [dbo].[tblUserDataListValueTransactionAliasLineItem].[UserDataListValueTransactionAliasLineItemGuid]
        WHERE CT.PK_UserDataListValueTransactionAliasLineItemGuid = @UserDataListValueTransactionAliasLineItemGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
