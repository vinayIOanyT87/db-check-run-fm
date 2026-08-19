CREATE TABLE [dbo].[tblFCEEMessage]
(
	[ImeiNumber] [nchar](15) NOT NULL,
   [Timestamp] DATETIMEOFFSET(7) NOT NULL,
	[MsgType] [int] NOT NULL,
	[Index] [int] NOT NULL,
	[Device] [int] NULL,
	[BinaryData] [varbinary](max) NOT NULL,
	[EdgeData] [nvarchar](max) NULL,
	[SoftwareVersion] nchar(32) CONSTRAINT [DF_tblFCEEMessage_SoftwareVersion] DEFAULT ('FCE-20221212.1') NOT NULL,
   [FCEEMessageGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblFCEEMessage_GUID] DEFAULT (newid()) NOT NULL,
	[CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblFCEEMessage_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
	[CreatedBy] [dbo].[udtUserID]  NULL,
	[UpdatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblFCEEMessage_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
	[UpdatedBy] [dbo].[udtUserID]  NULL,
	[_RowVersion] ROWVERSION NOT NULL,
	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL,
   [Validity] BIT NULL, 
    CONSTRAINT [PK_tblFCEEMessage_FCEEMessageGuid] PRIMARY KEY NONCLUSTERED ([FCEEMessageGuid] ASC),
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblFCEEMessage_ClusterIdx] 
	ON [dbo].[tblFCEEMessage]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblFCEEMessage_TimeStamp]
    ON [dbo].[tblFCEEMessage]([Timestamp] ASC);
GO

CREATE INDEX [IX_tblFCEEMessage_MsgType]
ON [dbo].[tblFCEEMessage] ([MsgType]) 
INCLUDE ([ImeiNumber], [Timestamp], [Index], [CreatedDate]);
GO


CREATE INDEX [IX_tblFCEEMessage_CreatedDate]
ON [dbo].[tblFCEEMessage] ([CreatedDate]);

GO


CREATE NONCLUSTERED INDEX [IX_tblFCEEMessage]
	ON [dbo].[tblFCEEMessage]  (
        [ImeiNumber] ASC,
        [Timestamp] ASC,
        [MsgType] ASC,
        [Index] ASC
   );
GO

