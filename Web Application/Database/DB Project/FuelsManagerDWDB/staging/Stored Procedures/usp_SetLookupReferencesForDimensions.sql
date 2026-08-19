/*
  EXEC [staging].[usp_SetLookupReferencesForDimensions]
	
*/
CREATE PROCEDURE [staging].[usp_SetLookupReferencesForDimensions]
AS
BEGIN
  ------------------------------------------------------------------------------------------------------
  -- Stored procedure: [staging].[usp_SetLookupReferencesForDimensions]
  -- Author: Hansraj Bapoo
  -- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
  -- Purpose: Sets, in staging, the value of the corresponding name field for each lookup index field for Dimension tables. 
  -- E.g. using the staging.tblProducts.LookupProductTypeIndex field value, look up the corresponding lookup.tblLookup.name field value, and use that value to set the staging.tblProducts.ProductTypeName field value.
  -- Notes:
  -- 1. In the data warehouse database, the majority of the lookup tables have been consolidated into a single lookup table, lookup.tblLookup, using a LookupType field to differentiate between the different lookups.
  -- 2. The OLAP database does not maintain any relationship to lookup tables. It requires all references to lookup data to have been pre-resolved.
  ------------------------------------------------------------------------------------------------------
  SET NOCOUNT ON;
  BEGIN TRY

    -- Site Lookup references
    UPDATE a
    SET a.LevelUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LevelUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.TemperatureUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.TemperatureUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.DensityUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.DensityUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.PressureUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.PressureUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.FlowUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.FlowUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.VolumeUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.VolumeUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.MassUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.MassUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.AdditiveVolumeUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.AdditiveVolumeUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.AdditiveProfileCycleAmountUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.AdditiveProfileCycleAmountUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.AdditiveProfileRateUnitName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.AdditiveProfileRateUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.NumberGroupSizesTypeName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupNumberGroupSizesTypeIndex
    WHERE b.LookupType = 'NumberGroupSizesType'

    UPDATE a
    SET a.SecondaryStorageFillMethodName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupSecondaryStorageFillMethodIndex
    WHERE b.LookupType = 'FillMethod'

    UPDATE a
    SET a.MailConnectModeName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupMailConnectModeIndex
    WHERE b.LookupType = 'MailServerConnectMode'

    UPDATE a
    SET a.QuantityDisplayDefaultName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupQuantityDisplayDefaultIndex
    WHERE b.LookupType = 'QuantityDisplay'

    UPDATE a
    SET a.WatchdogModeName = b.LookupName
    FROM staging.tblSites a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupWatchdogModeIndex
    WHERE b.LookupType = 'QuantityDisplay'



    IF ((SELECT
        COUNT(*)
      FROM staging.tblSites
      WHERE (LevelUnitIndex IS NOT NULL
      AND LevelUnitName IS NULL)
      OR (TemperatureUnitIndex IS NOT NULL
      AND TemperatureUnitName IS NULL)
      OR (DensityUnitIndex IS NOT NULL
      AND DensityUnitName IS NULL)
      OR (PressureUnitIndex IS NOT NULL
      AND PressureUnitName IS NULL)
      OR (FlowUnitIndex IS NOT NULL
      AND FlowUnitName IS NULL)
      OR (VolumeUnitIndex IS NOT NULL
      AND VolumeUnitName IS NULL)
      OR (MassUnitIndex IS NOT NULL
      AND MassUnitName IS NULL)
      OR (AdditiveVolumeUnitIndex IS NOT NULL
      AND AdditiveVolumeUnitName IS NULL)
      OR (AdditiveProfileCycleAmountUnitIndex IS NOT NULL
      AND AdditiveProfileCycleAmountUnitName IS NULL)
      OR (AdditiveProfileRateUnitIndex IS NOT NULL
      AND AdditiveProfileRateUnitName IS NULL)
      OR (LookupNumberGroupSizesTypeIndex IS NOT NULL
      AND NumberGroupSizesTypeName IS NULL)
      OR (LookupSecondaryStorageFillMethodIndex IS NOT NULL
      AND SecondaryStorageFillMethodName IS NULL)
      OR (LookupMailConnectModeIndex IS NOT NULL
      AND MailConnectModeName IS NULL)
      OR (LookupQuantityDisplayDefaultIndex IS NOT NULL
      AND QuantityDisplayDefaultName IS NULL)
      OR (LookupWatchdogModeIndex IS NOT NULL
      AND WatchdogModeName IS NULL))
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Site-Lookup references', 16, 1);
      RETURN;
    END


    
    -- ApplicationString Lookup references
    UPDATE a
    SET a.ApplicationStringTypeName = b.LookupName
    FROM staging.tblApplicationString a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupApplicationStringTypeIndex
    WHERE b.LookupType = 'ApplicationStringType'

    IF ((SELECT
        COUNT(*)
      FROM staging.tblApplicationString
      WHERE (LookupApplicationStringTypeIndex IS NOT NULL
      AND ApplicationStringTypeName IS NULL))
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve ApplicationString-Lookup references', 16, 1);
      RETURN;
    END
    


    -- Product Lookup references
    UPDATE a
    SET a.LevelUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LevelUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.TemperatureUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.TemperatureUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.DensityUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.DensityUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.PressureUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.PressureUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.FlowUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.FlowUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.VolumeUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.VolumeUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.MassUnitName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.MassUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.ProductTypeName = b.LookupName
    FROM staging.tblProducts a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupProductTypeIndex
    WHERE b.LookupType = 'ProductType'

    IF ((SELECT
        COUNT(*)
      FROM staging.tblProducts
      WHERE (LevelUnitIndex IS NOT NULL
      AND LevelUnitName IS NULL)
      OR (TemperatureUnitIndex IS NOT NULL
      AND TemperatureUnitName IS NULL)
      OR (DensityUnitIndex IS NOT NULL
      AND DensityUnitName IS NULL)
      OR (PressureUnitIndex IS NOT NULL
      AND PressureUnitName IS NULL)
      OR (FlowUnitIndex IS NOT NULL
      AND FlowUnitName IS NULL)
      OR (VolumeUnitIndex IS NOT NULL
      AND VolumeUnitName IS NULL)
      OR (MassUnitIndex IS NOT NULL
      AND MassUnitName IS NULL)
      OR (LookupProductTypeIndex IS NOT NULL
      AND ProductTypeName IS NULL))
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Product-Lookup references', 16, 1);
      RETURN;
    END

    -- Company Lookup references
    -- No Lookup references




    -- Equipment Lookup references		
    UPDATE a
    SET a.TemperatureUnitName = b.LookupName
    FROM staging.tblEquipment a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.TemperatureUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.DensityUnitName = b.LookupName
    FROM staging.tblEquipment a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.DensityUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.VolumeUnitName = b.LookupName
    FROM staging.tblEquipment a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.VolumeUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    UPDATE a
    SET a.MassUnitName = b.LookupName
    FROM staging.tblEquipment a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.MassUnitIndex
    WHERE b.LookupType = 'EngineeringUnit'

    IF ((SELECT
        COUNT(*)
      FROM staging.tblEquipment
      WHERE (TemperatureUnitIndex IS NOT NULL
      AND TemperatureUnitName IS NULL)
      OR (DensityUnitIndex IS NOT NULL
      AND DensityUnitName IS NULL)
      OR (VolumeUnitIndex IS NOT NULL
      AND VolumeUnitName IS NULL)
      OR (MassUnitIndex IS NOT NULL
      AND MassUnitName IS NULL))
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve Equipment-Lookup references', 16, 1);
      RETURN;
    END


    -- EquipmentType Lookup references	
    UPDATE a
    SET a.LookupEquipmentTypeName = b.LookupName
    FROM staging.tblEquipmentTypes a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.LookupEquipmentTypeIndex
    WHERE b.LookupType = 'EquipmentType'

    IF ((SELECT
        COUNT(*)
      FROM staging.tblEquipmentTypes
      WHERE (LookupEquipmentTypeIndex IS NOT NULL
      AND LookupEquipmentTypeName IS NULL))
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve EquipmentType-Lookup references', 16, 1);
      RETURN;
    END


    -- Station Lookup references	
    UPDATE a
    SET a.StationInterfaceTypeCode = b.LookupCode
    FROM staging.tblStations a
    INNER JOIN lookup.tblLookup b
    ON b.LookupIndex = a.LookupStationInterfaceTypeIndex
    WHERE b.LookupType = 'StationInterfaceType'

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblStations
            WHERE 
            LookupStationInterfaceTypeIndex IS NOT NULL
            AND StationInterfaceTypeCode IS NULL
        ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve Station-Lookup references', 16, 1);
      RETURN;
    END


    -- Tank Lookup references	
    UPDATE a
    SET a.DeviceTankTypeName = b.LookupCode
    FROM staging.tblTanks a
    INNER JOIN lookup.tblLookup b
    ON b.LookupIndex = a.LookupDeviceTankTypeIndex
    WHERE b.LookupType = 'DeviceTankType'

    UPDATE a
    SET a.VesselTypeName = b.LookupCode
    FROM staging.tblTanks a
    INNER JOIN lookup.tblLookup b
    ON b.LookupIndex = a.LookupVesselTypeIndex
    WHERE b.LookupType = 'VesselType'

    IF 
    (
        (
            SELECT COUNT(*) FROM staging.tblTanks
            WHERE 
            (
                (LookupDeviceTankTypeIndex IS NOT NULL
                AND DeviceTankTypeName IS NULL)
                OR (LookupVesselTypeIndex IS NOT NULL
                AND VesselTypeName IS NULL))
        ) > 0
    )
    BEGIN
      RAISERROR ('Failure to resolve Tank-Lookup references', 16, 1);
      RETURN;
    END



    /*
    -- FuelCard Lookup references		
    UPDATE a
    SET a.ActivationStatusName = b.LookupName
    FROM staging.tblFuelCards a
    INNER JOIN lookup.tblLookup b
      ON b.LookupIndex = a.ActivationStatusIndex
    WHERE b.LookupType = 'ActivationStatus'


    IF ((SELECT
        COUNT(*)
      FROM staging.tblFuelCards
      WHERE (ActivationStatusIndex IS NOT NULL
      AND ActivationStatusName IS NULL))
      > 0)
    BEGIN
      RAISERROR ('Failure to resolve FuelCard-Lookup references', 16, 1);
      RETURN;
    END
    */


  END TRY
  BEGIN CATCH
    DECLARE @_ErrMessage nvarchar(2048),
            @_ErrNumber int,
            @_ErrProcName nvarchar(126),
            @_ErrLineNumber int;
    SET @_ErrMessage = ERROR_MESSAGE();
    SET @_ErrNumber = ERROR_NUMBER();
    SET @_ErrProcName = ERROR_PROCEDURE();
    SET @_ErrLineNumber = ERROR_LINE();
    SET @_ErrMessage = 'Error: ' + @_ErrMessage + CHAR(13) + CHAR(10)
    + 'Number: ' + CAST(@_ErrNumber AS varchar(20)) + CHAR(13) + CHAR(10)
    + 'Procedure Name: [staging].[usp_SetLookupReferencesForDimensions]' + CHAR(13) + CHAR(10)
    + 'Line Number: ' + ISNULL(CAST(@_ErrLineNumber AS varchar(20)), '') + CHAR(13) + CHAR(10);
    RAISERROR (@_ErrMessage, 16, 1);
  END CATCH

END
GO
