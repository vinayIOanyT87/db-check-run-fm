-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblMinorCorrectionType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblMinorCorrectionType]
@MinorCorrectionTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblMinorCorrectionType].[MinorCorrectionTypeIndex],[lookup].[tblMinorCorrectionType].[MinorCorrectionTypeCode],[lookup].[tblMinorCorrectionType].[MinorCorrectionTypeName],[lookup].[tblMinorCorrectionType].[MinorCorrectionTypeGuid],[lookup].[tblMinorCorrectionType].[CreatedDate],[lookup].[tblMinorCorrectionType].[CreatedBy],[lookup].[tblMinorCorrectionType].[UpdatedDate],[lookup].[tblMinorCorrectionType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblMinorCorrectionType]
            INNER JOIN [track].[tblMinorCorrectionType] CT
                ON CT.PK_MinorCorrectionTypeIndex = [lookup].[tblMinorCorrectionType].[MinorCorrectionTypeIndex]
        WHERE CT.PK_MinorCorrectionTypeIndex = @MinorCorrectionTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
