

CREATE FUNCTION [map].[udf_CheckUniquenessEquipment]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(30)
			, @CompanyGuid uniqueidentifier
			, @CompanyEquipmentID nvarchar(30)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblEquipment e WHERE e.EquipmentGuid = @_MasterRecordGuid)
	SET @CompanyGuid = (SELECT CompanyGuid FROM tblEquipment e WHERE e.EquipmentGuid = @_MasterRecordGuid)
	SET @CompanyEquipmentID = (SELECT CompanyEquipmentID FROM tblEquipment e WHERE e.EquipmentGuid = @_MasterRecordGuid)
	IF 0 < (SELECT COUNT(*) FROM tblEquipment e 
	RIGHT JOIN map.tblEntityEquipmentToSite em ON em.SiteGuid = @SiteGuid AND em.EquipmentGuid = e._MasterRecordGuid 
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND (e.ID = @ID
	OR (@CompanyGuid IS NOT NULL
	AND ISNULL(CompanyEquipmentID,'') <> ''
	AND ISNULL(@CompanyEquipmentID,'') <> ''
	AND CompanyEquipmentID = @CompanyEquipmentID)))
		SET @Exists = 0

	RETURN @Exists
END