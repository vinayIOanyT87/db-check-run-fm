CREATE TABLE [dbo].[tblPointCalculatorRuns]
(
	[PointCalculatorRunId] [uniqueidentifier] NOT NULL,
	[SiteId] [nvarchar](50) NOT NULL,
	[PointId] [nvarchar](50) NOT NULL,
	[CalculationMode] [nvarchar](50) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[PointGuid] [uniqueidentifier] NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[Token] [uniqueidentifier] NOT NULL,
   [CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointCalculatorRuns_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
   [CreatedBy]   [dbo].[udtUserID]  NULL DEFAULT 'administrator',
	[UpdatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointCalculatorRuns_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
   [UpdatedBy]   [dbo].[udtUserID]  NULL DEFAULT 'administrator',
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL, 
    CONSTRAINT [PK_tblPointCalculatorRuns] PRIMARY KEY NONCLUSTERED ([PointCalculatorRunId]),
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointCalculatorRuns_ClusterIdx]
    ON [dbo].[tblPointCalculatorRuns]([_ClusterIdx] ASC);
GO