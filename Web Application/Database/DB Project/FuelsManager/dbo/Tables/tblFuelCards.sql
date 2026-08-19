CREATE TABLE [dbo].[tblFuelCards] (
    [ID]                                NVARCHAR (50)      CONSTRAINT [DF_tblFuelCards_ID] DEFAULT ('') NOT NULL,
    [Provider]                          NVARCHAR (50)      NULL,
    [ActivationStatus]                  INT                CONSTRAINT [DF_tblFuelCard_ActivationStatus] DEFAULT ((0)) NOT NULL,
    [InactivityPeriod]                  INT                CONSTRAINT [DF_tblFuelCard_InactivityInterval] DEFAULT ((4)) NULL,
    [Notes]                             NVARCHAR (MAX)     NULL,
    [StatusModifiedDate]                DATETIMEOFFSET (7) CONSTRAINT [DF_tblFuelCard_ActivatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [StatusModifiedBy]                  NVARCHAR (50)      CONSTRAINT [DF_tblFuelCards_StatusModifiedBy] DEFAULT ('') NOT NULL,
    [UserData1]                         NVARCHAR (60)      NULL,
    [UserData2]                         NVARCHAR (60)      NULL,
    [UserData3]                         NVARCHAR (60)      NULL,
    [UserData4]                         NVARCHAR (60)      NULL,
    [UserData5]                         NVARCHAR (60)      NULL,
    [UserData6]                         NVARCHAR (60)      NULL,
    [UserData7]                         NVARCHAR (60)      NULL,
    [UserData8]                         NVARCHAR (60)      NULL,
    [CreatedDate]                       DATETIMEOFFSET (7) CONSTRAINT [DF_tblFuelCard_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                         [dbo].[udtUserID]  CONSTRAINT [DF_tblFuelCards_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                       DATETIMEOFFSET (7) CONSTRAINT [DF_tblFuelCard_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                         [dbo].[udtUserID]  CONSTRAINT [DF_tblFuelCards_UpdatedBy] DEFAULT ('') NOT NULL,
    [FuelCardGuid]                      UNIQUEIDENTIFIER   CONSTRAINT [DF_tblFuelCards_Guid] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                       ROWVERSION         NOT NULL,
    [SiteGuid]                          UNIQUEIDENTIFIER   NOT NULL,
    [BillToCompanyGuid]                 UNIQUEIDENTIFIER   NULL,
    [ManagerCompanyGuid]                UNIQUEIDENTIFIER   NULL,
    [OwnerCompanyGuid]                  UNIQUEIDENTIFIER   NULL,
    [ShipperCompanyGuid]                UNIQUEIDENTIFIER   NULL,
    [ShipToCompanyGuid]                 UNIQUEIDENTIFIER   NULL,
    [ExpirationDate]                    DATETIMEOFFSET (7) NULL,
    [TransientCardFlag]                 BIT                CONSTRAINT [DF_tblFuelCards_TransientCardFlag] DEFAULT ((0)) NULL,
    [PIN]                               VARBINARY (256)    NULL,
    [ProviderID]                        NVARCHAR (60)      NULL,
    [FuelCardTypeApplicationStringGuid] UNIQUEIDENTIFIER   NULL,
    [HiddenDate]                        DATETIMEOFFSET (7) NULL,
    [_ClusterIdx]                       BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblFuelCards_Guid] PRIMARY KEY NONCLUSTERED ([FuelCardGuid] ASC),
    CONSTRAINT [CK_tblFuelCards_Uniqueness] CHECK ([dbo].[udf_CheckUniquenessFuelCard]([FuelCardGuid],[SiteGuid],[ID])=(1)),
    CONSTRAINT [FK_tblFuelCards_ActivationStatus] FOREIGN KEY ([ActivationStatus]) REFERENCES [lookup].[tblActivationStatus] ([ActivationStatusIndex]),
    CONSTRAINT [FK_tblFuelCards_BillToCompanyGuid] FOREIGN KEY ([BillToCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblFuelCards_FuelCardTypeApplicationStringGuid] FOREIGN KEY ([FuelCardTypeApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblFuelCards_ManagerCompanyGuid] FOREIGN KEY ([ManagerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblFuelCards_OwnerCompanyGuid] FOREIGN KEY ([OwnerCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblFuelCards_ShipperCompanyGuid] FOREIGN KEY ([ShipperCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblFuelCards_ShipToCompanyGuid] FOREIGN KEY ([ShipToCompanyGuid]) REFERENCES [dbo].[tblCompanies] ([CompanyGuid]),
    CONSTRAINT [FK_tblFuelCards_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblFuelCards_CreatedDate]
    ON [dbo].[tblFuelCards]([CreatedDate] ASC);




GO
CREATE NONCLUSTERED INDEX [IX_tblFuelCards_ID]
    ON [dbo].[tblFuelCards]([ID] ASC);

GO


CREATE INDEX [IX_tblFuelCards_FuelCardGuid_SiteGuid] ON [dbo].[tblFuelCards] 
([FuelCardGuid],[SiteGuid])
GO
CREATE TRIGGER [dbo].[trg_Audit_upd_tblFuelCards] ON [dbo].[tblFuelCards] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblFuelCards','D')=1 
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
	FuelCardGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblFuelCards (
		[ID]
	,	[Provider]
	,	[ActivationStatus]
	,	[InactivityPeriod]
	,	[Notes]
	,	[StatusModifiedDate]
	,	[StatusModifiedBy]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[FuelCardGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[BillToCompanyGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[ExpirationDate]
	,	[TransientCardFlag]
	,	[PIN]
	,	[ProviderID]
	,	[FuelCardTypeApplicationStringGuid]
	,	[HiddenDate]
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
	OUTPUT inserted.[FuelCardGuid] AS 'FuelCardGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[ID]
	,	d.[Provider]
	,	d.[ActivationStatus]
	,	d.[InactivityPeriod]
	,	d.[Notes]
	,	d.[StatusModifiedDate]
	,	d.[StatusModifiedBy]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[FuelCardGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[BillToCompanyGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[OwnerCompanyGuid]
	,	d.[ShipperCompanyGuid]
	,	d.[ShipToCompanyGuid]
	,	d.[ExpirationDate]
	,	d.[TransientCardFlag]
	,	d.[PIN]
	,	d.[ProviderID]
	,	d.[FuelCardTypeApplicationStringGuid]
	,	d.[HiddenDate]
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
 
	INSERT INTO [fmaudit].tblFuelCards (
		[ID]
	,	[Provider]
	,	[ActivationStatus]
	,	[InactivityPeriod]
	,	[Notes]
	,	[StatusModifiedDate]
	,	[StatusModifiedBy]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[FuelCardGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[BillToCompanyGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[ExpirationDate]
	,	[TransientCardFlag]
	,	[PIN]
	,	[ProviderID]
	,	[FuelCardTypeApplicationStringGuid]
	,	[HiddenDate]
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
		i.[ID]
	,	i.[Provider]
	,	i.[ActivationStatus]
	,	i.[InactivityPeriod]
	,	i.[Notes]
	,	i.[StatusModifiedDate]
	,	i.[StatusModifiedBy]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[FuelCardGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[BillToCompanyGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[OwnerCompanyGuid]
	,	i.[ShipperCompanyGuid]
	,	i.[ShipToCompanyGuid]
	,	i.[ExpirationDate]
	,	i.[TransientCardFlag]
	,	i.[PIN]
	,	i.[ProviderID]
	,	i.[FuelCardTypeApplicationStringGuid]
	,	i.[HiddenDate]
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
			agl.[FuelCardGuid]=i.[FuelCardGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblFuelCards
CREATE TRIGGER dbo.trg_insupd_tblFuelCards_ForSync 
   ON dbo.tblFuelCards
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
                    ,d.FuelCardGuid AS Deleted_PK_FuelCardGuid
                    ,i.FuelCardGuid AS Inserted_PK_FuelCardGuid
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
				    d.FuelCardGuid = i.FuelCardGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblFuelCards As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_FuelCardGuid = currentTrackingData.PK_FuelCardGuid
 
 
		    INSERT track.tblFuelCards (InsertedDate 
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
				    ,PK_FuelCardGuid
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
				    ,entityChanges.Inserted_PK_FuelCardGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblFuelCards As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_FuelCardGuid = currentTrackingData.PK_FuelCardGuid
)
    END
END 

GO
--Creating Delete Trigger for tblFuelCards
CREATE TRIGGER dbo.trg_del_tblFuelCards_ForSync 
   ON dbo.tblFuelCards
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
						,d.FuelCardGuid AS Deleted_PK_FuelCardGuid
                        ,d.FuelCardGuid AS Inserted_PK_FuelCardGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblFuelCards As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_FuelCardGuid = currentTrackingData.PK_FuelCardGuid
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
						,PK_FuelCardGuid
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
						,entityChanges.Deleted_PK_FuelCardGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblFuelCards] ON [dbo].[tblFuelCards] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblFuelCards','D')=1 
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
	INSERT INTO [fmaudit].tblFuelCards (
		[ID]
	,	[Provider]
	,	[ActivationStatus]
	,	[InactivityPeriod]
	,	[Notes]
	,	[StatusModifiedDate]
	,	[StatusModifiedBy]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[FuelCardGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[BillToCompanyGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[ExpirationDate]
	,	[TransientCardFlag]
	,	[PIN]
	,	[ProviderID]
	,	[FuelCardTypeApplicationStringGuid]
	,	[HiddenDate]
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
		d.[ID]
	,	d.[Provider]
	,	d.[ActivationStatus]
	,	d.[InactivityPeriod]
	,	d.[Notes]
	,	d.[StatusModifiedDate]
	,	d.[StatusModifiedBy]
	,	d.[UserData1]
	,	d.[UserData2]
	,	d.[UserData3]
	,	d.[UserData4]
	,	d.[UserData5]
	,	d.[UserData6]
	,	d.[UserData7]
	,	d.[UserData8]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[FuelCardGuid]
	,	d.[_RowVersion]
	,	d.[SiteGuid]
	,	d.[BillToCompanyGuid]
	,	d.[ManagerCompanyGuid]
	,	d.[OwnerCompanyGuid]
	,	d.[ShipperCompanyGuid]
	,	d.[ShipToCompanyGuid]
	,	d.[ExpirationDate]
	,	d.[TransientCardFlag]
	,	d.[PIN]
	,	d.[ProviderID]
	,	d.[FuelCardTypeApplicationStringGuid]
	,	d.[HiddenDate]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblFuelCards] ON [dbo].[tblFuelCards] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblFuelCards','D')=1 
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
	INSERT INTO [fmaudit].tblFuelCards (
		[ID]
	,	[Provider]
	,	[ActivationStatus]
	,	[InactivityPeriod]
	,	[Notes]
	,	[StatusModifiedDate]
	,	[StatusModifiedBy]
	,	[UserData1]
	,	[UserData2]
	,	[UserData3]
	,	[UserData4]
	,	[UserData5]
	,	[UserData6]
	,	[UserData7]
	,	[UserData8]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[FuelCardGuid]
	,	[OriginalRowVersion]
	,	[SiteGuid]
	,	[BillToCompanyGuid]
	,	[ManagerCompanyGuid]
	,	[OwnerCompanyGuid]
	,	[ShipperCompanyGuid]
	,	[ShipToCompanyGuid]
	,	[ExpirationDate]
	,	[TransientCardFlag]
	,	[PIN]
	,	[ProviderID]
	,	[FuelCardTypeApplicationStringGuid]
	,	[HiddenDate]
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
		i.[ID]
	,	i.[Provider]
	,	i.[ActivationStatus]
	,	i.[InactivityPeriod]
	,	i.[Notes]
	,	i.[StatusModifiedDate]
	,	i.[StatusModifiedBy]
	,	i.[UserData1]
	,	i.[UserData2]
	,	i.[UserData3]
	,	i.[UserData4]
	,	i.[UserData5]
	,	i.[UserData6]
	,	i.[UserData7]
	,	i.[UserData8]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[FuelCardGuid]
	,	i.[_RowVersion]
	,	i.[SiteGuid]
	,	i.[BillToCompanyGuid]
	,	i.[ManagerCompanyGuid]
	,	i.[OwnerCompanyGuid]
	,	i.[ShipperCompanyGuid]
	,	i.[ShipToCompanyGuid]
	,	i.[ExpirationDate]
	,	i.[TransientCardFlag]
	,	i.[PIN]
	,	i.[ProviderID]
	,	i.[FuelCardTypeApplicationStringGuid]
	,	i.[HiddenDate]
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


CREATE UNIQUE CLUSTERED INDEX [IX_tblFuelCards_ClusterIdx]
    ON [dbo].[tblFuelCards]([_ClusterIdx] ASC);

GO

CREATE NONCLUSTERED INDEX [IX_tblFuelCards_ActivationStatus_HiddenDate]
    ON [dbo].[tblFuelCards]([ActivationStatus] ASC, [HiddenDate] ASC)
    INCLUDE([ID], [FuelCardGuid], [ExpirationDate]) WITH (FILLFACTOR = 100);