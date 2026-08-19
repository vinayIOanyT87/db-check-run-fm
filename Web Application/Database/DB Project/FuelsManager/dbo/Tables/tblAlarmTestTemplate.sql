CREATE TABLE [dbo].[tblAlarmTestTemplate]
(
	[AlarmTestTemplateGuid] [uniqueidentifier] CONSTRAINT [DF_tblAlarmTestTemplate_AlarmTestTemplateGuid] DEFAULT (newid()) NOT NULL,
	[AlarmTemplateGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[LimitTemplateTagGuid] [uniqueidentifier] NOT NULL,
	[TagField] INT CONSTRAINT [DF_tblAlarmTestTemplate_TagField] DEFAULT (0) NOT NULL,
	[AlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[NormalUnacknowledgedAlarmPriorityGuid] [uniqueidentifier] NOT NULL,
	[TestType] [int] NOT NULL,  -- this is an enum for the different comparison types. See slide 5
	[BitMask] BIGINT CONSTRAINT [DF_tblAlarmTestTemplate_BitMask] DEFAULT (0xFFFFFFFFFFFFFFFF) NOT NULL,
	[Enabled] [Bit] CONSTRAINT [DF_tblAlarmTestTemplate_Enabled] DEFAULT (1) NOT NULL,
	[Order] [int] CONSTRAINT [DF_tblAlarmTestTemplate_Order] DEFAULT (0) NOT NULL,
	[AlarmState] [nvarchar](100) CONSTRAINT [DF_tblAlarmTestTemplate_AlarmState] DEFAULT ('Alarm') NOT NULL,
	[Holdoff] [float] CONSTRAINT [DF_tblAlarmTestTemplate_Holdoff] DEFAULT (0.00) NOT NULL,  -- between 0 and 1 a percentage of the delta between the tag Max and Min.
	[AlarmText] [nvarchar](256) NULL,
	[HelpFile] [nvarchar](Max) NULL, 	
	[DrawingGuid] [uniqueidentifier] NULL,	
	[CreatedDate] [datetimeoffset](7) CONSTRAINT [DF_tblAlarmTestTemplate_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTestTemplate_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate] [datetimeoffset](7) CONSTRAINT [DF_tblAlarmTestTemplate_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTestTemplate_UpdatedBy] DEFAULT ('') NOT NULL,
	[_RowVersion] ROWVERSION NOT NULL,
	[BitwiseOperator] [int] CONSTRAINT [DF_tblAlarmTestTemplate_BitwiseOperator] DEFAULT 0 NOT NULL,
	[TimedHoldOffInSeconds] [int] CONSTRAINT [DF_tblAlarmTestTemplate_TimedHoldOffInSeconds] DEFAULT 0 NOT NULL,
	CONSTRAINT [PK_tblAlarmTestTemplate_GUID] PRIMARY KEY NONCLUSTERED ([AlarmTestTemplateGuid] ASC),
	CONSTRAINT [FK_tblAlarmTestTemplate_LimitTemplateTagGuid] FOREIGN KEY([LimitTemplateTagGuid]) REFERENCES [dbo].[tblPointTemplateTag] ([PointTemplateTagGuid]),
	CONSTRAINT [FK_tblAlarmTestTemplate_AlarmPriorityGuid] FOREIGN KEY([AlarmPriorityGuid])REFERENCES [dbo].[tblAlarmPriorities] ([AlarmPriorityGuid]),
	CONSTRAINT [FK_tblAlarmTestTemplate_NormalUnacknowledgedAlarmPriorityGuid] FOREIGN KEY([NormalUnacknowledgedAlarmPriorityGuid])REFERENCES [dbo].[tblAlarmPriorities] ([AlarmPriorityGuid]),
	CONSTRAINT [FK_tblAlarmTestTemplate_AlarmTemplateGuid] FOREIGN KEY([AlarmTemplateGuid]) REFERENCES [dbo].[tblAlarmTemplate] ([AlarmTemplateGuid]),
	CONSTRAINT [FK_tblAlarmTestTemplate_DrawingGuid] FOREIGN KEY([DrawingGuid]) REFERENCES [dbo].[tblDrawings] ([DrawingGuid])
)

GO

CREATE NONCLUSTERED INDEX [IX_tblAlarmTestTemplate_AlarmTemplateGuid]
	ON [dbo].[tblAlarmTestTemplate]([AlarmTemplateGuid] ASC)
GO
--Creating Insert / Update Trigger for tblAlarmTestTemplate
CREATE TRIGGER dbo.trg_insupd_tblAlarmTestTemplate_ForSync 
   ON dbo.tblAlarmTestTemplate
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
                    ,d.AlarmTestTemplateGuid AS Deleted_PK_AlarmTestTemplateGuid
                    ,i.AlarmTestTemplateGuid AS Inserted_PK_AlarmTestTemplateGuid
                    ,d.AlarmTemplateGuid AS Deleted_FK_ParentPK
                    ,i.AlarmTemplateGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.AlarmTestTemplateGuid = i.AlarmTestTemplateGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAlarmTestTemplate As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AlarmTestTemplateGuid = currentTrackingData.PK_AlarmTestTemplateGuid
 
 
		    INSERT track.tblAlarmTestTemplate (InsertedDate 
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
				    ,PK_AlarmTestTemplateGuid
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
				    ,entityChanges.Inserted_PK_AlarmTestTemplateGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAlarmTestTemplate As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AlarmTestTemplateGuid = currentTrackingData.PK_AlarmTestTemplateGuid
)
    END
END 
GO
--Creating Delete Trigger for tblAlarmTestTemplate
CREATE TRIGGER dbo.trg_del_tblAlarmTestTemplate_ForSync 
   ON dbo.tblAlarmTestTemplate
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
						,d.AlarmTestTemplateGuid AS Deleted_PK_AlarmTestTemplateGuid
                        ,d.AlarmTestTemplateGuid AS Inserted_PK_AlarmTestTemplateGuid
                      ,d.AlarmTemplateGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAlarmTestTemplate As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AlarmTestTemplateGuid = currentTrackingData.PK_AlarmTestTemplateGuid
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
						,PK_AlarmTestTemplateGuid
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
						,entityChanges.Deleted_PK_AlarmTestTemplateGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAlarmTestTemplate] ON [dbo].[tblAlarmTestTemplate] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTestTemplate','D')=1 
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
	INSERT INTO [fmaudit].tblAlarmTestTemplate (
		[AlarmTestTemplateGuid]
	,	[AlarmTemplateGuid]
	,	[ID]
	,	[LimitTemplateTagGuid]
	,	[TagField]
	,	[AlarmPriorityGuid]
	,	[NormalUnacknowledgedAlarmPriorityGuid]
	,	[TestType]
	,	[BitMask]
	,	[Enabled]
	,	[Order]
	,	[AlarmState]
	,	[Holdoff]
	,	[AlarmText]
	,	[HelpFile]
	,	[DrawingGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[BitwiseOperator]
	,	[TimedHoldOffInSeconds]
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
		d.[AlarmTestTemplateGuid]
	,	d.[AlarmTemplateGuid]
	,	d.[ID]
	,	d.[LimitTemplateTagGuid]
	,	d.[TagField]
	,	d.[AlarmPriorityGuid]
	,	d.[NormalUnacknowledgedAlarmPriorityGuid]
	,	d.[TestType]
	,	d.[BitMask]
	,	d.[Enabled]
	,	d.[Order]
	,	d.[AlarmState]
	,	d.[Holdoff]
	,	d.[AlarmText]
	,	d.[HelpFile]
	,	d.[DrawingGuid]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[BitwiseOperator]
	,	d.[TimedHoldOffInSeconds]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAlarmTestTemplate] ON [dbo].[tblAlarmTestTemplate] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTestTemplate','D')=1 
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
	INSERT INTO [fmaudit].tblAlarmTestTemplate (
		[AlarmTestTemplateGuid]
	,	[AlarmTemplateGuid]
	,	[ID]
	,	[LimitTemplateTagGuid]
	,	[TagField]
	,	[AlarmPriorityGuid]
	,	[NormalUnacknowledgedAlarmPriorityGuid]
	,	[TestType]
	,	[BitMask]
	,	[Enabled]
	,	[Order]
	,	[AlarmState]
	,	[Holdoff]
	,	[AlarmText]
	,	[HelpFile]
	,	[DrawingGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[BitwiseOperator]
	,	[TimedHoldOffInSeconds]
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
		i.[AlarmTestTemplateGuid]
	,	i.[AlarmTemplateGuid]
	,	i.[ID]
	,	i.[LimitTemplateTagGuid]
	,	i.[TagField]
	,	i.[AlarmPriorityGuid]
	,	i.[NormalUnacknowledgedAlarmPriorityGuid]
	,	i.[TestType]
	,	i.[BitMask]
	,	i.[Enabled]
	,	i.[Order]
	,	i.[AlarmState]
	,	i.[Holdoff]
	,	i.[AlarmText]
	,	i.[HelpFile]
	,	i.[DrawingGuid]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[BitwiseOperator]
	,	i.[TimedHoldOffInSeconds]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAlarmTestTemplate] ON [dbo].[tblAlarmTestTemplate] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTestTemplate','D')=1 
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
	AlarmTestTemplateGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAlarmTestTemplate (
		[AlarmTestTemplateGuid]
	,	[AlarmTemplateGuid]
	,	[ID]
	,	[LimitTemplateTagGuid]
	,	[TagField]
	,	[AlarmPriorityGuid]
	,	[NormalUnacknowledgedAlarmPriorityGuid]
	,	[TestType]
	,	[BitMask]
	,	[Enabled]
	,	[Order]
	,	[AlarmState]
	,	[Holdoff]
	,	[AlarmText]
	,	[HelpFile]
	,	[DrawingGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[BitwiseOperator]
	,	[TimedHoldOffInSeconds]
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
	OUTPUT inserted.[AlarmTestTemplateGuid] AS 'AlarmTestTemplateGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AlarmTestTemplateGuid]
	,	d.[AlarmTemplateGuid]
	,	d.[ID]
	,	d.[LimitTemplateTagGuid]
	,	d.[TagField]
	,	d.[AlarmPriorityGuid]
	,	d.[NormalUnacknowledgedAlarmPriorityGuid]
	,	d.[TestType]
	,	d.[BitMask]
	,	d.[Enabled]
	,	d.[Order]
	,	d.[AlarmState]
	,	d.[Holdoff]
	,	d.[AlarmText]
	,	d.[HelpFile]
	,	d.[DrawingGuid]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[BitwiseOperator]
	,	d.[TimedHoldOffInSeconds]
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
 
	INSERT INTO [fmaudit].tblAlarmTestTemplate (
		[AlarmTestTemplateGuid]
	,	[AlarmTemplateGuid]
	,	[ID]
	,	[LimitTemplateTagGuid]
	,	[TagField]
	,	[AlarmPriorityGuid]
	,	[NormalUnacknowledgedAlarmPriorityGuid]
	,	[TestType]
	,	[BitMask]
	,	[Enabled]
	,	[Order]
	,	[AlarmState]
	,	[Holdoff]
	,	[AlarmText]
	,	[HelpFile]
	,	[DrawingGuid]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[BitwiseOperator]
	,	[TimedHoldOffInSeconds]
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
		i.[AlarmTestTemplateGuid]
	,	i.[AlarmTemplateGuid]
	,	i.[ID]
	,	i.[LimitTemplateTagGuid]
	,	i.[TagField]
	,	i.[AlarmPriorityGuid]
	,	i.[NormalUnacknowledgedAlarmPriorityGuid]
	,	i.[TestType]
	,	i.[BitMask]
	,	i.[Enabled]
	,	i.[Order]
	,	i.[AlarmState]
	,	i.[Holdoff]
	,	i.[AlarmText]
	,	i.[HelpFile]
	,	i.[DrawingGuid]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[BitwiseOperator]
	,	i.[TimedHoldOffInSeconds]
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
			agl.[AlarmTestTemplateGuid]=i.[AlarmTestTemplateGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO

