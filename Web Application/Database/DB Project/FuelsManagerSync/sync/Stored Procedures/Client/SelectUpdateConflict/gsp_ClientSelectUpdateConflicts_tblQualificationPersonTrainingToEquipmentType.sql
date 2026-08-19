-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonTrainingToEquipmentType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblQualificationPersonTrainingToEquipmentType]
@QualificationPersonTrainingToEquipmentTypeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonTrainingToEquipmentType].[QualificationPersonTrainingToEquipmentTypeGuid],[map].[tblQualificationPersonTrainingToEquipmentType].[QualificationGuid],[map].[tblQualificationPersonTrainingToEquipmentType].[EquipmentTypeGuid],[map].[tblQualificationPersonTrainingToEquipmentType].[Sequence],[map].[tblQualificationPersonTrainingToEquipmentType].[Instructor],[map].[tblQualificationPersonTrainingToEquipmentType].[DateCompleted],[map].[tblQualificationPersonTrainingToEquipmentType].[DateDue],[map].[tblQualificationPersonTrainingToEquipmentType].[ExpirationDate],[map].[tblQualificationPersonTrainingToEquipmentType].[ID],[map].[tblQualificationPersonTrainingToEquipmentType].[Rating],[map].[tblQualificationPersonTrainingToEquipmentType].[HistoricalRecord],[map].[tblQualificationPersonTrainingToEquipmentType].[CreatedDate],[map].[tblQualificationPersonTrainingToEquipmentType].[CreatedBy],[map].[tblQualificationPersonTrainingToEquipmentType].[UpdatedDate],[map].[tblQualificationPersonTrainingToEquipmentType].[UpdatedBy],[map].[tblQualificationPersonTrainingToEquipmentType].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonTrainingToEquipmentType]
            INNER JOIN [track].[tblQualificationPersonTrainingToEquipmentType] CT
                ON CT.PK_QualificationPersonTrainingToEquipmentTypeGuid = [map].[tblQualificationPersonTrainingToEquipmentType].[QualificationPersonTrainingToEquipmentTypeGuid]
        WHERE CT.PK_QualificationPersonTrainingToEquipmentTypeGuid = @QualificationPersonTrainingToEquipmentTypeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
