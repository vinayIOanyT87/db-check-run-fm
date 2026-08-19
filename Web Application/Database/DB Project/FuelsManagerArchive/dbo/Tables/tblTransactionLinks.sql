CREATE TABLE [dbo].[tblTransactionLinks] (
    [OriginalTransID]               NVARCHAR (64)      CONSTRAINT [DF_tblTransactionLinks_OriginalTransID] DEFAULT ('') NOT NULL,
    [LinkedTransID]                 NVARCHAR (64)      CONSTRAINT [DF_tblTransactionLinks_LinkedTransID] DEFAULT ('') NOT NULL,
    [Level]                         INT                CONSTRAINT [DF_tblTransactionLinks_Level] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                     [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionLinks_CreatedBy] DEFAULT ('') NOT NULL,
    [CreatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionLinks_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                     [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionLinks_UpdatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionLinks_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TransactionLinkGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionLinks_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                   ROWVERSION         NOT NULL,
    [SiteGuid]                      UNIQUEIDENTIFIER   NOT NULL,
    [LinkedTransactionLineItemGuid] UNIQUEIDENTIFIER   NULL,
    [TransactionLineItemGuid]       UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTransactionLinks_GUID] PRIMARY KEY NONCLUSTERED ([TransactionLinkGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTransactionLinks_CreatedDate]
    ON [dbo].[tblTransactionLinks]([CreatedDate] ASC);

GO
