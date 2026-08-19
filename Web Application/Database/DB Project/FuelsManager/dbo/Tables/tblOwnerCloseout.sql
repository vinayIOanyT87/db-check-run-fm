CREATE TABLE [dbo].[tblOwnerCloseout] (
    [Site]               NVARCHAR (30)      CONSTRAINT [DF_tblOwnerCloseout_Site] DEFAULT ('') NOT NULL,
    [ManagerName]        NVARCHAR (100)     CONSTRAINT [DF_tblOwnerCloseout_ManagerName] DEFAULT ('') NOT NULL,
    [ProductName]        NVARCHAR (30)      CONSTRAINT [DF_tblOwnerCloseout_ProductName] DEFAULT ('') NOT NULL,
    [CloseoutDate]       DATE               CONSTRAINT [DF_tblOwnerCloseout_CloseoutDate] DEFAULT (sysdatetime()) NOT NULL,
    [OwnerName]          NVARCHAR (100)     CONSTRAINT [DF_tblOwnerCloseout_OwnerName] DEFAULT ('') NOT NULL,
    [GrossBookInventory] FLOAT (53)         NULL,
    [NetBookInventory]   FLOAT (53)         NULL,
    [CreatedDate]        DATETIMEOFFSET (7) NULL,
    [CreatedBy]          [dbo].[udtUserID]  NULL,
    [UpdatedDate]        DATETIMEOFFSET (7) NULL,
    [UpdatedBy]          [dbo].[udtUserID]  NULL,
    [GrossBookPrice]     FLOAT (53)         NULL,
    [NetBookPrice]       FLOAT (53)         NULL,
    [TransVersion]       BIGINT             NULL,
    [MassBookInventory]  FLOAT (53)         NULL,
    [MassBookPrice]      FLOAT (53)         NULL,
    [OwnerCloseoutGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_tblOwnerCloseout_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]        ROWVERSION         NOT NULL,
    [SiteGuid]           UNIQUEIDENTIFIER   NULL,
    [ManagerCompanyGuid] UNIQUEIDENTIFIER   NULL,
    [OwnerCompanyGuid]   UNIQUEIDENTIFIER   NULL,
    [ProductGuid]        UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblOwnerCloseout_GUID] PRIMARY KEY NONCLUSTERED ([OwnerCloseoutGuid] ASC),
    CONSTRAINT [FK_tblOwnerCloseout_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);








GO
CREATE NONCLUSTERED INDEX [IX_tblOwnerCloseout_CreatedDate]
    ON [dbo].[tblOwnerCloseout]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblOwnerCloseout] ON [dbo].[tblOwnerCloseout] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblOwnerCloseout','D')=1 
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
	OwnerCloseoutGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblOwnerCloseout (
		[Site]
	,	[ManagerName]
	,	[ProductName]
	,	[CloseoutDate]
	,	[OwnerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassBookPrice]
	,	[OwnerCloseoutGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
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
	OUTPUT inserted.[OwnerCloseoutGuid] AS 'OwnerCloseoutGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[Site]
	,	d.[ManagerName]
	,	d.[ProductName]
	,	d.[CloseoutDate]
	,	d.[OwnerName]
	,	d.[GrossBookInventory]
	,	d.[NetBookInventory]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[GrossBookPrice]
	,	d.[NetBookPrice]
	,	d.[TransVersion]
	,	d.[MassBookInventory]
	,	d.[MassBookPrice]
	,	d.[OwnerCloseoutGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[OwnerCompanyGuid]
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
 
	INSERT INTO [fmaudit].tblOwnerCloseout (
		[Site]
	,	[ManagerName]
	,	[ProductName]
	,	[CloseoutDate]
	,	[OwnerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassBookPrice]
	,	[OwnerCloseoutGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
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
	,	i.[ManagerName]
	,	i.[ProductName]
	,	i.[CloseoutDate]
	,	i.[OwnerName]
	,	i.[GrossBookInventory]
	,	i.[NetBookInventory]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[GrossBookPrice]
	,	i.[NetBookPrice]
	,	i.[TransVersion]
	,	i.[MassBookInventory]
	,	i.[MassBookPrice]
	,	i.[OwnerCloseoutGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[OwnerCompanyGuid]
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
			agl.[OwnerCloseoutGuid]=i.[OwnerCloseoutGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblOwnerCloseout
CREATE TRIGGER dbo.trg_insupd_tblOwnerCloseout_ForSync 
   ON dbo.tblOwnerCloseout
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
                    ,d.OwnerCloseoutGuid AS Deleted_PK_OwnerCloseoutGuid
                    ,i.OwnerCloseoutGuid AS Inserted_PK_OwnerCloseoutGuid
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
				    d.OwnerCloseoutGuid = i.OwnerCloseoutGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblOwnerCloseout As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_OwnerCloseoutGuid = currentTrackingData.PK_OwnerCloseoutGuid
 
 
		    INSERT track.tblOwnerCloseout (InsertedDate 
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
				    ,PK_OwnerCloseoutGuid
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
				    ,entityChanges.Inserted_PK_OwnerCloseoutGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblOwnerCloseout As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_OwnerCloseoutGuid = currentTrackingData.PK_OwnerCloseoutGuid
)
    END
END 

GO
--Creating Delete Trigger for tblOwnerCloseout
CREATE TRIGGER dbo.trg_del_tblOwnerCloseout_ForSync 
   ON dbo.tblOwnerCloseout
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
						,d.OwnerCloseoutGuid AS Deleted_PK_OwnerCloseoutGuid
                        ,d.OwnerCloseoutGuid AS Inserted_PK_OwnerCloseoutGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblOwnerCloseout As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_OwnerCloseoutGuid = currentTrackingData.PK_OwnerCloseoutGuid
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
						,PK_OwnerCloseoutGuid
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
						,entityChanges.Deleted_PK_OwnerCloseoutGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblOwnerCloseout] ON [dbo].[tblOwnerCloseout] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblOwnerCloseout','D')=1 
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
	INSERT INTO [fmaudit].tblOwnerCloseout (
		[Site]
	,	[ManagerName]
	,	[ProductName]
	,	[CloseoutDate]
	,	[OwnerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassBookPrice]
	,	[OwnerCloseoutGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
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
	,	d.[ManagerName]
	,	d.[ProductName]
	,	d.[CloseoutDate]
	,	d.[OwnerName]
	,	d.[GrossBookInventory]
	,	d.[NetBookInventory]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[GrossBookPrice]
	,	d.[NetBookPrice]
	,	d.[TransVersion]
	,	d.[MassBookInventory]
	,	d.[MassBookPrice]
	,	d.[OwnerCloseoutGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[OwnerCompanyGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblOwnerCloseout] ON [dbo].[tblOwnerCloseout] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblOwnerCloseout','D')=1 
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
	INSERT INTO [fmaudit].tblOwnerCloseout (
		[Site]
	,	[ManagerName]
	,	[ProductName]
	,	[CloseoutDate]
	,	[OwnerName]
	,	[GrossBookInventory]
	,	[NetBookInventory]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[GrossBookPrice]
	,	[NetBookPrice]
	,	[TransVersion]
	,	[MassBookInventory]
	,	[MassBookPrice]
	,	[OwnerCloseoutGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
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
	,	i.[ManagerName]
	,	i.[ProductName]
	,	i.[CloseoutDate]
	,	i.[OwnerName]
	,	i.[GrossBookInventory]
	,	i.[NetBookInventory]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[GrossBookPrice]
	,	i.[NetBookPrice]
	,	i.[TransVersion]
	,	i.[MassBookInventory]
	,	i.[MassBookPrice]
	,	i.[OwnerCloseoutGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[OwnerCompanyGuid]
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

/****** Object:  Index [IX_tblOwnerCloseOut_MgrOwnerProdDate]    Script Date: 1/27/2014 11:26:09 AM ******/
CREATE NONCLUSTERED INDEX [IX_tblOwnerCloseOut_MgrOwnerProdDate] ON [dbo].[tblOwnerCloseout]
(
	[CloseoutDate] ASC,
	[SiteGuid] ASC,
	[ManagerCompanyGuid] ASC,
	[OwnerCompanyGuid] ASC,
	[ProductGuid] ASC
)

GO
CREATE NONCLUSTERED INDEX IX_tblOwnerCloseout_ManagerCompanyGuid ON [dbo].[tblOwnerCloseout]([ManagerCompanyGuid])
GO
CREATE NONCLUSTERED INDEX IX_tblOwnerCloseout_OwnerCompanyGuid ON [dbo].[tblOwnerCloseout]([OwnerCompanyGuid])
GO
CREATE NONCLUSTERED INDEX [IX_tblOwnerCloseOut_MgrNameOwnerNameProdNameDate]
    ON [dbo].[tblOwnerCloseout]([Site] ASC, [ManagerName] ASC, [ProductName] ASC, [OwnerName] ASC, [CloseoutDate] ASC)
    INCLUDE([GrossBookInventory], [NetBookInventory], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [GrossBookPrice], [NetBookPrice], [TransVersion], [MassBookInventory], [MassBookPrice], [OwnerCloseoutGuid], [_RowVersion], [SiteGuid], [ManagerCompanyGuid], [OwnerCompanyGuid], [ProductGuid]);


GO
CREATE NONCLUSTERED INDEX [IX_tblOwnerCloseOut_JournalReportCoveringIndex]
    ON [dbo].[tblOwnerCloseout]([ManagerCompanyGuid] ASC, [OwnerCompanyGuid] ASC, [ProductGuid] ASC, [CloseoutDate] ASC)
    INCLUDE([SiteGuid]);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblOwnerCloseout_ClusterIdx]
    ON [dbo].[tblOwnerCloseout]([_ClusterIdx] ASC);

