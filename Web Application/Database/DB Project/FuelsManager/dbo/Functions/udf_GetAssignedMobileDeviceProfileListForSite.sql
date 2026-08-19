CREATE FUNCTION [dbo].[udf_GetAssignedMobileDeviceProfileListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblMobileDeviceProfileList TABLE
(
	[MobileDeviceProfileToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileToSiteGuid], [dbo].[tblMobileDeviceProfile].[MobileDeviceProfileGuid],[dbo].[tblMobileDeviceProfile].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityMobileDeviceProfileToSite]
			INNER JOIN [dbo].[tblMobileDeviceProfile]
				ON [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileGuid] = [dbo].[tblMobileDeviceProfile].[MobileDeviceProfileGuid]
		WHERE ([map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END