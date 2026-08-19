CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringCompanyTypeListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[CompanyTypeToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityCompanyTypeToSite].[CompanyTypeToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityCompanyTypeToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityCompanyTypeToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityCompanyTypeToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityCompanyTypeToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END
