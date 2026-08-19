-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonTrainingToStation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualificationPersonTrainingToStation]
@QualificationPersonTrainingToStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonTrainingToStation].[QualificationPersonTrainingToStationGuid],[map].[tblQualificationPersonTrainingToStation].[QualificationGuid],[map].[tblQualificationPersonTrainingToStation].[StationGuid],[map].[tblQualificationPersonTrainingToStation].[Sequence],[map].[tblQualificationPersonTrainingToStation].[Instructor],[map].[tblQualificationPersonTrainingToStation].[DateCompleted],[map].[tblQualificationPersonTrainingToStation].[DateDue],[map].[tblQualificationPersonTrainingToStation].[ExpirationDate],[map].[tblQualificationPersonTrainingToStation].[ID],[map].[tblQualificationPersonTrainingToStation].[Rating],[map].[tblQualificationPersonTrainingToStation].[HistoricalRecord],[map].[tblQualificationPersonTrainingToStation].[CreatedDate],[map].[tblQualificationPersonTrainingToStation].[CreatedBy],[map].[tblQualificationPersonTrainingToStation].[UpdatedDate],[map].[tblQualificationPersonTrainingToStation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonTrainingToStation]
            INNER JOIN [track].[tblQualificationPersonTrainingToStation] CT
                ON CT.PK_QualificationPersonTrainingToStationGuid = [map].[tblQualificationPersonTrainingToStation].[QualificationPersonTrainingToStationGuid]
        WHERE CT.PK_QualificationPersonTrainingToStationGuid = @QualificationPersonTrainingToStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
