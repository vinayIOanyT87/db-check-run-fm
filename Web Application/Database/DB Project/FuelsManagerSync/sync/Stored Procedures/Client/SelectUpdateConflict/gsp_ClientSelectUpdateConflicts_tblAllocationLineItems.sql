-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAllocationLineItems
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAllocationLineItems]
@AllocationLineItemGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAllocationLineItems].[Limit],[dbo].[tblAllocationLineItems].[Next],[dbo].[tblAllocationLineItems].[ResetMultiple],[dbo].[tblAllocationLineItems].[ResetDate],[dbo].[tblAllocationLineItems].[CreatedDate],[dbo].[tblAllocationLineItems].[CreatedBy],[dbo].[tblAllocationLineItems].[UpdatedDate],[dbo].[tblAllocationLineItems].[UpdatedBy],[dbo].[tblAllocationLineItems].[AllocationLineItemGuid],[dbo].[tblAllocationLineItems].[LookupAllocationTypeIndex],[dbo].[tblAllocationLineItems].[LookupResetMethodIndex],[dbo].[tblAllocationLineItems].[LookupResetPeriodIndex],[dbo].[tblAllocationLineItems].[AllocationGuid],[dbo].[tblAllocationLineItems].[AssignedProductGuid],[dbo].[tblAllocationLineItems].[AssignedApplicationStringGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAllocationLineItems]
            INNER JOIN [track].[tblAllocationLineItems] CT
                ON CT.PK_AllocationLineItemGuid = [dbo].[tblAllocationLineItems].[AllocationLineItemGuid]
        WHERE CT.PK_AllocationLineItemGuid = @AllocationLineItemGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
