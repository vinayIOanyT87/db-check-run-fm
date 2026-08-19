CREATE TABLE [dbo].[tblDispatchGridColumn] (
    [DispatchGridColumnGuid]                    UNIQUEIDENTIFIER   CONSTRAINT [DF_tblDispatchGridColumn_GUID] DEFAULT (newid()) NOT NULL,
    [DispatchGridGuid]                          UNIQUEIDENTIFIER   NOT NULL,
    [DispatchGridID]                            NVARCHAR (50)      CONSTRAINT [DF_tblDispatchGridColumn_DispatchGridID] DEFAULT ('') NOT NULL,
    [LookupDispatchGridColumnTypeIndex]         INT                NOT NULL,
    [ID]                                        NVARCHAR (50)      CONSTRAINT [DF_tblDispatchGridColumn_ID] DEFAULT ('') NOT NULL,
    [ColumnOrder]                               INT                CONSTRAINT [DF_tblDispatchGridColumn_ColumnOrder] DEFAULT ((0)) NOT NULL,
    [CreatedDate]                               DATETIMEOFFSET (7) CONSTRAINT [DF_tblDispatchGridColumn_CreatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [CreatedBy]                                 [dbo].[udtUserID]  CONSTRAINT [DF_tblDispatchGridColumn_CreatedBy] DEFAULT (suser_sname()) NOT NULL,
    [UpdatedDate]                               DATETIMEOFFSET (7) CONSTRAINT [DF_tblDispatchGridColumn_UpdatedDate] DEFAULT (sysdatetimeoffset()) NOT NULL,
    [UpdatedBy]                                 [dbo].[udtUserID]  CONSTRAINT [DF_tblDispatchGridColumn_UpdatedBy] DEFAULT (suser_sname()) NOT NULL,
    [_RowVersion]                               ROWVERSION         NOT NULL,
    [UserDataFieldTransactionAliasGuid]         UNIQUEIDENTIFIER   NULL,
    [UserDataFieldTransactionAliasLineItemGuid] UNIQUEIDENTIFIER   NULL,
    [AliasName]                                 NVARCHAR (50)      NULL,
    [UserDataNumber]                            INT                NULL,
    [UserGuid]                                  UNIQUEIDENTIFIER   NOT NULL,
    [_ClusterIdx]                               BIGINT             IDENTITY (1, 1) NOT NULL,
    CONSTRAINT [PK_tblDispatchGridColumn_GUID] PRIMARY KEY NONCLUSTERED ([DispatchGridColumnGuid] ASC),
    CONSTRAINT [FK_tblDispatchGridColumn_DispatchGridGuid] FOREIGN KEY ([DispatchGridGuid]) REFERENCES [dbo].[tblDispatchGrid] ([DispatchGridGuid]),
    CONSTRAINT [FK_tblDispatchGridColumn_LookupDispatchGridColumnTypeIndex] FOREIGN KEY ([LookupDispatchGridColumnTypeIndex]) REFERENCES [lookup].[tblDispatchGridColumnType] ([DispatchGridColumnTypeIndex]),
    CONSTRAINT [FK_tblDispatchGridColumn_UserDataFieldTransactionAliasGuid] FOREIGN KEY ([UserDataFieldTransactionAliasGuid]) REFERENCES [dbo].[tblUserDataFieldTransactionAlias] ([UserDataFieldTransactionAliasGuid]),
    CONSTRAINT [FK_tblDispatchGridColumn_UserDataFieldTransactionAliasLineItemGuid] FOREIGN KEY ([UserDataFieldTransactionAliasLineItemGuid]) REFERENCES [dbo].[tblUserDataFieldTransactionAliasLineItem] ([UserDataFieldTransactionAliasLineItemGuid]),
    CONSTRAINT [FK_tblDispatchGridColumn_UserGuid] FOREIGN KEY ([UserGuid]) REFERENCES [dbo].[tblUsers] ([UserGuid])
);




GO
CREATE NONCLUSTERED INDEX [IX_tblDispatchGridColumn_CreatedDate]
    ON [dbo].[tblDispatchGridColumn]([CreatedDate] ASC);




GO
CREATE UNIQUE NONCLUSTERED INDEX [IXU_tblDispatchGridColumn_ID_UserDataFieldTransAliasGuid_UserDataFieldTransAliasLineItemGuid_DispatchGridGuid_UserGuid]
    ON [dbo].[tblDispatchGridColumn]([ID] ASC, [UserDataFieldTransactionAliasGuid] ASC, [UserDataFieldTransactionAliasLineItemGuid] ASC, [DispatchGridGuid] ASC, [UserGuid] ASC);


GO
CREATE TRIGGER [dbo].[trg_Audit_del_tblDispatchGridColumn] ON [dbo].[tblDispatchGridColumn] AFTER DELETE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchGridColumn','D')=1 
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
	INSERT INTO [fmaudit].tblDispatchGridColumn (
		[DispatchGridColumnGuid]
	,	[DispatchGridGuid]
	,	[DispatchGridID]
	,	[LookupDispatchGridColumnTypeIndex]
	,	[ID]
	,	[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[AliasName]
	,	[UserDataNumber]
	,	[UserGuid]
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
		d.[DispatchGridColumnGuid]
	,	d.[DispatchGridGuid]
	,	d.[DispatchGridID]
	,	d.[LookupDispatchGridColumnTypeIndex]
	,	d.[ID]
	,	d.[ColumnOrder]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[UserDataFieldTransactionAliasGuid]
	,	d.[UserDataFieldTransactionAliasLineItemGuid]
	,	d.[AliasName]
	,	d.[UserDataNumber]
	,	d.[UserGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_ins_tblDispatchGridColumn] ON [dbo].[tblDispatchGridColumn] AFTER INSERT 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchGridColumn','D')=1 
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
	INSERT INTO [fmaudit].tblDispatchGridColumn (
		[DispatchGridColumnGuid]
	,	[DispatchGridGuid]
	,	[DispatchGridID]
	,	[LookupDispatchGridColumnTypeIndex]
	,	[ID]
	,	[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[AliasName]
	,	[UserDataNumber]
	,	[UserGuid]
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
		i.[DispatchGridColumnGuid]
	,	i.[DispatchGridGuid]
	,	i.[DispatchGridID]
	,	i.[LookupDispatchGridColumnTypeIndex]
	,	i.[ID]
	,	i.[ColumnOrder]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[UserDataFieldTransactionAliasGuid]
	,	i.[UserDataFieldTransactionAliasLineItemGuid]
	,	i.[AliasName]
	,	i.[UserDataNumber]
	,	i.[UserGuid]
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
CREATE TRIGGER [dbo].[trg_Audit_upd_tblDispatchGridColumn] ON [dbo].[tblDispatchGridColumn] AFTER UPDATE 
AS
BEGIN
	SET NOCOUNT ON;
	-- Verifies whether the trigger is active based on configuration and Audit
	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]
	IF [fmaudit].[udf_DisableTriggerByAuditRule]('dbo','tblDispatchGridColumn','D')=1 
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
	DispatchGridColumnGuid	uniqueidentifier NULL
		,_AuditEventType CHAR(1)
		,_AuditEventSequence TINYINT
		,_AuditCreatedDate DATETIMEOFFSET
		,_AuditGUID UNIQUEIDENTIFIER
	)
 
	INSERT INTO [fmaudit].tblDispatchGridColumn (
		[DispatchGridColumnGuid]
	,	[DispatchGridGuid]
	,	[DispatchGridID]
	,	[LookupDispatchGridColumnTypeIndex]
	,	[ID]
	,	[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[AliasName]
	,	[UserDataNumber]
	,	[UserGuid]
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
	OUTPUT inserted.[DispatchGridColumnGuid] AS 'DispatchGridColumnGuid'
		,  inserted._AuditEventType AS '_AuditEventType'
		,  inserted._AuditEventSequence AS '_AuditEventSequence'
		,  inserted._AuditCreatedDate AS '_AuditCreatedDate'
		,  inserted._AuditGUID AS '_AuditGUID'
		INTO @AuditGuidList
	SELECT 
		d.[DispatchGridColumnGuid]
	,	d.[DispatchGridGuid]
	,	d.[DispatchGridID]
	,	d.[LookupDispatchGridColumnTypeIndex]
	,	d.[ID]
	,	d.[ColumnOrder]
	,	d.[CreatedDate]
	,	d.[CreatedBy]
	,	d.[UpdatedDate]
	,	d.[UpdatedBy]
	,	d.[_RowVersion]
	,	d.[UserDataFieldTransactionAliasGuid]
	,	d.[UserDataFieldTransactionAliasLineItemGuid]
	,	d.[AliasName]
	,	d.[UserDataNumber]
	,	d.[UserGuid]
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
 
	INSERT INTO [fmaudit].tblDispatchGridColumn (
		[DispatchGridColumnGuid]
	,	[DispatchGridGuid]
	,	[DispatchGridID]
	,	[LookupDispatchGridColumnTypeIndex]
	,	[ID]
	,	[ColumnOrder]
	,	[CreatedDate]
	,	[CreatedBy]
	,	[UpdatedDate]
	,	[UpdatedBy]
	,	[OriginalRowVersion]
	,	[UserDataFieldTransactionAliasGuid]
	,	[UserDataFieldTransactionAliasLineItemGuid]
	,	[AliasName]
	,	[UserDataNumber]
	,	[UserGuid]
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
		i.[DispatchGridColumnGuid]
	,	i.[DispatchGridGuid]
	,	i.[DispatchGridID]
	,	i.[LookupDispatchGridColumnTypeIndex]
	,	i.[ID]
	,	i.[ColumnOrder]
	,	i.[CreatedDate]
	,	i.[CreatedBy]
	,	i.[UpdatedDate]
	,	i.[UpdatedBy]
	,	i.[_RowVersion]
	,	i.[UserDataFieldTransactionAliasGuid]
	,	i.[UserDataFieldTransactionAliasLineItemGuid]
	,	i.[AliasName]
	,	i.[UserDataNumber]
	,	i.[UserGuid]
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
			agl.[DispatchGridColumnGuid]=i.[DispatchGridColumnGuid] 
		)
		WHERE	agl._AuditEventType='U'
		AND		agl._AuditEventSequence=1 
		AND		agl._AuditCreatedDate= @_AuditDatetime
END

GO
CREATE UNIQUE CLUSTERED INDEX [IX_tblDispatchGridColumn_ClusterIdx]
    ON [dbo].[tblDispatchGridColumn]([_ClusterIdx] ASC);
