CREATE TABLE [dbo].[tblDispatchGrid] (
    [DispatchGridGuid]            UNIQUEIDENTIFIER   CONSTRAINT [DF_tblDispatchGrid_GUID] DEFAULT (newid()) NOT NULL,
    [DispatchConfigurationGuid]   UNIQUEIDENTIFIER   NOT NULL,
    [LookupDispatchGridTypeIndex] INT                NOT NULL,
    [ID]                          NVARCHAR (50)      CONSTRAINT [DF_tblDispatchGrid_ID] DEFAULT ('') NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblDispatchGrid_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblDispatchGrid_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblDispatchGrid_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblDispatchGrid_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblDispatchGrid_GUID] PRIMARY KEY NONCLUSTERED ([DispatchGridGuid] ASC),
    CONSTRAINT [FK_tblDispatchGrid_DispatchConfigurationGuid] FOREIGN KEY ([DispatchConfigurationGuid]) REFERENCES [dbo].[tblDispatchConfiguration] ([DispatchConfigurationGuid]),
    CONSTRAINT [FK_tblDispatchGrid_LookupDispatchGridTypeIndex] FOREIGN KEY ([LookupDispatchGridTypeIndex]) REFERENCES [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblDispatchGrid_CreatedDate]
    ON [dbo].[tblDispatchGrid]([CreatedDate] ASC);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblDispatchGrid_ID_DispatchConfigurationGuid]
    ON [dbo].[tblDispatchGrid]([ID] ASC, [DispatchConfigurationGuid] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblDispatchGrid] ON [dbo].[tblDispatchGrid] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchGrid','D')=1 
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
	INSERT INTO [fmaudit].tblDispatchGrid (
		[DispatchGridGuid]
	,	[DispatchConfigurationGuid]
	,	[LookupDispatchGridTypeIndex]
	,	[ID]
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
		d.[DispatchGridGuid]
	,	d.[DispatchConfigurationGuid]
	,	d.[LookupDispatchGridTypeIndex]
	,	d.[ID]
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
/****** Object:  Trigger [dbo].[trg_insupd_tblDispatchGrid_ForSync]    Script Date: 6/3/2014 2:44:20 PM ******/
CREATE TRIGGER [dbo].[trg_insupd_tblDispatchGrid_ForSync] 
   ON [dbo].[tblDispatchGrid]
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

	SET @syncContext = dbo.udf_GetSyncContext(); 
    SET @currentDateTimeOffset = sysdatetimeoffset(); 

    IF (([track].[udf_IsInsertChangeTrackingEnabled](@bypassTrackingFlags) = 1) OR ([track].[udf_IsUpdateChangeTrackingEnabled](@bypassTrackingFlags) = 1))
    BEGIN 
	    ; WITH ChangeList AS ( 
		    SELECT @syncContext AS ChangeContext 
				    ,d.DispatchGridGuid AS Deleted_PK_DispatchGridGuid
				    ,i.DispatchGridGuid AS Inserted_PK_DispatchGridGuid
				    ,i.CreatedDate AS Inserted_CreatedDate 
				    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,NULL AS CurrentSiteGuid 
				    ,NULL AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,MIN_ACTIVE_ROWVERSION() - 1 AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.DispatchGridGuid = i.DispatchGridGuid
		    ) 
		    MERGE INTO track.tblDispatchGrid As ct 
			    USING ChangeList As src 
				    ON src.Inserted_PK_DispatchGridGuid = ct.PK_DispatchGridGuid
		    WHEN Matched  AND src.CurrentSiteGuid = ct.CurrentSiteGuid 
		    THEN 
		    UPDATE SET UpdatedDate = src.Inserted_UpdatedDate 
					    ,UpdatedContext = src.ChangeContext 
 					    ,UpdatedRowVersion = src.Inserted_RowVersion 
 					    ,CurrentSiteGuid = src.CurrentSiteGuid 
 					    ,PreviousSiteGuid = ct.PreviousSiteGuid 
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
				    ,PK_DispatchGridGuid
		    )
		    VALUES (CASE WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
				    ,src.ChangeContext 
				    ,src.Inserted_RowVersion 
				    ,src.Inserted_CreatedDate 
				    ,src.ChangeContext 
				    ,src.Inserted_RowVersion 
				    ,NULL 
				    ,NULL 
				    ,NULL 
				    ,src.CurrentSiteGuid 
				    ,CASE WHEN (src.PreviousSiteGuid <> src.CurrentSiteGuid) THEN src.PreviousSiteGuid ELSE NULL END 
				    ,src.Inserted_PK_DispatchGridGuid
		    );
    END
END 

GO
/****** Object:  Trigger [dbo].[trg_del_tblDispatchGrid_ForSync]    Script Date: 6/3/2014 2:44:20 PM ******/
CREATE TRIGGER [dbo].[trg_del_tblDispatchGrid_ForSync] 
   ON [dbo].[tblDispatchGrid]
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

    SET @syncContext = dbo.udf_GetSyncContext(); 
    SET @currentDateTimeOffset = sysdatetimeoffset(); 

    IF ([track].[udf_IsDeleteChangeTrackingEnabled](@bypassTrackingFlags) = 1)
    BEGIN
		  ; WITH ChangeList AS ( 
				SELECT @syncContext AS ChangeContext 
						,d.DispatchGridGuid AS Deleted_PK_DispatchGridGuid
						,d.DispatchGridGuid AS Inserted_PK_DispatchGridGuid
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,MIN_ACTIVE_ROWVERSION() - 1 AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblDispatchGrid As ct 
					USING ChangeList As src 
						ON src.Deleted_PK_DispatchGridGuid = ct.PK_DispatchGridGuid
				WHEN Matched 
				THEN 
					UPDATE SET DeletedDate = @currentDateTimeOffset 
								,DeletedContext = src.ChangeContext 
 								,DeletedRowVersion = src.Deleted_RowVersion 
 								,CurrentSiteGuid = src.CurrentSiteGuid 
 								,PreviousSiteGuid = CASE WHEN (src.CurrentSiteGuid <> ct.CurrentSiteGuid) THEN ct.CurrentSiteGuid ELSE ct.PreviousSiteGuid END
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
						,PK_DispatchGridGuid
				)
				VALUES (CASE WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ELSE CAST('1/1/1990' AS DateTimeOffset(7)) END 
						,src.ChangeContext 
						,src.Inserted_RowVersion 
						,NULL 
						,NULL 
						,NULL 
						,src.CurrentSiteGuid 
						,NULL 
						,@currentDateTimeOffset 
						,src.ChangeContext 
						,src.Deleted_RowVersion
						,src.Deleted_PK_DispatchGridGuid
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblDispatchGrid] ON [dbo].[tblDispatchGrid] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchGrid','D')=1 
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
	INSERT INTO [fmaudit].tblDispatchGrid (
		[DispatchGridGuid]
	,	[DispatchConfigurationGuid]
	,	[LookupDispatchGridTypeIndex]
	,	[ID]
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
		i.[DispatchGridGuid]
	,	i.[DispatchConfigurationGuid]
	,	i.[LookupDispatchGridTypeIndex]
	,	i.[ID]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblDispatchGrid] ON [dbo].[tblDispatchGrid] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchGrid','D')=1 
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
	DispatchGridGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblDispatchGrid (
		[DispatchGridGuid]
	,	[DispatchConfigurationGuid]
	,	[LookupDispatchGridTypeIndex]
	,	[ID]
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
	OUTPUT inserted.[DispatchGridGuid] AS 'DispatchGridGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[DispatchGridGuid]
	,	d.[DispatchConfigurationGuid]
	,	d.[LookupDispatchGridTypeIndex]
	,	d.[ID]
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
 
	INSERT INTO [fmaudit].tblDispatchGrid (
		[DispatchGridGuid]
	,	[DispatchConfigurationGuid]
	,	[LookupDispatchGridTypeIndex]
	,	[ID]
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
		i.[DispatchGridGuid]
	,	i.[DispatchConfigurationGuid]
	,	i.[LookupDispatchGridTypeIndex]
	,	i.[ID]
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
			agl.[DispatchGridGuid]=i.[DispatchGridGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDispatchGrid_ClusterIdx]
    ON [dbo].[tblDispatchGrid]([_ClusterIdx] ASC);

