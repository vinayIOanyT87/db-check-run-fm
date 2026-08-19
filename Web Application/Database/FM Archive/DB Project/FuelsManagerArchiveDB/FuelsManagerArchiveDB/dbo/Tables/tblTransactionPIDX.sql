/*

	DROP TABLE [dbo].[tblTransactionPIDX] 

*/
CREATE TABLE [dbo].[tblTransactionPIDX] (
    [AuthorizationNumber]                NVARCHAR (8)       NULL,
    [SentFlag]                           BIT                NULL,
    [DateSent]                           DATETIMEOFFSET (7) NULL,
    [CreatedBy]                          [dbo].[udtUserID]  NULL,
    [CreatedDate]                        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                          [dbo].[udtUserID]  NULL,
    [UpdatedDate]                        DATETIMEOFFSET (7) NULL,
    [BrokenBlend]                        BIT                NULL,
    [TransactionPIDXGuid]                UNIQUEIDENTIFIER   NOT NULL,    
    [PIDXProfileGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [TransactionGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [CompanyPersonnelToShipToBillToGuid] UNIQUEIDENTIFIER   NOT NULL,
	[InventoryDateKey]					 INT                NOT NULL,
	[ArchiveDate]						 DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]						 BIGINT			    NULL,
	[_RowVersion]                        ROWVERSION         NOT NULL,
    [_ClusterIdx]                        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionPIDX_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC,[TransactionPIDXGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionPIDX_ClusterIdx] 
	ON [dbo].[tblTransactionPIDX]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionPIDX_CreatedDate]
    ON [dbo].[tblTransactionPIDX]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionPIDX_TransactionGuid_PIDXProfileGuid]
    ON [dbo].[tblTransactionPIDX]([TransactionGuid] ASC, [PIDXProfileGuid] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO
