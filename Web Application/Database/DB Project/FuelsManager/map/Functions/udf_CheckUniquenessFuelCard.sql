
CREATE FUNCTION [map].[udf_CheckUniquenessFuelCard]
(@FuelCardGuid uniqueidentifier, @SiteGuid uniqueidentifier)
RETURNS BIT
AS
BEGIN
	DECLARE @ID nvarchar(50)
			, @Exists bit
	SET @Exists = 1

	SET @ID = (SELECT ID FROM tblFuelCards e WHERE e.FuelCardGuid = @FuelCardGuid)
	IF 0 < (SELECT COUNT(*) FROM tblFuelCards e 
	RIGHT JOIN map.tblEntityFuelCardToSite em ON em.SiteGuid = @SiteGuid AND em.FuelCardGuid = e.FuelCardGuid 
	WHERE e.FuelCardGuid <> @FuelCardGuid
	AND e.ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END
