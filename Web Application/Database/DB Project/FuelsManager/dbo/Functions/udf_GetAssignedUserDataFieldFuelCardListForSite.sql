
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldFuelCardListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldFuelCardList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldFuelCardGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldFuelCardList_CTE (UserDataToSiteGuid,UserDataFieldFuelCardGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldFuelCard]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldFuelCard].[SiteGuid]
			WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
					AND [dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldFuelCard].[UserGroupGuid] IS NULL 
	UNION
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldFuelCard]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldFuelCard].[SiteGuid]
			INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
				ON [dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid] = data.[TransactionAliasGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid] IS NOT NULL 
				AND [dbo].[tblUserDataFieldFuelCard].[UserGroupGuid] IS NULL 
	UNION
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldFuelCard]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldFuelCard].[SiteGuid]
			INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
				ON [dbo].[tblUserDataFieldFuelCard].[UserGroupGuid] = data.[GroupGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid] IS NULL 
				AND [dbo].[tblUserDataFieldFuelCard].[UserGroupGuid] IS NOT NULL 
	UNION
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldFuelCard].[UserDataFieldFuelCardGuid],[dbo].[tblUserDataFieldFuelCard].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldFuelCard]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldFuelCard].[SiteGuid]
			INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
				ON [dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
				ON [dbo].[tblUserDataFieldFuelCard].[UserGroupGuid] = data1.[GroupGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldFuelCard].[TransactionAliasGuid] IS NOT NULL 
				AND [dbo].[tblUserDataFieldFuelCard].[UserGroupGuid] IS NOT NULL 
	)
	INSERT INTO @tblUserDataFieldFuelCardList SELECT UserDataToSiteGuid,UserDataFieldFuelCardGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldFuelCardList_CTE

	RETURN;
END