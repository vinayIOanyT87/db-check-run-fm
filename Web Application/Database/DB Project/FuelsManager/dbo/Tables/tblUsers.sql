CREATE TABLE [dbo].[tblUsers] (
    [UserID]                [dbo].[udtUserID]  CONSTRAINT [DF_tblUsers_UserID] DEFAULT ('') NOT NULL,
    [Password]              VARBINARY (256)    NOT NULL,
    [LastLoginDate]         DATETIMEOFFSET (7) CONSTRAINT [DF_tblUsers_LastLoginDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [LastLogoffDate]        DATETIMEOFFSET (7) CONSTRAINT [DF_tblUsers_LastLogoffDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [ChangePassword]        BIT                CONSTRAINT [DF_tblUsers_ChangePassword] DEFAULT ((0)) NOT NULL,
    [PasswordTimeStamp]     DATETIMEOFFSET (7) NOT NULL,
    [Name]                  NVARCHAR (50)      CONSTRAINT [DF_tblUsers_Name] DEFAULT ('') NOT NULL,
    [EmailAddress]          NVARCHAR (50)      NULL,
    [CreatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblUsers_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblUsers_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]           DATETIMEOFFSET (7) CONSTRAINT [DF_tblUsers_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]             [dbo].[udtUserID]  CONSTRAINT [DF_tblUsers_UpdatedBy] DEFAULT ('') NOT NULL,
    [PasswordHistory1]      VARBINARY (256)    NULL,
    [PasswordHistory2]      VARBINARY (256)    NULL,
    [PasswordHistory3]      VARBINARY (256)    NULL,
    [PasswordHistory4]      VARBINARY (256)    NULL,
    [PasswordHistory5]      VARBINARY (256)    NULL,
    [PasswordHistory6]      VARBINARY (256)    NULL,
    [PasswordHistory7]      VARBINARY (256)    NULL,
    [PasswordHistory8]      VARBINARY (256)    NULL,
    [PasswordHistory9]      VARBINARY (256)    NULL,
    [PasswordHistory10]     VARBINARY (256)    NULL,
    [PasswordHistory11]     VARBINARY (256)    NULL,
    [PasswordHistory12]     VARBINARY (256)    NULL,
    [PasswordHistory13]     VARBINARY (256)    NULL,
    [PasswordHistory14]     VARBINARY (256)    NULL,
    [PasswordHistory15]     VARBINARY (256)    NULL,
    [PasswordHistory16]     VARBINARY (256)    NULL,
    [PasswordHistory17]     VARBINARY (256)    NULL,
    [PasswordHistory18]     VARBINARY (256)    NULL,
    [PasswordHistory19]     VARBINARY (256)    NULL,
    [PasswordHistory20]     VARBINARY (256)    NULL,
    [PasswordHistory21]     VARBINARY (256)    NULL,
    [PasswordHistory22]     VARBINARY (256)    NULL,
    [PasswordHistory23]     VARBINARY (256)    NULL,
    [PasswordHistory24]     VARBINARY (256)    NULL,
    [PasswordLockoutCount]  INT                NULL,
    [InactivityLockout]     BIT                NULL,
    [InactivityLockoutDate] DATETIMEOFFSET (7) NULL,
    [UserGuid]              UNIQUEIDENTIFIER   CONSTRAINT [DF_tblUsers_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]           ROWVERSION         NOT NULL,
    [SiteGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [PasswordHint]          VARCHAR (80)       CONSTRAINT [DF_tblUsers_PasswordHint] DEFAULT ('No hint available') NULL,
    [UserData1]				NVARCHAR (120)      NULL,
    [UserData2]				NVARCHAR (120)      NULL,
    [UserData3]				NVARCHAR (120)      NULL,
    [UserData4]				NVARCHAR (120)      NULL,
    [UserData5]				NVARCHAR (120)      NULL,
    [UserData6]				NVARCHAR (120)      NULL,
    [UserData7]				NVARCHAR (120)      NULL,
    [UserData8]				NVARCHAR (120)      NULL,
    [PhoneNumber]           NVARCHAR (20)      NULL,
    [AccountExpirationDate] DATETIME           CONSTRAINT [DF_tblUsers_AccountExpirationDate] DEFAULT (CONVERT(DATE, DATEADD(year, 1, GETDATE()))) NOT NULL,
	[ActiveDirectoryUser]   BIT                 NULL,
    [_ClusterIdx]           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblUsers_GUID] PRIMARY KEY NONCLUSTERED ([UserGuid] ASC),
    CONSTRAINT [CK_tblUsers_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessUser]([UserGuid],[SiteGuid],[UserID])=(1)),
    CONSTRAINT [FK_tblUsers_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblUsers_CreatedDate]
    ON [dbo].[tblUsers]([CreatedDate] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblUsers_UserID_SiteGuid]
    ON [dbo].[tblUsers]([UserID] ASC, [SiteGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblUsers_UserGuid]
    ON [dbo].[tblUsers]([UserGuid] ASC)
    INCLUDE([UserID]);
GO
CREATE INDEX [IX_tblUsers_InactivityLockout_SiteGuid] ON [dbo].[tblUsers] 
([InactivityLockout], [SiteGuid]) INCLUDE ([UserID], [PasswordTimeStamp], [InactivityLockoutDate], [UserGuid])
GO

CREATE INDEX [IX_tblUsers_InactivityLockout_UserID] ON [dbo].[tblUsers] 
([InactivityLockout],[UserID]) INCLUDE ([PasswordTimeStamp], [InactivityLockoutDate], [UserGuid], [SiteGuid])


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblUsers] ON [dbo].[tblUsers] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblUsers','D')=1 
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
	INSERT INTO [fmaudit].tblUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[UserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
	,   [ActiveDirectoryUser]
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
		d.[UserID]
	,	d.[Password]
	,	d.[LastLoginDate]
	,	d.[LastLogoffDate]
	,	d.[ChangePassword]
	,	d.[PasswordTimeStamp]
	,	d.[Name]
	,	d.[EmailAddress]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[PasswordHistory1]
	,	d.[PasswordHistory2]
	,	d.[PasswordHistory3]
	,	d.[PasswordHistory4]
	,	d.[PasswordHistory5]
	,	d.[PasswordHistory6]
	,	d.[PasswordHistory7]
	,	d.[PasswordHistory8]
	,	d.[PasswordHistory9]
	,	d.[PasswordHistory10]
	,	d.[PasswordHistory11]
	,	d.[PasswordHistory12]
	,	d.[PasswordHistory13]
	,	d.[PasswordHistory14]
	,	d.[PasswordHistory15]
	,	d.[PasswordHistory16]
	,	d.[PasswordHistory17]
	,	d.[PasswordHistory18]
	,	d.[PasswordHistory19]
	,	d.[PasswordHistory20]
	,	d.[PasswordHistory21]
	,	d.[PasswordHistory22]
	,	d.[PasswordHistory23]
	,	d.[PasswordHistory24]
	,	d.[PasswordLockoutCount]
	,	d.[InactivityLockout]
	,	d.[InactivityLockoutDate]
	,	d.[UserGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[PasswordHint]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[PhoneNumber]
	,	d.[AccountExpirationDate]
	,   d.[ActiveDirectoryUser]
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



-- =============================================
-- Author:		Chris Knight
-- Create date: 3/30/2010
-- Modified date: 05/01/24 - Srini
-- Description:	Insure that the Inactivity Lockout Date
--              gets set when an account is deactivated by
--              any means.
-- 05/01/24
-- Modified to only call for Update locks when necessary to avoid deadlocks
-- =============================================
CREATE TRIGGER [dbo].[trg_tblUsers_IU_UpdateInactivityLockoutDate]
   ON  [dbo].[tblUsers] 
   AFTER INSERT,UPDATE
AS 
BEGIN
	DROP TABLE IF EXISTS  #tblUsersTemp

	CREATE TABLE #tblUsersTemp (UserGuid uniqueidentifier)

	INSERT INTO #tblUsersTemp(UserGuid)
	(
	SELECT I.UserGuid
	FROM Inserted I LEFT OUTER JOIN Deleted D ON I.UserGuid = D.UserGuid
	WHERE I.InactivityLockout = 1 AND (D.InactivityLockout = 0 OR D.InactivityLockout IS NULL)
	);
	
	IF (SELECT COUNT(*) FROM #tblUsersTemp) > 0
	BEGIN
		UPDATE tblUsers SET InactivityLockoutDate = SYSDATETIMEOFFSET() 
		WHERE UserGuid IN (SELECT UserGuid FROM #tblUsersTemp)
	END

	DELETE FROM #tblUsersTemp;

	INSERT INTO #tblUsersTemp(UserGuid)
	(
	SELECT I.UserGuid
	FROM Inserted I LEFT OUTER JOIN Deleted D ON I.UserGuid = D.UserGuid
	WHERE I.InactivityLockout = 0 AND D.InactivityLockoutDate IS NOT NULL
	);

	IF (SELECT COUNT(*) FROM #tblUsersTemp) > 0
	BEGIN
		UPDATE tblUsers SET InactivityLockoutDate = NULL 
		WHERE UserGuid IN (SELECT UserGuid FROM #tblUsersTemp)
	END
END
GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblUsers] ON [dbo].[tblUsers] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblUsers','D')=1 
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
	INSERT INTO [fmaudit].tblUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[UserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
	,   [ActiveDirectoryUser]
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
		i.[UserID]
	,	i.[Password]
	,	i.[LastLoginDate]
	,	i.[LastLogoffDate]
	,	i.[ChangePassword]
	,	i.[PasswordTimeStamp]
	,	i.[Name]
	,	i.[EmailAddress]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[PasswordHistory1]
	,	i.[PasswordHistory2]
	,	i.[PasswordHistory3]
	,	i.[PasswordHistory4]
	,	i.[PasswordHistory5]
	,	i.[PasswordHistory6]
	,	i.[PasswordHistory7]
	,	i.[PasswordHistory8]
	,	i.[PasswordHistory9]
	,	i.[PasswordHistory10]
	,	i.[PasswordHistory11]
	,	i.[PasswordHistory12]
	,	i.[PasswordHistory13]
	,	i.[PasswordHistory14]
	,	i.[PasswordHistory15]
	,	i.[PasswordHistory16]
	,	i.[PasswordHistory17]
	,	i.[PasswordHistory18]
	,	i.[PasswordHistory19]
	,	i.[PasswordHistory20]
	,	i.[PasswordHistory21]
	,	i.[PasswordHistory22]
	,	i.[PasswordHistory23]
	,	i.[PasswordHistory24]
	,	i.[PasswordLockoutCount]
	,	i.[InactivityLockout]
	,	i.[InactivityLockoutDate]
	,	i.[UserGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[PasswordHint]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[PhoneNumber]
	,	i.[AccountExpirationDate]
	,   i.[ActiveDirectoryUser]
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
--Creating Insert / Update Trigger for tblUsers
CREATE TRIGGER dbo.trg_insupd_tblUsers_ForSync 
   ON dbo.tblUsers
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
                    ,d.UserGuid AS Deleted_PK_UserGuid
                    ,i.UserGuid AS Inserted_PK_UserGuid
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
				    d.UserGuid = i.UserGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblUsers As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_UserGuid = currentTrackingData.PK_UserGuid
 
 
		    INSERT track.tblUsers (InsertedDate 
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
				    ,PK_UserGuid
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
				    ,entityChanges.Inserted_PK_UserGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblUsers As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_UserGuid = currentTrackingData.PK_UserGuid
)
    END
END 

GO
--Creating Delete Trigger for tblUsers
CREATE TRIGGER dbo.trg_del_tblUsers_ForSync 
   ON dbo.tblUsers
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
						,d.UserGuid AS Deleted_PK_UserGuid
                        ,d.UserGuid AS Inserted_PK_UserGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblUsers As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_UserGuid = currentTrackingData.PK_UserGuid
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
						,PK_UserGuid
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
						,entityChanges.Deleted_PK_UserGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblUsers] ON [dbo].[tblUsers] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblUsers','D')=1 
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
	UserGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[UserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
	,   [ActiveDirectoryUser]
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
	OUTPUT inserted.[UserGuid] AS 'UserGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[UserID]
	,	d.[Password]
	,	d.[LastLoginDate]
	,	d.[LastLogoffDate]
	,	d.[ChangePassword]
	,	d.[PasswordTimeStamp]
	,	d.[Name]
	,	d.[EmailAddress]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[PasswordHistory1]
	,	d.[PasswordHistory2]
	,	d.[PasswordHistory3]
	,	d.[PasswordHistory4]
	,	d.[PasswordHistory5]
	,	d.[PasswordHistory6]
	,	d.[PasswordHistory7]
	,	d.[PasswordHistory8]
	,	d.[PasswordHistory9]
	,	d.[PasswordHistory10]
	,	d.[PasswordHistory11]
	,	d.[PasswordHistory12]
	,	d.[PasswordHistory13]
	,	d.[PasswordHistory14]
	,	d.[PasswordHistory15]
	,	d.[PasswordHistory16]
	,	d.[PasswordHistory17]
	,	d.[PasswordHistory18]
	,	d.[PasswordHistory19]
	,	d.[PasswordHistory20]
	,	d.[PasswordHistory21]
	,	d.[PasswordHistory22]
	,	d.[PasswordHistory23]
	,	d.[PasswordHistory24]
	,	d.[PasswordLockoutCount]
	,	d.[InactivityLockout]
	,	d.[InactivityLockoutDate]
	,	d.[UserGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[PasswordHint]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[PhoneNumber]
	,	d.[AccountExpirationDate]
	,   d.[ActiveDirectoryUser]
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
 
	INSERT INTO [fmaudit].tblUsers (
		[UserID]
	,	[Password]
	,	[LastLoginDate]
	,	[LastLogoffDate]
	,	[ChangePassword]
	,	[PasswordTimeStamp]
	,	[Name]
	,	[EmailAddress]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[PasswordHistory1]
	,	[PasswordHistory2]
	,	[PasswordHistory3]
	,	[PasswordHistory4]
	,	[PasswordHistory5]
	,	[PasswordHistory6]
	,	[PasswordHistory7]
	,	[PasswordHistory8]
	,	[PasswordHistory9]
	,	[PasswordHistory10]
	,	[PasswordHistory11]
	,	[PasswordHistory12]
	,	[PasswordHistory13]
	,	[PasswordHistory14]
	,	[PasswordHistory15]
	,	[PasswordHistory16]
	,	[PasswordHistory17]
	,	[PasswordHistory18]
	,	[PasswordHistory19]
	,	[PasswordHistory20]
	,	[PasswordHistory21]
	,	[PasswordHistory22]
	,	[PasswordHistory23]
	,	[PasswordHistory24]
	,	[PasswordLockoutCount]
	,	[InactivityLockout]
	,	[InactivityLockoutDate]
	,	[UserGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[PasswordHint]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[PhoneNumber]
	,	[AccountExpirationDate]
	,   [ActiveDirectoryUser]
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
		i.[UserID]
	,	i.[Password]
	,	i.[LastLoginDate]
	,	i.[LastLogoffDate]
	,	i.[ChangePassword]
	,	i.[PasswordTimeStamp]
	,	i.[Name]
	,	i.[EmailAddress]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[PasswordHistory1]
	,	i.[PasswordHistory2]
	,	i.[PasswordHistory3]
	,	i.[PasswordHistory4]
	,	i.[PasswordHistory5]
	,	i.[PasswordHistory6]
	,	i.[PasswordHistory7]
	,	i.[PasswordHistory8]
	,	i.[PasswordHistory9]
	,	i.[PasswordHistory10]
	,	i.[PasswordHistory11]
	,	i.[PasswordHistory12]
	,	i.[PasswordHistory13]
	,	i.[PasswordHistory14]
	,	i.[PasswordHistory15]
	,	i.[PasswordHistory16]
	,	i.[PasswordHistory17]
	,	i.[PasswordHistory18]
	,	i.[PasswordHistory19]
	,	i.[PasswordHistory20]
	,	i.[PasswordHistory21]
	,	i.[PasswordHistory22]
	,	i.[PasswordHistory23]
	,	i.[PasswordHistory24]
	,	i.[PasswordLockoutCount]
	,	i.[InactivityLockout]
	,	i.[InactivityLockoutDate]
	,	i.[UserGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[PasswordHint]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[PhoneNumber]
	,	i.[AccountExpirationDate]
	,   i.[ActiveDirectoryUser]
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
			agl.[UserGuid]=i.[UserGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblUsers]
ON [dbo].[tblUsers]
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
		INSERT INTO fmcdc.[tblUsers]
		(
		[UserID]
		, [Password]
		, [LastLoginDate]
		, [LastLogoffDate]
		, [ChangePassword]
		, [PasswordTimeStamp]
		, [Name]
		, [EmailAddress]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [PasswordHistory1]
		, [PasswordHistory2]
		, [PasswordHistory3]
		, [PasswordHistory4]
		, [PasswordHistory5]
		, [PasswordHistory6]
		, [PasswordHistory7]
		, [PasswordHistory8]
		, [PasswordHistory9]
		, [PasswordHistory10]
		, [PasswordHistory11]
		, [PasswordHistory12]
		, [PasswordHistory13]
		, [PasswordHistory14]
		, [PasswordHistory15]
		, [PasswordHistory16]
		, [PasswordHistory17]
		, [PasswordHistory18]
		, [PasswordHistory19]
		, [PasswordHistory20]
		, [PasswordHistory21]
		, [PasswordHistory22]
		, [PasswordHistory23]
		, [PasswordHistory24]
		, [PasswordLockoutCount]
		, [InactivityLockout]
		, [InactivityLockoutDate]
		, [UserGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [PasswordHint]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [PhoneNumber]
		, [AccountExpirationDate]
		, [ActiveDirectoryUser]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[UserID]
		, [Password]
		, [LastLoginDate]
		, [LastLogoffDate]
		, [ChangePassword]
		, [PasswordTimeStamp]
		, [Name]
		, [EmailAddress]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [PasswordHistory1]
		, [PasswordHistory2]
		, [PasswordHistory3]
		, [PasswordHistory4]
		, [PasswordHistory5]
		, [PasswordHistory6]
		, [PasswordHistory7]
		, [PasswordHistory8]
		, [PasswordHistory9]
		, [PasswordHistory10]
		, [PasswordHistory11]
		, [PasswordHistory12]
		, [PasswordHistory13]
		, [PasswordHistory14]
		, [PasswordHistory15]
		, [PasswordHistory16]
		, [PasswordHistory17]
		, [PasswordHistory18]
		, [PasswordHistory19]
		, [PasswordHistory20]
		, [PasswordHistory21]
		, [PasswordHistory22]
		, [PasswordHistory23]
		, [PasswordHistory24]
		, [PasswordLockoutCount]
		, [InactivityLockout]
		, [InactivityLockoutDate]
		, [UserGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [PasswordHint]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [PhoneNumber]
		, [AccountExpirationDate]
		, [ActiveDirectoryUser]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblUsers]
		(
		[UserID]
		, [Password]
		, [LastLoginDate]
		, [LastLogoffDate]
		, [ChangePassword]
		, [PasswordTimeStamp]
		, [Name]
		, [EmailAddress]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [PasswordHistory1]
		, [PasswordHistory2]
		, [PasswordHistory3]
		, [PasswordHistory4]
		, [PasswordHistory5]
		, [PasswordHistory6]
		, [PasswordHistory7]
		, [PasswordHistory8]
		, [PasswordHistory9]
		, [PasswordHistory10]
		, [PasswordHistory11]
		, [PasswordHistory12]
		, [PasswordHistory13]
		, [PasswordHistory14]
		, [PasswordHistory15]
		, [PasswordHistory16]
		, [PasswordHistory17]
		, [PasswordHistory18]
		, [PasswordHistory19]
		, [PasswordHistory20]
		, [PasswordHistory21]
		, [PasswordHistory22]
		, [PasswordHistory23]
		, [PasswordHistory24]
		, [PasswordLockoutCount]
		, [InactivityLockout]
		, [InactivityLockoutDate]
		, [UserGuid]
		, [SourceRowVersion]
		, [SiteGuid]
		, [PasswordHint]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [PhoneNumber]
		, [AccountExpirationDate]
		, [ActiveDirectoryUser]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[UserID]
		, [Password]
		, [LastLoginDate]
		, [LastLogoffDate]
		, [ChangePassword]
		, [PasswordTimeStamp]
		, [Name]
		, [EmailAddress]
		, [CreatedDate]
		, [CreatedBy]
		, [UpdatedDate]
		, [UpdatedBy]
		, [PasswordHistory1]
		, [PasswordHistory2]
		, [PasswordHistory3]
		, [PasswordHistory4]
		, [PasswordHistory5]
		, [PasswordHistory6]
		, [PasswordHistory7]
		, [PasswordHistory8]
		, [PasswordHistory9]
		, [PasswordHistory10]
		, [PasswordHistory11]
		, [PasswordHistory12]
		, [PasswordHistory13]
		, [PasswordHistory14]
		, [PasswordHistory15]
		, [PasswordHistory16]
		, [PasswordHistory17]
		, [PasswordHistory18]
		, [PasswordHistory19]
		, [PasswordHistory20]
		, [PasswordHistory21]
		, [PasswordHistory22]
		, [PasswordHistory23]
		, [PasswordHistory24]
		, [PasswordLockoutCount]
		, [InactivityLockout]
		, [InactivityLockoutDate]
		, [UserGuid]
		, CONVERT(bigint, _RowVersion)
		, [SiteGuid]
		, [PasswordHint]
		, [UserData1]
		, [UserData2]
		, [UserData3]
		, [UserData4]
		, [UserData5]
		, [UserData6]
		, [UserData7]
		, [UserData8]
		, [PhoneNumber]
		, [AccountExpirationDate]
		, [ActiveDirectoryUser]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO

CREATE TRIGGER [dbo].[trg_del_tblUsers] 
ON [dbo].[tblUsers] INSTEAD OF DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	BEGIN TRY
		DELETE x FROM [FuelsManagerDB].[fmcdc].[tblEntityUserToSite] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		UPDATE x SET UserGuid=NULL FROM [FuelsManagerDB].[fmcdc].[tblPersonnel] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[fmcdc].[tblUserToGroup] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[fmcdc].[tblUsers] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid

		DELETE x FROM [FuelsManagerDB].[map].[tblUserToGroup] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[map].[tblEntityUserToSite] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid

		DELETE x FROM [FuelsManagerDB].[dbo].[tblAccessibilityConfigurationSettings] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblDispatchGridColumn] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		--DELETE FROM [FuelsManagerDB].[dbo].[tblErrorTransactionSubmissions] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE SubmittedUserGuid=@UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblMenuFavorites] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblMovementSummary] x INNER JOIN deleted d ON  x.OwnerUserGuid=d.UserGuid WHERE OwnerUserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblOperateScreenConfiguration] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		UPDATE x SET UserGuid=NULL FROM [FuelsManagerDB].[dbo].[tblPersonnel] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE FROM [FuelsManagerDB].[dbo].[tblPointCalculatorRunDetails] WHERE PointCalculatorRunId IN (SELECT x.PointCalculatorRunId FROM [FuelsManagerDB].[dbo].[tblPointCalculatorRuns] x  INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid)
		DELETE x FROM [FuelsManagerDB].[dbo].[tblPointCalculatorRuns] x  INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblPointGroupColumns] x INNER JOIN deleted d ON  x.OwnerUserGuid=d.UserGuid WHERE OwnerUserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblPointGroupRows] x INNER JOIN deleted d ON  x.OwnerUserGuid=d.UserGuid WHERE OwnerUserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblPointGroupSchedule] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblPointGroup] x INNER JOIN deleted d ON  x.OwnerUserGuid=d.UserGuid WHERE OwnerUserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblPointHistory] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblQueryStorage] x INNER JOIN deleted d ON  x.OwnerUserGuid=d.UserGuid WHERE OwnerUserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblSavedQueries] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblSessions] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid
		DELETE x FROM [FuelsManagerDB].[dbo].[tblUserViewStateSettings] x INNER JOIN deleted d ON  x.UserGuid=d.UserGuid WHERE x.UserGuid=d.UserGuid

		DELETE u FROM [FuelsManagerDB].[dbo].[tblUsers] u INNER JOIN deleted d ON  u.UserGuid=d.UserGuid

	END TRY

	BEGIN CATCH

	    IF (XACT_STATE()) = -1
        BEGIN
            ROLLBACK TRANSACTION;
        END;

		THROW;

	END CATCH
	
END
GO

-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblUsers] ON [dbo].[tblUsers]
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblUsers_ClusterIdx]
    ON [dbo].[tblUsers]([_ClusterIdx] ASC);

