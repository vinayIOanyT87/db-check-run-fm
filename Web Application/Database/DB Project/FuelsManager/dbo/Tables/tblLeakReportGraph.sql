CREATE TABLE [dbo].[tblLeakReportGraph]
(
	[LeakReportId] UNIQUEIDENTIFIER NOT NULL , 
    [SampleTime] DATETIMEOFFSET NOT NULL, 
    [SampleVolume] FLOAT NULL, 
    [IsUsed] BIT NOT NULL, 
    PRIMARY KEY ([LeakReportId], [SampleTime]), 
    CONSTRAINT [FK_tblLeakReportGraph_tblLeakReport] FOREIGN KEY ([LeakReportId]) REFERENCES [tblLeakReport]([LeakReportId])
)
