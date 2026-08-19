CREATE TABLE [dbo].[tblSyncClientConfiguration] (
    [SyncClientConfigurationGuid]                 UNIQUEIDENTIFIER   NOT NULL,
    [RootSiteID]                                  NVARCHAR (30)      NULL,
    [EnterpriseURL]                               NVARCHAR (1024)    NULL,
    [SuspendSynchronizationFlag]                  BIT                NOT NULL,
    [ServerAuthUserName]                          NVARCHAR (256)     NULL,
    [ServerAuthPassword]                          VARBINARY (256)    NULL,
    [ServerAuthDomain]                            NVARCHAR (256)     NULL,
    [ServerAuthClientCertificate]                 NVARCHAR (768)     NULL,
    [FMAuthUserName]                              [dbo].[udtUserID]  NULL,
    [FMAuthPassword]                              VARBINARY (256)    NULL,
    [FMAuthClientCertificate]                     NVARCHAR (768)     NULL,
    [MessageSecuritySigningCertificate]           NVARCHAR (768)     NULL,
    [MessageSecurityOfflineEncryptionCertificate] NVARCHAR (768)     NULL,
    [MessageSecurityOfflineDecryptionCertificate] NVARCHAR (768)     NULL,
    [CreatedDate]                                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncClientConfiguration_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [CreatedBy]                                   [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncClientConfiguration_CreatedBy] DEFAULT (SUSER_SNAME()) NOT NULL,
    [UpdatedDate]                                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblSyncClientConfiguration_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [UpdatedBy]                                   [dbo].[udtUserID]  CONSTRAINT [DF_tblSyncClientConfiguration_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                                 ROWVERSION         NOT NULL,
    [ServiceMaximumRetryAttempts]                 INT                NULL,
    [ServiceRetryWaitTime]                        INT                NULL,
    [_ClusterIdx]                                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_dbo.SyncClientConfiguration] PRIMARY KEY NONCLUSTERED ([SyncClientConfigurationGuid] ASC)
);


GO



GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblSyncClientConfiguration] ON [dbo].[tblSyncClientConfiguration] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSyncClientConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblSyncClientConfiguration (
		[SyncClientConfigurationGuid]
	,	[RootSiteID]
	,	[EnterpriseURL]
	,	[SuspendSynchronizationFlag]
	,	[ServerAuthUserName]
	,	[ServerAuthPassword]
	,	[ServerAuthDomain]
	,	[ServerAuthClientCertificate]
	,	[FMAuthUserName]
	,	[FMAuthPassword]
	,	[FMAuthClientCertificate]
	,	[MessageSecuritySigningCertificate]
	,	[MessageSecurityOfflineEncryptionCertificate]
	,	[MessageSecurityOfflineDecryptionCertificate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[ServiceMaximumRetryAttempts]
	,	[ServiceRetryWaitTime]
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
		i.[SyncClientConfigurationGuid]
	,	i.[RootSiteID]
	,	i.[EnterpriseURL]
	,	i.[SuspendSynchronizationFlag]
	,	i.[ServerAuthUserName]
	,	i.[ServerAuthPassword]
	,	i.[ServerAuthDomain]
	,	i.[ServerAuthClientCertificate]
	,	i.[FMAuthUserName]
	,	i.[FMAuthPassword]
	,	i.[FMAuthClientCertificate]
	,	i.[MessageSecuritySigningCertificate]
	,	i.[MessageSecurityOfflineEncryptionCertificate]
	,	i.[MessageSecurityOfflineDecryptionCertificate]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[ServiceMaximumRetryAttempts]
	,	i.[ServiceRetryWaitTime]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblSyncClientConfiguration] ON [dbo].[tblSyncClientConfiguration] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSyncClientConfiguration','D')=1 
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
	SyncClientConfigurationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblSyncClientConfiguration (
		[SyncClientConfigurationGuid]
	,	[RootSiteID]
	,	[EnterpriseURL]
	,	[SuspendSynchronizationFlag]
	,	[ServerAuthUserName]
	,	[ServerAuthPassword]
	,	[ServerAuthDomain]
	,	[ServerAuthClientCertificate]
	,	[FMAuthUserName]
	,	[FMAuthPassword]
	,	[FMAuthClientCertificate]
	,	[MessageSecuritySigningCertificate]
	,	[MessageSecurityOfflineEncryptionCertificate]
	,	[MessageSecurityOfflineDecryptionCertificate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[ServiceMaximumRetryAttempts]
	,	[ServiceRetryWaitTime]
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
	OUTPUT inserted.[SyncClientConfigurationGuid] AS 'SyncClientConfigurationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SyncClientConfigurationGuid]
	,	d.[RootSiteID]
	,	d.[EnterpriseURL]
	,	d.[SuspendSynchronizationFlag]
	,	d.[ServerAuthUserName]
	,	d.[ServerAuthPassword]
	,	d.[ServerAuthDomain]
	,	d.[ServerAuthClientCertificate]
	,	d.[FMAuthUserName]
	,	d.[FMAuthPassword]
	,	d.[FMAuthClientCertificate]
	,	d.[MessageSecuritySigningCertificate]
	,	d.[MessageSecurityOfflineEncryptionCertificate]
	,	d.[MessageSecurityOfflineDecryptionCertificate]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[ServiceMaximumRetryAttempts]
	,	d.[ServiceRetryWaitTime]
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
 
	INSERT INTO [fmaudit].tblSyncClientConfiguration (
		[SyncClientConfigurationGuid]
	,	[RootSiteID]
	,	[EnterpriseURL]
	,	[SuspendSynchronizationFlag]
	,	[ServerAuthUserName]
	,	[ServerAuthPassword]
	,	[ServerAuthDomain]
	,	[ServerAuthClientCertificate]
	,	[FMAuthUserName]
	,	[FMAuthPassword]
	,	[FMAuthClientCertificate]
	,	[MessageSecuritySigningCertificate]
	,	[MessageSecurityOfflineEncryptionCertificate]
	,	[MessageSecurityOfflineDecryptionCertificate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[ServiceMaximumRetryAttempts]
	,	[ServiceRetryWaitTime]
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
		i.[SyncClientConfigurationGuid]
	,	i.[RootSiteID]
	,	i.[EnterpriseURL]
	,	i.[SuspendSynchronizationFlag]
	,	i.[ServerAuthUserName]
	,	i.[ServerAuthPassword]
	,	i.[ServerAuthDomain]
	,	i.[ServerAuthClientCertificate]
	,	i.[FMAuthUserName]
	,	i.[FMAuthPassword]
	,	i.[FMAuthClientCertificate]
	,	i.[MessageSecuritySigningCertificate]
	,	i.[MessageSecurityOfflineEncryptionCertificate]
	,	i.[MessageSecurityOfflineDecryptionCertificate]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[ServiceMaximumRetryAttempts]
	,	i.[ServiceRetryWaitTime]
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
			agl.[SyncClientConfigurationGuid]=i.[SyncClientConfigurationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblSyncClientConfiguration] ON [dbo].[tblSyncClientConfiguration] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSyncClientConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblSyncClientConfiguration (
		[SyncClientConfigurationGuid]
	,	[RootSiteID]
	,	[EnterpriseURL]
	,	[SuspendSynchronizationFlag]
	,	[ServerAuthUserName]
	,	[ServerAuthPassword]
	,	[ServerAuthDomain]
	,	[ServerAuthClientCertificate]
	,	[FMAuthUserName]
	,	[FMAuthPassword]
	,	[FMAuthClientCertificate]
	,	[MessageSecuritySigningCertificate]
	,	[MessageSecurityOfflineEncryptionCertificate]
	,	[MessageSecurityOfflineDecryptionCertificate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[ServiceMaximumRetryAttempts]
	,	[ServiceRetryWaitTime]
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
		d.[SyncClientConfigurationGuid]
	,	d.[RootSiteID]
	,	d.[EnterpriseURL]
	,	d.[SuspendSynchronizationFlag]
	,	d.[ServerAuthUserName]
	,	d.[ServerAuthPassword]
	,	d.[ServerAuthDomain]
	,	d.[ServerAuthClientCertificate]
	,	d.[FMAuthUserName]
	,	d.[FMAuthPassword]
	,	d.[FMAuthClientCertificate]
	,	d.[MessageSecuritySigningCertificate]
	,	d.[MessageSecurityOfflineEncryptionCertificate]
	,	d.[MessageSecurityOfflineDecryptionCertificate]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[ServiceMaximumRetryAttempts]
	,	d.[ServiceRetryWaitTime]
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
CREATE NONCLUSTERED INDEX [IX_tblSyncClientConfiguration_CreatedDate]
    ON [dbo].[tblSyncClientConfiguration]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSyncClientConfiguration_ClusterIdx]
    ON [dbo].[tblSyncClientConfiguration]([_ClusterIdx] ASC);

