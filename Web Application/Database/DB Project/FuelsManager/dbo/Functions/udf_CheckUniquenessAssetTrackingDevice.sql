CREATE FUNCTION [dbo].[udf_CheckUniquenessAssetTrackingDevice]
(@AssetTrackingDeviceGuid uniqueidentifier, @SiteGuid uniqueidentifier, @DeviceID nvarchar(30))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1

	IF 0 < (SELECT COUNT(*) 
			FROM tblAssetTrackingDevice e
				LEFT JOIN map.tblEntityAssetTrackingDeviceToSite em1 ON em1.AssetTrackingDeviceGuid = e.AssetTrackingDeviceGuid
				RIGHT JOIN map.tblEntityAssetTrackingDeviceToSite em2 ON em2.AssetTrackingDeviceGuid = @AssetTrackingDeviceGuid 
				AND em2.SiteGuid = em1.SiteGuid
			WHERE e.AssetTrackingDeviceGuid <> @AssetTrackingDeviceGuid
			AND DeviceID = @DeviceID)
		SET @Exists = 0

	RETURN @Exists
END
