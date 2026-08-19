/*

	DROP TABLE [dbo].[tblTransactionSubLineItems]

*/
CREATE TABLE [dbo].[tblTransactionSubLineItems] (
    [SequenceID]                   INT                NULL,
    [Product]                      NVARCHAR (30)      NULL,
    [ProductCode]                  NVARCHAR (50)      NULL,
    [ProductType]                  NVARCHAR (20)      NULL,
    [GrossQuantity]                FLOAT (53)         NULL,
    [NetQuantity]                  FLOAT (53)         NULL,
    [Vcf]                          FLOAT (53)         NULL,
    [Density]                      FLOAT (53)         NULL,
    [Temperature]                  FLOAT (53)         NULL,
    [Customs]                      NVARCHAR (20)      NULL,
    [ArmNumber]                    INT                NULL,
    [LineNumber]                   INT                NULL,
    [BatchNumber]                  NVARCHAR (20)      NULL,
    [LineFill]                     FLOAT (53)         NULL,
    [BottomVolume]                 FLOAT (53)         NULL,
    [NetCapacity]                  FLOAT (53)         NULL,
    [TankStatus]                   NVARCHAR (30)      NULL,
    [MeterFactor]                  FLOAT (53)         NULL,
    [MeterStart]                   FLOAT (53)         NULL,
    [MeterStop]                    FLOAT (53)         NULL,
    [MeterStopDateTime]            DATETIMEOFFSET (7) NULL,
    [MeterStartDateTime]           DATETIMEOFFSET (7) NULL,
    [FreezePoint]                  FLOAT (53)         NULL,
    [DifferentialPressure]         FLOAT (53)         NULL,
    [DosageRate]                   FLOAT (53)         NULL,
    [DeleteFlag]                   BIT                NULL,
    [PresetAmount]                 FLOAT (53)         NULL,
    [StorageLocationID]            NVARCHAR (50)      NULL,
    [MeterID]                      NVARCHAR (50)      NULL,
    [COAID]                        NVARCHAR (40)      NULL,
    [CreatedBy]                    [dbo].[udtUserID]  NULL,
    [CreatedDate]                  DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                    [dbo].[udtUserID]  NULL,
    [UpdatedDate]                  DATETIMEOFFSET (7) NULL,
    [TransactionInventoryDate]     DATE               NULL,
    [Tax1]                         FLOAT (53)         NULL,
    [Tax2]                         FLOAT (53)         NULL,
    [Tax3]                         FLOAT (53)         NULL,
    [Tax4]                         FLOAT (53)         NULL,
    [Tax5]                         FLOAT (53)         NULL,
    [TransVersion]                 BIGINT             NULL,
    [ImproperAdditization]         BIT                NULL,
    [BrokenBlend]                  BIT                NULL,
    [Flag01]                       BIT                NULL,
    [Flag02]                       BIT                NULL,
    [Flag03]                       BIT                NULL,
    [Flag04]                       BIT                NULL,
    [Flag05]                       BIT                NULL,
    [Flag06]                       BIT                NULL,
    [Number01]                     FLOAT (53)         NULL,
    [Number02]                     FLOAT (53)         NULL,
    [Number03]                     FLOAT (53)         NULL,
    [Number04]                     FLOAT (53)         NULL,
    [Number05]                     FLOAT (53)         NULL,
    [Number06]                     FLOAT (53)         NULL,
    [Date01]                       DATETIMEOFFSET (7) NULL,
    [Date02]                       DATETIMEOFFSET (7) NULL,
    [Date03]                       DATETIMEOFFSET (7) NULL,
    [Date04]                       DATETIMEOFFSET (7) NULL,
    [MassQuantity]                 FLOAT (53)         NULL,
    [NetManualValueFlag]           BIT                NULL,
    [MassManualValueFlag]          BIT                NULL,
    [GrossManualValueFlag]         BIT                NULL,
    [VcfManualValueFlag]           BIT                NULL,
    [TransactionSubLineItemGuid]   UNIQUEIDENTIFIER   NOT NULL,    
    [LookupTransactionStatusIndex] INT                NULL,
    [LookupQualityIndex]           INT                NULL,
    [TransactionLineItemGuid]      UNIQUEIDENTIFIER   NOT NULL,
    [ProductGuid]                  UNIQUEIDENTIFIER   NULL,
    [TransactionGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [StorageLocationTankGuid]      UNIQUEIDENTIFIER   NULL,
    [MeterGuid]                    UNIQUEIDENTIFIER   NULL,
    [PackageManualValueFlag]       BIT                NULL,
    [CleanLineItem]                BIT                NULL,
    [CleanLineDeductItem]          BIT                NULL,
    [CleanLineDeductQuantity]      FLOAT (53)         NULL,
    [CleanLinePackQuantity]        FLOAT (53)         NULL,
	[InventoryDateKey]			   INT                NOT NULL,
	[ArchiveDate]                  DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]				   BIGINT			  NULL,
	[_RowVersion]                  ROWVERSION         NOT NULL,
    [_ClusterIdx]                  BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionSubLineItems_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionSubLineItemGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionSubLineItems_ClusterIdx] 
	ON [dbo].[tblTransactionSubLineItems]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_TransactionInventoryDate]
    ON [dbo].[tblTransactionSubLineItems]([TransactionInventoryDate] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_LedgerCoveringIndex] ON [dbo].[tblTransactionSubLineItems]
(
	[TransactionGuid] ASC
)
INCLUDE
(
	[LookupQualityIndex],
	[GrossQuantity],
	[ProductGuid],
	[StorageLocationTankGuid],
	[NetQuantity],
	[MassQuantity]
) ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_MeterGuid]
    ON [dbo].[tblTransactionSubLineItems]([MeterGuid] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_ProductGuid]
    ON [dbo].[tblTransactionSubLineItems]([ProductGuid] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_TransactionLineItemGuid_SequenceID]
    ON [dbo].[tblTransactionSubLineItems]([TransactionLineItemGuid] ASC, [SequenceID] ASC)
    INCLUDE([TransactionSubLineItemGuid])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_TransactionGuid_TransVersion]
    ON [dbo].[tblTransactionSubLineItems]([TransactionGuid] ASC, [TransVersion] ASC)
	ON [AnnualPS]([InventoryDateKey]);
GO
