CREATE FUNCTION [dbo].[udf_GetAssignedQualificationEquipmentTestAndInspectionListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblQualificationsList TABLE
(
	[EquipmentTestAndInspectionToSiteGuid] [uniqueidentifier]
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
		SELECT [map].[tblEntityEquipmentTestAndInspectionToSite].[EquipmentTestAndInspectionToSiteGuid], [dbo].[tblQualifications].[QualificationGuid], [dbo].[tblQualifications].[SiteGuid] 'OwnerSiteGuid', [map].[tblEntityEquipmentTestAndInspectionToSite].[SiteGuid] 'AssignedToSiteGuid'
			FROM [map].[tblEntityEquipmentTestAndInspectionToSite]
				INNER JOIN [dbo].[tblQualifications]
					ON [map].[tblEntityEquipmentTestAndInspectionToSite].[QualificationGuid] = [dbo].[tblQualifications].[QualificationGuid]
			WHERE [map].[tblEntityEquipmentTestAndInspectionToSite].[SiteGuid] = @sync_context_site_guid

	RETURN;
END