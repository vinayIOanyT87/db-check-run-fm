CREATE TABLE [map].[tblEmailTemplateToAlarmAndEvent]
(
	EmailTemplateToAlarmAndEventGuid UNIQUEIDENTIFIER NOT NULL,
	EmailTemplateGuid UNIQUEIDENTIFIER  NOT NULL,
	AlarmAndEventGuid UNIQUEIDENTIFIER NOT NULL,
	[CreatedDate] [datetimeoffset](7) NULL,
	[CreatedBy] [dbo].[udtUserID] NULL,
	[UpdatedDate] [datetimeoffset](7) NULL,
	[UpdatedBy] [dbo].[udtUserID] NULL,
	[_RowVersion] [timestamp] NOT NULL,
	[_ClusterIdx] [bigint] IDENTITY(1,1) NOT NULL,
	CONSTRAINT [FK_map_tblEmailTemplateToAlarmAndEvent_AlarmAndEventGuid] FOREIGN KEY([AlarmAndEventGuid])
	REFERENCES [dbo].[tblAlarmAndEvents] ([AlarmAndEventGuid]), 
	CONSTRAINT [FK_map_tblEmailTemplateToEmailTemplate_EmailTemplateGuid] FOREIGN KEY([EmailTemplateGuid])
	REFERENCES [dbo].[tblEmailTemplate] ([EmailTemplateGuid]),
	CONSTRAINT [PK_map_tblEmailTemplateToAlarmAndEvent_GUID] PRIMARY KEY NONCLUSTERED 
	(
		[EmailTemplateToAlarmAndEventGuid] ASC
	)
	WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]
GO

-------------------------------------
-- AUDIT INSERT TRIGGERS
-------------------------------------
 
CREATE TRIGGER [map].[trg_Audit_ins_tblEmailTemplateToAlarmAndEvent] ON [map].[tblEmailTemplateToAlarmAndEvent] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblEmailTemplateToAlarmAndEvent','D')=1 
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
	INSERT INTO [fmaudit].map_tblEmailTemplateToAlarmAndEvent (
		[EmailTemplateToAlarmAndEventGuid]
	,	[EmailTemplateGuid]
	,	[AlarmAndEventGuid]
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
		i.[EmailTemplateToAlarmAndEventGuid]
	,	i.[EmailTemplateGuid]
	,	i.[AlarmAndEventGuid]
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
 
-------------------------------------
-- AUDIT UPDATE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [map].[trg_Audit_upd_tblEmailTemplateToAlarmAndEvent] ON [map].[tblEmailTemplateToAlarmAndEvent] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblEmailTemplateToAlarmAndEvent','D')=1 
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
	EmailTemplateToAlarmAndEventGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblEmailTemplateToAlarmAndEvent (
		[EmailTemplateToAlarmAndEventGuid]
	,	[EmailTemplateGuid]
	,	[AlarmAndEventGuid]
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
	OUTPUT inserted.[EmailTemplateToAlarmAndEventGuid] AS 'EmailTemplateToAlarmAndEventGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[EmailTemplateToAlarmAndEventGuid]
	,	d.[EmailTemplateGuid]
	,	d.[AlarmAndEventGuid]
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
 
	INSERT INTO [fmaudit].map_tblEmailTemplateToAlarmAndEvent (
		[EmailTemplateToAlarmAndEventGuid]
	,	[EmailTemplateGuid]
	,	[AlarmAndEventGuid]
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
		i.[EmailTemplateToAlarmAndEventGuid]
	,	i.[EmailTemplateGuid]
	,	i.[AlarmAndEventGuid]
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
			agl.[EmailTemplateToAlarmAndEventGuid]=i.[EmailTemplateToAlarmAndEventGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO

CREATE TRIGGER [map].[trg_Audit_del_tblEmailTemplateToAlarmAndEvent] ON [map].[tblEmailTemplateToAlarmAndEvent] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblEmailTemplateToAlarmAndEvent','D')=1 
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
	INSERT INTO [fmaudit].map_tblEmailTemplateToAlarmAndEvent (
		[EmailTemplateToAlarmAndEventGuid]
	,	[EmailTemplateGuid]
	,	[AlarmAndEventGuid]
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
		d.[EmailTemplateToAlarmAndEventGuid]
	,	d.[EmailTemplateGuid]
	,	d.[AlarmAndEventGuid]
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
--Creating Insert / Update Trigger for tblEmailTemplateToAlarmAndEvent
CREATE TRIGGER map.trg_insupd_tblEmailTemplateToAlarmAndEvent_ForSync 
   ON map.tblEmailTemplateToAlarmAndEvent
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
 
       ; WITH ChangeList AS ( 
       SELECT @syncContext AS ChangeContext 
                   ,d.EmailTemplateToAlarmAndEventGuid AS Deleted_PK_EmailTemplateToAlarmAndEventGuid
                    ,i.EmailTemplateToAlarmAndEventGuid AS Inserted_PK_EmailTemplateToAlarmAndEventGuid
                   ,i.CreatedDate AS Inserted_CreatedDate 
                   ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,NULL AS CurrentSiteGuid 
				    ,NULL AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,NULL AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.EmailTemplateToAlarmAndEventGuid = i.EmailTemplateToAlarmAndEventGuid
           ) 
		    MERGE INTO track.tblEmailTemplateToAlarmAndEvent As ct 
			    USING ChangeList As src 
				    ON src.Inserted_PK_EmailTemplateToAlarmAndEventGuid = ct.PK_EmailTemplateToAlarmAndEventGuid
           WHEN Matched AND ((src.CurrentSiteGuid IS NULL AND ct.CurrentSiteGuid IS NULL)
                        OR ((src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NOT NULL) AND (src.CurrentSiteGuid = ct.CurrentSiteGuid))) 
		    THEN 
		    UPDATE SET UpdatedDate = src.Inserted_UpdatedDate 
			    		,UpdatedContext = src.ChangeContext 
 				        ,UpdatedRowVersion = src.Inserted_RowVersion 
     					,CurrentSiteGuid = src.CurrentSiteGuid 
 	    				,PreviousSiteGuid = ct.PreviousSiteGuid 
		    WHEN Not Matched 
		    THEN 
		    INSERT (InsertedDate 
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
				    ,PK_EmailTemplateToAlarmAndEventGuid
		    )
		    VALUES (CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
		                 WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate 
		                 ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
			    	,src.ChangeContext 
				    ,src.Inserted_RowVersion 
    				,src.Inserted_CreatedDate 
	    			,src.ChangeContext 
		    		,src.Inserted_RowVersion 
			    	,NULL 
    				,NULL 
	    			,NULL 
		    		,src.CurrentSiteGuid 
			    	,CASE WHEN (((src.PreviousSiteGuid IS NOT NULL AND src.CurrentSiteGuid IS NOT NULL) AND (src.PreviousSiteGuid <> src.CurrentSiteGuid))
 				    				OR (src.PreviousSiteGuid IS NULL AND src.CurrentSiteGuid IS NOT NULL)
 					    			OR (src.PreviousSiteGuid IS NOT NULL AND src.CurrentSiteGuid IS NULL)) THEN src.PreviousSiteGuid ELSE NULL END
				    ,src.Inserted_PK_EmailTemplateToAlarmAndEventGuid
		    );
    END
END 
GO
--Creating Delete Trigger for tblEmailTemplateToAlarmAndEvent
CREATE TRIGGER map.trg_del_tblEmailTemplateToAlarmAndEvent_ForSync 
   ON map.tblEmailTemplateToAlarmAndEvent
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
						,d.EmailTemplateToAlarmAndEventGuid AS Deleted_PK_EmailTemplateToAlarmAndEventGuid
                        ,d.EmailTemplateToAlarmAndEventGuid AS Inserted_PK_EmailTemplateToAlarmAndEventGuid
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblEmailTemplateToAlarmAndEvent As ct 
					USING ChangeList As src 
						ON src.Deleted_PK_EmailTemplateToAlarmAndEventGuid = ct.PK_EmailTemplateToAlarmAndEventGuid
				WHEN Matched 
				THEN 
					UPDATE SET DeletedDate = @currentDateTimeOffset 
								,DeletedContext = src.ChangeContext 
 								,DeletedRowVersion = src.Deleted_RowVersion 
 								,CurrentSiteGuid = src.CurrentSiteGuid 
 								,PreviousSiteGuid = CASE WHEN (((src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NOT NULL) AND (src.CurrentSiteGuid <> ct.CurrentSiteGuid))
 																OR (src.CurrentSiteGuid IS NULL AND ct.CurrentSiteGuid IS NOT NULL)
 																OR (src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NULL)) THEN ct.CurrentSiteGuid ELSE ct.PreviousSiteGuid END
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
						,PK_EmailTemplateToAlarmAndEventGuid
				)
				VALUES (CASE WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
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
						,src.Deleted_PK_EmailTemplateToAlarmAndEventGuid
				);
    END
END 
GO
 
