CREATE TABLE [dbo].[tblStations] (
    [ID]                                  NVARCHAR (50)      CONSTRAINT [DF_tblStations_ID] DEFAULT ('') NOT NULL,
    [SwingArmPosition]                    BIT                CONSTRAINT [DF_tblStations_SwingArmPosition] DEFAULT ((0)) NOT NULL,
    [VaporRecovery]                       BIT                CONSTRAINT [DF_tblStations_VaporRecovery] DEFAULT ((0)) NOT NULL,
    [Enabled]                             BIT                CONSTRAINT [DF_tblStations_Enabled] DEFAULT ((0)) NOT NULL,
    [BOLPrinter]                          NVARCHAR (80)      NULL,
    [PreloadPrinter]                      NVARCHAR (80)      CONSTRAINT [DF_tblStations_PreloadPrinter] DEFAULT ('') NOT NULL,
    [BOLAgeInMinutes]                     INT                CONSTRAINT [DF_tblStations_BOLAgeInMinutes] DEFAULT ((0)) NOT NULL,
    [CardReader]                          BIT                CONSTRAINT [DF_tblStations_CardReader] DEFAULT ((0)) NOT NULL,
    [ThirtyFiveBitCardSupport]            BIT                CONSTRAINT [DF_tblStations_ThirtyFiveBitCardSupport] DEFAULT ((0)) NOT NULL,
    [NumberOfCopies]                      INT                NULL,
    [NumberOfPreloadCopies]               INT                NULL,
    [InhibitLoadingByLoadID]              BIT                NULL,
    [InhibitOperatingModePrompt]          BIT                NULL,
    [SynchronizeReferenceDensity]         BIT                NULL,
    [SignatureDevice]                     NVARCHAR (20)      NULL,
    [SetDefaultPresetToZero]              BIT                NULL,
    [ArmsServiced]                        NVARCHAR (100)     NULL,
    [InhibitSettingRecipeNames]           BIT                NULL,
    [SignatureDevicePort]                 INT                NULL,
    [SignatureDeviceBaudRate]             INT                NULL,
    [MeterRecircCardNumber]               NVARCHAR (30)      NULL,
    [TouchKeyReader]                      BIT                NULL,
    [OffLoadByOffLoadID]                  BIT                NULL,
    [UseManualMeterData]                  BIT                NULL,
    [PromptForBOLNumber]                  BIT                NULL,
	[QueryForTrailers]					  BIT                NULL,
	[PromptForGravityCaptured]            BIT				 CONSTRAINT [DF_tblStations_PromptForGravityCaptured] DEFAULT ((0)) NOT NULL,
    [PromptForTemperatureCaptured]        BIT				 CONSTRAINT [DF_tblStations_PromptForTemperatureCaptured] DEFAULT ((0)) NOT NULL,
    [LastTransactionNumber]               INT                NULL,
    [LastTransactionNumberDateTime]       DATETIMEOFFSET (7) NULL,
    [CreatedDate]                         DATETIMEOFFSET (7) CONSTRAINT [DF_tblStations_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                           [dbo].[udtUserID]  CONSTRAINT [DF_tblStations_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                         DATETIMEOFFSET (7) CONSTRAINT [DF_tblStations_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                           [dbo].[udtUserID]  CONSTRAINT [DF_tblStations_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                         ROWVERSION         NOT NULL,
    [StationGuid]                         UNIQUEIDENTIFIER   CONSTRAINT [DF_tblStations_GUID] DEFAULT (NEWID()) NOT NULL,
    [SiteGuid]                            UNIQUEIDENTIFIER   NOT NULL,
    [LookupStationTypeIndex]              INT                CONSTRAINT [DF_tblStations_LookupStationTypeIndex] DEFAULT ((0)) NOT NULL,
    [LookupStationInterfaceTypeIndex]     INT                CONSTRAINT [DF_tblStations_LookupStationInterfaceTypeIndex] DEFAULT ((0)) NOT NULL,
    [TankGuid]                            UNIQUEIDENTIFIER   NULL,
    [IssueByVolumeTransactionAliasGuid]   UNIQUEIDENTIFIER   NULL,
    [IssueByWeightTransactionAliasGuid]   UNIQUEIDENTIFIER   NULL,
    [ReceiptByVolumeTransactionAliasGuid] UNIQUEIDENTIFIER   NULL,
    [ReceiptByWeightTransactionAliasGuid] UNIQUEIDENTIFIER   NULL,
    [RecircTransactionAliasGuid]          UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                         BIGINT             IDENTITY (1, 1) NOT NULL,
    [LogCommunications] BIT NULL, 
    [LogCommPath] NVARCHAR(255) NULL, 
	[EnableScully]									BIT				 CONSTRAINT [DF_tblStations_EnableScully] DEFAULT ((0)) NULL,
	[EnableEquipmentValidate]					BIT				 CONSTRAINT [DF_tblStations_EnableEquipmentValidate] DEFAULT ((1)) NULL,
    [StationPromptTimeout]						INT NULL, 
    [StationMessageTimeout]					INT NULL, 
    [AssignedMeterGuid]							UNIQUEIDENTIFIER NULL, 
    [EnableDynamicRecipes]						BIT                CONSTRAINT [DF_tblStations_EnableDynamicRecipes] DEFAULT ((0)) NOT NULL,
    [EthanolExcess]								BIT                CONSTRAINT [DF_tblStations_EthanolExcess] DEFAULT ((0)) NOT NULL,
	 CONSTRAINT [PK_tblStations_GUID] PRIMARY KEY NONCLUSTERED ([StationGuid] ASC),
    CONSTRAINT [FK_tblStations_IssueByVolumeTransactionAliasGuid] FOREIGN KEY ([IssueByVolumeTransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblStations_IssueByWeightTransactionAliasGuid] FOREIGN KEY ([IssueByWeightTransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblStations_LookupStationInterfaceTypeIndex] FOREIGN KEY ([LookupStationInterfaceTypeIndex]) REFERENCES [lookup].[tblStationInterfaceType] ([StationInterfaceTypeIndex]),
    CONSTRAINT [FK_tblStations_LookupStationTypeIndex] FOREIGN KEY ([LookupStationTypeIndex]) REFERENCES [lookup].[tblStationType] ([StationTypeIndex]),
    CONSTRAINT [FK_tblStations_ReceiptByVolumeTransactionAliasGuid] FOREIGN KEY ([ReceiptByVolumeTransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblStations_ReceiptByWeightTransactionAliasGuid] FOREIGN KEY ([ReceiptByWeightTransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblStations_RecircTransactionAliasGuid] FOREIGN KEY ([RecircTransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblStations_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblStations_TankGuid] FOREIGN KEY ([TankGuid]) REFERENCES [dbo].[tblTanks] ([TankGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblStations_CreatedDate]
    ON [dbo].[tblStations]([CreatedDate] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblStations_ID_SiteGuid]
    ON [dbo].[tblStations]([ID] ASC, [SiteGuid] ASC);


GO
--Creating Insert / Update Trigger for tblStations
CREATE TRIGGER dbo.trg_insupd_tblStations_ForSync 
   ON dbo.tblStations
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
                    ,d.StationGuid AS Deleted_PK_StationGuid
                    ,i.StationGuid AS Inserted_PK_StationGuid
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
				    d.StationGuid = i.StationGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblStations As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_StationGuid = currentTrackingData.PK_StationGuid
 
 
		    INSERT track.tblStations (InsertedDate 
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
				    ,PK_StationGuid
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
				    ,entityChanges.Inserted_PK_StationGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblStations As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_StationGuid = currentTrackingData.PK_StationGuid
)
    END
END 

GO
--Creating Delete Trigger for tblStations
CREATE TRIGGER dbo.trg_del_tblStations_ForSync 
   ON dbo.tblStations
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
						,d.StationGuid AS Deleted_PK_StationGuid
                        ,d.StationGuid AS Inserted_PK_StationGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblStations As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_StationGuid = currentTrackingData.PK_StationGuid
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
						,PK_StationGuid
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
						,entityChanges.Deleted_PK_StationGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblStations] ON [dbo].[tblStations] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblStations','D')=1 
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
	INSERT INTO [fmaudit].tblStations (
		[ID]
	,	[SwingArmPosition]
	,	[VaporRecovery]
	,	[Enabled]
	,	[BOLPrinter]
	,	[PreloadPrinter]
	,	[BOLAgeInMinutes]
	,	[CardReader]
	,	[ThirtyFiveBitCardSupport]
	,	[NumberOfCopies]
	,	[NumberOfPreloadCopies]
	,	[InhibitLoadingByLoadID]
	,	[InhibitOperatingModePrompt]
	,	[SynchronizeReferenceDensity]
	,	[SignatureDevice]
	,	[SetDefaultPresetToZero]
	,	[ArmsServiced]
	,	[InhibitSettingRecipeNames]
	,	[SignatureDevicePort]
	,	[SignatureDeviceBaudRate]
	,	[MeterRecircCardNumber]
	,	[TouchKeyReader]
	,	[OffLoadByOffLoadID]
	,	[UseManualMeterData]
	,	[PromptForBOLNumber]
	,	[QueryForTrailers]
	,	[PromptForGravityCaptured]
	,	[PromptForTemperatureCaptured]
	,	[LastTransactionNumber]
	,	[LastTransactionNumberDateTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[StationGuid]
	,	[SiteGuid]
	,	[LookupStationTypeIndex]
	,	[LookupStationInterfaceTypeIndex]
	,	[TankGuid]
	,	[IssueByVolumeTransactionAliasGuid]
	,	[IssueByWeightTransactionAliasGuid]
	,	[ReceiptByVolumeTransactionAliasGuid]
	,	[ReceiptByWeightTransactionAliasGuid]
	,	[RecircTransactionAliasGuid]
	,	[LogCommunications]
	,	[LogCommPath]
	,	[EnableScully]
	,	[EnableEquipmentValidate]
	,	[StationPromptTimeout]
	,	[StationMessageTimeout]
	,	[AssignedMeterGuid]
	,  [EnableDynamicRecipes]
	,	[EthanolExcess]
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
	,	i.[SwingArmPosition]
	,	i.[VaporRecovery]
	,	i.[Enabled]
	,	i.[BOLPrinter]
	,	i.[PreloadPrinter]
	,	i.[BOLAgeInMinutes]
	,	i.[CardReader]
	,	i.[ThirtyFiveBitCardSupport]
	,	i.[NumberOfCopies]
	,	i.[NumberOfPreloadCopies]
	,	i.[InhibitLoadingByLoadID]
	,	i.[InhibitOperatingModePrompt]
	,	i.[SynchronizeReferenceDensity]
	,	i.[SignatureDevice]
	,	i.[SetDefaultPresetToZero]
	,	i.[ArmsServiced]
	,	i.[InhibitSettingRecipeNames]
	,	i.[SignatureDevicePort]
	,	i.[SignatureDeviceBaudRate]
	,	i.[MeterRecircCardNumber]
	,	i.[TouchKeyReader]
	,	i.[OffLoadByOffLoadID]
	,	i.[UseManualMeterData]
	,	i.[PromptForBOLNumber]
	,	i.[QueryForTrailers]
	,	i.[PromptForGravityCaptured]
	,	i.[PromptForTemperatureCaptured]
	,	i.[LastTransactionNumber]
	,	i.[LastTransactionNumberDateTime]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[StationGuid]
	,	i.[SiteGuid]
	,	i.[LookupStationTypeIndex]
	,	i.[LookupStationInterfaceTypeIndex]
	,	i.[TankGuid]
	,	i.[IssueByVolumeTransactionAliasGuid]
	,	i.[IssueByWeightTransactionAliasGuid]
	,	i.[ReceiptByVolumeTransactionAliasGuid]
	,	i.[ReceiptByWeightTransactionAliasGuid]
	,	i.[RecircTransactionAliasGuid]
	,	i.[LogCommunications]
	,	i.[LogCommPath]
	,	i.[EnableScully]
	,	i.[EnableEquipmentValidate]
	,	i.[StationPromptTimeout]
	,	i.[StationMessageTimeout]
	,	i.[AssignedMeterGuid]
	,  i.[EnableDynamicRecipes]
	,	i.[EthanolExcess]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblStations] ON [dbo].[tblStations] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblStations','D')=1 
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
	StationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblStations (
		[ID]
	,	[SwingArmPosition]
	,	[VaporRecovery]
	,	[Enabled]
	,	[BOLPrinter]
	,	[PreloadPrinter]
	,	[BOLAgeInMinutes]
	,	[CardReader]
	,	[ThirtyFiveBitCardSupport]
	,	[NumberOfCopies]
	,	[NumberOfPreloadCopies]
	,	[InhibitLoadingByLoadID]
	,	[InhibitOperatingModePrompt]
	,	[SynchronizeReferenceDensity]
	,	[SignatureDevice]
	,	[SetDefaultPresetToZero]
	,	[ArmsServiced]
	,	[InhibitSettingRecipeNames]
	,	[SignatureDevicePort]
	,	[SignatureDeviceBaudRate]
	,	[MeterRecircCardNumber]
	,	[TouchKeyReader]
	,	[OffLoadByOffLoadID]
	,	[UseManualMeterData]
	,	[PromptForBOLNumber]
	,	[QueryForTrailers]
	,	[PromptForGravityCaptured]
	,	[PromptForTemperatureCaptured]
	,	[LastTransactionNumber]
	,	[LastTransactionNumberDateTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[StationGuid]
	,	[SiteGuid]
	,	[LookupStationTypeIndex]
	,	[LookupStationInterfaceTypeIndex]
	,	[TankGuid]
	,	[IssueByVolumeTransactionAliasGuid]
	,	[IssueByWeightTransactionAliasGuid]
	,	[ReceiptByVolumeTransactionAliasGuid]
	,	[ReceiptByWeightTransactionAliasGuid]
	,	[RecircTransactionAliasGuid]
	,	[LogCommunications]
	,	[LogCommPath]
	,	[EnableScully]
	,	[EnableEquipmentValidate]
	,	[StationPromptTimeout]
	,	[StationMessageTimeout]
	,	[AssignedMeterGuid]
	,  [EnableDynamicRecipes]
	,	[EthanolExcess]	
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
	OUTPUT inserted.[StationGuid] AS 'StationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ID]
	,	d.[SwingArmPosition]
	,	d.[VaporRecovery]
	,	d.[Enabled]
	,	d.[BOLPrinter]
	,	d.[PreloadPrinter]
	,	d.[BOLAgeInMinutes]
	,	d.[CardReader]
	,	d.[ThirtyFiveBitCardSupport]
	,	d.[NumberOfCopies]
	,	d.[NumberOfPreloadCopies]
	,	d.[InhibitLoadingByLoadID]
	,	d.[InhibitOperatingModePrompt]
	,	d.[SynchronizeReferenceDensity]
	,	d.[SignatureDevice]
	,	d.[SetDefaultPresetToZero]
	,	d.[ArmsServiced]
	,	d.[InhibitSettingRecipeNames]
	,	d.[SignatureDevicePort]
	,	d.[SignatureDeviceBaudRate]
	,	d.[MeterRecircCardNumber]
	,	d.[TouchKeyReader]
	,	d.[OffLoadByOffLoadID]
	,	d.[UseManualMeterData]
	,	d.[PromptForBOLNumber]
	,	d.[QueryForTrailers]
	,	d.[PromptForGravityCaptured]
	,	d.[PromptForTemperatureCaptured]
	,	d.[LastTransactionNumber]
	,	d.[LastTransactionNumberDateTime]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[StationGuid]
	,	d.[SiteGuid]
	,	d.[LookupStationTypeIndex]
	,	d.[LookupStationInterfaceTypeIndex]
	,	d.[TankGuid]
	,	d.[IssueByVolumeTransactionAliasGuid]
	,	d.[IssueByWeightTransactionAliasGuid]
	,	d.[ReceiptByVolumeTransactionAliasGuid]
	,	d.[ReceiptByWeightTransactionAliasGuid]
	,	d.[RecircTransactionAliasGuid]
	,	d.[LogCommunications]
	,	d.[LogCommPath]
	,	d.[EnableScully]
	,	d.[EnableEquipmentValidate]
	,	d.[StationPromptTimeout]
	,	d.[StationMessageTimeout]
	,	d.[AssignedMeterGuid]
	,  d.[EnableDynamicRecipes]
	,	d.[EthanolExcess]
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
 
	INSERT INTO [fmaudit].tblStations (
		[ID]
	,	[SwingArmPosition]
	,	[VaporRecovery]
	,	[Enabled]
	,	[BOLPrinter]
	,	[PreloadPrinter]
	,	[BOLAgeInMinutes]
	,	[CardReader]
	,	[ThirtyFiveBitCardSupport]
	,	[NumberOfCopies]
	,	[NumberOfPreloadCopies]
	,	[InhibitLoadingByLoadID]
	,	[InhibitOperatingModePrompt]
	,	[SynchronizeReferenceDensity]
	,	[SignatureDevice]
	,	[SetDefaultPresetToZero]
	,	[ArmsServiced]
	,	[InhibitSettingRecipeNames]
	,	[SignatureDevicePort]
	,	[SignatureDeviceBaudRate]
	,	[MeterRecircCardNumber]
	,	[TouchKeyReader]
	,	[OffLoadByOffLoadID]
	,	[UseManualMeterData]
	,	[PromptForBOLNumber]
	,	[QueryForTrailers]
	,	[PromptForGravityCaptured]
	,	[PromptForTemperatureCaptured]
	,	[LastTransactionNumber]
	,	[LastTransactionNumberDateTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[StationGuid]
	,	[SiteGuid]
	,	[LookupStationTypeIndex]
	,	[LookupStationInterfaceTypeIndex]
	,	[TankGuid]
	,	[IssueByVolumeTransactionAliasGuid]
	,	[IssueByWeightTransactionAliasGuid]
	,	[ReceiptByVolumeTransactionAliasGuid]
	,	[ReceiptByWeightTransactionAliasGuid]
	,	[RecircTransactionAliasGuid]
	,	[LogCommunications]
	,	[LogCommPath]
	,	[EnableScully]
	,	[EnableEquipmentValidate]
	,	[StationPromptTimeout]
	,	[StationMessageTimeout]
	,	[AssignedMeterGuid]
	,  [EnableDynamicRecipes]
	,	[EthanolExcess]	
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
	,	i.[SwingArmPosition]
	,	i.[VaporRecovery]
	,	i.[Enabled]
	,	i.[BOLPrinter]
	,	i.[PreloadPrinter]
	,	i.[BOLAgeInMinutes]
	,	i.[CardReader]
	,	i.[ThirtyFiveBitCardSupport]
	,	i.[NumberOfCopies]
	,	i.[NumberOfPreloadCopies]
	,	i.[InhibitLoadingByLoadID]
	,	i.[InhibitOperatingModePrompt]
	,	i.[SynchronizeReferenceDensity]
	,	i.[SignatureDevice]
	,	i.[SetDefaultPresetToZero]
	,	i.[ArmsServiced]
	,	i.[InhibitSettingRecipeNames]
	,	i.[SignatureDevicePort]
	,	i.[SignatureDeviceBaudRate]
	,	i.[MeterRecircCardNumber]
	,	i.[TouchKeyReader]
	,	i.[OffLoadByOffLoadID]
	,	i.[UseManualMeterData]
	,	i.[PromptForBOLNumber]
	,	i.[QueryForTrailers]
	,	i.[PromptForGravityCaptured]
	,	i.[PromptForTemperatureCaptured]
	,	i.[LastTransactionNumber]
	,	i.[LastTransactionNumberDateTime]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[StationGuid]
	,	i.[SiteGuid]
	,	i.[LookupStationTypeIndex]
	,	i.[LookupStationInterfaceTypeIndex]
	,	i.[TankGuid]
	,	i.[IssueByVolumeTransactionAliasGuid]
	,	i.[IssueByWeightTransactionAliasGuid]
	,	i.[ReceiptByVolumeTransactionAliasGuid]
	,	i.[ReceiptByWeightTransactionAliasGuid]
	,	i.[RecircTransactionAliasGuid]
	,	i.[LogCommunications]
	,	i.[LogCommPath]
	,	i.[EnableScully]
	,	i.[EnableEquipmentValidate]
	,	i.[StationPromptTimeout]
	,	i.[StationMessageTimeout]
	,	i.[AssignedMeterGuid]
	,  i.[EnableDynamicRecipes]
	,	i.[EthanolExcess]	
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
			agl.[StationGuid]=i.[StationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblStations] ON [dbo].[tblStations] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblStations','D')=1 
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
	INSERT INTO [fmaudit].tblStations (
		[ID]
	,	[SwingArmPosition]
	,	[VaporRecovery]
	,	[Enabled]
	,	[BOLPrinter]
	,	[PreloadPrinter]
	,	[BOLAgeInMinutes]
	,	[CardReader]
	,	[ThirtyFiveBitCardSupport]
	,	[NumberOfCopies]
	,	[NumberOfPreloadCopies]
	,	[InhibitLoadingByLoadID]
	,	[InhibitOperatingModePrompt]
	,	[SynchronizeReferenceDensity]
	,	[SignatureDevice]
	,	[SetDefaultPresetToZero]
	,	[ArmsServiced]
	,	[InhibitSettingRecipeNames]
	,	[SignatureDevicePort]
	,	[SignatureDeviceBaudRate]
	,	[MeterRecircCardNumber]
	,	[TouchKeyReader]
	,	[OffLoadByOffLoadID]
	,	[UseManualMeterData]
	,	[PromptForBOLNumber]
	,	[QueryForTrailers]
	,	[PromptForGravityCaptured]
	,	[PromptForTemperatureCaptured]
	,	[LastTransactionNumber]
	,	[LastTransactionNumberDateTime]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[StationGuid]
	,	[SiteGuid]
	,	[LookupStationTypeIndex]
	,	[LookupStationInterfaceTypeIndex]
	,	[TankGuid]
	,	[IssueByVolumeTransactionAliasGuid]
	,	[IssueByWeightTransactionAliasGuid]
	,	[ReceiptByVolumeTransactionAliasGuid]
	,	[ReceiptByWeightTransactionAliasGuid]
	,	[RecircTransactionAliasGuid]
	,	[LogCommunications]
	,	[LogCommPath]
	,	[EnableScully]
	,	[EnableEquipmentValidate]
	,	[StationPromptTimeout]
	,	[StationMessageTimeout]
	,	[AssignedMeterGuid]
	,  [EnableDynamicRecipes]
	,	[EthanolExcess]
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
	,	d.[SwingArmPosition]
	,	d.[VaporRecovery]
	,	d.[Enabled]
	,	d.[BOLPrinter]
	,	d.[PreloadPrinter]
	,	d.[BOLAgeInMinutes]
	,	d.[CardReader]
	,	d.[ThirtyFiveBitCardSupport]
	,	d.[NumberOfCopies]
	,	d.[NumberOfPreloadCopies]
	,	d.[InhibitLoadingByLoadID]
	,	d.[InhibitOperatingModePrompt]
	,	d.[SynchronizeReferenceDensity]
	,	d.[SignatureDevice]
	,	d.[SetDefaultPresetToZero]
	,	d.[ArmsServiced]
	,	d.[InhibitSettingRecipeNames]
	,	d.[SignatureDevicePort]
	,	d.[SignatureDeviceBaudRate]
	,	d.[MeterRecircCardNumber]
	,	d.[TouchKeyReader]
	,	d.[OffLoadByOffLoadID]
	,	d.[UseManualMeterData]
	,	d.[PromptForBOLNumber]
	,	d.[QueryForTrailers]
	,	d.[PromptForGravityCaptured]
	,	d.[PromptForTemperatureCaptured]
	,	d.[LastTransactionNumber]
	,	d.[LastTransactionNumberDateTime]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[StationGuid]
	,	d.[SiteGuid]
	,	d.[LookupStationTypeIndex]
	,	d.[LookupStationInterfaceTypeIndex]
	,	d.[TankGuid]
	,	d.[IssueByVolumeTransactionAliasGuid]
	,	d.[IssueByWeightTransactionAliasGuid]
	,	d.[ReceiptByVolumeTransactionAliasGuid]
	,	d.[ReceiptByWeightTransactionAliasGuid]
	,	d.[RecircTransactionAliasGuid]
	,	d.[LogCommunications]
	,	d.[LogCommPath]
	,	d.[EnableScully]
	,	d.[EnableEquipmentValidate]
	,	d.[StationPromptTimeout]
	,	d.[StationMessageTimeout]
	,	d.[AssignedMeterGuid]
	,  d.[EnableDynamicRecipes]
	,	d.[EthanolExcess]
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


CREATE TRIGGER [dbo].[trg_fmcdc_tblStations]
ON [dbo].[tblStations]
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
		INSERT INTO fmcdc.[tblStations]
		(
		[ID]
		, [SwingArmPosition]
		, [VaporRecovery]
		, [Enabled]
		, [BOLPrinter]
		, [PreloadPrinter]
		, [BOLAgeInMinutes]
		, [CardReader]
		, [ThirtyFiveBitCardSupport]
		, [NumberOfCopies]
		, [NumberOfPreloadCopies]
		, [InhibitLoadingByLoadID]
		, [InhibitOperatingModePrompt]
		, [SynchronizeReferenceDensity]
		, [SignatureDevice]
		, [SetDefaultPresetToZero]
		, [ArmsServiced]
		, [InhibitSettingRecipeNames]
		, [SignatureDevicePort]
		, [SignatureDeviceBaudRate]
		, [MeterRecircCardNumber]
		, [TouchKeyReader]
		, [OffLoadByOffLoadID]
		, [UseManualMeterData]
		, [PromptForBOLNumber]
		, [QueryForTrailers]
		, [PromptForGravityCaptured]
		, [PromptForTemperatureCaptured]
		, [LastTransactionNumber]
		, [LastTransactionNumberDateTime]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [SourceRowVersion]
		, [StationGuid]
		, [SiteGuid]
		, [LookupStationTypeIndex]
		, [LookupStationInterfaceTypeIndex]
		, [TankGuid]
		, [IssueByVolumeTransactionAliasGuid]
		, [IssueByWeightTransactionAliasGuid]
		, [ReceiptByVolumeTransactionAliasGuid]
		, [ReceiptByWeightTransactionAliasGuid]
		, [RecircTransactionAliasGuid]
		, [_ClusterIdx]
		, [LogCommunications]
		, [LogCommPath]
		, [EnableScully]
		, [EnableEquipmentValidate]
		, [StationPromptTimeout]
		, [StationMessageTimeout]
		, [AssignedMeterGuid]
		, [EnableDynamicRecipes]
		, [EthanolExcess]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ID]
		, [SwingArmPosition]
		, [VaporRecovery]
		, [Enabled]
		, [BOLPrinter]
		, [PreloadPrinter]
		, [BOLAgeInMinutes]
		, [CardReader]
		, [ThirtyFiveBitCardSupport]
		, [NumberOfCopies]
		, [NumberOfPreloadCopies]
		, [InhibitLoadingByLoadID]
		, [InhibitOperatingModePrompt]
		, [SynchronizeReferenceDensity]
		, [SignatureDevice]
		, [SetDefaultPresetToZero]
		, [ArmsServiced]
		, [InhibitSettingRecipeNames]
		, [SignatureDevicePort]
		, [SignatureDeviceBaudRate]
		, [MeterRecircCardNumber]
		, [TouchKeyReader]
		, [OffLoadByOffLoadID]
		, [UseManualMeterData]
		, [PromptForBOLNumber]
		, [QueryForTrailers]
		, [PromptForGravityCaptured]
		, [PromptForTemperatureCaptured]
		, [LastTransactionNumber]
		, [LastTransactionNumberDateTime]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, CONVERT(bigint, _RowVersion)
		, [StationGuid]
		, [SiteGuid]
		, [LookupStationTypeIndex]
		, [LookupStationInterfaceTypeIndex]
		, [TankGuid]
		, [IssueByVolumeTransactionAliasGuid]
		, [IssueByWeightTransactionAliasGuid]
		, [ReceiptByVolumeTransactionAliasGuid]
		, [ReceiptByWeightTransactionAliasGuid]
		, [RecircTransactionAliasGuid]
		, [_ClusterIdx]
		, [LogCommunications]
		, [LogCommPath]
		, [EnableScully]
		, [EnableEquipmentValidate]
		, [StationPromptTimeout]
		, [StationMessageTimeout]
		, [AssignedMeterGuid]
		, [EnableDynamicRecipes]
		, [EthanolExcess]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblStations]
		(
		[ID]
		, [SwingArmPosition]
		, [VaporRecovery]
		, [Enabled]
		, [BOLPrinter]
		, [PreloadPrinter]
		, [BOLAgeInMinutes]
		, [CardReader]
		, [ThirtyFiveBitCardSupport]
		, [NumberOfCopies]
		, [NumberOfPreloadCopies]
		, [InhibitLoadingByLoadID]
		, [InhibitOperatingModePrompt]
		, [SynchronizeReferenceDensity]
		, [SignatureDevice]
		, [SetDefaultPresetToZero]
		, [ArmsServiced]
		, [InhibitSettingRecipeNames]
		, [SignatureDevicePort]
		, [SignatureDeviceBaudRate]
		, [MeterRecircCardNumber]
		, [TouchKeyReader]
		, [OffLoadByOffLoadID]
		, [UseManualMeterData]
		, [PromptForBOLNumber]
		, [QueryForTrailers]
		, [PromptForGravityCaptured]
		, [PromptForTemperatureCaptured]
		, [LastTransactionNumber]
		, [LastTransactionNumberDateTime]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [SourceRowVersion]
		, [StationGuid]
		, [SiteGuid]
		, [LookupStationTypeIndex]
		, [LookupStationInterfaceTypeIndex]
		, [TankGuid]
		, [IssueByVolumeTransactionAliasGuid]
		, [IssueByWeightTransactionAliasGuid]
		, [ReceiptByVolumeTransactionAliasGuid]
		, [ReceiptByWeightTransactionAliasGuid]
		, [RecircTransactionAliasGuid]
		, [_ClusterIdx]
		, [LogCommunications]
		, [LogCommPath]
		, [EnableScully]
		, [EnableEquipmentValidate]
		, [StationPromptTimeout]
		, [StationMessageTimeout]
		, [AssignedMeterGuid]
		, [EnableDynamicRecipes]
		, [EthanolExcess]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ID]
		, [SwingArmPosition]
		, [VaporRecovery]
		, [Enabled]
		, [BOLPrinter]
		, [PreloadPrinter]
		, [BOLAgeInMinutes]
		, [CardReader]
		, [ThirtyFiveBitCardSupport]
		, [NumberOfCopies]
		, [NumberOfPreloadCopies]
		, [InhibitLoadingByLoadID]
		, [InhibitOperatingModePrompt]
		, [SynchronizeReferenceDensity]
		, [SignatureDevice]
		, [SetDefaultPresetToZero]
		, [ArmsServiced]
		, [InhibitSettingRecipeNames]
		, [SignatureDevicePort]
		, [SignatureDeviceBaudRate]
		, [MeterRecircCardNumber]
		, [TouchKeyReader]
		, [OffLoadByOffLoadID]
		, [UseManualMeterData]
		, [PromptForBOLNumber]
		, [QueryForTrailers]
		, [PromptForGravityCaptured]
		, [PromptForTemperatureCaptured]
		, [LastTransactionNumber]
		, [LastTransactionNumberDateTime]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, CONVERT(bigint, _RowVersion)
		, [StationGuid]
		, [SiteGuid]
		, [LookupStationTypeIndex]
		, [LookupStationInterfaceTypeIndex]
		, [TankGuid]
		, [IssueByVolumeTransactionAliasGuid]
		, [IssueByWeightTransactionAliasGuid]
		, [ReceiptByVolumeTransactionAliasGuid]
		, [ReceiptByWeightTransactionAliasGuid]
		, [RecircTransactionAliasGuid]
		, [_ClusterIdx]
		, [LogCommunications]
		, [LogCommPath]
		, [EnableScully]
		, [EnableEquipmentValidate]
		, [StationPromptTimeout]
		, [StationMessageTimeout]
		, [AssignedMeterGuid]
		, [EnableDynamicRecipes]
		, [EthanolExcess]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblStations] ON [dbo].[tblStations]
GO



CREATE UNIQUE CLUSTERED INDEX [IX_tblStations_ClusterIdx]
    ON [dbo].[tblStations]([_ClusterIdx] ASC);

