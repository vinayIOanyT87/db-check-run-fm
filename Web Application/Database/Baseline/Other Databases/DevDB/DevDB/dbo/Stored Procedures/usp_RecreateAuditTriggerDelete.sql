


CREATE PROCEDURE [dbo].[usp_RecreateAuditTriggerDelete]
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

	PRINT ''
	PRINT '-------------------------------------'
	PRINT '-- AUDIT DELETE TRIGGERS'
	PRINT '-------------------------------------'
	PRINT ''
	
	DECLARE TriggerCursor CURSOR FOR
		SELECT Table_Schema,Table_Name
		FROM FuelsManagerDB.information_schema.tables
		WHERE Table_Schema IN('dbo','map','erv')
		AND Table_Name NOT IN ('__RefactorLog'
								,'tblAlarmAndEventLog'
                                ,'tblAuditHandler'
                                ,'tblSessions'
                                ,'tblSessionToSQLProcess'
                                ,'tblAlarmAndEventLog'
                                ,'tblAuditLog'
                                ,'tblEntityExternalAttribute'
                                ,'tblEntitySegmentTemplate'
                                ,'tblTempCompanyVersionSpecificFlag'
                                ,'tblTempEntityMappingHierarchy'
                                ,'tblTempEntityRecordVersion'
                                ,'tblTempEntityToSiteHierarchy'
                                ,'tblTempEquipmentVersionSpecificFlag'
                                ,'tblTempFieldLevelConfigMatrix'
                                ,'tblTempPersonnelVersionSpecificFlag'
                                ,'tblTempProdToCompanyForParentCompany'
                                ,'tblTempProdToCompanyForParentProduct'
                                ,'tblTempProdToTransactionAliasForParentProduct'
                                ,'tblTempProdToTransactionAliasForParentTransactionAlias'
                                ,'tblTempProductVersionSpecificFlag'
                                ,'tblTempTargetEntitySite'
                                ,'tblTempTransactionAliasVersionSpecificFlag'
                                ,'tblTempVersionSpecificField'
                                ,'tblTempProductVersionSpecificFlag'
                                ,'tblSequences'
                                ,'tblB2BResults'
                                ,'tblChangeLog'
                                ,'tblChangesQueue'
                                ,'tblImportExportConfig'
                                ,'tblExportFilters'
                                ,'tblExportPlugings'
                                ,'tblMigrationExportImportLog'
                                ,'tblFMAECompanyID'
                                ,'tblFMAEProductID'
                                ,'tblExportResults'
                                ,'tblExportResultDetails'
                                ,'tblGeneralConfigurationAliases'
                                ,'tblMessageLog'
                                ,'tblSiteAdmin'
                                ,'tblApplicationStringToDotHazardousMessage'
                                ,'tblVersion'
                                ,'tblSequences'
                                ,'tblCompanyCrossReference'
                                ,'tblCompanyCrossReferenceMap'
                                ,'tblHelpMapping'
                                ,'tblSavedQueries'
                                ,'tblSavedQueryItems'
                                ,'tblStandardImportConfig'
                                ,'tblInvoiceQueries'
                                ,'tblEnterpriseQueue'
                                ,'tblControllersLogToTransaction'
                                ,'tblBulkPaymentLinks'
                                ,'tblBulkPayments'
                                ,'tblImportExportFilters'
                                ,'tblImportExportPlugins'
                                ,'tblReportApprovals'
                                ,'tblFilterViews'
                                ,'tblTransactionLinks'
                                ,'tblExportPaiceTransTracking'
                                ,'tblExportTransportModeMapping'
								,'tblSitesShadow')
		AND table_type=  'BASE TABLE'
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
		SET @Trigger = 'trg_Audit_del_'+@Table
		
		
		PRINT 'IF EXISTS(SELECT 1 FROM sys.triggers WHERE [name]='''+@Trigger+''')'
		PRINT '	DROP TRIGGER ['+@Schema+'].['+@Trigger+'];'
		PRINT 'GO'
		
		PRINT 'CREATE TRIGGER ['+@Schema+'].['+@Trigger+'] ON ['+@Schema+'].['+@Table+'] AFTER DELETE '
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
		PRINT '	,	@AuditDatetime DATETIMEOFFSET'
		PRINT '	,	@_UserId NVARCHAR(100);' --NVARCHAR(100);' -- USER ID
		PRINT '	SET @AuditDatetime = SYSDATETIMEOFFSET();'
		PRINT '	SET @_AuditEventType= ''D''; -- For Deletes '
		PRINT '	SET @_AuditEventSequence= 1; '
		PRINT '	SELECT	@_AuditSessionGUID=s.SessionGuid '
		PRINT '		,	@_AuditSessionTokenID=s.SessionTokenID '
		PRINT '		,	@_AuditSiteGUID=s.SiteGuid'
		PRINT '		,	@_UserId=u.UserId' --USER ID
		PRINT '	FROM map.tblSessionToSQLProcess m '
		PRINT '	INNER JOIN  tblSessions s ON m.SessionGuid=s.SessionGuid '
		PRINT '	LEFT JOIN dbo.tblUsers u ON u.UserGuid=s.UserGuid ' -- USER ID
		PRINT '	WHERE m.SqlServerSessionID=@@SPID '
		PRINT '	IF @_UserId IS NULL'
		PRINT '		SET @_UserId = SUSER_NAME()' 
		PRINT '	INSERT INTO [fmaudit].'+@AuditTable+' WITH(ROWLOCK)('
		
		DECLARE ColumnCursor SCROLL CURSOR FOR
			SELECT COLUMN_NAME
			FROM FuelsManagerDB.information_schema.columns
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
		PRINT '	,	[_AuditUserId]' --USER ID
		
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
		PRINT '	,	@_UserId' -- USER ID

		
		PRINT '	FROM deleted d'
		PRINT 'END'
		CLOSE ColumnCursor
		DEALLOCATE ColumnCursor
		
		PRINT 'GO'
		
		FETCH NEXT FROM TriggerCursor INTO @Schema,@Table
	END
	CLOSE TriggerCursor
	DEALLOCATE TriggerCursor
END



