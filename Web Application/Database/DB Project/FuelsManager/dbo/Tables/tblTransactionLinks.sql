CREATE TABLE [dbo].[tblTransactionLinks] (
    [OriginalTransID]               NVARCHAR (64)      CONSTRAINT [DF_tblTransactionLinks_OriginalTransID] DEFAULT ('') NOT NULL,
    [LinkedTransID]                 NVARCHAR (64)      CONSTRAINT [DF_tblTransactionLinks_LinkedTransID] DEFAULT ('') NOT NULL,
    [Level]                         INT                CONSTRAINT [DF_tblTransactionLinks_Level] DEFAULT ((0)) NOT NULL,
    [CreatedBy]                     [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionLinks_CreatedBy] DEFAULT ('') NOT NULL,
    [CreatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionLinks_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                     [dbo].[udtUserID]  CONSTRAINT [DF_tblTransactionLinks_UpdatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                   DATETIMEOFFSET (7) CONSTRAINT [DF_tblTransactionLinks_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [TransactionLinkGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionLinks_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                   ROWVERSION         NOT NULL,
    [SiteGuid]                      UNIQUEIDENTIFIER   NOT NULL,
    [LinkedTransactionLineItemGuid] UNIQUEIDENTIFIER   NULL,
    [TransactionLineItemGuid]       UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]                   BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionLinks_GUID] PRIMARY KEY NONCLUSTERED ([TransactionLinkGuid] ASC),
    CONSTRAINT [FK_tblTransactionLinks_LinkedTransactionLineItemGuid] FOREIGN KEY ([LinkedTransactionLineItemGuid]) REFERENCES [dbo].[tblTransactionLineItems] ([TransactionLineItemGuid]),
    CONSTRAINT [FK_tblTransactionLinks_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblTransactionLinks_TransactionLineItemGuid] FOREIGN KEY ([TransactionLineItemGuid]) REFERENCES [dbo].[tblTransactionLineItems] ([TransactionLineItemGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_CreatedDate]
    ON [dbo].[tblTransactionLinks]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_LinkedTransactionLineItemGuid]
    ON [dbo].[tblTransactionLinks]([LinkedTransactionLineItemGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_LinkedTransID]
    ON [dbo].[tblTransactionLinks]([LinkedTransID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionLinks_OriginalTransID_TransactionLineItemGuid]
    ON [dbo].[tblTransactionLinks]([OriginalTransID] ASC, [TransactionLineItemGuid] ASC);


GO
--Creating Insert / Update Trigger for tblTransactionLinks
CREATE TRIGGER dbo.trg_insupd_tblTransactionLinks_ForSync 
   ON dbo.tblTransactionLinks
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
                    ,d.TransactionLinkGuid AS Deleted_PK_TransactionLinkGuid
                    ,i.TransactionLinkGuid AS Inserted_PK_TransactionLinkGuid
                    ,d.TransactionLineItemGuid AS Deleted_FK_ParentPK
                    ,i.TransactionLineItemGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
                    ,i.SiteGuid AS CurrentSiteGuid 
                    ,d.SiteGuid AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TransactionLinkGuid = i.TransactionLinkGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionLinks As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionLinkGuid = currentTrackingData.PK_TransactionLinkGuid
 
 
		    INSERT track.tblTransactionLinks (InsertedDate 
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
				    ,PK_TransactionLinkGuid
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
				    ,entityChanges.Inserted_PK_TransactionLinkGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionLinks As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionLinkGuid = currentTrackingData.PK_TransactionLinkGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionLinks
CREATE TRIGGER dbo.trg_del_tblTransactionLinks_ForSync 
   ON dbo.tblTransactionLinks
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
						,d.TransactionLinkGuid AS Deleted_PK_TransactionLinkGuid
                        ,d.TransactionLinkGuid AS Inserted_PK_TransactionLinkGuid
                      ,d.TransactionLineItemGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionLinks As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionLinkGuid = currentTrackingData.PK_TransactionLinkGuid
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
						,PK_TransactionLinkGuid
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
						,entityChanges.Deleted_PK_TransactionLinkGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionLinks_ClusterIdx]
    ON [dbo].[tblTransactionLinks]([_ClusterIdx] ASC);

