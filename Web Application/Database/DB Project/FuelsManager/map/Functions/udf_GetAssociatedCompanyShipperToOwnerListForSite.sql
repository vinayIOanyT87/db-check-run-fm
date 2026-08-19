CREATE FUNCTION [map].[udf_GetAssociatedCompanyShipperToOwnerListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblCompanyShipperToOwnerList TABLE
(
	[CompanyShipperToOwnerGuid] [uniqueidentifier]
	,[CompanyToSiteGuid] [uniqueidentifier]
	,[CompanyGuid] [uniqueidentifier]
	,[CompanyLoadOwnerToManagerGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblCompanyShipperToOwnerList 
		SELECT data.[CompanyShipperToOwnerGuid], data.[CompanyToSiteGuid], data.[CompanyGuid], data.[CompanyLoadOwnerToManagerGuid], data.[OwnerSiteGuid]
			FROM (SELECT [map].[tblCompanyShipperToOwner].[CompanyShipperToOwnerGuid], data1.[CompanyToSiteGuid], [map].[tblCompanyShipperToOwner].[CompanyGuid], [map].[tblCompanyShipperToOwner].[CompanyLoadOwnerToManagerGuid], [map].[tblCompanyShipperToOwner].[SiteGuid] 'OwnerSiteGuid'
					FROM [map].[tblCompanyShipperToOwner]
						INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
							ON [map].[tblCompanyShipperToOwner].[CompanyGuid] = data1.[CompanyGuid]
						INNER JOIN (SELECT [CompanyLoadOwnerToManagerGuid],[OwnerSiteGuid] FROM [map].[udf_GetAssociatedCompanyLoadOwnerToManagerListForSite](@sync_context_site_guid)) data2
							ON [map].[tblCompanyShipperToOwner].[CompanyLoadOwnerToManagerGuid] = data2.[CompanyLoadOwnerToManagerGuid]
				) data
	RETURN;
END