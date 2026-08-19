CREATE FUNCTION [map].[udf_GetAssociatedGroupToTransactionAliasListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblGroupToTransactionAliasList TABLE
(
	[GroupToTransactionAliasGuid] [uniqueidentifier]
	,[UserGroupToSiteGuid] [uniqueidentifier]
	,[TransactionAliasToSiteGuid] [uniqueidentifier]
	,[GroupGuid] [uniqueidentifier]
	,[TransactionAliasGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- 
	--
	INSERT INTO @tblGroupToTransactionAliasList
		SELECT data.[GroupToTransactionAliasGuid],data.[UserGroupToSiteGuid],data.[TransactionAliasToSiteGuid],data.[GroupGuid],data.[TransactionAliasGuid],data.[OwnerSiteGuid],data.[AssignedToSiteGuid] 
			FROM (SELECT [map].[tblGroupToTransactionAlias].[GroupToTransactionAliasGuid],data1.[UserGroupToSiteGuid],data2.[TransactionAliasToSiteGuid],[map].[tblGroupToTransactionAlias].[GroupGuid],data2.[TransactionAliasGuid],data1.[OwnerSiteGuid],data1.[AssignedToSiteGuid]
					FROM [map].[tblGroupToTransactionAlias]
						INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
							ON [map].[tblGroupToTransactionAlias].[GroupGuid] = data1.[GroupGuid]
						INNER JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data2
							ON [map].[tblGroupToTransactionAlias].[TransactionAliasGuid] = data2.[TransactionAliasGuid]
				) data

	RETURN;
END