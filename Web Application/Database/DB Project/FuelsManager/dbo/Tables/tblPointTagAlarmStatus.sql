CREATE TABLE [dbo].[tblPointTagAlarmStatus]
(
	[PointTagAlarmStatusGuid] [uniqueidentifier] CONSTRAINT [DF_PointTagAlarmStatus_GUID] DEFAULT (newid()) NOT NULL,
	[AlarmTestGuid] [uniqueidentifier] NOT NULL,
	[Acknowledged] [Bit] CONSTRAINT [DF_PointTagAlarmStatus_Acknowledged] DEFAULT (0) NOT NULL,
	[AcknowledgedTimestamp] [datetimeoffset](7) NULL,
	[AcknowledgedBy] [dbo].[udtUserID] NULL,
	[AcknowledgedComment] [nvarchar](MAX) NULL,
	[Silenced] [Bit] CONSTRAINT [DF_PointTagAlarmStatus_Silenced] DEFAULT (0) NOT NULL,
	[SilencedTimestamp] [datetimeoffset](7) NULL,
	[SilencedBy] [dbo].[udtUserID] NULL,
	[AlarmTestFailed] BIT CONSTRAINT [DF_PointTagAlarmStatus_AlarmTestFailed] DEFAULT (0) NOT NULL,
	[AlarmTestFailedTimestamp] [datetimeoffset](7) CONSTRAINT [DF_PointTagAlarmStatus_AlarmTestFailedTimestamp] DEFAULT (sysdatetimeoffset()) NULL,
	[CreatedDate] [datetimeoffset](7) CONSTRAINT [DF_PointTagAlarmStatus_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[CreatedBy] [dbo].[udtUserID] CONSTRAINT [DF_PointTagAlarmStatus_CreatedBy] DEFAULT ('') NOT NULL,
	[UpdatedDate] [datetimeoffset](7) CONSTRAINT [DF_PointTagAlarmStatus_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
	[UpdatedBy] [dbo].[udtUserID] CONSTRAINT [DF_PointTagAlarmStatus_UpdatedBy] DEFAULT ('') NOT NULL,
	[_RowVersion] ROWVERSION NOT NULL,
    CONSTRAINT [PK_tblPointTagAlarmStatus_GUID] PRIMARY KEY NONCLUSTERED ([PointTagAlarmStatusGuid] ASC),
	CONSTRAINT [FK_tblPointTagAlarmStatus_AlarmTestGuid] FOREIGN KEY([AlarmTestGuid]) REFERENCES [dbo].[tblAlarmTest] ([AlarmTestGuid])
)

--Don't forget to change the tblPoint rowversion trigger is columns are added or deleted or column order changed

GO

CREATE NONCLUSTERED INDEX [IX_PointTagAlarmStatus_AlarmTestGuid]
    ON [dbo].[tblPointTagAlarmStatus]([AlarmTestGuid] ASC);
GO
--Creating Insert / Update Trigger for tblPointTagAlarmStatus
CREATE TRIGGER dbo.trg_insupd_tblPointTagAlarmStatus_ForSync 
   ON dbo.tblPointTagAlarmStatus
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
                    ,d.PointTagAlarmStatusGuid AS Deleted_PK_PointTagAlarmStatusGuid
                    ,i.PointTagAlarmStatusGuid AS Inserted_PK_PointTagAlarmStatusGuid
                    ,d.AlarmTestGuid AS Deleted_FK_ParentPK
                    ,i.AlarmTestGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.PointTagAlarmStatusGuid = i.PointTagAlarmStatusGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblPointTagAlarmStatus As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_PointTagAlarmStatusGuid = currentTrackingData.PK_PointTagAlarmStatusGuid
 
 
		    INSERT track.tblPointTagAlarmStatus (InsertedDate 
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
				    ,PK_PointTagAlarmStatusGuid
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
				    ,entityChanges.Inserted_PK_PointTagAlarmStatusGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblPointTagAlarmStatus As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_PointTagAlarmStatusGuid = currentTrackingData.PK_PointTagAlarmStatusGuid
)
    END
END
GO
--Creating Delete Trigger for tblPointTagAlarmStatus
CREATE TRIGGER dbo.trg_del_tblPointTagAlarmStatus_ForSync 
   ON dbo.tblPointTagAlarmStatus
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
						,d.PointTagAlarmStatusGuid AS Deleted_PK_PointTagAlarmStatusGuid
                        ,d.PointTagAlarmStatusGuid AS Inserted_PK_PointTagAlarmStatusGuid
                      ,d.AlarmTestGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblPointTagAlarmStatus As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_PointTagAlarmStatusGuid = currentTrackingData.PK_PointTagAlarmStatusGuid
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
						,PK_PointTagAlarmStatusGuid
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
						,entityChanges.Deleted_PK_PointTagAlarmStatusGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 
GO
