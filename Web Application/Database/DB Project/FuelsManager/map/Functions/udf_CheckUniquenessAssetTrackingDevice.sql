CREATE FUNCTION [map].[udf_CheckUniquenessAssetTrackingDevice]
(@AssetTrackingDeviceGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @DeviceID nvarchar(30), @Exists bit

	SET @Exists = 1
	SET @DeviceID = (SELECT DeviceID FROM tblAssetTrackingDevice e WHERE e.AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid)

	IF 0 < (SELECT COUNT(*) 
			FROM tblAssetTrackingDevice e 
				RIGHT JOIN map.tblEntityAssetTrackingDeviceToSite em ON em.SiteGuid = @SiteGuid 
				AND em.AssetTrackingDeviceGuid = e.AssetTrackingDeviceGuid 
			WHERE e.AssetTrackingDeviceGuid <> @AssetTrackingDeviceGuid
			AND e.DeviceID = @DeviceID)
		SET @Exists = 0

	RETURN @Exists
END
