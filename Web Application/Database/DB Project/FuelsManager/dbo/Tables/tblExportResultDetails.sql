CREATE TABLE [dbo].[tblExportResultDetails] (
    [RecordID]               NVARCHAR (64)      NOT NULL,
    [Fail]                   BIT                NOT NULL,
    [TransVersion]           BIGINT             NULL,
    [CreatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportResultDetails_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]              [dbo].[udtUserID]  NOT NULL,
    [UpdatedDate]            DATETIMEOFFSET (7) CONSTRAINT [DF_tblExportResultDetails_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]              [dbo].[udtUserID]  NOT NULL,
    [Error]                  NVARCHAR (250)     NULL,
    [ExportResultDetailGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblExportResultDetails_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]            ROWVERSION         NOT NULL,
    [ExportResultGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [InterfaceData01]        NVARCHAR (100)     NULL,
    [InterfaceData02]        NVARCHAR (100)     NULL,
    [InterfaceData03]        NVARCHAR (100)     NULL,
    [InterfaceData04]        NVARCHAR (100)     NULL,
    [InterfaceData05]        NVARCHAR (100)     NULL,
    [InterfaceData06]        NVARCHAR (100)     NULL,
    [InterfaceData07]        NVARCHAR (100)     NULL,
    [InterfaceData08]        NVARCHAR (100)     NULL,
	[_ClusterIdx]				   BIGINT			  NOT NULL IDENTITY,
    CONSTRAINT [PK_tblExportResultDetails_GUID] PRIMARY KEY NONCLUSTERED ([ExportResultDetailGuid] ASC),
    CONSTRAINT [FK_tblExportResultDetails_ExportResultGuid] FOREIGN KEY ([ExportResultGuid]) REFERENCES [dbo].[tblExportResults] ([ExportResultGuid])
);


GO


CREATE UNIQUE CLUSTERED INDEX [IX_tblExportResultDetails_ClusterIdx] 
	ON [dbo].[tblExportResultDetails]([_ClusterIdx]);
GO


CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_CreatedDate]
    ON [dbo].[tblExportResultDetails]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_ExportResultGuid]
    ON [dbo].[tblExportResultDetails]([ExportResultGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_GuidRowVersion]
    ON [dbo].[tblExportResultDetails]([ExportResultDetailGuid] ASC, [_RowVersion] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblExportResultDetails_RecordID_TransVersion]
    ON [dbo].[tblExportResultDetails]([RecordID] ASC, [TransVersion] ASC);


GO
--Creating Insert / Update Trigger for tblExportResultDetails
CREATE TRIGGER dbo.trg_insupd_tblExportResultDetails_ForSync 
   ON dbo.tblExportResultDetails
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
                    ,d.ExportResultDetailGuid AS Deleted_PK_ExportResultDetailGuid
                    ,i.ExportResultDetailGuid AS Inserted_PK_ExportResultDetailGuid
                    ,d.ExportResultGuid AS Deleted_FK_ParentPK 
                    ,i.ExportResultGuid AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.ExportResultDetailGuid = i.ExportResultDetailGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblExportResultDetails As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_ExportResultDetailGuid = currentTrackingData.PK_ExportResultDetailGuid
 
 
		    INSERT track.tblExportResultDetails (InsertedDate 
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
				    ,PK_ExportResultDetailGuid
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
				    ,entityChanges.Inserted_PK_ExportResultDetailGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblExportResultDetails As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_ExportResultDetailGuid = currentTrackingData.PK_ExportResultDetailGuid
)
    END
END 

GO
--Creating Delete Trigger for tblExportResultDetails
CREATE TRIGGER dbo.trg_del_tblExportResultDetails_ForSync 
   ON dbo.tblExportResultDetails
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
						,d.ExportResultDetailGuid AS Deleted_PK_ExportResultDetailGuid
                        ,d.ExportResultDetailGuid AS Inserted_PK_ExportResultDetailGuid
                        ,d.ExportResultGuid AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblExportResultDetails As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_ExportResultDetailGuid = currentTrackingData.PK_ExportResultDetailGuid
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
						,PK_ExportResultDetailGuid
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
						,entityChanges.Deleted_PK_ExportResultDetailGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
