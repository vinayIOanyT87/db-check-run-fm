-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblAggregateField
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAggregateField]
@AggregateFieldIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblAggregateField].[AggregateFieldIndex],[lookup].[tblAggregateField].[AggregateFieldCode],[lookup].[tblAggregateField].[AggregateFieldName],[lookup].[tblAggregateField].[AggregateFieldGuid],[lookup].[tblAggregateField].[CreatedDate],[lookup].[tblAggregateField].[CreatedBy],[lookup].[tblAggregateField].[UpdatedDate],[lookup].[tblAggregateField].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblAggregateField]
            INNER JOIN [track].[tblAggregateField] CT
                ON CT.PK_AggregateFieldIndex = [lookup].[tblAggregateField].[AggregateFieldIndex]
        WHERE CT.PK_AggregateFieldIndex = @AggregateFieldIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
