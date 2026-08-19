-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonLicenseToStation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualificationPersonLicenseToStation]
@QualificationPersonLicenseToStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonLicenseToStation].[QualificationPersonLicenseToStationGuid],[map].[tblQualificationPersonLicenseToStation].[QualificationGuid],[map].[tblQualificationPersonLicenseToStation].[StationGuid],[map].[tblQualificationPersonLicenseToStation].[Sequence],[map].[tblQualificationPersonLicenseToStation].[Instructor],[map].[tblQualificationPersonLicenseToStation].[DateCompleted],[map].[tblQualificationPersonLicenseToStation].[DateDue],[map].[tblQualificationPersonLicenseToStation].[ExpirationDate],[map].[tblQualificationPersonLicenseToStation].[ID],[map].[tblQualificationPersonLicenseToStation].[Rating],[map].[tblQualificationPersonLicenseToStation].[HistoricalRecord],[map].[tblQualificationPersonLicenseToStation].[CreatedDate],[map].[tblQualificationPersonLicenseToStation].[CreatedBy],[map].[tblQualificationPersonLicenseToStation].[UpdatedDate],[map].[tblQualificationPersonLicenseToStation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonLicenseToStation]
            INNER JOIN [track].[tblQualificationPersonLicenseToStation] CT
                ON CT.PK_QualificationPersonLicenseToStationGuid = [map].[tblQualificationPersonLicenseToStation].[QualificationPersonLicenseToStationGuid]
        WHERE CT.PK_QualificationPersonLicenseToStationGuid = @QualificationPersonLicenseToStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
