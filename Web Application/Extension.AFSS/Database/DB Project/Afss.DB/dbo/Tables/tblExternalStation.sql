CREATE TABLE [dbo].[tblExternalStation]
(
	[ExternalStationGuid] UNIQUEIDENTIFIER NOT NULL, 
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL, 
    [ID] NVARCHAR(50) NOT NULL, 
    [LookupExternalStationTypeIndex] INT NOT NULL, 
    [BillingID] NVARCHAR(50) NULL, 
    [DownloadTransactionsAutomatically] BIT NOT NULL, 
    [LookupExternalStationStatusIndex] INT NOT NULL, 
    [LastSuccessfulConnection] DATETIMEOFFSET NULL, 
    [LastConnectionAttempt] DATETIMEOFFSET NULL, 
    [LastTransactionID] BIGINT NULL, 
    [CreatedBy] [dbo].[udtUserID] NOT NULL, 
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET NOT NULL, 
    [_RowVersion] TIMESTAMP NOT NULL,   
	[_ClusterIdx] BIGINT IDENTITY(1,1) NOT NULL, 
	 [LastDeviceCount] INT NULL,
    CONSTRAINT [PK_tblExternalStation] PRIMARY KEY NONCLUSTERED (ExternalStationGuid), 
    CONSTRAINT [FK_tblExternalStation_LookupExternalStationStatusIndex] FOREIGN KEY (LookupExternalStationStatusIndex) REFERENCES [lookup].[tblExternalStationStatus]([ExternalStationStatusIndex]), 
    CONSTRAINT [FK_tblExternalStation_LookupExternalStationTypeIndex] FOREIGN KEY (LookupExternalStationTypeIndex) REFERENCES [lookup].[tblExternalStationType]([ExternalStationTypeIndex]), 
    CONSTRAINT [FK_tblExternalStation_tblSites] FOREIGN KEY (SiteGuid) REFERENCES [dbo].[tblSites]([SiteGuid]),
	CONSTRAINT [CK_tblExternalStation_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessExternalStation]([ExternalStationGuid],[ID]) = 1)
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblExternalStation__ClusterIdx] ON [dbo].[tblExternalStation] (_ClusterIdx)
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStation_ID] ON [dbo].[tblExternalStation] (ID)
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStation_SiteGuid] ON [dbo].[tblExternalStation] (SiteGuid)
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStation_LookupExternalStationStatusIndex] ON [dbo].[tblExternalStation] (LookupExternalStationStatusIndex)
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStation_LookupExternalStationTypeIndex] ON [dbo].[tblExternalStation] (LookupExternalStationTypeIndex)
GO
-------------------------------------
-- AUDIT DELETE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_del_tblExternalStation] ON [dbo].[tblExternalStation] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExternalStation','D')=1 
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
	INSERT INTO [fmaudit].tblExternalStation (
		[ExternalStationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[LookupExternalStationTypeIndex]
	,	[BillingID]
	,	[DownloadTransactionsAutomatically]
	,	[LookupExternalStationStatusIndex]
	,	[LastSuccessfulConnection]
	,	[LastConnectionAttempt]
	,	[LastTransactionID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[LastDeviceCount]
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
		d.[ExternalStationGuid]
	,	d.[SiteGuid]
	,	d.[ID]
	,	d.[LookupExternalStationTypeIndex]
	,	d.[BillingID]
	,	d.[DownloadTransactionsAutomatically]
	,	d.[LookupExternalStationStatusIndex]
	,	d.[LastSuccessfulConnection]
	,	d.[LastConnectionAttempt]
	,	d.[LastTransactionID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[_RowVersion]
	,	d.[LastDeviceCount]
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
 
CREATE TRIGGER [dbo].[trg_Audit_ins_tblExternalStation] ON [dbo].[tblExternalStation] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExternalStation','D')=1 
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
	INSERT INTO [fmaudit].tblExternalStation (
		[ExternalStationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[LookupExternalStationTypeIndex]
	,	[BillingID]
	,	[DownloadTransactionsAutomatically]
	,	[LookupExternalStationStatusIndex]
	,	[LastSuccessfulConnection]
	,	[LastConnectionAttempt]
	,	[LastTransactionID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[LastDeviceCount]
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
		i.[ExternalStationGuid]
	,	i.[SiteGuid]
	,	i.[ID]
	,	i.[LookupExternalStationTypeIndex]
	,	i.[BillingID]
	,	i.[DownloadTransactionsAutomatically]
	,	i.[LookupExternalStationStatusIndex]
	,	i.[LastSuccessfulConnection]
	,	i.[LastConnectionAttempt]
	,	i.[LastTransactionID]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[_RowVersion]
	,	i.[LastDeviceCount]
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
 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblExternalStation] ON [dbo].[tblExternalStation] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblExternalStation','D')=1 
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
	ExternalStationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblExternalStation (
		[ExternalStationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[LookupExternalStationTypeIndex]
	,	[BillingID]
	,	[DownloadTransactionsAutomatically]
	,	[LookupExternalStationStatusIndex]
	,	[LastSuccessfulConnection]
	,	[LastConnectionAttempt]
	,	[LastTransactionID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[LastDeviceCount]
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
	OUTPUT inserted.[ExternalStationGuid] AS 'ExternalStationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ExternalStationGuid]
	,	d.[SiteGuid]
	,	d.[ID]
	,	d.[LookupExternalStationTypeIndex]
	,	d.[BillingID]
	,	d.[DownloadTransactionsAutomatically]
	,	d.[LookupExternalStationStatusIndex]
	,	d.[LastSuccessfulConnection]
	,	d.[LastConnectionAttempt]
	,	d.[LastTransactionID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[_RowVersion]
	,	d.[LastDeviceCount]
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
 
	INSERT INTO [fmaudit].tblExternalStation (
		[ExternalStationGuid]
	,	[SiteGuid]
	,	[ID]
	,	[LookupExternalStationTypeIndex]
	,	[BillingID]
	,	[DownloadTransactionsAutomatically]
	,	[LookupExternalStationStatusIndex]
	,	[LastSuccessfulConnection]
	,	[LastConnectionAttempt]
	,	[LastTransactionID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[OriginalRowVersion]
	,	[LastDeviceCount]
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
		i.[ExternalStationGuid]
	,	i.[SiteGuid]
	,	i.[ID]
	,	i.[LookupExternalStationTypeIndex]
	,	i.[BillingID]
	,	i.[DownloadTransactionsAutomatically]
	,	i.[LookupExternalStationStatusIndex]
	,	i.[LastSuccessfulConnection]
	,	i.[LastConnectionAttempt]
	,	i.[LastTransactionID]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[_RowVersion]
	,	i.[LastDeviceCount]
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
			agl.[ExternalStationGuid]=i.[ExternalStationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
--Creating Insert / Update Trigger for tblExternalStation
CREATE TRIGGER dbo.trg_insupd_tblExternalStation_ForSync 
   ON dbo.tblExternalStation
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
                   ,d.ExternalStationGuid AS Deleted_PK_ExternalStationGuid
                    ,i.ExternalStationGuid AS Inserted_PK_ExternalStationGuid
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
				    d.ExternalStationGuid = i.ExternalStationGuid
           ) 
		    MERGE INTO track.tblExternalStation WITH (HOLDLOCK) As currentTrackingData 
			    USING ChangeList As entityChanges 
				    ON entityChanges.Inserted_PK_ExternalStationGuid = currentTrackingData.PK_ExternalStationGuid
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
				    ,PK_ExternalStationGuid
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
				    ,entityChanges.Inserted_PK_ExternalStationGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    );
    END
END
GO
--Creating Delete Trigger for tblExternalStation
CREATE TRIGGER dbo.trg_del_tblExternalStation_ForSync 
   ON dbo.tblExternalStation
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
						,d.ExternalStationGuid AS Deleted_PK_ExternalStationGuid
                        ,d.ExternalStationGuid AS Inserted_PK_ExternalStationGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblExternalStation WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ExternalStationGuid = currentTrackingData.PK_ExternalStationGuid
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
						,PK_ExternalStationGuid
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
						,entityChanges.Deleted_PK_ExternalStationGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END