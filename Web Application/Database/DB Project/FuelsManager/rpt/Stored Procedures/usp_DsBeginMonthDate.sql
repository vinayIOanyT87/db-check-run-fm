-- \Database - Development\DB Project\FuelsManager\rpt\Stored Procedures\usp_DsBeginMonthDate.sql

CREATE PROCEDURE [rpt].[usp_DsBeginMonthDate]

AS
SET TRANSACTION ISOLATION LEVEL READ UNCOMMITTED
DECLARE @currentdate DATETIMEOFFSET(7);
DECLARE @beginmonthdate DATETIMEOFFSET(7);
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

SELECT @beginmonthdate = DATEADD(millisecond, (-@millisec), @currentdate);
SELECT @beginmonthdate = DATEADD(second, (-@sec), @beginmonthdate);
SELECT @beginmonthdate = DATEADD(minute, (-@min), @beginmonthdate);
SELECT @beginmonthdate = DATEADD(hour, (-@hour), @beginmonthdate);

SELECT DATEADD(day, (-@day)+1, @beginmonthdate) AS BeginMonthDate