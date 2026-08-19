CREATE TABLE [dbo].[tblAppointmentEquipment] (
    [AppointmentEquipmentGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblAppointmentEquipment_GUID] DEFAULT (newid()) NOT NULL,
    [EquipmentGuid]                      UNIQUEIDENTIFIER   NOT NULL,
    [TestSetDefinitionGuid]              UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                           UNIQUEIDENTIFIER   NOT NULL,
    [AssetText]                          NVARCHAR (100)     CONSTRAINT [DF_tblAppointmentEquipment_AssetText] DEFAULT ('') NOT NULL,
    [AppointmentCategory]                NVARCHAR (50)      CONSTRAINT [DF_tblAppointmentEquipment_AppointmentCategory] DEFAULT ('') NOT NULL,
    [AppointmentIsSingle]                BIT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentIsSingle] DEFAULT ((0)) NOT NULL,
    [ScheduleOnWeekends]                 BIT                CONSTRAINT [DF_tblAppointmentEquipment_ScheduleOnWeekends] DEFAULT ((0)) NOT NULL,
    [ScheduleOnHolidays]                 BIT                CONSTRAINT [DF_tblAppointmentEquipment_ScheduleOnHolidays] DEFAULT ((0)) NOT NULL,
    [StartDate]                          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAppointmentEquipment_StartDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [Duration]                           INT                CONSTRAINT [DF_tblAppointmentEquipment_Duration] DEFAULT ((0)) NOT NULL,
    [AppointmentPeriod]                  INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentPeriod] DEFAULT ((0)) NOT NULL,
    [AppointmentPeriodText]              NVARCHAR (50)      CONSTRAINT [DF_tblAppointmentEquipment_AppointmentPeriodText] DEFAULT ('') NOT NULL,
    [Description]                        NVARCHAR (50)      CONSTRAINT [DF_tblAppointmentEquipment_Description] DEFAULT ('') NOT NULL,
    [AppointmentTimeInterval]            INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentTimeInterval] DEFAULT ((0)) NOT NULL,
    [AppointmentDayOfTheWeekText]        NVARCHAR (20)      CONSTRAINT [DF_tblAppointmentEquipment_AppointmentDayOfTheWeekText] DEFAULT ('') NOT NULL,
    [AppointmentDayOfTheWeek]            INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentDayOfTheWeek] DEFAULT ((0)) NOT NULL,
    [AppointmentReoccuranceInterval]     INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentReoccuranceInterval] DEFAULT ((0)) NOT NULL,
    [AppointmentOption2Selected]         BIT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentOption2Selected] DEFAULT ((0)) NOT NULL,
    [AppointmentTimeOptionSelectionText] NVARCHAR (20)      CONSTRAINT [DF_tblAppointmentEquipment_AppointmentTimeOptionSelectionText] DEFAULT ('') NOT NULL,
    [AppointmentTimeOptionSelection]     INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentTimeOptionSelection] DEFAULT ((0)) NOT NULL,
    [AppointmentMonthSelectionText]      NVARCHAR (20)      CONSTRAINT [DF_tblAppointmentEquipment_AppointmentMonthSelectionText] DEFAULT ('') NOT NULL,
    [AppointmentMonthSelection]          INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentMonthSelection] DEFAULT ((0)) NOT NULL,
    [AppointmentDayOfTheMonth]           INT                CONSTRAINT [DF_tblAppointmentEquipment_AppointmentDayOfTheMonth] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                        DATETIMEOFFSET (7) CONSTRAINT [DF_tblAppointmentEquipment_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                          [dbo].[udtUserID]  CONSTRAINT [DF_tblAppointmentEquipment_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                        DATETIMEOFFSET (7) CONSTRAINT [DF_tblAppointmentEquipment_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                          [dbo].[udtUserID]  CONSTRAINT [DF_tblAppointmentEquipment_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]                        ROWVERSION         NOT NULL,
    [_ClusterIdx]                        BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAppointmentEquipment_GUID] PRIMARY KEY NONCLUSTERED ([AppointmentEquipmentGuid] ASC),
    CONSTRAINT [FK_tblAppointmentEquipment_EquipmentGuid] FOREIGN KEY ([EquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid]),
    CONSTRAINT [FK_tblAppointmentEquipment_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblAppointmentEquipment_TestSetDefinitionGuid] FOREIGN KEY ([TestSetDefinitionGuid]) REFERENCES [dbo].[tblTestSetDefinitions] ([TestSetDefinitionGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblAppointmentEquipment_CreatedDate]
    ON [dbo].[tblAppointmentEquipment]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAppointmentEquipment] ON [dbo].[tblAppointmentEquipment] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAppointmentEquipment','D')=1 
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
	INSERT INTO [fmaudit].tblAppointmentEquipment (
		[AppointmentEquipmentGuid]
	,	[EquipmentGuid]
	,	[TestSetDefinitionGuid]
	,	[SiteGuid]
	,	[AssetText]
	,	[AppointmentCategory]
	,	[AppointmentIsSingle]
	,	[ScheduleOnWeekends]
	,	[ScheduleOnHolidays]
	,	[StartDate]
	,	[Duration]
	,	[AppointmentPeriod]
	,	[AppointmentPeriodText]
	,	[Description]
	,	[AppointmentTimeInterval]
	,	[AppointmentDayOfTheWeekText]
	,	[AppointmentDayOfTheWeek]
	,	[AppointmentReoccuranceInterval]
	,	[AppointmentOption2Selected]
	,	[AppointmentTimeOptionSelectionText]
	,	[AppointmentTimeOptionSelection]
	,	[AppointmentMonthSelectionText]
	,	[AppointmentMonthSelection]
	,	[AppointmentDayOfTheMonth]
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
		d.[AppointmentEquipmentGuid]
	,	d.[EquipmentGuid]
	,	d.[TestSetDefinitionGuid]
	,	d.[SiteGuid]
	,	d.[AssetText]
	,	d.[AppointmentCategory]
	,	d.[AppointmentIsSingle]
	,	d.[ScheduleOnWeekends]
	,	d.[ScheduleOnHolidays]
	,	d.[StartDate]
	,	d.[Duration]
	,	d.[AppointmentPeriod]
	,	d.[AppointmentPeriodText]
	,	d.[Description]
	,	d.[AppointmentTimeInterval]
	,	d.[AppointmentDayOfTheWeekText]
	,	d.[AppointmentDayOfTheWeek]
	,	d.[AppointmentReoccuranceInterval]
	,	d.[AppointmentOption2Selected]
	,	d.[AppointmentTimeOptionSelectionText]
	,	d.[AppointmentTimeOptionSelection]
	,	d.[AppointmentMonthSelectionText]
	,	d.[AppointmentMonthSelection]
	,	d.[AppointmentDayOfTheMonth]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAppointmentEquipment] ON [dbo].[tblAppointmentEquipment] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAppointmentEquipment','D')=1 
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
	INSERT INTO [fmaudit].tblAppointmentEquipment (
		[AppointmentEquipmentGuid]
	,	[EquipmentGuid]
	,	[TestSetDefinitionGuid]
	,	[SiteGuid]
	,	[AssetText]
	,	[AppointmentCategory]
	,	[AppointmentIsSingle]
	,	[ScheduleOnWeekends]
	,	[ScheduleOnHolidays]
	,	[StartDate]
	,	[Duration]
	,	[AppointmentPeriod]
	,	[AppointmentPeriodText]
	,	[Description]
	,	[AppointmentTimeInterval]
	,	[AppointmentDayOfTheWeekText]
	,	[AppointmentDayOfTheWeek]
	,	[AppointmentReoccuranceInterval]
	,	[AppointmentOption2Selected]
	,	[AppointmentTimeOptionSelectionText]
	,	[AppointmentTimeOptionSelection]
	,	[AppointmentMonthSelectionText]
	,	[AppointmentMonthSelection]
	,	[AppointmentDayOfTheMonth]
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
		i.[AppointmentEquipmentGuid]
	,	i.[EquipmentGuid]
	,	i.[TestSetDefinitionGuid]
	,	i.[SiteGuid]
	,	i.[AssetText]
	,	i.[AppointmentCategory]
	,	i.[AppointmentIsSingle]
	,	i.[ScheduleOnWeekends]
	,	i.[ScheduleOnHolidays]
	,	i.[StartDate]
	,	i.[Duration]
	,	i.[AppointmentPeriod]
	,	i.[AppointmentPeriodText]
	,	i.[Description]
	,	i.[AppointmentTimeInterval]
	,	i.[AppointmentDayOfTheWeekText]
	,	i.[AppointmentDayOfTheWeek]
	,	i.[AppointmentReoccuranceInterval]
	,	i.[AppointmentOption2Selected]
	,	i.[AppointmentTimeOptionSelectionText]
	,	i.[AppointmentTimeOptionSelection]
	,	i.[AppointmentMonthSelectionText]
	,	i.[AppointmentMonthSelection]
	,	i.[AppointmentDayOfTheMonth]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAppointmentEquipment] ON [dbo].[tblAppointmentEquipment] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAppointmentEquipment','D')=1 
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
	AppointmentEquipmentGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAppointmentEquipment (
		[AppointmentEquipmentGuid]
	,	[EquipmentGuid]
	,	[TestSetDefinitionGuid]
	,	[SiteGuid]
	,	[AssetText]
	,	[AppointmentCategory]
	,	[AppointmentIsSingle]
	,	[ScheduleOnWeekends]
	,	[ScheduleOnHolidays]
	,	[StartDate]
	,	[Duration]
	,	[AppointmentPeriod]
	,	[AppointmentPeriodText]
	,	[Description]
	,	[AppointmentTimeInterval]
	,	[AppointmentDayOfTheWeekText]
	,	[AppointmentDayOfTheWeek]
	,	[AppointmentReoccuranceInterval]
	,	[AppointmentOption2Selected]
	,	[AppointmentTimeOptionSelectionText]
	,	[AppointmentTimeOptionSelection]
	,	[AppointmentMonthSelectionText]
	,	[AppointmentMonthSelection]
	,	[AppointmentDayOfTheMonth]
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
	OUTPUT inserted.[AppointmentEquipmentGuid] AS 'AppointmentEquipmentGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AppointmentEquipmentGuid]
	,	d.[EquipmentGuid]
	,	d.[TestSetDefinitionGuid]
	,	d.[SiteGuid]
	,	d.[AssetText]
	,	d.[AppointmentCategory]
	,	d.[AppointmentIsSingle]
	,	d.[ScheduleOnWeekends]
	,	d.[ScheduleOnHolidays]
	,	d.[StartDate]
	,	d.[Duration]
	,	d.[AppointmentPeriod]
	,	d.[AppointmentPeriodText]
	,	d.[Description]
	,	d.[AppointmentTimeInterval]
	,	d.[AppointmentDayOfTheWeekText]
	,	d.[AppointmentDayOfTheWeek]
	,	d.[AppointmentReoccuranceInterval]
	,	d.[AppointmentOption2Selected]
	,	d.[AppointmentTimeOptionSelectionText]
	,	d.[AppointmentTimeOptionSelection]
	,	d.[AppointmentMonthSelectionText]
	,	d.[AppointmentMonthSelection]
	,	d.[AppointmentDayOfTheMonth]
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
 
	INSERT INTO [fmaudit].tblAppointmentEquipment (
		[AppointmentEquipmentGuid]
	,	[EquipmentGuid]
	,	[TestSetDefinitionGuid]
	,	[SiteGuid]
	,	[AssetText]
	,	[AppointmentCategory]
	,	[AppointmentIsSingle]
	,	[ScheduleOnWeekends]
	,	[ScheduleOnHolidays]
	,	[StartDate]
	,	[Duration]
	,	[AppointmentPeriod]
	,	[AppointmentPeriodText]
	,	[Description]
	,	[AppointmentTimeInterval]
	,	[AppointmentDayOfTheWeekText]
	,	[AppointmentDayOfTheWeek]
	,	[AppointmentReoccuranceInterval]
	,	[AppointmentOption2Selected]
	,	[AppointmentTimeOptionSelectionText]
	,	[AppointmentTimeOptionSelection]
	,	[AppointmentMonthSelectionText]
	,	[AppointmentMonthSelection]
	,	[AppointmentDayOfTheMonth]
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
		i.[AppointmentEquipmentGuid]
	,	i.[EquipmentGuid]
	,	i.[TestSetDefinitionGuid]
	,	i.[SiteGuid]
	,	i.[AssetText]
	,	i.[AppointmentCategory]
	,	i.[AppointmentIsSingle]
	,	i.[ScheduleOnWeekends]
	,	i.[ScheduleOnHolidays]
	,	i.[StartDate]
	,	i.[Duration]
	,	i.[AppointmentPeriod]
	,	i.[AppointmentPeriodText]
	,	i.[Description]
	,	i.[AppointmentTimeInterval]
	,	i.[AppointmentDayOfTheWeekText]
	,	i.[AppointmentDayOfTheWeek]
	,	i.[AppointmentReoccuranceInterval]
	,	i.[AppointmentOption2Selected]
	,	i.[AppointmentTimeOptionSelectionText]
	,	i.[AppointmentTimeOptionSelection]
	,	i.[AppointmentMonthSelectionText]
	,	i.[AppointmentMonthSelection]
	,	i.[AppointmentDayOfTheMonth]
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
			agl.[AppointmentEquipmentGuid]=i.[AppointmentEquipmentGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblAppointmentEquipment
CREATE TRIGGER dbo.trg_insupd_tblAppointmentEquipment_ForSync 
   ON dbo.tblAppointmentEquipment
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
                    ,d.AppointmentEquipmentGuid AS Deleted_PK_AppointmentEquipmentGuid
                    ,i.AppointmentEquipmentGuid AS Inserted_PK_AppointmentEquipmentGuid
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
				    d.AppointmentEquipmentGuid = i.AppointmentEquipmentGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAppointmentEquipment As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AppointmentEquipmentGuid = currentTrackingData.PK_AppointmentEquipmentGuid
 
 
		    INSERT track.tblAppointmentEquipment (InsertedDate 
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
				    ,PK_AppointmentEquipmentGuid
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
				    ,entityChanges.Inserted_PK_AppointmentEquipmentGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAppointmentEquipment As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AppointmentEquipmentGuid = currentTrackingData.PK_AppointmentEquipmentGuid
)
    END
END 

GO
--Creating Delete Trigger for tblAppointmentEquipment
CREATE TRIGGER dbo.trg_del_tblAppointmentEquipment_ForSync 
   ON dbo.tblAppointmentEquipment
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
						,d.AppointmentEquipmentGuid AS Deleted_PK_AppointmentEquipmentGuid
                        ,d.AppointmentEquipmentGuid AS Inserted_PK_AppointmentEquipmentGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAppointmentEquipment As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AppointmentEquipmentGuid = currentTrackingData.PK_AppointmentEquipmentGuid
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
						,PK_AppointmentEquipmentGuid
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
						,entityChanges.Deleted_PK_AppointmentEquipmentGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAppointmentEquipment_ClusterIdx]
    ON [dbo].[tblAppointmentEquipment]([_ClusterIdx] ASC);
