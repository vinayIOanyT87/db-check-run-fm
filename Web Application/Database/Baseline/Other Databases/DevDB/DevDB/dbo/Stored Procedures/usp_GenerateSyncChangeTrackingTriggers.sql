
CREATE PROCEDURE [dbo].[usp_GenerateSyncChangeTrackingTriggers](@GenerateInsertUpdateTrigger bit=1, @GenerateDeleteTrigger bit=1, @GenerateDropStatements bit=0)
AS
BEGIN
    DECLARE @Schema nvarchar(300)
		    ,@Table nvarchar(500)
		    ,@LastSchema nvarchar(300)
		    ,@LastTable nvarchar(500)
		    ,@Column nvarchar(500)
		    ,@Type nvarchar(500)
		    ,@Default nvarchar(2000)
		    ,@Nullable varchar(50)
		    ,@MaxLength int
		    ,@Precision int
		    ,@PrecisionRadix int
		    ,@IsPKColumn bit
		    ,@DropInsUpdTrigger nvarchar(4000)
		    ,@DropDeleteTrigger nvarchar(4000)
		    ,@SqlPKSelectForInsUpd nvarchar(max)
		    ,@SqlPKSelectForDelete nvarchar(max)
		    ,@SqlPKColumns nvarchar(4000)
		    ,@SqlPKData nvarchar(4000)
		    ,@SqlPKDeletedData nvarchar(4000)
            ,@SqlForeignKeyColumnName nvarchar(2000)
		    ,@SqlFKSelectForInsUpd nvarchar(max)
		    ,@SqlFKSelectForDelete nvarchar(max)
		    ,@SqlFKColumns nvarchar(4000)
		    ,@SqlFKData nvarchar(4000)
		    ,@SqlFKDeletedData nvarchar(4000)
            ,@LastSqlForeignKeyColumnName nvarchar(2000)
		    ,@SqlInsertDeleteJoin nvarchar(4000)
		    ,@SqlChangeListJoin nvarchar(4000)
		    ,@SqlDeleteListJoin nvarchar(4000)
		    ,@SqlInsertUpdate nvarchar(MAX)
		    ,@SqlDelete nvarchar(MAX)
		    ,@ProcessStartTime datetime2
		    ,@StepStartTime datetime2
		    ,@StepEndTime datetime2
		    ,@AffectedRecords int
		    ,@SiteGuidColumnOverride nvarchar(256)
		    ,@HasSiteGuid bit
		    ,@HasCreatedDate bit
		    ,@HasUpdatedDate bit

    DECLARE @PrimaryKeyColumnList AS TABLE
    (
	    SchemaName nvarchar(200)
	    ,TableName nvarchar(512)
	    ,PKColumnName nvarchar(384)
    )

    DECLARE @SyncSinglePass AS TABLE
    (
	    SchemaName nvarchar(200)
	    ,TableName nvarchar(512)
	    ,FKColumnName nvarchar(384)
    )

    SET @AffectedRecords = 0
    SET @ProcessStartTime = GETDATE()
    SET @LastSchema = NULL
    SET @LastTable = NULL
    SET @LastSqlForeignKeyColumnName = NULL

    --PRINT '*** Generating Sync Change Tracking Triggers ***'
    --PRINT '*** Process started on '+CAST(GETDATE() AS nvarchar(50))
    --PRINT '*** Database: ' + DB_NAME();
    --PRINT '*** Checking database schema ***'
    IF ((SELECT COUNT(DISTINCT(tab.TABLE_SCHEMA)) FROM FuelsManagerDB.INFORMATION_SCHEMA.TABLES tab WHERE tab.TABLE_SCHEMA IN ('fmaudit', 'sync', 'track', 'dbo', 'lookup', 'map', 'erv')) < 7)
    BEGIN
	    PRINT '*** WARNING: Database does not appear to contain the FuelsManager schemas ***'
	    RETURN;
    END

    --PRINT 'Identifying primary keys...'
    SET NOCOUNT ON
    INSERT INTO @PrimaryKeyColumnList
	    SELECT DISTINCT tab.TABLE_SCHEMA AS SchemaName
					    ,tab.TABLE_NAME AS TableName
					    ,icl.COLUMN_NAME AS PKColumnName
		    FROM FuelsManagerDB.INFORMATION_SCHEMA.TABLES tab
				    LEFT JOIN FuelsManagerDB.INFORMATION_SCHEMA.TABLE_CONSTRAINTS cons ON (cons.CONSTRAINT_CATALOG=tab.TABLE_CATALOG AND cons.TABLE_SCHEMA = tab.TABLE_SCHEMA AND cons.TABLE_NAME=tab.TABLE_NAME)
				    LEFT JOIN FuelsManagerDB.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE icl ON (icl.CONSTRAINT_CATALOG=cons.CONSTRAINT_CATALOG AND icl.CONSTRAINT_NAME=cons.CONSTRAINT_NAME)
		    WHERE cons.CONSTRAINT_TYPE='PRIMARY KEY'
			    AND tab.TABLE_SCHEMA IN ('dbo', 'lookup', 'map', 'erv')
		    ORDER BY tab.TABLE_SCHEMA,tab.TABLE_NAME,icl.COLUMN_NAME
    SET NOCOUNT OFF

    --PRINT 'Processing table metadata...'
    DECLARE @TablesProcessed int
    SET @TablesProcessed = 0;

    DECLARE TableInfoCursor CURSOR FOR 
	    SELECT	tables.SchemaName
			    ,tables.TableName
			    ,s.COLUMN_NAME
			    ,s.DATA_TYPE
			    ,s.COLUMN_DEFAULT
			    ,s.IS_NULLABLE
			    ,s.CHARACTER_MAXIMUM_LENGTH
			    ,s.NUMERIC_PRECISION
			    ,s.NUMERIC_PRECISION_RADIX
			    ,CASE WHEN pk.PKColumnName IS NULL THEN 0 ELSE 1 END AS 'IsPrimaryKey'
                ,tables.ParentForeignKeyColumnName AS 'ParentForeignKeyColumnName'
	    FROM FuelsManagerDB.INFORMATION_SCHEMA.COLUMNS s
		    INNER JOIN (SELECT PARSENAME(st.TableName, 2) 'SchemaName', PARSENAME(st.TableName, 1) 'TableName', st.ParentSyncTableGuid, st.ParentForeignKeyColumnName
						    FROM FuelsManagerDB.sync.tblSyncTable st) tables
			    ON tables.[SchemaName] = s.TABLE_SCHEMA AND tables.[TableName] = s.TABLE_NAME
		    LEFT OUTER JOIN (SELECT SchemaName, TableName, PKColumnName FROM @PrimaryKeyColumnList) pk
			    ON s.TABLE_SCHEMA = pk.[SchemaName] AND s.TABLE_NAME = pk.[TableName] AND s.COLUMN_NAME = pk.[PKColumnName]
	    ORDER BY s.TABLE_SCHEMA
			    ,s.TABLE_NAME
			    ,s.ORDINAL_POSITION

    OPEN TableInfoCursor
    FETCH NEXT FROM TableInfoCursor INTO 
	    @Schema,@Table,@Column,@Type,@Default,@Nullable,@MaxLength,@Precision,@PrecisionRadix,@IsPKColumn,@SqlForeignKeyColumnName

    WHILE @@FETCH_STATUS = 0
    BEGIN
	    SET @StepStartTime = GETDATE()

	    IF (@LastTable IS NULL)
	    BEGIN
		    SET @TablesProcessed+=1;

		    PRINT ''
		    --PRINT '***>>> Step started at '+CAST(@StepStartTime AS nvarchar(50))

		    SET @LastSchema = @Schema
		    SET @LastTable = @Table;
            SET @LastSqlForeignKeyColumnName = @SqlForeignKeyColumnName
		    SET @SiteGuidColumnOverride = N'SiteGuid';
		    SET @HasSiteGuid = 0;
		    SET @HasCreatedDate = 0;
		    SET @HasUpdatedDate = 0;

		    SET @SqlPKSelectForInsUpd = N'';
		    SET @SqlPKSelectForDelete = N'';
		    SET @SqlPKColumns = N'';
		    SET @SqlPKData = N'';
		    SET @SqlPKDeletedData = N'';
		    SET @SqlInsertDeleteJoin = N'';
		    SET @SqlChangeListJoin = N'';
		    SET @SqlDeleteListJoin = N'';

		    SET @SqlFKSelectForInsUpd = N'';
		    SET @SqlFKSelectForDelete = N'';
		    SET @SqlFKColumns = N'';
		    SET @SqlFKData = N'';
		    SET @SqlFKDeletedData = N'';
 	    END
	    ELSE IF (@LastTable <> @Table)
	    BEGIN
		    SET @TablesProcessed+=1;

		    SET @SqlFKSelectForInsUpd+= ',d.' + @LastSqlForeignKeyColumnName + ' AS Deleted_FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET @SqlFKSelectForInsUpd+= '                    ,i.' + @LastSqlForeignKeyColumnName + ' AS Inserted_FK_ParentPK' + CHAR(13) + CHAR(10);

		    SET @SqlFKSelectForDelete+= ',d.' + @LastSqlForeignKeyColumnName + ' AS Deleted_FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET @SqlFKSelectForDelete+= '                        ,d.' + @LastSqlForeignKeyColumnName + ' AS Inserted_FK_ParentPK' + CHAR(13) + CHAR(10);

		    SET @SqlFKColumns+= ',FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET	@SqlFKData+= ',src.Inserted_FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET @SqlFKDeletedData+= ',src.Deleted_FK_ParentPK' + CHAR(13) + CHAR(10);

		    SET @DropInsUpdTrigger = 'IF EXISTS (SELECT name FROM sysobjects WHERE name = ''trg_insupd_'+ @LastTable + '_ForSync'' AND type = ''TR'')' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= 'BEGIN' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= '    DROP TRIGGER ' + @LastSchema + '.trg_insupd_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= 'END' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@DropInsUpdTrigger
		    IF ((@GenerateInsertUpdateTrigger = 1 AND @GenerateDropStatements = 1) OR
                (@GenerateInsertUpdateTrigger = 0 AND @GenerateDeleteTrigger = 0 AND @GenerateDropStatements = 1))
            BEGIN
		    PRINT '--Dropping Insert / Update Trigger for ' + @LastTable
            PRINT @DropInsUpdTrigger
            END
		
		    SET @SqlInsertUpdate= 'CREATE TRIGGER ' + @LastSchema + '.trg_insupd_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   ON ' + @LastSchema + '.' + @LastTable + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   AFTER INSERT, UPDATE ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'AS ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- SET NOCOUNT ON added to prevent extra result sets from ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- interfering with SELECT statements.' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	SET NOCOUNT ON; ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @changeContextName nvarchar(100); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @bypassTrackingFlags int; ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @bypassReason nvarchar(512); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    SELECT @changeContextName = ContextName ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '            ,@bypassTrackingFlags = BypassTrackingFlags ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '            ,@bypassReason = BypassReason '  + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '        FROM [track].[udf_GetChangeTrackingSessionDetails](); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- Get the synchronization context.  This will be NULL if this trigger was fired' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- due to a normal application insert or update.' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @syncContext varbinary(128); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @currentDateTimeOffset datetimeoffset(7); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    SET @currentDateTimeOffset = sysdatetimeoffset(); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   IF (([track].[udf_IsInsertChangeTrackingEnabled](@bypassTrackingFlags) = 1) OR ([track].[udf_IsUpdateChangeTrackingEnabled](@bypassTrackingFlags) = 1))' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       SET @syncContext = dbo.udf_GetSyncContext(); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       -- Treat the change as a local change so it can be synchronized back to the remote system. ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       IF ((SELECT trigger_nestlevel()) > 1) ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '           SET @syncContext = NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       END ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       ; WITH ChangeList AS ( ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       SELECT @syncContext AS ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '                   ' + @SqlPKSelectForInsUpd
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlInsertUpdate+= '                   ' + @SqlFKSelectForInsUpd
            END
		    IF (@HasCreatedDate = 1)
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,i.CreatedDate AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,@currentDateTimeOffset AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasUpdatedDate = 1)
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,i.UpdatedDate AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,@currentDateTimeOffset AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasSiteGuid = 1)
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,i.' + @SiteGuidColumnOverride + ' AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '                   ,d.' + @SiteGuidColumnOverride + ' AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlInsertUpdate+= '				    ,NULL AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ,NULL AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    SET @SqlInsertUpdate+= '				    ,i._RowVersion AS Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ,NULL AS Deleted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    FROM Inserted i ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    FULL OUTER JOIN Deleted d ON ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ' + @SqlInsertDeleteJoin
		    SET @SqlInsertUpdate+= '           ) ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    MERGE INTO track.' + @LastTable + ' As ct ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    USING ChangeList As src ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ON ' + @SqlChangeListJoin
		    SET @SqlInsertUpdate+= '           WHEN Matched AND ((src.CurrentSiteGuid IS NULL AND ct.CurrentSiteGuid IS NULL)' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '                        OR ((src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NOT NULL) AND (src.CurrentSiteGuid = ct.CurrentSiteGuid))) ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    UPDATE SET UpdatedDate = src.Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    		,UpdatedContext = src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				        ,UpdatedRowVersion = src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '     					,CurrentSiteGuid = src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 	    				,PreviousSiteGuid = ct.PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    WHEN Not Matched ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    INSERT (InsertedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 			    	,InsertedContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,InsertedRowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,UpdatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,UpdatedContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,UpdatedRowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,DeletedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,DeletedContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,DeletedRowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ' + @SqlPKColumns
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlInsertUpdate+= '				    ' + @SqlFKColumns
            END
		    SET @SqlInsertUpdate+= '		    )' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    VALUES (CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		                 WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		                 ELSE CAST(''1/1/1990'' AS DateTimeOffset(7)) END ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    	,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ,src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    				,src.Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	    			,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    		,src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    	,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    				,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	    			,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    		,src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    	,CASE WHEN (((src.PreviousSiteGuid IS NOT NULL AND src.CurrentSiteGuid IS NOT NULL) AND (src.PreviousSiteGuid <> src.CurrentSiteGuid))' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    				OR (src.PreviousSiteGuid IS NULL AND src.CurrentSiteGuid IS NOT NULL)' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 					    			OR (src.PreviousSiteGuid IS NOT NULL AND src.CurrentSiteGuid IS NULL)) THEN src.PreviousSiteGuid ELSE NULL END' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ' + @SqlPKData
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlInsertUpdate+= '					' + @SqlFKData
            END
		    SET @SqlInsertUpdate+= '		    );' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    END' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'END ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@SqlInsertUpdate
	        IF (@GenerateInsertUpdateTrigger = 1)
            BEGIN
		    PRINT '--Creating Insert / Update Trigger for ' + @LastTable
            EXEC [DevDB].[dbo].[LongPrint] @SqlInsertUpdate
            --PRINT @SqlInsertUpdate
            END

		    SET @DropDeleteTrigger = 'IF EXISTS (SELECT name FROM sysobjects WHERE name = ''trg_del_'+ @LastTable + '_ForSync'' AND type = ''TR'')' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= 'BEGIN' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= '    DROP TRIGGER ' + @LastSchema + '.trg_del_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= 'END' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@DropDeleteTrigger
		    IF ((@GenerateDeleteTrigger = 1 AND @GenerateDropStatements = 1) OR
                (@GenerateInsertUpdateTrigger = 0 AND @GenerateDeleteTrigger = 0 AND @GenerateDropStatements = 1))
            BEGIN
		    PRINT '--Dropping Delete Trigger for ' + @LastTable
            PRINT @DropDeleteTrigger
            END

		    SET @SqlDelete= 'CREATE TRIGGER ' + @LastSchema + '.trg_del_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '   ON ' + @LastSchema + '.' + @LastTable + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '   AFTER DELETE ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'AS ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- SET NOCOUNT ON added to prevent extra result sets from ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- interfering with SELECT statements.' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	SET NOCOUNT ON; ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @changeContextName nvarchar(100); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @bypassTrackingFlags int; ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @bypassReason nvarchar(512); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    SELECT @changeContextName = ContextName ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '            ,@bypassTrackingFlags = BypassTrackingFlags ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '            ,@bypassReason = BypassReason '  + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '        FROM [track].[udf_GetChangeTrackingSessionDetails](); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- Get the synchronization context.  This will be NULL if this trigger was fired' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- due to a normal application delete.' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @syncContext varbinary(128); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @currentDateTimeOffset datetimeoffset(7); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    SET @currentDateTimeOffset = sysdatetimeoffset(); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    IF ([track].[udf_IsDeleteChangeTrackingEnabled](@bypassTrackingFlags) = 1)' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    BEGIN' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       SET @syncContext = dbo.udf_GetSyncContext(); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       -- Treat the change as a local change so it can be synchronized back to the remote system. ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       IF ((SELECT trigger_nestlevel()) > 1) ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '           SET @syncContext = NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       END ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '		  ; WITH ChangeList AS ( ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				SELECT @syncContext AS ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						' + @SqlPKSelectForDelete
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlDelete+= '                   ' + @SqlFKSelectForDelete
            END
		    IF (@HasCreatedDate = 1)
		    BEGIN
		    SET @SqlDelete+= '						,d.CreatedDate AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlDelete+= '						,@currentDateTimeOffset AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasUpdatedDate = 1)
		    BEGIN
		    SET @SqlDelete+= '						,d.UpdatedDate AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlDelete+= '						,@currentDateTimeOffset AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasSiteGuid = 1)
		    BEGIN
		    SET @SqlDelete+= '						,d.' + @SiteGuidColumnOverride + ' AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlDelete+= '						,NULL AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    SET @SqlDelete+= '						,d._RowVersion AS Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '					FROM Deleted d ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				) ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				MERGE INTO track.' + @LastTable + ' As ct ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '					USING ChangeList As src ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						ON ' + @SqlDeleteListJoin
		    SET @SqlDelete+= '				WHEN Matched ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '					UPDATE SET DeletedDate = @currentDateTimeOffset ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '								,DeletedContext = src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 								,DeletedRowVersion = src.Deleted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 								,CurrentSiteGuid = src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 								,PreviousSiteGuid = CASE WHEN (((src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NOT NULL) AND (src.CurrentSiteGuid <> ct.CurrentSiteGuid))' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 																OR (src.CurrentSiteGuid IS NULL AND ct.CurrentSiteGuid IS NOT NULL)' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 																OR (src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NULL)) THEN ct.CurrentSiteGuid ELSE ct.PreviousSiteGuid END' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				WHEN Not Matched ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				INSERT (InsertedDate' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,InsertedContext' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,InsertedRowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,UpdatedDate' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,UpdatedContext' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,UpdatedRowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,CurrentSiteGuid' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,PreviousSiteGuid' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,DeletedDate' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,DeletedContext' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,DeletedRowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						' + @SqlPKColumns
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlDelete+= '						' + @SqlFKColumns
            END
		    SET @SqlDelete+= '				)' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				VALUES (CASE WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ELSE CAST(''1/1/1990'' AS DateTimeOffset(7)) END ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,@currentDateTimeOffset ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.Deleted_RowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						' + @SqlPKDeletedData
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlDelete+= '						' + @SqlFKDeletedData
            END
		    SET @SqlDelete+= '				);' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    END' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'END ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@SqlDelete
	        IF (@GenerateDeleteTrigger = 1)
            BEGIN
		    PRINT '--Creating Delete Trigger for ' + @LastTable
            EXEC [DevDB].[dbo].[LongPrint] @SqlDelete
		    --PRINT @SqlDelete
            END

		    SET @SiteGuidColumnOverride = 'SiteGuid'
		    SET @HasSiteGuid = 0;
		    SET @HasCreatedDate = 0;
		    SET @HasUpdatedDate = 0;

		    SET @SqlPKSelectForInsUpd = N'';
		    SET @SqlPKSelectForDelete = N'';
		    SET @SqlPKColumns = N'';
		    SET @SqlPKData = N'';
		    SET @SqlPKDeletedData = N'';
		    SET @SqlInsertDeleteJoin = N'';
		    SET @SqlChangeListJoin = N'';
		    SET @SqlDeleteListJoin = N'';

		    SET @SqlFKSelectForInsUpd = N'';
		    SET @SqlFKSelectForDelete = N'';
		    SET @SqlFKColumns = N'';
		    SET @SqlFKData = N'';
		    SET @SqlFKDeletedData = N'';

		    SET @StepEndTime=GETDATE()
		    --PRINT '***>>> Step finished on '+CAST(@StepEndTime AS nvarchar(50))
		    --PRINT '***>>> Affected Records: '+CAST (@@ROWCOUNT AS nvarchar(50))
		    --PRINT '***>>> Step elapse time in seconds: '+ CAST(DATEDIFF(ss,@StepStartTime,@StepEndTime) AS nvarchar(50))
		    --PRINT '***>>> Step elapse time in minutes: '+ CAST(DATEDIFF(mi,@StepStartTime,@StepEndTime) AS nvarchar(50))
		    SET @AffectedRecords += @@ROWCOUNT

		    PRINT ''
		    --PRINT '***>>> Step started at '+CAST(@StepStartTime AS nvarchar(50))

		    SET @LastSchema = @Schema
		    SET @LastTable = @Table
            SET @LastSqlForeignKeyColumnName = @SqlForeignKeyColumnName
	    END

	    IF (@Column = 'OwnerSiteGuid')
	    BEGIN
		    SET @HasSiteGuid = 1;
		    SET @SiteGuidColumnOverride = 'OwnerSiteGuid';
	    END
	    IF (@Column = 'MapToSiteGuid')
	    BEGIN
		    SET @HasSiteGuid = 1;
		    SET @SiteGuidColumnOverride = 'MapToSiteGuid';
	    END
	    IF (@Column = 'SiteGroupGuid')
	    BEGIN
		    SET @HasSiteGuid = 1;
		    SET @SiteGuidColumnOverride = 'SiteGroupGuid';
	    END
	    IF (@Column = 'SiteGuid')
	    BEGIN
		    -- Special case, since the primary key is the SiteGuid column, it won't change so we don't need to worry about Site Ownership
		    IF (@Schema = 'dbo' AND @Table = 'tblSites')
		    BEGIN
			    SET @HasSiteGuid = 0;
			    SET @SiteGuidColumnOverride = '';
		    END
		    ELSE
		    BEGIN
			    SET @HasSiteGuid = 1;
			    SET @SiteGuidColumnOverride = 'SiteGuid';
		    END
	    END

	    IF (@Column = 'CreatedDate') SET @HasCreatedDate = 1;
	    IF (@Column = 'UpdatedDate') SET @HasUpdatedDate = 1;

	    IF (@IsPKColumn = 1)
	    BEGIN
		    SET @SqlPKSelectForInsUpd+= ',d.' + @Column + ' AS Deleted_PK_' + @Column + CHAR(13) + CHAR(10);
		    SET @SqlPKSelectForInsUpd+= '                    ,i.' + @Column + ' AS Inserted_PK_' + @Column  + CHAR(13) + CHAR(10);

		    SET @SqlPKSelectForDelete+= ',d.' + @Column + ' AS Deleted_PK_' + @Column + CHAR(13) + CHAR(10);
		    SET @SqlPKSelectForDelete+= '                        ,d.' + @Column + ' AS Inserted_PK_' + @Column  + CHAR(13) + CHAR(10);

		    IF (LEN(@SqlInsertDeleteJoin) > 0) 
		    BEGIN
			    SET @SqlInsertDeleteJoin+= ' AND '
		    END

		    SET @SqlInsertDeleteJoin+= 'd.' + @Column + ' = i.' + @Column + CHAR(13) + CHAR(10);

		    IF (LEN(@SqlChangeListJoin) > 0) 
		    BEGIN
			    SET @SqlChangeListJoin+= ' AND '
		    END

		    SET @SqlChangeListJoin+= 'src.Inserted_PK_' + @Column + ' = ct.PK_' + @Column + CHAR(13) + CHAR(10);

		    IF (LEN(@SqlDeleteListJoin) > 0) 
		    BEGIN
			    SET @SqlDeleteListJoin+= ' AND '
		    END

		    SET @SqlDeleteListJoin+= 'src.Deleted_PK_' + @Column + ' = ct.PK_' + @Column + CHAR(13) + CHAR(10);

		    SET @SqlPKColumns+= ',PK_' + @Column + CHAR(13) + CHAR(10);
		    SET	@SqlPKData+= ',src.Inserted_PK_' + @Column + CHAR(13) + CHAR(10);
		    SET @SqlPKDeletedData+= ',src.Deleted_PK_' + @Column + CHAR(13) + CHAR(10);
	    END

	    FETCH NEXT FROM TableInfoCursor INTO
		    @Schema,@Table,@Column,@Type,@Default,@Nullable,@MaxLength,@Precision,@PrecisionRadix,@IsPKColumn,@SqlForeignKeyColumnName

        -- Handle Last Record
        IF (@@FETCH_STATUS <> 0)
        BEGIN
		    SET @TablesProcessed+=1;

		    SET @SqlFKSelectForInsUpd+= ',d.' + @LastSqlForeignKeyColumnName + ' AS Deleted_FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET @SqlFKSelectForInsUpd+= '                    ,i.' + @LastSqlForeignKeyColumnName + ' AS Inserted_FK_ParentPK' + CHAR(13) + CHAR(10);

		    SET @SqlFKSelectForDelete+= ',d.' + @LastSqlForeignKeyColumnName + ' AS Deleted_FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET @SqlFKSelectForDelete+= '                        ,d.' + @LastSqlForeignKeyColumnName + ' AS Inserted_FK_ParentPK' + CHAR(13) + CHAR(10);

		    SET @SqlFKColumns+= ',FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET	@SqlFKData+= ',src.Inserted_FK_ParentPK' + CHAR(13) + CHAR(10);
		    SET @SqlFKDeletedData+= ',src.Deleted_FK_ParentPK' + CHAR(13) + CHAR(10);

		    SET @DropInsUpdTrigger = 'IF EXISTS (SELECT name FROM sysobjects WHERE name = ''trg_insupd_'+ @LastTable + '_ForSync'' AND type = ''TR'')' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= 'BEGIN' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= '    DROP TRIGGER ' + @LastSchema + '.trg_insupd_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= 'END' + CHAR(13) + CHAR(10);
		    SET @DropInsUpdTrigger+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@DropInsUpdTrigger
		    IF ((@GenerateInsertUpdateTrigger = 1 AND @GenerateDropStatements = 1) OR
                (@GenerateInsertUpdateTrigger = 0 AND @GenerateDeleteTrigger = 0 AND @GenerateDropStatements = 1))
            BEGIN
		    PRINT '--Dropping Insert / Update Trigger for ' + @LastTable
            PRINT @DropInsUpdTrigger
            END
		
		    SET @SqlInsertUpdate= 'CREATE TRIGGER ' + @LastSchema + '.trg_insupd_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   ON ' + @LastSchema + '.' + @LastTable + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   AFTER INSERT, UPDATE ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'AS ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- SET NOCOUNT ON added to prevent extra result sets from ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- interfering with SELECT statements.' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	SET NOCOUNT ON; ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @changeContextName nvarchar(100); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @bypassTrackingFlags int; ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @bypassReason nvarchar(512); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    SELECT @changeContextName = ContextName ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '            ,@bypassTrackingFlags = BypassTrackingFlags ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '            ,@bypassReason = BypassReason '  + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '        FROM [track].[udf_GetChangeTrackingSessionDetails](); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- Get the synchronization context.  This will be NULL if this trigger was fired' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	-- due to a normal application insert or update.' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @syncContext varbinary(128); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    DECLARE @currentDateTimeOffset datetimeoffset(7); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    SET @currentDateTimeOffset = sysdatetimeoffset(); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   IF (([track].[udf_IsInsertChangeTrackingEnabled](@bypassTrackingFlags) = 1) OR ([track].[udf_IsUpdateChangeTrackingEnabled](@bypassTrackingFlags) = 1))' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '   BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       SET @syncContext = dbo.udf_GetSyncContext(); ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       -- Treat the change as a local change so it can be synchronized back to the remote system. ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       IF ((SELECT trigger_nestlevel()) > 1) ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '           SET @syncContext = NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       END ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       ; WITH ChangeList AS ( ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '       SELECT @syncContext AS ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '                   ' + @SqlPKSelectForInsUpd
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlInsertUpdate+= '                   ' + @SqlFKSelectForInsUpd
            END
		    IF (@HasCreatedDate = 1)
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,i.CreatedDate AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,@currentDateTimeOffset AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasUpdatedDate = 1)
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,i.UpdatedDate AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,@currentDateTimeOffset AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasSiteGuid = 1)
		    BEGIN
		    SET @SqlInsertUpdate+= '                   ,i.' + @SiteGuidColumnOverride + ' AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '                   ,d.' + @SiteGuidColumnOverride + ' AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlInsertUpdate+= '				    ,NULL AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ,NULL AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    SET @SqlInsertUpdate+= '				    ,i._RowVersion AS Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ,NULL AS Deleted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    FROM Inserted i ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    FULL OUTER JOIN Deleted d ON ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ' + @SqlInsertDeleteJoin
		    SET @SqlInsertUpdate+= '           ) ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    MERGE INTO track.' + @LastTable + ' As ct ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    USING ChangeList As src ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ON ' + @SqlChangeListJoin
		    SET @SqlInsertUpdate+= '           WHEN Matched AND ((src.CurrentSiteGuid IS NULL AND ct.CurrentSiteGuid IS NULL)' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '                        OR ((src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NOT NULL) AND (src.CurrentSiteGuid = ct.CurrentSiteGuid))) ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    UPDATE SET UpdatedDate = src.Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    		,UpdatedContext = src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				        ,UpdatedRowVersion = src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '     					,CurrentSiteGuid = src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 	    				,PreviousSiteGuid = ct.PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    WHEN Not Matched ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    INSERT (InsertedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 			    	,InsertedContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,InsertedRowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,UpdatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,UpdatedContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,UpdatedRowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,DeletedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,DeletedContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,DeletedRowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    ,PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ' + @SqlPKColumns
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlInsertUpdate+= '				    ' + @SqlFKColumns
            END
		    SET @SqlInsertUpdate+= '		    )' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    VALUES (CASE WHEN (@syncContext IS NOT NULL) THEN @currentDateTimeOffset ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		                 WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		                 ELSE CAST(''1/1/1990'' AS DateTimeOffset(7)) END ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    	,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ,src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    				,src.Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	    			,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    		,src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    	,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    				,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '	    			,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '		    		,src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '			    	,CASE WHEN (((src.PreviousSiteGuid IS NOT NULL AND src.CurrentSiteGuid IS NOT NULL) AND (src.PreviousSiteGuid <> src.CurrentSiteGuid))' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 				    				OR (src.PreviousSiteGuid IS NULL AND src.CurrentSiteGuid IS NOT NULL)' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= ' 					    			OR (src.PreviousSiteGuid IS NOT NULL AND src.CurrentSiteGuid IS NULL)) THEN src.PreviousSiteGuid ELSE NULL END' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '				    ' + @SqlPKData
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlInsertUpdate+= '					' + @SqlFKData
            END
		    SET @SqlInsertUpdate+= '		    );' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= '    END' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'END ' + CHAR(13) + CHAR(10);
		    SET @SqlInsertUpdate+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@SqlInsertUpdate
	        IF (@GenerateInsertUpdateTrigger = 1)
            BEGIN
		    PRINT '--Creating Insert / Update Trigger for ' + @LastTable
            EXEC [DevDB].[dbo].[LongPrint] @SqlInsertUpdate
            --PRINT @SqlInsertUpdate
            END

		    SET @DropDeleteTrigger = 'IF EXISTS (SELECT name FROM sysobjects WHERE name = ''trg_del_'+ @LastTable + '_ForSync'' AND type = ''TR'')' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= 'BEGIN' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= '    DROP TRIGGER ' + @LastSchema + '.trg_del_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= 'END' + CHAR(13) + CHAR(10);
		    SET @DropDeleteTrigger+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@DropDeleteTrigger
		    IF ((@GenerateDeleteTrigger = 1 AND @GenerateDropStatements = 1) OR
                (@GenerateInsertUpdateTrigger = 0 AND @GenerateDeleteTrigger = 0 AND @GenerateDropStatements = 1))
            BEGIN
		    PRINT '--Dropping Delete Trigger for ' + @LastTable
            PRINT @DropDeleteTrigger
            END

		    SET @SqlDelete= 'CREATE TRIGGER ' + @LastSchema + '.trg_del_'+ @LastTable + '_ForSync ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '   ON ' + @LastSchema + '.' + @LastTable + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '   AFTER DELETE ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'AS ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- SET NOCOUNT ON added to prevent extra result sets from ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- interfering with SELECT statements.' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	SET NOCOUNT ON; ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @changeContextName nvarchar(100); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @bypassTrackingFlags int; ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @bypassReason nvarchar(512); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    SELECT @changeContextName = ContextName ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '            ,@bypassTrackingFlags = BypassTrackingFlags ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '            ,@bypassReason = BypassReason '  + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '        FROM [track].[udf_GetChangeTrackingSessionDetails](); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- Get the synchronization context.  This will be NULL if this trigger was fired' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '	-- due to a normal application delete.' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @syncContext varbinary(128); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    DECLARE @currentDateTimeOffset datetimeoffset(7); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    SET @currentDateTimeOffset = sysdatetimeoffset(); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    IF ([track].[udf_IsDeleteChangeTrackingEnabled](@bypassTrackingFlags) = 1)' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    BEGIN' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       SET @syncContext = dbo.udf_GetSyncContext(); ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       -- If the changes were made as a result of a trigger being fired, then the update was not being done as a result of a synchronized changed directly to this table. ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       -- Treat the change as a local change so it can be synchronized back to the remote system. ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       IF ((SELECT trigger_nestlevel()) > 1) ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       BEGIN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '           SET @syncContext = NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '       END ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '		  ; WITH ChangeList AS ( ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				SELECT @syncContext AS ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						' + @SqlPKSelectForDelete
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlDelete+= '                   ' + @SqlFKSelectForDelete
            END
		    IF (@HasCreatedDate = 1)
		    BEGIN
		    SET @SqlDelete+= '						,d.CreatedDate AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlDelete+= '						,@currentDateTimeOffset AS Inserted_CreatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasUpdatedDate = 1)
		    BEGIN
		    SET @SqlDelete+= '						,d.UpdatedDate AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlDelete+= '						,@currentDateTimeOffset AS Inserted_UpdatedDate ' + CHAR(13) + CHAR(10);
		    END
		    IF (@HasSiteGuid = 1)
		    BEGIN
		    SET @SqlDelete+= '						,d.' + @SiteGuidColumnOverride + ' AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    ELSE
		    BEGIN
		    SET @SqlDelete+= '						,NULL AS CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL AS PreviousSiteGuid ' + CHAR(13) + CHAR(10);
		    END
		    SET @SqlDelete+= '						,d._RowVersion AS Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,CONVERT(binary(8), @@DBTS) AS Deleted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '					FROM Deleted d ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				) ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				MERGE INTO track.' + @LastTable + ' As ct ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '					USING ChangeList As src ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						ON ' + @SqlDeleteListJoin
		    SET @SqlDelete+= '				WHEN Matched ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '					UPDATE SET DeletedDate = @currentDateTimeOffset ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '								,DeletedContext = src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 								,DeletedRowVersion = src.Deleted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 								,CurrentSiteGuid = src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 								,PreviousSiteGuid = CASE WHEN (((src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NOT NULL) AND (src.CurrentSiteGuid <> ct.CurrentSiteGuid))' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 																OR (src.CurrentSiteGuid IS NULL AND ct.CurrentSiteGuid IS NOT NULL)' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= ' 																OR (src.CurrentSiteGuid IS NOT NULL AND ct.CurrentSiteGuid IS NULL)) THEN ct.CurrentSiteGuid ELSE ct.PreviousSiteGuid END' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				WHEN Not Matched ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				THEN ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				INSERT (InsertedDate' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,InsertedContext' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,InsertedRowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,UpdatedDate' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,UpdatedContext' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,UpdatedRowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,CurrentSiteGuid' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,PreviousSiteGuid' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,DeletedDate' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,DeletedContext' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				    	,DeletedRowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						' + @SqlPKColumns
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlDelete+= '						' + @SqlFKColumns
            END
		    SET @SqlDelete+= '				)' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '				VALUES (CASE WHEN (src.Inserted_CreatedDate IS NOT NULL) THEN src.Inserted_CreatedDate ELSE CAST(''1/1/1990'' AS DateTimeOffset(7)) END ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.Inserted_RowVersion ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.CurrentSiteGuid ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,NULL ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,@currentDateTimeOffset ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.ChangeContext ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						,src.Deleted_RowVersion' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '						' + @SqlPKDeletedData
            IF (@LastSqlForeignKeyColumnName IS NOT NULL AND @LastSqlForeignKeyColumnName <> '')
            BEGIN
		    SET @SqlDelete+= '						' + @SqlFKDeletedData
            END
		    SET @SqlDelete+= '				);' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= '    END' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'END ' + CHAR(13) + CHAR(10);
		    SET @SqlDelete+= 'GO' + CHAR(13) + CHAR(10);

		    --EXEC sp_executesql @Statement=@SqlDelete
	        IF (@GenerateDeleteTrigger = 1)
            BEGIN
		    PRINT '--Creating Delete Trigger for ' + @LastTable
		    EXEC [DevDB].[dbo].[LongPrint] @SqlDelete
            --PRINT @SqlDelete
            END

		    SET @SiteGuidColumnOverride = 'SiteGuid'
		    SET @HasSiteGuid = 0;
		    SET @HasCreatedDate = 0;
		    SET @HasUpdatedDate = 0;

		    SET @SqlPKSelectForInsUpd = N'';
		    SET @SqlPKSelectForDelete = N'';
		    SET @SqlPKColumns = N'';
		    SET @SqlPKData = N'';
		    SET @SqlPKDeletedData = N'';
		    SET @SqlInsertDeleteJoin = N'';
		    SET @SqlChangeListJoin = N'';
		    SET @SqlDeleteListJoin = N'';

		    SET @SqlFKSelectForInsUpd = N'';
		    SET @SqlFKSelectForDelete = N'';
		    SET @SqlFKColumns = N'';
		    SET @SqlFKData = N'';
		    SET @SqlFKDeletedData = N'';

		    SET @StepEndTime=GETDATE()
		    --PRINT '***>>> Step finished on '+CAST(@StepEndTime AS nvarchar(50))
		    --PRINT '***>>> Affected Records: '+CAST (@@ROWCOUNT AS nvarchar(50))
		    --PRINT '***>>> Step elapse time in seconds: '+ CAST(DATEDIFF(ss,@StepStartTime,@StepEndTime) AS nvarchar(50))
		    --PRINT '***>>> Step elapse time in minutes: '+ CAST(DATEDIFF(mi,@StepStartTime,@StepEndTime) AS nvarchar(50))
		    SET @AffectedRecords += @@ROWCOUNT

		    PRINT ''
		    --PRINT '***>>> Step started at '+CAST(@StepStartTime AS nvarchar(50))

		    SET @LastSchema = @Schema
		    SET @LastTable = @Table
            SET @LastSqlForeignKeyColumnName = @SqlForeignKeyColumnName
        END
    END

    --PRINT ''
    --PRINT '********************************************************************************'
    --PRINT '*** Process finished on '+CAST(@StepEndTime AS nvarchar(50))
    --PRINT '*** Total Number of Tables Processed: ' + CAST(@TablesProcessed AS nvarchar(50))
    --PRINT '*** Total Number of Affected Records: ' + CAST(@AffectedRecords AS nvarchar(50))
    --PRINT '*** Process elapse time in minutes: '+ CAST(DATEDIFF(mi,@ProcessStartTime,@StepEndTime) AS nvarchar(50))
    --PRINT '*** Process Complete.'
    CLOSE TableInfoCursor
    DEALLOCATE TableInfoCursor
END
