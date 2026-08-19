CREATE FUNCTION [map].[udf_CheckUniquenessFuelCardLimit]
(
	@FuelCardLimitGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
	DECLARE @FuelCardLimitID NVARCHAR(50), @Exists BIT = 1

	SET @FuelCardLimitID = (SELECT ID FROM tblFuelCardLimit WHERE tblFuelCardLimit.FuelCardLimitGuid = @FuelCardLimitGuid)

	IF 0 < (SELECT COUNT(*) FROM tblFuelCardLimit 
		RIGHT JOIN map.tblEntityFuelCardLimitToSite ON map.tblEntityFuelCardLimitToSite.SiteGuid = @SiteGuid AND map.tblEntityFuelCardLimitToSite.FuelCardLimitGuid = tblFuelCardLimit.FuelCardLimitGuid 
		WHERE tblFuelCardLimit.FuelCardLimitGuid <> @FuelCardLimitGuid
		AND tblFuelCardLimit.ID = @FuelCardLimitID
		)
	BEGIN
		SET @Exists = 0
	END

	RETURN @Exists
END
