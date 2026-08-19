CREATE TABLE [dbo].[tblListViewFields] (
    [ColumnOrder]                               INT                CONSTRAINT [DF_tblListViewFields_ColumnOrder] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                               DATETIMEOFFSET (7) CONSTRAINT [DF_tblListViewFields_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                                 [dbo].[udtUserID]  CONSTRAINT [DF_tblListViewFields_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                               DATETIMEOFFSET (7) CONSTRAINT [DF_tblListViewFields_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                                 [dbo].[udtUserID]  CONSTRAINT [DF_tblListViewFields_UpdatedBy] DEFAULT ('') NOT NULL,
    [ListViewID]                                NVARCHAR (50)      CONSTRAINT [DF_tblListViewFields_ListViewID] DEFAULT ('') NOT NULL,
    [ListViewFieldGuid]                         UNIQUEIDENTIFIER   CONSTRAINT [DF_tblListViewFields_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                               ROWVERSION         NOT NULL,
    [LookupListViewFieldTypeIndex]              INT                CONSTRAINT [DF_tblListViewFields_LookupListViewFieldTypeIndex] DEFAULT ((0)) NOT NULL,
    [LookupStandardFieldTypeIndex]              INT                NULL,
    [ListViewGuid]                              UNIQUEIDENTIFIER   NOT NULL,
    [TransactionAliasGuid]                      UNIQUEIDENTIFIER   NULL,
    [TransactionAliasFieldGuid]                 UNIQUEIDENTIFIER   NULL,
    [UserDataFieldTransactionAliasGuid]         UNIQUEIDENTIFIER   NULL,
    [UserDataFieldTransactionAliasLineItemGuid] UNIQUEIDENTIFIER   NULL,
    [LedgerAggregateColumnGuid]                 UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblListViewFields_GUID] PRIMARY KEY NONCLUSTERED ([ListViewFieldGuid] ASC),
    CONSTRAINT [FK_tblListViewFields_LedgerAggregateColumnGuid] FOREIGN KEY ([LedgerAggregateColumnGuid]) REFERENCES [dbo].[tblLedgerAggregateColumns] ([LedgerAggregateColumnGuid]),
    CONSTRAINT [FK_tblListViewFields_ListViewGuid] FOREIGN KEY ([ListViewGuid]) REFERENCES [dbo].[tblListViews] ([ListViewGuid]),
    CONSTRAINT [FK_tblListViewFields_LookupStandardFieldTypeIndex] FOREIGN KEY ([LookupStandardFieldTypeIndex]) REFERENCES [lookup].[tblStandardFieldType] ([StandardFieldTypeIndex]),
    CONSTRAINT [FK_tblListViewFields_LookuptblListViewFieldType] FOREIGN KEY ([LookupListViewFieldTypeIndex]) REFERENCES [lookup].[tblListViewFieldType] ([ListViewFieldTypeIndex]),
    CONSTRAINT [FK_tblListViewFields_TransactionAliasFieldGuid] FOREIGN KEY ([TransactionAliasFieldGuid]) REFERENCES [dbo].[tblTransactionAliasFields] ([TransactionAliasFieldGuid]),
    CONSTRAINT [FK_tblListViewFields_TransactionAliasGuid] FOREIGN KEY ([TransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblListViewFields_UserDataFieldTransactionAliasGuid] FOREIGN KEY ([UserDataFieldTransactionAliasGuid]) REFERENCES [dbo].[tblUserDataFieldTransactionAlias] ([UserDataFieldTransactionAliasGuid]),
    CONSTRAINT [FK_tblListViewFields_UserDataFieldTransactionAliasLineItemGuid] FOREIGN KEY ([UserDataFieldTransactionAliasLineItemGuid]) REFERENCES [dbo].[tblUserDataFieldTransactionAliasLineItem] ([UserDataFieldTransactionAliasLineItemGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblListViewFields_CreatedDate]
    ON [dbo].[tblListViewFields]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblListViewFields_ListViewGuid_OtherFKFields]
    ON [dbo].[tblListViewFields]([ListViewGuid] ASC, [LedgerAggregateColumnGuid] ASC, [TransactionAliasFieldGuid] ASC, [UserDataFieldTransactionAliasGuid] ASC, [UserDataFieldTransactionAliasLineItemGuid] ASC, [ColumnOrder] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblListViewFields] ON [dbo].[tblListViewFields] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblListViewFields','D')=1 
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
	ListViewFieldGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblListViewFields (
		[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ListViewID]
	,	[ListViewFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupListViewFieldTypeIndex]
	,	[LookupStandardFieldTypeIndex]
	,	[ListViewGuid]
	,	[TransactionAliasGuid]
	,	[TransactionAliasFieldGuid]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[LedgerAggregateColumnGuid]
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
	OUTPUT inserted.[ListViewFieldGuid] AS 'ListViewFieldGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ColumnOrder]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ListViewID]
	,	d.[ListViewFieldGuid]
	,	d.[_RowVersion]
	,	d.[LookupListViewFieldTypeIndex]
	,	d.[LookupStandardFieldTypeIndex]
	,	d.[ListViewGuid]
	,	d.[TransactionAliasGuid]
	,	d.[TransactionAliasFieldGuid]
	,	d.[UserDataFieldTransactionAliasGuid]
	,	d.[UserDataFieldTransactionAliasLineItemGuid]
	,	d.[LedgerAggregateColumnGuid]
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
 
	INSERT INTO [fmaudit].tblListViewFields (
		[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ListViewID]
	,	[ListViewFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupListViewFieldTypeIndex]
	,	[LookupStandardFieldTypeIndex]
	,	[ListViewGuid]
	,	[TransactionAliasGuid]
	,	[TransactionAliasFieldGuid]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[LedgerAggregateColumnGuid]
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
		i.[ColumnOrder]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ListViewID]
	,	i.[ListViewFieldGuid]
	,	i.[_RowVersion]
	,	i.[LookupListViewFieldTypeIndex]
	,	i.[LookupStandardFieldTypeIndex]
	,	i.[ListViewGuid]
	,	i.[TransactionAliasGuid]
	,	i.[TransactionAliasFieldGuid]
	,	i.[UserDataFieldTransactionAliasGuid]
	,	i.[UserDataFieldTransactionAliasLineItemGuid]
	,	i.[LedgerAggregateColumnGuid]
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
			agl.[ListViewFieldGuid]=i.[ListViewFieldGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblListViewFields
CREATE TRIGGER dbo.trg_insupd_tblListViewFields_ForSync 
   ON dbo.tblListViewFields
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
                    ,d.ListViewFieldGuid AS Deleted_PK_ListViewFieldGuid
                    ,i.ListViewFieldGuid AS Inserted_PK_ListViewFieldGuid
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
				    d.ListViewFieldGuid = i.ListViewFieldGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblListViewFields As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ListViewFieldGuid = currentTrackingData.PK_ListViewFieldGuid
 
 
		    INSERT track.tblListViewFields (InsertedDate 
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
				    ,PK_ListViewFieldGuid
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
				    ,entityChanges.Inserted_PK_ListViewFieldGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblListViewFields As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ListViewFieldGuid = currentTrackingData.PK_ListViewFieldGuid
)
    END
END 

GO
--Creating Delete Trigger for tblListViewFields
CREATE TRIGGER dbo.trg_del_tblListViewFields_ForSync 
   ON dbo.tblListViewFields
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
						,d.ListViewFieldGuid AS Deleted_PK_ListViewFieldGuid
                        ,d.ListViewFieldGuid AS Inserted_PK_ListViewFieldGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblListViewFields As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ListViewFieldGuid = currentTrackingData.PK_ListViewFieldGuid
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
						,PK_ListViewFieldGuid
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
						,entityChanges.Deleted_PK_ListViewFieldGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblListViewFields] ON [dbo].[tblListViewFields] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblListViewFields','D')=1 
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
	INSERT INTO [fmaudit].tblListViewFields (
		[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ListViewID]
	,	[ListViewFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupListViewFieldTypeIndex]
	,	[LookupStandardFieldTypeIndex]
	,	[ListViewGuid]
	,	[TransactionAliasGuid]
	,	[TransactionAliasFieldGuid]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[LedgerAggregateColumnGuid]
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
		d.[ColumnOrder]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[ListViewID]
	,	d.[ListViewFieldGuid]
	,	d.[_RowVersion]
	,	d.[LookupListViewFieldTypeIndex]
	,	d.[LookupStandardFieldTypeIndex]
	,	d.[ListViewGuid]
	,	d.[TransactionAliasGuid]
	,	d.[TransactionAliasFieldGuid]
	,	d.[UserDataFieldTransactionAliasGuid]
	,	d.[UserDataFieldTransactionAliasLineItemGuid]
	,	d.[LedgerAggregateColumnGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblListViewFields] ON [dbo].[tblListViewFields] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblListViewFields','D')=1 
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
	INSERT INTO [fmaudit].tblListViewFields (
		[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[ListViewID]
	,	[ListViewFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupListViewFieldTypeIndex]
	,	[LookupStandardFieldTypeIndex]
	,	[ListViewGuid]
	,	[TransactionAliasGuid]
	,	[TransactionAliasFieldGuid]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[LedgerAggregateColumnGuid]
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
		i.[ColumnOrder]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[ListViewID]
	,	i.[ListViewFieldGuid]
	,	i.[_RowVersion]
	,	i.[LookupListViewFieldTypeIndex]
	,	i.[LookupStandardFieldTypeIndex]
	,	i.[ListViewGuid]
	,	i.[TransactionAliasGuid]
	,	i.[TransactionAliasFieldGuid]
	,	i.[UserDataFieldTransactionAliasGuid]
	,	i.[UserDataFieldTransactionAliasLineItemGuid]
	,	i.[LedgerAggregateColumnGuid]
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
CREATE UNIQUE CLUSTERED INDEX [IX_tblListViewFields_ClusterIdx]
    ON [dbo].[tblListViewFields]([_ClusterIdx] ASC);

