CREATE PROCEDURE [dbo].[usp_DropAllAuditTriggers]
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
	PRINT '-- DROP AUDIT TRIGGERS'
	PRINT '-------------------------------------'
	PRINT ''
	
	DECLARE TriggerCursor CURSOR FOR
		SELECT Table_Schema,Table_Name
		FROM FuelsManagerDB.information_schema.tables
		WHERE Table_Schema IN('dbo','map','erv')
		AND Table_Name IN ('tblAlarmAndEventLog','tblSessions','tblSessionToSQLProcess','tblAlarmAndEventLog','tblAuditLog','tblEntityExternalAttribute','tblEntitySegmentTemplate',
		'tblTempCompanyVersionSpecificFlag','tblTempEntityToSiteHierarchy','tblTempEquipmentVersionSpecificFlag','tblTempFieldLevelConfigMatrix','tblTempPersonnelVersionSpecificFlag',
		'tblTempTransactionAliasVersionSpecificFlag','tblTempVersionSpecificField','tblTempProductVersionSpecificFlag','tblSequences','tblB2BResults','tblChangeLog','tblChangesQueue',
		'tblImportExportConfig','tblExportFilters','tblExportPlugings','tblMigrationExportImportLog','tblFMAECompanyID','tblFMAEProductID','tblExportResults','tblExportResultDetails',
		'tblGeneralConfigurationAliases','tblMessageLog','tblSiteAdmin','tblApplicationStringToDotHazardousMessage','tblVersion','tblSequences','tblCompanyCrossReference','tblCompanyCrossReferenceMap',
		'tblHelpMapping','tblSavedQueries','tblSavedQueryItems','tblStandardImportConfig','tblInvoiceQueries','tblEnterpriseQueue','tblControllersLogToTransaction','tblBulkPaymentLinks',
		'tblBulkPayments','tblImportExportFilters','tblImportExportPlugins','tblReportApprovals','tblFilterViews','tblTransactionLinks','tblExportPaiceTransTracking','tblExportTransportModeMapping')
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

		SET @Trigger = 'trg_Audit_ins_'+@Table
		
		
		PRINT 'IF EXISTS(SELECT 1 FROM sys.triggers WHERE [name]='''+@Trigger+''')'
		PRINT '	DROP TRIGGER ['+@Schema+'].['+@Trigger+'];'
		PRINT 'GO'

		SET @Trigger = 'trg_Audit_upd_'+@Table
		
		
		PRINT 'IF EXISTS(SELECT 1 FROM sys.triggers WHERE [name]='''+@Trigger+''')'
		PRINT '	DROP TRIGGER ['+@Schema+'].['+@Trigger+'];'
		PRINT 'GO'
		
		FETCH NEXT FROM TriggerCursor INTO @Schema,@Table
	END
	CLOSE TriggerCursor
	DEALLOCATE TriggerCursor
END
