CREATE FUNCTION [map].[udf_GetAssociatedCompanyPersonnelAssignedToCompanyListForSite](
@sync_context_site_guid uniqueidentifier,
@only_where_company_guid_is_null bit
)
RETURNS @tblCompanyPersonnelAssignedToCompanyList TABLE
(
    [CompanyPersonnelAssignedToCompanyGuid] [uniqueidentifier]
    ,[CompanyToSiteGuid] [uniqueidentifier]
    ,[PersonnelToSiteGuid] [uniqueidentifier]
    ,[CompanyGuid] [uniqueidentifier]
    ,[PersonnelGuid] [uniqueidentifier]
    ,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
    -- Company guid is null when Personnel is mapped to "All" companies.
    IF (@only_where_company_guid_is_null = 0)
    BEGIN
        INSERT INTO @tblCompanyPersonnelAssignedToCompanyList
            SELECT [map].[tblCompanyPersonnelAssignedToCompany].[CompanyPersonnelAssignedToCompanyGuid], data1.[CompanyToSiteGuid], data2.[PersonnelToSiteGuid], [map].[tblCompanyPersonnelAssignedToCompany].[CompanyGuid], [map].[tblCompanyPersonnelAssignedToCompany].[PersonnelGuid], [map].[tblCompanyPersonnelAssignedToCompany].[SiteGuid] 'OwnerSiteGuid'
                FROM [map].[tblCompanyPersonnelAssignedToCompany]
                    INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblCompanyPersonnelAssignedToCompany].[CompanyGuid] = data1.[CompanyGuid]
                    INNER JOIN (SELECT [PersonnelToSiteGuid],[PersonnelGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedPersonnelListForSite](@sync_context_site_guid)) data2
                        ON [map].[tblCompanyPersonnelAssignedToCompany].[PersonnelGuid] = data2.[PersonnelGuid]
                WHERE [map].[tblCompanyPersonnelAssignedToCompany].[CompanyGuid] IS NOT NULL
    END
    ELSE
    BEGIN
        INSERT INTO @tblCompanyPersonnelAssignedToCompanyList
		    SELECT [map].[tblCompanyPersonnelAssignedToCompany].[CompanyPersonnelAssignedToCompanyGuid], NULL 'CompanyToSiteGuid', data1.[PersonnelToSiteGuid], [map].[tblCompanyPersonnelAssignedToCompany].[CompanyGuid], [map].[tblCompanyPersonnelAssignedToCompany].[PersonnelGuid], [map].[tblCompanyPersonnelAssignedToCompany].[SiteGuid] 'OwnerSiteGuid'
                FROM [map].[tblCompanyPersonnelAssignedToCompany]
                    INNER JOIN (SELECT [PersonnelToSiteGuid],[PersonnelGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedPersonnelListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblCompanyPersonnelAssignedToCompany].[PersonnelGuid] = data1.[PersonnelGuid]
                WHERE [map].[tblCompanyPersonnelAssignedToCompany].[CompanyGuid] IS NULL
    END

    RETURN;
END