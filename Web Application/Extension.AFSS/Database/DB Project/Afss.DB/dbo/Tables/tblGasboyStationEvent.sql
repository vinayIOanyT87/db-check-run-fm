CREATE TABLE [dbo].[tblGasboyStationEvent]
(
	[GasboyStationEventGuid] UNIQUEIDENTIFIER NOT NULL, 
    [ExternalStationLogGuid] UNIQUEIDENTIFIER NOT NULL, 
	[EventID] INT NULL,
    [LookupGasboyEventErrorClassCodeIndex] INT NULL, 
	[ErrorCode] INT NULL, 
    [FleetID] INT NULL, 
    [ObjectID] INT NULL, 
	[LookupGasboyEventObjectTypeIndex] INT NULL,
	[DeviceName] NVARCHAR(100) NULL,
	[Field1] NVARCHAR(100) NULL,
	[Field2] NVARCHAR(100) NULL,
	[Field3] NVARCHAR(100) NULL,
	[Field4] NVARCHAR(100) NULL,
	[Field5] NVARCHAR(100) NULL,
	[Field6] NVARCHAR(100) NULL,
	[Field7] NVARCHAR(100) NULL,
	[Field8] NVARCHAR(100) NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL, 
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET NOT NULL, 
    [_RowVersion] TIMESTAMP NOT NULL,
    [_ClusterIdx] BIGINT IDENTITY(1,1) NOT NULL,
	CONSTRAINT [PK_tblGasboyStationEvent] PRIMARY KEY NONCLUSTERED ([GasboyStationEventGuid]), 
	CONSTRAINT [FK_tblGasboyStationEvent_ExternalStationLogGuid] FOREIGN KEY (ExternalStationLogGuid) REFERENCES [dbo].[tblExternalStationLog]([ExternalStationLogGuid]), 
	CONSTRAINT [FK_tblGasboyStationEvent_LookupGasboyEventErrorClassCodeIndex] FOREIGN KEY (LookupGasboyEventErrorClassCodeIndex) REFERENCES [lookup].[tblGasboyEventErrorClassCode]([GasboyEventErrorClassCodeIndex]), 
	CONSTRAINT [FK_tblGasboyStationEvent_LookupGasboyEventObjectTypeIndex] FOREIGN KEY (LookupGasboyEventObjectTypeIndex) REFERENCES [lookup].[tblGasboyEventObjectType]([GasboyEventObjectTypeIndex]) ,
	CONSTRAINT [FK_tblGasboyStationEvent_LookupGasboyErrorCodeIndex] FOREIGN KEY (ErrorCode) REFERENCES [lookup].[tblGasboyErrorCode]([GasboyErrorCodeIndex]) 
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblGasboyStationEvent__ClusterIdx] ON [dbo].[tblGasboyStationEvent] ([_ClusterIdx])
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyStationEvent_CreatedDate] ON [dbo].[tblGasboyStationEvent] (CreatedDate)
GO

CREATE UNIQUE INDEX [UIX_tblGasboyStationEvent_ExternalStationLogGuid] ON [dbo].[tblGasboyStationEvent] ([ExternalStationLogGuid])
GO

CREATE INDEX [IX_tblGasboyStationEvent_LookupGasboyEventErrorClassCodeIndex] ON [dbo].[tblGasboyStationEvent] ([LookupGasboyEventErrorClassCodeIndex])
GO

CREATE INDEX [IX_tblGasboyStationEvent_LookupGasboyEventObjectTypeIndex] ON [dbo].[tblGasboyStationEvent] ([LookupGasboyEventObjectTypeIndex])
GO
-------------------------------------
-- AUDIT DELETE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_del_tblGasboyStationEvent] ON [dbo].[tblGasboyStationEvent] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyStationEvent','D')=1 
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

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblGasboyStationEvent (
		[GasboyStationEventGuid]
	,	[ExternalStationLogGuid]
	,	[EventID]
	,	[LookupGasboyEventErrorClassCodeIndex]
	,	[ErrorCode]
	,	[FleetID]
	,	[ObjectID]
	,	[LookupGasboyEventObjectTypeIndex]
	,	[DeviceName]
	,	[Field1]
	,	[Field2]
	,	[Field3]
	,	[Field4]
	,	[Field5]
	,	[Field6]
	,	[Field7]
	,	[Field8]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
		d.[GasboyStationEventGuid]
	,	d.[ExternalStationLogGuid]
	,	d.[EventID]
	,	d.[LookupGasboyEventErrorClassCodeIndex]
	,	d.[ErrorCode]
	,	d.[FleetID]
	,	d.[ObjectID]
	,	d.[LookupGasboyEventObjectTypeIndex]
	,	d.[DeviceName]
	,	d.[Field1]
	,	d.[Field2]
	,	d.[Field3]
	,	d.[Field4]
	,	d.[Field5]
	,	d.[Field6]
	,	d.[Field7]
	,	d.[Field8]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
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
 
-------------------------------------
-- AUDIT INSERT TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_ins_tblGasboyStationEvent] ON [dbo].[tblGasboyStationEvent] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyStationEvent','D')=1 
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

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblGasboyStationEvent (
		[GasboyStationEventGuid]
	,	[ExternalStationLogGuid]
	,	[EventID]
	,	[LookupGasboyEventErrorClassCodeIndex]
	,	[ErrorCode]
	,	[FleetID]
	,	[ObjectID]
	,	[LookupGasboyEventObjectTypeIndex]
	,	[DeviceName]
	,	[Field1]
	,	[Field2]
	,	[Field3]
	,	[Field4]
	,	[Field5]
	,	[Field6]
	,	[Field7]
	,	[Field8]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
		i.[GasboyStationEventGuid]
	,	i.[ExternalStationLogGuid]
	,	i.[EventID]
	,	i.[LookupGasboyEventErrorClassCodeIndex]
	,	i.[ErrorCode]
	,	i.[FleetID]
	,	i.[ObjectID]
	,	i.[LookupGasboyEventObjectTypeIndex]
	,	i.[DeviceName]
	,	i.[Field1]
	,	i.[Field2]
	,	i.[Field3]
	,	i.[Field4]
	,	i.[Field5]
	,	i.[Field6]
	,	i.[Field7]
	,	i.[Field8]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
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
 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblGasboyStationEvent] ON [dbo].[tblGasboyStationEvent] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyStationEvent','D')=1 
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

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
 
	DECLARE @AuditGuidList TABLE
	(
	GasboyStationEventGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblGasboyStationEvent (
		[GasboyStationEventGuid]
	,	[ExternalStationLogGuid]
	,	[EventID]
	,	[LookupGasboyEventErrorClassCodeIndex]
	,	[ErrorCode]
	,	[FleetID]
	,	[ObjectID]
	,	[LookupGasboyEventObjectTypeIndex]
	,	[DeviceName]
	,	[Field1]
	,	[Field2]
	,	[Field3]
	,	[Field4]
	,	[Field5]
	,	[Field6]
	,	[Field7]
	,	[Field8]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
	OUTPUT inserted.[GasboyStationEventGuid] AS 'GasboyStationEventGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[GasboyStationEventGuid]
	,	d.[ExternalStationLogGuid]
	,	d.[EventID]
	,	d.[LookupGasboyEventErrorClassCodeIndex]
	,	d.[ErrorCode]
	,	d.[FleetID]
	,	d.[ObjectID]
	,	d.[LookupGasboyEventObjectTypeIndex]
	,	d.[DeviceName]
	,	d.[Field1]
	,	d.[Field2]
	,	d.[Field3]
	,	d.[Field4]
	,	d.[Field5]
	,	d.[Field6]
	,	d.[Field7]
	,	d.[Field8]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
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
 
	INSERT INTO [fmaudit].tblGasboyStationEvent (
		[GasboyStationEventGuid]
	,	[ExternalStationLogGuid]
	,	[EventID]
	,	[LookupGasboyEventErrorClassCodeIndex]
	,	[ErrorCode]
	,	[FleetID]
	,	[ObjectID]
	,	[LookupGasboyEventObjectTypeIndex]
	,	[DeviceName]
	,	[Field1]
	,	[Field2]
	,	[Field3]
	,	[Field4]
	,	[Field5]
	,	[Field6]
	,	[Field7]
	,	[Field8]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
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
		i.[GasboyStationEventGuid]
	,	i.[ExternalStationLogGuid]
	,	i.[EventID]
	,	i.[LookupGasboyEventErrorClassCodeIndex]
	,	i.[ErrorCode]
	,	i.[FleetID]
	,	i.[ObjectID]
	,	i.[LookupGasboyEventObjectTypeIndex]
	,	i.[DeviceName]
	,	i.[Field1]
	,	i.[Field2]
	,	i.[Field3]
	,	i.[Field4]
	,	i.[Field5]
	,	i.[Field6]
	,	i.[Field7]
	,	i.[Field8]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
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
			agl.[GasboyStationEventGuid]=i.[GasboyStationEventGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO

--Creating Insert / Update Trigger for tblGasboyStationEvent
CREATE TRIGGER dbo.trg_insupd_tblGasboyStationEvent_ForSync 
   ON dbo.tblGasboyStationEvent
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
                   ,d.GasboyStationEventGuid AS Deleted_PK_GasboyStationEventGuid
                    ,i.GasboyStationEventGuid AS Inserted_PK_GasboyStationEventGuid
                    ,NULL AS Deleted_FK_ParentPK 
                    ,NULL AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,NULL AS CurrentSiteGuid 
				    ,NULL AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,NULL AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.GasboyStationEventGuid = i.GasboyStationEventGuid
           ) 
		    MERGE INTO track.tblGasboyStationEvent WITH (HOLDLOCK) As currentTrackingData 
			    USING ChangeList As entityChanges 
				    ON entityChanges.Inserted_PK_GasboyStationEventGuid = currentTrackingData.PK_GasboyStationEventGuid
           WHEN Matched 
		    THEN 
		    UPDATE SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
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
				    ,PK_GasboyStationEventGuid
				    ,FK_ParentPK 
		    )
		    VALUES (CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
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
				    ,entityChanges.Inserted_PK_GasboyStationEventGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    );
    END
END
GO
--Creating Delete Trigger for tblGasboyStationEvent
CREATE TRIGGER dbo.trg_del_tblGasboyStationEvent_ForSync 
   ON dbo.tblGasboyStationEvent
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
						,d.GasboyStationEventGuid AS Deleted_PK_GasboyStationEventGuid
                        ,d.GasboyStationEventGuid AS Inserted_PK_GasboyStationEventGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblGasboyStationEvent WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_GasboyStationEventGuid = currentTrackingData.PK_GasboyStationEventGuid
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
						,PK_GasboyStationEventGuid
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
						,entityChanges.Deleted_PK_GasboyStationEventGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END