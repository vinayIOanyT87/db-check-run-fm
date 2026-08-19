CREATE FUNCTION [dbo].[udf_GetAssociatedMeterListForSite](
	@sync_context_site_guid uniqueidentifier
)
RETURNS @tblMeterList TABLE
(
	[MeterGuid] [uniqueidentifier]
	,[EquipmentToSiteGuid] [uniqueidentifier]
	,[EquipmentGuid] [uniqueidentifier]
	,[OwnerSiteGuid] [uniqueidentifier]
	,[AssignedToSiteGuid] [uniqueidentifier]
)
AS
BEGIN
	--
	--
	INSERT INTO @tblMeterList 
		SELECT data.[MeterGuid],data.[EquipmentToSiteGuid],data.[EquipmentGuid],data.[OwnerSiteGuid],data.[AssignedToSiteGuid]
			FROM (SELECT m.[MeterGuid],equipList.[EquipmentToSiteGuid],equipList.[EquipmentGuid],equipList.[OwnerSiteGuid],equipList.[AssignedToSiteGuid]
					FROM [dbo].[tblMeter] m
						INNER JOIN [map].[tblMeterToEquipment] map
							ON map.[MeterGuid] = m.[MeterGuid]
								INNER JOIN (SELECT [EquipmentToSiteGuid],[EquipmentGuid],[OwnerSiteGuid],[AssignedToSiteGuid] FROM [dbo].[udf_GetAssignedEquipmentListForSite](@sync_context_site_guid)) equipList
									ON equipList.[EquipmentGuid] = map.[EquipmentGuid]
				) data
	RETURN;
END