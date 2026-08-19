-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationEquipmentTestAndInspectionToStation
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationEquipmentTestAndInspectionToStation]
@QualificationEquipmentTestAndInspectionToStationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationEquipmentTestAndInspectionToStation].[QualificationEquipmentTestAndInspectionToStationGuid],[map].[tblQualificationEquipmentTestAndInspectionToStation].[QualificationGuid],[map].[tblQualificationEquipmentTestAndInspectionToStation].[StationGuid],[map].[tblQualificationEquipmentTestAndInspectionToStation].[Sequence],[map].[tblQualificationEquipmentTestAndInspectionToStation].[Instructor],[map].[tblQualificationEquipmentTestAndInspectionToStation].[DateCompleted],[map].[tblQualificationEquipmentTestAndInspectionToStation].[DateDue],[map].[tblQualificationEquipmentTestAndInspectionToStation].[ExpirationDate],[map].[tblQualificationEquipmentTestAndInspectionToStation].[ID],[map].[tblQualificationEquipmentTestAndInspectionToStation].[Rating],[map].[tblQualificationEquipmentTestAndInspectionToStation].[HistoricalRecord],[map].[tblQualificationEquipmentTestAndInspectionToStation].[CreatedDate],[map].[tblQualificationEquipmentTestAndInspectionToStation].[CreatedBy],[map].[tblQualificationEquipmentTestAndInspectionToStation].[UpdatedDate],[map].[tblQualificationEquipmentTestAndInspectionToStation].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationEquipmentTestAndInspectionToStation]
            INNER JOIN [track].[tblQualificationEquipmentTestAndInspectionToStation] CT
                ON CT.PK_QualificationEquipmentTestAndInspectionToStationGuid = [map].[tblQualificationEquipmentTestAndInspectionToStation].[QualificationEquipmentTestAndInspectionToStationGuid]
        WHERE CT.PK_QualificationEquipmentTestAndInspectionToStationGuid = @QualificationEquipmentTestAndInspectionToStationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
