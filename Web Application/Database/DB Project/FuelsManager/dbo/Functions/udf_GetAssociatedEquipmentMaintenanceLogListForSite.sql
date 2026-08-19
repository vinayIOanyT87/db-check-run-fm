CREATE FUNCTION [dbo].[udf_GetAssociatedEquipmentMaintenanceLogListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblEquipmentMaintenanceLogList TABLE
(
	[EquipmentMaintenanceLogGuid] [uniqueidentifier]
	,[EquipmentGuid] [uniqueidentifier]
	,[MaintenanceReasonGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblEquipmentMaintenanceLogList 
		SELECT [EquipmentMaintenanceLogGuid],[EquipmentGuid],[MaintenanceReasonGuid],[SiteGuid] 'OwnerSiteGuid'
			FROM [dbo].[tblEquipmentMaintenanceLog]
			WHERE [dbo].[tblEquipmentMaintenanceLog].[SiteGuid] = @sync_context_site_guid

	RETURN;
END