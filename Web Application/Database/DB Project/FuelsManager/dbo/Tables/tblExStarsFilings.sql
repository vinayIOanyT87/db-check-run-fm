CREATE TABLE [dbo].[tblExStarsFilings] (
    [FilingStartDate]       DATE               NOT NULL,
    [FilingEndDate]         DATE               NOT NULL,
    [ManagerCompanyGuid]    UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [ReportType]            NVARCHAR (30)      NOT NULL,
    [Modifier]              NVARCHAR (30)      NOT NULL,
    [ControlNumber]         NVARCHAR (9)       NOT NULL,
    [TransSetControlNumber] NVARCHAR (9)       NOT NULL,
    [OriginalControlNumber] NCHAR (9)          NULL,
    [FilingStatus]          NVARCHAR (30)      NOT NULL,
    [FilingCreated]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsFilings_FilingCreated] DEFAULT (GETDATE()) NOT NULL,
    [FilingSent]            DATETIMEOFFSET (7) NULL,
    [ResponseLoaded]        DATETIMEOFFSET (7) NULL,
    [RawDataFileName]       NVARCHAR (MAX)     NULL,
    [EasyReadFileName]      NVARCHAR (MAX)     NULL,
    [EdiReport]             NVARCHAR (MAX)     NULL,
    [EasyReadReport]        NVARCHAR (MAX)     NULL,
    [SerializedData]        NVARCHAR (MAX)     NULL,
    [Acknowledgement]       NVARCHAR (MAX)     NULL,
    [AckEasyRead]           NVARCHAR (MAX)     NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsFilings_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblExStarsFilings_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblExStarsFilings_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblExStarsFilings_UpdatedBy] DEFAULT ('') NOT NULL,
    [ExStarsFilingsGuid]    UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExStarsFilings_GUID] DEFAULT (newid()) NOT NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([ExStarsFilingsGuid] ASC),
    CONSTRAINT [fk_ExStarsFilingsManager] FOREIGN KEY ([ManagerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [fk_ExStarsFilingsSite] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);

GO

/*
 *  There could be multiple replacement, correction and supplemental files for the same company/site/date
*/
CREATE NONCLUSTERED  INDEX IX_tblExStarsFilingsMgrSiteType ON  [dbo].[tblExStarsFilings]
(
	[FilingStartDate] ASC,
	[ManagerCompanyGuid] ASC,
	[SiteGuid] ASC,
	[ReportType] ASC,
	[Modifier] ASC
)


GO
/*
 *  A Transaction Set Control number is only submitted once, all replacement, correction and supplemental submissions
 * reference the original Transaction Set Control  using a REF~FJ segment, but set their own new Transaction Set Control 
  */
CREATE UNIQUE NONCLUSTERED  INDEX IX_tblExStarsFilingsTransactionSetControlNUmber ON  [dbo].[tblExStarsFilings]
(
	[TransSetControlNumber] ASC
)

GO
CREATE UNIQUE NONCLUSTERED  INDEX IX_tblExStarsFilingsGuid ON  [dbo].[tblExStarsFilings]
(
	[ExStarsFilingsGuid] ASC
)

GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsFilings_FilingCreated]
    ON [dbo].[tblExStarsFilings]([FilingCreated] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExStarsFilings_ClusterIdx]
    ON [dbo].[tblExStarsFilings]([_ClusterIdx] ASC);

GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblExStarsFilings] ON [dbo].[tblExStarsFilings] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsFilings','D')=1 
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
	ExStarsFilingsGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblExStarsFilings (
		[FilingStartDate]
	,	[FilingEndDate]
	,	[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ReportType]
	,	[Modifier]
	,	[ControlNumber]
	,	[TransSetControlNumber]
	,	[OriginalControlNumber]
	,	[FilingStatus]
	,	[FilingCreated]
	,	[FilingSent]
	,	[ResponseLoaded]
	,	[RawDataFileName]
	,	[EasyReadFileName]
	,	[EdiReport]
	,	[EasyReadReport]
	,	[SerializedData]
	,	[Acknowledgement]
	,	[AckEasyRead]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsFilingsGuid]
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
	OUTPUT inserted.[ExStarsFilingsGuid] AS 'ExStarsFilingsGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[FilingStartDate]
	,	d.[FilingEndDate]
	,	d.[ManagerCompanyGuid]
	,	d.[SiteGuid]
	,	d.[ReportType]
	,	d.[Modifier]
	,	d.[ControlNumber]
	,	d.[TransSetControlNumber]
	,	d.[OriginalControlNumber]
	,	d.[FilingStatus]
	,	d.[FilingCreated]
	,	d.[FilingSent]
	,	d.[ResponseLoaded]
	,	d.[RawDataFileName]
	,	d.[EasyReadFileName]
	,	d.[EdiReport]
	,	d.[EasyReadReport]
	,	d.[SerializedData]
	,	d.[Acknowledgement]
	,	d.[AckEasyRead]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsFilingsGuid]
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
 
	INSERT INTO [fmaudit].tblExStarsFilings (
		[FilingStartDate]
	,	[FilingEndDate]
	,	[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ReportType]
	,	[Modifier]
	,	[ControlNumber]
	,	[TransSetControlNumber]
	,	[OriginalControlNumber]
	,	[FilingStatus]
	,	[FilingCreated]
	,	[FilingSent]
	,	[ResponseLoaded]
	,	[RawDataFileName]
	,	[EasyReadFileName]
	,	[EdiReport]
	,	[EasyReadReport]
	,	[SerializedData]
	,	[Acknowledgement]
	,	[AckEasyRead]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsFilingsGuid]
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
		i.[FilingStartDate]
	,	i.[FilingEndDate]
	,	i.[ManagerCompanyGuid]
	,	i.[SiteGuid]
	,	i.[ReportType]
	,	i.[Modifier]
	,	i.[ControlNumber]
	,	i.[TransSetControlNumber]
	,	i.[OriginalControlNumber]
	,	i.[FilingStatus]
	,	i.[FilingCreated]
	,	i.[FilingSent]
	,	i.[ResponseLoaded]
	,	i.[RawDataFileName]
	,	i.[EasyReadFileName]
	,	i.[EdiReport]
	,	i.[EasyReadReport]
	,	i.[SerializedData]
	,	i.[Acknowledgement]
	,	i.[AckEasyRead]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsFilingsGuid]
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
			agl.[ExStarsFilingsGuid]=i.[ExStarsFilingsGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblExStarsFilings] ON [dbo].[tblExStarsFilings] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsFilings','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsFilings (
		[FilingStartDate]
	,	[FilingEndDate]
	,	[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ReportType]
	,	[Modifier]
	,	[ControlNumber]
	,	[TransSetControlNumber]
	,	[OriginalControlNumber]
	,	[FilingStatus]
	,	[FilingCreated]
	,	[FilingSent]
	,	[ResponseLoaded]
	,	[RawDataFileName]
	,	[EasyReadFileName]
	,	[EdiReport]
	,	[EasyReadReport]
	,	[SerializedData]
	,	[Acknowledgement]
	,	[AckEasyRead]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsFilingsGuid]
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
		d.[FilingStartDate]
	,	d.[FilingEndDate]
	,	d.[ManagerCompanyGuid]
	,	d.[SiteGuid]
	,	d.[ReportType]
	,	d.[Modifier]
	,	d.[ControlNumber]
	,	d.[TransSetControlNumber]
	,	d.[OriginalControlNumber]
	,	d.[FilingStatus]
	,	d.[FilingCreated]
	,	d.[FilingSent]
	,	d.[ResponseLoaded]
	,	d.[RawDataFileName]
	,	d.[EasyReadFileName]
	,	d.[EdiReport]
	,	d.[EasyReadReport]
	,	d.[SerializedData]
	,	d.[Acknowledgement]
	,	d.[AckEasyRead]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsFilingsGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblExStarsFilings] ON [dbo].[tblExStarsFilings] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsFilings','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsFilings (
		[FilingStartDate]
	,	[FilingEndDate]
	,	[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ReportType]
	,	[Modifier]
	,	[ControlNumber]
	,	[TransSetControlNumber]
	,	[OriginalControlNumber]
	,	[FilingStatus]
	,	[FilingCreated]
	,	[FilingSent]
	,	[ResponseLoaded]
	,	[RawDataFileName]
	,	[EasyReadFileName]
	,	[EdiReport]
	,	[EasyReadReport]
	,	[SerializedData]
	,	[Acknowledgement]
	,	[AckEasyRead]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsFilingsGuid]
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
		i.[FilingStartDate]
	,	i.[FilingEndDate]
	,	i.[ManagerCompanyGuid]
	,	i.[SiteGuid]
	,	i.[ReportType]
	,	i.[Modifier]
	,	i.[ControlNumber]
	,	i.[TransSetControlNumber]
	,	i.[OriginalControlNumber]
	,	i.[FilingStatus]
	,	i.[FilingCreated]
	,	i.[FilingSent]
	,	i.[ResponseLoaded]
	,	i.[RawDataFileName]
	,	i.[EasyReadFileName]
	,	i.[EdiReport]
	,	i.[EasyReadReport]
	,	i.[SerializedData]
	,	i.[Acknowledgement]
	,	i.[AckEasyRead]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsFilingsGuid]
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