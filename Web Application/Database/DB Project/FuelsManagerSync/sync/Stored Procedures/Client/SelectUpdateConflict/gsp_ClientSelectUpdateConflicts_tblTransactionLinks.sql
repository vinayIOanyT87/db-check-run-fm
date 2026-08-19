-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionLinks
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionLinks]
@TransactionLinkGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionLinks].[OriginalTransID],[dbo].[tblTransactionLinks].[LinkedTransID],[dbo].[tblTransactionLinks].[Level],[dbo].[tblTransactionLinks].[CreatedBy],[dbo].[tblTransactionLinks].[CreatedDate],[dbo].[tblTransactionLinks].[UpdatedBy],[dbo].[tblTransactionLinks].[UpdatedDate],[dbo].[tblTransactionLinks].[TransactionLinkGuid],[dbo].[tblTransactionLinks].[SiteGuid],[dbo].[tblTransactionLinks].[LinkedTransactionLineItemGuid],[dbo].[tblTransactionLinks].[TransactionLineItemGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionLinks]
            INNER JOIN [track].[tblTransactionLinks] CT
                ON CT.PK_TransactionLinkGuid = [dbo].[tblTransactionLinks].[TransactionLinkGuid]
        WHERE CT.PK_TransactionLinkGuid = @TransactionLinkGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
