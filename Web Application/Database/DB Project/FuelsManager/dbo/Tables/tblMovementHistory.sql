CREATE TABLE [dbo].[tblMovementHistory]
(
	[MovementHistoryGuid]				UNIQUEIDENTIFIER CONSTRAINT [DF_tblMovementHistory_MovementHistoryGuid] DEFAULT (NEWID()) NOT NULL
	, [SiteGuid]							UNIQUEIDENTIFIER NOT NULL
	, [Name]									NVARCHAR(100) NOT NULL
	, [Node]									NVARCHAR(100) NULL
	, [InitiationCount]					BIGINT
	, [RecordType]							INT NULL
	, [TimeStamp]							DATETIME NULL -- Time as UTC
	, [ParentGuid]							UNIQUEIDENTIFIER NULL
	, [AutoStart]							BIT NULL
	, [AutoStartTime]						DATETIMEOFFSET(7) NULL
	, [AutoStop]							BIT NULL
	, [AutoStopTime]						DATETIMEOFFSET(7) NULL
	, [CloseoutDataModifiedBy]			NVARCHAR(50) NULL
	, [CloseoutDensityProductInAir]	FLOAT NULL
	, [CloseoutDensityProductObserved]		FLOAT NULL
	, [CloseoutDensityProductObservedTime]	DATETIMEOFFSET(7) NULL
	, [CloseoutDensityProductStandard]		FLOAT NULL
	, [CloseoutDensityProductStandardTime]	DATETIMEOFFSET(7) NULL
	, [CloseoutDensityProductStandardInAir]	FLOAT NULL
	, [CloseoutLevelProduct]			FLOAT NULL
	, [CloseoutLevelProductTime]		DATETIMEOFFSET(7) NULL
	, [CloseoutLevelWater]				FLOAT NULL
	, [CloseoutMassLiquid]				FLOAT NULL
	, [CloseoutPercentBsw]				FLOAT NULL
	, [CloseoutRoofMass]					FLOAT NULL
	, [CloseoutTankShellCorrection]	FLOAT NULL
	, [CloseoutTemperatureAmbient]	FLOAT NULL
	, [CloseoutTemperatureAmbientTime]		DATETIMEOFFSET(7) NULL
	, [CloseoutTemperatureDensity]	FLOAT NULL
	, [CloseoutTemperatureProduct]	FLOAT NULL
	, [CloseoutTime]						DATETIMEOFFSET(7) NULL
	, [CloseoutTransferGov]				FLOAT NULL
	, [CloseoutTransferNsv]				FLOAT NULL
	, [CloseoutTransferMassLiquid]	FLOAT NULL
	, [CloseoutTransferVolumeWater]	FLOAT NULL
	, [CloseoutVolumeBsw]				FLOAT NULL
	, [CloseoutVolumeCorrectionFactor]		FLOAT NULL
	, [CloseoutVolumeGrossObserved]	FLOAT NULL
	, [CloseoutVolumeGrossStandard]	FLOAT NULL
	, [CloseoutVolumeNetStandard]		FLOAT NULL
	, [CloseoutVolumeRoofCorrection]	FLOAT NULL
	, [CloseoutVolumeTotalObserved]	FLOAT NULL
	, [CloseoutVolumeWater]				FLOAT NULL
	, [Comment]								NVARCHAR(1000) NULL
	, [Type]									NVARCHAR(20) NULL
	, [OrderNumber]						NVARCHAR(100) NULL
	, [PlannedStartTime]					DATETIMEOFFSET(7) NULL
	, [Product]								NVARCHAR(100) NULL
	, [ProductDescription]				NVARCHAR(1000) NULL
	, [StartTime]							DATETIMEOFFSET(7) NULL
	, [StopTime]							DATETIMEOFFSET(7) NULL
	, [StartDensityProductObserved]			FLOAT NULL
	, [StartDensityProductObservedTime]		DATETIMEOFFSET(7) NULL
	, [StartDensityProductObservedInAir]	FLOAT NULL
	, [StartDensityProductStandard]			FLOAT NULL
	, [StartDensityProductStandardTime]		DATETIMEOFFSET(7) NULL
	, [StartUserID]						NVARCHAR(100) NULL
	, [StartLevelProduct]				FLOAT NULL
	, [StartLevelProductTime]			DATETIMEOFFSET(7) NULL
	, [StartLevelWater]					FLOAT NULL
	, [StartLevelWaterTime]				DATETIMEOFFSET(7) NULL
	, [StartMassLiquid]					FLOAT NULL
	, [StartPercentBsw]					FLOAT NULL
	, [StartTankShellCorrection]		FLOAT NULL
	, [StartTemperatureAmbient]		FLOAT NULL
	, [StartTemperatureAmbientTime]	DATETIMEOFFSET(7) NULL
	, [StartTemperatureProduct]		FLOAT NULL
	, [StartTemperatureProductTime]	DATETIMEOFFSET(7) NULL
	, [StartTemperatureDensity]		FLOAT NULL
	, [StartTemperatureDensityTime]	DATETIMEOFFSET(7) NULL
	, [StartVolume]						FLOAT NULL
	, [StartVolumeBsw]							FLOAT NULL
	, [StartVolumeCorrectionFactor]	FLOAT NULL
	, [StartVolumeGrossObserved]		FLOAT NULL
	, [StartVolumeGrossStandard]		FLOAT NULL
	, [StartVolumeNetStandard]			FLOAT NULL
	, [StartVolumeRoofCorrection]		FLOAT NULL
	, [StartVolumeTotalObserved]		FLOAT NULL
	, [StartVolumeWater]					FLOAT NULL
	, [UnitsLevelProductIndex]			INT NULL
	, [UnitsTemperatureAmbientIndex]	INT NULL
	, [UnitsTemperatureDensityIndex]	INT NULL
	, [UnitsTemperatureProductIndex]	INT NULL
	, [UnitsDensityProductObservedIndex]	INT NULL
	, [UnitsDensityProductStandardIndex]	INT NULL
	, [UnitsVolumeIndex]					INT NULL
	, [UnitsMassIndex]					INT NULL
	, [DecimalPlacesVolume]				INT NULL
	, [DecimalPlacesLevel]				INT NULL
	, [DecimalPlacesDensity]			INT NULL
	, [DecimalPlacesTemperature]		INT NULL
	, [UserData01]							NVARCHAR(100) NULL
	, [UserData02]							NVARCHAR(100) NULL
	, [UserData03]							NVARCHAR(100) NULL
	, [UserData04]							NVARCHAR(100) NULL
	, [UserData05]							NVARCHAR(100) NULL
	, [UserData06]							NVARCHAR(100) NULL
	, [UserData07]							NVARCHAR(100) NULL
	, [UserData08]							NVARCHAR(100) NULL
	, [UserData09]							NVARCHAR(100) NULL
	, [UserData10]							NVARCHAR(100) NULL
	, [TransferDeviation]				FLOAT NULL
	, [TransferPercentDeviation]		FLOAT NULL
	, [DecimalPlacesPercent]			INT NULL
	, [TransferMode]						INT NULL
	, [TransferStatus]					INT NULL
	, [TransferTarget]					FLOAT NULL
	, [TransferTargetUnitsIndex]		INT NULL
	, [TransferLevelTarget]				FLOAT NULL
	, [TransferVolumeTarget]			FLOAT NULL
	, [TransferTimeRemaining]			BIGINT NULL
	, [TransferDirection]				NVARCHAR(20) NULL
	, [CommentDateTime]					DATETIME NULL -- Time as UTC
	, [CommentUserID]						NVARCHAR(50) NULL
	, [Status]								INT NULL
	, [VolumeWater]						FLOAT NULL
	, [LevelProduct]						FLOAT NULL
	, [StartDensityProductStandardInAir]	FLOAT NULL
	, [TransferredVolumeWater]			FLOAT NULL
	, [TransferredVolume]				FLOAT NULL
	, [MidnightRecord]					BIT NULL
	, [PointGuid]							UNIQUEIDENTIFIER NULL
	, [RootParentGuid]					UNIQUEIDENTIFIER NULL
	, [RecordSeq]							INT NULL
	, [CreatedDate]						DATETIMEOFFSET (7) CONSTRAINT [DF_tblMovementHistory_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NULL
    , [CreatedBy]							[dbo].[udtUserID]  CONSTRAINT [DF_tblMovementHistory_CreatedBy] DEFAULT ('') NOT NULL
    , [UpdatedDate]						DATETIMEOFFSET (7) CONSTRAINT [DF_tblMovementHistory_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL
    , [UpdatedBy]							[dbo].[udtUserID]  CONSTRAINT [DF_tblMovementHistory_UpdatedBy] DEFAULT ('') NOT NULL
	, [_RowVersion]						ROWVERSION NOT NULL
	, [_ClusterIdx]						BIGINT IDENTITY (1, 1) NOT NULL
	, CONSTRAINT [PK_tblMovementHistory_GUID] PRIMARY KEY NONCLUSTERED ([MovementHistoryGuid] ASC)
	, CONSTRAINT [FK_tblMovementHistory_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
)
GO

CREATE NONCLUSTERED INDEX [IX_tblMovementHistory_CreatedDate]
    ON [dbo].[tblMovementHistory]([CreatedDate] ASC);
GO

CREATE NONCLUSTERED INDEX [IX_tblMovementHistory_SiteGuid_Name]
    ON [dbo].[tblMovementHistory]([SiteGuid] ASC, [Name] ASC);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblMovementHistory_MovementHistoryGuid_SiteGuid]
    ON [dbo].[tblMovementHistory]([MovementHistoryGuid] ASC, [SiteGuid] ASC);
GO

CREATE TRIGGER [dbo].[trg_Audit_upd_tblMovementHistory] ON [dbo].[tblMovementHistory] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMovementHistory','D') = 1 
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
	SET @_AuditEventType = 'U' -- For Updates 
	SET @_AuditEventSequence = 1 
	SELECT	@_AuditSessionGUID = s.SessionGuid 
		,	@_AuditSessionTokenID = s.SessionTokenID 
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid = s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid 
	WHERE m.SqlServerSessionID = @@SPID 

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
		MovementHistoryGuid	UNIQUEIDENTIFIER NULL
		, _AuditEventType CHAR(1)
		, _AuditEventSequence TINYINT
		, _AuditCreatedDate DATETIMEOFFSET
		, _AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblMovementHistory 
	(
		[MovementHistoryGuid]
		, [SiteGuid]
		, [Name]
		, [Node]
		, [InitiationCount]
		, [RecordType]
		, [TimeStamp]
		, [ParentGuid]
		, [AutoStart]
		, [AutoStartTime]
		, [AutoStop]
		, [AutoStopTime]
		, [CloseoutDataModifiedBy]
		, [CloseoutDensityProductInAir]
		, [CloseoutDensityProductObserved]
		, [CloseoutDensityProductObservedTime]
		, [CloseoutDensityProductStandard]
		, [CloseoutDensityProductStandardTime]
		, [CloseoutDensityProductStandardInAir]
		, [CloseoutLevelProduct]
		, [CloseoutLevelProductTime]
		, [CloseoutLevelWater]
		, [CloseoutMassLiquid]
		, [CloseoutPercentBsw]
		, [CloseoutRoofMass]
		, [CloseoutTankShellCorrection]
		, [CloseoutTemperatureAmbient]
		, [CloseoutTemperatureAmbientTime]
		, [CloseoutTemperatureDensity]
		, [CloseoutTemperatureProduct]
		, [CloseoutTime]
		, [CloseoutTransferGov]
		, [CloseoutTransferNsv]
		, [CloseoutTransferMassLiquid]
		, [CloseoutTransferVolumeWater]
		, [CloseoutVolumeBsw]
		, [CloseoutVolumeCorrectionFactor]
		, [CloseoutVolumeGrossObserved]
		, [CloseoutVolumeGrossStandard]
		, [CloseoutVolumeNetStandard]
		, [CloseoutVolumeRoofCorrection]
		, [CloseoutVolumeTotalObserved]
		, [CloseoutVolumeWater]
		, [Comment]
		, [Type]
		, [OrderNumber]
		, [PlannedStartTime]
		, [Product]
		, [ProductDescription]
		, [StartTime]
		, [StopTime]
		, [StartDensityProductObserved]
		, [StartDensityProductObservedTime]
		, [StartDensityProductObservedInAir]
		, [StartDensityProductStandard]
		, [StartDensityProductStandardTime]
		, [StartUserID]
		, [StartLevelProduct]
		, [StartLevelProductTime]
		, [StartLevelWater]
		, [StartLevelWaterTime]
		, [StartPercentBsw]
		, [StartMassLiquid]
		, [StartTankShellCorrection]
		, [StartTemperatureAmbient]
		, [StartTemperatureAmbientTime]
		, [StartTemperatureProduct]
		, [StartTemperatureProductTime]
		, [StartTemperatureDensity]
		, [StartTemperatureDensityTime]
		, [StartVolume]
		, [StartVolumeBsw]
		, [StartVolumeCorrectionFactor]
		, [StartVolumeGrossObserved]
		, [StartVolumeGrossStandard]
		, [StartVolumeNetStandard]
		, [StartVolumeRoofCorrection]
		, [StartVolumeTotalObserved]
		, [StartVolumeWater]
		, [UnitsLevelProductIndex]
		, [UnitsTemperatureAmbientIndex]
		, [UnitsTemperatureDensityIndex]
		, [UnitsTemperatureProductIndex]
		, [UnitsDensityProductObservedIndex]
		, [UnitsDensityProductStandardIndex]
		, [UnitsVolumeIndex]
		, [UnitsMassIndex]
		, [DecimalPlacesVolume]
		, [DecimalPlacesLevel]
		, [DecimalPlacesDensity]
		, [DecimalPlacesTemperature]
		, [UserData01]
		, [UserData02]
		, [UserData03]
		, [UserData04]
		, [UserData05]
		, [UserData06]
		, [UserData07]
		, [UserData08]
		, [UserData09]
		, [UserData10]
		, [TransferDeviation]
		, [TransferPercentDeviation]
		, [DecimalPlacesPercent]
		, [TransferMode]
		, [TransferStatus]
		, [TransferTarget]
		, [TransferTargetUnitsIndex]
		, [TransferLevelTarget]
		, [TransferVolumeTarget]
		, [TransferTimeRemaining]
		, [TransferDirection]
		, [CommentDateTime]
		, [CommentUserID]
		, [Status]
		, [VolumeWater]
		, [LevelProduct]
		, [StartDensityProductStandardInAir]
		, [TransferredVolumeWater]
		, [TransferredVolume]
		, [MidnightRecord]
		, [PointGuid]
		, [RootParentGuid]
		, [RecordSeq]
		, [_AuditEventType]
		, [_AuditEventSequence]
		, [_AuditSessionGUID]
		, [_AuditSessionTokenID]
		, [_AuditCreatedDate]
		, [_AuditSiteGUID]
		, [_AuditGUID]
		, [_AuditUserId]
		, [_AuditContext]
	)
	OUTPUT inserted.[MovementHistoryGuid] AS 'MovementHistoryGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[MovementHistoryGuid]
		, d.[SiteGuid]
		, d.[Name]
		, d.[Node]
		, d.[InitiationCount]
		, d.[RecordType]
		, d.[TimeStamp]
		, d.[ParentGuid]
		, d.[AutoStart]
		, d.[AutoStartTime]
		, d.[AutoStop]
		, d.[AutoStopTime]
		, d.[CloseoutDataModifiedBy]
		, d.[CloseoutDensityProductInAir]
		, d.[CloseoutDensityProductObserved]
		, d.[CloseoutDensityProductObservedTime]
		, d.[CloseoutDensityProductStandard]
		, d.[CloseoutDensityProductStandardTime]
		, d.[CloseoutDensityProductStandardInAir]
		, d.[CloseoutLevelProduct]
		, d.[CloseoutLevelProductTime]
		, d.[CloseoutLevelWater]
		, d.[CloseoutMassLiquid]
		, d.[CloseoutPercentBsw]
		, d.[CloseoutRoofMass]
		, d.[CloseoutTankShellCorrection]
		, d.[CloseoutTemperatureAmbient]
		, d.[CloseoutTemperatureAmbientTime]
		, d.[CloseoutTemperatureDensity]
		, d.[CloseoutTemperatureProduct]
		, d.[CloseoutTime]
		, d.[CloseoutTransferGov]
		, d.[CloseoutTransferNsv]
		, d.[CloseoutTransferMassLiquid]
		, d.[CloseoutTransferVolumeWater]
		, d.[CloseoutVolumeBsw]
		, d.[CloseoutVolumeCorrectionFactor]
		, d.[CloseoutVolumeGrossObserved]
		, d.[CloseoutVolumeGrossStandard]
		, d.[CloseoutVolumeNetStandard]
		, d.[CloseoutVolumeRoofCorrection]
		, d.[CloseoutVolumeTotalObserved]
		, d.[CloseoutVolumeWater]
		, d.[Comment]
		, d.[Type]
		, d.[OrderNumber]
		, d.[PlannedStartTime]
		, d.[Product]
		, d.[ProductDescription]
		, d.[StartTime]
		, d.[StopTime]
		, d.[StartDensityProductObserved]
		, d.[StartDensityProductObservedTime]
		, d.[StartDensityProductObservedInAir]
		, d.[StartDensityProductStandard]
		, d.[StartDensityProductStandardTime]
		, d.[StartUserID]
		, d.[StartLevelProduct]
		, d.[StartLevelProductTime]
		, d.[StartLevelWater]
		, d.[StartLevelWaterTime]
		, d.[StartPercentBsw]
		, d.[StartMassLiquid]
		, d.[StartTankShellCorrection]
		, d.[StartTemperatureAmbient]
		, d.[StartTemperatureAmbientTime]
		, d.[StartTemperatureProduct]
		, d.[StartTemperatureProductTime]
		, d.[StartTemperatureDensity]
		, d.[StartTemperatureDensityTime]
		, d.[StartVolume]
		, d.[StartVolumeBsw]
		, d.[StartVolumeCorrectionFactor]
		, d.[StartVolumeGrossObserved]
		, d.[StartVolumeGrossStandard]
		, d.[StartVolumeNetStandard]
		, d.[StartVolumeRoofCorrection]
		, d.[StartVolumeTotalObserved]
		, d.[StartVolumeWater]
		, d.[UnitsLevelProductIndex]
		, d.[UnitsTemperatureAmbientIndex]
		, d.[UnitsTemperatureDensityIndex]
		, d.[UnitsTemperatureProductIndex]
		, d.[UnitsDensityProductObservedIndex]
		, d.[UnitsDensityProductStandardIndex]
		, d.[UnitsVolumeIndex]
		, d.[UnitsMassIndex]
		, d.[DecimalPlacesVolume]
		, d.[DecimalPlacesLevel]
		, d.[DecimalPlacesDensity]
		, d.[DecimalPlacesTemperature]
		, d.[UserData01]
		, d.[UserData02]
		, d.[UserData03]
		, d.[UserData04]
		, d.[UserData05]
		, d.[UserData06]
		, d.[UserData07]
		, d.[UserData08]
		, d.[UserData09]
		, d.[UserData10]
		, d.[TransferDeviation]
		, d.[TransferPercentDeviation]
		, d.[DecimalPlacesPercent]
		, d.[TransferMode]
		, d.[TransferStatus]
		, d.[TransferTarget]
		, d.[TransferTargetUnitsIndex]
		, d.[TransferLevelTarget]
		, d.[TransferVolumeTarget]
		, d.[TransferTimeRemaining]
		, d.[TransferDirection]
		, d.[CommentDateTime]
		, d.[CommentUserID]
		, d.[Status]
		, d.[VolumeWater]
		, d.[LevelProduct]
		, d.[StartDensityProductStandardInAir]
		, d.[TransferredVolumeWater]
		, d.[TransferredVolume]
		, d.[MidnightRecord]
		, d.[PointGuid]
		, d.[RootParentGuid]
		, d.[RecordSeq]
		, @_AuditEventType
		, @_AuditEventSequence
		, @_AuditSessionGUID
		, @_AuditSessionTokenID
		, @_AuditDateTime
		, @_AuditSiteGUID
		, NEWID()
		, @_UserId
		, @_AuditContext
	FROM deleted d
 
	INSERT INTO [fmaudit].tblMovementHistory 
	(
		[MovementHistoryGuid]
		, [SiteGuid]
		, [Name]
		, [Node]
		, [InitiationCount]
		, [RecordType]
		, [TimeStamp]
		, [ParentGuid]
		, [AutoStart]
		, [AutoStartTime]
		, [AutoStop]
		, [AutoStopTime]
		, [CloseoutDataModifiedBy]
		, [CloseoutDensityProductInAir]
		, [CloseoutDensityProductObserved]
		, [CloseoutDensityProductObservedTime]
		, [CloseoutDensityProductStandard]
		, [CloseoutDensityProductStandardTime]
		, [CloseoutDensityProductStandardInAir]
		, [CloseoutLevelProduct]
		, [CloseoutLevelProductTime]
		, [CloseoutLevelWater]
		, [CloseoutMassLiquid]
		, [CloseoutPercentBsw]
		, [CloseoutRoofMass]
		, [CloseoutTankShellCorrection]
		, [CloseoutTemperatureAmbient]
		, [CloseoutTemperatureAmbientTime]
		, [CloseoutTemperatureDensity]
		, [CloseoutTemperatureProduct]
		, [CloseoutTime]
		, [CloseoutTransferGov]
		, [CloseoutTransferNsv]
		, [CloseoutTransferMassLiquid]
		, [CloseoutTransferVolumeWater]
		, [CloseoutVolumeBsw]
		, [CloseoutVolumeCorrectionFactor]
		, [CloseoutVolumeGrossObserved]
		, [CloseoutVolumeGrossStandard]
		, [CloseoutVolumeNetStandard]
		, [CloseoutVolumeRoofCorrection]
		, [CloseoutVolumeTotalObserved]
		, [CloseoutVolumeWater]
		, [Comment]
		, [Type]
		, [OrderNumber]
		, [PlannedStartTime]
		, [Product]
		, [ProductDescription]
		, [StartTime]
		, [StopTime]
		, [StartDensityProductObserved]
		, [StartDensityProductObservedTime]
		, [StartDensityProductObservedInAir]
		, [StartDensityProductStandard]
		, [StartDensityProductStandardTime]
		, [StartUserID]
		, [StartLevelProduct]
		, [StartLevelProductTime]
		, [StartLevelWater]
		, [StartLevelWaterTime]
		, [StartPercentBsw]
		, [StartMassLiquid]
		, [StartTankShellCorrection]
		, [StartTemperatureAmbient]
		, [StartTemperatureAmbientTime]
		, [StartTemperatureProduct]
		, [StartTemperatureProductTime]
		, [StartTemperatureDensity]
		, [StartTemperatureDensityTime]
		, [StartVolume]
		, [StartVolumeBsw]
		, [StartVolumeCorrectionFactor]
		, [StartVolumeGrossObserved]
		, [StartVolumeGrossStandard]
		, [StartVolumeNetStandard]
		, [StartVolumeRoofCorrection]
		, [StartVolumeTotalObserved]
		, [StartVolumeWater]
		, [UnitsLevelProductIndex]
		, [UnitsTemperatureAmbientIndex]
		, [UnitsTemperatureDensityIndex]
		, [UnitsTemperatureProductIndex]
		, [UnitsDensityProductObservedIndex]
		, [UnitsDensityProductStandardIndex]
		, [UnitsVolumeIndex]
		, [UnitsMassIndex]
		, [DecimalPlacesVolume]
		, [DecimalPlacesLevel]
		, [DecimalPlacesDensity]
		, [DecimalPlacesTemperature]
		, [UserData01]
		, [UserData02]
		, [UserData03]
		, [UserData04]
		, [UserData05]
		, [UserData06]
		, [UserData07]
		, [UserData08]
		, [UserData09]
		, [UserData10]
		, [TransferDeviation]
		, [TransferPercentDeviation]
		, [DecimalPlacesPercent]
		, [TransferMode]
		, [TransferStatus]
		, [TransferTarget]
		, [TransferTargetUnitsIndex]
		, [TransferLevelTarget]
		, [TransferVolumeTarget]
		, [TransferTimeRemaining]
		, [TransferDirection]
		, [CommentDateTime]
		, [CommentUserID]
		, [Status]
		, [VolumeWater]
		, [LevelProduct]
		, [StartDensityProductStandardInAir]
		, [TransferredVolumeWater]
		, [TransferredVolume]
		, [MidnightRecord]
		, [PointGuid]
		, [RootParentGuid]
		, [RecordSeq]
		, [_AuditEventType]
		, [_AuditEventSequence]
		, [_AuditSessionGUID]
		, [_AuditSessionTokenID]
		, [_AuditCreatedDate]
		, [_AuditSiteGUID]
		, [_AuditGUID]
		, [_AuditUserId]
		, [_AuditContext]
	)
	SELECT 
		i.[MovementHistoryGuid]
		, i.[SiteGuid]
		, i.[Name]
		, i.[Node]
		, i.[InitiationCount]
		, i.[RecordType]
		, i.[TimeStamp]
		, i.[ParentGuid]
		, i.[AutoStart]
		, i.[AutoStartTime]
		, i.[AutoStop]
		, i.[AutoStopTime]
		, i.[CloseoutDataModifiedBy]
		, i.[CloseoutDensityProductInAir]
		, i.[CloseoutDensityProductObserved]
		, i.[CloseoutDensityProductObservedTime]
		, i.[CloseoutDensityProductStandard]
		, i.[CloseoutDensityProductStandardTime]
		, i.[CloseoutDensityProductStandardInAir]
		, i.[CloseoutLevelProduct]
		, i.[CloseoutLevelProductTime]
		, i.[CloseoutLevelWater]
		, i.[CloseoutMassLiquid]
		, i.[CloseoutPercentBsw]
		, i.[CloseoutRoofMass]
		, i.[CloseoutTankShellCorrection]
		, i.[CloseoutTemperatureAmbient]
		, i.[CloseoutTemperatureAmbientTime]
		, i.[CloseoutTemperatureDensity]
		, i.[CloseoutTemperatureProduct]
		, i.[CloseoutTime]
		, i.[CloseoutTransferGov]
		, i.[CloseoutTransferNsv]
		, i.[CloseoutTransferMassLiquid]
		, i.[CloseoutTransferVolumeWater]
		, i.[CloseoutVolumeBsw]
		, i.[CloseoutVolumeCorrectionFactor]
		, i.[CloseoutVolumeGrossObserved]
		, i.[CloseoutVolumeGrossStandard]
		, i.[CloseoutVolumeNetStandard]
		, i.[CloseoutVolumeRoofCorrection]
		, i.[CloseoutVolumeTotalObserved]
		, i.[CloseoutVolumeWater]
		, i.[Comment]
		, i.[Type]
		, i.[OrderNumber]
		, i.[PlannedStartTime]
		, i.[Product]
		, i.[ProductDescription]
		, i.[StartTime]
		, i.[StopTime]
		, i.[StartDensityProductObserved]
		, i.[StartDensityProductObservedTime]
		, i.[StartDensityProductObservedInAir]
		, i.[StartDensityProductStandard]
		, i.[StartDensityProductStandardTime]
		, i.[StartUserID]
		, i.[StartLevelProduct]
		, i.[StartLevelProductTime]
		, i.[StartLevelWater]
		, i.[StartLevelWaterTime]
		, i.[StartPercentBsw]
		, i.[StartMassLiquid]
		, i.[StartTankShellCorrection]
		, i.[StartTemperatureAmbient]
		, i.[StartTemperatureAmbientTime]
		, i.[StartTemperatureProduct]
		, i.[StartTemperatureProductTime]
		, i.[StartTemperatureDensity]
		, i.[StartTemperatureDensityTime]
		, i.[StartVolume]
		, i.[StartVolumeBsw]
		, i.[StartVolumeCorrectionFactor]
		, i.[StartVolumeGrossObserved]
		, i.[StartVolumeGrossStandard]
		, i.[StartVolumeNetStandard]
		, i.[StartVolumeRoofCorrection]
		, i.[StartVolumeTotalObserved]
		, i.[StartVolumeWater]
		, i.[UnitsLevelProductIndex]
		, i.[UnitsTemperatureAmbientIndex]
		, i.[UnitsTemperatureDensityIndex]
		, i.[UnitsTemperatureProductIndex]
		, i.[UnitsDensityProductObservedIndex]
		, i.[UnitsDensityProductStandardIndex]
		, i.[UnitsVolumeIndex]
		, i.[UnitsMassIndex]
		, i.[DecimalPlacesVolume]
		, i.[DecimalPlacesLevel]
		, i.[DecimalPlacesDensity]
		, i.[DecimalPlacesTemperature]
		, i.[UserData01]
		, i.[UserData02]
		, i.[UserData03]
		, i.[UserData04]
		, i.[UserData05]
		, i.[UserData06]
		, i.[UserData07]
		, i.[UserData08]
		, i.[UserData09]
		, i.[UserData10]
		, i.[TransferDeviation]
		, i.[TransferPercentDeviation]
		, i.[DecimalPlacesPercent]
		, i.[TransferMode]
		, i.[TransferStatus]
		, i.[TransferTarget]
		, i.[TransferTargetUnitsIndex]
		, i.[TransferLevelTarget]
		, i.[TransferVolumeTarget]
		, i.[TransferTimeRemaining]
		, i.[TransferDirection]
		, i.[CommentDateTime]
		, i.[CommentUserID]
		, i.[Status]
		, i.[VolumeWater]
		, i.[LevelProduct]
		, i.[StartDensityProductStandardInAir]
		, i.[TransferredVolumeWater]
		, i.[TransferredVolume]
		, i.[MidnightRecord]
		, i.[PointGuid]
		, i.[RootParentGuid]
		, i.[RecordSeq]
		, @_AuditEventType
		, 2
		, @_AuditSessionGUID
		, @_AuditSessionTokenID
		, @_AuditDateTime
		, @_AuditSiteGUID
		, agl._AuditGUID
		, @_UserId
		, @_AuditContext
	FROM inserted i 
	INNER JOIN	@AuditGuidList agl ON
		(
			agl.[MovementHistoryGuid] = i.[MovementHistoryGuid] 
		)
		WHERE	agl._AuditEventType = 'U'
		AND		agl._AuditEventSequence = 1 
		AND		agl._AuditCreatedDate = @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblMovementHistory
CREATE TRIGGER dbo.trg_insupd_tblMovementHistory_ForSync 
   ON dbo.tblMovementHistory
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
                    , d.MovementHistoryGuid AS Deleted_PK_MovementHistoryGuid
                    , i.MovementHistoryGuid AS Inserted_PK_MovementHistoryGuid
                    , CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    , CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    , i.CreatedDate AS Inserted_CreatedDate 
                    , i.UpdatedDate AS Inserted_UpdatedDate 
                    , i.SiteGuid AS CurrentSiteGuid 
                    , d.SiteGuid AS PreviousSiteGuid 
				    , i._RowVersion AS Inserted_RowVersion 
				    , CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.MovementHistoryGuid = i.MovementHistoryGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		, UpdatedContext = entityChanges.ChangeContext 
 				        , UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					, CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				, PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblMovementHistory As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_MovementHistoryGuid = currentTrackingData.PK_MovementHistoryGuid
 
		    INSERT track.tblMovementHistory 
			(
				InsertedDate 
 			    , InsertedContext 
 				, InsertedRowVersion 
 				, UpdatedDate 
 				, UpdatedContext 
 				, UpdatedRowVersion 
 				, DeletedDate 
 				, DeletedContext 
 				, DeletedRowVersion 
 				, CurrentSiteGuid 
 				, PreviousSiteGuid 
				, PK_MovementHistoryGuid
				, FK_ParentPK 
		    )
		    SELECT CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
		                 WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate 
		                 ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
			    	, entityChanges.ChangeContext 
				    , entityChanges.Inserted_RowVersion 
    				, entityChanges.Inserted_CreatedDate 
	    			, entityChanges.ChangeContext 
		    		, entityChanges.Inserted_RowVersion 
			    	, NULL 
    				, NULL 
	    			, NULL 
		    		, entityChanges.CurrentSiteGuid 
			    	, CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), entityChanges.PreviousSiteGuid),'')) THEN entityChanges.PreviousSiteGuid ELSE NULL END
				    , entityChanges.Inserted_PK_MovementHistoryGuid
				    , entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblMovementHistory As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_MovementHistoryGuid = currentTrackingData.PK_MovementHistoryGuid
)
    END
END 

GO
--Creating Delete Trigger for tblMovementHistory
CREATE TRIGGER dbo.trg_del_tblMovementHistory_ForSync 
   ON dbo.tblMovementHistory
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
            , @bypassTrackingFlags = BypassTrackingFlags 
            , @bypassReason = BypassReason 
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
						, d.MovementHistoryGuid AS Deleted_PK_MovementHistoryGuid
                        , d.MovementHistoryGuid AS Inserted_PK_MovementHistoryGuid
                        , NULL AS Deleted_FK_ParentPK 
						, d.CreatedDate AS Inserted_CreatedDate 
						, d.UpdatedDate AS Inserted_UpdatedDate 
						, d.SiteGuid AS CurrentSiteGuid 
						, NULL AS PreviousSiteGuid 
						, d._RowVersion AS Inserted_RowVersion 
						, CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblMovementHistory As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_MovementHistoryGuid = currentTrackingData.PK_MovementHistoryGuid
				WHEN Matched 
				THEN 
					UPDATE SET DeletedDate = @currentDateTimeOffset 
								, DeletedContext = entityChanges.ChangeContext 
								, DeletedRowVersion = entityChanges.Deleted_RowVersion 
				WHEN Not Matched 
				THEN 
				INSERT 
				(
					InsertedDate
				    , InsertedContext
				    , InsertedRowVersion
				    , UpdatedDate
				    , UpdatedContext
				    , UpdatedRowVersion
				    , CurrentSiteGuid
				    , PreviousSiteGuid
				    , DeletedDate
				    , DeletedContext
				    , DeletedRowVersion
					, PK_MovementHistoryGuid
				    , FK_ParentPK 
				)
				VALUES (CASE WHEN (entityChanges.Inserted_CreatedDate IS NOT NULL) THEN entityChanges.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
						, entityChanges.ChangeContext 
						, entityChanges.Inserted_RowVersion 
						, NULL 
						, NULL 
						, NULL 
						, entityChanges.CurrentSiteGuid 
						, NULL 
						, @currentDateTimeOffset 
						, entityChanges.ChangeContext 
						, entityChanges.Deleted_RowVersion
						, entityChanges.Deleted_PK_MovementHistoryGuid
				        , entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblMovementHistory] ON [dbo].[tblMovementHistory] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMovementHistory','D')=1 
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
	SET @_AuditEventType = 'D'; -- For Deletes 
	SET @_AuditEventSequence = 1; 
	SELECT	@_AuditSessionGUID = s.SessionGuid 
		,	@_AuditSessionTokenID = s.SessionTokenID 
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid = s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid 
	WHERE m.SqlServerSessionID = @@SPID;

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
	INSERT INTO [fmaudit].tblMovementHistory 
	(
		[MovementHistoryGuid]
		, [SiteGuid]
		, [Name]
		, [Node]
		, [InitiationCount]
		, [RecordType]
		, [TimeStamp]
		, [ParentGuid]
		, [AutoStart]
		, [AutoStartTime]
		, [AutoStop]
		, [AutoStopTime]
		, [CloseoutDataModifiedBy]
		, [CloseoutDensityProductInAir]
		, [CloseoutDensityProductObserved]
		, [CloseoutDensityProductObservedTime]
		, [CloseoutDensityProductStandard]
		, [CloseoutDensityProductStandardTime]
		, [CloseoutDensityProductStandardInAir]
		, [CloseoutLevelProduct]
		, [CloseoutLevelProductTime]
		, [CloseoutLevelWater]
		, [CloseoutMassLiquid]
		, [CloseoutPercentBsw]
		, [CloseoutRoofMass]
		, [CloseoutTankShellCorrection]
		, [CloseoutTemperatureAmbient]
		, [CloseoutTemperatureAmbientTime]
		, [CloseoutTemperatureDensity]
		, [CloseoutTemperatureProduct]
		, [CloseoutTime]
		, [CloseoutTransferGov]
		, [CloseoutTransferNsv]
		, [CloseoutTransferMassLiquid]
		, [CloseoutTransferVolumeWater]
		, [CloseoutVolumeBsw]
		, [CloseoutVolumeCorrectionFactor]
		, [CloseoutVolumeGrossObserved]
		, [CloseoutVolumeGrossStandard]
		, [CloseoutVolumeNetStandard]
		, [CloseoutVolumeRoofCorrection]
		, [CloseoutVolumeTotalObserved]
		, [CloseoutVolumeWater]
		, [Comment]
		, [Type]
		, [OrderNumber]
		, [PlannedStartTime]
		, [Product]
		, [ProductDescription]
		, [StartTime]
		, [StopTime]
		, [StartDensityProductObserved]
		, [StartDensityProductObservedTime]
		, [StartDensityProductObservedInAir]
		, [StartDensityProductStandard]
		, [StartDensityProductStandardTime]
		, [StartUserID]
		, [StartLevelProduct]
		, [StartLevelProductTime]
		, [StartLevelWater]
		, [StartLevelWaterTime]
		, [StartPercentBsw]
		, [StartMassLiquid]
		, [StartTankShellCorrection]
		, [StartTemperatureAmbient]
		, [StartTemperatureAmbientTime]
		, [StartTemperatureProduct]
		, [StartTemperatureProductTime]
		, [StartTemperatureDensity]
		, [StartTemperatureDensityTime]
		, [StartVolume]
		, [StartVolumeBsw]
		, [StartVolumeCorrectionFactor]
		, [StartVolumeGrossObserved]
		, [StartVolumeGrossStandard]
		, [StartVolumeNetStandard]
		, [StartVolumeRoofCorrection]
		, [StartVolumeTotalObserved]
		, [StartVolumeWater]
		, [UnitsLevelProductIndex]
		, [UnitsTemperatureAmbientIndex]
		, [UnitsTemperatureDensityIndex]
		, [UnitsTemperatureProductIndex]
		, [UnitsDensityProductObservedIndex]
		, [UnitsDensityProductStandardIndex]
		, [UnitsVolumeIndex]
		, [UnitsMassIndex]
		, [DecimalPlacesVolume]
		, [DecimalPlacesLevel]
		, [DecimalPlacesDensity]
		, [DecimalPlacesTemperature]
		, [UserData01]
		, [UserData02]
		, [UserData03]
		, [UserData04]
		, [UserData05]
		, [UserData06]
		, [UserData07]
		, [UserData08]
		, [UserData09]
		, [UserData10]
		, [TransferDeviation]
		, [TransferPercentDeviation]
		, [DecimalPlacesPercent]
		, [TransferMode]
		, [TransferStatus]
		, [TransferTarget]
		, [TransferTargetUnitsIndex]
		, [TransferLevelTarget]
		, [TransferVolumeTarget]
		, [TransferTimeRemaining]
		, [TransferDirection]
		, [CommentDateTime]
		, [CommentUserID]
		, [Status]
		, [VolumeWater]
		, [LevelProduct]
		, [StartDensityProductStandardInAir]
		, [TransferredVolumeWater]
		, [TransferredVolume]
		, [MidnightRecord]
		, [PointGuid]
		, [RootParentGuid]
		, [RecordSeq]
		, [_AuditEventType]
		, [_AuditEventSequence]
		, [_AuditSessionGUID]
		, [_AuditSessionTokenID]
		, [_AuditCreatedDate]
		, [_AuditSiteGUID]
		, [_AuditGUID]
		, [_AuditUserId]
		, [_AuditContext]
	)
	SELECT 
		d.[MovementHistoryGuid]
		, d.[SiteGuid]
		, d.[Name]
		, d.[Node]
		, d.[InitiationCount]
		, d.[RecordType]
		, d.[TimeStamp]
		, d.[ParentGuid]
		, d.[AutoStart]
		, d.[AutoStartTime]
		, d.[AutoStop]
		, d.[AutoStopTime]
		, d.[CloseoutDataModifiedBy]
		, d.[CloseoutDensityProductInAir]
		, d.[CloseoutDensityProductObserved]
		, d.[CloseoutDensityProductObservedTime]
		, d.[CloseoutDensityProductStandard]
		, d.[CloseoutDensityProductStandardTime]
		, d.[CloseoutDensityProductStandardInAir]
		, d.[CloseoutLevelProduct]
		, d.[CloseoutLevelProductTime]
		, d.[CloseoutLevelWater]
		, d.[CloseoutMassLiquid]
		, d.[CloseoutPercentBsw]
		, d.[CloseoutRoofMass]
		, d.[CloseoutTankShellCorrection]
		, d.[CloseoutTemperatureAmbient]
		, d.[CloseoutTemperatureAmbientTime]
		, d.[CloseoutTemperatureDensity]
		, d.[CloseoutTemperatureProduct]
		, d.[CloseoutTime]
		, d.[CloseoutTransferGov]
		, d.[CloseoutTransferNsv]
		, d.[CloseoutTransferMassLiquid]
		, d.[CloseoutTransferVolumeWater]
		, d.[CloseoutVolumeBsw]
		, d.[CloseoutVolumeCorrectionFactor]
		, d.[CloseoutVolumeGrossObserved]
		, d.[CloseoutVolumeGrossStandard]
		, d.[CloseoutVolumeNetStandard]
		, d.[CloseoutVolumeRoofCorrection]
		, d.[CloseoutVolumeTotalObserved]
		, d.[CloseoutVolumeWater]
		, d.[Comment]
		, d.[Type]
		, d.[OrderNumber]
		, d.[PlannedStartTime]
		, d.[Product]
		, d.[ProductDescription]
		, d.[StartTime]
		, d.[StopTime]
		, d.[StartDensityProductObserved]
		, d.[StartDensityProductObservedTime]
		, d.[StartDensityProductObservedInAir]
		, d.[StartDensityProductStandard]
		, d.[StartDensityProductStandardTime]
		, d.[StartUserID]
		, d.[StartLevelProduct]
		, d.[StartLevelProductTime]
		, d.[StartLevelWater]
		, d.[StartLevelWaterTime]
		, d.[StartPercentBsw]
		, d.[StartMassLiquid]
		, d.[StartTankShellCorrection]
		, d.[StartTemperatureAmbient]
		, d.[StartTemperatureAmbientTime]
		, d.[StartTemperatureProduct]
		, d.[StartTemperatureProductTime]
		, d.[StartTemperatureDensity]
		, d.[StartTemperatureDensityTime]
		, d.[StartVolume]
		, d.[StartVolumeBsw]
		, d.[StartVolumeCorrectionFactor]
		, d.[StartVolumeGrossObserved]
		, d.[StartVolumeGrossStandard]
		, d.[StartVolumeNetStandard]
		, d.[StartVolumeRoofCorrection]
		, d.[StartVolumeTotalObserved]
		, d.[StartVolumeWater]
		, d.[UnitsLevelProductIndex]
		, d.[UnitsTemperatureAmbientIndex]
		, d.[UnitsTemperatureDensityIndex]
		, d.[UnitsTemperatureProductIndex]
		, d.[UnitsDensityProductObservedIndex]
		, d.[UnitsDensityProductStandardIndex]
		, d.[UnitsVolumeIndex]
		, d.[UnitsMassIndex]
		, d.[DecimalPlacesVolume]
		, d.[DecimalPlacesLevel]
		, d.[DecimalPlacesDensity]
		, d.[DecimalPlacesTemperature]
		, d.[UserData01]
		, d.[UserData02]
		, d.[UserData03]
		, d.[UserData04]
		, d.[UserData05]
		, d.[UserData06]
		, d.[UserData07]
		, d.[UserData08]
		, d.[UserData09]
		, d.[UserData10]
		, d.[TransferDeviation]
		, d.[TransferPercentDeviation]
		, d.[DecimalPlacesPercent]
		, d.[TransferMode]
		, d.[TransferStatus]
		, d.[TransferTarget]
		, d.[TransferLevelTarget]
		, d.[TransferTargetUnitsIndex]
		, d.[TransferVolumeTarget]
		, d.[TransferTimeRemaining]
		, d.[TransferDirection]
		, d.[CommentDateTime]
		, d.[CommentUserID]
		, d.[Status]
		, d.[VolumeWater]
		, d.[LevelProduct]
		, d.[StartDensityProductStandardInAir]
		, d.[TransferredVolumeWater]
		, d.[TransferredVolume]
		, d.[MidnightRecord]
		, d.[PointGuid]
		, d.[RootParentGuid]
		, d.[RecordSeq]
		, @_AuditEventType
		, @_AuditEventSequence
		, @_AuditSessionGUID
		, @_AuditSessionTokenID
		, @_AuditDateTime
		, @_AuditSiteGUID
		, NEWID()
		, @_UserId
		, @_AuditContext
	FROM deleted d
END

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblMovementHistory] ON [dbo].[tblMovementHistory] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMovementHistory','D')=1 
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
	SET @_AuditEventType = 'I' -- For Inserts 
	SET @_AuditEventSequence = 1 
	SELECT	@_AuditSessionGUID = s.SessionGuid 
		,	@_AuditSessionTokenID = s.SessionTokenID 
		,	@_AuditSiteGUID = s.SiteGuid
		,	@_UserId = u.UserId
		,	@_AuditContext = s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid = s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid = s.UserGuid 
	WHERE m.SqlServerSessionID = @@SPID 

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
	INSERT INTO [fmaudit].tblMovementHistory 
	(
		[MovementHistoryGuid]
		, [SiteGuid]
		, [Name]
		, [Node]
		, [InitiationCount]
		, [RecordType]
		, [TimeStamp]
		, [ParentGuid]
		, [AutoStart]
		, [AutoStartTime]
		, [AutoStop]
		, [AutoStopTime]
		, [CloseoutDataModifiedBy]
		, [CloseoutDensityProductInAir]
		, [CloseoutDensityProductObserved]
		, [CloseoutDensityProductObservedTime]
		, [CloseoutDensityProductStandard]
		, [CloseoutDensityProductStandardTime]
		, [CloseoutDensityProductStandardInAir]
		, [CloseoutLevelProduct]
		, [CloseoutLevelProductTime]
		, [CloseoutLevelWater]
		, [CloseoutMassLiquid]
		, [CloseoutPercentBsw]
		, [CloseoutRoofMass]
		, [CloseoutTankShellCorrection]
		, [CloseoutTemperatureAmbient]
		, [CloseoutTemperatureAmbientTime]
		, [CloseoutTemperatureDensity]
		, [CloseoutTemperatureProduct]
		, [CloseoutTime]
		, [CloseoutTransferGov]
		, [CloseoutTransferNsv]
		, [CloseoutTransferMassLiquid]
		, [CloseoutTransferVolumeWater]
		, [CloseoutVolumeBsw]
		, [CloseoutVolumeCorrectionFactor]
		, [CloseoutVolumeGrossObserved]
		, [CloseoutVolumeGrossStandard]
		, [CloseoutVolumeNetStandard]
		, [CloseoutVolumeRoofCorrection]
		, [CloseoutVolumeTotalObserved]
		, [CloseoutVolumeWater]
		, [Comment]
		, [Type]
		, [OrderNumber]
		, [PlannedStartTime]
		, [Product]
		, [ProductDescription]
		, [StartTime]
		, [StopTime]
		, [StartDensityProductObserved]
		, [StartDensityProductObservedTime]
		, [StartDensityProductObservedInAir]
		, [StartDensityProductStandard]
		, [StartDensityProductStandardTime]
		, [StartUserID]
		, [StartLevelProduct]
		, [StartLevelProductTime]
		, [StartLevelWater]
		, [StartLevelWaterTime]
		, [StartPercentBsw]
		, [StartMassLiquid]
		, [StartTankShellCorrection]
		, [StartTemperatureAmbient]
		, [StartTemperatureAmbientTime]
		, [StartTemperatureProduct]
		, [StartTemperatureProductTime]
		, [StartTemperatureDensity]
		, [StartTemperatureDensityTime]
		, [StartVolume]
		, [StartVolumeBsw]
		, [StartVolumeCorrectionFactor]
		, [StartVolumeGrossObserved]
		, [StartVolumeGrossStandard]
		, [StartVolumeNetStandard]
		, [StartVolumeRoofCorrection]
		, [StartVolumeTotalObserved]
		, [StartVolumeWater]
		, [UnitsLevelProductIndex]
		, [UnitsTemperatureAmbientIndex]
		, [UnitsTemperatureDensityIndex]
		, [UnitsTemperatureProductIndex]
		, [UnitsDensityProductObservedIndex]
		, [UnitsDensityProductStandardIndex]
		, [UnitsVolumeIndex]
		, [UnitsMassIndex]
		, [DecimalPlacesVolume]
		, [DecimalPlacesLevel]
		, [DecimalPlacesDensity]
		, [DecimalPlacesTemperature]
		, [UserData01]
		, [UserData02]
		, [UserData03]
		, [UserData04]
		, [UserData05]
		, [UserData06]
		, [UserData07]
		, [UserData08]
		, [UserData09]
		, [UserData10]
		, [TransferDeviation]
		, [TransferPercentDeviation]
		, [DecimalPlacesPercent]
		, [TransferMode]
		, [TransferStatus]
		, [TransferTarget]
		, [TransferTargetUnitsIndex]
		, [TransferLevelTarget]
		, [TransferVolumeTarget]
		, [TransferTimeRemaining]
		, [TransferDirection]
		, [CommentDateTime]
		, [CommentUserID]
		, [Status]
		, [VolumeWater]
		, [LevelProduct]
		, [StartDensityProductStandardInAir]
		, [TransferredVolumeWater]
		, [TransferredVolume]
		, [MidnightRecord]
		, [PointGuid]
		, [RootParentGuid]
		, [RecordSeq]
		, [_AuditEventType]
		, [_AuditEventSequence]
		, [_AuditSessionGUID]
		, [_AuditSessionTokenID]
		, [_AuditCreatedDate]
		, [_AuditSiteGUID]
		, [_AuditGUID]
		, [_AuditUserId]
		, [_AuditContext]
	)
	SELECT 
		i.[MovementHistoryGuid]
		, i.[SiteGuid]
		, i.[Name]
		, i.[Node]
		, i.[InitiationCount]
		, i.[RecordType]
		, i.[TimeStamp]
		, i.[ParentGuid]
		, i.[AutoStart]
		, i.[AutoStartTime]
		, i.[AutoStop]
		, i.[AutoStopTime]
		, i.[CloseoutDataModifiedBy]
		, i.[CloseoutDensityProductInAir]
		, i.[CloseoutDensityProductObserved]
		, i.[CloseoutDensityProductObservedTime]
		, i.[CloseoutDensityProductStandard]
		, i.[CloseoutDensityProductStandardTime]
		, i.[CloseoutDensityProductStandardInAir]
		, i.[CloseoutLevelProduct]
		, i.[CloseoutLevelProductTime]
		, i.[CloseoutLevelWater]
		, i.[CloseoutMassLiquid]
		, i.[CloseoutPercentBsw]
		, i.[CloseoutRoofMass]
		, i.[CloseoutTankShellCorrection]
		, i.[CloseoutTemperatureAmbient]
		, i.[CloseoutTemperatureAmbientTime]
		, i.[CloseoutTemperatureDensity]
		, i.[CloseoutTemperatureProduct]
		, i.[CloseoutTime]
		, i.[CloseoutTransferGov]
		, i.[CloseoutTransferNsv]
		, i.[CloseoutTransferMassLiquid]
		, i.[CloseoutTransferVolumeWater]
		, i.[CloseoutVolumeBsw]
		, i.[CloseoutVolumeCorrectionFactor]
		, i.[CloseoutVolumeGrossObserved]
		, i.[CloseoutVolumeGrossStandard]
		, i.[CloseoutVolumeNetStandard]
		, i.[CloseoutVolumeRoofCorrection]
		, i.[CloseoutVolumeTotalObserved]
		, i.[CloseoutVolumeWater]
		, i.[Comment]
		, i.[Type]
		, i.[OrderNumber]
		, i.[PlannedStartTime]
		, i.[Product]
		, i.[ProductDescription]
		, i.[StartTime]
		, i.[StopTime]
		, i.[StartDensityProductObserved]
		, i.[StartDensityProductObservedTime]
		, i.[StartDensityProductObservedInAir]
		, i.[StartDensityProductStandard]
		, i.[StartDensityProductStandardTime]
		, i.[StartUserID]
		, i.[StartLevelProduct]
		, i.[StartLevelProductTime]
		, i.[StartLevelWater]
		, i.[StartLevelWaterTime]
		, i.[StartPercentBsw]
		, i.[StartMassLiquid]
		, i.[StartTankShellCorrection]
		, i.[StartTemperatureAmbient]
		, i.[StartTemperatureAmbientTime]
		, i.[StartTemperatureProduct]
		, i.[StartTemperatureProductTime]
		, i.[StartTemperatureDensity]
		, i.[StartTemperatureDensityTime]
		, i.[StartVolume]
		, i.[StartVolumeBsw]
		, i.[StartVolumeCorrectionFactor]
		, i.[StartVolumeGrossObserved]
		, i.[StartVolumeGrossStandard]
		, i.[StartVolumeNetStandard]
		, i.[StartVolumeRoofCorrection]
		, i.[StartVolumeTotalObserved]
		, i.[StartVolumeWater]
		, i.[UnitsLevelProductIndex]
		, i.[UnitsTemperatureAmbientIndex]
		, i.[UnitsTemperatureDensityIndex]
		, i.[UnitsTemperatureProductIndex]
		, i.[UnitsDensityProductObservedIndex]
		, i.[UnitsDensityProductStandardIndex]
		, i.[UnitsVolumeIndex]
		, i.[UnitsMassIndex]
		, i.[DecimalPlacesVolume]
		, i.[DecimalPlacesLevel]
		, i.[DecimalPlacesDensity]
		, i.[DecimalPlacesTemperature]
		, i.[UserData01]
		, i.[UserData02]
		, i.[UserData03]
		, i.[UserData04]
		, i.[UserData05]
		, i.[UserData06]
		, i.[UserData07]
		, i.[UserData08]
		, i.[UserData09]
		, i.[UserData10]
		, i.[TransferDeviation]
		, i.[TransferPercentDeviation]
		, i.[DecimalPlacesPercent]
		, i.[TransferMode]
		, i.[TransferStatus]
		, i.[TransferTarget]
		, i.[TransferTargetUnitsIndex]
		, i.[TransferLevelTarget]
		, i.[TransferVolumeTarget]
		, i.[TransferTimeRemaining]
		, i.[TransferDirection]
		, i.[CommentDateTime]
		, i.[CommentUserID]
		, i.[Status]
		, i.[VolumeWater]
		, i.[LevelProduct]
		, i.[StartDensityProductStandardInAir]
		, i.[TransferredVolumeWater]
		, i.[TransferredVolume]
		, i.[MidnightRecord]
		, i.[PointGuid]
		, i.[RootParentGuid]
		, i.[RecordSeq]
		, @_AuditEventType
		, @_AuditEventSequence
		, @_AuditSessionGUID
		, @_AuditSessionTokenID
		, @_AuditDateTime
		, @_AuditSiteGUID
		, NEWID()
		, @_UserId
		, @_AuditContext
	FROM inserted i
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMovementHistory_ClusterIdx]
    ON [dbo].[tblMovementHistory]([_ClusterIdx] ASC);
