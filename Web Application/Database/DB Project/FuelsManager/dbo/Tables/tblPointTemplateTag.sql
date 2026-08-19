CREATE TABLE [dbo].[tblPointTemplateTag]
(
	[ID]												NVARCHAR (50)		CONSTRAINT [DF_tblPointTemplateTag_ID] DEFAULT ('') NOT NULL,
	[EngineeringUnitsType]								INT					NULL,
	[EngineeringUnitsIndex]								INT					NULL,
	[DecimalPlaces]										TINYINT				NULL,
	[ServerEngineeringUnitsIndex]						INT					NULL,
	[ValueType] nvarchar(max) NULL,
	[Value] XML NULL,
	[Maximum]											FLOAT				NULL,
	[Minimum]											FLOAT				NULL,
	[PointTagInputOutputTypeIndex]						INT					NULL,
	[Input]												BIT					NULL,
	[AlarmStatus]										BIT					NULL,
	[ApplyPointTemplateEngineeringUnits]				BIT					NULL,
	[ApplyPointTemplateDecimalPlaces]					BIT					NULL,
	[ApplyPointTemplateMaximum]							BIT					NULL,
	[ApplyPointTemplateMinimum]							BIT					NULL,
	[CreatedDate]										DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointTemplateTag_CreatedDate] DEFAULT (getdate()) NULL,
	[CreatedBy]											[dbo].[udtUserID]	CONSTRAINT [DF_tblPointTemplateTag_CreatedBy] DEFAULT (suser_sname()) NULL,
	[UpdatedDate]										DATETIMEOFFSET (7) CONSTRAINT [DF_tblPointTemplateTag_UpdatedDate] DEFAULT (getdate()) NULL,
	[UpdatedBy]											[dbo].[udtUserID]	CONSTRAINT [DF_tblPointTemplateTag_UpdatedBy] DEFAULT (suser_sname()) NULL,
	[_RowVersion]										ROWVERSION			NOT NULL,
	[PointTemplateTagGuid]								UNIQUEIDENTIFIER	CONSTRAINT [DF_tblPointTemplateTag_GUID] DEFAULT (NEWID()) NOT NULL,
	[PointTemplateGuid]									UNIQUEIDENTIFIER	NOT NULL,
	[WellKnownIdentityGuid]									UNIQUEIDENTIFIER	NULL,
	[AlarmsEnabled] BIT CONSTRAINT [DF_tblPointTemplateTag_AlarmsEnabled] DEFAULT (1) NOT NULL, 
	[InhibitInputOutputTypeConfiguration] BIT CONSTRAINT [DF_tblPointTemplateTag_InhibitInputOutputTypeConfiguration] DEFAULT (0) NOT NULL,
	[InhibitOverride] BIT CONSTRAINT [DF_tblPointTemplateTag_InhibitOverride] DEFAULT (0) NOT NULL,
	[Module]	BIT CONSTRAINT [DF_tblPointTemplateTag_Module] DEFAULT (0) NOT NULL,
	[Archived] BIT CONSTRAINT [DF_tblPointTemplateTag_Archived] DEFAULT (1) NOT NULL,
	[_ClusterIdx]									BIGINT			    NOT NULL IDENTITY,
	CONSTRAINT [PK_tblPointTemplateTag_GUID] PRIMARY KEY NONCLUSTERED ([PointTemplateTagGuid] ASC),
	CONSTRAINT [FK_tblPointTemplateTag_PointTemplateGuid] FOREIGN KEY ([PointTemplateGuid]) REFERENCES [dbo].[tblPointTemplate] ([PointTemplateGuid]),
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblPointTemplateTag_ClusterIdx] 
	ON [dbo].[tblPointTemplateTag]([_ClusterIdx]);


GO
CREATE NONCLUSTERED INDEX [IX_tblPointTemplateTag_CreatedDate]
    ON [dbo].[tblPointTemplateTag]([CreatedDate] ASC);


GO

CREATE NONCLUSTERED INDEX [IXU_tblPointTemplateTag_PointTemplateGuid]
    ON [dbo].[tblPointTemplateTag]([PointTemplateGuid] ASC);


GO
--Creating Insert / Update Trigger for tblPointTemplateTag
CREATE TRIGGER dbo.trg_insupd_tblPointTemplateTag_ForSync 
   ON dbo.tblPointTemplateTag
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
                    ,d.PointTemplateTagGuid AS Deleted_PK_PointTemplateTagGuid
                    ,i.PointTemplateTagGuid AS Inserted_PK_PointTemplateTagGuid
                    ,d.PointTemplateGuid AS Deleted_FK_ParentPK
                    ,i.PointTemplateGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.PointTemplateTagGuid = i.PointTemplateTagGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblPointTemplateTag As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_PointTemplateTagGuid = currentTrackingData.PK_PointTemplateTagGuid
 
 
		    INSERT track.tblPointTemplateTag (InsertedDate 
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
				    ,PK_PointTemplateTagGuid
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
				    ,entityChanges.Inserted_PK_PointTemplateTagGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblPointTemplateTag As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_PointTemplateTagGuid = currentTrackingData.PK_PointTemplateTagGuid
)
    END
END 
GO
--Creating Delete Trigger for tblPointTemplateTag
CREATE TRIGGER dbo.trg_del_tblPointTemplateTag_ForSync 
   ON dbo.tblPointTemplateTag
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
						,d.PointTemplateTagGuid AS Deleted_PK_PointTemplateTagGuid
                        ,d.PointTemplateTagGuid AS Inserted_PK_PointTemplateTagGuid
                      ,d.PointTemplateGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblPointTemplateTag As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_PointTemplateTagGuid = currentTrackingData.PK_PointTemplateTagGuid
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
						,PK_PointTemplateTagGuid
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
						,entityChanges.Deleted_PK_PointTemplateTagGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO-------------------------------------
CREATE TRIGGER [dbo].[trg_Audit_del_tblPointTemplateTag] ON [dbo].[tblPointTemplateTag] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPointTemplateTag','D')=1 
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
	INSERT INTO [fmaudit].tblPointTemplateTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointTemplateEngineeringUnits]
	,	[ApplyPointTemplateDecimalPlaces]
	,	[ApplyPointTemplateMaximum]
	,	[ApplyPointTemplateMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTemplateTagGuid]
	,	[PointTemplateGuid]
	,	[WellKnownIdentityGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Module]
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
	,	d.[Maximum]
	,	d.[Minimum]
	,	d.[PointTagInputOutputTypeIndex]
	,	d.[Input]
	,	d.[AlarmStatus]
	,	d.[ApplyPointTemplateEngineeringUnits]
	,	d.[ApplyPointTemplateDecimalPlaces]
	,	d.[ApplyPointTemplateMaximum]
	,	d.[ApplyPointTemplateMinimum]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[PointTemplateTagGuid]
	,	d.[PointTemplateGuid]
	,	d.[WellKnownIdentityGuid]
	,	d.[AlarmsEnabled]
	,	d.[InhibitInputOutputTypeConfiguration]
	,	d.[InhibitOverride]
	,	d.[Module]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblPointTemplateTag] ON [dbo].[tblPointTemplateTag] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPointTemplateTag','D')=1 
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
	INSERT INTO [fmaudit].tblPointTemplateTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointTemplateEngineeringUnits]
	,	[ApplyPointTemplateDecimalPlaces]
	,	[ApplyPointTemplateMaximum]
	,	[ApplyPointTemplateMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTemplateTagGuid]
	,	[PointTemplateGuid]
	,	[WellKnownIdentityGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Module]
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
	,	i.[Maximum]
	,	i.[Minimum]
	,	i.[PointTagInputOutputTypeIndex]
	,	i.[Input]
	,	i.[AlarmStatus]
	,	i.[ApplyPointTemplateEngineeringUnits]
	,	i.[ApplyPointTemplateDecimalPlaces]
	,	i.[ApplyPointTemplateMaximum]
	,	i.[ApplyPointTemplateMinimum]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[PointTemplateTagGuid]
	,	i.[PointTemplateGuid]
	,	i.[WellKnownIdentityGuid]
	,	i.[AlarmsEnabled]
	,	i.[InhibitInputOutputTypeConfiguration]
	,	i.[InhibitOverride]
	,	i.[Module]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblPointTemplateTag] ON [dbo].[tblPointTemplateTag] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblPointTemplateTag','D')=1 
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
	PointTemplateTagGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblPointTemplateTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointTemplateEngineeringUnits]
	,	[ApplyPointTemplateDecimalPlaces]
	,	[ApplyPointTemplateMaximum]
	,	[ApplyPointTemplateMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTemplateTagGuid]
	,	[PointTemplateGuid]
	,	[WellKnownIdentityGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Module]
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
	OUTPUT inserted.[PointTemplateTagGuid] AS 'PointTemplateTagGuid'
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
	,	d.[Maximum]
	,	d.[Minimum]
	,	d.[PointTagInputOutputTypeIndex]
	,	d.[Input]
	,	d.[AlarmStatus]
	,	d.[ApplyPointTemplateEngineeringUnits]
	,	d.[ApplyPointTemplateDecimalPlaces]
	,	d.[ApplyPointTemplateMaximum]
	,	d.[ApplyPointTemplateMinimum]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[PointTemplateTagGuid]
	,	d.[PointTemplateGuid]
	,	d.[WellKnownIdentityGuid]
	,	d.[AlarmsEnabled]
	,	d.[InhibitInputOutputTypeConfiguration]
	,	d.[InhibitOverride]
	,	d.[Module]
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
 
	INSERT INTO [fmaudit].tblPointTemplateTag (
		[ID]
	,	[EngineeringUnitsType]
	,	[EngineeringUnitsIndex]
	,	[DecimalPlaces]
	,	[ServerEngineeringUnitsIndex]
	,	[ValueType]
	,	[Maximum]
	,	[Minimum]
	,	[PointTagInputOutputTypeIndex]
	,	[Input]
	,	[AlarmStatus]
	,	[ApplyPointTemplateEngineeringUnits]
	,	[ApplyPointTemplateDecimalPlaces]
	,	[ApplyPointTemplateMaximum]
	,	[ApplyPointTemplateMinimum]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[PointTemplateTagGuid]
	,	[PointTemplateGuid]
	,	[WellKnownIdentityGuid]
	,	[AlarmsEnabled]
	,	[InhibitInputOutputTypeConfiguration]
	,	[InhibitOverride]
	,	[Module]
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
	,	i.[Maximum]
	,	i.[Minimum]
	,	i.[PointTagInputOutputTypeIndex]
	,	i.[Input]
	,	i.[AlarmStatus]
	,	i.[ApplyPointTemplateEngineeringUnits]
	,	i.[ApplyPointTemplateDecimalPlaces]
	,	i.[ApplyPointTemplateMaximum]
	,	i.[ApplyPointTemplateMinimum]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[PointTemplateTagGuid]
	,	i.[PointTemplateGuid]
	,	i.[WellKnownIdentityGuid]
	,	i.[AlarmsEnabled]
	,	i.[InhibitInputOutputTypeConfiguration]
	,	i.[InhibitOverride]
	,	i.[Module]
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
			agl.[PointTemplateTagGuid]=i.[PointTemplateTagGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO