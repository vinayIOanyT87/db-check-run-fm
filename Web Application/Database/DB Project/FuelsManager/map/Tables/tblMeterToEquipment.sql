CREATE TABLE [map].[tblMeterToEquipment] (
    [MeterToEquipmentGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblMeterToEquipment_GUID] DEFAULT (newid()) NOT NULL,
    [MeterGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [EquipmentGuid]   UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_tblMeterToEquipment_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]       [dbo].[udtUserID]  CONSTRAINT [DF_tblMeterToEquipment_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]     DATETIMEOFFSET (7) CONSTRAINT [DF_tblMeterToEquipment_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]       [dbo].[udtUserID]  CONSTRAINT [DF_tblMeterToEquipment_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]     ROWVERSION         NOT NULL,
    [_ClusterIdx]     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblMeterToEquipment] PRIMARY KEY NONCLUSTERED ([MeterToEquipmentGuid] ASC),
    CONSTRAINT [FK_tblMeterToEquipment_MeterGuid] FOREIGN KEY ([MeterGuid]) REFERENCES [dbo].[tblMeter] ([MeterGuid]),
    CONSTRAINT [FK_tblMeterToEquipment_EquipmentGuid] FOREIGN KEY ([EquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMeterToEquipment_ClusterIdx]
    ON [map].[tblMeterToEquipment]([_ClusterIdx] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblMeterToEquipment_CreatedDate]
    ON [map].[tblMeterToEquipment]([CreatedDate] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_tblMeterToEquipment_MeterGuid]
    ON [map].[tblMeterToEquipment]([MeterGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblMeterToEquipment_EquipmentGuid]
    ON [map].[tblMeterToEquipment]([EquipmentGuid] ASC);


GO
CREATE TRIGGER [map].[trg_Audit_del_tblMeterToEquipment] ON [map].[tblMeterToEquipment] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblMeterToEquipment','D')=1 
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

	INSERT INTO [fmaudit].map_tblMeterToEquipment (
		[MeterToEquipmentGuid]
	,	[MeterGuid]
	,	[EquipmentGuid]
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
		d.[MeterToEquipmentGuid]
	,	d.[MeterGuid]
	,	d.[EquipmentGuid]
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
CREATE TRIGGER [map].[trg_Audit_ins_tblMeterToEquipment] ON [map].[tblMeterToEquipment] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblMeterToEquipment','D')=1 
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
	INSERT INTO [fmaudit].map_tblMeterToEquipment (
		[MeterToEquipmentGuid]
	,	[MeterGuid]
	,	[EquipmentGuid]
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
		i.[MeterToEquipmentGuid]
	,	i.[MeterGuid]
	,	i.[EquipmentGuid]
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
CREATE TRIGGER [map].[trg_Audit_upd_tblMeterToEquipment] ON [map].[tblMeterToEquipment] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblMeterToEquipment','D')=1 
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
	MeterToEquipmentGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblMeterToEquipment (
		[MeterToEquipmentGuid]
	,	[MeterGuid]
	,	[EquipmentGuid]
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
	OUTPUT inserted.[MeterToEquipmentGuid] AS 'MeterToEquipmentGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[MeterToEquipmentGuid]
	,	d.[MeterGuid]
	,	d.[EquipmentGuid]
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
 
	INSERT INTO [fmaudit].map_tblMeterToEquipment (
		[MeterToEquipmentGuid]
	,	[MeterGuid]
	,	[EquipmentGuid]
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
		i.[MeterToEquipmentGuid]
	,	i.[MeterGuid]
	,	i.[EquipmentGuid]
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
			agl.[MeterToEquipmentGuid]=i.[MeterToEquipmentGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


--Creating Insert / Update Trigger for tblMeterToEquipment
CREATE TRIGGER map.trg_insupd_tblMeterToEquipment_ForSync 
	ON map.tblMeterToEquipment
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
				,d.MeterToEquipmentGuid AS Deleted_PK_MeterToEquipmentGuid
				,i.MeterToEquipmentGuid AS Inserted_PK_MeterToEquipmentGuid
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
					FULL OUTER JOIN Deleted d 
						ON d.MeterToEquipmentGuid = i.MeterToEquipmentGuid
 
		UPDATE currentTrackingData SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
				,UpdatedContext = entityChanges.ChangeContext 
				,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
				,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblMeterToEquipment As currentTrackingData 
				JOIN #ChangeList As entityChanges 
					ON entityChanges.Inserted_PK_MeterToEquipmentGuid = currentTrackingData.PK_MeterToEquipmentGuid
 
		INSERT track.tblMeterToEquipment (InsertedDate 
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
										,PK_MeterToEquipmentGuid
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
				,entityChanges.Inserted_PK_MeterToEquipmentGuid
				,entityChanges.Inserted_FK_ParentPK
			FROM #ChangeList As entityChanges 
				WHERE NOT EXISTS ( SELECT 1 
										FROM track.tblMeterToEquipment As currentTrackingData
										WHERE entityChanges.Inserted_PK_MeterToEquipmentGuid = currentTrackingData.PK_MeterToEquipmentGuid
								)
	END
END 
GO

--Creating Delete Trigger for tblMeterToEquipment
CREATE TRIGGER map.trg_del_tblMeterToEquipment_ForSync 
	ON map.tblMeterToEquipment
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

		SELECT @syncContext AS ChangeContext 
				,d.MeterToEquipmentGuid AS Deleted_PK_MeterToEquipmentGuid
				,d.MeterToEquipmentGuid AS Inserted_PK_MeterToEquipmentGuid
				,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
				,d.CreatedDate AS Inserted_CreatedDate 
				,d.UpdatedDate AS Inserted_UpdatedDate 
				,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				,d._RowVersion AS Inserted_RowVersion 
				,0x0000000000000000 AS Deleted_RowVersion -- The real Deleted_RowVersion will be generated by a separate trigger on the tracking table itself
			INTO #ChangeList 
			FROM Deleted d 
				FULL OUTER JOIN Inserted i 
					ON d.MeterToEquipmentGuid = i.MeterToEquipmentGuid

		UPDATE currentTrackingData SET DeletedDate = @currentDateTimeOffset 
										,DeletedContext = entityChanges.ChangeContext 
										,DeletedRowVersion = entityChanges.Deleted_RowVersion 
			FROM track.tblMeterToEquipment AS currentTrackingData 
				JOIN #ChangeList AS entityChanges 
					ON entityChanges.Deleted_PK_MeterToEquipmentGuid = currentTrackingData.PK_MeterToEquipmentGuid

		INSERT track.tblMeterToEquipment (InsertedDate
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
										,PK_MeterToEquipmentGuid
										,FK_ParentPK 
			)
			SELECT CASE WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
						ELSE CAST('1/1/1990' AS DateTimeOffset(7)) 
					END 
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
					,entityChanges.Deleted_PK_MeterToEquipmentGuid
					,entityChanges.Deleted_FK_ParentPK
				FROM #ChangeList As entityChanges 
				WHERE NOT EXISTS ( SELECT 1 
										FROM track.tblMeterToEquipment As currentTrackingData
										WHERE entityChanges.Deleted_PK_MeterToEquipmentGuid = currentTrackingData.PK_MeterToEquipmentGuid
			)
	END
END 
GO
