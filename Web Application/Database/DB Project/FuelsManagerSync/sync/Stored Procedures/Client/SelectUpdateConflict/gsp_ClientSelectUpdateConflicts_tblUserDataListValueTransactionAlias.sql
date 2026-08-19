-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblUserDataListValueTransactionAlias
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblUserDataListValueTransactionAlias]
@UserDataListValueTransactionAliasGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblUserDataListValueTransactionAlias].[UserDataListValueTransactionAliasGuid],[dbo].[tblUserDataListValueTransactionAlias].[UserDataFieldTransactionAliasGuid],[dbo].[tblUserDataListValueTransactionAlias].[Value],[dbo].[tblUserDataListValueTransactionAlias].[CreatedDate],[dbo].[tblUserDataListValueTransactionAlias].[CreatedBy],[dbo].[tblUserDataListValueTransactionAlias].[UpdatedDate],[dbo].[tblUserDataListValueTransactionAlias].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblUserDataListValueTransactionAlias]
            INNER JOIN [track].[tblUserDataListValueTransactionAlias] CT
                ON CT.PK_UserDataListValueTransactionAliasGuid = [dbo].[tblUserDataListValueTransactionAlias].[UserDataListValueTransactionAliasGuid]
        WHERE CT.PK_UserDataListValueTransactionAliasGuid = @UserDataListValueTransactionAliasGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
