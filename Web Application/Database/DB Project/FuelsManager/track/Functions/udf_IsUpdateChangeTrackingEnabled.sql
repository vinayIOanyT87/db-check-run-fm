CREATE FUNCTION [track].[udf_IsUpdateChangeTrackingEnabled](@BypassTrackingFlags int)
RETURNS bit
AS
BEGIN
	DECLARE @BypassChangeTracking bit
	
	SET @BypassChangeTracking = 0

	IF (@BypassTrackingFlags IS NOT NULL)
	BEGIN
		SELECT @BypassChangeTracking = 
				CASE (@BypassTrackingFlags & 0x02) WHEN 0x02 THEN 1 ELSE 0 END
	END

	IF (@BypassChangeTracking = 1)
		RETURN 0;

	RETURN 1;
END