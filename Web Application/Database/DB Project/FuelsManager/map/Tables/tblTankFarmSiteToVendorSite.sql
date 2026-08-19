CREATE TABLE [map].[tblTankFarmSiteToVendorSite] 
(
    [TankFarmSiteToVendorSiteGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_map_tblTankFarmSiteToVendorSite_TankFarmSiteToVendorSiteGuid] DEFAULT (newid()) NOT NULL,
    [ParentSiteGuid] UNIQUEIDENTIFIER   NULL,
    [ChildSiteGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [CreatedDate]    DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblTankFarmSiteToVendorSite_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]      [dbo].[udtUserID]  CONSTRAINT [DF_map_tblTankFarmSiteToVendorSite_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]    DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblTankFarmSiteToVendorSite_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]      [dbo].[udtUserID]  CONSTRAINT [DF_map_tblTankFarmSiteToVendorSite_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]    ROWVERSION         NOT NULL,
    [_ClusterIdx]    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_map_tblTankFarmSiteToVendorSite] PRIMARY KEY NONCLUSTERED ([TankFarmSiteToVendorSiteGuid] ASC),
    CONSTRAINT [CK_map_tblTankFarmSiteToVendorSite] CHECK ([dbo].[udf_CheckTankFarmSiteToVendorSiteMap]([ChildSiteGuid],[ParentSiteGuid]) = CONVERT([bit], (0), (0))),
    CONSTRAINT [FK_map_tblTankFarmSiteToVendorSite_ChildSiteGuid] FOREIGN KEY ([ChildSiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_map_tblTankFarmSiteToVendorSite_ParentSiteGuid] FOREIGN KEY ([ParentSiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);
GO

CREATE NONCLUSTERED INDEX [IX_map_tblTankFarmSiteToVendorSite_CreatedDate]
    ON [map].[tblTankFarmSiteToVendorSite]([CreatedDate] ASC);
GO

CREATE NONCLUSTERED INDEX IX_map_tblTankFarmSiteToVendorSite_ChildSiteGuid
ON [map].[tblTankFarmSiteToVendorSite] ([ChildSiteGuid]) INCLUDE ([TankFarmSiteToVendorSiteGuid], [ParentSiteGuid])
GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_map_tblTankFarmSiteToVendorSite_ParentSiteGuid_ChildSiteGuid]
    ON [map].[tblTankFarmSiteToVendorSite]([ParentSiteGuid] ASC, [ChildSiteGuid] ASC);
GO

CREATE TRIGGER [map].[trg_Audit_del_tblTankFarmSiteToVendorSite] ON [map].[tblTankFarmSiteToVendorSite] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblTankFarmSiteToVendorSite','D') = 1 
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
	SET @_AuditEventType = 'D'; -- For Deletes 
	SET @_AuditEventSequence = 1; 
	SELECT	@_AuditSessionGUID = s.SessionGuid 
		,	@_AuditSessionTokenID = s.SessionTokenID 
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid = s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid 
	WHERE m.SqlServerSessionID = @@SPID;

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

	INSERT INTO [fmaudit].map_tblTankFarmSiteToVendorSite 
	(
		[TankFarmSiteToVendorSiteGuid]
	,	[ParentSiteGuid]
	,	[ChildSiteGuid]
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
		d.[TankFarmSiteToVendorSiteGuid]
	,	d.[ParentSiteGuid]
	,	d.[ChildSiteGuid]
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
--Creating Insert / Update Trigger for tblTankFarmSiteToVendorSite
CREATE TRIGGER map.trg_insupd_tblTankFarmSiteToVendorSite_ForSync 
   ON map.tblTankFarmSiteToVendorSite
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
                    , d.TankFarmSiteToVendorSiteGuid AS Deleted_PK_TankFarmSiteToVendorSiteGuid
                    , i.TankFarmSiteToVendorSiteGuid AS Inserted_PK_TankFarmSiteToVendorSiteGuid
                    , CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    , CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    , i.CreatedDate AS Inserted_CreatedDate 
                    , i.UpdatedDate AS Inserted_UpdatedDate 
				    , CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    , CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    , i._RowVersion AS Inserted_RowVersion 
				    , CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TankFarmSiteToVendorSiteGuid = i.TankFarmSiteToVendorSiteGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		, UpdatedContext = entityChanges.ChangeContext 
 				        , UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					, CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				, PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTankFarmSiteToVendorSite As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TankFarmSiteToVendorSiteGuid = currentTrackingData.PK_TankFarmSiteToVendorSiteGuid
 
		    INSERT track.tblTankFarmSiteToVendorSite (InsertedDate 
 			    	, InsertedContext 
 				    , InsertedRowVersion 
 				    , UpdatedDate 
 				    , UpdatedContext 
 				    , UpdatedRowVersion 
 				    , DeletedDate 
 				    , DeletedContext 
 				    , DeletedRowVersion 
 				    , CurrentSiteGuid 
 				    , PreviousSiteGuid 
				    , PK_TankFarmSiteToVendorSiteGuid
				    , FK_ParentPK 
		    )
		    SELECT CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
		                 WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
		                 ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
			    	, entityChanges.ChangeContext 
				    , entityChanges.Inserted_RowVersion 
    				, entityChanges.Inserted_CreatedDate 
	    			, entityChanges.ChangeContext 
		    		, entityChanges.Inserted_RowVersion 
			    	, NULL 
    				, NULL 
	    			, NULL 
		    		, entityChanges.CurrentSiteGuid 
			    	, CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), entityChanges.PreviousSiteGuid),'')) THEN entityChanges.PreviousSiteGuid ELSE NULL END
				    , entityChanges.Inserted_PK_TankFarmSiteToVendorSiteGuid
				    , entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTankFarmSiteToVendorSite As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TankFarmSiteToVendorSiteGuid = currentTrackingData.PK_TankFarmSiteToVendorSiteGuid
)
    END
END 
GO

--Creating Delete Trigger for tblTankFarmSiteToVendorSite
CREATE TRIGGER map.trg_del_tblTankFarmSiteToVendorSite_ForSync 
   ON map.tblTankFarmSiteToVendorSite
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
            , @bypassTrackingFlags = BypassTrackingFlags 
            , @bypassReason = BypassReason 
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
						, d.TankFarmSiteToVendorSiteGuid AS Deleted_PK_TankFarmSiteToVendorSiteGuid
                        , d.TankFarmSiteToVendorSiteGuid AS Inserted_PK_TankFarmSiteToVendorSiteGuid
                        , NULL AS Deleted_FK_ParentPK 
						, d.CreatedDate AS Inserted_CreatedDate 
						, d.UpdatedDate AS Inserted_UpdatedDate 
						, NULL AS CurrentSiteGuid 
						, NULL AS PreviousSiteGuid 
						, d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTankFarmSiteToVendorSite As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TankFarmSiteToVendorSiteGuid = currentTrackingData.PK_TankFarmSiteToVendorSiteGuid
				WHEN Matched 
				THEN 
					UPDATE SET DeletedDate = @currentDateTimeOffset 
								, DeletedContext = entityChanges.ChangeContext 
                                , DeletedRowVersion = entityChanges.Deleted_RowVersion 
				WHEN Not Matched 
				THEN 
				INSERT (InsertedDate
				    	, InsertedContext
				    	, InsertedRowVersion
				    	, UpdatedDate
				    	, UpdatedContext
				    	, UpdatedRowVersion
				    	, CurrentSiteGuid
				    	, PreviousSiteGuid
				    	, DeletedDate
				    	, DeletedContext
				    	, DeletedRowVersion
						, PK_TankFarmSiteToVendorSiteGuid
				        , FK_ParentPK 
				)
				VALUES (CASE WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
						, entityChanges.ChangeContext 
						, entityChanges.Inserted_RowVersion 
						, NULL 
						, NULL 
						, NULL 
						, entityChanges.CurrentSiteGuid 
						, NULL 
						, @currentDateTimeOffset 
						, entityChanges.ChangeContext 
						, entityChanges.Deleted_RowVersion
						, entityChanges.Deleted_PK_TankFarmSiteToVendorSiteGuid
				        , entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO

CREATE TRIGGER [map].[trg_Audit_ins_tblTankFarmSiteToVendorSite] ON [map].[tblTankFarmSiteToVendorSite] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;

	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblTankFarmSiteToVendorSite','D') = 1 
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
	SET @_AuditEventType = 'I' -- For Inserts 
	SET @_AuditEventSequence = 1 
	SELECT	@_AuditSessionGUID = s.SessionGuid 
		,	@_AuditSessionTokenID = s.SessionTokenID 
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid = s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid 
	WHERE m.SqlServerSessionID = @@SPID 

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

	INSERT INTO [fmaudit].map_tblTankFarmSiteToVendorSite
	(
		[TankFarmSiteToVendorSiteGuid]
	,	[ParentSiteGuid]
	,	[ChildSiteGuid]
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
		i.[TankFarmSiteToVendorSiteGuid]
	,	i.[ParentSiteGuid]
	,	i.[ChildSiteGuid]
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
CREATE TRIGGER [map].[trg_Audit_upd_tblTankFarmSiteToVendorSite] ON [map].[tblTankFarmSiteToVendorSite] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblTankFarmSiteToVendorSite','D')=1 
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
	SET @_AuditEventType = 'U' -- For Updates 
	SET @_AuditEventSequence = 1 
	SELECT	@_AuditSessionGUID = s.SessionGuid 
		,	@_AuditSessionTokenID = s.SessionTokenID 
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid = s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid 
	WHERE m.SqlServerSessionID = @@SPID 

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
		TankFarmSiteToVendorSiteGuid	uniqueidentifier NULL
		, _AuditEventType CHAR(1)
		, _AuditEventSequence TINYINT
		, _AuditCreatedDate DATETIMEOFFSET
		, _AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblTankFarmSiteToVendorSite (
		[TankFarmSiteToVendorSiteGuid]
	,	[ParentSiteGuid]
	,	[ChildSiteGuid]
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
	OUTPUT inserted.[TankFarmSiteToVendorSiteGuid] AS 'TankFarmSiteToVendorSiteGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[TankFarmSiteToVendorSiteGuid]
	,	d.[ParentSiteGuid]
	,	d.[ChildSiteGuid]
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
 
	INSERT INTO [fmaudit].map_tblTankFarmSiteToVendorSiteGuid 
	(
		[TankFarmSiteToVendorSiteGuid]
	,	[ParentSiteGuid]
	,	[ChildSiteGuid]
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
		i.[TankFarmSiteToVendorSiteGuid]
	,	i.[ParentSiteGuid]
	,	i.[ChildSiteGuid]
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
			agl.[TankFarmSiteToVendorSiteGuid] = i.[TankFarmSiteToVendorSiteGuid] 
		)
		WHERE	agl._AuditEventType = 'U'
		AND		agl._AuditEventSequence = 1 
		AND		agl._AuditCreatedDate = @_AuditDatetime
END
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblTankFarmSiteToVendorSite_ClusterIdx]
    ON [map].[tblTankFarmSiteToVendorSite]([_ClusterIdx] ASC);