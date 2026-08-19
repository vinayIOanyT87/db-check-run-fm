
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldEquipmentListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldEquipmentList TABLE
(
	[UserDataToSiteGuid] [uniqueidentifier]
	,[UserDataFieldEquipmentGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldEquipmentList_CTE (UserDataToSiteGuid,UserDataFieldEquipmentGuid,OwnerSiteGuid,AssignedToSiteGuid)
	AS (
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldEquipment].[UserDataFieldEquipmentGuid],[dbo].[tblUserDataFieldEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldEquipment]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldEquipment].[SiteGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid] IS NULL 
				AND [dbo].[tblUserDataFieldEquipment].[UserGroupGuid] IS NULL 
	UNION
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldEquipment].[UserDataFieldEquipmentGuid],[dbo].[tblUserDataFieldEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldEquipment]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldEquipment].[SiteGuid]
			INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
				ON [dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid] = data.[TransactionAliasGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid] IS NOT NULL 
				AND [dbo].[tblUserDataFieldEquipment].[UserGroupGuid] IS NULL 
	UNION
	SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldEquipment].[UserDataFieldEquipmentGuid],[dbo].[tblUserDataFieldEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldEquipment]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldEquipment].[SiteGuid]
			INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
				ON [dbo].[tblUserDataFieldEquipment].[UserGroupGuid] = data.[GroupGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid] IS NULL 
				AND [dbo].[tblUserDataFieldEquipment].[UserGroupGuid] IS NOT NULL 
	UNION
		SELECT [map].[tblEntityUserDataToSite].[UserDataToSiteGuid], [dbo].[tblUserDataFieldEquipment].[UserDataFieldEquipmentGuid],[dbo].[tblUserDataFieldEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityUserDataToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityUserDataToSite]
			INNER JOIN [dbo].[tblUserDataFieldEquipment]
				ON [map].[tblEntityUserDataToSite].[OwnerSiteGuid] = [dbo].[tblUserDataFieldEquipment].[SiteGuid]
			INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
				ON [dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
				ON [dbo].[tblUserDataFieldEquipment].[UserGroupGuid] = data1.[GroupGuid]
		WHERE ([map].[tblEntityUserDataToSite].[MapToSiteGuid] = @sync_context_site_guid)
				AND [dbo].[tblUserDataFieldEquipment].[TransactionAliasGuid] IS NOT NULL 
				AND [dbo].[tblUserDataFieldEquipment].[UserGroupGuid] IS NOT NULL 
	)
	INSERT INTO @tblUserDataFieldEquipmentList SELECT UserDataToSiteGuid,UserDataFieldEquipmentGuid,OwnerSiteGuid,AssignedToSiteGuid FROM UserDataFieldEquipmentList_CTE

	RETURN;
END