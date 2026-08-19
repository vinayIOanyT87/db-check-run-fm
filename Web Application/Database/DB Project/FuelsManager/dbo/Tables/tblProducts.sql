CREATE TABLE [dbo].[tblProducts] (
    [ProductID]                        NVARCHAR (30)      CONSTRAINT [DF_tblProducts_ProductID] DEFAULT ('') NOT NULL,
    [Description]                      NVARCHAR (50)      NULL,
    [GenericType]                      NVARCHAR (10)      NULL,
    [StockResetDate]                   DATETIMEOFFSET (7) NULL,
    [StockTrack]                       BIT                NULL,
    [DensityHighLimit]                 FLOAT (53)         NULL,
    [DensityLowLimit]                  FLOAT (53)         NULL,
    [DensityDeadband]                  FLOAT (53)         NULL,
    [TemperatureHiHiLimit]             FLOAT (53)         NULL,
    [TemperatureHighLimit]             FLOAT (53)         NULL,
    [TemperatureLowLimit]              FLOAT (53)         NULL,
    [TemperatureLoLoLimit]             FLOAT (53)         NULL,
    [TemperatureDeadband]              FLOAT (53)         NULL,
    [Bonded]                           BIT                NULL,
    [LowStockWarning]                  FLOAT (53)         NULL,
    [GroundFuel]                       BIT                NULL,
    [ProductCode]                      NVARCHAR (15)      NULL,
    [Price]                            MONEY              NULL,
    [AviationFuelFlag]                 BIT                NULL,
    [StandardDensity]                  FLOAT (53)         NULL,
    [ApplyVolumeCorrection]            BIT                NULL,
    [ApplyStandardDensity]             BIT                NULL,
    [ApplyDensityLimits]					BIT                NULL,
    [ApplyTemperatureLimits]				BIT                NULL,
    [VolumeUnitIndex]                  INT                NULL,
    [TemperatureUnitIndex]             INT                NULL,
    [DensityUnitIndex]                 INT                NULL,
    [VolumeDecimalPlaces]              TINYINT            NULL,
    [TemperatureDecimalPlaces]         TINYINT            NULL,
    [DensityDecimalPlaces]             TINYINT            NULL,
    [Capitalize]                       BIT                CONSTRAINT [DF_tblProducts_Capitalize] DEFAULT ((0)) NOT NULL,
    [OctaneNumber]                     FLOAT (53)         NULL,
    [ReidVaporPressure]                FLOAT (53)         NULL,
    [HazardousMaterial]                BIT                NULL,
    [RegulatoryClass]                  INT                NULL,
    [LoadRackDisplayText]              NVARCHAR (10)      NULL,
    [ComponentTolerance]               FLOAT (53)         NULL,
    [VaporRecovery]                    BIT                NULL,
    [LockedOut]                        BIT                NULL,
    [LockedOutReason]                  NVARCHAR (80)      NULL,
    [LockedOutDate]                    DATETIMEOFFSET (7) NULL,
    [VarianceTolerance]                FLOAT (53)         NULL,
    [DielectricTolerance]              FLOAT (53)         NULL,
    [LoadByWeight]                     BIT                NULL,
    [PIDXCode]                         NVARCHAR (4)       NULL,
    [ContaminationPromptLoadRackText]  NVARCHAR (10)      NULL,
    [InhibitAccounting]                BIT                NULL,
    [UserData1]                        NVARCHAR (60)      NULL,
    [UserData2]                        NVARCHAR (60)      NULL,
    [UserData3]                        NVARCHAR (60)      NULL,
    [UserData4]                        NVARCHAR (60)      NULL,
    [UserData5]                        NVARCHAR (60)      NULL,
    [UserData6]                        NVARCHAR (60)      NULL,
    [UserData7]                        NVARCHAR (60)      NULL,
    [UserData8]                        NVARCHAR (60)      NULL,
    [CreatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblProducts_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblProducts_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblProducts_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblProducts_UpdatedBy] DEFAULT ('') NOT NULL,
    [MassUnitIndex]                    INT                NULL,
    [LevelUnitIndex]                   INT                NULL,
    [FlowUnitIndex]                    INT                NULL,
    [PressureUnitIndex]                INT                NULL,
    [MassDecimalPlaces]                TINYINT            NULL,
    [LevelDecimalPlaces]               TINYINT            NULL,
    [FlowDecimalPlaces]                TINYINT            NULL,
    [PressureDecimalPlaces]            TINYINT            NULL,
    [VolumePackageSize]                FLOAT (53)         NULL,
    [MassPackageSize]                  FLOAT (53)         NULL,
    [ProductGuid]                      UNIQUEIDENTIFIER   CONSTRAINT [DF_tblProducts_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                      ROWVERSION         NOT NULL,
    [SiteGuid]                         UNIQUEIDENTIFIER   NOT NULL,
    [LookupProductTypeIndex]           INT                NULL,
    [TrackingProductGuid]              UNIQUEIDENTIFIER   NULL,
    [TaxCode]                          NVARCHAR (10)      NULL,
    [VcfModuleSettings]                xml                NULL,
    [ProductColor]                     NVARCHAR(7)        NULL,
    [PatternColor]                     NVARCHAR(7)        NULL,
    [PatternNumber]                    INT					NULL,
    [_MasterRecordGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [HiddenDate]                       DATETIMEOFFSET (7) NULL,
    [AutomaticCloseout]                BIT                NOT NULL,
    [_ClusterIdx]                      BIGINT             IDENTITY (1, 1) NOT NULL,
    [PIDXFamilyCode]							NVARCHAR(4) NULL,
	 [IsEthanol]									BIT					 DEFAULT ((0)) NOT NULL
    CONSTRAINT [PK_tblProducts_GUID] PRIMARY KEY NONCLUSTERED ([ProductGuid] ASC),
    CONSTRAINT [CK_tblProducts_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessProduct]([_MasterRecordGuid],[SiteGuid],[ProductID])=(1)),
    CONSTRAINT [FK_tblProducts_DensityUnitIndex] FOREIGN KEY ([DensityUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblProducts_FlowUnitIndex] FOREIGN KEY ([FlowUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblProducts_LevelUnitIndex] FOREIGN KEY ([LevelUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblProducts_LookupProductTypeIndex] FOREIGN KEY ([LookupProductTypeIndex]) REFERENCES [lookup].[tblProductType] ([ProductTypeIndex]),
    CONSTRAINT [FK_tblProducts_MassUnitIndex] FOREIGN KEY ([MassUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblProducts_PressureUnitIndex] FOREIGN KEY ([PressureUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblProducts_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblProducts_TemperatureUnitIndex] FOREIGN KEY ([TemperatureUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblProducts_TrackingProductGuid] FOREIGN KEY ([TrackingProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
    CONSTRAINT [FK_tblProducts_VolumeUnitIndex] FOREIGN KEY ([VolumeUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex])
);


GO

CREATE CLUSTERED INDEX [IX_tblProducts_ClusterIdx] ON [dbo].[tblProducts](_ClusterIdx ASC)

GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblProducts_ProductID_SiteGuid]
    ON [dbo].[tblProducts]([ProductID] ASC, [SiteGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblProducts_SiteGuid_MasterRecordGuid]
    ON [dbo].[tblProducts]([SiteGuid] ASC, [_MasterRecordGuid] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblProducts] ON [dbo].[tblProducts] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProducts','D')=1 
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
	INSERT INTO [fmaudit].tblProducts (
		[ProductID]
	,	[Description]
	,	[GenericType]
	,	[StockResetDate]
	,	[StockTrack]
	,	[DensityHighLimit]
	,	[DensityLowLimit]
	,	[DensityDeadband]
	,	[TemperatureHiHiLimit]
	,	[TemperatureHighLimit]
	,	[TemperatureLowLimit]
	,	[TemperatureLoLoLimit]
	,	[TemperatureDeadband]
	,	[Bonded]
	,	[LowStockWarning]
	,	[GroundFuel]
	,	[ProductCode]
	,	[Price]
	,	[AviationFuelFlag]
	,	[StandardDensity]
	,	[ApplyVolumeCorrection]
	,	[ApplyStandardDensity]
	,	[ApplyDensityLimits]
	,	[ApplyTemperatureLimits]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[Capitalize]
	,	[OctaneNumber]
	,	[ReidVaporPressure]
	,	[HazardousMaterial]
	,	[RegulatoryClass]
	,	[LoadRackDisplayText]
	,	[ComponentTolerance]
	,	[VaporRecovery]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[VarianceTolerance]
	,	[DielectricTolerance]
	,	[LoadByWeight]
	,	[PIDXCode]
	,	[ContaminationPromptLoadRackText]
	,	[InhibitAccounting]
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
	,	[MassUnitIndex]
	,	[LevelUnitIndex]
	,	[FlowUnitIndex]
	,	[PressureUnitIndex]
	,	[MassDecimalPlaces]
	,	[LevelDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[VolumePackageSize]
	,	[MassPackageSize]
	,	[ProductGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupProductTypeIndex]
	,	[TrackingProductGuid]
	,	[TaxCode]
	,	[ProductColor]
	,	[PatternColor]
	,	[PatternNumber]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[AutomaticCloseout]
	,	[PIDXFamilyCode]
	,	[IsEthanol]
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
		i.[ProductID]
	,	i.[Description]
	,	i.[GenericType]
	,	i.[StockResetDate]
	,	i.[StockTrack]
	,	i.[DensityHighLimit]
	,	i.[DensityLowLimit]
	,	i.[DensityDeadband]
	,	i.[TemperatureHiHiLimit]
	,	i.[TemperatureHighLimit]
	,	i.[TemperatureLowLimit]
	,	i.[TemperatureLoLoLimit]
	,	i.[TemperatureDeadband]
	,	i.[Bonded]
	,	i.[LowStockWarning]
	,	i.[GroundFuel]
	,	i.[ProductCode]
	,	i.[Price]
	,	i.[AviationFuelFlag]
	,	i.[StandardDensity]
	,	i.[ApplyVolumeCorrection]
	,	i.[ApplyStandardDensity]
	,	i.[ApplyDensityLimits]
	,	i.[ApplyTemperatureLimits]
	,	i.[VolumeUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[VolumeDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[Capitalize]
	,	i.[OctaneNumber]
	,	i.[ReidVaporPressure]
	,	i.[HazardousMaterial]
	,	i.[RegulatoryClass]
	,	i.[LoadRackDisplayText]
	,	i.[ComponentTolerance]
	,	i.[VaporRecovery]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[VarianceTolerance]
	,	i.[DielectricTolerance]
	,	i.[LoadByWeight]
	,	i.[PIDXCode]
	,	i.[ContaminationPromptLoadRackText]
	,	i.[InhibitAccounting]
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
	,	i.[MassUnitIndex]
	,	i.[LevelUnitIndex]
	,	i.[FlowUnitIndex]
	,	i.[PressureUnitIndex]
	,	i.[MassDecimalPlaces]
	,	i.[LevelDecimalPlaces]
	,	i.[FlowDecimalPlaces]
	,	i.[PressureDecimalPlaces]
	,	i.[VolumePackageSize]
	,	i.[MassPackageSize]
	,	i.[ProductGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupProductTypeIndex]
	,	i.[TrackingProductGuid]
	,	i.[TaxCode]
	,	i.[ProductColor]
	,	i.[PatternColor]
	,	i.[PatternNumber]
	,	i.[_MasterRecordGuid]
	,	i.[HiddenDate]
	,	i.[AutomaticCloseout]
	,	i.[PIDXFamilyCode]
	,	i.[IsEthanol]
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

CREATE TRIGGER [dbo].[trg_tblProducts_UpdateProduct]
    ON [dbo].[tblProducts]
    AFTER UPDATE
    AS BEGIN
    DECLARE @ProductGuid UNIQUEIDENTIFIER
	DECLARE @SiteGuid UNIQUEIDENTIFIER
	DECLARE @MasterRecordGuid UNIQUEIDENTIFIER
    DECLARE @ProductID nvarchar(30)        
    DECLARE @VolumeUnitIndex int    
    DECLARE @MassUnitIndex int    
    DECLARE @VolumePackageSize float    
    DECLARE @MassPackageSize float    		
	DECLARE @VolumeDecimalPlaces tinyint
	DECLARE @MassDecimalPlaces tinyint
	DECLARE @newVolumeUnitIndex int    
    DECLARE @newMassUnitIndex int    
    DECLARE @newVolumePackageSize float    
    DECLARE @newMassPackageSize float    		
	DECLARE @newVolumeDecimalPlaces tinyint
	DECLARE @newMassDecimalPlaces tinyint
	DECLARE @Msg nvarchar(120)  
	DECLARE @Err  bit 
	DECLARE @ProductInPackageMode bit
	
	SET @Err = 0
	Set @ProductInPackageMode = 0;
	
	SET @Msg = 'Product %s is being used, its '
    SELECT	@ProductGuid = ProductGuid,
			@SiteGuid = deleted.SiteGuid,
			@MasterRecordGuid = deleted._MasterRecordGuid,
			@ProductID = deleted.ProductID,
			@VolumeUnitIndex = deleted.VolumeUnitIndex,
			@MassUnitIndex = deleted.MassUnitIndex,
			@VolumePackageSize = deleted.VolumePackageSize,
			@MassPackageSize = deleted.MassPackageSize,
			@VolumeDecimalPlaces = deleted.VolumeDecimalPlaces,
			@MassDecimalPlaces = deleted.MassDecimalPlaces
    FROM deleted
   
    SELECT	@newVolumeUnitIndex = inserted.VolumeUnitIndex,
			@newMassUnitIndex = inserted.MassUnitIndex,
			@newVolumePackageSize = inserted.VolumePackageSize,
			@newMassPackageSize = inserted.MassPackageSize,
			@newVolumeDecimalPlaces = inserted.VolumeDecimalPlaces,
			@newMassDecimalPlaces = inserted.MassDecimalPlaces
    FROM inserted


	if((@newMassPackageSize > 0 AND @MassPackageSize > 0) OR
	   (@newVolumePackageSize > 0 AND @VolumePackageSize > 0))
	BEGIN
		Set @ProductInPackageMode = 1
	END
	
	
	IF(@ProductInPackageMode = 1)
	BEGIN

		IF (@VolumeUnitIndex <> @newVolumeUnitIndex)
		BEGIN		
			SET @Msg = @Msg + 'Volume Unit, '		
			SET @Err = 1
		END
		IF (@VolumeDecimalPlaces <> @newVolumeDecimalPlaces)
		BEGIN
			SET @Msg = @Msg + 'Volume Decimal Places, '	
			SET @Err = 1
		END	
		IF (@MassUnitIndex <> @newMassUnitIndex)
		BEGIN	
			SET @Msg = @Msg + 'Mass Unit, '
			SET @Err = 1
		END
		IF (@MassDecimalPlaces <> @newMassDecimalPlaces)
		BEGIN
			SET @Msg = @Msg + 'Mass Decimal Places, '
			SET @Err = 1
		END
		IF (@VolumePackageSize <> @newVolumePackageSize)
		BEGIN
			SET @Msg = @Msg + 'Volume Package Size, '
			SET @Err = 1
		END
		IF (@MassPackageSize <> @newMassPackageSize)
		BEGIN
			SET @Msg = @Msg + 'Mass Package Size '
			SET @Err = 1
		END
		IF (  @Err = 1 AND
			(EXISTS ( SELECT TOP 1 t.TransactionGuid 
								FROM tblTransactions t
								INNER JOIN tblTransactionLineItems li on li.TransactionGuid = t.TransactionGuid
								where t.SiteGuid = @SiteGuid AND li.ProductGuid = @MasterRecordGuid and li.DeleteFlag = 0) OR
			 EXISTS ( SELECT TOP 1 t.TransactionGuid 
								FROM tblTransactions t
								INNER JOIN tblTransactionSubLineItems sli on sli.TransactionGuid = t.TransactionGuid
								where t.SiteGuid = @SiteGuid AND sli.ProductGuid = @MasterRecordGuid and sli.DeleteFlag = 0)))
		BEGIN
			update tblProducts 
			set tblProducts.ProductID = @ProductID,
			tblProducts.VolumeUnitIndex = @VolumeUnitIndex,
			tblProducts.MassUnitIndex = @MassUnitIndex,
			tblProducts.VolumePackageSize = @VolumePackageSize,
			tblProducts.MassPackageSize = @MassDecimalPlaces,
			tblProducts.VolumeDecimalPlaces = @VolumeDecimalPlaces,
			tblProducts.MassDecimalPlaces = @MassDecimalPlaces  
			where tblProducts.ProductGuid = @ProductGuid		
			SET @Msg = @Msg + 'cannot be changed.'
			RAISERROR(@Msg, 16, 1,  @ProductID) WITH NOWAIT
		END  
	END
END


GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblProducts] ON [dbo].[tblProducts] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProducts','D')=1 
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
	ProductGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblProducts (
		[ProductID]
	,	[Description]
	,	[GenericType]
	,	[StockResetDate]
	,	[StockTrack]
	,	[DensityHighLimit]
	,	[DensityLowLimit]
	,	[DensityDeadband]
	,	[TemperatureHiHiLimit]
	,	[TemperatureHighLimit]
	,	[TemperatureLowLimit]
	,	[TemperatureLoLoLimit]
	,	[TemperatureDeadband]
	,	[Bonded]
	,	[LowStockWarning]
	,	[GroundFuel]
	,	[ProductCode]
	,	[Price]
	,	[AviationFuelFlag]
	,	[StandardDensity]
	,	[ApplyVolumeCorrection]
	,	[ApplyStandardDensity]
	,	[ApplyDensityLimits]
	,	[ApplyTemperatureLimits]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[Capitalize]
	,	[OctaneNumber]
	,	[ReidVaporPressure]
	,	[HazardousMaterial]
	,	[RegulatoryClass]
	,	[LoadRackDisplayText]
	,	[ComponentTolerance]
	,	[VaporRecovery]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[VarianceTolerance]
	,	[DielectricTolerance]
	,	[LoadByWeight]
	,	[PIDXCode]
	,	[ContaminationPromptLoadRackText]
	,	[InhibitAccounting]
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
	,	[MassUnitIndex]
	,	[LevelUnitIndex]
	,	[FlowUnitIndex]
	,	[PressureUnitIndex]
	,	[MassDecimalPlaces]
	,	[LevelDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[VolumePackageSize]
	,	[MassPackageSize]
	,	[ProductGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupProductTypeIndex]
	,	[TrackingProductGuid]
	,	[TaxCode]
	,	[ProductColor]
	,	[PatternColor]
	,	[PatternNumber]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[AutomaticCloseout]
	,	[PIDXFamilyCode]
	,	[IsEthanol]
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
	OUTPUT inserted.[ProductGuid] AS 'ProductGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ProductID]
	,	d.[Description]
	,	d.[GenericType]
	,	d.[StockResetDate]
	,	d.[StockTrack]
	,	d.[DensityHighLimit]
	,	d.[DensityLowLimit]
	,	d.[DensityDeadband]
	,	d.[TemperatureHiHiLimit]
	,	d.[TemperatureHighLimit]
	,	d.[TemperatureLowLimit]
	,	d.[TemperatureLoLoLimit]
	,	d.[TemperatureDeadband]
	,	d.[Bonded]
	,	d.[LowStockWarning]
	,	d.[GroundFuel]
	,	d.[ProductCode]
	,	d.[Price]
	,	d.[AviationFuelFlag]
	,	d.[StandardDensity]
	,	d.[ApplyVolumeCorrection]
	,	d.[ApplyStandardDensity]
	,	d.[ApplyDensityLimits]
	,	d.[ApplyTemperatureLimits]
	,	d.[VolumeUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[VolumeDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[Capitalize]
	,	d.[OctaneNumber]
	,	d.[ReidVaporPressure]
	,	d.[HazardousMaterial]
	,	d.[RegulatoryClass]
	,	d.[LoadRackDisplayText]
	,	d.[ComponentTolerance]
	,	d.[VaporRecovery]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[VarianceTolerance]
	,	d.[DielectricTolerance]
	,	d.[LoadByWeight]
	,	d.[PIDXCode]
	,	d.[ContaminationPromptLoadRackText]
	,	d.[InhibitAccounting]
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
	,	d.[MassUnitIndex]
	,	d.[LevelUnitIndex]
	,	d.[FlowUnitIndex]
	,	d.[PressureUnitIndex]
	,	d.[MassDecimalPlaces]
	,	d.[LevelDecimalPlaces]
	,	d.[FlowDecimalPlaces]
	,	d.[PressureDecimalPlaces]
	,	d.[VolumePackageSize]
	,	d.[MassPackageSize]
	,	d.[ProductGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupProductTypeIndex]
	,	d.[TrackingProductGuid]
	,	d.[TaxCode]
	,	d.[ProductColor]
	,	d.[PatternColor]
	,	d.[PatternNumber]
	,	d.[_MasterRecordGuid]
	,	d.[HiddenDate]
	,	d.[AutomaticCloseout]
	,	d.[PIDXFamilyCode]
	,	d.[IsEthanol]
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
 
	INSERT INTO [fmaudit].tblProducts (
		[ProductID]
	,	[Description]
	,	[GenericType]
	,	[StockResetDate]
	,	[StockTrack]
	,	[DensityHighLimit]
	,	[DensityLowLimit]
	,	[DensityDeadband]
	,	[TemperatureHiHiLimit]
	,	[TemperatureHighLimit]
	,	[TemperatureLowLimit]
	,	[TemperatureLoLoLimit]
	,	[TemperatureDeadband]
	,	[Bonded]
	,	[LowStockWarning]
	,	[GroundFuel]
	,	[ProductCode]
	,	[Price]
	,	[AviationFuelFlag]
	,	[StandardDensity]
	,	[ApplyVolumeCorrection]
	,	[ApplyStandardDensity]
	,	[ApplyDensityLimits]
	,	[ApplyTemperatureLimits]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[Capitalize]
	,	[OctaneNumber]
	,	[ReidVaporPressure]
	,	[HazardousMaterial]
	,	[RegulatoryClass]
	,	[LoadRackDisplayText]
	,	[ComponentTolerance]
	,	[VaporRecovery]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[VarianceTolerance]
	,	[DielectricTolerance]
	,	[LoadByWeight]
	,	[PIDXCode]
	,	[ContaminationPromptLoadRackText]
	,	[InhibitAccounting]
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
	,	[MassUnitIndex]
	,	[LevelUnitIndex]
	,	[FlowUnitIndex]
	,	[PressureUnitIndex]
	,	[MassDecimalPlaces]
	,	[LevelDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[VolumePackageSize]
	,	[MassPackageSize]
	,	[ProductGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupProductTypeIndex]
	,	[TrackingProductGuid]
	,	[TaxCode]
	,	[ProductColor]
	,	[PatternColor]
	,	[PatternNumber]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[AutomaticCloseout]
	,	[PIDXFamilyCode]
	,	[IsEthanol]
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
		i.[ProductID]
	,	i.[Description]
	,	i.[GenericType]
	,	i.[StockResetDate]
	,	i.[StockTrack]
	,	i.[DensityHighLimit]
	,	i.[DensityLowLimit]
	,	i.[DensityDeadband]
	,	i.[TemperatureHiHiLimit]
	,	i.[TemperatureHighLimit]
	,	i.[TemperatureLowLimit]
	,	i.[TemperatureLoLoLimit]
	,	i.[TemperatureDeadband]
	,	i.[Bonded]
	,	i.[LowStockWarning]
	,	i.[GroundFuel]
	,	i.[ProductCode]
	,	i.[Price]
	,	i.[AviationFuelFlag]
	,	i.[StandardDensity]
	,	i.[ApplyVolumeCorrection]
	,	i.[ApplyStandardDensity]
	,	i.[ApplyDensityLimits]
	,	i.[ApplyTemperatureLimits]
	,	i.[VolumeUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[VolumeDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[Capitalize]
	,	i.[OctaneNumber]
	,	i.[ReidVaporPressure]
	,	i.[HazardousMaterial]
	,	i.[RegulatoryClass]
	,	i.[LoadRackDisplayText]
	,	i.[ComponentTolerance]
	,	i.[VaporRecovery]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[VarianceTolerance]
	,	i.[DielectricTolerance]
	,	i.[LoadByWeight]
	,	i.[PIDXCode]
	,	i.[ContaminationPromptLoadRackText]
	,	i.[InhibitAccounting]
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
	,	i.[MassUnitIndex]
	,	i.[LevelUnitIndex]
	,	i.[FlowUnitIndex]
	,	i.[PressureUnitIndex]
	,	i.[MassDecimalPlaces]
	,	i.[LevelDecimalPlaces]
	,	i.[FlowDecimalPlaces]
	,	i.[PressureDecimalPlaces]
	,	i.[VolumePackageSize]
	,	i.[MassPackageSize]
	,	i.[ProductGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupProductTypeIndex]
	,	i.[TrackingProductGuid]
	,	i.[TaxCode]
	,	i.[ProductColor]
	,	i.[PatternColor]
	,	i.[PatternNumber]
	,	i.[_MasterRecordGuid]
	,	i.[HiddenDate]
	,	i.[AutomaticCloseout]
	,	i.[PIDXFamilyCode]
	,	i.[IsEthanol]
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
			agl.[ProductGuid]=i.[ProductGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblProducts] ON [dbo].[tblProducts] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProducts','D')=1 
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
	INSERT INTO [fmaudit].tblProducts (
		[ProductID]
	,	[Description]
	,	[GenericType]
	,	[StockResetDate]
	,	[StockTrack]
	,	[DensityHighLimit]
	,	[DensityLowLimit]
	,	[DensityDeadband]
	,	[TemperatureHiHiLimit]
	,	[TemperatureHighLimit]
	,	[TemperatureLowLimit]
	,	[TemperatureLoLoLimit]
	,	[TemperatureDeadband]
	,	[Bonded]
	,	[LowStockWarning]
	,	[GroundFuel]
	,	[ProductCode]
	,	[Price]
	,	[AviationFuelFlag]
	,	[StandardDensity]
	,	[ApplyVolumeCorrection]
	,	[ApplyStandardDensity]
	,	[ApplyDensityLimits]
	,	[ApplyTemperatureLimits]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[Capitalize]
	,	[OctaneNumber]
	,	[ReidVaporPressure]
	,	[HazardousMaterial]
	,	[RegulatoryClass]
	,	[LoadRackDisplayText]
	,	[ComponentTolerance]
	,	[VaporRecovery]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[VarianceTolerance]
	,	[DielectricTolerance]
	,	[LoadByWeight]
	,	[PIDXCode]
	,	[ContaminationPromptLoadRackText]
	,	[InhibitAccounting]
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
	,	[MassUnitIndex]
	,	[LevelUnitIndex]
	,	[FlowUnitIndex]
	,	[PressureUnitIndex]
	,	[MassDecimalPlaces]
	,	[LevelDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[VolumePackageSize]
	,	[MassPackageSize]
	,	[ProductGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupProductTypeIndex]
	,	[TrackingProductGuid]
	,	[TaxCode]
	,	[ProductColor]
	,	[PatternColor]
	,	[PatternNumber]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[AutomaticCloseout]
	,	[PIDXFamilyCode]
	,	[IsEthanol]
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
		d.[ProductID]
	,	d.[Description]
	,	d.[GenericType]
	,	d.[StockResetDate]
	,	d.[StockTrack]
	,	d.[DensityHighLimit]
	,	d.[DensityLowLimit]
	,	d.[DensityDeadband]
	,	d.[TemperatureHiHiLimit]
	,	d.[TemperatureHighLimit]
	,	d.[TemperatureLowLimit]
	,	d.[TemperatureLoLoLimit]
	,	d.[TemperatureDeadband]
	,	d.[Bonded]
	,	d.[LowStockWarning]
	,	d.[GroundFuel]
	,	d.[ProductCode]
	,	d.[Price]
	,	d.[AviationFuelFlag]
	,	d.[StandardDensity]
	,	d.[ApplyVolumeCorrection]
	,	d.[ApplyStandardDensity]
	,	d.[ApplyDensityLimits]
	,	d.[ApplyTemperatureLimits]
	,	d.[VolumeUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[VolumeDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[Capitalize]
	,	d.[OctaneNumber]
	,	d.[ReidVaporPressure]
	,	d.[HazardousMaterial]
	,	d.[RegulatoryClass]
	,	d.[LoadRackDisplayText]
	,	d.[ComponentTolerance]
	,	d.[VaporRecovery]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[VarianceTolerance]
	,	d.[DielectricTolerance]
	,	d.[LoadByWeight]
	,	d.[PIDXCode]
	,	d.[ContaminationPromptLoadRackText]
	,	d.[InhibitAccounting]
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
	,	d.[MassUnitIndex]
	,	d.[LevelUnitIndex]
	,	d.[FlowUnitIndex]
	,	d.[PressureUnitIndex]
	,	d.[MassDecimalPlaces]
	,	d.[LevelDecimalPlaces]
	,	d.[FlowDecimalPlaces]
	,	d.[PressureDecimalPlaces]
	,	d.[VolumePackageSize]
	,	d.[MassPackageSize]
	,	d.[ProductGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupProductTypeIndex]
	,	d.[TrackingProductGuid]
	,	d.[TaxCode]
	,	d.[ProductColor]
	,	d.[PatternColor]
	,	d.[PatternNumber]
	,	d.[_MasterRecordGuid]
	,	d.[HiddenDate]
	,	d.[AutomaticCloseout]
	,	d.[PIDXFamilyCode]
	,	d.[IsEthanol]
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
--Creating Insert / Update Trigger for tblProducts
CREATE TRIGGER dbo.trg_insupd_tblProducts_ForSync 
   ON dbo.tblProducts
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
                    ,d.ProductGuid AS Deleted_PK_ProductGuid
                    ,i.ProductGuid AS Inserted_PK_ProductGuid
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
				    d.ProductGuid = i.ProductGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblProducts As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ProductGuid = currentTrackingData.PK_ProductGuid
 
 
		    INSERT track.tblProducts (InsertedDate 
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
				    ,PK_ProductGuid
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
				    ,entityChanges.Inserted_PK_ProductGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblProducts As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ProductGuid = currentTrackingData.PK_ProductGuid
)
    END
END 

GO
--Creating Delete Trigger for tblProducts
CREATE TRIGGER dbo.trg_del_tblProducts_ForSync 
   ON dbo.tblProducts
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
						,d.ProductGuid AS Deleted_PK_ProductGuid
                        ,d.ProductGuid AS Inserted_PK_ProductGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS+1) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblProducts WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ProductGuid = currentTrackingData.PK_ProductGuid
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
						,PK_ProductGuid
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
						,entityChanges.Deleted_PK_ProductGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO


CREATE TRIGGER [dbo].[trg_tblProducts_UpdateProduct_ForPoints]
    ON [dbo].[tblProducts]
    AFTER UPDATE
    AS BEGIN

	SET NOCOUNT ON

	DECLARE @ChangeTrackingSessionGuid UNIQUEIDENTIFIER
	DECLARE @InsertedTrackingSession TABLE( ChangeTrackingSessionGuid UNIQUEIDENTIFIER )


	-- Disable the tracking triggers when executing this trigger.  Disable only for the current SPID
	-- BypassTrackingFlags: Bypass Insert Change Tracking = 0x01
	--						Bypass Update Change Tracking = 0x02
	--						Bypass Delete Change Tracking = 0x04
	--
	-- Bypass all triggers: 0x01 & 0x02 & 0x04
	--

	INSERT [track].[tblChangeTrackingSession]( [ChangeTrackingSessionGuid], [SqlServerSessionID], [ContextName], [BypassTrackingFlags], [BypassReason], [CreatedDate])
	OUTPUT INSERTED.[ChangeTrackingSessionGuid] INTO @InsertedTrackingSession
	SELECT newid(), @@spid, 'usp_PointTagDataUpdate', 0x07, 'Ignore change to values', SYSDATETIMEOFFSET()
	

	-- known Guids for the tags to update
	DECLARE @DensityProductStandard UNIQUEIDENTIFIER,
	@TemperatureProductHiHi UNIQUEIDENTIFIER,
	@TemperatureProductHigh UNIQUEIDENTIFIER,
	@TemperatureProductLow UNIQUEIDENTIFIER,
	@TemperatureProductLoLo UNIQUEIDENTIFIER,
	@DensityProductHigh UNIQUEIDENTIFIER,
	@DensityProductLow UNIQUEIDENTIFIER

	SET @DensityProductStandard  = 'A8998003-ACED-4500-9D63-0B5A83942880'
	SET @TemperatureProductHiHi  = 'AA14DBAE-EDD9-4DA9-8549-4FB5F6C21BAF'
	SET @TemperatureProductHigh  = '8EBAFD8C-48C6-4750-A758-6EB398961BA0'
	SET @TemperatureProductLow  = '26253C9C-90D0-403F-80A8-738DEE390A21'
	SET @TemperatureProductLoLo  = '18721E39-22E0-418F-895E-294568D452BC'
	SET @DensityProductHigh  = '93A68748-F403-4DF6-8C22-849EFB0A5CAE'
	SET @DensityProductLow  = '27CE1BA8-5127-4715-9588-B1B39F782887'

	-- Apply VCF changes
	UPDATE pp
	SET Value = pr.VcfModuleSettings,
	updateddate = SYSDATETIMEOFFSET() 
	FROM tblpoint p
	JOIN inserted pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointproperty pp
	ON pp.PointGuid = p.PointGuid
	AND ValueType= 'FMBusinessObjects.DataObjects.VcfModuleSettings'
	WHERE pr.ApplyVolumeCorrection = 1
	AND cast(pp.Value as nvarchar(max)) <> cast( pr.VcfModuleSettings as nvarchar(max))


	-- Apply standard density
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.StandardDensity, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN inserted pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @DensityProductStandard
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyStandardDensity = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.StandardDensity, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'


	-- Density product High
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.DensityHighLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN inserted pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @DensityProductHigh
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyDensityLimits = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.DensityHighLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'

	-- Density product Low
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.DensityLowLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN inserted pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @DensityProductLow
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyDensityLimits = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.DensityLowLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'

	-- Temperature HiHi limit
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureHiHiLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @TemperatureProductHiHi
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyTemperatureLimits = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureHiHiLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'

	-- Temperature Hi limit
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureHighLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @TemperatureProductHigh
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyTemperatureLimits = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureHighLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'

	-- Temperature Lo limit
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureLowLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @TemperatureProductLow
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyTemperatureLimits = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureLowLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'

	-- Temperature LoLo limit
	UPDATE pp
	SET value = '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureLoLoLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>',
	[status] = 0,
	updateddate = SYSDATETIMEOFFSET(), 
	[ServerTimeStamp] =   SYSDATETIMEOFFSET(),
	[SourceTimeStamp] =   SYSDATETIMEOFFSET()
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @TemperatureProductLoLo
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	WHERE pr.ApplyTemperatureLimits = 1
	AND COALESCE( cast(pp.Value as nvarchar(max)), '' ) <> '<double>' + CAST( dbo.udf_ConvertFromSIUnits( pr.TemperatureLoLoLimit, pp.EngineeringUnitsIndex, pp.DecimalPlaces ) AS nvarchar(MAX)) + '</double>'

	-- Update the Holdoffs (deadband)
	-- Density High Limit
	UPDATE at
	SET Holdoff = dbo.udf_ConvertFromSIUnits( pr.DensityDeadband, pp.EngineeringUnitsIndex, pp.DecimalPlaces )
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @DensityProductHigh
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	JOIN tblAlarmTest at
	ON at.LimitTagGuid = pp.PointTagGuid
	WHERE pr.ApplyDensityLimits = 1
	AND at.Holdoff <> dbo.udf_ConvertFromSIUnits( pr.DensityDeadband, pp.EngineeringUnitsIndex, pp.DecimalPlaces )

	-- Density Low Limit
	UPDATE at
	SET Holdoff = dbo.udf_ConvertFromSIUnits( pr.DensityDeadband, pp.EngineeringUnitsIndex, pp.DecimalPlaces )
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid = @DensityProductLow
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	JOIN tblAlarmTest at
	ON at.LimitTagGuid = pp.PointTagGuid
	WHERE pr.ApplyDensityLimits = 1
	AND at.Holdoff <> dbo.udf_ConvertFromSIUnits( pr.DensityDeadband, pp.EngineeringUnitsIndex, pp.DecimalPlaces )

	-- Temperature HiHi Limit
	UPDATE at
	SET Holdoff = dbo.udf_ConvertFromSIUnits( pr.TemperatureDeadband, pp.EngineeringUnitsIndex, pp.DecimalPlaces )
	FROM tblpoint p
	JOIN tblproducts pr
	ON p.ProductGuid = pr.ProductGuid
	JOIN tblpointtag pp
	ON pp.PointGuid = p.PointGuid
	JOIN tblPointTemplateTag ptt
	ON ptt.WellKnownIdentityGuid IN ( @TemperatureProductHiHi, @TemperatureProductHigh, @TemperatureProductLow, @TemperatureProductLoLo )
	AND pp.PointTemplateTagGuid = ptt.PointTemplateTagGuid
	JOIN tblAlarmTest at
	ON at.LimitTagGuid = pp.PointTagGuid
	WHERE pr.ApplyTemperatureLimits = 1
	AND at.Holdoff <> dbo.udf_ConvertFromSIUnits( pr.TemperatureDeadband, pp.EngineeringUnitsIndex, pp.DecimalPlaces )

	UPDATE p SET [UpdatedDate] = GetDate()
	FROM [dbo].[tblPoint] p
	JOIN inserted pr
	ON p.ProductGuid = pr.ProductGuid

	-- Re-enable the tracking triggers
	SELECT @ChangeTrackingSessionGuid = [ChangeTrackingSessionGuid]
	FROM @InsertedTrackingSession

	DELETE 
	FROM [track].[tblChangeTrackingSession]
	WHERE [ChangeTrackingSessionGuid] = @ChangeTrackingSessionGuid


END
GO




CREATE TRIGGER [dbo].[trg_fmcdc_tblProducts]
ON [dbo].[tblProducts]
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
		INSERT INTO fmcdc.[tblProducts]
		(
		[ProductID]
		, [Description]
		, [GenericType]
		, [StockResetDate]
		, [StockTrack]
		, [DensityHighLimit]
		, [DensityLowLimit]
		, [DensityDeadband]
		, [TemperatureHiHiLimit]
		, [TemperatureHighLimit]
		, [TemperatureLowLimit]
		, [TemperatureLoLoLimit]
		, [TemperatureDeadband]
		, [Bonded]
		, [LowStockWarning]
		, [GroundFuel]
		, [ProductCode]
		, [Price]
		, [AviationFuelFlag]
		, [StandardDensity]
		, [ApplyVolumeCorrection]
		, [ApplyStandardDensity]
		, [ApplyDensityLimits]
		, [ApplyTemperatureLimits]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [Capitalize]
		, [OctaneNumber]
		, [ReidVaporPressure]
		, [HazardousMaterial]
		, [RegulatoryClass]
		, [LoadRackDisplayText]
		, [ComponentTolerance]
		, [VaporRecovery]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [VarianceTolerance]
		, [DielectricTolerance]
		, [LoadByWeight]
		, [PIDXCode]
		, [ContaminationPromptLoadRackText]
		, [InhibitAccounting]
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
		, [MassUnitIndex]
		, [LevelUnitIndex]
		, [FlowUnitIndex]
		, [PressureUnitIndex]
		, [MassDecimalPlaces]
		, [LevelDecimalPlaces]
		, [FlowDecimalPlaces]
		, [PressureDecimalPlaces]
		, [VolumePackageSize]
		, [MassPackageSize]
		, [ProductGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [LookupProductTypeIndex]
		, [TrackingProductGuid]
		, [TaxCode]
		, [VcfModuleSettings]
		, [ProductColor]
		, [PatternColor]
		, [PatternNumber]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [AutomaticCloseout]
		, [_ClusterIdx]
		, [PIDXFamilyCode]
		, [IsEthanol]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ProductID]
		, [Description]
		, [GenericType]
		, [StockResetDate]
		, [StockTrack]
		, [DensityHighLimit]
		, [DensityLowLimit]
		, [DensityDeadband]
		, [TemperatureHiHiLimit]
		, [TemperatureHighLimit]
		, [TemperatureLowLimit]
		, [TemperatureLoLoLimit]
		, [TemperatureDeadband]
		, [Bonded]
		, [LowStockWarning]
		, [GroundFuel]
		, [ProductCode]
		, [Price]
		, [AviationFuelFlag]
		, [StandardDensity]
		, [ApplyVolumeCorrection]
		, [ApplyStandardDensity]
		, [ApplyDensityLimits]
		, [ApplyTemperatureLimits]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [Capitalize]
		, [OctaneNumber]
		, [ReidVaporPressure]
		, [HazardousMaterial]
		, [RegulatoryClass]
		, [LoadRackDisplayText]
		, [ComponentTolerance]
		, [VaporRecovery]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [VarianceTolerance]
		, [DielectricTolerance]
		, [LoadByWeight]
		, [PIDXCode]
		, [ContaminationPromptLoadRackText]
		, [InhibitAccounting]
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
		, [MassUnitIndex]
		, [LevelUnitIndex]
		, [FlowUnitIndex]
		, [PressureUnitIndex]
		, [MassDecimalPlaces]
		, [LevelDecimalPlaces]
		, [FlowDecimalPlaces]
		, [PressureDecimalPlaces]
		, [VolumePackageSize]
		, [MassPackageSize]
		, [ProductGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [LookupProductTypeIndex]
		, [TrackingProductGuid]
		, [TaxCode]
		, [VcfModuleSettings]
		, [ProductColor]
		, [PatternColor]
		, [PatternNumber]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [AutomaticCloseout]
		, [_ClusterIdx]
		, [PIDXFamilyCode]
		, [IsEthanol]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblProducts]
		(
		[ProductID]
		, [Description]
		, [GenericType]
		, [StockResetDate]
		, [StockTrack]
		, [DensityHighLimit]
		, [DensityLowLimit]
		, [DensityDeadband]
		, [TemperatureHiHiLimit]
		, [TemperatureHighLimit]
		, [TemperatureLowLimit]
		, [TemperatureLoLoLimit]
		, [TemperatureDeadband]
		, [Bonded]
		, [LowStockWarning]
		, [GroundFuel]
		, [ProductCode]
		, [Price]
		, [AviationFuelFlag]
		, [StandardDensity]
		, [ApplyVolumeCorrection]
		, [ApplyStandardDensity]
		, [ApplyDensityLimits]
		, [ApplyTemperatureLimits]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [Capitalize]
		, [OctaneNumber]
		, [ReidVaporPressure]
		, [HazardousMaterial]
		, [RegulatoryClass]
		, [LoadRackDisplayText]
		, [ComponentTolerance]
		, [VaporRecovery]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [VarianceTolerance]
		, [DielectricTolerance]
		, [LoadByWeight]
		, [PIDXCode]
		, [ContaminationPromptLoadRackText]
		, [InhibitAccounting]
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
		, [MassUnitIndex]
		, [LevelUnitIndex]
		, [FlowUnitIndex]
		, [PressureUnitIndex]
		, [MassDecimalPlaces]
		, [LevelDecimalPlaces]
		, [FlowDecimalPlaces]
		, [PressureDecimalPlaces]
		, [VolumePackageSize]
		, [MassPackageSize]
		, [ProductGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [LookupProductTypeIndex]
		, [TrackingProductGuid]
		, [TaxCode]
		, [VcfModuleSettings]
		, [ProductColor]
		, [PatternColor]
		, [PatternNumber]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [AutomaticCloseout]
		, [_ClusterIdx]
		, [PIDXFamilyCode]
		, [IsEthanol]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ProductID]
		, [Description]
		, [GenericType]
		, [StockResetDate]
		, [StockTrack]
		, [DensityHighLimit]
		, [DensityLowLimit]
		, [DensityDeadband]
		, [TemperatureHiHiLimit]
		, [TemperatureHighLimit]
		, [TemperatureLowLimit]
		, [TemperatureLoLoLimit]
		, [TemperatureDeadband]
		, [Bonded]
		, [LowStockWarning]
		, [GroundFuel]
		, [ProductCode]
		, [Price]
		, [AviationFuelFlag]
		, [StandardDensity]
		, [ApplyVolumeCorrection]
		, [ApplyStandardDensity]
		, [ApplyDensityLimits]
		, [ApplyTemperatureLimits]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [Capitalize]
		, [OctaneNumber]
		, [ReidVaporPressure]
		, [HazardousMaterial]
		, [RegulatoryClass]
		, [LoadRackDisplayText]
		, [ComponentTolerance]
		, [VaporRecovery]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [VarianceTolerance]
		, [DielectricTolerance]
		, [LoadByWeight]
		, [PIDXCode]
		, [ContaminationPromptLoadRackText]
		, [InhibitAccounting]
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
		, [MassUnitIndex]
		, [LevelUnitIndex]
		, [FlowUnitIndex]
		, [PressureUnitIndex]
		, [MassDecimalPlaces]
		, [LevelDecimalPlaces]
		, [FlowDecimalPlaces]
		, [PressureDecimalPlaces]
		, [VolumePackageSize]
		, [MassPackageSize]
		, [ProductGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [LookupProductTypeIndex]
		, [TrackingProductGuid]
		, [TaxCode]
		, [VcfModuleSettings]
		, [ProductColor]
		, [PatternColor]
		, [PatternNumber]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [AutomaticCloseout]
		, [_ClusterIdx]
		, [PIDXFamilyCode]
		, [IsEthanol]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblProducts] ON [dbo].[tblProducts]
GO