CREATE FUNCTION [map].[udf_GetAssociatedCompanyAuthorizedCarrierToCompanyListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanyAuthorizedCarrierToCompanyList TABLE
(
	[CompanyAuthorizedCarrierToCompanyGuid] [uniqueidentifier]
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
	INSERT INTO @tblCompanyAuthorizedCarrierToCompanyList 
		SELECT data.[CompanyAuthorizedCarrierToCompanyGuid], data.[CompanyToSiteGuid], data.[AssignedToCompanyToSiteGuid], data.[CompanyGuid], data.[AssignedToCompanyGuid], data.[OwnerSiteGuid]
			FROM (SELECT [map].[tblCompanyAuthorizedCarrierToCompany].[CompanyAuthorizedCarrierToCompanyGuid], data1.[CompanyToSiteGuid], data2.[CompanyToSiteGuid] 'AssignedToCompanyToSiteGuid', [map].[tblCompanyAuthorizedCarrierToCompany].[CompanyGuid], [map].[tblCompanyAuthorizedCarrierToCompany].[AssignedToCompanyGuid], [map].[tblCompanyAuthorizedCarrierToCompany].[SiteGuid] 'OwnerSiteGuid'
					FROM [map].[tblCompanyAuthorizedCarrierToCompany]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
							ON [map].[tblCompanyAuthorizedCarrierToCompany].[CompanyGuid] = data1.[CompanyGuid]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data2
							ON [map].[tblCompanyAuthorizedCarrierToCompany].[AssignedToCompanyGuid] = data2.[CompanyGuid]
				) data
			WHERE data.OwnerSiteGuid = @sync_context_site_guid
	RETURN;
END