

CREATE FUNCTION [map].[udf_CheckUniquenessMobileDeviceProfile]
(@MobileDeviceProfileGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ProfileID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @ProfileID = (SELECT ProfileID FROM tblMobileDeviceProfile e WHERE e.MobileDeviceProfileGuid = @MobileDeviceProfileGuid)
	IF 0 < (SELECT COUNT(*) FROM tblMobileDeviceProfile e 
	RIGHT JOIN map.tblEntityMobileDeviceProfileToSite em ON em.SiteGuid = @SiteGuid AND em.MobileDeviceProfileGuid = e.MobileDeviceProfileGuid 
	WHERE e.MobileDeviceProfileGuid <> @MobileDeviceProfileGuid
	AND e.ProfileID = @ProfileID)
		SET @Exists = 0

	RETURN @Exists
END
