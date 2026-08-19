/*

	DROP TABLE [staging].[tblTransactionWeightReadings]

*/
CREATE TABLE [staging].[tblTransactionWeightReadings] (
    [CompartmentID]					NVARCHAR (30)		NULL,
    [BeginQuantityValue]			FLOAT (53)			NULL,
    [RequestedQuantityValue]		FLOAT (53)			NULL,
    [FinalQuantityValue]			FLOAT (53)			NULL,
    [CreatedBy]						[dbo].[udtUserID]	NULL,
    [CreatedDate]					DATETIMEOFFSET (7)	NULL,
    [UpdatedBy]						[dbo].[udtUserID]	NULL,
    [UpdatedDate]					DATETIMEOFFSET (7)	NULL,
    [TransVersion]					BIGINT				NULL,
    [TransactionWeightReadingGuid]	UNIQUEIDENTIFIER	NULL,    
    [TransactionGuid]				UNIQUEIDENTIFIER	NULL,
    [FuelsManagerVersionNumber]		INT					NULL,
    [SourceVersionNumber]			INT					NULL,
    [HistoricalFlag]				BIT					NULL,
    [VolumetricTopOffFlag]			BIT					NULL,
	[InventoryDateKey]				INT                 NULL,
	[ArchiveDate]					DATETIMEOFFSET (7)  NULL,
	[ETLProcessKey]					BIGINT			    NULL,
	[SourceClusterIdx]				BIGINT				NULL,
	[SourceRowVersion]				BIGINT				NULL,
	[IgnoreRecord]					BIT					NOT NULL,
	[IsProcessed]					BIT					NOT NULL,
	[_RowVersion]					ROWVERSION			NOT NULL,
	[SKey]							INT					IDENTITY (1, 1) NOT NULL
    CONSTRAINT [PK_tblTransactionWeightReadings_SKey] PRIMARY KEY CLUSTERED ([SKey] ASC)
);
GO


ALTER TABLE [staging].[tblTransactionWeightReadings] ADD  DEFAULT ((0)) FOR [IgnoreRecord]
GO


ALTER TABLE [staging].[tblTransactionWeightReadings] ADD  DEFAULT ((0)) FOR [IsProcessed]
GO
