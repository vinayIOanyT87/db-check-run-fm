CREATE TABLE [dbo].[tblEquipment] (
    [ID]                       NVARCHAR (30)      CONSTRAINT [DF_tblEquipment_ID] DEFAULT ('') NOT NULL,
    [Description]              NVARCHAR (50)      NULL,
    [Make]                     NVARCHAR (20)      NULL,
    [Model]                    NVARCHAR (50)      NULL,
    [Year]                     INT                NULL,
    [IssPtNum]                 NVARCHAR (20)      NULL,
    [Fixed]                    BIT                NULL,
    [StorageType]              NVARCHAR (2)       NULL,
    [InUse]                    BIT                NULL,
    [FixedVolume]              BIT                NULL,
    [IntoPlane]                BIT                NULL,
    [Mobile]                   BIT                NULL,
    [AttachedTo]               NVARCHAR (6)       NULL,
    [MediaType]                CHAR (1)           NULL,
    [Meters]                   INT                NULL,
    [DefuelMeterForwards]      BIT                NULL,
    [PulseRatio]               FLOAT (53)         NULL,
    [Round]                    BIT                NULL,
    [Xref]                     NVARCHAR (10)      NULL,
    [LowStockWarning]          FLOAT (53)         NULL,
    [StockTrack]               BIT                NULL,
    [Totalisor1]               NVARCHAR (10)      NULL,
    [Totalisor2]               NVARCHAR (10)      NULL,
    [FuelingState]             NVARCHAR (10)      NULL,
    [Volume]                   FLOAT (53)         NULL,
    [MeterReading]             FLOAT (53)         NULL,
    [Consecutive_OOS_Variance] INT                NULL,
    [Notes]                    NVARCHAR (1000)    NULL,
    [Capacity]                 FLOAT (53)         NULL,
    [SafeFill]                 FLOAT (53)         NULL,
    [VolumeUnitIndex]          INT                NULL,
    [TemperatureUnitIndex]     INT                NULL,
    [DensityUnitIndex]         INT                NULL,
    [MassUnitIndex]            INT                NULL,
    [VolumeDecimalPlaces]      TINYINT            NULL,
    [TemperatureDecimalPlaces] TINYINT            NULL,
    [DensityDecimalPlaces]     TINYINT            NULL,
    [MassDecimalPlaces]        TINYINT            NULL,
    [EquipmentSequence]        NVARCHAR (50)      NULL,
    [LockedOut]                BIT                NULL,
    [LockedOutReason]          NVARCHAR (80)      NULL,
    [LockedOutDate]            DATETIMEOFFSET (7) NULL,
    [SerialNumber]             NVARCHAR (30)      NULL,
    [CompanyEquipmentID]       NVARCHAR (30)      NULL,
    [TruckCardNumber]          NVARCHAR (32)      NULL,
    [CreatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipment_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipment_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipment_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipment_UpdatedBy] DEFAULT ('') NOT NULL,
    [RatedGPM]                 FLOAT (53)         NULL,
    [ActualGPM]                FLOAT (53)         NULL,
    [FuelAdditiveFlag]         BIT                NULL,
    [ManufactureDate]          DATETIMEOFFSET (7) NULL,
    [InstallationDate]         DATETIMEOFFSET (7) NULL,
    [InspectionDate]           DATETIMEOFFSET (7) NULL,
    [CalibrationDate]          DATETIMEOFFSET (7) NULL,
    [QCDate]                   DATETIMEOFFSET (7) NULL,
    [SecondaryStorageFlag]     BIT                NULL,
    [ManagedEquipmentFlag]     BIT                CONSTRAINT [DF_tblEquipment_ManagedEquipmentFlag] DEFAULT ((0)) NOT NULL,
    [FuelingType]              SMALLINT           NULL,
    [UserData1]                NVARCHAR (60)      NULL,
    [UserData2]                NVARCHAR (60)      NULL,
    [UserData3]                NVARCHAR (60)      NULL,
    [UserData4]                NVARCHAR (60)      NULL,
    [UserData5]                NVARCHAR (60)      NULL,
    [UserData6]                NVARCHAR (60)      NULL,
    [UserData7]                NVARCHAR (60)      NULL,
    [UserData8]                NVARCHAR (60)      NULL,
    [UserData9]                NVARCHAR (60)      NULL,
    [UserData10]               NVARCHAR (60)      NULL,
    [UserData11]               NVARCHAR (60)      NULL,
    [UserData12]               NVARCHAR (60)      NULL,
    [UserData13]               NVARCHAR (60)      NULL,
    [UserData14]               NVARCHAR (60)      NULL,
    [UserData15]               NVARCHAR (60)      NULL,
    [UserData16]               NVARCHAR (60)      NULL,
    [UserData17]               NVARCHAR (60)      NULL,
    [UserData18]               NVARCHAR (60)      NULL,
    [UserData19]               NVARCHAR (60)      NULL,
    [UserData20]               NVARCHAR (60)      NULL,
    [UserData21]               NVARCHAR (60)      NULL,
    [UserData22]               NVARCHAR (60)      NULL,
    [UserData23]               NVARCHAR (60)      NULL,
    [UserData24]               NVARCHAR (60)      NULL,
    [_RowVersion]              ROWVERSION         NOT NULL,
    [EquipmentGuid]            UNIQUEIDENTIFIER   CONSTRAINT [DF_tblEquipment_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]                 UNIQUEIDENTIFIER   NOT NULL,
    [CompanyGuid]              UNIQUEIDENTIFIER   NULL,
    [ParentEquipmentGuid]      UNIQUEIDENTIFIER   NULL,
    [EquipmentTypeGuid]        UNIQUEIDENTIFIER   NULL,
    [FuelCardGuid]             UNIQUEIDENTIFIER   NULL,
    [ProductGuid]              UNIQUEIDENTIFIER   NULL,
    [AssignedToMeterGuid]      UNIQUEIDENTIFIER   NULL,
    [AssetTrackingDeviceGuid]  UNIQUEIDENTIFIER   NULL, 
    [_MasterRecordGuid]        UNIQUEIDENTIFIER   NOT NULL,
    [HiddenDate]               DATETIMEOFFSET (7) NULL,
    [_ClusterIdx]              BIGINT             IDENTITY (1, 1) NOT NULL,
    [ScullyRequired]		   BIT				  CONSTRAINT [DF_tblEquipment_ScullyRequired] DEFAULT 0 NOT NULL, 
    CONSTRAINT [PK_tblEquipment_GUID] PRIMARY KEY NONCLUSTERED ([EquipmentGuid] ASC),
    CONSTRAINT [CK_tblEquipment_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessEquipment]([_MasterRecordGuid],[SiteGuid],[ID],[CompanyGuid],[CompanyEquipmentID])=(1)),
    CONSTRAINT [FK_tblEquipment_CompanyGuid] FOREIGN KEY ([CompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblEquipment_DensityUnitIndex] FOREIGN KEY ([DensityUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblEquipment_EquipmentTypeGuid] FOREIGN KEY ([EquipmentTypeGuid]) REFERENCES [dbo].[tblEquipmentTypes] ([EquipmentTypeGuid]),
    CONSTRAINT [FK_tblEquipment_FuelCardGuid] FOREIGN KEY ([FuelCardGuid]) REFERENCES [dbo].[tblFuelCards] ([FuelCardGuid]),
    CONSTRAINT [FK_tblEquipment_MassUnitIndex] FOREIGN KEY ([MassUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblEquipment_MeterGuid] FOREIGN KEY ([AssignedToMeterGuid]) REFERENCES [dbo].[tblMeter] ([MeterGuid]),
    CONSTRAINT [FK_tblEquipment_ParentEquipmentGuid] FOREIGN KEY ([ParentEquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid]),
    CONSTRAINT [FK_tblEquipment_ProductGuid] FOREIGN KEY ([ProductGuid]) REFERENCES [dbo].[tblProducts] ([ProductGuid]),
    CONSTRAINT [FK_tblEquipment_AssetTrackingDeviceGuid] FOREIGN KEY ([AssetTrackingDeviceGuid]) REFERENCES [dbo].[tblAssetTrackingDevice] ([AssetTrackingDeviceGuid]),
    CONSTRAINT [FK_tblEquipment_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblEquipment_TemperatureUnitIndex] FOREIGN KEY ([TemperatureUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
    CONSTRAINT [FK_tblEquipment_VolumeUnitIndex] FOREIGN KEY ([VolumeUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblEquipment_CreatedDate]
    ON [dbo].[tblEquipment]([CreatedDate] ASC);




GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblEquipment_001]
    ON [dbo].[tblEquipment]([_MasterRecordGuid] ASC, [SiteGuid] ASC)
    INCLUDE([EquipmentGuid]);


GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblEquipment_EquipmentGuid_CoveringForLineItemTrigger]
    ON [dbo].[tblEquipment]([EquipmentGuid] ASC)
    INCLUDE([Volume], [SafeFill], [SecondaryStorageFlag], [Consecutive_OOS_Variance]);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblEquipment_EquipmentGuid_CoveringBasicFields]
    ON [dbo].[tblEquipment]([EquipmentGuid] ASC)
    INCLUDE([ID], [Xref], [SiteGuid], [_MasterRecordGuid], [ParentEquipmentGuid])
GO

CREATE INDEX [IX_tblEquipment_FuelCardGuid] ON [dbo].[tblEquipment] ([FuelCardGuid])
GO
--Creating Insert / Update Trigger for tblEquipment
CREATE TRIGGER dbo.trg_insupd_tblEquipment_ForSync 
   ON dbo.tblEquipment
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
                    ,d.EquipmentGuid AS Deleted_PK_EquipmentGuid
                    ,i.EquipmentGuid AS Inserted_PK_EquipmentGuid
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
				    d.EquipmentGuid = i.EquipmentGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblEquipment As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_EquipmentGuid = currentTrackingData.PK_EquipmentGuid
 
 
		    INSERT track.tblEquipment (InsertedDate 
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
				    ,PK_EquipmentGuid
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
				    ,entityChanges.Inserted_PK_EquipmentGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblEquipment As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_EquipmentGuid = currentTrackingData.PK_EquipmentGuid
)
    END
END 

GO
--Creating Delete Trigger for tblEquipment
CREATE TRIGGER dbo.trg_del_tblEquipment_ForSync 
   ON dbo.tblEquipment
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
						,d.EquipmentGuid AS Deleted_PK_EquipmentGuid
                        ,d.EquipmentGuid AS Inserted_PK_EquipmentGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblEquipment As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_EquipmentGuid = currentTrackingData.PK_EquipmentGuid
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
						,PK_EquipmentGuid
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
						,entityChanges.Deleted_PK_EquipmentGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblEquipment] ON [dbo].[tblEquipment] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipment','D')=1 
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
	INSERT INTO [fmaudit].tblEquipment (
		[ID]
	,	[Description]
	,	[Make]
	,	[Model]
	,	[Year]
	,	[IssPtNum]
	,	[Fixed]
	,	[StorageType]
	,	[InUse]
	,	[FixedVolume]
	,	[IntoPlane]
	,	[Mobile]
	,	[AttachedTo]
	,	[MediaType]
	,	[Meters]
	,	[DefuelMeterForwards]
	,	[PulseRatio]
	,	[Round]
	,	[Xref]
	,	[LowStockWarning]
	,	[StockTrack]
	,	[Totalisor1]
	,	[Totalisor2]
	,	[FuelingState]
	,	[Volume]
	,	[MeterReading]
	,	[Consecutive_OOS_Variance]
	,	[Notes]
	,	[Capacity]
	,	[SafeFill]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[MassUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[EquipmentSequence]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[SerialNumber]
	,	[CompanyEquipmentID]
	,	[TruckCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[RatedGPM]
	,	[ActualGPM]
	,	[FuelAdditiveFlag]
	,	[ManufactureDate]
	,	[InstallationDate]
	,	[InspectionDate]
	,	[CalibrationDate]
	,	[QCDate]
	,	[SecondaryStorageFlag]
	,	[ManagedEquipmentFlag]
	,	[FuelingType]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[UserData9]
	,	[UserData10]
	,	[UserData11]
	,	[UserData12]
	,	[UserData13]
	,	[UserData14]
	,	[UserData15]
	,	[UserData16]
	,	[UserData17]
	,	[UserData18]
	,	[UserData19]
	,	[UserData20]
	,	[UserData21]
	,	[UserData22]
	,	[UserData23]
	,	[UserData24]
	,	[OriginalRowVersion]
	,	[EquipmentGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[ParentEquipmentGuid]
	,	[EquipmentTypeGuid]
	,	[FuelCardGuid]
	,	[ProductGuid]
	,	[AssignedToMeterGuid]
	,	[AssetTrackingDeviceGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[ScullyRequired]
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
	,	d.[Make]
	,	d.[Model]
	,	d.[Year]
	,	d.[IssPtNum]
	,	d.[Fixed]
	,	d.[StorageType]
	,	d.[InUse]
	,	d.[FixedVolume]
	,	d.[IntoPlane]
	,	d.[Mobile]
	,	d.[AttachedTo]
	,	d.[MediaType]
	,	d.[Meters]
	,	d.[DefuelMeterForwards]
	,	d.[PulseRatio]
	,	d.[Round]
	,	d.[Xref]
	,	d.[LowStockWarning]
	,	d.[StockTrack]
	,	d.[Totalisor1]
	,	d.[Totalisor2]
	,	d.[FuelingState]
	,	d.[Volume]
	,	d.[MeterReading]
	,	d.[Consecutive_OOS_Variance]
	,	d.[Notes]
	,	d.[Capacity]
	,	d.[SafeFill]
	,	d.[VolumeUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[MassUnitIndex]
	,	d.[VolumeDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[MassDecimalPlaces]
	,	d.[EquipmentSequence]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[SerialNumber]
	,	d.[CompanyEquipmentID]
	,	d.[TruckCardNumber]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[RatedGPM]
	,	d.[ActualGPM]
	,	d.[FuelAdditiveFlag]
	,	d.[ManufactureDate]
	,	d.[InstallationDate]
	,	d.[InspectionDate]
	,	d.[CalibrationDate]
	,	d.[QCDate]
	,	d.[SecondaryStorageFlag]
	,	d.[ManagedEquipmentFlag]
	,	d.[FuelingType]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[UserData9]
	,	d.[UserData10]
	,	d.[UserData11]
	,	d.[UserData12]
	,	d.[UserData13]
	,	d.[UserData14]
	,	d.[UserData15]
	,	d.[UserData16]
	,	d.[UserData17]
	,	d.[UserData18]
	,	d.[UserData19]
	,	d.[UserData20]
	,	d.[UserData21]
	,	d.[UserData22]
	,	d.[UserData23]
	,	d.[UserData24]
	,	d.[_RowVersion]
	,	d.[EquipmentGuid]
	,	d.[SiteGuid]
	,	d.[CompanyGuid]
	,	d.[ParentEquipmentGuid]
	,	d.[EquipmentTypeGuid]
	,	d.[FuelCardGuid]
	,	d.[ProductGuid]
	,	d.[AssignedToMeterGuid]
	,	d.[AssetTrackingDeviceGuid]
	,	d.[_MasterRecordGuid]
	,	d.[HiddenDate]
	,	d.[ScullyRequired]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblEquipment] ON [dbo].[tblEquipment] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipment','D')=1 
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
	INSERT INTO [fmaudit].tblEquipment (
		[ID]
	,	[Description]
	,	[Make]
	,	[Model]
	,	[Year]
	,	[IssPtNum]
	,	[Fixed]
	,	[StorageType]
	,	[InUse]
	,	[FixedVolume]
	,	[IntoPlane]
	,	[Mobile]
	,	[AttachedTo]
	,	[MediaType]
	,	[Meters]
	,	[DefuelMeterForwards]
	,	[PulseRatio]
	,	[Round]
	,	[Xref]
	,	[LowStockWarning]
	,	[StockTrack]
	,	[Totalisor1]
	,	[Totalisor2]
	,	[FuelingState]
	,	[Volume]
	,	[MeterReading]
	,	[Consecutive_OOS_Variance]
	,	[Notes]
	,	[Capacity]
	,	[SafeFill]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[MassUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[EquipmentSequence]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[SerialNumber]
	,	[CompanyEquipmentID]
	,	[TruckCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[RatedGPM]
	,	[ActualGPM]
	,	[FuelAdditiveFlag]
	,	[ManufactureDate]
	,	[InstallationDate]
	,	[InspectionDate]
	,	[CalibrationDate]
	,	[QCDate]
	,	[SecondaryStorageFlag]
	,	[ManagedEquipmentFlag]
	,	[FuelingType]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[UserData9]
	,	[UserData10]
	,	[UserData11]
	,	[UserData12]
	,	[UserData13]
	,	[UserData14]
	,	[UserData15]
	,	[UserData16]
	,	[UserData17]
	,	[UserData18]
	,	[UserData19]
	,	[UserData20]
	,	[UserData21]
	,	[UserData22]
	,	[UserData23]
	,	[UserData24]
	,	[OriginalRowVersion]
	,	[EquipmentGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[ParentEquipmentGuid]
	,	[EquipmentTypeGuid]
	,	[FuelCardGuid]
	,	[ProductGuid]
	,	[AssignedToMeterGuid]
	,	[AssetTrackingDeviceGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[ScullyRequired]
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
	,	i.[Make]
	,	i.[Model]
	,	i.[Year]
	,	i.[IssPtNum]
	,	i.[Fixed]
	,	i.[StorageType]
	,	i.[InUse]
	,	i.[FixedVolume]
	,	i.[IntoPlane]
	,	i.[Mobile]
	,	i.[AttachedTo]
	,	i.[MediaType]
	,	i.[Meters]
	,	i.[DefuelMeterForwards]
	,	i.[PulseRatio]
	,	i.[Round]
	,	i.[Xref]
	,	i.[LowStockWarning]
	,	i.[StockTrack]
	,	i.[Totalisor1]
	,	i.[Totalisor2]
	,	i.[FuelingState]
	,	i.[Volume]
	,	i.[MeterReading]
	,	i.[Consecutive_OOS_Variance]
	,	i.[Notes]
	,	i.[Capacity]
	,	i.[SafeFill]
	,	i.[VolumeUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[MassUnitIndex]
	,	i.[VolumeDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[MassDecimalPlaces]
	,	i.[EquipmentSequence]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[SerialNumber]
	,	i.[CompanyEquipmentID]
	,	i.[TruckCardNumber]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[RatedGPM]
	,	i.[ActualGPM]
	,	i.[FuelAdditiveFlag]
	,	i.[ManufactureDate]
	,	i.[InstallationDate]
	,	i.[InspectionDate]
	,	i.[CalibrationDate]
	,	i.[QCDate]
	,	i.[SecondaryStorageFlag]
	,	i.[ManagedEquipmentFlag]
	,	i.[FuelingType]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[UserData9]
	,	i.[UserData10]
	,	i.[UserData11]
	,	i.[UserData12]
	,	i.[UserData13]
	,	i.[UserData14]
	,	i.[UserData15]
	,	i.[UserData16]
	,	i.[UserData17]
	,	i.[UserData18]
	,	i.[UserData19]
	,	i.[UserData20]
	,	i.[UserData21]
	,	i.[UserData22]
	,	i.[UserData23]
	,	i.[UserData24]
	,	i.[_RowVersion]
	,	i.[EquipmentGuid]
	,	i.[SiteGuid]
	,	i.[CompanyGuid]
	,	i.[ParentEquipmentGuid]
	,	i.[EquipmentTypeGuid]
	,	i.[FuelCardGuid]
	,	i.[ProductGuid]
	,	i.[AssignedToMeterGuid]
	,	i.[AssetTrackingDeviceGuid]
	,	i.[_MasterRecordGuid]
	,	i.[HiddenDate]
	,	i.[ScullyRequired]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblEquipment] ON [dbo].[tblEquipment] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipment','D')=1 
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
	EquipmentGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblEquipment (
		[ID]
	,	[Description]
	,	[Make]
	,	[Model]
	,	[Year]
	,	[IssPtNum]
	,	[Fixed]
	,	[StorageType]
	,	[InUse]
	,	[FixedVolume]
	,	[IntoPlane]
	,	[Mobile]
	,	[AttachedTo]
	,	[MediaType]
	,	[Meters]
	,	[DefuelMeterForwards]
	,	[PulseRatio]
	,	[Round]
	,	[Xref]
	,	[LowStockWarning]
	,	[StockTrack]
	,	[Totalisor1]
	,	[Totalisor2]
	,	[FuelingState]
	,	[Volume]
	,	[MeterReading]
	,	[Consecutive_OOS_Variance]
	,	[Notes]
	,	[Capacity]
	,	[SafeFill]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[MassUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[EquipmentSequence]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[SerialNumber]
	,	[CompanyEquipmentID]
	,	[TruckCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[RatedGPM]
	,	[ActualGPM]
	,	[FuelAdditiveFlag]
	,	[ManufactureDate]
	,	[InstallationDate]
	,	[InspectionDate]
	,	[CalibrationDate]
	,	[QCDate]
	,	[SecondaryStorageFlag]
	,	[ManagedEquipmentFlag]
	,	[FuelingType]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[UserData9]
	,	[UserData10]
	,	[UserData11]
	,	[UserData12]
	,	[UserData13]
	,	[UserData14]
	,	[UserData15]
	,	[UserData16]
	,	[UserData17]
	,	[UserData18]
	,	[UserData19]
	,	[UserData20]
	,	[UserData21]
	,	[UserData22]
	,	[UserData23]
	,	[UserData24]
	,	[OriginalRowVersion]
	,	[EquipmentGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[ParentEquipmentGuid]
	,	[EquipmentTypeGuid]
	,	[FuelCardGuid]
	,	[ProductGuid]
	,	[AssignedToMeterGuid]
	,	[AssetTrackingDeviceGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[ScullyRequired]
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
	OUTPUT inserted.[EquipmentGuid] AS 'EquipmentGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ID]
	,	d.[Description]
	,	d.[Make]
	,	d.[Model]
	,	d.[Year]
	,	d.[IssPtNum]
	,	d.[Fixed]
	,	d.[StorageType]
	,	d.[InUse]
	,	d.[FixedVolume]
	,	d.[IntoPlane]
	,	d.[Mobile]
	,	d.[AttachedTo]
	,	d.[MediaType]
	,	d.[Meters]
	,	d.[DefuelMeterForwards]
	,	d.[PulseRatio]
	,	d.[Round]
	,	d.[Xref]
	,	d.[LowStockWarning]
	,	d.[StockTrack]
	,	d.[Totalisor1]
	,	d.[Totalisor2]
	,	d.[FuelingState]
	,	d.[Volume]
	,	d.[MeterReading]
	,	d.[Consecutive_OOS_Variance]
	,	d.[Notes]
	,	d.[Capacity]
	,	d.[SafeFill]
	,	d.[VolumeUnitIndex]
	,	d.[TemperatureUnitIndex]
	,	d.[DensityUnitIndex]
	,	d.[MassUnitIndex]
	,	d.[VolumeDecimalPlaces]
	,	d.[TemperatureDecimalPlaces]
	,	d.[DensityDecimalPlaces]
	,	d.[MassDecimalPlaces]
	,	d.[EquipmentSequence]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[SerialNumber]
	,	d.[CompanyEquipmentID]
	,	d.[TruckCardNumber]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[RatedGPM]
	,	d.[ActualGPM]
	,	d.[FuelAdditiveFlag]
	,	d.[ManufactureDate]
	,	d.[InstallationDate]
	,	d.[InspectionDate]
	,	d.[CalibrationDate]
	,	d.[QCDate]
	,	d.[SecondaryStorageFlag]
	,	d.[ManagedEquipmentFlag]
	,	d.[FuelingType]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[UserData9]
	,	d.[UserData10]
	,	d.[UserData11]
	,	d.[UserData12]
	,	d.[UserData13]
	,	d.[UserData14]
	,	d.[UserData15]
	,	d.[UserData16]
	,	d.[UserData17]
	,	d.[UserData18]
	,	d.[UserData19]
	,	d.[UserData20]
	,	d.[UserData21]
	,	d.[UserData22]
	,	d.[UserData23]
	,	d.[UserData24]
	,	d.[_RowVersion]
	,	d.[EquipmentGuid]
	,	d.[SiteGuid]
	,	d.[CompanyGuid]
	,	d.[ParentEquipmentGuid]
	,	d.[EquipmentTypeGuid]
	,	d.[FuelCardGuid]
	,	d.[ProductGuid]
	,	d.[AssignedToMeterGuid]
	,	d.[AssetTrackingDeviceGuid]
	,	d.[_MasterRecordGuid]
	,	d.[HiddenDate]
	,	d.[ScullyRequired]
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
 
	INSERT INTO [fmaudit].tblEquipment (
		[ID]
	,	[Description]
	,	[Make]
	,	[Model]
	,	[Year]
	,	[IssPtNum]
	,	[Fixed]
	,	[StorageType]
	,	[InUse]
	,	[FixedVolume]
	,	[IntoPlane]
	,	[Mobile]
	,	[AttachedTo]
	,	[MediaType]
	,	[Meters]
	,	[DefuelMeterForwards]
	,	[PulseRatio]
	,	[Round]
	,	[Xref]
	,	[LowStockWarning]
	,	[StockTrack]
	,	[Totalisor1]
	,	[Totalisor2]
	,	[FuelingState]
	,	[Volume]
	,	[MeterReading]
	,	[Consecutive_OOS_Variance]
	,	[Notes]
	,	[Capacity]
	,	[SafeFill]
	,	[VolumeUnitIndex]
	,	[TemperatureUnitIndex]
	,	[DensityUnitIndex]
	,	[MassUnitIndex]
	,	[VolumeDecimalPlaces]
	,	[TemperatureDecimalPlaces]
	,	[DensityDecimalPlaces]
	,	[MassDecimalPlaces]
	,	[EquipmentSequence]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[SerialNumber]
	,	[CompanyEquipmentID]
	,	[TruckCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[RatedGPM]
	,	[ActualGPM]
	,	[FuelAdditiveFlag]
	,	[ManufactureDate]
	,	[InstallationDate]
	,	[InspectionDate]
	,	[CalibrationDate]
	,	[QCDate]
	,	[SecondaryStorageFlag]
	,	[ManagedEquipmentFlag]
	,	[FuelingType]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[UserData9]
	,	[UserData10]
	,	[UserData11]
	,	[UserData12]
	,	[UserData13]
	,	[UserData14]
	,	[UserData15]
	,	[UserData16]
	,	[UserData17]
	,	[UserData18]
	,	[UserData19]
	,	[UserData20]
	,	[UserData21]
	,	[UserData22]
	,	[UserData23]
	,	[UserData24]
	,	[OriginalRowVersion]
	,	[EquipmentGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[ParentEquipmentGuid]
	,	[EquipmentTypeGuid]
	,	[FuelCardGuid]
	,	[ProductGuid]
	,	[AssignedToMeterGuid]
	,	[AssetTrackingDeviceGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
	,	[ScullyRequired]
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
	,	i.[Make]
	,	i.[Model]
	,	i.[Year]
	,	i.[IssPtNum]
	,	i.[Fixed]
	,	i.[StorageType]
	,	i.[InUse]
	,	i.[FixedVolume]
	,	i.[IntoPlane]
	,	i.[Mobile]
	,	i.[AttachedTo]
	,	i.[MediaType]
	,	i.[Meters]
	,	i.[DefuelMeterForwards]
	,	i.[PulseRatio]
	,	i.[Round]
	,	i.[Xref]
	,	i.[LowStockWarning]
	,	i.[StockTrack]
	,	i.[Totalisor1]
	,	i.[Totalisor2]
	,	i.[FuelingState]
	,	i.[Volume]
	,	i.[MeterReading]
	,	i.[Consecutive_OOS_Variance]
	,	i.[Notes]
	,	i.[Capacity]
	,	i.[SafeFill]
	,	i.[VolumeUnitIndex]
	,	i.[TemperatureUnitIndex]
	,	i.[DensityUnitIndex]
	,	i.[MassUnitIndex]
	,	i.[VolumeDecimalPlaces]
	,	i.[TemperatureDecimalPlaces]
	,	i.[DensityDecimalPlaces]
	,	i.[MassDecimalPlaces]
	,	i.[EquipmentSequence]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[SerialNumber]
	,	i.[CompanyEquipmentID]
	,	i.[TruckCardNumber]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[RatedGPM]
	,	i.[ActualGPM]
	,	i.[FuelAdditiveFlag]
	,	i.[ManufactureDate]
	,	i.[InstallationDate]
	,	i.[InspectionDate]
	,	i.[CalibrationDate]
	,	i.[QCDate]
	,	i.[SecondaryStorageFlag]
	,	i.[ManagedEquipmentFlag]
	,	i.[FuelingType]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[UserData9]
	,	i.[UserData10]
	,	i.[UserData11]
	,	i.[UserData12]
	,	i.[UserData13]
	,	i.[UserData14]
	,	i.[UserData15]
	,	i.[UserData16]
	,	i.[UserData17]
	,	i.[UserData18]
	,	i.[UserData19]
	,	i.[UserData20]
	,	i.[UserData21]
	,	i.[UserData22]
	,	i.[UserData23]
	,	i.[UserData24]
	,	i.[_RowVersion]
	,	i.[EquipmentGuid]
	,	i.[SiteGuid]
	,	i.[CompanyGuid]
	,	i.[ParentEquipmentGuid]
	,	i.[EquipmentTypeGuid]
	,	i.[FuelCardGuid]
	,	i.[ProductGuid]
	,	i.[AssignedToMeterGuid]
	,	i.[AssetTrackingDeviceGuid]
	,	i.[_MasterRecordGuid]
	,	i.[HiddenDate]
	,	i.[ScullyRequired]
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
			agl.[EquipmentGuid]=i.[EquipmentGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblEquipment]
ON [dbo].[tblEquipment]
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
		INSERT INTO fmcdc.[tblEquipment]
		(
		[ID]
		, [Description]
		, [Make]
		, [Model]
		, [Year]
		, [IssPtNum]
		, [Fixed]
		, [StorageType]
		, [InUse]
		, [FixedVolume]
		, [IntoPlane]
		, [Mobile]
		, [AttachedTo]
		, [MediaType]
		, [Meters]
		, [DefuelMeterForwards]
		, [PulseRatio]
		, [Round]
		, [Xref]
		, [LowStockWarning]
		, [StockTrack]
		, [Totalisor1]
		, [Totalisor2]
		, [FuelingState]
		, [Volume]
		, [MeterReading]
		, [Consecutive_OOS_Variance]
		, [Notes]
		, [Capacity]
		, [SafeFill]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [MassUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [MassDecimalPlaces]
		, [EquipmentSequence]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [SerialNumber]
		, [CompanyEquipmentID]
		, [TruckCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [RatedGPM]
		, [ActualGPM]
		, [FuelAdditiveFlag]
		, [ManufactureDate]
		, [InstallationDate]
		, [InspectionDate]
		, [CalibrationDate]
		, [QCDate]
		, [SecondaryStorageFlag]
		, [ManagedEquipmentFlag]
		, [FuelingType]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [UserData9]
		, [UserData10]
		, [UserData11]
		, [UserData12]
		, [UserData13]
		, [UserData14]
		, [UserData15]
		, [UserData16]
		, [UserData17]
		, [UserData18]
		, [UserData19]
		, [UserData20]
		, [UserData21]
		, [UserData22]
		, [UserData23]
		, [UserData24]
		, [SourceRowVersion]
		, [EquipmentGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [ParentEquipmentGuid]
		, [EquipmentTypeGuid]
		, [FuelCardGuid]
		, [ProductGuid]
		, [AssignedToMeterGuid]
		, [AssetTrackingDeviceGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ID]
		, [Description]
		, [Make]
		, [Model]
		, [Year]
		, [IssPtNum]
		, [Fixed]
		, [StorageType]
		, [InUse]
		, [FixedVolume]
		, [IntoPlane]
		, [Mobile]
		, [AttachedTo]
		, [MediaType]
		, [Meters]
		, [DefuelMeterForwards]
		, [PulseRatio]
		, [Round]
		, [Xref]
		, [LowStockWarning]
		, [StockTrack]
		, [Totalisor1]
		, [Totalisor2]
		, [FuelingState]
		, [Volume]
		, [MeterReading]
		, [Consecutive_OOS_Variance]
		, [Notes]
		, [Capacity]
		, [SafeFill]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [MassUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [MassDecimalPlaces]
		, [EquipmentSequence]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [SerialNumber]
		, [CompanyEquipmentID]
		, [TruckCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [RatedGPM]
		, [ActualGPM]
		, [FuelAdditiveFlag]
		, [ManufactureDate]
		, [InstallationDate]
		, [InspectionDate]
		, [CalibrationDate]
		, [QCDate]
		, [SecondaryStorageFlag]
		, [ManagedEquipmentFlag]
		, [FuelingType]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [UserData9]
		, [UserData10]
		, [UserData11]
		, [UserData12]
		, [UserData13]
		, [UserData14]
		, [UserData15]
		, [UserData16]
		, [UserData17]
		, [UserData18]
		, [UserData19]
		, [UserData20]
		, [UserData21]
		, [UserData22]
		, [UserData23]
		, [UserData24]
		, CONVERT(bigint, _RowVersion)
		, [EquipmentGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [ParentEquipmentGuid]
		, [EquipmentTypeGuid]
		, [FuelCardGuid]
		, [ProductGuid]
		, [AssignedToMeterGuid]
		, [AssetTrackingDeviceGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblEquipment]
		(
		[ID]
		, [Description]
		, [Make]
		, [Model]
		, [Year]
		, [IssPtNum]
		, [Fixed]
		, [StorageType]
		, [InUse]
		, [FixedVolume]
		, [IntoPlane]
		, [Mobile]
		, [AttachedTo]
		, [MediaType]
		, [Meters]
		, [DefuelMeterForwards]
		, [PulseRatio]
		, [Round]
		, [Xref]
		, [LowStockWarning]
		, [StockTrack]
		, [Totalisor1]
		, [Totalisor2]
		, [FuelingState]
		, [Volume]
		, [MeterReading]
		, [Consecutive_OOS_Variance]
		, [Notes]
		, [Capacity]
		, [SafeFill]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [MassUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [MassDecimalPlaces]
		, [EquipmentSequence]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [SerialNumber]
		, [CompanyEquipmentID]
		, [TruckCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [RatedGPM]
		, [ActualGPM]
		, [FuelAdditiveFlag]
		, [ManufactureDate]
		, [InstallationDate]
		, [InspectionDate]
		, [CalibrationDate]
		, [QCDate]
		, [SecondaryStorageFlag]
		, [ManagedEquipmentFlag]
		, [FuelingType]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [UserData9]
		, [UserData10]
		, [UserData11]
		, [UserData12]
		, [UserData13]
		, [UserData14]
		, [UserData15]
		, [UserData16]
		, [UserData17]
		, [UserData18]
		, [UserData19]
		, [UserData20]
		, [UserData21]
		, [UserData22]
		, [UserData23]
		, [UserData24]
		, [SourceRowVersion]
		, [EquipmentGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [ParentEquipmentGuid]
		, [EquipmentTypeGuid]
		, [FuelCardGuid]
		, [ProductGuid]
		, [AssignedToMeterGuid]
		, [AssetTrackingDeviceGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[ID]
		, [Description]
		, [Make]
		, [Model]
		, [Year]
		, [IssPtNum]
		, [Fixed]
		, [StorageType]
		, [InUse]
		, [FixedVolume]
		, [IntoPlane]
		, [Mobile]
		, [AttachedTo]
		, [MediaType]
		, [Meters]
		, [DefuelMeterForwards]
		, [PulseRatio]
		, [Round]
		, [Xref]
		, [LowStockWarning]
		, [StockTrack]
		, [Totalisor1]
		, [Totalisor2]
		, [FuelingState]
		, [Volume]
		, [MeterReading]
		, [Consecutive_OOS_Variance]
		, [Notes]
		, [Capacity]
		, [SafeFill]
		, [VolumeUnitIndex]
		, [TemperatureUnitIndex]
		, [DensityUnitIndex]
		, [MassUnitIndex]
		, [VolumeDecimalPlaces]
		, [TemperatureDecimalPlaces]
		, [DensityDecimalPlaces]
		, [MassDecimalPlaces]
		, [EquipmentSequence]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [SerialNumber]
		, [CompanyEquipmentID]
		, [TruckCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [RatedGPM]
		, [ActualGPM]
		, [FuelAdditiveFlag]
		, [ManufactureDate]
		, [InstallationDate]
		, [InspectionDate]
		, [CalibrationDate]
		, [QCDate]
		, [SecondaryStorageFlag]
		, [ManagedEquipmentFlag]
		, [FuelingType]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [UserData9]
		, [UserData10]
		, [UserData11]
		, [UserData12]
		, [UserData13]
		, [UserData14]
		, [UserData15]
		, [UserData16]
		, [UserData17]
		, [UserData18]
		, [UserData19]
		, [UserData20]
		, [UserData21]
		, [UserData22]
		, [UserData23]
		, [UserData24]
		, CONVERT(bigint, _RowVersion)
		, [EquipmentGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [ParentEquipmentGuid]
		, [EquipmentTypeGuid]
		, [FuelCardGuid]
		, [ProductGuid]
		, [AssignedToMeterGuid]
		, [AssetTrackingDeviceGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [ScullyRequired]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblEquipment] ON [dbo].[tblEquipment]
GO



CREATE NONCLUSTERED INDEX [IXU_tblEquipment_MasterRecordGuid] ON [dbo].[tblEquipment]
([_MasterRecordGuid] ASC)
INCLUDE ([ID],	[CompanyEquipmentID])
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEquipment_ClusterIdx]
    ON [dbo].[tblEquipment]([_ClusterIdx] ASC);
