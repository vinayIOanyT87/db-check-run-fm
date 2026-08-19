CREATE TABLE [dbo].[tblReportDetails] (
    [ReportName]           NVARCHAR (60)      CONSTRAINT [DF_tblReportDetails_ReportName] DEFAULT ('') NOT NULL,
    [ReportDescription]    NVARCHAR (255)     CONSTRAINT [DF_tblReportDetails_ReportDescription] DEFAULT ('') NOT NULL,
    [ReportPath]           NVARCHAR (200)     CONSTRAINT [DF_tblReportDetails_ReportPath] DEFAULT ('') NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  NULL,
    [CreatedDate]          DATETIMEOFFSET (7) NULL,
    [UpdatedBy]            [dbo].[udtUserID]  NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) NULL,
    [OrderNumber]          INT                NULL,
    [PrintOnlyFlag]        BIT                NULL,
    [PrimaryPrinterName]   NVARCHAR (100)     NULL,
    [SecondaryPrinterName] NVARCHAR (100)     NULL,
    [PrintAtEndOfDay]      BIT                NULL,
    [PrintAtEndOfMonth]    BIT                NULL,
	[DWReportFlag]    	   BIT                NULL,
    [ReportDetailGuid]     UNIQUEIDENTIFIER   CONSTRAINT [DF_tblReportDetails_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [SiteGuid]             UNIQUEIDENTIFIER   NOT NULL,
    [ReportGroupGuid]      UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblReportDetails_GUID] PRIMARY KEY NONCLUSTERED ([ReportDetailGuid] ASC),
    CONSTRAINT [FK_tblReportDetails_ReportGroupGuid] FOREIGN KEY ([ReportGroupGuid]) REFERENCES [dbo].[tblReportGroups] ([ReportGroupGuid]),
    CONSTRAINT [FK_tblReportDetails_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblReportDetails_CreatedDate]
    ON [dbo].[tblReportDetails]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblReportDetails_SiteGuid_ReportName]
    ON [dbo].[tblReportDetails]([SiteGuid] ASC, [ReportName] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblReportDetails] ON [dbo].[tblReportDetails] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblReportDetails','D')=1 
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
	INSERT INTO [fmaudit].tblReportDetails (
		[ReportName]
	,	[ReportDescription]
	,	[ReportPath]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OrderNumber]
	,	[PrintOnlyFlag]
	,	[PrimaryPrinterName]
	,	[SecondaryPrinterName]
	,	[PrintAtEndOfDay]
	,	[PrintAtEndOfMonth]
	,   [DWReportFlag] 
	,	[ReportDetailGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ReportGroupGuid]
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
		i.[ReportName]
	,	i.[ReportDescription]
	,	i.[ReportPath]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[OrderNumber]
	,	i.[PrintOnlyFlag]
	,	i.[PrimaryPrinterName]
	,	i.[SecondaryPrinterName]
	,	i.[PrintAtEndOfDay]
	,	i.[PrintAtEndOfMonth]
	,   i.[DWReportFlag] 
	,	i.[ReportDetailGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ReportGroupGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblReportDetails] ON [dbo].[tblReportDetails] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblReportDetails','D')=1 
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
	ReportDetailGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblReportDetails (
		[ReportName]
	,	[ReportDescription]
	,	[ReportPath]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OrderNumber]
	,	[PrintOnlyFlag]
	,	[PrimaryPrinterName]
	,	[SecondaryPrinterName]
	,	[PrintAtEndOfDay]
	,	[PrintAtEndOfMonth]
	, 	[DWReportFlag] 
	,	[ReportDetailGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ReportGroupGuid]
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
	OUTPUT inserted.[ReportDetailGuid] AS 'ReportDetailGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ReportName]
	,	d.[ReportDescription]
	,	d.[ReportPath]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[OrderNumber]
	,	d.[PrintOnlyFlag]
	,	d.[PrimaryPrinterName]
	,	d.[SecondaryPrinterName]
	,	d.[PrintAtEndOfDay]
	,	d.[PrintAtEndOfMonth]
	, 	d.[DWReportFlag] 
	,	d.[ReportDetailGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ReportGroupGuid]
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
 
	INSERT INTO [fmaudit].tblReportDetails (
		[ReportName]
	,	[ReportDescription]
	,	[ReportPath]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OrderNumber]
	,	[PrintOnlyFlag]
	,	[PrimaryPrinterName]
	,	[SecondaryPrinterName]
	,	[PrintAtEndOfDay]
	,	[PrintAtEndOfMonth]
	,	[DWReportFlag] 
	,	[ReportDetailGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ReportGroupGuid]
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
		i.[ReportName]
	,	i.[ReportDescription]
	,	i.[ReportPath]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[OrderNumber]
	,	i.[PrintOnlyFlag]
	,	i.[PrimaryPrinterName]
	,	i.[SecondaryPrinterName]
	,	i.[PrintAtEndOfDay]
	,	i.[PrintAtEndOfMonth]
	, 	i.[DWReportFlag] 
	,	i.[ReportDetailGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ReportGroupGuid]
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
			agl.[ReportDetailGuid]=i.[ReportDetailGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblReportDetails] ON [dbo].[tblReportDetails] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblReportDetails','D')=1 
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
	INSERT INTO [fmaudit].tblReportDetails (
		[ReportName]
	,	[ReportDescription]
	,	[ReportPath]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OrderNumber]
	,	[PrintOnlyFlag]
	,	[PrimaryPrinterName]
	,	[SecondaryPrinterName]
	,	[PrintAtEndOfDay]
	,	[PrintAtEndOfMonth]
	,	[DWReportFlag] 
	,	[ReportDetailGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ReportGroupGuid]
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
		d.[ReportName]
	,	d.[ReportDescription]
	,	d.[ReportPath]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[OrderNumber]
	,	d.[PrintOnlyFlag]
	,	d.[PrimaryPrinterName]
	,	d.[SecondaryPrinterName]
	,	d.[PrintAtEndOfDay]
	,	d.[PrintAtEndOfMonth]
	,	d.[DWReportFlag] 
	,	d.[ReportDetailGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ReportGroupGuid]
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
--Creating Delete Trigger for tblReportDetails

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblReportDetails_ClusterIdx]
    ON [dbo].[tblReportDetails]([_ClusterIdx] ASC);

