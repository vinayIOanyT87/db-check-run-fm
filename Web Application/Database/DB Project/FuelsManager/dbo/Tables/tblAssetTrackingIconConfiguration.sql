CREATE TABLE [dbo].[tblAssetTrackingIconConfiguration]
(
	[AssetTrackingIconConfigurationGuid] UNIQUEIDENTIFIER NOT NULL, 
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL,
    [IconConfigurationID] NVARCHAR(20) UNIQUE NOT NULL, 
	[EquipmentIconName] NVARCHAR(50) NULL,
	[EquipmentVarianceIconName] NVARCHAR(50) NULL, 
	[EquipmentInvestigationIconName] NVARCHAR(50) NULL, 
	[EquipmentCompleteInvestigationFailedIconName] NVARCHAR(50) NULL, 
	[EquipmentCompleteInvestigationPassedIconName] NVARCHAR(50) NULL, 
	[TankIconName] NVARCHAR(50) NULL, 
	[FacilityIconName] NVARCHAR(50) NULL, 
	[DeliveryLocationIconName] NVARCHAR(50) NULL, 
	[BreadcrumbIconName] NVARCHAR(50) NULL,
	[BreadcrumbVarianceIconName] NVARCHAR(50) NULL, 
	[BreadcrumbInvestigationIconName] NVARCHAR(50) NULL, 
	[BreadcrumbCompleteInvestigationFailedIconName] NVARCHAR(50) NULL,
	[BreadcrumbCompleteInvestigationPassedIconName] NVARCHAR(50) NULL,
	[MapPinIconName] NVARCHAR(50) NULL, 
	[CreatedDate] DATETIMEOFFSET CONSTRAINT [DF_tblAssetTrackingIconConfiguration_CreatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL, 
    [CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAssetTrackingIconConfiguration_CreatedBy] DEFAULT ('') NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET CONSTRAINT [DF_tblAssetTrackingIconConfiguration_UpdatedDate] DEFAULT (SYSDATETIMEOFFSET()) NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblAssetTrackingIconConfiguration_UpdatedBy] DEFAULT ('') NOT NULL, 
	[_RowVersion] ROWVERSION NOT NULL,
    [_ClusterIdx] BIGINT IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAssetTrackingIconConfiguration] PRIMARY KEY NONCLUSTERED ([AssetTrackingIconConfigurationGuid]),
	CONSTRAINT [FK_tblAssetTrackingIconConfiguration_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
)

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAssetTrackingIconConfiguration_ClusterIdx]
    ON [dbo].[tblAssetTrackingIconConfiguration]([_ClusterIdx] ASC);

	GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAssetTrackingIconConfiguration] ON [dbo].[tblAssetTrackingIconConfiguration] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingIconConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblAssetTrackingIconConfiguration (
		[AssetTrackingIconConfigurationGuid]
	,	[SiteGuid]
	,	[IconConfigurationID]
	,	[EquipmentIconName]
	,	[EquipmentVarianceIconName]
	,	[EquipmentInvestigationIconName]
	,	[EquipmentCompleteInvestigationFailedIconName]
	,	[EquipmentCompleteInvestigationPassedIconName]
	,	[TankIconName]
	,	[FacilityIconName]
	,	[DeliveryLocationIconName]
	,	[BreadcrumbIconName]
	,	[BreadcrumbVarianceIconName]
	,	[BreadcrumbInvestigationIconName]
	,	[BreadcrumbCompleteInvestigationFailedIconName]
	,	[BreadcrumbCompleteInvestigationPassedIconName]
	,	[MapPinIconName]
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
		d.[AssetTrackingIconConfigurationGuid]
	,	d.[SiteGuid]
	,	d.[IconConfigurationID]
	,	d.[EquipmentIconName]
	,	d.[EquipmentVarianceIconName]
	,	d.[EquipmentInvestigationIconName]
	,	d.[EquipmentCompleteInvestigationFailedIconName]
	,	d.[EquipmentCompleteInvestigationPassedIconName]
	,	d.[TankIconName]
	,	d.[FacilityIconName]
	,	d.[DeliveryLocationIconName]
	,	d.[BreadcrumbIconName]
	,	d.[BreadcrumbVarianceIconName]
	,	d.[BreadcrumbInvestigationIconName]
	,	d.[BreadcrumbCompleteInvestigationFailedIconName]
	,	d.[BreadcrumbCompleteInvestigationPassedIconName]
	,	d.[MapPinIconName]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAssetTrackingIconConfiguration] ON [dbo].[tblAssetTrackingIconConfiguration] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingIconConfiguration','D')=1 
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
	INSERT INTO [fmaudit].tblAssetTrackingIconConfiguration (
		[AssetTrackingIconConfigurationGuid]
	,	[SiteGuid]
	,	[IconConfigurationID]
	,	[EquipmentIconName]
	,	[EquipmentVarianceIconName]
	,	[EquipmentInvestigationIconName]
	,	[EquipmentCompleteInvestigationFailedIconName]
	,	[EquipmentCompleteInvestigationPassedIconName]
	,	[TankIconName]
	,	[FacilityIconName]
	,	[DeliveryLocationIconName]
	,	[BreadcrumbIconName]
	,	[BreadcrumbVarianceIconName]
	,	[BreadcrumbInvestigationIconName]
	,	[BreadcrumbCompleteInvestigationFailedIconName]
	,	[BreadcrumbCompleteInvestigationPassedIconName]
	,	[MapPinIconName]
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
		i.[AssetTrackingIconConfigurationGuid]
	,	i.[SiteGuid]
	,	i.[IconConfigurationID]
	,	i.[EquipmentIconName]
	,	i.[EquipmentVarianceIconName]
	,	i.[EquipmentInvestigationIconName]
	,	i.[EquipmentCompleteInvestigationFailedIconName]
	,	i.[EquipmentCompleteInvestigationPassedIconName]
	,	i.[TankIconName]
	,	i.[FacilityIconName]
	,	i.[DeliveryLocationIconName]
	,	i.[BreadcrumbIconName]
	,	i.[BreadcrumbVarianceIconName]
	,	i.[BreadcrumbInvestigationIconName]
	,	i.[BreadcrumbCompleteInvestigationFailedIconName]
	,	i.[BreadcrumbCompleteInvestigationPassedIconName]
	,	i.[MapPinIconName]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAssetTrackingIconConfiguration] ON [dbo].[tblAssetTrackingIconConfiguration] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAssetTrackingIconConfiguration','D')=1 
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
	AssetTrackingIconConfigurationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAssetTrackingIconConfiguration (
		[AssetTrackingIconConfigurationGuid]
	,	[SiteGuid]
	,	[IconConfigurationID]
	,	[EquipmentIconName]
	,	[EquipmentVarianceIconName]
	,	[EquipmentInvestigationIconName]
	,	[EquipmentCompleteInvestigationFailedIconName]
	,	[EquipmentCompleteInvestigationPassedIconName]
	,	[TankIconName]
	,	[FacilityIconName]
	,	[DeliveryLocationIconName]
	,	[BreadcrumbIconName]
	,	[BreadcrumbVarianceIconName]
	,	[BreadcrumbInvestigationIconName]
	,	[BreadcrumbCompleteInvestigationFailedIconName]
	,	[BreadcrumbCompleteInvestigationPassedIconName]
	,	[MapPinIconName]
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
	OUTPUT inserted.[AssetTrackingIconConfigurationGuid] AS 'AssetTrackingIconConfigurationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AssetTrackingIconConfigurationGuid]
	,	d.[SiteGuid]
	,	d.[IconConfigurationID]
	,	d.[EquipmentIconName]
	,	d.[EquipmentVarianceIconName]
	,	d.[EquipmentInvestigationIconName]
	,	d.[EquipmentCompleteInvestigationFailedIconName]
	,	d.[EquipmentCompleteInvestigationPassedIconName]
	,	d.[TankIconName]
	,	d.[FacilityIconName]
	,	d.[DeliveryLocationIconName]
	,	d.[BreadcrumbIconName]
	,	d.[BreadcrumbVarianceIconName]
	,	d.[BreadcrumbInvestigationIconName]
	,	d.[BreadcrumbCompleteInvestigationFailedIconName]
	,	d.[BreadcrumbCompleteInvestigationPassedIconName]
	,	d.[MapPinIconName]
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
 
	INSERT INTO [fmaudit].tblAssetTrackingIconConfiguration (
		[AssetTrackingIconConfigurationGuid]
	,	[SiteGuid]
	,	[IconConfigurationID]
	,	[EquipmentIconName]
	,	[EquipmentVarianceIconName]
	,	[EquipmentInvestigationIconName]
	,	[EquipmentCompleteInvestigationFailedIconName]
	,	[EquipmentCompleteInvestigationPassedIconName]
	,	[TankIconName]
	,	[FacilityIconName]
	,	[DeliveryLocationIconName]
	,	[BreadcrumbIconName]
	,	[BreadcrumbVarianceIconName]
	,	[BreadcrumbInvestigationIconName]
	,	[BreadcrumbCompleteInvestigationFailedIconName]
	,	[BreadcrumbCompleteInvestigationPassedIconName]
	,	[MapPinIconName]
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
		i.[AssetTrackingIconConfigurationGuid]
	,	i.[SiteGuid]
	,	i.[IconConfigurationID]
	,	i.[EquipmentIconName]
	,	i.[EquipmentVarianceIconName]
	,	i.[EquipmentInvestigationIconName]
	,	i.[EquipmentCompleteInvestigationFailedIconName]
	,	i.[EquipmentCompleteInvestigationPassedIconName]
	,	i.[TankIconName]
	,	i.[FacilityIconName]
	,	i.[DeliveryLocationIconName]
	,	i.[BreadcrumbIconName]
	,	i.[BreadcrumbVarianceIconName]
	,	i.[BreadcrumbInvestigationIconName]
	,	i.[BreadcrumbCompleteInvestigationFailedIconName]
	,	i.[BreadcrumbCompleteInvestigationPassedIconName]
	,	i.[MapPinIconName]
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
			agl.[AssetTrackingIconConfigurationGuid]=i.[AssetTrackingIconConfigurationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END