-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToFootNoteShipTo
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblApplicationStringToFootNoteShipTo]
@ApplicationStringToFootNoteShipToGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid],[map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipTo].[CompanyGuid],[map].[tblApplicationStringToFootNoteShipTo].[Sequence],[map].[tblApplicationStringToFootNoteShipTo].[CreatedDate],[map].[tblApplicationStringToFootNoteShipTo].[CreatedBy],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedDate],[map].[tblApplicationStringToFootNoteShipTo].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToFootNoteShipTo]
            INNER JOIN [track].[tblApplicationStringToFootNoteShipTo] CT
                ON CT.PK_ApplicationStringToFootNoteShipToGuid = [map].[tblApplicationStringToFootNoteShipTo].[ApplicationStringToFootNoteShipToGuid]
        WHERE CT.PK_ApplicationStringToFootNoteShipToGuid = @ApplicationStringToFootNoteShipToGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
