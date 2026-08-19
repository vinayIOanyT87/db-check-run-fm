CREATE TABLE [dbo].[tblExternalStationLog]
(
	[ExternalStationLogGuid] UNIQUEIDENTIFIER NOT NULL,
	[SiteGuid] UNIQUEIDENTIFIER NOT NULL,
	[ExternalStationGuid] UNIQUEIDENTIFIER NOT NULL, 
    [LogText] NVARCHAR(MAX) NOT NULL, 
    [LookupExternalStationLogTypeIndex] INT NOT NULL, 
	[LogDate] DATETIMEOFFSET NOT NULL,    
    [CreatedBy] [dbo].[udtUserID] NOT NULL, 
    [CreatedDate] DATETIMEOFFSET NOT NULL, 
    [UpdatedBy] [dbo].[udtUserID] NOT NULL, 
    [UpdatedDate] DATETIMEOFFSET NOT NULL, 
    [_RowVersion] TIMESTAMP NOT NULL, 
	[_ClusterIdx] BIGINT IDENTITY(1,1) NOT NULL, 
    CONSTRAINT [FK_tblExternalStationLog_tblExternalStation] FOREIGN KEY (ExternalStationGuid) REFERENCES [tblExternalStation]([ExternalStationGuid]), 
    CONSTRAINT [PK_tblExternalStationLog] PRIMARY KEY NONCLUSTERED ([ExternalStationLogGuid]), 
    CONSTRAINT [FK_tblExternalStationLog_LookupExternalStationLogTypeIndex] FOREIGN KEY (LookupExternalStationLogTypeIndex) REFERENCES [lookup].[tblExternalStationLogType]([ExternalStationLogTypeIndex]),
	CONSTRAINT [FK_tblExternalStationLog_SiteGuid] FOREIGN KEY (SiteGuid) REFERENCES [dbo].[tblSites]([SiteGuid])
)

GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblExternalStationLog__ClusterIdx] ON [dbo].[tblExternalStationLog] (_ClusterIdx)
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStationLog_CreatedDate] ON [dbo].[tblExternalStationLog] (CreatedDate)
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStationLog_LogDate] ON [dbo].[tblExternalStationLog] ([LogDate])
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStationLog_ExternalStationGuid] ON [dbo].[tblExternalStationLog] ([ExternalStationGuid])
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStationLog_LookupExternalStationLogTypeIndex] ON [dbo].[tblExternalStationLog] ([LookupExternalStationLogTypeIndex])
GO

CREATE NONCLUSTERED INDEX [IX_tblExternalStationLog_SiteGuid] ON [dbo].[tblExternalStationLog] ([SiteGuid])

GO
--Creating Insert / Update Trigger for tblExternalStationLog
CREATE TRIGGER dbo.trg_insupd_tblExternalStationLog_ForSync 
   ON dbo.tblExternalStationLog
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
                   ,d.ExternalStationLogGuid AS Deleted_PK_ExternalStationLogGuid
                    ,i.ExternalStationLogGuid AS Inserted_PK_ExternalStationLogGuid
                    ,NULL AS Deleted_FK_ParentPK 
                    ,NULL AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,NULL AS Deleted_RowVersion 
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.ExternalStationLogGuid = i.ExternalStationLogGuid
           ) 
		    MERGE INTO track.tblExternalStationLog WITH (HOLDLOCK) As currentTrackingData 
			    USING ChangeList As entityChanges 
				    ON entityChanges.Inserted_PK_ExternalStationLogGuid = currentTrackingData.PK_ExternalStationLogGuid
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
				    ,PK_ExternalStationLogGuid
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
				    ,entityChanges.Inserted_PK_ExternalStationLogGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    );
    END
END
GO
--Creating Delete Trigger for tblExternalStationLog
CREATE TRIGGER dbo.trg_del_tblExternalStationLog_ForSync 
   ON dbo.tblExternalStationLog
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
						,d.ExternalStationLogGuid AS Deleted_PK_ExternalStationLogGuid
                        ,d.ExternalStationLogGuid AS Inserted_PK_ExternalStationLogGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblExternalStationLog WITH (HOLDLOCK) As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ExternalStationLogGuid = currentTrackingData.PK_ExternalStationLogGuid
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
						,PK_ExternalStationLogGuid
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
						,entityChanges.Deleted_PK_ExternalStationLogGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END