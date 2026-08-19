CREATE TABLE [dbo].[tblLoadArms] (
    [LoadRackText]          NVARCHAR (9)       NULL,
    [Enabled]               BIT                CONSTRAINT [DF_tblLoadArms_Enabled] DEFAULT ((0)) NOT NULL,
    [SwingArm]              BIT                CONSTRAINT [DF_tblLoadArms_SwingArm] DEFAULT ((0)) NOT NULL,
    [BayAArmNumber]         INT                NULL,
    [BayBArmNumber]         INT                NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblLoadingArms_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblLoadArms_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblLoadingArms_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblLoadArms_UpdatedBy] DEFAULT ('') NOT NULL,
    [LoadArmGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblLoadArms_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [LookupPresetTypeIndex] INT                CONSTRAINT [DF_tblLoadArms_LookupPresetTypeIndex] DEFAULT ((0)) NOT NULL,
    [BayAStationGuid]       UNIQUEIDENTIFIER   NULL,
    [BayBStationGuid]       UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblLoadArms_GUID] PRIMARY KEY NONCLUSTERED ([LoadArmGuid] ASC),
    CONSTRAINT [FK_tblLoadArms_BayAStationGuid] FOREIGN KEY ([BayAStationGuid]) REFERENCES [dbo].[tblStations] ([StationGuid]),
    CONSTRAINT [FK_tblLoadArms_BayBStationGuid] FOREIGN KEY ([BayBStationGuid]) REFERENCES [dbo].[tblStations] ([StationGuid]),
    CONSTRAINT [FK_tblLoadArms_LookupPresetTypeIndex] FOREIGN KEY ([LookupPresetTypeIndex]) REFERENCES [lookup].[tblPresetType] ([PresetTypeIndex])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblLoadArms_CreatedDate]
    ON [dbo].[tblLoadArms]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblLoadArms] ON [dbo].[tblLoadArms] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblLoadArms','D')=1 
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
	LoadArmGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblLoadArms (
		[LoadRackText]
	,	[Enabled]
	,	[SwingArm]
	,	[BayAArmNumber]
	,	[BayBArmNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[LoadArmGuid]
	,	[OriginalRowVersion]
	,	[LookupPresetTypeIndex]
	,	[BayAStationGuid]
	,	[BayBStationGuid]
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
	OUTPUT inserted.[LoadArmGuid] AS 'LoadArmGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[LoadRackText]
	,	d.[Enabled]
	,	d.[SwingArm]
	,	d.[BayAArmNumber]
	,	d.[BayBArmNumber]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[LoadArmGuid]
	,	d.[_RowVersion]
	,	d.[LookupPresetTypeIndex]
	,	d.[BayAStationGuid]
	,	d.[BayBStationGuid]
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
 
	INSERT INTO [fmaudit].tblLoadArms (
		[LoadRackText]
	,	[Enabled]
	,	[SwingArm]
	,	[BayAArmNumber]
	,	[BayBArmNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[LoadArmGuid]
	,	[OriginalRowVersion]
	,	[LookupPresetTypeIndex]
	,	[BayAStationGuid]
	,	[BayBStationGuid]
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
		i.[LoadRackText]
	,	i.[Enabled]
	,	i.[SwingArm]
	,	i.[BayAArmNumber]
	,	i.[BayBArmNumber]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[LoadArmGuid]
	,	i.[_RowVersion]
	,	i.[LookupPresetTypeIndex]
	,	i.[BayAStationGuid]
	,	i.[BayBStationGuid]
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
			agl.[LoadArmGuid]=i.[LoadArmGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblLoadArms
CREATE TRIGGER dbo.trg_insupd_tblLoadArms_ForSync 
   ON dbo.tblLoadArms
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
                    ,d.LoadArmGuid AS Deleted_PK_LoadArmGuid
                    ,i.LoadArmGuid AS Inserted_PK_LoadArmGuid
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
				    d.LoadArmGuid = i.LoadArmGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblLoadArms As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_LoadArmGuid = currentTrackingData.PK_LoadArmGuid
 
 
		    INSERT track.tblLoadArms (InsertedDate 
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
				    ,PK_LoadArmGuid
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
				    ,entityChanges.Inserted_PK_LoadArmGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblLoadArms As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_LoadArmGuid = currentTrackingData.PK_LoadArmGuid
)
    END
END 

GO
--Creating Delete Trigger for tblLoadArms
CREATE TRIGGER dbo.trg_del_tblLoadArms_ForSync 
   ON dbo.tblLoadArms
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
						,d.LoadArmGuid AS Deleted_PK_LoadArmGuid
                        ,d.LoadArmGuid AS Inserted_PK_LoadArmGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblLoadArms As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_LoadArmGuid = currentTrackingData.PK_LoadArmGuid
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
						,PK_LoadArmGuid
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
						,entityChanges.Deleted_PK_LoadArmGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblLoadArms] ON [dbo].[tblLoadArms] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblLoadArms','D')=1 
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
	INSERT INTO [fmaudit].tblLoadArms (
		[LoadRackText]
	,	[Enabled]
	,	[SwingArm]
	,	[BayAArmNumber]
	,	[BayBArmNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[LoadArmGuid]
	,	[OriginalRowVersion]
	,	[LookupPresetTypeIndex]
	,	[BayAStationGuid]
	,	[BayBStationGuid]
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
		d.[LoadRackText]
	,	d.[Enabled]
	,	d.[SwingArm]
	,	d.[BayAArmNumber]
	,	d.[BayBArmNumber]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[LoadArmGuid]
	,	d.[_RowVersion]
	,	d.[LookupPresetTypeIndex]
	,	d.[BayAStationGuid]
	,	d.[BayBStationGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblLoadArms] ON [dbo].[tblLoadArms] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblLoadArms','D')=1 
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
	INSERT INTO [fmaudit].tblLoadArms (
		[LoadRackText]
	,	[Enabled]
	,	[SwingArm]
	,	[BayAArmNumber]
	,	[BayBArmNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[LoadArmGuid]
	,	[OriginalRowVersion]
	,	[LookupPresetTypeIndex]
	,	[BayAStationGuid]
	,	[BayBStationGuid]
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
		i.[LoadRackText]
	,	i.[Enabled]
	,	i.[SwingArm]
	,	i.[BayAArmNumber]
	,	i.[BayBArmNumber]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[LoadArmGuid]
	,	i.[_RowVersion]
	,	i.[LookupPresetTypeIndex]
	,	i.[BayAStationGuid]
	,	i.[BayBStationGuid]
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


CREATE TRIGGER [dbo].[trg_fmcdc_tblLoadArms]
ON [dbo].[tblLoadArms]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @eventType nvarchar(20)
	IF ((EXISTS(SELECT * FROM inserted)) AND (EXISTS(SELECT * FROM deleted)))
		SELECT @eventType = 'update'
	ELSE IF (EXISTS(SELECT * FROM inserted))
		SELECT @eventType = 'insert'
	ELSE IF (EXISTS(SELECT * FROM deleted))
		SELECT @eventType = 'delete'
	IF (@eventType = 'delete')
	BEGIN
		INSERT INTO fmcdc.[tblLoadArms]
		(
		[LoadRackText]
		, [Enabled]
		, [SwingArm]
		, [BayAArmNumber]
		, [BayBArmNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [LoadArmGuid]
		, [SourceRowVersion]
		, [LookupPresetTypeIndex]
		, [BayAStationGuid]
		, [BayBStationGuid]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[LoadRackText]
		, [Enabled]
		, [SwingArm]
		, [BayAArmNumber]
		, [BayBArmNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [LoadArmGuid]
		, CONVERT(bigint, _RowVersion)
		, [LookupPresetTypeIndex]
		, [BayAStationGuid]
		, [BayBStationGuid]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblLoadArms]
		(
		[LoadRackText]
		, [Enabled]
		, [SwingArm]
		, [BayAArmNumber]
		, [BayBArmNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [LoadArmGuid]
		, [SourceRowVersion]
		, [LookupPresetTypeIndex]
		, [BayAStationGuid]
		, [BayBStationGuid]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[LoadRackText]
		, [Enabled]
		, [SwingArm]
		, [BayAArmNumber]
		, [BayBArmNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [LoadArmGuid]
		, CONVERT(bigint, _RowVersion)
		, [LookupPresetTypeIndex]
		, [BayAStationGuid]
		, [BayBStationGuid]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblLoadArms] ON [dbo].[tblLoadArms]
GO



CREATE UNIQUE CLUSTERED INDEX [IX_tblLoadArms_ClusterIdx]
    ON [dbo].[tblLoadArms]([_ClusterIdx] ASC);

