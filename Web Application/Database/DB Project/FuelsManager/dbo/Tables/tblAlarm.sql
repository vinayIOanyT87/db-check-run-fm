CREATE TABLE [dbo].[tblAlarm]
(
	[AlarmGuid] [uniqueidentifier] NOT NULL DEFAULT (newid()),
	[InputTagGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[Enabled] [Bit] Not NULL DEFAULT (1),
	[AlarmCategoryApplicationStringGuid] [uniqueidentifier] NOT NULL,
	[Order] [int] NOT NULL DEFAULT (0),
	[NotAlarmState] [nvarchar](100) NOT NULL DEFAULT ('Normal'),
	[Comment] [nvarchar](256) NULL,
	[ShelvedStartTimeStamp] [datetimeoffset](7),
	[ShelvedEndTimeStamp] [datetimeoffset](7),
	[ShelvedOneShot] [Bit]NOT NULL DEFAULT (0),
	[ShelvedBy] [dbo].[udtUserID] NULL DEFAULT (''),
	[Suppressed] [Bit]NOT NULL DEFAULT (0),
	[CreatedDate] [datetimeoffset](7) NOT NULL DEFAULT (sysdatetimeoffset()),
	[CreatedBy] [dbo].[udtUserID] NOT NULL DEFAULT (''),
	[UpdatedDate] [datetimeoffset](7) NOT NULL DEFAULT (sysdatetimeoffset()),
	[UpdatedBy] [dbo].[udtUserID] NOT NULL DEFAULT (''),
	[_RowVersion] ROWVERSION NOT NULL,
	[AlarmStateTagGuid] UNIQUEIDENTIFIER NOT NULL, 
	[ExclusiveAlarm] BIT NOT NULL DEFAULT 1, 
	[AlarmTemplateGuid] [uniqueidentifier] NULL,
	Notify bit NULL,
	CONSTRAINT [PK_tblAlarm_GUID] PRIMARY KEY NONCLUSTERED ([AlarmGuid] ASC), 
	CONSTRAINT [FK_tblAlarm_InputTagGuid] FOREIGN KEY ([InputTagGuid]) REFERENCES [dbo].[tblPointTag] ([PointTagGuid]),
	CONSTRAINT [FK_tblAlarm_AlarmStateTagGuid] FOREIGN KEY ([AlarmStateTagGuid]) REFERENCES [dbo].[tblPointTag] ([PointTagGuid]),
	CONSTRAINT [FK_tblAlarm_AlarmCategoryApplicationStringGuid] FOREIGN KEY([AlarmCategoryApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
	CONSTRAINT [FK_tblAlarm_AlarmTemplate] FOREIGN KEY([AlarmTemplateGuid]) REFERENCES [dbo].[tblAlarmTemplate] ([AlarmTemplateGuid])
)
--Don't forget to change the tblPoint rowversion trigger is columns are added or deleted or column order changed
GO

CREATE NONCLUSTERED INDEX [IX_tblAlarm_InputTagGuid]
    ON [dbo].[tblAlarm]([InputTagGuid] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_tblAlarm_InputTagGuid_AlarmTemplateGuid]
    ON [dbo].[tblAlarm](
	[InputTagGuid] ASC,
	[AlarmTemplateGuid] ASC);
GO
--Creating Insert / Update Trigger for tblAlarm
CREATE TRIGGER dbo.trg_insupd_tblAlarm_ForSync 
   ON dbo.tblAlarm
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
                    ,d.AlarmGuid AS Deleted_PK_AlarmGuid
                    ,i.AlarmGuid AS Inserted_PK_AlarmGuid
                    ,d.InputTagGuid AS Deleted_FK_ParentPK
                    ,i.InputTagGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.AlarmGuid = i.AlarmGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAlarm As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AlarmGuid = currentTrackingData.PK_AlarmGuid
 
 
		    INSERT track.tblAlarm (InsertedDate 
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
				    ,PK_AlarmGuid
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
				    ,entityChanges.Inserted_PK_AlarmGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAlarm As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AlarmGuid = currentTrackingData.PK_AlarmGuid
)
    END
END 
GO
--Creating Delete Trigger for tblAlarm
CREATE TRIGGER dbo.trg_del_tblAlarm_ForSync 
   ON dbo.tblAlarm
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
						,d.AlarmGuid AS Deleted_PK_AlarmGuid
                        ,d.AlarmGuid AS Inserted_PK_AlarmGuid
                        ,d.InputTagGuid AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAlarm As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AlarmGuid = currentTrackingData.PK_AlarmGuid
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
						,PK_AlarmGuid
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
						,entityChanges.Deleted_PK_AlarmGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAlarm] ON [dbo].[tblAlarm] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarm','D')=1 
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
	INSERT INTO [fmaudit].tblAlarm (
		[AlarmGuid]
	,	[InputTagGuid]
	,	[ID]
	,	[Enabled]
	,	[AlarmCategoryApplicationStringGuid]
	,	[Order]
	,	[NotAlarmState]
	,	[Comment]
	,	[ShelvedStartTimeStamp]
	,	[ShelvedEndTimeStamp]
	,	[ShelvedOneShot]
	,	[ShelvedBy]
	,	[Suppressed]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AlarmStateTagGuid]
	,	[ExclusiveAlarm]
	,	[AlarmTemplateGuid]
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
		d.[AlarmGuid]
	,	d.[InputTagGuid]
	,	d.[ID]
	,	d.[Enabled]
	,	d.[AlarmCategoryApplicationStringGuid]
	,	d.[Order]
	,	d.[NotAlarmState]
	,	d.[Comment]
	,	d.[ShelvedStartTimeStamp]
	,	d.[ShelvedEndTimeStamp]
	,	d.[ShelvedOneShot]
	,	d.[ShelvedBy]
	,	d.[Suppressed]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[AlarmStateTagGuid]
	,	d.[ExclusiveAlarm]
	,	d.[AlarmTemplateGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAlarm] ON [dbo].[tblAlarm] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarm','D')=1 
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
	INSERT INTO [fmaudit].tblAlarm (
		[AlarmGuid]
	,	[InputTagGuid]
	,	[ID]
	,	[Enabled]
	,	[AlarmCategoryApplicationStringGuid]
	,	[Order]
	,	[NotAlarmState]
	,	[Comment]
	,	[ShelvedStartTimeStamp]
	,	[ShelvedEndTimeStamp]
	,	[ShelvedOneShot]
	,	[ShelvedBy]
	,	[Suppressed]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AlarmStateTagGuid]
	,	[ExclusiveAlarm]
	,	[AlarmTemplateGuid]
	,	[Notify]
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
		i.[AlarmGuid]
	,	i.[InputTagGuid]
	,	i.[ID]
	,	i.[Enabled]
	,	i.[AlarmCategoryApplicationStringGuid]
	,	i.[Order]
	,	i.[NotAlarmState]
	,	i.[Comment]
	,	i.[ShelvedStartTimeStamp]
	,	i.[ShelvedEndTimeStamp]
	,	i.[ShelvedOneShot]
	,	i.[ShelvedBy]
	,	i.[Suppressed]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[AlarmStateTagGuid]
	,	i.[ExclusiveAlarm]
	,	i.[AlarmTemplateGuid]
	,	i.[Notify]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAlarm] ON [dbo].[tblAlarm] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarm','D')=1 
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
	AlarmGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAlarm (
		[AlarmGuid]
	,	[InputTagGuid]
	,	[ID]
	,	[Enabled]
	,	[AlarmCategoryApplicationStringGuid]
	,	[Order]
	,	[NotAlarmState]
	,	[Comment]
	,	[ShelvedStartTimeStamp]
	,	[ShelvedEndTimeStamp]
	,	[ShelvedOneShot]
	,	[ShelvedBy]
	,	[Suppressed]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AlarmStateTagGuid]
	,	[ExclusiveAlarm]
	,	[AlarmTemplateGuid]
	,	[Notify]
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
	OUTPUT inserted.[AlarmGuid] AS 'AlarmGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AlarmGuid]
	,	d.[InputTagGuid]
	,	d.[ID]
	,	d.[Enabled]
	,	d.[AlarmCategoryApplicationStringGuid]
	,	d.[Order]
	,	d.[NotAlarmState]
	,	d.[Comment]
	,	d.[ShelvedStartTimeStamp]
	,	d.[ShelvedEndTimeStamp]
	,	d.[ShelvedOneShot]
	,	d.[ShelvedBy]
	,	d.[Suppressed]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[AlarmStateTagGuid]
	,	d.[ExclusiveAlarm]
	,	d.[AlarmTemplateGuid]
	,	d.[Notify]
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
 
	INSERT INTO [fmaudit].tblAlarm (
		[AlarmGuid]
	,	[InputTagGuid]
	,	[ID]
	,	[Enabled]
	,	[AlarmCategoryApplicationStringGuid]
	,	[Order]
	,	[NotAlarmState]
	,	[Comment]
	,	[ShelvedStartTimeStamp]
	,	[ShelvedEndTimeStamp]
	,	[ShelvedOneShot]
	,	[ShelvedBy]
	,	[Suppressed]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AlarmStateTagGuid]
	,	[ExclusiveAlarm]
	,	[AlarmTemplateGuid]
	,	[Notify]
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
		i.[AlarmGuid]
	,	i.[InputTagGuid]
	,	i.[ID]
	,	i.[Enabled]
	,	i.[AlarmCategoryApplicationStringGuid]
	,	i.[Order]
	,	i.[NotAlarmState]
	,	i.[Comment]
	,	i.[ShelvedStartTimeStamp]
	,	i.[ShelvedEndTimeStamp]
	,	i.[ShelvedOneShot]
	,	i.[ShelvedBy]
	,	i.[Suppressed]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[AlarmStateTagGuid]
	,	i.[ExclusiveAlarm]
	,	i.[AlarmTemplateGuid]
	,	i.[Notify]
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
			agl.[AlarmGuid]=i.[AlarmGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
