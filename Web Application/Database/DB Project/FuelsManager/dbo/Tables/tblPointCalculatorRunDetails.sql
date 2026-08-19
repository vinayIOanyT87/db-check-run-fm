CREATE TABLE [dbo].[tblPointCalculatorRunDetails](
	[PointCalculatorRunId] [uniqueidentifier] NOT NULL,
	[TagName] [nvarchar](50) NOT NULL,
	[Units] [nvarchar](50) NULL,
	[Acronym] [nvarchar](10) NULL,
	[BeginValue] [nvarchar](50) NOT NULL,
	[EndValue] [nvarchar](50) NOT NULL,
	[DiffValue] [nvarchar](50) NULL,
	[DisplayOrder] [int] NOT NULL,
   [CreatedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointCalculatorRunDetails_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
   [CreatedBy]    [dbo].[udtUserID]  NULL DEFAULT 'administrator',
	[UpdatedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointCalculatorRunDetails_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
   [UpdatedBy]    [dbo].[udtUserID]  NULL DEFAULT 'administrator',
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL, 
    CONSTRAINT [FK_tblPointCalculatorRunDetails_ToTable] FOREIGN KEY ([PointCalculatorRunId]) REFERENCES [dbo].[tblPointCalculatorRuns]([PointCalculatorRunId]),
) 


GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointCalculatorRunDetails_ClusterIdx]
    ON [dbo].[tblPointCalculatorRunDetails]([_ClusterIdx] ASC);
GO