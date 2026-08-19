CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldUserListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldUserList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldUserGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldUserList_CTE (UserDataToSiteGuid,UserDataFieldUserGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldUser].[UserDataFieldUserGuid],[dbo].[tblUserDataFieldUser].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldUser]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldUser].[SiteGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldUser].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldUser].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldUser].[UserDataFieldUserGuid],[dbo].[tblUserDataFieldUser].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldUser]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldUser].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldUser].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldUser].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldUser].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldUser].[UserDataFieldUserGuid],[dbo].[tblUserDataFieldUser].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldUser]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldUser].[SiteGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldUser].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldUser].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldUser].[UserGroupGuid] IS NOT NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldUser].[UserDataFieldUserGuid],[dbo].[tblUserDataFieldUser].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldUser]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldUser].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldUser].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldUser].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldUser].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldUser].[UserGroupGuid] IS NOT NULL 
	)
	INSERT INTO @tblUserDataFieldUserList SELECT UserDataToSiteGuid,UserDataFieldUserGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldUserList_CTE

	RETURN;
END