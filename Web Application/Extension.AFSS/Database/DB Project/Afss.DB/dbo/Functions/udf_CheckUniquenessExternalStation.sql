CREATE FUNCTION [dbo].[udf_CheckUniquenessExternalStation]
(
	@ExternalStationGuid UNIQUEIDENTIFIER, 
	@ID NVARCHAR(50)
)
RETURNS BIT
AS
BEGIN
	DECLARE @Exists BIT
	SET @Exists = 1

	IF 0 < (SELECT COUNT(*) FROM tblExternalStation
		LEFT JOIN map.tblEntityExternalStationToSite em1 ON em1.ExternalStationGuid = tblExternalStation.ExternalStationGuid
		RIGHT JOIN map.tblEntityExternalStationToSite em2 ON em2.ExternalStationGuid = @ExternalStationGuid AND em2.SiteGuid = em1.SiteGuid
		WHERE tblExternalStation.ExternalStationGuid <> @ExternalStationGuid AND tblExternalStation.ID = @ID)
	BEGIN
		SET @Exists = 0
	END

	RETURN @Exists
END
