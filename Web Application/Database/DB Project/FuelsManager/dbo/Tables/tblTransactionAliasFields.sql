CREATE TABLE [dbo].[tblTransactionAliasFields] (
    [AliasID]                         INT                CONSTRAINT [DF_tblTransactionAliasFields_AliasID] DEFAULT ((0)) NOT NULL,
    [DbName]                          NVARCHAR (50)      CONSTRAINT [DF_tblTransactionAliasFields_DbName] DEFAULT ('') NOT NULL,
    [DisplayOrder]                    INT                CONSTRAINT [DF_tblTransactionAliasFields_DisplayOrder] DEFAULT ((0)) NOT NULL,
    [DisplayName]                     NVARCHAR (50)      CONSTRAINT [DF_tblTransactionAliasFields_DisplayName] DEFAULT ('') NOT NULL,
    [CreatedDate]                     DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionAliasFields_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                       [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionAliasFields_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                     DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionAliasFields_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                       [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionAliasFields_UpdatedBy] DEFAULT ('') NOT NULL,
    [Required]                        BIT                NULL,
    [Virtual]                         BIT                CONSTRAINT [DF_tblTransactionAliasFields_Virtual] DEFAULT ((0)) NOT NULL,
    [TransactionAliasFieldGuid]       UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionAliasFields_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                     ROWVERSION         NOT NULL,
    [LookupTransactionFieldTypeIndex] INT                CONSTRAINT [DF_tblTransactionAliasFields_LookupTransactionFieldTypeIndex] DEFAULT ((0)) NOT NULL,
    [TransactionAliasGuid]            UNIQUEIDENTIFIER   NOT NULL,
    [UserGroupGuid]                   UNIQUEIDENTIFIER   NULL,
    [DispatchField]                   BIT                CONSTRAINT [DF_tblTransactionAliasFields_DispatchField] DEFAULT ((0)) NOT NULL,
    [ClearOnNew]                      BIT                CONSTRAINT [DF_tblTransactionAliasFields_ClearOnNew] DEFAULT ((0)) NOT NULL,
    [_ClusterIdx]                     BIGINT             IDENTITY (1, 1) NOT NULL,
    [ReadOnly]                        BIT                CONSTRAINT [DF_tblTransactionAliasFields_ReadOnly] DEFAULT ((0)) NOT NULL,
    [Visibility]                      INT                CONSTRAINT [DF_tblTransactionAliasFields_Visibility] DEFAULT ((0)) NOT NULL,
	[DefaultValueType]				  NVARCHAR (MAX)	 NULL,
	[DefaultValue]					  XML				 NULL,
    CONSTRAINT [PK_tblTransactionAliasFields_GUID] PRIMARY KEY NONCLUSTERED ([TransactionAliasFieldGuid] ASC),
    CONSTRAINT [FK_tblTransactionAliasFields_LookupTransactionFieldTypeIndex] FOREIGN KEY ([LookupTransactionFieldTypeIndex]) REFERENCES [lookup].[tblTransactionFieldType] ([TransactionFieldTypeIndex]),
    CONSTRAINT [FK_tblTransactionAliasFields_TransactionAliasGuid] FOREIGN KEY ([TransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid]),
    CONSTRAINT [FK_tblTransactionAliasFields_UserGroupGuid] FOREIGN KEY ([UserGroupGuid]) REFERENCES [dbo].[tblGroups] ([GroupGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasFields_CreatedDate]
    ON [dbo].[tblTransactionAliasFields]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionAliasFields_TransactionAliasGuid_LookupTransactionFieldTypeIndex_DispatchField]
    ON [dbo].[tblTransactionAliasFields]([TransactionAliasGuid] ASC, [LookupTransactionFieldTypeIndex] ASC, [DispatchField] ASC);


GO

--Creating Insert / Update Trigger for tblTransactionAliasFields
CREATE TRIGGER dbo.trg_insupd_tblTransactionAliasFields_ForSync 
   ON dbo.tblTransactionAliasFields
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
                    ,d.TransactionAliasFieldGuid AS Deleted_PK_TransactionAliasFieldGuid
                    ,i.TransactionAliasFieldGuid AS Inserted_PK_TransactionAliasFieldGuid
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
				    d.TransactionAliasFieldGuid = i.TransactionAliasFieldGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionAliasFields As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionAliasFieldGuid = currentTrackingData.PK_TransactionAliasFieldGuid
 
 
		    INSERT track.tblTransactionAliasFields (InsertedDate 
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
				    ,PK_TransactionAliasFieldGuid
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
				    ,entityChanges.Inserted_PK_TransactionAliasFieldGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionAliasFields As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionAliasFieldGuid = currentTrackingData.PK_TransactionAliasFieldGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionAliasFields
CREATE TRIGGER dbo.trg_del_tblTransactionAliasFields_ForSync 
   ON dbo.tblTransactionAliasFields
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
						,d.TransactionAliasFieldGuid AS Deleted_PK_TransactionAliasFieldGuid
                        ,d.TransactionAliasFieldGuid AS Inserted_PK_TransactionAliasFieldGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionAliasFields As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionAliasFieldGuid = currentTrackingData.PK_TransactionAliasFieldGuid
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
						,PK_TransactionAliasFieldGuid
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
						,entityChanges.Deleted_PK_TransactionAliasFieldGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO

-------------------------------------
-- AUDIT INSERT TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionAliasFields] ON [dbo].[tblTransactionAliasFields] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionAliasFields','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionAliasFields (
		[AliasID]
	,	[DbName]
	,	[DisplayOrder]
	,	[DisplayName]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[Required]
	,	[Virtual]
	,	[TransactionAliasFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionFieldTypeIndex]
	,	[TransactionAliasGuid]
	,	[UserGroupGuid]
	,	[DispatchField]
	,	[ClearOnNew]
	,	[ReadOnly]
	,	[Visibility]
	,	[DefaultValueType]
	,	[DefaultValue]
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
		i.[AliasID]
	,	i.[DbName]
	,	i.[DisplayOrder]
	,	i.[DisplayName]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[Required]
	,	i.[Virtual]
	,	i.[TransactionAliasFieldGuid]
	,	i.[_RowVersion]
	,	i.[LookupTransactionFieldTypeIndex]
	,	i.[TransactionAliasGuid]
	,	i.[UserGroupGuid]
	,	i.[DispatchField]
	,	i.[ClearOnNew]
	,	i.[ReadOnly]
	,	i.[Visibility]
	,	i.[DefaultValueType]
	,	i.[DefaultValue]
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
 
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionAliasFields] ON [dbo].[tblTransactionAliasFields] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionAliasFields','D')=1 
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
	TransactionAliasFieldGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionAliasFields (
		[AliasID]
	,	[DbName]
	,	[DisplayOrder]
	,	[DisplayName]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[Required]
	,	[Virtual]
	,	[TransactionAliasFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionFieldTypeIndex]
	,	[TransactionAliasGuid]
	,	[UserGroupGuid]
	,	[DispatchField]
	,	[ClearOnNew]
	,	[ReadOnly]
	,	[Visibility]
	,	[DefaultValueType]
	,	[DefaultValue]
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
	OUTPUT inserted.[TransactionAliasFieldGuid] AS 'TransactionAliasFieldGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[AliasID]
	,	d.[DbName]
	,	d.[DisplayOrder]
	,	d.[DisplayName]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[Required]
	,	d.[Virtual]
	,	d.[TransactionAliasFieldGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionFieldTypeIndex]
	,	d.[TransactionAliasGuid]
	,	d.[UserGroupGuid]
	,	d.[DispatchField]
	,	d.[ClearOnNew]
	,	d.[ReadOnly]
	,	d.[Visibility]
	,	d.[DefaultValueType]
	,	d.[DefaultValue]
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
 
	INSERT INTO [fmaudit].tblTransactionAliasFields (
		[AliasID]
	,	[DbName]
	,	[DisplayOrder]
	,	[DisplayName]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[Required]
	,	[Virtual]
	,	[TransactionAliasFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionFieldTypeIndex]
	,	[TransactionAliasGuid]
	,	[UserGroupGuid]
	,	[DispatchField]
	,	[ClearOnNew]
	,	[ReadOnly]
	,	[Visibility]
	,	[DefaultValueType]
	,	[DefaultValue]
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
		i.[AliasID]
	,	i.[DbName]
	,	i.[DisplayOrder]
	,	i.[DisplayName]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[Required]
	,	i.[Virtual]
	,	i.[TransactionAliasFieldGuid]
	,	i.[_RowVersion]
	,	i.[LookupTransactionFieldTypeIndex]
	,	i.[TransactionAliasGuid]
	,	i.[UserGroupGuid]
	,	i.[DispatchField]
	,	i.[ClearOnNew]
	,	i.[ReadOnly]
	,	i.[Visibility]
	,	i.[DefaultValueType]
	,	i.[DefaultValue]
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
			agl.[TransactionAliasFieldGuid]=i.[TransactionAliasFieldGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO
 
-------------------------------------
-- AUDIT DELETE TRIGGERS
-------------------------------------
 
CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionAliasFields] ON [dbo].[tblTransactionAliasFields] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionAliasFields','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionAliasFields (
		[AliasID]
	,	[DbName]
	,	[DisplayOrder]
	,	[DisplayName]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[Required]
	,	[Virtual]
	,	[TransactionAliasFieldGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionFieldTypeIndex]
	,	[TransactionAliasGuid]
	,	[UserGroupGuid]
	,	[DispatchField]
	,	[ClearOnNew]
	,	[ReadOnly]
	,	[Visibility]
	,	[DefaultValueType]
	,	[DefaultValue]
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
		d.[AliasID]
	,	d.[DbName]
	,	d.[DisplayOrder]
	,	d.[DisplayName]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[Required]
	,	d.[Virtual]
	,	d.[TransactionAliasFieldGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionFieldTypeIndex]
	,	d.[TransactionAliasGuid]
	,	d.[UserGroupGuid]
	,	d.[DispatchField]
	,	d.[ClearOnNew]
	,	d.[ReadOnly]
	,	d.[Visibility]
	,	d.[DefaultValueType]
	,	d.[DefaultValue]
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
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionAliasFields_ClusterIdx]
    ON [dbo].[tblTransactionAliasFields]([_ClusterIdx] ASC);

