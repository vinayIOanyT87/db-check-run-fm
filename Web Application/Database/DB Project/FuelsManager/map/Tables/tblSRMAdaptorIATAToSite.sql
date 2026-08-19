CREATE TABLE [map].[tblSRMAdaptorIATAToSite] (
    [SRMAdaptorIATAToSiteGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_map_tblSRMAdaptorIATAToSite_SRMAdaptorIATAToSiteGuid] DEFAULT (newid()) NOT NULL,
    [SRMAdaptorGuid]           UNIQUEIDENTIFIER   NOT NULL,
    [SiteGuid]                 UNIQUEIDENTIFIER   NOT NULL,
    [IATAGuid]                 UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblSRMAdaptorIATAToSite_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_map_tblSRMAdaptorIATAToSite_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblSRMAdaptorIATAToSite_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_map_tblSRMAdaptorIATAToSite_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [IsEnabled]                BIT                NOT NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_map_tblSRMAdaptorIATAToSite] PRIMARY KEY NONCLUSTERED ([SRMAdaptorIATAToSiteGuid] ASC),
    CONSTRAINT [FK_map_tblSRMAdaptorIATAToSite_IATAGuid] FOREIGN KEY ([IATAGuid]) REFERENCES [dbo].[tblIATA] ([IATAGuid]),
    CONSTRAINT [FK_map_tblSRMAdaptorIATAToSite_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_map_tblSRMAdaptorIATAToSite_SRMAdaptorGuid] FOREIGN KEY ([SRMAdaptorGuid]) REFERENCES [dbo].[tblSRMAdaptor] ([SRMAdaptorGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblSRMAdaptorIATAToSite_CreatedDate]
    ON [map].[tblSRMAdaptorIATAToSite]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblSRMAdaptorIATAToSite_SRMAdaptorGuid_IATAGuid]
    ON [map].[tblSRMAdaptorIATAToSite]([SRMAdaptorGuid] ASC, [IATAGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UIX_map_tblSRMAdaptorIATAToSite_SRMAdaptorGuid_SiteGuid_IATAGuid]
    ON [map].[tblSRMAdaptorIATAToSite]([SRMAdaptorGuid] ASC, [SiteGuid] ASC, [IATAGuid] ASC);


GO
CREATE TRIGGER [map].[trg_Audit_del_tblSRMAdaptorIATAToSite] ON [map].[tblSRMAdaptorIATAToSite] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblSRMAdaptorIATAToSite','D')=1 
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
	INSERT INTO [fmaudit].map_tblSRMAdaptorIATAToSite (
		[SRMAdaptorIATAToSiteGuid]
	,	[SRMAdaptorGuid]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[IsEnabled]
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
		d.[SRMAdaptorIATAToSiteGuid]
	,	d.[SRMAdaptorGuid]
	,	d.[SiteGuid]
	,	d.[IATAGuid]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[IsEnabled]
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
CREATE TRIGGER [map].[trg_Audit_ins_tblSRMAdaptorIATAToSite] ON [map].[tblSRMAdaptorIATAToSite] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblSRMAdaptorIATAToSite','D')=1 
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
	INSERT INTO [fmaudit].map_tblSRMAdaptorIATAToSite (
		[SRMAdaptorIATAToSiteGuid]
	,	[SRMAdaptorGuid]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[IsEnabled]
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
		i.[SRMAdaptorIATAToSiteGuid]
	,	i.[SRMAdaptorGuid]
	,	i.[SiteGuid]
	,	i.[IATAGuid]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[IsEnabled]
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

CREATE TRIGGER map.trg_insupd_tblSRMAdaptorIATAToSite_ForSync
   ON map.tblSRMAdaptorIATAToSite
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application insert.
    DECLARE @syncContext varbinary(128)
    DECLARE @currentDateTimeOffset datetimeoffset(7)

	SET @syncContext = dbo.udf_GetSyncContext();
    SET @currentDateTimeOffset = sysdatetimeoffset();

	; WITH ChangeList AS (
		SELECT @syncContext AS ChangeContext
                ,d.SRMAdaptorIATAToSiteGuid AS Deleted_PK_SRMAdaptorIATAToSiteGuid
                ,i.SRMAdaptorIATAToSiteGuid AS Inserted_PK_SRMAdaptorIATAToSiteGuid
				,i.CreatedDate AS Inserted_CreatedDate
				,i.UpdatedDate AS Inserted_UpdatedDate
				,i.SiteGuid AS CurrentSiteGuid
				,d.SiteGuid AS PreviousSiteGuid
				,i._RowVersion AS Inserted_RowVersion
				,MIN_ACTIVE_ROWVERSION() - 1 AS Deleted_RowVersion
		FROM Inserted i
			FULL OUTER JOIN Deleted d ON 
            d.SRMAdaptorIATAToSiteGuid = i.SRMAdaptorIATAToSiteGuid            
	)
	MERGE INTO track.tblSRMAdaptorIATAToSite  As ct
		USING ChangeList As src
			ON src.Inserted_PK_SRMAdaptorIATAToSiteGuid = ct.PK_SRMAdaptorIATAToSiteGuid
	WHEN Matched 
	THEN 
		UPDATE SET UpdatedDate = src.Inserted_UpdatedDate
									,UpdatedContext = src.ChangeContext
									,UpdatedRowVersion = src.Inserted_RowVersion
									,CurrentSiteGuid = src.CurrentSiteGuid
									,PreviousSiteGuid = CASE WHEN (src.PreviousSiteGuid <> src.CurrentSiteGuid) THEN src.PreviousSiteGuid 
                                                                ELSE ct.PreviousSiteGuid END
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
            ,PK_SRMAdaptorIATAToSiteGuid
            )
	VALUES (src.Inserted_CreatedDate
			,src.ChangeContext
			,src.Inserted_RowVersion
			,NULL
			,NULL
			,NULL
			,src.CurrentSiteGuid
			,NULL
			,NULL
			,NULL
			,NULL
            ,src.Inserted_PK_SRMAdaptorIATAToSiteGuid
            )
	; 
END
GO

CREATE TRIGGER [map].[trg_del_tblSRMAdaptorIATAToSite_ForSync]
   ON [map].[tblSRMAdaptorIATAToSite]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application insert.
    DECLARE @syncContext varbinary(128)
    DECLARE @currentDateTimeOffset datetimeoffset(7)

	SET @syncContext = dbo.udf_GetSyncContext();
    SET @currentDateTimeOffset = sysdatetimeoffset();

	; WITH ChangeList AS (
		SELECT @syncContext AS ChangeContext
                ,d.SRMAdaptorIATAToSiteGuid AS Deleted_PK_SRMAdaptorIATAToSiteGuid
                ,d.SRMAdaptorIATAToSiteGuid AS Inserted_PK_SRMAdaptorIATAToSiteGuid
				,d.CreatedDate AS Inserted_CreatedDate
				,d.UpdatedDate AS Inserted_UpdatedDate
				,d.SiteGuid AS CurrentSiteGuid
				,NULL AS PreviousSiteGuid
				,d._RowVersion AS Inserted_RowVersion
				,MIN_ACTIVE_ROWVERSION() - 1 AS Deleted_RowVersion
		FROM Deleted d 
	)
	MERGE INTO track.tblSRMAdaptorIATAToSite  As ct
		USING ChangeList As src
			ON src.Inserted_PK_SRMAdaptorIATAToSiteGuid = ct.PK_SRMAdaptorIATAToSiteGuid
	WHEN Matched 
	THEN 
		UPDATE SET DeletedDate = @currentDateTimeOffset
									,DeletedContext = src.ChangeContext
									,DeletedRowVersion = src.Deleted_RowVersion
									,CurrentSiteGuid = src.CurrentSiteGuid
									,PreviousSiteGuid = CASE WHEN (src.CurrentSiteGuid <> ct.CurrentSiteGuid) THEN ct.CurrentSiteGuid
                                                                ELSE ct.PreviousSiteGuid END
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
            ,PK_SRMAdaptorIATAToSiteGuid
            )
	VALUES (src.Inserted_CreatedDate
			,src.ChangeContext
			,src.Inserted_RowVersion
			,NULL
			,NULL
			,NULL
			,src.CurrentSiteGuid
			,NULL
			,@currentDateTimeOffset
			,src.ChangeContext
			,src.Deleted_RowVersion
            ,src.Deleted_PK_SRMAdaptorIATAToSiteGuid
            )
	; 
END
GO
CREATE TRIGGER [map].[trg_Audit_upd_tblSRMAdaptorIATAToSite] ON [map].[tblSRMAdaptorIATAToSite] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblSRMAdaptorIATAToSite','D')=1 
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
	SRMAdaptorIATAToSiteGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblSRMAdaptorIATAToSite (
		[SRMAdaptorIATAToSiteGuid]
	,	[SRMAdaptorGuid]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[IsEnabled]
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
	OUTPUT inserted.[SRMAdaptorIATAToSiteGuid] AS 'SRMAdaptorIATAToSiteGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SRMAdaptorIATAToSiteGuid]
	,	d.[SRMAdaptorGuid]
	,	d.[SiteGuid]
	,	d.[IATAGuid]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[IsEnabled]
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
 
	INSERT INTO [fmaudit].map_tblSRMAdaptorIATAToSite (
		[SRMAdaptorIATAToSiteGuid]
	,	[SRMAdaptorGuid]
	,	[SiteGuid]
	,	[IATAGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[IsEnabled]
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
		i.[SRMAdaptorIATAToSiteGuid]
	,	i.[SRMAdaptorGuid]
	,	i.[SiteGuid]
	,	i.[IATAGuid]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[IsEnabled]
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
			agl.[SRMAdaptorIATAToSiteGuid]=i.[SRMAdaptorIATAToSiteGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblSRMAdaptorIATAToSite_ClusterIdx]
    ON [map].[tblSRMAdaptorIATAToSite]([_ClusterIdx] ASC);

