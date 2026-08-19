CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringAllocationGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[AllocationGroupToSiteGuid] [uniqueidentifier]
	,[ApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblApplicationStringList 
		SELECT [map].[tblEntityAllocationGroupToSite].[AllocationGroupToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAllocationGroupToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityAllocationGroupToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityAllocationGroupToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityAllocationGroupToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END