
CREATE FUNCTION [rpt].[udf_MonthYearToTimestamp] 
(
	@MonthYear nvarchar(20)
)
RETURNS datetimeoffset(7)	
AS 
BEGIN
	------------------------------------------------------------------------------------------------------
	-- Stored Procedure: [rpt].[udf_MonthYearToTimestamp] 
	-- Author: Shawn Marlin
	-- Version/Date: 1.0.003 / 2013-04-02 07:54:10.4470770 -10:00
	-- Purpose: Change month year input parameter into a timestamp.
	-- Notes:
	-- 1. @MonthYear: month year string to convert to a datetimeoffset for the first day of the month
	------------------------------------------------------------------------------------------------------

	DECLARE @Month nvarchar(20)
	SET @Month = (SELECT LEFT(@MonthYear, CHARINDEX(' ',@MonthYear+' ')-1))
	IF(@Month = 'January')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'January ','1/1/'))
	ELSE IF(@Month = 'February')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'February ','2/1/'))
	ELSE IF(@Month = 'March')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'March ','3/1/'))
	ELSE IF(@Month = 'April')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'April ','4/1/'))
	ELSE IF(@Month = 'May')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'May ','5/1/'))
	ELSE IF(@Month = 'June')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'June ','6/1/'))
	ELSE IF(@Month = 'July')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'July ','7/1/'))
	ELSE IF(@Month = 'August')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'August ','8/1/'))
	ELSE IF(@Month = 'September')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'September ','9/1/'))
	ELSE IF(@Month = 'October')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'October ','10/1/'))
	ELSE IF(@Month = 'November')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'November ','11/1/'))
	ELSE IF(@Month = 'December')
		SET @MonthYear = (SELECT REPLACE(@MonthYear,'December ','12/1/'))

	DECLARE @BeginDate datetimeoffset(7)
	
	SET @BeginDate = @MonthYear
	RETURN @BeginDate;

END