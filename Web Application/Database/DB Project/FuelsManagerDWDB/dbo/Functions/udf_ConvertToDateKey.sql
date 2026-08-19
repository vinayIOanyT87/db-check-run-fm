/*
	DROP FUNCTION [dbo].[udf_ConvertToDateKey]

	SELECT [dbo].[udf_ConvertToDateKey]('03/07/2013')
	SELECT [dbo].[udf_ConvertToDateKey](NULL)
	
*/


CREATE FUNCTION [dbo].[udf_ConvertToDateKey](@date DateTime)
RETURNS int
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_ConvertToDateKey]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Converts a date parameter into an integer of the format YYYYMMDD.
	-- Notes:
	--------------
	DECLARE	@dayNumberInMonth int
	DECLARE @calendarYear int
	DECLARE @calendarMonthNumberInYear int
	DECLARE @openEndedDate Datetimeoffset(7)		
	
	SELECT @openEndedDate = DATEADD(year, 100, GETDATE())

	IF (@date IS NULL)
	BEGIN
		SET @date = @openEndedDate
	END


	SET @dayNumberInMonth = DATEPART(DAY, @date)
	SET @calendarMonthNumberInYear = DATEPART(MONTH, @date)
	SET @calendarYear = DATEPART(YEAR, @date)

	RETURN @calendarYear * 10000 + @calendarMonthNumberInYear * 100 + @dayNumberInMonth
   
END;