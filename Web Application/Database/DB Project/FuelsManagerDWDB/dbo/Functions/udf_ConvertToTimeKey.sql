/*
	DROP FUNCTION [dbo].[udf_ConvertToTimeKey]

	SELECT [dbo].[udf_ConvertToTimeKey]('03/07/2013 23:10:12')
	SELECT [dbo].[udf_ConvertToTimeKey]('03/07/2013 00:00:12')
	SELECT [dbo].[udf_ConvertToTimeKey](NULL)
	
*/


CREATE FUNCTION [dbo].[udf_ConvertToTimeKey](@date DateTime)
RETURNS int
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Function: [dbo].[udf_ConvertToTimeKey]
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Converts a datetime parameter into an SKey value of the DimTime table.
	-- Notes:
	--------------
	DECLARE	@hourNumber int
	DECLARE @minuteNumber int
	DECLARE @secondNumber int
	DECLARE @timeSKey int
	DECLARE @openEndedDate Datetimeoffset(7)		
	
	SELECT @openEndedDate = DATEADD(year, 100, GETDATE())

	IF (@date IS NULL)
	BEGIN
		SET @date = @openEndedDate
	END


	SET @hourNumber = DATEPART(HOUR, @date)
	SET @minuteNumber = DATEPART(MINUTE, @date)
	SET @secondNumber = DATEPART(SECOND, @date)

	SELECT @timeSKey = SKey FROM dbo.DimTime 
	WHERE Hour24 = @hourNumber
	AND MinuteNumber = @minuteNumber
	AND SecondNumber = @secondNumber
	
	RETURN @timeSKey   

END;