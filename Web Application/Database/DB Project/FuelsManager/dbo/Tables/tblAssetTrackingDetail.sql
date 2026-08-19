CREATE TABLE [dbo].[tblAssetTrackingDetail]
(
	[AssetTrackingDetailGuid] UNIQUEIDENTIFIER NOT NULL, 
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL DEFAULT ('00000000-0000-0000-0000-000000000001'),
    [EquipmentID] NVARCHAR(30) NULL, 
    [ProductID] NVARCHAR(30) NULL, 
    [ConvoyID] NVARCHAR(50) NULL, 
    [AssetTrackingDeviceID] NVARCHAR(30) NULL, 
    [AssetSessionDateTime] DATETIME NULL, 
    [AssetSessionStatus] INT NULL, 
    [MOMSN] INT NULL, 
    [MTMSN] INT NULL, 
    [CDRReference] INT NULL, 
    [Latitude] FLOAT NULL, 
    [Longitude] FLOAT NULL, 
    [CEPRadius] INT NULL,
	[ChecksumFlag] BIT NULL,
	[Contaminated] BIT NULL,
	[StartInvestigationDate] DATETIME NULL,
	[CompleteInvestigationDate] DATETIME NULL,
	[Remarks] NVARCHAR(4000) NULL,
	[LookupAssetTrackingPayloadTypeIndex] INT NULL,
	[LookupAssetTrackingMessageStateIndex] INT NULL,
	[CreatedDate] DATETIMEOFFSET CONSTRAINT [DF_tblAssetTrackingDetail_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL, 
    [CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAssetTrackingDetail_CreatedBy] DEFAULT ('') NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET CONSTRAINT [DF_tblAssetTrackingDetail_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAssetTrackingDetail_UpdatedBy] DEFAULT ('') NOT NULL, 
	[_RowVersion] ROWVERSION NOT NULL,
    [_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL, 
    CONSTRAINT [PK_tblAssetTrackingDetail] PRIMARY KEY NONCLUSTERED ([AssetTrackingDetailGuid]),
	CONSTRAINT [FK_tblAssetTrackingDetail_LookupAssetTrackingPayloadTypeIndex] FOREIGN KEY ([LookupAssetTrackingPayloadTypeIndex]) REFERENCES [lookup].[tblAssetTrackingPayloadType] ([AssetTrackingPayloadTypeIndex]),
	CONSTRAINT [FK_tblAssetTrackingDetail_LookupAssetTrackingMessageStateIndex] FOREIGN KEY ([LookupAssetTrackingMessageStateIndex]) REFERENCES [lookup].[tblAssetTrackingMessageState] ([AssetTrackingMessageStateIndex]),
	CONSTRAINT [FK_tblAssetTrackingDetail_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAssetTrackingDetail_ClusterIdx]
    ON [dbo].[tblAssetTrackingDetail]([_ClusterIdx] ASC);
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAssetTrackingDetail] ON [dbo].[tblAssetTrackingDetail] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingDetail','D')=1 
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
	SET IDENTITY_INSERT [fmaudit].tblAssetTrackingDetail ON
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
	INSERT INTO [fmaudit].tblAssetTrackingDetail (
		[AssetTrackingDetailGuid]
	,	[SiteGuid]
	,	[EquipmentID]
	,	[ProductID]
	,	[ConvoyID]
	,	[AssetTrackingDeviceID]
	,	[AssetSessionDateTime]
	,	[AssetSessionStatus]
	,	[MOMSN]
	,	[MTMSN]
	,	[CDRReference]
	,	[Latitude]
	,	[Longitude]
	,	[CEPRadius]
	,	[ChecksumFlag]
	,	[Contaminated]
	,	[StartInvestigationDate]
	,	[CompleteInvestigationDate]
	,	[Remarks]
	,	[LookupAssetTrackingPayloadTypeIndex]
	,	[LookupAssetTrackingMessageStateIndex]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
		d.[AssetTrackingDetailGuid]
	,	d.[SiteGuid]
	,	d.[EquipmentID]
	,	d.[ProductID]
	,	d.[ConvoyID]
	,	d.[AssetTrackingDeviceID]
	,	d.[AssetSessionDateTime]
	,	d.[AssetSessionStatus]
	,	d.[MOMSN]
	,	d.[MTMSN]
	,	d.[CDRReference]
	,	d.[Latitude]
	,	d.[Longitude]
	,	d.[CEPRadius]
	,	d.[ChecksumFlag]
	,	d.[Contaminated]
	,	d.[StartInvestigationDate]
	,	d.[CompleteInvestigationDate]
	,	d.[Remarks]
	,	d.[LookupAssetTrackingPayloadTypeIndex]
	,	d.[LookupAssetTrackingMessageStateIndex]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAssetTrackingDetail] ON [dbo].[tblAssetTrackingDetail] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingDetail','D')=1 
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
	SET IDENTITY_INSERT [fmaudit].tblAssetTrackingDetail ON
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
	INSERT INTO [fmaudit].tblAssetTrackingDetail (
		[AssetTrackingDetailGuid]
	,	[SiteGuid]
	,	[EquipmentID]
	,	[ProductID]
	,	[ConvoyID]
	,	[AssetTrackingDeviceID]
	,	[AssetSessionDateTime]
	,	[AssetSessionStatus]
	,	[MOMSN]
	,	[MTMSN]
	,	[CDRReference]
	,	[Latitude]
	,	[Longitude]
	,	[CEPRadius]
	,	[ChecksumFlag]
	,	[Contaminated]
	,	[StartInvestigationDate]
	,	[CompleteInvestigationDate]
	,	[Remarks]
	,	[LookupAssetTrackingPayloadTypeIndex]
	,	[LookupAssetTrackingMessageStateIndex]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
		i.[AssetTrackingDetailGuid]
	,	i.[SiteGuid]
	,	i.[EquipmentID]
	,	i.[ProductID]
	,	i.[ConvoyID]
	,	i.[AssetTrackingDeviceID]
	,	i.[AssetSessionDateTime]
	,	i.[AssetSessionStatus]
	,	i.[MOMSN]
	,	i.[MTMSN]
	,	i.[CDRReference]
	,	i.[Latitude]
	,	i.[Longitude]
	,	i.[CEPRadius]
	,	i.[ChecksumFlag]
	,	i.[Contaminated]
	,	i.[StartInvestigationDate]
	,	i.[CompleteInvestigationDate]
	,	i.[Remarks]
	,	i.[LookupAssetTrackingPayloadTypeIndex]
	,	i.[LookupAssetTrackingMessageStateIndex]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAssetTrackingDetail] ON [dbo].[tblAssetTrackingDetail] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingDetail','D')=1 
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
	SET IDENTITY_INSERT [fmaudit].tblAssetTrackingDetail ON
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
	AssetTrackingDetailGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAssetTrackingDetail (
		[AssetTrackingDetailGuid]
	,	[SiteGuid]
	,	[EquipmentID]
	,	[ProductID]
	,	[ConvoyID]
	,	[AssetTrackingDeviceID]
	,	[AssetSessionDateTime]
	,	[AssetSessionStatus]
	,	[MOMSN]
	,	[MTMSN]
	,	[CDRReference]
	,	[Latitude]
	,	[Longitude]
	,	[CEPRadius]
	,	[ChecksumFlag]
	,	[Contaminated]
	,	[StartInvestigationDate]
	,	[CompleteInvestigationDate]
	,	[Remarks]
	,	[LookupAssetTrackingPayloadTypeIndex]
	,	[LookupAssetTrackingMessageStateIndex]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
	OUTPUT inserted.[AssetTrackingDetailGuid] AS 'AssetTrackingDetailGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AssetTrackingDetailGuid]
	,	d.[SiteGuid]
	,	d.[EquipmentID]
	,	d.[ProductID]
	,	d.[ConvoyID]
	,	d.[AssetTrackingDeviceID]
	,	d.[AssetSessionDateTime]
	,	d.[AssetSessionStatus]
	,	d.[MOMSN]
	,	d.[MTMSN]
	,	d.[CDRReference]
	,	d.[Latitude]
	,	d.[Longitude]
	,	d.[CEPRadius]
	,	d.[ChecksumFlag]
	,	d.[Contaminated]
	,	d.[StartInvestigationDate]
	,	d.[CompleteInvestigationDate]
	,	d.[Remarks]
	,	d.[LookupAssetTrackingPayloadTypeIndex]
	,	d.[LookupAssetTrackingMessageStateIndex]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
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
 
	INSERT INTO [fmaudit].tblAssetTrackingDetail (
		[AssetTrackingDetailGuid]
	,	[SiteGuid]
	,	[EquipmentID]
	,	[ProductID]
	,	[ConvoyID]
	,	[AssetTrackingDeviceID]
	,	[AssetSessionDateTime]
	,	[AssetSessionStatus]
	,	[MOMSN]
	,	[MTMSN]
	,	[CDRReference]
	,	[Latitude]
	,	[Longitude]
	,	[CEPRadius]
	,	[ChecksumFlag]
	,	[Contaminated]
	,	[StartInvestigationDate]
	,	[CompleteInvestigationDate]
	,	[Remarks]
	,	[LookupAssetTrackingPayloadTypeIndex]
	,	[LookupAssetTrackingMessageStateIndex]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
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
		i.[AssetTrackingDetailGuid]
	,	i.[SiteGuid]
	,	i.[EquipmentID]
	,	i.[ProductID]
	,	i.[ConvoyID]
	,	i.[AssetTrackingDeviceID]
	,	i.[AssetSessionDateTime]
	,	i.[AssetSessionStatus]
	,	i.[MOMSN]
	,	i.[MTMSN]
	,	i.[CDRReference]
	,	i.[Latitude]
	,	i.[Longitude]
	,	i.[CEPRadius]
	,	i.[ChecksumFlag]
	,	i.[Contaminated]
	,	i.[StartInvestigationDate]
	,	i.[CompleteInvestigationDate]
	,	i.[Remarks]
	,	i.[LookupAssetTrackingPayloadTypeIndex]
	,	i.[LookupAssetTrackingMessageStateIndex]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
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
			agl.[AssetTrackingDetailGuid]=i.[AssetTrackingDetailGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END