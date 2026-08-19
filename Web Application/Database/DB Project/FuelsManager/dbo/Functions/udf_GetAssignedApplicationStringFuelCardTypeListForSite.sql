CREATE FUNCTION [dbo].[udf_GetAssignedApplicationStringFuelCardTypeListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblApplicationStringList TABLE
(
	[FuelCardTypeToSiteGuid] [uniqueidentifier]
	,[ApplicationStringGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblApplicationStringList 
		SELECT [map].[tblEntityFuelCardTypeToSite].[FuelCardTypeToSiteGuid],[dbo].[tblApplicationString].[ApplicationStringGuid],[dbo].[tblApplicationString].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityFuelCardTypeToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityFuelCardTypeToSite]
				INNER JOIN [dbo].[tblApplicationString]
					ON [map].[tblEntityFuelCardTypeToSite].[ApplicationStringGuid] = [dbo].[tblApplicationString].[ApplicationStringGuid]
			WHERE [map].[tblEntityFuelCardTypeToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END