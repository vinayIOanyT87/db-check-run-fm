/*************************************
  FactTransaction
*****************************************/

CREATE TABLE [dbo].[FactTransaction]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[BillToCompanySKey] [int] NOT NULL DEFAULT(0),
[CarrierCompanySKey] [int] NOT NULL DEFAULT(0),
[ConjoinOwnerSKey] [int] NOT NULL DEFAULT(0),
[ConjoinTransID] [nvarchar](64) NULL,
[CreatedDate] [datetimeoffset](7) NULL,
[CreatedDateSKey] [int] NOT NULL DEFAULT(19000101),
[CreatedTimeSKey] [int] NOT NULL DEFAULT (0),
[Date01DateSKey] [int] NOT NULL DEFAULT(19000101),
[Date01TimeSKey] [int] NOT NULL DEFAULT(0),
[DeleteFlag] [bit] NULL,	
[DestinationEquipment1SKey] [int] NOT NULL DEFAULT(0),
[DocumentNumber] [nvarchar](30) NULL, 
[InternationalRouteIndicator] [bit] NULL,
[InventoryDateSKey] [int] NOT NULL DEFAULT(19000101),	
[Line_ConjoinProductSKey] [int] NOT NULL DEFAULT(0),
[Line_Density] [float] NULL,
[Line_DestinationEquipmentSKey] [int] NOT NULL DEFAULT(0),
[Line_GrossQuantitySI] [float] NULL,
[Line_GrossQuantityUSGallon] [float] NULL,
[Line_LoadArmSKey]  [int] NOT NULL DEFAULT(0),
[Line_MeterID] [nvarchar](50) NULL,
[Line_MeterStart] [float] NULL,
[Line_MeterStartDateTime] [datetimeoffset](7) NULL,
[Line_MeterStartStopTimeDiff] [int] NULL,
[Line_MeterStopDateTime] [datetimeoffset](7) NULL,
[Line_MeterStop] [float] NULL,
[Line_NetQuantitySI] [float] NULL,
[Line_NetQuantityUSGallon] [float] NULL,
[Line_NetVolumeIndicator] [bit] NULL,
[Line_ProductSKey] [int] NOT NULL DEFAULT(0),
[Line_SequenceID] [smallint] NULL,	
[Line_SourceEquipmentSKey]  [int] NOT NULL DEFAULT(0),
[Line_StationSKey]  [int] NOT NULL DEFAULT(0),
[Line_StorageLocationTankSKey] [int] NOT NULL DEFAULT(0),
[Line_Temperature] [float] NULL,
[LineUData_UserData1] [nvarchar](255) NULL,
[Line_Vcf] [float] NULL,
[ManagerCompanySKey] [int] NOT NULL DEFAULT(0),
[Number01] [float] NULL,
[OperatorPersonnelSKey] [int] NOT NULL DEFAULT(0),
[OwnerCompanySKey] [int] NOT NULL DEFAULT(0),
[ReasonCodeSKey] [int] NOT NULL DEFAULT(0),
[ReversalType] [nvarchar](2) NULL,
[ReversedTransID] [nvarchar](64) NULL,
[RoutingID] [nvarchar](30) NULL,
[ShipperCompanySKey] [int] NOT NULL DEFAULT(0),
[ShipToCompanySKey] [int] NOT NULL DEFAULT(0),
[SiteSKey] [int] NOT NULL DEFAULT(0),
[SourceEquipment1SKey] [int] NOT NULL DEFAULT(0),
[SubType] [nvarchar](20) NULL,
[SupplierCompanySKey] [int] NOT NULL DEFAULT(0),
[TimeIn] [datetimeoffset](7) NULL,
[TimeInDateSKey] [int] NOT NULL DEFAULT(19000101),
[TimeInTimeSKey] [int] NOT NULL DEFAULT(0),
[TimeOut] [datetimeoffset](7) NULL,
[TimeOutDateSKey] [int] NOT NULL DEFAULT(19000101),
[TimeOutTimeSKey] [int] NOT NULL DEFAULT(0),
[TransactionAliasSKey] [int] NOT NULL DEFAULT(0),
[TransactionStatusIndex] [int] NULL,
[TransactionStatusName] [nvarchar](100) NULL,
[TransDateTime] [datetimeoffset](7) NULL,
[TransDateSKey] [int] NOT NULL DEFAULT(19000101),
[TransTimeSKey] [int] NOT NULL DEFAULT(0),
[TransID] [nvarchar](64) NULL,
[TransactionAttributesSKey] [int] NOT NULL,
[TransactionTypeSKey] [int] NOT NULL,
[TransVersion] [bigint] NULL,
[UData_UserData2] [nvarchar](255) NULL,
[UData_UserData3] [nvarchar](255) NULL,
[UData_UserData4SI] [float] NULL,
[UData_UserData4USGallon] [float] NULL,
[UData_UserData5SI] [float] NULL,
[UData_UserData5USGallon] [float] NULL,
[UData_UserData6SI] [float] NULL,
[UData_UserData6USGallon] [float] NULL,
[UData_UserData23] [nvarchar](255) NULL,

[TransactionKey] [nvarchar](50) NULL,
[TransactionLineItemKey] [nvarchar](50) NULL,
[TransactionLineItemUserDataKey] [nvarchar](50) NULL,
[TransactionUserDataKey] [nvarchar](50) NULL,
[TransactionSubLineItemKey] [nvarchar](50) NULL,

[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_RecordUpdatedDateSKey] [int] NOT NULL DEFAULT(19000101),
[_IsRecordDeleted] [bit] NULL
CONSTRAINT [PK_FactTransaction_Clustered] PRIMARY KEY CLUSTERED ([InventoryDateSKey] ASC, [SKey] ASC) WITH (FILLFACTOR = 100) ON [AnnualPS] ([InventoryDateSKey])
)
GO

CREATE TRIGGER [dbo].[trg_insupd_FactTransaction] 
   ON [dbo].[FactTransaction]
   AFTER INSERT, UPDATE 
AS 
BEGIN 
  ------------------------------------------------------------------------------------------------------
  -- Trigger: [dbo].[trg_insupd_FactTransaction]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Capture the key of all modified or newly inserted FactTransaction records. To be used to support the processing of the transaction dimensions.
  -- Notes:
  ------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON; 

	INSERT INTO staging.tblEditedFactTransaction
	(FactTransactionSKey)
	SELECT SKey FROM INSERTED
 
END
GO