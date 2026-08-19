CREATE TABLE [dbo].[tblTransactions] (
    [TransID]                        NVARCHAR (64)      CONSTRAINT [DF_tblTransactions_TransID] DEFAULT ('') NOT NULL,
    [AliasName]                      NVARCHAR (32)      CONSTRAINT [DF_tblTransactions_AliasName] DEFAULT ('') NOT NULL,
    [SubType]                        NVARCHAR (20)      NULL,
    [Site]                           NVARCHAR (30)      NULL,
    [TransReferenceID]               NVARCHAR (64)      NULL,
    [InventoryDate]                  DATE               NULL,
    [ShipToID]                       NVARCHAR (100)     NULL,
    [ShipToCode]                     NVARCHAR (10)      NULL,
    [SupplierID]                     NVARCHAR (100)     NULL,
    [SupplierCode]                   NVARCHAR (10)      NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactions_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                      [dbo].[udtUserID]  NULL,
    [RequestedDeliveryDate]          DATETIMEOFFSET (7) NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactions_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  NULL,
    [TransDateTime]                  DATETIMEOFFSET (7) NULL,
    [TransVersion]                   BIGINT             CONSTRAINT [DF_tblTransactions_TransVersion] DEFAULT ((0)) NULL,
    [SCACCode]                       NVARCHAR (4)       NULL,
    [CardNumber]                     NVARCHAR (30)      NULL,
    [ShipmentNumber]                 NVARCHAR (30)      NULL,
    [ShipperID]                      NVARCHAR (100)     NULL,
    [ShipperCode]                    NVARCHAR (10)      NULL,
    [OwnerID]                        NVARCHAR (100)     NULL,
    [OwnerCode]                      NVARCHAR (10)      NULL,
    [ManagerID]                      NVARCHAR (100)     NULL,
    [ManagerCode]                    NVARCHAR (10)      NULL,
    [CarrierID]                      NVARCHAR (100)     NULL,
    [CarrierCode]                    NVARCHAR (10)      NULL,
    [ConjoinTransID]                 NVARCHAR (64)      NULL,
    [ReversedTransID]                NVARCHAR (64)      NULL,
    [LinkedDocumentNumber]           NVARCHAR (64)      NULL,
    [ReversalType]                   NVARCHAR (2)       NULL,
    [PONumber]                       NVARCHAR (14)      NULL,
    [TimeIn]                         DATETIMEOFFSET (7) NULL,
    [TimeOut]                        DATETIMEOFFSET (7) NULL,
    [TimeEnd]                        DATETIMEOFFSET (7) NULL,
    [RoutingID]                      NVARCHAR (30)      NULL,
    [TicketSource]                   NVARCHAR (20)      NULL,
    [LoadID]                         NVARCHAR (50)      NULL,
    [BillToID]                       NVARCHAR (100)     NULL,
    [BillToCode]                     NVARCHAR (10)      NULL,
    [DriverIdentificationNumber]     NVARCHAR (50)      NULL,
    [CreditAmount]                   FLOAT (53)         NULL,
    [CardExpiration]                 DATETIMEOFFSET (7) NULL,
    [CardName]                       NVARCHAR (30)      NULL,
    [CardType]                       NVARCHAR (30)      NULL,
    [CashAmount]                     FLOAT (53)         NULL,
    [RouteOriginationDate]           DATETIMEOFFSET (7) NULL,
    [InternationalRouteIndicator]    BIT                NULL,
    [PreviousRoutingID]              NVARCHAR (30)      NULL,
    [ShippingDocumentNumber]         NVARCHAR (30)      NULL,
    [DocumentNumber]                 NVARCHAR (30)      NULL,
    [STD]                            DATETIMEOFFSET (7) NULL,
    [ETD]                            DATETIMEOFFSET (7) NULL,
    [STA]                            DATETIMEOFFSET (7) NULL,
    [ETA]                            DATETIMEOFFSET (7) NULL,
    [SFT]                            DATETIMEOFFSET (7) NULL,
    [FST]                            DATETIMEOFFSET (7) NULL,
    [EstimatedFuelingDuration]       INT                NULL,
    [DeleteFlag]                     BIT                NULL,
    [TicketMode]                     NVARCHAR (15)      NULL,
    [DestinationRegistrationID1]     NVARCHAR (30)      NULL,
    [DestinationSerialNumber1]       NVARCHAR (10)      NULL,
    [DestinationEquipmentType1]      NVARCHAR (50)      NULL,
    [DestinationEquipmentModel1]     NVARCHAR (20)      NULL,
    [DestinationCompanyEquipmentID1] NVARCHAR (30)      NULL,
    [DestinationRegistrationID2]     NVARCHAR (30)      NULL,
    [DestinationSerialNumber2]       NVARCHAR (10)      NULL,
    [DestinationEquipmentType2]      NVARCHAR (50)      NULL,
    [DestinationEquipmentModel2]     NVARCHAR (20)      NULL,
    [DestinationCompanyEquipmentID2] NVARCHAR (30)      NULL,
    [DestinationRegistrationID3]     NVARCHAR (30)      NULL,
    [DestinationSerialNumber3]       NVARCHAR (10)      NULL,
    [DestinationEquipmentType3]      NVARCHAR (50)      NULL,
    [DestinationEquipmentModel3]     NVARCHAR (20)      NULL,
    [DestinationCompanyEquipmentID3] NVARCHAR (30)      NULL,
    [SourceRegistrationID1]          NVARCHAR (30)      NULL,
    [SourceSerialNumber1]            NVARCHAR (10)      NULL,
    [SourceEquipmentType1]           NVARCHAR (50)      NULL,
    [SourceEquipmentModel1]          NVARCHAR (20)      NULL,
    [SourceCompanyEquipmentID1]      NVARCHAR (30)      NULL,
    [SourceRegistrationID2]          NVARCHAR (30)      NULL,
    [SourceSerialNumber2]            NVARCHAR (10)      NULL,
    [SourceEquipmentType2]           NVARCHAR (50)      NULL,
    [SourceEquipmentModel2]          NVARCHAR (20)      NULL,
    [SourceCompanyEquipmentID2]      NVARCHAR (30)      NULL,
    [SourceRegistrationID3]          NVARCHAR (30)      NULL,
    [SourceSerialNumber3]            NVARCHAR (10)      NULL,
    [SourceEquipmentType3]           NVARCHAR (50)      NULL,
    [SourceEquipmentModel3]          NVARCHAR (20)      NULL,
    [SourceCompanyEquipmentID3]      NVARCHAR (30)      NULL,
    [OperatorID]                     NVARCHAR (50)      NULL,
    [EffectiveDate]                  DATETIMEOFFSET (7) NULL,
    [ExpirationDate]                 DATETIMEOFFSET (7) NULL,
    [ScheduledDate]                  DATETIMEOFFSET (7) NULL,
    [AutoComplete]                   BIT                NULL,
    [Flag01]                         BIT                NULL,
    [Flag02]                         BIT                NULL,
    [Flag03]                         BIT                NULL,
    [Flag04]                         BIT                NULL,
    [Flag05]                         BIT                NULL,
    [Flag06]                         BIT                NULL,
    [Number01]                       FLOAT (53)         NULL,
    [Number02]                       FLOAT (53)         NULL,
    [Number03]                       FLOAT (53)         NULL,
    [Number04]                       FLOAT (53)         NULL,
    [Number05]                       FLOAT (53)         NULL,
    [Number06]                       FLOAT (53)         NULL,
    [ContactFirstName]               NVARCHAR (50)      NULL,
    [ContactSurname]                 NVARCHAR (50)      NULL,
    [Date01]                         DATETIMEOFFSET (7) NULL,
    [Date02]                         DATETIMEOFFSET (7) NULL,
    [Date03]                         DATETIMEOFFSET (7) NULL,
    [Date04]                         DATETIMEOFFSET (7) NULL,
    [LegacyNumber]                   NVARCHAR (50)      NULL,
    [Country]                        NVARCHAR (50)      NULL,
    [ContactInfo]                    NVARCHAR (50)      NULL,
    [AssociatedDocNumber]            NVARCHAR (30)      NULL,
    [AssociatedCLIN]                 NVARCHAR (10)      NULL,
    [SubmittedToAccounting]          BIT                CONSTRAINT [DF_tblTransactions_SubmittedToAccounting] DEFAULT ((1)) NULL,
    [FuelCardID]                     NVARCHAR (50)      NULL,
    [AssociatedTransportOrderNumber] NVARCHAR (30)      NULL,
    [RequestedDateTime]              DATETIMEOFFSET (7) NULL,
    [DispatchedDateTime]             DATETIMEOFFSET (7) NULL,
    [ErrorFlag]                      BIT                NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [TransactionGuid]                UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactions_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]                       UNIQUEIDENTIFIER   NULL,
    [LookupTransTypeIndex]           SMALLINT           CONSTRAINT [DF_tblTransactions_LookupTransTypeIndex] DEFAULT ((0)) NOT NULL,
    [LookupTransactionStatusIndex]   INT                NULL,
    [LookupOriginApplicationIndex]   INT                CONSTRAINT [DF_tblTransactions_LookupOriginApplicationIndex] DEFAULT ((2)) NULL,
    [TransactionAliasGuid]           UNIQUEIDENTIFIER   NULL,
    [BillToCompanyGuid]              UNIQUEIDENTIFIER   NULL,
    [Destination1EquipmentGuid]      UNIQUEIDENTIFIER   NULL,
    [Destination2EquipmentGuid]      UNIQUEIDENTIFIER   NULL,
    [Destination3EquipmentGuid]      UNIQUEIDENTIFIER   NULL,
    [FinalStationIATAGuid]           UNIQUEIDENTIFIER   NULL,
    [FuelCardGuid]                   UNIQUEIDENTIFIER   NULL,
    [ManagerCompanyGuid]             UNIQUEIDENTIFIER   NULL,
    [NextStationIATAGuid]            UNIQUEIDENTIFIER   NULL,
    [OperatorPersonnelGuid]          UNIQUEIDENTIFIER   NULL,
    [OriginStationIATAGuid]          UNIQUEIDENTIFIER   NULL,
    [OwnerCompanyGuid]               UNIQUEIDENTIFIER   NULL,
    [PreviousStationIATAGuid]        UNIQUEIDENTIFIER   NULL,
    [ShipperCompanyGuid]             UNIQUEIDENTIFIER   NULL,
    [ShipToCompanyGuid]              UNIQUEIDENTIFIER   NULL,
    [Source1EquipmentGuid]           UNIQUEIDENTIFIER   NULL,
    [Source2EquipmentGuid]           UNIQUEIDENTIFIER   NULL,
    [Source3EquipmentGuid]           UNIQUEIDENTIFIER   NULL,
    [SupplierCompanyGuid]            UNIQUEIDENTIFIER   NULL,
    [CarrierCompanyGuid]             UNIQUEIDENTIFIER   NULL,
    [ReasonCodeGuid]                 UNIQUEIDENTIFIER   NULL,
    [OriginStationIATAID]            NVARCHAR (50)      NULL,
    [PreviousStationIATAID]          NVARCHAR (50)      NULL,
    [NextStationIATAID]              NVARCHAR (50)      NULL,
    [FinalStationIATAID]             NVARCHAR (50)      NULL,
    [OperatorName]                   NVARCHAR (150)     NULL,
    [FuelAdditiveFlag]               BIT                NOT NULL,
    [IssuePoint]                     NVARCHAR (MAX)     NULL,
    [IssuePointNumber]               NVARCHAR (MAX)     NULL,
    [RadioNumber]                    NVARCHAR (MAX)     NULL,
    [GateID]                         NVARCHAR (10)      NULL,
    [GateGuid]                       UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    [ShippingMethod]                 NVARCHAR (150)     NULL,
	[ReferencedTransactionGuid]		 UNIQUEIDENTIFIER	NULL,
    CONSTRAINT [PK_tblTransactions_GUID] PRIMARY KEY NONCLUSTERED ([TransactionGuid] ASC),
    CONSTRAINT [FK_tblTransactions_LookupTransactionOriginIndex] FOREIGN KEY ([LookupOriginApplicationIndex]) REFERENCES [lookup].[tblTransactionOrigin] ([TransactionOriginIndex]),
    CONSTRAINT [FK_tblTransactions_LookupTransactionStatusIndex] FOREIGN KEY ([LookupTransactionStatusIndex]) REFERENCES [lookup].[tblTransactionStatus] ([TransactionStatusIndex]),
    CONSTRAINT [FK_tblTransactions_LookupTransactionTypesIndex] FOREIGN KEY ([LookupTransTypeIndex]) REFERENCES [lookup].[tblTransactionTypes] ([TransactionTypesIndex]),
    CONSTRAINT [FK_tblTransactions_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblTransactions_TransactionAliasGuid] FOREIGN KEY ([TransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactions_ClusterIdx] 
	ON [dbo].[tblTransactions]([_ClusterIdx]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactions_InventoryDate]
    ON [dbo].[tblTransactions]([InventoryDate] ASC)
	INCLUDE( [TransactionGuid], [_RowVersion]);

GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactions_CoveringPreviousVersionInformation]
    ON [dbo].[tblTransactions]([TransactionGuid] ASC)
    INCLUDE([DeleteFlag], [LookupTransactionStatusIndex], [TransVersion], [_RowVersion]  );


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactions_AliasName_SiteGuid_InventoryDate_QueryWriterCoveringIdx]
    ON [dbo].[tblTransactions]([AliasName] ASC, [SiteGuid] ASC, [InventoryDate] ASC)
    INCLUDE([TransID], [Site], [ShipToID], [SupplierID], [CreatedDate], [TransVersion], [ConjoinTransID], [Flag01], [Flag02], [Flag04], [Flag05], [Flag06], [ErrorFlag], [TransactionGuid], [LookupTransTypeIndex], [LookupTransactionStatusIndex], [TransactionAliasGuid]);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactions_DeleteFlag_SiteGuid]
    ON [dbo].[tblTransactions]([DeleteFlag] ASC, [SiteGuid] ASC)
    INCLUDE([TransID], [InventoryDate], [CreatedDate], [Date03], [TransactionGuid]);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactions_DeleteFlag_TransID]
    ON [dbo].[tblTransactions]([DeleteFlag] ASC, [TransID] ASC)
    INCLUDE([InventoryDate], [CreatedDate], [Date03], [TransactionGuid], [SiteGuid]);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactions_LookupTransactionStatusIndex]
    ON [dbo].[tblTransactions]([LookupTransactionStatusIndex] ASC)
    INCLUDE([TransID], [AliasName], [DocumentNumber], [DeleteFlag], [Flag01], [Flag02], [Flag03], [Flag04], [Flag05], [Flag06], [SiteGuid], [LookupTransTypeIndex]);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactions_TransID_ShipToID_OwnerID]
    ON [dbo].[tblTransactions]([TransID] ASC)
    INCLUDE([ShipToID], [OwnerID], [ManagerID], [CarrierID], [BillToID], [TransactionGuid]);

GO
CREATE NONCLUSTERED INDEX [IX_tblTransactions_Paice_Covering]
ON [dbo].[tblTransactions]( [InventoryDate] ASC, [SiteGuid] ASC, [TransactionAliasGuid] ASC, [OwnerCompanyGuid] ASC,  [ManagerCompanyGuid] ASC )
INCLUDE( DATE01, [TransactionGuid], [DeleteFlag] , [TransID],  [UpdatedDate]);


GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_TransactionAliasGuid]
    ON [dbo].[tblTransactions]([TransactionAliasGuid] ASC, [InventoryDate] ASC,  [DeleteFlag] ASC)
	INCLUDE ( ManagerCompanyGuid, OwnerCompanyGuid, SiteGuid);


GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactions_TransID]
    ON [dbo].[tblTransactions]([TransID] ASC, ManagerCompanyGuid ASC)
    INCLUDE([TransactionGuid], [LookupTransTypeIndex], [InventoryDate]);

GO
CREATE NONCLUSTERED INDEX [IXU_tblTransactions_ReversedTransID]
    ON [dbo].[tblTransactions]([ReversedTransID] ASC)
    INCLUDE([TransactionGuid], [SiteGuid], [LookupTransTypeIndex], [InventoryDate]);


GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactions_CoveringAssociatedTransactionQueries] ON [dbo].[tblTransactions]
(
	[TransactionGuid] ASC,
	[TransID] ASC
)
INCLUDE ( 	[SiteGuid],
	[TransactionAliasGuid],
	[LookupTransTypeIndex],
	[AliasName],
	[SubType],
	[Site],
	[TransReferenceID],
	[InventoryDate],
	[ShipToID],
	[ShipToCode],
	[SupplierID]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactions] ON [dbo].[tblTransactions] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactions','D')=1 
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
	INSERT INTO [fmaudit].tblTransactions (
		[TransID]
	,	[AliasName]
	,	[SubType]
	,	[Site]
	,	[TransReferenceID]
	,	[InventoryDate]
	,	[ShipToID]
	,	[ShipToCode]
	,	[SupplierID]
	,	[SupplierCode]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[RequestedDeliveryDate]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TransDateTime]
	,	[TransVersion]
	,	[SCACCode]
	,	[CardNumber]
	,	[ShipmentNumber]
	,	[ShipperID]
	,	[ShipperCode]
	,	[OwnerID]
	,	[OwnerCode]
	,	[ManagerID]
	,	[ManagerCode]
	,	[CarrierID]
	,	[CarrierCode]
	,	[ConjoinTransID]
	,	[ReversedTransID]
	,	[LinkedDocumentNumber]
	,	[ReversalType]
	,	[PONumber]
	,	[TimeIn]
	,	[TimeOut]
	,	[TimeEnd]
	,	[RoutingID]
	,	[TicketSource]
	,	[LoadID]
	,	[BillToID]
	,	[BillToCode]
	,	[DriverIdentificationNumber]
	,	[CreditAmount]
	,	[CardExpiration]
	,	[CardName]
	,	[CardType]
	,	[CashAmount]
	,	[RouteOriginationDate]
	,	[InternationalRouteIndicator]
	,	[PreviousRoutingID]
	,	[ShippingDocumentNumber]
	,	[DocumentNumber]
	,	[STD]
	,	[ETD]
	,	[STA]
	,	[ETA]
	,	[SFT]
	,	[FST]
	,	[EstimatedFuelingDuration]
	,	[DeleteFlag]
	,	[TicketMode]
	,	[DestinationRegistrationID1]
	,	[DestinationSerialNumber1]
	,	[DestinationEquipmentType1]
	,	[DestinationEquipmentModel1]
	,	[DestinationCompanyEquipmentID1]
	,	[DestinationRegistrationID2]
	,	[DestinationSerialNumber2]
	,	[DestinationEquipmentType2]
	,	[DestinationEquipmentModel2]
	,	[DestinationCompanyEquipmentID2]
	,	[DestinationRegistrationID3]
	,	[DestinationSerialNumber3]
	,	[DestinationEquipmentType3]
	,	[DestinationEquipmentModel3]
	,	[DestinationCompanyEquipmentID3]
	,	[SourceRegistrationID1]
	,	[SourceSerialNumber1]
	,	[SourceEquipmentType1]
	,	[SourceEquipmentModel1]
	,	[SourceCompanyEquipmentID1]
	,	[SourceRegistrationID2]
	,	[SourceSerialNumber2]
	,	[SourceEquipmentType2]
	,	[SourceEquipmentModel2]
	,	[SourceCompanyEquipmentID2]
	,	[SourceRegistrationID3]
	,	[SourceSerialNumber3]
	,	[SourceEquipmentType3]
	,	[SourceEquipmentModel3]
	,	[SourceCompanyEquipmentID3]
	,	[OperatorID]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[ScheduledDate]
	,	[AutoComplete]
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
	,	[ContactFirstName]
	,	[ContactSurname]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[LegacyNumber]
	,	[Country]
	,	[ContactInfo]
	,	[AssociatedDocNumber]
	,	[AssociatedCLIN]
	,	[SubmittedToAccounting]
	,	[FuelCardID]
	,	[AssociatedTransportOrderNumber]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[ErrorFlag]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupTransactionStatusIndex]
	,	[LookupOriginApplicationIndex]
	,	[TransactionAliasGuid]
	,	[BillToCompanyGuid]
	,	[Destination1EquipmentGuid]
	,	[Destination2EquipmentGuid]
	,	[Destination3EquipmentGuid]
	,	[FinalStationIATAGuid]
	,	[FuelCardGuid]
	,	[ManagerCompanyGuid]
	,	[NextStationIATAGuid]
	,	[OperatorPersonnelGuid]
	,	[OriginStationIATAGuid]
	,	[OwnerCompanyGuid]
	,	[PreviousStationIATAGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[Source1EquipmentGuid]
	,	[Source2EquipmentGuid]
	,	[Source3EquipmentGuid]
	,	[SupplierCompanyGuid]
	,	[CarrierCompanyGuid]
	,	[ReasonCodeGuid]
	,	[OriginStationIATAID]
	,	[PreviousStationIATAID]
	,	[NextStationIATAID]
	,	[FinalStationIATAID]
	,	[OperatorName]
	,	[FuelAdditiveFlag]
	,	[IssuePoint]
	,	[IssuePointNumber]
	,	[RadioNumber]
	,	[GateID]
	,	[GateGuid]
	,	[ShippingMethod]
	,	[ReferencedTransactionGuid]
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
		d.[TransID]
	,	d.[AliasName]
	,	d.[SubType]
	,	d.[Site]
	,	d.[TransReferenceID]
	,	d.[InventoryDate]
	,	d.[ShipToID]
	,	d.[ShipToCode]
	,	d.[SupplierID]
	,	d.[SupplierCode]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[RequestedDeliveryDate]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TransDateTime]
	,	d.[TransVersion]
	,	d.[SCACCode]
	,	d.[CardNumber]
	,	d.[ShipmentNumber]
	,	d.[ShipperID]
	,	d.[ShipperCode]
	,	d.[OwnerID]
	,	d.[OwnerCode]
	,	d.[ManagerID]
	,	d.[ManagerCode]
	,	d.[CarrierID]
	,	d.[CarrierCode]
	,	d.[ConjoinTransID]
	,	d.[ReversedTransID]
	,	d.[LinkedDocumentNumber]
	,	d.[ReversalType]
	,	d.[PONumber]
	,	d.[TimeIn]
	,	d.[TimeOut]
	,	d.[TimeEnd]
	,	d.[RoutingID]
	,	d.[TicketSource]
	,	d.[LoadID]
	,	d.[BillToID]
	,	d.[BillToCode]
	,	d.[DriverIdentificationNumber]
	,	d.[CreditAmount]
	,	d.[CardExpiration]
	,	d.[CardName]
	,	d.[CardType]
	,	d.[CashAmount]
	,	d.[RouteOriginationDate]
	,	d.[InternationalRouteIndicator]
	,	d.[PreviousRoutingID]
	,	d.[ShippingDocumentNumber]
	,	d.[DocumentNumber]
	,	d.[STD]
	,	d.[ETD]
	,	d.[STA]
	,	d.[ETA]
	,	d.[SFT]
	,	d.[FST]
	,	d.[EstimatedFuelingDuration]
	,	d.[DeleteFlag]
	,	d.[TicketMode]
	,	d.[DestinationRegistrationID1]
	,	d.[DestinationSerialNumber1]
	,	d.[DestinationEquipmentType1]
	,	d.[DestinationEquipmentModel1]
	,	d.[DestinationCompanyEquipmentID1]
	,	d.[DestinationRegistrationID2]
	,	d.[DestinationSerialNumber2]
	,	d.[DestinationEquipmentType2]
	,	d.[DestinationEquipmentModel2]
	,	d.[DestinationCompanyEquipmentID2]
	,	d.[DestinationRegistrationID3]
	,	d.[DestinationSerialNumber3]
	,	d.[DestinationEquipmentType3]
	,	d.[DestinationEquipmentModel3]
	,	d.[DestinationCompanyEquipmentID3]
	,	d.[SourceRegistrationID1]
	,	d.[SourceSerialNumber1]
	,	d.[SourceEquipmentType1]
	,	d.[SourceEquipmentModel1]
	,	d.[SourceCompanyEquipmentID1]
	,	d.[SourceRegistrationID2]
	,	d.[SourceSerialNumber2]
	,	d.[SourceEquipmentType2]
	,	d.[SourceEquipmentModel2]
	,	d.[SourceCompanyEquipmentID2]
	,	d.[SourceRegistrationID3]
	,	d.[SourceSerialNumber3]
	,	d.[SourceEquipmentType3]
	,	d.[SourceEquipmentModel3]
	,	d.[SourceCompanyEquipmentID3]
	,	d.[OperatorID]
	,	d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[ScheduledDate]
	,	d.[AutoComplete]
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
	,	d.[ContactFirstName]
	,	d.[ContactSurname]
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[LegacyNumber]
	,	d.[Country]
	,	d.[ContactInfo]
	,	d.[AssociatedDocNumber]
	,	d.[AssociatedCLIN]
	,	d.[SubmittedToAccounting]
	,	d.[FuelCardID]
	,	d.[AssociatedTransportOrderNumber]
	,	d.[RequestedDateTime]
	,	d.[DispatchedDateTime]
	,	d.[ErrorFlag]
	,	d.[_RowVersion]
	,	d.[TransactionGuid]
	,	d.[SiteGuid]
	,	d.[LookupTransTypeIndex]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupOriginApplicationIndex]
	,	d.[TransactionAliasGuid]
	,	d.[BillToCompanyGuid]
	,	d.[Destination1EquipmentGuid]
	,	d.[Destination2EquipmentGuid]
	,	d.[Destination3EquipmentGuid]
	,	d.[FinalStationIATAGuid]
	,	d.[FuelCardGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[NextStationIATAGuid]
	,	d.[OperatorPersonnelGuid]
	,	d.[OriginStationIATAGuid]
	,	d.[OwnerCompanyGuid]
	,	d.[PreviousStationIATAGuid]
	,	d.[ShipperCompanyGuid]
	,	d.[ShipToCompanyGuid]
	,	d.[Source1EquipmentGuid]
	,	d.[Source2EquipmentGuid]
	,	d.[Source3EquipmentGuid]
	,	d.[SupplierCompanyGuid]
	,	d.[CarrierCompanyGuid]
	,	d.[ReasonCodeGuid]
	,	d.[OriginStationIATAID]
	,	d.[PreviousStationIATAID]
	,	d.[NextStationIATAID]
	,	d.[FinalStationIATAID]
	,	d.[OperatorName]
	,	d.[FuelAdditiveFlag]
	,	d.[IssuePoint]
	,	d.[IssuePointNumber]
	,	d.[RadioNumber]
	,	d.[GateID]
	,	d.[GateGuid]
	,	d.[ShippingMethod]
	,	d.[ReferencedTransactionGuid]
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
--Creating Insert / Update Trigger for tblTransactions
CREATE TRIGGER dbo.trg_insupd_tblTransactions_ForSync 
   ON dbo.tblTransactions
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
                    ,d.TransactionGuid AS Deleted_PK_TransactionGuid
                    ,i.TransactionGuid AS Inserted_PK_TransactionGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TransactionGuid = i.TransactionGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactions As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionGuid = currentTrackingData.PK_TransactionGuid
 
 
		    INSERT track.tblTransactions (InsertedDate 
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
				    ,PK_TransactionGuid
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
				    ,entityChanges.Inserted_PK_TransactionGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactions As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionGuid = currentTrackingData.PK_TransactionGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactions
CREATE TRIGGER dbo.trg_del_tblTransactions_ForSync 
   ON dbo.tblTransactions
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
						,d.TransactionGuid AS Deleted_PK_TransactionGuid
                        ,d.TransactionGuid AS Inserted_PK_TransactionGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactions As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionGuid = currentTrackingData.PK_TransactionGuid
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
						,PK_TransactionGuid
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
						,entityChanges.Deleted_PK_TransactionGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactions] ON [dbo].[tblTransactions] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactions','D')=1 
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
	INSERT INTO [fmaudit].tblTransactions (
		[TransID]
	,	[AliasName]
	,	[SubType]
	,	[Site]
	,	[TransReferenceID]
	,	[InventoryDate]
	,	[ShipToID]
	,	[ShipToCode]
	,	[SupplierID]
	,	[SupplierCode]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[RequestedDeliveryDate]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TransDateTime]
	,	[TransVersion]
	,	[SCACCode]
	,	[CardNumber]
	,	[ShipmentNumber]
	,	[ShipperID]
	,	[ShipperCode]
	,	[OwnerID]
	,	[OwnerCode]
	,	[ManagerID]
	,	[ManagerCode]
	,	[CarrierID]
	,	[CarrierCode]
	,	[ConjoinTransID]
	,	[ReversedTransID]
	,	[LinkedDocumentNumber]
	,	[ReversalType]
	,	[PONumber]
	,	[TimeIn]
	,	[TimeOut]
	,	[TimeEnd]
	,	[RoutingID]
	,	[TicketSource]
	,	[LoadID]
	,	[BillToID]
	,	[BillToCode]
	,	[DriverIdentificationNumber]
	,	[CreditAmount]
	,	[CardExpiration]
	,	[CardName]
	,	[CardType]
	,	[CashAmount]
	,	[RouteOriginationDate]
	,	[InternationalRouteIndicator]
	,	[PreviousRoutingID]
	,	[ShippingDocumentNumber]
	,	[DocumentNumber]
	,	[STD]
	,	[ETD]
	,	[STA]
	,	[ETA]
	,	[SFT]
	,	[FST]
	,	[EstimatedFuelingDuration]
	,	[DeleteFlag]
	,	[TicketMode]
	,	[DestinationRegistrationID1]
	,	[DestinationSerialNumber1]
	,	[DestinationEquipmentType1]
	,	[DestinationEquipmentModel1]
	,	[DestinationCompanyEquipmentID1]
	,	[DestinationRegistrationID2]
	,	[DestinationSerialNumber2]
	,	[DestinationEquipmentType2]
	,	[DestinationEquipmentModel2]
	,	[DestinationCompanyEquipmentID2]
	,	[DestinationRegistrationID3]
	,	[DestinationSerialNumber3]
	,	[DestinationEquipmentType3]
	,	[DestinationEquipmentModel3]
	,	[DestinationCompanyEquipmentID3]
	,	[SourceRegistrationID1]
	,	[SourceSerialNumber1]
	,	[SourceEquipmentType1]
	,	[SourceEquipmentModel1]
	,	[SourceCompanyEquipmentID1]
	,	[SourceRegistrationID2]
	,	[SourceSerialNumber2]
	,	[SourceEquipmentType2]
	,	[SourceEquipmentModel2]
	,	[SourceCompanyEquipmentID2]
	,	[SourceRegistrationID3]
	,	[SourceSerialNumber3]
	,	[SourceEquipmentType3]
	,	[SourceEquipmentModel3]
	,	[SourceCompanyEquipmentID3]
	,	[OperatorID]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[ScheduledDate]
	,	[AutoComplete]
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
	,	[ContactFirstName]
	,	[ContactSurname]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[LegacyNumber]
	,	[Country]
	,	[ContactInfo]
	,	[AssociatedDocNumber]
	,	[AssociatedCLIN]
	,	[SubmittedToAccounting]
	,	[FuelCardID]
	,	[AssociatedTransportOrderNumber]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[ErrorFlag]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupTransactionStatusIndex]
	,	[LookupOriginApplicationIndex]
	,	[TransactionAliasGuid]
	,	[BillToCompanyGuid]
	,	[Destination1EquipmentGuid]
	,	[Destination2EquipmentGuid]
	,	[Destination3EquipmentGuid]
	,	[FinalStationIATAGuid]
	,	[FuelCardGuid]
	,	[ManagerCompanyGuid]
	,	[NextStationIATAGuid]
	,	[OperatorPersonnelGuid]
	,	[OriginStationIATAGuid]
	,	[OwnerCompanyGuid]
	,	[PreviousStationIATAGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[Source1EquipmentGuid]
	,	[Source2EquipmentGuid]
	,	[Source3EquipmentGuid]
	,	[SupplierCompanyGuid]
	,	[CarrierCompanyGuid]
	,	[ReasonCodeGuid]
	,	[OriginStationIATAID]
	,	[PreviousStationIATAID]
	,	[NextStationIATAID]
	,	[FinalStationIATAID]
	,	[OperatorName]
	,	[FuelAdditiveFlag]
	,	[IssuePoint]
	,	[IssuePointNumber]
	,	[RadioNumber]
	,	[GateID]
	,	[GateGuid]
	,	[ShippingMethod]
	,	[ReferencedTransactionGuid]
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
		i.[TransID]
	,	i.[AliasName]
	,	i.[SubType]
	,	i.[Site]
	,	i.[TransReferenceID]
	,	i.[InventoryDate]
	,	i.[ShipToID]
	,	i.[ShipToCode]
	,	i.[SupplierID]
	,	i.[SupplierCode]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[RequestedDeliveryDate]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TransDateTime]
	,	i.[TransVersion]
	,	i.[SCACCode]
	,	i.[CardNumber]
	,	i.[ShipmentNumber]
	,	i.[ShipperID]
	,	i.[ShipperCode]
	,	i.[OwnerID]
	,	i.[OwnerCode]
	,	i.[ManagerID]
	,	i.[ManagerCode]
	,	i.[CarrierID]
	,	i.[CarrierCode]
	,	i.[ConjoinTransID]
	,	i.[ReversedTransID]
	,	i.[LinkedDocumentNumber]
	,	i.[ReversalType]
	,	i.[PONumber]
	,	i.[TimeIn]
	,	i.[TimeOut]
	,	i.[TimeEnd]
	,	i.[RoutingID]
	,	i.[TicketSource]
	,	i.[LoadID]
	,	i.[BillToID]
	,	i.[BillToCode]
	,	i.[DriverIdentificationNumber]
	,	i.[CreditAmount]
	,	i.[CardExpiration]
	,	i.[CardName]
	,	i.[CardType]
	,	i.[CashAmount]
	,	i.[RouteOriginationDate]
	,	i.[InternationalRouteIndicator]
	,	i.[PreviousRoutingID]
	,	i.[ShippingDocumentNumber]
	,	i.[DocumentNumber]
	,	i.[STD]
	,	i.[ETD]
	,	i.[STA]
	,	i.[ETA]
	,	i.[SFT]
	,	i.[FST]
	,	i.[EstimatedFuelingDuration]
	,	i.[DeleteFlag]
	,	i.[TicketMode]
	,	i.[DestinationRegistrationID1]
	,	i.[DestinationSerialNumber1]
	,	i.[DestinationEquipmentType1]
	,	i.[DestinationEquipmentModel1]
	,	i.[DestinationCompanyEquipmentID1]
	,	i.[DestinationRegistrationID2]
	,	i.[DestinationSerialNumber2]
	,	i.[DestinationEquipmentType2]
	,	i.[DestinationEquipmentModel2]
	,	i.[DestinationCompanyEquipmentID2]
	,	i.[DestinationRegistrationID3]
	,	i.[DestinationSerialNumber3]
	,	i.[DestinationEquipmentType3]
	,	i.[DestinationEquipmentModel3]
	,	i.[DestinationCompanyEquipmentID3]
	,	i.[SourceRegistrationID1]
	,	i.[SourceSerialNumber1]
	,	i.[SourceEquipmentType1]
	,	i.[SourceEquipmentModel1]
	,	i.[SourceCompanyEquipmentID1]
	,	i.[SourceRegistrationID2]
	,	i.[SourceSerialNumber2]
	,	i.[SourceEquipmentType2]
	,	i.[SourceEquipmentModel2]
	,	i.[SourceCompanyEquipmentID2]
	,	i.[SourceRegistrationID3]
	,	i.[SourceSerialNumber3]
	,	i.[SourceEquipmentType3]
	,	i.[SourceEquipmentModel3]
	,	i.[SourceCompanyEquipmentID3]
	,	i.[OperatorID]
	,	i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[ScheduledDate]
	,	i.[AutoComplete]
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
	,	i.[ContactFirstName]
	,	i.[ContactSurname]
	,	i.[Date01]
	,	i.[Date02]
	,	i.[Date03]
	,	i.[Date04]
	,	i.[LegacyNumber]
	,	i.[Country]
	,	i.[ContactInfo]
	,	i.[AssociatedDocNumber]
	,	i.[AssociatedCLIN]
	,	i.[SubmittedToAccounting]
	,	i.[FuelCardID]
	,	i.[AssociatedTransportOrderNumber]
	,	i.[RequestedDateTime]
	,	i.[DispatchedDateTime]
	,	i.[ErrorFlag]
	,	i.[_RowVersion]
	,	i.[TransactionGuid]
	,	i.[SiteGuid]
	,	i.[LookupTransTypeIndex]
	,	i.[LookupTransactionStatusIndex]
	,	i.[LookupOriginApplicationIndex]
	,	i.[TransactionAliasGuid]
	,	i.[BillToCompanyGuid]
	,	i.[Destination1EquipmentGuid]
	,	i.[Destination2EquipmentGuid]
	,	i.[Destination3EquipmentGuid]
	,	i.[FinalStationIATAGuid]
	,	i.[FuelCardGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[NextStationIATAGuid]
	,	i.[OperatorPersonnelGuid]
	,	i.[OriginStationIATAGuid]
	,	i.[OwnerCompanyGuid]
	,	i.[PreviousStationIATAGuid]
	,	i.[ShipperCompanyGuid]
	,	i.[ShipToCompanyGuid]
	,	i.[Source1EquipmentGuid]
	,	i.[Source2EquipmentGuid]
	,	i.[Source3EquipmentGuid]
	,	i.[SupplierCompanyGuid]
	,	i.[CarrierCompanyGuid]
	,	i.[ReasonCodeGuid]
	,	i.[OriginStationIATAID]
	,	i.[PreviousStationIATAID]
	,	i.[NextStationIATAID]
	,	i.[FinalStationIATAID]
	,	i.[OperatorName]
	,	i.[FuelAdditiveFlag]
	,	i.[IssuePoint]
	,	i.[IssuePointNumber]
	,	i.[RadioNumber]
	,	i.[GateID]
	,	i.[GateGuid]
	,	i.[ShippingMethod]
	,	i.[ReferencedTransactionGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactions] ON [dbo].[tblTransactions] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactions','D')=1 
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
	TransactionGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactions (
		[TransID]
	,	[AliasName]
	,	[SubType]
	,	[Site]
	,	[TransReferenceID]
	,	[InventoryDate]
	,	[ShipToID]
	,	[ShipToCode]
	,	[SupplierID]
	,	[SupplierCode]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[RequestedDeliveryDate]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TransDateTime]
	,	[TransVersion]
	,	[SCACCode]
	,	[CardNumber]
	,	[ShipmentNumber]
	,	[ShipperID]
	,	[ShipperCode]
	,	[OwnerID]
	,	[OwnerCode]
	,	[ManagerID]
	,	[ManagerCode]
	,	[CarrierID]
	,	[CarrierCode]
	,	[ConjoinTransID]
	,	[ReversedTransID]
	,	[LinkedDocumentNumber]
	,	[ReversalType]
	,	[PONumber]
	,	[TimeIn]
	,	[TimeOut]
	,	[TimeEnd]
	,	[RoutingID]
	,	[TicketSource]
	,	[LoadID]
	,	[BillToID]
	,	[BillToCode]
	,	[DriverIdentificationNumber]
	,	[CreditAmount]
	,	[CardExpiration]
	,	[CardName]
	,	[CardType]
	,	[CashAmount]
	,	[RouteOriginationDate]
	,	[InternationalRouteIndicator]
	,	[PreviousRoutingID]
	,	[ShippingDocumentNumber]
	,	[DocumentNumber]
	,	[STD]
	,	[ETD]
	,	[STA]
	,	[ETA]
	,	[SFT]
	,	[FST]
	,	[EstimatedFuelingDuration]
	,	[DeleteFlag]
	,	[TicketMode]
	,	[DestinationRegistrationID1]
	,	[DestinationSerialNumber1]
	,	[DestinationEquipmentType1]
	,	[DestinationEquipmentModel1]
	,	[DestinationCompanyEquipmentID1]
	,	[DestinationRegistrationID2]
	,	[DestinationSerialNumber2]
	,	[DestinationEquipmentType2]
	,	[DestinationEquipmentModel2]
	,	[DestinationCompanyEquipmentID2]
	,	[DestinationRegistrationID3]
	,	[DestinationSerialNumber3]
	,	[DestinationEquipmentType3]
	,	[DestinationEquipmentModel3]
	,	[DestinationCompanyEquipmentID3]
	,	[SourceRegistrationID1]
	,	[SourceSerialNumber1]
	,	[SourceEquipmentType1]
	,	[SourceEquipmentModel1]
	,	[SourceCompanyEquipmentID1]
	,	[SourceRegistrationID2]
	,	[SourceSerialNumber2]
	,	[SourceEquipmentType2]
	,	[SourceEquipmentModel2]
	,	[SourceCompanyEquipmentID2]
	,	[SourceRegistrationID3]
	,	[SourceSerialNumber3]
	,	[SourceEquipmentType3]
	,	[SourceEquipmentModel3]
	,	[SourceCompanyEquipmentID3]
	,	[OperatorID]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[ScheduledDate]
	,	[AutoComplete]
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
	,	[ContactFirstName]
	,	[ContactSurname]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[LegacyNumber]
	,	[Country]
	,	[ContactInfo]
	,	[AssociatedDocNumber]
	,	[AssociatedCLIN]
	,	[SubmittedToAccounting]
	,	[FuelCardID]
	,	[AssociatedTransportOrderNumber]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[ErrorFlag]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupTransactionStatusIndex]
	,	[LookupOriginApplicationIndex]
	,	[TransactionAliasGuid]
	,	[BillToCompanyGuid]
	,	[Destination1EquipmentGuid]
	,	[Destination2EquipmentGuid]
	,	[Destination3EquipmentGuid]
	,	[FinalStationIATAGuid]
	,	[FuelCardGuid]
	,	[ManagerCompanyGuid]
	,	[NextStationIATAGuid]
	,	[OperatorPersonnelGuid]
	,	[OriginStationIATAGuid]
	,	[OwnerCompanyGuid]
	,	[PreviousStationIATAGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[Source1EquipmentGuid]
	,	[Source2EquipmentGuid]
	,	[Source3EquipmentGuid]
	,	[SupplierCompanyGuid]
	,	[CarrierCompanyGuid]
	,	[ReasonCodeGuid]
	,	[OriginStationIATAID]
	,	[PreviousStationIATAID]
	,	[NextStationIATAID]
	,	[FinalStationIATAID]
	,	[OperatorName]
	,	[FuelAdditiveFlag]
	,	[IssuePoint]
	,	[IssuePointNumber]
	,	[RadioNumber]
	,	[GateID]
	,	[GateGuid]
	,	[ShippingMethod]
	,	[ReferencedTransactionGuid]
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
	OUTPUT inserted.[TransactionGuid] AS 'TransactionGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[TransID]
	,	d.[AliasName]
	,	d.[SubType]
	,	d.[Site]
	,	d.[TransReferenceID]
	,	d.[InventoryDate]
	,	d.[ShipToID]
	,	d.[ShipToCode]
	,	d.[SupplierID]
	,	d.[SupplierCode]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[RequestedDeliveryDate]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TransDateTime]
	,	d.[TransVersion]
	,	d.[SCACCode]
	,	d.[CardNumber]
	,	d.[ShipmentNumber]
	,	d.[ShipperID]
	,	d.[ShipperCode]
	,	d.[OwnerID]
	,	d.[OwnerCode]
	,	d.[ManagerID]
	,	d.[ManagerCode]
	,	d.[CarrierID]
	,	d.[CarrierCode]
	,	d.[ConjoinTransID]
	,	d.[ReversedTransID]
	,	d.[LinkedDocumentNumber]
	,	d.[ReversalType]
	,	d.[PONumber]
	,	d.[TimeIn]
	,	d.[TimeOut]
	,	d.[TimeEnd]
	,	d.[RoutingID]
	,	d.[TicketSource]
	,	d.[LoadID]
	,	d.[BillToID]
	,	d.[BillToCode]
	,	d.[DriverIdentificationNumber]
	,	d.[CreditAmount]
	,	d.[CardExpiration]
	,	d.[CardName]
	,	d.[CardType]
	,	d.[CashAmount]
	,	d.[RouteOriginationDate]
	,	d.[InternationalRouteIndicator]
	,	d.[PreviousRoutingID]
	,	d.[ShippingDocumentNumber]
	,	d.[DocumentNumber]
	,	d.[STD]
	,	d.[ETD]
	,	d.[STA]
	,	d.[ETA]
	,	d.[SFT]
	,	d.[FST]
	,	d.[EstimatedFuelingDuration]
	,	d.[DeleteFlag]
	,	d.[TicketMode]
	,	d.[DestinationRegistrationID1]
	,	d.[DestinationSerialNumber1]
	,	d.[DestinationEquipmentType1]
	,	d.[DestinationEquipmentModel1]
	,	d.[DestinationCompanyEquipmentID1]
	,	d.[DestinationRegistrationID2]
	,	d.[DestinationSerialNumber2]
	,	d.[DestinationEquipmentType2]
	,	d.[DestinationEquipmentModel2]
	,	d.[DestinationCompanyEquipmentID2]
	,	d.[DestinationRegistrationID3]
	,	d.[DestinationSerialNumber3]
	,	d.[DestinationEquipmentType3]
	,	d.[DestinationEquipmentModel3]
	,	d.[DestinationCompanyEquipmentID3]
	,	d.[SourceRegistrationID1]
	,	d.[SourceSerialNumber1]
	,	d.[SourceEquipmentType1]
	,	d.[SourceEquipmentModel1]
	,	d.[SourceCompanyEquipmentID1]
	,	d.[SourceRegistrationID2]
	,	d.[SourceSerialNumber2]
	,	d.[SourceEquipmentType2]
	,	d.[SourceEquipmentModel2]
	,	d.[SourceCompanyEquipmentID2]
	,	d.[SourceRegistrationID3]
	,	d.[SourceSerialNumber3]
	,	d.[SourceEquipmentType3]
	,	d.[SourceEquipmentModel3]
	,	d.[SourceCompanyEquipmentID3]
	,	d.[OperatorID]
	,	d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[ScheduledDate]
	,	d.[AutoComplete]
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
	,	d.[ContactFirstName]
	,	d.[ContactSurname]
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[LegacyNumber]
	,	d.[Country]
	,	d.[ContactInfo]
	,	d.[AssociatedDocNumber]
	,	d.[AssociatedCLIN]
	,	d.[SubmittedToAccounting]
	,	d.[FuelCardID]
	,	d.[AssociatedTransportOrderNumber]
	,	d.[RequestedDateTime]
	,	d.[DispatchedDateTime]
	,	d.[ErrorFlag]
	,	d.[_RowVersion]
	,	d.[TransactionGuid]
	,	d.[SiteGuid]
	,	d.[LookupTransTypeIndex]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupOriginApplicationIndex]
	,	d.[TransactionAliasGuid]
	,	d.[BillToCompanyGuid]
	,	d.[Destination1EquipmentGuid]
	,	d.[Destination2EquipmentGuid]
	,	d.[Destination3EquipmentGuid]
	,	d.[FinalStationIATAGuid]
	,	d.[FuelCardGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[NextStationIATAGuid]
	,	d.[OperatorPersonnelGuid]
	,	d.[OriginStationIATAGuid]
	,	d.[OwnerCompanyGuid]
	,	d.[PreviousStationIATAGuid]
	,	d.[ShipperCompanyGuid]
	,	d.[ShipToCompanyGuid]
	,	d.[Source1EquipmentGuid]
	,	d.[Source2EquipmentGuid]
	,	d.[Source3EquipmentGuid]
	,	d.[SupplierCompanyGuid]
	,	d.[CarrierCompanyGuid]
	,	d.[ReasonCodeGuid]
	,	d.[OriginStationIATAID]
	,	d.[PreviousStationIATAID]
	,	d.[NextStationIATAID]
	,	d.[FinalStationIATAID]
	,	d.[OperatorName]
	,	d.[FuelAdditiveFlag]
	,	d.[IssuePoint]
	,	d.[IssuePointNumber]
	,	d.[RadioNumber]
	,	d.[GateID]
	,	d.[GateGuid]
	,	d.[ShippingMethod]
	,	d.[ReferencedTransactionGuid]
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
 
	INSERT INTO [fmaudit].tblTransactions (
		[TransID]
	,	[AliasName]
	,	[SubType]
	,	[Site]
	,	[TransReferenceID]
	,	[InventoryDate]
	,	[ShipToID]
	,	[ShipToCode]
	,	[SupplierID]
	,	[SupplierCode]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[RequestedDeliveryDate]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TransDateTime]
	,	[TransVersion]
	,	[SCACCode]
	,	[CardNumber]
	,	[ShipmentNumber]
	,	[ShipperID]
	,	[ShipperCode]
	,	[OwnerID]
	,	[OwnerCode]
	,	[ManagerID]
	,	[ManagerCode]
	,	[CarrierID]
	,	[CarrierCode]
	,	[ConjoinTransID]
	,	[ReversedTransID]
	,	[LinkedDocumentNumber]
	,	[ReversalType]
	,	[PONumber]
	,	[TimeIn]
	,	[TimeOut]
	,	[TimeEnd]
	,	[RoutingID]
	,	[TicketSource]
	,	[LoadID]
	,	[BillToID]
	,	[BillToCode]
	,	[DriverIdentificationNumber]
	,	[CreditAmount]
	,	[CardExpiration]
	,	[CardName]
	,	[CardType]
	,	[CashAmount]
	,	[RouteOriginationDate]
	,	[InternationalRouteIndicator]
	,	[PreviousRoutingID]
	,	[ShippingDocumentNumber]
	,	[DocumentNumber]
	,	[STD]
	,	[ETD]
	,	[STA]
	,	[ETA]
	,	[SFT]
	,	[FST]
	,	[EstimatedFuelingDuration]
	,	[DeleteFlag]
	,	[TicketMode]
	,	[DestinationRegistrationID1]
	,	[DestinationSerialNumber1]
	,	[DestinationEquipmentType1]
	,	[DestinationEquipmentModel1]
	,	[DestinationCompanyEquipmentID1]
	,	[DestinationRegistrationID2]
	,	[DestinationSerialNumber2]
	,	[DestinationEquipmentType2]
	,	[DestinationEquipmentModel2]
	,	[DestinationCompanyEquipmentID2]
	,	[DestinationRegistrationID3]
	,	[DestinationSerialNumber3]
	,	[DestinationEquipmentType3]
	,	[DestinationEquipmentModel3]
	,	[DestinationCompanyEquipmentID3]
	,	[SourceRegistrationID1]
	,	[SourceSerialNumber1]
	,	[SourceEquipmentType1]
	,	[SourceEquipmentModel1]
	,	[SourceCompanyEquipmentID1]
	,	[SourceRegistrationID2]
	,	[SourceSerialNumber2]
	,	[SourceEquipmentType2]
	,	[SourceEquipmentModel2]
	,	[SourceCompanyEquipmentID2]
	,	[SourceRegistrationID3]
	,	[SourceSerialNumber3]
	,	[SourceEquipmentType3]
	,	[SourceEquipmentModel3]
	,	[SourceCompanyEquipmentID3]
	,	[OperatorID]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[ScheduledDate]
	,	[AutoComplete]
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
	,	[ContactFirstName]
	,	[ContactSurname]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[LegacyNumber]
	,	[Country]
	,	[ContactInfo]
	,	[AssociatedDocNumber]
	,	[AssociatedCLIN]
	,	[SubmittedToAccounting]
	,	[FuelCardID]
	,	[AssociatedTransportOrderNumber]
	,	[RequestedDateTime]
	,	[DispatchedDateTime]
	,	[ErrorFlag]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupTransactionStatusIndex]
	,	[LookupOriginApplicationIndex]
	,	[TransactionAliasGuid]
	,	[BillToCompanyGuid]
	,	[Destination1EquipmentGuid]
	,	[Destination2EquipmentGuid]
	,	[Destination3EquipmentGuid]
	,	[FinalStationIATAGuid]
	,	[FuelCardGuid]
	,	[ManagerCompanyGuid]
	,	[NextStationIATAGuid]
	,	[OperatorPersonnelGuid]
	,	[OriginStationIATAGuid]
	,	[OwnerCompanyGuid]
	,	[PreviousStationIATAGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[Source1EquipmentGuid]
	,	[Source2EquipmentGuid]
	,	[Source3EquipmentGuid]
	,	[SupplierCompanyGuid]
	,	[CarrierCompanyGuid]
	,	[ReasonCodeGuid]
	,	[OriginStationIATAID]
	,	[PreviousStationIATAID]
	,	[NextStationIATAID]
	,	[FinalStationIATAID]
	,	[OperatorName]
	,	[FuelAdditiveFlag]
	,	[IssuePoint]
	,	[IssuePointNumber]
	,	[RadioNumber]
	,	[GateID]
	,	[GateGuid]
	,	[ShippingMethod]
	,	[ReferencedTransactionGuid]
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
		i.[TransID]
	,	i.[AliasName]
	,	i.[SubType]
	,	i.[Site]
	,	i.[TransReferenceID]
	,	i.[InventoryDate]
	,	i.[ShipToID]
	,	i.[ShipToCode]
	,	i.[SupplierID]
	,	i.[SupplierCode]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[RequestedDeliveryDate]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TransDateTime]
	,	i.[TransVersion]
	,	i.[SCACCode]
	,	i.[CardNumber]
	,	i.[ShipmentNumber]
	,	i.[ShipperID]
	,	i.[ShipperCode]
	,	i.[OwnerID]
	,	i.[OwnerCode]
	,	i.[ManagerID]
	,	i.[ManagerCode]
	,	i.[CarrierID]
	,	i.[CarrierCode]
	,	i.[ConjoinTransID]
	,	i.[ReversedTransID]
	,	i.[LinkedDocumentNumber]
	,	i.[ReversalType]
	,	i.[PONumber]
	,	i.[TimeIn]
	,	i.[TimeOut]
	,	i.[TimeEnd]
	,	i.[RoutingID]
	,	i.[TicketSource]
	,	i.[LoadID]
	,	i.[BillToID]
	,	i.[BillToCode]
	,	i.[DriverIdentificationNumber]
	,	i.[CreditAmount]
	,	i.[CardExpiration]
	,	i.[CardName]
	,	i.[CardType]
	,	i.[CashAmount]
	,	i.[RouteOriginationDate]
	,	i.[InternationalRouteIndicator]
	,	i.[PreviousRoutingID]
	,	i.[ShippingDocumentNumber]
	,	i.[DocumentNumber]
	,	i.[STD]
	,	i.[ETD]
	,	i.[STA]
	,	i.[ETA]
	,	i.[SFT]
	,	i.[FST]
	,	i.[EstimatedFuelingDuration]
	,	i.[DeleteFlag]
	,	i.[TicketMode]
	,	i.[DestinationRegistrationID1]
	,	i.[DestinationSerialNumber1]
	,	i.[DestinationEquipmentType1]
	,	i.[DestinationEquipmentModel1]
	,	i.[DestinationCompanyEquipmentID1]
	,	i.[DestinationRegistrationID2]
	,	i.[DestinationSerialNumber2]
	,	i.[DestinationEquipmentType2]
	,	i.[DestinationEquipmentModel2]
	,	i.[DestinationCompanyEquipmentID2]
	,	i.[DestinationRegistrationID3]
	,	i.[DestinationSerialNumber3]
	,	i.[DestinationEquipmentType3]
	,	i.[DestinationEquipmentModel3]
	,	i.[DestinationCompanyEquipmentID3]
	,	i.[SourceRegistrationID1]
	,	i.[SourceSerialNumber1]
	,	i.[SourceEquipmentType1]
	,	i.[SourceEquipmentModel1]
	,	i.[SourceCompanyEquipmentID1]
	,	i.[SourceRegistrationID2]
	,	i.[SourceSerialNumber2]
	,	i.[SourceEquipmentType2]
	,	i.[SourceEquipmentModel2]
	,	i.[SourceCompanyEquipmentID2]
	,	i.[SourceRegistrationID3]
	,	i.[SourceSerialNumber3]
	,	i.[SourceEquipmentType3]
	,	i.[SourceEquipmentModel3]
	,	i.[SourceCompanyEquipmentID3]
	,	i.[OperatorID]
	,	i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[ScheduledDate]
	,	i.[AutoComplete]
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
	,	i.[ContactFirstName]
	,	i.[ContactSurname]
	,	i.[Date01]
	,	i.[Date02]
	,	i.[Date03]
	,	i.[Date04]
	,	i.[LegacyNumber]
	,	i.[Country]
	,	i.[ContactInfo]
	,	i.[AssociatedDocNumber]
	,	i.[AssociatedCLIN]
	,	i.[SubmittedToAccounting]
	,	i.[FuelCardID]
	,	i.[AssociatedTransportOrderNumber]
	,	i.[RequestedDateTime]
	,	i.[DispatchedDateTime]
	,	i.[ErrorFlag]
	,	i.[_RowVersion]
	,	i.[TransactionGuid]
	,	i.[SiteGuid]
	,	i.[LookupTransTypeIndex]
	,	i.[LookupTransactionStatusIndex]
	,	i.[LookupOriginApplicationIndex]
	,	i.[TransactionAliasGuid]
	,	i.[BillToCompanyGuid]
	,	i.[Destination1EquipmentGuid]
	,	i.[Destination2EquipmentGuid]
	,	i.[Destination3EquipmentGuid]
	,	i.[FinalStationIATAGuid]
	,	i.[FuelCardGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[NextStationIATAGuid]
	,	i.[OperatorPersonnelGuid]
	,	i.[OriginStationIATAGuid]
	,	i.[OwnerCompanyGuid]
	,	i.[PreviousStationIATAGuid]
	,	i.[ShipperCompanyGuid]
	,	i.[ShipToCompanyGuid]
	,	i.[Source1EquipmentGuid]
	,	i.[Source2EquipmentGuid]
	,	i.[Source3EquipmentGuid]
	,	i.[SupplierCompanyGuid]
	,	i.[CarrierCompanyGuid]
	,	i.[ReasonCodeGuid]
	,	i.[OriginStationIATAID]
	,	i.[PreviousStationIATAID]
	,	i.[NextStationIATAID]
	,	i.[FinalStationIATAID]
	,	i.[OperatorName]
	,	i.[FuelAdditiveFlag]
	,	i.[IssuePoint]
	,	i.[IssuePointNumber]
	,	i.[RadioNumber]
	,	i.[GateID]
	,	i.[GateGuid]
	,	i.[ShippingMethod]
	,	i.[ReferencedTransactionGuid]
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
			agl.[TransactionGuid]=i.[TransactionGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblTransactions]
ON [dbo].[tblTransactions]
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
		INSERT INTO fmcdc.[tblTransactions]
		(
		[TransID]
		, [AliasName]
		, [SubType]
		, [Site]
		, [TransReferenceID]
		, [InventoryDate]
		, [ShipToID]
		, [ShipToCode]
		, [SupplierID]
		, [SupplierCode]
		, [CreatedDate]
		, [CreatedBy]
		, [RequestedDeliveryDate]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TransDateTime]
		, [TransVersion]
		, [SCACCode]
		, [CardNumber]
		, [ShipmentNumber]
		, [ShipperID]
		, [ShipperCode]
		, [OwnerID]
		, [OwnerCode]
		, [ManagerID]
		, [ManagerCode]
		, [CarrierID]
		, [CarrierCode]
		, [ConjoinTransID]
		, [ReversedTransID]
		, [LinkedDocumentNumber]
		, [ReversalType]
		, [PONumber]
		, [TimeIn]
		, [TimeOut]
		, [TimeEnd]
		, [RoutingID]
		, [TicketSource]
		, [LoadID]
		, [BillToID]
		, [BillToCode]
		, [DriverIdentificationNumber]
		, [CreditAmount]
		, [CardExpiration]
		, [CardName]
		, [CardType]
		, [CashAmount]
		, [RouteOriginationDate]
		, [InternationalRouteIndicator]
		, [PreviousRoutingID]
		, [ShippingDocumentNumber]
		, [DocumentNumber]
		, [STD]
		, [ETD]
		, [STA]
		, [ETA]
		, [SFT]
		, [FST]
		, [EstimatedFuelingDuration]
		, [DeleteFlag]
		, [TicketMode]
		, [DestinationRegistrationID1]
		, [DestinationSerialNumber1]
		, [DestinationEquipmentType1]
		, [DestinationEquipmentModel1]
		, [DestinationCompanyEquipmentID1]
		, [DestinationRegistrationID2]
		, [DestinationSerialNumber2]
		, [DestinationEquipmentType2]
		, [DestinationEquipmentModel2]
		, [DestinationCompanyEquipmentID2]
		, [DestinationRegistrationID3]
		, [DestinationSerialNumber3]
		, [DestinationEquipmentType3]
		, [DestinationEquipmentModel3]
		, [DestinationCompanyEquipmentID3]
		, [SourceRegistrationID1]
		, [SourceSerialNumber1]
		, [SourceEquipmentType1]
		, [SourceEquipmentModel1]
		, [SourceCompanyEquipmentID1]
		, [SourceRegistrationID2]
		, [SourceSerialNumber2]
		, [SourceEquipmentType2]
		, [SourceEquipmentModel2]
		, [SourceCompanyEquipmentID2]
		, [SourceRegistrationID3]
		, [SourceSerialNumber3]
		, [SourceEquipmentType3]
		, [SourceEquipmentModel3]
		, [SourceCompanyEquipmentID3]
		, [OperatorID]
		, [EffectiveDate]
		, [ExpirationDate]
		, [ScheduledDate]
		, [AutoComplete]
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
		, [ContactFirstName]
		, [ContactSurname]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [LegacyNumber]
		, [Country]
		, [ContactInfo]
		, [AssociatedDocNumber]
		, [AssociatedCLIN]
		, [SubmittedToAccounting]
		, [FuelCardID]
		, [AssociatedTransportOrderNumber]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [ErrorFlag]
		, [SourceRowVersion]
		, [TransactionGuid]
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupTransactionStatusIndex]
		, [LookupOriginApplicationIndex]
		, [TransactionAliasGuid]
		, [BillToCompanyGuid]
		, [Destination1EquipmentGuid]
		, [Destination2EquipmentGuid]
		, [Destination3EquipmentGuid]
		, [FinalStationIATAGuid]
		, [FuelCardGuid]
		, [ManagerCompanyGuid]
		, [NextStationIATAGuid]
		, [OperatorPersonnelGuid]
		, [OriginStationIATAGuid]
		, [OwnerCompanyGuid]
		, [PreviousStationIATAGuid]
		, [ShipperCompanyGuid]
		, [ShipToCompanyGuid]
		, [Source1EquipmentGuid]
		, [Source2EquipmentGuid]
		, [Source3EquipmentGuid]
		, [SupplierCompanyGuid]
		, [CarrierCompanyGuid]
		, [ReasonCodeGuid]
		, [OriginStationIATAID]
		, [PreviousStationIATAID]
		, [NextStationIATAID]
		, [FinalStationIATAID]
		, [OperatorName]
		, [FuelAdditiveFlag]
		, [IssuePoint]
		, [IssuePointNumber]
		, [RadioNumber]
		, [GateID]
		, [GateGuid]
		, [_ClusterIdx]
		, [ShippingMethod]
		, [ReferencedTransactionGuid]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[TransID]
		, [AliasName]
		, [SubType]
		, [Site]
		, [TransReferenceID]
		, [InventoryDate]
		, [ShipToID]
		, [ShipToCode]
		, [SupplierID]
		, [SupplierCode]
		, [CreatedDate]
		, [CreatedBy]
		, [RequestedDeliveryDate]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TransDateTime]
		, [TransVersion]
		, [SCACCode]
		, [CardNumber]
		, [ShipmentNumber]
		, [ShipperID]
		, [ShipperCode]
		, [OwnerID]
		, [OwnerCode]
		, [ManagerID]
		, [ManagerCode]
		, [CarrierID]
		, [CarrierCode]
		, [ConjoinTransID]
		, [ReversedTransID]
		, [LinkedDocumentNumber]
		, [ReversalType]
		, [PONumber]
		, [TimeIn]
		, [TimeOut]
		, [TimeEnd]
		, [RoutingID]
		, [TicketSource]
		, [LoadID]
		, [BillToID]
		, [BillToCode]
		, [DriverIdentificationNumber]
		, [CreditAmount]
		, [CardExpiration]
		, [CardName]
		, [CardType]
		, [CashAmount]
		, [RouteOriginationDate]
		, [InternationalRouteIndicator]
		, [PreviousRoutingID]
		, [ShippingDocumentNumber]
		, [DocumentNumber]
		, [STD]
		, [ETD]
		, [STA]
		, [ETA]
		, [SFT]
		, [FST]
		, [EstimatedFuelingDuration]
		, [DeleteFlag]
		, [TicketMode]
		, [DestinationRegistrationID1]
		, [DestinationSerialNumber1]
		, [DestinationEquipmentType1]
		, [DestinationEquipmentModel1]
		, [DestinationCompanyEquipmentID1]
		, [DestinationRegistrationID2]
		, [DestinationSerialNumber2]
		, [DestinationEquipmentType2]
		, [DestinationEquipmentModel2]
		, [DestinationCompanyEquipmentID2]
		, [DestinationRegistrationID3]
		, [DestinationSerialNumber3]
		, [DestinationEquipmentType3]
		, [DestinationEquipmentModel3]
		, [DestinationCompanyEquipmentID3]
		, [SourceRegistrationID1]
		, [SourceSerialNumber1]
		, [SourceEquipmentType1]
		, [SourceEquipmentModel1]
		, [SourceCompanyEquipmentID1]
		, [SourceRegistrationID2]
		, [SourceSerialNumber2]
		, [SourceEquipmentType2]
		, [SourceEquipmentModel2]
		, [SourceCompanyEquipmentID2]
		, [SourceRegistrationID3]
		, [SourceSerialNumber3]
		, [SourceEquipmentType3]
		, [SourceEquipmentModel3]
		, [SourceCompanyEquipmentID3]
		, [OperatorID]
		, [EffectiveDate]
		, [ExpirationDate]
		, [ScheduledDate]
		, [AutoComplete]
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
		, [ContactFirstName]
		, [ContactSurname]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [LegacyNumber]
		, [Country]
		, [ContactInfo]
		, [AssociatedDocNumber]
		, [AssociatedCLIN]
		, [SubmittedToAccounting]
		, [FuelCardID]
		, [AssociatedTransportOrderNumber]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [ErrorFlag]
		, CONVERT(bigint, _RowVersion)
		, [TransactionGuid]
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupTransactionStatusIndex]
		, [LookupOriginApplicationIndex]
		, [TransactionAliasGuid]
		, [BillToCompanyGuid]
		, [Destination1EquipmentGuid]
		, [Destination2EquipmentGuid]
		, [Destination3EquipmentGuid]
		, [FinalStationIATAGuid]
		, [FuelCardGuid]
		, [ManagerCompanyGuid]
		, [NextStationIATAGuid]
		, [OperatorPersonnelGuid]
		, [OriginStationIATAGuid]
		, [OwnerCompanyGuid]
		, [PreviousStationIATAGuid]
		, [ShipperCompanyGuid]
		, [ShipToCompanyGuid]
		, [Source1EquipmentGuid]
		, [Source2EquipmentGuid]
		, [Source3EquipmentGuid]
		, [SupplierCompanyGuid]
		, [CarrierCompanyGuid]
		, [ReasonCodeGuid]
		, [OriginStationIATAID]
		, [PreviousStationIATAID]
		, [NextStationIATAID]
		, [FinalStationIATAID]
		, [OperatorName]
		, [FuelAdditiveFlag]
		, [IssuePoint]
		, [IssuePointNumber]
		, [RadioNumber]
		, [GateID]
		, [GateGuid]
		, [_ClusterIdx]
		, [ShippingMethod]
		, [ReferencedTransactionGuid]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTransactions]
		(
		[TransID]
		, [AliasName]
		, [SubType]
		, [Site]
		, [TransReferenceID]
		, [InventoryDate]
		, [ShipToID]
		, [ShipToCode]
		, [SupplierID]
		, [SupplierCode]
		, [CreatedDate]
		, [CreatedBy]
		, [RequestedDeliveryDate]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TransDateTime]
		, [TransVersion]
		, [SCACCode]
		, [CardNumber]
		, [ShipmentNumber]
		, [ShipperID]
		, [ShipperCode]
		, [OwnerID]
		, [OwnerCode]
		, [ManagerID]
		, [ManagerCode]
		, [CarrierID]
		, [CarrierCode]
		, [ConjoinTransID]
		, [ReversedTransID]
		, [LinkedDocumentNumber]
		, [ReversalType]
		, [PONumber]
		, [TimeIn]
		, [TimeOut]
		, [TimeEnd]
		, [RoutingID]
		, [TicketSource]
		, [LoadID]
		, [BillToID]
		, [BillToCode]
		, [DriverIdentificationNumber]
		, [CreditAmount]
		, [CardExpiration]
		, [CardName]
		, [CardType]
		, [CashAmount]
		, [RouteOriginationDate]
		, [InternationalRouteIndicator]
		, [PreviousRoutingID]
		, [ShippingDocumentNumber]
		, [DocumentNumber]
		, [STD]
		, [ETD]
		, [STA]
		, [ETA]
		, [SFT]
		, [FST]
		, [EstimatedFuelingDuration]
		, [DeleteFlag]
		, [TicketMode]
		, [DestinationRegistrationID1]
		, [DestinationSerialNumber1]
		, [DestinationEquipmentType1]
		, [DestinationEquipmentModel1]
		, [DestinationCompanyEquipmentID1]
		, [DestinationRegistrationID2]
		, [DestinationSerialNumber2]
		, [DestinationEquipmentType2]
		, [DestinationEquipmentModel2]
		, [DestinationCompanyEquipmentID2]
		, [DestinationRegistrationID3]
		, [DestinationSerialNumber3]
		, [DestinationEquipmentType3]
		, [DestinationEquipmentModel3]
		, [DestinationCompanyEquipmentID3]
		, [SourceRegistrationID1]
		, [SourceSerialNumber1]
		, [SourceEquipmentType1]
		, [SourceEquipmentModel1]
		, [SourceCompanyEquipmentID1]
		, [SourceRegistrationID2]
		, [SourceSerialNumber2]
		, [SourceEquipmentType2]
		, [SourceEquipmentModel2]
		, [SourceCompanyEquipmentID2]
		, [SourceRegistrationID3]
		, [SourceSerialNumber3]
		, [SourceEquipmentType3]
		, [SourceEquipmentModel3]
		, [SourceCompanyEquipmentID3]
		, [OperatorID]
		, [EffectiveDate]
		, [ExpirationDate]
		, [ScheduledDate]
		, [AutoComplete]
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
		, [ContactFirstName]
		, [ContactSurname]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [LegacyNumber]
		, [Country]
		, [ContactInfo]
		, [AssociatedDocNumber]
		, [AssociatedCLIN]
		, [SubmittedToAccounting]
		, [FuelCardID]
		, [AssociatedTransportOrderNumber]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [ErrorFlag]
		, [SourceRowVersion]
		, [TransactionGuid]
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupTransactionStatusIndex]
		, [LookupOriginApplicationIndex]
		, [TransactionAliasGuid]
		, [BillToCompanyGuid]
		, [Destination1EquipmentGuid]
		, [Destination2EquipmentGuid]
		, [Destination3EquipmentGuid]
		, [FinalStationIATAGuid]
		, [FuelCardGuid]
		, [ManagerCompanyGuid]
		, [NextStationIATAGuid]
		, [OperatorPersonnelGuid]
		, [OriginStationIATAGuid]
		, [OwnerCompanyGuid]
		, [PreviousStationIATAGuid]
		, [ShipperCompanyGuid]
		, [ShipToCompanyGuid]
		, [Source1EquipmentGuid]
		, [Source2EquipmentGuid]
		, [Source3EquipmentGuid]
		, [SupplierCompanyGuid]
		, [CarrierCompanyGuid]
		, [ReasonCodeGuid]
		, [OriginStationIATAID]
		, [PreviousStationIATAID]
		, [NextStationIATAID]
		, [FinalStationIATAID]
		, [OperatorName]
		, [FuelAdditiveFlag]
		, [IssuePoint]
		, [IssuePointNumber]
		, [RadioNumber]
		, [GateID]
		, [GateGuid]
		, [_ClusterIdx]
		, [ShippingMethod]
		, [ReferencedTransactionGuid]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[TransID]
		, [AliasName]
		, [SubType]
		, [Site]
		, [TransReferenceID]
		, [InventoryDate]
		, [ShipToID]
		, [ShipToCode]
		, [SupplierID]
		, [SupplierCode]
		, [CreatedDate]
		, [CreatedBy]
		, [RequestedDeliveryDate]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TransDateTime]
		, [TransVersion]
		, [SCACCode]
		, [CardNumber]
		, [ShipmentNumber]
		, [ShipperID]
		, [ShipperCode]
		, [OwnerID]
		, [OwnerCode]
		, [ManagerID]
		, [ManagerCode]
		, [CarrierID]
		, [CarrierCode]
		, [ConjoinTransID]
		, [ReversedTransID]
		, [LinkedDocumentNumber]
		, [ReversalType]
		, [PONumber]
		, [TimeIn]
		, [TimeOut]
		, [TimeEnd]
		, [RoutingID]
		, [TicketSource]
		, [LoadID]
		, [BillToID]
		, [BillToCode]
		, [DriverIdentificationNumber]
		, [CreditAmount]
		, [CardExpiration]
		, [CardName]
		, [CardType]
		, [CashAmount]
		, [RouteOriginationDate]
		, [InternationalRouteIndicator]
		, [PreviousRoutingID]
		, [ShippingDocumentNumber]
		, [DocumentNumber]
		, [STD]
		, [ETD]
		, [STA]
		, [ETA]
		, [SFT]
		, [FST]
		, [EstimatedFuelingDuration]
		, [DeleteFlag]
		, [TicketMode]
		, [DestinationRegistrationID1]
		, [DestinationSerialNumber1]
		, [DestinationEquipmentType1]
		, [DestinationEquipmentModel1]
		, [DestinationCompanyEquipmentID1]
		, [DestinationRegistrationID2]
		, [DestinationSerialNumber2]
		, [DestinationEquipmentType2]
		, [DestinationEquipmentModel2]
		, [DestinationCompanyEquipmentID2]
		, [DestinationRegistrationID3]
		, [DestinationSerialNumber3]
		, [DestinationEquipmentType3]
		, [DestinationEquipmentModel3]
		, [DestinationCompanyEquipmentID3]
		, [SourceRegistrationID1]
		, [SourceSerialNumber1]
		, [SourceEquipmentType1]
		, [SourceEquipmentModel1]
		, [SourceCompanyEquipmentID1]
		, [SourceRegistrationID2]
		, [SourceSerialNumber2]
		, [SourceEquipmentType2]
		, [SourceEquipmentModel2]
		, [SourceCompanyEquipmentID2]
		, [SourceRegistrationID3]
		, [SourceSerialNumber3]
		, [SourceEquipmentType3]
		, [SourceEquipmentModel3]
		, [SourceCompanyEquipmentID3]
		, [OperatorID]
		, [EffectiveDate]
		, [ExpirationDate]
		, [ScheduledDate]
		, [AutoComplete]
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
		, [ContactFirstName]
		, [ContactSurname]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [LegacyNumber]
		, [Country]
		, [ContactInfo]
		, [AssociatedDocNumber]
		, [AssociatedCLIN]
		, [SubmittedToAccounting]
		, [FuelCardID]
		, [AssociatedTransportOrderNumber]
		, [RequestedDateTime]
		, [DispatchedDateTime]
		, [ErrorFlag]
		, CONVERT(bigint, _RowVersion)
		, [TransactionGuid]
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupTransactionStatusIndex]
		, [LookupOriginApplicationIndex]
		, [TransactionAliasGuid]
		, [BillToCompanyGuid]
		, [Destination1EquipmentGuid]
		, [Destination2EquipmentGuid]
		, [Destination3EquipmentGuid]
		, [FinalStationIATAGuid]
		, [FuelCardGuid]
		, [ManagerCompanyGuid]
		, [NextStationIATAGuid]
		, [OperatorPersonnelGuid]
		, [OriginStationIATAGuid]
		, [OwnerCompanyGuid]
		, [PreviousStationIATAGuid]
		, [ShipperCompanyGuid]
		, [ShipToCompanyGuid]
		, [Source1EquipmentGuid]
		, [Source2EquipmentGuid]
		, [Source3EquipmentGuid]
		, [SupplierCompanyGuid]
		, [CarrierCompanyGuid]
		, [ReasonCodeGuid]
		, [OriginStationIATAID]
		, [PreviousStationIATAID]
		, [NextStationIATAID]
		, [FinalStationIATAID]
		, [OperatorName]
		, [FuelAdditiveFlag]
		, [IssuePoint]
		, [IssuePointNumber]
		, [RadioNumber]
		, [GateID]
		, [GateGuid]
		, [_ClusterIdx]
		, [ShippingMethod]
		, [ReferencedTransactionGuid]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTransactions] ON [dbo].[tblTransactions]
GO



CREATE NONCLUSTERED INDEX [IX_tblTransactions_LedgerCovering] ON [dbo].[tblTransactions]
(
	[SiteGuid] ASC,
	[InventoryDate] ASC,
	[ManagerCompanyGuid] ASC,
	[OwnerCompanyGuid] ASC
)
INCLUDE ( 	[AliasName],
	[ReversalType],
	[ErrorFlag],
	[LookupTransactionStatusIndex],
	[DeleteFlag],
	[SupplierCompanyGuid],
	[ShipperCompanyGuid],
	[CarrierCompanyGuid],
	[BillToCompanyGuid],
	[ShipToCompanyGuid],
	[LookupTransTypeIndex],
	[TransactionGuid],
	[TransVersion])
GO



CREATE NONCLUSTERED INDEX [IX_tblTransactions_AuditRpt] ON [dbo].[tblTransactions]
(
	[DeleteFlag],
	[InventoryDate] ASC,	
	[SiteGuid],
	[ManagerCompanyGuid],
	[OwnerCompanyGuid],
	[LookupTransactionStatusIndex],
	[LookupTransTypeIndex],
	[CarrierCompanyGuid],
	[ReversalType]
)
	INCLUDE ( 
		[TransactionGuid],
		[TransVersion],
		[ShipperCompanyGuid], 
		[ShipToCompanyGuid],
		[BillToCompanyGuid],
		[SupplierCompanyGuid],
		[Site],
		[OwnerId],
		[AliasName]
		)

GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_CoveringJournalReports] ON [dbo].[tblTransactions] 
(
	[OwnerCompanyGuid],
	[ManagerCompanyGuid], 
	[InventoryDate], 
	[LookupTransTypeIndex], 
	[TransactionGuid], 
	[DeleteFlag], 
	[OwnerID], 
	[AliasName], 
	[Site]
)

GO

CREATE NONCLUSTERED INDEX [IX_tbltransactions_SiteGuid_LookupTransTypeIndex] ON [dbo].[tblTransactions]
(
	[SiteGuid] ASC,
	[LookupTransTypeIndex] ASC
)
INCLUDE ( 	[AliasName],[InventoryDate],[TransDateTime])
GO
CREATE NONCLUSTERED INDEX [IX_tbltransactions_AliasNameSubmittedToAccounting_InventoryDate] ON [dbo].[tblTransactions]
( 	
	[AliasName] ASC,
	[SubmittedToAccounting] ASC,
	[InventoryDate] ASC
) 
INCLUDE ( 	[Site],	[ShipToID],	[SupplierID],[ShipperID],[OwnerID],[ManagerID],[CarrierID],[BillToID],[DeleteFlag],[TransactionGuid]) 
GO
CREATE INDEX [IX_tblTransactions_DeleteFlag_SiteGuid_AliasName_RequestedDateTime] ON [dbo].[tblTransactions]
 ([DeleteFlag], [SiteGuid],[AliasName], [RequestedDateTime]) INCLUDE ([_RowVersion], [TransactionGuid])
 GO


CREATE INDEX [IX_tblTransactions_ConjoinTransID] ON [dbo].[tblTransactions] 
([ConjoinTransID]) INCLUDE ([TransID], [ShipToID], [TransVersion], [BillToID], [DestinationRegistrationID1], [TransactionGuid])
GO


CREATE INDEX [IX_tblTransactions_DeleteFlag_SiteGuid_AliasName_RequestedDateTime__RowVersion] ON [dbo].[tblTransactions] 
([DeleteFlag], [SiteGuid],[AliasName], [RequestedDateTime], [_RowVersion]) INCLUDE ([TransactionGuid])
GO

CREATE INDEX [IX_tblTransactions_Flag02] ON [dbo].[tblTransactions] 
([Flag02]) INCLUDE ([TransID], [InventoryDate], [UpdatedDate], [TransactionGuid], [SiteGuid])
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_SiteGuid_DocumentNumber] ON [dbo].[tblTransactions]
(
	[SiteGuid] ASC,
	[DocumentNumber] ASC
)
INCLUDE ( 	[LookupTransTypeIndex],	[LookupOriginApplicationIndex])
GO

CREATE INDEX [IX_tblTransactions_SubmittedToAccounting_SiteGuid_LookupOriginApplicationIndex] ON [dbo].[tblTransactions] 
(
	[SubmittedToAccounting]
	,[SiteGuid]
	,[LookupOriginApplicationIndex]
	) 
INCLUDE ([_RowVersion])
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_DeleteFlag_Flag02_Flag05_SiteGuid_AliasName_LookupTransactionStatusIndex_LookupOriginApplicationIndex]
ON [dbo].[tblTransactions] ([DeleteFlag],[Flag02],[Flag05],[SiteGuid],[AliasName],[LookupTransactionStatusIndex],[LookupOriginApplicationIndex])
INCLUDE ([TransID],[SubType],[TransactionGuid])
GO
