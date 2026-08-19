CREATE TABLE [dbo].[tblExportResults] (
    [InterfaceName]               NVARCHAR (150)     NOT NULL,
    [TransVersion]                BIGINT             NULL,
    [FailedCount]                 INT                NOT NULL,
    [SuccessCount]                INT                NOT NULL,
    [TransDateTime]               DATETIMEOFFSET (7) NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportResults_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportResults_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  NOT NULL,
    [BatchID]                     NVARCHAR (64)      NULL,
    [ExportResultGuid]            UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExportResults_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [SiteGuid]                    UNIQUEIDENTIFIER   NOT NULL,
    [LookupExportResultTypeIndex] INT                NOT NULL,
    [ArchiveFileName]             NVARCHAR (150)     NULL,
	[_ClusterIdx]				  BIGINT			 NOT NULL IDENTITY,
    CONSTRAINT [PK_tblExportResults_GUID] PRIMARY KEY NONCLUSTERED ([ExportResultGuid] ASC),
    CONSTRAINT [FK_tblExportResults_LookupExportResultTypeIndex] FOREIGN KEY ([LookupExportResultTypeIndex]) REFERENCES [lookup].[tblExportResultType] ([ExportResultTypeIndex]),
    CONSTRAINT [FK_tblExportResults_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);


GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblExportResults_ClusterIdx] 
	ON [dbo].[tblExportResults]([_ClusterIdx]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResults_CreatedDate]
    ON [dbo].[tblExportResults]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblExportResults_LookupExportResultTypeIndex]
    ON [dbo].[tblExportResults]([LookupExportResultTypeIndex] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblExportResults_SiteGuid]
    ON [dbo].[tblExportResults]([SiteGuid] ASC, [ExportResultGuid] ASC)
    INCLUDE([InterfaceName], [TransVersion], [FailedCount], [SuccessCount], [TransDateTime], [CreatedDate], [CreatedBy], [UpdatedDate], [UpdatedBy], [BatchID], [ArchiveFileName], [_RowVersion], [LookupExportResultTypeIndex]);


GO


CREATE INDEX [IX_tblExportResults_InterfaceName] ON [dbo].[tblExportResults] 
([InterfaceName]) INCLUDE ([ExportResultGuid])

GO
--Creating Insert / Update Trigger for tblExportResults
CREATE TRIGGER dbo.trg_insupd_tblExportResults_ForSync 
   ON dbo.tblExportResults
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
                    ,d.ExportResultGuid AS Deleted_PK_ExportResultGuid
                    ,i.ExportResultGuid AS Inserted_PK_ExportResultGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.ExportResultGuid = i.ExportResultGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblExportResults As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ExportResultGuid = currentTrackingData.PK_ExportResultGuid
 
 
		    INSERT track.tblExportResults (InsertedDate 
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
				    ,PK_ExportResultGuid
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
				    ,entityChanges.Inserted_PK_ExportResultGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblExportResults As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ExportResultGuid = currentTrackingData.PK_ExportResultGuid
)
    END
END 

GO
--Creating Delete Trigger for tblExportResults
CREATE TRIGGER dbo.trg_del_tblExportResults_ForSync 
   ON dbo.tblExportResults
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
						,d.ExportResultGuid AS Deleted_PK_ExportResultGuid
                        ,d.ExportResultGuid AS Inserted_PK_ExportResultGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblExportResults As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ExportResultGuid = currentTrackingData.PK_ExportResultGuid
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
						,PK_ExportResultGuid
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
						,entityChanges.Deleted_PK_ExportResultGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
