CREATE TABLE [dbo].[tblExportRequest] (
    [ExportRequestGuid]   UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExportRequest_GUID] DEFAULT (newid()) NOT NULL,
    [RequestID]           NVARCHAR (200)     NOT NULL,
    [InterfaceID]         NVARCHAR (200)     NOT NULL,
    [OwnerCode]           NVARCHAR (10)      NULL,
    [UploadStagingFolder] NVARCHAR (200)     NOT NULL,
    [ArchiveFolder]       NVARCHAR (200)     NOT NULL,
    [ConnectionInfo]      NVARCHAR (MAX)     NULL,
    [SendingCompanyCode]  NVARCHAR (50)      NOT NULL,
    [SendViaFTP]          BIT                CONSTRAINT [DF_tblExportRequest_SendViaFTP] DEFAULT ((0)) NOT NULL,
    [SendSecure]          BIT                CONSTRAINT [DF_tblExportRequest_SendSecure] DEFAULT ((0)) NOT NULL,
    [CompanyNames]        NVARCHAR (MAX)     NULL,
    [LatestRowVersion]    BIGINT             NOT NULL,
    [LastExportTime]      DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportRequest_LastExportTime] DEFAULT ('1/1/1900') NOT NULL,
    [ExportFrequency]     INT                NOT NULL,
    [BaselineDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportRequest_BaselineDate] DEFAULT ('1/1/1900') NOT NULL,
    [ExcludeEmptyFiles]   BIT                NULL,
    [UseTimeOfDay]        BIT                CONSTRAINT [DF_tblExportRequests_UseTimeOfDay] DEFAULT ((0)) NOT NULL,
    [NextExportTime]      DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportRequests_NextExportTime] DEFAULT ('1/1/1900') NOT NULL,
    [CreatedDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportRequest_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]           [dbo].[udtUserID]  CONSTRAINT [DF_tblExportRequest_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportRequest_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]           [dbo].[udtUserID]  CONSTRAINT [DF_tblExportRequest_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]         ROWVERSION         NOT NULL,
    [_ClusterIdx]         BIGINT             IDENTITY (1, 1) NOT NULL,
	[SendMethod]          INT                CONSTRAINT [DF_tblExportRequest_SendMethod]  DEFAULT ((0)) NOT NULL,
	[WebServicePluginType] NVARCHAR(100)     NULL,
	[WebServiceConfiguration] NVARCHAR(512)  NULL,
    CONSTRAINT [PK_tblExportRequest_GUID] PRIMARY KEY NONCLUSTERED ([ExportRequestGuid] ASC)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblExportRequest_CreatedDate]
    ON [dbo].[tblExportRequest]([CreatedDate] ASC);




GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_tblExportRequest_RequestID]
    ON [dbo].[tblExportRequest]([RequestID] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblExportRequest] ON [dbo].[tblExportRequest] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExportRequest','D')=1 
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
	INSERT INTO [fmaudit].tblExportRequest (
		[ExportRequestGuid]
	,	[RequestID]
	,	[InterfaceID]
	,	[OwnerCode]
	,	[UploadStagingFolder]
	,	[ArchiveFolder]
	,	[ConnectionInfo]
	,	[SendingCompanyCode]
	,	[SendViaFTP]
	,	[SendSecure]
	,	[CompanyNames]
	,	[LatestRowVersion]
	,	[LastExportTime]
	,	[ExportFrequency]
	,	[BaselineDate]
	,	[ExcludeEmptyFiles]
	,	[UseTimeOfDay]
	,	[NextExportTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[SendMethod]
	,	[WebServicePluginType]
	,	[WebServiceConfiguration]
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
		d.[ExportRequestGuid]
	,	d.[RequestID]
	,	d.[InterfaceID]
	,	d.[OwnerCode]
	,	d.[UploadStagingFolder]
	,	d.[ArchiveFolder]
	,	d.[ConnectionInfo]
	,	d.[SendingCompanyCode]
	,	d.[SendViaFTP]
	,	d.[SendSecure]
	,	d.[CompanyNames]
	,	d.[LatestRowVersion]
	,	d.[LastExportTime]
	,	d.[ExportFrequency]
	,	d.[BaselineDate]
	,	d.[ExcludeEmptyFiles]
	,	d.[UseTimeOfDay]
	,	d.[NextExportTime]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[SendMethod]
	,	d.[WebServicePluginType]
	,	d.[WebServiceConfiguration]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblExportRequest] ON [dbo].[tblExportRequest] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExportRequest','D')=1 
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
	ExportRequestGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblExportRequest (
		[ExportRequestGuid]
	,	[RequestID]
	,	[InterfaceID]
	,	[OwnerCode]
	,	[UploadStagingFolder]
	,	[ArchiveFolder]
	,	[ConnectionInfo]
	,	[SendingCompanyCode]
	,	[SendViaFTP]
	,	[SendSecure]
	,	[CompanyNames]
	,	[LatestRowVersion]
	,	[LastExportTime]
	,	[ExportFrequency]
	,	[BaselineDate]
	,	[ExcludeEmptyFiles]
	,	[UseTimeOfDay]
	,	[NextExportTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[SendMethod]
	,	[WebServicePluginType]
	,	[WebServiceConfiguration]
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
	OUTPUT inserted.[ExportRequestGuid] AS 'ExportRequestGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ExportRequestGuid]
	,	d.[RequestID]
	,	d.[InterfaceID]
	,	d.[OwnerCode]
	,	d.[UploadStagingFolder]
	,	d.[ArchiveFolder]
	,	d.[ConnectionInfo]
	,	d.[SendingCompanyCode]
	,	d.[SendViaFTP]
	,	d.[SendSecure]
	,	d.[CompanyNames]
	,	d.[LatestRowVersion]
	,	d.[LastExportTime]
	,	d.[ExportFrequency]
	,	d.[BaselineDate]
	,	d.[ExcludeEmptyFiles]
	,	d.[UseTimeOfDay]
	,	d.[NextExportTime]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[SendMethod]
	,	d.[WebServicePluginType]
	,	d.[WebServiceConfiguration]
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
 
	INSERT INTO [fmaudit].tblExportRequest (
		[ExportRequestGuid]
	,	[RequestID]
	,	[InterfaceID]
	,	[OwnerCode]
	,	[UploadStagingFolder]
	,	[ArchiveFolder]
	,	[ConnectionInfo]
	,	[SendingCompanyCode]
	,	[SendViaFTP]
	,	[SendSecure]
	,	[CompanyNames]
	,	[LatestRowVersion]
	,	[LastExportTime]
	,	[ExportFrequency]
	,	[BaselineDate]
	,	[ExcludeEmptyFiles]
	,	[UseTimeOfDay]
	,	[NextExportTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[SendMethod]
	,	[WebServicePluginType]
	,	[WebServiceConfiguration]
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
		i.[ExportRequestGuid]
	,	i.[RequestID]
	,	i.[InterfaceID]
	,	i.[OwnerCode]
	,	i.[UploadStagingFolder]
	,	i.[ArchiveFolder]
	,	i.[ConnectionInfo]
	,	i.[SendingCompanyCode]
	,	i.[SendViaFTP]
	,	i.[SendSecure]
	,	i.[CompanyNames]
	,	i.[LatestRowVersion]
	,	i.[LastExportTime]
	,	i.[ExportFrequency]
	,	i.[BaselineDate]
	,	i.[ExcludeEmptyFiles]
	,	i.[UseTimeOfDay]
	,	i.[NextExportTime]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[SendMethod]
	,	i.[WebServicePluginType]
	,	i.[WebServiceConfiguration]
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
			agl.[ExportRequestGuid]=i.[ExportRequestGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblExportRequest] ON [dbo].[tblExportRequest] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExportRequest','D')=1 
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
	INSERT INTO [fmaudit].tblExportRequest (
		[ExportRequestGuid]
	,	[RequestID]
	,	[InterfaceID]
	,	[OwnerCode]
	,	[UploadStagingFolder]
	,	[ArchiveFolder]
	,	[ConnectionInfo]
	,	[SendingCompanyCode]
	,	[SendViaFTP]
	,	[SendSecure]
	,	[CompanyNames]
	,	[LatestRowVersion]
	,	[LastExportTime]
	,	[ExportFrequency]
	,	[BaselineDate]
	,	[ExcludeEmptyFiles]
	,	[UseTimeOfDay]
	,	[NextExportTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[SendMethod]
	,	[WebServicePluginType]
	,	[WebServiceConfiguration]
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
		i.[ExportRequestGuid]
	,	i.[RequestID]
	,	i.[InterfaceID]
	,	i.[OwnerCode]
	,	i.[UploadStagingFolder]
	,	i.[ArchiveFolder]
	,	i.[ConnectionInfo]
	,	i.[SendingCompanyCode]
	,	i.[SendViaFTP]
	,	i.[SendSecure]
	,	i.[CompanyNames]
	,	i.[LatestRowVersion]
	,	i.[LastExportTime]
	,	i.[ExportFrequency]
	,	i.[BaselineDate]
	,	i.[ExcludeEmptyFiles]
	,	i.[UseTimeOfDay]
	,	i.[NextExportTime]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[SendMethod]
	,	i.[WebServicePluginType]
	,	i.[WebServiceConfiguration]
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
CREATE UNIQUE CLUSTERED INDEX [IX_tblExportRequest_ClusterIdx]
    ON [dbo].[tblExportRequest]([_ClusterIdx] ASC);
