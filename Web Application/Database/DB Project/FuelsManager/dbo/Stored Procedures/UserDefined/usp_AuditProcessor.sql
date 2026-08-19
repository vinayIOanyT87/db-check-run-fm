-- @ Mode - 0 = Delete Audit Logs
--          1 = Process Audits

CREATE PROCEDURE [dbo].[usp_AuditProcessor]
WITH EXECUTE AS CALLER
AS
BEGIN
	SET NOCOUNT ON

	BEGIN
		DECLARE @SourceNode NVARCHAR(256)
		SELECT @SourceNode = SettingValue FROM [dbo].[tblConfigurationSetting] WHERE SettingKey = 'InstallDetailsSynchronizationNodeName'
		

		DECLARE @Months TABLE(
			MonthIndex INT,
			MonthID NVARCHAR(3))

		INSERT INTO @Months (MonthIndex,MonthID) VALUES (1,'Jan')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (2,'Feb')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (3,'Mar')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (4,'Apr')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (5,'May')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (6,'Jun')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (7,'Jul')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (8,'Aug')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (9,'Sep')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (10,'Oct')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (11,'Nov')
		INSERT INTO @Months (MonthIndex,MonthID) VALUES (12,'Dec')

		CREATE TABLE #ToProcessAudit (
				TableName	NVARCHAR(100)
			,	_AuditRowVersion binary(8)
			,	_AuditGUID UNIQUEIDENTIFIER
			,	_AuditEventType CHAR(1)
			,	_AuditSiteGuid UNIQUEIDENTIFIER
			,	_AuditSessionGuid UNIQUEIDENTIFIER
			,	_AuditCreatedDate DATETIMEOFFSET
			,	_AuditUserID NVARCHAR(100)
			,	_AuditContext UNIQUEIDENTIFIER
			)

		CREATE NONCLUSTERED INDEX [IX_#ToProcessAudit_AuditRowVersion] ON [#ToProcessAudit]
		(
			[_AuditRowVersion] ASC
		)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)

		CREATE TABLE #differenceTable (colname nvarchar(250) primary key, oldval nvarchar(max) , newval nvarchar(max))

		DECLARE	@TableName NVARCHAR(100)
				,	@_AuditGUID UNIQUEIDENTIFIER
				,	@_AuditEventType CHAR(1)
				,	@_AuditSiteGuid UNIQUEIDENTIFIER
				,	@_AuditSessionGuid UNIQUEIDENTIFIER
				,	@_AuditCreatedDate DATETIMEOFFSET
				,	@_AuditUserID NVARCHAR(100)
				,	@_AuditContext UNIQUEIDENTIFIER
				,	@Sql NVARCHAR(max)
				,	@SqlParams NVARCHAR(MAX)
				,	@ColumnName NVARCHAR(500)
				,	@Type NVARCHAR(500)
				,	@TypeID NVARCHAR(50)
				,	@ParentTypeID NVARCHAR(50)
				,	@IDQuery NVARCHAR(MAX)
				,	@SiteGuidQuery NVARCHAR(MAX)

		DECLARE @tblTargetColumns TABLE
		(
			ColName nvarchar(100),
			DataType nvarchar(100),
			IsProcessed bit
		)

		-- This is used to eliminate retrieving records that are entered concurrently into fmaudit tables while the below cursor is processing
		-- which have resulted in essentially getting deletes out of order in the past.
		DECLARE @minRowVer binary(8)
		SET @minRowVer = MIN_ACTIVE_ROWVERSION()

		DECLARE @defaultTopCount int = 400
		DECLARE @topCount int
		DECLARE @topFieldCount int = 100
		BEGIN TRY

		SELECT @topCount = cast(Settingvalue as int) from tblConfigurationSetting where settingkey = 'FMService_AuditLogProcessBatchCount'
		SELECT @topCount = isnull(@topCount, @defaultTopCount)
		END TRY
		BEGIN CATCH
			SET @topCount = @defaultTopCount
		END CATCH

		DECLARE TableCursor CURSOR FOR
			SELECT	t.name as TableName
			FROM	sys.tables t
			INNER JOIN sys.schemas s on s.schema_id=t.schema_id
			WHERE s.name = 'fmaudit'
		OPEN TableCursor
		FETCH NEXT FROM TableCursor INTO @TableName
		WHILE @@FETCH_STATUS=0
		BEGIN
			SET @Sql	=	'INSERT INTO #ToProcessAudit (TableName, _AuditRowVersion, _AuditGUID, _AuditEventType, _AuditSiteGuid, _AuditSessionGuid, _AuditCreatedDate, _AuditUserID, _AuditContext)'
							+ ' SELECT TOP(@topCount)''' + @TableName + ''', _AuditRowVersion, _AuditGUID, _AuditEventType, _AuditSiteGuid, _AuditSessionGuid, _AuditCreatedDate, _AuditUserID, _AuditContext' 
							+ ' FROM [fmaudit].[' + @TableName + '] WHERE _AuditRowVersion < @minRowVer AND (_AuditEventType <> ''U'' OR _AuditEventSequence <> 1) ORDER BY _AuditRowVersion ASC'
			SET @SqlParams = '@minRowVer binary(8), @topCount int'
			EXEC sp_executesql @Sql,@SqlParams, @minRowVer = @minRowVer, @topCount = @topCount
			FETCH NEXT FROM TableCursor INTO @TableName
		END
		CLOSE TableCursor
		DEALLOCATE TableCursor
										
--		SELECT #ToProcessAudit.TableName , _AuditRowVersion, _AuditGuid, _AuditEventType, _AuditSiteGuid, _AuditSessionGuid, _AuditCreatedDate, _AuditUserID, TypeID, ParentTypeID, IDquery
--		FROM #ToProcessAudit LEFT JOIN [dbo].[tblAuditHandler] ON [dbo].[tblAuditHandler].TableName = #ToProcessAudit.TableName ORDER BY _AuditRowVersion ASC

		DECLARE AuditCursor SCROLL CURSOR  FOR
			SELECT TOP(@topCount) #ToProcessAudit.TableName, _AuditGuid, _AuditEventType, _AuditSiteGuid, _AuditSessionGuid, _AuditCreatedDate, _AuditUserID, _AuditContext, TypeID, ParentTypeID, IDquery, SiteGuidQuery
			FROM #ToProcessAudit LEFT JOIN [dbo].[tblAuditHandler] ON [dbo].[tblAuditHandler].TableName = #ToProcessAudit.TableName ORDER BY _AuditRowVersion ASC
		OPEN AuditCursor
		FETCH NEXT FROM AuditCursor INTO @TableName, @_AuditGUID, @_AuditEventType, @_AuditSiteGuid, @_AuditSessionGuid, @_AuditCreatedDate, @_AuditUserID, @_AuditContext, @TypeID, @ParentTypeID, @IDQuery, @SiteGuidQuery
		WHILE @@FETCH_STATUS=0
		BEGIN
--			PRINT '***BEGIN FETCH NEXT FROM AuditCursor'
--			PRINT 'AuditGuid = ' + CAST(@_AuditGuid AS VARCHAR(100))
--			PRINT 'AuditSiteGuid = ' + CAST(@_AuditSiteGuid AS VARCHAR(100))
			DECLARE	@ID NVARCHAR(256) = ''
					, @PropertyID NVARCHAR(50)
					, @SiteGuid UNIQUEIDENTIFIER
			IF @TypeID IS NULL
				SET @TypeID = 'Unknown Type B'		

			-- This Section is for handling Tables not yet supported with TypeID, ParentTypeID, and IDQuery
			IF @IDQuery IS NULL
			BEGIN

				SET @TypeID = ISNULL(@TableName,'Unknown Type A')
				SET @ID = 'Unsupported Table'
				SET @ParentTypeID = ''
				SET @SqlParams = '@SessionID UNIQUEIDENTIFIER,
							@ActionID NVARCHAR(20),
							@TypeID NVARCHAR(50),
							@ID NVARCHAR(256),
							@CreatedDate DATETIMEOFFSET(7),
							@CreatedBy udtUserID,
							@ParentTypeID NVARCHAR(50),
							@SiteGuid UNIQUEIDENTIFIER,
							@SourceNode NVARCHAR(256),
							@AuditContext UNIQUEIDENTIFIER'

				SET @Sql =	'INSERT INTO [dbo].[tblAuditLog] (SessionID, ActionID, TypeID, ID, CreatedDate, CreatedBy, ParentTypeID, SiteGuid, SourceNode, AuditContext) VALUES ('
						+ 'CONVERT(NVARCHAR(50), @SessionID), @ActionID, @TypeID, isnull(@ID,''unknown table''), @CreatedDate, @CreatedBy, @ParentTypeID, @SiteGuid, @SourceNode, @AuditContext)'

--				PRINT '@IDQuery IS NULL - Unsupported Table' 
--				PRINT 'TypeID = ' +  @TypeID
--				PRINT 'TableName = ' + @TableName
				IF @_AuditSiteGuid IS NULL BEGIN SET @_AuditSiteGuid = '00000000-0000-0000-0000-000000000001' END 

				-- Insert Audit Operation
				IF @_AuditEventType = 'I'
				BEGIN

					EXEC sp_executesql @Sql, @SqlParams, 
							@SessionID = @_AuditSessionGuid,
							@ActionID = 'Add',
							@TypeID = @TypeID,
							@ID = @ID,
							@CreatedDate = @_AuditCreatedDate,
							@CreatedBy = @_AuditUserID,
							@ParentTypeID = @ParentTypeID,
							@SiteGuid = @_AuditSiteGuid,
							@SourceNode = @SourceNode,
							@AuditContext = @_AuditContext
				END
				
				-- Update Audit Operation
				ELSE IF @_AuditEventType = 'U'
				BEGIN
		
					EXEC sp_executesql @Sql, @SqlParams, 
							@SessionID = @_AuditSessionGuid,
							@ActionID = 'Modify',
							@TypeID = @TypeID,
							@ID = @ID,
							@CreatedDate = @_AuditCreatedDate,
							@CreatedBy = @_AuditUserID,
							@ParentTypeID = @ParentTypeID,
							@SiteGuid = @_AuditSiteGuid,
							@SourceNode = @SourceNode,
							@AuditContext = @_AuditContext
				END

				-- Delete Audit Operation
				ELSE  IF @_AuditEventType = 'D'
				BEGIN
--					PRINT @Sql
					EXEC sp_executesql @Sql, @SqlParams, 
							@SessionID = @_AuditSessionGuid,
							@ActionID = 'Purge',
							@TypeID = @TypeID,
							@ID = @ID,
							@CreatedDate = @_AuditCreatedDate,
							@CreatedBy = @_AuditUserID,
							@ParentTypeID = @ParentTypeID,
							@SiteGuid = @_AuditSiteGuid,
							@SourceNode = @SourceNode,
							@AuditContext = @_AuditContext
				END
			END
			ELSE
			BEGIN
--				PRINT @IDQuery
				EXEC sp_executesql @statement=@IDQuery,@params=N'@ID NVARCHAR(256) OUTPUT, @_AuditGuid UNIQUEIDENTIFIER', @ID=@ID OUTPUT, @_AuditGUID = @_AuditGuid 
--				PRINT @SiteGuidQuery
				EXEC sp_executesql @statement=@SiteGuidQuery,@params=N'@SiteGuid UNIQUEIDENTIFIER OUTPUT, @_AuditGuid UNIQUEIDENTIFIER', @SiteGuid=@SiteGuid OUTPUT, @_AuditGUID = @_AuditGuid 

				IF (@_AuditSiteGuid is null)
				BEGIN
					set @_AuditSiteGuid = @SiteGuid
				END

				IF CAST(1 AS BIT) = (SELECT EnableAuditLogging FROM dbo.tblSitesShadow WHERE SiteGuid = @SiteGuid)
				BEGIN

					IF @TypeID = 'Application Strings'
					BEGIN
						SET @TypeID = (SELECT dbo.udf_GetLowerWithInitUpperString(ApplicationStringTypeName) 
												FROM [lookup].[tblApplicationStringType]
												WHERE ApplicationStringTypeIndex = (SELECT LookupApplicationStringTypeIndex FROM [fmaudit].[tblApplicationString] WHERE _AuditGUID = @_AuditGUID AND _AuditEventSequence = 1))
					END					
				
					ELSE IF @TypeID = 'List Views'
					BEGIN
						SET @TypeID = (SELECT CASE WHEN LookupListViewTypeIndex = 2 AND LookupListViewStandardTypeIndex = 1 THEN 'Ledger Views' ELSE 'List Views' END 
												FROM [fmaudit].[tblListViews] WHERE _AuditGUID = @_AuditGUID AND _AuditEventSequence = 1)
					END

					ELSE IF @TypeID = 'List View - List View Field'
					BEGIN
						SET @TypeID = (SELECT CASE WHEN v.LookupListViewTypeIndex IS NULL
												THEN CASE WHEN va.LookupListViewTypeIndex = 2 AND va.LookupListViewStandardTypeIndex = 1 THEN 'Ledger View - Ledger View Field' ELSE 'List View - List View Field' END 
												ELSE CASE WHEN v.LookupListViewTypeIndex = 2 AND v.LookupListViewStandardTypeIndex = 1 THEN 'Ledger View - Ledger View Field' ELSE 'List View - List View Field' END
												END
												FROM [fmaudit].[tblListViewFields] a
												LEFT JOIN [dbo].[tblListViews] v ON v.ListViewGuid = a.ListViewGuid
											LEFT JOIN [fmaudit].[tblListViews] va ON va.ListViewGuid = a.ListViewGuid AND va._AuditEventType = 'D'
												WHERE a._AuditGUID = @_AuditGUID AND a._AuditEventSequence = 1)
					END

					ELSE IF @TypeID = 'Qualifications'
					BEGIN
						SET @TypeID = (SELECT dbo.udf_GetLowerWithInitUpperString(QualificationTypeName) 
												FROM [lookup].[tblQualificationType]
												WHERE QualificationTypeIndex = (SELECT LookupQualificationTypeIndex FROM [fmaudit].[tblQualifications] WHERE _AuditGUID = @_AuditGUID AND _AuditEventSequence = 1))
					END	
				
					ELSE IF @TypeID = 'User - Menu Favorite'
					BEGIN
						IF CAST(1 AS BIT) = (SELECT IsQuickLink FROM [fmaudit].[tblMenuFavorites]	WHERE _AuditGUID = @_AuditGUID AND _AuditEventSequence = 1)
							SET @TypeID = 'User - Quick Link'
					END	

					IF @TypeID IS NULL
						SET @TypeID = 'Unknown Type C'
					
					TRUNCATE TABLE #differenceTable						

					-- Insert Audit Operation
					IF @_AuditEventType = 'I'
					BEGIN
						SET @Sql = 'INSERT INTO [dbo].[tblAuditLog] (SessionID, ActionID, TypeID, ID, CreatedDate, CreatedBy, ParentTypeID, SiteGuid, SourceNode, AuditContext) VALUES ('
									+ 'CONVERT(NVARCHAR(50), @SessionID), @ActionID, @TypeID, isnull(@ID,''unknown table''), @CreatedDate, @CreatedBy, @ParentTypeID, @SiteGuid, @SourceNode, @AuditContext)'
	
						SET @SqlParams = '@SessionID UNIQUEIDENTIFIER,
							@ActionID NVARCHAR(20),
							@TypeID NVARCHAR(50),
							@ID NVARCHAR(256),
							@CreatedDate DATETIMEOFFSET(7),
							@CreatedBy udtUserID,
							@ParentTypeID NVARCHAR(50),
							@SiteGuid UNIQUEIDENTIFIER,
							@SourceNode NVARCHAR(256),
							@AuditContext UNIQUEIDENTIFIER'
								
--						PRINT @sql
						EXEC sp_executesql @Sql, @SqlParams, 
							@SessionID = @_AuditSessionGuid,
							@ActionID = 'Add',
							@TypeID = @TypeID,
							@ID = @ID,
							@CreatedDate = @_AuditCreatedDate,
							@CreatedBy = @_AuditUserID,
							@ParentTypeID = @ParentTypeID,
							@SiteGuid = @SiteGuid,
							@SourceNode = @SourceNode,
							@AuditContext = @_AuditContext
					END

					-- Update Audit Operation
					ELSE IF @_AuditEventType = 'U'
					BEGIN
						DECLARE @compareSQL nvarchar(max)
						DECLARE @pivotCol nvarchar(max), @compareCol nvarchar(max)

						DELETE @tblTargetColumns

						INSERT INTO @tblTargetColumns
						(ColName, DataType, IsProcessed)
						SELECT TOP(@topFieldCount) X.COLUMN_NAME, X.DATA_TYPE, 0
						FROM INFORMATION_SCHEMA.COLUMNS x
						WHERE x.TABLE_SCHEMA = 'fmaudit' AND x.TABLE_NAME = @TableName
						AND x.COLUMN_NAME NOT LIKE('_Audit%')
						AND x.COLUMN_NAME NOT IN('CreatedDate','CreatedBy','UpdatedDate','UpdatedBy','_ClusterIdx')
						AND NOT EXISTS
						(
							SELECT * FROM @tblTargetColumns b
							WHERE b.colname = x.COLUMN_NAME
						)
						
						WHILE ((SELECT COUNT(*) FROM @tblTargetColumns WHERe IsProcessed = 0) > 0)
						BEGIN
							SET @pivotCol = STUFF(
								( SELECT ', ' + QUOTENAME(a.ColName)
									FROM  @tblTargetColumns a
									WHERE IsProcessed = 0
									FOR XML PATH('')),1,2,'')

							DECLARE @collation varchar(100) = 'SQL_Latin1_General_CP1_CI_AS'

							SET @compareCol = STUFF(
								( SELECT CASE WHEN a.DataType = 'image' THEN ', CAST(CAST(' +QUOTENAME(a.ColName) +' as varbinary(max)) as nvarchar(max)) COLLATE ' + @collation  ELSE ', CONVERT(NVARCHAR(max), ' + QUOTENAME(a.ColName) + ' ) COLLATE ' + @collation END + ' AS ' + QUOTENAME(a.ColName)
									FROM @tblTargetColumns a
									WHERE IsProcessed = 0
									FOR XML PATH('')),1,2,'')

							SET @compareSQL = N' INSERT INTO #differenceTable '					
							+ N'SELECT COALESCE(New.ColumnName, OLD.COlumnName) AS CN, New.[AuditValueFromTableToProcess] AS NV, Old.[AuditValueFromTableToProcess] AS OV FROM (SELECT Unpvt._auditguid, Unpvt.ColumnName, Unpvt.[AuditValueFromTableToProcess] FROM (SELECT [_auditguid], ' + @compareCol
							+ N' FROM fmaudit.' + @TableName + '  where _auditguid = @_AuditGUID and _auditeventsequence = 1 ) AS DataSource UNPIVOT ([AuditValueFromTableToProcess] FOR ColumnName IN (' + @pivotCol
							+ N') ) AS Unpvt ) AS Old FULL OUTER JOIN ( SELECT Unpvt._auditguid, Unpvt.ColumnName, Unpvt.[AuditValueFromTableToProcess] FROM (SELECT [_auditguid], ' + @compareCol
							+ N' FROM fmaudit.' + @TableName + ' where _auditguid = @_AuditGUID and _auditeventsequence = 2 ) AS DataSource UNPIVOT ([AuditValueFromTableToProcess] FOR ColumnName IN (' + @pivotCol
							+ N')) AS Unpvt) AS New ON Old.ColumnName = New.ColumnName AND Old._auditguid = New._auditguid '
							+ N' WHERE New.[AuditValueFromTableToProcess] != Old.[AuditValueFromTableToProcess] OR New.[AuditValueFromTableToProcess] IS NULL OR Old.[AuditValueFromTableToProcess] IS NULL' 

							--print @compareSQL
							EXEC sp_executesql @statement=@compareSQL,@params=N'@_AuditGUID UNIQUEIDENTIFIER',@_AuditGUID=@_AuditGUID

							UPDATE @tblTargetColumns SET IsProcessed = 1

							INSERT INTO @tblTargetColumns
							(ColName, DataType, IsProcessed)
							SELECT TOP(@topFieldCount) X.COLUMN_NAME, X.DATA_TYPE, 0
							FROM INFORMATION_SCHEMA.COLUMNS x
							WHERE x.TABLE_SCHEMA = 'fmaudit' AND x.TABLE_NAME = @TableName
							AND x.COLUMN_NAME NOT LIKE('_Audit%')
							AND x.COLUMN_NAME NOT IN('CreatedDate','CreatedBy','UpdatedDate','UpdatedBy','_ClusterIdx')
							AND NOT EXISTS
							(
								SELECT * FROM @tblTargetColumns b
								WHERE b.colname = x.COLUMN_NAME
							)
						END

						DECLARE ColumnCursor SCROLL CURSOR FOR
							SELECT	x.COLUMN_NAME
									,	x.DATA_TYPE
							FROM INFORMATION_SCHEMA.COLUMNS x
							INNER JOIN #differenceTable dt on x.COLUMN_NAME = dt.colname
							WHERE x.TABLE_SCHEMA = 'fmaudit' AND x.TABLE_NAME = @TableName
							AND x.COLUMN_NAME NOT LIKE('_Audit%')
							AND x.COLUMN_NAME NOT IN('CreatedDate','CreatedBy','UpdatedDate','UpdatedBy','_ClusterIdx', 'OriginalRowVersion')

						OPEN ColumnCursor
						FETCH NEXT FROM ColumnCursor INTO @ColumnName,@Type

						WHILE @@FETCH_STATUS=0
						BEGIN
							DECLARE	@OldString NVARCHAR(2000)
										, @NewString NVARCHAR(2000)
										, @Lookup NVARCHAR(40)
										, @LookupColumn NVARCHAR(40)
										, @EngineeringUnitsIndex INT
										, @DecimalPlaces TINYINT
										, @ProductType NVARCHAR(20)
										, @TimePattern NVARCHAR(20)
										, @TimeSeparator NVARCHAR(1)
										, @ShortDatePattern NVARCHAR(20)
										, @DateSeparator NVARCHAR(1)
										, @AMSymbol NVARCHAR(2)
										, @PMSymbol NVARCHAR(2)
										, @Year INT
										, @Month INT
										, @Day INT
										, @Hour INT
										, @Minute INT
										, @Second INT
										, @TZOffset INT
										, @TZOffsetMinute INT
										, @TZoffsetSign NVARCHAR(1)
										, @ProductTypeFieldExist INT

							SET @PropertyID = dbo.udf_GetDisplayName(@ColumnName,1)
							SET @OldString = NULL
							SET @NewString = NULL
							SET @EngineeringUnitsIndex = NULL
							SET @DecimalPlaces = NULL

							-- Setup Lookup Table Name
							IF @ColumnName LIKE('Lookup%') 
								OR (@TableName = 'tblFuelCardLimitLineItem' AND @ColumnName = 'Period')
							BEGIN
								IF @ColumnName LIKE('Lookup%')
								BEGIN
									SET @Lookup = REPLACE(REPLACE(REPLACE(@ColumnName,'Lookup',''),'Default',''),'Index','')
									IF @Lookup = 'MailConnectMode'
										SET @Lookup = 'MailServerConnectMode'
									ELSE IF @Lookup = 'MajorCorrectionMethod'
										SET @Lookup = 'MajorCorrectionType'
									ELSE IF @Lookup = 'MinorCorrectionMethod'
										SET @Lookup = 'MinorCorrectionType'
									ELSE IF @Lookup = 'MinimumVariantType'
										SET @Lookup = 'VariantType'
									ELSE IF @Lookup = 'MaximumVariantType'
										SET @Lookup = 'VariantType'
									ELSE IF @Lookup = 'SIValueVariantType'
										SET @Lookup = 'VariantType'
									ELSE IF @Lookup = 'FrequencyType'
										SET @Lookup = 'MessageFrequencyType'
									ELSE IF @Lookup = 'LocationType'
										SET @Lookup = 'MessageLocationType'
									ELSE IF @Lookup = 'Status'
										SET @Lookup = 'TransactionStatus'
									ELSE IF @Lookup = 'SecondaryStorageFillMethod'
										SET @Lookup = 'FillMethod'
									ELSE IF @Lookup = 'OriginApplication'
										SET @Lookup = 'TransactionOrigin'
									ELSE IF @Lookup = 'TransType'
										SET @Lookup = 'TransactionTypes'
									ELSE IF @Lookup = 'Quality'
										SET @Lookup = 'TransactionQuality'
								END
								ELSE
								BEGIN
									SET @Lookup = 'FuelCardLimitPeriod'
								END
							END
							ELSE
								SET @Lookup = NULL
						
					
							SET @LookupColumn = @Lookup + 'Name'
							IF @LookupColumn = 'VariantTypeName'
								SET @LookupColumn = 'CodeType'
					
							IF @Type = 'text'
							BEGIN
								SET @OldString = 'Old Text Non-displayable'
								SET @NewString = 'New Text Non-displayable'
							END

							ELSE IF (@Type = 'nvarchar' OR @Type = 'nchar')
							BEGIN
								IF(@TableName = 'tblPointTemplate' AND @ColumnName = 'PointLogicScript')
								BEGIN
									SET @OldString = 'Old Script Data Non-displayable'
									SET @NewString = 'New Script Data Non-displayable'
								END
								ELSE if(@TableName = 'tblDrawings' AND @ColumnName = 'Image')
								BEGIN
									SET @OldString = 'Old Image Data Non-displayable'
									SET @NewString = 'New Image Data Non-displayable'
								END
								ELSE
								BEGIN
									SET @Sql = 'SELECT @OldString=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGuid'
									EXEC sp_executesql @statement=@sql,@params=N'@_AuditGuid UNIQUEIDENTIFIER, @OldString NVARCHAR(2000) OUTPUT',@_AuditGUID = @_AuditGuid, @OldString=@OldString OUTPUT
									
									SET @Sql = 'SELECT @NewString=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGuid'
									EXEC sp_executesql @statement=@sql,@params=N'@_AuditGuid UNIQUEIDENTIFIER, @NewString NVARCHAR(2000) OUTPUT',@_AuditGUID = @_AuditGuid, @NewString=@NewString OUTPUT
									
									IF @OldString IS NULL
										SET @OldString = 'NULL'
									IF @NewString IS NULL
										SET @NewString = 'NULL'

									--Truncate the Old and New String to 2000 characters to prevent insertion issue.
									IF(len(@NewString) > 2000) Set @NewString = Left(@NewString,2000)
									IF(len(@OldString) > 2000) Set @OldString = Left(@OldString,2000)
								END
							END
							ELSE IF @Type = 'date'
							BEGIN
								DECLARE		@OldDate DATE
										,	@NewDate DATE
										
								SET @Sql = 'SELECT @OldDate=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND  _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@OldDate DATE OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldDate=@OldDate OUTPUT,@_AuditGUID = @_AuditGuid

								SET @Sql = 'SELECT @NewDate=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND  _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@NewDate DATE OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewDate=@NewDate OUTPUT,@_AuditGUID = @_AuditGuid

								IF @OldDate <> @NewDate
								BEGIN
									SET @Sql = 'SELECT @ShortDatePattern=ShortDatePattern,
												@DateSeparator=DateSeparator FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
									EXEC sp_executesql @statement=@sql,@params=N'@ShortDatePattern NVARCHAR(20) OUTPUT,
																				@DateSeparator NVARCHAR(1) OUTPUT, 
																				@_AuditSiteGuid UNIQUEIDENTIFIER',
																				@ShortDatePattern=@ShortDatePattern OUTPUT,
																				@DateSeparator=@DateSeparator OUTPUT,
																				@_AuditSiteGuid = @_AuditSiteGuid

									SET @Year = DATEPART(year,@OldDate)
									SET @Month = DATEPART(month,@OldDate)
									SET @Day = DATEPART(day,@OldDate)

									IF @ShortDatePattern = 'M/d/yyyy'
										SET @OldString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) 
									ELSE IF @ShortDatePattern = 'M/d/yy'
										SET @OldString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)  
									ELSE IF @ShortDatePattern = 'MM/dd/yy'
										SET @OldString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'MM/dd/yyyy'
										SET @OldString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + CAST(@Year AS NVARCHAR(4))
									ELSE IF @ShortDatePattern = 'yy/MM/dd'
										SET @OldString = RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'yyyy-MM-dd'
										SET @OldString = RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'dd-MMM-yy'
										SET @OldString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + (SELECT MonthId FROM @Months WHERE MonthIndex = @Month) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'dd-MM-yy'
										SET @OldString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)

									SET @Year = DATEPART(year,@NewDate)
									SET @Month = DATEPART(month,@NewDate)
									SET @Day = DATEPART(day,@NewDate)

									IF @ShortDatePattern = 'M/d/yyyy'
										SET @NewString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4)
									ELSE IF @ShortDatePattern = 'M/d/yy'
										SET @NewString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'MM/dd/yy'
										SET @NewString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'MM/dd/yyyy'
										SET @NewString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + CAST(@Year AS NVARCHAR(4))
									ELSE IF @ShortDatePattern = 'yy/MM/dd'
										SET @NewString = RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'yyyy-MM-dd'
										SET @NewString = RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'dd-MMM-yy'
										SET @NewString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + (SELECT MonthId FROM @Months WHERE MonthIndex = @Month) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)
									ELSE IF @ShortDatePattern = 'dd-MM-yy'
										SET @NewString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2)
								END
							END

							ELSE IF @Type = 'datetimeoffset' AND @TableName NOT LIKE('tblProcessVariable%')
							BEGIN
								DECLARE		@OldDateTimeOffset DATETIMEOFFSET
											,@NewDateTimeOffset DATETIMEOFFSET
										
								SET @Sql = 'SELECT @OldDateTimeOffset=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@OldDateTimeOffset DATETIMEOFFSET OUTPUT,@_AuditGuid UNIQUEIDENTIFIER',@OldDateTimeOffset=@OldDateTimeOffset OUTPUT, @_AuditGuid = @_AuditGuid

								SET @Sql = 'SELECT @NewDateTimeOffset=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@NewDateTimeOffset DATETIMEOFFSET OUTPUT,@_AuditGuid UNIQUEIDENTIFIER',@NewDateTimeOffset=@NewDateTimeOffset OUTPUT, @_AuditGuid = @_AuditGuid

								-- Handle NULL values. Remember that NULL is neither equal to or not equal to anything, including NULL.
								IF ISNULL(@OldDateTimeOffset, '') <> ISNULL(@NewDateTimeOffset, '')
								BEGIN
									SET @Sql = 'SELECT @TimePattern=TimePattern, @TimeSeparator=TimeSeparator, @ShortDatePattern=ShortDatePattern,
												@DateSeparator=DateSeparator, @AMSymbol=AMSymbol, @PMSymbol=PMSymbol FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'

									EXEC sp_executesql @statement=@sql,@params=N'@TimePattern NVARCHAR(20) OUTPUT,
																				@TimeSeparator NVARCHAR(1) OUTPUT,
																				@ShortDatePattern NVARCHAR(20) OUTPUT,
																				@DateSeparator NVARCHAR(1) OUTPUT,
																				@AMSymbol NVARCHAR(2) OUTPUT,
																				@PMSymbol NVARCHAR(2) OUTPUT,
																				@_AuditSiteGuid UNIQUEIDENTIFIER',
																				@TimePattern=@TimePattern OUTPUT,
																				@TimeSeparator=@TimeSeparator OUTPUT,
																				@ShortDatePattern=@ShortDatePattern OUTPUT,
																				@DateSeparator=@DateSeparator OUTPUT,
																				@AMSymbol=@AMSymbol OUTPUT,
																				@PMSymbol=@PMSymbol OUTPUT,
																				@_AuditSiteGuid = @_AuditSiteGuid
									IF (@OldDateTimeOffset IS NOT NULL)
									BEGIN
										SET @Year = DATEPART(year,@OldDateTimeOffset)
										SET @Month = DATEPART(month,@OldDateTimeOffset)
										SET @Day = DATEPART(day,@OldDateTimeOffset)
										SET @Hour = DATEPART(hour,@OldDateTimeOffset)
										SET @Minute = DATEPART(minute,@OldDateTimeOffset)
										SET @Second = DATEPART(second,@OldDateTimeOffset)
										SET @TZoffset = DATEPART(TZoffset,@OldDateTimeOffset)

										IF @TZoffset < 0
											SET @TZoffsetSign = '-'
										ELSE
											SET @TZoffsetSign = '+'

										IF @TZoffsetSign = '-'
											SET @TZoffset = @TZoffset * -1

										SET @TZoffsetMinute = @TZoffset % 60
										SET @TZoffset = (@TZoffset - @TZoffsetMinute)/60

										SET @OldString = ' ' + @TZoffsetSign + RIGHT('00' + CAST(@TZoffset AS NVARCHAR(2)),2) + @TimeSeparator + RIGHT('00' + CAST(@TZoffsetMinute AS NVARCHAR(2)),2)

										IF @TimePattern = 'hh:mm:ss tt'
										OR @TimePattern = 'h:mm:ss tt'
										OR @TimePattern = 'h:mm tt'
										OR @TimePattern = 'hh:mm tt'
										BEGIN
											IF @Hour > 12
												SET @OldString = ' ' + @PMSymbol + @OldString
											ELSE
												SET @OldString = ' ' + @AMSymbol + @OldString
										END 
										
										IF @TimePattern = 'hh:mm:ss tt'
										OR @TimePattern = 'h:mm:ss tt'
											SET @OldString = @TimeSeparator + RIGHT('00' + CAST(@Second AS NVARCHAR(2)),2) + @OldString

										SET @OldString = @TimeSeparator + RIGHT('00' + CAST(@Minute AS NVARCHAR(2)),2) + @OldString

										IF @TimePattern = 'hh:mm:ss tt'
										OR @TimePattern = 'hh:mm tt'
											SET @OldString = ' ' + RIGHT('00' + CAST(@Hour % 12 AS NVARCHAR(2)),2) + @OldString
	
										ELSE IF @TimePattern = 'h:mm:ss tt'
										OR @TimePattern = 'h:mm tt'
											SET @OldString = ' ' + CAST(@Hour % 12 AS NVARCHAR(2)) + @OldString

										ELSE IF @TimePattern = 'HH:mm:ss'
										OR @TimePattern = 'HH:mm'
											SET @OldString = ' ' + RIGHT('00' + CAST(@Hour AS NVARCHAR(2)),2) + @OldString
								
										ELSE
											SET @OldString = ' ' + CAST(@Hour AS NVARCHAR(2)) + @OldString

										IF @ShortDatePattern = 'M/d/yyyy'
											SET @OldString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) + @OldString 
										ELSE IF @ShortDatePattern = 'M/d/yy'
											SET @OldString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @OldString 
										ELSE IF @ShortDatePattern = 'MM/dd/yy'
											SET @OldString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @OldString 
										ELSE IF @ShortDatePattern = 'MM/dd/yyyy'
											SET @OldString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + CAST(@Year AS NVARCHAR(4)) + @OldString 
										ELSE IF @ShortDatePattern = 'yy/MM/dd'
											SET @OldString = RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @OldString 
										ELSE IF @ShortDatePattern = 'yyyy-MM-dd'
											SET @OldString = RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @OldString 
										ELSE IF @ShortDatePattern = 'dd-MMM-yy'
											SET @OldString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + (SELECT MonthId FROM @Months WHERE MonthIndex = @Month) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @OldString 
										ELSE IF @ShortDatePattern = 'dd-MM-yy'
											SET @OldString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @OldString 
									END
									ELSE 
									BEGIN
										SET @OldString = ''
									END

									IF (@NewDateTimeOffset IS NOT NULL)
									BEGIN
										SET @Year = DATEPART(year,@NewDateTimeOffset)
										SET @Month = DATEPART(month,@NewDateTimeOffset)
										SET @Day = DATEPART(day,@NewDateTimeOffset)
										SET @Hour = DATEPART(hour,@NewDateTimeOffset)
										SET @Minute = DATEPART(minute,@NewDateTimeOffset)
										SET @Second = DATEPART(second,@NewDateTimeOffset)
										SET @TZoffset = DATEPART(TZoffset,@NewDateTimeOffset)

										IF @TZoffset < 0
											SET @TZoffsetSign = '-'
										ELSE
											SET @TZoffsetSign = '+'

										IF @TZoffsetSign = '-'
											SET @TZoffset = @TZoffset * -1

										SET @TZoffsetMinute = @TZoffset % 60
										SET @TZoffset = (@TZoffset - @TZoffsetMinute)/60

										SET @NewString = ' ' + @TZoffsetSign + RIGHT('00' + CAST(@TZoffset AS NVARCHAR(2)),2) + @TimeSeparator + RIGHT('00' + CAST(@TZoffsetMinute AS NVARCHAR(2)),2)

										IF @TimePattern = 'hh:mm:ss tt'
										OR @TimePattern = 'h:mm:ss tt'
										OR @TimePattern = 'h:mm tt'
										OR @TimePattern = 'hh:mm tt'
										BEGIN
											IF @Hour > 12
												SET @NewString = ' ' + @PMSymbol + @NewString
											ELSE
												SET @NewString = ' ' + @AMSymbol + @NewString
										END 
										
										IF @TimePattern = 'hh:mm:ss tt'
										OR @TimePattern = 'h:mm:ss tt'
											SET @NewString = @TimeSeparator + RIGHT('00' + CAST(@Second AS NVARCHAR(2)),2) + @NewString

										SET @NewString = @TimeSeparator + RIGHT('00' + CAST(@Minute AS NVARCHAR(2)),2) + @NewString

										IF @TimePattern = 'hh:mm:ss tt'
										OR @TimePattern = 'hh:mm tt'
											SET @NewString = ' ' + RIGHT('00' + CAST(@Hour % 12 AS NVARCHAR(2)),2) + @NewString
	
										ELSE IF @TimePattern = 'h:mm:ss tt'
										OR @TimePattern = 'h:mm tt'
											SET @NewString = ' ' + CAST(@Hour % 12 AS NVARCHAR(2)) + @NewString

										ELSE IF @TimePattern = 'HH:mm:ss'
										OR @TimePattern = 'HH:mm'
											SET @NewString = ' ' + RIGHT('00' + CAST(@Hour AS NVARCHAR(2)),2) + @NewString
								
										ELSE
											SET @NewString = ' ' + CAST(@Hour AS NVARCHAR(2)) + @NewString

										IF @ShortDatePattern = 'M/d/yyyy'
											SET @NewString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) + @NewString 
										ELSE IF @ShortDatePattern = 'M/d/yy'
											SET @NewString = CAST(@Month AS NVARCHAR(2)) + @DateSeparator + CAST(@Day AS NVARCHAR(2)) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @NewString 
										ELSE IF @ShortDatePattern = 'MM/dd/yy'
											SET @NewString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @NewString 
										ELSE IF @ShortDatePattern = 'MM/dd/yyyy'
											SET @NewString = RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @DateSeparator + CAST(@Year AS NVARCHAR(4)) + @NewString 
										ELSE IF @ShortDatePattern = 'yy/MM/dd'
											SET @NewString = RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @NewString 
										ELSE IF @ShortDatePattern = 'yyyy-MM-dd'
											SET @NewString = RIGHT('0000' + CAST(@Year AS NVARCHAR(4)),4) + @DateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @NewString 
										ELSE IF @ShortDatePattern = 'dd-MMM-yy'
											SET @NewString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + (SELECT MonthId FROM @Months WHERE MonthIndex = @Month) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @NewString 
										ELSE IF @ShortDatePattern = 'dd-MM-yy'
											SET @NewString = RIGHT('00' + CAST(@Day AS NVARCHAR(2)),2) + @dateSeparator + RIGHT('00' + CAST(@Month AS NVARCHAR(2)),2) + @DateSeparator + RIGHT('00' + CAST(@Year % 100 AS NVARCHAR(2)),2) + @NewString 
									END
									ELSE 
									BEGIN
										SET @NewString = ''
									END
								END
							END

							ELSE IF @Type = 'uniqueidentifier'
							BEGIN
								-- Uniqueidentifiers in transactions tables are not processed as the ID's are also stored as part of the record
								IF (@TableName = 'tblAnimation' AND @ColumnName = 'AnimationGuid')
								BEGIN
									SET @OldString = 'Old Animation Definition'
									SET @NewString = 'New Animation Definition'
								END
								ELSE IF @TableName <> 'tblTransactions'
								AND @TableName <> 'tblTransactionLineItems'
								AND @TableName <> 'tblTransactionSubLineItems'
								BEGIN
									DECLARE		@OldGuid UNIQUEIDENTIFIER
												,	@NewGuid UNIQUEIDENTIFIER
										
									SET @Sql = 'SELECT @OldGuid=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGuid'
									EXEC sp_executesql @statement=@sql,@params=N'@OldGuid UNIQUEIDENTIFIER OUTPUT,@_AuditGuid UNIQUEIDENTIFIER',@OldGuid=@OldGuid OUTPUT, @_AuditGuid = @_AuditGuid

									SET @Sql = 'SELECT @NewGuid=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGuid'
									EXEC sp_executesql @statement=@sql,@params=N'@NewGuid UNIQUEIDENTIFIER OUTPUT,@_AuditGuid UNIQUEIDENTIFIER',@NewGuid=@NewGuid OUTPUT, @_AuditGuid = @_AuditGuid

									IF (@OldGuid IS NULL AND @NewGuid IS NOT NULL)
									OR (@OldGuid IS NOT NULL AND @NewGuid IS NULL)
									OR @OldGuid <> @NewGuid
									BEGIN
										IF @OldGuid IS NULL
										BEGIN
											IF @TableName = 'map_tblCompanyPersonnelToShipToBillTo'
											OR @TableName = 'map_tblCompanyPersonnelToSupplierOwner'
											OR @TableName = 'tblUserDataFieldTransactionAlias'
											OR @TableName = 'tblUserDataFieldTransactionAliasLineItem'
											OR @TableName = 'tblMessages'
												SET @OldString = '{All}'

											ELSE IF @TableName = 'tblHouseCards'
											OR @TableName = 'tblAlarmAndEvents'
											OR @TableName = 'tblStations'
											OR @TableName = 'tblTanks'
											OR @TableName = 'tblCompanies'
												SET @OldString = '{None}'

											ELSE IF @TableName = 'tblPersonnel'
											OR @TableName = 'tblEquipment'
												SET @OldString = '{Unassigned}'

											ELSE
												SET @OldString = 'NULL'
										END
										ELSE IF @TableName = 'tblUserDataFieldTransactionAlias'
										OR @TableName = 'tblUserDataFieldTransactionAliasLineItem'
										BEGIN
											SET @OldString = (SELECT GroupID FROM dbo.tblGroups WHERE GroupGuid = @OldGuid)
											IF @OldString IS NULL
												SET @OldString = (SELECT GroupID FROM fmaudit.tblGroups WHERE GroupGuid = @OldGuid AND _AuditEventType = 'D')
										END
										ELSE IF @TableName = 'tblFuelCards'
										BEGIN
											IF @ColumnName = 'SiteGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblSitesShadow WHERE SiteGuid = @OldGuid)
												
												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT ID FROM fmaudit.tblSites WHERE SiteGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
											ELSE IF @ColumnName LIKE('%CompanyGuid')
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @OldGuid)

												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
											ELSE IF @ColumnName LIKE('%ApplicationStringGuid')
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)

												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName LIKE('map_tblProductToPreset%')
										BEGIN
											IF @ColumnName = 'TankGuid'
											BEGIN
												SET @OldString = (SELECT TankID FROM dbo.tblTanks WHERE TankGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT TankID FROM fmaudit.tblTanks WHERE TankGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'TankGroupApplicationStringGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblTankGroups WHERE TankGroupGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblTankGroups WHERE TankGroupGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName LIKE('map_tblTrendPenToTrend')
										BEGIN
											IF @ColumnName = 'PointTagGuid'
											BEGIN
												SET @OldString = (SELECT p.ID + '.' + pt.ID FROM dbo.tblPointTag pt LEFT JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid WHERE pt.PointTagGuid = @OldGuid )
												IF @OldString IS NULL
													SET @OldString = (SELECT p.ID + '.' + pt.ID FROM fmaudit.tblPointTag pt LEFT JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid WHERE pt.PointTagGuid = @OldGuid AND pt._AuditEventType = 'D')
												IF @OldString IS NULL
													SET @OldString = (SELECT p.ID + '.' + pt.ID FROM fmaudit.tblPointTag pt LEFT JOIN fmaudit.tblPoint p ON p.PointGuid = pt.PointGuid WHERE pt.PointTagGuid = @OldGuid AND pt._AuditEventType = 'D' AND p._AuditEventType = 'D')
											END
											IF @ColumnName = 'PointTemplateTagGuid'
											BEGIN
												SET @OldString = (SELECT pt.ID + '.' + ptt.ID FROM dbo.tblPointTemplateTag ptt LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid WHERE ptt.PointTemplateTagGuid = @OldGuid )
												IF @OldString IS NULL
													SET @OldString = (SELECT pt.ID + '.' + pt.ID FROM fmaudit.tblPointTemplateTag ptt LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid WHERE ptt.PointTemplateTagGuid = @OldGuid AND ptt._AuditEventType = 'D')
												IF @OldString IS NULL
													SET @OldString = (SELECT pt.ID + '.' + pt.ID FROM fmaudit.tblPointTemplateTag ptt LEFT JOIN fmaudit.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid WHERE ptt.PointTemplateTagGuid = @OldGuid AND ptt._AuditEventType = 'D' AND pt._AuditEventType = 'D')
											END
										END
										ELSE IF @TableName LIKE('tblProcessVariable%')
										BEGIN
											IF @ColumnName = 'MessageApplicationStringGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblSitesAncillaryData'
										BEGIN
											IF @ColumnName = 'AdjustmentTransactionAliasGuid'
											OR @ColumnName = 'InventoryTransactionAliasGuid'
											BEGIN
												SET @OldString = (SELECT AliasName FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT AliasName FROM fmaudit.tblTransactionAliases WHERE TransactionAliasGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'IATAGuid'
											BEGIN
												SET @OldString = (SELECT IATAID FROM dbo.tblIATA WHERE IATAGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT IATAID FROM fmaudit.tblIATA WHERE IATAGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblProducts'
										BEGIN
											IF @ColumnName = 'TrackingProductGuid'
											BEGIN
												SET @OldString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblPoint'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @OldString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblTanks'
										BEGIN
											IF @ColumnName = 'ManagerCompanyGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @OldString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblTankGroups'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @OldString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblEquipment'
										BEGIN
											IF @ColumnName = 'CompanyGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @OldString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'FuelCardGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblFuelCards WHERE FuelCardGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblFuelCards WHERE FuelCardGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'AssignedToMeterGuid'
											BEGIN
												SET @OldString = (SELECT MeterID FROM dbo.tblMeter WHERE MeterGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT MeterID FROM fmaudit.tblMeter WHERE MeterGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'map_tblProductToCompany'
										OR @TableName = 'map_tblProductToCompanyGroup'
										BEGIN
											IF @ColumnName = 'AdditiveProfileGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblAdditiveProfiles ap WHERE AdditiveProfileGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblAdditiveProfiles WHERE AdditiveProfileGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblPersonnel'
										BEGIN
											IF @ColumnName = 'CompanyGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'SupervisorPersonnelGuid'
											BEGIN
												SET @OldString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblAutoDistributionRule'
										BEGIN
											IF @ColumnName = 'TransactionAliasGuid'
											BEGIN
												SET @OldString = (SELECT AliasName FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT AliasName FROM fmaudit.tblTransactionAliases WHERE TransactionAliasGuid = @OldGuid AND _AuditEventType = 'D')
											END
											IF @ColumnName = 'DefaultReasonCodeGuid'
											BEGIN
												SET @OldString = (SELECT ReasonCode FROM dbo.tblAutoDistributionReasonCodes WHERE AutoDistributionReasonCodeGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ReasonCode FROM fmaudit.tblAutoDistributionReasonCodes WHERE AutoDistributionReasonCodeGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'map_tblCompanyPersonnelToShipToBillTo'
										OR @TableName = 'map_tblCompanyPersonnelToSupplierOwner'
										BEGIN
											IF @ColumnName = 'PersonnelGuid'
											BEGIN
												SET @OldString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblHouseCards'
										BEGIN
											IF @ColumnName = 'DriverPersonnelGuid'
											BEGIN
												SET @OldString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblAlarmAndEvents'
										BEGIN
											IF @ColumnName = 'CategoryGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)
											END		
											ELSE IF @ColumnName = 'PriorityGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblAlarmPriorities WHERE AlarmPriorityGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblAlarmPriorities WHERE AlarmPriorityGuid = @OldGuid)
											END		
										END
										ELSE IF @TableName = 'tblStations'
										BEGIN
											IF @ColumnName = 'TankGuid'
											BEGIN
												SET @OldString = (SELECT TankID FROM dbo.tblTanks WHERE TankGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT TankID FROM fmaudit.tblTanks WHERE TankGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'IssueByVolumeTransactionAliasGuid'
											OR @ColumnName = 'IssueByWeightTransactionAliasGuid'
											OR @ColumnName = 'ReceiptByVolumeTransactionAliasGuid'
											OR @ColumnName = 'ReceiptByWeightTransactionAliasGuid'
											OR @ColumnName = 'RecircTransactionAliasGuid'
											BEGIN
												SET @OldString = (SELECT AliasName FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT AliasName FROM fmaudit.tblTransactionAliases WHERE TransactionAliasGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblCompanies'
										BEGIN
											IF @ColumnName = 'IATAGuid'
											BEGIN
												SET @OldString = (SELECT IATAID FROM dbo.tblIATA WHERE IATAGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT IATAID FROM fmaudit.tblIATA WHERE IATAGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName LIKE('%ApplicationStringGuid')
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)
											END
										END
										ELSE IF @TableName = 'tblMessages'
										BEGIN
											IF @ColumnName = 'CompanyGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @OldGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'PersonnelGuid'
											BEGIN
												SET @OldString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @OldGuid)
												IF @OldString IS NULL
													SET @OldString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @OldGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblFuelCardLimit' OR @TableName = 'tblExternalStation' 
										BEGIN
											IF @ColumnName = 'SiteGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblSitesShadow WHERE SiteGuid = @OldGuid)
												
												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT ID FROM fmaudit.tblSites WHERE SiteGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName = 'tblFuelCardLimitLineItem'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @OldString = (SELECT TOP(1) ProductID FROM dbo.tblProducts WHERE _MasterRecordGuid = @OldGuid)

												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT TOP(1) ProductID FROM fmaudit.tblProducts WHERE _MasterRecordGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
											ELSE IF @ColumnName = 'ProductGroupApplicationStringGuid'
											BEGIN
												SET @OldString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @OldGuid)
												
												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName = 'map_tblExternalStationToProduct'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @OldString = (SELECT TOP(1) ProductID FROM dbo.tblProducts WHERE _MasterRecordGuid = @OldGuid)

												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT TOP(1) ProductID FROM fmaudit.tblProducts WHERE _MasterRecordGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName = 'tblExternalStationGeneralConfiguration'
										BEGIN
											IF @ColumnName = 'RetailSaleTransactionAliasGuid'
											BEGIN
												SET @OldString = (SELECT TOP(1) AliasName FROM dbo.tblTransactionAliases WHERE _MasterRecordGuid = @OldGuid)

												IF @OldString IS NULL
												BEGIN
													SET @OldString = (SELECT TOP(1) AliasName FROM fmaudit.tblTransactionAliases WHERE _MasterRecordGuid = @OldGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE
											SET @OldString = CONVERT(NVARCHAR(36),@OldGuid)
																
										IF @NewGuid IS NULL
										BEGIN
											IF @TableName = 'map_tblCompanyPersonnelToShipToBillTo'
											OR @TableName = 'map_tblCompanyPersonnelToSupplierOwner'
											OR @TableName = 'tblUserDataFieldTransactionAlias'
											OR @TableName = 'tblUserDataFieldTransactionAliasLineItem'
											OR @TableName = 'tblMessages'
												SET @NewString = '{All}'

											ELSE IF @TableName = 'tblHouseCards'
											OR @TableName = 'tblAlarmAndEvents'
											OR @TableName = 'tblStations'
											OR @TableName = 'tblTanks'
											OR @TableName = 'tblCompanies'
												SET @NewString = '{None}'

											ELSE IF @TableName = 'tblPersonnel'
											OR @TableName = 'tblEquipment'
												SET @NewString = '{Unassigned}'

											ELSE
												SET @NewString = 'NULL'
										END
										ELSE IF @TableName = 'tblUserDataFieldTransactionAlias'
										OR @TableName = 'tblUserDataFieldTransactionAliasLineItem'
										BEGIN
											SET @NewString = (SELECT GroupID FROM dbo.tblGroups WHERE GroupGuid = @NewGuid)
											IF @NewString IS NULL
												SET @NewString = (SELECT GroupID FROM fmaudit.tblGroups WHERE GroupGuid = @NewGuid AND _AuditEventType = 'D')
										END
										ELSE IF @TableName = 'tblFuelCards'
										BEGIN
											IF @ColumnName = 'SiteGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblSitesShadow WHERE SiteGuid = @NewGuid)
												
												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT ID FROM fmaudit.tblSites WHERE SiteGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
											ELSE IF @ColumnName LIKE('%CompanyGuid')
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @NewGuid)

												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
											ELSE IF @ColumnName LIKE('%ApplicationStringGuid')
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)

												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName LIKE('map_tblProductToPreset%')
										BEGIN
											IF @ColumnName = 'TankGuid'
											BEGIN
												SET @NewString = (SELECT TankID FROM dbo.tblTanks WHERE TankGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT TankID FROM fmaudit.tblTanks WHERE TankGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'TankGroupApplicationStringGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblTankGroups WHERE TankGroupGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblTankGroups WHERE TankGroupGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName LIKE('map_tblTrendPenToTrend')
										BEGIN
											IF @ColumnName = 'PointTagGuid'
											BEGIN
												SET @NewString = (SELECT p.ID + '.' + pt.ID FROM dbo.tblPointTag pt LEFT JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid WHERE pt.PointTagGuid = @NewGuid )
												IF @NewString IS NULL
													SET @NewString = (SELECT p.ID + '.' + pt.ID FROM fmaudit.tblPointTag pt LEFT JOIN dbo.tblPoint p ON p.PointGuid = pt.PointGuid WHERE pt.PointTagGuid = @NewGuid AND pt._AuditEventType = 'D')
												IF @NewString IS NULL
													SET @NewString = (SELECT p.ID + '.' + pt.ID FROM fmaudit.tblPointTag pt LEFT JOIN fmaudit.tblPoint p ON p.PointGuid = pt.PointGuid WHERE pt.PointTagGuid = @NewGuid AND pt._AuditEventType = 'D' AND p._AuditEventType = 'D')
											END
											IF @ColumnName = 'PointTemplateTagGuid'
											BEGIN
												SET @NewString = (SELECT pt.ID + '.' + ptt.ID FROM dbo.tblPointTemplateTag ptt LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid WHERE ptt.PointTemplateTagGuid = @NewGuid )
												IF @NewString IS NULL
													SET @NewString = (SELECT pt.ID + '.' + pt.ID FROM fmaudit.tblPointTemplateTag ptt LEFT JOIN dbo.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid WHERE ptt.PointTemplateTagGuid = @NewGuid AND ptt._AuditEventType = 'D')
												IF @NewString IS NULL
													SET @NewString = (SELECT pt.ID + '.' + pt.ID FROM fmaudit.tblPointTemplateTag ptt LEFT JOIN fmaudit.tblPointTemplate pt ON pt.PointTemplateGuid = ptt.PointTemplateGuid WHERE ptt.PointTemplateTagGuid = @NewGuid AND ptt._AuditEventType = 'D' AND pt._AuditEventType = 'D')
											END
										END
										ELSE IF @TableName LIKE('tblProcessVariable%')
										BEGIN
											IF @ColumnName = 'MessageApplicationStringGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblSitesAncillaryData'
										BEGIN
											IF @ColumnName = 'AdjustmentTransactionAliasGuid'
											OR @ColumnName = 'InventoryTransactionAliasGuid'
											BEGIN
												SET @NewString = (SELECT AliasName FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT AliasName FROM fmaudit.tblTransactionAliases WHERE TransactionAliasGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'IATAGuid'
											BEGIN
												SET @NewString = (SELECT IATAID FROM dbo.tblIATA WHERE IATAGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT IATAID FROM fmaudit.tblIATA WHERE IATAGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblProducts'
										BEGIN
											IF @ColumnName = 'TrackingProductGuid'
											BEGIN
												SET @NewString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblPoint'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @NewString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblTanks'
										BEGIN
											IF @ColumnName = 'ManagerCompanyGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @NewString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblTankGroups'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @NewString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblEquipment'
										BEGIN
											IF @ColumnName = 'CompanyGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @NewString = (SELECT ProductID FROM dbo.tblProducts WHERE ProductGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ProductID FROM fmaudit.tblProducts WHERE ProductGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'FuelCardGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblFuelCards WHERE FuelCardGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblFuelCards WHERE FuelCardGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'AssignedToMeterGuid'
											BEGIN
												SET @NewString = (SELECT MeterID FROM dbo.tblMeter WHERE MeterGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT MeterID FROM fmaudit.tblMeter WHERE MeterGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'map_tblProductToCompany'
										OR @TableName = 'map_tblProductToCompanyGroup'
										BEGIN
											IF @ColumnName = 'AdditiveProfileGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblAdditiveProfiles ap WHERE AdditiveProfileGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblAdditiveProfiles WHERE AdditiveProfileGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblPersonnel'
										BEGIN
											IF @ColumnName = 'CompanyGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'SupervisorPersonnelGuid'
											BEGIN
												SET @NewString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblAutoDistributionRule'
										BEGIN
											IF @ColumnName = 'TransactionAliasGuid'
											BEGIN
												SET @NewString = (SELECT AliasName FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT AliasName FROM fmaudit.tblTransactionAliases WHERE TransactionAliasGuid = @NewGuid AND _AuditEventType = 'D')
											END
											IF @ColumnName = 'DefaultReasonCodeGuid'
											BEGIN
												SET @NewString = (SELECT ReasonCode FROM dbo.tblAutoDistributionReasonCodes WHERE AutoDistributionReasonCodeGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ReasonCode FROM fmaudit.tblAutoDistributionReasonCodes WHERE AutoDistributionReasonCodeGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'map_tblCompanyPersonnelToShipToBillTo'
										OR @TableName = 'map_tblCompanyPersonnelToSupplierOwner'
										BEGIN
											IF @ColumnName = 'PersonnelGuid'
											BEGIN
												SET @NewString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblHouseCards'
										BEGIN
											IF @ColumnName = 'DriverPersonnelGuid'
											BEGIN
												SET @NewString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblAlarmAndEvents'
										BEGIN
											IF @ColumnName = 'CategoryGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)
											END		
											ELSE IF @ColumnName = 'PriorityGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblAlarmPriorities WHERE AlarmPriorityGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblAlarmPriorities WHERE AlarmPriorityGuid = @NewGuid)
											END		
										END
										ELSE IF @TableName = 'tblStations'
										BEGIN
											IF @ColumnName = 'TankGuid'
											BEGIN
												SET @NewString = (SELECT TankID FROM dbo.tblTanks WHERE TankGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT TankID FROM fmaudit.tblTanks WHERE TankGuid = @NewGuid)
											END
											ELSE IF @ColumnName = 'IssueByVolumeTransactionAliasGuid'
											OR @ColumnName = 'IssueByWeightTransactionAliasGuid'
											OR @ColumnName = 'ReceiptByVolumeTransactionAliasGuid'
											OR @ColumnName = 'ReceiptByWeightTransactionAliasGuid'
											OR @ColumnName = 'RecircTransactionAliasGuid'
											BEGIN
												SET @NewString = (SELECT AliasName FROM dbo.tblTransactionAliases WHERE TransactionAliasGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT AliasName FROM fmaudit.tblTransactionAliases WHERE TransactionAliasGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblCompanies'
										BEGIN
											IF @ColumnName = 'IATAGuid'
											BEGIN
												SET @NewString = (SELECT IATAID FROM dbo.tblIATA WHERE IATAGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT IATAID FROM fmaudit.tblIATA WHERE IATAGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName LIKE('%ApplicationStringGuid')
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)
											END
										END
										ELSE IF @TableName = 'tblMessages'
										BEGIN
											IF @ColumnName = 'CompanyGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblCompanies WHERE CompanyGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT ID FROM fmaudit.tblCompanies WHERE CompanyGuid = @NewGuid AND _AuditEventType = 'D')
											END
											ELSE IF @ColumnName = 'PersonnelGuid'
											BEGIN
												SET @NewString = (SELECT PersonID FROM dbo.tblPersonnel WHERE PersonnelGuid = @NewGuid)
												IF @NewString IS NULL
													SET @NewString = (SELECT PersonID FROM fmaudit.tblPersonnel WHERE PersonnelGuid = @NewGuid AND _AuditEventType = 'D')
											END
										END
										ELSE IF @TableName = 'tblFuelCardLimit' OR @TableName = 'tblExternalStation'
										BEGIN
											IF @ColumnName = 'SiteGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblSitesShadow WHERE SiteGuid = @NewGuid)
												
												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT ID FROM fmaudit.tblSites WHERE SiteGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName = 'tblFuelCardLimitLineItem'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @NewString = (SELECT TOP(1) ProductID FROM dbo.tblProducts WHERE _MasterRecordGuid = @NewGuid)

												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT TOP(1) ProductID FROM fmaudit.tblProducts WHERE _MasterRecordGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
											ELSE IF @ColumnName = 'ProductGroupApplicationStringGuid'
											BEGIN
												SET @NewString = (SELECT ID FROM dbo.tblApplicationString WHERE ApplicationStringGuid = @NewGuid)
												
												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT ID FROM fmaudit.tblApplicationString WHERE ApplicationStringGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName = 'map_tblExternalStationToProduct'
										BEGIN
											IF @ColumnName = 'ProductGuid'
											BEGIN
												SET @NewString = (SELECT TOP(1) ProductID FROM dbo.tblProducts WHERE _MasterRecordGuid = @NewGuid)

												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT TOP(1) ProductID FROM fmaudit.tblProducts WHERE _MasterRecordGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE IF @TableName = 'tblExternalStationGeneralConfiguration'
										BEGIN
											IF @ColumnName = 'RetailSaleTransactionAliasGuid'
											BEGIN
												SET @NewString = (SELECT TOP(1) AliasName FROM dbo.tblTransactionAliases WHERE _MasterRecordGuid = @NewGuid)

												IF @NewString IS NULL
												BEGIN
													SET @NewString = (SELECT TOP(1) AliasName FROM fmaudit.tblTransactionAliases WHERE _MasterRecordGuid = @NewGuid AND _AuditEventType = 'D')
												END
											END
										END
										ELSE
											SET @NewString = CONVERT(NVARCHAR(36),@NewGuid)
									END
								END
							END
						
							ELSE IF @Type = 'bit'
							BEGIN
								DECLARE		@OldBit BIT
											,	@NewBit BIT
										
								SET @Sql = 'SELECT @OldBit=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@OldBit BIT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldBit=@OldBit OUTPUT, @_AuditGuid = @_AuditGuid

								SET @Sql = 'SELECT @NewBit=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@NewBit BIT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewBit=@NewBit OUTPUT, @_AuditGuid = @_AuditGuid

								IF (@OldBit IS NULL AND @NewBit IS NOT NULL)
								OR (@OldBit IS NOT NULL AND @NewBit IS NULL)
								OR @OldBit <> @NewBit
								BEGIN
									IF @OldBit IS NULL
										Set @OldString = 'NULL'
									ELSE IF @OldBit = CAST(1 AS BIT)
										SET @OldString = 'true'
									ELSE
										SET @OldString = 'false'
									
									IF @NewBit IS NULL
										SET @NewString = 'NULL'
									ELSE IF @NewBit = CAST(1 AS BIT)
										SET @NewString = 'true'
									ELSE
										SET @NewString = 'false'
								END
							END

							ELSE IF @Type = 'xml'
							BEGIN
								DECLARE @OldXml xml;
								DECLARE @NewXml xml;

								IF @TableName = 'tblTransactionAliasFields'
										BEGIN
											IF @ColumnName = 'DefaultValue'
											BEGIN
												SET @Sql = 'SELECT @OldString=[' + @ColumnName + '].value(''/'', ''nvarchar(MAX)'') FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
												EXEC sp_executesql @statement=@sql,@params=N'@OldString nvarchar(MAX) OUTPUT',@OldString=@OldString OUTPUT
												SET @Sql = 'SELECT @NewString=[' + @ColumnName + '].value(''/'', ''nvarchar(MAX)'') FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
												EXEC sp_executesql @statement=@sql,@params=N'@NewString nvarchar(MAX) OUTPUT',@NewString=@NewString OUTPUT

												SET @OldString  = IsNull(@OldString, 'NULL')
												SET @NewString  = IsNull(@NewString, 'NULL')
											END
										END
								ELSE
									BEGIN
										SET @Sql = 'SELECT @OldXml=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
										EXEC sp_executesql @statement=@sql,@params=N'@OldXml xml OUTPUT',@OldXml=@OldXml OUTPUT

										SET @Sql = 'SELECT @NewXml=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
										EXEC sp_executesql @statement=@sql,@params=N'@NewXml xml OUTPUT',@NewXml=@NewXml OUTPUT

										IF (@OldXml IS NULL AND @NewXml IS NOT NULL)
										OR (@OldXml IS NOT NULL AND @NewXml IS NULL)
										OR CONVERT(NVARCHAR(MAX),@OldXml) <> CONVERT(NVARCHAR(MAX),@NewXml)
										BEGIN
											SET @OldString = 'Old Xml'
											SET @NewString = 'New Xml'
										END
									END
							END
						
							ELSE IF @Type = 'int'
							BEGIN
								DECLARE		@OldInt INT 
											,	@NewInt INT
										
								SET @Sql = 'SELECT @OldInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@OldInt INT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldInt=@OldInt OUTPUT, @_AuditGuid = @_AuditGuid

								SET @Sql = 'SELECT @NewInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGuid'
								EXEC sp_executesql @statement=@sql,@params=N'@NewInt INT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewInt=@NewInt OUTPUT, @_AuditGuid = @_AuditGuid


								IF (@OldInt IS NULL AND @NewInt IS NOT NULL)
								OR (@OldInt IS NOT NULL AND @NewInt IS NULL)
								OR @OldInt <> @NewInt
								BEGIN
									IF @ColumnName LIKE('%UnitIndex')
									OR @ColumnName LIKE('%Units')
									BEGIN
										IF @OldInt IS NULL
											SET @OldString = '{Site}'
										ELSE
											SET @OldString = dbo.udf_GetUnitAbbrev(@OldInt,0)
									END 

									ELSE IF @OldInt IS NULL
										SET @OldString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @OldString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = @OldInt'
										EXEC sp_executesql @statement=@sql,@params=N'@OldString NVARCHAR(2000) OUTPUT, @OldInt INT ',@OldString=@OldString OUTPUT, @OldInt = @OldInt
									END
									ELSE IF @TableName = 'tblFuelCards'
									AND @ColumnName = 'ActivationStatus'
									BEGIN
										IF @OldInt = 0
											SET @OldString = 'Active'
										ELSE IF @OldInt = 1
											SET @OldString = 'Inactive'
										ELSE IF @OldInt = 2
											SET @OldString = 'Cancelled' 
										ELSE IF @OldInt = 3
											SET @OldString = 'Locked' 
										ELSE IF @OldInt = 4
											SET @OldString = 'Lost/Stolen' 
									END
									ELSE IF @TableName = 'tblFCEEMapping' AND @ColumnName = 'MsgType' 
									BEGIN
										SET @OldString = (SELECT TOP 1 EdgeMessageCode FROM lookup.tblEdgeMessage WHERE EdgeMessageIndex=@OldInt)
									END
									ELSE IF @TableName = 'tblPointTag' AND @ColumnName = 'PointTagInputOutputTypeIndex' 
									BEGIN
										SET @OldString = (SELECT TOP 1 [PointTagInputOutputTypeName] FROM [lookup].[tblPointTagInputOutputType] WHERE[PointTagInputOutputTypeIndex]=@OldInt)
									END
									ELSE IF @TableName = 'tblPointTag' AND @ColumnName = 'LastPointTagInputOutputTypeIndex' 
									BEGIN
										SET @OldString = (SELECT TOP 1 [PointTagInputOutputTypeName] FROM [lookup].[tblPointTagInputOutputType] WHERE[PointTagInputOutputTypeIndex]=@OldInt)
									END
									ELSE IF @TableName = 'tblPointTemplateTag' AND @ColumnName = 'PointTagInputOutputTypeIndex' 
									BEGIN
										SET @OldString = (SELECT TOP 1 [PointTagInputOutputTypeName] FROM [lookup].[tblPointTagInputOutputType] WHERE [PointTagInputOutputTypeIndex]=@OldInt)
									END
									ELSE
										SET @OldString = CONVERT(NVARCHAR(20),@OldInt)
									
									IF @ColumnName LIKE('%UnitIndex')
									OR @ColumnName LIKE('%Units')
									BEGIN
										IF @NewInt IS NULL
											SET @NewString = '{Site}'
										ELSE
											SET @NewString = dbo.udf_GetUnitAbbrev(@NewInt,0)
									END 

									ELSE IF @NewInt IS NULL
										SET @NewString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @NewString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = @NewInt'
										EXEC sp_executesql @statement=@sql,@params=N'@NewString NVARCHAR(2000) OUTPUT, @NewInt INT ',@NewString=@NewString OUTPUT, @NewInt = @NewInt
									END
									ELSE IF @TableName = 'tblFuelCards'
									AND @ColumnName = 'ActivationStatus'
									BEGIN
										IF @NewInt = 0
											SET @NewString = 'Active'
										ELSE IF @NewInt = 1
											SET @NewString = 'Inactive'
										ELSE IF @NewInt = 2
											SET @NewString = 'Cancelled' 
										ELSE IF @NewInt = 3
											SET @NewString = 'Locked' 
										ELSE IF @NewInt = 4
											SET @NewString = 'Lost/Stolen' 
									END
									ELSE IF @TableName = 'tblFCEEMapping' AND @ColumnName = 'MsgType' 
									BEGIN
										SET @NewString = (SELECT TOP 1 EdgeMessageCode FROM lookup.tblEdgeMessage WHERE EdgeMessageIndex=@NewInt)
									END
									ELSE IF @TableName = 'tblPointTag' AND @ColumnName = 'PointTagInputOutputTypeIndex' 
									BEGIN
										SET @NewString = (SELECT TOP 1 [PointTagInputOutputTypeName] FROM [lookup].[tblPointTagInputOutputType] WHERE[PointTagInputOutputTypeIndex]=@NewInt)
									END
									ELSE IF @TableName = 'tblPointTag' AND @ColumnName = 'LastPointTagInputOutputTypeIndex' 
									BEGIN
										SET @NewString = (SELECT TOP 1 [PointTagInputOutputTypeName] FROM [lookup].[tblPointTagInputOutputType] WHERE[PointTagInputOutputTypeIndex]=@NewInt)
									END
									ELSE IF @TableName = 'tblPointTemplateTag' AND @ColumnName = 'PointTagInputOutputTypeIndex' 
									BEGIN
										SET @NewString = (SELECT TOP 1 [PointTagInputOutputTypeName] FROM [lookup].[tblPointTagInputOutputType] WHERE [PointTagInputOutputTypeIndex]=@NewInt)
									END
									ELSE
										SET @NewString = CONVERT(NVARCHAR(20),@NewInt)
								END
							END

							ELSE IF @Type = 'smallint'
							BEGIN
								DECLARE		@OldSmallInt SMALLINT
											,	@NewSmallInt SMALLINT

										
								SET @Sql = 'SELECT @OldSmallInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
								EXEC sp_executesql @statement=@sql,@params=N'@OldSmallInt SMALLINT OUTPUT',@OldSmallInt=@OldSmallInt OUTPUT

								SET @Sql = 'SELECT @NewSmallInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
								EXEC sp_executesql @statement=@sql,@params=N'@NewSmallInt SMALLINT OUTPUT',@NewSmallInt=@NewSmallInt OUTPUT

								IF (@OldSmallInt IS NULL AND @NewSmallInt IS NOT NULL)
								OR (@OldSmallInt IS NOT NULL AND @NewSmallInt IS NULL)
								OR @OldSmallInt <> @NewSmallInt
								BEGIN

									IF @OldSmallInt IS NULL
										SET @OldString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @OldString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = ' + CONVERT(NVARCHAR(10),@OldSmallInt)
										EXEC sp_executesql @statement=@sql,@params=N'@OldString NVARCHAR(2000) OUTPUT',@OldString=@OldString OUTPUT
									END

									ELSE
										SET @OldString = CONVERT(NVARCHAR(20),@OldSmallInt)

									IF @NewSmallInt IS NULL
										SET @NewString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @NewString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = ' + CONVERT(NVARCHAR(10),@NewSmallInt)
										EXEC sp_executesql @statement=@sql,@params=N'@NewString NVARCHAR(2000) OUTPUT',@NewString=@NewString OUTPUT
									END

									ELSE
										SET @NewString = CONVERT(NVARCHAR(20),@NewSmallInt)
	
								END
							END

							ELSE IF @Type = 'tinyint'
							BEGIN
								DECLARE		@OldTinyInt TINYINT
											,	@NewTinyInt TINYINT

										
								SET @Sql = 'SELECT @OldTinyInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID '
								EXEC sp_executesql @statement=@sql,@params=N'@OldTinyInt TINYINT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldTinyInt=@OldTinyInt OUTPUT, @_AuditGUID = @_AuditGUID 

								SET @Sql = 'SELECT @NewTinyInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGUID '
								EXEC sp_executesql @statement=@sql,@params=N'@NewTinyInt TINYINT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewTinyInt=@NewTinyInt OUTPUT, @_AuditGUID = @_AuditGUID 

								IF (@OldTinyInt IS NULL AND @NewTinyInt IS NOT NULL)
								OR (@OldTinyInt IS NOT NULL AND @NewTinyInt IS NULL)
								OR @OldTinyInt <> @NewTinyInt
								BEGIN

									IF @OldTinyInt IS NULL
										SET @OldString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @OldString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = @OldTinyInt'
										EXEC sp_executesql @statement=@sql,@params=N'@OldString NVARCHAR(2000) OUTPUT, @OldTinyInt TINYINT',@OldString=@OldString OUTPUT, @OldTinyInt=@OldTinyInt
									END

									ELSE
										SET @OldString = CONVERT(NVARCHAR(20),@OldTinyInt)

									IF @NewTinyInt IS NULL
										SET @NewString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @NewString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = @NewTinyInt'
										EXEC sp_executesql @statement=@sql,@params=N'@NewString NVARCHAR(2000) OUTPUT, @NewTinyInt TINYINT',@NewString=@NewString OUTPUT, @NewTinyInt=@NewTinyInt
									END

									ELSE
										SET @NewString = CONVERT(NVARCHAR(20),@NewTinyInt)
	
								END
							END

							ELSE IF @Type = 'bigint'
							BEGIN
								DECLARE		@OldBigInt BIGINT
											,	@NewBigInt BIGINT

										
								SET @Sql = 'SELECT @OldBigInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
								EXEC sp_executesql @statement=@sql,@params=N'@OldBigInt BIGINT OUTPUT',@OldBigInt=@OldBigInt OUTPUT

								SET @Sql = 'SELECT @NewBigInt=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = ''' + CONVERT(NVARCHAR(50),@_AuditGUID) + ''''
								EXEC sp_executesql @statement=@sql,@params=N'@NewBigInt BIGINT OUTPUT',@NewBigInt=@NewBigInt OUTPUT

								IF (@OldBigInt IS NULL AND @NewBigInt IS NOT NULL)
								OR (@OldBigInt IS NOT NULL AND @NewBigInt IS NULL)
								OR @OldBigInt <> @NewBigInt
								BEGIN

									IF @OldBigInt IS NULL
										SET @OldString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @OldString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = ' + CONVERT(NVARCHAR(1),@OldBigInt)
										EXEC sp_executesql @statement=@sql,@params=N'@OldString NVARCHAR(2000) OUTPUT',@OldString=@OldString OUTPUT
									END

									ELSE
										SET @OldString = CONVERT(NVARCHAR(20),@OldBigInt)

									IF @NewBigInt IS NULL
										SET @NewString = 'NULL'

									ELSE IF @Lookup IS NOT NULL
									BEGIN
										SET @Sql = 'SELECT @NewString=dbo.udf_GetLowerWithInitUpperString(' + @LookupColumn + ') FROM [lookup].[tbl' + @Lookup + '] WHERE ' + @Lookup + 'Index = ' + CONVERT(NVARCHAR(10),@NewBigInt)
										EXEC sp_executesql @statement=@sql,@params=N'@NewString NVARCHAR(2000) OUTPUT',@NewString=@NewString OUTPUT
									END

									ELSE
										SET @NewString = CONVERT(NVARCHAR(20),@NewBigInt)
	
								END
							END
							ELSE IF @Type = 'float'
							BEGIN
								DECLARE		@OldFloat FLOAT 
											,	@NewFloat FLOAT
										
								SET @Sql = 'SELECT @OldFloat=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID '
								EXEC sp_executesql @statement=@sql,@params=N'@OldFloat FLOAT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldFloat=@OldFloat OUTPUT, @_AuditGUID = @_AuditGUID 

								SET @Sql = 'SELECT @NewFloat=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGUID '
								EXEC sp_executesql @statement=@sql,@params=N'@NewFloat FLOAT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewFloat=@NewFloat OUTPUT, @_AuditGUID = @_AuditGUID 


								IF (@OldFloat IS NULL AND @NewFloat IS NOT NULL)
								OR (@OldFloat IS NOT NULL AND @NewFloat IS NULL)
								OR @OldFloat <> @NewFloat
								BEGIN
									IF @ColumnName = 'MassQuantity'
									OR @ColumnName = 'BeginQuantityValue'
									OR @ColumnName = 'RequestedQuantityValue'
									OR @ColumnName = 'FinalQuantityValue'
									BEGIN
										SET @Sql = 'SELECT @EngineeringUnitsIndex = MassUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										SET @Sql = 'SELECT @DecimalPlaces = MassDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
									ELSE IF @ColumnName = 'GrossQuantity'
									OR @ColumnName = 'NetQuantity'
									OR @ColumnName = 'LineFill'
									OR @ColumnName = 'BottomVolume'
									OR @ColumnName = 'CleanLineDeductQuantity'
									OR @ColumnName = 'CleanLinePackQuantity'
									OR @ColumnName = 'PresetAmount'
									OR @ColumnName = 'ReceiptVariance'
									OR @ColumnName = 'Variance'
									OR @ColumnName = 'LoadRackVariance'
									OR @ColumnName = 'MinimumLevel'
									OR @ColumnName = 'WarningLevel'
									BEGIN
										SET @SQL = 'SELECT @ProductTypeFieldExist = COL_LENGTH(''[fmaudit].[' + @TableName + ']'', ''ProductType'')'
										EXEC sp_executesql @statement = @sql, @params = N'@ProductTypeFieldExist INT OUTPUT ', @ProductTypeFieldExist = @ProductTypeFieldExist OUTPUT
										
										IF (@ProductTypeFieldExist IS NOT NULL)
										BEGIN

											SET @SQL = 'SELECT @ProductType = ProductType FROM [fmaudit].['+@TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID'
											EXEC sp_executesql @statement=@sql,@parems=N'@ProductType NVARCHAR(20) OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@ProductType=@ProductType OUTPUT, @_AuditGUID = @_AuditGUID  
										
											IF @ProductType = 'Additive'
											BEGIN
												SET @Sql = 'SELECT @EngineeringUnitsIndex = AdditiveVolumeUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
												EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
												SET @Sql = 'SELECT @DecimalPlaces = AdditiveVolumeDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
												EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
											END
											ELSE
											BEGIN
												SET @Sql = 'SELECT @EngineeringUnitsIndex = VolumeUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
												EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
												SET @Sql = 'SELECT @DecimalPlaces = VolumeDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
												EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
											END
										END
										ELSE
										BEGIN
											SET @Sql = 'SELECT @EngineeringUnitsIndex = VolumeUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
											EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
											SET @Sql = 'SELECT @DecimalPlaces = VolumeDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
											EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										END
									END
									ELSE IF @ColumnName = 'AdditiveRate'
									BEGIN
										SET @Sql = 'SELECT @EngineeringUnitsIndex = AdditiveProfileRateUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										SET @Sql = 'SELECT @DecimalPlaces = AdditiveProfileRateDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
									ELSE IF @ColumnName = 'AdditiveCycleVolume'
									BEGIN
										SET @Sql = 'SELECT @EngineeringUnitsIndex = AdditiveProfileCycleAmountUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										SET @Sql = 'SELECT @DecimalPlaces = AdditiveProfileCycleAmountDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
									ELSE IF @ColumnName = 'Temperature'
									OR @ColumnName = 'FreezePoint'
									BEGIN
										SET @Sql = 'SELECT @EngineeringUnitsIndex = TemperatureUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										SET @Sql = 'SELECT @DecimalPlaces = TemperatureDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
									ELSE IF @ColumnName = 'Density'
									BEGIN
										SET @Sql = 'SELECT @EngineeringUnitsIndex = DensityUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										SET @Sql = 'SELECT @DecimalPlaces = DensityDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
									ELSE IF @ColumnName = 'DifferentialPressure'
									BEGIN
										SET @Sql = 'SELECT @EngineeringUnitsIndex = PressureUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
										SET @Sql = 'SELECT @DecimalPlaces = PressureDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
									ELSE IF @TableName = 'tblFuelCardLimitLineItem' AND @ColumnName = 'Limit'
									BEGIN
											SET @Sql = 'SELECT @EngineeringUnitsIndex = VolumeUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
											EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid

											SET @Sql = 'SELECT @DecimalPlaces = VolumeDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
											EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END

									IF @OldFloat IS NULL
										SET @OldString = 'NULL'
									
									ELSE IF @EngineeringUnitsIndex IS NOT NULL
										SET @OldString = CONVERT(NVARCHAR(20),dbo.udf_ConvertFromSIUnits(@OldFloat,@EngineeringUnitsIndex,@DecimalPlaces))

									ELSE
										SET @OldString = CONVERT(NVARCHAR(20),@OldFloat)
									
									IF @NewFloat IS NULL
										SET @NewString = 'NULL'
									
									ELSE IF @EngineeringUnitsIndex IS NOT NULL
										SET @NewString = CONVERT(NVARCHAR(20),dbo.udf_ConvertFromSIUnits(@NewFloat,@EngineeringUnitsIndex,@DecimalPlaces))

									ELSE
										SET @NewString = CONVERT(NVARCHAR(20),@NewFloat)

									
								END
							END

							ELSE IF @Type = 'money'
							BEGIN
								DECLARE		@OldMoney MONEY 
											,	@NewMoney MONEY
										
								SET @Sql = 'SELECT @OldMoney=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID'
								EXEC sp_executesql @statement=@sql,@params=N'@OldMoney MONEY OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldMoney=@OldMoney OUTPUT, @_AuditGUID = @_AuditGUID  

								SET @Sql = 'SELECT @NewMoney=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGUID'
								EXEC sp_executesql @statement=@sql,@params=N'@NewMoney MONEY OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewMoney=@NewMoney OUTPUT, @_AuditGUID = @_AuditGUID  


								IF (@OldMoney IS NULL AND @NewMoney IS NOT NULL)
								OR (@OldMoney IS NOT NULL AND @NewMoney IS NULL)
								OR @OldMoney <> @NewMoney
								BEGIN
									IF @OldMoney IS NULL
										SET @OldString = 'NULL'
									
									ELSE
										SET @OldString = CONVERT(NVARCHAR(20),@OldMoney)
									
									IF @NewMoney IS NULL
										SET @NewString = 'NULL'
									
									ELSE
										SET @NewString = CONVERT(NVARCHAR(20),@NewMoney)

									
								END
							END

							ELSE IF @Type = 'varbinary'
							AND ( @TableName LIKE('tblProcessVariable%') OR @TableName = 'tblSyncClientConfiguration' )
							BEGIN
								DECLARE		@OldVarBinary VARBINARY(max) 
											,	@NewVarBinary VARBINARY(max)
											, @OldQuality SMALLINT
											, @NewQuality SMALLINT
									
								SET @Sql = 'SELECT @OldVarBinary=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID'
								EXEC sp_executesql @statement=@sql,@params=N'@OldVarBinary VARBINARY(max) OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldVarBinary=@OldVarBinary OUTPUT, @_AuditGUID = @_AuditGUID 

								SET @Sql = 'SELECT @NewVarBinary=[' + @ColumnName + '] FROM [fmAudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGUID'
								EXEC sp_executesql @statement=@sql,@params=N'@NewVarBinary VARBINARY(max) OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewVarBinary=@NewVarBinary OUTPUT, @_AuditGUID = @_AuditGUID 
								
								IF @ColumnName = 'SIValue'
								BEGIN
									SET @Sql = 'SELECT @OldQuality = Quality FROM [fmaudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID'
									EXEC sp_executesql @statement=@sql,@params=N'@OldQuality SMALLINT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@OldQuality=@OldQuality OUTPUT, @_AuditGUID = @_AuditGUID 
									SET @Sql = 'SELECT @NewQuality = Quality FROM [fmaudit].[' + @TableName + '] WHERE _AuditEventSequence = 2 AND _AuditGUID = @_AuditGUID'
									EXEC sp_executesql @statement=@sql,@params=N'@NewQuality SMALLINT OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@NewQuality=@NewQuality OUTPUT, @_AuditGUID = @_AuditGUID 
								END
								ELSE
								BEGIN
									SET @OldQuality = NULL
									SET @NewQuality = NULL
								END
							
								IF (@OldVarBinary IS NULL AND @NewVarBinary IS NOT NULL)
								OR (@OldVarBinary IS NOT NULL AND @NewVarBinary IS NULL)
								OR (@OldVarBinary <> @NewVarBinary)
								OR (@OldQuality IS NULL AND @NewQuality IS NOT NULL)
								OR (@OldQuality IS NOT NULL AND @NewQuality IS NULL)
								OR (@OldQuality <> @NewQuality)
								BEGIN
									DECLARE @VariantType NVARCHAR(20)
											, @BinaryFloat VARBINARY(8)
											, @BinaryReal VARBINARY(4)
											, @Float FLOAT
											, @Real FLOAT
											, @ProcessVariableTypeName NVARCHAR(100)
									SET @Sql = ''
									IF @ColumnName = 'SIValue'
										SET @Sql = 'SELECT @VariantType = DataBaseType FROM [lookup].[tblVariantType] WHERE VariantTypeIndex = (SELECT LookupSIValueVariantTypeIndex FROM [fmaudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID)'
									ELSE IF @ColumnName = 'Maximum'
										SET @Sql = 'SELECT @VariantType = DataBaseType FROM [lookup].[tblVariantType] WHERE VariantTypeIndex = (SELECT LookupMaximumVariantTypeIndex FROM [fmaudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID)'
									ELSE IF @ColumnName = 'Minimum'
										SET @Sql = 'SELECT @VariantType = DataBaseType FROM [lookup].[tblVariantType] WHERE VariantTypeIndex = (SELECT LookupMinimumVariantTypeIndex FROM [fmaudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID)'
								
									EXEC sp_executesql @statement=@sql,@params=N'@VariantType NVARCHAR(20) OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@VariantType=@VariantType OUTPUT, @_AuditGUID = @_AuditGUID 
													
									IF @VariantType = 'Float'
									OR @VariantType = 'Real'
									BEGIN
										SET @Sql = 'SELECT @ProcessVariableTypeName = ProcessVariableTypeName FROM [lookup].[tblProcessVariableType] WHERE ProcessVariableTypeIndex = (SELECT LookupProcessVariableTypeIndex FROM [fmaudit].[' + @TableName + '] WHERE _AuditEventSequence = 1 AND _AuditGUID = @_AuditGUID)'
										EXEC sp_executesql @statement=@sql,@params=N'@ProcessVariableTypeName NVARCHAR(100) OUTPUT, @_AuditGuid UNIQUEIDENTIFIER',@ProcessVariableTypeName=@ProcessVariableTypeName OUTPUT, @_AuditGUID = @_AuditGUID 

										SET @Sql = NULL

										IF @ProcessVariableTypeName LIKE('%LEVEL%')
											SET @Sql = 'SELECT @EngineeringUnitsIndex = LevelUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%TEMPERATURE%')
											SET @Sql = 'SELECT @EngineeringUnitsIndex = TemperatureUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%DENSITY%')
											SET @Sql = 'SELECT @EngineeringUnitsIndex = DensityUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%VOLUME%')
										OR @ProcessVariableTypeName LIKE('%FLOW TOTAL%') 
											SET @Sql = 'SELECT @EngineeringUnitsIndex = VolumeUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%FLOW%')
											SET @Sql = 'SELECT @EngineeringUnitsIndex = FlowUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%MASS%')
											SET @Sql = 'SELECT @EngineeringUnitsIndex = MassUnitIndex FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'

										IF @Sql IS NOT NULL
											EXEC sp_executesql @statement=@sql,@params=N'@EngineeringUnitsIndex INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@EngineeringUnitsIndex=@EngineeringUnitsIndex OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid

										IF @ProcessVariableTypeName LIKE('%LEVEL%')
											SET @Sql = 'SELECT @DecimalPlaces = LevelDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%TEMPERATURE%')
											SET @Sql = 'SELECT @DecimalPlaces = TemperatureDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%DENSITY%')
											SET @Sql = 'SELECT @DecimalPlaces = DensityDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%FLOW%')
											SET @Sql = 'SELECT @DecimalPlaces = FlowDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%VOLUME%')
											SET @Sql = 'SELECT @DecimalPlaces = VolumeDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'
										ELSE IF @ProcessVariableTypeName LIKE('%MASS%')
											SET @Sql = 'SELECT @DecimalPlaces = MassDecimalPlaces FROM [dbo].[tblSitesShadow] WHERE SiteGuid = @_AuditSiteGuid'

										IF @Sql IS NOT NULL
											EXEC sp_executesql @statement=@sql,@params=N'@DecimalPlaces INT OUTPUT, @_AuditSiteGuid UNIQUEIDENTIFIER',@DecimalPlaces=@DecimalPlaces OUTPUT, @_AuditSiteGuid=@_AuditSiteGuid
									END
																															
									IF @OldQuality IS NULL
									OR @OldQuality = 0xC0
									OR @OldQuality = 0xD8
									BEGIN
										IF @OldVarBinary IS NULL
											SET @OldString = 'NULL'
										ELSE IF @VariantType = 'SmallInt'
											SET @OldString = CONVERT(NVARCHAR(20),CONVERT(SMALLINT,@OldVarBinary))
										ELSE IF @VariantType = 'Int'
											SET @OldString = CONVERT(NVARCHAR(20),CONVERT(INT,@OldVarBinary))
										ELSE IF @VariantType = 'BigInt'
											SET @OldString = CONVERT(NVARCHAR(20),CONVERT(BIGINT,@OldVarBinary))
										ELSE IF @VariantType = 'Real'
										BEGIN
											SET @BinaryReal = CONVERT(VARBINARY(4),REVERSE(CONVERT(VARBINARY(4),@OldVarBinary)))
											SET @Real = (SELECT SIGN(CAST(@BinaryFloat AS INT))   * (1.0 + (CAST(@BinaryFloat AS INT) &  0x007FFFFF) * POWER(CAST(2 AS REAL), -23))   * POWER(CAST(2 AS REAL), (CAST(@BinaryFloat AS INT) & 0x7f800000) / 0x00800000 - 127)) 

											
											BEGIN TRY
												IF @EngineeringUnitsIndex IS NOT NULL
													SET @Real = dbo.udf_ConvertFromSIUnits(@Real,@EngineeringUnitsIndex,@DecimalPlaces)

												SET @OldString = dbo.udf_FormatProcessVariable(@Real,@EngineeringUnitsIndex)
											END TRY
											BEGIN CATCH
												SET @OldString = 'Out of Range'
											END CATCH

										END
										ELSE IF @VariantType = 'NVarChar'
											SET @OldString = CONVERT(NVARCHAR(20),@OldVarBinary)
										ELSE IF @VariantType = 'Float'
										BEGIN
											SET @BinaryFloat = CONVERT(VARBINARY(8),REVERSE(CONVERT(VARBINARY(8),@OldVarBinary)))
											SET @Float = (SELECT SIGN(CAST(@BinaryFloat AS BIGINT)) * (1.0 + (CAST(@BinaryFloat AS BIGINT) & 0x000FFFFFFFFFFFFF) * POWER(CAST(2 AS FLOAT), -52)) * POWER(CAST(2 AS FLOAT), (CAST(@BinaryFloat AS BIGINT) & 0x7ff0000000000000) / 0x0010000000000000 - 1023)) 

											BEGIN TRY
												IF @EngineeringUnitsIndex IS NOT NULL
													SET @Float = dbo.udf_ConvertFromSIUnits(@Float,@EngineeringUnitsIndex,@DecimalPlaces)

												SET @OldString = dbo.udf_FormatProcessVariable(@Float,@EngineeringUnitsIndex)
											END TRY
											BEGIN CATCH
												SET @OldString = 'Out of Range'
											END CATCH
										END
										ELSE IF @VariantType = 'DateTime'
											SET @OldString = CONVERT(NVARCHAR(20),CONVERT(DATETIME,@OldVarBinary))
									END
									ELSE
										SET @OldString = 'Bad'
									
									IF @NewQuality IS NULL
									OR @NewQuality = 0xC0
									OR @NewQuality = 0xD8
									BEGIN
										IF @NewVarBinary IS NULL
											SET @NewString = 'NULL'
										ELSE IF @VariantType = 'SmallInt'
											SET @NewString = CONVERT(NVARCHAR(20),CONVERT(SMALLINT,@NewVarBinary))
										ELSE IF @VariantType = 'Int'
											SET @NewString = CONVERT(NVARCHAR(20),CONVERT(INT,@NewVarBinary))
										ELSE IF @VariantType = 'BigInt'
											SET @NewString = CONVERT(NVARCHAR(20),CONVERT(BIGINT,@NewVarBinary))
										ELSE IF @VariantType = 'Real'
										BEGIN
											SET @BinaryReal = CONVERT(VARBINARY(4),REVERSE(CONVERT(VARBINARY(4),@NewVarBinary)))
											SET @Real = (SELECT SIGN(CAST(@BinaryFloat AS INT))   * (1.0 + (CAST(@BinaryFloat AS INT) &  0x007FFFFF) * POWER(CAST(2 AS REAL), -23))   * POWER(CAST(2 AS REAL), (CAST(@BinaryFloat AS INT) & 0x7f800000) / 0x00800000 - 127)) 

											IF @EngineeringUnitsIndex IS NOT NULL
												SET @Real = dbo.udf_ConvertFromSIUnits(@Real,@EngineeringUnitsIndex,@DecimalPlaces)

											SET @NewString = dbo.udf_FormatProcessVariable(@Real,@EngineeringUnitsIndex)
										END
										ELSE IF @VariantType = 'NVarChar'
											SET @NewString = CONVERT(NVARCHAR(20),@NewVarBinary)
										ELSE IF @VariantType = 'Float'
										BEGIN
											SET @BinaryFloat = CONVERT(VARBINARY(8),REVERSE(CONVERT(VARBINARY(8),@NewVarBinary)))
											SET @Float = (SELECT SIGN(CAST(@BinaryFloat AS BIGINT)) * (1.0 + (CAST(@BinaryFloat AS BIGINT) & 0x000FFFFFFFFFFFFF) * POWER(CAST(2 AS FLOAT), -52)) * POWER(CAST(2 AS FLOAT), (CAST(@BinaryFloat AS BIGINT) & 0x7ff0000000000000) / 0x0010000000000000 - 1023)) 

											BEGIN TRY
												IF @EngineeringUnitsIndex IS NOT NULL
													SET @Real = dbo.udf_ConvertFromSIUnits(@Real,@EngineeringUnitsIndex,@DecimalPlaces)

												SET @NewString = dbo.udf_FormatProcessVariable(@Real,@EngineeringUnitsIndex)
											END TRY
											BEGIN CATCH
												SET @NewString = 'Out of Range'
											END CATCH
										END
										ELSE IF @VariantType = 'DateTime'
											SET @NewString = CONVERT(NVARCHAR(20),CONVERT(DATETIME,@NewVarBinary))
									END
									ELSE
										SET @NewString = 'Bad' 										
								END
							END
							-- the varbinary columns in tblSyncClientConfiguration are passwords so we do not want to show any values in the audit log
							IF @TableName = 'tblSyncClientConfiguration' 
							BEGIN
								SET @OldString = '*******'
								SET @NewString = '********'
							END	

							IF @OldString <> @NewString
							BEGIN

								SET @Sql = 'INSERT INTO [dbo].[tblAuditLog] (SessionID, ActionID, TypeID, ID, PropertyID, NewValue, OldValue, CreatedDate, CreatedBy, ParentTypeID, SiteGuid, AuditContext, SourceNode) VALUES ('
											+ 'CONVERT(NVARCHAR(50), @SessionID), @ActionID, @TypeID, isnull(@ID,''unknown table''), REPLACE(REPLACE(@PropertyID,'' Guid'',''''),'' Index'',''''), @NewValue, @OldValue, @CreatedDate, @CreatedBy, @ParentTypeID, @SiteGuid, @AuditContext, @SourceNode)'
	
								SET @SqlParams = '@SessionID UNIQUEIDENTIFIER,
									@ActionID NVARCHAR(20),
									@TypeID NVARCHAR(50),
									@ID NVARCHAR(256),
									@PropertyID NVARCHAR(50),
									@NewValue NVARCHAR(2000),
									@OldValue NVARCHAR(2000),
									@CreatedDate DATETIMEOFFSET(7),
									@CreatedBy udtUserID,
									@ParentTypeID NVARCHAR(50),
									@SiteGuid UNIQUEIDENTIFIER,
									@SourceNode NVARCHAR(256),
									@AuditContext UNIQUEIDENTIFIER'
								
--								PRINT @sql
								EXEC sp_executesql @Sql, @SqlParams, 
									@SessionID = @_AuditSessionGuid,
									@ActionID = 'Modify',
									@TypeID = @TypeID,
									@ID = @ID,
									@PropertyID = @PropertyID,
									@NewValue = @NewString,
									@OldValue = @OldString,
									@CreatedDate = @_AuditCreatedDate,
									@CreatedBy = @_AuditUserID,
									@ParentTypeID = @ParentTypeID,
									@SiteGuid = @SiteGuid,
									@SourceNode = @SourceNode,
									@AuditContext = @_AuditContext 
							END
						
							FETCH NEXT FROM ColumnCursor INTO @ColumnName,@Type
						END

						CLOSE ColumnCursor
						DEALLOCATE ColumnCursor
					END

					-- Delete Audit Operation
					ELSE IF @_AuditEventType = 'D'
					BEGIN
						SET @Sql = 'INSERT INTO [dbo].[tblAuditLog] (SessionID, ActionID, TypeID, ID, CreatedDate, CreatedBy, ParentTypeID, SiteGuid, SourceNode, AuditContext) VALUES ('
									+ 'CONVERT(NVARCHAR(50), @SessionID), @ActionID, @TypeID, isnull(@ID,''unknown table''), @CreatedDate, @CreatedBy, @ParentTypeID, @SiteGuid, @SourceNode, @AuditContext)'
	
						SET @SqlParams = '@SessionID UNIQUEIDENTIFIER,
							@ActionID NVARCHAR(20),
							@TypeID NVARCHAR(50),
							@ID NVARCHAR(256),
							@CreatedDate DATETIMEOFFSET(7),
							@CreatedBy udtUserID,
							@ParentTypeID NVARCHAR(50),
							@SiteGuid UNIQUEIDENTIFIER,
							@SourceNode NVARCHAR(256),
							@AuditContext UNIQUEIDENTIFIER'
								
--						PRINT @sql
						EXEC sp_executesql @Sql, @SqlParams, 
							@SessionID = @_AuditSessionGuid,
							@ActionID = 'Purge',
							@TypeID = @TypeID,
							@ID = @ID,
							@CreatedDate = @_AuditCreatedDate,
							@CreatedBy = @_AuditUserID,
							@ParentTypeID = @ParentTypeID,
							@SiteGuid = @SiteGuid,
							@SourceNode = @SourceNode,
							@AuditContext = @_AuditContext
					END
				END
			END
	
			SET @Sql='DELETE FROM [fmAudit].['+@TableName+'] WHERE _AuditGuid = @_AuditGuid'
--			PRINT @Sql
			SET @SqlParams = '@_AuditGuid UNIQUEIDENTIFIER'
			EXEC sp_executesql @Sql, @SqlParams, @_AuditGuid=@_AuditGuid
--			PRINT '***END FETCH NEXT FROM AuditCursor'
--			PRINT ''

			FETCH NEXT FROM AuditCursor INTO @TableName, @_AuditGUID, @_AuditEventType, @_AuditSiteGuid, @_AuditSessionGuid, @_AuditCreatedDate, @_AuditUserID, @_AuditContext, @TypeID, @ParentTypeID, @IDQuery, @SiteGuidQuery
		END
		CLOSE AuditCursor
		DEALLOCATE AuditCursor

		DROP TABLE #ToProcessAudit
		drop table #differencetable

	END
END
GO
