CREATE FUNCTION [dbo].[udf_GetAssociatedStationListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblStationList TABLE
(
	[StationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- Stations belong to a site 
	--
	INSERT INTO @tblStationList 
		SELECT [dbo].[tblStations].[StationGuid],[dbo].[tblStations].[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblStations]
		WHERE [dbo].[tblStations].[SiteGuid] = @sync_context_site_guid 
	RETURN;
END