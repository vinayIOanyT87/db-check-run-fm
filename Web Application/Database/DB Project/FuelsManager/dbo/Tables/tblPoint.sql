CREATE TABLE [dbo].[tblPoint]
(
	[ID]											NVARCHAR (30)		CONSTRAINT [DF_tblPoint_ID] DEFAULT ('') NOT NULL,
	[Description]									NVARCHAR (50)		NULL,
	[Enabled]										BIT					NULL,
	[Standard]										BIT					NOT NULL,
	[ExecutionInterval]								INT					NULL, 
	[LevelUnitIndex]								INT					NULL,
	[TemperatureUnitIndex]							INT					NULL,
	[DensityUnitIndex]								INT					NULL,
	[PressureUnitIndex]								INT					NULL,
	[FlowUnitIndex]									INT					NULL,
	[VolumeUnitIndex]								INT					NULL,
	[MassUnitIndex]									INT					NULL,
	[VelocityUnitIndex]								INT					NULL,
	[MassFlowUnitIndex]								INT					NULL,
	[LevelDecimalPlaces]							TINYINT				NULL,
	[TemperatureDecimalPlaces]						TINYINT				NULL,
	[DensityDecimalPlaces]							TINYINT				NULL,
	[PressureDecimalPlaces]							TINYINT				NULL,
	[FlowDecimalPlaces]								TINYINT				NULL,
	[VolumeDecimalPlaces]							TINYINT				NULL,
	[MassDecimalPlaces]								TINYINT				NULL,
	[VelocityDecimalPlaces]							TINYINT				NULL,
	[MassFlowDecimalPlaces]							TINYINT				NULL,
	[LevelMaximum]									FLOAT				NULL,
	[LevelMinimum]									FLOAT				NULL,
	[TemperatureMaximum]							FLOAT				NULL,
	[TemperatureMinimum]							FLOAT				NULL,
	[DensityMaximum]								FLOAT				NULL,
	[DensityMinimum]								FLOAT				NULL,
	[PressureMaximum]								FLOAT				NULL,
	[PressureMinimum]								FLOAT				NULL,
	[VolumetricFlowMaximum]							FLOAT				NULL,
	[VolumetricFlowMinimum]							FLOAT				NULL,
	[VolumeMaximum]									FLOAT				NULL,
	[VolumeMinimum]									FLOAT				NULL,
	[MassMaximum]									FLOAT				NULL,
	[MassMinimum]									FLOAT				NULL,
	[VelocityMaximum]								FLOAT				NULL,
	[VelocityMinimum]								FLOAT				NULL,
	[MassFlowMaximum]								FLOAT				NULL,
	[MassFlowMinimum]								FLOAT				NULL,
	[CreatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblPoint_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy]										[dbo].[udtUserID]  CONSTRAINT [DF_tblPoint_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate]									DATETIMEOFFSET (7) CONSTRAINT [DF_tblPoint_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy]										[dbo].[udtUserID]  CONSTRAINT [DF_tblPoint_UpdatedBy] DEFAULT ('') NOT NULL,
	[PointGuid]										UNIQUEIDENTIFIER	CONSTRAINT [DF_tblPoint_GUID] DEFAULT (newid()) NOT NULL,
	[_RowVersion]									ROWVERSION			NOT NULL,
	[SiteGuid]										UNIQUEIDENTIFIER	NOT NULL,
	[PointTemplateGuid]							UNIQUEIDENTIFIER	NULL,
	[ProfileImageGuid]							UNIQUEIDENTIFIER	NULL,
	[ProductGuid]				   				UNIQUEIDENTIFIER	NULL,
	[Notes]											NVARCHAR (255)		NULL,
	[OverrideDefaultDrawingGuid]				UNIQUEIDENTIFIER	NULL,
	[PointTemplateVersion]						int               NOT NULL,
 
   [_ClusterIdx]									BIGINT			    NOT NULL IDENTITY,
   CONSTRAINT [PK_tblPoint_GUID] PRIMARY KEY NONCLUSTERED ([PointGuid] ASC),
	CONSTRAINT [FK_tblPoint_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]), 
	CONSTRAINT [FK_tblPoint_PointTemplateGuid] FOREIGN KEY ([PointTemplateGuid]) REFERENCES [dbo].[tblPointTemplate] ([PointTemplateGuid]),
	CONSTRAINT [FK_tblPoint_ProfileImageGuid] FOREIGN KEY ([ProfileImageGuid]) REFERENCES [dbo].[tblPictures] ([PictureGuid]),
	CONSTRAINT [FK_tblPoint_ProductGuid] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
	CONSTRAINT [FK_tblPoint_OverrideDefaultDrawingGuid] FOREIGN KEY ([OverrideDefaultDrawingGuid]) REFERENCES [dbo].[tblDrawings] ([DrawingGuid]),
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPoint_ClusterIdx] 
	ON [dbo].[tblPoint]([_ClusterIdx]);

GO
CREATE NONCLUSTERED INDEX [IX_tblPoint_CreatedDate]
    ON [dbo].[tblPoint]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblPoint_SiteGuid]
    ON [dbo].[tblPoint]([SiteGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblPoint_ProductGuid]
    ON [dbo].[tblPoint]([ProductGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblPoint_ID_SiteGuid]
    ON [dbo].[tblPoint]([ID] ASC, [SiteGuid] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_tblPoint_Enabled_PointGuid_SiteGuid]
ON [dbo].[tblPoint] ([Enabled]) INCLUDE ([PointGuid],[SiteGuid]);
GO


CREATE NONCLUSTERED INDEX [IX_tblPoint_Enabled_PointGuid_RowVersion]
ON [dbo].[tblPoint] ([Enabled]) INCLUDE ([PointGuid],[_RowVersion]);
GO


CREATE NONCLUSTERED INDEX [IX_tblPoint_PointTemplateGuid]
    ON [dbo].[tblPoint]([PointTemplateGuid] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_tblPoint_PointGuid_RowVersion]
ON tblpoint ( PointGuid, _RowVersion ) INCLUDE ( SiteGuid )
GO
--Creating Insert / Update Trigger for tblPoint
CREATE TRIGGER dbo.trg_insupd_tblPoint_ForSync 
   ON dbo.tblPoint
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
                    ,d.PointGuid AS Deleted_PK_PointGuid
                    ,i.PointGuid AS Inserted_PK_PointGuid
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
				    d.PointGuid = i.PointGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblPoint As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_PointGuid = currentTrackingData.PK_PointGuid
 
 
		    INSERT track.tblPoint (InsertedDate 
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
				    ,PK_PointGuid
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
				    ,entityChanges.Inserted_PK_PointGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblPoint As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_PointGuid = currentTrackingData.PK_PointGuid
)
    END
END 
GO
--Creating Delete Trigger for tblPoint
CREATE TRIGGER dbo.trg_del_tblPoint_ForSync 
   ON dbo.tblPoint
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
						,d.PointGuid AS Deleted_PK_PointGuid
                        ,d.PointGuid AS Inserted_PK_PointGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblPoint As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_PointGuid = currentTrackingData.PK_PointGuid
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
						,PK_PointGuid
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
						,entityChanges.Deleted_PK_PointGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblPoint] ON [dbo].[tblPoint] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPoint','D')=1 
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
	INSERT INTO [fmaudit].tblPoint (
		[ID]
	,	[Description]
	,	[Enabled]
	,	[Standard]
	,	[ExecutionInterval]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[VelocityUnitIndex]
	,	[MassFlowUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[VelocityDecimalPlaces]
	,	[MassFlowDecimalPlaces]
	,	[LevelMaximum]
	,	[LevelMinimum]
	,	[TemperatureMaximum]
	,	[TemperatureMinimum]
	,	[DensityMaximum]
	,	[DensityMinimum]
	,	[PressureMaximum]
	,	[PressureMinimum]
	,	[VolumetricFlowMaximum]
	,	[VolumetricFlowMinimum]
	,	[VolumeMaximum]
	,	[VolumeMinimum]
	,	[MassMaximum]
	,	[MassMinimum]
	,	[VelocityMaximum]
	,	[VelocityMinimum]
	,	[MassFlowMaximum]
	,	[MassFlowMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PointGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PointTemplateGuid]
	,	[ProfileImageGuid]
	,	[ProductGuid]
	,	[Notes]
	,	[OverrideDefaultDrawingGuid]
	,	[PointTemplateVersion]
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
		d.[ID]
	,	d.[Description]
	,	d.[Enabled]
	,	d.[Standard]
	,	d.[ExecutionInterval]
	,	d.[LevelUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[PressureUnitIndex]
	,	d.[FlowUnitIndex]
	,	d.[VolumeUnitIndex]
	,	d.[MassUnitIndex]
	,	d.[VelocityUnitIndex]
	,	d.[MassFlowUnitIndex]
	,	d.[LevelDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[PressureDecimalPlaces]
	,	d.[FlowDecimalPlaces]
	,	d.[VolumeDecimalPlaces]
	,	d.[MassDecimalPlaces]
	,	d.[VelocityDecimalPlaces]
	,	d.[MassFlowDecimalPlaces]
	,	d.[LevelMaximum]
	,	d.[LevelMinimum]
	,	d.[TemperatureMaximum]
	,	d.[TemperatureMinimum]
	,	d.[DensityMaximum]
	,	d.[DensityMinimum]
	,	d.[PressureMaximum]
	,	d.[PressureMinimum]
	,	d.[VolumetricFlowMaximum]
	,	d.[VolumetricFlowMinimum]
	,	d.[VolumeMaximum]
	,	d.[VolumeMinimum]
	,	d.[MassMaximum]
	,	d.[MassMinimum]
	,	d.[VelocityMaximum]
	,	d.[VelocityMinimum]
	,	d.[MassFlowMaximum]
	,	d.[MassFlowMinimum]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[PointGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[PointTemplateGuid]
	,	d.[ProfileImageGuid]
	,	d.[ProductGuid]
	,	d.[Notes]
	,	d.[OverrideDefaultDrawingGuid]
	,	d.[PointTemplateVersion]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblPoint] ON [dbo].[tblPoint] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPoint','D')=1 
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
	INSERT INTO [fmaudit].tblPoint (
		[ID]
	,	[Description]
	,	[Enabled]
	,	[Standard]
	,	[ExecutionInterval]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[VelocityUnitIndex]
	,	[MassFlowUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[VelocityDecimalPlaces]
	,	[MassFlowDecimalPlaces]
	,	[LevelMaximum]
	,	[LevelMinimum]
	,	[TemperatureMaximum]
	,	[TemperatureMinimum]
	,	[DensityMaximum]
	,	[DensityMinimum]
	,	[PressureMaximum]
	,	[PressureMinimum]
	,	[VolumetricFlowMaximum]
	,	[VolumetricFlowMinimum]
	,	[VolumeMaximum]
	,	[VolumeMinimum]
	,	[MassMaximum]
	,	[MassMinimum]
	,	[VelocityMaximum]
	,	[VelocityMinimum]
	,	[MassFlowMaximum]
	,	[MassFlowMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PointGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PointTemplateGuid]
	,	[ProfileImageGuid]
	,	[ProductGuid]
	,	[Notes]
	,	[OverrideDefaultDrawingGuid]
	,	[PointTemplateVersion]
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
		i.[ID]
	,	i.[Description]
	,	i.[Enabled]
	,	i.[Standard]
	,	i.[ExecutionInterval]
	,	i.[LevelUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[PressureUnitIndex]
	,	i.[FlowUnitIndex]
	,	i.[VolumeUnitIndex]
	,	i.[MassUnitIndex]
	,	i.[VelocityUnitIndex]
	,	i.[MassFlowUnitIndex]
	,	i.[LevelDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[PressureDecimalPlaces]
	,	i.[FlowDecimalPlaces]
	,	i.[VolumeDecimalPlaces]
	,	i.[MassDecimalPlaces]
	,	i.[VelocityDecimalPlaces]
	,	i.[MassFlowDecimalPlaces]
	,	i.[LevelMaximum]
	,	i.[LevelMinimum]
	,	i.[TemperatureMaximum]
	,	i.[TemperatureMinimum]
	,	i.[DensityMaximum]
	,	i.[DensityMinimum]
	,	i.[PressureMaximum]
	,	i.[PressureMinimum]
	,	i.[VolumetricFlowMaximum]
	,	i.[VolumetricFlowMinimum]
	,	i.[VolumeMaximum]
	,	i.[VolumeMinimum]
	,	i.[MassMaximum]
	,	i.[MassMinimum]
	,	i.[VelocityMaximum]
	,	i.[VelocityMinimum]
	,	i.[MassFlowMaximum]
	,	i.[MassFlowMinimum]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[PointGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[PointTemplateGuid]
	,	i.[ProfileImageGuid]
	,	i.[ProductGuid]
	,	i.[Notes]
	,	i.[OverrideDefaultDrawingGuid]
	,	i.[PointTemplateVersion]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblPoint] ON [dbo].[tblPoint] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPoint','D')=1 
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
	PointGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblPoint (
		[ID]
	,	[Description]
	,	[Enabled]
	,	[Standard]
	,	[ExecutionInterval]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[VelocityUnitIndex]
	,	[MassFlowUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[VelocityDecimalPlaces]
	,	[MassFlowDecimalPlaces]
	,	[LevelMaximum]
	,	[LevelMinimum]
	,	[TemperatureMaximum]
	,	[TemperatureMinimum]
	,	[DensityMaximum]
	,	[DensityMinimum]
	,	[PressureMaximum]
	,	[PressureMinimum]
	,	[VolumetricFlowMaximum]
	,	[VolumetricFlowMinimum]
	,	[VolumeMaximum]
	,	[VolumeMinimum]
	,	[MassMaximum]
	,	[MassMinimum]
	,	[VelocityMaximum]
	,	[VelocityMinimum]
	,	[MassFlowMaximum]
	,	[MassFlowMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PointGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PointTemplateGuid]
	,	[ProfileImageGuid]
	,	[ProductGuid]
	,	[Notes]
	,	[OverrideDefaultDrawingGuid]
	,	[PointTemplateVersion]
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
	OUTPUT inserted.[PointGuid] AS 'PointGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ID]
	,	d.[Description]
	,	d.[Enabled]
	,	d.[Standard]
	,	d.[ExecutionInterval]
	,	d.[LevelUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[PressureUnitIndex]
	,	d.[FlowUnitIndex]
	,	d.[VolumeUnitIndex]
	,	d.[MassUnitIndex]
	,	d.[VelocityUnitIndex]
	,	d.[MassFlowUnitIndex]
	,	d.[LevelDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[PressureDecimalPlaces]
	,	d.[FlowDecimalPlaces]
	,	d.[VolumeDecimalPlaces]
	,	d.[MassDecimalPlaces]
	,	d.[VelocityDecimalPlaces]
	,	d.[MassFlowDecimalPlaces]
	,	d.[LevelMaximum]
	,	d.[LevelMinimum]
	,	d.[TemperatureMaximum]
	,	d.[TemperatureMinimum]
	,	d.[DensityMaximum]
	,	d.[DensityMinimum]
	,	d.[PressureMaximum]
	,	d.[PressureMinimum]
	,	d.[VolumetricFlowMaximum]
	,	d.[VolumetricFlowMinimum]
	,	d.[VolumeMaximum]
	,	d.[VolumeMinimum]
	,	d.[MassMaximum]
	,	d.[MassMinimum]
	,	d.[VelocityMaximum]
	,	d.[VelocityMinimum]
	,	d.[MassFlowMaximum]
	,	d.[MassFlowMinimum]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[PointGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[PointTemplateGuid]
	,	d.[ProfileImageGuid]
	,	d.[ProductGuid]
	,	d.[Notes]
	,	d.[OverrideDefaultDrawingGuid]
	,	d.[PointTemplateVersion]
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
 
	INSERT INTO [fmaudit].tblPoint (
		[ID]
	,	[Description]
	,	[Enabled]
	,	[Standard]
	,	[ExecutionInterval]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[VelocityUnitIndex]
	,	[MassFlowUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[VelocityDecimalPlaces]
	,	[MassFlowDecimalPlaces]
	,	[LevelMaximum]
	,	[LevelMinimum]
	,	[TemperatureMaximum]
	,	[TemperatureMinimum]
	,	[DensityMaximum]
	,	[DensityMinimum]
	,	[PressureMaximum]
	,	[PressureMinimum]
	,	[VolumetricFlowMaximum]
	,	[VolumetricFlowMinimum]
	,	[VolumeMaximum]
	,	[VolumeMinimum]
	,	[MassMaximum]
	,	[MassMinimum]
	,	[VelocityMaximum]
	,	[VelocityMinimum]
	,	[MassFlowMaximum]
	,	[MassFlowMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PointGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PointTemplateGuid]
	,	[ProfileImageGuid]
	,	[ProductGuid]
	,	[Notes]
	,	[OverrideDefaultDrawingGuid]
	,	[PointTemplateVersion]
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
		i.[ID]
	,	i.[Description]
	,	i.[Enabled]
	,	i.[Standard]
	,	i.[ExecutionInterval]
	,	i.[LevelUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[PressureUnitIndex]
	,	i.[FlowUnitIndex]
	,	i.[VolumeUnitIndex]
	,	i.[MassUnitIndex]
	,	i.[VelocityUnitIndex]
	,	i.[MassFlowUnitIndex]
	,	i.[LevelDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[PressureDecimalPlaces]
	,	i.[FlowDecimalPlaces]
	,	i.[VolumeDecimalPlaces]
	,	i.[MassDecimalPlaces]
	,	i.[VelocityDecimalPlaces]
	,	i.[MassFlowDecimalPlaces]
	,	i.[LevelMaximum]
	,	i.[LevelMinimum]
	,	i.[TemperatureMaximum]
	,	i.[TemperatureMinimum]
	,	i.[DensityMaximum]
	,	i.[DensityMinimum]
	,	i.[PressureMaximum]
	,	i.[PressureMinimum]
	,	i.[VolumetricFlowMaximum]
	,	i.[VolumetricFlowMinimum]
	,	i.[VolumeMaximum]
	,	i.[VolumeMinimum]
	,	i.[MassMaximum]
	,	i.[MassMinimum]
	,	i.[VelocityMaximum]
	,	i.[VelocityMinimum]
	,	i.[MassFlowMaximum]
	,	i.[MassFlowMinimum]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[PointGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[PointTemplateGuid]
	,	i.[ProfileImageGuid]
	,	i.[ProductGuid]
	,	i.[Notes]
	,	i.[OverrideDefaultDrawingGuid]
	,	i.[PointTemplateVersion]
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
			agl.[PointGuid]=i.[PointGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END