CREATE VIEW [dbo].[vw_FactPhysicalInventory] AS 
  ------------------------------------------------------------------------------------------------------
  -- View: [dbog].[dbo].[vw_FactPhysicalInventory]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: FactPhysicalInventoraySnapshot view to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------

SELECT 

	[SKey],

	[InventoryDateSKey],
	[Line_GrossQuantitySI],
    [Line_GrossQuantityUSGallon],	
	[Line_NetQuantitySI],
	[Line_NetQuantityUSGallon],
	[Line_NetVolumeIndicator],
	[Line_ProductSKey],
	[ManagerCompanySKey],
	[OwnerCompanySKey],
	[StorageLocationTankSKey],
	[SiteSKey],
	[SubType],
	[TransactionAliasSKey],	
    [TransactionStatusName],   
	[TransDateTime],
    [TransID],	

	[TransactionKey],
	[TransactionLineItemKey],
	[TransactionSubLineItemKey],

	[_RecordUpdatedDate],
	[_RecordUpdatedDateSKey]

FROM dbo.FactPhysicalInventorySnapshot