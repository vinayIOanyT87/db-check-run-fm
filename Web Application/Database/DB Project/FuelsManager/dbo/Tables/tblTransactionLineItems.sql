CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItems_LedgerCoveringIndex] ON [dbo].[tblTransactionLineItems]
(
	[TransactionGuid] ASC,
	[SequenceID] ASC
)
INCLUDE ( 	[LookupQualityIndex],
	[GrossQuantity],
	[ProductPrice],
	[ProductGuid],
	[StorageLocationTankGuid],
	[UpdatedDate],
	[NetQuantity],
	[MassQuantity],
	[Number01],
	[Number02],
	[Number03],
	[Number04],
	[Number05],
	[Number06]) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO


CREATE TABLE [dbo].[tblTransactionLineItems] (
    [SequenceID]                            SMALLINT           NULL,
    [MeterStart]                            FLOAT (53)         NULL,
    [MeterStop]                             FLOAT (53)         NULL,
    [GrossQuantity]                         FLOAT (53)         NULL,
	 [DeliveredGrossQuantity]                FLOAT (53)         NULL,
    [Temperature]                           FLOAT (53)         NULL,
    [Vcf]                                   FLOAT (53)         NULL,
    [Density]                               FLOAT (53)         NULL,
    [Product]                               NVARCHAR (30)      NULL,
    [ProductCode]                           NVARCHAR (30)      NULL,
    [ProductType]                           NVARCHAR (20)      NULL,
    [ProductPrice]                          FLOAT (53)         NULL,
    [CLIN]                                  NVARCHAR (10)      NULL,
    [NetQuantity]                           FLOAT (53)         NULL,
	 [DeliveredNetQuantity]						  FLOAT (53)         NULL,
	 [Pressure]                   FLOAT (53)         NULL,	 
    [ContractNumber]                        NVARCHAR (30)      NULL,
    [DestinationRegistrationID]             NVARCHAR (30)      NULL,
    [DestinationSerialNumber]               NVARCHAR (10)      NULL,
    [DestinationEquipmentType]              NVARCHAR (50)      NULL,
    [DestinationEquipmentModel]             NVARCHAR (20)      NULL,
    [DestinationCompanyEquipmentID]         NVARCHAR (30)      NULL,
    [DestinationCompartmentID]              NVARCHAR (50)      NULL,
    [SourceRegistrationID]                  NVARCHAR (30)      NULL,
    [SourceSerialNumber]                    NVARCHAR (10)      NULL,
    [SourceEquipmentType]                   NVARCHAR (50)      NULL,
    [SourceEquipmentModel]                  NVARCHAR (20)      NULL,
    [SourceCompanyEquipmentID]              NVARCHAR (30)      NULL,
    [SourceCompartmentID]                   NVARCHAR (50)      NULL,
    [MeterFactor]                           FLOAT (53)         NULL,
    [LineItemSequenceNumber]                NVARCHAR (5)       NULL,
    [BatchNumber]                           NVARCHAR (20)      NULL,
    [DocumentNumber]                        NVARCHAR (30)      NULL,
    [LineFill]                              FLOAT (53)         NULL,
    [BottomVolume]                          FLOAT (53)         NULL,
    [NetCapacity]                           FLOAT (53)         NULL,
    [Customs]                               NVARCHAR (20)      NULL,
    [ArmNumber]                             INT                NULL,
    [LineNumber]                            INT                NULL,
    [OperatorID]                            NVARCHAR (50)      NULL,
    [TankStatus]                            NVARCHAR (30)      NULL,
    [MeterStartDateTime]                    DATETIMEOFFSET (7) NULL,
    [MeterStopDateTime]                     DATETIMEOFFSET (7) NULL,
    [Pit]                                   NVARCHAR (10)      NULL,
    [RequestedDateTime]                     DATETIMEOFFSET (7) NULL,
    [DispatchedDateTime]                    DATETIMEOFFSET (7) NULL,
    [AcknowledgedDateTime]                  DATETIMEOFFSET (7) NULL,
    [OnLocationTime]                        DATETIMEOFFSET (7) NULL,
    [ValidationDateTime]                    DATETIMEOFFSET (7) NULL,
    [CompletionDateTime]                    DATETIMEOFFSET (7) NULL,
    [ReceiptVariance]                       FLOAT (53)         NULL,
    [DifferentialPressure]                  FLOAT (53)         NULL,
    [LoadRackVariance]                      FLOAT (53)         NULL,
    [RequestedBy]                           NVARCHAR (50)      NULL,
    [FreezePoint]                           FLOAT (53)         NULL,
    [DeleteFlag]                            BIT                NULL,
    [StorageLocationID]                     NVARCHAR (50)      NULL,
    [MeterID]                               NVARCHAR (50)      NULL,
    [AdditiveProfileID]                     NVARCHAR (50)      NULL,
    [CreatedBy]                             [dbo].[udtUserID]  NULL,
    [CreatedDate]                           DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                             [dbo].[udtUserID]  NULL,
    [UpdatedDate]                           DATETIMEOFFSET (7) NULL,
    [PresetAmount]                          FLOAT (53)         NULL,
    [EngineeringUnitsIndex]                 INT                NULL,
    [CustomerProductName]                   NVARCHAR (50)      NULL,
    [CustomerProductCode]                   NVARCHAR (20)      NULL,
    [TransactionInventoryDate]              DATE               NULL,
    [COAWaiver]                             BIT                NULL,
    [COANote]                               NVARCHAR (50)      NULL,
    [COAID]                                 NVARCHAR (40)      NULL,
    [Tax1]                                  FLOAT (53)         NULL,
    [Tax2]                                  FLOAT (53)         NULL,
    [Tax3]                                  FLOAT (53)         NULL,
    [Tax4]                                  FLOAT (53)         NULL,
    [Tax5]                                  FLOAT (53)         NULL,
    [TransVersion]                          BIGINT             NULL,
    [LoadingLocationID]                     NVARCHAR (30)      NULL,
    [ImproperAdditization]                  BIT                NULL,
    [BrokenBlend]                           BIT                NULL,
    [ContaminatePrompt]                     BIT                NULL,
    [CompartmentsPreviouslyLoaded]          BIT                NULL,
    [CompartmentsEmpty]                     BIT                NULL,
    [Flag01]                                BIT                NULL,
    [Flag02]                                BIT                NULL,
    [Flag03]                                BIT                NULL,
    [Flag04]                                BIT                NULL,
    [Flag05]                                BIT                NULL,
    [Flag06]                                BIT                NULL,
    [Number01]                              FLOAT (53)         NULL,
    [Number02]                              FLOAT (53)         NULL,
    [Number03]                              FLOAT (53)         NULL,
    [Number04]                              FLOAT (53)         NULL,
    [Number05]                              FLOAT (53)         NULL,
    [Number06]                              FLOAT (53)         NULL,
    [OdometerHours]                         FLOAT (53)         NULL,
    [EndDeliveryDate]                       DATETIMEOFFSET (7) NULL,
    [RequestedDeliveryDate]                 DATETIMEOFFSET (7) NULL,
    [InvoiceNumber]                         NVARCHAR (50)      NULL,
    [InvoiceLineNumber]                     NVARCHAR (50)      NULL,
    [AlternativeGrossVolume]                FLOAT (53)         NULL,
    [AlternativeNetVolume]                  FLOAT (53)         NULL,
    [AlternativeUnits]                      INT                NULL,
    [TankLevel]                             FLOAT (53)         NULL,
    [TankLevelUnits]                        INT                NULL,
    [Date01]                                DATETIMEOFFSET (7) NULL,
    [Date02]                                DATETIMEOFFSET (7) NULL,
    [Date03]                                DATETIMEOFFSET (7) NULL,
    [Date04]                                DATETIMEOFFSET (7) NULL,
    [NonDomesticPrice]                      FLOAT (53)         NULL,
    [CurrencyUnit]                          INT                NULL,
    [ExchangeRate]                          FLOAT (53)         NULL,
    [QualityTestNumber]                     NVARCHAR (50)      NULL,
    [Odometer]                              FLOAT (53)         NULL,
    [DeliveryLocation]                      NVARCHAR (50)      NULL,
    [Variance]                              FLOAT (53)         NULL,
    [PartialFill]                           BIT                NULL,
    [MassQuantity]                          FLOAT (53)         NULL,
    [NetManualValueFlag]                    BIT                NULL,
    [MassManualValueFlag]                   BIT                NULL,
    [GrossManualValueFlag]                  BIT                NULL,
    [VcfManualValueFlag]                    BIT                NULL,
	 [DeliveredGrossManualValueFlag]         BIT                NULL,
	 [DeliveredNetManualValueFlag]           BIT                NULL,
    [TransactionLineItemGuid]               UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionLineItems_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                           ROWVERSION         NOT NULL,
    [LookupTransactionStatusIndex]          INT                NULL,
    [LookupQualityIndex]                    INT                CONSTRAINT [DF_tblTransactionLineItems_LookupQualityIndex] DEFAULT ((1)) NOT NULL,
    [StorageLocationTankGuid]               UNIQUEIDENTIFIER   NULL,
    [AdditiveProfileGuid]                   UNIQUEIDENTIFIER   NULL,
    [DestinationCompartmentEquipmentGuid]   UNIQUEIDENTIFIER   NULL,
    [DestinationEquipmentGuid]              UNIQUEIDENTIFIER   NULL,
    [OperatorPersonnelGuid]                 UNIQUEIDENTIFIER   NULL,
    [ProductGuid]                           UNIQUEIDENTIFIER   NULL,
    [SourceCompartmentEquipmentGuid]        UNIQUEIDENTIFIER   NULL,
    [SourceEquipmentGuid]                   UNIQUEIDENTIFIER   NULL,
    [TransactionGuid]                       UNIQUEIDENTIFIER   NOT NULL,
    [CurrencyGuid]                          UNIQUEIDENTIFIER   NULL,
    [OrderReferenceTransactionLineItemGuid] UNIQUEIDENTIFIER   NULL,
    [LoadingLocationStationGuid]            UNIQUEIDENTIFIER   NULL,
    [MeterGuid]                             UNIQUEIDENTIFIER   NULL,
    [PackageManualValueFlag]                BIT                NULL,
    [CleanLineItem]                         BIT                NULL,
    [CleanLineDeductItem]                   BIT                NULL,
    [CleanLineDeductQuantity]               FLOAT (53)         NULL,
    [CleanLinePackQuantity]                 FLOAT (53)         NULL,
    [DualFuelingModeFlag]                   BIT                NULL,
    [DualFuelingPrimaryFlag]                BIT                NULL,
    [EngineRunTime]                         FLOAT (53)         NULL,
    [FlowRate]                              FLOAT (53)         NULL,
    [FuelCompressionFactor]                 FLOAT (53)         NULL,
    [HydrantPressure]                       FLOAT (53)         NULL,
    [MobileDeviceID]                        NVARCHAR (50)      NULL,
    [MobileDeviceGuid]                      UNIQUEIDENTIFIER   NULL,
    [TemperatureQualityStatus]              NVARCHAR (50)      NULL,
    [MeterStartObtainedAutomaticallyFlag]   BIT                NULL,
    [MeterStopObtainedAutomaticallyFlag]    BIT                NULL,
	 [NetVolumeIndicator]						  BIT						NULL,	 
    [_ClusterIdx]                           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionLineItems_GUID] PRIMARY KEY NONCLUSTERED ([TransactionLineItemGuid] ASC),
    CONSTRAINT [FK_tblTransactionLineItems_LookupTransactionQualityIndex] FOREIGN KEY ([LookupQualityIndex]) REFERENCES [lookup].[tblTransactionQuality] ([TransactionQualityIndex]),
    CONSTRAINT [FK_tblTransactionLineItems_LookupTransactionStatusIndex] FOREIGN KEY ([LookupTransactionStatusIndex]) REFERENCES [lookup].[tblTransactionStatus] ([TransactionStatusIndex]),
    CONSTRAINT [FK_tblTransactionLineItems_OrderReferenceTransactionLineItemGuid] FOREIGN KEY ([OrderReferenceTransactionLineItemGuid]) REFERENCES [dbo].[tblTransactionLineItems] ([TransactionLineItemGuid]),
    CONSTRAINT [FK_tblTransactionLineItems_TransactionGuid] FOREIGN KEY ([TransactionGuid]) REFERENCES [dbo].[tblTransactions] ([TransactionGuid])
);

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionLineItems_ClusterIdx] 
	ON [dbo].[tblTransactionLineItems]([_ClusterIdx]);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItems_TransactionGuid_SequenceID]
    ON [dbo].[tblTransactionLineItems]([TransactionGuid] ASC, [SequenceID] ASC)
    INCLUDE([TransactionLineItemGuid]);


GO


CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactionLineItems_CoveringAssociatedTransactionQueries] ON [dbo].[tblTransactionLineItems]
(
	[TransactionLineItemGuid] ASC
)
INCLUDE ( 	[TransactionGuid],
	[GrossQuantity],
	[Product],
	[ProductPrice],
	[Tax1],
	[Tax2],
	[Tax3],
	[LookupTransactionStatusIndex],
	[LookupQualityIndex],
	[DeleteFlag]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionLineItems] ON [dbo].[tblTransactionLineItems] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItems','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'D'; -- For Deletes 
	SET @_AuditEventSequence= 1; 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID;

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblTransactionLineItems (
		[SequenceID]
	,	[MeterStart]
	,	[MeterStop]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[Temperature]
	,	[Vcf]
	,	[Density]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[ProductPrice]
	,	[CLIN]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,	[Pressure]	
	,	[ContractNumber]
	,	[DestinationRegistrationID]
	,	[DestinationSerialNumber]
	,	[DestinationEquipmentType]
	,	[DestinationEquipmentModel]
	,	[DestinationCompanyEquipmentID]
	,	[DestinationCompartmentID]
	,	[SourceRegistrationID]
	,	[SourceSerialNumber]
	,	[SourceEquipmentType]
	,	[SourceEquipmentModel]
	,	[SourceCompanyEquipmentID]
	,	[SourceCompartmentID]
	,	[MeterFactor]
	,	[LineItemSequenceNumber]
	,	[BatchNumber]
	,	[DocumentNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[OperatorID]
	,	[TankStatus]
	,	[MeterStartDateTime]
	,	[MeterStopDateTime]
	,	[Pit]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[AcknowledgedDateTime]
	,	[OnLocationTime]
	,	[ValidationDateTime]
	,	[CompletionDateTime]
	,	[ReceiptVariance]
	,	[DifferentialPressure]
	,	[LoadRackVariance]
	,	[RequestedBy]
	,	[FreezePoint]
	,	[DeleteFlag]
	,	[StorageLocationID]
	,	[MeterID]
	,	[AdditiveProfileID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[PresetAmount]
	,	[EngineeringUnitsIndex]
	,	[CustomerProductName]
	,	[CustomerProductCode]
	,	[TransactionInventoryDate]
	,	[COAWaiver]
	,	[COANote]
	,	[COAID]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[LoadingLocationID]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[ContaminatePrompt]
	,	[CompartmentsPreviouslyLoaded]
	,	[CompartmentsEmpty]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[OdometerHours]
	,	[EndDeliveryDate]
	,	[RequestedDeliveryDate]
	,	[InvoiceNumber]
	,	[InvoiceLineNumber]
	,	[AlternativeGrossVolume]
	,	[AlternativeNetVolume]
	,	[AlternativeUnits]
	,	[TankLevel]
	,	[TankLevelUnits]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[NonDomesticPrice]
	,	[CurrencyUnit]
	,	[ExchangeRate]
	,	[QualityTestNumber]
	,	[Odometer]
	,	[DeliveryLocation]
	,	[Variance]
	,	[PartialFill]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,	[DeliveredGrossManualValueFlag]
	,	[DeliveredNetManualValueFlag]
	,	[TransactionLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[StorageLocationTankGuid]
	,	[AdditiveProfileGuid]
	,	[DestinationCompartmentEquipmentGuid]
	,	[DestinationEquipmentGuid]
	,	[OperatorPersonnelGuid]
	,	[ProductGuid]
	,	[SourceCompartmentEquipmentGuid]
	,	[SourceEquipmentGuid]
	,	[TransactionGuid]
	,	[CurrencyGuid]
	,	[OrderReferenceTransactionLineItemGuid]
	,	[LoadingLocationStationGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
	,	[DualFuelingModeFlag]
	,	[DualFuelingPrimaryFlag]
	,	[EngineRunTime]
	,	[FlowRate]
	,	[FuelCompressionFactor]
	,	[HydrantPressure]
	,	[MobileDeviceID]
	,	[MobileDeviceGuid]
	,	[TemperatureQualityStatus]
	,	[MeterStartObtainedAutomaticallyFlag]
	,	[MeterStopObtainedAutomaticallyFlag]
	,	[NetVolumeIndicator]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		d.[SequenceID]
	,	d.[MeterStart]
	,	d.[MeterStop]
	,	d.[GrossQuantity]
	,  d.[DeliveredGrossQuantity]
	,	d.[Temperature]
	,	d.[Vcf]
	,	d.[Density]
	,	d.[Product]
	,	d.[ProductCode]
	,	d.[ProductType]
	,	d.[ProductPrice]
	,	d.[CLIN]
	,	d.[NetQuantity]
	,  d.[DeliveredNetQuantity]
	,  d.[Pressure]	
	,	d.[ContractNumber]
	,	d.[DestinationRegistrationID]
	,	d.[DestinationSerialNumber]
	,	d.[DestinationEquipmentType]
	,	d.[DestinationEquipmentModel]
	,	d.[DestinationCompanyEquipmentID]
	,	d.[DestinationCompartmentID]
	,	d.[SourceRegistrationID]
	,	d.[SourceSerialNumber]
	,	d.[SourceEquipmentType]
	,	d.[SourceEquipmentModel]
	,	d.[SourceCompanyEquipmentID]
	,	d.[SourceCompartmentID]
	,	d.[MeterFactor]
	,	d.[LineItemSequenceNumber]
	,	d.[BatchNumber]
	,	d.[DocumentNumber]
	,	d.[LineFill]
	,	d.[BottomVolume]
	,	d.[NetCapacity]
	,	d.[Customs]
	,	d.[ArmNumber]
	,	d.[LineNumber]
	,	d.[OperatorID]
	,	d.[TankStatus]
	,	d.[MeterStartDateTime]
	,	d.[MeterStopDateTime]
	,	d.[Pit]
	,	d.[RequestedDateTime]
	,	d.[DispatchedDateTime]
	,	d.[AcknowledgedDateTime]
	,	d.[OnLocationTime]
	,	d.[ValidationDateTime]
	,	d.[CompletionDateTime]
	,	d.[ReceiptVariance]
	,	d.[DifferentialPressure]
	,	d.[LoadRackVariance]
	,	d.[RequestedBy]
	,	d.[FreezePoint]
	,	d.[DeleteFlag]
	,	d.[StorageLocationID]
	,	d.[MeterID]
	,	d.[AdditiveProfileID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[PresetAmount]
	,	d.[EngineeringUnitsIndex]
	,	d.[CustomerProductName]
	,	d.[CustomerProductCode]
	,	d.[TransactionInventoryDate]
	,	d.[COAWaiver]
	,	d.[COANote]
	,	d.[COAID]
	,	d.[Tax1]
	,	d.[Tax2]
	,	d.[Tax3]
	,	d.[Tax4]
	,	d.[Tax5]
	,	d.[TransVersion]
	,	d.[LoadingLocationID]
	,	d.[ImproperAdditization]
	,	d.[BrokenBlend]
	,	d.[ContaminatePrompt]
	,	d.[CompartmentsPreviouslyLoaded]
	,	d.[CompartmentsEmpty]
	,	d.[Flag01]
	,	d.[Flag02]
	,	d.[Flag03]
	,	d.[Flag04]
	,	d.[Flag05]
	,	d.[Flag06]
	,	d.[Number01]
	,	d.[Number02]
	,	d.[Number03]
	,	d.[Number04]
	,	d.[Number05]
	,	d.[Number06]
	,	d.[OdometerHours]
	,	d.[EndDeliveryDate]
	,	d.[RequestedDeliveryDate]
	,	d.[InvoiceNumber]
	,	d.[InvoiceLineNumber]
	,	d.[AlternativeGrossVolume]
	,	d.[AlternativeNetVolume]
	,	d.[AlternativeUnits]
	,	d.[TankLevel]
	,	d.[TankLevelUnits]
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[NonDomesticPrice]
	,	d.[CurrencyUnit]
	,	d.[ExchangeRate]
	,	d.[QualityTestNumber]
	,	d.[Odometer]
	,	d.[DeliveryLocation]
	,	d.[Variance]
	,	d.[PartialFill]
	,	d.[MassQuantity]
	,	d.[NetManualValueFlag]
	,	d.[MassManualValueFlag]
	,	d.[GrossManualValueFlag]
	,	d.[VcfManualValueFlag]
	,	d.[DeliveredGrossManualValueFlag]
	,	d.[DeliveredNetManualValueFlag]
	,	d.[TransactionLineItemGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupQualityIndex]
	,	d.[StorageLocationTankGuid]
	,	d.[AdditiveProfileGuid]
	,	d.[DestinationCompartmentEquipmentGuid]
	,	d.[DestinationEquipmentGuid]
	,	d.[OperatorPersonnelGuid]
	,	d.[ProductGuid]
	,	d.[SourceCompartmentEquipmentGuid]
	,	d.[SourceEquipmentGuid]
	,	d.[TransactionGuid]
	,	d.[CurrencyGuid]
	,	d.[OrderReferenceTransactionLineItemGuid]
	,	d.[LoadingLocationStationGuid]
	,	d.[MeterGuid]
	,	d.[PackageManualValueFlag]
	,	d.[CleanLineItem]
	,	d.[CleanLineDeductItem]
	,	d.[CleanLineDeductQuantity]
	,	d.[CleanLinePackQuantity]
	,	d.[DualFuelingModeFlag]
	,	d.[DualFuelingPrimaryFlag]
	,	d.[EngineRunTime]
	,	d.[FlowRate]
	,	d.[FuelCompressionFactor]
	,	d.[HydrantPressure]
	,	d.[MobileDeviceID]
	,	d.[MobileDeviceGuid]
	,	d.[TemperatureQualityStatus]
	,	d.[MeterStartObtainedAutomaticallyFlag]
	,	d.[MeterStopObtainedAutomaticallyFlag]
	,	d.[NetVolumeIndicator]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
END

GO

/*
	Updates the equipment volume based on line item information.
	This trigger is based on the approach used in defense (8.0.4.17-205 WI-26142 ALTER_TR_tblTransactionLineItems_IU_UpdatedEquipment.sql)
*/
CREATE TRIGGER [dbo].[trg_tblTransactionLineItems_IU_UpdateEquipmentVolume]
    ON [dbo].[tblTransactionLineItems]
    AFTER INSERT, UPDATE
    AS BEGIN
	SET NOCOUNT ON

	DECLARE @LookupTransactionStatusIndex INT, 
		@LookupTransTypeIndex SMALLINT,
		@PartialFill BIT, 
		@ReversalType NVARCHAR (2),
		@NetQuantity FLOAT, 
		@DeleteFlag BIT,
		@EquipmentGuid UNIQUEIDENTIFIER,
		@SafeFill FLOAT,
		@Volume FLOAT,
		@Variance FLOAT,
		@Tolerance FLOAT,
		@DestinationEquipmentGuid UNIQUEIDENTIFIER,
		@SourceEquipmentGuid UNIQUEIDENTIFIER,
		@TransactionLineItemGuid UNIQUEIDENTIFIER,
		@OldTransactionStatus INT,
		@IsSecondaryStorage BIT,
		@FillMethod AS TINYINT,
		@VarianceTolerance FLOAT

	DECLARE Inserted_Cursor CURSOR FAST_FORWARD FOR 
		SELECT 
			inserted.TransactionLineItemGuid,
			inserted.DestinationEquipmentGuid, 
			inserted.SourceEquipmentGuid,  
			T.LookupTransTypeIndex, 
			inserted.LookupTransactionStatusIndex, 		 
			inserted.NetQuantity, 
			T.ReversalType, 
			inserted.PartialFill, 
			inserted.DeleteFlag,		
			ISNULL(deleted.LookupTransactionStatusIndex, -99),
			tblSites.LookupSecondaryStorageFillMethodIndex,
			tblSites.ExcessVarianceTolerance		
		FROM inserted 
		LEFT JOIN tblTransactions T ON inserted.TransactionGuid = T.TransactionGuid
		LEFT JOIN tblSites ON T.SiteGuid = tblSites.SiteGuid
		LEFT JOIN deleted ON inserted.TransactionLineItemGuid = deleted.TransactionLineItemGuid 
		WHERE (inserted.LookupTransactionStatusIndex = 0 OR inserted.LookupTransactionStatusIndex = 11) -- Completed or Posted only
			-- Only process records where the previous status value was dispatched, arrived, started, or stopped, or where there was no previous status
			AND (deleted.LookupTransactionStatusIndex IS NULL OR deleted.LookupTransactionStatusIndex IN (2, 12, 13, 14)) -- 2 = dispatched, 12 = arrived, 13 = started,  14 = stopped
			AND inserted.NetQuantity IS NOT NULL AND inserted.NetQuantity <> 0.0
			AND (inserted.DeleteFlag IS NULL OR inserted.DeleteFlag = 0)
			AND (((LookupTransTypeIndex = 4 OR LookupTransTypeIndex = 7) AND inserted.DestinationEquipmentGuid IS NOT NULL) -- T4_SecondaryDefuel or T7_FillStand
				OR ((LookupTransTypeIndex = 6 OR LookupTransTypeIndex = 10) AND inserted.SourceEquipmentGuid IS NOT NULL)-- T6_SecondaryDisbursement or T10_Unload
				OR (LookupTransTypeIndex = 12 AND (inserted.DestinationEquipmentGuid IS NOT NULL OR inserted.SourceEquipmentGuid IS NOT NULL))) -- T12_InventoryNotAffected
	
	OPEN  Inserted_Cursor

	FETCH NEXT 
	FROM Inserted_Cursor 
	INTO @TransactionLineItemGuid, 
		@DestinationEquipmentGuid, 
		@SourceEquipmentGuid, 
		@LookupTransTypeIndex, 
		@LookupTransactionStatusIndex, 
		@NetQuantity, 
		@ReversalType,
		@PartialFill, 	
		@DeleteFlag, 
		@OldTransactionStatus,
		@FillMethod,
		@VarianceTolerance
	
	WHILE @@FETCH_STATUS = 0 
	BEGIN 
		-- Determine if this update/insert should affect the inventory
		IF (@LookupTransactionStatusIndex NOT IN (0, 11) -- only want to update quantities if the transaction is going to a completed status or posted	
			OR (@OldTransactionStatus NOT IN -- Do not process for transaction if status is null or if previous status is not null and not in dispatched, arrived, started, or stopped
				(	-99, -- did not exist before or was null
					2, -- dispatched
					12, -- arrived
					13, -- started
					14 -- stopped
				)			 
			) 		
		)
		BEGIN
			FETCH NEXT FROM Inserted_Cursor 
			INTO @TransactionLineItemGuid, 
				@DestinationEquipmentGuid, 
				@SourceEquipmentGuid, 
				@LookupTransTypeIndex, 
				@LookupTransactionStatusIndex, 
				@NetQuantity, 
				@ReversalType,
				@PartialFill, 	
				@DeleteFlag, 
				@OldTransactionStatus,
				@FillMethod,
				@VarianceTolerance
			CONTINUE
		END	

		IF @LookupTransTypeIndex = 4 OR @LookupTransTypeIndex = 7 -- T4_SecondaryDefuel OR T7_FillStand
		BEGIN
			SET @EquipmentGuid = @DestinationEquipmentGuid
		END
		ELSE IF @LookupTransTypeIndex = 6 OR @LookupTransTypeIndex = 10 -- T6_SecondaryDisbursement OR T10_Unload 
		BEGIN
			SET @EquipmentGuid = @SourceEquipmentGuid
		END
		ELSE IF @LookupTransTypeIndex = 12 -- T12_InventoryNotAffected
		BEGIN
			SET @EquipmentGuid = CASE WHEN @DestinationEquipmentGuid IS NULL THEN @SourceEquipmentGuid ELSE @DestinationEquipmentGuid END
		END

		IF @EquipmentGuid IS NOT NULL
		BEGIN
			SELECT @SafeFill = SafeFill,
				@Volume = Volume,
				@IsSecondaryStorage = ISNULL(SecondaryStorageFlag, 0)
			FROM tblEquipment 
			WHERE EquipmentGuid = @EquipmentGuid

			-- We only update the equipment record if it is secondary storage
			IF(@IsSecondaryStorage = 0)
			BEGIN
				FETCH NEXT 
				FROM Inserted_Cursor 
				INTO @TransactionLineItemGuid, 
					@DestinationEquipmentGuid, 
					@SourceEquipmentGuid, 
					@LookupTransTypeIndex, 
					@LookupTransactionStatusIndex, 
					@NetQuantity, 
					@ReversalType,
					@PartialFill, 	
					@DeleteFlag, 
					@OldTransactionStatus,
					@FillMethod,
					@VarianceTolerance

				CONTINUE
			END

			DECLARE @ComputedVariance BIT 
			SET @ComputedVariance = 0

			IF @LookupTransTypeIndex = 7 -- T7_FillStand
				AND (@PartialFill IS NULL OR @PartialFill = 0)
				AND @SafeFill IS NOT NULL AND @SafeFill <> 0.0
				AND (@ReversalType IS NULL OR @ReversalType = '')
				AND (@DeleteFlag IS NULL OR @DeleteFlag = 0)
			BEGIN
				IF (@Volume + @NetQuantity) > @SafeFill
				BEGIN
					SET @Variance = @SafeFill - (@Volume + @NetQuantity)
				END
				ELSE
				BEGIN
					SET @Variance = (@SafeFill - @Volume) - @NetQuantity
				END
					
				SET @Tolerance = @Variance / @SafeFill

				SET @ComputedVariance = 1

				UPDATE tblTransactionLineItems 
				SET Variance = @Variance 
				WHERE TransactionLineItemGuid = @TransactionLineItemGuid	
			END
			ELSE IF (@LookupTransTypeIndex = 10 -- T10_Unload aka Return to Bulk
				AND (@PartialFill IS NULL OR @PartialFill = 0)
				AND (@ReversalType IS NULL OR @ReversalType = '')
				AND (@DeleteFlag IS NULL OR @DeleteFlag = 0)
				AND @Volume IS NOT NULL AND @Volume <> 0)
			BEGIN
				SET @Variance = @NetQuantity - @Volume
								
				SET @Tolerance = @Variance / @Volume
	
				SET @ComputedVariance = 1

				UPDATE tblTransactionLineItems 
				SET Variance = @Variance 
				WHERE TransactionLineItemGuid = @TransactionLineItemGuid 	
			END

			UPDATE tblEquipment 
			SET Volume = 
					CASE 
						WHEN @LookupTransTypeIndex = 7 AND @FillMethod = 0 AND (@PartialFill IS NULL OR @PartialFill = 0) THEN @SafeFill
						WHEN @LookupTransTypeIndex = 10
							AND ((@FillMethod = 0 AND (@PartialFill IS NULL OR @PartialFill = 0)) OR (@Volume - @NetQuantity) < 0) THEN 0
						WHEN @LookupTransTypeIndex = 10 THEN Volume - @NetQuantity
						ELSE Volume + @NetQuantity
					END,
				Consecutive_OOS_Variance = 
					CASE 
						WHEN @ComputedVariance <> 1 THEN Consecutive_OOS_Variance
						WHEN ABS(@Tolerance) <= @VarianceTolerance / 100 THEN 0
						WHEN @Tolerance > 0 AND Consecutive_OOS_Variance < 0 THEN 1 
						WHEN @Tolerance > 0 AND ISNULL(Consecutive_OOS_Variance, 0) >= 0 THEN ISNULL(Consecutive_OOS_Variance, 0) + 1
						WHEN @Tolerance <= 0 AND Consecutive_OOS_Variance > 0 THEN -1
						WHEN @Tolerance <= 0 AND ISNULL(Consecutive_OOS_Variance, 0) <= 0 THEN ISNULL(Consecutive_OOS_Variance, 0) - 1
					END,
				UpdatedDate = SYSDATETIMEOFFSET()
			WHERE EquipmentGuid = @EquipmentGuid	
		END	

		FETCH NEXT 
		FROM Inserted_Cursor 
		INTO @TransactionLineItemGuid, 
			@DestinationEquipmentGuid, 
			@SourceEquipmentGuid, 
			@LookupTransTypeIndex, 
			@LookupTransactionStatusIndex, 
			@NetQuantity, 
			@ReversalType,
			@PartialFill, 	
			@DeleteFlag, 
			@OldTransactionStatus,
			@FillMethod,
			@VarianceTolerance
	
	END

	CLOSE Inserted_Cursor
	DEALLOCATE Inserted_Cursor;
END


GO
--Creating Insert / Update Trigger for tblTransactionLineItems
CREATE TRIGGER dbo.trg_insupd_tblTransactionLineItems_ForSync 
   ON dbo.tblTransactionLineItems
   AFTER INSERT, UPDATE 
AS 
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
 
    DECLARE @changeContextName nvarchar(100); 
    DECLARE @bypassTrackingFlags int; 
    DECLARE @bypassReason nvarchar(512); 
 
    SELECT @changeContextName = ContextName 
            ,@bypassTrackingFlags = BypassTrackingFlags 
            ,@bypassReason = BypassReason 
        FROM [track].[udf_GetChangeTrackingSessionDetails](); 
 
	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application insert or update.
    DECLARE @syncContext varbinary(128); 
    DECLARE @currentDateTimeOffset datetimeoffset(7); 
 
    SET @currentDateTimeOffset = sysdatetimeoffset(); 
 
   IF (([track].[udf_IsInsertChangeTrackingEnabled](@bypassTrackingFlags) = 1) OR ([track].[udf_IsUpdateChangeTrackingEnabled](@bypassTrackingFlags) = 1))
   BEGIN 
       SET @syncContext = dbo.udf_GetSyncContext(); 
 
       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
       -- Treat the change as a local change so it can be synchronized back to the remote system. 
       IF ((SELECT trigger_nestlevel()) > 1) 
       BEGIN 
           SET @syncContext = NULL 
       END 
 
       SELECT @syncContext AS ChangeContext 
                    ,d.TransactionLineItemGuid AS Deleted_PK_TransactionLineItemGuid
                    ,i.TransactionLineItemGuid AS Inserted_PK_TransactionLineItemGuid
                    ,d.TransactionGuid AS Deleted_FK_ParentPK
                    ,i.TransactionGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TransactionLineItemGuid = i.TransactionLineItemGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionLineItems As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionLineItemGuid = currentTrackingData.PK_TransactionLineItemGuid
 
 
		    INSERT track.tblTransactionLineItems (InsertedDate 
 			    	,InsertedContext 
 				    ,InsertedRowVersion 
 				    ,UpdatedDate 
 				    ,UpdatedContext 
 				    ,UpdatedRowVersion 
 				    ,DeletedDate 
 				    ,DeletedContext 
 				    ,DeletedRowVersion 
 				    ,CurrentSiteGuid 
 				    ,PreviousSiteGuid 
				    ,PK_TransactionLineItemGuid
				    ,FK_ParentPK 
		    )
		    SELECT CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
		                 WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
		                 ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
			    	,entityChanges.ChangeContext 
				    ,entityChanges.Inserted_RowVersion 
    				,entityChanges.Inserted_CreatedDate 
	    			,entityChanges.ChangeContext 
		    		,entityChanges.Inserted_RowVersion 
			    	,NULL 
    				,NULL 
	    			,NULL 
		    		,entityChanges.CurrentSiteGuid 
			    	,CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), entityChanges.PreviousSiteGuid),'')) THEN entityChanges.PreviousSiteGuid ELSE NULL END
				    ,entityChanges.Inserted_PK_TransactionLineItemGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionLineItems As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionLineItemGuid = currentTrackingData.PK_TransactionLineItemGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionLineItems
CREATE TRIGGER dbo.trg_del_tblTransactionLineItems_ForSync 
   ON dbo.tblTransactionLineItems
   AFTER DELETE 
AS 
BEGIN 
	-- SET NOCOUNT ON added to prevent extra result sets from 
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 

    DECLARE @changeContextName nvarchar(100); 
    DECLARE @bypassTrackingFlags int; 
    DECLARE @bypassReason nvarchar(512); 

    SELECT @changeContextName = ContextName 
            ,@bypassTrackingFlags = BypassTrackingFlags 
            ,@bypassReason = BypassReason 
        FROM [track].[udf_GetChangeTrackingSessionDetails](); 

	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application delete.
    DECLARE @syncContext varbinary(128); 
    DECLARE @currentDateTimeOffset datetimeoffset(7); 

    SET @currentDateTimeOffset = sysdatetimeoffset(); 

    IF ([track].[udf_IsDeleteChangeTrackingEnabled](@bypassTrackingFlags) = 1)
    BEGIN
       SET @syncContext = dbo.udf_GetSyncContext(); 

       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
       -- Treat the change as a local change so it can be synchronized back to the remote system. 
       IF ((SELECT trigger_nestlevel()) > 1) 
       BEGIN 
           SET @syncContext = NULL 
       END 

		  ; WITH ChangeList AS ( 
				SELECT @syncContext AS ChangeContext 
						,d.TransactionLineItemGuid AS Deleted_PK_TransactionLineItemGuid
                        ,d.TransactionLineItemGuid AS Inserted_PK_TransactionLineItemGuid
                      ,d.TransactionGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionLineItems As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionLineItemGuid = currentTrackingData.PK_TransactionLineItemGuid
				WHEN Matched 
				THEN 
					UPDATE SET DeletedDate = @currentDateTimeOffset 
								,DeletedContext = entityChanges.ChangeContext 
                             ,DeletedRowVersion = entityChanges.Deleted_RowVersion 
				WHEN Not Matched 
				THEN 
				INSERT (InsertedDate
				    	,InsertedContext
				    	,InsertedRowVersion
				    	,UpdatedDate
				    	,UpdatedContext
				    	,UpdatedRowVersion
				    	,CurrentSiteGuid
				    	,PreviousSiteGuid
				    	,DeletedDate
				    	,DeletedContext
				    	,DeletedRowVersion
						,PK_TransactionLineItemGuid
				        ,FK_ParentPK 
				)
				VALUES (CASE WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
						,entityChanges.ChangeContext 
						,entityChanges.Inserted_RowVersion 
						,NULL 
						,NULL 
						,NULL 
						,entityChanges.CurrentSiteGuid 
						,NULL 
						,@currentDateTimeOffset 
						,entityChanges.ChangeContext 
						,entityChanges.Deleted_RowVersion
						,entityChanges.Deleted_PK_TransactionLineItemGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionLineItems] ON [dbo].[tblTransactionLineItems] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItems','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'I' -- For Inserts 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblTransactionLineItems (
		[SequenceID]
	,	[MeterStart]
	,	[MeterStop]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[Temperature]
	,	[Vcf]
	,	[Density]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[ProductPrice]
	,	[CLIN]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[ContractNumber]
	,	[DestinationRegistrationID]
	,	[DestinationSerialNumber]
	,	[DestinationEquipmentType]
	,	[DestinationEquipmentModel]
	,	[DestinationCompanyEquipmentID]
	,	[DestinationCompartmentID]
	,	[SourceRegistrationID]
	,	[SourceSerialNumber]
	,	[SourceEquipmentType]
	,	[SourceEquipmentModel]
	,	[SourceCompanyEquipmentID]
	,	[SourceCompartmentID]
	,	[MeterFactor]
	,	[LineItemSequenceNumber]
	,	[BatchNumber]
	,	[DocumentNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[OperatorID]
	,	[TankStatus]
	,	[MeterStartDateTime]
	,	[MeterStopDateTime]
	,	[Pit]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[AcknowledgedDateTime]
	,	[OnLocationTime]
	,	[ValidationDateTime]
	,	[CompletionDateTime]
	,	[ReceiptVariance]
	,	[DifferentialPressure]
	,	[LoadRackVariance]
	,	[RequestedBy]
	,	[FreezePoint]
	,	[DeleteFlag]
	,	[StorageLocationID]
	,	[MeterID]
	,	[AdditiveProfileID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[PresetAmount]
	,	[EngineeringUnitsIndex]
	,	[CustomerProductName]
	,	[CustomerProductCode]
	,	[TransactionInventoryDate]
	,	[COAWaiver]
	,	[COANote]
	,	[COAID]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[LoadingLocationID]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[ContaminatePrompt]
	,	[CompartmentsPreviouslyLoaded]
	,	[CompartmentsEmpty]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[OdometerHours]
	,	[EndDeliveryDate]
	,	[RequestedDeliveryDate]
	,	[InvoiceNumber]
	,	[InvoiceLineNumber]
	,	[AlternativeGrossVolume]
	,	[AlternativeNetVolume]
	,	[AlternativeUnits]
	,	[TankLevel]
	,	[TankLevelUnits]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[NonDomesticPrice]
	,	[CurrencyUnit]
	,	[ExchangeRate]
	,	[QualityTestNumber]
	,	[Odometer]
	,	[DeliveryLocation]
	,	[Variance]
	,	[PartialFill]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,	[DeliveredGrossManualValueFlag]
	,	[DeliveredNetManualValueFlag]
	,	[TransactionLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[StorageLocationTankGuid]
	,	[AdditiveProfileGuid]
	,	[DestinationCompartmentEquipmentGuid]
	,	[DestinationEquipmentGuid]
	,	[OperatorPersonnelGuid]
	,	[ProductGuid]
	,	[SourceCompartmentEquipmentGuid]
	,	[SourceEquipmentGuid]
	,	[TransactionGuid]
	,	[CurrencyGuid]
	,	[OrderReferenceTransactionLineItemGuid]
	,	[LoadingLocationStationGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
	,	[DualFuelingModeFlag]
	,	[DualFuelingPrimaryFlag]
	,	[EngineRunTime]
	,	[FlowRate]
	,	[FuelCompressionFactor]
	,	[HydrantPressure]
	,	[MobileDeviceID]
	,	[MobileDeviceGuid]
	,	[TemperatureQualityStatus]
	,	[MeterStartObtainedAutomaticallyFlag]
	,	[MeterStopObtainedAutomaticallyFlag]
	,	[NetVolumeIndicator]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		i.[SequenceID]
	,	i.[MeterStart]
	,	i.[MeterStop]
	,	i.[GrossQuantity]
	,  i.[DeliveredGrossQuantity]
	,	i.[Temperature]
	,	i.[Vcf]
	,	i.[Density]
	,	i.[Product]
	,	i.[ProductCode]
	,	i.[ProductType]
	,	i.[ProductPrice]
	,	i.[CLIN]
	,	i.[NetQuantity]
	,  i.[DeliveredNetQuantity]
	,  i.[Pressure]	
	,	i.[ContractNumber]
	,	i.[DestinationRegistrationID]
	,	i.[DestinationSerialNumber]
	,	i.[DestinationEquipmentType]
	,	i.[DestinationEquipmentModel]
	,	i.[DestinationCompanyEquipmentID]
	,	i.[DestinationCompartmentID]
	,	i.[SourceRegistrationID]
	,	i.[SourceSerialNumber]
	,	i.[SourceEquipmentType]
	,	i.[SourceEquipmentModel]
	,	i.[SourceCompanyEquipmentID]
	,	i.[SourceCompartmentID]
	,	i.[MeterFactor]
	,	i.[LineItemSequenceNumber]
	,	i.[BatchNumber]
	,	i.[DocumentNumber]
	,	i.[LineFill]
	,	i.[BottomVolume]
	,	i.[NetCapacity]
	,	i.[Customs]
	,	i.[ArmNumber]
	,	i.[LineNumber]
	,	i.[OperatorID]
	,	i.[TankStatus]
	,	i.[MeterStartDateTime]
	,	i.[MeterStopDateTime]
	,	i.[Pit]
	,	i.[RequestedDateTime]
	,	i.[DispatchedDateTime]
	,	i.[AcknowledgedDateTime]
	,	i.[OnLocationTime]
	,	i.[ValidationDateTime]
	,	i.[CompletionDateTime]
	,	i.[ReceiptVariance]
	,	i.[DifferentialPressure]
	,	i.[LoadRackVariance]
	,	i.[RequestedBy]
	,	i.[FreezePoint]
	,	i.[DeleteFlag]
	,	i.[StorageLocationID]
	,	i.[MeterID]
	,	i.[AdditiveProfileID]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[PresetAmount]
	,	i.[EngineeringUnitsIndex]
	,	i.[CustomerProductName]
	,	i.[CustomerProductCode]
	,	i.[TransactionInventoryDate]
	,	i.[COAWaiver]
	,	i.[COANote]
	,	i.[COAID]
	,	i.[Tax1]
	,	i.[Tax2]
	,	i.[Tax3]
	,	i.[Tax4]
	,	i.[Tax5]
	,	i.[TransVersion]
	,	i.[LoadingLocationID]
	,	i.[ImproperAdditization]
	,	i.[BrokenBlend]
	,	i.[ContaminatePrompt]
	,	i.[CompartmentsPreviouslyLoaded]
	,	i.[CompartmentsEmpty]
	,	i.[Flag01]
	,	i.[Flag02]
	,	i.[Flag03]
	,	i.[Flag04]
	,	i.[Flag05]
	,	i.[Flag06]
	,	i.[Number01]
	,	i.[Number02]
	,	i.[Number03]
	,	i.[Number04]
	,	i.[Number05]
	,	i.[Number06]
	,	i.[OdometerHours]
	,	i.[EndDeliveryDate]
	,	i.[RequestedDeliveryDate]
	,	i.[InvoiceNumber]
	,	i.[InvoiceLineNumber]
	,	i.[AlternativeGrossVolume]
	,	i.[AlternativeNetVolume]
	,	i.[AlternativeUnits]
	,	i.[TankLevel]
	,	i.[TankLevelUnits]
	,	i.[Date01]
	,	i.[Date02]
	,	i.[Date03]
	,	i.[Date04]
	,	i.[NonDomesticPrice]
	,	i.[CurrencyUnit]
	,	i.[ExchangeRate]
	,	i.[QualityTestNumber]
	,	i.[Odometer]
	,	i.[DeliveryLocation]
	,	i.[Variance]
	,	i.[PartialFill]
	,	i.[MassQuantity]
	,	i.[NetManualValueFlag]
	,	i.[MassManualValueFlag]
	,	i.[GrossManualValueFlag]
	,	i.[VcfManualValueFlag]
	,  i.[DeliveredGrossManualValueFlag]
	,  i.[DeliveredNetManualValueFlag]
	,	i.[TransactionLineItemGuid]
	,	i.[_RowVersion]
	,	i.[LookupTransactionStatusIndex]
	,	i.[LookupQualityIndex]
	,	i.[StorageLocationTankGuid]
	,	i.[AdditiveProfileGuid]
	,	i.[DestinationCompartmentEquipmentGuid]
	,	i.[DestinationEquipmentGuid]
	,	i.[OperatorPersonnelGuid]
	,	i.[ProductGuid]
	,	i.[SourceCompartmentEquipmentGuid]
	,	i.[SourceEquipmentGuid]
	,	i.[TransactionGuid]
	,	i.[CurrencyGuid]
	,	i.[OrderReferenceTransactionLineItemGuid]
	,	i.[LoadingLocationStationGuid]
	,	i.[MeterGuid]
	,	i.[PackageManualValueFlag]
	,	i.[CleanLineItem]
	,	i.[CleanLineDeductItem]
	,	i.[CleanLineDeductQuantity]
	,	i.[CleanLinePackQuantity]
	,	i.[DualFuelingModeFlag]
	,	i.[DualFuelingPrimaryFlag]
	,	i.[EngineRunTime]
	,	i.[FlowRate]
	,	i.[FuelCompressionFactor]
	,	i.[HydrantPressure]
	,	i.[MobileDeviceID]
	,	i.[MobileDeviceGuid]
	,	i.[TemperatureQualityStatus]
	,	i.[MeterStartObtainedAutomaticallyFlag]
	,	i.[MeterStopObtainedAutomaticallyFlag]
	,	i.[NetVolumeIndicator]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM inserted i
END

GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionLineItems] ON [dbo].[tblTransactionLineItems] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItems','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'U' -- For Updates 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
 
	DECLARE @AuditGuidList TABLE
	(
	TransactionLineItemGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionLineItems (
		[SequenceID]
	,	[MeterStart]
	,	[MeterStop]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[Temperature]
	,	[Vcf]
	,	[Density]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[ProductPrice]
	,	[CLIN]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[ContractNumber]
	,	[DestinationRegistrationID]
	,	[DestinationSerialNumber]
	,	[DestinationEquipmentType]
	,	[DestinationEquipmentModel]
	,	[DestinationCompanyEquipmentID]
	,	[DestinationCompartmentID]
	,	[SourceRegistrationID]
	,	[SourceSerialNumber]
	,	[SourceEquipmentType]
	,	[SourceEquipmentModel]
	,	[SourceCompanyEquipmentID]
	,	[SourceCompartmentID]
	,	[MeterFactor]
	,	[LineItemSequenceNumber]
	,	[BatchNumber]
	,	[DocumentNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[OperatorID]
	,	[TankStatus]
	,	[MeterStartDateTime]
	,	[MeterStopDateTime]
	,	[Pit]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[AcknowledgedDateTime]
	,	[OnLocationTime]
	,	[ValidationDateTime]
	,	[CompletionDateTime]
	,	[ReceiptVariance]
	,	[DifferentialPressure]
	,	[LoadRackVariance]
	,	[RequestedBy]
	,	[FreezePoint]
	,	[DeleteFlag]
	,	[StorageLocationID]
	,	[MeterID]
	,	[AdditiveProfileID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[PresetAmount]
	,	[EngineeringUnitsIndex]
	,	[CustomerProductName]
	,	[CustomerProductCode]
	,	[TransactionInventoryDate]
	,	[COAWaiver]
	,	[COANote]
	,	[COAID]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[LoadingLocationID]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[ContaminatePrompt]
	,	[CompartmentsPreviouslyLoaded]
	,	[CompartmentsEmpty]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[OdometerHours]
	,	[EndDeliveryDate]
	,	[RequestedDeliveryDate]
	,	[InvoiceNumber]
	,	[InvoiceLineNumber]
	,	[AlternativeGrossVolume]
	,	[AlternativeNetVolume]
	,	[AlternativeUnits]
	,	[TankLevel]
	,	[TankLevelUnits]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[NonDomesticPrice]
	,	[CurrencyUnit]
	,	[ExchangeRate]
	,	[QualityTestNumber]
	,	[Odometer]
	,	[DeliveryLocation]
	,	[Variance]
	,	[PartialFill]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,	[DeliveredGrossManualValueFlag]
	,	[DeliveredNetManualValueFlag]
	,	[TransactionLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[StorageLocationTankGuid]
	,	[AdditiveProfileGuid]
	,	[DestinationCompartmentEquipmentGuid]
	,	[DestinationEquipmentGuid]
	,	[OperatorPersonnelGuid]
	,	[ProductGuid]
	,	[SourceCompartmentEquipmentGuid]
	,	[SourceEquipmentGuid]
	,	[TransactionGuid]
	,	[CurrencyGuid]
	,	[OrderReferenceTransactionLineItemGuid]
	,	[LoadingLocationStationGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
	,	[DualFuelingModeFlag]
	,	[DualFuelingPrimaryFlag]
	,	[EngineRunTime]
	,	[FlowRate]
	,	[FuelCompressionFactor]
	,	[HydrantPressure]
	,	[MobileDeviceID]
	,	[MobileDeviceGuid]
	,	[TemperatureQualityStatus]
	,	[MeterStartObtainedAutomaticallyFlag]
	,	[MeterStopObtainedAutomaticallyFlag]
	,	[NetVolumeIndicator]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	OUTPUT inserted.[TransactionLineItemGuid] AS 'TransactionLineItemGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SequenceID]
	,	d.[MeterStart]
	,	d.[MeterStop]
	,	d.[GrossQuantity]
	,  d.[DeliveredGrossQuantity]
	,	d.[Temperature]
	,	d.[Vcf]
	,	d.[Density]
	,	d.[Product]
	,	d.[ProductCode]
	,	d.[ProductType]
	,	d.[ProductPrice]
	,	d.[CLIN]
	,	d.[NetQuantity]
	,  d.[DeliveredNetQuantity]
	,  d.[Pressure]	
	,	d.[ContractNumber]
	,	d.[DestinationRegistrationID]
	,	d.[DestinationSerialNumber]
	,	d.[DestinationEquipmentType]
	,	d.[DestinationEquipmentModel]
	,	d.[DestinationCompanyEquipmentID]
	,	d.[DestinationCompartmentID]
	,	d.[SourceRegistrationID]
	,	d.[SourceSerialNumber]
	,	d.[SourceEquipmentType]
	,	d.[SourceEquipmentModel]
	,	d.[SourceCompanyEquipmentID]
	,	d.[SourceCompartmentID]
	,	d.[MeterFactor]
	,	d.[LineItemSequenceNumber]
	,	d.[BatchNumber]
	,	d.[DocumentNumber]
	,	d.[LineFill]
	,	d.[BottomVolume]
	,	d.[NetCapacity]
	,	d.[Customs]
	,	d.[ArmNumber]
	,	d.[LineNumber]
	,	d.[OperatorID]
	,	d.[TankStatus]
	,	d.[MeterStartDateTime]
	,	d.[MeterStopDateTime]
	,	d.[Pit]
	,	d.[RequestedDateTime]
	,	d.[DispatchedDateTime]
	,	d.[AcknowledgedDateTime]
	,	d.[OnLocationTime]
	,	d.[ValidationDateTime]
	,	d.[CompletionDateTime]
	,	d.[ReceiptVariance]
	,	d.[DifferentialPressure]
	,	d.[LoadRackVariance]
	,	d.[RequestedBy]
	,	d.[FreezePoint]
	,	d.[DeleteFlag]
	,	d.[StorageLocationID]
	,	d.[MeterID]
	,	d.[AdditiveProfileID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[PresetAmount]
	,	d.[EngineeringUnitsIndex]
	,	d.[CustomerProductName]
	,	d.[CustomerProductCode]
	,	d.[TransactionInventoryDate]
	,	d.[COAWaiver]
	,	d.[COANote]
	,	d.[COAID]
	,	d.[Tax1]
	,	d.[Tax2]
	,	d.[Tax3]
	,	d.[Tax4]
	,	d.[Tax5]
	,	d.[TransVersion]
	,	d.[LoadingLocationID]
	,	d.[ImproperAdditization]
	,	d.[BrokenBlend]
	,	d.[ContaminatePrompt]
	,	d.[CompartmentsPreviouslyLoaded]
	,	d.[CompartmentsEmpty]
	,	d.[Flag01]
	,	d.[Flag02]
	,	d.[Flag03]
	,	d.[Flag04]
	,	d.[Flag05]
	,	d.[Flag06]
	,	d.[Number01]
	,	d.[Number02]
	,	d.[Number03]
	,	d.[Number04]
	,	d.[Number05]
	,	d.[Number06]
	,	d.[OdometerHours]
	,	d.[EndDeliveryDate]
	,	d.[RequestedDeliveryDate]
	,	d.[InvoiceNumber]
	,	d.[InvoiceLineNumber]
	,	d.[AlternativeGrossVolume]
	,	d.[AlternativeNetVolume]
	,	d.[AlternativeUnits]
	,	d.[TankLevel]
	,	d.[TankLevelUnits]
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[NonDomesticPrice]
	,	d.[CurrencyUnit]
	,	d.[ExchangeRate]
	,	d.[QualityTestNumber]
	,	d.[Odometer]
	,	d.[DeliveryLocation]
	,	d.[Variance]
	,	d.[PartialFill]
	,	d.[MassQuantity]
	,	d.[NetManualValueFlag]
	,	d.[MassManualValueFlag]
	,	d.[GrossManualValueFlag]
	,	d.[VcfManualValueFlag]
	,	d.[DeliveredGrossManualValueFlag]
	,	d.[DeliveredNetManualValueFlag]
	,	d.[TransactionLineItemGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupQualityIndex]
	,	d.[StorageLocationTankGuid]
	,	d.[AdditiveProfileGuid]
	,	d.[DestinationCompartmentEquipmentGuid]
	,	d.[DestinationEquipmentGuid]
	,	d.[OperatorPersonnelGuid]
	,	d.[ProductGuid]
	,	d.[SourceCompartmentEquipmentGuid]
	,	d.[SourceEquipmentGuid]
	,	d.[TransactionGuid]
	,	d.[CurrencyGuid]
	,	d.[OrderReferenceTransactionLineItemGuid]
	,	d.[LoadingLocationStationGuid]
	,	d.[MeterGuid]
	,	d.[PackageManualValueFlag]
	,	d.[CleanLineItem]
	,	d.[CleanLineDeductItem]
	,	d.[CleanLineDeductQuantity]
	,	d.[CleanLinePackQuantity]
	,	d.[DualFuelingModeFlag]
	,	d.[DualFuelingPrimaryFlag]
	,	d.[EngineRunTime]
	,	d.[FlowRate]
	,	d.[FuelCompressionFactor]
	,	d.[HydrantPressure]
	,	d.[MobileDeviceID]
	,	d.[MobileDeviceGuid]
	,	d.[TemperatureQualityStatus]
	,	d.[MeterStartObtainedAutomaticallyFlag]
	,	d.[MeterStopObtainedAutomaticallyFlag]
	,	d.[NetVolumeIndicator]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
 
	INSERT INTO [fmaudit].tblTransactionLineItems (
		[SequenceID]
	,	[MeterStart]
	,	[MeterStop]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[Temperature]
	,	[Vcf]
	,	[Density]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[ProductPrice]
	,	[CLIN]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[ContractNumber]
	,	[DestinationRegistrationID]
	,	[DestinationSerialNumber]
	,	[DestinationEquipmentType]
	,	[DestinationEquipmentModel]
	,	[DestinationCompanyEquipmentID]
	,	[DestinationCompartmentID]
	,	[SourceRegistrationID]
	,	[SourceSerialNumber]
	,	[SourceEquipmentType]
	,	[SourceEquipmentModel]
	,	[SourceCompanyEquipmentID]
	,	[SourceCompartmentID]
	,	[MeterFactor]
	,	[LineItemSequenceNumber]
	,	[BatchNumber]
	,	[DocumentNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[OperatorID]
	,	[TankStatus]
	,	[MeterStartDateTime]
	,	[MeterStopDateTime]
	,	[Pit]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[AcknowledgedDateTime]
	,	[OnLocationTime]
	,	[ValidationDateTime]
	,	[CompletionDateTime]
	,	[ReceiptVariance]
	,	[DifferentialPressure]
	,	[LoadRackVariance]
	,	[RequestedBy]
	,	[FreezePoint]
	,	[DeleteFlag]
	,	[StorageLocationID]
	,	[MeterID]
	,	[AdditiveProfileID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[PresetAmount]
	,	[EngineeringUnitsIndex]
	,	[CustomerProductName]
	,	[CustomerProductCode]
	,	[TransactionInventoryDate]
	,	[COAWaiver]
	,	[COANote]
	,	[COAID]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[LoadingLocationID]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[ContaminatePrompt]
	,	[CompartmentsPreviouslyLoaded]
	,	[CompartmentsEmpty]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[OdometerHours]
	,	[EndDeliveryDate]
	,	[RequestedDeliveryDate]
	,	[InvoiceNumber]
	,	[InvoiceLineNumber]
	,	[AlternativeGrossVolume]
	,	[AlternativeNetVolume]
	,	[AlternativeUnits]
	,	[TankLevel]
	,	[TankLevelUnits]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[NonDomesticPrice]
	,	[CurrencyUnit]
	,	[ExchangeRate]
	,	[QualityTestNumber]
	,	[Odometer]
	,	[DeliveryLocation]
	,	[Variance]
	,	[PartialFill]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,  [DeliveredGrossManualValueFlag]
	,  [DeliveredNetManualValueFlag]
	,	[TransactionLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[StorageLocationTankGuid]
	,	[AdditiveProfileGuid]
	,	[DestinationCompartmentEquipmentGuid]
	,	[DestinationEquipmentGuid]
	,	[OperatorPersonnelGuid]
	,	[ProductGuid]
	,	[SourceCompartmentEquipmentGuid]
	,	[SourceEquipmentGuid]
	,	[TransactionGuid]
	,	[CurrencyGuid]
	,	[OrderReferenceTransactionLineItemGuid]
	,	[LoadingLocationStationGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
	,	[DualFuelingModeFlag]
	,	[DualFuelingPrimaryFlag]
	,	[EngineRunTime]
	,	[FlowRate]
	,	[FuelCompressionFactor]
	,	[HydrantPressure]
	,	[MobileDeviceID]
	,	[MobileDeviceGuid]
	,	[TemperatureQualityStatus]
	,	[MeterStartObtainedAutomaticallyFlag]
	,	[MeterStopObtainedAutomaticallyFlag]
	,	[NetVolumeIndicator]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		i.[SequenceID]
	,	i.[MeterStart]
	,	i.[MeterStop]
	,	i.[GrossQuantity]
	,  i.[DeliveredGrossQuantity]
	,	i.[Temperature]
	,	i.[Vcf]
	,	i.[Density]
	,	i.[Product]
	,	i.[ProductCode]
	,	i.[ProductType]
	,	i.[ProductPrice]
	,	i.[CLIN]
	,	i.[NetQuantity]
	,  i.[DeliveredNetQuantity]
	,  i.[Pressure]	
	,	i.[ContractNumber]
	,	i.[DestinationRegistrationID]
	,	i.[DestinationSerialNumber]
	,	i.[DestinationEquipmentType]
	,	i.[DestinationEquipmentModel]
	,	i.[DestinationCompanyEquipmentID]
	,	i.[DestinationCompartmentID]
	,	i.[SourceRegistrationID]
	,	i.[SourceSerialNumber]
	,	i.[SourceEquipmentType]
	,	i.[SourceEquipmentModel]
	,	i.[SourceCompanyEquipmentID]
	,	i.[SourceCompartmentID]
	,	i.[MeterFactor]
	,	i.[LineItemSequenceNumber]
	,	i.[BatchNumber]
	,	i.[DocumentNumber]
	,	i.[LineFill]
	,	i.[BottomVolume]
	,	i.[NetCapacity]
	,	i.[Customs]
	,	i.[ArmNumber]
	,	i.[LineNumber]
	,	i.[OperatorID]
	,	i.[TankStatus]
	,	i.[MeterStartDateTime]
	,	i.[MeterStopDateTime]
	,	i.[Pit]
	,	i.[RequestedDateTime]
	,	i.[DispatchedDateTime]
	,	i.[AcknowledgedDateTime]
	,	i.[OnLocationTime]
	,	i.[ValidationDateTime]
	,	i.[CompletionDateTime]
	,	i.[ReceiptVariance]
	,	i.[DifferentialPressure]
	,	i.[LoadRackVariance]
	,	i.[RequestedBy]
	,	i.[FreezePoint]
	,	i.[DeleteFlag]
	,	i.[StorageLocationID]
	,	i.[MeterID]
	,	i.[AdditiveProfileID]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[PresetAmount]
	,	i.[EngineeringUnitsIndex]
	,	i.[CustomerProductName]
	,	i.[CustomerProductCode]
	,	i.[TransactionInventoryDate]
	,	i.[COAWaiver]
	,	i.[COANote]
	,	i.[COAID]
	,	i.[Tax1]
	,	i.[Tax2]
	,	i.[Tax3]
	,	i.[Tax4]
	,	i.[Tax5]
	,	i.[TransVersion]
	,	i.[LoadingLocationID]
	,	i.[ImproperAdditization]
	,	i.[BrokenBlend]
	,	i.[ContaminatePrompt]
	,	i.[CompartmentsPreviouslyLoaded]
	,	i.[CompartmentsEmpty]
	,	i.[Flag01]
	,	i.[Flag02]
	,	i.[Flag03]
	,	i.[Flag04]
	,	i.[Flag05]
	,	i.[Flag06]
	,	i.[Number01]
	,	i.[Number02]
	,	i.[Number03]
	,	i.[Number04]
	,	i.[Number05]
	,	i.[Number06]
	,	i.[OdometerHours]
	,	i.[EndDeliveryDate]
	,	i.[RequestedDeliveryDate]
	,	i.[InvoiceNumber]
	,	i.[InvoiceLineNumber]
	,	i.[AlternativeGrossVolume]
	,	i.[AlternativeNetVolume]
	,	i.[AlternativeUnits]
	,	i.[TankLevel]
	,	i.[TankLevelUnits]
	,	i.[Date01]
	,	i.[Date02]
	,	i.[Date03]
	,	i.[Date04]
	,	i.[NonDomesticPrice]
	,	i.[CurrencyUnit]
	,	i.[ExchangeRate]
	,	i.[QualityTestNumber]
	,	i.[Odometer]
	,	i.[DeliveryLocation]
	,	i.[Variance]
	,	i.[PartialFill]
	,	i.[MassQuantity]
	,	i.[NetManualValueFlag]
	,	i.[MassManualValueFlag]
	,	i.[GrossManualValueFlag]
	,	i.[VcfManualValueFlag]
	,  i.[DeliveredGrossManualValueFlag]
	,  i.[DeliveredNetManualValueFlag]
	,	i.[TransactionLineItemGuid]
	,	i.[_RowVersion]
	,	i.[LookupTransactionStatusIndex]
	,	i.[LookupQualityIndex]
	,	i.[StorageLocationTankGuid]
	,	i.[AdditiveProfileGuid]
	,	i.[DestinationCompartmentEquipmentGuid]
	,	i.[DestinationEquipmentGuid]
	,	i.[OperatorPersonnelGuid]
	,	i.[ProductGuid]
	,	i.[SourceCompartmentEquipmentGuid]
	,	i.[SourceEquipmentGuid]
	,	i.[TransactionGuid]
	,	i.[CurrencyGuid]
	,	i.[OrderReferenceTransactionLineItemGuid]
	,	i.[LoadingLocationStationGuid]
	,	i.[MeterGuid]
	,	i.[PackageManualValueFlag]
	,	i.[CleanLineItem]
	,	i.[CleanLineDeductItem]
	,	i.[CleanLineDeductQuantity]
	,	i.[CleanLinePackQuantity]
	,	i.[DualFuelingModeFlag]
	,	i.[DualFuelingPrimaryFlag]
	,	i.[EngineRunTime]
	,	i.[FlowRate]
	,	i.[FuelCompressionFactor]
	,	i.[HydrantPressure]
	,	i.[MobileDeviceID]
	,	i.[MobileDeviceGuid]
	,	i.[TemperatureQualityStatus]
	,	i.[MeterStartObtainedAutomaticallyFlag]
	,	i.[MeterStopObtainedAutomaticallyFlag]
	,	i.[NetVolumeIndicator]
	,	@_AuditEventType
	,	2
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	agl._AuditGUID
	,	@_UserId
	,	@_AuditContext
	FROM inserted i 
	INNER JOIN	@AuditGuidList agl ON
		(
			agl.[TransactionLineItemGuid]=i.[TransactionLineItemGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblTransactionLineItems]
ON [dbo].[tblTransactionLineItems]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @eventType nvarchar(20)
	IF ((EXISTS(SELECT * FROM inserted)) AND (EXISTS(SELECT * FROM deleted)))
		SELECT @eventType = 'update'
	ELSE IF (EXISTS(SELECT * FROM inserted))
		SELECT @eventType = 'insert'
	ELSE IF (EXISTS(SELECT * FROM deleted))
		SELECT @eventType = 'delete'
	IF (@eventType = 'delete')
	BEGIN
		DECLARE  @context_info varbinary(128)
		DECLARE  @context_info_str varchar(128)
		SELECT @Context_Info = CONTEXT_INFO()
		SELECT @context_info_str = CAST (@context_info as varchar(128))
		IF (@context_info_str = 'dbo.fm_ArchiveTransaction')
		BEGIN
			RETURN
		END
		INSERT INTO fmcdc.[tblTransactionLineItems]
		(
		[SequenceID]
		, [MeterStart]
		, [MeterStop]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [Temperature]
		, [Vcf]
		, [Density]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [ProductPrice]
		, [CLIN]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [ContractNumber]
		, [DestinationRegistrationID]
		, [DestinationSerialNumber]
		, [DestinationEquipmentType]
		, [DestinationEquipmentModel]
		, [DestinationCompanyEquipmentID]
		, [DestinationCompartmentID]
		, [SourceRegistrationID]
		, [SourceSerialNumber]
		, [SourceEquipmentType]
		, [SourceEquipmentModel]
		, [SourceCompanyEquipmentID]
		, [SourceCompartmentID]
		, [MeterFactor]
		, [LineItemSequenceNumber]
		, [BatchNumber]
		, [DocumentNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [OperatorID]
		, [TankStatus]
		, [MeterStartDateTime]
		, [MeterStopDateTime]
		, [Pit]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [AcknowledgedDateTime]
		, [OnLocationTime]
		, [ValidationDateTime]
		, [CompletionDateTime]
		, [ReceiptVariance]
		, [DifferentialPressure]
		, [LoadRackVariance]
		, [RequestedBy]
		, [FreezePoint]
		, [DeleteFlag]
		, [StorageLocationID]
		, [MeterID]
		, [AdditiveProfileID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [PresetAmount]
		, [EngineeringUnitsIndex]
		, [CustomerProductName]
		, [CustomerProductCode]
		, [TransactionInventoryDate]
		, [COAWaiver]
		, [COANote]
		, [COAID]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [LoadingLocationID]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [ContaminatePrompt]
		, [CompartmentsPreviouslyLoaded]
		, [CompartmentsEmpty]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [OdometerHours]
		, [EndDeliveryDate]
		, [RequestedDeliveryDate]
		, [InvoiceNumber]
		, [InvoiceLineNumber]
		, [AlternativeGrossVolume]
		, [AlternativeNetVolume]
		, [AlternativeUnits]
		, [TankLevel]
		, [TankLevelUnits]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [NonDomesticPrice]
		, [CurrencyUnit]
		, [ExchangeRate]
		, [QualityTestNumber]
		, [Odometer]
		, [DeliveryLocation]
		, [Variance]
		, [PartialFill]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionLineItemGuid]
		, [SourceRowVersion]
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [StorageLocationTankGuid]
		, [AdditiveProfileGuid]
		, [DestinationCompartmentEquipmentGuid]
		, [DestinationEquipmentGuid]
		, [OperatorPersonnelGuid]
		, [ProductGuid]
		, [SourceCompartmentEquipmentGuid]
		, [SourceEquipmentGuid]
		, [TransactionGuid]
		, [CurrencyGuid]
		, [OrderReferenceTransactionLineItemGuid]
		, [LoadingLocationStationGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [DualFuelingModeFlag]
		, [DualFuelingPrimaryFlag]
		, [EngineRunTime]
		, [FlowRate]
		, [FuelCompressionFactor]
		, [HydrantPressure]
		, [MobileDeviceID]
		, [MobileDeviceGuid]
		, [TemperatureQualityStatus]
		, [MeterStartObtainedAutomaticallyFlag]
		, [MeterStopObtainedAutomaticallyFlag]
		, [NetVolumeIndicator]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[SequenceID]
		, [MeterStart]
		, [MeterStop]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [Temperature]
		, [Vcf]
		, [Density]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [ProductPrice]
		, [CLIN]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [ContractNumber]
		, [DestinationRegistrationID]
		, [DestinationSerialNumber]
		, [DestinationEquipmentType]
		, [DestinationEquipmentModel]
		, [DestinationCompanyEquipmentID]
		, [DestinationCompartmentID]
		, [SourceRegistrationID]
		, [SourceSerialNumber]
		, [SourceEquipmentType]
		, [SourceEquipmentModel]
		, [SourceCompanyEquipmentID]
		, [SourceCompartmentID]
		, [MeterFactor]
		, [LineItemSequenceNumber]
		, [BatchNumber]
		, [DocumentNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [OperatorID]
		, [TankStatus]
		, [MeterStartDateTime]
		, [MeterStopDateTime]
		, [Pit]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [AcknowledgedDateTime]
		, [OnLocationTime]
		, [ValidationDateTime]
		, [CompletionDateTime]
		, [ReceiptVariance]
		, [DifferentialPressure]
		, [LoadRackVariance]
		, [RequestedBy]
		, [FreezePoint]
		, [DeleteFlag]
		, [StorageLocationID]
		, [MeterID]
		, [AdditiveProfileID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [PresetAmount]
		, [EngineeringUnitsIndex]
		, [CustomerProductName]
		, [CustomerProductCode]
		, [TransactionInventoryDate]
		, [COAWaiver]
		, [COANote]
		, [COAID]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [LoadingLocationID]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [ContaminatePrompt]
		, [CompartmentsPreviouslyLoaded]
		, [CompartmentsEmpty]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [OdometerHours]
		, [EndDeliveryDate]
		, [RequestedDeliveryDate]
		, [InvoiceNumber]
		, [InvoiceLineNumber]
		, [AlternativeGrossVolume]
		, [AlternativeNetVolume]
		, [AlternativeUnits]
		, [TankLevel]
		, [TankLevelUnits]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [NonDomesticPrice]
		, [CurrencyUnit]
		, [ExchangeRate]
		, [QualityTestNumber]
		, [Odometer]
		, [DeliveryLocation]
		, [Variance]
		, [PartialFill]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionLineItemGuid]
		, CONVERT(bigint, _RowVersion)
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [StorageLocationTankGuid]
		, [AdditiveProfileGuid]
		, [DestinationCompartmentEquipmentGuid]
		, [DestinationEquipmentGuid]
		, [OperatorPersonnelGuid]
		, [ProductGuid]
		, [SourceCompartmentEquipmentGuid]
		, [SourceEquipmentGuid]
		, [TransactionGuid]
		, [CurrencyGuid]
		, [OrderReferenceTransactionLineItemGuid]
		, [LoadingLocationStationGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [DualFuelingModeFlag]
		, [DualFuelingPrimaryFlag]
		, [EngineRunTime]
		, [FlowRate]
		, [FuelCompressionFactor]
		, [HydrantPressure]
		, [MobileDeviceID]
		, [MobileDeviceGuid]
		, [TemperatureQualityStatus]
		, [MeterStartObtainedAutomaticallyFlag]
		, [MeterStopObtainedAutomaticallyFlag]
		, [NetVolumeIndicator]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTransactionLineItems]
		(
		[SequenceID]
		, [MeterStart]
		, [MeterStop]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [Temperature]
		, [Vcf]
		, [Density]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [ProductPrice]
		, [CLIN]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [ContractNumber]
		, [DestinationRegistrationID]
		, [DestinationSerialNumber]
		, [DestinationEquipmentType]
		, [DestinationEquipmentModel]
		, [DestinationCompanyEquipmentID]
		, [DestinationCompartmentID]
		, [SourceRegistrationID]
		, [SourceSerialNumber]
		, [SourceEquipmentType]
		, [SourceEquipmentModel]
		, [SourceCompanyEquipmentID]
		, [SourceCompartmentID]
		, [MeterFactor]
		, [LineItemSequenceNumber]
		, [BatchNumber]
		, [DocumentNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [OperatorID]
		, [TankStatus]
		, [MeterStartDateTime]
		, [MeterStopDateTime]
		, [Pit]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [AcknowledgedDateTime]
		, [OnLocationTime]
		, [ValidationDateTime]
		, [CompletionDateTime]
		, [ReceiptVariance]
		, [DifferentialPressure]
		, [LoadRackVariance]
		, [RequestedBy]
		, [FreezePoint]
		, [DeleteFlag]
		, [StorageLocationID]
		, [MeterID]
		, [AdditiveProfileID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [PresetAmount]
		, [EngineeringUnitsIndex]
		, [CustomerProductName]
		, [CustomerProductCode]
		, [TransactionInventoryDate]
		, [COAWaiver]
		, [COANote]
		, [COAID]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [LoadingLocationID]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [ContaminatePrompt]
		, [CompartmentsPreviouslyLoaded]
		, [CompartmentsEmpty]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [OdometerHours]
		, [EndDeliveryDate]
		, [RequestedDeliveryDate]
		, [InvoiceNumber]
		, [InvoiceLineNumber]
		, [AlternativeGrossVolume]
		, [AlternativeNetVolume]
		, [AlternativeUnits]
		, [TankLevel]
		, [TankLevelUnits]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [NonDomesticPrice]
		, [CurrencyUnit]
		, [ExchangeRate]
		, [QualityTestNumber]
		, [Odometer]
		, [DeliveryLocation]
		, [Variance]
		, [PartialFill]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionLineItemGuid]
		, [SourceRowVersion]
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [StorageLocationTankGuid]
		, [AdditiveProfileGuid]
		, [DestinationCompartmentEquipmentGuid]
		, [DestinationEquipmentGuid]
		, [OperatorPersonnelGuid]
		, [ProductGuid]
		, [SourceCompartmentEquipmentGuid]
		, [SourceEquipmentGuid]
		, [TransactionGuid]
		, [CurrencyGuid]
		, [OrderReferenceTransactionLineItemGuid]
		, [LoadingLocationStationGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [DualFuelingModeFlag]
		, [DualFuelingPrimaryFlag]
		, [EngineRunTime]
		, [FlowRate]
		, [FuelCompressionFactor]
		, [HydrantPressure]
		, [MobileDeviceID]
		, [MobileDeviceGuid]
		, [TemperatureQualityStatus]
		, [MeterStartObtainedAutomaticallyFlag]
		, [MeterStopObtainedAutomaticallyFlag]
		, [NetVolumeIndicator]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[SequenceID]
		, [MeterStart]
		, [MeterStop]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [Temperature]
		, [Vcf]
		, [Density]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [ProductPrice]
		, [CLIN]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [ContractNumber]
		, [DestinationRegistrationID]
		, [DestinationSerialNumber]
		, [DestinationEquipmentType]
		, [DestinationEquipmentModel]
		, [DestinationCompanyEquipmentID]
		, [DestinationCompartmentID]
		, [SourceRegistrationID]
		, [SourceSerialNumber]
		, [SourceEquipmentType]
		, [SourceEquipmentModel]
		, [SourceCompanyEquipmentID]
		, [SourceCompartmentID]
		, [MeterFactor]
		, [LineItemSequenceNumber]
		, [BatchNumber]
		, [DocumentNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [OperatorID]
		, [TankStatus]
		, [MeterStartDateTime]
		, [MeterStopDateTime]
		, [Pit]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [AcknowledgedDateTime]
		, [OnLocationTime]
		, [ValidationDateTime]
		, [CompletionDateTime]
		, [ReceiptVariance]
		, [DifferentialPressure]
		, [LoadRackVariance]
		, [RequestedBy]
		, [FreezePoint]
		, [DeleteFlag]
		, [StorageLocationID]
		, [MeterID]
		, [AdditiveProfileID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [PresetAmount]
		, [EngineeringUnitsIndex]
		, [CustomerProductName]
		, [CustomerProductCode]
		, [TransactionInventoryDate]
		, [COAWaiver]
		, [COANote]
		, [COAID]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [LoadingLocationID]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [ContaminatePrompt]
		, [CompartmentsPreviouslyLoaded]
		, [CompartmentsEmpty]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [OdometerHours]
		, [EndDeliveryDate]
		, [RequestedDeliveryDate]
		, [InvoiceNumber]
		, [InvoiceLineNumber]
		, [AlternativeGrossVolume]
		, [AlternativeNetVolume]
		, [AlternativeUnits]
		, [TankLevel]
		, [TankLevelUnits]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [NonDomesticPrice]
		, [CurrencyUnit]
		, [ExchangeRate]
		, [QualityTestNumber]
		, [Odometer]
		, [DeliveryLocation]
		, [Variance]
		, [PartialFill]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionLineItemGuid]
		, CONVERT(bigint, _RowVersion)
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [StorageLocationTankGuid]
		, [AdditiveProfileGuid]
		, [DestinationCompartmentEquipmentGuid]
		, [DestinationEquipmentGuid]
		, [OperatorPersonnelGuid]
		, [ProductGuid]
		, [SourceCompartmentEquipmentGuid]
		, [SourceEquipmentGuid]
		, [TransactionGuid]
		, [CurrencyGuid]
		, [OrderReferenceTransactionLineItemGuid]
		, [LoadingLocationStationGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [DualFuelingModeFlag]
		, [DualFuelingPrimaryFlag]
		, [EngineRunTime]
		, [FlowRate]
		, [FuelCompressionFactor]
		, [HydrantPressure]
		, [MobileDeviceID]
		, [MobileDeviceGuid]
		, [TemperatureQualityStatus]
		, [MeterStartObtainedAutomaticallyFlag]
		, [MeterStopObtainedAutomaticallyFlag]
		, [NetVolumeIndicator]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTransactionLineItems] ON [dbo].[tblTransactionLineItems]
GO



CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItems_TransactionSummary] ON [dbo].[tblTransactionLineItems]
(
	[TransactionGuid] ASC
)
INCLUDE 
( 
	[GrossQuantity],
	[Product],
	[NetQuantity]
) 
GO

 CREATE NONCLUSTERED INDEX [IX_tbltransactionlineitems_LookupQualityIndex_ProductGuid] ON [dbo].[tblTransactionlineitems]
(
	[LookupQualityIndex] ASC,
	 [ProductGuid] ASC
)
INCLUDE ( [GrossQuantity], [ProductPrice], [NetQuantity], [Number01], [Number02], [Number03], [Number04], [Number05], [Number06], [MassQuantity], [TransactionGuid], [_ClusterIdx]) 
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItems_ProductGuid] ON [dbo].[tblTransactionLineItems] ([ProductGuid] ASC) WITH (FILLFACTOR = 90);
GO

CREATE INDEX [IX_tblTransactionLineItems_OrderReferenceTransactionLineItemGuid] ON [dbo].[tblTransactionLineItems]
 ([OrderReferenceTransactionLineItemGuid])
 GO
