CREATE FUNCTION [map].[udf_CheckUniquenessExternalStation]
(
	@ExternalStationGuid UNIQUEIDENTIFIER, 
	@SiteGuid UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
	DECLARE @ExternalStationID NVARCHAR(50), @Exists BIT = 1

	SET @ExternalStationID = (SELECT ID FROM tblExternalStation WHERE tblExternalStation.ExternalStationGuid = @ExternalStationGuid)

	IF 0 < (SELECT COUNT(*) 
                FROM [dbo].[tblExternalStation] entity
		            RIGHT JOIN [map].[tblEntityExternalStationToSite] map
                        ON map.[SiteGuid] = @SiteGuid 
                            AND map.[ExternalStationGuid] = entity.[ExternalStationGuid]
		        WHERE entity.[ExternalStationGuid] <> @ExternalStationGuid
		                AND entity.[ID] = @ExternalStationID
		)
	BEGIN
		SET @Exists = 0
	END

	RETURN @Exists
END
