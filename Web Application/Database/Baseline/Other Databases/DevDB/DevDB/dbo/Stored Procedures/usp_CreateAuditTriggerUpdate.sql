CREATE PROCEDURE [dbo].[usp_CreateAuditTriggerUpdate]
AS
BEGIN
	DECLARE @Schema NVARCHAR(100)
		,	@Table NVARCHAR(500)
		,	@AuditTable NVARCHAR(600)
		,	@Column NVARCHAR(500)
		,	@DataType NVARCHAR(500)
		,	@MaxLength INT
		,	@Position INT
		,	@Separator VARCHAR(1)
		,	@Trigger NVARCHAR(500)
		

	DECLARE TriggerCursor CURSOR FOR
		SELECT Table_Schema,Table_Name
		FROM FuelsManagerDB.information_schema.tables
		WHERE Table_Schema IN('dbo','map')
		--AND Table_Name IN ('tblAlarmAndEventLog')
		AND LEFT(Table_Name,3)<>'vw_'
		ORDER BY Table_Schema,Table_Name

	OPEN TriggerCursor
	FETCH NEXT FROM TriggerCursor INTO @Schema,@Table
	WHILE @@FETCH_STATUS=0
	BEGIN
		IF @Schema <> 'dbo'
			SET @AuditTable=@Schema+'_'+@Table
		ELSE
			SET @AuditTable=@Table
		SET @Separator =''
		SET @Trigger = 'trg_Audit_upd_'+@Table
		PRINT 'IF EXISTS(SELECT 1 FROM sys.triggers WHERE [name]='''+@Trigger+''')'
		PRINT '	DROP TRIGGER ['+@Schema+'].['+@Trigger+'];'
		PRINT 'GO'
		
		PRINT 'CREATE TRIGGER ['+@Trigger+'] ON ['+@Schema+'].['+@Table+'] AFTER UPDATE '
		PRINT 'AS'
		PRINT 'BEGIN'
		PRINT '	SET NOCOUNT ON;'
		PRINT '	-- Verifies whether the trigger is active based on configuration and Audit'
		PRINT '	-- rules which are resolved by the User Defined Function [fmaudit].[udf_DisableTriggerByAuditRule]'
		PRINT '	IF [fmaudit].[udf_DisableTriggerByAuditRule]('''+@Schema+''','''+@Table+''',''D'')=1 '
		PRINT '		RETURN'


		PRINT '	DECLARE @_AuditEventType CHAR(1)'
		PRINT '	,	@_AuditEventSequence TINYINT'
		PRINT '	,	@_AuditSessionGUID UNIQUEIDENTIFIER'
		PRINT '	,	@_AuditSessionTokenID UNIQUEIDENTIFIER'
		PRINT '	,	@_AuditSiteGUID UNIQUEIDENTIFIER'
		PRINT '	,	@_AuditGUID UNIQUEIDENTIFIER'
		PRINT '	,	@AuditDatetime DATETIMEOFFSET;'
		PRINT '	SET @AuditDatetime = SYSDATETIMEOFFSET();'
		PRINT '	SET @_AuditEventType= ''I'' -- For Deletes '
		PRINT '	SET @_AuditEventSequence= 1 '
		PRINT '	SELECT	@_AuditSessionGUID=s.SessionGuid '
		PRINT '		,	@_AuditSessionTokenID=s.SessionTokenID '
		PRINT '		,	@_AuditSessionTokenID=s.SessionTokenID '
		PRINT '		,	@_AuditSiteGUID=s.LoginSiteGuid '
		PRINT '	FROM dbo.tblSessions s '
		PRINT '	WHERE s.SqlServerSessionID=@@SPID '
		PRINT '	INSERT INTO [fmaudit].'+@AuditTable+' WITH(ROWLOCK)('
		
		DECLARE ColumnCursor SCROLL CURSOR FOR
			SELECT COLUMN_NAME
			FROM information_schema.columns
			WHERE TABLE_SCHEMA = @Schema
			AND TABLE_NAME= @Table
			AND Data_Type NOT IN ('image','xml','text')
			ORDER BY ORDINAL_POSITION
		
		OPEN ColumnCursor
		FETCH NEXT FROM ColumnCursor INTO @Column
		WHILE @@FETCH_STATUS=0
		BEGIN
			PRINT '	'+@Separator+'	['+REPLACE(@Column,'_RowVersion','OriginalRowVersion')+']'
			SET @Separator=','
			FETCH NEXT FROM ColumnCursor INTO @Column
		END
		
		PRINT '	,	[_AuditEventType]'
		PRINT '	,	[_AuditEventSequence]'
		PRINT '	,	[_AuditSessionGUID]'
		PRINT '	,	[_AuditSessionTokenID]'
		PRINT '	,	[_AuditCreatedDate]'
		PRINT '	,	[_AuditSiteGUID]'
		PRINT '	,	[_AuditGUID]'
		
		PRINT '	)'
		PRINT '	SELECT '
		SET @Separator=''
		
		FETCH FIRST FROM ColumnCursor INTO @Column
		WHILE @@FETCH_STATUS=0
		BEGIN
			PRINT '	'+@Separator+'	d.['+@Column+']'
			SET @Separator=','
			FETCH NEXT FROM ColumnCursor INTO @Column
		END
		PRINT '	,	@_AuditEventType'
		PRINT '	,	@_AuditEventSequence'
		PRINT '	,	@_AuditSessionGUID'
		PRINT '	,	@_AuditSessionTokenID'
		PRINT '	,	@AuditDatetime'
		PRINT '	,	@_AuditSiteGUID'
		PRINT '	,	NEWID()'

		
		PRINT '	FROM deleted d'

		PRINT '	INSERT INTO [fmaudit].'+@AuditTable+' WITH(ROWLOCK)('
		FETCH FIRST FROM ColumnCursor INTO @Column
		WHILE @@FETCH_STATUS=0
		BEGIN
			PRINT '	'+@Separator+'	d.['+@Column+']'
			SET @Separator=','
			FETCH NEXT FROM ColumnCursor INTO @Column
		END
		PRINT '	,	@_AuditEventType'
		PRINT '	,	2'
		PRINT '	,	@_AuditSessionGUID'
		PRINT '	,	@_AuditSessionTokenID'
		PRINT '	,	@AuditDatetime'
		PRINT '	,	@_AuditSiteGUID'
		PRINT '	,	a._AuditGUID'
		
		PRINT '	FROM inserted i '
		PRINT '	INNER JOIN	[fmaudit].[tblCompanies] a ON a.[CompanyGuid]= i.[CompanyGuid]' 
		PRINT '	WHERE	a._AuditEventType=''U''' 
		PRINT '	AND		a._AuditEventSequence=1 '
		PRINT '	AND		a._AuditCreatedDate= @AuditDatetime'
		
		FETCH NEXT FROM ColumnCursor INTO @Column
		WHILE @@FETCH_STATUS=0
		BEGIN
			PRINT '	'+@Separator+'	['+REPLACE(@Column,'_RowVersion','OriginalRowVersion')+']'
			SET @Separator=','
			FETCH NEXT FROM ColumnCursor INTO @Column
		END


		PRINT 'END'
		CLOSE ColumnCursor
		DEALLOCATE ColumnCursor
		
		PRINT 'GO'
		
		FETCH NEXT FROM TriggerCursor INTO @Schema,@Table
	END
	CLOSE TriggerCursor
	DEALLOCATE TriggerCursor
END
