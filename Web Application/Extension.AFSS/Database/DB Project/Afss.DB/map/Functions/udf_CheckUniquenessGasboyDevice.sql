CREATE FUNCTION [map].[udf_CheckUniquenessGasboyDevice]
(
	@GasboyDeviceGuid UNIQUEIDENTIFIER, 
    @SiteGuid UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
    DECLARE @DeviceCode BIGINT
	DECLARE @DeviceName NVARCHAR(50)

	DECLARE @IsUnique BIT
	SET @IsUnique = 1

    SELECT @DeviceCode = DeviceCode, @DeviceName = DeviceName FROM [dbo].[tblGasboyDevice] WHERE [dbo].[tblGasboyDevice].[GasboyDeviceGuid] = @GasboyDeviceGuid

	IF 0 < (SELECT COUNT(*) 
                FROM [dbo].[tblGasboyDevice] entity
		            RIGHT JOIN [map].[tblEntityGasboyDeviceToSite] map
                        ON map.[OwnerSiteGuid] = @GasboyDeviceGuid AND map.[MapToSiteGuid] = entity.[SiteGuid]
		        WHERE entity.[GasboyDeviceGuid] <> @GasboyDeviceGuid 
                        AND (entity.[DeviceCode] = @DeviceCode OR entity.[DeviceName] = @DeviceName)
            )
	BEGIN
		SET @IsUnique = 0
	END

	RETURN @IsUnique
END