CREATE TABLE [dbo].[tblMeter] (
    [MeterGuid]            UNIQUEIDENTIFIER   CONSTRAINT [DF_tblMeters_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]             UNIQUEIDENTIFIER   NOT NULL,
    [MeterID]              NVARCHAR (30)      NOT NULL,
    [NumberOfDigits]       TINYINT            NOT NULL,
    [RotatesBackwardsFlag] BIT                NOT NULL,
    [ReceiptMeterFlag]     BIT                NOT NULL,
    [MeterFactor]		   FLOAT (53)         CONSTRAINT [DF_tblMeter_MeterFactor] DEFAULT ((1.0)) NULL,
    [FuelCompressionFactor]FLOAT (53)         CONSTRAINT [DF_tblMeter_FuelCompressionFactor] DEFAULT ((1.0)) NULL,
    [CreatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblMeters_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblMeters_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblMeters_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblMeters_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [DcuID]                NVARCHAR (50)      CONSTRAINT [DF_tblMeter_DcuID] DEFAULT (N'') NULL,
    [DcuBatteryVoltage]    FLOAT (53)         CONSTRAINT [DF_tblMeter_DcuBatteryVoltage] DEFAULT ((0.0)) NULL,
    [DcuBatteryCurrent]    FLOAT (53)         CONSTRAINT [DF_tblMeter_DcuBatteryCurrent] DEFAULT ((0.0)) NULL,
    [DcuTemperature]       FLOAT (53)         CONSTRAINT [DF_tblMeter_DcuTemperature] DEFAULT ((0.0)) NULL,
    [DcuResets]            INT                CONSTRAINT [DF_tblMeter_DcuResets] DEFAULT ((0)) NULL,
    [DcuUpdateDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblMeter_DcuUpdateDate] DEFAULT ('January 1, 1900') NULL,
    [DcuConfigurationDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblMeter_DcuConfigurationDate] DEFAULT ('January 1, 1900') NULL,
    [DcuFirmwareVersion]   NVARCHAR (50)      CONSTRAINT [DF_tblMeter_DcuFirmwareVersion] DEFAULT (N'') NULL,
    [DcuBluetoothAddress]  NVARCHAR (50)      CONSTRAINT [DF_tblMeter_DcuBluetoothAddress] DEFAULT (N'') NULL,
    [_ClusterIdx] BIGINT NOT NULL IDENTITY, 
    CONSTRAINT [PK_tblMeter] PRIMARY KEY NONCLUSTERED ([MeterGuid] ASC),
    CONSTRAINT [FK_tblMeters_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMeter__ClusterIdx] 
	ON [dbo].[tblMeter] ([_ClusterIdx]);

GO

CREATE NONCLUSTERED INDEX [IX_tblMeter_MeterID]
    ON [dbo].[tblMeter]([MeterID] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblMeter_MeterID_SiteGuid]
    ON [dbo].[tblMeter]([MeterID] ASC, [SiteGuid] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblMeter] ON [dbo].[tblMeter] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMeter','D')=1 
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
		MeterGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblMeter (
		[MeterGuid]
	,	[SiteGuid]
	,	[MeterID]
	,	[NumberOfDigits]
	,	[RotatesBackwardsFlag]
	,	[ReceiptMeterFlag]
	,	[MeterFactor]
	,	[FuelCompressionFactor]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[DcuID]
	,	[DcuBatteryVoltage]
	,	[DcuBatteryCurrent]
	,	[DcuTemperature]
	,	[DcuResets]
	,	[DcuUpdateDate]
	,	[DcuConfigurationDate]
	,	[DcuFirmwareVersion]
	,	[DcuBluetoothAddress]
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
	OUTPUT inserted.[MeterGuid] AS 'MeterGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[MeterGuid]
	,	d.[SiteGuid]
	,	d.[MeterID]
	,	d.[NumberOfDigits]
	,	d.[RotatesBackwardsFlag]
	,	d.[ReceiptMeterFlag]
	,	d.[MeterFactor]
	,	d.[FuelCompressionFactor]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[DcuID]
	,	d.[DcuBatteryVoltage]
	,	d.[DcuBatteryCurrent]
	,	d.[DcuTemperature]
	,	d.[DcuResets]
	,	d.[DcuUpdateDate]
	,	d.[DcuConfigurationDate]
	,	d.[DcuFirmwareVersion]
	,	d.[DcuBluetoothAddress]
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
 
	INSERT INTO [fmaudit].tblMeter (
		[MeterGuid]
	,	[SiteGuid]
	,	[MeterID]
	,	[NumberOfDigits]
	,	[RotatesBackwardsFlag]
	,	[ReceiptMeterFlag]
	,	[MeterFactor]
	,	[FuelCompressionFactor]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[DcuID]
	,	[DcuBatteryVoltage]
	,	[DcuBatteryCurrent]
	,	[DcuTemperature]
	,	[DcuResets]
	,	[DcuUpdateDate]
	,	[DcuConfigurationDate]
	,	[DcuFirmwareVersion]
	,	[DcuBluetoothAddress]
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
		i.[MeterGuid]
	,	i.[SiteGuid]
	,	i.[MeterID]
	,	i.[NumberOfDigits]
	,	i.[RotatesBackwardsFlag]
	,	i.[ReceiptMeterFlag]
	,	i.[MeterFactor]
	,	i.[FuelCompressionFactor]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[DcuID]
	,	i.[DcuBatteryVoltage]
	,	i.[DcuBatteryCurrent]
	,	i.[DcuTemperature]
	,	i.[DcuResets]
	,	i.[DcuUpdateDate]
	,	i.[DcuConfigurationDate]
	,	i.[DcuFirmwareVersion]
	,	i.[DcuBluetoothAddress]
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
			agl.[MeterGuid]=i.[MeterGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


--Creating Insert / Update Trigger for tblMeter
CREATE TRIGGER dbo.trg_insupd_tblMeter_ForSync 
	ON dbo.tblMeter
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
				,d.MeterGuid AS Deleted_PK_MeterGuid
				,i.MeterGuid AS Inserted_PK_MeterGuid
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
					FULL OUTER JOIN Deleted d 
						ON d.MeterGuid = i.MeterGuid
 
		UPDATE currentTrackingData SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
				,UpdatedContext = entityChanges.ChangeContext 
				,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
				,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblMeter As currentTrackingData 
				JOIN #ChangeList As entityChanges 
					ON entityChanges.Inserted_PK_MeterGuid = currentTrackingData.PK_MeterGuid
 
		INSERT track.tblMeter (InsertedDate 
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
										,PK_MeterGuid
										,FK_ParentPK 
		)
		SELECT CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
					WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
					ELSE CAST('1/1/1990' AS DateTimeOffset(7)) 
				END 
				,entityChanges.ChangeContext 
				,entityChanges.Inserted_RowVersion 
				,entityChanges.Inserted_CreatedDate 
				,entityChanges.ChangeContext 
				,entityChanges.Inserted_RowVersion 
				,NULL 
				,NULL 
				,NULL 
				,entityChanges.CurrentSiteGuid 
				,CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), entityChanges.PreviousSiteGuid),'')) THEN entityChanges.PreviousSiteGuid 
					  ELSE NULL 
				 END
				,entityChanges.Inserted_PK_MeterGuid
				,entityChanges.Inserted_FK_ParentPK
			FROM #ChangeList As entityChanges 
				WHERE NOT EXISTS ( SELECT 1 
										FROM track.tblMeter As currentTrackingData
										WHERE entityChanges.Inserted_PK_MeterGuid = currentTrackingData.PK_MeterGuid
								)
	END
END 
GO

--Creating Delete Trigger for tblMeter
CREATE TRIGGER dbo.trg_del_tblMeter_ForSync 
	ON dbo.tblMeter
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

		SELECT @syncContext AS ChangeContext 
				,d.MeterGuid AS Deleted_PK_MeterGuid
				,d.MeterGuid AS Inserted_PK_MeterGuid
				,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
				,d.CreatedDate AS Inserted_CreatedDate 
				,d.UpdatedDate AS Inserted_UpdatedDate 
				,d.SiteGuid AS CurrentSiteGuid 
				,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				,d._RowVersion AS Inserted_RowVersion 
				,0x0000000000000000 AS Deleted_RowVersion -- The real Deleted_RowVersion will be generated by a separate trigger on the tracking table itself
			INTO #ChangeList 
			FROM Deleted d 
				FULL OUTER JOIN Inserted i 
					ON d.MeterGuid = i.MeterGuid

		UPDATE currentTrackingData SET DeletedDate = @currentDateTimeOffset 
										,DeletedContext = entityChanges.ChangeContext 
										,DeletedRowVersion = entityChanges.Deleted_RowVersion 
			FROM track.tblMeter AS currentTrackingData 
				JOIN #ChangeList AS entityChanges 
					ON entityChanges.Deleted_PK_MeterGuid = currentTrackingData.PK_MeterGuid

		INSERT track.tblMeter (InsertedDate
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
										,PK_MeterGuid
										,FK_ParentPK 
			)
			SELECT CASE WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
						ELSE CAST('1/1/1990' AS DateTimeOffset(7)) 
					END 
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
					,entityChanges.Deleted_PK_MeterGuid
					,entityChanges.Deleted_FK_ParentPK
				FROM #ChangeList As entityChanges 
				WHERE NOT EXISTS ( SELECT 1 
										FROM track.tblMeter As currentTrackingData
										WHERE entityChanges.Deleted_PK_MeterGuid = currentTrackingData.PK_MeterGuid
			)
	END
END 
GO
