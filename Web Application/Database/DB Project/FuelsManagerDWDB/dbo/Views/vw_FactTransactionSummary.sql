/*
	DROP VIEW [dbo].[vw_FactTransactionSummary]

*/
CREATE VIEW [dbo].[vw_FactTransactionSummary] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_FactTransactionSummary]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: FactTransactionSummary View to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------

SELECT 
	[SKey],

	[BillToCompanySKey],
	[CarrierCompanySKey],
	[DeleteFlag],
	[DestinationEquipment1SKey],
	[InventoryDateSKey],
	[Line_MeterMinStartTime],
	[Line_MeterMaxStopTime],
	[Line_TimeInMinMeterStartDiff],
	[Line_MeterMinStartMaxStopTimeDiff],
	[Line_MaxMeterStopTimeOutDiff],
	[ManagerCompanySKey],
	[OperatorPersonnelSKey],
	[OwnerCompanySKey],
	[ReasonCodeSKey],
	[ReversalType],
	[ShipperCompanySKey],
	[ShipToCompanySKey],
	[SiteSKey],	
	[SourceEquipment1SKey],
	[SubType],
	[SupplierCompanySKey],
	[TimeIn],
	[TimeInDateSKey],
	[TimeInTimeSKey],
	[TimeInTimeOutDiff],
	[TimeOut],
	[TimeOutDateSKey],
	[TimeOutTimeSKey],
	[TransactionAliasSKey],
	[TransactionAttributesSKey],
	[TransDateTime],
	[TransDateSKey],
	[TransTimeSKey],
	[TransactionKey],
	[TransactionStatusIndex],
	[TransactionTypeSKey],
	[TransID],
	[_IsRecordDeleted],
	[_RecordUpdatedDate],
	[_RecordUpdatedDateSKey]

FROM dbo.FactTransactionSummary