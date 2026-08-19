CREATE TABLE [dbo].[tblTransactionTransportLineItems] (
    [TransportOrderNumber]             NVARCHAR (50)      CONSTRAINT [DF_tblTransactionTransportLineItems_TransportOrderNumber] DEFAULT ('') NOT NULL,
    [TransVersion]                     BIGINT             NULL,
    [LocationName]                     NVARCHAR (30)      NULL,
    [Address1]                         NVARCHAR (60)      NULL,
    [Address2]                         NVARCHAR (60)      NULL,
    [City]                             NVARCHAR (60)      NULL,
    [State]                            NVARCHAR (20)      NULL,
    [Zip]                              NVARCHAR (11)      NULL,
    [POCName]                          NVARCHAR (50)      NULL,
    [POCPhone]                         NVARCHAR (20)      NULL,
    [CreatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionTransportLineItems_CreatedBy] DEFAULT ('') NOT NULL,
    [CreatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionTransportLineItems_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                        [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionTransportLineItems_UpdatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                      DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionTransportLineItems_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TransactionTransportLineItemGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionTransportLineItems_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                      ROWVERSION         NOT NULL,
    [TransactionGuid]                  UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]                      BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionTransportLineItems_GUID] PRIMARY KEY NONCLUSTERED ([TransactionTransportLineItemGuid] ASC),
    CONSTRAINT [FK_tblTransactionTransportLineItems_TransactionGuid] FOREIGN KEY ([TransactionGuid]) REFERENCES [dbo].[tblTransactions] ([TransactionGuid]),
    CONSTRAINT [IX_tblTransactionTransportLineItems_UniqueTransportOrderNumber] UNIQUE NONCLUSTERED ([TransportOrderNumber] ASC, [TransactionGuid] ASC)
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionTransportLineItems_ClusterIdx] 
	ON [dbo].[tblTransactionTransportLineItems]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionTransportLineItems_CreatedDate]
    ON [dbo].[tblTransactionTransportLineItems]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionTransportLineItems_TransactionGuid_TransportOrderNumber]
    ON [dbo].[tblTransactionTransportLineItems]([TransactionGuid] ASC, [TransportOrderNumber] ASC)
    INCLUDE([TransactionTransportLineItemGuid]);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionTransportLineItems] ON [dbo].[tblTransactionTransportLineItems] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionTransportLineItems','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionTransportLineItems (
		[TransportOrderNumber]
	,	[TransVersion]
	,	[LocationName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[POCName]
	,	[POCPhone]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionTransportLineItemGuid]
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
		d.[TransportOrderNumber]
	,	d.[TransVersion]
	,	d.[LocationName]
	,	d.[Address1]
	,	d.[Address2]
	,	d.[City]
	,	d.[State]
	,	d.[Zip]
	,	d.[POCName]
	,	d.[POCPhone]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionTransportLineItemGuid]
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
--Creating Insert / Update Trigger for tblTransactionTransportLineItems
CREATE TRIGGER dbo.trg_insupd_tblTransactionTransportLineItems_ForSync 
   ON dbo.tblTransactionTransportLineItems
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
                    ,d.TransactionTransportLineItemGuid AS Deleted_PK_TransactionTransportLineItemGuid
                    ,i.TransactionTransportLineItemGuid AS Inserted_PK_TransactionTransportLineItemGuid
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
				    d.TransactionTransportLineItemGuid = i.TransactionTransportLineItemGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionTransportLineItems As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionTransportLineItemGuid = currentTrackingData.PK_TransactionTransportLineItemGuid
 
 
		    INSERT track.tblTransactionTransportLineItems (InsertedDate 
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
				    ,PK_TransactionTransportLineItemGuid
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
				    ,entityChanges.Inserted_PK_TransactionTransportLineItemGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionTransportLineItems As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionTransportLineItemGuid = currentTrackingData.PK_TransactionTransportLineItemGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionTransportLineItems
CREATE TRIGGER dbo.trg_del_tblTransactionTransportLineItems_ForSync 
   ON dbo.tblTransactionTransportLineItems
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
						,d.TransactionTransportLineItemGuid AS Deleted_PK_TransactionTransportLineItemGuid
                        ,d.TransactionTransportLineItemGuid AS Inserted_PK_TransactionTransportLineItemGuid
                      ,d.TransactionGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionTransportLineItems As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionTransportLineItemGuid = currentTrackingData.PK_TransactionTransportLineItemGuid
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
						,PK_TransactionTransportLineItemGuid
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
						,entityChanges.Deleted_PK_TransactionTransportLineItemGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionTransportLineItems] ON [dbo].[tblTransactionTransportLineItems] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionTransportLineItems','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionTransportLineItems (
		[TransportOrderNumber]
	,	[TransVersion]
	,	[LocationName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[POCName]
	,	[POCPhone]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionTransportLineItemGuid]
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
		i.[TransportOrderNumber]
	,	i.[TransVersion]
	,	i.[LocationName]
	,	i.[Address1]
	,	i.[Address2]
	,	i.[City]
	,	i.[State]
	,	i.[Zip]
	,	i.[POCName]
	,	i.[POCPhone]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[TransactionTransportLineItemGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionTransportLineItems] ON [dbo].[tblTransactionTransportLineItems] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionTransportLineItems','D')=1 
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
	TransactionTransportLineItemGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionTransportLineItems (
		[TransportOrderNumber]
	,	[TransVersion]
	,	[LocationName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[POCName]
	,	[POCPhone]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionTransportLineItemGuid]
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
	OUTPUT inserted.[TransactionTransportLineItemGuid] AS 'TransactionTransportLineItemGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[TransportOrderNumber]
	,	d.[TransVersion]
	,	d.[LocationName]
	,	d.[Address1]
	,	d.[Address2]
	,	d.[City]
	,	d.[State]
	,	d.[Zip]
	,	d.[POCName]
	,	d.[POCPhone]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionTransportLineItemGuid]
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
 
	INSERT INTO [fmaudit].tblTransactionTransportLineItems (
		[TransportOrderNumber]
	,	[TransVersion]
	,	[LocationName]
	,	[Address1]
	,	[Address2]
	,	[City]
	,	[State]
	,	[Zip]
	,	[POCName]
	,	[POCPhone]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionTransportLineItemGuid]
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
		i.[TransportOrderNumber]
	,	i.[TransVersion]
	,	i.[LocationName]
	,	i.[Address1]
	,	i.[Address2]
	,	i.[City]
	,	i.[State]
	,	i.[Zip]
	,	i.[POCName]
	,	i.[POCPhone]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[TransactionTransportLineItemGuid]
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
			agl.[TransactionTransportLineItemGuid]=i.[TransactionTransportLineItemGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
