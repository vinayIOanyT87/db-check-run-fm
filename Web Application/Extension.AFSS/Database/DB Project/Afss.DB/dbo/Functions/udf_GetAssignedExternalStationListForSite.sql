CREATE FUNCTION [dbo].[udf_GetAssignedExternalStationListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblExternalStationList TABLE
(
	[ExternalStationToSiteGuid] [uniqueidentifier]
	,[ExternalStationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblExternalStationList 
		SELECT [map].[tblEntityExternalStationToSite].[ExternalStationToSiteGuid], [dbo].[tblExternalStation].[ExternalStationGuid],[dbo].[tblExternalStation].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityExternalStationToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityExternalStationToSite]
			INNER JOIN [dbo].[tblExternalStation]
				ON [map].[tblEntityExternalStationToSite].[ExternalStationGuid] = [dbo].[tblExternalStation].[ExternalStationGuid]
		WHERE ([map].[tblEntityExternalStationToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END