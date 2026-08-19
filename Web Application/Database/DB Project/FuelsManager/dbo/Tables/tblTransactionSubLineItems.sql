CREATE TABLE [dbo].[tblTransactionSubLineItems] (
    [SequenceID]                   INT                CONSTRAINT [DF_tblTransactionSubLineItems_SequenceID] DEFAULT ((0)) NOT NULL,
    [Product]                      NVARCHAR (30)      NULL,
    [ProductCode]                  NVARCHAR (50)      NULL,
    [ProductType]                  NVARCHAR (20)      NULL,
    [GrossQuantity]                FLOAT (53)         NULL,
	 [DeliveredGrossQuantity]       FLOAT (53)         NULL,
    [NetQuantity]                  FLOAT (53)         NULL,
	 [DeliveredNetQuantity]         FLOAT (53)         NULL,
	 [Pressure]          FLOAT (53)         NULL,
    [Vcf]                          FLOAT (53)         NULL,
    [Density]                      FLOAT (53)         NULL,
    [Temperature]                  FLOAT (53)         NULL,
    [Customs]                      NVARCHAR (20)      NULL,
    [ArmNumber]                    INT                NULL,
    [LineNumber]                   INT                NULL,
    [BatchNumber]                  NVARCHAR (20)      NULL,
    [LineFill]                     FLOAT (53)         NULL,
    [BottomVolume]                 FLOAT (53)         NULL,
    [NetCapacity]                  FLOAT (53)         NULL,
    [TankStatus]                   NVARCHAR (30)      NULL,
    [MeterFactor]                  FLOAT (53)         NULL,
    [MeterStart]                   FLOAT (53)         NULL,
    [MeterStop]                    FLOAT (53)         NULL,
    [MeterStopDateTime]            DATETIMEOFFSET (7) NULL,
    [MeterStartDateTime]           DATETIMEOFFSET (7) NULL,
    [FreezePoint]                  FLOAT (53)         NULL,
    [DifferentialPressure]         FLOAT (53)         NULL,
    [DosageRate]                   FLOAT (53)         NULL,
    [DeleteFlag]                   BIT                NULL,
    [PresetAmount]                 FLOAT (53)         NULL,
    [StorageLocationID]            NVARCHAR (50)      NULL,
    [MeterID]                      NVARCHAR (50)      NULL,
    [COAID]                        NVARCHAR (40)      NULL,
    [CreatedBy]                    [dbo].[udtUserID]  NULL,
    [CreatedDate]                  DATETIMEOFFSET (7) NULL,
    [UpdatedBy]                    [dbo].[udtUserID]  NULL,
    [UpdatedDate]                  DATETIMEOFFSET (7) NULL,
    [TransactionInventoryDate]     DATE               NULL,
    [Tax1]                         FLOAT (53)         NULL,
    [Tax2]                         FLOAT (53)         NULL,
    [Tax3]                         FLOAT (53)         NULL,
    [Tax4]                         FLOAT (53)         NULL,
    [Tax5]                         FLOAT (53)         NULL,
    [TransVersion]                 BIGINT             NULL,
    [ImproperAdditization]         BIT                NULL,
    [BrokenBlend]                  BIT                NULL,
    [Flag01]                       BIT                NULL,
    [Flag02]                       BIT                NULL,
    [Flag03]                       BIT                NULL,
    [Flag04]                       BIT                NULL,
    [Flag05]                       BIT                NULL,
    [Flag06]                       BIT                NULL,
    [Number01]                     FLOAT (53)         NULL,
    [Number02]                     FLOAT (53)         NULL,
    [Number03]                     FLOAT (53)         NULL,
    [Number04]                     FLOAT (53)         NULL,
    [Number05]                     FLOAT (53)         NULL,
    [Number06]                     FLOAT (53)         NULL,
    [Date01]                       DATETIMEOFFSET (7) NULL,
    [Date02]                       DATETIMEOFFSET (7) NULL,
    [Date03]                       DATETIMEOFFSET (7) NULL,
    [Date04]                       DATETIMEOFFSET (7) NULL,
    [MassQuantity]                 FLOAT (53)         NULL,
    [NetManualValueFlag]           BIT                NULL,
    [MassManualValueFlag]          BIT                NULL,
    [GrossManualValueFlag]         BIT                NULL,
    [VcfManualValueFlag]           BIT                NULL,
	 [DeliveredGrossManualValueFlag] BIT                NULL,
	 [DeliveredNetManualValueFlag]   BIT                NULL,
    [TransactionSubLineItemGuid]   UNIQUEIDENTIFIER   CONSTRAINT [DF_tblTransactionSubLineItems_GUID] DEFAULT (newid()) NOT NULL,
    [_RowVersion]                  ROWVERSION         NOT NULL,
    [LookupTransactionStatusIndex] INT                NULL,
    [LookupQualityIndex]           INT                CONSTRAINT [DF_tblTransactionSubLineItems_LookupQualityIndex] DEFAULT ((1)) NOT NULL,
    [TransactionLineItemGuid]      UNIQUEIDENTIFIER   NOT NULL,
    [ProductGuid]                  UNIQUEIDENTIFIER   NULL,
    [TransactionGuid]              UNIQUEIDENTIFIER   NOT NULL,
    [StorageLocationTankGuid]      UNIQUEIDENTIFIER   NULL,
    [MeterGuid]                    UNIQUEIDENTIFIER   NULL,
    [PackageManualValueFlag]       BIT                NULL,
    [CleanLineItem]                BIT                NULL,
    [CleanLineDeductItem]          BIT                NULL,
    [CleanLineDeductQuantity]      FLOAT (53)         NULL,
    [CleanLinePackQuantity]        FLOAT (53)         NULL,	 
    [_ClusterIdx]                  BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblTransactionSubLineItems_GUID] PRIMARY KEY NONCLUSTERED ([TransactionSubLineItemGuid] ASC),
    CONSTRAINT [FK_tblTransactionSubLineItems_LookupTransactionQualityIndex] FOREIGN KEY ([LookupQualityIndex]) REFERENCES [lookup].[tblTransactionQuality] ([TransactionQualityIndex]),
    CONSTRAINT [FK_tblTransactionSubLineItems_LookupTransactionStatusIndex] FOREIGN KEY ([LookupTransactionStatusIndex]) REFERENCES [lookup].[tblTransactionStatus] ([TransactionStatusIndex]),
    CONSTRAINT [FK_tblTransactionSubLineItems_TransactionGuid] FOREIGN KEY ([TransactionGuid]) REFERENCES [dbo].[tblTransactions] ([TransactionGuid]),
    CONSTRAINT [FK_TransactionSubLineItems_TransactionLineItemGuid] FOREIGN KEY ([TransactionLineItemGuid]) REFERENCES [dbo].[tblTransactionLineItems] ([TransactionLineItemGuid])
);


GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblTransactionSubLineItems_ClusterIdx] 
	ON [dbo].[tblTransactionSubLineItems]([_ClusterIdx]);
GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_TransactionInventoryDate]
    ON [dbo].[tblTransactionSubLineItems]([TransactionInventoryDate] ASC);


GO

CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_LedgerCoveringIndex] ON [dbo].[tblTransactionSubLineItems]
(
	[TransactionGuid] ASC
)
INCLUDE ( 	[GrossQuantity],
	[LookupQualityIndex],
	[MassQuantity],
	[NetQuantity],
	[ProductGuid],
	[StorageLocationTankGuid],
	[Product],
	[DeleteFlag],
	[SequenceID],
	[ProductCode]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 100)
GO


CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_MeterGuid]
    ON [dbo].[tblTransactionSubLineItems]([MeterGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_ProductGuid]
    ON [dbo].[tblTransactionSubLineItems]([ProductGuid] ASC);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_TransactionLineItemGuid_SequenceID]
    ON [dbo].[tblTransactionSubLineItems]([TransactionLineItemGuid] ASC, [SequenceID] ASC)
    INCLUDE([TransactionSubLineItemGuid]);


GO
CREATE NONCLUSTERED INDEX [IX_tblTransactionSubLineItems_TransactionGuid_TransVersion]
    ON [dbo].[tblTransactionSubLineItems]([TransactionGuid] ASC, [TransVersion] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionSubLineItems','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionSubLineItems (
		[SequenceID]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[Vcf]
	,	[Density]
	,	[Temperature]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[BatchNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[TankStatus]
	,	[MeterFactor]
	,	[MeterStart]
	,	[MeterStop]
	,	[MeterStopDateTime]
	,	[MeterStartDateTime]
	,	[FreezePoint]
	,	[DifferentialPressure]
	,	[DosageRate]
	,	[DeleteFlag]
	,	[PresetAmount]
	,	[StorageLocationID]
	,	[MeterID]
	,	[COAID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionInventoryDate]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,  [DeliveredGrossManualValueFlag]
	,  [DeliveredNetManualValueFlag]
	,	[TransactionSubLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[TransactionLineItemGuid]
	,	[ProductGuid]
	,	[TransactionGuid]
	,	[StorageLocationTankGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
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
		d.[SequenceID]
	,	d.[Product]
	,	d.[ProductCode]
	,	d.[ProductType]
	,	d.[GrossQuantity]
	,  d.[DeliveredGrossQuantity]
	,	d.[NetQuantity]
	,  d.[DeliveredNetQuantity]
	,  d.[Pressure]	
	,	d.[Vcf]
	,	d.[Density]
	,	d.[Temperature]
	,	d.[Customs]
	,	d.[ArmNumber]
	,	d.[LineNumber]
	,	d.[BatchNumber]
	,	d.[LineFill]
	,	d.[BottomVolume]
	,	d.[NetCapacity]
	,	d.[TankStatus]
	,	d.[MeterFactor]
	,	d.[MeterStart]
	,	d.[MeterStop]
	,	d.[MeterStopDateTime]
	,	d.[MeterStartDateTime]
	,	d.[FreezePoint]
	,	d.[DifferentialPressure]
	,	d.[DosageRate]
	,	d.[DeleteFlag]
	,	d.[PresetAmount]
	,	d.[StorageLocationID]
	,	d.[MeterID]
	,	d.[COAID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionInventoryDate]
	,	d.[Tax1]
	,	d.[Tax2]
	,	d.[Tax3]
	,	d.[Tax4]
	,	d.[Tax5]
	,	d.[TransVersion]
	,	d.[ImproperAdditization]
	,	d.[BrokenBlend]
	,	d.[Flag01]
	,	d.[Flag02]
	,	d.[Flag03]
	,	d.[Flag04]
	,	d.[Flag05]
	,	d.[Flag06]
	,	d.[Number01]
	,	d.[Number02]
	,	d.[Number03]
	,	d.[Number04]
	,	d.[Number05]
	,	d.[Number06]
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[MassQuantity]
	,	d.[NetManualValueFlag]
	,	d.[MassManualValueFlag]
	,	d.[GrossManualValueFlag]
	,	d.[VcfManualValueFlag]
	,  d.[DeliveredGrossManualValueFlag]
	,  d.[DeliveredNetManualValueFlag]
	,	d.[TransactionSubLineItemGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupQualityIndex]
	,	d.[TransactionLineItemGuid]
	,	d.[ProductGuid]
	,	d.[TransactionGuid]
	,	d.[StorageLocationTankGuid]
	,	d.[MeterGuid]
	,	d.[PackageManualValueFlag]
	,	d.[CleanLineItem]
	,	d.[CleanLineDeductItem]
	,	d.[CleanLineDeductQuantity]
	,	d.[CleanLinePackQuantity]
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
--Creating Insert / Update Trigger for tblTransactionSubLineItems
CREATE TRIGGER dbo.trg_insupd_tblTransactionSubLineItems_ForSync 
   ON dbo.tblTransactionSubLineItems
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
                    ,d.TransactionSubLineItemGuid AS Deleted_PK_TransactionSubLineItemGuid
                    ,i.TransactionSubLineItemGuid AS Inserted_PK_TransactionSubLineItemGuid
                    ,d.TransactionLineItemGuid AS Deleted_FK_ParentPK
                    ,i.TransactionLineItemGuid AS Inserted_FK_ParentPK
                    ,i.CreatedDate AS Inserted_CreatedDate 
                    ,i.UpdatedDate AS Inserted_UpdatedDate 
				    ,CAST(NULL AS uniqueidentifier) AS CurrentSiteGuid 
				    ,CAST(NULL AS uniqueidentifier) AS PreviousSiteGuid 
				    ,i._RowVersion AS Inserted_RowVersion 
				    ,CAST(NULL AS varbinary(8)) AS Deleted_RowVersion 
				INTO #ChangeList
			    FROM Inserted i 
			    FULL OUTER JOIN Deleted d ON 
				    d.TransactionSubLineItemGuid = i.TransactionSubLineItemGuid
 
           UPDATE currentTrackingData
			SET UpdatedDate = entityChanges.Inserted_UpdatedDate 
			    		,UpdatedContext = entityChanges.ChangeContext 
 				        ,UpdatedRowVersion = entityChanges.Inserted_RowVersion 
     					,CurrentSiteGuid = entityChanges.CurrentSiteGuid 
 	    				,PreviousSiteGuid = CASE WHEN (ISNULL(CONVERT(nvarchar(64), entityChanges.CurrentSiteGuid),'') <> ISNULL(CONVERT(nvarchar(64), currentTrackingData.CurrentSiteGuid),'')) THEN currentTrackingData.CurrentSiteGuid ELSE currentTrackingData.PreviousSiteGuid END 
			FROM track.tblTransactionSubLineItems As currentTrackingData 
		    JOIN #ChangeList As entityChanges 
		    ON entityChanges.Inserted_PK_TransactionSubLineItemGuid = currentTrackingData.PK_TransactionSubLineItemGuid
 
 
		    INSERT track.tblTransactionSubLineItems (InsertedDate 
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
				    ,PK_TransactionSubLineItemGuid
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
				    ,entityChanges.Inserted_PK_TransactionSubLineItemGuid
				    ,entityChanges.Inserted_FK_ParentPK
		    FROM #ChangeList As entityChanges 
		    			WHERE NOT EXISTS ( SELECT 1 
		    								FROM   track.tblTransactionSubLineItems As currentTrackingData
		    								WHERE entityChanges.Inserted_PK_TransactionSubLineItemGuid = currentTrackingData.PK_TransactionSubLineItemGuid
)
    END
END 

GO
--Creating Delete Trigger for tblTransactionSubLineItems
CREATE TRIGGER dbo.trg_del_tblTransactionSubLineItems_ForSync 
   ON dbo.tblTransactionSubLineItems
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
						,d.TransactionSubLineItemGuid AS Deleted_PK_TransactionSubLineItemGuid
                        ,d.TransactionSubLineItemGuid AS Inserted_PK_TransactionSubLineItemGuid
                      ,d.TransactionLineItemGuid AS Deleted_FK_ParentPK
						,d.CreatedDate AS Inserted_CreatedDate 
						,d.UpdatedDate AS Inserted_UpdatedDate 
						,NULL AS CurrentSiteGuid 
						,NULL AS PreviousSiteGuid 
						,d._RowVersion AS Inserted_RowVersion 
						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion 
					FROM Deleted d 
				) 
				MERGE INTO track.tblTransactionSubLineItems As currentTrackingData 
					USING ChangeList As entityChanges 
						ON entityChanges.Deleted_PK_TransactionSubLineItemGuid = currentTrackingData.PK_TransactionSubLineItemGuid
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
						,PK_TransactionSubLineItemGuid
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
						,entityChanges.Deleted_PK_TransactionSubLineItemGuid
				        ,entityChanges.Deleted_FK_ParentPK
				);
    END
END 

GO
CREATE TRIGGER [dbo].[trg_Audit_ins_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionSubLineItems','D')=1 
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
	INSERT INTO [fmaudit].tblTransactionSubLineItems (
		[SequenceID]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[Vcf]
	,	[Density]
	,	[Temperature]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[BatchNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[TankStatus]
	,	[MeterFactor]
	,	[MeterStart]
	,	[MeterStop]
	,	[MeterStopDateTime]
	,	[MeterStartDateTime]
	,	[FreezePoint]
	,	[DifferentialPressure]
	,	[DosageRate]
	,	[DeleteFlag]
	,	[PresetAmount]
	,	[StorageLocationID]
	,	[MeterID]
	,	[COAID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionInventoryDate]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,  [DeliveredGrossManualValueFlag]
	,  [DeliveredNetManualValueFlag]
	,	[TransactionSubLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[TransactionLineItemGuid]
	,	[ProductGuid]
	,	[TransactionGuid]
	,	[StorageLocationTankGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
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
		i.[SequenceID]
	,	i.[Product]
	,	i.[ProductCode]
	,	i.[ProductType]
	,	i.[GrossQuantity]
	,  i.[DeliveredGrossQuantity]
	,	i.[NetQuantity]
	,  i.[DeliveredNetQuantity]
	,  i.[Pressure]	
	,	i.[Vcf]
	,	i.[Density]
	,	i.[Temperature]
	,	i.[Customs]
	,	i.[ArmNumber]
	,	i.[LineNumber]
	,	i.[BatchNumber]
	,	i.[LineFill]
	,	i.[BottomVolume]
	,	i.[NetCapacity]
	,	i.[TankStatus]
	,	i.[MeterFactor]
	,	i.[MeterStart]
	,	i.[MeterStop]
	,	i.[MeterStopDateTime]
	,	i.[MeterStartDateTime]
	,	i.[FreezePoint]
	,	i.[DifferentialPressure]
	,	i.[DosageRate]
	,	i.[DeleteFlag]
	,	i.[PresetAmount]
	,	i.[StorageLocationID]
	,	i.[MeterID]
	,	i.[COAID]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[TransactionInventoryDate]
	,	i.[Tax1]
	,	i.[Tax2]
	,	i.[Tax3]
	,	i.[Tax4]
	,	i.[Tax5]
	,	i.[TransVersion]
	,	i.[ImproperAdditization]
	,	i.[BrokenBlend]
	,	i.[Flag01]
	,	i.[Flag02]
	,	i.[Flag03]
	,	i.[Flag04]
	,	i.[Flag05]
	,	i.[Flag06]
	,	i.[Number01]
	,	i.[Number02]
	,	i.[Number03]
	,	i.[Number04]
	,	i.[Number05]
	,	i.[Number06]
	,	i.[Date01]
	,	i.[Date02]
	,	i.[Date03]
	,	i.[Date04]
	,	i.[MassQuantity]
	,	i.[NetManualValueFlag]
	,	i.[MassManualValueFlag]
	,	i.[GrossManualValueFlag]
	,	i.[VcfManualValueFlag]
	,  i.[DeliveredGrossManualValueFlag]
	,  i.[DeliveredNetManualValueFlag]
	,	i.[TransactionSubLineItemGuid]
	,	i.[_RowVersion]
	,	i.[LookupTransactionStatusIndex]
	,	i.[LookupQualityIndex]
	,	i.[TransactionLineItemGuid]
	,	i.[ProductGuid]
	,	i.[TransactionGuid]
	,	i.[StorageLocationTankGuid]
	,	i.[MeterGuid]
	,	i.[PackageManualValueFlag]
	,	i.[CleanLineItem]
	,	i.[CleanLineDeductItem]
	,	i.[CleanLineDeductQuantity]
	,	i.[CleanLinePackQuantity]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblTransactionSubLineItems','D')=1 
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
	TransactionSubLineItemGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblTransactionSubLineItems (
		[SequenceID]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[Vcf]
	,	[Density]
	,	[Temperature]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[BatchNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[TankStatus]
	,	[MeterFactor]
	,	[MeterStart]
	,	[MeterStop]
	,	[MeterStopDateTime]
	,	[MeterStartDateTime]
	,	[FreezePoint]
	,	[DifferentialPressure]
	,	[DosageRate]
	,	[DeleteFlag]
	,	[PresetAmount]
	,	[StorageLocationID]
	,	[MeterID]
	,	[COAID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionInventoryDate]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,  [DeliveredGrossManualValueFlag]
	,  [DeliveredNetManualValueFlag]
	,	[TransactionSubLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[TransactionLineItemGuid]
	,	[ProductGuid]
	,	[TransactionGuid]
	,	[StorageLocationTankGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
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
	OUTPUT inserted.[TransactionSubLineItemGuid] AS 'TransactionSubLineItemGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[SequenceID]
	,	d.[Product]
	,	d.[ProductCode]
	,	d.[ProductType]
	,	d.[GrossQuantity]
	,  d.[DeliveredGrossQuantity]
	,	d.[NetQuantity]
	,  d.[DeliveredNetQuantity]
	,  d.[Pressure]	
	,	d.[Vcf]
	,	d.[Density]
	,	d.[Temperature]
	,	d.[Customs]
	,	d.[ArmNumber]
	,	d.[LineNumber]
	,	d.[BatchNumber]
	,	d.[LineFill]
	,	d.[BottomVolume]
	,	d.[NetCapacity]
	,	d.[TankStatus]
	,	d.[MeterFactor]
	,	d.[MeterStart]
	,	d.[MeterStop]
	,	d.[MeterStopDateTime]
	,	d.[MeterStartDateTime]
	,	d.[FreezePoint]
	,	d.[DifferentialPressure]
	,	d.[DosageRate]
	,	d.[DeleteFlag]
	,	d.[PresetAmount]
	,	d.[StorageLocationID]
	,	d.[MeterID]
	,	d.[COAID]
	,	d.[CreatedBy]
	,	d.[CreatedDate]
	,	d.[UpdatedBy]
	,	d.[UpdatedDate]
	,	d.[TransactionInventoryDate]
	,	d.[Tax1]
	,	d.[Tax2]
	,	d.[Tax3]
	,	d.[Tax4]
	,	d.[Tax5]
	,	d.[TransVersion]
	,	d.[ImproperAdditization]
	,	d.[BrokenBlend]
	,	d.[Flag01]
	,	d.[Flag02]
	,	d.[Flag03]
	,	d.[Flag04]
	,	d.[Flag05]
	,	d.[Flag06]
	,	d.[Number01]
	,	d.[Number02]
	,	d.[Number03]
	,	d.[Number04]
	,	d.[Number05]
	,	d.[Number06]
	,	d.[Date01]
	,	d.[Date02]
	,	d.[Date03]
	,	d.[Date04]
	,	d.[MassQuantity]
	,	d.[NetManualValueFlag]
	,	d.[MassManualValueFlag]
	,	d.[GrossManualValueFlag]
	,	d.[VcfManualValueFlag]
	,  d.[DeliveredGrossManualValueFlag]
	,  d.[DeliveredNetManualValueFlag]
	,	d.[TransactionSubLineItemGuid]
	,	d.[_RowVersion]
	,	d.[LookupTransactionStatusIndex]
	,	d.[LookupQualityIndex]
	,	d.[TransactionLineItemGuid]
	,	d.[ProductGuid]
	,	d.[TransactionGuid]
	,	d.[StorageLocationTankGuid]
	,	d.[MeterGuid]
	,	d.[PackageManualValueFlag]
	,	d.[CleanLineItem]
	,	d.[CleanLineDeductItem]
	,	d.[CleanLineDeductQuantity]
	,	d.[CleanLinePackQuantity]
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
 
	INSERT INTO [fmaudit].tblTransactionSubLineItems (
		[SequenceID]
	,	[Product]
	,	[ProductCode]
	,	[ProductType]
	,	[GrossQuantity]
	,  [DeliveredGrossQuantity]
	,	[NetQuantity]
	,  [DeliveredNetQuantity]
	,  [Pressure]	
	,	[Vcf]
	,	[Density]
	,	[Temperature]
	,	[Customs]
	,	[ArmNumber]
	,	[LineNumber]
	,	[BatchNumber]
	,	[LineFill]
	,	[BottomVolume]
	,	[NetCapacity]
	,	[TankStatus]
	,	[MeterFactor]
	,	[MeterStart]
	,	[MeterStop]
	,	[MeterStopDateTime]
	,	[MeterStartDateTime]
	,	[FreezePoint]
	,	[DifferentialPressure]
	,	[DosageRate]
	,	[DeleteFlag]
	,	[PresetAmount]
	,	[StorageLocationID]
	,	[MeterID]
	,	[COAID]
	,	[CreatedBy]
	,	[CreatedDate]
	,	[UpdatedBy]
	,	[UpdatedDate]
	,	[TransactionInventoryDate]
	,	[Tax1]
	,	[Tax2]
	,	[Tax3]
	,	[Tax4]
	,	[Tax5]
	,	[TransVersion]
	,	[ImproperAdditization]
	,	[BrokenBlend]
	,	[Flag01]
	,	[Flag02]
	,	[Flag03]
	,	[Flag04]
	,	[Flag05]
	,	[Flag06]
	,	[Number01]
	,	[Number02]
	,	[Number03]
	,	[Number04]
	,	[Number05]
	,	[Number06]
	,	[Date01]
	,	[Date02]
	,	[Date03]
	,	[Date04]
	,	[MassQuantity]
	,	[NetManualValueFlag]
	,	[MassManualValueFlag]
	,	[GrossManualValueFlag]
	,	[VcfManualValueFlag]
	,  [DeliveredGrossManualValueFlag]
	,  [DeliveredNetManualValueFlag]
	,	[TransactionSubLineItemGuid]
	,	[OriginalRowVersion]
	,	[LookupTransactionStatusIndex]
	,	[LookupQualityIndex]
	,	[TransactionLineItemGuid]
	,	[ProductGuid]
	,	[TransactionGuid]
	,	[StorageLocationTankGuid]
	,	[MeterGuid]
	,	[PackageManualValueFlag]
	,	[CleanLineItem]
	,	[CleanLineDeductItem]
	,	[CleanLineDeductQuantity]
	,	[CleanLinePackQuantity]
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
		i.[SequenceID]
	,	i.[Product]
	,	i.[ProductCode]
	,	i.[ProductType]
	,	i.[GrossQuantity]
	,  i.[DeliveredGrossQuantity]
	,	i.[NetQuantity]
	,  i.[DeliveredNetQuantity]
	,  i.[Pressure]	
	,	i.[Vcf]
	,	i.[Density]
	,	i.[Temperature]
	,	i.[Customs]
	,	i.[ArmNumber]
	,	i.[LineNumber]
	,	i.[BatchNumber]
	,	i.[LineFill]
	,	i.[BottomVolume]
	,	i.[NetCapacity]
	,	i.[TankStatus]
	,	i.[MeterFactor]
	,	i.[MeterStart]
	,	i.[MeterStop]
	,	i.[MeterStopDateTime]
	,	i.[MeterStartDateTime]
	,	i.[FreezePoint]
	,	i.[DifferentialPressure]
	,	i.[DosageRate]
	,	i.[DeleteFlag]
	,	i.[PresetAmount]
	,	i.[StorageLocationID]
	,	i.[MeterID]
	,	i.[COAID]
	,	i.[CreatedBy]
	,	i.[CreatedDate]
	,	i.[UpdatedBy]
	,	i.[UpdatedDate]
	,	i.[TransactionInventoryDate]
	,	i.[Tax1]
	,	i.[Tax2]
	,	i.[Tax3]
	,	i.[Tax4]
	,	i.[Tax5]
	,	i.[TransVersion]
	,	i.[ImproperAdditization]
	,	i.[BrokenBlend]
	,	i.[Flag01]
	,	i.[Flag02]
	,	i.[Flag03]
	,	i.[Flag04]
	,	i.[Flag05]
	,	i.[Flag06]
	,	i.[Number01]
	,	i.[Number02]
	,	i.[Number03]
	,	i.[Number04]
	,	i.[Number05]
	,	i.[Number06]
	,	i.[Date01]
	,	i.[Date02]
	,	i.[Date03]
	,	i.[Date04]
	,	i.[MassQuantity]
	,	i.[NetManualValueFlag]
	,	i.[MassManualValueFlag]
	,	i.[GrossManualValueFlag]
	,	i.[VcfManualValueFlag]
	,  i.[DeliveredGrossManualValueFlag]
	,  i.[DeliveredNetManualValueFlag]
	,	i.[TransactionSubLineItemGuid]
	,	i.[_RowVersion]
	,	i.[LookupTransactionStatusIndex]
	,	i.[LookupQualityIndex]
	,	i.[TransactionLineItemGuid]
	,	i.[ProductGuid]
	,	i.[TransactionGuid]
	,	i.[StorageLocationTankGuid]
	,	i.[MeterGuid]
	,	i.[PackageManualValueFlag]
	,	i.[CleanLineItem]
	,	i.[CleanLineDeductItem]
	,	i.[CleanLineDeductQuantity]
	,	i.[CleanLinePackQuantity]
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
			agl.[TransactionSubLineItemGuid]=i.[TransactionSubLineItemGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END
GO


CREATE TRIGGER [dbo].[trg_fmcdc_tblTransactionSubLineItems]
ON [dbo].[tblTransactionSubLineItems]
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
	DECLARE @eventType nvarchar(20)
	IF ((EXISTS(SELECT * FROM inserted)) AND (EXISTS(SELECT * FROM deleted)))
		SELECT @eventType = 'update'
	ELSE IF (EXISTS(SELECT * FROM inserted))
		SELECT @eventType = 'insert'
	ELSE IF (EXISTS(SELECT * FROM deleted))
		SELECT @eventType = 'delete'
	IF (@eventType = 'delete')
	BEGIN
		DECLARE  @context_info varbinary(128)
		DECLARE  @context_info_str varchar(128)
		SELECT @Context_Info = CONTEXT_INFO()
		SELECT @context_info_str = CAST (@context_info as varchar(128))
		IF (@context_info_str = 'dbo.fm_ArchiveTransaction')
		BEGIN
			RETURN
		END
		INSERT INTO fmcdc.[tblTransactionSubLineItems]
		(
		[SequenceID]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [Vcf]
		, [Density]
		, [Temperature]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [BatchNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [TankStatus]
		, [MeterFactor]
		, [MeterStart]
		, [MeterStop]
		, [MeterStopDateTime]
		, [MeterStartDateTime]
		, [FreezePoint]
		, [DifferentialPressure]
		, [DosageRate]
		, [DeleteFlag]
		, [PresetAmount]
		, [StorageLocationID]
		, [MeterID]
		, [COAID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionInventoryDate]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionSubLineItemGuid]
		, [SourceRowVersion]
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [TransactionLineItemGuid]
		, [ProductGuid]
		, [TransactionGuid]
		, [StorageLocationTankGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[SequenceID]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [Vcf]
		, [Density]
		, [Temperature]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [BatchNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [TankStatus]
		, [MeterFactor]
		, [MeterStart]
		, [MeterStop]
		, [MeterStopDateTime]
		, [MeterStartDateTime]
		, [FreezePoint]
		, [DifferentialPressure]
		, [DosageRate]
		, [DeleteFlag]
		, [PresetAmount]
		, [StorageLocationID]
		, [MeterID]
		, [COAID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionInventoryDate]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionSubLineItemGuid]
		, CONVERT(bigint, _RowVersion)
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [TransactionLineItemGuid]
		, [ProductGuid]
		, [TransactionGuid]
		, [StorageLocationTankGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [_ClusterIdx]
		, GETDATE()
		, 1
		FROM deleted
	END
	ELSE IF ((@eventType = 'insert') OR (@eventType = 'update'))
	BEGIN
		INSERT INTO fmcdc.[tblTransactionSubLineItems]
		(
		[SequenceID]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [Vcf]
		, [Density]
		, [Temperature]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [BatchNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [TankStatus]
		, [MeterFactor]
		, [MeterStart]
		, [MeterStop]
		, [MeterStopDateTime]
		, [MeterStartDateTime]
		, [FreezePoint]
		, [DifferentialPressure]
		, [DosageRate]
		, [DeleteFlag]
		, [PresetAmount]
		, [StorageLocationID]
		, [MeterID]
		, [COAID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionInventoryDate]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionSubLineItemGuid]
		, [SourceRowVersion]
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [TransactionLineItemGuid]
		, [ProductGuid]
		, [TransactionGuid]
		, [StorageLocationTankGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [_ClusterIdx]
		, [RecordUpdatedDate]
		, [IsRecordDeleted])
		SELECT 
		[SequenceID]
		, [Product]
		, [ProductCode]
		, [ProductType]
		, [GrossQuantity]
		, [DeliveredGrossQuantity]
		, [NetQuantity]
		, [DeliveredNetQuantity]
		, [Pressure]		
		, [Vcf]
		, [Density]
		, [Temperature]
		, [Customs]
		, [ArmNumber]
		, [LineNumber]
		, [BatchNumber]
		, [LineFill]
		, [BottomVolume]
		, [NetCapacity]
		, [TankStatus]
		, [MeterFactor]
		, [MeterStart]
		, [MeterStop]
		, [MeterStopDateTime]
		, [MeterStartDateTime]
		, [FreezePoint]
		, [DifferentialPressure]
		, [DosageRate]
		, [DeleteFlag]
		, [PresetAmount]
		, [StorageLocationID]
		, [MeterID]
		, [COAID]
		, [CreatedBy]
		, [CreatedDate]
		, [UpdatedBy]
		, [UpdatedDate]
		, [TransactionInventoryDate]
		, [Tax1]
		, [Tax2]
		, [Tax3]
		, [Tax4]
		, [Tax5]
		, [TransVersion]
		, [ImproperAdditization]
		, [BrokenBlend]
		, [Flag01]
		, [Flag02]
		, [Flag03]
		, [Flag04]
		, [Flag05]
		, [Flag06]
		, [Number01]
		, [Number02]
		, [Number03]
		, [Number04]
		, [Number05]
		, [Number06]
		, [Date01]
		, [Date02]
		, [Date03]
		, [Date04]
		, [MassQuantity]
		, [NetManualValueFlag]
		, [MassManualValueFlag]
		, [GrossManualValueFlag]
		, [VcfManualValueFlag]
		, [DeliveredGrossManualValueFlag]
		, [DeliveredNetManualValueFlag]
		, [TransactionSubLineItemGuid]
		, CONVERT(bigint, _RowVersion)
		, [LookupTransactionStatusIndex]
		, [LookupQualityIndex]
		, [TransactionLineItemGuid]
		, [ProductGuid]
		, [TransactionGuid]
		, [StorageLocationTankGuid]
		, [MeterGuid]
		, [PackageManualValueFlag]
		, [CleanLineItem]
		, [CleanLineDeductItem]
		, [CleanLineDeductQuantity]
		, [CleanLinePackQuantity]
		, [_ClusterIdx]
		, GETDATE()
		, NULL
		FROM inserted
	END
END
GO
-- Disable fmcdc trigger during deployment. Trigger is to be enabled only when ready to start capturing fmcdc changes.
DISABLE TRIGGER [dbo].[trg_fmcdc_tblTransactionSubLineItems] ON [dbo].[tblTransactionSubLineItems]
GO