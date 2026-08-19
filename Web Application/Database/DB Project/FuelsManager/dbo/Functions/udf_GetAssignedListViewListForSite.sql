CREATE FUNCTION [dbo].[udf_GetAssignedListViewListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblListViewList TABLE
(
	[ListViewToSiteGuid] [uniqueidentifier]
	,[ListViewGuid] [uniqueidentifier]
	,[LedgerAggregateColumnGuid] [uniqueidentifier]
	,[TransactionAliasGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	; WITH EntityListViewToSiteList_CTE ([ListViewToSiteGuid],[ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid])
	AS (
		SELECT [map].[tblEntityListViewToSite].[ListViewToSiteGuid],[dbo].[tblListViews].[ListViewGuid],[dbo].[tblListViews].[LedgerAggregateColumnGuid],[dbo].[tblListViews].TransactionAliasGuid,[dbo].[tblListViews].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityListViewToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityListViewToSite]
				INNER JOIN [dbo].[tblListViews]
					ON [map].[tblEntityListViewToSite].[ListViewGuid] = [dbo].[tblListViews].[ListViewGuid]
				LEFT JOIN [dbo].[tblListViews] B
					ON [map].[tblEntityListViewToSite].[ListViewGuid] = B.[ListViewGuid]
				LEFT JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data1
					ON B.[TransactionAliasGuid] = data1.[TransactionAliasGuid]
				LEFT JOIN [dbo].[tblListViews] C
					ON [map].[tblEntityListViewToSite].[ListViewGuid] = C.[ListViewGuid]
				LEFT JOIN (SELECT [LedgerAggregateColumnToSiteGuid],[LedgerAggregateColumnGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedLedgerAggregateColumnListForSite](@sync_context_site_guid)) data2
					ON [dbo].[tblListViews].[LedgerAggregateColumnGuid] = data2.[LedgerAggregateColumnGuid]
			WHERE ([map].[tblEntityListViewToSite].[SiteGuid] = @sync_context_site_guid)
					AND ([dbo].[tblListViews].[TransactionAliasGuid] IS NULL OR B.[TransactionAliasGuid] IS NOT NULL)
					AND ([dbo].[tblListViews].[LedgerAggregateColumnGuid] IS NULL OR C.[LedgerAggregateColumnGuid] IS NOT NULL)
	)
	INSERT INTO @tblListViewList SELECT [ListViewToSiteGuid],[ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM EntityListViewToSiteList_CTE

	RETURN;
END