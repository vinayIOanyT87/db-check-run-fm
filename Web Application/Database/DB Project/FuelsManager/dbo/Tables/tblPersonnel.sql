CREATE TABLE [dbo].[tblPersonnel] (
    [PersonID]                NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_PersonID] DEFAULT ('') NOT NULL,
    [CardNumber]              NVARCHAR (30)      NULL,
    [FirstName]               NVARCHAR (20)      CONSTRAINT [DF_tblPersonnel_FirstName] DEFAULT ('') NOT NULL,
    [MiddleName]              NVARCHAR (20)      CONSTRAINT [DF_tblPersonnel_MiddleName] DEFAULT ('') NOT NULL,
    [LastName]                NVARCHAR (30)      CONSTRAINT [DF_tblPersonnel_LastName] DEFAULT ('') NOT NULL,
    [Title]                   NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_Title] DEFAULT ('') NOT NULL,
    [Department]              NVARCHAR (20)      CONSTRAINT [DF_tblPersonnel_Department] DEFAULT ('') NOT NULL,
    [Address1]                NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_Address1] DEFAULT ('') NOT NULL,
    [Address2]                NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_Address2] DEFAULT ('') NOT NULL,
    [City]                    NVARCHAR (60)      CONSTRAINT [DF_tblPersonnel_City] DEFAULT ('') NOT NULL,
    [State]                   NVARCHAR (20)      CONSTRAINT [DF_tblPersonnel_State] DEFAULT ('') NOT NULL,
    [Zip]                     NVARCHAR (10)      CONSTRAINT [DF_tblPersonnel_Zip] DEFAULT ('') NOT NULL,
    [Country]                 NVARCHAR (20)      CONSTRAINT [DF_tblPersonnel_Country] DEFAULT ('') NOT NULL,
    [Phone1]                  NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_Phone1] DEFAULT ('') NOT NULL,
    [Phone2]                  NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_Phone2] DEFAULT ('') NOT NULL,
    [AssignmentDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblPersonnel_AssignmentDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [SupervisionDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblPersonnel_SupervisionDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [SSAN]                    NVARCHAR (11)      CONSTRAINT [DF_tblPersonnel_SSAN] DEFAULT ('') NOT NULL,
    [BirthDate]               DATETIMEOFFSET (7) CONSTRAINT [DF_tblPersonnel_BirthDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [PayRate]                 MONEY              CONSTRAINT [DF_tblPersonnel_PayRate] DEFAULT ((0.0)) NOT NULL,
    [LaborRate1]              FLOAT (53)         CONSTRAINT [DF_tblPersonnel_LaborRate1] DEFAULT ((0.0)) NOT NULL,
    [LaborRate2]              FLOAT (53)         CONSTRAINT [DF_tblPersonnel_LaborRate2] DEFAULT ((0.0)) NOT NULL,
    [LaborRate3]              FLOAT (53)         CONSTRAINT [DF_tblPersonnel_LaborRate3] DEFAULT ((0.0)) NOT NULL,
    [LaborRate4]              FLOAT (53)         CONSTRAINT [DF_tblPersonnel_LaborRate4] DEFAULT ((0.0)) NOT NULL,
    [Status]                  SMALLINT           CONSTRAINT [DF_tblPersonnel_Status] DEFAULT ((0)) NOT NULL,
    [Email]                   NVARCHAR (50)      CONSTRAINT [DF_tblPersonnel_Email] DEFAULT ('') NOT NULL,
    [ResponsibleOfficer]      BIT                CONSTRAINT [DF_tblEmployees_ResponsibleOfficer] DEFAULT ((0)) NOT NULL,
    [Shift]                   SMALLINT           CONSTRAINT [DF_tblPersonnel_Shift] DEFAULT ((0)) NOT NULL,
    [PINNumber]               VARBINARY (256)    NULL,
    [PINRequired]             BIT                CONSTRAINT [DF_tblPersonnel_PINRequired] DEFAULT ((0)) NOT NULL,
    [LockedOut]               BIT                CONSTRAINT [DF_tblPersonnel_LockedOut] DEFAULT ((0)) NOT NULL,
    [LockedOutReason]         NVARCHAR (80)      CONSTRAINT [DF_tblPersonnel_LockedOutReason] DEFAULT ('') NOT NULL,
    [LockedOutDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblPersonnel_LockedOutDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [LastActivityDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblPersonnel_LastActivityDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CardedIn]                BIT                CONSTRAINT [DF_tblPersonnel_CardedIn] DEFAULT ((0)) NOT NULL,
    [ShortCardNumber]         NVARCHAR (6)       NULL,
    [CreatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblEmployees_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblPersonnel_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) CONSTRAINT [DF_tblEmployees_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]               [dbo].[udtUserID]  CONSTRAINT [DF_tblPersonnel_UpdatedBy] DEFAULT ('') NOT NULL,
    [OnFileSignature]         VARBINARY(MAX)     NULL,
    [UserData1]               NVARCHAR (60)      NULL,
    [UserData2]               NVARCHAR (60)      NULL,
    [UserData3]               NVARCHAR (60)      NULL,
    [UserData4]               NVARCHAR (60)      NULL,
    [UserData5]               NVARCHAR (60)      NULL,
    [UserData6]               NVARCHAR (60)      NULL,
    [UserData7]               NVARCHAR (60)      NULL,
    [UserData8]               NVARCHAR (60)      NULL,
    [UserData9]               NVARCHAR (60)      NULL,
    [UserData10]              NVARCHAR (60)      NULL,
    [UserData11]              NVARCHAR (60)      NULL,
    [UserData12]              NVARCHAR (60)      NULL,
    [UserData13]              NVARCHAR (60)      NULL,
    [UserData14]              NVARCHAR (60)      NULL,
    [UserData15]              NVARCHAR (60)      NULL,
    [UserData16]              NVARCHAR (60)      NULL,
    [UserData17]              NVARCHAR (60)      NULL,
    [UserData18]              NVARCHAR (60)      NULL,
    [UserData19]              NVARCHAR (60)      NULL,
    [UserData20]              NVARCHAR (60)      NULL,
    [UserData21]              NVARCHAR (60)      NULL,
    [UserData22]              NVARCHAR (60)      NULL,
    [UserData23]              NVARCHAR (60)      NULL,
    [UserData24]              NVARCHAR (60)      NULL,
	[InhibitInactivityLockout] BIT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [PersonnelGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblPersonnel_GUID] DEFAULT (newid()) NOT NULL,
    [SiteGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [CompanyGuid]             UNIQUEIDENTIFIER   NULL,
    [SupervisorPersonnelGuid] UNIQUEIDENTIFIER   NULL,
    [UserGuid]                UNIQUEIDENTIFIER   NULL,
    [AssignedEquipmentGuid]   UNIQUEIDENTIFIER   NULL,	
    [_MasterRecordGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [HiddenDate]              DATETIMEOFFSET (7) NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblPersonnel_GUID] PRIMARY KEY NONCLUSTERED ([PersonnelGuid] ASC),
    CONSTRAINT [CK_tblPersonnel_CannotBeOwnSupervisor] CHECK ([PersonnelGuid]<>[SupervisorPersonnelGuid]),
    CONSTRAINT [CK_tblPersonnel_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessPerson]([_MasterRecordGuid],[SiteGuid],[PersonID],[CardNumber],[ShortCardNumber])=(1)),
    CONSTRAINT [FK_tblPersonnel_AssignedEquipmentGuid] FOREIGN KEY ([AssignedEquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid]),
    CONSTRAINT [FK_tblPersonnel_CompanyGuid] FOREIGN KEY ([CompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblPersonnel_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblPersonnel_SupervisorPersonnelGuid] FOREIGN KEY ([SupervisorPersonnelGuid]) REFERENCES [dbo].[tblPersonnel] ([PersonnelGuid]),
    CONSTRAINT [FK_tblPersonnel_UserGuid] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers] ([UserGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblPersonnel_CreatedDate]
    ON [dbo].[tblPersonnel]([CreatedDate] ASC);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblPersonnel_PersonID_SiteGuid]
    ON [dbo].[tblPersonnel]([PersonID] ASC, [SiteGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblPersonnel_SiteGuid_MasterRecordGuid]
    ON [dbo].[tblPersonnel]([SiteGuid] ASC, [_MasterRecordGuid] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblPersonnel] ON [dbo].[tblPersonnel] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPersonnel','D')=1 
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
	PersonnelGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblPersonnel (
		[PersonID]
	,	[CardNumber]
	,	[FirstName]
	,	[MiddleName]
	,	[LastName]
	,	[Title]
	,	[Department]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone1]
	,	[Phone2]
	,	[AssignmentDate]
	,	[SupervisionDate]
	,	[SSAN]
	,	[BirthDate]
	,	[PayRate]
	,	[LaborRate1]
	,	[LaborRate2]
	,	[LaborRate3]
	,	[LaborRate4]
	,	[Status]
	,	[Email]
	,	[ResponsibleOfficer]
	,	[Shift]
	,	[PINNumber]
	,	[PINRequired]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[LastActivityDate]
	,	[CardedIn]
	,	[ShortCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OnFileSignature]
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
	,	[InhibitInactivityLockout]
	,	[OriginalRowVersion]
	,	[PersonnelGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[SupervisorPersonnelGuid]
	,	[UserGuid]
	,	[AssignedEquipmentGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
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
	OUTPUT inserted.[PersonnelGuid] AS 'PersonnelGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[PersonID]
	,	d.[CardNumber]
	,	d.[FirstName]
	,	d.[MiddleName]
	,	d.[LastName]
	,	d.[Title]
	,	d.[Department]
	,	d.[Address1]
	,	d.[Address2]
	,	d.[City]
	,	d.[State]
	,	d.[Zip]
	,	d.[Country]
	,	d.[Phone1]
	,	d.[Phone2]
	,	d.[AssignmentDate]
	,	d.[SupervisionDate]
	,	d.[SSAN]
	,	d.[BirthDate]
	,	d.[PayRate]
	,	d.[LaborRate1]
	,	d.[LaborRate2]
	,	d.[LaborRate3]
	,	d.[LaborRate4]
	,	d.[Status]
	,	d.[Email]
	,	d.[ResponsibleOfficer]
	,	d.[Shift]
	,	d.[PINNumber]
	,	d.[PINRequired]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[LastActivityDate]
	,	d.[CardedIn]
	,	d.[ShortCardNumber]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[OnFileSignature]
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
	,	d.[InhibitInactivityLockout]
	,	d.[_RowVersion]
	,	d.[PersonnelGuid]
	,	d.[SiteGuid]
	,	d.[CompanyGuid]
	,	d.[SupervisorPersonnelGuid]
	,	d.[UserGuid]
	,	d.[AssignedEquipmentGuid]
	,	d.[_MasterRecordGuid]
	,	d.[HiddenDate]
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
 
	INSERT INTO [fmaudit].tblPersonnel (
		[PersonID]
	,	[CardNumber]
	,	[FirstName]
	,	[MiddleName]
	,	[LastName]
	,	[Title]
	,	[Department]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone1]
	,	[Phone2]
	,	[AssignmentDate]
	,	[SupervisionDate]
	,	[SSAN]
	,	[BirthDate]
	,	[PayRate]
	,	[LaborRate1]
	,	[LaborRate2]
	,	[LaborRate3]
	,	[LaborRate4]
	,	[Status]
	,	[Email]
	,	[ResponsibleOfficer]
	,	[Shift]
	,	[PINNumber]
	,	[PINRequired]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[LastActivityDate]
	,	[CardedIn]
	,	[ShortCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OnFileSignature]
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
	,	[InhibitInactivityLockout]
	,	[OriginalRowVersion]
	,	[PersonnelGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[SupervisorPersonnelGuid]
	,	[UserGuid]
	,	[AssignedEquipmentGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
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
		i.[PersonID]
	,	i.[CardNumber]
	,	i.[FirstName]
	,	i.[MiddleName]
	,	i.[LastName]
	,	i.[Title]
	,	i.[Department]
	,	i.[Address1]
	,	i.[Address2]
	,	i.[City]
	,	i.[State]
	,	i.[Zip]
	,	i.[Country]
	,	i.[Phone1]
	,	i.[Phone2]
	,	i.[AssignmentDate]
	,	i.[SupervisionDate]
	,	i.[SSAN]
	,	i.[BirthDate]
	,	i.[PayRate]
	,	i.[LaborRate1]
	,	i.[LaborRate2]
	,	i.[LaborRate3]
	,	i.[LaborRate4]
	,	i.[Status]
	,	i.[Email]
	,	i.[ResponsibleOfficer]
	,	i.[Shift]
	,	i.[PINNumber]
	,	i.[PINRequired]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[LastActivityDate]
	,	i.[CardedIn]
	,	i.[ShortCardNumber]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[OnFileSignature]
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
	,	i.[InhibitInactivityLockout]
	,	i.[_RowVersion]
	,	i.[PersonnelGuid]
	,	i.[SiteGuid]
	,	i.[CompanyGuid]
	,	i.[SupervisorPersonnelGuid]
	,	i.[UserGuid]
	,	i.[AssignedEquipmentGuid]
	,	i.[_MasterRecordGuid]
	,	i.[HiddenDate]
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
			agl.[PersonnelGuid]=i.[PersonnelGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblPersonnel
CREATE TRIGGER dbo.trg_insupd_tblPersonnel_ForSync 
   ON dbo.tblPersonnel
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
                    ,d.PersonnelGuid AS Deleted_PK_PersonnelGuid
                    ,i.PersonnelGuid AS Inserted_PK_PersonnelGuid
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
				    d.PersonnelGuid = i.PersonnelGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblPersonnel As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_PersonnelGuid = currentTrackingData.PK_PersonnelGuid
 
 
		    INSERT track.tblPersonnel (InsertedDate 
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
				    ,PK_PersonnelGuid
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
				    ,entityChanges.Inserted_PK_PersonnelGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblPersonnel As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_PersonnelGuid = currentTrackingData.PK_PersonnelGuid
)
    END
END 

GO
--Creating Delete Trigger for tblPersonnel
CREATE TRIGGER dbo.trg_del_tblPersonnel_ForSync 
   ON dbo.tblPersonnel
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
						,d.PersonnelGuid AS Deleted_PK_PersonnelGuid
                        ,d.PersonnelGuid AS Inserted_PK_PersonnelGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblPersonnel As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_PersonnelGuid = currentTrackingData.PK_PersonnelGuid
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
						,PK_PersonnelGuid
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
						,entityChanges.Deleted_PK_PersonnelGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblPersonnel] ON [dbo].[tblPersonnel] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPersonnel','D')=1 
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
	INSERT INTO [fmaudit].tblPersonnel (
		[PersonID]
	,	[CardNumber]
	,	[FirstName]
	,	[MiddleName]
	,	[LastName]
	,	[Title]
	,	[Department]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone1]
	,	[Phone2]
	,	[AssignmentDate]
	,	[SupervisionDate]
	,	[SSAN]
	,	[BirthDate]
	,	[PayRate]
	,	[LaborRate1]
	,	[LaborRate2]
	,	[LaborRate3]
	,	[LaborRate4]
	,	[Status]
	,	[Email]
	,	[ResponsibleOfficer]
	,	[Shift]
	,	[PINNumber]
	,	[PINRequired]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[LastActivityDate]
	,	[CardedIn]
	,	[ShortCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OnFileSignature]
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
	,	[InhibitInactivityLockout]
	,	[OriginalRowVersion]
	,	[PersonnelGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[SupervisorPersonnelGuid]
	,	[UserGuid]
	,	[AssignedEquipmentGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
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
		d.[PersonID]
	,	d.[CardNumber]
	,	d.[FirstName]
	,	d.[MiddleName]
	,	d.[LastName]
	,	d.[Title]
	,	d.[Department]
	,	d.[Address1]
	,	d.[Address2]
	,	d.[City]
	,	d.[State]
	,	d.[Zip]
	,	d.[Country]
	,	d.[Phone1]
	,	d.[Phone2]
	,	d.[AssignmentDate]
	,	d.[SupervisionDate]
	,	d.[SSAN]
	,	d.[BirthDate]
	,	d.[PayRate]
	,	d.[LaborRate1]
	,	d.[LaborRate2]
	,	d.[LaborRate3]
	,	d.[LaborRate4]
	,	d.[Status]
	,	d.[Email]
	,	d.[ResponsibleOfficer]
	,	d.[Shift]
	,	d.[PINNumber]
	,	d.[PINRequired]
	,	d.[LockedOut]
	,	d.[LockedOutReason]
	,	d.[LockedOutDate]
	,	d.[LastActivityDate]
	,	d.[CardedIn]
	,	d.[ShortCardNumber]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[OnFileSignature]
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
	,	d.[InhibitInactivityLockout]
	,	d.[_RowVersion]
	,	d.[PersonnelGuid]
	,	d.[SiteGuid]
	,	d.[CompanyGuid]
	,	d.[SupervisorPersonnelGuid]
	,	d.[UserGuid]
	,	d.[AssignedEquipmentGuid]
	,	d.[_MasterRecordGuid]
	,	d.[HiddenDate]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblPersonnel] ON [dbo].[tblPersonnel] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPersonnel','D')=1 
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
	INSERT INTO [fmaudit].tblPersonnel (
		[PersonID]
	,	[CardNumber]
	,	[FirstName]
	,	[MiddleName]
	,	[LastName]
	,	[Title]
	,	[Department]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[Country]
	,	[Phone1]
	,	[Phone2]
	,	[AssignmentDate]
	,	[SupervisionDate]
	,	[SSAN]
	,	[BirthDate]
	,	[PayRate]
	,	[LaborRate1]
	,	[LaborRate2]
	,	[LaborRate3]
	,	[LaborRate4]
	,	[Status]
	,	[Email]
	,	[ResponsibleOfficer]
	,	[Shift]
	,	[PINNumber]
	,	[PINRequired]
	,	[LockedOut]
	,	[LockedOutReason]
	,	[LockedOutDate]
	,	[LastActivityDate]
	,	[CardedIn]
	,	[ShortCardNumber]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OnFileSignature]
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
	,	[InhibitInactivityLockout]
	,	[OriginalRowVersion]
	,	[PersonnelGuid]
	,	[SiteGuid]
	,	[CompanyGuid]
	,	[SupervisorPersonnelGuid]
	,	[UserGuid]
	,	[AssignedEquipmentGuid]
	,	[_MasterRecordGuid]
	,	[HiddenDate]
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
		i.[PersonID]
	,	i.[CardNumber]
	,	i.[FirstName]
	,	i.[MiddleName]
	,	i.[LastName]
	,	i.[Title]
	,	i.[Department]
	,	i.[Address1]
	,	i.[Address2]
	,	i.[City]
	,	i.[State]
	,	i.[Zip]
	,	i.[Country]
	,	i.[Phone1]
	,	i.[Phone2]
	,	i.[AssignmentDate]
	,	i.[SupervisionDate]
	,	i.[SSAN]
	,	i.[BirthDate]
	,	i.[PayRate]
	,	i.[LaborRate1]
	,	i.[LaborRate2]
	,	i.[LaborRate3]
	,	i.[LaborRate4]
	,	i.[Status]
	,	i.[Email]
	,	i.[ResponsibleOfficer]
	,	i.[Shift]
	,	i.[PINNumber]
	,	i.[PINRequired]
	,	i.[LockedOut]
	,	i.[LockedOutReason]
	,	i.[LockedOutDate]
	,	i.[LastActivityDate]
	,	i.[CardedIn]
	,	i.[ShortCardNumber]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[OnFileSignature]
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
	,	i.[InhibitInactivityLockout]
	,	i.[_RowVersion]
	,	i.[PersonnelGuid]
	,	i.[SiteGuid]
	,	i.[CompanyGuid]
	,	i.[SupervisorPersonnelGuid]
	,	i.[UserGuid]
	,	i.[AssignedEquipmentGuid]
	,	i.[_MasterRecordGuid]
	,	i.[HiddenDate]
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


CREATE TRIGGER [dbo].[trg_fmcdc_tblPersonnel]
ON [dbo].[tblPersonnel]
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
		INSERT INTO fmcdc.[tblPersonnel]
		(
		[PersonID]
		, [CardNumber]
		, [FirstName]
		, [MiddleName]
		, [LastName]
		, [Title]
		, [Department]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone1]
		, [Phone2]
		, [AssignmentDate]
		, [SupervisionDate]
		, [SSAN]
		, [BirthDate]
		, [PayRate]
		, [LaborRate1]
		, [LaborRate2]
		, [LaborRate3]
		, [LaborRate4]
		, [Status]
		, [Email]
		, [ResponsibleOfficer]
		, [Shift]
		, [PINNumber]
		, [PINRequired]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [LastActivityDate]
		, [CardedIn]
		, [ShortCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [OnFileSignature]
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
		, [InhibitInactivityLockout]
		, [SourceRowVersion]
		, [PersonnelGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [SupervisorPersonnelGuid]
		, [UserGuid]
		, [AssignedEquipmentGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[PersonID]
		, [CardNumber]
		, [FirstName]
		, [MiddleName]
		, [LastName]
		, [Title]
		, [Department]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone1]
		, [Phone2]
		, [AssignmentDate]
		, [SupervisionDate]
		, [SSAN]
		, [BirthDate]
		, [PayRate]
		, [LaborRate1]
		, [LaborRate2]
		, [LaborRate3]
		, [LaborRate4]
		, [Status]
		, [Email]
		, [ResponsibleOfficer]
		, [Shift]
		, [PINNumber]
		, [PINRequired]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [LastActivityDate]
		, [CardedIn]
		, [ShortCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [OnFileSignature]
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
		, [InhibitInactivityLockout]
		, CONVERT(bigint, _RowVersion)
		, [PersonnelGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [SupervisorPersonnelGuid]
		, [UserGuid]
		, [AssignedEquipmentGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblPersonnel]
		(
		[PersonID]
		, [CardNumber]
		, [FirstName]
		, [MiddleName]
		, [LastName]
		, [Title]
		, [Department]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone1]
		, [Phone2]
		, [AssignmentDate]
		, [SupervisionDate]
		, [SSAN]
		, [BirthDate]
		, [PayRate]
		, [LaborRate1]
		, [LaborRate2]
		, [LaborRate3]
		, [LaborRate4]
		, [Status]
		, [Email]
		, [ResponsibleOfficer]
		, [Shift]
		, [PINNumber]
		, [PINRequired]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [LastActivityDate]
		, [CardedIn]
		, [ShortCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [OnFileSignature]
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
		, [InhibitInactivityLockout]
		, [SourceRowVersion]
		, [PersonnelGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [SupervisorPersonnelGuid]
		, [UserGuid]
		, [AssignedEquipmentGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[PersonID]
		, [CardNumber]
		, [FirstName]
		, [MiddleName]
		, [LastName]
		, [Title]
		, [Department]
		, [Address1]
		, [Address2]
		, [City]
		, [State]
		, [Zip]
		, [Country]
		, [Phone1]
		, [Phone2]
		, [AssignmentDate]
		, [SupervisionDate]
		, [SSAN]
		, [BirthDate]
		, [PayRate]
		, [LaborRate1]
		, [LaborRate2]
		, [LaborRate3]
		, [LaborRate4]
		, [Status]
		, [Email]
		, [ResponsibleOfficer]
		, [Shift]
		, [PINNumber]
		, [PINRequired]
		, [LockedOut]
		, [LockedOutReason]
		, [LockedOutDate]
		, [LastActivityDate]
		, [CardedIn]
		, [ShortCardNumber]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [OnFileSignature]
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
		, [InhibitInactivityLockout]
		, CONVERT(bigint, _RowVersion)
		, [PersonnelGuid]
		, [SiteGuid]
		, [CompanyGuid]
		, [SupervisorPersonnelGuid]
		, [UserGuid]
		, [AssignedEquipmentGuid]
		, [_MasterRecordGuid]
		, [HiddenDate]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblPersonnel] ON [dbo].[tblPersonnel]
GO



CREATE NONCLUSTERED INDEX IX_tblPersonnel_CompanyGuid ON [dbo].[tblPersonnel]([CompanyGuid])
GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblPersonnel_PersonnelGuid_IncludeBasicInformation] ON [dbo].[tblPersonnel] (PersonnelGuid) INCLUDE (_MasterRecordGuid, SiteGuid, PersonID)

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPersonnel_ClusterIdx]
    ON [dbo].[tblPersonnel]([_ClusterIdx] ASC);

