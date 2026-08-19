
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldSiteListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldSiteList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldSiteGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldSiteList_CTE (UserDataToSiteGuid,UserDataFieldSiteGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldSite].[UserDataFieldSiteGuid],[dbo].[tblUserDataFieldSite].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldSite]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldSite].[SiteGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldSite].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldSite].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldSite].[UserDataFieldSiteGuid],[dbo].[tblUserDataFieldSite].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldSite]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldSite].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldSite].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldSite].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldSite].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldSite].[UserDataFieldSiteGuid],[dbo].[tblUserDataFieldSite].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldSite]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldSite].[SiteGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldSite].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldSite].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldSite].[UserGroupGuid] IS NOT NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldSite].[UserDataFieldSiteGuid],[dbo].[tblUserDataFieldSite].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldSite]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldSite].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldSite].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldSite].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldSite].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldSite].[UserGroupGuid] IS NOT NULL 
	)
	INSERT INTO @tblUserDataFieldSiteList SELECT UserDataToSiteGuid,UserDataFieldSiteGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldSiteList_CTE

	RETURN;
END