-- ========================================================================================
-- Author:      <Author,,George Peters>
-- Create date: <Create Date,System.DateTime,>
-- SyncTable  : dbo.tblTransactionLineItems
-- Description: Apply Updates
-- ========================================================================================
CREATE PROCEDURE [sync].[gsp_ClientApplyIncrementalUpdates_tblTransactionLineItems]
@sync_client_id_binary binary(16),
@sync_client_id uniqueidentifier,
@sync_force_write int,
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
    DECLARE @sync_last_received_anchor_varbinary varbinary(8)

    DECLARE @wasDeleted int

    SET @sync_last_received_anchor_varbinary = CONVERT(varbinary(8), @sync_last_received_anchor);

    SET @wasDeleted = 0
    
    IF EXISTS (SELECT 1 FROM [track].[tblTransactionLineItems] CT
                        WHERE CT.PK_TransactionLineItemGuid = @TransactionLineItemGuid
                                AND (CT.DeletedRowVersion IS NOT NULL))
    BEGIN
        SET @wasDeleted = 1
    END

    IF (@wasDeleted = 0)
    BEGIN
        ;   WITH existingData AS (
                SELECT [dbo].[tblTransactionLineItems].[SequenceID],[dbo].[tblTransactionLineItems].[MeterStart],[dbo].[tblTransactionLineItems].[MeterStop],[dbo].[tblTransactionLineItems].[GrossQuantity],[dbo].[tblTransactionLineItems].[DeliveredGrossQuantity],[dbo].[tblTransactionLineItems].[Temperature],[dbo].[tblTransactionLineItems].[Vcf],[dbo].[tblTransactionLineItems].[Density],[dbo].[tblTransactionLineItems].[Product],[dbo].[tblTransactionLineItems].[ProductCode],[dbo].[tblTransactionLineItems].[ProductType],[dbo].[tblTransactionLineItems].[ProductPrice],[dbo].[tblTransactionLineItems].[CLIN],[dbo].[tblTransactionLineItems].[NetQuantity],[dbo].[tblTransactionLineItems].[DeliveredNetQuantity],[dbo].[tblTransactionLineItems].[Pressure],[dbo].[tblTransactionLineItems].[ContractNumber],[dbo].[tblTransactionLineItems].[DestinationRegistrationID],[dbo].[tblTransactionLineItems].[DestinationSerialNumber],[dbo].[tblTransactionLineItems].[DestinationEquipmentType],[dbo].[tblTransactionLineItems].[DestinationEquipmentModel],[dbo].[tblTransactionLineItems].[DestinationCompanyEquipmentID],[dbo].[tblTransactionLineItems].[DestinationCompartmentID],[dbo].[tblTransactionLineItems].[SourceRegistrationID],[dbo].[tblTransactionLineItems].[SourceSerialNumber],[dbo].[tblTransactionLineItems].[SourceEquipmentType],[dbo].[tblTransactionLineItems].[SourceEquipmentModel],[dbo].[tblTransactionLineItems].[SourceCompanyEquipmentID],[dbo].[tblTransactionLineItems].[SourceCompartmentID],[dbo].[tblTransactionLineItems].[MeterFactor],[dbo].[tblTransactionLineItems].[LineItemSequenceNumber],[dbo].[tblTransactionLineItems].[BatchNumber],[dbo].[tblTransactionLineItems].[DocumentNumber],[dbo].[tblTransactionLineItems].[LineFill],[dbo].[tblTransactionLineItems].[BottomVolume],[dbo].[tblTransactionLineItems].[NetCapacity],[dbo].[tblTransactionLineItems].[Customs],[dbo].[tblTransactionLineItems].[ArmNumber],[dbo].[tblTransactionLineItems].[LineNumber],[dbo].[tblTransactionLineItems].[OperatorID],[dbo].[tblTransactionLineItems].[TankStatus],[dbo].[tblTransactionLineItems].[MeterStartDateTime],[dbo].[tblTransactionLineItems].[MeterStopDateTime],[dbo].[tblTransactionLineItems].[Pit],[dbo].[tblTransactionLineItems].[RequestedDateTime],[dbo].[tblTransactionLineItems].[DispatchedDateTime],[dbo].[tblTransactionLineItems].[AcknowledgedDateTime],[dbo].[tblTransactionLineItems].[OnLocationTime],[dbo].[tblTransactionLineItems].[ValidationDateTime],[dbo].[tblTransactionLineItems].[CompletionDateTime],[dbo].[tblTransactionLineItems].[ReceiptVariance],[dbo].[tblTransactionLineItems].[DifferentialPressure],[dbo].[tblTransactionLineItems].[LoadRackVariance],[dbo].[tblTransactionLineItems].[RequestedBy],[dbo].[tblTransactionLineItems].[FreezePoint],[dbo].[tblTransactionLineItems].[DeleteFlag],[dbo].[tblTransactionLineItems].[StorageLocationID],[dbo].[tblTransactionLineItems].[MeterID],[dbo].[tblTransactionLineItems].[AdditiveProfileID],[dbo].[tblTransactionLineItems].[CreatedBy],[dbo].[tblTransactionLineItems].[CreatedDate],[dbo].[tblTransactionLineItems].[UpdatedBy],[dbo].[tblTransactionLineItems].[UpdatedDate],[dbo].[tblTransactionLineItems].[PresetAmount],[dbo].[tblTransactionLineItems].[EngineeringUnitsIndex],[dbo].[tblTransactionLineItems].[CustomerProductName],[dbo].[tblTransactionLineItems].[CustomerProductCode],[dbo].[tblTransactionLineItems].[TransactionInventoryDate],[dbo].[tblTransactionLineItems].[COAWaiver],[dbo].[tblTransactionLineItems].[COANote],[dbo].[tblTransactionLineItems].[COAID],[dbo].[tblTransactionLineItems].[Tax1],[dbo].[tblTransactionLineItems].[Tax2],[dbo].[tblTransactionLineItems].[Tax3],[dbo].[tblTransactionLineItems].[Tax4],[dbo].[tblTransactionLineItems].[Tax5],[dbo].[tblTransactionLineItems].[TransVersion],[dbo].[tblTransactionLineItems].[LoadingLocationID],[dbo].[tblTransactionLineItems].[ImproperAdditization],[dbo].[tblTransactionLineItems].[BrokenBlend],[dbo].[tblTransactionLineItems].[ContaminatePrompt],[dbo].[tblTransactionLineItems].[CompartmentsPreviouslyLoaded],[dbo].[tblTransactionLineItems].[CompartmentsEmpty],[dbo].[tblTransactionLineItems].[Flag01],[dbo].[tblTransactionLineItems].[Flag02],[dbo].[tblTransactionLineItems].[Flag03],[dbo].[tblTransactionLineItems].[Flag04],[dbo].[tblTransactionLineItems].[Flag05],[dbo].[tblTransactionLineItems].[Flag06],[dbo].[tblTransactionLineItems].[Number01],[dbo].[tblTransactionLineItems].[Number02],[dbo].[tblTransactionLineItems].[Number03],[dbo].[tblTransactionLineItems].[Number04],[dbo].[tblTransactionLineItems].[Number05],[dbo].[tblTransactionLineItems].[Number06],[dbo].[tblTransactionLineItems].[OdometerHours],[dbo].[tblTransactionLineItems].[EndDeliveryDate],[dbo].[tblTransactionLineItems].[RequestedDeliveryDate],[dbo].[tblTransactionLineItems].[InvoiceNumber],[dbo].[tblTransactionLineItems].[InvoiceLineNumber],[dbo].[tblTransactionLineItems].[AlternativeGrossVolume],[dbo].[tblTransactionLineItems].[AlternativeNetVolume],[dbo].[tblTransactionLineItems].[AlternativeUnits],[dbo].[tblTransactionLineItems].[TankLevel],[dbo].[tblTransactionLineItems].[TankLevelUnits],[dbo].[tblTransactionLineItems].[Date01],[dbo].[tblTransactionLineItems].[Date02],[dbo].[tblTransactionLineItems].[Date03],[dbo].[tblTransactionLineItems].[Date04],[dbo].[tblTransactionLineItems].[NonDomesticPrice],[dbo].[tblTransactionLineItems].[CurrencyUnit],[dbo].[tblTransactionLineItems].[ExchangeRate],[dbo].[tblTransactionLineItems].[QualityTestNumber],[dbo].[tblTransactionLineItems].[Odometer],[dbo].[tblTransactionLineItems].[DeliveryLocation],[dbo].[tblTransactionLineItems].[Variance],[dbo].[tblTransactionLineItems].[PartialFill],[dbo].[tblTransactionLineItems].[MassQuantity],[dbo].[tblTransactionLineItems].[NetManualValueFlag],[dbo].[tblTransactionLineItems].[MassManualValueFlag],[dbo].[tblTransactionLineItems].[GrossManualValueFlag],[dbo].[tblTransactionLineItems].[VcfManualValueFlag],[dbo].[tblTransactionLineItems].[DeliveredGrossManualValueFlag],[dbo].[tblTransactionLineItems].[DeliveredNetManualValueFlag],[dbo].[tblTransactionLineItems].[TransactionLineItemGuid],[dbo].[tblTransactionLineItems].[LookupTransactionStatusIndex],[dbo].[tblTransactionLineItems].[LookupQualityIndex],[dbo].[tblTransactionLineItems].[StorageLocationTankGuid],[dbo].[tblTransactionLineItems].[AdditiveProfileGuid],[dbo].[tblTransactionLineItems].[DestinationCompartmentEquipmentGuid],[dbo].[tblTransactionLineItems].[DestinationEquipmentGuid],[dbo].[tblTransactionLineItems].[OperatorPersonnelGuid],[dbo].[tblTransactionLineItems].[ProductGuid],[dbo].[tblTransactionLineItems].[SourceCompartmentEquipmentGuid],[dbo].[tblTransactionLineItems].[SourceEquipmentGuid],[dbo].[tblTransactionLineItems].[TransactionGuid],[dbo].[tblTransactionLineItems].[CurrencyGuid],[dbo].[tblTransactionLineItems].[OrderReferenceTransactionLineItemGuid],[dbo].[tblTransactionLineItems].[LoadingLocationStationGuid],[dbo].[tblTransactionLineItems].[MeterGuid],[dbo].[tblTransactionLineItems].[PackageManualValueFlag],[dbo].[tblTransactionLineItems].[CleanLineItem],[dbo].[tblTransactionLineItems].[CleanLineDeductItem],[dbo].[tblTransactionLineItems].[CleanLineDeductQuantity],[dbo].[tblTransactionLineItems].[CleanLinePackQuantity],[dbo].[tblTransactionLineItems].[DualFuelingModeFlag],[dbo].[tblTransactionLineItems].[DualFuelingPrimaryFlag],[dbo].[tblTransactionLineItems].[EngineRunTime],[dbo].[tblTransactionLineItems].[FlowRate],[dbo].[tblTransactionLineItems].[FuelCompressionFactor],[dbo].[tblTransactionLineItems].[HydrantPressure],[dbo].[tblTransactionLineItems].[MobileDeviceID],[dbo].[tblTransactionLineItems].[MobileDeviceGuid],[dbo].[tblTransactionLineItems].[TemperatureQualityStatus],[dbo].[tblTransactionLineItems].[MeterStartObtainedAutomaticallyFlag],[dbo].[tblTransactionLineItems].[MeterStopObtainedAutomaticallyFlag],[dbo].[tblTransactionLineItems].[NetVolumeIndicator]
                        ,CT.UpdatedRowVersion 'CT_UpdatedRowVersion'
                        ,CT.UpdatedContext 'CT_UpdatedContext'
                        ,CT.UpdatedDate 'CT_UpdatedDate'
                    FROM [dbo].[tblTransactionLineItems]
                        INNER JOIN [track].[tblTransactionLineItems] CT
                            ON CT.PK_TransactionLineItemGuid = [dbo].[tblTransactionLineItems].[TransactionLineItemGuid] 
                    WHERE CT.PK_TransactionLineItemGuid = @TransactionLineItemGuid
            ) MERGE existingData
            USING (SELECT @SequenceID,@MeterStart,@MeterStop,@GrossQuantity,@DeliveredGrossQuantity,@Temperature,@Vcf,@Density,@Product,@ProductCode,@ProductType,@ProductPrice,@CLIN,@NetQuantity,@DeliveredNetQuantity,@Pressure,@ContractNumber,@DestinationRegistrationID,@DestinationSerialNumber,@DestinationEquipmentType,@DestinationEquipmentModel,@DestinationCompanyEquipmentID,@DestinationCompartmentID,@SourceRegistrationID,@SourceSerialNumber,@SourceEquipmentType,@SourceEquipmentModel,@SourceCompanyEquipmentID,@SourceCompartmentID,@MeterFactor,@LineItemSequenceNumber,@BatchNumber,@DocumentNumber,@LineFill,@BottomVolume,@NetCapacity,@Customs,@ArmNumber,@LineNumber,@OperatorID,@TankStatus,@MeterStartDateTime,@MeterStopDateTime,@Pit,@RequestedDateTime,@DispatchedDateTime,@AcknowledgedDateTime,@OnLocationTime,@ValidationDateTime,@CompletionDateTime,@ReceiptVariance,@DifferentialPressure,@LoadRackVariance,@RequestedBy,@FreezePoint,@DeleteFlag,@StorageLocationID,@MeterID,@AdditiveProfileID,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@PresetAmount,@EngineeringUnitsIndex,@CustomerProductName,@CustomerProductCode,@TransactionInventoryDate,@COAWaiver,@COANote,@COAID,@Tax1,@Tax2,@Tax3,@Tax4,@Tax5,@TransVersion,@LoadingLocationID,@ImproperAdditization,@BrokenBlend,@ContaminatePrompt,@CompartmentsPreviouslyLoaded,@CompartmentsEmpty,@Flag01,@Flag02,@Flag03,@Flag04,@Flag05,@Flag06,@Number01,@Number02,@Number03,@Number04,@Number05,@Number06,@OdometerHours,@EndDeliveryDate,@RequestedDeliveryDate,@InvoiceNumber,@InvoiceLineNumber,@AlternativeGrossVolume,@AlternativeNetVolume,@AlternativeUnits,@TankLevel,@TankLevelUnits,@Date01,@Date02,@Date03,@Date04,@NonDomesticPrice,@CurrencyUnit,@ExchangeRate,@QualityTestNumber,@Odometer,@DeliveryLocation,@Variance,@PartialFill,@MassQuantity,@NetManualValueFlag,@MassManualValueFlag,@GrossManualValueFlag,@VcfManualValueFlag,@DeliveredGrossManualValueFlag,@DeliveredNetManualValueFlag ,@TransactionLineItemGuid,@LookupTransactionStatusIndex,@LookupQualityIndex,@StorageLocationTankGuid,@AdditiveProfileGuid,@DestinationCompartmentEquipmentGuid,@DestinationEquipmentGuid,@OperatorPersonnelGuid,@ProductGuid,@SourceCompartmentEquipmentGuid,@SourceEquipmentGuid,@TransactionGuid,@CurrencyGuid,@OrderReferenceTransactionLineItemGuid,@LoadingLocationStationGuid,@MeterGuid,@PackageManualValueFlag,@CleanLineItem,@CleanLineDeductItem,@CleanLineDeductQuantity,@CleanLinePackQuantity,@DualFuelingModeFlag,@DualFuelingPrimaryFlag,@EngineRunTime,@FlowRate,@FuelCompressionFactor,@HydrantPressure,@MobileDeviceID,@MobileDeviceGuid,@TemperatureQualityStatus,@MeterStartObtainedAutomaticallyFlag,@MeterStopObtainedAutomaticallyFlag,@NetVolumeIndicator
                    ) AS remoteChanges ([SequenceID],[MeterStart],[MeterStop],[GrossQuantity],[DeliveredGrossQuantity],[Temperature],[Vcf],[Density],[Product],[ProductCode],[ProductType],[ProductPrice],[CLIN],[NetQuantity],[DeliveredNetQuantity],[Pressure],[ContractNumber],[DestinationRegistrationID],[DestinationSerialNumber],[DestinationEquipmentType],[DestinationEquipmentModel],[DestinationCompanyEquipmentID],[DestinationCompartmentID],[SourceRegistrationID],[SourceSerialNumber],[SourceEquipmentType],[SourceEquipmentModel],[SourceCompanyEquipmentID],[SourceCompartmentID],[MeterFactor],[LineItemSequenceNumber],[BatchNumber],[DocumentNumber],[LineFill],[BottomVolume],[NetCapacity],[Customs],[ArmNumber],[LineNumber],[OperatorID],[TankStatus],[MeterStartDateTime],[MeterStopDateTime],[Pit],[RequestedDateTime],[DispatchedDateTime],[AcknowledgedDateTime],[OnLocationTime],[ValidationDateTime],[CompletionDateTime],[ReceiptVariance],[DifferentialPressure],[LoadRackVariance],[RequestedBy],[FreezePoint],[DeleteFlag],[StorageLocationID],[MeterID],[AdditiveProfileID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[PresetAmount],[EngineeringUnitsIndex],[CustomerProductName],[CustomerProductCode],[TransactionInventoryDate],[COAWaiver],[COANote],[COAID],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[LoadingLocationID],[ImproperAdditization],[BrokenBlend],[ContaminatePrompt],[CompartmentsPreviouslyLoaded],[CompartmentsEmpty],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[OdometerHours],[EndDeliveryDate],[RequestedDeliveryDate],[InvoiceNumber],[InvoiceLineNumber],[AlternativeGrossVolume],[AlternativeNetVolume],[AlternativeUnits],[TankLevel],[TankLevelUnits],[Date01],[Date02],[Date03],[Date04],[NonDomesticPrice],[CurrencyUnit],[ExchangeRate],[QualityTestNumber],[Odometer],[DeliveryLocation],[Variance],[PartialFill],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag], [TransactionLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[StorageLocationTankGuid],[AdditiveProfileGuid],[DestinationCompartmentEquipmentGuid],[DestinationEquipmentGuid],[OperatorPersonnelGuid],[ProductGuid],[SourceCompartmentEquipmentGuid],[SourceEquipmentGuid],[TransactionGuid],[CurrencyGuid],[OrderReferenceTransactionLineItemGuid],[LoadingLocationStationGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity],[DualFuelingModeFlag],[DualFuelingPrimaryFlag],[EngineRunTime],[FlowRate],[FuelCompressionFactor],[HydrantPressure],[MobileDeviceID],[MobileDeviceGuid],[TemperatureQualityStatus],[MeterStartObtainedAutomaticallyFlag],[MeterStopObtainedAutomaticallyFlag],[NetVolumeIndicator])
            ON (existingData.[TransactionLineItemGuid] = remoteChanges.[TransactionLineItemGuid])
            WHEN MATCHED AND (@sync_force_write = 1 
                            OR (existingData.CT_UpdatedRowVersion IS NULL) -- Record has never been changed.
                            OR (existingData.CT_UpdatedRowVersion IS NOT NULL AND existingData.CT_UpdatedRowVersion <= @sync_last_received_anchor_varbinary) -- it's been changed but not since our last sync session
                            OR (remoteChanges.UpdatedDate > existingData.CT_UpdatedDate AND (existingData.CT_UpdatedContext IS NULL OR existingData.CT_UpdatedContext <> @sync_client_id_binary)) -- incoming changes are newer than changes made locally or by another client via sync
                            OR (remoteChanges.UpdatedDate >= existingData.CT_UpdatedDate AND existingData.CT_UpdatedContext IS NOT NULL AND existingData.CT_UpdatedContext = @sync_client_id_binary)) -- (INTERNALLY, THE SERVER ID HAS BEEN SWAPPED IN FOR THE CLIENT ID), IF THE SERVER WAS THE LAST ONE THAT UPDATED IT, IT CAN REPLACE IT.
                THEN
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
                INSERT ([SequenceID],[MeterStart],[MeterStop],[GrossQuantity],[DeliveredGrossQuantity],[Temperature],[Vcf],[Density],[Product],[ProductCode],[ProductType],[ProductPrice],[CLIN],[NetQuantity],[DeliveredNetQuantity],[Pressure],[ContractNumber],[DestinationRegistrationID],[DestinationSerialNumber],[DestinationEquipmentType],[DestinationEquipmentModel],[DestinationCompanyEquipmentID],[DestinationCompartmentID],[SourceRegistrationID],[SourceSerialNumber],[SourceEquipmentType],[SourceEquipmentModel],[SourceCompanyEquipmentID],[SourceCompartmentID],[MeterFactor],[LineItemSequenceNumber],[BatchNumber],[DocumentNumber],[LineFill],[BottomVolume],[NetCapacity],[Customs],[ArmNumber],[LineNumber],[OperatorID],[TankStatus],[MeterStartDateTime],[MeterStopDateTime],[Pit],[RequestedDateTime],[DispatchedDateTime],[AcknowledgedDateTime],[OnLocationTime],[ValidationDateTime],[CompletionDateTime],[ReceiptVariance],[DifferentialPressure],[LoadRackVariance],[RequestedBy],[FreezePoint],[DeleteFlag],[StorageLocationID],[MeterID],[AdditiveProfileID],[CreatedBy],[CreatedDate],[UpdatedBy],[UpdatedDate],[PresetAmount],[EngineeringUnitsIndex],[CustomerProductName],[CustomerProductCode],[TransactionInventoryDate],[COAWaiver],[COANote],[COAID],[Tax1],[Tax2],[Tax3],[Tax4],[Tax5],[TransVersion],[LoadingLocationID],[ImproperAdditization],[BrokenBlend],[ContaminatePrompt],[CompartmentsPreviouslyLoaded],[CompartmentsEmpty],[Flag01],[Flag02],[Flag03],[Flag04],[Flag05],[Flag06],[Number01],[Number02],[Number03],[Number04],[Number05],[Number06],[OdometerHours],[EndDeliveryDate],[RequestedDeliveryDate],[InvoiceNumber],[InvoiceLineNumber],[AlternativeGrossVolume],[AlternativeNetVolume],[AlternativeUnits],[TankLevel],[TankLevelUnits],[Date01],[Date02],[Date03],[Date04],[NonDomesticPrice],[CurrencyUnit],[ExchangeRate],[QualityTestNumber],[Odometer],[DeliveryLocation],[Variance],[PartialFill],[MassQuantity],[NetManualValueFlag],[MassManualValueFlag],[GrossManualValueFlag],[VcfManualValueFlag],[DeliveredGrossManualValueFlag],[DeliveredNetManualValueFlag], [TransactionLineItemGuid],[LookupTransactionStatusIndex],[LookupQualityIndex],[StorageLocationTankGuid],[AdditiveProfileGuid],[DestinationCompartmentEquipmentGuid],[DestinationEquipmentGuid],[OperatorPersonnelGuid],[ProductGuid],[SourceCompartmentEquipmentGuid],[SourceEquipmentGuid],[TransactionGuid],[CurrencyGuid],[OrderReferenceTransactionLineItemGuid],[LoadingLocationStationGuid],[MeterGuid],[PackageManualValueFlag],[CleanLineItem],[CleanLineDeductItem],[CleanLineDeductQuantity],[CleanLinePackQuantity],[DualFuelingModeFlag],[DualFuelingPrimaryFlag],[EngineRunTime],[FlowRate],[FuelCompressionFactor],[HydrantPressure],[MobileDeviceID],[MobileDeviceGuid],[TemperatureQualityStatus],[MeterStartObtainedAutomaticallyFlag],[MeterStopObtainedAutomaticallyFlag],[NetVolumeIndicator])
                    VALUES (@SequenceID,@MeterStart,@MeterStop,@GrossQuantity,@DeliveredGrossQuantity,@Temperature,@Vcf,@Density,@Product,@ProductCode,@ProductType,@ProductPrice,@CLIN,@NetQuantity,@DeliveredNetQuantity,@Pressure,@ContractNumber,@DestinationRegistrationID,@DestinationSerialNumber,@DestinationEquipmentType,@DestinationEquipmentModel,@DestinationCompanyEquipmentID,@DestinationCompartmentID,@SourceRegistrationID,@SourceSerialNumber,@SourceEquipmentType,@SourceEquipmentModel,@SourceCompanyEquipmentID,@SourceCompartmentID,@MeterFactor,@LineItemSequenceNumber,@BatchNumber,@DocumentNumber,@LineFill,@BottomVolume,@NetCapacity,@Customs,@ArmNumber,@LineNumber,@OperatorID,@TankStatus,@MeterStartDateTime,@MeterStopDateTime,@Pit,@RequestedDateTime,@DispatchedDateTime,@AcknowledgedDateTime,@OnLocationTime,@ValidationDateTime,@CompletionDateTime,@ReceiptVariance,@DifferentialPressure,@LoadRackVariance,@RequestedBy,@FreezePoint,@DeleteFlag,@StorageLocationID,@MeterID,@AdditiveProfileID,@CreatedBy,@CreatedDate,@UpdatedBy,@UpdatedDate,@PresetAmount,@EngineeringUnitsIndex,@CustomerProductName,@CustomerProductCode,@TransactionInventoryDate,@COAWaiver,@COANote,@COAID,@Tax1,@Tax2,@Tax3,@Tax4,@Tax5,@TransVersion,@LoadingLocationID,@ImproperAdditization,@BrokenBlend,@ContaminatePrompt,@CompartmentsPreviouslyLoaded,@CompartmentsEmpty,@Flag01,@Flag02,@Flag03,@Flag04,@Flag05,@Flag06,@Number01,@Number02,@Number03,@Number04,@Number05,@Number06,@OdometerHours,@EndDeliveryDate,@RequestedDeliveryDate,@InvoiceNumber,@InvoiceLineNumber,@AlternativeGrossVolume,@AlternativeNetVolume,@AlternativeUnits,@TankLevel,@TankLevelUnits,@Date01,@Date02,@Date03,@Date04,@NonDomesticPrice,@CurrencyUnit,@ExchangeRate,@QualityTestNumber,@Odometer,@DeliveryLocation,@Variance,@PartialFill,@MassQuantity,@NetManualValueFlag,@MassManualValueFlag,@GrossManualValueFlag,@VcfManualValueFlag,@DeliveredGrossManualValueFlag,@DeliveredNetManualValueFlag ,@TransactionLineItemGuid,@LookupTransactionStatusIndex,@LookupQualityIndex,@StorageLocationTankGuid,@AdditiveProfileGuid,@DestinationCompartmentEquipmentGuid,@DestinationEquipmentGuid,@OperatorPersonnelGuid,@ProductGuid,@SourceCompartmentEquipmentGuid,@SourceEquipmentGuid,@TransactionGuid,@CurrencyGuid,@OrderReferenceTransactionLineItemGuid,@LoadingLocationStationGuid,@MeterGuid,@PackageManualValueFlag,@CleanLineItem,@CleanLineDeductItem,@CleanLineDeductQuantity,@CleanLinePackQuantity,@DualFuelingModeFlag,@DualFuelingPrimaryFlag,@EngineRunTime,@FlowRate,@FuelCompressionFactor,@HydrantPressure,@MobileDeviceID,@MobileDeviceGuid,@TemperatureQualityStatus,@MeterStartObtainedAutomaticallyFlag,@MeterStopObtainedAutomaticallyFlag,@NetVolumeIndicator)
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
        RAISERROR(N'(CU)Time between synchronization has exceeded the maximum amount of time for table ''%s'' (MIN: %I64d, LAST: %I64d).  To avoid data corruption and old data, the client must reinitialize its local database with a current refresh from the server.', 16, 3, @sync_table_name, @minValidVersion, @sync_last_received_anchor)
END
