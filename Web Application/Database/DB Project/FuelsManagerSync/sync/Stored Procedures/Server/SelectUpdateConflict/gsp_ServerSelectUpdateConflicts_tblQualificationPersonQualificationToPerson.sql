-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonQualificationToPerson
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationPersonQualificationToPerson]
@QualificationPersonQualificationToPersonGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonQualificationToPerson].[QualificationPersonQualificationToPersonGuid],[map].[tblQualificationPersonQualificationToPerson].[QualificationGuid],[map].[tblQualificationPersonQualificationToPerson].[PersonnelGuid],[map].[tblQualificationPersonQualificationToPerson].[Sequence],[map].[tblQualificationPersonQualificationToPerson].[Instructor],[map].[tblQualificationPersonQualificationToPerson].[DateCompleted],[map].[tblQualificationPersonQualificationToPerson].[DateDue],[map].[tblQualificationPersonQualificationToPerson].[ExpirationDate],[map].[tblQualificationPersonQualificationToPerson].[ID],[map].[tblQualificationPersonQualificationToPerson].[Rating],[map].[tblQualificationPersonQualificationToPerson].[HistoricalRecord],[map].[tblQualificationPersonQualificationToPerson].[CreatedDate],[map].[tblQualificationPersonQualificationToPerson].[CreatedBy],[map].[tblQualificationPersonQualificationToPerson].[UpdatedDate],[map].[tblQualificationPersonQualificationToPerson].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonQualificationToPerson]
            INNER JOIN [track].[tblQualificationPersonQualificationToPerson] CT
                ON CT.PK_QualificationPersonQualificationToPersonGuid = [map].[tblQualificationPersonQualificationToPerson].[QualificationPersonQualificationToPersonGuid]
        WHERE CT.PK_QualificationPersonQualificationToPersonGuid = @QualificationPersonQualificationToPersonGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
