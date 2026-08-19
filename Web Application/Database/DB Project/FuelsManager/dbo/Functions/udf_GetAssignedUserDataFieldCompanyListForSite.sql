
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldCompanyListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldCompanyList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldCompanyGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldCompanyList_CTE (UserDataToSiteGuid,UserDataFieldCompanyGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataFieldCompany].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldCompany]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldCompany].[SiteGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldCompany].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldCompany].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataFieldCompany].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldCompany]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldCompany].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldCompany].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldCompany].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldCompany].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataFieldCompany].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldCompany]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldCompany].[SiteGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldCompany].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldCompany].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldCompany].[UserGroupGuid] IS NOT NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldCompany].[UserDataFieldCompanyGuid],[dbo].[tblUserDataFieldCompany].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldCompany]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldCompany].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldCompany].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldCompany].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldCompany].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldCompany].[UserGroupGuid] IS NOT NULL 
	)
	INSERT INTO @tblUserDataFieldCompanyList SELECT UserDataToSiteGuid,UserDataFieldCompanyGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldCompanyList_CTE

	RETURN;
END