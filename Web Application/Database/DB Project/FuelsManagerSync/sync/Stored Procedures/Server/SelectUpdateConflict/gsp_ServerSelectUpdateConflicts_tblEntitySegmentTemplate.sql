-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : erv.tblEntitySegmentTemplate
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntitySegmentTemplate]
@EntitySegmentTemplateGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [erv].[tblEntitySegmentTemplate].[EntitySegmentTemplateGuid],[erv].[tblEntitySegmentTemplate].[AppTableName],[erv].[tblEntitySegmentTemplate].[EntityIndexFieldName],[erv].[tblEntitySegmentTemplate].[EntityTypeId],[erv].[tblEntitySegmentTemplate].[EntityTypeDisplayName],[erv].[tblEntitySegmentTemplate].[FilterFieldName],[erv].[tblEntitySegmentTemplate].[FilterDisplayName],[erv].[tblEntitySegmentTemplate].[FilterValuesStoredProc],[erv].[tblEntitySegmentTemplate].[FieldLevelConfigSegment],[erv].[tblEntitySegmentTemplate].[LocationBasedConstraintSegment],[erv].[tblEntitySegmentTemplate].[SystemSegment],[erv].[tblEntitySegmentTemplate].[EntityAssignmentTableName],[erv].[tblEntitySegmentTemplate].[CreatedDate],[erv].[tblEntitySegmentTemplate].[CreatedBy],[erv].[tblEntitySegmentTemplate].[UpdatedDate],[erv].[tblEntitySegmentTemplate].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [erv].[tblEntitySegmentTemplate]
            INNER JOIN [track].[tblEntitySegmentTemplate] CT
                ON CT.PK_EntitySegmentTemplateGuid = [erv].[tblEntitySegmentTemplate].[EntitySegmentTemplateGuid]
        WHERE CT.PK_EntitySegmentTemplateGuid = @EntitySegmentTemplateGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
