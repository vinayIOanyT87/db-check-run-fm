CREATE TABLE [dbo].[tblTransactionLineItemUserData] (
    [UserData1]                       NVARCHAR (60)      NULL,
    [UserData2]                       NVARCHAR (60)      NULL,
    [UserData3]                       NVARCHAR (60)      NULL,
    [UserData4]                       NVARCHAR (60)      NULL,
    [UserData5]                       NVARCHAR (60)      NULL,
    [UserData6]                       NVARCHAR (60)      NULL,
    [UserData7]                       NVARCHAR (60)      NULL,
    [UserData8]                       NVARCHAR (60)      NULL,
    [UserData9]                       NVARCHAR (60)      NULL,
    [UserData10]                      NVARCHAR (60)      NULL,
    [UserData11]                      NVARCHAR (60)      NULL,
    [UserData12]                      NVARCHAR (60)      NULL,
    [UserData13]                      NVARCHAR (60)      NULL,
    [UserData14]                      NVARCHAR (60)      NULL,
    [UserData15]                      NVARCHAR (60)      NULL,
    [UserData16]                      NVARCHAR (60)      NULL,
    [UserData17]                      NVARCHAR (60)      NULL,
    [UserData18]                      NVARCHAR (60)      NULL,
    [UserData19]                      NVARCHAR (60)      NULL,
    [UserData20]                      NVARCHAR (60)      NULL,
    [UserData21]                      NVARCHAR (60)      NULL,
    [UserData22]                      NVARCHAR (60)      NULL,
    [UserData23]                      NVARCHAR (60)      NULL,
    [UserData24]                      NVARCHAR (60)      NULL,
    [CreatedBy]                       [dbo].[udtUserID]  NULL,
    [CreatedDate]                     DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                       [dbo].[udtUserID]  NULL,
    [UpdatedDate]                     DATETIMEOFFSET (7) NULL,
    [TransactionLineItemUserDataGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionLineItemUserData_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                     ROWVERSION         NOT NULL,
    [TransactionLineItemGuid]         UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]                     BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionLineItemUserData_GUID] PRIMARY KEY NONCLUSTERED ([TransactionLineItemUserDataGuid] ASC),
    CONSTRAINT [FK_tblTransactionLineItemUserData] FOREIGN KEY ([TransactionLineItemGuid]) REFERENCES [dbo].[tblTransactionLineItems] ([TransactionLineItemGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionLineItemUserData_ClusterIdx] 
	ON [dbo].[tblTransactionLineItemUserData]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_CreatedDate]
    ON [dbo].[tblTransactionLineItemUserData]([CreatedDate] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionLineItemUserData_TransactionLineItemGuid] ON [dbo].[tblTransactionLineItemUserData]
(
	[TransactionLineItemGuid] ASC
)
INCLUDE ( 	[TransactionLineItemUserDataGuid],
	[UserData1],
	[UserData2],
	[UserData3],
	[UserData4],
	[UserData5],
	[UserData6],
	[UserData7]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO

CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItemUserData','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionLineItemUserData (
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
	,	[TransactionLineItemUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionLineItemGuid]
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
	,	d.[TransactionLineItemUserDataGuid]
	,	d.[_RowVersion]
	,	d.[TransactionLineItemGuid]
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
--Creating Insert / Update Trigger for tblTransactionLineItemUserData
CREATE TRIGGER dbo.trg_insupd_tblTransactionLineItemUserData_ForSync 
   ON dbo.tblTransactionLineItemUserData
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
                    ,d.TransactionLineItemUserDataGuid AS Deleted_PK_TransactionLineItemUserDataGuid
                    ,i.TransactionLineItemUserDataGuid AS Inserted_PK_TransactionLineItemUserDataGuid
                    ,d.TransactionLineItemGuid AS Deleted_FK_ParentPK
                    ,i.TransactionLineItemGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TransactionLineItemUserDataGuid = i.TransactionLineItemUserDataGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionLineItemUserData As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionLineItemUserDataGuid = currentTrackingData.PK_TransactionLineItemUserDataGuid
 
 
		    INSERT track.tblTransactionLineItemUserData (InsertedDate 
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
				    ,PK_TransactionLineItemUserDataGuid
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
				    ,entityChanges.Inserted_PK_TransactionLineItemUserDataGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionLineItemUserData As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionLineItemUserDataGuid = currentTrackingData.PK_TransactionLineItemUserDataGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionLineItemUserData
CREATE TRIGGER dbo.trg_del_tblTransactionLineItemUserData_ForSync 
   ON dbo.tblTransactionLineItemUserData
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
						,d.TransactionLineItemUserDataGuid AS Deleted_PK_TransactionLineItemUserDataGuid
                        ,d.TransactionLineItemUserDataGuid AS Inserted_PK_TransactionLineItemUserDataGuid
                      ,d.TransactionLineItemGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionLineItemUserData As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionLineItemUserDataGuid = currentTrackingData.PK_TransactionLineItemUserDataGuid
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
						,PK_TransactionLineItemUserDataGuid
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
						,entityChanges.Deleted_PK_TransactionLineItemUserDataGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItemUserData','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionLineItemUserData (
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
	,	[TransactionLineItemUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionLineItemGuid]
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
	,	i.[TransactionLineItemUserDataGuid]
	,	i.[_RowVersion]
	,	i.[TransactionLineItemGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionLineItemUserData','D')=1 
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
	TransactionLineItemUserDataGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionLineItemUserData (
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
	,	[TransactionLineItemUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionLineItemGuid]
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
	OUTPUT inserted.[TransactionLineItemUserDataGuid] AS 'TransactionLineItemUserDataGuid'
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
	,	d.[TransactionLineItemUserDataGuid]
	,	d.[_RowVersion]
	,	d.[TransactionLineItemGuid]
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
 
	INSERT INTO [fmaudit].tblTransactionLineItemUserData (
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
	,	[TransactionLineItemUserDataGuid]
	,	[OriginalRowVersion]
	,	[TransactionLineItemGuid]
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
	,	i.[TransactionLineItemUserDataGuid]
	,	i.[_RowVersion]
	,	i.[TransactionLineItemGuid]
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
			agl.[TransactionLineItemUserDataGuid]=i.[TransactionLineItemUserDataGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblTransactionLineItemUserData]
ON [dbo].[tblTransactionLineItemUserData]
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
		INSERT INTO fmcdc.[tblTransactionLineItemUserData]
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
		, [TransactionLineItemUserDataGuid]
		, [SourceRowVersion]
		, [TransactionLineItemGuid]
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
		, [TransactionLineItemUserDataGuid]
		, CONVERT(bigint, _RowVersion)
		, [TransactionLineItemGuid]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTransactionLineItemUserData]
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
		, [TransactionLineItemUserDataGuid]
		, [SourceRowVersion]
		, [TransactionLineItemGuid]
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
		, [TransactionLineItemUserDataGuid]
		, CONVERT(bigint, _RowVersion)
		, [TransactionLineItemGuid]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTransactionLineItemUserData] ON [dbo].[tblTransactionLineItemUserData]
GO
