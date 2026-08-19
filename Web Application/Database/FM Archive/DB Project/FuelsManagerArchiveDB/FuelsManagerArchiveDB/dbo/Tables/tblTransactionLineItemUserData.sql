/*

	DROP TABLE [dbo].[tblTransactionLineItemUserData]

*/
CREATE TABLE [dbo].[tblTransactionLineItemUserData] (
    [UserData1]                       NVARCHAR (60)      NULL,
    [UserData2]                       NVARCHAR (60)      NULL,
    [UserData3]                       NVARCHAR (60)      NULL,
    [UserData4]                       NVARCHAR (60)      NULL,
    [UserData5]                       NVARCHAR (60)      NULL,
    [UserData6]                       NVARCHAR (60)      NULL,
    [UserData7]                       NVARCHAR (60)      NULL,
    [UserData8]                       NVARCHAR (60)      NULL,
    [UserData9]                       NVARCHAR (60)      NULL,
    [UserData10]                      NVARCHAR (60)      NULL,
    [UserData11]                      NVARCHAR (60)      NULL,
    [UserData12]                      NVARCHAR (60)      NULL,
    [UserData13]                      NVARCHAR (60)      NULL,
    [UserData14]                      NVARCHAR (60)      NULL,
    [UserData15]                      NVARCHAR (60)      NULL,
    [UserData16]                      NVARCHAR (60)      NULL,
    [UserData17]                      NVARCHAR (60)      NULL,
    [UserData18]                      NVARCHAR (60)      NULL,
    [UserData19]                      NVARCHAR (60)      NULL,
    [UserData20]                      NVARCHAR (60)      NULL,
    [UserData21]                      NVARCHAR (60)      NULL,
    [UserData22]                      NVARCHAR (60)      NULL,
    [UserData23]                      NVARCHAR (60)      NULL,
    [UserData24]                      NVARCHAR (60)      NULL,
    [CreatedBy]                       [dbo].[udtUserID]  NULL,
    [CreatedDate]                     DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                       [dbo].[udtUserID]  NULL,
    [UpdatedDate]                     DATETIMEOFFSET (7) NULL,
    [TransactionLineItemUserDataGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [TransactionLineItemGuid]         UNIQUEIDENTIFIER   NOT NULL,
	[InventoryDateKey]			      INT                NOT NULL,
	[ArchiveDate]					  DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]					  BIGINT  		     NULL,
	[_RowVersion]                     ROWVERSION         NOT NULL,
    [_ClusterIdx]                     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionLineItemUserData_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionLineItemUserDataGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionLineItemUserData_ClusterIdx] 
	ON [dbo].[tblTransactionLineItemUserData]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_CreatedDate]
    ON [dbo].[tblTransactionLineItemUserData]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_TransactionLineItemGuid]
    ON [dbo].[tblTransactionLineItemUserData]([TransactionLineItemGuid] ASC)
    INCLUDE([TransactionLineItemUserDataGuid])
	ON [AnnualPS]([InventoryDateKey]);
GO
