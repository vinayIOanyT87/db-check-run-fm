/*************************************
  FactTransactionSumary
*****************************************/

CREATE TABLE [dbo].[FactTransactionSummary]
(
[SKey] [int] IDENTITY(1,1) NOT NULL,
[BillToCompanySKey] [int] NOT NULL DEFAULT(0),
[CarrierCompanySKey] [int] NOT NULL DEFAULT(0),
[DeleteFlag] [bit] NULL,	
[DestinationEquipment1SKey] [int] NOT NULL DEFAULT(0),
[InventoryDateSKey] [int] NOT NULL DEFAULT(19000101),	
[Line_MeterMaxStopTime] [datetimeoffset](7) NULL,
[Line_MeterMinStartTime] [datetimeoffset](7) NULL,
[Line_MeterMinStartMaxStopTimeDiff] [int] NULL,
[Line_MaxMeterStopTimeOutDiff] [int] NULL,
[Line_TimeInMinMeterStartDiff] [int] NULL,
[ManagerCompanySKey] [int] NOT NULL DEFAULT(0),
[OperatorPersonnelSKey] [int] NOT NULL DEFAULT(0),
[OwnerCompanySKey] [int] NOT NULL DEFAULT(0),
[ReasonCodeSKey] [int] NOT NULL DEFAULT(0),
[ReversalType] [nvarchar](2) NULL,
[ShipperCompanySKey] [int] NOT NULL DEFAULT(0),
[ShipToCompanySKey] [int] NOT NULL DEFAULT(0),
[SiteSKey] [int] NOT NULL DEFAULT(0),
[SourceEquipment1SKey] [int] NOT NULL DEFAULT(0),
[SubType] [nvarchar](20) NULL,
[SupplierCompanySKey] [int] NOT NULL DEFAULT(0),
[TimeIn] [datetimeoffset](7) NULL,
[TimeInDateSKey] [int] NOT NULL DEFAULT(19000101),
[TimeInTimeSKey] [int] NOT NULL DEFAULT(0),
[TimeInTimeOutDiff] [int] NULL,
[TimeOut] [datetimeoffset](7) NULL,
[TimeOutDateSKey] [int] NOT NULL DEFAULT(19000101),
[TimeOutTimeSKey] [int] NOT NULL DEFAULT(0),
[TransactionAliasSKey] [int] NOT NULL DEFAULT(0),
[TransactionStatusIndex] [int] NULL,
[TransactionStatusName] [nvarchar](100) NULL,
[TransID] [nvarchar](64) NULL,
[TransactionAttributesSKey] [int] NOT NULL,
[TransactionTypeSKey] [int] NOT NULL,
[TransDateTime] [datetimeoffset](7) NULL,
[TransDateSKey] [int] NOT NULL DEFAULT(19000101),
[TransTimeSKey] [int] NOT NULL DEFAULT(0),

[TransactionKey] [nvarchar](50) NULL,
[_RecordUpdatedDate] [datetimeoffset](7) NULL,
[_RecordUpdatedDateSKey] [int] NOT NULL DEFAULT(19000101),
[_IsRecordDeleted] [bit] NULL
CONSTRAINT [PK_FactTransactionSummary_Clustered] PRIMARY KEY CLUSTERED ([InventoryDateSKey] ASC, [SKey] ASC) WITH (FILLFACTOR = 100) ON [AnnualPS] ([InventoryDateSKey])
)
GO

CREATE TRIGGER [dbo].[trg_insupd_FactTransactionSummary] 
   ON [dbo].[FactTransactionSummary]
   AFTER INSERT, UPDATE 
AS 
BEGIN 
  ------------------------------------------------------------------------------------------------------
  -- Trigger: [dbo].[trg_insupd_FactTransactionSummary]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Capture the key of all modified or newly inserted FactTransactionSummary records. To be used to support the processing of the transaction dimensions.
  -- Notes:
  ------------------------------------------------------------------------------------------------------
	SET NOCOUNT ON; 

	INSERT INTO staging.tblEditedFactTransactionSummary
	(FactTransactionSummarySKey)
	SELECT SKey FROM INSERTED
 
END
GO