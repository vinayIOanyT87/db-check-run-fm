-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonLicenseToPerson
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualificationPersonLicenseToPerson]
@QualificationPersonLicenseToPersonGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonLicenseToPerson].[QualificationPersonLicenseToPersonGuid],[map].[tblQualificationPersonLicenseToPerson].[QualificationGuid],[map].[tblQualificationPersonLicenseToPerson].[PersonnelGuid],[map].[tblQualificationPersonLicenseToPerson].[Sequence],[map].[tblQualificationPersonLicenseToPerson].[Instructor],[map].[tblQualificationPersonLicenseToPerson].[DateCompleted],[map].[tblQualificationPersonLicenseToPerson].[DateDue],[map].[tblQualificationPersonLicenseToPerson].[ExpirationDate],[map].[tblQualificationPersonLicenseToPerson].[ID],[map].[tblQualificationPersonLicenseToPerson].[Rating],[map].[tblQualificationPersonLicenseToPerson].[HistoricalRecord],[map].[tblQualificationPersonLicenseToPerson].[CreatedDate],[map].[tblQualificationPersonLicenseToPerson].[CreatedBy],[map].[tblQualificationPersonLicenseToPerson].[UpdatedDate],[map].[tblQualificationPersonLicenseToPerson].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonLicenseToPerson]
            INNER JOIN [track].[tblQualificationPersonLicenseToPerson] CT
                ON CT.PK_QualificationPersonLicenseToPersonGuid = [map].[tblQualificationPersonLicenseToPerson].[QualificationPersonLicenseToPersonGuid]
        WHERE CT.PK_QualificationPersonLicenseToPersonGuid = @QualificationPersonLicenseToPersonGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
