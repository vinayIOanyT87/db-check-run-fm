CREATE TABLE [dbo].[tblTransactionUserData] (
    [UserData1]               NVARCHAR (MAX)     NULL,
    [UserData2]               NVARCHAR (MAX)     NULL,
    [UserData3]               NVARCHAR (MAX)     NULL,
    [UserData4]               NVARCHAR (MAX)     NULL,
    [UserData5]               NVARCHAR (MAX)     NULL,
    [UserData6]               NVARCHAR (MAX)     NULL,
    [UserData7]               NVARCHAR (MAX)     NULL,
    [UserData8]               NVARCHAR (MAX)     NULL,
    [UserData9]               NVARCHAR (MAX)     NULL,
    [UserData10]              NVARCHAR (MAX)     NULL,
    [UserData11]              NVARCHAR (MAX)     NULL,
    [UserData12]              NVARCHAR (MAX)     NULL,
    [UserData13]              NVARCHAR (MAX)     NULL,
    [UserData14]              NVARCHAR (MAX)     NULL,
    [UserData15]              NVARCHAR (MAX)     NULL,
    [UserData16]              NVARCHAR (MAX)     NULL,
    [UserData17]              NVARCHAR (MAX)     NULL,
    [UserData18]              NVARCHAR (MAX)     NULL,
    [UserData19]              NVARCHAR (MAX)     NULL,
    [UserData20]              NVARCHAR (MAX)     NULL,
    [UserData21]              NVARCHAR (MAX)     NULL,
    [UserData22]              NVARCHAR (MAX)     NULL,
    [UserData23]              NVARCHAR (MAX)     NULL,
    [UserData24]              NVARCHAR (MAX)     NULL,
    [CreatedBy]               [dbo].[udtUserID]  NULL,
    [CreatedDate]             DATETIMEOFFSET (7) NULL,
    [UpdatedBy]               [dbo].[udtUserID]  NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) NULL,
    [TransactionUserDataGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionUserData_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [TransactionGuid]         UNIQUEIDENTIFIER   NOT NULL,
    CONSTRAINT [PK_tblTransactionUserData_GUID] PRIMARY KEY NONCLUSTERED ([TransactionUserDataGuid] ASC)
);


GO

CREATE CLUSTERED INDEX [IX_tblTransactionUserData_CreatedDate]
    ON [dbo].[tblTransactionUserData]([CreatedDate] ASC);

GO
