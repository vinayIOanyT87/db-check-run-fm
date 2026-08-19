-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblQualificationType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblQualificationType]
@QualificationTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblQualificationType].[QualificationTypeIndex],[lookup].[tblQualificationType].[QualificationTypeCode],[lookup].[tblQualificationType].[QualificationTypeName],[lookup].[tblQualificationType].[QualificationTypeGuid],[lookup].[tblQualificationType].[CreatedDate],[lookup].[tblQualificationType].[CreatedBy],[lookup].[tblQualificationType].[UpdatedDate],[lookup].[tblQualificationType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblQualificationType]
            INNER JOIN [track].[tblQualificationType] CT
                ON CT.PK_QualificationTypeIndex = [lookup].[tblQualificationType].[QualificationTypeIndex]
        WHERE CT.PK_QualificationTypeIndex = @QualificationTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
