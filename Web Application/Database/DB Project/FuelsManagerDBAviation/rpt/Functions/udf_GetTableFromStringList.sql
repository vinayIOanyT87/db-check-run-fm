CREATE FUNCTION [rpt].[udf_GetTableFromStringList]
(
	@List NVARCHAR(MAX)
)
RETURNS @tblGuids TABLE
(
	Guid UNIQUEIDENTIFIER
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [rpt].[udf_GetTableFromStringList] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to returns Table of Guids from string list.
	-- Notes:
	-- 1. @List: List of strigified, comma separated, Guids.
	-- 2014-Jan-28.  @List became nvarchar(max) because inputs exceeded 4000 char.   Changed logic to use
	--               iterative method because recursion is limited to 100 deep.
	------------------------------------------------------------------------------------------------------

	DECLARE @start INT
	DECLARE @nextComma INT
	DECLARE @len INT
	SET @start = 1

	SET @List = ISNULL(@List, '') + ','
	SET @len = LEN(@List)
	WHILE( @start < @len)
	BEGIN
		SET @nextComma = CHARINDEX( ',', @List, @start)
		INSERT INTO @tblGuids (Guid) 
			SELECT  CAST( SUBSTRING( @List, @start, @nextComma - @start) AS UNIQUEIDENTIFIER)
		SET @start = @nextComma + 1
	END

	RETURN;
END