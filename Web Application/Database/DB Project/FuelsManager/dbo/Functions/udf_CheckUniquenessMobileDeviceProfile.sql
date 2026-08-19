

CREATE FUNCTION [dbo].[udf_CheckUniquenessMobileDeviceProfile]
(@MobileDeviceProfileGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ProfileID nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblMobileDeviceProfile
	IF 0 < (SELECT COUNT(*) FROM tblMobileDeviceProfile e
	LEFT JOIN map.tblEntityMobileDeviceProfileToSite em1 ON em1.MobileDeviceProfileGuid = e.MobileDeviceProfileGuid
	RIGHT JOIN map.tblEntityMobileDeviceProfileToSite em2 ON em2.MobileDeviceProfileGuid = @MobileDeviceProfileGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.MobileDeviceProfileGuid <> @MobileDeviceProfileGuid
	AND ProfileID = @ProfileID)
		SET @Exists = 0

	RETURN @Exists
END
