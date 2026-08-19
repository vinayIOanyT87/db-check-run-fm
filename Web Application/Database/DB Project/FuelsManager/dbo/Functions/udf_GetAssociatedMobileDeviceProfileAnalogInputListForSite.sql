CREATE FUNCTION [dbo].[udf_GetAssociatedMobileDeviceProfileAnalogInputListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblMobileDeviceProfileList TABLE
(
	[MobileDeviceProfileAnalogInputGuid] [uniqueidentifier]
	,[MobileDeviceProfileToSiteGuid] [uniqueidentifier]
	,[MobileDeviceProfileGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblMobileDeviceProfileList 
		SELECT [dbo].[tblMobileDeviceProfileAnalogInput].[MobileDeviceProfileAnalogInputGuid],[map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileToSiteGuid], [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileGuid],[map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityMobileDeviceProfileToSite]
			INNER JOIN [dbo].[tblMobileDeviceProfileAnalogInput]
				ON [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileGuid] = [dbo].[tblMobileDeviceProfileAnalogInput].[MobileDeviceProfileGuid]
		WHERE ([map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END