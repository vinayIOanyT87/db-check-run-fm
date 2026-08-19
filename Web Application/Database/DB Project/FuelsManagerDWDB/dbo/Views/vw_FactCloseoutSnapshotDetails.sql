CREATE VIEW [dbo].[vw_FactCloseoutSnapshotDetails] AS 
  ------------------------------------------------------------------------------------------------------
  -- View: [dbo].[vw_FactCloseoutSnapshotDetails]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: View to support all direct queries against the data warehouse for Owner Closeout data.
  -- Notes:
  -- 1. This view should be used by all external queries and reports against the data warehouse database, instead of querying the 
  --    FactCloseoutSnapshotDetails table directly.
  ------------------------------------------------------------------------------------------------------

SELECT 

	[SKey],
	[CloseoutDate],
	[CloseoutDateSKey],
	[GrossBookInventorySI],
	[GrossBookInventoryUSGallon],
	[GrossBookPrice],
	[ManagerAddress1],
	[ManagerAddress2],
	[ManagerCity],
	[ManagerCode],
	[ManagerCompanyName],
	[ManagerCompanyKey],
	[ManagerCompanySKey],
	[ManagerCountry],
	[ManagerEmergencyContact],
	[ManagerId],
	[ManagerLockedOut],
	[ManagerState],
	[ManagerZip],
	[MassBookInventoryLb],
	[MassBookInventorySI],
	[MassBookPrice],
	[NetBookInventorySI],
	[NetBookInventoryUSGallon],
	[NetBookPrice],
	[OwnerAddress1],
	[OwnerAddress2],
	[OwnerCity],
	[OwnerCode],
	[OwnerCompanyKey],
	[OwnerCompanyName],
	[OwnerCompanySKey],
	[OwnerCountry],
	[OwnerEmergencyContact],
	[OwnerId],
	[OwnerLockedOut],
	[OwnerState],
	[OwnerZip],
	[ProductAviationFuelFlag],
	[ProductCode],
	[ProductGroundFuel],
	[ProductId],
	[ProductLockedOut],
	[ProductKey],
	[ProductSKey],
	[ProductVolumeDecimalPlaces],
	[SiteAddress1],
	[SiteAddress2],
	[SiteCity],
	[SiteContact1Name],
	[SiteCountry],
	[SiteDensityDecimalPlaces],
	[SiteDensityUnitIndex],
	[SiteGroupFlag],
	[SiteId],
	[SitePhone],
	[SiteKey],
	[SiteSKey],
	[SiteState],
	[SiteTemperatureDecimalPlaces],
	[SiteTemperatureUnitIndex],
	[SiteTimeZone],
	[SiteVolumeDecimalPlaces],
	[SiteVolumeUnitIndex],
	[SiteZip],

	[OwnerCloseoutKey],

	[_RecordUpdatedDate],
	[_RecordUpdatedDateSKey],
	[_IsRecordDeleted]

FROM dbo.FactCloseoutSnapshot

