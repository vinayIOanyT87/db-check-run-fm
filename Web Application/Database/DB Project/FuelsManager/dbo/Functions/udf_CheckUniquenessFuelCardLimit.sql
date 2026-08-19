CREATE FUNCTION [dbo].[udf_CheckUniquenessFuelCardLimit]
(
	@FuelCardLimitGuid UNIQUEIDENTIFIER, 
	@ID NVARCHAR(50)
)
RETURNS BIT
AS
BEGIN
	DECLARE @Exists BIT
	SET @Exists = 1

	IF 0 < (SELECT COUNT(*) FROM tblFuelCardLimit
	LEFT JOIN map.tblEntityFuelCardLimitToSite em1 ON em1.FuelCardLimitGuid = tblFuelCardLimit.FuelCardLimitGuid
	RIGHT JOIN map.tblEntityFuelCardLimitToSite em2 ON em2.FuelCardLimitGuid = @FuelCardLimitGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE tblFuelCardLimit.FuelCardLimitGuid <> @FuelCardLimitGuid AND tblFuelCardLimit.ID = @ID)
	BEGIN
		SET @Exists = 0
	END

	RETURN @Exists
END

