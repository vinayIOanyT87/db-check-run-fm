IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trg_Audit_del_tblTransactionSubLineItems]')) 
	DROP TRIGGER [dbo].[trg_Audit_del_tblTransactionSubLineItems]
GO


CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE  @context_info varbinary(128)
	DECLARE  @context_info_str varchar(128)
	SELECT @Context_Info = CONTEXT_INFO()  
	SELECT @context_info_str = CAST (@context_info as varchar(128))  
	IF (@context_info_str = 'TransactionArchiving')
	BEGIN				
		RETURN	--The archiving of transactions is not logged
	END

	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionSubLineItems','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionSubLineItems (
		[SequenceID]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[GrossQuantity]
	,	[NetQuantity]
	,	[Vcf]
	,	[Density]
	,	[Temperature]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[BatchNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[TankStatus]
	,	[MeterFactor]
	,	[MeterStart]
	,	[MeterStop]
	,	[MeterStopDateTime]
	,	[MeterStartDateTime]
	,	[FreezePoint]
	,	[DifferentialPressure]
	,	[DosageRate]
	,	[DeleteFlag]
	,	[PresetAmount]
	,	[StorageLocationID]
	,	[MeterID]
	,	[COAID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionInventoryDate]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[ImproperAdditization]
	,	[BrokenBlend]
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
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,	[TransactionSubLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[TransactionLineItemGuid]
	,	[ProductGuid]
	,	[TransactionGuid]
	,	[StorageLocationTankGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
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
	,	d.[Product]
	,	d.[ProductCode]
	,	d.[ProductType]
	,	d.[GrossQuantity]
	,	d.[NetQuantity]
	,	d.[Vcf]
	,	d.[Density]
	,	d.[Temperature]
	,	d.[Customs]
	,	d.[ArmNumber]
	,	d.[LineNumber]
	,	d.[BatchNumber]
	,	d.[LineFill]
	,	d.[BottomVolume]
	,	d.[NetCapacity]
	,	d.[TankStatus]
	,	d.[MeterFactor]
	,	d.[MeterStart]
	,	d.[MeterStop]
	,	d.[MeterStopDateTime]
	,	d.[MeterStartDateTime]
	,	d.[FreezePoint]
	,	d.[DifferentialPressure]
	,	d.[DosageRate]
	,	d.[DeleteFlag]
	,	d.[PresetAmount]
	,	d.[StorageLocationID]
	,	d.[MeterID]
	,	d.[COAID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionInventoryDate]
	,	d.[Tax1]
	,	d.[Tax2]
	,	d.[Tax3]
	,	d.[Tax4]
	,	d.[Tax5]
	,	d.[TransVersion]
	,	d.[ImproperAdditization]
	,	d.[BrokenBlend]
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
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[MassQuantity]
	,	d.[NetManualValueFlag]
	,	d.[MassManualValueFlag]
	,	d.[GrossManualValueFlag]
	,	d.[VcfManualValueFlag]
	,	d.[TransactionSubLineItemGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupQualityIndex]
	,	d.[TransactionLineItemGuid]
	,	d.[ProductGuid]
	,	d.[TransactionGuid]
	,	d.[StorageLocationTankGuid]
	,	d.[MeterGuid]
	,	d.[PackageManualValueFlag]
	,	d.[CleanLineItem]
	,	d.[CleanLineDeductItem]
	,	d.[CleanLineDeductQuantity]
	,	d.[CleanLinePackQuantity]
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
