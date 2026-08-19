CREATE FUNCTION [map].[udf_GetAssociatedCompanyLoadOwnerToManagerListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanyLoadOwnerToManagerList TABLE
(
	[CompanyLoadOwnerToManagerGuid] [uniqueidentifier]
	,[CompanyToSiteGuid] [uniqueidentifier]
	,[AssignedToCompanyToSiteGuid] [uniqueidentifier]
	,[CompanyGuid] [uniqueidentifier]
	,[AssignedToCompanyGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblCompanyLoadOwnerToManagerList 
		SELECT data.[CompanyLoadOwnerToManagerGuid], data.[CompanyToSiteGuid], data.[AssignedToCompanySiteToGuid], data.[CompanyGuid], data.[AssignedToCompanyGuid], data.[OwnerSiteGuid]
			FROM (SELECT [map].[tblCompanyLoadOwnerToManager].[CompanyLoadOwnerToManagerGuid], data1.[CompanyToSiteGuid], data2.[CompanyToSiteGuid] 'AssignedToCompanySiteToGuid', [map].[tblCompanyLoadOwnerToManager].[CompanyGuid], [map].[tblCompanyLoadOwnerToManager].[AssignedToCompanyGuid], [map].[tblCompanyLoadOwnerToManager].[SiteGuid] 'OwnerSiteGuid'
					FROM [map].[tblCompanyLoadOwnerToManager]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
							ON [map].[tblCompanyLoadOwnerToManager].[CompanyGuid] = data1.[CompanyGuid]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data2
							ON [map].[tblCompanyLoadOwnerToManager].[AssignedToCompanyGuid] = data2.[CompanyGuid]
							WHERE @sync_context_site_guid=[tblCompanyLoadOwnerToManager].[SiteGuid]
				) data
	RETURN;
END