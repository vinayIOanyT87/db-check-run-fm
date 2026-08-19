CREATE TABLE [dbo].[tblTransactionUserData] (
    [UserData1]               NVARCHAR (255)     NULL,
    [UserData2]               NVARCHAR (255)     NULL,
    [UserData3]               NVARCHAR (255)     NULL,
    [UserData4]               NVARCHAR (255)     NULL,
    [UserData5]               NVARCHAR (255)     NULL,
    [UserData6]               NVARCHAR (255)     NULL,
    [UserData7]               NVARCHAR (255)     NULL,
    [UserData8]               NVARCHAR (255)     NULL,
    [UserData9]               NVARCHAR (255)     NULL,
    [UserData10]              NVARCHAR (255)     NULL,
    [UserData11]              NVARCHAR (255)     NULL,
    [UserData12]              NVARCHAR (255)     NULL,
    [UserData13]              NVARCHAR (255)     NULL,
    [UserData14]              NVARCHAR (255)     NULL,
    [UserData15]              NVARCHAR (255)     NULL,
    [UserData16]              NVARCHAR (255)     NULL,
    [UserData17]              NVARCHAR (255)     NULL,
    [UserData18]              NVARCHAR (255)     NULL,
    [UserData19]              NVARCHAR (255)     NULL,
    [UserData20]              NVARCHAR (255)     NULL,
    [UserData21]              NVARCHAR (255)     NULL,
    [UserData22]              NVARCHAR (255)     NULL,
    [UserData23]              NVARCHAR (255)     NULL,
    [UserData24]              NVARCHAR (255)     NULL,
    [CreatedBy]               [dbo].[udtUserID]  NULL,
    [CreatedDate]             DATETIMEOFFSET (7) NULL,
    [UpdatedBy]               [dbo].[udtUserID]  NULL,
    [UpdatedDate]             DATETIMEOFFSET (7) NULL,
    [TransactionUserDataGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionUserData_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]             ROWVERSION         NOT NULL,
    [TransactionGuid]         UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]             BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionUserData_GUID] PRIMARY KEY NONCLUSTERED ([TransactionUserDataGuid] ASC),
    CONSTRAINT [FK_tblTransactionUserData_TransactionGuid] FOREIGN KEY ([TransactionGuid]) REFERENCES [dbo].[tblTransactions] ([TransactionGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionUserData_ClusterIdx] 
	ON [dbo].[tblTransactionUserData]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_CreatedDate]
    ON [dbo].[tblTransactionUserData]([CreatedDate] ASC);

GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_TransactionGuid]
    ON [dbo].[tblTransactionUserData]([TransactionGuid] ASC)
    INCLUDE([TransactionUserDataGuid]);


GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionUserData_TransactionGuidUserData4] ON [dbo].[tblTransactionUserData]
(
	[TransactionGuid] ASC
)
INCLUDE ( 	[UserData4],
	[UserData1],
	[UserData2],
	[UserData3],
	[UserData5],
	[UserData6],
	[UserData7],
	[UserData8],
	[UserData9]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionUserData] ON [dbo].[tblTransactionUserData] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionUserData','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionUserData (
		[UserData1]
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
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
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
		d.[UserData1]
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
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionUserDataGuid]
	,	d.[_RowVersion]
	,	d.[TransactionGuid]
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
--Creating Insert / Update Trigger for tblTransactionUserData
CREATE TRIGGER dbo.trg_insupd_tblTransactionUserData_ForSync 
   ON dbo.tblTransactionUserData
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
                    ,d.TransactionUserDataGuid AS Deleted_PK_TransactionUserDataGuid
                    ,i.TransactionUserDataGuid AS Inserted_PK_TransactionUserDataGuid
                    ,d.TransactionGuid AS Deleted_FK_ParentPK
                    ,i.TransactionGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TransactionUserDataGuid = i.TransactionUserDataGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionUserData As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionUserDataGuid = currentTrackingData.PK_TransactionUserDataGuid
 
 
		    INSERT track.tblTransactionUserData (InsertedDate 
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
				    ,PK_TransactionUserDataGuid
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
				    ,entityChanges.Inserted_PK_TransactionUserDataGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionUserData As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionUserDataGuid = currentTrackingData.PK_TransactionUserDataGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionUserData
CREATE TRIGGER dbo.trg_del_tblTransactionUserData_ForSync 
   ON dbo.tblTransactionUserData
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
						,d.TransactionUserDataGuid AS Deleted_PK_TransactionUserDataGuid
                        ,d.TransactionUserDataGuid AS Inserted_PK_TransactionUserDataGuid
                      ,d.TransactionGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionUserData As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionUserDataGuid = currentTrackingData.PK_TransactionUserDataGuid
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
						,PK_TransactionUserDataGuid
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
						,entityChanges.Deleted_PK_TransactionUserDataGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionUserData] ON [dbo].[tblTransactionUserData] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionUserData','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionUserData (
		[UserData1]
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
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
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
		i.[UserData1]
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
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[TransactionUserDataGuid]
	,	i.[_RowVersion]
	,	i.[TransactionGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionUserData] ON [dbo].[tblTransactionUserData] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionUserData','D')=1 
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
	TransactionUserDataGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionUserData (
		[UserData1]
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
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
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
	OUTPUT inserted.[TransactionUserDataGuid] AS 'TransactionUserDataGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[UserData1]
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
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionUserDataGuid]
	,	d.[_RowVersion]
	,	d.[TransactionGuid]
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
 
	INSERT INTO [fmaudit].tblTransactionUserData (
		[UserData1]
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
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionGuid]
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
		i.[UserData1]
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
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[TransactionUserDataGuid]
	,	i.[_RowVersion]
	,	i.[TransactionGuid]
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
			agl.[TransactionUserDataGuid]=i.[TransactionUserDataGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblTransactionUserData]
ON [dbo].[tblTransactionUserData]
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
		DECLARE  @context_info varbinary(128)
		DECLARE  @context_info_str varchar(128)
		SELECT @Context_Info = CONTEXT_INFO()
		SELECT @context_info_str = CAST (@context_info as varchar(128))
		IF (@context_info_str = 'dbo.fm_ArchiveTransaction')
		BEGIN
			RETURN
		END
		INSERT INTO fmcdc.[tblTransactionUserData]
		(
		[UserData1]
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
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionUserDataGuid]
		, [SourceRowVersion]
		, [TransactionGuid]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[UserData1]
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
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionUserDataGuid]
		, CONVERT(bigint, _RowVersion)
		, [TransactionGuid]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTransactionUserData]
		(
		[UserData1]
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
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionUserDataGuid]
		, [SourceRowVersion]
		, [TransactionGuid]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[UserData1]
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
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionUserDataGuid]
		, CONVERT(bigint, _RowVersion)
		, [TransactionGuid]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTransactionUserData] ON [dbo].[tblTransactionUserData]
GO