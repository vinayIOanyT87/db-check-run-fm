-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblFilterField
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblFilterField]
@FilterFieldIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblFilterField].[FilterFieldIndex],[lookup].[tblFilterField].[FilterFieldCode],[lookup].[tblFilterField].[FilterFieldName],[lookup].[tblFilterField].[FilterFieldGuid],[lookup].[tblFilterField].[CreatedDate],[lookup].[tblFilterField].[CreatedBy],[lookup].[tblFilterField].[UpdatedDate],[lookup].[tblFilterField].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblFilterField]
            INNER JOIN [track].[tblFilterField] CT
                ON CT.PK_FilterFieldIndex = [lookup].[tblFilterField].[FilterFieldIndex]
        WHERE CT.PK_FilterFieldIndex = @FilterFieldIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
