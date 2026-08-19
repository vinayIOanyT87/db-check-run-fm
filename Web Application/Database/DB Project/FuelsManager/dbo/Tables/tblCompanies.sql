CREATE TABLE [dbo].[tblCompanies] (
    [ID]                                      NVARCHAR (100)     CONSTRAINT [DF_tblCompanies_ID] DEFAULT ('') NOT NULL,
    [Code]                                    NVARCHAR (10)      CONSTRAINT [DF_tblCompanies_Code] DEFAULT ('') NOT NULL,
    [Name]                                    NVARCHAR (100)     NULL,
    [ShortName]                               NVARCHAR (4)       NULL,
    [Address1]                                NVARCHAR (60)      NULL,
    [Address2]                                NVARCHAR (60)      NULL,
    [City]                                    NVARCHAR (60)      NULL,
    [State]                                   NVARCHAR (20)      NULL,
    [Zip]                                     NVARCHAR (11)      NULL,
    [Country]                                 NVARCHAR (30)      NULL,
    [Phone]                                   NVARCHAR (20)      NULL,
    [FAX]                                     NVARCHAR (20)      NULL,
    [EmergencyContact]                        NVARCHAR (30)      NULL,
    [EmergencyPhone]                          NVARCHAR (20)      NULL,
    [FlightPrefix]                            NVARCHAR (5)       NULL,
    [EffectiveDate]                           DATETIMEOFFSET (7) NULL,
    [ExpirationDate]                          DATETIMEOFFSET (7) NULL,
    [OnHold]                                  BIT                NULL,
    [PickupFLights]                           BIT                NULL,
    [StockTrack]                              BIT                NULL,
    [SufferLossGain]                          BIT                NULL,
    [LowStockWarning]                         FLOAT (53)         NULL,
    [LockedOut]                               BIT                NULL,
    [LockedOutReason]                         NVARCHAR (80)      NULL,
    [LockedOutDate]                           DATETIMEOFFSET (7) NULL,
    [ReceivableAccount]                       NVARCHAR (20)      NULL,
    [RefinerCode]                             NVARCHAR (20)      NULL,
    [LastActivityDate]                        DATETIMEOFFSET (7) NULL,
    [CreditOK]                                BIT                NULL,
    [AdditiveAccounting]                      BIT                NULL,
    [PurchaseOrderRequired]                   BIT                NULL,
    [EPANumber]                               NVARCHAR (20)      NULL,
    [FederalID]                               NVARCHAR (20)      NULL,
    [FederalID2]                              NVARCHAR (20)      NULL,
    [FederalID3]                              NVARCHAR (20)      NULL,
    [FederalID4]                              NVARCHAR (20)      NULL,
    [FederalID5]                              NVARCHAR (20)      NULL,
    [StateID]                                 NVARCHAR (20)      NULL,
    [TaxNumber]                               NVARCHAR (20)      NULL,
    [FlushPermitted]                          BIT                NULL,
    [PumpOffPermitted]                        BIT                NULL,
    [DeliveryToTerminalPermitted]             BIT                NULL,
    [LicenseNumber]                           NVARCHAR (20)      NULL,
    [LicenseExpiration]                       DATETIMEOFFSET (7) NULL,
    [InsuranceCompany]                        NVARCHAR (20)      NULL,
    [InsurancePolicy]                         NVARCHAR (20)      NULL,
    [LiabilityAmount]                         MONEY              NULL,
    [HazardousMaterialExclusion]              BIT                NULL,
    [InsuranceExpiration]                     DATETIMEOFFSET (7) NULL,
    [AllowDriverEntry]                        BIT                NULL,
    [PINRequired]                             BIT                NULL,
    [MaximumVehicleWeight]                    FLOAT (53)         NULL,
    [WeightUnits]                             SMALLINT           NULL,
    [AccountNumber]                           NVARCHAR (30)      NULL,
    [SCACCode]                                NVARCHAR (4)       NULL,
    [DisableOwnerAllocationsCheck]            BIT                NULL,
    [DisableShipperAllocationsCheck]          BIT                NULL,
    [DisableBillToAllocationsCheck]           BIT                NULL,
    [DisableShipToAllocationsCheck]           BIT                NULL,
    [LoadRackDisplayText]                     NVARCHAR (30)      NULL,
    [UserData1]                               NVARCHAR (60)      NULL,
    [UserData2]                               NVARCHAR (60)      NULL,
    [UserData3]                               NVARCHAR (60)      NULL,
    [UserData4]                               NVARCHAR (60)      NULL,
    [UserData5]                               NVARCHAR (60)      NULL,
    [UserData6]                               NVARCHAR (60)      NULL,
    [UserData7]                               NVARCHAR (60)      NULL,
    [UserData8]                               NVARCHAR (60)      NULL,
    [CreatedDate]                             DATETIMEOFFSET (7) CONSTRAINT [DF_tblCompanies_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                               [dbo].[udtUserID]  CONSTRAINT [DF_tblCompanies_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                             DATETIMEOFFSET (7) CONSTRAINT [DF_tblCompanies_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                               [dbo].[udtUserID]  CONSTRAINT [DF_tblCompanies_UpdatedBy] DEFAULT ('') NOT NULL,
    [CompanyGuid]                             UNIQUEIDENTIFIER   CONSTRAINT [DF_tblCompanies_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                             ROWVERSION         NOT NULL,
    [SiteGuid]                                UNIQUEIDENTIFIER   NOT NULL,
    [IATAGuid]                                UNIQUEIDENTIFIER   NULL,
    [ShipperTypeApplicationStringGuid]        UNIQUEIDENTIFIER   NULL,
    [CustomerBillToTypeApplicationStringGuid] UNIQUEIDENTIFIER   NULL,
    [CustomerShipToTypeApplicationStringGuid] UNIQUEIDENTIFIER   NULL,
    [Contact1Name]                            NVARCHAR (30)      NULL,
    [Contact1Address1]                        NVARCHAR (30)      NULL,
    [Contact1Address2]                        NVARCHAR (30)      NULL,
    [Contact1City]                            NVARCHAR (60)      NULL,
    [Contact1State]                           NVARCHAR (20)      NULL,
    [Contact1Zip]                             NVARCHAR (11)      NULL,
    [Contact1Country]                         NVARCHAR (30)      NULL,
    [Contact1PhoneOffice]                     NVARCHAR (20)      NULL,
    [Contact1Fax]                             NVARCHAR (20)      NULL,
    [Contact1EmailAddress]                    NVARCHAR (30)      NULL,
    [Contact2Name]                            NVARCHAR (30)      NULL,
    [Contact2Address1]                        NVARCHAR (30)      NULL,
    [Contact2Address2]                        NVARCHAR (30)      NULL,
    [Contact2City]                            NVARCHAR (60)      NULL,
    [Contact2State]                           NVARCHAR (20)      NULL,
    [Contact2Zip]                             NVARCHAR (11)      NULL,
    [Contact2Country]                         NVARCHAR (30)      NULL,
    [Contact2PhoneOffice]                     NVARCHAR (20)      NULL,
    [Contact2Fax]                             NVARCHAR (20)      NULL,
    [Contact2EmailAddress]                    NVARCHAR (30)      NULL,
    [Contact1PhoneMobile]                     NVARCHAR (20)      NULL,
    [Contact2PhoneMobile]                     NVARCHAR (20)      NULL,
    [_MasterRecordGuid]                       UNIQUEIDENTIFIER   NOT NULL,
    [Note]                                    NVARCHAR (2000)    NULL,
    [HiddenDate]                              DATETIMEOFFSET (7) NULL,
    [_ClusterIdx]                             BIGINT             IDENTITY (1, 1) NOT NULL,
    [ScullyRequired]                          BIT                CONSTRAINT [DF_tblCompanies_ScullyRequired] DEFAULT 0 NOT NULL, 
	[ConsortiumTypeIndex]                     INT                NULL,
	[CompanyIATACode]						  NVARCHAR (50)		 NULL,
	[CompanyICAOCode]						  NVARCHAR (50)		 NULL,
    CONSTRAINT [PK_tblCompanies_GUID] PRIMARY KEY NONCLUSTERED ([CompanyGuid] ASC),
    CONSTRAINT [CK_tblCompanies_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessCompany]([_MasterRecordGuid],[SiteGuid],[ID])=(1)),
    CONSTRAINT [FK_tblCompanies_CustomerBillToTypeApplicationStringGuid] FOREIGN KEY ([CustomerBillToTypeApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblCompanies_CustomerShipToTypeApplicationStringGuid] FOREIGN KEY ([CustomerShipToTypeApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblCompanies_IATAIndexGuid] FOREIGN KEY ([IATAGuid]) REFERENCES [dbo].[tblIATA] ([IATAGuid]),
    CONSTRAINT [FK_tblCompanies_ShipperTypeApplicationStringGuid] FOREIGN KEY ([ShipperTypeApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblCompanies_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblCompanies_ConsortiumTypeIndex] FOREIGN KEY ([ConsortiumTypeIndex]) REFERENCES [lookup].[tblConsortiumType] ([ConsortiumTypeIndex])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblCompanies_CreatedDate]
    ON [dbo].[tblCompanies]([CreatedDate] ASC);



GO
CREATE NONCLUSTERED INDEX [IX_tblCompanies_SiteGuid_ID]
    ON [dbo].[tblCompanies]([SiteGuid] ASC, [ID] ASC)
	INCLUDE([_MasterRecordGuid]);

GO
CREATE NONCLUSTERED INDEX [IX_tblCompanies_Code]
    ON [dbo].[tblCompanies]([Code] ASC)
	INCLUDE([_MasterRecordGuid]);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblCompanies_ID_SiteGuid]
    ON [dbo].[tblCompanies]([ID] ASC, [SiteGuid] ASC)
	INCLUDE([_MasterRecordGuid]);
GO

CREATE  NONCLUSTERED INDEX [IXU_tblCompanies_MasterGuid]
    ON [dbo].[tblCompanies]([_MasterRecordGuid] ASC)
	INCLUDE([ID], [SiteGuid]);
GO


CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblCompanies_SiteGuid_MasterRecordGuid]
    ON [dbo].[tblCompanies]([SiteGuid] ASC, [_MasterRecordGuid] ASC);
GO

-- Make summary journal report happen faster
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblCompanies_GetCompanyRecordVersionsCovering]
    ON [dbo].[tblCompanies]([CompanyGuid] ASC, [_MasterRecordGuid] ASC, [SiteGuid] ASC)


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblCompanies_CompanyGuid_ID]
    ON [dbo].[tblCompanies]([CompanyGuid] ASC, [ID] ASC)
    INCLUDE([Name], [Address1], [City], [State], [LockedOut]);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblCompanies] ON [dbo].[tblCompanies] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblCompanies','D')=1 
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
	INSERT INTO [fmaudit].tblCompanies (
		[ID]
	,	[Code]
	,	[Name]
	,	[ShortName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone]
	,	[FAX]
	,	[EmergencyContact]
	,	[EmergencyPhone]
	,	[FlightPrefix]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[OnHold]
	,	[PickupFLights]
	,	[StockTrack]
	,	[SufferLossGain]
	,	[LowStockWarning]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[ReceivableAccount]
	,	[RefinerCode]
	,	[LastActivityDate]
	,	[CreditOK]
	,	[AdditiveAccounting]
	,	[PurchaseOrderRequired]
	,	[EPANumber]
	,	[FederalID]
	,	[FederalID2]
	,	[FederalID3]
	,	[FederalID4]
	,	[FederalID5]
	,	[StateID]
	,	[TaxNumber]
	,	[FlushPermitted]
	,	[PumpOffPermitted]
	,	[DeliveryToTerminalPermitted]
	,	[LicenseNumber]
	,	[LicenseExpiration]
	,	[InsuranceCompany]
	,	[InsurancePolicy]
	,	[LiabilityAmount]
	,	[HazardousMaterialExclusion]
	,	[InsuranceExpiration]
	,	[AllowDriverEntry]
	,	[PINRequired]
	,	[MaximumVehicleWeight]
	,	[WeightUnits]
	,	[AccountNumber]
	,	[SCACCode]
	,	[DisableOwnerAllocationsCheck]
	,	[DisableShipperAllocationsCheck]
	,	[DisableBillToAllocationsCheck]
	,	[DisableShipToAllocationsCheck]
	,	[LoadRackDisplayText]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[CompanyGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[ShipperTypeApplicationStringGuid]
	,	[CustomerBillToTypeApplicationStringGuid]
	,	[CustomerShipToTypeApplicationStringGuid]
	,	[Contact1Name]
	,	[Contact1Address1]
	,	[Contact1Address2]
	,	[Contact1City]
	,	[Contact1State]
	,	[Contact1Zip]
	,	[Contact1Country]
	,	[Contact1PhoneOffice]
	,	[Contact1Fax]
	,	[Contact1EmailAddress]
	,	[Contact2Name]
	,	[Contact2Address1]
	,	[Contact2Address2]
	,	[Contact2City]
	,	[Contact2State]
	,	[Contact2Zip]
	,	[Contact2Country]
	,	[Contact2PhoneOffice]
	,	[Contact2Fax]
	,	[Contact2EmailAddress]
	,	[Contact1PhoneMobile]
	,	[Contact2PhoneMobile]
	,	[_MasterRecordGuid]
	,	[Note]
	,	[HiddenDate]
	,	[ScullyRequired]
	,	[ConsortiumTypeIndex]
	,	[CompanyIATACode]
	,	[CompanyICAOCode]
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
		d.[ID]
	,	d.[Code]
	,	d.[Name]
	,	d.[ShortName]
	,	d.[Address1]
	,	d.[Address2]
	,	d.[City]
	,	d.[State]
	,	d.[Zip]
	,	d.[Country]
	,	d.[Phone]
	,	d.[FAX]
	,	d.[EmergencyContact]
	,	d.[EmergencyPhone]
	,	d.[FlightPrefix]
	,	d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[OnHold]
	,	d.[PickupFLights]
	,	d.[StockTrack]
	,	d.[SufferLossGain]
	,	d.[LowStockWarning]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[ReceivableAccount]
	,	d.[RefinerCode]
	,	d.[LastActivityDate]
	,	d.[CreditOK]
	,	d.[AdditiveAccounting]
	,	d.[PurchaseOrderRequired]
	,	d.[EPANumber]
	,	d.[FederalID]
	,	d.[FederalID2]
	,	d.[FederalID3]
	,	d.[FederalID4]
	,	d.[FederalID5]
	,	d.[StateID]
	,	d.[TaxNumber]
	,	d.[FlushPermitted]
	,	d.[PumpOffPermitted]
	,	d.[DeliveryToTerminalPermitted]
	,	d.[LicenseNumber]
	,	d.[LicenseExpiration]
	,	d.[InsuranceCompany]
	,	d.[InsurancePolicy]
	,	d.[LiabilityAmount]
	,	d.[HazardousMaterialExclusion]
	,	d.[InsuranceExpiration]
	,	d.[AllowDriverEntry]
	,	d.[PINRequired]
	,	d.[MaximumVehicleWeight]
	,	d.[WeightUnits]
	,	d.[AccountNumber]
	,	d.[SCACCode]
	,	d.[DisableOwnerAllocationsCheck]
	,	d.[DisableShipperAllocationsCheck]
	,	d.[DisableBillToAllocationsCheck]
	,	d.[DisableShipToAllocationsCheck]
	,	d.[LoadRackDisplayText]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[CompanyGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[IATAGuid]
	,	d.[ShipperTypeApplicationStringGuid]
	,	d.[CustomerBillToTypeApplicationStringGuid]
	,	d.[CustomerShipToTypeApplicationStringGuid]
	,	d.[Contact1Name]
	,	d.[Contact1Address1]
	,	d.[Contact1Address2]
	,	d.[Contact1City]
	,	d.[Contact1State]
	,	d.[Contact1Zip]
	,	d.[Contact1Country]
	,	d.[Contact1PhoneOffice]
	,	d.[Contact1Fax]
	,	d.[Contact1EmailAddress]
	,	d.[Contact2Name]
	,	d.[Contact2Address1]
	,	d.[Contact2Address2]
	,	d.[Contact2City]
	,	d.[Contact2State]
	,	d.[Contact2Zip]
	,	d.[Contact2Country]
	,	d.[Contact2PhoneOffice]
	,	d.[Contact2Fax]
	,	d.[Contact2EmailAddress]
	,	d.[Contact1PhoneMobile]
	,	d.[Contact2PhoneMobile]
	,	d.[_MasterRecordGuid]
	,	d.[Note]
	,	d.[HiddenDate]
	,	d.[ScullyRequired]
	,	d.[ConsortiumTypeIndex]
	,	d.[CompanyIATACode]
	,	d.[CompanyICAOCode]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblCompanies] ON [dbo].[tblCompanies] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblCompanies','D')=1 
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
	INSERT INTO [fmaudit].tblCompanies (
		[ID]
	,	[Code]
	,	[Name]
	,	[ShortName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone]
	,	[FAX]
	,	[EmergencyContact]
	,	[EmergencyPhone]
	,	[FlightPrefix]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[OnHold]
	,	[PickupFLights]
	,	[StockTrack]
	,	[SufferLossGain]
	,	[LowStockWarning]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[ReceivableAccount]
	,	[RefinerCode]
	,	[LastActivityDate]
	,	[CreditOK]
	,	[AdditiveAccounting]
	,	[PurchaseOrderRequired]
	,	[EPANumber]
	,	[FederalID]
	,	[FederalID2]
	,	[FederalID3]
	,	[FederalID4]
	,	[FederalID5]
	,	[StateID]
	,	[TaxNumber]
	,	[FlushPermitted]
	,	[PumpOffPermitted]
	,	[DeliveryToTerminalPermitted]
	,	[LicenseNumber]
	,	[LicenseExpiration]
	,	[InsuranceCompany]
	,	[InsurancePolicy]
	,	[LiabilityAmount]
	,	[HazardousMaterialExclusion]
	,	[InsuranceExpiration]
	,	[AllowDriverEntry]
	,	[PINRequired]
	,	[MaximumVehicleWeight]
	,	[WeightUnits]
	,	[AccountNumber]
	,	[SCACCode]
	,	[DisableOwnerAllocationsCheck]
	,	[DisableShipperAllocationsCheck]
	,	[DisableBillToAllocationsCheck]
	,	[DisableShipToAllocationsCheck]
	,	[LoadRackDisplayText]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[CompanyGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[ShipperTypeApplicationStringGuid]
	,	[CustomerBillToTypeApplicationStringGuid]
	,	[CustomerShipToTypeApplicationStringGuid]
	,	[Contact1Name]
	,	[Contact1Address1]
	,	[Contact1Address2]
	,	[Contact1City]
	,	[Contact1State]
	,	[Contact1Zip]
	,	[Contact1Country]
	,	[Contact1PhoneOffice]
	,	[Contact1Fax]
	,	[Contact1EmailAddress]
	,	[Contact2Name]
	,	[Contact2Address1]
	,	[Contact2Address2]
	,	[Contact2City]
	,	[Contact2State]
	,	[Contact2Zip]
	,	[Contact2Country]
	,	[Contact2PhoneOffice]
	,	[Contact2Fax]
	,	[Contact2EmailAddress]
	,	[Contact1PhoneMobile]
	,	[Contact2PhoneMobile]
	,	[_MasterRecordGuid]
	,	[Note]
	,	[HiddenDate]
	,	[ScullyRequired]
	,	[ConsortiumTypeIndex]
	,	[CompanyIATACode]
	,	[CompanyICAOCode]
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
		i.[ID]
	,	i.[Code]
	,	i.[Name]
	,	i.[ShortName]
	,	i.[Address1]
	,	i.[Address2]
	,	i.[City]
	,	i.[State]
	,	i.[Zip]
	,	i.[Country]
	,	i.[Phone]
	,	i.[FAX]
	,	i.[EmergencyContact]
	,	i.[EmergencyPhone]
	,	i.[FlightPrefix]
	,	i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[OnHold]
	,	i.[PickupFLights]
	,	i.[StockTrack]
	,	i.[SufferLossGain]
	,	i.[LowStockWarning]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[ReceivableAccount]
	,	i.[RefinerCode]
	,	i.[LastActivityDate]
	,	i.[CreditOK]
	,	i.[AdditiveAccounting]
	,	i.[PurchaseOrderRequired]
	,	i.[EPANumber]
	,	i.[FederalID]
	,	i.[FederalID2]
	,	i.[FederalID3]
	,	i.[FederalID4]
	,	i.[FederalID5]
	,	i.[StateID]
	,	i.[TaxNumber]
	,	i.[FlushPermitted]
	,	i.[PumpOffPermitted]
	,	i.[DeliveryToTerminalPermitted]
	,	i.[LicenseNumber]
	,	i.[LicenseExpiration]
	,	i.[InsuranceCompany]
	,	i.[InsurancePolicy]
	,	i.[LiabilityAmount]
	,	i.[HazardousMaterialExclusion]
	,	i.[InsuranceExpiration]
	,	i.[AllowDriverEntry]
	,	i.[PINRequired]
	,	i.[MaximumVehicleWeight]
	,	i.[WeightUnits]
	,	i.[AccountNumber]
	,	i.[SCACCode]
	,	i.[DisableOwnerAllocationsCheck]
	,	i.[DisableShipperAllocationsCheck]
	,	i.[DisableBillToAllocationsCheck]
	,	i.[DisableShipToAllocationsCheck]
	,	i.[LoadRackDisplayText]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[CompanyGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[IATAGuid]
	,	i.[ShipperTypeApplicationStringGuid]
	,	i.[CustomerBillToTypeApplicationStringGuid]
	,	i.[CustomerShipToTypeApplicationStringGuid]
	,	i.[Contact1Name]
	,	i.[Contact1Address1]
	,	i.[Contact1Address2]
	,	i.[Contact1City]
	,	i.[Contact1State]
	,	i.[Contact1Zip]
	,	i.[Contact1Country]
	,	i.[Contact1PhoneOffice]
	,	i.[Contact1Fax]
	,	i.[Contact1EmailAddress]
	,	i.[Contact2Name]
	,	i.[Contact2Address1]
	,	i.[Contact2Address2]
	,	i.[Contact2City]
	,	i.[Contact2State]
	,	i.[Contact2Zip]
	,	i.[Contact2Country]
	,	i.[Contact2PhoneOffice]
	,	i.[Contact2Fax]
	,	i.[Contact2EmailAddress]
	,	i.[Contact1PhoneMobile]
	,	i.[Contact2PhoneMobile]
	,	i.[_MasterRecordGuid]
	,	i.[Note]
	,	i.[HiddenDate]
	,	i.[ScullyRequired]
	,	i.[ConsortiumTypeIndex]
	,	i.[CompanyIATACode]
	,	i.[CompanyICAOCode]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblCompanies] ON [dbo].[tblCompanies] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblCompanies','D')=1 
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
	CompanyGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblCompanies (
		[ID]
	,	[Code]
	,	[Name]
	,	[ShortName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone]
	,	[FAX]
	,	[EmergencyContact]
	,	[EmergencyPhone]
	,	[FlightPrefix]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[OnHold]
	,	[PickupFLights]
	,	[StockTrack]
	,	[SufferLossGain]
	,	[LowStockWarning]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[ReceivableAccount]
	,	[RefinerCode]
	,	[LastActivityDate]
	,	[CreditOK]
	,	[AdditiveAccounting]
	,	[PurchaseOrderRequired]
	,	[EPANumber]
	,	[FederalID]
	,	[FederalID2]
	,	[FederalID3]
	,	[FederalID4]
	,	[FederalID5]
	,	[StateID]
	,	[TaxNumber]
	,	[FlushPermitted]
	,	[PumpOffPermitted]
	,	[DeliveryToTerminalPermitted]
	,	[LicenseNumber]
	,	[LicenseExpiration]
	,	[InsuranceCompany]
	,	[InsurancePolicy]
	,	[LiabilityAmount]
	,	[HazardousMaterialExclusion]
	,	[InsuranceExpiration]
	,	[AllowDriverEntry]
	,	[PINRequired]
	,	[MaximumVehicleWeight]
	,	[WeightUnits]
	,	[AccountNumber]
	,	[SCACCode]
	,	[DisableOwnerAllocationsCheck]
	,	[DisableShipperAllocationsCheck]
	,	[DisableBillToAllocationsCheck]
	,	[DisableShipToAllocationsCheck]
	,	[LoadRackDisplayText]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[CompanyGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[ShipperTypeApplicationStringGuid]
	,	[CustomerBillToTypeApplicationStringGuid]
	,	[CustomerShipToTypeApplicationStringGuid]
	,	[Contact1Name]
	,	[Contact1Address1]
	,	[Contact1Address2]
	,	[Contact1City]
	,	[Contact1State]
	,	[Contact1Zip]
	,	[Contact1Country]
	,	[Contact1PhoneOffice]
	,	[Contact1Fax]
	,	[Contact1EmailAddress]
	,	[Contact2Name]
	,	[Contact2Address1]
	,	[Contact2Address2]
	,	[Contact2City]
	,	[Contact2State]
	,	[Contact2Zip]
	,	[Contact2Country]
	,	[Contact2PhoneOffice]
	,	[Contact2Fax]
	,	[Contact2EmailAddress]
	,	[Contact1PhoneMobile]
	,	[Contact2PhoneMobile]
	,	[_MasterRecordGuid]
	,	[Note]
	,	[HiddenDate]
	,	[ScullyRequired]
	,	[ConsortiumTypeIndex]
	,	[CompanyIATACode]
	,	[CompanyICAOCode]
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
	OUTPUT inserted.[CompanyGuid] AS 'CompanyGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ID]
	,	d.[Code]
	,	d.[Name]
	,	d.[ShortName]
	,	d.[Address1]
	,	d.[Address2]
	,	d.[City]
	,	d.[State]
	,	d.[Zip]
	,	d.[Country]
	,	d.[Phone]
	,	d.[FAX]
	,	d.[EmergencyContact]
	,	d.[EmergencyPhone]
	,	d.[FlightPrefix]
	,	d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[OnHold]
	,	d.[PickupFLights]
	,	d.[StockTrack]
	,	d.[SufferLossGain]
	,	d.[LowStockWarning]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[ReceivableAccount]
	,	d.[RefinerCode]
	,	d.[LastActivityDate]
	,	d.[CreditOK]
	,	d.[AdditiveAccounting]
	,	d.[PurchaseOrderRequired]
	,	d.[EPANumber]
	,	d.[FederalID]
	,	d.[FederalID2]
	,	d.[FederalID3]
	,	d.[FederalID4]
	,	d.[FederalID5]
	,	d.[StateID]
	,	d.[TaxNumber]
	,	d.[FlushPermitted]
	,	d.[PumpOffPermitted]
	,	d.[DeliveryToTerminalPermitted]
	,	d.[LicenseNumber]
	,	d.[LicenseExpiration]
	,	d.[InsuranceCompany]
	,	d.[InsurancePolicy]
	,	d.[LiabilityAmount]
	,	d.[HazardousMaterialExclusion]
	,	d.[InsuranceExpiration]
	,	d.[AllowDriverEntry]
	,	d.[PINRequired]
	,	d.[MaximumVehicleWeight]
	,	d.[WeightUnits]
	,	d.[AccountNumber]
	,	d.[SCACCode]
	,	d.[DisableOwnerAllocationsCheck]
	,	d.[DisableShipperAllocationsCheck]
	,	d.[DisableBillToAllocationsCheck]
	,	d.[DisableShipToAllocationsCheck]
	,	d.[LoadRackDisplayText]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[CompanyGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[IATAGuid]
	,	d.[ShipperTypeApplicationStringGuid]
	,	d.[CustomerBillToTypeApplicationStringGuid]
	,	d.[CustomerShipToTypeApplicationStringGuid]
	,	d.[Contact1Name]
	,	d.[Contact1Address1]
	,	d.[Contact1Address2]
	,	d.[Contact1City]
	,	d.[Contact1State]
	,	d.[Contact1Zip]
	,	d.[Contact1Country]
	,	d.[Contact1PhoneOffice]
	,	d.[Contact1Fax]
	,	d.[Contact1EmailAddress]
	,	d.[Contact2Name]
	,	d.[Contact2Address1]
	,	d.[Contact2Address2]
	,	d.[Contact2City]
	,	d.[Contact2State]
	,	d.[Contact2Zip]
	,	d.[Contact2Country]
	,	d.[Contact2PhoneOffice]
	,	d.[Contact2Fax]
	,	d.[Contact2EmailAddress]
	,	d.[Contact1PhoneMobile]
	,	d.[Contact2PhoneMobile]
	,	d.[_MasterRecordGuid]
	,	d.[Note]
	,	d.[HiddenDate]
	,	d.[ScullyRequired]
	,	d.[ConsortiumTypeIndex]
	,	d.[CompanyIATACode]
	,	d.[CompanyICAOCode]
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
 
	INSERT INTO [fmaudit].tblCompanies (
		[ID]
	,	[Code]
	,	[Name]
	,	[ShortName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone]
	,	[FAX]
	,	[EmergencyContact]
	,	[EmergencyPhone]
	,	[FlightPrefix]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[OnHold]
	,	[PickupFLights]
	,	[StockTrack]
	,	[SufferLossGain]
	,	[LowStockWarning]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[ReceivableAccount]
	,	[RefinerCode]
	,	[LastActivityDate]
	,	[CreditOK]
	,	[AdditiveAccounting]
	,	[PurchaseOrderRequired]
	,	[EPANumber]
	,	[FederalID]
	,	[FederalID2]
	,	[FederalID3]
	,	[FederalID4]
	,	[FederalID5]
	,	[StateID]
	,	[TaxNumber]
	,	[FlushPermitted]
	,	[PumpOffPermitted]
	,	[DeliveryToTerminalPermitted]
	,	[LicenseNumber]
	,	[LicenseExpiration]
	,	[InsuranceCompany]
	,	[InsurancePolicy]
	,	[LiabilityAmount]
	,	[HazardousMaterialExclusion]
	,	[InsuranceExpiration]
	,	[AllowDriverEntry]
	,	[PINRequired]
	,	[MaximumVehicleWeight]
	,	[WeightUnits]
	,	[AccountNumber]
	,	[SCACCode]
	,	[DisableOwnerAllocationsCheck]
	,	[DisableShipperAllocationsCheck]
	,	[DisableBillToAllocationsCheck]
	,	[DisableShipToAllocationsCheck]
	,	[LoadRackDisplayText]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[CompanyGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[ShipperTypeApplicationStringGuid]
	,	[CustomerBillToTypeApplicationStringGuid]
	,	[CustomerShipToTypeApplicationStringGuid]
	,	[Contact1Name]
	,	[Contact1Address1]
	,	[Contact1Address2]
	,	[Contact1City]
	,	[Contact1State]
	,	[Contact1Zip]
	,	[Contact1Country]
	,	[Contact1PhoneOffice]
	,	[Contact1Fax]
	,	[Contact1EmailAddress]
	,	[Contact2Name]
	,	[Contact2Address1]
	,	[Contact2Address2]
	,	[Contact2City]
	,	[Contact2State]
	,	[Contact2Zip]
	,	[Contact2Country]
	,	[Contact2PhoneOffice]
	,	[Contact2Fax]
	,	[Contact2EmailAddress]
	,	[Contact1PhoneMobile]
	,	[Contact2PhoneMobile]
	,	[_MasterRecordGuid]
	,	[Note]
	,	[HiddenDate]
	,	[ScullyRequired]
	,	[ConsortiumTypeIndex]
	,	[CompanyIATACode]
	,	[CompanyICAOCode]
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
		i.[ID]
	,	i.[Code]
	,	i.[Name]
	,	i.[ShortName]
	,	i.[Address1]
	,	i.[Address2]
	,	i.[City]
	,	i.[State]
	,	i.[Zip]
	,	i.[Country]
	,	i.[Phone]
	,	i.[FAX]
	,	i.[EmergencyContact]
	,	i.[EmergencyPhone]
	,	i.[FlightPrefix]
	,	i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[OnHold]
	,	i.[PickupFLights]
	,	i.[StockTrack]
	,	i.[SufferLossGain]
	,	i.[LowStockWarning]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[ReceivableAccount]
	,	i.[RefinerCode]
	,	i.[LastActivityDate]
	,	i.[CreditOK]
	,	i.[AdditiveAccounting]
	,	i.[PurchaseOrderRequired]
	,	i.[EPANumber]
	,	i.[FederalID]
	,	i.[FederalID2]
	,	i.[FederalID3]
	,	i.[FederalID4]
	,	i.[FederalID5]
	,	i.[StateID]
	,	i.[TaxNumber]
	,	i.[FlushPermitted]
	,	i.[PumpOffPermitted]
	,	i.[DeliveryToTerminalPermitted]
	,	i.[LicenseNumber]
	,	i.[LicenseExpiration]
	,	i.[InsuranceCompany]
	,	i.[InsurancePolicy]
	,	i.[LiabilityAmount]
	,	i.[HazardousMaterialExclusion]
	,	i.[InsuranceExpiration]
	,	i.[AllowDriverEntry]
	,	i.[PINRequired]
	,	i.[MaximumVehicleWeight]
	,	i.[WeightUnits]
	,	i.[AccountNumber]
	,	i.[SCACCode]
	,	i.[DisableOwnerAllocationsCheck]
	,	i.[DisableShipperAllocationsCheck]
	,	i.[DisableBillToAllocationsCheck]
	,	i.[DisableShipToAllocationsCheck]
	,	i.[LoadRackDisplayText]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[CompanyGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[IATAGuid]
	,	i.[ShipperTypeApplicationStringGuid]
	,	i.[CustomerBillToTypeApplicationStringGuid]
	,	i.[CustomerShipToTypeApplicationStringGuid]
	,	i.[Contact1Name]
	,	i.[Contact1Address1]
	,	i.[Contact1Address2]
	,	i.[Contact1City]
	,	i.[Contact1State]
	,	i.[Contact1Zip]
	,	i.[Contact1Country]
	,	i.[Contact1PhoneOffice]
	,	i.[Contact1Fax]
	,	i.[Contact1EmailAddress]
	,	i.[Contact2Name]
	,	i.[Contact2Address1]
	,	i.[Contact2Address2]
	,	i.[Contact2City]
	,	i.[Contact2State]
	,	i.[Contact2Zip]
	,	i.[Contact2Country]
	,	i.[Contact2PhoneOffice]
	,	i.[Contact2Fax]
	,	i.[Contact2EmailAddress]
	,	i.[Contact1PhoneMobile]
	,	i.[Contact2PhoneMobile]
	,	i.[_MasterRecordGuid]
	,	i.[Note]
	,	i.[HiddenDate]
	,	i.[ScullyRequired]
	,	i.[ConsortiumTypeIndex]
	,	i.[CompanyIATACode]
	,	i.[CompanyICAOCode]
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
			agl.[CompanyGuid]=i.[CompanyGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblCompanies
CREATE TRIGGER dbo.trg_insupd_tblCompanies_ForSync 
   ON dbo.tblCompanies
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
                    ,d.CompanyGuid AS Deleted_PK_CompanyGuid
                    ,i.CompanyGuid AS Inserted_PK_CompanyGuid
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
				    d.CompanyGuid = i.CompanyGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblCompanies As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_CompanyGuid = currentTrackingData.PK_CompanyGuid
 
 
		    INSERT track.tblCompanies (InsertedDate 
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
				    ,PK_CompanyGuid
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
				    ,entityChanges.Inserted_PK_CompanyGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblCompanies As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_CompanyGuid = currentTrackingData.PK_CompanyGuid
)
    END
END 

GO
--Creating Delete Trigger for tblCompanies
CREATE TRIGGER dbo.trg_del_tblCompanies_ForSync 
   ON dbo.tblCompanies
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
						,d.CompanyGuid AS Deleted_PK_CompanyGuid
                        ,d.CompanyGuid AS Inserted_PK_CompanyGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblCompanies As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_CompanyGuid = currentTrackingData.PK_CompanyGuid
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
						,PK_CompanyGuid
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
						,entityChanges.Deleted_PK_CompanyGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblCompanies]
ON [dbo].[tblCompanies]
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
		INSERT INTO fmcdc.[tblCompanies]
		(
		[ID]
		, [Code]
		, [Name]
		, [ShortName]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone]
		, [FAX]
		, [EmergencyContact]
		, [EmergencyPhone]
		, [FlightPrefix]
		, [EffectiveDate]
		, [ExpirationDate]
		, [OnHold]
		, [PickupFLights]
		, [StockTrack]
		, [SufferLossGain]
		, [LowStockWarning]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [ReceivableAccount]
		, [RefinerCode]
		, [LastActivityDate]
		, [CreditOK]
		, [AdditiveAccounting]
		, [PurchaseOrderRequired]
		, [EPANumber]
		, [FederalID]
		, [FederalID2]
		, [FederalID3]
		, [FederalID4]
		, [FederalID5]
		, [StateID]
		, [TaxNumber]
		, [FlushPermitted]
		, [PumpOffPermitted]
		, [DeliveryToTerminalPermitted]
		, [LicenseNumber]
		, [LicenseExpiration]
		, [InsuranceCompany]
		, [InsurancePolicy]
		, [LiabilityAmount]
		, [HazardousMaterialExclusion]
		, [InsuranceExpiration]
		, [AllowDriverEntry]
		, [PINRequired]
		, [MaximumVehicleWeight]
		, [WeightUnits]
		, [AccountNumber]
		, [SCACCode]
		, [DisableOwnerAllocationsCheck]
		, [DisableShipperAllocationsCheck]
		, [DisableBillToAllocationsCheck]
		, [DisableShipToAllocationsCheck]
		, [LoadRackDisplayText]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [CompanyGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [IATAGuid]
		, [ShipperTypeApplicationStringGuid]
		, [CustomerBillToTypeApplicationStringGuid]
		, [CustomerShipToTypeApplicationStringGuid]
		, [Contact1Name]
		, [Contact1Address1]
		, [Contact1Address2]
		, [Contact1City]
		, [Contact1State]
		, [Contact1Zip]
		, [Contact1Country]
		, [Contact1PhoneOffice]
		, [Contact1Fax]
		, [Contact1EmailAddress]
		, [Contact2Name]
		, [Contact2Address1]
		, [Contact2Address2]
		, [Contact2City]
		, [Contact2State]
		, [Contact2Zip]
		, [Contact2Country]
		, [Contact2PhoneOffice]
		, [Contact2Fax]
		, [Contact2EmailAddress]
		, [Contact1PhoneMobile]
		, [Contact2PhoneMobile]
		, [_MasterRecordGuid]
		, [Note]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, [ConsortiumTypeIndex]
		, [CompanyIATACode]
		, [CompanyICAOCode]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ID]
		, [Code]
		, [Name]
		, [ShortName]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone]
		, [FAX]
		, [EmergencyContact]
		, [EmergencyPhone]
		, [FlightPrefix]
		, [EffectiveDate]
		, [ExpirationDate]
		, [OnHold]
		, [PickupFLights]
		, [StockTrack]
		, [SufferLossGain]
		, [LowStockWarning]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [ReceivableAccount]
		, [RefinerCode]
		, [LastActivityDate]
		, [CreditOK]
		, [AdditiveAccounting]
		, [PurchaseOrderRequired]
		, [EPANumber]
		, [FederalID]
		, [FederalID2]
		, [FederalID3]
		, [FederalID4]
		, [FederalID5]
		, [StateID]
		, [TaxNumber]
		, [FlushPermitted]
		, [PumpOffPermitted]
		, [DeliveryToTerminalPermitted]
		, [LicenseNumber]
		, [LicenseExpiration]
		, [InsuranceCompany]
		, [InsurancePolicy]
		, [LiabilityAmount]
		, [HazardousMaterialExclusion]
		, [InsuranceExpiration]
		, [AllowDriverEntry]
		, [PINRequired]
		, [MaximumVehicleWeight]
		, [WeightUnits]
		, [AccountNumber]
		, [SCACCode]
		, [DisableOwnerAllocationsCheck]
		, [DisableShipperAllocationsCheck]
		, [DisableBillToAllocationsCheck]
		, [DisableShipToAllocationsCheck]
		, [LoadRackDisplayText]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [CompanyGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [IATAGuid]
		, [ShipperTypeApplicationStringGuid]
		, [CustomerBillToTypeApplicationStringGuid]
		, [CustomerShipToTypeApplicationStringGuid]
		, [Contact1Name]
		, [Contact1Address1]
		, [Contact1Address2]
		, [Contact1City]
		, [Contact1State]
		, [Contact1Zip]
		, [Contact1Country]
		, [Contact1PhoneOffice]
		, [Contact1Fax]
		, [Contact1EmailAddress]
		, [Contact2Name]
		, [Contact2Address1]
		, [Contact2Address2]
		, [Contact2City]
		, [Contact2State]
		, [Contact2Zip]
		, [Contact2Country]
		, [Contact2PhoneOffice]
		, [Contact2Fax]
		, [Contact2EmailAddress]
		, [Contact1PhoneMobile]
		, [Contact2PhoneMobile]
		, [_MasterRecordGuid]
		, [Note]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, [ConsortiumTypeIndex]
		, [CompanyIATACode]
		, [CompanyICAOCode]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblCompanies]
		(
		[ID]
		, [Code]
		, [Name]
		, [ShortName]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone]
		, [FAX]
		, [EmergencyContact]
		, [EmergencyPhone]
		, [FlightPrefix]
		, [EffectiveDate]
		, [ExpirationDate]
		, [OnHold]
		, [PickupFLights]
		, [StockTrack]
		, [SufferLossGain]
		, [LowStockWarning]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [ReceivableAccount]
		, [RefinerCode]
		, [LastActivityDate]
		, [CreditOK]
		, [AdditiveAccounting]
		, [PurchaseOrderRequired]
		, [EPANumber]
		, [FederalID]
		, [FederalID2]
		, [FederalID3]
		, [FederalID4]
		, [FederalID5]
		, [StateID]
		, [TaxNumber]
		, [FlushPermitted]
		, [PumpOffPermitted]
		, [DeliveryToTerminalPermitted]
		, [LicenseNumber]
		, [LicenseExpiration]
		, [InsuranceCompany]
		, [InsurancePolicy]
		, [LiabilityAmount]
		, [HazardousMaterialExclusion]
		, [InsuranceExpiration]
		, [AllowDriverEntry]
		, [PINRequired]
		, [MaximumVehicleWeight]
		, [WeightUnits]
		, [AccountNumber]
		, [SCACCode]
		, [DisableOwnerAllocationsCheck]
		, [DisableShipperAllocationsCheck]
		, [DisableBillToAllocationsCheck]
		, [DisableShipToAllocationsCheck]
		, [LoadRackDisplayText]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [CompanyGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [IATAGuid]
		, [ShipperTypeApplicationStringGuid]
		, [CustomerBillToTypeApplicationStringGuid]
		, [CustomerShipToTypeApplicationStringGuid]
		, [Contact1Name]
		, [Contact1Address1]
		, [Contact1Address2]
		, [Contact1City]
		, [Contact1State]
		, [Contact1Zip]
		, [Contact1Country]
		, [Contact1PhoneOffice]
		, [Contact1Fax]
		, [Contact1EmailAddress]
		, [Contact2Name]
		, [Contact2Address1]
		, [Contact2Address2]
		, [Contact2City]
		, [Contact2State]
		, [Contact2Zip]
		, [Contact2Country]
		, [Contact2PhoneOffice]
		, [Contact2Fax]
		, [Contact2EmailAddress]
		, [Contact1PhoneMobile]
		, [Contact2PhoneMobile]
		, [_MasterRecordGuid]
		, [Note]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, [ConsortiumTypeIndex]
		, [CompanyIATACode]
		, [CompanyICAOCode]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ID]
		, [Code]
		, [Name]
		, [ShortName]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone]
		, [FAX]
		, [EmergencyContact]
		, [EmergencyPhone]
		, [FlightPrefix]
		, [EffectiveDate]
		, [ExpirationDate]
		, [OnHold]
		, [PickupFLights]
		, [StockTrack]
		, [SufferLossGain]
		, [LowStockWarning]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [ReceivableAccount]
		, [RefinerCode]
		, [LastActivityDate]
		, [CreditOK]
		, [AdditiveAccounting]
		, [PurchaseOrderRequired]
		, [EPANumber]
		, [FederalID]
		, [FederalID2]
		, [FederalID3]
		, [FederalID4]
		, [FederalID5]
		, [StateID]
		, [TaxNumber]
		, [FlushPermitted]
		, [PumpOffPermitted]
		, [DeliveryToTerminalPermitted]
		, [LicenseNumber]
		, [LicenseExpiration]
		, [InsuranceCompany]
		, [InsurancePolicy]
		, [LiabilityAmount]
		, [HazardousMaterialExclusion]
		, [InsuranceExpiration]
		, [AllowDriverEntry]
		, [PINRequired]
		, [MaximumVehicleWeight]
		, [WeightUnits]
		, [AccountNumber]
		, [SCACCode]
		, [DisableOwnerAllocationsCheck]
		, [DisableShipperAllocationsCheck]
		, [DisableBillToAllocationsCheck]
		, [DisableShipToAllocationsCheck]
		, [LoadRackDisplayText]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [CompanyGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [IATAGuid]
		, [ShipperTypeApplicationStringGuid]
		, [CustomerBillToTypeApplicationStringGuid]
		, [CustomerShipToTypeApplicationStringGuid]
		, [Contact1Name]
		, [Contact1Address1]
		, [Contact1Address2]
		, [Contact1City]
		, [Contact1State]
		, [Contact1Zip]
		, [Contact1Country]
		, [Contact1PhoneOffice]
		, [Contact1Fax]
		, [Contact1EmailAddress]
		, [Contact2Name]
		, [Contact2Address1]
		, [Contact2Address2]
		, [Contact2City]
		, [Contact2State]
		, [Contact2Zip]
		, [Contact2Country]
		, [Contact2PhoneOffice]
		, [Contact2Fax]
		, [Contact2EmailAddress]
		, [Contact1PhoneMobile]
		, [Contact2PhoneMobile]
		, [_MasterRecordGuid]
		, [Note]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, [ConsortiumTypeIndex]
		, [CompanyIATACode]
		, [CompanyICAOCode]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblCompanies] ON [dbo].[tblCompanies]
GO





CREATE UNIQUE CLUSTERED INDEX [IX_tblCompanies_ClusterIdx]
    ON [dbo].[tblCompanies]([_ClusterIdx] ASC);

	
GO

ALTER TABLE [dbo].[tblCompanies] WITH CHECK CHECK CONSTRAINT [FK_tblCompanies_IATAIndexGuid];

------------------------------------------------------------------------------------------------------
-- Trigger: [dbo].[CaptureCompaniesChange]
-- Author: Hansraj Bapoo
-- Version/Date: 1.0.003 / 2013-11-07 14:21:10.4470770 -04:00
-- Purpose: Following a dbo.[tblCompanies] record change, copies the latest version of the dbo.[tblCompanies] record that is created/modified, or
-- deleted into a temporary, historical table, for the purpose of capturing historical data for the FuelsManager data-warehouse.
-- Notes:
-- 1. For new records and updates, the trigger captures the current date into the RecordUpdatedDate field. For deletions, the trigger
-- captures the current date into the RecordDeletedDate field. Those dates are captured, and will be used by the data-warehouse ETL process
-- instead of the existing UpdatedDate field, because the UpdatedDate field is only set by the .NET application, and not by the database
-- itself, and is therefore less reliable, than the new ones that are set at the database trigger level.
------------------------------------------------------------------------------------------------------
