-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblExcise
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblExcise]
@ExciseGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblExcise].[ExciseRate],[dbo].[tblExcise].[ExciseCode],[dbo].[tblExcise].[ExciseDate],[dbo].[tblExcise].[CreatedBy],[dbo].[tblExcise].[CreatedDate],[dbo].[tblExcise].[UpdatedBy],[dbo].[tblExcise].[UpdatedDate],[dbo].[tblExcise].[ExciseGuid],[dbo].[tblExcise].[ProductGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblExcise]
            INNER JOIN [track].[tblExcise] CT
                ON CT.PK_ExciseGuid = [dbo].[tblExcise].[ExciseGuid]
        WHERE CT.PK_ExciseGuid = @ExciseGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
