CREATE FUNCTION [map].[udf_GetAssociatedCompanyCompanyToUserGroupListForSite](
@sync_context_site_guid uniqueidentifier,
@only_where_company_guid_is_null bit
)
RETURNS @tblCompanyCompanyToUserGroupList TABLE
(
    [CompanyCompanyToUserGroupGuid] [uniqueidentifier]
    ,[CompanyToSiteGuid] [uniqueidentifier]
    ,[UserGroupToSiteGuid] [uniqueidentifier]
    ,[CompanyGuid] [uniqueidentifier]
    ,[GroupGuid] [uniqueidentifier]
    ,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
    -- Company guid is null when a User Group is mapped to "All" companies.
    IF (@only_where_company_guid_is_null = 0)
    BEGIN
        INSERT INTO @tblCompanyCompanyToUserGroupList
            SELECT [map].[tblCompanyCompanyToUserGroup].[CompanyCompanyToUserGroupGuid], data1.[CompanyToSiteGuid], data2.[UserGroupToSiteGuid], [map].[tblCompanyCompanyToUserGroup].[CompanyGuid], [map].[tblCompanyCompanyToUserGroup].[GroupGuid], [map].[tblCompanyCompanyToUserGroup].[SiteGuid] 'OwnerSiteGuid'
                FROM [map].[tblCompanyCompanyToUserGroup]
                    INNER JOIN (SELECT [CompanyToSiteGuid],[CompanyGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedCompanyListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblCompanyCompanyToUserGroup].[CompanyGuid] = data1.[CompanyGuid]
                    INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data2
                        ON [map].[tblCompanyCompanyToUserGroup].[GroupGuid] = data2.[GroupGuid]
                WHERE [map].[tblCompanyCompanyToUserGroup].[CompanyGuid] IS NOT NULL
    END
    ELSE
    BEGIN
        INSERT INTO @tblCompanyCompanyToUserGroupList
		    SELECT [map].[tblCompanyCompanyToUserGroup].[CompanyCompanyToUserGroupGuid], NULL 'CompanyToSiteGuid', data1.[UserGroupToSiteGuid], [map].[tblCompanyCompanyToUserGroup].[CompanyGuid], [map].[tblCompanyCompanyToUserGroup].[GroupGuid], [map].[tblCompanyCompanyToUserGroup].[SiteGuid] 'OwnerSiteGuid'
                FROM [map].[tblCompanyCompanyToUserGroup]
                    INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
                        ON [map].[tblCompanyCompanyToUserGroup].[GroupGuid] = data1.[GroupGuid]
                WHERE [map].[tblCompanyCompanyToUserGroup].[CompanyGuid] IS NULL
    END

    RETURN;
END