CREATE FUNCTION [dbo].[udf_GetAssignedGasboyDeviceListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblGasboyDeviceList TABLE
(
	[GasboyDeviceToSiteGuid] [uniqueidentifier]
	,[GasboyDeviceGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblGasboyDeviceList 
		SELECT [map].[tblEntityGasboyDeviceToSite].[GasboyDeviceToSiteGuid], [dbo].[tblGasboyDevice].[GasboyDeviceGuid],[dbo].[tblGasboyDevice].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityGasboyDeviceToSite].[MapToSiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityGasboyDeviceToSite]
			INNER JOIN [dbo].[tblGasboyDevice]
				ON [map].[tblEntityGasboyDeviceToSite].[OwnerSiteGuid] = [dbo].[tblGasboyDevice].[SiteGuid]
		WHERE ([map].[tblEntityGasboyDeviceToSite].[MapToSiteGuid] = @sync_context_site_guid)

	RETURN;
END