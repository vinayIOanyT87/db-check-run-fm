/*
	DROP TABLE [erv].[tblGlobalSpecificChangesQueue]
*/
CREATE TABLE [erv].[tblGlobalSpecificChangesQueue](
	[GSQueueGuid]		UNIQUEIDENTIFIER NOT NULL CONSTRAINT [DF_tblGlobalSpecificChangesQueue_GUID]  DEFAULT (newid()),
	[EntityTypeId]		NVARCHAR (100) NOT NULL,
	[EntityGuid]		UNIQUEIDENTIFIER NOT NULL,
	[MasterRecordGuid]	UNIQUEIDENTIFIER NOT NULL,
	[SiteGuid]			UNIQUEIDENTIFIER NOT NULL,
	[BatchProcessingMarker]	UNIQUEIDENTIFIER NULL,
	[CreatedDate]		DATETIMEOFFSET (7) NOT NULL CONSTRAINT [DF_tblGlobalSpecificChangesQueue_CreatedDate]  DEFAULT (sysdatetimeoffset()),
	[CreatedBy]			[dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblGlobalSpecificChangesQueue_CreatedBy]  DEFAULT (''),
	[UpdatedDate]		DATETIMEOFFSET (7) NOT NULL CONSTRAINT [DF_tblGlobalSpecificChangesQueue_UpdatedDate]  DEFAULT (sysdatetimeoffset()),
	[UpdatedBy]			[dbo].[udtUserID] NOT NULL CONSTRAINT [DF_tblGlobalSpecificChangesQueue_UpdatedBy]  DEFAULT (''),
	[_RowVersion]		ROWVERSION NOT NULL,
	[_ClusterIdx]       BIGINT IDENTITY (1, 1) NOT NULL,
 CONSTRAINT [PK_tblGlobalSpecificChangesQueue] PRIMARY KEY NONCLUSTERED 
(
	[GSQueueGuid] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [erv].[tblGlobalSpecificChangesQueue]  WITH CHECK ADD  CONSTRAINT [FK_tblGlobalSpecificChangesQueue_tblSites] FOREIGN KEY([SiteGuid])
REFERENCES [dbo].[tblSites] ([SiteGuid])
GO

ALTER TABLE [erv].[tblGlobalSpecificChangesQueue] CHECK CONSTRAINT [FK_tblGlobalSpecificChangesQueue_tblSites]
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblGlobalSpecificChangesQueue_ClusterIdx] ON [erv].[tblGlobalSpecificChangesQueue]
(
	[_ClusterIdx] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO

CREATE NONCLUSTERED INDEX [IXU_tblGlobalSpecificChangesQueue_001] ON [erv].[tblGlobalSpecificChangesQueue]
(
	[EntityTypeId] ASC,
	[SiteGuid] ASC,
	[EntityGuid] ASC
)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
GO
--Creating Insert / Update Trigger for tblGlobalSpecificChangesQueue
CREATE TRIGGER erv.trg_insupd_tblGlobalSpecificChangesQueue_ForSync 
   ON erv.tblGlobalSpecificChangesQueue
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
                    ,d.GSQueueGuid AS Deleted_PK_GSQueueGuid
                    ,i.GSQueueGuid AS Inserted_PK_GSQueueGuid
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
				    d.GSQueueGuid = i.GSQueueGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblGlobalSpecificChangesQueue As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_GSQueueGuid = currentTrackingData.PK_GSQueueGuid
 
 
		    INSERT track.tblGlobalSpecificChangesQueue (InsertedDate 
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
				    ,PK_GSQueueGuid
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
				    ,entityChanges.Inserted_PK_GSQueueGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblGlobalSpecificChangesQueue As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_GSQueueGuid = currentTrackingData.PK_GSQueueGuid
)
    END
END
GO
--Creating Delete Trigger for tblGlobalSpecificChangesQueue
CREATE TRIGGER erv.trg_del_tblGlobalSpecificChangesQueue_ForSync 
   ON erv.tblGlobalSpecificChangesQueue
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
						,d.GSQueueGuid AS Deleted_PK_GSQueueGuid
                        ,d.GSQueueGuid AS Inserted_PK_GSQueueGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblGlobalSpecificChangesQueue As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_GSQueueGuid = currentTrackingData.PK_GSQueueGuid
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
						,PK_GSQueueGuid
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
						,entityChanges.Deleted_PK_GSQueueGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END
GO
