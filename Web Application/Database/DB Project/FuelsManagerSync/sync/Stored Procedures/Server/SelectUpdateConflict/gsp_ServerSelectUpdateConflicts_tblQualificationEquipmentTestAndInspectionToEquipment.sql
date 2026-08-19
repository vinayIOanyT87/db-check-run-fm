-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationEquipmentTestAndInspectionToEquipment
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationEquipmentTestAndInspectionToEquipment]
@QualificationEquipmentTestAndInspectionToEquipmentGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationEquipmentTestAndInspectionToEquipmentGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[EquipmentGuid],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Sequence],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Instructor],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[DateCompleted],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[DateDue],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[ExpirationDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[ID],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[Rating],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[HistoricalRecord],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[CreatedDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[CreatedBy],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[UpdatedDate],[map].[tblQualificationEquipmentTestAndInspectionToEquipment].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationEquipmentTestAndInspectionToEquipment]
            INNER JOIN [track].[tblQualificationEquipmentTestAndInspectionToEquipment] CT
                ON CT.PK_QualificationEquipmentTestAndInspectionToEquipmentGuid = [map].[tblQualificationEquipmentTestAndInspectionToEquipment].[QualificationEquipmentTestAndInspectionToEquipmentGuid]
        WHERE CT.PK_QualificationEquipmentTestAndInspectionToEquipmentGuid = @QualificationEquipmentTestAndInspectionToEquipmentGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
