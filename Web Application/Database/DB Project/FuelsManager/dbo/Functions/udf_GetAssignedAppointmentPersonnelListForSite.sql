CREATE FUNCTION [dbo].[udf_GetAssignedAppointmentPersonnelListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAppointmentPersonnelList TABLE
(
	[AppointmentPersonnelToSiteGuid] [uniqueidentifier]
	,[AppointmentPersonnelGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAppointmentPersonnelList 
		SELECT [map].[tblEntityAppointmentPersonnelToSite].[AppointmentPersonnelToSiteGuid], [dbo].[tblAppointmentPersonnel].[AppointmentPersonnelGuid],[dbo].[tblAppointmentPersonnel].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAppointmentPersonnelToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAppointmentPersonnelToSite]
			INNER JOIN [dbo].[tblAppointmentPersonnel]
				ON [map].[tblEntityAppointmentPersonnelToSite].[AppointmentPersonnelGuid] = [dbo].[tblAppointmentPersonnel].[AppointmentPersonnelGuid]
		WHERE ([map].[tblEntityAppointmentPersonnelToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END