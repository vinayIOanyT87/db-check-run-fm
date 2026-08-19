CREATE FUNCTION [map].[udf_GetAssociatedCompanyOffLoadOwnerToManagerListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanyOffLoadOwnerToManagerList TABLE
(
	[CompanyOffLoadOwnerToManagerGuid] [uniqueidentifier]
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
	INSERT INTO @tblCompanyOffLoadOwnerToManagerList 
		SELECT data.[CompanyOffLoadOwnerToManagerGuid], data.[CompanyToSiteGuid], data.[AssignedToCompanySiteToGuid], data.[CompanyGuid], data.[AssignedToCompanyGuid], data.[OwnerSiteGuid]
			FROM (SELECT [map].[tblCompanyOffLoadOwnerToManager].[CompanyOffLoadOwnerToManagerGuid], data1.[CompanyToSiteGuid], data2.[CompanyToSiteGuid] 'AssignedToCompanySiteToGuid', [map].[tblCompanyOffLoadOwnerToManager].[CompanyGuid], [map].[tblCompanyOffLoadOwnerToManager].[AssignedToCompanyGuid], [map].[tblCompanyOffLoadOwnerToManager].[SiteGuid] 'OwnerSiteGuid'
					FROM [map].[tblCompanyOffLoadOwnerToManager]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
							ON [map].[tblCompanyOffLoadOwnerToManager].[CompanyGuid] = data1.[CompanyGuid]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data2
							ON [map].[tblCompanyOffLoadOwnerToManager].[AssignedToCompanyGuid] = data2.[CompanyGuid]
							WHERE @sync_context_site_guid=[tblCompanyOffLoadOwnerToManager].[SiteGuid]
				) data
	RETURN;
END