-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProducts
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblProducts]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@ProductID nvarchar(30),
@Description nvarchar(50),
@GenericType nvarchar(10),
@StockResetDate datetimeoffset(7),
@StockTrack bit,
@DensityHighLimit float,
@DensityLowLimit float,
@DensityDeadband float,
@TemperatureHiHiLimit float,
@TemperatureHighLimit float,
@TemperatureLowLimit float,
@TemperatureLoLoLimit float,
@TemperatureDeadband float,
@Bonded bit,
@LowStockWarning float,
@GroundFuel bit,
@ProductCode nvarchar(15),
@Price money,
@AviationFuelFlag bit,
@StandardDensity float,
@ApplyVolumeCorrection bit,
@ApplyStandardDensity bit,
@ApplyDensityLimits bit,
@ApplyTemperatureLimits bit,
@VolumeUnitIndex int,
@TemperatureUnitIndex int,
@DensityUnitIndex int,
@VolumeDecimalPlaces tinyint,
@TemperatureDecimalPlaces tinyint,
@DensityDecimalPlaces tinyint,
@Capitalize bit,
@OctaneNumber float,
@ReidVaporPressure float,
@HazardousMaterial bit,
@RegulatoryClass int,
@LoadRackDisplayText nvarchar(10),
@ComponentTolerance float,
@VaporRecovery bit,
@LockedOut bit,
@LockedOutReason nvarchar(80),
@LockedOutDate datetimeoffset(7),
@VarianceTolerance float,
@DielectricTolerance float,
@LoadByWeight bit,
@PIDXCode nvarchar(4),
@ContaminationPromptLoadRackText nvarchar(10),
@InhibitAccounting bit,
@UserData1 nvarchar(60),
@UserData2 nvarchar(60),
@UserData3 nvarchar(60),
@UserData4 nvarchar(60),
@UserData5 nvarchar(60),
@UserData6 nvarchar(60),
@UserData7 nvarchar(60),
@UserData8 nvarchar(60),
@CreatedDate datetimeoffset(7),
@CreatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@MassUnitIndex int,
@LevelUnitIndex int,
@FlowUnitIndex int,
@PressureUnitIndex int,
@MassDecimalPlaces tinyint,
@LevelDecimalPlaces tinyint,
@FlowDecimalPlaces tinyint,
@PressureDecimalPlaces tinyint,
@VolumePackageSize float,
@MassPackageSize float,
@ProductGuid uniqueidentifier,
@SiteGuid uniqueidentifier,
@LookupProductTypeIndex int,
@TrackingProductGuid uniqueidentifier,
@TaxCode nvarchar(10),
@VcfModuleSettings xml,
@ProductColor nvarchar(7),
@PatternColor nvarchar(7),
@PatternNumber int,
@_MasterRecordGuid uniqueidentifier,
@HiddenDate datetimeoffset(7),
@AutomaticCloseout bit,
@PIDXFamilyCode nvarchar(4),
@IsEthanol bit,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    ;   MERGE [dbo].[tblProducts] AS existingData
        USING (SELECT @ProductID 'ProductID',@Description 'Description',@GenericType 'GenericType',@StockResetDate 'StockResetDate',@StockTrack 'StockTrack',@DensityHighLimit 'DensityHighLimit',@DensityLowLimit 'DensityLowLimit',@DensityDeadband 'DensityDeadband',@TemperatureHiHiLimit 'TemperatureHiHiLimit',@TemperatureHighLimit 'TemperatureHighLimit',@TemperatureLowLimit 'TemperatureLowLimit',@TemperatureLoLoLimit 'TemperatureLoLoLimit',@TemperatureDeadband 'TemperatureDeadband',@Bonded 'Bonded',@LowStockWarning 'LowStockWarning',@GroundFuel 'GroundFuel',@ProductCode 'ProductCode',@Price 'Price',@AviationFuelFlag 'AviationFuelFlag',@StandardDensity 'StandardDensity',@ApplyVolumeCorrection 'ApplyVolumeCorrection',@ApplyStandardDensity 'ApplyStandardDensity',@ApplyDensityLimits 'ApplyDensityLimits',@ApplyTemperatureLimits 'ApplyTemperatureLimits',@VolumeUnitIndex 'VolumeUnitIndex',@TemperatureUnitIndex 'TemperatureUnitIndex',@DensityUnitIndex 'DensityUnitIndex',@VolumeDecimalPlaces 'VolumeDecimalPlaces',@TemperatureDecimalPlaces 'TemperatureDecimalPlaces',@DensityDecimalPlaces 'DensityDecimalPlaces',@Capitalize 'Capitalize',@OctaneNumber 'OctaneNumber',@ReidVaporPressure 'ReidVaporPressure',@HazardousMaterial 'HazardousMaterial',@RegulatoryClass 'RegulatoryClass',@LoadRackDisplayText 'LoadRackDisplayText',@ComponentTolerance 'ComponentTolerance',@VaporRecovery 'VaporRecovery',@LockedOut 'LockedOut',@LockedOutReason 'LockedOutReason',@LockedOutDate 'LockedOutDate',@VarianceTolerance 'VarianceTolerance',@DielectricTolerance 'DielectricTolerance',@LoadByWeight 'LoadByWeight',@PIDXCode 'PIDXCode',@ContaminationPromptLoadRackText 'ContaminationPromptLoadRackText',@InhibitAccounting 'InhibitAccounting',@UserData1 'UserData1',@UserData2 'UserData2',@UserData3 'UserData3',@UserData4 'UserData4',@UserData5 'UserData5',@UserData6 'UserData6',@UserData7 'UserData7',@UserData8 'UserData8',@CreatedDate 'CreatedDate',@CreatedBy 'CreatedBy',@UpdatedDate 'UpdatedDate',@UpdatedBy 'UpdatedBy',@MassUnitIndex 'MassUnitIndex',@LevelUnitIndex 'LevelUnitIndex',@FlowUnitIndex 'FlowUnitIndex',@PressureUnitIndex 'PressureUnitIndex',@MassDecimalPlaces 'MassDecimalPlaces',@LevelDecimalPlaces 'LevelDecimalPlaces',@FlowDecimalPlaces 'FlowDecimalPlaces',@PressureDecimalPlaces 'PressureDecimalPlaces',@VolumePackageSize 'VolumePackageSize',@MassPackageSize 'MassPackageSize',@ProductGuid 'ProductGuid',@SiteGuid 'SiteGuid',@LookupProductTypeIndex 'LookupProductTypeIndex',@TrackingProductGuid 'TrackingProductGuid',@TaxCode 'TaxCode',@VcfModuleSettings 'VcfModuleSettings',@ProductColor 'ProductColor',@PatternColor 'PatternColor',@PatternNumber 'PatternNumber',@_MasterRecordGuid '_MasterRecordGuid',@HiddenDate 'HiddenDate',@AutomaticCloseout 'AutomaticCloseout',@PIDXFamilyCode 'PIDXFamilyCode',@IsEthanol 'IsEthanol'
                ) AS remoteChanges ([ProductID],[Description],[GenericType],[StockResetDate],[StockTrack],[DensityHighLimit],[DensityLowLimit],[DensityDeadband],[TemperatureHiHiLimit],[TemperatureHighLimit],[TemperatureLowLimit],[TemperatureLoLoLimit],[TemperatureDeadband],[Bonded],[LowStockWarning],[GroundFuel],[ProductCode],[Price],[AviationFuelFlag],[StandardDensity],[ApplyVolumeCorrection],[ApplyStandardDensity],[ApplyDensityLimits],[ApplyTemperatureLimits],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[Capitalize],[OctaneNumber],[ReidVaporPressure],[HazardousMaterial],[RegulatoryClass],[LoadRackDisplayText],[ComponentTolerance],[VaporRecovery],[LockedOut],[LockedOutReason],[LockedOutDate],[VarianceTolerance],[DielectricTolerance],[LoadByWeight],[PIDXCode],[ContaminationPromptLoadRackText],[InhibitAccounting],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MassUnitIndex],[LevelUnitIndex],[FlowUnitIndex],[PressureUnitIndex],[MassDecimalPlaces],[LevelDecimalPlaces],[FlowDecimalPlaces],[PressureDecimalPlaces],[VolumePackageSize],[MassPackageSize],[ProductGuid],[SiteGuid],[LookupProductTypeIndex],[TrackingProductGuid],[TaxCode],[VcfModuleSettings],[ProductColor],[PatternColor],[PatternNumber],[_MasterRecordGuid],[HiddenDate],[AutomaticCloseout],[PIDXFamilyCode],[IsEthanol])
        ON (existingData.[ProductGuid] = remoteChanges.[ProductGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [ProductID] = remoteChanges.[ProductID]
                       ,[Description] = remoteChanges.[Description]
                       ,[GenericType] = remoteChanges.[GenericType]
                       ,[StockResetDate] = remoteChanges.[StockResetDate]
                       ,[StockTrack] = remoteChanges.[StockTrack]
                       ,[DensityHighLimit] = remoteChanges.[DensityHighLimit]
                       ,[DensityLowLimit] = remoteChanges.[DensityLowLimit]
                       ,[DensityDeadband] = remoteChanges.[DensityDeadband]
                       ,[TemperatureHiHiLimit] = remoteChanges.[TemperatureHiHiLimit]
                       ,[TemperatureHighLimit] = remoteChanges.[TemperatureHighLimit]
                       ,[TemperatureLowLimit] = remoteChanges.[TemperatureLowLimit]
                       ,[TemperatureLoLoLimit] = remoteChanges.[TemperatureLoLoLimit]
                       ,[TemperatureDeadband] = remoteChanges.[TemperatureDeadband]
                       ,[Bonded] = remoteChanges.[Bonded]
                       ,[LowStockWarning] = remoteChanges.[LowStockWarning]
                       ,[GroundFuel] = remoteChanges.[GroundFuel]
                       ,[ProductCode] = remoteChanges.[ProductCode]
                       ,[Price] = remoteChanges.[Price]
                       ,[AviationFuelFlag] = remoteChanges.[AviationFuelFlag]
                       ,[StandardDensity] = remoteChanges.[StandardDensity]
                       ,[ApplyVolumeCorrection] = remoteChanges.[ApplyVolumeCorrection]
                       ,[ApplyStandardDensity] = remoteChanges.[ApplyStandardDensity]
                       ,[ApplyDensityLimits] = remoteChanges.[ApplyDensityLimits]
                       ,[ApplyTemperatureLimits] = remoteChanges.[ApplyTemperatureLimits]
                       ,[VolumeUnitIndex] = remoteChanges.[VolumeUnitIndex]
                       ,[TemperatureUnitIndex] = remoteChanges.[TemperatureUnitIndex]
                       ,[DensityUnitIndex] = remoteChanges.[DensityUnitIndex]
                       ,[VolumeDecimalPlaces] = remoteChanges.[VolumeDecimalPlaces]
                       ,[TemperatureDecimalPlaces] = remoteChanges.[TemperatureDecimalPlaces]
                       ,[DensityDecimalPlaces] = remoteChanges.[DensityDecimalPlaces]
                       ,[Capitalize] = remoteChanges.[Capitalize]
                       ,[OctaneNumber] = remoteChanges.[OctaneNumber]
                       ,[ReidVaporPressure] = remoteChanges.[ReidVaporPressure]
                       ,[HazardousMaterial] = remoteChanges.[HazardousMaterial]
                       ,[RegulatoryClass] = remoteChanges.[RegulatoryClass]
                       ,[LoadRackDisplayText] = remoteChanges.[LoadRackDisplayText]
                       ,[ComponentTolerance] = remoteChanges.[ComponentTolerance]
                       ,[VaporRecovery] = remoteChanges.[VaporRecovery]
                       ,[LockedOut] = remoteChanges.[LockedOut]
                       ,[LockedOutReason] = remoteChanges.[LockedOutReason]
                       ,[LockedOutDate] = remoteChanges.[LockedOutDate]
                       ,[VarianceTolerance] = remoteChanges.[VarianceTolerance]
                       ,[DielectricTolerance] = remoteChanges.[DielectricTolerance]
                       ,[LoadByWeight] = remoteChanges.[LoadByWeight]
                       ,[PIDXCode] = remoteChanges.[PIDXCode]
                       ,[ContaminationPromptLoadRackText] = remoteChanges.[ContaminationPromptLoadRackText]
                       ,[InhibitAccounting] = remoteChanges.[InhibitAccounting]
                       ,[UserData1] = remoteChanges.[UserData1]
                       ,[UserData2] = remoteChanges.[UserData2]
                       ,[UserData3] = remoteChanges.[UserData3]
                       ,[UserData4] = remoteChanges.[UserData4]
                       ,[UserData5] = remoteChanges.[UserData5]
                       ,[UserData6] = remoteChanges.[UserData6]
                       ,[UserData7] = remoteChanges.[UserData7]
                       ,[UserData8] = remoteChanges.[UserData8]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[MassUnitIndex] = remoteChanges.[MassUnitIndex]
                       ,[LevelUnitIndex] = remoteChanges.[LevelUnitIndex]
                       ,[FlowUnitIndex] = remoteChanges.[FlowUnitIndex]
                       ,[PressureUnitIndex] = remoteChanges.[PressureUnitIndex]
                       ,[MassDecimalPlaces] = remoteChanges.[MassDecimalPlaces]
                       ,[LevelDecimalPlaces] = remoteChanges.[LevelDecimalPlaces]
                       ,[FlowDecimalPlaces] = remoteChanges.[FlowDecimalPlaces]
                       ,[PressureDecimalPlaces] = remoteChanges.[PressureDecimalPlaces]
                       ,[VolumePackageSize] = remoteChanges.[VolumePackageSize]
                       ,[MassPackageSize] = remoteChanges.[MassPackageSize]
                       ,[SiteGuid] = remoteChanges.[SiteGuid]
                       ,[LookupProductTypeIndex] = remoteChanges.[LookupProductTypeIndex]
                       ,[TrackingProductGuid] = remoteChanges.[TrackingProductGuid]
                       ,[TaxCode] = remoteChanges.[TaxCode]
                       ,[VcfModuleSettings] = remoteChanges.[VcfModuleSettings]
                       ,[ProductColor] = remoteChanges.[ProductColor]
                       ,[PatternColor] = remoteChanges.[PatternColor]
                       ,[PatternNumber] = remoteChanges.[PatternNumber]
                       ,[_MasterRecordGuid] = remoteChanges.[_MasterRecordGuid]
                       ,[HiddenDate] = remoteChanges.[HiddenDate]
                       ,[AutomaticCloseout] = remoteChanges.[AutomaticCloseout]
                       ,[PIDXFamilyCode] = remoteChanges.[PIDXFamilyCode]
                       ,[IsEthanol] = remoteChanges.[IsEthanol]

        WHEN NOT MATCHED THEN
            INSERT ([ProductID],[Description],[GenericType],[StockResetDate],[StockTrack],[DensityHighLimit],[DensityLowLimit],[DensityDeadband],[TemperatureHiHiLimit],[TemperatureHighLimit],[TemperatureLowLimit],[TemperatureLoLoLimit],[TemperatureDeadband],[Bonded],[LowStockWarning],[GroundFuel],[ProductCode],[Price],[AviationFuelFlag],[StandardDensity],[ApplyVolumeCorrection],[ApplyStandardDensity],[ApplyDensityLimits],[ApplyTemperatureLimits],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[Capitalize],[OctaneNumber],[ReidVaporPressure],[HazardousMaterial],[RegulatoryClass],[LoadRackDisplayText],[ComponentTolerance],[VaporRecovery],[LockedOut],[LockedOutReason],[LockedOutDate],[VarianceTolerance],[DielectricTolerance],[LoadByWeight],[PIDXCode],[ContaminationPromptLoadRackText],[InhibitAccounting],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MassUnitIndex],[LevelUnitIndex],[FlowUnitIndex],[PressureUnitIndex],[MassDecimalPlaces],[LevelDecimalPlaces],[FlowDecimalPlaces],[PressureDecimalPlaces],[VolumePackageSize],[MassPackageSize],[ProductGuid],[SiteGuid],[LookupProductTypeIndex],[TrackingProductGuid],[TaxCode],[VcfModuleSettings],[ProductColor],[PatternColor],[PatternNumber],[_MasterRecordGuid],[HiddenDate],[AutomaticCloseout],[PIDXFamilyCode],[IsEthanol])
                VALUES (@ProductID,@Description,@GenericType,@StockResetDate,@StockTrack,@DensityHighLimit,@DensityLowLimit,@DensityDeadband,@TemperatureHiHiLimit,@TemperatureHighLimit,@TemperatureLowLimit,@TemperatureLoLoLimit,@TemperatureDeadband,@Bonded,@LowStockWarning,@GroundFuel,@ProductCode,@Price,@AviationFuelFlag,@StandardDensity,@ApplyVolumeCorrection,@ApplyStandardDensity,@ApplyDensityLimits,@ApplyTemperatureLimits,@VolumeUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@VolumeDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@Capitalize,@OctaneNumber,@ReidVaporPressure,@HazardousMaterial,@RegulatoryClass,@LoadRackDisplayText,@ComponentTolerance,@VaporRecovery,@LockedOut,@LockedOutReason,@LockedOutDate,@VarianceTolerance,@DielectricTolerance,@LoadByWeight,@PIDXCode,@ContaminationPromptLoadRackText,@InhibitAccounting,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@MassUnitIndex,@LevelUnitIndex,@FlowUnitIndex,@PressureUnitIndex,@MassDecimalPlaces,@LevelDecimalPlaces,@FlowDecimalPlaces,@PressureDecimalPlaces,@VolumePackageSize,@MassPackageSize,@ProductGuid,@SiteGuid,@LookupProductTypeIndex,@TrackingProductGuid,@TaxCode,@VcfModuleSettings,@ProductColor,@PatternColor,@PatternNumber,@_MasterRecordGuid,@HiddenDate,@AutomaticCloseout,@PIDXFamilyCode,@IsEthanol)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @ProductGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblProducts] WHERE ProductGuid = @ProductGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
