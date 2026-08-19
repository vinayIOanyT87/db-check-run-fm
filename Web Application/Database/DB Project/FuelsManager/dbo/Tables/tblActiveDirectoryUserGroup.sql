CREATE TABLE [dbo].[tblActiveDirectoryUserGroup] (
    [ActiveDirectoryUserGroupGuid]           UNIQUEIDENTIFIER   CONSTRAINT [DF_tblActiveDirectoryUserGroup_GUID] DEFAULT (newid()) NOT NULL,
    [Name]  NVARCHAR (50)               CONSTRAINT [DF_tblActiveDirectoryUserGroup_Name] DEFAULT ('') NOT NULL,
    [Ssid]  NVARCHAR (50)               NULL,
    [CreatedBy]   [dbo].[udtUserID]     CONSTRAINT [DF_tblActiveDirectoryUserGroup_CreatedBy] DEFAULT ('') NOT NULL,
    [CreatedDate] DATETIMEOFFSET (7)    CONSTRAINT [DF_tblActiveDirectoryUserGroup_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]   [dbo].[udtUserID]     CONSTRAINT [DF_tblActiveDirectoryUserGroup_UpdatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate] DATETIMEOFFSET (7)    CONSTRAINT [DF_tblActiveDirectoryUserGroup_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [_RowVersion] ROWVERSION            NOT NULL,
    [_ClusterIdx] BIGINT                IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblActiveDirectoryUserGroup_GUID] PRIMARY KEY NONCLUSTERED ([ActiveDirectoryUserGroupGuid] ASC)
);
GO


CREATE TRIGGER dbo.trg_insupd_tblActiveDirectoryUserGroup_ForSync
   ON dbo.tblActiveDirectoryUserGroup
   AFTER INSERT, UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    
	IF NOT EXISTS(SELECT 1 FROM inserted)
	BEGIN
		RETURN;
	END

    DECLARE @changeContextName nvarchar(100)
    DECLARE @bypassTrackingFlags int
    DECLARE @bypassReason nvarchar(512)
    
    SELECT @changeContextName = ContextName
            ,@bypassTrackingFlags = BypassTrackingFlags
            ,@bypassReason = BypassReason 
        FROM [track].[udf_GetChangeTrackingSessionDetails]()

	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application insert.
    DECLARE @syncContext varbinary(128)
    DECLARE @currentDateTimeOffset datetimeoffset(7)

	SET @syncContext = dbo.udf_GetSyncContext();
    SET @currentDateTimeOffset = sysdatetimeoffset();

	; WITH ChangeList AS (
		SELECT @syncContext AS ChangeContext
                ,d.ActiveDirectoryUserGroupGuid AS Deleted_PK_ActiveDirectoryUserGroupGuid
                ,i.ActiveDirectoryUserGroupGuid AS Inserted_PK_ActiveDirectoryUserGroupGuid
				,i.CreatedDate AS Inserted_CreatedDate
				,i.UpdatedDate AS Inserted_UpdatedDate
				,NULL AS CurrentSiteGuid
				,NULL AS PreviousSiteGuid
				,i._RowVersion AS Inserted_RowVersion
				,NULL AS Deleted_RowVersion
		FROM Inserted i
			FULL OUTER JOIN Deleted d ON 
            d.ActiveDirectoryUserGroupGuid = i.ActiveDirectoryUserGroupGuid            
	)
	MERGE INTO track.tblActiveDirectoryUserGroup  As currentTrackingData
		USING ChangeList As entityChanges
			ON entityChanges.Inserted_PK_ActiveDirectoryUserGroupGuid = currentTrackingData.PK_ActiveDirectoryUserGroupGuid
	WHEN Matched  AND entityChanges.CurrentSiteGuid = currentTrackingData.CurrentSiteGuid
	THEN 
		UPDATE SET UpdatedDate = entityChanges.Inserted_UpdatedDate
									,UpdatedContext = entityChanges.ChangeContext
									,UpdatedRowVersion = entityChanges.Inserted_RowVersion
									,CurrentSiteGuid = entityChanges.CurrentSiteGuid
									,PreviousSiteGuid = currentTrackingData.PreviousSiteGuid
	WHEN Not Matched 
	THEN 
	INSERT (InsertedDate
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
            ,PK_ActiveDirectoryUserGroupGuid
            )
	VALUES (entityChanges.Inserted_CreatedDate
			,entityChanges.ChangeContext
			,entityChanges.Inserted_RowVersion
			,entityChanges.Inserted_CreatedDate
			,entityChanges.ChangeContext
			,entityChanges.Inserted_RowVersion
            ,NULL
			,NULL
			,NULL
			,entityChanges.CurrentSiteGuid
			,CASE WHEN (entityChanges.PreviousSiteGuid <> entityChanges.CurrentSiteGuid) THEN entityChanges.PreviousSiteGuid ELSE NULL END
            ,entityChanges.Inserted_PK_ActiveDirectoryUserGroupGuid
            )
	; 
END
GO


CREATE TRIGGER [dbo].[trg_del_tblActiveDirectoryUserGroup_ForSync]
   ON [dbo].[tblActiveDirectoryUserGroup]
   AFTER DELETE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	-- Get the synchronization context.  This will be NULL if this trigger was fired
	-- due to a normal application insert.
    DECLARE @syncContext varbinary(128)
    DECLARE @currentDateTimeOffset datetimeoffset(7)

    DECLARE @changeContextName nvarchar(100)
    DECLARE @bypassTrackingFlags int
    DECLARE @bypassReason nvarchar(512)
    
    SELECT @changeContextName = ContextName
            ,@bypassTrackingFlags = BypassTrackingFlags
            ,@bypassReason = BypassReason 
        FROM [track].[udf_GetChangeTrackingSessionDetails]()

	SET @syncContext = dbo.udf_GetSyncContext();
    SET @currentDateTimeOffset = sysdatetimeoffset();

    IF ([track].[udf_IsDeleteChangeTrackingEnabled](@bypassTrackingFlags) = 0)
    BEGIN
    	; WITH ChangeList AS (
    		SELECT @syncContext AS ChangeContext
                    ,d.ActiveDirectoryUserGroupGuid AS Deleted_PK_ActiveDirectoryUserGroupGuid
                    ,d.ActiveDirectoryUserGroupGuid AS Inserted_PK_ActiveDirectoryUserGroupGuid
    				,d.CreatedDate AS Inserted_CreatedDate
    				,d.UpdatedDate AS Inserted_UpdatedDate
    				,NULL AS CurrentSiteGuid
    				,NULL AS PreviousSiteGuid
    				,d._RowVersion AS Inserted_RowVersion
    				,NULL AS Deleted_RowVersion
    		FROM Deleted d 
    	)
    	MERGE INTO track.tblActiveDirectoryUserGroup  As currentTrackingData
    		USING ChangeList As entityChanges
    			ON entityChanges.Inserted_PK_ActiveDirectoryUserGroupGuid = currentTrackingData.PK_ActiveDirectoryUserGroupGuid
    	WHEN Matched 
    	THEN 
    		UPDATE SET DeletedDate = @currentDateTimeOffset
    									,DeletedContext = entityChanges.ChangeContext
    									,DeletedRowVersion = entityChanges.Deleted_RowVersion
    									,CurrentSiteGuid = entityChanges.CurrentSiteGuid
    									,PreviousSiteGuid = CASE WHEN (entityChanges.CurrentSiteGuid <> currentTrackingData.CurrentSiteGuid) THEN currentTrackingData.CurrentSiteGuid
                                                                    ELSE currentTrackingData.PreviousSiteGuid END
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
                ,PK_ActiveDirectoryUserGroupGuid
                )
    	VALUES (entityChanges.Inserted_CreatedDate
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
                ,entityChanges.Deleted_PK_ActiveDirectoryUserGroupGuid
                )
    	; 
    END
END

GO

