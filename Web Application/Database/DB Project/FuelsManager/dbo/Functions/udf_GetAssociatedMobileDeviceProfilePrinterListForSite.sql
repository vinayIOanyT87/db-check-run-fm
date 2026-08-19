CREATE FUNCTION [dbo].[udf_GetAssociatedMobileDeviceProfilePrinterListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblMobileDeviceProfileList TABLE
(
	[MobileDeviceProfilePrinterGuid] [uniqueidentifier]
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
		SELECT [dbo].[tblMobileDeviceProfilePrinter].[MobileDeviceProfilePrinterGuid],[map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileToSiteGuid], [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileGuid],[map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityMobileDeviceProfileToSite]
			INNER JOIN [dbo].[tblMobileDeviceProfilePrinter]
				ON [map].[tblEntityMobileDeviceProfileToSite].[MobileDeviceProfileGuid] = [dbo].[tblMobileDeviceProfilePrinter].[MobileDeviceProfileGuid]
		WHERE ([map].[tblEntityMobileDeviceProfileToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END