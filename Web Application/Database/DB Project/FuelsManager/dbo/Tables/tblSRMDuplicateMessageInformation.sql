CREATE TABLE [dbo].[tblSRMDuplicateMessageInformation] (
    [SRMDuplicateMessageInformationGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblSRMDuplicateMessageInformation_SRMDuplicateMessageInformationGuid] DEFAULT (newid()) NOT NULL,
    [MessageSequenceNumber]              NVARCHAR (100)     NOT NULL,
    [FlightNumber]                       NVARCHAR (10)      NOT NULL,
    [FlightOriginationDate]              DATETIMEOFFSET (7) NOT NULL,
    [OriginIATACode]                     NVARCHAR (10)      NOT NULL,
    [DestinationIATACode]                NVARCHAR (10)      NOT NULL,
    [AirlineIATACode]                    NVARCHAR (10)      NOT NULL,
    [TimesLegFlown]                      NVARCHAR (10)      NOT NULL,
    [HashValue]                          NVARCHAR (32)      NOT NULL,
    [CreatedDate]                        DATETIMEOFFSET (7) CONSTRAINT [DF_tblSRMDuplicateMessageInformation_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                          [dbo].[udtUserID]  CONSTRAINT [DF_tblSRMDuplicateMessageInformation_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                        DATETIMEOFFSET (7) CONSTRAINT [DF_tblSRMDuplicateMessageInformation_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                          [dbo].[udtUserID]  CONSTRAINT [DF_tblSRMDuplicateMessageInformation_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]                        ROWVERSION         NOT NULL,
    [_ClusterIdx]                        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblSRMDuplicateMessageInformation] PRIMARY KEY NONCLUSTERED ([SRMDuplicateMessageInformationGuid] ASC)
);


GO
CREATE NONCLUSTERED INDEX [IX_tblSRMDuplicateMessageInformation_CreatedDate]
    ON [dbo].[tblSRMDuplicateMessageInformation]([CreatedDate] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_tblSRMDuplicateMessageInformation_FlightKeyFields]
    ON [dbo].[tblSRMDuplicateMessageInformation]([CreatedDate] ASC, [FlightNumber] ASC, [FlightOriginationDate] ASC, [OriginIATACode] ASC, [DestinationIATACode] ASC, [AirlineIATACode] ASC, [TimesLegFlown] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblSRMDuplicateMessageInformation] ON [dbo].[tblSRMDuplicateMessageInformation] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSRMDuplicateMessageInformation','D')=1 
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
	INSERT INTO [fmaudit].tblSRMDuplicateMessageInformation (
		[SRMDuplicateMessageInformationGuid]
	,	[MessageSequenceNumber]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
	,	[HashValue]
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
		i.[SRMDuplicateMessageInformationGuid]
	,	i.[MessageSequenceNumber]
	,	i.[FlightNumber]
	,	i.[FlightOriginationDate]
	,	i.[OriginIATACode]
	,	i.[DestinationIATACode]
	,	i.[AirlineIATACode]
	,	i.[TimesLegFlown]
	,	i.[HashValue]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblSRMDuplicateMessageInformation] ON [dbo].[tblSRMDuplicateMessageInformation] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSRMDuplicateMessageInformation','D')=1 
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
	SRMDuplicateMessageInformationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblSRMDuplicateMessageInformation (
		[SRMDuplicateMessageInformationGuid]
	,	[MessageSequenceNumber]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
	,	[HashValue]
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
	OUTPUT inserted.[SRMDuplicateMessageInformationGuid] AS 'SRMDuplicateMessageInformationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SRMDuplicateMessageInformationGuid]
	,	d.[MessageSequenceNumber]
	,	d.[FlightNumber]
	,	d.[FlightOriginationDate]
	,	d.[OriginIATACode]
	,	d.[DestinationIATACode]
	,	d.[AirlineIATACode]
	,	d.[TimesLegFlown]
	,	d.[HashValue]
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
 
	INSERT INTO [fmaudit].tblSRMDuplicateMessageInformation (
		[SRMDuplicateMessageInformationGuid]
	,	[MessageSequenceNumber]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
	,	[HashValue]
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
		i.[SRMDuplicateMessageInformationGuid]
	,	i.[MessageSequenceNumber]
	,	i.[FlightNumber]
	,	i.[FlightOriginationDate]
	,	i.[OriginIATACode]
	,	i.[DestinationIATACode]
	,	i.[AirlineIATACode]
	,	i.[TimesLegFlown]
	,	i.[HashValue]
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
	INNER JOIN	@AuditGuidList agl ON
		(
			agl.[SRMDuplicateMessageInformationGuid]=i.[SRMDuplicateMessageInformationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblSRMDuplicateMessageInformation] ON [dbo].[tblSRMDuplicateMessageInformation] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSRMDuplicateMessageInformation','D')=1 
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
	INSERT INTO [fmaudit].tblSRMDuplicateMessageInformation (
		[SRMDuplicateMessageInformationGuid]
	,	[MessageSequenceNumber]
	,	[FlightNumber]
	,	[FlightOriginationDate]
	,	[OriginIATACode]
	,	[DestinationIATACode]
	,	[AirlineIATACode]
	,	[TimesLegFlown]
	,	[HashValue]
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
		d.[SRMDuplicateMessageInformationGuid]
	,	d.[MessageSequenceNumber]
	,	d.[FlightNumber]
	,	d.[FlightOriginationDate]
	,	d.[OriginIATACode]
	,	d.[DestinationIATACode]
	,	d.[AirlineIATACode]
	,	d.[TimesLegFlown]
	,	d.[HashValue]
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
CREATE UNIQUE CLUSTERED INDEX [IX_tblSRMDuplicateMessageInformation_ClusterIdx]
    ON [dbo].[tblSRMDuplicateMessageInformation]([_ClusterIdx] ASC);

