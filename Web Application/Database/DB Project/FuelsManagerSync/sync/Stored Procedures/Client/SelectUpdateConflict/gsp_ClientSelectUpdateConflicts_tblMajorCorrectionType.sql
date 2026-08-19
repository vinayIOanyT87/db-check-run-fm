-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblMajorCorrectionType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMajorCorrectionType]
@MajorCorrectionTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblMajorCorrectionType].[MajorCorrectionTypeIndex],[lookup].[tblMajorCorrectionType].[MajorCorrectionTypeCode],[lookup].[tblMajorCorrectionType].[MajorCorrectionTypeName],[lookup].[tblMajorCorrectionType].[MajorCorrectionTypeGuid],[lookup].[tblMajorCorrectionType].[CreatedDate],[lookup].[tblMajorCorrectionType].[CreatedBy],[lookup].[tblMajorCorrectionType].[UpdatedDate],[lookup].[tblMajorCorrectionType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblMajorCorrectionType]
            INNER JOIN [track].[tblMajorCorrectionType] CT
                ON CT.PK_MajorCorrectionTypeIndex = [lookup].[tblMajorCorrectionType].[MajorCorrectionTypeIndex]
        WHERE CT.PK_MajorCorrectionTypeIndex = @MajorCorrectionTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
