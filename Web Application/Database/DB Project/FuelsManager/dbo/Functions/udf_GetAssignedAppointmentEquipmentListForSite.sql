CREATE FUNCTION [dbo].[udf_GetAssignedAppointmentEquipmentListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAppointmentEquipmentList TABLE
(
	[AppointmentEquipmentToSiteGuid] [uniqueidentifier]
	,[AppointmentEquipmentGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAppointmentEquipmentList 
		SELECT [map].[tblEntityAppointmentEquipmentToSite].[AppointmentEquipmentToSiteGuid], [dbo].[tblAppointmentEquipment].[AppointmentEquipmentGuid],[dbo].[tblAppointmentEquipment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAppointmentEquipmentToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAppointmentEquipmentToSite]
			INNER JOIN [dbo].[tblAppointmentEquipment]
				ON [map].[tblEntityAppointmentEquipmentToSite].[AppointmentEquipmentGuid] = [dbo].[tblAppointmentEquipment].[AppointmentEquipmentGuid]
		WHERE ([map].[tblEntityAppointmentEquipmentToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END