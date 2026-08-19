-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationEquipmentTagAndLicenseToStation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualificationEquipmentTagAndLicenseToStation]
@QualificationEquipmentTagAndLicenseToStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationEquipmentTagAndLicenseToStation].[QualificationEquipmentTagAndLicenseToStationGuid],[map].[tblQualificationEquipmentTagAndLicenseToStation].[QualificationGuid],[map].[tblQualificationEquipmentTagAndLicenseToStation].[StationGuid],[map].[tblQualificationEquipmentTagAndLicenseToStation].[Sequence],[map].[tblQualificationEquipmentTagAndLicenseToStation].[Instructor],[map].[tblQualificationEquipmentTagAndLicenseToStation].[DateCompleted],[map].[tblQualificationEquipmentTagAndLicenseToStation].[DateDue],[map].[tblQualificationEquipmentTagAndLicenseToStation].[ExpirationDate],[map].[tblQualificationEquipmentTagAndLicenseToStation].[ID],[map].[tblQualificationEquipmentTagAndLicenseToStation].[Rating],[map].[tblQualificationEquipmentTagAndLicenseToStation].[HistoricalRecord],[map].[tblQualificationEquipmentTagAndLicenseToStation].[CreatedDate],[map].[tblQualificationEquipmentTagAndLicenseToStation].[CreatedBy],[map].[tblQualificationEquipmentTagAndLicenseToStation].[UpdatedDate],[map].[tblQualificationEquipmentTagAndLicenseToStation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationEquipmentTagAndLicenseToStation]
            INNER JOIN [track].[tblQualificationEquipmentTagAndLicenseToStation] CT
                ON CT.PK_QualificationEquipmentTagAndLicenseToStationGuid = [map].[tblQualificationEquipmentTagAndLicenseToStation].[QualificationEquipmentTagAndLicenseToStationGuid]
        WHERE CT.PK_QualificationEquipmentTagAndLicenseToStationGuid = @QualificationEquipmentTagAndLicenseToStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
