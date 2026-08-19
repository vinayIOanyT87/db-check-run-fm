-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTransactionStatus
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionStatus]
@TransactionStatusIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblTransactionStatus].[TransactionStatusIndex],[lookup].[tblTransactionStatus].[TransactionStatusCode],[lookup].[tblTransactionStatus].[TransactionStatusName],[lookup].[tblTransactionStatus].[TransactionStatusGuid],[lookup].[tblTransactionStatus].[CreatedDate],[lookup].[tblTransactionStatus].[CreatedBy],[lookup].[tblTransactionStatus].[UpdatedDate],[lookup].[tblTransactionStatus].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTransactionStatus]
            INNER JOIN [track].[tblTransactionStatus] CT
                ON CT.PK_TransactionStatusIndex = [lookup].[tblTransactionStatus].[TransactionStatusIndex]
        WHERE CT.PK_TransactionStatusIndex = @TransactionStatusIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
