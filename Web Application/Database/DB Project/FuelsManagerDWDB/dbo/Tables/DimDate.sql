/*********************** 
	dbo.DimDate 
*************************/

CREATE TABLE [dbo].[DimDate](
[SKey] [int] NOT NULL,
[FullDateAKey] [date] NOT NULL,
[FullDate] [varchar](10) NULL,
[FullDateDescription] [varchar](50) NULL,
[DayNumberOfWeek] [tinyint] NULL,
[DayNameOfWeek] [varchar](50) NULL,
[DayNumberOfMonth] [tinyint] NULL,
[DayNumberOfYear] [smallint] NULL,
[WeekNumberOfYear] [tinyint] NULL,
[DayNumberInFiscalMonth] [smallint] NULL,
[MonthNumberOfYear] [tinyint] NULL,
[LastDayInMonthIndicator] [bit] NULL,
[CalendarWeekEndingDate] [date] NULL,
[CalendarWeekNumberInYear] [smallint] NULL,
[CalendarMonthNumberInYear] [smallint] NULL,
[CalendarMonthName] [varchar](50) NULL,
[CalendarYearMonthNumber] [int] NULL,
[CalendarYearMonth] [varchar](7) NULL,
[CalendarQuarter] [varchar](2) NULL,
[DayNumberInFiscalYear] [smallint] NULL,
[CalendarYearQuarter] [varchar](7) NULL,
[CalendarYear] [varchar](4) NULL,
[FiscalWeek] [smallint] NULL,
[FiscalWeekNumberInYear] [smallint] NULL,
[FiscalMonth] [smallint] NULL,
[FiscalMonthNumberInYear] [smallint] NULL,
[FiscalYearMonth] [varchar](7) NULL,
[FiscalQuarter] [varchar](2) NULL,
[FiscalYearQuarter] [varchar](7) NULL,
[FiscalYear] [varchar](4) NULL,
[CreatedDate] [datetimeoffset](7) NULL,
[CreatedBy] [nvarchar](100) NULL,
[UpdatedDate] [datetimeoffset](7) NULL,
[UpdatedBy] [nvarchar](100) NULL,
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_IsRecordDeleted] [bit] NULL
CONSTRAINT [PK_DimDate] PRIMARY KEY CLUSTERED 
(
[SKey] ASC
))