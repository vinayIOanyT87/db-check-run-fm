CREATE TABLE [dbo].[tblMessageLog] (
    [CreatedDate]    DATETIMEOFFSET (7) CONSTRAINT [DF_tblMessageLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]      [dbo].[udtUserID]  CONSTRAINT [DF_tblMessageLog_CreatedBy] DEFAULT ('') NOT NULL,
    [MessageLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblMessageLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]    ROWVERSION         NOT NULL,
    [CompanyGuid]    UNIQUEIDENTIFIER   NOT NULL,
    [MessageGuid]    UNIQUEIDENTIFIER   NOT NULL,
    [PersonnelGuid]  UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]    BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblMessageLog_GUID] PRIMARY KEY NONCLUSTERED ([MessageLogGuid] ASC),
    CONSTRAINT [FK_tblMessageLog_CompanyGuid] FOREIGN KEY ([CompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblMessageLog_MessageGuid] FOREIGN KEY ([MessageGuid]) REFERENCES [dbo].[tblMessages] ([MessageGuid]),
    CONSTRAINT [FK_tblMessageLog_PersonnelGuid] FOREIGN KEY ([PersonnelGuid]) REFERENCES [dbo].[tblPersonnel] ([PersonnelGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblMessageLog_CreatedDate]
    ON [dbo].[tblMessageLog]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblMessageLog_PersonnelGuid_CompanyGuid_CreatedDate_MessageGuid]
    ON [dbo].[tblMessageLog]([PersonnelGuid] ASC, [CompanyGuid] ASC, [CreatedDate] ASC, [MessageGuid] ASC);


GO
--Creating Insert / Update Trigger for tblMessageLog
CREATE TRIGGER dbo.trg_insupd_tblMessageLog_ForSync 
   ON dbo.tblMessageLog
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
                    ,d.MessageLogGuid AS Deleted_PK_MessageLogGuid
                    ,i.MessageLogGuid AS Inserted_PK_MessageLogGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,@currentDateTimeOffset AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.MessageLogGuid = i.MessageLogGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblMessageLog As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_MessageLogGuid = currentTrackingData.PK_MessageLogGuid
 
 
		    INSERT track.tblMessageLog (InsertedDate 
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
				    ,PK_MessageLogGuid
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
				    ,entityChanges.Inserted_PK_MessageLogGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblMessageLog As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_MessageLogGuid = currentTrackingData.PK_MessageLogGuid
)
    END
END 

GO
--Creating Delete Trigger for tblMessageLog
CREATE TRIGGER dbo.trg_del_tblMessageLog_ForSync 
   ON dbo.tblMessageLog
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
						,d.MessageLogGuid AS Deleted_PK_MessageLogGuid
                        ,d.MessageLogGuid AS Inserted_PK_MessageLogGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,@currentDateTimeOffset AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblMessageLog As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_MessageLogGuid = currentTrackingData.PK_MessageLogGuid
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
						,PK_MessageLogGuid
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
						,entityChanges.Deleted_PK_MessageLogGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE NONCLUSTERED INDEX IX_tblMessageLog_CompanyGuid ON [dbo].[tblMessageLog](CompanyGuid)
GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblMessageLog_ClusterIdx]
    ON [dbo].[tblMessageLog]([_ClusterIdx] ASC);

