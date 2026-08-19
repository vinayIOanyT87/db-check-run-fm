CREATE TABLE [dbo].[tblWeightedAverageCosts] (
    [WacValue]                FLOAT (53)         NOT NULL,
    [IsManualOverride]        BIT                NOT NULL,
    [Source]                  NVARCHAR (64)      NOT NULL,
    [Notes]                   NVARCHAR (2048)    NULL,
    [CreatedBy]               [dbo].[udtUserID]  NULL,
    [CreatedDate]             DATETIMEOFFSET (7) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) NOT NULL,
    [InventoryDate]           DATE               NOT NULL,
    [WeightedAverageCostGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblWeightedAverageCosts_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [SiteGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [ProductGuid]             UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblWeightedAverageCosts_GUID] PRIMARY KEY NONCLUSTERED ([WeightedAverageCostGuid] ASC),
    CONSTRAINT [FK_tblWeightedAverageCosts_ProductGuid] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
    CONSTRAINT [FK_tblWeightedAverageCosts_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblWeightedAverageCosts_CreatedDate]
    ON [dbo].[tblWeightedAverageCosts]([CreatedDate] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblWeightedAverageCosts] ON [dbo].[tblWeightedAverageCosts] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblWeightedAverageCosts','D')=1 
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
	INSERT INTO [fmaudit].tblWeightedAverageCosts (
		[WacValue]
	,	[IsManualOverride]
	,	[Source]
	,	[Notes]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[InventoryDate]
	,	[WeightedAverageCostGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ProductGuid]
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
		d.[WacValue]
	,	d.[IsManualOverride]
	,	d.[Source]
	,	d.[Notes]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[InventoryDate]
	,	d.[WeightedAverageCostGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ProductGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblWeightedAverageCosts] ON [dbo].[tblWeightedAverageCosts] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblWeightedAverageCosts','D')=1 
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
	INSERT INTO [fmaudit].tblWeightedAverageCosts (
		[WacValue]
	,	[IsManualOverride]
	,	[Source]
	,	[Notes]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[InventoryDate]
	,	[WeightedAverageCostGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ProductGuid]
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
		i.[WacValue]
	,	i.[IsManualOverride]
	,	i.[Source]
	,	i.[Notes]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[InventoryDate]
	,	i.[WeightedAverageCostGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ProductGuid]
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
--Creating Insert / Update Trigger for tblWeightedAverageCosts
CREATE TRIGGER dbo.trg_insupd_tblWeightedAverageCosts_ForSync 
   ON dbo.tblWeightedAverageCosts
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
                    ,d.WeightedAverageCostGuid AS Deleted_PK_WeightedAverageCostGuid
                    ,i.WeightedAverageCostGuid AS Inserted_PK_WeightedAverageCostGuid
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
				    d.WeightedAverageCostGuid = i.WeightedAverageCostGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblWeightedAverageCosts As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_WeightedAverageCostGuid = currentTrackingData.PK_WeightedAverageCostGuid
 
 
		    INSERT track.tblWeightedAverageCosts (InsertedDate 
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
				    ,PK_WeightedAverageCostGuid
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
				    ,entityChanges.Inserted_PK_WeightedAverageCostGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblWeightedAverageCosts As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_WeightedAverageCostGuid = currentTrackingData.PK_WeightedAverageCostGuid
)
    END
END 

GO
--Creating Delete Trigger for tblWeightedAverageCosts
CREATE TRIGGER dbo.trg_del_tblWeightedAverageCosts_ForSync 
   ON dbo.tblWeightedAverageCosts
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
						,d.WeightedAverageCostGuid AS Deleted_PK_WeightedAverageCostGuid
                        ,d.WeightedAverageCostGuid AS Inserted_PK_WeightedAverageCostGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblWeightedAverageCosts As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_WeightedAverageCostGuid = currentTrackingData.PK_WeightedAverageCostGuid
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
						,PK_WeightedAverageCostGuid
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
						,entityChanges.Deleted_PK_WeightedAverageCostGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblWeightedAverageCosts] ON [dbo].[tblWeightedAverageCosts] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblWeightedAverageCosts','D')=1 
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
	WeightedAverageCostGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblWeightedAverageCosts (
		[WacValue]
	,	[IsManualOverride]
	,	[Source]
	,	[Notes]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[InventoryDate]
	,	[WeightedAverageCostGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ProductGuid]
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
	OUTPUT inserted.[WeightedAverageCostGuid] AS 'WeightedAverageCostGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[WacValue]
	,	d.[IsManualOverride]
	,	d.[Source]
	,	d.[Notes]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[InventoryDate]
	,	d.[WeightedAverageCostGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[ProductGuid]
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
 
	INSERT INTO [fmaudit].tblWeightedAverageCosts (
		[WacValue]
	,	[IsManualOverride]
	,	[Source]
	,	[Notes]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[InventoryDate]
	,	[WeightedAverageCostGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[ProductGuid]
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
		i.[WacValue]
	,	i.[IsManualOverride]
	,	i.[Source]
	,	i.[Notes]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[InventoryDate]
	,	i.[WeightedAverageCostGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[ProductGuid]
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
			agl.[WeightedAverageCostGuid]=i.[WeightedAverageCostGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblWeightedAverageCosts_ClusterIdx]
    ON [dbo].[tblWeightedAverageCosts]([_ClusterIdx] ASC);

