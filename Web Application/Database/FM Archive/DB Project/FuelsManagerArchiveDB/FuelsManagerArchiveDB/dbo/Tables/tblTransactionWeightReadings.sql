/*

	DROP TABLE [dbo].[tblTransactionWeightReadings]

*/
CREATE TABLE [dbo].[tblTransactionWeightReadings] (
    [CompartmentID]                NVARCHAR (30)      NULL,
    [BeginQuantityValue]           FLOAT (53)         NULL,
    [RequestedQuantityValue]       FLOAT (53)         NULL,
    [FinalQuantityValue]           FLOAT (53)         NULL,
    [CreatedBy]                    [dbo].[udtUserID]  NULL,
    [CreatedDate]                  DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                    [dbo].[udtUserID]  NULL,
    [UpdatedDate]                  DATETIMEOFFSET (7) NULL,
    [TransVersion]                 BIGINT             NULL,
    [TransactionWeightReadingGuid] UNIQUEIDENTIFIER   NOT NULL,    
    [TransactionGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [FuelsManagerVersionNumber]    INT                NULL,
    [SourceVersionNumber]          INT                NULL,
    [HistoricalFlag]               BIT                NULL,
    [VolumetricTopOffFlag]         BIT                NULL,
	[InventoryDateKey]			   INT                NOT NULL,
	[ArchiveDate]                  DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]				   BIGINT			  NULL,
	[_RowVersion]                  ROWVERSION         NOT NULL,
    [_ClusterIdx]                  BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionWeightReadings_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionWeightReadingGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionWeightReadings_ClusterIdx] 
	ON [dbo].[tblTransactionWeightReadings]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionWeightReadings_CreatedDate]
    ON [dbo].[tblTransactionWeightReadings]([CreatedDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionWeightReadings_TransactionGuid_HistoricalFlag]
    ON [dbo].[tblTransactionWeightReadings]([TransactionGuid] ASC, [HistoricalFlag] ASC)
    INCLUDE([FuelsManagerVersionNumber])
	ON [AnnualPS]([InventoryDateKey]);
GO
