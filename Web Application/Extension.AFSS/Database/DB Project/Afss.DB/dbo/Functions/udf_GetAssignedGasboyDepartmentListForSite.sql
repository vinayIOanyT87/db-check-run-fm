CREATE FUNCTION [dbo].[udf_GetAssignedGasboyDepartmentListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblGasboyDepartmentList TABLE
(
	[GasboyDepartmentToSiteGuid] [uniqueidentifier]
	,[GasboyDepartmentGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblGasboyDepartmentList 
		SELECT [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentToSiteGuid], [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid],[dbo].[tblGasboyDepartment].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityGasboyDepartmentToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityGasboyDepartmentToSite]
			INNER JOIN [dbo].[tblGasboyDepartment]
				ON [map].[tblEntityGasboyDepartmentToSite].[GasboyDepartmentGuid] = [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid]
		WHERE ([map].[tblEntityGasboyDepartmentToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END