-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblGroupToTransactionAlias
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblGroupToTransactionAlias]
@GroupToTransactionAliasGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblGroupToTransactionAlias].[GroupToTransactionAliasGuid],[map].[tblGroupToTransactionAlias].[GroupGuid],[map].[tblGroupToTransactionAlias].[TransactionAliasGuid],[map].[tblGroupToTransactionAlias].[LookupRightIndex],[map].[tblGroupToTransactionAlias].[CreatedDate],[map].[tblGroupToTransactionAlias].[CreatedBy],[map].[tblGroupToTransactionAlias].[UpdatedDate],[map].[tblGroupToTransactionAlias].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblGroupToTransactionAlias]
            INNER JOIN [track].[tblGroupToTransactionAlias] CT
                ON CT.PK_GroupToTransactionAliasGuid = [map].[tblGroupToTransactionAlias].[GroupToTransactionAliasGuid]
        WHERE CT.PK_GroupToTransactionAliasGuid = @GroupToTransactionAliasGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
