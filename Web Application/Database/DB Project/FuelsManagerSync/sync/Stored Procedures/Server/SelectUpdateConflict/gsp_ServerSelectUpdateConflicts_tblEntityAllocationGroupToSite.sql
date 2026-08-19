-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : map.tblEntityAllocationGroupToSite
-- Description: Select Update Conflicts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerSelectUpdateConflicts_tblEntityAllocationGroupToSite]
@AllocationGroupToSiteGuid uniqueidentifier
AS
BEGIN
    -- This command is used if @sync_row_count returns
    -- 0 when changes are applied to the server.
    --
    SELECT [map].[tblEntityAllocationGroupToSite].[AllocationGroupToSiteGuid],[map].[tblEntityAllocationGroupToSite].[ApplicationStringGuid],[map].[tblEntityAllocationGroupToSite].[SiteGuid],[map].[tblEntityAllocationGroupToSite].[CreatedDate],[map].[tblEntityAllocationGroupToSite].[CreatedBy],[map].[tblEntityAllocationGroupToSite].[UpdatedDate],[map].[tblEntityAllocationGroupToSite].[UpdatedBy],[map].[tblEntityAllocationGroupToSite].[AssignedFromSiteGuid], CT.UpdatedContext, CT.UpdatedRowVersion AS '_RowVersion'
        FROM [map].[tblEntityAllocationGroupToSite]
            INNER JOIN [track].[tblEntityAllocationGroupToSite] CT
                ON CT.PK_AllocationGroupToSiteGuid = [map].[tblEntityAllocationGroupToSite].[AllocationGroupToSiteGuid]
        WHERE CT.PK_AllocationGroupToSiteGuid = @AllocationGroupToSiteGuid
    ORDER BY CT.UpdatedRowVersion ASC
END
