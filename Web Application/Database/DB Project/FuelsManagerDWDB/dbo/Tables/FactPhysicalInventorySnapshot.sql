/*
	DROP TABLE [dbo].[FactPhysicalInventorySnapshot]
*/
CREATE TABLE [dbo].[FactPhysicalInventorySnapshot](		
	[InventoryDateSKey]					INT NOT NULL DEFAULT(19000101),
	[Line_GrossQuantitySI]				FLOAT NULL,
    [Line_GrossQuantityUSGallon]		FLOAT NULL,	
	[Line_NetQuantitySI]				FLOAT NULL,
	[Line_NetQuantityUSGallon]			FLOAT NULL,
	[Line_NetVolumeIndicator]			BIT NULL,
	[Line_ProductSKey]					INT NOT NULL DEFAULT(0),
	[ManagerCompanySKey]				INT NOT NULL DEFAULT(0),
	[OwnerCompanySKey]					INT NOT NULL DEFAULT(0),
	[StorageLocationTankSKey]			INT NOT NULL DEFAULT(0),
	[SiteSKey]							INT NOT NULL DEFAULT(0),
	[SubType]							NVARCHAR(20) NULL,
	[TransactionAliasSKey]				INT NOT NULL DEFAULT(0),	
    [TransactionStatusName]				NVARCHAR(100) NULL,   
	[TransDateTime]						DATETIMEOFFSET(7) NULL,
    [TransID]							NVARCHAR(64) NULL,	

	[TransactionKey]					NVARCHAR(50) NULL,
	[TransactionLineItemKey]			NVARCHAR(50) NULL,
	[TransactionSubLineItemKey]			NVARCHAR(50) NULL,

    [_RecordUpdatedDate]				DATETIMEOFFSET (7) NULL,
	[_RecordUpdatedDateSKey]			INT NOT NULL DEFAULT(19000101),
	[SKey]								INT IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_FactPhysicalInventorySnapshot_Clustered] PRIMARY KEY CLUSTERED ([InventoryDateSKey] ASC, [SKey] ASC) WITH (FILLFACTOR = 100) ON [AnnualPS] ([InventoryDateSKey])
)
GO

