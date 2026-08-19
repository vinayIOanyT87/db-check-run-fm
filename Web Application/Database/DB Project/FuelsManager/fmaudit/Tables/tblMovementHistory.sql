CREATE TABLE [fmaudit].[tblMovementHistory]
(
	[MovementHistoryGuid]					UNIQUEIDENTIFIER NULL
	, [SiteGuid]							UNIQUEIDENTIFIER NULL
	, [Name]								NVARCHAR(100) NULL
	, [Node]								NVARCHAR(100) NULL
	, [InitiationCount]						BIGINT
	, [RecordType]							INT NULL
	, [TimeStamp]							DATETIME NULL -- Time as UTC
	, [ParentGuid]							UNIQUEIDENTIFIER NULL
	, [AutoStart]							BIT NULL
	, [AutoStartTime]						DATETIMEOFFSET(7) NULL
	, [AutoStop]							BIT NULL
	, [AutoStopTime]						DATETIMEOFFSET(7) NULL
	, [CloseoutDataModifiedBy]				NVARCHAR(50) NULL
	, [CloseoutDensityProductInAir]			FLOAT NULL
	, [CloseoutDensityProductObserved]		FLOAT NULL
	, [CloseoutDensityProductObservedTime]	DATETIMEOFFSET(7) NULL
	, [CloseoutDensityProductStandard]		FLOAT NULL
	, [CloseoutDensityProductStandardTime]	DATETIMEOFFSET(7) NULL
	, [CloseoutDensityProductStandardInAir]	FLOAT NULL
	, [CloseoutLevelProduct]				FLOAT NULL
	, [CloseoutLevelProductTime]			DATETIMEOFFSET(7) NULL
	, [CloseoutLevelWater]					FLOAT NULL
	, [CloseoutMassLiquid]					FLOAT NULL
	, [CloseoutPercentBsw]					FLOAT NULL
	, [CloseoutRoofMass]					FLOAT NULL
	, [CloseoutTankShellCorrection]			FLOAT NULL
	, [CloseoutTemperatureAmbient]			FLOAT NULL
	, [CloseoutTemperatureAmbientTime]		DATETIMEOFFSET(7) NULL
	, [CloseoutTemperatureDensity]			FLOAT NULL
	, [CloseoutTemperatureProduct]			FLOAT NULL
	, [CloseoutTime]						DATETIMEOFFSET(7) NULL
	, [CloseoutTransferGov]					FLOAT NULL
	, [CloseoutTransferNsv]					FLOAT NULL
	, [CloseoutTransferMassLiquid]			FLOAT NULL
	, [CloseoutTransferVolumeWater]			FLOAT NULL
	, [CloseoutVolumeBsw]					FLOAT NULL
	, [CloseoutVolumeCorrectionFactor]		FLOAT NULL
	, [CloseoutVolumeGrossObserved]			FLOAT NULL
	, [CloseoutVolumeGrossStandard]			FLOAT NULL
	, [CloseoutVolumeNetStandard]			FLOAT NULL
	, [CloseoutVolumeRoofCorrection]		FLOAT NULL
	, [CloseoutVolumeTotalObserved]			FLOAT NULL
	, [CloseoutVolumeWater]					FLOAT NULL
	, [Comment]								NVARCHAR(1000) NULL
	, [Type]									NVARCHAR(20) NULL
	, [OrderNumber]							NVARCHAR(100) NULL
	, [PlannedStartTime]					DATETIMEOFFSET(7) NULL
	, [Product]								NVARCHAR(100) NULL
	, [ProductDescription]					NVARCHAR(1000) NULL
	, [StartTime]							DATETIMEOFFSET(7) NULL
	, [StopTime]							DATETIMEOFFSET(7) NULL
	, [StartDensityProductObserved]			FLOAT NULL
	, [StartDensityProductObservedTime]		DATETIMEOFFSET(7) NULL
	, [StartDensityProductObservedInAir]	FLOAT NULL
	, [StartDensityProductStandard]			FLOAT NULL
	, [StartDensityProductStandardTime]		DATETIMEOFFSET(7) NULL
	, [StartUserID]							NVARCHAR(100) NULL
	, [StartLevelProduct]					FLOAT NULL
	, [StartLevelProductTime]				DATETIMEOFFSET(7) NULL
	, [StartLevelWater]						FLOAT NULL
	, [StartLevelWaterTime]					DATETIMEOFFSET(7) NULL
	, [StartMassLiquid]						FLOAT NULL
	, [StartPercentBsw]						FLOAT NULL
	, [StartTankShellCorrection]			FLOAT NULL
	, [StartTemperatureAmbient]				FLOAT NULL
	, [StartTemperatureAmbientTime]			DATETIMEOFFSET(7) NULL
	, [StartTemperatureProduct]				FLOAT NULL
	, [StartTemperatureProductTime]			DATETIMEOFFSET(7) NULL
	, [StartTemperatureDensity]				FLOAT NULL
	, [StartTemperatureDensityTime]			DATETIMEOFFSET(7) NULL
	, [StartVolume]							FLOAT NULL
	, [StartVolumeBsw]								FLOAT NULL
	, [StartVolumeCorrectionFactor]			FLOAT NULL
	, [StartVolumeGrossObserved]			FLOAT NULL
	, [StartVolumeGrossStandard]			FLOAT NULL
	, [StartVolumeNetStandard]				FLOAT NULL
	, [StartVolumeRoofCorrection]			FLOAT NULL
	, [StartVolumeTotalObserved]			FLOAT NULL
	, [StartVolumeWater]					FLOAT NULL
	, [UnitsLevelProductIndex]				INT NULL
	, [UnitsTemperatureAmbientIndex]		INT NULL
	, [UnitsTemperatureDensityIndex]		INT NULL
	, [UnitsTemperatureProductIndex]		INT NULL
	, [UnitsDensityProductObservedIndex]	INT NULL
	, [UnitsDensityProductStandardIndex]	INT NULL
	, [UnitsVolumeIndex]					INT NULL
	, [UnitsMassIndex]						INT NULL
	, [DecimalPlacesVolume]				    INT NULL
	, [DecimalPlacesLevel]			        INT NULL
	, [DecimalPlacesDensity]			    INT NULL
	, [DecimalPlacesTemperature]			INT NULL
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
	, [_AuditEventType]					CHAR(1) NULL
	, [_AuditEventSequence]				TINYINT NULL CONSTRAINT DF_tblMovementHistory_AuditEventSequence DEFAULT 0
	, [_AuditSiteGuid]					UNIQUEIDENTIFIER NULL
	, [_AuditSessionGuid]				UNIQUEIDENTIFIER NULL
	, [_AuditUserID]						udtUserID NULL
	, [_AuditSessionTokenID]			UNIQUEIDENTIFIER NULL
	, [_AuditCreatedDate]				DATETIMEOFFSET(7) NULL CONSTRAINT DF_tblMovementHistory_AuditCreatedDate DEFAULT SYSDATETIMEOFFSET()
	, [_AuditGUID]							UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_tblMovementHistory_AuditGUID DEFAULT NEWID()
	, [_AuditRowVersion]					ROWVERSION 
	, [_ClusterIdx]						BIGINT IDENTITY (1, 1) NOT NULL 
	, [_AuditContext]						VARBINARY(128) NULL 
)
GO

CREATE NONCLUSTERED INDEX [IX_tblMovementHistory_AuditGUID] ON [fmaudit].[tblMovementHistory](_AuditGUID ASC)
GO

CREATE NONCLUSTERED INDEX [IX_tblMovementHistory_AuditRowVersion_EventType_EventSequence] ON [fmaudit].[tblMovementHistory] 
	([_AuditRowVersion] ASC, [_AuditEventType] ASC, [_AuditEventSequence] ASC) 
	INCLUDE ([_AuditSiteGuid], [_AuditSessionGuid], [_AuditUserID], [_AuditGUID]) 
	WITH (STATISTICS_NORECOMPUTE = OFF, DROP_EXISTING = OFF, ONLINE = OFF)
GO

CREATE CLUSTERED INDEX [IX_tblMovementHistory_ClusterIdx] ON [fmaudit].[tblMovementHistory](_ClusterIdx ASC)
GO