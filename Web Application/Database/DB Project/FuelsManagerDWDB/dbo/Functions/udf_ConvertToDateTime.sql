/*
	DROP FUNCTION [dbo].[udf_ConvertToDateTime]

	SELECT [dbo].[udf_ConvertToDateTime](20130702, 231912)
	SELECT [dbo].[udf_ConvertToDateTime](20130702, 91012)
	SELECT [dbo].[udf_ConvertToDateTime](20130702, 100)
	SELECT [dbo].[udf_ConvertToDateTime](20130702, NULL)
	SELECT [dbo].[udf_ConvertToDateTime](NULL, 231012)
	
*/



CREATE FUNCTION [dbo].[udf_ConvertToDateTime](@dateKey int, @timeKey int)
RETURNS DateTimeOffset(7)
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_ConvertToDateTime]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Converts a combination of dateKey of the format YYYYMMDD and a TimeKey of the format HHMMSS into a datetime.
	-- Notes:
	--------------
	DECLARE	@dayNumberInMonth int
	DECLARE @calendarYear int
	DECLARE @calendarMonthNumberInYear int
	DECLARE	@hourNumber int
	DECLARE @minuteNumber int
	DECLARE @secondNumber int
	
	DECLARE @openEndedDate Datetimeoffset(7)
	DECLARE @dateTime Datetimeoffset(7)

	SELECT @openEndedDate = DATEADD(year, 100, GETDATE())
	SET @dateTime = @openEndedDate
		
	IF (@dateKey > 0)
	BEGIN
		SELECT @calendarYear = CONVERT(int, CalendarYear), 
		@calendarMonthNumberInYear = CalendarMonthNumberInYear, 
		@dayNumberInMonth = DayNumberOfMonth 
		FROM dbo.DimDate WHERE SKey = @dateKey
		SET @dateTime = DATEFROMPARTS(@calendarYear, @calendarMonthNumberInYear, @dayNumberInMonth)
	END
	IF ((@dateKey > 0) AND (@timeKey > 0))
	BEGIN
		SELECT @hourNumber = Hour24, 
		@minuteNumber = MinuteNumber, 
		@secondNumber = SecondNumber 
		FROM dbo.DimTime WHERE SKey = @timeKey
		
		SET @dateTime = DATETIMEFROMPARTS(@calendarYear, @calendarMonthNumberInYear, @dayNumberInMonth, @hourNumber, @minuteNumber, @secondNumber, 0)
	END
	
	RETURN @dateTime   

END;