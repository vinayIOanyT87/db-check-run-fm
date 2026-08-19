-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblAllocations
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientSelectUpdateConflicts_tblAllocations]
@AllocationGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [dbo].[tblAllocations].[EffectiveDate],[dbo].[tblAllocations].[ExpirationDate],[dbo].[tblAllocations].[LoadWarning],[dbo].[tblAllocations].[LoadDenial],[dbo].[tblAllocations].[ContractNumber],[dbo].[tblAllocations].[AllocationGroupIndex],[dbo].[tblAllocations].[LastAllocationResetDate],[dbo].[tblAllocations].[CreatedDate],[dbo].[tblAllocations].[CreatedBy],[dbo].[tblAllocations].[UpdatedDate],[dbo].[tblAllocations].[UpdatedBy],[dbo].[tblAllocations].[AllocationGuid],[dbo].[tblAllocations].[CompanyBillToToShipperGuid],[dbo].[tblAllocations].[CompanyLoadOwnerToManagerGuid],[dbo].[tblAllocations].[CompanyOffLoadOwnerToManagerGuid],[dbo].[tblAllocations].[CompanyShipperToOwnerGuid],[dbo].[tblAllocations].[CompanyShipToToBillToGuid],[dbo].[tblAllocations].[CompanySupplierToOwnerGuid],[dbo].[tblAllocations].[SiteGuid],[dbo].[tblAllocations].[LookupCompanyMapTypeIndex],[dbo].[tblAllocations].[AllocationGroupApplicationStringGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [dbo].[tblAllocations]
            INNER JOIN [track].[tblAllocations] CT
                ON CT.PK_AllocationGuid = [dbo].[tblAllocations].[AllocationGuid]
        WHERE CT.PK_AllocationGuid = @AllocationGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
