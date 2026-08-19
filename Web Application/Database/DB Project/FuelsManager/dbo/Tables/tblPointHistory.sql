CREATE TABLE [dbo].[tblPointHistory](
	[PointHistoryGuid] [uniqueidentifier] NOT NULL CONSTRAINT [DF_tblPointHistory_GUID]  DEFAULT (newid()),
	[UserGuid] [uniqueidentifier] NOT NULL,
	[SiteGuid] [uniqueidentifier] NOT NULL,
	[StartDate] [datetimeoffset](7) NOT NULL,
	[IntervalQuantity] int NOT NULL,
	[IntervalType] int NOT NULL,
	[RangeQuantity] int NOT NULL,
	[RangeType] int NOT NULL,
	[ColumnsDefinition] [nvarchar](MAX) NOT NULL CONSTRAINT [DF_tblPointHistoryColumns_ColumnsDefinition]  DEFAULT (''),
	[CreatedDate] [datetimeoffset](7) CONSTRAINT [DF_PointHistory_CreatedDate]  DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_PointHistory_CreatedBy]  DEFAULT ('') NOT NULL,
	[UpdatedDate] [datetimeoffset](7) CONSTRAINT [DF_PointHistory_UpdatedDate]  DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_PointHistory_UpdatedBy]  DEFAULT ('') NOT NULL,
	[_RowVersion] [timestamp] NOT NULL,
	[_ClusterIdx] BIGINT NOT NULL IDENTITY,
	CONSTRAINT [PK_tblPointHistory_GUID] PRIMARY KEY NONCLUSTERED ([PointHistoryGuid] ASC),
	CONSTRAINT [FK_tblPointHistory_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
	CONSTRAINT [FK_tblPointHistory_UserGuid] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers] ([UserGuid])
) ON [PRIMARY]
GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblPointHistory_ClusterIdx] 
	ON [dbo].[tblPointHistory]([_ClusterIdx]);
GO

CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblPointHistory_SiteGuid_UserGuid] ON [dbo].tblPointHistory
(
	SiteGuid ASC,
	UserGuid ASC
) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
GO
--Creating Insert / Update Trigger for tblPointHistory
CREATE TRIGGER dbo.trg_insupd_tblPointHistory_ForSync 
   ON dbo.tblPointHistory
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
                    ,d.PointHistoryGuid AS Deleted_PK_PointHistoryGuid
                    ,i.PointHistoryGuid AS Inserted_PK_PointHistoryGuid
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
				    d.PointHistoryGuid = i.PointHistoryGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblPointHistory As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_PointHistoryGuid = currentTrackingData.PK_PointHistoryGuid
 
 
		    INSERT track.tblPointHistory (InsertedDate 
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
				    ,PK_PointHistoryGuid
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
				    ,entityChanges.Inserted_PK_PointHistoryGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblPointHistory As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_PointHistoryGuid = currentTrackingData.PK_PointHistoryGuid
)
    END
END 
GO
--Creating Delete Trigger for tblPointHistory
CREATE TRIGGER dbo.trg_del_tblPointHistory_ForSync 
   ON dbo.tblPointHistory
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
						,d.PointHistoryGuid AS Deleted_PK_PointHistoryGuid
                        ,d.PointHistoryGuid AS Inserted_PK_PointHistoryGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblPointHistory As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_PointHistoryGuid = currentTrackingData.PK_PointHistoryGuid
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
						,PK_PointHistoryGuid
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
						,entityChanges.Deleted_PK_PointHistoryGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
