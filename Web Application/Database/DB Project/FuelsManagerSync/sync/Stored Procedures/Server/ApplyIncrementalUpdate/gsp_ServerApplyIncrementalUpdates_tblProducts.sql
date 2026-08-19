-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblProducts
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalUpdates_tblProducts]
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
@sync_table_name nvarchar(512),
@sync_supported_columns_tblProducts varchar(8000)
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
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- IF THE CLIENT WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
                UPDATE SET [ProductID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductID'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ProductID] ELSE remoteChanges.[ProductID] END
                       ,[Description] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[Description] ELSE remoteChanges.[Description] END
                       ,[GenericType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GenericType'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[GenericType] ELSE remoteChanges.[GenericType] END
                       ,[StockResetDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockResetDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[StockResetDate] ELSE remoteChanges.[StockResetDate] END
                       ,[StockTrack] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockTrack'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[StockTrack] ELSE remoteChanges.[StockTrack] END
                       ,[DensityHighLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityHighLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[DensityHighLimit] ELSE remoteChanges.[DensityHighLimit] END
                       ,[DensityLowLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityLowLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[DensityLowLimit] ELSE remoteChanges.[DensityLowLimit] END
                       ,[DensityDeadband] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDeadband'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[DensityDeadband] ELSE remoteChanges.[DensityDeadband] END
                       ,[TemperatureHiHiLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureHiHiLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureHiHiLimit] ELSE remoteChanges.[TemperatureHiHiLimit] END
                       ,[TemperatureHighLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureHighLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureHighLimit] ELSE remoteChanges.[TemperatureHighLimit] END
                       ,[TemperatureLowLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureLowLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureLowLimit] ELSE remoteChanges.[TemperatureLowLimit] END
                       ,[TemperatureLoLoLimit] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureLoLoLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureLoLoLimit] ELSE remoteChanges.[TemperatureLoLoLimit] END
                       ,[TemperatureDeadband] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDeadband'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureDeadband] ELSE remoteChanges.[TemperatureDeadband] END
                       ,[Bonded] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Bonded'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[Bonded] ELSE remoteChanges.[Bonded] END
                       ,[LowStockWarning] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowStockWarning'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LowStockWarning] ELSE remoteChanges.[LowStockWarning] END
                       ,[GroundFuel] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GroundFuel'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[GroundFuel] ELSE remoteChanges.[GroundFuel] END
                       ,[ProductCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ProductCode] ELSE remoteChanges.[ProductCode] END
                       ,[Price] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Price'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[Price] ELSE remoteChanges.[Price] END
                       ,[AviationFuelFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AviationFuelFlag'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[AviationFuelFlag] ELSE remoteChanges.[AviationFuelFlag] END
                       ,[StandardDensity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StandardDensity'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[StandardDensity] ELSE remoteChanges.[StandardDensity] END
                       ,[ApplyVolumeCorrection] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyVolumeCorrection'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ApplyVolumeCorrection] ELSE remoteChanges.[ApplyVolumeCorrection] END
                       ,[ApplyStandardDensity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyStandardDensity'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ApplyStandardDensity] ELSE remoteChanges.[ApplyStandardDensity] END
                       ,[ApplyDensityLimits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyDensityLimits'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ApplyDensityLimits] ELSE remoteChanges.[ApplyDensityLimits] END
                       ,[ApplyTemperatureLimits] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyTemperatureLimits'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ApplyTemperatureLimits] ELSE remoteChanges.[ApplyTemperatureLimits] END
                       ,[VolumeUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[VolumeUnitIndex] ELSE remoteChanges.[VolumeUnitIndex] END
                       ,[TemperatureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureUnitIndex] ELSE remoteChanges.[TemperatureUnitIndex] END
                       ,[DensityUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[DensityUnitIndex] ELSE remoteChanges.[DensityUnitIndex] END
                       ,[VolumeDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[VolumeDecimalPlaces] ELSE remoteChanges.[VolumeDecimalPlaces] END
                       ,[TemperatureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TemperatureDecimalPlaces] ELSE remoteChanges.[TemperatureDecimalPlaces] END
                       ,[DensityDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[DensityDecimalPlaces] ELSE remoteChanges.[DensityDecimalPlaces] END
                       ,[Capitalize] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Capitalize'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[Capitalize] ELSE remoteChanges.[Capitalize] END
                       ,[OctaneNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OctaneNumber'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[OctaneNumber] ELSE remoteChanges.[OctaneNumber] END
                       ,[ReidVaporPressure] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReidVaporPressure'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ReidVaporPressure] ELSE remoteChanges.[ReidVaporPressure] END
                       ,[HazardousMaterial] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HazardousMaterial'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[HazardousMaterial] ELSE remoteChanges.[HazardousMaterial] END
                       ,[RegulatoryClass] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RegulatoryClass'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[RegulatoryClass] ELSE remoteChanges.[RegulatoryClass] END
                       ,[LoadRackDisplayText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadRackDisplayText'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LoadRackDisplayText] ELSE remoteChanges.[LoadRackDisplayText] END
                       ,[ComponentTolerance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ComponentTolerance'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ComponentTolerance] ELSE remoteChanges.[ComponentTolerance] END
                       ,[VaporRecovery] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VaporRecovery'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[VaporRecovery] ELSE remoteChanges.[VaporRecovery] END
                       ,[LockedOut] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LockedOut] ELSE remoteChanges.[LockedOut] END
                       ,[LockedOutReason] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LockedOutReason] ELSE remoteChanges.[LockedOutReason] END
                       ,[LockedOutDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LockedOutDate] ELSE remoteChanges.[LockedOutDate] END
                       ,[VarianceTolerance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VarianceTolerance'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[VarianceTolerance] ELSE remoteChanges.[VarianceTolerance] END
                       ,[DielectricTolerance] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DielectricTolerance'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[DielectricTolerance] ELSE remoteChanges.[DielectricTolerance] END
                       ,[LoadByWeight] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadByWeight'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LoadByWeight] ELSE remoteChanges.[LoadByWeight] END
                       ,[PIDXCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIDXCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[PIDXCode] ELSE remoteChanges.[PIDXCode] END
                       ,[ContaminationPromptLoadRackText] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ContaminationPromptLoadRackText'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ContaminationPromptLoadRackText] ELSE remoteChanges.[ContaminationPromptLoadRackText] END
                       ,[InhibitAccounting] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitAccounting'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[InhibitAccounting] ELSE remoteChanges.[InhibitAccounting] END
                       ,[UserData1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData1] ELSE remoteChanges.[UserData1] END
                       ,[UserData2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData2] ELSE remoteChanges.[UserData2] END
                       ,[UserData3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData3] ELSE remoteChanges.[UserData3] END
                       ,[UserData4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData4] ELSE remoteChanges.[UserData4] END
                       ,[UserData5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData5] ELSE remoteChanges.[UserData5] END
                       ,[UserData6] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData6] ELSE remoteChanges.[UserData6] END
                       ,[UserData7] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData7] ELSE remoteChanges.[UserData7] END
                       ,[UserData8] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UserData8] ELSE remoteChanges.[UserData8] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[MassUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[MassUnitIndex] ELSE remoteChanges.[MassUnitIndex] END
                       ,[LevelUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LevelUnitIndex] ELSE remoteChanges.[LevelUnitIndex] END
                       ,[FlowUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[FlowUnitIndex] ELSE remoteChanges.[FlowUnitIndex] END
                       ,[PressureUnitIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[PressureUnitIndex] ELSE remoteChanges.[PressureUnitIndex] END
                       ,[MassDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[MassDecimalPlaces] ELSE remoteChanges.[MassDecimalPlaces] END
                       ,[LevelDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LevelDecimalPlaces] ELSE remoteChanges.[LevelDecimalPlaces] END
                       ,[FlowDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[FlowDecimalPlaces] ELSE remoteChanges.[FlowDecimalPlaces] END
                       ,[PressureDecimalPlaces] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[PressureDecimalPlaces] ELSE remoteChanges.[PressureDecimalPlaces] END
                       ,[VolumePackageSize] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumePackageSize'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[VolumePackageSize] ELSE remoteChanges.[VolumePackageSize] END
                       ,[MassPackageSize] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassPackageSize'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[MassPackageSize] ELSE remoteChanges.[MassPackageSize] END
                       ,[SiteGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SiteGuid'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[SiteGuid] ELSE remoteChanges.[SiteGuid] END
                       ,[LookupProductTypeIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupProductTypeIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[LookupProductTypeIndex] ELSE remoteChanges.[LookupProductTypeIndex] END
                       ,[TrackingProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TrackingProductGuid'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TrackingProductGuid] ELSE remoteChanges.[TrackingProductGuid] END
                       ,[TaxCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaxCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[TaxCode] ELSE remoteChanges.[TaxCode] END
                       ,[VcfModuleSettings] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VcfModuleSettings'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[VcfModuleSettings] ELSE remoteChanges.[VcfModuleSettings] END
                       ,[ProductColor] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductColor'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[ProductColor] ELSE remoteChanges.[ProductColor] END
                       ,[PatternColor] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PatternColor'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[PatternColor] ELSE remoteChanges.[PatternColor] END
                       ,[PatternNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PatternNumber'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[PatternNumber] ELSE remoteChanges.[PatternNumber] END
                       ,[_MasterRecordGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('_MasterRecordGuid'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[_MasterRecordGuid] ELSE remoteChanges.[_MasterRecordGuid] END
                       ,[HiddenDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[HiddenDate] ELSE remoteChanges.[HiddenDate] END
                       ,[AutomaticCloseout] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AutomaticCloseout'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[AutomaticCloseout] ELSE remoteChanges.[AutomaticCloseout] END
                       ,[PIDXFamilyCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIDXFamilyCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[PIDXFamilyCode] ELSE remoteChanges.[PIDXFamilyCode] END
                       ,[IsEthanol] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('IsEthanol'), @sync_supported_columns_tblProducts)) WHEN 0 THEN existingData.[IsEthanol] ELSE remoteChanges.[IsEthanol] END

            WHEN NOT MATCHED THEN
                INSERT ([ProductID],[Description],[GenericType],[StockResetDate],[StockTrack],[DensityHighLimit],[DensityLowLimit],[DensityDeadband],[TemperatureHiHiLimit],[TemperatureHighLimit],[TemperatureLowLimit],[TemperatureLoLoLimit],[TemperatureDeadband],[Bonded],[LowStockWarning],[GroundFuel],[ProductCode],[Price],[AviationFuelFlag],[StandardDensity],[ApplyVolumeCorrection],[ApplyStandardDensity],[ApplyDensityLimits],[ApplyTemperatureLimits],[VolumeUnitIndex],[TemperatureUnitIndex],[DensityUnitIndex],[VolumeDecimalPlaces],[TemperatureDecimalPlaces],[DensityDecimalPlaces],[Capitalize],[OctaneNumber],[ReidVaporPressure],[HazardousMaterial],[RegulatoryClass],[LoadRackDisplayText],[ComponentTolerance],[VaporRecovery],[LockedOut],[LockedOutReason],[LockedOutDate],[VarianceTolerance],[DielectricTolerance],[LoadByWeight],[PIDXCode],[ContaminationPromptLoadRackText],[InhibitAccounting],[UserData1],[UserData2],[UserData3],[UserData4],[UserData5],[UserData6],[UserData7],[UserData8],[CreatedDate],[CreatedBy],[UpdatedDate],[UpdatedBy],[MassUnitIndex],[LevelUnitIndex],[FlowUnitIndex],[PressureUnitIndex],[MassDecimalPlaces],[LevelDecimalPlaces],[FlowDecimalPlaces],[PressureDecimalPlaces],[VolumePackageSize],[MassPackageSize],[ProductGuid],[SiteGuid],[LookupProductTypeIndex],[TrackingProductGuid],[TaxCode],[VcfModuleSettings],[ProductColor],[PatternColor],[PatternNumber],[_MasterRecordGuid],[HiddenDate],[AutomaticCloseout],[PIDXFamilyCode],[IsEthanol])
                    VALUES (@ProductID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Description'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @Description END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GenericType'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @GenericType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockResetDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @StockResetDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StockTrack'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @StockTrack END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityHighLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @DensityHighLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityLowLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @DensityLowLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDeadband'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @DensityDeadband END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureHiHiLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureHiHiLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureHighLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureHighLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureLowLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureLowLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureLoLoLimit'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureLoLoLimit END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDeadband'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureDeadband END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Bonded'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @Bonded END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LowStockWarning'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LowStockWarning END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GroundFuel'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @GroundFuel END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ProductCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Price'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @Price END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('AviationFuelFlag'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @AviationFuelFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StandardDensity'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @StandardDensity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyVolumeCorrection'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ApplyVolumeCorrection END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyStandardDensity'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ApplyStandardDensity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyDensityLimits'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ApplyDensityLimits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ApplyTemperatureLimits'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ApplyTemperatureLimits END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @VolumeUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @DensityUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumeDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @VolumeDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TemperatureDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TemperatureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DensityDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @DensityDecimalPlaces END),@Capitalize,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('OctaneNumber'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @OctaneNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ReidVaporPressure'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ReidVaporPressure END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HazardousMaterial'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @HazardousMaterial END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('RegulatoryClass'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @RegulatoryClass END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadRackDisplayText'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LoadRackDisplayText END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ComponentTolerance'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ComponentTolerance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VaporRecovery'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @VaporRecovery END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOut'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LockedOut END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutReason'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LockedOutReason END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LockedOutDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LockedOutDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VarianceTolerance'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @VarianceTolerance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DielectricTolerance'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @DielectricTolerance END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LoadByWeight'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LoadByWeight END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIDXCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @PIDXCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ContaminationPromptLoadRackText'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ContaminationPromptLoadRackText END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('InhibitAccounting'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @InhibitAccounting END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData1'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData2'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData3'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData4'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData5'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData6'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData6 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData7'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData7 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UserData8'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @UserData8 END),@CreatedDate,@CreatedBy,@UpdatedDate,@UpdatedBy,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @MassUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LevelUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @FlowUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureUnitIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @PressureUnitIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @MassDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LevelDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LevelDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FlowDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @FlowDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PressureDecimalPlaces'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @PressureDecimalPlaces END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VolumePackageSize'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @VolumePackageSize END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassPackageSize'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @MassPackageSize END),@ProductGuid,@SiteGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupProductTypeIndex'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @LookupProductTypeIndex END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TrackingProductGuid'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TrackingProductGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TaxCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @TaxCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VcfModuleSettings'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @VcfModuleSettings END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductColor'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @ProductColor END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PatternColor'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @PatternColor END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PatternNumber'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @PatternNumber END),@_MasterRecordGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('HiddenDate'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @HiddenDate END),@AutomaticCloseout,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PIDXFamilyCode'), @sync_supported_columns_tblProducts)) WHEN 0 THEN NULL ELSE @PIDXFamilyCode END),@IsEthanol)
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
        RAISERROR(N'(SU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
