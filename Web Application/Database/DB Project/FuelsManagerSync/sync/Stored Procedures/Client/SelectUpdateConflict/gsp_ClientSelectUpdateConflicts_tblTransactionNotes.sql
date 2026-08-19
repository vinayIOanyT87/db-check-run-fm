-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionNotes
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblTransactionNotes]
@TransactionNoteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblTransactionNotes].[Notes],[dbo].[tblTransactionNotes].[CreatedBy],[dbo].[tblTransactionNotes].[CreatedDate],[dbo].[tblTransactionNotes].[UpdatedBy],[dbo].[tblTransactionNotes].[UpdatedDate],[dbo].[tblTransactionNotes].[AdditionalInformation],[dbo].[tblTransactionNotes].[TransactionNoteGuid],[dbo].[tblTransactionNotes].[TransactionGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblTransactionNotes]
            INNER JOIN [track].[tblTransactionNotes] CT
                ON CT.PK_TransactionNoteGuid = [dbo].[tblTransactionNotes].[TransactionNoteGuid]
        WHERE CT.PK_TransactionNoteGuid = @TransactionNoteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
