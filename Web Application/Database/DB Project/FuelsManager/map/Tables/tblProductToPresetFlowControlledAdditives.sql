CREATE TABLE [map].[tblProductToPresetFlowControlledAdditive] (
    [ProductToPresetFlowControlledAdditiveGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_GUID] DEFAULT (newid()) NOT NULL,
    [ProductGuid]                          UNIQUEIDENTIFIER   NOT NULL,
    [AssignedToLoadArmGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [Sequence]                             INT                NOT NULL,
    [BlendPercentage]                      FLOAT (53)         CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_BlendPercentage] DEFAULT ((0.0)) NOT NULL,
    [AdditiveRate]                         FLOAT (53)         CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_AdditiveRate] DEFAULT ((0.0)) NOT NULL,
    [Ratio]                                FLOAT (53)         CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_Ratio] DEFAULT ((0.0)) NOT NULL,
    [AdditiveCycleVolume]                  FLOAT (53)         CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_AdditiveCycleVolume] DEFAULT ((0.0)) NOT NULL,
    [Tolerance]                            FLOAT (53)         CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_Tolerance] DEFAULT ((0.0)) NOT NULL,
    [PresetNumber]                         INT                CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_PresetNumber] DEFAULT ((0)) NOT NULL,
    [AdditiveProfileGuid]                  UNIQUEIDENTIFIER   NULL,
    [TankGuid]                             UNIQUEIDENTIFIER   NULL,
    [MeterID]                              NVARCHAR (20)      NULL,
    [ShipToProductID]                      NVARCHAR (30)      NULL,
    [ShipToProductCode]                    NVARCHAR (15)      NULL,
    [ShipToLoadRackDisplayText]            NVARCHAR (10)      NULL,
    [UnavailableInventoryGross]            FLOAT (53)         NULL,
    [UnavailableInventoryNet]              FLOAT (53)         NULL,
    [CreatedDate]                          DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                            [dbo].[udtUserID]  CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                          DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                            [dbo].[udtUserID]  CONSTRAINT [DF_map_tblProductToPresetFlowControlledAdditive_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                          ROWVERSION         NOT NULL,
    [AssignedToMeterGuid]                  UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_map_tblProductToPresetFlowControlledAdditive_GUID] PRIMARY KEY NONCLUSTERED ([ProductToPresetFlowControlledAdditiveGuid] ASC),
    CONSTRAINT [FK_map_tblProductToPresetFlowControlledAdditive_AdditiveProfileGuid] FOREIGN KEY ([AdditiveProfileGuid]) REFERENCES [dbo].[tblAdditiveProfiles] ([AdditiveProfileGuid]),
    CONSTRAINT [FK_map_tblProductToPresetFlowControlledAdditive_AssignedToLoadArmGuid] FOREIGN KEY ([AssignedToLoadArmGuid]) REFERENCES [dbo].[tblLoadArms] ([LoadArmGuid]),
    CONSTRAINT [FK_map_tblProductToPresetFlowControlledAdditive_ProductIndex] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
    CONSTRAINT [FK_map_tblProductToPresetFlowControlledAdditive_TankGuid] FOREIGN KEY ([TankGuid]) REFERENCES [dbo].[tblTanks] ([TankGuid]),
    CONSTRAINT [FK_tblProductToPresetFlowControlledAdditive_MeterGuid] FOREIGN KEY ([AssignedToMeterGuid]) REFERENCES [dbo].[tblMeter] ([MeterGuid])
);






GO



GO
CREATE NONCLUSTERED INDEX [IX_map_tblProductToPresetFlowControlledAdditive_AdditiveProfileGuid]
    ON [map].[tblProductToPresetFlowControlledAdditive]([AdditiveProfileGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblProductToPresetFlowControlledAdditive_AssignedToLoadArmGuid]
    ON [map].[tblProductToPresetFlowControlledAdditive]([AssignedToLoadArmGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblProductToPresetFlowControlledAdditive_ProductGuid]
    ON [map].[tblProductToPresetFlowControlledAdditive]([ProductGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblProductToPresetFlowControlledAdditive_TankGuid]
    ON [map].[tblProductToPresetFlowControlledAdditive]([TankGuid] ASC);
	GO

CREATE NONCLUSTERED INDEX [IX_tblProductToPresetFlowControlledAdditive_AssignedToMeterGuid]
    ON [map].[tblProductToPresetFlowControlledAdditive]([AssignedToMeterGuid] ASC)
    INCLUDE([AssignedToLoadArmGuid]);



GO
--Creating Insert / Update Trigger for tblProductToPresetFlowControlledAdditive
CREATE TRIGGER map.trg_insupd_tblProductToPresetFlowControlledAdditive_ForSync 
   ON map.tblProductToPresetFlowControlledAdditive
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
                    ,d.ProductToPresetFlowControlledAdditiveGuid AS Deleted_PK_ProductToPresetFlowControlledAdditiveGuid
                    ,i.ProductToPresetFlowControlledAdditiveGuid AS Inserted_PK_ProductToPresetFlowControlledAdditiveGuid
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
				    d.ProductToPresetFlowControlledAdditiveGuid = i.ProductToPresetFlowControlledAdditiveGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblProductToPresetFlowControlledAdditive As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ProductToPresetFlowControlledAdditiveGuid = currentTrackingData.PK_ProductToPresetFlowControlledAdditiveGuid
 
 
		    INSERT track.tblProductToPresetFlowControlledAdditive (InsertedDate 
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
				    ,PK_ProductToPresetFlowControlledAdditiveGuid
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
				    ,entityChanges.Inserted_PK_ProductToPresetFlowControlledAdditiveGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblProductToPresetFlowControlledAdditive As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ProductToPresetFlowControlledAdditiveGuid = currentTrackingData.PK_ProductToPresetFlowControlledAdditiveGuid
)
    END
END 

GO
--Creating Delete Trigger for tblProductToPresetFlowControlledAdditive
CREATE TRIGGER map.trg_del_tblProductToPresetFlowControlledAdditive_ForSync 
   ON map.tblProductToPresetFlowControlledAdditive
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
						,d.ProductToPresetFlowControlledAdditiveGuid AS Deleted_PK_ProductToPresetFlowControlledAdditiveGuid
                        ,d.ProductToPresetFlowControlledAdditiveGuid AS Inserted_PK_ProductToPresetFlowControlledAdditiveGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblProductToPresetFlowControlledAdditive As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ProductToPresetFlowControlledAdditiveGuid = currentTrackingData.PK_ProductToPresetFlowControlledAdditiveGuid
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
						,PK_ProductToPresetFlowControlledAdditiveGuid
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
						,entityChanges.Deleted_PK_ProductToPresetFlowControlledAdditiveGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [map].[trg_Audit_del_tblProductToPresetFlowControlledAdditive] ON [map].[tblProductToPresetFlowControlledAdditive] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblProductToPresetFlowControlledAdditive','D')=1 
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
	INSERT INTO [fmaudit].map_tblProductToPresetFlowControlledAdditive (
		[ProductToPresetFlowControlledAdditiveGuid]
	,	[ProductGuid]
	,	[AssignedToLoadArmGuid]
	,	[Sequence]
	,	[BlendPercentage]
	,	[AdditiveRate]
	,	[Ratio]
	,	[AdditiveCycleVolume]
	,	[Tolerance]
	,	[PresetNumber]
	,	[AdditiveProfileGuid]
	,	[TankGuid]
	,	[MeterID]
	,	[ShipToProductID]
	,	[ShipToProductCode]
	,	[ShipToLoadRackDisplayText]
	,	[UnavailableInventoryGross]
	,	[UnavailableInventoryNet]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AssignedToMeterGuid]
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
		d.[ProductToPresetFlowControlledAdditiveGuid]
	,	d.[ProductGuid]
	,	d.[AssignedToLoadArmGuid]
	,	d.[Sequence]
	,	d.[BlendPercentage]
	,	d.[AdditiveRate]
	,	d.[Ratio]
	,	d.[AdditiveCycleVolume]
	,	d.[Tolerance]
	,	d.[PresetNumber]
	,	d.[AdditiveProfileGuid]
	,	d.[TankGuid]
	,	d.[MeterID]
	,	d.[ShipToProductID]
	,	d.[ShipToProductCode]
	,	d.[ShipToLoadRackDisplayText]
	,	d.[UnavailableInventoryGross]
	,	d.[UnavailableInventoryNet]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[AssignedToMeterGuid]
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
CREATE TRIGGER [map].[trg_Audit_ins_tblProductToPresetFlowControlledAdditive] ON [map].[tblProductToPresetFlowControlledAdditive] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblProductToPresetFlowControlledAdditive','D')=1 
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
	INSERT INTO [fmaudit].map_tblProductToPresetFlowControlledAdditive (
		[ProductToPresetFlowControlledAdditiveGuid]
	,	[ProductGuid]
	,	[AssignedToLoadArmGuid]
	,	[Sequence]
	,	[BlendPercentage]
	,	[AdditiveRate]
	,	[Ratio]
	,	[AdditiveCycleVolume]
	,	[Tolerance]
	,	[PresetNumber]
	,	[AdditiveProfileGuid]
	,	[TankGuid]
	,	[MeterID]
	,	[ShipToProductID]
	,	[ShipToProductCode]
	,	[ShipToLoadRackDisplayText]
	,	[UnavailableInventoryGross]
	,	[UnavailableInventoryNet]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AssignedToMeterGuid]
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
		i.[ProductToPresetFlowControlledAdditiveGuid]
	,	i.[ProductGuid]
	,	i.[AssignedToLoadArmGuid]
	,	i.[Sequence]
	,	i.[BlendPercentage]
	,	i.[AdditiveRate]
	,	i.[Ratio]
	,	i.[AdditiveCycleVolume]
	,	i.[Tolerance]
	,	i.[PresetNumber]
	,	i.[AdditiveProfileGuid]
	,	i.[TankGuid]
	,	i.[MeterID]
	,	i.[ShipToProductID]
	,	i.[ShipToProductCode]
	,	i.[ShipToLoadRackDisplayText]
	,	i.[UnavailableInventoryGross]
	,	i.[UnavailableInventoryNet]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[AssignedToMeterGuid]
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
CREATE TRIGGER [map].[trg_Audit_upd_tblProductToPresetFlowControlledAdditive] ON [map].[tblProductToPresetFlowControlledAdditive] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblProductToPresetFlowControlledAdditive','D')=1 
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
	ProductToPresetFlowControlledAdditiveGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblProductToPresetFlowControlledAdditive (
		[ProductToPresetFlowControlledAdditiveGuid]
	,	[ProductGuid]
	,	[AssignedToLoadArmGuid]
	,	[Sequence]
	,	[BlendPercentage]
	,	[AdditiveRate]
	,	[Ratio]
	,	[AdditiveCycleVolume]
	,	[Tolerance]
	,	[PresetNumber]
	,	[AdditiveProfileGuid]
	,	[TankGuid]
	,	[MeterID]
	,	[ShipToProductID]
	,	[ShipToProductCode]
	,	[ShipToLoadRackDisplayText]
	,	[UnavailableInventoryGross]
	,	[UnavailableInventoryNet]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AssignedToMeterGuid]
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
	OUTPUT inserted.[ProductToPresetFlowControlledAdditiveGuid] AS 'ProductToPresetFlowControlledAdditiveGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ProductToPresetFlowControlledAdditiveGuid]
	,	d.[ProductGuid]
	,	d.[AssignedToLoadArmGuid]
	,	d.[Sequence]
	,	d.[BlendPercentage]
	,	d.[AdditiveRate]
	,	d.[Ratio]
	,	d.[AdditiveCycleVolume]
	,	d.[Tolerance]
	,	d.[PresetNumber]
	,	d.[AdditiveProfileGuid]
	,	d.[TankGuid]
	,	d.[MeterID]
	,	d.[ShipToProductID]
	,	d.[ShipToProductCode]
	,	d.[ShipToLoadRackDisplayText]
	,	d.[UnavailableInventoryGross]
	,	d.[UnavailableInventoryNet]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[AssignedToMeterGuid]
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
 
	INSERT INTO [fmaudit].map_tblProductToPresetFlowControlledAdditive (
		[ProductToPresetFlowControlledAdditiveGuid]
	,	[ProductGuid]
	,	[AssignedToLoadArmGuid]
	,	[Sequence]
	,	[BlendPercentage]
	,	[AdditiveRate]
	,	[Ratio]
	,	[AdditiveCycleVolume]
	,	[Tolerance]
	,	[PresetNumber]
	,	[AdditiveProfileGuid]
	,	[TankGuid]
	,	[MeterID]
	,	[ShipToProductID]
	,	[ShipToProductCode]
	,	[ShipToLoadRackDisplayText]
	,	[UnavailableInventoryGross]
	,	[UnavailableInventoryNet]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[AssignedToMeterGuid]
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
		i.[ProductToPresetFlowControlledAdditiveGuid]
	,	i.[ProductGuid]
	,	i.[AssignedToLoadArmGuid]
	,	i.[Sequence]
	,	i.[BlendPercentage]
	,	i.[AdditiveRate]
	,	i.[Ratio]
	,	i.[AdditiveCycleVolume]
	,	i.[Tolerance]
	,	i.[PresetNumber]
	,	i.[AdditiveProfileGuid]
	,	i.[TankGuid]
	,	i.[MeterID]
	,	i.[ShipToProductID]
	,	i.[ShipToProductCode]
	,	i.[ShipToLoadRackDisplayText]
	,	i.[UnavailableInventoryGross]
	,	i.[UnavailableInventoryNet]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[AssignedToMeterGuid]
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
			agl.[ProductToPresetFlowControlledAdditiveGuid]=i.[ProductToPresetFlowControlledAdditiveGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE NONCLUSTERED INDEX [IX_tblProductToPresetFlowControlledAdditive_CreatedDate]
    ON [map].[tblProductToPresetFlowControlledAdditive]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblProductToPresetFlowControlledAdditive_ClusterIdx]
    ON [map].[tblProductToPresetFlowControlledAdditive]([_ClusterIdx] ASC);

