
CREATE FUNCTION [dbo].[udf_CalculateInterval]
(@IntervalType NVARCHAR (30), @CurrentDate DATETIMEOFFSET(7), @FiscalStartDate DATETIMEOFFSET(7))
RETURNS NVARCHAR (40)
AS
BEGIN
	DECLARE @YearPart nvarchar(4)
	DECLARE @MonthPart nvarchar(2)
	DECLARE @DayPart nvarchar(2)
	DECLARE @Interval nvarchar (40)
	DECLARE @Temp nvarchar(1)
	SET @Interval = null

	IF (@IntervalType = 'Daily')
	BEGIN
	   -- The format for the interval is "Day of yyyy-mm-dd".
	   SET @Interval  = 'Day of '
	   SET @YearPart  = DATEPART(year, @CurrentDate)
	   SET @MonthPart = DATEPART(month, @CurrentDate)
	   SET @DayPart   = DATEPART(day, @CurrentDate)

	   IF (DATEPART(day, @CurrentDate) < 10)
	   BEGIN
		  SET @Temp = @DayPart
		  SET @DayPart = '0' + @Temp
	   END

	   IF (DATEPART(month, @CurrentDate) < 10)
	   BEGIN
		  SET @Temp = @MonthPart
		  SET @MonthPart = '0' + @Temp
	   END

	   SET @Interval  = @Interval + ' ' + @YearPart + '-' + @MonthPart + '-' + @DayPart
	END

	IF (@IntervalType = 'Weekly')
	BEGIN
	   -- The format for the interval is "Week of yyyy-mm-dd" where yyyy-mm-dd is always Sunday's date of the
	   -- current week.
	   DECLARE @WeekBeginningDate DATETIMEOFFSET(7)
	   DECLARE @DayCount int

	   -- Calculate the beginning date of the current week (always Sunday's date)
	   SET @DayCount          = (DATEPART(dw, @CurrentDate) - 1) * -1
	   SET @WeekBeginningDate = DATEADD(day, @DayCount, @CurrentDate)
	   SET @YearPart          = DATEPART(year, @WeekBeginningDate)
	   SET @MonthPart         = DATEPART(month, @WeekBeginningDate)
	   SET @DayPart           = DATEPART(day, @WeekBeginningDate)

	   IF (DATEPART(month, @WeekBeginningDate) < 10)
	   BEGIN
		  SET @Temp = @MonthPart
		  SET @MonthPart = '0' + @Temp
	   END

	   IF (DATEPART(day, @WeekBeginningDate) < 10)
	   BEGIN
		  SET @Temp = @DayPart
		  SET @DayPart = '0' + @Temp
	   END

	   SET @Interval = 'Week of ' + @YearPart + '-' + @MonthPart + '-' + @DayPart
	END

	IF (@IntervalType = 'Monthly')
	BEGIN
	   -- The format for the interval is "January yyyy".
	   SET @YearPart = DATEPART(year, @CurrentDate)
	   SET @Interval = DATENAME(month, @CurrentDate)
	   SET @Interval = @Interval + ' ' + @YearPart
	END

	IF (@IntervalType = 'Yearly')
	BEGIN
	   -- The format for the interval is "Year yyyy".
	   SET @YearPart = DATEPART(year, @CurrentDate)
	   SET @Interval = 'Year ' + @YearPart
	END

	IF (@IntervalType = 'Fiscal Year')
	BEGIN
		-- The format for the interval is "Fiscal Year yyyy".
		IF (@FiscalStartDate IS NULL)
		  BEGIN
			SET @YearPart = DATEPART(year, @CurrentDate)
		  END
		ELSE
		  BEGIN
			IF (@CurrentDate >= @FiscalStartDate)
			  BEGIN
				SET @YearPart = DATEPART(year, @CurrentDate)
			  END
			ELSE
			  BEGIN
				DECLARE @FiscalDate DATETIMEOFFSET(7)
				SET @FiscalDate = DATEADD(year, -1, @CurrentDate)
				SET @YearPart = DATEPART(year, @FiscalDate)
			  END
		  END

		SET @Interval = 'Fiscal Year ' + @YearPart
	END

	Return @Interval
END