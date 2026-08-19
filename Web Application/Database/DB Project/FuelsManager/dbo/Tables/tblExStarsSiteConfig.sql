CREATE TABLE [dbo].[tblExStarsSiteConfig] (
    [SiteGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [ManagerCompanyGuid]      UNIQUEIDENTIFIER   NOT NULL,
    [InterchangeSenderId]     NVARCHAR (15)      NOT NULL,
    [ApplicationSendersCode]  NVARCHAR (15)      NULL,
    [AuthorizationCode]       NVARCHAR (10)      NULL,
    [FeinCode]                NVARCHAR (18)      NULL,
    [SecurityCode]            NVARCHAR (10)      NULL,
    [InfoProviderName]        NVARCHAR (18)      NULL,
    [AbbreviatedProviderName] NVARCHAR (18)      NULL,
    [GroupControlNumber]      NVARCHAR (9)       NULL,
    [IRS_637Registration]     NVARCHAR (18)      NULL,
    [TerminalControlNumber]   NVARCHAR (9)       NULL,
    [ISA05Qualifier]          NVARCHAR (2)       NULL,
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsSiteConfig_CreatedDate] DEFAULT (GETDATE()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsSiteConfig_UpdatedDate] DEFAULT (GETDATE()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  NOT NULL,
    [ExStarsSiteConfigdGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExStarsSiteConfig_GUID] DEFAULT (newid()) NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([ExStarsSiteConfigdGuid] ASC),
    CONSTRAINT [fk_ExStarsSiteConfigManager] FOREIGN KEY ([ManagerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [fk_ExStarsSiteConfigSite] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO
 CREATE NONCLUSTERED INDEX IX_tblExStarsSiteConfig_SiteManager  ON dbo.tblExStarsSiteConfig(
	[SiteGuid] ASC,
	[ManagerCompanyGuid] ASC
)

GO
 CREATE NONCLUSTERED INDEX [IX_tblExStarsSiteConfig_CreatedDate]
    ON [dbo].[tblExStarsSiteConfig]([CreatedDate] ASC);



GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExStarsSiteConfig_ClusterIdx]
    ON [dbo].[tblExStarsSiteConfig]([_ClusterIdx] ASC);

GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblExStarsSiteConfig] ON [dbo].[tblExStarsSiteConfig] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsSiteConfig','D')=1 
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
	ExStarsSiteConfigdGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblExStarsSiteConfig (
		[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[InterchangeSenderId]
	,	[ApplicationSendersCode]
	,	[AuthorizationCode]
	,	[FeinCode]
	,	[SecurityCode]
	,	[InfoProviderName]
	,	[AbbreviatedProviderName]
	,	[GroupControlNumber]
	,	[IRS_637Registration]
	,	[TerminalControlNumber]
	,	[ISA05Qualifier]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsSiteConfigdGuid]
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
	OUTPUT inserted.[ExStarsSiteConfigdGuid] AS 'ExStarsSiteConfigdGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SiteGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[InterchangeSenderId]
	,	d.[ApplicationSendersCode]
	,	d.[AuthorizationCode]
	,	d.[FeinCode]
	,	d.[SecurityCode]
	,	d.[InfoProviderName]
	,	d.[AbbreviatedProviderName]
	,	d.[GroupControlNumber]
	,	d.[IRS_637Registration]
	,	d.[TerminalControlNumber]
	,	d.[ISA05Qualifier]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsSiteConfigdGuid]
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
 
	INSERT INTO [fmaudit].tblExStarsSiteConfig (
		[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[InterchangeSenderId]
	,	[ApplicationSendersCode]
	,	[AuthorizationCode]
	,	[FeinCode]
	,	[SecurityCode]
	,	[InfoProviderName]
	,	[AbbreviatedProviderName]
	,	[GroupControlNumber]
	,	[IRS_637Registration]
	,	[TerminalControlNumber]
	,	[ISA05Qualifier]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsSiteConfigdGuid]
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
		i.[SiteGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[InterchangeSenderId]
	,	i.[ApplicationSendersCode]
	,	i.[AuthorizationCode]
	,	i.[FeinCode]
	,	i.[SecurityCode]
	,	i.[InfoProviderName]
	,	i.[AbbreviatedProviderName]
	,	i.[GroupControlNumber]
	,	i.[IRS_637Registration]
	,	i.[TerminalControlNumber]
	,	i.[ISA05Qualifier]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsSiteConfigdGuid]
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
			agl.[ExStarsSiteConfigdGuid]=i.[ExStarsSiteConfigdGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblExStarsSiteConfig] ON [dbo].[tblExStarsSiteConfig] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsSiteConfig','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsSiteConfig (
		[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[InterchangeSenderId]
	,	[ApplicationSendersCode]
	,	[AuthorizationCode]
	,	[FeinCode]
	,	[SecurityCode]
	,	[InfoProviderName]
	,	[AbbreviatedProviderName]
	,	[GroupControlNumber]
	,	[IRS_637Registration]
	,	[TerminalControlNumber]
	,	[ISA05Qualifier]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsSiteConfigdGuid]
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
		d.[SiteGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[InterchangeSenderId]
	,	d.[ApplicationSendersCode]
	,	d.[AuthorizationCode]
	,	d.[FeinCode]
	,	d.[SecurityCode]
	,	d.[InfoProviderName]
	,	d.[AbbreviatedProviderName]
	,	d.[GroupControlNumber]
	,	d.[IRS_637Registration]
	,	d.[TerminalControlNumber]
	,	d.[ISA05Qualifier]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsSiteConfigdGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblExStarsSiteConfig] ON [dbo].[tblExStarsSiteConfig] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsSiteConfig','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsSiteConfig (
		[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[InterchangeSenderId]
	,	[ApplicationSendersCode]
	,	[AuthorizationCode]
	,	[FeinCode]
	,	[SecurityCode]
	,	[InfoProviderName]
	,	[AbbreviatedProviderName]
	,	[GroupControlNumber]
	,	[IRS_637Registration]
	,	[TerminalControlNumber]
	,	[ISA05Qualifier]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsSiteConfigdGuid]
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
		i.[SiteGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[InterchangeSenderId]
	,	i.[ApplicationSendersCode]
	,	i.[AuthorizationCode]
	,	i.[FeinCode]
	,	i.[SecurityCode]
	,	i.[InfoProviderName]
	,	i.[AbbreviatedProviderName]
	,	i.[GroupControlNumber]
	,	i.[IRS_637Registration]
	,	i.[TerminalControlNumber]
	,	i.[ISA05Qualifier]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsSiteConfigdGuid]
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