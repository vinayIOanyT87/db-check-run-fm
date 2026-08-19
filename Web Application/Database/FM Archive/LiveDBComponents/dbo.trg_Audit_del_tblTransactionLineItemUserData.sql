IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trg_Audit_del_tblTransactionLineItemUserData]')) 
	DROP TRIGGER [dbo].[trg_Audit_del_tblTransactionLineItemUserData]
GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData] AFTER DELETE 
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
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItemUserData','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionLineItemUserData (
		[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[UserData9]
	,	[UserData10]
	,	[UserData11]
	,	[UserData12]
	,	[UserData13]
	,	[UserData14]
	,	[UserData15]
	,	[UserData16]
	,	[UserData17]
	,	[UserData18]
	,	[UserData19]
	,	[UserData20]
	,	[UserData21]
	,	[UserData22]
	,	[UserData23]
	,	[UserData24]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionLineItemUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionLineItemGuid]
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
		d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[UserData9]
	,	d.[UserData10]
	,	d.[UserData11]
	,	d.[UserData12]
	,	d.[UserData13]
	,	d.[UserData14]
	,	d.[UserData15]
	,	d.[UserData16]
	,	d.[UserData17]
	,	d.[UserData18]
	,	d.[UserData19]
	,	d.[UserData20]
	,	d.[UserData21]
	,	d.[UserData22]
	,	d.[UserData23]
	,	d.[UserData24]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionLineItemUserDataGuid]
	,	d.[_RowVersion]
	,	d.[TransactionLineItemGuid]
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
