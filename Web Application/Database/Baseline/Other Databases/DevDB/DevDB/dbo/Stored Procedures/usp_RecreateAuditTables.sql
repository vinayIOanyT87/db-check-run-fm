CREATE PROCEDURE [dbo].[usp_RecreateAuditTables]
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

	PRINT 'IF EXISTS(SELECT 1 FROM sys.triggers WHERE NAME = ''TRDDL_DROP_TABLE'')'
	PRINT '	DISABLE TRIGGER TRDDL_DROP_TABLE ON DATABASE'
	PRINT 'GO'

	DECLARE TableCursor CURSOR FOR
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
                                ,'tblExportTransportModeMapping')
		AND table_type=  'BASE TABLE'
		ORDER BY Table_Schema,Table_Name

	OPEN TableCursor
	FETCH NEXT FROM TableCursor INTO @Schema,@Table
	WHILE @@FETCH_STATUS=0
	BEGIN
		
		IF @Schema='map'
			SET @AuditTable='map_'+@Table
		ELSE IF @Schema='erv'
			SET @AuditTable='erv_'+@Table
		ELSE
			SET @AuditTable=@Table
			
		PRINT 'IF EXISTS(SELECT 1 FROM information_schema.tables WHERE Table_Schema=''fmaudit'' AND Table_Name='''+@AuditTable+''') '
		PRINT '	DROP TABLE [fmaudit].['+@AuditTable+']'
		PRINT 'GO'
		PRINT 'CREATE TABLE [fmaudit].['+@AuditTable+']('	
		SET @Separator=''
		DECLARE ColumnCursor CURSOR FOR
			SELECT Column_Name,Data_Type,Character_Maximum_Length
			FROM FuelsManagerDB.information_schema.columns
			WHERE Table_Schema = @Schema
			AND	 Table_Name = @Table
			ORDER BY Table_Schema,Table_Name,Ordinal_Position
		
		OPEN ColumnCursor
		FETCH NEXT FROM ColumnCursor INTO @Column,@DataType,@Maxlength
		WHILE @@FETCH_STATUS=0
		BEGIN
			
			IF @Column <> '_RowVersion' 
			BEGIN
				IF @Maxlength = -1 AND @DataType NOT IN('xml') 
				BEGIN
					PRINT @Separator+'	['+@Column+'] '+@DataType+ISNULL(' (max)','') +' NULL'
				END
				ELSE
				BEGIN
					IF @DataType NOT IN('xml','image','text')
					BEGIN
						PRINT @Separator+'	['+@Column+'] '+@DataType+ISNULL(' ('+ CAST(@Maxlength AS VARCHAR(50))+')','') +' NULL'
					END
					ELSE
					BEGIN
						PRINT @Separator+'	['+@Column+'] '+@DataType + ' NULL'
					END
				END
			END
			ELSE
			BEGIN
				PRINT @Separator+'	[OriginalRowVersion] binary(8) NULL'
			END 		
			SET @Separator=','
			FETCH NEXT FROM ColumnCursor INTO @Column,@DataType,@Maxlength
		END
		-- COLUMNS FOR AUDIT
		PRINT @Separator+'	[_AuditEventType] char(1) NULL'
		PRINT @Separator+'	[_AuditEventSequence] tinyint NULL CONSTRAINT DF_'+@AuditTable+'_AuditEventSequence DEFAULT 0'
		PRINT @Separator+'	[_AuditSiteGuid] uniqueidentifier NULL'
		PRINT @Separator+'	[_AuditSessionGuid] uniqueidentifier NULL'
		PRINT @Separator+'	[_AuditUserID] udtUserID NULL'
		PRINT @Separator+'	[_AuditSessionTokenID] uniqueidentifier NULL'
		PRINT @Separator+'	[_AuditCreatedDate] datetimeoffset(7) NULL CONSTRAINT DF_'+@AuditTable+'_AuditCreatedDate DEFAULT sysdatetimeoffset()'
		PRINT @Separator+'	[_AuditGUID] uniqueidentifier NOT NULL CONSTRAINT DF_'+@AuditTable+'_AuditGUID DEFAULT newid()'
		PRINT @Separator+'	[_AuditRowVersion] ROWVERSION '
		-- PRINT @Separator+'	[_ConjoinAuditGUID] uniqueidentifier NULL'
		PRINT ')'
		PRINT 'GO'
		--PRINT 'ALTER TABLE [fmaudit].['+@AuditTable+'] ADD CONSTRAINT [PK_'+@AuditTable+'] PRIMARY KEY NONCLUSTERED([_AuditGUID] ASC) '
		--PRINT 'GO'
		PRINT 'CREATE CLUSTERED INDEX [IX_'+@AuditTable+'_AuditCreatedDate] ON [fmaudit].['+@AuditTable+'](_AuditCreatedDate ASC) '
		PRINT 'GO'
		PRINT 'CREATE NONCLUSTERED INDEX [IX_'+@AuditTable+'_AuditGUID] ON [fmaudit].['+@AuditTable+'](_AuditGUID ASC) '
		PRINT 'GO'

		CLOSE ColumnCursor
		DEALLOCATE ColumnCursor
		
		FETCH NEXT FROM TableCursor INTO @Schema,@Table
	END
	CLOSE TableCursor
	DEALLOCATE TableCursor

	PRINT 'IF EXISTS(SELECT 1 FROM sys.triggers WHERE NAME = ''TRDDL_DROP_TABLE'')'
	PRINT '	ENABLE TRIGGER TRDDL_DROP_TABLE ON DATABASE'
	PRINT 'GO'

END

