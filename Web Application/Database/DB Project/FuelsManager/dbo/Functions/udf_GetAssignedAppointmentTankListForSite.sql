CREATE FUNCTION [dbo].[udf_GetAssignedAppointmentTankListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblAppointmentTankList TABLE
(
	[AppointmentTankToSiteGuid] [uniqueidentifier]
	,[AppointmentTankGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblAppointmentTankList 
		SELECT [map].[tblEntityAppointmentTankToSite].[AppointmentTankToSiteGuid], [dbo].[tblAppointmentTank].[AppointmentTankGuid],[dbo].[tblAppointmentTank].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityAppointmentTankToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityAppointmentTankToSite]
			INNER JOIN [dbo].[tblAppointmentTank]
				ON [map].[tblEntityAppointmentTankToSite].[AppointmentTankGuid] = [dbo].[tblAppointmentTank].[AppointmentTankGuid]
		WHERE ([map].[tblEntityAppointmentTankToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END