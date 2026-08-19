CREATE TABLE [dbo].[tblArchivedUsers] (
    [UserID]                [dbo].[udtUserID]  NOT NULL,
    [Password]              VARBINARY (256)    NOT NULL,
    [LastLoginDate]         DATETIMEOFFSET (7) NOT NULL,
    [LastLogoffDate]        DATETIMEOFFSET (7) NOT NULL,
    [ChangePassword]        BIT                NOT NULL,
    [PasswordTimeStamp]     DATETIMEOFFSET (7) NOT NULL,
    [Name]                  NVARCHAR (50)      NOT NULL,
    [EmailAddress]          NVARCHAR (50)      NULL,
    [CreatedDate]           DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  NOT NULL,
    [PasswordHistory1]      VARBINARY (256)    NULL,
    [PasswordHistory2]      VARBINARY (256)    NULL,
    [PasswordHistory3]      VARBINARY (256)    NULL,
    [PasswordHistory4]      VARBINARY (256)    NULL,
    [PasswordHistory5]      VARBINARY (256)    NULL,
    [PasswordHistory6]      VARBINARY (256)    NULL,
    [PasswordHistory7]      VARBINARY (256)    NULL,
    [PasswordHistory8]      VARBINARY (256)    NULL,
    [PasswordHistory9]      VARBINARY (256)    NULL,
    [PasswordHistory10]     VARBINARY (256)    NULL,
    [PasswordHistory11]     VARBINARY (256)    NULL,
    [PasswordHistory12]     VARBINARY (256)    NULL,
    [PasswordHistory13]     VARBINARY (256)    NULL,
    [PasswordHistory14]     VARBINARY (256)    NULL,
    [PasswordHistory15]     VARBINARY (256)    NULL,
    [PasswordHistory16]     VARBINARY (256)    NULL,
    [PasswordHistory17]     VARBINARY (256)    NULL,
    [PasswordHistory18]     VARBINARY (256)    NULL,
    [PasswordHistory19]     VARBINARY (256)    NULL,
    [PasswordHistory20]     VARBINARY (256)    NULL,
    [PasswordHistory21]     VARBINARY (256)    NULL,
    [PasswordHistory22]     VARBINARY (256)    NULL,
    [PasswordHistory23]     VARBINARY (256)    NULL,
    [PasswordHistory24]     VARBINARY (256)    NULL,
    [PasswordLockoutCount]  INT                NULL,
    [InactivityLockout]     INT                NULL,
    [InactivityLockoutDate] DATETIMEOFFSET (7) NULL,
    [ArchivedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblArchivedUsers_ArchivedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [ArchivedUserGuid]      UNIQUEIDENTIFIER   CONSTRAINT [DF_tblArchivedUsers_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [SiteGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [UserGuid]              UNIQUEIDENTIFIER   NULL,
    [PasswordHint]          VARCHAR (80)       NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
	[UserData1]				NVARCHAR (120)      NULL,
	[UserData2]				NVARCHAR (120)      NULL,
	[UserData3]				NVARCHAR (120)      NULL,
	[UserData4]				NVARCHAR (120)      NULL,
	[UserData5]				NVARCHAR (120)      NULL,
	[UserData6]				NVARCHAR (120)      NULL,
	[UserData7]				NVARCHAR (120)      NULL,
	[UserData8]				NVARCHAR (120)      NULL,
    [PhoneNumber]           NVARCHAR (20)		NULL,
    [AccountExpirationDate] DATETIME			NULL
    CONSTRAINT [PK_tblArchivedUsers_GUID] PRIMARY KEY NONCLUSTERED ([ArchivedUserGuid] ASC),
    CONSTRAINT [FK_tblArchivedUsers_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblArchivedUsers_CreatedDate]
    ON [dbo].[tblArchivedUsers]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblArchivedUsers] ON [dbo].[tblArchivedUsers] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblArchivedUsers','D')=1 
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
	INSERT INTO [fmaudit].tblArchivedUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[ArchivedDate]
	,	[ArchivedUserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[UserGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
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
		d.[UserID]
	,	d.[Password]
	,	d.[LastLoginDate]
	,	d.[LastLogoffDate]
	,	d.[ChangePassword]
	,	d.[PasswordTimeStamp]
	,	d.[Name]
	,	d.[EmailAddress]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[PasswordHistory1]
	,	d.[PasswordHistory2]
	,	d.[PasswordHistory3]
	,	d.[PasswordHistory4]
	,	d.[PasswordHistory5]
	,	d.[PasswordHistory6]
	,	d.[PasswordHistory7]
	,	d.[PasswordHistory8]
	,	d.[PasswordHistory9]
	,	d.[PasswordHistory10]
	,	d.[PasswordHistory11]
	,	d.[PasswordHistory12]
	,	d.[PasswordHistory13]
	,	d.[PasswordHistory14]
	,	d.[PasswordHistory15]
	,	d.[PasswordHistory16]
	,	d.[PasswordHistory17]
	,	d.[PasswordHistory18]
	,	d.[PasswordHistory19]
	,	d.[PasswordHistory20]
	,	d.[PasswordHistory21]
	,	d.[PasswordHistory22]
	,	d.[PasswordHistory23]
	,	d.[PasswordHistory24]
	,	d.[PasswordLockoutCount]
	,	d.[InactivityLockout]
	,	d.[InactivityLockoutDate]
	,	d.[ArchivedDate]
	,	d.[ArchivedUserGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[UserGuid]
	,	d.[PasswordHint]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[PhoneNumber]
	,	d.[AccountExpirationDate]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblArchivedUsers] ON [dbo].[tblArchivedUsers] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblArchivedUsers','D')=1 
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
	INSERT INTO [fmaudit].tblArchivedUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[ArchivedDate]
	,	[ArchivedUserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[UserGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
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
		i.[UserID]
	,	i.[Password]
	,	i.[LastLoginDate]
	,	i.[LastLogoffDate]
	,	i.[ChangePassword]
	,	i.[PasswordTimeStamp]
	,	i.[Name]
	,	i.[EmailAddress]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[PasswordHistory1]
	,	i.[PasswordHistory2]
	,	i.[PasswordHistory3]
	,	i.[PasswordHistory4]
	,	i.[PasswordHistory5]
	,	i.[PasswordHistory6]
	,	i.[PasswordHistory7]
	,	i.[PasswordHistory8]
	,	i.[PasswordHistory9]
	,	i.[PasswordHistory10]
	,	i.[PasswordHistory11]
	,	i.[PasswordHistory12]
	,	i.[PasswordHistory13]
	,	i.[PasswordHistory14]
	,	i.[PasswordHistory15]
	,	i.[PasswordHistory16]
	,	i.[PasswordHistory17]
	,	i.[PasswordHistory18]
	,	i.[PasswordHistory19]
	,	i.[PasswordHistory20]
	,	i.[PasswordHistory21]
	,	i.[PasswordHistory22]
	,	i.[PasswordHistory23]
	,	i.[PasswordHistory24]
	,	i.[PasswordLockoutCount]
	,	i.[InactivityLockout]
	,	i.[InactivityLockoutDate]
	,	i.[ArchivedDate]
	,	i.[ArchivedUserGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[UserGuid]
	,	i.[PasswordHint]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[PhoneNumber]
	,	i.[AccountExpirationDate]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblArchivedUsers] ON [dbo].[tblArchivedUsers] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblArchivedUsers','D')=1 
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
	ArchivedUserGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblArchivedUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[ArchivedDate]
	,	[ArchivedUserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[UserGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
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
	OUTPUT inserted.[ArchivedUserGuid] AS 'ArchivedUserGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[UserID]
	,	d.[Password]
	,	d.[LastLoginDate]
	,	d.[LastLogoffDate]
	,	d.[ChangePassword]
	,	d.[PasswordTimeStamp]
	,	d.[Name]
	,	d.[EmailAddress]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[PasswordHistory1]
	,	d.[PasswordHistory2]
	,	d.[PasswordHistory3]
	,	d.[PasswordHistory4]
	,	d.[PasswordHistory5]
	,	d.[PasswordHistory6]
	,	d.[PasswordHistory7]
	,	d.[PasswordHistory8]
	,	d.[PasswordHistory9]
	,	d.[PasswordHistory10]
	,	d.[PasswordHistory11]
	,	d.[PasswordHistory12]
	,	d.[PasswordHistory13]
	,	d.[PasswordHistory14]
	,	d.[PasswordHistory15]
	,	d.[PasswordHistory16]
	,	d.[PasswordHistory17]
	,	d.[PasswordHistory18]
	,	d.[PasswordHistory19]
	,	d.[PasswordHistory20]
	,	d.[PasswordHistory21]
	,	d.[PasswordHistory22]
	,	d.[PasswordHistory23]
	,	d.[PasswordHistory24]
	,	d.[PasswordLockoutCount]
	,	d.[InactivityLockout]
	,	d.[InactivityLockoutDate]
	,	d.[ArchivedDate]
	,	d.[ArchivedUserGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[UserGuid]
	,	d.[PasswordHint]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[PhoneNumber]
	,	d.[AccountExpirationDate]
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
 
	INSERT INTO [fmaudit].tblArchivedUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[ArchivedDate]
	,	[ArchivedUserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[UserGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
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
		i.[UserID]
	,	i.[Password]
	,	i.[LastLoginDate]
	,	i.[LastLogoffDate]
	,	i.[ChangePassword]
	,	i.[PasswordTimeStamp]
	,	i.[Name]
	,	i.[EmailAddress]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[PasswordHistory1]
	,	i.[PasswordHistory2]
	,	i.[PasswordHistory3]
	,	i.[PasswordHistory4]
	,	i.[PasswordHistory5]
	,	i.[PasswordHistory6]
	,	i.[PasswordHistory7]
	,	i.[PasswordHistory8]
	,	i.[PasswordHistory9]
	,	i.[PasswordHistory10]
	,	i.[PasswordHistory11]
	,	i.[PasswordHistory12]
	,	i.[PasswordHistory13]
	,	i.[PasswordHistory14]
	,	i.[PasswordHistory15]
	,	i.[PasswordHistory16]
	,	i.[PasswordHistory17]
	,	i.[PasswordHistory18]
	,	i.[PasswordHistory19]
	,	i.[PasswordHistory20]
	,	i.[PasswordHistory21]
	,	i.[PasswordHistory22]
	,	i.[PasswordHistory23]
	,	i.[PasswordHistory24]
	,	i.[PasswordLockoutCount]
	,	i.[InactivityLockout]
	,	i.[InactivityLockoutDate]
	,	i.[ArchivedDate]
	,	i.[ArchivedUserGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[UserGuid]
	,	i.[PasswordHint]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[PhoneNumber]
	,	i.[AccountExpirationDate]
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
			agl.[ArchivedUserGuid]=i.[ArchivedUserGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblArchivedUsers
CREATE TRIGGER dbo.trg_insupd_tblArchivedUsers_ForSync 
   ON dbo.tblArchivedUsers
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
                    ,d.ArchivedUserGuid AS Deleted_PK_ArchivedUserGuid
                    ,i.ArchivedUserGuid AS Inserted_PK_ArchivedUserGuid
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
				    d.ArchivedUserGuid = i.ArchivedUserGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblArchivedUsers As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ArchivedUserGuid = currentTrackingData.PK_ArchivedUserGuid
 
 
		    INSERT track.tblArchivedUsers (InsertedDate 
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
				    ,PK_ArchivedUserGuid
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
				    ,entityChanges.Inserted_PK_ArchivedUserGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblArchivedUsers As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ArchivedUserGuid = currentTrackingData.PK_ArchivedUserGuid
)
    END
END 

GO
--Creating Delete Trigger for tblArchivedUsers
CREATE TRIGGER dbo.trg_del_tblArchivedUsers_ForSync 
   ON dbo.tblArchivedUsers
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
						,d.ArchivedUserGuid AS Deleted_PK_ArchivedUserGuid
                        ,d.ArchivedUserGuid AS Inserted_PK_ArchivedUserGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblArchivedUsers As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ArchivedUserGuid = currentTrackingData.PK_ArchivedUserGuid
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
						,PK_ArchivedUserGuid
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
						,entityChanges.Deleted_PK_ArchivedUserGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblArchivedUsers_ClusterIdx]
    ON [dbo].[tblArchivedUsers]([_ClusterIdx] ASC);

