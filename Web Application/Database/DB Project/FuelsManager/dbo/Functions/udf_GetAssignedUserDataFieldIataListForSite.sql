
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldIataListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldIataList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldIataGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldIataList_CTE (UserDataToSiteGuid,UserDataFieldIataGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldIata].[UserDataFieldIataGuid],[dbo].[tblUserDataFieldIata].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldIata]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldIata].[SiteGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldIata].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldIata].[UserGroupGuid] IS NULL 
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldIata].[UserDataFieldIataGuid],[dbo].[tblUserDataFieldIata].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldIata]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldIata].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldIata].[TransactionAliasGuid] = DATA.[TransactionAliasGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldIata].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldIata].[UserGroupGuid] IS NULL
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldIata].[UserDataFieldIataGuid],[dbo].[tblUserDataFieldIata].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldIata]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldIata].[SiteGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid], [GroupGuid], [OwnerSiteGuid], [AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldIata].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldIata].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldIata].[UserGroupGuid] IS NOT NULL
		UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldIata].[UserDataFieldIataGuid],[dbo].[tblUserDataFieldIata].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityUserDataToSite]
				INNER JOIN [dbo].[tblUserDataFieldIata]
					ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldIata].[SiteGuid]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldIata].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldIata].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldIata].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldIata].[UserGroupGuid] IS NOT NULL
	)
	INSERT INTO @tblUserDataFieldIataList SELECT UserDataToSiteGuid,UserDataFieldIataGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldIataList_CTE

	RETURN;
END