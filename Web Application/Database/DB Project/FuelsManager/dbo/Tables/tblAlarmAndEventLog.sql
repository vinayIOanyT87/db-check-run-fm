CREATE TABLE [dbo].[tblAlarmAndEventLog] (
    [SequenceNumber]       BIGINT             IDENTITY (1, 1) NOT NULL,
    [Source]               NVARCHAR (120)     CONSTRAINT [DF_tblAlarmAndEventLog_Source] DEFAULT ('') NOT NULL,
    [Alarm]                BIT                CONSTRAINT [DF_tblAlarmAndEventLog_Alarm] DEFAULT ((0)) NOT NULL,
    [ID]                   NVARCHAR (120)     CONSTRAINT [DF_tblAlarmAndEventLog_ID] DEFAULT ('') NOT NULL,
    [AssociatedData]       NVARCHAR (MAX)     CONSTRAINT [DF_tblAlarmAndEventLog_AssociatedData] DEFAULT ('') NOT NULL,
    [CategoryID]           NVARCHAR (50)      CONSTRAINT [DF_tblAlarmAndEventLog_CategoryID] DEFAULT ('') NOT NULL,
    [PriorityID]           NVARCHAR (50)      CONSTRAINT [DF_tblAlarmAndEventLog_PriorityID] DEFAULT ('') NOT NULL,
    [Acknowledged]         BIT                CONSTRAINT [DF_tblAlarmAndEventLog_Acknowledged] DEFAULT ((0)) NOT NULL,
    [CreatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAlarmAndEventLog_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblAlarmAndEventLog_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAlarmAndEventLog_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]            [dbo].[udtUserID]  CONSTRAINT [DF_tblAlarmAndEventLog_UpdatedBy] DEFAULT ('') NOT NULL,
    [AlarmAndEventLogGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblAlarmAndEventLog_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]          ROWVERSION         NOT NULL,
    [SiteGuid]             UNIQUEIDENTIFIER   NOT NULL,
    [SourceNode]		  NVARCHAR (256)     NULL,
    CONSTRAINT [PK_tblAlarmAndEventLog_GUID] PRIMARY KEY NONCLUSTERED ([AlarmAndEventLogGuid] ASC),
    CONSTRAINT [FK_tblAlarmAndEventLog_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);
GO

CREATE NONCLUSTERED INDEX [IX_tblAlarmAndEventLog_SiteGuid_CreatedDate]
    ON [dbo].[tblAlarmAndEventLog]([SiteGuid] ASC, [CreatedDate] ASC);
GO

CREATE UNIQUE CLUSTERED INDEX [IX_tblAlarmAndEventLog_SequenceNumber]
    ON [dbo].[tblAlarmAndEventLog]([SequenceNumber] ASC);
GO
--Creating Insert / Update Trigger for tblAlarmAndEventLog
CREATE TRIGGER dbo.trg_insupd_tblAlarmAndEventLog_ForSync 
   ON dbo.tblAlarmAndEventLog
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
                    ,d.AlarmAndEventLogGuid AS Deleted_PK_AlarmAndEventLogGuid
                    ,i.AlarmAndEventLogGuid AS Inserted_PK_AlarmAndEventLogGuid
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
				    d.AlarmAndEventLogGuid = i.AlarmAndEventLogGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAlarmAndEventLog As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AlarmAndEventLogGuid = currentTrackingData.PK_AlarmAndEventLogGuid
 
 
		    INSERT track.tblAlarmAndEventLog (InsertedDate 
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
				    ,PK_AlarmAndEventLogGuid
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
				    ,entityChanges.Inserted_PK_AlarmAndEventLogGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAlarmAndEventLog As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AlarmAndEventLogGuid = currentTrackingData.PK_AlarmAndEventLogGuid
)
    END
END
GO
--Creating Delete Trigger for tblAlarmAndEventLog
CREATE TRIGGER dbo.trg_del_tblAlarmAndEventLog_ForSync 
   ON dbo.tblAlarmAndEventLog
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
						,d.AlarmAndEventLogGuid AS Deleted_PK_AlarmAndEventLogGuid
                        ,d.AlarmAndEventLogGuid AS Inserted_PK_AlarmAndEventLogGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAlarmAndEventLog As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AlarmAndEventLogGuid = currentTrackingData.PK_AlarmAndEventLogGuid
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
						,PK_AlarmAndEventLogGuid
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
						,entityChanges.Deleted_PK_AlarmAndEventLogGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END