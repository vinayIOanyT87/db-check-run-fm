CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringCompanyGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[CompanyGroupToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityCompanyGroupToSite].[CompanyGroupToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityCompanyGroupToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityCompanyGroupToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityCompanyGroupToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityCompanyGroupToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END