

CREATE FUNCTION [rpt].[udf_GetIntTableFromStringList]
(
	@List NVARCHAR(MAX)
)
RETURNS @tblNums TABLE
(
	Num INT
)
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [rpt].[udf_GetIntTableFromStringList] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Function to returns Table of integers from string list.
	-- Notes:
	-- 1. @List: List of strigified, comma separated, ints.
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
		INSERT INTO @tblNums (Num) 
			SELECT  CAST( SUBSTRING( @List, @start, @nextComma - @start) AS INT)
		SET @start = @nextComma + 1
	END

	RETURN;	
END