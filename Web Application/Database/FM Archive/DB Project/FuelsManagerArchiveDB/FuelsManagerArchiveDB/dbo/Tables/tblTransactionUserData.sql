/*

	DROP TABLE [dbo].[tblTransactionUserData]

*/
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
    [TransactionUserDataGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [TransactionGuid]         UNIQUEIDENTIFIER   NOT NULL,
	[InventoryDateKey]		  INT                NOT NULL,
	[ArchiveDate]             DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]			  BIGINT			 NULL,
	[_RowVersion]             ROWVERSION         NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionUserData_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionUserDataGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO



CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionUserData_ClusterIdx] 
	ON [dbo].[tblTransactionUserData]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_CreatedDate]
    ON [dbo].[tblTransactionUserData]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_TransactionGuid]
    ON [dbo].[tblTransactionUserData]([TransactionGuid] ASC)
    INCLUDE([TransactionUserDataGuid])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_TransactionGuidUserData4] 
ON [dbo].[tblTransactionUserData] ([TransactionGuid] ASC)
INCLUDE ( [UserData4]) 
ON [AnnualPS]([InventoryDateKey])
GO


