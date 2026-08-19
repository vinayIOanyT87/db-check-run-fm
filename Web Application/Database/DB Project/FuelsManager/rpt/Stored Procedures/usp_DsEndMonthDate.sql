

CREATE PROCEDURE [rpt].[usp_DsEndMonthDate]

AS
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
DECLARE @currentdate DATETIMEOFFSET(7);
DECLARE @endlastmonthdate DATETIMEOFFSET(7);
DECLARE @day int;
DECLARE @millisec int;
DECLARE @sec int;
DECLARE @min int;
DECLARE @hour int;

SELECT @currentdate = SYSDATETIMEOFFSET();

SELECT @day = DATEPART(day, @currentdate);
SELECT @millisec = DATEPART(millisecond, @currentdate);
SELECT @sec = DATEPART(second, @currentdate);
SELECT @min = DATEPART(minute, @currentdate);
SELECT @hour = DATEPART(hour, @currentdate);

SELECT @endlastmonthdate = DATEADD(millisecond, (-@millisec), @currentdate);
SELECT @endlastmonthdate = DATEADD(second, (-@sec), @endlastmonthdate);
SELECT @endlastmonthdate= DATEADD(minute, (-@min), @endlastmonthdate);
SELECT @endlastmonthdate = DATEADD(hour, (-@hour), @endlastmonthdate);

SELECT @endlastmonthdate = DATEADD(day,(-@day)+1,@endlastmonthdate);
SELECT @endlastmonthdate = DATEADD(month,1,@endlastmonthdate);
SELECT @endlastmonthdate = DATEADD(day,-1,@endlastmonthdate);

SELECT @endlastmonthdate AS EndMonthDate