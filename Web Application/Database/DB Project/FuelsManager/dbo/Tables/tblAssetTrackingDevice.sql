CREATE TABLE [dbo].[tblAssetTrackingDevice]
(
	[AssetTrackingDeviceGuid] UNIQUEIDENTIFIER NOT NULL, 
    [SiteGuid] UNIQUEIDENTIFIER NULL, 
    [DeviceID] NVARCHAR(30) NOT NULL, 
    [Description] NVARCHAR(50) NULL, 
    [ModelNumber] NVARCHAR(50) NULL, 
    [SerialNumber] NVARCHAR(50) NULL, 
    [Active] BIT NULL,
	[LookupAssetTrackingDeviceTypeIndex] INT NULL,
	[LookupEngineeringUnitIndex] INT NULL,
	[CreatedDate] DATETIMEOFFSET CONSTRAINT [DF_tblAssetTrackingDevice_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL, 
    [CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAssetTrackingDevice_CreatedBy] DEFAULT ('') NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET CONSTRAINT [DF_tblAssetTrackingDevice_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAssetTrackingDevice_UpdatedBy] DEFAULT ('') NOT NULL, 
	[_RowVersion] ROWVERSION NOT NULL,
    [_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL, 
    CONSTRAINT [PK_tblAssetTrackingDevice] PRIMARY KEY NONCLUSTERED ([AssetTrackingDeviceGuid]),
	CONSTRAINT [CK_tblAssetTrackingDevice_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessAssetTrackingDevice]([AssetTrackingDeviceGuid],[SiteGuid],[DeviceID])=(1)),
	CONSTRAINT [FK_tblAssetTrackingAssetTrackingDevice_LookupAssetTrackingDeviceTypeIndex] FOREIGN KEY ([LookupAssetTrackingDeviceTypeIndex]) REFERENCES [lookup].[tblAssetTrackingDeviceType] ([AssetTrackingDeviceTypeIndex]),
	CONSTRAINT [FK_tblAssetTrackingAssetTrackingDevice_LookupEngineeringUnitIndex] FOREIGN KEY ([LookupEngineeringUnitIndex]) REFERENCES [lookup].[tblEngineeringUnit] ([EngineeringUnitIndex]),
	CONSTRAINT [FK_tblAssetTrackingDevice_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAssetTrackingDevice_ClusterIdx]
    ON [dbo].[tblAssetTrackingDevice]([_ClusterIdx] ASC);

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAssetTrackingDevice] ON [dbo].[tblAssetTrackingDevice] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingDevice','D')=1 
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
	INSERT INTO [fmaudit].tblAssetTrackingDevice (
		[AssetTrackingDeviceGuid]
	,	[SiteGuid]
	,	[DeviceID]
	,	[Description]
	,	[ModelNumber]
	,	[SerialNumber]
	,	[Active]
	,	[LookupAssetTrackingDeviceTypeIndex]
	,	[LookupEngineeringUnitIndex]
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
		d.[AssetTrackingDeviceGuid]
	,	d.[SiteGuid]
	,	d.[DeviceID]
	,	d.[Description]
	,	d.[ModelNumber]
	,	d.[SerialNumber]
	,	d.[Active]
	,	d.[LookupAssetTrackingDeviceTypeIndex]
	,	d.[LookupEngineeringUnitIndex]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAssetTrackingDevice] ON [dbo].[tblAssetTrackingDevice] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingDevice','D')=1 
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
	INSERT INTO [fmaudit].tblAssetTrackingDevice (
		[AssetTrackingDeviceGuid]
	,	[SiteGuid]
	,	[DeviceID]
	,	[Description]
	,	[ModelNumber]
	,	[SerialNumber]
	,	[Active]
	,	[LookupAssetTrackingDeviceTypeIndex]
	,	[LookupEngineeringUnitIndex]
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
		i.[AssetTrackingDeviceGuid]
	,	i.[SiteGuid]
	,	i.[DeviceID]
	,	i.[Description]
	,	i.[ModelNumber]
	,	i.[SerialNumber]
	,	i.[Active]
	,	i.[LookupAssetTrackingDeviceTypeIndex]
	,	i.[LookupEngineeringUnitIndex]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAssetTrackingDevice] ON [dbo].[tblAssetTrackingDevice] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingDevice','D')=1 
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
	AssetTrackingDeviceGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAssetTrackingDevice (
		[AssetTrackingDeviceGuid]
	,	[SiteGuid]
	,	[DeviceID]
	,	[Description]
	,	[ModelNumber]
	,	[SerialNumber]
	,	[Active]
	,	[LookupAssetTrackingDeviceTypeIndex]
	,	[LookupEngineeringUnitIndex]
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
	OUTPUT inserted.[AssetTrackingDeviceGuid] AS 'AssetTrackingDeviceGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AssetTrackingDeviceGuid]
	,	d.[SiteGuid]
	,	d.[DeviceID]
	,	d.[Description]
	,	d.[ModelNumber]
	,	d.[SerialNumber]
	,	d.[Active]
	,	d.[LookupAssetTrackingDeviceTypeIndex]
	,	d.[LookupEngineeringUnitIndex]
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
 
	INSERT INTO [fmaudit].tblAssetTrackingDevice (
		[AssetTrackingDeviceGuid]
	,	[SiteGuid]
	,	[DeviceID]
	,	[Description]
	,	[ModelNumber]
	,	[SerialNumber]
	,	[Active]
	,	[LookupAssetTrackingDeviceTypeIndex]
	,	[LookupEngineeringUnitIndex]
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
		i.[AssetTrackingDeviceGuid]
	,	i.[SiteGuid]
	,	i.[DeviceID]
	,	i.[Description]
	,	i.[ModelNumber]
	,	i.[SerialNumber]
	,	i.[Active]
	,	i.[LookupAssetTrackingDeviceTypeIndex]
	,	i.[LookupEngineeringUnitIndex]
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
			agl.[AssetTrackingDeviceGuid]=i.[AssetTrackingDeviceGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END