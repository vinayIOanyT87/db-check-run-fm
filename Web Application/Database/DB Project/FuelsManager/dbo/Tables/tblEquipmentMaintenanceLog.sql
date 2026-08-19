CREATE TABLE [dbo].[tblEquipmentMaintenanceLog] (
    [EquipmentID]                 NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_EquipmentID] DEFAULT ('') NOT NULL,
    [EquipmentType]               NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_EquipmentType] DEFAULT ('') NOT NULL,
    [OperatorID]                  NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_OperatorID] DEFAULT ('') NOT NULL,
    [MaintenanceReason]           NVARCHAR (50)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_MaintenanceReason] DEFAULT ('Is In Service') NOT NULL,
    [InServiceFlag]               TINYINT            CONSTRAINT [DF_tblEquipmentMaintenanceLog_InServiceFlag] DEFAULT ((1)) NOT NULL,
    [ChangeDate]                  DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_ChangeDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [EstReturnToServiceDate]      DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_EstReturnToServiceDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [WorkOrder]                   NVARCHAR (20)      CONSTRAINT [DF_tblEquipmentMaintenanceLog_WorkOrder] DEFAULT ('') NOT NULL,
    [Memo]                        NVARCHAR (1000)    CONSTRAINT [DF_tblEquipmentMaintenanceLog_Memo] DEFAULT ('') NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentMaintenanceLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEquipmentMaintenanceLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblEquipmentMaintenanceLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [EquipmentMaintenanceLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblEquipmentMaintenanceLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [SiteGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [EquipmentGuid]               UNIQUEIDENTIFIER   NOT NULL,
    [MaintenanceReasonGuid]       UNIQUEIDENTIFIER   NULL,
    [OperatorPersonnelGuid]       UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblEquipmentMaintenanceLog_GUID] PRIMARY KEY NONCLUSTERED ([EquipmentMaintenanceLogGuid] ASC),
    CONSTRAINT [CK_tblEquipmentMaintenanceLog_InServiceFlag] CHECK ([InServiceFlag]=(1) AND [MaintenanceReasonGuid] IS NULL OR [InServiceFlag]=(0) AND [MaintenanceReasonGuid] IS NOT NULL),
    CONSTRAINT [FK_tblEquipmentMaintenanceLog_EquipmentGuid] FOREIGN KEY ([EquipmentGuid]) REFERENCES [dbo].[tblEquipment] ([EquipmentGuid]),
    CONSTRAINT [FK_tblEquipmentMaintenanceLog_MaintenanceReasonGuid] FOREIGN KEY ([MaintenanceReasonGuid]) REFERENCES [dbo].[tblMaintenanceReasons] ([MaintenanceReasonGuid]),
    CONSTRAINT [FK_tblEquipmentMaintenanceLog_OperatorPersonnelGuid] FOREIGN KEY ([OperatorPersonnelGuid]) REFERENCES [dbo].[tblPersonnel] ([PersonnelGuid]),
    CONSTRAINT [FK_tblEquipmentMaintenanceLog_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_CreatedDate]
    ON [dbo].[tblEquipmentMaintenanceLog]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_EquipmentGuid_ChangeDate]
    ON [dbo].[tblEquipmentMaintenanceLog]([EquipmentGuid] ASC, [ChangeDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_EquipmentGuid_InserviceFlag]
    ON [dbo].[tblEquipmentMaintenanceLog]([EquipmentGuid] ASC, [InServiceFlag] ASC)
    INCLUDE([ChangeDate], [EstReturnToServiceDate]);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_EquipmentGuid]
    ON [dbo].[tblEquipmentMaintenanceLog]([EquipmentGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_MaintenanceReasonGuid]
    ON [dbo].[tblEquipmentMaintenanceLog]([MaintenanceReasonGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_OperatorGuid]
    ON [dbo].[tblEquipmentMaintenanceLog]([OperatorPersonnelGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_SiteGuid]
    ON [dbo].[tblEquipmentMaintenanceLog]([SiteGuid] ASC);


GO
--Creating Insert / Update Trigger for tblEquipmentMaintenanceLog
CREATE TRIGGER dbo.trg_insupd_tblEquipmentMaintenanceLog_ForSync 
   ON dbo.tblEquipmentMaintenanceLog
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
                    ,d.EquipmentMaintenanceLogGuid AS Deleted_PK_EquipmentMaintenanceLogGuid
                    ,i.EquipmentMaintenanceLogGuid AS Inserted_PK_EquipmentMaintenanceLogGuid
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
				    d.EquipmentMaintenanceLogGuid = i.EquipmentMaintenanceLogGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblEquipmentMaintenanceLog As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_EquipmentMaintenanceLogGuid = currentTrackingData.PK_EquipmentMaintenanceLogGuid
 
 
		    INSERT track.tblEquipmentMaintenanceLog (InsertedDate 
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
				    ,PK_EquipmentMaintenanceLogGuid
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
				    ,entityChanges.Inserted_PK_EquipmentMaintenanceLogGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblEquipmentMaintenanceLog As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_EquipmentMaintenanceLogGuid = currentTrackingData.PK_EquipmentMaintenanceLogGuid
)
    END
END 

GO
--Creating Delete Trigger for tblEquipmentMaintenanceLog
CREATE TRIGGER dbo.trg_del_tblEquipmentMaintenanceLog_ForSync 
   ON dbo.tblEquipmentMaintenanceLog
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
						,d.EquipmentMaintenanceLogGuid AS Deleted_PK_EquipmentMaintenanceLogGuid
                        ,d.EquipmentMaintenanceLogGuid AS Inserted_PK_EquipmentMaintenanceLogGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblEquipmentMaintenanceLog As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_EquipmentMaintenanceLogGuid = currentTrackingData.PK_EquipmentMaintenanceLogGuid
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
						,PK_EquipmentMaintenanceLogGuid
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
						,entityChanges.Deleted_PK_EquipmentMaintenanceLogGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[TR_tblEquipmentMaintenanceLog_U]
    ON [dbo].[tblEquipmentMaintenanceLog]
    FOR UPDATE
    AS BEGIN
	SET NOCOUNT ON
	RAISERROR(N'     *** UPDATEs are not allowed on the tblEquipmentMaintenanceLog table.', 20, 1) WITH NOWAIT
	ROLLBACK
END
GO
CREATE TRIGGER [dbo].[TR_tblEquipmentMaintenanceLog_D]
    ON [dbo].[tblEquipmentMaintenanceLog]
    FOR UPDATE
    AS BEGIN
	SET NOCOUNT ON
	RAISERROR(N'     *** UPDATEs are not allowed on the tblEquipmentMaintenanceLog table.', 20, 1) WITH NOWAIT
	ROLLBACK
END
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblEquipmentMaintenanceLog] ON [dbo].[tblEquipmentMaintenanceLog] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipmentMaintenanceLog','D')=1 
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
	INSERT INTO [fmaudit].tblEquipmentMaintenanceLog (
		[EquipmentID]
	,	[EquipmentType]
	,	[OperatorID]
	,	[MaintenanceReason]
	,	[InServiceFlag]
	,	[ChangeDate]
	,	[EstReturnToServiceDate]
	,	[WorkOrder]
	,	[Memo]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[EquipmentMaintenanceLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[MaintenanceReasonGuid]
	,	[OperatorPersonnelGuid]
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
		d.[EquipmentID]
	,	d.[EquipmentType]
	,	d.[OperatorID]
	,	d.[MaintenanceReason]
	,	d.[InServiceFlag]
	,	d.[ChangeDate]
	,	d.[EstReturnToServiceDate]
	,	d.[WorkOrder]
	,	d.[Memo]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[EquipmentMaintenanceLogGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[EquipmentGuid]
	,	d.[MaintenanceReasonGuid]
	,	d.[OperatorPersonnelGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblEquipmentMaintenanceLog] ON [dbo].[tblEquipmentMaintenanceLog] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipmentMaintenanceLog','D')=1 
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
	INSERT INTO [fmaudit].tblEquipmentMaintenanceLog (
		[EquipmentID]
	,	[EquipmentType]
	,	[OperatorID]
	,	[MaintenanceReason]
	,	[InServiceFlag]
	,	[ChangeDate]
	,	[EstReturnToServiceDate]
	,	[WorkOrder]
	,	[Memo]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[EquipmentMaintenanceLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[MaintenanceReasonGuid]
	,	[OperatorPersonnelGuid]
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
		i.[EquipmentID]
	,	i.[EquipmentType]
	,	i.[OperatorID]
	,	i.[MaintenanceReason]
	,	i.[InServiceFlag]
	,	i.[ChangeDate]
	,	i.[EstReturnToServiceDate]
	,	i.[WorkOrder]
	,	i.[Memo]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[EquipmentMaintenanceLogGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[EquipmentGuid]
	,	i.[MaintenanceReasonGuid]
	,	i.[OperatorPersonnelGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblEquipmentMaintenanceLog] ON [dbo].[tblEquipmentMaintenanceLog] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblEquipmentMaintenanceLog','D')=1 
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
	EquipmentMaintenanceLogGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblEquipmentMaintenanceLog (
		[EquipmentID]
	,	[EquipmentType]
	,	[OperatorID]
	,	[MaintenanceReason]
	,	[InServiceFlag]
	,	[ChangeDate]
	,	[EstReturnToServiceDate]
	,	[WorkOrder]
	,	[Memo]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[EquipmentMaintenanceLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[MaintenanceReasonGuid]
	,	[OperatorPersonnelGuid]
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
	OUTPUT inserted.[EquipmentMaintenanceLogGuid] AS 'EquipmentMaintenanceLogGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[EquipmentID]
	,	d.[EquipmentType]
	,	d.[OperatorID]
	,	d.[MaintenanceReason]
	,	d.[InServiceFlag]
	,	d.[ChangeDate]
	,	d.[EstReturnToServiceDate]
	,	d.[WorkOrder]
	,	d.[Memo]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[EquipmentMaintenanceLogGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[EquipmentGuid]
	,	d.[MaintenanceReasonGuid]
	,	d.[OperatorPersonnelGuid]
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
 
	INSERT INTO [fmaudit].tblEquipmentMaintenanceLog (
		[EquipmentID]
	,	[EquipmentType]
	,	[OperatorID]
	,	[MaintenanceReason]
	,	[InServiceFlag]
	,	[ChangeDate]
	,	[EstReturnToServiceDate]
	,	[WorkOrder]
	,	[Memo]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[EquipmentMaintenanceLogGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[EquipmentGuid]
	,	[MaintenanceReasonGuid]
	,	[OperatorPersonnelGuid]
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
		i.[EquipmentID]
	,	i.[EquipmentType]
	,	i.[OperatorID]
	,	i.[MaintenanceReason]
	,	i.[InServiceFlag]
	,	i.[ChangeDate]
	,	i.[EstReturnToServiceDate]
	,	i.[WorkOrder]
	,	i.[Memo]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[EquipmentMaintenanceLogGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[EquipmentGuid]
	,	i.[MaintenanceReasonGuid]
	,	i.[OperatorPersonnelGuid]
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
			agl.[EquipmentMaintenanceLogGuid]=i.[EquipmentMaintenanceLogGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEquipmentMaintenanceLog_ClusterIdx]
    ON [dbo].[tblEquipmentMaintenanceLog]([_ClusterIdx] ASC);
