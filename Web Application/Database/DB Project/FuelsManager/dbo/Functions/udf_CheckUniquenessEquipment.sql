

CREATE FUNCTION [dbo].[udf_CheckUniquenessEquipment]
(@_MasterRecordGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(30), @CompanyGuid uniqueidentifier, @CompanyEquipmentID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblEquipment
	IF 0 < (SELECT COUNT(*) FROM tblEquipment e
	LEFT JOIN map.tblEntityEquipmentToSite em1 ON em1.EquipmentGuid = e._MasterRecordGuid
	RIGHT JOIN map.tblEntityEquipmentToSite em2 ON em2.EquipmentGuid = @_MasterRecordGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e._MasterRecordGuid <> @_MasterRecordGuid
	AND (ID = @ID
	OR (@CompanyGuid IS NOT NULL
	AND ISNULL(CompanyEquipmentID,'') <> ''
	AND ISNULL(@CompanyEquipmentID,'') <> ''
	AND CompanyEquipmentID = @CompanyEquipmentID and CompanyGuid = @CompanyGuid)))
		SET @Exists = 0

	RETURN @Exists
END
