
/*
=============================================
Author: Ryan Hill
Create date: 4/24/12
Description:

Read a record from the meter table based on input parameters.
=============================================
*/
CREATE PROCEDURE [dbo].[usp_MeterSelect]
(
	@SiteGuid UNIQUEIDENTIFIER = NULL,
	@MeterGuid UNIQUEIDENTIFIER = NULL,
	@TankGuid UNIQUEIDENTIFIER = NULL,
	@LoadArmGuid UNIQUEIDENTIFIER = NULL,
	@MeterAssetGuid UNIQUEIDENTIFIER = NULL,
	@MeterID NVARCHAR(30) = NULL,
	@MeterIDFilterValue NVARCHAR(30) = NULL
)
AS
BEGIN
	SET NOCOUNT OFF

	IF (@MeterGuid IS NOT NULL) -- Direct lookup on MeterGuid
	BEGIN

		SELECT MeterGuid,
			tblMeter.SiteGuid,
			tblMeter.MeterID,
			tblMeter.NumberOfDigits,
			tblMeter.RotatesBackwardsFlag,
			tblMeter.ReceiptMeterFlag,
			tblMeter.MeterFactor,
			tblMeter.FuelCompressionFactor,
			tblMeter.DcuID,
			tblMeter.DcuBatteryVoltage,
			tblMeter.DcuBatteryCurrent,
			tblMeter.DcuTemperature,
			tblMeter.DcuResets,
			tblMeter.DcuUpdateDate,
			tblMeter.DcuConfigurationDate,
			tblMeter.DcuFirmwareVersion,
			tblMeter.DcuBluetoothAddress,
			tblMeter.CreatedDate,
			tblMeter.CreatedBy,
			tblMeter.UpdatedDate,
			tblMeter.UpdatedBy
		FROM tblMeter
		WHERE MeterGuid = @MeterGuid 

	END
	ELSE IF (@TankGuid IS NOT NULL) --Show all meters for a specified Tank
	BEGIN 

		SELECT tblMeter.MeterGuid,
			tblMeter.SiteGuid,
			tblMeter.MeterID,
			tblMeter.NumberOfDigits,
			tblMeter.RotatesBackwardsFlag,
			tblMeter.ReceiptMeterFlag,
			tblMeter.MeterFactor,
			tblMeter.FuelCompressionFactor,
			tblMeter.DcuID,
			tblMeter.DcuBatteryVoltage,
			tblMeter.DcuBatteryCurrent,
			tblMeter.DcuTemperature,
			tblMeter.DcuResets,
			tblMeter.DcuUpdateDate,
			tblMeter.DcuConfigurationDate,
			tblMeter.DcuFirmwareVersion,
			tblMeter.DcuBluetoothAddress,
			tblMeter.CreatedDate,
			tblMeter.CreatedBy,
			tblMeter.UpdatedDate,
			tblMeter.UpdatedBy
		FROM tblMeter INNER JOIN map.tblMeterToTank ON map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid 
		WHERE map.tblMeterToTank.TankGuid = @TankGuid 

	END
	ELSE IF (@MeterAssetGuid IS NOT NULL) --Lookup on the Guid of the record to which the meter is assigned
	BEGIN

		IF(@MeterIDFilterValue IS NOT NULL) -- We may filter on a partial ID match, for example from the meter select form
		BEGIN
			SELECT * FROM (
				SELECT tblMeter.MeterGuid,
					tblMeter.SiteGuid,
					tblMeter.MeterID,
					tblMeter.NumberOfDigits,
					tblMeter.RotatesBackwardsFlag,
					tblMeter.ReceiptMeterFlag,
					tblMeter.MeterFactor,
					tblMeter.FuelCompressionFactor,
					tblMeter.DcuID,
					tblMeter.DcuBatteryVoltage,
					tblMeter.DcuBatteryCurrent,
					tblMeter.DcuTemperature,
					tblMeter.DcuResets,
					tblMeter.DcuUpdateDate,
					tblMeter.DcuConfigurationDate,
					tblMeter.DcuFirmwareVersion,
					tblMeter.DcuBluetoothAddress,
					tblMeter.CreatedDate,
					tblMeter.CreatedBy,
					tblMeter.UpdatedDate,
					tblMeter.UpdatedBy
				FROM tblMeter INNER JOIN map.tblMeterToTank ON map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid 
				WHERE map.tblMeterToTank.TankGuid = @MeterAssetGuid 
				UNION 
				SELECT tblMeter.MeterGuid,
					tblMeter.SiteGuid,
					tblMeter.MeterID,
					tblMeter.NumberOfDigits,
					tblMeter.RotatesBackwardsFlag,
					tblMeter.ReceiptMeterFlag,
					tblMeter.MeterFactor,
					tblMeter.FuelCompressionFactor,
					tblMeter.DcuID,
					tblMeter.DcuBatteryVoltage,
					tblMeter.DcuBatteryCurrent,
					tblMeter.DcuTemperature,
					tblMeter.DcuResets,
					tblMeter.DcuUpdateDate,
					tblMeter.DcuConfigurationDate,
					tblMeter.DcuFirmwareVersion,
					tblMeter.DcuBluetoothAddress,
					tblMeter.CreatedDate,
					tblMeter.CreatedBy,
					tblMeter.UpdatedDate,
					tblMeter.UpdatedBy
				FROM tblMeter
					INNER JOIN map.tblMeterToEquipment map ON map.MeterGuid = tblMeter.MeterGuid
					INNER JOIN dbo.tblEquipment e ON e.EquipmentGuid = map.EquipmentGuid
					INNER JOIN map.tblEntityEquipmentToSite ets ON ets.EquipmentGuid = e._MasterRecordGuid AND ets.SiteGuid = @SiteGuid
				WHERE map.EquipmentGuid = @MeterAssetGuid AND ets.SiteGuid = @SiteGuid
				UNION
				SELECT tblMeter.MeterGuid,
					tblMeter.SiteGuid,
					tblMeter.MeterID,
					tblMeter.NumberOfDigits,
					tblMeter.RotatesBackwardsFlag,
					tblMeter.ReceiptMeterFlag,
					tblMeter.MeterFactor,
					tblMeter.FuelCompressionFactor,
					tblMeter.DcuID,
					tblMeter.DcuBatteryVoltage,
					tblMeter.DcuBatteryCurrent,
					tblMeter.DcuTemperature,
					tblMeter.DcuResets,
					tblMeter.DcuUpdateDate,
					tblMeter.DcuConfigurationDate,
					tblMeter.DcuFirmwareVersion,
					tblMeter.DcuBluetoothAddress,
					tblMeter.CreatedDate,
					tblMeter.CreatedBy,
					tblMeter.UpdatedDate,
					tblMeter.UpdatedBy
				FROM tblMeter INNER JOIN map.tblProductToPresetComponentTankOrTankGroup ON map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid = tblMeter.MeterGuid 
				WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToLoadArmGuid = @MeterAssetGuid
				UNION 
				SELECT tblMeter.MeterGuid,
					tblMeter.SiteGuid,
					tblMeter.MeterID,
					tblMeter.NumberOfDigits,
					tblMeter.RotatesBackwardsFlag,
					tblMeter.ReceiptMeterFlag,
					tblMeter.MeterFactor,
					tblMeter.FuelCompressionFactor,
					tblMeter.DcuID,
					tblMeter.DcuBatteryVoltage,
					tblMeter.DcuBatteryCurrent,
					tblMeter.DcuTemperature,
					tblMeter.DcuResets,
					tblMeter.DcuUpdateDate,
					tblMeter.DcuConfigurationDate,
					tblMeter.DcuFirmwareVersion,
					tblMeter.DcuBluetoothAddress,
					tblMeter.CreatedDate,
					tblMeter.CreatedBy,
					tblMeter.UpdatedDate,
					tblMeter.UpdatedBy
				FROM tblMeter INNER JOIN map.tblProductToPresetInjector ON map.tblProductToPresetInjector.AssignedToMeterGuid = tblMeter.MeterGuid 
				WHERE map.tblProductToPresetInjector.AssignedToLoadArmGuid = @MeterAssetGuid ) AS Results
			WHERE MeterID LIKE ('%' + @MeterIDFilterValue + '%') 
			ORDER BY Results.CreatedDate

		END
		ELSE --Lookup on the Guid of the record to which the meter is assigned without any filtering
		BEGIN

			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter INNER JOIN map.tblMeterToTank ON map.tblMeterToTank.MeterGuid = tblMeter.MeterGuid 
			WHERE map.tblMeterToTank.TankGuid = @MeterAssetGuid 
			UNION 
			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter
				INNER JOIN map.tblMeterToEquipment map ON map.MeterGuid = tblMeter.MeterGuid
				INNER JOIN dbo.tblEquipment e ON e.EquipmentGuid = map.EquipmentGuid
				INNER JOIN map.tblEntityEquipmentToSite ets ON ets.EquipmentGuid = e._MasterRecordGuid AND ets.SiteGuid = @SiteGuid
			WHERE map.EquipmentGuid = @MeterAssetGuid AND ets.SiteGuid = @SiteGuid
			UNION 
			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter INNER JOIN map.tblProductToPresetComponentTankOrTankGroup ON map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid = tblMeter.MeterGuid 
			WHERE map.tblProductToPresetComponentTankOrTankGroup.AssignedToLoadArmGuid = @MeterAssetGuid
			UNION 
			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter INNER JOIN map.tblProductToPresetInjector ON map.tblProductToPresetInjector.AssignedToMeterGuid = tblMeter.MeterGuid 
			WHERE map.tblProductToPresetInjector.AssignedToLoadArmGuid = @MeterAssetGuid 
			ORDER BY tblMeter.CreatedDate

		END
	END
	ELSE IF (@MeterID IS NOT NULL) --Search for meters with a matching meter ID
	BEGIN
		
		IF(@LoadArmGuid IS NOT NULL) --If we're searching on LoadArmGuid and MeterID, we are only looking for load arm components which match.
		BEGIN

			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter INNER JOIN map.tblProductToPresetComponentTankOrTankGroup ON tblMeter.MeterGuid = map.tblProductToPresetComponentTankOrTankGroup.AssignedToMeterGuid 
			WHERE tblMeter.MeterID = @MeterID AND tblMeter.SiteGuid = @SiteGuid
				AND map.tblProductToPresetComponentTankOrTankGroup.AssignedToLoadArmGuid = @LoadArmGuid

		END
		ELSE -- lookup on Meter ID only
		BEGIN

			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter
			WHERE tblMeter.MeterID = @MeterID AND tblMeter.SiteGuid = @SiteGuid

		END

	END
	ELSE IF (@SiteGuid IS NOT NULL) -- Show all meters for this site
	BEGIN

		IF(@MeterIDFilterValue IS NULL) -- Filter on a partial ID match
		BEGIN

			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter
			WHERE tblMeter.SiteGuid = @SiteGuid

		END
		ELSE -- Look for matches on SiteGuid only
		BEGIN 

			SELECT tblMeter.MeterGuid,
				tblMeter.SiteGuid,
				tblMeter.MeterID,
				tblMeter.NumberOfDigits,
				tblMeter.RotatesBackwardsFlag,
				tblMeter.ReceiptMeterFlag,
				tblMeter.MeterFactor,
				tblMeter.FuelCompressionFactor,
				tblMeter.DcuID,
				tblMeter.DcuBatteryVoltage,
				tblMeter.DcuBatteryCurrent,
				tblMeter.DcuTemperature,
				tblMeter.DcuResets,
				tblMeter.DcuUpdateDate,
				tblMeter.DcuConfigurationDate,
				tblMeter.DcuFirmwareVersion,
				tblMeter.DcuBluetoothAddress,
				tblMeter.CreatedDate,
				tblMeter.CreatedBy,
				tblMeter.UpdatedDate,
				tblMeter.UpdatedBy
			FROM tblMeter 
			WHERE tblMeter.SiteGuid = @SiteGuid
			AND MeterID LIKE ('%' + @MeterIDFilterValue + '%')

		END

	END

END
