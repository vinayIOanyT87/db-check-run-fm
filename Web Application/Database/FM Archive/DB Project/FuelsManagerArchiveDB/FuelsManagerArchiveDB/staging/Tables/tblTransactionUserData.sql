/*

	DROP TABLE [staging].[tblTransactionUserData]

*/
CREATE TABLE [staging].[tblTransactionUserData] (
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
    [TransactionUserDataGuid] UNIQUEIDENTIFIER   NULL,    
    [TransactionGuid]         UNIQUEIDENTIFIER   NULL,
	[InventoryDateKey]		  INT                NULL,
	[ArchiveDate]             DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]			  BIGINT			 NULL,
	[SourceClusterIdx]		  BIGINT			 NULL,
	[SourceRowVersion]		  BIGINT			 NULL,
	[IgnoreRecord]			  BIT				 NOT NULL,
	[IsProcessed]			  BIT				 NOT NULL,
	[_RowVersion]			  ROWVERSION		 NOT NULL,
	[SKey]					  INT			 IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblTransactionUserData_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblTransactionUserData] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblTransactionUserData] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO


