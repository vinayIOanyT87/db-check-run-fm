CREATE TABLE [dbo].[tblSRMMessage] (
    [SRMMessageGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSRMMessage_SRMMessageGuid] DEFAULT (newid()) NOT NULL,
    [SRMAdaptorGuid]           UNIQUEIDENTIFIER   NOT NULL,
    [ReceiptDateTime]          DATETIMEOFFSET (7) NOT NULL,
    [ExternalSourceIdentifier] NVARCHAR (100)     NOT NULL,
    [FlightNumber]             NVARCHAR (10)      NULL,
    [FlightOriginationDate]    DATETIMEOFFSET (7) NULL,
    [OriginIATACode]           NVARCHAR (10)      NULL,
    [DestinationIATACode]      NVARCHAR (10)      NULL,
    [MessageText]              NVARCHAR (MAX)     NOT NULL,
    [ConvertedMessageXML]      NVARCHAR (MAX)     NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblSRMMessage_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblSRMMessage_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblSRMMessage_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblSRMMessage_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [AirlineIATACode]          NVARCHAR (10)      NULL,
    [TimesLegFlown]            NVARCHAR (10)      NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSRMMessage] PRIMARY KEY NONCLUSTERED ([SRMMessageGuid] ASC),
    CONSTRAINT [FK_tblSRMMessage_SRMAdaptorGuid] FOREIGN KEY ([SRMAdaptorGuid]) REFERENCES [dbo].[tblSRMAdaptor] ([SRMAdaptorGuid])
);

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblSRMMessage_ClusterIdx]
    ON [dbo].[tblSRMMessage]([_ClusterIdx] ASC)

GO

CREATE NONCLUSTERED INDEX [IX_tblSRMMessage_FlightOriginationDate]
    ON [dbo].[tblSRMMessage]([FlightOriginationDate] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblSRMMessage] ON [dbo].[tblSRMMessage] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSRMMessage','D')=1 
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
	INSERT INTO [fmaudit].tblSRMMessage (
		[SRMMessageGuid]
	,	[SRMAdaptorGuid]
	,	[ReceiptDateTime]
	,	[ExternalSourceIdentifier]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[MessageText]
	,	[ConvertedMessageXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
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
		i.[SRMMessageGuid]
	,	i.[SRMAdaptorGuid]
	,	i.[ReceiptDateTime]
	,	i.[ExternalSourceIdentifier]
	,	i.[FlightNumber]
	,	i.[FlightOriginationDate]
	,	i.[OriginIATACode]
	,	i.[DestinationIATACode]
	,	i.[MessageText]
	,	i.[ConvertedMessageXML]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[AirlineIATACode]
	,	i.[TimesLegFlown]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblSRMMessage] ON [dbo].[tblSRMMessage] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSRMMessage','D')=1 
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
	SRMMessageGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblSRMMessage (
		[SRMMessageGuid]
	,	[SRMAdaptorGuid]
	,	[ReceiptDateTime]
	,	[ExternalSourceIdentifier]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[MessageText]
	,	[ConvertedMessageXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
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
	OUTPUT inserted.[SRMMessageGuid] AS 'SRMMessageGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SRMMessageGuid]
	,	d.[SRMAdaptorGuid]
	,	d.[ReceiptDateTime]
	,	d.[ExternalSourceIdentifier]
	,	d.[FlightNumber]
	,	d.[FlightOriginationDate]
	,	d.[OriginIATACode]
	,	d.[DestinationIATACode]
	,	d.[MessageText]
	,	d.[ConvertedMessageXML]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[AirlineIATACode]
	,	d.[TimesLegFlown]
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
 
	INSERT INTO [fmaudit].tblSRMMessage (
		[SRMMessageGuid]
	,	[SRMAdaptorGuid]
	,	[ReceiptDateTime]
	,	[ExternalSourceIdentifier]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[MessageText]
	,	[ConvertedMessageXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
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
		i.[SRMMessageGuid]
	,	i.[SRMAdaptorGuid]
	,	i.[ReceiptDateTime]
	,	i.[ExternalSourceIdentifier]
	,	i.[FlightNumber]
	,	i.[FlightOriginationDate]
	,	i.[OriginIATACode]
	,	i.[DestinationIATACode]
	,	i.[MessageText]
	,	i.[ConvertedMessageXML]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[AirlineIATACode]
	,	i.[TimesLegFlown]
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
			agl.[SRMMessageGuid]=i.[SRMMessageGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblSRMMessage] ON [dbo].[tblSRMMessage] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSRMMessage','D')=1 
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
	INSERT INTO [fmaudit].tblSRMMessage (
		[SRMMessageGuid]
	,	[SRMAdaptorGuid]
	,	[ReceiptDateTime]
	,	[ExternalSourceIdentifier]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[MessageText]
	,	[ConvertedMessageXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
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
		d.[SRMMessageGuid]
	,	d.[SRMAdaptorGuid]
	,	d.[ReceiptDateTime]
	,	d.[ExternalSourceIdentifier]
	,	d.[FlightNumber]
	,	d.[FlightOriginationDate]
	,	d.[OriginIATACode]
	,	d.[DestinationIATACode]
	,	d.[MessageText]
	,	d.[ConvertedMessageXML]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[AirlineIATACode]
	,	d.[TimesLegFlown]
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