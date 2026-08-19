CREATE FUNCTION [dbo].[udf_GetAssignedQualificationEquipmentTagAndLicenseListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQualificationsList TABLE
(
	[EquipmentTagAndLicenseToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityEquipmentTagAndLicenseToSite].[EquipmentTagAndLicenseToSiteGuid], [dbo].[tblQualifications].[QualificationGuid], [dbo].[tblQualifications].[SiteGuid] 'OwnerSiteGuid', [map].[tblEntityEquipmentTagAndLicenseToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityEquipmentTagAndLicenseToSite]
				INNER JOIN [dbo].[tblQualifications]
					ON [map].[tblEntityEquipmentTagAndLicenseToSite].[QualificationGuid] = [dbo].[tblQualifications].[QualificationGuid]
			WHERE [map].[tblEntityEquipmentTagAndLicenseToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END