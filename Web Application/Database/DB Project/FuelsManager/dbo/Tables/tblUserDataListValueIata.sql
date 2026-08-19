CREATE TABLE [dbo].[tblUserDataListValueIata] (
    [UserDataListValueIataGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_map_tblUserDataListValueIata_UserDataFieldIataGuid] DEFAULT (newid()) NOT NULL,
    [UserDataFieldIataGuid]     UNIQUEIDENTIFIER   NULL,
    [Value]                        NVARCHAR (120)     NULL,
    [CreatedDate]                  DATETIMEOFFSET (7) CONSTRAINT [DF_tblUserDataListValueIata_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                    [dbo].[udtUserID]  CONSTRAINT [DF_tblUserDataListValueIata_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                  DATETIMEOFFSET (7) CONSTRAINT [DF_tblUserDataListValueIata_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                    [dbo].[udtUserID]  CONSTRAINT [DF_tblUserDataListValueIata_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                  ROWVERSION         NOT NULL,
    CONSTRAINT [PK_tblUserDataListValueIata_GUID] PRIMARY KEY NONCLUSTERED ([UserDataListValueIataGuid] ASC),
    CONSTRAINT [FK_tblUserDataListValueIata_Guid] FOREIGN KEY ([UserDataFieldIataGuid]) REFERENCES [dbo].[tblUserDataFieldIata] ([UserDataFieldIataGuid])
);
GO
CREATE CLUSTERED INDEX [IX_tblUserDataFieldIata_CreatedDate]
    ON [dbo].[tblUserDataListValueIata]([CreatedDate] ASC);
GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblUserDataListValueIata] ON [dbo].[tblUserDataListValueIata] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblUserDataListValueIata','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@AuditDatetime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100);
	SET @AuditDatetime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'D'; -- For Deletes 
	SET @_AuditEventSequence= 1; 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 
	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblUserDataListValueIata WITH(ROWLOCK)(
		[UserDataListValueIataGuid]
	,	[UserDataFieldIataGuid]
	,	[Value]
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
	)
	SELECT 
		d.[UserDataListValueIataGuid]
	,	d.[UserDataFieldIataGuid]
	,	d.[Value]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@AuditDatetime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	FROM deleted d
END
GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblUserDataListValueIata] ON [dbo].[tblUserDataListValueIata] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblUserDataListValueIata','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@AuditDatetime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100);
	SET @AuditDatetime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'I' -- For Inserts 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 
	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblUserDataListValueIata WITH(ROWLOCK)(
		[UserDataListValueIataGuid]
	,	[UserDataFieldIataGuid]
	,	[Value]
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
	)
	SELECT 
		i.[UserDataListValueIataGuid]
	,	i.[UserDataFieldIataGuid]
	,	i.[Value]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@AuditDatetime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	FROM inserted i
END
GO
--Creating Insert / Update Trigger for tblUserDataListValueIata
CREATE TRIGGER dbo.trg_insupd_tblUserDataListValueIata_ForSync 
   ON dbo.tblUserDataListValueIata
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
 
       ; WITH ChangeList AS ( 
       SELECT @syncContext AS ChangeContext 
                    ,d.UserDataListValueIataGuid AS Deleted_PK_UserDataListValueIataGuid
                    ,i.UserDataListValueIataGuid AS Inserted_PK_UserDataListValueIataGuid
                    ,NULL AS Deleted_FK_ParentPK 
                    ,NULL AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,NULL AS CurrentSiteGuid 
				    ,NULL AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,NULL AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.UserDataListValueIataGuid = i.UserDataListValueIataGuid
           ) 
		    MERGE INTO track.tblUserDataListValueIata WITH (HOLDLOCK) As currentTrackingData 
			    USING ChangeList As entityChanges 
				    ON entityChanges.Inserted_PK_UserDataListValueIataGuid = currentTrackingData.PK_UserDataListValueIataGuid
           WHEN Matched 
           THEN 
           UPDATE SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
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
				    ,PK_UserDataListValueIataGuid
				    ,FK_ParentPK 
		    )
		    VALUES (CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset 
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
				    ,entityChanges.Inserted_PK_UserDataListValueIataGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    );
    END
END
GO
--Creating Delete Trigger for tblUserDataListValueIata
CREATE TRIGGER dbo.trg_del_tblUserDataListValueIata_ForSync 
   ON dbo.tblUserDataListValueIata
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
						,d.UserDataListValueIataGuid AS Deleted_PK_UserDataListValueIataGuid
                        ,d.UserDataListValueIataGuid AS Inserted_PK_UserDataListValueIataGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblUserDataListValueIata WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_UserDataListValueIataGuid = currentTrackingData.PK_UserDataListValueIataGuid
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
						,PK_UserDataListValueIataGuid
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
						,entityChanges.Deleted_PK_UserDataListValueIataGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END
GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblUserDataListValueIata] ON [dbo].[tblUserDataListValueIata] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblUserDataListValueIata','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@AuditDatetime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100);
	SET @AuditDatetime = SYSDATETIMEOFFSET();
	SET @_AuditGUID = NEWID();
	SET @_AuditEventType= 'U' -- For Updates 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 
	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].tblUserDataListValueIata WITH(ROWLOCK)(
		[UserDataListValueIataGuid]
	,	[UserDataFieldIataGuid]
	,	[Value]
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
	)
	SELECT 
		d.[UserDataListValueIataGuid]
	,	d.[UserDataFieldIataGuid]
	,	d.[Value]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@AuditDatetime
	,	@_AuditSiteGUID
	,	@_AuditGUID
	,	@_UserId
	FROM deleted d
	INSERT INTO [fmaudit].tblUserDataListValueIata WITH(ROWLOCK)(
		[UserDataListValueIataGuid]
	,	[UserDataFieldIataGuid]
	,	[Value]
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
	)
	SELECT 
		i.[UserDataListValueIataGuid]
	,	i.[UserDataFieldIataGuid]
	,	i.[Value]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	@_AuditEventType
	,	2
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@AuditDatetime
	,	@_AuditSiteGUID
	,	@_AuditGUID
	,	@_UserId
	FROM inserted i 
END