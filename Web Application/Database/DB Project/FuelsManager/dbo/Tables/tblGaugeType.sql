CREATE TABLE [dbo].[tblGaugeType]
(
	[GaugeTypeGuid] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
    [GaugeTypeIndex] INT NOT NULL,
    [ID] NVARCHAR(50) NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Type] INT NOT NULL,
    [DeltaTemp] FLOAT NULL, 
    [Threshold] FLOAT NULL, 
    [CertificationLeakRate] FLOAT NULL, 
    [MinHours] INT NULL,
    [CreatedDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblGauge_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]           [dbo].[udtUserID]  CONSTRAINT [DF_tblGauge_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblGauge_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]           [dbo].[udtUserID]  CONSTRAINT [DF_tblGauge_UpdatedBy] DEFAULT ('') NOT NULL,
)
