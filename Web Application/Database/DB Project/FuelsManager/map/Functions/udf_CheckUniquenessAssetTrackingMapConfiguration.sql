CREATE FUNCTION [map].[udf_CheckUniquenessAssetTrackingMapConfiguration]
(@AssetTrackingMapConfigurationGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @MapName nvarchar(100), @Exists bit

	SET @Exists = 1
	SET @Mapname = (SELECT MapName FROM tblAssetTrackingMapConfiguration e WHERE e.AssetTrackingMapConfigurationGuid = @AssetTrackingMapConfigurationGuid)

	IF 0 < (SELECT COUNT(*) 
			FROM tblAssetTrackingMapConfiguration e 
				RIGHT JOIN map.tblEntityAssetTrackingMapConfigurationToSite em ON em.SiteGuid = @SiteGuid 
				AND em.AssetTrackingMapConfigurationGuid = e.AssetTrackingMapConfigurationGuid 
			WHERE e.AssetTrackingMapConfigurationGuid <> @AssetTrackingMapConfigurationGuid
			AND e.MapName = @MapName)
		SET @Exists = 0

	RETURN @Exists
END
