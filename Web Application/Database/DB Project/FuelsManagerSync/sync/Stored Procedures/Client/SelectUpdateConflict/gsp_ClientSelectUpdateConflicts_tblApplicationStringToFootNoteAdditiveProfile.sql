-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblApplicationStringToFootNoteAdditiveProfile
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblApplicationStringToFootNoteAdditiveProfile]
@ApplicationStringToFootNoteAdditiveProfileGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblApplicationStringToFootNoteAdditiveProfile].[ApplicationStringToFootNoteAdditiveProfileGuid],[map].[tblApplicationStringToFootNoteAdditiveProfile].[ApplicationStringGuid],[map].[tblApplicationStringToFootNoteAdditiveProfile].[AdditiveProfileGuid],[map].[tblApplicationStringToFootNoteAdditiveProfile].[Sequence],[map].[tblApplicationStringToFootNoteAdditiveProfile].[CreatedDate],[map].[tblApplicationStringToFootNoteAdditiveProfile].[CreatedBy],[map].[tblApplicationStringToFootNoteAdditiveProfile].[UpdatedDate],[map].[tblApplicationStringToFootNoteAdditiveProfile].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblApplicationStringToFootNoteAdditiveProfile]
            INNER JOIN [track].[tblApplicationStringToFootNoteAdditiveProfile] CT
                ON CT.PK_ApplicationStringToFootNoteAdditiveProfileGuid = [map].[tblApplicationStringToFootNoteAdditiveProfile].[ApplicationStringToFootNoteAdditiveProfileGuid]
        WHERE CT.PK_ApplicationStringToFootNoteAdditiveProfileGuid = @ApplicationStringToFootNoteAdditiveProfileGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
