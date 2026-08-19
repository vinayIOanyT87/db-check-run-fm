-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipment
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblEquipment]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ID nvarchar(30),
@Description nvarchar(50),
@Make nvarchar(20),
@Model nvarchar(50),
@Year int,
@IssPtNum nvarchar(20),
@Fixed bit,
@StorageType nvarchar(2),
@InUse bit,
@FixedVolume bit,
@IntoPlane bit,
@Mobile bit,
@AttachedTo nvarchar(6),
@MediaType char(1),
@Meters int,
@DefuelMeterForwards bit,
@PulseRatio float,
@Round bit,
@Xref nvarchar(10),
@LowStockWarning float,
@StockTrack bit,
@Totalisor1 nvarchar(10),
@Totalisor2 nvarchar(10),
@FuelingState nvarchar(10),
@Volume float,
@MeterReading float,
@Consecutive_OOS_Variance int,
@Notes nvarchar(1000),
@Capacity float,
@SafeFill float,
@VolumeUnitIndex int,
@TemperatureUnitIndex int,
@DensityUnitIndex int,
@MassUnitIndex int,
@VolumeDecimalPlaces tinyint,
@TemperatureDecimalPlaces tinyint,
@DensityDecimalPlaces tinyint,
@MassDecimalPlaces tinyint,
@EquipmentSequence nvarchar(50),
@LockedOut bit,
@LockedOutReason nvarchar(80),
@LockedOutDate datetimeoffset(7),
@SerialNumber nvarchar(30),
@CompanyEquipmentID nvarchar(30),
@TruckCardNumber nvarchar(32),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@RatedGPM float,
@ActualGPM float,
@FuelAdditiveFlag bit,
@ManufactureDate datetimeoffset(7),
@InstallationDate datetimeoffset(7),
@InspectionDate datetimeoffset(7),
@CalibrationDate datetimeoffset(7),
@QCDate datetimeoffset(7),
@SecondaryStorageFlag bit,
@ManagedEquipmentFlag bit,
@FuelingType smallint,
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@UserData9 nvarchar(60),
@UserData10 nvarchar(60),
@UserData11 nvarchar(60),
@UserData12 nvarchar(60),
@UserData13 nvarchar(60),
@UserData14 nvarchar(60),
@UserData15 nvarchar(60),
@UserData16 nvarchar(60),
@UserData17 nvarchar(60),
@UserData18 nvarchar(60),
@UserData19 nvarchar(60),
@UserData20 nvarchar(60),
@UserData21 nvarchar(60),
@UserData22 nvarchar(60),
@UserData23 nvarchar(60),
@UserData24 nvarchar(60),
@EquipmentGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@CompanyGuid uniqueidentifier,
@ParentEquipmentGuid uniqueidentifier,
@EquipmentTypeGuid uniqueidentifier,
@FuelCardGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@AssignedToMeterGuid uniqueidentifier,
@AssetTrackingDeviceGuid uniqueidentifier,
@_MasterRecordGuid uniqueidentifier,
@HiddenDate datetimeoffset(7),
@ScullyRequired bit,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblEquipment varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblEquipment] CT
                        WHERE CT.PK_EquipmentGuid = @EquipmentGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblEquipment].[ID],[dbo].[tblEquipment].[Description],[dbo].[tblEquipment].[Make],[dbo].[tblEquipment].[Model],[dbo].[tblEquipment].[Year],[dbo].[tblEquipment].[IssPtNum],[dbo].[tblEquipment].[Fixed],[dbo].[tblEquipment].[StorageType],[dbo].[tblEquipment].[InUse],[dbo].[tblEquipment].[FixedVolume],[dbo].[tblEquipment].[IntoPlane],[dbo].[tblEquipment].[Mobile],[dbo].[tblEquipment].[AttachedTo],[dbo].[tblEquipment].[MediaType],[dbo].[tblEquipment].[Meters],[dbo].[tblEquipment].[DefuelMeterForwards],[dbo].[tblEquipment].[PulseRatio],[dbo].[tblEquipment].[Round],[dbo].[tblEquipment].[Xref],[dbo].[tblEquipment].[LowStockWarning],[dbo].[tblEquipment].[StockTrack],[dbo].[tblEquipment].[Totalisor1],[dbo].[tblEquipment].[Totalisor2],[dbo].[tblEquipment].[FuelingState],[dbo].[tblEquipment].[Volume],[dbo].[tblEquipment].[MeterReading],[dbo].[tblEquipment].[Consecutive_OOS_Variance],[dbo].[tblEquipment].[Notes],[dbo].[tblEquipment].[Capacity],[dbo].[tblEquipment].[SafeFill],[dbo].[tblEquipment].[VolumeUnitIndex],[dbo].[tblEquipment].[TemperatureUnitIndex],[dbo].[tblEquipment].[DensityUnitIndex],[dbo].[tblEquipment].[MassUnitIndex],[dbo].[tblEquipment].[VolumeDecimalPlaces],[dbo].[tblEquipment].[TemperatureDecimalPlaces],[dbo].[tblEquipment].[DensityDecimalPlaces],[dbo].[tblEquipment].[MassDecimalPlaces],[dbo].[tblEquipment].[EquipmentSequence],[dbo].[tblEquipment].[LockedOut],[dbo].[tblEquipment].[LockedOutReason],[dbo].[tblEquipment].[LockedOutDate],[dbo].[tblEquipment].[SerialNumber],[dbo].[tblEquipment].[CompanyEquipmentID],[dbo].[tblEquipment].[TruckCardNumber],[dbo].[tblEquipment].[CreatedDate],[dbo].[tblEquipment].[CreatedBy],[dbo].[tblEquipment].[UpdatedDate],[dbo].[tblEquipment].[UpdatedBy],[dbo].[tblEquipment].[RatedGPM],[dbo].[tblEquipment].[ActualGPM],[dbo].[tblEquipment].[FuelAdditiveFlag],[dbo].[tblEquipment].[ManufactureDate],[dbo].[tblEquipment].[InstallationDate],[dbo].[tblEquipment].[InspectionDate],[dbo].[tblEquipment].[CalibrationDate],[dbo].[tblEquipment].[QCDate],[dbo].[tblEquipment].[SecondaryStorageFlag],[dbo].[tblEquipment].[ManagedEquipmentFlag],[dbo].[tblEquipment].[FuelingType],[dbo].[tblEquipment].[UserData1],[dbo].[tblEquipment].[UserData2],[dbo].[tblEquipment].[UserData3],[dbo].[tblEquipment].[UserData4],[dbo].[tblEquipment].[UserData5],[dbo].[tblEquipment].[UserData6],[dbo].[tblEquipment].[UserData7],[dbo].[tblEquipment].[UserData8],[dbo].[tblEquipment].[UserData9],[dbo].[tblEquipment].[UserData10],[dbo].[tblEquipment].[UserData11],[dbo].[tblEquipment].[UserData12],[dbo].[tblEquipment].[UserData13],[dbo].[tblEquipment].[UserData14],[dbo].[tblEquipment].[UserData15],[dbo].[tblEquipment].[UserData16],[dbo].[tblEquipment].[UserData17],[dbo].[tblEquipment].[UserData18],[dbo].[tblEquipment].[UserData19],[dbo].[tblEquipment].[UserData20],[dbo].[tblEquipment].[UserData21],[dbo].[tblEquipment].[UserData22],[dbo].[tblEquipment].[UserData23],[dbo].[tblEquipment].[UserData24],[dbo].[tblEquipment].[EquipmentGuid],[dbo].[tblEquipment].[SiteGuid],[dbo].[tblEquipment].[CompanyGuid],[dbo].[tblEquipment].[ParentEquipmentGuid],[dbo].[tblEquipment].[EquipmentTypeGuid],[dbo].[tblEquipment].[FuelCardGuid],[dbo].[tblEquipment].[ProductGuid],[dbo].[tblEquipment].[AssignedToMeterGuid],[dbo].[tblEquipment].[AssetTrackingDeviceGuid],[dbo].[tblEquipment].[_MasterRecordGuid],[dbo].[tblEquipment].[HiddenDate],[dbo].[tblEquipment].[ScullyRequired]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblEquipment]
                        INNER JOIN [track].[tblEquipment] CT
                            ON CT.PK_EquipmentGuid = [dbo].[tblEquipment].[EquipmentGuid] 
                    WHERE CT.PK_EquipmentGuid = @EquipmentGuid
            ) MERGE existingData
            USING (SELECT @ID,@Description,@Make,@Model,@Year,@IssPtNum,@Fixed,@StorageType,@InUse,@FixedVolume,@IntoPlane,@Mobile,@AttachedTo,@MediaType,@Meters,@DefuelMeterForwards,@PulseRatio,@Round,@Xref,@LowStockWarning,@StockTrack,@Totalisor1,@Totalisor2,@FuelingState,@Volume,@MeterReading,@Consecutive_OOS_Variance,@Notes,@Capacity,@SafeFill,@VolumeUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@MassUnitIndex,@VolumeDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@MassDecimalPlaces,@EquipmentSequence,@LockedOut,@LockedOutReason,@LockedOutDate,@SerialNumber,@CompanyEquipmentID,@TruckCardNumber,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@RatedGPM,@ActualGPM,@FuelAdditiveFlag,@ManufactureDate,@InstallationDate,@InspectionDate,@CalibrationDate,@QCDate,@SecondaryStorageFlag,@ManagedEquipmentFlag,@FuelingType,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@UserData9,@UserData10,@UserData11,@UserData12,@UserData13,@UserData14,@UserData15,@UserData16,@UserData17,@UserData18,@UserData19,@UserData20,@UserData21,@UserData22,@UserData23,@UserData24,@EquipmentGuid,@SiteGuid,@CompanyGuid,@ParentEquipmentGuid,@EquipmentTypeGuid,@FuelCardGuid,@ProductGuid,@AssignedToMeterGuid,@AssetTrackingDeviceGuid,@_MasterRecordGuid,@HiddenDate,@ScullyRequired
                    ) AS remoteChanges ([ID],[Description],[Make],[Model],[Year],[IssPtNum],[Fixed],[StorageType],[InUse],[FixedVolume],[IntoPlane],[Mobile],[AttachedTo],[MediaType],[Meters],[DefuelMeterForwards],[PulseRatio],[Round],[Xref],[LowStockWarning],[StockTrack],[Totalisor1],[Totalisor2],[FuelingState],[Volume],[MeterReading],[Consecutive_OOS_Variance],[Notes],[Capacity],[SafeFill],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[MassUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[MassDecimalPlaces],[EquipmentSequence],[LockedOut],[LockedOutReason],[LockedOutDate],[SerialNumber],[CompanyEquipmentID],[TruckCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[RatedGPM],[ActualGPM],[FuelAdditiveFlag],[ManufactureDate],[InstallationDate],[InspectionDate],[CalibrationDate],[QCDate],[SecondaryStorageFlag],[ManagedEquipmentFlag],[FuelingType],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[EquipmentGuid],[SiteGuid],[CompanyGuid],[ParentEquipmentGuid],[EquipmentTypeGuid],[FuelCardGuid],[ProductGuid],[AssignedToMeterGuid],[AssetTrackingDeviceGuid],[_MasterRecordGuid],[HiddenDate],[ScullyRequired])
            ON (existingData.[EquipmentGuid] = remoteChanges.[EquipmentGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ID'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ID] ELSE remoteChanges.[ID] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[Make] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Make'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Make] ELSE remoteChanges.[Make] END
                       ,[Model] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Model'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Model] ELSE remoteChanges.[Model] END
                       ,[Year] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Year'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Year] ELSE remoteChanges.[Year] END
                       ,[IssPtNum] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssPtNum'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[IssPtNum] ELSE remoteChanges.[IssPtNum] END
                       ,[Fixed] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Fixed'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Fixed] ELSE remoteChanges.[Fixed] END
                       ,[StorageType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StorageType'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[StorageType] ELSE remoteChanges.[StorageType] END
                       ,[InUse] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InUse'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[InUse] ELSE remoteChanges.[InUse] END
                       ,[FixedVolume] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FixedVolume'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[FixedVolume] ELSE remoteChanges.[FixedVolume] END
                       ,[IntoPlane] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IntoPlane'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[IntoPlane] ELSE remoteChanges.[IntoPlane] END
                       ,[Mobile] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Mobile'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Mobile] ELSE remoteChanges.[Mobile] END
                       ,[AttachedTo] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AttachedTo'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[AttachedTo] ELSE remoteChanges.[AttachedTo] END
                       ,[MediaType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MediaType'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[MediaType] ELSE remoteChanges.[MediaType] END
                       ,[Meters] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Meters'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Meters] ELSE remoteChanges.[Meters] END
                       ,[DefuelMeterForwards] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DefuelMeterForwards'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[DefuelMeterForwards] ELSE remoteChanges.[DefuelMeterForwards] END
                       ,[PulseRatio] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PulseRatio'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[PulseRatio] ELSE remoteChanges.[PulseRatio] END
                       ,[Round] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Round'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Round] ELSE remoteChanges.[Round] END
                       ,[Xref] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Xref'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Xref] ELSE remoteChanges.[Xref] END
                       ,[LowStockWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowStockWarning'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[LowStockWarning] ELSE remoteChanges.[LowStockWarning] END
                       ,[StockTrack] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockTrack'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[StockTrack] ELSE remoteChanges.[StockTrack] END
                       ,[Totalisor1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Totalisor1'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Totalisor1] ELSE remoteChanges.[Totalisor1] END
                       ,[Totalisor2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Totalisor2'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Totalisor2] ELSE remoteChanges.[Totalisor2] END
                       ,[FuelingState] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelingState'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[FuelingState] ELSE remoteChanges.[FuelingState] END
                       ,[Volume] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Volume'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Volume] ELSE remoteChanges.[Volume] END
                       ,[MeterReading] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterReading'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[MeterReading] ELSE remoteChanges.[MeterReading] END
                       ,[Consecutive_OOS_Variance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Consecutive_OOS_Variance'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Consecutive_OOS_Variance] ELSE remoteChanges.[Consecutive_OOS_Variance] END
                       ,[Notes] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Notes'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Notes] ELSE remoteChanges.[Notes] END
                       ,[Capacity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Capacity'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[Capacity] ELSE remoteChanges.[Capacity] END
                       ,[SafeFill] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SafeFill'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[SafeFill] ELSE remoteChanges.[SafeFill] END
                       ,[VolumeUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[VolumeUnitIndex] ELSE remoteChanges.[VolumeUnitIndex] END
                       ,[TemperatureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[TemperatureUnitIndex] ELSE remoteChanges.[TemperatureUnitIndex] END
                       ,[DensityUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[DensityUnitIndex] ELSE remoteChanges.[DensityUnitIndex] END
                       ,[MassUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[MassUnitIndex] ELSE remoteChanges.[MassUnitIndex] END
                       ,[VolumeDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[VolumeDecimalPlaces] ELSE remoteChanges.[VolumeDecimalPlaces] END
                       ,[TemperatureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[TemperatureDecimalPlaces] ELSE remoteChanges.[TemperatureDecimalPlaces] END
                       ,[DensityDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[DensityDecimalPlaces] ELSE remoteChanges.[DensityDecimalPlaces] END
                       ,[MassDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[MassDecimalPlaces] ELSE remoteChanges.[MassDecimalPlaces] END
                       ,[EquipmentSequence] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentSequence'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[EquipmentSequence] ELSE remoteChanges.[EquipmentSequence] END
                       ,[LockedOut] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[LockedOut] ELSE remoteChanges.[LockedOut] END
                       ,[LockedOutReason] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[LockedOutReason] ELSE remoteChanges.[LockedOutReason] END
                       ,[LockedOutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[LockedOutDate] ELSE remoteChanges.[LockedOutDate] END
                       ,[SerialNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SerialNumber'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[SerialNumber] ELSE remoteChanges.[SerialNumber] END
                       ,[CompanyEquipmentID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyEquipmentID'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[CompanyEquipmentID] ELSE remoteChanges.[CompanyEquipmentID] END
                       ,[TruckCardNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TruckCardNumber'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[TruckCardNumber] ELSE remoteChanges.[TruckCardNumber] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[RatedGPM] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RatedGPM'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[RatedGPM] ELSE remoteChanges.[RatedGPM] END
                       ,[ActualGPM] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ActualGPM'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ActualGPM] ELSE remoteChanges.[ActualGPM] END
                       ,[FuelAdditiveFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelAdditiveFlag'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[FuelAdditiveFlag] ELSE remoteChanges.[FuelAdditiveFlag] END
                       ,[ManufactureDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManufactureDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ManufactureDate] ELSE remoteChanges.[ManufactureDate] END
                       ,[InstallationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InstallationDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[InstallationDate] ELSE remoteChanges.[InstallationDate] END
                       ,[InspectionDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InspectionDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[InspectionDate] ELSE remoteChanges.[InspectionDate] END
                       ,[CalibrationDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CalibrationDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[CalibrationDate] ELSE remoteChanges.[CalibrationDate] END
                       ,[QCDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QCDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[QCDate] ELSE remoteChanges.[QCDate] END
                       ,[SecondaryStorageFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SecondaryStorageFlag'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[SecondaryStorageFlag] ELSE remoteChanges.[SecondaryStorageFlag] END
                       ,[ManagedEquipmentFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManagedEquipmentFlag'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ManagedEquipmentFlag] ELSE remoteChanges.[ManagedEquipmentFlag] END
                       ,[FuelingType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelingType'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[FuelingType] ELSE remoteChanges.[FuelingType] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[UserData9] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData9'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData9] ELSE remoteChanges.[UserData9] END
                       ,[UserData10] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData10'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData10] ELSE remoteChanges.[UserData10] END
                       ,[UserData11] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData11'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData11] ELSE remoteChanges.[UserData11] END
                       ,[UserData12] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData12'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData12] ELSE remoteChanges.[UserData12] END
                       ,[UserData13] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData13'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData13] ELSE remoteChanges.[UserData13] END
                       ,[UserData14] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData14'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData14] ELSE remoteChanges.[UserData14] END
                       ,[UserData15] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData15'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData15] ELSE remoteChanges.[UserData15] END
                       ,[UserData16] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData16'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData16] ELSE remoteChanges.[UserData16] END
                       ,[UserData17] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData17'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData17] ELSE remoteChanges.[UserData17] END
                       ,[UserData18] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData18'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData18] ELSE remoteChanges.[UserData18] END
                       ,[UserData19] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData19'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData19] ELSE remoteChanges.[UserData19] END
                       ,[UserData20] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData20'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData20] ELSE remoteChanges.[UserData20] END
                       ,[UserData21] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData21'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData21] ELSE remoteChanges.[UserData21] END
                       ,[UserData22] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData22'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData22] ELSE remoteChanges.[UserData22] END
                       ,[UserData23] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData23'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData23] ELSE remoteChanges.[UserData23] END
                       ,[UserData24] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData24'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[UserData24] ELSE remoteChanges.[UserData24] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[CompanyGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[CompanyGuid] ELSE remoteChanges.[CompanyGuid] END
                       ,[ParentEquipmentGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParentEquipmentGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ParentEquipmentGuid] ELSE remoteChanges.[ParentEquipmentGuid] END
                       ,[EquipmentTypeGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentTypeGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[EquipmentTypeGuid] ELSE remoteChanges.[EquipmentTypeGuid] END
                       ,[FuelCardGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelCardGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[FuelCardGuid] ELSE remoteChanges.[FuelCardGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[AssignedToMeterGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedToMeterGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[AssignedToMeterGuid] ELSE remoteChanges.[AssignedToMeterGuid] END
                       ,[AssetTrackingDeviceGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssetTrackingDeviceGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[AssetTrackingDeviceGuid] ELSE remoteChanges.[AssetTrackingDeviceGuid] END
                       ,[_MasterRecordGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('_MasterRecordGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[_MasterRecordGuid] ELSE remoteChanges.[_MasterRecordGuid] END
                       ,[HiddenDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[HiddenDate] ELSE remoteChanges.[HiddenDate] END
                       ,[ScullyRequired] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ScullyRequired'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN existingData.[ScullyRequired] ELSE remoteChanges.[ScullyRequired] END

            WHEN NOT MATCHED THEN
                INSERT ([ID],[Description],[Make],[Model],[Year],[IssPtNum],[Fixed],[StorageType],[InUse],[FixedVolume],[IntoPlane],[Mobile],[AttachedTo],[MediaType],[Meters],[DefuelMeterForwards],[PulseRatio],[Round],[Xref],[LowStockWarning],[StockTrack],[Totalisor1],[Totalisor2],[FuelingState],[Volume],[MeterReading],[Consecutive_OOS_Variance],[Notes],[Capacity],[SafeFill],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[MassUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[MassDecimalPlaces],[EquipmentSequence],[LockedOut],[LockedOutReason],[LockedOutDate],[SerialNumber],[CompanyEquipmentID],[TruckCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[RatedGPM],[ActualGPM],[FuelAdditiveFlag],[ManufactureDate],[InstallationDate],[InspectionDate],[CalibrationDate],[QCDate],[SecondaryStorageFlag],[ManagedEquipmentFlag],[FuelingType],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[EquipmentGuid],[SiteGuid],[CompanyGuid],[ParentEquipmentGuid],[EquipmentTypeGuid],[FuelCardGuid],[ProductGuid],[AssignedToMeterGuid],[AssetTrackingDeviceGuid],[_MasterRecordGuid],[HiddenDate],[ScullyRequired])
                    VALUES (@ID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Description END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Make'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Make END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Model'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Model END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Year'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Year END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IssPtNum'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @IssPtNum END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Fixed'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Fixed END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StorageType'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @StorageType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InUse'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @InUse END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FixedVolume'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @FixedVolume END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IntoPlane'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @IntoPlane END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Mobile'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Mobile END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AttachedTo'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @AttachedTo END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MediaType'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @MediaType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Meters'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Meters END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DefuelMeterForwards'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @DefuelMeterForwards END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PulseRatio'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @PulseRatio END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Round'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Round END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Xref'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Xref END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowStockWarning'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @LowStockWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockTrack'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @StockTrack END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Totalisor1'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Totalisor1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Totalisor2'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Totalisor2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelingState'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @FuelingState END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Volume'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Volume END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterReading'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @MeterReading END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Consecutive_OOS_Variance'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Consecutive_OOS_Variance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Notes'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Notes END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Capacity'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @Capacity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SafeFill'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @SafeFill END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @VolumeUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @TemperatureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @DensityUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @MassUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @VolumeDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @TemperatureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @DensityDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @MassDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentSequence'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @EquipmentSequence END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @LockedOut END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @LockedOutReason END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @LockedOutDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SerialNumber'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @SerialNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyEquipmentID'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @CompanyEquipmentID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TruckCardNumber'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @TruckCardNumber END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RatedGPM'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @RatedGPM END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ActualGPM'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @ActualGPM END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelAdditiveFlag'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @FuelAdditiveFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ManufactureDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @ManufactureDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InstallationDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @InstallationDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InspectionDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @InspectionDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CalibrationDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @CalibrationDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('QCDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @QCDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SecondaryStorageFlag'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @SecondaryStorageFlag END),@ManagedEquipmentFlag,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelingType'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @FuelingType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData8 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData9'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData9 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData10'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData10 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData11'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData11 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData12'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData12 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData13'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData13 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData14'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData14 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData15'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData15 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData16'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData16 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData17'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData17 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData18'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData18 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData19'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData19 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData20'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData20 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData21'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData21 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData22'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData22 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData23'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData23 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData24'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @UserData24 END),@EquipmentGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CompanyGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @CompanyGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ParentEquipmentGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @ParentEquipmentGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('EquipmentTypeGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @EquipmentTypeGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FuelCardGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @FuelCardGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @ProductGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssignedToMeterGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @AssignedToMeterGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AssetTrackingDeviceGuid'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @AssetTrackingDeviceGuid END),@_MasterRecordGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblEquipment)) WHEN 0 THEN NULL ELSE @HiddenDate END),@ScullyRequired)
            ;
         SET @sync_row_count = @@rowcount;
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END

    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @EquipmentGuid)
        END
        SET NOCOUNT OFF
    END

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblEquipment] WHERE EquipmentGuid = @EquipmentGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;
    END
    
    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor 
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
