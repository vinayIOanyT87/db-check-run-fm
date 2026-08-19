-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : lookup.tblAllocationType
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAllocationType]
@AllocationTypeIndex int
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [lookup].[tblAllocationType].[AllocationTypeIndex],[lookup].[tblAllocationType].[AllocationTypeCode],[lookup].[tblAllocationType].[AllocationTypeName],[lookup].[tblAllocationType].[AllocationTypeGuid],[lookup].[tblAllocationType].[CreatedDate],[lookup].[tblAllocationType].[CreatedBy],[lookup].[tblAllocationType].[UpdatedDate],[lookup].[tblAllocationType].[UpdatedBy], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [lookup].[tblAllocationType]
            INNER JOIN [track].[tblAllocationType] CT
                ON CT.PK_AllocationTypeIndex = [lookup].[tblAllocationType].[AllocationTypeIndex]
        WHERE CT.PK_AllocationTypeIndex = @AllocationTypeIndex
    ORDER BY CT.UpdatedRowVersion ASC
END
