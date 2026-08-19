CREATE FUNCTION [dbo].[udf_GetAssignedMaintenanceReasonListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblMaintenanceReasonList TABLE
(
	[MaintenanceReasonToSiteGuid] [uniqueidentifier]
	,[MaintenanceReasonGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblMaintenanceReasonList 
		SELECT [map].[tblEntityMaintenanceReasonToSite].[MaintenanceReasonToSiteGuid], [dbo].[tblMaintenanceReasons].[MaintenanceReasonGuid],[dbo].[tblMaintenanceReasons].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityMaintenanceReasonToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityMaintenanceReasonToSite]
			INNER JOIN [dbo].[tblMaintenanceReasons]
				ON [map].[tblEntityMaintenanceReasonToSite].[MaintenanceReasonGuid] = [dbo].[tblMaintenanceReasons].[MaintenanceReasonGuid]
		WHERE ([map].[tblEntityMaintenanceReasonToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END