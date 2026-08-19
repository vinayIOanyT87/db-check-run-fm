CREATE FUNCTION [map].[udf_CheckUniquenessGasboyFleet]
(
	@GasboyFleetGuid UNIQUEIDENTIFIER, 
    @SiteGuid UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
    DECLARE @FleetId BIGINT
	DECLARE @FleetName NVARCHAR(50)

	DECLARE @IsUnique BIT
	SET @IsUnique = 1

    SELECT @FleetId = FleetId, @FleetName = FleetName FROM [dbo].[tblGasboyFleet] WHERE [dbo].[tblGasboyFleet].[GasboyFleetGuid] = @GasboyFleetGuid

	IF 0 < (SELECT COUNT(*) 
                FROM [dbo].[tblGasboyFleet] entity
		            RIGHT JOIN [map].[tblEntityGasboyFleetToSite] map
                        ON map.[SiteGuid] = @SiteGuid 
                            AND map.[GasboyFleetGuid] = entity.[GasboyFleetGuid]
		        WHERE entity.[GasboyFleetGuid] <> @GasboyFleetGuid
		                AND (entity.[FleetId] = @FleetId OR entity.[FleetName] = @FleetName)
		)
	BEGIN
		SET @IsUnique = 0
	END

	RETURN @IsUnique
END
