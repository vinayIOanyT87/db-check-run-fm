/*
	DROP FUNCTION [staging].[udf_DateTimeToDateKey]

	SELECT [staging].[udf_DateTimeToDateKey] (GetDate())
	SELECT [staging].[udf_DateTimeToDateKey] (SysDateTimeOffset())
	SELECT [staging].[udf_DateTimeToDateKey] ('2017-03-14')	
	SELECT [staging].[udf_DateTimeToDateKey] (NULL)

*/
CREATE FUNCTION [staging].[udf_DateTimeToDateKey]
(
	@SourceDateTime DateTime
)
RETURNS Int
AS
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [archive].[udf_DateTimeToDateKey]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Converts a DateTime value to a date integer key in the format YYYMMDD.
	-- Notes:
	-- 1. @SourceDateTime: DateTime value to be converted to integer.
	------------------------------------------------------------------------------------------------------

	DECLARE @result int
	SET @result = 19000101
	IF (@SourceDateTime IS NOT NULL)
	BEGIn
		SELECT @result = YEAR(@SourceDateTime) * 10000 + MONTH(@SourceDateTime) * 100 + DAY(@SourceDateTime)
	END
	RETURN @result;

END