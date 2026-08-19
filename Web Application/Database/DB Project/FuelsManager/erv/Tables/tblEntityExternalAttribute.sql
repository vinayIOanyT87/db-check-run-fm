CREATE TABLE [erv].[tblEntityExternalAttribute] (
    [EntityExternalAttributeGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_tblEntityExternalAttribute_GUID] DEFAULT (newid()) NOT NULL,
    [EntitySegmentTemplateGuid]   UNIQUEIDENTIFIER   NOT NULL,
    [InternalFieldName]           NVARCHAR (100)     NULL,
    [RelationshipTableName]       VARCHAR (250)      NULL,
    [RelationshipName]            NVARCHAR (100)     NULL,
    [CreatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEntityExternalAttribute_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblEntityExternalAttribute_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                 DATETIMEOFFSET (7) CONSTRAINT [DF_tblEntityExternalAttribute_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                   [dbo].[udtUserID]  CONSTRAINT [DF_tblEntityExternalAttribute_UpdatedBy] DEFAULT ('') NOT NULL,
    [_RowVersion]                 ROWVERSION         NOT NULL,
    [_ClusterIdx]                 BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblEntityExternalAttribute] PRIMARY KEY NONCLUSTERED ([EntityExternalAttributeGuid] ASC),
    CONSTRAINT [CK_tblEntityExternalAttribute_RelationshipName] CHECK ([erv].[udf_IsFieldNameUsed]([EntitySegmentTemplateGuid],[RelationshipName])=(0)),
    CONSTRAINT [FK_tblEntityExternalAttribute_tblEntitySegmentTemplate] FOREIGN KEY ([EntitySegmentTemplateGuid]) REFERENCES [erv].[tblEntitySegmentTemplate] ([EntitySegmentTemplateGuid])
);


GO



GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblEntityExternalAttribute_001]
    ON [erv].[tblEntityExternalAttribute]([EntitySegmentTemplateGuid] ASC, [RelationshipName] ASC);


GO


CREATE TRIGGER [erv].[TRG_EntityExternalAttribute_RECVER_INS_UPD]
    ON [erv].[tblEntityExternalAttribute]
    AFTER INSERT, UPDATE
AS BEGIN
	------------------------------------------------------------------------------------------------------
	-- Trigger: [erv].[EntityExternalAttribute_RECVER_INS_UPD] 
	-- Author: Hansraj Bapoo
	-- Version/Date: 1.0.003 / 2012-07-13 14:21:10.4470770 -04:00
	-- Purpose: Checks the InternalFieldName value during inserts and updates to table tblEntityExternalAttribute, and make sure that the field is
	-- valid and does not correspond to a filter field defined on an Entity Segment Template. This prevents users from being able to configure the 
	-- filter field as VersionSpecific, and therefore prevents the filter field on a record version to be changed further down the site hierarchy, 
	-- in its child record versions.
	------------------------------------------------------------------------------------------------------

	SET NOCOUNT ON;
	IF ((SELECT COUNT(*) FROM Inserted a
	INNER JOIN erv.tblEntitySegmentTemplate b
	ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
	WHERE a.InternalFieldName = b.FilterFieldName) > 0)
	BEGIN
		RAISERROR('Cannot set up an External Attribute on a field that is used as a Filter Field in an Entity Segmment Template.',16,1); 
		ROLLBACK TRANSACTION
	END

	IF ((SELECT COUNT(*) FROM sys.columns a
		INNER JOIN Inserted b
		ON b.RelationshipName = a.Name
		INNER JOIN erv.tblEntitySegmentTemplate c
		ON c.EntitySegmentTemplateGuid = b.EntitySegmentTemplateGuid
		WHERE a.object_id = OBJECT_ID(c.AppTableName)) > 0)
	BEGIN
		RAISERROR('Cannot set up an External Attribute with a name that corresponds to an internal field name of the entity table.',16,1); 
		ROLLBACK TRANSACTION
	END

	IF ((SELECT COUNT(*) FROM Inserted a
		WHERE a.InternalFieldName IS NOT NULL) > 0)
	BEGIN
		IF ((SELECT COUNT(*) FROM Inserted a
		INNER JOIN erv.tblEntitySegmentTemplate b
		ON b.EntitySegmentTemplateGuid = a.EntitySegmentTemplateGuid
		WHERE a.InternalFieldName IS NOT NULL
		AND NOT EXISTS
		(
			SELECT * FROM sys.columns c
			WHERE c.object_id = OBJECT_ID(b.AppTableName)  
			AND c.name = a.InternalFieldName
		) ) > 0)
		BEGIN
			RAISERROR('The InternalFieldName does not correspond to a field of the entity table.',16,1); 
			ROLLBACK TRANSACTION
		END
	END
		
END
GO
--Creating Insert / Update Trigger for tblEntityExternalAttribute
CREATE TRIGGER erv.trg_insupd_tblEntityExternalAttribute_ForSync 
   ON erv.tblEntityExternalAttribute
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
                    ,d.EntityExternalAttributeGuid AS Deleted_PK_EntityExternalAttributeGuid
                    ,i.EntityExternalAttributeGuid AS Inserted_PK_EntityExternalAttributeGuid
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
				    d.EntityExternalAttributeGuid = i.EntityExternalAttributeGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblEntityExternalAttribute As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_EntityExternalAttributeGuid = currentTrackingData.PK_EntityExternalAttributeGuid
 
 
		    INSERT track.tblEntityExternalAttribute (InsertedDate 
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
				    ,PK_EntityExternalAttributeGuid
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
				    ,entityChanges.Inserted_PK_EntityExternalAttributeGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblEntityExternalAttribute As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_EntityExternalAttributeGuid = currentTrackingData.PK_EntityExternalAttributeGuid
)
    END
END 

GO
--Creating Delete Trigger for tblEntityExternalAttribute
CREATE TRIGGER erv.trg_del_tblEntityExternalAttribute_ForSync 
   ON erv.tblEntityExternalAttribute
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
						,d.EntityExternalAttributeGuid AS Deleted_PK_EntityExternalAttributeGuid
                        ,d.EntityExternalAttributeGuid AS Inserted_PK_EntityExternalAttributeGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblEntityExternalAttribute As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_EntityExternalAttributeGuid = currentTrackingData.PK_EntityExternalAttributeGuid
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
						,PK_EntityExternalAttributeGuid
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
						,entityChanges.Deleted_PK_EntityExternalAttributeGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE NONCLUSTERED INDEX [IX_tblEntityExternalAttribute_CreatedDate]
    ON [erv].[tblEntityExternalAttribute]([CreatedDate] ASC);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblEntityExternalAttribute_ClusterIdx]
    ON [erv].[tblEntityExternalAttribute]([_ClusterIdx] ASC);

