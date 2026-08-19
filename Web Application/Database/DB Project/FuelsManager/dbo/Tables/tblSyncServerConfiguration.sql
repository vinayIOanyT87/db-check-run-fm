CREATE TABLE [dbo].[tblSyncServerConfiguration] (
    [SyncServerConfigurationGuid]               UNIQUEIDENTIFIER   NOT NULL,
    [AllowSynchronizationFlag]                  BIT                NOT NULL,
    [AcceptFMUserAuthenticationFlag]            BIT                NOT NULL,
    [AcceptClientCertificateAuthenticationFlag] BIT                NOT NULL,
    [ClientSignatureRequiredForMessagesFlag]    BIT                NOT NULL,
    [ClientEncryptionRequiredForMessagesFlag]   BIT                NOT NULL,
	[NodeHealthCriticalThresholdHours]			INT				   CONSTRAINT [DF_tblSyncServerConfiguration_NodeHealthCriticalThresholdHours] DEFAULT (24) NOT NULL,
	[NodeHealthCautionThresholdHours]			INT				   CONSTRAINT [DF_tblSyncServerConfiguration_NodeHealthCautionThresholdHours] DEFAULT (12) NOT NULL,
    [CreatedDate]                               DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncServerConfiguration_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [CreatedBy]                                 [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncServerConfiguration_CreatedBy] DEFAULT (SUSER_SNAME()) NOT NULL,
    [UpdatedDate]                               DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncServerConfiguration_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [UpdatedBy]                                 [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncServerConfiguration_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                               ROWVERSION         NOT NULL,
    [_ClusterIdx]                               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSyncServerConfiguration] PRIMARY KEY NONCLUSTERED ([SyncServerConfigurationGuid] ASC)
);


GO



GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblSyncServerConfiguration] ON [dbo].[tblSyncServerConfiguration] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSyncServerConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblSyncServerConfiguration (
		[SyncServerConfigurationGuid]
	,	[AllowSynchronizationFlag]
	,	[AcceptFMUserAuthenticationFlag]
	,	[AcceptClientCertificateAuthenticationFlag]
	,	[ClientSignatureRequiredForMessagesFlag]
	,	[ClientEncryptionRequiredForMessagesFlag]
	,	[NodeHealthCriticalThresholdHours]
	,	[NodeHealthCautionThresholdHours]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
		i.[SyncServerConfigurationGuid]
	,	i.[AllowSynchronizationFlag]
	,	i.[AcceptFMUserAuthenticationFlag]
	,	i.[AcceptClientCertificateAuthenticationFlag]
	,	i.[ClientSignatureRequiredForMessagesFlag]
	,	i.[ClientEncryptionRequiredForMessagesFlag]
	,	i.[NodeHealthCriticalThresholdHours]
	,	i.[NodeHealthCautionThresholdHours]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblSyncServerConfiguration] ON [dbo].[tblSyncServerConfiguration] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSyncServerConfiguration','D')=1 
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
	SyncServerConfigurationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblSyncServerConfiguration (
		[SyncServerConfigurationGuid]
	,	[AllowSynchronizationFlag]
	,	[AcceptFMUserAuthenticationFlag]
	,	[AcceptClientCertificateAuthenticationFlag]
	,	[ClientSignatureRequiredForMessagesFlag]
	,	[ClientEncryptionRequiredForMessagesFlag]
	,	[NodeHealthCriticalThresholdHours]
	,	[NodeHealthCautionThresholdHours]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
	OUTPUT inserted.[SyncServerConfigurationGuid] AS 'SyncServerConfigurationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SyncServerConfigurationGuid]
	,	d.[AllowSynchronizationFlag]
	,	d.[AcceptFMUserAuthenticationFlag]
	,	d.[AcceptClientCertificateAuthenticationFlag]
	,	d.[ClientSignatureRequiredForMessagesFlag]
	,	d.[ClientEncryptionRequiredForMessagesFlag]
	,	d.[NodeHealthCriticalThresholdHours]
	,	d.[NodeHealthCautionThresholdHours]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
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
 
	INSERT INTO [fmaudit].tblSyncServerConfiguration (
		[SyncServerConfigurationGuid]
	,	[AllowSynchronizationFlag]
	,	[AcceptFMUserAuthenticationFlag]
	,	[AcceptClientCertificateAuthenticationFlag]
	,	[ClientSignatureRequiredForMessagesFlag]
	,	[ClientEncryptionRequiredForMessagesFlag]
	,	[NodeHealthCriticalThresholdHours]
	,	[NodeHealthCautionThresholdHours]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
		i.[SyncServerConfigurationGuid]
	,	i.[AllowSynchronizationFlag]
	,	i.[AcceptFMUserAuthenticationFlag]
	,	i.[AcceptClientCertificateAuthenticationFlag]
	,	i.[ClientSignatureRequiredForMessagesFlag]
	,	i.[ClientEncryptionRequiredForMessagesFlag]
	,	i.[NodeHealthCriticalThresholdHours]
	,	i.[NodeHealthCautionThresholdHours]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
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
			agl.[SyncServerConfigurationGuid]=i.[SyncServerConfigurationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblSyncServerConfiguration] ON [dbo].[tblSyncServerConfiguration] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSyncServerConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblSyncServerConfiguration (
		[SyncServerConfigurationGuid]
	,	[AllowSynchronizationFlag]
	,	[AcceptFMUserAuthenticationFlag]
	,	[AcceptClientCertificateAuthenticationFlag]
	,	[ClientSignatureRequiredForMessagesFlag]
	,	[ClientEncryptionRequiredForMessagesFlag]
	,	[NodeHealthCriticalThresholdHours]
	,	[NodeHealthCautionThresholdHours]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
		d.[SyncServerConfigurationGuid]
	,	d.[AllowSynchronizationFlag]
	,	d.[AcceptFMUserAuthenticationFlag]
	,	d.[AcceptClientCertificateAuthenticationFlag]
	,	d.[ClientSignatureRequiredForMessagesFlag]
	,	d.[ClientEncryptionRequiredForMessagesFlag]
	,	d.[NodeHealthCriticalThresholdHours]
	,	d.[NodeHealthCautionThresholdHours]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
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
CREATE NONCLUSTERED INDEX [IX_tblSyncServerConfiguration_CreatedDate]
    ON [dbo].[tblSyncServerConfiguration]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncServerConfiguration_ClusterIdx]
    ON [dbo].[tblSyncServerConfiguration]([_ClusterIdx] ASC);

