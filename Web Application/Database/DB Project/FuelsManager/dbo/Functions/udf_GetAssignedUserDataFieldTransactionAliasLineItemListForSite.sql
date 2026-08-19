CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldTransactionAliasLineItemListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldTransactionAliasLineItemList TABLE
(
	[UserDataFieldTransactionAliasLineItemGuid] [uniqueidentifier]
	,[TransactionAliasToSiteGuid] [uniqueidentifier]
	,[UserGroupToSiteGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldTransactionAliasLineItemList_CTE (UserDataFieldTransactionAliasLineItemGuid,TransactionAliasToSiteGuid,UserGroupToSiteGuid,OwnerSiteGuid)
	AS (
		SELECT [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserDataFieldTransactionAliasLineItemGuid],data.[TransactionAliasToSiteGuid],NULL 'UserGroupToSiteGuid',[dbo].[tblUserDataFieldTransactionAliasLineItem].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldTransactionAliasLineItem].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			WHERE ([dbo].[tblUserDataFieldTransactionAliasLineItem].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserGroupGuid] IS NULL)
		UNION
		SELECT [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserDataFieldTransactionAliasLineItemGuid],NULL 'TransactionAliasToSiteGuid',data.[UserGroupToSiteGuid],[dbo].[tblUserDataFieldTransactionAliasLineItem].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([dbo].[tblUserDataFieldTransactionAliasLineItem].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserGroupGuid] IS NOT NULL)
		UNION
		SELECT [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserDataFieldTransactionAliasLineItemGuid],data.[TransactionAliasToSiteGuid],data1.[UserGroupToSiteGuid],[dbo].[tblUserDataFieldTransactionAliasLineItem].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblUserDataFieldTransactionAliasLineItem]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldTransactionAliasLineItem].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([dbo].[tblUserDataFieldTransactionAliasLineItem].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldTransactionAliasLineItem].[UserGroupGuid] IS NOT NULL) 
	)
	INSERT INTO @tblUserDataFieldTransactionAliasLineItemList SELECT UserDataFieldTransactionAliasLineItemGuid,TransactionAliasToSiteGuid,UserGroupToSiteGuid,OwnerSiteGuid FROM UserDataFieldTransactionAliasLineItemList_CTE

	RETURN;
END