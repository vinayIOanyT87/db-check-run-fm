CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringProductGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[ProductGroupToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityProductGroupToSite].[ProductGroupToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityProductGroupToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityProductGroupToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityProductGroupToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityProductGroupToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END