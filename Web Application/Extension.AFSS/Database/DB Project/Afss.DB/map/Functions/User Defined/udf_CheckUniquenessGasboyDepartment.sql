CREATE FUNCTION [map].[udf_CheckUniquenessGasboyDepartment]
(
	@GasboyDepartmentGuid UNIQUEIDENTIFIER, 
    @SiteGuid UNIQUEIDENTIFIER
)
RETURNS BIT
AS
BEGIN
    DECLARE @DepartmentId BIGINT
	DECLARE @DepartmentName NVARCHAR(50)

	DECLARE @IsUnique BIT
	SET @IsUnique = 1

    SELECT @DepartmentId = DepartmentId, @DepartmentName = DepartmentName FROM [dbo].[tblGasboyDepartment] WHERE [dbo].[tblGasboyDepartment].[GasboyDepartmentGuid] = @GasboyDepartmentGuid

	IF 0 < (SELECT COUNT(*) 
                FROM [dbo].[tblGasboyDepartment] entity
		            RIGHT JOIN [map].[tblEntityGasboyDepartmentToSite] map
                        ON map.[GasboyDepartmentGuid] = @GasboyDepartmentGuid 
                            AND map.[SiteGuid] = entity.[SiteGuid]
		        WHERE entity.[GasboyDepartmentGuid] <> @GasboyDepartmentGuid 
                        AND (entity.[DepartmentId] = @DepartmentId OR entity.[DepartmentName] = @DepartmentName)
            )
	BEGIN
		SET @IsUnique = 0
	END

	RETURN @IsUnique
END
