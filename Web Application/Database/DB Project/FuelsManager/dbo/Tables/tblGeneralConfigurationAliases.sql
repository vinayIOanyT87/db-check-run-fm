CREATE TABLE [dbo].[tblGeneralConfigurationAliases] (
    [AliasID]                       INT                NULL,
    [CreatedBy]                     [dbo].[udtUserID]  CONSTRAINT [DF_tblGeneralConfigurationAliases_CreatedBy] DEFAULT (suser_sname()) NULL,
    [CreatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblGeneralConfigurationAliases_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                     [dbo].[udtUserID]  CONSTRAINT [DF_tblGeneralConfigurationAliases_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblGeneralConfigurationAliases_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [GeneralConfigurationAliasGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblGeneralConfigurationAliases_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                   ROWVERSION         NOT NULL,
    [GeneralConfigurationGuid]      UNIQUEIDENTIFIER   NOT NULL,
    [TransactionAliasGuid]          UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblGeneralConfigurationAliases_GUID] PRIMARY KEY NONCLUSTERED ([GeneralConfigurationAliasGuid] ASC),
    CONSTRAINT [FK_tblGeneralConfigurationAliases_GeneralConfigurationGuid] FOREIGN KEY ([GeneralConfigurationGuid]) REFERENCES [dbo].[tblGeneralConfiguration] ([GeneralConfigurationGuid]),
    CONSTRAINT [FK_tblGeneralConfigurationAliases_TransactionAliasGuid] FOREIGN KEY ([TransactionAliasGuid]) REFERENCES [dbo].[tblTransactionAliases] ([TransactionAliasGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblGeneralConfigurationAliases_CreatedDate]
    ON [dbo].[tblGeneralConfigurationAliases]([CreatedDate] ASC);




GO
--Creating Insert / Update Trigger for tblGeneralConfigurationAliases
CREATE TRIGGER dbo.trg_insupd_tblGeneralConfigurationAliases_ForSync 
   ON dbo.tblGeneralConfigurationAliases
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
                    ,d.GeneralConfigurationAliasGuid AS Deleted_PK_GeneralConfigurationAliasGuid
                    ,i.GeneralConfigurationAliasGuid AS Inserted_PK_GeneralConfigurationAliasGuid
                    ,CAST(NULL AS uniqueidentifier) AS Deleted_FK_ParentPK 
                    ,CAST(NULL AS uniqueidentifier) AS Inserted_FK_ParentPK 
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.GeneralConfigurationAliasGuid = i.GeneralConfigurationAliasGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblGeneralConfigurationAliases As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_GeneralConfigurationAliasGuid = currentTrackingData.PK_GeneralConfigurationAliasGuid
 
 
		    INSERT track.tblGeneralConfigurationAliases (InsertedDate 
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
				    ,PK_GeneralConfigurationAliasGuid
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
				    ,entityChanges.Inserted_PK_GeneralConfigurationAliasGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblGeneralConfigurationAliases As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_GeneralConfigurationAliasGuid = currentTrackingData.PK_GeneralConfigurationAliasGuid
)
    END
END 

GO
--Creating Delete Trigger for tblGeneralConfigurationAliases
CREATE TRIGGER dbo.trg_del_tblGeneralConfigurationAliases_ForSync 
   ON dbo.tblGeneralConfigurationAliases
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
						,d.GeneralConfigurationAliasGuid AS Deleted_PK_GeneralConfigurationAliasGuid
                        ,d.GeneralConfigurationAliasGuid AS Inserted_PK_GeneralConfigurationAliasGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblGeneralConfigurationAliases As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_GeneralConfigurationAliasGuid = currentTrackingData.PK_GeneralConfigurationAliasGuid
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
						,PK_GeneralConfigurationAliasGuid
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
						,entityChanges.Deleted_PK_GeneralConfigurationAliasGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblGeneralConfigurationAliases_ClusterIdx]
    ON [dbo].[tblGeneralConfigurationAliases]([_ClusterIdx] ASC);

