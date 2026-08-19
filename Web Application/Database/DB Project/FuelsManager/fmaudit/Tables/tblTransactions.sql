CREATE TABLE [fmaudit].[tblTransactions](
	[TransID] nvarchar (64) NULL
,	[AliasName] nvarchar (32) NULL
,	[SubType] nvarchar (20) NULL
,	[Site] nvarchar (30) NULL
,	[TransReferenceID] nvarchar (64) NULL
,	[InventoryDate] date NULL
,	[ShipToID] nvarchar (100) NULL
,	[ShipToCode] nvarchar (10) NULL
,	[SupplierID] nvarchar (100) NULL
,	[SupplierCode] nvarchar (10) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[RequestedDeliveryDate] datetimeoffset NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[TransDateTime] datetimeoffset NULL
,	[TransVersion] bigint NULL
,	[SCACCode] nvarchar (4) NULL
,	[CardNumber] nvarchar (30) NULL
,	[ShipmentNumber] nvarchar (30) NULL
,	[ShipperID] nvarchar (100) NULL
,	[ShipperCode] nvarchar (10) NULL
,	[OwnerID] nvarchar (100) NULL
,	[OwnerCode] nvarchar (10) NULL
,	[ManagerID] nvarchar (100) NULL
,	[ManagerCode] nvarchar (10) NULL
,	[CarrierID] nvarchar (100) NULL
,	[CarrierCode] nvarchar (10) NULL
,	[ConjoinTransID] nvarchar (64) NULL
,	[ReversedTransID] nvarchar (64) NULL
,	[LinkedDocumentNumber] nvarchar (64) NULL
,	[ReversalType] nvarchar (2) NULL
,	[PONumber] nvarchar (14) NULL
,	[TimeIn] datetimeoffset NULL
,	[TimeOut] datetimeoffset NULL
,	[TimeEnd] datetimeoffset NULL
,	[RoutingID] nvarchar (30) NULL
,	[TicketSource] nvarchar (20) NULL
,	[LoadID] nvarchar (50) NULL
,	[BillToID] nvarchar (100) NULL
,	[BillToCode] nvarchar (10) NULL
,	[DriverIdentificationNumber] nvarchar (50) NULL
,	[CreditAmount] float NULL
,	[CardExpiration] datetimeoffset NULL
,	[CardName] nvarchar (30) NULL
,	[CardType] nvarchar (30) NULL
,	[CashAmount] float NULL
,	[RouteOriginationDate] datetimeoffset NULL
,	[InternationalRouteIndicator] bit NULL
,	[PreviousRoutingID] nvarchar (30) NULL
,	[ShippingDocumentNumber] nvarchar (30) NULL
,	[DocumentNumber] nvarchar (30) NULL
,	[STD] datetimeoffset NULL
,	[ETD] datetimeoffset NULL
,	[STA] datetimeoffset NULL
,	[ETA] datetimeoffset NULL
,	[SFT] datetimeoffset NULL
,	[FST] datetimeoffset NULL
,	[EstimatedFuelingDuration] int NULL
,	[DeleteFlag] bit NULL
,	[TicketMode] nvarchar (15) NULL
,	[DestinationRegistrationID1] nvarchar (30) NULL
,	[DestinationSerialNumber1] nvarchar (10) NULL
,	[DestinationEquipmentType1] nvarchar (50) NULL
,	[DestinationEquipmentModel1] nvarchar (20) NULL
,	[DestinationCompanyEquipmentID1] nvarchar (30) NULL
,	[DestinationRegistrationID2] nvarchar (30) NULL
,	[DestinationSerialNumber2] nvarchar (10) NULL
,	[DestinationEquipmentType2] nvarchar (50) NULL
,	[DestinationEquipmentModel2] nvarchar (20) NULL
,	[DestinationCompanyEquipmentID2] nvarchar (30) NULL
,	[DestinationRegistrationID3] nvarchar (30) NULL
,	[DestinationSerialNumber3] nvarchar (10) NULL
,	[DestinationEquipmentType3] nvarchar (50) NULL
,	[DestinationEquipmentModel3] nvarchar (20) NULL
,	[DestinationCompanyEquipmentID3] nvarchar (30) NULL
,	[SourceRegistrationID1] nvarchar (30) NULL
,	[SourceSerialNumber1] nvarchar (10) NULL
,	[SourceEquipmentType1] nvarchar (50) NULL
,	[SourceEquipmentModel1] nvarchar (20) NULL
,	[SourceCompanyEquipmentID1] nvarchar (30) NULL
,	[SourceRegistrationID2] nvarchar (30) NULL
,	[SourceSerialNumber2] nvarchar (10) NULL
,	[SourceEquipmentType2] nvarchar (50) NULL
,	[SourceEquipmentModel2] nvarchar (20) NULL
,	[SourceCompanyEquipmentID2] nvarchar (30) NULL
,	[SourceRegistrationID3] nvarchar (30) NULL
,	[SourceSerialNumber3] nvarchar (10) NULL
,	[SourceEquipmentType3] nvarchar (50) NULL
,	[SourceEquipmentModel3] nvarchar (20) NULL
,	[SourceCompanyEquipmentID3] nvarchar (30) NULL
,	[OperatorID] nvarchar (50) NULL
,	[EffectiveDate] datetimeoffset NULL
,	[ExpirationDate] datetimeoffset NULL
,	[ScheduledDate] datetimeoffset NULL
,	[AutoComplete] bit NULL
,	[Flag01] bit NULL
,	[Flag02] bit NULL
,	[Flag03] bit NULL
,	[Flag04] bit NULL
,	[Flag05] bit NULL
,	[Flag06] bit NULL
,	[Number01] float NULL
,	[Number02] float NULL
,	[Number03] float NULL
,	[Number04] float NULL
,	[Number05] float NULL
,	[Number06] float NULL
,	[ContactFirstName] nvarchar (50) NULL
,	[ContactSurname] nvarchar (50) NULL
,	[Date01] datetimeoffset NULL
,	[Date02] datetimeoffset NULL
,	[Date03] datetimeoffset NULL
,	[Date04] datetimeoffset NULL
,	[LegacyNumber] nvarchar (50) NULL
,	[Country] nvarchar (50) NULL
,	[ContactInfo] nvarchar (50) NULL
,	[AssociatedDocNumber] nvarchar (30) NULL
,	[AssociatedCLIN] nvarchar (10) NULL
,	[SubmittedToAccounting] bit NULL
,	[FuelCardID] nvarchar (50) NULL
,	[AssociatedTransportOrderNumber] nvarchar (30) NULL
,	[RequestedDateTime] datetimeoffset NULL
,	[DispatchedDateTime] datetimeoffset NULL
,	[ErrorFlag] bit NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[TransactionGuid] uniqueidentifier NULL
,	[SiteGuid] uniqueidentifier NULL
,	[LookupTransTypeIndex] smallint NULL
,	[LookupTransactionStatusIndex] int NULL
,	[LookupOriginApplicationIndex] int NULL
,	[TransactionAliasGuid] uniqueidentifier NULL
,	[BillToCompanyGuid] uniqueidentifier NULL
,	[Destination1EquipmentGuid] uniqueidentifier NULL
,	[Destination2EquipmentGuid] uniqueidentifier NULL
,	[Destination3EquipmentGuid] uniqueidentifier NULL
,	[FinalStationIATAGuid] uniqueidentifier NULL
,	[FuelCardGuid] uniqueidentifier NULL
,	[ManagerCompanyGuid] uniqueidentifier NULL
,	[NextStationIATAGuid] uniqueidentifier NULL
,	[OperatorPersonnelGuid] uniqueidentifier NULL
,	[OriginStationIATAGuid] uniqueidentifier NULL
,	[OwnerCompanyGuid] uniqueidentifier NULL
,	[PreviousStationIATAGuid] uniqueidentifier NULL
,	[ShipperCompanyGuid] uniqueidentifier NULL
,	[ShipToCompanyGuid] uniqueidentifier NULL
,	[Source1EquipmentGuid] uniqueidentifier NULL
,	[Source2EquipmentGuid] uniqueidentifier NULL
,	[Source3EquipmentGuid] uniqueidentifier NULL
,	[SupplierCompanyGuid] uniqueidentifier NULL
,	[CarrierCompanyGuid] uniqueidentifier NULL
,	[ReasonCodeGuid] uniqueidentifier NULL
,	[OriginStationIATAID] nvarchar (50) NULL
,	[PreviousStationIATAID] nvarchar (50) NULL
,	[NextStationIATAID] nvarchar (50) NULL
,	[FinalStationIATAID] nvarchar (50) NULL
,	[OperatorName] nvarchar (150) NULL
,	[FuelAdditiveFlag] bit NULL
,	[IssuePoint] nvarchar (max) NULL
,	[IssuePointNumber] nvarchar (max) NULL
,	[RadioNumber] nvarchar (max) NULL
,	[GateID] nvarchar (10) NULL
,	[GateGuid] uniqueidentifier NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblTransactions_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblTransactions_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblTransactions_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,   [ShippingMethod] NVARCHAR (150)     NULL
,	[ReferencedTransactionGuid] UNIQUEIDENTIFIER NULL
)



GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_AuditGUID] ON [fmaudit].[tblTransactions](_AuditGUID ASC)
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblTransactions] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE NONCLUSTERED INDEX [IX_fmaudit_tblTransactions_TransactionGuid__AuditEventType] ON [fmaudit].[tblTransactions] 
	([TransactionGuid], [_AuditEventType]) INCLUDE ([SiteGuid])
GO

CREATE CLUSTERED INDEX [IX_tblTransactions_ClusterIdx] ON [fmaudit].[tblTransactions](_ClusterIdx ASC)
GO
