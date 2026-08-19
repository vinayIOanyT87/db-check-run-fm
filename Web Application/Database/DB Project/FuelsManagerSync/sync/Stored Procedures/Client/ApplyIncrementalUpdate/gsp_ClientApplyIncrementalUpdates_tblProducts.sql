-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProducts
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblProducts]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblProducts] CT
                        WHERE CT.PK_ProductGuid = @ProductGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblProducts].[ProductID],[dbo].[tblProducts].[Description],[dbo].[tblProducts].[GenericType],[dbo].[tblProducts].[StockResetDate],[dbo].[tblProducts].[StockTrack],[dbo].[tblProducts].[DensityHighLimit],[dbo].[tblProducts].[DensityLowLimit],[dbo].[tblProducts].[DensityDeadband],[dbo].[tblProducts].[TemperatureHiHiLimit],[dbo].[tblProducts].[TemperatureHighLimit],[dbo].[tblProducts].[TemperatureLowLimit],[dbo].[tblProducts].[TemperatureLoLoLimit],[dbo].[tblProducts].[TemperatureDeadband],[dbo].[tblProducts].[Bonded],[dbo].[tblProducts].[LowStockWarning],[dbo].[tblProducts].[GroundFuel],[dbo].[tblProducts].[ProductCode],[dbo].[tblProducts].[Price],[dbo].[tblProducts].[AviationFuelFlag],[dbo].[tblProducts].[StandardDensity],[dbo].[tblProducts].[ApplyVolumeCorrection],[dbo].[tblProducts].[ApplyStandardDensity],[dbo].[tblProducts].[ApplyDensityLimits],[dbo].[tblProducts].[ApplyTemperatureLimits],[dbo].[tblProducts].[VolumeUnitIndex],[dbo].[tblProducts].[TemperatureUnitIndex],[dbo].[tblProducts].[DensityUnitIndex],[dbo].[tblProducts].[VolumeDecimalPlaces],[dbo].[tblProducts].[TemperatureDecimalPlaces],[dbo].[tblProducts].[DensityDecimalPlaces],[dbo].[tblProducts].[Capitalize],[dbo].[tblProducts].[OctaneNumber],[dbo].[tblProducts].[ReidVaporPressure],[dbo].[tblProducts].[HazardousMaterial],[dbo].[tblProducts].[RegulatoryClass],[dbo].[tblProducts].[LoadRackDisplayText],[dbo].[tblProducts].[ComponentTolerance],[dbo].[tblProducts].[VaporRecovery],[dbo].[tblProducts].[LockedOut],[dbo].[tblProducts].[LockedOutReason],[dbo].[tblProducts].[LockedOutDate],[dbo].[tblProducts].[VarianceTolerance],[dbo].[tblProducts].[DielectricTolerance],[dbo].[tblProducts].[LoadByWeight],[dbo].[tblProducts].[PIDXCode],[dbo].[tblProducts].[ContaminationPromptLoadRackText],[dbo].[tblProducts].[InhibitAccounting],[dbo].[tblProducts].[UserData1],[dbo].[tblProducts].[UserData2],[dbo].[tblProducts].[UserData3],[dbo].[tblProducts].[UserData4],[dbo].[tblProducts].[UserData5],[dbo].[tblProducts].[UserData6],[dbo].[tblProducts].[UserData7],[dbo].[tblProducts].[UserData8],[dbo].[tblProducts].[CreatedDate],[dbo].[tblProducts].[CreatedBy],[dbo].[tblProducts].[UpdatedDate],[dbo].[tblProducts].[UpdatedBy],[dbo].[tblProducts].[MassUnitIndex],[dbo].[tblProducts].[LevelUnitIndex],[dbo].[tblProducts].[FlowUnitIndex],[dbo].[tblProducts].[PressureUnitIndex],[dbo].[tblProducts].[MassDecimalPlaces],[dbo].[tblProducts].[LevelDecimalPlaces],[dbo].[tblProducts].[FlowDecimalPlaces],[dbo].[tblProducts].[PressureDecimalPlaces],[dbo].[tblProducts].[VolumePackageSize],[dbo].[tblProducts].[MassPackageSize],[dbo].[tblProducts].[ProductGuid],[dbo].[tblProducts].[SiteGuid],[dbo].[tblProducts].[LookupProductTypeIndex],[dbo].[tblProducts].[TrackingProductGuid],[dbo].[tblProducts].[TaxCode],[dbo].[tblProducts].[VcfModuleSettings],[dbo].[tblProducts].[ProductColor],[dbo].[tblProducts].[PatternColor],[dbo].[tblProducts].[PatternNumber],[dbo].[tblProducts].[_MasterRecordGuid],[dbo].[tblProducts].[HiddenDate],[dbo].[tblProducts].[AutomaticCloseout],[dbo].[tblProducts].[PIDXFamilyCode],[dbo].[tblProducts].[IsEthanol]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblProducts]
                        INNER JOIN [track].[tblProducts] CT
                            ON CT.PK_ProductGuid = [dbo].[tblProducts].[ProductGuid] 
                    WHERE CT.PK_ProductGuid = @ProductGuid
            ) MERGE existingData
            USING (SELECT @ProductID,@Description,@GenericType,@StockResetDate,@StockTrack,@DensityHighLimit,@DensityLowLimit,@DensityDeadband,@TemperatureHiHiLimit,@TemperatureHighLimit,@TemperatureLowLimit,@TemperatureLoLoLimit,@TemperatureDeadband,@Bonded,@LowStockWarning,@GroundFuel,@ProductCode,@Price,@AviationFuelFlag,@StandardDensity,@ApplyVolumeCorrection,@ApplyStandardDensity,@ApplyDensityLimits,@ApplyTemperatureLimits,@VolumeUnitIndex,@TemperatureUnitIndex,@DensityUnitIndex,@VolumeDecimalPlaces,@TemperatureDecimalPlaces,@DensityDecimalPlaces,@Capitalize,@OctaneNumber,@ReidVaporPressure,@HazardousMaterial,@RegulatoryClass,@LoadRackDisplayText,@ComponentTolerance,@VaporRecovery,@LockedOut,@LockedOutReason,@LockedOutDate,@VarianceTolerance,@DielectricTolerance,@LoadByWeight,@PIDXCode,@ContaminationPromptLoadRackText,@InhibitAccounting,@UserData1,@UserData2,@UserData3,@UserData4,@UserData5,@UserData6,@UserData7,@UserData8,@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,@MassUnitIndex,@LevelUnitIndex,@FlowUnitIndex,@PressureUnitIndex,@MassDecimalPlaces,@LevelDecimalPlaces,@FlowDecimalPlaces,@PressureDecimalPlaces,@VolumePackageSize,@MassPackageSize,@ProductGuid,@SiteGuid,@LookupProductTypeIndex,@TrackingProductGuid,@TaxCode,@VcfModuleSettings,@ProductColor,@PatternColor,@PatternNumber,@_MasterRecordGuid,@HiddenDate,@AutomaticCloseout,@PIDXFamilyCode,@IsEthanol
                    ) AS remoteChanges ([ProductID],[Description],[GenericType],[StockResetDate],[StockTrack],[DensityHighLimit],[DensityLowLimit],[DensityDeadband],[TemperatureHiHiLimit],[TemperatureHighLimit],[TemperatureLowLimit],[TemperatureLoLoLimit],[TemperatureDeadband],[Bonded],[LowStockWarning],[GroundFuel],[ProductCode],[Price],[AviationFuelFlag],[StandardDensity],[ApplyVolumeCorrection],[ApplyStandardDensity],[ApplyDensityLimits],[ApplyTemperatureLimits],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[Capitalize],[OctaneNumber],[ReidVaporPressure],[HazardousMaterial],[RegulatoryClass],[LoadRackDisplayText],[ComponentTolerance],[VaporRecovery],[LockedOut],[LockedOutReason],[LockedOutDate],[VarianceTolerance],[DielectricTolerance],[LoadByWeight],[PIDXCode],[ContaminationPromptLoadRackText],[InhibitAccounting],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MassUnitIndex],[LevelUnitIndex],[FlowUnitIndex],[PressureUnitIndex],[MassDecimalPlaces],[LevelDecimalPlaces],[FlowDecimalPlaces],[PressureDecimalPlaces],[VolumePackageSize],[MassPackageSize],[ProductGuid],[SiteGuid],[LookupProductTypeIndex],[TrackingProductGuid],[TaxCode],[VcfModuleSettings],[ProductColor],[PatternColor],[PatternNumber],[_MasterRecordGuid],[HiddenDate],[AutomaticCloseout],[PIDXFamilyCode],[IsEthanol])
            ON (existingData.[ProductGuid] = remoteChanges.[ProductGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
    END
    ELSE
    BEGIN
          SET @sync_row_count = 1
    END
    
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
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
