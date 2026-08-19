CREATE TABLE [dbo].[tblOpcUaSession]
(
	[SerializedSession]								XML NULL,
	[CreatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaSession_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy]										[dbo].[udtUserID] CONSTRAINT [DF_tblOpcUaSession_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblOpcUaSession_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy]										[dbo].[udtUserID] CONSTRAINT [DF_tblOpcUaSession_UpdatedBy] DEFAULT ('') NOT NULL,
	[SessionGuid]									UNIQUEIDENTIFIER  CONSTRAINT [DF_tblOpcUaSession_SessionGuid] DEFAULT (NEWID()) NOT NULL,
	[_RowVersion]									ROWVERSION NOT NULL,
	[_ClusterIdx]									BIGINT NOT NULL IDENTITY,
    CONSTRAINT [PK_tblOpcUaSession_SessionGuid] PRIMARY KEY NONCLUSTERED ([SessionGuid] ASC),
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblOpcUaSession_ClusterIdx] 
	ON [dbo].[tblOpcUaSession]([_ClusterIdx]);
GO
