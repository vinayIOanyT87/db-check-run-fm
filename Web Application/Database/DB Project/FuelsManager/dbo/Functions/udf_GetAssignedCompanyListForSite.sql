
CREATE FUNCTION [dbo].[udf_GetAssignedCompanyListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanyList TABLE
(
	[CompanyToSiteGuid] [uniqueidentifier]
	,[CompanyGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	DECLARE @entityAssignmentList AS TABLE
	(
		CompanyGuid uniqueidentifier
		,MasterRecordGuid uniqueidentifier
		,AssignedFromSiteGuid uniqueidentifier
		,AssignedToSiteGuid uniqueidentifier
	)

	-- First we will leverage a record versioning function which will return a list of all self-owned entities 
	-- in addition to the actual entities that are applicable to @sync_context_site_guid.
	-- Remember, the CompanyGuid in the mapping table always points to the MasterCompanyGuid so we need to utilize this
	-- function in order to obtain the correct CompanyGuid values that correspond to each entity assignment record.
	INSERT INTO @entityAssignmentList
		SELECT data.[CompanyGuid], data.[MasterRecordGuid],data.[AssignedFromSiteGuid],data.[AssignedToSiteGuid]
		FROM [erv].[udf_GetCompanyRecordVersions](@sync_context_site_guid) data

	-- (Below), the first part of the union is our standard entity assignment query which will only return entities that were 
	-- created by the site (owned) or the master entity associated with the site via record versioning.  Typically, this query
	-- would return owned entities and the correct assigned entity.  However; record versioning only records the master 
	-- entity's guid in the entity assignment table, not the actual entity that is applicable to the site (unless it's self owned).
	-- This first query is still needed, because we must synchronize the master entity in order to support record versioning 
	-- because of foreign key references.

	-- One item that is missing from the record versioning results above, and that synchronization needs, is the 
	-- EntityCompanyToSiteGuid.  So, the second part of the union joins the @entityAssignmentList back to the entity mapping
	-- table so that we can obtain the missing piece of information.  Caution: the CompanyGuid in the mapping table 
	-- always points to the MasterCompanyGuid, so we will return the CompanyGuid from the @entityAssignmentList table
	-- instead of the CompanyGuid from the mapping table.
	INSERT INTO @tblCompanyList 
		SELECT [map].[tblEntityCompanyToSite].[CompanyToSiteGuid], [dbo].[tblCompanies].[CompanyGuid],[dbo].[tblCompanies].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityCompanyToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityCompanyToSite]
			INNER JOIN [dbo].[tblCompanies]
				ON [map].[tblEntityCompanyToSite].[CompanyGuid] = [dbo].[tblCompanies].[CompanyGuid]
		WHERE ([map].[tblEntityCompanyToSite].[SiteGuid] = @sync_context_site_guid)
		UNION
		SELECT [map].[tblEntityCompanyToSite].[CompanyToSiteGuid], data1.[CompanyGuid],[dbo].[tblCompanies].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityCompanyToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM @entityAssignmentList data1
				INNER JOIN [map].[tblEntityCompanyToSite]
					ON [map].[tblEntityCompanyToSite].[CompanyGuid] = data1.[MasterRecordGuid]
						AND [map].[tblEntityCompanyToSite].[AssignedFromSiteGuid] = data1.[AssignedFromSiteGuid]
						AND [map].[tblEntityCompanyToSite].[SiteGuid] = data1.[AssignedToSiteGuid]
				INNER JOIN [dbo].[tblCompanies]
					ON [dbo].[tblCompanies].[CompanyGuid] = data1.[CompanyGuid]
			WHERE [dbo].[tblCompanies].[CompanyGuid] <> data1.[MasterRecordGuid]

	RETURN;
END