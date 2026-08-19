CREATE FUNCTION [dbo].[udf_GetAssignedQualificationPersonnelTrainingListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQualificationsList TABLE
(
	[PersonnelTrainingToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityPersonnelTrainingToSite].[PersonnelTrainingToSiteGuid], [dbo].[tblQualifications].[QualificationGuid], [dbo].[tblQualifications].[SiteGuid] 'OwnerSiteGuid', [map].[tblEntityPersonnelTrainingToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityPersonnelTrainingToSite]
				INNER JOIN [dbo].[tblQualifications]
					ON [map].[tblEntityPersonnelTrainingToSite].[QualificationGuid] = [dbo].[tblQualifications].[QualificationGuid]
			WHERE [map].[tblEntityPersonnelTrainingToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END