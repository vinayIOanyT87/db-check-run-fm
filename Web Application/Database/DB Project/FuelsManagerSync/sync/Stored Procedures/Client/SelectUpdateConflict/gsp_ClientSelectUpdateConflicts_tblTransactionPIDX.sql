-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionPIDX
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionPIDX]
@TransactionPIDXGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionPIDX].[AuthorizationNumber],[dbo].[tblTransactionPIDX].[SentFlag],[dbo].[tblTransactionPIDX].[DateSent],[dbo].[tblTransactionPIDX].[CreatedBy],[dbo].[tblTransactionPIDX].[CreatedDate],[dbo].[tblTransactionPIDX].[UpdatedBy],[dbo].[tblTransactionPIDX].[UpdatedDate],[dbo].[tblTransactionPIDX].[BrokenBlend],[dbo].[tblTransactionPIDX].[TransactionPIDXGuid],[dbo].[tblTransactionPIDX].[PIDXProfileGuid],[dbo].[tblTransactionPIDX].[TransactionGuid],[dbo].[tblTransactionPIDX].[CompanyPersonnelToShipToBillToGuid],[dbo].[tblTransactionPIDX].[BOLVersion], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionPIDX]
            INNER JOIN [track].[tblTransactionPIDX] CT
                ON CT.PK_TransactionPIDXGuid = [dbo].[tblTransactionPIDX].[TransactionPIDXGuid]
        WHERE CT.PK_TransactionPIDXGuid = @TransactionPIDXGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
