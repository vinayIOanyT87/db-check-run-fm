
CREATE FUNCTION [dbo].[udf_GetAssignedUserDataFieldTransactionAliasListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblUserDataFieldTransactionAliasList TABLE
(
	[UserDataFieldTransactionAliasGuid] [uniqueidentifier]
	,[TransactionAliasToSiteGuid] [uniqueidentifier]
	,[UserGroupToSiteGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH UserDataFieldTransactionAliasList_CTE (UserDataFieldTransactionAliasGuid,TransactionAliasToSiteGuid,UserGroupToSiteGuid,OwnerSiteGuid)
	AS (
		SELECT [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],data.[TransactionAliasToSiteGuid],NULL 'UserGroupToSiteGuid',[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblUserDataFieldTransactionAlias]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid] = data.[TransactionAliasGuid]
			WHERE ([dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid] IS NULL)
		UNION
		SELECT [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],NULL 'TransactionAliasToSiteGuid',data.[UserGroupToSiteGuid],[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblUserDataFieldTransactionAlias]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid] = data.[GroupGuid]
			WHERE ([dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid] IS NULL 
					AND [dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid] IS NOT NULL)
		UNION
		SELECT [dbo].[tblUserDataFieldTransactionAlias].[UserDataFieldTransactionAliasGuid],data.[TransactionAliasToSiteGuid],data1.[UserGroupToSiteGuid],[dbo].[tblUserDataFieldTransactionAlias].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblUserDataFieldTransactionAlias]
				INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data
					ON [dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid] = data.[TransactionAliasGuid]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid] = data1.[GroupGuid]
			WHERE ([dbo].[tblUserDataFieldTransactionAlias].[TransactionAliasGuid] IS NOT NULL 
					AND [dbo].[tblUserDataFieldTransactionAlias].[UserGroupGuid] IS NOT NULL) 
	)
	INSERT INTO @tblUserDataFieldTransactionAliasList SELECT UserDataFieldTransactionAliasGuid,TransactionAliasToSiteGuid,UserGroupToSiteGuid,OwnerSiteGuid FROM UserDataFieldTransactionAliasList_CTE

	RETURN;
END