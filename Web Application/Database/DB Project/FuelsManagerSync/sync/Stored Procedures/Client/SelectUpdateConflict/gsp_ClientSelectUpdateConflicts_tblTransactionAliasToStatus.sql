-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblTransactionAliasToStatus
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionAliasToStatus]
@TransactionAliasToStatusGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblTransactionAliasToStatus].[TransactionAliasToStatusGuid],[map].[tblTransactionAliasToStatus].[TransactionAliasGuid],[map].[tblTransactionAliasToStatus].[LookupTransactionStatusIndex],[map].[tblTransactionAliasToStatus].[CreatedDate],[map].[tblTransactionAliasToStatus].[CreatedBy],[map].[tblTransactionAliasToStatus].[UpdatedDate],[map].[tblTransactionAliasToStatus].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblTransactionAliasToStatus]
            INNER JOIN [track].[tblTransactionAliasToStatus] CT
                ON CT.PK_TransactionAliasToStatusGuid = [map].[tblTransactionAliasToStatus].[TransactionAliasToStatusGuid]
        WHERE CT.PK_TransactionAliasToStatusGuid = @TransactionAliasToStatusGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
