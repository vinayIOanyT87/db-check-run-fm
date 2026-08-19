-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblDispatchGridColumnType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblDispatchGridColumnType]
@DispatchGridColumnTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblDispatchGridColumnType].[DispatchGridColumnTypeIndex],[lookup].[tblDispatchGridColumnType].[LookupDispatchGridTypeIndex],[lookup].[tblDispatchGridColumnType].[DispatchGridColumnTypeGuid],[lookup].[tblDispatchGridColumnType].[CreatedDate],[lookup].[tblDispatchGridColumnType].[CreatedBy],[lookup].[tblDispatchGridColumnType].[UpdatedDate],[lookup].[tblDispatchGridColumnType].[UpdatedBy],[lookup].[tblDispatchGridColumnType].[ID],[lookup].[tblDispatchGridColumnType].[DisplayName],[lookup].[tblDispatchGridColumnType].[DataField],[lookup].[tblDispatchGridColumnType].[Width],[lookup].[tblDispatchGridColumnType].[DefaultColumnOrder], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblDispatchGridColumnType]
            INNER JOIN [track].[tblDispatchGridColumnType] CT
                ON CT.PK_DispatchGridColumnTypeIndex = [lookup].[tblDispatchGridColumnType].[DispatchGridColumnTypeIndex]
        WHERE CT.PK_DispatchGridColumnTypeIndex = @DispatchGridColumnTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
