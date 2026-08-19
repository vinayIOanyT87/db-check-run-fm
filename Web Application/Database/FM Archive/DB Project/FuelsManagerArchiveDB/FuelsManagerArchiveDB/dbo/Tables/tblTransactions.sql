/*

	DROP TABLE [dbo].[tblTransactions]

*/
CREATE TABLE [dbo].[tblTransactions] (
    [TransID]                        NVARCHAR (64)      NULL,
    [AliasName]                      NVARCHAR (32)      NULL,
    [SubType]                        NVARCHAR (20)      NULL,
    [Site]                           NVARCHAR (30)      NULL,
    [TransReferenceID]               NVARCHAR (64)      NULL,
    [InventoryDate]                  DATE               NULL,
    [ShipToID]                       NVARCHAR (100)     NULL,
    [ShipToCode]                     NVARCHAR (10)      NULL,
    [SupplierID]                     NVARCHAR (100)     NULL,
    [SupplierCode]                   NVARCHAR (10)      NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) NULL,
    [CreatedBy]                      [dbo].[udtUserID]  NULL,
    [RequestedDeliveryDate]          DATETIMEOFFSET (7) NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  NULL,
    [TransDateTime]                  DATETIMEOFFSET (7) NULL,
    [TransVersion]                   BIGINT             NULL,
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
    [SubmittedToAccounting]          BIT                NULL,
    [FuelCardID]                     NVARCHAR (50)      NULL,
    [AssociatedTransportOrderNumber] NVARCHAR (30)      NULL,
    [RequestedDateTime]              DATETIMEOFFSET (7) NULL,
    [DispatchedDateTime]             DATETIMEOFFSET (7) NULL,
    [ErrorFlag]                      BIT                NULL,    
    [TransactionGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]                       UNIQUEIDENTIFIER   NULL,
    [LookupTransTypeIndex]           SMALLINT           NULL,
    [LookupTransactionStatusIndex]   INT                NULL,
    [LookupOriginApplicationIndex]   INT                NULL,
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
    [FuelAdditiveFlag]               BIT                NULL,
    [IssuePoint]                     NVARCHAR (MAX)     NULL,
    [IssuePointNumber]               NVARCHAR (MAX)     NULL,
    [RadioNumber]                    NVARCHAR (MAX)     NULL,
    [GateID]                         NVARCHAR (10)      NULL,
    [GateGuid]                       UNIQUEIDENTIFIER   NULL,    
    [ShippingMethod]                 NVARCHAR (150)     NULL,
	[InventoryDateKey]				 INT                NOT NULL,
	[ArchiveDate]					 DATETIMEOFFSET (7) NULL,
	[ETLProcessKey]					 BIGINT			    NULL,
	[_RowVersion]                    ROWVERSION         NOT NULL,
	[_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactions_GUID] PRIMARY KEY NONCLUSTERED ([InventoryDateKey] ASC, [TransactionGuid] ASC) ON [AnnualPS]([InventoryDateKey])
) ON [AnnualPS]([InventoryDateKey]);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactions_ClusterIdx] 
	ON [dbo].[tblTransactions]([InventoryDateKey], [_ClusterIdx])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactions_InventoryDate]
    ON [dbo].[tblTransactions]([InventoryDate] ASC)
	INCLUDE( [TransactionGuid], [_RowVersion])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactions_CoveringPreviousVersionInformation]
    ON [dbo].[tblTransactions]([InventoryDateKey] ASC, [TransactionGuid] ASC)
    INCLUDE([DeleteFlag], [LookupTransactionStatusIndex], [TransVersion], [_RowVersion])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactions_Paice_Covering]
ON [dbo].[tblTransactions]( [InventoryDate] ASC, [SiteGuid] ASC, [TransactionAliasGuid] ASC, [OwnerCompanyGuid] ASC,  [ManagerCompanyGuid] ASC )
INCLUDE(DATE01, [TransactionGuid], [DeleteFlag] , [TransID], [UpdatedDate])
ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactions_TransactionAliasGuid]
    ON [dbo].[tblTransactions]([TransactionAliasGuid] ASC, [InventoryDate] ASC,  [DeleteFlag] ASC)
	INCLUDE (ManagerCompanyGuid, OwnerCompanyGuid, SiteGuid)
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactions_TransID]
    ON [dbo].[tblTransactions]([InventoryDateKey] ASC,[TransID] ASC, ManagerCompanyGuid ASC)
    INCLUDE([TransactionGuid], [LookupTransTypeIndex], [InventoryDate])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IXU_tblTransactions_ReversedTransID]
    ON [dbo].[tblTransactions]([ReversedTransID] ASC)
    INCLUDE([TransactionGuid], [SiteGuid], [LookupTransTypeIndex], [InventoryDate])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactions_CoveringAssociatedTransactionQueries]
    ON [dbo].[tblTransactions]([InventoryDateKey] ASC, [TransactionGuid] ASC)
    INCLUDE([SiteGuid], [TransactionAliasGuid], [LookupTransTypeIndex])
	ON [AnnualPS]([InventoryDateKey]);
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactions_LedgerCovering] ON [dbo].[tblTransactions]
(
	[SiteGuid] ASC,
	[InventoryDate] ASC,
	[ManagerCompanyGuid] ASC,
	[OwnerCompanyGuid] ASC
)
INCLUDE ([AliasName],
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
	ON [AnnualPS]([InventoryDateKey])
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
		ON [AnnualPS]([InventoryDateKey])
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
ON [AnnualPS]([InventoryDateKey])
GO


CREATE NONCLUSTERED INDEX [IX_tbltransactions_SiteGuid_LookupTransTypeIndex] ON [dbo].[tblTransactions]
(
	[SiteGuid] ASC,
	[LookupTransTypeIndex] ASC
)
INCLUDE ( 	[AliasName],[InventoryDate],[TransDateTime])
ON [AnnualPS]([InventoryDateKey])
GO


CREATE NONCLUSTERED INDEX [IX_tbltransactions_AliasNameSubmittedToAccounting_InventoryDate] ON [dbo].[tblTransactions]
( 	
	[AliasName] ASC,
	[SubmittedToAccounting] ASC,
	[InventoryDate] ASC
) 
INCLUDE ([Site],[ShipToID],[SupplierID],[ShipperID],[OwnerID],[ManagerID],[CarrierID],[BillToID],[DeleteFlag],[TransactionGuid]) 
ON [AnnualPS]([InventoryDateKey])
GO


CREATE INDEX [IX_tblTransactions_DeleteFlag_SiteGuid_AliasName_RequestedDateTime] ON [dbo].[tblTransactions]
 ([DeleteFlag], [SiteGuid],[AliasName], [RequestedDateTime]) INCLUDE ([_RowVersion], [TransactionGuid])
 ON [AnnualPS]([InventoryDateKey])
 GO


CREATE INDEX [IX_tblTransactions_ConjoinTransID] ON [dbo].[tblTransactions] 
([ConjoinTransID]) INCLUDE ([TransID], [ShipToID], [TransVersion], [BillToID], [DestinationRegistrationID1], [TransactionGuid])
ON [AnnualPS]([InventoryDateKey])
GO


CREATE INDEX [IX_tblTransactions_DeleteFlag_SiteGuid_AliasName_RequestedDateTime__RowVersion] ON [dbo].[tblTransactions] 
([DeleteFlag], [SiteGuid],[AliasName], [RequestedDateTime], [_RowVersion]) INCLUDE ([TransactionGuid])
ON [AnnualPS]([InventoryDateKey])
GO


CREATE INDEX [IX_tblTransactions_Flag02] ON [dbo].[tblTransactions] 
([Flag02]) INCLUDE ([TransID], [InventoryDate], [UpdatedDate], [TransactionGuid], [SiteGuid])
ON [AnnualPS]([InventoryDateKey])
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactions_SiteGuid_DocumentNumber] ON [dbo].[tblTransactions]
(
	[SiteGuid] ASC,
	[DocumentNumber] ASC
)
INCLUDE ([LookupTransTypeIndex], [LookupOriginApplicationIndex])
ON [AnnualPS]([InventoryDateKey])
GO


CREATE INDEX [IX_tblTransactions_SubmittedToAccounting_SiteGuid_LookupOriginApplicationIndex] ON [dbo].[tblTransactions] 
(
	[SubmittedToAccounting]
	,[SiteGuid]
	,[LookupOriginApplicationIndex]
	) 
INCLUDE ([_RowVersion])
ON [AnnualPS]([InventoryDateKey])
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactions_DeleteFlag_Flag02_Flag05_SiteGuid_AliasName_LookupTransactionStatusIndex_LookupOriginApplicationIndex]
ON [dbo].[tblTransactions] ([DeleteFlag],[Flag02],[Flag05],[SiteGuid],[AliasName],[LookupTransactionStatusIndex],[LookupOriginApplicationIndex])
INCLUDE ([TransID],[SubType],[TransactionGuid])
ON [AnnualPS]([InventoryDateKey])
GO
