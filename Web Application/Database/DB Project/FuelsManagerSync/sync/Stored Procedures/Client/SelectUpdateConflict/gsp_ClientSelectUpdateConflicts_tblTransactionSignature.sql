-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionSignature
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionSignature]
@TransactionSignatureGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionSignature].[Signature],[dbo].[tblTransactionSignature].[CreatedBy],[dbo].[tblTransactionSignature].[CreatedDate],[dbo].[tblTransactionSignature].[UpdatedBy],[dbo].[tblTransactionSignature].[UpdatedDate],[dbo].[tblTransactionSignature].[TransactionSignatureGuid],[dbo].[tblTransactionSignature].[TransactionGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionSignature]
            INNER JOIN [track].[tblTransactionSignature] CT
                ON CT.PK_TransactionSignatureGuid = [dbo].[tblTransactionSignature].[TransactionSignatureGuid]
        WHERE CT.PK_TransactionSignatureGuid = @TransactionSignatureGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
