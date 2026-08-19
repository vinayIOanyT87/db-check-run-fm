CREATE TABLE [lookup].[tblDispatchGridColumnType] (
    [DispatchGridColumnTypeIndex] INT                NOT NULL,
    [LookupDispatchGridTypeIndex] INT                NOT NULL,
    [DispatchGridColumnTypeGuid]  UNIQUEIDENTIFIER   CONSTRAINT [DF_lookup_tblDispatchGridColumnType_GUID] DEFAULT (newid()) NOT NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblDispatchGridColumnType_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblDispatchGridColumnType_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_lookup_tblDispatchGridColumnType_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_lookup_tblDispatchGridColumnType_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [ID]                          NVARCHAR (100)     NOT NULL,
    [DisplayName]                 NVARCHAR (100)     NOT NULL,
    [DataField]                   NVARCHAR (100)     NOT NULL,
    [Width]                       INT                CONSTRAINT [DF_lookup_tblDispatchGridColumnType_Width] DEFAULT ((60)) NOT NULL,
    [DefaultColumnOrder]          INT                CONSTRAINT [DF_lookup_tblDispatchGridColumnType_DefaultColumnOrder] DEFAULT ((-1)) NOT NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_lookup_tblDispatchGridColumnType] PRIMARY KEY NONCLUSTERED ([DispatchGridColumnTypeIndex] ASC),
    CONSTRAINT [FK_tblDispatchGridColumnType_LookupDispatchGridTypeIndex] FOREIGN KEY ([LookupDispatchGridTypeIndex]) REFERENCES [lookup].[tblDispatchGridType] ([DispatchGridTypeIndex])
);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblDispatchGridColumnType_DispatchGridColumnTypeGuid]
    ON [lookup].[tblDispatchGridColumnType]([DispatchGridColumnTypeGuid] ASC);


GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_lookup_tblDispatchGridColumnType_LookupDispatchGridTypeIndex_ID]
    ON [lookup].[tblDispatchGridColumnType]([LookupDispatchGridTypeIndex] ASC, [ID] ASC);


GO
--Creating Insert / Update Trigger for tblDispatchGridColumnType
CREATE TRIGGER lookup.trg_insupd_tblDispatchGridColumnType_ForSync 
   ON lookup.tblDispatchGridColumnType
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
                    ,d.DispatchGridColumnTypeIndex AS Deleted_PK_DispatchGridColumnTypeIndex
                    ,i.DispatchGridColumnTypeIndex AS Inserted_PK_DispatchGridColumnTypeIndex
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
				    d.DispatchGridColumnTypeIndex = i.DispatchGridColumnTypeIndex
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblDispatchGridColumnType As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_DispatchGridColumnTypeIndex = currentTrackingData.PK_DispatchGridColumnTypeIndex
 
 
		    INSERT track.tblDispatchGridColumnType (InsertedDate 
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
				    ,PK_DispatchGridColumnTypeIndex
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
				    ,entityChanges.Inserted_PK_DispatchGridColumnTypeIndex
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblDispatchGridColumnType As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_DispatchGridColumnTypeIndex = currentTrackingData.PK_DispatchGridColumnTypeIndex
)
    END
END 

GO
--Creating Delete Trigger for tblDispatchGridColumnType
CREATE TRIGGER lookup.trg_del_tblDispatchGridColumnType_ForSync 
   ON lookup.tblDispatchGridColumnType
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
						,d.DispatchGridColumnTypeIndex AS Deleted_PK_DispatchGridColumnTypeIndex
                        ,d.DispatchGridColumnTypeIndex AS Inserted_PK_DispatchGridColumnTypeIndex
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblDispatchGridColumnType As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_DispatchGridColumnTypeIndex = currentTrackingData.PK_DispatchGridColumnTypeIndex
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
						,PK_DispatchGridColumnTypeIndex
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
						,entityChanges.Deleted_PK_DispatchGridColumnTypeIndex
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDispatchGridColumnType_ClusterIdx]
    ON [lookup].[tblDispatchGridColumnType]([_ClusterIdx] ASC);

