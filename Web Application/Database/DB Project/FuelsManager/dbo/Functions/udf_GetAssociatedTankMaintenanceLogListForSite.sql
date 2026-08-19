CREATE FUNCTION [dbo].[udf_GetAssociatedTankMaintenanceLogListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblTankMaintenanceLogList TABLE
(
	[TankMaintenanceLogGuid] [uniqueidentifier]
	,[TankGuid] [uniqueidentifier]
	,[MaintenanceReasonGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblTankMaintenanceLogList 
		SELECT [TankMaintenanceLogGuid],[TankGuid],[MaintenanceReasonGuid],[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblTankMaintenanceLog]
			WHERE [dbo].[tblTankMaintenanceLog].[SiteGuid] = @sync_context_site_guid

	RETURN;
END