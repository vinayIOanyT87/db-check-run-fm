CREATE TABLE [dbo].[tblTestSetEquipmentResults] (
    [ResultTimeStamp]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetEquipmentResults_ResultTimeStamp] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TestSetName]                NVARCHAR (80)      CONSTRAINT [DF_tblTestSetEquipmentResults_TestSetName] DEFAULT ('') NOT NULL,
    [Inspector]                  NVARCHAR (100)     CONSTRAINT [DF_tblTestSetEquipmentResults_Inspector] DEFAULT ('') NOT NULL,
    [Supervisor]                 NVARCHAR (100)     CONSTRAINT [DF_tblTestSetEquipmentResults_Supervisor] DEFAULT ('') NOT NULL,
    [EquipmentID]                NVARCHAR (50)      NOT NULL,
    [SampleNumber]               INT                CONSTRAINT [DF_tblTestSetEquipmentResults_SampleNumber] DEFAULT ((0)) NULL,
    [SampleSize]                 FLOAT (53)         CONSTRAINT [DF_tblTestSetEquipmentResults_SampleSize] DEFAULT ((0.0)) NOT NULL,
    [IsRetest]                   BIT                CONSTRAINT [DF_tblTestSetEquipmentResults_IsRetest] DEFAULT ((0)) NOT NULL,
    [PreviousSampleNumber]       INT                NULL,
    [DocumentNumber]             NVARCHAR (50)      NULL,
    [Memo]                       NVARCHAR (1000)    NULL,
    [GallonsRepresented]         FLOAT (53)         NULL,
    [Override]                   BIT                CONSTRAINT [DF_tblTestSetEquipmentResults_Override] DEFAULT ((0)) NOT NULL,
    [DeleteFlag]                 BIT                CONSTRAINT [DF_tblTestSetEquipmentResults_DeleteFlag] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetEquipmentResults_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblTestSetEquipmentResults_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblTestSetEquipmentResults_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                  [dbo].[udtUserID]  CONSTRAINT [DF_tblTestSetEquipmentResults_UpdatedBy] DEFAULT ('') NOT NULL,
    [TestSetEquipmentResultGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTestSetEquipmentResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                ROWVERSION         NOT NULL,
    [SiteGuid]                   UNIQUEIDENTIFIER   NOT NULL,
    [LookupTestSetStatusIndex]   INT                CONSTRAINT [DF_tblTestSetEquipmentResults_LookupTestSetStatusIndex] DEFAULT ((0)) NOT NULL,
    [EquipmentGuid]              UNIQUEIDENTIFIER   NOT NULL,
	[Flag01]					 BIT				CONSTRAINT [DF_tblTestSetEquipmentResults_Flag01] DEFAULT 0 NULL,
	[Flag02]					 BIT				CONSTRAINT [DF_tblTestSetEquipmentResults_Flag02] DEFAULT 0 NULL,
	[UserData01]				 NVARCHAR(60)       NULL,
	[UserData02]				 NVARCHAR(60)       NULL,
    [_ClusterIdx]                BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTestSetEquipmentResults_GUID] PRIMARY KEY NONCLUSTERED ([TestSetEquipmentResultGuid] ASC),
    CONSTRAINT [FK_tblTestSetEquipmentResults_EquipmentGuid] FOREIGN KEY ([EquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid]),
    CONSTRAINT [FK_tblTestSetEquipmentResults_LookupTestSetStatusIndex] FOREIGN KEY ([LookupTestSetStatusIndex]) REFERENCES [lookup].[tblTestSetStatus] ([TestSetStatusIndex]),
    CONSTRAINT [FK_tblTestSetEquipmentResults_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTestSetEquipmentResults_CreatedDate]
    ON [dbo].[tblTestSetEquipmentResults]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTestSetEquipmentResults_EquipmentGuid]
    ON [dbo].[tblTestSetEquipmentResults]([EquipmentGuid] ASC)
    INCLUDE([ResultTimeStamp], [Memo]);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblTestSetEquipmentResults] ON [dbo].[tblTestSetEquipmentResults] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTestSetEquipmentResults','D')=1 
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
	INSERT INTO [fmaudit].tblTestSetEquipmentResults (
		[ResultTimeStamp]
	,	[TestSetName]
	,	[Inspector]
	,	[Supervisor]
	,	[EquipmentID]
	,	[SampleNumber]
	,	[SampleSize]
	,	[IsRetest]
	,	[PreviousSampleNumber]
	,	[DocumentNumber]
	,	[Memo]
	,	[GallonsRepresented]
	,	[Override]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TestSetEquipmentResultGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTestSetStatusIndex]
	,	[EquipmentGuid]
	,	[Flag01]
	,	[Flag02]
	,	[UserData01]
	,	[UserData02]
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
		d.[ResultTimeStamp]
	,	d.[TestSetName]
	,	d.[Inspector]
	,	d.[Supervisor]
	,	d.[EquipmentID]
	,	d.[SampleNumber]
	,	d.[SampleSize]
	,	d.[IsRetest]
	,	d.[PreviousSampleNumber]
	,	d.[DocumentNumber]
	,	d.[Memo]
	,	d.[GallonsRepresented]
	,	d.[Override]
	,	d.[DeleteFlag]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TestSetEquipmentResultGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupTestSetStatusIndex]
	,	d.[EquipmentGuid]
	,	d.[Flag01]
	,	d.[Flag02]
	,	d.[UserData01]
	,	d.[UserData02]
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
--Creating Insert / Update Trigger for tblTestSetEquipmentResults
CREATE TRIGGER dbo.trg_insupd_tblTestSetEquipmentResults_ForSync 
   ON dbo.tblTestSetEquipmentResults
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
                    ,d.TestSetEquipmentResultGuid AS Deleted_PK_TestSetEquipmentResultGuid
                    ,i.TestSetEquipmentResultGuid AS Inserted_PK_TestSetEquipmentResultGuid
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
				    d.TestSetEquipmentResultGuid = i.TestSetEquipmentResultGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTestSetEquipmentResults As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TestSetEquipmentResultGuid = currentTrackingData.PK_TestSetEquipmentResultGuid
 
 
		    INSERT track.tblTestSetEquipmentResults (InsertedDate 
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
				    ,PK_TestSetEquipmentResultGuid
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
				    ,entityChanges.Inserted_PK_TestSetEquipmentResultGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTestSetEquipmentResults As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TestSetEquipmentResultGuid = currentTrackingData.PK_TestSetEquipmentResultGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTestSetEquipmentResults
CREATE TRIGGER dbo.trg_del_tblTestSetEquipmentResults_ForSync 
   ON dbo.tblTestSetEquipmentResults
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
						,d.TestSetEquipmentResultGuid AS Deleted_PK_TestSetEquipmentResultGuid
                        ,d.TestSetEquipmentResultGuid AS Inserted_PK_TestSetEquipmentResultGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTestSetEquipmentResults As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TestSetEquipmentResultGuid = currentTrackingData.PK_TestSetEquipmentResultGuid
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
						,PK_TestSetEquipmentResultGuid
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
						,entityChanges.Deleted_PK_TestSetEquipmentResultGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTestSetEquipmentResults] ON [dbo].[tblTestSetEquipmentResults] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTestSetEquipmentResults','D')=1 
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
	INSERT INTO [fmaudit].tblTestSetEquipmentResults (
		[ResultTimeStamp]
	,	[TestSetName]
	,	[Inspector]
	,	[Supervisor]
	,	[EquipmentID]
	,	[SampleNumber]
	,	[SampleSize]
	,	[IsRetest]
	,	[PreviousSampleNumber]
	,	[DocumentNumber]
	,	[Memo]
	,	[GallonsRepresented]
	,	[Override]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TestSetEquipmentResultGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTestSetStatusIndex]
	,	[EquipmentGuid]
	,	[Flag01]
	,	[Flag02]
	,	[UserData01]
	,	[UserData02]
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
		i.[ResultTimeStamp]
	,	i.[TestSetName]
	,	i.[Inspector]
	,	i.[Supervisor]
	,	i.[EquipmentID]
	,	i.[SampleNumber]
	,	i.[SampleSize]
	,	i.[IsRetest]
	,	i.[PreviousSampleNumber]
	,	i.[DocumentNumber]
	,	i.[Memo]
	,	i.[GallonsRepresented]
	,	i.[Override]
	,	i.[DeleteFlag]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TestSetEquipmentResultGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupTestSetStatusIndex]
	,	i.[EquipmentGuid]
	,	i.[Flag01]
	,	i.[Flag02]
	,	i.[UserData01]
	,	i.[UserData02]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTestSetEquipmentResults] ON [dbo].[tblTestSetEquipmentResults] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTestSetEquipmentResults','D')=1 
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
	TestSetEquipmentResultGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTestSetEquipmentResults (
		[ResultTimeStamp]
	,	[TestSetName]
	,	[Inspector]
	,	[Supervisor]
	,	[EquipmentID]
	,	[SampleNumber]
	,	[SampleSize]
	,	[IsRetest]
	,	[PreviousSampleNumber]
	,	[DocumentNumber]
	,	[Memo]
	,	[GallonsRepresented]
	,	[Override]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TestSetEquipmentResultGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTestSetStatusIndex]
	,	[EquipmentGuid]
	,	[Flag01]
	,	[Flag02]
	,	[UserData01]
	,	[UserData02]
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
	OUTPUT inserted.[TestSetEquipmentResultGuid] AS 'TestSetEquipmentResultGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ResultTimeStamp]
	,	d.[TestSetName]
	,	d.[Inspector]
	,	d.[Supervisor]
	,	d.[EquipmentID]
	,	d.[SampleNumber]
	,	d.[SampleSize]
	,	d.[IsRetest]
	,	d.[PreviousSampleNumber]
	,	d.[DocumentNumber]
	,	d.[Memo]
	,	d.[GallonsRepresented]
	,	d.[Override]
	,	d.[DeleteFlag]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[TestSetEquipmentResultGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[LookupTestSetStatusIndex]
	,	d.[EquipmentGuid]
	,	d.[Flag01]
	,	d.[Flag02]
	,	d.[UserData01]
	,	d.[UserData02]
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
 
	INSERT INTO [fmaudit].tblTestSetEquipmentResults (
		[ResultTimeStamp]
	,	[TestSetName]
	,	[Inspector]
	,	[Supervisor]
	,	[EquipmentID]
	,	[SampleNumber]
	,	[SampleSize]
	,	[IsRetest]
	,	[PreviousSampleNumber]
	,	[DocumentNumber]
	,	[Memo]
	,	[GallonsRepresented]
	,	[Override]
	,	[DeleteFlag]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[TestSetEquipmentResultGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[LookupTestSetStatusIndex]
	,	[EquipmentGuid]
	,	[Flag01]
	,	[Flag02]
	,	[UserData01]
	,	[UserData02]
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
		i.[ResultTimeStamp]
	,	i.[TestSetName]
	,	i.[Inspector]
	,	i.[Supervisor]
	,	i.[EquipmentID]
	,	i.[SampleNumber]
	,	i.[SampleSize]
	,	i.[IsRetest]
	,	i.[PreviousSampleNumber]
	,	i.[DocumentNumber]
	,	i.[Memo]
	,	i.[GallonsRepresented]
	,	i.[Override]
	,	i.[DeleteFlag]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[TestSetEquipmentResultGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[LookupTestSetStatusIndex]
	,	i.[EquipmentGuid]
	,	i.[Flag01]
	,	i.[Flag02]
	,	i.[UserData01]
	,	i.[UserData02]
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
			agl.[TestSetEquipmentResultGuid]=i.[TestSetEquipmentResultGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTestSetEquipmentResults_ClusterIdx]
    ON [dbo].[tblTestSetEquipmentResults]([_ClusterIdx] ASC);

