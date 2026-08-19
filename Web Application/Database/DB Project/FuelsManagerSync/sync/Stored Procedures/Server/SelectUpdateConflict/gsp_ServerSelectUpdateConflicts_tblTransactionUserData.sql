-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionUserData
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblTransactionUserData]
@TransactionUserDataGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionUserData].[UserData1],[dbo].[tblTransactionUserData].[UserData2],[dbo].[tblTransactionUserData].[UserData3],[dbo].[tblTransactionUserData].[UserData4],[dbo].[tblTransactionUserData].[UserData5],[dbo].[tblTransactionUserData].[UserData6],[dbo].[tblTransactionUserData].[UserData7],[dbo].[tblTransactionUserData].[UserData8],[dbo].[tblTransactionUserData].[UserData9],[dbo].[tblTransactionUserData].[UserData10],[dbo].[tblTransactionUserData].[UserData11],[dbo].[tblTransactionUserData].[UserData12],[dbo].[tblTransactionUserData].[UserData13],[dbo].[tblTransactionUserData].[UserData14],[dbo].[tblTransactionUserData].[UserData15],[dbo].[tblTransactionUserData].[UserData16],[dbo].[tblTransactionUserData].[UserData17],[dbo].[tblTransactionUserData].[UserData18],[dbo].[tblTransactionUserData].[UserData19],[dbo].[tblTransactionUserData].[UserData20],[dbo].[tblTransactionUserData].[UserData21],[dbo].[tblTransactionUserData].[UserData22],[dbo].[tblTransactionUserData].[UserData23],[dbo].[tblTransactionUserData].[UserData24],[dbo].[tblTransactionUserData].[CreatedBy],[dbo].[tblTransactionUserData].[CreatedDate],[dbo].[tblTransactionUserData].[UpdatedBy],[dbo].[tblTransactionUserData].[UpdatedDate],[dbo].[tblTransactionUserData].[TransactionUserDataGuid],[dbo].[tblTransactionUserData].[TransactionGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionUserData]
            INNER JOIN [track].[tblTransactionUserData] CT
                ON CT.PK_TransactionUserDataGuid = [dbo].[tblTransactionUserData].[TransactionUserDataGuid]
        WHERE CT.PK_TransactionUserDataGuid = @TransactionUserDataGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
