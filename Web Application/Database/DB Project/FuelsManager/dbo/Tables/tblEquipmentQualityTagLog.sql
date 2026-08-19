CREATE TABLE [dbo].[tblEquipmentQualityTagLog] (
    [QualityTagName]             NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_QualityTagName] DEFAULT ('') NOT NULL,
    [EquipmentID]                NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_EquipmentID] DEFAULT ('') NOT NULL,
    [EquipmentType]              NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_EquipmentType] DEFAULT ('') NOT NULL,
    [TaggedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentQualityTagLog_TaggedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TaggedBy]                   NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentQualityTagLog_TaggedBy] DEFAULT ('') NOT NULL,
    [Memo]                       NVARCHAR (1000)    NULL,
    [RemovedDate]                DATETIMEOFFSET (7) NULL,
    [RemovedBy]                  NVARCHAR (255)     NULL,
    [DeleteFlag]                 BIT                CONSTRAINT [DF_tblEquipmentQualityTagLog_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentQualityTagLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentQualityTagLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentQualityTagLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentQualityTagLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [TagNumber]                  INT                NULL,
    [EquipmentQualityTagLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblEquipmentQualityTagLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [SiteGuid]                   UNIQUEIDENTIFIER   NOT NULL,
    [EquipmentGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [QualityTagGuid]             UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblEquipmentQualityTagLog_GUID] PRIMARY KEY NONCLUSTERED ([EquipmentQualityTagLogGuid] ASC),
    CONSTRAINT [FK_tblEquipmentQualityTagLog_EquipmentGuid] FOREIGN KEY ([EquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid]),
    CONSTRAINT [FK_tblEquipmentQualityTagLog_QualityTagGuid] FOREIGN KEY ([QualityTagGuid]) REFERENCES [dbo].[tblQualityTags] ([QualityTagGuid]),
    CONSTRAINT [FK_tblEquipmentQualityTagLog_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentQualityTagLog_CreatedDate]
    ON [dbo].[tblEquipmentQualityTagLog]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentQualityTagLog_EquipmentIndex]
    ON [dbo].[tblEquipmentQualityTagLog]([EquipmentGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentQualityTagLog_QualityTagIndex]
    ON [dbo].[tblEquipmentQualityTagLog]([QualityTagGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentQualityTagLog_SiteIndex]
    ON [dbo].[tblEquipmentQualityTagLog]([SiteGuid] ASC);


GO
--Creating Insert / Update Trigger for tblEquipmentQualityTagLog
CREATE TRIGGER dbo.trg_insupd_tblEquipmentQualityTagLog_ForSync 
   ON dbo.tblEquipmentQualityTagLog
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
                    ,d.EquipmentQualityTagLogGuid AS Deleted_PK_EquipmentQualityTagLogGuid
                    ,i.EquipmentQualityTagLogGuid AS Inserted_PK_EquipmentQualityTagLogGuid
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
				    d.EquipmentQualityTagLogGuid = i.EquipmentQualityTagLogGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblEquipmentQualityTagLog As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_EquipmentQualityTagLogGuid = currentTrackingData.PK_EquipmentQualityTagLogGuid
 
 
		    INSERT track.tblEquipmentQualityTagLog (InsertedDate 
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
				    ,PK_EquipmentQualityTagLogGuid
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
				    ,entityChanges.Inserted_PK_EquipmentQualityTagLogGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblEquipmentQualityTagLog As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_EquipmentQualityTagLogGuid = currentTrackingData.PK_EquipmentQualityTagLogGuid
)
    END
END 

GO
--Creating Delete Trigger for tblEquipmentQualityTagLog
CREATE TRIGGER dbo.trg_del_tblEquipmentQualityTagLog_ForSync 
   ON dbo.tblEquipmentQualityTagLog
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
						,d.EquipmentQualityTagLogGuid AS Deleted_PK_EquipmentQualityTagLogGuid
                        ,d.EquipmentQualityTagLogGuid AS Inserted_PK_EquipmentQualityTagLogGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblEquipmentQualityTagLog As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_EquipmentQualityTagLogGuid = currentTrackingData.PK_EquipmentQualityTagLogGuid
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
						,PK_EquipmentQualityTagLogGuid
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
						,entityChanges.Deleted_PK_EquipmentQualityTagLogGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblEquipmentQualityTagLog] ON [dbo].[tblEquipmentQualityTagLog] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipmentQualityTagLog','D')=1 
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
	INSERT INTO [fmaudit].tblEquipmentQualityTagLog (
		[QualityTagName]
	,	[EquipmentID]
	,	[EquipmentType]
	,	[TaggedDate]
	,	[TaggedBy]
	,	[Memo]
	,	[RemovedDate]
	,	[RemovedBy]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TagNumber]
	,	[EquipmentQualityTagLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[QualityTagGuid]
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
		d.[QualityTagName]
	,	d.[EquipmentID]
	,	d.[EquipmentType]
	,	d.[TaggedDate]
	,	d.[TaggedBy]
	,	d.[Memo]
	,	d.[RemovedDate]
	,	d.[RemovedBy]
	,	d.[DeleteFlag]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TagNumber]
	,	d.[EquipmentQualityTagLogGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[EquipmentGuid]
	,	d.[QualityTagGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblEquipmentQualityTagLog] ON [dbo].[tblEquipmentQualityTagLog] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipmentQualityTagLog','D')=1 
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
	INSERT INTO [fmaudit].tblEquipmentQualityTagLog (
		[QualityTagName]
	,	[EquipmentID]
	,	[EquipmentType]
	,	[TaggedDate]
	,	[TaggedBy]
	,	[Memo]
	,	[RemovedDate]
	,	[RemovedBy]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TagNumber]
	,	[EquipmentQualityTagLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[QualityTagGuid]
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
		i.[QualityTagName]
	,	i.[EquipmentID]
	,	i.[EquipmentType]
	,	i.[TaggedDate]
	,	i.[TaggedBy]
	,	i.[Memo]
	,	i.[RemovedDate]
	,	i.[RemovedBy]
	,	i.[DeleteFlag]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TagNumber]
	,	i.[EquipmentQualityTagLogGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[EquipmentGuid]
	,	i.[QualityTagGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblEquipmentQualityTagLog] ON [dbo].[tblEquipmentQualityTagLog] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipmentQualityTagLog','D')=1 
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
	EquipmentQualityTagLogGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblEquipmentQualityTagLog (
		[QualityTagName]
	,	[EquipmentID]
	,	[EquipmentType]
	,	[TaggedDate]
	,	[TaggedBy]
	,	[Memo]
	,	[RemovedDate]
	,	[RemovedBy]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TagNumber]
	,	[EquipmentQualityTagLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[QualityTagGuid]
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
	OUTPUT inserted.[EquipmentQualityTagLogGuid] AS 'EquipmentQualityTagLogGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[QualityTagName]
	,	d.[EquipmentID]
	,	d.[EquipmentType]
	,	d.[TaggedDate]
	,	d.[TaggedBy]
	,	d.[Memo]
	,	d.[RemovedDate]
	,	d.[RemovedBy]
	,	d.[DeleteFlag]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TagNumber]
	,	d.[EquipmentQualityTagLogGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[EquipmentGuid]
	,	d.[QualityTagGuid]
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
 
	INSERT INTO [fmaudit].tblEquipmentQualityTagLog (
		[QualityTagName]
	,	[EquipmentID]
	,	[EquipmentType]
	,	[TaggedDate]
	,	[TaggedBy]
	,	[Memo]
	,	[RemovedDate]
	,	[RemovedBy]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TagNumber]
	,	[EquipmentQualityTagLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[QualityTagGuid]
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
		i.[QualityTagName]
	,	i.[EquipmentID]
	,	i.[EquipmentType]
	,	i.[TaggedDate]
	,	i.[TaggedBy]
	,	i.[Memo]
	,	i.[RemovedDate]
	,	i.[RemovedBy]
	,	i.[DeleteFlag]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TagNumber]
	,	i.[EquipmentQualityTagLogGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[EquipmentGuid]
	,	i.[QualityTagGuid]
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
			agl.[EquipmentQualityTagLogGuid]=i.[EquipmentQualityTagLogGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEquipmentQualityTagLog_ClusterIdx]
    ON [dbo].[tblEquipmentQualityTagLog]([_ClusterIdx] ASC);
