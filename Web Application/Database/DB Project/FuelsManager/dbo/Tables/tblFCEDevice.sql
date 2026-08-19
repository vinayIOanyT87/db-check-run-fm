CREATE TABLE [dbo].[tblFCEDevice]
(
   [FCEDeviceGuid] UNIQUEIDENTIFIER CONSTRAINT [DF_tblFCEDevice_GUID] DEFAULT (newid()) NOT NULL,
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL,
	[ImeiNumber] nchar(15) NOT NULL,
	[FriendlyName] nchar(30),
	[HeartbeatTimeoutProcessed] Bit NOT NULL,
	[ConfigReady] Bit NOT NULL,
	[MinTime]	int NOT NULL,
	[MaxTime]	int NOT NULL,
	[LevelDeadband] float NOT NULL,
	[TempDeadband] float NOT NULL,
	[Heartbeat] int NOT NULL,
	[TLStanks] smallint NOT NULL,
	[ModbusMap] smallint NOT NULL,
	[MidnightOffset] int NOT NULL,
	[ShortDeadband] float NOT NULL,
	[ShortTime] int NOT NULL,
	[LongDeadband] float NOT NULL,
	[LongTime] int NOT NULL,
	[SoftwareVersion] nchar(32) CONSTRAINT [DF_tblFCEDevice_SoftwareVersion] DEFAULT ('FCE-20221212.1') NOT NULL,
	[CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblFCEDevice_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
	[CreatedBy] [dbo].[udtUserID]  NULL,
	[UpdatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblFCEDevice_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
	[UpdatedBy] [dbo].[udtUserID]  NULL,
	[_RowVersion] ROWVERSION NOT NULL,
	[_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL,
	CONSTRAINT [FK_tblFCEDevice_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
	CONSTRAINT [FK_tblFCEDevice_ImeiNumber] UNIQUE NONCLUSTERED ([ImeiNumber]),
	CONSTRAINT [PK_tblFCEEMapping_FCEDeviceGuid] PRIMARY KEY NONCLUSTERED ([FCEDeviceGuid]),
	)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblFCEDevice_ClusterIdx] 
	ON [dbo].[tblFCEDevice]([_ClusterIdx]);

GO



CREATE TRIGGER [dbo].[trg_Audit_del_tblFCEDevice] ON [dbo].[tblFCEDevice] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblFCEDevice','D')=1 
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
	INSERT INTO [fmaudit].[tblFCEDevice] (
    	[FCEDeviceGuid]
	,	[SiteGuid]
	,	[ImeiNumber]
	,	[FriendlyName]
	,	[HeartbeatTimeoutProcessed]
	,	[ConfigReady]
	,	[MinTime]
	,	[MaxTime]
	,	[LevelDeadband]
	,	[TempDeadband]
	,	[Heartbeat]
	,	[TLStanks]
	,	[ModbusMap]
	,	[MidnightOffset]
	,	[ShortDeadband]
	,	[ShortTime]
	,	[LongDeadband]
	,	[LongTime]
	,	[SoftwareVersion]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
    	d.[FCEDeviceGuid]
	,	d.[SiteGuid]
	,	d.[ImeiNumber]
	,	d.[FriendlyName]
	,	d.[HeartbeatTimeoutProcessed]
	,	d.[ConfigReady]
	,	d.[MinTime]
	,	d.[MaxTime]
	,	d.[LevelDeadband]
	,	d.[TempDeadband]
	,	d.[Heartbeat]
	,	d.[TLStanks]
	,	d.[ModbusMap]
	,	d.[MidnightOffset]
	,	d.[ShortDeadband]
	,	d.[ShortTime]
	,	d.[LongDeadband]
	,	d.[LongTime]
	,	d.[SoftwareVersion]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblFCEDevice] ON [dbo].[tblFCEDevice] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblFCEDevice','D')=1 
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
	INSERT INTO [fmaudit].[tblFCEDevice] (
    	[FCEDeviceGuid]
	,	[SiteGuid]
	,	[ImeiNumber]
	,	[FriendlyName]
	,	[HeartbeatTimeoutProcessed]
	,	[ConfigReady]
	,	[MinTime]
	,	[MaxTime]
	,	[LevelDeadband]
	,	[TempDeadband]
	,	[Heartbeat]
	,	[TLStanks]
	,	[ModbusMap]
	,	[MidnightOffset]
	,	[ShortDeadband]
	,	[ShortTime]
	,	[LongDeadband]
	,	[LongTime]
	,	[SoftwareVersion]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
    	i.[FCEDeviceGuid]
	,	i.[SiteGuid]
	,	i.[ImeiNumber]
	,	i.[FriendlyName]
	,	i.[HeartbeatTimeoutProcessed]
	,	i.[ConfigReady]
	,	i.[MinTime]
	,	i.[MaxTime]
	,	i.[LevelDeadband]
	,	i.[TempDeadband]
	,	i.[Heartbeat]
	,	i.[TLStanks]
	,	i.[ModbusMap]
	,	i.[MidnightOffset]
	,	i.[ShortDeadband]
	,	i.[ShortTime]
	,	i.[LongDeadband]
	,	i.[LongTime]
	,	i.[SoftwareVersion]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblFCEDevice] ON [dbo].[tblFCEDevice] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblFCEDevice','D')=1 
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
		FCEDeviceGuid uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].[tblFCEDevice] (
    	[FCEDeviceGuid]
	,	[SiteGuid]
	,	[ImeiNumber]
	,	[FriendlyName]
	,	[HeartbeatTimeoutProcessed]
	,	[ConfigReady]
	,	[MinTime]
	,	[MaxTime]
	,	[LevelDeadband]
	,	[TempDeadband]
	,	[Heartbeat]
	,	[TLStanks]
	,	[ModbusMap]
	,	[MidnightOffset]
	,	[ShortDeadband]
	,	[ShortTime]
	,	[LongDeadband]
	,	[LongTime]
	,	[SoftwareVersion]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
	OUTPUT inserted.[FCEDeviceGuid] AS 'FCEDeviceGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
    	d.[FCEDeviceGuid]
	,	d.[SiteGuid]
	,	d.[ImeiNumber]
	,	d.[FriendlyName]
	,	d.[HeartbeatTimeoutProcessed]
	,	d.[ConfigReady]
	,	d.[MinTime]
	,	d.[MaxTime]
	,	d.[LevelDeadband]
	,	d.[TempDeadband]
	,	d.[Heartbeat]
	,	d.[TLStanks]
	,	d.[ModbusMap]
	,	d.[MidnightOffset]
	,	d.[ShortDeadband]
	,	d.[ShortTime]
	,	d.[LongDeadband]
	,	d.[LongTime]
	,	d.[SoftwareVersion]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
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
 
	INSERT INTO [fmaudit].[tblFCEDevice] (
    	[FCEDeviceGuid]
	,	[SiteGuid]
	,	[ImeiNumber]
	,	[FriendlyName]
	,	[HeartbeatTimeoutProcessed]
	,	[ConfigReady]
	,	[MinTime]
	,	[MaxTime]
	,	[LevelDeadband]
	,	[TempDeadband]
	,	[Heartbeat]
	,	[TLStanks]
	,	[ModbusMap]
	,	[MidnightOffset]
	,	[ShortDeadband]
	,	[ShortTime]
	,	[LongDeadband]
	,	[LongTime]
	,	[SoftwareVersion]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
    	i.[FCEDeviceGuid]
	,	i.[SiteGuid]
	,	i.[ImeiNumber]
	,	i.[FriendlyName]
	,	i.[HeartbeatTimeoutProcessed]
	,	i.[ConfigReady]
	,	i.[MinTime]
	,	i.[MaxTime]
	,	i.[LevelDeadband]
	,	i.[TempDeadband]
	,	i.[Heartbeat]
	,	i.[TLStanks]
	,	i.[ModbusMap]
	,	i.[MidnightOffset]
	,	i.[ShortDeadband]
	,	i.[ShortTime]
	,	i.[LongDeadband]
	,	i.[LongTime]
	,	i.[SoftwareVersion]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
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
			agl.[FCEDeviceGuid]=i.[FCEDeviceGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END




