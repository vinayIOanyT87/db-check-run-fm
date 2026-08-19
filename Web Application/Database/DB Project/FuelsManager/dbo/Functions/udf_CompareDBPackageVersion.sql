CREATE FUNCTION [dbo].[udf_CompareDBPackageVersion]
(
	@PackageName NVARCHAR(MAX),
    @TargetVersion nvarchar(max),
    @Parts int = 4
)
RETURNS INT
AS
BEGIN
/*
-1 : target has higher version number (later version)
0 : same
1 : source has higher version number (later version)
*/ 
    DECLARE @ReturnValue as int = 0;
    DECLARE @PartIndex as int = 1;
    DECLARE @SourcePartValue as int = 0;
    DECLARE @TargetPartValue as int = 0;
	DECLARE @SourceVersion nvarchar(max)

	select TOP 1 @SourceVersion = [Version] from tblVersion where PackageName = @PackageName ORDER BY DateApplied DESC
    
	IF (@SourceVersion IS NULL)
	BEGIN
		SET @ReturnValue = -1;
	END

    WHILE (@PartIndex <= @Parts AND @ReturnValue = 0)
    BEGIN
        SET @SourcePartValue = [dbo].[udf_VersionNthPart](@SourceVersion, @PartIndex);
        SET @TargetPartValue = [dbo].[udf_VersionNthPart](@TargetVersion, @PartIndex);
        IF @SourcePartValue > @TargetPartValue
            SET @ReturnValue = 1
        ELSE IF @SourcePartValue < @TargetPartValue
            SET @ReturnValue = -1
        SET @PartIndex = @PartIndex + 1;
    END
    RETURN @ReturnValue
END