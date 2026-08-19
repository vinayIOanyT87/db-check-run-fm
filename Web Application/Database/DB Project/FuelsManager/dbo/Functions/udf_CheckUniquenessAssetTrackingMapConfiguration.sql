CREATE FUNCTION [dbo].[udf_CheckUniquenessAssetTrackingMapConfiguration]
(@AssetTrackingMapConfigurationGuid uniqueidentifier, @SiteGuid uniqueidentifier, @MapName nvarchar(100))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1

	IF 0 < (SELECT COUNT(*) 
			FROM tblAssetTrackingMapConfiguration e
				LEFT JOIN map.tblEntityAssetTrackingMapConfigurationToSite em1 ON em1.AssetTrackingMapConfigurationGuid = e.AssetTrackingMapConfigurationGuid
				RIGHT JOIN map.tblEntityAssetTrackingMapConfigurationToSite em2 ON em2.AssetTrackingMapConfigurationGuid = @AssetTrackingMapConfigurationGuid 
				AND em2.SiteGuid = em1.SiteGuid
			WHERE e.AssetTrackingMapConfigurationGuid <> @AssetTrackingMapConfigurationGuid
			AND MapName = @MapName)
		SET @Exists = 0

	RETURN @Exists
END