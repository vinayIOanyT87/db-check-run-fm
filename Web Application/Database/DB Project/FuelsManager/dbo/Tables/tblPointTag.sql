CREATE TABLE [dbo].[tblPointTag]
(
	[ID] NVARCHAR (50) CONSTRAINT [DF_tblPointTag_ID] DEFAULT ('') NOT NULL,
	[EngineeringUnitsType] INT NULL,
	[EngineeringUnitsIndex] INT NULL,
	[DecimalPlaces] TINYINT NULL,
	[ServerEngineeringUnitsIndex] INT NULL,
	[ValueType] nvarchar(max) NULL,
	[Status] BIGINT NULL,
	[Value] xml NULL,
	[ServerTimeStamp] DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointTag_ServerTimeStamp] DEFAULT (getdate()) NULL,
	[SourceTimeStamp] DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointTag_SourceTimeStamp] DEFAULT (getdate()) NULL,
	[Maximum] FLOAT NULL,
	[Minimum] FLOAT NULL,
	[PointTagInputOutputTypeIndex] INT NULL,
	[LastPointTagInputOutputTypeIndex] INT NULL,
	[Input] BIT NULL,
	[AlarmStatus] BIT NULL,
	[ApplyPointEngineeringUnits] BIT NULL,
	[ApplyPointDecimalPlaces] BIT NULL,
	[ApplyPointMaximum] BIT NULL,
	[ApplyPointMinimum] BIT NULL,
	[OpcUaServerGuid] UNIQUEIDENTIFIER NULL,
	[OpcUaBrowsePath] NVARCHAR (250) NULL,
	[OpcUaNamespaceUri] NVARCHAR (250) NULL,
	[OpcUaPublishingInterval] INT NULL,
	[OpcUaNodeId] NVARCHAR (250) NULL,
	[OpcUaIsReadable] BIT NULL,
	[OpcUaServerDataType] INT NULL,
	[OpcUaWriteHoldoffTime] INT NULL,
	[OpcUaWritePeriodicUpdateInterval] INT NULL,
	[CreatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointTag_CreatedDate] DEFAULT (getdate()) NULL,
	[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblPointTag_CreatedBy] DEFAULT (suser_sname()) NULL,
	[UpdatedDate] DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointTag_UpdatedDate] DEFAULT (getdate()) NULL,
	[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_tblPointTag_UpdatedBy] DEFAULT (suser_sname()) NULL,
	[_RowVersion] ROWVERSION NOT NULL,
	[PointTagGuid] UNIQUEIDENTIFIER	CONSTRAINT [DF_tblPointTag_GUID] DEFAULT (newid()) NOT NULL,
	[PointGuid] UNIQUEIDENTIFIER	NOT NULL,
	[PointTemplateTagGuid] UNIQUEIDENTIFIER	NULL,
	[AlarmsEnabled] BIT DEFAULT (1) NOT NULL,
	[InhibitInputOutputTypeConfiguration] BIT CONSTRAINT [DF_tblPointTag_InhibitInputOutputTypeConfiguration] DEFAULT (0) NOT NULL,
	[InhibitOverride] BIT CONSTRAINT [DF_tblPointTag_InhibitOverride] DEFAULT (0) NOT NULL,
	[Deadband] float(53) NULL,
	[Holdoff] int NULL,
	[Archived] BIT CONSTRAINT [DF_tblPointTag_Archived] DEFAULT (1) NOT NULL,
	[_ClusterIdx]										BIGINT			    NOT NULL IDENTITY,
	CONSTRAINT [PK_tblPointTag_GUID] PRIMARY KEY NONCLUSTERED ([PointTagGuid] ASC),
	CONSTRAINT [FK_tblPointTag_PointGuid] FOREIGN KEY ([PointGuid]) REFERENCES [dbo].[tblPoint] ([PointGuid]),
	CONSTRAINT [FK_tblPointTag_PointTagInputOutputType] FOREIGN KEY ([PointTagInputOutputTypeIndex]) REFERENCES [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex]),
	CONSTRAINT [FK_tblPointTag_LastPointTagInputOutputType] FOREIGN KEY ([LastPointTagInputOutputTypeIndex]) REFERENCES [lookup].[tblPointTagInputOutputType] ([PointTagInputOutputTypeIndex]),
	CONSTRAINT [FK_tblPointTag_tblOpcUaServer] FOREIGN KEY([OpcUaServerGuid]) REFERENCES [dbo].[tblOpcUaServer] ([OpcUaServerGuid])
);
GO


ALTER TABLE [dbo].[tblPointTag]  ADD  CONSTRAINT [FK_tblPointTag_PointTemplateTagGuid] FOREIGN KEY([PointTemplateTagGuid])
REFERENCES [dbo].[tblPointTemplateTag] ([PointTemplateTagGuid])
GO

ALTER TABLE [dbo].[tblPointTag] NOCHECK CONSTRAINT [FK_tblPointTag_PointTemplateTagGuid]
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblPointTag_ClusterIdx] 
	ON [dbo].[tblPointTag] ([_ClusterIdx])


GO
CREATE NONCLUSTERED INDEX [IX_tblPointTag_CreatedDate]
    ON [dbo].[tblPointTag]([CreatedDate] ASC);


GO

CREATE NONCLUSTERED INDEX [IXU_tblPointTag_PointTemplateTagGuid_PointGuid]
    ON [dbo].[tblPointTag](
	 [PointTemplateTagGuid] ASC,
	 [PointGuid] ASC);
GO

CREATE NONCLUSTERED INDEX IX_tblPointTag_guid_include 
ON [dbo].[tblPointTag]
(
	[PointTagGuid] ASC
) INCLUDE( EngineeringUnitsType, EngineeringUnitsIndex, DecimalPlaces, Maximum, Minimum)
GO
CREATE NONCLUSTERED INDEX [IXU_tblPointTag_OpcUaServerGuid]
    ON [dbo].[tblPointTag]([OpcUaServerGuid] ASC);


GO

CREATE NONCLUSTERED INDEX [IXU_tblPointTag_PointGuid_PointTagGuid]
    ON [dbo].[tblPointTag](
	 [PointGuid] ASC,
	 [PointTagGuid] ASC);
GO
--Creating Insert / Update Trigger for tblPointTag
CREATE TRIGGER dbo.trg_insupd_tblPointTag_ForSync 
   ON dbo.tblPointTag
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
                    ,d.PointTagGuid AS Deleted_PK_PointTagGuid
                    ,i.PointTagGuid AS Inserted_PK_PointTagGuid
                    ,d.PointGuid AS Deleted_FK_ParentPK
                    ,i.PointGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.PointTagGuid = i.PointTagGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblPointTag As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_PointTagGuid = currentTrackingData.PK_PointTagGuid
 
 
		    INSERT track.tblPointTag (InsertedDate 
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
				    ,PK_PointTagGuid
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
				    ,entityChanges.Inserted_PK_PointTagGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblPointTag As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_PointTagGuid = currentTrackingData.PK_PointTagGuid
)
    END
END 
GO
--Creating Delete Trigger for tblPointTag
CREATE TRIGGER dbo.trg_del_tblPointTag_ForSync 
   ON dbo.tblPointTag
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
						,d.PointTagGuid AS Deleted_PK_PointTagGuid
                        ,d.PointTagGuid AS Inserted_PK_PointTagGuid
                      ,d.PointGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblPointTag As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_PointTagGuid = currentTrackingData.PK_PointTagGuid
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
						,PK_PointTagGuid
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
						,entityChanges.Deleted_PK_PointTagGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblPointTag] ON [dbo].[tblPointTag] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPointTag','D')=1 
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
	INSERT INTO [fmaudit].tblPointTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Status]
	,	[ServerTimeStamp]
	,	[SourceTimeStamp]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[LastPointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointEngineeringUnits]
	,	[ApplyPointDecimalPlaces]
	,	[ApplyPointMaximum]
	,	[ApplyPointMinimum]
	,	[OpcUaServerGuid]
	,	[OpcUaBrowsePath]
	,	[OpcUaNamespaceUri]
	,	[OpcUaPublishingInterval]
	,	[OpcUaNodeId]
	,	[OpcUaIsReadable]
	,	[OpcUaServerDataType]
	,	[OpcUaWriteHoldoffTime]
	,	[OpcUaWritePeriodicUpdateInterval]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTagGuid]
	,	[PointGuid]
	,	[PointTemplateTagGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Deadband]
	,	[Holdoff]
	,	[Archived]
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
		d.[ID]
	,	d.[EngineeringUnitsType]
	,	d.[EngineeringUnitsIndex]
	,	d.[DecimalPlaces]
	,	d.[ServerEngineeringUnitsIndex]
	,	d.[ValueType]
	,	d.[Status]
	,	d.[ServerTimeStamp]
	,	d.[SourceTimeStamp]
	,	d.[Maximum]
	,	d.[Minimum]
	,	d.[PointTagInputOutputTypeIndex]
	,	d.[LastPointTagInputOutputTypeIndex]
	,	d.[Input]
	,	d.[AlarmStatus]
	,	d.[ApplyPointEngineeringUnits]
	,	d.[ApplyPointDecimalPlaces]
	,	d.[ApplyPointMaximum]
	,	d.[ApplyPointMinimum]
	,	d.[OpcUaServerGuid]
	,	d.[OpcUaBrowsePath]
	,	d.[OpcUaNamespaceUri]
	,	d.[OpcUaPublishingInterval]
	,	d.[OpcUaNodeId]
	,	d.[OpcUaIsReadable]
	,	d.[OpcUaServerDataType]
	,	d.[OpcUaWriteHoldoffTime]
	,	d.[OpcUaWritePeriodicUpdateInterval]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[PointTagGuid]
	,	d.[PointGuid]
	,	d.[PointTemplateTagGuid]
	,	d.[AlarmsEnabled]
	,	d.[InhibitInputOutputTypeConfiguration]
	,	d.[InhibitOverride]
	,	d.[Deadband]
	,	d.[Holdoff]
	,	d.[Archived]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblPointTag] ON [dbo].[tblPointTag] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPointTag','D')=1 
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
	INSERT INTO [fmaudit].tblPointTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Status]
	,	[ServerTimeStamp]
	,	[SourceTimeStamp]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[LastPointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointEngineeringUnits]
	,	[ApplyPointDecimalPlaces]
	,	[ApplyPointMaximum]
	,	[ApplyPointMinimum]
	,	[OpcUaServerGuid]
	,	[OpcUaBrowsePath]
	,	[OpcUaNamespaceUri]
	,	[OpcUaPublishingInterval]
	,	[OpcUaNodeId]
	,	[OpcUaIsReadable]
	,	[OpcUaServerDataType]
	,	[OpcUaWriteHoldoffTime]
	,	[OpcUaWritePeriodicUpdateInterval]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTagGuid]
	,	[PointGuid]
	,	[PointTemplateTagGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Deadband]
	,	[Holdoff]
	,	[Archived]
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
		i.[ID]
	,	i.[EngineeringUnitsType]
	,	i.[EngineeringUnitsIndex]
	,	i.[DecimalPlaces]
	,	i.[ServerEngineeringUnitsIndex]
	,	i.[ValueType]
	,	i.[Status]
	,	i.[ServerTimeStamp]
	,	i.[SourceTimeStamp]
	,	i.[Maximum]
	,	i.[Minimum]
	,	i.[PointTagInputOutputTypeIndex]
	,	i.[LastPointTagInputOutputTypeIndex]
	,	i.[Input]
	,	i.[AlarmStatus]
	,	i.[ApplyPointEngineeringUnits]
	,	i.[ApplyPointDecimalPlaces]
	,	i.[ApplyPointMaximum]
	,	i.[ApplyPointMinimum]
	,	i.[OpcUaServerGuid]
	,	i.[OpcUaBrowsePath]
	,	i.[OpcUaNamespaceUri]
	,	i.[OpcUaPublishingInterval]
	,	i.[OpcUaNodeId]
	,	i.[OpcUaIsReadable]
	,	i.[OpcUaServerDataType]
	,	i.[OpcUaWriteHoldoffTime]
	,	i.[OpcUaWritePeriodicUpdateInterval]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[PointTagGuid]
	,	i.[PointGuid]
	,	i.[PointTemplateTagGuid]
	,	i.[AlarmsEnabled]
	,	i.[InhibitInputOutputTypeConfiguration]
	,	i.[InhibitOverride]
	,	i.[Deadband]
	,	i.[Holdoff]
	,	i.[Archived]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblPointTag] ON [dbo].[tblPointTag] AFTER UPDATE 
AS
BEGIN


	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPointTag','D')=1 
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
	PointTagGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblPointTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Status]
	,	[ServerTimeStamp]
	,	[SourceTimeStamp]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[LastPointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointEngineeringUnits]
	,	[ApplyPointDecimalPlaces]
	,	[ApplyPointMaximum]
	,	[ApplyPointMinimum]
	,	[OpcUaServerGuid]
	,	[OpcUaBrowsePath]
	,	[OpcUaNamespaceUri]
	,	[OpcUaPublishingInterval]
	,	[OpcUaNodeId]
	,	[OpcUaIsReadable]
	,	[OpcUaServerDataType]
	,	[OpcUaWriteHoldoffTime]
	,	[OpcUaWritePeriodicUpdateInterval]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTagGuid]
	,	[PointGuid]
	,	[PointTemplateTagGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Deadband]
	,	[Holdoff]
	,	[Archived]
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
	OUTPUT inserted.[PointTagGuid] AS 'PointTagGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ID]
	,	d.[EngineeringUnitsType]
	,	d.[EngineeringUnitsIndex]
	,	d.[DecimalPlaces]
	,	d.[ServerEngineeringUnitsIndex]
	,	d.[ValueType]
	,	d.[Status]
	,	d.[ServerTimeStamp]
	,	d.[SourceTimeStamp]
	,	d.[Maximum]
	,	d.[Minimum]
	,	d.[PointTagInputOutputTypeIndex]
	,	d.[LastPointTagInputOutputTypeIndex]
	,	d.[Input]
	,	d.[AlarmStatus]
	,	d.[ApplyPointEngineeringUnits]
	,	d.[ApplyPointDecimalPlaces]
	,	d.[ApplyPointMaximum]
	,	d.[ApplyPointMinimum]
	,	d.[OpcUaServerGuid]
	,	d.[OpcUaBrowsePath]
	,	d.[OpcUaNamespaceUri]
	,	d.[OpcUaPublishingInterval]
	,	d.[OpcUaNodeId]
	,	d.[OpcUaIsReadable]
	,	d.[OpcUaServerDataType]
	,	d.[OpcUaWriteHoldoffTime]
	,	d.[OpcUaWritePeriodicUpdateInterval]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[PointTagGuid]
	,	d.[PointGuid]
	,	d.[PointTemplateTagGuid]
	,	d.[AlarmsEnabled]
	,	d.[InhibitInputOutputTypeConfiguration]
	,	d.[InhibitOverride]
	,	d.[Deadband]
	,	d.[Holdoff]
	,	d.[Archived]
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
 
	INSERT INTO [fmaudit].tblPointTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Status]
	,	[ServerTimeStamp]
	,	[SourceTimeStamp]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[LastPointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointEngineeringUnits]
	,	[ApplyPointDecimalPlaces]
	,	[ApplyPointMaximum]
	,	[ApplyPointMinimum]
	,	[OpcUaServerGuid]
	,	[OpcUaBrowsePath]
	,	[OpcUaNamespaceUri]
	,	[OpcUaPublishingInterval]
	,	[OpcUaNodeId]
	,	[OpcUaIsReadable]
	,	[OpcUaServerDataType]
	,	[OpcUaWriteHoldoffTime]
	,	[OpcUaWritePeriodicUpdateInterval]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTagGuid]
	,	[PointGuid]
	,	[PointTemplateTagGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Deadband]
	,	[Holdoff]
	,	[Archived]
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
		i.[ID]
	,	i.[EngineeringUnitsType]
	,	i.[EngineeringUnitsIndex]
	,	i.[DecimalPlaces]
	,	i.[ServerEngineeringUnitsIndex]
	,	i.[ValueType]
	,	i.[Status]
	,	i.[ServerTimeStamp]
	,	i.[SourceTimeStamp]
	,	i.[Maximum]
	,	i.[Minimum]
	,	i.[PointTagInputOutputTypeIndex]
	,	i.[LastPointTagInputOutputTypeIndex]
	,	i.[Input]
	,	i.[AlarmStatus]
	,	i.[ApplyPointEngineeringUnits]
	,	i.[ApplyPointDecimalPlaces]
	,	i.[ApplyPointMaximum]
	,	i.[ApplyPointMinimum]
	,	i.[OpcUaServerGuid]
	,	i.[OpcUaBrowsePath]
	,	i.[OpcUaNamespaceUri]
	,	i.[OpcUaPublishingInterval]
	,	i.[OpcUaNodeId]
	,	i.[OpcUaIsReadable]
	,	i.[OpcUaServerDataType]
	,	i.[OpcUaWriteHoldoffTime]
	,	i.[OpcUaWritePeriodicUpdateInterval]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[PointTagGuid]
	,	i.[PointGuid]
	,	i.[PointTemplateTagGuid]
	,	i.[AlarmsEnabled]
	,	i.[InhibitInputOutputTypeConfiguration]
	,	i.[InhibitOverride]
	,	i.[Deadband]
	,	i.[Holdoff]
	,	i.[Archived]
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
			agl.[PointTagGuid]=i.[PointTagGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO

