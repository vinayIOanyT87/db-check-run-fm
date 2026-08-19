-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToFootNoteShipToState
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblApplicationStringToFootNoteShipToState]
@ApplicationStringToFootNoteShipToStateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToFootNoteShipToState].[ApplicationStringToFootNoteShipToStateGuid],[map].[tblApplicationStringToFootNoteShipToState].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipToState].[AssignedToApplicationStringGuid],[map].[tblApplicationStringToFootNoteShipToState].[Sequence],[map].[tblApplicationStringToFootNoteShipToState].[CreatedDate],[map].[tblApplicationStringToFootNoteShipToState].[CreatedBy],[map].[tblApplicationStringToFootNoteShipToState].[UpdatedDate],[map].[tblApplicationStringToFootNoteShipToState].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToFootNoteShipToState]
            INNER JOIN [track].[tblApplicationStringToFootNoteShipToState] CT
                ON CT.PK_ApplicationStringToFootNoteShipToStateGuid = [map].[tblApplicationStringToFootNoteShipToState].[ApplicationStringToFootNoteShipToStateGuid]
        WHERE CT.PK_ApplicationStringToFootNoteShipToStateGuid = @ApplicationStringToFootNoteShipToStateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
