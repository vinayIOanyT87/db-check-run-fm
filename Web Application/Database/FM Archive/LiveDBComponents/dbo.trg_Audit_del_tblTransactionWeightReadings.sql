IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trg_Audit_del_tblTransactionWeightReadings]')) 
	DROP TRIGGER [dbo].[trg_Audit_del_tblTransactionWeightReadings]
GO


CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionWeightReadings] ON [dbo].[tblTransactionWeightReadings] AFTER DELETE 
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
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionWeightReadings','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionWeightReadings (
		[CompartmentID]
	,	[BeginQuantityValue]
	,	[RequestedQuantityValue]
	,	[FinalQuantityValue]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransVersion]
	,	[TransactionWeightReadingGuid]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
	,	[FuelsManagerVersionNumber]
	,	[SourceVersionNumber]
	,	[HistoricalFlag]
	,	[VolumetricTopOffFlag]
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
		d.[CompartmentID]
	,	d.[BeginQuantityValue]
	,	d.[RequestedQuantityValue]
	,	d.[FinalQuantityValue]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransVersion]
	,	d.[TransactionWeightReadingGuid]
	,	d.[_RowVersion]
	,	d.[TransactionGuid]
	,	d.[FuelsManagerVersionNumber]
	,	d.[SourceVersionNumber]
	,	d.[HistoricalFlag]
	,	d.[VolumetricTopOffFlag]
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
