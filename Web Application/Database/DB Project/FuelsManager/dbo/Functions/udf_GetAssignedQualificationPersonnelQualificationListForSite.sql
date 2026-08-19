CREATE FUNCTION [dbo].[udf_GetAssignedQualificationPersonnelQualificationListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQualificationsList TABLE
(
	[PersonnelQualificationToSiteGuid] [uniqueidentifier]
	,[QualificationGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblQualificationsList 
		SELECT [map].[tblEntityPersonnelQualificationToSite].[PersonnelQualificationToSiteGuid], [dbo].[tblQualifications].[QualificationGuid], [dbo].[tblQualifications].[SiteGuid] 'OwnerSiteGuid', [map].[tblEntityPersonnelQualificationToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityPersonnelQualificationToSite]
				INNER JOIN [dbo].[tblQualifications]
					ON [map].[tblEntityPersonnelQualificationToSite].[QualificationGuid] = [dbo].[tblQualifications].[QualificationGuid]
			WHERE [map].[tblEntityPersonnelQualificationToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END