
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldProductListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldProductList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldProductGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldProductList_CTE (UserDataToSiteGuid,UserDataFieldProductGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldProduct].[UserDataFieldProductGuid],[dbo].[tblUserDataFieldProduct].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldProduct]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldProduct].[SiteGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldProduct].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldProduct].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldProduct].[UserDataFieldProductGuid],[dbo].[tblUserDataFieldProduct].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldProduct]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldProduct].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldProduct].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldProduct].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldProduct].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldProduct].[UserDataFieldProductGuid],[dbo].[tblUserDataFieldProduct].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldProduct]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldProduct].[SiteGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldProduct].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldProduct].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldProduct].[UserGroupGuid] IS NOT NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldProduct].[UserDataFieldProductGuid],[dbo].[tblUserDataFieldProduct].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldProduct]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldProduct].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldProduct].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldProduct].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldProduct].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldProduct].[UserGroupGuid] IS NOT NULL 
	)
	INSERT INTO @tblUserDataFieldProductList SELECT UserDataToSiteGuid,UserDataFieldProductGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldProductList_CTE

	RETURN;
END