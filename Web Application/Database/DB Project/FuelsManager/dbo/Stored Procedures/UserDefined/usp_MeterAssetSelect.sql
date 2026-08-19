
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:

Find all assets (equipments, tanks, load arms) which have meters assigned to them
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterAssetSelect]
(
	@SiteGuid UNIQUEIDENTIFIER,
	@AssetIDFilterValue NVARCHAR(30) = NULL
)
AS
BEGIN
	SET NOCOUNT OFF

	IF(@AssetIDFilterValue IS NULL)
	BEGIN
		SELECT DISTINCT 
			AssetGuid = tblEquipment._MasterRecordGuid,
			AssetType = 1,
			AssetID = tblEquipment.ID,
			tblEquipment.CreatedDate,
			tblEquipment.CreatedBy,
			tblEquipment.UpdatedDate,
			tblEquipment.UpdatedBy
		FROM tblEquipment 
		JOIN map.tblMeterToEquipment on map.tblMeterToEquipment.EquipmentGuid = tblEquipment.EquipmentGuid 
			WHERE tblEquipment.SiteGuid = @SiteGuid 
			--AND AssignedToMeterGuid IS NOT NULL
		UNION ALL
		SELECT 
			AssetGuid = tblTanks.TankGuid,
			AssetType = 2,
			AssetID = tblTanks.TankID,
			tblTanks.CreatedDate,
			tblTanks.CreatedBy,
			tblTanks.UpdatedDate,
			tblTanks.UpdatedBy
		FROM tblTanks 
		WHERE EXISTS (SELECT map.tblMeterToTank.MeterToTankGuid 
			FROM map.tblMeterToTank 
			WHERE tblTanks.TankGuid = map.tblMeterToTank.TankGuid)
			AND tblTanks.SiteGuid = @SiteGuid 
		UNION ALL
		SELECT 
			AssetGuid = tblLoadArms.LoadArmGuid,
			AssetType = 3,
			AssetID = tblLoadArms.LoadRackText,
			tblLoadArms.CreatedDate,
			tblLoadArms.CreatedBy,
			tblLoadArms.UpdatedDate,
			tblLoadArms.UpdatedBy
		FROM tblLoadArms INNER JOIN tblStations ON tblStations.StationGuid = tblLoadArms.BayAStationGuid OR tblStations.StationGuid = tblLoadArms.BayBStationGuid
			WHERE tblStations.SiteGuid = @SiteGuid 
			AND (EXISTS (SELECT map.tblProductToPresetComponentTankOrTankGroup.ProductToPresetComponentTankOrTankGroupGuid 
				FROM map.tblProductToPresetComponentTankOrTankGroup
				WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid IS NOT NULL
				AND map.tblProductToPresetComponentTankOrTankGroup.AssignedToLoadArmGuid = tblLoadArms.LoadArmGuid)
				OR EXISTS (SELECT map.tblProductToPresetInjector.ProductToPresetInjectorGuid 
				FROM map.tblProductToPresetInjector
				WHERE map.tblProductToPresetInjector.AssignedToMeterGuid IS NOT NULL
				AND map.tblProductToPresetInjector.AssignedToLoadArmGuid = tblLoadArms.LoadArmGuid))
	END
	ELSE 
	BEGIN
		SELECT DISTINCT 
			AssetGuid = tblEquipment._MasterRecordGuid,
			AssetType = 1,
			AssetID = tblEquipment.ID,
			tblEquipment.CreatedDate,
			tblEquipment.CreatedBy,
			tblEquipment.UpdatedDate,
			tblEquipment.UpdatedBy
		FROM tblEquipment
		JOIN map.tblMeterToEquipment on map.tblMeterToEquipment.EquipmentGuid = tblEquipment.EquipmentGuid
			WHERE tblEquipment.SiteGuid = @SiteGuid 
			--AND AssignedToMeterGuid IS NOT NULL
			AND tblEquipment.ID LIKE ('%' + @AssetIDFilterValue + '%')
		UNION ALL
		SELECT 
			AssetGuid = tblTanks.TankGuid,
			AssetType = 2,
			AssetID = tblTanks.TankID,
			tblTanks.CreatedDate,
			tblTanks.CreatedBy,
			tblTanks.UpdatedDate,
			tblTanks.UpdatedBy
		FROM tblTanks
		WHERE EXISTS (SELECT map.tblMeterToTank.MeterToTankGuid 
			FROM map.tblMeterToTank
			WHERE tblTanks.TankGuid = map.tblMeterToTank.TankGuid)
			AND tblTanks.SiteGuid = @SiteGuid 
			AND tblTanks.TankID LIKE ('%' + @AssetIDFilterValue + '%')
		UNION ALL
		SELECT 
			AssetGuid = tblLoadArms.LoadArmGuid,
			AssetType = 3,
			AssetID = tblLoadArms.LoadRackText,
			tblLoadArms.CreatedDate,
			tblLoadArms.CreatedBy,
			tblLoadArms.UpdatedDate,
			tblLoadArms.UpdatedBy
		FROM tblLoadArms INNER JOIN tblStations ON tblStations.StationGuid = tblLoadArms.BayAStationGuid OR tblStations.StationGuid = tblLoadArms.BayBStationGuid
			WHERE tblStations.SiteGuid = @SiteGuid 
			AND (EXISTS (SELECT map.tblProductToPresetComponentTankOrTankGroup.ProductToPresetComponentTankOrTankGroupGuid 
				FROM map.tblProductToPresetComponentTankOrTankGroup
				WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid IS NOT NULL
				AND map.tblProductToPresetComponentTankOrTankGroup.AssignedToLoadArmGuid = tblLoadArms.LoadArmGuid)
				OR EXISTS (SELECT map.tblProductToPresetInjector.ProductToPresetInjectorGuid 
				FROM map.tblProductToPresetInjector 
				WHERE map.tblProductToPresetInjector.AssignedToMeterGuid IS NOT NULL
				AND map.tblProductToPresetInjector.AssignedToLoadArmGuid = tblLoadArms.LoadArmGuid))
			AND tblLoadArms.LoadRackText LIKE ('%' + @AssetIDFilterValue + '%')
	END
END

