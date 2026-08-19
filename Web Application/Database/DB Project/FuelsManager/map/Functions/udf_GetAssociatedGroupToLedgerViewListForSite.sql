CREATE FUNCTION [map].[udf_GetAssociatedGroupToLedgerViewListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblGroupToLedgerViewList TABLE
(
	[GroupToLedgerViewGuid] [uniqueidentifier]
	,[UserGroupToSiteGuid] [uniqueidentifier]
	,[LedgerViewToSiteGuid] [uniqueidentifier]
	,[GroupGuid] [uniqueidentifier]
	,[ListViewGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- 
	--
	; WITH GroupToLedgerView_CTE ([GroupToLedgerViewGuid],[UserGroupToSiteGuid],[LedgerViewToSiteGuid],[GroupGuid],[ListViewGuid],[OwnerSiteGuid])
	AS (
		SELECT [map].[tblGroupToLedgerView].[GroupToLedgerViewGuid],data1.[UserGroupToSiteGuid],data2.[LedgerViewToSiteGuid],[map].[tblGroupToLedgerView].[GroupGuid],[map].[tblGroupToLedgerView].[ListViewGuid],data1.[OwnerSiteGuid] 'OwnerSiteGuid'
			FROM [map].[tblGroupToLedgerView]
				INNER JOIN (SELECT [UserGroupToSiteGuid],[GroupGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedGroupListForSite](@sync_context_site_guid)) data1
					ON [map].[tblGroupToLedgerView].[GroupGuid] = data1.[GroupGuid]
				INNER JOIN (SELECT [LedgerViewToSiteGuid],[ListViewGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedLedgerViewListForSite](@sync_context_site_guid)) data2
					ON [map].[tblGroupToLedgerView].[ListViewGuid] = data2.[ListViewGuid]
	)
	INSERT INTO @tblGroupToLedgerViewList SELECT [GroupToLedgerViewGuid],[UserGroupToSiteGuid],[LedgerViewToSiteGuid],[GroupGuid],[ListViewGuid],[OwnerSiteGuid] FROM GroupToLedgerView_CTE

	RETURN;
END