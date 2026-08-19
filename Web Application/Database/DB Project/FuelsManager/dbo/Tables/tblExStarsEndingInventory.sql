/****** Object:  Table [dbo].[tblExStarsEndingInventory]    Script Date: 6/23/2014 1:57:14 PM ******/

GO
CREATE TABLE [dbo].[tblExStarsEndingInventory] (
    [ManagerCompanyGuid]         UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]                   UNIQUEIDENTIFIER   NOT NULL,
    [ProductGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [ReportYear]                 INT                NOT NULL,
    [ReportMonth]                INT                NOT NULL,
    [ReportDay]                  INT                NOT NULL,
    [PriorInventoryExists]       BIT                NOT NULL,
    [GrossVolume]                FLOAT (53)         NOT NULL,
    [NetVolume]                  FLOAT (53)         NOT NULL,
    [EndingInventoryDate]        DATETIMEOFFSET (7) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  NOT NULL,
    [ExStarsEndingInventoryGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExStarsEndingInventory_GUID] DEFAULT (newid()) NOT NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    PRIMARY KEY NONCLUSTERED ([ExStarsEndingInventoryGuid] ASC),
    CONSTRAINT [fk_ExStarsEndingInventoryManager] FOREIGN KEY ([ManagerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [fk_ExStarsEndingInventoryProduct] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
    CONSTRAINT [fk_ExStarsEndingInventorySite] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);

 
GO
CREATE NONCLUSTERED INDEX [IX_tblExStarsEndingInventory_CreatedDate]
    ON [dbo].[tblExStarsEndingInventory]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblExStarsEndingInventory_ClusterIdx]
    ON [dbo].[tblExStarsEndingInventory]([_ClusterIdx] ASC);

GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblExStarsEndingInventory] ON [dbo].[tblExStarsEndingInventory] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsEndingInventory','D')=1 
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
	ExStarsEndingInventoryGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblExStarsEndingInventory (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ProductGuid]
	,	[ReportYear]
	,	[ReportMonth]
	,	[ReportDay]
	,	[PriorInventoryExists]
	,	[GrossVolume]
	,	[NetVolume]
	,	[EndingInventoryDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsEndingInventoryGuid]
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
	OUTPUT inserted.[ExStarsEndingInventoryGuid] AS 'ExStarsEndingInventoryGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ManagerCompanyGuid]
	,	d.[SiteGuid]
	,	d.[ProductGuid]
	,	d.[ReportYear]
	,	d.[ReportMonth]
	,	d.[ReportDay]
	,	d.[PriorInventoryExists]
	,	d.[GrossVolume]
	,	d.[NetVolume]
	,	d.[EndingInventoryDate]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsEndingInventoryGuid]
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
 
	INSERT INTO [fmaudit].tblExStarsEndingInventory (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ProductGuid]
	,	[ReportYear]
	,	[ReportMonth]
	,	[ReportDay]
	,	[PriorInventoryExists]
	,	[GrossVolume]
	,	[NetVolume]
	,	[EndingInventoryDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsEndingInventoryGuid]
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
	,	i.[ProductGuid]
	,	i.[ReportYear]
	,	i.[ReportMonth]
	,	i.[ReportDay]
	,	i.[PriorInventoryExists]
	,	i.[GrossVolume]
	,	i.[NetVolume]
	,	i.[EndingInventoryDate]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsEndingInventoryGuid]
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
			agl.[ExStarsEndingInventoryGuid]=i.[ExStarsEndingInventoryGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblExStarsEndingInventory] ON [dbo].[tblExStarsEndingInventory] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsEndingInventory','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsEndingInventory (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ProductGuid]
	,	[ReportYear]
	,	[ReportMonth]
	,	[ReportDay]
	,	[PriorInventoryExists]
	,	[GrossVolume]
	,	[NetVolume]
	,	[EndingInventoryDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsEndingInventoryGuid]
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
	,	d.[ProductGuid]
	,	d.[ReportYear]
	,	d.[ReportMonth]
	,	d.[ReportDay]
	,	d.[PriorInventoryExists]
	,	d.[GrossVolume]
	,	d.[NetVolume]
	,	d.[EndingInventoryDate]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ExStarsEndingInventoryGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblExStarsEndingInventory] ON [dbo].[tblExStarsEndingInventory] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExStarsEndingInventory','D')=1 
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
	INSERT INTO [fmaudit].tblExStarsEndingInventory (
		[ManagerCompanyGuid]
	,	[SiteGuid]
	,	[ProductGuid]
	,	[ReportYear]
	,	[ReportMonth]
	,	[ReportDay]
	,	[PriorInventoryExists]
	,	[GrossVolume]
	,	[NetVolume]
	,	[EndingInventoryDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ExStarsEndingInventoryGuid]
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
	,	i.[ProductGuid]
	,	i.[ReportYear]
	,	i.[ReportMonth]
	,	i.[ReportDay]
	,	i.[PriorInventoryExists]
	,	i.[GrossVolume]
	,	i.[NetVolume]
	,	i.[EndingInventoryDate]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ExStarsEndingInventoryGuid]
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