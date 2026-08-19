-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationEquipmentTagAndLicenseToEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationEquipmentTagAndLicenseToEquipment]
@QualificationEquipmentTagAndLicenseToEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationEquipmentTagAndLicenseToEquipment].[QualificationEquipmentTagAndLicenseToEquipmentGuid],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[QualificationGuid],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[EquipmentGuid],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[Sequence],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[Instructor],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[DateCompleted],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[DateDue],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[ExpirationDate],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[ID],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[Rating],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[HistoricalRecord],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[CreatedDate],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[CreatedBy],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[UpdatedDate],[map].[tblQualificationEquipmentTagAndLicenseToEquipment].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationEquipmentTagAndLicenseToEquipment]
            INNER JOIN [track].[tblQualificationEquipmentTagAndLicenseToEquipment] CT
                ON CT.PK_QualificationEquipmentTagAndLicenseToEquipmentGuid = [map].[tblQualificationEquipmentTagAndLicenseToEquipment].[QualificationEquipmentTagAndLicenseToEquipmentGuid]
        WHERE CT.PK_QualificationEquipmentTagAndLicenseToEquipmentGuid = @QualificationEquipmentTagAndLicenseToEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
