CREATE TABLE [dbo].[tblAlarmTest]
(
	[AlarmTestGuid] [uniqueidentifier] CONSTRAINT [DF_tblAlarmTest_AlarmTestGuid] DEFAULT (newid()) NOT NULL,
	[AlarmGuid] [uniqueidentifier] NOT NULL,
	[ID] [nvarchar](256) NOT NULL,
	[LimitTagGuid] [uniqueidentifier] NOT NULL,
	[TagField] INT CONSTRAINT [DF_tblAlarmTest_TagField] DEFAULT (0) NOT NULL,
	[AlarmPriorityGuid] [UNIQUEIDENTIFIER] NOT NULL,
	[NormalUnacknowledgedAlarmPriorityGuid] [UNIQUEIDENTIFIER] NOT NULL,
	[TestType] [INT] NOT NULL,  -- this is an enum for the different comparison types. See slide 5
	[BitMask] BIGINT CONSTRAINT [DF_tblAlarmTest_BitMask] DEFAULT (0xFFFFFFFFFFFFFFFF) NOT NULL, 
	[Enabled] [BIT] CONSTRAINT [DF_tblAlarmTest_Enabled] DEFAULT (1) NOT NULL,
	[Order] [INT] CONSTRAINT [DF_tblAlarmTest_Order] DEFAULT (0) NOT NULL,
	[AlarmState] [NVARCHAR](100) CONSTRAINT [DF_tblAlarmTest_AlarmState] DEFAULT ('Alarm') NOT NULL,
	[Holdoff] [FLOAT] CONSTRAINT [DF_tblAlarmTest_Holdoff] DEFAULT (0.00) NOT NULL,  -- between 0 and 1 a percentage of the delta between the tag Max and Min.
	[AlarmText] [NVARCHAR](256) NULL,
	[HelpFile] [NVARCHAR](MAX) NULL, 	
	[DrawingGuid] [UNIQUEIDENTIFIER] NULL,
	[CreatedDate] [DATETIMEOFFSET](7) CONSTRAINT [DF_tblAlarmTest_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTest_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate] [DATETIMEOFFSET](7) CONSTRAINT [DF_tblAlarmTest_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAlarmTest_UpdatedBy] DEFAULT ('') NOT NULL,
	[_RowVersion] ROWVERSION NOT NULL,
	[BitwiseOperator] [INT] CONSTRAINT [DF_tblAlarmTest_BitwiseOperator] DEFAULT 0 NOT NULL,
	[TimedHoldOffInSeconds] [INT] CONSTRAINT [DF_tblAlarmTest_TimedHoldOffInSeconds] DEFAULT 0 NOT NULL,
	[AlarmTestTemplateGuid] [uniqueidentifier] NULL,
	CONSTRAINT [PK_tblAlarmTest_GUID] PRIMARY KEY NONCLUSTERED ([AlarmTestGuid] ASC),
	CONSTRAINT [FK_tblAlarmTest_LimitTagGuid] FOREIGN KEY([LimitTagGuid]) REFERENCES [dbo].[tblPointTag] ([PointTagGuid]),
	CONSTRAINT [FK_tblAlarmTest_AlarmPriorityGuid] FOREIGN KEY([AlarmPriorityGuid])REFERENCES [dbo].[tblAlarmPriorities] ([AlarmPriorityGuid]),
	CONSTRAINT [FK_tblAlarmTest_NormalUnacknowledgedAlarmPriorityGuid] FOREIGN KEY([NormalUnacknowledgedAlarmPriorityGuid])REFERENCES [dbo].[tblAlarmPriorities] ([AlarmPriorityGuid]),
	CONSTRAINT [FK_tblAlarmTest_AlarmGuid] FOREIGN KEY([AlarmGuid]) REFERENCES [dbo].[tblAlarm] ([AlarmGuid]),
	CONSTRAINT [FK_tblAlarmTest_DrawingGuid] FOREIGN KEY([DrawingGuid]) REFERENCES [dbo].[tblDrawings] ([DrawingGuid]),
	CONSTRAINT [FK_tblAlarmTest_AlarmTestTemplateGuid] FOREIGN KEY([AlarmTestTemplateGuid]) REFERENCES [dbo].[tblAlarmTestTemplate] ([AlarmTestTemplateGuid])
)
GO

ALTER TABLE [dbo].[tblAlarmTest] NOCHECK CONSTRAINT [FK_tblAlarmTest_AlarmTestTemplateGuid]
GO


CREATE NONCLUSTERED INDEX [IX_tblAlarmTest_AlarmGuid]
    ON [dbo].[tblAlarmTest]([AlarmGuid] ASC);
GO
--Creating Insert / Update Trigger for tblAlarmTest
CREATE TRIGGER dbo.trg_insupd_tblAlarmTest_ForSync 
   ON dbo.tblAlarmTest
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
                    ,d.AlarmTestGuid AS Deleted_PK_AlarmTestGuid
                    ,i.AlarmTestGuid AS Inserted_PK_AlarmTestGuid
                    ,d.AlarmGuid AS Deleted_FK_ParentPK
                    ,i.AlarmGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.AlarmTestGuid = i.AlarmTestGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAlarmTest As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AlarmTestGuid = currentTrackingData.PK_AlarmTestGuid
 
 
		    INSERT track.tblAlarmTest (InsertedDate 
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
				    ,PK_AlarmTestGuid
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
				    ,entityChanges.Inserted_PK_AlarmTestGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAlarmTest As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AlarmTestGuid = currentTrackingData.PK_AlarmTestGuid
)
    END
END 
GO
--Creating Delete Trigger for tblAlarmTest
CREATE TRIGGER dbo.trg_del_tblAlarmTest_ForSync 
   ON dbo.tblAlarmTest
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
						,d.AlarmTestGuid AS Deleted_PK_AlarmTestGuid
                        ,d.AlarmTestGuid AS Inserted_PK_AlarmTestGuid
                      ,d.AlarmGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAlarmTest As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AlarmTestGuid = currentTrackingData.PK_AlarmTestGuid
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
						,PK_AlarmTestGuid
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
						,entityChanges.Deleted_PK_AlarmTestGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAlarmTest] ON [dbo].[tblAlarmTest] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTest','D')=1 
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
	INSERT INTO [fmaudit].tblAlarmTest (
		[AlarmTestGuid]
	,	[AlarmGuid]
	,	[ID]
	,	[LimitTagGuid]
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
	,	[AlarmTestTemplateGuid]
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
		d.[AlarmTestGuid]
	,	d.[AlarmGuid]
	,	d.[ID]
	,	d.[LimitTagGuid]
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
	,	d.[AlarmTestTemplateGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAlarmTest] ON [dbo].[tblAlarmTest] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTest','D')=1 
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
	INSERT INTO [fmaudit].tblAlarmTest (
		[AlarmTestGuid]
	,	[AlarmGuid]
	,	[ID]
	,	[LimitTagGuid]
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
	,	[AlarmTestTemplateGuid]
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
		i.[AlarmTestGuid]
	,	i.[AlarmGuid]
	,	i.[ID]
	,	i.[LimitTagGuid]
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
	,	i.[AlarmTestTemplateGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAlarmTest] ON [dbo].[tblAlarmTest] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAlarmTest','D')=1 
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
	AlarmTestGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAlarmTest (
		[AlarmTestGuid]
	,	[AlarmGuid]
	,	[ID]
	,	[LimitTagGuid]
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
	,	[AlarmTestTemplateGuid]
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
	OUTPUT inserted.[AlarmTestGuid] AS 'AlarmTestGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AlarmTestGuid]
	,	d.[AlarmGuid]
	,	d.[ID]
	,	d.[LimitTagGuid]
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
	,	d.[AlarmTestTemplateGuid]
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
 
	INSERT INTO [fmaudit].tblAlarmTest (
		[AlarmTestGuid]
	,	[AlarmGuid]
	,	[ID]
	,	[LimitTagGuid]
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
	,	[AlarmTestTemplateGuid]
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
		i.[AlarmTestGuid]
	,	i.[AlarmGuid]
	,	i.[ID]
	,	i.[LimitTagGuid]
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
	,	i.[AlarmTestTemplateGuid]
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
			agl.[AlarmTestGuid]=i.[AlarmTestGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
