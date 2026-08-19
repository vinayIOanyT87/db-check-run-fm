CREATE FUNCTION [map].[udf_GetAssociatedCompanySupplierToOwnerListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanySupplierToOwnerList TABLE
(
	[CompanySupplierToOwnerGuid] [uniqueidentifier]
	,[CompanyToSiteGuid] [uniqueidentifier]
	,[CompanyGuid] [uniqueidentifier]
	,[CompanyOffLoadOwnerToManagerGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblCompanySupplierToOwnerList 
		SELECT data.[CompanySupplierToOwnerGuid], data.[CompanyToSiteGuid], data.[CompanyGuid], data.[CompanyOffLoadOwnerToManagerGuid], data.[OwnerSiteGuid]
			FROM (SELECT [map].[tblCompanySupplierToOwner].[CompanySupplierToOwnerGuid], data1.[CompanyToSiteGuid], [map].[tblCompanySupplierToOwner].[CompanyGuid], [map].[tblCompanySupplierToOwner].[CompanyOffLoadOwnerToManagerGuid], [map].[tblCompanySupplierToOwner].[SiteGuid] 'OwnerSiteGuid'
					FROM [map].[tblCompanySupplierToOwner]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
							ON [map].[tblCompanySupplierToOwner].[CompanyGuid] = data1.[CompanyGuid]
						INNER JOIN (SELECT [CompanyOffLoadOwnerToManagerGuid],[OwnerSiteGuid] FROM [map].[udf_GetAssociatedCompanyOffLoadOwnerToManagerListForSite](@sync_context_site_guid)) data2
							ON [map].[tblCompanySupplierToOwner].[CompanyOffLoadOwnerToManagerGuid] = data2.[CompanyOffLoadOwnerToManagerGuid]
					WHERE [map].[tblCompanySupplierToOwner].[SiteGuid] = @sync_context_site_guid
				) data
	RETURN;
END