CREATE TABLE [fmaudit].[tblCompanies](
	[ID] nvarchar (100) NULL
,	[Code] nvarchar (10) NULL
,	[Name] nvarchar (100) NULL
,	[ShortName] nvarchar (4) NULL
,	[Address1] nvarchar (60) NULL
,	[Address2] nvarchar (60) NULL
,	[City] nvarchar (60) NULL
,	[State] nvarchar (20) NULL
,	[Zip] nvarchar (11) NULL
,	[Country] nvarchar (30) NULL
,	[Phone] nvarchar (20) NULL
,	[FAX] nvarchar (20) NULL
,	[EmergencyContact] nvarchar (30) NULL
,	[EmergencyPhone] nvarchar (20) NULL
,	[FlightPrefix] nvarchar (5) NULL
,	[EffectiveDate] datetimeoffset NULL
,	[ExpirationDate] datetimeoffset NULL
,	[OnHold] bit NULL
,	[PickupFLights] bit NULL
,	[StockTrack] bit NULL
,	[SufferLossGain] bit NULL
,	[LowStockWarning] float NULL
,	[LockedOut] bit NULL
,	[LockedOutReason] nvarchar (80) NULL
,	[LockedOutDate] datetimeoffset NULL
,	[ReceivableAccount] nvarchar (20) NULL
,	[RefinerCode] nvarchar (20) NULL
,	[LastActivityDate] datetimeoffset NULL
,	[CreditOK] bit NULL
,	[AdditiveAccounting] bit NULL
,	[PurchaseOrderRequired] bit NULL
,	[EPANumber] nvarchar (20) NULL
,	[FederalID] nvarchar (20) NULL
,	[FederalID2] nvarchar (20) NULL
,	[FederalID3] nvarchar (20) NULL
,	[FederalID4] nvarchar (20) NULL
,	[FederalID5] nvarchar (20) NULL
,	[StateID] nvarchar (20) NULL
,	[TaxNumber] nvarchar (20) NULL
,	[FlushPermitted] bit NULL
,	[PumpOffPermitted] bit NULL
,	[DeliveryToTerminalPermitted] bit NULL
,	[LicenseNumber] nvarchar (20) NULL
,	[LicenseExpiration] datetimeoffset NULL
,	[InsuranceCompany] nvarchar (20) NULL
,	[InsurancePolicy] nvarchar (20) NULL
,	[LiabilityAmount] money NULL
,	[HazardousMaterialExclusion] bit NULL
,	[InsuranceExpiration] datetimeoffset NULL
,	[AllowDriverEntry] bit NULL
,	[PINRequired] bit NULL
,	[MaximumVehicleWeight] float NULL
,	[WeightUnits] smallint NULL
,	[AccountNumber] nvarchar (30) NULL
,	[SCACCode] nvarchar (4) NULL
,	[DisableOwnerAllocationsCheck] bit NULL
,	[DisableShipperAllocationsCheck] bit NULL
,	[DisableBillToAllocationsCheck] bit NULL
,	[DisableShipToAllocationsCheck] bit NULL
,	[LoadRackDisplayText] nvarchar (30) NULL
,	[UserData1] nvarchar (60) NULL
,	[UserData2] nvarchar (60) NULL
,	[UserData3] nvarchar (60) NULL
,	[UserData4] nvarchar (60) NULL
,	[UserData5] nvarchar (60) NULL
,	[UserData6] nvarchar (60) NULL
,	[UserData7] nvarchar (60) NULL
,	[UserData8] nvarchar (60) NULL
,	[CreatedDate] datetimeoffset NULL
,	[CreatedBy] nvarchar (100) NULL
,	[UpdatedDate] datetimeoffset NULL
,	[UpdatedBy] nvarchar (100) NULL
,	[CompanyGuid] uniqueidentifier NULL
,	[OriginalRowVersion] BINARY(8) NULL
,	[SiteGuid] uniqueidentifier NULL
,	[IATAGuid] uniqueidentifier NULL
,	[ShipperTypeApplicationStringGuid] uniqueidentifier NULL
,	[CustomerBillToTypeApplicationStringGuid] uniqueidentifier NULL
,	[CustomerShipToTypeApplicationStringGuid] uniqueidentifier NULL
,	[Contact1Name] nvarchar (30) NULL
,	[Contact1Address1] nvarchar (30) NULL
,	[Contact1Address2] nvarchar (30) NULL
,	[Contact1City] nvarchar (60) NULL
,	[Contact1State] nvarchar (20) NULL
,	[Contact1Zip] nvarchar (11) NULL
,	[Contact1Country] nvarchar (30) NULL
,	[Contact1PhoneOffice] nvarchar (20) NULL
,	[Contact1Fax] nvarchar (20) NULL
,	[Contact1EmailAddress] nvarchar (30) NULL
,	[Contact2Name] nvarchar (30) NULL
,	[Contact2Address1] nvarchar (30) NULL
,	[Contact2Address2] nvarchar (30) NULL
,	[Contact2City] nvarchar (60) NULL
,	[Contact2State] nvarchar (20) NULL
,	[Contact2Zip] nvarchar (11) NULL
,	[Contact2Country] nvarchar (30) NULL
,	[Contact2PhoneOffice] nvarchar (20) NULL
,	[Contact2Fax] nvarchar (20) NULL
,	[Contact2EmailAddress] nvarchar (30) NULL
,	[Contact1PhoneMobile] nvarchar (20) NULL
,	[Contact2PhoneMobile] nvarchar (20) NULL
,	[_MasterRecordGuid] uniqueidentifier NULL
,	[Note] nvarchar (2000) NULL
,	[HiddenDate] datetimeoffset NULL
,	[_AuditEventType] CHAR(1) NULL
,	[_AuditEventSequence] TINYINT NULL CONSTRAINT DF_tblCompanies_AuditEventSequence DEFAULT 0
,	[_AuditSiteGuid] UNIQUEIDENTIFIER NULL
,	[_AuditSessionGuid] UNIQUEIDENTIFIER NULL
,	[_AuditUserID] udtUserID NULL
,	[_AuditSessionTokenID] UNIQUEIDENTIFIER NULL
,	[_AuditCreatedDate] DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblCompanies_AuditCreatedDate DEFAULT sysdatetimeoffset()
,	[_AuditGUID] UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblCompanies_AuditGUID DEFAULT newid()
,	[_AuditRowVersion] ROWVERSION 
,	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL 
,	[_AuditContext] VARBINARY(128) NULL 
,	[ScullyRequired] BIT CONSTRAINT [DF_tblCompanies_ScullyRequired] DEFAULT ((0)) NULL
,	[ConsortiumTypeIndex] INT NULL
,	[CompanyIATACode] NVARCHAR (50) NULL
,	[CompanyICAOCode] NVARCHAR (50) NULL
)
GO

CREATE NONCLUSTERED INDEX [IX_tblCompanies_AuditGUID] ON [fmaudit].[tblCompanies](_AuditGUID ASC)


GO
CREATE NONCLUSTERED INDEX [IX_tblCompanies_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblCompanies] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblCompanies_ClusterIdx] ON [fmaudit].[tblCompanies](_ClusterIdx ASC)
GO

CREATE NONCLUSTERED INDEX [IX_fmaudit_tblCompanies_CompanyGuid__AuditEventType] ON [fmaudit].[tblCompanies]
(
	[CompanyGuid] ASC,
	[_AuditEventType] ASC
)
INCLUDE (ID)
GO