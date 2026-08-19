CREATE TABLE [dbo].[tblProcessVariableRecipeInputPermissive] (
    [ProcessVariableProductToPresetRecipeGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblProcessVariableRecipeInputPermissive_GUID] DEFAULT (newid()) NOT NULL,
    [LookupProcessVariableTypeIndex]           INT                CONSTRAINT [DF_tblProcessVariableRecipeInputPermissive_LookupProcessVariableType] DEFAULT ((0)) NOT NULL,
    [InstanceNumber]                           INT                NOT NULL,
    [ProductToPresetRecipeGuid]                UNIQUEIDENTIFIER   NOT NULL,
    [OPCConnectionGuid]                        UNIQUEIDENTIFIER   NULL,
    [OPCItemID]                                NVARCHAR (255)     NULL,
    [DataType]                                 INT                NULL,
    [ServerEngineeringUnitsIndex]              INT                NULL,
    [Quality]                                  SMALLINT           NULL,
    [SIValue]                                  VARBINARY (MAX)    NULL,
    [LookupSIValueVariantTypeIndex]            INT                NULL,
    [DateTimeStamp]                            DATETIMEOFFSET (7) NULL,
    [Maximum]                                  VARBINARY (MAX)    NULL,
    [LookupMaximumVariantTypeIndex]            INT                NULL,
    [Minimum]                                  VARBINARY (MAX)    NULL,
    [LookupMinimumVariantTypeIndex]            INT                NULL,
    [DataTypeEnabled]                          BIT                NULL,
    [Input]                                    BIT                NULL,
    [InputEnabled]                             BIT                NULL,
    [MessageApplicationStringGuid]             UNIQUEIDENTIFIER   NULL,
    [CreatedDate]                              DATETIMEOFFSET (7) CONSTRAINT [DF_tblProcessVariableRecipeInputPermissive_CreatedDate] DEFAULT (getdate()) NULL,
    [CreatedBy]                                [dbo].[udtUserID]  CONSTRAINT [DF_tblProcessVariableRecipeInputPermissive_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                              DATETIMEOFFSET (7) CONSTRAINT [DF_tblProcessVariableRecipeInputPermissive_UpdatedDate] DEFAULT (getdate()) NULL,
    [UpdatedBy]                                [dbo].[udtUserID]  CONSTRAINT [DF_tblProcessVariableRecipeInputPermissive_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                              ROWVERSION         NOT NULL,
    [_ClusterIdx]                              BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblProcessVariableRecipeInputPermissive_GUID] PRIMARY KEY NONCLUSTERED ([ProcessVariableProductToPresetRecipeGuid] ASC),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_LookupProcessVariableType] FOREIGN KEY ([LookupProcessVariableTypeIndex]) REFERENCES [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_Maximum_DataType] FOREIGN KEY ([LookupMaximumVariantTypeIndex]) REFERENCES [lookup].[tblVariantType] ([VariantTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_MessageApplicationStringGuid] FOREIGN KEY ([MessageApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_Minimum_DataType] FOREIGN KEY ([LookupMinimumVariantTypeIndex]) REFERENCES [lookup].[tblVariantType] ([VariantTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_OPCConnectionGuid] FOREIGN KEY ([OPCConnectionGuid]) REFERENCES [dbo].[tblOPCConnections] ([OPCConnectionGuid]),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_ProductToPresetRecipeGuid] FOREIGN KEY ([ProductToPresetRecipeGuid]) REFERENCES [map].[tblProductToPresetRecipe] ([ProductToPresetRecipeGuid]),
    CONSTRAINT [FK_tblProcessVariableRecipeInputPermissive_SIValue_DataType] FOREIGN KEY ([LookupSIValueVariantTypeIndex]) REFERENCES [lookup].[tblVariantType] ([VariantTypeIndex])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableRecipeInputPermissive_CreatedDate]
    ON [dbo].[tblProcessVariableRecipeInputPermissive]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblProcessVariableRecipeInputPermissive] ON [dbo].[tblProcessVariableRecipeInputPermissive] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProcessVariableRecipeInputPermissive','D')=1 
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
	INSERT INTO [fmaudit].tblProcessVariableRecipeInputPermissive (
		[ProcessVariableProductToPresetRecipeGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[ProductToPresetRecipeGuid]
	,	[OPCConnectionGuid]
	,	[OPCItemID]
	,	[DataType]
	,	[ServerEngineeringUnitsIndex]
	,	[Quality]
	,	[SIValue]
	,	[LookupSIValueVariantTypeIndex]
	,	[DateTimeStamp]
	,	[Maximum]
	,	[LookupMaximumVariantTypeIndex]
	,	[Minimum]
	,	[LookupMinimumVariantTypeIndex]
	,	[DataTypeEnabled]
	,	[Input]
	,	[InputEnabled]
	,	[MessageApplicationStringGuid]
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
		i.[ProcessVariableProductToPresetRecipeGuid]
	,	i.[LookupProcessVariableTypeIndex]
	,	i.[InstanceNumber]
	,	i.[ProductToPresetRecipeGuid]
	,	i.[OPCConnectionGuid]
	,	i.[OPCItemID]
	,	i.[DataType]
	,	i.[ServerEngineeringUnitsIndex]
	,	i.[Quality]
	,	i.[SIValue]
	,	i.[LookupSIValueVariantTypeIndex]
	,	i.[DateTimeStamp]
	,	i.[Maximum]
	,	i.[LookupMaximumVariantTypeIndex]
	,	i.[Minimum]
	,	i.[LookupMinimumVariantTypeIndex]
	,	i.[DataTypeEnabled]
	,	i.[Input]
	,	i.[InputEnabled]
	,	i.[MessageApplicationStringGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblProcessVariableRecipeInputPermissive] ON [dbo].[tblProcessVariableRecipeInputPermissive] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProcessVariableRecipeInputPermissive','D')=1 
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
	ProcessVariableProductToPresetRecipeGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblProcessVariableRecipeInputPermissive (
		[ProcessVariableProductToPresetRecipeGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[ProductToPresetRecipeGuid]
	,	[OPCConnectionGuid]
	,	[OPCItemID]
	,	[DataType]
	,	[ServerEngineeringUnitsIndex]
	,	[Quality]
	,	[SIValue]
	,	[LookupSIValueVariantTypeIndex]
	,	[DateTimeStamp]
	,	[Maximum]
	,	[LookupMaximumVariantTypeIndex]
	,	[Minimum]
	,	[LookupMinimumVariantTypeIndex]
	,	[DataTypeEnabled]
	,	[Input]
	,	[InputEnabled]
	,	[MessageApplicationStringGuid]
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
	OUTPUT inserted.[ProcessVariableProductToPresetRecipeGuid] AS 'ProcessVariableProductToPresetRecipeGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ProcessVariableProductToPresetRecipeGuid]
	,	d.[LookupProcessVariableTypeIndex]
	,	d.[InstanceNumber]
	,	d.[ProductToPresetRecipeGuid]
	,	d.[OPCConnectionGuid]
	,	d.[OPCItemID]
	,	d.[DataType]
	,	d.[ServerEngineeringUnitsIndex]
	,	d.[Quality]
	,	d.[SIValue]
	,	d.[LookupSIValueVariantTypeIndex]
	,	d.[DateTimeStamp]
	,	d.[Maximum]
	,	d.[LookupMaximumVariantTypeIndex]
	,	d.[Minimum]
	,	d.[LookupMinimumVariantTypeIndex]
	,	d.[DataTypeEnabled]
	,	d.[Input]
	,	d.[InputEnabled]
	,	d.[MessageApplicationStringGuid]
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
 
	INSERT INTO [fmaudit].tblProcessVariableRecipeInputPermissive (
		[ProcessVariableProductToPresetRecipeGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[ProductToPresetRecipeGuid]
	,	[OPCConnectionGuid]
	,	[OPCItemID]
	,	[DataType]
	,	[ServerEngineeringUnitsIndex]
	,	[Quality]
	,	[SIValue]
	,	[LookupSIValueVariantTypeIndex]
	,	[DateTimeStamp]
	,	[Maximum]
	,	[LookupMaximumVariantTypeIndex]
	,	[Minimum]
	,	[LookupMinimumVariantTypeIndex]
	,	[DataTypeEnabled]
	,	[Input]
	,	[InputEnabled]
	,	[MessageApplicationStringGuid]
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
		i.[ProcessVariableProductToPresetRecipeGuid]
	,	i.[LookupProcessVariableTypeIndex]
	,	i.[InstanceNumber]
	,	i.[ProductToPresetRecipeGuid]
	,	i.[OPCConnectionGuid]
	,	i.[OPCItemID]
	,	i.[DataType]
	,	i.[ServerEngineeringUnitsIndex]
	,	i.[Quality]
	,	i.[SIValue]
	,	i.[LookupSIValueVariantTypeIndex]
	,	i.[DateTimeStamp]
	,	i.[Maximum]
	,	i.[LookupMaximumVariantTypeIndex]
	,	i.[Minimum]
	,	i.[LookupMinimumVariantTypeIndex]
	,	i.[DataTypeEnabled]
	,	i.[Input]
	,	i.[InputEnabled]
	,	i.[MessageApplicationStringGuid]
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
			agl.[ProcessVariableProductToPresetRecipeGuid]=i.[ProcessVariableProductToPresetRecipeGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblProcessVariableRecipeInputPermissive] ON [dbo].[tblProcessVariableRecipeInputPermissive] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProcessVariableRecipeInputPermissive','D')=1 
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
	INSERT INTO [fmaudit].tblProcessVariableRecipeInputPermissive (
		[ProcessVariableProductToPresetRecipeGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[ProductToPresetRecipeGuid]
	,	[OPCConnectionGuid]
	,	[OPCItemID]
	,	[DataType]
	,	[ServerEngineeringUnitsIndex]
	,	[Quality]
	,	[SIValue]
	,	[LookupSIValueVariantTypeIndex]
	,	[DateTimeStamp]
	,	[Maximum]
	,	[LookupMaximumVariantTypeIndex]
	,	[Minimum]
	,	[LookupMinimumVariantTypeIndex]
	,	[DataTypeEnabled]
	,	[Input]
	,	[InputEnabled]
	,	[MessageApplicationStringGuid]
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
		d.[ProcessVariableProductToPresetRecipeGuid]
	,	d.[LookupProcessVariableTypeIndex]
	,	d.[InstanceNumber]
	,	d.[ProductToPresetRecipeGuid]
	,	d.[OPCConnectionGuid]
	,	d.[OPCItemID]
	,	d.[DataType]
	,	d.[ServerEngineeringUnitsIndex]
	,	d.[Quality]
	,	d.[SIValue]
	,	d.[LookupSIValueVariantTypeIndex]
	,	d.[DateTimeStamp]
	,	d.[Maximum]
	,	d.[LookupMaximumVariantTypeIndex]
	,	d.[Minimum]
	,	d.[LookupMinimumVariantTypeIndex]
	,	d.[DataTypeEnabled]
	,	d.[Input]
	,	d.[InputEnabled]
	,	d.[MessageApplicationStringGuid]
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
--Creating Insert / Update Trigger for tblProcessVariableRecipeInputPermissive
CREATE TRIGGER dbo.trg_insupd_tblProcessVariableRecipeInputPermissive_ForSync 
   ON dbo.tblProcessVariableRecipeInputPermissive
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
                    ,d.ProcessVariableProductToPresetRecipeGuid AS Deleted_PK_ProcessVariableProductToPresetRecipeGuid
                    ,i.ProcessVariableProductToPresetRecipeGuid AS Inserted_PK_ProcessVariableProductToPresetRecipeGuid
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
				    d.ProcessVariableProductToPresetRecipeGuid = i.ProcessVariableProductToPresetRecipeGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblProcessVariableRecipeInputPermissive As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ProcessVariableProductToPresetRecipeGuid = currentTrackingData.PK_ProcessVariableProductToPresetRecipeGuid
 
 
		    INSERT track.tblProcessVariableRecipeInputPermissive (InsertedDate 
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
				    ,PK_ProcessVariableProductToPresetRecipeGuid
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
				    ,entityChanges.Inserted_PK_ProcessVariableProductToPresetRecipeGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblProcessVariableRecipeInputPermissive As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ProcessVariableProductToPresetRecipeGuid = currentTrackingData.PK_ProcessVariableProductToPresetRecipeGuid
)
    END
END 

GO
--Creating Delete Trigger for tblProcessVariableRecipeInputPermissive
CREATE TRIGGER dbo.trg_del_tblProcessVariableRecipeInputPermissive_ForSync 
   ON dbo.tblProcessVariableRecipeInputPermissive
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
						,d.ProcessVariableProductToPresetRecipeGuid AS Deleted_PK_ProcessVariableProductToPresetRecipeGuid
                        ,d.ProcessVariableProductToPresetRecipeGuid AS Inserted_PK_ProcessVariableProductToPresetRecipeGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblProcessVariableRecipeInputPermissive As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ProcessVariableProductToPresetRecipeGuid = currentTrackingData.PK_ProcessVariableProductToPresetRecipeGuid
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
						,PK_ProcessVariableProductToPresetRecipeGuid
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
						,entityChanges.Deleted_PK_ProcessVariableProductToPresetRecipeGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblProcessVariableRecipeInputPermissive_ClusterIdx]
    ON [dbo].[tblProcessVariableRecipeInputPermissive]([_ClusterIdx] ASC);

