CREATE TABLE [dbo].[tblAlarmTemplate]
(
	[AlarmTemplateGuid] [uniqueidentifier] CONSTRAINT [DF_tblAlarmTemplate_AlarmTemplateGuid] DEFAULT (NEWID()) NOT NULL,
	[InputTemplateTagGuid] [UNIQUEIDENTIFIER] NOT NULL,
	[ID] [NVARCHAR](256) NOT NULL,
	[Enabled] [BIT] CONSTRAINT [DF_tblAlarmTemplate_Enabled] DEFAULT (1) NOT NULL,
	[AlarmCategoryApplicationStringGuid] [UNIQUEIDENTIFIER] NOT NULL,
	[Order] [INT] CONSTRAINT [DF_tblAlarmTemplate_Order] DEFAULT (0) NOT NULL,
	[NotAlarmState] [NVARCHAR](100) CONSTRAINT [DF_tblAlarmTemplate_NotAlarmState] DEFAULT ('Normal') NOT NULL,
	[Comment] [NVARCHAR](256) NULL,
	[ShelvedStartTimeStamp] [DATETIMEOFFSET](7),
	[ShelvedEndTimeStamp] [DATETIMEOFFSET](7),
	[ShelvedOneShot] [BIT] CONSTRAINT [DF_tblAlarmTemplate_ShelvedOneShot] DEFAULT (0) NOT NULL,
	[ShelvedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTemplate_ShelvedBy] DEFAULT ('') NULL,
	[Suppressed] [BIT] CONSTRAINT [DF_tblAlarmTemplate_Suppressed] DEFAULT (0) NOT NULL,
	[CreatedDate] [DATETIMEOFFSET](7) CONSTRAINT [DF_tblAlarmTemplate_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTemplate_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate] [DATETIMEOFFSET](7) CONSTRAINT [DF_tblAlarmTemplate_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTemplate_UpdatedBy] DEFAULT ('') NOT NULL,
	[_RowVersion] ROWVERSION NOT NULL,
	[AlarmStateTemplateTagGuid] UNIQUEIDENTIFIER NOT NULL,
	[ExclusiveAlarm] BIT CONSTRAINT [DF_tblAlarmTemplate_ExclusiveAlarm] DEFAULT 1 NOT NULL,
	CONSTRAINT [PK_tblAlarmTemplate_GUID] PRIMARY KEY NONCLUSTERED ([AlarmTemplateGuid] ASC),
	CONSTRAINT [FK_tblAlarmTemplate_InputTemplateTagGuid] FOREIGN KEY ([InputTemplateTagGuid]) REFERENCES [dbo].[tblPointTemplateTag] ([PointTemplateTagGuid]),
	CONSTRAINT [FK_tblAlarmTemplate_AlarmStateTemplateTagGuid] FOREIGN KEY ([AlarmStateTemplateTagGuid]) REFERENCES [dbo].[tblPointTemplateTag] ([PointTemplateTagGuid]),
	CONSTRAINT [FK_tblAlarmTemplate_AlarmCategoryApplicationStringGuid] FOREIGN KEY([AlarmCategoryApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid])
)

GO

CREATE NONCLUSTERED INDEX [IX_tblAlarmTemplate_InputTemplateTagGuid]
	ON [dbo].[tblAlarmTemplate]([InputTemplateTagGuid] ASC)
GO
--Creating Insert / Update Trigger for tblAlarmTemplate
CREATE TRIGGER dbo.trg_insupd_tblAlarmTemplate_ForSync 
   ON dbo.tblAlarmTemplate
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
                    ,d.AlarmTemplateGuid AS Deleted_PK_AlarmTemplateGuid
                    ,i.AlarmTemplateGuid AS Inserted_PK_AlarmTemplateGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.AlarmTemplateGuid = i.AlarmTemplateGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAlarmTemplate As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AlarmTemplateGuid = currentTrackingData.PK_AlarmTemplateGuid
 
 
		    INSERT track.tblAlarmTemplate (InsertedDate 
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
				    ,PK_AlarmTemplateGuid
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
				    ,entityChanges.Inserted_PK_AlarmTemplateGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAlarmTemplate As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AlarmTemplateGuid = currentTrackingData.PK_AlarmTemplateGuid
)
    END
END 
GO
--Creating Delete Trigger for tblAlarmTemplate
CREATE TRIGGER dbo.trg_del_tblAlarmTemplate_ForSync 
   ON dbo.tblAlarmTemplate
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
						,d.AlarmTemplateGuid AS Deleted_PK_AlarmTemplateGuid
                        ,d.AlarmTemplateGuid AS Inserted_PK_AlarmTemplateGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAlarmTemplate As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AlarmTemplateGuid = currentTrackingData.PK_AlarmTemplateGuid
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
						,PK_AlarmTemplateGuid
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
						,entityChanges.Deleted_PK_AlarmTemplateGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAlarmTemplate] ON [dbo].[tblAlarmTemplate] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTemplate','D')=1 
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
	INSERT INTO [fmaudit].tblAlarmTemplate (
		[AlarmTemplateGuid]
	,	[InputTemplateTagGuid]
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
	,	[AlarmStateTemplateTagGuid]
	,	[ExclusiveAlarm]
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
		d.[AlarmTemplateGuid]
	,	d.[InputTemplateTagGuid]
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
	,	d.[AlarmStateTemplateTagGuid]
	,	d.[ExclusiveAlarm]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAlarmTemplate] ON [dbo].[tblAlarmTemplate] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTemplate','D')=1 
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
	INSERT INTO [fmaudit].tblAlarmTemplate (
		[AlarmTemplateGuid]
	,	[InputTemplateTagGuid]
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
	,	[AlarmStateTemplateTagGuid]
	,	[ExclusiveAlarm]
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
		i.[AlarmTemplateGuid]
	,	i.[InputTemplateTagGuid]
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
	,	i.[AlarmStateTemplateTagGuid]
	,	i.[ExclusiveAlarm]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAlarmTemplate] ON [dbo].[tblAlarmTemplate] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTemplate','D')=1 
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
	AlarmTemplateGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAlarmTemplate (
		[AlarmTemplateGuid]
	,	[InputTemplateTagGuid]
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
	,	[AlarmStateTemplateTagGuid]
	,	[ExclusiveAlarm]
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
	OUTPUT inserted.[AlarmTemplateGuid] AS 'AlarmTemplateGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AlarmTemplateGuid]
	,	d.[InputTemplateTagGuid]
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
	,	d.[AlarmStateTemplateTagGuid]
	,	d.[ExclusiveAlarm]
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
 
	INSERT INTO [fmaudit].tblAlarmTemplate (
		[AlarmTemplateGuid]
	,	[InputTemplateTagGuid]
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
	,	[AlarmStateTemplateTagGuid]
	,	[ExclusiveAlarm]
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
		i.[AlarmTemplateGuid]
	,	i.[InputTemplateTagGuid]
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
	,	i.[AlarmStateTemplateTagGuid]
	,	i.[ExclusiveAlarm]
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
			agl.[AlarmTemplateGuid]=i.[AlarmTemplateGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO

