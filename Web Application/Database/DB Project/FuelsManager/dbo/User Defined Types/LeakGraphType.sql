CREATE TYPE [dbo].[LeakGraphType] AS TABLE
(
	[LeakReportId] UNIQUEIDENTIFIER NOT NULL , 
    [SampleTime] DATETIMEOFFSET NOT NULL, 
    [SampleVolume] FLOAT NULL, 
    [IsUsed] BIT NOT NULL 
)
