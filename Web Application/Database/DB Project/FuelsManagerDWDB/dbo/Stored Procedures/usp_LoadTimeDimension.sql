CREATE PROCEDURE [dbo].[usp_LoadTimeDimension]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [dbo].[usp_LoadTimeDimension]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Loads the time dimension table, dimTime.
  -- Notes:
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON
  DECLARE @ElapsedSeconds int,
          @MaxElapsedSeconds int,
          @Date datetime,
          @AMPM char(2),
          @hour24 tinyint,
          @hour tinyint,
          @minute tinyint,
          @second int

  SET @ElapsedSeconds = 0
  SET @MaxElapsedSeconds = 60 * 60 * 24

  TRUNCATE TABLE dbo.DimTime

  WHILE @ElapsedSeconds < @MaxElapsedSeconds
  BEGIN
    SET @Date = DATEADD(SECOND, @ElapsedSeconds, CONVERT(datetime, '20100101'))
    SET @AMPM = RIGHT(CONVERT(varchar, @Date, 109), 2)
    SET @Hour24 = LEFT(CONVERT(time, @Date), 2)
    SET @hour =
               CASE
                 WHEN @AMPM = 'PM' THEN @hour24 - 12
                 ELSE @hour24
               END
    SET @minute = DATEPART(MINUTE, @Date)
    SET @second = DATEPART(SECOND, @Date)

    INSERT INTO dbo.DimTime ([SKey]
    , [Time]
    , [Time24]
    , [HourName]
    , [MinuteName]
    , [HourNumber]
    , [Hour24]
    , [MinuteNumber]
    , [SecondNumber]
    , [AMPM]
    , [ElapsedMinutes]
    , [ElapsedSeconds])
      SELECT
        ((@Hour24 * 10000) + (@minute * 100) + @second) AS [TimeKey],
        RIGHT('0' + CONVERT(varchar(2), @hour), 2) + ':' + RIGHT('0' + CONVERT(varchar(2), @minute), 2) + ':' + RIGHT('0' + CONVERT(varchar(2), @second), 2) + ' ' + @AMPM AS [Time],
        CONVERT(varchar(8), @Date, 108) AS [Time24],
        RIGHT('0' + CONVERT(varchar(2), @hour), 2) + ' ' + @AMPM AS [HourName],
        RIGHT('0' + CONVERT(varchar(2), @hour), 2) + ':' + RIGHT('0' + CONVERT(varchar(2), @minute), 2) + ' ' + @AMPM AS [MinuteName],
        @hour AS [HourNumber],
        @hour24 AS [Hour24],
        @minute AS [MinuteNumber],
        @second AS [SecondNumber],
        @AMPM AS [AMPM],
        @ElapsedSeconds / 60 AS [ElapsedMinutes],
        @ElapsedSeconds AS [ElapsedSeconds]
    SET @ElapsedSeconds = @ElapsedSeconds + 1
  END
END