-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionSubLineItems
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ServerApplyIncrementalInserts_tblTransactionSubLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@SequenceID int,
@Product nvarchar(30),
@ProductCode nvarchar(50),
@ProductType nvarchar(20),
@GrossQuantity float,
@DeliveredGrossQuantity float,
@NetQuantity float,
@DeliveredNetQuantity float,
@Pressure float,
@Vcf float,
@Density float,
@Temperature float,
@Customs nvarchar(20),
@ArmNumber int,
@LineNumber int,
@BatchNumber nvarchar(20),
@LineFill float,
@BottomVolume float,
@NetCapacity float,
@TankStatus nvarchar(30),
@MeterFactor float,
@MeterStart float,
@MeterStop float,
@MeterStopDateTime datetimeoffset(7),
@MeterStartDateTime datetimeoffset(7),
@FreezePoint float,
@DifferentialPressure float,
@DosageRate float,
@DeleteFlag bit,
@PresetAmount float,
@StorageLocationID nvarchar(50),
@MeterID nvarchar(50),
@COAID nvarchar(40),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@TransactionInventoryDate date,
@Tax1 float,
@Tax2 float,
@Tax3 float,
@Tax4 float,
@Tax5 float,
@TransVersion bigint,
@ImproperAdditization bit,
@BrokenBlend bit,
@Flag01 bit,
@Flag02 bit,
@Flag03 bit,
@Flag04 bit,
@Flag05 bit,
@Flag06 bit,
@Number01 float,
@Number02 float,
@Number03 float,
@Number04 float,
@Number05 float,
@Number06 float,
@Date01 datetimeoffset(7),
@Date02 datetimeoffset(7),
@Date03 datetimeoffset(7),
@Date04 datetimeoffset(7),
@MassQuantity float,
@NetManualValueFlag bit,
@MassManualValueFlag bit,
@GrossManualValueFlag bit,
@VcfManualValueFlag bit,
@DeliveredGrossManualValueFlag bit,
@DeliveredNetManualValueFlag bit,
@TransactionSubLineItemGuid uniqueidentifier,
@LookupTransactionStatusIndex int,
@LookupQualityIndex int,
@TransactionLineItemGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@TransactionGuid uniqueidentifier,
@StorageLocationTankGuid uniqueidentifier,
@MeterGuid uniqueidentifier,
@PackageManualValueFlag bit,
@CleanLineItem bit,
@CleanLineDeductItem bit,
@CleanLineDeductQuantity float,
@CleanLinePackQuantity float,
@sync_row_count int out,
@sync_table_name nvarchar(512),
@sync_supported_columns_tblTransactionSubLineItems varchar(8000)
AS
BEGIN
    DECLARE @minValidVersion BigInt 

    ;   MERGE [dbo].[tblTransactionSubLineItems] AS existingData
        USING (SELECT @SequenceID 'SequenceID',@Product 'Product',@ProductCode 'ProductCode',@ProductType 'ProductType',@GrossQuantity 'GrossQuantity',@DeliveredGrossQuantity 'DeliveredGrossQuantity',@NetQuantity 'NetQuantity',@DeliveredNetQuantity 'DeliveredNetQuantity',@Pressure 'Pressure',@Vcf 'Vcf',@Density 'Density',@Temperature 'Temperature',@Customs 'Customs',@ArmNumber 'ArmNumber',@LineNumber 'LineNumber',@BatchNumber 'BatchNumber',@LineFill 'LineFill',@BottomVolume 'BottomVolume',@NetCapacity 'NetCapacity',@TankStatus 'TankStatus',@MeterFactor 'MeterFactor',@MeterStart 'MeterStart',@MeterStop 'MeterStop',@MeterStopDateTime 'MeterStopDateTime',@MeterStartDateTime 'MeterStartDateTime',@FreezePoint 'FreezePoint',@DifferentialPressure 'DifferentialPressure',@DosageRate 'DosageRate',@DeleteFlag 'DeleteFlag',@PresetAmount 'PresetAmount',@StorageLocationID 'StorageLocationID',@MeterID 'MeterID',@COAID 'COAID',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@TransactionInventoryDate 'TransactionInventoryDate',@Tax1 'Tax1',@Tax2 'Tax2',@Tax3 'Tax3',@Tax4 'Tax4',@Tax5 'Tax5',@TransVersion 'TransVersion',@ImproperAdditization 'ImproperAdditization',@BrokenBlend 'BrokenBlend',@Flag01 'Flag01',@Flag02 'Flag02',@Flag03 'Flag03',@Flag04 'Flag04',@Flag05 'Flag05',@Flag06 'Flag06',@Number01 'Number01',@Number02 'Number02',@Number03 'Number03',@Number04 'Number04',@Number05 'Number05',@Number06 'Number06',@Date01 'Date01',@Date02 'Date02',@Date03 'Date03',@Date04 'Date04',@MassQuantity 'MassQuantity',@NetManualValueFlag 'NetManualValueFlag',@MassManualValueFlag 'MassManualValueFlag',@GrossManualValueFlag 'GrossManualValueFlag',@VcfManualValueFlag 'VcfManualValueFlag',@DeliveredGrossManualValueFlag 'DeliveredGrossManualValueFlag',@DeliveredNetManualValueFlag 'DeliveredNetManualValueFlag' ,@TransactionSubLineItemGuid 'TransactionSubLineItemGuid',@LookupTransactionStatusIndex 'LookupTransactionStatusIndex',@LookupQualityIndex 'LookupQualityIndex',@TransactionLineItemGuid 'TransactionLineItemGuid',@ProductGuid 'ProductGuid',@TransactionGuid 'TransactionGuid',@StorageLocationTankGuid 'StorageLocationTankGuid',@MeterGuid 'MeterGuid',@PackageManualValueFlag 'PackageManualValueFlag',@CleanLineItem 'CleanLineItem',@CleanLineDeductItem 'CleanLineDeductItem',@CleanLineDeductQuantity 'CleanLineDeductQuantity',@CleanLinePackQuantity 'CleanLinePackQuantity'
                ) AS remoteChanges ([SequenceID],[Product],[ProductCode],[ProductType],[GrossQuantity],[DeliveredGrossQuantity],[NetQuantity],[DeliveredNetQuantity],[Pressure],[Vcf],[Density],[Temperature],[Customs],[ArmNumber],[LineNumber],[BatchNumber],[LineFill],[BottomVolume],[NetCapacity],[TankStatus],[MeterFactor],[MeterStart],[MeterStop],[MeterStopDateTime],[MeterStartDateTime],[FreezePoint],[DifferentialPressure],[DosageRate],[DeleteFlag],[PresetAmount],[StorageLocationID],[MeterID],[COAID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionInventoryDate],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[ImproperAdditization],[BrokenBlend],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[Date01],[Date02],[Date03],[Date04],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag], [TransactionSubLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[TransactionLineItemGuid],[ProductGuid],[TransactionGuid],[StorageLocationTankGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity])
        ON (existingData.[TransactionSubLineItemGuid] = remoteChanges.[TransactionSubLineItemGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SequenceID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('SequenceID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[SequenceID] ELSE remoteChanges.[SequenceID] END
                       ,[Product] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Product'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Product] ELSE remoteChanges.[Product] END
                       ,[ProductCode] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductCode'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[ProductCode] ELSE remoteChanges.[ProductCode] END
                       ,[ProductType] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductType'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[ProductType] ELSE remoteChanges.[ProductType] END
                       ,[GrossQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[GrossQuantity] ELSE remoteChanges.[GrossQuantity] END
                       ,[DeliveredGrossQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredGrossQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DeliveredGrossQuantity] ELSE remoteChanges.[DeliveredGrossQuantity] END
                       ,[NetQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[NetQuantity] ELSE remoteChanges.[NetQuantity] END
                       ,[DeliveredNetQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredNetQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DeliveredNetQuantity] ELSE remoteChanges.[DeliveredNetQuantity] END
                       ,[Pressure] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Pressure'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Pressure] ELSE remoteChanges.[Pressure] END
                       ,[Vcf] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Vcf'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Vcf] ELSE remoteChanges.[Vcf] END
                       ,[Density] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Density'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Density] ELSE remoteChanges.[Density] END
                       ,[Temperature] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Temperature'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Temperature] ELSE remoteChanges.[Temperature] END
                       ,[Customs] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Customs'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Customs] ELSE remoteChanges.[Customs] END
                       ,[ArmNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ArmNumber'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[ArmNumber] ELSE remoteChanges.[ArmNumber] END
                       ,[LineNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LineNumber'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[LineNumber] ELSE remoteChanges.[LineNumber] END
                       ,[BatchNumber] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BatchNumber'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[BatchNumber] ELSE remoteChanges.[BatchNumber] END
                       ,[LineFill] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LineFill'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[LineFill] ELSE remoteChanges.[LineFill] END
                       ,[BottomVolume] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BottomVolume'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[BottomVolume] ELSE remoteChanges.[BottomVolume] END
                       ,[NetCapacity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetCapacity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[NetCapacity] ELSE remoteChanges.[NetCapacity] END
                       ,[TankStatus] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankStatus'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[TankStatus] ELSE remoteChanges.[TankStatus] END
                       ,[MeterFactor] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterFactor'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterFactor] ELSE remoteChanges.[MeterFactor] END
                       ,[MeterStart] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStart'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterStart] ELSE remoteChanges.[MeterStart] END
                       ,[MeterStop] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStop'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterStop] ELSE remoteChanges.[MeterStop] END
                       ,[MeterStopDateTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStopDateTime'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterStopDateTime] ELSE remoteChanges.[MeterStopDateTime] END
                       ,[MeterStartDateTime] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStartDateTime'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterStartDateTime] ELSE remoteChanges.[MeterStartDateTime] END
                       ,[FreezePoint] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FreezePoint'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[FreezePoint] ELSE remoteChanges.[FreezePoint] END
                       ,[DifferentialPressure] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DifferentialPressure'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DifferentialPressure] ELSE remoteChanges.[DifferentialPressure] END
                       ,[DosageRate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DosageRate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DosageRate] ELSE remoteChanges.[DosageRate] END
                       ,[DeleteFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DeleteFlag] ELSE remoteChanges.[DeleteFlag] END
                       ,[PresetAmount] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PresetAmount'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[PresetAmount] ELSE remoteChanges.[PresetAmount] END
                       ,[StorageLocationID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StorageLocationID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[StorageLocationID] ELSE remoteChanges.[StorageLocationID] END
                       ,[MeterID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterID] ELSE remoteChanges.[MeterID] END
                       ,[COAID] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('COAID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[COAID] ELSE remoteChanges.[COAID] END
                       ,[CreatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[CreatedBy] ELSE remoteChanges.[CreatedBy] END
                       ,[CreatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[CreatedDate] ELSE remoteChanges.[CreatedDate] END
                       ,[UpdatedBy] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[UpdatedBy] ELSE remoteChanges.[UpdatedBy] END
                       ,[UpdatedDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[UpdatedDate] ELSE remoteChanges.[UpdatedDate] END
                       ,[TransactionInventoryDate] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionInventoryDate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[TransactionInventoryDate] ELSE remoteChanges.[TransactionInventoryDate] END
                       ,[Tax1] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax1'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Tax1] ELSE remoteChanges.[Tax1] END
                       ,[Tax2] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax2'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Tax2] ELSE remoteChanges.[Tax2] END
                       ,[Tax3] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax3'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Tax3] ELSE remoteChanges.[Tax3] END
                       ,[Tax4] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax4'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Tax4] ELSE remoteChanges.[Tax4] END
                       ,[Tax5] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax5'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Tax5] ELSE remoteChanges.[Tax5] END
                       ,[TransVersion] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[TransVersion] ELSE remoteChanges.[TransVersion] END
                       ,[ImproperAdditization] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ImproperAdditization'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[ImproperAdditization] ELSE remoteChanges.[ImproperAdditization] END
                       ,[BrokenBlend] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BrokenBlend'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[BrokenBlend] ELSE remoteChanges.[BrokenBlend] END
                       ,[Flag01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag01'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Flag01] ELSE remoteChanges.[Flag01] END
                       ,[Flag02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag02'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Flag02] ELSE remoteChanges.[Flag02] END
                       ,[Flag03] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag03'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Flag03] ELSE remoteChanges.[Flag03] END
                       ,[Flag04] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag04'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Flag04] ELSE remoteChanges.[Flag04] END
                       ,[Flag05] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag05'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Flag05] ELSE remoteChanges.[Flag05] END
                       ,[Flag06] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag06'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Flag06] ELSE remoteChanges.[Flag06] END
                       ,[Number01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number01'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Number01] ELSE remoteChanges.[Number01] END
                       ,[Number02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number02'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Number02] ELSE remoteChanges.[Number02] END
                       ,[Number03] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number03'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Number03] ELSE remoteChanges.[Number03] END
                       ,[Number04] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number04'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Number04] ELSE remoteChanges.[Number04] END
                       ,[Number05] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number05'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Number05] ELSE remoteChanges.[Number05] END
                       ,[Number06] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number06'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Number06] ELSE remoteChanges.[Number06] END
                       ,[Date01] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date01'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Date01] ELSE remoteChanges.[Date01] END
                       ,[Date02] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date02'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Date02] ELSE remoteChanges.[Date02] END
                       ,[Date03] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date03'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Date03] ELSE remoteChanges.[Date03] END
                       ,[Date04] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date04'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[Date04] ELSE remoteChanges.[Date04] END
                       ,[MassQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MassQuantity] ELSE remoteChanges.[MassQuantity] END
                       ,[NetManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[NetManualValueFlag] ELSE remoteChanges.[NetManualValueFlag] END
                       ,[MassManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MassManualValueFlag] ELSE remoteChanges.[MassManualValueFlag] END
                       ,[GrossManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[GrossManualValueFlag] ELSE remoteChanges.[GrossManualValueFlag] END
                       ,[VcfManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VcfManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[VcfManualValueFlag] ELSE remoteChanges.[VcfManualValueFlag] END
                       ,[DeliveredGrossManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredGrossManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DeliveredGrossManualValueFlag] ELSE remoteChanges.[DeliveredGrossManualValueFlag] END
                       ,[DeliveredNetManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredNetManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[DeliveredNetManualValueFlag] ELSE remoteChanges.[DeliveredNetManualValueFlag] END
                       ,[LookupTransactionStatusIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupTransactionStatusIndex'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[LookupTransactionStatusIndex] ELSE remoteChanges.[LookupTransactionStatusIndex] END
                       ,[LookupQualityIndex] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupQualityIndex'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[LookupQualityIndex] ELSE remoteChanges.[LookupQualityIndex] END
                       ,[TransactionLineItemGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionLineItemGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[TransactionLineItemGuid] ELSE remoteChanges.[TransactionLineItemGuid] END
                       ,[ProductGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[ProductGuid] ELSE remoteChanges.[ProductGuid] END
                       ,[TransactionGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[TransactionGuid] ELSE remoteChanges.[TransactionGuid] END
                       ,[StorageLocationTankGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StorageLocationTankGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[StorageLocationTankGuid] ELSE remoteChanges.[StorageLocationTankGuid] END
                       ,[MeterGuid] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[MeterGuid] ELSE remoteChanges.[MeterGuid] END
                       ,[PackageManualValueFlag] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PackageManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[PackageManualValueFlag] ELSE remoteChanges.[PackageManualValueFlag] END
                       ,[CleanLineItem] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLineItem'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[CleanLineItem] ELSE remoteChanges.[CleanLineItem] END
                       ,[CleanLineDeductItem] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLineDeductItem'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[CleanLineDeductItem] ELSE remoteChanges.[CleanLineDeductItem] END
                       ,[CleanLineDeductQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLineDeductQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[CleanLineDeductQuantity] ELSE remoteChanges.[CleanLineDeductQuantity] END
                       ,[CleanLinePackQuantity] = CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLinePackQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN existingData.[CleanLinePackQuantity] ELSE remoteChanges.[CleanLinePackQuantity] END

        WHEN NOT MATCHED THEN
            INSERT ([SequenceID],[Product],[ProductCode],[ProductType],[GrossQuantity],[DeliveredGrossQuantity],[NetQuantity],[DeliveredNetQuantity],[Pressure],[Vcf],[Density],[Temperature],[Customs],[ArmNumber],[LineNumber],[BatchNumber],[LineFill],[BottomVolume],[NetCapacity],[TankStatus],[MeterFactor],[MeterStart],[MeterStop],[MeterStopDateTime],[MeterStartDateTime],[FreezePoint],[DifferentialPressure],[DosageRate],[DeleteFlag],[PresetAmount],[StorageLocationID],[MeterID],[COAID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[TransactionInventoryDate],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[ImproperAdditization],[BrokenBlend],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[Date01],[Date02],[Date03],[Date04],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag], [TransactionSubLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[TransactionLineItemGuid],[ProductGuid],[TransactionGuid],[StorageLocationTankGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity])
                VALUES (@SequenceID,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Product'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Product END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductCode'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @ProductCode END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductType'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @ProductType END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @GrossQuantity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredGrossQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DeliveredGrossQuantity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @NetQuantity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredNetQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DeliveredNetQuantity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Pressure'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Pressure END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Vcf'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Vcf END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Density'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Density END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Temperature'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Temperature END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Customs'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Customs END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ArmNumber'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @ArmNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LineNumber'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @LineNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BatchNumber'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @BatchNumber END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LineFill'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @LineFill END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BottomVolume'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @BottomVolume END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetCapacity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @NetCapacity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TankStatus'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @TankStatus END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterFactor'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterFactor END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStart'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterStart END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStop'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterStop END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStopDateTime'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterStopDateTime END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterStartDateTime'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterStartDateTime END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('FreezePoint'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @FreezePoint END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DifferentialPressure'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DifferentialPressure END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DosageRate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DosageRate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeleteFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DeleteFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PresetAmount'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @PresetAmount END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StorageLocationID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @StorageLocationID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('COAID'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @COAID END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedBy'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @CreatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CreatedDate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @CreatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedBy'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @UpdatedBy END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('UpdatedDate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @UpdatedDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransactionInventoryDate'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @TransactionInventoryDate END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax1'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Tax1 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax2'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Tax2 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax3'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Tax3 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax4'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Tax4 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Tax5'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Tax5 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('TransVersion'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @TransVersion END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ImproperAdditization'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @ImproperAdditization END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('BrokenBlend'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @BrokenBlend END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag01'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Flag01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag02'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Flag02 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag03'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Flag03 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag04'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Flag04 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag05'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Flag05 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Flag06'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Flag06 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number01'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Number01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number02'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Number02 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number03'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Number03 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number04'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Number04 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number05'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Number05 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Number06'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Number06 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date01'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Date01 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date02'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Date02 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date03'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Date03 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('Date04'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @Date04 END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MassQuantity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('NetManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @NetManualValueFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MassManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MassManualValueFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('GrossManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @GrossManualValueFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('VcfManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @VcfManualValueFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredGrossManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DeliveredGrossManualValueFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('DeliveredNetManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @DeliveredNetManualValueFlag END), @TransactionSubLineItemGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('LookupTransactionStatusIndex'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @LookupTransactionStatusIndex END),@LookupQualityIndex,@TransactionLineItemGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('ProductGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @ProductGuid END),@TransactionGuid,(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('StorageLocationTankGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @StorageLocationTankGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('MeterGuid'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @MeterGuid END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('PackageManualValueFlag'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @PackageManualValueFlag END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLineItem'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @CleanLineItem END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLineDeductItem'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @CleanLineDeductItem END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLineDeductQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @CleanLineDeductQuantity END),(CASE (SELECT PATINDEX([sync].[udf_GetColumnSearchPattern]('CleanLinePackQuantity'), @sync_supported_columns_tblTransactionSubLineItems)) WHEN 0 THEN NULL ELSE @CleanLinePackQuantity END))
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionSubLineItemGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionSubLineItemGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionSubLineItemGuid)
        END
        SET NOCOUNT OFF
    END    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionSubLineItems] WHERE TransactionSubLineItemGuid = @TransactionSubLineItemGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
    
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(SI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END

