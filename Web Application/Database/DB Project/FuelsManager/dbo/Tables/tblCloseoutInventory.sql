CREATE TABLE [dbo].[tblCloseoutInventory] (
    [Site]                   NVARCHAR (30)      NULL,
    [CloseoutDate]           DATE               CONSTRAINT [DF_tblCloseoutInventory_CloseoutDate] DEFAULT (sysdatetime()) NOT NULL,
    [ProductName]            NVARCHAR (30)      CONSTRAINT [DF_tblCloseoutInventory_ProductName] DEFAULT ('') NOT NULL,
    [ManagerName]            NVARCHAR (100)     CONSTRAINT [DF_tblCloseoutInventory_ManagerName] DEFAULT ('') NOT NULL,
    [GrossBookInventory]     FLOAT (53)         NULL,
    [NetBookInventory]       FLOAT (53)         NULL,
    [GrossPhysicalInventory] FLOAT (53)         NULL,
    [NetPhysicalInventory]   FLOAT (53)         NULL,
    [GrossVariance]          FLOAT (53)         NULL,
    [NetVariance]            FLOAT (53)         NULL,
    [CreatedDate]            DATETIMEOFFSET (7) NULL,
    [CreatedBy]              [dbo].[udtUserID]  NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NULL,
    [GrossBookPrice]         FLOAT (53)         NULL,
    [NetBookPrice]           FLOAT (53)         NULL,
    [GrossPhysicalPrice]     FLOAT (53)         NULL,
    [NetPhysicalPrice]       FLOAT (53)         NULL,
    [TransVersion]           BIGINT             NULL,
    [MassBookInventory]      FLOAT (53)         NULL,
    [MassPhysicalInventory]  FLOAT (53)         NULL,
    [MassVariance]           FLOAT (53)         NULL,
    [MassBookPrice]          FLOAT (53)         NULL,
    [MassPhysicalPrice]      FLOAT (53)         NULL,
    [CloseoutInventoryGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_tblCloseoutInventory_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [SiteGuid]               UNIQUEIDENTIFIER   NULL,
    [ManagerCompanyGuid]     UNIQUEIDENTIFIER   NULL,
    [ProductGuid]            UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]            BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblCloseoutInventory_GUID] PRIMARY KEY NONCLUSTERED ([CloseoutInventoryGuid] ASC),
    CONSTRAINT [FK_tblCloseoutInventory_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblCloseoutInventory_CreatedDate]
    ON [dbo].[tblCloseoutInventory]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblCloseoutInventory_SiteGuid_CloseoutDate_ProductGuid_ManagerCompanyGuid]
    ON [dbo].[tblCloseoutInventory]([SiteGuid] ASC, [CloseoutDate] ASC, [ProductGuid] ASC, [ManagerCompanyGuid] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblCloseoutInventory] ON [dbo].[tblCloseoutInventory] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblCloseoutInventory','D')=1 
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
	INSERT INTO [fmaudit].tblCloseoutInventory (
		[Site]
	,	[CloseoutDate]
	,	[ProductName]
	,	[ManagerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[GrossPhysicalInventory]
	,	[NetPhysicalInventory]
	,	[GrossVariance]
	,	[NetVariance]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[GrossPhysicalPrice]
	,	[NetPhysicalPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassPhysicalInventory]
	,	[MassVariance]
	,	[MassBookPrice]
	,	[MassPhysicalPrice]
	,	[CloseoutInventoryGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
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
		d.[Site]
	,	d.[CloseoutDate]
	,	d.[ProductName]
	,	d.[ManagerName]
	,	d.[GrossBookInventory]
	,	d.[NetBookInventory]
	,	d.[GrossPhysicalInventory]
	,	d.[NetPhysicalInventory]
	,	d.[GrossVariance]
	,	d.[NetVariance]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[GrossBookPrice]
	,	d.[NetBookPrice]
	,	d.[GrossPhysicalPrice]
	,	d.[NetPhysicalPrice]
	,	d.[TransVersion]
	,	d.[MassBookInventory]
	,	d.[MassPhysicalInventory]
	,	d.[MassVariance]
	,	d.[MassBookPrice]
	,	d.[MassPhysicalPrice]
	,	d.[CloseoutInventoryGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[ProductGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblCloseoutInventory] ON [dbo].[tblCloseoutInventory] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblCloseoutInventory','D')=1 
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
	INSERT INTO [fmaudit].tblCloseoutInventory (
		[Site]
	,	[CloseoutDate]
	,	[ProductName]
	,	[ManagerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[GrossPhysicalInventory]
	,	[NetPhysicalInventory]
	,	[GrossVariance]
	,	[NetVariance]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[GrossPhysicalPrice]
	,	[NetPhysicalPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassPhysicalInventory]
	,	[MassVariance]
	,	[MassBookPrice]
	,	[MassPhysicalPrice]
	,	[CloseoutInventoryGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
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
		i.[Site]
	,	i.[CloseoutDate]
	,	i.[ProductName]
	,	i.[ManagerName]
	,	i.[GrossBookInventory]
	,	i.[NetBookInventory]
	,	i.[GrossPhysicalInventory]
	,	i.[NetPhysicalInventory]
	,	i.[GrossVariance]
	,	i.[NetVariance]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[GrossBookPrice]
	,	i.[NetBookPrice]
	,	i.[GrossPhysicalPrice]
	,	i.[NetPhysicalPrice]
	,	i.[TransVersion]
	,	i.[MassBookInventory]
	,	i.[MassPhysicalInventory]
	,	i.[MassVariance]
	,	i.[MassBookPrice]
	,	i.[MassPhysicalPrice]
	,	i.[CloseoutInventoryGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[ProductGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblCloseoutInventory] ON [dbo].[tblCloseoutInventory] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblCloseoutInventory','D')=1 
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
	CloseoutInventoryGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblCloseoutInventory (
		[Site]
	,	[CloseoutDate]
	,	[ProductName]
	,	[ManagerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[GrossPhysicalInventory]
	,	[NetPhysicalInventory]
	,	[GrossVariance]
	,	[NetVariance]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[GrossPhysicalPrice]
	,	[NetPhysicalPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassPhysicalInventory]
	,	[MassVariance]
	,	[MassBookPrice]
	,	[MassPhysicalPrice]
	,	[CloseoutInventoryGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
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
	OUTPUT inserted.[CloseoutInventoryGuid] AS 'CloseoutInventoryGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[Site]
	,	d.[CloseoutDate]
	,	d.[ProductName]
	,	d.[ManagerName]
	,	d.[GrossBookInventory]
	,	d.[NetBookInventory]
	,	d.[GrossPhysicalInventory]
	,	d.[NetPhysicalInventory]
	,	d.[GrossVariance]
	,	d.[NetVariance]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[GrossBookPrice]
	,	d.[NetBookPrice]
	,	d.[GrossPhysicalPrice]
	,	d.[NetPhysicalPrice]
	,	d.[TransVersion]
	,	d.[MassBookInventory]
	,	d.[MassPhysicalInventory]
	,	d.[MassVariance]
	,	d.[MassBookPrice]
	,	d.[MassPhysicalPrice]
	,	d.[CloseoutInventoryGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[ProductGuid]
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
 
	INSERT INTO [fmaudit].tblCloseoutInventory (
		[Site]
	,	[CloseoutDate]
	,	[ProductName]
	,	[ManagerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[GrossPhysicalInventory]
	,	[NetPhysicalInventory]
	,	[GrossVariance]
	,	[NetVariance]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[GrossPhysicalPrice]
	,	[NetPhysicalPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassPhysicalInventory]
	,	[MassVariance]
	,	[MassBookPrice]
	,	[MassPhysicalPrice]
	,	[CloseoutInventoryGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
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
		i.[Site]
	,	i.[CloseoutDate]
	,	i.[ProductName]
	,	i.[ManagerName]
	,	i.[GrossBookInventory]
	,	i.[NetBookInventory]
	,	i.[GrossPhysicalInventory]
	,	i.[NetPhysicalInventory]
	,	i.[GrossVariance]
	,	i.[NetVariance]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[GrossBookPrice]
	,	i.[NetBookPrice]
	,	i.[GrossPhysicalPrice]
	,	i.[NetPhysicalPrice]
	,	i.[TransVersion]
	,	i.[MassBookInventory]
	,	i.[MassPhysicalInventory]
	,	i.[MassVariance]
	,	i.[MassBookPrice]
	,	i.[MassPhysicalPrice]
	,	i.[CloseoutInventoryGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[ProductGuid]
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
			agl.[CloseoutInventoryGuid]=i.[CloseoutInventoryGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblCloseoutInventory
CREATE TRIGGER dbo.trg_insupd_tblCloseoutInventory_ForSync 
   ON dbo.tblCloseoutInventory
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
                    ,d.CloseoutInventoryGuid AS Deleted_PK_CloseoutInventoryGuid
                    ,i.CloseoutInventoryGuid AS Inserted_PK_CloseoutInventoryGuid
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
				    d.CloseoutInventoryGuid = i.CloseoutInventoryGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblCloseoutInventory As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_CloseoutInventoryGuid = currentTrackingData.PK_CloseoutInventoryGuid
 
 
		    INSERT track.tblCloseoutInventory (InsertedDate 
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
				    ,PK_CloseoutInventoryGuid
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
				    ,entityChanges.Inserted_PK_CloseoutInventoryGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblCloseoutInventory As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_CloseoutInventoryGuid = currentTrackingData.PK_CloseoutInventoryGuid
)
    END
END 

GO
--Creating Delete Trigger for tblCloseoutInventory
CREATE TRIGGER dbo.trg_del_tblCloseoutInventory_ForSync 
   ON dbo.tblCloseoutInventory
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
						,d.CloseoutInventoryGuid AS Deleted_PK_CloseoutInventoryGuid
                        ,d.CloseoutInventoryGuid AS Inserted_PK_CloseoutInventoryGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblCloseoutInventory As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_CloseoutInventoryGuid = currentTrackingData.PK_CloseoutInventoryGuid
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
						,PK_CloseoutInventoryGuid
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
						,entityChanges.Deleted_PK_CloseoutInventoryGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE NONCLUSTERED INDEX IX_tblCloseoutInventory_ManagerCompanyGuid ON [tblCloseoutInventory]([ManagerCompanyGuid])
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblCloseoutInventory_ClusterIdx]
    ON [dbo].[tblCloseoutInventory]([_ClusterIdx] ASC);

