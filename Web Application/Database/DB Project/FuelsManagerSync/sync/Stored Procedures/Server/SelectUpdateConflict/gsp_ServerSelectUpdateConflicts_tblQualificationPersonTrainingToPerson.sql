-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonTrainingToPerson
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationPersonTrainingToPerson]
@QualificationPersonTrainingToPersonGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonTrainingToPerson].[QualificationPersonTrainingToPersonGuid],[map].[tblQualificationPersonTrainingToPerson].[QualificationGuid],[map].[tblQualificationPersonTrainingToPerson].[PersonnelGuid],[map].[tblQualificationPersonTrainingToPerson].[Sequence],[map].[tblQualificationPersonTrainingToPerson].[Instructor],[map].[tblQualificationPersonTrainingToPerson].[DateCompleted],[map].[tblQualificationPersonTrainingToPerson].[DateDue],[map].[tblQualificationPersonTrainingToPerson].[ExpirationDate],[map].[tblQualificationPersonTrainingToPerson].[ID],[map].[tblQualificationPersonTrainingToPerson].[Rating],[map].[tblQualificationPersonTrainingToPerson].[HistoricalRecord],[map].[tblQualificationPersonTrainingToPerson].[CreatedDate],[map].[tblQualificationPersonTrainingToPerson].[CreatedBy],[map].[tblQualificationPersonTrainingToPerson].[UpdatedDate],[map].[tblQualificationPersonTrainingToPerson].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonTrainingToPerson]
            INNER JOIN [track].[tblQualificationPersonTrainingToPerson] CT
                ON CT.PK_QualificationPersonTrainingToPersonGuid = [map].[tblQualificationPersonTrainingToPerson].[QualificationPersonTrainingToPersonGuid]
        WHERE CT.PK_QualificationPersonTrainingToPersonGuid = @QualificationPersonTrainingToPersonGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
