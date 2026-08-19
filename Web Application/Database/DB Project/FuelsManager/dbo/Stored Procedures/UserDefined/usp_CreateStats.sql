CREATE PROCEDURE [dbo].[usp_CreateStats]
@indexonly CHAR (9)='NO', @fullscan CHAR (9)='NO', @norecompute CHAR (12)='NO'
AS
BEGIN
	RAISERROR('Creating any missing statistics', 10, 1) WITH NOWAIT
	
	-- NOTE: This sp will create statistics for *all* columns of all tables
	-- which the user has the privilege to create stats on (sysadmin, dbo, owner).
	-- The following columns are not considered
	-- - first column of an index
	-- - column which already has statistics
	-- - XML columns
	DECLARE @sysadmin INT,
			  @dbname sysname

	SELECT @indexonly = UPPER(@indexonly collate Latin1_General_CI_AS)
	
	-- Validate input options
	IF(@indexonly NOT IN('INDEXONLY', 'NO')
	OR UPPER(@fullscan) NOT IN('FULLSCAN', 'NO')
	OR UPPER(@norecompute) NOT IN('NORECOMPUTE', 'NO'))
	BEGIN
		RAISERROR(15600, -1, -1, 'usp_createstats')
		RETURN @@error
	END
	
	-- remember dbname
	SELECT @dbname = db_name()
	
	-- create temporary table (column, index position)
	CREATE TABLE #colpostab
	(
		col_name sysname collate database_default,
		col_pos INT,
	)
	
	SET nocount ON

	-- required for usp_createstats so it can create stats on on ICC/IVs
	SET ansi_warnings ON
	SET ansi_padding ON
	SET arithabort ON
	SET concat_null_yields_null ON
	SET numeric_roundabort OFF

	DECLARE @exec_stmt nvarchar(4000)
	DECLARE @tablename sysname
	DECLARE @columnname sysname
	DECLARE @indexname sysname
	DECLARE @uid INT
	DECLARE @indid SMALLINT
	DECLARE @position SMALLINT
	DECLARE @table_id INT
	DECLARE @schema_name sysname
	DECLARE @numcols INT
	-- number of eligible columns found
	DECLARE @msg nvarchar(388)
	-- adding two more chars for msg with 15654
	DECLARE @twopart_tablename nvarchar(517)
	DECLARE @timestamp VARCHAR(17)
	DECLARE @tablename_header VARCHAR(267)

	DECLARE ms_crs_tnames CURSOR local static FOR
		SELECT o.name,
				 o.object_id,
				 s.principal_id,
				 s.name
		  FROM sys.objects o
		  JOIN sys.schemas s
			 ON s.schema_id = o.schema_id
		 WHERE o.type      = 'U'
			 OR o.type      = 'IT'
		 
	SET @numcols = 0
	
	-- cannot execute against R/O databases
	IF DATABASEPROPERTYEX(db_name(), N'Updateability') = N'READ_ONLY'
	BEGIN
		RAISERROR(15635, -1, -1, N'usp_createstats')
		RETURN(1)
	END
	
	DECLARE @nTableCount INT
	SET @nTableCount = 0
	
	OPEN ms_crs_tnames

	FETCH NEXT
	 FROM ms_crs_tnames
	 INTO @tablename,
			@table_id,
			@uid,
			@schema_name
			
	WHILE (@@fetch_status <> -1)
	BEGIN
		SET @nTableCount = @nTableCount + 1
		
		-- check for table with disabled clustered index
		IF (1 = ISNULL((SELECT is_disabled FROM sys.indexes WHERE object_id = @table_id AND index_id = 1), 0))
		BEGIN
			-- raiserror('Table ''%s'': cannot perform the operation on the table because its clustered index is disabled', -1, -1, @tablename)
			-- note that we cannot use '%s' in the sqlerrorcodes.h as the same error is reused by sp_create|updatestats and they have
			-- different formatting styles. This style is consistent with the rest of the messages in this SP
			SELECT @msg = '''' + @dbname + '.' + @schema_name + '.' + @tablename + '''' RAISERROR(15654, -1, -1, @msg)
		END
		ELSE
		BEGIN
			-- filter out local temp tables.
			IF ((@@fetch_status <> -2)
			AND (substring(@tablename, 1, 1) <> '#')
			AND
			(
				(is_member('db_owner') = 1)
				OR
				(is_member('ddl_admin') = 1)
				OR
				(is_member(user_name(@uid)) = 1)
				OR
				(user_id() = @uid)))
			BEGIN
				-- these are all columns for which the statistics will be created
				DECLARE ms_crs_cnames CURSOR local FOR
					SELECT c.name
					  FROM sys.columns c
					 WHERE c.object_id   = @table_id
						AND c.is_computed = 0
						AND (type_name(c.system_type_id) NOT IN('xml'))
						AND c.name NOT IN	(SELECT col_name FROM #colpostab WHERE col_pos = 1)
						AND ((c.name IN (SELECT col_name FROM #colpostab))
							OR
							 (@indexonly <> 'INDEXONLY'))

				-- populate temporary table of all (column, index position) tuples for this table
				TRUNCATE TABLE #colpostab
				
				-- for each index on the table, loop though all columns and insert rows
				-- open cursor over indexes
				DECLARE ms_crs_ind CURSOR local static FOR
					SELECT stats_id,
							 name
					  FROM sys.stats
					 WHERE object_id = @table_id
					 ORDER BY stats_id

				SELECT @twopart_tablename = quotename(@schema_name, '[') + '.' + quotename(@tablename, '[')

				OPEN ms_crs_ind
			
				FETCH ms_crs_ind
				 INTO @indid,
						@indexname
						
				-- if an index exists
				WHILE @@fetch_status >= 0
				BEGIN
					-- check if the index is not disabled
					-- if there is no entry (null) in sys.indexes for current @indid we are looking at the statistic (not index)
					-- we need to include the columns of that statistic in #colpostab
					IF (1 <> ISNULL((SELECT is_disabled
											 FROM sys.indexes
											WHERE object_id = @table_id
											  AND index_id = @indid), 0))
					BEGIN
						-- every index has at least one column at position 1
						INSERT INTO #colpostab VALUES
						(
							index_col(@twopart_tablename, @indid, 1),
							1
						)
						-- now try position 2 and beyond....
						SET @columnname = index_col(@twopart_tablename, @indid, 2)
						SET @position   = 2
						
						WHILE (@columnname IS NOT NULL)
						BEGIN
							INSERT INTO #colpostab 
							VALUES (@columnname, @position)
								SELECT @position   = @position +1

							SELECT @columnname = index_col(@twopart_tablename, @indid, @position)
						END
					END

					-- next index
					FETCH ms_crs_ind
					 INTO @indid,
							@indexname
				END
			
				CLOSE ms_crs_ind
				DEALLOCATE ms_crs_ind
				
				-- now go over all columns which are eligible for creating statistics
				-- and are not first columns of any index
				-- optionaly we test if they are covered by some index (as non-leading)
				OPEN ms_crs_cnames
				
				FETCH NEXT
				 FROM ms_crs_cnames
				 INTO @columnname

				IF @@fetch_status < 0
				BEGIN
					SELECT @msg = @dbname + '.' + @schema_name + '.' + @tablename
					--RAISERROR(15013, -1, -1, @msg)
				END
				ELSE
				BEGIN
					SELECT @msg = @dbname + '.' + @schema_name + '.' + @tablename
					--RAISERROR(15018, -1, -1, @msg)
				END
				
				WHILE @@fetch_status >= 0
				BEGIN
					SELECT @numcols = @numcols +1
					-- use the column name as the name for the statistics as well
					SELECT @exec_stmt = 'CREATE STATISTICS ' + quotename(@columnname, '[') + ' ON ' + quotename(@schema_name, '[') + '.' + quotename(@tablename, '[') + '(' + quotename(@columnname, '[') + ')'
					-- determining the correct suffix
					IF ((UPPER(@fullscan)         = 'FULLSCAN')
					AND (UPPER(@norecompute)      = 'NORECOMPUTE'))
						SELECT @exec_stmt          = @exec_stmt + ' WITH FULLSCAN, NORECOMPUTE'
					ELSE IF (UPPER(@fullscan)     = 'FULLSCAN')
						SELECT @exec_stmt          = @exec_stmt + ' WITH FULLSCAN'
					ELSE IF (UPPER(@norecompute)  = 'NORECOMPUTE')
						SELECT @exec_stmt          = @exec_stmt + ' WITH NORECOMPUTE'

					EXEC(@exec_stmt)
					--print 'Statement='+@exec_stmt

					--IF (@@error = 0)
					-- otherwise the CREATE STATS will give a message
					-- PRINT '     ' + @columnname
	
					FETCH NEXT
					 FROM ms_crs_cnames
					 INTO @columnname
				END

				CLOSE ms_crs_cnames
				DEALLOCATE ms_crs_cnames
			END
		END

		FETCH NEXT
		 FROM ms_crs_tnames
		 INTO @tablename,
				@table_id,
				@uid,
				@schema_name
	END
	
	DEALLOCATE ms_crs_tnames

	IF OBJECT_ID(N'tempdb..#colpostab') IS NULL
	BEGIN
		DROP TABLE [#colpostab]
	END
	
	RAISERROR('Statistics have been created for %d columns in %d tables.', 10, 1, @numcols, @nTableCount) WITH NOWAIT
	RETURN(0)
END