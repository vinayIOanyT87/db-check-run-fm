-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblEquipment
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblEquipment]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
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
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblEquipment] AS existingData
        USING (SELECT @ID 'ID',@Description 'Description',@Make 'Make',@Model 'Model',@Year 'Year',@IssPtNum 'IssPtNum',@Fixed 'Fixed',@StorageType 'StorageType',@InUse 'InUse',@FixedVolume 'FixedVolume',@IntoPlane 'IntoPlane',@Mobile 'Mobile',@AttachedTo 'AttachedTo',@MediaType 'MediaType',@Meters 'Meters',@DefuelMeterForwards 'DefuelMeterForwards',@PulseRatio 'PulseRatio',@Round 'Round',@Xref 'Xref',@LowStockWarning 'LowStockWarning',@StockTrack 'StockTrack',@Totalisor1 'Totalisor1',@Totalisor2 'Totalisor2',@FuelingState 'FuelingState',@Volume 'Volume',@MeterReading 'MeterReading',@Consecutive_OOS_Variance 'Consecutive_OOS_Variance',@Notes 'Notes',@Capacity 'Capacity',@SafeFill 'SafeFill',@VolumeUnitIndex 'VolumeUnitIndex',@TemperatureUnitIndex 'TemperatureUnitIndex',@DensityUnitIndex 'DensityUnitIndex',@MassUnitIndex 'MassUnitIndex',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@TemperatureDecimalPlaces 'TemperatureDecimalPlaces',@DensityDecimalPlaces 'DensityDecimalPlaces',@MassDecimalPlaces 'MassDecimalPlaces',@EquipmentSequence 'EquipmentSequence',@LockedOut 'LockedOut',@LockedOutReason 'LockedOutReason',@LockedOutDate 'LockedOutDate',@SerialNumber 'SerialNumber',@CompanyEquipmentID 'CompanyEquipmentID',@TruckCardNumber 'TruckCardNumber',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@RatedGPM 'RatedGPM',@ActualGPM 'ActualGPM',@FuelAdditiveFlag 'FuelAdditiveFlag',@ManufactureDate 'ManufactureDate',@InstallationDate 'InstallationDate',@InspectionDate 'InspectionDate',@CalibrationDate 'CalibrationDate',@QCDate 'QCDate',@SecondaryStorageFlag 'SecondaryStorageFlag',@ManagedEquipmentFlag 'ManagedEquipmentFlag',@FuelingType 'FuelingType',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@UserData9 'UserData9',@UserData10 'UserData10',@UserData11 'UserData11',@UserData12 'UserData12',@UserData13 'UserData13',@UserData14 'UserData14',@UserData15 'UserData15',@UserData16 'UserData16',@UserData17 'UserData17',@UserData18 'UserData18',@UserData19 'UserData19',@UserData20 'UserData20',@UserData21 'UserData21',@UserData22 'UserData22',@UserData23 'UserData23',@UserData24 'UserData24',@EquipmentGuid 'EquipmentGuid',@SiteGuid 'SiteGuid',@CompanyGuid 'CompanyGuid',@ParentEquipmentGuid 'ParentEquipmentGuid',@EquipmentTypeGuid 'EquipmentTypeGuid',@FuelCardGuid 'FuelCardGuid',@ProductGuid 'ProductGuid',@AssignedToMeterGuid 'AssignedToMeterGuid',@AssetTrackingDeviceGuid 'AssetTrackingDeviceGuid',@_MasterRecordGuid '_MasterRecordGuid',@HiddenDate 'HiddenDate',@ScullyRequired 'ScullyRequired'
                ) AS remoteChanges ([ID],[Description],[Make],[Model],[Year],[IssPtNum],[Fixed],[StorageType],[InUse],[FixedVolume],[IntoPlane],[Mobile],[AttachedTo],[MediaType],[Meters],[DefuelMeterForwards],[PulseRatio],[Round],[Xref],[LowStockWarning],[StockTrack],[Totalisor1],[Totalisor2],[FuelingState],[Volume],[MeterReading],[Consecutive_OOS_Variance],[Notes],[Capacity],[SafeFill],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[MassUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[MassDecimalPlaces],[EquipmentSequence],[LockedOut],[LockedOutReason],[LockedOutDate],[SerialNumber],[CompanyEquipmentID],[TruckCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[RatedGPM],[ActualGPM],[FuelAdditiveFlag],[ManufactureDate],[InstallationDate],[InspectionDate],[CalibrationDate],[QCDate],[SecondaryStorageFlag],[ManagedEquipmentFlag],[FuelingType],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[EquipmentGuid],[SiteGuid],[CompanyGuid],[ParentEquipmentGuid],[EquipmentTypeGuid],[FuelCardGuid],[ProductGuid],[AssignedToMeterGuid],[AssetTrackingDeviceGuid],[_MasterRecordGuid],[HiddenDate],[ScullyRequired])
        ON (existingData.[EquipmentGuid] = remoteChanges.[EquipmentGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ID] = remoteChanges.[ID]
                       ,[Description] = remoteChanges.[Description]
                       ,[Make] = remoteChanges.[Make]
                       ,[Model] = remoteChanges.[Model]
                       ,[Year] = remoteChanges.[Year]
                       ,[IssPtNum] = remoteChanges.[IssPtNum]
                       ,[Fixed] = remoteChanges.[Fixed]
                       ,[StorageType] = remoteChanges.[StorageType]
                       ,[InUse] = remoteChanges.[InUse]
                       ,[FixedVolume] = remoteChanges.[FixedVolume]
                       ,[IntoPlane] = remoteChanges.[IntoPlane]
                       ,[Mobile] = remoteChanges.[Mobile]
                       ,[AttachedTo] = remoteChanges.[AttachedTo]
                       ,[MediaType] = remoteChanges.[MediaType]
                       ,[Meters] = remoteChanges.[Meters]
                       ,[DefuelMeterForwards] = remoteChanges.[DefuelMeterForwards]
                       ,[PulseRatio] = remoteChanges.[PulseRatio]
                       ,[Round] = remoteChanges.[Round]
                       ,[Xref] = remoteChanges.[Xref]
                       ,[LowStockWarning] = remoteChanges.[LowStockWarning]
                       ,[StockTrack] = remoteChanges.[StockTrack]
                       ,[Totalisor1] = remoteChanges.[Totalisor1]
                       ,[Totalisor2] = remoteChanges.[Totalisor2]
                       ,[FuelingState] = remoteChanges.[FuelingState]
                       ,[Volume] = remoteChanges.[Volume]
                       ,[MeterReading] = remoteChanges.[MeterReading]
                       ,[Consecutive_OOS_Variance] = remoteChanges.[Consecutive_OOS_Variance]
                       ,[Notes] = remoteChanges.[Notes]
                       ,[Capacity] = remoteChanges.[Capacity]
                       ,[SafeFill] = remoteChanges.[SafeFill]
                       ,[VolumeUnitIndex] = remoteChanges.[VolumeUnitIndex]
                       ,[TemperatureUnitIndex] = remoteChanges.[TemperatureUnitIndex]
                       ,[DensityUnitIndex] = remoteChanges.[DensityUnitIndex]
                       ,[MassUnitIndex] = remoteChanges.[MassUnitIndex]
                       ,[VolumeDecimalPlaces] = remoteChanges.[VolumeDecimalPlaces]
                       ,[TemperatureDecimalPlaces] = remoteChanges.[TemperatureDecimalPlaces]
                       ,[DensityDecimalPlaces] = remoteChanges.[DensityDecimalPlaces]
                       ,[MassDecimalPlaces] = remoteChanges.[MassDecimalPlaces]
                       ,[EquipmentSequence] = remoteChanges.[EquipmentSequence]
                       ,[LockedOut] = remoteChanges.[LockedOut]
                       ,[LockedOutReason] = remoteChanges.[LockedOutReason]
                       ,[LockedOutDate] = remoteChanges.[LockedOutDate]
                       ,[SerialNumber] = remoteChanges.[SerialNumber]
                       ,[CompanyEquipmentID] = remoteChanges.[CompanyEquipmentID]
                       ,[TruckCardNumber] = remoteChanges.[TruckCardNumber]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[RatedGPM] = remoteChanges.[RatedGPM]
                       ,[ActualGPM] = remoteChanges.[ActualGPM]
                       ,[FuelAdditiveFlag] = remoteChanges.[FuelAdditiveFlag]
                       ,[ManufactureDate] = remoteChanges.[ManufactureDate]
                       ,[InstallationDate] = remoteChanges.[InstallationDate]
                       ,[InspectionDate] = remoteChanges.[InspectionDate]
                       ,[CalibrationDate] = remoteChanges.[CalibrationDate]
                       ,[QCDate] = remoteChanges.[QCDate]
                       ,[SecondaryStorageFlag] = remoteChanges.[SecondaryStorageFlag]
                       ,[ManagedEquipmentFlag] = remoteChanges.[ManagedEquipmentFlag]
                       ,[FuelingType] = remoteChanges.[FuelingType]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[UserData9] = remoteChanges.[UserData9]
                       ,[UserData10] = remoteChanges.[UserData10]
                       ,[UserData11] = remoteChanges.[UserData11]
                       ,[UserData12] = remoteChanges.[UserData12]
                       ,[UserData13] = remoteChanges.[UserData13]
                       ,[UserData14] = remoteChanges.[UserData14]
                       ,[UserData15] = remoteChanges.[UserData15]
                       ,[UserData16] = remoteChanges.[UserData16]
                       ,[UserData17] = remoteChanges.[UserData17]
                       ,[UserData18] = remoteChanges.[UserData18]
                       ,[UserData19] = remoteChanges.[UserData19]
                       ,[UserData20] = remoteChanges.[UserData20]
                       ,[UserData21] = remoteChanges.[UserData21]
                       ,[UserData22] = remoteChanges.[UserData22]
                       ,[UserData23] = remoteChanges.[UserData23]
                       ,[UserData24] = remoteChanges.[UserData24]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[CompanyGuid] = remoteChanges.[CompanyGuid]
                       ,[ParentEquipmentGuid] = remoteChanges.[ParentEquipmentGuid]
                       ,[EquipmentTypeGuid] = remoteChanges.[EquipmentTypeGuid]
                       ,[FuelCardGuid] = remoteChanges.[FuelCardGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[AssignedToMeterGuid] = remoteChanges.[AssignedToMeterGuid]
                       ,[AssetTrackingDeviceGuid] = remoteChanges.[AssetTrackingDeviceGuid]
                       ,[_MasterRecordGuid] = remoteChanges.[_MasterRecordGuid]
                       ,[HiddenDate] = remoteChanges.[HiddenDate]
                       ,[ScullyRequired] = remoteChanges.[ScullyRequired]

        WHEN NOT MATCHED THEN
            INSERT ([ID],[Description],[Make],[Model],[Year],[IssPtNum],[Fixed],[StorageType],[InUse],[FixedVolume],[IntoPlane],[Mobile],[AttachedTo],[MediaType],[Meters],[DefuelMeterForwards],[PulseRatio],[Round],[Xref],[LowStockWarning],[StockTrack],[Totalisor1],[Totalisor2],[FuelingState],[Volume],[MeterReading],[Consecutive_OOS_Variance],[Notes],[Capacity],[SafeFill],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[MassUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[MassDecimalPlaces],[EquipmentSequence],[LockedOut],[LockedOutReason],[LockedOutDate],[SerialNumber],[CompanyEquipmentID],[TruckCardNumber],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[RatedGPM],[ActualGPM],[FuelAdditiveFlag],[ManufactureDate],[InstallationDate],[InspectionDate],[CalibrationDate],[QCDate],[SecondaryStorageFlag],[ManagedEquipmentFlag],[FuelingType],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[UserData9],[UserData10],[UserData11],[UserData12],[UserData13],[UserData14],[UserData15],[UserData16],[UserData17],[UserData18],[UserData19],[UserData20],[UserData21],[UserData22],[UserData23],[UserData24],[EquipmentGuid],[SiteGuid],[CompanyGuid],[ParentEquipmentGuid],[EquipmentTypeGuid],[FuelCardGuid],[ProductGuid],[AssignedToMeterGuid],[AssetTrackingDeviceGuid],[_MasterRecordGuid],[HiddenDate],[ScullyRequired])
                VALUES (@ID,@Description,@Make,@Model,@Year,@IssPtNum,@Fixed,@StorageType,@InUse,@FixedVolume,@IntoPlane,@Mobile,@AttachedTo,@MediaType,@Meters,@DefuelMeterForwards,@PulseRatio,@Round,@Xref,@LowStockWarning,@StockTrack,@Totalisor1,@Totalisor2,@FuelingState,@Volume,@MeterReading,@Consecutive_OOS_Variance,@Notes,@Capacity,@SafeFill,@VolumeUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@MassUnitIndex,@VolumeDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@MassDecimalPlaces,@EquipmentSequence,@LockedOut,@LockedOutReason,@LockedOutDate,@SerialNumber,@CompanyEquipmentID,@TruckCardNumber,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@RatedGPM,@ActualGPM,@FuelAdditiveFlag,@ManufactureDate,@InstallationDate,@InspectionDate,@CalibrationDate,@QCDate,@SecondaryStorageFlag,@ManagedEquipmentFlag,@FuelingType,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@UserData9,@UserData10,@UserData11,@UserData12,@UserData13,@UserData14,@UserData15,@UserData16,@UserData17,@UserData18,@UserData19,@UserData20,@UserData21,@UserData22,@UserData23,@UserData24,@EquipmentGuid,@SiteGuid,@CompanyGuid,@ParentEquipmentGuid,@EquipmentTypeGuid,@FuelCardGuid,@ProductGuid,@AssignedToMeterGuid,@AssetTrackingDeviceGuid,@_MasterRecordGuid,@HiddenDate,@ScullyRequired)
        ;
    
    SET @sync_row_count = @@rowcount;
    
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
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
