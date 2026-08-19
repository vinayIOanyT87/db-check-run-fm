CREATE TABLE [dbo].[tblOperateScreenConfiguration]
(
	[OperateScreenConfigurationGuid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_OperateScreenConfigurationGuid] DEFAULT (newid()),
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[UserGuid] [uniqueidentifier] NOT NULL,
	[ClientIpAddress] NVARCHAR (50) NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_ClientIpAddress] DEFAULT ('0.0.0.0'),
	[ScreenMask] [bigint] NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_ScreenMask] DEFAULT ((1)),
	[CreatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_CreatedDate] DEFAULT (sysdatetimeoffset()),
	[CreatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_CreatedBy] DEFAULT (''),
	[UpdatedDate] [datetimeoffset](7) NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_UpdatedDate] DEFAULT (sysdatetimeoffset()),
	[UpdatedBy] [dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblOperateScreenConfiguration_UpdatedBy] DEFAULT (''),
	[_RowVersion] [timestamp] NOT NULL,
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
	CONSTRAINT [PK_tblOperateScreenConfiguration__OperateScreenConfigurationGuid] PRIMARY KEY NONCLUSTERED ([OperateScreenConfigurationGuid] ASC),
	CONSTRAINT [FK_tblOperateScreenConfiguration_tblSites] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites]([SiteGuid]),
	CONSTRAINT [FK_tblOperateScreenConfiguration_tblUsers] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers]([UserGuid]),
	CONSTRAINT [CK_tblOperateScreenConfiguration_ClientIpAddress_IPv4] CHECK (
		[ClientIpAddress] NOT LIKE N'%[^0-9.]%'
		AND (LEN([ClientIpAddress]) - LEN(REPLACE([ClientIpAddress], N'.', N''))) = 3
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 1)) IS NOT NULL
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 1)) BETWEEN 0 AND 255
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 2)) IS NOT NULL
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 2)) BETWEEN 0 AND 255
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 3)) IS NOT NULL
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 3)) BETWEEN 0 AND 255
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 4)) IS NOT NULL
		AND TRY_CONVERT(INT, PARSENAME([ClientIpAddress], 4)) BETWEEN 0 AND 255
	)
)

GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblOperateScreenConfiguration_UserGuid_SiteGuid_ClientIpAddress]
ON [dbo].[tblOperateScreenConfiguration] ([UserGuid], [SiteGuid], [ClientIpAddress]);

GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblOperateScreenConfiguration] ON [dbo].[tblOperateScreenConfiguration] AFTER DELETE
AS
BEGIN
	SET NOCOUNT ON;

	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo', 'tblOperateScreenConfiguration', 'D') = 1
		RETURN

	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext VARBINARY(128);

	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType = 'D';
	SET @_AuditEventSequence = 1;

	SELECT	@_AuditSessionGUID = s.SessionGuid
		,	@_AuditSessionTokenID = s.SessionTokenID
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m
	INNER JOIN tblSessions s ON m.SessionGuid = s.SessionGuid
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid
	WHERE m.SqlServerSessionID = @@SPID;

	IF ((SELECT TRIGGER_NESTLEVEL()) > 1)
	BEGIN
		SET @_AuditContext = NULL
	END

	IF (@_AuditContext IS NOT NULL)
	BEGIN
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()

	INSERT INTO [fmaudit].tblOperateScreenConfiguration (
		[OperateScreenConfigurationGuid]
	,	[SiteGuid]
	,	[UserGuid]
	,	[ClientIpAddress]
	,	[ScreenMask]
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
		d.[OperateScreenConfigurationGuid]
	,	d.[SiteGuid]
	,	d.[UserGuid]
	,	d.[ClientIpAddress]
	,	d.[ScreenMask]
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

CREATE TRIGGER [dbo].[trg_Audit_ins_tblOperateScreenConfiguration] ON [dbo].[tblOperateScreenConfiguration] AFTER INSERT
AS
BEGIN
	SET NOCOUNT ON;

	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo', 'tblOperateScreenConfiguration', 'D') = 1
		RETURN

	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext VARBINARY(128);

	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType = 'I';
	SET @_AuditEventSequence = 1;

	SELECT	@_AuditSessionGUID = s.SessionGuid
		,	@_AuditSessionTokenID = s.SessionTokenID
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m
	INNER JOIN tblSessions s ON m.SessionGuid = s.SessionGuid
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid
	WHERE m.SqlServerSessionID = @@SPID;

	IF ((SELECT TRIGGER_NESTLEVEL()) > 1)
	BEGIN
		SET @_AuditContext = NULL
	END

	IF (@_AuditContext IS NOT NULL)
	BEGIN
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()

	INSERT INTO [fmaudit].tblOperateScreenConfiguration (
		[OperateScreenConfigurationGuid]
	,	[SiteGuid]
	,	[UserGuid]
	,	[ClientIpAddress]
	,	[ScreenMask]
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
		i.[OperateScreenConfigurationGuid]
	,	i.[SiteGuid]
	,	i.[UserGuid]
	,	i.[ClientIpAddress]
	,	i.[ScreenMask]
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

CREATE TRIGGER [dbo].[trg_Audit_upd_tblOperateScreenConfiguration] ON [dbo].[tblOperateScreenConfiguration] AFTER UPDATE
AS
BEGIN
	SET NOCOUNT ON;

	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo', 'tblOperateScreenConfiguration', 'D') = 1
		RETURN

	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext VARBINARY(128);

	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType = 'U';
	SET @_AuditEventSequence = 1;

	SELECT	@_AuditSessionGUID = s.SessionGuid
		,	@_AuditSessionTokenID = s.SessionTokenID
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m
	INNER JOIN tblSessions s ON m.SessionGuid = s.SessionGuid
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid
	WHERE m.SqlServerSessionID = @@SPID;

	IF ((SELECT TRIGGER_NESTLEVEL()) > 1)
	BEGIN
		SET @_AuditContext = NULL
	END

	IF (@_AuditContext IS NOT NULL)
	BEGIN
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()

	DECLARE @AuditGuidList TABLE
	(
		OperateScreenConfigurationGuid UNIQUEIDENTIFIER NULL
	,	_AuditEventType CHAR(1)
	,	_AuditEventSequence TINYINT
	,	_AuditCreatedDate DATETIMEOFFSET
	,	_AuditGUID UNIQUEIDENTIFIER
	)

	INSERT INTO [fmaudit].tblOperateScreenConfiguration (
		[OperateScreenConfigurationGuid]
	,	[SiteGuid]
	,	[UserGuid]
	,	[ClientIpAddress]
	,	[ScreenMask]
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
	OUTPUT inserted.[OperateScreenConfigurationGuid] AS 'OperateScreenConfigurationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT
		d.[OperateScreenConfigurationGuid]
	,	d.[SiteGuid]
	,	d.[UserGuid]
	,	d.[ClientIpAddress]
	,	d.[ScreenMask]
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

	INSERT INTO [fmaudit].tblOperateScreenConfiguration (
		[OperateScreenConfigurationGuid]
	,	[SiteGuid]
	,	[UserGuid]
	,	[ClientIpAddress]
	,	[ScreenMask]
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
		i.[OperateScreenConfigurationGuid]
	,	i.[SiteGuid]
	,	i.[UserGuid]
	,	i.[ClientIpAddress]
	,	i.[ScreenMask]
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
	INNER JOIN @AuditGuidList agl ON agl.[OperateScreenConfigurationGuid] = i.[OperateScreenConfigurationGuid]
	WHERE agl._AuditEventType = 'U'
	AND agl._AuditEventSequence = 1
	AND agl._AuditCreatedDate = @_AuditDateTime
END

GO
