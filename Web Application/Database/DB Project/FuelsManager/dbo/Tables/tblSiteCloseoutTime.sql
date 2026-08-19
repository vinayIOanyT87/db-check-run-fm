CREATE TABLE [dbo].[tblSiteCloseoutTime]
(
	[SiteCloseoutTimeGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT (newid()), 
    [EffectiveDate] DATETIMEOFFSET NOT NULL, 
    [ExpirationDate] DATETIMEOFFSET NULL, 
    [CloseoutTime] TIME NULL,
	 [PointTagRefDataAsXML] XML NULL,
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [CreatedBy] [dbo].[udtUserID] NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] NOT NULL, 
    [SiteGuid] UNIQUEIDENTIFIER NOT NULL, 
    [_RowVersion] ROWVERSION NOT NULL, 
    [_ClusterIdx] BIGINT NOT NULL IDENTITY, 
    CONSTRAINT [PK_tblSiteCloseoutTime_GUID] PRIMARY KEY NONCLUSTERED ([SiteCloseoutTimeGuid] ASC),
    CONSTRAINT [FK_tblSiteCloseoutTime_tblSites] FOREIGN KEY ([SiteGuid]) REFERENCES [tblSites]([SiteGuid])
)
GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-SiteGuid-EffectiveDate] ON [dbo].[tblSiteCloseoutTime]
(
	[EffectiveDate] ASC,
	[SiteGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [NonClusteredIndex-SiteGuid-ExpirationDate] ON [dbo].[tblSiteCloseoutTime]
(
	[ExpirationDate] ASC,
	[SiteGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


ALTER TABLE [dbo].[tblSiteCloseoutTime]  WITH CHECK ADD  CONSTRAINT [CK_tblSiteCloseoutTime] CHECK  (([EffectiveDate]<=isnull([ExpirationDate],[EffectiveDate])))
GO

ALTER TABLE [dbo].[tblSiteCloseoutTime] CHECK CONSTRAINT [CK_tblSiteCloseoutTime]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Effective date cannot be after expiration date' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'tblSiteCloseoutTime', @level2type=N'CONSTRAINT',@level2name=N'CK_tblSiteCloseoutTime'
GO


CREATE TRIGGER [dbo].[trg_Audit_upd_tblSiteCloseoutTime] ON [dbo].[tblSiteCloseoutTime] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSiteCloseoutTime','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDatetime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDatetime = SYSDATETIMEOFFSET();
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
	INSERT INTO [fmaudit].tblSiteCloseoutTime WITH(ROWLOCK)(
		[SiteCloseoutTimeGuid]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[CloseoutTime]
	,  [PointTagRefDataAsXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[SiteGuid]
	,	[OriginalRowVersion]
	,	[_ClusterIdx]
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
		d.[SiteCloseoutTimeGuid]
	,	d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[CloseoutTime]
	,	d.[PointTagRefDataAsXML]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[SiteGuid]
	,	d.[_RowVersion]
	,	d.[_ClusterIdx]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDatetime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
	INSERT INTO [fmaudit].tblSiteCloseoutTime WITH(ROWLOCK)(
		[SiteCloseoutTimeGuid]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[CloseoutTime]
	,  [PointTagRefDataAsXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[SiteGuid]
	,	[OriginalRowVersion]
	,	[_ClusterIdx]
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
		i.[SiteCloseoutTimeGuid]
	,	i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[CloseoutTime]
	,  i.[PointTagRefDataAsXML]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[SiteGuid]
	,	i.[_RowVersion]
	,	i.[_ClusterIdx]
	,	@_AuditEventType
	,	2
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDatetime
	,	@_AuditSiteGUID
	,	a._AuditGUID
	,	@_UserId
 ,   @_AuditContext
	FROM inserted i 
	INNER JOIN	[fmaudit].[tblSiteCloseoutTime] a ON
	(
		a.[SiteCloseoutTimeGuid]=i.[SiteCloseoutTimeGuid] 
	)
	WHERE	a._AuditEventType='U'
	AND		a._AuditEventSequence=1 
	AND		a._AuditCreatedDate= @_AuditDatetime
END
GO

CREATE TRIGGER [dbo].[trg_Audit_ins_tblSiteCloseoutTime] ON [dbo].[tblSiteCloseoutTime] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSiteCloseoutTime','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDatetime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDatetime = SYSDATETIMEOFFSET();
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
	INSERT INTO [fmaudit].tblSiteCloseoutTime WITH(ROWLOCK)(
		[SiteCloseoutTimeGuid]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[CloseoutTime]
	,  [PointTagRefDataAsXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[SiteGuid]
	,	[OriginalRowVersion]
	,	[_ClusterIdx]
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
		i.[SiteCloseoutTimeGuid]
	,	i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[CloseoutTime]
	,  i.[PointTagRefDataAsXML]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[SiteGuid]
	,	i.[_RowVersion]
	,	i.[_ClusterIdx]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDatetime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM inserted i
END
GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblSiteCloseoutTime] ON [dbo].[tblSiteCloseoutTime] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblSiteCloseoutTime','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDatetime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDatetime = SYSDATETIMEOFFSET();
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
	INSERT INTO [fmaudit].tblSiteCloseoutTime WITH(ROWLOCK)(
		[SiteCloseoutTimeGuid]
	,	[EffectiveDate]
	,	[ExpirationDate]
	,	[CloseoutTime]
	,  [PointTagRefDataAsXML]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[SiteGuid]
	,	[OriginalRowVersion]
	,	[_ClusterIdx]
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
		d.[SiteCloseoutTimeGuid]
	,	d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[CloseoutTime]
	,  d.[PointTagRefDataAsXML]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[SiteGuid]
	,	d.[_RowVersion]
	,	d.[_ClusterIdx]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDatetime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
END
GO

--Creating Insert / Update Trigger for tblSiteCloseoutTime
CREATE TRIGGER dbo.trg_insupd_tblSiteCloseoutTime_ForSync 
	ON dbo.tblSiteCloseoutTime
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
				,d.SiteCloseoutTimeGuid AS Deleted_PK_SiteCloseoutTimeGuid
			,i.SiteCloseoutTimeGuid AS Inserted_PK_SiteCloseoutTimeGuid
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
					FULL OUTER JOIN Deleted d 
						ON d.SiteCloseoutTimeGuid = i.SiteCloseoutTimeGuid
 
		UPDATE currentTrackingData SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
				,UpdatedContext = entityChanges.ChangeContext 
				,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
				,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblSiteCloseoutTime As currentTrackingData 
				JOIN #ChangeList As entityChanges 
					ON entityChanges.Inserted_PK_SiteCloseoutTimeGuid = currentTrackingData.PK_SiteCloseoutTimeGuid
 
		INSERT track.tblSiteCloseoutTime (InsertedDate 
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
										,PK_SiteCloseoutTimeGuid
										,FK_ParentPK 
		)
		SELECT CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
					WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
					ELSE CAST('1/1/1990' AS DateTimeOffset(7)) 
				END 
				,entityChanges.ChangeContext 
				,entityChanges.Inserted_RowVersion 
				,entityChanges.Inserted_CreatedDate 
				,entityChanges.ChangeContext 
				,entityChanges.Inserted_RowVersion 
				,NULL 
				,NULL 
				,NULL 
				,entityChanges.CurrentSiteGuid 
				,CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), entityChanges.PreviousSiteGuid),'')) THEN entityChanges.PreviousSiteGuid 
					  ELSE NULL 
				 END
				,entityChanges.Inserted_PK_SiteCloseoutTimeGuid
				,entityChanges.Inserted_FK_ParentPK
			FROM #ChangeList As entityChanges 
				WHERE NOT EXISTS ( SELECT 1 
										FROM track.tblSiteCloseoutTime As currentTrackingData
										WHERE entityChanges.Inserted_PK_SiteCloseoutTimeGuid = currentTrackingData.PK_SiteCloseoutTimeGuid
								)
	END
END 
GO
--Creating Delete Trigger for tblSiteCloseoutTime
CREATE TRIGGER dbo.trg_del_tblSiteCloseoutTime_ForSync 
   ON dbo.tblSiteCloseoutTime
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
						,d.SiteCloseoutTimeGuid AS Deleted_PK_SiteCloseoutTimeGuid
                        ,d.SiteCloseoutTimeGuid AS Inserted_PK_SiteCloseoutTimeGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblSiteCloseoutTime As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_SiteCloseoutTimeGuid = currentTrackingData.PK_SiteCloseoutTimeGuid
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
						,PK_SiteCloseoutTimeGuid
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
						,entityChanges.Deleted_PK_SiteCloseoutTimeGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO