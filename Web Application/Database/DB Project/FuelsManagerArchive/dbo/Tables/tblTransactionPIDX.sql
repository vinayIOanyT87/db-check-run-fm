CREATE TABLE [dbo].[tblTransactionPIDX] (
    [AuthorizationNumber]                NVARCHAR (8)       NULL,
    [SentFlag]                           BIT                CONSTRAINT [DF_tblTransactionPIDX_SentFlag] DEFAULT ((0)) NOT NULL,
    [DateSent]                           DATETIMEOFFSET (7) NULL,
    [CreatedBy]                          [dbo].[udtUserID]  NULL,
    [CreatedDate]                        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                          [dbo].[udtUserID]  NULL,
    [UpdatedDate]                        DATETIMEOFFSET (7) NULL,
    [BrokenBlend]                        BIT                NULL,
    [TransactionPIDXGuid]                UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionPIDX_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                        ROWVERSION         NOT NULL,
    [PIDXProfileGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [TransactionGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [CompanyPersonnelToShipToBillToGuid] UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTransactionPIDX_GUID] PRIMARY KEY NONCLUSTERED ([TransactionPIDXGuid] ASC)
);


GO
CREATE CLUSTERED INDEX [IX_tblTransactionPIDX_CreatedDate]
    ON [dbo].[tblTransactionPIDX]([CreatedDate] ASC);

GO
