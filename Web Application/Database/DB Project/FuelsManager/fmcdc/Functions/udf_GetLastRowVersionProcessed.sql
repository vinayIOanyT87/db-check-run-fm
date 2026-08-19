/*
	DROP FUNCTION [fmcdc].[udf_GetLastRowVersionProcessed]
*/
CREATE FUNCTION [fmcdc].[udf_GetLastRowVersionProcessed]
(
	@EntityTypeName nvarchar(50)
)
RETURNS bigint
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [fmcdc].[udf_GetLastRowVersionProcessed]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the last RowVersion of a given fmcdc table (indicated by the EntityTypeName).
	-- Notes:	
	-- 1. @EntityTypeName: Name of the Entity for which an fmcdc table needs to be queried.
	------------------------------------------------------------------------------------------------------

	DECLARE @lastRowVersion bigint

	SELECT @lastRowVersion = LastRowVersionProcessed FROM fmcdc.tblLastRowVersionProcessed WHERE EntityName = @EntityTypeName
	
	RETURN @lastRowVersion;
END
GO