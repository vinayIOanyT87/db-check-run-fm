CREATE FUNCTION [map].[udf_CheckUniquenessGasboyDepartment]
(
    @GasboyDepartmentGuid UNIQUEIDENTIFIER, 
    @SiteGuid UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
    DECLARE @DepartmentCode BIGINT
    DECLARE @DepartmentName NVARCHAR(50)

    DECLARE @IsUnique BIT
    SET @IsUnique = 1

    SELECT @DepartmentCode = DepartmentCode, @DepartmentName = DepartmentName FROM [dbo].[tblGasboyDepartment] WHERE [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid] = @GasboyDepartmentGuid

    IF 0 < (SELECT COUNT(*) 
                FROM [dbo].[tblGasboyDepartment] entity
                    RIGHT JOIN [map].[tblEntityGasboyDepartmentToSite] map
                        ON map.[GasboyDepartmentGuid] = @GasboyDepartmentGuid 
                            AND map.[SiteGuid] = entity.[SiteGuid]
                WHERE entity.[GasboyDepartmentGuid] <> @GasboyDepartmentGuid 
                        AND (entity.[DepartmentCode] = @DepartmentCode OR entity.[DepartmentName] = @DepartmentName)
            )
    BEGIN
        SET @IsUnique = 0
    END

    RETURN @IsUnique
END
