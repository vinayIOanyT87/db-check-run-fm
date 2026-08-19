

CREATE FUNCTION [dbo].[udf_CheckUniquenessEquipmentType]
(@EquipmentTypeGuid uniqueidentifier, @SiteGuid uniqueidentifier, @EqTypeName nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblEquipmentType
	IF 0 < (SELECT COUNT(*) FROM tblEquipmentTypes e
	LEFT JOIN map.tblEntityEquipmentTypeToSite em1 ON em1.EquipmentTypeGuid = e.EquipmentTypeGuid
	RIGHT JOIN map.tblEntityEquipmentTypeToSite em2 ON em2.EquipmentTypeGuid = @EquipmentTypeGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.EquipmentTypeGuid <> @EquipmentTypeGuid
	AND EqTypeName = @EqTypeName)
		SET @Exists = 0

	RETURN @Exists
END
