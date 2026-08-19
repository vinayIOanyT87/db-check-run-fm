-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblNotes
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblNotes]
@NoteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblNotes].[Note],[dbo].[tblNotes].[CreatedDate],[dbo].[tblNotes].[CreatedBy],[dbo].[tblNotes].[UpdatedDate],[dbo].[tblNotes].[UpdatedBy],[dbo].[tblNotes].[NoteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblNotes]
            INNER JOIN [track].[tblNotes] CT
                ON CT.PK_NoteGuid = [dbo].[tblNotes].[NoteGuid]
        WHERE CT.PK_NoteGuid = @NoteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
