-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationCompanyCertificateAndPermitToCompany
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualificationCompanyCertificateAndPermitToCompany]
@QualificationCompanyCertificateAndPermitToCompanyGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationCompanyCertificateAndPermitToCompanyGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CompanyGuid],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Sequence],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Instructor],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[DateCompleted],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[DateDue],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[ExpirationDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[ID],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[Rating],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[HistoricalRecord],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CreatedDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[CreatedBy],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[UpdatedDate],[map].[tblQualificationCompanyCertificateAndPermitToCompany].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationCompanyCertificateAndPermitToCompany]
            INNER JOIN [track].[tblQualificationCompanyCertificateAndPermitToCompany] CT
                ON CT.PK_QualificationCompanyCertificateAndPermitToCompanyGuid = [map].[tblQualificationCompanyCertificateAndPermitToCompany].[QualificationCompanyCertificateAndPermitToCompanyGuid]
        WHERE CT.PK_QualificationCompanyCertificateAndPermitToCompanyGuid = @QualificationCompanyCertificateAndPermitToCompanyGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
