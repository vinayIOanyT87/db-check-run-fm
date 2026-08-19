-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblLedgerAggregateColumnToTransactionAlias
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblLedgerAggregateColumnToTransactionAlias]
@LedgerAggregateColumnToTransactionAliasGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblLedgerAggregateColumnToTransactionAlias].[LedgerAggregateColumnToTransactionAliasGuid],[map].[tblLedgerAggregateColumnToTransactionAlias].[LedgerAggregateColumnGuid],[map].[tblLedgerAggregateColumnToTransactionAlias].[TransactionAliasGuid],[map].[tblLedgerAggregateColumnToTransactionAlias].[Symbol],[map].[tblLedgerAggregateColumnToTransactionAlias].[CreatedDate],[map].[tblLedgerAggregateColumnToTransactionAlias].[CreatedBy],[map].[tblLedgerAggregateColumnToTransactionAlias].[UpdatedDate],[map].[tblLedgerAggregateColumnToTransactionAlias].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblLedgerAggregateColumnToTransactionAlias]
            INNER JOIN [track].[tblLedgerAggregateColumnToTransactionAlias] CT
                ON CT.PK_LedgerAggregateColumnToTransactionAliasGuid = [map].[tblLedgerAggregateColumnToTransactionAlias].[LedgerAggregateColumnToTransactionAliasGuid]
        WHERE CT.PK_LedgerAggregateColumnToTransactionAliasGuid = @LedgerAggregateColumnToTransactionAliasGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
