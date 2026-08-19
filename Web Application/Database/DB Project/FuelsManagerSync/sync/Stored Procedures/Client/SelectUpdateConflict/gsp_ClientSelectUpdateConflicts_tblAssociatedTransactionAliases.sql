-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblAssociatedTransactionAliases
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAssociatedTransactionAliases]
@AssociatedTransactionAliasGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblAssociatedTransactionAliases].[AssociatedTransactionAliasGuid],[map].[tblAssociatedTransactionAliases].[ParentTransactionAliasGuid],[map].[tblAssociatedTransactionAliases].[ChildTransactionAliasGuid],[map].[tblAssociatedTransactionAliases].[CreatedDate],[map].[tblAssociatedTransactionAliases].[CreatedBy],[map].[tblAssociatedTransactionAliases].[UpdatedDate],[map].[tblAssociatedTransactionAliases].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblAssociatedTransactionAliases]
            INNER JOIN [track].[tblAssociatedTransactionAliases] CT
                ON CT.PK_AssociatedTransactionAliasGuid = [map].[tblAssociatedTransactionAliases].[AssociatedTransactionAliasGuid]
        WHERE CT.PK_AssociatedTransactionAliasGuid = @AssociatedTransactionAliasGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
