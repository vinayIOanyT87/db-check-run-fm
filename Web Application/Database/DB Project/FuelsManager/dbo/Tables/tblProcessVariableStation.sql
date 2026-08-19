CREATE TABLE [dbo].[tblProcessVariableStation] (
    [ProcessVariableStationGuid]     UNIQUEIDENTIFIER   CONSTRAINT [DF_tblProcessVariableStation_GUID] DEFAULT (newid()) NOT NULL,
    [LookupProcessVariableTypeIndex] INT                CONSTRAINT [DF_tblProcessVariableStation_LookupProcessVariableType] DEFAULT ((0)) NOT NULL,
    [InstanceNumber]                 INT                NOT NULL,
    [StationGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [OPCConnectionGuid]              UNIQUEIDENTIFIER   NULL,
    [OPCItemID]                      NVARCHAR (255)     NULL,
    [DataType]                       INT                NULL,
    [ServerEngineeringUnitsIndex]    INT                NULL,
    [Quality]                        SMALLINT           NULL,
    [SIValue]                        VARBINARY (MAX)    NULL,
    [LookupSIValueVariantTypeIndex]  INT                NULL,
    [DateTimeStamp]                  DATETIMEOFFSET (7) NULL,
    [Maximum]                        VARBINARY (MAX)    NULL,
    [LookupMaximumVariantTypeIndex]  INT                NULL,
    [Minimum]                        VARBINARY (MAX)    NULL,
    [LookupMinimumVariantTypeIndex]  INT                NULL,
    [DataTypeEnabled]                BIT                NULL,
    [Input]                          BIT                NULL,
    [InputEnabled]                   BIT                NULL,
    [MessageApplicationStringGuid]   UNIQUEIDENTIFIER   NULL,
    [CreatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblProcessVariableStation_CreatedDate] DEFAULT (getdate()) NULL,
    [CreatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblProcessVariableStation_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                    DATETIMEOFFSET (7) CONSTRAINT [DF_tblProcessVariableStation_UpdatedDate] DEFAULT (getdate()) NULL,
    [UpdatedBy]                      [dbo].[udtUserID]  CONSTRAINT [DF_tblProcessVariableStation_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                    ROWVERSION         NOT NULL,
    [_ClusterIdx]                    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblProcessVariableStation_GUID] PRIMARY KEY NONCLUSTERED ([ProcessVariableStationGuid] ASC),
    CONSTRAINT [FK_tblProcessVariableStation_LookupProcessVariableType] FOREIGN KEY ([LookupProcessVariableTypeIndex]) REFERENCES [lookup].[tblProcessVariableType] ([ProcessVariableTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableStation_Maximum_DataType] FOREIGN KEY ([LookupMaximumVariantTypeIndex]) REFERENCES [lookup].[tblVariantType] ([VariantTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableStation_MessageApplicationStringGuid] FOREIGN KEY ([MessageApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblProcessVariableStation_Minimum_DataType] FOREIGN KEY ([LookupMinimumVariantTypeIndex]) REFERENCES [lookup].[tblVariantType] ([VariantTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableStation_OPCConnectionGuid] FOREIGN KEY ([OPCConnectionGuid]) REFERENCES [dbo].[tblOPCConnections] ([OPCConnectionGuid]),
    CONSTRAINT [FK_tblProcessVariableStation_SIValue_DataType] FOREIGN KEY ([LookupSIValueVariantTypeIndex]) REFERENCES [lookup].[tblVariantType] ([VariantTypeIndex]),
    CONSTRAINT [FK_tblProcessVariableStation_StationGuid] FOREIGN KEY ([StationGuid]) REFERENCES [dbo].[tblStations] ([StationGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblProcessVariableStation_CreatedDate]
    ON [dbo].[tblProcessVariableStation]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblProcessVariableStation] ON [dbo].[tblProcessVariableStation] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProcessVariableStation','D')=1 
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
	INSERT INTO [fmaudit].tblProcessVariableStation (
		[ProcessVariableStationGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[StationGuid]
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
		i.[ProcessVariableStationGuid]
	,	i.[LookupProcessVariableTypeIndex]
	,	i.[InstanceNumber]
	,	i.[StationGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblProcessVariableStation] ON [dbo].[tblProcessVariableStation] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProcessVariableStation','D')=1 
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
	ProcessVariableStationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblProcessVariableStation (
		[ProcessVariableStationGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[StationGuid]
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
	OUTPUT inserted.[ProcessVariableStationGuid] AS 'ProcessVariableStationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ProcessVariableStationGuid]
	,	d.[LookupProcessVariableTypeIndex]
	,	d.[InstanceNumber]
	,	d.[StationGuid]
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
 
	INSERT INTO [fmaudit].tblProcessVariableStation (
		[ProcessVariableStationGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[StationGuid]
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
		i.[ProcessVariableStationGuid]
	,	i.[LookupProcessVariableTypeIndex]
	,	i.[InstanceNumber]
	,	i.[StationGuid]
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
			agl.[ProcessVariableStationGuid]=i.[ProcessVariableStationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblProcessVariableStation] ON [dbo].[tblProcessVariableStation] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblProcessVariableStation','D')=1 
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
	INSERT INTO [fmaudit].tblProcessVariableStation (
		[ProcessVariableStationGuid]
	,	[LookupProcessVariableTypeIndex]
	,	[InstanceNumber]
	,	[StationGuid]
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
		d.[ProcessVariableStationGuid]
	,	d.[LookupProcessVariableTypeIndex]
	,	d.[InstanceNumber]
	,	d.[StationGuid]
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
--Creating Insert / Update Trigger for tblProcessVariableStation
CREATE TRIGGER dbo.trg_insupd_tblProcessVariableStation_ForSync 
   ON dbo.tblProcessVariableStation
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
                    ,d.ProcessVariableStationGuid AS Deleted_PK_ProcessVariableStationGuid
                    ,i.ProcessVariableStationGuid AS Inserted_PK_ProcessVariableStationGuid
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
				    d.ProcessVariableStationGuid = i.ProcessVariableStationGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblProcessVariableStation As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ProcessVariableStationGuid = currentTrackingData.PK_ProcessVariableStationGuid
 
 
		    INSERT track.tblProcessVariableStation (InsertedDate 
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
				    ,PK_ProcessVariableStationGuid
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
				    ,entityChanges.Inserted_PK_ProcessVariableStationGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblProcessVariableStation As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ProcessVariableStationGuid = currentTrackingData.PK_ProcessVariableStationGuid
)
    END
END 

GO
--Creating Delete Trigger for tblProcessVariableStation
CREATE TRIGGER dbo.trg_del_tblProcessVariableStation_ForSync 
   ON dbo.tblProcessVariableStation
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
						,d.ProcessVariableStationGuid AS Deleted_PK_ProcessVariableStationGuid
                        ,d.ProcessVariableStationGuid AS Inserted_PK_ProcessVariableStationGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblProcessVariableStation As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ProcessVariableStationGuid = currentTrackingData.PK_ProcessVariableStationGuid
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
						,PK_ProcessVariableStationGuid
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
						,entityChanges.Deleted_PK_ProcessVariableStationGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblProcessVariableStation_ClusterIdx]
    ON [dbo].[tblProcessVariableStation]([_ClusterIdx] ASC);

