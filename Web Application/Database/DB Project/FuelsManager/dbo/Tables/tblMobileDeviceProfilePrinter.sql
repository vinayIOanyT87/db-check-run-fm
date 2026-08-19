CREATE TABLE [dbo].[tblMobileDeviceProfilePrinter] (
    [MobileDeviceProfilePrinterGUID] UNIQUEIDENTIFIER   NOT NULL,
    [MobileDeviceProfileGUID]        UNIQUEIDENTIFIER   NOT NULL,
    [PrinterID]                      NVARCHAR (30)      NOT NULL,
    [BaudRate]                       NVARCHAR (8)       NULL,
    [COMPort]                        NVARCHAR (4)       NULL,
    [DataBits]                       NVARCHAR (8)       NULL,
    [StopBits]                       NVARCHAR (8)       NULL,
    [UseXonXoff]                     NVARCHAR (8)       NULL,
    [XonChar]                        NVARCHAR (8)       NULL,
    [XoffChar]                       NVARCHAR (8)       NULL,
    [BufferSize]                     NVARCHAR (8)       NULL,
    [Parity]                         NVARCHAR (12)      NULL,
    [CreatedBy]                      NVARCHAR (50)      CONSTRAINT [DF_tblMobileDeviceProfilePrinter_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedBy]                      NVARCHAR (50)      CONSTRAINT [DF_tblMobileDeviceProfilePrinter_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblMobileDeviceProfilePrinter_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblMobileDeviceProfilePrinter_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblMobileDeviceProfilePrinter] PRIMARY KEY NONCLUSTERED ([MobileDeviceProfilePrinterGUID] ASC),
    CONSTRAINT [FK_tblMobileDeviceProfilePrinter_tblMobileDeviceProfile] FOREIGN KEY ([MobileDeviceProfileGUID]) REFERENCES [dbo].[tblMobileDeviceProfile] ([MobileDeviceProfileGuid])
);




GO



GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblMobileDeviceProfilePrinter] ON [dbo].[tblMobileDeviceProfilePrinter] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMobileDeviceProfilePrinter','D')=1 
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
	MobileDeviceProfilePrinterGUID	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblMobileDeviceProfilePrinter (
		[MobileDeviceProfilePrinterGUID]
	,	[MobileDeviceProfileGUID]
	,	[PrinterID]
	,	[BaudRate]
	,	[COMPort]
	,	[DataBits]
	,	[StopBits]
	,	[UseXonXoff]
	,	[XonChar]
	,	[XoffChar]
	,	[BufferSize]
	,	[Parity]
	,	[CreatedBy]
	,	[UpdatedBy]
	,	[CreatedDate]
	,	[UpdatedDate]
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
	OUTPUT inserted.[MobileDeviceProfilePrinterGUID] AS 'MobileDeviceProfilePrinterGUID'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[MobileDeviceProfilePrinterGUID]
	,	d.[MobileDeviceProfileGUID]
	,	d.[PrinterID]
	,	d.[BaudRate]
	,	d.[COMPort]
	,	d.[DataBits]
	,	d.[StopBits]
	,	d.[UseXonXoff]
	,	d.[XonChar]
	,	d.[XoffChar]
	,	d.[BufferSize]
	,	d.[Parity]
	,	d.[CreatedBy]
	,	d.[UpdatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedDate]
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
 
	INSERT INTO [fmaudit].tblMobileDeviceProfilePrinter (
		[MobileDeviceProfilePrinterGUID]
	,	[MobileDeviceProfileGUID]
	,	[PrinterID]
	,	[BaudRate]
	,	[COMPort]
	,	[DataBits]
	,	[StopBits]
	,	[UseXonXoff]
	,	[XonChar]
	,	[XoffChar]
	,	[BufferSize]
	,	[Parity]
	,	[CreatedBy]
	,	[UpdatedBy]
	,	[CreatedDate]
	,	[UpdatedDate]
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
		i.[MobileDeviceProfilePrinterGUID]
	,	i.[MobileDeviceProfileGUID]
	,	i.[PrinterID]
	,	i.[BaudRate]
	,	i.[COMPort]
	,	i.[DataBits]
	,	i.[StopBits]
	,	i.[UseXonXoff]
	,	i.[XonChar]
	,	i.[XoffChar]
	,	i.[BufferSize]
	,	i.[Parity]
	,	i.[CreatedBy]
	,	i.[UpdatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedDate]
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
			agl.[MobileDeviceProfilePrinterGUID]=i.[MobileDeviceProfilePrinterGUID] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblMobileDeviceProfilePrinter
CREATE TRIGGER dbo.trg_insupd_tblMobileDeviceProfilePrinter_ForSync 
   ON dbo.tblMobileDeviceProfilePrinter
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
                    ,d.MobileDeviceProfilePrinterGUID AS Deleted_PK_MobileDeviceProfilePrinterGUID
                    ,i.MobileDeviceProfilePrinterGUID AS Inserted_PK_MobileDeviceProfilePrinterGUID
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.MobileDeviceProfilePrinterGUID = i.MobileDeviceProfilePrinterGUID
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblMobileDeviceProfilePrinter As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_MobileDeviceProfilePrinterGUID = currentTrackingData.PK_MobileDeviceProfilePrinterGUID
 
 
		    INSERT track.tblMobileDeviceProfilePrinter (InsertedDate 
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
				    ,PK_MobileDeviceProfilePrinterGUID
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
				    ,entityChanges.Inserted_PK_MobileDeviceProfilePrinterGUID
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblMobileDeviceProfilePrinter As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_MobileDeviceProfilePrinterGUID = currentTrackingData.PK_MobileDeviceProfilePrinterGUID
)
    END
END 

GO
--Creating Delete Trigger for tblMobileDeviceProfilePrinter
CREATE TRIGGER dbo.trg_del_tblMobileDeviceProfilePrinter_ForSync 
   ON dbo.tblMobileDeviceProfilePrinter
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
						,d.MobileDeviceProfilePrinterGUID AS Deleted_PK_MobileDeviceProfilePrinterGUID
                        ,d.MobileDeviceProfilePrinterGUID AS Inserted_PK_MobileDeviceProfilePrinterGUID
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblMobileDeviceProfilePrinter As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_MobileDeviceProfilePrinterGUID = currentTrackingData.PK_MobileDeviceProfilePrinterGUID
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
						,PK_MobileDeviceProfilePrinterGUID
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
						,entityChanges.Deleted_PK_MobileDeviceProfilePrinterGUID
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblMobileDeviceProfilePrinter] ON [dbo].[tblMobileDeviceProfilePrinter] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMobileDeviceProfilePrinter','D')=1 
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
	INSERT INTO [fmaudit].tblMobileDeviceProfilePrinter (
		[MobileDeviceProfilePrinterGUID]
	,	[MobileDeviceProfileGUID]
	,	[PrinterID]
	,	[BaudRate]
	,	[COMPort]
	,	[DataBits]
	,	[StopBits]
	,	[UseXonXoff]
	,	[XonChar]
	,	[XoffChar]
	,	[BufferSize]
	,	[Parity]
	,	[CreatedBy]
	,	[UpdatedBy]
	,	[CreatedDate]
	,	[UpdatedDate]
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
		d.[MobileDeviceProfilePrinterGUID]
	,	d.[MobileDeviceProfileGUID]
	,	d.[PrinterID]
	,	d.[BaudRate]
	,	d.[COMPort]
	,	d.[DataBits]
	,	d.[StopBits]
	,	d.[UseXonXoff]
	,	d.[XonChar]
	,	d.[XoffChar]
	,	d.[BufferSize]
	,	d.[Parity]
	,	d.[CreatedBy]
	,	d.[UpdatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedDate]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblMobileDeviceProfilePrinter] ON [dbo].[tblMobileDeviceProfilePrinter] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblMobileDeviceProfilePrinter','D')=1 
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
	INSERT INTO [fmaudit].tblMobileDeviceProfilePrinter (
		[MobileDeviceProfilePrinterGUID]
	,	[MobileDeviceProfileGUID]
	,	[PrinterID]
	,	[BaudRate]
	,	[COMPort]
	,	[DataBits]
	,	[StopBits]
	,	[UseXonXoff]
	,	[XonChar]
	,	[XoffChar]
	,	[BufferSize]
	,	[Parity]
	,	[CreatedBy]
	,	[UpdatedBy]
	,	[CreatedDate]
	,	[UpdatedDate]
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
		i.[MobileDeviceProfilePrinterGUID]
	,	i.[MobileDeviceProfileGUID]
	,	i.[PrinterID]
	,	i.[BaudRate]
	,	i.[COMPort]
	,	i.[DataBits]
	,	i.[StopBits]
	,	i.[UseXonXoff]
	,	i.[XonChar]
	,	i.[XoffChar]
	,	i.[BufferSize]
	,	i.[Parity]
	,	i.[CreatedBy]
	,	i.[UpdatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedDate]
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
CREATE NONCLUSTERED INDEX [IX_tblMobileDeviceProfilePrinter_CreatedDate]
    ON [dbo].[tblMobileDeviceProfilePrinter]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMobileDeviceProfilePrinter_ClusterIdx]
    ON [dbo].[tblMobileDeviceProfilePrinter]([_ClusterIdx] ASC);

