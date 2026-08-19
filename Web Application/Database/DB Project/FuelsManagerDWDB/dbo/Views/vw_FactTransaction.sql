/*
	DROP VIEW [dbo].[vw_FactTransaction]

*/
CREATE VIEW [dbo].[vw_FactTransaction] AS
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_FactTransaction]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: FactTransaction View to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------

SELECT 
	[SKey],

	[BillToCompanySKey],
	[CarrierCompanySKey],
	[ConjoinOwnerSKey],
	[ConjoinTransID],
	[CreatedDateSKey],
	[CreatedTimeSKey],
	[Date01DateSKey],
	[Date01TimeSKey],
	[DeleteFlag],
	[DestinationEquipment1SKey],
	[InventoryDateSKey],
	[Line_ConjoinProductSKey],	
	[Line_DestinationEquipmentSKey],	
	[Line_GrossQuantitySI],
	[Line_GrossQuantityUSGallon],
	[Line_LoadArmSKey],
	[Line_MeterID],
	[Line_MeterStart],
	[Line_MeterStartStopTimeDiff],
	[Line_MeterStop],
	[Line_MeterStopDateTime],
	[Line_NetQuantitySI],
	[Line_NetQuantityUSGallon],
	[Line_NetVolumeIndicator],
	[Line_ProductSKey],
	[Line_SourceEquipmentSKey],
	[Line_StationSKey],
	[Line_StorageLocationTankSKey],
	[Line_Temperature],
	[Line_Vcf],
	[LineUData_UserData1],
	[ManagerCompanySKey],
	[Number01],
	[OperatorPersonnelSKey],
	[OwnerCompanySKey],
	[ReasonCodeSKey],
	[ReversalType],
	[ReversedTransID],
	[ShipperCompanySKey],
	[ShipToCompanySKey],
	[SiteSKey],	
	[SourceEquipment1SKey],
	[SubType],
	[SupplierCompanySKey],
	[TimeIn],
	[TimeInDateSKey],
	[TimeInTimeSKey],
	[TimeOut],
	[TimeOutDateSKey],
	[TimeOutTimeSKey],
	[TransactionAliasSKey],
	[TransactionAttributesSKey],
	[TransactionKey],
	[TransactionLineItemKey],
	[TransactionLineItemUserDataKey],
	[TransactionStatusIndex],
	[TransactionSubLineItemKey],
	[TransactionTypeSKey],
	[TransactionUserDataKey],
	[TransDateTime],
	[TransDateSKey],
	[TransTimeSKey],
	[TransID],
	[TransVersion],
	[UData_UserData2],
	[UData_UserData23],
	[UData_UserData3],
	[UData_UserData4SI],
	[UData_UserData4USGallon],
	[UData_UserData5SI],
	[UData_UserData5USGallon],
	[UData_UserData6SI],
	[UData_UserData6USGallon],

	[_IsRecordDeleted],
	[_RecordUpdatedDate],
	[_RecordUpdatedDateSKey]

FROM dbo.FactTransaction