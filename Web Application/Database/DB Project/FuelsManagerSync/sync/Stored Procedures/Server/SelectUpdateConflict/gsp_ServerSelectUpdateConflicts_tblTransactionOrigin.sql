-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblTransactionOrigin
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionOrigin]
@TransactionOriginIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblTransactionOrigin].[TransactionOriginIndex],[lookup].[tblTransactionOrigin].[TransactionOriginCode],[lookup].[tblTransactionOrigin].[TransactionOriginName],[lookup].[tblTransactionOrigin].[TransactionOriginGuid],[lookup].[tblTransactionOrigin].[CreatedDate],[lookup].[tblTransactionOrigin].[CreatedBy],[lookup].[tblTransactionOrigin].[UpdatedDate],[lookup].[tblTransactionOrigin].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblTransactionOrigin]
            INNER JOIN [track].[tblTransactionOrigin] CT
                ON CT.PK_TransactionOriginIndex = [lookup].[tblTransactionOrigin].[TransactionOriginIndex]
        WHERE CT.PK_TransactionOriginIndex = @TransactionOriginIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
