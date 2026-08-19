CREATE TABLE [dbo].[tblTransactionNotes] (
    [Notes]                 NVARCHAR (1000)    NULL,
    [CreatedBy]             [dbo].[udtUserID]  NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NULL,
    [AdditionalInformation] NVARCHAR (1000)    NULL,
    [TransactionNoteGuid]   UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionNotes_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [TransactionGuid]       UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTransactionNotes_GUID] PRIMARY KEY NONCLUSTERED ([TransactionNoteGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTransactionNotes_CreatedDate]
    ON [dbo].[tblTransactionNotes]([CreatedDate] ASC);

GO
