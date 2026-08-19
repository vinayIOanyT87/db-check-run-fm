-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToFootNoteProduct
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblApplicationStringToFootNoteProduct]
@ApplicationStringToFootNoteProductGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToFootNoteProduct].[ApplicationStringToFootNoteProductGuid],[map].[tblApplicationStringToFootNoteProduct].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteProduct].[ProductGuid],[map].[tblApplicationStringToFootNoteProduct].[Sequence],[map].[tblApplicationStringToFootNoteProduct].[CreatedDate],[map].[tblApplicationStringToFootNoteProduct].[CreatedBy],[map].[tblApplicationStringToFootNoteProduct].[UpdatedDate],[map].[tblApplicationStringToFootNoteProduct].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToFootNoteProduct]
            INNER JOIN [track].[tblApplicationStringToFootNoteProduct] CT
                ON CT.PK_ApplicationStringToFootNoteProductGuid = [map].[tblApplicationStringToFootNoteProduct].[ApplicationStringToFootNoteProductGuid]
        WHERE CT.PK_ApplicationStringToFootNoteProductGuid = @ApplicationStringToFootNoteProductGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
