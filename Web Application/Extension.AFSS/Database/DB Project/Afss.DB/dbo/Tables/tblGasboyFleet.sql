CREATE TABLE [dbo].[tblGasboyFleet]
(
	[GasboyFleetGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL, 
	[FleetCode] BIGINT NOT NULL,
	[FleetName] NVARCHAR(50) NOT NULL,
	[GroupRuleName] NVARCHAR(50) NULL,
	[PriceListName] NVARCHAR(50) NULL,
	[LookupGasboyRecordStatusIndex] INT NOT NULL,
	[UsePINCodeFlag] BIT CONSTRAINT [DF_tblGasboyFleet_UsePINCodeFlag] DEFAULT (0) NOT NULL,
	[PINCode] VARBINARY(256) NOT NULL,
	[AuthPINFrom] TINYINT CONSTRAINT [DF_tblGasboyFleet_AuthPINFrom] DEFAULT (2) NOT NULL,
	[PromptForVehiclePlateFlag] BIT NOT NULL,
	[LookupGasboyVehiclePlateCheckTypeIndex] INT NOT NULL,
	[AlwaysPromptForAdditionalValidationFlag] TINYINT NOT NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL, 
	[CreatedDate] DATETIMEOFFSET NOT NULL, 
	[UpdatedBy] [dbo].[udtUserID] NOT NULL, 
	[UpdatedDate] DATETIMEOFFSET NOT NULL, 
	[_RowVersion] TIMESTAMP NOT NULL,   
	[_ClusterIdx] BIGINT IDENTITY(1,1) NOT NULL, 
	[FleetID] BIGINT NOT NULL,
	CONSTRAINT [PK_tblGasboyFleet] PRIMARY KEY NONCLUSTERED ([GasboyFleetGuid]), 
	CONSTRAINT [FK_tblGasboyFleet_SiteGuid] FOREIGN KEY (SiteGuid) REFERENCES [dbo].[tblSites]([SiteGuid]), 
	CONSTRAINT [FK_tblGasboyFleet_LookupGasboyRecordStatusIndex] FOREIGN KEY ([LookupGasboyRecordStatusIndex]) REFERENCES [lookup].[tblGasboyRecordStatus]([GasboyRecordStatusIndex]), 
	CONSTRAINT [FK_tblGasboyFleet_LookupGasboyVehiclePlateCheckTypeIndex] FOREIGN KEY (LookupGasboyVehiclePlateCheckTypeIndex) REFERENCES [lookup].[tblGasboyVehiclePlateCheckType]([GasboyVehiclePlateCheckTypeIndex]) 
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblGasboyFleet__ClusterIdx] ON [dbo].[tblGasboyFleet] (_ClusterIdx)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_SiteGuid] ON [dbo].[tblGasboyFleet] (SiteGuid)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_FleetID] ON [dbo].[tblGasboyFleet] (FleetID)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_FleetName] ON [dbo].[tblGasboyFleet] (FleetName)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_LookupGasboyRecordStatusIndex] ON [dbo].[tblGasboyFleet] (LookupGasboyRecordStatusIndex)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyFleet_LookupGasboyVehiclePlateCheckTypeIndex] ON [dbo].[tblGasboyFleet] (LookupGasboyVehiclePlateCheckTypeIndex)
GO
 
-------------------------------------
-- AUDIT DELETE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_del_tblGasboyFleet] ON [dbo].[tblGasboyFleet] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyFleet','D')=1 
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
	INSERT INTO [fmaudit].tblGasboyFleet (
		[GasboyFleetGuid]
	,	[SiteGuid]
	,	[FleetCode]
	,	[FleetName]
	,	[GroupRuleName]
	,	[PriceListName]
	,	[LookupGasboyRecordStatusIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[FleetID]
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
		d.[GasboyFleetGuid]
	,	d.[SiteGuid]
	,	d.[FleetCode]
	,	d.[FleetName]
	,	d.[GroupRuleName]
	,	d.[PriceListName]
	,	d.[LookupGasboyRecordStatusIndex]
	,	d.[UsePINCodeFlag]
	,	d.[PINCode]
	,	d.[AuthPINFrom]
	,	d.[PromptForVehiclePlateFlag]
	,	d.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	d.[AlwaysPromptForAdditionalValidationFlag]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[_RowVersion]
	,	d.[FleetID]
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
 
CREATE TRIGGER [dbo].[trg_Audit_ins_tblGasboyFleet] ON [dbo].[tblGasboyFleet] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyFleet','D')=1 
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
	INSERT INTO [fmaudit].tblGasboyFleet (
		[GasboyFleetGuid]
	,	[SiteGuid]
	,	[FleetCode]
	,	[FleetName]
	,	[GroupRuleName]
	,	[PriceListName]
	,	[LookupGasboyRecordStatusIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[FleetID]
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
		i.[GasboyFleetGuid]
	,	i.[SiteGuid]
	,	i.[FleetCode]
	,	i.[FleetName]
	,	i.[GroupRuleName]
	,	i.[PriceListName]
	,	i.[LookupGasboyRecordStatusIndex]
	,	i.[UsePINCodeFlag]
	,	i.[PINCode]
	,	i.[AuthPINFrom]
	,	i.[PromptForVehiclePlateFlag]
	,	i.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	i.[AlwaysPromptForAdditionalValidationFlag]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[_RowVersion]
	,	i.[FleetID]
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
 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblGasboyFleet] ON [dbo].[tblGasboyFleet] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyFleet','D')=1 
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
	GasboyFleetGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblGasboyFleet (
		[GasboyFleetGuid]
	,	[SiteGuid]
	,	[FleetCode]
	,	[FleetName]
	,	[GroupRuleName]
	,	[PriceListName]
	,	[LookupGasboyRecordStatusIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[FleetID]
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
	OUTPUT inserted.[GasboyFleetGuid] AS 'GasboyFleetGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[GasboyFleetGuid]
	,	d.[SiteGuid]
	,	d.[FleetCode]
	,	d.[FleetName]
	,	d.[GroupRuleName]
	,	d.[PriceListName]
	,	d.[LookupGasboyRecordStatusIndex]
	,	d.[UsePINCodeFlag]
	,	d.[PINCode]
	,	d.[AuthPINFrom]
	,	d.[PromptForVehiclePlateFlag]
	,	d.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	d.[AlwaysPromptForAdditionalValidationFlag]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[_RowVersion]
	,	d.[FleetID]
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
 
	INSERT INTO [fmaudit].tblGasboyFleet (
		[GasboyFleetGuid]
	,	[SiteGuid]
	,	[FleetCode]
	,	[FleetName]
	,	[GroupRuleName]
	,	[PriceListName]
	,	[LookupGasboyRecordStatusIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[FleetID]
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
		i.[GasboyFleetGuid]
	,	i.[SiteGuid]
	,	i.[FleetCode]
	,	i.[FleetName]
	,	i.[GroupRuleName]
	,	i.[PriceListName]
	,	i.[LookupGasboyRecordStatusIndex]
	,	i.[UsePINCodeFlag]
	,	i.[PINCode]
	,	i.[AuthPINFrom]
	,	i.[PromptForVehiclePlateFlag]
	,	i.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	i.[AlwaysPromptForAdditionalValidationFlag]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[_RowVersion]
	,	i.[FleetID]
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
			agl.[GasboyFleetGuid]=i.[GasboyFleetGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO

--Creating Insert / Update Trigger for tblGasboyFleet
CREATE TRIGGER dbo.trg_insupd_tblGasboyFleet_ForSync 
   ON dbo.tblGasboyFleet
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
                   ,d.GasboyFleetGuid AS Deleted_PK_GasboyFleetGuid
                    ,i.GasboyFleetGuid AS Inserted_PK_GasboyFleetGuid
                    ,NULL AS Deleted_FK_ParentPK 
                    ,NULL AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,NULL AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.GasboyFleetGuid = i.GasboyFleetGuid
           ) 
		    MERGE INTO track.tblGasboyFleet WITH (HOLDLOCK) As currentTrackingData 
			    USING ChangeList As entityChanges 
				    ON entityChanges.Inserted_PK_GasboyFleetGuid = currentTrackingData.PK_GasboyFleetGuid
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
				    ,PK_GasboyFleetGuid
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
				    ,entityChanges.Inserted_PK_GasboyFleetGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    );
    END
END
GO
--Creating Delete Trigger for tblGasboyFleet
CREATE TRIGGER dbo.trg_del_tblGasboyFleet_ForSync 
   ON dbo.tblGasboyFleet
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
						,d.GasboyFleetGuid AS Deleted_PK_GasboyFleetGuid
                        ,d.GasboyFleetGuid AS Inserted_PK_GasboyFleetGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblGasboyFleet WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_GasboyFleetGuid = currentTrackingData.PK_GasboyFleetGuid
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
						,PK_GasboyFleetGuid
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
						,entityChanges.Deleted_PK_GasboyFleetGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END