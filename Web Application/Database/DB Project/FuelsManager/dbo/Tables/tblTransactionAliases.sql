CREATE TABLE [dbo].[tblTransactionAliases] (
    [AliasName]                              NVARCHAR (32)      CONSTRAINT [DF_tblTransactionAliases_AliasName] DEFAULT ('') NOT NULL,
    [MeterCloseout]                          BIT                CONSTRAINT [DF_tblTransactionAliases_MeterCloseout] DEFAULT ((0)) NOT NULL,
    [BulkShipment]                           BIT                CONSTRAINT [DF_tblTransactionAliases_BulkShipment] DEFAULT ((0)) NOT NULL,
    [DistributedImpact]                      BIT                CONSTRAINT [DF_tblTransactionAliases_DistributedImpact] DEFAULT ((0)) NOT NULL,
    [MultipleLineItems]                      BIT                CONSTRAINT [DF_tblTransactionAliases_MultipleLineItems] DEFAULT ((0)) NOT NULL,
    [LimitSelectionsBasedOnHierarchy]        BIT                NULL,
    [LineItemEditControl]                    BIT                CONSTRAINT [DF_tblTransactionAliases_LineItemEditControl] DEFAULT ((0)) NOT NULL,
    [MultipleWeightReadings]                 BIT                CONSTRAINT [DF_tblTransactionAliases_MultipleWeightReadings] DEFAULT ((0)) NOT NULL,
    [WeightReadingEditControl]               BIT                CONSTRAINT [DF_tblTransactionAliases_WeightReadingEditControl] DEFAULT ((0)) NOT NULL,
    [AssociatedReport]                       NVARCHAR (80)      NULL,
    [AssociatedPreloadReport]                NVARCHAR (80)      NULL,
    [DestinationEquipmentTypes1]             BIGINT             NULL,
    [DestinationEquipmentTypes2]             BIGINT             NULL,
    [DestinationEquipmentTypes3]             BIGINT             NULL,
    [SourceEquipmentTypes1]                  BIGINT             NULL,
    [SourceEquipmentTypes2]                  BIGINT             NULL,
    [SourceEquipmentTypes3]                  BIGINT             NULL,
    [CreatedDate]                            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionAliases_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                              [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionAliases_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionAliases_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                              [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionAliases_UpdatedBy] DEFAULT ('') NOT NULL,
    [ShowCompanyName]                        SMALLINT           NULL,
    [AggregateAssocTrans]                    BIT                NULL,
    [EnableTotalQuantityExceededWarning]     BIT                NULL,
    [EnableQuantityToleranceExceededWarning] BIT                NULL,
    [EnableTotalValueExceededWarning]        BIT                NULL,
    [EnableValueToleranceExceededWarning]    BIT                NULL,
    [LevelUnitIndex]                         INT                NULL,
    [TemperatureUnitIndex]                   INT                NULL,
    [DensityUnitIndex]                       INT                NULL,
    [PressureUnitIndex]                      INT                NULL,
    [FlowUnitIndex]                          INT                NULL,
    [VolumeUnitIndex]                        INT                NULL,
    [MassUnitIndex]                          INT                NULL,
    [AdditiveVolumeUnitIndex]                INT                NULL,
    [AdditiveProfileCycleAmountUnitIndex]    INT                NULL,
    [AdditiveProfileRateUnitIndex]           INT                NULL,
    [LevelDecimalPlaces]                     TINYINT            NULL,
    [TemperatureDecimalPlaces]               TINYINT            NULL,
    [DensityDecimalPlaces]                   TINYINT            NULL,
    [PressureDecimalPlaces]                  TINYINT            NULL,
    [FlowDecimalPlaces]                      TINYINT            NULL,
    [VolumeDecimalPlaces]                    TINYINT            NULL,
    [MassDecimalPlaces]                      TINYINT            NULL,
    [AdditiveVolumeDecimalPlaces]            TINYINT            NULL,
    [UseComboBoxControls]                    BIT                NULL,
    [MultipleTransportLineItems]             BIT                CONSTRAINT [DF_tblTransactionAliases_MultipleTransportLineItems] DEFAULT ((0)) NULL,
    [TransactionAliasGuid]                   UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionAliases_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                            ROWVERSION         NOT NULL,
    [SiteGuid]                               UNIQUEIDENTIFIER   NOT NULL,
    [LookupTransTypeIndex]                   SMALLINT           CONSTRAINT [DF_tblTransactionAliases_LookupTransTypeIndex] DEFAULT ((0)) NOT NULL,
    [LookupDefaultStatusIndex]               INT                CONSTRAINT [DF_tblTransactionAliases_LookupDefaultStatusIndex] DEFAULT ((0)) NOT NULL,
    [AssociatedTransactionAliasGuid]         UNIQUEIDENTIFIER   NULL,
    [IncludeInDispatch]                      BIT                CONSTRAINT [DF_tblTransactionAliases_IncludeInDispatch] DEFAULT ((0)) NOT NULL,
    [_MasterRecordGuid]                      UNIQUEIDENTIFIER   NOT NULL,
    [EnableAutoCompleteControls]             BIT                NULL,
    [PermitNonReferenceData]                 BIT                NULL,
    [_ClusterIdx]                            BIGINT             IDENTITY (1, 1) NOT NULL,
	[UseTransactionDetailWithLayout]		 BIT                CONSTRAINT [DF_tblTransactionAliases_UseTransactionDetailWithLayout] DEFAULT ((0)) NOT NULL,
	[DefaultMeterToEquipmentID]				 BIT                CONSTRAINT [DF_tblTransactionAliases_DefaultMeterToEquipmentID] DEFAULT ((0)) NOT NULL,
	[LimitSourceEquipmentByProduct]			 BIT                CONSTRAINT [DF_tblTransactionAliases_LimitSourceEquipmentByProduct] DEFAULT ((0)) NOT NULL,
	[RememberMeterEndForMeterID]			 BIT                CONSTRAINT [DF_tblTransactionAliases_RememberMeterEndForMeterID] DEFAULT ((0)) NOT NULL,
	[PopulateCompaniesFromEquipment]		 BIT                CONSTRAINT [DF_tblTransactionAliases_PopulateCompaniesFromEquipment] DEFAULT ((0)) NOT NULL,
	[PopulateGrossVolumeFromMeterValues]	 BIT                CONSTRAINT [DF_tblTransactionAliases_PopulateGrossVolumeFromMeterValues] DEFAULT ((0)) NOT NULL,
	[UseMeterAndCompressionFactorFromMeter]	 BIT                CONSTRAINT [DF_tblTransactionAliases_UseMeterAndCompressionFactorFromMeter] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_tblTransactionAliases_GUID] PRIMARY KEY NONCLUSTERED ([TransactionAliasGuid] ASC),
    CONSTRAINT [CK_tblTransactionAliases_LookupDefaultStatusIndex] CHECK ([LookupDefaultStatusIndex]>=(-1) AND [LookupDefaultStatusIndex]<(100)),
    CONSTRAINT [FK_tblTransactionAliases_AdditiveProfileCycleAmountUnitIndex] FOREIGN KEY ([AdditiveProfileCycleAmountUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_AdditiveProfileRateUnitIndex] FOREIGN KEY ([AdditiveProfileRateUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_AdditiveVolumeUnitIndex] FOREIGN KEY ([AdditiveVolumeUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_AssociatedTransactionAliasGuid] FOREIGN KEY ([AssociatedTransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblTransactionAliases_DensityUnitIndex] FOREIGN KEY ([DensityUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_FlowUnitIndex] FOREIGN KEY ([FlowUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_LevelUnitIndex] FOREIGN KEY ([LevelUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_LookupTransactionStatusIndex] FOREIGN KEY ([LookupDefaultStatusIndex]) REFERENCES [lookup].[tblTransactionStatus] ([TransactionStatusIndex]),
    CONSTRAINT [FK_tblTransactionAliases_LookupTransactionTypesIndex] FOREIGN KEY ([LookupTransTypeIndex]) REFERENCES [lookup].[tblTransactionTypes] ([TransactionTypesIndex]),
    CONSTRAINT [FK_tblTransactionAliases_MassUnitIndex] FOREIGN KEY ([MassUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_PressureUnitIndex] FOREIGN KEY ([PressureUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblTransactionAliases_TemperatureUnitIndex] FOREIGN KEY ([TemperatureUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblTransactionAliases_VolumeUnitIndex] FOREIGN KEY ([VolumeUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]), 
    CONSTRAINT [CK_tblTransactionAliases_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessTransactionAlias]([_MasterRecordGuid],[SiteGuid],[AliasName],[MeterCloseout]) = 1)
);




GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliases_SiteGuid]
    ON [dbo].[tblTransactionAliases]([SiteGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliases_AliasName_SiteGuid]
    ON [dbo].[tblTransactionAliases]([AliasName] ASC, [SiteGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactionAliases_SiteGuid_MasterRecordGuid]
    ON [dbo].[tblTransactionAliases]([SiteGuid] ASC, [_MasterRecordGuid] ASC)
    INCLUDE([TransactionAliasGuid]);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblTransactionAliases_TransactionAliasGuid_AliasName]
    ON [dbo].[tblTransactionAliases]([TransactionAliasGuid] ASC, [AliasName] ASC)
    INCLUDE([LookupTransTypeIndex], [IncludeInDispatch], [_MasterRecordGuid]);


GO

--Creating Insert / Update Trigger for tblTransactionAliases
CREATE TRIGGER dbo.trg_insupd_tblTransactionAliases_ForSync 
   ON dbo.tblTransactionAliases
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
                    ,d.TransactionAliasGuid AS Deleted_PK_TransactionAliasGuid
                    ,i.TransactionAliasGuid AS Inserted_PK_TransactionAliasGuid
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
				    d.TransactionAliasGuid = i.TransactionAliasGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionAliases As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionAliasGuid = currentTrackingData.PK_TransactionAliasGuid
 
 
		    INSERT track.tblTransactionAliases (InsertedDate 
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
				    ,PK_TransactionAliasGuid
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
				    ,entityChanges.Inserted_PK_TransactionAliasGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionAliases As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionAliasGuid = currentTrackingData.PK_TransactionAliasGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionAliases
CREATE TRIGGER dbo.trg_del_tblTransactionAliases_ForSync 
   ON dbo.tblTransactionAliases
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
						,d.TransactionAliasGuid AS Deleted_PK_TransactionAliasGuid
                        ,d.TransactionAliasGuid AS Inserted_PK_TransactionAliasGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionAliases As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionAliasGuid = currentTrackingData.PK_TransactionAliasGuid
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
						,PK_TransactionAliasGuid
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
						,entityChanges.Deleted_PK_TransactionAliasGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO

-------------------------------------
-- AUDIT INSERT TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionAliases] ON [dbo].[tblTransactionAliases] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionAliases','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionAliases (
		[AliasName]
	,	[MeterCloseout]
	,	[BulkShipment]
	,	[DistributedImpact]
	,	[MultipleLineItems]
	,	[LimitSelectionsBasedOnHierarchy]
	,	[LineItemEditControl]
	,	[MultipleWeightReadings]
	,	[WeightReadingEditControl]
	,	[AssociatedReport]
	,	[AssociatedPreloadReport]
	,	[DestinationEquipmentTypes1]
	,	[DestinationEquipmentTypes2]
	,	[DestinationEquipmentTypes3]
	,	[SourceEquipmentTypes1]
	,	[SourceEquipmentTypes2]
	,	[SourceEquipmentTypes3]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ShowCompanyName]
	,	[AggregateAssocTrans]
	,	[EnableTotalQuantityExceededWarning]
	,	[EnableQuantityToleranceExceededWarning]
	,	[EnableTotalValueExceededWarning]
	,	[EnableValueToleranceExceededWarning]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[AdditiveVolumeUnitIndex]
	,	[AdditiveProfileCycleAmountUnitIndex]
	,	[AdditiveProfileRateUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[AdditiveVolumeDecimalPlaces]
	,	[UseComboBoxControls]
	,	[MultipleTransportLineItems]
	,	[TransactionAliasGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupDefaultStatusIndex]
	,	[AssociatedTransactionAliasGuid]
	,	[IncludeInDispatch]
	,	[_MasterRecordGuid]
	,	[EnableAutoCompleteControls]
	,	[PermitNonReferenceData]
	,	[UseTransactionDetailWithLayout]
	,	[DefaultMeterToEquipmentID]
	,	[LimitSourceEquipmentByProduct]
	,	[RememberMeterEndForMeterID]
	,	[PopulateCompaniesFromEquipment]
	,	[PopulateGrossVolumeFromMeterValues]
	,	[UseMeterAndCompressionFactorFromMeter]
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
		i.[AliasName]
	,	i.[MeterCloseout]
	,	i.[BulkShipment]
	,	i.[DistributedImpact]
	,	i.[MultipleLineItems]
	,	i.[LimitSelectionsBasedOnHierarchy]
	,	i.[LineItemEditControl]
	,	i.[MultipleWeightReadings]
	,	i.[WeightReadingEditControl]
	,	i.[AssociatedReport]
	,	i.[AssociatedPreloadReport]
	,	i.[DestinationEquipmentTypes1]
	,	i.[DestinationEquipmentTypes2]
	,	i.[DestinationEquipmentTypes3]
	,	i.[SourceEquipmentTypes1]
	,	i.[SourceEquipmentTypes2]
	,	i.[SourceEquipmentTypes3]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ShowCompanyName]
	,	i.[AggregateAssocTrans]
	,	i.[EnableTotalQuantityExceededWarning]
	,	i.[EnableQuantityToleranceExceededWarning]
	,	i.[EnableTotalValueExceededWarning]
	,	i.[EnableValueToleranceExceededWarning]
	,	i.[LevelUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[PressureUnitIndex]
	,	i.[FlowUnitIndex]
	,	i.[VolumeUnitIndex]
	,	i.[MassUnitIndex]
	,	i.[AdditiveVolumeUnitIndex]
	,	i.[AdditiveProfileCycleAmountUnitIndex]
	,	i.[AdditiveProfileRateUnitIndex]
	,	i.[LevelDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[PressureDecimalPlaces]
	,	i.[FlowDecimalPlaces]
	,	i.[VolumeDecimalPlaces]
	,	i.[MassDecimalPlaces]
	,	i.[AdditiveVolumeDecimalPlaces]
	,	i.[UseComboBoxControls]
	,	i.[MultipleTransportLineItems]
	,	i.[TransactionAliasGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupTransTypeIndex]
	,	i.[LookupDefaultStatusIndex]
	,	i.[AssociatedTransactionAliasGuid]
	,	i.[IncludeInDispatch]
	,	i.[_MasterRecordGuid]
	,	i.[EnableAutoCompleteControls]
	,	i.[PermitNonReferenceData]
	,	i.[UseTransactionDetailWithLayout]
	,	i.[DefaultMeterToEquipmentID]
	,	i.[LimitSourceEquipmentByProduct]
	,	i.[RememberMeterEndForMeterID]
	,	i.[PopulateCompaniesFromEquipment]
	,	i.[PopulateGrossVolumeFromMeterValues]
	,	i.[UseMeterAndCompressionFactorFromMeter]
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
 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionAliases] ON [dbo].[tblTransactionAliases] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionAliases','D')=1 
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
	TransactionAliasGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionAliases (
		[AliasName]
	,	[MeterCloseout]
	,	[BulkShipment]
	,	[DistributedImpact]
	,	[MultipleLineItems]
	,	[LimitSelectionsBasedOnHierarchy]
	,	[LineItemEditControl]
	,	[MultipleWeightReadings]
	,	[WeightReadingEditControl]
	,	[AssociatedReport]
	,	[AssociatedPreloadReport]
	,	[DestinationEquipmentTypes1]
	,	[DestinationEquipmentTypes2]
	,	[DestinationEquipmentTypes3]
	,	[SourceEquipmentTypes1]
	,	[SourceEquipmentTypes2]
	,	[SourceEquipmentTypes3]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ShowCompanyName]
	,	[AggregateAssocTrans]
	,	[EnableTotalQuantityExceededWarning]
	,	[EnableQuantityToleranceExceededWarning]
	,	[EnableTotalValueExceededWarning]
	,	[EnableValueToleranceExceededWarning]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[AdditiveVolumeUnitIndex]
	,	[AdditiveProfileCycleAmountUnitIndex]
	,	[AdditiveProfileRateUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[AdditiveVolumeDecimalPlaces]
	,	[UseComboBoxControls]
	,	[MultipleTransportLineItems]
	,	[TransactionAliasGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupDefaultStatusIndex]
	,	[AssociatedTransactionAliasGuid]
	,	[IncludeInDispatch]
	,	[_MasterRecordGuid]
	,	[EnableAutoCompleteControls]
	,	[PermitNonReferenceData]
	,	[UseTransactionDetailWithLayout]
	,	[DefaultMeterToEquipmentID]
	,	[LimitSourceEquipmentByProduct]
	,	[RememberMeterEndForMeterID]
	,	[PopulateCompaniesFromEquipment]
	,	[PopulateGrossVolumeFromMeterValues]
	,	[UseMeterAndCompressionFactorFromMeter]
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
	OUTPUT inserted.[TransactionAliasGuid] AS 'TransactionAliasGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AliasName]
	,	d.[MeterCloseout]
	,	d.[BulkShipment]
	,	d.[DistributedImpact]
	,	d.[MultipleLineItems]
	,	d.[LimitSelectionsBasedOnHierarchy]
	,	d.[LineItemEditControl]
	,	d.[MultipleWeightReadings]
	,	d.[WeightReadingEditControl]
	,	d.[AssociatedReport]
	,	d.[AssociatedPreloadReport]
	,	d.[DestinationEquipmentTypes1]
	,	d.[DestinationEquipmentTypes2]
	,	d.[DestinationEquipmentTypes3]
	,	d.[SourceEquipmentTypes1]
	,	d.[SourceEquipmentTypes2]
	,	d.[SourceEquipmentTypes3]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ShowCompanyName]
	,	d.[AggregateAssocTrans]
	,	d.[EnableTotalQuantityExceededWarning]
	,	d.[EnableQuantityToleranceExceededWarning]
	,	d.[EnableTotalValueExceededWarning]
	,	d.[EnableValueToleranceExceededWarning]
	,	d.[LevelUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[PressureUnitIndex]
	,	d.[FlowUnitIndex]
	,	d.[VolumeUnitIndex]
	,	d.[MassUnitIndex]
	,	d.[AdditiveVolumeUnitIndex]
	,	d.[AdditiveProfileCycleAmountUnitIndex]
	,	d.[AdditiveProfileRateUnitIndex]
	,	d.[LevelDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[PressureDecimalPlaces]
	,	d.[FlowDecimalPlaces]
	,	d.[VolumeDecimalPlaces]
	,	d.[MassDecimalPlaces]
	,	d.[AdditiveVolumeDecimalPlaces]
	,	d.[UseComboBoxControls]
	,	d.[MultipleTransportLineItems]
	,	d.[TransactionAliasGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupTransTypeIndex]
	,	d.[LookupDefaultStatusIndex]
	,	d.[AssociatedTransactionAliasGuid]
	,	d.[IncludeInDispatch]
	,	d.[_MasterRecordGuid]
	,	d.[EnableAutoCompleteControls]
	,	d.[PermitNonReferenceData]
	,	d.[UseTransactionDetailWithLayout]
	,	d.[DefaultMeterToEquipmentID]
	,	d.[LimitSourceEquipmentByProduct]
	,	d.[RememberMeterEndForMeterID]
	,	d.[PopulateCompaniesFromEquipment]
	,	d.[PopulateGrossVolumeFromMeterValues]
	,	d.[UseMeterAndCompressionFactorFromMeter]
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
 
	INSERT INTO [fmaudit].tblTransactionAliases (
		[AliasName]
	,	[MeterCloseout]
	,	[BulkShipment]
	,	[DistributedImpact]
	,	[MultipleLineItems]
	,	[LimitSelectionsBasedOnHierarchy]
	,	[LineItemEditControl]
	,	[MultipleWeightReadings]
	,	[WeightReadingEditControl]
	,	[AssociatedReport]
	,	[AssociatedPreloadReport]
	,	[DestinationEquipmentTypes1]
	,	[DestinationEquipmentTypes2]
	,	[DestinationEquipmentTypes3]
	,	[SourceEquipmentTypes1]
	,	[SourceEquipmentTypes2]
	,	[SourceEquipmentTypes3]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ShowCompanyName]
	,	[AggregateAssocTrans]
	,	[EnableTotalQuantityExceededWarning]
	,	[EnableQuantityToleranceExceededWarning]
	,	[EnableTotalValueExceededWarning]
	,	[EnableValueToleranceExceededWarning]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[AdditiveVolumeUnitIndex]
	,	[AdditiveProfileCycleAmountUnitIndex]
	,	[AdditiveProfileRateUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[AdditiveVolumeDecimalPlaces]
	,	[UseComboBoxControls]
	,	[MultipleTransportLineItems]
	,	[TransactionAliasGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupDefaultStatusIndex]
	,	[AssociatedTransactionAliasGuid]
	,	[IncludeInDispatch]
	,	[_MasterRecordGuid]
	,	[EnableAutoCompleteControls]
	,	[PermitNonReferenceData]
	,	[UseTransactionDetailWithLayout]
	,	[DefaultMeterToEquipmentID]
	,	[LimitSourceEquipmentByProduct]
	,	[RememberMeterEndForMeterID]
	,	[PopulateCompaniesFromEquipment]
	,	[PopulateGrossVolumeFromMeterValues]
	,	[UseMeterAndCompressionFactorFromMeter]
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
		i.[AliasName]
	,	i.[MeterCloseout]
	,	i.[BulkShipment]
	,	i.[DistributedImpact]
	,	i.[MultipleLineItems]
	,	i.[LimitSelectionsBasedOnHierarchy]
	,	i.[LineItemEditControl]
	,	i.[MultipleWeightReadings]
	,	i.[WeightReadingEditControl]
	,	i.[AssociatedReport]
	,	i.[AssociatedPreloadReport]
	,	i.[DestinationEquipmentTypes1]
	,	i.[DestinationEquipmentTypes2]
	,	i.[DestinationEquipmentTypes3]
	,	i.[SourceEquipmentTypes1]
	,	i.[SourceEquipmentTypes2]
	,	i.[SourceEquipmentTypes3]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ShowCompanyName]
	,	i.[AggregateAssocTrans]
	,	i.[EnableTotalQuantityExceededWarning]
	,	i.[EnableQuantityToleranceExceededWarning]
	,	i.[EnableTotalValueExceededWarning]
	,	i.[EnableValueToleranceExceededWarning]
	,	i.[LevelUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[PressureUnitIndex]
	,	i.[FlowUnitIndex]
	,	i.[VolumeUnitIndex]
	,	i.[MassUnitIndex]
	,	i.[AdditiveVolumeUnitIndex]
	,	i.[AdditiveProfileCycleAmountUnitIndex]
	,	i.[AdditiveProfileRateUnitIndex]
	,	i.[LevelDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[PressureDecimalPlaces]
	,	i.[FlowDecimalPlaces]
	,	i.[VolumeDecimalPlaces]
	,	i.[MassDecimalPlaces]
	,	i.[AdditiveVolumeDecimalPlaces]
	,	i.[UseComboBoxControls]
	,	i.[MultipleTransportLineItems]
	,	i.[TransactionAliasGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupTransTypeIndex]
	,	i.[LookupDefaultStatusIndex]
	,	i.[AssociatedTransactionAliasGuid]
	,	i.[IncludeInDispatch]
	,	i.[_MasterRecordGuid]
	,	i.[EnableAutoCompleteControls]
	,	i.[PermitNonReferenceData]
	,	i.[UseTransactionDetailWithLayout]
	,	i.[DefaultMeterToEquipmentID]
	,	i.[LimitSourceEquipmentByProduct]
	,	i.[RememberMeterEndForMeterID]
	,	i.[PopulateCompaniesFromEquipment]
	,	i.[PopulateGrossVolumeFromMeterValues]
	,	i.[UseMeterAndCompressionFactorFromMeter]
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
			agl.[TransactionAliasGuid]=i.[TransactionAliasGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
 
-------------------------------------
-- AUDIT DELETE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionAliases] ON [dbo].[tblTransactionAliases] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionAliases','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionAliases (
		[AliasName]
	,	[MeterCloseout]
	,	[BulkShipment]
	,	[DistributedImpact]
	,	[MultipleLineItems]
	,	[LimitSelectionsBasedOnHierarchy]
	,	[LineItemEditControl]
	,	[MultipleWeightReadings]
	,	[WeightReadingEditControl]
	,	[AssociatedReport]
	,	[AssociatedPreloadReport]
	,	[DestinationEquipmentTypes1]
	,	[DestinationEquipmentTypes2]
	,	[DestinationEquipmentTypes3]
	,	[SourceEquipmentTypes1]
	,	[SourceEquipmentTypes2]
	,	[SourceEquipmentTypes3]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ShowCompanyName]
	,	[AggregateAssocTrans]
	,	[EnableTotalQuantityExceededWarning]
	,	[EnableQuantityToleranceExceededWarning]
	,	[EnableTotalValueExceededWarning]
	,	[EnableValueToleranceExceededWarning]
	,	[LevelUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[PressureUnitIndex]
	,	[FlowUnitIndex]
	,	[VolumeUnitIndex]
	,	[MassUnitIndex]
	,	[AdditiveVolumeUnitIndex]
	,	[AdditiveProfileCycleAmountUnitIndex]
	,	[AdditiveProfileRateUnitIndex]
	,	[LevelDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[PressureDecimalPlaces]
	,	[FlowDecimalPlaces]
	,	[VolumeDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[AdditiveVolumeDecimalPlaces]
	,	[UseComboBoxControls]
	,	[MultipleTransportLineItems]
	,	[TransactionAliasGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTransTypeIndex]
	,	[LookupDefaultStatusIndex]
	,	[AssociatedTransactionAliasGuid]
	,	[IncludeInDispatch]
	,	[_MasterRecordGuid]
	,	[EnableAutoCompleteControls]
	,	[PermitNonReferenceData]
	,	[UseTransactionDetailWithLayout]
	,	[DefaultMeterToEquipmentID]
	,	[LimitSourceEquipmentByProduct]
	,	[RememberMeterEndForMeterID]
	,	[PopulateCompaniesFromEquipment]
	,	[PopulateGrossVolumeFromMeterValues]
	,	[UseMeterAndCompressionFactorFromMeter]
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
		d.[AliasName]
	,	d.[MeterCloseout]
	,	d.[BulkShipment]
	,	d.[DistributedImpact]
	,	d.[MultipleLineItems]
	,	d.[LimitSelectionsBasedOnHierarchy]
	,	d.[LineItemEditControl]
	,	d.[MultipleWeightReadings]
	,	d.[WeightReadingEditControl]
	,	d.[AssociatedReport]
	,	d.[AssociatedPreloadReport]
	,	d.[DestinationEquipmentTypes1]
	,	d.[DestinationEquipmentTypes2]
	,	d.[DestinationEquipmentTypes3]
	,	d.[SourceEquipmentTypes1]
	,	d.[SourceEquipmentTypes2]
	,	d.[SourceEquipmentTypes3]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ShowCompanyName]
	,	d.[AggregateAssocTrans]
	,	d.[EnableTotalQuantityExceededWarning]
	,	d.[EnableQuantityToleranceExceededWarning]
	,	d.[EnableTotalValueExceededWarning]
	,	d.[EnableValueToleranceExceededWarning]
	,	d.[LevelUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[PressureUnitIndex]
	,	d.[FlowUnitIndex]
	,	d.[VolumeUnitIndex]
	,	d.[MassUnitIndex]
	,	d.[AdditiveVolumeUnitIndex]
	,	d.[AdditiveProfileCycleAmountUnitIndex]
	,	d.[AdditiveProfileRateUnitIndex]
	,	d.[LevelDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[PressureDecimalPlaces]
	,	d.[FlowDecimalPlaces]
	,	d.[VolumeDecimalPlaces]
	,	d.[MassDecimalPlaces]
	,	d.[AdditiveVolumeDecimalPlaces]
	,	d.[UseComboBoxControls]
	,	d.[MultipleTransportLineItems]
	,	d.[TransactionAliasGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupTransTypeIndex]
	,	d.[LookupDefaultStatusIndex]
	,	d.[AssociatedTransactionAliasGuid]
	,	d.[IncludeInDispatch]
	,	d.[_MasterRecordGuid]
	,	d.[EnableAutoCompleteControls]
	,	d.[PermitNonReferenceData]
	,	d.[UseTransactionDetailWithLayout]
	,	d.[DefaultMeterToEquipmentID]
	,	d.[LimitSourceEquipmentByProduct]
	,	d.[RememberMeterEndForMeterID]
	,	d.[PopulateCompaniesFromEquipment]
	,	d.[PopulateGrossVolumeFromMeterValues]
	,	d.[UseMeterAndCompressionFactorFromMeter]
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


CREATE TRIGGER [dbo].[trg_fmcdc_tblTransactionAliases]
ON [dbo].[tblTransactionAliases]
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
		INSERT INTO fmcdc.[tblTransactionAliases]
		(
		[AliasName]
		, [MeterCloseout]
		, [BulkShipment]
		, [DistributedImpact]
		, [MultipleLineItems]
		, [LimitSelectionsBasedOnHierarchy]
		, [LineItemEditControl]
		, [MultipleWeightReadings]
		, [WeightReadingEditControl]
		, [AssociatedReport]
		, [AssociatedPreloadReport]
		, [DestinationEquipmentTypes1]
		, [DestinationEquipmentTypes2]
		, [DestinationEquipmentTypes3]
		, [SourceEquipmentTypes1]
		, [SourceEquipmentTypes2]
		, [SourceEquipmentTypes3]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [ShowCompanyName]
		, [AggregateAssocTrans]
		, [EnableTotalQuantityExceededWarning]
		, [EnableQuantityToleranceExceededWarning]
		, [EnableTotalValueExceededWarning]
		, [EnableValueToleranceExceededWarning]
		, [LevelUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [PressureUnitIndex]
		, [FlowUnitIndex]
		, [VolumeUnitIndex]
		, [MassUnitIndex]
		, [AdditiveVolumeUnitIndex]
		, [AdditiveProfileCycleAmountUnitIndex]
		, [AdditiveProfileRateUnitIndex]
		, [LevelDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [PressureDecimalPlaces]
		, [FlowDecimalPlaces]
		, [VolumeDecimalPlaces]
		, [MassDecimalPlaces]
		, [AdditiveVolumeDecimalPlaces]
		, [UseComboBoxControls]
		, [MultipleTransportLineItems]
		, [TransactionAliasGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupDefaultStatusIndex]
		, [AssociatedTransactionAliasGuid]
		, [IncludeInDispatch]
		, [_MasterRecordGuid]
		, [EnableAutoCompleteControls]
		, [PermitNonReferenceData]
		, [_ClusterIdx]
		, [UseTransactionDetailWithLayout]
		, [DefaultMeterToEquipmentID]
		, [LimitSourceEquipmentByProduct]
		, [RememberMeterEndForMeterID]
		, [PopulateCompaniesFromEquipment]
		, [PopulateGrossVolumeFromMeterValues]
		, [UseMeterAndCompressionFactorFromMeter]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[AliasName]
		, [MeterCloseout]
		, [BulkShipment]
		, [DistributedImpact]
		, [MultipleLineItems]
		, [LimitSelectionsBasedOnHierarchy]
		, [LineItemEditControl]
		, [MultipleWeightReadings]
		, [WeightReadingEditControl]
		, [AssociatedReport]
		, [AssociatedPreloadReport]
		, [DestinationEquipmentTypes1]
		, [DestinationEquipmentTypes2]
		, [DestinationEquipmentTypes3]
		, [SourceEquipmentTypes1]
		, [SourceEquipmentTypes2]
		, [SourceEquipmentTypes3]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [ShowCompanyName]
		, [AggregateAssocTrans]
		, [EnableTotalQuantityExceededWarning]
		, [EnableQuantityToleranceExceededWarning]
		, [EnableTotalValueExceededWarning]
		, [EnableValueToleranceExceededWarning]
		, [LevelUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [PressureUnitIndex]
		, [FlowUnitIndex]
		, [VolumeUnitIndex]
		, [MassUnitIndex]
		, [AdditiveVolumeUnitIndex]
		, [AdditiveProfileCycleAmountUnitIndex]
		, [AdditiveProfileRateUnitIndex]
		, [LevelDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [PressureDecimalPlaces]
		, [FlowDecimalPlaces]
		, [VolumeDecimalPlaces]
		, [MassDecimalPlaces]
		, [AdditiveVolumeDecimalPlaces]
		, [UseComboBoxControls]
		, [MultipleTransportLineItems]
		, [TransactionAliasGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupDefaultStatusIndex]
		, [AssociatedTransactionAliasGuid]
		, [IncludeInDispatch]
		, [_MasterRecordGuid]
		, [EnableAutoCompleteControls]
		, [PermitNonReferenceData]
		, [_ClusterIdx]
		, [UseTransactionDetailWithLayout]
		, [DefaultMeterToEquipmentID]
		, [LimitSourceEquipmentByProduct]
		, [RememberMeterEndForMeterID]
		, [PopulateCompaniesFromEquipment]
		, [PopulateGrossVolumeFromMeterValues]
		, [UseMeterAndCompressionFactorFromMeter]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTransactionAliases]
		(
		[AliasName]
		, [MeterCloseout]
		, [BulkShipment]
		, [DistributedImpact]
		, [MultipleLineItems]
		, [LimitSelectionsBasedOnHierarchy]
		, [LineItemEditControl]
		, [MultipleWeightReadings]
		, [WeightReadingEditControl]
		, [AssociatedReport]
		, [AssociatedPreloadReport]
		, [DestinationEquipmentTypes1]
		, [DestinationEquipmentTypes2]
		, [DestinationEquipmentTypes3]
		, [SourceEquipmentTypes1]
		, [SourceEquipmentTypes2]
		, [SourceEquipmentTypes3]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [ShowCompanyName]
		, [AggregateAssocTrans]
		, [EnableTotalQuantityExceededWarning]
		, [EnableQuantityToleranceExceededWarning]
		, [EnableTotalValueExceededWarning]
		, [EnableValueToleranceExceededWarning]
		, [LevelUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [PressureUnitIndex]
		, [FlowUnitIndex]
		, [VolumeUnitIndex]
		, [MassUnitIndex]
		, [AdditiveVolumeUnitIndex]
		, [AdditiveProfileCycleAmountUnitIndex]
		, [AdditiveProfileRateUnitIndex]
		, [LevelDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [PressureDecimalPlaces]
		, [FlowDecimalPlaces]
		, [VolumeDecimalPlaces]
		, [MassDecimalPlaces]
		, [AdditiveVolumeDecimalPlaces]
		, [UseComboBoxControls]
		, [MultipleTransportLineItems]
		, [TransactionAliasGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupDefaultStatusIndex]
		, [AssociatedTransactionAliasGuid]
		, [IncludeInDispatch]
		, [_MasterRecordGuid]
		, [EnableAutoCompleteControls]
		, [PermitNonReferenceData]
		, [_ClusterIdx]
		, [UseTransactionDetailWithLayout]
		, [DefaultMeterToEquipmentID]
		, [LimitSourceEquipmentByProduct]
		, [RememberMeterEndForMeterID]
		, [PopulateCompaniesFromEquipment]
		, [PopulateGrossVolumeFromMeterValues]
		, [UseMeterAndCompressionFactorFromMeter]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[AliasName]
		, [MeterCloseout]
		, [BulkShipment]
		, [DistributedImpact]
		, [MultipleLineItems]
		, [LimitSelectionsBasedOnHierarchy]
		, [LineItemEditControl]
		, [MultipleWeightReadings]
		, [WeightReadingEditControl]
		, [AssociatedReport]
		, [AssociatedPreloadReport]
		, [DestinationEquipmentTypes1]
		, [DestinationEquipmentTypes2]
		, [DestinationEquipmentTypes3]
		, [SourceEquipmentTypes1]
		, [SourceEquipmentTypes2]
		, [SourceEquipmentTypes3]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [ShowCompanyName]
		, [AggregateAssocTrans]
		, [EnableTotalQuantityExceededWarning]
		, [EnableQuantityToleranceExceededWarning]
		, [EnableTotalValueExceededWarning]
		, [EnableValueToleranceExceededWarning]
		, [LevelUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [PressureUnitIndex]
		, [FlowUnitIndex]
		, [VolumeUnitIndex]
		, [MassUnitIndex]
		, [AdditiveVolumeUnitIndex]
		, [AdditiveProfileCycleAmountUnitIndex]
		, [AdditiveProfileRateUnitIndex]
		, [LevelDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [PressureDecimalPlaces]
		, [FlowDecimalPlaces]
		, [VolumeDecimalPlaces]
		, [MassDecimalPlaces]
		, [AdditiveVolumeDecimalPlaces]
		, [UseComboBoxControls]
		, [MultipleTransportLineItems]
		, [TransactionAliasGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [LookupTransTypeIndex]
		, [LookupDefaultStatusIndex]
		, [AssociatedTransactionAliasGuid]
		, [IncludeInDispatch]
		, [_MasterRecordGuid]
		, [EnableAutoCompleteControls]
		, [PermitNonReferenceData]
		, [_ClusterIdx]
		, [UseTransactionDetailWithLayout]
		, [DefaultMeterToEquipmentID]
		, [LimitSourceEquipmentByProduct]
		, [RememberMeterEndForMeterID]
		, [PopulateCompaniesFromEquipment]
		, [PopulateGrossVolumeFromMeterValues]
		, [UseMeterAndCompressionFactorFromMeter]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTransactionAliases] ON [dbo].[tblTransactionAliases]
GO



CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionAliases_ClusterIdx]
    ON [dbo].[tblTransactionAliases]([_ClusterIdx] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliases_AliasName]
    ON [dbo].[tblTransactionAliases]([AliasName] ASC);

