CREATE VIEW [dbo].[vw_FactCloseoutSnapshot] AS 
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_FactCloseoutSnapshot]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: FactCloseoutSnapshotDetails view to support the OLAP cube.
  ------------------------------------------------------------------------------------------------------

SELECT 

	[SKey],

	[CloseoutDateSKey],
	[GrossBookInventorySI],
	[GrossBookInventoryUSGallon],
	[GrossBookPrice],
	[ManagerCompanySKey],
	[MassBookInventoryLb],
	[MassBookInventorySI],
	[MassBookPrice],
	[NetBookInventorySI],
	[NetBookInventoryUSGallon],
	[NetBookPrice],
	[OwnerCompanySKey],
	[ProductSKey],
	[SiteSKey],

	[OwnerCloseoutKey],

	[_RecordUpdatedDate],
	[_RecordUpdatedDateSKey],
	[_IsRecordDeleted]

FROM dbo.FactCloseoutSnapshot