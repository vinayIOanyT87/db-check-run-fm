-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblProcessVariableType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblProcessVariableType]
@ProcessVariableTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblProcessVariableType].[ProcessVariableTypeIndex],[lookup].[tblProcessVariableType].[ProcessVariableTypeCode],[lookup].[tblProcessVariableType].[ProcessVariableTypeName],[lookup].[tblProcessVariableType].[ProcessVariableTypeGuid],[lookup].[tblProcessVariableType].[CreatedDate],[lookup].[tblProcessVariableType].[CreatedBy],[lookup].[tblProcessVariableType].[UpdatedDate],[lookup].[tblProcessVariableType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblProcessVariableType]
            INNER JOIN [track].[tblProcessVariableType] CT
                ON CT.PK_ProcessVariableTypeIndex = [lookup].[tblProcessVariableType].[ProcessVariableTypeIndex]
        WHERE CT.PK_ProcessVariableTypeIndex = @ProcessVariableTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
