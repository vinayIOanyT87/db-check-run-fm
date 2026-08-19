CREATE TABLE [dbo].[tblMovementSummaryColumns](
	[MovementSummaryColumnsGuid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MovementSummaryColumns_GUID]  DEFAULT (newid()),
	[MovementSummaryGuid] [uniqueidentifier] NOT NULL,
	[ColumnsDefinition] [nvarchar](MAX) NOT NULL CONSTRAINT [DF_tblMovementSummaryColumns_ColumnsDefinition]  DEFAULT (''),
	[FontSize] [int] NOT NULL CONSTRAINT [DF_tblMovementSummaryColumns_FontSize]  DEFAULT (14),
	[OwnerUserGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblMovementSummaryColumns_CreatedDate]  DEFAULT (sysdatetimeoffset()),
	[CreatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblMovementSummaryColumns_CreatedBy]  DEFAULT (''),
	[UpdatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblMovementSummaryColumns_UpdatedDate]  DEFAULT (sysdatetimeoffset()),
	[UpdatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblMovementSummaryColumns_UpdatedBy]  DEFAULT (''),
	[_RowVersion] [timestamp] NOT NULL,
	[_ClusterIdx] BIGINT NOT NULL IDENTITY,
	CONSTRAINT [PK_tblMovementSummaryColumns_GUID] PRIMARY KEY NONCLUSTERED ([MovementSummaryColumnsGuid] ASC),
	CONSTRAINT [FK_tblMovementSummaryColumns_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
	CONSTRAINT [FK_tblMovementSummaryColumns_MovementSummaryGuid] FOREIGN KEY ([MovementSummaryGuid]) REFERENCES [dbo].[tblMovementSummary] ([MovementSummaryGuid])
) ON [PRIMARY]
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblMovementSummaryColumns_ClusterIdx] 
	ON [dbo].[tblMovementSummaryColumns]([_ClusterIdx]);
GO

CREATE INDEX [IX_tblMovementSummaryColumns_MovementSummaryGuid]
ON [dbo].[tblMovementSummaryColumns] ([MovementSummaryGuid]);
GO
