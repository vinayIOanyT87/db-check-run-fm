CREATE FUNCTION [dbo].[udf_GetAssignedLedgerViewListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblLedgerViewList TABLE
(
	[LedgerViewToSiteGuid] [uniqueidentifier]
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
	; WITH EntityLedgerViewToSiteList_CTE ([LedgerViewToSiteGuid],[ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid])
	AS (
		SELECT [map].[tblEntityLedgerViewToSite].[LedgerViewToSiteGuid],[dbo].[tblListViews].[ListViewGuid],[dbo].[tblListViews].[LedgerAggregateColumnGuid],[dbo].[tblListViews].TransactionAliasGuid,[dbo].[tblListViews].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityLedgerViewToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityLedgerViewToSite]
				INNER JOIN [dbo].[tblListViews]
					ON [map].[tblEntityLedgerViewToSite].[ListViewGuid] = [dbo].[tblListViews].[ListViewGuid]
				LEFT JOIN [dbo].[tblListViews] B
					ON [map].[tblEntityLedgerViewToSite].[ListViewGuid] = B.[ListViewGuid]
				LEFT JOIN (SELECT [TransactionAliasToSiteGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedTransactionAliasListForSite](@sync_context_site_guid)) data1
					ON B.[TransactionAliasGuid] = data1.[TransactionAliasGuid]
				LEFT JOIN [dbo].[tblListViews] C
					ON [map].[tblEntityLedgerViewToSite].[ListViewGuid] = C.[ListViewGuid]
				LEFT JOIN (SELECT [LedgerAggregateColumnToSiteGuid],[LedgerAggregateColumnGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedLedgerAggregateColumnListForSite](@sync_context_site_guid)) data2
					ON [dbo].[tblListViews].[LedgerAggregateColumnGuid] = data2.[LedgerAggregateColumnGuid]
			WHERE ([map].[tblEntityLedgerViewToSite].[SiteGuid] = @sync_context_site_guid)
					AND ([dbo].[tblListViews].[TransactionAliasGuid] IS NULL OR B.[TransactionAliasGuid] IS NOT NULL)
					AND ([dbo].[tblListViews].[LedgerAggregateColumnGuid] IS NULL OR C.[LedgerAggregateColumnGuid] IS NOT NULL)
	)
	INSERT INTO @tblLedgerViewList SELECT [LedgerViewToSiteGuid],[ListViewGuid],[LedgerAggregateColumnGuid],[TransactionAliasGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM EntityLedgerViewToSiteList_CTE

	RETURN;
END