CREATE TABLE [map].[tblQualificationPersonTrainingToEquipmentType] (
    [QualificationPersonTrainingToEquipmentTypeGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_map_tblQualificationPersonTrainingToEquipmentTypeGUID] DEFAULT (newid()) NOT NULL,
    [QualificationGuid]                              UNIQUEIDENTIFIER   NOT NULL,
    [EquipmentTypeGuid]                              UNIQUEIDENTIFIER   NOT NULL,
    [Sequence]                                       INT                NOT NULL,
    [Instructor]                                     NVARCHAR (50)      NULL,
    [DateCompleted]                                  DATETIMEOFFSET (7) NULL,
    [DateDue]                                        DATETIMEOFFSET (7) NULL,
    [ExpirationDate]                                 DATETIMEOFFSET (7) NULL,
    [ID]                                             VARCHAR (50)       NULL,
    [Rating]                                         NVARCHAR (20)      NULL,
    [HistoricalRecord]                               BIT                CONSTRAINT [DF_map_tblQualificationPersonTrainingToEquipmentType_HistoricalRecord] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                                    DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblQualificationPersonTrainingToEquipmentType_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                                      [dbo].[udtUserID]  CONSTRAINT [DF_map_tblQualificationPersonTrainingToEquipmentType_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                                    DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblQualificationPersonTrainingToEquipmentType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                                      [dbo].[udtUserID]  CONSTRAINT [DF_map_tblQualificationPersonTrainingToEquipmentType_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                                    BIGINT             IDENTITY (1, 1) NOT NULL,
    [SiteGuid]								   UNIQUEIDENTIFIER   NOT NULL, 
    CONSTRAINT [PK_map_tblQualificationPersonTrainingToEquipmentType] PRIMARY KEY NONCLUSTERED ([QualificationPersonTrainingToEquipmentTypeGuid] ASC),
    CONSTRAINT [FK_map_tblQualificationPersonTrainingToEquipmentType_EquipmentTypeGuid] FOREIGN KEY ([EquipmentTypeGuid]) REFERENCES [dbo].[tblEquipmentTypes] ([EquipmentTypeGuid]),
    CONSTRAINT [FK_map_tblQualificationPersonTrainingToEquipmentType_QualificationGuid] FOREIGN KEY ([QualificationGuid]) REFERENCES [dbo].[tblQualifications] ([QualificationGuid]),
    CONSTRAINT [FK_map_tblQualificationPersonTrainingToEquipmentType_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites]([SiteGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblQualificationPersonTrainingToEquipmentType_CreatedDate]
    ON [map].[tblQualificationPersonTrainingToEquipmentType]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblQualificationPersonTrainingToEquipmentType_EquipmentTypeGuid]
    ON [map].[tblQualificationPersonTrainingToEquipmentType]([EquipmentTypeGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblQualificationPersonTrainingToEquipmentType_QualificationGuid]
    ON [map].[tblQualificationPersonTrainingToEquipmentType]([QualificationGuid] ASC);


GO
--Creating Insert / Update Trigger for tblQualificationPersonTrainingToEquipmentType
CREATE TRIGGER map.trg_insupd_tblQualificationPersonTrainingToEquipmentType_ForSync 
   ON map.tblQualificationPersonTrainingToEquipmentType
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
                    ,d.QualificationPersonTrainingToEquipmentTypeGuid AS Deleted_PK_QualificationPersonTrainingToEquipmentTypeGuid
                    ,i.QualificationPersonTrainingToEquipmentTypeGuid AS Inserted_PK_QualificationPersonTrainingToEquipmentTypeGuid
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
				    d.QualificationPersonTrainingToEquipmentTypeGuid = i.QualificationPersonTrainingToEquipmentTypeGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblQualificationPersonTrainingToEquipmentType As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_QualificationPersonTrainingToEquipmentTypeGuid = currentTrackingData.PK_QualificationPersonTrainingToEquipmentTypeGuid
 
 
		    INSERT track.tblQualificationPersonTrainingToEquipmentType (InsertedDate 
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
				    ,PK_QualificationPersonTrainingToEquipmentTypeGuid
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
				    ,entityChanges.Inserted_PK_QualificationPersonTrainingToEquipmentTypeGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblQualificationPersonTrainingToEquipmentType As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_QualificationPersonTrainingToEquipmentTypeGuid = currentTrackingData.PK_QualificationPersonTrainingToEquipmentTypeGuid
)
    END
END 

GO
--Creating Delete Trigger for tblQualificationPersonTrainingToEquipmentType
CREATE TRIGGER map.trg_del_tblQualificationPersonTrainingToEquipmentType_ForSync 
   ON map.tblQualificationPersonTrainingToEquipmentType
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
						,d.QualificationPersonTrainingToEquipmentTypeGuid AS Deleted_PK_QualificationPersonTrainingToEquipmentTypeGuid
                        ,d.QualificationPersonTrainingToEquipmentTypeGuid AS Inserted_PK_QualificationPersonTrainingToEquipmentTypeGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblQualificationPersonTrainingToEquipmentType As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_QualificationPersonTrainingToEquipmentTypeGuid = currentTrackingData.PK_QualificationPersonTrainingToEquipmentTypeGuid
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
						,PK_QualificationPersonTrainingToEquipmentTypeGuid
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
						,entityChanges.Deleted_PK_QualificationPersonTrainingToEquipmentTypeGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [map].[trg_Audit_del_tblQualificationPersonTrainingToEquipmentType] ON [map].[tblQualificationPersonTrainingToEquipmentType] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblQualificationPersonTrainingToEquipmentType','D')=1 
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
	INSERT INTO [fmaudit].map_tblQualificationPersonTrainingToEquipmentType (
		[QualificationPersonTrainingToEquipmentTypeGuid]
	,	[QualificationGuid]
	,	[EquipmentTypeGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
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
	,	[SiteGuid]
	)
	SELECT 
		d.[QualificationPersonTrainingToEquipmentTypeGuid]
	,	d.[QualificationGuid]
	,	d.[EquipmentTypeGuid]
	,	d.[Sequence]
	,	d.[Instructor]
	,	d.[DateCompleted]
	,	d.[DateDue]
	,	d.[ExpirationDate]
	,	d.[ID]
	,	d.[Rating]
	,	d.[HistoricalRecord]
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
	,	d.[SiteGuid]
	FROM deleted d
END

GO
CREATE TRIGGER [map].[trg_Audit_ins_tblQualificationPersonTrainingToEquipmentType] ON [map].[tblQualificationPersonTrainingToEquipmentType] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblQualificationPersonTrainingToEquipmentType','D')=1 
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
	INSERT INTO [fmaudit].map_tblQualificationPersonTrainingToEquipmentType (
		[QualificationPersonTrainingToEquipmentTypeGuid]
	,	[QualificationGuid]
	,	[EquipmentTypeGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
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
	,	[SiteGuid]
	)
	SELECT 
		i.[QualificationPersonTrainingToEquipmentTypeGuid]
	,	i.[QualificationGuid]
	,	i.[EquipmentTypeGuid]
	,	i.[Sequence]
	,	i.[Instructor]
	,	i.[DateCompleted]
	,	i.[DateDue]
	,	i.[ExpirationDate]
	,	i.[ID]
	,	i.[Rating]
	,	i.[HistoricalRecord]
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
	,	i.[SiteGuid]
	FROM inserted i
END

GO
CREATE TRIGGER [map].[trg_Audit_upd_tblQualificationPersonTrainingToEquipmentType] ON [map].[tblQualificationPersonTrainingToEquipmentType] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblQualificationPersonTrainingToEquipmentType','D')=1 
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
	QualificationPersonTrainingToEquipmentTypeGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblQualificationPersonTrainingToEquipmentType (
		[QualificationPersonTrainingToEquipmentTypeGuid]
	,	[QualificationGuid]
	,	[EquipmentTypeGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
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
	,	[SiteGuid]
	)
	OUTPUT inserted.[QualificationPersonTrainingToEquipmentTypeGuid] AS 'QualificationPersonTrainingToEquipmentTypeGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[QualificationPersonTrainingToEquipmentTypeGuid]
	,	d.[QualificationGuid]
	,	d.[EquipmentTypeGuid]
	,	d.[Sequence]
	,	d.[Instructor]
	,	d.[DateCompleted]
	,	d.[DateDue]
	,	d.[ExpirationDate]
	,	d.[ID]
	,	d.[Rating]
	,	d.[HistoricalRecord]
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
	,	d.[SiteGuid]
	FROM deleted d
 
	INSERT INTO [fmaudit].map_tblQualificationPersonTrainingToEquipmentType (
		[QualificationPersonTrainingToEquipmentTypeGuid]
	,	[QualificationGuid]
	,	[EquipmentTypeGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
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
	,	[SiteGuid]
	)
	SELECT 
		i.[QualificationPersonTrainingToEquipmentTypeGuid]
	,	i.[QualificationGuid]
	,	i.[EquipmentTypeGuid]
	,	i.[Sequence]
	,	i.[Instructor]
	,	i.[DateCompleted]
	,	i.[DateDue]
	,	i.[ExpirationDate]
	,	i.[ID]
	,	i.[Rating]
	,	i.[HistoricalRecord]
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
	,	i.[SiteGuid]
	FROM inserted i 
	INNER JOIN	@AuditGuidList agl ON
		(
			agl.[QualificationPersonTrainingToEquipmentTypeGuid]=i.[QualificationPersonTrainingToEquipmentTypeGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblQualificationPersonTrainingToEquipmentType_ClusterIdx]
    ON [map].[tblQualificationPersonTrainingToEquipmentType]([_ClusterIdx] ASC);

