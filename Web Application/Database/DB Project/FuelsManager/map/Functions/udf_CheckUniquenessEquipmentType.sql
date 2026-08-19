

CREATE FUNCTION [map].[udf_CheckUniquenessEquipmentType]
(@EquipmentTypeGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @EqTypeName nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @EqTypeName = (SELECT EqTypeName FROM tblEquipmentTypes e WHERE e.EquipmentTypeGuid = @EquipmentTypeGuid)
	IF 0 < (SELECT COUNT(*) FROM tblEquipmentTypes e 
	RIGHT JOIN map.tblEntityEquipmentTypeToSite em ON em.SiteGuid = @SiteGuid AND em.EquipmentTypeGuid = e.EquipmentTypeGuid 
	WHERE e.EquipmentTypeGuid <> @EquipmentTypeGuid
	AND e.EqTypeName = @EqTypeName)
		SET @Exists = 0

	RETURN @Exists
END
