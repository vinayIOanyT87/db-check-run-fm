/*
  DROP PROCEDURE [dbo].[usp_LoadDateDimension]

	EXEC [dbo].[usp_LoadDateDimension] '01/01/1901', '12/31/2030'
	EXEC [dbo].[usp_LoadDateDimension] NULL, '12/31/2021'
	
*/
CREATE PROCEDURE [dbo].[usp_LoadDateDimension] @StartDate date, @EndDate date
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_LoadDateDimension]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads the date dimension table, dimDate, with date records for a given date range.
  -- Notes:
  -- @StartDate: Date from which to start adding entries into dimDate. If @StartDate is null, then the day following the last date entry in dimDate is used as the StartDate.
  -- @EndDate: Last date for which to add entries into dimDate.
  ------------------------------------------------------------------------------------------------------
    SET NOCOUNT ON;
    BEGIN TRY

        IF ((@StartDate IS NULL) OR (@StartDate = '01/01/1900'))
        BEGIN
            SELECT
            @StartDate = DATEADD(DAY, 1, MAX(FullDateAKey))
            FROM dbo.DimDate
        END

        IF (@StartDate IS NULL)
        BEGIN
            RAISERROR ('Start Date is missing.', 16, 1);
            RETURN;
        END

        IF (@EndDate IS NULL)
        BEGIN
            RAISERROR ('End Date is missing.', 16, 1);
            RETURN;
        END

        DECLARE @dayNumberInMonth int
        DECLARE @calendarMonthName nvarchar(20)
        DECLARE @calendarYear int
        DECLARE @calendarMonthNumberInYear int

        DECLARE @currentDate datetime = @StartDate

        WHILE @currentDate <= @EndDate
        BEGIN
            SET @dayNumberInMonth = DATEPART(DAY, @currentDate)
            SET @calendarMonthNumberInYear = DATEPART(MONTH, @currentDate)
            SET @calendarMonthName = DATENAME(mm, @currentDate)
            SET @calendarYear = DATEPART(YEAR, @currentDate)

            INSERT INTO DimDate 
            (
                SKey,
                FullDateAKey,
                FullDateDescription,
                DayNumberOfWeek,
                DayNameOfWeek,
                DayNumberOfMonth,
                DayNumberOfYear,
                WeekNumberOfYear,
                MonthNumberOfYear,
                CalendarWeekNumberInYear,
                CalendarMonthNumberInYear,                
                CalendarMonthName,
                CalendarYearMonthNumber,
                CalendarYearMonth,
                CalendarQuarter,
                CalendarYear,
                FiscalWeek,
                FiscalWeekNumberInYear,
                FiscalMonth,
                FiscalMonthNumberInYear,
                FiscalYearMonth,
                FiscalQuarter,
                FiscalYearQuarter,
                FiscalYear
            )
            VALUES 
            (
                @calendarYear * 10000 + @calendarMonthNumberInYear * 100 + @dayNumberInMonth, @currentDate, 
                CONVERT(varchar(2), @dayNumberInMonth) + ' ' + @calendarMonthName + ' ' + CONVERT(varchar(4), @calendarYear), 
                DATEPART(DW, @currentDate), 
                dbo.udf_GetWeekDayName(@currentDate, 0),
                DATEPART(DAY, @currentDate),                 
                DATEPART(DY, @currentDate), 
                DATEPART(WEEK, @currentDate), 
                DATEPART(MONTH, @currentDate), 
                DATEPART(WEEK, @currentDate), 
                DATEPART(MONTH, @currentDate), 
                DATENAME(mm, @currentDate), 
                @calendarYear * 100 + @calendarMonthNumberInYear,
                CONVERT(varchar(4), @calendarYear) + '-' + CONVERT(varchar(2), @calendarMonthNumberInYear), 
                DATEPART(qq, @currentDate), DATEPART(YEAR, @currentDate), DATEPART(WEEK, @currentDate), 
                DATEPART(WEEK, @currentDate), DATEPART(MONTH, @currentDate), DATEPART(MONTH, @currentDate), 
                CONVERT(varchar(4), @calendarYear) + '-' + CONVERT(varchar(2), @calendarMonthNumberInYear), 
                DATEPART(qq, @currentDate), DATEPART(qq, @currentDate), 
                DATEPART(YEAR, @currentDate)
            )

            SET @currentDate = DATEADD(DAY, 1, @currentDate)
        END

        UPDATE DimDate 
        SET FullDate = CONVERT(varchar(10), FullDateAKey)

  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [dbo].[usp_LoadDateDimension]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END