CREATE TABLE [dbo].[tblTanks] (
    [TankID]                  NVARCHAR (50)      CONSTRAINT [DF_tblTanks_TankID] DEFAULT ('') NOT NULL,
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblTanks_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblTanks_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblTanks_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblTanks_UpdatedBy] DEFAULT ('') NOT NULL,
    [TankGuid]                UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTanks_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [SiteGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [LookupVesselTypeIndex]   INT                NOT NULL,
    [ManagerCompanyGuid]      UNIQUEIDENTIFIER   NULL,
    [ProductGuid]             UNIQUEIDENTIFIER   NULL,
    [HiddenDate]              DATETIMEOFFSET (7) NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
	[AssetTrackingDeviceGuid] UNIQUEIDENTIFIER   NULL,
	[LookupDeviceTankTypeIndex]    INT                NULL,
	[Latitude]                FLOAT              NULL,
	[Longitude]              FLOAT              NULL,
	[TankConfigurationNumber]  INT NULL,
	[Zoom] INT NULL,
    [OwnerCompanyGuid] UNIQUEIDENTIFIER NULL, 
    CONSTRAINT [PK_tblTanks_GUID] PRIMARY KEY NONCLUSTERED ([TankGuid] ASC),
    CONSTRAINT [FK_tblTanks_LookupVesselTypeIndex] FOREIGN KEY ([LookupVesselTypeIndex]) REFERENCES [lookup].[tblVesselType] ([VesselTypeIndex]),
    CONSTRAINT [FK_tblTanks_LookupDeviceTankTypeIndex] FOREIGN KEY ([LookupDeviceTankTypeIndex]) REFERENCES [lookup].[tblDeviceTankType] ([DeviceTankTypeIndex]),
    CONSTRAINT [FK_tblTanks_ManagerCompanyGuid] FOREIGN KEY ([ManagerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblTanks_ProductGuid] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
    CONSTRAINT [FK_tblTanks_AssetTrackingDeviceGuid] FOREIGN KEY ([AssetTrackingDeviceGuid]) REFERENCES [dbo].[tblAssetTrackingDevice] ([AssetTrackingDeviceGuid]),
    CONSTRAINT [FK_tblTanks_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblTanks_OwnerCompanyGuid] FOREIGN KEY ([OwnerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid])
);

GO
CREATE NONCLUSTERED INDEX [IX_tblTanks_CreatedDate]
    ON [dbo].[tblTanks]([CreatedDate] ASC);

GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTanks_TankID_SiteGuid]
    ON [dbo].[tblTanks]([TankID] ASC, [SiteGuid] ASC);

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblTanks] ON [dbo].[tblTanks] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTanks','D')=1 
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
	INSERT INTO [fmaudit].tblTanks (
		[TankID]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TankGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupVesselTypeIndex]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
	,	[HiddenDate]
	,	[AssetTrackingDeviceGuid]
	,	[LookupDeviceTankTypeIndex]
	,	[Latitude]
	,	[Longitude]
	,	[TankConfigurationNumber]
	,	[Zoom]
	,	[OwnerCompanyGuid]
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
		d.[TankID]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TankGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupVesselTypeIndex]
	,	d.[ManagerCompanyGuid]
	,	d.[ProductGuid]
	,	d.[HiddenDate]
	,	d.[AssetTrackingDeviceGuid]
	,	d.[LookupDeviceTankTypeIndex]
	,	d.[Latitude]
	,	d.[Longitude]
	,	d.[TankConfigurationNumber]
	,	d.[Zoom]
	,	d.[OwnerCompanyGuid]
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
--Creating Insert / Update Trigger for tblTanks
CREATE TRIGGER dbo.trg_insupd_tblTanks_ForSync 
   ON dbo.tblTanks
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
                    ,d.TankGuid AS Deleted_PK_TankGuid
                    ,i.TankGuid AS Inserted_PK_TankGuid
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
				    d.TankGuid = i.TankGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTanks As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TankGuid = currentTrackingData.PK_TankGuid
 
 
		    INSERT track.tblTanks (InsertedDate 
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
				    ,PK_TankGuid
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
				    ,entityChanges.Inserted_PK_TankGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTanks As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TankGuid = currentTrackingData.PK_TankGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTanks
CREATE TRIGGER dbo.trg_del_tblTanks_ForSync 
   ON dbo.tblTanks
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
						,d.TankGuid AS Deleted_PK_TankGuid
                        ,d.TankGuid AS Inserted_PK_TankGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTanks As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TankGuid = currentTrackingData.PK_TankGuid
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
						,PK_TankGuid
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
						,entityChanges.Deleted_PK_TankGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTanks] ON [dbo].[tblTanks] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTanks','D')=1 
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
	INSERT INTO [fmaudit].tblTanks (
		[TankID]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TankGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupVesselTypeIndex]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
	,	[HiddenDate]
	,	[AssetTrackingDeviceGuid]
	,	[LookupDeviceTankTypeIndex]
	,	[Latitude]
	,	[Longitude]
	,	[TankConfigurationNumber]
	,	[Zoom]
	,	[OwnerCompanyGuid]
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
		i.[TankID]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TankGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupVesselTypeIndex]
	,	i.[ManagerCompanyGuid]
	,	i.[ProductGuid]
	,	i.[HiddenDate]
	,	i.[AssetTrackingDeviceGuid]
	,	i.[LookupDeviceTankTypeIndex]
	,	i.[Latitude]
	,	i.[Longitude]
	,	i.[TankConfigurationNumber]
	,	i.[Zoom]
	,	i.[OwnerCompanyGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTanks] ON [dbo].[tblTanks] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTanks','D')=1 
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
	TankGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTanks (
		[TankID]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TankGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupVesselTypeIndex]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
	,	[HiddenDate]
	,	[AssetTrackingDeviceGuid]
	,	[LookupDeviceTankTypeIndex]
	,	[Latitude]
	,	[Longitude]
	,	[TankConfigurationNumber]
	,	[Zoom]
	,	[OwnerCompanyGuid]
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
	OUTPUT inserted.[TankGuid] AS 'TankGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[TankID]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TankGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupVesselTypeIndex]
	,	d.[ManagerCompanyGuid]
	,	d.[ProductGuid]
	,	d.[HiddenDate]
	,	d.[AssetTrackingDeviceGuid]
	,	d.[LookupDeviceTankTypeIndex]
	,	d.[Latitude]
	,	d.[Longitude]
	,	d.[TankConfigurationNumber]
	,	d.[Zoom]
	,	d.[OwnerCompanyGuid]
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
 
	INSERT INTO [fmaudit].tblTanks (
		[TankID]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TankGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupVesselTypeIndex]
	,	[ManagerCompanyGuid]
	,	[ProductGuid]
	,	[HiddenDate]
	,	[AssetTrackingDeviceGuid]
	,	[LookupDeviceTankTypeIndex]
	,	[Latitude]
	,	[Longitude]
	,	[TankConfigurationNumber]
	,	[Zoom]
	,	[OwnerCompanyGuid]
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
		i.[TankID]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TankGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupVesselTypeIndex]
	,	i.[ManagerCompanyGuid]
	,	i.[ProductGuid]
	,	i.[HiddenDate]
	,	i.[AssetTrackingDeviceGuid]
	,	i.[LookupDeviceTankTypeIndex]
	,	i.[Latitude]
	,	i.[Longitude]
	,	i.[TankConfigurationNumber]
	,	i.[Zoom]
	,	i.[OwnerCompanyGuid]
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
			agl.[TankGuid]=i.[TankGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblTanks]
ON [dbo].[tblTanks]
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
		INSERT INTO fmcdc.[tblTanks]
		(
		[TankID]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TankGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [LookupVesselTypeIndex]
		, [ManagerCompanyGuid]
		, [ProductGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [AssetTrackingDeviceGuid]
		, [LookupDeviceTankTypeIndex]
		, [Latitude]
		, [Longitude]
		, [TankConfigurationNumber]
		, [Zoom]
		, [OwnerCompanyGuid]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[TankID]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TankGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [LookupVesselTypeIndex]
		, [ManagerCompanyGuid]
		, [ProductGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [AssetTrackingDeviceGuid]
		, [LookupDeviceTankTypeIndex]
		, [Latitude]
		, [Longitude]
		, [TankConfigurationNumber]
		, [Zoom]
		, [OwnerCompanyGuid]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTanks]
		(
		[TankID]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TankGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [LookupVesselTypeIndex]
		, [ManagerCompanyGuid]
		, [ProductGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [AssetTrackingDeviceGuid]
		, [LookupDeviceTankTypeIndex]
		, [Latitude]
		, [Longitude]
		, [TankConfigurationNumber]
		, [Zoom]
		, [OwnerCompanyGuid]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[TankID]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [TankGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [LookupVesselTypeIndex]
		, [ManagerCompanyGuid]
		, [ProductGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [AssetTrackingDeviceGuid]
		, [LookupDeviceTankTypeIndex]
		, [Latitude]
		, [Longitude]
		, [TankConfigurationNumber]
		, [Zoom]
		, [OwnerCompanyGuid]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTanks] ON [dbo].[tblTanks]
GO



CREATE NONCLUSTERED INDEX IX_tblTanks_ManagerCompanyGuid ON dbo.tblTanks(ManagerCompanyGuid)
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTanks_ClusterIdx]
    ON [dbo].[tblTanks]([_ClusterIdx] ASC);

