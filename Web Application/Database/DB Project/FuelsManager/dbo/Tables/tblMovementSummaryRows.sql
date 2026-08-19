CREATE TABLE [dbo].[tblMovementSummaryRows](
	[MovementSummaryRowsGuid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_MovementSummaryRows_GUID]  DEFAULT (newid()),
	[MovementSummaryGuid] [uniqueidentifier] NOT NULL,
	[RowsDefinition] [nvarchar](MAX) NULL CONSTRAINT [DF_tblMovementSummaryRows_RowsDefinition]  DEFAULT (''),
	[OwnerUserGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblMovementSummaryRows_CreatedDate]  DEFAULT (sysdatetimeoffset()),
	[CreatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblMovementSummaryRows_CreatedBy]  DEFAULT (''),
	[UpdatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblMovementSummaryRows_UpdatedDate]  DEFAULT (sysdatetimeoffset()),
	[UpdatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblMovementSummaryRows_UpdatedBy]  DEFAULT (''),
	[_RowVersion] [timestamp] NOT NULL,
    [_ClusterIdx] BIGINT NOT NULL IDENTITY,
	CONSTRAINT [PK_tblMovementSummaryRows_GUID] PRIMARY KEY NONCLUSTERED ([MovementSummaryRowsGuid] ASC),
	CONSTRAINT [FK_tblMovementSummaryRows_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
	CONSTRAINT [FK_tblMovementSummaryRows_MovementSummaryGuid] FOREIGN KEY ([MovementSummaryGuid]) REFERENCES [dbo].[tblMovementSummary] ([MovementSummaryGuid])
) ON [PRIMARY]
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblMovementSummaryRows_ClusterIdx] 
	ON [dbo].[tblMovementSummaryRows]([_ClusterIdx]);
GO

CREATE INDEX [IX_tblMovementSummaryRows_MovementSummaryGuid] 
ON [dbo].[tblMovementSummaryRows] ([MovementSummaryGuid]);
GO

