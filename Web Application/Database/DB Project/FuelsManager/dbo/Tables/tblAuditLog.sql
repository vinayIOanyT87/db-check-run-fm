CREATE TABLE [dbo].[tblAuditLog] (
    [SessionID]    NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_SessionID] DEFAULT ('') NULL,
    [ActionID]     NVARCHAR (20)      CONSTRAINT [DF_tblAuditLog_ActionID] DEFAULT ('') NOT NULL,
    [TypeID]       NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_TypeID] DEFAULT ('') NOT NULL,
    [ID]           NVARCHAR (256)     CONSTRAINT [DF_tblAuditLog_ID] DEFAULT ('') NOT NULL,
    [PropertyID]   NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_PropertyID] DEFAULT ('') NOT NULL,
    [NewValue]     NVARCHAR (MAX)    CONSTRAINT [DF_tblAuditLog_NewValue] DEFAULT ('') NOT NULL,
    [OldValue]     NVARCHAR (MAX)    CONSTRAINT [DF_tblAuditLog_OldValue] DEFAULT ('') NOT NULL,
    [CreatedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_tblAuditLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]    [dbo].[udtUserID]  CONSTRAINT [DF_tblAuditLog_CreatedBy] DEFAULT ('') NOT NULL,
    [ParentTypeID] NVARCHAR (50)      CONSTRAINT [DF_tblAuditLog_ParentTypeID] DEFAULT ('') NOT NULL,
    [AuditLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblAuditLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]  ROWVERSION         NOT NULL,
    [SiteGuid]     UNIQUEIDENTIFIER   NOT NULL,
    [AuditedDate]  DATETIMEOFFSET (7) CONSTRAINT [DF_tblAuditLog_AuditedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [_ClusterIdx]  BIGINT             IDENTITY (1, 1) NOT NULL,
    [SourceNode]   NVARCHAR (256)     NULL,
    [AuditContext] VARBINARY(128) NULL, 
    CONSTRAINT [PK_tblAuditLog_GUID] PRIMARY KEY NONCLUSTERED ([AuditLogGuid] ASC)
);

GO
CREATE NONCLUSTERED INDEX [IX_tblAuditLog_CreatedDate]
    ON [dbo].[tblAuditLog]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAuditLog_ClusterIdx]
    ON [dbo].[tblAuditLog]([_ClusterIdx] ASC);


GO
--Creating Insert / Update Trigger for tblAuditLog
CREATE TRIGGER dbo.trg_insupd_tblAuditLog_ForSync 
   ON dbo.tblAuditLog
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
                    ,d.AuditLogGuid AS Deleted_PK_AuditLogGuid
                    ,i.AuditLogGuid AS Inserted_PK_AuditLogGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,@currentDateTimeOffset AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.AuditLogGuid = i.AuditLogGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAuditLog As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AuditLogGuid = currentTrackingData.PK_AuditLogGuid
 
 
		    INSERT track.tblAuditLog (InsertedDate 
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
				    ,PK_AuditLogGuid
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
				    ,entityChanges.Inserted_PK_AuditLogGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAuditLog As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AuditLogGuid = currentTrackingData.PK_AuditLogGuid
)
    END
END
GO
--Creating Delete Trigger for tblAuditLog
CREATE TRIGGER dbo.trg_del_tblAuditLog_ForSync 
   ON dbo.tblAuditLog
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
						,d.AuditLogGuid AS Deleted_PK_AuditLogGuid
                        ,d.AuditLogGuid AS Inserted_PK_AuditLogGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,@currentDateTimeOffset AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAuditLog As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AuditLogGuid = currentTrackingData.PK_AuditLogGuid
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
						,PK_AuditLogGuid
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
						,entityChanges.Deleted_PK_AuditLogGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END