
CREATE FUNCTION [dbo].[udf_GetUnitAbbrev](@UnitIndex [int], @LangID [int])
RETURNS [nvarchar](100)
AS 
BEGIN
	DECLARE @abbrev NVARCHAR(100)
	SELECT @abbrev = EngineeringUnitAbbreviation
	  FROM lookup.tblEngineeringUnit
	 WHERE EngineeringUnitIndex = @UnitIndex
	 
	IF @abbrev IS NULL
		SET @abbrev = 'Undefined'
	
	RETURN @abbrev
END