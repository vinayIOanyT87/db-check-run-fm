CREATE TABLE [dbo].[tblDispatchConfiguration] (
    [DispatchConfigurationGuid]                UNIQUEIDENTIFIER   CONSTRAINT [DF_tblDispatchConfiguration_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]                                 UNIQUEIDENTIFIER   NOT NULL,
    [ID]                                       NVARCHAR (50)      CONSTRAINT [DF_tblDispatchConfiguration_ID] DEFAULT ('') NOT NULL,
    [DisplayCurrentTime]                       BIT                CONSTRAINT [DF_tblDispatchConfiguration_DisplayCurrentTime] DEFAULT ((0)) NOT NULL,
    [DispatchDataRefreshPeriod]                INT                CONSTRAINT [DF_tblDispatchConfiguration_DispatchDataRefreshPeriod] DEFAULT ((5)) NOT NULL,
    [TabularViewDisplayMilitaryDate]           BIT                CONSTRAINT [DF_tblDispatchConfiguration_TabularViewDisplayMilitaryDate] DEFAULT ((0)) NOT NULL,
    [QuantityNotZeroCheck]                     BIT                CONSTRAINT [DF_tblDispatchConfiguration_QuantityNotZeroCheck] DEFAULT ((0)) NOT NULL,
    [ExactlyOneManagerCheck]                   BIT                CONSTRAINT [DF_tblDispatchConfiguration_ExactlyOneManagerCheck] DEFAULT ((0)) NOT NULL,
    [ExactlyOneOwnerCheck]                     BIT                CONSTRAINT [DF_tblDispatchConfiguration_ExactlyOneOwnerCheck] DEFAULT ((0)) NOT NULL,
    [DispatchFuelAdditiveFlagCheck]            BIT                CONSTRAINT [DF_tblDispatchConfiguration_DispatchFuelAdditiveFlagCheck] DEFAULT ((0)) NOT NULL,
    [FastLogFuelAdditiveFlagCheck]             BIT                CONSTRAINT [DF_tblDispatchConfiguration_FastLogFuelAdditiveFlagCheck] DEFAULT ((0)) NOT NULL,
    [FillstandVolumeWithinToleranceCheck]      BIT                CONSTRAINT [DF_tblDispatchConfiguration_FillstandVolumeWithinToleranceCheck] DEFAULT ((0)) NOT NULL,
    [ReturnToBulkVolumeWithinToleranceCheck]   BIT                CONSTRAINT [DF_tblDispatchConfiguration_ReturnToBulkVolumeWithinToleranceCheck] DEFAULT ((0)) NOT NULL,
    [RecirculationVolumesGreaterThanZeroCheck] BIT                CONSTRAINT [DF_tblDispatchConfiguration_RecirculationVolumesGreaterThanZeroCheck] DEFAULT ((0)) NOT NULL,
    [OperatorIsInCheck]                        BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorIsInCheck] DEFAULT ((0)) NOT NULL,
    [OperatorNotAssignedCheck]                 BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorNotAssignedCheck] DEFAULT ((0)) NOT NULL,
    [OperatorHasRequiredTrainingCheck]         BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorHasRequiredTrainingCheck] DEFAULT ((0)) NOT NULL,
    [OperatorTrainingNotExpiredCheck]          BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorTrainingNotExpiredCheck] DEFAULT ((0)) NOT NULL,
    [OperatorNotLockedOutCheck]                BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorNotLockedOutCheck] DEFAULT ((0)) NOT NULL,
    [OperatorHasRequiredQualificationsCheck]   BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorHasRequiredQualificationsCheck] DEFAULT ((0)) NOT NULL,
    [OperatorQualificationsNotExpiredCheck]    BIT                CONSTRAINT [DF_tblDispatchConfiguration_OperatorQualificationsNotExpiredCheck] DEFAULT ((0)) NOT NULL,
    [DefuelStatusCheck]                        BIT                CONSTRAINT [DF_tblDispatchConfiguration_DefuelStatusCheck] DEFAULT ((0)) NOT NULL,
    [RefuelStatusCheck]                        BIT                CONSTRAINT [DF_tblDispatchConfiguration_RefuelStatusCheck] DEFAULT ((0)) NOT NULL,
    [EquipmentFuelGradeCheck]                  BIT                CONSTRAINT [DF_tblDispatchConfiguration_EquipmentFuelGradeCheck] DEFAULT ((0)) NOT NULL,
    [EquipmentNotLockedOutCheck]               BIT                CONSTRAINT [DF_tblDispatchConfiguration_EquipmentNotLockedOutCheck] DEFAULT ((0)) NOT NULL,
    [EquipmentNotAssignedCheck]                BIT                CONSTRAINT [DF_tblDispatchConfiguration_EquipmentNotAssignedCheck] DEFAULT ((0)) NOT NULL,
    [EquipmentInServiceCheck]                  BIT                CONSTRAINT [DF_tblDispatchConfiguration_EquipmentInServiceCheck] DEFAULT ((0)) NOT NULL,
    [TagLicenseNotExpiredCheck]                BIT                CONSTRAINT [DF_tblDispatchConfiguration_TagLicenseNotExpiredCheck] DEFAULT ((0)) NOT NULL,
    [TestInspectionNotExpiredCheck]            BIT                CONSTRAINT [DF_tblDispatchConfiguration_TestInspectionNotExpiredCheck] DEFAULT ((0)) NOT NULL,
    [QualityControlCheckupDateCheck]           BIT                CONSTRAINT [DF_tblDispatchConfiguration_QualityControlCheckupDateCheck] DEFAULT ((0)) NOT NULL,
    [CautionQualityTagCheck]                   BIT                CONSTRAINT [DF_tblDispatchConfiguration_CautionQualityTagCheck] DEFAULT ((0)) NOT NULL,
    [WarningQualityTagCheck]                   BIT                CONSTRAINT [DF_tblDispatchConfiguration_WarningQualityTagCheck] DEFAULT ((0)) NOT NULL,
    [DangerQualityTagCheck]                    BIT                CONSTRAINT [DF_tblDispatchConfiguration_DangerQualityTagCheck] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                              DATETIMEOFFSET (7) CONSTRAINT [DF_tblDispatchConfiguration_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                                [dbo].[udtUserID]  CONSTRAINT [DF_tblDispatchConfiguration_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                              DATETIMEOFFSET (7) CONSTRAINT [DF_tblDispatchConfiguration_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                                [dbo].[udtUserID]  CONSTRAINT [DF_tblDispatchConfiguration_UpdatedBy] DEFAULT ('') NULL,
    [_RowVersion]                              ROWVERSION         NOT NULL,
    [EnableServiceRequests]                    BIT                CONSTRAINT [DF_tblDispatchConfiguration_EnableServiceRequests] DEFAULT ((1)) NOT NULL,
    [AutomaticRestartDelay]                    INT                CONSTRAINT [DF_tblDispatchConfiguration_AutomaticRestartDelay] DEFAULT ((30)) NOT NULL,
    [EquipmentRequired]                        BIT                CONSTRAINT [DF_tblDispatchConfiguration_EquipmentRequired] DEFAULT ((0)) NOT NULL,
    [PersonnelRequired]                        BIT                CONSTRAINT [DF_tblDispatchConfiguration_PersonnelRequired] DEFAULT ((0)) NOT NULL,
    [FillToActualOrStandard]                   INT                CONSTRAINT [DF_tblDispatchConfiguration_FillToActualOrStandard] DEFAULT ((0)) NULL,
    [OperationalWindowPastHours]               INT                CONSTRAINT [DF_tblDispatchConfiguration_OperationalWindowPastHours] DEFAULT ((8)) NOT NULL,
    [OperationalWindowFutureHours]             INT                CONSTRAINT [DF_tblDispatchConfiguration_OperationalWindowFutureHours] DEFAULT ((16)) NOT NULL,
    [ShowGridLines]                            BIT                CONSTRAINT [DF_tblDispatchConfiguration_ShowGridLines] DEFAULT ((0)) NOT NULL,
    [StaticTimeDisplay]                        BIT                CONSTRAINT [DF_tblDispatchConfiguration_StaticTimeDisplay] DEFAULT ((0)) NOT NULL,
    [UseArrivalTime]                           BIT                CONSTRAINT [DF_tblDispatchConfiguration_UseArrivalTime] DEFAULT ((0)) NOT NULL,
    [UseStartTime]                             BIT                CONSTRAINT [DF_tblDispatchConfiguration_UseStartTime] DEFAULT ((0)) NOT NULL,
    [UseStopTime]                              BIT                CONSTRAINT [DF_tblDispatchConfiguration_UseStopTime] DEFAULT ((0)) NOT NULL,
    [FuelsManagerReportURL]                    NVARCHAR (MAX)     NULL,
    [_ClusterIdx]                              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblDispatchConfiguration_GUID] PRIMARY KEY NONCLUSTERED ([DispatchConfigurationGuid] ASC),
    CONSTRAINT [CK_tblDispatchConfiguration_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessDispatchConfiguration]([DispatchConfigurationGuid],[SiteGuid],[ID])=(1)),
    CONSTRAINT [FK_tblDispatchConfiguration_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblDispatchConfiguration_CreatedDate]
    ON [dbo].[tblDispatchConfiguration]([CreatedDate] ASC);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblDispatchConfiguration_ID_SiteGuid]
    ON [dbo].[tblDispatchConfiguration]([ID] ASC, [SiteGuid] ASC);


GO
--Creating Insert / Update Trigger for tblDispatchConfiguration
CREATE TRIGGER dbo.trg_insupd_tblDispatchConfiguration_ForSync 
   ON dbo.tblDispatchConfiguration
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
                    ,d.DispatchConfigurationGuid AS Deleted_PK_DispatchConfigurationGuid
                    ,i.DispatchConfigurationGuid AS Inserted_PK_DispatchConfigurationGuid
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
				    d.DispatchConfigurationGuid = i.DispatchConfigurationGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblDispatchConfiguration As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_DispatchConfigurationGuid = currentTrackingData.PK_DispatchConfigurationGuid
 
 
		    INSERT track.tblDispatchConfiguration (InsertedDate 
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
				    ,PK_DispatchConfigurationGuid
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
				    ,entityChanges.Inserted_PK_DispatchConfigurationGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblDispatchConfiguration As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_DispatchConfigurationGuid = currentTrackingData.PK_DispatchConfigurationGuid
)
    END
END 

GO
--Creating Delete Trigger for tblDispatchConfiguration
CREATE TRIGGER dbo.trg_del_tblDispatchConfiguration_ForSync 
   ON dbo.tblDispatchConfiguration
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
						,d.DispatchConfigurationGuid AS Deleted_PK_DispatchConfigurationGuid
                        ,d.DispatchConfigurationGuid AS Inserted_PK_DispatchConfigurationGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblDispatchConfiguration As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_DispatchConfigurationGuid = currentTrackingData.PK_DispatchConfigurationGuid
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
						,PK_DispatchConfigurationGuid
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
						,entityChanges.Deleted_PK_DispatchConfigurationGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblDispatchConfiguration] ON [dbo].[tblDispatchConfiguration] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblDispatchConfiguration (
		[DispatchConfigurationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[DisplayCurrentTime]
	,	[DispatchDataRefreshPeriod]
	,	[TabularViewDisplayMilitaryDate]
	,	[QuantityNotZeroCheck]
	,	[ExactlyOneManagerCheck]
	,	[ExactlyOneOwnerCheck]
	,	[DispatchFuelAdditiveFlagCheck]
	,	[FastLogFuelAdditiveFlagCheck]
	,	[FillstandVolumeWithinToleranceCheck]
	,	[ReturnToBulkVolumeWithinToleranceCheck]
	,	[RecirculationVolumesGreaterThanZeroCheck]
	,	[OperatorIsInCheck]
	,	[OperatorNotAssignedCheck]
	,	[OperatorHasRequiredTrainingCheck]
	,	[OperatorTrainingNotExpiredCheck]
	,	[OperatorNotLockedOutCheck]
	,	[OperatorHasRequiredQualificationsCheck]
	,	[OperatorQualificationsNotExpiredCheck]
	,	[DefuelStatusCheck]
	,	[RefuelStatusCheck]
	,	[EquipmentFuelGradeCheck]
	,	[EquipmentNotLockedOutCheck]
	,	[EquipmentNotAssignedCheck]
	,	[EquipmentInServiceCheck]
	,	[TagLicenseNotExpiredCheck]
	,	[TestInspectionNotExpiredCheck]
	,	[QualityControlCheckupDateCheck]
	,	[CautionQualityTagCheck]
	,	[WarningQualityTagCheck]
	,	[DangerQualityTagCheck]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[EnableServiceRequests]
	,	[AutomaticRestartDelay]
	,	[EquipmentRequired]
	,	[PersonnelRequired]
	,	[FillToActualOrStandard]
	,	[OperationalWindowPastHours]
	,	[OperationalWindowFutureHours]
	,	[ShowGridLines]
	,	[StaticTimeDisplay]
	,	[UseArrivalTime]
	,	[UseStartTime]
	,	[UseStopTime]
	,	[FuelsManagerReportURL]
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
		d.[DispatchConfigurationGuid]
	,	d.[SiteGuid]
	,	d.[ID]
	,	d.[DisplayCurrentTime]
	,	d.[DispatchDataRefreshPeriod]
	,	d.[TabularViewDisplayMilitaryDate]
	,	d.[QuantityNotZeroCheck]
	,	d.[ExactlyOneManagerCheck]
	,	d.[ExactlyOneOwnerCheck]
	,	d.[DispatchFuelAdditiveFlagCheck]
	,	d.[FastLogFuelAdditiveFlagCheck]
	,	d.[FillstandVolumeWithinToleranceCheck]
	,	d.[ReturnToBulkVolumeWithinToleranceCheck]
	,	d.[RecirculationVolumesGreaterThanZeroCheck]
	,	d.[OperatorIsInCheck]
	,	d.[OperatorNotAssignedCheck]
	,	d.[OperatorHasRequiredTrainingCheck]
	,	d.[OperatorTrainingNotExpiredCheck]
	,	d.[OperatorNotLockedOutCheck]
	,	d.[OperatorHasRequiredQualificationsCheck]
	,	d.[OperatorQualificationsNotExpiredCheck]
	,	d.[DefuelStatusCheck]
	,	d.[RefuelStatusCheck]
	,	d.[EquipmentFuelGradeCheck]
	,	d.[EquipmentNotLockedOutCheck]
	,	d.[EquipmentNotAssignedCheck]
	,	d.[EquipmentInServiceCheck]
	,	d.[TagLicenseNotExpiredCheck]
	,	d.[TestInspectionNotExpiredCheck]
	,	d.[QualityControlCheckupDateCheck]
	,	d.[CautionQualityTagCheck]
	,	d.[WarningQualityTagCheck]
	,	d.[DangerQualityTagCheck]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[EnableServiceRequests]
	,	d.[AutomaticRestartDelay]
	,	d.[EquipmentRequired]
	,	d.[PersonnelRequired]
	,	d.[FillToActualOrStandard]
	,	d.[OperationalWindowPastHours]
	,	d.[OperationalWindowFutureHours]
	,	d.[ShowGridLines]
	,	d.[StaticTimeDisplay]
	,	d.[UseArrivalTime]
	,	d.[UseStartTime]
	,	d.[UseStopTime]
	,	d.[FuelsManagerReportURL]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblDispatchConfiguration] ON [dbo].[tblDispatchConfiguration] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblDispatchConfiguration (
		[DispatchConfigurationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[DisplayCurrentTime]
	,	[DispatchDataRefreshPeriod]
	,	[TabularViewDisplayMilitaryDate]
	,	[QuantityNotZeroCheck]
	,	[ExactlyOneManagerCheck]
	,	[ExactlyOneOwnerCheck]
	,	[DispatchFuelAdditiveFlagCheck]
	,	[FastLogFuelAdditiveFlagCheck]
	,	[FillstandVolumeWithinToleranceCheck]
	,	[ReturnToBulkVolumeWithinToleranceCheck]
	,	[RecirculationVolumesGreaterThanZeroCheck]
	,	[OperatorIsInCheck]
	,	[OperatorNotAssignedCheck]
	,	[OperatorHasRequiredTrainingCheck]
	,	[OperatorTrainingNotExpiredCheck]
	,	[OperatorNotLockedOutCheck]
	,	[OperatorHasRequiredQualificationsCheck]
	,	[OperatorQualificationsNotExpiredCheck]
	,	[DefuelStatusCheck]
	,	[RefuelStatusCheck]
	,	[EquipmentFuelGradeCheck]
	,	[EquipmentNotLockedOutCheck]
	,	[EquipmentNotAssignedCheck]
	,	[EquipmentInServiceCheck]
	,	[TagLicenseNotExpiredCheck]
	,	[TestInspectionNotExpiredCheck]
	,	[QualityControlCheckupDateCheck]
	,	[CautionQualityTagCheck]
	,	[WarningQualityTagCheck]
	,	[DangerQualityTagCheck]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[EnableServiceRequests]
	,	[AutomaticRestartDelay]
	,	[EquipmentRequired]
	,	[PersonnelRequired]
	,	[FillToActualOrStandard]
	,	[OperationalWindowPastHours]
	,	[OperationalWindowFutureHours]
	,	[ShowGridLines]
	,	[StaticTimeDisplay]
	,	[UseArrivalTime]
	,	[UseStartTime]
	,	[UseStopTime]
	,	[FuelsManagerReportURL]
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
		i.[DispatchConfigurationGuid]
	,	i.[SiteGuid]
	,	i.[ID]
	,	i.[DisplayCurrentTime]
	,	i.[DispatchDataRefreshPeriod]
	,	i.[TabularViewDisplayMilitaryDate]
	,	i.[QuantityNotZeroCheck]
	,	i.[ExactlyOneManagerCheck]
	,	i.[ExactlyOneOwnerCheck]
	,	i.[DispatchFuelAdditiveFlagCheck]
	,	i.[FastLogFuelAdditiveFlagCheck]
	,	i.[FillstandVolumeWithinToleranceCheck]
	,	i.[ReturnToBulkVolumeWithinToleranceCheck]
	,	i.[RecirculationVolumesGreaterThanZeroCheck]
	,	i.[OperatorIsInCheck]
	,	i.[OperatorNotAssignedCheck]
	,	i.[OperatorHasRequiredTrainingCheck]
	,	i.[OperatorTrainingNotExpiredCheck]
	,	i.[OperatorNotLockedOutCheck]
	,	i.[OperatorHasRequiredQualificationsCheck]
	,	i.[OperatorQualificationsNotExpiredCheck]
	,	i.[DefuelStatusCheck]
	,	i.[RefuelStatusCheck]
	,	i.[EquipmentFuelGradeCheck]
	,	i.[EquipmentNotLockedOutCheck]
	,	i.[EquipmentNotAssignedCheck]
	,	i.[EquipmentInServiceCheck]
	,	i.[TagLicenseNotExpiredCheck]
	,	i.[TestInspectionNotExpiredCheck]
	,	i.[QualityControlCheckupDateCheck]
	,	i.[CautionQualityTagCheck]
	,	i.[WarningQualityTagCheck]
	,	i.[DangerQualityTagCheck]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[EnableServiceRequests]
	,	i.[AutomaticRestartDelay]
	,	i.[EquipmentRequired]
	,	i.[PersonnelRequired]
	,	i.[FillToActualOrStandard]
	,	i.[OperationalWindowPastHours]
	,	i.[OperationalWindowFutureHours]
	,	i.[ShowGridLines]
	,	i.[StaticTimeDisplay]
	,	i.[UseArrivalTime]
	,	i.[UseStartTime]
	,	i.[UseStopTime]
	,	i.[FuelsManagerReportURL]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblDispatchConfiguration] ON [dbo].[tblDispatchConfiguration] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchConfiguration','D')=1 
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
	DispatchConfigurationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblDispatchConfiguration (
		[DispatchConfigurationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[DisplayCurrentTime]
	,	[DispatchDataRefreshPeriod]
	,	[TabularViewDisplayMilitaryDate]
	,	[QuantityNotZeroCheck]
	,	[ExactlyOneManagerCheck]
	,	[ExactlyOneOwnerCheck]
	,	[DispatchFuelAdditiveFlagCheck]
	,	[FastLogFuelAdditiveFlagCheck]
	,	[FillstandVolumeWithinToleranceCheck]
	,	[ReturnToBulkVolumeWithinToleranceCheck]
	,	[RecirculationVolumesGreaterThanZeroCheck]
	,	[OperatorIsInCheck]
	,	[OperatorNotAssignedCheck]
	,	[OperatorHasRequiredTrainingCheck]
	,	[OperatorTrainingNotExpiredCheck]
	,	[OperatorNotLockedOutCheck]
	,	[OperatorHasRequiredQualificationsCheck]
	,	[OperatorQualificationsNotExpiredCheck]
	,	[DefuelStatusCheck]
	,	[RefuelStatusCheck]
	,	[EquipmentFuelGradeCheck]
	,	[EquipmentNotLockedOutCheck]
	,	[EquipmentNotAssignedCheck]
	,	[EquipmentInServiceCheck]
	,	[TagLicenseNotExpiredCheck]
	,	[TestInspectionNotExpiredCheck]
	,	[QualityControlCheckupDateCheck]
	,	[CautionQualityTagCheck]
	,	[WarningQualityTagCheck]
	,	[DangerQualityTagCheck]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[EnableServiceRequests]
	,	[AutomaticRestartDelay]
	,	[EquipmentRequired]
	,	[PersonnelRequired]
	,	[FillToActualOrStandard]
	,	[OperationalWindowPastHours]
	,	[OperationalWindowFutureHours]
	,	[ShowGridLines]
	,	[StaticTimeDisplay]
	,	[UseArrivalTime]
	,	[UseStartTime]
	,	[UseStopTime]
	,	[FuelsManagerReportURL]
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
	OUTPUT inserted.[DispatchConfigurationGuid] AS 'DispatchConfigurationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[DispatchConfigurationGuid]
	,	d.[SiteGuid]
	,	d.[ID]
	,	d.[DisplayCurrentTime]
	,	d.[DispatchDataRefreshPeriod]
	,	d.[TabularViewDisplayMilitaryDate]
	,	d.[QuantityNotZeroCheck]
	,	d.[ExactlyOneManagerCheck]
	,	d.[ExactlyOneOwnerCheck]
	,	d.[DispatchFuelAdditiveFlagCheck]
	,	d.[FastLogFuelAdditiveFlagCheck]
	,	d.[FillstandVolumeWithinToleranceCheck]
	,	d.[ReturnToBulkVolumeWithinToleranceCheck]
	,	d.[RecirculationVolumesGreaterThanZeroCheck]
	,	d.[OperatorIsInCheck]
	,	d.[OperatorNotAssignedCheck]
	,	d.[OperatorHasRequiredTrainingCheck]
	,	d.[OperatorTrainingNotExpiredCheck]
	,	d.[OperatorNotLockedOutCheck]
	,	d.[OperatorHasRequiredQualificationsCheck]
	,	d.[OperatorQualificationsNotExpiredCheck]
	,	d.[DefuelStatusCheck]
	,	d.[RefuelStatusCheck]
	,	d.[EquipmentFuelGradeCheck]
	,	d.[EquipmentNotLockedOutCheck]
	,	d.[EquipmentNotAssignedCheck]
	,	d.[EquipmentInServiceCheck]
	,	d.[TagLicenseNotExpiredCheck]
	,	d.[TestInspectionNotExpiredCheck]
	,	d.[QualityControlCheckupDateCheck]
	,	d.[CautionQualityTagCheck]
	,	d.[WarningQualityTagCheck]
	,	d.[DangerQualityTagCheck]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[EnableServiceRequests]
	,	d.[AutomaticRestartDelay]
	,	d.[EquipmentRequired]
	,	d.[PersonnelRequired]
	,	d.[FillToActualOrStandard]
	,	d.[OperationalWindowPastHours]
	,	d.[OperationalWindowFutureHours]
	,	d.[ShowGridLines]
	,	d.[StaticTimeDisplay]
	,	d.[UseArrivalTime]
	,	d.[UseStartTime]
	,	d.[UseStopTime]
	,	d.[FuelsManagerReportURL]
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
 
	INSERT INTO [fmaudit].tblDispatchConfiguration (
		[DispatchConfigurationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[DisplayCurrentTime]
	,	[DispatchDataRefreshPeriod]
	,	[TabularViewDisplayMilitaryDate]
	,	[QuantityNotZeroCheck]
	,	[ExactlyOneManagerCheck]
	,	[ExactlyOneOwnerCheck]
	,	[DispatchFuelAdditiveFlagCheck]
	,	[FastLogFuelAdditiveFlagCheck]
	,	[FillstandVolumeWithinToleranceCheck]
	,	[ReturnToBulkVolumeWithinToleranceCheck]
	,	[RecirculationVolumesGreaterThanZeroCheck]
	,	[OperatorIsInCheck]
	,	[OperatorNotAssignedCheck]
	,	[OperatorHasRequiredTrainingCheck]
	,	[OperatorTrainingNotExpiredCheck]
	,	[OperatorNotLockedOutCheck]
	,	[OperatorHasRequiredQualificationsCheck]
	,	[OperatorQualificationsNotExpiredCheck]
	,	[DefuelStatusCheck]
	,	[RefuelStatusCheck]
	,	[EquipmentFuelGradeCheck]
	,	[EquipmentNotLockedOutCheck]
	,	[EquipmentNotAssignedCheck]
	,	[EquipmentInServiceCheck]
	,	[TagLicenseNotExpiredCheck]
	,	[TestInspectionNotExpiredCheck]
	,	[QualityControlCheckupDateCheck]
	,	[CautionQualityTagCheck]
	,	[WarningQualityTagCheck]
	,	[DangerQualityTagCheck]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[EnableServiceRequests]
	,	[AutomaticRestartDelay]
	,	[EquipmentRequired]
	,	[PersonnelRequired]
	,	[FillToActualOrStandard]
	,	[OperationalWindowPastHours]
	,	[OperationalWindowFutureHours]
	,	[ShowGridLines]
	,	[StaticTimeDisplay]
	,	[UseArrivalTime]
	,	[UseStartTime]
	,	[UseStopTime]
	,	[FuelsManagerReportURL]
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
		i.[DispatchConfigurationGuid]
	,	i.[SiteGuid]
	,	i.[ID]
	,	i.[DisplayCurrentTime]
	,	i.[DispatchDataRefreshPeriod]
	,	i.[TabularViewDisplayMilitaryDate]
	,	i.[QuantityNotZeroCheck]
	,	i.[ExactlyOneManagerCheck]
	,	i.[ExactlyOneOwnerCheck]
	,	i.[DispatchFuelAdditiveFlagCheck]
	,	i.[FastLogFuelAdditiveFlagCheck]
	,	i.[FillstandVolumeWithinToleranceCheck]
	,	i.[ReturnToBulkVolumeWithinToleranceCheck]
	,	i.[RecirculationVolumesGreaterThanZeroCheck]
	,	i.[OperatorIsInCheck]
	,	i.[OperatorNotAssignedCheck]
	,	i.[OperatorHasRequiredTrainingCheck]
	,	i.[OperatorTrainingNotExpiredCheck]
	,	i.[OperatorNotLockedOutCheck]
	,	i.[OperatorHasRequiredQualificationsCheck]
	,	i.[OperatorQualificationsNotExpiredCheck]
	,	i.[DefuelStatusCheck]
	,	i.[RefuelStatusCheck]
	,	i.[EquipmentFuelGradeCheck]
	,	i.[EquipmentNotLockedOutCheck]
	,	i.[EquipmentNotAssignedCheck]
	,	i.[EquipmentInServiceCheck]
	,	i.[TagLicenseNotExpiredCheck]
	,	i.[TestInspectionNotExpiredCheck]
	,	i.[QualityControlCheckupDateCheck]
	,	i.[CautionQualityTagCheck]
	,	i.[WarningQualityTagCheck]
	,	i.[DangerQualityTagCheck]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[EnableServiceRequests]
	,	i.[AutomaticRestartDelay]
	,	i.[EquipmentRequired]
	,	i.[PersonnelRequired]
	,	i.[FillToActualOrStandard]
	,	i.[OperationalWindowPastHours]
	,	i.[OperationalWindowFutureHours]
	,	i.[ShowGridLines]
	,	i.[StaticTimeDisplay]
	,	i.[UseArrivalTime]
	,	i.[UseStartTime]
	,	i.[UseStopTime]
	,	i.[FuelsManagerReportURL]
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
			agl.[DispatchConfigurationGuid]=i.[DispatchConfigurationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDispatchConfiguration_ClusterIdx]
    ON [dbo].[tblDispatchConfiguration]([_ClusterIdx] ASC);
