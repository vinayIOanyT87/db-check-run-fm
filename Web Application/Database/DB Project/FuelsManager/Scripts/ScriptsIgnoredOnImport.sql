
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAggProductByTransDay] ON [dbo].[tblAggProductByTransDay] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAggProductByTransDay','D')=1 
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
	index	int NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAggProductByTransDay (
		[index]
	,	[SiteGuid]
	,	[TransDate]
	,	[ProductGuid]
	,	[Total]
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
	OUTPUT inserted.[index] AS 'index'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[index]
	,	d.[SiteGuid]
	,	d.[TransDate]
	,	d.[ProductGuid]
	,	d.[Total]
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
 
	INSERT INTO [fmaudit].tblAggProductByTransDay (
		[index]
	,	[SiteGuid]
	,	[TransDate]
	,	[ProductGuid]
	,	[Total]
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
		i.[index]
	,	i.[SiteGuid]
	,	i.[TransDate]
	,	i.[ProductGuid]
	,	i.[Total]
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
			agl.[index]=i.[index] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO

--Syntax Error: Incorrect syntax near index.
--CREATE TRIGGER [dbo].[trg_Audit_upd_tblAggProductByTransDay] ON [dbo].[tblAggProductByTransDay] AFTER UPDATE 
--AS
--BEGIN
--	SET NOCOUNT ON;
--	-- Verifies whether the trigger is active based on configuration and Audit
--	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
--	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAggProductByTransDay','D')=1 
--		RETURN
--	DECLARE @_AuditEventType CHAR(1)
--	,	@_AuditEventSequence TINYINT
--	,	@_AuditSessionGUID UNIQUEIDENTIFIER
--	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
--	,	@_AuditSiteGUID UNIQUEIDENTIFIER
--	,	@_AuditGUID UNIQUEIDENTIFIER
--	,	@_AuditDateTime DATETIMEOFFSET
--	,	@_UserId NVARCHAR(100)
--	,	@_AuditContext varbinary(128);
--	SET @_AuditDateTime = SYSDATETIMEOFFSET();
--	SET @_AuditEventType= 'U' -- For Updates 
--	SET @_AuditEventSequence= 1 
--	SELECT	@_AuditSessionGUID=s.SessionGuid 
--		,	@_AuditSessionTokenID=s.SessionTokenID 
--		,	@_AuditSiteGUID=s.SiteGuid
--		,	@_UserId=u.UserId
--		,	@_AuditContext=s.SynchronizationNodeGuid
--	FROM map.tblSessionToSQLProcess m 
--	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
--	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
--	WHERE m.SqlServerSessionID=@@SPID 
--
--	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
--	-- Treat the change as a local change so it can be synchronized back to the remote system. 
--	IF ((SELECT trigger_nestlevel()) > 1) 
--	BEGIN 
--		SET @_AuditContext = NULL 
--	END 
--
--	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
--	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
--	IF (@_AuditContext IS NOT NULL) 
--	BEGIN 
--		RETURN
--	END
--
--	IF @_UserId IS NULL
--		SET @_UserId = SUSER_NAME()
-- 
--	DECLARE @AuditGuidList TABLE
--	(
--	index	int NULL
--		,_AuditEventType CHAR(1)
--		,_AuditEventSequence TINYINT
--		,_AuditCreatedDate DATETIMEOFFSET
--		,_AuditGUID UNIQUEIDENTIFIER
--	)
-- 
--	INSERT INTO [fmaudit].tblAggProductByTransDay (
--		[index]
--	,	[SiteGuid]
--	,	[TransDate]
--	,	[ProductGuid]
--	,	[Total]
--	,	[_AuditEventType]
--	,	[_AuditEventSequence]
--	,	[_AuditSessionGUID]
--	,	[_AuditSessionTokenID]
--	,	[_AuditCreatedDate]
--	,	[_AuditSiteGUID]
--	,	[_AuditGUID]
--	,	[_AuditUserId]
--	,	[_AuditContext]
--	)
--	OUTPUT inserted.[index] AS 'index'
--		,  inserted._AuditEventType AS '_AuditEventType'
--		,  inserted._AuditEventSequence AS '_AuditEventSequence'
--		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
--		,  inserted._AuditGUID AS '_AuditGUID'
--		INTO @AuditGuidList
--	SELECT 
--		d.[index]
--	,	d.[SiteGuid]
--	,	d.[TransDate]
--	,	d.[ProductGuid]
--	,	d.[Total]
--	,	@_AuditEventType
--	,	@_AuditEventSequence
--	,	@_AuditSessionGUID
--	,	@_AuditSessionTokenID
--	,	@_AuditDateTime
--	,	@_AuditSiteGUID
--	,	NEWID()
--	,	@_UserId
--	,	@_AuditContext
--	FROM deleted d
-- 
--	INSERT INTO [fmaudit].tblAggProductByTransDay (
--		[index]
--	,	[SiteGuid]
--	,	[TransDate]
--	,	[ProductGuid]
--	,	[Total]
--	,	[_AuditEventType]
--	,	[_AuditEventSequence]
--	,	[_AuditSessionGUID]
--	,	[_AuditSessionTokenID]
--	,	[_AuditCreatedDate]
--	,	[_AuditSiteGUID]
--	,	[_AuditGUID]
--	,	[_AuditUserId]
--	,	[_AuditContext]
--	)
--	SELECT 
--		i.[index]
--	,	i.[SiteGuid]
--	,	i.[TransDate]
--	,	i.[ProductGuid]
--	,	i.[Total]
--	,	@_AuditEventType
--	,	2
--	,	@_AuditSessionGUID
--	,	@_AuditSessionTokenID
--	,	@_AuditDateTime
--	,	@_AuditSiteGUID
--	,	agl._AuditGUID
--	,	@_UserId
--	,	@_AuditContext
--	FROM inserted i 
--	INNER JOIN	@AuditGuidList agl ON
--		(
--			agl.[index]=i.[index] 
--		)
--		WHERE	agl._AuditEventType='U'
--		AND		agl._AuditEventSequence=1 
--		AND		agl._AuditCreatedDate= @_AuditDatetime
--END

GO

CREATE TRIGGER [dbo].[trg_Audit_upd_tblErrorNotificationConfigurations] ON [dbo].[tblErrorNotificationConfigurations] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblErrorNotificationConfigurations','D')=1 
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
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblErrorNotificationConfigurations (
		[SiteGuid]
	,	[EmailAddresses]
	,	[ErrorFolder]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SiteGuid]
	,	d.[EmailAddresses]
	,	d.[ErrorFolder]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
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
 
	INSERT INTO [fmaudit].tblErrorNotificationConfigurations (
		[SiteGuid]
	,	[EmailAddresses]
	,	[ErrorFolder]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
	,	i.[EmailAddresses]
	,	i.[ErrorFolder]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
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
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO

--Syntax Error: Incorrect syntax near ,.
--CREATE TRIGGER [dbo].[trg_Audit_upd_tblErrorNotificationConfigurations] ON [dbo].[tblErrorNotificationConfigurations] AFTER UPDATE 
--AS
--BEGIN
--	SET NOCOUNT ON;
--	-- Verifies whether the trigger is active based on configuration and Audit
--	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
--	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblErrorNotificationConfigurations','D')=1 
--		RETURN
--	DECLARE @_AuditEventType CHAR(1)
--	,	@_AuditEventSequence TINYINT
--	,	@_AuditSessionGUID UNIQUEIDENTIFIER
--	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
--	,	@_AuditSiteGUID UNIQUEIDENTIFIER
--	,	@_AuditGUID UNIQUEIDENTIFIER
--	,	@_AuditDateTime DATETIMEOFFSET
--	,	@_UserId NVARCHAR(100)
--	,	@_AuditContext varbinary(128);
--	SET @_AuditDateTime = SYSDATETIMEOFFSET();
--	SET @_AuditEventType= 'U' -- For Updates 
--	SET @_AuditEventSequence= 1 
--	SELECT	@_AuditSessionGUID=s.SessionGuid 
--		,	@_AuditSessionTokenID=s.SessionTokenID 
--		,	@_AuditSiteGUID=s.SiteGuid
--		,	@_UserId=u.UserId
--		,	@_AuditContext=s.SynchronizationNodeGuid
--	FROM map.tblSessionToSQLProcess m 
--	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
--	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
--	WHERE m.SqlServerSessionID=@@SPID 
--
--	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
--	-- Treat the change as a local change so it can be synchronized back to the remote system. 
--	IF ((SELECT trigger_nestlevel()) > 1) 
--	BEGIN 
--		SET @_AuditContext = NULL 
--	END 
--
--	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
--	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
--	IF (@_AuditContext IS NOT NULL) 
--	BEGIN 
--		RETURN
--	END
--
--	IF @_UserId IS NULL
--		SET @_UserId = SUSER_NAME()
-- 
--	DECLARE @AuditGuidList TABLE
--	(
--		,_AuditEventType CHAR(1)
--		,_AuditEventSequence TINYINT
--		,_AuditCreatedDate DATETIMEOFFSET
--		,_AuditGUID UNIQUEIDENTIFIER
--	)
-- 
--	INSERT INTO [fmaudit].tblErrorNotificationConfigurations (
--		[SiteGuid]
--	,	[EmailAddresses]
--	,	[ErrorFolder]
--	,	[CreatedBy]
--	,	[CreatedDate]
--	,	[UpdatedBy]
--	,	[UpdatedDate]
--	,	[_AuditEventType]
--	,	[_AuditEventSequence]
--	,	[_AuditSessionGUID]
--	,	[_AuditSessionTokenID]
--	,	[_AuditCreatedDate]
--	,	[_AuditSiteGUID]
--	,	[_AuditGUID]
--	,	[_AuditUserId]
--	,	[_AuditContext]
--	)
--		,  inserted._AuditEventType AS '_AuditEventType'
--		,  inserted._AuditEventSequence AS '_AuditEventSequence'
--		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
--		,  inserted._AuditGUID AS '_AuditGUID'
--		INTO @AuditGuidList
--	SELECT 
--		d.[SiteGuid]
--	,	d.[EmailAddresses]
--	,	d.[ErrorFolder]
--	,	d.[CreatedBy]
--	,	d.[CreatedDate]
--	,	d.[UpdatedBy]
--	,	d.[UpdatedDate]
--	,	@_AuditEventType
--	,	@_AuditEventSequence
--	,	@_AuditSessionGUID
--	,	@_AuditSessionTokenID
--	,	@_AuditDateTime
--	,	@_AuditSiteGUID
--	,	NEWID()
--	,	@_UserId
--	,	@_AuditContext
--	FROM deleted d
-- 
--	INSERT INTO [fmaudit].tblErrorNotificationConfigurations (
--		[SiteGuid]
--	,	[EmailAddresses]
--	,	[ErrorFolder]
--	,	[CreatedBy]
--	,	[CreatedDate]
--	,	[UpdatedBy]
--	,	[UpdatedDate]
--	,	[_AuditEventType]
--	,	[_AuditEventSequence]
--	,	[_AuditSessionGUID]
--	,	[_AuditSessionTokenID]
--	,	[_AuditCreatedDate]
--	,	[_AuditSiteGUID]
--	,	[_AuditGUID]
--	,	[_AuditUserId]
--	,	[_AuditContext]
--	)
--	SELECT 
--		i.[SiteGuid]
--	,	i.[EmailAddresses]
--	,	i.[ErrorFolder]
--	,	i.[CreatedBy]
--	,	i.[CreatedDate]
--	,	i.[UpdatedBy]
--	,	i.[UpdatedDate]
--	,	@_AuditEventType
--	,	2
--	,	@_AuditSessionGUID
--	,	@_AuditSessionTokenID
--	,	@_AuditDateTime
--	,	@_AuditSiteGUID
--	,	agl._AuditGUID
--	,	@_UserId
--	,	@_AuditContext
--	FROM inserted i 
--	INNER JOIN	@AuditGuidList agl ON
--		(
--		)
--		WHERE	agl._AuditEventType='U'
--		AND		agl._AuditEventSequence=1 
--		AND		agl._AuditCreatedDate= @_AuditDatetime
--END



GO
