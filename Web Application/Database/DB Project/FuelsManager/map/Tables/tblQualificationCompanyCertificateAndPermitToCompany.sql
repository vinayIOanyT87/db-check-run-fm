CREATE TABLE [map].[tblQualificationCompanyCertificateAndPermitToCompany] (
    [QualificationCompanyCertificateAndPermitToCompanyGuid] UNIQUEIDENTIFIER   CONSTRAINT [DF_map_tblQualificationCompanyCertificateAndPermitToCompanyGUID] DEFAULT (newid()) NOT NULL,
    [QualificationGuid]                                     UNIQUEIDENTIFIER   NOT NULL,
    [CompanyGuid]                                           UNIQUEIDENTIFIER   NOT NULL,
    [Sequence]                                              INT                NOT NULL,
    [Instructor]                                            NVARCHAR (50)      NULL,
    [DateCompleted]                                         DATETIMEOFFSET (7) NULL,
    [DateDue]                                               DATETIMEOFFSET (7) NULL,
    [ExpirationDate]                                        DATETIMEOFFSET (7) NULL,
    [ID]                                                    VARCHAR (50)       NULL,
    [Rating]                                                NVARCHAR (20)      NULL,
    [HistoricalRecord]                                      BIT                CONSTRAINT [DF_map_tblQualificationCompanyCertificateAndPermitToCompany_HistoricalRecord] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                                           DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblQualificationCompanyCertificateAndPermitToCompany_CreatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [CreatedBy]                                             [dbo].[udtUserID]  CONSTRAINT [DF_map_tblQualificationCompanyCertificateAndPermitToCompany_CreatedBy] DEFAULT (suser_sname()) NULL,
    [UpdatedDate]                                           DATETIMEOFFSET (7) CONSTRAINT [DF_map_tblQualificationCompanyCertificateAndPermitToCompany_UpdatedDate] DEFAULT (sysdatetimeoffset()) NULL,
    [UpdatedBy]                                             [dbo].[udtUserID]  CONSTRAINT [DF_map_tblQualificationCompanyCertificateAndPermitToCompany_UpdatedBy] DEFAULT (suser_sname()) NULL,
    [_RowVersion]                                           ROWVERSION         NOT NULL,
    [_ClusterIdx]                                           BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_map_tblQualificationCompanyCertificateAndPermitToCompany] PRIMARY KEY NONCLUSTERED ([QualificationCompanyCertificateAndPermitToCompanyGuid] ASC),
    CONSTRAINT [FK_map_tblQualificationCompanyCertificateAndPermitToCompany_CompanyGuid] FOREIGN KEY ([CompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_map_tblQualificationCompanyCertificateAndPermitToCompany_QualificationGuid] FOREIGN KEY ([QualificationGuid]) REFERENCES [dbo].[tblQualifications] ([QualificationGuid])
);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblQualificationCompanyCertificateAndPermitToCompany_CreatedDate]
    ON [map].[tblQualificationCompanyCertificateAndPermitToCompany]([CreatedDate] ASC);


GO
CREATE NONCLUSTERED INDEX [IXU_map_tblQualificationCompanyCertificateAndPermitToCompany_CompanyGuid]
    ON [map].[tblQualificationCompanyCertificateAndPermitToCompany]([CompanyGuid] ASC, [ID] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_map_tblQualificationCompanyCertificateAndPermitToCompany_QualificationGuid]
    ON [map].[tblQualificationCompanyCertificateAndPermitToCompany]([QualificationGuid] ASC);


GO
--Creating Insert / Update Trigger for tblQualificationCompanyCertificateAndPermitToCompany
CREATE TRIGGER map.trg_insupd_tblQualificationCompanyCertificateAndPermitToCompany_ForSync 
   ON map.tblQualificationCompanyCertificateAndPermitToCompany
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
                    ,d.QualificationCompanyCertificateAndPermitToCompanyGuid AS Deleted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid
                    ,i.QualificationCompanyCertificateAndPermitToCompanyGuid AS Inserted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid
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
				    d.QualificationCompanyCertificateAndPermitToCompanyGuid = i.QualificationCompanyCertificateAndPermitToCompanyGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblQualificationCompanyCertificateAndPermitToCompany As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid = currentTrackingData.PK_QualificationCompanyCertificateAndPermitToCompanyGuid
 
 
		    INSERT track.tblQualificationCompanyCertificateAndPermitToCompany (InsertedDate 
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
				    ,PK_QualificationCompanyCertificateAndPermitToCompanyGuid
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
				    ,entityChanges.Inserted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblQualificationCompanyCertificateAndPermitToCompany As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid = currentTrackingData.PK_QualificationCompanyCertificateAndPermitToCompanyGuid
)
    END
END 

GO
--Creating Delete Trigger for tblQualificationCompanyCertificateAndPermitToCompany
CREATE TRIGGER map.trg_del_tblQualificationCompanyCertificateAndPermitToCompany_ForSync 
   ON map.tblQualificationCompanyCertificateAndPermitToCompany
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
						,d.QualificationCompanyCertificateAndPermitToCompanyGuid AS Deleted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid
                        ,d.QualificationCompanyCertificateAndPermitToCompanyGuid AS Inserted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblQualificationCompanyCertificateAndPermitToCompany As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid = currentTrackingData.PK_QualificationCompanyCertificateAndPermitToCompanyGuid
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
						,PK_QualificationCompanyCertificateAndPermitToCompanyGuid
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
						,entityChanges.Deleted_PK_QualificationCompanyCertificateAndPermitToCompanyGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [map].[trg_Audit_del_tblQualificationCompanyCertificateAndPermitToCompany] ON [map].[tblQualificationCompanyCertificateAndPermitToCompany] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblQualificationCompanyCertificateAndPermitToCompany','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'D'; -- For Deletes 
	SET @_AuditEventSequence= 1; 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID;

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].map_tblQualificationCompanyCertificateAndPermitToCompany (
		[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	[QualificationGuid]
	,	[CompanyGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		d.[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	d.[QualificationGuid]
	,	d.[CompanyGuid]
	,	d.[Sequence]
	,	d.[Instructor]
	,	d.[DateCompleted]
	,	d.[DateDue]
	,	d.[ExpirationDate]
	,	d.[ID]
	,	d.[Rating]
	,	d.[HistoricalRecord]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
END

GO
CREATE TRIGGER [map].[trg_Audit_ins_tblQualificationCompanyCertificateAndPermitToCompany] ON [map].[tblQualificationCompanyCertificateAndPermitToCompany] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblQualificationCompanyCertificateAndPermitToCompany','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'I' -- For Inserts 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
	INSERT INTO [fmaudit].map_tblQualificationCompanyCertificateAndPermitToCompany (
		[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	[QualificationGuid]
	,	[CompanyGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		i.[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	i.[QualificationGuid]
	,	i.[CompanyGuid]
	,	i.[Sequence]
	,	i.[Instructor]
	,	i.[DateCompleted]
	,	i.[DateDue]
	,	i.[ExpirationDate]
	,	i.[ID]
	,	i.[Rating]
	,	i.[HistoricalRecord]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM inserted i
END

GO
CREATE TRIGGER [map].[trg_Audit_upd_tblQualificationCompanyCertificateAndPermitToCompany] ON [map].[tblQualificationCompanyCertificateAndPermitToCompany] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('map','tblQualificationCompanyCertificateAndPermitToCompany','D')=1 
		RETURN
	DECLARE @_AuditEventType CHAR(1)
	,	@_AuditEventSequence TINYINT
	,	@_AuditSessionGUID UNIQUEIDENTIFIER
	,	@_AuditSessionTokenID UNIQUEIDENTIFIER
	,	@_AuditSiteGUID UNIQUEIDENTIFIER
	,	@_AuditGUID UNIQUEIDENTIFIER
	,	@_AuditDateTime DATETIMEOFFSET
	,	@_UserId NVARCHAR(100)
	,	@_AuditContext varbinary(128);
	SET @_AuditDateTime = SYSDATETIMEOFFSET();
	SET @_AuditEventType= 'U' -- For Updates 
	SET @_AuditEventSequence= 1 
	SELECT	@_AuditSessionGUID=s.SessionGuid 
		,	@_AuditSessionTokenID=s.SessionTokenID 
		,	@_AuditSiteGUID=s.SiteGuid
		,	@_UserId=u.UserId
		,	@_AuditContext=s.SynchronizationNodeGuid
	FROM map.tblSessionToSQLProcess m 
	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid 
	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid 
	WHERE m.SqlServerSessionID=@@SPID 

	-- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. 
	-- Treat the change as a local change so it can be synchronized back to the remote system. 
	IF ((SELECT trigger_nestlevel()) > 1) 
	BEGIN 
		SET @_AuditContext = NULL 
	END 

	-- If it has been determined that this trigger is being fired in response to the synchronization process propagating changes from one system to another, 
	-- do not audit the changes.  When tblAuditLog is synchronized, it will contain the original audit event(s) any and all changes to this record. 
	IF (@_AuditContext IS NOT NULL) 
	BEGIN 
		RETURN
	END

	IF @_UserId IS NULL
		SET @_UserId = SUSER_NAME()
 
	DECLARE @AuditGuidList TABLE
	(
	QualificationCompanyCertificateAndPermitToCompanyGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].map_tblQualificationCompanyCertificateAndPermitToCompany (
		[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	[QualificationGuid]
	,	[CompanyGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	OUTPUT inserted.[QualificationCompanyCertificateAndPermitToCompanyGuid] AS 'QualificationCompanyCertificateAndPermitToCompanyGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	d.[QualificationGuid]
	,	d.[CompanyGuid]
	,	d.[Sequence]
	,	d.[Instructor]
	,	d.[DateCompleted]
	,	d.[DateDue]
	,	d.[ExpirationDate]
	,	d.[ID]
	,	d.[Rating]
	,	d.[HistoricalRecord]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	@_AuditEventType
	,	@_AuditEventSequence
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	NEWID()
	,	@_UserId
	,	@_AuditContext
	FROM deleted d
 
	INSERT INTO [fmaudit].map_tblQualificationCompanyCertificateAndPermitToCompany (
		[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	[QualificationGuid]
	,	[CompanyGuid]
	,	[Sequence]
	,	[Instructor]
	,	[DateCompleted]
	,	[DateDue]
	,	[ExpirationDate]
	,	[ID]
	,	[Rating]
	,	[HistoricalRecord]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[_AuditEventType]
	,	[_AuditEventSequence]
	,	[_AuditSessionGUID]
	,	[_AuditSessionTokenID]
	,	[_AuditCreatedDate]
	,	[_AuditSiteGUID]
	,	[_AuditGUID]
	,	[_AuditUserId]
	,	[_AuditContext]
	)
	SELECT 
		i.[QualificationCompanyCertificateAndPermitToCompanyGuid]
	,	i.[QualificationGuid]
	,	i.[CompanyGuid]
	,	i.[Sequence]
	,	i.[Instructor]
	,	i.[DateCompleted]
	,	i.[DateDue]
	,	i.[ExpirationDate]
	,	i.[ID]
	,	i.[Rating]
	,	i.[HistoricalRecord]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	@_AuditEventType
	,	2
	,	@_AuditSessionGUID
	,	@_AuditSessionTokenID
	,	@_AuditDateTime
	,	@_AuditSiteGUID
	,	agl._AuditGUID
	,	@_UserId
	,	@_AuditContext
	FROM inserted i 
	INNER JOIN	@AuditGuidList agl ON
		(
			agl.[QualificationCompanyCertificateAndPermitToCompanyGuid]=i.[QualificationCompanyCertificateAndPermitToCompanyGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblQualificationCompanyCertificateAndPermitToCompany_ClusterIdx]
    ON [map].[tblQualificationCompanyCertificateAndPermitToCompany]([_ClusterIdx] ASC);

