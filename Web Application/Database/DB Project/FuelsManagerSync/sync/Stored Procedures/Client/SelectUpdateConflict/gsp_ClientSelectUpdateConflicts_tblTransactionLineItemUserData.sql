-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionLineItemUserData
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionLineItemUserData]
@TransactionLineItemUserDataGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionLineItemUserData].[UserData1],[dbo].[tblTransactionLineItemUserData].[UserData2],[dbo].[tblTransactionLineItemUserData].[UserData3],[dbo].[tblTransactionLineItemUserData].[UserData4],[dbo].[tblTransactionLineItemUserData].[UserData5],[dbo].[tblTransactionLineItemUserData].[UserData6],[dbo].[tblTransactionLineItemUserData].[UserData7],[dbo].[tblTransactionLineItemUserData].[UserData8],[dbo].[tblTransactionLineItemUserData].[UserData9],[dbo].[tblTransactionLineItemUserData].[UserData10],[dbo].[tblTransactionLineItemUserData].[UserData11],[dbo].[tblTransactionLineItemUserData].[UserData12],[dbo].[tblTransactionLineItemUserData].[UserData13],[dbo].[tblTransactionLineItemUserData].[UserData14],[dbo].[tblTransactionLineItemUserData].[UserData15],[dbo].[tblTransactionLineItemUserData].[UserData16],[dbo].[tblTransactionLineItemUserData].[UserData17],[dbo].[tblTransactionLineItemUserData].[UserData18],[dbo].[tblTransactionLineItemUserData].[UserData19],[dbo].[tblTransactionLineItemUserData].[UserData20],[dbo].[tblTransactionLineItemUserData].[UserData21],[dbo].[tblTransactionLineItemUserData].[UserData22],[dbo].[tblTransactionLineItemUserData].[UserData23],[dbo].[tblTransactionLineItemUserData].[UserData24],[dbo].[tblTransactionLineItemUserData].[CreatedBy],[dbo].[tblTransactionLineItemUserData].[CreatedDate],[dbo].[tblTransactionLineItemUserData].[UpdatedBy],[dbo].[tblTransactionLineItemUserData].[UpdatedDate],[dbo].[tblTransactionLineItemUserData].[TransactionLineItemUserDataGuid],[dbo].[tblTransactionLineItemUserData].[TransactionLineItemGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionLineItemUserData]
            INNER JOIN [track].[tblTransactionLineItemUserData] CT
                ON CT.PK_TransactionLineItemUserDataGuid = [dbo].[tblTransactionLineItemUserData].[TransactionLineItemUserDataGuid]
        WHERE CT.PK_TransactionLineItemUserDataGuid = @TransactionLineItemUserDataGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
