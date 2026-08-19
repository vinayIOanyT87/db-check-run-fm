-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionLineItems
-- Description: Apply Inserts
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalInserts_tblTransactionLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_last_received_anchor bigint,
@sync_start_daterange datetimeoffset(7),
@sync_end_daterange datetimeoffset(7),
@sync_filter_by_daterange bit,
@SequenceID smallint,
@MeterStart float,
@MeterStop float,
@GrossQuantity float,
@DeliveredGrossQuantity float,
@Temperature float,
@Vcf float,
@Density float,
@Product nvarchar(30),
@ProductCode nvarchar(30),
@ProductType nvarchar(20),
@ProductPrice float,
@CLIN nvarchar(10),
@NetQuantity float,
@DeliveredNetQuantity float,
@Pressure float,
@ContractNumber nvarchar(30),
@DestinationRegistrationID nvarchar(30),
@DestinationSerialNumber nvarchar(10),
@DestinationEquipmentType nvarchar(50),
@DestinationEquipmentModel nvarchar(20),
@DestinationCompanyEquipmentID nvarchar(30),
@DestinationCompartmentID nvarchar(50),
@SourceRegistrationID nvarchar(30),
@SourceSerialNumber nvarchar(10),
@SourceEquipmentType nvarchar(50),
@SourceEquipmentModel nvarchar(20),
@SourceCompanyEquipmentID nvarchar(30),
@SourceCompartmentID nvarchar(50),
@MeterFactor float,
@LineItemSequenceNumber nvarchar(5),
@BatchNumber nvarchar(20),
@DocumentNumber nvarchar(30),
@LineFill float,
@BottomVolume float,
@NetCapacity float,
@Customs nvarchar(20),
@ArmNumber int,
@LineNumber int,
@OperatorID nvarchar(50),
@TankStatus nvarchar(30),
@MeterStartDateTime datetimeoffset(7),
@MeterStopDateTime datetimeoffset(7),
@Pit nvarchar(10),
@RequestedDateTime datetimeoffset(7),
@DispatchedDateTime datetimeoffset(7),
@AcknowledgedDateTime datetimeoffset(7),
@OnLocationTime datetimeoffset(7),
@ValidationDateTime datetimeoffset(7),
@CompletionDateTime datetimeoffset(7),
@ReceiptVariance float,
@DifferentialPressure float,
@LoadRackVariance float,
@RequestedBy nvarchar(50),
@FreezePoint float,
@DeleteFlag bit,
@StorageLocationID nvarchar(50),
@MeterID nvarchar(50),
@AdditiveProfileID nvarchar(50),
@CreatedBy nvarchar(100),
@CreatedDate datetimeoffset(7),
@UpdatedBy nvarchar(100),
@UpdatedDate datetimeoffset(7),
@PresetAmount float,
@EngineeringUnitsIndex int,
@CustomerProductName nvarchar(50),
@CustomerProductCode nvarchar(20),
@TransactionInventoryDate date,
@COAWaiver bit,
@COANote nvarchar(50),
@COAID nvarchar(40),
@Tax1 float,
@Tax2 float,
@Tax3 float,
@Tax4 float,
@Tax5 float,
@TransVersion bigint,
@LoadingLocationID nvarchar(30),
@ImproperAdditization bit,
@BrokenBlend bit,
@ContaminatePrompt bit,
@CompartmentsPreviouslyLoaded bit,
@CompartmentsEmpty bit,
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
@OdometerHours float,
@EndDeliveryDate datetimeoffset(7),
@RequestedDeliveryDate datetimeoffset(7),
@InvoiceNumber nvarchar(50),
@InvoiceLineNumber nvarchar(50),
@AlternativeGrossVolume float,
@AlternativeNetVolume float,
@AlternativeUnits int,
@TankLevel float,
@TankLevelUnits int,
@Date01 datetimeoffset(7),
@Date02 datetimeoffset(7),
@Date03 datetimeoffset(7),
@Date04 datetimeoffset(7),
@NonDomesticPrice float,
@CurrencyUnit int,
@ExchangeRate float,
@QualityTestNumber nvarchar(50),
@Odometer float,
@DeliveryLocation nvarchar(50),
@Variance float,
@PartialFill bit,
@MassQuantity float,
@NetManualValueFlag bit,
@MassManualValueFlag bit,
@GrossManualValueFlag bit,
@VcfManualValueFlag bit,
@DeliveredGrossManualValueFlag bit,
@DeliveredNetManualValueFlag bit,
@TransactionLineItemGuid uniqueidentifier,
@LookupTransactionStatusIndex int,
@LookupQualityIndex int,
@StorageLocationTankGuid uniqueidentifier,
@AdditiveProfileGuid uniqueidentifier,
@DestinationCompartmentEquipmentGuid uniqueidentifier,
@DestinationEquipmentGuid uniqueidentifier,
@OperatorPersonnelGuid uniqueidentifier,
@ProductGuid uniqueidentifier,
@SourceCompartmentEquipmentGuid uniqueidentifier,
@SourceEquipmentGuid uniqueidentifier,
@TransactionGuid uniqueidentifier,
@CurrencyGuid uniqueidentifier,
@OrderReferenceTransactionLineItemGuid uniqueidentifier,
@LoadingLocationStationGuid uniqueidentifier,
@MeterGuid uniqueidentifier,
@PackageManualValueFlag bit,
@CleanLineItem bit,
@CleanLineDeductItem bit,
@CleanLineDeductQuantity float,
@CleanLinePackQuantity float,
@DualFuelingModeFlag bit,
@DualFuelingPrimaryFlag bit,
@EngineRunTime float,
@FlowRate float,
@FuelCompressionFactor float,
@HydrantPressure float,
@MobileDeviceID nvarchar(50),
@MobileDeviceGuid uniqueidentifier,
@TemperatureQualityStatus nvarchar(50),
@MeterStartObtainedAutomaticallyFlag bit,
@MeterStopObtainedAutomaticallyFlag bit,
@NetVolumeIndicator bit,
@sync_row_count int out,
@sync_table_name nvarchar(512)
AS
BEGIN
    DECLARE @minValidVersion BigInt 
    ;   MERGE [dbo].[tblTransactionLineItems] AS existingData
        USING (SELECT @SequenceID 'SequenceID',@MeterStart 'MeterStart',@MeterStop 'MeterStop',@GrossQuantity 'GrossQuantity',@DeliveredGrossQuantity 'DeliveredGrossQuantity',@Temperature 'Temperature',@Vcf 'Vcf',@Density 'Density',@Product 'Product',@ProductCode 'ProductCode',@ProductType 'ProductType',@ProductPrice 'ProductPrice',@CLIN 'CLIN',@NetQuantity 'NetQuantity',@DeliveredNetQuantity 'DeliveredNetQuantity',@Pressure 'Pressure',@ContractNumber 'ContractNumber',@DestinationRegistrationID 'DestinationRegistrationID',@DestinationSerialNumber 'DestinationSerialNumber',@DestinationEquipmentType 'DestinationEquipmentType',@DestinationEquipmentModel 'DestinationEquipmentModel',@DestinationCompanyEquipmentID 'DestinationCompanyEquipmentID',@DestinationCompartmentID 'DestinationCompartmentID',@SourceRegistrationID 'SourceRegistrationID',@SourceSerialNumber 'SourceSerialNumber',@SourceEquipmentType 'SourceEquipmentType',@SourceEquipmentModel 'SourceEquipmentModel',@SourceCompanyEquipmentID 'SourceCompanyEquipmentID',@SourceCompartmentID 'SourceCompartmentID',@MeterFactor 'MeterFactor',@LineItemSequenceNumber 'LineItemSequenceNumber',@BatchNumber 'BatchNumber',@DocumentNumber 'DocumentNumber',@LineFill 'LineFill',@BottomVolume 'BottomVolume',@NetCapacity 'NetCapacity',@Customs 'Customs',@ArmNumber 'ArmNumber',@LineNumber 'LineNumber',@OperatorID 'OperatorID',@TankStatus 'TankStatus',@MeterStartDateTime 'MeterStartDateTime',@MeterStopDateTime 'MeterStopDateTime',@Pit 'Pit',@RequestedDateTime 'RequestedDateTime',@DispatchedDateTime 'DispatchedDateTime',@AcknowledgedDateTime 'AcknowledgedDateTime',@OnLocationTime 'OnLocationTime',@ValidationDateTime 'ValidationDateTime',@CompletionDateTime 'CompletionDateTime',@ReceiptVariance 'ReceiptVariance',@DifferentialPressure 'DifferentialPressure',@LoadRackVariance 'LoadRackVariance',@RequestedBy 'RequestedBy',@FreezePoint 'FreezePoint',@DeleteFlag 'DeleteFlag',@StorageLocationID 'StorageLocationID',@MeterID 'MeterID',@AdditiveProfileID 'AdditiveProfileID',@CreatedBy 'CreatedBy',@CreatedDate 'CreatedDate',@UpdatedBy 'UpdatedBy',@UpdatedDate 'UpdatedDate',@PresetAmount 'PresetAmount',@EngineeringUnitsIndex 'EngineeringUnitsIndex',@CustomerProductName 'CustomerProductName',@CustomerProductCode 'CustomerProductCode',@TransactionInventoryDate 'TransactionInventoryDate',@COAWaiver 'COAWaiver',@COANote 'COANote',@COAID 'COAID',@Tax1 'Tax1',@Tax2 'Tax2',@Tax3 'Tax3',@Tax4 'Tax4',@Tax5 'Tax5',@TransVersion 'TransVersion',@LoadingLocationID 'LoadingLocationID',@ImproperAdditization 'ImproperAdditization',@BrokenBlend 'BrokenBlend',@ContaminatePrompt 'ContaminatePrompt',@CompartmentsPreviouslyLoaded 'CompartmentsPreviouslyLoaded',@CompartmentsEmpty 'CompartmentsEmpty',@Flag01 'Flag01',@Flag02 'Flag02',@Flag03 'Flag03',@Flag04 'Flag04',@Flag05 'Flag05',@Flag06 'Flag06',@Number01 'Number01',@Number02 'Number02',@Number03 'Number03',@Number04 'Number04',@Number05 'Number05',@Number06 'Number06',@OdometerHours 'OdometerHours',@EndDeliveryDate 'EndDeliveryDate',@RequestedDeliveryDate 'RequestedDeliveryDate',@InvoiceNumber 'InvoiceNumber',@InvoiceLineNumber 'InvoiceLineNumber',@AlternativeGrossVolume 'AlternativeGrossVolume',@AlternativeNetVolume 'AlternativeNetVolume',@AlternativeUnits 'AlternativeUnits',@TankLevel 'TankLevel',@TankLevelUnits 'TankLevelUnits',@Date01 'Date01',@Date02 'Date02',@Date03 'Date03',@Date04 'Date04',@NonDomesticPrice 'NonDomesticPrice',@CurrencyUnit 'CurrencyUnit',@ExchangeRate 'ExchangeRate',@QualityTestNumber 'QualityTestNumber',@Odometer 'Odometer',@DeliveryLocation 'DeliveryLocation',@Variance 'Variance',@PartialFill 'PartialFill',@MassQuantity 'MassQuantity',@NetManualValueFlag 'NetManualValueFlag',@MassManualValueFlag 'MassManualValueFlag',@GrossManualValueFlag 'GrossManualValueFlag',@VcfManualValueFlag 'VcfManualValueFlag',@DeliveredGrossManualValueFlag 'DeliveredGrossManualValueFlag',@DeliveredNetManualValueFlag 'DeliveredNetManualValueFlag',@TransactionLineItemGuid 'TransactionLineItemGuid',@LookupTransactionStatusIndex 'LookupTransactionStatusIndex',@LookupQualityIndex 'LookupQualityIndex',@StorageLocationTankGuid 'StorageLocationTankGuid',@AdditiveProfileGuid 'AdditiveProfileGuid',@DestinationCompartmentEquipmentGuid 'DestinationCompartmentEquipmentGuid',@DestinationEquipmentGuid 'DestinationEquipmentGuid',@OperatorPersonnelGuid 'OperatorPersonnelGuid',@ProductGuid 'ProductGuid',@SourceCompartmentEquipmentGuid 'SourceCompartmentEquipmentGuid',@SourceEquipmentGuid 'SourceEquipmentGuid',@TransactionGuid 'TransactionGuid',@CurrencyGuid 'CurrencyGuid',@OrderReferenceTransactionLineItemGuid 'OrderReferenceTransactionLineItemGuid',@LoadingLocationStationGuid 'LoadingLocationStationGuid',@MeterGuid 'MeterGuid',@PackageManualValueFlag 'PackageManualValueFlag',@CleanLineItem 'CleanLineItem',@CleanLineDeductItem 'CleanLineDeductItem',@CleanLineDeductQuantity 'CleanLineDeductQuantity',@CleanLinePackQuantity 'CleanLinePackQuantity',@DualFuelingModeFlag 'DualFuelingModeFlag',@DualFuelingPrimaryFlag 'DualFuelingPrimaryFlag',@EngineRunTime 'EngineRunTime',@FlowRate 'FlowRate',@FuelCompressionFactor 'FuelCompressionFactor',@HydrantPressure 'HydrantPressure',@MobileDeviceID 'MobileDeviceID',@MobileDeviceGuid 'MobileDeviceGuid',@TemperatureQualityStatus 'TemperatureQualityStatus',@MeterStartObtainedAutomaticallyFlag 'MeterStartObtainedAutomaticallyFlag',@MeterStopObtainedAutomaticallyFlag 'MeterStopObtainedAutomaticallyFlag',@NetVolumeIndicator 'NetVolumeIndicator'
                ) AS remoteChanges ([SequenceID],[MeterStart],[MeterStop],[GrossQuantity],[DeliveredGrossQuantity],[Temperature],[Vcf],[Density],[Product],[ProductCode],[ProductType],[ProductPrice],[CLIN],[NetQuantity],[DeliveredNetQuantity],[Pressure],[ContractNumber],[DestinationRegistrationID],[DestinationSerialNumber],[DestinationEquipmentType],[DestinationEquipmentModel],[DestinationCompanyEquipmentID],[DestinationCompartmentID],[SourceRegistrationID],[SourceSerialNumber],[SourceEquipmentType],[SourceEquipmentModel],[SourceCompanyEquipmentID],[SourceCompartmentID],[MeterFactor],[LineItemSequenceNumber],[BatchNumber],[DocumentNumber],[LineFill],[BottomVolume],[NetCapacity],[Customs],[ArmNumber],[LineNumber],[OperatorID],[TankStatus],[MeterStartDateTime],[MeterStopDateTime],[Pit],[RequestedDateTime],[DispatchedDateTime],[AcknowledgedDateTime],[OnLocationTime],[ValidationDateTime],[CompletionDateTime],[ReceiptVariance],[DifferentialPressure],[LoadRackVariance],[RequestedBy],[FreezePoint],[DeleteFlag],[StorageLocationID],[MeterID],[AdditiveProfileID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[PresetAmount],[EngineeringUnitsIndex],[CustomerProductName],[CustomerProductCode],[TransactionInventoryDate],[COAWaiver],[COANote],[COAID],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[LoadingLocationID],[ImproperAdditization],[BrokenBlend],[ContaminatePrompt],[CompartmentsPreviouslyLoaded],[CompartmentsEmpty],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[OdometerHours],[EndDeliveryDate],[RequestedDeliveryDate],[InvoiceNumber],[InvoiceLineNumber],[AlternativeGrossVolume],[AlternativeNetVolume],[AlternativeUnits],[TankLevel],[TankLevelUnits],[Date01],[Date02],[Date03],[Date04],[NonDomesticPrice],[CurrencyUnit],[ExchangeRate],[QualityTestNumber],[Odometer],[DeliveryLocation],[Variance],[PartialFill],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag],[TransactionLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[StorageLocationTankGuid],[AdditiveProfileGuid],[DestinationCompartmentEquipmentGuid],[DestinationEquipmentGuid],[OperatorPersonnelGuid],[ProductGuid],[SourceCompartmentEquipmentGuid],[SourceEquipmentGuid],[TransactionGuid],[CurrencyGuid],[OrderReferenceTransactionLineItemGuid],[LoadingLocationStationGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity],[DualFuelingModeFlag],[DualFuelingPrimaryFlag],[EngineRunTime],[FlowRate],[FuelCompressionFactor],[HydrantPressure],[MobileDeviceID],[MobileDeviceGuid],[TemperatureQualityStatus],[MeterStartObtainedAutomaticallyFlag],[MeterStopObtainedAutomaticallyFlag],[NetVolumeIndicator])
        ON (existingData.[TransactionLineItemGuid] = remoteChanges.[TransactionLineItemGuid])
        WHEN MATCHED AND (remoteChanges.CreatedDate > existingData.CreatedDate OR remoteChanges.UpdatedDate > existingData.UpdatedDate) THEN
            UPDATE SET [SequenceID] = remoteChanges.[SequenceID]
                       ,[MeterStart] = remoteChanges.[MeterStart]
                       ,[MeterStop] = remoteChanges.[MeterStop]
                       ,[GrossQuantity] = remoteChanges.[GrossQuantity]
                       ,[DeliveredGrossQuantity] = remoteChanges.[DeliveredGrossQuantity]
                       ,[Temperature] = remoteChanges.[Temperature]
                       ,[Vcf] = remoteChanges.[Vcf]
                       ,[Density] = remoteChanges.[Density]
                       ,[Product] = remoteChanges.[Product]
                       ,[ProductCode] = remoteChanges.[ProductCode]
                       ,[ProductType] = remoteChanges.[ProductType]
                       ,[ProductPrice] = remoteChanges.[ProductPrice]
                       ,[CLIN] = remoteChanges.[CLIN]
                       ,[NetQuantity] = remoteChanges.[NetQuantity]
                       ,[DeliveredNetQuantity] = remoteChanges.[DeliveredNetQuantity]
                       ,[Pressure] = remoteChanges.[Pressure]
                       ,[ContractNumber] = remoteChanges.[ContractNumber]
                       ,[DestinationRegistrationID] = remoteChanges.[DestinationRegistrationID]
                       ,[DestinationSerialNumber] = remoteChanges.[DestinationSerialNumber]
                       ,[DestinationEquipmentType] = remoteChanges.[DestinationEquipmentType]
                       ,[DestinationEquipmentModel] = remoteChanges.[DestinationEquipmentModel]
                       ,[DestinationCompanyEquipmentID] = remoteChanges.[DestinationCompanyEquipmentID]
                       ,[DestinationCompartmentID] = remoteChanges.[DestinationCompartmentID]
                       ,[SourceRegistrationID] = remoteChanges.[SourceRegistrationID]
                       ,[SourceSerialNumber] = remoteChanges.[SourceSerialNumber]
                       ,[SourceEquipmentType] = remoteChanges.[SourceEquipmentType]
                       ,[SourceEquipmentModel] = remoteChanges.[SourceEquipmentModel]
                       ,[SourceCompanyEquipmentID] = remoteChanges.[SourceCompanyEquipmentID]
                       ,[SourceCompartmentID] = remoteChanges.[SourceCompartmentID]
                       ,[MeterFactor] = remoteChanges.[MeterFactor]
                       ,[LineItemSequenceNumber] = remoteChanges.[LineItemSequenceNumber]
                       ,[BatchNumber] = remoteChanges.[BatchNumber]
                       ,[DocumentNumber] = remoteChanges.[DocumentNumber]
                       ,[LineFill] = remoteChanges.[LineFill]
                       ,[BottomVolume] = remoteChanges.[BottomVolume]
                       ,[NetCapacity] = remoteChanges.[NetCapacity]
                       ,[Customs] = remoteChanges.[Customs]
                       ,[ArmNumber] = remoteChanges.[ArmNumber]
                       ,[LineNumber] = remoteChanges.[LineNumber]
                       ,[OperatorID] = remoteChanges.[OperatorID]
                       ,[TankStatus] = remoteChanges.[TankStatus]
                       ,[MeterStartDateTime] = remoteChanges.[MeterStartDateTime]
                       ,[MeterStopDateTime] = remoteChanges.[MeterStopDateTime]
                       ,[Pit] = remoteChanges.[Pit]
                       ,[RequestedDateTime] = remoteChanges.[RequestedDateTime]
                       ,[DispatchedDateTime] = remoteChanges.[DispatchedDateTime]
                       ,[AcknowledgedDateTime] = remoteChanges.[AcknowledgedDateTime]
                       ,[OnLocationTime] = remoteChanges.[OnLocationTime]
                       ,[ValidationDateTime] = remoteChanges.[ValidationDateTime]
                       ,[CompletionDateTime] = remoteChanges.[CompletionDateTime]
                       ,[ReceiptVariance] = remoteChanges.[ReceiptVariance]
                       ,[DifferentialPressure] = remoteChanges.[DifferentialPressure]
                       ,[LoadRackVariance] = remoteChanges.[LoadRackVariance]
                       ,[RequestedBy] = remoteChanges.[RequestedBy]
                       ,[FreezePoint] = remoteChanges.[FreezePoint]
                       ,[DeleteFlag] = remoteChanges.[DeleteFlag]
                       ,[StorageLocationID] = remoteChanges.[StorageLocationID]
                       ,[MeterID] = remoteChanges.[MeterID]
                       ,[AdditiveProfileID] = remoteChanges.[AdditiveProfileID]
                       ,[CreatedBy] = remoteChanges.[CreatedBy]
                       ,[CreatedDate] = remoteChanges.[CreatedDate]
                       ,[UpdatedBy] = remoteChanges.[UpdatedBy]
                       ,[UpdatedDate] = remoteChanges.[UpdatedDate]
                       ,[PresetAmount] = remoteChanges.[PresetAmount]
                       ,[EngineeringUnitsIndex] = remoteChanges.[EngineeringUnitsIndex]
                       ,[CustomerProductName] = remoteChanges.[CustomerProductName]
                       ,[CustomerProductCode] = remoteChanges.[CustomerProductCode]
                       ,[TransactionInventoryDate] = remoteChanges.[TransactionInventoryDate]
                       ,[COAWaiver] = remoteChanges.[COAWaiver]
                       ,[COANote] = remoteChanges.[COANote]
                       ,[COAID] = remoteChanges.[COAID]
                       ,[Tax1] = remoteChanges.[Tax1]
                       ,[Tax2] = remoteChanges.[Tax2]
                       ,[Tax3] = remoteChanges.[Tax3]
                       ,[Tax4] = remoteChanges.[Tax4]
                       ,[Tax5] = remoteChanges.[Tax5]
                       ,[TransVersion] = remoteChanges.[TransVersion]
                       ,[LoadingLocationID] = remoteChanges.[LoadingLocationID]
                       ,[ImproperAdditization] = remoteChanges.[ImproperAdditization]
                       ,[BrokenBlend] = remoteChanges.[BrokenBlend]
                       ,[ContaminatePrompt] = remoteChanges.[ContaminatePrompt]
                       ,[CompartmentsPreviouslyLoaded] = remoteChanges.[CompartmentsPreviouslyLoaded]
                       ,[CompartmentsEmpty] = remoteChanges.[CompartmentsEmpty]
                       ,[Flag01] = remoteChanges.[Flag01]
                       ,[Flag02] = remoteChanges.[Flag02]
                       ,[Flag03] = remoteChanges.[Flag03]
                       ,[Flag04] = remoteChanges.[Flag04]
                       ,[Flag05] = remoteChanges.[Flag05]
                       ,[Flag06] = remoteChanges.[Flag06]
                       ,[Number01] = remoteChanges.[Number01]
                       ,[Number02] = remoteChanges.[Number02]
                       ,[Number03] = remoteChanges.[Number03]
                       ,[Number04] = remoteChanges.[Number04]
                       ,[Number05] = remoteChanges.[Number05]
                       ,[Number06] = remoteChanges.[Number06]
                       ,[OdometerHours] = remoteChanges.[OdometerHours]
                       ,[EndDeliveryDate] = remoteChanges.[EndDeliveryDate]
                       ,[RequestedDeliveryDate] = remoteChanges.[RequestedDeliveryDate]
                       ,[InvoiceNumber] = remoteChanges.[InvoiceNumber]
                       ,[InvoiceLineNumber] = remoteChanges.[InvoiceLineNumber]
                       ,[AlternativeGrossVolume] = remoteChanges.[AlternativeGrossVolume]
                       ,[AlternativeNetVolume] = remoteChanges.[AlternativeNetVolume]
                       ,[AlternativeUnits] = remoteChanges.[AlternativeUnits]
                       ,[TankLevel] = remoteChanges.[TankLevel]
                       ,[TankLevelUnits] = remoteChanges.[TankLevelUnits]
                       ,[Date01] = remoteChanges.[Date01]
                       ,[Date02] = remoteChanges.[Date02]
                       ,[Date03] = remoteChanges.[Date03]
                       ,[Date04] = remoteChanges.[Date04]
                       ,[NonDomesticPrice] = remoteChanges.[NonDomesticPrice]
                       ,[CurrencyUnit] = remoteChanges.[CurrencyUnit]
                       ,[ExchangeRate] = remoteChanges.[ExchangeRate]
                       ,[QualityTestNumber] = remoteChanges.[QualityTestNumber]
                       ,[Odometer] = remoteChanges.[Odometer]
                       ,[DeliveryLocation] = remoteChanges.[DeliveryLocation]
                       ,[Variance] = remoteChanges.[Variance]
                       ,[PartialFill] = remoteChanges.[PartialFill]
                       ,[MassQuantity] = remoteChanges.[MassQuantity]
                       ,[NetManualValueFlag] = remoteChanges.[NetManualValueFlag]
                       ,[MassManualValueFlag] = remoteChanges.[MassManualValueFlag]
                       ,[GrossManualValueFlag] = remoteChanges.[GrossManualValueFlag]
                       ,[VcfManualValueFlag] = remoteChanges.[VcfManualValueFlag]
                       ,[DeliveredGrossManualValueFlag] = remoteChanges.[DeliveredGrossManualValueFlag]
                       ,[DeliveredNetManualValueFlag] = remoteChanges.[DeliveredNetManualValueFlag]
                       ,[LookupTransactionStatusIndex] = remoteChanges.[LookupTransactionStatusIndex]
                       ,[LookupQualityIndex] = remoteChanges.[LookupQualityIndex]
                       ,[StorageLocationTankGuid] = remoteChanges.[StorageLocationTankGuid]
                       ,[AdditiveProfileGuid] = remoteChanges.[AdditiveProfileGuid]
                       ,[DestinationCompartmentEquipmentGuid] = remoteChanges.[DestinationCompartmentEquipmentGuid]
                       ,[DestinationEquipmentGuid] = remoteChanges.[DestinationEquipmentGuid]
                       ,[OperatorPersonnelGuid] = remoteChanges.[OperatorPersonnelGuid]
                       ,[ProductGuid] = remoteChanges.[ProductGuid]
                       ,[SourceCompartmentEquipmentGuid] = remoteChanges.[SourceCompartmentEquipmentGuid]
                       ,[SourceEquipmentGuid] = remoteChanges.[SourceEquipmentGuid]
                       ,[TransactionGuid] = remoteChanges.[TransactionGuid]
                       ,[CurrencyGuid] = remoteChanges.[CurrencyGuid]
                       ,[OrderReferenceTransactionLineItemGuid] = remoteChanges.[OrderReferenceTransactionLineItemGuid]
                       ,[LoadingLocationStationGuid] = remoteChanges.[LoadingLocationStationGuid]
                       ,[MeterGuid] = remoteChanges.[MeterGuid]
                       ,[PackageManualValueFlag] = remoteChanges.[PackageManualValueFlag]
                       ,[CleanLineItem] = remoteChanges.[CleanLineItem]
                       ,[CleanLineDeductItem] = remoteChanges.[CleanLineDeductItem]
                       ,[CleanLineDeductQuantity] = remoteChanges.[CleanLineDeductQuantity]
                       ,[CleanLinePackQuantity] = remoteChanges.[CleanLinePackQuantity]
                       ,[DualFuelingModeFlag] = remoteChanges.[DualFuelingModeFlag]
                       ,[DualFuelingPrimaryFlag] = remoteChanges.[DualFuelingPrimaryFlag]
                       ,[EngineRunTime] = remoteChanges.[EngineRunTime]
                       ,[FlowRate] = remoteChanges.[FlowRate]
                       ,[FuelCompressionFactor] = remoteChanges.[FuelCompressionFactor]
                       ,[HydrantPressure] = remoteChanges.[HydrantPressure]
                       ,[MobileDeviceID] = remoteChanges.[MobileDeviceID]
                       ,[MobileDeviceGuid] = remoteChanges.[MobileDeviceGuid]
                       ,[TemperatureQualityStatus] = remoteChanges.[TemperatureQualityStatus]
                       ,[MeterStartObtainedAutomaticallyFlag] = remoteChanges.[MeterStartObtainedAutomaticallyFlag]
                       ,[MeterStopObtainedAutomaticallyFlag] = remoteChanges.[MeterStopObtainedAutomaticallyFlag]
                       ,[NetVolumeIndicator] = remoteChanges.[NetVolumeIndicator]

        WHEN NOT MATCHED THEN
            INSERT ([SequenceID],[MeterStart],[MeterStop],[GrossQuantity],[DeliveredGrossQuantity],[Temperature],[Vcf],[Density],[Product],[ProductCode],[ProductType],[ProductPrice],[CLIN],[NetQuantity],[DeliveredNetQuantity],[Pressure],[ContractNumber],[DestinationRegistrationID],[DestinationSerialNumber],[DestinationEquipmentType],[DestinationEquipmentModel],[DestinationCompanyEquipmentID],[DestinationCompartmentID],[SourceRegistrationID],[SourceSerialNumber],[SourceEquipmentType],[SourceEquipmentModel],[SourceCompanyEquipmentID],[SourceCompartmentID],[MeterFactor],[LineItemSequenceNumber],[BatchNumber],[DocumentNumber],[LineFill],[BottomVolume],[NetCapacity],[Customs],[ArmNumber],[LineNumber],[OperatorID],[TankStatus],[MeterStartDateTime],[MeterStopDateTime],[Pit],[RequestedDateTime],[DispatchedDateTime],[AcknowledgedDateTime],[OnLocationTime],[ValidationDateTime],[CompletionDateTime],[ReceiptVariance],[DifferentialPressure],[LoadRackVariance],[RequestedBy],[FreezePoint],[DeleteFlag],[StorageLocationID],[MeterID],[AdditiveProfileID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[PresetAmount],[EngineeringUnitsIndex],[CustomerProductName],[CustomerProductCode],[TransactionInventoryDate],[COAWaiver],[COANote],[COAID],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[LoadingLocationID],[ImproperAdditization],[BrokenBlend],[ContaminatePrompt],[CompartmentsPreviouslyLoaded],[CompartmentsEmpty],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[OdometerHours],[EndDeliveryDate],[RequestedDeliveryDate],[InvoiceNumber],[InvoiceLineNumber],[AlternativeGrossVolume],[AlternativeNetVolume],[AlternativeUnits],[TankLevel],[TankLevelUnits],[Date01],[Date02],[Date03],[Date04],[NonDomesticPrice],[CurrencyUnit],[ExchangeRate],[QualityTestNumber],[Odometer],[DeliveryLocation],[Variance],[PartialFill],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag],[TransactionLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[StorageLocationTankGuid],[AdditiveProfileGuid],[DestinationCompartmentEquipmentGuid],[DestinationEquipmentGuid],[OperatorPersonnelGuid],[ProductGuid],[SourceCompartmentEquipmentGuid],[SourceEquipmentGuid],[TransactionGuid],[CurrencyGuid],[OrderReferenceTransactionLineItemGuid],[LoadingLocationStationGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity],[DualFuelingModeFlag],[DualFuelingPrimaryFlag],[EngineRunTime],[FlowRate],[FuelCompressionFactor],[HydrantPressure],[MobileDeviceID],[MobileDeviceGuid],[TemperatureQualityStatus],[MeterStartObtainedAutomaticallyFlag],[MeterStopObtainedAutomaticallyFlag],[NetVolumeIndicator])
                VALUES (@SequenceID,@MeterStart,@MeterStop,@GrossQuantity,@DeliveredGrossQuantity,@Temperature,@Vcf,@Density,@Product,@ProductCode,@ProductType,@ProductPrice,@CLIN,@NetQuantity,@DeliveredNetQuantity,@Pressure,@ContractNumber,@DestinationRegistrationID,@DestinationSerialNumber,@DestinationEquipmentType,@DestinationEquipmentModel,@DestinationCompanyEquipmentID,@DestinationCompartmentID,@SourceRegistrationID,@SourceSerialNumber,@SourceEquipmentType,@SourceEquipmentModel,@SourceCompanyEquipmentID,@SourceCompartmentID,@MeterFactor,@LineItemSequenceNumber,@BatchNumber,@DocumentNumber,@LineFill,@BottomVolume,@NetCapacity,@Customs,@ArmNumber,@LineNumber,@OperatorID,@TankStatus,@MeterStartDateTime,@MeterStopDateTime,@Pit,@RequestedDateTime,@DispatchedDateTime,@AcknowledgedDateTime,@OnLocationTime,@ValidationDateTime,@CompletionDateTime,@ReceiptVariance,@DifferentialPressure,@LoadRackVariance,@RequestedBy,@FreezePoint,@DeleteFlag,@StorageLocationID,@MeterID,@AdditiveProfileID,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@PresetAmount,@EngineeringUnitsIndex,@CustomerProductName,@CustomerProductCode,@TransactionInventoryDate,@COAWaiver,@COANote,@COAID,@Tax1,@Tax2,@Tax3,@Tax4,@Tax5,@TransVersion,@LoadingLocationID,@ImproperAdditization,@BrokenBlend,@ContaminatePrompt,@CompartmentsPreviouslyLoaded,@CompartmentsEmpty,@Flag01,@Flag02,@Flag03,@Flag04,@Flag05,@Flag06,@Number01,@Number02,@Number03,@Number04,@Number05,@Number06,@OdometerHours,@EndDeliveryDate,@RequestedDeliveryDate,@InvoiceNumber,@InvoiceLineNumber,@AlternativeGrossVolume,@AlternativeNetVolume,@AlternativeUnits,@TankLevel,@TankLevelUnits,@Date01,@Date02,@Date03,@Date04,@NonDomesticPrice,@CurrencyUnit,@ExchangeRate,@QualityTestNumber,@Odometer,@DeliveryLocation,@Variance,@PartialFill,@MassQuantity,@NetManualValueFlag,@MassManualValueFlag,@GrossManualValueFlag,@VcfManualValueFlag,@DeliveredGrossManualValueFlag,@DeliveredNetManualValueFlag,@TransactionLineItemGuid,@LookupTransactionStatusIndex,@LookupQualityIndex,@StorageLocationTankGuid,@AdditiveProfileGuid,@DestinationCompartmentEquipmentGuid,@DestinationEquipmentGuid,@OperatorPersonnelGuid,@ProductGuid,@SourceCompartmentEquipmentGuid,@SourceEquipmentGuid,@TransactionGuid,@CurrencyGuid,@OrderReferenceTransactionLineItemGuid,@LoadingLocationStationGuid,@MeterGuid,@PackageManualValueFlag,@CleanLineItem,@CleanLineDeductItem,@CleanLineDeductQuantity,@CleanLinePackQuantity,@DualFuelingModeFlag,@DualFuelingPrimaryFlag,@EngineRunTime,@FlowRate,@FuelCompressionFactor,@HydrantPressure,@MobileDeviceID,@MobileDeviceGuid,@TemperatureQualityStatus,@MeterStartObtainedAutomaticallyFlag,@MeterStopObtainedAutomaticallyFlag,@NetVolumeIndicator)
        ;
    
    SET @sync_row_count = @@rowcount;
    
    -- If we updated / inserted a record here, go ahead and remove any pending conflict records associated with this ID because this record should contain the most recent data.
    IF (@sync_row_count > 0)
    BEGIN
        SET NOCOUNT ON
        IF EXISTS (SELECT 1 FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionLineItemGuid) AND SyncConflictResolutionStatusIndex IN (0,3))
        BEGIN
            DELETE FROM [sync].[tblSyncRecordConflictToSyncSessionScopeLog] WHERE SyncRecordConflictGuid IN (SELECT SyncRecordConflictGuid FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionLineItemGuid))
            DELETE FROM [sync].[tblSyncRecordConflict] WHERE RecordKey = CONVERT(nvarchar(512), @TransactionLineItemGuid)
        END
        SET NOCOUNT OFF
    END
    

    -- One issue with the MERGE approach is that we don't know if the @@rowcount is zero because of a true conflict now or if it was simply because
    -- the CreatedDate and UpdatedDate qualifiers were not met (in which case we want to simply throw away the update)
    -- In order to determine what took place, we're going to need to perform a query if @@rowcount = 0 to see if it was caused by the additional qualifiers.
    --
    IF (@sync_row_count = 0)
    BEGIN
        IF EXISTS (SELECT 1 FROM [dbo].[tblTransactionLineItems] WHERE TransactionLineItemGuid = @TransactionLineItemGuid AND (CreatedDate >= @CreatedDate OR UpdatedDate >= @UpdatedDate))
            SET @sync_row_count = 1;    
    END

    SET @minValidVersion = 0;   -- This is used to detect Change Tracking cleanup
                                -- If we support this, we should add a column to SynchronizationTable
                                -- that records the MinValidVersion after change tracking information for
                                -- a table gets cleaned up.  I don't think this will be necessary.
                                        
    IF @minValidVersion > @sync_last_received_anchor
        RAISERROR(N'(CI)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
