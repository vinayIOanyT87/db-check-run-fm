CREATE TABLE [dbo].[tblGasboyDevice]
(
	[GasboyDeviceGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL, 
	[GasboyDepartmentGuid] UNIQUEIDENTIFIER NOT NULL,
	[DeviceCode] BIGINT NOT NULL,
	[DeviceName] NVARCHAR(50) NOT NULL,
	[CardNumber] NVARCHAR(50) NULL,
	[GroupRuleName] NVARCHAR(50) NULL,
	[LookupGasboyDeviceTypeIndex] INT NOT NULL,
	[LookupGasboyRecordStatusIndex] INT NOT NULL,
	[LookupGasboyHardwareTypeIndex] INT NOT NULL,
	[LookupGasboyAuthTypeIndex] INT NOT NULL,
	[LookupGasboyEmployeeTypeIndex] INT NOT NULL,
	[LookupGasboyTwoStageDriverValidationTypeIndex] INT NOT NULL,
	[UsePINCodeFlag] BIT CONSTRAINT [DF_tblGasboyDevice_UsePINCodeFlag] DEFAULT (0) NOT NULL,
	[PINCode] VARBINARY(256) NOT NULL,
	[AuthPINFrom] TINYINT CONSTRAINT [DF_tblGasboyDevice_AuthPINFrom] DEFAULT (2) NOT NULL,
	[VehiclePlate] NVARCHAR(50) NOT NULL,
	[PromptForVehiclePlateFlag] BIT NOT NULL,
	[LookupGasboyVehiclePlateCheckTypeIndex] INT NOT NULL,
	[AlwaysPromptForAdditionalValidationFlag] TINYINT NOT NULL,
	[CreatedBy] [dbo].[udtUserID] NOT NULL, 
	[CreatedDate] DATETIMEOFFSET NOT NULL, 
	[UpdatedBy] [dbo].[udtUserID] NOT NULL, 
	[UpdatedDate] DATETIMEOFFSET NOT NULL, 
	[_RowVersion] TIMESTAMP NOT NULL,   
	[_ClusterIdx] BIGINT IDENTITY(1,1) NOT NULL, 
	[DeviceID] BIGINT NOT NULL,
	CONSTRAINT [PK_tblGasboyDevice] PRIMARY KEY NONCLUSTERED ([GasboyDeviceGuid]), 
	CONSTRAINT [FK_tblGasboyDevice_SiteGuid] FOREIGN KEY (SiteGuid) REFERENCES [dbo].[tblSites]([SiteGuid]), 
	CONSTRAINT [FK_tblGasboyDevice_GasboyDepartmentGuid] FOREIGN KEY (GasboyDepartmentGuid) REFERENCES [dbo].[tblGasboyDepartment]([GasboyDepartmentGuid]), 
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyRecordStatusIndex] FOREIGN KEY ([LookupGasboyRecordStatusIndex]) REFERENCES [lookup].[tblGasboyRecordStatus]([GasboyRecordStatusIndex]), 
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyVehiclePlateCheckTypeIndex] FOREIGN KEY (LookupGasboyVehiclePlateCheckTypeIndex) REFERENCES [lookup].[tblGasboyVehiclePlateCheckType]([GasboyVehiclePlateCheckTypeIndex]),
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyDeviceTypeIndex] FOREIGN KEY (LookupGasboyDeviceTypeIndex) REFERENCES [lookup].[tblGasboyDeviceType]([GasboyDeviceTypeIndex]),
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyHardwareTypeIndex] FOREIGN KEY (LookupGasboyHardwareTypeIndex) REFERENCES [lookup].[tblGasboyHardwareType]([GasboyHardwareTypeIndex]),
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyAuthTypeIndex] FOREIGN KEY (LookupGasboyAuthTypeIndex) REFERENCES [lookup].[tblGasboyAuthType]([GasboyAuthTypeIndex]),
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyEmployeeTypeIndex] FOREIGN KEY (LookupGasboyEmployeeTypeIndex) REFERENCES [lookup].[tblGasboyEmployeeType]([GasboyEmployeeTypeIndex]),
	CONSTRAINT [FK_tblGasboyDevice_LookupGasboyTwoStageDriverValidationTypeIndex] FOREIGN KEY ([LookupGasboyTwoStageDriverValidationTypeIndex]) REFERENCES [lookup].[tblGasboyTwoStageDriverValidationType]([GasboyTwoStageDriverValidationTypeIndex])
)
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblGasboyDevice__ClusterIdx] ON [dbo].[tblGasboyDevice] (_ClusterIdx)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_SiteGuid] ON [dbo].[tblGasboyDevice] (SiteGuid)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_GasboyDepartmentGuid] ON [dbo].[tblGasboyDevice] (GasboyDepartmentGuid)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_DeviceID] ON [dbo].[tblGasboyDevice] (DeviceID)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_DeviceName] ON [dbo].[tblGasboyDevice] (DeviceName)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_CardNumber] ON [dbo].[tblGasboyDevice] (CardNumber)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_LookupGasboyRecordStatusIndex] ON [dbo].[tblGasboyDevice] (LookupGasboyRecordStatusIndex)
GO

CREATE NONCLUSTERED INDEX [IX_tblGasboyDevice_LookupGasboyVehiclePlateCheckTypeIndex] ON [dbo].[tblGasboyDevice] (LookupGasboyVehiclePlateCheckTypeIndex)
GO
-------------------------------------
-- AUDIT DELETE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_del_tblGasboyDevice] ON [dbo].[tblGasboyDevice] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyDevice','D')=1 
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

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblGasboyDevice (
		[GasboyDeviceGuid]
	,	[SiteGuid]
	,	[GasboyDepartmentGuid]
	,	[DeviceCode]
	,	[DeviceName]
	,	[CardNumber]
	,	[GroupRuleName]
	,	[LookupGasboyDeviceTypeIndex]
	,	[LookupGasboyRecordStatusIndex]
	,	[LookupGasboyHardwareTypeIndex]
	,	[LookupGasboyAuthTypeIndex]
	,	[LookupGasboyEmployeeTypeIndex]
	,	[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[VehiclePlate]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[DeviceID]
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
		d.[GasboyDeviceGuid]
	,	d.[SiteGuid]
	,	d.[GasboyDepartmentGuid]
	,	d.[DeviceCode]
	,	d.[DeviceName]
	,	d.[CardNumber]
	,	d.[GroupRuleName]
	,	d.[LookupGasboyDeviceTypeIndex]
	,	d.[LookupGasboyRecordStatusIndex]
	,	d.[LookupGasboyHardwareTypeIndex]
	,	d.[LookupGasboyAuthTypeIndex]
	,	d.[LookupGasboyEmployeeTypeIndex]
	,	d.[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	d.[UsePINCodeFlag]
	,	d.[PINCode]
	,	d.[AuthPINFrom]
	,	d.[VehiclePlate]
	,	d.[PromptForVehiclePlateFlag]
	,	d.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	d.[AlwaysPromptForAdditionalValidationFlag]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[_RowVersion]
	,	d.[DeviceID]
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
 
-------------------------------------
-- AUDIT INSERT TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_ins_tblGasboyDevice] ON [dbo].[tblGasboyDevice] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyDevice','D')=1 
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

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblGasboyDevice (
		[GasboyDeviceGuid]
	,	[SiteGuid]
	,	[GasboyDepartmentGuid]
	,	[DeviceCode]
	,	[DeviceName]
	,	[CardNumber]
	,	[GroupRuleName]
	,	[LookupGasboyDeviceTypeIndex]
	,	[LookupGasboyRecordStatusIndex]
	,	[LookupGasboyHardwareTypeIndex]
	,	[LookupGasboyAuthTypeIndex]
	,	[LookupGasboyEmployeeTypeIndex]
	,	[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[VehiclePlate]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[DeviceID]
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
		i.[GasboyDeviceGuid]
	,	i.[SiteGuid]
	,	i.[GasboyDepartmentGuid]
	,	i.[DeviceCode]
	,	i.[DeviceName]
	,	i.[CardNumber]
	,	i.[GroupRuleName]
	,	i.[LookupGasboyDeviceTypeIndex]
	,	i.[LookupGasboyRecordStatusIndex]
	,	i.[LookupGasboyHardwareTypeIndex]
	,	i.[LookupGasboyAuthTypeIndex]
	,	i.[LookupGasboyEmployeeTypeIndex]
	,	i.[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	i.[UsePINCodeFlag]
	,	i.[PINCode]
	,	i.[AuthPINFrom]
	,	i.[VehiclePlate]
	,	i.[PromptForVehiclePlateFlag]
	,	i.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	i.[AlwaysPromptForAdditionalValidationFlag]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[_RowVersion]
	,	i.[DeviceID]
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
 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblGasboyDevice] ON [dbo].[tblGasboyDevice] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblGasboyDevice','D')=1 
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

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
 
	DECLARE @AuditGuidList TABLE
	(
	GasboyDeviceGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblGasboyDevice (
		[GasboyDeviceGuid]
	,	[SiteGuid]
	,	[GasboyDepartmentGuid]
	,	[DeviceCode]
	,	[DeviceName]
	,	[CardNumber]
	,	[GroupRuleName]
	,	[LookupGasboyDeviceTypeIndex]
	,	[LookupGasboyRecordStatusIndex]
	,	[LookupGasboyHardwareTypeIndex]
	,	[LookupGasboyAuthTypeIndex]
	,	[LookupGasboyEmployeeTypeIndex]
	,	[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[VehiclePlate]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[DeviceID]
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
	OUTPUT inserted.[GasboyDeviceGuid] AS 'GasboyDeviceGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[GasboyDeviceGuid]
	,	d.[SiteGuid]
	,	d.[GasboyDepartmentGuid]
	,	d.[DeviceCode]
	,	d.[DeviceName]
	,	d.[CardNumber]
	,	d.[GroupRuleName]
	,	d.[LookupGasboyDeviceTypeIndex]
	,	d.[LookupGasboyRecordStatusIndex]
	,	d.[LookupGasboyHardwareTypeIndex]
	,	d.[LookupGasboyAuthTypeIndex]
	,	d.[LookupGasboyEmployeeTypeIndex]
	,	d.[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	d.[UsePINCodeFlag]
	,	d.[PINCode]
	,	d.[AuthPINFrom]
	,	d.[VehiclePlate]
	,	d.[PromptForVehiclePlateFlag]
	,	d.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	d.[AlwaysPromptForAdditionalValidationFlag]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[_RowVersion]
	,	d.[DeviceID]
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
 
	INSERT INTO [fmaudit].tblGasboyDevice (
		[GasboyDeviceGuid]
	,	[SiteGuid]
	,	[GasboyDepartmentGuid]
	,	[DeviceCode]
	,	[DeviceName]
	,	[CardNumber]
	,	[GroupRuleName]
	,	[LookupGasboyDeviceTypeIndex]
	,	[LookupGasboyRecordStatusIndex]
	,	[LookupGasboyHardwareTypeIndex]
	,	[LookupGasboyAuthTypeIndex]
	,	[LookupGasboyEmployeeTypeIndex]
	,	[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	[UsePINCodeFlag]
	,	[PINCode]
	,	[AuthPINFrom]
	,	[VehiclePlate]
	,	[PromptForVehiclePlateFlag]
	,	[LookupGasboyVehiclePlateCheckTypeIndex]
	,	[AlwaysPromptForAdditionalValidationFlag]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[DeviceID]
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
		i.[GasboyDeviceGuid]
	,	i.[SiteGuid]
	,	i.[GasboyDepartmentGuid]
	,	i.[DeviceCode]
	,	i.[DeviceName]
	,	i.[CardNumber]
	,	i.[GroupRuleName]
	,	i.[LookupGasboyDeviceTypeIndex]
	,	i.[LookupGasboyRecordStatusIndex]
	,	i.[LookupGasboyHardwareTypeIndex]
	,	i.[LookupGasboyAuthTypeIndex]
	,	i.[LookupGasboyEmployeeTypeIndex]
	,	i.[LookupGasboyTwoStageDriverValidationTypeIndex]
	,	i.[UsePINCodeFlag]
	,	i.[PINCode]
	,	i.[AuthPINFrom]
	,	i.[VehiclePlate]
	,	i.[PromptForVehiclePlateFlag]
	,	i.[LookupGasboyVehiclePlateCheckTypeIndex]
	,	i.[AlwaysPromptForAdditionalValidationFlag]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[_RowVersion]
	,	i.[DeviceID]
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
			agl.[GasboyDeviceGuid]=i.[GasboyDeviceGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


--Creating Insert / Update Trigger for tblGasboyDevice
CREATE TRIGGER dbo.trg_insupd_tblGasboyDevice_ForSync 
   ON dbo.tblGasboyDevice
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
 
       ; WITH ChangeList AS ( 
       SELECT @syncContext AS ChangeContext 
                   ,d.GasboyDeviceGuid AS Deleted_PK_GasboyDeviceGuid
                    ,i.GasboyDeviceGuid AS Inserted_PK_GasboyDeviceGuid
                    ,NULL AS Deleted_FK_ParentPK 
                    ,NULL AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,NULL AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.GasboyDeviceGuid = i.GasboyDeviceGuid
           ) 
		    MERGE INTO track.tblGasboyDevice WITH (HOLDLOCK) As currentTrackingData 
			    USING ChangeList As entityChanges 
				    ON entityChanges.Inserted_PK_GasboyDeviceGuid = currentTrackingData.PK_GasboyDeviceGuid
           WHEN Matched 
		    THEN 
		    UPDATE SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
		    WHEN Not Matched 
		    THEN 
		    INSERT (InsertedDate 
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
				    ,PK_GasboyDeviceGuid
				    ,FK_ParentPK 
		    )
		    VALUES (CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
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
				    ,entityChanges.Inserted_PK_GasboyDeviceGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    );
    END
END
GO
--Creating Delete Trigger for tblGasboyDevice
CREATE TRIGGER dbo.trg_del_tblGasboyDevice_ForSync 
   ON dbo.tblGasboyDevice
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
						,d.GasboyDeviceGuid AS Deleted_PK_GasboyDeviceGuid
                        ,d.GasboyDeviceGuid AS Inserted_PK_GasboyDeviceGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblGasboyDevice WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_GasboyDeviceGuid = currentTrackingData.PK_GasboyDeviceGuid
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
						,PK_GasboyDeviceGuid
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
						,entityChanges.Deleted_PK_GasboyDeviceGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END