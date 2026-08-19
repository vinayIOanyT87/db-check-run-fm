-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToFootNoteShipper
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblApplicationStringToFootNoteShipper]
@ApplicationStringToFootNoteShipperGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToFootNoteShipper].[ApplicationStringToFootNoteShipperGuid],[map].[tblApplicationStringToFootNoteShipper].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipper].[CompanyGuid],[map].[tblApplicationStringToFootNoteShipper].[Sequence],[map].[tblApplicationStringToFootNoteShipper].[CreatedDate],[map].[tblApplicationStringToFootNoteShipper].[CreatedBy],[map].[tblApplicationStringToFootNoteShipper].[UpdatedDate],[map].[tblApplicationStringToFootNoteShipper].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToFootNoteShipper]
            INNER JOIN [track].[tblApplicationStringToFootNoteShipper] CT
                ON CT.PK_ApplicationStringToFootNoteShipperGuid = [map].[tblApplicationStringToFootNoteShipper].[ApplicationStringToFootNoteShipperGuid]
        WHERE CT.PK_ApplicationStringToFootNoteShipperGuid = @ApplicationStringToFootNoteShipperGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
