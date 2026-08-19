CREATE FUNCTION [dbo].[udf_GetAssignedEquipmentTypeListForSite](
@sync_context_site_guid uniqueidentifier
)
RETURNS @tblEquipmentTypeList TABLE
(
	[EquipmentTypeToSiteGuid] [uniqueidentifier]
	,[EquipmentTypeGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN

	-- In the case of entity assignment, the current model always contains a self-assigned entity assignment back to the owning site so we can leverage this to find
	-- all assigned entities.
	--
	INSERT INTO @tblEquipmentTypeList 
		SELECT [map].[tblEntityEquipmentTypeToSite].[EquipmentTypeToSiteGuid], [dbo].[tblEquipmentTypes].[EquipmentTypeGuid],[dbo].[tblEquipmentTypes].[SiteGuid] 'OwnerSiteGuid',[map].[tblEntityEquipmentTypeToSite].[SiteGuid] 'AssignedToSiteGuid'
		FROM [map].[tblEntityEquipmentTypeToSite]
			INNER JOIN [dbo].[tblEquipmentTypes]
				ON [map].[tblEntityEquipmentTypeToSite].[EquipmentTypeGuid] = [dbo].[tblEquipmentTypes].[EquipmentTypeGuid]
		WHERE ([map].[tblEntityEquipmentTypeToSite].[SiteGuid] = @sync_context_site_guid)

	RETURN;
END