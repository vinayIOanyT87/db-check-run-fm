CREATE FUNCTION [map].[udf_GetAssociatedProductToLedgerViewListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblProductToLedgerViewList TABLE
(
	[ProductToLedgerViewGuid] [uniqueidentifier]
	,[ProductToSiteGuid] [uniqueidentifier]
	,[LedgerViewToSiteGuid] [uniqueidentifier]
	,[ProductGuid] [uniqueidentifier]
	,[AssignedToListViewGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	; WITH ProductToLedgerView_CTE ([ProductToLedgerViewGuid],[ProductToSiteGuid],[LedgerViewToSiteGuid],[ProductGuid],[AssignedToListViewGuid],[OwnerSiteGuid])
	AS (
		SELECT [map].[tblProductToLedgerView].[ProductToLedgerViewGuid],data1.[ProductToSiteGuid],data2.[LedgerViewToSiteGuid], [map].[tblProductToLedgerView].[ProductGuid],[map].[tblProductToLedgerView].[AssignedToListViewGuid],data1.[OwnerSiteGuid]
			FROM [map].[tblProductToLedgerView]
				INNER JOIN (SELECT [ProductToSiteGuid],[ProductGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedProductListForSite](@sync_context_site_guid)) data1
					ON [map].[tblProductToLedgerView].[ProductGuid] = data1.[ProductGuid]
				INNER JOIN (SELECT [LedgerViewToSiteGuid],[ListViewGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedLedgerViewListForSite](@sync_context_site_guid)) data2
					ON [map].[tblProductToLedgerView].[AssignedToListViewGuid] = data2.[ListViewGuid]
	)
	INSERT INTO @tblProductToLedgerViewList SELECT [ProductToLedgerViewGuid],[ProductToSiteGuid],[LedgerViewToSiteGuid],[ProductGuid],[AssignedToListViewGuid],[OwnerSiteGuid] FROM ProductToLedgerView_CTE

	RETURN;
END