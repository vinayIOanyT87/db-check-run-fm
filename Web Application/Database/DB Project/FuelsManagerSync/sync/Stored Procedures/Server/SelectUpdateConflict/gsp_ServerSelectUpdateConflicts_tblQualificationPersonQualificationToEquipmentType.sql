-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblQualificationPersonQualificationToEquipmentType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationPersonQualificationToEquipmentType]
@QualificationPersonQualificationToEquipmentTypeGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblQualificationPersonQualificationToEquipmentType].[QualificationPersonQualificationToEquipmentTypeGuid],[map].[tblQualificationPersonQualificationToEquipmentType].[QualificationGuid],[map].[tblQualificationPersonQualificationToEquipmentType].[EquipmentTypeGuid],[map].[tblQualificationPersonQualificationToEquipmentType].[Sequence],[map].[tblQualificationPersonQualificationToEquipmentType].[Instructor],[map].[tblQualificationPersonQualificationToEquipmentType].[DateCompleted],[map].[tblQualificationPersonQualificationToEquipmentType].[DateDue],[map].[tblQualificationPersonQualificationToEquipmentType].[ExpirationDate],[map].[tblQualificationPersonQualificationToEquipmentType].[ID],[map].[tblQualificationPersonQualificationToEquipmentType].[Rating],[map].[tblQualificationPersonQualificationToEquipmentType].[HistoricalRecord],[map].[tblQualificationPersonQualificationToEquipmentType].[CreatedDate],[map].[tblQualificationPersonQualificationToEquipmentType].[CreatedBy],[map].[tblQualificationPersonQualificationToEquipmentType].[UpdatedDate],[map].[tblQualificationPersonQualificationToEquipmentType].[UpdatedBy],[map].[tblQualificationPersonQualificationToEquipmentType].[SiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblQualificationPersonQualificationToEquipmentType]
            INNER JOIN [track].[tblQualificationPersonQualificationToEquipmentType] CT
                ON CT.PK_QualificationPersonQualificationToEquipmentTypeGuid = [map].[tblQualificationPersonQualificationToEquipmentType].[QualificationPersonQualificationToEquipmentTypeGuid]
        WHERE CT.PK_QualificationPersonQualificationToEquipmentTypeGuid = @QualificationPersonQualificationToEquipmentTypeGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
