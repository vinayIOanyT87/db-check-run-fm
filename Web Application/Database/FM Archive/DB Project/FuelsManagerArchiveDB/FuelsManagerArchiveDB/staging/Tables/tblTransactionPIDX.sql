/*

	DROP TABLE [staging].[tblTransactionPIDX] 

*/
CREATE TABLE [staging].[tblTransactionPIDX] (
    [AuthorizationNumber]                NVARCHAR (8)       NULL,
    [SentFlag]                           BIT                NULL,
    [DateSent]                           DATETIMEOFFSET (7) NULL,
    [CreatedBy]                          [dbo].[udtUserID]  NULL,
    [CreatedDate]                        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                          [dbo].[udtUserID]  NULL,
    [UpdatedDate]                        DATETIMEOFFSET (7) NULL,
    [BrokenBlend]                        BIT                NULL,
    [TransactionPIDXGuid]                UNIQUEIDENTIFIER   NULL,    
    [PIDXProfileGuid]                    UNIQUEIDENTIFIER   NULL,
    [TransactionGuid]                    UNIQUEIDENTIFIER   NULL,
    [CompanyPersonnelToShipToBillToGuid] UNIQUEIDENTIFIER   NULL,
	[InventoryDateKey]					 INT                NULL,
	[ArchiveDate]						 DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]						 BIGINT			    NULL,
	[SourceClusterIdx]					 BIGINT				NULL,
	[SourceRowVersion]					 BIGINT				NULL,
	[IgnoreRecord]						 BIT				NOT NULL,
	[IsProcessed]						 BIT				NOT NULL,
	[_RowVersion]						 ROWVERSION			NOT NULL,
	[SKey]								 INT				IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblTransactionPIDX_SKey] PRIMARY KEY CLUSTERED ([sKey] ASC)
);
GO


ALTER TABLE [staging].[tblTransactionPIDX] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblTransactionPIDX] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
