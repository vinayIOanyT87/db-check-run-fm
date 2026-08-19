CREATE FUNCTION [dbo].[udf_GetAssignedGasboyFleetListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblGasboyFleetList TABLE
(
	[GasboyFleetToSiteGuid] [uniqueidentifier]
	,[GasboyFleetGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblGasboyFleetList 
		SELECT [map].[tblEntityGasboyFleetToSite].[GasboyFleetToSiteGuid], [dbo].[tblGasboyFleet].[GasboyFleetGuid],[dbo].[tblGasboyFleet].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityGasboyFleetToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityGasboyFleetToSite]
			INNER JOIN [dbo].[tblGasboyFleet]
				ON [map].[tblEntityGasboyFleetToSite].[GasboyFleetGuid] = [dbo].[tblGasboyFleet].[GasboyFleetGuid]
		WHERE ([map].[tblEntityGasboyFleetToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END