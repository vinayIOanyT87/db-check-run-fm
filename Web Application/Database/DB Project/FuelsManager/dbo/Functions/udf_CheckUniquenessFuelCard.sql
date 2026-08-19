
CREATE FUNCTION [dbo].[udf_CheckUniquenessFuelCard]
(@FuelCardGuid uniqueidentifier, @SiteGuid uniqueidentifier, @ID nvarchar(50))
RETURNS BIT
AS
BEGIN
	DECLARE @Exists bit
	SET @Exists = 1


	-- dbo.tblFuelCard
	IF 0 < (SELECT COUNT(*) FROM tblFuelCards e
	LEFT JOIN map.tblEntityFuelCardToSite em1 ON em1.FuelCardGuid = e.FuelCardGuid
	RIGHT JOIN map.tblEntityFuelCardToSite em2 ON em2.FuelCardGuid = @FuelCardGuid AND em2.SiteGuid = em1.SiteGuid
	WHERE e.FuelCardGuid <> @FuelCardGuid
	AND ID = @ID)
		SET @Exists = 0

	RETURN @Exists
END

