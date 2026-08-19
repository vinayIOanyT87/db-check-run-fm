CREATE TABLE [dbo].[tblAllocations] (
    [EffectiveDate]                        DATETIMEOFFSET (7) CONSTRAINT [DF_tblAllocations_EffectiveDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [ExpirationDate]                       DATETIMEOFFSET (7) CONSTRAINT [DF_tblAllocations_ExpirationDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [LoadWarning]                          FLOAT (53)         CONSTRAINT [DF_tblAllocations_LoadWarning] DEFAULT ((0.0)) NOT NULL,
    [LoadDenial]                           FLOAT (53)         CONSTRAINT [DF_tblAllocations_LoadDenial] DEFAULT ((0.0)) NOT NULL,
    [ContractNumber]                       NVARCHAR (10)      CONSTRAINT [DF_tblAllocations_ContractNumber] DEFAULT ('') NOT NULL,
    [AllocationGroupIndex]                 INT                NULL,
    [LastAllocationResetDate]              DATETIMEOFFSET (7) CONSTRAINT [DF_tblAllocations_LastAllocationResetDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedDate]                          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAllocations_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                            [dbo].[udtUserID]  CONSTRAINT [DF_tblAllocations_CreatedBy] DEFAULT ('') NOT NULL,
    [UpdatedDate]                          DATETIMEOFFSET (7) CONSTRAINT [DF_tblAllocations_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                            [dbo].[udtUserID]  CONSTRAINT [DF_tblAllocations_UpdatedBy] DEFAULT ('') NOT NULL,
    [AllocationGuid]                       UNIQUEIDENTIFIER   CONSTRAINT [DF_tblAllocations_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                          ROWVERSION         NOT NULL,
    [CompanyBillToToShipperGuid]           UNIQUEIDENTIFIER   NULL,
    [CompanyLoadOwnerToManagerGuid]        UNIQUEIDENTIFIER   NULL,
    [CompanyOffLoadOwnerToManagerGuid]     UNIQUEIDENTIFIER   NULL,
    [CompanyShipperToOwnerGuid]            UNIQUEIDENTIFIER   NULL,
    [CompanyShipToToBillToGuid]            UNIQUEIDENTIFIER   NULL,
    [CompanySupplierToOwnerGuid]           UNIQUEIDENTIFIER   NULL,
    [SiteGuid]                             UNIQUEIDENTIFIER   NOT NULL,
    [LookupCompanyMapTypeIndex]            INT                CONSTRAINT [DF_tblAllocations_LookupCompanyMapTypeIndex] DEFAULT ((0)) NOT NULL,
    [AllocationGroupApplicationStringGuid] UNIQUEIDENTIFIER   NULL,
    [_ClusterIdx]                          BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblAllocations_GUID] PRIMARY KEY NONCLUSTERED ([AllocationGuid] ASC),
    CONSTRAINT [FK_tblAllocations_AllocationGroupApplicationStringGuid] FOREIGN KEY ([AllocationGroupApplicationStringGuid]) REFERENCES [dbo].[tblApplicationString] ([ApplicationStringGuid]),
    CONSTRAINT [FK_tblAllocations_BillToShipperCompanyGuid] FOREIGN KEY ([CompanyBillToToShipperGuid]) REFERENCES [map].[tblCompanyBillToToShipper] ([CompanyBillToToShipperGuid]),
    CONSTRAINT [FK_tblAllocations_LoadOwnerToManagerCompanyGuid] FOREIGN KEY ([CompanyLoadOwnerToManagerGuid]) REFERENCES [map].[tblCompanyLoadOwnerToManager] ([CompanyLoadOwnerToManagerGuid]),
    CONSTRAINT [FK_tblAllocations_LookupCompanyMapTypeIndex] FOREIGN KEY ([LookupCompanyMapTypeIndex]) REFERENCES [lookup].[tblCompanyMapType] ([CompanyMapTypeIndex]),
    CONSTRAINT [FK_tblAllocations_OffLoadOwnerToManagerCompanyGuid] FOREIGN KEY ([CompanyOffLoadOwnerToManagerGuid]) REFERENCES [map].[tblCompanyOffLoadOwnerToManager] ([CompanyOffLoadOwnerToManagerGuid]),
    CONSTRAINT [FK_tblAllocations_ShipperToOwnerCompanyGuid] FOREIGN KEY ([CompanyShipperToOwnerGuid]) REFERENCES [map].[tblCompanyShipperToOwner] ([CompanyShipperToOwnerGuid]),
    CONSTRAINT [FK_tblAllocations_ShipToBillToCompanyGuid] FOREIGN KEY ([CompanyShipToToBillToGuid]) REFERENCES [map].[tblCompanyShipToToBillTo] ([CompanyShipToToBillToGuid]),
    CONSTRAINT [FK_tblAllocations_SiteGuid] FOREIGN KEY ([SiteGuid]) REFERENCES [dbo].[tblSites] ([SiteGuid]),
    CONSTRAINT [FK_tblAllocations_SupplierToOwnerCompanyGuid] FOREIGN KEY ([CompanySupplierToOwnerGuid]) REFERENCES [map].[tblCompanySupplierToOwner] ([CompanySupplierToOwnerGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblAllocations_CreatedDate]
    ON [dbo].[tblAllocations]([CreatedDate] ASC);




GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblAllocations] ON [dbo].[tblAllocations] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAllocations','D')=1 
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
	INSERT INTO [fmaudit].tblAllocations (
		[EffectiveDate]
	,	[ExpirationDate]
	,	[LoadWarning]
	,	[LoadDenial]
	,	[ContractNumber]
	,	[AllocationGroupIndex]
	,	[LastAllocationResetDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[AllocationGuid]
	,	[OriginalRowVersion]
	,	[CompanyBillToToShipperGuid]
	,	[CompanyLoadOwnerToManagerGuid]
	,	[CompanyOffLoadOwnerToManagerGuid]
	,	[CompanyShipperToOwnerGuid]
	,	[CompanyShipToToBillToGuid]
	,	[CompanySupplierToOwnerGuid]
	,	[SiteGuid]
	,	[LookupCompanyMapTypeIndex]
	,	[AllocationGroupApplicationStringGuid]
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
		d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[LoadWarning]
	,	d.[LoadDenial]
	,	d.[ContractNumber]
	,	d.[AllocationGroupIndex]
	,	d.[LastAllocationResetDate]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[AllocationGuid]
	,	d.[_RowVersion]
	,	d.[CompanyBillToToShipperGuid]
	,	d.[CompanyLoadOwnerToManagerGuid]
	,	d.[CompanyOffLoadOwnerToManagerGuid]
	,	d.[CompanyShipperToOwnerGuid]
	,	d.[CompanyShipToToBillToGuid]
	,	d.[CompanySupplierToOwnerGuid]
	,	d.[SiteGuid]
	,	d.[LookupCompanyMapTypeIndex]
	,	d.[AllocationGroupApplicationStringGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblAllocations] ON [dbo].[tblAllocations] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAllocations','D')=1 
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
	INSERT INTO [fmaudit].tblAllocations (
		[EffectiveDate]
	,	[ExpirationDate]
	,	[LoadWarning]
	,	[LoadDenial]
	,	[ContractNumber]
	,	[AllocationGroupIndex]
	,	[LastAllocationResetDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[AllocationGuid]
	,	[OriginalRowVersion]
	,	[CompanyBillToToShipperGuid]
	,	[CompanyLoadOwnerToManagerGuid]
	,	[CompanyOffLoadOwnerToManagerGuid]
	,	[CompanyShipperToOwnerGuid]
	,	[CompanyShipToToBillToGuid]
	,	[CompanySupplierToOwnerGuid]
	,	[SiteGuid]
	,	[LookupCompanyMapTypeIndex]
	,	[AllocationGroupApplicationStringGuid]
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
		i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[LoadWarning]
	,	i.[LoadDenial]
	,	i.[ContractNumber]
	,	i.[AllocationGroupIndex]
	,	i.[LastAllocationResetDate]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[AllocationGuid]
	,	i.[_RowVersion]
	,	i.[CompanyBillToToShipperGuid]
	,	i.[CompanyLoadOwnerToManagerGuid]
	,	i.[CompanyOffLoadOwnerToManagerGuid]
	,	i.[CompanyShipperToOwnerGuid]
	,	i.[CompanyShipToToBillToGuid]
	,	i.[CompanySupplierToOwnerGuid]
	,	i.[SiteGuid]
	,	i.[LookupCompanyMapTypeIndex]
	,	i.[AllocationGroupApplicationStringGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblAllocations] ON [dbo].[tblAllocations] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblAllocations','D')=1 
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
	AllocationGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblAllocations (
		[EffectiveDate]
	,	[ExpirationDate]
	,	[LoadWarning]
	,	[LoadDenial]
	,	[ContractNumber]
	,	[AllocationGroupIndex]
	,	[LastAllocationResetDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[AllocationGuid]
	,	[OriginalRowVersion]
	,	[CompanyBillToToShipperGuid]
	,	[CompanyLoadOwnerToManagerGuid]
	,	[CompanyOffLoadOwnerToManagerGuid]
	,	[CompanyShipperToOwnerGuid]
	,	[CompanyShipToToBillToGuid]
	,	[CompanySupplierToOwnerGuid]
	,	[SiteGuid]
	,	[LookupCompanyMapTypeIndex]
	,	[AllocationGroupApplicationStringGuid]
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
	OUTPUT inserted.[AllocationGuid] AS 'AllocationGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[EffectiveDate]
	,	d.[ExpirationDate]
	,	d.[LoadWarning]
	,	d.[LoadDenial]
	,	d.[ContractNumber]
	,	d.[AllocationGroupIndex]
	,	d.[LastAllocationResetDate]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[AllocationGuid]
	,	d.[_RowVersion]
	,	d.[CompanyBillToToShipperGuid]
	,	d.[CompanyLoadOwnerToManagerGuid]
	,	d.[CompanyOffLoadOwnerToManagerGuid]
	,	d.[CompanyShipperToOwnerGuid]
	,	d.[CompanyShipToToBillToGuid]
	,	d.[CompanySupplierToOwnerGuid]
	,	d.[SiteGuid]
	,	d.[LookupCompanyMapTypeIndex]
	,	d.[AllocationGroupApplicationStringGuid]
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
 
	INSERT INTO [fmaudit].tblAllocations (
		[EffectiveDate]
	,	[ExpirationDate]
	,	[LoadWarning]
	,	[LoadDenial]
	,	[ContractNumber]
	,	[AllocationGroupIndex]
	,	[LastAllocationResetDate]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[AllocationGuid]
	,	[OriginalRowVersion]
	,	[CompanyBillToToShipperGuid]
	,	[CompanyLoadOwnerToManagerGuid]
	,	[CompanyOffLoadOwnerToManagerGuid]
	,	[CompanyShipperToOwnerGuid]
	,	[CompanyShipToToBillToGuid]
	,	[CompanySupplierToOwnerGuid]
	,	[SiteGuid]
	,	[LookupCompanyMapTypeIndex]
	,	[AllocationGroupApplicationStringGuid]
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
		i.[EffectiveDate]
	,	i.[ExpirationDate]
	,	i.[LoadWarning]
	,	i.[LoadDenial]
	,	i.[ContractNumber]
	,	i.[AllocationGroupIndex]
	,	i.[LastAllocationResetDate]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[AllocationGuid]
	,	i.[_RowVersion]
	,	i.[CompanyBillToToShipperGuid]
	,	i.[CompanyLoadOwnerToManagerGuid]
	,	i.[CompanyOffLoadOwnerToManagerGuid]
	,	i.[CompanyShipperToOwnerGuid]
	,	i.[CompanyShipToToBillToGuid]
	,	i.[CompanySupplierToOwnerGuid]
	,	i.[SiteGuid]
	,	i.[LookupCompanyMapTypeIndex]
	,	i.[AllocationGroupApplicationStringGuid]
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
			agl.[AllocationGuid]=i.[AllocationGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
--Creating Insert / Update Trigger for tblAllocations
CREATE TRIGGER dbo.trg_insupd_tblAllocations_ForSync 
   ON dbo.tblAllocations
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
                    ,d.AllocationGuid AS Deleted_PK_AllocationGuid
                    ,i.AllocationGuid AS Inserted_PK_AllocationGuid
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
				    d.AllocationGuid = i.AllocationGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblAllocations As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_AllocationGuid = currentTrackingData.PK_AllocationGuid
 
 
		    INSERT track.tblAllocations (InsertedDate 
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
				    ,PK_AllocationGuid
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
				    ,entityChanges.Inserted_PK_AllocationGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblAllocations As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_AllocationGuid = currentTrackingData.PK_AllocationGuid
)
    END
END 

GO
--Creating Delete Trigger for tblAllocations
CREATE TRIGGER dbo.trg_del_tblAllocations_ForSync 
   ON dbo.tblAllocations
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
						,d.AllocationGuid AS Deleted_PK_AllocationGuid
                        ,d.AllocationGuid AS Inserted_PK_AllocationGuid
                        ,NULL AS Deleted_FK_ParentPK 
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,d.SiteGuid AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblAllocations As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_AllocationGuid = currentTrackingData.PK_AllocationGuid
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
						,PK_AllocationGuid
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
						,entityChanges.Deleted_PK_AllocationGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblAllocations_ClusterIdx]
    ON [dbo].[tblAllocations]([_ClusterIdx] ASC);

