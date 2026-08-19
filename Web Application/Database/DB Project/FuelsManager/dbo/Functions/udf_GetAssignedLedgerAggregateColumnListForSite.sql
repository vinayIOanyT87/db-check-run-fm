CREATE FUNCTION [dbo].[udf_GetAssignedLedgerAggregateColumnListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblLedgerAggregateColumnList TABLE
(
	[LedgerAggregateColumnToSiteGuid] [uniqueidentifier]
	,[LedgerAggregateColumnGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblLedgerAggregateColumnList 
		SELECT [map].[tblEntityLedgerAggregateColumnToSite].[LedgerAggregateColumnToSiteGuid], [dbo].[tblLedgerAggregateColumns].[LedgerAggregateColumnGuid],[dbo].[tblLedgerAggregateColumns].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityLedgerAggregateColumnToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityLedgerAggregateColumnToSite]
			INNER JOIN [dbo].[tblLedgerAggregateColumns]
				ON [map].[tblEntityLedgerAggregateColumnToSite].[LedgerAggregateColumnGuid] = [dbo].[tblLedgerAggregateColumns].[LedgerAggregateColumnGuid]
		WHERE ([map].[tblEntityLedgerAggregateColumnToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END