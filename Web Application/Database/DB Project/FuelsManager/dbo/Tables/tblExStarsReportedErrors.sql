CREATE TABLE [dbo].[tblExStarsReportedErrors] (
    [ManagerCompanyGuid]        UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]                  UNIQUEIDENTIFIER   NOT NULL,
    [SequenceNumber]            NVARCHAR (20)      NOT NULL,
    [MustCorrect]               BIT                NOT NULL,
    [PBI01_Primary]             NVARCHAR (10)      NOT NULL,
    [PBI01_Secondary]           NVARCHAR (10)      NOT NULL,
    [PBI03_Primary]             NVARCHAR (10)      NOT NULL,
    [PBI03_Secondary]           NVARCHAR (10)      NOT NULL,
    [PBI04]                     NVARCHAR (10)      NOT NULL,
    [OriginalValue]             NVARCHAR (MAX)     NULL,
    [IRSErrorText]              NVARCHAR (MAX)     NULL,
    [ErrorCorrected]            BIT                CONSTRAINT [DF_tblExStarsReportedError_ErrorCorrected] DEFAULT ((0)) NOT NULL,
    [ExStarsFilingsGuid]        UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsReportedError_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                 [dbo].[udtUserID]  CONSTRAINT [DF_tblExStarsReportedError_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsReportedError_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                 [dbo].[udtUserID]  CONSTRAINT [DF_tblExStarsReportedError_UpdatedBy] DEFAULT ('') NOT NULL,
    [ExStarsReportedErrorsGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExStarsReportedError_GUID] DEFAULT (newid()) NOT NULL,
    [_ClusterIdx]               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblExStarsReportedErrors_ExStarsReportedErrorsGuid] PRIMARY KEY NONCLUSTERED ([ExStarsReportedErrorsGuid] ASC),
    CONSTRAINT [fk_ExStarsReportedErrorsFilingsGuid] FOREIGN KEY ([ExStarsFilingsGuid]) REFERENCES [dbo].[tblExStarsFilings] ([ExStarsFilingsGuid]),
    CONSTRAINT [fk_ExStarsReportedErrorsManager] FOREIGN KEY ([ManagerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [fk_ExStarsReportedErrorsPBI01_Primary] FOREIGN KEY ([PBI01_Primary]) REFERENCES [dbo].[tblExStarsIrsErrorCodes] ([Code]),
    CONSTRAINT [fk_ExStarsReportedErrorsPBI01_Secondary] FOREIGN KEY ([PBI01_Secondary]) REFERENCES [dbo].[tblExStarsIrsErrorCodes] ([Code]),
    CONSTRAINT [fk_ExStarsReportedErrorsPBI03_Primary] FOREIGN KEY ([PBI03_Primary]) REFERENCES [dbo].[tblExStarsIrsErrorCodes] ([Code]),
    CONSTRAINT [fk_ExStarsReportedErrorsPBI03_Secondary] FOREIGN KEY ([PBI03_Secondary]) REFERENCES [dbo].[tblExStarsIrsErrorCodes] ([Code]),
    CONSTRAINT [fk_ExStarsReportedErrorsPBI04] FOREIGN KEY ([PBI04]) REFERENCES [dbo].[tblExStarsIrsErrorCodes] ([Code]),
    CONSTRAINT [fk_ExStarsReportedErrorsSite] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);

GO
CREATE UNIQUE NONCLUSTERED  INDEX IX_tblExStarsReportedErrorsUnique ON  [dbo].[tblExStarsReportedErrors]
(
	[ExStarsFilingsGuid] ASC
	, [SequenceNumber] ASC
	,[PBI01_Primary] ASC
	,[PBI01_Secondary] ASC
	,[PBI03_Primary]  ASC
	,[PBI03_Secondary]  ASC
	,[PBI04]  ASC
)

GO


CREATE NONCLUSTERED  INDEX IX_tblExStarsReportedErrorsGuid ON  [dbo].[tblExStarsReportedErrors]
(
	[ExStarsReportedErrorsGuid] ASC
)



GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsReportedErrors_CreatedDate]
    ON [dbo].[tblExStarsReportedErrors]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExStarsReportedErrors_ClusterIdx]
    ON [dbo].[tblExStarsReportedErrors]([_ClusterIdx] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblExStarsReportedErrors] ON [dbo].[tblExStarsReportedErrors] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsReportedErrors','D')=1 
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
	ExStarsReportedErrorsGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblExStarsReportedErrors (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[SequenceNumber]
	,	[MustCorrect]
	,	[PBI01_Primary]
	,	[PBI01_Secondary]
	,	[PBI03_Primary]
	,	[PBI03_Secondary]
	,	[PBI04]
	,	[OriginalValue]
	,	[IRSErrorText]
	,	[ErrorCorrected]
	,	[ExStarsFilingsGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsReportedErrorsGuid]
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
	OUTPUT inserted.[ExStarsReportedErrorsGuid] AS 'ExStarsReportedErrorsGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ManagerCompanyGuid]
	,	d.[SiteGuid]
	,	d.[SequenceNumber]
	,	d.[MustCorrect]
	,	d.[PBI01_Primary]
	,	d.[PBI01_Secondary]
	,	d.[PBI03_Primary]
	,	d.[PBI03_Secondary]
	,	d.[PBI04]
	,	d.[OriginalValue]
	,	d.[IRSErrorText]
	,	d.[ErrorCorrected]
	,	d.[ExStarsFilingsGuid]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsReportedErrorsGuid]
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
 
	INSERT INTO [fmaudit].tblExStarsReportedErrors (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[SequenceNumber]
	,	[MustCorrect]
	,	[PBI01_Primary]
	,	[PBI01_Secondary]
	,	[PBI03_Primary]
	,	[PBI03_Secondary]
	,	[PBI04]
	,	[OriginalValue]
	,	[IRSErrorText]
	,	[ErrorCorrected]
	,	[ExStarsFilingsGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsReportedErrorsGuid]
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
		i.[ManagerCompanyGuid]
	,	i.[SiteGuid]
	,	i.[SequenceNumber]
	,	i.[MustCorrect]
	,	i.[PBI01_Primary]
	,	i.[PBI01_Secondary]
	,	i.[PBI03_Primary]
	,	i.[PBI03_Secondary]
	,	i.[PBI04]
	,	i.[OriginalValue]
	,	i.[IRSErrorText]
	,	i.[ErrorCorrected]
	,	i.[ExStarsFilingsGuid]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsReportedErrorsGuid]
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
			agl.[ExStarsReportedErrorsGuid]=i.[ExStarsReportedErrorsGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblExStarsReportedErrors] ON [dbo].[tblExStarsReportedErrors] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsReportedErrors','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsReportedErrors (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[SequenceNumber]
	,	[MustCorrect]
	,	[PBI01_Primary]
	,	[PBI01_Secondary]
	,	[PBI03_Primary]
	,	[PBI03_Secondary]
	,	[PBI04]
	,	[OriginalValue]
	,	[IRSErrorText]
	,	[ErrorCorrected]
	,	[ExStarsFilingsGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsReportedErrorsGuid]
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
		d.[ManagerCompanyGuid]
	,	d.[SiteGuid]
	,	d.[SequenceNumber]
	,	d.[MustCorrect]
	,	d.[PBI01_Primary]
	,	d.[PBI01_Secondary]
	,	d.[PBI03_Primary]
	,	d.[PBI03_Secondary]
	,	d.[PBI04]
	,	d.[OriginalValue]
	,	d.[IRSErrorText]
	,	d.[ErrorCorrected]
	,	d.[ExStarsFilingsGuid]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsReportedErrorsGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblExStarsReportedErrors] ON [dbo].[tblExStarsReportedErrors] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsReportedErrors','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsReportedErrors (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[SequenceNumber]
	,	[MustCorrect]
	,	[PBI01_Primary]
	,	[PBI01_Secondary]
	,	[PBI03_Primary]
	,	[PBI03_Secondary]
	,	[PBI04]
	,	[OriginalValue]
	,	[IRSErrorText]
	,	[ErrorCorrected]
	,	[ExStarsFilingsGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsReportedErrorsGuid]
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
		i.[ManagerCompanyGuid]
	,	i.[SiteGuid]
	,	i.[SequenceNumber]
	,	i.[MustCorrect]
	,	i.[PBI01_Primary]
	,	i.[PBI01_Secondary]
	,	i.[PBI03_Primary]
	,	i.[PBI03_Secondary]
	,	i.[PBI04]
	,	i.[OriginalValue]
	,	i.[IRSErrorText]
	,	i.[ErrorCorrected]
	,	i.[ExStarsFilingsGuid]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsReportedErrorsGuid]
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