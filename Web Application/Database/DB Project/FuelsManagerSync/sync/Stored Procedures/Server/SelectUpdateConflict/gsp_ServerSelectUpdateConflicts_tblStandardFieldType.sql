-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblStandardFieldType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblStandardFieldType]
@StandardFieldTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblStandardFieldType].[StandardFieldTypeIndex],[lookup].[tblStandardFieldType].[StandardFieldTypeCode],[lookup].[tblStandardFieldType].[StandardFieldTypeName],[lookup].[tblStandardFieldType].[StandardFieldTypeGuid],[lookup].[tblStandardFieldType].[CreatedDate],[lookup].[tblStandardFieldType].[CreatedBy],[lookup].[tblStandardFieldType].[UpdatedDate],[lookup].[tblStandardFieldType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblStandardFieldType]
            INNER JOIN [track].[tblStandardFieldType] CT
                ON CT.PK_StandardFieldTypeIndex = [lookup].[tblStandardFieldType].[StandardFieldTypeIndex]
        WHERE CT.PK_StandardFieldTypeIndex = @StandardFieldTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
