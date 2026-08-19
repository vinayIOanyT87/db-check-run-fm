CREATE FUNCTION [map].[udf_GetAssociatedCompanyCompanyToCompanyGroupListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanyCompanyToCompanyGroupList TABLE
(
	[CompanyCompanyToCompanyGroupGuid] [uniqueidentifier]
	,[CompanyToSiteGuid] [uniqueidentifier]
	,[CompanyGroupToSiteGuid] [uniqueidentifier]
	,[CompanyGuid] [uniqueidentifier]
	,[ApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblCompanyCompanyToCompanyGroupList 
		SELECT data.[CompanyCompanyToCompanyGroupGuid], data.[CompanyToSiteGuid], data.[CompanyGroupToSiteGuid], data.[CompanyGuid], data.[ApplicationStringGuid], data.[OwnerSiteGuid]
			FROM (SELECT [map].[tblCompanyCompanyToCompanyGroup].[CompanyCompanyToCompanyGroupGuid], data1.[CompanyToSiteGuid], data2.[CompanyGroupToSiteGuid], [map].[tblCompanyCompanyToCompanyGroup].[CompanyGuid], [map].[tblCompanyCompanyToCompanyGroup].[ApplicationStringGuid], data1.[OwnerSiteGuid] 'OwnerSiteGuid'
					FROM [map].[tblCompanyCompanyToCompanyGroup]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
							ON [map].[tblCompanyCompanyToCompanyGroup].[CompanyGuid] = data1.[CompanyGuid]
						INNER JOIN (SELECT [CompanyGroupToSiteGuid],[ApplicationStringGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedApplicationStringCompanyGroupListForSite](@sync_context_site_guid)) data2
							ON [map].[tblCompanyCompanyToCompanyGroup].[ApplicationStringGuid] = data2.[ApplicationStringGuid]
				) data
	RETURN;
END