/*
	DROP FUNCTION [dbo].[udf_GetWeekDayName]

	SELECT [dbo].[udf_GetWeekDayName]('03/07/2013', 0)
	SELECT [dbo].[udf_GetWeekDayName]('03/07/2013', 1)

	SELECT [dbo].[udf_GetWeekDayName](NULL, 0)
	
*/
CREATE FUNCTION [dbo].[udf_GetWeekDayName](@date DateTime, @format bit)
RETURNS nvarchar(20)
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_ConvertToDateKey]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Returns the week day name of a date
	-- Notes:
	-- @date: target date
	-- @format: text format to return the week day name (0: short 1: long)
	--------------
	DECLARE	@dayNumberInWeek int
	DECLARE @weekDayName nvarchar(20)
	DECLARE @openEndedDate Datetimeoffset(7)			

	IF (@date IS NULL)
	BEGIN
		SET @date = @openEndedDate
	END
	SELECT @dayNumberInWeek = DATEPART(DW, @date)

	SELECT @weekDayName = 
	(
		CASE @dayNumberInWeek
			WHEN 1 THEN IIF (@format = 0, 'Sun', 'Sunday')
			WHEN 2 THEN IIF (@format = 0, 'Mon', 'Monday')
			WHEN 3 THEN IIF (@format = 0, 'Tue', 'Tuesday')
			WHEN 4 THEN IIF (@format = 0, 'Wed', 'Wednesday')
			WHEN 5 THEN IIF (@format = 0, 'Thu', 'Thursday')
			WHEN 6 THEN IIF (@format = 0, 'Fri', 'Friday')
			WHEN 7 THEN IIF (@format = 0, 'Sat', 'Saturday')
		END
	)

	RETURN @weekDayName
   
END;
